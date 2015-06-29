using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_SearchRsltPlant : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.PlantSearchCriteria != null)
                {
                    DoPlantSearch((string)SessionManager.PlantSearchCriteria);
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
        }


        private void DoPlantSearch(string searchString)
        {
            bool activeOnly = false;
            bool unAssocOnly = false;
            string[] args = searchString.Split('~');
            if (args.Length > 1 && args[1] == "A")
                activeOnly = true;
            if (args.Length > 1 && args[1] == "U")
                unAssocOnly = true;

            List<PLANT> plantList = SQMModelMgr.SearchPlantList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, args[0], activeOnly, unAssocOnly);
            GridView gvPlantList = (GridView)hfBase.FindControl("gvPlantList");
            gvPlantList.DataSource = plantList;
            gvPlantList.DataBind();
            SetGridViewDisplay(gvPlantList, (Label)hfBase.FindControl("lblPlantListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divGVScroll"), 0);
        }

        public void gvPlantList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblBusorg_out");
                    if (!String.IsNullOrEmpty(lbl.Text))
                    {
                        BUSINESS_ORG busorg = SQMModelMgr.LookupBusOrg(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, Convert.ToDecimal(lbl.Text));
                        if (busorg != null)
                            lbl.Text = busorg.ORG_NAME;
                    }
 
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                }
                catch
                {
                }
            }
        }

        protected void lnkPlantView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal plantID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            SessionManager.Plant = SQMModelMgr.LookupPlant(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, 0, plantID, "", false);

            Response.Redirect("/Admin/Administrate_ViewPlant.aspx");
        }

        protected void lbSearchPlant_Click(object sender, EventArgs e)
        {
            SessionManager.PlantSearchCriteria = (string)tbSearchString.Text;
            DoPlantSearch(tbSearchString.Text);
        }

        protected void lbPlantAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_BusOrg.aspx");
        }
    }
}