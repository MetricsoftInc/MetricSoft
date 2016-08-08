using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using System.IO;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_AttachVideoPanel : System.Web.UI.UserControl
	{
		public event CommandClick AttachmentEvent;

		public DocumentScope staticScope
		{
			get { return ViewState["attachScope"] == null ? new DocumentScope() : (DocumentScope)ViewState["attachScope"]; }
			set { ViewState["attachScope"] = value; }
		}

		public event UpdateAttachmentVideo OnUpdateAttachment;

		public DateTime SourceDate
		{
			get { return ViewState["SourceDate"] == null ? DateTime.Now : (DateTime)ViewState["SourceDate"]; }
			set { ViewState["SourceDate"] = value; }
		}

		public int SourceType // incident/audit type - we will determine the correct list to access based on the UploadSource
		{
			get { return ViewState["SourceType"] == null ? 0 : (int)ViewState["SourceType"]; }
			set { ViewState["SourceType"] = value; }
		}

		protected int _recordType
		{
			get { return ViewState["RecordType"] == null ? 0 : (int)ViewState["RecordType"]; }
			set { ViewState["RecordType"] = value; }
		}

		protected decimal _recordId
		{
			get { return ViewState["RecordId"] == null ? 0 : (decimal)ViewState["RecordId"]; }
			set { ViewState["RecordId"] = value; }
		}

		protected string _recordStep
		{
			get { return ViewState["RecordStep"] == null ? "" : (string)ViewState["RecordStep"]; }
			set { ViewState["RecordStep"] = value; }
		}

		protected decimal _plantId
		{
			get { return ViewState["PlantId"] == null ? 0 : (decimal)ViewState["PlantId"]; }
			set { ViewState["PlantId"] = value; }
		}

		public string _bodyPart
		{
			get { return ViewState["BodyPart"] == null ? "" : (string)ViewState["BodyPart"]; }
			set { ViewState["BodyPart"] = value; }
		}

		public string _injuryType
		{
			get { return ViewState["InjuryType"] == null ? "" : (string)ViewState["InjuryType"]; }
			set { ViewState["InjuryType"] = value; }
		}

		public string _videoType
		{
			get { return ViewState["VideoType"] == null ? "0" : (string)ViewState["VideoType"]; }
			set { ViewState["VideoType"] = value; }
		}

		public bool _showCancel
		{
			get { return ViewState["ShowCancel"] == null ? false : (bool)ViewState["ShowCancel"]; }
			set { ViewState["ShowCancel"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			string script =
			"function pageLoad() {\n" +
			"	var radAsyncUpload = $find(\"" + raUpload.ClientID + "\")\n" +
			"   var hfDescriptions = document.getElementById('" + hfDescriptions.ClientID + "');\n" +
			"   if (hfDescriptions != null && hfDescriptions.value != null) {\n" +
			"      var descriptions = hfDescriptions.value.split('|');\n" +
			"	   var hfListId = document.getElementById('" + hfListId.ClientID + "');\n" +
			"      if (hfListId.value != null) {\n" +
			"	       var parentList = document.getElementById(hfListId.value);\n" +
			"	       if (parentList != null) {\n" +
			"		       var rows = parentList.childNodes;\n" +
			"		       for (i = 0; i < rows.length - 1; i++) {\n" +
			"			       var row = rows[i];\n" +
			"			       if (radAsyncUpload != null) {\n" +
			"				       inputName = radAsyncUpload.getAdditionalFieldID('TextBox');\n" +
			"				       inputType = 'text';\n" +
			"				       inputID = inputName;\n" +
			"				       input = createInput(inputType, inputID, inputName);\n" +
			"				       if (i < descriptions.length)\n" +
			"				       	input.value = descriptions[i];\n" +
			"				       label = createLabel(inputID);\n" +
			"				       br = document.createElement( 'br' );\n" +
			"				       input.onchange = inputChanged;\n" +
			"				       row.appendChild( br );\n" +
			"				       row.appendChild( label );\n" +
			"				       row.appendChild( input );\n" +
			"					}\n" +
			"				}\n" +
			"			}\n" +
			"		}\n" +
			"	}\n" +
			"}\n" +
			"function onClientFileUploaded(radAsyncUpload, args) {\n" +
			"	var row = args.get_row();\n" +
			"	if (radAsyncUpload != null) {\n" +
			"		inputName = radAsyncUpload.getAdditionalFieldID('TextBox');\n" +
			"		inputType = 'text';\n" +
			"		inputID = inputName;\n" +
			"		input = createInput(inputType, inputID, inputName);\n" +
			"		label = createLabel(inputID);\n" +
			"		br = document.createElement( 'br' );\n" +
			"		parentList = row.parentNode;\n" +
			"		document.getElementById('" + hfListId.ClientID + "').value = parentList.id;\n" +
			"		input.onchange = inputChanged;\n" +
			"		row.appendChild( br );\n" +
			"       ChangeUpdate(); \n" +
				//"		row.appendChild( label );\n" +
				//"		row.appendChild( input );\n" +
			"	}\n" +
			"}\n" +
			"function inputChanged() {\n" +
			"	var parentList = this.parentNode.parentNode;\n" +
			"	if (parentList != null) {\n" +
			"		var rows = parentList.childNodes;\n" +
			"		var descField = document.getElementById('" + hfDescriptions.ClientID + "');\n" +
			"		descField.value = '';\n" +
			"		for (i = 0; i < rows.length - 1; i++) {\n" +
			"			var textBox = rows[i].childNodes[4];\n" +
			"			descField.value += textBox.value + '|';\n" +
			"		}\n" +
			"	}\n" +
			"}\n" +
			"function createInput(inputType, inputID, inputName) {\n" +
			"	 var input = document.createElement( 'input' );\n" +
			"\n" +
			"	 input.setAttribute( 'type', inputType);\n" +
			"	 input.setAttribute( 'id', inputID );\n" +
			"	 input.setAttribute( 'class', 'descriptionInput' );\n" +
			"	 input.setAttribute( 'name', inputName );\n" +
			"\n" +
			"	 return input;\n" +
			"}\n" +
			"\n" +
			"function createLabel(forArrt) {\n" +
			"	 var label = document.createElement( 'label' );\n" +
			"\n" +
			"	 label.setAttribute( 'for', forArrt );\n" +
			"	 label.setAttribute( 'class', 'descriptionLabel' );\n" +
			"	 label.innerHTML = 'Description (optional): ';\n" +
			"\n" +
			"	 return label;\n" +
			"}\n";

			ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "script" + raUpload.ClientID, script, true);

			if (Page.IsPostBack)
			{
				if ((bool)SessionManager.ReturnStatus)
				{
					if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("ATTACHMENT_VIDEO"))
					{
						SessionManager.ClearReturns();
						if (OnUpdateAttachment != null)
						{
							OnUpdateAttachment("");
						}
					}
				}
			}
		}

		private void SetAttachmentsScope(int recordType, string sessionID, decimal recordID, string recordStep)
		{
			staticScope = new DocumentScope().CreateNew("REC", recordType, sessionID, recordID, recordStep);
			SessionManager.DocumentContext = staticScope;
		}

		public void rptAttachList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				string[] fileTypes = { ".mov", ".qt", ".wmv", ".yuv", ".m4v", ".3gp", ".3g2", ".nsv" };

				try
				{
					ATTACHMENT attachment = (ATTACHMENT)e.Item.DataItem;
					if (fileTypes.Any(attachment.FILE_NAME.ToLower().Contains) == false)
					{
						Image img = (Image)e.Item.FindControl("imgBindAttachment");
						string[] args = attachment.FILE_NAME.Split('.');
						if (args.Length > 0)
						{
							string ext = args[args.Length - 1];
							img.ImageUrl = "~/images/filetype/icon_" + ext.ToLower() + ".jpg";
							img.CssClass = "";
						}
						else
						{
							img.Visible = false;
						}

						pnlListVideo.Attributes.Remove("Class");
						System.Web.UI.HtmlControls.HtmlGenericControl div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divAttachment");
						div.Attributes.Remove("Class");
					}
				}
				catch
				{ }
			}
		}

		#region attachwindow
		public void OpenManageVideosWindow(int recordType, decimal recordID, string recordStep, decimal plantID, string windowTitle, string description, string videoType, string injuryType, string bodyPart)
		{
			OpenManageVideosWindow(recordType, recordID, recordStep, plantID, windowTitle, description, videoType, injuryType, bodyPart, PageUseMode.EditEnabled, false);
		}
		public void OpenManageVideosWindow(int recordType, decimal recordID, string recordStep, decimal plantID, string windowTitle, string description, string videoType, string injuryType, string bodyPart, PageUseMode viewMode, bool showCancel)
		{
			_recordType = recordType;
			_recordId = recordID;
			_recordStep = recordStep;
			_plantId = plantID;
			_injuryType = injuryType;
			_bodyPart = bodyPart;
			_videoType = videoType;
			_showCancel = showCancel;

			staticScope = new DocumentScope();
			staticScope.CompanyID = 1;
			staticScope.RecordType = recordType;
			staticScope.RecordID = recordID;
			staticScope.RecordStep = recordStep;

			//uclUpload.SetReportOption(false);
			//uclUpload.SetSizeOption(true);
			if (viewMode == PageUseMode.ViewOnly)
			{
				//uclUpload.SetViewMode(false);
				pnlAttachVideoBody.Visible = btnSave.Visible = false;
				rgFiles.MasterTableView.GetColumn("DeleteButtonColumn").Visible = false;
			}
			else
			{
				pnlAttachVideoBody.Visible = btnSave.Visible = true;
				dmFromDate.ShowPopupOnFocus = true;
			}

			LoadDefaults();

			GetUploadedFiles();

			tbTitle.Text = tbFileDescription.Text = "";

			lblManageVideos.Text = description;
			pnlManageAttachVideos.Visible = true;
			pnlAttachMsg.Visible = false;

			btnCancel.Visible = showCancel;
		}

		private void LoadDefaults()
		{
			//lbUpload.Attributes.Add("onmouseup", "ShowModalDialog();");
			if (ddlInjuryType.Items.Count == 0)
			{
				PopulateInjuryTypeDropDown();
				PopulateBodyPartDropDown();
				PopulateVideoTypeDropDown();
			}
			dmFromDate.SelectedDate = SourceDate;
			if (string.IsNullOrEmpty(_injuryType))
				ddlInjuryType.SelectedValue = "";
			else
				ddlInjuryType.SelectedValue = _injuryType;
			if (string.IsNullOrEmpty(_bodyPart))
				rdlBodyPart.SelectedValue = "";
			else
				rdlBodyPart.SelectedValue = _bodyPart;
			if (_videoType.Length == 0)
				ddlVideoType.SelectedValue = "";
			else
				ddlVideoType.SelectedValue = _videoType.ToString();

		}

		public void GetUploadedFiles()
		{
			var entities = new PSsqmEntities();

			var files = (from a in entities.VIDEO
						 where a.SOURCE_TYPE == _recordType && a.SOURCE_ID == _recordId
						 orderby a.FILE_NAME
						 select new
						 {
							 VideoId = a.VIDEO_ID,
							 RecordStep = a.SOURCE_STEP,
							 FileName = a.FILE_NAME,
							 Description = a.DESCRIPTION,
							 Title = a.TITLE,
							 Size = a.FILE_SIZE
						 }).ToList();

			if (!string.IsNullOrEmpty(_recordStep))
			{
				if (_recordType == 40 && _recordStep == "2") // Special case to cover second page incident attachments originally entered with no recordstep
					rgFiles.DataSource = files.Where(f => (f.RecordStep == "2" || f.RecordStep == ""));
				else
					rgFiles.DataSource = files.Where(f => f.RecordStep == _recordStep);
			}
			else
			{
				rgFiles.DataSource = files;
			}
			rgFiles.DataBind();

			if (files.Count > 0 && (_recordType == (int)TaskRecordType.Audit || _recordType == (int)TaskRecordType.HealthSafetyIncident))
			{
				pnlListVideo.Visible = true;
				rgFiles.Visible = true;
			}
			else
			{
				pnlListVideo.Visible = false;
				rgFiles.Visible = false;
			}
			//rgFiles.Visible = (files.Count > 0);
		}

		void PopulateInjuryTypeDropDown()
		{
			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");
			if (injtype != null && injtype.Count > 0)
			{

				ddlInjuryType.Items.Add(new DropDownListItem("", ""));

				foreach (var s in injtype)
				{
					{
						ddlInjuryType.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}

		void PopulateBodyPartDropDown()
		{
			bool categorize = SessionManager.GetUserSetting("EHS", "INJURYPART_CATEGORIZE") != null && SessionManager.GetUserSetting("EHS", "INJURYPART_CATEGORIZE").VALUE == "Y" ? true : false;
			List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[1] { "INJURY_PART" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
			SQMBasePage.SetCategorizedDropDownItems(rdlBodyPart, xlatList.Where(l => l.XLAT_GROUP == "INJURY_PART").ToList(), categorize);
		}

		void PopulateVideoTypeDropDown()
		{
			List<EHSMetaData> videotype = EHSMetaDataMgr.SelectMetaDataList("MEDIA_VIDEO_TYPE");
			if (videotype != null && videotype.Count > 0)
			{
				foreach (var s in videotype)
				{
					{
						ddlVideoType.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			string name = "";
			string fileType = "";

			if (string.IsNullOrEmpty(tbTitle.Text.Trim()) ||  string.IsNullOrEmpty(tbFileDescription.Text.Trim()) ||  raUpload.UploadedFiles.Count == 0)
			{
				pnlAttachMsg.Visible = true;
				lblAttachMsg.Text = Resources.LocalizedText.RequiredFieldsMustBeCompleted;
				return;
			}


			int i = 0;

			//if (flFileUpload.HasFile)
			foreach (UploadedFile file in raUpload.UploadedFiles) // there should only be 1
			{
				//name = flFileUpload.FileName;
				name = file.FileName;
				fileType = file.GetExtension();

				// first we need to create the video header so that we have the video id
				VIDEO video = MediaVideoMgr.Add(file.FileName, fileType, tbFileDescription.Text.ToString(), tbTitle.Text.ToString(), _recordType, _recordId, _recordStep, ddlInjuryType.SelectedValue.ToString(), rdlBodyPart.SelectedValue.ToString(), ddlVideoType.SelectedValue.ToString(), (DateTime)dmFromDate.SelectedDate, SourceDate, file.InputStream, _plantId);

				pnlAttachMsg.Visible = false;
				pnlListVideo.Visible = false;

				if (AttachmentEvent != null)
				{
					SessionManager.ReturnRecordID = video.VIDEO_ID;
					SessionManager.ReturnObject = "AddVideo";
					SessionManager.ReturnStatus = true;
					AttachmentEvent("save");
				}
				else
				{
					GetUploadedFiles();
					tbTitle.Text = tbFileDescription.Text = "";
				}
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			SessionManager.ReturnRecordID = 0;
			SessionManager.ReturnObject = "DisplayVideos";
			SessionManager.ReturnStatus = true;
			pnlListVideo.Visible = false;
			if (AttachmentEvent != null)
			{
				AttachmentEvent("cancel");
			}
		}
		#endregion

		public string GetFileExtension(string fileName)
		{
			string ext = "";

			string[] parts = fileName.Split('.');
			if (parts.Count() > 0)
				ext = parts[parts.Count() - 1];

			return ext;
		}

		public bool GetChecked(int displayType)
		{
			return (displayType == 1);
		}

		protected void rgFiles_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
		{
			GridEditableItem item = (GridEditableItem)e.Item;
			decimal videoId = (decimal)item.GetDataKeyValue("VideoId");
			VIDEO video = MediaVideoMgr.SelectVideoById(videoId);
			string filename = Server.MapPath(video.FILE_NAME);
			int status = MediaVideoMgr.DeleteVideo(videoId, filename);

			//			this.GetUploadedFiles();
			pnlListVideo.Visible = false;
			SessionManager.ReturnRecordID = video.VIDEO_ID;
			SessionManager.ReturnObject = "AddVideo";
			SessionManager.ReturnStatus = true;

			if (AttachmentEvent != null)
			{
				AttachmentEvent("delete");
			}

		}

		protected void rgFiles_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = e.Item as GridDataItem;
			}
		}
	}
}