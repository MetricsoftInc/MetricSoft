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
    public partial class Ucl_AttachVideoText : System.Web.UI.UserControl
    {
		public delegate void TextDelete(VIDEO_ATTACHMENT attachment);
		public event TextDelete OnAttachmentDelete;
		public event CommandClick AttachmentEvent;

		public DocumentScope staticScope 
        {
            get { return ViewState["attachScope"] == null ? new DocumentScope() : (DocumentScope)ViewState["attachScope"]; }
            set { ViewState["attachScope"] = value; }
        }

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

		public int _bodyPart
		{
			get { return ViewState["BodyPart"] == null ? 0 : (int)ViewState["BodyPart"]; }
			set { ViewState["BodyPart"] = value; }
		}

		public int _injuryType
		{
			get { return ViewState["InjuryType"] == null ? 0 : (int)ViewState["InjuryType"]; }
			set { ViewState["InjuryType"] = value; }
		}

		public string _videoType
		{
			get { return ViewState["VideoType"] == null ? "0" : (string)ViewState["VideoType"]; }
			set { ViewState["VideoType"] = value; }
		}

		public void SetAttachmentRecordStep(string step)
		{
			_recordStep = step;
		}

		public void SetViewMode(bool visible)
		{
			rgFiles.MasterTableView.GetColumn("DeleteButtonColumn").Visible = visible;
			rgFiles.Width = new System.Web.UI.WebControls.Unit("99%");
		}

		protected void Page_Load(object sender, EventArgs e)
        {

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
							 DisplayType = a.DISPLAY_TYPE,
							 Title = a.TITLE
						 }).ToList();

			rgFiles.DataSource = files;
			rgFiles.DataBind();

			rgFiles.Visible = (files.Count > 0);
		}

		protected void rgFiles_OnEditCommand(object source, GridCommandEventArgs e)
		{
			GridEditableItem item = (GridEditableItem)e.Item;
			decimal attachmentId = (decimal)item.GetDataKeyValue("VideoAttachId");
			VIDEO_ATTACHMENT attach = SQMDocumentMgr.GetVideoAttachment(attachmentId);
			tbText.Text = attach.DESCRIPTION;
			tbTimestamp.Text = attach.TITLE;
			hdnVideoAttachId.Value = attachmentId.ToString();

		}

		protected void lbVideoId_OnClick(object sender, EventArgs e)
		{
			LinkButton lb = (LinkButton)sender;
			decimal attachmentId = Convert.ToDecimal(lb.CommandArgument.ToString());
			VIDEO_ATTACHMENT attach = SQMDocumentMgr.GetVideoAttachment(attachmentId);
			tbText.Text = attach.DESCRIPTION;
			tbTimestamp.Text = attach.TITLE;
			hdnVideoAttachId.Value = attachmentId.ToString();
		}

		protected void rgFiles_OnDeleteCommand(object source, GridCommandEventArgs e)
		{
			GridEditableItem item = (GridEditableItem)e.Item;
			decimal attachmentId = (decimal)item.GetDataKeyValue("VideoAttachId");
			VIDEO_ATTACHMENT attach = SQMDocumentMgr.DeleteVideoAttachment(attachmentId);

			this.GetUploadedFiles(_recordType, _recordId);

			if (OnAttachmentDelete != null && attach != null)
			{
				OnAttachmentDelete(attach);
			}
		}

		protected void rgFiles_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = e.Item as GridDataItem;
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			//SessionManager.DocumentContext = new SQM.Shared.DocumentScope().CreateNew(1, "", staticScope.RecordType, "", staticScope.RecordID, staticScope.RecordStep, new decimal[0] {});
			//SessionManager.DocumentContext.RecordType = staticScope.RecordType;
			//SessionManager.DocumentContext.RecordID = staticScope.RecordID;
			//SessionManager.DocumentContext.RecordStep = staticScope.RecordStep;
			//uclUpload.SaveFiles();
			if (hdnVideoAttachId.Value.ToString().Equals(""))
			{
				SQMDocumentMgr.AddVideoAttachment(
					"",
					tbText.Text.ToString(),
					tbTimestamp.Text.ToString(),
					0,
					_recordType,
					_recordId,
					Session.SessionID,
					null
					);
			}
			else
			{
				decimal videoAttachId = Convert.ToDecimal(hdnVideoAttachId.Value.ToString());
				SQMDocumentMgr.UpdateVideoAttachment(
					videoAttachId,
					"",
					tbText.Text.ToString(),
					tbTimestamp.Text.ToString(),
					0,
					_recordType,
					_recordId,
					Session.SessionID,
					null
					);
			}

			this.GetUploadedFiles(_recordType, _recordId);
			tbTimestamp.Text = "";
			tbText.Text = "";
			hdnVideoAttachId.Value = "";

			if (AttachmentEvent != null)
			{
				AttachmentEvent("save");
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			tbTimestamp.Text = "";
			tbText.Text = "";
			hdnVideoAttachId.Value = "";

			if (AttachmentEvent != null)
			{
				AttachmentEvent("cancel");
			}
		}

	}
}