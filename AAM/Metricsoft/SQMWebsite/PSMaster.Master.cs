using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{

	public partial class PSMaster : System.Web.UI.MasterPage
	{

		public List<XLAT> menuXLATList 
		{
			get { return ViewState["MenuXLATList"] == null ? null : (List<XLAT>)ViewState["MenuXLATList"]; }
			set { ViewState["MenuXLATList"] = value; }
		}
		public List<SETTINGS> settingList
		{
			get { return ViewState["MenuSettingList"] == null ? null : (List<SETTINGS>)ViewState["MenuSettingList"]; }
			set { ViewState["MenuSettingList"] = value; }
		}

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.IsLoggedIn() == false)
                {
                    //throw new UserContextError();
                    // AW201312 - if this is a 404 error and not logged in, go to a generic 404 page
                    if (HttpContext.Current.Request.Url.AbsolutePath.ToString().ToLower().Equals("/home/error404.aspx"))
                    {
                        string strPath = "";
                        try { strPath = Request.QueryString.Get("aspxerrorpath"); }
                        catch { }
                        Response.Redirect("~/GenericError404.aspx?aspxerrorpath=" + strPath);
                    }
                    else
                    {
                        string redirectPath = Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery);
                        if (!string.IsNullOrEmpty(redirectPath))
                            Response.Redirect("~/login.aspx?rdp=" + redirectPath);
                        else
                            Response.Redirect("~/login.aspx" + redirectPath);
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
		{
            try
            {
                if (!Page.IsPostBack)
                {
					if (settingList == null || settingList.Count == 0)
					{
						settingList = SQMSettings.SelectSettingsGroup("MODULE", "");
					}

					if (menuXLATList == null || menuXLATList.Count == 0)
					{
						menuXLATList = SQMBasePage.SelectXLATList(new string[6] { "MENU_HOME", "MENU_ORG", "MENU_ENV", "MENU_HS", "MENU_AUDIT", "MENU_DATA" });
					}

                    SessionManager.CurrentAdminPage = "";
                    string customerLogo = "";
                    customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"];
                    if (string.IsNullOrEmpty(customerLogo) || customerLogo.Contains("Metricsoft"))
                    {
                        imgLogo.ImageUrl = "~/images/company/MetricsoftLogoSmall.png";
                    }
                    else
                    {
                        int pos = customerLogo.IndexOf('.');
                        customerLogo = customerLogo.Substring(0, pos) + "Small." + customerLogo.Substring(pos + 1, customerLogo.Length - pos - 1);
                        imgLogo.ImageUrl = "~/images/company/" + customerLogo;
                    }

                    string title = System.Configuration.ConfigurationManager.AppSettings["MainTitle"];
                    if (!string.IsNullOrEmpty(title))
                        lblMainTitle.Text = title;

                    SetupPage();

					bool addConsole = false;
					RadMenu1.Skin = "Metro";
					RadMenu1.ExpandDelay = 225;
					RadMenu1.CollapseDelay = 500;
					RadMenu1.ExpandAnimation.Duration = 40;
					RadMenu1.CollapseAnimation.Duration = 20;
					RadMenu1.DefaultGroupSettings.Flow = Telerik.Web.UI.ItemFlow.Horizontal;

					RadMenuItem HomeMenu = new RadMenuItem(GetMenu("MENU_HOME", "0").DESCRIPTION);
					RadMenu1.Items.Add(HomeMenu);
					if (UserContext.GetScopePrivileges(SysScope.dashboard).Count() > 0)
						HomeMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_HOME", "11").DESCRIPTION, GetMenu("MENU_HOME", "11").DESCRIPTION_SHORT));
					if (UserContext.GetScopePrivileges(SysScope.inbox).Count() > 0)
						HomeMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_HOME", "12").DESCRIPTION, GetMenu("MENU_HOME", "12").DESCRIPTION_SHORT));

					if (UserContext.GetMaxScopePrivilege(SysScope.busorg) <= SysPriv.admin)
					{
						RadMenuItem OrgMenu = new RadMenuItem(GetMenu("MENU_ORG", "0").DESCRIPTION);
						RadMenu1.Items.Add(OrgMenu);
						OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ORG", "11").DESCRIPTION, GetMenu("MENU_ORG", "11").DESCRIPTION_SHORT));
						OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ORG", "12").DESCRIPTION, GetMenu("MENU_ORG", "12").DESCRIPTION_SHORT));
						if (UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.system) && SessionManager.UserContext.Person.PERSON_ID == 1)
						{
							OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ORG", "13").DESCRIPTION, GetMenu("MENU_ORG", "13").DESCRIPTION_SHORT));
							OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ORG", "14").DESCRIPTION, GetMenu("MENU_ORG", "14").DESCRIPTION_SHORT));
						}
					}

					if (UserContext.GetScopePrivileges(SysScope.envdata).Count() > 0)
					{
						RadMenuItem EHSMenu1 = new RadMenuItem(GetMenu("MENU_ENV", "0").DESCRIPTION);
						RadMenu1.Items.Add(EHSMenu1);

						if (UserContext.CheckUserPrivilege(SysPriv.config, SysScope.envdata))
						{
							EHSMenu1.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ENV", "11").DESCRIPTION, GetMenu("MENU_ENV", "11").DESCRIPTION_SHORT));
							EHSMenu1.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ENV", "12").DESCRIPTION, GetMenu("MENU_ENV", "12").DESCRIPTION_SHORT));
						}
						EHSMenu1.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ENV", "13").DESCRIPTION, GetMenu("MENU_ENV", "13").DESCRIPTION_SHORT));
						EHSMenu1.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ENV", "14").DESCRIPTION, GetMenu("MENU_ENV", "14").DESCRIPTION_SHORT));
						if (addConsole == false && UserContext.GetScopePrivileges(SysScope.console).Count() > 0)
						{
							EHSMenu1.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_ENV", "15").DESCRIPTION, GetMenu("MENU_ENV", "15").DESCRIPTION_SHORT));
							addConsole = true;
						}
					}

					if (UserContext.GetScopePrivileges(SysScope.incident).Count() > 0)
					{
						RadMenuItem EHSMenu2 = new RadMenuItem(GetMenu("MENU_HS", "0").DESCRIPTION);
						RadMenu1.Items.Add(EHSMenu2);
						if (UserContext.GetScopePrivileges(SysScope.incident).Count() > 0)
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_HS", "11").DESCRIPTION, GetMenu("MENU_HS", "11").DESCRIPTION_SHORT));
						if (UserContext.GetScopePrivileges(SysScope.console).Count() > 0)
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_HS", "12").DESCRIPTION, GetMenu("MENU_HS", "12").DESCRIPTION_SHORT));
					}

					if (UserContext.GetScopePrivileges(SysScope.audit).Count() > 0)
					{
						RadMenuItem EHSMenu2 = new RadMenuItem(GetMenu("MENU_AUDIT", "0").DESCRIPTION);
						RadMenu1.Items.Add(EHSMenu2);
						if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.config)
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_AUDIT", "11").DESCRIPTION, GetMenu("MENU_AUDIT", "11").DESCRIPTION_SHORT));
						if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.originate)
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_AUDIT", "12").DESCRIPTION, GetMenu("MENU_AUDIT", "12").DESCRIPTION_SHORT));
						if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.config)
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_AUDIT", "13").DESCRIPTION, GetMenu("MENU_AUDIT", "13").DESCRIPTION_SHORT));
					}

					if (settingList.Where(l => l.SETTING_CD == "PREVACTION" && l.VALUE == "A").Count() > 0)
					{
						if (UserContext.GetScopePrivileges(SysScope.prevent).Count() > 0 && IsMenuActive("MENU_RM"))
						{
							RadMenuItem EHSMenu2 = new RadMenuItem(GetMenu("MENU_RM", "0").DESCRIPTION);
							RadMenu1.Items.Add(EHSMenu2);
							if (UserContext.GetScopePrivileges(SysScope.prevent).Count() > 0)
								EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_RM", "11").DESCRIPTION, GetMenu("MENU_RM", "11").DESCRIPTION_SHORT));
						}
					}

					if (settingList.Where(l => l.SETTING_CD == "EHSDATA" && l.VALUE == "A").Count() > 0)
					{
						if (UserContext.GetMaxScopePrivilege(SysScope.ehsdata) <= SysPriv.originate && IsMenuActive("MENU_DATA"))
						{
							RadMenuItem EHSMenu2 = new RadMenuItem(GetMenu("MENU_DATA", "0").DESCRIPTION);
							RadMenu1.Items.Add(EHSMenu2);
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_DATA", "11").DESCRIPTION, GetMenu("MENU_DATA", "11").DESCRIPTION_SHORT));
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_DATA", "12").DESCRIPTION, GetMenu("MENU_DATA", "12").DESCRIPTION_SHORT));
							EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem(GetMenu("MENU_DATA", "13").DESCRIPTION, GetMenu("MENU_DATA", "13").DESCRIPTION_SHORT));
							if (UserContext.GetMaxScopePrivilege(SysScope.ehsdata) <= SysPriv.config)
							{
								EHSMenu2.Items.Add(new RadMenuItem(GetMenu("MENU_DATA", "21").DESCRIPTION, GetMenu("MENU_DATA", "21").DESCRIPTION_SHORT));
								EHSMenu2.Items.Add(new RadMenuItem(GetMenu("MENU_DATA", "22").DESCRIPTION, GetMenu("MENU_DATA", "22").DESCRIPTION_SHORT));
							}
							if (UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.ehsdata) || UserContext.CheckUserPrivilege(SysPriv.approve1, SysScope.ehsdata))
							{
								EHSMenu2.Items.Add(new RadMenuItem(GetMenu("MENU_DATA", "25").DESCRIPTION, GetMenu("MENU_DATA", "25").DESCRIPTION_SHORT));
							}
						}
					}
                }
            }

            catch (Exception ex)
            {
               // SQMLogger.LogException(ex);
            }
		}

		protected bool IsMenuActive(string menuGroup)
		{
			return menuXLATList.Where(l => l.XLAT_GROUP == menuGroup).Count() == 0 ? false : true;
		}

		protected XLAT GetMenu(string menuGroup, string menuItem)
		{
			XLAT menu = menuXLATList.Where(l => l.XLAT_GROUP == menuGroup && l.XLAT_CODE == menuItem).FirstOrDefault();
			if (menu == null)
			{
				menu = new XLAT();
			}
			return menu;
		}

		protected void SetupPage()
		{
			hdCurrentActiveMenu.Value = SessionManager.CurrentMenuItem;

            lbBusinessCard1_out.Text = SessionManager.UserContext.UserName(); // +": " + SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME;
            lblActiveLocation.Text = SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME;
			if (SessionManager.StatOfTheDay != null)
			{
                lblActiveLocation.Text = SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME + ": ";
				lblStatOfTheDay_out.Text = SQMBasePage.FormatValue(SessionManager.StatOfTheDay.Value, 0);
                lblStatOfTheDay.Visible = lblStatOfTheDay_out.Visible = true;
			}
			else
			{
		        lblStatOfTheDay_out.Text = "n/a";
			}
			pnlStatOfTheDay.Visible = true;

			if (ddlUserLang.Items.Count == 0)
			{
				foreach (LOCAL_LANGUAGE lang in SQMModelMgr.SelectLanguageList(new PSsqmEntities(), true))
				{
					ddlUserLang.Items.Add(new RadComboBoxItem(lang.LANGUAGE_NAME, lang.LANGUAGE_ID.ToString()));
				}
			}
			tbUserPhone.Text = SessionManager.UserContext.Person.PHONE;
			if (ddlUserLang.FindItemByValue(SessionManager.UserContext.Person.PREFERRED_LANG_ID.ToString()) != null)
				ddlUserLang.SelectedValue = SessionManager.UserContext.Person.PREFERRED_LANG_ID.ToString();

			// ABW 20160118 - show password reset based on parameter
			SETTINGS setsPwdReset = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "COMPANY", "TASK", "PasswordResetEnable");
			if (setsPwdReset != null && setsPwdReset.VALUE.ToUpper() == "Y")
			{
				trChangePwd.Visible = true;
			}
		}

		protected void menu_Click(object sender, EventArgs e)
		{
			LinkButton lb = (LinkButton)sender;
			string cmd = lb.CommandArgument;

			hdCurrentActiveMenu.Value = SessionManager.CurrentMenuItem = lb.ClientID;
			SessionManager.CurrentAdminTab = SessionManager.CurrentSecondaryTab = "";

			Response.Redirect(cmd);
		}


        protected void btnPassEdit_Click(object sender, EventArgs e)
        {
            uclPassEdit.BindPwdEdit(true);
        }


		protected void lbLogout_Click(object sender, EventArgs e)
		{
            string loginURL = SessionManager.LoginURL;
			SessionManager.Clear();
			Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            try
            {
                Response.Redirect(loginURL);
            }
            catch
            {
                Response.Redirect("/Login.aspx");
            }
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			PSsqmEntities ctx = new PSsqmEntities();
			PERSON user = SQMModelMgr.LookupPerson(ctx, SessionManager.UserContext.Person.PERSON_ID, "", false);
			user.PHONE = tbUserPhone.Text;
			if (!string.IsNullOrEmpty(ddlUserLang.SelectedValue))
			{
				user.PREFERRED_LANG_ID = Convert.ToInt32(ddlUserLang.SelectedValue);
				SessionManager.SessionContext.SetLanguage((int)user.PREFERRED_LANG_ID);
			}
			user = SQMModelMgr.UpdatePerson(ctx, user, SessionManager.UserContext.UserName());
			if (user != null)
				SessionManager.UserContext.Person = user;  // ??
		}
	}

}