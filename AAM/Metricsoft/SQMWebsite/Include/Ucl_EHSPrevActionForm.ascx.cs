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
	public partial class Ucl_EHSPrevActionForm : System.Web.UI.UserControl
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

		protected Ucl_RadAsyncUpload uploader;
		protected Ucl_PreventionLocation preventionLocationForm;
		protected RadDropDownList rddlLocation;
		protected RadDropDownList rddlFilteredUsers;

		// Incident Custom Forms:
		protected Ucl_INCFORM_InjuryIllness injuryIllnessForm;


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

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			entities = new PSsqmEntities();
			controlQuestionChanged = false;

			btnSaveReturn.Visible = btnSaveContinue.Visible = false;

			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
			}

			if (IsPostBack)
			{
				LoadHeaderInformation();
				BuildForm();
			}
			else
			{
				ahReturn.HRef = "/EHS/EHS_PrevActions.aspx";
				//RefreshPageContext();
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
			List<DEPARTMENT> deptList = SQMModelMgr.SelectDepartmentList(entities, shouldPopulate ? (decimal)incident.DETECT_PLANT_ID : SessionManager.IncidentLocation.Plant.PLANT_ID);
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

			if (IsEditContext)
			{
				ShowIncidentDetails(EditIncidentId, "Recommendation Details", CurrentStep - 1);
			}

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
				incidentDescription = incident.DESCRIPTION;
			}

			pnlForm.Controls.Clear();
			divForm.Visible = true;
			lblResults.Visible = false;

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

				bool shouldPopulate = IsEditContext  && !string.IsNullOrEmpty(q.AnswerText);

				// set default answers when initial creation
				if (q.QuestionId == (decimal)EHSQuestionId.InspectionCategory  &&  !IsEditContext)
				{
					q.AnswerText = SelectedTypeText;
					shouldPopulate = true;
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

				if (q.QuestionId == 84)
				{
					;
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
						pnl.Controls.Add(tu);
						break;
					case EHSIncidentQuestionType.CurrentLocation:
						var tl = new RadTextBox() { ID = qid, Width = 250, MaxLength = MaxTextLength, Skin = "Metro" };
						if (shouldPopulate)
							tl.Text = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID).PLANT_NAME;
						else
							tl.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						tl.Enabled = false;
						pnl.Controls.Add(tl);
						break;

					case EHSIncidentQuestionType.TextField:
						var tf = new RadTextBox() { ID = qid, Width = 550, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
						{
							tf.Text = q.AnswerText;
						}
						else if (q.QuestionId == (decimal)EHSQuestionId.Location) // Location
							tf.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						if (q.QuestionId == (decimal)EHSQuestionId.CompletedBy)
							tf.Enabled = false;
						if (q.QuestionId == 31)  // auitor
						{
							tf.Enabled = false;
							if (incident.AUDIT_PERSON.HasValue)
								tf.Text = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)incident.AUDIT_PERSON, ""));
							else
								tf.Text = "";
						}
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
						pnl.Controls.Add(rddl);
						break;

					case EHSIncidentQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp = SQMBasePage.SetRadDateCulture(rdp, "");
						rdp.ShowPopupOnFocus = true;
						if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate) // Default incident date
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
						{
							rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
						}
						if (q.QuestionId == 30)
						{
							rdp.Enabled = false;
							if (incident.CLOSE_DATE.HasValue)
							{
								rdp.SelectedDate = incident.CLOSE_DATE;
							}
							else
							{
								rdp.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
							}
							shouldPopulate = false;
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
										DateTime parseDate = DateTime.UtcNow;
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
							bcb.AutoPostBack = true;
						}

						pnl.Controls.Add(bcb);

						pnl.Controls.Add(new LiteralControl("</div>"));
						break;

					case EHSIncidentQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep+1).ToString());
						uploader.SetReportOption(false);
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
						{
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep+1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
						}
						break;

					case EHSIncidentQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep + 1).ToString());
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep + 1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
						break;

					case EHSIncidentQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep + 1).ToString());
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep + 1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
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
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
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
						BuildFilteredUsersDropdownList(incident);

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;

						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.prevent))
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && !UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.prevent))
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>")); { }

			UpdateAnswersFromForm();

			SetPageAccess(incident, CurrentStep);

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
				btnSaveReturn.Visible = false;
				btnSaveContinue.Visible = false;
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
										DateTime parseDate = DateTime.UtcNow;
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
							bcb.AutoPostBack = true;
						}

						pnl.Controls.Add(bcb);

						pnl.Controls.Add(new LiteralControl("</div>"));
						break;

					case EHSIncidentQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep + 1).ToString());
						uploader.SetReportOption(false);
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
						{
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep + 1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[2] { SysPriv.action, SysPriv.update}, IncidentStepCompleted));
						}
						break;

					case EHSIncidentQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep + 1).ToString());
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep + 1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
						break;

					case EHSIncidentQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep((CurrentStep + 1).ToString());
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditIncidentId, (CurrentStep + 1).ToString());
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
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
							uploader.SetViewMode(EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted));
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
						BuildFilteredUsersDropdownList(incident);

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;

						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.prevent))
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && !UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.prevent))
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));

			UpdateAnswersFromForm();

			SetPageAccess(incident, CurrentStep);
		}

		void rddlLocation_SelectedIndexChanged(object sender, EventArgs e)
		{
			BuildFilteredUsersDropdownList(null);
		}

		void tb_TextRequiredClosedChanged(object sender, EventArgs e)
		{
			UpdateClosedQuestions();
		}

		void rdp_SelectedDateRequiredClosedChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
		{
			UpdateClosedQuestions();
		}

		void bcb_CheckedChangedClose(object sender, EventArgs e)
		{
			// Handle the Close Incident event, autopopulate fields
			UpdateClosedQuestions();
		}

		void BuildFilteredUsersDropdownList(INCIDENT incident)
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
					{
						locationPersonList = EHSIncidentMgr.SelectPrevActionPersonList(this.SelectedLocationId, new SysPriv[1] { SysPriv.action }, true);
					}
				}

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

					criteriaIsMet = true; // !!!! forcing all controls to display since autopostack not working with radajax panel

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

		public void InitNewIncident(decimal newTypeID, string inspectionType, decimal newLocationID)
		{
			if (newTypeID > 0)
			{
				SessionManager.SetIncidentLocation(newLocationID);
				IncidentLocationId = newLocationID;
				IncidentLocationTZ = SessionManager.IncidentLocation.Plant.LOCAL_TIMEZONE;
				SelectedTypeId = Convert.ToDecimal(newTypeID);
				SelectedTypeText = inspectionType;
				CreatePersonId = 0;
				EditIncidentId = 0;
				IncidentStepCompleted = 0;
				IsEditContext = false;
				CurrentStep = 0;
				BuildForm();
			}
		}

		public void BindIncident(decimal incidentID, int stepNumber, string redirectOverride)
		{
			IsEditContext = true;
			EditIncidentId = incidentID;
			IncidentStepCompleted = 0;
			CurrentStep = stepNumber;
			switch (CurrentStep)
			{
				case 1:
					lblPageTitle.Text = Resources.LocalizedText.CorrectiveAction;
					break;
				case 2:
					lblPageTitle.Text = Resources.LocalizedText.Resolution;
					break;
				default:
					lblPageTitle.Text = Resources.LocalizedText.Recommendation;
					break;
			}
			if (!string.IsNullOrEmpty(redirectOverride))
				ahReturn.HRef = redirectOverride;

			BuildForm();
		}

		#endregion


		#region Form Events

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				CurrentSubnav = (sender as RadButton).CommandArgument;
				CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
				INCIDENT theIncident = Save(false);

				SetPageAccess(theIncident, CurrentStep);

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
				INCIDENT theIncident = Save(false);

				SetPageAccess(theIncident, CurrentStep);
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

				btnSaveReturn.Visible = false;
				btnSaveContinue.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSIncidentMgr.DeleteIncident(EditIncidentId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Incident deleted." : "Error deleting incident.";
				lblResults.Text += "</div>";
			}
		}

		protected INCIDENT Save(bool shouldReturn)
		{
			INCIDENT theIncident = null;
			decimal incidentId = 0;
			bool shouldCreate8d = false;
			SysPriv notifyType = SysPriv.update;

			string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			//SessionManager.ClearIncidentLocation();

			if (shouldReturn == true)
			{
				divForm.Visible = false;
				//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = false;

				pnlAddEdit.Visible = false;
				btnSaveReturn.Visible = false;
				btnSaveContinue.Visible = false;

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
					notifyType = SysPriv.originate;
				}
				else
				{
					// Edit context - step 0
					incidentId = EditIncidentId;
					if (incidentId > 0)
					{
						theIncident = UpdateIncident(incidentId);  
					}
				}

				if (incidentId > 0)
				{
					shouldCreate8d = AddOrUpdateAnswers(questions, incidentId);
					SaveAttachments(incidentId);
					UpdateTaskInfo(questions, incidentId, DateTime.UtcNow);
				}
			}
			else
			{
				// Edit context - step 1
				incidentId = EditIncidentId;
				if (incidentId > 0)
				{
					theIncident = UpdateIncident(incidentId);  

					AddOrUpdateAnswers(questions, incidentId);

					SaveAttachments(incidentId);
					UpdateTaskInfo(questions, incidentId, DateTime.UtcNow);
				}
			}

			EHSNotificationMgr.NotifyPrevActionStatus(theIncident, ((int)notifyType).ToString(), "");

			decimal finalPlantId = 0;
			var finalIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			if (finalIncident != null)
				finalPlantId = (decimal)finalIncident.DETECT_PLANT_ID;
			else
				finalPlantId = selectedPlantId;

			if (shouldReturn)
				Response.Redirect("/EHS/EHS_PrevActions.aspx");  // mt - temporary

			return theIncident;
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
								//DateTime theDate = WebSiteCommon.LocalTime(Convert.ToDateTime(fromDate), IncidentLocationTZ);
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
			}
			entities.SaveChanges();

			return shouldCreate8d;
		}

		protected INCIDENT CreateNewIncident()
		{
			decimal incidentId = 0;
			var newIncident = new INCIDENT()
			{
				DETECT_COMPANY_ID = (decimal)SessionManager.IncidentLocation.Plant.COMPANY_ID,
				DETECT_BUS_ORG_ID = (decimal)SessionManager.IncidentLocation.Plant.BUS_ORG_ID,
				DETECT_PLANT_ID = SessionManager.IncidentLocation.Plant.PLANT_ID,
				INCIDENT_TYPE = "EHS",
				CREATE_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
				LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = incidentDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				INCIDENT_DT = incidentDate,
				ISSUE_TYPE = questions.Where(l => l.QuestionId == (decimal)EHSQuestionId.InspectionCategory).Select(s => s.AnswerText).FirstOrDefault(),
				ISSUE_TYPE_ID = SelectedTypeId,
				INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined
			};
			newIncident = EHSIncidentMgr.UpdatePrevActionStatus(newIncident, questions, CurrentStep, newIncident.LAST_UPD_DT);

			entities.AddToINCIDENT(newIncident);
			entities.SaveChanges();
			incidentId = newIncident.INCIDENT_ID;

			return newIncident;
		}

		protected INCIDENT UpdateIncident(decimal incidentId)
		{
			INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.DESCRIPTION = incidentDescription;
				incident.INCIDENT_DT = incidentDate;
				incident.ISSUE_TYPE = questions.Where(l => l.QuestionId == (decimal)EHSQuestionId.InspectionCategory).Select(s => s.AnswerText).FirstOrDefault();
				incident.ISSUE_TYPE_ID = SelectedTypeId;
				incident.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
				incident.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;

				incident = EHSIncidentMgr.UpdatePrevActionStatus(incident, questions, CurrentStep, incident.LAST_UPD_DT);

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
			string detailDesc = "";

			foreach (var q in questions)
			{
				if (!String.IsNullOrEmpty(q.AnswerText))
				{
					{
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate)
							dueDate = createDate.AddDays(30);  // per TI the due date should be based on the incident CREATE date instead of the inspection date
						else if (q.QuestionId == (decimal)EHSQuestionId.AssignToPerson)
							responsiblePersonId = Convert.ToDecimal(q.AnswerText);
						else if (q.QuestionId == (decimal)EHSQuestionId.RecommendationSummary)
							detailDesc = q.AnswerText;
					}
				}
			}

			if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			{
				EHSIncidentMgr.CreateOrUpdateTask(incidentId, ((int)SysPriv.update).ToString(), 0, responsiblePersonId, 45, dueDate, "", detailDesc);
			}
		}

		protected void GoToNextStep(decimal incidentId)
		{
			// Go to next step (report)
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = incidentId;
		}

		protected void ShowIncidentDetails(decimal incidentId, string result, int stepToDisplay)
		{
			if (stepToDisplay > -1)
			{
				lblResults.Text = result.ToString();
				divDetails.Visible = uclIncidentDetails.Visible = true;
				var displaySteps = new int[] { stepToDisplay };
				uclIncidentDetails.Refresh(incidentId, displaySteps);
			}
			else
			{
				lblResults.Text = result.ToString();
				divDetails.Visible = uclIncidentDetails.Visible = false;
			}
		}

		#endregion

		#region subnav

		private void SetPageAccess(INCIDENT incident, int actionStep)
		{
			btnSaveReturn.Text = Resources.LocalizedText.SaveAndReturn;  // ???

			if (incident == null)  // new incident
			{
				btnSubnavIncident.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = false;
				btnDelete.Visible = false;
			}
			else
			{
				btnSubnavIncident.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
				btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = true;
				btnSubnavIncident.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = "buttonLink";
			}

			switch (actionStep)
			{
				case 0:
					pnlForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[1] {SysPriv.originate}, IncidentStepCompleted);
					btnDelete.Visible = EHSIncidentMgr.CanDeletePrevAction(CreatePersonId, IncidentStepCompleted);
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					break;
				case 1:
					pnlForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, IncidentStepCompleted);
					btnDelete.Visible = false;
					btnSubnavAction.Enabled = false;
					btnSubnavAction.CssClass = "buttonLinkDisabled";
					break;
				case 2:
					pnlForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdatePrevAction(incident, IsEditContext, new SysPriv[3] { SysPriv.approve, SysPriv.approve1, SysPriv.approve2 }, IncidentStepCompleted);
					btnDelete.Visible = false;
					btnSubnavApproval.Enabled = false;
					btnSubnavApproval.CssClass = "buttonLinkDisabled";
					break;
				default:
					pnlForm.Enabled = btnSubnavSave.Visible = btnDelete.Visible = false;
					break;
			}
		}

		protected void btnSubnavSave_Click(object sender, EventArgs e)
		{
			int status = 0;

			if (IsEditContext)
			{
				btnSaveContinue_Click(sender, null);
				BuildForm();
			}
			else
			{
				btnSaveReturn_Click(sender, null);
			}

			if (status >= 0)
			{
				//string script = string.Format("alert('{0}');", "Your updates have been saved.");
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnSubnav_Click(object sender, EventArgs e)
		{
			LinkButton btn = (LinkButton)sender;
			CurrentSubnav = btn.CommandArgument;

			btnSubnavIncident.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
			btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = true;
			btnSubnavIncident.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = "buttonLink";

			switch (btn.CommandArgument)
			{
				case "4":
					lblPageTitle.Text = Resources.LocalizedText.CorrectiveAction;
					BindIncident(EditIncidentId, 1, "");
					break;
				case "5":
					lblPageTitle.Text = Resources.LocalizedText.Resolution;
					BindIncident(EditIncidentId, 2, "");
					break;
				case "0":
				default:
					lblPageTitle.Text = Resources.LocalizedText.Recommendation;
					BindIncident(EditIncidentId, 0, "");
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
			if (!IsEditContext)
			{
				lblAddOrEditIncident.Text = "New" + "&nbsp" + Resources.LocalizedText.Recommendation;
				lblIncidentType.Text =  SelectedTypeText;
				lblIncidentLocation.Text = SessionManager.IncidentLocation.Plant.PLANT_NAME;
			}
			else
			{
				lblAddOrEditIncident.Text = Resources.LocalizedText.Recommendation + "&nbsp" + WebSiteCommon.FormatID(EditIncidentId, 6);
				lblIncidentType.Text  = SelectedTypeText;
				lblIncidentLocation.Text = EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(EditIncidentId);
			}

			UpdateControlledQuestions();
		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			if (UserContext.GetMaxScopePrivilege(SysScope.prevent) <= SysPriv.admin)
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