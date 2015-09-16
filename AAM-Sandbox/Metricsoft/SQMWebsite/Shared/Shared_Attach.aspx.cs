using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website.Shared
{
    //public enum DocDisplayType { General, Instruction, Standards, Specification, d4, d5, d6, d7, d8, d9, Company, Supplier, Customer, d13, d14, User }
    public partial class Shared_Attach : SQMBasePage
    {
        private List<ATTACHMENT> files = new List<ATTACHMENT>();

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionManager.DocumentContext != null)
            {
                gvUploadedFiles.Visible = false;
                Bind_gvUploadedFiles();
            }
        }

        protected void lbUpload_Click(object sender, EventArgs e)
        {
            string name = "";

            if (flFileUpload.HasFile)
            {
                name = flFileUpload.FileName;
  
                Stream stream = flFileUpload.FileContent;
               // string sessionID = Session.SessionID;
                ATTACHMENT d = SQMDocumentMgr.AddAttachment(flFileUpload.FileName, tbFileDescription.Text, 0, "", SessionManager.DocumentContext.RecordType, SessionManager.DocumentContext.RecordID, SessionManager.DocumentContext.RecordStep, Session.SessionID, stream);
                if (d != null)
                {
                    Bind_gvUploadedFiles();
                    // mt - put the new document and upload status in session so that we can retrieve it (if necessary) from the calling page
                    SessionManager.ReturnObject = d;
                    SessionManager.ReturnStatus = true;
                }
                else
                {
                    SessionManager.ClearReturns();
                }

            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            lbUpload.Attributes.Add("onmouseup", "ShowModalDialog();");
        }

        protected void gvUploadedFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            decimal document_id = (decimal)e.Keys[0];
            SQMDocumentMgr.DeleteAttachment(document_id);
            Bind_gvUploadedFiles();
            // do below to signal the calling page to refresh the attachment list
            SessionManager.ReturnObject = new ATTACHMENT();
            SessionManager.ReturnStatus = true;
        }

        #endregion

        #region Helper methods

        protected string FormatFilesize(Object d)
        {
            if (d == null)
            {
                return "";
            }
            string f = WebSiteCommon.GetFileSizeReadable(long.Parse(d.ToString()));
            return f;
        }

        protected void Bind_gvUploadedFiles()
        {
            files = SQMDocumentMgr.SelectAttachmentListByRecord(SessionManager.DocumentContext.RecordType, SessionManager.DocumentContext.RecordID, SessionManager.DocumentContext.RecordStep, Session.SessionID);

            if (files != null && files.Count > 0)
            {
                gvUploadedFiles.DataSource = files;
                gvUploadedFiles.DataKeyNames = new string[] { "ATTACHMENT_ID" };
                gvUploadedFiles.DataBind();
                gvUploadedFiles.Visible = true;
            }
        }

        public void gvUploadedFiles_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {

                try
                {
                    HiddenField hfField = (HiddenField)e.Row.Cells[0].FindControl("hfFileName");
                    string ext = hfField.Value.Substring(hfField.Value.LastIndexOf('.') + 1).ToLower();
                    if (!string.IsNullOrEmpty(ext))
                    {
                        Image img = (Image)e.Row.Cells[0].FindControl("imgFileType");
                        img.ImageUrl = "~/images/filetype/icon_" + ext + ".gif";
                    }
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}