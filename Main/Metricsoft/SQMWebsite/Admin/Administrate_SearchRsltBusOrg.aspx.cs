using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace SQM.Website
{
    public partial class Administrate_SearchRsltBusOrg : SQMBasePage
    {
        List<BUSINESS_ORG> orgList;
        int totalRowCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.BusOrgSearchCriteria != null)
                {
                    DoBusOrgSearch((string)SessionManager.BusOrgSearchCriteria);
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
        }

        private void DoBusOrgSearch(string searchString)
        {   
            bool activeOnly = false;
            string[] args = searchString.Split('~');
            if (args.Length > 1 && args[1] == "A")
                activeOnly = true;

            totalRowCount = 0;
            orgList = SQMModelMgr.SearchBusOrgList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, args[0], activeOnly);
            gvBusOrgList.DataSource = orgList;
            gvBusOrgList.DataBind();
            SetGridViewDisplay(gvBusOrgList, lblBusOrgListEmpty, divGVScroll, 20, (totalRowCount += gvBusOrgList.Rows.Count));
        }

        public void gvBusOrgList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);

                    BUSINESS_ORG busOrg = orgList[e.Row.RowIndex];
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvPlantGrid");
                    gv.DataSource = busOrg.PLANT;
                    gv.DataBind();
                    totalRowCount += gv.Rows.Count;
                }
                catch
                {
                }
            }
        }

        protected void lnkView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal busOrgID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            SessionManager.BusinessOrg = SQMModelMgr.LookupBusOrg(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, busOrgID);
            Response.Redirect("/Admin/Administrate_ViewBusOrg.aspx");
        }

        protected void lnkPlantView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal plantID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            SessionManager.Plant = SQMModelMgr.LookupPlant(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, 0, plantID, "", false);

            Response.Redirect("/Admin/Administrate_ViewPlant.aspx");
        }

        protected void lbSearchBusOrg_Click(object sender, EventArgs e)
        {
            SessionManager.BusOrgSearchCriteria = (string)tbSearchString.Text;
            DoBusOrgSearch(tbSearchString.Text);
        }

        protected void lbBusOrgAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_BusOrg.aspx");
        }
    }
}