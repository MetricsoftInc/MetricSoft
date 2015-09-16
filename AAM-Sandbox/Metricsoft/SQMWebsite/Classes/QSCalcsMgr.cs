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
    public enum QSAttribute {QS_ACTIVITY, PROBLEM_AREA, NONCONF_CATEGORY, NONCONF_ID, DISPOSITION, SEVERITY, PART_TYPE};

    public class SQMMetricMgr
    {
        public COMPANY Company
        {
            get;
            set;
        }
        public string perspective
        {
            get;
            set;
        }
        public QSCalcsCtl qsCtl
        {
            get;
            set;
        }
        public EHSCalcsCtl ehsCtl
        {
            get;
            set;
        }
        public TargetCalcsMgr TargetsCtl
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
        public int FYStartMonth
        {
            get;
            set;
        }
        public int AddFromYear
        {
            get;
            set;
        }
        public decimal[] PlantArray
        {
            get;
            set;
        }
        public decimal[] IncidentTypeArray
        {
            get;
            set;
        }
        public object ObjAny
        {
            get;
            set;
        }

        public SQMMetricMgr CreateNew(COMPANY company, string perspective, DateTime fromDate, DateTime toDate, int addFromYear, decimal[] plantArray)
        {
            this.Company = company;
            this.perspective = perspective;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.AddFromYear = addFromYear;
            this.PlantArray = plantArray;
            this.ObjAny = null;

            if (company.COMPANY_ID > 0 && company.FYSTART_MONTH.HasValue)
                this.FYStartMonth = (int)company.FYSTART_MONTH;
            else
                this.FYStartMonth = 1;

            if (SessionManager.SessionContext != null)
                this.TargetsCtl = new TargetCalcsMgr().CreateNew(this.Company.COMPANY_ID, SessionManager.FYStartDate(), toDate);

            return this;
        }

        public SQMMetricMgr CreateNew(COMPANY company, string perspective, DateTime fromDate, DateTime toDate, decimal[] plantArray)
        {
            this.CreateNew(company, perspective, fromDate, toDate, 0, plantArray);
            return this;
        }

        public SQMMetricMgr Load(DateIntervalType dateInterval, DateSpanOption dateSpanType)
        {
            switch (this.perspective)
            {
                case "0":  // overall company targets YTD
                    this.TargetsCtl = new TargetCalcsMgr().CreateNew(this.Company.COMPANY_ID, SessionManager.FYStartDate(), this.ToDate).LoadCurrentMetrics(true, true, DateIntervalType.month);
                    this.ehsCtl = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, dateSpanType).LoadMetricHistory(this.PlantArray, this.FromDate.AddYears(this.AddFromYear), this.ToDate, dateInterval, true).SetTargets(this.TargetsCtl);
                    break;
                case "E":
                    this.TargetsCtl = new TargetCalcsMgr().CreateNew(this.Company.COMPANY_ID, SessionManager.FYStartDate(), this.ToDate).LoadCurrentMetrics(true, true, DateIntervalType.month);
                    this.ehsCtl = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, dateSpanType).LoadMetricHistory(this.PlantArray, this.FromDate.AddYears(this.AddFromYear), this.ToDate, dateInterval, true).SetTargets(this.TargetsCtl);
                    break;
                case "HS":
                    //this.TargetsCtl.LoadCurrentMetrics(true, true, DateIntervalType.year);
                    this.ehsCtl = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, dateSpanType).LoadMetricHistory(this.PlantArray, this.FromDate.AddYears(this.AddFromYear), this.ToDate, dateInterval, true).LoadIncidentHistory(this.PlantArray, this.FromDate.AddYears(this.AddFromYear), this.ToDate, dateInterval).SetTargets(this.TargetsCtl);
                    break;
                case "I":
                    this.ehsCtl = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, dateSpanType).LoadMetricInputs(this.FromDate, this.ToDate, this.PlantArray);
                     break;
                case "IR":
                     this.ehsCtl = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, dateSpanType).LoadMetricInputs(this.FromDate, this.ToDate, this.PlantArray, "IR");
                     break;
                case "QS":
                case "CST":
                case "RCV":
                     this.TargetsCtl.LoadCurrentMetrics(true, true, DateIntervalType.year);
                    this.qsCtl = new QSCalcsCtl().CreateNew().LoadIncidentHistory(this.PlantArray, this.FromDate.AddYears(this.AddFromYear), this.ToDate, this.perspective, dateInterval).SetTargets(this.TargetsCtl);
                    break;
                default:
                    break;
            }

            return this;
        }

        public CalcsResult CalcsMethods(decimal[] plantArray, string itemCalcsMethod, string calcsScope, string calcsStat, int controlType, int seriesOrder)
        {
            if (controlType == 1 || string.IsNullOrEmpty(calcsScope))
                return new CalcsResult().Initialize();

            CalcsResult rslt = null;

            string calcsMethod = itemCalcsMethod;

            if (this.perspective == "HS")  // determine which data set for EH metrics (incidents or plant-accounting)
            {
                string[] ids = calcsScope.Split(',');
                int id = 0;
                if (int.TryParse(ids[0], out id) == true && id < 1000000)
                    calcsMethod = "IN";     // incident topic id's
            }

            if (controlType == 5 || controlType == 10 || controlType == 20 || controlType == 60)
                calcsMethod += "STAT";
            else
                calcsMethod += "SERIES";

            DateTime fromDate = this.FromDate;

            switch (calcsMethod)
            {
                case "ESTAT":  // environment stats
                case "HSSTAT": // HS stats
                    this.ehsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).EHMetric(this.ehsCtl.GetPlantsByScope(plantArray), this.ehsCtl.GetMetricsByScope(), fromDate, this.ToDate);
                    rslt = this.ehsCtl.Results;
                    break;
                case "ESERIES":  // environment series
                case "HSSERIES":  // HS series
                    this.ehsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).MetricSeries((EHSCalcsCtl.SeriesOrder)seriesOrder, fromDate, this.ToDate, this.ehsCtl.GetPlantsByScope(plantArray), this.ehsCtl.GetMetricsByScope(), 0m);
                    rslt = this.ehsCtl.Results;
                    break;
                case "ISERIES":
                case "IRSERIES":
                    this.ehsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).InputsSeries((EHSCalcsCtl.SeriesOrder)seriesOrder, plantArray, this.ehsCtl.GetMetricsByScope(), fromDate, this.ToDate);
                    rslt = this.ehsCtl.Results;
                    break;
                case "INSTAT": // incident stats
                    this.ehsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).IncidentStat(plantArray, this.ehsCtl.GetIncidentTopics(), fromDate, this.ToDate);
                    rslt = this.ehsCtl.Results;
                    break;
                case "INSERIES":  // incident series
                    this.ehsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).IncidentSeries((EHSCalcsCtl.SeriesOrder)seriesOrder, plantArray, fromDate, this.ToDate, this.ehsCtl.GetIncidentTopics());
                    rslt = this.ehsCtl.Results;
                    break;

                case "QSSTAT":
                    this.qsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).QSMetric(plantArray, new decimal[] { }, fromDate, this.ToDate);
                    rslt =  this.qsCtl.Results;
                    break;
                case "QSSERIES":
                    this.qsCtl.SetCalcParams(itemCalcsMethod, calcsScope, calcsStat, seriesOrder).MetricSeries(fromDate, this.ToDate, plantArray);
                    rslt = this.qsCtl.Results;
                    break;
                    
                default:
                    break;
            }

            return rslt;
        }

        public CalcsResult DaysElapsedLTC(COMPANY company, decimal[] plantArray)
        {
            CalcsResult results = new CalcsResult().Initialize();

            SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(company, "HS", DateTime.Now, DateTime.Now, plantArray).Load(DateIntervalType.fuzzy, DateSpanOption.SelectRange);
            metricMgr.CalcsMethods(plantArray, "HS", "63", SStat.deltaDy.ToString(), 5, 1);
            results = metricMgr.ehsCtl.Results;
            if (!results.ValidResult)
            {
                PLANT plant = SQMModelMgr.LookupPlant(plantArray[0]);
                if (plant != null && plant.PLANT_START_DT.HasValue)
                {
                    results.Result = (decimal)Math.Truncate(DateTime.Now.Subtract((DateTime)plant.PLANT_START_DT).TotalDays);
                    results.ValidResult = true;
                }
            }
            return results;
        }

        public static List<decimal[]> GetPlantGroups(decimal[] plantArray, decimal orgID)
        {
            List<decimal[]> groupList = new List<decimal[]>();

            List<BusinessLocation> plantList = SessionManager.PlantList.Where(l => plantArray.Contains(l.Plant.PLANT_ID)).ToList();
            decimal[] orgIDS;

            if (orgID == 0)
            {
                orgIDS = plantList.Select(l => (decimal)l.Plant.BUS_ORG_ID).Distinct().ToArray();
            }
            else
            {
                orgIDS = new decimal[1] { orgID };
            }

            foreach (decimal org in orgIDS)
            {
                decimal[] plantIDS = plantList.Where(l => l.Plant.BUS_ORG_ID == org).Select(l => l.Plant.PLANT_ID).ToArray();
                groupList.Add(plantIDS);
            }


            return groupList;
        }
    }

    public partial class QSCalcs
    {
        public decimal result;

        public List<QualityIncidentData> Records
        {
            get;
            set;
        }
        public Dictionary<string, string> AttributeXref
        {
            get;
            set;
        }
        public List<NONCONFORMANCE> NonConfList
        {
            get;
            set;
        }
        public object anyList
        {
            get;
            set;
        }

        public List<QualityIncidentData> Select(DateTime fromDate, DateTime toDate, decimal[] plantArray, decimal[] partArray)
        {
            this.Records = this.Records.Where(l => l.Incident.INCIDENT_DT >= fromDate && l.Incident.INCIDENT_DT <= toDate).ToList();
            
            if (plantArray.Length > 0)
            {
                this.Records = this.Records.Where(l => plantArray.Contains((decimal)l.Incident.DETECT_PLANT_ID)  || plantArray.Contains((decimal)l.Incident.RESP_PLANT_ID)).ToList();
            }
            if (partArray.Length > 0)
            {
                this.Records = this.Records.Where(l => partArray.Contains(l.QIIssue.PART_ID)).ToList();
            }
            
            return this.Records;
        }

    }


    public class QSCalcsCtl
    {
     // public enum SeriesOrder { SumAll, MeasurePlant, PlantMeasure, PeriodMeasure, YearMeasure, PartMeasure };
        public enum SeriesOrder { SumAll, MeasurePlant, PlantMeasure, PeriodMeasurePlant, YearMeasurePlant, PeriodMeasure, YearMeasure, PartMeasure, MeasureSeries };
    
        public PART[] PartScope
        {
            get;
            set;
        }
        public string SubScope
        {
            get;
            set;
        }
        public List<QualityIncidentData> RecordList
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }
        public QSCalcs Calc
        {
            get;
            set;
        }
        public TargetCalcsMgr TargetCtl
        {
            get;
            set;
        }
        public CalcsResult Results
        {
            get;
            set;
        }
        public DateIntervalType DateInterval
        {
            get;
            set;
        }
        public List<NONCONFORMANCE> NCCategoryList
        {
            get;
            set;
        }
        public List<NONCONFORMANCE> NonConfList
        {
            get;
            set;
        }
        public string CalcsMethod
        {
            get;
            set;
        }
        public string MetricScope
        {
            get;
            set;
        }
        public string Calculation
        {
            get;
            set;
        }
        public SStat Stat
        {
            get;
            set;
        }
        public SeriesOrder Seriesorder
        {
            get;
            set;
        }
 
        public List<GaugeSeries> metricSeries;
        public List<GaugeSeriesItem> dataList;

        public decimal sumx;
        public decimal sumx2;
        public decimal mean;
        public decimal minimum;
        public decimal maximum;
        public decimal range;
        public decimal stdDev;
        public decimal vars;
  
        public QSCalcsCtl CreateNew()
        {
            this.RecordList = new List<QualityIncidentData>();
            this.Entities = new PSsqmEntities();
            this.Calc = new QSCalcs();
            this.Calc.Records = new List<QualityIncidentData>();
            this.Results = new CalcsResult().Initialize();
            this.DateInterval = DateIntervalType.fuzzy;
            return this;
        }
        public QSCalcsCtl SetTargets(TargetCalcsMgr targetMgr)
        {
            this.TargetCtl = targetMgr;
            return this;
        }
        public QSCalcsCtl InitCalc()
        {
            this.Calc.Records = this.RecordList;
            return this;
        }
        public PLANT GetPlant(decimal plantID)
        {
            PLANT plant;
            plant = this.RecordList.Where(l => l.Plant.PLANT_ID == plantID).Select(l => l.Plant).FirstOrDefault();
            if (plant == null)
                plant = SQMModelMgr.LookupPlant(plantID);
            return plant;
        }

        public QSCalcsCtl SetCalcParams(string calcsMethod, string scope, string stat, int seriesOrder)
        {
            this.CalcsMethod = calcsMethod;
            string[] scopeArgs = WebSiteCommon.SplitString(scope, '|');
            string[] statArgs = WebSiteCommon.SplitString(stat, '|');

            try
            {
                this.MetricScope = scopeArgs[0];
                if (scopeArgs.Length > 1)
                    this.SubScope = scopeArgs[1];
                else
                    this.SubScope = "";

                if (statArgs.Length > 1)
                {
                    this.Calculation = statArgs[0];
                    this.Stat = (SStat)Enum.Parse(typeof(SStat), statArgs[1], true);
                }
                else
                {
                    this.Calculation = "";
                    this.Stat = (SStat)Enum.Parse(typeof(SStat), statArgs[0], true);
                }
                this.Seriesorder = (SeriesOrder)seriesOrder;

            }
            catch
            {
                this.Stat = SStat.sum;
                this.Seriesorder = SeriesOrder.SumAll;
            }

            return this;
        }

        public string[] GetFactorsByScope(string calcScope)
        {
            string[] factorArray;
           
            if (calcScope.Contains(','))
            {
                factorArray = calcScope.Split(',');
            }
            else 
            {
                switch (calcScope)
                {
                    case "issueCost":
                        factorArray = new string[] { "actCost", "potCost" };
                        break;
                    default:
                        factorArray = new string[] {calcScope };
                        break;
                }
            }

            return factorArray;
        }

        public void SetAttributeRef(QSAttribute atrType)
        {
            switch (atrType)
            {
                case QSAttribute.QS_ACTIVITY:
                    this.Calc.AttributeXref = WebSiteCommon.GetXlatList("qualityActivity", "", "short");
                    break;
                case QSAttribute.PROBLEM_AREA:
                    this.Calc.AttributeXref = WebSiteCommon.GetXlatList("problemArea", "", "short");
                    break;
                case QSAttribute.NONCONF_CATEGORY:
                    this.NCCategoryList = SQMResourcesMgr.SelectNonconfCategoryList("");
                    string[] catList = this.RecordList.Where(r => r.Nonconform != null && !string.IsNullOrEmpty(r.Nonconform.NONCONF_CATEGORY)).Select(r => r.Nonconform.NONCONF_CATEGORY).Distinct().ToArray();
                    this.Calc.AttributeXref = new Dictionary<string, string>();
                    foreach (string cat in catList)
                    {
                        NONCONFORMANCE nc = this.NCCategoryList.Where(l=> l.NONCONF_CD == cat).FirstOrDefault();
                        this.Calc.AttributeXref.Add(nc.NONCONF_CD.ToString(), nc.NONCONF_NAME);
                    }
                    break;
                case QSAttribute.NONCONF_ID:
                    this.NonConfList = this.RecordList.Where(r => r.Nonconform != null).Select(r => r.Nonconform).Distinct().ToList();
                    this.Calc.AttributeXref = new Dictionary<string, string>();
                    foreach (NONCONFORMANCE nc in this.NonConfList)
                    {
                        this.Calc.AttributeXref.Add(nc.NONCONF_ID.ToString(), nc.NONCONF_NAME);
                    }
                    break;
                case QSAttribute.DISPOSITION:
                    this.Calc.AttributeXref = WebSiteCommon.GetXlatList("NCDisposition", "", "short");
                    break;
                case QSAttribute.SEVERITY:
                    this.Calc.AttributeXref = WebSiteCommon.GetXlatList("incidentSeverity", "", "short");
                    break;
                case QSAttribute.PART_TYPE:
                    this.Calc.AttributeXref = new Dictionary<string, string>();
                    foreach (PART_ATTRIBUTE pat in SQMModelMgr.SelectPartAttributeList("TYPE", "", false))
                    {
                        this.Calc.AttributeXref.Add(pat.ATTRIBUTE_CD, pat.ATTRIBUTE_VALUE);
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        public Dictionary<string, string> SetScopeRef(string calcScope)
        {
            this.Calc.AttributeXref = new Dictionary<string, string>();
            this.Calc.AttributeXref.Add("", calcScope);
            return this.Calc.AttributeXref;
        }

        public PART GetPart(decimal partID)
        {
            PART part;
            part = this.RecordList.Where(l => l.Part.PART_ID == partID).Select(l => l.Part).FirstOrDefault();
            if (part == null)
                part = SQMModelMgr.LookupPart(this.Entities, partID, "", 1, false);
            return part;
        }

        public QSCalcsCtl LoadIncidentHistory(decimal[] plantIDS, DateTime fromDate, DateTime toDate, string qsActivity, DateIntervalType dateIntervalType)
        {
            this.DateInterval = dateIntervalType;
            this.RecordList = new List<QualityIncidentData>();
            try
            {
                this.RecordList = (from i in this.Entities.INCIDENT
                                  join o in this.Entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                  join t in this.Entities.QI_OCCUR_ITEM on o.QIO_ID equals t.QIO_ID
                                  join s in this.Entities.QI_OCCUR_NC on t.QIO_ITEM_ID equals s.QIO_ITEM_ID into s_t 
                                  join p in this.Entities.PART on o.PART_ID equals p.PART_ID into p_i
                                  join l in this.Entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID into l_i
                                  where (i.INCIDENT_TYPE == "QI" && (i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate) 
                                  &&  (plantIDS.Contains((decimal)i.DETECT_PLANT_ID)  ||  plantIDS.Contains((decimal)i.RESP_PLANT_ID))
                                  &&  o.QS_ACTIVITY == qsActivity)
                                  from s in s_t.DefaultIfEmpty()
                                  from p in p_i.DefaultIfEmpty()
                                  from l in l_i.DefaultIfEmpty()
                                  join nc in this.Entities.NONCONFORMANCE on s.NONCONF_ID equals nc.NONCONF_ID into nc_s
                                  from nc in nc_s.DefaultIfEmpty()
                                  orderby(i.INCIDENT_DT)  
                                  select new QualityIncidentData
                                  {
                                      Incident = i,
                                      QIIssue = o,
                                      QIItem = t,
                                      QISample = s,
                                      Part = p,
                                      Plant = l,
                                      Nonconform = nc 
                                  }).ToList();
            }
            catch (Exception ex)
            {
         //       SQMLogger.LogException(ex);
            }

            return this;
        }

        public List<QualityIncidentData> SetIncidentHistory(List<QualityIncidentData> issueList)
        {
            this.RecordList = new List<QualityIncidentData>();
            this.RecordList.AddRange(issueList);
            return this.RecordList;
        }

        public int FilterByDate(DateTime fromDate, DateTime toDate)
        {
            this.Calc.Records = this.RecordList.Where(l => l.Incident.INCIDENT_DT >= fromDate && l.Incident.INCIDENT_DT <= toDate).ToList();
            return 0;
        }
        public int FilterByPlant(decimal[] plantIDS)
        {
            this.Calc.Records = this.RecordList.Where(l => plantIDS.Contains((decimal)l.Incident.DETECT_PLANT_ID) || plantIDS.Contains((decimal)l.Incident.RESP_PLANT_ID)).ToList();
            return 0;
        }
        public int FilterByPerson(decimal[] personIDS)
        {
            this.Calc.Records = this.RecordList.Where(l => personIDS.Contains((decimal)l.Incident.CREATE_PERSON)).ToList();
            return 0;
        }
        public int FilterByPart(decimal[] partIDS)
        {
            this.Calc.Records = this.RecordList.Where(l => partIDS.Contains((decimal)l.QIIssue.PART_ID)).ToList();
            return 0;
        }
        public int FilterByPartType(string[] patCDS)
        {
            this.Calc.Records = this.RecordList.Where(l => patCDS.Contains(l.QIIssue.PART_TYPE)).ToList();
            return 0;
        }
        public int FilterByAttribute(string atrScope, string[] atrArray)
        {
            QSAttribute atr;
            if (Enum.TryParse(atrScope, out atr))
            switch (atr)
            {
                case QSAttribute.QS_ACTIVITY:
                    this.Calc.Records = this.RecordList.Where(r => atrArray.Contains(r.QIIssue.QS_ACTIVITY)).ToList();
                    break;
                case QSAttribute.PROBLEM_AREA:
                    this.Calc.Records = this.RecordList.Where(r => r.QISample != null && atrArray.Contains(r.QISample.PROBLEM_AREA)).ToList();
                    break;
                case QSAttribute.NONCONF_CATEGORY:
                    this.Calc.Records = this.RecordList.Where(r => r.Nonconform != null && atrArray.Contains(r.Nonconform.NONCONF_CATEGORY)).ToList();
                    break;
                case QSAttribute.NONCONF_ID:
                    this.Calc.Records = this.RecordList.Where(r => r.QISample != null && atrArray.Contains(r.QISample.NONCONF_ID.ToString())).ToList();
                    break;
                case QSAttribute.DISPOSITION:
                    this.Calc.Records = this.RecordList.Where(r => atrArray.Contains(r.QIIssue.DISPOSITION)).ToList();
                    break;
                case QSAttribute.SEVERITY:
                    this.Calc.Records = this.RecordList.Where(r => atrArray.Contains(r.QIIssue.SEVERITY)).ToList();
                    break;
                case QSAttribute.PART_TYPE:
                    this.Calc.Records = this.RecordList.Where(r => atrArray.Contains(r.QIIssue.PART_TYPE)).ToList();
                    break;
                default:
                    break;
            }
            return 0;
        }

        private decimal MetricFactor(decimal[] plantArray, decimal[] partArray, DateTime fromDate, DateTime toDate, string factor, string attribute, Func<decimal[], int> addFilter )
        {
            this.Results.ValidResult = true;
            decimal metricFactor = 0;
            try
            {
                switch (factor)
                {
                    case "count":
                        metricFactor = (decimal)this.Calc.Records.Count();
                        break;
                    case "ppm":
                        decimal totqty = (decimal)this.Calc.Records.Select(l => l.QIItem.TOTAL_QTY).Sum();
                        decimal totnc = (decimal)this.Calc.Records.Select(l => l.QIItem.TOTAL_NC_QTY).Sum();
                        if (totqty > 0)
                            metricFactor = (totnc / totqty) * 1000000;
                        else
                            metricFactor = 0;
                        break;
                    case "totQty":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIItem.TOTAL_QTY).Sum();
                        break;
                    case "qty":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIItem.INSPECT_QTY).Sum();
                        break;
                    case "ncQty":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIItem.INSPECT_NC_QTY).Sum();
                        break;
                    case "totNcQty":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIItem.TOTAL_NC_QTY).Sum();
                        break;
                    case "actCost":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIIssue.EST_ACT_COST).Sum();
                        break;
                    case "potCost":
                        metricFactor = (decimal)this.Calc.Records.Select(l => l.QIIssue.EST_POT_COST).Sum();
                        break;
                    default:
                        QSAttribute atr;
                        if (Enum.TryParse(this.Calculation, out atr))
                        {
                            switch (atr)
                            {
                                case QSAttribute.QS_ACTIVITY:
                                    metricFactor = this.Calc.Records.Where(r => r.QIIssue.QS_ACTIVITY == attribute).Count();
                                    break;
                                case QSAttribute.PROBLEM_AREA:
                                    metricFactor = this.Calc.Records.Where(r => r.QISample != null && r.QISample.PROBLEM_AREA == attribute).Count();
                                    break;
                                case QSAttribute.NONCONF_CATEGORY:
                                    metricFactor = this.Calc.Records.Where(r => r.Nonconform != null && r.Nonconform.NONCONF_CATEGORY == attribute).Count();
                                    break;
                                case QSAttribute.NONCONF_ID:
                                    metricFactor = this.Calc.Records.Where(r => r.QISample != null && r.QISample.NONCONF_ID == Convert.ToDecimal(attribute)).Count();
                                    break;
                                case QSAttribute.DISPOSITION:
                                    metricFactor = this.Calc.Records.Where(r => r.QIIssue.DISPOSITION == attribute).Count();
                                    break;
                                case QSAttribute.SEVERITY:
                                    metricFactor = this.Calc.Records.Where(r => r.QIIssue.SEVERITY == attribute).Count();
                                    break;
                                case QSAttribute.PART_TYPE:
                                    metricFactor = this.Calc.Records.Where(r => r.QIIssue.PART_TYPE == attribute).Count();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
                this.Results.Result = metricFactor;
            }
            catch
            {
                this.Results.ValidResult = false;
            }

            return metricFactor;
        }


        public bool QSMetric(decimal[] plantArray, decimal[] partArray, DateTime fromDate, DateTime toDate)
        {
            this.Results.Result = 0;
            bool status = this.Results.ValidResult = true;

            try
            {
                this.InitCalc().Calc.Select(fromDate, toDate, plantArray, partArray);
                this.Results.Result = MetricFactor(plantArray, partArray, fromDate, toDate,  this.Calculation, "", null);
            }
            catch
            {
                this.Results.ValidResult = false;
            }
            return status;
        }

        public int MetricSeries(DateTime fromDate, DateTime toDate, decimal[] plantArray)
        {
            int status = 0;
            int item = 0;
            GaugeSeries series;
            
            this.Results.Initialize();

            QSAttribute atr;
            bool isAttribute;
            if ((isAttribute = Enum.TryParse(this.Calculation, out atr)) == false)
                this.SetScopeRef(this.Calculation);

            List<object> foList = new List<object>();
            string text = "";

            switch (this.Seriesorder)
            {
                case QSCalcsCtl.SeriesOrder.MeasureSeries:
                    switch (this.MetricScope)
                    {
                        case "plant":
                             foList = this.RecordList.Where(r=> plantArray.Contains(r.PlantResponsible.PLANT_ID)).Select(r => (object)r.PlantResponsible).Distinct().ToList();
                            break;
                        case "plantResp":  
                            foList = this.RecordList.Select(r => (object)r.PlantResponsible).Distinct().ToList();
                            break;
                        case "plantDetect":
                            foList = this.RecordList.Select(r => (object)r.Plant).Distinct().ToList();
                            break;
                        case "person":
                            foList = this.RecordList.Select(r => (object)r.Person).Distinct().ToList();
                            break;
                        case "partType":
                            List<string> pat = this.RecordList.Select(r => r.QIIssue.PART_TYPE).Distinct().ToList();
                            foList = SQMModelMgr.SelectPartAttributeList("TYPE", "", false).Where(l => pat.Contains(l.ATTRIBUTE_CD)).Select(l => (object)l).ToList();
                            break;
                        case "month":
                            foreach (WebSiteCommon.DatePeriod pd in WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, DateSpanOption.FYEffTimespan, ""))
                                foList.Add(pd);
                            break;
                        default:
                            if ((isAttribute = Enum.TryParse(this.MetricScope, out atr)))
                            {
                                this.SetAttributeRef(atr);
                                foreach (KeyValuePair<string, string> xlat in this.Calc.AttributeXref)
                                    foList.Add(xlat);
                            }
                            break;
                    }

                    foreach (string factor in this.GetFactorsByScope(this.Calculation))
                    {
                        series = new GaugeSeries().CreateNew(1, factor, "");
                        foreach (object fo in foList)
                        {
                            switch (this.MetricScope)
                            {
                                case "plant":
                                case "plantDetect":
                                case "plantResp":
                                    text = ((PLANT)fo).PLANT_NAME;
                                    this.FilterByPlant(new decimal[1] { ((PLANT)fo).PLANT_ID });
                                    break;
                                case "person":
                                    text = SQMModelMgr.FormatPersonListItem((PERSON)fo);
                                    this.FilterByPerson(new decimal[1] { ((PERSON)fo).PERSON_ID });
                                    break;
                                case "partType":
                                    text = ((PART_ATTRIBUTE)fo).ATTRIBUTE_VALUE;
                                    this.FilterByPartType(new string[1] { ((PART_ATTRIBUTE)fo).ATTRIBUTE_CD });
                                    break;
                                case "month":
                                    WebSiteCommon.DatePeriod pd = (WebSiteCommon.DatePeriod)fo;
                                    text = pd.Label;
                                    this.FilterByDate(pd.FromDate, pd.ToDate);
                                    break;
                                default:
                                    text = ((KeyValuePair<string, string>)fo).Value;
                                    this.FilterByAttribute(this.MetricScope, new string[1] { ((KeyValuePair<string, string>)fo).Key});
                                    break;
                            }

                            this.MetricFactor(plantArray, new decimal[0] { }, fromDate, toDate, factor, "",  null);
                            if (this.Results.ValidResult)
                            {
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, item++, text, this.Results));
                            }
                        }
                        this.Results.metricSeries.Add(series);
                    }
                    break;

                case QSCalcsCtl.SeriesOrder.SumAll:
                    if (isAttribute)
                    {
                        this.InitCalc();
                        this.SetAttributeRef(atr);
                        GaugeSeries sumSeries = new GaugeSeries().CreateNew(0, "", "");
                        foreach (KeyValuePair<string, string> xlat in this.Calc.AttributeXref)
                        {
                            if (this.MetricFactor(plantArray, new decimal[0] { }, fromDate, toDate, this.Calculation, xlat.Key, null) > 0)
                            {
                                sumSeries.ItemList.Add(new GaugeSeriesItem().CreateNew(1, item++, 0, this.Results.Result, xlat.Value));
                            }
                        }
                        if (sumSeries.ItemList.Count > 0)
                            this.Results.metricSeries.Add(sumSeries);

                        if (this.Results.metricSeries.Count > 0)
                            this.Results.ValidResult = true;

                        if (this.Stat == SStat.pct)  // transform series from values to percentages
                            this.Results.TransformSeries(this.Stat, EHSCalcsCtl.SeriesOrder.MeasureSeries);
                    }
                    break;

                default:
                    break;
            }
            return status;
        }
    }
}