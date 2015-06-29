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
    public partial class Shared_Upload : SQMBasePage
    {
        private List<DOCUMENT> files = new List<DOCUMENT>();

        #region Event Handlers
       
        protected void Page_Load(object sender, EventArgs e)
        {
            gvUploadedFiles.Visible = false;
            Bind_gvUploadedFiles();
        }

        protected void lbUpload_Click(object sender, EventArgs e)
        {
            string name = "";

            if (flFileUpload.HasFile)
            {
                name = flFileUpload.FileName;

                decimal? display_type = null;
                if (SessionManager.DocumentContext.Scope == "USR")
                    display_type = 15;
                else
                    display_type = Convert.ToDecimal(ddlDisplayType.SelectedValue);

                Stream stream = flFileUpload.FileContent;
                DOCUMENT d = SQMDocumentMgr.Add(flFileUpload.FileName, tbFileDescription.Text, display_type, SessionManager.DocumentContext.Scope, SessionManager.DocumentContext.RecordID, stream);
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
            if (ddlDisplayType.Items.Count == 0)
            {
                ddlDisplayType.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("docDisplayType"));
                foreach (ListItem item in ddlDisplayType.Items)
                {
                    switch (SessionManager.DocumentContext.Scope)
                    {
                        case "USR":
                        case "BLI":
                            if (item.Value != "15")
                                item.Enabled = false;
                            break;
                        case "EHS":
                        case "SQM":
                            if (item.Value != "2" &&  item.Value != "10")
                                item.Enabled = false;
                            break;
                        case "SYS":
                            if (item.Value != "10" && item.Value != "31" && item.Value != "32")
                                item.Enabled = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void gvUploadedFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            decimal document_id = (decimal)e.Keys[0];
            SQMDocumentMgr.Delete(document_id);
            Bind_gvUploadedFiles();
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
            files = null;

            switch (SessionManager.DocumentContext.Scope)
            {
                case "USR":
                    files = SQMDocumentMgr.SelectDocListByOwner(SessionManager.UserContext.Person.PERSON_ID, 15);
                    break;
                default:
                    files = SQMDocumentMgr.SelectDocList(SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.DocumentContext);
                    break;
            }

            if (files != null  &&  files.Count > 0)
            {
                gvUploadedFiles.DataSource = files;
                gvUploadedFiles.DataKeyNames = new string[] { "DOCUMENT_ID" };
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