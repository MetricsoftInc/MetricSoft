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
	public partial class Ucl_EHSAuditForm : System.Web.UI.UserControl
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

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected decimal auditPlantId = 0;

		protected AccessMode accessLevel;
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


		
		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//accessLevel = UserContext.CheckAccess("EHS", "312");
			//accessLevel = UserContext.CheckAccess("EHS", "");
			accessLevel = AccessMode.Admin;  // mt - temporary
			entities = new PSsqmEntities();
			controlQuestionChanged = false;

			//if (Mode == AuditMode.Audit)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx";
			//else if (Mode == AuditMode.Prevent)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx?mode=prevent";

			UpdateAuditTypes();

			bool returnFromClick = false;
			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
				returnFromClick = true;
			}

			if (IsPostBack)
			{
					divAuditForm.Visible = true;
					//if (!returnFromClick)
					//{
						LoadHeaderInformation();
					//}
					BuildForm();
			}
			else
			{
				RefreshPageContext();
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
					selectString = "[Select An Audit Type]";
				}
				if (auditTypeList.Count > 1)
					auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });
				
				if (accessLevel < AccessMode.Admin)
						auditTypeList = (from i in auditTypeList where i.AUDIT_TYPE_ID != 10 select i).ToList();
				
				rddlAuditType.DataSource = auditTypeList;
				rddlAuditType.DataTextField = "TITLE";
				rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
				rddlAuditType.DataBind();
			}
		}

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info
			AccessMode accessmode = UserContext.RoleAccess();

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
			}
			else
			{
				if (accessmode >= AccessMode.Plant)
				{
					// List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
					List<BusinessLocation> locationList = SessionManager.PlantList;
					locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");
					locationList = UserContext.FilterPlantAccessList(locationList, "SQM", "");
					if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone") == false)
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
							ddlAuditLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
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
					dmAuditDate.SelectedDate = DateTime.Now;

			}
		}


		public void BuildForm()
		{
			if (accessLevel <= AccessMode.View)
				return;

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

			pnlForm.Controls.Clear();
			pnlForm.Visible = true;
			lblResults.Visible = false;

			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);

			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"5\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));
			string previousTopic = "";
			string qid = "";
			string tid = "";
			string ptid = "";

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
						pnlPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
						pnlPercent.Controls.Add(new Label() { ID = "Label" + ptid, Text = "0%" }); // we will populate the values later
						pnlPercent.Controls.Add(new LiteralControl("</td></tr>"));
						pnlForm.Controls.Add(pnlPercent);
					}
					var pnlTopic = new Panel() { ID = "Panel" + tid };
					pnlTopic.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"blueCell\" style=\"width: 100%; font-weight: bold;\">"));
					pnlTopic.Controls.Add(new Label() { ID = "Label" + tid, Text = q.TopicTitle });
					pnlTopic.Controls.Add(new LiteralControl("</td></tr>"));
					pnlForm.Controls.Add(pnlTopic);
					previousTopic = q.TopicId.ToString();
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
							validator.InitialValue = "[Select One]";

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
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rbl.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Value);
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
						var rblp = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged auditanswer" };
						rblp.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Value);
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
							var li = new ListItem(choice.Value);
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
						rddl.Items.Add(new DropDownListItem("[Select One]", ""));

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
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.Value, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.Value, choice.Value));
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
						var bcb = new CheckBox() { ID = qid, Text = "Yes", CssClass = "WarnIfChanged" };

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
							rddlLocation.Items.Add(new DropDownListItem("[Select One]", ""));
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
						rddl3.Items.Add(new DropDownListItem("[Select One]", ""));

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
						var choices = new string[] { "Yes", "No" };
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

				pnl.Controls.Add(new LiteralControl("</td><td class=\"greyCell\">"));

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
					string cid = "Comment" + qid;
					var comment = new RadTextBox() { ID = cid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
					comment.TextMode = InputMode.MultiLine;
					comment.Rows = 2;
					comment.Resize = ResizeMode.Both;
					comment.Text = q.AnswerComment;
					pnl.Controls.Add(comment);
				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				pnlForm.Controls.Add(pnl);
			}

			// add the final topic percent line and then an audit total percent line
			if (!previousTopic.Equals(""))
			{
				// need to add a display for the topic percentage
				Panel pnlPercent = new Panel() { ID = "PanelP" + previousTopic };
				pnlPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
				pnlPercent.Controls.Add(new Label() { ID = "LabelP" + previousTopic, Text = "0%" }); // we will populate the values later
				pnlPercent.Controls.Add(new LiteralControl("</td></tr>"));
				pnlForm.Controls.Add(pnlPercent);
			}
			Panel pnlTotalPercent = new Panel() { ID = "PanelTotalPercent" };
			pnlTotalPercent.Controls.Add(new LiteralControl("<tr><td colspan=\"5\" class=\"greyCell\" style=\"width: 100%; text-align: right; font-weight: bold;\">"));
			pnlTotalPercent.Controls.Add(new Label() { ID = "LabelTotalPercent", Text = "Total Score:  0%" }); // we will populate the values later
			pnlTotalPercent.Controls.Add(new LiteralControl("</td></tr>"));
			pnlForm.Controls.Add(pnlTotalPercent);

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
			else if (sender is RadSlider)
			{
				location = ddlAuditLocation.SelectedValue;
			}
			BuildAuditUsersDropdownList(location);
			hdnAuditLocation.Value = location;

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
			if (accessLevel <= AccessMode.View)
				return;

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
							validator.InitialValue = "[Select One]";

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
						var rbl = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Value);
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
						var rblp = new RadioButtonList() { ID = qid, CssClass = "WarnIfChanged" };
						rblp.RepeatDirection = RepeatDirection.Horizontal;
						foreach (var choice in q.AnswerChoices)
						{
							var li = new ListItem(choice.Value);
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
							var li = new ListItem(choice.Value);
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
						rddl.Items.Add(new DropDownListItem("[Select One]", ""));

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
										rddl.Items.Add(new DropDownListItem(" ∙ " + choice.Value, choice.Value));
								}
								else
								{
									rddl.Items.Add(new DropDownListItem(choice.Value, choice.Value));
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
						var bcb = new CheckBox() { ID = qid, Text = "Yes", CssClass = "WarnIfChanged" };

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
							rddlLocation.Items.Add(new DropDownListItem("[Select One]", ""));
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

					case EHSAuditQuestionType.UsersDropdown:
						var rddl3 = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						rddl3.Items.Add(new DropDownListItem("[Select One]", ""));

						var personList = new List<PERSON>();
						if (CurrentStep == 1)
							personList = EHSIncidentMgr.SelectIncidentPersonList(EditAuditId);
						else if (CurrentStep == 0)
							personList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

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
						var choices = new string[] { "Yes", "No" };
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

				pnl.Controls.Add(new LiteralControl("</td> class=\"greyCell\">"));

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio ||
					q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
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

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && accessLevel < AccessMode.Admin)
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && accessLevel < AccessMode.Admin)
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
			btnSaveReturn.Text = "Save & Return";

			if (IsEditContext)
				btnSaveContinue.Text = "Save Audit";
			else
				btnSaveContinue.Text = "Create Audit";

			if (IsEditContext)
				btnSaveReturn.Text = "Save Audit";
			else
				btnSaveReturn.Text = "Create Audit";
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
				rddlFilteredUsers.Items.Add(new DropDownListItem("[Select One]", ""));

				var locationPersonList = new List<PERSON>();
				if (this.SelectedLocationId > 0)
				{
					locationPersonList = EHSAuditMgr.SelectEhsDataOriginatorsAtPlant(this.SelectedLocationId);
					locationPersonList.AddRange(EHSAuditMgr.SelectDataOriginatorAdditionalPlantAccess(this.SelectedLocationId));
					locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
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
				rddlAuditUsers.Items.Add(new DropDownListItem("[Select One]", ""));
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
							cdFormControl.SelectedDate = DateTime.Now;
							cdFormControl.Enabled = false;
						}
					}
					else
					{
						closeCh.Checked = false;
						string script = string.Format("alert('{0}');", "You must complete all required fields on this page to close the audit.");
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
										(formControl as RadioButtonList).SelectedValue = "No";
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
										rbl.SelectedValue = "Yes";
										affectedQuestion.AnswerText = "Yes";

										// Recursively process any other controls triggered by a forced checkbox
										if (affectedQuestion.QuestionControls != null && affectedQuestion.QuestionControls.Count > 0)
											ProcessQuestionControls(affectedQuestion, depth);
									}
									else
									{
										// Only force to false if the result of a parent checkbox changing (avoids changes on form rebuilds)
										if (controlQuestionChanged)
											rbl.SelectedValue = "No";
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
			rcb.Items.Add(new RadComboBoxItem("[Select One]"));
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
				Save(true);
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
			if (EditAuditId > 0)
			{
				pnlForm.Visible = false;
				
				btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSAuditMgr.DeleteAudit(EditAuditId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Audit deleted." : "Error deleting audit.";
				lblResults.Text += "</div>";
			}

			rddlAuditType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
            AUDIT theAudit = null;
			decimal auditId = 0;
			string result = "<h3>EHS Audit " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			if (Mode == AuditMode.Prevent)
				result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

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
				AddOrUpdateAnswers(questions, auditId);
				SaveAttachments(auditId);
			}

			if (shouldReturn)
			{
				SessionManager.ReturnStatus = false;
				SessionManager.ReturnObject = "DisplayAudits";
				Response.Redirect("/EHS/EHS_Audits.aspx");  // mt - temporary
			}

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
							answer = (control as RadioButtonList).SelectedValue;
							break;

						case EHSAuditQuestionType.RadioPercentage:
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
					q.QuestionType == EHSAuditQuestionType.RadioPercentage)
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
				auditDate = DateTime.Now;

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
							totalPercent = totalTopicPositive / totalTopicQuestions;
							topicTotal.Text = string.Format("{0:0%}", totalPercent);
						}
						totalTopicQuestions = 0;
						totalTopicPositive = 0;
					}
					previousTopic = tid;
				}

				// next, add to totals if the response is a positive one
				if (q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
					totalQuestions += 1;
					totalTopicQuestions += 1;
					answer = q.AnswerText;
					answerIsPositive = false;
					foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
					{
						if (choice.Value.Equals(q.AnswerText) && choice.ChoicePositive)
							answerIsPositive = true;
					}
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
				totalPercent = totalTopicPositive / totalTopicQuestions;
				topicLastTotal.Text = string.Format("{0:0%}", totalPercent);
			}
			// update the audit total
			topicLastTotal = (Label)pnlForm.FindControl("LabelTotalPercent");
			if (topicLastTotal != null)
			{
				totalPercent = totalPositive / totalQuestions;
				topicLastTotal.Text = string.Format("Total Score:   {0:0%}", totalPercent);
			}
		}

		protected bool AddOrUpdateAnswers(List<EHSAuditQuestion> questions, decimal auditId)
		{
			bool shouldCreate8d = false;
			decimal totalQuestions = 0;
			decimal totalAnswered = 0;
			decimal totalPositive = 0;
			decimal totalPercent = 0;
			bool answerIsPositive = false;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var auditAnswer = (from ia in entities.AUDIT_ANSWER
									  where ia.AUDIT_ID == auditId
										  && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();
				if (auditAnswer != null)
				{
					auditAnswer.ANSWER_VALUE = q.AnswerText;
					//auditAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText; // don't want to update text after the audit has been created
					auditAnswer.COMMENT = q.AnswerComment;
				}
				else
				{
					auditAnswer = new AUDIT_ANSWER()
					{
						AUDIT_ID = auditId,
						AUDIT_QUESTION_ID = q.QuestionId,
						ANSWER_VALUE = q.AnswerText,
						ORIGINAL_QUESTION_TEXT = q.QuestionText,
						COMMENT = q.AnswerComment
					};
					entities.AddToAUDIT_ANSWER(auditAnswer);
				}
				// calculate the total percentages
				if (q.QuestionType == EHSAuditQuestionType.RadioPercentage)
				{
					totalQuestions += 1;
					answerIsPositive = false;
					if (!q.AnswerText.ToString().Equals(""))
					{
						totalAnswered += 1;
						foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
						{
							if (choice.Value.Equals(q.AnswerText) && choice.ChoicePositive)
								answerIsPositive = true;
						}
						if (answerIsPositive)
						{
							totalPositive += 1;
						}
					}
				}
			}
			// now update the header info
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			totalPercent = Math.Round((totalAnswered / totalQuestions),2) * 100;
			audit.PERCENT_COMPLETE = totalPercent;
			if (totalPercent >= 100)
			{
				audit.CURRENT_STATUS = "C";
				if (!audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
				{
					audit.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
					audit.CLOSE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				}
			}
			else if (totalPercent > 0)
			{
				audit.CURRENT_STATUS = "I";
			}
			else
				audit.CURRENT_STATUS = "A";

			totalPercent = Math.Round((totalPositive / totalQuestions),2) * 100;
			audit.TOTAL_SCORE = totalPercent;

			// save all the changes
			entities.SaveChanges();

			return shouldCreate8d;
		}

		protected AUDIT CreateNewAudit()
		{
			decimal auditId = 0;
			PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			var newAudit = new AUDIT()
			{
				DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
				DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
				DETECT_PLANT_ID = auditPlant.PLANT_ID,
				AUDIT_TYPE = "EHS",
				CREATE_DT = DateTime.Now,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = tbDescription.Text.ToString(),
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				AUDIT_DT = (DateTime)dmAuditDate.SelectedDate,
				AUDIT_TYPE_ID = auditTypeId,
				AUDIT_PERSON = Convert.ToDecimal(rddlAuditUsers.SelectedValue)
			};
			entities.AddToAUDIT(newAudit);
			entities.SaveChanges();
			auditId = newAudit.AUDIT_ID;
			
			return newAudit;
		}

		protected AUDIT UpdateAudit(decimal auditId)
		{
			// we are not going to let them update any of this info.
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			//if (audit != null)
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

			if (accessLevel > AccessMode.View && CurrentStep == 0)
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
					lblAddOrEditAudit.Text = "<strong>Add a New Audit:</strong>";

					lblAuditType.Visible = false;
					btnDelete.Visible = false;
				}
				else
				{
					// Edit
					typeString = " Audit";
					//btnSaveContinue.Visible = true;
					btnSaveReturn.CommandArgument = "0";
					SelectedTypeId = 0;
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;

					lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

					rddlAuditType.Visible = false;
					//if (Mode == AuditMode.Audit)
					lblAuditType.Text = "Audit Type: ";
					//else if (Mode == AuditMode.Prevent)
					//	lblAuditType.Text = "Type: ";

					lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
					lblAuditType.Visible = true;
					btnDelete.Visible = true;
					LoadHeaderInformation();
					BuildForm();
				}

				UpdateControlledQuestions();
				UpdateButtonText();

				
				// Only plant admin and higher can view closed audits
				//if (accessLevel < AccessMode.Plant)
				//pnlShowClosed.Visible = false;

				// Only admin and higher can delete audits
				if (accessLevel < AccessMode.Admin)
					btnDelete.Visible = false;
			}
			else
			{
				// View only
				typeString = " Audit";
				SelectedTypeId = 0;
				btnSaveReturn.Enabled = false;
				btnSaveReturn.Visible = false;

				lblAddOrEditAudit.Text = "<strong>" + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + " Closed</strong><br/>";

				rddlAuditType.Visible = false;
				lblAuditType.Text = "Audit Type: ";

				lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
				lblAuditType.Visible = true;
				btnDelete.Visible = false;
				LoadHeaderInformation();
				var displaySteps = new int[] { CurrentStep };
				uclAuditDetails.Refresh(EditAuditId, displaySteps);
				pnlAddEdit.Visible = false;
			}

		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			//accessLevel = UserContext.CheckAccess("EHS", "312");
			accessLevel = UserContext.CheckAccess("EHS", "");

			if (accessLevel >= AccessMode.Admin)
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