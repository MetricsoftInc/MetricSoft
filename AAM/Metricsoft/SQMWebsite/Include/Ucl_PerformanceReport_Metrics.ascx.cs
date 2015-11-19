using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_PerformanceReport_Metrics : UserControl
	{
		public string Title { get; set; }
		public int Year { get; set; }
		public List<EHS.EHS_PerformanceReport.Data> Data { get; set; }
		public GaugeSeries IncidentRateSeries { get; set; }
		public GaugeSeries IncidentRateTrendSeries { get; set; }
		public decimal IncidentRateTarget { get; set; }
		public List<GaugeSeries> FrequencyRateSeries { get; set; }
		public List<GaugeSeries> SeverityRateSeries { get; set; }
		public List<GaugeSeriesItem> OrdinalTypeSeries { get; set; }
		public List<GaugeSeriesItem> OrdinalBodyPartSeries { get; set; }
		public List<GaugeSeriesItem> OrdinalRootCauseSeries { get; set; }
		public List<GaugeSeriesItem> OrdinalTenureSeries { get; set; }
		public List<GaugeSeriesItem> OrdinalDaysToCloseSeries { get; set; }
		public GaugeSeries JSAsSeries { get; set; }
		public GaugeSeries JSAsTrendSeries { get; set; }
		public decimal JSAsTarget { get; set; }
		public GaugeSeries SafetyTrainingHoursSeries { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			this.divTitle.InnerText = this.Title;

			this.rgReport.DataSource = this.Data;
			this.rgReport.DataBind();

			EHS.EHS_PerformanceReport.gaugeDef.Height = 500;
			EHS.EHS_PerformanceReport.gaugeDef.Title = "TOTAL RECORDABLE INCIDENT RATE";
			EHS.EHS_PerformanceReport.gaugeDef.Target = new PERSPECTIVE_TARGET()
			{
				TARGET_VALUE = this.IncidentRateTarget,
				DESCR_SHORT = "Target"
			};
			var series = new List<GaugeSeries>() { this.IncidentRateSeries, this.IncidentRateTrendSeries };
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.gaugeDef, series);
			this.uclChart.CreateMultiLineChart(EHS.EHS_PerformanceReport.gaugeDef, series, this.divTRIR);

			var calcsResult = new CalcsResult().Initialize();

			EHS.EHS_PerformanceReport.gaugeDef.Title = "FREQUENCY RATE";
			EHS.EHS_PerformanceReport.gaugeDef.Target = null;
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.gaugeDef, this.FrequencyRateSeries);
			calcsResult.metricSeries = this.FrequencyRateSeries;
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, EHS.EHS_PerformanceReport.gaugeDef, calcsResult, this.divFrequencyRate);

			EHS.EHS_PerformanceReport.gaugeDef.Title = "SEVERITY RATE";
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.gaugeDef, this.SeverityRateSeries);
			calcsResult.metricSeries = this.SeverityRateSeries;
			this.uclChart.CreateControl(SQMChartType.ColumnChartGrouped, EHS.EHS_PerformanceReport.gaugeDef, calcsResult, this.divSeverityRate);

			if (this.Year != DateTime.Today.Year || this.Data[12].TRIR == 0)
				this.divPie1.Visible = this.divPie2.Visible = this.divPie3.Visible = this.divBreakPie.Visible = false;
			else
			{
				this.divPie1.Visible = this.divPie2.Visible = this.divPie3.Visible = this.divBreakPie.Visible = true;
				this.pieRecordableType.Values = this.OrdinalTypeSeries;
				this.pieRecordableBodyPart.Values = this.OrdinalBodyPartSeries;
				this.pieRecordableRootCause.Values = this.OrdinalRootCauseSeries;
				this.pieRecordableTenure.Values = this.OrdinalTenureSeries;
				this.pieRecordableDaysToClose.Values = this.OrdinalDaysToCloseSeries;
			}

			EHS.EHS_PerformanceReport.smallGaugeDef.Title = "Current Indicators - JSAs & Combined Audits";
			EHS.EHS_PerformanceReport.smallGaugeDef.Target = new PERSPECTIVE_TARGET()
			{
				TARGET_VALUE = this.JSAsTarget,
				DESCR_SHORT = "Target"
			};
			series = new List<GaugeSeries>() { this.JSAsSeries, this.JSAsTrendSeries };
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.smallGaugeDef, series);
			this.uclChart.CreateMultiLineChart(EHS.EHS_PerformanceReport.smallGaugeDef, series, this.divJSAsAndAudits_Metrics);

			EHS.EHS_PerformanceReport.smallGaugeDef.Title = "Safety Training Hours";
			EHS.EHS_PerformanceReport.smallGaugeDef.Target = null;
			series = new List<GaugeSeries>() { this.SafetyTrainingHoursSeries };
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.smallGaugeDef, series);
			this.uclChart.CreateMultiLineChart(EHS.EHS_PerformanceReport.smallGaugeDef, series, this.divSafetyTrainingHours_Metrics);
		}

		bool didFirstHeader_rgReport = false;

		protected void rgReport_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header && !this.didFirstHeader_rgReport)
			{
				e.Item.Cells[e.Item.Cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.Year.ToString();
				this.didFirstHeader_rgReport = true;
			}
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				var month = (e.Item.DataItem as EHS.EHS_PerformanceReport.Data).Month;
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
	}
}
