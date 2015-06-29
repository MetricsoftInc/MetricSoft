using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_SearchRsltUser : SQMBasePage
    {
        static List<PERSON> userList;
        static List<BUSINESS_ORG> busorgList;
        static string[] args;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.SearchCriteria != null)
                {
                    busorgList = SQMModelMgr.SelectBusOrgList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, 0, false);
                    args = SessionManager.SearchCriteria.ToString().Split('~');
                    SessionManager.SearchCriteria = "";
                }
                DoUserSearch();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
            tbSearchString.Text = args[0];
        }


        private void DoUserSearch()
        {
            bool activeOnly = false;
            if (args.Length > 1 && args[1] == "A")
                activeOnly = true;

            userList = SQMModelMgr.SearchPersonList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, args[0], activeOnly);
            GridView gv = (GridView)hfBase.FindControl("gvUserList");
            gv.DataSource = userList;
            gv.DataBind();
            SetGridViewDisplay(gv, (Label)hfBase.FindControl("lblUserListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divGVScroll"), 0);
        }

        public void gvUserList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();
                try
                {
                    try
                    {
                        hfField = (HiddenField)e.Row.Cells[0].FindControl("hfBusOrg_out");
                        decimal busorgID = Convert.ToDecimal(hfField.Value);
                        if (busorgID > 0)
                        {
                            BUSINESS_ORG busOrg = busorgList.FirstOrDefault(b => (b.BUS_ORG_ID == busorgID));
                            if (busOrg != null)
                            {
                                lbl = (Label)e.Row.Cells[0].FindControl("lblBusOrg_out");
                                lbl.Text = busOrg.ORG_NAME;
                            }
                        }
                    }
                    catch
                    {
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);

                    lbl = (Label)e.Row.Cells[0].FindControl("lblLastUpd_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfLastUpd_out");
                    DateTime dt = new DateTime();
                    dt = Convert.ToDateTime(hfField.Value);
                    lbl.Text = dt.ToShortDateString();
                }
                catch
                {
                }
            }
        }

        protected void lbSearchUser_Click(object sender, EventArgs e)
        {
            SessionManager.SearchCriteria = tbSearchString.Text.Trim();
            args = SessionManager.SearchCriteria.ToString().Split('~');
            DoUserSearch();
        }

        protected void lnkUserView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal userID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            SessionManager.SelectedUser = SQMModelMgr.LookupPerson(entities, userID, "", false);

            Response.Redirect("/Admin/Administrate_ViewUser.aspx");
        }

        protected void lbUserAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_User.aspx");
        }
    }
}