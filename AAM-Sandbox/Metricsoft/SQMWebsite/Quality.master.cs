using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Quality : System.Web.UI.MasterPage
    {
        public string PageGroup = "quality";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HiddenField hf = (HiddenField)this.Master.FindControl("hdCurrentActiveMenu");
            hf.Value = SessionManager.CurrentMenuItem = "lbQualityMain";
            /*
            lbCtlPlan.Visible = UserContext.CheckAccess("SQM", "admin", "203") == AccessMode.None ? false : true;
            lbMaterialReceipt.Visible = UserContext.CheckAccess("SQM", "qual", "204") == AccessMode.None ? false : true;
            lbNCOccur.Visible = UserContext.CheckAccess("SQM", "qual", "205") == AccessMode.None ? false : true;
            */
        }

        protected void lbMenu_Click(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = ((LinkButton)sender).ClientID;
            SessionManager.CurrentSecondaryTab = "";
            switch (((LinkButton)sender).CommandArgument)
            {
                case "201":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Quality/Quality_Resources.aspx");
                    break;
                case "203":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Quality/Quality_ViewCtlPlan.aspx");
                    break;
                case "204":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Quality/MaterialReceipt.aspx");
                    break;
                case "205":
                    Response.Redirect("/Quality/Quality_Issue.aspx");
                    break;
            }
        }

    }
}