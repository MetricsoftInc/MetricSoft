using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Objects;
using System.Text;
using System.Reflection;
using SQM.Shared;
using SQM.Website.Classes;

namespace SQM.Website
{
    public class ViewModel
    {
        #region view
        public static List<PERSPECTIVE_VIEW> SelectViewList(string perspective, decimal companyID)
        {
            List<PERSPECTIVE_VIEW> viewList = new List<PERSPECTIVE_VIEW>();
            if (perspective != "0")
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    if (string.IsNullOrEmpty(perspective))
						viewList = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM").Include("PERSPECTIVE_VIEW_LANG")
                                    where (v.COMPANY_ID == companyID)
                                    select v).ToList();
                    else
						viewList = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM").Include("PERSPECTIVE_VIEW_LANG")
                                    where (v.PERSPECTIVE == perspective && v.COMPANY_ID == companyID)
                                    select v).ToList();
                }
            }

            return viewList;
        }

		public static List<PERSPECTIVE_VIEW> SelectFilteredViewList(string perspective, decimal personID, decimal companyID, decimal busOrgID, decimal plantID, bool activeOnly, string nlsLanguage)
        {
            List<PERSPECTIVE_VIEW> viewList = SelectViewList(perspective, companyID);
            List<PERSPECTIVE_VIEW> outList = new List<PERSPECTIVE_VIEW>();

            foreach (PERSPECTIVE_VIEW view in viewList)
            {
                if (!activeOnly  ||  (activeOnly &&  view.STATUS != "I"))
                {
                    if (view.AVAILABILTY == 4 || view.OWNER_ID == personID)
                    {
                        outList.Add(view);
                    }
                    else if ((view.AVAILABILTY == 3 && view.BUS_ORG_ID == busOrgID) || (view.AVAILABILTY == 2 && view.PLANT_ID == plantID))
                    {
                        outList.Add(view);
                    }
                    else
                    {
                        ;
                    }
                }
            }

			// get alternate language texts
			if (nlsLanguage != "en")	
			{
				PERSPECTIVE_VIEW_LANG langView = null;
				{
					foreach (PERSPECTIVE_VIEW view in outList)
					{
						if ((langView = view.PERSPECTIVE_VIEW_LANG.Where(v => v.NLS_LANGUAGE.ToLower() == nlsLanguage.ToLower()).FirstOrDefault()) != null)
						{
							view.VIEW_NAME = langView.VIEW_NAME;
							view.VIEW_DESC = langView.VIEW_DESC;
						}
					}
				}
			}

            return outList;
        }

		public static PERSPECTIVE_VIEW LookupView(PSsqmEntities ctx, string perspective, string viewName, decimal viewID, string nlsLanguage)
        {
            PERSPECTIVE_VIEW view = null;
            try
            {
                if (viewID > 0)
                {
					view = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM").Include("PERSPECTIVE_VIEW_LANG").Include("PERSPECTIVE_VIEW_ITEM.PERSPECTIVE_VIEW_ITEM_LANG") 
                     where (v.VIEW_ID == viewID)
                     select v).SingleOrDefault();
                }
                else
                {
					view = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM").Include("PERSPECTIVE_VIEW_LANG").Include("PERSPECTIVE_VIEW_ITEM.PERSPECTIVE_VIEW_ITEM_LANG") 
                     where (v.PERSPECTIVE == perspective && v.VIEW_NAME == viewName)
                     select v).SingleOrDefault();
                }


				if (view != null && nlsLanguage.ToLower() != "en")
				{
					PERSPECTIVE_VIEW_LANG lang = view.PERSPECTIVE_VIEW_LANG.Where(v => v.NLS_LANGUAGE.ToLower() == nlsLanguage.ToLower()).FirstOrDefault();
					if (lang != null)
					{
						view.VIEW_NAME = lang.VIEW_NAME;
						view.VIEW_DESC = lang.VIEW_DESC;
						PERSPECTIVE_VIEW_ITEM_LANG langItem = null;
						foreach (PERSPECTIVE_VIEW_ITEM item in view.PERSPECTIVE_VIEW_ITEM)
						{
							if ((langItem = item.PERSPECTIVE_VIEW_ITEM_LANG.Where(i => i.NLS_LANGUAGE.ToLower() == nlsLanguage.ToLower()).FirstOrDefault()) != null)
							{
								item.TITLE = langItem.TITLE;
								item.SCALE_LABEL = langItem.SCALE_LABEL;
								item.A_LABEL = langItem.A_LABEL;
								if (!string.IsNullOrEmpty(langItem.OPTIONS))
									item.OPTIONS = langItem.OPTIONS;
							}
						}
					}
				}

            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
            }

            return view;
        }

        public static PERSPECTIVE_VIEW CreateView(string perspective, decimal companyID, decimal busOrgID, decimal plantID, decimal ownerID)
        {
            PERSPECTIVE_VIEW view = new PERSPECTIVE_VIEW();

            view.PERSPECTIVE = perspective;
            view.COMPANY_ID = companyID;
            view.BUS_ORG_ID = busOrgID;
            view.PLANT_ID = plantID;
            view.OWNER_ID = ownerID;
            view.AVAILABILTY = 1;
            view.DFLT_OPTION = 0;
            view.STATUS = "N";

            return view;
        }

        public static PERSPECTIVE_VIEW_ITEM CreateViewItem(decimal viewID)
        {
            PERSPECTIVE_VIEW_ITEM vi = new PERSPECTIVE_VIEW_ITEM();
            vi.VIEW_ID = viewID;
            vi.ITEM_SEQ = 0;
            vi.CONTROL_TYPE = 10;
            vi.DISPLAY_TYPE = 1;
            vi.NEW_ROW = true;
            vi.SERIES_ORDER = 0;
            vi.CALCS_STAT = "sum";
            vi.CALCS_SCOPE = "";
            vi.OPTIONS = "N";
            vi.STATUS = "N";

            return vi;
        }

        public static PERSPECTIVE_VIEW UpdateView(SQM.Website.PSsqmEntities ctx, PERSPECTIVE_VIEW view, string updateBy)
        {
            try
            {

                if (view.STATUS == "D")
                    ctx.DeleteObject(view);
                else
                {
                    view = (PERSPECTIVE_VIEW)SQMModelMgr.SetObjectTimestamp((object)view, updateBy, view.EntityState);

                    if (view.EntityState == EntityState.Detached || view.EntityState == EntityState.Added)
                    {
                        view.STATUS = "A";
                        ctx.AddToPERSPECTIVE_VIEW(view);
                    }

                    foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.ToList())
                    {
                        if (vi.STATUS == "D" || vi.ITEM_SEQ < 1)
                            ctx.DeleteObject(vi);
                        else
                        {
                            if (vi.EntityState == EntityState.Detached || vi.EntityState == EntityState.Added)
                            {
                                vi.STATUS = "A";
                                vi.CALCS_SCOPE = view.PERSPECTIVE;
                                ctx.AddToPERSPECTIVE_VIEW_ITEM(vi);
                            }
                        }
                    }
                }
 
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return view;
        }

        public static bool HasArrays(PERSPECTIVE_VIEW view)
        {
            // does the view contain any array controls ?
            bool hasArrays = false;

            foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM)
            {
                if (vi.DISPLAY_TYPE.HasValue  &&  vi.DISPLAY_TYPE == 2)
                {
                    hasArrays = true;
                    break;
                }
            }
            return hasArrays;
        }
        public static bool HasSections(PERSPECTIVE_VIEW view)
        {
            // does the view contain any section areas ?
            bool hasArrays = false;

            foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM)
            {
                if (vi.CONTROL_TYPE == 1)
                {
                    hasArrays = true;
                    break;
                }
            }
            return hasArrays;
        }

        public static int AddFromYear(PERSPECTIVE_VIEW view)
        {
            int numYears = 0;
            if (view.PERSPECTIVE == "0")
            {
                numYears = -1;
            }
            else
            {
                foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM)
                {
                    if (vi.CALCS_STAT == SStat.pctChange.ToString() || vi.CALCS_STAT == SStat.pctReduce.ToString() || vi.SERIES_ORDER == Convert.ToInt16(EHSCalcsCtl.SeriesOrder.YearPlant) || vi.SERIES_ORDER == Convert.ToInt16(EHSCalcsCtl.SeriesOrder.YearPlantAvg) || vi.SERIES_ORDER == Convert.ToInt16(EHSCalcsCtl.SeriesOrder.PeriodMeasureYOY) | vi.SERIES_ORDER == Convert.ToInt16(EHSCalcsCtl.SeriesOrder.PeriodSumYOY))
                    {
                        numYears = -1;
                    }
                }
            }

            return numYears;
        }

        #endregion

        #region images
        public static string GetViewImageStyle(PERSPECTIVE_VIEW view)
        {
            string style = "";

            if (view.PERSPECTIVE_VIEW_ITEM.Where(l => l.CONTROL_TYPE == 50).ToList().Count > 0)
            {
                style = "buttonPie";
            }
            else if (view.PERSPECTIVE_VIEW_ITEM.Where(l => l.CONTROL_TYPE == 10  ||  l.CONTROL_TYPE == 32  ||  l.CONTROL_TYPE == 34).ToList().Count > 0)
            {
                style = "buttonGraph";
            }
            else 
            {
                style = "buttonChart"; 
            }

            return style;
        }

        public static string GetViewImageURL(PERSPECTIVE_VIEW view)
        {
            string url = "";

            if (view.PERSPECTIVE_VIEW_ITEM.Where(l => l.CONTROL_TYPE == 50).ToList().Count > 0)
            {
                url = "/images/defaulticon/16x16/statistics-pie-chart.png";
            }
            else if (view.PERSPECTIVE_VIEW_ITEM.Where(l => l.CONTROL_TYPE == 10 || l.CONTROL_TYPE == 32 || l.CONTROL_TYPE == 34).ToList().Count > 0)
            {
                url = "/images/defaulticon/16x16/activity.png";
            }
            else
            {
                url = "/images/defaulticon/16x16/statistics-chart.png";
            }

            return url;
        }

        public static string GetViewItemImageURL(PERSPECTIVE_VIEW_ITEM vi)
        {
            string url = "";

            switch (vi.CONTROL_TYPE)
            {
                case 10:
                case 32:
                case 34:
                    url = "/images/defaulticon/16x16/activity.png";
                    break;
                case 50:
                    url = "/images/defaulticon/16x16/statistics-pie-chart.png";
                    break;
                default:
                    url = "/images/defaulticon/16x16/statistics-chart.png";
                    break;
            }

            return url;
        }
 

        #endregion

        #region target

        public static List<PERSPECTIVE_TARGET> SelectTargets(PSsqmEntities ctx, decimal companyID, int effectiveYear)
        {
            List<PERSPECTIVE_TARGET> targetList = new List<PERSPECTIVE_TARGET>();
            try
            {
                if (effectiveYear == 0)
                {
                    targetList = (from v in ctx.PERSPECTIVE_TARGET
                                  where (v.COMPANY_ID == companyID)
                                  select v).OrderBy(l=> l.EFF_YEAR).ToList();
                }
                else
                {
                    targetList = (from v in ctx.PERSPECTIVE_TARGET
                                  where (v.COMPANY_ID == companyID && v.EFF_YEAR == effectiveYear)
                                  select v).ToList();
                    // try to select prior year if current does not exist 
                    if (targetList == null || targetList.Count == 0)
                        targetList = (from v in ctx.PERSPECTIVE_TARGET
                                      where (v.COMPANY_ID == companyID && v.EFF_YEAR == (effectiveYear - 1))
                                      select v).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return targetList;
        }

        public static PERSPECTIVE_TARGET LookupTarget(PSsqmEntities ctx, decimal targetID)
        {
            PERSPECTIVE_TARGET target = null;
            try
            {
                target = (from t in ctx.PERSPECTIVE_TARGET
                          where (t.TARGET_ID == targetID)
                          select t).Single();
            }
            catch
            { ; }

            return target;
        }


        public static PERSPECTIVE_TARGET UpdateTarget(PSsqmEntities ctx, PERSPECTIVE_TARGET target, string updateBy)
        {
            PERSPECTIVE_TARGET ret = null;

            target = (PERSPECTIVE_TARGET)SQMModelMgr.SetObjectTimestamp(target, updateBy, target.EntityState);
            if (target.EntityState == EntityState.Detached)
                ctx.AddToPERSPECTIVE_TARGET(target);
            if (ctx.SaveChanges() > 0)
                ret = target;

            return ret;
        }

        #endregion
    }

    #region targetkpimgr

	public class TargetStruct
	{
		public decimal? CompanyID
		{
			get;
			set;
		}
		public decimal? BusOrgID
		{
			get;
			set;
		}
		public decimal? PlantID
		{
			get;
			set;
		}
		public PERSPECTIVE_TARGET Target
		{
			get;
			set;
		}
	}

    public class TargetMgr
    {
        public DateTime FromDate
        {
            get;
            set;
        }
        public DateTime ToDate
        {
            get;
            set;
        }
        public DateIntervalType DateInterval
        {
            get;
            set;
        }
        public List<TargetStruct> TargetList
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }

        public TargetMgr CreateNew(string perspective, decimal companyID, DateTime fromDate, DateTime toDate)
        {
            this.Entities = new PSsqmEntities();
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.DateInterval = DateIntervalType.fuzzy;
			this.SelectTargets(perspective);

            return this;
        }

		public TargetMgr SelectTargets(string perspective)
		{
			this.TargetList = new List<TargetStruct>();
			string calcScope = "";

			if (perspective == "XD")
			{
				List<EHS_TARGETS> ehsTargets = (from t in Entities.EHS_TARGETS
												select t).ToList();
				// standardize EHS_TARGET to general target structure
				foreach (EHS_TARGETS et in ehsTargets)
				{
					TargetStruct ts = new TargetStruct() {CompanyID = et.COMPANY_ID, BusOrgID = et.BUS_ORG_ID, PlantID = et.PLANT_ID};
					switch (et.TYPE)
					{
						case "TRIR":
							calcScope = "rcr";
							break;
						case "FREQUENCY":
							calcScope = "ltcr";
							break;
						case "SEVERITY":
							calcScope = "ltsr";
							break;
						default:
							calcScope = et.TYPE;
							break;
					}

					ts.Target = new PERSPECTIVE_TARGET() {PERSPECTIVE = "XD", CALCS_SCOPE = calcScope, VALUE = et.TARGET_VALUE, MIN_MAX = et.MIN_MAX.HasValue ? et.MIN_MAX : 0, DESCR_SHORT = et.TYPE, EFF_YEAR = DateTime.UtcNow.Year };
					this.TargetList.Add(ts);
				}
			}
			else
			{
				List<PERSPECTIVE_TARGET> pvTargets = (from t in Entities.PERSPECTIVE_TARGET
													  where t.PERSPECTIVE == perspective 
													  select t).ToList();
				// insert PERSPECTIVE_TARGETS into target structure
				foreach (PERSPECTIVE_TARGET pt in pvTargets)
				{
					TargetStruct ts = new TargetStruct() { CompanyID = pt.COMPANY_ID };
					ts.Target = pt;
					this.TargetList.Add(ts);
				}
			}

			return this;
		}

        public PERSPECTIVE_TARGET GetTarget(string perspective, string calcScope, string calcStat, decimal companyID, decimal busOrgID, decimal plantID, int effYear)
        {
			PERSPECTIVE_TARGET target = null;

			bool byYear = effYear == 0 ? false : true;
			bool byStat = this.TargetList.Where(l => l.Target.SSTAT != null && l.Target.SSTAT != "").Count() > 0 ? true : false;
			bool byPlant = this.TargetList.Where(l => l.PlantID != null && l.PlantID > 0).Count() > 0  &&  plantID > 0 ? true : false;
			bool byBusOrg = this.TargetList.Where(l => l.BusOrgID != null && l.BusOrgID > 0).Count() > 0  &&  busOrgID > 0  && !byPlant ? true : false;
			bool byCompany = !byBusOrg && !byPlant ? true : false;		// finally default to getting the company targets

			this.TargetList.Where(l => l.PlantID != null && l.PlantID > 0).Count();

			if (byYear)
			{
				// get most current effyear target
				target = this.TargetList.Where(l => l.Target.PERSPECTIVE == perspective && l.Target.CALCS_SCOPE == calcScope
						&& (!byStat || l.Target.SSTAT == calcStat)
						&& (!byCompany || l.CompanyID == companyID)
						&& (!byBusOrg || l.BusOrgID == busOrgID)
						&& (!byPlant || l.PlantID == plantID)
						&&  l.Target.EFF_YEAR <= effYear
					).Select(l => l.Target).LastOrDefault();
			}
			else
			{
				target = this.TargetList.Where(l => l.Target.PERSPECTIVE == perspective && l.Target.CALCS_SCOPE == calcScope 
						&& (!byStat || l.Target.SSTAT == calcStat)
						&& (!byCompany || l.CompanyID == companyID)
						&& (!byBusOrg || l.BusOrgID == busOrgID)
						&& (!byPlant || l.PlantID == plantID)
					).Select(l => l.Target).FirstOrDefault();
			}

            return target;
        }
    }
    #endregion

}