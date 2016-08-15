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

		protected void Page_Init(object sender, EventArgs e)
		{
			//uclAuditForm.OnAttachmentListItemClick += OpenFileUpload;
			//uclAuditForm.OnExceptionListItemClick += AddTask;
			
			uploadText.AttachmentEvent += OnTextUpdate;

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			entities = new PSsqmEntities();

			UpdateTypes();  // this will populate any lists for edit

			bool returnFromClick = false;
			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && (sourceId.EndsWith("btnSaveContinue") || sourceId.EndsWith("btnSaveReturn") || sourceId.EndsWith("btnSave")))
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
			List<XLAT> listXLAT = SQMBasePage.SelectXLATList(new string[5] { "INJURY_PART", "INJURY_TYPE", "MEDIA_VIDEO_SOURCE", "MEDIA_VIDEO_STATUS", "MEDIA_VIDEO_TYPE" }, 1);
			List<MediaVideoData> videos = MediaVideoMgr.SelectVideoDataById(entities, EditVideoId);
			if (videos != null)
			{
				MediaVideoData videoData = videos[0];
				BusinessLocation location = new BusinessLocation().Initialize((decimal)videoData.Video.PLANT_ID);
				lblVideoSourceType.Text = ((TaskRecordType)videoData.Video.SOURCE_TYPE).ToString();
				if ((TaskRecordType)videoData.Video.SOURCE_TYPE != TaskRecordType.Media)
					lblVideoSourceType.Text += " " + videoData.Video.SOURCE_ID;
				lblVideoLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblVideoPersonName.Text = videoData.Person.LAST_NAME + ", " + videoData.Person.FIRST_NAME;
				DateTime dt = (DateTime)videoData.Video.VIDEO_DT;
				lblVideoDate.Text = dt.ToString("MM/dd/yyyy");
				dt = (DateTime)videoData.Video.INCIDENT_DT;
				lblVideoIncidentDate.Text = dt.ToString("MM/dd/yyyy");
				lblVideoInjuryType.Text = SQMBasePage.GetXLAT(listXLAT, "INJURY_TYPE", videoData.Video.INJURY_TYPES).DESCRIPTION; // for now we assume only one
				lblVideoBodyPart.Text = SQMBasePage.GetXLAT(listXLAT, "INJURY_PART", videoData.Video.BODY_PARTS).DESCRIPTION; // for now we assume only one

				tbTitle.Text = videoData.Video.TITLE;
				tbDescription.Text = videoData.Video.DESCRIPTION;
				if (ddlVideoType.Items.Count == 0)
				{
					PopulateVideoTypeDropDown();
				}
				if (videoData.Video.VIDEO_TYPE == null)
					ddlVideoType.SelectedValue = "";
				else
					ddlVideoType.SelectedValue = videoData.Video.VIDEO_TYPE;
				if (videoData.Video.VIDEO_AVAILABILITY == null)
					ddlAvailability.SelectedIndex = 0;
				else
					ddlAvailability.SelectedValue = videoData.Video.VIDEO_AVAILABILITY;
				litVideoLink.Text = "<a href='/Shared/FileHandler.ashx?DOC=v&DOC_ID=" + EditVideoId + "&FILE_NAME=" + videoData.Video.FILE_NAME + "' target='_blank'>" + Resources.LocalizedText.VideoDownload + "</a>";
				rcbStatusSelect = SQMBasePage.SetComboBoxItemsFromXLAT(rcbStatusSelect, listXLAT.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_STATUS" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbStatusSelect.Items.Insert(0, new RadComboBoxItem("Select a status", ""));
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
				if (videoData.Video.TEXT_ADDED || videoData.VideoTextList.Count > 0)
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
					ddlVideoType.Enabled = false;
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
				Response.Redirect("/Media/Media_Videos.aspx");  
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
				video.VIDEO_TYPE = ddlVideoType.SelectedValue.ToString();
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

		#endregion


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
					uploadReleases.RAVideoUpload.PostbackTriggers = new string[] { "btnSaveReturn", "btnDelete" };
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
		void PopulateVideoTypeDropDown()
		{
			List<EHSMetaData> videotype = EHSMetaDataMgr.SelectMetaDataList("MEDIA_VIDEO_TYPE");
			if (videotype != null && videotype.Count > 0)
			{
				foreach (var s in videotype)
				{
					{
						ddlVideoType.Items.Add(new ListItem(s.Text, s.Value));
					}
				}
			}
		}

		private void OnTextUpdate(string cmd)
		{
			// update the page
			List<MediaVideoData> videos = MediaVideoMgr.SelectVideoDataById(entities, EditVideoId);
			if (videos != null)
			{
				MediaVideoData videoData = videos[0];
				GetAttachments(EditVideoId, (int)MediaAttachmentType.Text, videoData.VideoTextList.Count);
				if (videoData.Video.TEXT_ADDED || videoData.VideoTextList.Count > 0)
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
			}

		}


	}
}