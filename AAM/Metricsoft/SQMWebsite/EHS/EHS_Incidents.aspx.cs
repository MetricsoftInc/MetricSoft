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
	public partial class EHS_Incidents : SQMBasePage
	{
		protected enum DisplayState
		{
			IncidentList,
			IncidentNotificationNew,
			IncidentNotificationEdit,
			IncidentReportEdit
		}

		// Mode should be "incident" (standard) or "prevent" (RMCAR)
		public IncidentMode Mode 
		{
			get { return ViewState["Mode"] == null ? IncidentMode.Incident : (IncidentMode)ViewState["Mode"]; }
			set	{ ViewState["Mode"] = value; }
		}
		public bool isDirected
		{
			get { return ViewState["isDirected"] == null ? false : (bool)ViewState["isDirected"]; }
			set { ViewState["isDirected"] = value; }
		}

		protected AccessMode accessLevel;
		decimal companyId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclExport.OnExportClick += ExportClick;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbFindingsSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlChartType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclIncidentList.IncidentListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbIncidentType);
		  
			if (Request.QueryString["mode"] != null)
			{
				string mode = Request.QueryString["mode"].ToString().ToLower();
				if (!string.IsNullOrEmpty(mode))
				{
					if (mode == "incident")
						this.Mode = IncidentMode.Incident;
					else if (mode == "prevent")
						this.Mode = IncidentMode.Prevent;
				}
			}

			


			uclIncidentForm.Mode = this.Mode;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

			accessLevel = UserContext.CheckAccess("EHS", "");
			if (accessLevel < AccessMode.Update)
				rbNew.Visible = false;

			if (IsPostBack)
			{
				if (!uclIncidentForm.IsEditContext)
					RadPersistenceManager1.SaveState();

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayIncidents":
								UpdateDisplayState(DisplayState.IncidentList);
								break;

							case "Notification":
								//UpdateDisplayState(DisplayState.IncidentNotificationEdit); 
								//UpdateDisplayState(DisplayState.IncidentReportEdit);
								uclIncidentForm.EditIncidentId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.IncidentNotificationEdit);
								if (isDirected)
								{
									rbNew.Visible = false;
									uclIncidentForm.EnableReturnButton(false);
								}
								break;

							case "Report":
								uclIncidentForm.EditIncidentId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.IncidentReportEdit);
								break;
						}
					}
					SessionManager.ClearReturns();
				}
			}
			else
			{
				if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
				{
					try
					{
						// from inbox
						DisplayNonPostback();
						SessionManager.ReturnRecordID = Convert.ToDecimal(SessionManager.ReturnObject.ToString());
						SessionManager.ReturnObject = "Notification";
						SessionManager.ReturnStatus = true;
						isDirected = true;

						StringBuilder sbScript = new StringBuilder();
						ClientScriptManager cs = Page.ClientScript;

						sbScript.Append("<script language='JavaScript' type='text/javascript'>\n");
						sbScript.Append("<!--\n");
						sbScript.Append(cs.GetPostBackEventReference(this, "PBArg") + ";\n");
						sbScript.Append("// -->\n");
						sbScript.Append("</script>\n");

						cs.RegisterStartupScript(this.GetType(), "AutoPostBackScript", sbScript.ToString());
					}
					catch
					{
						// not a number, parse as type
						DisplayNonPostback();
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

			if (SessionManager.ReturnStatus == null  ||  SessionManager.ReturnStatus != true)
			//if ( SessionManager.ReturnObject == null)
				SearchIncidents();      // suppress list when invoking page from inbox

			Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
			if (ucl != null)
			{
				ucl.BindDocumentSelect("EHS", 2, true, true, "");
			}

		}

		protected void UpdateDisplayState(DisplayState state)
		{
			switch (state)
			{
				case DisplayState.IncidentList:
					SearchIncidents();
					uclIncidentForm.Visible = false;
					break;

				case DisplayState.IncidentNotificationNew:
					divIncidentList.Visible = false;
					uclIncidentForm.Visible = true;
					uclIncidentForm.IsEditContext = false;
					uclIncidentForm.ClearControls();
					uclIncidentForm.CheckForSingleType();
					break;

				case DisplayState.IncidentNotificationEdit:
					divIncidentList.Visible = false;
					uclIncidentForm.CurrentStep = 0;
					uclIncidentForm.IsEditContext = true;
					uclIncidentForm.Visible = true;
					uclIncidentForm.BuildForm();
					break;

				case DisplayState.IncidentReportEdit:
					divIncidentList.Visible = false;
					uclIncidentForm.CurrentStep = 1;
					uclIncidentForm.IsEditContext = true;
					uclIncidentForm.Visible = true;
					uclIncidentForm.BuildForm();
					break;

			}

			SessionManager.ClearReturns();
		}
		
		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList, "EHS", ""), 0);
			 
				rcbStatusSelect.SelectedValue = "A";
				rcbFindingsSelect.FindItemByValue("A").Checked = true;
			}
			divIncidentList.Visible = true;
			pnlChartSection.Style.Add("display", "none");
			lblChartType.Visible = ddlChartType.Visible = false;

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			dmFromDate.SelectedDate = DateTime.Now.AddMonths(-11);
			dmToDate.SelectedDate = DateTime.Now;

			if (Mode == IncidentMode.Incident)
			{
				
				lblViewEHSRezTitle.Text = "Manage Environmental Health &amp; Safety Incidents";
				lblPageInstructions.Text = "Add or update EH&amp;S Incidents below.";
				//lblStatus.Text = "Incident Status:";
				rbNew.Text = "New Incident";
				lblIncidentDate.Visible = true;
				lblInspectionDate.Visible = false;
				phPrevent.Visible = false;
				phIncident.Visible = true;

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
							dmFromDate.SelectedDate = DateTime.Now.AddMonths(Convert.ToInt32(args[0]) * -1);
						}
					}
					catch { }
				}

				foreach (INCIDENT_TYPE ip in EHSIncidentMgr.SelectIncidentTypeList(SessionManager.PrimaryCompany().COMPANY_ID))
				{
					RadComboBoxItem item = new RadComboBoxItem(ip.TITLE, ip.INCIDENT_TYPE_ID.ToString());
					item.Checked = true;
					rcbIncidentType.Items.Add(item);
				}

				// lookup charts defined for this module & app context
				PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);
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
			}
			else if (Mode == IncidentMode.Prevent)
			{
				lblViewEHSRezTitle.Text = "Manage Preventative Actions";
				lblPageInstructions.Text = "Add or update preventative actions below.";
				//lblStatus.Text = "Findings Status:";
				rbNew.Text = "New Preventative Action";
				if (accessLevel < AccessMode.Admin)
					rbNew.Visible = false;
				lblIncidentDate.Visible = false;
				lblInspectionDate.Visible = true;
				phPrevent.Visible = true;
				phIncident.Visible = false;

				SETTINGS sets = SQMSettings.GetSetting("EHS", "ACTIONSEARCHFROM");
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
							dmFromDate.SelectedDate = DateTime.Now.AddMonths(Convert.ToInt32(args[0]) * -1);
						}
					}
					catch { }
				}

				// workaround for persistance mgr not supporting raddate controls
				if (HSCalcs() != null)
				{
					dmFromDate.SelectedDate = HSCalcs().FromDate;
					dmToDate.SelectedDate = HSCalcs().ToDate;
					if (HSCalcs().ObjAny != null && HSCalcs().ObjAny is bool)
						cbShowImage.Checked = (bool)HSCalcs().ObjAny;
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
			}

			if (UserContext.CheckAccess("EHS", "301") >= AccessMode.Plant)
				uclExport.Visible = true;
			else
				uclExport.Visible = false;
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			rbNew.Visible = false;
			UpdateDisplayState(DisplayState.IncidentNotificationNew);
		}

		#region incidentaging
		
		protected void btnIncidentsSearchClick(object sender, EventArgs e)
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
				lnkChartClose.Visible = lnkPrint.Visible =  false;
			}
			else
			{
				PERSPECTIVE_VIEW view = null;
				divChart.Controls.Clear();

				if (Mode == IncidentMode.Prevent)
					view = ViewModel.LookupView(entities, "HSCA", "HSCA", 0);
				else
					view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);

				if (view != null)
				{
					PERSPECTIVE_VIEW_ITEM vi = view.PERSPECTIVE_VIEW_ITEM.Where(i => i.ITEM_SEQ.ToString() == ddlChartType.SelectedValue).FirstOrDefault();
					if (vi != null)
					{
						GaugeDefinition ggCfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
						ggCfg.Position = null;
						HSCalcs().ehsCtl.SetCalcParams(vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, (int)vi.SERIES_ORDER).IncidentSeries((EHSCalcsCtl.SeriesOrder)vi.SERIES_ORDER, SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToArray(), new DateTime(1900, 1, 1), DateTime.Now.AddYears(100), HSCalcs().ehsCtl.GetIncidentTopics());
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
			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate); 
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
		   
			accessLevel = UserContext.CheckAccess("EHS", "");
			List<decimal>  plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();
			
			var typeList = new List<decimal>();
			List<string> statusList = new List<string>();

			if (Mode == IncidentMode.Incident)
			{
				if (HSCalcs() == null)
				{
					foreach (RadComboBoxItem item in rcbIncidentType.Items)
						item.Checked = true;
				}
				typeList = rcbIncidentType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
				selectedValue = rcbStatusSelect.SelectedValue;
			}
			else if (Mode == IncidentMode.Prevent)
			{
				typeList = EHSIncidentMgr.SelectPreventativeTypeList(SessionManager.PrimaryCompany().COMPANY_ID).Select(l => l.INCIDENT_TYPE_ID).ToList();
				statusList = SQMBasePage.GetComboBoxCheckedItems(rcbFindingsSelect).Select(l => l.Value).ToList();
			}

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			HSCalcs().ObjAny = cbShowImage.Checked;

			if (selectedValue == "T")
			{
				HSCalcs().ehsCtl.SelectIncidentList(plantIDS, typeList, fromDate, toDate, "A", false);
				List<EHSIncidentData> actionList = new List<EHSIncidentData>();
				foreach (EHSIncidentData data in HSCalcs().ehsCtl.IncidentHst)
				{
					if (data.EntryList.Where(l=> l.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Create8D && l.ANSWER_VALUE != "Yes").Count() > 0)
				//	if (data.Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Create8D && l.ANSWER_VALUE != "Yes").Count() > 0)
						actionList.Add(data);
				}
				HSCalcs().ehsCtl.IncidentHst = actionList;
				
				if (accessLevel < AccessMode.Admin)
					actionList = HSCalcs().ehsCtl.IncidentHst = (from i in HSCalcs().ehsCtl.IncidentHst where i.Incident.ISSUE_TYPE_ID != 10 select i).ToList();

				if (actionList != null)
					uclIncidentList.BindIncidentActionList(actionList, "EHS");
			}
			else
			{
				if (Mode == IncidentMode.Incident)
					HSCalcs().ehsCtl.SelectIncidentList(plantIDS, typeList, fromDate, toDate, selectedValue, cbShowImage.Checked);
				else if (Mode == IncidentMode.Prevent)
				{
					List<string> inspectionCatetoryList = new List<string>();
					inspectionCatetoryList.AddRange(rcbInspectCatetory.Items.Where(c => c.Checked).Select(c => c.Value).ToList());
					List<string> recommendationTypeList = new List<string>();
					recommendationTypeList.AddRange(rcbRecommendationType.Items.Where(c => c.Checked).Select(c => c.Value).ToList());
					HSCalcs().ehsCtl.SelectPreventativeList(plantIDS, typeList, inspectionCatetoryList, recommendationTypeList, fromDate, toDate, statusList, cbShowImage.Checked);
				}
				
				if (accessLevel < AccessMode.Admin)
					HSCalcs().ehsCtl.IncidentHst = (from i in HSCalcs().ehsCtl.IncidentHst where i.Incident.ISSUE_TYPE_ID != 10 select i).ToList();

				if (HSCalcs().ehsCtl.IncidentHst != null)
				{
					HSCalcs().ehsCtl.IncidentHst.OrderByDescending(x => x.Incident.INCIDENT_DT);
					if (Mode == IncidentMode.Incident)
						uclIncidentList.BindIncidentListRepeater(HSCalcs().ehsCtl.IncidentHst, "EHS", cbShowImage.Checked);
					else if (Mode == IncidentMode.Prevent)
						uclIncidentList.BindPreventativeListRepeater(HSCalcs().ehsCtl.IncidentHst, "EHS", cbShowImage.Checked);
				}
			}

			if (HSCalcs().ehsCtl.IncidentHst != null && HSCalcs().ehsCtl.IncidentHst.Count > 0)
				lblChartType.Visible = ddlChartType.Visible = true;

			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;

			if (ddlChartType.SelectedValue != "")
				ddlChartTypeChange(null, null);

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
			if (Mode == IncidentMode.Prevent)
				uclExport.GeneratePreventativeActionExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
			else
				uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}
		protected void lnkExportClick(object sender, EventArgs e)
		{
			uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}
		#endregion

	}

}
