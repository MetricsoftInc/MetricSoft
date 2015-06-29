using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_ViewBuyer : SQMBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.Buyer != null)
                {
                    ClearTempData();
                    SetupPage();
                }
            }
        }

        private void ClearTempData()
        {
 
        }

        private void SetupPage()
        {
            BUYER buyer = SessionManager.Buyer;

            lblViewBuyerTitle.Text += (" - " + buyer.BUYER_CODE);
            HiddenField hdCurrentActiveTab = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveSecondaryTab");
            hdCurrentActiveTab.Value = SessionManager.CurrentAdminTab = "lbTrade";

            ddlUsers.Enabled = false;
            lbSaveBuyerPerson1.Enabled = lbSaveBuyerPerson2.Enabled = false;
            SetUserList(false);

            DoBuyerPartList();
        }

        protected void wasChanged(object sender, EventArgs e)
        {

        }

        protected void lbSearchBuyers_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_TradePartner.aspx");
        }

        private void SetUserList(bool loadAll)
        {
            BUYER buyer = SessionManager.Buyer;
            List<PERSON> personList = null;

            if (loadAll)
            {
                personList = SQMModelMgr.SelectPersonList(SessionManager.ActiveCompany().COMPANY_ID, 0, false);
                foreach (PERSON person in personList)
                {
                    person.SSO_ID = person.FIRST_NAME + " " + person.LAST_NAME;
                }
            }
            else
            {
                PERSON person = SQMModelMgr.LookupPerson(entities, buyer.PERSON_ID, "", false);
                if (person != null)
                {
                    person.SSO_ID = person.FIRST_NAME + " " + person.LAST_NAME;
                    personList = new List<PERSON>();
                    personList.Add(person);
                }
            }

            ddlUsers.DataSource = personList;
            ddlUsers.DataTextField = "SSO_ID";
            ddlUsers.DataValueField = "PERSON_ID";
            ddlUsers.DataBind();
            ddlUsers.Items.Insert(0, new ListItem("--- no user selected ---", "0"));

            if (buyer.PERSON_ID > 0)
                ddlUsers.SelectedValue = buyer.PERSON_ID.ToString();
            else
                ddlUsers.SelectedValue = "0";
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


        protected void tab_Click(object sender, EventArgs e)
        {
            if (sender != null)
            {
                //isEditMode = isChanged = false;
                Button btn = (Button)sender;

                switch (btn.CommandArgument)
                {
                    case "edit":
                        AddBuyerPerson();
                        pnlBuyerpartList.Visible = true;
                        ddlUsers.Enabled = true;
                        break;
                    case "save":
                        SetupPage();
                        break;
                    case "buyer":
                        //lnkBuyerView_Click(sender, e);
                        break;
                    case "part":
                        DoBuyerPartList();
                        pnlBuyerpartList.Visible = true;
                        break;
                    default:
                        break;
                }
            }
        }

        #region buyerpart
        private void DoBuyerPartList()
        {
            BUYER buyer = SessionManager.Buyer;
            uclPartList.BindBuyerPartList(SQMModelMgr.SelectBuyerSupPartList(entities, (decimal)SessionManager.ActiveCompany().COMPANY_ID, 0, buyer.BUYER_CODE));
            uclPartList.BuyerPartGrid.Columns[3].Visible = false;
        }

        #endregion

        #region adduser

        protected void AddBuyerPerson()
        {
            SetUserList(true);
            ddlUsers.Enabled = true;
            lbSaveBuyerPerson1.Enabled = lbSaveBuyerPerson2.Enabled = true;
            ddlUsers.Focus();
        }


        #endregion
    }
}