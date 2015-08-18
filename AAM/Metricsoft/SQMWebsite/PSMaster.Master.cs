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

                    RadMenu1.Skin = "Metro";
                    RadMenu1.ExpandDelay = 225;
                    RadMenu1.CollapseDelay = 500;
                    RadMenu1.ExpandAnimation.Duration = 40;
                    RadMenu1.CollapseAnimation.Duration = 20;
                    RadMenu1.DefaultGroupSettings.Flow = Telerik.Web.UI.ItemFlow.Horizontal;

                    if (UserContext.RoleAccess() > AccessMode.None)
                    {
                        RadMenuItem HomeMenu = new RadMenuItem("Home");
                        RadMenu1.Items.Add(HomeMenu);
                        if (UserContext.RoleAccess() != AccessMode.Partner)
                            HomeMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Dashboard", "/Home/Dashboard.aspx"));
                         HomeMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Calendar", "/Home/Calendar.aspx"));
                        // HomeMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(inboxLabel, "/Home/Inbox.aspx"));
                    }
                
                    if (UserContext.CheckAccess("CQM", "101") >= AccessMode.Admin)
                    {
                        RadMenuItem OrgMenu = new RadMenuItem("Organization");
                        RadMenu1.Items.Add(OrgMenu);
                        OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Business Structure", "/Admin/Administrate_ViewBusOrg.aspx"));
                        OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Exchange Rates", "/Admin/Administrate_CurrencyInput.aspx"));
                        //if (UserContext.RoleAccess() == AccessMode.SA)
                        OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Settings", "/Admin/Administrate_SettingInput.aspx"));
                        OrgMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Upload Data", "/Admin/Administrate_FileUpload.aspx"));
                    }

					/*
                    if (UserContext.CheckAccess("SQM", "") > AccessMode.Limited)
                    {
                        RadMenuItem SQMMenu = new RadMenuItem("Quality");
                        RadMenu1.Items.Add(SQMMenu);
                        if (UserContext.CheckAccess("SQM", "201") > AccessMode.Plant)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Library", "/Quality/Quality_Resources.aspx"));
                        if (UserContext.CheckAccess("SQM", "201") > AccessMode.Plant)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Parts", "/Admin/Administrate_ViewPart.aspx"));
                        if (UserContext.CheckAccess("SQM", "211") > AccessMode.Limited)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(SQMSettings.GetSetting("QS", "3").XLAT_SHORT, "/Quality/Quality_Issue.aspx?c=RCV"));
                        if (UserContext.CheckAccess("SQM", "212") > AccessMode.Limited)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem(SQMSettings.GetSetting("QS", "4").XLAT_SHORT, "/Quality/Quality_Issue.aspx?c=CST"));
                        if (UserContext.CheckAccess("SQM", "220") > AccessMode.Admin)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Cost Reporting", "/Quality/Quality_CostRecord.aspx"));
                        if (UserContext.CheckAccess("SQM", "221") > AccessMode.Limited)
                            SQMMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Problem Control", "/Problem/Problem_Case.aspx?c=QI"));
                    }
					*/

                    if (UserContext.CheckAccess("EHS", "") > AccessMode.Limited)
                    {
                        RadMenuItem EHSMenu = new RadMenuItem("Environmental");
                        RadMenu1.Items.Add(EHSMenu);
                        if (UserContext.CheckAccess("EHS", "301") > AccessMode.Plant)
                            EHSMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Library", "/EHS/EHS_Resources.aspx"));
                        if (UserContext.CheckAccess("EHS", "301") >= AccessMode.Plant)
                            EHSMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Metric Profiles", "/EHS/EHS_Profile.aspx"));
                        if (UserContext.CheckAccess("EHS", "311") > AccessMode.Limited)
                            EHSMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Data Input", "/EHS/EHS_MetricInput.aspx"));
                        if (UserContext.CheckAccess("EHS", "311") > AccessMode.Limited)
                            EHSMenu.Items.Add(new Telerik.Web.UI.RadMenuItem("Plant Analytics", "/EHS/EHS_ENVReport.aspx"));

						RadMenuItem EHSMenu2 = new RadMenuItem("Health & Safety");
						RadMenu1.Items.Add(EHSMenu2);
                        if (UserContext.CheckAccess("EHS", "312") > AccessMode.Limited)
                            EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem("Incidents", "/EHS/EHS_Incidents.aspx"));
                        if (UserContext.CheckAccess("EHS", "313") > AccessMode.Limited)
                            EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem("Preventative Actions", "/EHS/EHS_Incidents.aspx?mode=prevent"));
                        if (UserContext.CheckAccess("EHS", "301") > AccessMode.Plant)
                            EHSMenu2.Items.Add(new Telerik.Web.UI.RadMenuItem("Console", "/EHS/EHS_Console.aspx?c=EHS"));
                    }

                    string menu8DActive = System.Configuration.ConfigurationManager.AppSettings["Menu8DActive"];
                    if (!string.IsNullOrEmpty(menu8DActive)  &&  menu8DActive.ToUpper() == "FALSE")
                    {
                        foreach (RadMenuItem mi in RadMenu1.Items)
                        {
                            foreach (RadMenuItem ms in mi.Items)
                            {
                                if (ms.NavigateUrl.Contains("Problem_Case.aspx?c=QI"))
                                    ms.Visible = false;
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

		protected void SetupPage()
		{
			hdCurrentActiveMenu.Value = SessionManager.CurrentMenuItem;

            lbBusinessCard1_out.Text = SessionManager.UserContext.UserName(); // +": " + SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME;
            lblActiveLocation.Text = SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME;
			if (SessionManager.StatOfTheDay != null  &&  UserContext.RoleAccess() != AccessMode.Partner)
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