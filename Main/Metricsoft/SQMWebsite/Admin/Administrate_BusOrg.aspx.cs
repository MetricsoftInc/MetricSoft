using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using SQM.Shared;

namespace SQM.Website
{
    public partial class Administrate_BusOrg : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.CurrentAdminTab = "lbBusOrg";        // temporary !
            if (rblSearchBusOrg.SelectedIndex < 0)
                rblSearchBusOrg.SelectedIndex = rblSearchPlant.SelectedIndex = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
            }
            catch (Exception exp)
            {
                SQMLogger.LogException(exp);
            }

        }


        protected void lbCreateBusOrg_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_EditBusOrg.aspx");
        }

        protected void lbSearchBusOrg_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (rblSearchBusOrg.SelectedIndex == 0)
                SessionManager.BusOrgSearchCriteria = tbSearchString.Text;
            else
                SessionManager.BusOrgSearchCriteria = rblSearchBusOrg.SelectedValue;

            Response.Redirect("/Admin/Administrate_SearchRsltBusOrg.aspx");
        }

        protected void lbSearchPlant_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (rblSearchPlant.SelectedIndex == 0)
                SessionManager.PlantSearchCriteria = tbPlantSearchString.Text;
            else
                SessionManager.PlantSearchCriteria = rblSearchPlant.SelectedValue;

            Response.Redirect("/Admin/Administrate_SearchRsltPlant.aspx");
        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }

        protected void btnDfltDocs_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_DefaultDocuments.aspx");
        }
    }
}