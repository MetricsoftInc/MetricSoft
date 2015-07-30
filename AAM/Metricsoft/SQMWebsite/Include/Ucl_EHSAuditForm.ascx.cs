﻿using System;
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

		protected decimal SelectedLocationId
		{
			get { return ViewState["SelectedLocationId"] == null ? 0 : (decimal)ViewState["SelectedLocationId"]; }
			set { ViewState["SelectedLocationId"] = value; }
		}


		
		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//accessLevel = UserContext.CheckAccess("EHS", "312");
			accessLevel = UserContext.CheckAccess("EHS", "");
			entities = new PSsqmEntities();
			controlQuestionChanged = false;

			//if (Mode == AuditMode.Audit)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx";
			//else if (Mode == AuditMode.Prevent)
			//	ahReturn.HRef = "/EHS/EHS_Audits.aspx?mode=prevent";

			if (IsPostBack)
			{
				var sourceId = Page.Request[Page.postEventSourceID];

				if (sourceId == "")
				{
					LoadHeaderInformation();
					UpdateAuditTypes();

					divAuditForm.Visible = true;
					BuildForm();
				}

				if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
				{
					// Stop extra script warning in when not actually editing a form
					string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
					ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
				}
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
				var sourceId = Page.Request[Page.postEventSourceID];

				if ((sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn"))) || sourceId == "")
				{
					UpdateControlledQuestions();
				}
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
					auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId);
					selectString = "[Select An Audit Type]";
				}
				//else if (Mode == AuditMode.Prevent)
				//{
				//	auditTypeList = EHSAuditMgr.SelectPreventativeTypeList(companyId);
				//	selectString = "[Select A Preventative Action Type]";
				//}
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
			if (IsEditContext)
			{
				// in edit mode, load the header field values and make all fields display only
				ddlScheduleScope.Visible = false;
				mnuScheduleScope.Visible = false;
				dmAuditDate.Enabled = false;
				tbDescription.Enabled = false;

			}
			else
			{
				// set up for adding the header info
				AccessMode accessmode = UserContext.RoleAccess();

				ddlScheduleScope.Items.Clear();
				mnuScheduleScope.Items.Clear();

				if (accessmode >= AccessMode.Plant)
				{
					// List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
					List<BusinessLocation> locationList = SessionManager.PlantList;
					locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");
					locationList = UserContext.FilterPlantAccessList(locationList, "SQM", "");

					if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone") == false)
					{
						ddlScheduleScope.Visible = false;
						mnuScheduleScope.Visible = true;
						SQMBasePage.SetLocationList(mnuScheduleScope, locationList, 0, "Select a Location", "", true);
						//RadMenuItem mi = new RadMenuItem();
						//mi.Text = (SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME);
						//mi.Value = "0";
						//mi.ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
						//mnuScheduleScope.Items[0].Items.Insert(0, mi);
					}
					else
					{
						ddlScheduleScope.Visible = true;
						mnuScheduleScope.Visible = false;
						SQMBasePage.SetLocationList(ddlScheduleScope, locationList, 0, true);
						//ddlScheduleScope.Items.Insert(0, new RadComboBoxItem((SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME), "0"));
						ddlScheduleScope.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
					}
				}
				// set defaults for add mode
				dmAuditDate.ShowPopupOnFocus = true;
				dmAuditDate.SelectedDate = DateTime.Now;

			}

		}

		public void BuildForm()
		{
			if (accessLevel <= AccessMode.View)
				return;

			if (Mode == AuditMode.Prevent)
			{
				if (accessLevel <= AccessMode.Plant)
				{
					pnlAuditHeader.Visible = false;

					if (CurrentStep == 0)
					{
						if (IsEditContext)
						{
							ShowAuditDetails(EditAuditId, "Recommendation Details");
							btnSaveReturn.Visible = false;
							btnSaveContinue.Visible = false;
						}
						return;
					}
				}
				if (CurrentStep == 1)
				{
					uclAuditDetails.Visible = true;
					var displaySteps = new int[] { 0 };
					uclAuditDetails.Refresh(EditAuditId, displaySteps);
				}
			}

			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			pnlForm.Controls.Clear();
			pnlForm.Visible = true;
			lblResults.Visible = false;

			//if (typeId == 10)
			//{
			//	preventionLocationForm = (Ucl_PreventionLocation)LoadControl("~/Include/Ucl_PreventionLocation.ascx");
			//	preventionLocationForm.ID = "plf1";
			//	preventionLocationForm.IsEditContext = IsEditContext;
			//	preventionLocationForm.AuditId = EditAuditId;

			//	preventionLocationForm.BuildCaseComboBox();
			//	if (IsEditContext == true)
			//		preventionLocationForm.PopulateForm();

			//	pnlForm.Controls.Add(new LiteralControl("<br/>"));
			//	pnlForm.Controls.Add(preventionLocationForm);
			//	pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));
			//	btnSaveReturn.Visible = false;
			//	btnSaveContinue.Visible = false;
			//	return;
			//}

			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0);

			pnlForm.Controls.Add(new LiteralControl("<br/><table width=\"100%\" cellpadding=\"5\" cellspacing=\"0\" style=\"border-collapse: collapse;\">"));
			string previousTopic = "";
			foreach (var q in questions)
			{
				var validator = new RequiredFieldValidator();

				// Look up answer if edit context
				var localQuestion = q;
				if (IsEditContext)
				{
					//q.AnswerText = (from a in entities.AUDIT_ANSWER
					//				where a.AUDIT_ID == EditAuditId
					//					&& a.AUDIT_QUESTION_ID == localQuestion.QuestionId
					//				select a.ANSWER_VALUE).FirstOrDefault();
					var auditAnswer = (from a in entities.AUDIT_ANSWER
									   where a.AUDIT_ID == EditAuditId
										   && a.AUDIT_QUESTION_ID == localQuestion.QuestionId
									   select a).FirstOrDefault();
					q.AnswerText = auditAnswer.ANSWER_VALUE;
					q.AnswerComment = auditAnswer.COMMENT;
				}
				bool shouldPopulate = IsEditContext && !string.IsNullOrEmpty(q.AnswerText);

				string qid = q.QuestionId.ToString();
				string tid = "T" + q.TopicId.ToString();

				if (!previousTopic.Equals(tid)) // add a topic header
				{
					var pnlTopic = new Panel() { ID = "Panel" + tid };
					pnlTopic.Controls.Add(new LiteralControl("<tr><td colspan=\"4\" class=\"blueCell\" style=\"width: 100%;\">"));
					//pnlTopic.Controls.Add(new LiteralControl("<tr><td class=\"tanCell\" style=\"width: 30%;\">"));
					pnlTopic.Controls.Add(new Label() { ID = "Label" + tid, Text = q.TopicTitle });
				//	pnlTopic.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				//	//if (!string.IsNullOrEmpty(q.HelpText))
				//	//	AddToolTip(pnlTopic, q);
				//	pnlTopic.Controls.Add(new LiteralControl("</td><td class=\"tanCell\" style=\"width: 10px; padding-left: 0 !important;\">"));
				//	//if (q.IsRequired)
				//	//	pnlTopic.Controls.Add(new LiteralControl("<span class=\"requiredStar\">&bull;</span>"));
				//	//if (q.IsRequiredClose)
				//	//	pnlTopic.Controls.Add(new LiteralControl("<span class=\"requiredCloseStar\">&bull;</span>"));

				//	pnlTopic.Controls.Add(new LiteralControl("</td><td class=\"tanCell\">"));
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
						else if (q.QuestionId == (decimal)EHSAuditQuestionId.Location) // Location
							tf.Text = SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME;
						if (q.QuestionId == (decimal)EHSAuditQuestionId.CompletedBy)
							tf.Enabled = false;
						pnl.Controls.Add(tf);
						break;

					case EHSAuditQuestionType.TextBox:
						var tb = new RadTextBox() { ID = qid, Width = 550, TextMode = InputMode.MultiLine, Rows = 6, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
						if (shouldPopulate)
							tb.Text = q.AnswerText;
						pnl.Controls.Add(tb);

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
						if (q.QuestionId == (decimal)EHSAuditQuestionId.AuditDate) // Default audit date
						{
							rdp.SelectedDate = DateTime.Now;
						}
						if (q.QuestionId == (decimal)EHSAuditQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
						{
							rdp.SelectedDate = DateTime.Now;
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
							if (q.QuestionId == (decimal)EHSAuditQuestionId.ProjectedCompletionDate) 
							{
								if (EditAuditId > 0)
								{
									AUDIT audit = (from inc in entities.AUDIT where inc.AUDIT_ID == EditAuditId select inc).FirstOrDefault();
									if (audit != null)
									{
                                        // mt - due date now based on Audit creation date per TI
										//string dateAnswer = EHSAuditMgr.SelectAuditAnswer(audit, (decimal)EHSAuditQuestionId.ReportDate);
										//DateTime parseDate;
										//if (DateTime.TryParse(dateAnswer, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
                                        DateTime parseDate = DateTime.Now;
										//{
											string answer = EHSAuditMgr.SelectAuditAnswer(audit, (decimal)EHSAuditQuestionId.RecommendationType);
											if (answer.ToLower() == "behavioral")
												rdp.SelectedDate = parseDate.AddDays(30);
											else
												rdp.SelectedDate = parseDate.AddDays(60);
										//}
									}
								}
							}
						}
						
						// Audit report date, completion date, projected completion date are not editable
						if (q.QuestionId == (decimal)EHSAuditQuestionId.ReportDate ||
							q.QuestionId == (decimal)EHSAuditQuestionId.CompletionDate ||
							q.QuestionId == (decimal)EHSAuditQuestionId.ProjectedCompletionDate) 
						{
							rdp.Enabled = false;
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
						//else if (q.QuestionId == (decimal)EHSAuditQuestionId.Create8D && EHSAuditMgr.IsTypeDefault8D(typeId))
						//	bcb.Checked = true;

						// If controller question, "close" checkbox, or "create 8d" checkbox, register ajax behavior
						if ((q.QuestionControls != null && q.QuestionControls.Count > 0) ||
							q.QuestionId == (decimal)EHSAuditQuestionId.CloseAudit )
						{
							bcb.CheckedChanged += new EventHandler(bcb_CheckedChanged);
							if (q.QuestionId == (decimal)EHSAuditQuestionId.CloseAudit)
								bcb.CheckedChanged += new EventHandler(bcb_CheckedChangedClose);
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

					case EHSAuditQuestionType.CurrencyTextBox:
						var ctb = new RadNumericTextBox() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Type = NumericType.Number };
						ctb.NumberFormat.DecimalDigits = 2;
						if (shouldPopulate)
							ctb.Text = q.AnswerText;

						if (EditAuditId > 0 && Mode == AuditMode.Prevent)
						{
							AUDIT audit = (from inc in entities.AUDIT where inc.AUDIT_ID == EditAuditId select inc).FirstOrDefault();
							if (audit != null)
							{
								string answer = EHSAuditMgr.SelectAuditAnswer(audit, (decimal)EHSAuditQuestionId.RecommendationType);
								if (!string.IsNullOrEmpty(answer) && answer.ToLower() != "infrastructure")
								{
									ctb.Enabled = false;
									pnl.Visible = false;
								}
							}
						}

						pnl.Controls.Add(ctb);
						break;

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

				// Add a comment box that hides/shows via a link to certain field types
				if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
					q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio)
				{
					var comment = new RadTextBox() { ID = "Comment" + qid, Width = 400, MaxLength = MaxTextLength, Skin = "Metro", CssClass = "WarnIfChanged" };
					comment.Rows = 2;
					pnl.Controls.Add(comment);
				}

				pnl.Controls.Add(new LiteralControl("</td></tr>"));

				if (q.QuestionId == (decimal)EHSAuditQuestionId.FinalAuditStepResolved && accessLevel < AccessMode.Admin)
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSAuditQuestionId.CostToImplement && accessLevel < AccessMode.Admin)
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));

			UpdateAnswersFromForm();

			UpdateButtonText();
		}

		protected void ScheduleScope_Select(object sender, EventArgs e)
		{
			string location = "0";
			if (sender is RadMenu)
			{
				location = mnuScheduleScope.SelectedItem.Value;
				mnuScheduleScope.Items[0].Text = mnuScheduleScope.SelectedItem.Text;
			}
			else if (sender is RadSlider)
			{
				location = ddlScheduleScope.SelectedValue;
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

			int chInt = (int)EHSAuditQuestionId.Create8D;
			string chString = chInt.ToString();
			CheckBox create8dCh = (CheckBox)pnlForm.FindControl(chString);

			if (create8dCh != null && create8dCh.Checked == true)
			{
				if (IsEditContext)
					btnSaveContinue.Text = "Save & Edit 8D";
				else
					btnSaveContinue.Text = "Save & Create 8D";
			}
			else
			{
				if (IsEditContext)
					btnSaveContinue.Text = "Save & Edit Report";
				else
					btnSaveContinue.Text = "Save & Create Report";
			}
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
						locationPersonList = EHSAuditMgr.SelectEhsDataOriginatorsAtPlant(locationId);
						locationPersonList.AddRange(EHSAuditMgr.SelectDataOriginatorAdditionalPlantAccess(locationId));
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

		void UpdateClosedQuestionsPrevent()
		{
			if (AuditReportRequiredFieldsComplete() == true)
			{
			}
		}

		protected bool AuditReportRequiredFieldsComplete()
		{
			int score = 0;
			foreach (int fId in requiredToCloseFields)
			{
				var field = pnlForm.FindControl(fId.ToString());
                if (field == null)   //mt - need to enable close if field was not included in the audit meta-data
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

			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;
			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0);
			EHSAuditQuestion question = questions.Where(q => q.QuestionId == questionId).FirstOrDefault();
			ProcessQuestionControls(question, 0);

			controlQuestionChanged = false;
		}

		protected void UpdateControlledQuestions()
		{
			decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

			questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0);
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
				if (IsEditContext == true)
				{
					CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
					Save(true);
				}
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
				btnSaveContinue.Visible = false;
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
			bool shouldCreate8d = false;
			string result = "<h3>EHS Audit " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			if (Mode == AuditMode.Prevent)
				result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			if (shouldReturn == true)
			{
				pnlForm.Visible = false;

				pnlAddEdit.Visible = false;
				btnSaveReturn.Visible = false;
				btnSaveContinue.Visible = false;

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
			
			questions = EHSAuditMgr.SelectAuditQuestionList(auditTypeId, 0);
			UpdateAnswersFromForm();
			GetAuditInfoFromQuestions(questions);

			if (CurrentStep == 0)
			{
				GetAuditInfoFromQuestions(questions);
				if (!IsEditContext)
				{
					// Add context - step 0
					theAudit = CreateNewAudit();
                    auditId = theAudit.AUDIT_ID;
					EHSNotificationMgr.NotifyOnCreate(auditId, selectedPlantId);
				}
				else
				{
					// Edit context - step 0
					auditId = EditAuditId;
					if (auditId > 0)
					{
						theAudit = UpdateAudit(auditId);
                        if (Mode == AuditMode.Audit)
                        {
                            EHSAuditMgr.TryCloseAudit(auditId);
                        }
					}
				}
				if (auditId > 0)
				{
					shouldCreate8d = AddOrUpdateAnswers(questions, auditId);
					SaveAttachments(auditId);
				}

				if (Mode == AuditMode.Prevent)
                    UpdateTaskInfo(questions, auditId, (DateTime)theAudit.CREATE_DT);
			}
			else if (CurrentStep == 1)
			{
				// Edit context - step 1
				auditId = EditAuditId;
				if (auditId > 0)
				{
					AddOrUpdateAnswers(questions, auditId);
					shouldCreate8d = false;
					SaveAttachments(auditId);

					if (Mode == AuditMode.Audit)
					{
                        UpdateTaskInfo(questions, auditId, DateTime.Now);
						EHSAuditMgr.TryCloseAudit(auditId);
					}
					else
					{
						EHSAuditMgr.TryClosePrevention(auditId, SessionManager.UserContext.Person.PERSON_ID);
					}
				}
			}

			decimal finalPlantId = 0;
			var finalAudit = EHSAuditMgr.SelectAuditById(entities, auditId);
			if (finalAudit != null)
				finalPlantId = (decimal)finalAudit.DETECT_PLANT_ID;
			else
				finalPlantId = selectedPlantId;

			// Start plant accounting rollup in a background thread

			Thread thread = new Thread(() => EHSAccountingMgr.RollupPlantAccounting(InitialPlantId, finalPlantId));
			thread.IsBackground = true;
			thread.Start();

			//Thread obj = new Thread(new ThreadStart(EHSAccountingMgr.RollupPlantAccounting(initialPlantId, finalPlantId)));
            //obj.IsBackground = true;


			//if (shouldCreate8d == true && shouldReturn == false)
			//	Create8dAndRedirect(auditId);
			//else 
			if (CurrentStep == 0 && shouldReturn == false)
				GoToNextStep(auditId);
			else
				ShowAuditDetails(auditId, result);

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
							if (q.QuestionId == (decimal)EHSAuditQuestionId.Description)
								auditDescription = answer;
							break;

						case EHSAuditQuestionType.RichTextBox:
							answer = (control as RadEditor).Content;
							if (q.QuestionId == (decimal)EHSAuditQuestionId.RecommendationSummary)
								auditDescription = answer;
							break;

						case EHSAuditQuestionType.Radio:
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
								if (q.QuestionId == (decimal)EHSAuditQuestionId.AuditDate || q.QuestionId == (decimal)EHSAuditQuestionId.InspectionDate)
									auditDate = (DateTime)fromDate;
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
					q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio)
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

		protected void GetAuditInfoFromQuestions(List<EHSAuditQuestion> questions)
		{
			// ABW - this was already commented out in incidents!!
			//foreach (var q in questions)
			//{
			//	string answer = q.AnswerText;

			//	if (answer != null)
			//	{
			//		// Special case values to populate in audit table
			//		if (q.QuestionType == EHSAuditQuestionType.TextBox && q.QuestionId == (decimal)EHSAuditQuestionId.Description)
			//			auditDescription = answer;
			//		if (q.QuestionType == EHSAuditQuestionType.Date && q.QuestionId == (decimal)EHSAuditQuestionId.AuditDate)
			//			auditDate = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US"));
			//	}

			//	if (answer.Length > MaxTextLength)
			//		answer = answer.Substring(0, MaxTextLength);
			//	if (auditDescription.Length > MaxTextLength)
			//		auditDescription = auditDescription.Substring(0, MaxTextLength);
			//}
		}

		protected bool AddOrUpdateAnswers(List<EHSAuditQuestion> questions, decimal auditId)
		{
			bool shouldCreate8d = false;

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
					auditAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText;
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

				// Check if Quality Issue (8D) question set to true
				if (q.QuestionId == (decimal)EHSAuditQuestionId.Create8D && q.AnswerText == "Yes")
					shouldCreate8d = true;
			}
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
				DETECT_PLANT_ID = auditPlantId,
				AUDIT_TYPE = "EHS",
				CREATE_DT = DateTime.Now,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = auditDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				AUDIT_DT = auditDate,
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
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			if (audit != null)
			{
				audit.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				audit.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				audit.DETECT_PLANT_ID = selectedPlantId;
				audit.AUDIT_TYPE = "EHS";
				//audit.CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				audit.DESCRIPTION = auditDescription;
				//audit.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				audit.AUDIT_DT = auditDate;
				//audit.ISSUE_TYPE = auditType;
				audit.AUDIT_TYPE_ID = auditTypeId;

				entities.SaveChanges();
			}

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
			DateTime dueDate = DateTime.MinValue;
			decimal responsiblePersonId = 0;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var auditAnswer = (from ia in entities.AUDIT_ANSWER
									  where ia.AUDIT_ID == auditId
										  && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();

				if (auditAnswer != null && !String.IsNullOrEmpty(auditAnswer.ANSWER_VALUE))
				{
					if (Mode == AuditMode.Audit)
					{
						if (q.QuestionId == (decimal)EHSAuditQuestionId.DateDue)
							dueDate = DateTime.Parse(auditAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US"));
						else if (q.QuestionId == (decimal)EHSAuditQuestionId.ResponsiblePersonDropdown) 
							responsiblePersonId = Convert.ToDecimal(auditAnswer.ANSWER_VALUE);
					}
					else if (Mode == AuditMode.Prevent)
					{
						if (q.QuestionId == (decimal)EHSAuditQuestionId.ReportDate)
							dueDate = createDate.AddDays(30);  // mt - per TI the due date should be based on the audit CREATE date instead of the inspection date
                            // dueDate = DateTime.Parse(auditAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US")).AddDays(30);
						else if (q.QuestionId == (decimal)EHSAuditQuestionId.AssignToPerson) 
							responsiblePersonId = Convert.ToDecimal(auditAnswer.ANSWER_VALUE);
					}
				}
			}
			
			//if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			//{
			//	int recordTypeId = (Mode == AuditMode.Prevent) ? 45 : 40;
			//	EHSAuditMgr.CreateOrUpdateTask(auditId, responsiblePersonId, recordTypeId, dueDate);
			//}
		}

		

		//protected void Create8dAndRedirect(decimal auditId)
		//{
		//	SessionManager.ReturnStatus = true;

		//	PROB_CASE probCase = ProblemCase.LookupCaseByAudit(auditId);
		//	if (probCase != null)
		//	{
		//		// If 8D problem case exists, redirect with problem case ID
		//		SessionManager.ReturnObject = probCase.PROBCASE_ID;
		//	}
		//	else
		//	{
		//		// Otherwise, redirect with the intent of creating a new problem case
		//		var entities = new PSsqmEntities();
		//		SessionManager.ReturnObject = EHSAuditMgr.SelectAuditById(entities, auditId);
		//	}

		//	Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
		//}

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

			if (accessLevel > AccessMode.View)
			{
				if (!IsEditContext)
				{
					// Add
					btnSaveReturn.Enabled = false;
					btnSaveReturn.Visible = (SelectedTypeId > 0);
					btnSaveContinue.Visible = (SelectedTypeId > 0);
					if (Mode == AuditMode.Audit)
						typeString = "Audit";
					else if (Mode == AuditMode.Prevent)
						typeString = "Recommendation";
				
					rddlAuditType.Visible = (rddlAuditType.Items.Count == 1) ? false : true;
					lblAddOrEditAudit.Text = "<strong>Add a New " + typeString + ":</strong>";
					
					lblAuditType.Visible = false;
					btnDelete.Visible = false;
				}
				else
				{
					// Edit
					if (CurrentStep == 0)
					{
						if (Mode == AuditMode.Audit)
							typeString = " Notification";
						else if (Mode == AuditMode.Prevent)
							typeString = " Recommendations";
						btnSaveContinue.Visible = true;
						btnSaveReturn.CommandArgument = "0";
					}
					else if (CurrentStep == 1)
					{
						if (Mode == AuditMode.Audit)
							typeString = " Report";
						else if (Mode == AuditMode.Prevent)
							typeString = " Response";
						btnSaveContinue.Visible = false;
						btnSaveReturn.CommandArgument = "1";
					}

					SelectedTypeId = 0;
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;

					lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

					rddlAuditType.Visible = false;
					if (Mode == AuditMode.Audit)
						lblAuditType.Text = "Audit Type: ";
					else if (Mode == AuditMode.Prevent)
						lblAuditType.Text = "Type: ";
					
					lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
					lblAuditType.Visible = true;
					btnDelete.Visible = true;
					BuildForm();
				}

				UpdateControlledQuestions();
				UpdateButtonText();

				if (CurrentStep == 1)
				{
					if (Mode == AuditMode.Audit)
						UpdateClosedQuestions();
					else if (Mode == AuditMode.Prevent)
						UpdateClosedQuestionsPrevent();
				}
				
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
				var displaySteps = new int[] { CurrentStep };
				uclAuditDetails.Refresh(EditAuditId, displaySteps);
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