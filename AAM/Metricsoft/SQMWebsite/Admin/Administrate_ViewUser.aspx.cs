using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;
using Telerik.Charting;
using System.Web.Configuration;
using System.Threading;

namespace SQM.Website
{
	public partial class Administrate_ViewUser : SQMBasePage 
	{
        public bool isNew
        {
            get { return ViewState["isNew"] == null ? false : (bool)ViewState["isNew"]; }
            set { ViewState["isNew"] = value; }
        }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			uclSearchBar.OnReturnClick += uclSearchBar_OnReturnClick;
			uclUserList.OnUserListClick += SelectUser;
		}

		private void ClearPage()
		{
			SetLocalPerson(null);
			ddlPlantSelect.ClearCheckedItems();
			//ddlCustPlantSelect.ClearCheckedItems();
            ddlModuleAccess.ClearCheckedItems();
            lblPlantAccess.Text = "";
            lblModuleAccess.Text = "";
            ddlHRLocation.ClearSelection();
            ddlUserTimezone.ClearSelection();
			ddlUserRole.SelectedIndex = 4;

            lblDuplicateSSOId.Visible =  lblDuplicateEmail.Visible = false;
		}

        protected void OnCancel_Click(object sender, EventArgs e)
        {
            ClearPage();
        }

        protected void OnSave_Click(object sender, EventArgs e)
        {
            bool success = SaveUser(true);

            if (success)
            {
                ClearPage();
                if (isNew = true)
                    ListUsers();
            }
        }

        protected void btnUserAdd_Click(object sender, EventArgs e)
        {
            SetLocalPerson(SQMModelMgr.NewPerson("", SessionManager.EffLocation.Company.COMPANY_ID));
            isNew = true;
            DisplayUser();
        }

		protected void UserStatusSelect(object sender, EventArgs e)
		{
			ListUsers();
		}

