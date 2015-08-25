using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SQM.Shared;

namespace SQM.Website
{
    public partial class Administrate_ViewPlant : SQMBasePage 
    {
      
       // static List<LOCAL_LANGUAGE> langList;

        public bool isEditMode
        {
            get { return ViewState["isEditMode"] == null ? false : (bool)ViewState["isEditMode"]; }
            set { ViewState["isEditMode"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnReturnClick += uclSearchBar_OnReturnClick;
            uclAdminTabs.OnTabClick += uclAdminTabs_OnTabClick;

            uclSubLists.OnDeptListClick += uclAdminList_OnDeptClick;
            uclSubLists.OnAddDeptClick += uclAdminList_OnAddDeptClick;
            uclSubLists.OnLaborListClick += uclAdminList_OnLaborClick;
            uclSubLists.OnAddLaborClick += uclAdminList_OnAddLaborClick;
            uclSubLists.OnLineListClick += uclAdminList_OnLineClick;
            uclSubLists.OnAddLineClick += uclAdminList_OnAddLineClick;

            uclAdminEdit.OnEditSaveClick += uclAdminEdit_OnSaveClick;
            uclAdminEdit.OnEditCancelClick += uclAdminEdit_OnCancelClick;

			uclNotifyList.OnNotifyActionCommand += UpdateNotifyActionList;

        }

        private void uclAdminTabs_OnTabClick(string tabID, string cmdArg)
        {
            tab_Click(tabID, cmdArg);
        }

        private void uclSearchBar_OnReturnClick()
        {
            SessionManager.ReturnObject = SessionManager.EffLocation.Plant;
            SessionManager.ReturnStatus = true;
            SessionManager.EffLocation.Plant = null;
            SetLocalOrg(null);
            Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewBusOrg.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.PageTitle.Text = lblViewPlantTitle.Text;
                uclSearchBar.SetButtonsVisible(false, false, false, false, false, true);
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
                uclSearchBar.ReturnButton.Text = lblViewBusOrgText.Text;
                if (SessionManager.IsEffLocationPrimary() &&  SessionManager.EffLocation.Plant != null)
                    divNavArea.Visible = true;
                else
                    divNavArea.Visible = false;

                //if (SessionManager.EffLocation.Plant != null)
               // {
                    ClearTempData();
                    SetupPage();
               // }
            }
            else
            {
                if (SessionManager.CurrentSecondaryTab.Equals("lblPlantDocs_tab"))
                {
                    pnlPlantDocs.Visible = true;
                    uclDocMgr.BindDocMgr("BLI", 0, SessionManager.EffLocation.Plant.PLANT_ID);
                }
            }

        }

        private void ClearTempData()
        {
            SetLocalOrg(null);
        }

        private void SetupPage()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            BUSINESS_ORG busOrg = (BUSINESS_ORG)SessionManager.EffLocation.BusinessOrg;

            isEditMode = false;
            LocalOrg().EditObject = null;
            DropDownList ddl;

            HiddenField hdCurrentActiveTab = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveSecondaryTab");
            uclItemHdr.DisplayPlant(busOrg, plant);

            List<BUSINESS_ORG> parent_orgs = SQMModelMgr.SelectBusOrgList(entities, SessionManager.EffLocation.Company.COMPANY_ID, 0, true);
            ddl = (DropDownList)hfBase.FindControl("ddlParentBusOrg");
            ddl.DataSource = parent_orgs;
            ddl.DataTextField = "ORG_NAME";
            ddl.DataValueField = "BUS_ORG_ID";
            ddl.DataBind();
            ddl.SelectedIndex = 0;

            if (SessionManager.EffLocation != null  &&  SessionManager.EffLocation.BusinessOrg != null  &&  ddl.Items.FindByValue(SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID.ToString()) != null)
            {
                ddl.SelectedValue = SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID.ToString();
            }

            ddlLocationType.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("locationType"));

            ddlCountryCode.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("countryCode"));
            ddlCountryCode.Items.Insert(0, new ListItem("", ""));

