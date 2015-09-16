using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Ucl_ItemHdr : System.Web.UI.UserControl
    {
        public event GridItemClick OnCompanyClick;
        public event GridItemClick OnCompanyUsersClick;
        public event GridItemClick OnHdrItemClick;

        public void ToggleVisible(Panel pnlTarget)
        {
            pnlCompanyHdr.Visible = pnlCtlPlanHdr.Visible = pnlBusOrgHdr.Visible = pnlPlantHdr.Visible = pnlPartHdr.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        #region company
        public void DisplayCompany(COMPANY company)
        {
            pnlCompanyHdr.Visible = true;
            lblCompanyName_out.Text = company.COMPANY_NAME;
            lblUlDunsCode_out.Text = company.ULT_DUNS_CODE;
            lblCompanyStatus_out.Text = WebSiteCommon.GetStatusString(company.STATUS);
            lblCompanyUpdatedDate_out.Text = WebSiteCommon.LocalTime((DateTime)company.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
        }
        #endregion

        #region companydata

        public void DisplayCompanyData(COMPANY company, int userCount)
        {
            pnlCompanyData.Visible = true;
            lnkCompany.Text = company.COMPANY_NAME;
            lnkCompany.CommandArgument =  lnkCompanyUsers.CommandArgument = company.COMPANY_ID.ToString();
            lblCompanyCode.Text = company.ULT_DUNS_CODE;
            lblCompanyStatus2.Text = WebSiteCommon.GetStatusString(company.STATUS);
            if (userCount > -1)
                lnkCompanyUsers.Text = lblManageUsers.Text + " (" + userCount.ToString() + ")";
        }

        protected void lnkCompany_Click(object sender, EventArgs e)
        {
            if (OnCompanyClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                decimal companyID;
                if (decimal.TryParse(lnk.CommandArgument.ToString(), out companyID))
                    OnCompanyClick(companyID);
            }
        }
        protected void lnkCompanyUsers_Click(object sender, EventArgs e)
        {
            if (OnCompanyUsersClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                decimal companyID;
                if (decimal.TryParse(lnk.CommandArgument.ToString(), out companyID))
                    OnCompanyUsersClick(companyID);
            }
        }
        #endregion

        #region busorg
        public void DisplayBusOrg(COMPANY company, BUSINESS_ORG busOrg)
        {
            pnlBusOrgHdr.Visible = true;
            lblOrgName_out.Text = busOrg.ORG_NAME;
            lblLocCode_out.Text = busOrg.DUNS_CODE;

            if (busOrg.PARENT_BUS_ORG_ID == busOrg.BUS_ORG_ID || busOrg.PARENT_BUS_ORG_ID < 1)
                lblParentBU_out.Text =  "Top Level";
            else
            {
                if (SessionManager.ParentBusinessOrg == null)
                {
                    if ((SessionManager.ParentBusinessOrg = SQMModelMgr.LookupParentBusOrg(null, busOrg)) == null)
                    {
                        SessionManager.ParentBusinessOrg = busOrg;
                    }
                }
                BUSINESS_ORG parentOrg = (BUSINESS_ORG)SessionManager.ParentBusinessOrg;
                lblParentBU_out.Text = parentOrg.ORG_NAME;
            }
        }
 
        #endregion

        #region plant
        public void DisplayPlant(BUSINESS_ORG busOrg, PLANT plant)
        {
            pnlPlantHdr.Visible = true;
            lblPlantOrgName_out.Text = busOrg.ORG_NAME;
            if (plant != null)
            {
                lblPlantName_out.Text = plant.PLANT_NAME;
                lblLocCodePlant_out.Text = plant.DUNS_CODE;
                lblLocationType_out.Text = WebSiteCommon.GetXlatValue("locationType", plant.LOCATION_TYPE);
            }
        }
        #endregion

        #region part
        public void DisplayPart(PartData partData)
        {
            pnlPartHdr.Visible = true;
            lblPartNum_out.Text = partData.Part.PART_NUM;
            lblPartNumFull_out.Text = SQMModelMgr.GetFullPartNumber(partData.Part);
            lblPartName_out.Text = partData.Part.PART_NAME;
            if (partData.Program != null)
                lblPartProgram_out.Text = partData.Program.PROGRAM_NAME;
        }
        #endregion

        #region ctlplan
        public void DisplayCtlPlan(PartData partData, CTL_PLAN ctlPlan)
        {
            DisplayPart(partData);
            pnlCtlPlanHdr.Visible = true;
            lblPlanName_out.Text = ctlPlan.CTLPLAN_NAME;
            lblPlanVersion_out.Text = ctlPlan.VERSION;
            lblPlanDesc_out.Text = ctlPlan.CTLPLAN_DESC;
            lblPlanType_out.Text = WebSiteCommon.GetXlatValueLong("planType", ctlPlan.CTLPLAN_TYPE);
            lblPlanRef_out.Text = ctlPlan.RTE_REF;
            lblPlanResponsible_out.Text = ctlPlan.RESPONSIBILITY;
            lblEffDate_out.Text = ctlPlan.EFF_DATE.ToString();
        }
        #endregion
    }
}