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
    public partial class Administrate_Plant : SQMBasePage
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
        }

        protected void lbSearchPlant_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (string.IsNullOrEmpty(btn.CommandArgument))
            {
                SessionManager.PlantSearchCriteria = (string)tbSearchString.Text;
            }
            else
            {
                SessionManager.PlantSearchCriteria = btn.CommandArgument;
            }
            Response.Redirect("/Admin/Administrate_SearchRsltPlant.aspx");
        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }
    }
}