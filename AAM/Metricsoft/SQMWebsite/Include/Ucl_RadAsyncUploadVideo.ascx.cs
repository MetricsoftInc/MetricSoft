using System;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using SQM.Website.Classes;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	public partial class Ucl_RadAsyncUploadVideo : System.Web.UI.UserControl
	{
       
        public delegate void AttachmentDelete(VIDEO_ATTACHMENT attachment);
        public event AttachmentDelete OnAttachmentDelete;

		public RadAsyncUpload RAUpload
		{
			get { return raUpload; }
		}

		public void SetAttachmentRecordStep(string step)
		{
			_recordStep = step;
		}

        public void SetViewMode(bool visible)
        {
			hfMode.Value = visible.ToString();
			this.raUpload.Visible = visible;
			tdUploadImg.Visible = visible;
			rgFiles.MasterTableView.GetColumn("SizeColumn").Visible = visible;
			rgFiles.MasterTableView.GetColumn("DeleteButtonColumn").Visible = visible;
            rgFiles.Width = new System.Web.UI.WebControls.Unit("99%");
        }

        public void SetReportOption(bool visible)
        {
            rgFiles.MasterTableView.GetColumn(" DisplayTypeColumn").Visible = visible;
        }

		public void SetSizeOption(bool visible)
		{
			rgFiles.MasterTableView.GetColumn("SizeColumn").Visible = visible;
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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string script =
			"function pageLoad() {\n" +
			"	var radAsyncUploadVideo = $find(\"" + raUpload.ClientID + "\")\n" +
			"   var hfDescriptionsVideo = document.getElementById('" + hfDescriptions.ClientID + "');\n" +
			"   if (hfDescriptionsVideo != null && hfDescriptionsVideo.value != null) {\n" +
			"      var descriptionsVideo = hfDescriptionsVideo.value.split('|');\n" +
			"	   var hfListIdVideo = document.getElementById('" + hfListId.ClientID + "');\n" +
			"      if (hfListIdVideo.value != null) {\n" +
			"	       var parentListVideo = document.getElementById(hfListId.value);\n" +
			"	       if (parentListVideo != null) {\n" +
			"		       var rows = parentListVideo.childNodes;\n" +
			"		       for (i = 0; i < rows.length - 1; i++) {\n" +
			"			       var row = rows[i];\n" +
			"			       if (radAsyncUploadVideo != null) {\n" +
			"				       inputName = radAsyncUploadVideo.getAdditionalFieldID('TextBox');\n" +
			"				       inputType = 'text';\n" +
			"				       inputID = inputNameVideo;\n" +
			"				       input = createInputVideo(inputType, inputID, inputName);\n" +
			"				       if (i < descriptionsVideo.length)\n" +
			"				       	input.value = descriptionsVideo[i];\n" +
			"				       label = createLabelVideo(inputID);\n" +
			"				       br = document.createElement( 'br' );\n" +
			"				       input.onchange = inputChangedVideo;\n" +
			"				       row.appendChild( br );\n" +
			"				       row.appendChild( label );\n" +
			"				       row.appendChild( input );\n" +
			"					}\n" +
			"				}\n" +
			"			}\n" +
			"		}\n" +
			"	}\n" +
			"}\n" +
			"function onClientFileUploadedVideo(radAsyncUpload, args) {\n" +
			"	var row = args.get_row();\n" +
			"	if (radAsyncUpload != null) {\n" +
			"		inputName = radAsyncUpload.getAdditionalFieldID('TextBox');\n" +
			"		inputType = 'text';\n" +
			"		inputID = inputName;\n" +
			"		input = createInputVideo(inputType, inputID, inputName);\n" +
			"		label = createLabelVideo(inputID);\n" +
			"		br = document.createElement( 'br' );\n" +
			"		parentList = row.parentNode;\n" +
			"		document.getElementById('" + hfListId.ClientID + "').value = parentList.id;\n" +
			"		input.onchange = inputChangedVideo;\n" +
			"		row.appendChild( br );\n" +
			"		row.appendChild( label );\n" +
			"		row.appendChild( input );\n" +
			"	}\n" +
			"}\n" +
			"function inputChangedVideo() {\n" +
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
			"function createInputVideo(inputType, inputID, inputName) {\n" +
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
			"function createLabelVideo(forArrt) {\n" +
			"	 var label = document.createElement( 'label' );\n" +
			"\n" +
			"	 label.setAttribute( 'for', forArrt );\n" +
			"	 label.setAttribute( 'class', 'descriptionLabel' );\n" +
			"	 label.innerHTML = 'Description (optional): ';\n" +
			"\n" +
			"	 return label;\n" +
			"}\n";

			ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "script" + raUpload.ClientID, script, true);
		}

		public void GetUploadedFiles(int recordType, decimal recordId)
		{
			_recordType = recordType;
			_recordId = recordId;
			
			var entities = new PSsqmEntities();

			var files = (from a in entities.VIDEO_ATTACHMENT
						 where a.RECORD_TYPE == recordType && a.VIDEO_ID == recordId
						 orderby a.FILE_NAME
						 select new
						 {
							 VideoAttachId = a.VIDEO_ATTACH_ID,
							 VideoId = a.VIDEO_ID,
							 FileName = a.FILE_NAME,
							 Description = a.DESCRIPTION,
							 Size = a.FILE_SIZE,
							 DisplayType = a.DISPLAY_TYPE
						 }).ToList();

			rgFiles.DataSource = files;
			rgFiles.DataBind();

			rgFiles.Visible = (files.Count > 0);
		}

		public void SaveFiles(int recordType, decimal recordId)
		{
			string[] descriptions = hfDescriptions.Value.Split('|');

			int i = 0;
			foreach (UploadedFile file in raUpload.UploadedFiles)
			{
				string description = (i < descriptions.Count()) ? descriptions[i] : "";
				decimal displayType = (file.FileName.ToLower().Contains(".jpeg") || file.FileName.ToLower().Contains(".jpg") ||
					file.FileName.ToLower().Contains(".gif") || file.FileName.ToLower().Contains(".png")) ||
					file.FileName.ToLower().Contains(".bmp") ? 1 : 0;
				switch (recordType)
				{
					case (int)MediaAttachmentType.ReleaseForm:
						SQMDocumentMgr.AddVideoAttachment(
							file.FileName,
							description,
							"",
							displayType,
							recordType,
							recordId,
							Session.SessionID,
							file.InputStream
							);
						break;
					default: // text entries
						SQMDocumentMgr.AddVideoAttachment(
							file.FileName,
							description,
							"",
							displayType,
							recordType,
							recordId,
							Session.SessionID,
							file.InputStream
							);
						break;
				}
				i++;
			}

			// Update "display" status of existing files
			foreach (GridDataItem item in rgFiles.Items)
			{
				decimal attachmentId = Convert.ToDecimal(item.GetDataKeyValue("VideoAttachId"));
				CheckBox cb = (CheckBox)item["DisplayTypeColumn"].FindControl("checkBox");
				decimal displayType = (cb.Checked) ? 1 : 0;
				SQMDocumentMgr.UpdateAttachmentDisplayType(attachmentId, displayType);
			}
		}

		protected void rgFiles_OnDeleteCommand(object source, GridCommandEventArgs e)
		{
			GridEditableItem item = (GridEditableItem)e.Item;
			decimal attachmentId = (decimal)item.GetDataKeyValue("VideoAttachId");
			VIDEO_ATTACHMENT attach = SQMDocumentMgr.DeleteVideoAttachment(attachmentId);

			this.GetUploadedFiles(_recordType, _recordId);

            if (OnAttachmentDelete != null  &&  attach != null)
            {
                OnAttachmentDelete(attach);
            }
		} 

		protected void rgFiles_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = e.Item as GridDataItem;
				string fileName = ((Literal)dataItem["FileNameColumn"].FindControl("ltrFileName")).Text.ToLower();

				CheckBox cb = ((CheckBox)dataItem["DisplayTypeColumn"].FindControl("checkBox"));
				if (fileName.Contains(".mov") || fileName.Contains(".qt") || fileName.Contains(".wmv") || fileName.Contains(".yuv") || fileName.Contains(".m4v")
					 || fileName.Contains(".3gp") || fileName.Contains(".3g2") || fileName.Contains(".nsv"))
					cb.Visible = true;
				else
					cb.Visible = cb.Checked = false;
			}
		}

		public string GetVideoFileExtension(string fileName)
		{
			string ext = "";

			string[] parts = fileName.Split('.');
			if (parts.Count() > 0)
				ext = parts[parts.Count() - 1];

			return ext;
		}

		public bool GetVideoChecked(int displayType)
		{
			return (displayType == 1);
		}


	}
}