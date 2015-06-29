using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using Telerik.Web.UI;

namespace SQM.Website
{

    public partial class Ucl_DocMgr : System.Web.UI.UserControl
    {

        private DocumentScope staticScope
        {
            get { return ViewState["docMgrScope"] == null ? new DocumentScope() : (DocumentScope)ViewState["docMgrScope"]; }
            set { ViewState["docMgrScope"] = value; }
        }

        public Panel DocMgrPnl
        {
            get { return pnlDocMgr; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ;
        }

        public void BindDocMgr(string docScope, int recordType, decimal recordID)
        {
            staticScope = new DocumentScope().CreateNew(docScope, recordType, "", recordID, "");
            SessionManager.DocumentContext = staticScope;
            pnlDocMgr.Visible = true;
            Bind_gvUploadedFiles();
        }

        public void HideDocMgr()
        {
            pnlDocMgr.Visible = false;
        }

        protected void gvUploadedFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            decimal document_id = (decimal)e.Keys[0];
            SQMDocumentMgr.Delete(document_id);
            Bind_gvUploadedFiles();
        }

        #region selectdoc

        public int BindDocumentSelect(string docScope, int displayType, bool showLabel)
        {
            return BindDocumentSelect(docScope, displayType, showLabel, true, "");
        }

        public int BindDocumentSelect(string docScope, int displayType, bool showLabel, bool clearList, string message)
        {
            int status = 0;
           

            if (displayType > 0)
                SessionManager.DocumentContext = new DocumentScope().CreateNew(SessionManager.PrimaryCompany().COMPANY_ID, docScope, 0, "", 0, "", new decimal[1] {(decimal)displayType});
            else 
                SessionManager.DocumentContext = new DocumentScope().CreateNew(docScope, 0, "", 0, "");

            List<DOCUMENT> files = SQMDocumentMgr.SelectDocList(SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.DocumentContext);

            if (clearList)
                ddlSelectDocs.Items.Clear();

            foreach (DOCUMENT doc in files)
            {
                RadComboBoxItem item = new RadComboBoxItem(doc.FILE_DESC, doc.DOCUMENT_ID.ToString());
                item.ToolTip = doc.FILE_NAME;
               // item.Font.Size = 10;
                string[] args = doc.FILE_NAME.Split('.');
                if (args.Length > 0)
                {
                    string ext = args[args.Length - 1];
                    item.ImageUrl = "~/images/filetype/icon_" + ext.ToLower() + ".jpg";
                }
                /*
                if (++status % 2 == 0)
                {
                    item.CssClass = "rcbComboItemAlt";
                }
                */
                ddlSelectDocs.Items.Add(item);
            }
            ddlSelectDocs.Font.Size = 10;
            if (!string.IsNullOrEmpty(message))
                ddlSelectDocs.EmptyMessage = message;

            pnlSelectDocument.Visible = true;
            lblSelectDocs.Visible = showLabel;
            return status;
        }

        protected void ddlSelectDocsSelect(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox ddl = (RadComboBox)o;
          //  string cmd = "window.open('../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID=" + ddl.SelectedValue + "');";
          //  Response.Redirect("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID=" + ddl.SelectedValue);

           // ddl.PostBackUrl = "../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID=" + ddl.SelectedValue;

            //ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", cmd, true);
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
            pnlDocMgr.Visible = true;
            List<DOCUMENT> files = SQMDocumentMgr.SelectDocList(SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.DocumentContext);
        	gvUploadedFiles.DataSource = files;
			gvUploadedFiles.DataKeyNames = new string[] { "DOCUMENT_ID" };
			gvUploadedFiles.DataBind();
			gvUploadedFiles.Visible = true;
			SetGridViewDisplay(gvUploadedFiles, lblDocsListEmpty, divDocsGVScroll, 20, 0);
       }

        public void BindRadDocsList(List<DOCUMENT> docList)
        {
            rgDocsList.DataSource = docList;
            rgDocsList.DataBind();
            pnlRadDocsList.Visible = true;
        }

        public string GetFileExtension(string fileName)
        {
            string ext = "";

            string[] parts = fileName.Split('.');
            if (parts.Count() > 0)
                ext = parts[parts.Count() - 1];

            return ext;
        }

