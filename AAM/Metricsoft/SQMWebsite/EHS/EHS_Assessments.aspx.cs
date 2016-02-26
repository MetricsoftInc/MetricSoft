using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;

namespace SQM.Website.EHS
{
	public partial class EHS_Assessments : System.Web.UI.Page
	{
		protected enum DisplayState
		{
			AuditList,
			AuditNotificationNew,
			AuditNotificationEdit,
			AuditNotificationClosed,
			AuditNotificationDisplay,
			AuditReportEdit
		}

		// Mode should be "audit" (standard) or "prevent" (RMCAR)
		public AuditMode Mode
		{
			get { return ViewState["Mode"] == null ? AuditMode.Audit : (AuditMode)ViewState["Mode"]; }
			set { ViewState["Mode"] = value; }
		}

		private List<XLAT> TaskXLATList
		{
			get { return ViewState["TaskXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["TaskXLATList"]; }
			set { ViewState["TaskXLATList"] = value; }
		}

		public bool isDirected
		{
			get { return ViewState["isDirected"] == null ? false : (bool)ViewState["isDirected"]; }
			set { ViewState["isDirected"] = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclAssessmentForm.OnAttachmentListItemClick += OpenFileUpload;
			uclAssessmentForm.OnExceptionListItemClick += AddTask;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.EHSAudits;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ":";
			this.lblToDate.Text = Resources.LocalizedText.To + ":";
			this.lblAuditType.Text = Resources.LocalizedText.AssessmentType + ":";

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclAuditList.AuditListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbAuditType);

			if (Request.QueryString["mode"] != null)
			{
				string mode = Request.QueryString["mode"].ToString().ToLower();
				if (!string.IsNullOrEmpty(mode))
				{
					if (mode == "audit")
						this.Mode = AuditMode.Audit;
				}
			}
			uclAssessmentForm.Mode = this.Mode;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

			bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.audit);

			var sourceId = Page.Request[Page.postEventSourceID];

			if (IsPostBack)
			{
				if (!uclAssessmentForm.IsEditContext)
					RadPersistenceManager1.SaveState();

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayAudits":
								rbNew.Visible = createAuditAccess;
								UpdateDisplayState(DisplayState.AuditList);
								break;

							case "Notification":
								//UpdateDisplayState(DisplayState.AuditNotificationEdit); 
								//UpdateDisplayState(DisplayState.AuditReportEdit);
								uclAssessmentForm.EditAuditId = SessionManager.ReturnRecordID;
								// need to determine if the Audit is past due and force it into display mode (probelm when coming from Calendar)
								string auditStatus = EHSAuditMgr.SelectAuditStatus(SessionManager.ReturnRecordID);
								if (auditStatus == "C")
									UpdateDisplayState(DisplayState.AuditNotificationDisplay);
								else
									UpdateDisplayState(DisplayState.AuditNotificationEdit);
								if (isDirected)
								{
									rbNew.Visible = false;
									uclAssessmentForm.EnableReturnButton(false);
								}
								break;

							case "Closed":
								uclAssessmentForm.EditAuditId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.AuditNotificationClosed);
								if (isDirected)
								{
									rbNew.Visible = false;
									uclAssessmentForm.EnableReturnButton(false);
								}
								break;
							case "DisplayOnly":
								uclAssessmentForm.EditAuditId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.AuditNotificationDisplay);
								if (isDirected)
								{
									rbNew.Visible = false;
									uclAssessmentForm.EnableReturnButton(false);
								}
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
			// for now, we will only let 'admin' create audits
			//if (!SessionManager.UserContext.Person.SSO_ID.ToLower().Equals("admin"))
			//	rbNew.Visible = false;

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
				SearchAudits();      // suppress list when invoking page from inbox

		}

