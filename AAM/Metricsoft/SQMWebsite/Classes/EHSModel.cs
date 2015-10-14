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
//using SQM.Website.Classes;

namespace SQM.Website
{
    public enum EHSProfileStatus { Normal, NonExist, NoMeasures, NoInputs, InputError, OutOFRange, PeriodLimit, Incomplete};
	//public enum EHSMetricOption { None, NoValue, NoCost, Empty };


	#region ehsdata
	public class EHSDataMapping
	{
		public static EHS_DATA LookupEHSDataPeriod(PSsqmEntities ctx, decimal plantID, DateTime periodDate, decimal measureID, bool createNew)
		{
			EHS_DATA dataPeriod = null;

			dataPeriod = (from d in ctx.EHS_DATA
						  where d.PLANT_ID == plantID && d.DATE == periodDate.Date && d.MEASURE_ID == measureID
						  select d).SingleOrDefault();

			if (dataPeriod == null && createNew)
			{
				dataPeriod = new EHS_DATA();
				dataPeriod.MEASURE_ID = measureID;
				dataPeriod.PLANT_ID = plantID;
				dataPeriod.DATE = periodDate.Date;
			}

			return dataPeriod;
		}

		public static List<EHS_DATA> SelectEHSDataPeriodList(PSsqmEntities ctx, decimal plantID, DateTime periodDate, List<decimal> measureList, bool createNew)
		{
			List<EHS_DATA> dataList = new List<EHS_DATA>();

			dataList = (from d in ctx.EHS_DATA
							  where d.PLANT_ID == plantID && d.DATE == periodDate.Date && (measureList.Count == 0  ||  measureList.Contains(d.MEASURE_ID))
							  select d).ToList();

			if (createNew  &&  measureList != null)
			{
				EHS_DATA ehsData = null;
				foreach (decimal measureID in measureList)
				{
					if ((ehsData = dataList.Where(l => l.MEASURE_ID == measureID).SingleOrDefault()) == null)
					{
						ehsData = new EHS_DATA();
						ehsData.MEASURE_ID = measureID;
						ehsData.PLANT_ID = plantID;
						ehsData.DATE = periodDate.Date;
						dataList.Add(ehsData);
					}
				}
			}

			return dataList;
		}

		public static List<EHS_DATA> ClearEHSDataValues(List<EHS_DATA> dataList)
		{
			foreach (EHS_DATA ehsData in dataList)
			{
				ehsData.VALUE = null;
				ehsData.ATTRIBUTE = null;
			}

			return dataList;
		}

		public static int UpdateEHSDataList(PSsqmEntities ctx, List<EHS_DATA> dataList)
		{
			int status = 0;

			foreach (EHS_DATA ehsData in dataList)
			{
				if (ehsData.EntityState == EntityState.Added || ehsData.EntityState == EntityState.Detached)
				{
					if (ehsData.VALUE.HasValue || !string.IsNullOrEmpty(ehsData.ATTRIBUTE))
					{
						ctx.AddToEHS_DATA(ehsData);
					}
				}
			}

			status = ctx.SaveChanges();

			return status;
		}

		public static int SetEHSDataValue(List<EHS_DATA> dataList, decimal measureID, decimal addValue)
		{
			int status = -1;

			EHS_DATA ehsData = GetEHSData(dataList, measureID);
			if (ehsData != null)
			{
				if (ehsData.VALUE.HasValue)
					ehsData.VALUE += addValue;
				else
					ehsData.VALUE = addValue;
				status = 0;
			}

			return status;
		}

		public static EHS_DATA GetEHSData(List<EHS_DATA> dataPeriodList, decimal measureID)
		{
			return dataPeriodList.Where(l => l.MEASURE_ID == measureID).FirstOrDefault();
		}

		public static decimal GetMappedMeasure(List<EHS_MEASURE> measureList, string mappingFieldName)
		{
			decimal measureID = 0;
			EHS_MEASURE measure = measureList.Where(l => l.PLANT_ACCT_FIELD.ToUpper() == mappingFieldName.ToUpper()).SingleOrDefault();
			if (measure != null)
				measureID = measure.MEASURE_ID;

			return measureID;
		}
	}
	#endregion

	#region ehsmodel

	public class EHSModel
    {
        public static List<EFM_REFERENCE> SelectEFMTypeList(string efmCategory)
        {
            if (!string.IsNullOrEmpty(efmCategory))
                return SessionManager.EFMList.Where(l=> l.EFM_CATEGORY == efmCategory).ToList();
            else
                return SessionManager.EFMList;
        }

        public partial class Emissions
        {
            public string Location
            {
                get;
                set;
            }
            public List<GHG_FACTOR> GHGFactorList
            {
                get;
                set;
            }
            public List<GWP_FACTOR> GWPFactorList
            {
                get;
                set;
            }
            public GHG_FACTOR GHGFactor
            {
                get;
                set;
            }
            public GWP_FACTOR GWPFactor
            {
                get;
                set;
            }
            public int Status
            {
                get;
                set;
            }

            public Emissions Initialize(string locationCode, string regionDesig)
            {
                this.Location = locationCode;
                string locationRegion = string.IsNullOrEmpty(regionDesig) ? "" :  locationCode + "," + regionDesig;
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    // get all factors for the location AND all constant combustion factors
                    this.GHGFactorList = (from c in entities.GHG_FACTOR where c.LOCATION_CODE == locationCode 
                                            || (!string.IsNullOrEmpty(locationRegion) && c.LOCATION_CODE.Contains(locationRegion))
                                            || string.IsNullOrEmpty(c.LOCATION_CODE) select c).ToList();
                    if (!string.IsNullOrEmpty(locationRegion) &&  this.GHGFactorList.Where(l=> l.LOCATION_CODE == locationRegion).Count() > 0)
                    {
                        this.GHGFactorList = this.GHGFactorList.Where(l => l.LOCATION_CODE != locationCode).ToList();
                    }

                    this.GWPFactorList = (from c in entities.GWP_FACTOR  select c).ToList();
                }

                return this;
            }
 
            public decimal LookupGHG(string efmType, int forYear, string gasCode)
            {
                decimal ghg = 0;
                this.GHGFactor = null;

                if (this.GHGFactorList == null || this.GHGFactorList.Count == 0)
                {   // no data error 
                    this.Status = -1;
                    return ghg;
                }

                List<GHG_FACTOR> workList = this.GHGFactorList.Where(l => l.EFM_TYPE.ToUpper() == efmType.ToUpper()  &&  l.GAS_CODE.ToUpper() == gasCode.ToUpper()).ToList();

                if (workList.Count == 0)
                    return ghg;
                else 
                {
                    if (workList.Count == 1)
                    {   // was for constant commbustion fuel factor
                        this.GHGFactor = workList[0];
                    }
                    else
                    {
                        foreach (GHG_FACTOR gf in workList)
                        {
                            // get the yearly factor or most current factor on record
                            this.GHGFactor = gf;
                            if (gf.EFF_YEAR >= forYear)
                            {
                                break;
                            }
                        }
                    }

                    ghg = (decimal)this.GHGFactor.FACTOR;
                }
                
                return ghg;
            }

            public decimal GetGWP(string gasCode, string gwpYr)
            {
                decimal gwp = 0;

                this.GWPFactor = this.GWPFactorList.Where(l => l.GAS_CODE.ToUpper() == gasCode.ToUpper()).SingleOrDefault();
                if (this.GWPFactor != null)
                {
                    switch (gwpYr)
                    {
                        case "gwp":
                            gwp = 1.0m;
                            break;
                        case "gwp20":
                            gwp = (decimal)this.GWPFactor.GWP_20;
                            break;
                        case "gwp100":
                            gwp = (decimal)this.GWPFactor.GWP_100;
                            break;
                        case "gwp500":
                            gwp = (decimal)this.GWPFactor.GWP_500;
                            break;
                        default:
                            break;
                    }
                }

                return gwp;
            }
        }

