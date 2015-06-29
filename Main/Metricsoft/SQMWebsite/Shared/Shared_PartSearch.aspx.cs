using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website.Shared
{
    public partial class Shared_PartSearch : SQMBasePage
    {
        static B2BPartner searchMgr;

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchList.OnPartDataClick += uclSearchList_OnPartDataClick;
           // uclSearchList.OnPartListCloseClick += uclSearchList_OnPartListCloseClick;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                    searchMgr = new B2BPartner().Initialize();
                    searchMgr.CompanyID = SessionManager.SessionContext.PrimaryCompany.COMPANY_ID;
                    searchMgr.BusorgID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
                    if (SessionManager.UserContext.HRLocation.Plant != null)
                        searchMgr.PlantID = SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID;
                SetupPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        private void SetupPage()
        {
            SessionManager.ClearReturns();

            ddlPlantWhereUsed.DataSource = SQMModelMgr.SelectPlantList(entities, searchMgr.CompanyID, searchMgr.BusorgID);
            ddlPlantWhereUsed.DataTextField = "PLANT_NAME";
            ddlPlantWhereUsed.DataValueField = "PLANT_ID";
            ddlPlantWhereUsed.DataBind();
            ddlPlantWhereUsed.Items.Insert(0, new ListItem("", "0"));
            ddlPlantWhereUsed.SelectedValue = searchMgr.PlantID.ToString();


            decimal customerID = 0;
            if (SessionManager.UserContext.WorkingLocation.IsCustomerCompany(true))     // show part programs for user's customer location
                customerID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

            ddlPartProgram.DataSource = SQMModelMgr.SelectPartProgramList(customerID, 0);
            ddlPartProgram.DataTextField = "PROGRAM_NAME";
            ddlPartProgram.DataValueField = "PROGRAM_ID";
            ddlPartProgram.DataBind();
            ddlPartProgram.Items.Insert(0, new ListItem("", "0"));

          //  btnSearchParts_Click(null, null);
        }

        protected void btnPartSearchReset_Click(object sender, EventArgs e)
        {
            tbPartString.Text =  "";
            ddlPlantWhereUsed.SelectedValue = searchMgr.PlantID.ToString();
            pnlSearchList.Visible = false;
        }


        protected void btnSearchParts_Click(object sender, EventArgs e)
        {
            searchMgr.PlantID = Convert.ToInt32(ddlPlantWhereUsed.SelectedValue);
            pnlSearchList.Visible = true;
            uclSearchList.BindPartList(SQMModelMgr.SelectPartDataList(entities, searchMgr.CompanyID, 0, Convert.ToDecimal(ddlPartProgram.SelectedValue), Convert.ToDecimal(ddlPlantWhereUsed.SelectedValue), 0));
            //uclSearchList.PartListCloseButton.Visible = false;
        }

        protected void uclSearchList_OnPartDataClick(PartData part)
        {
            SessionManager.ReturnObject = part;
            SessionManager.ReturnStatus = true;

            // close the selector window ...
            System.Threading.Thread.Sleep(250);
            string javaScript = "<script language=JavaScript>\n" + "window.opener.document.forms[0].submit(); window.close();\n" + "</script>";
            RegisterStartupScript("lnkPartNum_Click", javaScript);
        }


        #endregion

    }
}