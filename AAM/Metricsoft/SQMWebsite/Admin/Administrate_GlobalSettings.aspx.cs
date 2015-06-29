using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_GlobalSettings : SQMBasePage
    {
        static bool editEnabled;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnCancelClick += uclSearchBar_OnCancelClick;
            uclSearchBar.OnSaveClick += uclSearchBar_OnSaveClick;
        }

        private void uclSearchBar_OnCancelClick()
        {
            SessionManager.ReturnObject = SessionManager.EffLocation;
            SessionManager.ReturnStatus = true;
            Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewBusOrg.aspx");
        }
 
        private void uclSearchBar_OnSaveClick()
        {
            SaveSettings();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.PageTitle.Text = (lblViewSettingsTitle.Text + SessionManager.EffLocation.Company.COMPANY_NAME);
                uclSearchBar.SetButtonsVisible(false, false, false, true, false, false);
                uclSearchBar.SetButtonsEnabled(false, true, false, true, false, false);
                editEnabled = true;
                SetupPage();
            }
            else
            {
                if (SessionManager.IsEffLocationPrimary())
                    uclDocMgr.BindDocMgr("SYS", 0, 0);
            }
        }

        private void SetupPage()
        {
            COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);

            uclItemHdr.DisplayCompany(company);
            SetFindControlValue("cbIsCustomer", hfBase, company.IS_CUSTOMER.ToString(), editEnabled);
            SetFindControlValue("cbIsSupplier", hfBase, company.IS_SUPPLIER.ToString(), editEnabled);

            if (SessionManager.IsEffLocationPrimary())
                uclDocMgr.BindDocMgr("SYS", 0, 0);
        }


        protected void SaveSettings()
        {
            bool success;
            string value;
            COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);

            value = GetFindControlValue("cbIsCustomer", hfBase, out success);
            if (success)
                company.IS_CUSTOMER = Convert.ToBoolean(value);

            value = GetFindControlValue("cbIsSupplier", hfBase, out success);
            if (success)
                company.IS_SUPPLIER = Convert.ToBoolean(value);

            SQMModelMgr.UpdateCompany(entities, company, SessionManager.UserContext.UserName());
            SetupPage();
        }
    }
}