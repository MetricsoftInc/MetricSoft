using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Quality_Resources : SQMBasePage
    {
        static NONCONFORMANCE staticNonconf;
        static List<NONCONFORMANCE> nonconfList;
        static List<NONCONFORMANCE> categoryList;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			this.lblCategory.Text = Resources.LocalizedText.ProblemArea + ": ";

            if (!Page.IsPostBack)
            {
                uclAdminTabs.QualityResourcePanel.Visible = true;
                if (string.IsNullOrEmpty(hfActiveTab.Value))
                    tab_Click("lbQSDocs_tab", "");
            }
            else 
            {
                if (SessionManager.CurrentSecondaryTab == "lbQSDocs_tab")
                    uclDocMgr.BindDocMgr("SQM", 0, 0);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("SQM", 2, true, true, "");
                }
            }
            SessionManager.ClearReturns();  // clear any sesion objects from document upload popup
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        protected void tab_Click(string tabID, string cmdArg)
        {
            if (tabID != null)
            {
                // setup for ps_admin.js to toggle the tab active/inactive display
                SetActiveTab(SessionManager.CurrentSecondaryTab = hfActiveTab.Value = tabID);

                uclDocMgr.DocMgrPnl.Visible =  pnlNonconfList.Visible = false;

                switch (tabID)
                {
                    case "lbQSNonConf_tab":
                        pnlNonconfList.Visible = true;
                        SetupPage();
                        break;
                    default:
                        uclAdminTabs.QualityResourcePanel.Visible = true;
                        uclDocMgr.BindDocMgr("SQM", 0, 0);
                        break;
                }
            }
        }

        private void SetupPage()
        {
            switch (SessionManager.CurrentSecondaryTab)
            {
                case "lbQSNonConf_tab":
                    if (ddlNonconfStatus.Items.Count == 0)
                        ddlNonconfStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCode", "short"));
                    if (ddlProblemArea.Items.Count == 0)
                    {
                        ddlProblemArea.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("problemArea"));
                        ddlProblemArea.SelectedIndex = 0;
                    }
                    BindCategoryList();
                    nonconfList = SQMResourcesMgr.SelectNonconfList(ddlProblemArea.SelectedValue, false);
                    BindNonconfList(nonconfList);
                    break;
                default:
                    break;
            }
        }

        #region nonconformance
        protected void SelectProblemArea(object sender, EventArgs e)
        {
            nonconfList.Clear();
            categoryList.Clear();
            SetupPage();
        }

        private void BindCategoryList()
        {
            if (categoryList == null || categoryList.Count == 0)
                categoryList = SQMResourcesMgr.SelectNonconfCategoryList(ddlProblemArea.SelectedValue);
            ddlNonconfCategory.DataSource = categoryList;
            ddlNonconfCategory.DataValueField = "NONCONF_CD";
            ddlNonconfCategory.DataTextField = "NONCONF_NAME";
            ddlNonconfCategory.DataBind();
            ddlNonconfCategory.Items.Insert(0, new ListItem(""));
        }

        public void BindNonconfList(List<NONCONFORMANCE> theList)
        {
            //ToggleVisible(pnlMeasureList);
            pnlNonconfList.Visible = true;

            if (categoryList == null || categoryList.Count == 0)
                categoryList = SQMResourcesMgr.SelectNonconfCategoryList(ddlProblemArea.SelectedValue);

            if (categoryList.Count > 0)
            {
                gvNonconfCatList.DataSource = categoryList.OrderBy(l => l.NONCONF_NAME);
                gvNonconfCatList.DataBind();
                SetGridViewDisplay(gvNonconfCatList, lblNonconfListEmpty, divNonconfGVScroll, 20, 0);
            }
            else
            {
                List<NONCONFORMANCE> lst = new List<NONCONFORMANCE>();
                lst.Add(new NONCONFORMANCE());
                gvNonconfCatList.DataSource = lst;
                gvNonconfCatList.DataBind();
                lblNonconfListEmpty.Visible = true;
            }

            pnlNonconfEdit.Enabled = btnNonconfCancel.Enabled = btnNonconfSave.Enabled = false;
        }

        protected void lnkNonconfList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal nonconfID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindNonconf((staticNonconf = nonconfList.FirstOrDefault(l => l.NONCONF_ID == nonconfID)));
            lblAddNonconf.Visible = true;
        }

        public int BindNonconf(NONCONFORMANCE nonconf)
        {
            int status = 0;

            if (nonconf != null)
            {
                tbNonconfCode.Text = nonconf.NONCONF_CD;
                tbNonconfName.Text = nonconf.NONCONF_NAME;
                tbNonconfDesc.Text = nonconf.NONCONF_DESC;
                ddlNonconfCategory.SelectedValue = nonconf.NONCONF_CATEGORY;
                SQMBasePage.SetStatusList(ddlNonconfStatus, nonconf.STATUS);

                pnlNonconfEdit.Enabled = btnNonconfCancel.Enabled = btnNonconfSave.Enabled = true;
            }

            return status;
        }

        protected void btnNonconfCatSave_Click(object sender, EventArgs e)
        {
            bool success;
            NONCONFORMANCE nonconfCat = null;

            if (!string.IsNullOrEmpty(tbNonconfCategoryCode.Text))
            {
                nonconfCat = new NONCONFORMANCE();
                nonconfCat.NONCONF_CD = tbNonconfCategoryCode.Text;
                nonconfCat.NONCONF_NAME = nonconfCat.NONCONF_DESC = tbNonconfCategoryName.Text;
                nonconfCat.STATUS = "A";
                nonconfCat.PROBLEM_AREA = ddlProblemArea.SelectedValue;
                if ((nonconfCat = SQMResourcesMgr.UpdateNonconf(entities, nonconfCat, SessionManager.UserContext.UserName())) != null)
                {
                    categoryList.Add(nonconfCat);
                    BindNonconfList(nonconfList);
                    BindCategoryList();
                }
            }
        }

        protected void btnNonconfSave_Click(object sender, EventArgs e)
        {
            bool success;

            if (hfOper.Value == "add")
            {
                staticNonconf = new NONCONFORMANCE();
                staticNonconf.COMPANY_ID = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
                staticNonconf.STATUS = "A";
            }
            else
            {
                staticNonconf = SQMResourcesMgr.LookupNonconf(entities, staticNonconf.NONCONF_ID, "");
            }

            btnNonconfSave.Enabled = false;
            hfOper.Value = "";

            staticNonconf.PROBLEM_AREA = ddlProblemArea.SelectedValue;
            staticNonconf.NONCONF_CATEGORY = ddlNonconfCategory.SelectedValue;
            staticNonconf.NONCONF_CD = tbNonconfCode.Text;
            staticNonconf.NONCONF_NAME = tbNonconfName.Text;
            staticNonconf.NONCONF_DESC = tbNonconfDesc.Text;
            staticNonconf.STATUS = ddlNonconfStatus.SelectedValue;

            if ((staticNonconf = SQMResourcesMgr.UpdateNonconf(entities, staticNonconf, SessionManager.UserContext.UserName())) != null)
            {
                NONCONFORMANCE nonconf = null;
                if ((nonconf = nonconfList.FirstOrDefault(l => l.NONCONF_ID == staticNonconf.NONCONF_ID)) == null)
                    nonconfList.Add(staticNonconf);
                else
                {
                    if (staticNonconf.EntityState == System.Data.EntityState.Detached || staticNonconf.EntityState == System.Data.EntityState.Deleted)
                        nonconfList.Remove(nonconf);
                    else
                        nonconf = (NONCONFORMANCE)SQMModelMgr.CopyObjectValues(nonconf, staticNonconf, false);
                }
                BindNonconfList(nonconfList);
            }
        }

        public void gvList_OnNonconfCatRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblNonconfCat");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfNonconfCat");
                    NONCONFORMANCE nonconf = categoryList.FirstOrDefault(l => l.NONCONF_CD == hf.Value);
                    if (nonconf != null)
                        lbl.Text = nonconf.NONCONF_NAME;
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvNonconfList");
                    gv.DataSource = nonconfList.Where(l => l.NONCONF_CATEGORY == hf.Value).ToList();
                    gv.DataBind();
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }
        public void gvList_OnNonconfRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
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

        #region measure
 
        #endregion
    }
}