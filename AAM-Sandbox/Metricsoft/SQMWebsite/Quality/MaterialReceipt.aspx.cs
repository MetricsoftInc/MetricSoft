using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class MaterialReceipt : SQMBasePage
    {
        static SQMStream staticStream;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclStreamList.OnStreamClick += uclStreamList_OnStreamClick;
            //uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.SetButtonsVisible(false, false, true, true, false, false);
                uclSearchBar.SetButtonsEnabled(false, false, true, false, false, false);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.PageTitle.Text = lblMaterialReceiptTitle.Text;
            }
        }

        protected void btnStreamSearch_Click(object sender, EventArgs e)
        {
            uclStreamList.BindSuppStreamList(SQMStream.SelectSuppStreamList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, tbStreamSupp.Text, tbStreamPart.Text));
        }

        private void uclStreamList_OnStreamClick(decimal streamID)
        {
            staticStream = new SQMStream().CreateNew("").Load(streamID);
            pnlStreamSearch.Visible = false;
            uclStreamList.BindStreamRecList(staticStream.RecList);
            uclStreamHdr.BindStreamRecHdr(staticStream.Data);
            uclSearchBar.SetButtonsEnabled(false, false, true, true, false, false);
            SetupPage();
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        protected void tab_Click(string tabID, string cmdArg)
        {
            if (tabID != null)
            {
                // setup for ps_admin.js to toggle the tab active/inactive display
                SessionManager.CurrentSecondaryTab = tabID;

                switch (tabID)
                {
                    default:
                        break;
                }
            }
        }

        private void SetupPage()
        {
            pnlReceiptEdit.Visible = true;
            tbReceiptDate.Text = WebSiteCommon.FormatDateString(WebSiteCommon.LocalTime(DateTime.UtcNow, SessionManager.UserContext.TimeZoneID), false);
        }

        protected void btnCreateIssue_Click(object sender, EventArgs e)
        {
            SessionManager.ReturnObject = staticStream;
            SessionManager.ReturnStatus = true;

            Response.Redirect("/Quality/Quality_Issue.aspx");
        }

    }
}