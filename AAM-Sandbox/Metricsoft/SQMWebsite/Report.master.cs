using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Report : System.Web.UI.MasterPage
    {
        public string PageGroup = "report";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HiddenField hf = (HiddenField)this.Master.FindControl("hdCurrentActiveMenu");
            hf.Value = SessionManager.CurrentMenuItem = "lbReportsMain";
        }
        /*
        protected void lbCOA_Click(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = ((LinkButton)sender).ClientID;
            Response.Redirect(SessionManager.CurrentAdminPage = "/Report/Compliance_COA.aspx");
        }

        protected void lbFulfillment_Click(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = ((LinkButton)sender).ClientID;
            Response.Redirect(SessionManager.CurrentAdminPage = "/Report/Quality_ViewSCORModel.aspx");
        }
         */
    }
}