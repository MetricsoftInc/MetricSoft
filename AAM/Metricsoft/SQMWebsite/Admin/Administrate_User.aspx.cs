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
    public partial class Administrate_User : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (rblSearchUser.SelectedIndex < 0)
                rblSearchUser.SelectedIndex = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
            // mt - testing the page access scheme ...
            lbCreateUser.Enabled = UserContext.CheckAccess("admin", "user", "add");
            lbUploadData.Enabled = UserContext.CheckAccess("admin", "user", "upload");
            lbSearchUser.Enabled = UserContext.CheckAccess("admin", "user", "search");
            if (!UserContext.CheckAccess("admin", "user", "search") && !lbUploadData.Enabled)
                lbMyUser_Click(sender, null);
        }

        protected void lbMyUser_Click(object sender, EventArgs e)
        {
            SessionManager.SelectedUser = SessionManager.UserContext.Person;
            Response.Redirect("/Admin/Administrate_ViewUser.aspx");
        }

        protected void lbSearchUser_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbSearchString.Text))
                SessionManager.SearchCriteria = "%";
            else
                SessionManager.SearchCriteria = tbSearchString.Text;

            SessionManager.SearchCriteria += ("~" + rblSearchUser.SelectedValue);

            Response.Redirect("/Admin/Administrate_SearchRsltUser.aspx");
        }

        protected void lbCreateUser_Click(object sender, EventArgs e)
        {
            SessionManager.SelectedUser = null;
            Response.Redirect("/Admin/Administrate_ViewUser.aspx");
        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }
    }
}