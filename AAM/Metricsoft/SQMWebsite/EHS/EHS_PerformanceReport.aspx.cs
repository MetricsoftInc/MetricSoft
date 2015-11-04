using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website.EHS
{
	public partial class EHS_PerformanceReport : SQMBasePage
	{
		class Data
		{
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
				return new Data()
				{
					ManHours = a.ManHours + b.ManHours,
					Incidents = a.Incidents + b.Incidents,
					Frequency = a.Frequency + b.Frequency,
					Restricted = a.Restricted + b.Restricted,
					Severity = a.Severity + b.Severity,
					FirstAid = a.FirstAid + b.FirstAid,
					Leadership = a.Leadership + b.Leadership,
					JSAs = a.JSAs + b.JSAs,
					SafetyTraining = a.SafetyTraining + b.SafetyTraining,
					OrdinalType = a.OrdinalType.Keys.Concat(b.OrdinalType.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalType.ContainsKey(key) ? a.OrdinalType[key] : 0) + (b.OrdinalType.ContainsKey(key) ? b.OrdinalType[key] : 0)
					}).ToDictionary(x => x.key, x => x.value),
					OrdinalBodyPart = a.OrdinalBodyPart.Keys.Concat(b.OrdinalBodyPart.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalBodyPart.ContainsKey(key) ? a.OrdinalBodyPart[key] : 0) + (b.OrdinalBodyPart.ContainsKey(key) ? b.OrdinalBodyPart[key] : 0)
					}).ToDictionary(x => x.key, x => x.value),
					OrdinalRootCause = a.OrdinalRootCause.Keys.Concat(b.OrdinalRootCause.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalRootCause.ContainsKey(key) ? a.OrdinalRootCause[key] : 0) + (b.OrdinalRootCause.ContainsKey(key) ? b.OrdinalRootCause[key] : 0)
					}).ToDictionary(x => x.key, x => x.value),
					OrdinalTenure = a.OrdinalTenure.Keys.Concat(b.OrdinalTenure.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalTenure.ContainsKey(key) ? a.OrdinalTenure[key] : 0) + (b.OrdinalTenure.ContainsKey(key) ? b.OrdinalTenure[key] : 0)
					}).ToDictionary(x => x.key, x => x.value),
					OrdinalDaysToClose = a.OrdinalDaysToClose.Keys.Concat(b.OrdinalDaysToClose.Keys).Distinct().Select(key => new
					{
						key = key,
						value = (a.OrdinalDaysToClose.ContainsKey(key) ? a.OrdinalDaysToClose[key] : 0) + (b.OrdinalDaysToClose.ContainsKey(key) ? b.OrdinalDaysToClose[key] : 0)
					}).ToDictionary(x => x.key, x => x.value)
				};
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

		static Dictionary<string, decimal> GetOrdinalData(IQueryable<EHS_DATA_ORD> allOrdData, dynamic types)
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

		static dynamic PullData(PSsqmEntities entities, string plantID, decimal companyID, int year)
		{
			decimal plantID_dec = plantID.StartsWith("BU") ? decimal.Parse(plantID.Substring(2)) : decimal.Parse(plantID);
			var plantIDs = plantID.StartsWith("BU") ? SQMModelMgr.SelectPlantList(entities, companyID, plantID_dec).Select(p => p.PLANT_ID).ToList() : new List<decimal>();

			var data = new List<Data>();
			Data YTD = null;

			var incidentRateSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 2, year), "")
			{
				DisplayLabels = true
			};
			var frequencyRateSeries = new List<GaugeSeries>();
			var severityRateSeries = new List<GaugeSeries>();
			var jsasSeries = new GaugeSeries(0, string.Format("{0} - {1}", year - 1, year), "");
			var safetyTrainingHoursSeries = new GaugeSeries(0, "Total Safety Training Hours", "");

			// Ordinal types
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			for (int y = year - 2; y <= year; ++y)
			{
				YTD = new Data();
				var frequencyRateSeriesYear = new GaugeSeries(y - year - 2, y.ToString(), "");
				var severityRateSeriesYear = new GaugeSeries(y - year - 2, y.ToString(), "");

				for (int i = 1; i < 13; ++i)
				{
					var startOfMonth = new DateTime(y, i, 1);
					var startOfNextMonth = startOfMonth.AddMonths(1);

					var allData = from d in entities.EHS_DATA
								  where EntityFunctions.TruncateTime(d.DATE) >= startOfMonth.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextMonth.Date &&
								  (plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(d.PLANT_ID) : d.PLANT_ID == plantID_dec))
								  select d;
					var allOrdData = from o in entities.EHS_DATA_ORD
									 where EntityFunctions.TruncateTime(o.EHS_DATA.DATE) >= startOfMonth.Date && EntityFunctions.TruncateTime(o.EHS_DATA.DATE) < startOfNextMonth.Date &&
									 (plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(o.EHS_DATA.PLANT_ID) : o.EHS_DATA.PLANT_ID == plantID_dec))
									 select o;

					var manHours = y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
						allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var incidents = y < year || (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
						allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var frequency = y > year - 2 ? allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20005" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var severity = y > year - 2 ? allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;

					var monthData = new Data()
					{
						Month = startOfMonth.ToString("MMMM"),
						TRIR = manHours == 0 ? 0 : incidents * 200000 / manHours,
						FrequencyRate = manHours == 0 ? 0 : frequency * 200000 / manHours,
						SeverityRate = manHours == 0 ? 0 : severity * 200000 / manHours,
						ManHours = manHours,
						Incidents = incidents,
						Frequency = frequency,
						Restricted = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						Severity = severity,
						FirstAid = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						Leadership = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S30002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						JSAs = y > year - 2 && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S40003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						SafetyTraining = y == year && (y == DateTime.Today.Year ? startOfMonth.Month <= DateTime.Today.Month : true) ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S50001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						OrdinalType = y == year ? GetOrdinalData(allOrdData, types) : new Dictionary<string, decimal>(),
						OrdinalBodyPart = y == year ? GetOrdinalData(allOrdData, bodyParts) : new Dictionary<string, decimal>(),
						OrdinalRootCause = y == year ? GetOrdinalData(allOrdData, rootCauses) : new Dictionary<string, decimal>(),
						OrdinalTenure = y == year ? GetOrdinalData(allOrdData, tenures) : new Dictionary<string, decimal>(),
						OrdinalDaysToClose = y == year ? GetOrdinalData(allOrdData, daysToCloses) : new Dictionary<string, decimal>()
					};

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

				if (y > year - 2)
				{
					frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours, "YTD"));
					severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - year - 2, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Severity * 200000 / YTD.ManHours, "YTD"));

					frequencyRateSeries.Add(frequencyRateSeriesYear);
					severityRateSeries.Add(severityRateSeriesYear);
				}
			}

			YTD.Month = "YTD";
			YTD.TRIR = YTD.ManHours == 0 ? 0 : YTD.Incidents * 200000 / YTD.ManHours;
			YTD.FrequencyRate = YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours;
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

			return new
			{
				data,
				incidentRateSeries,
				incidentRateTrendSeries,
				frequencyRateSeries,
				severityRateSeries,
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
				safetyTrainingHoursSeries
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

				this.divExport.Style.Add("width", gaugeDef.Width + "px");
				this.divTRIR.Style.Add("width", gaugeDef.Width + "px");
				this.divFrequencyRate.Style.Add("width", gaugeDef.Width + "px");
				this.divSeverityRate.Style.Add("width", gaugeDef.Width + "px");

				dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, this.rmypYear.SelectedDate.Value.Year);

				this.rgReport.DataSource = data.data;
				this.rgReport.DataBind();

				this.UpdateCharts(data);
			}
		}

		bool didFirstHeader = false;

		protected void rgReport_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header && !this.didFirstHeader)
			{
				e.Item.Cells[e.Item.Cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.rmypYear.SelectedDate.Value.Year.ToString();
				this.didFirstHeader = true;
			}
			if ((e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem) && (e.Item.DataItem as Data).Month == "YTD")
				e.Item.BackColor = Color.FromArgb(255, 255, 153);
		}

		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, decimal.Parse(this.hfCompanyID.Value), this.rmypYear.SelectedDate.Value.Year);

			this.rgReport.DataSource = data.data;
			this.rgReport.DataBind();

			this.UpdateCharts(data);
		}

		void UpdateCharts(dynamic data)
		{
			gaugeDef.Title = "TOTAL RECORDABLE INCIDENT RATE";
			var series = new List<GaugeSeries>() { data.incidentRateSeries, data.incidentRateTrendSeries };
			SetScale(gaugeDef, series);
			this.uclChart.CreateMultiLineChart(gaugeDef, series, this.divTRIR);

			var calcsResult = new CalcsResult().Initialize();

			gaugeDef.Title = "FREQUENCY RATE";
			SetScale(gaugeDef, data.frequencyRateSeries);
			calcsResult.metricSeries = data.frequencyRateSeries;
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, calcsResult, this.divFrequencyRate);

			gaugeDef.Title = "SEVERITY RATE";
			SetScale(gaugeDef, data.severityRateSeries);
			calcsResult.metricSeries = data.severityRateSeries;
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, calcsResult, this.divSeverityRate);

			if (this.rmypYear.SelectedDate.Value.Year != DateTime.Today.Year || (data.data as List<Data>).Last().TRIR == 0)
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
			series = new List<GaugeSeries>() { data.jsasSeries, data.jsasTrendSeries };
			SetScale(smallGaugeDef, series);
			this.uclChart.CreateMultiLineChart(smallGaugeDef, series, this.divJSAsAndAudits);

			smallGaugeDef.Title = "Safety Training Hours";
			series = new List<GaugeSeries>() { data.safetyTrainingHoursSeries };
			SetScale(smallGaugeDef, series);
			this.uclChart.CreateMultiLineChart(smallGaugeDef, series, this.divSafetyTrainingHours);
		}
	}
}