        public partial class GHGResult
        {
            public PLANT Plant
            {
                get;
                set;
            }
            public EHS_MEASURE Measure
            {
                get;
                set;
            }
            public decimal InputValue
            {
                get;
                set;
            }
            public decimal InputUOM
            {
                get;
                set;
            }
            public decimal MetricValue
            {
                get;
                set;
            }
            public decimal MetricUOM
            {
                get;
                set;
            }
            public string EFMType
            {
                get;
                set;
            }
            public decimal GHGFactor
            {
                get;
                set;
            }
            public decimal GHGValue
            {
                get;
                set;
            }
            public int GasSeq
            {
                get;
                set;
            }
            public string GasCode
            {
                get;
                set;
            }
            public decimal GWPFactor
            {
                get;
                set;
            }
            public decimal GHGTotal
            {
                get;
                set;
            }
            public decimal GHGUOM
            {
                get;
                set;
            }

            public GHGResult CreateNew(PLANT plant, EHS_MEASURE measure, 
                                        decimal inputValue, decimal inputUOM, 
                                        decimal metricValue, decimal metricUOM,
                                        string efmType, int gasSeq, decimal ghgFactor, decimal ghgValue,
                                        string gasCode, decimal gwpFactor, 
                                        decimal ghgTotal, decimal ghgUOM)
            {
                this.Plant = plant;
                this.Measure = measure;
                this.InputValue = inputValue;
                this.InputUOM = inputUOM;
                this.MetricValue = metricValue;
                this.MetricUOM = metricUOM;
                this.EFMType = efmType;
                this.GHGFactor = ghgFactor;
                this.GHGValue = ghgValue;
                this.GasSeq = gasSeq;
                this.GasCode = gasCode;
                this.GWPFactor = gwpFactor;
                this.GHGTotal = ghgTotal;
                this.GHGUOM = ghgUOM;

                return this;
            }
        }

        public partial class GHGResultList
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
            public List<GHGResult> ResultList
            {
                get;
                set;
            }
            public decimal GHGTotal
            {
                get;
                set;
            }

            public GHGResultList CreateNew(DateTime fromDate, DateTime toDate)
            {
                this.FromDate = fromDate;
                this.ToDate = toDate;
                this.ResultList = new List<GHGResult>();
                this.GHGTotal = 0;
                return this;
            }

            public void AddResult(GHGResult result)
            {
                this.ResultList.Add(result);
            }
            public void AddResultRange(GHGResultList resultList)
            {
                this.ResultList.AddRange(resultList.ResultList);
            }

