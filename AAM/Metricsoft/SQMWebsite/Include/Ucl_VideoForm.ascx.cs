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
	public partial class Ucl_VideoForm : System.Web.UI.UserControl
	{
		const Int32 MaxTextLength = 4000;

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected decimal videoPlantId = 0;

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

		public decimal EditVideoId
		{
			get { return ViewState["EditVideoId"] == null ? 0 : (decimal)ViewState["EditVideoId"]; }
			set { ViewState["EditVideoId"] = value; }
		}

		public decimal InitialPlantId
		{
			get { return ViewState["InitialPlantId"] == null ? 0 : (decimal)ViewState["InitialPlantId"]; }
			set { ViewState["InitialPlantId"] = value; }
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
			entities = new PSsqmEntities();

			UpdateTypes();  // this will populate any lists for edit

			bool returnFromClick = false;
			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn")))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
				returnFromClick = true;
			}

			if (IsPostBack && EditVideoId > 0)
			{
				if (!returnFromClick)
				{
					LoadVideoInformation();
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
				//var sourceId = Page.Request[Page.postEventSourceID];

				//if ((sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn"))) || sourceId == "")
				//{
				//	UpdateControlledQuestions();
				//}
			}
		}

		#region Form

		protected void UpdateTypes()
		{
			if (!IsEditContext) // add mode?
			{
				//var auditTypeList = new List<AUDIT_TYPE>();
				//string selectString = "";
				//if (Mode == AuditMode.Audit)
				//{
				//	auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId, true);
				//	selectString = "[Select An Assessment Type]";
				//}
				//if (auditTypeList.Count > 0)
				//	auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });
				
				//rddlAuditType.DataSource = auditTypeList;
				//rddlAuditType.DataTextField = "TITLE";
				//rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
				//rddlAuditType.DataBind();
			}
		}

		public void LoadVideoInformation()
		{
			// populate all the fields
			List<XLAT> listXLAT = SQMBasePage.SelectXLATList(new string[4] { "INJURY_PART", "INJURY_TYPE", "MEDIA_VIDEO_TYPE", "MEDIA_VIDEO_STATUS" }, 1);
			List<MediaVideoData> videos = MediaVideoMgr.SelectVideoDataById(entities, EditVideoId);
			if (videos != null)
			{
				MediaVideoData videoData = videos[0];
				BusinessLocation location = new BusinessLocation().Initialize((decimal)videoData.Video.PLANT_ID);
				lblVideoSourceType.Text = ((TaskRecordType)videoData.Video.SOURCE_TYPE).ToString();
				if ((TaskRecordType)videoData.Video.SOURCE_TYPE != TaskRecordType.Media)
					lblVideoSourceType.Text += " " + videoData.Video.SOURCE_ID;
				lblVideoType.Text = videoData.Video.VIDEO_TYPE;
				lblVideoLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblVideoPersonName.Text = videoData.Person.LAST_NAME + ", " + videoData.Person.FIRST_NAME;
				DateTime dt = (DateTime)videoData.Video.VIDEO_DT;
				lblVideoDate.Text = dt.ToString("MM/dd/yyyy");
				dt = (DateTime)videoData.Video.INCIDENT_DT;
				lblVideoIncidentDate.Text = dt.ToString("MM/dd/yyyy");
				lblVideoInjuryType.Text = SQMBasePage.GetXLAT(listXLAT, "INJURY_TYPE", videoData.Video.INJURY_TYPES).DESCRIPTION; // for now we assume only one
				lblVideoBodyPart.Text = SQMBasePage.GetXLAT(listXLAT, "INJURY_PART", videoData.Video.INJURY_TYPES).DESCRIPTION; // for now we assume only one

				tbTitle.Text = videoData.Video.TITLE;
				tbDescription.Text = videoData.Video.DESCRIPTION;
				if (videoData.Video.VIDEO_AVAILABILITY == null)
					ddlAvailability.SelectedIndex = 0;
				else
					ddlAvailability.SelectedValue = videoData.Video.VIDEO_AVAILABILITY;
				litVideoLink.Text = "<a href='/Shared/FileHandler.ashx?DOC=v&DOC_ID=" + EditVideoId + "&FILE_NAME=" + videoData.Video.FILE_NAME + "' target='_blank'>" + Resources.LocalizedText.VideoDownload + "</a>";
				rcbStatusSelect = SQMBasePage.SetComboBoxItemsFromXLAT(rcbStatusSelect, listXLAT.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_STATUS" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbStatusSelect.SelectedValue = videoData.Video.VIDEO_STATUS;

				cbVideoText.Checked = videoData.Video.SPEAKER_AUDIO;

				// populate release form list
				if (videoData.Video.RELEASE_REQUIRED)
				{
					cbReleaseForms.Checked = true;
					//dvAttach.Visible = true;
					dvAttach.Style.Add("display", "block");
					// get the new async working
					GetAttachments(EditVideoId, (int)MediaAttachmentType.ReleaseForm, videoData.ReleaseFormList.Count);
				}
				else
				{
					cbReleaseForms.Checked = false;
					//dvAttach.Visible = false;
					dvAttach.Style.Add("display", "none");
				}
				uploadReleases.SetViewMode(true);
				// populate text list
				GetAttachments(EditVideoId, (int)MediaAttachmentType.Text, videoData.VideoTextList.Count);
				if (videoData.Video.TEXT_ADDED)
				{
					cbVideoText.Checked = true;
					//dvText.Visible = true;
					dvText.Style.Add("display", "block");
				}
				else
				{
					cbVideoText.Checked = false;
					//dvText.Visible = false;
					dvText.Style.Add("display", "none");
				}

				pnlAddEdit.Visible = true;
				pnlVideoHeader.Visible = true;
				divVideoForm.Visible = true;

				if (videoData.Video.SOURCE_TYPE == (int)TaskRecordType.Audit || videoData.Video.SOURCE_TYPE == (int)TaskRecordType.HealthSafetyIncident)
					btnDelete.Visible = false;
				else
					btnDelete.Visible = true;

				// if not edit mode, all fields are display only
				if (!IsEditContext)
				{
					tbTitle.Enabled = false;
					tbDescription.Enabled = false;
					ddlAvailability.Enabled = false;
					cbReleaseForms.Enabled = false;
					cbVideoText.Enabled = false;
					cbSpeakerAudio.Enabled = false;

				}
			}
		}


		void UpdateButtonText()
		{
			btnSaveReturn.Text = Resources.LocalizedText.SaveAndReturn;
			// is there anything else to do?
		}

		//void BuildFilteredUsersDropdownList()
		//{
		//	if (rddlLocation != null)
		//	{
		//		if (!string.IsNullOrEmpty(rddlLocation.SelectedValue))
		//			this.SelectedLocationId = Convert.ToDecimal(rddlLocation.SelectedValue);
		//		else
		//			this.SelectedLocationId = 0;
		//	}

		//	if (rddlFilteredUsers != null)
		//	{
		//		rddlFilteredUsers.Items.Clear();
		//		rddlFilteredUsers.Items.Add(new DropDownListItem("", ""));

		//		var locationPersonList = new List<PERSON>();
		//		if (this.SelectedLocationId > 0)
		//		{
		//			locationPersonList = EHSAuditMgr.SelectEhsPeopleAtPlant(this.SelectedLocationId);
		//		}
		//		//else
		//		//	locationPersonList = EHSAuditMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

		//		if (locationPersonList.Count > 0)
		//		{
		//			foreach (PERSON p in locationPersonList)
		//			{
		//				string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
		//				// Check for duplicate list items
		//				var ddli = rddlFilteredUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
		//				if (ddli == null)
		//					rddlFilteredUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
		//			}

		//			// If only one user, select by default
		//			if (rddlFilteredUsers.Items.Count() == 2)
		//				rddlFilteredUsers.SelectedIndex = 1;
		//		}
		//		else
		//		{
		//			rddlFilteredUsers.Items[0].Text = "[No valid users - please change location]";
		//		}
		//	}
		//}

		//void UpdateClosedQuestions()
		//{
		//	// Close checkbox
		//	int chInt = (int)EHSAuditQuestionId.CloseAudit;
		//	string chString = chInt.ToString();
		//	CheckBox closeCh = (CheckBox)pnlForm.FindControl(chString);

		//	if (closeCh != null)
		//	{
		//		// Completion date
		//		int cdInt = (int)EHSAuditQuestionId.CompletionDate;
		//		string cdString = cdInt.ToString();
		//		RadDatePicker cdFormControl = (RadDatePicker)pnlForm.FindControl(cdString);

		//		// Completed by
		//		int cbInt = (int)EHSAuditQuestionId.CompletedBy;
		//		string cbString = cbInt.ToString();
		//		RadTextBox cbFormControl = (RadTextBox)pnlForm.FindControl(cbString);

		//		if (closeCh.Checked)
		//		{
		//			// Check if audit report required fields are filled out
		//			if (AuditReportRequiredFieldsComplete() == true)
		//			{
		//				if (cbFormControl != null)
		//				{
		//					cbFormControl.Text = SessionManager.UserContext.UserName();
		//					cbFormControl.Enabled = false;
		//				}
		//				if (cdFormControl != null)
		//				{
		//					cdFormControl.SelectedDate = SessionManager.UserContext.LocalTime;
		//					cdFormControl.Enabled = false;
		//				}
		//			}
		//			else
		//			{
		//				closeCh.Checked = false;
		//				//string script = string.Format("alert('{0}');", "You must complete all required fields on this page to close the assessment.");
		//				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentIncompleteMsg);
		//				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
		//			}
		//		}
		//		else
		//		{
		//			if (cbFormControl != null)
		//			{
		//				cbFormControl.Text = "";
		//				cbFormControl.Enabled = false;
		//			}
		//			if (cdFormControl != null)
		//			{
		//				cdFormControl.Clear();
		//				cbFormControl.Enabled = false;
		//			}
		//		}
		//	}
		//}

		//protected void ProcessQuestionControlEvent(decimal questionId)
		//{
		//	//controlQuestionChanged = true;

		//	//decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
		//	//if (typeId < 1)
		//	//	return;
		//	//questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);
		//	//EHSAuditQuestion question = questions.Where(q => q.QuestionId == questionId).FirstOrDefault();
		//	//ProcessQuestionControls(question, 0);

		//	//controlQuestionChanged = false;
		//}

		//protected void UpdateControlledQuestions()
		//{
		//	//decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
		//	//if (typeId < 1)
		//	//	return;

		//	//// why do we keep getting the questions?
		//	//questions = EHSAuditMgr.SelectAuditQuestionList(typeId, 0, EditAuditId);
		//	//UpdateAnswersFromForm();

		//	//foreach (var q in questions)
		//	//{
		//	//	if (q.QuestionControls != null && q.QuestionControls.Count > 0)
		//	//		ProcessQuestionControls(q, 0);
		//	//}

		//	//UpdateButtonText(); // One last check to fix 8d button text
		//}

		//protected void ProcessQuestionControls(EHSAuditQuestion question, int depth)
		//{
		//	if (depth > 1)
		//		return;

		//	depth++;

		//	if (question.QuestionControls != null)
		//	{
		//		foreach (AUDIT_QUESTION_CONTROL control in question.QuestionControls)
		//		{
		//			Panel containerControl = (Panel)pnlForm.FindControl("Panel" + control.AUDIT_QUESTION_AFFECTED_ID);
		//			Label formLabel = (Label)pnlForm.FindControl("Label" + control.AUDIT_QUESTION_AFFECTED_ID);
		//			var formControl = pnlForm.FindControl(control.AUDIT_QUESTION_AFFECTED_ID.ToString());

		//			string answer = question.AnswerText;
		//			var triggerVal = control.TRIGGER_VALUE;
		//			bool criteriaIsMet = false;
		//			if (triggerVal.Contains("|"))
		//			{
		//				var arrTrigger = triggerVal.Split('|');
		//				criteriaIsMet = arrTrigger.Contains(answer);
		//			}
		//			else
		//			{
		//				criteriaIsMet = (answer == triggerVal);
		//			}

		//			if (criteriaIsMet)
		//			{
		//				// Check for optional secondary criteria on control question
		//				if (control.SECONDARY_QUESTION_ID != null && control.SECONDARY_TRIGGER_VALUE != null)
		//				{
		//					var secondaryControl = pnlForm.FindControl(control.SECONDARY_QUESTION_ID.ToString());

		//					if (secondaryControl is RadDropDownList)
		//					{
		//						string secondaryValue = (secondaryControl as RadDropDownList).SelectedValue;
		//						criteriaIsMet = (control.SECONDARY_TRIGGER_VALUE == secondaryValue);
		//					}
		//				}
		//			}

		//			var localControl = control;
		//			EHSAuditQuestion affectedQuestion = questions.FirstOrDefault(q => q.QuestionId == localControl.AUDIT_QUESTION_AFFECTED_ID);

		//			if (containerControl != null && affectedQuestion != null)
		//			{
		//				switch (control.ACTION)
		//				{
		//					case "Show":

		//						if (criteriaIsMet)
		//						{
		//							containerControl.Enabled = true;
		//							formLabel.ForeColor = System.Drawing.Color.Black;
		//							if (formControl is CheckBox)
		//								(formControl as CheckBox).ForeColor = System.Drawing.Color.Black;
		//							else if (formControl is RadioButtonList)
		//								(formControl as RadioButtonList).ForeColor = System.Drawing.Color.Black;
		//							else if (formControl is RadDropDownList)
		//							{
		//								(formControl as RadDropDownList).Enabled = true;
		//							}
		//						}
		//						else
		//						{
		//							var greyColor = System.Drawing.Color.FromArgb(200, 200, 200);
		//							containerControl.Enabled = false;
		//							formLabel.ForeColor = greyColor;
		//							if (formControl is CheckBox)
		//							{
		//								(formControl as CheckBox).Checked = false;
		//								(formControl as CheckBox).ForeColor = greyColor;
		//							}
		//							else if (formControl is RadioButtonList)
		//							{
		//								(formControl as RadioButtonList).SelectedValue = Resources.LocalizedText.No;
		//								(formControl as RadioButtonList).ForeColor = greyColor;
		//							}
		//							else if (formControl is RadDropDownList)
		//							{
		//								(formControl as RadDropDownList).SelectedValue = "";
		//								(formControl as RadDropDownList).Enabled = false;
		//							}
		//						}

		//						break;

		//					case "Force":
		//						if (formControl is CheckBox)
		//						{
		//							var cb = (formControl as CheckBox);
		//							if (criteriaIsMet)
		//							{
		//								cb.Checked = true;

		//								affectedQuestion.AnswerText = "Yes";

		//								// Recursively process any other controls triggered by a forced checkbox
		//								if (affectedQuestion.QuestionControls != null && affectedQuestion.QuestionControls.Count > 0)
		//									ProcessQuestionControls(affectedQuestion, depth);
		//							}
		//							else
		//							{
		//								// Only force to false if the result of a parent checkbox changing (avoids changes on form rebuilds)
		//								if (controlQuestionChanged)
		//									cb.Checked = false;
		//							}

		//							cb.Enabled = (answer != triggerVal);
		//						}
		//						else if (formControl is RadioButtonList)
		//						{
		//							var rbl = (formControl as RadioButtonList);
		//							if (criteriaIsMet)
		//							{
		//								rbl.SelectedValue = Resources.LocalizedText.Yes;
		//								affectedQuestion.AnswerText = "Yes";

		//								// Recursively process any other controls triggered by a forced checkbox
		//								if (affectedQuestion.QuestionControls != null && affectedQuestion.QuestionControls.Count > 0)
		//									ProcessQuestionControls(affectedQuestion, depth);
		//							}
		//							else
		//							{
		//								// Only force to false if the result of a parent checkbox changing (avoids changes on form rebuilds)
		//								if (controlQuestionChanged)
		//									rbl.SelectedValue = Resources.LocalizedText.No;
		//							}

		//							rbl.Enabled = (answer != triggerVal);
		//						}

		//						break;
		//				}
		//			}
		//		}
		//	}
		//}

		//void rblp_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	decimal typeId = (IsEditContext) ? EditAuditTypeId : SelectedTypeId;
		//	if (typeId < 1)
		//		return;
		//	// add the logic to recalculate the percentages
		//	UpdateAnswersFromForm();
		//	CalculatePercentages(questions);
		//}

		//protected RadComboBox CreateReferencesDropdown(string standard)
		//{
		//	List<STANDARDS_REFERENCES> srList = SQMStandardsReferencesMgr.SelectReferencesByStandard("IMS");
		//	var rcb = new RadComboBox();
		//	rcb.Items.Add(new RadComboBoxItem(""));
		//	foreach (var s in srList)
		//	{
		//		string combined = s.SECTION + " - " + s.DESCRIPTION;
		//		rcb.Items.Add(new RadComboBoxItem(combined, combined));
		//	}
		//	return rcb;
		//}

		//protected void AddToolTip(Panel container, EHSAuditQuestion question)
		//{
		//	var imgHelp = new System.Web.UI.WebControls.Image()
		//	{
		//		ID = "help_" + question.QuestionId.ToString(),
		//		ImageUrl = "/images/ico-question.png",
		//	};

		//	var rttHelp = new RadToolTip()
		//	{
		//		Text = "<div style=\"font-size: 11px; line-height: 1.5em;\" data-html=\"true\" >" + question.HelpText + "</div>",
		//		TargetControlID = imgHelp.ID,
		//		IsClientID = false,
		//		RelativeTo = ToolTipRelativeDisplay.Element,
		//		Width = 400,
		//		Height = 200,
		//		Animation = ToolTipAnimation.Fade,
		//		Position = ToolTipPosition.MiddleRight,
		//		ContentScrolling = ToolTipScrolling.Auto,
		//		Skin = "Metro",
		//		HideEvent = ToolTipHideEvent.LeaveTargetAndToolTip
		//	};
		//	pnlForm.Controls.Add(new LiteralControl("<span style=\"float: right;\">"));
		//	container.Controls.Add(imgHelp);
		//	pnlForm.Controls.Add(new LiteralControl("</span>"));
		//	container.Controls.Add(rttHelp);
		//}

		//public void CheckForSingleType()
		//{
		//	if (rddlAuditType.Items.Count == 1)
		//	{
		//		string selectedTypeId = rddlAuditType.Items[0].Value;
		//		if (!string.IsNullOrEmpty(selectedTypeId))
		//		{
		//			SelectedTypeId = Convert.ToDecimal(selectedTypeId);
		//			IsEditContext = false;
		//			BuildForm();
		//		}
		//	}
		//}

		#endregion


		#region Form Events

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				//if there are additional validations, put them here
				Save(true);
			}
			else
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if (EditVideoId > 0)
			{
				btnSaveReturn.Visible = false;
				//btnSaveContinue.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				VIDEO video = MediaVideoMgr.SelectVideoById(EditVideoId);
				string filename = Server.MapPath(video.FILE_NAME);
				int delStatus = MediaVideoMgr.DeleteVideo(EditVideoId, filename);
				// delete the task
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? Resources.LocalizedText.VideoDeleted : Resources.LocalizedText.VideoErrorDeleting;
				lblResults.Text += "</div>";
			}
		}

		protected void Save(bool shouldReturn)
		{
            VIDEO theVideo = null;
			decimal videoId = 0;
			string result = "<h3>EHS Assessment " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			// Edit context
			videoId = EditVideoId;
			if (videoId > 0)
			{
				theVideo = UpdateVideo(videoId);
				SaveAttachments(videoId);
			}


			if (shouldReturn == true)
			{
				divForm.Visible = false;

				pnlAddEdit.Visible = false;
				btnSaveReturn.Visible = false;

				RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
				if (rcbWarnNavigate != null)
					rcbWarnNavigate.Visible = false;

				lblResults.Visible = true;
			}
			else
			{
				EditVideoId = videoId;
				SessionManager.ReturnRecordID = videoId;
				CurrentStep = 0;
				IsEditContext = true;
				Visible = true;

				// and send a message that the field need to be entered
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsIndicatedMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}

			if (shouldReturn)
			{
				SessionManager.ReturnStatus = false;
				SessionManager.ReturnObject = "DisplayVideos";
				Response.Redirect("/Media/Media_Videos.aspx");  // mt - temporary
			}

		}

		#endregion


		#region Save Methods

		protected VIDEO UpdateVideo(decimal videoId)
		{
			// we are not going to let them update any of this info.
			VIDEO video = (from i in entities.VIDEO where i.VIDEO_ID == videoId select i).FirstOrDefault();

			if (video != null)
			{
				video.TITLE = tbTitle.Text.ToString();
				video.DESCRIPTION = tbDescription.Text.ToString();
				video.VIDEO_AVAILABILITY = ddlAvailability.SelectedValue.ToString();
				video.VIDEO_STATUS = rcbStatusSelect.SelectedValue.ToString();
				video.RELEASE_REQUIRED = cbReleaseForms.Checked;
				video.TEXT_ADDED = cbVideoText.Checked;
				video.SPEAKER_AUDIO = cbSpeakerAudio.Checked;
				entities.SaveChanges();
			}

			return video;

		}

		protected void SaveAttachments(decimal videoId)
		{
			if (uploadReleases != null)
			{
				uploadReleases.SaveFiles((int)MediaAttachmentType.ReleaseForm, videoId);
			}
			//if (uploader != null)
			//{
			//	string recordStep = (this.CurrentStep + 1).ToString();

			//	// Add files to database
			//	SessionManager.DocumentContext = new DocumentScope().CreateNew(
			//		SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
			//		SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
			//		);
			//	SessionManager.DocumentContext.RecordType = (int)TaskRecordType.Media;
			//	SessionManager.DocumentContext.RecordID = videoId;
			//	SessionManager.DocumentContext.RecordStep = recordStep;
			//	uploader.SaveFiles();
			//}
		}

		//protected void GoToNextStep(decimal auditId)
		//{
		//	// Go to next step (report)
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "Report";
		//	SessionManager.ReturnRecordID = auditId;
		//}

		//protected void ShowAuditDetails(decimal auditId, string result)
		//{
		//	rddlAuditType.SelectedIndex = 0;

		//	// Display audit details control
		//	btnDelete.Visible = false;
		//	lblResults.Text = result.ToString();
		//	uclAuditDetails.Visible = true;
		//	var displaySteps = new int[] { CurrentStep };
		//	uclAuditDetails.Refresh(auditId, displaySteps);
		//}

		#endregion


		//public void ClearControls()
		//{
		//	pnlForm.Controls.Clear();
		//}

		protected void RefreshPageContext()
		{
			string typeString = " " + Resources.LocalizedText.Video;

			bool createVideoAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
			btnDelete.Visible = createVideoAccess;

			if (EditVideoId > 0)
			{
				if (UserContext.GetMaxScopePrivilege(SysScope.media) < SysPriv.view && CurrentStep == 0)
				{
					// Edit
					btnSaveReturn.CommandArgument = "0";
					SelectedTypeId = 0;
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;

					lblAddOrEditVideo.Text = "<strong>" + Resources.LocalizedText.Editing + " " + WebSiteCommon.FormatID(EditVideoId, 6) + typeString + "</strong><br/>";
					LoadVideoInformation();
					UpdateButtonText();
				}
				else
				{
					// View only
					SelectedTypeId = 0;
					btnSaveReturn.Enabled = false;
					btnSaveReturn.Visible = false;

					lblAddOrEditVideo.Text = "<strong>" + WebSiteCommon.FormatID(EditVideoId, 6) + typeString + "</strong><br/>";

					LoadVideoInformation();
					var displaySteps = new int[] { CurrentStep };
				}
			}

		}

		//List<decimal> SelectPlantIdsByAccessLevel()
		//{
		//	List<decimal> plantIdList = new List<decimal>();

		//	if (UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.system))
		//	{
		//		plantIdList = EHSAuditMgr.SelectPlantIdsByCompanyId(companyId);
		//	}
		//	else
		//	{
		//		plantIdList = SessionManager.UserContext.PlantAccessList;
		//		if (plantIdList == null || plantIdList.Count == 0)
		//		{
		//			plantIdList.Add(SessionManager.UserContext.HRLocation.Plant.PLANT_ID);
		//		}
		//	}

		//	return plantIdList;
		//}

		private void GetAttachments(decimal videoId, int recordType, int count)
		{
			switch (recordType)
			{
				case (int)MediaAttachmentType.ReleaseForm:
					uploadReleases.RAUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnDelete" };
					int px = 128;
					uploadReleases.GetUploadedFiles(recordType, videoId);
					break;
				case (int)MediaAttachmentType.Text:
					uploadText.GetUploadedFiles(recordType, videoId);
					break;
			}
			//uploader.SetAttachmentRecordStep("1");
			//// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
			//uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete" };

			////int attCnt = ;
			//int px = 128;

			//if (attCnt > 0)
			//{
			//	px = px + (attCnt * 30) + 35;
			//	uploader.GetUploadedFiles(40, incidentId, "");

			//}

			//// Set the html Div height based on number of attachments to be displayed in the grid:
			//dvAttachLbl.Style.Add("height", px.ToString() + "px !important");
			//dvAttach.Style.Add("height", px.ToString() + "px !important");
		}


	}
}