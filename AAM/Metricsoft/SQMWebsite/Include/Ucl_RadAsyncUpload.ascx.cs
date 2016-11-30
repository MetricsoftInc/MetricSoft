using System;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using SQM.Website.Classes;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	public partial class Ucl_RadAsyncUpload : System.Web.UI.UserControl
	{
       
        public delegate void AttachmentDelete(ATTACHMENT attachment);
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
            rgFiles.MasterTableView.GetColumn("DisplayTypeColumn").Visible = visible;
        }

		public void SetSizeOption(bool visible)
		{
			rgFiles.MasterTableView.GetColumn("SizeColumn").Visible = visible;
		}

		public void SetDescription(bool visible)
		{
			trAttachDesc.Visible = visible;
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
			"		row.appendChild( label );\n" +
			"		row.appendChild( input );\n" +
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
		}


        public void GetUploadedFiles(int recordType, decimal recordId)
        {
            GetUploadedFiles(recordType, recordId, "");
        }

		public void GetUploadedFiles(int recordType, decimal recordId, string recordStep)
		{
			_recordType = recordType;
			_recordId = recordId;
			
			var entities = new PSsqmEntities();

			var files = (from a in entities.ATTACHMENT
						 where a.RECORD_TYPE == recordType && a.RECORD_ID == recordId
						 orderby a.FILE_NAME
						 select new
						 {
							 AttachmentId = a.ATTACHMENT_ID,
                             RecordStep = a.RECORD_STEP,
							 FileName = a.FILE_NAME,
							 Description = a.FILE_DESC,
							 Size = a.FILE_SIZE,
							 DisplayType = a.DISPLAY_TYPE
						 }).ToList();

			if (!string.IsNullOrEmpty(recordStep))
			{
				if (recordType == 40 && recordStep == "2") // Special case to cover second page incident attachments originally entered with no recordstep
					rgFiles.DataSource = files.Where(f => (f.RecordStep == "2" || f.RecordStep == ""));
				else
					rgFiles.DataSource = files.Where(f => f.RecordStep == recordStep);
			}
			else
			{
				rgFiles.DataSource = files;
			}
			rgFiles.DataBind();

			rgFiles.Visible = (files.Count > 0);

			tbAttachDesc.Text = "";
		}

		public void GetUploadedFilesProblemCase(List<ATTACHMENT> attachList)
		{
			var files = (from a in attachList
						 orderby a.FILE_NAME
						 select new
						 {
							 AttachmentId = a.ATTACHMENT_ID,
							 FileName = a.FILE_NAME,
							 Description = a.FILE_DESC,
							 Size = a.FILE_SIZE,
							 DisplayType = a.DISPLAY_TYPE
						 }).ToList();

			rgFiles.DataSource = files;
			rgFiles.DataBind();

			rgFiles.Visible = (files.Count > 0);
		}

		public void SaveFiles()
		{
			string[] descriptions;

			if (trAttachDesc.Visible == true)
				descriptions = tbAttachDesc.Text.Split('|');
			else 
				descriptions = hfDescriptions.Value.Split('|');

			int i = 0;
			foreach (UploadedFile file in raUpload.UploadedFiles)
			{
				string description = (i < descriptions.Count()) ? descriptions[i] : "";
				decimal displayType = (file.FileName.ToLower().Contains(".jpeg") || file.FileName.ToLower().Contains(".jpg") ||
					file.FileName.ToLower().Contains(".gif") || file.FileName.ToLower().Contains(".png")) ||
					file.FileName.ToLower().Contains(".bmp") ? 1 : 0;
				SQMDocumentMgr.AddAttachment(
					file.FileName,
					description,
					displayType,
					"",
					SessionManager.DocumentContext.RecordType,
					SessionManager.DocumentContext.RecordID,
					SessionManager.DocumentContext.RecordStep,
					Session.SessionID,
					file.InputStream
					);
				i++;
			}

			// Update "display" status of existing files
			foreach (GridDataItem item in rgFiles.Items)
			{
				decimal attachmentId = Convert.ToDecimal(item.GetDataKeyValue("AttachmentId"));
				CheckBox cb = (CheckBox)item["DisplayTypeColumn"].FindControl("checkBox");
				decimal displayType = (cb.Checked) ? 1 : 0;
				SQMDocumentMgr.UpdateAttachmentDisplayType(attachmentId, displayType);
			}
		}

		protected void rgFiles_OnDeleteCommand(object source, GridCommandEventArgs e)
		{
			GridEditableItem item = (GridEditableItem)e.Item;
			decimal attachmentId = (decimal)item.GetDataKeyValue("AttachmentId");
			ATTACHMENT attach = SQMDocumentMgr.DeleteAttachment(attachmentId);

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
				if (fileName.Contains(".jpg") || fileName.Contains(".jpeg") || fileName.Contains(".gif") || fileName.Contains(".png") || fileName.Contains(".bmp"))
					cb.Visible = true;
				else
					cb.Visible = cb.Checked = false;
			}
		}

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


	}
}