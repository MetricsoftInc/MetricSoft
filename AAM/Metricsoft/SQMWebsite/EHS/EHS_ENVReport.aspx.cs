using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class EHS_ENVReport : SQMBasePage
	{

		private decimal[] PlantIDS
		{
			get { return ViewState["LocalPlantArray"] == null ? null : (decimal[])ViewState["LocalPlantArray"]; }
			set { ViewState["LocalPlantArray"] = value; }
		}
		private string PlantSelect
		{
			get { return ViewState["LocalPlantSelect"] == null ? "" : (string)ViewState["LocalPlantSelect"]; }
			set { ViewState["LocalPlantSelect"] = value; }
		}
		private List<PERSPECTIVE_VIEW> LocalViewList
		{
			get { return ViewState["LocalViewList"] == null ? null : (List<PERSPECTIVE_VIEW>)ViewState["LocalViewList"]; }
			set { ViewState["LocalViewList"] = value; }
		}

		protected object LocalDataset()
		{
			if (SessionManager.CurrentDataset != null)
				return SessionManager.CurrentDataset;
			else
				return null;
		}
		protected object SetLocalDataset(object dataset)
		{
			SessionManager.CurrentDataset = dataset;
			return LocalDataset();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.lblDateFrom.Text = Resources.LocalizedText.From + ": ";
			this.lblDateTo.Text = Resources.LocalizedText.To + ": ";
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("EHS", 2, true, true, "");
				}
				SetupPage();
			}
			else
			{
				if (!string.IsNullOrEmpty(hfViewOption.Value))
				{
					try
					{
						decimal viewID = Convert.ToDecimal(hfViewOption.Value);
						if (viewID > 5)
							lnkOpt_Click(viewID, null);
					}
					catch
					{
						;
					}
				}
				DisplayOptions();
			}
		}

		private void SetupPage()
		{
			hfViewOption.Value = "";
			radDateFrom.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
			radDateTo.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
			radDateFrom.MinDate = new DateTime(2001, 1, 1); radDateTo.MinDate = new DateTime(2001, 1, 1);
			radDateFrom.MaxDate = SessionManager.UserContext.LocalTime.AddMonths(1); radDateTo.MaxDate = SessionManager.UserContext.LocalTime.AddMonths(1);
			radDateFrom.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(-11);
			radDateTo.SelectedDate = SessionManager.UserContext.LocalTime;

			PlantIDS = new decimal[0] { };
			SetLocalDataset(null);

			List<BusinessLocation> locationList = SQMModelMgr.UserAccessibleLocations(SessionManager.UserContext.Person, SQMModelMgr.SelectBusinessLocationList(1, 0, true), true, false, "").Where(l => l.Plant.TRACK_EW_DATA != null && l.Plant.TRACK_EW_DATA == true).ToList();

			if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone") == false)
			{
				mnuPlantSelect.Visible = true;
				SQMBasePage.SetLocationList(mnuPlantSelect, locationList.Where(l => l.Plant.TRACK_EW_DATA != null && l.Plant.TRACK_EW_DATA == true).ToList(), SessionManager.UserContext.HRLocation.Plant.PLANT_ID, "select a location ...", "TOP", false);
				mnuPlantSelect_Select(null, null);
			}
			else
			{
				ddlPlantSelect.Visible = lblPlantSelect.Visible = true;
				SQMBasePage.SetLocationList(ddlPlantSelect, locationList, SessionManager.UserContext.HRLocation.Plant.PLANT_ID);
			}

			uclExport.BindExport("", false, "");

			DisplayOptions();
		}

		private void DisplayOptions()
		{
			if (LocalViewList == null || LocalViewList.Count == 0)
				LocalViewList = ViewModel.SelectViewList("ERPT", 1);
			if (LocalViewList != null)
			{
				foreach (PERSPECTIVE_VIEW view in LocalViewList)
				{
					LinkButton lnk = new LinkButton();
					lnk.Text = view.VIEW_NAME;
					lnk.Style.Add("margin-left", "12px");
					lnk.ID = view.VIEW_ID.ToString();
					lnk.CommandArgument = view.VIEW_ID.ToString();
					lnk.OnClientClick = "SetViewOption('" + view.VIEW_ID.ToString() + "');";
					lnk.CssClass = ViewModel.GetViewImageStyle(view);
					pnlOptions.Controls.Add(lnk);
				}
			}
		}

		protected void selectsChanged_Event(object sender, EventArgs e)
		{
			divInputs.Visible = divView.Visible = divGHG.Visible = divExport.Visible = false;
			lblReportTitle.Text = hfViewOption.Value = "";
		}

		protected void mnuPlantSelect_Select(object sender, EventArgs e)
		{
			if (mnuPlantSelect.SelectedItem != null && !string.IsNullOrEmpty(mnuPlantSelect.SelectedItem.Value))
			{
				try
				{
					PlantSelect = mnuPlantSelect.Items[0].Text = mnuPlantSelect.SelectedItem.Text;
					PlantIDS = new decimal[1] { Convert.ToDecimal(mnuPlantSelect.SelectedItem.Value) };
				}
				catch
				{
					PlantIDS = new decimal[0] { };
				}
			}
			else
			{
				PlantIDS = new decimal[0] { };
			}

			divInputs.Visible = divView.Visible = divGHG.Visible = divExport.Visible = false;
			lblReportTitle.Text = hfViewOption.Value = "";
		}

		protected void lnkOpt_Click(object sender, EventArgs e)
		{
			int status = 0;
			divInputs.Visible = divView.Visible = divGHG.Visible = divExport.Visible = false;
			lblReportTitle.Text = "";

			if (ddlPlantSelect.Visible)
			{
				PlantIDS = Array.ConvertAll(SQMBasePage.GetComboBoxSelectedValues(ddlPlantSelect).ToArray(), new Converter<string, decimal>(decimal.Parse));
				PlantSelect = ddlPlantSelect.SelectedItem.Text;
			}

			if (PlantIDS.Length == 0 || radDateFrom.SelectedDate == null || radDateTo.SelectedDate == null || radDateFrom.SelectedDate > radDateTo.SelectedDate)
			{
				lblReportTitle.CssClass = "promptAlert";
				lblReportTitle.Text = hfCriteriaErr.Value;
				return;
			}

			if (sender is decimal)
			{
				status = GeneratePerspectiveView((decimal)sender);
			}
			else
			{
				LinkButton lnk = (LinkButton)sender;
				lblReportTitle.CssClass = "labelTitle";
				lblReportTitle.Text = PlantSelect + " - " + lnk.Text;
				hfViewOption.Value = lnk.CommandArgument;
				switch (lnk.CommandArgument)
				{
					case "1":
						status = GenerateInputsReport();
						break;
					case "2":
						break;
					case "5":
						status = GenerateEmissionsReport();
						break;
					default:
						break;
				}
			}

			if (status >= 0)
			{
				divExport.Visible = true;
			}
		}

		protected int GenerateInputsReport()
		{
			int status = 0;
			EHSProfile profile = new EHSProfile().Load(PlantIDS[0], false, true);
			SessionManager.TempObject = profile;

			uclInputs.BindPeriodList(uclInputs.SelectProfilePeriodList(profile, (DateTime)radDateFrom.SelectedDate, (DateTime)radDateTo.SelectedDate).OrderByDescending(l => l.PeriodDate).ToList());
			divInputs.Visible = true;
			return status;
		}

		protected int GenerateEmissionsReport()
		{
			int status = 0;
			try
			{
				DateTime fromDate = (DateTime)radDateFrom.SelectedDate;
				fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);
				DateTime toDate = (DateTime)radDateTo.SelectedDate;
				toDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));

				SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "E", fromDate, toDate, PlantIDS);
				//metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
				//CalcsResult rslt = metricMgr.CalcsMethods(PlantIDS, "E", "ghg|co2,ch4,n2o", "gwp100|sum", 22, (int)EHSCalcsCtl.SeriesOrder.YearMeasurePlant);
				metricMgr.Load(DateIntervalType.span, DateSpanOption.SelectRange);
				CalcsResult rslt = metricMgr.CalcsMethods(PlantIDS, "E", "ghg|co2,ch4,n2o", "gwp100|sum", 22, (int)EHSCalcsCtl.SeriesOrder.MeasurePlant);
				EHSModel.GHGResultList ghgTable = (EHSModel.GHGResultList)rslt.ResultObj;
				uclGHG.BindGHGReport(ghgTable);
				divGHG.Visible = true;

				SetLocalDataset(metricMgr);
			}
			catch { status = -1; }

			return status;
		}

		protected int GeneratePerspectiveView(decimal viewID)
		{
			int status = 0;
			//hfViewOption.Value = viewID.ToString();
			divView.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
			divView.Controls.Clear();

			PERSPECTIVE_VIEW view = LocalViewList.Where(v => v.VIEW_ID == viewID).FirstOrDefault();
			if (view == null)
			{
				lblReportTitle.CssClass = "promptAlert";
				lblReportTitle.Text = hfViewOpenErr.Value;
				return -1;
			}

			lblReportTitle.CssClass = "labelTitle";
			lblReportTitle.Text = PlantSelect + " - " + view.VIEW_DESC;

			DateTime fromDate = (DateTime)radDateFrom.SelectedDate;
			fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);
			DateTime toDate = (DateTime)radDateTo.SelectedDate;
			toDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));

			SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "E", fromDate, toDate, ViewModel.AddFromYear(view), PlantIDS);
			metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);

			foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.OrderBy(i => i.ITEM_SEQ).ToList())
			{
				if (vi.STATUS != "I")
				{
					GaugeDefinition cfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
					uclView.CreateControl((SQMChartType)cfg.ControlType, cfg, metricMgr.CalcsMethods(PlantIDS, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER), divView);
				}
			}

			SetLocalDataset(metricMgr);
			divView.Visible = true;
			return status;
		}

		protected void lnkExport_Click(object sender, EventArgs e)
		{
			GenerateDataExport();
		}

		private void GenerateDataExport()
		{
			uclProgress.BindProgressDisplay(100, "Exporting...");
			DateTime fromDate = (DateTime)radDateFrom.SelectedDate;
			fromDate = new DateTime(fromDate.Year, fromDate.Month, 1);
			DateTime toDate = (DateTime)radDateTo.SelectedDate;
			toDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));

			if (divInputs.Visible)
			{
				uclProgress.UpdateDisplay(2, 50, "Exporting...");
				uclExport.ExportProfileInputsExcel(entities, PlantIDS[0].ToString(), fromDate, toDate);
			}
			else
			{
				uclProgress.UpdateDisplay(2, 50, "Exporting...");
				if (LocalDataset() is SQMMetricMgr)
				{
					uclExport.GenerateExportHistoryExcel(entities, ((SQMMetricMgr)LocalDataset()).ehsCtl.MetricHst, false);
				}
				else
				{
					SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "E", fromDate, toDate, 0, PlantIDS);
					metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
				}
			}

			uclProgress.ProgressComplete();
		}

	}
}