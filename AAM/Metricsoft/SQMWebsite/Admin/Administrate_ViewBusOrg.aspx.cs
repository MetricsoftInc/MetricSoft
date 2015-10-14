using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;

namespace SQM.Website
{
    public partial class Administrate_ViewBusOrg : SQMBasePage
    {
      
        bool initSearch;
     
        public bool isEditMode
        {
            get { return ViewState["isEditMode"] == null ? false : (bool)ViewState["isEditMode"]; }
            set { ViewState["isEditMode"] = value; }
        }

        #region events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnCancelClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnReturnClick += uclSearchBar_OnSearchClick;

            uclBusLoc.OnBusinessLocationChange += OnBusinessLocationSelect;
            uclBusLoc.OnBusinessLocationAdd += OnBusinessLocationAdd;
            uclBusLoc.OnCompanyChange += OnBusinessLocationSelect;
            uclBusLoc.OnCompanyClick += uclBusLocOnCompanyClick;
            uclBusLoc.OnUsersClick += uclBusLocOnUsersClick;

            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;
            uclSubLists.OnDeptListClick += uclAdminList_OnDeptClick;
            uclSubLists.OnAddDeptClick += uclAdminList_OnAddDeptClick;
            uclSubLists.OnLaborListClick += uclAdminList_OnLaborClick;
            uclSubLists.OnAddLaborClick += uclAdminList_OnAddLaborClick;
 
            uclAdminEdit.OnEditSaveClick += uclAdminEdit_OnSaveClick;
            uclAdminEdit.OnEditCancelClick += uclAdminEdit_OnCancelClick;

			uclNotifyList.OnNotifyActionCommand += UpdateNotifyActionList;
        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
           tab_Click(tabID, cmdArg);
        }

        private void uclSearchBar_OnCancelClick()
        {
            SessionManager.EffLocation.BusinessOrg = null;
            ClearTempData();
            uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
            ResetControlValues(hfBase.FindControl("divPartEdit").Controls);
            divPageBody.Visible = false;
        }

        private void uclSearchBar_OnSearchClick()
        {
            initSearch = true;
            divPageBody.Visible = uclAdminTabs.BusOrgPanel.Visible = false;
            pnlSearchList.Visible = true;
            uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
            uclSearchBar.PageTitle.Text = lbViewBusStructTitle.Text;

            if (SessionManager.EffLocation == null)
            {
                uclBusLoc.BindBusinessLocation(SessionManager.UserContext.HRLocation, true, true, false, true);
                uclBusLoc.RefreshOrgList(SessionManager.UserContext.HRLocation);
            }
            else
            {
                uclBusLoc.BindBusinessLocation(SessionManager.EffLocation, true, true, false, true);
                uclBusLoc.RefreshOrgList(SessionManager.EffLocation);
            }
		}

        protected void lbCancel_Click(object sender, EventArgs e)
        {
           // uclSearchBar.SetButtonsNotClicked();
            uclSearchBar_OnSearchClick();
        }
        protected void lbSave_Click(object sender, EventArgs e)
        {
            SaveBusOrg();
            DisplayBusOrg();
        }

        protected void uclBusLocOnCompanyClick(decimal companyID)
        {
            Response.Redirect("/Admin/Administrate_Company.aspx");
        }

