using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Web.Services;
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
		public enum DataToUse
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
			SupervisorAudits = 0x100,
			Leadership = 0x0200,
			JSAs = 0x0400,
			SafetyTraining = 0x0800,
			Fatalities = 0x1000,
			NearMisses = 0x2000,
			Ordinals = 0x4000,

			Pyramid = Incidents | Frequency | FirstAid | SupervisorAudits | Leadership | JSAs | SafetyTraining | Fatalities | NearMisses,
			BalancedScorecard = TRIR | FrequencyRate | SeverityRate | Incidents | Frequency | Severity,
			Metrics = TRIR | FrequencyRate | SeverityRate | Incidents | Frequency | Restricted | Severity | FirstAid | SupervisorAudits | Leadership | JSAs | SafetyTraining | Ordinals
		}

		public class Data
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
			public decimal SupervisorAudits { get; set; }
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
				if (a.DataToUse.Has(DataToUse.SupervisorAudits))
					c.SupervisorAudits = a.SupervisorAudits + b.SupervisorAudits;
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

		public static GaugeDefinition gaugeDef = new GaugeDefinition()
		{
			Height = 500,
			Width = 1500,
			DisplayLegend = true,
			LegendPosition = ChartLegendPosition.Right,
			LegendBackgroundColor = Color.White,
			OnLoad = "radHtmlChart_Load"
		};
		public static GaugeDefinition smallGaugeDef = new GaugeDefinition()
		{
			Height = 500,
			Width = 740,
			DisplayLegend = true,
			LegendPosition = ChartLegendPosition.Right,
			LegendBackgroundColor = Color.White,
			OnLoad = "radHtmlChart_Load"
		};

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
			var plantIDs = plantID.StartsWith("BU") ? SQMModelMgr.SelectPlantList(entities, companyID, plantID_dec).Where(p => p.STATUS == "A").Select(p => p.PLANT_ID).ToList() : new List<decimal>();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Include("PLANT").Where(d => d.PLANT.STATUS == "A" &&
				(plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(d.PLANT_ID) : d.PLANT_ID == plantID_dec)) &&
				EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();
			var allOrdData = entities.EHS_DATA_ORD.Where(o => o.EHS_DATA.PLANT.STATUS == "A" &&
				(plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(o.EHS_DATA.PLANT_ID) : o.EHS_DATA.PLANT_ID == plantID_dec)) &&
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
			var safetyTrainingHoursSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 1, year), "")
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
						monthData.FirstAid = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.SupervisorAudits))
						monthData.SupervisorAudits = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S30002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.Leadership))
						monthData.Leadership = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S30003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.JSAs))
						monthData.JSAs = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allMonthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S40003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					if (dataToUse.Has(DataToUse.SafetyTraining))
						monthData.SafetyTraining = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
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

					if (y < year || (y == DateTime.Today.Year ? startOfMonth.Month < DateTime.Today.Month : true))
						incidentRateSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, monthData.TRIR, monthData.Month));
					if (y > year - 2)
					{
						frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.FrequencyRate, monthData.Month));
						severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.SeverityRate, monthData.Month));
					}
					if (y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month < DateTime.Today.Month : true))
					{
						jsasSeries.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, monthData.SupervisorAudits + monthData.Leadership + monthData.JSAs, monthData.Month));
						safetyTrainingHoursSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, monthData.SafetyTraining, monthData.Month));
					}

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
				incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, null, ""));
			for (int i = 5; i < incidentRateSeries.ItemList.Count; ++i)
				incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0,
					Enumerable.Range(i - 5, 6).Select(j => incidentRateSeries.ItemList[j]).Sum(v => v.YValue ?? 0) / 6m, incidentRateSeries.ItemList[i].Text));
			var jsasTrendSeries = new GaugeSeries(0, "Leading Trend", "");
			for (int i = 0; i < 5; ++i)
				jsasTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, null, ""));
			for (int i = 5; i < jsasSeries.ItemList.Count; ++i)
				jsasTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, Enumerable.Range(i - 5, 6).Select(j => jsasSeries.ItemList[j]).Sum(v => v.YValue ?? 0) / 6m, jsasSeries.ItemList[i].Text));
			var safetyTrainingHoursTrendSeries = new GaugeSeries(0, "Leading Trend", "");
			for (int i = 0; i < 5; ++i)
				safetyTrainingHoursTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, null, ""));
			for (int i = 5; i < safetyTrainingHoursSeries.ItemList.Count; ++i)
				safetyTrainingHoursTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0,
					Enumerable.Range(i - 5, 6).Select(j => safetyTrainingHoursSeries.ItemList[j]).Sum(v => v.YValue ?? 0) / 6m, safetyTrainingHoursSeries.ItemList[i].Text));

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
				title = plantID_dec == -1 ? entities.COMPANY.First(c => c.COMPANY_ID == companyID).COMPANY_NAME :
					(plantID.StartsWith("BU") ? entities.BUSINESS_ORG.First(b => b.BUS_ORG_ID == plantID_dec).ORG_NAME : entities.PLANT.First(p => p.PLANT_ID == plantID_dec).PLANT_NAME),
				data,
				year,
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
				safetyTrainingHoursSeries,
				safetyTrainingHoursTrendSeries
			};
		}

		static List<dynamic> PullTRIRByBusinessUnit(PSsqmEntities entities, decimal companyID, int year)
		{
			var startOf2YearsAgo = new DateTime(year - 2, 1, 1);
			var startOfNextYear = new DateTime(year + 1, 1, 1);

			var plantIDs = SQMModelMgr.SelectPlantList(entities, companyID, 0).Where(p => p.STATUS == "A").Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Include("PLANT").Where(d => plantIDs.Contains(d.PLANT_ID) &&
				EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			var businessOrgs = SQMModelMgr.SelectBusOrgList(entities, companyID, 0, true);
			var data = new List<dynamic>();
			for (int b = -1; b < businessOrgs.Count; ++b)
			{
				var busOrgID = b == -1 ? 0 : businessOrgs[b].BUS_ORG_ID;
				if (busOrgID == 99) // AAM
					continue;
				plantIDs = b != -1 ? SQMModelMgr.SelectPlantList(entities, companyID, busOrgID).Where(p => p.STATUS == "A").Select(p => p.PLANT_ID).ToList() : new List<decimal>();
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

						var manHours = y < year || (y == DateTime.Today.Year ? startOfMonth.Month < DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
						var incidents = y < year || (y == DateTime.Today.Year ? startOfMonth.Month < DateTime.Today.Month : true) ?
							monthData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;

						var TRIR = manHours == 0 ? 0 : incidents * 200000 / manHours;

						if (y < year || (y == DateTime.Today.Year ? startOfMonth.Month < DateTime.Today.Month : true))
							incidentRateSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, TRIR, startOfMonth.ToString("MMMM")));
					}
				}

				var incidentRateTrendSeries = new GaugeSeries(0, "TRIR Trend (6 Month Rolling Avg.)", "");
				for (int i = 0; i < 5; ++i)
					incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, null, ""));
				for (int i = 5; i < incidentRateSeries.ItemList.Count; ++i)
					incidentRateTrendSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0,
						Enumerable.Range(i - 5, 6).Select(j => incidentRateSeries.ItemList[j]).Sum(v => v.YValue ?? 0) / 6m, incidentRateSeries.ItemList[i].Text));

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

			var plants = SQMModelMgr.SelectPlantList(entities, companyID, 0).Where(p => p.STATUS == "A");
			var plantIDs = plants.Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Include("PLANT").Where(d => plantIDs.Contains(d.PLANT_ID) &&
				EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			decimal totalManHours2YearsAgo = 0;
			decimal totalManHoursPreviousYear = 0;
			decimal totalManHoursYTD = 0;
			decimal totalIncidents2YearsAgo = 0;
			decimal totalIncidentsPreviousYear = 0;
			decimal totalIncidentsYTD = 0;

			var data = new List<dynamic>();
			var businessOrgCounts = new Dictionary<decimal?, int>();
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

				if (!businessOrgCounts.ContainsKey(plant.BUS_ORG_ID))
					businessOrgCounts.Add(plant.BUS_ORG_ID, 0);
				++businessOrgCounts[plant.BUS_ORG_ID];

				data.Add(new
				{
					BusOrgID = plant.BUS_ORG_ID,
					BusinessUnit = plant.BUS_ORG_ID == 99 ? " " : entities.BUSINESS_ORG.First(bu => bu.BUS_ORG_ID == plant.BUS_ORG_ID).ORG_NAME,
					Plant = plant.DISP_PLANT_NAME,
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
				if (busOrgID == 99 || businessOrgCounts[busOrgID] == 1) // AAM or business unit with 1 plant
					continue;

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

			int annualizeMonths = DateTime.Today.Month == 1 ? 12 : DateTime.Today.Month - 1;

			var plants = SQMModelMgr.SelectPlantList(entities, companyID, 0).Where(p => p.STATUS == "A");
			var plantIDs = plants.Select(p => p.PLANT_ID).ToList();
			var allData = entities.EHS_DATA.Include("EHS_MEASURE").Where(d => plantIDs.Contains(d.PLANT_ID) &&
				EntityFunctions.TruncateTime(d.DATE) >= startOf2YearsAgo.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextYear.Date).ToList();

			decimal totalIncidentsPreviousYear = 0;
			decimal totalIncidentsYTD = 0;

			var data = new List<dynamic>();
			var businessOrgCounts = new Dictionary<decimal?, int>();
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

				decimal incidentsAnnualized = Math.Round(incidentsYTD * 12 / annualizeMonths);

				if (!businessOrgCounts.ContainsKey(plant.BUS_ORG_ID))
					businessOrgCounts.Add(plant.BUS_ORG_ID, 0);
				++businessOrgCounts[plant.BUS_ORG_ID];

				data.Add(new
				{
					BusOrgID = plant.BUS_ORG_ID,
					BusinessUnit = plant.BUS_ORG_ID == 99 ? " " : entities.BUSINESS_ORG.First(bu => bu.BUS_ORG_ID == plant.BUS_ORG_ID).ORG_NAME,
					Plant = plant.DISP_PLANT_NAME,
					RecPreviousYear = incidentsPreviousYear,
					RecYTD = incidentsYTD,
					RecAnnualized = incidentsAnnualized,
					PercentChange = incidentsPreviousYear == 0 ? 0 : (incidentsAnnualized - incidentsPreviousYear) / incidentsPreviousYear
				});
			}

			data = data.OrderBy(d => d.BusOrgID).ToList();

			foreach (string businessUnit in data.Select(d => d.BusinessUnit).Distinct().ToList())
			{
				var last = data.Last(d => d.BusinessUnit == businessUnit);
				if (last.BusOrgID == 99 || businessOrgCounts[last.BusOrgID] == 1) // AAM or business unit with 1 plant
					continue;

				int lastIndex = data.IndexOf(last);
				var allPlantsForBU = data.Where(d => d.BusinessUnit == businessUnit);
				decimal incidentsPreviousYear = allPlantsForBU.Sum(d => (decimal)d.RecPreviousYear);
				decimal incidentsYTD = allPlantsForBU.Sum(d => (decimal)d.RecYTD);
				decimal incidentsAnnualized = Math.Round(incidentsYTD * 12 / annualizeMonths);
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

			var totalIncidentsAnnualized = Math.Round(totalIncidentsYTD * 12 / annualizeMonths);

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

		static dynamic PullBalancedScorecardData(PSsqmEntities entities, decimal companyID, int year)
		{
			var data = new List<dynamic>();
			var businessLocs = SQMModelMgr.SelectBusinessLocationList(companyID, 0, true);
			var businessOrgCounts = businessLocs.GroupBy(l => l.Plant.BUS_ORG_ID).ToDictionary(l => l.Key, l => l.Count());

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
					ItemType = "Frequency Rate",
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
					if (busOrgID != 99 && businessOrgCounts[busOrgID] != 1)
					{
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
								ItemType = "Frequency Rate",
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
						ItemType = "Frequency Rate",
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

			return new
			{
				data,
				year
			};
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

				this.divExportAll.Style.Add("width", gaugeDef.Width + "px");
				this.divExport.Style.Add("width", gaugeDef.Width + "px");

				dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, this.rmypYear.SelectedDate.Value.Year,
					DataToUse.Metrics);

				this.UpdateCharts(data);
			}
		}

		static HtmlGenericControl CreatePageBreakDiv()
		{
			var div = new HtmlGenericControl("div");
			div.Style.Add("page-break-after", "always");
			div.Style.Add("padding", "1px");
			return div;
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
				var YTD = (data.data as List<Data>)[12];

				var uclPyramid = this.LoadControl<Ucl_PerformanceReport_Pyramid>("~/Include/Ucl_PerformanceReport_Pyramid.ascx");
				uclPyramid.FirstAidCasesPreviousYear = data.previousYTD.Fatalities;
				uclPyramid.Fatalities = YTD.Fatalities;
				uclPyramid.LostTimeCasesPreviousYear = data.previousYTD.Frequency;
				uclPyramid.LostTimeCases = YTD.Frequency;
				uclPyramid.RecordableInjuriesPreviousYear = data.previousYTD.Incidents;
				uclPyramid.RecordableInjuries = YTD.Incidents;
				uclPyramid.FirstAidCasesPreviousYear = data.previousYTD.FirstAid;
				uclPyramid.FirstAidCases = YTD.FirstAid;
				uclPyramid.NearMissesPreviousYear = data.previousYTD.NearMisses;
				uclPyramid.NearMisses = YTD.NearMisses;
				uclPyramid.JSAsSeries = data.jsasSeries;
				uclPyramid.JSAsTrendSeries = data.jsasTrendSeries;
				uclPyramid.JSAsTarget = data.jsasTarget;
				uclPyramid.SafetyTrainingHoursSeries = data.safetyTrainingHoursSeries;
				uclPyramid.SafetyTrainingHoursTrendSeries = data.safetyTrainingHoursTrendSeries;
				this.pnlPyramidOutput.Controls.Add(uclPyramid);
			}
			if (trirBusiness)
			{
				gaugeDef.Height = 395;
				int count = 0, len = (data as List<dynamic>).Count;
				foreach (var businessOrgData in data)
				{
					gaugeDef.Title = businessOrgData.name.ToUpper() + " TOTAL RECORDABLE INCIDENT RATE";
					gaugeDef.Target = new PERSPECTIVE_TARGET()
					{
						TARGET_VALUE = businessOrgData.incidentRateTarget,
						DESCR_SHORT = "Target"
					};
					var series = new List<GaugeSeries>() { businessOrgData.incidentRateSeries, businessOrgData.incidentRateTrendSeries };
					WebSiteCommon.SetScale(gaugeDef, series);
					var container = new HtmlGenericControl("div");
					container.Attributes.Add("class", "chartMarginTop");
					this.uclChart.CreateMultiLineChart(gaugeDef, series, container);
					this.pnlTRIRBusinessOutput.Controls.Add(container);

					++count;
					if (count != len && (count % 4) == 0)
						this.pnlTRIRBusinessOutput.Controls.Add(CreatePageBreakDiv());
				}
			}
			if (trirPlant)
			{
				var uclTRIRPlant = this.LoadControl<Ucl_PerformanceReport_TRIRPlant>("~/Include/Ucl_PerformanceReport_TRIRPlant.ascx");
				uclTRIRPlant.Year = data.year;
				uclTRIRPlant.Data = data.data;
				this.pnlTRIRPlantOutput.Controls.Add(uclTRIRPlant);
			}
			if (recPlant)
			{
				var uclRecPlant = this.LoadControl<Ucl_PerformanceReport_RecPlant>("~/Include/Ucl_PerformanceReport_RecPlant.ascx");
				uclRecPlant.Year = data.year;
				uclRecPlant.Data = data.data;
				this.pnlRecPlantOutput.Controls.Add(uclRecPlant);
			}
			if (balancedScorecard)
			{
				var uclBalancedScorecord = this.LoadControl<Ucl_PerformanceReport_BalancedScorecard>("~/Include/Ucl_PerformanceReport_BalancedScorecard.ascx");
				uclBalancedScorecord.Year = data.year;
				uclBalancedScorecord.Width = gaugeDef.Width;
				uclBalancedScorecord.Data = data.data;
				this.pnlBalancedScorecardOutput.Controls.Add(uclBalancedScorecord);
			}
			if (metrics)
			{
				var uclMetrics = this.LoadControl<Ucl_PerformanceReport_Metrics>("~/Include/Ucl_PerformanceReport_Metrics.ascx");
				uclMetrics.Title = data.title;
				uclMetrics.Year = data.year;
				uclMetrics.Data = data.data;
				uclMetrics.IncidentRateSeries = data.incidentRateSeries;
				uclMetrics.IncidentRateTrendSeries = data.incidentRateTrendSeries;
				uclMetrics.IncidentRateTarget = data.incidentRateTarget;
				uclMetrics.FrequencyRateSeries = data.frequencyRateSeries;
				uclMetrics.SeverityRateSeries = data.severityRateSeries;
				uclMetrics.OrdinalTypeSeries = data.ordinalTypeSeries;
				uclMetrics.OrdinalBodyPartSeries = data.ordinalBodyPartSeries;
				uclMetrics.OrdinalRootCauseSeries = data.ordinalRootCauseSeries;
				uclMetrics.OrdinalTenureSeries = data.ordinalTenureSeries;
				uclMetrics.OrdinalDaysToCloseSeries = data.ordinalDaysToCloseSeries;
				uclMetrics.JSAsSeries = data.jsasSeries;
				uclMetrics.JSAsTrendSeries = data.jsasTrendSeries;
				uclMetrics.JSAsTarget = data.jsasTarget;
				uclMetrics.SafetyTrainingHoursSeries = data.safetyTrainingHoursSeries;
				uclMetrics.SafetyTrainingHoursTrendSeries = data.safetyTrainingHoursTrendSeries;
				this.pnlMetricsOutput.Controls.Add(uclMetrics);
			}
		}

		protected void radAjaxManager_AjaxRequest(object sender, AjaxRequestEventArgs e)
		{
			this.divExportAll.Controls.Clear();
			this.divExportAll.Style["width"] = gaugeDef.Width + "px";

			gaugeDef.Target = smallGaugeDef.Target = null;

			decimal companyID = decimal.Parse(this.hfCompanyID.Value);
			int year = this.rmypYear.SelectedDate.Value.Year;

			if (e.Argument == "pyramid")
			{
				var pnlPyramidOutput = new Panel();

				dynamic data = PullData(this.entities, "-1", companyID, year, DataToUse.Pyramid);
				var YTD = (data.data as List<Data>)[12];

				var uclPyramid = this.LoadControl<Ucl_PerformanceReport_Pyramid>("~/Include/Ucl_PerformanceReport_Pyramid.ascx");
				uclPyramid.FirstAidCasesPreviousYear = data.previousYTD.Fatalities;
				uclPyramid.Fatalities = YTD.Fatalities;
				uclPyramid.LostTimeCasesPreviousYear = data.previousYTD.Frequency;
				uclPyramid.LostTimeCases = YTD.Frequency;
				uclPyramid.RecordableInjuriesPreviousYear = data.previousYTD.Incidents;
				uclPyramid.RecordableInjuries = YTD.Incidents;
				uclPyramid.FirstAidCasesPreviousYear = data.previousYTD.FirstAid;
				uclPyramid.FirstAidCases = YTD.FirstAid;
				uclPyramid.NearMissesPreviousYear = data.previousYTD.NearMisses;
				uclPyramid.NearMisses = YTD.NearMisses;
				uclPyramid.JSAsSeries = data.jsasSeries;
				uclPyramid.JSAsTrendSeries = data.jsasTrendSeries;
				uclPyramid.JSAsTarget = data.jsasTarget;
				uclPyramid.SafetyTrainingHoursSeries = data.safetyTrainingHoursSeries;
				uclPyramid.SafetyTrainingHoursTrendSeries = data.safetyTrainingHoursTrendSeries;
				pnlPyramidOutput.Controls.Add(uclPyramid);

				this.divExportAll.Controls.Add(pnlPyramidOutput);
			}
			else if (e.Argument == "trirBusinessUnit")
			{
				var pnlTRIRBusinessOutput = new Panel();

				dynamic data = PullTRIRByBusinessUnit(this.entities, companyID, year);

				gaugeDef.Height = 395;
				int count = 0, len = (data as List<dynamic>).Count;
				foreach (var businessOrgData in data)
				{
					gaugeDef.Title = businessOrgData.name.ToUpper() + " TOTAL RECORDABLE INCIDENT RATE";
					gaugeDef.Target = new PERSPECTIVE_TARGET()
					{
						TARGET_VALUE = businessOrgData.incidentRateTarget,
						DESCR_SHORT = "Target"
					};
					var series = new List<GaugeSeries>() { businessOrgData.incidentRateSeries, businessOrgData.incidentRateTrendSeries };
					WebSiteCommon.SetScale(gaugeDef, series);
					var container = new HtmlGenericControl("div");
					container.Attributes.Add("class", "chartMarginTop");
					this.uclChart.CreateMultiLineChart(gaugeDef, series, container);
					pnlTRIRBusinessOutput.Controls.Add(container);

					++count;
					if (count != len && (count % 4) == 0)
						pnlTRIRBusinessOutput.Controls.Add(CreatePageBreakDiv());
				}

				this.divExportAll.Controls.Add(pnlTRIRBusinessOutput);
			}
			else if (e.Argument == "trirPlant")
			{
				var pnlTRIRPlantOutput = new Panel();

				dynamic data = PullTRIRByPlant(this.entities, companyID, year);

				var uclTRIRPlant = this.LoadControl<Ucl_PerformanceReport_TRIRPlant>("~/Include/Ucl_PerformanceReport_TRIRPlant.ascx");
				uclTRIRPlant.Year = data.year;
				uclTRIRPlant.Data = data.data;
				pnlTRIRPlantOutput.Controls.Add(uclTRIRPlant);

				this.divExportAll.Controls.Add(pnlTRIRPlantOutput);
			}
			else if (e.Argument == "recPlant")
			{
				var pnlRecPlantOutput = new Panel();

				dynamic data = PullRecByPlant(this.entities, companyID, year);

				var uclRecPlant = this.LoadControl<Ucl_PerformanceReport_RecPlant>("~/Include/Ucl_PerformanceReport_RecPlant.ascx");
				uclRecPlant.Year = data.year;
				uclRecPlant.Data = data.data;
				pnlRecPlantOutput.Controls.Add(uclRecPlant);

				this.divExportAll.Controls.Add(pnlRecPlantOutput);
			}
			else if (e.Argument == "balancedScorecard")
			{
				var pnlBalancedScorecardOutput = new Panel();

				dynamic data = PullBalancedScorecardData(this.entities, companyID, year);

				var uclBalancedScorecord = this.LoadControl<Ucl_PerformanceReport_BalancedScorecard>("~/Include/Ucl_PerformanceReport_BalancedScorecard.ascx");
				uclBalancedScorecord.Year = data.year;
				uclBalancedScorecord.Width = gaugeDef.Width;
				uclBalancedScorecord.Data = data.data;
				pnlBalancedScorecardOutput.Controls.Add(uclBalancedScorecord);

				this.divExportAll.Controls.Add(pnlBalancedScorecardOutput);
			}
			else if (e.Argument.StartsWith("metrics_"))
			{
				var pnlMetricsOutput = new Panel();

				// Total Corp.
				dynamic data = PullData(this.entities, e.Argument.Substring(8), companyID, year, DataToUse.Metrics);

				var uclMetrics = this.LoadControl<Ucl_PerformanceReport_Metrics>("~/Include/Ucl_PerformanceReport_Metrics.ascx");
				uclMetrics.Title = data.title;
				uclMetrics.Year = data.year;
				uclMetrics.Data = data.data;
				uclMetrics.IncidentRateSeries = data.incidentRateSeries;
				uclMetrics.IncidentRateTrendSeries = data.incidentRateTrendSeries;
				uclMetrics.IncidentRateTarget = data.incidentRateTarget;
				uclMetrics.FrequencyRateSeries = data.frequencyRateSeries;
				uclMetrics.SeverityRateSeries = data.severityRateSeries;
				uclMetrics.OrdinalTypeSeries = data.ordinalTypeSeries;
				uclMetrics.OrdinalBodyPartSeries = data.ordinalBodyPartSeries;
				uclMetrics.OrdinalRootCauseSeries = data.ordinalRootCauseSeries;
				uclMetrics.OrdinalTenureSeries = data.ordinalTenureSeries;
				uclMetrics.OrdinalDaysToCloseSeries = data.ordinalDaysToCloseSeries;
				uclMetrics.JSAsSeries = data.jsasSeries;
				uclMetrics.JSAsTrendSeries = data.jsasTrendSeries;
				uclMetrics.JSAsTarget = data.jsasTarget;
				uclMetrics.SafetyTrainingHoursSeries = data.safetyTrainingHoursSeries;
				uclMetrics.SafetyTrainingHoursTrendSeries = data.safetyTrainingHoursTrendSeries;
				pnlMetricsOutput.Controls.Add(uclMetrics);

				this.divExportAll.Controls.Add(pnlMetricsOutput);
			}
		}

		[WebMethod]
		public static List<string> GetMetricsList(decimal companyID)
		{
			var metricsList = new List<string>() { "-1" };
			var businessLocs = SQMModelMgr.SelectBusinessLocationList(companyID, 0, true);
			var businessOrgCounts = businessLocs.GroupBy(l => l.Plant.BUS_ORG_ID).ToDictionary(l => l.Key, l => l.Count());
			decimal? busOrgID = null;
			foreach (var businessLoc in businessLocs.OrderBy(l => l.Plant.BUS_ORG_ID).ThenBy(l => l.Plant.PLANT_NAME))
			{
				if (businessLoc.Plant.BUS_ORG_ID != busOrgID)
				{
					busOrgID = businessLoc.Plant.BUS_ORG_ID;
					if (busOrgID != 99 && businessOrgCounts[busOrgID] != 1)
						metricsList.Add("BU" + busOrgID);
				}
				metricsList.Add(businessLoc.Plant.PLANT_ID.ToString());
			}
			return metricsList;
		}
	}
}
