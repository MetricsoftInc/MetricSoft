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
    public delegate void EditItemClick(string cmd);
    public delegate void PersonUpdate(PERSON person);

    public partial class Ucl_AdminEdit : System.Web.UI.UserControl
    {
        static private List<LOCAL_LANGUAGE> staticLangList;

        private bool isNew
        {
            get { return ViewState["adminIsNew"] == null ? false : (bool)ViewState["adminIsNew"]; }
            set { ViewState["adminIsNew"] = value; }
        }

        public bool IsNew
        {
            get { return isNew; }
            set { ;}
        }

        #region events
        public event EditItemClick OnEditCancelClick;
        public event EditItemClick OnEditSaveClick;
        public event EditItemClick OnEditAddClick;
        public event EditItemClick OnEditSelectChange;

        public event BusinessLocationChange OnBusinessLocationChanged;
        public event PersonUpdate OnPersonUpdate;

        public Ucl_BusinessLoc HRLocationUCL
        {
            get { return uclHRLoc; }
        }
        public Ucl_BusinessLoc WorkingLocationUCL
        {
            get {return uclWorkingLoc;}
        }
 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsPostBack)
            {
                pnlAdminPasswordEdit.Visible = false;
            }
		}

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.CommandArgument == "prefs")
            {
                if (pnlAdminPasswordEdit.Visible)
                {
                    uclPassEdit.UpdatePwdEdit(false);
                }
            }

            if (OnEditCancelClick != null)
            {
                btn = (Button)sender;
                OnEditCancelClick(btn.CommandArgument);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.CommandArgument == "prefs")
            {
                UpdateUser();
                if (pnlAdminPasswordEdit.Visible)
                {
                    uclPassEdit.UpdatePwdEdit(true);
                }
            }

            if (OnEditSaveClick != null)
            {
                btn = (Button)sender;
                OnEditSaveClick(btn.CommandArgument);
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (OnEditAddClick != null)
            {
                Button btn = (Button)sender;
                OnEditAddClick(btn.CommandArgument);
            }
        }
        protected void ddlSelect_Change(object sender, EventArgs e)
        {
            if (OnEditSelectChange != null)
            {
                DropDownList ddl = (DropDownList)sender;
                OnEditSelectChange(ddl.SelectedValue);
            }
        }

		// AW 201310 - allow password edit
		protected void btnPassEdit_Click(object sender, EventArgs e)
		{
            if (pnlAdminPasswordEdit.Visible)
                pnlAdminPasswordEdit.Visible = false;
            else
            {
                pnlAdminPasswordEdit.Visible = true;
                uclPassEdit.BindPwdEdit(false);
            }
		}
		#endregion

        #region department

        public void BindDeptartment(DEPARTMENT dept)
        {
            isNew = true;
            ToggleVisible(pnlEditDept);
            tbDeptName.Text = dept.DEPT_NAME;
            tbDeptCode.Text = dept.DEPT_CODE;
            lblDeptLastUpdate.Text = dept.LAST_UPD_BY;
            if (dept.LAST_UPD_DT != null)
            {
                lblDeptLastUpdateDate.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)dept.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false); // WebSiteCommon.LocalTime((DateTime)dept.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
                isNew = false;
            }
            if (string.IsNullOrEmpty(dept.STATUS))
                SetStatusList(ddlDeptStatus, "A");
            else
                SetStatusList(ddlDeptStatus, dept.STATUS);
        }

        public DEPARTMENT ReadDepartment(DEPARTMENT dept)
        {
            dept.DEPT_NAME = tbDeptName.Text;
            dept.DEPT_CODE = tbDeptCode.Text;
            dept.STATUS = ddlDeptStatus.SelectedValue;
            isNew = false;
            return dept;
        }

        #endregion

        #region userprefs

        public void SetLanguageList(List<LOCAL_LANGUAGE> langList)
        {
            staticLangList = langList;
        }

        public void BindUser(PERSON person, BusinessLocation HRLocation, BusinessLocation workingLocation)
        {
            ToggleVisible(pnlUserPrefEdit);
            PERSON user = person;
            if (user == null)
                user = SessionManager.UserContext.Person;

            tbUserPhone.Text = user.PHONE;

            if (HRLocation != null)
                uclHRLoc.BindBusinessLocation(HRLocation, true, false, true, false);
        }

        public PERSON UpdateUser()
        {
            PSsqmEntities ctx = new PSsqmEntities();
            PERSON user = SQMModelMgr.LookupPerson(ctx, SessionManager.UserContext.Person.PERSON_ID, "", false);

            user.PHONE = tbUserPhone.Text;

            user = SQMModelMgr.UpdatePerson(ctx, user, SessionManager.UserContext.UserName());

            if (OnPersonUpdate != null)
            {
                OnPersonUpdate(user);
            }

 
            return user;
        }
        #endregion



        #region labor

        public void BindLaborType(LABOR_TYPE labor)
        {
            isNew = true;
            ToggleVisible(pnlLaborEdit);
            tbLaborName.Text = labor.LABOR_NAME;
            tbLaborCode.Text = labor.LABOR_CODE;
            tbLaborRate.Text = labor.LABOR_RATE.ToString();
            lblLaborLastUpdate.Text = labor.LAST_UPD_BY;
            if (labor.LAST_UPD_DT != null)
            {
                lblLaborLastUpdateDate.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)labor.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false);  //WebSiteCommon.LocalTime((DateTime)labor.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
                isNew = false;
            }
            if (string.IsNullOrEmpty(labor.STATUS))
                SetStatusList(ddlLaborStatus, "A");
            else
                SetStatusList(ddlLaborStatus, labor.STATUS);
        }

        public LABOR_TYPE ReadLaborType(LABOR_TYPE labor)
        {
            decimal decVal = 0;
            labor.LABOR_NAME = tbLaborName.Text;
            labor.LABOR_CODE = tbLaborCode.Text;
            labor.STATUS = ddlLaborStatus.SelectedValue;
            if (decimal.TryParse(tbLaborRate.Text.Trim(), out decVal))
                labor.LABOR_RATE = decVal;
            isNew = false;
            return labor;
        }

        #endregion

        #region line
        public void BindPlantLine(PLANT_LINE line)
        {
            isNew = true;
            ToggleVisible(pnlLineEdit);
            tbLineName.Text = line.PLANT_LINE_NAME;
            tbLineDownRate.Text = line.DOWNTIME_RATE.ToString();
            lblLineLastUpdate.Text = line.LAST_UPD_BY;
            if (line.LAST_UPD_DT != null)
            {
                lblLineLastUpdateDate.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)line.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false);  //WebSiteCommon.LocalTime((DateTime)line.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
                isNew = false;
            }
            if (string.IsNullOrEmpty(line.STATUS))
                SetStatusList(ddlLineStatus, "A");
            else
                SetStatusList(ddlLineStatus, line.STATUS);
        }

        public PLANT_LINE ReadPlantLine(PLANT_LINE line)
        {
            decimal decVal = 0;
            line.PLANT_LINE_NAME = tbLineName.Text;
            line.STATUS = ddlLineStatus.SelectedValue;
            if (decimal.TryParse(tbLineDownRate.Text.Trim(), out decVal))
                line.DOWNTIME_RATE = decVal;
            isNew = false;
            return line;
        }
        #endregion

        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            pnlEditDept.Visible = pnlLaborEdit.Visible = pnlLineEdit.Visible = pnlUserPrefEdit.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        private DropDownList SetStatusList(DropDownList ddlStatus, string currentStatus)
        {
            List<Settings> status_codes = SQMSettings.Status;
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
        #endregion
    }
}