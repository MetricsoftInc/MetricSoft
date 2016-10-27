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
    public enum SStat { none, value, sum, pctChange, deltaDy, sumCost, count, cost, pct, pctReduce, ratio, ratioPct, normRev};
    public enum HSAttr { none, type};

	public class AttributeValue
	{
		public string Group
		{
			get;
			set;
		}
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

		public AttributeValue CreateNew(string group, string key, decimal value)
		{
			this.Group = group;
			this.Key = key;
			this.Value = value;
			return this;
		}

		public AttributeValue CreateNew(string key, decimal value)
		{
			this.Group = "";
			this.Key = key;
			this.Value = value;
			return this;
		}
	}

	public class MetricData
	{
		public EHS_METRIC_HISTORY MetricRec
		{
			get;
			set;
		}
		public EHS_MEASURE Measure
		{
			get;
			set;
		}
		public List<AttributeValue> AtrList
		{
			get;
			set;
		}
		public decimal DataID
		{
			get;
			set;
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
                        total += gsi.YValue ?? 0;
                    }
                }
                foreach (GaugeSeries gs in this.metricSeries)
                {
                    foreach (GaugeSeriesItem gsi in gs.ItemList)
                    {
                        if (seriesOrder == EHSCalcsCtl.SeriesOrder.MeasureSeries)
                        {
                            if (total != 0)
                                gsi.YValue = Decimal.Round((gsi.YValue ?? 0) / total * 100m);
                            else
                                gsi.YValue = 0;
                        }
                        else
                        {
                            tempItem = tempList.Where(l => l.Text == gsi.Text).FirstOrDefault();
                            if (tempItem.YValue != 0)
                                gsi.YValue = Decimal.Round((gsi.YValue ?? 0) / (tempItem.YValue ?? 1) * 100m);
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

        public List<MetricData> Metrics
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
		public List<XLAT> XLATSeries
		{
			get;
			set;
		}

        public List<MetricData> Select(DateTime fromDate, DateTime toDate, decimal[] plantArray, decimal[] measureArray)
        {
            this.Metrics = this.Metrics.Where(l => new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, 1) >= fromDate && new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, DateTime.DaysInMonth(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH)) <= toDate).ToList();

            if ((NumPlants = plantArray.Length) > 0)
            {
                this.Metrics = this.Metrics.Where(l => plantArray.Contains(l.MetricRec.PLANT_ID)).ToList();
            }
            if ((NumMeasures = measureArray.Length) > 0)
            {
				this.Metrics = this.Metrics.Where(l => measureArray.Contains(l.MetricRec.MEASURE_ID)).ToList();
            }

            return this.Metrics;
        }

		public List<MetricData> SelectByAttribute(DateTime fromDate, DateTime toDate, decimal[] plantArray, string group, string attributeValue)
		{

			//decimal[] mds = this.Metrics.Where(l => l.AtrList != null  &&  l.AtrList.Any(a => a.Group == group && a.Key == attributeValue)).Select(m => m.Measure.MEASURE_ID).Distinct().ToArray();
			if (group == "DISPOSAL")
			{
				this.Metrics = this.Metrics.Where(l => new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, 1) >= fromDate && new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, DateTime.DaysInMonth(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH)) <= toDate && plantArray.Contains(l.MetricRec.PLANT_ID) && l.AtrList != null && l.AtrList.Any(a => a.Group == group && a.Key.StartsWith(attributeValue))).ToList();
			}
			else
			{
				this.Metrics = this.Metrics.Where(l => new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, 1) >= fromDate && new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, DateTime.DaysInMonth(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH)) <= toDate && plantArray.Contains(l.MetricRec.PLANT_ID) && l.AtrList != null && l.AtrList.Any(a => a.Group == group && a.Key == attributeValue)).ToList();
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
		public enum SeriesOrder { MeasureSeries, MeasurePlant, PlantMeasure, PeriodMeasurePlant, YearMeasurePlant, PeriodMeasure, YearMeasure, YearPlant, YearPlantAvg, PlantMeasureTotal, PlantMeasureAvg, SumTotal, TimespanSeries, PeriodMeasureYOY, PeriodSum, PeriodSumYOY, YearSum, MeasurePlantTotal, PeriodMeasurePlantTotal };
        //                            0               1               2               3                   4               5               6           7           8           9                   10            11              12              13              14          15      16		17					18
        public enum MetricElement { value, cost };
        public enum AccountingElement { cost, revenue, production, throughput, timeworked, timelost, recordedcases, timelostcases};

		public string Perspective
		{
			get;
			set;
		}
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
        public List<MetricData> MetricHst
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
		public List<EHSAuditData> AuditHst
		{
			get;
			set;
		}
		public List<EHSAuditSchedulerData> AuditSchedulerHst
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
        public TargetMgr TargetCtl
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
		public string Filter
		{
			get;
			set;
		}
		public string Options 
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

        public EHSCalcsCtl CreateNew(int FYStartMonth, DateSpanOption dateSpanType, string perspective)
        {
			this.Perspective = perspective;
            this.MetricHst = new List<MetricData>();
            this.PlantHst = new List<PLANT_ACCOUNTING>();
            this.Entities = new PSsqmEntities();
            this.Calc = new EHSCalcs();
            this.Calc.Metrics = new List<MetricData>();
            this.Calc.Accounting = new List<PLANT_ACCOUNTING>();
            this.Results = new CalcsResult().Initialize();
            this.FYStartMonth = FYStartMonth;   // use for year over year calculations
            this.DateInterval = DateIntervalType.fuzzy;
            this.DateSpanType = dateSpanType;
            this.PlantList = new List<PLANT>();
            //this.TopicSelects = new decimal[] { 1, 12, 13, 62, 63, 78, 81, 82, 83 };
			this.TopicSelects = new decimal[] { 1, 2, 12, 13, 24, 27, 54, 62, 63, 64, 65, 69, 78, 80, 81, 82, 83, 84, 86, 88, 92, 93 };
            return this;
        }

        public EHSCalcsCtl SetCalcParams(string calcsMethod, string scope, string stat, int seriesOrder, string filter, string options)
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


				if (scope == "eeff" || scope == "eSeff" || scope == "eReff" || scope == "eReffCost")
                {
                    this.MetricScope = "ENGY";
					this.Calculation = scope;
                    this.Stat = (SStat)Enum.Parse(typeof(SStat), stat, true);
                }

				this.Filter = filter;
				this.Options = options;

            }
            catch
            {
                this.Stat = SStat.sum;
                this.Seriesorder = EHSCalcsCtl.SeriesOrder.MeasureSeries;
            }

            return this;
        }

        public EHSCalcsCtl SetTargets(TargetMgr targetMgr)
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
            PLANT plant = this.PlantList.Where(l => l.PLANT_ID == plantID).FirstOrDefault();
            if (plant == null)
            {
                plant = (from pl in this.Entities.PLANT.Include("EHS_PROFILE").Include("EHS_PROFILE.EHS_PROFILE_FACT")
                                    where pl.PLANT_ID == plantID select pl).Single();
                this.PlantList.Add(plant);
            }

            return plant;
        }
        public EHS_MEASURE GetMeasure(decimal measureID)
        {
            EHS_MEASURE measure = null;

            if (this.Calc.XLATSeries != null  &&  this.Calc.XLATSeries.Count > 0)
            {
                measure = new EHS_MEASURE();
				measure.MEASURE_NAME = this.Calc.XLATSeries.ElementAt((int)measureID).XLAT_CODE;
				measure.MEASURE_DESC = this.Calc.XLATSeries.ElementAt((int)measureID).DESCRIPTION_SHORT;
            }
            else if (measureID == 0)
            {
                measure = new EHS_MEASURE();
            }
            else
            {
                measure = this.MetricHst.Where(l => l.Measure.MEASURE_ID == measureID).Select(l => l.Measure).FirstOrDefault();
                if (measure == null)
                    measure = EHSModel.LookupEHSMeasure(this.Entities, measureID, "");
            }
            return measure;
        }

		public string MeasureLabel(EHS_MEASURE measure)
		{
			// return measure description if it represents a member of an XLAT (iterating thru attrubute list)
			return measure.MEASURE_ID > 0 ? measure.MEASURE_NAME.Trim() : measure.MEASURE_DESC.Trim();
		}

        public decimal[] GetPlantsByScope(decimal[] plantIDS)
        {
			List<PLANT> plantList = this.PlantList.Where(p => plantIDS.Contains(p.PLANT_ID)).ToList();

            switch (this.CalcsMethod)
            {
                case "E":
                    plantList = this.PlantList.Where(l => l.TRACK_EW_DATA == true).ToList();
                    switch (this.SubScope)
                    {
                        case "":
                            plantList = plantList.Where(l => plantIDS.Contains(l.PLANT_ID)).ToList();
                            break;
                        default:
                            if (!string.IsNullOrEmpty(this.SubScope)  && this.Calculation.ToLower().Contains("norm"))
                            {
                                int id;
                                if (int.TryParse(this.SubScope, out id))
                                {
									// this logic special for TI - requires the plant profile to use the metric ID as it's normalization factor
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
						case "eReff":
						case "eReffCost":
                            plantList = plantList.Where(l => l.TRACK_FIN_DATA == true).ToList();
                            break;
                        default:
                            break;
                    }
                    switch (this.Calculation)
                    {
                        case "eeff":
                        case "eSeff":
						case "eReff":
						case "eReffCost":
                            plantList = plantList.Where(l => l.TRACK_FIN_DATA == true).ToList();
                            break;
                        default:
                            break;
                    }
                    //return plantList.Select(l => l.PLANT_ID).ToArray();
                    break;
                case "HS":
                case "IN":
                    break;
                default:
                    break;
            }

			if (!string.IsNullOrEmpty(this.Filter))
			{
				string[] filterArgs = WebSiteCommon.SplitString(this.Filter, ':');		// ex:  BU|2,4,5
				if (filterArgs.Length > 1)
				{
					string[] filterItems = WebSiteCommon.SplitString(filterArgs[1], ',');
					try
					{
						switch (filterArgs[0])	// filter type 
						{
							case "B":	// bus orgs
								if (filterItems.Length > 0)
								{
									plantList = plantList.Where(p => filterItems.Contains(p.BUS_ORG_ID.ToString())).ToList();
								}
								break;
							case "P":	// plants
								if (filterItems.Length > 0)
								{
									plantList = plantList.Where(p => filterItems.Contains(p.PLANT_ID.ToString())).ToList();
								}
								break;
							case "L":	// location code  (country ?)
								if (filterItems.Length > 0)
								{
									plantList = plantList.Where(p => filterItems.Contains(p.LOCATION_CODE)).ToList();
								}
								break;
							case "R":	// region
								if (filterItems.Length > 0)
								{
									plantList = plantList.Where(p => filterItems.Contains(p.COMP_INT_ID)).ToList();
								}
								break;
							default:
								break;
						}
					}
					catch
					{
					}
				}
			}

			return plantList.Select(l => l.PLANT_ID).ToArray();
            //return plantIDS;
        }

		public decimal[] GetMetricsByScope()
		{
			return GetMetricsByScope("");
		}
        public decimal[] GetMetricsByScope(string scopeOverride)
        {
			List<decimal> measureList = new List<decimal>();
			string scope = !string.IsNullOrEmpty(scopeOverride) ? scopeOverride : this.MetricScope;
            int id = 0;
            string s;

			this.Calc.XLATSeries = new List<XLAT>();

            if (scope.Contains(',') || int.TryParse(scope, out id))
            {
                string[] ids = scope.Split(',');
                measureList = ids.Select(i => decimal.Parse(i)).ToList();
            }
            else if (scope.Contains("EFM_"))  // get specific efm type
            {
                string[] efm = scope.Split('_');
                measureList = this.MetricHst.Where(m => m.Measure.EFM_TYPE == efm[1]).Select(m => m.Measure.MEASURE_ID).Distinct().ToList();
            }
            else if (scope.ToUpper().Contains("CO2")  ||  scope.ToUpper().Contains("GHG"))
            {
                measureList = this.MetricHst.Where(m => m.Measure.MEASURE_CATEGORY == "ENGY").Select(m => m.Measure.MEASURE_ID).Distinct().ToList();
            }
            else // get all metrics of a specified catetory
            {
                switch (scope)
                {
                    case "WASTE":
                        measureList = this.MetricHst.Where(m => m.Measure.MEASURE_CATEGORY.StartsWith("EW")).Select(m => m.Measure.MEASURE_ID).Distinct().ToList();
                        break;
                    case "WASTE_CAT":
						this.Calc.XLATSeries.AddRange(SQMBasePage.SelectXLATList(new string[1] { "MEASURE_CATEGORY" }, SessionManager.UserContext.Language.LANGUAGE_ID).Where(x=> x.XLAT_CODE.StartsWith("EW")).ToList());
						for (int i = 0; i < this.Calc.XLATSeries.Count; i++)
						{
							measureList.Add(i);
						}
                        break;
					case "DISPOSAL":
					case "REG_STATUS":
					case "INJURY_TYPE":
					case "INJURY_PART":
					case "INJURY_TENURE":
					case "INJURY_CAUSE":
					case "INJURY_DAYS_TO_CLOSE":
						this.Calc.XLATSeries.AddRange(SQMBasePage.SelectXLATList(new string[1] { scope }, SessionManager.UserContext.Language.LANGUAGE_ID));
						for (int i = 0; i < this.Calc.XLATSeries.Count; i++)
						{
							measureList.Add(i);
						}
						break;
					case "AUDIT_TYPE":
						measureList = this.MetricHst.Where(m => m.Measure.MEASURE_CD.StartsWith("S3") && !m.Measure.MEASURE_CD.EndsWith("0")).Select(m => m.Measure.MEASURE_ID).Distinct().ToList();
						break;
                    default:
                        measureList = this.MetricHst.Where(m => m.Measure.MEASURE_CATEGORY == scope).Select(m => m.Measure.MEASURE_ID).Distinct().ToList();
                        break;
                }

				if (measureList.Count == 0)
					measureList.Add(0);
            }

			return measureList.ToArray();		// for backwards compatibility with all the other related methods 
        }

		public decimal[] GetMetricsByFieldName(string fieldName)
		{
			return this.MetricHst.Where(m => m.Measure.PLANT_ACCT_FIELD == fieldName).Select(m => m.Measure.MEASURE_ID).Distinct().ToArray();
		}

		public decimal[] GetMetricsByMeasureCode(string fieldName)
		{
			return this.MetricHst.Where(m => m.Measure.MEASURE_CD == fieldName).Select(m => m.Measure.MEASURE_ID).Distinct().ToArray();
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
													 where t.INCIDENT_TYPE_ID != 10 && t.INCIDENT_TYPE_ID != 5 && t.INCIDENT_TYPE_ID != 11 && t.INCIDENT_TYPE_ID != 12
                                                     select t).ToList();
                            topicArray = IncidentTypeList.Select(l => l.INCIDENT_TYPE_ID).ToArray();
                            break;
                        case "TYPE_AUDIT":		// IMS audit, EHS walk, Near miss
                            this.IncidentTypeList = (from t in this.Entities.INCIDENT_TYPE	
													 where t.INCIDENT_TYPE_ID == 5 || t.INCIDENT_TYPE_ID == 11 || t.INCIDENT_TYPE_ID == 12 
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

		public EHSCalcsCtl LoadEHSData(decimal[] plantIDS, DateTime startDate, DateTime endDate, DateIntervalType dateIntervalType, decimal[]measureIDS)
		{
			this.DateInterval = dateIntervalType;
			this.MetricHst = new List<MetricData>();
			this.PlantHst = new List<PLANT_ACCOUNTING>();
			DateTime fromDate = WebSiteCommon.PeriodFromDate(startDate);
			DateTime toDate = WebSiteCommon.PeriodToDate(endDate);

			try
			{
				this.PlantList = (from pl in this.Entities.PLANT 
								  where (plantIDS.Contains((decimal)pl.PLANT_ID))
								  select pl).ToList();

				this.MetricHst = (from h in this.Entities.EHS_DATA
							 join m in this.Entities.EHS_MEASURE on h.MEASURE_ID equals m.MEASURE_ID into m_h
							 from m in m_h.DefaultIfEmpty()
							 join o in Entities.EHS_DATA_ORD on h.DATA_ID equals o.DATA_ID into ordlist
							 where (
								   plantIDS.Contains((decimal)h.PLANT_ID)
								   && (measureIDS.Count() == 0  || measureIDS.Contains(h.MEASURE_ID))
								   && h.DATE >= fromDate && h.DATE <= toDate
								   && h.VALUE != null 
									 )
							 select new
							 {
								 EhsData = h,
								 Measure = m,
								 DataID = h.DATA_ID,
								 OrdList = ordlist,
								 Value = h.VALUE
							 }).ToList().Where(r => (measureIDS.Length == 0  || measureIDS.Contains(r.Measure.MEASURE_ID))).Select(r => new MetricData 
							 {
								 MetricRec = new EHS_METRIC_HISTORY() { PLANT_ID = r.EhsData.PLANT_ID, MEASURE_ID = r.EhsData.MEASURE_ID, PERIOD_YEAR = r.EhsData.DATE.Year, PERIOD_MONTH = r.EhsData.DATE.Month, LAST_UPD_DT = r.EhsData.DATE, MEASURE_VALUE = r.EhsData.VALUE.HasValue ? (decimal)r.EhsData.VALUE : 0, STATUS = r.EhsData.VALUE.HasValue ? "A" : "N" },
								 Measure = r.Measure,
								 DataID = r.DataID,
								 AtrList = TransformOrdList((List<EHS_DATA_ORD>)r.OrdList)
							 }).ToList();

			}
			catch (Exception ex)
			{
				; //   SQMLogger.LogException(ex);
			}

			return this;
		}

		public List<AttributeValue> TransformOrdList(List<EHS_DATA_ORD> ordList)
		{
			List<AttributeValue> atrList = new List<AttributeValue>();

			foreach (EHS_DATA_ORD ord in ordList)
			{
				atrList.Add(new AttributeValue().CreateNew(ord.XLAT_GROUP, ord.XLAT_CODE, ord.VALUE.HasValue ? (decimal)ord.VALUE : 0));
			}

			return atrList;
		}

        public EHSCalcsCtl LoadMetricHistory(decimal[] plantIDS, DateTime startDate, DateTime endDate, DateIntervalType dateIntervalType, bool loadPlantAccounting)
        {
            this.DateInterval = dateIntervalType;
            this.MetricHst = new List<MetricData>();
            this.PlantHst = new List<PLANT_ACCOUNTING>();
            DateTime fromDate = WebSiteCommon.PeriodFromDate(startDate);
            DateTime toDate = WebSiteCommon.PeriodToDate(endDate);

            try
            {
				this.PlantList = (from pl in this.Entities.PLANT.Include("EHS_PROFILE").Include("EHS_PROFILE.EHS_PROFILE_FACT").Include("EHS_PROFILE.EHS_PROFILE_MEASURE")
                         where (plantIDS.Contains((decimal)pl.PLANT_ID))
                         select pl).ToList();

				this.MetricHst = (from h in this.Entities.EHS_METRIC_HISTORY
								  join m in this.Entities.EHS_MEASURE on h.MEASURE_ID equals m.MEASURE_ID into m_h
								  from m in m_h.DefaultIfEmpty()
								  where (plantIDS.Contains((decimal)h.PLANT_ID)
										  && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(h.PERIOD_YEAR, h.PERIOD_MONTH, 1, 0, 0, 0) <= toDate)
								  select new MetricData
								  {
									  MetricRec = h,
									  Measure = m
								  }).ToList();

                if (loadPlantAccounting)
                {
                    this.LoadPlantAccounting(plantIDS, startDate, endDate);
                   
                    string[] paNames = new string[8];
                    try
                    {
                        paNames[0] = MetricHst.Where(h => h.Measure.PLANT_ACCT_FIELD == "OPER_COST").Select(h => h.Measure.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[1] = MetricHst.Where(h => h.Measure.PLANT_ACCT_FIELD == "REVENUE").Select(h => h.Measure.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[4] = MetricHst.Where(h => h.Measure.PLANT_ACCT_FIELD == "TIME_WORKED").Select(h => h.Measure.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                        paNames[5] = MetricHst.Where(h => h.Measure.PLANT_ACCT_FIELD == "TIME_LOST").Select(h => h.Measure.MEASURE_NAME).FirstOrDefault();  // get equivalent measure names for plant accounting metrics
                    }
                    catch { }

                    foreach (PLANT_ACCOUNTING pac in this.PlantHst)
                    {
                        try
                        {
                            for (int n = 0; n < 10; n++)
                            {
								MetricData data = new MetricData();
								EHS_METRIC_HISTORY rec = new EHS_METRIC_HISTORY();
								EHS_MEASURE measure = new EHS_MEASURE();
								data.MetricRec = rec;
								data.Measure = measure;
                                rec.MEASURE_ID = measure.MEASURE_ID = 1000000m + (decimal)n;
                                measure.MEASURE_CATEGORY = "PROD";
                                rec.PLANT = pac.PLANT;
                                rec.PLANT_ID = pac.PLANT_ID;
                                rec.PERIOD_YEAR = pac.PERIOD_YEAR;
                                rec.PERIOD_MONTH = pac.PERIOD_MONTH;
                                rec.MEASURE_COST = 0;
                                if (!string.IsNullOrEmpty(paNames[4]))
                                    measure.MEASURE_NAME = paNames[4]; 
                                switch (n)
                                {
									case 0: rec.MEASURE_VALUE = pac.OPER_COST.HasValue ? (decimal)pac.OPER_COST : 0; measure.PLANT_ACCT_FIELD = "OPER_COST"; break;
									case 1: rec.MEASURE_VALUE = pac.REVENUE.HasValue ? (decimal)pac.REVENUE : 0; measure.PLANT_ACCT_FIELD = "REVENUE"; break;
									case 2: rec.MEASURE_VALUE = pac.PRODUCTION.HasValue ? (decimal)pac.PRODUCTION : 0; measure.PLANT_ACCT_FIELD = "PRODUCTION"; break;
									case 3: rec.MEASURE_VALUE = pac.THROUGHPUT.HasValue ? (decimal)pac.THROUGHPUT : 0; measure.PLANT_ACCT_FIELD = "THROUGHPUT"; break;
									case 4: rec.MEASURE_VALUE = pac.TIME_WORKED.HasValue ? (decimal)pac.TIME_WORKED : 0; measure.PLANT_ACCT_FIELD = "TIME_WORKED"; break;
									case 5: rec.MEASURE_VALUE = pac.TIME_LOST.HasValue ? (decimal)pac.TIME_LOST : 0; measure.PLANT_ACCT_FIELD = "TIME_LOST"; break;
									case 6: rec.MEASURE_VALUE = pac.TIME_LOST_CASES.HasValue ? (decimal)pac.TIME_LOST_CASES : 0; measure.PLANT_ACCT_FIELD = "TIME_LOST_CASES"; break;
									case 7: rec.MEASURE_VALUE = pac.RECORDED_CASES.HasValue ? (decimal)pac.RECORDED_CASES : 0; measure.PLANT_ACCT_FIELD = "RECORDED_CASES"; break;
                                    case 8: rec.LAST_UPD_DT = pac.APPROVAL_DT.HasValue ? pac.APPROVAL_DT : null; break;
                                    case 9: rec.LAST_UPD_BY = pac.LAST_UPD_BY;  break;
                                }
                                this.MetricHst.Add(data);
                            }
                        }
                        catch (Exception ee)
                        {
                            ;
                        }
                    }
                }
	
				PLANT plant = null;
				EHS_PROFILE_MEASURE pm = null;
				foreach (MetricData md in this.MetricHst)
				{
					md.AtrList = new List<AttributeValue>();
					plant = this.PlantList.Where(p => p.PLANT_ID == md.MetricRec.PLANT_ID).FirstOrDefault();
					pm = plant.EHS_PROFILE.EHS_PROFILE_MEASURE.Where(m => m.MEASURE_ID == md.MetricRec.MEASURE_ID).FirstOrDefault();
					md.AtrList.Add(new AttributeValue().CreateNew("WASTE_CAT", md.Measure.MEASURE_CATEGORY, 1m));
					md.AtrList.Add(new AttributeValue().CreateNew("UN_CODE", pm.UN_CODE, 1m));
					md.AtrList.Add(new AttributeValue().CreateNew("REG_STATUS", pm.REG_STATUS, 1m));
					md.AtrList.Add(new AttributeValue().CreateNew("DISPOSAL", !string.IsNullOrEmpty(pm.UN_CODE) ? pm.UN_CODE.Substring(0,1) : "", 1m));
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

        public List<EHSIncidentData> SelectIncidentList(List<decimal> plantIdList, List<decimal> incidentTypeList, DateTime fromDate, DateTime toDate, string incidentStatus, bool selectAttachments, decimal createID)
        {
            try
            {
				this.IncidentHst = (from i in this.Entities.INCIDENT 
                                    join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
                                    join r in this.Entities.PERSON on i.CREATE_PERSON equals r.PERSON_ID
                                    where ((i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate)
									&& (createID == 0  ||  i.CREATE_PERSON == createID)
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
                    else if (incidentStatus == "C")  // get closed incidents
                        this.IncidentHst = this.IncidentHst.Where(l => l.Status == "C").ToList();
 
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

		public List<EHSIncidentData> SelectPreventativeList(List<decimal> plantIdList, List<decimal> incidentTypeList, List<string>inspectionCatetoryList, List<string> recommendTypeList, DateTime fromDate, DateTime toDate, List<string> statusList, bool selectAttachments, decimal createID)
		{
			try
			{
				this.IncidentHst = (from i in this.Entities.INCIDENT
									join p in this.Entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
									join r in this.Entities.PERSON on i.CREATE_PERSON equals r.PERSON_ID
									where ((i.INCIDENT_DT >= fromDate && i.INCIDENT_DT <= toDate)
									&& (createID == 0 || i.CREATE_PERSON == createID)
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

		public List<EHSAuditData> SelectAuditList(List<decimal> plantIdList, List<decimal> auditTypeList, DateTime fromDate, DateTime toDate, string auditStatus)
		{
			try
			{
				this.AuditHst = (from a in this.Entities.AUDIT
									join p in this.Entities.PLANT on a.DETECT_PLANT_ID equals p.PLANT_ID
									join r in this.Entities.PERSON on a.AUDIT_PERSON equals r.PERSON_ID
									join t in this.Entities.AUDIT_TYPE on a.AUDIT_TYPE_ID equals t.AUDIT_TYPE_ID
									where ((a.AUDIT_DT >= fromDate && a.AUDIT_DT <= toDate)
									&& auditTypeList.Contains((decimal)a.AUDIT_TYPE_ID) && plantIdList.Contains((decimal)a.DETECT_PLANT_ID))
									select new EHSAuditData
									{
										Audit = a,
										Plant = p,
										Person = r,
										AuditType = t
									}).OrderByDescending(l => l.Audit.AUDIT_DT).ToList();

				if (this.AuditHst != null)
				{
					decimal[] ids = this.AuditHst.Select(a => a.Audit.AUDIT_ID).Distinct().ToArray();
					var qaList = (from a in this.Entities.AUDIT_ANSWER
								  where (ids.Contains(a.AUDIT_ID))
								  select a).ToList();
					foreach (EHSAuditData data in this.AuditHst)
					{
						data.EntryList = new List<AUDIT_ANSWER>();
						data.EntryList.AddRange(qaList.Where(l => l.AUDIT_ID == data.Audit.AUDIT_ID).ToList());
						data.DeriveStatus();
						data.DaysElapsed();
						DEPARTMENT dept = new DEPARTMENT();
						if (data.Audit.DEPT_ID != null && data.Audit.DEPT_ID > 0)
						{
							dept = SQMModelMgr.LookupDepartment(this.Entities, (decimal)data.Plant.COMPANY_ID, (decimal)data.Plant.BUS_ORG_ID, (decimal)data.Plant.PLANT_ID, (decimal)data.Audit.DEPT_ID, "", false);
						}
						else
						{
							dept.DEPT_ID = 0;
							dept.DEPT_NAME = "Plant Wide"; // where are we going to store the valid values for language pref??
						}
						if (dept != null)
							data.Department = dept;
					}

					if (auditStatus == "A")  // get open audits
						this.AuditHst = this.AuditHst.Where(l => l.Status == "A").ToList();
					if (auditStatus == "N")  // data incomplete
						this.AuditHst = this.AuditHst.Where(l => l.Status == "N").ToList();
					else if (auditStatus == "C")  // get closed audits
						this.AuditHst = this.AuditHst.Where(l => l.Status == "C" || l.Status == "C8").ToList();

				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return this.AuditHst;
		}

		public List<EHSAuditSchedulerData> SelectAuditSchedulerList(List<decimal> plantIdList, List<decimal> auditTypeList, List<int> daysOfWeekList, string auditSchedulerStatus)
		{
			var entities = new PSsqmEntities();
			try
			{
				this.AuditSchedulerHst = (from a in entities.AUDIT_SCHEDULER
										  join p in entities.PLANT on a.PLANT_ID equals p.PLANT_ID
										  join t in entities.AUDIT_TYPE on a.AUDIT_TYPE_ID equals t.AUDIT_TYPE_ID
										  join j in entities.PRIVGROUP on a.JOBCODE_CD equals j.PRIV_GROUP
										  where (daysOfWeekList.Contains((int)a.DAY_OF_WEEK)
										  && auditTypeList.Contains((decimal)a.AUDIT_TYPE_ID) && plantIdList.Contains((decimal)a.PLANT_ID))
										  select new EHSAuditSchedulerData
										  {
											  AuditScheduler = a,
											  Plant = p,
											  AuditType = t,
											  Privgroup = j
										  }).ToList();

				if (this.AuditSchedulerHst != null)
				{
					if (auditSchedulerStatus == "A")  // get open audits
						this.AuditSchedulerHst = this.AuditSchedulerHst.Where(l => l.AuditScheduler.INACTIVE == false).ToList();
					else if (auditSchedulerStatus == "I")  // data incomplete
						this.AuditSchedulerHst = this.AuditSchedulerHst.Where(l => l.AuditScheduler.INACTIVE == true).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return this.AuditSchedulerHst;
		}

		public List<EHSAuditData> SelectAuditExceptionList(List<decimal> plantIdList, List<decimal> auditTypeList, DateTime fromDate, DateTime toDate, bool includeWithTasks)
		{

			var auditList = new List<EHSAuditData>();
			EHSAuditQuestionType QuestionType = new EHSAuditQuestionType();
			bool answerIsNegative = false;
			bool tasksAssigned = false;
			try
			{
				this.AuditHst = (from a in this.Entities.AUDIT
								 join p in this.Entities.PLANT on a.DETECT_PLANT_ID equals p.PLANT_ID
								 join r in this.Entities.PERSON on a.AUDIT_PERSON equals r.PERSON_ID
								 join t in this.Entities.AUDIT_TYPE on a.AUDIT_TYPE_ID equals t.AUDIT_TYPE_ID
								 where ((a.AUDIT_DT >= fromDate && a.AUDIT_DT <= toDate)
								 && auditTypeList.Contains((decimal)a.AUDIT_TYPE_ID) && plantIdList.Contains((decimal)a.DETECT_PLANT_ID))
								 select new EHSAuditData
								 {
									 Audit = a,
									 Plant = p,
									 Person = r,
									 AuditType = t
								 }).OrderByDescending(l => l.Audit.AUDIT_DT).ToList();

				if (this.AuditHst != null)
				{
					List<EHSMetaData> xlats = EHSMetaDataMgr.SelectMetaDataList("AQ");  // get localized answer texts

					decimal[] ids = this.AuditHst.Select(a => a.Audit.AUDIT_ID).Distinct().ToArray();
					var qaList = (from a in this.Entities.AUDIT_ANSWER
								  where (ids.Contains(a.AUDIT_ID))
								  select a).ToList();
					foreach (EHSAuditData data in this.AuditHst)
					{
						data.EntryList = new List<AUDIT_ANSWER>();
						data.EntryList.AddRange(qaList.Where(l => l.AUDIT_ID == data.Audit.AUDIT_ID).ToList());
						data.DeriveStatus();
						data.DaysElapsed();
					}

					// get only closed audits
					// AW20151011 - we want to include all adverse answers, because audits may not have been completed yet, but we want to see them
					//		** do we want to only show the ones where all the questions have been completed??
					//this.AuditHst = this.AuditHst.Where(l => l.Status == "C" || l.Status == "C8").ToList();

					// now we need to build a list of only the Audits that have negative responses
					foreach (EHSAuditData data in this.AuditHst)
					{
						answerIsNegative = false;
						tasksAssigned = false;
						foreach (AUDIT_ANSWER auditAnswer in data.Audit.AUDIT_ANSWER)
						{
							// for each answer in the audit get the answer value
							string answer = (auditAnswer.ANSWER_VALUE == null) ? "" : auditAnswer.ANSWER_VALUE;
							if (answer.Length > 0)
							{
								// get the original question info for the answer
								AUDIT_QUESTION question = (from q in this.Entities.AUDIT_QUESTION
														   where q.AUDIT_QUESTION_ID == auditAnswer.AUDIT_QUESTION_ID
														   select q).FirstOrDefault();
								if (question != null)
								{
									// if this is a question type that we care about, get the possible choices for an answer
									QuestionType = (EHSAuditQuestionType)question.AUDIT_QUESTION_TYPE_ID;
									if (QuestionType == EHSAuditQuestionType.RadioPercentage || QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft
										|| QuestionType == EHSAuditQuestionType.Radio || QuestionType == EHSAuditQuestionType.RadioCommentLeft)
									{
										List<EHSAuditAnswerChoice> choices = (from qc in this.Entities.AUDIT_QUESTION_CHOICE
																			  where qc.AUDIT_QUESTION_ID == question.AUDIT_QUESTION_ID
																			  orderby qc.SORT_ORDER
																			  select new EHSAuditAnswerChoice
																			  {
																				  Value = qc.QUESTION_CHOICE_VALUE,
																				  IsCategoryHeading = qc.IS_CATEGORY_HEADING,
																				  ChoiceWeight = qc.CHOICE_WEIGHT,
																				  ChoicePositive = qc.CHOICE_POSITIVE
																			  }).ToList();
										if (choices.Count > 0)
										{
											// set the flag if there are any negative answers
											foreach (EHSAuditAnswerChoice choice in choices)
											{
												// AW 2016 try to convert the value with the AQ XLAT.  If it isn't there, then just use the default text for now.  We will add true audit language during the Audit Maint project
												try
												{
													choice.Text = xlats.Where(x => x.Value == choice.Value).FirstOrDefault().TextLong;
												}
												catch
												{
													choice.Text = choice.Value;
												}
												if (choice.Value.Equals(answer) && !choice.ChoicePositive)
													answerIsNegative = true;
											}
										}
									}
								}
							}
							// see if there are any tasks assigned to the answer
							if (includeWithTasks)
							{
								List<decimal> taskIds = (from a in this.Entities.TASK_STATUS
														 where a.RECORD_TYPE == 50 && a.RECORD_ID == data.Audit.AUDIT_ID && a.RECORD_SUBID == auditAnswer.AUDIT_QUESTION_ID
														 select a.TASK_ID).ToList();
								if (taskIds.Count > 0)
									tasksAssigned = true;
							}
						}
						// add the audit to the list only if there are negative answers
						// also include if there are tasks assigned
						if (answerIsNegative || tasksAssigned)
						{
							auditList.Add(data);
						}
					}
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			this.AuditHst = auditList;

			return this.AuditHst;
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
                    DateTime now = DateTime.UtcNow;

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
			List<WebSiteCommon.DatePeriod> pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
           
            switch (seriesOrder)
            {
                case EHSCalcsCtl.SeriesOrder.PeriodMeasure:
                    foreach (decimal measureID in measureIDS)
                    {
                        seriesName = "";
                        EHS_MEASURE measure = this.InputsList.Where(l => l.PRMR_ID == measureIDS[0]).Select(l => l.EHS_PROFILE_MEASURE.EHS_MEASURE).FirstOrDefault();
                        if (measure != null  &&  measure.MEASURE_NAME != null)
							seriesName = MeasureLabel(measure);
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
								series.AddItem(item++, MeasureLabel(measure), this.Results);
                            }
                        }
                        this.Results.metricSeries.Add(series);
                        break;

                    case EHSCalcsCtl.SeriesOrder.PeriodMeasure:
                    case EHSCalcsCtl.SeriesOrder.PeriodMeasureYOY:
						pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
                        foreach (decimal measureID in measureIDS)
                        {
                            EHS_MEASURE measure = GetMeasure(measureID);
							series = new GaugeSeries().CreateNew(1, MeasureLabel(measure), "");
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
							pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate.AddYears(-1), DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
                            foreach (decimal measureID in measureIDS)
                            {
                                EHS_MEASURE measure = GetMeasure(measureID);
								series = new GaugeSeries().CreateNew(1, MeasureLabel(measure), "");
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
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
                        series = new GaugeSeries().CreateNew(1, "", "");
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
							pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(-1), toDate.AddYears(-1), DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
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
					case EHSCalcsCtl.SeriesOrder.PeriodMeasurePlantTotal:
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
								pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
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

						if (seriesOrder == SeriesOrder.PeriodMeasurePlantTotal)
						{
							GaugeSeries totalsSeries = new GaugeSeries().CreateNew(1, "Total", "");
							totalsSeries.SeriesType = 9; // totals series - need enum for this ...
							pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, this.DateSpanType, "", this.Options.ToUpper().Contains("MAX12") ? 12 : 0);
							ns = 0;
							foreach (WebSiteCommon.DatePeriod pd in pdList)
							{
								if (EHMetric(plantIDS, measureIDS, pd.FromDate, pd.ToDate))
								{
									totalsSeries.ItemList.Add(new GaugeSeriesItem().CreateNew(1, ++ns, SQMBasePage.FormatDate(pd.FromDate, "yyyy/MM", false), this.Results));
								}
							}
							this.Results.metricSeries.Insert(0, totalsSeries);
						}

                        break;
                    case EHSCalcsCtl.SeriesOrder.MeasurePlant:
					case EHSCalcsCtl.SeriesOrder.MeasurePlantTotal:
                        foreach (decimal measureID in measureIDS)
                        {
                            EHS_MEASURE measure = GetMeasure(measureID);
							series = new GaugeSeries().CreateNew(1, MeasureLabel(measure), "");
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

						// add totals logic here

                        break;

                    case EHSCalcsCtl.SeriesOrder.YearPlant:
                    case EHSCalcsCtl.SeriesOrder.YearPlantAvg:
                        result0 = 0;
                        ns = 0;
                        if (this.DateSpanType == DateSpanOption.FYYearOverYear)  // don't deduct start year because it was already done upstream
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.FYyear, this.DateSpanType, "");
                        else
                            pdList = WebSiteCommon.CalcDatePeriods(fromDate.AddYears(0), toDate, DateIntervalType.year, this.DateSpanType, "");

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
						case "ratio":
						case "ratioPct":
							// ratio of two measures (nnn|nnn) e.g. pct of audits complete, jsa's / jsa's required,  observations / people
							decimal[] denominatorIDS = GetMetricsByScope(this.SubScope); // this.SubScope.Split(',').Select(x => decimal.Parse(x)).ToArray();
							if ((temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, denominatorIDS).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum()) != 0)
							{
								value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum() / temp;
								if (this.Calculation == "ratioPct")
									value = value * 100;
							}
							else
							{
								value = 0;
							}
							break;
                        case "cost":
							value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_COST).ToList().Sum();
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
                                EHSModel.GHGResultList ghgTable =  CalcGHG(fromDate, toDate, plantID, measureArray, 0);
                                ((EHSModel.GHGResultList)this.Results.ResultObj).AddResultRange(ghgTable);
								decimal normValue = 1;
								if (this.Stat == SStat.normRev)
									normValue = this.InitCalc().Calc.Select(fromDate, toDate, new decimal[1] {plantID}, new decimal[1] { 1000001 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
                                value += normValue == 0 ? ghgTable.GHGTotal : ghgTable.GHGTotal / normValue;
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
								this.Results.FactorText = MeasureLabel(factMeasure);
                                if (Calculation == "normCost")
                                {
									sumVal = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_COST).ToList().Sum();
                                }
                                else
                                {
									sumVal = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
                                }
								decimal sumFact = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { factMeasure.MEASURE_ID }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
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
						case "eReff":
							decimal sumE = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							decimal sumPCost = this.Calculation == "eReff" ? 0 : this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000000 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							decimal sumPRevenue = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000001 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
                            if (sumE > 0 && (sumPRevenue - sumPCost != 0))
                            {
                                value = sumE / (sumPRevenue - sumPCost);
                            }
                            else
                                valid = false;
                            break;
						case "eReffCost":
							decimal sumECost = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => (decimal)l.MetricRec.MEASURE_COST).ToList().Sum();
							decimal sumRRevenue = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000001 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							if (sumRRevenue != 0)
							{
								value = sumECost / (sumRRevenue);
							}
							else
								valid = false;
							break;
                        case "eSeff":
							decimal sumESpend = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_COST).ToList().Sum();
							sumPCost = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000000 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							sumPRevenue = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000001 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							if (sumESpend > 0 && (sumPRevenue - sumPCost != 0))
                            {
                                value = sumESpend / (sumPRevenue - sumPCost);
                            }
                            else
                                valid = false;
                            break;
                        default:   // undefined calculation - just return the sum of measure values
							value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
                            break;
                    }
                }
                else
                {
                    switch (this.MetricScope)
                    {
                        // rcr, ltcr, ltsr should return zero and valid result if no occurences found
                        case "rcr":
							if (this.Perspective == "XD")
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S20004")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();		// recorded cases
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S60002")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							}
							else
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000007 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();		// recorded cases
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							}
							if (temp2 != 0)
								value = (temp * temp3) / temp2;
							break;
                        case "ltcr":
							if (this.Perspective == "XD")
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);     // hours basis
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S20005")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();  // lost time cases
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S60002")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();  // hrs worked
							}
							else
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);     // hours basis
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000006 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();  // lost time cases
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();  // hrs worked
							}
							if (temp2 != 0)
								value = (temp * temp3) / temp2;
                            break;
                        case "ltsr":
							if (this.Perspective == "XD")
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S60001")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();		// time lost
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, GetMetricsByMeasureCode("S60002")).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							}
							else
							{
								temp3 = ProRateWorkHours(fromDate, toDate, plantArray);
								temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000005 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();		// time lost
								temp2 = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, new decimal[1] { 1000004 }).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
							}
							if (temp2 != 0)
								value = (temp * temp3) / temp2;
                            break;
                        case "cost":
                            //value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MEASURE_COST).ToList().Sum();
                            break;
						case "DISPOSAL":
						case "REG_STATUS":
						case "WASTE_CAT":
						case "INJURY_TYPE":
						case "INJURY_PART":
						case "INJURY_TENURE":
						case "INJURY_CAUSE":
						case "INJURY_DAYS_TO_CLOSE":
							// in this case the measure id will be an iteration thru the attribute xlat elements 
							string attributeValue = this.Calc.XLATSeries.ElementAt((int)measureArray[0]).XLAT_CODE;
                            if (this.Stat == SStat.cost)
								value = this.InitCalc().Calc.SelectByAttribute(fromDate, toDate, plantArray, this.MetricScope, attributeValue).Select(l => (decimal)l.MetricRec.MEASURE_COST).ToList().Sum();
                            else
                            {
								value = this.InitCalc().Calc.SelectByAttribute(fromDate, toDate, plantArray, this.MetricScope, attributeValue).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
								if (this.Stat == SStat.pct && value == 0)
								{
									valid = false;
								}
                            }
                            break;
                        default:
							if (this.Stat == SStat.ratio  ||  this.Stat == SStat.ratioPct)
							{
								// ratio of two measures (nnn|nnn) e.g. pct of audits complete, jsa's / jsa's required,  observations / people
								decimal[] numeratorIDS = GetMetricsByScope(this.MetricScope); // this.MetricScope.Split(',').Select(x => decimal.Parse(x)).ToArray();
								decimal[] denominatorIDS = GetMetricsByScope(this.SubScope); // this.SubScope.Split(',').Select(x => decimal.Parse(x)).ToArray();
								if ((temp = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, denominatorIDS).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum()) != 0)
								{
									value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, numeratorIDS).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum() / temp;
									if (this.Stat == SStat.ratioPct)
										value = value * 100;
								}
								else
								{
									value = 0;
								}
							}
							else if (this.Stat == SStat.cost)
								value = (decimal)this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_COST).ToList().Sum();
							else
								value = this.InitCalc().Calc.Select(fromDate, toDate, plantArray, measureArray).Select(l => l.MetricRec.MEASURE_VALUE).ToList().Sum();
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
			// get distinct list of measures that have the specified attribute
            decimal[] measureArray = null;
  
            try
            {
				if (attribute == "DISPOSAL")
				{
					measureArray = this.MetricHst.Where(l => plantArray.Contains(l.MetricRec.PLANT_ID) && l.AtrList.Any(a => a.Group == attribute && a.Key.StartsWith(attributeValue))).Select(l => l.Measure.MEASURE_ID).Distinct().ToArray();
				}
				else
				{
					measureArray = this.MetricHst.Where(l => plantArray.Contains(l.MetricRec.PLANT_ID) && l.AtrList.Any(a => a.Group == attribute && a.Key == attributeValue)).Select(l => l.Measure.MEASURE_ID).Distinct().ToArray();
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

		public EHSModel.GHGResultList CalcGHG(DateTime fromDate, DateTime toDate, decimal plantID, decimal[] measureArray, decimal normMeasure)
		{
			PLANT plant = this.GetPlant(plantID);
			EHSModel.Emissions emc = new EHSModel.Emissions().Initialize(plant.LOCATION_CODE, plant.COMP_INT_ID);
			string[] ghgCodes = this.SubScope.Split(',');
			List<WebSiteCommon.DatePeriod> pdList = this.DateInterval == DateIntervalType.span ? WebSiteCommon.CalcDatePeriods(fromDate, toDate, this.DateInterval, this.DateSpanType, "") : WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.year, this.DateSpanType, "");

			EHSModel.GHGResultList ghgTable = new EHSModel.GHGResultList().CreateNew(fromDate, toDate);

			foreach (decimal measID in measureArray)
			{
				EHS_MEASURE meas = this.MetricHst.Where(l => l.Measure.MEASURE_ID == measID).Select(l => l.Measure).FirstOrDefault();
				if (meas != null && !string.IsNullOrEmpty(meas.EFM_TYPE))
				{
					foreach (WebSiteCommon.DatePeriod pd in pdList)
					{
						decimal metricQty = this.InitCalc().Calc.Select(pd.FromDate, pd.ToDate, new decimal[1] { plantID }, new decimal[1] { measID }).Select(l => l.MetricRec.MEASURE_VALUE).Sum();
						MetricData hist1 = this.Calc.Metrics.FirstOrDefault();

						if (hist1 != null)
						{
							int gasSeq = 0;
							foreach (string ghgCode in ghgCodes)
							{
								++gasSeq;
								if (ghgCode.ToUpper() == "CO2" || (meas.EFM_TYPE != "P" && meas.EFM_TYPE != "HW"))
								{
									decimal ghg = emc.LookupGHG(meas.EFM_TYPE, pd.FromDate.Year, ghgCode);
									decimal gwp = emc.GetGWP(ghgCode, this.Calculation);
									decimal ghgQty = metricQty * ghg * gwp;
									ghgTable.SumTotal(ghgQty);
									decimal ghgUOM = 17; // calculated in kg
									try
									{
										EHSModel.GHGResult ghgRrec = new EHSModel.GHGResult().CreateNew(plant, meas, 0, hist1.MetricRec.INPUT_UOM_ID.HasValue ? (decimal)hist1.MetricRec.INPUT_UOM_ID : 0,
											metricQty, hist1.MetricRec.UOM_ID, meas.EFM_TYPE, gasSeq, ghg, (metricQty * ghg), ghgCode, gwp, ghgQty, ghgUOM);
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
                            this.Results.Result = (decimal)Math.Truncate(DateTime.UtcNow.Subtract(incidentList.Select(l => l.Incident.INCIDENT_DT).Last()).TotalDays);
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
                    DateTime now = DateTime.UtcNow;
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
                            result.Result = (decimal)Math.Truncate(DateTime.UtcNow.Subtract(incident.INCIDENT_DT).TotalDays);
                            result.Result = Math.Max(0, result.Result - 1);
                            result.ValidResult = true;
                        }
                        else
                        {
                            if (plant.PLANT_START_DT.HasValue)
                            {
                                result.Result = (decimal)Math.Truncate(DateTime.UtcNow.Subtract((DateTime)plant.PLANT_START_DT).TotalDays);
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