using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;
using System.Drawing;

namespace SQM.Website
{
	public partial class Ucl_EHSIncidentForm : System.Web.UI.UserControl
	{
		const Int32 MaxTextLength = 4000;

		int[] requiredToCloseFields = new int[] {
				(int)EHSQuestionId.Containment,
				(int)EHSQuestionId.RootCause,
				(int)EHSQuestionId.RootCauseOperationalControl,
				(int)EHSQuestionId.CorrectiveActions,
				(int)EHSQuestionId.Verification,
				(int)EHSQuestionId.ResponsiblePersonDropdown,
				(int)EHSQuestionId.DateDue
			};

		//static SQMMetricMgr HSCalcs;

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected bool IsUseCustomForm;

		bool controlQuestionChanged;

		List<EHSIncidentQuestion> questions;
		PSsqmEntities entities;

		public PageUseMode PageMode { get; set; }

		protected Ucl_RadAsyncUpload uploader;
		protected Ucl_PreventionLocation preventionLocationForm;
		protected RadDropDownList rddlLocation;
		protected RadDropDownList rddlFilteredUsers;

		// Incident Custom Forms:
		//protected Ucl_INCFORM_InjuryIllness injuryIllnessForm;

		protected static List<INCFORM_TYPE_CONTROL> incidentStepList;


		// Special answers used in INCIDENT table
		string incidentDescription = "";
		protected DateTime incidentDate;

