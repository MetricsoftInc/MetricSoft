using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website
{
	public partial class Ucl_PerformanceReport_Pyramid : UserControl
	{
		public decimal FatalitiesPreviousYear { get; set; }
		public decimal Fatalities { get; set; }
		public decimal LostTimeCasesPreviousYear { get; set; }
		public decimal LostTimeCases { get; set; }
		public decimal RecordableInjuriesPreviousYear { get; set; }
		public decimal RecordableInjuries { get; set; }
		public decimal FirstAidCasesPreviousYear { get; set; }
		public decimal FirstAidCases { get; set; }
		public decimal NearMissesPreviousYear { get; set; }
		public decimal NearMisses { get; set; }
		public GaugeSeries JSAsSeries { get; set; }
		public GaugeSeries JSAsTrendSeries { get; set; }
		public decimal JSAsTarget { get; set; }
		public GaugeSeries SafetyTrainingHoursSeries { get; set; }
		public GaugeSeries SafetyTrainingHoursTrendSeries { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			int annualizeMonths = DateTime.Today.Month == 1 ? 12 : DateTime.Today.Month - 1;

			var left = (EHS.EHS_PerformanceReport.gaugeDef.Width - 420 - this.pyramid.Width.ToPixels()) / 2;
			this.pyramid.Style.Add("left", left + "px");
			this.pyramid.Fatalities = this.Fatalities;
			this.pyramid.LostTimeCases = this.LostTimeCases;
			this.pyramid.RecordableInjuries = this.RecordableInjuries;
			this.pyramid.FirstAidCases = this.FirstAidCases;
			this.pyramid.NearMisses = this.NearMisses;

			var rowHeight = this.pyramid.Height.Divide(5).ToPixels() * 0.996m;
			var halfWidth = this.pyramid.Width.Divide(2).ToPixels();
			this.pyramidTable.Style.Add("left", (halfWidth + left) + "px");
			this.pyramidTable_column1.Style.Add("width", (halfWidth + 20) + "px");
			this.pyramidTable_columnAnnualized.InnerText = "Annualized " + DateTime.Today.Year;
			this.pyramidTable_columnPreviousYear.InnerText = (DateTime.Today.Year - 1).ToString();

			this.pyramidTable_fatalitiesRow.Style.Add("height", rowHeight + "px");
			this.pyramidTable_fatalitiesYTD.InnerText = this.Fatalities.ToString();
			var fatalitiesAnnualized = Math.Round(this.Fatalities * 12 / annualizeMonths);
			this.pyramidTable_fatalitiesAnnualized.InnerText = fatalitiesAnnualized.ToString();
			this.pyramidTable_fatalitiesPreviousYear.InnerText = this.FatalitiesPreviousYear.ToString();
			var fatalitiesVariance = this.FatalitiesPreviousYear == 0 ? 0 : (fatalitiesAnnualized - this.FatalitiesPreviousYear) / this.FatalitiesPreviousYear;
			this.pyramidTable_fatalitiesVariance.InnerText = fatalitiesVariance.ToString("P1");
			this.pyramidTable_fatalitiesVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (fatalitiesVariance > 0 ? "Bad" : "Good"));

			this.pyramidTable_lostTimeRow.Style.Add("height", rowHeight + "px");
			this.pyramidTable_lostTimeYTD.InnerText = this.LostTimeCases.ToString();
			var lostTimeAnnualized = Math.Round(this.LostTimeCases * 12 / annualizeMonths);
			this.pyramidTable_lostTimeAnnualized.InnerText = lostTimeAnnualized.ToString();
			this.pyramidTable_lostTimePreviousYear.InnerText = this.LostTimeCasesPreviousYear.ToString();
			var lostTimeVariance = this.LostTimeCasesPreviousYear == 0 ? 0 : (lostTimeAnnualized - this.LostTimeCasesPreviousYear) / this.LostTimeCasesPreviousYear;
			this.pyramidTable_lostTimeVariance.InnerText = lostTimeVariance.ToString("P1");
			this.pyramidTable_lostTimeVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (lostTimeVariance > 0 ? "Bad" : "Good"));

			this.pyramidTable_recordableRow.Style.Add("height", rowHeight + "px");
			this.pyramidTable_recordableYTD.InnerText = this.RecordableInjuries.ToString();
			var recordableAnnualized = Math.Round(this.RecordableInjuries * 12 / annualizeMonths);
			this.pyramidTable_recordableAnnualized.InnerText = recordableAnnualized.ToString();
			this.pyramidTable_recordablePreviousYear.InnerText = this.RecordableInjuriesPreviousYear.ToString();
			var recordableVariance = this.RecordableInjuriesPreviousYear == 0 ? 0 : (recordableAnnualized - this.RecordableInjuriesPreviousYear) / this.RecordableInjuriesPreviousYear;
			this.pyramidTable_recordableVariance.InnerText = recordableVariance.ToString("P1");
			this.pyramidTable_recordableVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (recordableVariance > 0 ? "Bad" : "Good"));

			this.pyramidTable_firstAidRow.Style.Add("height", rowHeight + "px");
			this.pyramidTable_firstAidYTD.InnerText = this.FirstAidCases.ToString();
			var firstAidAnnualized = Math.Round(this.FirstAidCases * 12 / annualizeMonths);
			this.pyramidTable_firstAidAnnualized.InnerText = firstAidAnnualized.ToString();
			this.pyramidTable_firstAidPreviousYear.InnerText = this.FirstAidCasesPreviousYear.ToString();
			var firstAidVariance = this.FirstAidCasesPreviousYear == 0 ? 0 : (firstAidAnnualized - this.FirstAidCasesPreviousYear) / this.FirstAidCasesPreviousYear;
			this.pyramidTable_firstAidVariance.InnerText = firstAidVariance.ToString("P1");
			this.pyramidTable_firstAidVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (firstAidVariance > 0 ? "Bad" : "Good"));

			this.pyramidTable_nearMissesRow.Style.Add("height", rowHeight + "px");
			this.pyramidTable_nearMissesYTD.InnerText = this.NearMisses.ToString();
			var nearMissesAnnualized = Math.Round(this.NearMisses * 12 / annualizeMonths);
			this.pyramidTable_nearMissesAnnualized.InnerText = nearMissesAnnualized.ToString();
			this.pyramidTable_nearMissesPreviousYear.InnerText = this.NearMissesPreviousYear.ToString();
			var nearMissesVariance = this.NearMissesPreviousYear == 0 ? 0 : (nearMissesAnnualized - this.NearMissesPreviousYear) / this.NearMissesPreviousYear;
			this.pyramidTable_nearMissesVariance.InnerText = nearMissesVariance.ToString("P1");
			this.pyramidTable_nearMissesVariance.Attributes.Add("class", "pyramidTable_cell pyramidTable_variance" + (nearMissesVariance > 0 ? "Bad" : "Good"));

			EHS.EHS_PerformanceReport.gaugeDef.Height = 410;
			EHS.EHS_PerformanceReport.gaugeDef.Title = "Current Indicators - JSAs & Combined Audits";
			EHS.EHS_PerformanceReport.gaugeDef.Target = new PERSPECTIVE_TARGET()
			{
				TARGET_VALUE = this.JSAsTarget,
				DESCR_SHORT = "Target"
			};
			var series = new List<GaugeSeries>() { this.JSAsSeries, this.JSAsTrendSeries };
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.gaugeDef, series);
			this.uclChart.CreateMultiLineChart(EHS.EHS_PerformanceReport.gaugeDef, series, this.divJSAsAndAudits_Pyramid);

			EHS.EHS_PerformanceReport.gaugeDef.Title = "Safety Training Hours";
			EHS.EHS_PerformanceReport.gaugeDef.Target = null;
			series = new List<GaugeSeries>() { this.SafetyTrainingHoursSeries, this.SafetyTrainingHoursTrendSeries };
			WebSiteCommon.SetScale(EHS.EHS_PerformanceReport.gaugeDef, series);
			this.uclChart.CreateMultiLineChart(EHS.EHS_PerformanceReport.gaugeDef, series, this.divSafetyTrainingHours_Pyramid);
		}
	}
}
