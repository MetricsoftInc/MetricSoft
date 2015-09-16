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
                        viewList = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM")
                                    where (v.COMPANY_ID == companyID)
                                    select v).ToList();
                    else
                        viewList = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM")
                                    where (v.PERSPECTIVE == perspective && v.COMPANY_ID == companyID)
                                    select v).ToList();
                }
            }

            return viewList;
        }

        public static List<PERSPECTIVE_VIEW> SelectFilteredViewList(string perspective, decimal personID, decimal companyID, decimal busOrgID, decimal plantID, bool activeOnly)
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
            return outList;
        }

        public static PERSPECTIVE_VIEW LookupView(PSsqmEntities ctx, string perspective, string viewName, decimal viewID)
        {
            PERSPECTIVE_VIEW view = null;
            try
            {
                if (viewID > 0)
                {
                  view = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM")
                     where (v.VIEW_ID == viewID)
                     select v).Single();
                }
                else
                {
                   view = (from v in ctx.PERSPECTIVE_VIEW.Include("PERSPECTIVE_VIEW_ITEM")
                     where (v.PERSPECTIVE == perspective && v.VIEW_NAME == viewName)
                     select v).Single();
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


        public static PERSPECTIVE_TARGET_CALC CreateTargetCalc(decimal targetID)
        {
           PERSPECTIVE_TARGET_CALC tc = new PERSPECTIVE_TARGET_CALC();
           tc.TARGET_ID = targetID;
           tc.PERIOD_YEAR = tc.PERIOD_MONTH = 0;
           tc.VALUE_IND = false;
           return tc;
        }

        static PERSPECTIVE_TARGET_CALC LookupTargetCalc(PSsqmEntities ctx, decimal targetID, decimal plantID, int year, int month, bool createNew)
        {
            PERSPECTIVE_TARGET_CALC tc = null;

            try
            {
                tc = (from c in ctx.PERSPECTIVE_TARGET_CALC 
                          where (c.TARGET_ID == targetID  &&  c.PLANT_ID == plantID  &&  c.PERIOD_YEAR == year  &&  c.PERIOD_MONTH == month)
                          select c).Single();
            }
            catch
            { ; }

            if (tc == null && createNew)
            {
                tc = CreateTargetCalc(targetID);
                tc.PLANT_ID = plantID;
                tc.PERIOD_YEAR = year;
                tc.PERIOD_MONTH = month;
            }

            return tc;
        }

        public static List<PERSPECTIVE_TARGET_CALC> SelectTargetCalcList(PSsqmEntities ctx, decimal plantID, int year, int month)
        {
            List<PERSPECTIVE_TARGET_CALC> tcList = new List<PERSPECTIVE_TARGET_CALC>();

            try
            {
                if (plantID > 0)
                {
                    tcList = (from c in ctx.PERSPECTIVE_TARGET_CALC
                              where (c.PERIOD_YEAR == year && c.PERIOD_MONTH == month && (c.PLANT_ID == plantID ||  c.PLANT_ID == null))
                              select c).ToList();
                }
                else
                {
                    tcList = (from c in ctx.PERSPECTIVE_TARGET_CALC
                              where (c.PERIOD_YEAR == year && c.PERIOD_MONTH == month)
                              select c).OrderBy(l=> l.PLANT_ID).ToList();
                }
            }
            catch (Exception e)
            {
           //     SQMLogger.LogException(e);
            }

            return tcList;
        }


        public static PERSPECTIVE_TARGET_CALC UpdateTargetCalc(PSsqmEntities ctx, PERSPECTIVE_TARGET_CALC tc, string updateBy)
        {
            PERSPECTIVE_TARGET_CALC ret = null;

            tc = (PERSPECTIVE_TARGET_CALC)SQMModelMgr.SetObjectTimestamp(tc, updateBy, tc.EntityState);
            if (tc.EntityState == EntityState.Detached)
                ctx.AddToPERSPECTIVE_TARGET_CALC(tc);
            if (ctx.SaveChanges() > 0)
                ret = tc;

            return ret;
        }

        public static int UpdateTargetCalcList(PSsqmEntities ctx, List<PERSPECTIVE_TARGET_CALC> tcList, string updateBy)
        {
            int status = 0;

            try
            {
                for (int n = 0; n < tcList.Count; n++)
                {
                    PERSPECTIVE_TARGET_CALC tc = tcList[n];
                    if (tc.EntityState != EntityState.Unchanged)
                    {
                        tc = (PERSPECTIVE_TARGET_CALC)SQMModelMgr.SetObjectTimestamp(tc, updateBy, tc.EntityState);
                        if (tc.EntityState == EntityState.Detached)
                            ctx.AddToPERSPECTIVE_TARGET_CALC(tc);
                    }
                }
                status = ctx.SaveChanges();
            }
            catch (Exception e)
            {
               // SQMLogger.LogException(e);
            }

            return status;
        }

        #endregion
    }

    #region targetkpimgr
    public class TargetCalcsMgr
    {
        public decimal CompanyID
        {
            get;
            set;
        }
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
        public List<PERSPECTIVE_TARGET> TargetList
        {
            get;
            set;
        }
        public CalcsResult Results
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }

        public TargetCalcsMgr CreateNew(decimal companyID, DateTime fromDate, DateTime toDate)
        {
            this.Entities = new PSsqmEntities();
            this.CompanyID = companyID;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.DateInterval = DateIntervalType.fuzzy;
            this.TargetList = ViewModel.SelectTargets(this.Entities, companyID, fromDate.Year);
            this.Results = new CalcsResult().Initialize();

            return this;
        }

        public TargetCalcsMgr InitCalc()
        {
            this.Results = new CalcsResult();
            this.Results.Status = 0;
            return this;
        }

        public PERSPECTIVE_TARGET GetTarget(decimal targetID, string calcScope)
        {
            if (targetID > 0)
                return this.TargetList.Where(l => l.TARGET_ID == targetID).FirstOrDefault();
            else
                return this.TargetList.Where(l => l.CALCS_SCOPE == calcScope).FirstOrDefault();
        }

        public DateTime LastCalcDate()
        {
            DateTime lastUpdDt;

            try
            {
                lastUpdDt = (DateTime)(from c in this.Entities.PERSPECTIVE_TARGET_CALC
                             select c.LAST_UPD_DT).Max();
            }
            catch
            {
                lastUpdDt = DateTime.MinValue;
            }

            return lastUpdDt;
        }

        public TargetCalcsMgr LoadCurrentMetrics(bool periodYearOnly, bool companyOverallOnly, DateIntervalType dateIntervalType)
        {
            this.DateInterval = dateIntervalType;
 
            try
            {
                decimal[] targetArray = this.TargetList.Select(l=> l.TARGET_ID).ToArray();
                List<PERSPECTIVE_TARGET_CALC> tcList;
                if (dateIntervalType == DateIntervalType.month)
                {
                    tcList = (from c in this.Entities.PERSPECTIVE_TARGET_CALC
                              where (targetArray.Contains(c.TARGET_ID) &&  c.PERIOD_YEAR == this.ToDate.Year && c.PERIOD_MONTH == this.ToDate.Month) 
                              select c).OrderByDescending(l => l.TARGET_ID).ToList();
                }
                else if (dateIntervalType == DateIntervalType.year)
                {
                    tcList = (from c in this.Entities.PERSPECTIVE_TARGET_CALC
                                where(targetArray.Contains(c.TARGET_ID) && EntityFunctions.CreateDateTime(c.PERIOD_YEAR,c.PERIOD_MONTH,1,0,0,0) >= this.FromDate &&  EntityFunctions.CreateDateTime(c.PERIOD_YEAR,c.PERIOD_MONTH,1,0,0,0) <= this.ToDate) 
                                select c).OrderByDescending(c => EntityFunctions.CreateDateTime(c.PERIOD_YEAR, c.PERIOD_MONTH, 1, 0, 0, 0)).ThenBy(l => l.TARGET_ID).ToList();
                }
                else
                {
                    tcList = (from c in this.Entities.PERSPECTIVE_TARGET_CALC
                                where (targetArray.Contains(c.TARGET_ID))
                                select c).OrderByDescending(c => EntityFunctions.CreateDateTime(c.PERIOD_YEAR, c.PERIOD_MONTH, 1, 0, 0, 0)).ThenBy(l => l.TARGET_ID).ToList();
                }

                if (tcList != null)
                {
                    foreach (PERSPECTIVE_TARGET_CALC tc in tcList)
                    {
                        this.TargetList.Where(t => t.TARGET_ID == tc.TARGET_ID).FirstOrDefault().PERSPECTIVE_TARGET_CALC.Add(tc);
                    }
                }
            }
            catch (Exception e)
            {
                //   SQMLogger.LogException(e);
            }

            return this;
        }

        public int TargetMetric(string targetScope, decimal[] plantIDS, DateTime targetDate)
        {
            int status = 0;
            this.Results.ValidResult = false;
  
            try
            {
                PERSPECTIVE_TARGET target = this.TargetList.Where(t => t.CALCS_SCOPE == targetScope).FirstOrDefault();
                PERSPECTIVE_TARGET_CALC tc = null;

                if (targetDate > DateTime.MinValue)
                {   // get specific date period
                    if (plantIDS.Length == 1)   // get specific plant
                        tc = target.PERSPECTIVE_TARGET_CALC.Where(l => l.PLANT_ID != null && plantIDS.Contains((decimal)l.PLANT_ID) && (l.PERIOD_YEAR == targetDate.Year && l.PERIOD_MONTH == targetDate.Month)).FirstOrDefault();
                    else
                        tc = target.PERSPECTIVE_TARGET_CALC.Where(l => l.PLANT_ID == null && (l.PERIOD_YEAR == targetDate.Year && l.PERIOD_MONTH == targetDate.Month)).FirstOrDefault();
                }
                else
                {   // get the most recent results
                    if (plantIDS.Length == 1)
                        tc = target.PERSPECTIVE_TARGET_CALC.Where(l => l.PLANT_ID != null && plantIDS.Contains((decimal)l.PLANT_ID) && l.VALUE_IND == true).FirstOrDefault();
                    else
                        tc = target.PERSPECTIVE_TARGET_CALC.Where(l => l.PLANT_ID == null &&  l.VALUE_IND == true).FirstOrDefault();
                       
                }
                if (tc != null)
                {
                    this.Results.Text = this.TargetList.Where(t => t.CALCS_SCOPE == targetScope).Select(t => t.DESCR_LONG).FirstOrDefault();
                    this.Results.ValidResult = (bool)tc.VALUE_IND;
                    this.Results.Result = (decimal)tc.VALUE;
                    this.Results.ResultDate = new DateTime(tc.PERIOD_YEAR, tc.PERIOD_MONTH, DateTime.DaysInMonth(tc.PERIOD_YEAR, tc.PERIOD_MONTH));
                }
            }
            catch
            {
                status = -1;
            }

            return status;
        }

        public int MetricSeries(EHSCalcsCtl.SeriesOrder seriesOrder, decimal[] plantIDS, string targetScope)
        {
            int status = 0;
            int item = 0;
            GaugeSeries series = null;
            this.Results.Initialize();
            PLANT plant = null;

            switch (seriesOrder)
            {
                case EHSCalcsCtl.SeriesOrder.PlantMeasure:
                    int numYears = this.ToDate.Year - this.FromDate.Year + 1;
                    int numPeriods = ((this.ToDate.Year - this.FromDate.Year) * 12) + (this.ToDate.Month - this.FromDate.Month) + 1;
                    foreach (decimal plantID in plantIDS)
                    {
                        try
                        {
                            if (plantID > 0)
                            {
                                plant = SQMModelMgr.LookupPlant(plantID);
                                series = new GaugeSeries().CreateNew(1, plant.PLANT_NAME, "");
                            }
                            else
                            {
                                series = new GaugeSeries().CreateNew(1, "overall", "");
                            }

                            for (int n = 0; n < numPeriods; n++)
                            {
                                DateTime thePeriod = this.FromDate.AddMonths(n);
                                if (TargetMetric(targetScope, new decimal[1] { plantID }, thePeriod) >= 0 && this.Results.ValidResult)
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, n + 1, 0, this.Results.Result, SQMBasePage.FormatDate(thePeriod, "yyyy/MM", false)));
                                else 
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, n + 1, 0, 0, SQMBasePage.FormatDate(thePeriod, "yyyy/MM", false)));
                            }

                            this.Results.metricSeries.Add(series);
                        }
                        catch
                        {
                        }
                    }
                    break;

                default:
                    series = new GaugeSeries().CreateNew(1, "", "");
                    foreach (decimal plantID in plantIDS)
                    {
                        plant = SQMModelMgr.LookupPlant(plantID);
                       
                        if (TargetMetric(targetScope, new decimal[1] { plantID }, DateTime.MinValue) >= 0 && this.Results.ValidResult)
                        {
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, item++, 0, this.Results.Result, plant.PLANT_NAME));
                            series.Name = this.Results.Text;
                        }
                    }
                    this.Results.metricSeries.Add(series);
                    break;
            }

            if (this.Results.metricSeries.Count > 0)
                this.Results.ValidResult = true;

            return status;
        }

        public PERSPECTIVE_VIEW CreateCorpTargetView(decimal companyID, int effYear, string viewTitle)
        {
            PERSPECTIVE_VIEW targetView = new PERSPECTIVE_VIEW();
            PERSPECTIVE_VIEW_ITEM vi = null;

            targetView.PERSPECTIVE = "0";
            targetView.DFLT_TIMEFRAME = 1;
            targetView.VIEW_DESC = viewTitle;
            int seq = -1;
            string perspective = "";
            foreach (PERSPECTIVE_TARGET pt in this.TargetList.OrderBy(l => l.PERSPECTIVE).ToList())
            {
                vi = new PERSPECTIVE_VIEW_ITEM();
                vi.ITEM_SEQ = ++seq;
                vi.DISPLAY_TYPE = 1;
                vi.SERIES_ORDER = 1;
                vi.MULTIPLIER = vi.SCALE_MIN = vi.SCALE_MAX = vi.SCALE_UNIT = 0;
                vi.CALCS_METHOD = pt.PERSPECTIVE;  // important diff
                vi.CALCS_STAT = pt.SSTAT;
                vi.CALCS_SCOPE = pt.CALCS_SCOPE;
                vi.TITLE = vi.A_LABEL = pt.DESCR_SHORT;
                vi.SCALE_LABEL = pt.DESCR_SHORT;
                vi.DISPLAY_TARGET_ID = pt.TARGET_ID;
               // vi.INDICATOR_1_VALUE = vi.INDICATOR_2_VALUE = vi.INDICATOR_3_VALUE = 0;
               // vi.INDICATOR_1_COLOR = vi.INDICATOR_2_COLOR = vi.INDICATOR_3_COLOR = "";
                vi.COLOR_PALLETE = vi.OPTIONS = "";
                vi.STATUS = "0";
                vi.ITEM_WIDTH = 280;
               // if (pt.PERSPECTIVE != perspective)
               //     vi.NEW_ROW = true;
               // else
                    vi.NEW_ROW = false;

                vi.CONTROL_TYPE = 60;  // vi.CONTROL_TYPE = 1;
                vi.ITEM_WIDTH = 185;
                vi.ITEM_HEIGHT = 150;
                vi.OPTIONS = "N";
                vi.SCALE_LABEL = "";
                vi.SCALE_MIN = -100; vi.SCALE_MAX = 100; vi.SCALE_UNIT = 25;

                perspective = pt.PERSPECTIVE;
                targetView.PERSPECTIVE_VIEW_ITEM.Add(vi);
            }

            return targetView;
        }
    }
    #endregion

}