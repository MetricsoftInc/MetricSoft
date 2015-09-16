using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_EditBusOrg : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSelects();

                if (SessionManager.BusinessOrg == null)
                {
                    pnlBusOrgDetail.Visible = false;
                }
                else
                {
                    SetupPage();
                }
            }
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));
        }

        private void SetupPage()
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.BusinessOrg;

            pnlBusOrgDetail.Visible = true;
            // bu summary section
            SetFindControlValue("lblOrgName_out", hfBase, busOrg.ORG_NAME);
            SetFindControlValue("lblLocCode_out", hfBase, busOrg.DUNS_CODE);
            SetFindControlValue("lblCurrency_out", hfBase, busOrg.PREFERRED_CURRENCY_CODE);
            SetFindControlValue("lblCaseThreshold_out", hfBase, busOrg.THRESHOLD_AMT.ToString());
            SetFindControlValue("lblStatus_out", hfBase, WebSiteCommon.GetStatusString(busOrg.STATUS));
            SetFindControlValue("lblUpdatedBy_out", hfBase, busOrg.LAST_UPD_BY);
            SetFindControlValue("lblUpdatedDate_out", hfBase, WebSiteCommon.LocalTime((DateTime)busOrg.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString());


            if (busOrg.PARENT_BUS_ORG_ID == busOrg.BUS_ORG_ID || busOrg.PARENT_BUS_ORG_ID < 1)
                SetFindControlValue("lblParentBU_out", hfBase, "Top Level");
            else
            {
                if (SessionManager.ParentBusinessOrg == null)
                {
                    SessionManager.ParentBusinessOrg = SQMModelMgr.LookupParentBusOrg(entities, busOrg);
                }
                BUSINESS_ORG parentOrg = (BUSINESS_ORG)SessionManager.ParentBusinessOrg;
                SetFindControlValue("lblParentBU_out", hfBase, parentOrg.ORG_NAME);
            }

            // editable fields
            SetFindControlValue("tbOrgname", hfBase, busOrg.ORG_NAME);
            SetFindControlValue("tbOrgLocCode", hfBase, busOrg.DUNS_CODE);
            SetFindControlValue("tbThreshold", hfBase, busOrg.THRESHOLD_AMT.ToString());
            SetFindControlValue("ddlCurrencyCodes", hfBase, busOrg.PREFERRED_CURRENCY_CODE);
            SetFindControlValue("ddlStatus", hfBase, busOrg.STATUS);
            if (busOrg.PARENT_BUS_ORG_ID > 0)
                SetFindControlValue("ddlParentBusOrg", hfBase, busOrg.PARENT_BUS_ORG_ID.ToString());
            SetFindControlValue("lblLastUpdate_out", hfBase, busOrg.LAST_UPD_BY);
            SetFindControlValue("lblLastUpdateDate_out", hfBase, WebSiteCommon.LocalTime((DateTime)busOrg.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString());
        }

        private void LoadSelects()
        {
            List<Settings> currency_codes = SQMSettings.CurrencyCode;
            DropDownList ddl = (DropDownList)hfBase.FindControl("ddlCurrencyCodes");
            ddl.DataSource = currency_codes;
            ddl.DataTextField = "code";
            ddl.DataValueField = "code";
            ddl.DataBind();
            ddl.SelectedValue = "USD";

            List<BUSINESS_ORG> parent_orgs = SQMModelMgr.SelectBusOrgList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, 0, true);
            ddl = (DropDownList)hfBase.FindControl("ddlParentBusOrg");
            ddl.DataSource = parent_orgs;
            ddl.DataTextField = "ORG_NAME";
            ddl.DataValueField = "BUS_ORG_ID";
            ddl.DataBind();

            List<Settings> status_codes = SQMSettings.Status;
            ddl = (DropDownList)hfBase.FindControl("ddlStatus");
            ddl.DataSource = status_codes;
            ddl.DataTextField = "short_desc";
            ddl.DataValueField = "code";
            ddl.DataBind();
        }

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_ViewBusOrg.aspx");
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            BUSINESS_ORG bu = (BUSINESS_ORG)SessionManager.BusinessOrg;
            BUSINESS_ORG busOrg = null;

            if (!Page.IsValid)
                return;

            if (bu == null)
            {
                busOrg = new BUSINESS_ORG();
            }
            else
            {
                busOrg = SQMModelMgr.LookupBusOrg(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, bu.BUS_ORG_ID);
            }

            bool success;
            decimal decVal;
            busOrg.ORG_NAME = GetFindControlValue("tbOrgname", hfBase, out success);
            busOrg.DUNS_CODE = GetFindControlValue("tbOrgLocCode", hfBase, out success);
            if (decimal.TryParse(GetFindControlValue("tbThreshold", hfBase, out success), out decVal))
                busOrg.THRESHOLD_AMT = decVal;
            busOrg.PREFERRED_CURRENCY_CODE = GetFindControlValue("ddlCurrencyCodes", hfBase, out success);
            busOrg.STATUS = GetFindControlValue("ddlStatus", hfBase, out success);
            string sel = GetFindControlValue("ddlParentBusOrg", hfBase, out success);
            if (string.IsNullOrEmpty(sel))
                busOrg.PARENT_BUS_ORG_ID = Convert.ToInt32(null);
            else
                busOrg.PARENT_BUS_ORG_ID = Int32.Parse(sel);

            busOrg = (BUSINESS_ORG)SQMModelMgr.SetObjectTimestamp((object)busOrg, SessionManager.UserContext.UserName(), busOrg.EntityState);

            if (bu == null)
            {
                busOrg.COMPANY_ID = SessionManager.SessionContext.ActiveCompany().COMPANY_ID;
                SQMModelMgr.CreateBusOrg(entities, busOrg);
                SessionManager.BusOrgSearchCriteria = busOrg.ORG_NAME;
            }
            else
            {
                entities.SaveChanges();
            }

            SessionManager.BusinessOrg = busOrg;

            SetupPage();
        }

        protected void lbBusOrgAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_BusOrg.aspx");
        }

        protected void lbBusOrgSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_SearchRsltBusOrg.aspx");
        }

    }
}