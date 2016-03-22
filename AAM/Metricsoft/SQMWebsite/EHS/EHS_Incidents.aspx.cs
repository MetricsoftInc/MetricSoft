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

		decimal companyId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclExport.OnExportClick += ExportClick;
			uclIncidentList.OnIncidentClick += IncidentClick;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.EHSIncidents;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblIncidentType.Text = Resources.LocalizedText.IncidentType + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ": ";
			this.lblToDate.Text = Resources.LocalizedText.To + ":";

			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			//RadPersistenceManager1.PersistenceSettings.AddSetting(ddlChartType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclIncidentList.IncidentListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbIncidentType);
		  
			this.Mode = IncidentMode.Incident;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool createIncidentAccess = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);
			if (rbNew.Visible)
				rbNew.Visible = createIncidentAccess;

			if (SessionManager.UserContext.Person.PERSON_ID == 1)
			{
				SETTINGS sets = SQMSettings.GetSetting("EHS","CONVERSION");
				if (sets != null && (sets.VALUE.ToUpper() == "Y" || sets.VALUE.ToUpper() == "TRUE"))
					btnConversion.Visible = true;
			}


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

			if (SessionManager.ReturnStatus == null  ||  SessionManager.ReturnStatus != true)
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
					if (rddlNewIncidentType.SelectedItem != null)
					{
						INCIDENT newIncident = new INCIDENT();
						newIncident.DETECT_PLANT_ID = Convert.ToDecimal(ddlIncidentLocation.SelectedValue);
						newIncident.ISSUE_TYPE_ID = Convert.ToDecimal(rddlNewIncidentType.SelectedValue);
						SessionManager.ReturnObject = newIncident;
						SessionManager.ReturnStatus = true;
						if (EHSIncidentMgr.IsUseCustomForm(Convert.ToDecimal(rddlNewIncidentType.SelectedValue)))
						{
							Response.Redirect("/EHS/EHS_InjuryIllnessForm.aspx");
						}
						else
						{
							Response.Redirect("/EHS/EHS_IncidentForm.aspx");
						}
					}
					break;
				case DisplayState.IncidentNotificationEdit:
					SessionManager.ClearReturns();
					INCIDENT theIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentID);
					if (theIncident != null)
					{
						SessionManager.ReturnObject = theIncident;
						SessionManager.ReturnStatus = true;
						if (EHSIncidentMgr.IsUseCustomForm((decimal)theIncident.ISSUE_TYPE_ID))
						{
							Response.Redirect("/EHS/EHS_InjuryIllnessForm.aspx");
						}
						else
						{
							Response.Redirect("/EHS/EHS_IncidentForm.aspx");
						}
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
			 
				rcbStatusSelect.SelectedValue = "A";
			}
			divIncidentList.Visible = true;
			pnlChartSection.Style.Add("display", "none");
			lblChartType.Visible = ddlChartType.Visible = false;

			SQMBasePage.SetRadDateCulture(dmFromDate, "");
			SQMBasePage.SetRadDateCulture(dmToDate, "");

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			dmFromDate.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(-11);
			dmToDate.SelectedDate = SessionManager.UserContext.LocalTime;

			lblViewEHSRezTitle.Text = GetLocalResourceObject("lblViewEHSRezTitleResource1.Text").ToString();
			lblPageInstructions.Text = GetLocalResourceObject("lblPageInstructionsResource1.Text").ToString();
			//lblStatus.Text = "Incident Status:";
			rbNew.Text = GetLocalResourceObject("rbNewResource1.Text").ToString();
			lblIncidentDate.Visible = true;
			lblInspectionDate.Visible = false;
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
						dmFromDate.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(Convert.ToInt32(args[0]) * -1);
					}
				}
				catch { }
			}

			foreach (INCIDENT_TYPE ip in EHSIncidentMgr.SelectIncidentTypeList(SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.SessionContext.Language().NLS_LANGUAGE))
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

			if (UserContext.GetMaxScopePrivilege(SysScope.incident) <= SysPriv.action)
				uclExport.Visible = true;
			else
				uclExport.Visible = false;
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			List<BusinessLocation> locationList = SessionManager.PlantList;
			locationList = UserContext.FilterPlantAccessList(locationList);
			SQMBasePage.SetLocationList(ddlIncidentLocation, locationList, 0, true);

			var incidentTypeList = EHSIncidentMgr.SelectIncidentTypeList(companyId, SessionManager.SessionContext.Language().NLS_LANGUAGE);
			rddlNewIncidentType.DataSource = incidentTypeList;
			rddlNewIncidentType.DataTextField = "TITLE";
			rddlNewIncidentType.DataValueField = "INCIDENT_TYPE_ID";
			rddlNewIncidentType.DataBind();

			string script = "function f(){OpenNewIncidentWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnNewIncidentCreate_Click(object sender, EventArgs e)
		{
			if (ddlIncidentLocation.SelectedItem != null && rddlNewIncidentType.SelectedItem != null)
			{
				UpdateDisplayState(DisplayState.IncidentNotificationNew, 0);
			}
			else
			{
				SearchIncidents();
			}
		}
		protected void btnNewIncidentCancel_Click(object sender, EventArgs e)
		{
			SearchIncidents(); 
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

				view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);

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
			if (rcbIncidentType.CheckedItems.Count == 0)
			{
				foreach (RadComboBoxItem item in rcbIncidentType.Items)
					item.Checked = true;
			}
			if (rcbStatusSelect.SelectedItem == null)
				rcbStatusSelect.SelectedIndex = 0;

			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate); 
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
		   
			List<decimal>  plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();
			
			var typeList = new List<decimal>();
			List<string> statusList = new List<string>();

			if (HSCalcs() == null)
			{
				foreach (RadComboBoxItem item in rcbIncidentType.Items)
					item.Checked = true;
			}
			typeList = rcbIncidentType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
			selectedValue = rcbStatusSelect.SelectedValue;

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			HSCalcs().ObjAny = cbShowImage.Checked;

			HSCalcs().ehsCtl.SelectIncidentList(plantIDS, typeList, fromDate, toDate, selectedValue, cbShowImage.Checked, cbCreatedByMe.Checked ? SessionManager.UserContext.Person.PERSON_ID : 0);
				
			if (!UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident))
				HSCalcs().ehsCtl.IncidentHst = (from i in HSCalcs().ehsCtl.IncidentHst where i.Incident.ISSUE_TYPE_ID != 10 select i).ToList();

			if (HSCalcs().ehsCtl.IncidentHst != null)
			{
				uclIncidentList.BindIncidentListRepeater(HSCalcs().ehsCtl.IncidentHst.OrderByDescending(x => x.Incident.INCIDENT_DT).ToList(), "EHS", cbShowImage.Checked, false);
			}


			if (HSCalcs().ehsCtl.IncidentHst != null && HSCalcs().ehsCtl.IncidentHst.Count > 0)
				lblChartType.Visible = ddlChartType.Visible = true;

			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;

			if (ddlChartType.SelectedValue != "")
				lnkCloseChart(null, null);

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

		#region conversion
		protected void btnConversion_Click(object sender, EventArgs e)
		{
			int status = 0;
			INCIDENT_ANSWER answer = null;
			List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[6] { "SHIFT","INJURY_CAUSE", "INJURY_TYPE", "INJURY_PART", "INJURY_TENURE", "IQ_10"});

			foreach (EHSIncidentData eda in HSCalcs().ehsCtl.IncidentHst.Where(i=> i.Incident.INCFORM_LAST_STEP_COMPLETED < 1  &&  i.Incident.INCIDENT_ID > 0))
			{
				INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == eda.Incident.INCIDENT_ID select i).SingleOrDefault();
				incident.INCIDENT_ANSWER.Load();

				// clear any prior conversion reecords
				string delCmd = " = " + incident.INCIDENT_ID.ToString();
				status = entities.ExecuteStoreCommand("DELETE FROM TASK_STATUS WHERE RECORD_TYPE = 40 AND RECORD_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_LOSTTIME_HIST WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_CONTAIN WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_ACTION WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_CAUSATION WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_WITNESS WHERE INCIDENT_ID" + delCmd);
				status = entities.ExecuteStoreCommand("DELETE FROM INCFORM_INJURYILLNESS WHERE INCIDENT_ID" + delCmd);

				EHSIncidentTypeId issueType = (EHSIncidentTypeId)incident.ISSUE_TYPE_ID;
				switch (issueType)
				{
					case EHSIncidentTypeId.PropertyDamage:
					case EHSIncidentTypeId.PowerOutage:
					case EHSIncidentTypeId.Fire:
					case EHSIncidentTypeId.Explosion:
					case EHSIncidentTypeId.ImsAudit:
					case EHSIncidentTypeId.RegulatoryContact:
					case EHSIncidentTypeId.FireSystemImpairment:
					case EHSIncidentTypeId.SpillRelease:
					case EHSIncidentTypeId.EhsWalk:
					case EHSIncidentTypeId.NearMiss:
					case EHSIncidentTypeId.InjuryIllness:
						incident.INCFORM_LAST_STEP_COMPLETED = 100;  // assume new status
						answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 69).FirstOrDefault();  // containment
						if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
						{
							incident.INCFORM_LAST_STEP_COMPLETED = 110;  // containment
							INCFORM_CONTAIN contain = new INCFORM_CONTAIN();
							contain.INCIDENT_ID = incident.INCIDENT_ID;
							contain.ITEM_SEQ = 1;
							contain.ITEM_DESCRIPTION = answer.ANSWER_VALUE;
							contain.ASSIGNED_PERSON_ID = incident.CREATE_PERSON;
							contain.START_DATE = contain.COMPLETION_DATE = incident.CREATE_DT;
							contain.IsCompleted = true;
							contain.LAST_UPD_BY = SessionManager.UserContext.UserName();
							contain.LAST_UPD_DT = DateTime.UtcNow;

							entities.AddToINCFORM_CONTAIN(contain);
						}

						answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 24).FirstOrDefault();  // root cause
						if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
						{
							incident.INCFORM_LAST_STEP_COMPLETED = 120;  // root cause
							INCFORM_ROOT5Y rootc = new INCFORM_ROOT5Y();
							rootc.INCIDENT_ID = incident.INCIDENT_ID;
							rootc.ITEM_SEQ = 1;
							rootc.ITEM_DESCRIPTION = answer.ANSWER_VALUE;
							rootc.LAST_UPD_BY = SessionManager.UserContext.UserName();
							rootc.LAST_UPD_DT = DateTime.UtcNow;

							entities.AddToINCFORM_ROOT5Y(rootc);
						}

						answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 78).FirstOrDefault();  // causation
						if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
						{
							incident.INCFORM_LAST_STEP_COMPLETED = 125;  // causation
							INCFORM_CAUSATION cause = new INCFORM_CAUSATION();
							cause.INCIDENT_ID = incident.INCIDENT_ID;
							cause.CAUSEATION_CD = xlatList.Where(l => l.XLAT_GROUP == "INJURY_CAUSE" && l.DESCRIPTION == answer.ANSWER_VALUE).FirstOrDefault() == null ? "1000" : xlatList.Where(l => l.XLAT_GROUP == "INJURY_CAUSE" && l.DESCRIPTION == answer.ANSWER_VALUE).FirstOrDefault().XLAT_CODE;
							cause.LAST_UPD_BY = SessionManager.UserContext.UserName();
							cause.LAST_UPD_DT = DateTime.UtcNow;

							entities.AddToINCFORM_CAUSATION(cause);
						}

						answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 27).FirstOrDefault();  // corrective action
						if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
						{
							incident.INCFORM_LAST_STEP_COMPLETED = 130;  // corrective action
							TASK_STATUS action = new TASK_STATUS();
							action.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
							action.RECORD_ID = incident.INCIDENT_ID;
							action.TASK_STEP = ((int)SysPriv.action).ToString();
							action.TASK_SEQ = 0;
							action.RECORD_SUBID = 0;
							action.TASK_TYPE = "T";
							action.TASK_SEQ = 0;
							action.DESCRIPTION = answer.ANSWER_VALUE;
							action.DETAIL = incident.DESCRIPTION;
							action.STATUS = "1";
							action.CREATE_ID = action.RESPONSIBLE_ID = action.COMPLETE_ID = incident.CREATE_PERSON;  // default action values
							action.CREATE_DT = action.DUE_DT = action.COMPLETE_DT = incident.CREATE_DT;
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 79).FirstOrDefault();  // responsible 
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								action.RESPONSIBLE_ID = action.COMPLETE_ID = decimal.Parse(answer.ANSWER_VALUE);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 65).FirstOrDefault();  // action due date
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								action.DUE_DT = DateTime.ParseExact(answer.ANSWER_VALUE, "M/d/yyyy hh:mm:ss tt", null);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 66).FirstOrDefault();  // action complete date
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								action.COMPLETE_DT = DateTime.ParseExact(answer.ANSWER_VALUE, "M/d/yyyy hh:mm:ss tt", null);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 70).FirstOrDefault();  // verification ?
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								action.COMMENTS = answer.ANSWER_VALUE;
							}

							entities.AddToTASK_STATUS(action);
						}

						if (incident.CLOSE_DATE_DATA_COMPLETE.HasValue || incident.CLOSE_DATE.HasValue)
						{
							incident.INCFORM_LAST_STEP_COMPLETED = 151;  // signoff
							INCFORM_APPROVAL approval = new INCFORM_APPROVAL();
							approval.INCIDENT_ID = incident.INCIDENT_ID;
							approval.ITEM_SEQ = (int)SysPriv.approve1;
							approval.APPROVAL_DATE = incident.CLOSE_DATE.HasValue ? incident.CLOSE_DATE : incident.CLOSE_DATE_DATA_COMPLETE;
							approval.IsAccepted = true;
							approval.APPROVER_PERSON_ID = incident.CLOSE_PERSON.HasValue ? incident.CLOSE_PERSON : incident.CREATE_PERSON;
							PERSON person = (from p in entities.PERSON where p.PERSON_ID == approval.APPROVER_PERSON_ID select p).FirstOrDefault();
							approval.APPROVAL_MESSAGE = approval.APPROVER_PERSON = (person.FIRST_NAME + " " + person.LAST_NAME);
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 67).FirstOrDefault();  // completed by
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								approval.APPROVAL_MESSAGE = approval.APPROVER_PERSON = answer.ANSWER_VALUE;
								string[] names = answer.ANSWER_VALUE.ToLower().Split(' ');
								if (names.Length > 1)
								{
									string firstName = names[0];
									string lastnamne = names[1];
									person = (from p in entities.PERSON where p.FIRST_NAME.ToLower() == firstName && p.LAST_NAME.ToLower() == lastnamne select p).FirstOrDefault();
								}
							}
							if (person != null)
							{
								approval.APPROVER_PERSON_ID = person.PERSON_ID;
								approval.APPROVER_TITLE = person.JOB_TITLE;
							}

							entities.AddToINCFORM_APPROVAL(approval);
						}

						if (issueType == EHSIncidentTypeId.InjuryIllness)
						{
							INCFORM_INJURYILLNESS inRec = new INCFORM_INJURYILLNESS();
							INCFORM_WITNESS witness = new INCFORM_WITNESS();
							INCFORM_LOSTTIME_HIST hist = new INCFORM_LOSTTIME_HIST();

							inRec.INCIDENT_ID = incident.INCIDENT_ID;
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 6).FirstOrDefault(); // shift

							inRec.SHIFT = GetXLATCode(xlatList, "SHIFT", answer.ANSWER_VALUE);
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 7).FirstOrDefault(); // department
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.DEPARTMENT = answer.ANSWER_VALUE;
								DEPARTMENT dept = (from d in entities.DEPARTMENT where d.DEPT_NAME.ToLower() == answer.ANSWER_VALUE.ToLower() select d).SingleOrDefault();
								if (dept != null)
									inRec.DEPT_ID = dept.DEPT_ID;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 8).FirstOrDefault(); // involved person
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.INVOLVED_PERSON_NAME = answer.ANSWER_VALUE;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 4).FirstOrDefault(); // supervisor inform date
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.SUPERVISOR_INFORMED_DT = DateTime.ParseExact(answer.ANSWER_VALUE, "M/d/yyyy hh:mm:ss tt", null);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 5).FirstOrDefault(); // time of incident
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.INCIDENT_TIME = TimeSpan.Parse(answer.ANSWER_VALUE);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 9).FirstOrDefault(); // witness
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE) &&  answer.ANSWER_VALUE.Split(' ').Length > 1)
							{
								witness.INCIDENT_ID = incident.INCIDENT_ID;
								witness.WITNESS_NO = 1;
								witness.WITNESS_NAME = answer.ANSWER_VALUE;
								witness.LAST_UPD_BY = SessionManager.UserContext.UserName();
								witness.LAST_UPD_DT = DateTime.UtcNow;

								entities.AddToINCFORM_WITNESS(witness);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 10).FirstOrDefault(); // inside/outside
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.INSIDE_OUTSIDE_BLDNG = GetXLATCode(xlatList, "IQ_10", answer.ANSWER_VALUE);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 11).FirstOrDefault(); // weather
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								; // NO FIELD
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 12).FirstOrDefault(); // injury type
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.INJURY_TYPE = GetXLATCode(xlatList, "INJURY_TYPE", answer.ANSWER_VALUE);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 13).FirstOrDefault(); // body part
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.INJURY_BODY_PART = GetXLATCode(xlatList, "INJURY_PART", answer.ANSWER_VALUE);
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 15).FirstOrDefault(); // reocurrance
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.REOCCUR = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 16).FirstOrDefault(); // first aid case
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.FIRST_AID = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 37).FirstOrDefault(); // employee
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.COMPANY_SUPERVISED = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 58).FirstOrDefault(); // specific description
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.DESCRIPTION_LOCAL = answer.ANSWER_VALUE;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 62).FirstOrDefault(); // recordable
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.RECORDABLE = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 63).FirstOrDefault(); // lost time case
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.LOST_TIME = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;
								if (inRec.LOST_TIME)
								{
									hist.INCIDENT_ID = incident.INCIDENT_ID;
									hist.WORK_STATUS = "03";
									hist.WORK_STATUS = "Lost Time";
									hist.BEGIN_DT = incident.INCIDENT_DT;
									answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 3).FirstOrDefault(); // expected return date
									if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
									{
										hist.RETURN_EXPECTED_DT = DateTime.ParseExact(answer.ANSWER_VALUE, "M/d/yyyy hh:mm:ss tt", null);
									}
									answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 55).FirstOrDefault(); // actual return date
									if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
									{
										hist.RETURN_TOWORK_DT = DateTime.ParseExact(answer.ANSWER_VALUE, "M/d/yyyy hh:mm:ss tt", null);
									}
									hist.LAST_UPD_BY = SessionManager.UserContext.UserName();
									hist.LAST_UPD_DT = DateTime.UtcNow;

									entities.AddToINCFORM_LOSTTIME_HIST(hist);
								}
							}
							answer = incident.INCIDENT_ANSWER.Where(a => a.INCIDENT_QUESTION_ID == 74).FirstOrDefault(); // occupational event ?
							if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
							{
								inRec.STD_PROCS_FOLLOWED = answer.ANSWER_VALUE.ToLower() == "yes" ? true : false;  // map to std procedures ?
							}

							entities.AddToINCFORM_INJURYILLNESS(inRec);
						}

						status = entities.SaveChanges();

						break;
					case EHSIncidentTypeId.PreventativeAction:
						break;
					default:
						break;
				}
			}
		}

		public static string GetXLATCode(List<XLAT> xlatList, string xlatGroup, string value)
		{
			string xlatCode = "";

			XLAT xlat = xlatList.Where(l => l.XLAT_GROUP == xlatGroup && l.DESCRIPTION == value).FirstOrDefault();
			if (xlat != null)
			{
				xlatCode = xlat.XLAT_CODE;
			}

			return xlatCode;
		}
		#endregion

	}

}
