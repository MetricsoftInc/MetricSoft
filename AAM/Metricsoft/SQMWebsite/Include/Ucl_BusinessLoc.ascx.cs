using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website
{
    public delegate void BusinessLocationChange(BusinessLocation businessLocation);


    public partial class Ucl_BusinessLoc : System.Web.UI.UserControl
    {
        static private BusinessLocation newLocation;
        static private BusinessLocation currentLocation;
        static private bool staticShowHeader;

        public event BusinessLocationChange OnCompanyChange;
        public event BusinessLocationChange OnBusinessLocationChange;
        public event BusinessLocationChange OnBusinessLocationAdd;

        public event GridItemClick OnCompanyClick;
        public event GridItemClick OnUsersClick;

        public Button CancelButton
        {
            get { return btnCancel; }
        }
        public Button ResetButton
        {
            get { return btnReset; }
        }

        public Panel LocationSelectPanel
        {
            get { return pnlBusinessLoc; }
        }
        public BusinessLocation NewLocation
        {
            get { return newLocation; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclCompanyList.OnCompanyClick += SelectCompany;
            uclCompanyList.OnCompanyCloseClick += SelectCompany;
            uclCompanyList.OnOrgClick += SelectBusOrg;
            uclCompanyList.OnPlantClick += SelectPlant;

            uclCompanyList.OnAddPlantClick += AddPlant;
       
            uclItemHdr.OnCompanyClick += CompanyDataClick;
            uclItemHdr.OnCompanyUsersClick += CompanyUsersClick;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (SessionManager.UserContext != null && SessionManager.UserContext.HRLocation != null)
                ddlSelectLocation.Items[0].Text = SessionManager.UserContext.HRLocation.Company.COMPANY_NAME;
        }

        public void BindBusinessLocation(BusinessLocation businessLocation, bool bindAll, bool allowChange, bool showLocationInputs, bool showHeader)
        {
            newLocation = currentLocation = businessLocation;
            staticShowHeader = showHeader;

            if (allowChange)
            {
                pnlBusinessLocEdit.Visible = true;
                if (bindAll)
                {
                    if (newLocation.BusinessOrg != null)
                        lblSelBusOrg.Text = newLocation.BusinessOrg.ORG_NAME;
                    if (newLocation.Plant != null)
                        lblSelPlant.Text = newLocation.Plant.PLANT_NAME;
                }
                else
                {
                    lblSelBusOrg.Visible =lblSelPlant.Visible = false;
                }
            }
            else
            {
                pnlBusinessLoc.Visible = true;
                if (newLocation.Company != null)
                    lblUserCompany.Text = newLocation.Company.COMPANY_NAME;
                if (bindAll)
                {
                    if (newLocation.BusinessOrg != null)
                        lblUserBusOrg.Text = newLocation.BusinessOrg.ORG_NAME;
                    if (newLocation.Plant != null)
                        lblUserPlant.Text = newLocation.Plant.PLANT_NAME;
                }
            }

            if (showLocationInputs)
            {
                pnlLocationText.Visible = btnCancel.Visible = true;
                //btnReset.Visible = true;
            }
        }

        protected void ddlSelectLocation_Change(object sender, EventArgs e)
        {
        }

        protected void btnListCompanies_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            pnlSelectCompany.Visible = true;
            switch (btn.CommandArgument)
            {
                case "prime":
				default:
                    SelectCompany(SessionManager.UserContext.HRLocation.Company.COMPANY_ID);
                    return;
                    break;
            }
            uclCompanyList.CloseCompanyListButton.Visible = false;
            btn.Focus();
        }

        public void RefreshOrgList(BusinessLocation location)
        {
            pnlSelectCompany.Visible = true;
            SelectCompany(location.Company.COMPANY_ID);
        }

        protected void SelectCompany(decimal companyID)
        {
            if (companyID > 0)
            {
                newLocation = new BusinessLocation();
                newLocation.Company = (COMPANY)SQMModelMgr.LookupCompany(companyID);
                lblSelBusOrg.Text = lblSelPlant.Text = "";
                if (staticShowHeader)
                    uclItemHdr.DisplayCompanyData(newLocation.Company, SQMModelMgr.PersonCount(companyID));
                uclCompanyList.BindOrgListRepeater(SQMModelMgr.SearchBusOrgList(new PSsqmEntities(), companyID, "", false).OrderBy(l=> l.BUS_ORG_ID).ToList());
                if (OnCompanyChange != null)
                {
                    OnCompanyChange(newLocation);
                }
            }
            else
            {
                pnlSelectCompany.Visible = false;
            }
        }

        protected void SelectBusOrg(decimal busOrgID)
        {
            newLocation.BusinessOrg = SQMModelMgr.LookupBusOrg(busOrgID);
            newLocation.Plant = null;
            lblSelBusOrg.Text = newLocation.BusinessOrg.ORG_NAME;
            lblSelPlant.Text = "";
            pnlSelectCompany.Visible = false;
            if (OnBusinessLocationChange != null)
            {
                OnBusinessLocationChange(newLocation);
            }
        }


        protected void AddPlant(decimal busOrgID, decimal plantID)
        {
            newLocation.BusinessOrg = SQMModelMgr.LookupBusOrg(busOrgID);
            newLocation.Company = SQMModelMgr.LookupCompany(newLocation.BusinessOrg.COMPANY_ID);
            newLocation.Plant = null;
            OnBusinessLocationAdd(newLocation);
        }

        protected void SelectPlant(decimal plantID)
        {
            newLocation.Plant = SQMModelMgr.LookupPlant(plantID);
            if (newLocation.BusinessOrg == null || (newLocation.BusinessOrg.BUS_ORG_ID != newLocation.Plant.BUS_ORG_ID))
            {
                newLocation.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)newLocation.Plant.BUS_ORG_ID);
                lblSelBusOrg.Text = newLocation.BusinessOrg.ORG_NAME;
            }

            lblSelPlant.Text = newLocation.Plant.PLANT_NAME;

            pnlSelectCompany.Visible = false;
            if (OnBusinessLocationChange != null)
            {
                OnBusinessLocationChange(newLocation);
            }
        }

        protected void CompanyDataClick(decimal companyID)
        {
            if (OnCompanyClick != null)
            {
                OnCompanyClick(companyID);
            }
        }

        protected void CompanyUsersClick(decimal companyID)
        {
            if (OnUsersClick != null)
            {
                OnUsersClick(companyID);
            }
        }
    }
}