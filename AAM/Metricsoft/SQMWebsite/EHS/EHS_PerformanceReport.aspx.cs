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
	public partial class EHS_PerformanceReport : Page
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
					SafetyTraining = a.SafetyTraining + b.SafetyTraining
				};
			}
		}

		static GaugeDefinition gaugeDef = new GaugeDefinition()
		{
			Height = 500,
			Width = 1500,
			DisplayLegend = true,
			LegendPosition = ChartLegendPosition.Right,
			LegendBackgroundColor = Color.FromArgb(254, 254, 254),
			OnLoad = "chartOnLoad"
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

		static dynamic PullData(PSsqmEntities entities, string plantID, decimal companyID)
		{
			decimal plantID_dec = plantID.StartsWith("BU") ? decimal.Parse(plantID.Substring(2)) : decimal.Parse(plantID);
			var plantIDs = plantID.StartsWith("BU") ? SQMModelMgr.SelectPlantList(entities, companyID, plantID_dec).Select(p => p.PLANT_ID).ToList() : new List<decimal>();

			var data = new List<Data>();
			Data YTD = null;

			var incidentRateSeries = new GaugeSeries(0, "2013 - 2015", "")
			{
				DisplayLabels = true
			};
			var frequencyRateSeries = new List<GaugeSeries>();
			var severityRateSeries = new List<GaugeSeries>();

			for (int y = 2013; y < 2016; ++y)
			{
				YTD = new Data();
				var frequencyRateSeriesYear = new GaugeSeries(y - 2013, y.ToString(), "");
				var severityRateSeriesYear = new GaugeSeries(y - 2013, y.ToString(), "");

				for (int i = 1; i < 13; ++i)
				{
					var startOfMonth = new DateTime(y, i, 1);
					var startOfNextMonth = startOfMonth.AddMonths(1);

					var allData = from d in entities.EHS_DATA
								  where EntityFunctions.TruncateTime(d.DATE) >= startOfMonth.Date && EntityFunctions.TruncateTime(d.DATE) < startOfNextMonth.Date &&
								  (plantID_dec == -1 ? true : (plantID.StartsWith("BU") ? plantIDs.Contains(d.PLANT_ID) : d.PLANT_ID == plantID_dec))
								  select d;

					var manHours = y < 2015 || startOfMonth.Month <= DateTime.Today.Month ?
						allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var incidents = y < 2015 || startOfMonth.Month <= DateTime.Today.Month ?
						allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20004" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var frequency = y > 2013 ? allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20005" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;
					var severity = y > 2013 ? allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0;

					var monthData = new Data()
					{
						Month = startOfMonth.ToString("MMMM"),
						TRIR = manHours == 0 ? 0 : incidents * 200000 / manHours,
						FrequencyRate = manHours == 0 ? 0 : frequency * 200000 / manHours,
						SeverityRate = manHours == 0 ? 0 : severity * 200000 / manHours,
						ManHours = manHours,
						Incidents = incidents,
						Frequency = frequency,
						Restricted = y == 2015 && startOfMonth.Month <= DateTime.Today.Month ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S60003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						Severity = severity,
						FirstAid = y == 2015 && startOfMonth.Month <= DateTime.Today.Month ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S20003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						Leadership = y == 2015 && startOfMonth.Month <= DateTime.Today.Month ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S30002" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						JSAs = y == 2015 && startOfMonth.Month <= DateTime.Today.Month ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S40003" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0,
						SafetyTraining = y == 2015 && startOfMonth.Month <= DateTime.Today.Month ?
							allData.Where(d => d.EHS_MEASURE.MEASURE_CD == "S50001" && d.VALUE.HasValue).Sum(d => d.VALUE) ?? 0 : 0
					};

					if (y < 2015 || startOfMonth.Month <= DateTime.Today.Month)
						incidentRateSeries.ItemList.Add(new GaugeSeriesItem(0, 0, 0, monthData.TRIR, monthData.Month));
					if (y > 2013)
					{
						frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - 2013, 0, 0, monthData.FrequencyRate, monthData.Month));
						severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - 2013, 0, 0, monthData.SeverityRate, monthData.Month));
					}

					if (y == 2015)
						data.Add(monthData);
					YTD += monthData;
				}

				if (y > 2013)
				{
					frequencyRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - 2013, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours, "YTD"));
					severityRateSeriesYear.ItemList.Add(new GaugeSeriesItem(y - 2013, 0, 0, YTD.ManHours == 0 ? 0 : YTD.Severity * 200000 / YTD.ManHours, "YTD"));

					frequencyRateSeries.Add(frequencyRateSeriesYear);
					severityRateSeries.Add(severityRateSeriesYear);
				}
			}

			YTD.Month = "YTD";
			YTD.TRIR = YTD.ManHours == 0 ? 0 : YTD.Incidents * 200000 / YTD.ManHours;
			YTD.FrequencyRate = YTD.ManHours == 0 ? 0 : YTD.Frequency * 200000 / YTD.ManHours;
			YTD.SeverityRate = YTD.ManHours == 0 ? 0 : YTD.Severity * 200000 / YTD.ManHours;

			data.Add(YTD);

			return new
			{
				data,
				incidentRateSeries,
				frequencyRateSeries,
				severityRateSeries
			};
		}

		int year;
		PSsqmEntities entities;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.year = 2015;
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

				this.divExport.Style.Add("width", gaugeDef.Width + "px");
				this.divTRIR.Style.Add("width", gaugeDef.Width + "px");
				this.divFrequencyRate.Style.Add("width", gaugeDef.Width + "px");
				this.divSeverityRate.Style.Add("width", gaugeDef.Width + "px");

				dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, SessionManager.UserContext.HRLocation.Company.COMPANY_ID);

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
				e.Item.Cells[e.Item.Cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.year.ToString();
				this.didFirstHeader = true;
			}
			if ((e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem) && (e.Item.DataItem as Data).Month == "YTD")
				e.Item.BackColor = Color.FromArgb(255, 255, 153);
		}

		protected void rcbPlant_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			dynamic data = PullData(this.entities, this.rcbPlant.SelectedValue, decimal.Parse(this.hfCompanyID.Value));

			this.rgReport.DataSource = data.data;
			this.rgReport.DataBind();

			this.UpdateCharts(data);
		}

		void UpdateCharts(dynamic data)
		{
			gaugeDef.Title = "TOTAL RECORDABLE INCIDENT RATE";
			SetScale(gaugeDef, new List<GaugeSeries>() { data.incidentRateSeries });
			this.uclChart.CreateMultiLineChart(gaugeDef, new List<GaugeSeries>() { data.incidentRateSeries }, this.divTRIR);

			gaugeDef.Title = "FREQUENCY RATE";
			SetScale(gaugeDef, data.frequencyRateSeries);
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, new CalcsResult() { metricSeries = data.frequencyRateSeries }, this.divFrequencyRate);

			gaugeDef.Title = "SEVERITY RATE";
			SetScale(gaugeDef, data.severityRateSeries);
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, gaugeDef, new CalcsResult() { metricSeries = data.severityRateSeries }, this.divSeverityRate);
		}
	}
}
