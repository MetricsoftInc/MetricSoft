using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website
{
    public delegate void UpdateAttachment(string cmd);

    public partial class Ucl_Attach : System.Web.UI.UserControl
    {
		public event CommandClick AttachmentEvent;

        public DocumentScope staticScope 
        {
            get { return ViewState["attachScope"] == null ? new DocumentScope() : (DocumentScope)ViewState["attachScope"]; }
            set { ViewState["attachScope"] = value; }
        }

        public event UpdateAttachment OnUpdateAttachment;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if ((bool)SessionManager.ReturnStatus)
                {
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("ATTACHMENT"))
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
            staticScope = new DocumentScope().CreateNew("REC",recordType, sessionID, recordID, recordStep);
            SessionManager.DocumentContext = staticScope;
        }

        public void BindAttachments(int recordType, string sessionID, decimal recordID, string recordStep, List<ATTACHMENT> attachList)
        {
            SetAttachmentsScope(recordType, sessionID, recordID, recordStep);
            pnlManageAttachments.Visible = true;
            foreach (ATTACHMENT attach in attachList)
            {
                LinkButton lnk = new LinkButton();
                lnk.Text = attach.FILE_NAME;
                lnk.ToolTip = attach.FILE_DESC;
                lnk.CssClass = "buttonRefLink";
                lnk.Style.Add("MARGIN-LEFT", "15px");
                lnk.OnClientClick = "Popup('../Shared/SQMImageHandler.ashx?DOC=a&DOC_ID=" + attach.ATTACHMENT_ID.ToString() + "', 'newPage', 800, 600); return false;";
                //  lnk.PostBackUrl = "../Shared/SQMImageHandler.ashx?DOC=a&DOC_ID="+attach.ATTACHMENT_ID.ToString();
                pnlManageAttachments.Controls.Add(lnk);
            }
        }

        public int BindListAttachment(List<ATTACHMENT> attachList, string recordStep, int attachNum)
        {
            return BindListAttachment(attachList, recordStep, attachNum, true);
        }

        public int BindListAttachment(List<ATTACHMENT> attachList, string recordStep, int attachNum, bool scrollEnabled)
        {
            int count = 0;
            List<ATTACHMENT> tempList = new List<ATTACHMENT>();

            if (attachNum == 0)
            {
                tempList.AddRange(attachList.Where(a => a.RECORD_STEP == recordStep).ToList());
                if ((count = tempList.Count) > 0)
                {
                    if (scrollEnabled)
                    {
                        rptListAttachment.DataSource = attachList;
                        rptListAttachment.DataBind();
                        pnlListAttachment.Visible = true;
                    }
                    else
                    {
                        rptAttachmentsSmall.DataSource = attachList;
                        rptAttachmentsSmall.DataBind();
                        pnlDisplayAttachmentsSmall.Visible = true;
                    }
                }
            }
            else
            {
                if (attachNum == 1)
                    tempList.Add(attachList.Where(a => a.RECORD_STEP == recordStep).FirstOrDefault());
                else
                    tempList.Add(attachList.Where(a => a.RECORD_STEP == recordStep).LastOrDefault());

                if (tempList.Count > 0 && tempList[0] != null)
                {
                    if (scrollEnabled)
                    {
                        rptListAttachment.DataSource = tempList;
                        rptListAttachment.DataBind();
                        pnlListAttachment.Visible = true;
                    }
                    else
                    {
                        rptAttachmentsSmall.DataSource = tempList;
                        rptAttachmentsSmall.DataBind();
                        pnlDisplayAttachmentsSmall.Visible = true;
                    }
                    count = 1;
                }
            }

            if (scrollEnabled &&  count < 2)
                pnlListAttachment.Attributes.Remove("Class");

            return count;
        }

        public int BindDisplayAttachments(int recordType, decimal recordID, string recordStep, decimal displayType)
        {
            int fileCount = 0;

            try
            {
                var entities = new PSsqmEntities();
                List<ATTACHMENT> attachList = (from a in entities.ATTACHMENT
                             where
                                (a.RECORD_TYPE == recordType && a.RECORD_ID == recordID) &&  (string.IsNullOrEmpty(a.RECORD_STEP)  ||  a.RECORD_STEP == recordStep) 
                             orderby a.RECORD_TYPE, a.FILE_NAME
                             select a).ToList();


                if (displayType == 0)
                {
                    if ((fileCount=attachList.Count) > 0)
                    {
                        rptAttachments.DataSource = attachList;
                        rptAttachments.DataBind();
                    }
                }
                else
                {
                    if ((fileCount = attachList.Where(f => f.DISPLAY_TYPE == displayType).Count()) > 0)
                    {
                        rptAttachments.DataSource = attachList.Where(f => f.DISPLAY_TYPE == displayType).ToList();
                        rptAttachments.DataBind();
                    }
                }
            }

            catch (Exception ex)
            {
                ;
            }

            if (fileCount > 0)
                pnlDisplayAttachments.Visible = true;

            return fileCount;
        }

        public void rptAttachList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                string[] fileTypes = { ".jpg", ".jpeg", ".gif", ".png", "bmp" };

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
                    
                        pnlListAttachment.Attributes.Remove("Class");
                        System.Web.UI.HtmlControls.HtmlGenericControl div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divAttachment");
                        div.Attributes.Remove("Class");
                    }
                }
                catch
                { }
            }
		}

		#region attachwindow
		public void OpenManageAttachmentsWindow(int recordType, decimal recordID, string recordStep, string windowTitle, string description)
		{
			staticScope = new DocumentScope();
			staticScope.CompanyID = 1;
			staticScope.RecordType = recordType;
			staticScope.RecordID = recordID;
			staticScope.RecordStep = recordStep;

			uclUpload.SetReportOption(false);
			uclUpload.SetSizeOption(true);
			uclUpload.GetUploadedFiles(recordType, recordID, recordStep);

			winManageAttachments.Title = windowTitle;
			lblManageAttachments.Text = description;
			string script = "function f(){OpenManageAttachmentsWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			SessionManager.DocumentContext = new SQM.Shared.DocumentScope().CreateNew(1, "", staticScope.RecordType, "", staticScope.RecordID, staticScope.RecordStep, new decimal[0] {});
			SessionManager.DocumentContext.RecordType = staticScope.RecordType;
			SessionManager.DocumentContext.RecordID = staticScope.RecordID;
			SessionManager.DocumentContext.RecordStep = staticScope.RecordStep;
			uclUpload.SaveFiles();

			if (AttachmentEvent != null)
			{
				AttachmentEvent("save");
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			if (AttachmentEvent != null)
			{
				AttachmentEvent("cancel");
			}
		}
		#endregion
	}
}