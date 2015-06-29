using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_ViewTradePartner : SQMBasePage
    {
        static string[] args;
        static List<vw_CustPlantPart> custList;
        static List<vw_CustPlantPart> suppList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(SessionManager.SearchCriteria.ToString()))
            {
                args = SessionManager.SearchCriteria.ToString().Split('~');
                SessionManager.SearchCriteria = "";
            }

            DoB2BSearch();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
            hfld.Value = SessionManager.CurrentAdminTab = "lbTrade";

            lblViewTradePartnerTitle.Text += SessionManager.ActiveCompany().COMPANY_NAME;
            tbSearchString.Text = args[1];
        }

        protected void lbTradePartnerAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_TradePartner.aspx");
        }

        protected void lbSearchCompany_Click(object sender, EventArgs e)
        {
            args[1] = tbSearchString.Text.Trim();
            DoB2BSearch();
        }

        private void DoB2BSearch()
        {
            List<COMPANY> hierList = new List<COMPANY>();
            List<SQM.Shared.B2BPartner> b2bList = new List<SQM.Shared.B2BPartner>();

            if (args[0] == "cust")
            {
                pnlB2BSupplier.Visible = lblSearchSupplier.Visible = lblSupplierListHdr.Visible = false;
                pnlB2BCustomer.Visible = lblSearchCustomer.Visible = lblCustomerListHdr.Visible = true;

                if (custList == null)
                    custList = SQMModelMgr.SelectCustomerPlantPartList(entities, SessionManager.ActiveCompany().COMPANY_ID, 0, 0);

                uclPartList.BindCustPlantList(custList);
            }

            if (args[0] == "supp")
            {
                pnlB2BSupplier.Visible = lblSearchSupplier.Visible = lblSupplierListHdr.Visible = true;
                pnlB2BCustomer.Visible = lblSearchCustomer.Visible = lblCustomerListHdr.Visible = false;

                if (suppList == null)
                    suppList = SQMModelMgr.SelectSupplierPlantPartList(entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, 0, 0);

                uclPartList.BindSuppPlantList(suppList);
            }
        }

        protected void wasChanged(object sender, EventArgs e)
        {

        }

        protected void btnTradePartnerCancel_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnTradePartnerSave_Click(object sender, EventArgs e)
        {

        }

        protected void lnkPartView_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal partID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());

            SessionManager.Part = SQMModelMgr.LookupPartData(entities, SessionManager.ActiveCompany().COMPANY_ID, partID);

            Response.Redirect("/Admin/Administrate_ViewPart.aspx");
        }
    }
}