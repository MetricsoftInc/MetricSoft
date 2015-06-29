using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class EHS : System.Web.UI.MasterPage
    {
        public string PageGroup = "ehs";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HiddenField hf = (HiddenField)this.Master.FindControl("hdCurrentActiveMenu");
            hf.Value = SessionManager.CurrentMenuItem = "lbEHSMain";

           // lbResources.Visible = UserContext.CheckAccess("EHS", "admin", "301") == AccessMode.None ? false : true;
          //  lbDocuments.Visible = UserContext.CheckAccess("EHS", "admin", "302") == AccessMode.None ? false : true;
        }

        protected void Page_Load()
        {
            //RadMenu menu = (RadMenu)this.Master.FindControl("RadMenu1");
            //menu.Items[3].Selected = true;
            
           
        }

        protected void lbMenu_Click(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = ((LinkButton)sender).ClientID;
            SessionManager.CurrentSecondaryTab = "";
            switch (((LinkButton)sender).CommandArgument)
            {
                case "301":
                    Response.Redirect("/EHS/EHS_Resources.aspx");
                    break;
                case "302":
                    Response.Redirect("/Admin/Administrate_DefaultDocuments.aspx");
                    break;
                case "303":
                    Response.Redirect("/EHS/EHS_Incidents_New.aspx");
                    break;
                case "304":
                    Response.Redirect("/EHS/EHS_MetricInput.aspx");
                    break;
                case "305":
                    Response.Redirect("/EHS/EHS_Incidents_Reports.aspx");
                    break;
            }
        }

    }
}