		private void ListUsers()
		{
			SetLocalPerson(null);
			isNew = false;
            List<PERSON> userList;

            if (ddlListStatus.SelectedValue == "A")
                userList = SQMModelMgr.SearchPersonList(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", true);
			else if  (ddlListStatus.SelectedValue == "I")
				userList = SQMModelMgr.SearchPersonList(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false).Where(l=> l.STATUS == "I").ToList();
            else if (ddlListStatus.SelectedValue == "PA")
                userList = SQMModelMgr.SearchPersonList(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false).Where(l => l.ROLE == 150).ToList();
            else if (ddlListStatus.SelectedValue == "CA")
                userList = SQMModelMgr.SearchPersonList(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false).Where(l => l.ROLE <= 100).ToList();
			else
                userList = SQMModelMgr.SearchPersonList(entities, SessionManager.EffLocation.Company.COMPANY_ID, "", false);

			string selectedModule = ddlListModule.SelectedValue;
			if (selectedModule == "EHS"  ||  selectedModule == "SQM")
			{
				for (int n = userList.Count-1; n>=0;  n--)
				{
					PERSON person = userList[n];
					if (SQMModelMgr.CheckProductModuleAccess(person, selectedModule) == false)
						userList.Remove(person);
				}
			}

            uclUserList.BindUserList(userList.Where(l => l.ROLE > 1).OrderBy(l => l.LAST_NAME).ToList(), SessionManager.EffLocation.Company.COMPANY_ID);
            lblUserCount_out.Text = userList.Count.ToString();
		}

		private void uclSearchBar_OnReturnClick()
		{
			SessionManager.ReturnObject = SessionManager.EffLocation;
			SessionManager.ReturnStatus = true;
			Response.Redirect(SessionManager.CurrentAdminPage = "/Admin/Administrate_ViewBusOrg.aspx");
		}

		protected void SelectUser(decimal personID)
		{
			SetLocalPerson(SQMModelMgr.LookupPerson(entities, personID, "", false));
			DisplayUser();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				uclSearchBar.PageTitle.Text = lblViewUserTitle.Text;

				SetupPage();

				uclSearchBar.SetButtonsVisible(false, false, false, false, false, true);
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, true);
				uclSearchBar.ReturnButton.Text = lblViewUserText.Text;
                uclSearchBar.TitleItem.Text = SessionManager.EffLocation.Company.COMPANY_NAME;
                ListUsers();
			 }
		}

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (SessionManager.ReturnObject != null  &&  SessionManager.ReturnObject == "DisplayUsers")
                {
                    ListUsers();
                    SessionManager.ClearReturns();
                }
            }
        }

		protected void ddlLocationChange(object sender, EventArgs e)
		{
			decimal plantID;
			try
			{
				if (decimal.TryParse(ddlHRLocation.SelectedValue, out plantID))
				{
					ddlUserTimezone.SelectedValue = SQMModelMgr.LookupPlant(plantID).LOCAL_TIMEZONE;
					// if (accessLevel >= changeLocationAccessLevel) 
					if (ddlPlantSelect.Items.FindItemByValue(plantID.ToString()) != null)
						ddlPlantSelect.Items.FindItemByValue(plantID.ToString()).Checked = true;
				}
			}
			catch { }
		}

		private void SetupPage()
		{
			DropDownList ddl;
 
			ddl = (DropDownList)hfBase.FindControl("ddlPrefListSize");
			if (ddl != null)
			{
                ddl.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 50, 10));
			}

			if (ddlJobCode.Items.Count == 0)
			{
				foreach (JOBCODE jc in SQMModelMgr.SelectJobcodeList("", 1).OrderBy(j => j.JOB_DESC).ToList())
				{
					ddlJobCode.Items.Add(new RadComboBoxItem(jc.JOBCODE_CD + "  /  " + jc.JOB_DESC, jc.JOBCODE_CD));
				}
				ddlJobCode.Items.Insert(0, new RadComboBoxItem("System Admin", "1"));
				ddlJobCode.Items.Insert(0, new RadComboBoxItem("", ""));
			}

            ddlUserLanguage.DataSource = SQMModelMgr.SelectLanguageList(entities, false);
            ddlUserLanguage.DataTextField = "LANGUAGE_NAME";
            ddlUserLanguage.DataValueField = "LANGUAGE_ID";
            ddlUserLanguage.DataBind();
            ddlUserLanguage.SelectedIndex = 0;

            ddlUserTimezone.DataSource = SQMSettings.TimeZone;
            ddlUserTimezone.DataTextField = "long_desc";
            ddlUserTimezone.DataValueField = "code";
            ddlUserTimezone.DataBind();
            ddlUserTimezone.SelectedValue = "035";

            List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.EffLocation.Company.COMPANY_ID, 0, false);
			ddlHRLocation.Items.Clear();
			ddlPlantSelect.Items.Clear();
			SQMBasePage.SetLocationList(ddlHRLocation, locationList, 0);
            ddlHRLocation.Items.Insert(0, new RadComboBoxItem("", ""));
			SQMBasePage.SetLocationList(ddlPlantSelect, locationList, 0);
		   
			/* quality module reference
			if (ddlCustPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> customerLocations = SQMModelMgr.SelectBusinessLocationList(0, 0, false, true, true).OrderBy(l => l.Plant.PLANT_NAME).ToList();
				RadComboBoxItem item = null;
				ddlCustPlantSelect.Items.Insert(0, new RadComboBoxItem("", ""));
				foreach (BusinessLocation cust in customerLocations.GroupBy(l => l.Plant.PLANT_ID).Select(l => l.FirstOrDefault()).ToList())
				{
					item = new RadComboBoxItem(cust.Plant.PLANT_NAME, cust.Plant.PLANT_ID.ToString());
					if (cust.Address != null)
						item.ToolTip = cust.Address.STREET1 + " " + cust.Address.CITY;

					ddlCustPlantSelect.Items.Add(item);
				}
			}
			*/
		}

		private DropDownList SetStatusList(string ddlName, string currentStatus, bool editEnabled)
		{
			List<Settings> status_codes = SQMSettings.Status;
            ddlUserStatus.DataSource = status_codes;
            ddlUserStatus.DataTextField = "short_desc";
            ddlUserStatus.DataValueField = "code";
            ddlUserStatus.DataBind();

			if (!string.IsNullOrEmpty(currentStatus))
			{
				SetFindControlValue(ddlName, hfBase, currentStatus, true);
			}

            return ddlUserStatus;
		}

		protected void DisplayUser()
		{
            PERSON person = LocalPerson();
            BusinessLocation businessLocation;
            SETTINGS sets = SQMSettings.GetSetting("COMPANY", "ESCALATEANYUSER");
            SQM_ACCESS sysAccess = SQMModelMgr.LookupCredentials(entities, person.SSO_ID, "", false);
            
			divPageBody.Visible = true;
			ddlPlantSelect.ClearCheckedItems();
			/* quality module reference
			ddlCustPlantSelect.ClearCheckedItems();
						 */

            DisplayErrorMessage(null);

			if (person == null || string.IsNullOrEmpty(person.STATUS))     // new user
			{
                winUserEdit.Title = hfAddUser.Value;
				businessLocation = new BusinessLocation();
				businessLocation.Company = SessionManager.EffLocation.Company;
				SetStatusList("ddlUserStatus", "A", true);
                tbUserSSOID.Enabled = true;
                tbUserSSOID.Text = "";
                tbUserSSOID.Focus();
			}
			else
			{
                winUserEdit.Title = hfUpdateUser.Value;
                tbUserSSOID.Enabled = false;
                tbUserFirstName.Focus();

                lblPlantAccess.Text = "";
				if (person.PLANT_ID > 0)
				{
					ddlHRLocation.SelectedValue = person.PLANT_ID.ToString();
					if (ddlPlantSelect.Items.FindItemByValue(person.PLANT_ID.ToString()) != null)
						ddlPlantSelect.Items.FindItemByValue(person.PLANT_ID.ToString()).Checked = true;
				}

				if (!string.IsNullOrEmpty(person.NEW_LOCATION_CD))
				{
                    RadComboBoxItem plantItem = null;
					string[] locs = person.NEW_LOCATION_CD.Split(',');
					foreach (string locid in locs)
					{
                        if ((plantItem=ddlPlantSelect.Items.FindItemByValue(locid)) != null)
                        {
                            ddlPlantSelect.Items.FindItemByValue(locid).Checked = true;
                            if (locs.Length > 2)
                                lblPlantAccess.Text += lblPlantAccess.Text.Length == 0 ? plantItem.Text : (", " + plantItem.Text);
                        }
					}
				}
							/* quality module reference
				if (!string.IsNullOrEmpty(person.OLD_LOCATION_CD))
				{
					string[] locs = person.OLD_LOCATION_CD.Split(',');
					foreach (string locid in locs)
					{
						if (ddlCustPlantSelect.Items.FindItemByValue(locid) != null)
							ddlCustPlantSelect.Items.FindItemByValue(locid).Checked = true;
					}
				}
							 */
			}

				// AW20131106 - do not want to be able to change a SSO ID once a person has been added
            if (!string.IsNullOrEmpty(person.SSO_ID.Trim()))
                tbUserSSOID.Text = person.SSO_ID;
           
			tbUserFirstName.Text = person.FIRST_NAME;
			tbUserLastName.Text = person.LAST_NAME;
			if (ddlJobCode.Items.FindItemByValue(person.JOBCODE_CD) != null)
				ddlJobCode.SelectedValue = person.JOBCODE_CD;
			tbUserPhone.Text =  person.PHONE;
			tbUserEmail.Text = person.EMAIL;
			SetStatusList("ddlUserStatus", person.STATUS, true);
            if (sysAccess != null && sysAccess.LAST_LOGON_DT.HasValue)
                lblUserLoginDate_out.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)sysAccess.LAST_LOGON_DT, WebSiteCommon.GetXlatValue("timeZone", person.PREFERRED_TIMEZONE)), "g", true);
            else
                lblUserLoginDate_out.Text = "";
			lblUserLastUpdate.Text = person.LAST_UPD_BY + "  " + SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)person.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "g", true);

            if (ddlUserLanguage.Items.FindByValue(person.PREFERRED_LANG_ID.ToString()) != null)
                ddlUserLanguage.SelectedValue = person.PREFERRED_LANG_ID.ToString();
			if (ddlUserTimezone.Items.FindByValue(person.PREFERRED_TIMEZONE) != null)
				ddlUserTimezone.SelectedValue = person.PREFERRED_TIMEZONE;

			ddlUserRole.Items.Clear();
			ddlUserRole.Items.AddRange(WebSiteCommon.PopulateRadListItems("userRole"));
            for (int i = ddlUserRole.Items.Count - 1; i >= 0; i--)
            {
                AccessMode roleMode = SessionManager.RoleAccessMode(Convert.ToInt32(ddlUserRole.Items[i].Value));
                if (SessionManager.IsEffLocationPrimary() && roleMode == AccessMode.Partner)
                    ddlUserRole.Items.Remove(i);
                if (!SessionManager.IsEffLocationPrimary()  &&  roleMode != AccessMode.Partner)             // restrict roles of non-primary company users
                    ddlUserRole.Items.Remove(i);
                else if (UserContext.RoleAccess() < AccessMode.SA  && roleMode > AccessMode.Admin)     // only QAI admin may see or edit sys admin role
				    ddlUserRole.Items.Remove(i); 
            }
 
			string rolesDesc = "";
            if (ddlUserRole.Items.FindItemByValue(person.ROLE.ToString()) != null)
            {
                ddlUserRole.SelectedValue = person.ROLE.ToString();
            }

            if (sets == null || sets.VALUE.ToUpper() != "Y")      // prompt for user escalation
            {
                cbUserRcvEscalation.Visible = true;
                if (person.RCV_ESCALATION.HasValue)
                    cbUserRcvEscalation.Checked = (bool)person.RCV_ESCALATION;
            }
            else
            {
                cbUserRcvEscalation.Visible = false;        // enable escalation for anyone
                cbUserRcvEscalation.Checked = true;
            }

						/* quality module reference
			if (person.ROLE <= 100 || person.PERSON_ACCESS.Where(a => a.ACCESS_PROD == "SQM").Count() > 0)
			{
				ddlCustPlantSelect.Enabled = true;
			}
			else
			{
				ddlCustPlantSelect.Enabled = false;
				ddlCustPlantSelect.SelectedIndex = 0;
			}
			*/

            List<SysModule> sysmodList = SQMSettings.SystemModuleItems();
            string prod = "";
            RadComboBoxItem item = null; RadComboBoxItem itemSep = null;
            ddlModuleAccess.Items.Clear();
            lblModuleAccess.Text = "";
            foreach (SysModule sysmod in sysmodList.Where(l=> l.prod != "CQM").ToList()) 
            {
                if (string.IsNullOrEmpty(sysmod.mod))
                {
                    prod = sysmod.prod;
                    itemSep = new RadComboBoxItem(((HiddenField)pnlUserEdit.FindControl("hf" + prod)).Value, prod);
                    itemSep.IsSeparator = true;
                    ddlModuleAccess.Items.Add(itemSep);
                }
                else
                {
                    item = new RadComboBoxItem(sysmod.desc, sysmod.topic);
                    ddlModuleAccess.Items.Add(item);
                    if (GetSelectedUserRole() <= 100)
                        item.Checked = true;
                    else if ((sysmod.prod == "SQM" && person.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == "201").Count() > 0)
                        || (sysmod.prod == "EHS" && person.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == "301").Count() > 0))
                        item.Checked = true;
                    else if (person.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == sysmod.topic).Count() > 0)
                        item.Checked = true;
                    // exclusions
                    if (!SessionManager.IsEffLocationPrimary())  // restrict module selections if not primary company user
                    {
                        if (sysmod.prod != "SQM")
                        {
                            itemSep.Enabled = item.Enabled = item.Checked = false;
                            ddlModuleAccess.Height = 120;
                        }
                    }
                    if (item.Checked)
                        lblModuleAccess.Text += lblModuleAccess.Text.Length == 0 ? item.Text : (", " + item.Text);
                }
            }

            if (!SessionManager.IsEffLocationPrimary())
            {
                //ddlCustPlantSelect.Enabled = false;
                cbUserRcvEscalation.Visible = cbUserRcvEscalation.Checked = false;
            }

            string script = "function f(){OpenUserEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected bool SaveUser(bool updateUser)
		{
            bool bErrors = false;
            bool success;
            Label lblErrorMessage = null;

            PERSON person = LocalPerson();
            string currentSSOID = LocalPerson().SSO_ID;
            PERSON currentPerson = new PERSON();

            if (isNew)
            {
                person.SSO_ID = string.IsNullOrEmpty(tbUserSSOID.Text) ? "" : tbUserSSOID.Text.Trim();  // trim traling blanks when creating new user
            }
            else
            {
                person = SQMModelMgr.LookupPerson(entities, person.PERSON_ID, "", false);
                person.SSO_ID = string.IsNullOrEmpty(tbUserSSOID.Text) ? "" : tbUserSSOID.Text;
                currentPerson.ROLE = person.ROLE;
            }

            person.FIRST_NAME = string.IsNullOrEmpty(tbUserFirstName.Text) ? "" : tbUserFirstName.Text;
            person.LAST_NAME = string.IsNullOrEmpty(tbUserLastName.Text) ? "" : tbUserLastName.Text;
			person.JOBCODE_CD = ddlJobCode.SelectedValue;
            person.PHONE = tbUserPhone.Text;
            person.EMAIL = tbUserEmail.Text;

            if (!string.IsNullOrEmpty(ddlUserLanguage.SelectedValue))
                person.PREFERRED_LANG_ID = Convert.ToInt32(ddlUserLanguage.SelectedValue);
            if (!string.IsNullOrEmpty(ddlUserTimezone.SelectedValue))
                person.PREFERRED_TIMEZONE = ddlUserTimezone.SelectedValue;

            person.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
            if (!string.IsNullOrEmpty(ddlHRLocation.SelectedValue))
            {
                PLANT plant = SQMModelMgr.LookupPlant(Convert.ToDecimal(ddlHRLocation.SelectedValue));
                if (plant != null)
                {
                    person.PLANT_ID = plant.PLANT_ID;
                    person.BUS_ORG_ID = (decimal)plant.BUS_ORG_ID;
                }
            }
            person.NEW_LOCATION_CD = "";

            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect))
            {
                person.NEW_LOCATION_CD += (item.Value + ",");
            }
            person.NEW_LOCATION_CD = person.NEW_LOCATION_CD.TrimEnd(',');

            person.OLD_LOCATION_CD = "";

						/* quality module reference
            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlCustPlantSelect))
            {
                person.OLD_LOCATION_CD += (item.Value + ",");
            }
            person.OLD_LOCATION_CD = person.OLD_LOCATION_CD.TrimEnd(',');
			*/
            person.STATUS = ddlUserStatus.SelectedValue;

            // roles were originally a list - let's keep the logic below just in case we need to restore a multi-role strategy
            //person.PERSON_ROLE.Clear();
            person.ROLE = GetSelectedUserRole();
            person.RCV_ESCALATION = cbUserRcvEscalation.Checked;

            person.PERSON_ACCESS.Clear();
            string prod = "";
            foreach (RadComboBoxItem accessItem in ddlModuleAccess.Items)
            {
                if (accessItem.IsSeparator)
                    prod = accessItem.Value;
                if (accessItem.Checked || person.ROLE <= 100)
                {
                    person = SQMModelMgr.AddPersonAccess(person, prod, accessItem.Value, true);
                }
            }

            SetLocalPerson(person);

            if (string.IsNullOrEmpty(tbUserSSOID.Text) || string.IsNullOrEmpty(tbUserFirstName.Text) || string.IsNullOrEmpty(tbUserLastName.Text)
                    || string.IsNullOrEmpty(tbUserEmail.Text) || ddlUserRole.SelectedIndex < 0 || string.IsNullOrEmpty(ddlHRLocation.SelectedValue)
                    || string.IsNullOrEmpty(ddlHRLocation.SelectedValue))
            {
                lblErrorMessage = lblErrRequiredInputs;
            }

            if (lblErrorMessage == null)
            {
                // AW20131106 - need to verify that the SSO_ID and email address are unique in the system 
                lblDuplicateSSOId.Visible = false;
                lblDuplicateEmail.Visible = false;
                string strSSOId = tbUserSSOID.Text;
                string strEmail = tbUserEmail.Text;
                if (isNew) // || !strSSOId.Equals(person.SSO_ID))
                {
                    // verify unique sso_id
                    strSSOId = tbUserSSOID.Text.Trim();
                    SQM.Website.PSsqmEntities ctxAccess = new PSsqmEntities();
                    SQM_ACCESS access = SQMModelMgr.LookupCredentials(ctxAccess, strSSOId, "", false);
                    if (access != null && access.SSO_ID.ToLower().Equals(strSSOId.ToLower()))
                    {
                        lblErrorMessage = lblDuplicateSSOId;
                    }
                }
                if (lblErrorMessage == null  &&  (isNew || !strEmail.Equals(person.EMAIL)))
                {
                    // verify unique email
                    SQM.Website.PSsqmEntities ctxAccess = new PSsqmEntities();
                    //SQM_ACCESS access = SQMModelMgr.LookupCredentialsByEmail(ctxAccess, strEmail, false);
                    // ABW 20140117 - we want to look up email on person record... 
                    PERSON personEmail = SQMModelMgr.LookupPersonByEmail(ctxAccess, strEmail);
                    if (personEmail != null && personEmail.EMAIL.Trim().ToLower().Equals(strEmail.Trim().ToLower()))
                    {
                        lblErrorMessage = lblDuplicateEmail;
                    }
                }
            }
            if (lblErrorMessage != null)
            {
                DisplayUser();
                DisplayErrorMessage(lblErrorMessage);
                return false;
            }

			if (updateUser)
			{
                List<SETTINGS> MailSettings = SQMSettings.SelectSettingsGroup("MAIL", ""); // ABW 20140805
                SETTINGS setting = new SETTINGS(); // ABW 20140805
				SetLocalPerson(SQMModelMgr.UpdatePerson(entities, person, SessionManager.UserContext.UserName(),false, currentSSOID));
				//selectedUser = SQMModelMgr.UpdatePerson(entities, person, SessionManager.UserContext.UserName(), Convert.ToBoolean(GetFindControlValue("cbIsBuyer", hfBase, out success)), GetFindControlValue("tbBuyerCode", hfBase, out success));
				// AW20131106 - send an email for new users with random password generation

				setting = MailSettings.Find(x => x.SETTING_CD == "MailFromSystem"); // ABW 20140805
				string strEmailCompanyName = ""; // ABW 20140805
				if (setting != null) // ABW 20140805
					strEmailCompanyName = setting.VALUE; 

				if (isNew)
				{
					// send a confirmation email
					// string strength = WebConfigurationManager.AppSettings["PasswordComplexity"]; // ABW 20140805
					SETTINGS complexity = SQMSettings.SelectSettingByCode(entities, "COMPANY", "TASK", "PasswordComplexity"); // ABW 20140805
					string strength = ""; // ABW 20140805
					if (complexity == null)
						strength = "4";
					else
						strength = complexity.VALUE;

					SQM.Website.PSsqmEntities ctxAccess = new PSsqmEntities();
                    SQM_ACCESS access = SQMModelMgr.LookupCredentials(ctxAccess, LocalPerson().SSO_ID, "", false);
					string key = SQMModelMgr.GetPasswordKey();
					string strPassword = WebSiteCommon.Decrypt(access.PASSWORD, key);

					// ABW 20140805 - Build the email based on fields in the SETTINGS table
					// the following is standard email
					//string strEmailBody = lblPasswordEmailBody1a.Text.ToString() + strEmailCompanyName + lblPasswordEmailBody1b.Text.ToString() + " " + selectedUser.SSO_ID + lblPasswordEmailBody2.Text.ToString() + " " + strPassword;
					//strEmailBody += "<br><br>" + WebSiteCommon.GetXlatValueLong("passwordComplexity", strength) + "<br><br>" + lblPasswordEmailBody3.Text.ToString().Trim();

					// the following is for TI only
					//string strEmailBody = "Risk Management Professional,<br><br>TI Automotive Risk Management is pleased to offer you access to the TI Automotive Risk Management Portal (Portal)<br><br>The Portal will be used to provide tracking for:<br>";
					//strEmailBody += "<ul><li>Environmental performance tracking</li><li>Insurer Recommendations response</li><li>Internal Risk Quality Index Recommendations Response</li><li>Safety Alerts</li>";
					//strEmailBody += "<br>A new user account has been created for you in the Portal.<br><br>Access the website by clicking on the link: <a href='http://Ti.qai.luxinteractive.com'>Ti.qai.luxinteractive.com</a><br><br>";
					//strEmailBody += "Your username has been assigned: <font color='red'>" + selectedUser.SSO_ID + "</font><br>Your temporary password is: <font color='red'>" + strPassword + "</font>";
					//strEmailBody += "<br>Once you gain access to the Portal you must change your password. " + WebSiteCommon.GetXlatValueLong("passwordComplexity", strength) + "<br><br>" + lblPasswordEmailBody3.Text.ToString().Trim();
					//strEmailBody += "<br><br><b>Michael D. Wildfong</b><br>Global Director Facilities Risk Management<br>TI Automotive<br>1272 Doris Road<br>Auburn Hills, MI 48326<br>t: +1 248 494 5320<br>m: + 1 810 265 1677<br>f: +1 248 494 5302";
					//strEmailBody += "<br>e: <a href='mailto:mwildfong@us.tiauto.com'>mwildfong@us.tiauto.com</a>";

					// ABW 20140805 - Build the email based on fields in the SETTINGS table
					string strEmailSubject = "";
					setting = MailSettings.Find(x => x.SETTING_CD == "NewUserSubject");
					if (setting == null)
						strEmailSubject = strEmailCompanyName + " " + lblPasswordEmailSubject.Text.ToString();
					else
						strEmailSubject = setting.VALUE.Trim();
					setting = MailSettings.Find(x => x.SETTING_CD == "NewUserWelcome");
					string strEmailBody = "";
					if (setting == null)
						strEmailBody = lblPasswordEmailBody1a.Text.ToString();
					else
						strEmailBody = setting.VALUE.Trim();
                    strEmailBody += lblPasswordEmailBody1b.Text.ToString() + " " + LocalPerson().SSO_ID + lblPasswordEmailBody2.Text.ToString() + " " + strPassword;
					setting = MailSettings.Find(x => x.SETTING_CD == "MailURL");
					if (setting != null)
						strEmailBody += lblPasswordEmailBody2b.Text.ToString() + "<a href='" + setting.VALUE + "'>" + setting.VALUE + "</a>";
					complexity = SQMSettings.SelectSettingByCode(entities, "PASSWORDCOMPLEXITY", "TASK", strength); // ABW 20140805
					if (complexity != null)
						strEmailBody += "<br><br>" + complexity.VALUE + "<br><br>";
					setting = MailSettings.Find(x => x.SETTING_CD == "NewUserSignature");
					if (setting == null)
						strEmailBody += "<br><br>" + lblPasswordEmailBody3.Text.ToString();
					else
						strEmailBody += "<br><br>" + setting.VALUE.Trim();
					
					// ABW 20140117 - we are now using the email on the Person record
					Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, strEmailSubject, strEmailBody.Trim(), ""));
					thread.IsBackground = true;
					thread.Start();
				}
				else
				{
					bool roleChanged = person.ROLE != currentPerson.ROLE ||  person.PERSON_ACCESS.Count != currentPerson.PERSON_ACCESS.Count ? true : false;
					if (roleChanged)
					{
//						string strEmailBody = lblUserRoleEmailBodya.Text + strEmailCompanyName + lblUserRoleEmailBodyb.Text + "<br><br>" + lblUserRoleEmailBodyc.Text;
						// ABW 20140805 - Build the email based on fields in the SETTINGS table
						string strEmailSubject = "";
						setting = MailSettings.Find(x => x.SETTING_CD == "AdminRoleChangeSubject");
						if (setting == null)
							strEmailSubject = lblUserRoleEmailSubjecta.Text + strEmailCompanyName + lblUserRoleEmailSubjectb.Text;
						else
							strEmailSubject = setting.VALUE.Trim();
						setting = MailSettings.Find(x => x.SETTING_CD == "AdminRoleChangeWelcome");
						string strEmailBody = "";
						if (setting == null)
							strEmailBody = lblUserRoleEmailBodya.Text + strEmailCompanyName + lblUserRoleEmailBodyb.Text;
						else
							strEmailBody = setting.VALUE.Trim();

						setting = MailSettings.Find(x => x.SETTING_CD == "AdminRoleChangeSignature");
						if (setting == null)
							strEmailBody += "<br><br>" + lblUserRoleEmailBodyc.Text;
						else
							strEmailBody += "<br><br>" + setting.VALUE.Trim();
						Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, strEmailSubject, strEmailBody, ""));
						thread.IsBackground = true;
						thread.Start();
					}
					if (cbResetPassword.Checked)
					{
						// build the email body in 3 segments
						SETTINGS complexity = SQMSettings.SelectSettingByCode(entities, "COMPANY", "TASK", "PasswordComplexity");
						string strength = "";
						if (complexity == null)
							strength = "4";
						else
							strength = complexity.VALUE;
						string strEmailSubject = "";
						setting = MailSettings.Find(x => x.SETTING_CD == "AdminPasswordResetSubject");
						if (setting == null)
							strEmailSubject = strEmailCompanyName + " " + lblResetEmailSubject.Text.ToString();
						else
							strEmailSubject = setting.VALUE.Trim();
						setting = MailSettings.Find(x => x.SETTING_CD == "AdminPasswordResetWelcome");
						string strEmailBodya = "";
						string strEmailBodyb = "";
						string strEmailBodyc = "";
						if (setting == null)
							strEmailBodya = lblPasswordEmailBody1a.Text.ToString();
						else
							strEmailBodya = setting.VALUE.Trim();
						strEmailBodya += lblPasswordEmailBody1b.Text.ToString();
						strEmailBodyb = lblPasswordEmailBody2.Text.ToString();
						setting = MailSettings.Find(x => x.SETTING_CD == "MailURL");
						if (setting != null)
							strEmailBodyc += lblPasswordEmailBody2b.Text.ToString() + "<a href='" + setting.VALUE + "'>" + setting.VALUE + "</a>";
						complexity = SQMSettings.SelectSettingByCode(entities, "PASSWORDCOMPLEXITY", "TASK", strength);
						if (complexity != null)
							strEmailBodyc += "<br><br>" + complexity.VALUE + "<br><br>";
						setting = MailSettings.Find(x => x.SETTING_CD == "AdminPasswordResetSignature");
						if (setting == null)
							strEmailBodyc += "<br><br>" + lblPasswordEmailBody3.Text.ToString();
						else
							strEmailBodyc += "<br><br>" + setting.VALUE.Trim();
						int msg = WebSiteCommon.RecoverPassword(person.EMAIL, person.SSO_ID, strEmailSubject, strEmailBodya, strEmailBodyb, strEmailBodyc);
					}
				}
				isNew = false;
                if (SQMModelMgr.updateStatus < 0)  // report error
				    AlertUpdateResult(SQMModelMgr.updateStatus);
			}
			else
				SetLocalPerson(person);
			return true;
		}

		private int GetSelectedUserRole()
		{
			int  role = Convert.ToInt32(ddlUserRole.SelectedValue);
			return role;
		}

        private void DisplayErrorMessage(Label lblMessage)
        {
            if (lblMessage == null)
                lblErrorMessage.Text = "";
            else
                lblErrorMessage.Text = lblMessage.Text;
        }

        // manage current session object  (formerly was page static variable)
        PERSON  LocalPerson()
        {
            if (SessionManager.CurrentObject  != null  &&  SessionManager.CurrentObject is PERSON)
                return (PERSON)SessionManager.CurrentObject;
            else
                return null;
        }

        PERSON SetLocalPerson(PERSON person)
        {
            SessionManager.CurrentObject = person;
            return LocalPerson();
        }

	}
}