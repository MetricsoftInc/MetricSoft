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
    public enum SStat { none, value, sum, pctChange, deltaDy, sumCost, count, cost, pct, pctReduce};
    public enum HSAttr { none, type};

    public class AttributeValue
    {
        public string Key
        {
            get;
            set;
        }
        public decimal Value
        {
            get;
            set;
        }

        public AttributeValue CreateNew(string key, decimal value)
        {
            this.Key = key;
            this.Value = value;
            return this;
        }
    }

    public class CalcsResult
    {
        public int Status
        {
            get;
            set;
        }
        public bool ValidResult
        {
            get;
            set;
        }
        public decimal Result
        {
            get;
            set;
        }
        public bool ValidResult2
        {
            get;
            set;
        }
        public decimal Result2
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public string FactorText
        {
            get;
            set;
        }
        public DateTime ResultDate
        {
            get;
            set;
        }
        public List<GaugeSeries> metricSeries
        {
            get;
            set;
        }
        public DataTable SummaryTable
        {
            get;
            set;
        }
        public object ResultObj
        {
            get;
            set;
        }
        public CalcsResult Initialize()
        {
            this.metricSeries = new List<GaugeSeries>();
            this.ValidResult = true;
            this.ValidResult2 = false;
            this.Status = 0;
            this.Result = this.Result2 = 0;
            this.Text = this.FactorText = "";
            this.ResultDate = DateTime.MinValue;
            this.SummaryTable = new DataTable();
            this.ResultObj = null;

            return this;
        }

        public CalcsResult TrimSeries(decimal valueToTrim)
        {
            List<GaugeSeriesItem> itemList = new List<GaugeSeriesItem>();
            GaugeSeriesItem sumItem = null;

            foreach (GaugeSeries series in this.metricSeries)
            {
                foreach (GaugeSeriesItem item in series.ItemList)
                {
                    if ((sumItem = itemList.Where(l => l.Text == item.Text).FirstOrDefault()) == null)
                    {
                        sumItem = new GaugeSeriesItem();
                        sumItem.Text = item.Text;
                        sumItem.YValue = 0;
                        itemList.Add(sumItem);
                    }
                    sumItem.YValue += item.YValue;
                }
            }

            foreach (GaugeSeries series in this.metricSeries)
            {
                for (int i = series.ItemList.Count-1; i >= 0; i--)
                {
                    GaugeSeriesItem item = series.ItemList[i];
                    if (itemList.Where(l => l.Text == item.Text && l.YValue == valueToTrim).Count() > 0)
                        series.ItemList.Remove(item);
                }
            }

            return this;
        }

        public CalcsResult TransformSeries(SStat transform, EHSCalcsCtl.SeriesOrder seriesOrder)
        {
            if (transform == SStat.pct)
            {
                decimal total = 0;
                List<GaugeSeriesItem> tempList = new List<GaugeSeriesItem>();
                GaugeSeriesItem tempItem = null;
                foreach (GaugeSeries gs in this.metricSeries)
                {
                    foreach (GaugeSeriesItem gsi in gs.ItemList)
                    {
                        tempItem = tempList.Where(l => l.Text == gsi.Text).FirstOrDefault();
                        if (tempItem != null && tempItem.Text == gsi.Text)
                        {
                            tempItem.YValue += gsi.YValue;
                        }
                        else
                        {
                            tempItem = new GaugeSeriesItem();
                            tempItem.Text = gsi.Text;
                            tempItem.YValue = gsi.YValue;
                            tempList.Add(tempItem);
                        }
                        total += gsi.YValue;
                    }
                }
                foreach (GaugeSeries gs in this.metricSeries)
                {
                    foreach (GaugeSeriesItem gsi in gs.ItemList)
                    {
                        if (seriesOrder == EHSCalcsCtl.SeriesOrder.MeasureSeries)
                        {
                            if (total != 0)
                                gsi.YValue = Decimal.Round(gsi.YValue / total * 100m);
                            else
                                gsi.YValue = 0;
                        }
                        else
                        {
                            tempItem = tempList.Where(l => l.Text == gsi.Text).FirstOrDefault();
                            if (tempItem.YValue != 0)
                                gsi.YValue = Decimal.Round(gsi.YValue / tempItem.YValue * 100m);
                            else
                                gsi.YValue = 0;
                        }
                    }
                }
            }
            return this;
        }
    }

    public partial class EHSCalcs
    {
        public int NumPeriods;
        public int NumPlants;
        public int NumMeasures;
        public decimal result;

        public List<EHS_METRIC_HISTORY> Metrics
        {
            get;
            set;
        }
        public List<PLANT_ACCOUNTING> Accounting
        {
            get;
            set;
        }
        public List<EHSIncidentData> Incidents
        {
            get;
            set;
        }
        public List<EHS_PROFILE_MEASURE> ProfileMeasures
        {
            get;
            set;
        }
        public Dictionary<string, string> AttributeXref
        {
            get;
            set;
        }
        public List<DateTime> FromDates
        {
            get;
            set;
        }
        public List<DateTime> ToDates
        {
            get;
            set;
        }
        public List<AttributeValue> AttrList
        {
            get;
            set;
        }

        public List<EHS_METRIC_HISTORY> Select(DateTime fromDate, DateTime toDate, decimal[] plantArray, decimal[] measureArray)
        {
            this.Metrics = this.Metrics.Where(l => new DateTime(l.PERIOD_YEAR, l.PERIOD_MONTH, 1) >= fromDate && new DateTime(l.PERIOD_YEAR, l.PERIOD_MONTH, DateTime.DaysInMonth(l.PERIOD_YEAR, l.PERIOD_MONTH)) <= toDate).ToList();

            if ((NumPlants = plantArray.Length) > 0)
            {
                this.Metrics = this.Metrics.Where(l => plantArray.Contains(l.PLANT_ID)).ToList();
            }
            if ((NumMeasures = measureArray.Length) > 0)
            {
                this.Metrics = this.Metrics.Where(l => measureArray.Contains(l.MEASURE_ID)).ToList();
            }

            return this.Metrics;
        }

        // EHS Incident filter
        public List<EHSIncidentData> SelectIncidents(DateTime fromDate, DateTime toDate, decimal[] plantArray, decimal[] topicArray)
        {
            this.Incidents = this.Incidents.Where(l => l.Incident.INCIDENT_DT >= fromDate && l.Incident.INCIDENT_DT <= toDate).ToList();
            if ((NumPlants = plantArray.Length) > 0)
            {
                this.Incidents = this.Incidents.Where(l => plantArray.Contains((decimal)l.Incident.DETECT_PLANT_ID)).ToList();
            }
            if ((NumMeasures = topicArray.Length) > 0)
            {
                List<EHSIncidentData> tempList = new List<EHSIncidentData>();
                foreach (EHSIncidentData data in this.Incidents)
                {

                    if (data.EntryList.Where(l => topicArray.Contains(l.INCIDENT_QUESTION_ID)).Count() > 0)
                        tempList.Add(data);
                }
                this.Incidents = tempList.ToList();
            }
            return this.Incidents;
        }

    }

    public class EHSCalcsCtl
    {
        public enum SeriesOrder { MeasureSeries, MeasurePlant, PlantMeasure, PeriodMeasurePlant, YearMeasurePlant, PeriodMeasure, YearMeasure, YearPlant, YearPlantAvg, PlantMeasureTotal, PlantMeasureAvg, SumTotal, TimespanSeries, PeriodMeasureYOY, PeriodSum, PeriodSumYOY, YearSum };
        //                            0               1               2               3                   4               5               6           7           8           9                   10            11              12              13              14          15          16
        public enum MetricElement { value, cost };
        public enum AccountingElement { cost, revenue, production, throughput, timeworked, timelost, recordedcases, timelostcases};
 
        public DateIntervalType DateInterval
        {
            get;
            set;
        }
        public DateSpanOption DateSpanType
        {
            get;
            set;
        }
        public List<EHS_PROFILE_INPUT> InputsList
        {
            get;
            set;
        }
        public List<EHS_METRIC_HISTORY> MetricHst
        {
            get;
            set;
        }
        public List<PLANT_ACCOUNTING> PlantHst
        {
            get;
            set;
        }
        public List<EHSIncidentData> IncidentHst
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }
        public EHSCalcs Calc
        {
            get;
            set;
        }
        public TargetCalcsMgr TargetCtl 
        {
            get;
            set;
        }
        public int FYStartMonth
        {
            get;
            set;
        }
        public CalcsResult Results
        {
            get;
            set;
        }
        public List<INCIDENT_TYPE> IncidentTypeList
        {
            get;
            set;
        }
        public List<PLANT> PlantList
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
        public string SubScope
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
        public EHSCalcsCtl.SeriesOrder Seriesorder
        {
            get;
            set;
        }
        public decimal[] TopicSelects
        {
            get;
            set;
        }

        public EHSCalcsCtl CreateNew(int FYStartMonth, DateSpanOption dateSpanType)
        {
            this.MetricHst = new List<EHS_METRIC_HISTORY>();
            this.PlantHst = new List<PLANT_ACCOUNTING>();
            this.Entities = new PSsqmEntities();
            this.Calc = new EHSCalcs();
            this.Calc.Metrics = new List<EHS_METRIC_HISTORY>();
            this.Calc.Accounting = new List<PLANT_ACCOUNTING>();
            this.Calc.AttributeXref = new Dictionary<string, string>();
            this.Results = new CalcsResult().Initialize();
            this.FYStartMonth = FYStartMonth;   // use for year over year calculations
            this.DateInterval = DateIntervalType.fuzzy;
            this.DateSpanType = dateSpanType;
            this.PlantList = new List<PLANT>();
            //this.TopicSelects = new decimal[] { 1, 12, 13, 62, 63, 78, 81, 82, 83 };
			this.TopicSelects = new decimal[] { 1, 2, 12, 13, 24, 27, 54, 62, 63, 64, 65, 69, 78, 80, 81, 82, 83, 84, 86, 88, 92, 93 };
            return this;
        }

        public EHSCalcsCtl SetCalcParams(string calcsMethod, string scope, string stat, int seriesOrder)
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
                this.Seriesorder = (EHSCalcsCtl.SeriesOrder)seriesOrder;


                if (scope == "eeff")
                {
                    this.MetricScope = "ENGY";
                    this.Calculation = "eeff";
                    this.Stat = (SStat)Enum.Parse(typeof(SStat), stat, true);
                }

            }
            catch
            {
                this.Stat = SStat.sum;
                this.Seriesorder = EHSCalcsCtl.SeriesOrder.MeasureSeries;
            }

            return this;
        }

        public EHSCalcsCtl SetTargets(TargetCalcsMgr targetMgr)
        {
            this.TargetCtl = targetMgr;
            return this;
        }

        public EHSCalcsCtl InitCalc()
        {
            this.Calc.Metrics = this.MetricHst;
            this.Calc.Accounting = this.PlantHst;
            this.Calc.Incidents = this.IncidentHst;
            this.Calc.NumMeasures = this.Calc.NumPlants = this.Calc.NumPeriods = 1;  // avoid div by zero ?
            return this;
        }

        public PLANT GetPlant(decimal plantID)
        {
           // PLANT plant = SessionManager.PlantList.Where(l => l.Plant.PLANT_ID == plantID).Select(l => l.Plant).FirstOrDefault();
            PLANT plant = this.PlantList.Where(l => l.PLANT_ID == plantID).FirstOrDefault();
            if (plant == null)
            {
                plant = (from pl in this.Entities.PLANT.Include("EHS_PROFILE").Include("EHS_PROFILE.EHS_PROFILE_FACT")
                                    where pl.PLANT_ID == plantID select pl).Single();
                this.PlantList.Add(plant);
                //this.PlantList.Add(SessionManager.PlantList.Where(l => l.Plant.PLANT_ID == plantID).Select(l => l.Plant).FirstOrDefault());
            }

            return plant;
        }
        public EHS_MEASURE GetMeasure(decimal measureID)
        {
            EHS_MEASURE measure = null;
            if (measureID < 0)
            {
                measure = new EHS_MEASURE();
                measure.MEASURE_NAME = this.Calc.AttributeXref.ElementAt((int)Math.Abs(measureID)-1).Value;
            }
            else if (measureID == 0)
            {
                measure = new EHS_MEASURE();
            }
            else
            {
                measure = this.MetricHst.Where(l => l.MEASURE_ID == measureID).Select(l => l.EHS_MEASURE).FirstOrDefault();
                if (measure == null)
                    measure = EHSModel.LookupEHSMeasure(this.Entities, measureID, "");
            }
            return measure;
        }


        public decimal[] GetPlantsByScope(decimal[] plantIDS)
        {
            switch (this.CalcsMethod)
            {
                case "E":
                    //List<PLANT> plantList = SQMModelMgr.SelectPlantList(plantIDS).Where(l => l.TRACK_EW_DATA == true).ToList();
                    List<PLANT> plantList = this.PlantList.Where(l => l.TRACK_EW_DATA == true).ToList();
                    switch (this.SubScope)
                    {
                        case "":
                            plantList = plantList.Where(l => plantIDS.Contains(l.PLANT_ID)).ToList();
                            break;
                        default:
                            if (!string.IsNullOrEmpty(this.SubScope))
                            {
                                int id;
                                if (int.TryParse(this.SubScope, out id))
                                {
                                    plantList = plantList.Where(l => l.EHS_PROFILE != null && l.EHS_PROFILE.EHS_PROFILE_FACT != null
                                       && (l.EHS_PROFILE.EHS_PROFILE_FACT.Where(f=> f.CALCS_STAT.ToLower() == this.Calculation.ToLower() && f.FACTOR_ID == id).Count() > 0)
                                       ).ToList();
                                }
                            }
                            break;
                    }
                    switch (this.MetricScope)
                    {
                        case "eeff":
                        case "eSeff":
                            plantList = plantList.Where(l => l.TRACK_FIN_DATA == true).ToList();
                            break;
                        default:
                            break;
                    }
                    switch (this.Calculation)
                    {
                        case "eeff":
                        case "eSeff":
                            plantList = plantList.Where(l => l.TRACK_FIN_DATA == true).ToList();
                            break;
                        default:
                            break;
                    }
                    return plantList.Select(l => l.PLANT_ID).ToArray();
                    break;
                case "HS":
                case "IN":
                    //List<PLANT> plantList = plantList.Where(l => l.TRACK_HS_DATA == true).ToList();
                    // return plantList.Select(l => l.PLANT_ID).ToArray();
                    break;
                default:
                    break;
            }

            return plantIDS;
        }


        public decimal[] GetMetricsByScope()
        {
            decimal[] measureArray;
            int id = 0;
            int n;
            string s;

            if (this.MetricScope.Contains(',') || int.TryParse(this.MetricScope, out id))
            {
                string[] ids = this.MetricScope.Split(',');
                measureArray = ids.Select(i => decimal.Parse(i)).ToArray();
            }
            else if (this.MetricScope.Contains("EFM_"))  // get specific efm type
            {
                string[] efm = this.MetricScope.Split('_');
                measureArray = this.MetricHst.Where(m => m.EHS_MEASURE.EFM_TYPE == efm[1]).Select(m => m.MEASURE_ID).Distinct().ToArray();
            }
            else if (this.MetricScope.ToUpper().Contains("CO2")  ||  this.MetricScope.ToUpper().Contains("GHG"))
            {
                measureArray = this.MetricHst.Where(m => m.EHS_MEASURE.MEASURE_CATEGORY == "ENGY").Select(m => m.MEASURE_ID).Distinct().ToArray();
            }
            else // get all metrics of a specified catetory
            {
                switch (this.MetricScope)
                {
                    case "WASTE":
                        measureArray = this.MetricHst.Where(m => m.EHS_MEASURE.MEASURE_CATEGORY.StartsWith("EW")).Select(m => m.MEASURE_ID).Distinct().ToArray();
                        break;
                    case "WASTE_CAT":
                        this.Calc.AttributeXref = WebSiteCommon.GetXlatList("measureCategoryEHS", "", "short").Where(l => l.Key.StartsWith("EW")).ToDictionary(l => l.Key, l => l.Value);
                        measureArray = new decimal[this.Calc.AttributeXref.Count];
                        n = -1;
                        foreach (KeyValuePair<string, string> xItem in this.Calc.AttributeXref)
                        {
                            measureArray[++n] = (n + 1) * -1;
                        }
                        break;
                    case "REG_STATUS":
                        this.Calc.AttributeXref = WebSiteCommon.GetXlatList("regulatoryStatus", "", "short");
                        measureArray = new decimal[this.Calc.AttributeXref.Count];
                        n = -1;
                        foreach (KeyValuePair<string, string> xItem in this.Calc.AttributeXref)
                        {
                            measureArray[++n] = (n + 1) * -1;
                        }
                        break;
                    case "DISPOSAL":
                        this.Calc.AttributeXref = WebSiteCommon.GetXlatList("disposal", "", "short");
                        measureArray = new decimal[this.Calc.AttributeXref.Count];
                        n = -1;
                        foreach (KeyValuePair<string, string> xItem in this.Calc.AttributeXref)
                        {
                            measureArray[++n] = (n + 1) * -1;
                        }
                        break;
                    default:
                        measureArray = this.MetricHst.Where(m => m.EHS_MEASURE.MEASURE_CATEGORY == this.MetricScope).Select(m => m.MEASURE_ID).Distinct().ToArray();
                        break;
                }
                if (measureArray.Length == 0)
                    measureArray = new decimal[1] { 0 };
            }

            return measureArray;
        }

        public decimal[] GetIncidentTopics()
        {
            decimal[] topicArray;
            int id;
            string scope = this.MetricScope;

            try
            {
                if (scope.Contains(',') || int.TryParse(scope, out id))   // literal topic id's
                {
                    string[] ids = scope.Split(',');
                    topicArray = ids.Select(i => decimal.Parse(i)).ToArray();
                }
                else
                {
                    switch (scope)
                    {
                        case "type":
                        case "TYPE_ALL":
                            this.IncidentTypeList = (from t in this.Entities.INCIDENT_TYPE select t).ToList();
                            topicArray = IncidentTypeList.Select(l => l.INCIDENT_TYPE_ID).ToArray();
                            break;
                        case "TYPE_INCIDENT":
                            this.IncidentTypeList = (from t in this.Entities.INCIDENT_TYPE
                                                     where t.INCIDENT_TYPE_ID != 10 && t.INCIDENT_TYPE_ID != 5 && t.INCIDENT_TYPE_ID != 11
                                                     select t).ToList();
                            topicArray = IncidentTypeList.Select(l => l.INCIDENT_TYPE_ID).ToArray();
                            break;
                        case "TYPE_AUDIT":
                            this.IncidentTypeList = (from t in this.Entities.INCIDENT_TYPE
                                                     where t.INCIDENT_TYPE_ID == 5 || t.INCIDENT_TYPE_ID == 11
                                                     select t).ToList();
                            topicArray = IncidentTypeList.Select(l => l.INCIDENT_TYPE_ID).ToArray();
                            break;
                        default:
                            List<decimal> tempList = new List<decimal>();
                            foreach (EHSIncidentData data in this.IncidentHst)
                            {
                                if (!string.IsNullOrEmpty(scope))   // get topics of a standard type
                                    tempList.AddRange(data.EntryList.Where(l => l.INCIDENT_QUESTION.STANDARD_TYPE == scope).Select(l => l.INCIDENT_QUESTION.INCIDENT_QUESTION_ID).ToList());
                                else
                                    tempList.AddRange(data.EntryList.Select(l => l.INCIDENT_QUESTION.INCIDENT_QUESTION_ID).ToList());
                            }
                            topicArray = tempList.Distinct().ToArray();
                            break;
                    }
                }
            }
            catch
            {
                topicArray = new decimal[0] { };
            }

            return topicArray;
        }

        public EHSCalcsCtl LoadMetricHistory(decimal[] plantIDS, DateTime startDate, DateTime endDate, DateIntervalType dateIntervalType, bool loadPlantAccounting)
        {
            this.DateInterval = dateIntervalType;
            this.MetricHst = new List<EHS_METRIC_HISTORY>();
            this.PlantHst = new List<PLANT_ACCOUNTING>();
            DateTime fromDate = WebSiteCommon.PeriodFromDate(startDate);
            DateTime toDate = WebSiteCommon.PeriodToDate(endDate);

            try
            {
                this.PlantList = (from pl in this.Entities.PLANT.Include("EHS_PROFILE").Include("EHS_PROFILE.EHS_PROFILE_FACT")
                         where (plantIDS.Contains((decimal)pl.PLANT_ID))
                         select pl).ToList();

                this.MetricHst = (from h in this.Entities.EHS_METRIC_HISTORY.Include("EHS_MEASURE") 
                                  where (plantIDS.Contains((decimal)h.PLANT_ID)
                                          && EntityFunctions.CreateDateTime(h.PERIOD_YEAR,h.PERIOD_MONTH,1,0,0,0) >= fromDate &&  EntityFunctions.CreateDateTime(h.PERIOD_YEAR,h.PERIOD_MONTH,1,0,0,0) <= toDate) 
                                  select h).OrderBy(h=> h.PERIOD_YEAR).ThenBy(h=> h.PERIOD_MONTH).ToList();

                if (loadPlantAccounting)
                {
                    this.LoadPlantAccounting(plantIDS, startDate, endDate);
                   
                    string[] paNames = new string[8];
                    try
                    {
                        paNames[0] = MetricHst.Where(h => h.EHS_MEASURE.PLANT_ACCT_FIELD == "OPER_COST").Select(h => h.EHS_MEASURE.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[1] = MetricHst.Where(h => h.EHS_MEASURE.PLANT_ACCT_FIELD == "REVENUE").Select(h => h.EHS_MEASURE.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[4] = MetricHst.Where(h => h.EHS_MEASURE.PLANT_ACCT_FIELD == "TIME_WORKED").Select(h => h.EHS_MEASURE.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[5] = MetricHst.Where(h => h.EHS_MEASURE.PLANT_ACCT_FIELD == "TIME_LOST").Select(h => h.EHS_MEASURE.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                    }
                    catch { }

                    foreach (PLANT_ACCOUNTING pac in this.PlantHst)
                    {
                        EHS_METRIC_HISTORY[] mstArray = new EHS_METRIC_HISTORY[10];
                        try
                        {
                            for (int n = 0; n < 10; n++)
                            {
                                mstArray[n] = new EHS_METRIC_HISTORY();
                                mstArray[n].EHS_MEASURE = new EHS_MEASURE();
                                mstArray[n].MEASURE_ID = mstArray[n].EHS_MEASURE.MEASURE_ID = 1000000m + (decimal)n;
                                mstArray[n].EHS_MEASURE.MEASURE_CATEGORY = "PROD";
                                mstArray[n].PLANT = pac.PLANT;
                                mstArray[n].PLANT_ID = pac.PLANT_ID;
                                mstArray[n].PERIOD_YEAR = pac.PERIOD_YEAR;
                                mstArray[n].PERIOD_MONTH = pac.PERIOD_MONTH;
                                mstArray[n].MEASURE_COST = 0;
                                if (!string.IsNullOrEmpty(paNames[4]))
                                    mstArray[n].EHS_MEASURE.MEASURE_NAME = paNames[4]; 
                                switch (n)
                                {
                                    case 0: mstArray[n].MEASURE_VALUE = pac.OPER_COST.HasValue ? (decimal)pac.OPER_COST : 0; break;
                                    case 1: mstArray[n].MEASURE_VALUE = pac.REVENUE.HasValue ? (decimal)pac.REVENUE : 0; break;
                                    case 2: mstArray[n].MEASURE_VALUE = pac.PRODUCTION.HasValue ? (decimal)pac.PRODUCTION : 0; break;
                                    case 3: mstArray[n].MEASURE_VALUE = pac.THROUGHPUT.HasValue ? (decimal)pac.THROUGHPUT : 0; break;
                                    case 4: mstArray[n].MEASURE_VALUE = pac.TIME_WORKED.HasValue ? (decimal)pac.TIME_WORKED : 0; break;
                                    case 5: mstArray[n].MEASURE_VALUE = pac.TIME_LOST.HasValue ? (decimal)pac.TIME_LOST : 0; break;
                                    case 6: mstArray[n].MEASURE_VALUE = pac.TIME_LOST_CASES.HasValue ? (decimal)pac.TIME_LOST_CASES : 0; break;
                                    case 7: mstArray[n].MEASURE_VALUE = pac.RECORDED_CASES.HasValue ? (decimal)pac.RECORDED_CASES : 0; break;
                                    case 8: mstArray[n].LAST_UPD_DT = pac.APPROVAL_DT.HasValue ? pac.APPROVAL_DT : null; break;
                                    case 9: mstArray[n].LAST_UPD_BY = pac.LAST_UPD_BY;  break;
                                }
                                this.MetricHst.Add(mstArray[n]);
                            }
                        }
                        catch (Exception ee)
                        {
                            ;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                ; //   SQMLogger.LogException(ex);
            }

            return this;
        }

        public EHSCalcsCtl LoadPlantAccounting(decimal[] plantIDS, DateTime startDate, DateTime endDate)
        {
            this.PlantHst = new List<PLANT_ACCOUNTING>();
            DateTime fromDate = WebSiteCommon.PeriodFromDate(startDate);
            DateTime toDate = WebSiteCommon.PeriodToDate(endDate);

            this.PlantHst = (from h in this.Entities.PLANT_ACCOUNTING.Include("PLANT")
                             where (plantIDS.Contains((decimal)h.PLANT_ID)
                             && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) <= toDate)
                             select h).OrderBy(h => h.PERIOD_YEAR).ThenBy(h => h.PERIOD_MONTH).ToList();
            return this;

        }

        public EHSCalcsCtl LoadMetricInputs(DateTime fromDate, DateTime toDate, decimal[] plantIDS)
        {
            return LoadMetricInputs(fromDate, toDate, plantIDS, "I");
        }

        public EHSCalcsCtl LoadMetricInputs(DateTime startDate, DateTime endDate, decimal[] plantIDS, string option)
        {
            DateTime fromDate = WebSiteCommon.PeriodFromDate(startDate);
            DateTime toDate = WebSiteCommon.PeriodToDate(endDate);

            try
            {
                if (option == "IR")  // select required inputs only
                    this.InputsList = (from i in this.Entities.EHS_PROFILE_INPUT.Include("EHS_PROFILE_MEASURE").Include("EHS_PROFILE_MEASURE.EHS_MEASURE")
                                       where (i.EHS_PROFILE_MEASURE.IS_REQUIRED == true  &&   plantIDS.Contains(i.EHS_PROFILE_MEASURE.PLANT_ID) && EntityFunctions.CreateDateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1, 0, 0, 0) <= toDate)
                                       select i).OrderBy(h => h.EHS_PROFILE_MEASURE.PLANT_ID).ThenBy(h => h.PERIOD_YEAR).ThenBy(h => h.PERIOD_MONTH).ThenBy(l => l.PRMR_ID).ToList();
                else 
                    this.InputsList = (from i in this.Entities.EHS_PROFILE_INPUT.Include("EHS_PROFILE_MEASURE").Include("EHS_PROFILE_MEASURE.EHS_MEASURE") 
                                       where (plantIDS.Contains(i.EHS_PROFILE_MEASURE.PLANT_ID)  &&  EntityFunctions.CreateDateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1, 0, 0, 0) <= toDate)
                                       select i).OrderBy(h => h.EHS_PROFILE_MEASURE.PLANT_ID).ThenBy(h => h.PERIOD_YEAR).ThenBy(h => h.PERIOD_MONTH).ThenBy(l => l.PRMR_ID).ToList();

                LoadPlantAccounting(plantIDS, startDate, endDate);

                foreach (PLANT_ACCOUNTING pac in this.PlantHst)
                {
                    EHS_PROFILE_INPUT[] mstArray = new EHS_PROFILE_INPUT[8];
                    try
                    {
                        for (int n = 0; n < 8; n++)
                        {
                            mstArray[n] = new EHS_PROFILE_INPUT();
                            mstArray[n].EHS_PROFILE_MEASURE = new EHS_PROFILE_MEASURE();
                            mstArray[n].EHS_PROFILE_MEASURE.PLANT_ID = pac.PLANT_ID;
                            mstArray[n].EHS_PROFILE_MEASURE.EHS_MEASURE = new EHS_MEASURE();
                            mstArray[n].EHS_PROFILE_MEASURE.EHS_MEASURE.MEASURE_CATEGORY = "PROD";
                            mstArray[n].PRMR_ID = 1000000m + (decimal)n;
                            mstArray[n].PERIOD_YEAR = pac.PERIOD_YEAR;
                            mstArray[n].PERIOD_MONTH = pac.PERIOD_MONTH;
                            mstArray[n].MEASURE_COST = 0;
                            switch (n)
                            {
                                case 0: mstArray[n].MEASURE_VALUE = pac.OPER_COST.HasValue ? (decimal)pac.OPER_COST : 0; break;
                                case 1: mstArray[n].MEASURE_VALUE = pac.REVENUE.HasValue ? (decimal)pac.REVENUE : 0; break;
                                case 2: mstArray[n].MEASURE_VALUE = pac.PRODUCTION.HasValue ? (decimal)pac.PRODUCTION : 0; break;
                                case 3: mstArray[n].MEASURE_VALUE = pac.THROUGHPUT.HasValue ? (decimal)pac.THROUGHPUT : 0; break;
                                case 4: mstArray[n].MEASURE_VALUE = pac.TIME_WORKED.HasValue ? (decimal)pac.TIME_WORKED : 0; break;
                                case 5: mstArray[n].MEASURE_VALUE = pac.TIME_LOST.HasValue ? (decimal)pac.TIME_LOST : 0; break;
                                case 6: mstArray[n].MEASURE_VALUE = pac.TIME_LOST_CASES.HasValue ? (decimal)pac.TIME_LOST_CASES : 0; break;
                                case 7: mstArray[n].MEASURE_VALUE = pac.RECORDED_CASES.HasValue ? (decimal)pac.RECORDED_CASES : 0; break;
                            }
                            this.InputsList.Add(mstArray[n]);
                        }
                    }
                    catch
                    {
                        ;
                    }
                }

                this.InputsList = this.InputsList.OrderBy(l => l.EHS_PROFILE_MEASURE.PLANT_ID).ThenBy(l => l.PERIOD_MONTH).ThenBy(l => l.PERIOD_MONTH).ThenBy(l => l.PRMR_ID).ToList();
            }
            catch
            {
                ;
            }

            return this;
        }

        public EHSCalcsCtl LoadIncidentHistory(decimal[] plantIDS, DateTime fromDate, DateTime toDate, DateIntervalType dateIntervalType)
        {
            this.DateInterval = dateIntervalType;
            this.IncidentHst = new List<EHSIncidentData>();

            try
            {
                this.IncidentHst = (from i in this.Entities.INCIDENT
                                    where (i.INCIDENT_TYPE == "EHS" && i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate && plantIDS.Contains((decimal)i.DETECT_PLANT_ID))
                                    join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID 
                                   select new EHSIncidentData 
                                   {
                                       Incident = i,
                                       Plant = p 
                                   }).OrderBy(l=> l.Incident.INCIDENT_ID).ToList();

                if (this.IncidentHst != null)
                {
                    decimal[] ids = this.IncidentHst.Select(i => i.Incident.INCIDENT_ID).Distinct().ToArray();
                    var qaList = (from a in this.Entities.INCIDENT_ANSWER.Include("INCIDENT_QUESTION")
                                  where (ids.Contains(a.INCIDENT_ID) && this.TopicSelects.Contains(a.INCIDENT_QUESTION_ID))
                                  select a).ToList();
                    foreach (EHSIncidentData data in this.IncidentHst)
                    {
                        data.EntryList = new List<INCIDENT_ANSWER>();
                        data.EntryList.AddRange(qaList.Where(l => l.INCIDENT_ID == data.Incident.INCIDENT_ID).ToList());
                        data.DeriveStatus();
                        data.DaysElapsed();
                    }
                }
            }
            catch (Exception ex)
            {
               // SQMLogger.LogException(ex);
            }

            return this;
        }

        public List<EHSIncidentData> SelectIncidentList(List<decimal> plantIdList, List<decimal> incidentTypeList, DateTime fromDate, DateTime toDate, string incidentStatus, bool selectAttachments)
        {
            try
            {
                this.IncidentHst = (from i in this.Entities.INCIDENT 
                                    join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
                                    join r in this.Entities.PERSON on i.CREATE_PERSON equals r.PERSON_ID
                                    where ((i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate)
                                    && incidentTypeList.Contains((decimal)i.ISSUE_TYPE_ID) && plantIdList.Contains((decimal)i.DETECT_PLANT_ID))
                                    select new EHSIncidentData
                                    {
                                        Incident = i,
                                        Plant = p,
                                        Person = r
                                    }).ToList();

                if (this.IncidentHst != null)
                {
                    decimal[] ids = this.IncidentHst.Select(i => i.Incident.INCIDENT_ID).Distinct().ToArray();
                    var qaList = (from a in this.Entities.INCIDENT_ANSWER.Include("INCIDENT_QUESTION")
                                  where (ids.Contains(a.INCIDENT_ID) && this.TopicSelects.Contains(a.INCIDENT_QUESTION_ID))
                                  select a).ToList();
                    foreach (EHSIncidentData data in this.IncidentHst)
                    {
                        data.EntryList = new List<INCIDENT_ANSWER>();
                        data.EntryList.AddRange(qaList.Where(l=> l.INCIDENT_ID == data.Incident.INCIDENT_ID).ToList());
                        data.DeriveStatus();
                        data.DaysElapsed();
                    }

                    if (incidentStatus == "A")  // get open incidents
                        this.IncidentHst = this.IncidentHst.Where(l => l.Status == "A").ToList();
                    if (incidentStatus == "N")  // data incomplete
                        this.IncidentHst = this.IncidentHst.Where(l => l.Status == "N").ToList();
                    else if (incidentStatus == "C")  // get closed incidents
                        this.IncidentHst = this.IncidentHst.Where(l => l.Status == "C"  ||  l.Status == "C8").ToList();
                    else if (incidentStatus == "C8") // get closed 8d
                        this.IncidentHst = this.IncidentHst.Where(l => l.Status == "C8").ToList();

                    if (selectAttachments)
                    {
                        List<ATTACHMENT> attachList = (from a in this.Entities.ATTACHMENT
                                                       where
                                                       (a.RECORD_TYPE == 40 && ids.Contains(a.RECORD_ID)
                                                       && a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
                                                       a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
                                                       a.FILE_NAME.ToLower().Contains(".bmp"))
                                                       select a).OrderBy(l => l.ATTACHMENT_ID).ToList();
                        foreach (EHSIncidentData data in this.IncidentHst)
                        {
                            data.AttachList = new List<ATTACHMENT>();
                            data.AttachList.AddRange(attachList.Where(l => l.RECORD_ID == data.Incident.INCIDENT_ID).ToList());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return this.IncidentHst;
        }

		public List<EHSIncidentData> SelectPreventativeList(List<decimal> plantIdList, List<decimal> incidentTypeList, List<string>inspectionCatetoryList, List<string> recommendTypeList, DateTime fromDate, DateTime toDate, List<string> statusList, bool selectAttachments)
		{
			try
			{
				this.IncidentHst = (from i in this.Entities.INCIDENT
									join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
									join r in this.Entities.PERSON on i.CREATE_PERSON equals r.PERSON_ID
									where ((i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate)
									&& incidentTypeList.Contains((decimal)i.ISSUE_TYPE_ID) && plantIdList.Contains((decimal)i.DETECT_PLANT_ID))
									select new EHSIncidentData
									{
										Incident = i,
										Plant = p,
										Person = r
									}).ToList();

				if (this.IncidentHst != null)
				{
					decimal[] ids = this.IncidentHst.Select(i => i.Incident.INCIDENT_ID).Distinct().ToArray();
					var qaList = (from a in this.Entities.INCIDENT_ANSWER.Include("INCIDENT_QUESTION")
								  where (ids.Contains(a.INCIDENT_ID) && this.TopicSelects.Contains(a.INCIDENT_QUESTION_ID))
								  select a).ToList();
                    var respIDS = qaList.Where(l => l.INCIDENT_QUESTION_ID == 84 && !string.IsNullOrEmpty(l.ANSWER_VALUE)).Select(l => Convert.ToDecimal(l.ANSWER_VALUE)).Distinct().ToList();
                    List<PERSON> personList = (from p in this.Entities.PERSON where respIDS.Contains(p.PERSON_ID) select p).ToList();
                    INCIDENT_ANSWER entry = null;
                    foreach (EHSIncidentData data in this.IncidentHst)
					{
						data.EntryList = new List<INCIDENT_ANSWER>();
						data.EntryList.AddRange(qaList.Where(l => l.INCIDENT_ID == data.Incident.INCIDENT_ID).ToList());
                        if ((entry = data.EntryList.Where(l => l.INCIDENT_QUESTION_ID == 84).FirstOrDefault()) != null)
                        {
                            data.RespPerson = personList.Where(l => l.PERSON_ID.ToString() == entry.ANSWER_VALUE).FirstOrDefault();
                        }
                        data.DeriveStatus();
						data.DaysElapsed();
					}

                    if (statusList.Count > 0)
                        this.IncidentHst = this.IncidentHst.Where(l => statusList.Contains(l.Status)).ToList();

                    if (inspectionCatetoryList != null && inspectionCatetoryList.Count > 0)    // get specific inspection catetories
                    {
                        this.IncidentHst = this.IncidentHst.Where(l => l.EntryList.Where(e => e.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.InspectionCategory && inspectionCatetoryList.Contains(e.ANSWER_VALUE)).Count() > 0).ToList();
                    }

                    if (recommendTypeList != null  && recommendTypeList.Count > 0)    // get specific recommendaton types
                    {
                         this.IncidentHst = this.IncidentHst.Where(l => l.EntryList.Where(e => e.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.RecommendationType && recommendTypeList.Contains(e.ANSWER_VALUE)).Count() > 0).ToList();
                    }

                    if (selectAttachments)
                    {
                        List<ATTACHMENT> attachList = (from a in this.Entities.ATTACHMENT
                                                    where
                                                    (a.RECORD_TYPE == 40 && ids.Contains(a.RECORD_ID)
                                                    && a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
                                                    a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
                                                    a.FILE_NAME.ToLower().Contains(".bmp"))
                                                    select a).OrderBy(l => l.ATTACHMENT_ID).ToList();
                        foreach (EHSIncidentData data in this.IncidentHst)
                        {
                            data.AttachList = new List<ATTACHMENT>();
                            data.AttachList.AddRange(attachList.Where(l => l.RECORD_ID == data.Incident.INCIDENT_ID).ToList());
                        }
                    }
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return this.IncidentHst;
		}

        public List<GaugeSeries> CalculateIncidentSeries(string option, SStat stat)
        {
            this.Results.Initialize();
            GaugeSeries series = new GaugeSeries().CreateNew(0, "", "");

            switch (option)
            {
                case "type":
                    List<string> keyList = new List<string>();
                    keyList = this.IncidentHst.Select(l => l.Incident.ISSUE_TYPE).Distinct().ToList();
                    decimal totalCount = (decimal)this.IncidentHst.Count();
                    int nItem = -1;
                    foreach (string key in keyList)
                    {
                        decimal keyCount = (decimal)this.IncidentHst.Where(l => l.Incident.ISSUE_TYPE == key).Count();
                        if (stat == SStat.pct)
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nItem, 0, (keyCount / totalCount) * 100m, key));
                        else
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nItem, 0, keyCount, key));
                    }
                    break;
                case "age":
                    GaugeSeriesItem item;
                    DateTime now = DateTime.Now;

                    item = new GaugeSeriesItem();
                    item.Item = 1;
                    item.Text = "1 - 7 Days";
                    item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) <= 7).Count();
                    series.ItemList.Add(item);

                    item = new GaugeSeriesItem();
                    item.Item = 2;
                    item.Text = "8 - 30 Days";
                    item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) > 7 && Math.Max(l.DaysOpen, l.DaysToClose) <= 30).Count();
                    series.ItemList.Add(item);

                    item = new GaugeSeriesItem();
                    item.Item = 3;
                    item.Text = "31 - 90 Days";
                    item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) > 30 && Math.Max(l.DaysOpen, l.DaysToClose) <= 90).Count();
                    series.ItemList.Add(item);

                    item = new GaugeSeriesItem();
                    item.Item = 3;
                    item.Text = "90+ Days";
                    item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) > 90).Count();
                    series.ItemList.Add(item);
                    break;
                case "count":
                default:
                    List<string> locList = new List<string>();
                    locList = this.IncidentHst.Select(l => l.Plant.PLANT_NAME).Distinct().ToList();
                    int nLoc = -1;
                    foreach (string key in locList)
                    {
                        decimal count = (decimal)this.IncidentHst.Where(l => l.Plant.PLANT_NAME == key).Count();
                        series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nLoc, 0, count, key));
                    }
                    break;
            }

            this.Results.metricSeries.Add(series);

            return this.Results.metricSeries;
        }
  

        public List<EHSIncidentData> LookupIncidentTopicOccurs(decimal[] plantIDS,  decimal[] incidentTypeIDS, decimal topicID, string topicValue)
        {
            // get absolute incident occurs regardless of date
            this.IncidentHst = new List<EHSIncidentData>();
  
            try
            {
                this.IncidentHst = (from i in this.Entities.INCIDENT
                                    join r in this.Entities.INCIDENT_ANSWER on i.INCIDENT_ID equals r.INCIDENT_ID
                                    join q in this.Entities.INCIDENT_QUESTION on r.INCIDENT_QUESTION_ID equals q.INCIDENT_QUESTION_ID
                                    where (i.INCIDENT_TYPE == "EHS" && incidentTypeIDS.Contains((decimal)i.ISSUE_TYPE_ID) && plantIDS.Contains((decimal)i.DETECT_PLANT_ID) 
                                        &&  r.INCIDENT_QUESTION_ID == topicID  &&  r.ANSWER_VALUE.ToUpper() == topicValue)
                                    select new EHSIncidentData
                                    {
                                        Incident = i
                                    }).OrderBy(l => l.Incident.INCIDENT_DT).ToList();
            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
            }

            return this.IncidentHst;
        }


        public int InputsSeries(EHSCalcsCtl.SeriesOrder seriesOrder, decimal[]plantIDS, decimal[] measureIDS, DateTime fromDate, DateTime toDate)
        {
            int status = 0;
            decimal temp = 0;
            int ns = 0;
            string seriesName = "";
            GaugeSeries series = null;
            this.Results.Initialize();
            List<WebSiteCommon.DatePeriod> pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType,  "");
           
            switch (seriesOrder)
            {
                case EHSCalcsCtl.SeriesOrder.PeriodMeasure:
                    foreach (decimal measureID in measureIDS)
                    {
                        seriesName = "";
                        EHS_MEASURE measure = this.InputsList.Where(l => l.PRMR_ID == measureIDS[0]).Select(l => l.EHS_PROFILE_MEASURE.EHS_MEASURE).FirstOrDefault();
                        if (measure != null  &&  measure.MEASURE_NAME != null)
                            seriesName = measure.MEASURE_NAME.Trim();
                        series = new GaugeSeries().CreateNew(1, seriesName, "");
                        foreach (WebSiteCommon.DatePeriod pd in pdList)
                        {
                            switch (this.Stat)
                            {
                                case SStat.cost:
                                    temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID)  &&  measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Select(l => l.MEASURE_COST).Sum();
                                    break;
                                case SStat.value:
                                case SStat.sum:
                                    temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID)  &&  measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Select(l => l.MEASURE_VALUE).Sum();
                                    break;
                                case SStat.count:
                                    temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID)  &&  measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Distinct().Count();
                                    break;
                                default:
                                    break;
                            }
  
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, 0, temp, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false)));
                        }
                        this.Results.metricSeries.Add(series);
                    }
                    break;
                default:
                    series = new GaugeSeries().CreateNew(1, seriesName, "");
                    foreach (WebSiteCommon.DatePeriod pd in pdList)
                    {
                        switch (this.Stat)
                        {
                            case SStat.cost:
                                temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID) && measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Select(l => l.MEASURE_COST).Sum();
                                break;
                            case SStat.value:
                            case SStat.sum:
                                temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID) && measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Select(l => l.MEASURE_VALUE).Sum();
                                break;
                            case SStat.count:
                                temp = (decimal)this.InputsList.Where(l => plantIDS.Contains(l.EHS_PROFILE_MEASURE.PLANT_ID) && measureIDS.Contains(l.PRMR_ID) && l.PERIOD_YEAR == pd.FromDate.Year && l.PERIOD_MONTH == pd.FromDate.Month).Distinct().Count();
                                break;
                            default:
                                break;
                        }

                        series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, 0, temp, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false)));
                    }
                    this.Results.metricSeries.Add(series);
                    break;
            }

            if (this.Results.metricSeries.Count > 0)
                this.Results.ValidResult = true;

            return status;
        }

        public int MetricSeries(EHSCalcsCtl.SeriesOrder seriesOrder, DateTime fromDate, DateTime toDate, decimal[] plantIDS, decimal[] measureIDS, decimal multFactor)
        {
            int status = 0;
            int item = 0;
            int ns = 0;
            decimal result0 = 0;
            GaugeSeries series = null;
            this.Results.Initialize();
            PLANT plant = null;
            List<WebSiteCommon.DatePeriod> pdList;

            try
            {
                switch (seriesOrder)
                {
                    case EHSCalcsCtl.SeriesOrder.SumTotal:
                        series = new GaugeSeries().CreateNew(1, "", "");
                        if (EHMetric(plantIDS, measureIDS, fromDate, toDate))
                        {
                            series.AddItem(item++, "Total", this.Results);
                        }
                        this.Results.metricSeries.Add(series);
                        break;

                    case EHSCalcsCtl.SeriesOrder.MeasureSeries:   // formerly SumAll (0)
                        series = new GaugeSeries().CreateNew(1, "", "");
                        foreach (decimal measureID in measureIDS)
                        {
                            EHS_MEASURE measure = GetMeasure(measureID);
                            if (EHMetric(plantIDS, new decimal[1] { measureID }, fromDate, toDate))
                            {
                                series.AddItem(item++, measure.MEASURE_NAME.Trim(), this.Results);
                            }
                        }
                        this.Results.metricSeries.Add(series);
                        break;

                    case EHSCalcsCtl.SeriesOrder.PeriodMeasure:
                    case EHSCalcsCtl.SeriesOrder.PeriodMeasureYOY:
                        pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType,  "");
                        foreach (decimal measureID in measureIDS)
                        {
                            EHS_MEASURE measure = GetMeasure(measureID);
                            series = new GaugeSeries().CreateNew(1, measure.MEASURE_NAME.Trim(), "");
                            ns = 0;
                            foreach (WebSiteCommon.DatePeriod pd in pdList)
                            {
                                if (EHMetric(plantIDS, new decimal[1] { measureID }, pd.FromDate, pd.ToDate))
                                {
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
                                }
                            }
                            this.Results.metricSeries.Add(series);
                        }
                        if (seriesOrder == SeriesOrder.PeriodMeasureYOY)
                        {
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate.AddYears(-1), DateIntervalType.month, this.DateSpanType, "");
                            foreach (decimal measureID in measureIDS)
                            {
                                EHS_MEASURE measure = GetMeasure(measureID);
                                series = new GaugeSeries().CreateNew(1, measure.MEASURE_NAME.Trim(), "");
                                ns = 0;
                                foreach (WebSiteCommon.DatePeriod pd in pdList)
                                {
                                    if (EHMetric(plantIDS, new decimal[1] { measureID }, pd.FromDate, pd.ToDate))
                                    {
                                        series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
                                    }
                                }
                                this.Results.metricSeries.Add(series);
                            }
                        }
                        break;

                    case EHSCalcsCtl.SeriesOrder.PeriodSum:
                    case EHSCalcsCtl.SeriesOrder.PeriodSumYOY:
                    case EHSCalcsCtl.SeriesOrder.YearSum:
                        if (seriesOrder == SeriesOrder.YearSum)
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.year, this.DateSpanType, "");
                        else 
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "");
                        series = new GaugeSeries().CreateNew(1, "1", "");
                        ns = 0;
                        foreach (WebSiteCommon.DatePeriod pd in pdList)
                        {
                            if (EHMetric(plantIDS, measureIDS, pd.FromDate, pd.ToDate))
                            {
                                if (seriesOrder == SeriesOrder.YearSum)
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy", false), this.Results));
                                else 
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
                            }
                        }
                        this.Results.metricSeries.Add(series);

                        if (seriesOrder == SeriesOrder.PeriodSumYOY)
                        {
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate.AddYears(-1), DateIntervalType.month, this.DateSpanType, "");
                            ns = 0;
                            series = new GaugeSeries().CreateNew(2, "2", "");
                            foreach (WebSiteCommon.DatePeriod pd in pdList)
                            {
                                if (EHMetric(plantIDS, measureIDS, pd.FromDate, pd.ToDate))
                                {
                                    series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
                                }
                            }
                            this.Results.metricSeries.Add(series);
                        }
                        break;

                    case EHSCalcsCtl.SeriesOrder.PeriodMeasurePlant:
                    case EHSCalcsCtl.SeriesOrder.YearMeasurePlant:
                        foreach (decimal plantID in plantIDS)
                        {
                            plant = GetPlant(plantID);
                            series = new GaugeSeries().CreateNew(1, plant.PLANT_NAME, "");
                            if (seriesOrder == SeriesOrder.YearMeasurePlant)
                            {
                                ns = 0;
                                pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.year, this.DateSpanType, "");
                                foreach (WebSiteCommon.DatePeriod pd in pdList)
                                {
                                    if (EHMetric(new decimal[1] { plantID }, measureIDS, pd.FromDate, pd.ToDate))
                                    {
                                        series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, pd.Label, this.Results));
                                    }
                                }
                            }
                            else
                            {
                                ns = 0;
                                pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "");
                                foreach (WebSiteCommon.DatePeriod pd in pdList)
                                {
                                    if (EHMetric(new decimal[1] { plantID }, measureIDS, pd.FromDate, pd.ToDate))
                                    {
                                        series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
                                    }
                                }
                            }
                            this.Results.metricSeries.Add(series);
                        }
                        break;
                    case EHSCalcsCtl.SeriesOrder.MeasurePlant:
                        foreach (decimal measureID in measureIDS)
                        {
                            EHS_MEASURE measure = GetMeasure(measureID);
                            series = new GaugeSeries().CreateNew(1, measure.MEASURE_NAME.Trim(), "");
                            foreach (decimal plantID in plantIDS)
                            {
                                plant = GetPlant(plantID);
                                if (EHMetric(new decimal[1] { plantID }, new decimal[1] { measureID }, fromDate, toDate))
                                {
                                    series.AddItem(item++, plant.PLANT_NAME, this.Results);
                                }
                            }
                            this.Results.metricSeries.Add(series);
                        }
                        break;

                    case EHSCalcsCtl.SeriesOrder.YearPlant:
                    case EHSCalcsCtl.SeriesOrder.YearPlantAvg:
                        result0 = 0;
                        ns = 0;
                        if (this.DateSpanType == DateSpanOption.FYYearOverYear)  // don't deduct start year because it was already done upstream
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.FYyear, this.DateSpanType, "");
                        else
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate, DateIntervalType.FYyear, this.DateSpanType, "");

                        foreach (WebSiteCommon.DatePeriod pd in pdList)
                        {
                            series = new GaugeSeries().CreateNew(1, pd.Label, "");
                            result0 = 0;
                            foreach (decimal plantID in plantIDS)
                            {
                                plant = GetPlant(plantID);
                                EHMetric(new decimal[1] { plantID }, measureIDS, pd.FromDate, pd.ToDate);
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, plant.PLANT_NAME, this.Results));
                                result0 += this.Results.Result;
                            }

                            EHMetric(plantIDS, measureIDS, pd.FromDate, pd.ToDate);
                            series.ItemList.Insert(0, new GaugeSeriesItem().CreateNew(1, 0, "Total", this.Results));

                            /*
                            if (seriesOrder == SeriesOrder.YearPlantAvg)
                                series.ItemList.Insert(0, new GaugeSeriesItem().CreateNew(1, 0, 0, result0/Math.Max(1,plantIDS.Length), "Average"));
                            */
                            this.Results.metricSeries.Add(series);
                        }
                        break;

                    case EHSCalcsCtl.SeriesOrder.PlantMeasureTotal:
                    case EHSCalcsCtl.SeriesOrder.PlantMeasureAvg:
                        series = new GaugeSeries().CreateNew(1, this.Stat.ToString(), "");
                        foreach (decimal plantID in plantIDS)
                        {
                            plant = GetPlant(plantID);
                            if (EHMetric(new decimal[1] { plantID }, measureIDS, fromDate, toDate))
                            {
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, item++, plant.PLANT_NAME, this.Results));
                            }
                        }

                        if (EHMetric(plantIDS, measureIDS, fromDate, toDate))
                        {
                            if (seriesOrder == EHSCalcsCtl.SeriesOrder.PlantMeasureAvg)
                            {
                                this.Results.Result = this.Results.Result / Math.Max(1, plantIDS.Length);
                                series.ItemList.Insert(0, new GaugeSeriesItem().CreateNew(1, 0, "Avg", this.Results));
                            }
                            else
                                series.ItemList.Insert(0, new GaugeSeriesItem().CreateNew(1, 0, "Total", this.Results));
                        }

                        this.Results.metricSeries.Add(series);
                        break;

                    case EHSCalcsCtl.SeriesOrder.PlantMeasure:
                    default:
                        series = new GaugeSeries().CreateNew(1, this.Stat.ToString(), "");
                        foreach (decimal plantID in plantIDS)
                        {
                            plant = GetPlant(plantID);
                            if (EHMetric(new decimal[1] { plantID }, measureIDS, fromDate, toDate))
                            {
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, item++, plant.PLANT_NAME, this.Results));
                            }
                        }
                        this.Results.metricSeries.Add(series);
                        break;
                }

                if (this.Results.metricSeries.Count > 0)
                    this.Results.ValidResult = true;

                if (this.Stat == SStat.pct)  // transform series from values to percentages
                    this.Results.TransformSeries(this.Stat, seriesOrder);
            }
            catch (Exception ex)
            {
                this.Results.ValidResult = false;
            }

            return status;
        }

        public bool EHMetric(decimal[] plantArray, decimal[] measureArray, DateTime fromDate, DateTime toDate)
        {
            this.Results.Result2 = this.Results.Result2 = 0;
            this.Results.ValidResult = this.Results.ValidResult2 = false;
            decimal value = 0;

            try
            {
                switch (this.Stat)
                {
                    case SStat.pctChange:
                    case SStat.pctReduce:
                        decimal currentFactor, priorFactor;
                        if (this.Results.ValidResult = MetricFactor(plantArray, measureArray, fromDate, toDate, out currentFactor))
                        {
                            if (this.Results.ValidResult = MetricFactor(plantArray, measureArray, fromDate.AddYears(-1), toDate.AddYears(-1),out priorFactor))
                            {
                                if (priorFactor == 0 && currentFactor == 0)
                                    this.Results.Result = 0;
                                else if (priorFactor == 0)
                                    this.Results.Result = 100;
                                else if (currentFactor == 0)
                                    this.Results.Result = -100;
                                else
                                {
                                    if (this.Stat == SStat.pctReduce)
                                        this.Results.Result = ((currentFactor - priorFactor) / priorFactor) * (-100);
                                    else 
                                        this.Results.Result = ((currentFactor -priorFactor) / priorFactor) * 100;
                                }
                            }
                        }
                        break;
                    case SStat.sumCost:
                        if (this.Results.ValidResult = MetricFactor(plantArray, measureArray, fromDate, toDate, out value))
                            this.Results.Result = value;
                        this.MetricScope = SStat.cost.ToString();  // ????????????????
                        if (this.Results.ValidResult2 = MetricFactor(plantArray, measureArray, fromDate, toDate, out value))
                            this.Results.Result2 = value;
                        break;
                    default:
                        if (this.Results.ValidResult = MetricFactor(plantArray, measureArray, fromDate, toDate, out value))
                            this.Results.Result = value;
                        break;
                }
            }
            catch
            {
                ;
            }

            return this.Results.ValidResult;
        }

        public bool MetricFactor(decimal[] plantArray, decimal[] measureArray, DateTime fromDate, DateTime toDate, out decimal value)
        {
            bool valid = true;
            decimal temp, temp2, temp3;
            value = 0;

            try
            {
                if (!string.IsNullOrEmpty(this.Calculation))
                {
                    switch (this.Calculation)
                    {
                        case "cost":
                            value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                            break;
                        case "gwp":
                        case "gwp20":
                        case "gwp100":
                        case "gwp500":
                            if (this.Results.ResultObj == null)
                            {
                                this.Results.ResultObj = new EHSModel.GHGResultList().CreateNew(fromDate, toDate);
                            }
                            foreach (decimal plantID in plantArray)
                            {
                                EHSModel.GHGResultList ghgTable =  CalcGHG(fromDate, toDate, plantID, measureArray);
                                ((EHSModel.GHGResultList)this.Results.ResultObj).AddResultRange(ghgTable);
                                value += ghgTable.GHGTotal;
                            }
                            break;
                        case "norm":
                        case "normCost":
                            valid = false;
                            
                            EHS_MEASURE factMeasure = null;
                            PLANT plant = GetPlant(plantArray[0]);

                            if (!string.IsNullOrEmpty(this.SubScope))
                            {
                                factMeasure = GetMeasure(Convert.ToDecimal(this.SubScope));
                            }
                            else 
                            {
                                if (plant.EHS_PROFILE != null && plant.EHS_PROFILE.EHS_PROFILE_FACT.FirstOrDefault() != null)
                                    factMeasure = GetMeasure((decimal)plant.EHS_PROFILE.EHS_PROFILE_FACT.Where(f => f.CALCS_STAT.ToLower() == this.Calculation.ToLower()).FirstOrDefault().FACTOR_ID);
                            }

                            if (factMeasure != null  &&  factMeasure.MEASURE_ID > 0)
                            {
                                decimal sumVal;
                                this.Results.FactorText = factMeasure.MEASURE_NAME;
                                if (Calculation == "normCost")
                                {
                                    sumVal = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                                }
                                else
                                {
                                    sumVal = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_VALUE).ToList().Sum();
                                }
                                decimal sumFact = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { factMeasure.MEASURE_ID }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                                if (sumFact != 0 && factMeasure.STD_UOM.HasValue)
                                {
                                    if ((sumFact = (decimal)SQMResourcesMgr.ConvertUOM((decimal)factMeasure.STD_UOM, (decimal)factMeasure.STD_UOM, (double)sumFact, true)) != 0)
                                    {
                                        value = sumVal / sumFact;
                                        valid = true;
                                    }
                                }
                            }
                            break;
                        case "eeff":
                            decimal sumE = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            decimal sumPCost = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000000 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            decimal sumPRevenue = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000001 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            if (sumE > 0 && sumPCost > 0 && sumPRevenue > 0)
                            {
                                value = sumE / (sumPRevenue - sumPCost);
                            }
                            else
                                valid = false;
                            break;
                        case "eSeff":
                            decimal sumESpend = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                            sumPCost = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000000 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            sumPRevenue = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000001 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            if (sumESpend > 0 && sumPCost > 0 && sumPRevenue > 0)
                            {
                                value = sumESpend / (sumPRevenue - sumPCost);
                            }
                            else
                                valid = false;
                            break;
                        default:   // undefined calculation - just return the sum of measure values
                            value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            break;
                    }
                }
                else
                {
                    switch (this.MetricScope)
                    {
                        // rcr, ltcr, ltsr should return zero and valid result if no occurences found
                        case "rcr":
                            temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
                            temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000007 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            if (temp2 != 0)
                                value = (temp * temp3) / temp2;
                            break;
                        case "ltcr":
                            temp3 = ProRateWorkHours(fromDate, toDate, plantArray);     // hours basis
                            temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000006 }).Select(l => l.MEASURE_VALUE).ToList().Sum();  // lost time cases
                            temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MEASURE_VALUE).ToList().Sum();  // hrs worked
                            if (temp2 != 0)
                                value = (temp * temp3) / temp2;
                            break;
                        case "ltsr":
                            temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
                            temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000005 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            if (temp2 != 0)
                                value = (temp * temp3) / temp2;
                            break;
                        case "cost":
                            //value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                            break;
                        case "WASTE_CAT":
                        case "REG_STATUS":
                        case "DISPOSAL":
                            string attributeValue = this.Calc.AttributeXref.ElementAt((int)Math.Abs(measureArray[0]) - 1).Key;
                            decimal[] measuresForAttribute = SelectMeasureByAttribute(this.MetricScope, attributeValue, plantArray);

                            if (attributeValue == "EWMR")
                            {
                                ;
                            }
                            if (measuresForAttribute != null && measuresForAttribute.Length > 0)
                            {
                                if (this.Stat == SStat.cost)
                                    value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measuresForAttribute).Select(l => l.MEASURE_COST).ToList().Sum();
                                else
                                {
                                    foreach (decimal plantID in plantArray)
                                    {
                                        measuresForAttribute = SelectMeasureByAttribute(this.MetricScope, attributeValue, new decimal[1] {plantID});
                                        value += this.InitCalc().Calc.Select(fromDate, toDate, new decimal[1] { plantID }, measuresForAttribute).Select(l => l.MEASURE_VALUE).ToList().Sum();
                                    }
                                }
                            }
                            break;
                        default:
                            if (this.Stat == SStat.cost)
                                value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                            else
                                value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_VALUE).ToList().Sum();
                            break;
                    }
                }
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        public decimal[] SelectMeasureByAttribute(string attribute, string attributeValue, decimal[] plantArray)
        {
            decimal[] measureArray = null;
  
            try
            {
                switch (attribute)
                {
                    case "WASTE_CAT":  // specific waste category
                        measureArray = (from m in this.Entities.EHS_PROFILE_MEASURE.Include("EHS_MEASURE")
                                        where (plantArray.Contains((decimal)m.PLANT_ID) && m.EHS_MEASURE.MEASURE_CATEGORY == attributeValue)
                                select m.MEASURE_ID).ToArray();
                        break;
                    case "REG_STATUS":
                        measureArray =  (from m in this.Entities.EHS_PROFILE_MEASURE.Include("EHS_MEASURE")
                                         where (plantArray.Contains((decimal)m.PLANT_ID) && m.REG_STATUS == attributeValue)
                                                    select m.MEASURE_ID).ToArray();
                        break;
                    case "DISPOSAL":
                        measureArray = (from m in this.Entities.EHS_PROFILE_MEASURE.Include("EHS_MEASURE")
                                                    where (plantArray.Contains((decimal)m.PLANT_ID) && m.UN_CODE.StartsWith(attributeValue))
                                                    select m.MEASURE_ID).ToArray();
                        break;
                    default: break;
                }
            }
            catch { ; }

            return measureArray;
        }

        public decimal ProRateWorkHours(DateTime fromDate, DateTime toDate, decimal[] plantArray)
        {
            decimal hrsFactor = 0;
            decimal hrsBase = 200000m / 12m;    // est hours worked per month per plant


           // int numPeriods = ((toDate.Year - fromDate.Year) * 12) + (toDate.Month - fromDate.Month) + 1;
           // hrsFactor = hrsBase * numPeriods; // * plantArray.Length;
            hrsFactor = 200000;  // per Varroc, this is an absolute constant regardless of number of periods analyzed 

            return hrsFactor;
        }

        public EHSModel.GHGResultList CalcGHG(DateTime fromDate, DateTime toDate, decimal plantID, decimal[] measureArray)
        {
            PLANT plant = this.GetPlant(plantID);
            EHSModel.Emissions emc = new EHSModel.Emissions().Initialize(plant.LOCATION_CODE, plant.COMP_INT_ID);
            string[] ghgCodes = this.SubScope.Split(',');
            List<WebSiteCommon.DatePeriod> pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.year, this.DateSpanType, "");

            EHSModel.GHGResultList ghgTable = new EHSModel.GHGResultList().CreateNew(fromDate, toDate);

            foreach (decimal measID in measureArray)
            {
                EHS_MEASURE meas = this.MetricHst.Where(l => l.MEASURE_ID == measID).Select(l => l.EHS_MEASURE).FirstOrDefault();
                if (meas != null  &&  !string.IsNullOrEmpty(meas.EFM_TYPE))
                {
                    foreach (WebSiteCommon.DatePeriod pd in pdList)
                    {
                        decimal metricQty = this.InitCalc().Calc.Select(pd.FromDate, pd.ToDate, new decimal[1] {plantID}, new decimal[1] { measID }).Select(l => l.MEASURE_VALUE).Sum();
                        EHS_METRIC_HISTORY hist1 = this.Calc.Metrics.FirstOrDefault();
                        if (hist1 != null)
                        {
                            int gasSeq = 0;
                            foreach (string ghgCode in ghgCodes)
                            {
                                ++gasSeq;
                                if (ghgCode.ToUpper() == "CO2" || meas.EFM_TYPE != "P")
                                {
                                    decimal ghg = emc.LookupGHG(meas.EFM_TYPE, pd.FromDate.Year, ghgCode);
                                    decimal gwp = emc.GetGWP(ghgCode, this.Calculation);
                                    decimal ghgQty = metricQty * ghg * gwp;
                                    ghgTable.SumTotal(ghgQty);
                                    decimal ghgUOM = 17; // calculated in kg
                                    try
                                    {
                                        EHSModel.GHGResult ghgRrec = new EHSModel.GHGResult().CreateNew(plant, meas, 0, hist1.INPUT_UOM_ID.HasValue ? (decimal)hist1.INPUT_UOM_ID : 0,
                                            metricQty, hist1.UOM_ID, meas.EFM_TYPE, gasSeq, ghg, metricQty * ghg, ghgCode, gwp, ghgQty, ghgUOM);  
                                        ghgTable.AddResult(ghgRrec);
                                    }
                                    catch
                                    {
                                        ;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ghgTable;
        }

        public int IncidentStat(decimal[] plantArray, decimal[] topicArray, DateTime fromDate, DateTime toDate)
        {
            int status = 0;
            this.Results.Result = 0;
            this.Results.ValidResult = true;
            try
            {
                this.Results.Result = 0;
                this.Results.ValidResult = true;
                switch (this.Stat)
                {
                    case SStat.deltaDy:
                        List<EHSIncidentData> incidentList =  LookupIncidentTopicOccurs(plantArray, new decimal[1] {8}, topicArray[0], "YES");  // get specific topic
                        if (incidentList != null && incidentList.Count > 0)
                        {
                            this.Results.Result = (decimal)Math.Truncate(DateTime.Now.Subtract(incidentList.Select(l => l.Incident.INCIDENT_DT).Last()).TotalDays);
                        }
                        else
                            this.Results.ValidResult = false;
                        break;
                    case SStat.sum:
                    default:
                        this.Results.Result = this.InitCalc().Calc.SelectIncidents(fromDate, toDate, plantArray, topicArray).Select(l => l.Incident).Count();
                        break;
                }
            }
            catch
            {
                this.Results.ValidResult = false;
            }

            return status;
        }

        public int IncidentSeries(EHSCalcsCtl.SeriesOrder seriesOrder, decimal[] plantIDS, DateTime fromDate, DateTime toDate, decimal[] topicIDS)
        {
            decimal result0 = 0;
            GaugeSeries series = null;
            PLANT plant = null;
            this.Results.Initialize();
            string value = ""; int count = 0; int seriesitem = 0;
            List<string> keyList = new List<string>();
            List<WebSiteCommon.DatePeriod> pdList = new List<WebSiteCommon.DatePeriod>();
            decimal keyCount = 0;
            int nItem = -1;

            switch (seriesOrder)
            {
                case EHSCalcsCtl.SeriesOrder.TimespanSeries:

                    int[] tss;
                    string[] args = this.SubScope.Split(',');
                    if (args.Length > 1)
                        tss = args.Select(x => int.Parse(x)).ToArray();
                    else 
                        tss = new int[4]  {1,8,31,91};

                    GaugeSeriesItem item;
                    DateTime now = DateTime.Now;
                    series = new GaugeSeries().CreateNew(0, "", "");

                    for (int n=0; n<tss.Length; n++)
                    {
                        item = new GaugeSeriesItem();
                        item.Item = n+1;
                        if (n == 0)
                        {
                            item.Text = tss[n].ToString() + " to " + (tss[n + 1] - 1).ToString() + " Days";
                            item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) < tss[n+1]).Count();
                        }
                        else if (n == tss.Length - 1)
                        {
                            item.Text = (tss[n] - 1).ToString() + "+ Days";
                            item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) >= tss[n]).Count();
                        }
                        else
                        {
                            item.Text = tss[n].ToString() + " to " + (tss[n + 1] - 1).ToString() + " Days";
                            item.YValue = this.IncidentHst.Where(l => Math.Max(l.DaysOpen, l.DaysToClose) >= tss[n] && Math.Max(l.DaysOpen, l.DaysToClose) < tss[n+1]).Count();
                        }
                        
                        series.ItemList.Add(item);
                    }
                    
                    this.Results.metricSeries.Add(series);
                    break;

                case EHSCalcsCtl.SeriesOrder.SumTotal:
                case EHSCalcsCtl.SeriesOrder.MeasureSeries:
                    decimal totalCount = (decimal)this.IncidentHst.Count();
                   
                    switch (this.MetricScope)
                    {
                        case "TYPE_INCIDENT":
                        case "type":
                            keyList = this.IncidentHst.Select(l => l.Incident.ISSUE_TYPE).Distinct().ToList();
                            break;
                        case "plant":
                            keyList = this.IncidentHst.Select(l => l.Plant.PLANT_NAME).Distinct().ToList();
                            break;
                        case "status":
                            keyList = this.IncidentHst.Select(l => l.Status).Distinct().ToList();
                            break;
                        //case "month":
                        //    foreach (WebSiteCommon.DatePeriod pd in WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, DateSpanOption.FYEffTimespan, ""))
                        //        pdList.Add(pd);
                        //    break;
                        default:
                            break;
                    }
                    
                    series = new GaugeSeries().CreateNew(0, "", "");
                    nItem = -1;
                    string itemText = "";
                    foreach (string key in keyList)
                    {
                        itemText = key;
                        switch (this.MetricScope)
                        {
                            case "TYPE_INCIDENT":
                            case "type":
                                keyCount = (decimal)this.IncidentHst.Where(l => l.Incident.ISSUE_TYPE == key).Count();
                            break;
                            case "plant":
                                keyCount = (decimal)this.IncidentHst.Where(l => l.Plant.PLANT_NAME == key).Count();
                            break;
                            case "status":
                                keyCount = (decimal)this.IncidentHst.Where(l => l.Status == key).Count();
                                itemText = WebSiteCommon.GetXlatValue("incidentStatus", key);
                            break;
                            default:
                            break;
                        }
                        if (this.Stat == SStat.pct)  //if (stat == SStat.pct)
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nItem, 0, (keyCount / totalCount) * 100m, itemText));
                        else
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nItem, 0, keyCount, itemText));
                    }
                    this.Results.metricSeries.Add(series);
                    break;

                case EHSCalcsCtl.SeriesOrder.PlantMeasure:
                    keyList = this.IncidentHst.Select(l => l.Incident.ISSUE_TYPE).Distinct().ToList();
                    foreach (string key in keyList)
                    {
                        series = new GaugeSeries().CreateNew(0, key, "");
                        nItem = -1;
                        foreach (decimal plantID in plantIDS)
                        {
                            plant = GetPlant(plantID);
                            keyCount = 0;
                            foreach (EHSIncidentData data in this.IncidentHst.Where(l => l.Incident.DETECT_PLANT_ID == plantID && l.Incident.ISSUE_TYPE == key))
                            {
                                if (data.EntryList.Where(e => e.INCIDENT_QUESTION_ID == 1).Count() > 0)
                                    ++keyCount;
                            }
                          //  keyCount = (decimal)this.IncidentHst.Where(l => l.Incident.DETECT_PLANT_ID == plantID && l.Incident.ISSUE_TYPE == key  &&  l.Topic.INCIDENT_QUESTION_ID == 1).Distinct().Count();
                            series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++nItem, 0, keyCount, plant.PLANT_NAME));
                        }
                        this.Results.metricSeries.Add(series);
                    }
                    break;

                case EHSCalcsCtl.SeriesOrder.YearPlant:
                   // pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(1), toDate, DateIntervalType.year, this.DateSpanType, "Total");
                    if (this.DateSpanType == DateSpanOption.FYYearOverYear)  // don't deduct start year because it was already done upstream
                        pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.FYyear, this.DateSpanType, "Total");
                    else
                        pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate, DateIntervalType.FYyear, this.DateSpanType, "Total");
                    List<AttributeValue> attrList = new List<AttributeValue>();
                    foreach (decimal incidentType in topicIDS)
                    {
                        string key = this.IncidentTypeList.Where(l => l.INCIDENT_TYPE_ID == incidentType).Select(l => l.TITLE).FirstOrDefault();
                        series = new GaugeSeries().CreateNew(0, key, "");
                        List<GaugeSeriesItem> yearTotals = new List<GaugeSeriesItem>();
                        foreach (WebSiteCommon.DatePeriod pd in pdList)  //foreach (DateTime dt in this.Calc.FromDates)
                        {
                            yearTotals.Add(new GaugeSeriesItem().CreateNew(0, 0, 0, 0, pd.Label + " TOTAL"));
                            attrList.Add(new AttributeValue().CreateNew(pd.Label + " Total", 0));
                        }
                        foreach (decimal plantID in plantIDS)
                        {
                            nItem = -1;
                            foreach (WebSiteCommon.DatePeriod pd in pdList)
                            {
                                ++nItem;
                                plant = GetPlant(plantID);
                                keyCount = (decimal)this.IncidentHst.Where(l => l.Incident.DETECT_PLANT_ID == plantID && l.Incident.ISSUE_TYPE == key && (l.Incident.INCIDENT_DT >= pd.FromDate && l.Incident.INCIDENT_DT <= pd.ToDate)).Distinct().Count();
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, nItem, 0, keyCount, (pd.Label + "&nbsp;&nbsp;" + plant.PLANT_NAME)));
                                attrList[nItem].Value += keyCount;
                                yearTotals[nItem].YValue += keyCount;
                            }
                        }
                        foreach (GaugeSeriesItem si in yearTotals)
                        {
                            series.ItemList.Insert(0, si);
                        }
                        this.Results.metricSeries.Add(series);
                    }
                    this.Results.ValidResult = true;
                    this.Results.metricSeries[0].ObjData = attrList;
                    break;

                case EHSCalcsCtl.SeriesOrder.MeasurePlant:
                default:
                    foreach (decimal topic in topicIDS)
                    {
                        this.InitCalc().Calc.SelectIncidents(fromDate, toDate, plantIDS, new decimal[0] {});
                        series = new GaugeSeries().CreateNew(0, "", "");
                        decimal numAnswers = 0;
                        Dictionary<string, decimal> foList = new Dictionary<string,decimal>();
                        foreach (EHSIncidentData d in this.Calc.Incidents)
                        {
                            INCIDENT_ANSWER entry = d.EntryList.Where(l => l.INCIDENT_QUESTION_ID == topic).FirstOrDefault();
                            if (entry != null  &&  !string.IsNullOrEmpty(entry.ANSWER_VALUE))
                            {
                                ++numAnswers;
                                series.Name = entry.INCIDENT_QUESTION.QUESTION_TEXT;
                                if (foList.ContainsKey(entry.ANSWER_VALUE))
                                    foList[entry.ANSWER_VALUE] += 1;
                                else
                                    foList.Add(entry.ANSWER_VALUE, 1);
                            }
                        }
 
                        foreach (KeyValuePair<string, decimal> fo in foList)
                        {
                            if (this.Stat == SStat.pct)
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++seriesitem, 0, (fo.Value / numAnswers) * 100m, fo.Key)); 
                            else
                                series.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++seriesitem, 0, fo.Value, fo.Key)); 
                        }
                      
                        if (series.ItemList.Count > 0)
                            this.Results.metricSeries.Add(series);
                    }

                    if (this.Results.metricSeries.Count > 0)
                        this.Results.ValidResult = true;
                    break;
            }

            return seriesitem;
        }

        public int ElapsedTimeSeries(decimal[] plantIDS,  decimal[] incidentTypeIDS, decimal[] topicIDS, string topicValue, bool bStartDate)
        {
            int status = 0;
            this.Results.Initialize();
            GaugeSeries series = new GaugeSeries().CreateNew(0, "", "");
            int nItem = -1;
          
            try
            {
                List<PLANT> plantList = (from p in this.Entities.PLANT where plantIDS.Contains(p.PLANT_ID) select p).OrderBy(p => p.PLANT_NAME).ToList();

                List<INCIDENT> dataList = (from i in this.Entities.INCIDENT
                                join r in this.Entities.INCIDENT_ANSWER on i.INCIDENT_ID equals r.INCIDENT_ID
                                join q in this.Entities.INCIDENT_QUESTION on r.INCIDENT_QUESTION_ID equals q.INCIDENT_QUESTION_ID
                                join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
                                where (i.INCIDENT_TYPE == "EHS" && incidentTypeIDS.Contains((decimal)i.ISSUE_TYPE_ID) && plantIDS.Contains((decimal)i.DETECT_PLANT_ID)
                                    && topicIDS.Contains(r.INCIDENT_QUESTION_ID) && r.ANSWER_VALUE.ToUpper() == topicValue)
                                    group i by i.DETECT_PLANT_ID into d 
                                select d.OrderByDescending(l=> l.INCIDENT_DT).FirstOrDefault() ).ToList();
               
                INCIDENT incident = null;

                foreach (PLANT plant in plantList)
                {
                    try
                    {
                        CalcsResult result = new CalcsResult().Initialize();
                        result.Text = plant.PLANT_NAME;
                        result.ValidResult = false;

                        if ((incident = dataList.Where(l => l.DETECT_PLANT_ID == plant.PLANT_ID).FirstOrDefault()) != null)
                        {
                            result.Result = (decimal)Math.Truncate(DateTime.Now.Subtract(incident.INCIDENT_DT).TotalDays);
                            result.Result = Math.Max(0, result.Result - 1);
                            result.ValidResult = true;
                        }
                        else
                        {
                            if (plant.PLANT_START_DT.HasValue)
                            {
                                result.Result = (decimal)Math.Truncate(DateTime.Now.Subtract((DateTime)plant.PLANT_START_DT).TotalDays);
                                result.ValidResult = true;
                            }
                        }

                        series.AddItem(nItem++, plant.PLANT_NAME, result);
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            catch (Exception ex)
            {
              //  SQMLogger.LogException(ex);
            }

            this.Results.metricSeries.Add(series);

            return status;
        }

    }
}