        protected void uclBusLocOnUsersClick(decimal companyID)
        {
            Response.Redirect("/Admin/Administrate_ViewUser.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ClearTempData();
                uclSearchBar.PageTitle.Text = lbViewBusStructTitle.Text;
                uclSearchBar.ReturnButton.Text = lblViewBusOrgText.Text;
                divPageBody.Visible = pnlSearchList.Visible = false;
                uclSearchBar.SetButtonsVisible(false, false, false, false, false, true);
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
                SetupPage();
                if (SessionManager.ReturnStatus == true)
                {
                    SessionManager.ReturnStatus = false;   // return from plant page
                    SessionManager.EffLocation.Plant = null;
                    uclSearchBar_OnSearchClick();
                }
                else
                {
                    if (!initSearch  ||  SessionManager.EffLocation == null)
                    {
                        // display the full busorg list for the active company upon 1st entering the page
                        uclSearchBar_OnSearchClick();
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
					ucl.BindDocumentSelect("SYS", 10, true, false, hfDocviewMessage.Value);
                }
            }
        }

        protected void OnBusinessLocationAdd(BusinessLocation location)
        {
            if (location.BusinessOrg != null)
            {
                SessionManager.EffLocation = location;
                SetLocalOrg(null);
                uclSearchBar.SetButtonsNotClicked();
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
                Response.Redirect("/Admin/Administrate_ViewPlant.aspx");
            }
        }

        protected void OnBusinessLocationSelect(BusinessLocation location)
        {
            if (location.BusinessOrg == null)
            {
                SessionManager.EffLocation = location;
              //  uclSearchBar.TitleItem.Text =  "- " + location.Company.COMPANY_NAME;
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
            }
            else if (location.Plant == null)
            {
                SessionManager.EffLocation = location;
                pnlSearchList.Visible = uclBusLoc.LocationSelectPanel.Visible = false;
                divPageBody.Visible = uclAdminTabs.BusOrgPanel.Visible = true;
                uclSearchBar.SetButtonsNotClicked();
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
                SetupPage();
                tab_Click("lbBusOrgDetail_tab", "");
            }
            else
            {
                SessionManager.EffLocation = location;
                SetLocalOrg(null);
                uclSearchBar.SetButtonsNotClicked();
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
                Response.Redirect("/Admin/Administrate_ViewPlant.aspx");
            }
        }

        private void ClearTempData()
        {
            SetLocalOrg(null);
        }

        private void SetupPage()
        {
            if (SessionManager.EffLocation != null && SessionManager.EffLocation.BusinessOrg != null)
            {
                uclItemHdr.DisplayBusOrg(SessionManager.EffLocation.Company, SessionManager.EffLocation.BusinessOrg);
                if (SessionManager.IsEffLocationPrimary())
                    divNavArea.Visible = true;
                else
                    divNavArea.Visible = false;
            }
        }

        private DropDownList SetStatusList(string ddlName, string currentStatus)
        {
            List<Settings> status_codes = SQMSettings.Status;
            DropDownList ddlStatus = (DropDownList)hfBase.FindControl(ddlName);
            ddlStatus.DataSource = status_codes;
            ddlStatus.DataTextField = "short_desc";
            ddlStatus.DataValueField = "code";
            ddlStatus.DataBind();

            if (!string.IsNullOrEmpty(currentStatus))
            {
                ddlStatus.SelectedValue = currentStatus;
            }

            return ddlStatus;
        }

        protected void wasChanged(object sender, EventArgs e)
        {
            ;
        }

        protected void tab_Click(object sender, EventArgs e)
        {
        }
        protected void tab_Click(string tabID, string cmdArg)
        {
            pnlSubLists.Visible = pnlPartProgram.Visible = pnlBusOrgEdit.Visible = pnlAdminEdit.Visible = pnlEscalation.Visible = false;

            if (tabID != null)
            {
                SetActiveTab(SessionManager.CurrentSecondaryTab = tabID);

                BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;

                switch (cmdArg)
                {
                    case "dept":
                        DoDeptList();
                        pnlSubLists.Visible = true;
                        break;
                    case "labor":
                        DoLaborList();
                        pnlSubLists.Visible = true;
                        break;
                    case "prog":
                        isEditMode = true;
                        uclProgramList.BindProgramList(SQMModelMgr.SelectPartProgramList(0, busOrg.BUS_ORG_ID));
                        pnlPartProgram.Visible = true;
                        break;
                    case "notify":
                        pnlEscalation.Visible = true;
						UpdateNotifyActionList("");
                        break;
                    default:
                        pnlBusOrgEdit.Visible = true;
                        DisplayBusOrg();
                        break;
                }
            }
        }

		private void UpdateNotifyActionList(string cmd)
		{
			uclNotifyList.BindNotfyPlan(SQMModelMgr.SelectNotifyActionList(entities, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, null).OrderBy(n => n.NOTIFY_SCOPE).ThenBy(n=> n.SCOPE_TASK).ToList(), SessionManager.EffLocation, "busorg");
		}

        private void uclAdminEdit_OnSaveClick(string cmd)
        {
            switch (cmd)
            {
                case "dept":
                    SaveDept();
                    break;
                case "labor":
                    SaveLabor();
                    break;
                default:
                    break;
            }
        }
        private void uclAdminEdit_OnCancelClick(string cmd)
        {
            switch (cmd)
            {
                case "dept":
                    tab_Click("lbDepartment_tab", "dept");
                    break;
                case "labor":
                    tab_Click("lbLabor_tab", "labor");
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region busorg

        private void DisplayBusOrg()
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;

             DropDownList ddl = (DropDownList)hfBase.FindControl("ddlCurrencyCodes");
             if (ddl.Items.Count == 0)
             {
                 SQMBasePage.FillCurrencyDDL(ddl, "USD");

                 List<BUSINESS_ORG> parent_orgs = SQMModelMgr.SelectBusOrgList(entities, SessionManager.EffLocation.Company.COMPANY_ID, 0, true);
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

             if (busOrg != null)
             {
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
                 SetFindControlValue("ddlCurrencyCodes", hfBase, busOrg.PREFERRED_CURRENCY_CODE);
                 SetFindControlValue("ddlStatus", hfBase, busOrg.STATUS);
                 if (busOrg.PARENT_BUS_ORG_ID > 0)
                     SetFindControlValue("ddlParentBusOrg", hfBase, busOrg.PARENT_BUS_ORG_ID.ToString());
                 SetFindControlValue("lblLastUpdate_out", hfBase, busOrg.LAST_UPD_BY);
                 lblLastUpdateDate_out.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)busOrg.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false);
             }
        }

        protected void SaveBusOrg()
        {
            BUSINESS_ORG bu = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
            BUSINESS_ORG busOrg = null;

            if (!Page.IsValid)
                return;

            if (bu == null)
            {
                busOrg = SQMModelMgr.CreateBusOrg(entities, SessionManager.EffLocation.Company.COMPANY_ID, "");
            }
            else
            {
                busOrg = SQMModelMgr.LookupBusOrg(entities, SessionManager.EffLocation.Company.COMPANY_ID, bu.BUS_ORG_ID);
            }

            bool success;
            decimal decVal;
            busOrg.ORG_NAME = GetFindControlValue("tbOrgname", hfBase, out success);
            busOrg.DUNS_CODE = GetFindControlValue("tbOrgLocCode", hfBase, out success);
            busOrg.PREFERRED_CURRENCY_CODE = GetFindControlValue("ddlCurrencyCodes", hfBase, out success);
            busOrg.STATUS = GetFindControlValue("ddlStatus", hfBase, out success);
            string sel = GetFindControlValue("ddlParentBusOrg", hfBase, out success);
            if (string.IsNullOrEmpty(sel))
                busOrg.PARENT_BUS_ORG_ID = Convert.ToInt32(null);
            else
                busOrg.PARENT_BUS_ORG_ID = Int32.Parse(sel);

            busOrg = (BUSINESS_ORG)SQMModelMgr.SetObjectTimestamp((object)busOrg, SessionManager.UserContext.UserName(), busOrg.EntityState);


            entities.SaveChanges();

            SessionManager.EffLocation.BusinessOrg = busOrg;

           // SetupPage();

        }
        #endregion

        #region plant

        public void gvList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region department

        private void DoDeptList()
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
          
            LocalOrg().DeptList = SQMModelMgr.SelectDepartmentList(entities, SessionManager.EffLocation.Company.COMPANY_ID, busOrg.BUS_ORG_ID, 0);
            
            uclSubLists.BindDeptList(LocalOrg().DeptList);
        }

