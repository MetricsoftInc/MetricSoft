using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_Response : System.Web.UI.UserControl
    {
        public string ResponseInput
        {
            get { return rtResponseInput.Text; }
        }

        public void BindResponseList(List<ResponseItem> theList)
        {
            BindResponseList(theList, true, true);
        }

        public void BindResponseList(List<ResponseItem> theList, bool responseEnabled, bool attachEnabled)
        {
            pnlResponseList.Visible = true;
            pnlResponseList.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            rtResponseInput.Text = "";
            rtResponseInput.Visible = responseEnabled;
            if (responseEnabled && attachEnabled)
                uclResponseAttach.Visible = true;
            else
                uclResponseAttach.Visible = false;

            if (theList == null || theList.Count == 0)
            {
                lblResponseListEmpty.Visible = true;
            }
            else
            {
                rptResponseList.DataSource = theList;
                rptResponseList.DataBind();
            }
        }

        public void rptResponseList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    ResponseItem response = (ResponseItem)e.Item.DataItem;

                    Label lbl = (Label)e.Item.FindControl("lblPersonName_out");
                    lbl.Text = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson(response.Response.PERSON_ID, ""));
                    if (!string.IsNullOrEmpty(response.Response.REFERENCE_DATA))
                    {
                        Control parent = lbl.Parent;
                        int idx = parent.Controls.IndexOf(lbl) + 1;
                        lbl = new Label();
                        lbl.Text = response.Response.REFERENCE_DATA;
                        lbl.CssClass = "refText";
                        parent.Controls.AddAt(idx, new LiteralControl("<br />"));
                        parent.Controls.AddAt(idx+1, lbl);
                    }
                    lbl = (Label)e.Item.FindControl("lblResponseDate_out");
                    lbl.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime(response.Response.RESPONSE_DT, SessionManager.UserContext.TimeZoneID), "g", true);
                    RadTextBox rt = (RadTextBox)e.Item.FindControl("rtResponseText_out");
                    rt.Text = response.Response.RESPONSE_TEXT;

                    if (response.AttachmentList != null  &&  response.AttachmentList.Count > 0)
                    {
                        Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
                       // rt.Parent.Controls.AddAt(rt.Parent.Controls.IndexOf(rt),new LiteralControl("<br/>"));
                        rt.Parent.Controls.AddAt(rt.Parent.Controls.IndexOf(rt)+1, attch);
                        attch.BindListAttachment(response.AttachmentList, "1", 0, false);
                    }
                }
                catch
                {
                }
            }
        }

        public int SaveAttachments(ResponseItem response)
        {
            // save attachments
            int status = 0;

            if (response != null  &&  uclResponseAttach.RAUpload.UploadedFiles.Count > 0)
            {
                SessionManager.DocumentContext =
                    new SQM.Shared.DocumentScope().CreateNew(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
                                                    SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0);
                SessionManager.DocumentContext.RecordType = 111;
                SessionManager.DocumentContext.RecordID = response.Response.RESPONSE_ID;
                SessionManager.DocumentContext.RecordStep = "1";

                uclResponseAttach.SaveFiles();
               // theIssue.LoadAttachments();
            }

            return status;
        }
    }
}