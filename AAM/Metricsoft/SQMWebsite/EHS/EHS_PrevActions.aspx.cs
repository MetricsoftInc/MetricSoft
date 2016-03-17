using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;

namespace SQM.Website
{
	public partial class EHS_PrevActions : SQMBasePage
	{
		protected enum DisplayState
		{
			IncidentList,
			IncidentNotificationNew,
			IncidentNotificationEdit,
			IncidentReportEdit
		}

		decimal companyId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclExport.OnExportClick += ExportClick;
			uclIncidentList.OnIncidentClick += IncidentClick;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			/*
			this.Title = Resources.LocalizedText.EHSIncidents;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblInspectionType.Text = Resources.LocalizedText.IncidentType + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ": ";
			this.lblToDate.Text = Resources.LocalizedText.To + ":";
			*/
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			//RadPersistenceManager1.PersistenceSettings.AddSetting(ddlChartType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclIncidentList.IncidentListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbInspectionType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbRecommendType);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool createIncidentAccess = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.prevent);
			rbNew.Visible = createIncidentAccess;

			if (IsPostBack)
			{
				RadPersistenceManager1.SaveState();

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayIncidents":
								UpdateDisplayState(DisplayState.IncidentList, 0);
								break;
							default:
								break;
						}
					}
					SessionManager.ClearReturns();
				}
			}
			else
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("EHS", 2, true, true, "");
				}

				if (!string.IsNullOrEmpty(Request.QueryString["r"]))   // incident Record id is supplied from email notification
				{
					string targetRec = Request.QueryString["r"];
					decimal targetRecID;
					if (decimal.TryParse(targetRec, out targetRecID))
					{
						UpdateDisplayState(DisplayState.IncidentNotificationEdit, targetRecID);
					}
				}
				else if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
				{
					// from inbox ?
					decimal targetRecID;
					if (decimal.TryParse(SessionManager.ReturnObject.ToString(), out targetRecID))
					{
						SessionManager.ClearReturns();
						UpdateDisplayState(DisplayState.IncidentNotificationEdit, targetRecID);
					}

				}
				else
				{
					DisplayNonPostback();
				}
			}
		}

		protected void DisplayNonPostback()
		{
			SetupPage();

			try
			{
				RadPersistenceManager1.LoadState();
			}
			catch
			{
			}

			if (SessionManager.ReturnStatus == null || SessionManager.ReturnStatus != true)
				//if ( SessionManager.ReturnObject == null)
				SearchIncidents();      // suppress list when invoking page from inbox

			Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
			if (ucl != null)
			{
				ucl.BindDocumentSelect("EHS", 2, true, true, "");
			}

		}

		protected void UpdateDisplayState(DisplayState state, decimal incidentID)
		{
			string key;
			switch (state)
			{
				case DisplayState.IncidentList:
					SearchIncidents();
					SessionManager.ClearReturns();
					break;
				case DisplayState.IncidentNotificationNew:
					SessionManager.ClearReturns();
					if (rddlNewActionType.SelectedItem != null)
					{
						INCIDENT newIncident = new INCIDENT();
						newIncident.DETECT_PLANT_ID = Convert.ToDecimal(ddlActionLocation.SelectedValue);
						newIncident.ISSUE_TYPE_ID = (decimal)EHSIncidentTypeId.PreventativeAction;
						newIncident.ISSUE_TYPE = rddlNewActionType.SelectedValue;
						SessionManager.ReturnObject = newIncident;
						SessionManager.ReturnStatus = true;
						Response.Redirect("/EHS/EHS_PrevActionForm.aspx");
					}
					break;
				case DisplayState.IncidentNotificationEdit:
					SessionManager.ClearReturns();
					INCIDENT theIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentID);
					if (theIncident != null)
					{
						SessionManager.ReturnObject = theIncident;
						SessionManager.ReturnStatus = true;
						string stepCmd = "";
						if (!string.IsNullOrEmpty(Request.QueryString["s"]))   // from inbox/calendar assume this is a task assignment. direct to corrective actions page
						{
							stepCmd = ("?s=" +  Request.QueryString["s"]);
						}
						Response.Redirect("/EHS/EHS_PrevActionForm.aspx"+stepCmd);
					}
					break;
				default:
					SessionManager.ClearReturns();
					break;
			}
		}

		private void IncidentClick(decimal incidentID)
		{
			UpdateDisplayState(DisplayState.IncidentNotificationEdit, incidentID);
		}

		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList), 0);

				List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[3] { "IQ_81", "IQ_82", "IQ_83" }, 1);
				rcbInspectionType = SQMBasePage.SetComboBoxItemsFromXLAT(rcbInspectionType, xlatList.Where(l => l.XLAT_GROUP == "IQ_81" && l.STATUS == "A").ToList(), "SHORT");
				rcbRecommendType = SQMBasePage.SetComboBoxItemsFromXLAT(rcbRecommendType, xlatList.Where(l => l.XLAT_GROUP == "IQ_83").ToList(), "SHORT");
				rcbStatusSelect.SelectedValue = "A";

				// work-around for problem w/ radwindow combobox not retaining items created/set from a basepage method ?
				SQMBasePage.SetLocationList(ddlActionLocation, locationList, 0, true);
				ddlActionLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";

				rddlNewActionType.DataSource = xlatList.Where(l=> l.XLAT_GROUP == "IQ_81" && l.STATUS == "A").ToList();
				rddlNewActionType.DataTextField = "DESCRIPTION_SHORT";
				rddlNewActionType.DataValueField = "XLAT_CODE";
				rddlNewActionType.DataBind();
			}
			divIncidentList.Visible = true;
			pnlChartSection.Style.Add("display", "none");
			lblChartType.Visible = ddlChartType.Visible = false;

			SQMBasePage.SetRadDateCulture(dmFromDate, "");
			SQMBasePage.SetRadDateCulture(dmToDate, "");

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			dmFromDate.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(-11);
			dmToDate.SelectedDate = SessionManager.UserContext.LocalTime;

			//lblViewEHSRezTitle.Text = GetLocalResourceObject("lblViewEHSRezTitleResource1.Text").ToString();
			//lblPageInstructions.Text = GetLocalResourceObject("lblPageInstructionsResource1.Text").ToString();
			//lblStatus.Text = "Incident Status:";
			//rbNew.Text = GetLocalResourceObject("rbNewResource1.Text").ToString();
			lblIncidentDate.Visible = true;
			lblInspectionDate.Visible = false;

			SETTINGS sets = SQMSettings.GetSetting("EHS", "INCIDENTSEARCHFROM");
			if (sets != null)
			{
				try
				{
					string[] args = sets.VALUE.Split('-');
					if (args.Length > 1)
					{
						dmFromDate.SelectedDate = new DateTime(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
					}
					else
					{
						dmFromDate.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(Convert.ToInt32(args[0]) * -1);
					}
				}
				catch { }
			}

			// lookup charts defined for this module & app context
			PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, "HSCA", "HSCA", 0);
			if (view != null)
			{
				ddlChartType.Items.Clear();
				ddlChartType.Items.Add(new RadComboBoxItem("", ""));
				foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.Where(l => l.STATUS != "I").OrderBy(l => l.ITEM_SEQ).ToList())
				{
					RadComboBoxItem item = new RadComboBoxItem();
					item.Text = vi.TITLE;
					item.Value = vi.ITEM_SEQ.ToString();
					item.ImageUrl = ViewModel.GetViewItemImageURL(vi);
					ddlChartType.Items.Add(item);
				}
			}

			if (UserContext.GetMaxScopePrivilege(SysScope.prevent) <= SysPriv.action)
				uclExport.Visible = true;
			else
				uclExport.Visible = false;
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			string script = "function f(){OpenNewActionWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnNewActionCreate_Click(object sender, EventArgs e)
		{
			if (ddlActionLocation.SelectedItem != null && rddlNewActionType.SelectedItem != null)
			{
				UpdateDisplayState(DisplayState.IncidentNotificationNew, 0);
			}
			else
			{
				SearchIncidents();
			}
		}
		protected void btnNewActionCancel_Click(object sender, EventArgs e)
		{
			SearchIncidents();
		}

		#region incidentaging

		protected void btnActionsSearch_Click(object sender, EventArgs e)
		{
			//SearchIncidents();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}

		protected void btnFindingsSearchClick(object sender, EventArgs e)
		{
			SearchIncidents();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}

		protected void lnkCloseDetails(object sender, EventArgs e)
		{
			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;
			if (ddlChartType.SelectedValue != "")
				ddlChartTypeChange(null, null);
		}

		protected void lnkCloseChart(object sender, EventArgs e)
		{
			ddlChartType.SelectedValue = "";
			lnkChartClose.Visible = lnkPrint.Visible = false;
			ddlChartTypeChange(null, null);
		}

		protected void ddlChartTypeChange(object sender, EventArgs e)
		{
			divChart.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
			if (ddlChartType.SelectedValue == "" || HSCalcs().ehsCtl.IncidentHst == null || HSCalcs().ehsCtl.IncidentHst.Count == 0)
			{
				pnlChartSection.Style.Add("display", "none");
				lnkChartClose.Visible = lnkPrint.Visible = false;
			}
			else
			{
				PERSPECTIVE_VIEW view = null;
				divChart.Controls.Clear();

				view = ViewModel.LookupView(entities, "HSCA", "HSCA", 0);

				if (view != null)
				{
					PERSPECTIVE_VIEW_ITEM vi = view.PERSPECTIVE_VIEW_ITEM.Where(i => i.ITEM_SEQ.ToString() == ddlChartType.SelectedValue).FirstOrDefault();
					if (vi != null)
					{
						GaugeDefinition ggCfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
						ggCfg.Position = null;
						HSCalcs().ehsCtl.SetCalcParams(vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, (int)vi.SERIES_ORDER).IncidentSeries((EHSCalcsCtl.SeriesOrder)vi.SERIES_ORDER, SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToArray(), new DateTime(1900, 1, 1), SessionManager.UserContext.LocalTime.AddYears(100), HSCalcs().ehsCtl.GetIncidentTopics());
						uclChart.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, HSCalcs().ehsCtl.Results, divChart);
						pnlChartSection.Style.Add("display", "inline");
						lnkChartClose.Visible = lnkPrint.Visible = true;
						// return;
					}
				}
			}
		}

		private void SearchIncidents()
		{
			string selectedValue = "";
			// work-around for rad persistence manager being cleared upon re-build ??
			if (ddlPlantSelect.CheckedItems.Count == 0)
			{
				foreach (RadComboBoxItem item in ddlPlantSelect.Items)
					item.Checked = true;
			}
			if (rcbInspectionType.CheckedItems.Count == 0)
			{
				foreach (RadComboBoxItem item in rcbInspectionType.Items)
					item.Checked = true;
			}
			if (rcbStatusSelect.SelectedItem == null)
				rcbStatusSelect.SelectedIndex = 0;

			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate);
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

			List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();

			if (HSCalcs() == null)
			{
				foreach (RadComboBoxItem item in rcbInspectionType.Items)
					item.Checked = true;
			}

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			HSCalcs().ObjAny = cbShowImage.Checked;

			var typeList = new List<decimal>();
			typeList = EHSIncidentMgr.SelectPreventativeTypeList(SessionManager.PrimaryCompany().COMPANY_ID).Select(l => l.INCIDENT_TYPE_ID).ToList();

			List<string> inspectionCatetoryList = new List<string>();
			inspectionCatetoryList.AddRange(rcbInspectionType.Items.Where(c => c.Checked).Select(c => c.Value).ToList());
			List<string> recommendationTypeList = new List<string>();
			recommendationTypeList.AddRange(rcbRecommendType.Items.Where(c => c.Checked).Select(c => c.Value).ToList());
			List<string> statusList = SQMBasePage.GetComboBoxCheckedItems(rcbStatusSelect).Select(l => l.Value).ToList();
			HSCalcs().ehsCtl.SelectPreventativeList(plantIDS, typeList, inspectionCatetoryList, recommendationTypeList, fromDate, toDate, statusList, cbShowImage.Checked, cbCreatedByMe.Checked ? SessionManager.UserContext.Person.PERSON_ID : 0);

			if (HSCalcs().ehsCtl.IncidentHst != null)
			{
				uclIncidentList.BindPreventativeListRepeater(HSCalcs().ehsCtl.IncidentHst.OrderByDescending(x => x.Incident.INCIDENT_DT).ToList(), "EHS", cbShowImage.Checked);
			}

			if (HSCalcs().ehsCtl.IncidentHst != null && HSCalcs().ehsCtl.IncidentHst.Count > 0)
				lblChartType.Visible = ddlChartType.Visible = true;

			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;

			if (ddlChartType.SelectedValue != "")
				lnkCloseChart(null, null);
		}

		#endregion


		#region conversion
		protected void btnConvert_Click(object sender, EventArgs e)
		{
			// convert preventative actions to new format
			int status = 0;
			PSsqmEntities ctx = new PSsqmEntities();

			status = ctx.ExecuteStoreCommand("update incident set INCFORM_LAST_STEP_COMPLETED = 130 where incident_id in (select i.incident_id from incident i, INCIDENT_ANSWER a where i.ISSUE_TYPE_ID = 13 and  i.INCIDENT_ID = a.INCIDENT_ID and a.INCIDENT_QUESTION_ID = 93 and a.ANSWER_VALUE = 'In Progress')");

			status = ctx.ExecuteStoreCommand("update incident set INCFORM_LAST_STEP_COMPLETED = 135 where incident_id in (select i.incident_id from incident i, INCIDENT_ANSWER a where i.ISSUE_TYPE_ID = 13 and  i.INCIDENT_ID = a.INCIDENT_ID and a.INCIDENT_QUESTION_ID = 93 and a.ANSWER_VALUE = 'Closed')");

			status = ctx.ExecuteStoreCommand("update incident set INCFORM_LAST_STEP_COMPLETED = 155 where incident_id in (select i.incident_id from incident i, INCIDENT_ANSWER a where i.ISSUE_TYPE_ID = 13 and  i.INCIDENT_ID = a.INCIDENT_ID and a.INCIDENT_QUESTION_ID = 88 and a.ANSWER_VALUE = 'Yes')");

			status = ctx.ExecuteStoreCommand("update incident set INCFORM_LAST_STEP_COMPLETED = 155 where incident_id in (select i.incident_id from incident i, INCIDENT_ANSWER a where i.ISSUE_TYPE_ID = 13 and  i.INCIDENT_ID = a.INCIDENT_ID and a.INCIDENT_QUESTION_ID = 88 and a.ANSWER_VALUE = 'Awaiting Funding')");

			status = ctx.ExecuteStoreCommand("update incident set INCFORM_LAST_STEP_COMPLETED = 100 where ISSUE_TYPE_ID = 13 and INCFORM_LAST_STEP_COMPLETED is null");
		}
		#endregion

		#region formatting helpers

		protected string DisplayProblemIcon(decimal incidentId)
		{
			string content = "";

			var occurs = entities.PROB_OCCUR.Where(p => p.INCIDENT_ID == incidentId);
			if (occurs.Count() > 0)
				content = "<img src=\"/images/ico-8d.png\" alt=\"Problem Case has been created for this incident\" />";

			return content;
		}

		protected string GetPlantName(decimal incidentId)
		{
			string content = "";
			content = EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(incidentId);
			return content;
		}

		protected string TruncatedText(string text, int maxLength)
		{
			if (text.Length > maxLength)
				return text.Substring(0, maxLength - 2) + "…";
			else
				return text;
		}

		#endregion

		// manage current session object  (formerly was page static variable)
		SQMMetricMgr HSCalcs()
		{
			if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is SQMMetricMgr)
				return (SQMMetricMgr)SessionManager.CurrentObject;
			else
				return null;
		}
		SQMMetricMgr SetHSCalcs(SQMMetricMgr hscalcs)
		{
			SessionManager.CurrentObject = hscalcs;
			return HSCalcs();
		}

		#region export

		protected void ExportClick(string cmd)
		{
			uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}

		protected void lnkExportClick(object sender, EventArgs e)
		{
			uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}
		#endregion

	}

}
