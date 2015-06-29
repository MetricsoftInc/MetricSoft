using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_Resources : SQMBasePage
    {
        static EHS_MEASURE staticMeasure;
        static List<EHS_MEASURE> measureList;
        static string staticSubcat;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclAdminTabs.QualityResourcePanel.Visible = true;
            }
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        protected void SelectMeasureCategory(object sender, EventArgs e)
        {
            SetupPage();
        }

        private void SetupPage()
        {

            if (ddlMeasureStatus.Items.Count == 0)
                ddlMeasureStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCode", "short"));

            if (ddlMeasureCategory.Items.Count == 0)
            {
                ddlMeasureCategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureCategory"));
                ddlMeasureCategory.SelectedIndex = 0;
            }

            if (ddlMeasureSubcategory.Items.Count == 0)
            {
                ddlMeasureSubcategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureSubCategory"));
                ddlMeasureSubcategory.SelectedIndex = 0;
            }

            if (ddlMeasureCurrency.Items.Count == 0)
            {
                ddlMeasureCurrency.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("currencyCode"));
                ddlMeasureCurrency.SelectedIndex = 0;
            }

            if (ddlMeasureUOM.Items.Count == 0)
            {
                if (ddlUOMCategory.Items.Count == 0)
                    ddlUOMCategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("UOMCategory"));
                ddlUOMCategory.SelectedIndex = 0;

                DropDownList ddlRef = (DropDownList)hfBase.FindControl("ddlMeasureUOM_ref");
                ddlRef.Items.Clear();
                ddlRef.Items.Insert(0, new ListItem("--select--"));
                foreach (UOM uom in SessionManager.UOMList)
                {
                    ddlRef.Items.Add(new ListItem(uom.UOM_NAME, (uom.UOM_CATEGORY + "|" + uom.UOM_ID.ToString())));
                }
            }

            measureList = SQMResourcesMgr.SelectEHSMeasureList(ddlMeasureCategory.SelectedValue, false);
            BindMeasureList(measureList);
        }

        protected void tab_Click(string tabID, string cmdArg)
        {
            if (tabID != null)
            {
                // setup for ps_admin.js to toggle the tab active/inactive display
                SessionManager.CurrentSecondaryTab = tabID;
                HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveSecondaryTab");
                hfld.Value = SessionManager.CurrentSecondaryTab;

                switch (tabID)
                {
                    case "lbEHSMeasure_tab":
                        SetupPage();
                        break;
                    case "lbQSMeasure_tab":
                        BindMeasureList(SQMResourcesMgr.SelectEHSMeasureList("DV", false));
                        break;
                    default:
                        break;
                }
            }
        }

        #region measure

        public void BindMeasureList(List<EHS_MEASURE> theList)
        {
            //ToggleVisible(pnlMeasureList);
            pnlMeasureList.Visible = true;
            staticSubcat = "";
            gvMeasureList.DataSource = theList.OrderBy(l => l.MEASURE_SUBCATEGORY).ThenBy(l => l.MEASURE_CD);
            gvMeasureList.DataBind();
            SetGridViewDisplay(gvMeasureList, lblMeasureListEmpty, divMeasureGVScroll, 5, 0);
            pnlMeasureEdit.Enabled = btnMeasureCancel.Enabled = btnMeasureSave.Enabled = false;
        }

        protected void lnkMeasureList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal measureID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindMeasure((staticMeasure = measureList.FirstOrDefault(l=> l.MEASURE_ID == measureID)));
            btnMeasureCancel.OnClientClick = "CancelMeasure()";
        }

        public int BindMeasure(EHS_MEASURE measure)
        {
            int status = 0;

            if (measure != null)
            {
                tbMeasureCode.Text = measure.MEASURE_CD;
                tbMeasureName.Text = measure.MEASURE_NAME;
                tbMeasureDesc.Text = measure.MEASURE_DESC;
                ddlMeasureSubcategory.SelectedValue = measure.MEASURE_SUBCATEGORY;
                SQMBasePage.SetStatusList(ddlMeasureStatus, measure.STATUS);
                UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == measure.STD_UOM);
                if (uom != null)
                {
                    SetFindControlValue("hfUOMCategory_out", hfBase, uom.UOM_CATEGORY);
                    SetFindControlValue("hfMeasureUOM_out", hfBase, (uom.UOM_CATEGORY + "|" + measure.STD_UOM.ToString()));
                    ScriptManager.RegisterStartupScript(this, GetType(),"filterlist", "filterDependentList('ddlUOMCategory', 'ddlMeasureUOM', 'hfUOMCategory_out', 'hfMeasureUOM_out');", true);
                    ddlUOMCategory.SelectedValue = uom.UOM_CATEGORY;
                    ddlMeasureUOM.SelectedValue = measure.STD_UOM.ToString();
                }
                pnlMeasureEdit.Enabled = btnMeasureCancel.Enabled = btnMeasureSave.Enabled = true;
            }

            return status;
        }

        protected void btnMeasureSave_Click(object sender, EventArgs e)
        {
            bool success;

            if (hfOper.Value == "add")
            {
                staticMeasure = new EHS_MEASURE();
                staticMeasure.COMPANY_ID = SessionManager.SessionContext.ActiveCompany().COMPANY_ID;
                staticMeasure.STATUS = "A";
            }
            else
            {
                staticMeasure = SQMResourcesMgr.LookupEHSMeasure(entities, staticMeasure.MEASURE_ID, "");
            }

            btnMeasureSave.Enabled = false;
            hfOper.Value = "";

            staticMeasure.MEASURE_CATEGORY = ddlMeasureCategory.SelectedValue;
            staticMeasure.STD_CURRENCY_CODE = ddlMeasureCurrency.SelectedValue;
            staticMeasure.MEASURE_SUBCATEGORY = ddlMeasureSubcategory.SelectedValue;
            staticMeasure.STATUS = ddlMeasureStatus.SelectedValue;
            staticMeasure.STD_UOM = Convert.ToDecimal(null);
            string sel = GetFindControlValue("hfMeasureUOM_out", hfBase, out success);
            if (!string.IsNullOrEmpty(sel))
            {
                string[] parms = sel.Split('|');
                if (parms.Length > 1)
                    staticMeasure.STD_UOM = Convert.ToDecimal(parms[1]);
            }

            staticMeasure.MEASURE_CD = tbMeasureCode.Text;
            staticMeasure.MEASURE_NAME = tbMeasureName.Text;
            staticMeasure.MEASURE_DESC = tbMeasureDesc.Text;

            if ((staticMeasure = SQMResourcesMgr.UpdateEHSMeasure(entities, staticMeasure, SessionManager.UserContext.UserName())) != null)
            {
                EHS_MEASURE measure;
                if ((measure=measureList.FirstOrDefault(l => l.MEASURE_ID == staticMeasure.MEASURE_ID)) == null)
                    measureList.Add(staticMeasure);
                else
                {
                    if (staticMeasure.EntityState == System.Data.EntityState.Detached || staticMeasure.EntityState == System.Data.EntityState.Deleted)
                        measureList.Remove(measure);
                    else
                        measure = (EHS_MEASURE)SQMModelMgr.CopyObjectValues(measure, staticMeasure, false);
                }
                BindMeasureList(measureList);
            }
        }

        #endregion

        public void gvList_OnMeasureRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hf.Value);

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureSubcat");
                    if (hf.Value != staticSubcat)
                    {
                        staticSubcat = hf.Value;
                        lbl = (Label)e.Row.Cells[0].FindControl("lblMeasureSubcat");
                        lbl.Text =  WebSiteCommon.GetXlatValue("measureSubCategory",hf.Value);
                        e.Row.Cells[0].Style.Add("BORDER-TOP", "#787878 1px solid");
                    }

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureUOM");
                    lbl = (Label)e.Row.Cells[0].FindControl("lblMeasureUOM_out");
                    UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == Convert.ToDecimal(hf.Value));
                    if (uom != null)
                        lbl.Text = uom.UOM_CD;

                    for (int n = 1; n <= e.Row.Cells.Count; n++)
                    {
                        e.Row.Cells[n].Style.Add("BORDER-TOP", "#787878 1px solid");
                    }
                }
                catch
                {
                }
            }
        }
    }
}