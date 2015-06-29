using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_SearchRsltBuyer : SQMBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.BuyerSearchCriteria != null)
                {
                    DoBuyerSearch((string)SessionManager.BuyerSearchCriteria);
                }
            }
        }

        private void DoBuyerSearch(string searchString)
        {
            string[] args = searchString.Split('~');

            var buyerList = SQMModelMgr.SearchBuyerList(entities, SessionManager.ActiveCompany().COMPANY_ID, 0, SessionManager.BuyerSearchCriteria);
            GridView gv = (GridView)hfBase.FindControl("gvBuyerList");
            gv.DataSource = buyerList;
            gv.DataBind();
            SetGridViewDisplay(gv, gv.Rows.Count, "lblBuyerListEmpty");
        }

        public void gvBuyerList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
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

        private void SetGridViewDisplay(GridView gv, int listCount, string gridLabel)
        {
            if (listCount == 0)
            {
                gv.Visible = false;
                hfBase.FindControl(gridLabel).Visible = true;
            }
            else
            {
                gv.Visible = true;
                hfBase.FindControl(gridLabel).Visible = false;
            }
        }

        protected void lbSearchBuyer_Click(object sender, EventArgs e)
        {
            SessionManager.BuyerSearchCriteria = tbSearchString.Text.Trim();
            DoBuyerSearch(SessionManager.BuyerSearchCriteria);
        }

        protected void lbBuyerAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_TradePartner.aspx");
        }

        protected void lnkBuyerView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal personID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            PERSON buyerPerson = SQMModelMgr.LookupPerson(entities, personID, "", false);
            SessionManager.Buyer = buyerPerson.BUYER.First();

            Response.Redirect("/Admin/Administrate_ViewBuyer.aspx");
        }
    }
}