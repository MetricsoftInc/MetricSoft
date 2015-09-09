using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Administrate_Company : SQMBasePage
    {
        static List<PERSPECTIVE_TARGET> targetList;
        static PERSPECTIVE_TARGET staticTarget;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;
            uclSearchBar.OnReturnClick += uclSearchBar_OnReturnClick;
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        private void uclSearchBar_OnReturnClick()
        {
            SessionManager.ReturnObject = SessionManager.EffLocation.Company;
            SessionManager.ReturnStatus = true;
            Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewBusOrg.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.SetButtonsVisible(false, false, false, false, false, true);
                uclSearchBar.ReturnButton.Text = lblViewBusOrgText.Text;
                uclAdminTabs.CompanyPanel.Visible = true;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);
                uclItemHdr.DisplayCompany(company);
                if (SessionManager.IsEffLocationPrimary())
                    divNavArea.Visible = true;
                else
                    divNavArea.Visible = false;

                if (ddlStatus.Items.Count == 0)
                {
                    List<Settings> status_codes = SQMSettings.Status;
                    ddlStatus.DataSource = status_codes;
                    ddlStatus.DataTextField = "short_desc";
                    ddlStatus.DataValueField = "code";
                    ddlStatus.DataBind();
                }
              
                if (string.IsNullOrEmpty(hfActiveTab.Value))
                    tab_Click("lbCompanyDetail_tab", "");
            }
        }

        protected void tab_Click(string tabID, string cmdArg)
        {
            if (tabID != null)
            {
                // setup for ps_admin.js to toggle the tab active/inactive display
                SetActiveTab(SessionManager.CurrentSecondaryTab = hfActiveTab.Value = tabID);

                pnlDetails.Visible = uclDocMgr.DocMgrPnl.Visible = pnlTargetList.Visible = pnlUomStd.Visible = false;
                COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);

                switch (tabID)
                {
                    case "lbCompanyDetail_tab":
                        pnlDetails.Visible = true;
                        if (ddlStatus.Items.FindByValue(company.STATUS) != null)
                            ddlStatus.SelectedValue = company.STATUS;
                        if (SessionManager.IsEffLocationPrimary() == true)
                        {
                            phUpdateCompany.Visible = false;
                            ddlStatus.Enabled = false;
                        }
                        break;
                    case "lbUomStds_tab":
                        pnlUomStd.Visible = true;
                        BindStdUnits(SessionManager.EffLocation.Company);
                        break;
                    case "lbCompanyTargets_tab":
                        pnlTargetList.Visible = true;
                        targetList = ViewModel.SelectTargets(entities, company.COMPANY_ID, 0);
                        BindTargetList(targetList);
                        if (ddlEffYear.Items.Count == 0)
                        {
                            ddlEffYear.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(2005, 15));
                            ddlTargetStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCodeDelete", "short"));
                            string[] targs = {"statScopeE", "statScopeHS", "statScopeQS"};
                            foreach (string scopelist in targs)
                            {
                                 foreach (WebSiteCommon.SelectItem si in WebSiteCommon.PopulateListItems(scopelist))
                                {
                                    if (string.IsNullOrEmpty(si.Value))
                                    {
                                        RadComboBoxItem li = new RadComboBoxItem(si.Text, si.Text);
                                        li.IsSeparator = true;
                                        ddlTarget.Items.Add(li);
                                    }
                                    else
                                    {
                                        ddlTarget.Items.Add(new RadComboBoxItem(si.Text, (si.Value + "|" + scopelist.Substring(9))));
                                    }
                                }
                            }
                        }

                        break;
                    default:
                        if (SessionManager.IsEffLocationPrimary())
                            uclDocMgr.BindDocMgr("SYS", 0, 0);
                        break;
                }
            }
        }

        private void SetupPage()
        {

        }

        protected void CancelCompany(object sender, EventArgs e)
        {
            uclSearchBar_OnReturnClick();
        }

        protected void UpdateCompany(object sender, EventArgs e)
        {
            COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);
            if (company != null)
            {
				company.IS_CUSTOMER = true;
				company.IS_SUPPLIER = true;
                company.STATUS = ddlStatus.SelectedValue;
                if (entities.SaveChanges() > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                }
                SessionManager.EffLocation.Company = company;
            }  
        }

        #region stds
        public void BindStdUnits(COMPANY company)
        {
            List<UOM> stdUom = SQMResourcesMgr.GetCompanyStdUnits(company.COMPANY_ID);

            if (ddlStdEnergy.Items.Count == 0)
            {
                ddlStdEnergy.DataSource = SessionManager.UOMList.Where(l => l.UOM_CATEGORY == "ENGY").ToList();
                ddlStdEnergy.DataValueField = "UOM_ID";
                ddlStdEnergy.DataTextField = "UOM_NAME";
                ddlStdEnergy.DataBind();

                ddlStdWeight.DataSource = SessionManager.UOMList.Where(l => l.UOM_CATEGORY == "WEIT").ToList();
                ddlStdWeight.DataValueField = "UOM_ID";
                ddlStdWeight.DataTextField = "UOM_NAME";
                ddlStdWeight.DataBind();

                ddlStdVolume.DataSource = SessionManager.UOMList.Where(l => l.UOM_CATEGORY == "VOL").ToList();
                ddlStdVolume.DataValueField = "UOM_ID";
                ddlStdVolume.DataTextField = "UOM_NAME";
                ddlStdVolume.DataBind();

                ddlStdLqdVolume.DataSource = SessionManager.UOMList.Where(l => l.UOM_CATEGORY == "VOL").ToList();
                ddlStdLqdVolume.DataValueField = "UOM_ID";
                ddlStdLqdVolume.DataTextField = "UOM_NAME";
                ddlStdLqdVolume.DataBind();
            }

            if (ddlStdEnergy.Items.FindByValue(stdUom.Where(u => u.UOM_CATEGORY == "ENGY").SingleOrDefault().UOM_ID.ToString()) != null)
                ddlStdEnergy.SelectedValue = stdUom.Where(u => u.UOM_CATEGORY == "ENGY").SingleOrDefault().UOM_ID.ToString();
            if (ddlStdWeight.Items.FindByValue(stdUom.Where(u => u.UOM_CATEGORY == "WEIT").SingleOrDefault().UOM_ID.ToString()) != null)
                ddlStdWeight.SelectedValue = stdUom.Where(u => u.UOM_CATEGORY == "WEIT").SingleOrDefault().UOM_ID.ToString();
            if (ddlStdVolume.Items.FindByValue(stdUom.Where(u => u.UOM_CATEGORY == "VOL").SingleOrDefault().UOM_ID.ToString()) != null)
                ddlStdVolume.SelectedValue = stdUom.Where(u => u.UOM_CATEGORY == "VOL").SingleOrDefault().UOM_ID.ToString();
            if (ddlStdLqdVolume.Items.FindByValue(stdUom.Where(u => u.UOM_CATEGORY == "EUTL").SingleOrDefault().UOM_ID.ToString()) != null)
                ddlStdLqdVolume.SelectedValue = stdUom.Where(u => u.UOM_CATEGORY == "EUTL").SingleOrDefault().UOM_ID.ToString();
            lblStdCurrency_out.Text = stdUom.Where(u => u.UOM_CATEGORY == "CUR").SingleOrDefault().UOM_CD.ToString();

        }
        #endregion
        #region targets
 
        protected void lnkTargetList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal id = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindTarget((staticTarget = targetList.FirstOrDefault(l => l.TARGET_ID == id)));
            lblAddTarget.Visible = true;
        }

        public void BindTargetList(List<PERSPECTIVE_TARGET> theList)
        {
            gvTargetList.DataSource = theList;
            gvTargetList.DataBind();
        }

        protected void BindTarget(PERSPECTIVE_TARGET target)
        {
            int n = -1;
            if (target != null)
            { 
                if (!string.IsNullOrEmpty(target.CALCS_SCOPE))
                {
                    foreach (RadComboBoxItem item in ddlTarget.Items)
                    {
                        ++n;
                        string[] args = item.Value.Split('|');
                        if (!item.IsSeparator  &&  args[0] == target.CALCS_SCOPE)
                        {
                            ddlTarget.SelectedIndex = n;
                            break;
                        }
                    }
                }

                tbTargetDescLong.Text = target.DESCR_LONG;
                if (ddlStatType.Items.FindByValue(target.SSTAT) != null)
                    ddlStatType.SelectedValue = target.SSTAT;
                if (target.EFF_YEAR > 0)
                    ddlEffYear.SelectedValue = target.EFF_YEAR.ToString();

                btnYTDMetric.Checked = btnYOYMetric.Checked = btnABSMetric.Checked = false;
                if (target.DATE_SPAN <= 0)
                    btnABSMetric.Checked = true;
                else if (target.DATE_SPAN == 1)
                    btnYTDMetric.Checked = true;
                else
                    btnYOYMetric.Checked = true;

                if (target.TARGET_VALUE.HasValue)
                    tbTargetValue.Text = SQMBasePage.FormatValue((decimal)target.TARGET_VALUE, 4);
                else
                    tbTargetValue.Text = "";

                btnTargetMin.Checked = btnTargetMax.Checked = false;
                if (target.MIN_MAX > 0)
                    btnTargetMax.Checked = true;
                else
                    btnTargetMin.Checked = true;

                ddlTargetStatus.SelectedValue = target.STATUS;
                pnlTargetEdit.Enabled = true;
                btnTargetSave.Enabled = btnTargetCancel.Enabled = true;

                udpTarget.Update();
            }
        }

        protected void btnTargetAdd_Click(object sender, EventArgs e)
        {
            staticTarget = new PERSPECTIVE_TARGET();
            staticTarget.TARGET_ID = 0;
            staticTarget.STATUS = "A";
            staticTarget.EFF_YEAR = DateTime.Now.Year;
            BindTarget(staticTarget);
        }

        protected void btnTargetSave_Click(object sender, EventArgs e)
        {
            bool success;
            Button btn = (Button)sender;
 
            if (btn.CommandArgument == "save")
            {
                if (staticTarget.TARGET_ID < 1)
                {
                    staticTarget = new PERSPECTIVE_TARGET();
                    staticTarget.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
                }
                else
                {
                    staticTarget = ViewModel.LookupTarget(entities, staticTarget.TARGET_ID);
                }

                string perspective = ddlTarget.SelectedValue.Substring(ddlTarget.SelectedValue.IndexOf('|')+1);
                staticTarget.PERSPECTIVE = perspective;
                staticTarget.CALCS_SCOPE = ddlTarget.SelectedValue.Substring(0, ddlTarget.SelectedValue.IndexOf('|'));
                staticTarget.DESCR_LONG = staticTarget.DESCR_SHORT = ddlTarget.SelectedItem.Text;
                if (!string.IsNullOrEmpty(tbTargetDescLong.Text))
                    staticTarget.DESCR_LONG = tbTargetDescLong.Text;
                staticTarget.SSTAT = ddlStatType.SelectedValue;
                staticTarget.EFF_YEAR = Convert.ToInt32(ddlEffYear.SelectedValue);
                staticTarget.STATUS = ddlTargetStatus.SelectedValue;

                if (btnABSMetric.Checked)
                    staticTarget.DATE_SPAN = 0;
                else if (btnYTDMetric.Checked)
                    staticTarget.DATE_SPAN = 1;
                else
                    staticTarget.DATE_SPAN = 2;
               
                decimal value;
                if (decimal.TryParse(tbTargetValue.Text, out value))
                {
                    staticTarget.TARGET_VALUE = value;
                    staticTarget.VALUE_IND = true;
                }
                else
                {
                    staticTarget.TARGET_VALUE = Convert.ToDecimal(null);
                    staticTarget.VALUE_IND = false;
                }

                if (btnTargetMax.Checked)
                    staticTarget.MIN_MAX = 1;
                else
                    staticTarget.MIN_MAX = 0;

                if ((staticTarget = ViewModel.UpdateTarget(entities, staticTarget, SessionManager.UserContext.UserName())) != null)
                {
                    PERSPECTIVE_TARGET target = null;
                    if ((target = targetList.FirstOrDefault(l => l.TARGET_ID == staticTarget.TARGET_ID)) == null)
                        targetList.Add(staticTarget);
                    else
                    {
                        if (staticTarget.EntityState == System.Data.EntityState.Detached || staticTarget.EntityState == System.Data.EntityState.Deleted)
                            targetList.Remove(target);
                        else
                            target = (PERSPECTIVE_TARGET)SQMModelMgr.CopyObjectValues(target, staticTarget, false);
                    }
                    BindTargetList(targetList);
                }
            }
            else
            {
                staticTarget = new PERSPECTIVE_TARGET();
                BindTarget(staticTarget);
            }

            pnlTargetEdit.Enabled = false;
            btnTargetSave.Enabled = btnTargetCancel.Enabled = false;
            udpList.Update();
            udpTarget.Update();
        }

        protected void onMinMaxClick(object o, EventArgs e)
        {
            RadButton btn = (RadButton)o;
            if (btn.CommandArgument == "0")
                btnTargetMax.Checked = false;
            else
                btnTargetMin.Checked = false;
        }
        protected void onTimeSpanClick(object o, EventArgs e)
        {
            RadButton btn = (RadButton)o;
            switch (btn.CommandArgument)
            {
                case "1":
                    btnABSMetric.Checked = btnYOYMetric.Checked = false;
                    break;
                case "2":
                    btnABSMetric.Checked = btnYTDMetric.Checked = false;
                    break;
                default:
                    btnYTDMetric.Checked = btnYOYMetric.Checked = false;
                    break;
            }
        }

        public void gvList_OnTargetRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfTargetValue");
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblTargetValue");
                    lbl.Text = SQMBasePage.FormatValue(Convert.ToDecimal(hf.Value), 4);
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}