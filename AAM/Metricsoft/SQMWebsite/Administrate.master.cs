using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate : System.Web.UI.MasterPage
    {
        public string PageGroup = "admin";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            /*
            hdCurrentActiveTab.Value = SessionManager.CurrentAdminTab;

            lbPart.Parent.Visible = SessionManager.IsActivePrimaryCompany();
            
            lbUser.Parent.Visible = UserContext.CheckAccess("CQM", "admin", "101") == AccessMode.None ? false : true;
            lbGlobalSettings.Parent.Visible = UserContext.CheckAccess("CQM", "admin", "102") == AccessMode.None ? false : true;
            lbBusOrg.Parent.Visible = UserContext.CheckAccess("CQM", "admin", "103") == AccessMode.None ? false : true;
            lbUpload.Parent.Visible = (!SessionManager.IsActivePrimaryCompany() || UserContext.CheckAccess("CQM", "admin", "107") == AccessMode.None) ? false : true;
          */
        }

        protected void Page_Load(object sender, EventArgs e)
        {
      
        }

        protected void lbMenu_Click(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = ((LinkButton)sender).ClientID;
            switch (((LinkButton)sender).CommandArgument)
            {
                case "101":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewUser.aspx");
                    break;
                case "102":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_GlobalSettings.aspx");
                    break;
                case "103":
                    SessionManager.BusOrgSearchCriteria = "";
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewBusOrg.aspx");
                    break;
                case "104":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewPart.aspx");
                    break;
                case "107":
                    Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_FileUpload.aspx");
                    break;
            }
        }

    }
}