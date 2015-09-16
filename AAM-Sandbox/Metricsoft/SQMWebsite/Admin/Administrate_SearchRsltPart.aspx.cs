using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_SearchRsltPart : SQMBasePage
    {
        static List<PART> partList;
        static string[] args;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(SessionManager.PartSearchCriteria.ToString()))
                {
                    args = SessionManager.PartSearchCriteria.ToString().Split('~');
                    SessionManager.PartSearchCriteria = "";
                }
                    
                DoPartSearch();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
            tbSearchString.Text = args[0];
        }


        private void DoPartSearch()
        {
            bool activeOnly = false;
            if (args.Length > 1 && args[1] == "A")
                activeOnly = true;

            partList = SQMModelMgr.SearchPartList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, args[0], activeOnly);
            GridView gv = (GridView)hfBase.FindControl("gvPartList");
            gv.DataSource = partList;
            gv.DataBind();
            SetGridViewDisplay(gv, (Label)hfBase.FindControl("lblPartListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divGVScroll"), 0);
        }

        public void gvList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                }
                catch
                {
                }
            }
        }

        protected void lbSearchPart_Click(object sender, EventArgs e)
        {
            SessionManager.PartSearchCriteria = tbSearchString.Text.Trim();
            args = SessionManager.PartSearchCriteria.ToString().Split('~');
            DoPartSearch();
        }

        protected void lnkPartView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal partID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            SessionManager.Part = SQMModelMgr.LookupPart(entities, partID, "", SessionManager.SessionContext.ActiveCompany().COMPANY_ID, false);

            Response.Redirect("/Admin/Administrate_ViewPart.aspx");
        }

        protected void lbPartAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_Part.aspx");
        }
    }
}