            public decimal SumTotal(decimal ghgValue)
            {
                this.GHGTotal += ghgValue;
                return this.GHGTotal;
            }
        }

        public static List<EHS_MEASURE> SelectEHSMeasureSubCategoryList(string measureCategory)
        {
            List<EHS_MEASURE> subList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    subList = (from m in ctx.EHS_MEASURE
                               where (m.MEASURE_SUBCATEGORY == null)
                               select m).OrderBy(l => l.MEASURE_CATEGORY).ThenBy(l => l.MEASURE_CD).ToList();

                    if (!string.IsNullOrEmpty(measureCategory))
                        subList = subList.ToList().FindAll(l => l.MEASURE_CATEGORY == measureCategory);
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return subList;
        }

        public static List<EHS_MEASURE> SelectEHSMeasureList(string measureCategory, bool activeOnly)
        {
            List<EHS_MEASURE> measList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    if (string.IsNullOrEmpty(measureCategory))
                        measList = (from m in ctx.EHS_MEASURE
                                    where (m.MEASURE_SUBCATEGORY != null)
                                   // select m).OrderBy(l => l.MEASURE_CATEGORY).ThenBy(l => l.MEASURE_CD).ToList();
                                select m).OrderBy(l => l.MEASURE_CD).ToList();
                    else
                        measList = (from m in ctx.EHS_MEASURE
                                    where (m.MEASURE_SUBCATEGORY != null && m.MEASURE_CATEGORY == measureCategory)
                                    select m).OrderBy(l => l.MEASURE_CD).ToList();

                    if (activeOnly)
                        measList = measList.ToList().FindAll(l => l.STATUS == "A");
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return measList;
        }


        public static EHS_MEASURE LookupEHSMeasure(SQM.Website.PSsqmEntities ctx, decimal measureID, string measureCode)
        {
            return LookupEHSMeasure(ctx, measureID, measureCode, "");
        }

        public static EHS_MEASURE LookupEHSMeasure(SQM.Website.PSsqmEntities ctx, decimal measureID, string measureCode, string acctField)
        {
            EHS_MEASURE measure = null;

            try
            { 
               if (!string.IsNullOrEmpty(measureCode))
                    measure = (from m in ctx.EHS_MEASURE
                               where (m.MEASURE_CD.ToUpper() == measureCode.ToUpper())
                               select m).SingleOrDefault();
               else if (!string.IsNullOrEmpty(acctField))
                   measure = (from m in ctx.EHS_MEASURE
                              where (m.PLANT_ACCT_FIELD.ToUpper() == acctField.ToUpper())
                              select m).SingleOrDefault();
                else
                    measure = (from m in ctx.EHS_MEASURE
                               where (m.MEASURE_ID == measureID)
                               select m).SingleOrDefault();
            }
            catch { }

            return measure;
        }

        public static EHS_MEASURE UpdateEHSMeasure(SQM.Website.PSsqmEntities ctx, EHS_MEASURE measure, string updateBy)
        {
            try
            {
                measure = (EHS_MEASURE)SQMModelMgr.SetObjectTimestamp((object)measure, updateBy, measure.EntityState);

                if (measure.EntityState == EntityState.Detached || measure.EntityState == EntityState.Added)
                    ctx.AddToEHS_MEASURE(measure);

                if (measure.STATUS == "D")
                    ctx.DeleteObject(measure);

                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return measure;
        }

        public static List<EHS_PROFILE> SelectPlantProfileList(decimal companyID)
        {
            List<EHS_PROFILE> profileList = new List<EHS_PROFILE>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                profileList = (from e in entities.EHS_PROFILE.Include("EHS_PROFILE_FACT") 
                               join p in entities.PLANT on e.PLANT_ID equals p.PLANT_ID
                               where (p.COMPANY_ID == companyID)
                           select e).ToList();
            }

            return profileList;
        }

        public static EHS_PROFILE_MEASURE LookupEHSProfileMeasure(SQM.Website.PSsqmEntities ctx, decimal prmrID)
        {
            EHS_PROFILE_MEASURE measure = null;

            try
            {
                measure = (from m in ctx.EHS_PROFILE_MEASURE.Include("EHS_MEASURE").Include("EHS_PROFILE_MEASURE_EXT") 
                            where (m.PRMR_ID == prmrID)
                            select m).SingleOrDefault();
            }
            catch { }

            return measure;
        }

        public static PLANT_ACCOUNTING LookupPlantAccounting(PSsqmEntities ctx, decimal plantID, int periodYear, int periodMonth, bool createNew)
        {
            PLANT_ACCOUNTING pa = null;
            try
            {
                pa = (from a in ctx.PLANT_ACCOUNTING
                      where (a.PLANT_ID == plantID
                                    && a.PERIOD_YEAR == periodYear
                                    && a.PERIOD_MONTH == periodMonth
                                )
                      select a).Single();
            }
            catch
            {
                if (createNew)
                {
                    pa = new PLANT_ACCOUNTING();
                    pa.PLANT_ID = plantID;
                    pa.PERIOD_YEAR = periodYear;
                    pa.PERIOD_MONTH = periodMonth;
                }
            }
            return pa;
        }

        public static PLANT_ACCOUNTING UpdatePlantAccounting(PSsqmEntities entities, PLANT_ACCOUNTING pa)
        {
            PLANT_ACCOUNTING ret = null;

            if (pa.EntityState == EntityState.Added || pa.EntityState == EntityState.Detached)
                entities.AddToPLANT_ACCOUNTING(pa);
            pa = (PLANT_ACCOUNTING)SQMModelMgr.SetObjectTimestamp(pa, "", pa.EntityState);

            if (entities.SaveChanges() > 0)
                ret = pa;

            return ret;
        }

        public static string ProdMeasureDesc(string plantAcctField)
        {
            string desc = plantAcctField;

            switch (plantAcctField)
            {
                case "OPER_COST":
                    desc = "Material Cost";
                    break;
                case "REVENUE":
                    desc = "Revenue";
                    break;
                case "TIME_WORKED":
                    desc="Time Worked";
                    break;
                default:
                    break;
            }

            return desc;
        }

        public static decimal ConvertPRODMeasure(EHS_MEASURE measure, decimal prmrID)
        {
            decimal id = 0;

            if (measure.MEASURE_CATEGORY == "PROD" || measure.MEASURE_CATEGORY == "SAFE")
            {
                switch (measure.PLANT_ACCT_FIELD)
                {
                    case "OPER_COST":
                        id = 1000000;
                        break;
                    case "REVENUE":
                        id = 1000001;
                        break;
                    case "OPER_PRODUCTION":
                        id = 1000002;
                        break;
                    case "THROUGHPUT":
                        id = 1000003;
                        break;
                    case "TIME_WORKED":
                        id = 1000004;
                        break;
                    case "TIME_LOST":
                        id = 1000005;
                        break;
                    case "TIME_LOST_CASES":
                        id = 1000006;
                        break;
                    case "RECORDED_CASES":
                        id = 1000007;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (prmrID > 0)
                    id = prmrID;
                else
                    id = measure.MEASURE_ID;
            }
            return id;
        }

        public static bool IsMeasureValueInRange(EHS_PROFILE_MEASURE measure, double value, double varsMultiplier)
        {
            bool inRange = true;
            double mean = 0;
            double sdev = 0;

            try
            {
                if (!string.IsNullOrEmpty(measure.INPUT_SNAPSHOT))
                {
                    double lastValue;
                    if (double.TryParse(measure.INPUT_SNAPSHOT, out lastValue))
                    {
                        double ratio = Math.Abs(value / Math.Max(1, lastValue));
                        if (ratio < .20 || ratio > 5)
                            inRange = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ;
            }
            return true;
           // return inRange;
        }
    }

    #endregion

    #region ehsprofile

    public class EHSProfileMeasure
    {
        public EHS_PROFILE_MEASURE ProfileMeasure
        {
            get;
            set;
        }
        public EHS_MEASURE Measure
        {
            get;
            set;
        }
        public PERSON ResponsiblePerson
        {
            get;
            set;
        }
    }

    public class EHSProfile
    {
        public PLANT Plant
        {
            get;
            set;
        }
        public BUSINESS_ORG BusOrg
        {
            get;
            set;
        }
        public EHS_PROFILE Profile
        {
            get;
            set;
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }
        public EHSProfileStatus Status
        {
            get;
            set;
        }
        public EHSProfileStatus CurrentStatus
        {
            get;
            set;
        }
        public bool isNew
        {
            get;
            set;
        }
        public EHS_MEASURE CurrentEHSMeasure
        {
            get;
            set;
        }
        public EHS_PROFILE_MEASURE CurrentProfileMeasure
        {
            get;
            set;
        }
        public string Notes
        {
            get;
            set;
        }
        public EHSProfilePeriod InputPeriod
        {
            get;
            set;
        }
        public List<EHSProfilePeriod> PeriodList
        {
            get;
            set;
        }
        public DateTime MinPeriodDate
        {
            get;
            set;
        }
        public decimal FilterByResponsibleID
        {
            get;
            set;
        }
        public bool ActiveOnly
        {
            get;
            set;
        }
 
        public EHSProfile CreateNew(decimal plantID)
        {
            this.isNew = true;
            this.Status = 0;
            this.Entities = new PSsqmEntities();
            return this;
        }

        public EHSProfile Load(decimal plantID)
        {
            Load(plantID, false, false);
            return this;
        }

        public EHSProfile Load(decimal plantID, bool createNew, bool activeOnly)
        {
            this.isNew = false;
            this.FilterByResponsibleID = 0;
            this.ActiveOnly = activeOnly;
            this.Entities = new PSsqmEntities();
            this.Status = EHSProfileStatus.NonExist;
            this.CurrentStatus = EHSProfileStatus.Normal;
            try
            {
                this.Plant = SQMModelMgr.LookupPlant(plantID);
                this.BusOrg = SQMModelMgr.LookupBusOrg((decimal)this.Plant.BUS_ORG_ID);
                SessionManager.SetEffLocationPlant(this.Plant);
                this.Profile = LookupProfile(this.Entities, plantID);
                if (this.Profile == null)
                {
                    if (createNew && this.Plant != null)
                    {
                        this.Profile = new EHS_PROFILE();
                        this.Profile.PLANT_ID = plantID;
                        this.Profile.STATUS = "A";
                        this.Profile.DAY_DUE = 28;
                        this.Profile.REMINDER_DAYS = 2;
                        this.Profile.LAST_UPD_BY = SessionManager.UserContext.UserName();
                        this.Profile.LAST_UPD_DT = DateTime.UtcNow;
                        this.isNew = true;
                    }
                }
                if (this.Profile != null)
                {
                    if (!this.isNew && this.Profile.EHS_PROFILE_MEASURE.Count == 0)
                        this.Status = EHSProfileStatus.NoMeasures;
                    else
                        this.Status = EHSProfileStatus.Normal;
                }
            }
            catch
            {
                this.Status = this.CurrentStatus = EHSProfileStatus.NonExist;
            }

            return this;
        }

        public EHS_PROFILE LookupProfile(SQM.Website.PSsqmEntities ctx, decimal plantID)
        {
            EHS_PROFILE profile = null;
 
            try
            {
                profile = (from p in ctx.EHS_PROFILE.Include("EHS_PROFILE_MEASURE").Include("EHS_PROFILE_MEASURE.EHS_PROFILE_MEASURE_EXT").Include("EHS_PROFILE_FACT") 
                            where (p.PLANT_ID == plantID)
                            select p).SingleOrDefault();

                if (profile != null)
                {
                    List<EHSProfileMeasure> prmrList = new List<EHSProfileMeasure>();
                    prmrList = (from p in ctx.EHS_PROFILE_MEASURE 
                                join m in ctx.EHS_MEASURE on p.MEASURE_ID equals m.MEASURE_ID
                                join u in ctx.PERSON on p.RESPONSIBLE_ID equals u.PERSON_ID into u_t
                                where (p.PLANT_ID == plantID)
                                from u in u_t.DefaultIfEmpty()
                                select new EHSProfileMeasure
                                    {
                                        ProfileMeasure = p,
                                        Measure = m,
                                        ResponsiblePerson = u
                                    }).ToList();

                    foreach (EHS_PROFILE_MEASURE measure in profile.EHS_PROFILE_MEASURE)
                    {
                        EHSProfileMeasure prmr = prmrList.FirstOrDefault(l => l.ProfileMeasure.PRMR_ID == measure.PRMR_ID);
                        if (prmr != null)
                        {
                            measure.EHS_MEASURE = prmr.Measure;
                            measure.PERSON = prmr.ResponsiblePerson;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ;
            }

            return profile;
        }

        public EHS_PROFILE_FACT AddFactor(string calcsStat, string calcsScope, decimal factorID)
        {
            EHS_PROFILE_FACT fact = new EHS_PROFILE_FACT();

            fact.PLANT_ID = this.Profile.PLANT_ID;
            fact.CALCS_STAT = calcsStat;
            fact.CALCS_SCOPE = calcsScope;
            fact.FACTOR_ID = factorID;

            return fact;
        }

        public EHS_PROFILE_MEASURE AddMeasure(EHS_PROFILE_MEASURE pm, decimal measureID)
        {
            //EHS_PROFILE_MEASURE pm = new EHS_PROFILE_MEASURE();
            pm.MEASURE_ID = measureID;
            pm.EHS_MEASURE = EHSModel.LookupEHSMeasure(this.Entities, pm.MEASURE_ID, "");
            this.Profile.EHS_PROFILE_MEASURE.Add(pm);
            return pm;
        }

        public EHS_PROFILE_MEASURE CopyMeasure(EHS_PROFILE_MEASURE om, decimal plantID, decimal personID)
        {
            EHS_PROFILE_MEASURE pm = new EHS_PROFILE_MEASURE();
            pm.EHS_MEASURE = om.EHS_MEASURE;
            pm.PLANT_ID = plantID;
            pm.MEASURE_ID = om.MEASURE_ID;
            pm.INPUT_SEQ = om.INPUT_SEQ;
            pm.MEASURE_PROMPT = om.MEASURE_PROMPT;
            pm.UN_CODE = om.UN_CODE;
            pm.REG_STATUS = om.REG_STATUS;
            pm.DEFAULT_CURRENCY_CODE = om.DEFAULT_CURRENCY_CODE;
            pm.DEFAULT_UOM = om.DEFAULT_UOM;
            pm.RESPONSIBLE_ID = personID > 0 ? personID : om.RESPONSIBLE_ID;
            pm.NEG_VALUE_ALLOWED = om.NEG_VALUE_ALLOWED;
            pm.IS_REQUIRED = om.IS_REQUIRED;
            pm.STATUS = om.STATUS;
            pm.WASTE_CODE = om.WASTE_CODE;
            pm.INPUT_SNAPSHOT = "";
            pm.LAST_UPD_DT = DateTime.Now;
            pm.UOM_FACTOR = om.UOM_FACTOR;
            this.Profile.EHS_PROFILE_MEASURE.Add(pm);

            return pm;
        }

        public EHS_PROFILE_MEASURE GetMeasure(decimal prmrID)
        {
            return this.Profile.EHS_PROFILE_MEASURE.FirstOrDefault(l=> l.PRMR_ID == prmrID);
        }

        public EHS_PROFILE_MEASURE_EXT GetMeasureExt(EHS_PROFILE_MEASURE measure, DateTime effDate)
        {
            if (measure.EHS_PROFILE_MEASURE_EXT != null && (measure.EHS_PROFILE_MEASURE_EXT.EFF_END_DT == null || (measure.EHS_PROFILE_MEASURE_EXT.EFF_END_DT.HasValue  &&  measure.EHS_PROFILE_MEASURE_EXT.EFF_END_DT >= effDate.Date)))
                return measure.EHS_PROFILE_MEASURE_EXT;
            else
                return null;
        }

        public EHS_PROFILE_MEASURE UpdateMeasureResponsible(EHS_PROFILE_MEASURE measure, decimal personID)
        {
            measure.PERSON = SQMModelMgr.LookupPerson(this.Entities, personID, "", false);
            return measure;
        }

        public static int UpdateProfile(EHSProfile theProfile)
        {
            int status = 0;

            try
            {
				if (theProfile.Profile.EntityState == EntityState.Detached || theProfile.Profile.EntityState == EntityState.Added)
				{
					theProfile.Entities.AddToEHS_PROFILE(theProfile.Profile);
				}
                theProfile.Profile = (EHS_PROFILE)SQMModelMgr.SetObjectTimestamp(theProfile.Profile, SessionManager.UserContext.UserName(), theProfile.Profile.EntityState);
                /*
                for (int n = 0; n < theProfile.Profile.EHS_PROFILE_MEASURE.Count; n++ )
                {
                    EHS_PROFILE_MEASURE measure = theProfile.Profile.EHS_PROFILE_MEASURE.ElementAt(n);
                    if (measure != null && measure.STATUS == "D")
                        theProfile.Entities.DeleteObject(measure);
                }
                */
                status = theProfile.Entities.SaveChanges();
                theProfile.isNew = false;
            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
                status = -1;
            }

            return status;
        }

        public List<EHS_PROFILE_MEASURE> MeasureList(bool applyFilters)
        {
            if (applyFilters)
            {
                if (this.FilterByResponsibleID > 0)
                    return this.Profile.EHS_PROFILE_MEASURE.Where(l => l.STATUS != "I"  &&   l.RESPONSIBLE_ID == this.FilterByResponsibleID).ToList();
                else
                    return this.Profile.EHS_PROFILE_MEASURE.Where(l => l.STATUS != "I").ToList();
            }
            else
                return this.Profile.EHS_PROFILE_MEASURE.ToList();
        }

        public List<EHS_PROFILE_MEASURE> MeasureList(bool applyFilters, List<PRIVLIST> privList)
        {
            if (applyFilters)
            {
                if (this.FilterByResponsibleID > 0)
                {
					if (privList.Where(p => p.PRIV == (int)SysPriv.originate).Count() > 0)
						return this.Profile.EHS_PROFILE_MEASURE.Where(l => l.STATUS != "I" && l.RESPONSIBLE_ID == this.FilterByResponsibleID).ToList();
					else
						return this.Profile.EHS_PROFILE_MEASURE.Where(l => l.STATUS != "I").ToList();
                }
				else
					return this.Profile.EHS_PROFILE_MEASURE.Where(l => l.STATUS != "I").ToList();
            }
            else
                return this.Profile.EHS_PROFILE_MEASURE.ToList();
        }


        public static int DeleteProfileMeasure(EHSProfile theProfile, decimal measureID, bool deleteInputs, bool deleteMeasure)
        {
            // delete a profile metric history and optionally, the profile measure
            int status = 0;
       

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                try
                {
                    status = ctx.ExecuteStoreCommand("DELETE FROM EHS_METRIC_HISTORY WHERE PLANT_ID = " + theProfile.Plant.PLANT_ID.ToString() + " AND MEASURE_ID = " + measureID.ToString());
                    if (deleteInputs)
                    {
                        status = ctx.ExecuteStoreCommand("DELETE FROM EHS_PROFILE_INPUT WHERE PRMR_ID IN (SELECT PRMR_ID FROM EHS_PROFILE_MEASURE WHERE PLANT_ID = " + theProfile.Plant.PLANT_ID.ToString() + " AND MEASURE_ID = " + measureID.ToString() + ")");
                        status = ctx.ExecuteStoreCommand("DELETE FROM EHS_PROFILE_MEASURE_EXT WHERE PRMR_ID IN (SELECT PRMR_ID FROM EHS_PROFILE_MEASURE WHERE PLANT_ID = " + theProfile.Plant.PLANT_ID.ToString() + " AND MEASURE_ID = " + measureID.ToString() + ")");
                    }
                    if (deleteMeasure)
                    {
                        status = ctx.ExecuteStoreCommand("DELETE FROM EHS_PROFILE_MEASURE WHERE PLANT_ID = " + theProfile.Plant.PLANT_ID.ToString() + " AND MEASURE_ID = " + measureID.ToString());
                    }
                }
                catch (Exception ex)
                {
                    status = -1;
                    // log error
                }
            }

            return status;
        }

        public EHSProfilePeriod LoadPeriod(DateTime periodDate)
        {
            this.InputPeriod = new EHSProfilePeriod().Initialize(periodDate, this.Profile.DAY_DUE);
            this.InputPeriod.InputsList = new List<EHS_PROFILE_INPUT>();

            try
            {
                decimal[] ids = new decimal[this.Profile.EHS_PROFILE_MEASURE.Count];
                int n = 0;
                foreach (EHS_PROFILE_MEASURE ms in this.Profile.EHS_PROFILE_MEASURE)
                    ids[n++] = ms.PRMR_ID;

                this.InputPeriod.InputsList = (from m in this.InputPeriod.Entities.EHS_PROFILE_INPUT  /*.Include("EHS_PROFILE_MEASURE.EHS_MEASURE") */
                                where (ids.Contains((decimal)m.PRMR_ID) && (this.InputPeriod.PeriodYear == m.PERIOD_YEAR && this.InputPeriod.PeriodMonth == m.PERIOD_MONTH))
                                select m).OrderBy(m=> m.PRMR_ID).ThenBy(m=> m.INPUT_DT).ToList();

                this.InputPeriod.PlantAccounting = (from a in this.InputPeriod.Entities.PLANT_ACCOUNTING
                                                    where (a.PLANT_ID == this.Plant.PLANT_ID && a.PERIOD_YEAR == periodDate.Year && a.PERIOD_MONTH == periodDate.Month)
                                                    select a).SingleOrDefault();
            }
            catch (Exception e)
            {
                ;
            }

            return this.InputPeriod;
        }
        

        public int UpdatePeriod(bool applyFilters, string statusOverride, bool approvalStatus, string updateByUser)
        {
            int status = 0;
            DateTime updateDate = DateTime.UtcNow;
            var PropertyInfos = this.InputPeriod.PlantAccounting.GetType().GetProperties();
            System.Reflection.PropertyInfo pInfo = null;

            try
            {
                foreach (EHS_PROFILE_MEASURE measure in MeasureList(true))
                {
                    foreach (EHS_PROFILE_INPUT input in this.InputPeriod.GetPeriodInputList(measure.PRMR_ID))
                    {
                        if (input.STATUS == "C" || input.STATUS == "N")
                        {
                            this.InputPeriod.Entities.AddToEHS_PROFILE_INPUT(input);
                        }
                        if (input.EntityState != System.Data.EntityState.Unchanged)
                        {
                            if ((pInfo = PropertyInfos.FirstOrDefault(i => i.Name == measure.EHS_MEASURE.PLANT_ACCT_FIELD)) != null)
                            {
                                pInfo.SetValue(this.InputPeriod.PlantAccounting, input.MEASURE_VALUE, null);
                                this.InputPeriod.PlantAccounting.LAST_UPD_DT = updateDate;
                                this.InputPeriod.PlantAccounting.LAST_UPD_BY = updateByUser;
                            }
                            else
                            {
                                input.UOM_FACTOR = measure.UOM_FACTOR;
                                if (input.STATUS != "D")
                                    input.STATUS = "A";
                                input.LAST_UPD_DT = updateDate;
                                input.LAST_UPD_BY = SessionManager.UserContext.UserName();
                                UpdateMeasureSnapshot(measure, this.InputPeriod.PeriodDate, (double)input.MEASURE_VALUE);
                            }
                        }
                    }
                }

                if (this.InputPeriod.PlantAccounting.EntityState == EntityState.Detached)
                    this.InputPeriod.Entities.AddToPLANT_ACCOUNTING(this.InputPeriod.PlantAccounting);

                this.InputPeriod.PlantAccounting = (PLANT_ACCOUNTING)SQMModelMgr.SetObjectTimestamp(this.InputPeriod.PlantAccounting, SessionManager.UserContext.UserName(), this.InputPeriod.PlantAccounting.EntityState);
                if (approvalStatus) 
                {
                    if (this.InputPeriod.PlantAccounting.APPROVER_ID.HasValue == false)
                    {
                        this.InputPeriod.PlantAccounting.APPROVER_ID = SessionManager.UserContext.Person.PERSON_ID;
                        this.InputPeriod.PlantAccounting.APPROVAL_DT = updateDate;
                    }
                }
                else
                {
                    if (this.InputPeriod.PlantAccounting.APPROVER_ID.HasValue)
                    {
                        this.InputPeriod.PlantAccounting.APPROVER_ID = null;
                        this.InputPeriod.PlantAccounting.APPROVAL_DT = null;
                    }
                }

                status = this.InputPeriod.Entities.SaveChanges();

                bool doDelete = false;
                foreach (EHS_PROFILE_MEASURE measure in MeasureList(true))
                {
                    foreach (EHS_PROFILE_INPUT input in this.InputPeriod.GetPeriodInputList(measure.PRMR_ID))
                    {
                        if (input.STATUS == "D" && !input.ROLLUP_DT.HasValue)
                        {
                            this.InputPeriod.Entities.EHS_PROFILE_INPUT.DeleteObject(input);
                            doDelete = true;
                        }
                    }
                }
                if (doDelete)
                    status = this.InputPeriod.Entities.SaveChanges();

              //  int status2 = this.Entities.SaveChanges();  // updates the measure snapshot

            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
                status = -1;
            }

            return status;
        }

        public bool ValidPeriod()
        {
            bool isValid = true;
            // create plant accounting table if missing
            if (this.InputPeriod == null)
            {
                return false;
            }

            if (this.InputPeriod.PlantAccounting == null)
            {
                if ((this.InputPeriod.PlantAccounting = CreatePlantAccounting(this.Plant.PLANT_ID, this.InputPeriod)) == null)
                    isValid = false;
            }

            return isValid;
        }

        public int MapPlantAccountingInputs(bool createIfEmpty, bool displayIfNull)
        {
            int status = 0;
            if (this.InputPeriod.PlantAccounting == null && createIfEmpty)
            {
                this.InputPeriod.PlantAccounting = CreatePlantAccounting(this.Plant.PLANT_ID, this.InputPeriod);
            }
            if (this.InputPeriod.PlantAccounting != null)
            {
                var PropertyInfos = this.InputPeriod.PlantAccounting.GetType().GetProperties();
                System.Reflection.PropertyInfo pInfo = null;
                foreach (EHS_PROFILE_MEASURE measure in this.Profile.EHS_PROFILE_MEASURE)
                {
                    try
                    {
                        if ((measure.EHS_MEASURE.MEASURE_CATEGORY == "PROD" || measure.EHS_MEASURE.MEASURE_CATEGORY == "SAFE") && (pInfo = PropertyInfos.FirstOrDefault(i => i.Name == measure.EHS_MEASURE.PLANT_ACCT_FIELD)) != null)
                        {
                            EHS_PROFILE_INPUT input = CreatePeriodInput(this.InputPeriod, measure, false);
                            if (pInfo.GetValue(this.InputPeriod.PlantAccounting, null) != null)
                            {
                                input.EFF_FROM_DT = new DateTime(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth, 1);
                                input.EFF_TO_DT = new DateTime(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth, DateTime.DaysInMonth(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth));
                                input.MEASURE_VALUE = (decimal)pInfo.GetValue(this.InputPeriod.PlantAccounting, null);
                            }
                            else 
                            {
                                input.MEASURE_VALUE = 0;
                                if (displayIfNull)
                                {
                                    input.EFF_FROM_DT = new DateTime(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth, 1);
                                    input.EFF_TO_DT = new DateTime(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth, DateTime.DaysInMonth(this.InputPeriod.PeriodYear, this.InputPeriod.PeriodMonth));
                                }
                            }
                        }
                    }
                    catch
                    {
                        status = -1;
                    }
                }
            }

            return status;
        }

        public TaskStatus PeriodStatus(string[] measureCategories, bool applyFilters, out DateTime lastUpdateDate)
        {
            // check status of all required metrics (assumed) assigned to a user
            this.InputPeriod.NumResponsible = this.InputPeriod.NumComplete = this.InputPeriod.NumRequired = this.InputPeriod.NumRequiredComplete = 0;
            TaskStatus periodStatus = TaskStatus.Pending;
            lastUpdateDate = DateTime.MinValue;
            DateTime inputDate;
            string[] excludeCategory = new string[] {"PROD", "SAFE", "FACT"}; 

            foreach (EHS_PROFILE_MEASURE metric in this.MeasureList(applyFilters))
            {
                if ((measureCategories.Length == 0  ||  measureCategories.Contains(metric.EHS_MEASURE.MEASURE_CATEGORY)) 
                   &&  (metric.EHS_MEASURE.MEASURE_CATEGORY != excludeCategory[0]  &&  metric.EHS_MEASURE.MEASURE_CATEGORY != excludeCategory[1]))
                {
                    //if ((bool)metric.IS_REQUIRED)
                    if ((bool)metric.IS_REQUIRED  &&  metric.STATUS != "I")
                        ++this.InputPeriod.NumRequired;
                    if (this.InputPeriod.GetPeriodInputList(metric.PRMR_ID).Count > 0)
                    {
                        ++this.InputPeriod.NumComplete;
                        if ((bool)metric.IS_REQUIRED)
                            ++this.InputPeriod.NumRequiredComplete;
                        try
                        {
                            inputDate = (DateTime)this.InputPeriod.GetPeriodInputList(metric.PRMR_ID).Where(l => l.LAST_UPD_DT != null).Select(l => l.LAST_UPD_DT).Max();
                            if (inputDate > lastUpdateDate)
                                lastUpdateDate = inputDate;
                        }
                        catch { }
                    }
                }
            }

            if (this.InputPeriod.NumRequiredComplete >= this.InputPeriod.NumRequired)
                periodStatus = TaskStatus.Complete;

            return periodStatus;
        }

        public EHS_METRIC_HISTORY LastRollupRecord()
        {
            EHS_METRIC_HISTORY lastRec = null;

            try
            {
                lastRec = (from h in this.Entities.EHS_METRIC_HISTORY
                           select h).OrderByDescending(h => h.LAST_UPD_DT).Where(h => h.PLANT_ID == this.Plant.PLANT_ID && h.PERIOD_YEAR == this.InputPeriod.PeriodYear && h.PERIOD_MONTH == this.InputPeriod.PeriodMonth && string.IsNullOrEmpty(h.LAST_UPD_BY) == false).FirstOrDefault();
            }
            catch { }

            return lastRec;
        }

        public EHS_PROFILE_INPUT CreatePeriodInput(EHSProfilePeriod period, EHS_PROFILE_MEASURE measure, bool addByUser)
        {
            EHS_PROFILE_INPUT input = new EHS_PROFILE_INPUT();
            if (measure != null)
            {
                input.PERIOD_YEAR = period.PeriodYear;
                input.PERIOD_MONTH = period.PeriodMonth;
                input.PRMR_ID = measure.PRMR_ID;
                if (measure.DEFAULT_UOM.HasValue)
                {
                    input.UOM = (decimal)measure.DEFAULT_UOM;
                    input.UOM_FACTOR = measure.UOM_FACTOR;
                }
                else
                    input.UOM = 0;

                input.CURRENCY_CODE = measure.DEFAULT_CURRENCY_CODE;
                input.INPUT_DT = DateTime.UtcNow;
                input.STATUS = "C";
                if (addByUser)
                    input.STATUS = "N";
                if (measure.EHS_MEASURE.MEASURE_CATEGORY == "PROD"  ||  measure.EHS_MEASURE.MEASURE_CATEGORY == "SAFE")
                    input.STATUS = "P";             // mark prod measures so that we can remove them prior to saving

                this.InputPeriod.InputsList.Add(input);  
            }
 
            return input;
        }

        public static PLANT_ACCOUNTING CreatePlantAccounting(decimal plantID, EHSProfilePeriod period)
        {
            PLANT_ACCOUNTING plantAcct = new PLANT_ACCOUNTING();

            plantAcct.PLANT_ID = plantID;
            plantAcct.PERIOD_YEAR = period.PeriodYear;
            plantAcct.PERIOD_MONTH = period.PeriodMonth;
            plantAcct.LAST_UPD_DT = DateTime.UtcNow;

            return plantAcct;
        }


        /* =======================  this is the BIG one =================================================== */

        public int UpdateMetricHistory()
       
        {
            int status = 0;
            DateTime updateDate = DateTime.UtcNow;
            decimal actualUOM = 0;
            bool shouldSave = false;

            try
            {
                // get the entire metric history per the min/max invoice dates (i.e. multiple months)

                DateTime fromDate = (DateTime)this.InputPeriod.InputsList.Select(l => l.EFF_FROM_DT).Min();
                fromDate = WebSiteCommon.PeriodFromDate(fromDate.AddMonths(-1));
                DateTime toDate = (DateTime)this.InputPeriod.InputsList.Select(l => l.EFF_TO_DT).Max();
                toDate = WebSiteCommon.PeriodToDate(toDate);

                this.InputPeriod.HistoryList = (from h in this.InputPeriod.Entities.EHS_METRIC_HISTORY
                                                where (h.PLANT_ID == this.Plant.PLANT_ID
                                             && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) <= toDate)
                                                select h).ToList();
            }
            catch (Exception e)
            {
               // SQMLogger.LogException(e);
                return -1;
            }

            EHS_METRIC_HISTORY metric = null;
            List<PeriodValue> pvList = null;
            foreach (EHS_PROFILE_MEASURE measure in this.Profile.EHS_PROFILE_MEASURE.OrderBy(m => m.EHS_MEASURE.MEASURE_ID))
            {
                if (measure.EHS_MEASURE.MEASURE_CATEGORY != "PROD"  &&  measure.EHS_MEASURE.MEASURE_CATEGORY != "SAFE")
                {
                    if ((metric = this.InputPeriod.HistoryList.FirstOrDefault(l => l.MEASURE_ID == measure.MEASURE_ID && l.PERIOD_YEAR == InputPeriod.PeriodYear && l.PERIOD_MONTH == InputPeriod.PeriodMonth)) == null)
                    {
                        metric = CreateHistoryMetric(this.Plant.PLANT_ID, measure.MEASURE_ID, InputPeriod.PeriodDate, SQMResourcesMgr.GetStdUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, (decimal)measure.DEFAULT_UOM, SessionManager.StdUOMList), BusOrg.PREFERRED_CURRENCY_CODE);
                        this.InputPeriod.HistoryList.Add(metric);
                    }

                    double sampleValue = 0;
                    int numInputs = 0;
                    List<EHS_PROFILE_INPUT> inputList = this.InputPeriod.GetPeriodInputList(measure.PRMR_ID);

                    foreach (EHS_PROFILE_INPUT input in inputList)
                    {
                        // get all inputs for the measure
                        if (input.STATUS == "A"  ||  input.STATUS == "D")   
                        {
                            // inputs marked for delete should only get here if they had been rolled up previously
                            // need to deduct previous inputs (if any) from each period it was accrued against
                            if (input.ROLLUP_DT > DateTime.MinValue)
                            {
                                pvList = CalcPeriodValue((DateTime)input.ORIGINAL_EFF_FROM_DT, (DateTime)input.ORIGINAL_EFF_TO_DT, (decimal)input.ORIGINAL_MEASURE_VALUE, (decimal)input.ORIGINAL_MEASURE_COST);
                                foreach (PeriodValue pv in pvList)
                                {
                                    if ((metric = this.InputPeriod.HistoryList.FirstOrDefault(l => l.MEASURE_ID == measure.MEASURE_ID && l.PERIOD_YEAR == pv.PeriodDate.Year && l.PERIOD_MONTH == pv.PeriodDate.Month)) != null && metric.STATUS != "N")
                                    {
                                        // deduct original values if metric period is not new
                                        metric.MEASURE_VALUE -= decimal.Round((decimal)SQMResourcesMgr.ConvertUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, (decimal)input.ORIGINAL_UOM, (double)pv.Value, input.UOM_FACTOR, SessionManager.StdUOMList, out actualUOM), 2);
										metric.MEASURE_COST -= decimal.Round((decimal)CurrencyConverter.Convert(input.ORIGINAL_CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, (double)pv.Cost, InputPeriod.PeriodYear, InputPeriod.PeriodMonth), 2);
                                        metric.INPUT_VALUE -= pv.Value;
                                        metric.INPUT_COST -= pv.Cost;
                                    }
                                }
                            }

                            if (input.STATUS == "D")    // delete input 
                            {
                                this.InputPeriod.Entities.EHS_PROFILE_INPUT.DeleteObject(input);
                                break;
                            }

                            ++numInputs;
                            sampleValue += (double)input.MEASURE_VALUE;
                            pvList = CalcPeriodValue((DateTime)input.EFF_FROM_DT, (DateTime)input.EFF_TO_DT, (decimal)input.MEASURE_VALUE, (decimal)input.MEASURE_COST);
                           
                            foreach (PeriodValue pv in pvList)
                            {
                                if ((metric = this.InputPeriod.HistoryList.FirstOrDefault(l => l.MEASURE_ID == measure.MEASURE_ID && l.PERIOD_YEAR == pv.PeriodDate.Year && l.PERIOD_MONTH == pv.PeriodDate.Month)) == null)
                                {
                                    metric = CreateHistoryMetric(this.Plant.PLANT_ID, measure.MEASURE_ID, pv.PeriodDate, SQMResourcesMgr.GetStdUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, (decimal)measure.DEFAULT_UOM, SessionManager.StdUOMList), BusOrg.PREFERRED_CURRENCY_CODE);
                                    this.InputPeriod.HistoryList.Add(metric);
                                }
                                metric.MEASURE_VALUE += decimal.Round((decimal)SQMResourcesMgr.ConvertUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, (decimal)input.UOM, (double)pv.Value, input.UOM_FACTOR, SessionManager.StdUOMList, out actualUOM), 2);
								metric.MEASURE_COST += decimal.Round((decimal)CurrencyConverter.Convert(input.CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, (double)pv.Cost, InputPeriod.PeriodYear, InputPeriod.PeriodMonth), 2);
                                metric.UOM_ID = actualUOM;
                                metric.CURRENCY_CODE = BusOrg.PREFERRED_CURRENCY_CODE;
                                metric.INPUT_VALUE += pv.Value;
                                metric.INPUT_COST += pv.Cost;
                                metric.INPUT_UOM_ID = input.UOM;
                                metric.INPUT_CURRENCY_CODE = input.CURRENCY_CODE;
                                metric.LAST_UPD_DT = updateDate;
                                metric.LAST_UPD_BY = SessionManager.UserContext.UserName();
                                metric.STATUS = "A";
                                shouldSave = true;
                            }
                            // take a snapshot of the current input fields so that we can 'back out' from the metric history if the input gets re-rolled due to changes/edits
                            input.ROLLUP_DT = DateTime.UtcNow;
                            input.ORIGINAL_MEASURE_VALUE = input.MEASURE_VALUE;
                            input.ORIGINAL_MEASURE_COST = input.MEASURE_COST;
                            input.ORIGINAL_EFF_FROM_DT = input.EFF_FROM_DT;
                            input.ORIGINAL_EFF_TO_DT = input.EFF_TO_DT;
                            input.ORIGINAL_UOM = input.UOM;
                            input.ORIGINAL_CURRENCY_CODE = input.CURRENCY_CODE;
                        }
                    }
                    UpdateMeasureSnapshot(measure, this.InputPeriod.PeriodDate, sampleValue / Math.Max(1,numInputs));
                }
            }

            if (shouldSave)
            {
                try
                {
                    this.InputPeriod.PlantAccounting.FINALIZE_DT = updateDate;
                    this.InputPeriod.PlantAccounting.FINALIZE_ID = SessionManager.UserContext.Person.PERSON_ID;
                    status = this.InputPeriod.Entities.SaveChanges();  // save the metric history
  
                    this.Entities.SaveChanges();  // save any updated profile measures
                }
                catch (Exception ex)
                {
                  //  SQMLogger.LogException(ex);
                    status = -1;
                }
            }

            return status;
        }

        // ******************************** new BIG ONE  *******************************************************************
        public int UpdateMetricHistory(DateTime periodDate)
        {
            int status = 0;
            DateTime updateDate = DateTime.UtcNow;
            decimal actualUOM = 0;
            bool shouldSave = false;

            try
            {
                // get all inputs that may span the period 
                DateTime fromDate = new DateTime(periodDate.Year, periodDate.Month, 1);
                DateTime toDate = new DateTime(periodDate.Year, periodDate.Month, DateTime.DaysInMonth(periodDate.Year, periodDate.Month));
                decimal[] ids = this.Profile.EHS_PROFILE_MEASURE.Select(l => l.PRMR_ID).ToArray();
                this.InputPeriod.InputsList = (from m in this.InputPeriod.Entities.EHS_PROFILE_INPUT.Include("EHS_PROFILE_MEASURE") 
                                                where (ids.Contains((decimal)m.PRMR_ID) 
                                                &&  (m.EFF_TO_DT >= fromDate  &&  m.EFF_FROM_DT <= toDate))
                                                select m).OrderBy(m => m.PRMR_ID).ThenBy(m => m.INPUT_DT).ToList();
  
                // get existing history records for the specific period
                this.InputPeriod.HistoryList = (from h in this.InputPeriod.Entities.EHS_METRIC_HISTORY
                                                where (h.PLANT_ID == this.Plant.PLANT_ID &&  h.PERIOD_YEAR == periodDate.Year  &&  h.PERIOD_MONTH == periodDate.Month)
                                                select h).ToList();
                // clear existing history values becuase we will be recalculating all from all inputs spaning the period
                foreach (EHS_METRIC_HISTORY hst in this.InputPeriod.HistoryList)
                {
                    hst.MEASURE_COST = hst.INPUT_VALUE = hst.INPUT_COST = hst.MEASURE_VALUE = 0;
                }
            }
            catch (Exception e)
            {
                // SQMLogger.LogException(e);
                return -1;
            }

            EHS_METRIC_HISTORY metric = null;
            List<PeriodValue> pvList = null;
            foreach (EHS_PROFILE_MEASURE measure in this.Profile.EHS_PROFILE_MEASURE.OrderBy(m => m.EHS_MEASURE.MEASURE_ID))
            {
                if (measure.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && measure.EHS_MEASURE.MEASURE_CATEGORY != "SAFE")
                {
                    // create the metric history record if it does not already exist
                    if ((metric = this.InputPeriod.HistoryList.FirstOrDefault(l => l.MEASURE_ID == measure.MEASURE_ID && l.PERIOD_YEAR == InputPeriod.PeriodYear && l.PERIOD_MONTH == InputPeriod.PeriodMonth)) == null)
                    {
                        metric = CreateHistoryMetric(this.Plant.PLANT_ID, measure.MEASURE_ID, InputPeriod.PeriodDate, SQMResourcesMgr.GetStdUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, SQMResourcesMgr.ZeroIfNull(measure.DEFAULT_UOM), SessionManager.StdUOMList), BusOrg.PREFERRED_CURRENCY_CODE);
                        this.InputPeriod.HistoryList.Add(metric);
                    }

                    double sampleValue = 0;
                    int numInputs = 0;
                    List<EHS_PROFILE_INPUT> inputList = this.InputPeriod.GetPeriodInputList(measure.PRMR_ID); // this.InputPeriod.GetPeriodMeasureInputList(measure.EHS_MEASURE.MEASURE_ID);
                    foreach (EHS_PROFILE_INPUT input in inputList)
                    {
                        // get all inputs for the measure
                        if (input.STATUS == "A" || input.STATUS == "D")
                        {
                            if (input.STATUS == "D")    // delete input 
                            {
                               // this.InputPeriod.Entities.EHS_PROFILE_INPUT.DeleteObject(input);
                                this.InputPeriod.Entities.ExecuteStoreCommand("DELETE FROM EHS_PROFILE_INPUT WHERE PRMR_ID = " + input.PRMR_ID.ToString() + " AND PERIOD_YEAR = " + input.PERIOD_YEAR.ToString() + " AND PERIOD_MONTH = " + input.PERIOD_MONTH.ToString() );
                                shouldSave = true;
                                break;
                            }

                            // calculate the input amounts invoiced for this period
                            pvList = CalcPeriodValue((DateTime)input.EFF_FROM_DT, (DateTime)input.EFF_TO_DT, SQMResourcesMgr.ZeroIfNull(input.MEASURE_VALUE), SQMResourcesMgr.ZeroIfNull(input.MEASURE_COST));
                            PeriodValue pv = pvList.Where(l => l.PeriodDate.Year == periodDate.Year && l.PeriodDate.Month == periodDate.Month).FirstOrDefault();
                            if (pv != null)
                            {
                                if (measure.EHS_MEASURE.MEASURE_CATEGORY == "FACT")
                                {
                                    metric.MEASURE_VALUE += pv.Value;
                                    metric.MEASURE_COST += pv.Cost;
                                    metric.UOM_ID = (decimal)measure.EHS_MEASURE.STD_UOM;
                                    metric.INPUT_UOM_ID = (decimal)measure.EHS_MEASURE.STD_UOM;
                                }
                                else
                                {
                                    metric.MEASURE_VALUE += decimal.Round((decimal)SQMResourcesMgr.ConvertUOM(measure.EHS_MEASURE.MEASURE_CATEGORY, (decimal)input.UOM, (double)pv.Value, input.UOM_FACTOR, SessionManager.StdUOMList, out actualUOM), 2);
                                    metric.MEASURE_COST += decimal.Round((decimal)CurrencyConverter.Convert(input.CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, BusOrg.PREFERRED_CURRENCY_CODE, (double)pv.Cost, InputPeriod.PeriodYear, InputPeriod.PeriodMonth), 2);
                                    metric.UOM_ID = actualUOM;
                                    metric.INPUT_UOM_ID = input.UOM;
                                }
                                metric.CURRENCY_CODE = BusOrg.PREFERRED_CURRENCY_CODE;
                                metric.INPUT_VALUE += pv.Value;
                                metric.INPUT_COST += pv.Cost;
                                metric.INPUT_CURRENCY_CODE = input.CURRENCY_CODE;
                                metric.LAST_UPD_DT = updateDate;
                                metric.LAST_UPD_BY = SessionManager.UserContext.UserName();
                                metric.STATUS = "A";
                                shouldSave = true;

                                // take a snapshot of the current input fields so that we can 'back out' from the metric history if the input gets re-rolled due to changes/edits
                                input.ROLLUP_DT = DateTime.UtcNow;
                                input.ORIGINAL_MEASURE_VALUE = input.MEASURE_VALUE;
                                input.ORIGINAL_MEASURE_COST = input.MEASURE_COST;
                                input.ORIGINAL_EFF_FROM_DT = input.EFF_FROM_DT;
                                input.ORIGINAL_EFF_TO_DT = input.EFF_TO_DT;
                                input.ORIGINAL_UOM = input.UOM;
                                input.ORIGINAL_CURRENCY_CODE = input.CURRENCY_CODE;
                                ++numInputs;
                                sampleValue += (double)input.MEASURE_VALUE;
                            }
                        }
                    }
                }
            }

            if (shouldSave)
            {
                try
                {
                    if (this.InputPeriod.PlantAccounting.EntityState == EntityState.Detached)
                    {
                        this.InputPeriod.Entities.AddToPLANT_ACCOUNTING(this.InputPeriod.PlantAccounting);
                        this.InputPeriod.PlantAccounting.LAST_UPD_BY = SessionManager.UserContext.UserName();
                    }
                    this.InputPeriod.PlantAccounting.FINALIZE_DT = updateDate;
                    this.InputPeriod.PlantAccounting.FINALIZE_ID = SessionManager.UserContext.Person.PERSON_ID;
                    status = this.InputPeriod.Entities.SaveChanges();  // save the metric history

                   // this.Entities.SaveChanges();  // save any updated profile measures
                }
                catch (Exception ex)
                {
                    //  SQMLogger.LogException(ex);
                    status = -1;
                }
            }
            
            return status;
        }

 
        private EHS_METRIC_HISTORY CreateHistoryMetric(decimal plantID, decimal measureID, DateTime periodDate, decimal measureUOM, string currencyCode)
        {
            EHS_METRIC_HISTORY metric = new EHS_METRIC_HISTORY();
            metric.PLANT_ID = plantID;
            metric.MEASURE_ID = measureID;
            metric.PERIOD_YEAR = periodDate.Year;
            metric.PERIOD_MONTH = periodDate.Month;
            metric.MEASURE_COST = metric.MEASURE_VALUE = 0;
            metric.INPUT_VALUE = metric.INPUT_COST = 0;
            metric.CURRENCY_CODE = currencyCode;
            metric.LAST_UPD_DT = DateTime.UtcNow;
            metric.LAST_UPD_BY = "";
            metric.STATUS = "N";  // new status
            metric.UOM_ID = measureUOM;
            this.InputPeriod.Entities.AddToEHS_METRIC_HISTORY(metric);
      
            return metric;
        }

        public static List<PeriodValue> CalcPeriodValue(DateTime fromDate, DateTime toDate, decimal amount, decimal cost)
        {
            List<PeriodValue> valueList = new List<PeriodValue>();

            int numPeriods = ((toDate.Year - fromDate.Year) * 12) + (toDate.Month - fromDate.Month) + 1;
            TimeSpan ts = toDate - fromDate;
            ts = ts.Add(new TimeSpan(24,0,0));
            decimal amtPerDay = amount/(decimal)(ts.TotalDays);
            decimal costPerDay = cost / (decimal)(ts.TotalDays);
            double ndays = ts.TotalDays;

            for (int n = 1; n <= numPeriods; n++)
            {
                PeriodValue pv = new PeriodValue();
                pv.PeriodDate = fromDate.AddMonths(n - 1);
                pv.Value = pv.Cost = 0;
                if (n == 1)
                {
                    if (fromDate.Month == toDate.Month)
                        ndays = toDate.Day - fromDate.Day + 1;
                    else 
                        ndays = DateTime.DaysInMonth(fromDate.Year, fromDate.Month) - fromDate.Day + 1;
                }
                else if (n == numPeriods)
                {
                    ndays = toDate.Day;
                }
                else
                {
                    ndays = DateTime.DaysInMonth(pv.PeriodDate.Year, pv.PeriodDate.Month); 
                }
                pv.Value = decimal.Round((decimal)ndays * amtPerDay, 2);
                pv.Cost = decimal.Round((decimal)ndays * costPerDay, 2);
 
                valueList.Add(pv);
            }
            return valueList;
        }

        private EHS_PROFILE_MEASURE UpdateMeasureSnapshot(EHS_PROFILE_MEASURE measure, DateTime periodDate, double inputValue)
        {
            measure.INPUT_SNAPSHOT = inputValue.ToString();
 
            measure.LAST_UPD_DT = periodDate;

            return measure;
        }
    }

    #endregion


    public class EHSProfilePeriod
    {        
        public DateTime PeriodDate
        {
            get;
            set;
        }
        public DateTime PeriodEndDate
        {
            get;
            set;
        }
        public int PeriodYear
        {
            get;
            set;
        }
        public int PeriodMonth
        {
            get;
            set;
        }
        public string LocationName
        {
            get;
            set;
        }
        public string Notes
        {
            get;
            set;
        }
        public List<EHS_PROFILE_INPUT> InputsList
        {
            get;
            set;
        }
        public PLANT_ACCOUNTING PlantAccounting
        {
            get;
            set;
        }
        public List<EHS_METRIC_HISTORY> HistoryList
        {
            get;
            set;
        }
        public DateTime DueDate
        {
            get;
            set;
        }
        public int NumResponsible
        {
            get;
            set;
        }
        public int NumComplete
        {
            get;
            set;
        }
        public int NumRequired
        {
            get;
            set;
        }
        public int NumRequiredComplete
        {
            get;
            set;
        }
        public double Progress
        {
            get;
            set;
        }
        public TaskStatus PeriodStatus
        {
            get;
            set;
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }

        public EHSProfilePeriod Initialize(DateTime periodDate, int dayDue)
        {
           // this.PeriodDate = periodDate;
            this.PeriodDate = new DateTime(periodDate.Year, periodDate.Month, 1);
            this.PeriodEndDate = new DateTime(periodDate.Year, periodDate.Month, DateTime.DaysInMonth(periodDate.Year, periodDate.Month));
            this.PeriodYear = periodDate.Year;
            this.PeriodMonth = periodDate.Month;
            this.DueDate = this.PeriodDate.AddDays(dayDue - 1);
            this.PlantAccounting = null;
            this.Entities = new PSsqmEntities();
            return this;
        }

        public int CountPeriodInputList(decimal prmrID)
        {
            return InputsList.Where(l => l.PRMR_ID == prmrID).Count();
        }

        public EHS_PROFILE_INPUT GetLastInput()
        {
            return InputsList.Where(l => l.STATUS != "N" && l.STATUS != "P").ToList().FirstOrDefault(l => l.LAST_UPD_DT == InputsList.Max(m => m.LAST_UPD_DT));
        }

        public List<EHS_PROFILE_INPUT> GetPeriodInputList(decimal prmrID)
        {
            return ( new List<EHS_PROFILE_INPUT>(InputsList.Where(l => l.PRMR_ID == prmrID).ToList()) );
        }

        public List<EHS_PROFILE_INPUT> GetPeriodMeasureInputList(decimal measureID)
        {
            List<EHS_PROFILE_INPUT> inputList = InputsList.Where(l => l.EHS_PROFILE_MEASURE.MEASURE_ID == measureID).ToList();
            return (inputList);
        }
 
        public EHS_PROFILE_INPUT GetPeriodInput(decimal prmrID, DateTime inputDate)
        {
            EHS_PROFILE_INPUT input = null;

            try
            {
                input = InputsList.FirstOrDefault(l=> l.PRMR_ID == prmrID  &&  l.INPUT_DT.ToString() == inputDate.ToString());
            }
            catch
            {
            }

            return input;
        }

        public bool DeletePeriodInput(EHS_PROFILE_INPUT input)
        {
            return InputsList.Remove(input);
        }

        public bool IsRequiredComplete()
        {
            return this.NumRequiredComplete >= this.NumRequired ? true : false;
        }

        public CURRENCY_XREF PeriodExchangeRate(PLANT plant)
        {
            CURRENCY_XREF exchangeRate = CurrencyConverter.CurrentRate(plant.CURRENCY_CODE, this.PeriodYear, this.PeriodMonth);
            return exchangeRate;
        }
    }

    public class PeriodValue
    {
        public DateTime PeriodDate
        {
            get;
            set;
        }
        public decimal Value
        {
            get;
            set;
        }
        public decimal Cost
        {
            get;
            set;
        }
    }
}
