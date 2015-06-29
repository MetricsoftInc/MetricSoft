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
    public partial class Administrate_TradePartner : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (rblAssignedBuyer.SelectedIndex < 0)
                rblAssignedBuyer.SelectedIndex = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
            hfld.Value = SessionManager.CurrentAdminTab = "lbTrade";
        }

        protected void lbSearchBuyer_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (String.IsNullOrEmpty(tbSearchString.Text))
                SessionManager.BuyerSearchCriteria = "%";
            else
                SessionManager.BuyerSearchCriteria = tbSearchString.Text;

            if (rblAssignedBuyer.SelectedIndex > 0)
                SessionManager.BuyerSearchCriteria += ("~"+rblAssignedBuyer.SelectedValue);

            Response.Redirect("/Admin/Administrate_SearchRsltBuyer.aspx");
        }

        protected void btnSearchSupplier_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SessionManager.SearchCriteria = (btn.CommandArgument.ToString() + "~" + tbSearchSupplierCompany.Text);

            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_ViewTradePartner.aspx");
        }

        protected void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SessionManager.SearchCriteria = (btn.CommandArgument.ToString() + "~" + tbSearchCustomerCompany.Text);

            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_ViewTradePartner.aspx");
        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }

        protected void btnB2B_Click(object sender, EventArgs e)
        {
            SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_ViewTradePartner.aspx");
        }
    }
}