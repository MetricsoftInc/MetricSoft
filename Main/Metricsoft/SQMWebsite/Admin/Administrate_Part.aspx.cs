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
    public partial class Administrate_Part : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (rblSearchPart.SelectedIndex < 0)
                rblSearchPart.SelectedIndex = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
            if ((bool)SessionManager.ReturnStatus)
            {
                vw_CustPlantPart plantPart = (vw_CustPlantPart)SessionManager.ReturnObject;
                SessionManager.Part = SQMModelMgr.LookupPart(entities, plantPart.PART_ID, "", plantPart.CUST_COMPANY_ID, false);
                SessionManager.PartSearchCriteria = (SessionManager.Part.PART_NUM + "~" + rblSearchPart.SelectedValue);
                SessionManager.ReturnObject = null;
                SessionManager.ReturnStatus = false;
                Response.Redirect("/Admin/Administrate_ViewPart.aspx");
            }
        }

        protected void lbSearchPart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbSearchString.Text))
                SessionManager.PartSearchCriteria = "%";
            else
                SessionManager.PartSearchCriteria = tbSearchString.Text;

            SessionManager.PartSearchCriteria += ("~" + rblSearchPart.SelectedValue);
            Response.Redirect("/Admin/Administrate_SearchRsltPart.aspx");
        }

        protected void lbCreatePart_Click(object sender, EventArgs e)
        {
        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }
    }
}