        public void gvUploadedFiles_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {

                try
                {
                    HiddenField hfField = (HiddenField)e.Row.Cells[0].FindControl("hfDisplayArea");
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblDisplayArea");
                    lbl.Text = WebSiteCommon.GetXlatValue("docDisplayType", hfField.Value);

                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfFileName");
                    string ext = hfField.Value.Substring(hfField.Value.LastIndexOf('.')+1).ToLower();
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

        public void BindDocList(List<DOCUMENT> docList)
        {
            pnlDocList.Visible = true;
            gvDocList.DataSource = docList;
            gvDocList.DataKeyNames = new string[] { "DOCUMENT_ID" };
            gvDocList.DataBind();
            SetGridViewDisplay(gvDocList, lblGVDocsListEmpty, divDocListGVScroll, 20, 0);
        }

        public void gvDocs_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblPosted");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfPostedBy");
                    lbl.Text = hf.Value;
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfPostedDate");
                    lbl.Text = lbl.Text + " " + SQMBasePage.FormatDate(WebSiteCommon.LocalTime(Convert.ToDateTime(hf.Value), SessionManager.UserContext.TimeZoneID), "d", true);  //WebSiteCommon.LocalTime(Convert.ToDateTime(hf.Value), SessionManager.UserContext.TimeZoneID).ToShortDateString();

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfDisplayArea");
                    lbl = (Label)e.Row.Cells[0].FindControl("lblDisplayArea");
                    lbl.Text = WebSiteCommon.GetXlatValue("docDisplayType", hf.Value);

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfFileName");
                    string ext = hf.Value.Substring(hf.Value.LastIndexOf('.') + 1).ToLower();
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

        public void BindDocListRepeater(List<DOCUMENT> docList)
        {
            pnlDocListRpt.Visible = true;
            rptDocList.DataSource = docList;
          //  rptDocList.DataKeyNames = new string[] { "DOCUMENT_ID" };
            rptDocList.DataBind();
            SetRepeaterDisplay(rptDocList, lblDocListRptEmpty, divDocListRptScroll, 5, (rptDocList.Items.Count), "scrollAreaLarge");
        }

        public void rptDocList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    DOCUMENT doc = (DOCUMENT)e.Item.DataItem;
                    Label lbl = (Label)e.Item.FindControl("lblPosted");
                    lbl.Text = doc.UPLOADED_BY;
                    lbl.Text = lbl.Text + " " + SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)doc.UPLOADED_DT, SessionManager.UserContext.TimeZoneID), "d", true);  //WebSiteCommon.LocalTime((DateTime)doc.UPLOADED_DT, SessionManager.UserContext.TimeZoneID).ToShortDateString();

                    lbl = (Label)e.Item.FindControl("lblDisplayArea");
                    lbl.Text = WebSiteCommon.GetXlatValue("docDisplayType", doc.DISPLAY_TYPE.ToString());
                    if (doc.RECORD_ID > 0)
                    {
                        lbl = (Label)e.Item.FindControl("lblDocReference");
                        if (doc.DOCUMENT_SCOPE == "BLI")
                        {
                            PLANT plant = SQMModelMgr.LookupPlant((decimal)doc.RECORD_ID);
                            if (plant != null)
                                lbl.Text = plant.PLANT_NAME;
                        }
                    }

                    string ext = doc.FILE_NAME.Substring(doc.FILE_NAME.LastIndexOf('.') + 1).ToLower();
                    if (!string.IsNullOrEmpty(ext))
                    {
                        Image img = (Image)e.Item.FindControl("imgFileType");
                        img.ImageUrl = "~/images/filetype/icon_" + ext + ".gif";
                    }
                }
                catch
                {
                }
            }
        }

        public void SetGridViewDisplay(GridView gv, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount)
        {
            if (gv.Rows.Count == 0)
            {
                gv.Visible = false;
                lblAlert.Visible = true;
            }
            else
            {
                gv.Visible = true;
                lblAlert.Visible = false;
                int gridRows = gridRowCount;
                if (gridRows == 0)
                    gridRows = gv.Rows.Count;
                int rowLimit = rowsToScroll;
                if (rowLimit == 0)
                    rowLimit = 12; // dfltRowsToScroll;
                if (gridRows > rowLimit && divScroll != null)
                {
                    divScroll.Attributes["class"] = "scrollArea";
                }
            }
        }

        public void SetRepeaterDisplay(Repeater rpt, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount, string className)
        {
            if (rpt.Items.Count == 0)
            {
                rpt.Visible = false;
                lblAlert.Visible = true;
            }
            else
            {
                rpt.Visible = true;
                lblAlert.Visible = false;
                int gridRows = gridRowCount;
                if (gridRows == 0)
                    gridRows = rpt.Items.Count;
                int rowLimit = rowsToScroll;
                if (rowLimit == 0)
                    rowLimit = 12; // dfltRowsToScroll;
                if (gridRows > rowLimit && divScroll != null)
                {
                    divScroll.Attributes["class"] = className;
                }
            }
        }

        #endregion
    }
}