		public void EnableReturnButton(bool bEnabled)
		{
			ahReturn.Visible = bEnabled;
		}

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
				RefreshPageContext();
			}
		}

		public int CurrentStep
		{
			get { return ViewState["CurrentStep"] == null ? 0 : (int)ViewState["CurrentStep"]; }
			set { ViewState["CurrentStep"] = value; }
		}
		public string CurrentSubnav
		{
			get { return ViewState["CurrentSubnav"] == null ? "I" : (string)ViewState["CurrentSubnav"]; }
			set { ViewState["CurrentSubnav"] = value; }
		}

		public decimal EditIncidentId
		{
			get { return ViewState["EditIncidentId"] == null ? 0 : (decimal)ViewState["EditIncidentId"]; }
			set { ViewState["EditIncidentId"] = value; }
		}

		public decimal InitialPlantId
		{
			get { return ViewState["InitialPlantId"] == null ? 0 : (decimal)ViewState["InitialPlantId"]; }
			set { ViewState["InitialPlantId"] = value; }
		}

		protected decimal IncidentLocationId
		{
			get { return ViewState["IncidentLocationId"] == null ? 0 : (decimal)ViewState["IncidentLocationId"]; }
			set { ViewState["IncidentLocationId"] = value; }
		}

		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
		}

		public decimal EditIncidentTypeId
		{
			get { return EditIncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(EditIncidentId); }
		}

		public decimal SelectedTypeId
		{
			get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
			set { ViewState["SelectedTypeId"] = value; }
		}

		protected string SelectedTypeText
		{
			get { return ViewState["SelectedTypeText"] == null ? " " : (string)ViewState["SelectedTypeText"]; }
			set { ViewState["SelectedTypeText"] = value; }
		}

		protected decimal SelectedLocationId
		{
			get { return ViewState["SelectedLocationId"] == null ? 0 : (decimal)ViewState["SelectedLocationId"]; }
			set { ViewState["SelectedLocationId"] = value; }
		}
		protected int IncidentStepCompleted
		{
			get { return ViewState["IncidentStepCompleted"] == null ? 0 : (int)ViewState["IncidentStepCompleted"]; }
			set { ViewState["IncidentStepCompleted"] = value; }
		}
		protected decimal CreatePersonId
		{
			get { return ViewState["CreatePersonId"] == null ? 0 : (decimal)ViewState["CreatePersonId"]; }
			set { ViewState["CreatePersonId"] = value; }
		}

		protected List<SETTINGS> EHSSettings
		{
			get { return ViewState["EHSSettings"] == null ? SQMSettings.SelectSettingsGroup("EHS", "") : (List<SETTINGS>)ViewState["EHSSettings"]; }
			set { ViewState["EHSSettings"] = value; }
		}

		protected List<XLAT> XLATList
		{
			get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
			set { ViewState["XLATList"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			entities = new PSsqmEntities();
			controlQuestionChanged = false;

			ahReturn.HRef = "/EHS/EHS_Incidents.aspx";
			//btnSaveReturn.Visible = btnSaveContinue.Visible = false;

			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
			}

			if (IsPostBack)
			{
				string senderControl = Request.Params["__EVENTTARGET"].ToString();
				if (senderControl.ToLower().Contains("btnsubnav") || uclcontain.Visible == true || uclroot5y.Visible == true || uclCausation.Visible == true || uclaction.Visible == true || uclapproval.Visible == true || uclAlert.Visible == true || uclVideoPanel.Visible == true)
				{
					if (!senderControl.Contains("btnSubnavSave"))
						return;
				}

				LoadHeaderInformation();
				BuildForm();
			}
			else
			{
				incidentStepList = EHSIncidentMgr.SelectIncidentSteps(entities, -1m);
				XLATList = SQMBasePage.SelectXLATList(new string[1] { "INCIDENT_STEP" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				//UpdateControlledQuestions();
			}
		}

		#region Form

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info

			if (IsEditContext || CurrentStep > 0)
			{
				// in edit mode, load the header field values and make all fields display only

				var incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();

				BusinessLocation location = new BusinessLocation().Initialize((decimal)incident.DETECT_PLANT_ID);

				lblIncidentLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblIncidentLocation.Visible = true;
			}
		}

		private RadDropDownList HandleDepartmentList(EHSIncidentQuestion q, INCIDENT incident, bool shouldPopulate, RadDropDownList rddl)
		{
			List<DEPARTMENT> deptList = SQMModelMgr.SelectDepartmentList(entities, (decimal)incident.DETECT_PLANT_ID);
			foreach (DEPARTMENT dept in deptList)
			{
				rddl.Items.Add(new DropDownListItem(dept.DEPT_NAME, dept.DEPT_ID.ToString()));
			}
			if (shouldPopulate && rddl.FindItemByValue(q.AnswerText) != null)
				rddl.SelectedValue = q.AnswerText;

			return rddl;
		}

		public void BuildForm()
		{

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			if (typeId < 1)
				return;

			string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();

			INCIDENT incident = null;
			if (EditIncidentId > 0)
			{
				incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
				SessionManager.SetIncidentLocation(Convert.ToDecimal(incident.DETECT_PLANT_ID));
				IncidentLocationId = Convert.ToDecimal(incident.DETECT_PLANT_ID);
				IncidentLocationTZ = SQMModelMgr.LookupPlant(entities, (decimal)incident.DETECT_PLANT_ID, "").LOCAL_TIMEZONE;
				SelectedTypeId = (decimal)incident.ISSUE_TYPE_ID;
				SelectedTypeText = incident.ISSUE_TYPE;
				CreatePersonId = (decimal)incident.CREATE_PERSON;
				incidentDate = incident.INCIDENT_DT;
				IncidentStepCompleted = incident.INCFORM_LAST_STEP_COMPLETED;
			}

			pnlForm.Controls.Clear();
			divForm.Visible = true;
			//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = true;
			lblResults.Visible = false;

			if (PageMode == PageUseMode.ViewOnly)
			{
				pnlForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = false;
			}
			else
			{
				pnlForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.action, IncidentStepCompleted);
			}


			questions = EHSIncidentMgr.SelectIncidentQuestionList(typeId, companyId, CurrentStep);

			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"5\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));

			foreach (var q in questions)
			{
				var validator = new RequiredFieldValidator();

				// Look up answer if edit context
				var localQuestion = q;
				if (IsEditContext)
				{
					q.AnswerText = (from a in entities.INCIDENT_ANSWER
									where a.INCIDENT_ID == EditIncidentId
										&& a.INCIDENT_QUESTION_ID == localQuestion.QuestionId
									select a.ANSWER_VALUE).FirstOrDefault();
				}

				if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.UserContext.Language.NLS_LANGUAGE) == false)
				{
					continue;
				}

				bool shouldPopulate = IsEditContext && !string.IsNullOrEmpty(q.AnswerText);

				string qid = q.QuestionId.ToString();

				var pnl = new Panel() { ID = "Panel" + qid };

				pnl.Controls.Add(new LiteralControl("<tr><td class=\"tanCell\" style=\"width: 30%;\">"));
				pnl.Controls.Add(new Label() { ID = "Label" + qid, Text = q.QuestionText, AssociatedControlID = qid });
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (!string.IsNullOrEmpty(q.HelpText))
					AddToolTip(pnl, q);
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (q.IsRequired)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredStar\">&bull;</span>"));
				if (q.IsRequiredClose)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredCloseStar\">&bull;</span>"));

				pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));

				if (q.QuestionType != EHSIncidentQuestionType.BooleanCheckBox && q.QuestionType != EHSIncidentQuestionType.CheckBox &&
					q.QuestionType != EHSIncidentQuestionType.Attachment && q.QuestionType != EHSIncidentQuestionType.DocumentAttachment &&
					q.QuestionType != EHSIncidentQuestionType.ImageAttachment && q.QuestionType != EHSIncidentQuestionType.PageOneAttachment)
				{
					if (q.IsRequired)
					{
						validator = new RequiredFieldValidator()
						{
							ID = "Val" + qid,
							ControlToValidate = qid,
							ValidationGroup = "Val",
							Text = "<span class=\"formRequired\">Required</span>"
						};

						if (q.QuestionType == EHSIncidentQuestionType.Dropdown || q.QuestionType == EHSIncidentQuestionType.LocationDropdown ||
						q.QuestionType == EHSIncidentQuestionType.StandardsReferencesDropdown || q.QuestionType == EHSIncidentQuestionType.UsersDropdown ||
						q.QuestionType == EHSIncidentQuestionType.UsersDropdownLocationFiltered)
							validator.InitialValue = "";

						pnl.Controls.Add(validator);
					}
				}

				switch (q.QuestionType)
				{
					case EHSIncidentQuestionType.CurrentUser:
						var tu = new RadTextBox() { ID = qid, Width = 250, MaxLength = MaxTextLength, Skin = "Metro" };
						if (shouldPopulate)
							tu.Text = incident.CREATE_BY;
						else
							tu.Text = SQMModelMgr.FormatPersonListItem(SessionManager.UserContext.Person);
						tu.Enabled = false;
						tu.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(tu);
						break;
					case EHSIncidentQuestionType.CurrentLocation:
						var tl = new RadTextBox() { ID = qid, Width = 250, MaxLength = MaxTextLength, Skin = "Metro" };
						if (shouldPopulate)
							tl.Text = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID).PLANT_NAME;
						else
							tl.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						tl.Enabled = false;
						tl.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(tl);
						break;

					case EHSIncidentQuestionType.TextField:
						var tf = new RadTextBox() { ID = qid, Width = 550, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tf.Text = q.AnswerText;
						else if (q.QuestionId == (decimal)EHSQuestionId.Location) // Location
							tf.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						if (q.QuestionId == (decimal)EHSQuestionId.CompletedBy)
							tf.Enabled = false;
						tf.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(tf);
						break;

					case EHSIncidentQuestionType.TextBox:
						var tb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tb.Text = q.AnswerText;
						tb.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(tb);
						break;

					case EHSIncidentQuestionType.NativeLangTextBox:
						var nltb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							nltb.Text = q.AnswerText;
						nltb.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(nltb);
						break;

					case EHSIncidentQuestionType.RichTextBox:
						var re = new RadEditor() { ID = qid, Width = 550, Height=300, Skin = "Metro", MaxHtmlLength = MaxTextLength, CssClass = "WarnIfChanged" };
						re.EditModes = EditModes.Design;
						re.ToolsFile = "~/RadEditorToolsFile.xml";
						re.ContentAreaCssFile = "~/css/RadEditor.css";
						re.OnClientLoad = "OnEditorClientLoad";
						if (shouldPopulate)
							re.Content = q.AnswerText;
						pnl.Controls.Add(re);
						pnl.Controls.Add(new Literal() { Text = "<br style=\"clear: both;\"/><span style=\"font-size: 10px;\">Double-click links to preview</a>" });
						break;

					case EHSIncidentQuestionType.Radio:
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.TextLong, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rbl.Items.Add(li);
						}
						if (!shouldPopulate)
							rbl.SelectedIndex = 0; // Default to first

						rbl.Attributes.Add("ONCLICK", "ChangeUpdate()");
						pnl.Controls.Add(rbl);
						break;

					case EHSIncidentQuestionType.CheckBox:
						var cbl = new CheckBoxList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.TextLong, choice.Value);
							if (shouldPopulate)
							{
								string[] answers = q.AnswerText.Split('|');
								if (answers.Contains(choice.Value))
									li.Selected = true;
							}
							cbl.Items.Add(li);
						}

						cbl.Attributes.Add("ONCHANGE", "return ChangeUpdate();");
						pnl.Controls.Add(cbl);
						break;

					case EHSIncidentQuestionType.Dropdown:
						var rddl = new RadDropDownList() { ID = qid, CssClass = "WarnIfChanged", Width = 550, Skin = "Metro", ValidationGroup = "Val", AutoPostBack = false };
						rddl.Items.Add(new DropDownListItem("", ""));

						if (q.QuestionId == (decimal)EHSQuestionId.Department)
						{
							rddl = HandleDepartmentList(q, incident, shouldPopulate, rddl);
						}

						if (q.AnswerChoices != null && q.AnswerChoices.Count > 0)
						{
							// Check for any category headings
							var matches = q.AnswerChoices.Where(ac => ac.IsHeading == true);
							bool containsCategoryHeadings = (matches.Count() > 0);

							foreach (var choice in q.AnswerChoices)
							{
								if (containsCategoryHeadings == true)
								{
									if (choice.IsHeading)
										rddl.Items.Add(new DropDownListItem(choice.TextLong, "") { CssClass = "dropdownItemHeading", Enabled = false });
									else
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.TextLong, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.TextLong, choice.Value));
								}
							}

							rddl.ClearSelection();
							if (shouldPopulate)
								if (!string.IsNullOrEmpty(q.AnswerText))
									rddl.SelectedValue = q.AnswerText;
						}
						//rddl.AutoPostBack = true;
						rddl.OnClientSelectedIndexChanged = "ChangeUpdate";
						pnl.Controls.Add(rddl);
						break;

					case EHSIncidentQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp = SQMBasePage.SetRadDateCulture(rdp, "");
						/*
						if (CultureSettings.gregorianCalendarOverrides.Contains(lang))
						{
							rdp.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
							rdp.DateInput.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
						}
						*/
						rdp.ShowPopupOnFocus = true;
						if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate) // Default incident date
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}

						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdp.SelectedDate = parseDate;
						}
						else
						{
							// Default projected completion date 30 or 60 days based on previous questions
							if (q.QuestionId == (decimal)EHSQuestionId.ProjectedCompletionDate)
							{
								if (EditIncidentId > 0)
								{
									//INCIDENT incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
									if (incident != null)
									{
										// mt - due date now based on Incident creation date per TI
										//string dateAnswer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.ReportDate);
										//DateTime parseDate;
										//if (DateTime.TryParse(dateAnswer, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
										DateTime parseDate = DateTime.Now;
										//{
											string answer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.RecommendationType);
											if (answer.ToLower() == "behavioral")
												rdp.SelectedDate = parseDate.AddDays(30);
											else
												rdp.SelectedDate = parseDate.AddDays(60);
										//}
									}
								}
							}
						}

						// Incident report date, completion date, projected completion date are not editable
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate ||
							q.QuestionId == (decimal)EHSQuestionId.CompletionDate ||
							q.QuestionId == (decimal)EHSQuestionId.ProjectedCompletionDate)
						{
							rdp.Enabled = false;
						}

						rdp.ClientEvents.OnDateSelected = "ChangeUpdate";

						pnl.Controls.Add(rdp);
						break;

					case EHSIncidentQuestionType.Time:
						var rtp = new RadTimePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rtp.ShowPopupOnFocus = true;
						rtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						if (shouldPopulate)
						{
							TimeSpan parseTime;
							if (TimeSpan.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), out parseTime))
								rtp.SelectedTime = parseTime;
						}
						rtp.ClientEvents.OnDateSelected = "ChangeUpdate";
						pnl.Controls.Add(rtp);
						break;

					case EHSIncidentQuestionType.DateTime:
						var rdtp = new RadDateTimePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdtp.SelectedDate = parseDate;
						}
						rdtp.ClientEvents.OnDateSelected = "ChangeUpdate";
						pnl.Controls.Add(rdtp);
						break;

					case EHSIncidentQuestionType.BooleanCheckBox:
						pnl.Controls.Add(new LiteralControl("<div style=\"padding: 0 3px;\">"));
						var bcb = new CheckBox() { ID = qid, Text = Resources.LocalizedText.Yes, CssClass = "WarnIfChanged" };

						if (shouldPopulate)
							bcb.Checked = (q.AnswerText.ToLower() == "yes") ? true : false;
						else if (q.QuestionId == (decimal)EHSQuestionId.Create8D && EHSIncidentMgr.IsTypeDefault8D(typeId))
							bcb.Checked = true;

						// If controller question, "close" checkbox, or "create 8d" checkbox, register ajax behavior
						if ((q.QuestionControls != null && q.QuestionControls.Count > 0) ||
							q.QuestionId == (decimal)EHSQuestionId.CloseIncident ||
							q.QuestionId == (decimal)EHSQuestionId.Create8D)
						{
							bcb.CheckedChanged += new EventHandler(bcb_CheckedChanged);
							if (q.QuestionId == (decimal)EHSQuestionId.CloseIncident)
								bcb.CheckedChanged += new EventHandler(bcb_CheckedChangedClose);
							if (q.QuestionId == (decimal)EHSQuestionId.Create8D)
								bcb.CheckedChanged += new EventHandler(bcb_CheckedChangedCreate8D);
							bcb.AutoPostBack = true;
						}

						bcb.Attributes.Add("ONCHECKEDCHANGED", "ChangeUpdate()");
						pnl.Controls.Add(bcb);

						pnl.Controls.Add(new LiteralControl("</div>"));
						break;

					case EHSIncidentQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						uploader.SetReportOption(false);
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
						{
							uploader.GetUploadedFiles(40, EditIncidentId, "");
							uploader.SetViewMode(EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted));
						}
						break;

					case EHSIncidentQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "2");
						break;

					case EHSIncidentQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "2");
						break;

					case EHSIncidentQuestionType.PageOneAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "1");
						break;

					case EHSIncidentQuestionType.CurrencyTextBox:
						var ctb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Number };
						ctb.NumberFormat.DecimalDigits = 2;
						if (shouldPopulate)
							ctb.Text = q.AnswerText;

						ctb.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(ctb);
						break;

					case EHSIncidentQuestionType.PercentTextBox:
						var ptb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Percent };
						if (shouldPopulate)
							ptb.Text = q.AnswerText;
						ptb.ClientEvents.OnValueChanged = "ChangeUpdate";
						pnl.Controls.Add(ptb);
						break;

					case EHSIncidentQuestionType.StandardsReferencesDropdown:
						RadComboBox rcb = CreateReferencesDropdown(q.StandardType);
						rcb.ID = qid;
						rcb.Skin = "Metro";
						rcb.CssClass = "WarnIfChanged";
						rcb.Font.Size = 8;
						rcb.Width = 320;
						rcb.DropDownWidth = 400;
						rcb.Height = 300;
						if (shouldPopulate)
							rcb.SelectedValue = q.AnswerText;
						rcb.OnClientSelectedIndexChanged = "ChangeUpdate";
						pnl.Controls.Add(rcb);
						break;

					case EHSIncidentQuestionType.LocationDropdown:
					case EHSIncidentQuestionType.IncidentLocation:
						rddlLocation = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };

						if (SessionManager.IncidentLocation != null)
						{
							rddlLocation.Items.Add(new DropDownListItem(SessionManager.IncidentLocation.Plant.PLANT_NAME, Convert.ToString(SessionManager.IncidentLocation.Plant.PLANT_ID)));
							rddlLocation.Enabled = false;
						}
						else
						{
							var plantIdList = SelectPlantIdsByAccessLevel();
							if (plantIdList.Count > 1)
								rddlLocation.Items.Add(new DropDownListItem("", ""));
							foreach (decimal pid in plantIdList)
							{
								string plantName = EHSIncidentMgr.SelectPlantNameById(pid);
								rddlLocation.Items.Add(new DropDownListItem(plantName, Convert.ToString(pid)));
							}
						}

						if (shouldPopulate)
						{
							rddlLocation.SelectedValue = q.AnswerText;
							this.SelectedLocationId = Convert.ToDecimal(q.AnswerText);
						}
						rddlLocation.SelectedIndexChanged += rddlLocation_SelectedIndexChanged;
						rddlLocation.AutoPostBack = true;
						rddlLocation.OnClientSelectedIndexChanged = "ChangeUpdate";

						pnl.Controls.Add(rddlLocation);
						break;

					case EHSIncidentQuestionType.UsersDropdown:
						var rddl3 = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						rddl3.Items.Add(new DropDownListItem("", ""));

						var personList = new List<PERSON>();
						personList = EHSIncidentMgr.SelectIncidentPersonList(incident, true);

						foreach (PERSON p in personList)
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddl3.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
						}

						if (shouldPopulate)
							rddl3.SelectedValue = q.AnswerText;

						rddl3.OnClientSelectedIndexChanged = "ChangeUpdate";
						pnl.Controls.Add(rddl3);
						break;

					case EHSIncidentQuestionType.RequiredYesNoRadio:
						var rblYN = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblYN.RepeatDirection = RepeatDirection.Horizontal;
						rblYN.RepeatColumns = 2;
						rblYN.AutoPostBack = true;
						rblYN.Items.Add(new ListItem(Resources.LocalizedText.Yes, "Yes"));
						rblYN.Items.Add(new ListItem(Resources.LocalizedText.No, "No"));
						if (shouldPopulate)
							rblYN.SelectedValue = q.AnswerText;
						if (q.QuestionControls != null && q.QuestionControls.Count > 0)
						{
							rblYN.SelectedIndexChanged += rblYN_SelectedIndexChanged;
						}

						rblYN.Attributes.Add("ONCLICK", "ChangeUpdate()");
						pnl.Controls.Add(rblYN);
						break;

					case EHSIncidentQuestionType.UsersDropdownLocationFiltered:
						rddlFilteredUsers = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						BuildFilteredUsersDropdownList();

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;

						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						rddlFilteredUsers.OnClientSelectedIndexChanged = "ChangeUpdate";

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.system))
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && !UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.system))
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/>")); { }

			UpdateAnswersFromForm();

			UpdateButtonText();

			RefreshPageContext();
		}

		public void GetForm()
		{

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();

			INCIDENT incident = null;
			if (EditIncidentId > 0)
			{
				incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
			}

			pnlForm.Controls.Clear();
			divForm.Visible = true;
			//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = true;
			lblResults.Visible = false;

			if (typeId == 10)
			{
				preventionLocationForm = (Ucl_PreventionLocation)LoadControl("~/Include/Ucl_PreventionLocation.ascx");
				preventionLocationForm.ID = "plf1";
				preventionLocationForm.IsEditContext = IsEditContext;
				preventionLocationForm.IncidentId = EditIncidentId;

				preventionLocationForm.BuildCaseComboBox();
				if (IsEditContext == true)
					preventionLocationForm.PopulateForm();

				pnlForm.Controls.Add(new LiteralControl("<br/>"));
				pnlForm.Controls.Add(preventionLocationForm);
				pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));
				//btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;
				return;
			}

			questions = EHSIncidentMgr.SelectIncidentQuestionList(typeId, companyId, CurrentStep);

			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"5\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));

			foreach (var q in questions)
			{
				var validator = new RequiredFieldValidator();

				// Look up answer if edit context
				var localQuestion = q;
				if (IsEditContext)
				{
					q.AnswerText = (from a in entities.INCIDENT_ANSWER
									where a.INCIDENT_ID == EditIncidentId
										&& a.INCIDENT_QUESTION_ID == localQuestion.QuestionId
									select a.ANSWER_VALUE).FirstOrDefault();
				}
				bool shouldPopulate = IsEditContext && !string.IsNullOrEmpty(q.AnswerText);

				if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.UserContext.Language.NLS_LANGUAGE) == false)
				{
					continue;
				}

				string qid = q.QuestionId.ToString();

				var pnl = new Panel() { ID = "Panel" + qid };

				pnl.Controls.Add(new LiteralControl("<tr><td class=\"tanCell\" style=\"width: 30%;\">"));
				pnl.Controls.Add(new Label() { ID = "Label" + qid, Text = q.QuestionText, AssociatedControlID = qid });
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (!string.IsNullOrEmpty(q.HelpText))
					AddToolTip(pnl, q);
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (q.IsRequired)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredStar\">&bull;</span>"));
				if (q.IsRequiredClose)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredCloseStar\">&bull;</span>"));

				pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));

				if (q.QuestionType != EHSIncidentQuestionType.BooleanCheckBox && q.QuestionType != EHSIncidentQuestionType.CheckBox &&
					q.QuestionType != EHSIncidentQuestionType.Attachment && q.QuestionType != EHSIncidentQuestionType.DocumentAttachment &&
					q.QuestionType != EHSIncidentQuestionType.ImageAttachment && q.QuestionType != EHSIncidentQuestionType.PageOneAttachment)
				{
					if (q.IsRequired)
					{
						validator = new RequiredFieldValidator()
						{
							ID = "Val" + qid,
							ControlToValidate = qid,
							ValidationGroup = "Val",
							Text = "<span class=\"formRequired\">Required</span>"
						};

						if (q.QuestionType == EHSIncidentQuestionType.Dropdown || q.QuestionType == EHSIncidentQuestionType.LocationDropdown ||
						q.QuestionType == EHSIncidentQuestionType.StandardsReferencesDropdown || q.QuestionType == EHSIncidentQuestionType.UsersDropdown ||
						q.QuestionType == EHSIncidentQuestionType.UsersDropdownLocationFiltered)
							validator.InitialValue = "";

						pnl.Controls.Add(validator);
					}
				}

				switch (q.QuestionType)
				{
					case EHSIncidentQuestionType.TextField:
						var tf = new RadTextBox() { ID = qid, Width = 550, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tf.Text = q.AnswerText;
						else if (q.QuestionId == (decimal)EHSQuestionId.Location) // Location
							tf.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						if (q.QuestionId == (decimal)EHSQuestionId.CompletedBy)
							tf.Enabled = false;
						pnl.Controls.Add(tf);
						break;

					case EHSIncidentQuestionType.TextBox:
						var tb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tb.Text = q.AnswerText;
						pnl.Controls.Add(tb);
						break;

					case EHSIncidentQuestionType.NativeLangTextBox:
						var nltb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							nltb.Text = q.AnswerText;
						pnl.Controls.Add(nltb);
						break;

					case EHSIncidentQuestionType.RichTextBox:
						var re = new RadEditor() { ID = qid, Width = 550, Height = 300, Skin = "Metro", MaxHtmlLength = MaxTextLength, CssClass = "WarnIfChanged" };
						re.EditModes = EditModes.Design;
						re.ToolsFile = "~/RadEditorToolsFile.xml";
						re.ContentAreaCssFile = "~/css/RadEditor.css";
						re.OnClientLoad = "OnEditorClientLoad";
						if (shouldPopulate)
							re.Content = q.AnswerText;
						pnl.Controls.Add(re);
						pnl.Controls.Add(new Literal() { Text = "<br style=\"clear: both;\"/><span style=\"font-size: 10px;\">Double-click links to preview</a>" });
						break;

					case EHSIncidentQuestionType.Radio:
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.TextLong, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rbl.Items.Add(li);
						}
						if (!shouldPopulate)
							rbl.SelectedIndex = 0; // Default to first
						pnl.Controls.Add(rbl);
						break;

					case EHSIncidentQuestionType.CheckBox:
						var cbl = new CheckBoxList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.TextLong, choice.Value);
							if (shouldPopulate)
							{
								string[] answers = q.AnswerText.Split('|');
								if (answers.Contains(choice.Value))
									li.Selected = true;
							}
							cbl.Items.Add(li);
						}
						pnl.Controls.Add(cbl);
						break;

					case EHSIncidentQuestionType.Dropdown:
						var rddl = new RadDropDownList() { ID = qid, CssClass = "WarnIfChanged", Width = 550, Skin = "Metro", ValidationGroup = "Val" };
						rddl.Items.Add(new DropDownListItem("", ""));

						if (q.AnswerChoices != null && q.AnswerChoices.Count > 0)
						{
							// Check for any category headings
							var matches = q.AnswerChoices.Where(ac => ac.IsHeading == true);
							bool containsCategoryHeadings = (matches.Count() > 0);

							foreach (var choice in q.AnswerChoices)
							{
								if (containsCategoryHeadings == true)
								{
									if (choice.IsHeading)
										rddl.Items.Add(new DropDownListItem(choice.TextLong, choice.Value) { CssClass = "dropdownItemHeading", Enabled = false });
									else
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.TextLong, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.TextLong, choice.Value));
								}
							}

							rddl.ClearSelection();
							if (shouldPopulate)
								if (!string.IsNullOrEmpty(q.AnswerText))
									rddl.SelectedValue = q.AnswerText;
						}
						rddl.AutoPostBack = true;
						pnl.Controls.Add(rddl);
						break;

					case EHSIncidentQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp = SQMBasePage.SetRadDateCulture(rdp, "");
						/*
						if (CultureSettings.gregorianCalendarOverrides.Contains(lang))
						{
							rdp.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
							rdp.DateInput.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
						}
						*/
						rdp.ShowPopupOnFocus = true;
						if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate) // Default incident date
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}

						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdp.SelectedDate = parseDate;
						}
						else
						{
							// Default projected completion date 30 or 60 days based on previous questions
							if (q.QuestionId == (decimal)EHSQuestionId.ProjectedCompletionDate)
							{
								if (EditIncidentId > 0)
								{
									//INCIDENT incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
									if (incident != null)
									{
										// mt - due date now based on Incident creation date per TI
										//string dateAnswer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.ReportDate);
										//DateTime parseDate;
										//if (DateTime.TryParse(dateAnswer, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
										DateTime parseDate = DateTime.Now;
										//{
										string answer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.RecommendationType);
										if (answer.ToLower() == "behavioral")
											rdp.SelectedDate = parseDate.AddDays(30);
										else
											rdp.SelectedDate = parseDate.AddDays(60);
										//}
									}
								}
							}
						}

						// Incident report date, completion date, projected completion date are not editable
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate ||
							q.QuestionId == (decimal)EHSQuestionId.CompletionDate ||
							q.QuestionId == (decimal)EHSQuestionId.ProjectedCompletionDate)
						{
							rdp.Enabled = false;
						}

						pnl.Controls.Add(rdp);
						break;

					case EHSIncidentQuestionType.Time:
						var rtp = new RadTimePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rtp.ShowPopupOnFocus = true;
						rtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						if (shouldPopulate)
						{
							TimeSpan parseTime;
							if (TimeSpan.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), out parseTime))
								rtp.SelectedTime = parseTime;
						}
						pnl.Controls.Add(rtp);
						break;

					case EHSIncidentQuestionType.DateTime:
						var rdtp = new RadDateTimePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdtp.SelectedDate = parseDate;
						}
						pnl.Controls.Add(rdtp);
						break;

					case EHSIncidentQuestionType.BooleanCheckBox:
						pnl.Controls.Add(new LiteralControl("<div style=\"padding: 0 3px;\">"));
						var bcb = new CheckBox() { ID = qid, Text = Resources.LocalizedText.Yes, CssClass = "WarnIfChanged" };

						if (shouldPopulate)
							bcb.Checked = (q.AnswerText.ToLower() == "yes") ? true : false;
						else if (q.QuestionId == (decimal)EHSQuestionId.Create8D && EHSIncidentMgr.IsTypeDefault8D(typeId))
							bcb.Checked = true;

						// If controller question, "close" checkbox, or "create 8d" checkbox, register ajax behavior
						if ((q.QuestionControls != null && q.QuestionControls.Count > 0) ||
							q.QuestionId == (decimal)EHSQuestionId.CloseIncident ||
							q.QuestionId == (decimal)EHSQuestionId.Create8D)
						{
							bcb.CheckedChanged += new EventHandler(bcb_CheckedChanged);
							if (q.QuestionId == (decimal)EHSQuestionId.CloseIncident)
								bcb.CheckedChanged += new EventHandler(bcb_CheckedChangedClose);
							if (q.QuestionId == (decimal)EHSQuestionId.Create8D)
								bcb.CheckedChanged += new EventHandler(bcb_CheckedChangedCreate8D);
							bcb.AutoPostBack = true;
						}

						pnl.Controls.Add(bcb);

						pnl.Controls.Add(new LiteralControl("</div>"));
						break;

					case EHSIncidentQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						uploader.SetReportOption(false);
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
						{
							uploader.GetUploadedFiles(40, EditIncidentId, "");
							uploader.SetViewMode(EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted));
						}
						break;

					case EHSIncidentQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "2");
						break;

					case EHSIncidentQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "2");
						break;

					case EHSIncidentQuestionType.PageOneAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, "1");
						break;

					case EHSIncidentQuestionType.CurrencyTextBox:
						var ctb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Number };
						ctb.NumberFormat.DecimalDigits = 2;
						if (shouldPopulate)
							ctb.Text = q.AnswerText;

						pnl.Controls.Add(ctb);
						break;

					case EHSIncidentQuestionType.PercentTextBox:
						var ptb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Percent };
						if (shouldPopulate)
							ptb.Text = q.AnswerText;
						pnl.Controls.Add(ptb);
						break;

					case EHSIncidentQuestionType.StandardsReferencesDropdown:
						RadComboBox rcb = CreateReferencesDropdown(q.StandardType);
						rcb.ID = qid;
						rcb.Skin = "Metro";
						rcb.CssClass = "WarnIfChanged";
						rcb.Font.Size = 8;
						rcb.Width = 320;
						rcb.DropDownWidth = 400;
						rcb.Height = 300;
						if (shouldPopulate)
							rcb.SelectedValue = q.AnswerText;
						pnl.Controls.Add(rcb);
						break;

					case EHSIncidentQuestionType.LocationDropdown:
						rddlLocation = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						var plantIdList = SelectPlantIdsByAccessLevel();
						if (plantIdList.Count > 1)
							rddlLocation.Items.Add(new DropDownListItem("", ""));
						foreach (decimal pid in plantIdList)
						{
							string plantName = EHSIncidentMgr.SelectPlantNameById(pid);
							rddlLocation.Items.Add(new DropDownListItem(plantName, Convert.ToString(pid)));
						}
						if (shouldPopulate)
						{
							rddlLocation.SelectedValue = q.AnswerText;
							this.SelectedLocationId = Convert.ToDecimal(q.AnswerText);
						}
						rddlLocation.SelectedIndexChanged += rddlLocation_SelectedIndexChanged;
						rddlLocation.AutoPostBack = true;

						pnl.Controls.Add(rddlLocation);
						break;

					case EHSIncidentQuestionType.UsersDropdown:
						var rddl3 = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						rddl3.Items.Add(new DropDownListItem("", ""));

						var personList = new List<PERSON>();
						personList = EHSIncidentMgr.SelectIncidentPersonList(incident, true);

						foreach (PERSON p in personList)
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddl3.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
						}

						if (shouldPopulate)
							rddl3.SelectedValue = q.AnswerText;
						pnl.Controls.Add(rddl3);
						break;

					case EHSIncidentQuestionType.RequiredYesNoRadio:
						var rblYN = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblYN.RepeatDirection = RepeatDirection.Horizontal;
						rblYN.RepeatColumns = 2;
						rblYN.AutoPostBack = true;
						rblYN.Items.Add(new ListItem(Resources.LocalizedText.Yes, "Yes"));
						rblYN.Items.Add(new ListItem(Resources.LocalizedText.No, "No"));
						if (shouldPopulate)
							rblYN.SelectedValue = q.AnswerText;
						if (q.QuestionControls != null && q.QuestionControls.Count > 0)
						{
							rblYN.SelectedIndexChanged += rblYN_SelectedIndexChanged;
						}
						pnl.Controls.Add(rblYN);
						break;

					case EHSIncidentQuestionType.UsersDropdownLocationFiltered:
						rddlFilteredUsers = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						BuildFilteredUsersDropdownList();

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;

						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident))
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && !UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident))
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/>"));

			UpdateAnswersFromForm();

			UpdateButtonText();


		}

		void rddlLocation_SelectedIndexChanged(object sender, EventArgs e)
		{
			BuildFilteredUsersDropdownList();
		}

		void tb_TextRequiredClosedChanged(object sender, EventArgs e)
		{
			UpdateClosedQuestions();
		}

		void rdp_SelectedDateRequiredClosedChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
		{
			UpdateClosedQuestions();
		}

		void UpdateButtonText()
		{

			//btnSaveReturn.Text = Resources.LocalizedText.SaveAndReturn;

			int chInt = (int)EHSQuestionId.Create8D;
			string chString = chInt.ToString();
			CheckBox create8dCh = (CheckBox)pnlForm.FindControl(chString);

			if (create8dCh != null && create8dCh.Checked == true)
			{
				/*
				if (IsEditContext)
					btnSaveContinue.Text = "Save & Edit 8D";
				else
					btnSaveContinue.Text = "Save & Create 8D";
				*/
			}
			else
			{
				/*
				if (IsEditContext)
					btnSaveContinue.Text = "Save & Edit Report";
				else
					btnSaveContinue.Text = Resources.LocalizedText.SaveAndCreateReport;
			*/
			}

			if (IsEditContext)
			{
				if (PageMode == PageUseMode.ViewOnly)
					SetSubnav("alert");
				else 
					SetSubnav("edit");
			}
			else
				SetSubnav("new");
		}

		void bcb_CheckedChangedCreate8D(object sender, EventArgs e)
		{
			UpdateButtonText();
		}

		void bcb_CheckedChangedClose(object sender, EventArgs e)
		{
			// Handle the Close Incident event, autopopulate fields
			UpdateClosedQuestions();
		}

		void BuildFilteredUsersDropdownList()
		{
			if (rddlLocation != null)
			{
				if (!string.IsNullOrEmpty(rddlLocation.SelectedValue))
					this.SelectedLocationId = Convert.ToDecimal(rddlLocation.SelectedValue);
				else
					this.SelectedLocationId = 0;
			}

			if (rddlFilteredUsers != null)
			{
				rddlFilteredUsers.Items.Clear();
				rddlFilteredUsers.Items.Add(new DropDownListItem("", ""));

				var locationPersonList = new List<PERSON>();
				if (this.SelectedLocationId > 0)
				{
					locationPersonList = EHSIncidentMgr.SelectEhsPeopleAtPlant(this.SelectedLocationId);
				}
				//else
				//	locationPersonList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

				if (locationPersonList.Count > 0)
				{
					foreach (PERSON p in locationPersonList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						// Check for duplicate list items
						var ddli = rddlFilteredUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
						if (ddli == null)
							rddlFilteredUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					// If only one user, select by default
					if (rddlFilteredUsers.Items.Count() == 2)
						rddlFilteredUsers.SelectedIndex = 1;
				}
				else
				{
					rddlFilteredUsers.Items[0].Text = "[No valid users - please change location]";
				}
			}
		}

		void UpdateClosedQuestions()
		{
			// Close checkbox
			int chInt = (int)EHSQuestionId.CloseIncident;
			string chString = chInt.ToString();
			CheckBox closeCh = (CheckBox)pnlForm.FindControl(chString);

			if (closeCh != null)
			{
				// Completion date
				int cdInt = (int)EHSQuestionId.CompletionDate;
				string cdString = cdInt.ToString();
				RadDatePicker cdFormControl = (RadDatePicker)pnlForm.FindControl(cdString);

				// Completed by
				int cbInt = (int)EHSQuestionId.CompletedBy;
				string cbString = cbInt.ToString();
				RadTextBox cbFormControl = (RadTextBox)pnlForm.FindControl(cbString);

				if (closeCh.Checked)
				{
					// Check if incident report required fields are filled out
					if (IncidentReportRequiredFieldsComplete() == true)
					{
						if (cbFormControl != null)
						{
							cbFormControl.Text = SessionManager.UserContext.UserName();
							cbFormControl.Enabled = false;
						}
						if (cdFormControl != null)
						{
							cdFormControl.SelectedDate = DateTime.Now;
							cdFormControl.Enabled = false;
						}
					}
					else
					{
						closeCh.Checked = false;
						string script = string.Format("alert('{0}');", "You must complete all required fields on this page to close the incident.");
						ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
					}
				}
				else
				{
					if (cbFormControl != null)
					{
						cbFormControl.Text = "";
						cbFormControl.Enabled = false;
					}
					if (cdFormControl != null)
					{
						cdFormControl.Clear();
						cbFormControl.Enabled = false;
					}
				}
			}
		}

		void UpdateClosedQuestionsPrevent()
		{
			if (IncidentReportRequiredFieldsComplete() == true)
			{
			}
		}

		protected bool IncidentReportRequiredFieldsComplete()
		{
			int score = 0;
			foreach (int fId in requiredToCloseFields)
			{
				var field = pnlForm.FindControl(fId.ToString());
				if (field == null)   //mt - need to enable close if field was not included in the incident meta-data
				{
					score++;
				}
				else
				{
					if (field is RadTextBox)
					{
						if (!String.IsNullOrEmpty((field as RadTextBox).Text))
							score++;
					}
					else if (field is RadDatePicker)
					{
						if ((field as RadDatePicker).SelectedDate != null)
							score++;
					}
					else if (field is DropDownList)
					{
						if (!String.IsNullOrEmpty((field as DropDownList).SelectedValue))
							score++;
					}
					else if (field is RadDropDownList)
					{
						if (!String.IsNullOrEmpty((field as RadDropDownList).SelectedValue))
							score++;
					}
				}
			}

			return (score == requiredToCloseFields.Length);
		}

		protected void bcb_CheckedChanged(object sender, EventArgs e)
		{
			ProcessQuestionControlEvent(Convert.ToDecimal((sender as CheckBox).ID));
		}

		void rblYN_SelectedIndexChanged(object sender, EventArgs e)
		{
			ProcessQuestionControlEvent(Convert.ToDecimal((sender as RadioButtonList).ID));
		}

		protected void ProcessQuestionControlEvent(decimal questionId)
		{
			controlQuestionChanged = true;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			if (typeId < 1)
				return;
			questions = EHSIncidentMgr.SelectIncidentQuestionList(typeId, companyId, CurrentStep);
			EHSIncidentQuestion question = questions.Where(q => q.QuestionId == questionId).FirstOrDefault();
			ProcessQuestionControls(question, 0);

			controlQuestionChanged = false;
		}

		protected bool IsControllQuestion(EHSIncidentQuestion q)
		{
			if (questions.Where(l => l.QuestionControls.Select(c => c.INCIDENT_QUESTION_ID).FirstOrDefault() == q.QuestionId).Count() > 0
				|| questions.Where(l => l.QuestionControls.Select(c => c.SECONDARY_QUESTION_ID).FirstOrDefault() == q.QuestionId).Count() > 0)
				return true;
			else
				return false;
		}

		protected void UpdateControlledQuestions()
		{
			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			questions = EHSIncidentMgr.SelectIncidentQuestionList(typeId, companyId, CurrentStep);
			UpdateAnswersFromForm();

			foreach (var q in questions)
			{
				if (q.QuestionControls != null && q.QuestionControls.Count > 0)
					ProcessQuestionControls(q, 0);
			}

			UpdateButtonText(); // One last check to fix 8d button text
		}

		protected void ProcessQuestionControls(EHSIncidentQuestion question, int depth)
		{
			if (depth > 1)
				return;

			depth++;

			if (question.QuestionControls != null)
			{
				foreach (INCIDENT_QUESTION_CONTROL control in question.QuestionControls)
				{
					Panel containerControl = (Panel)pnlForm.FindControl("Panel" + control.INCIDENT_QUESTION_AFFECTED_ID);
					Label formLabel = (Label)pnlForm.FindControl("Label" + control.INCIDENT_QUESTION_AFFECTED_ID);
					var formControl = pnlForm.FindControl(control.INCIDENT_QUESTION_AFFECTED_ID.ToString());

					string answer = question.AnswerText;
					var triggerVal = control.TRIGGER_VALUE;
					bool criteriaIsMet = false;
					if (triggerVal.Contains("|"))
					{
						var arrTrigger = triggerVal.Split('|');
						criteriaIsMet = arrTrigger.Contains(answer);
					}
					else
					{
						criteriaIsMet = (answer == triggerVal);
					}

					if (criteriaIsMet)
					{
						// Check for optional secondary criteria on control question
						if (control.SECONDARY_QUESTION_ID != null && control.SECONDARY_TRIGGER_VALUE != null)
						{
							var secondaryControl = pnlForm.FindControl(control.SECONDARY_QUESTION_ID.ToString());

							if (secondaryControl is RadDropDownList)
							{
								string secondaryValue = (secondaryControl as RadDropDownList).SelectedValue;
								criteriaIsMet = (control.SECONDARY_TRIGGER_VALUE == secondaryValue);
							}
						}
					}

					var localControl = control;
					EHSIncidentQuestion affectedQuestion = questions.FirstOrDefault(q => q.QuestionId == localControl.INCIDENT_QUESTION_AFFECTED_ID);

					if (containerControl != null && affectedQuestion != null)
					{
						switch (control.ACTION)
						{
							case "Show":

								if (criteriaIsMet)
								{
									containerControl.Enabled = true;
									formLabel.ForeColor = System.Drawing.Color.Black;
									if (formControl is CheckBox)
										(formControl as CheckBox).ForeColor = System.Drawing.Color.Black;
									else if (formControl is RadioButtonList)
										(formControl as RadioButtonList).ForeColor = System.Drawing.Color.Black;
									else if (formControl is RadDropDownList)
									{
										(formControl as RadDropDownList).Enabled = true;
									}
								}
								else
								{
									var greyColor = System.Drawing.Color.FromArgb(200, 200, 200);
									containerControl.Enabled = false;
									formLabel.ForeColor = greyColor;
									if (formControl is CheckBox)
									{
										(formControl as CheckBox).Checked = false;
										(formControl as CheckBox).ForeColor = greyColor;
									}
									else if (formControl is RadioButtonList)
									{
										(formControl as RadioButtonList).SelectedValue = Resources.LocalizedText.No;
										(formControl as RadioButtonList).ForeColor = greyColor;
									}
									else if (formControl is RadDropDownList)
									{
										(formControl as RadDropDownList).SelectedValue = "";
										(formControl as RadDropDownList).Enabled = false;
									}
								}

								break;

							case "Force":
								if (formControl is CheckBox)
								{
									var cb = (formControl as CheckBox);
									if (criteriaIsMet)
									{
										cb.Checked = true;

										affectedQuestion.AnswerText = "Yes";

										// Recursively process any other controls triggered by a forced checkbox
										if (affectedQuestion.QuestionControls != null && affectedQuestion.QuestionControls.Count > 0)
											ProcessQuestionControls(affectedQuestion, depth);
									}
									else
									{
										// Only force to false if the result of a parent checkbox changing (avoids changes on form rebuilds)
										if (controlQuestionChanged)
											cb.Checked = false;
									}

									cb.Enabled = (answer != triggerVal);
								}
								else if (formControl is RadioButtonList)
								{
									var rbl = (formControl as RadioButtonList);
									if (criteriaIsMet)
									{
										rbl.SelectedValue = Resources.LocalizedText.Yes;
										affectedQuestion.AnswerText = "Yes";

										// Recursively process any other controls triggered by a forced checkbox
										if (affectedQuestion.QuestionControls != null && affectedQuestion.QuestionControls.Count > 0)
											ProcessQuestionControls(affectedQuestion, depth);
									}
									else
									{
										// Only force to false if the result of a parent checkbox changing (avoids changes on form rebuilds)
										if (controlQuestionChanged)
											rbl.SelectedValue = Resources.LocalizedText.No;
									}

									rbl.Enabled = (answer != triggerVal);
								}

								break;
						}
					}
				}
			}
		}

		protected RadComboBox CreateReferencesDropdown(string standard)
		{
			List<STANDARDS_REFERENCES> srList = SQMStandardsReferencesMgr.SelectReferencesByStandard("IMS");
			var rcb = new RadComboBox();
			rcb.Items.Add(new RadComboBoxItem(""));
			foreach (var s in srList)
			{
				string combined = s.SECTION + " - " + s.DESCRIPTION;
				rcb.Items.Add(new RadComboBoxItem(combined, combined));
			}
			return rcb;
		}

		protected void AddToolTip(Panel container, EHSIncidentQuestion question)
		{
			var imgHelp = new System.Web.UI.WebControls.Image()
			{
				ID = "help_" + question.QuestionId.ToString(),
				ImageUrl = "/images/ico-question.png",
			};

			var rttHelp = new RadToolTip()
			{
				Text = "<div style=\"font-size: 11px; line-height: 1.5em;\">" + question.HelpText + "</div>",
				TargetControlID = imgHelp.ID,
				IsClientID = false,
				RelativeTo = ToolTipRelativeDisplay.Element,
				Width = 400,
				Height = 200,
				Animation = ToolTipAnimation.Fade,
				Position = ToolTipPosition.MiddleRight,
				ContentScrolling = ToolTipScrolling.Auto,
				Skin = "Metro",
				HideEvent = ToolTipHideEvent.LeaveTargetAndToolTip
			};
			pnlForm.Controls.Add(new LiteralControl("<span style=\"float: right;\">"));
			container.Controls.Add(imgHelp);
			pnlForm.Controls.Add(new LiteralControl("</span>"));
			container.Controls.Add(rttHelp);
		}

		public void InitNewIncident(decimal newTypeID, decimal newLocationID)
		{
			if (newTypeID > 0)
			{
				SessionManager.SetIncidentLocation(newLocationID);
				IncidentLocationId = newLocationID;
				IncidentLocationTZ = SessionManager.IncidentLocation.Plant.LOCAL_TIMEZONE;
				SelectedTypeId = Convert.ToDecimal(newTypeID);
				SelectedTypeText = EHSIncidentMgr.SelectIncidentType(newTypeID, SessionManager.UserContext.Language.NLS_LANGUAGE).TITLE;
				CreatePersonId = 0;
				EditIncidentId = 0;
				IncidentStepCompleted = 0;
				IsEditContext = false;
				BuildForm();
			}
		}

		public void BindIncident(decimal incidentID)
		{
			IsEditContext = true;
			EditIncidentId = incidentID;
			PageMode = PageUseMode.EditEnabled;
			IncidentStepCompleted = 0;
			BuildForm();
		}

		public void BindIncidentAlert(decimal incidentID)
		{
			IsEditContext = true;
			EditIncidentId = incidentID;
			PageMode = PageUseMode.ViewOnly;
			IncidentStepCompleted = 0;
			BuildForm();
			SetSubnav("alert");
		}

		#endregion


		#region Form Events

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				CurrentSubnav = (sender as RadButton).CommandArgument;
				CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
				Save(false);
			}
			else
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnSaveContinue_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				Save(false);
			}
			else
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if (EditIncidentId > 0)
			{
				divForm.Visible = false;
				//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = false;

				//btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSIncidentMgr.DeleteIncident(EditIncidentId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Incident deleted." : "Error deleting incident.";
				lblResults.Text += "</div>";
			}
		}

		protected void Save(bool shouldReturn)
		{
			INCIDENT theIncident = null;
			decimal incidentId = 0;
			bool shouldCreate8d = false;
			string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			//SessionManager.ClearIncidentLocation();

			if (shouldReturn == true)
			{
				divForm.Visible = false;
				//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = false;

				pnlAddEdit.Visible = false;
				//btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;

				RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
				if (rcbWarnNavigate != null)
					rcbWarnNavigate.Visible = false;

				lblResults.Visible = true;
			}

			questions = EHSIncidentMgr.SelectIncidentQuestionList(SelectedTypeId, companyId, CurrentStep);
			UpdateAnswersFromForm();
			GetIncidentInfoFromQuestions(questions);

			if (CurrentStep == 0)
			{
				GetIncidentInfoFromQuestions(questions);
				if (!IsEditContext)
				{
					// Add context - step 0
					theIncident = CreateNewIncident();
					EditIncidentId = incidentId = theIncident.INCIDENT_ID;
					IsEditContext = true;
					//EHSNotificationMgr.NotifyOnCreate(incidentId, selectedPlantId);
					EHSNotificationMgr.NotifyIncidentStatus(theIncident, ((int)SysPriv.originate).ToString(), "");
				}
				else
				{
					// Edit context - step 0
					incidentId = EditIncidentId;
					if (incidentId > 0)
					{
						theIncident = UpdateIncident(incidentId);
						EHSIncidentMgr.TryCloseIncident(incidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
					}
				}
				if (incidentId > 0)
				{
					shouldCreate8d = AddOrUpdateAnswers(questions, incidentId);
					SaveAttachments(incidentId);
				}
			}
			else if (CurrentStep == 1)
			{
				// Edit context - step 1
				incidentId = EditIncidentId;
				if (incidentId > 0)
				{
					AddOrUpdateAnswers(questions, incidentId);
					shouldCreate8d = false;
					SaveAttachments(incidentId);

					UpdateTaskInfo(questions, incidentId, DateTime.Now);
					EHSIncidentMgr.TryCloseIncident(incidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
				}
			}

			if (shouldReturn)
				Response.Redirect("/EHS/EHS_Incidents.aspx");  // mt - temporary
		}

		#endregion


		#region Save Methods


		protected void UpdateAnswersFromForm()
		{
			// Save to dates & times to database using US culture info
			//DateTimeFormatInfo fromDtfi = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
			//DateTimeFormatInfo toDtfi = new CultureInfo("en-US", false).DateTimeFormat;

			foreach (var q in questions)
			{
				if (q.QuestionId == 107)
				{
					;
				}
				var control = pnlForm.FindControl(q.QuestionId.ToString());
				string answer = "";
				if (control != null)
				{
					switch (q.QuestionType)
					{
						case EHSIncidentQuestionType.TextField:
							answer = (control as RadTextBox).Text;
							break;

						case EHSIncidentQuestionType.TextBox:
							answer = (control as RadTextBox).Text;
							if (q.QuestionId == (decimal)EHSQuestionId.Description)
								incidentDescription = answer;
							break;
						case EHSIncidentQuestionType.NativeLangTextBox:
							answer = (control as RadTextBox).Text;
							if (q.QuestionId == (decimal)EHSQuestionId.Description)
								incidentDescription = answer;
							break;
						case EHSIncidentQuestionType.RichTextBox:
							answer = (control as RadEditor).Content;
							if (q.QuestionId == (decimal)EHSQuestionId.RecommendationSummary)
								incidentDescription = answer;
							break;

						case EHSIncidentQuestionType.Radio:
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSIncidentQuestionType.CheckBox:
							foreach (var item in (control as CheckBoxList).Items)
								if ((item as ListItem).Selected)
									answer += (item as ListItem).Value + "|";
							break;

						case EHSIncidentQuestionType.Dropdown:
							answer = (control as RadDropDownList).SelectedValue;
							break;

						case EHSIncidentQuestionType.Date:
							DateTime? fromDate = (control as RadDatePicker).SelectedDate;
							if (fromDate != null)
							{
								answer = ((DateTime)fromDate).ToString(CultureInfo.GetCultureInfo("en-US"));
								if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate || q.QuestionId == (decimal)EHSQuestionId.InspectionDate)
									incidentDate = (DateTime)fromDate;
							}
							break;

						case EHSIncidentQuestionType.Time:
							TimeSpan? fromTime = (control as RadTimePicker).SelectedTime;
							if (fromTime != null)
								answer = ((TimeSpan)fromTime).ToString("c", CultureInfo.GetCultureInfo("en-US"));
							break;

						case EHSIncidentQuestionType.DateTime:
							DateTime? fromDateTime = (control as RadDateTimePicker).SelectedDate;
							if (fromDateTime != null)
								answer = ((DateTime)fromDateTime).ToString(CultureInfo.GetCultureInfo("en-US"));
							break;

						case EHSIncidentQuestionType.BooleanCheckBox:
							answer = (control as CheckBox).Checked ? "Yes" : "No";
							break;

						case EHSIncidentQuestionType.Attachment:
							uploader = (control as Ucl_RadAsyncUpload);
							//foreach (var file1 in uploader.RAUpload.fil
							foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
								answer += file.FileName + "|";
							break;

						case EHSIncidentQuestionType.CurrencyTextBox:
							answer = (control as RadNumericTextBox).Text;
							break;

						case EHSIncidentQuestionType.PercentTextBox:
							answer = (control as RadNumericTextBox).Text;
							break;

						case EHSIncidentQuestionType.StandardsReferencesDropdown:
							answer = (control as RadComboBox).SelectedValue;
							break;

						case EHSIncidentQuestionType.LocationDropdown:
							answer = (control as RadDropDownList).SelectedValue;
							if (!string.IsNullOrEmpty(answer))
								selectedPlantId = Convert.ToDecimal(answer);
							break;

						case EHSIncidentQuestionType.UsersDropdown:
							answer = (control as RadDropDownList).SelectedValue;
							break;

						case EHSIncidentQuestionType.RequiredYesNoRadio:
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSIncidentQuestionType.PageOneAttachment:
							uploader = (control as Ucl_RadAsyncUpload);
							foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
								answer += file.FileName + "|";
							break;

						case EHSIncidentQuestionType.UsersDropdownLocationFiltered:
							answer = (control as RadDropDownList).SelectedValue;
							break;
					}
				}
				q.AnswerText = answer;
			}

			if (incidentDate == null || incidentDate < DateTime.UtcNow.AddYears(-100))
				incidentDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = selectedPlantId;
		}

		protected void GetIncidentInfoFromQuestions(List<EHSIncidentQuestion> questions)
		{

		}

		protected bool AddOrUpdateAnswers(List<EHSIncidentQuestion> questions, decimal incidentId)
		{
			bool shouldCreate8d = false;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var incidentAnswer = (from ia in entities.INCIDENT_ANSWER
									  where ia.INCIDENT_ID == incidentId
										  && ia.INCIDENT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();
				if (incidentAnswer != null)
				{
					incidentAnswer.ANSWER_VALUE = q.AnswerText;
					incidentAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText;
				}
				else
				{
					incidentAnswer = new INCIDENT_ANSWER()
					{
						INCIDENT_ID = incidentId,
						INCIDENT_QUESTION_ID = q.QuestionId,
						ANSWER_VALUE = q.AnswerText,
						ORIGINAL_QUESTION_TEXT = q.QuestionText
					};
					entities.AddToINCIDENT_ANSWER(incidentAnswer);
				}

				// Check if Quality Issue (8D) question set to true
				if (q.QuestionId == (decimal)EHSQuestionId.Create8D && q.AnswerText == "Yes")
					shouldCreate8d = true;
			}
			entities.SaveChanges();

			return shouldCreate8d;
		}

		protected INCIDENT CreateNewIncident()
		{
			decimal incidentId = 0;
			var newIncident = new INCIDENT()
			{
				DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
				DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
				DETECT_PLANT_ID = SessionManager.IncidentLocation.Plant.PLANT_ID,
				INCIDENT_TYPE = "EHS",
				CREATE_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
				LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = incidentDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				INCIDENT_DT = incidentDate,
				ISSUE_TYPE = SelectedTypeText,
				ISSUE_TYPE_ID = SelectedTypeId,
				INCFORM_LAST_STEP_COMPLETED = 100
			};
			entities.AddToINCIDENT(newIncident);

			if (entities.SaveChanges() > 0)
			{
				incidentId = newIncident.INCIDENT_ID;
			}

			return newIncident;
		}

		protected INCIDENT UpdateIncident(decimal incidentId)
		{
			INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				incident.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				incident.DETECT_PLANT_ID = selectedPlantId;
				incident.INCIDENT_TYPE = "EHS";
				//incident.CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				incident.DESCRIPTION = incidentDescription;
				//incident.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				incident.INCIDENT_DT = incidentDate;
				incident.ISSUE_TYPE = SelectedTypeText;
				incident.ISSUE_TYPE_ID = SelectedTypeId;
				incident.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
				incident.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				if (incident.INCFORM_LAST_STEP_COMPLETED < 100)
					incident.INCFORM_LAST_STEP_COMPLETED = 100;

				entities.SaveChanges();
			}

			return incident;

		}

		protected void SaveAttachments(decimal incidentId)
		{
			if (uploader != null)
			{
				string recordStep = (this.CurrentStep + 1).ToString();

				// Add files to database
				SessionManager.DocumentContext = new DocumentScope().CreateNew(
					SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
					SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
					);
				SessionManager.DocumentContext.RecordType = 40;
				SessionManager.DocumentContext.RecordID = incidentId;
				SessionManager.DocumentContext.RecordStep = recordStep;
				uploader.SaveFiles();
			}
		}

		protected void UpdateTaskInfo(List<EHSIncidentQuestion> questions, decimal incidentId, DateTime createDate)
		{
			DateTime dueDate = DateTime.MinValue;
			decimal responsiblePersonId = 0;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var incidentAnswer = (from ia in entities.INCIDENT_ANSWER
									  where ia.INCIDENT_ID == incidentId
										  && ia.INCIDENT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();

				if (incidentAnswer != null && !String.IsNullOrEmpty(incidentAnswer.ANSWER_VALUE))
				{
					if (q.QuestionId == (decimal)EHSQuestionId.DateDue)
						dueDate = DateTime.Parse(incidentAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US"));
					else if (q.QuestionId == (decimal)EHSQuestionId.ResponsiblePersonDropdown)
						responsiblePersonId = Convert.ToDecimal(incidentAnswer.ANSWER_VALUE);
				}
			}

			if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			{
				//int recordTypeId = (Mode == IncidentMode.Prevent) ? 45 : 40;
				int recordTypeId = 40;
				EHSIncidentMgr.CreateOrUpdateTask(incidentId, ((int)SysPriv.update).ToString(), 0, responsiblePersonId, recordTypeId, dueDate, "", "");
			}
		}

		protected void Create8dAndRedirect(decimal incidentId)
		{
			SessionManager.ReturnStatus = true;

			PROB_CASE probCase = ProblemCase.LookupCaseByIncident(incidentId);
			if (probCase != null)
			{
				// If 8D problem case exists, redirect with problem case ID
				SessionManager.ReturnObject = probCase.PROBCASE_ID;
			}
			else
			{
				// Otherwise, redirect with the intent of creating a new problem case
				var entities = new PSsqmEntities();
				SessionManager.ReturnObject = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			}

			Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
		}

		protected void GoToNextStep(decimal incidentId)
		{
			// Go to next step (report)
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = incidentId;
		}

		protected void ShowIncidentDetails(decimal incidentId, string result)
		{
			// Display incident details control
			btnDelete.Visible = false;
			lblResults.Text = result.ToString();
			var displaySteps = new int[] { CurrentStep };
		}

		#endregion

		#region subnav
		private void SetSubnav(string context)
		{
			if (context == "new")
			{
				uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = uclVideoPanel.Visible = false;
				btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavAlert.Visible = btnSubnavVideo.Visible = false;
				btnSubnavInitialActionApproval.Visible = btnSubnavCorrectiveActionApproval.Visible = false;
				btnDelete.Visible = false;
				uploader.SetViewMode(true);
			}
			else if (context == "alert")
			{
				btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavAlert.Visible = btnSubnavVideo.Visible = false;
				btnSubnavInitialActionApproval.Visible = btnSubnavCorrectiveActionApproval.Visible = false;
				uclcontain.Visible = uclroot5y.Visible = uclCausation.Visible = uclaction.Visible = uclapproval.Visible = true;
				uploader.SetViewMode(false);
				uploader.SetReportOption(false);

				uclcontain.IsEditContext = true;
				uclcontain.IncidentId = EditIncidentId;
				uclcontain.PageMode = PageUseMode.ViewOnly;
				uclcontain.PopulateInitialForm();

				uclroot5y.IsEditContext = true;
				uclroot5y.IncidentId = EditIncidentId;
				uclroot5y.PageMode = PageUseMode.ViewOnly;
				uclroot5y.PopulateInitialForm();

				uclCausation.IsEditContext = true;
				uclCausation.IncidentId = EditIncidentId;
				uclCausation.PageMode = PageUseMode.ViewOnly;
				uclCausation.PopulateInitialForm(entities);

				uclaction.IsEditContext = true;
				uclaction.IncidentId = EditIncidentId;
				uclaction.PageMode = PageUseMode.ViewOnly;
				uclaction.PopulateInitialForm();

				uclapproval.IsEditContext = true;
				uclapproval.IncidentId = EditIncidentId;
				uclapproval.PageMode = PageUseMode.ViewOnly;
				uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == SelectedTypeId &&  s.STEP == 10.0m).FirstOrDefault());
			}
			else
			{
				divSubnavPage.Visible = uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = uclVideoPanel.Visible = false;
				btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavVideo.Visible = true;
				btnSubnavIncident.Visible = true;
				btnSubnavIncident.Enabled = false;
				btnSubnavIncident.CssClass = "buttonLinkDisabled";
				btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
				btnDelete.Visible = EHSIncidentMgr.CanDeleteIncident(CreatePersonId, IncidentStepCompleted);

				btnSubnavVideo.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 1.1m);
				btnSubnavVideo.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavVideo.Text;
				btnSubnavAlert.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 11.0m);
				btnSubnavAlert.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "PreventativeMeasure").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavAlert.Text;

				btnSubnavInitialActionApproval.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 2.5m);
				btnSubnavInitialActionApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "InitialActionApproval").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "InitialActionApproval").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavInitialActionApproval.Text;
				btnSubnavCorrectiveActionApproval.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 5.5m);
				btnSubnavCorrectiveActionApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "CorrectiveActionApproval").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "CorrectiveActionApproval").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavCorrectiveActionApproval.Text;

				btnSubnavApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "Approvals").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "Approvals").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavApproval.Text;
			}
		}

		protected void btnSubnavSave_Click(object sender, EventArgs e)
		{
			int status = 0;
			bool isEdit = IsEditContext;

			btnSubnavIncident.Visible = btnSubnavApproval.Visible = btnSubnavAction.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavContainment.Visible = btnSubnavVideo.Visible = true;

			decimal incidentId = EditIncidentId;
			//decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			switch (CurrentSubnav)
			{
				case "2":
					if ((status = uclcontain.AddUpdateINCFORM_CONTAIN(incidentId)) >= 0)
						btnSubnav_Click(btnSubnavContainment, null);
					break;
				case "2.5":
					if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
						btnSubnav_Click(btnSubnavInitialActionApproval, null);
					break;
				case "3":
					if ((status = uclroot5y.AddUpdateINCFORM_ROOT5Y(incidentId)) >= 0)
						btnSubnav_Click(btnSubnavRootCause, null);
					break;
				case "4":
					if ((status = uclCausation.UpdateCausation(EditIncidentId)) >= 0)
						btnSubnav_Click(btnSubnavCausation, null);
					break;
				case "5":
					if ((status = uclaction.AddUpdateINCFORM_ACTION(incidentId)) >= 0)
						btnSubnav_Click(btnSubnavAction, null);
					break;
				case "5.5":
					if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
						btnSubnav_Click(btnSubnavInitialActionApproval, null);
					break;
				case "10":
					if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
						btnSubnav_Click(btnSubnavApproval, null);
					break;
				case "11":
					// save cross-plant alerts
					break;
				default:
					if (IsEditContext)
					{
						btnSaveContinue_Click(sender, null);
						BuildForm();
					}
					else
					{
						btnSaveReturn_Click(sender, null);
					}
					break;
					break;
			}

			if (status >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnSubnav_Click(object sender, EventArgs e)
		{
			LinkButton btn = (LinkButton)sender;

			pnlForm.Visible = divSubnavPage.Visible = uclcontain.Visible = uclroot5y.Visible = uclCausation.Visible = uclaction.Visible = uclapproval.Visible = uclAlert.Visible = uclVideoPanel.Visible = false;
			btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
			CurrentSubnav = btn.CommandArgument;

			btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = btnSubnavRootCause.Enabled = btnSubnavCausation.Enabled = btnSubnavContainment.Enabled = btnSubnavVideo.Enabled = true;
			btnSubnavIncident.CssClass = btnSubnavContainment.CssClass = btnSubnavRootCause.CssClass = btnSubnavCausation.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = btnSubnavAlert.CssClass = btnSubnavVideo.CssClass = "buttonLink";
			btnSubnavSave.Visible = btnDelete.Visible = false;

			btnSubnavVideo.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 1.1m);
			btnSubnavAlert.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 11.0m);

			btnSubnavInitialActionApproval.Visible = btnSubnavInitialActionApproval.Enabled = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 2.5m);
			btnSubnavInitialActionApproval.CssClass = "buttonLink";
			btnSubnavCorrectiveActionApproval.Visible = btnSubnavCorrectiveActionApproval.Enabled = EHSIncidentMgr.IsStepActive(incidentStepList, SelectedTypeId, 5.5m);
			btnSubnavCorrectiveActionApproval.CssClass = "buttonLink";

			lblPageTitle.Text = Resources.LocalizedText.Incident;

			//decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			switch (btn.CommandArgument)
			{
				case "1.1":
					lblPageTitle.Text = btnSubnavVideo.Text;
					uclVideoPanel.Visible = divSubnavPage.Visible = true;
					btnSubnavVideo.Enabled = false;
					btnSubnavVideo.CssClass = "buttonLinkDisabled";
					INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
					PageUseMode viewMode = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.originate, incident.INCFORM_LAST_STEP_COMPLETED) == true ? PageUseMode.EditEnabled : PageUseMode.ViewOnly;
					uclVideoPanel.OpenManageVideosWindow((int)TaskRecordType.HealthSafetyIncident, EditIncidentId, "1", (decimal)incident.DETECT_PLANT_ID, Resources.LocalizedText.VideoUpload, Resources.LocalizedText.VideoForIncident, "", "", "", viewMode, false, "");
					break;
				case "2":
					lblPageTitle.Text = btnSubnavContainment.Text;
					btnSubnavContainment.Enabled = false;
					btnSubnavContainment.CssClass = "buttonLinkDisabled";
					uclcontain.Visible = divSubnavPage.Visible = true;
					uclcontain.IsEditContext = true;
					uclcontain.IncidentId = EditIncidentId;
					uclcontain.PopulateInitialForm();
					break;
				case "3":
					lblPageTitle.Text = btnSubnavRootCause.Text;
					btnSubnavRootCause.Enabled = false;
					btnSubnavRootCause.CssClass = "buttonLinkDisabled";
					uclroot5y.Visible = divSubnavPage.Visible = true;
					uclroot5y.IsEditContext = true;
					uclroot5y.IncidentId = EditIncidentId;
					uclroot5y.PopulateInitialForm();
					break;
				case "4":
					lblPageTitle.Text = btnSubnavCausation.Text;
					btnSubnavCausation.Enabled = false;
					btnSubnavCausation.CssClass = "buttonLinkDisabled";
					uclCausation.Visible = divSubnavPage.Visible = true;
					uclCausation.IsEditContext = true;
					uclCausation.IncidentId = EditIncidentId;
					uclCausation.PopulateInitialForm(entities);
					break;
				case "5":
					lblPageTitle.Text = btnSubnavAction.Text;
					btnSubnavAction.Enabled = false;
					btnSubnavAction.CssClass = "buttonLinkDisabled";
					uclaction.Visible = divSubnavPage.Visible = true;
					uclaction.IsEditContext = true;
					uclaction.IncidentId = EditIncidentId;
					uclaction.PopulateInitialForm();
					break;
				// approval steps
				case "2.5":
					lblPageTitle.Text = btnSubnavInitialActionApproval.Text;
					btnSubnavInitialActionApproval.Enabled = false;
					btnSubnavInitialActionApproval.CssClass = "buttonLinkDisabled";
					uclapproval.IsEditContext = true;
					uclapproval.IncidentId = EditIncidentId;
					uclapproval.Visible = divSubnavPage.Visible = true;
					uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == SelectedTypeId && s.STEP == 2.5m).FirstOrDefault());
					break;
				case "5.5":
					lblPageTitle.Text = btnSubnavCorrectiveActionApproval.Text;
					btnSubnavCorrectiveActionApproval.Enabled = false;
					btnSubnavCorrectiveActionApproval.CssClass = "buttonLinkDisabled";
					uclapproval.IsEditContext = true;
					uclapproval.IncidentId = EditIncidentId;
					uclapproval.Visible = divSubnavPage.Visible = true;
					uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == SelectedTypeId && s.STEP == 5.5m).FirstOrDefault());
					break;
				case "10":
					lblPageTitle.Text = btnSubnavApproval.Text;
					btnSubnavApproval.Enabled = false;
					btnSubnavApproval.CssClass = "buttonLinkDisabled";
					uclapproval.IsEditContext = true;
					uclapproval.IncidentId = EditIncidentId;
					uclapproval.Visible = divSubnavPage.Visible = true;
					uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == SelectedTypeId && s.STEP == 10.0m).FirstOrDefault());
					break;
				case "11":
					lblPageTitle.Text = btnSubnavAlert.Text;
					btnSubnavAlert.Enabled = false;
					btnSubnavAlert.CssClass = "buttonLinkDisabled";
					uclAlert.IncidentId = EditIncidentId;
					uclAlert.Visible = divSubnavPage.Visible = true;
					uclAlert.PopulateInitialForm(entities);
					break;
				case "0":
				default:
					lblPageTitle.Text = btnSubnavIncident.Text;
					btnSubnavIncident.Visible = true;
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					if (pnlForm.Visible == false)
					{
						BuildForm();
						pnlForm.Visible = true;
					}
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
					btnDelete.Visible = EHSIncidentMgr.CanDeleteIncident(CreatePersonId, IncidentStepCompleted);
					break;
			}
		}
		#endregion

		public void ClearControls()
		{
			pnlForm.Controls.Clear();
		}

		protected void RefreshPageContext()
		{
			string typeString = Resources.LocalizedText.Incident;
			lblPageTitle.Text = typeString;

				if (!IsEditContext)
				{
					lblAddOrEditIncident.Text = "New" + "&nbsp" + typeString;
					lblIncidentType.Text = Resources.LocalizedText.IncidentType + ": ";
					lblIncidentType.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SelectedTypeText);
					lblIncidentLocation.Text = "Incident Location: ";
					lblIncidentLocation.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SessionManager.IncidentLocation.Plant.PLANT_NAME);
				}
				else
				{

					lblAddOrEditIncident.Text = typeString + "&nbsp" + WebSiteCommon.FormatID(EditIncidentId, 6);
					lblIncidentType.Text = Resources.LocalizedText.IncidentType + ": ";
					lblIncidentLocation.Text = "Incident Location: ";
					lblIncidentType.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SelectedTypeText);
					lblIncidentLocation.Text += EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(EditIncidentId);
				}

				UpdateControlledQuestions();
				UpdateButtonText();
		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			if (UserContext.GetMaxScopePrivilege(SysScope.incident) <= SysPriv.admin)
			{
				plantIdList = EHSIncidentMgr.SelectPlantIdsByCompanyId(companyId);
			}
			else
			{
				plantIdList = SessionManager.UserContext.PlantAccessList;
				if (plantIdList == null || plantIdList.Count == 0)
				{
					plantIdList.Add(SessionManager.UserContext.HRLocation.Plant.PLANT_ID);
				}
			}

			return plantIdList;
		}


	}
}