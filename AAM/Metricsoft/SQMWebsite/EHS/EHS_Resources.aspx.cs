using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class EHS_Resources : SQMBasePage
    {
        public List<EHS_MEASURE> measureList
        {
            get { return ViewState["MeasureList"] == null ? new List<EHS_MEASURE>() : (List<EHS_MEASURE>)ViewState["MeasureList"]; }
            set { ViewState["MeasureList"] = value; }
        }
        public List<EHS_MEASURE> subList
        {
            get { return ViewState["SubCatList"] == null ? new List<EHS_MEASURE>() : (List<EHS_MEASURE>)ViewState["SubCatList"]; }
            set { ViewState["SubCatList"] = value; }
        }
        public EHS_MEASURE staticMeasure
        {
            get { return ViewState["Measure"] == null ? new EHS_MEASURE() : (EHS_MEASURE)ViewState["Measure"]; }
            set { ViewState["Measure"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclAdminTabs.EHSResourcePanel.Visible = true;
                if (string.IsNullOrEmpty(hfActiveTab.Value))
                    tab_Click("lbEHSDocs_tab", "");
            }
            else
            {
                if (SessionManager.CurrentSecondaryTab.Equals("lbEHSDocs_tab") || SessionManager.CurrentSecondaryTab.Equals(""))
                    uclDocMgr.BindDocMgr("EHS", 0, 0);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("EHS", 2, true, true, "");
                }
            }
            SessionManager.ClearReturns();  // clear any sesion objects from document upload popup
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        protected void ddlMeasureCategoryChanged(object sender, EventArgs e)
        {
            subList.Clear();
            SetupPage();
        }

        private void SetupPage()
        {
            // this is really the Measures tab

            ToggleVisible(pnlMeasureList);
            if (ddlMeasureStatus.Items.Count == 0)
                ddlMeasureStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCodeDelete", "short"));

            if (ddlMeasureCategory.Items.Count < 2)
            {
                ddlMeasureCategory.Items.Clear();
                ddlMeasureCategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureCategoryEHS"));
                ddlMeasureCategory.Items.Insert(0, new ListItem("-all categories-",""));
                ddlMeasureCategory.SelectedValue = "";
                lnkMeasureNew.Visible = false;
            }
  
            if (ddlMeasureEFMType.Items.Count == 0)
            {
                ddlMeasureEFMType.DataSource = EHSModel.SelectEFMTypeList("");
                ddlMeasureEFMType.DataValueField = "EFM_TYPE";
                ddlMeasureEFMType.DataTextField = "DESCRIPTION";
                ddlMeasureEFMType.DataBind();
                ddlMeasureEFMType.Items.Insert(0, new ListItem(""));
            }

            if (ddlOutputUOM.Items.Count == 0)
            {
                string[] uomcats = {"FACT"};
                foreach (UOM uom in SessionManager.UOMList.Where(l => uomcats.Contains(l.UOM_CATEGORY)).ToList())
                {
                    ddlOutputUOM.Items.Add(new ListItem(uom.UOM_DESC, uom.UOM_ID.ToString()));
                }
                ddlOutputUOM.Items.Insert(0,new ListItem("", ""));
            }

            measureList = EHSModel.SelectEHSMeasureList(ddlMeasureCategory.SelectedValue, false);
            BindMeasureList(measureList);
        }

        private void BindSubcategoryList(string category)
        {
            if (!string.IsNullOrEmpty(category))
                subList = EHSModel.SelectEHSMeasureSubCategoryList(category);
            else 
                subList = EHSModel.SelectEHSMeasureSubCategoryList(ddlMeasureCategory.SelectedValue);

            if (subList != null && subList.Count > 0)
            {
                ddlMeasureSubcategory.DataSource = subList;
                ddlMeasureSubcategory.DataValueField = "MEASURE_CD";
                ddlMeasureSubcategory.DataTextField = "MEASURE_NAME";
                ddlMeasureSubcategory.DataBind();

                if (ddlMeasureCategory.SelectedIndex > 0)
                    lnkMeasureNew.Visible = true;
                else
                    lnkMeasureNew.Visible = false;

                if (ddlMeasureCategory.SelectedValue != "PROD" && ddlMeasureCategory.SelectedValue != "SAFE" && ddlMeasureCategory.SelectedValue != "FACT")
                    ddlMeasureSubcategory.Items.Insert(0, new ListItem(""));
                ddlMeasureSubcategory.Enabled = true;
            }
        }

        protected void tab_Click(string tabID, string cmdArg)
        {
            if (tabID != null)
            {
                // setup for ps_admin.js to toggle the tab active/inactive display
                SetActiveTab(SessionManager.CurrentSecondaryTab = hfActiveTab.Value = tabID);
                 
                switch (tabID)
                {
                    case "lbEHSMeasure_tab":
                        SetupPage();
                        break;
                    case "lbEHSDocs_tab":
                        SetupEHSDocumentPanel();
                        SessionManager.ClearReturns();
                        break;
                    default:
                        break;
                }
            }
        }

        #region measure

        public void BindMeasureList(List<EHS_MEASURE> theList)
        {
            pnlMeasureList.Visible = true;
            pnlMeasureEdit.Visible = false;
            btnMeasureSave.Enabled = btnMeasureCancel.Enabled = false;

            BindSubcategoryList(ddlMeasureCategory.SelectedValue);
            gvMeasureSubcatList.DataSource = EHSModel.SelectEHSMeasureSubCategoryList(ddlMeasureCategory.SelectedValue);
            gvMeasureSubcatList.DataBind();
        }

        protected void lnkMeasureList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal measureID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindMeasure((staticMeasure = measureList.FirstOrDefault(l => l.MEASURE_ID == measureID)));
        }

        protected void AddMeasure(object sender, EventArgs e)
        {
            ClearMeasure(null, null);
            hfOper.Value = "add";
            BindMeasure(null);
        }

        protected void ClearMeasure(object sender, EventArgs e)
        {
            hfOper.Value = "";
            staticMeasure = null;
            tbMeasureCode.Text = tbMeasureDesc.Text = tbMeasureName.Text = "";
            ddlMeasureSubcategory.SelectedIndex = ddlMeasureEFMType.SelectedIndex = ddlMeasureStatus.SelectedIndex = 0;
            btnMeasureCancel.Enabled = btnMeasureSave.Enabled = false;
            pnlMeasureEdit.Visible = false;
        }

        public int BindMeasure(EHS_MEASURE measure)
        {
            int status = 0;
            pnlMeasureEdit.Visible = true;
            DisplayErrorMessage(null);

            BindSubcategoryList(measure != null ? measure.MEASURE_CATEGORY : "");

            trMeasureEFMType.Visible = false;
            phProdTableField.Visible = phSafeTableField.Visible = phOutputUOM.Visible = false;
            ddlMeasureEFMType.SelectedIndex = ddlProdTableField.SelectedIndex = ddlSafeTableField.SelectedIndex = ddlOutputUOM.SelectedIndex = 0;

            if ((measure != null &&  measure.MEASURE_CATEGORY ==  "PROD") ||  ddlMeasureCategory.SelectedValue == "PROD")
                phProdTableField.Visible = true;
            else if ((measure != null && measure.MEASURE_CATEGORY == "SAFE") || ddlMeasureCategory.SelectedValue == "SAFE")
                phSafeTableField.Visible = true;
            else if ((measure != null && measure.MEASURE_CATEGORY == "FACT") || ddlMeasureCategory.SelectedValue == "FACT")
                phOutputUOM.Visible = true;
            else if ((measure != null && measure.MEASURE_CATEGORY == "ENGY") || ddlMeasureCategory.SelectedValue == "ENGY")
                trMeasureEFMType.Visible = true;

            winMeasureEdit.Title = hfAddMeasure.Value;

            if (measure != null)
            {
                winMeasureEdit.Title = hfUpdateMeasure.Value;
                tbMeasureCode.Text = measure.MEASURE_CD;
                tbMeasureName.Text = measure.MEASURE_NAME.Trim();
                tbMeasureDesc.Text = measure.MEASURE_DESC.Trim();
                ddlMeasureSubcategory.SelectedValue = measure.MEASURE_SUBCATEGORY;
                SQMBasePage.SetStatusList(ddlMeasureStatus, measure.STATUS);

                ddlProdTableField.SelectedIndex = ddlSafeTableField.SelectedIndex = ddlOutputUOM.SelectedIndex = 0;
               
                if (!string.IsNullOrEmpty(measure.PLANT_ACCT_FIELD) && ddlProdTableField.Items.FindByValue(measure.PLANT_ACCT_FIELD) != null)
                    ddlProdTableField.SelectedValue = measure.PLANT_ACCT_FIELD;
               
                if (!string.IsNullOrEmpty(measure.PLANT_ACCT_FIELD) && ddlSafeTableField.Items.FindByValue(measure.PLANT_ACCT_FIELD) != null)
                    ddlSafeTableField.SelectedValue = measure.PLANT_ACCT_FIELD;

                if (measure.STD_UOM.HasValue && ddlOutputUOM.Items.FindByValue(measure.STD_UOM.ToString()) != null)
                    ddlOutputUOM.SelectedValue = measure.STD_UOM.ToString();
              
                if (ddlMeasureEFMType.Items.FindByValue(measure.EFM_TYPE) != null)
                    ddlMeasureEFMType.SelectedValue = measure.EFM_TYPE;
            }

            btnMeasureCancel.Enabled = btnMeasureSave.Enabled = true;

            string script = "function f(){OpenMeasureEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
 
            return status;
        }

        protected void btnMeasureSave_Click(object sender, EventArgs e)
        {
            bool success;
            EHS_MEASURE subCategory = null;
 
            if (hfOper.Value == "add")
            {
                staticMeasure = new EHS_MEASURE();
                staticMeasure.COMPANY_ID = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
                staticMeasure.MEASURE_CATEGORY = ddlMeasureCategory.SelectedValue;
            }
            else
                staticMeasure = EHSModel.LookupEHSMeasure(entities, staticMeasure.MEASURE_ID, "");

            staticMeasure.MEASURE_SUBCATEGORY = ddlMeasureSubcategory.SelectedValue;
            staticMeasure.STATUS = ddlMeasureStatus.SelectedValue;
            staticMeasure.MEASURE_CD = tbMeasureCode.Text;
            staticMeasure.MEASURE_NAME = tbMeasureName.Text;
            staticMeasure.MEASURE_DESC = tbMeasureDesc.Text;

            // validate 
            if (hfOper.Value == "add"  &&  measureList.FirstOrDefault(l => l.MEASURE_CD == tbMeasureCode.Text) != null)
            {
                BindMeasure(staticMeasure);
                DisplayErrorMessage(hfDuplicateMeasure);
                return;
            }
            if (string.IsNullOrEmpty(staticMeasure.MEASURE_CD) || string.IsNullOrEmpty(staticMeasure.MEASURE_NAME))
            {
                BindMeasure(staticMeasure);
                DisplayErrorMessage(hfErrRequiredInputs);
                return;
            }

            btnMeasureSave.Enabled = false;

            staticMeasure.PLANT_ACCT_FIELD = "";
            if (staticMeasure.MEASURE_CATEGORY == "PROD")
            {
                staticMeasure.PLANT_ACCT_FIELD = ddlProdTableField.SelectedValue;
            }
            else if (staticMeasure.MEASURE_CATEGORY == "SAFE")
            {
                staticMeasure.PLANT_ACCT_FIELD = ddlSafeTableField.SelectedValue;
            }
            else if (staticMeasure.MEASURE_CATEGORY == "FACT")
            {
                if (ddlOutputUOM.SelectedIndex > 0)
                    staticMeasure.STD_UOM = Convert.ToDecimal(ddlOutputUOM.SelectedValue);
                else
                    staticMeasure.STD_UOM = null;
            }

            if (ddlMeasureEFMType.SelectedIndex == 0)
                staticMeasure.EFM_TYPE = null;
            else
                staticMeasure.EFM_TYPE = ddlMeasureEFMType.SelectedValue;

            hfOper.Value = "";

            if ((staticMeasure = EHSModel.UpdateEHSMeasure(entities, staticMeasure, SessionManager.UserContext.UserName())) != null)
            {
                EHS_MEASURE measure;
                if ((measure = measureList.FirstOrDefault(l => l.MEASURE_ID == staticMeasure.MEASURE_ID)) == null)
                    measureList.Add(staticMeasure);
                else
                {
                    if (staticMeasure.EntityState == System.Data.EntityState.Detached || staticMeasure.EntityState == System.Data.EntityState.Deleted)
                        measureList.Remove(measure);
                    else
                        measure = (EHS_MEASURE)SQMModelMgr.CopyObjectValues(measure, staticMeasure, false);
                }

                ClearMeasure(null, null);
                BindMeasureList(measureList);
            }
        }

        private void DisplayErrorMessage(HiddenField hfMessage)
        {
            if (hfMessage == null)
                lblErrorMessage.Text = "";
            else
                lblErrorMessage.Text = hfMessage.Value;
        }

        public void gvList_OnMeasureSubcatRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblMeasureSubcat");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureSubcat");
                    EHS_MEASURE subcat = subList.FirstOrDefault(l => l.MEASURE_CD == hf.Value);
                    if (subcat != null)
                        lbl.Text = subcat.MEASURE_NAME;
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMeasureList");
                    gv.DataSource = measureList.Where(l => l.MEASURE_SUBCATEGORY == hf.Value).ToList();
                    gv.DataBind();
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        public void gvList_OnMeasureRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hf.Value);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region documents
        public void SetupEHSDocumentPanel()
        {
            ToggleVisible(uclDocMgr.DocMgrPnl);
            uclDocMgr.BindDocMgr("EHS", 0, 0);
        }
        #endregion


        #region common
        public void ToggleVisible(Panel pnlTarget)
        {

            uclDocMgr.DocMgrPnl.Visible = pnlMeasureList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        #endregion
    }
}