        protected void uclAdminList_OnAddDeptClick(decimal deptID)
        {
            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindDeptartment(new DEPARTMENT());
        }

        protected void uclAdminList_OnDeptClick(decimal deptID)
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
            DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, SessionManager.EffLocation.Company.COMPANY_ID, busOrg.BUS_ORG_ID, 0, deptID, "", false);

            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindDeptartment(dept);
            LocalOrg().EditObject = dept;
        }

        protected void SaveDept()
        {
             bool success;

            if (uclAdminEdit.IsNew)
            {
                BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
                DEPARTMENT deptNew = new DEPARTMENT();
                deptNew = uclAdminEdit.ReadDepartment(deptNew);
                LocalOrg().DeptList.Add(SQMModelMgr.CreateDepartment(entities, busOrg, deptNew, SessionManager.UserContext.UserName()));
            }
            else
            {
                DEPARTMENT dept = (DEPARTMENT)LocalOrg().EditObject;
                dept = uclAdminEdit.ReadDepartment(dept);
                SQMModelMgr.UpdateDepartment(entities, dept, SessionManager.UserContext.UserName());
                LocalOrg().DeptList[LocalOrg().DeptList.FindIndex(d => (d.DEPT_ID == dept.DEPT_ID))] = dept;
            }

            LocalOrg().EditObject = null;
            DoDeptList();
            pnlAdminEdit.Visible = false;
        }

        #endregion

        #region labor

        private void DoLaborList()
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
            
            LocalOrg().LaborList = SQMModelMgr.SelectLaborTypeList(entities, SessionManager.EffLocation.Company.COMPANY_ID, busOrg.BUS_ORG_ID, 0);
            
            uclSubLists.BindLaborList(LocalOrg().LaborList);
        }

        protected void uclAdminList_OnAddLaborClick(decimal deptID)
        {
            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindLaborType(new LABOR_TYPE());
        }

        protected void uclAdminList_OnLaborClick(decimal laborID)
        {
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
            LABOR_TYPE labor = SQMModelMgr.LookupLaborType(entities, SessionManager.EffLocation.Company.COMPANY_ID, busOrg.BUS_ORG_ID, 0, laborID, "", false);

            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindLaborType(labor);
            LocalOrg().EditObject = labor;
        }

        protected void SaveLabor()
        {
            bool success;

            if (uclAdminEdit.IsNew)
            {
                BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;
                LABOR_TYPE laborNew = new LABOR_TYPE();
                laborNew = uclAdminEdit.ReadLaborType(laborNew);
                LocalOrg().LaborList.Add(SQMModelMgr.CreateLaborType(entities, busOrg, laborNew, SessionManager.UserContext.UserName()));
            }
            else
            {
                LABOR_TYPE labor = (LABOR_TYPE)LocalOrg().EditObject;
                labor = SQMModelMgr.LookupLaborType(entities, (decimal)labor.COMPANY_ID, (decimal)labor.BUS_ORG_ID, 0, (decimal)labor.LABOR_TYP_ID, "", false);
                labor = uclAdminEdit.ReadLaborType(labor);
                SQMModelMgr.UpdateLaborType(entities, labor, SessionManager.UserContext.UserName());
                LocalOrg().LaborList[LocalOrg().LaborList.FindIndex(d => (d.LABOR_TYP_ID == labor.LABOR_TYP_ID))] = labor;
            }

            LocalOrg().EditObject = null;
            DoLaborList();
            pnlAdminEdit.Visible = false;
        }

        #endregion

        #region prodlinelist
        private void DoProdlineList()
        {
            LocalOrg().ProdLineList = SQMModelMgr.SelectProductLineList(entities, SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID);

            BindDataList((GridView)hfBase.FindControl("gvProdlineList"), LocalOrg().ProdLineList);
        }

        private void BindDataList(GridView gv, List<PRODUCT_LINE> theList)
        {
            gv.DataSource = theList;
            gv.DataBind();
            SetGridViewDisplay(gv, (Label)hfBase.FindControl("lblProdLineListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divProdLineGVScroll"), 20);
        }

        public void gvProdlineList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.TextBox tbx = new TextBox();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    tbx = (TextBox)e.Row.Cells[0].FindControl("tbProdlineCode");
                    lbl = (Label)e.Row.Cells[0].FindControl("lbProdlineCode");
                    if (isEditMode)
                    {
                        lbl.Visible = false;
                        tbx.Visible = true;
                    }
                    else
                        lbl.Visible = true;

                    tbx = (TextBox)e.Row.Cells[0].FindControl("tbProdlineDesc");
                    lbl = (Label)e.Row.Cells[0].FindControl("lbProdlineDesc");
                    if (isEditMode)
                    {
                        lbl.Visible = false;
                        tbx.Visible = true;
                    }
                    else
                        lbl.Visible = true;
                }
                catch
                {
                }
            }
        }

        protected void lnkProdLineAdd_Click(object sender, EventArgs e)
        {
            PRODUCT_LINE newProd = new PRODUCT_LINE();
            LocalOrg().ProdLineList.Add(newProd);
            GridView gv = (GridView)hfBase.FindControl("gvProdlineList");
            BindDataList(gv, LocalOrg().ProdLineList);
            gv.Rows[gv.Rows.Count - 1].FindControl("tbProdLineCode").Focus();
        }

        protected void lbSaveProdLineList_Click(object sender, EventArgs e)
        {
            GridView gv = (GridView)hfBase.FindControl("gvProdlineList");
            TextBox tb;
            CheckBox cb;
            PRODUCT_LINE prod;
            int nrow = -1;

            List<PRODUCT_LINE> updateList = new List<PRODUCT_LINE>();

            foreach (GridViewRow row in gv.Rows)
            {
                if (++nrow > LocalOrg().ProdLineList.Count)
                    LocalOrg().ProdLineList.Add(new PRODUCT_LINE());
                
                prod = LocalOrg().ProdLineList[nrow];
                cb = (CheckBox)row.FindControl("cbStatus");
                if (cb.Checked)
                {
                    prod.STATUS = cb.Checked.ToString();
                }
                else 
                {
                    tb = (TextBox)row.FindControl("tbProdlineCode");
                    prod.PRODUCT_LINE_CODE = tb.Text;

                    tb = (TextBox)row.FindControl("tbProdlineDesc");
                    prod.PRODUCT_LINE_DESC = tb.Text;
                }
            }

            SQMModelMgr.UpdateProductLineList(entities, SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, LocalOrg().ProdLineList);
            AlertUpdateResult(SQMModelMgr.updateStatus);
            LocalOrg().ProdLineList.Clear();
            DoProdlineList();
        }
        #endregion

        // manage current session object  (formerly was page static variable)
        OrgData LocalOrg()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is OrgData)
            {
            }
            else
            {
                SessionManager.CurrentObject = new OrgData().Initialize();
            }
            
            return (OrgData)SessionManager.CurrentObject;
        }

        OrgData SetLocalOrg(OrgData orgdata)
        {
            SessionManager.CurrentObject = orgdata;
            return LocalOrg();
        }

    }
}