		protected void UpdateDisplayState(DisplayState state)
		{
			switch (state)
			{
				case DisplayState.AuditList:
					SearchAudits();
					uclAssessmentForm.Visible = false;
					//rbNew.Visible = true;
					break;

				case DisplayState.AuditNotificationNew:
					divAuditList.Visible = false;
					uclAssessmentForm.Visible = true;
					uclAssessmentForm.IsEditContext = false;
					uclAssessmentForm.ClearControls();
					//rbNew.Visible = false;
					uclAssessmentForm.CheckForSingleType();
					break;

				case DisplayState.AuditNotificationEdit:
					divAuditList.Visible = false;
					uclAssessmentForm.CurrentStep = 0;
					uclAssessmentForm.IsEditContext = true;
					uclAssessmentForm.Visible = true;
					//rbNew.Visible = false;
					uclAssessmentForm.BuildForm();
					break;

				case DisplayState.AuditNotificationDisplay:
				case DisplayState.AuditNotificationClosed:
					divAuditList.Visible = false;
					uclAssessmentForm.CurrentStep = 1;
					uclAssessmentForm.IsEditContext = false;
					//rbNew.Visible = false;
					uclAssessmentForm.Visible = true;
					//uclAuditForm.BuildForm();
					break;

			}

			// for now, we will only let 'admin' create audits
			//if (!SessionManager.UserContext.Person.SSO_ID.ToLower().Equals("admin"))
			//	rbNew.Visible = false;

			SessionManager.ClearReturns();
		}

		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList), 0);

				rcbStatusSelect.SelectedValue = "A";
			}
			divAuditList.Visible = true;
			pnlChartSection.Style.Add("display", "none");
			lblChartType.Visible = ddlChartType.Visible = false;

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			// ABW 1/5/16 - use user's default plant local time for search default
			DateTime localTime = SessionManager.UserContext.LocalTime;
			dmFromDate.SelectedDate = localTime.AddMonths(-1);
			dmToDate.SelectedDate = localTime.AddMonths(1);

			if (Mode == AuditMode.Audit)
			{

				lblViewEHSRezTitle.Text = GetLocalResourceObject("lblViewEHSRezTitleResource1.Text").ToString();
				lblPageInstructions.Text = GetLocalResourceObject("lblPageInstructionsResource1.Text").ToString();
				//lblStatus.Text = "Assessment Status:";
				rbNew.Text = GetLocalResourceObject("rbNewResource1.Text").ToString();
				lblAuditDate.Visible = true;
				phAudit.Visible = true;

				SETTINGS sets = SQMSettings.GetSetting("EHS", "AUDITSEARCHFROM");
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

				if (rcbAuditType.Items.Count < 1)
				{

					foreach (AUDIT_TYPE ip in EHSAuditMgr.SelectAuditTypeList(SessionManager.PrimaryCompany().COMPANY_ID, false))
					{
						RadComboBoxItem item = new RadComboBoxItem(ip.TITLE, ip.AUDIT_TYPE_ID.ToString());
						item.Checked = true;
						rcbAuditType.Items.Add(item);
					}
				}

				// lookup charts defined for this module & app context
				//PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);
				//if (view != null)
				//{
				//	ddlChartType.Items.Clear();
				//	ddlChartType.Items.Add(new RadComboBoxItem("", ""));
				//	foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.Where(l => l.STATUS != "I").OrderBy(l => l.ITEM_SEQ).ToList())
				//	{
				//		RadComboBoxItem item = new RadComboBoxItem();
				//		item.Text = vi.TITLE;
				//		item.Value = vi.ITEM_SEQ.ToString();
				//		item.ImageUrl = ViewModel.GetViewItemImageURL(vi);
				//		ddlChartType.Items.Add(item);
				//	}
				//}
				ddlChartType.Items.Clear();
				ddlChartType.Items.Add(new RadComboBoxItem("", ""));
				ddlChartType.Items.Add(new RadComboBoxItem("Audit Summary", "charts"));
				ddlChartType.Items.Add(new RadComboBoxItem("Data Results", "report"));

			}
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			rbNew.Visible = false;
			UpdateDisplayState(DisplayState.AuditNotificationNew);
		}

		#region auditaging

		protected void btnAuditsSearchClick(object sender, EventArgs e)
		{
			//SearchAudits();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void btnFindingsSearchClick(object sender, EventArgs e)
		{
			SearchAudits();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void lnkCloseDetails(object sender, EventArgs e)
		{
			pnlAuditDetails.Visible = lnkAuditDetailsClose.Visible = false;
		}

		private void SearchAudits()
		{
			string selectedValue = "";
			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate);
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

			List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();

			var typeList = new List<decimal>();
			List<string> statusList = new List<string>();

			if (Mode == AuditMode.Audit)
			{
				if (HSCalcs() == null)
				{
					foreach (RadComboBoxItem item in rcbAuditType.Items)
						item.Checked = true;
				}
				typeList = rcbAuditType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
				selectedValue = rcbStatusSelect.SelectedValue;
			}

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			//HSCalcs().ObjAny = cbShowImage.Checked;

			HSCalcs().ehsCtl.SelectAuditList(plantIDS, typeList, fromDate, toDate, selectedValue);

			// may want to access only the ones assigned to that person
			//if (accessLevel < AccessMode.Admin)
			//	HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.ISSUE_TYPE_ID != 10 select i).ToList();
			bool allAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
			if (!allAuditAccess)
				HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.AUDIT_PERSON == SessionManager.UserContext.Person.PERSON_ID select i).ToList();

			if (HSCalcs().ehsCtl.AuditHst != null)
			{
				HSCalcs().ehsCtl.AuditHst.OrderByDescending(x => x.Audit.AUDIT_DT);
				uclAuditList.BindAuditListRepeater(HSCalcs().ehsCtl.AuditHst, "EHS");
			}
			//}

			if (HSCalcs().ehsCtl.AuditHst != null && HSCalcs().ehsCtl.AuditHst.Count > 0)
				lblChartType.Visible = ddlChartType.Visible = true;

			pnlAuditDetails.Visible = lnkAuditDetailsClose.Visible = false;

			if (ddlChartType.SelectedValue != "")
				ddlChartTypeChange(null, null);

		}

		#endregion


		#region formatting helpers

		protected string GetPlantName(decimal auditId)
		{
			string content = "";
			content = EHSAuditMgr.SelectAuditLocationNameByAuditId(auditId);
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
			//if (Mode == AuditMode.Prevent)
			//	uclExport.GeneratePreventativeActionExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
			//else

			//uclExport.GenerateAuditExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
		}
		protected void lnkExportClick(object sender, EventArgs e)
		{
			//uclExport.GenerateAuditExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
		}
		#endregion

		//protected void lnkAddTask_Click(Object sender, EventArgs e)
		//{
		//	//if (OnExceptionListItemClick != null)
		//	//{
		//	//LinkButton lnk = (LinkButton)sender;
		//	//string[] cmd = lnk.CommandArgument.Split(',');
		//	string[] cmd = lnkAddTask.CommandArgument.Split(',');
		//	AddTask(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
		//	// call sending AuditID, QuestionID
		//	//	OnExceptionListItemClick(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
		//	//}
		//}

		private void AddTask(decimal auditID, decimal questionID)
		{
			int recordType = (int)TaskRecordType.Audit;
			EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(auditID, questionID);
			AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), auditID);
			uclTaskList.TaskWindow(50, auditQuestion.AuditId, auditQuestion.QuestionId, "350", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID);

			//uclTask.BindTaskAdd(recordType, auditQuestion.AuditId, auditQuestion.QuestionId, "350", "T", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID, "");
			//string script = "function f(){OpenUpdateTaskWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}
		private void UpdateTaskList(string cmd)
		{
			//if (cmd != "cancel")
			//	SearchAudits();
		}

		private void UpdateTaskList(string cmd, decimal recordID, decimal recordSubID)
		{
			// update the status when a task is added
			if (cmd == "added")
			{
				EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(recordID, recordSubID);

				if (auditQuestion != null)
				{
					auditQuestion.Status = "02";
					EHSAuditMgr.UpdateAnswer(auditQuestion);
				}
				//SearchAudits();
			}
		}

		protected void OpenFileUpload(decimal auditID, decimal questionID)
		{
			int recordType = (int)TaskRecordType.Audit;
			//EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(auditID, questionID);
			//AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), auditID);

			uclAttachWin.OpenManageAttachmentsWindow(recordType, auditID, questionID.ToString(), "Upload Attachments", "Upload or view attachments for this assessment question");
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
			//if (ddlChartType.SelectedValue == "" || HSCalcs().ehsCtl.IncidentHst == null || HSCalcs().ehsCtl.IncidentHst.Count == 0)
			if (ddlChartType.SelectedValue == "")
			{
				pnlChartSection.Style.Add("display", "none");
				lnkChartClose.Visible = lnkPrint.Visible = false;
			}
			else
			{
				//PERSPECTIVE_VIEW view = null;
				//divSummaryCharts.Controls.Clear();

				//view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);

				//if (view != null)
				//{
				//	PERSPECTIVE_VIEW_ITEM vi = view.PERSPECTIVE_VIEW_ITEM.Where(i => i.ITEM_SEQ.ToString() == ddlChartType.SelectedValue).FirstOrDefault();
				//	if (vi != null)
				//	{
				//		GaugeDefinition ggCfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
				//		ggCfg.Position = null;
				//		HSCalcs().ehsCtl.SetCalcParams(vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, (int)vi.SERIES_ORDER).IncidentSeries((EHSCalcsCtl.SeriesOrder)vi.SERIES_ORDER, SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToArray(), new DateTime(1900, 1, 1), SessionManager.UserContext.LocalTime.AddYears(100), HSCalcs().ehsCtl.GetIncidentTopics());
				//		uclChart.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, HSCalcs().ehsCtl.Results, divSummaryCharts);
				//		pnlChartSection.Style.Add("display", "inline");
				//		lnkChartClose.Visible = lnkPrint.Visible = true;
				//		// return;
				//	}
				//}
				divChart.Controls.Clear();
				switch (ddlChartType.SelectedValue)
				{
					case "charts":
						decimal protection = 0;
						decimal protectionTotal = 0;
						decimal surveillance = 0;
						decimal surveillanceTotal = 0;
						decimal manualresponse = 0;
						decimal manualresponseTotal = 0;
						decimal humanelement = 0;
						decimal humanelementTotal = 0;
						decimal hazards = 0;
						decimal hazardsTotal = 0;
						decimal maintenence = 0;
						decimal maintenenceTotal = 0;
						int iTopic = 0;
						decimal answerValue;

						// get the current year results
						foreach (EHSAuditData audit in HSCalcs().ehsCtl.AuditHst)
						{
							foreach (AUDIT_ANSWER answer in audit.Audit.AUDIT_ANSWER)
							{
								try
								{
									answerValue = Convert.ToInt16(answer.ANSWER_VALUE);
								}
								catch
								{
									answerValue = 0;
								}
								EHSAuditQuestion question = EHSAuditMgr.SelectAuditQuestion(audit.Audit.AUDIT_ID, answer.AUDIT_QUESTION_ID);
								iTopic = Convert.ToInt16(question.TopicId);
								switch (iTopic)
								{
									case 3:
										protection += answerValue;
										protectionTotal += 3;
										break;
									case 4:
										surveillance += answerValue;
										surveillanceTotal += 3;
										break;
									case 5:
										manualresponse += answerValue;
										manualresponseTotal += 3;
										break;
									case 6:
										humanelement += answerValue;
										humanelementTotal += 3;
										break;
									case 7:
										hazards += answerValue;
										hazardsTotal += 3;
										break;
									case 8:
										maintenence += answerValue;
										maintenenceTotal += 3;
										break;
								}
							}
						}
						List<GaugeSeries> gaugeSeries = new List<GaugeSeries>();
						GaugeSeries g = new GaugeSeries();
						g.Name = "Percentage";
						if (protectionTotal > 0)
							answerValue = Math.Round((protection / protectionTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Protection"));
						if (surveillanceTotal > 0)
							answerValue = Math.Round((surveillance / surveillanceTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Surveillance"));
						if (manualresponseTotal > 0)
							answerValue = Math.Round((manualresponse / manualresponseTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Manual Response"));
						if (humanelementTotal > 0)
							answerValue = Math.Round((humanelement / humanelementTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Human Element"));
						if (hazardsTotal > 0)
							answerValue = Math.Round((hazards / hazardsTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Hazards"));
						if (maintenenceTotal > 0)
							answerValue = Math.Round((maintenence / maintenenceTotal), 3) * 100;
						else
							answerValue = 0;
						g.ItemList.Add(new GaugeSeriesItem(0, 0, 0, answerValue, "Maintenence"));
						gaugeSeries.Add(g);
						GaugeDefinition gauge = new GaugeDefinition();
						gauge.ColorPallete = "chartSeriesColor";
						//gauge.ContainerHeight = 550;
						//gauge.ContainerWidth = 550;
						gauge.ControlType = 70;
						gauge.DisplayLabel = true;
						gauge.DisplayLegend = true;
						gauge.DisplayTitle = true;
						gauge.DisplayTooltip = true;
						gauge.Grouping = 0;
						gauge.Height = 550;
						gauge.Width = 650;
						gauge.ItemVisual = "CENTER";
						gauge.MinorTics = false;
						gauge.Multiplier = 0;
						gauge.ScaleMax = 100;
						gauge.ScaleMin = 0;

						gauge.Title = "Audit Summary";
						gauge.DisplayTitle = false;
						CalcsResult rslt = new CalcsResult();
						rslt.metricSeries = gaugeSeries;
						rslt.Status = 0;
						rslt.ValidResult = true;

						uclChart.CreateControl(SQMChartType.SpiderChart, gauge, rslt, divChart);
						pnlChartSection.Style.Add("display", "inline");
						lnkChartClose.Visible = lnkPrint.Visible = true;
						divChart.Visible = true;
						divDataResults.Visible = false;
						break;
					case "report":
						lnkChartClose.Visible = lnkPrint.Visible = true;
						divChart.Visible = false;
						divDataResults.Visible = true;
						break;
				}
			}
		}

	}

}
