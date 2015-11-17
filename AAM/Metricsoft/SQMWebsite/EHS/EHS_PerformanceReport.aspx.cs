using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website.EHS
{
	public partial class EHS_PerformanceReport : SQMBasePage
	{
		[Flags]
		enum DataToUse
		{
			None = 0,
			TRIR = 0x0001,
			FrequencyRate = 0x0002,
			SeverityRate = 0x0004,
			Incidents = 0x0008,
			Frequency = 0x0010,
			Restricted = 0x0020,
			Severity = 0x0040,
			FirstAid = 0x0080,
			Leadership = 0x0100,
			JSAs = 0x0200,
			SafetyTraining = 0x0400,
			Fatalities = 0x0800,
			NearMisses = 0x1000,
			Ordinals = 0x2000,

			Pyramid = Incidents | Frequency | FirstAid | Leadership | JSAs | SafetyTraining | Fatalities | NearMisses,
			BalancedScorecard = TRIR | FrequencyRate | SeverityRate | Incidents | Frequency | Severity,
			Metrics = TRIR | FrequencyRate | SeverityRate | Incidents | Frequency | Restricted | Severity | FirstAid | Leadership | JSAs | SafetyTraining | Ordinals
		}

		class Data
		{
			public DataToUse DataToUse { get; set; }
			public string Month { get; set; }
			public decimal TRIR { get; set; }
			public decimal FrequencyRate { get; set; }
			public decimal SeverityRate { get; set; }
			public decimal ManHours { get; set; }
			public decimal Incidents { get; set; }
			public decimal Frequency { get; set; }
			public decimal Restricted { get; set; }
			public decimal Severity { get; set; }
			public decimal FirstAid { get; set; }
			public decimal Leadership { get; set; }
			public decimal JSAs { get; set; }
			public decimal SafetyTraining { get; set; }
			public decimal Fatalities { get; set; }
			public decimal NearMisses { get; set; }
			// Ordinals
			public Dictionary<string, decimal> OrdinalType { get; set; }
			public Dictionary<string, decimal> OrdinalBodyPart { get; set; }
			public Dictionary<string, decimal> OrdinalRootCause { get; set; }
			public Dictionary<string, decimal> OrdinalTenure { get; set; }
			public Dictionary<string, decimal> OrdinalDaysToClose { get; set; }

			public Data()
			{
				this.OrdinalType = new Dictionary<string, decimal>();
				this.OrdinalBodyPart = new Dictionary<string, decimal>();
				this.OrdinalRootCause = new Dictionary<string, decimal>();
				this.OrdinalTenure = new Dictionary<string, decimal>();
				this.OrdinalDaysToClose = new Dictionary<string, decimal>();
			}

			public static Data operator +(Data a, Data b)
			{
				var c = new Data()
				{
					DataToUse = a.DataToUse
				};
				if (a.DataToUse.Has(DataToUse.TRIR) || a.DataToUse.Has(DataToUse.FrequencyRate) || a.DataToUse.Has(DataToUse.SeverityRate))
					c.ManHours = a.ManHours + b.ManHours;
				if (a.DataToUse.Has(DataToUse.Incidents))
					c.Incidents = a.Incidents + b.Incidents;
				if (a.DataToUse.Has(DataToUse.Frequency))
					c.Frequency = a.Frequency + b.Frequency;
				if (a.DataToUse.Has(DataToUse.Restricted))
					c.Restricted = a.Restricted + b.Restricted;
				if (a.DataToUse.Has(DataToUse.Severity))
					c.Severity = a.Severity + b.Severity;
				if (a.DataToUse.Has(DataToUse.FirstAid))
					c.FirstAid = a.FirstAid + b.FirstAid;
				if (a.DataToUse.Has(DataToUse.Leadership))
					c.Leadership = a.Leadership + b.Leadership;
				if (a.DataToUse.Has(DataToUse.JSAs))
					c.JSAs = a.JSAs + b.JSAs;
				if (a.DataToUse.Has(DataToUse.SafetyTraining))
					c.SafetyTraining = a.SafetyTraining + b.SafetyTraining;
				if (a.DataToUse.Has(DataToUse.Fatalities))
					c.Fatalities = a.Fatalities + b.Fatalities;
				if (a.DataToUse.Has(DataToUse.NearMisses))
					c.NearMisses = a.NearMisses + b.NearMisses;
				if (a.DataToUse.Has(DataToUse.Ordinals))
				{
					c.OrdinalType = a.OrdinalType.Keys.Concat(b.OrdinalType.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalType.ContainsKey(key) ? a.OrdinalType[key] : 0) + (b.OrdinalType.ContainsKey(key) ? b.OrdinalType[key] : 0)
					}).ToDictionary(x => x.key, x => x.value);
					c.OrdinalBodyPart = a.OrdinalBodyPart.Keys.Concat(b.OrdinalBodyPart.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalBodyPart.ContainsKey(key) ? a.OrdinalBodyPart[key] : 0) + (b.OrdinalBodyPart.ContainsKey(key) ? b.OrdinalBodyPart[key] : 0)
					}).ToDictionary(x => x.key, x => x.value);
					c.OrdinalRootCause = a.OrdinalRootCause.Keys.Concat(b.OrdinalRootCause.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalRootCause.ContainsKey(key) ? a.OrdinalRootCause[key] : 0) + (b.OrdinalRootCause.ContainsKey(key) ? b.OrdinalRootCause[key] : 0)
					}).ToDictionary(x => x.key, x => x.value);
					c.OrdinalTenure = a.OrdinalTenure.Keys.Concat(b.OrdinalTenure.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalTenure.ContainsKey(key) ? a.OrdinalTenure[key] : 0) + (b.OrdinalTenure.ContainsKey(key) ? b.OrdinalTenure[key] : 0)
					}).ToDictionary(x => x.key, x => x.value);
					c.OrdinalDaysToClose = a.OrdinalDaysToClose.Keys.Concat(b.OrdinalDaysToClose.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalDaysToClose.ContainsKey(key) ? a.OrdinalDaysToClose[key] : 0) + (b.OrdinalDaysToClose.ContainsKey(key) ? b.OrdinalDaysToClose[key] : 0)
					}).ToDictionary(x => x.key, x => x.value);
				}
				return c;
			}
		}

		static GaugeDefinition gaugeDef = new GaugeDefinition()
		{
			Height = 500,
			Width = 1500,
			DisplayLegend = true,
			LegendPosition = ChartLegendPosition.Right,
			LegendBackgroundColor = Color.White
		};
		static GaugeDefinition smallGaugeDef = new GaugeDefinition()
		{
			Height = 500,
			Width = 740,
			DisplayLegend = true,
			LegendPosition = ChartLegendPosition.Right,
			LegendBackgroundColor = Color.White
		};

		static void SetScale(GaugeDefinition gd, List<GaugeSeries> series)
		{
			decimal min = 0, max = 0;
			foreach (var ser in series)
			{
				min = Math.Min(min, ser.ItemList.Min(i => i.YValue));
				max = Math.Max(max, ser.ItemList.Max(i => i.YValue));
			}
			gd.ScaleMin = min == 0 && max == 0 ? 0 : (decimal?)null;
			gd.ScaleMax = min == 0 && max == 0 ? 1 : 0;
		}

		/// <summary>
		/// Gets all the types for an ordinal from the XLAT table.
		/// </summary>
		/// <param name="entities">The entities to get the table from.</param>
		/// <param name="groupName">The group name to get the types for.</param>
		/// <returns>An IEnumerable of an anonymous type as a dynamic.</returns>
		static dynamic GetOrdinalTypes(PSsqmEntities entities, string groupName)
		{
			return from x in entities.XLAT
				   where x.XLAT_GROUP == groupName && x.XLAT_LANGUAGE == "en" && x.STATUS == "A"
				   select new { x.XLAT_GROUP, x.XLAT_CODE, x.DESCRIPTION };
		}

		static Dictionary<string, decimal> GetOrdinalData(IEnumerable<EHS_DATA_ORD> allOrdData, dynamic types)
		{
			var ret = new Dictionary<string, decimal>();
			foreach (var t in types)
			{
				var group = t.XLAT_GROUP as string;
				var code = t.XLAT_CODE as string;
				var sum = allOrdData.Where(o => o.XLAT_GROUP == group && o.XLAT_CODE == code).Sum(o => o.VALUE) ?? 0;
				ret.Add(t.DESCRIPTION as string, sum);
			}
			return ret;
		}

		static dynamic PullData(PSsqmEntities entities, string plantID, decimal companyID, int year, DataToUse dataToUse)
		{
			var startOf2YearsAgo = new DateTime(year - 2, 1, 1);
			var startOfNextYear = new DateTime(year + 1, 1, 1);

			decimal plantID_dec = plantID.StartsWith("BU") ? decimal.Parse(plantID.Substring(2)) : decimal.Parse(plantID);
			var plantIDs = plantID.StartsWith("BU") ? SQMModelMgr.SelectPlantList(entities, companyID, plantID_dec).Select(p => p.PLANT_ID).ToList() : new List<decimal>();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Where(d => (plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(d.PLANT_ID) : d.PLANT_ID == plantID_dec)) &&
				EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();
			var allOrdData = entities.EHS_DATA_ORD.Where(o => (plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(o.EHS_DATA.PLANT_ID) : o.EHS_DATA.PLANT_ID == plantID_dec)) &&
				EntityFunctions.TruncateTime(o.EHS_DATA.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(o.EHS_DATA.DATE) < startOfNextYear.Date).ToList();

			var data = new List<Data>();
			Data YTD = null;
			Data previousYTD = null;

			var incidentRateSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 2, year), "")
			{
				DisplayLabels = true
			};
			var frequencyRateSeries = new List<GaugeSeries>();
			var severityRateSeries = new List<GaugeSeries>();
			var jsasSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 1, year), "")
			{
				DisplayLabels = true
			};
			var safetyTrainingHoursSeries = new GaugeSeries(0, "Total Safety Training Hours", "")
			{
				DisplayLabels = true
			};

			// Ordinal types
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			for (int y = year - 2; y <= year; ++y)
			{
				YTD = new Data()
				{
					DataToUse = dataToUse
				};
				var frequencyRateSeriesYear = new GaugeSeries(y - year - 2, y.ToString(), "");
				var severityRateSeriesYear = new GaugeSeries(y - year - 2, y.ToString(), "");

				for (int i = 1; i < 13; ++i)
				{
					var startOfMonth = new DateTime(y, i, 1);
					var startOfNextMonth = startOfMonth.AddMonths(1);

					var allMonthData = allData.Where(d => d.DATE.Date >= startOfMonth.Date && d.DATE.Date < startOfNextMonth.Date);
					var monthOrdData = allOrdData.Where(o => o.EHS_DATA.DATE.Date >= startOfMonth.Date && o.EHS_DATA.DATE.Date < startOfNextMonth.Date);

					var manHours = (dataToUse.Has(DataToUse.TRIR) || dataToUse.Has(DataToUse.FrequencyRate) || dataToUse.Has(DataToUse.SeverityRate)) &&
						(y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true)) ?
						allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var incidents = (dataToUse.Has(DataToUse.TRIR) || dataToUse.Has(DataToUse.Incidents)) &&
						(y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true)) ?
						allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var frequency = (dataToUse.Has(DataToUse.FrequencyRate) || dataToUse.Has(DataToUse.Frequency)) && y > year - 2 ?
						allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20005" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var severity = (dataToUse.Has(DataToUse.SeverityRate) || dataToUse.Has(DataToUse.Severity)) && y > year - 2 ?
						allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;

					var monthData = new Data()
					{
						DataToUse = dataToUse,
						Month = startOfMonth.ToString("MMMM"),
					};
					if (dataToUse.Has(DataToUse.TRIR))
						monthData.TRIR = manHours == 0 ? 0 : incidents * 200000 / manHours;
					if (dataToUse.Has(DataToUse.FrequencyRate))
						monthData.FrequencyRate = manHours == 0 ? 0 : frequency * 200000 / manHours;
					if (dataToUse.Has(DataToUse.SeverityRate))
						monthData.SeverityRate = manHours == 0 ? 0 : severity * 200000 / manHours;
					if (dataToUse.Has(DataToUse.TRIR) || dataToUse.Has(DataToUse.FrequencyRate) || dataToUse.Has(DataToUse.SeverityRate))
						monthData.ManHours = manHours;
					if (dataToUse.Has(DataToUse.Incidents))
						monthData.Incidents = incidents;
					if (dataToUse.Has(DataToUse.Frequency))
						monthData.Frequency = frequency;
					if (dataToUse.Has(DataToUse.Restricted))
						monthData.Restricted = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.Severity))
						monthData.Severity = severity;
					if (dataToUse.Has(DataToUse.FirstAid))
						monthData.FirstAid = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.Leadership))
						monthData.Leadership = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S30002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.JSAs))
						monthData.JSAs = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S40003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.SafetyTraining))
						monthData.SafetyTraining = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S50001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.Fatalities))
						monthData.Fatalities = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20006" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.NearMisses))
						monthData.NearMisses = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.Ordinals))
					{
						monthData.OrdinalType = y == year ? GetOrdinalData(monthOrdData, types) : new Dictionary<string, decimal>();
						monthData.OrdinalBodyPart = y == year ? GetOrdinalData(monthOrdData, bodyParts) : new Dictionary<string, decimal>();
						monthData.OrdinalRootCause = y == year ? GetOrdinalData(monthOrdData, rootCauses) : new Dictionary<string, decimal>();
						monthData.OrdinalTenure = y == year ? GetOrdinalData(monthOrdData, tenures) : new Dictionary<string, decimal>();
						monthData.OrdinalDaysToClose = y == year ? GetOrdinalData(monthOrdData, daysToCloses) : new Dictionary<string, decimal>();
					}

					if (y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true))
						incidentRateSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, monthData.TRIR, monthData.Month));
					if (y > year - 2)
					{
						frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.FrequencyRate, monthData.Month));
						severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.SeverityRate, monthData.Month));
					}
					if (y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true))
						jsasSeries.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.Leadership + monthData.JSAs, monthData.Month));
					if (y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true))
						safetyTrainingHoursSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, monthData.SafetyTraining, monthData.Month));

					if (y == year)
						data.Add(monthData);
					YTD += monthData;
				}

				if (y == year - 1)
					previousYTD = YTD;

				if (y > year - 2)
				{
					frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours, "YTD"));
					severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Severity * 200000 / YTD.ManHours, "YTD"));

					frequencyRateSeries.Add(frequencyRateSeriesYear);
					severityRateSeries.Add(severityRateSeriesYear);
				}
			}

			YTD.Month = "YTD";
			if (dataToUse.Has(DataToUse.TRIR))
				YTD.TRIR = YTD.ManHours == 0 ? 0 : YTD.Incidents * 200000 / YTD.ManHours;
			if (dataToUse.Has(DataToUse.FrequencyRate))
				YTD.FrequencyRate = YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours;
			if (dataToUse.Has(DataToUse.SeverityRate))
				YTD.SeverityRate = YTD.ManHours == 0 ? 0 : YTD.Severity * 200000 / YTD.ManHours;

			data.Add(YTD);

			var incidentRateTrendSeries = new GaugeSeries(0, "TRIR Trend (6 Month Rolling Avg.)", "");
			for (int i = 0; i < 5; ++i)
				incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, 0, ""));
			for (int i = 5; i < incidentRateSeries.ItemList.Count; ++i)
				incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0,
					Enumerable.Range(i - 5, 6).Select(j => incidentRateSeries.ItemList[j]).Sum(v => v.YValue) / 6m, incidentRateSeries.ItemList[i].Text));
			var jsasTrendSeries = new GaugeSeries(0, "Leading Trend", "");
			jsasTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, 0, ""));
			for (int i = 1; i < jsasSeries.ItemList.Count; ++i)
				jsasTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, Enumerable.Range(i - 1, 2).Select(j => jsasSeries.ItemList[j]).Sum(v => v.YValue) / 2m, jsasSeries.ItemList[i].Text));

			var incidentRateTarget = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "TRIR" &&
				(plantID_dec == -1 ? t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID :
				(plantID.StartsWith("BU") ? t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == plantID_dec : t.PLANT_ID.HasValue && t.PLANT_ID == plantID_dec)));
			var frequencyRateTarget = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "FREQUENCY" &&
				(plantID_dec == -1 ? t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID :
				(plantID.StartsWith("BU") ? t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == plantID_dec : t.PLANT_ID.HasValue && t.PLANT_ID == plantID_dec)));
			var severityRateTarget = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "SEVERITY" &&
				(plantID_dec == -1 ? t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID :
				(plantID.StartsWith("BU") ? t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == plantID_dec : t.PLANT_ID.HasValue && t.PLANT_ID == plantID_dec)));
			var jsasTarget = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "AUDITS" &&
				(plantID_dec == -1 ? t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID :
				(plantID.StartsWith("BU") ? t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == plantID_dec : t.PLANT_ID.HasValue && t.PLANT_ID == plantID_dec)));

			var targetData = new Data()
			{
				Month = "Target",
				TRIR = incidentRateTarget != null ? incidentRateTarget.TARGET_VALUE : 0,
				FrequencyRate = frequencyRateTarget != null ? frequencyRateTarget.TARGET_VALUE : 0,
				SeverityRate = severityRateTarget != null ? severityRateTarget.TARGET_VALUE : 0
			};
			data.Add(targetData);

			return new
			{
				data,
				previousYTD,
				incidentRateSeries,
				incidentRateTrendSeries,
				incidentRateTarget = targetData.TRIR,
				frequencyRateSeries,
				frequencyRateTarget = targetData.FrequencyRate,
				severityRateSeries,
				severityRateTarget = targetData.SeverityRate,
				ordinalTypeSeries = YTD.OrdinalType.Select(type => new GaugeSeriesItem(0, 0, 0, type.Value, type.Key)
				{
					Exploded = false
				}).ToList(),
				ordinalBodyPartSeries = YTD.OrdinalBodyPart.Select(type => new GaugeSeriesItem(0, 0, 0, type.Value, type.Key)
				{
					Exploded = false
				}).ToList(),
				ordinalRootCauseSeries = YTD.OrdinalRootCause.Select(type => new GaugeSeriesItem(0, 0, 0, type.Value, type.Key)
				{
					Exploded = false
				}).ToList(),
				ordinalTenureSeries = YTD.OrdinalTenure.Select(type => new GaugeSeriesItem(0, 0, 0, type.Value, type.Key)
				{
					Exploded = false
				}).ToList(),
				ordinalDaysToCloseSeries = YTD.OrdinalDaysToClose.Select(type => new GaugeSeriesItem(0, 0, 0, type.Value, type.Key)
				{
					Exploded = false
				}).ToList(),
				jsasSeries,
				jsasTrendSeries,
				jsasTarget = jsasTarget != null ? jsasTarget.TARGET_VALUE : 0,
				safetyTrainingHoursSeries
			};
		}

		static List<dynamic> PullTRIRByBusinessUnit(PSsqmEntities entities, decimal companyID, int year)
		{
			var startOf2YearsAgo = new DateTime(year - 2, 1, 1);
			var startOfNextYear = new DateTime(year + 1, 1, 1);

			var plantIDs = SQMModelMgr.SelectPlantList(entities, companyID, 0).Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Where(d => plantIDs.Contains(d.PLANT_ID) && EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date &&
				EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			var businessOrgs = SQMModelMgr.SelectBusOrgList(entities, companyID, 0, true);
			var data = new List<dynamic>();
			for (int b = -1; b < businessOrgs.Count; ++b)
			{
				var busOrgID = b == -1 ? 0 : businessOrgs[b].BUS_ORG_ID;
				plantIDs = b != -1 ? SQMModelMgr.SelectPlantList(entities, companyID, busOrgID).Select(p => p.PLANT_ID).ToList() : new List<decimal>();
				var incidentRateSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 2, year), "")
				{
					DisplayLabels = true
				};

				for (int y = year - 2; y <= year; ++y)
				{
					for (int i = 1; i < 13; ++i)
					{
						var startOfMonth = new DateTime(y, i, 1);
						var startOfNextMonth = startOfMonth.AddMonths(1);

						var monthData = allData.Where(d => d.DATE.Date >= startOfMonth.Date && d.DATE.Date < startOfNextMonth.Date && (b == -1 ? true : plantIDs.Contains(d.PLANT_ID)));

						var manHours = y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
						var incidents = y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;

						var TRIR = manHours == 0 ? 0 : incidents * 200000 / manHours;

						if (y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true))
							incidentRateSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, TRIR, startOfMonth.ToString("MMMM")));
					}
				}

				var incidentRateTrendSeries = new GaugeSeries(0, "TRIR Trend (6 Month Rolling Avg.)", "");
				for (int i = 0; i < 5; ++i)
					incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, 0, ""));
				for (int i = 5; i < incidentRateSeries.ItemList.Count; ++i)
					incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0,
						Enumerable.Range(i - 5, 6).Select(j => incidentRateSeries.ItemList[j]).Sum(v => v.YValue) / 6m, incidentRateSeries.ItemList[i].Text));

				var target = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "TRIR" &&
					(b == -1 ? t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID : t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == busOrgID));

				data.Add(new
				{
					name = b == -1 ? entities.COMPANY.First(c => c.COMPANY_ID == companyID).COMPANY_NAME : businessOrgs[b].ORG_NAME,
					incidentRateSeries,
					incidentRateTrendSeries,
					incidentRateTarget = target != null ? target.TARGET_VALUE : 0
				});
			}
			return data;
		}

		static dynamic PullTRIRByPlant(PSsqmEntities entities, decimal companyID, int year)
		{
			var startOf2YearsAgo = new DateTime(year - 2, 1, 1);
			var startOfNextYear = new DateTime(year + 1, 1, 1);

			var plants = SQMModelMgr.SelectPlantList(entities, companyID, 0);
			var plantIDs = plants.Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Where(d => plantIDs.Contains(d.PLANT_ID) && EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date &&
				EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			decimal totalManHours2YearsAgo = 0;
			decimal totalManHoursPreviousYear = 0;
			decimal totalManHoursYTD = 0;
			decimal totalIncidents2YearsAgo = 0;
			decimal totalIncidentsPreviousYear = 0;
			decimal totalIncidentsYTD = 0;

			var data = new List<dynamic>();
			foreach (var plant in plants)
			{
				decimal manHours2YearsAgo = 0;
				decimal manHoursPreviousYear = 0;
				decimal manHoursYTD = 0;
				decimal incidents2YearsAgo = 0;
				decimal incidentsPreviousYear = 0;
				decimal incidentsYTD = 0;
				decimal TRIR2YearsAgo = 0;
				decimal TRIRPreviousYear = 0;
				decimal TRIRYTD = 0;

				for (int y = year - 2; y <= year; ++y)
				{
					decimal manHoursTotal = 0;
					decimal incidentsTotal = 0;

					for (int i = 1; i < 13; ++i)
					{
						var startOfMonth = new DateTime(y, i, 1);
						var startOfNextMonth = startOfMonth.AddMonths(1);

						var monthData = allData.Where(d => d.DATE.Date >= startOfMonth.Date && d.DATE.Date < startOfNextMonth.Date && d.PLANT_ID == plant.PLANT_ID);

						manHoursTotal += y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
						incidentsTotal += y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					}

					var TRIR = manHoursTotal == 0 ? 0 : incidentsTotal * 200000 / manHoursTotal;
					if (y == year - 2)
					{
						manHours2YearsAgo = manHoursTotal;
						incidents2YearsAgo = incidentsTotal;
						TRIR2YearsAgo = TRIR;
					}
					else if (y == year - 1)
					{
						manHoursPreviousYear = manHoursTotal;
						incidentsPreviousYear = incidentsTotal;
						TRIRPreviousYear = TRIR;
					}
					else
					{
						manHoursYTD = manHoursTotal;
						incidentsYTD = incidentsTotal;
						TRIRYTD = TRIR;
					}
				}

				totalManHours2YearsAgo += manHours2YearsAgo;
				totalManHoursPreviousYear += manHoursPreviousYear;
				totalManHoursYTD += manHoursYTD;
				totalIncidents2YearsAgo += incidents2YearsAgo;
				totalIncidentsPreviousYear += incidentsPreviousYear;
				totalIncidentsYTD += incidentsYTD;

				var target = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "TRIR" && t.PLANT_ID.HasValue && t.PLANT_ID == plant.PLANT_ID);
				decimal TRIRGoal = target != null ? target.TARGET_VALUE : 0;

				data.Add(new
				{
					BusOrgID = plant.BUS_ORG_ID,
					BusinessUnit = entities.BUSINESS_ORG.First(bu => bu.BUS_ORG_ID == plant.BUS_ORG_ID).ORG_NAME,
					Plant = plant.DUNS_CODE,
					manHours2YearsAgo,
					manHoursPreviousYear,
					manHoursYTD,
					incidents2YearsAgo,
					incidentsPreviousYear,
					incidentsYTD,
					TRIRGoal,
					TRIR2YearsAgo,
					TRIRPreviousYear,
					TRIRYTD,
					PercentChange = TRIRPreviousYear == 0 ? 0 : (TRIRYTD - TRIRPreviousYear) / TRIRPreviousYear,
					ProgressToGoal = TRIRGoal == 0 ? 0 : (TRIRYTD - TRIRGoal) / TRIRGoal
				});
			}

			data = data.OrderBy(d => d.BusOrgID).ToList();

			foreach (string businessUnit in data.Select(d => d.BusinessUnit).Distinct().ToList())
			{
				var last = data.Last(d => d.BusinessUnit == businessUnit);
				decimal busOrgID = last.BusOrgID;

				var target = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "TRIR" && t.BUS_ORG_ID.HasValue && t.BUS_ORG_ID == busOrgID);

				int lastIndex = data.IndexOf(last);
				var allPlantsForBU = data.Where(d => d.BusinessUnit == businessUnit);
				decimal manHours2YearsAgo = allPlantsForBU.Sum(d => (decimal)d.manHours2YearsAgo);
				decimal manHoursPreviousYear = allPlantsForBU.Sum(d => (decimal)d.manHoursPreviousYear);
				decimal manHoursYTD = allPlantsForBU.Sum(d => (decimal)d.manHoursYTD);
				decimal incidents2YearsAgo = allPlantsForBU.Sum(d => (decimal)d.incidents2YearsAgo);
				decimal incidentsPreviousYear = allPlantsForBU.Sum(d => (decimal)d.incidentsPreviousYear);
				decimal incidentsYTD = allPlantsForBU.Sum(d => (decimal)d.incidentsYTD);
				decimal TRIRGoal = target != null ? target.TARGET_VALUE : 0;
				decimal TRIRPreviousYear = manHoursPreviousYear == 0 ? 0 : incidentsPreviousYear * 200000 / manHoursPreviousYear;
				decimal TRIRYTD = manHoursYTD == 0 ? 0 : incidentsYTD * 200000 / manHoursYTD;
				data.Insert(lastIndex + 1, new
				{
					BusinessUnit = "",
					Plant = "",
					TRIRGoal,
					TRIR2YearsAgo = manHours2YearsAgo == 0 ? 0 : incidents2YearsAgo * 200000 / manHours2YearsAgo,
					TRIRPreviousYear,
					TRIRYTD,
					PercentChange = TRIRPreviousYear == 0 ? 0 : (TRIRYTD - TRIRPreviousYear) / TRIRPreviousYear,
					ProgressToGoal = TRIRGoal == 0 ? 0 : (TRIRYTD - TRIRGoal) / TRIRGoal
				});
			}

			var totalTarget = entities.EHS_TARGETS.FirstOrDefault(t => t.TYPE == "TRIR" && t.COMPANY_ID.HasValue && t.COMPANY_ID == companyID);

			decimal totalTRIRGoal = totalTarget != null ? totalTarget.TARGET_VALUE : 0;
			decimal totalTRIRPreviousYear = totalManHoursPreviousYear == 0 ? 0 : totalIncidentsPreviousYear * 200000 / totalManHoursPreviousYear;
			decimal totalTRIRYTD = totalManHoursYTD == 0 ? 0 : totalIncidentsYTD * 200000 / totalManHoursYTD;

			data.Add(new
			{
				BusinessUnit = "Total Corp.",
				Plant = "",
				TRIRGoal = totalTRIRGoal,
				TRIR2YearsAgo = totalManHours2YearsAgo == 0 ? 0 : totalIncidents2YearsAgo * 200000 / totalManHours2YearsAgo,
				TRIRPreviousYear = totalTRIRPreviousYear,
				TRIRYTD = totalTRIRYTD,
				PercentChange = totalTRIRPreviousYear == 0 ? 0 : (totalTRIRYTD - totalTRIRPreviousYear) / totalTRIRPreviousYear,
				ProgressToGoal = totalTRIRGoal == 0 ? 0 : (totalTRIRYTD - totalTRIRGoal) / totalTRIRGoal
			});

			return new
			{
				data,
				year
			};
		}

		static dynamic PullRecByPlant(PSsqmEntities entities, decimal companyID, int year)
		{
			var startOf2YearsAgo = new DateTime(year - 2, 1, 1);
			var startOfNextYear = new DateTime(year + 1, 1, 1);

			var plants = SQMModelMgr.SelectPlantList(entities, companyID, 0);
			var plantIDs = plants.Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Where(d => plantIDs.Contains(d.PLANT_ID) && EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date &&
				EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			decimal totalIncidentsPreviousYear = 0;
			decimal totalIncidentsYTD = 0;

			var data = new List<dynamic>();
			foreach (var plant in plants)
			{
				decimal incidentsPreviousYear = 0;
				decimal incidentsYTD = 0;

				for (int y = year - 2; y <= year; ++y)
				{
					decimal incidentsTotal = 0;

					for (int i = 1; i < 13; ++i)
					{
						var startOfMonth = new DateTime(y, i, 1);
						var startOfNextMonth = startOfMonth.AddMonths(1);

						var monthData = allData.Where(d => d.DATE.Date >= startOfMonth.Date && d.DATE.Date < startOfNextMonth.Date && d.PLANT_ID == plant.PLANT_ID);

						incidentsTotal += y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					}

					if (y == year - 1)
						incidentsPreviousYear = incidentsTotal;
					else
						incidentsYTD = incidentsTotal;
				}

				totalIncidentsPreviousYear += incidentsPreviousYear;
				totalIncidentsYTD += incidentsYTD;

				decimal incidentsAnnualized = Math.Round(incidentsYTD * 12 / DateTime.Today.Month);

				data.Add(new
				{
					BusOrgID = plant.BUS_ORG_ID,
					BusinessUnit = entities.BUSINESS_ORG.First(bu => bu.BUS_ORG_ID == plant.BUS_ORG_ID).ORG_NAME,
					Plant = plant.DUNS_CODE,
					RecPreviousYear = incidentsPreviousYear,
					RecYTD = incidentsYTD,
					RecAnnualized = incidentsAnnualized,
					PercentChange = incidentsPreviousYear == 0 ? 0 : (incidentsAnnualized - incidentsPreviousYear) / incidentsPreviousYear
				});
			}

			data = data.OrderBy(d => d.BusOrgID).ToList();

			foreach (string businessUnit in data.Select(d => d.BusinessUnit).Distinct().ToList())
			{
				int lastIndex = data.IndexOf(data.Last(d => d.BusinessUnit == businessUnit));
				var allPlantsForBU = data.Where(d => d.BusinessUnit == businessUnit);
				decimal incidentsPreviousYear = allPlantsForBU.Sum(d => (decimal)d.RecPreviousYear);
				decimal incidentsYTD = allPlantsForBU.Sum(d => (decimal)d.RecYTD);
				decimal incidentsAnnualized = Math.Round(incidentsYTD * 12 / DateTime.Today.Month);
				data.Insert(lastIndex + 1, new
				{
					BusinessUnit = "",
					Plant = "",
					RecPreviousYear = incidentsPreviousYear,
					RecYTD = incidentsYTD,
					RecAnnualized = incidentsAnnualized,
					PercentChange = incidentsPreviousYear == 0 ? 0 : (incidentsAnnualized - incidentsPreviousYear) / incidentsPreviousYear
				});
			}

			var totalIncidentsAnnualized = Math.Round(totalIncidentsYTD * 12 / DateTime.Today.Month);

			data.Add(new
			{
				BusinessUnit = "Total Corp.",
				Plant = "",
				RecPreviousYear = totalIncidentsPreviousYear,
				RecYTD = totalIncidentsYTD,
				RecAnnualized = totalIncidentsAnnualized,
				PercentChange = totalIncidentsPreviousYear == 0 ? 0 : (totalIncidentsAnnualized - totalIncidentsPreviousYear) / totalIncidentsPreviousYear
			});

			return new
			{
				data,
				year
			};
		}

		static List<dynamic> PullBalancedScorecardData(PSsqmEntities entities, decimal companyID, int year)
		{
			var data = new List<dynamic>();
			var businessLocs = SQMModelMgr.SelectBusinessLocationList(companyID, 0, true);

			var totalCorpData = PullData(entities, "-1", companyID, year, DataToUse.BalancedScorecard);
			var totalCorpDataList = totalCorpData.data as List<Data>;
			data.Add(new
			{
				Name = "Total Corp.",
				TRIR = new
				{
					ItemType = "Incident Rate",
					Target = totalCorpData.incidentRateTarget,
					Jan = totalCorpDataList[0].TRIR,
					Feb = totalCorpDataList[1].TRIR,
					Mar = totalCorpDataList[2].TRIR,
					Apr = totalCorpDataList[3].TRIR,
					May = totalCorpDataList[4].TRIR,
					Jun = totalCorpDataList[5].TRIR,
					Jul = totalCorpDataList[6].TRIR,
					Aug = totalCorpDataList[7].TRIR,
					Sep = totalCorpDataList[8].TRIR,
					Oct = totalCorpDataList[9].TRIR,
					Nov = totalCorpDataList[10].TRIR,
					Dec = totalCorpDataList[11].TRIR,
					YTD = totalCorpDataList[12].TRIR
				},
				FrequencyRate = new
				{
					ItemType = "Freqeucny Rate",
					Target = totalCorpData.frequencyRateTarget,
					Jan = totalCorpDataList[0].FrequencyRate,
					Feb = totalCorpDataList[1].FrequencyRate,
					Mar = totalCorpDataList[2].FrequencyRate,
					Apr = totalCorpDataList[3].FrequencyRate,
					May = totalCorpDataList[4].FrequencyRate,
					Jun = totalCorpDataList[5].FrequencyRate,
					Jul = totalCorpDataList[6].FrequencyRate,
					Aug = totalCorpDataList[7].FrequencyRate,
					Sep = totalCorpDataList[8].FrequencyRate,
					Oct = totalCorpDataList[9].FrequencyRate,
					Nov = totalCorpDataList[10].FrequencyRate,
					Dec = totalCorpDataList[11].FrequencyRate,
					YTD = totalCorpDataList[12].FrequencyRate
				},
				SeverityRate = new
				{
					ItemType = "Severity Rate",
					Target = totalCorpData.severityRateTarget,
					Jan = totalCorpDataList[0].SeverityRate,
					Feb = totalCorpDataList[1].SeverityRate,
					Mar = totalCorpDataList[2].SeverityRate,
					Apr = totalCorpDataList[3].SeverityRate,
					May = totalCorpDataList[4].SeverityRate,
					Jun = totalCorpDataList[5].SeverityRate,
					Jul = totalCorpDataList[6].SeverityRate,
					Aug = totalCorpDataList[7].SeverityRate,
					Sep = totalCorpDataList[8].SeverityRate,
					Oct = totalCorpDataList[9].SeverityRate,
					Nov = totalCorpDataList[10].SeverityRate,
					Dec = totalCorpDataList[11].SeverityRate,
					YTD = totalCorpDataList[12].SeverityRate
				}
			});

			decimal? busOrgID = null;
			foreach (var businessLoc in businessLocs.OrderBy(l => l.Plant.BUS_ORG_ID).ThenBy(l => l.Plant.PLANT_NAME))
			{
				if (businessLoc.Plant.BUS_ORG_ID != busOrgID)
				{
					busOrgID = businessLoc.Plant.BUS_ORG_ID;
					var busOrgData = PullData(entities, "BU" + busOrgID, companyID, year, DataToUse.BalancedScorecard);
					var busOrgDataList = busOrgData.data as List<Data>;
					data.Add(new
					{
						Name = businessLoc.BusinessOrg.ORG_NAME,
						TRIR = new
						{
							ItemType = "Incident Rate",
							Target = busOrgData.incidentRateTarget,
							Jan = busOrgDataList[0].TRIR,
							Feb = busOrgDataList[1].TRIR,
							Mar = busOrgDataList[2].TRIR,
							Apr = busOrgDataList[3].TRIR,
							May = busOrgDataList[4].TRIR,
							Jun = busOrgDataList[5].TRIR,
							Jul = busOrgDataList[6].TRIR,
							Aug = busOrgDataList[7].TRIR,
							Sep = busOrgDataList[8].TRIR,
							Oct = busOrgDataList[9].TRIR,
							Nov = busOrgDataList[10].TRIR,
							Dec = busOrgDataList[11].TRIR,
							YTD = busOrgDataList[12].TRIR
						},
						FrequencyRate = new
						{
							ItemType = "Freqeucny Rate",
							Target = busOrgData.frequencyRateTarget,
							Jan = busOrgDataList[0].FrequencyRate,
							Feb = busOrgDataList[1].FrequencyRate,
							Mar = busOrgDataList[2].FrequencyRate,
							Apr = busOrgDataList[3].FrequencyRate,
							May = busOrgDataList[4].FrequencyRate,
							Jun = busOrgDataList[5].FrequencyRate,
							Jul = busOrgDataList[6].FrequencyRate,
							Aug = busOrgDataList[7].FrequencyRate,
							Sep = busOrgDataList[8].FrequencyRate,
							Oct = busOrgDataList[9].FrequencyRate,
							Nov = busOrgDataList[10].FrequencyRate,
							Dec = busOrgDataList[11].FrequencyRate,
							YTD = busOrgDataList[12].FrequencyRate
						},
						SeverityRate = new
						{
							ItemType = "Severity Rate",
							Target = busOrgData.severityRateTarget,
							Jan = busOrgDataList[0].SeverityRate,
							Feb = busOrgDataList[1].SeverityRate,
							Mar = busOrgDataList[2].SeverityRate,
							Apr = busOrgDataList[3].SeverityRate,
							May = busOrgDataList[4].SeverityRate,
							Jun = busOrgDataList[5].SeverityRate,
							Jul = busOrgDataList[6].SeverityRate,
							Aug = busOrgDataList[7].SeverityRate,
							Sep = busOrgDataList[8].SeverityRate,
							Oct = busOrgDataList[9].SeverityRate,
							Nov = busOrgDataList[10].SeverityRate,
							Dec = busOrgDataList[11].SeverityRate,
							YTD = busOrgDataList[12].SeverityRate
						}
					});
				}

				var plantData = PullData(entities, businessLoc.Plant.PLANT_ID.ToString(), companyID, year, DataToUse.BalancedScorecard);
				var plantDataList = plantData.data as List<Data>;
				data.Add(new
				{
					Name = businessLoc.Plant.PLANT_NAME,
					TRIR = new
					{
						ItemType = "Incident Rate",
						Target = plantData.incidentRateTarget,
						Jan = plantDataList[0].TRIR,
						Feb = plantDataList[1].TRIR,
						Mar = plantDataList[2].TRIR,
						Apr = plantDataList[3].TRIR,
						May = plantDataList[4].TRIR,
						Jun = plantDataList[5].TRIR,
						Jul = plantDataList[6].TRIR,
						Aug = plantDataList[7].TRIR,
						Sep = plantDataList[8].TRIR,
						Oct = plantDataList[9].TRIR,
						Nov = plantDataList[10].TRIR,
						Dec = plantDataList[11].TRIR,
						YTD = plantDataList[12].TRIR
					},
					FrequencyRate = new
					{
						ItemType = "Freqeucny Rate",
						Target = plantData.frequencyRateTarget,
						Jan = plantDataList[0].FrequencyRate,
						Feb = plantDataList[1].FrequencyRate,
						Mar = plantDataList[2].FrequencyRate,
						Apr = plantDataList[3].FrequencyRate,
						May = plantDataList[4].FrequencyRate,
						Jun = plantDataList[5].FrequencyRate,
						Jul = plantDataList[6].FrequencyRate,
						Aug = plantDataList[7].FrequencyRate,
						Sep = plantDataList[8].FrequencyRate,
						Oct = plantDataList[9].FrequencyRate,
						Nov = plantDataList[10].FrequencyRate,
						Dec = plantDataList[11].FrequencyRate,
						YTD = plantDataList[12].FrequencyRate
					},
					SeverityRate = new
					{
						ItemType = "Severity Rate",
						Target = plantData.severityRateTarget,
						Jan = plantDataList[0].SeverityRate,
						Feb = plantDataList[1].SeverityRate,
						Mar = plantDataList[2].SeverityRate,
						Apr = plantDataList[3].SeverityRate,
						May = plantDataList[4].SeverityRate,
						Jun = plantDataList[5].SeverityRate,
						Jul = plantDataList[6].SeverityRate,
						Aug = plantDataList[7].SeverityRate,
						Sep = plantDataList[8].SeverityRate,
						Oct = plantDataList[9].SeverityRate,
						Nov = plantDataList[10].SeverityRate,
						Dec = plantDataList[11].SeverityRate,
						YTD = plantDataList[12].SeverityRate
					}
				});
			}

			return data;
		}

		PSsqmEntities entities;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.entities = new PSsqmEntities();

			if (!this.IsPostBack)
			{
				this.hfCompanyID.Value = SessionManager.UserContext.HRLocation.Company.COMPANY_ID.ToString();
				SQMBasePage.SetLocationList(this.rcbPlant,
					UserContext.FilterPlantAccessList(SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true)),
					SessionManager.UserContext.HRLocation.Plant.PLANT_ID, true);
				this.rcbPlant.Items.Insert(0, new RadComboBoxItem("AAM - Whole Company", "-1")
				{
					IsSeparator = false,
					BackColor = Color.Red,
					ForeColor = Color.White,
					ToolTip = "Whole Company",
					ImageUrl = "~/images/defaulticon/16x16/asterisk.png"
				});
				this.rmypYear.SelectedDate = DateTime.Today;

				this.divExport.Style.Add("width", gaugeDef.Width + "px");
				this.divTRIR.Style.Add("width", gaugeDef.Width + "px");
				this.divFrequencyRate.Style.Add("width", gaugeDef.Width + "px");
				this.divSeverityRate.Style.Add("width", gaugeDef.Width + "px");

				dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, this.rmypYear.SelectedDate.Value.Year,
					DataToUse.Metrics);

				this.UpdateCharts(data);
			}
		}

		protected void rgTRIRPlant_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				dynamic dataItem = e.Item.DataItem;
				var row = e.Item as GridDataItem;

				if (row["BusinessUnit"].Text == "&nbsp;")
					row.Style.Add("background-color", "#d9d9d9");

				var percentChangeCell = row["PercentChange"];
				var label = new Label();
				if (dataItem.PercentChange < 0)
				{
					label.Text = "&#8659;";
					label.ForeColor = percentChangeCell.BackColor = Color.Green;
					percentChangeCell.ForeColor = Color.White;
				}
				else if (dataItem.PercentChange > 0)
				{
					label.Text = "&#8657;";
					label.ForeColor = percentChangeCell.BackColor = Color.Red;
					percentChangeCell.ForeColor = Color.White;
				}
				else
					label.Text = "=";
				row["ImprovedOrDeclined"].Controls.Add(label);

				if (sender == this.rgTRIRPlant)
				{
					var progressToGoalCell = row["ProgressToGoal"];
					if (dataItem.ProgressToGoal < 0)
					{
						progressToGoalCell.BackColor = Color.Green;
						progressToGoalCell.ForeColor = Color.White;
					}
					else if (dataItem.ProgressToGoal > 0)
					{
						progressToGoalCell.BackColor = Color.Red;
						progressToGoalCell.ForeColor = Color.White;
					}
				}
			}
		}

		protected void rgTRIRPlant_PreRender(object sender, EventArgs e)
		{
			var rg = sender as RadGrid;
			for (int i = rg.Items.Count - 2; i >= 0; --i)
			{
				var rowBU = rg.Items[i]["BusinessUnit"];
				var nextRow = rg.Items[i + 1];
				var nextRowBU = nextRow["BusinessUnit"];
				if (rowBU.Text == nextRowBU.Text)
				{
					rowBU.RowSpan = nextRowBU.RowSpan < 2 ? 2 : nextRowBU.RowSpan + 1;
					nextRowBU.Visible = false;
					nextRow["Plant"].Style.Add("border-left-width", "1px");
				}
			}
		}

		protected void rptBalancedScorecard_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				var rgBalancedScorescardHeader = e.Item.FindControl("rgBalancedScorescardHeader") as RadGrid;
				if (this.rmypYear.SelectedDate.Value.Year == DateTime.Today.Year)
				{
					int nextMonth = DateTime.Today.Month + 1;
					for (int i = nextMonth; i < 13; ++i)
						rgBalancedScorescardHeader.MasterTableView.GetColumn("Month" + i).Visible = false;
				}
				rgBalancedScorescardHeader.MasterTableView.Width = new Unit(gaugeDef.Width, UnitType.Pixel);
				rgBalancedScorescardHeader.DataSource = new List<dynamic>();
				rgBalancedScorescardHeader.DataBind();
			}
			else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				dynamic dataItem = e.Item.DataItem;
				var rgBalancedScorecardItem = e.Item.FindControl("rgBalancedScorecardItem") as RadGrid;
				if (this.rmypYear.SelectedDate.Value.Year == DateTime.Today.Year)
				{
					int nextMonth = DateTime.Today.Month + 1;
					for (int i = nextMonth; i < 13; ++i)
						rgBalancedScorecardItem.MasterTableView.GetColumn("Month" + i).Visible = false;
				}
				rgBalancedScorecardItem.MasterTableView.Width = new Unit(gaugeDef.Width, UnitType.Pixel);
				rgBalancedScorecardItem.DataSource = new List<dynamic>()
				{
					new
					{
						ItemType = dataItem.Name
					},
					dataItem.TRIR,
					dataItem.FrequencyRate,
					dataItem.SeverityRate
				};
				rgBalancedScorecardItem.DataBind();
			}
		}

		bool didFirstHeader_rgBalancedScorescardHeader = false;

		protected void rgBalancedScorescardHeader_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header)
			{
				if (!this.didFirstHeader_rgBalancedScorescardHeader)
				{
					e.Item.Cells[e.Item.Cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.rmypYear.SelectedDate.Value.Year.ToString();
					this.didFirstHeader_rgBalancedScorescardHeader = true;
				}
				else
				{
					int width = 200;
					if (this.rmypYear.SelectedDate.Value.Year == DateTime.Today.Year)
						width += 100 * (12 - DateTime.Today.Month);
					(sender as RadGrid).MasterTableView.GetColumn("Target").HeaderStyle.Width = new Unit(width, UnitType.Pixel);
				}
			}
		}

		protected void rgBalancedScorecardItem_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				int width = 150;
				if (this.rmypYear.SelectedDate.Value.Year == DateTime.Today.Year)
					width += 100 * (12 - DateTime.Today.Month);
				(sender as RadGrid).MasterTableView.GetColumn("ItemType").HeaderStyle.Width = new Unit(width, UnitType.Pixel);

				var item = e.Item as GridDataItem;
				if (item["Target"].Text == "&nbsp;")
				{
					item["ItemType"].ColumnSpan = (this.rmypYear.SelectedDate.Value.Year == DateTime.Today.Year ? DateTime.Today.Month : 12) + 3;
					item["ItemType"].Font.Bold = true;
					item.Cells.Remove(item["YTD"]);
					for (int i = 12; i > 0; --i)
						item.Cells.Remove(item["Month" + i]);
					item.Cells.Remove(item["Target"]);
				}
				else
				{
					dynamic dataItem = e.Item.DataItem as dynamic;
					decimal target = dataItem.Target;
					var values = new decimal[]
					{
						dataItem.Jan, dataItem.Feb, dataItem.Mar, dataItem.Apr, dataItem.May, dataItem.Jun, dataItem.Jul, dataItem.Aug, dataItem.Sep, dataItem.Oct, dataItem.Nov, dataItem.Dec
					};
					TableCell cell;
					for (int i = 0; i < 12; ++i)
					{
						cell = item["Month" + (i + 1)];
						if (values[i] > target)
							cell.BackColor = Color.Red;
						else
							cell.BackColor = Color.Green;
					}
					cell = item["YTD"];
					if (dataItem.YTD > target)
						cell.BackColor = Color.Red;
					else
						cell.BackColor = Color.Green;
				}
			}
		}

		bool didFirstHeader_rgReport = false;

		protected void rgReport_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header && !this.didFirstHeader_rgReport)
			{
				e.Item.Cells[e.Item.Cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.rmypYear.SelectedDate.Value.Year.ToString();
				this.didFirstHeader_rgReport = true;
			}
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				var month = (e.Item.DataItem as Data).Month;
				if (month == "YTD")
					e.Item.BackColor = Color.FromArgb(255, 255, 153);
				else if (month == "Target")
				{
					var item = e.Item as GridDataItem;
					foreach (var column in new[] { "ManHours", "Incidents", "Frequency", "Restricted", "Severity", "FirstAid", "Leadership", "JSAs", "SafetyTraining" })
						item[column].Text = "";
				}
			}
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			string plantID = this.rddlType.SelectedValue == "Metrics" ? this.rcbPlant.SelectedValue : "-1";
			dynamic data = this.rddlType.SelectedValue == "TRIRBusiness" ? PullTRIRByBusinessUnit(this.entities, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year) :
				(this.rddlType.SelectedValue == "TRIRPlant" ? PullTRIRByPlant(this.entities, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year) :
				(this.rddlType.SelectedValue == "RecPlant" ? PullRecByPlant(this.entities, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year) :
				(this.rddlType.SelectedValue == "BalancedScordcard" ? PullBalancedScorecardData(this.entities, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year) :
				PullData(this.entities, plantID, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year,
				(DataToUse)Enum.Parse(typeof(DataToUse), this.rddlType.SelectedValue)))));

			this.UpdateCharts(data);
		}

		void UpdateCharts(dynamic data)
		{
			gaugeDef.Target = smallGaugeDef.Target = null;

			bool pyramid = this.rddlType.SelectedValue == "Pyramid";
			bool trirBusiness = this.rddlType.SelectedValue == "TRIRBusiness";
			bool trirPlant = this.rddlType.SelectedValue == "TRIRPlant";
			bool recPlant = this.rddlType.SelectedValue == "RecPlant";
			bool balancedScorecard = this.rddlType.SelectedValue == "BalancedScordcard";
			bool metrics = this.rddlType.SelectedValue == "Metrics";

			this.pnlPyramidOutput.Visible = pyramid;
			this.pnlTRIRBusinessOutput.Visible = trirBusiness;
			this.pnlTRIRPlantOutput.Visible = trirPlant;
			this.pnlRecPlantOutput.Visible = recPlant;
			this.pnlBalancedScorecardOutput.Visible = balancedScorecard;
			this.pnlMetricsOutput.Visible = metrics;
			this.pnlMetrics.Style["display"] = metrics ? "visible" : "none";

			if (pyramid)
			{
				var left = (gaugeDef.Width - 420 - this.pyramid.Width.ToPixels()) / 2;
				this.pyramid.Style.Add("left", left + "px");
				var YTD = (data.data as List<Data>)[12];
				this.pyramid.Fatalities = YTD.Fatalities;
				this.pyramid.LostTimeCases = YTD.Frequency;
				this.pyramid.RecordableInjuries = YTD.Incidents;
				this.pyramid.FirstAidCases = YTD.FirstAid;
				this.pyramid.NearMisses = YTD.NearMisses;

				var rowHeight = this.pyramid.Height.Divide(5).ToPixels() * 0.996m;
				var halfWidth = this.pyramid.Width.Divide(2).ToPixels();
				this.pyramidTable.Style.Add("left", (halfWidth + left) + "px");
				this.pyramidTable_column1.Style.Add("width", (halfWidth + 20) + "px");
				this.pyramidTable_columnAnnualized.InnerText = "Annualized " + DateTime.Today.Year;
				this.pyramidTable_columnPreviousYear.InnerText = (DateTime.Today.Year - 1).ToString();

				this.pyramidTable_fatalitiesRow.Style.Add("height", rowHeight + "px");
				this.pyramidTable_fatalitiesYTD.InnerText = YTD.Fatalities.ToString();
				var fatalitiesAnnualized = Math.Round(YTD.Fatalities * 12 / DateTime.Today.Month);
				this.pyramidTable_fatalitiesAnnualized.InnerText = fatalitiesAnnualized.ToString();
				this.pyramidTable_fatalitiesPreviousYear.InnerText = data.previousYTD.Fatalities.ToString();
				var fatalitiesVariance = data.previousYTD.Fatalities == 0 ? 0 : (fatalitiesAnnualized - data.previousYTD.Fatalities) / data.previousYTD.Fatalities;
				this.pyramidTable_fatalitiesVariance.InnerText = fatalitiesVariance.ToString("P1");
				this.pyramidTable_fatalitiesVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (fatalitiesVariance > 0 ? "Bad" : "Good"));

				this.pyramidTable_lostTimeRow.Style.Add("height", rowHeight + "px");
				this.pyramidTable_lostTimeYTD.InnerText = YTD.Frequency.ToString();
				var lostTimeAnnualized = Math.Round(YTD.Frequency * 12 / DateTime.Today.Month);
				this.pyramidTable_lostTimeAnnualized.InnerText = lostTimeAnnualized.ToString();
				this.pyramidTable_lostTimePreviousYear.InnerText = data.previousYTD.Frequency.ToString();
				var lostTimeVariance = data.previousYTD.Frequency == 0 ? 0 : (lostTimeAnnualized - data.previousYTD.Frequency) / data.previousYTD.Frequency;
				this.pyramidTable_lostTimeVariance.InnerText = lostTimeVariance.ToString("P1");
				this.pyramidTable_lostTimeVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (lostTimeVariance > 0 ? "Bad" : "Good"));

				this.pyramidTable_recordableRow.Style.Add("height", rowHeight + "px");
				this.pyramidTable_recordableYTD.InnerText = YTD.Incidents.ToString();
				var recordableAnnualized = Math.Round(YTD.Incidents * 12 / DateTime.Today.Month);
				this.pyramidTable_recordableAnnualized.InnerText = recordableAnnualized.ToString();
				this.pyramidTable_recordablePreviousYear.InnerText = data.previousYTD.Incidents.ToString();
				var recordableVariance = data.previousYTD.Incidents == 0 ? 0 : (recordableAnnualized - data.previousYTD.Incidents) / data.previousYTD.Incidents;
				this.pyramidTable_recordableVariance.InnerText = recordableVariance.ToString("P1");
				this.pyramidTable_recordableVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (recordableVariance > 0 ? "Bad" : "Good"));

				this.pyramidTable_firstAidRow.Style.Add("height", rowHeight + "px");
				this.pyramidTable_firstAidYTD.InnerText = YTD.FirstAid.ToString();
				var firstAidAnnualized = Math.Round(YTD.FirstAid * 12 / DateTime.Today.Month);
				this.pyramidTable_firstAidAnnualized.InnerText = firstAidAnnualized.ToString();
				this.pyramidTable_firstAidPreviousYear.InnerText = data.previousYTD.FirstAid.ToString();
				var firstAidVariance = data.previousYTD.FirstAid == 0 ? 0 : (firstAidAnnualized - data.previousYTD.FirstAid) / data.previousYTD.FirstAid;
				this.pyramidTable_firstAidVariance.InnerText = firstAidVariance.ToString("P1");
				this.pyramidTable_firstAidVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (firstAidVariance > 0 ? "Bad" : "Good"));

				this.pyramidTable_nearMissesRow.Style.Add("height", rowHeight + "px");
				this.pyramidTable_nearMissesYTD.InnerText = YTD.NearMisses.ToString();
				var nearMissesAnnualized = Math.Round(YTD.NearMisses * 12 / DateTime.Today.Month);
				this.pyramidTable_nearMissesAnnualized.InnerText = nearMissesAnnualized.ToString();
				this.pyramidTable_nearMissesPreviousYear.InnerText = data.previousYTD.NearMisses.ToString();
				var nearMissesVariance = data.previousYTD.NearMisses == 0 ? 0 : (nearMissesAnnualized - data.previousYTD.NearMisses) / data.previousYTD.NearMisses;
				this.pyramidTable_nearMissesVariance.InnerText = nearMissesVariance.ToString("P1");
				this.pyramidTable_nearMissesVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (nearMissesVariance > 0 ? "Bad" : "Good"));

				gaugeDef.Height = 410;
				gaugeDef.Title = "Current Indicators - JSAs & Combined Audits";
				gaugeDef.Target = new PERSPECTIVE_TARGET()
				{
					TARGET_VALUE = data.jsasTarget,
					DESCR_SHORT = "Target"
				};
				var series = new List<GaugeSeries>() { data.jsasSeries, data.jsasTrendSeries };
				SetScale(gaugeDef, series);
				this.uclChart.CreateMultiLineChart(gaugeDef, series, this.divJSAsAndAudits_Pyramid);

				gaugeDef.Title = "Safety Training Hours";
				gaugeDef.Target = null;
				series = new List<GaugeSeries>() { data.safetyTrainingHoursSeries };
				SetScale(gaugeDef, series);
				this.uclChart.CreateMultiLineChart(gaugeDef, series, this.divSafetyTrainingHours_Pyramid);
			}
			if (trirBusiness)
			{
				gaugeDef.Height = 410;
				int count = 0;
				foreach (var businessOrgData in data)
				{
					gaugeDef.Title = businessOrgData.name.ToUpper() + " TOTAL RECORDABLE INCIDENT RATE";
					gaugeDef.Target = new PERSPECTIVE_TARGET()
					{
						TARGET_VALUE = businessOrgData.incidentRateTarget,
						DESCR_SHORT = "Target"
					};
					var series = new List<GaugeSeries>() { businessOrgData.incidentRateSeries, businessOrgData.incidentRateTrendSeries };
					SetScale(gaugeDef, series);
					var container = new HtmlGenericControl("div");
					container.Attributes.Add("class", "chartMarginTop");
					this.uclChart.CreateMultiLineChart(gaugeDef, series, container);
					this.pnlTRIRBusinessOutput.Controls.Add(container);

					++count;
					if ((count % 2) == 0)
					{
						var pageBreak = new HtmlGenericControl("div");
						pageBreak.Style.Add("page-break-after", "always");
						this.pnlTRIRBusinessOutput.Controls.Add(pageBreak);
					}
				}
			}
			if (trirPlant)
			{
				this.rgTRIRPlant.MasterTableView.GetColumn("ImprovedOrDeclined").HeaderText =
					"Improved <span style=\"color: green; font-size: 18px\">&#8659;</span><br/>or<br/>Declined <span style=\"color: red; font-size: 18px\">&#8657;</span>";

				this.rgTRIRPlant.MasterTableView.GetColumn("TRIR2YearsAgo").HeaderText = "TRIR<br/>" + (data.year - 2);
				this.rgTRIRPlant.MasterTableView.GetColumn("TRIRPreviousYear").HeaderText = "TRIR<br/>" + (data.year - 1);
				this.rgTRIRPlant.MasterTableView.GetColumn("TRIRYTD").HeaderText = "TRIR YTD<br/>" + data.year;
				this.rgTRIRPlant.MasterTableView.GetColumn("PercentChange").HeaderText = "% Change<br/>" + data.year + " vs. " + (data.year - 1);

				this.rgTRIRPlant.DataSource = data.data;
				this.rgTRIRPlant.DataBind();
			}
			if (recPlant)
			{
				this.rgRecPlant.MasterTableView.GetColumn("ImprovedOrDeclined").HeaderText =
					"Improved <span style=\"color: green; font-size: 18px\">&#8659;</span><br/>or<br/>Declined <span style=\"color: red; font-size: 18px\">&#8657;</span>";

				this.rgRecPlant.MasterTableView.GetColumn("RecPreviousYear").HeaderText = "Recordables<br/>" + (data.year - 1);
				this.rgRecPlant.MasterTableView.GetColumn("RecYTD").HeaderText = "Recordables YTD<br/>" + data.year;

				this.rgRecPlant.DataSource = data.data;
				this.rgRecPlant.DataBind();
			}
			if (balancedScorecard)
			{
				this.rptBalancedScorecard.DataSource = data;
				this.rptBalancedScorecard.DataBind();
			}
			if (metrics)
			{
				this.rgReport.DataSource = data.data;
				this.rgReport.DataBind();

				gaugeDef.Height = 500;
				gaugeDef.Title = "TOTAL RECORDABLE INCIDENT RATE";
				gaugeDef.Target = new PERSPECTIVE_TARGET()
				{
					TARGET_VALUE = data.incidentRateTarget,
					DESCR_SHORT = "Target"
				};
				var series = new List<GaugeSeries>() { data.incidentRateSeries, data.incidentRateTrendSeries };
				SetScale(gaugeDef, series);
				this.uclChart.CreateMultiLineChart(gaugeDef, series, this.divTRIR);

				var calcsResult = new CalcsResult().Initialize();

				gaugeDef.Title = "FREQUENCY RATE";
				gaugeDef.Target = null;
				SetScale(gaugeDef, data.frequencyRateSeries);
				calcsResult.metricSeries = data.frequencyRateSeries;
				this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, calcsResult, this.divFrequencyRate);

				gaugeDef.Title = "SEVERITY RATE";
				SetScale(gaugeDef, data.severityRateSeries);
				calcsResult.metricSeries = data.severityRateSeries;
				this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, calcsResult, this.divSeverityRate);

				if (this.rmypYear.SelectedDate.Value.Year != DateTime.Today.Year || (data.data as List<Data>)[12].TRIR == 0)
					this.divPie1.Visible = this.divPie2.Visible = this.divPie3.Visible = this.divBreakPie.Visible = false;
				else
				{
					this.divPie1.Visible = this.divPie2.Visible = this.divPie3.Visible = this.divBreakPie.Visible = true;
					this.pieRecordableType.Values = data.ordinalTypeSeries;
					this.pieRecordableBodyPart.Values = data.ordinalBodyPartSeries;
					this.pieRecordableRootCause.Values = data.ordinalRootCauseSeries;
					this.pieRecordableTenure.Values = data.ordinalTenureSeries;
					this.pieRecordableDaysToClose.Values = data.ordinalDaysToCloseSeries;
				}

				smallGaugeDef.Title = "Current Indicators - JSAs & Combined Audits";
				smallGaugeDef.Target = new PERSPECTIVE_TARGET()
				{
					TARGET_VALUE = data.jsasTarget,
					DESCR_SHORT = "Target"
				};
				series = new List<GaugeSeries>() { data.jsasSeries, data.jsasTrendSeries };
				SetScale(smallGaugeDef, series);
				this.uclChart.CreateMultiLineChart(smallGaugeDef, series, this.divJSAsAndAudits_Metrics);

				smallGaugeDef.Title = "Safety Training Hours";
				smallGaugeDef.Target = null;
				series = new List<GaugeSeries>() { data.safetyTrainingHoursSeries };
				SetScale(smallGaugeDef, series);
				this.uclChart.CreateMultiLineChart(smallGaugeDef, series, this.divSafetyTrainingHours_Metrics);
			}
		}
	}
}
