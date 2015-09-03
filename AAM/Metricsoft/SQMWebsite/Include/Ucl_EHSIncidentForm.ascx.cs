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

		protected AccessMode accessLevel;
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
		protected decimal incidentTypeId;
		protected string incidentType;

		public Ucl_EHSIncidentDetails IncidentDetails
		{
			get { return uclIncidentDetails; }
		}

		public void EnableReturnButton(bool bEnabled)
		{
			ahReturn.Visible = bEnabled;
		}

		// Mode should be "incident" (standard) or "prevent" (RMCAR)
		public IncidentMode Mode
		{
			get { return ViewState["Mode"] == null ? IncidentMode.Incident : (IncidentMode)ViewState["Mode"]; }
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

		protected decimal EditIncidentTypeId
		{
			get { return EditIncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(EditIncidentId); }
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

			if (Mode == IncidentMode.Incident)
			{
				ahReturn.HRef = "/EHS/EHS_Incidents.aspx";
				btnSaveReturn.Visible = btnSaveContinue.Visible = false;
			}
			else if (Mode == IncidentMode.Prevent)
				ahReturn.HRef = "/EHS/EHS_Incidents.aspx?mode=prevent";


			UpdateIncidentTypes();

			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
			}

			if (IsPostBack)
			{
				if (uclContainment.Visible == true || uclRootCause.Visible == true  ||  uclAction.Visible == true  ||  uclApproval.Visible == true)
				{
					return;
				}
				divIncidentForm.Visible = true;

				LoadHeaderInformation();
					
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
				//UpdateControlledQuestions();
			}
		}

		#region Form

		protected void UpdateIncidentTypes()
		{
			if (!IsEditContext)
			{
				var incidentTypeList = new List<INCIDENT_TYPE>();
				string selectString = "";
				if (Mode == IncidentMode.Incident)
				{
					incidentTypeList = EHSIncidentMgr.SelectIncidentTypeList(companyId);
					selectString = "&nbsp;&nbsp;[Select An Incident Type]";
				}
				else if (Mode == IncidentMode.Prevent)
				{
					incidentTypeList = EHSIncidentMgr.SelectPreventativeTypeList(companyId);
					selectString = "[Select A Preventative Action Type]";
				}
				if (incidentTypeList.Count > 1)
					incidentTypeList.Insert(0, new INCIDENT_TYPE() { INCIDENT_TYPE_ID = 0, TITLE = selectString });
				
				if (accessLevel < AccessMode.Admin)
						incidentTypeList = (from i in incidentTypeList where i.INCIDENT_TYPE_ID != 10 select i).ToList();

				rddlIncidentType.Font.Bold = true;
				rddlIncidentType.DataSource = incidentTypeList;
				rddlIncidentType.DataTextField = "TITLE";
				rddlIncidentType.DataValueField = "INCIDENT_TYPE_ID";
				rddlIncidentType.DataBind();
			}
		}

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info
			AccessMode accessmode = UserContext.RoleAccess();

			if (IsEditContext || CurrentStep > 0)
			{
				// in edit mode, load the header field values and make all fields display only
	
				var incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();

			BusinessLocation location = new BusinessLocation().Initialize((decimal)incident.DETECT_PLANT_ID);

			rddlIncidentType.Enabled = false;
			rddlIncidentType.Visible = false;

			lblIncidentLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
			lblIncidentLocation.Visible = true;

				ddlIncidentLocation.Visible = false;
				mnuIncidentLocation.Visible = false;

				//lblAuditDescription.Text = audit.DESCRIPTION;
				//lblAuditDescription.Visible = true;
				//tbDescription.Visible = false;

				// build the audit user list
				//lblAuditPersonName.Text = EHSAuditMgr.SelectUserNameById((Decimal)audit.AUDIT_PERSON);
				//lblAuditPersonName.Visible = true;
				//rddlAuditUsers.Visible = false;

				//lblAuditDueDate.Text = audit.AUDIT_DT.ToString("MM/dd/yyyy");
				//lblAuditDueDate.Visible = true;
				//dmAuditDate.Enabled = false;
				//dmAuditDate.Visible = false;
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
						if (mnuIncidentLocation.Items.Count == 0)
						{
							mnuIncidentLocation.Items.Clear();

							ddlIncidentLocation.Visible = false;
							mnuIncidentLocation.Visible = true;
							mnuIncidentLocation.Enabled = true;
							SQMBasePage.SetLocationList(mnuIncidentLocation, locationList, 0, "[Select a Location]", "", true);
						}
					}
					else
					{
						if (ddlIncidentLocation.Items.Count == 0)
						{
							ddlIncidentLocation.Items.Clear();
							ddlIncidentLocation.Visible = true;
							ddlIncidentLocation.Enabled = true;
							mnuIncidentLocation.Visible = false;
							SQMBasePage.SetLocationList(ddlIncidentLocation, locationList, 0, true);
							ddlIncidentLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
						}
					}
				}
				// set defaults for add mode
				rddlIncidentType.Enabled = false;
				rddlIncidentType.Visible = false;
				//lblAuditLocation.Visible = false;
				//lblAuditDescription.Visible = false;
				//tbDescription.Visible = true;
				//rddlAuditUsers.Enabled = true;
				//rddlAuditUsers.Visible = true;
				//lblAuditPersonName.Visible = false;
				//lblAuditDueDate.Visible = false;
				//dmAuditDate.Visible = true;
				//dmAuditDate.Enabled = true;
				//dmAuditDate.ShowPopupOnFocus = true;
				//if (!dmAuditDate.SelectedDate.HasValue)
				//	dmAuditDate.SelectedDate = DateTime.Now;

			}
		}

		public void BuildForm()
		{
			if (accessLevel <= AccessMode.View)
				return;

			if (Mode == IncidentMode.Prevent)
			{
				if (accessLevel <= AccessMode.Plant)
				{
					pnlIncidentHeader.Visible = false;

					if (CurrentStep == 0)
					{
						if (IsEditContext)
						{
							ShowIncidentDetails(EditIncidentId, "Recommendation Details");
							btnSaveReturn.Visible = false;
							//btnSaveContinue.Visible = false;
						}
						return;
					}
				}
				if (CurrentStep == 1)
				{
					uclIncidentDetails.Visible = true;
					var displaySteps = new int[] { 0 };
					uclIncidentDetails.Refresh(EditIncidentId, displaySteps);
				}
			}

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			IsUseCustomForm = EHSIncidentMgr.IsUseCustomForm(typeId);

			if (typeId < 1)
				return;

			INCIDENT incident = null;
			if (EditIncidentId > 0)
			{
				incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
				SessionManager.SetIncidentLocation(Convert.ToDecimal(incident.DETECT_PLANT_ID));
			}

			string typeText = SelectedTypeText;
			incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);

			pnlForm.Controls.Clear();
			divForm.Visible = true;
			//divForm.Visible = pnlForm.Visible = pnlContainment.Visible = pnlRootCause.Visible = pnlAction.Visible = pnlApproval.Visible = true;
			lblResults.Visible = false;

			if (IsUseCustomForm)
			{
				BuildCustomForm(typeId);
				return;
			}

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

				if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.SessionContext.Language().NLS_LANGUAGE) == false)
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
							validator.InitialValue = "[Select One]";

						if (Mode == IncidentMode.Prevent)
							validator.EnableClientScript = false;

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
							var li = new ListItem(choice.Value);
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

					case EHSIncidentQuestionType.Dropdown:
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

					case EHSIncidentQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						rdp.ShowPopupOnFocus = true;
						if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate) // Default incident date
						{
							rdp.SelectedDate = DateTime.Now;
						}
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
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
						var bcb = new CheckBox() { ID = qid, Text = "Yes", CssClass = "WarnIfChanged" };

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
							uploader.GetUploadedFiles(40, EditIncidentId, "1");
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

						if (EditIncidentId > 0 && Mode == IncidentMode.Prevent)
						{
							//INCIDENT incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
							if (incident != null)
							{
								string answer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.RecommendationType);
								if (!string.IsNullOrEmpty(answer) && answer.ToLower() != "infrastructure")
								{
									ctb.Enabled = false;
									pnl.Visible = false;
								}
							}
						}

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
								rddlLocation.Items.Add(new DropDownListItem("[Select One]", ""));
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
						rddl3.Items.Add(new DropDownListItem("[Select One]", ""));

						var personList = new List<PERSON>();
						if (CurrentStep == 1)
							personList = EHSIncidentMgr.SelectIncidentPersonList(EditIncidentId);
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

					case EHSIncidentQuestionType.RequiredYesNoRadio:
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

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && accessLevel < AccessMode.Admin)
					pnl.Visible = false;

				if (q.QuestionId == (decimal)EHSQuestionId.CostToImplement && accessLevel < AccessMode.Admin)
					pnl.Visible = false;

				pnlForm.Controls.Add(pnl);
			}

			pnlForm.Controls.Add(new LiteralControl("</table>"));
			pnlForm.Controls.Add(new LiteralControl("<br/><br/>")); { }

			UpdateAnswersFromForm();

			UpdateButtonText();
		}

		public void BuildCustomForm(decimal typeId)
		{

			string baseCustomForm = EHSIncidentMgr.SelectBaseFormNameByIncidentTypeId(typeId);

			switch (baseCustomForm)
			{
				case "INCFORM_INJURYILLNESS":

					try
					{
						injuryIllnessForm = (Ucl_INCFORM_InjuryIllness)LoadControl("~/Include/Ucl_INCFORM_InjuryIllness.ascx");
					}
					catch (Exception e)
					{
					}

					injuryIllnessForm.ID = "iif1";
					injuryIllnessForm.IsEditContext = IsEditContext;
					injuryIllnessForm.IncidentId = EditIncidentId;
					injuryIllnessForm.EditIncidentId = EditIncidentId;
					injuryIllnessForm.SelectedTypeId = SelectedTypeId;
					injuryIllnessForm.SelectedTypeText = SelectedTypeText;
					pnlForm.Controls.Add(new LiteralControl("<br/>"));
					pnlForm.Controls.Add(injuryIllnessForm);
					pnlForm.Controls.Add(new LiteralControl("<br/><br/>"));
					btnSaveReturn.Visible = false;
					btnSaveContinue.Visible = false;
					btnDelete.Visible = false;
					break;

			}

			SetSubnav("custom");

		}

		public void GetForm()
		{
			if (accessLevel <= AccessMode.View)
				return;

			if (Mode == IncidentMode.Prevent)
			{
				if (accessLevel <= AccessMode.Plant)
				{
					pnlIncidentHeader.Visible = false;

					if (CurrentStep == 0)
					{
						if (IsEditContext)
						{
							ShowIncidentDetails(EditIncidentId, "Recommendation Details");
							btnSaveReturn.Visible = false;
							btnSaveContinue.Visible = false;
						}
						return;
					}
				}
				if (CurrentStep == 1)
				{
					uclIncidentDetails.Visible = true;
					var displaySteps = new int[] { 0 };
					uclIncidentDetails.Refresh(EditIncidentId, displaySteps);
				}
			}

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			if (typeId < 1)
				return;

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

				if (q.QuestionId == (decimal)EHSQuestionId.NativeLangComment && EHSIncidentMgr.EnableNativeLangQuestion(SessionManager.SessionContext.Language().NLS_LANGUAGE) == false)
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
							validator.InitialValue = "[Select One]";

						if (Mode == IncidentMode.Prevent)
							validator.EnableClientScript = false;

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
							var li = new ListItem(choice.Value);
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

					case EHSIncidentQuestionType.Dropdown:
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

					case EHSIncidentQuestionType.Date:
						var rdp = new RadDatePicker() { ID = qid, Skin = "Metro", CssClass = "WarnIfChanged", Width = 400 };
						rdp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
						rdp.ShowPopupOnFocus = true;
						if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate) // Default incident date
						{
							rdp.SelectedDate = DateTime.Now;
						}
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate && !IsEditContext) // Default report date if add mode
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
						var bcb = new CheckBox() { ID = qid, Text = "Yes", CssClass = "WarnIfChanged" };

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
							uploader.GetUploadedFiles(40, EditIncidentId, "1");
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

						if (EditIncidentId > 0 && Mode == IncidentMode.Prevent)
						{
							//INCIDENT incident = (from inc in entities.INCIDENT where inc.INCIDENT_ID == EditIncidentId select inc).FirstOrDefault();
							if (incident != null)
							{
								string answer = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.RecommendationType);
								if (!string.IsNullOrEmpty(answer) && answer.ToLower() != "infrastructure")
								{
									ctb.Enabled = false;
									pnl.Visible = false;
								}
							}
						}

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

					case EHSIncidentQuestionType.UsersDropdown:
						var rddl3 = new RadDropDownList() { ID = qid, Width = 550, Skin = "Metro", CssClass = "WarnIfChanged", ValidationGroup = "Val" };
						rddl3.Items.Add(new DropDownListItem("[Select One]", ""));

						var personList = new List<PERSON>();
						if (CurrentStep == 1)
							personList = EHSIncidentMgr.SelectIncidentPersonList(EditIncidentId);
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

					case EHSIncidentQuestionType.RequiredYesNoRadio:
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

			int chInt = (int)EHSQuestionId.Create8D;
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

			if (IsEditContext)
				SetSubnav("edit");
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
				rddlFilteredUsers.Items.Add(new DropDownListItem("[Select One]", ""));

				var locationPersonList = new List<PERSON>();
				if (this.SelectedLocationId > 0)
				{
					locationPersonList = EHSIncidentMgr.SelectEhsDataOriginatorsAtPlant(this.SelectedLocationId);
					locationPersonList.AddRange(EHSIncidentMgr.SelectDataOriginatorAdditionalPlantAccess(this.SelectedLocationId));
					locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
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

		public void CheckForSingleType()
		{
			if (rddlIncidentType.Items.Count == 1)
			{
				string selectedTypeId = rddlIncidentType.Items[0].Value;
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

		protected void rddlIncidentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedTypeId = rddlIncidentType.SelectedValue;
			if (!string.IsNullOrEmpty(selectedTypeId))
			{
				//Session["IncidentTypeID"] = selectedTypeId;
				SelectedTypeId = Convert.ToDecimal(selectedTypeId);
				SelectedTypeText = rddlIncidentType.SelectedText;
				IsEditContext = false;

				rddlIncidentType.Enabled = false;
				rddlIncidentType.Visible = true;
				ddlIncidentLocation.Enabled = false;
				mnuIncidentLocation.Enabled = false;

				BuildForm();
			}
		}

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				CurrentSubnav = (sender as RadButton).CommandArgument;
				CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
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

			rddlIncidentType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
			INCIDENT theIncident = null;
			decimal incidentId = 0;
			bool shouldCreate8d = false;
			string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			if (Mode == IncidentMode.Prevent)
				result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			SessionManager.ClearIncidentLocation();

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

			if (!IsEditContext)
			{
				incidentTypeId = SelectedTypeId;
				incidentType = rddlIncidentType.SelectedText;
			}
			else
			{
				incidentTypeId = EditIncidentTypeId;
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);
			}
			
			questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, CurrentStep);
			UpdateAnswersFromForm();
			GetIncidentInfoFromQuestions(questions);

			if (CurrentStep == 0)
			{
				GetIncidentInfoFromQuestions(questions);
				if (!IsEditContext)
				{
					// Add context - step 0
					theIncident = CreateNewIncident();
					incidentId = theIncident.INCIDENT_ID;
					//EHSNotificationMgr.NotifyOnCreate(incidentId, selectedPlantId);
					EHSNotificationMgr.NotifyIncidentStatus(theIncident, "IN-0", ((int)SysPriv.originate).ToString());
				}
				else
				{
					// Edit context - step 0
					incidentId = EditIncidentId;
					if (incidentId > 0)
					{
						theIncident = UpdateIncident(incidentId);
						if (Mode == IncidentMode.Incident)
						{
							EHSIncidentMgr.TryCloseIncident(incidentId);
						}
					}
				}
				if (incidentId > 0)
				{
					shouldCreate8d = AddOrUpdateAnswers(questions, incidentId);
					SaveAttachments(incidentId);
				}

				if (Mode == IncidentMode.Prevent)
					UpdateTaskInfo(questions, incidentId, (DateTime)theIncident.CREATE_DT);
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

					if (Mode == IncidentMode.Incident)
					{
						UpdateTaskInfo(questions, incidentId, DateTime.Now);
						EHSIncidentMgr.TryCloseIncident(incidentId);
					}
					else
					{
						EHSIncidentMgr.TryClosePrevention(incidentId, SessionManager.UserContext.Person.PERSON_ID);
					}
				}
			}

			decimal finalPlantId = 0;
			var finalIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			if (finalIncident != null)
				finalPlantId = (decimal)finalIncident.DETECT_PLANT_ID;
			else
				finalPlantId = selectedPlantId;

			// Start plant accounting rollup in a background thread

			Thread thread = new Thread(() => EHSAccountingMgr.RollupPlantAccounting(InitialPlantId, finalPlantId));
			thread.IsBackground = true;
			thread.Start();

			//Thread obj = new Thread(new ThreadStart(EHSAccountingMgr.RollupPlantAccounting(initialPlantId, finalPlantId)));
			//obj.IsBackground = true;

			if (shouldReturn)
				Response.Redirect("/EHS/EHS_Incidents.aspx");  // mt - temporary

			/*
			if (shouldCreate8d == true && shouldReturn == false)
				Create8dAndRedirect(incidentId);
			else if (CurrentStep == 0 && shouldReturn == false)
				GoToNextStep(incidentId);
			else
				ShowIncidentDetails(incidentId, result);
			*/
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
						case EHSIncidentQuestionType.TextField:
							answer = (control as RadTextBox).Text;
							break;

						case EHSIncidentQuestionType.TextBox:
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

			if (incidentDate == null || incidentDate < DateTime.Now.AddYears(-100))
				incidentDate = DateTime.Now;

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = selectedPlantId;
		}

		protected void GetIncidentInfoFromQuestions(List<EHSIncidentQuestion> questions)
		{
			//foreach (var q in questions)
			//{
			//	string answer = q.AnswerText;

			//	if (answer != null)
			//	{
			//		// Special case values to populate in incident table
			//		if (q.QuestionType == EHSIncidentQuestionType.TextBox && q.QuestionId == (decimal)EHSQuestionId.Description)
			//			incidentDescription = answer;
			//		if (q.QuestionType == EHSIncidentQuestionType.Date && q.QuestionId == (decimal)EHSQuestionId.IncidentDate)
			//			incidentDate = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US"));
			//	}

			//	if (answer.Length > MaxTextLength)
			//		answer = answer.Substring(0, MaxTextLength);
			//	if (incidentDescription.Length > MaxTextLength)
			//		incidentDescription = incidentDescription.Substring(0, MaxTextLength);
			//}
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
				DETECT_PLANT_ID = selectedPlantId,
				INCIDENT_TYPE = "EHS",
				CREATE_DT = DateTime.Now,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = incidentDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				INCIDENT_DT = incidentDate,
				ISSUE_TYPE = incidentType,
				ISSUE_TYPE_ID = incidentTypeId
			};
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
				incident.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				incident.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				incident.DETECT_PLANT_ID = selectedPlantId;
				incident.INCIDENT_TYPE = "EHS";
				//incident.CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				incident.DESCRIPTION = incidentDescription;
				//incident.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				incident.INCIDENT_DT = incidentDate;
				incident.ISSUE_TYPE = incidentType;
				incident.ISSUE_TYPE_ID = incidentTypeId;

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
					if (Mode == IncidentMode.Incident)
					{
						if (q.QuestionId == (decimal)EHSQuestionId.DateDue)
							dueDate = DateTime.Parse(incidentAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US"));
						else if (q.QuestionId == (decimal)EHSQuestionId.ResponsiblePersonDropdown) 
							responsiblePersonId = Convert.ToDecimal(incidentAnswer.ANSWER_VALUE);
					}
					else if (Mode == IncidentMode.Prevent)
					{
						if (q.QuestionId == (decimal)EHSQuestionId.ReportDate)
							dueDate = createDate.AddDays(30);  // mt - per TI the due date should be based on the incident CREATE date instead of the inspection date
							// dueDate = DateTime.Parse(incidentAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US")).AddDays(30);
						else if (q.QuestionId == (decimal)EHSQuestionId.AssignToPerson) 
							responsiblePersonId = Convert.ToDecimal(incidentAnswer.ANSWER_VALUE);
					}
				}
			}
			
			if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			{
				int recordTypeId = (Mode == IncidentMode.Prevent) ? 45 : 40;
				EHSIncidentMgr.CreateOrUpdateTask(incidentId, responsiblePersonId, recordTypeId, dueDate);
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
			rddlIncidentType.SelectedIndex = 0;

			// Display incident details control
			btnDelete.Visible = false;
			lblResults.Text = result.ToString();
			uclIncidentDetails.Visible = true;
			var displaySteps = new int[] { CurrentStep };
			uclIncidentDetails.Refresh(incidentId, displaySteps);
		}

		#endregion

		#region subnav
		private void SetSubnav(string context)
		{
			if (context == "new")
			{
				divSubnavPage.Visible = uclContainment.Visible = uclRootCause.Visible = uclAction.Visible = uclApproval.Visible = false;
				btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = false;
			}
			else if (context == "custom")
			{
				divSubnavPage.Visible = uclContainment.Visible = uclRootCause.Visible = uclAction.Visible = uclApproval.Visible = false;
				btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = false;
				btnSubnavSave.Visible = false;
			}
			else
			{
				divSubnavPage.Visible = uclContainment.Visible = uclRootCause.Visible = uclAction.Visible = uclApproval.Visible = false;
				btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
				btnSubnavIncident.Visible = true;
				btnSubnavIncident.Enabled = false;
				btnSubnavIncident.CssClass = "buttonLinkDisabled";
			}
		}

		protected void btnSubnavSave_Click(object sender, EventArgs e)
		{
			int status = 0;
			switch (CurrentSubnav)
			{
				case "2":
					status = uclContainment.AddUpdateINCFORM_CONTAIN(EditIncidentId);
					btnSubnav_Click(btnSubnavContainment, null);
					break;
				case "3":
					status = uclRootCause.AddUpdateINCFORM_ROOT5Y(EditIncidentId);
					btnSubnav_Click(btnSubnavRootCause, null);
					break;
				case "4":
					status = uclAction.AddUpdateINCFORM_ACTION(EditIncidentId);
					btnSubnav_Click(btnSubnavAction, null);
					break;
				case "5":
					status = uclApproval.AddUpdateINCFORM_APPROVAL(EditIncidentId);
					btnSubnav_Click(btnSubnavApproval, null);
					break;
				default:
					if (IsEditContext)
					{
						btnSaveContinue_Click(sender, null);
						BuildForm();
					}
					else
						btnSaveReturn_Click(sender, null);
					break;
			}
			if (status >= 0)
			{
				string script = string.Format("alert('{0}');", "Your updates have been saved.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnSubnav_Click(object sender, EventArgs e)
		{
			//RadButton btn = (RadButton)sender;

			LinkButton btn = (LinkButton)sender;

			pnlForm.Visible =  divSubnavPage.Visible = uclContainment.Visible = uclRootCause.Visible = uclAction.Visible = uclApproval.Visible = false;
			btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
			CurrentSubnav = btn.CommandArgument;

			btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = btnSubnavRootCause.Enabled = btnSubnavContainment.Enabled = true;
			btnSubnavIncident.CssClass = btnSubnavContainment.CssClass = btnSubnavRootCause.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = "buttonLink";

			lblPageTitle.Text = "Incident";

			switch (btn.CommandArgument)
			{
				case "2":
					btnDelete.Visible = false;
					//btnSubnavContainment.Visible = false;
					lblPageTitle.Text = "Containment";
					btnSubnavContainment.Enabled = false;
					btnSubnavContainment.CssClass = "buttonLinkDisabled";
					uclContainment.Visible = divSubnavPage.Visible = true;
					uclContainment.IsEditContext = true;
					uclContainment.EditIncidentId = EditIncidentId;
					uclContainment.PopulateInitialForm();
					break;
				case "3":
					btnDelete.Visible = false;
					//btnSubnavRootCause.Visible = false;
					lblPageTitle.Text = "Root Cause";
					btnSubnavRootCause.Enabled = false;
					btnSubnavRootCause.CssClass = "buttonLinkDisabled";
					uclRootCause.Visible = divSubnavPage.Visible = true;
					uclRootCause.IsEditContext = true;
					uclRootCause.EditIncidentId = EditIncidentId;
					uclRootCause.PopulateInitialForm();
					break;
				case "4":
					btnDelete.Visible = false;
					//btnSubnavAction.Visible = false;
					lblPageTitle.Text = "Corrective Action";
					btnSubnavAction.Enabled = false;
					btnSubnavAction.CssClass = "buttonLinkDisabled";
					uclAction.Visible = divSubnavPage.Visible = true;
					uclAction.IsEditContext = true;
					uclAction.EditIncidentId = EditIncidentId;
					uclAction.PopulateInitialForm();
					break;
				case "5":
					btnDelete.Visible = false;
					//btnSubnavApproval.Visible = false;
					lblPageTitle.Text = "Approval";
					btnSubnavApproval.Enabled = false;
					btnSubnavApproval.CssClass = "buttonLinkDisabled";
					uclApproval.Visible = divSubnavPage.Visible = true;
					uclApproval.IsEditContext = true;
					uclApproval.EditIncidentId = EditIncidentId;
					uclApproval.PopulateInitialForm();
					break;
				case "0":
				default:
					lblPageTitle.Text = "Incident";
					btnDelete.Visible = true;
					btnSubnavIncident.Visible = true;
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					if (pnlForm.Visible == false)
					{
						pnlForm.Visible = true;
						BuildForm();
					}
					//RefreshPageContext();
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
			uclIncidentDetails.Clear();
			string typeString = "";

			if (accessLevel > AccessMode.View)
			{
				if (!IsEditContext)
				{
					// Add
					//btnSaveReturn.Enabled = false;
					//btnSaveReturn.Visible = (SelectedTypeId > 0);
					//btnSaveContinue.Visible = (SelectedTypeId > 0);
					if (Mode == IncidentMode.Incident)
						typeString = "Incident";
					else if (Mode == IncidentMode.Prevent)
					{
						btnSaveReturn.Visible = (SelectedTypeId > 0);
						typeString = "Recommendation";
					}
				
					//rddlIncidentType.Visible = (rddlIncidentType.Items.Count == 1) ? false : true;
					lblAddOrEditIncident.Text = "<strong>Add a New " + typeString + ":</strong>";
					
					lblIncidentType.Visible = false;
				
					btnDelete.Visible = false;
				}
				else
				{
					// Edit
					if (CurrentStep == 0)
					{
						if (Mode == IncidentMode.Incident)
							typeString = " Notification";
						else if (Mode == IncidentMode.Prevent)
							typeString = " Recommendations";
						//btnSaveContinue.Visible = true;
						btnSaveReturn.CommandArgument = "0";
					}
					else if (CurrentStep == 1)
					{
						if (Mode == IncidentMode.Incident)
							typeString = " Report";
						else if (Mode == IncidentMode.Prevent)
							typeString = " Response";
						btnSaveContinue.Visible = false;
						btnSaveReturn.CommandArgument = "1";
					}

					SelectedTypeId = 0;
					if (Mode != IncidentMode.Incident)
					{
						btnSaveReturn.Enabled = true;
						btnSaveReturn.Visible = true;
					}

					lblAddOrEditIncident.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditIncidentId, 6) + typeString + "</strong><br/>";

					rddlIncidentType.Visible = false;
					ddlIncidentLocation.Visible = false;
					mnuIncidentLocation.Visible = false;
					if (Mode == IncidentMode.Incident)
					{
						lblIncidentType.Text = "Incident Type: ";
						lblIncidentLocation.Text = "Incident Location: ";
					}
					else if (Mode == IncidentMode.Prevent)
					{
						lblIncidentType.Text = "Type: ";
						lblIncidentLocation.Text = "Location";
					}

					lblIncidentType.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId));
					lblIncidentLocation.Text += EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(EditIncidentId);
					lblIncidentType.Visible = true;
					lblIncidentLocation.Visible = true;
					btnDelete.Visible = true;
					BuildForm();
				}

				UpdateControlledQuestions();
				UpdateButtonText();

				if (CurrentStep == 1)
				{
					if (Mode == IncidentMode.Incident)
						UpdateClosedQuestions();
					else if (Mode == IncidentMode.Prevent)
						UpdateClosedQuestionsPrevent();
				}
				
				// Only plant admin and higher can view closed incidents
				//if (accessLevel < AccessMode.Plant)
				//pnlShowClosed.Visible = false;

				// Only admin and higher can delete incidents
				if (accessLevel < AccessMode.Admin)
					btnDelete.Visible = false;
			}
			else
			{
				// View only
				var displaySteps = new int[] { CurrentStep };
				uclIncidentDetails.Refresh(EditIncidentId, displaySteps);
			}

		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			accessLevel = UserContext.CheckAccess("EHS", "312");
			accessLevel = AccessMode.Admin;  // mt - temporary

			if (accessLevel >= AccessMode.Admin)
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


		protected void IncidentLocation_Select(object sender, EventArgs e)
		{
			string location = "0";
			if (sender is RadMenu)
			{
				location = mnuIncidentLocation.SelectedItem.Value;
				mnuIncidentLocation.Items[0].Text = mnuIncidentLocation.SelectedItem.Text;
			}
			else if (sender is RadSlider)
			{
				location = ddlIncidentLocation.SelectedValue;
			}
			//BuildAuditUsersDropdownList(location);
			hdnIncidentLocation.Value = location;

			SessionManager.SetIncidentLocation(Convert.ToDecimal(location));


			rddlIncidentType.Enabled = rddlIncidentType.Visible = (rddlIncidentType.Items.Count == 1) ? false : true;
			
			// need to rebuild the form
			string selectedTypeId = rddlIncidentType.SelectedValue;
			if (!string.IsNullOrEmpty(selectedTypeId))
			{
				SelectedTypeId = Convert.ToDecimal(selectedTypeId);
				IsEditContext = false;
				//BuildForm();
			}

		}
		
	}
}