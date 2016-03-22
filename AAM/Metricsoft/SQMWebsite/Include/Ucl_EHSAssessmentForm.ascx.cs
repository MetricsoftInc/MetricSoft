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
	public partial class Ucl_EHSAssessmentForm : System.Web.UI.UserControl
	{
		const Int32 MaxTextLength = 4000;

		int[] requiredToCloseFields = new int[] {
				(int)EHSAuditQuestionId.Containment,
				(int)EHSAuditQuestionId.RootCause,
				(int)EHSAuditQuestionId.RootCauseOperationalControl,
				(int)EHSAuditQuestionId.CorrectiveActions,
				(int)EHSAuditQuestionId.Verification,
				(int)EHSAuditQuestionId.ResponsiblePersonDropdown,
				(int)EHSAuditQuestionId.DateDue
			};

		//static SQMMetricMgr HSCalcs;
		public event GridItemClick2 OnAttachmentListItemClick;
		public event GridItemClick2 OnExceptionListItemClick;

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected decimal auditPlantId = 0;

		bool controlQuestionChanged;

		List<EHSAuditQuestion> questions;
		PSsqmEntities entities;

		protected Ucl_RadAsyncUpload uploader;
		protected Ucl_PreventionLocation preventionLocationForm;
		protected RadDropDownList rddlLocation;
		protected RadDropDownList rddlFilteredUsers;

		// Special answers used in AUDIT table
		string auditDescription = "";
		protected DateTime auditDate;
		protected decimal auditTypeId;
		protected string auditType;
		protected bool allQuestionsAnswered;

		public Ucl_EHSAuditDetails AuditDetails
		{
			get { return uclAuditDetails; }
		}

        public void EnableReturnButton(bool bEnabled)
        {
            ahReturn.Visible = bEnabled;
        }

		// Mode should be "audit" (standard) or "prevent" (RMCAR)
		public AuditMode Mode
		{
			get { return ViewState["Mode"] == null ? AuditMode.Audit : (AuditMode)ViewState["Mode"]; }
			set { ViewState["Mode"] = value; }
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

		public decimal EditAuditId
		{
			get { return ViewState["EditAuditId"] == null ? 0 : (decimal)ViewState["EditAuditId"]; }
			set { ViewState["EditAuditId"] = value; }
		}

		public decimal InitialPlantId
		{
			get { return ViewState["InitialPlantId"] == null ? 0 : (decimal)ViewState["InitialPlantId"]; }
			set { ViewState["InitialPlantId"] = value; }
		}

		protected decimal EditAuditTypeId
		{
			get { return EditAuditId == null ? 0 : EHSAuditMgr.SelectAuditTypeIdByAuditId(EditAuditId); }
		}

		protected decimal SelectedTypeId
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

		public string EditReturnState
		{
			get { return ViewState["EditReturnState"] == null ? " " : (string)ViewState["EditReturnState"]; }
			set { ViewState["EditReturnState"] = value; }
		}


		protected void Page_Init(object sender, EventArgs e)
		{
			//uclAuditForm.OnExceptionListItemClick += AddTask;
			//uclTaskList.OnTaskAdd += UpdateTaskList;
			//uclTaskList.OnTaskUpdate += UpdateTaskList;
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			entities = new PSsqmEntities();
			controlQuestionChanged = false;

			//if (Mode == AuditMode.Audit)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx";
			//else if (Mode == AuditMode.Prevent)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx?mode=prevent";

			UpdateAuditTypes();

			bool returnFromClick = false;
			bool addingExtraInfo = false;
			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
				returnFromClick = true;
				EditReturnState = "NeedsRefresh";
			}

			if (sourceId != null && (sourceId.Contains("AddTask") || sourceId.Contains("FileUpload")))
			{
				addingExtraInfo = true;
			}

			//if (IsPostBack && EditReturnState.Equals("NeedsRefresh"))
			if (IsPostBack)
			{
				divAuditForm.Visible = true;
				if (!returnFromClick && !addingExtraInfo)
				{
					LoadHeaderInformation();
				}
				BuildForm();
				EditReturnState = "NoRefresh";
			}
			else
			{
				//RefreshPageContext();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				//var sourceId = Page.Request[Page.postEventSourceID];

				//if ((sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn"))) || sourceId == "")
				//{
				//	UpdateControlledQuestions();
				//}
			}
		}

		#region Form

		protected void UpdateAuditTypes()
		{
			if (!IsEditContext)
			{
				var auditTypeList = new List<AUDIT_TYPE>();
				string selectString = "";
				if (Mode == AuditMode.Audit)
				{
					auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId, true);
					selectString = "[Select An Assessment Type]";
				}
				if (auditTypeList.Count > 0)
					auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });
				
				rddlAuditType.DataSource = auditTypeList;
				rddlAuditType.DataTextField = "TITLE";
				rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
				rddlAuditType.DataBind();
			}
		}

		protected void UpdateDepartments(decimal selectedplantID)
		{
			rddlDepartment.Items.Clear();
			var departmentList = new List<DEPARTMENT>();
			string selectString = "";
			try
			{
				departmentList = SQMModelMgr.SelectDepartmentList(entities, selectedplantID);
			}
			catch
			{ }
			selectString = "[Select A Department]";
			departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = 0, DEPT_NAME = "Plant Wide" });
			departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = -1, DEPT_NAME = selectString });

			rddlDepartment.DataSource = departmentList;
			rddlDepartment.DataTextField = "DEPT_NAME";
			rddlDepartment.DataValueField = "DEPT_ID";
			rddlDepartment.DataBind();
		}

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info
			if (IsEditContext || CurrentStep > 0)
			{
				// in edit mode, load the header field values and make all fields display only
				AUDIT audit = EHSAuditMgr.SelectAuditById(entities, EditAuditId);
				BusinessLocation location = new BusinessLocation().Initialize((decimal)audit.DETECT_PLANT_ID);
				rddlAuditType.Enabled = false;
				rddlAuditType.Visible = false;

				lblAuditLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblAuditLocation.Visible = true;
				ddlAuditLocation.Visible = false;
				mnuAuditLocation.Visible = false;

				lblAuditDescription.Text = audit.DESCRIPTION;
				lblAuditDescription.Visible = true;
				tbDescription.Visible = false;

				// build the audit user list
				lblAuditPersonName.Text = EHSAuditMgr.SelectUserNameById((Decimal)audit.AUDIT_PERSON);
				lblAuditPersonName.Visible = true;
				rddlAuditUsers.Visible = false;

				lblAuditDueDate.Text = audit.AUDIT_DT.ToString("MM/dd/yyyy");
				lblAuditDueDate.Visible = true;
				dmAuditDate.Enabled = false;
				dmAuditDate.Visible = false;

				if (rddlDepartment.Items.Count == 0)
				{
					UpdateDepartments((decimal)audit.DETECT_PLANT_ID);
					rddlDepartment.SelectedValue = "-1";
				}
				
				if (audit.DEPT_ID != null && rddlDepartment.SelectedIndex == 0)
				{
					rddlDepartment.SelectedValue = audit.DEPT_ID.ToString();
					lblDepartment.Text = rddlDepartment.SelectedText.ToString();
				}

				if (!IsEditContext)
				{
					lblDepartment.Visible = true;
					rddlDepartment.Enabled = false;
					rddlDepartment.Visible = false;
				}
				else if (SessionManager.UserContext.Person.PERSON_ID == audit.AUDIT_PERSON && !audit.CURRENT_STATUS.Equals("C"))
				{
					lblDepartment.Visible = false;
					rddlDepartment.Enabled = true;
					rddlDepartment.Visible = true;
				}
				else
				{
					lblDepartment.Visible = true;
					rddlDepartment.Enabled = false;
					rddlDepartment.Visible = false;
				}
			}
			else
			{
				if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.config)
				{
					// List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
					List<BusinessLocation> locationList = SessionManager.PlantList;
					locationList = UserContext.FilterPlantAccessList(locationList);
					if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone,android") == false)
					{
						if (mnuAuditLocation.Items.Count == 0)
						{
							mnuAuditLocation.Items.Clear();

							ddlAuditLocation.Visible = false;
							mnuAuditLocation.Visible = true;
							mnuAuditLocation.Enabled = true;
							SQMBasePage.SetLocationList(mnuAuditLocation, locationList, 0, "Select a Location", "", true);
						}
					}
					else
					{
						if (ddlAuditLocation.Items.Count == 0)
						{
							ddlAuditLocation.Items.Clear();
							ddlAuditLocation.Visible = true;
							ddlAuditLocation.Enabled = true;
							mnuAuditLocation.Visible = false;
							SQMBasePage.SetLocationList(ddlAuditLocation, locationList, 0, true);
							//ddlAuditLocation.Items.Insert(0, new RadComboBoxItem("", ""));
						}
					}
				}
				// set defaults for add mode
				rddlAuditType.Enabled = true;
				rddlAuditType.Visible = true;
				lblAuditLocation.Visible = false;
				lblAuditDescription.Visible = false;
				tbDescription.Visible = true;
				rddlAuditUsers.Enabled = true;
				rddlAuditUsers.Visible = true;
				lblAuditPersonName.Visible = false;
				lblAuditDueDate.Visible = false;
				dmAuditDate.Visible = true;
				dmAuditDate.Enabled = true;
				dmAuditDate.ShowPopupOnFocus = true;
				if (!dmAuditDate.SelectedDate.HasValue)
					dmAuditDate.SelectedDate = SessionManager.UserContext.LocalTime;
				//if (rddlDepartment.Items.Count == 0)
				//{
				//	UpdateDepartments((decimal)audit.DETECT_PLANT_ID);
				//}
				//if (audit.DEPT_ID != null)
				//{
				//	rddlDepartment.SelectedValue = audit.DEPT_ID.ToString();
				//	lblDepartment.Text = rddlDepartment.SelectedText.ToString();
				//}
				//else
				//{
				//	rddlDepartment.SelectedValue = "-1";
				//}
				rddlDepartment.Enabled = true;
				rddlDepartment.Visible = true;

			}
		}


		public void BuildForm()
		{
			// Currently, ALL audits will be like the Supervisory Audit example provided.  This is a simple Y/N/NA radio button and a comment box for each question.
			//    Each positive answer counts towards the percentage of total answers.

			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			AUDIT audit = null;
			if (EditAuditId > 0)
			{
				audit = (from aud in entities.AUDIT where aud.AUDIT_ID == EditAuditId select aud).FirstOrDefault();
			}

			string typeText = SelectedTypeText;
			auditType = EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);

			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);
			try
			{
				UpdateAnswersFromForm(); // just in case
			}
			catch { }

			pnlForm.Controls.Clear();
			pnlForm.Visible = true;
			lblResults.Visible = false;


			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"6\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));
			string previousTopic = "";
			string qid = "";
			string tid = "";
			string ptid = "";
			int percentInTopic = 0; // used to determine if the percentage value should show for the specific topic.
			bool showTotals = false; // are we showing totals at all for the audit?

			foreach (var q in questions)
			{
				var validator = new RequiredFieldValidator();

				// Look up answer if edit context
				var localQuestion = q;
				if (IsEditContext)
				{
					var auditAnswer = (from a in entities.AUDIT_ANSWER
									   where a.AUDIT_ID == EditAuditId
										   && a.AUDIT_QUESTION_ID == localQuestion.QuestionId
									   select a).FirstOrDefault();
					q.AnswerText = auditAnswer.ANSWER_VALUE;
					q.AnswerComment = auditAnswer.COMMENT;
				}

				//if (q.QuestionType == (decimal)EHSAuditQuestionType.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.SessionContext.Language().NLS_LANGUAGE) == false)
				//{
				//	continue;
				//}

				bool shouldPopulate = IsEditContext && !string.IsNullOrEmpty(q.AnswerText);

				qid = q.QuestionId.ToString();
				tid = "T" + q.TopicId.ToString();
				ptid = "P" + previousTopic;

				if (!previousTopic.Equals(q.TopicId.ToString())) // add a topic header
				{
					if (!previousTopic.Equals(""))
					{
						// need to add a display for the topic percentage
						var pnlPercent = new Panel() { ID = "Panel" + ptid };
						pnlPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
						if (percentInTopic > 0)
							pnlPercent.Controls.Add(new Label() { ID = "Label" + ptid, Text = "0%" }); // we will populate the values later
						pnlPercent.Controls.Add(new LiteralControl("</td></tr>"));
						pnlForm.Controls.Add(pnlPercent);
					}
					var pnlTopic = new Panel() { ID = "Panel" + tid };
					pnlTopic.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"blueCell\" style=\"width: 100%; font-weight: bold;\">"));
					pnlTopic.Controls.Add(new Label() { ID = "Label" + tid, Text = q.TopicTitle });
					pnlTopic.Controls.Add(new LiteralControl("</td></tr>"));
					pnlForm.Controls.Add(pnlTopic);
					previousTopic = q.TopicId.ToString();
					percentInTopic = 0;
				}

				var pnl = new Panel() { ID = "Panel" + qid };

				pnl.Controls.Add(new LiteralControl("<tr><td class=\"tanCell auditquestion\" style=\"width: 30%;\">"));
				pnl.Controls.Add(new Label() { ID = "Label" + qid, Text = q.QuestionText, AssociatedControlID = qid });
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (!string.IsNullOrEmpty(q.HelpText))
					AddToolTip(pnl, q);
				pnl.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				if (q.IsRequired)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredStar\">&bull;</span>"));
				if (q.IsRequiredClose)
					pnl.Controls.Add(new LiteralControl("<span class=\"requiredCloseStar\">&bull;</span>"));

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.RadioCommentLeft || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));
					string cid = "Comment" + qid;
					bool answerIsPositive = true;
					bool questionAnswered = false;
					if (q.AnswerText != null && !q.AnswerText.ToString().Equals(""))
					{
						foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
						{
							if (choice.Value.Equals(q.AnswerText) && !choice.ChoicePositive)
								answerIsPositive = false;
							if (choice.Value.Equals(q.AnswerText))
								questionAnswered = true;
						}
					}
					if (((q.CommentRequired && questionAnswered) || !answerIsPositive) && (q.AnswerComment == null || q.AnswerComment.Trim().Length == 0))
					{
						var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, CssClass = "audittextrequired" };
						comment.TextMode = InputMode.MultiLine;
						comment.Rows = 2;
						comment.Resize = ResizeMode.Both;
						comment.Text = q.AnswerComment;
						pnl.Controls.Add(comment);
					}
					else
					{
						var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						comment.TextMode = InputMode.MultiLine;
						comment.Rows = 2;
						comment.Resize = ResizeMode.Both;
						comment.Text = q.AnswerComment;
						pnl.Controls.Add(comment);
					}
				}

				pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));

				if (q.QuestionType != EHSAuditQuestionType.BooleanCheckBox && q.QuestionType != EHSAuditQuestionType.CheckBox &&
					q.QuestionType != EHSAuditQuestionType.Attachment && q.QuestionType != EHSAuditQuestionType.DocumentAttachment && 
					q.QuestionType != EHSAuditQuestionType.ImageAttachment && q.QuestionType != EHSAuditQuestionType.PageOneAttachment)
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

						if (q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.LocationDropdown ||
						q.QuestionType == EHSAuditQuestionType.StandardsReferencesDropdown || q.QuestionType == EHSAuditQuestionType.UsersDropdown ||
						q.QuestionType == EHSAuditQuestionType.UsersDropdownLocationFiltered)
							validator.InitialValue = "";

						if (Mode == AuditMode.Prevent)
							validator.EnableClientScript = false;

						pnl.Controls.Add(validator);
					}
				}

				switch (q.QuestionType)
				{
					case EHSAuditQuestionType.TextField:
						var tf = new RadTextBox() { ID = qid, Width = 550, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tf.Text = q.AnswerText;
						pnl.Controls.Add(tf);
						break;

					case EHSAuditQuestionType.TextBox:
						var tb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tb.Text = q.AnswerText;
						pnl.Controls.Add(tb);

						break;

					case EHSAuditQuestionType.NativeLangTextBox:
						var nltb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							nltb.Text = q.AnswerText;
						pnl.Controls.Add(nltb);
						break;

					case EHSAuditQuestionType.RichTextBox:
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

					case EHSAuditQuestionType.Radio:
					case EHSAuditQuestionType.RadioCommentLeft:
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged auditanswer" };
						rbl.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rbl.Items.Add(li);
						}
						//if (!shouldPopulate)
						//	rbl.SelectedIndex = 0; // Default to first
						pnl.Controls.Add(rbl);
						break;

					case EHSAuditQuestionType.RadioPercentage:
					case EHSAuditQuestionType.RadioPercentageCommentLeft:
						var rblp = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged auditanswer" };
						rblp.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rblp.Items.Add(li);
						}
						// all RBL will cause the percentages to change
						//rblp.AutoPostBack = true;
						//rblp.SelectedIndexChanged += rblp_SelectedIndexChanged;

						//if (!shouldPopulate)
						//	rbl.SelectedIndex = 0; // Default to first
						pnl.Controls.Add(rblp);
						percentInTopic += 1; // increase the value so we know if we should show the percentage value for the topic
						showTotals = true;
						break;

					case EHSAuditQuestionType.CheckBox:
						var cbl = new CheckBoxList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
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

					case EHSAuditQuestionType.Dropdown:
						var rddl = new RadDropDownList() { ID = qid, CssClass = "WarnIfChanged", Width = 550, Skin = "Metro", ValidationGroup = "Val" };
						rddl.Items.Add(new DropDownListItem("", ""));

						if (q.AnswerChoices != null && q.AnswerChoices.Count > 0)
						{
							// Check for any category headings
							var matches = q.AnswerChoices.Where(ac => ac.IsCategoryHeading == true);
							bool containsCategoryHeadings = (matches.Count() > 0);

							foreach (var choice in q.AnswerChoices)
							{
								if (containsCategoryHeadings == true)
								{
									if (choice.IsCategoryHeading)
										rddl.Items.Add(new DropDownListItem(choice.Value, "") { CssClass = "dropdownItemHeading", Enabled = false });
									else
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.Text, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.Text, choice.Value));
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

					case EHSAuditQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						rdp.ShowPopupOnFocus = true;
						
						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdp.SelectedDate = parseDate;
						}
						
						pnl.Controls.Add(rdp);
						break;

					case EHSAuditQuestionType.Time:
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

					case EHSAuditQuestionType.DateTime:
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

					case EHSAuditQuestionType.BooleanCheckBox:
						pnl.Controls.Add(new LiteralControl("<div style=\"padding: 0 3px;\">"));
						var bcb = new CheckBox() { ID = qid, Text = Resources.LocalizedText.Yes, CssClass = "WarnIfChanged" };

						if (shouldPopulate)
							bcb.Checked = (q.AnswerText.ToLower() == "yes") ? true : false;

						pnl.Controls.Add(bcb);
						
						pnl.Controls.Add(new LiteralControl("</div>"));
						break;

					case EHSAuditQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.PageOneAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "1");
						break;

					//case EHSAuditQuestionType.CurrencyTextBox:
					//	var ctb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Number };
					//	ctb.NumberFormat.DecimalDigits = 2;
					//	if (shouldPopulate)
					//		ctb.Text = q.AnswerText;

					//	if (EditAuditId > 0 && Mode == AuditMode.Prevent)
					//	{
					//		AUDIT audit = (from inc in entities.AUDIT where inc.AUDIT_ID == EditAuditId select inc).FirstOrDefault();
					//		if (audit != null)
					//		{
					//			string answer = EHSAuditMgr.SelectAuditAnswer(audit, (decimal)EHSAuditQuestionId.RecommendationType);
					//			if (!string.IsNullOrEmpty(answer) && answer.ToLower() != "infrastructure")
					//			{
					//				ctb.Enabled = false;
					//				pnl.Visible = false;
					//			}
					//		}
					//	}

					//	pnl.Controls.Add(ctb);
					//	break;

					case EHSAuditQuestionType.PercentTextBox:
						var ptb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Percent };
						if (shouldPopulate)
							ptb.Text = q.AnswerText;
						pnl.Controls.Add(ptb);
						break;

					case EHSAuditQuestionType.StandardsReferencesDropdown:
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

					case EHSAuditQuestionType.LocationDropdown:
						rddlLocation = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						var plantIdList = SelectPlantIdsByAccessLevel();
						if (plantIdList.Count > 1)
							rddlLocation.Items.Add(new DropDownListItem("", ""));
						foreach (decimal pid in plantIdList)
						{
							string plantName = EHSAuditMgr.SelectPlantNameById(pid);
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

					case EHSAuditQuestionType.UsersDropdown:
						var rddl3 = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						rddl3.Items.Add(new DropDownListItem("", ""));

						var personList = new List<PERSON>();
						if (CurrentStep == 1)
							personList = EHSAuditMgr.SelectAuditPersonList(EditAuditId);
						else if (CurrentStep == 0)
							personList = EHSAuditMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

						foreach (PERSON p in personList)
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddl3.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
						}

						if (shouldPopulate)
							rddl3.SelectedValue = q.AnswerText;
						pnl.Controls.Add(rddl3);
						break;

					case EHSAuditQuestionType.RequiredYesNoRadio:
						var rblYN = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblYN.RepeatDirection = RepeatDirection.Horizontal;
						rblYN.RepeatColumns = 2;
						rblYN.AutoPostBack = true;
						var choices = new string[] { Resources.LocalizedText.Yes, Resources.LocalizedText.No };
						foreach (var choice in choices)
						{
							var li = new ListItem(choice);
							rblYN.Items.Add(li);
						}
						if (shouldPopulate)
							rblYN.SelectedValue = q.AnswerText;
						if (q.QuestionControls != null && q.QuestionControls.Count > 0)
						{
							rblYN.SelectedIndexChanged += rblYN_SelectedIndexChanged;
						}
						pnl.Controls.Add(rblYN); 
						break;

					case EHSAuditQuestionType.UsersDropdownLocationFiltered:
						rddlFilteredUsers = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						BuildFilteredUsersDropdownList();

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;
						
						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
					pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));
					string cid = "Comment" + qid;
					bool answerIsPositive = true;
					bool questionAnswered = false;
					if (q.AnswerText != null && !q.AnswerText.ToString().Equals(""))
					{
						foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
						{
							if (choice.Value.Equals(q.AnswerText) && !choice.ChoicePositive)
								answerIsPositive = false;
							if (choice.Value.Equals(q.AnswerText))
								questionAnswered = true;
						}
					}
					if (((q.CommentRequired && questionAnswered) || !answerIsPositive) && (q.AnswerComment == null || q.AnswerComment.Trim().Length == 0))
					{
						var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, CssClass = "audittextrequired" };
						comment.TextMode = InputMode.MultiLine;
						comment.Rows = 2;
						comment.Resize = ResizeMode.Both;
						comment.Text = q.AnswerComment;
						pnl.Controls.Add(comment);
					}
					else
					{
						var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						comment.TextMode = InputMode.MultiLine;
						comment.Rows = 2;
						comment.Resize = ResizeMode.Both;
						comment.Text = q.AnswerComment;
						pnl.Controls.Add(comment);
					}
				}

				// Add a link to add followup tasks
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RadioCommentLeft || 
					q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));
					string lid = "AddTask" + qid;
					string buttonText = Resources.LocalizedText.AssignTask + "(" + q.TasksAssigned.ToString() + ")";
					RadButton lnk = new RadButton() { ID = lid, Text = buttonText, CssClass = "WarnIfChanged" };
					lnk.ToolTip = "Create a Task to complete this exception";
					lnk.Click += new System.EventHandler(lnkAddTask_Click);
					lnk.Enabled = true;
					lnk.AutoPostBack = true;
					lnk.CommandArgument = q.AuditId.ToString() + "," + qid;
					pnl.Controls.Add(lnk);
					pnl.Controls.Add(new LiteralControl("<span style=\"padding-left: 10px;\">"));
					lid = "FileUpload" + qid;
					buttonText = Resources.LocalizedText.Attachments + "(" + q.FilesAttached.ToString() + ")";
					lnk = new RadButton() { ID = lid, Text = buttonText, CssClass = "WarnIfChanged" };
					lnk.ToolTip = "Upload attachments for this question";
					lnk.Click += new System.EventHandler(lnkFileUpload_Click);
					lnk.Enabled = true;
					lnk.CommandArgument = q.AuditId.ToString() + "," + qid;
					pnl.Controls.Add(lnk);
					pnl.Controls.Add(new LiteralControl("</>"));
				}

				if (q.QuestionType != EHSAuditQuestionType.BooleanCheckBox && q.QuestionType != EHSAuditQuestionType.CheckBox &&
					q.QuestionType != EHSAuditQuestionType.Dropdown && q.QuestionType != EHSAuditQuestionType.PercentTextBox &&
					q.QuestionType != EHSAuditQuestionType.Radio && q.QuestionType != EHSAuditQuestionType.RadioCommentLeft &&
					q.QuestionType != EHSAuditQuestionType.RequiredYesNoRadio &&
					q.QuestionType != EHSAuditQuestionType.RadioPercentage && q.QuestionType != EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\"></td><td class=\"greyCell\">"));
				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				pnlForm.Controls.Add(pnl);
			}

			// add the final topic percent line and then an audit total percent line
			if (!previousTopic.Equals(""))
			{
				// need to add a display for the topic percentage
				Panel pnlPercent = new Panel() { ID = "PanelP" + previousTopic };
				pnlPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				if (percentInTopic > 0)
					pnlPercent.Controls.Add(new Label() { ID = "LabelP" + previousTopic, Text = "0%" }); // we will populate the values later
				pnlPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlForm.Controls.Add(pnlPercent);
			}
			if (showTotals)
			{
				Panel pnlTotalPercent = new Panel() { ID = "PanelTotalPercent" };
				//pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				//pnlTotalPercent.Controls.Add(new Label() { ID = "LabelTotalPercent", Text = "Total Positive Score:  0%" }); // we will populate the values later
				//pnlTotalPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">&nbsp;</td></tr>"));
				pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				pnlTotalPercent.Controls.Add(new Label() { ID = "LabelTotalPossiblePoints", Text = "Total Possible Points:  0" }); // we will populate the values later
				pnlTotalPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				pnlTotalPercent.Controls.Add(new Label() { ID = "LabelTotalPointsAchieved", Text = "Total Points Achieved:  0" }); // we will populate the values later
				pnlTotalPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"6\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				pnlTotalPercent.Controls.Add(new Label() { ID = "LabelTotalPointsPercentage", Text = "Percentage of Points Achieved:  0%" }); // we will populate the values later
				pnlTotalPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlForm.Controls.Add(pnlTotalPercent);
			}
			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));

			UpdateAnswersFromForm();

			UpdateButtonText();

			CalculatePercentages(questions);
		}

		protected void AuditLocation_Select(object sender, EventArgs e)
		{
			string location = "0";
			if (sender is RadMenu)
			{
				location = mnuAuditLocation.SelectedItem.Value;
				mnuAuditLocation.Items[0].Text = mnuAuditLocation.SelectedItem.Text;
			}
			else if (sender is RadComboBox)
			{
				location = ddlAuditLocation.SelectedValue;
			}
			BuildAuditUsersDropdownList(location);
			hdnAuditLocation.Value = location;
			// rebuild the department list
			if (!location.ToLower().Equals("top"))
			{
				UpdateDepartments(Convert.ToDecimal(location));
			}

			// need to rebuild the form
			string selectedTypeId = rddlAuditType.SelectedValue;
			if (!string.IsNullOrEmpty(selectedTypeId))
			{
				SelectedTypeId = Convert.ToDecimal(selectedTypeId);
				IsEditContext = false;
				BuildForm();
			}

		}

		public void GetForm()
		{
			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			AUDIT audit = null;
			if (EditAuditId > 0)
			{
				audit = (from aud in entities.AUDIT where aud.AUDIT_ID == EditAuditId select aud).FirstOrDefault();
			}

			pnlForm.Controls.Clear();
			divForm.Visible = true;
			//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = true;
			lblResults.Visible = false;

			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);

			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"5\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));
			string previousTopic = "";
			foreach (var q in questions)
			{
				var validator = new RequiredFieldValidator();

				// Look up answer if edit context
				var localQuestion = q;
				if (IsEditContext)
				{
					var auditAnswer = (from a in entities.AUDIT_ANSWER
									   where a.AUDIT_ID == EditAuditId
										   && a.AUDIT_QUESTION_ID == localQuestion.QuestionId
									   select a).FirstOrDefault();
					q.AnswerText = auditAnswer.ANSWER_VALUE;
					q.AnswerComment = auditAnswer.COMMENT;
				}

				//if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.SessionContext.Language().NLS_LANGUAGE) == false)
				//{
				//	continue;
				//}

				bool shouldPopulate = IsEditContext && !string.IsNullOrEmpty(q.AnswerText);

				//if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSAuditMgr.EnableNativeLangQuestion(SessionManager.SessionContext.Language().NLS_LANGUAGE) == false)
				//{
				//	continue;
				//}

				string qid = q.QuestionId.ToString();
				string tid = "T" + q.TopicId.ToString();

				if (!previousTopic.Equals(tid)) // add a topic header
				{
					var pnlTopic = new Panel() { ID = "Panel" + tid };
					pnlTopic.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"blueCell\" style=\"width: 100%;\">"));
					pnlTopic.Controls.Add(new Label() { ID = "Label" + tid, Text = q.TopicTitle });
					pnlTopic.Controls.Add(new LiteralControl("</td></tr>"));
					pnlForm.Controls.Add(pnlTopic);
					previousTopic = tid;
				}

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

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.RadioCommentLeft ||q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					pnl.Controls.Add(new LiteralControl("</td> class=\"greyCell\">"));
					string cid = "Comment" + qid;
					var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
					comment.TextMode = InputMode.MultiLine;
					comment.Rows = 2;
					comment.Resize = ResizeMode.Both;
					comment.Text = q.AnswerComment;
					comment.Text = q.AnswerComment;
					pnl.Controls.Add(comment);
				}

				pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));

				if (q.QuestionType != EHSAuditQuestionType.BooleanCheckBox && q.QuestionType != EHSAuditQuestionType.CheckBox &&
					q.QuestionType != EHSAuditQuestionType.Attachment && q.QuestionType != EHSAuditQuestionType.DocumentAttachment &&
					q.QuestionType != EHSAuditQuestionType.ImageAttachment && q.QuestionType != EHSAuditQuestionType.PageOneAttachment)
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

						if (q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.LocationDropdown ||
						q.QuestionType == EHSAuditQuestionType.StandardsReferencesDropdown || q.QuestionType == EHSAuditQuestionType.UsersDropdown ||
						q.QuestionType == EHSAuditQuestionType.UsersDropdownLocationFiltered)
							validator.InitialValue = "";

						if (Mode == AuditMode.Prevent)
							validator.EnableClientScript = false;

						pnl.Controls.Add(validator);
					}
				}

				switch (q.QuestionType)
				{
					case EHSAuditQuestionType.TextField:
						var tf = new RadTextBox() { ID = qid, Width = 550, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tf.Text = q.AnswerText;
						pnl.Controls.Add(tf);
						break;

					case EHSAuditQuestionType.TextBox:
						var tb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tb.Text = q.AnswerText;
						pnl.Controls.Add(tb);
						break;

					case EHSAuditQuestionType.NativeLangTextBox:
						var nltb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							nltb.Text = q.AnswerText;
						pnl.Controls.Add(nltb);
						break;

					case EHSAuditQuestionType.RichTextBox:
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

					case EHSAuditQuestionType.Radio:
					case EHSAuditQuestionType.RadioCommentLeft:
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rbl.Items.Add(li);
						}
						//if (!shouldPopulate)
						//	rbl.SelectedIndex = 0; // Default to first
						pnl.Controls.Add(rbl);
						break;

					case EHSAuditQuestionType.RadioPercentage:
					case EHSAuditQuestionType.RadioPercentageCommentLeft:
						var rblp = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblp.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
							// Don't try to explicitly set SelectedValue in case answer choice text changed in database
							if (shouldPopulate)
							{
								if (choice.Value == q.AnswerText)
									li.Selected = true;
							}
							rblp.Items.Add(li);
						}
						// all RBL will cause the percentages to change
						rblp.AutoPostBack = true;
						rblp.SelectedIndexChanged += rblp_SelectedIndexChanged;

						//if (!shouldPopulate)
						//	rbl.SelectedIndex = 0; // Default to first
						pnl.Controls.Add(rblp);
						break;

					case EHSAuditQuestionType.CheckBox:
						var cbl = new CheckBoxList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Text, choice.Value);
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

					case EHSAuditQuestionType.Dropdown:
						var rddl = new RadDropDownList() { ID = qid, CssClass = "WarnIfChanged", Width = 550, Skin = "Metro", ValidationGroup = "Val" };
						rddl.Items.Add(new DropDownListItem("", ""));

						if (q.AnswerChoices != null && q.AnswerChoices.Count > 0)
						{
							// Check for any category headings
							var matches = q.AnswerChoices.Where(ac => ac.IsCategoryHeading == true);
							bool containsCategoryHeadings = (matches.Count() > 0);

							foreach (var choice in q.AnswerChoices)
							{
								if (containsCategoryHeadings == true)
								{
									if (choice.IsCategoryHeading)
										rddl.Items.Add(new DropDownListItem(choice.Text, "") { CssClass = "dropdownItemHeading", Enabled = false });
									else
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.Text, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.Text, choice.Value));
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

					case EHSAuditQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						rdp.ShowPopupOnFocus = true;

						if (shouldPopulate)
						{
							DateTime parseDate;
							if (DateTime.TryParse(q.AnswerText, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
								rdp.SelectedDate = parseDate;
						}

						pnl.Controls.Add(rdp);
						break;

					case EHSAuditQuestionType.Time:
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

					case EHSAuditQuestionType.DateTime:
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

					case EHSAuditQuestionType.BooleanCheckBox:
						pnl.Controls.Add(new LiteralControl("<div style=\"padding: 0 3px;\">"));
						var bcb = new CheckBox() { ID = qid, Text = Resources.LocalizedText.Yes, CssClass = "WarnIfChanged" };

						if (shouldPopulate)
							bcb.Checked = (q.AnswerText.ToLower() == "yes") ? true : false;
						//else if (q.QuestionId == (decimal)EHSQuestionId.Create8D && EHSIncidentMgr.IsTypeDefault8D(typeId))
						//	bcb.Checked = true;

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

					case EHSAuditQuestionType.Attachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.DocumentAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.ImageAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("2");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						uploader.RAUpload.FileFilters.Add(new FileFilter("Images (.jpeg, .jpg, .png, .gif)", new string[] { ".jpeg", ".jpg", ".png", ".gif" }));
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "2");
						break;

					case EHSAuditQuestionType.PageOneAttachment:
						uploader = (Ucl_RadAsyncUpload)LoadControl("~/Include/Ucl_RadAsyncUpload.ascx");
						uploader.ID = qid;
						uploader.SetAttachmentRecordStep("1");
						// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
						uploader.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnSaveContinue", "btnDelete" };
						pnl.Controls.Add(uploader);
						// Data bind after adding the control to avoid radgrid "unwanted expand arrow" bug
						if (IsEditContext)
							uploader.GetUploadedFiles(40, EditAuditId, "1");
						break;

					//case EHSAuditQuestionType.CurrencyTextBox:
					//	var ctb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Number };
					//	ctb.NumberFormat.DecimalDigits = 2;
					//	if (shouldPopulate)
					//		ctb.Text = q.AnswerText;

					//	if (EditAuditId > 0 && Mode == AuditMode.Prevent)
					//	{
					//		//INCIDENT incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
					//		if (audit != null)
					//		{
					//			string answer = EHSAuditMgr.SelectAuditAnswer(audit, (decimal)EHSQuestionId.RecommendationType);
					//			if (!string.IsNullOrEmpty(answer) && answer.ToLower() != "infrastructure")
					//			{
					//				ctb.Enabled = false;
					//				pnl.Visible = false;
					//			}
					//		}
					//	}

					//	pnl.Controls.Add(ctb);
					//	break;

					case EHSAuditQuestionType.PercentTextBox:
						var ptb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Percent };
						if (shouldPopulate)
							ptb.Text = q.AnswerText;
						pnl.Controls.Add(ptb);
						break;

					case EHSAuditQuestionType.StandardsReferencesDropdown:
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

					case EHSAuditQuestionType.LocationDropdown:
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

					case EHSAuditQuestionType.RequiredYesNoRadio:
						var rblYN = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblYN.RepeatDirection = RepeatDirection.Horizontal;
						rblYN.RepeatColumns = 2;
						rblYN.AutoPostBack = true;
						var choices = new string[] { Resources.LocalizedText.Yes, Resources.LocalizedText.No };
						foreach (var choice in choices)
						{
							var li = new ListItem(choice);
							rblYN.Items.Add(li);
						}
						if (shouldPopulate)
							rblYN.SelectedValue = q.AnswerText;
						if (q.QuestionControls != null && q.QuestionControls.Count > 0)
						{
							rblYN.SelectedIndexChanged += rblYN_SelectedIndexChanged;
						}
						pnl.Controls.Add(rblYN);
						break;

					case EHSAuditQuestionType.UsersDropdownLocationFiltered:
						rddlFilteredUsers = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						BuildFilteredUsersDropdownList();

						if (shouldPopulate)
							rddlFilteredUsers.SelectedValue = q.AnswerText;

						if (rddlFilteredUsers.Items.Count() == 1)
							validator.InitialValue = rddlFilteredUsers.Items[0].Text;

						pnl.Controls.Add(rddlFilteredUsers);
						break;

				}

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
					pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));
					string cid = "Comment" + qid;
					var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
					comment.TextMode = InputMode.MultiLine;
					comment.Rows = 2;
					comment.Resize = ResizeMode.Both;
					comment.Text = q.AnswerComment;
					comment.Text = q.AnswerComment;
					pnl.Controls.Add(comment);
				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved)
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement)
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));

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
			btnSaveReturn.Text = Resources.LocalizedText.SaveAndReturn;

			if (IsEditContext)
				btnSaveContinue.Text = "Save Assessment";
			else
				btnSaveContinue.Text = "Create Assessment";

			if (IsEditContext)
				btnSaveReturn.Text = "Save Assessment";
			else
				btnSaveReturn.Text = "Create Assessment";
		}

		void bcb_CheckedChangedCreate8D(object sender, EventArgs e)
		{
			UpdateButtonText();
		}

		void bcb_CheckedChangedClose(object sender, EventArgs e)
		{
			// Handle the Close Audit event, autopopulate fields
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
					locationPersonList = EHSAuditMgr.SelectEhsPeopleAtPlant(this.SelectedLocationId);
				}
				//else
				//	locationPersonList = EHSAuditMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

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

		void BuildAuditUsersDropdownList(string location)
		{
			if (location != "")
			{
				rddlAuditUsers.Items.Clear();
				rddlAuditUsers.Items.Add(new DropDownListItem("", ""));
				var locationPersonList = new List<PERSON>();
				try
				{
					decimal locationId = Convert.ToDecimal(location);
					if (locationId > 0)
					{
						//locationPersonList = EHSAuditMgr.SelectEhsDataOriginatorsAtPlant(locationId);
						//locationPersonList.AddRange(EHSAuditMgr.SelectDataOriginatorAdditionalPlantAccess(locationId));
						locationPersonList = SQMModelMgr.SelectPrivGroupPersonList(SysPriv.originate, SysScope.audit, locationId);
						locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
					}
					//else
					//	locationPersonList = EHSAuditMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);
				}
				catch
				{ }
				if (locationPersonList.Count > 0)
				{
					foreach (PERSON p in locationPersonList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						// Check for duplicate list items
						var ddli = rddlAuditUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
						if (ddli == null)
							rddlAuditUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					// If only one user, select by default
					if (rddlAuditUsers.Items.Count() == 2)
						rddlAuditUsers.SelectedIndex = 1;
				}
				else
				{
					rddlAuditUsers.Items[0].Text = "[No valid users - please change location]";
				}
			}
		}

		void UpdateClosedQuestions()
		{
			// Close checkbox
			int chInt = (int)EHSAuditQuestionId.CloseAudit;
			string chString = chInt.ToString();
			CheckBox closeCh = (CheckBox)pnlForm.FindControl(chString);

			if (closeCh != null)
			{
				// Completion date
				int cdInt = (int)EHSAuditQuestionId.CompletionDate;
				string cdString = cdInt.ToString();
				RadDatePicker cdFormControl = (RadDatePicker)pnlForm.FindControl(cdString);

				// Completed by
				int cbInt = (int)EHSAuditQuestionId.CompletedBy;
				string cbString = cbInt.ToString();
				RadTextBox cbFormControl = (RadTextBox)pnlForm.FindControl(cbString);

				if (closeCh.Checked)
				{
					// Check if audit report required fields are filled out
					if (AuditReportRequiredFieldsComplete() == true)
					{
						if (cbFormControl != null)
						{
							cbFormControl.Text = SessionManager.UserContext.UserName();
							cbFormControl.Enabled = false;
						}
						if (cdFormControl != null)
						{
							cdFormControl.SelectedDate = SessionManager.UserContext.LocalTime;
							cdFormControl.Enabled = false;
						}
					}
					else
					{
						closeCh.Checked = false;
						//string script = string.Format("alert('{0}');", "You must complete all required fields on this page to close the assessment.");
						string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentIncompleteMsg);
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

		protected bool AuditReportRequiredFieldsComplete()
		{
			// AW20150820 we aren't really using this. need to figure out if we need to 

			//int score = 0;
			//foreach (int fId in requiredToCloseFields)
			//{
			//	var field = pnlForm.FindControl(fId.ToString());
			//	if (field == null)   //mt - need to enable close if field was not included in the audit meta-data
			//	{
			//		score++;
			//	}
			//	else 
			//	{
			//		if (field is RadTextBox)
			//		{
			//			if (!String.IsNullOrEmpty((field as RadTextBox).Text))
			//				score++;
			//		}
			//		else if (field is RadDatePicker)
			//		{
			//			if ((field as RadDatePicker).SelectedDate != null)
			//				score++;
			//		}
			//		else if (field is DropDownList)
			//		{
			//			if (!String.IsNullOrEmpty((field as DropDownList).SelectedValue))
			//				score++;
			//		}
			//		else if (field is RadDropDownList)
			//		{
			//			if (!String.IsNullOrEmpty((field as RadDropDownList).SelectedValue))
			//				score++;
			//		}
			//	}
			//}

			//return (score == requiredToCloseFields.Length);
			return false;
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

			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;
			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);
			EHSAuditQuestion question = questions.Where(q => q.QuestionId == questionId).FirstOrDefault();
			ProcessQuestionControls(question, 0);

			controlQuestionChanged = false;
		}

		protected void UpdateControlledQuestions()
		{
			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			// why do we keep getting the questions?
			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);
			UpdateAnswersFromForm();

			foreach (var q in questions)
			{
				if (q.QuestionControls != null && q.QuestionControls.Count > 0)
					ProcessQuestionControls(q, 0);
			}

			UpdateButtonText(); // One last check to fix 8d button text
		}

		protected void ProcessQuestionControls(EHSAuditQuestion question, int depth)
		{
			if (depth > 1)
				return;

			depth++;

			if (question.QuestionControls != null)
			{
				foreach (AUDIT_QUESTION_CONTROL control in question.QuestionControls)
				{
					Panel containerControl = (Panel)pnlForm.FindControl("Panel" + control.AUDIT_QUESTION_AFFECTED_ID);
					Label formLabel = (Label)pnlForm.FindControl("Label" + control.AUDIT_QUESTION_AFFECTED_ID);
					var formControl = pnlForm.FindControl(control.AUDIT_QUESTION_AFFECTED_ID.ToString());

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
					EHSAuditQuestion affectedQuestion = questions.FirstOrDefault(q => q.QuestionId == localControl.AUDIT_QUESTION_AFFECTED_ID);

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

		void rblp_SelectedIndexChanged(object sender, EventArgs e)
		{
			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;
			// add the logic to recalculate the percentages
			UpdateAnswersFromForm();
			CalculatePercentages(questions);
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

		protected void AddToolTip(Panel container, EHSAuditQuestion question)
		{
			var imgHelp = new System.Web.UI.WebControls.Image()
			{
				ID = "help_" + question.QuestionId.ToString(),
				ImageUrl = "/images/ico-question.png",
			};

			var rttHelp = new RadToolTip()
			{
				Text = "<div style=\"font-size: 11px; line-height: 1.5em;\" data-html=\"true\" >" + question.HelpText + "</div>",
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

		public void CheckForSingleType()
		{
			if (rddlAuditType.Items.Count == 1)
			{
				string selectedTypeId = rddlAuditType.Items[0].Value;
				if (!string.IsNullOrEmpty(selectedTypeId))
				{
					SelectedTypeId = Convert.ToDecimal(selectedTypeId);
					IsEditContext = false;
					BuildForm();
				}
			}
		}

		#endregion


		#region Form Events

		protected void rddlAuditType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedTypeId = rddlAuditType.SelectedValue;
			if (!string.IsNullOrEmpty(selectedTypeId))
			{
				SelectedTypeId = Convert.ToDecimal(selectedTypeId);
				IsEditContext = false;
				BuildForm();
			}
		}

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				//CurrentSubnav = (sender as RadButton).CommandArgument;
				try
				{
					CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
				}
				catch
				{
					CurrentStep = 0;
				}
				if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0) || (rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0) || (rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
				{
					string requiredFields = "";
					if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += Resources.LocalizedText.BusinessLocation;
					}
					if ((rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += "Assessment Person";
					}
					if ((rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += Resources.LocalizedText.Department;
					}
					if (requiredFields.Trim().Length > 0)
						requiredFields = " (" + requiredFields + ")";
					string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg + requiredFields);
					ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				}
				else
					Save(true);
			}
			else
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnSaveContinue_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				if (hdnAuditLocation.Value.ToString().Trim().Length == 0 || rddlAuditUsers.SelectedIndex == 0 || rddlDepartment.SelectedIndex == 0)
				{
					string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
					ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				}
				else
					Save(false);
			}
			else
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if (EditAuditId > 0)
			{
				pnlForm.Visible = false;
				
				btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSAuditMgr.DeleteAudit(EditAuditId);
				// delete the task
				if (delStatus == 1)
				{
					EHSAuditMgr.DeleteAuditTask(EditAuditId, 50);
				}
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Assessment deleted." : "Error deleting assessment.";
				lblResults.Text += "</div>";
			}

			rddlAuditType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
            AUDIT theAudit = null;
			decimal auditId = 0;
			string result = "<h3>EHS Assessment " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			//if (Mode == AuditMode.Prevent)
			//	result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			if (!IsEditContext)
			{
				auditTypeId = SelectedTypeId;
				auditType = rddlAuditType.SelectedText;
			}
			else
			{
				auditTypeId = EditAuditTypeId;
				auditType = EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
			}

			//questions = EHSAuditMgr.SelectAuditQuestionList(auditTypeId, 0, EditAuditId);  // do we really need to get these again??

			UpdateAnswersFromForm();

			if (!IsEditContext)
			{
				// Add context
				theAudit = CreateNewAudit();
				auditId = theAudit.AUDIT_ID;
			}
			else
			{
				// Edit context
				auditId = EditAuditId;
				if (auditId > 0)
				{
					theAudit = UpdateAudit(auditId);
				}
			}
			if (auditId > 0)
			{
				shouldReturn = AddOrUpdateAnswers(questions, auditId);
				SaveAttachments(auditId);
				if (!IsEditContext)
				{
					// notify the user of a new audit
					EHSNotificationMgr.NotifyOnAuditCreate(auditId, (decimal)theAudit.AUDIT_PERSON);
				}
			}


			if (shouldReturn == true)
			{
				divForm.Visible = false;
				//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = false;

				pnlAddEdit.Visible = false;
				btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;

				RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
				if (rcbWarnNavigate != null)
					rcbWarnNavigate.Visible = false;

				lblResults.Visible = true;
			}
			else
			{
				EditAuditId = auditId;
				SessionManager.ReturnRecordID = auditId;
				CurrentStep = 0;
				IsEditContext = true;
				Visible = true;
				// need to redraw the page with text boxes highlighted
				BuildForm();
				// and send a message that the field need to be entered
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsIndicatedMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}

			if (shouldReturn)
			{
				SessionManager.ReturnStatus = false;
				SessionManager.ReturnObject = "DisplayAudits";
				Response.Redirect("/EHS/EHS_Audits.aspx");  // mt - temporary
			}

		}

		protected void lnkAddTask_Click(Object sender, EventArgs e)
		{
			if (OnExceptionListItemClick != null)
			{
				RadButton lnk = (RadButton)sender;
				string[] cmd = lnk.CommandArgument.Split(',');

				SessionManager.ReturnRecordID = Convert.ToDecimal(cmd[0].ToString());
				SessionManager.ReturnObject = "CallAddTask";
				SessionManager.ReturnStatus = true;
				EditReturnState = "NeedsRefresh";

				//AddTask(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
				// call sending AuditID, QuestionID
				OnExceptionListItemClick(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			}
		}

		//protected void AddTask(decimal auditID, decimal questionID)
		//{
		//	int recordType = (int)TaskRecordType.Audit;
		//	AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), auditID);
		//	EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(auditID, questionID, audit.AUDIT_TYPE_ID);
		//	uclTaskList.TaskWindow(50, auditQuestion.AuditId, auditQuestion.QuestionId, "350", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID);

		//	//uclTask.BindTaskAdd(recordType, auditQuestion.AuditId, auditQuestion.QuestionId, "350", "T", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID, "");
		//	//string script = "function f(){OpenUpdateTaskWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
		//	//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		//}

		protected void lnkFileUpload_Click(Object sender, EventArgs e)
		{
			if (OnAttachmentListItemClick != null)
			{
				RadButton lnk = (RadButton)sender;
				string[] cmd = lnk.CommandArgument.Split(',');
				SessionManager.ReturnRecordID = Convert.ToDecimal(cmd[0].ToString());
				SessionManager.ReturnObject = "CallAddFile";
				SessionManager.ReturnStatus = true;
				EditReturnState = "NeedsRefresh";
				//OpenFileUpload(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
				OnAttachmentListItemClick(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			}
		}

		//protected void OpenFileUpload(decimal auditID, decimal questionID)
		//{
		//	int recordType = (int)TaskRecordType.Audit;
		//	//AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), auditID);
		//	//EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(auditID, questionID, audit.AUDIT_TYPE_ID);

		//	uclAttachWin.OpenManageAttachmentsWindow(recordType, auditID, questionID.ToString(), "Upload Attachments", "Upload or view attachments for this assessment question");
		//}

#endregion


		#region Save Methods

		protected void UpdateAnswersFromForm()
		{
			// Save to dates & times to database using US culture info
			//DateTimeFormatInfo fromDtfi = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
			//DateTimeFormatInfo toDtfi = new CultureInfo("en-US", false).DateTimeFormat;

			foreach (var q in questions)
			{
				var control = pnlForm.FindControl(q.QuestionId.ToString());
				string answer = "";
				if (control != null)
				{
					switch (q.QuestionType)
					{
						case EHSAuditQuestionType.TextField:
							answer = (control as RadTextBox).Text;
							break;

						case EHSAuditQuestionType.TextBox:
							answer = (control as RadTextBox).Text;
							break;

						case EHSAuditQuestionType.RichTextBox:
							answer = (control as RadEditor).Content;
							break;

						case EHSAuditQuestionType.Radio:
						case EHSAuditQuestionType.RadioCommentLeft:
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSAuditQuestionType.RadioPercentage:
						case EHSAuditQuestionType.RadioPercentageCommentLeft:
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSAuditQuestionType.CheckBox:
							foreach (var item in (control as CheckBoxList).Items)
								if ((item as ListItem).Selected)
									answer += (item as ListItem).Value + "|";
							break;

						case EHSAuditQuestionType.Dropdown:
							answer = (control as RadDropDownList).SelectedValue;
							break;

						case EHSAuditQuestionType.Date:
							DateTime? fromDate = (control as RadDatePicker).SelectedDate;
							if (fromDate != null)
							{
								answer = ((DateTime)fromDate).ToString(CultureInfo.GetCultureInfo("en-US"));
							}
							break;

						case EHSAuditQuestionType.Time:
							TimeSpan? fromTime = (control as RadTimePicker).SelectedTime;
							if (fromTime != null)
								answer = ((TimeSpan)fromTime).ToString("c", CultureInfo.GetCultureInfo("en-US"));
							break;

						case EHSAuditQuestionType.DateTime:
							DateTime? fromDateTime = (control as RadDateTimePicker).SelectedDate;
							if (fromDateTime != null)
								answer = ((DateTime)fromDateTime).ToString(CultureInfo.GetCultureInfo("en-US"));
							break;

						case EHSAuditQuestionType.BooleanCheckBox:
							answer = (control as CheckBox).Checked ? "Yes" : "No";
							break;

						case EHSAuditQuestionType.Attachment:
							uploader = (control as Ucl_RadAsyncUpload);
							//foreach (var file1 in uploader.RAUpload.fil
							foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
								answer += file.FileName + "|";
							break;

						case EHSAuditQuestionType.CurrencyTextBox:
							answer = (control as RadNumericTextBox).Text;
							break;

						case EHSAuditQuestionType.PercentTextBox:
							answer = (control as RadNumericTextBox).Text;
							break;

						case EHSAuditQuestionType.StandardsReferencesDropdown:
							answer = (control as RadComboBox).SelectedValue;
							break;

						case EHSAuditQuestionType.LocationDropdown:
							answer = (control as RadDropDownList).SelectedValue;
							if (!string.IsNullOrEmpty(answer))
								selectedPlantId = Convert.ToDecimal(answer);
							break;

						case EHSAuditQuestionType.UsersDropdown:
							answer = (control as RadDropDownList).SelectedValue;
							break;

						case EHSAuditQuestionType.RequiredYesNoRadio:
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSAuditQuestionType.PageOneAttachment:
							uploader = (control as Ucl_RadAsyncUpload);
							foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
								answer += file.FileName + "|";
							break;

						case EHSAuditQuestionType.UsersDropdownLocationFiltered:
							answer = (control as RadDropDownList).SelectedValue;
							break;
					}
				}
				q.AnswerText = answer;

				var commentControl = new RadTextBox();
				string comment = "";
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioCommentLeft ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					try
					{
						commentControl = (RadTextBox)pnlForm.FindControl("Comment" + q.QuestionId.ToString());
						if (commentControl != null)
						{
							comment = commentControl.Text;
						}
					}
					catch
					{ }
				}
				q.AnswerComment = comment;

			}

			if (auditDate == null || auditDate < DateTime.Now.AddYears(-100))
			{
				try
				{
					AUDIT audit = EHSAuditMgr.SelectAuditById(entities, questions[0].AuditId);
					PLANT plant = SQMModelMgr.LookupPlant((decimal)audit.DETECT_PLANT_ID);
					DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, plant.LOCAL_TIMEZONE);
					auditDate = localTime;
				}
				catch
				{
					auditDate = SessionManager.UserContext.LocalTime;
				}
			}

			if (auditDescription.Length > MaxTextLength)
				auditDescription = auditDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = selectedPlantId;
		}

		protected void CalculatePercentages(List<EHSAuditQuestion> questions)
		{
			string previousTopic = "";
			decimal totalQuestions = 0;
			decimal totalTopicQuestions = 0;
			decimal totalPositive = 0;
			decimal totalTopicPositive = 0;
			decimal totalPercent = 0;
			decimal totalWeightScore = 0;
			decimal totalPossibleScore = 0;
			decimal totalTopicWeightScore = 0;
			decimal totalTopicPossibleScore = 0;
			decimal possibleScore = 0;
			bool answerIsPositive = false;

			foreach (var q in questions)
			{

				string tid = q.TopicId.ToString();

				string answer = "";
				// first set topic percent display if the topic has changed
				if (!previousTopic.Equals(tid))
				{
					if (!previousTopic.Equals(""))
					{
						Label topicTotal = (Label)pnlForm.FindControl("LabelP" + previousTopic);
						if (topicTotal != null)
						{
							// AW201602 - Percents will be % of possible score, not # questions answered.  If weight is 0/1, then this should not change.
							//if (totalTopicQuestions > 0)
							//	totalPercent = totalTopicPositive / totalTopicQuestions;
							//else
							//	totalPercent = 0;
							if (totalTopicPossibleScore > 0)
								totalPercent = totalTopicWeightScore / totalTopicPossibleScore;
							else
								totalPercent = 0;
							topicTotal.Text = string.Format("{0:0%}", totalPercent);
						}
						totalTopicQuestions = 0;
						totalTopicPositive = 0;
						totalTopicWeightScore = 0;
						totalTopicPossibleScore = 0;
					}
					previousTopic = tid;
				}

				// next, add to totals if the response is a positive one
				if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					totalQuestions += 1;
					totalTopicQuestions += 1;
					answer = q.AnswerText;
					answerIsPositive = false;
					possibleScore = 0;
					foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
					{
						if (choice.ChoiceWeight > possibleScore)
							possibleScore = choice.ChoiceWeight;
						if (choice.Value.Equals(q.AnswerText))
						{
							if (choice.ChoicePositive)
								answerIsPositive = true;
							totalWeightScore += choice.ChoiceWeight;
							totalTopicWeightScore += choice.ChoiceWeight;
						}
					}
					totalPossibleScore += possibleScore;
					totalTopicPossibleScore += possibleScore;
					if (answerIsPositive)
					{
						totalPositive += 1;
						totalTopicPositive += 1;
					}
				}
			}
			// update the last topic total
			Label topicLastTotal = (Label)pnlForm.FindControl("LabelP" + previousTopic);
			if (topicLastTotal != null)
			{
				//if (totalTopicQuestions > 0)
				//	totalPercent = totalTopicPositive / totalTopicQuestions;
				//else
				//	totalPercent = 0;
				if (totalTopicPossibleScore > 0)
					totalPercent = totalTopicWeightScore / totalTopicPossibleScore;
				else
					totalPercent = 0;
				topicLastTotal.Text = string.Format("{0:0%}", totalPercent);
			}
			// update the audit total
			//topicLastTotal = (Label)pnlForm.FindControl("LabelTotalPercent");
			//if (topicLastTotal != null)
			//{
			//	if (totalQuestions > 0)
			//		totalPercent = totalPositive / totalQuestions;
			//	else
			//		totalPercent = 0;
			//	topicLastTotal.Text = string.Format("Total Positive Score:   {0:0%}", totalPercent);
			//}
			// update point totals
			topicLastTotal = (Label)pnlForm.FindControl("LabelTotalPossiblePoints");
			if (topicLastTotal != null)
			{
				topicLastTotal.Text = string.Format("Total Possible Points:   {0:0}", totalPossibleScore);
			}
			topicLastTotal = (Label)pnlForm.FindControl("LabelTotalPointsAchieved");
			if (topicLastTotal != null)
			{
				topicLastTotal.Text = string.Format("Total Points Achieved:   {0:0}", totalWeightScore);
			}
			topicLastTotal = (Label)pnlForm.FindControl("LabelTotalPointsPercentage");
			if (topicLastTotal != null)
			{
				if (totalPossibleScore > 0)
					totalPercent = totalWeightScore / totalPossibleScore;
				else
					totalPercent = 0;
				topicLastTotal.Text = string.Format("Percentage of Points Achieved:   {0:0%}", totalPercent);
			}
		}

		protected bool AddOrUpdateAnswers(List<EHSAuditQuestion> questions, decimal auditId)
		{
			bool negativeTextComplete = true;
			decimal totalQuestions = 0;
			decimal totalAnswered = 0;
			decimal totalPositive = 0;
			decimal totalPercent = 0;
			decimal totalWeightScore = 0;
			decimal totalPossibleScore = 0;
			decimal possibleScore = 0;
			bool answerIsPositive = false;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var auditAnswer = (from ia in entities.AUDIT_ANSWER
									  where ia.AUDIT_ID == auditId
										  && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();
				// calculate the total percentages
				totalQuestions += 1;
				if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					answerIsPositive = false;
					//if (!q.AnswerText.ToString().Equals(""))
					//{
					possibleScore = 0;
					foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
					{
						if (choice.ChoiceWeight > possibleScore)
							possibleScore = choice.ChoiceWeight;
						if (choice.Value.Equals(q.AnswerText) && choice.ChoicePositive)
							answerIsPositive = true;
						if (choice.Value.Equals(q.AnswerText))
						{
							totalWeightScore += choice.ChoiceWeight;
							totalAnswered += 1;
						}
					}
					totalPossibleScore += possibleScore;
					if (answerIsPositive)
					{
						q.ChoicePositive = true;
						totalPositive += 1;
					}
					else
					{
						if (!q.AnswerText.ToString().Equals(""))
						{
							q.ChoicePositive = false;
							if (q.AnswerComment.ToString().Trim().Length == 0)
								negativeTextComplete = false;
						}
					}
					//}
				}
				else if (!q.AnswerText.ToString().Equals(""))
				{
					totalAnswered += 1;
				}

				if (q.CommentRequired && q.AnswerComment.ToString().Trim().Length == 0)
					negativeTextComplete = false;
				if (auditAnswer != null)
				{
					auditAnswer.ANSWER_VALUE = q.AnswerText;
					//auditAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText; // don't want to update text after the audit has been created
					auditAnswer.COMMENT = q.AnswerComment;
					auditAnswer.CHOICE_POSITIVE = q.ChoicePositive;
					if (q.ChoicePositive)
						auditAnswer.STATUS = "03";
					else
						auditAnswer.STATUS = "01";
				}
				else
				{
					auditAnswer = new AUDIT_ANSWER()
					{
						AUDIT_ID = auditId,
						AUDIT_QUESTION_ID = q.QuestionId,
						ANSWER_VALUE = q.AnswerText,
						ORIGINAL_QUESTION_TEXT = q.QuestionText,
						COMMENT = q.AnswerComment,
						CHOICE_POSITIVE = q.ChoicePositive
					};

					if (q.ChoicePositive)
						auditAnswer.STATUS = "03";
					else
						auditAnswer.STATUS = "01";

					entities.AddToAUDIT_ANSWER(auditAnswer);
				}
			}
			// now update the header info
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			if (totalQuestions > 0)
				totalPercent = Math.Round((totalAnswered / totalQuestions), 2) * 100;
			else
				totalPercent = 0;
			audit.PERCENT_COMPLETE = totalPercent;
			if (totalPercent >= 100 && negativeTextComplete)
			{
				audit.CURRENT_STATUS = "C";
				if (!audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
				{
					PLANT plant = SQMModelMgr.LookupPlant((decimal)audit.DETECT_PLANT_ID);
					DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, plant.LOCAL_TIMEZONE);
					audit.CLOSE_DATE_DATA_COMPLETE = localTime;
					audit.CLOSE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				}
			}
			else if (totalPercent > 0)
			{
				audit.CURRENT_STATUS = "I";
			}
			else
				audit.CURRENT_STATUS = "A";

			//if (totalQuestions > 0)
			//	totalPercent = Math.Round((totalPositive / totalQuestions), 2) * 100;
			//else
			//	totalPercent = 0;
			if (totalPossibleScore > 0)
				totalPercent = Math.Round((totalWeightScore / totalPossibleScore), 2) * 100;
			else
				totalPercent = 0;
			audit.TOTAL_SCORE = totalPercent;

			if (rddlDepartment.SelectedIndex > 0)
				audit.DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue);

			// save all the changes
			entities.SaveChanges();

			if (audit.CURRENT_STATUS.Equals("C"))
			{
				AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
				EHSAuditMgr.CreateOrUpdateTask(auditId, SessionManager.UserContext.Person.PERSON_ID, 50, audit.AUDIT_DT.AddDays(audittype.DAYS_TO_COMPLETE), "C", SessionManager.UserContext.Person.PERSON_ID);
			}
			return negativeTextComplete;
		}

		protected AUDIT CreateNewAudit()
		{
			decimal auditId = 0;
			PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, auditPlant.LOCAL_TIMEZONE);
			var newAudit = new AUDIT()
			{
				DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
				DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
				DETECT_PLANT_ID = auditPlant.PLANT_ID,
				AUDIT_TYPE = "EHS",
				CREATE_DT = localTime,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = tbDescription.Text.ToString(),
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				AUDIT_DT = (DateTime)dmAuditDate.SelectedDate,
				AUDIT_TYPE_ID = auditTypeId,
				AUDIT_PERSON = Convert.ToDecimal(rddlAuditUsers.SelectedValue),
				DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue)
			};
			entities.AddToAUDIT(newAudit);
			entities.SaveChanges();
			auditId = newAudit.AUDIT_ID;

			// create task record for their calendar
			AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
			EHSAuditMgr.CreateOrUpdateTask(auditId, Convert.ToDecimal(rddlAuditUsers.SelectedValue), 50, ((DateTime)dmAuditDate.SelectedDate).AddDays(audittype.DAYS_TO_COMPLETE), "A", SessionManager.UserContext.Person.PERSON_ID);

			return newAudit;
		}

		protected AUDIT UpdateAudit(decimal auditId)
		{
			// we are not going to let them update any of this info.
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();

			//if (audit != null && rddlDepartment.SelectedIndex > 0)
			//{
			//	audit.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//	audit.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
			//	audit.DETECT_PLANT_ID = selectedPlantId;
			//	audit.AUDIT_TYPE = "EHS";
			//	//audit.CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
			//	audit.DESCRIPTION = auditDescription;
			//	//audit.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
			//	audit.AUDIT_DT = auditDate;
			//	//audit.ISSUE_TYPE = auditType;
			//	audit.AUDIT_TYPE_ID = auditTypeId;

			//	audit.DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue);

			//	entities.SaveChanges();
			//}

            return audit;

		}

		protected void SaveAttachments(decimal auditId)
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
				SessionManager.DocumentContext.RecordID = auditId;
				SessionManager.DocumentContext.RecordStep = recordStep;
				uploader.SaveFiles();
			}
		}

		protected void UpdateTaskInfo(List<EHSAuditQuestion> questions, decimal auditId, DateTime createDate)
		{
			// AW20150820 this still needs to be worked out
			DateTime dueDate = DateTime.MinValue;
			decimal responsiblePersonId = 0;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var auditAnswer = (from ia in entities.AUDIT_ANSWER
									  where ia.AUDIT_ID == auditId
										  && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();

				//if (auditAnswer != null && !String.IsNullOrEmpty(auditAnswer.ANSWER_VALUE))
				//{
				//	if (Mode == AuditMode.Audit)
				//	{
				//		if (q.QuestionId == (decimal)EHSAuditQuestionId.DateDue)
				//			dueDate = DateTime.Parse(auditAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US"));
				//		else if (q.QuestionId == (decimal)EHSAuditQuestionId.ResponsiblePersonDropdown) 
				//			responsiblePersonId = Convert.ToDecimal(auditAnswer.ANSWER_VALUE);
				//	}
				//	else if (Mode == AuditMode.Prevent)
				//	{
				//		if (q.QuestionId == (decimal)EHSAuditQuestionId.ReportDate)
				//			dueDate = createDate.AddDays(30);  // mt - per TI the due date should be based on the audit CREATE date instead of the inspection date
				//			// dueDate = DateTime.Parse(auditAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US")).AddDays(30);
				//		else if (q.QuestionId == (decimal)EHSAuditQuestionId.AssignToPerson) 
				//			responsiblePersonId = Convert.ToDecimal(auditAnswer.ANSWER_VALUE);
				//	}
				//}
			}
			
			//if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			//{
			//	int recordTypeId = (Mode == AuditMode.Prevent) ? 45 : 40;
			//	EHSAuditMgr.CreateOrUpdateTask(auditId, responsiblePersonId, recordTypeId, dueDate);
			//}
		}

		

		protected void GoToNextStep(decimal auditId)
		{
			// Go to next step (report)
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = auditId;
		}

		protected void ShowAuditDetails(decimal auditId, string result)
		{
			rddlAuditType.SelectedIndex = 0;

			// Display audit details control
			btnDelete.Visible = false;
			lblResults.Text = result.ToString();
			uclAuditDetails.Visible = true;
			var displaySteps = new int[] { CurrentStep };
			uclAuditDetails.Refresh(auditId, displaySteps);
		}

		#endregion


		public void ClearControls()
		{
			pnlForm.Controls.Clear();
		}

		protected void RefreshPageContext()
		{
			uclAuditDetails.Clear();
			string typeString = "";

			if (UserContext.GetMaxScopePrivilege(SysScope.audit) < SysPriv.view && CurrentStep == 0)
			{
				pnlAddEdit.Visible = true;
				if (!IsEditContext)
				{
					// Add
					//btnSaveReturn.Enabled = false;
					//btnSaveReturn.Visible = (SelectedTypeId > 0);
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;
					//btnSaveContinue.Visible = (SelectedTypeId > 0);
					rddlAuditType.Visible = (rddlAuditType.Items.Count == 1) ? false : true;
					lblAddOrEditAudit.Text = "<strong>Add a New Assessment:</strong>";

					lblAuditType.Visible = false;
					btnDelete.Visible = false;
				}
				else
				{
					// Edit
					typeString = " Assessment";
					//btnSaveContinue.Visible = true;
					btnSaveReturn.CommandArgument = "0";
					SelectedTypeId = 0;
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;

					lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

					rddlAuditType.Visible = false;
					//if (Mode == AuditMode.Audit)
					lblAuditType.Text = Resources.LocalizedText.AssessmentType + ": ";
					//else if (Mode == AuditMode.Prevent)
					//	lblAuditType.Text = "Type: ";

					lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
					lblAuditType.Visible = true;
					bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
					btnDelete.Visible = createAuditAccess;
					LoadHeaderInformation();
					BuildForm();
				}

				UpdateControlledQuestions();
				UpdateButtonText();
			}
			else
			{
				// View only
				typeString = " Assessment";
				SelectedTypeId = 0;
				btnSaveReturn.Enabled = false;
				btnSaveReturn.Visible = false;

				//lblAddOrEditAudit.Text = "<strong>" + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + " Closed</strong><br/>";
				// should we look up the closed date to determine if the label should include "Closed"?
				lblAddOrEditAudit.Text = "<strong>" + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

				rddlAuditType.Visible = false;
				lblAuditType.Text = Resources.LocalizedText.AssessmentType + ": ";

				lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
				lblAuditType.Visible = true;
				bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
				btnDelete.Visible = createAuditAccess;
				LoadHeaderInformation();
				var displaySteps = new int[] { CurrentStep };
				uclAuditDetails.Refresh(EditAuditId, displaySteps);
				pnlAddEdit.Visible = false;
			}

		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			if (UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.system))
			{
				plantIdList = EHSAuditMgr.SelectPlantIdsByCompanyId(companyId);
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