            ddlPowerSourcedRegion.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("powerSourcedRegion"));
            ddlPowerSourcedRegion.Items.Insert(0, new ListItem("", ""));

            ddl = (DropDownList)hfBase.FindControl("ddlPlantCurrencyCodes");
            SQMBasePage.FillCurrencyDDL(ddl, "USD");

            ddl = (DropDownList)hfBase.FindControl("ddlPlantTimezone");
            List<Settings> time_zones = SQMSettings.TimeZone;
            ddl.DataSource = time_zones;
            ddl.DataTextField = "long_desc";
            ddl.DataValueField = "code";
            ddl.DataBind();
            ddl.SelectedValue = "035";

 
            SetStatusList("ddlPlantStatus", "A");

            tab_Click("lbPLantDetail_tab", "");
        }

        private void LoadPlantSelects(PLANT plant)
        {
            List<BUSINESS_ORG> parent_orgs = SQMModelMgr.SelectBusOrgList(entities, SessionManager.EffLocation.Company.COMPANY_ID, 0, true);
            DropDownList ddl = (DropDownList)hfBase.FindControl("ddlParentBusOrg");
           
            if (plant.BUS_ORG_ID > 0)
                ddl.SelectedValue = plant.BUS_ORG_ID.ToString();

            if (ddlLocationType.Items.FindByValue(plant.LOCATION_TYPE) != null)
                ddlLocationType.SelectedValue = plant.LOCATION_TYPE;

            if (ddlCountryCode.Items.FindByValue(plant.LOCATION_CODE) != null)
                ddlCountryCode.SelectedValue = plant.LOCATION_CODE;

            if (ddlPowerSourcedRegion.Items.FindByValue(plant.COMP_INT_ID) != null)
                ddlPowerSourcedRegion.SelectedValue = plant.COMP_INT_ID;

            ddl = (DropDownList)hfBase.FindControl("ddlPlantCurrencyCodes");
            if (!string.IsNullOrEmpty(plant.CURRENCY_CODE))
                ddl.SelectedValue = plant.CURRENCY_CODE;

            ddl = (DropDownList)hfBase.FindControl("ddlPlantTimezone");
            if (!string.IsNullOrEmpty(plant.LOCAL_TIMEZONE))
                ddl.SelectedValue = plant.LOCAL_TIMEZONE;
           /*
            ddl = (DropDownList)hfBase.FindControl("ddlPlantLanguage");
            if (plant.LOCAL_LANGUAGE.HasValue)
                ddl.SelectedValue = plant.LOCAL_LANGUAGE.ToString();
            */
            SetStatusList("ddlPlantStatus", plant.STATUS);
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
            uclAdminTabs.PlantPanel.Visible = true;
            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = pnlPlantEdit.Visible = pnlPlantDocs.Visible = pnlEscalation.Visible = pnlB2B.Visible = false;
            SetActiveTab(SessionManager.CurrentSecondaryTab = tabID);

            if (tabID != null)
            {
                isEditMode = false;
                // setup for ps_admin.js to toggle the tab active/inactive display
               SessionManager.CurrentSecondaryTab = tabID;

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
                    case "line":
                        DoLineList();
                        pnlSubLists.Visible = true;
                        break;
                    case "cust":
                        DoCustList();
                        pnlSubLists.Visible = true;
                        break;
                    case "supp":
                        DoSuppList();
                        pnlSubLists.Visible = true;
                        break;
                    case "notify":
                        pnlEscalation.Visible = true;
                        List<TaskRecordType> recordTypeList = new List<TaskRecordType>();
						//if (UserContext.CheckAccess("SQM", "") >= AccessMode.Update)
						//{
						//	recordTypeList.Add(TaskRecordType.InternalQualityIncident);
						//	recordTypeList.Add(TaskRecordType.CustomerQualityIncident);
						//	recordTypeList.Add(TaskRecordType.SupplierQualityIncident);
						//}
 
                        recordTypeList.Add(TaskRecordType.ProfileInput);
                        recordTypeList.Add(TaskRecordType.ProfileInputApproval);
                        recordTypeList.Add(TaskRecordType.HealthSafetyIncident);
                        recordTypeList.Add(TaskRecordType.PreventativeAction);
 
                       // if (recordTypeList.Count > 0  &&  !recordTypeList.Contains(TaskRecordType.ProblemCase))
                        //    recordTypeList.Add(TaskRecordType.ProblemCase);
                        
						uclNotifyList.BindNotifyList(entities, SessionManager.EffLocation.Company.COMPANY_ID, 0, SessionManager.EffLocation.Plant.PLANT_ID, recordTypeList);
						UpdateNotifyActionList("");
                        break;
                    case "docs":
                        pnlPlantDocs.Visible = true;
                        uclDocMgr.BindDocMgr("BLI", 0, SessionManager.EffLocation.Plant.PLANT_ID);
                        break;
                    default:
                        pnlPlantEdit.Visible = true;
                        if (SessionManager.EffLocation.Plant != null)
                            lnkPlantView_Click(null, null);
                        break;
                }
            }
        }

		private void UpdateNotifyActionList(string cmd)
		{
			uclNotifyList.BindNotfyPlan(SQMModelMgr.SelectNotifyActionList(entities, null, SessionManager.EffLocation.Plant.PLANT_ID), SessionManager.EffLocation, "plant");
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
                case "line":
                    SaveLine();
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
                case "line":
                    tab_Click("lbProductLine_tab", "line");
                    break;
                default:
                    break;
            }
        }

        #region plant

        protected void lnkPlantView_Click(object sender, EventArgs e)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
    
            TextBox tbPlantName = (TextBox)hfBase.FindControl("tbPlantName"); tbPlantName.Text = plant.PLANT_NAME;
            TextBox tbPlantDesc = (TextBox)hfBase.FindControl("tbPlantDesc"); tbPlantDesc.Text = plant.DISP_PLANT_NAME;
            TextBox tbPlantLocCode = (TextBox)hfBase.FindControl("tbPlantLocCode"); tbPlantLocCode.Text = plant.DUNS_CODE;
            cbTrackFinData.Checked = (bool)plant.TRACK_FIN_DATA;
            cbTrackEWData.Checked = (bool)plant.TRACK_EW_DATA;
            Label lblLastUpdate = (Label)hfBase.FindControl("lblPlantLastUpdate"); lblLastUpdate.Text = plant.LAST_UPD_BY;
            lblPlantLastUpdateDate.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)plant.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false);
           // Label lblLastUpdateDate = (Label)hfBase.FindControl("lblPlantLastUpdateDate"); lblLastUpdateDate.Text = WebSiteCommon.LocalTime((DateTime)plant.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
            
            LoadPlantSelects(plant);

            if (plant.ADDRESS != null)
            {
                ADDRESS address = plant.ADDRESS.FirstOrDefault();
                if (address != null)
                {
                    tbAddress1.Text = address.STREET1;
                    tbAddress2.Text = address.STREET2;
                    tbCity.Text = address.CITY;
                    tbState.Text = address.STATE_PROV;
                    tbPostal.Text = address.POSTAL_CODE;
                }
            }

            LocalOrg().EditObject = plant;
        }

        protected void lbPlantSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            PLANT plant = (PLANT)LocalOrg().EditObject;
            ADDRESS address = null;

            TextBox tbPlantName = (TextBox)hfBase.FindControl("tbPlantName");
            TextBox tbPlantDesc = (TextBox)hfBase.FindControl("tbPlantDesc");
            TextBox tbOrgLocCode = (TextBox)hfBase.FindControl("tbOrgLocCode"); 
            TextBox tbPlantLocCode = (TextBox)hfBase.FindControl("tbPlantLocCode");

            DropDownList ddlParentBusOrg = (DropDownList)hfBase.FindControl("ddlParentBusOrg");
            DropDownList ddlPlantCurrecyCodes = (DropDownList)hfBase.FindControl("ddlPlantCurrencyCodes");
            DropDownList ddlPlantTimeZone = (DropDownList)hfBase.FindControl("ddlPlantTimezone");
            DropDownList ddlStatus = (DropDownList)hfBase.FindControl("ddlPlantStatus");

            if (btn.CommandArgument == "edit")
            {
                if (plant == null)
                {
                    plant = new PLANT();
                    plant.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
                    entities.AddToPLANT(plant);
                }
                else
                    plant = SQMModelMgr.LookupPlant(entities, (decimal)plant.COMPANY_ID, 0, plant.PLANT_ID, "", false);

                if (string.IsNullOrEmpty(tbPlantName.Text)) // || string.IsNullOrEmpty(tbPlantLocCode.Text))
                {
                    ErrorAlert("RequiredInputs");
                    return;
                }

                plant.PLANT_NAME = tbPlantName.Text.Trim();
                plant.DISP_PLANT_NAME = tbPlantDesc.Text.Trim();
                plant.DUNS_CODE = tbPlantLocCode.Text.Trim();
                plant.LOCATION_CODE = ddlCountryCode.SelectedValue;
                if (plant.LOCATION_CODE == "US")
                    plant.COMP_INT_ID = ddlPowerSourcedRegion.SelectedValue;
                else
                    plant.COMP_INT_ID = "";
                plant.LOCATION_TYPE = ddlLocationType.SelectedValue;
                plant.TRACK_FIN_DATA = cbTrackFinData.Checked;
                plant.TRACK_EW_DATA = cbTrackEWData.Checked;
                plant.BUS_ORG_ID = Int32.Parse(ddlParentBusOrg.SelectedValue);
                plant.CURRENCY_CODE = ddlPlantCurrecyCodes.SelectedValue;
                plant.LOCAL_TIMEZONE = ddlPlantTimeZone.SelectedValue;
               // plant.LOCAL_LANGUAGE = Convert.ToInt32(ddlPlantLanguage.SelectedValue);
                plant.STATUS = ddlStatus.SelectedValue;

                if (plant.ADDRESS == null || plant.ADDRESS.Count == 0)
                {
                    address = new ADDRESS();
                    address.COMPANY_ID = plant.COMPANY_ID;
                    address.PLANT_ID = plant.PLANT_ID;
                    address.ADDRESS_TYPE = "S";
                    plant.ADDRESS.Add(address);
                }
                else
                    address = plant.ADDRESS.FirstOrDefault();

                address.STREET1 = tbAddress1.Text;
                address.STREET2 = tbAddress2.Text;
                address.CITY = tbCity.Text;
                address.STATE_PROV = tbState.Text;
                address.POSTAL_CODE = tbPostal.Text;
                address.COUNTRY = ddlCountryCode.SelectedValue;

                if (SQMModelMgr.UpdatePlant(entities, plant, SessionManager.UserContext.UserName()) != null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                }
                else
                {
                    ErrorAlert("SaveError");
                    return;
                }
            }
            else
            {
                uclSearchBar_OnReturnClick();
            }

            SessionManager.EffLocation.Plant = plant;
            LocalOrg().EditObject = null;
            SetupPage();
        }
        
        private void ErrorAlert(string errorType)
        {
            HiddenField hf = (HiddenField)hfBase.FindControl("hfErr" + errorType);
            if (hf != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + hf.Value + "');", true);
            }
        }
        
        #endregion  

        #region department

        private void DoDeptList()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LocalOrg().DeptList = SQMModelMgr.SelectDepartmentList(entities, (decimal)plant.COMPANY_ID, (decimal)plant.BUS_ORG_ID, plant.PLANT_ID);

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
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, (decimal)plant.COMPANY_ID, (decimal)plant.BUS_ORG_ID, (decimal)plant.PLANT_ID, deptID, "", false);

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
                PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
                DEPARTMENT deptNew = new DEPARTMENT();
                deptNew = uclAdminEdit.ReadDepartment(deptNew);
                LocalOrg().DeptList.Add(SQMModelMgr.CreateDepartment(entities, plant, deptNew, SessionManager.UserContext.UserName()));
            }
            else
            {
                DEPARTMENT dept = (DEPARTMENT)LocalOrg().EditObject;
                dept = SQMModelMgr.LookupDepartment(entities, (decimal)dept.COMPANY_ID, (decimal)dept.BUS_ORG_ID, (decimal)dept.PLANT_ID, (decimal)dept.DEPT_ID, "", false);
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
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            
            LocalOrg().LaborList = SQMModelMgr.SelectLaborTypeList(entities, (decimal)plant.COMPANY_ID, (decimal)plant.BUS_ORG_ID, plant.PLANT_ID);

            uclSubLists.BindLaborList(LocalOrg().LaborList);
        }

        protected void uclAdminList_OnAddLaborClick(decimal unused)
        {
            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindLaborType(new LABOR_TYPE());
        }

        protected void uclAdminList_OnLaborClick(decimal laborID)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LABOR_TYPE labor = SQMModelMgr.LookupLaborType(entities, (decimal)plant.COMPANY_ID, (decimal)plant.BUS_ORG_ID, plant.PLANT_ID, laborID, "", false);

            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindLaborType(labor);
            LocalOrg().EditObject = labor;
        }

        protected void SaveLabor()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            bool success;

            if (uclAdminEdit.IsNew)
            {
                LABOR_TYPE laborNew = new LABOR_TYPE();
                laborNew = uclAdminEdit.ReadLaborType(laborNew);
                LocalOrg().LaborList.Add(SQMModelMgr.CreateLaborType(entities, plant, laborNew, SessionManager.UserContext.UserName()));
            }
            else
            {
                LABOR_TYPE labor = (LABOR_TYPE)LocalOrg().EditObject;
                labor = SQMModelMgr.LookupLaborType(entities, (decimal)labor.COMPANY_ID, (decimal)labor.BUS_ORG_ID, (decimal)labor.PLANT_ID, (decimal)labor.LABOR_TYP_ID, "", false);
                labor = uclAdminEdit.ReadLaborType(labor);
                SQMModelMgr.UpdateLaborType(entities, labor, SessionManager.UserContext.UserName());
                LocalOrg().LaborList[LocalOrg().LaborList.FindIndex(d => (d.LABOR_TYP_ID == labor.LABOR_TYP_ID))] = labor;
            }

            LocalOrg().EditObject = null;
            DoLaborList();
            pnlAdminEdit.Visible = false;
        }

        #endregion

        #region plantline

        private void DoLineList()
        {
            LocalOrg().LineList = SQMModelMgr.SelectPlantLineList(entities, SessionManager.EffLocation.Plant.PLANT_ID);
            uclSubLists.BindLineList(LocalOrg().LineList);
        }

        protected void uclAdminList_OnAddLineClick(decimal unused)
        {
            uclSubLists.ToggleVisible(null);
            pnlAdminEdit.Visible = true;
            uclAdminEdit.BindPlantLine(new PLANT_LINE());
        }

        protected void uclAdminList_OnLineClick(decimal lineID)
        {
            PLANT_LINE line = LocalOrg().LineList.FirstOrDefault(l => l.PLANT_LINE_ID == lineID);
            if (line != null)
            {
                uclSubLists.ToggleVisible(null);
                pnlAdminEdit.Visible = true;
                uclAdminEdit.BindPlantLine(line);
                LocalOrg().EditObject = line;
            }
        }

        protected void SaveLine()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            bool success;

            if (uclAdminEdit.IsNew)
            {
                PLANT_LINE lineNew = new PLANT_LINE();
                lineNew = uclAdminEdit.ReadPlantLine(lineNew);
                SQMModelMgr.CreatePlantLine(entities, plant, lineNew, SessionManager.UserContext.UserName());
               // plant.PLANT_LINE.Load();
            }
            else
            {
                PLANT_LINE line = (PLANT_LINE)LocalOrg().EditObject;
                line = SQMModelMgr.LookupPlantLine(entities, line.PLANT_ID, line.PLANT_LINE_ID, "", false);
                line = uclAdminEdit.ReadPlantLine(line);
                PLANT_LINE lineExisting = SQMModelMgr.UpdatePlantLine(entities, line, SessionManager.UserContext.UserName());
                //PLANT_LINE lineExisting = SQMModelMgr.FindPlantLine(entities, plant, line.PLANT_LINE_ID, "", false);
                lineExisting.PLANT_LINE_NAME = line.PLANT_LINE_NAME;
                lineExisting.DOWNTIME_RATE = line.DOWNTIME_RATE;
                lineExisting.STATUS = line.STATUS;
            }

            SessionManager.EffLocation.Plant = plant;
            LocalOrg().EditObject = null;
            DoLineList();
            pnlAdminEdit.Visible = false;
        }

        #endregion

        #region customers
        private void DoCustList()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LocalOrg().PersonList = SQMModelMgr.SelectPlantPersonList((decimal)plant.COMPANY_ID, plant.PLANT_ID, "SQM");
            List<PartData> tradeList = SQMModelMgr.SelectTradingRelationshipList(entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, plant.PLANT_ID, 0, 1, 0);
            pnlB2B.Visible = true;
            uclCustList.BindCustPartList(tradeList, LocalOrg().PersonList);
        }
        #endregion

        #region suppliers
        private void DoSuppList()
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LocalOrg().PersonList = SQMModelMgr.SelectPlantPersonList((decimal)plant.COMPANY_ID, plant.PLANT_ID, "SQM");
            List<PartData> tradeList = SQMModelMgr.SelectTradingRelationshipList(entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, plant.PLANT_ID, 0, 2, 0);
            pnlB2B.Visible = true;
            uclCustList.BindSuppPartList(tradeList, LocalOrg().PersonList);
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