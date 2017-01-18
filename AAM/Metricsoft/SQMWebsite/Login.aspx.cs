using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Login : SQMBasePage
	{
		private int LoginAttempts
		{
			get { return ViewState["loginAttempts"] == null ? 0 : (int)ViewState["loginAttempts"]; }
			set { ViewState["loginAttempts"] = value; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			pnlLoginPasswordEdit.Visible = false;
			tbUsername.Text = tbPassword.Text = "";
			tbUsername.Focus();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			SETTINGS sets = null;

			if (IsPostBack)
			{
				
				// Save the current view port width (used for responsive design later)
				var vpWidth = viewPortWidth.Value;
				if (!string.IsNullOrEmpty(vpWidth))
					Session["vpWidth"] = vpWidth;
				else
					Session["vpWidth"] = "1000";
			}
			else 
			{
				try
				{
					if (SessionManager.SessionContext != null || SessionManager.UserContext != null)
					{
						SessionManager.Clear();
						Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
					}

					string info = System.Configuration.ConfigurationManager.AppSettings["MainInfo"];
					if (!string.IsNullOrEmpty(info))
					{
						lblMainInfo.Text = info;
						lblMainInfo.Visible = true;
					}

					System.Web.HttpBrowserCapabilities browser = Request.Browser;

					SETTINGS setsPwdReset = SQMSettings.SelectSettingByCode(entities, "COMPANY", "TASK", "PasswordResetEnable");
					if (setsPwdReset != null && setsPwdReset.VALUE.ToUpper() == "Y")
					{
						lnkForgotPassword.Visible = true;
					}
					
					SETTINGS setsLoginPosting = SQMSettings.SelectSettingByCode(entities, "COMPANY", "TASK", "LoginPostingsEnable");
					if (setsLoginPosting == null || setsLoginPosting.VALUE.ToUpper() == "Y")
					{

						// url format login.aspx/?t=ltc:60&p=EHS
						// execOverride == override query params to force ticker and image posting 

						System.Collections.Specialized.NameValueCollection qry = new System.Collections.Specialized.NameValueCollection();
						string execOverride = System.Configuration.ConfigurationManager.AppSettings["LoginOverride"];
						if (execOverride != null && execOverride == "QAI")
						{
							qry.Add("t", "ltc");
							qry.Add("p", "EHS,QS");
						}
						else
						{
							sets = SQMSettings.GetSetting("ENV", "LOGINIMAGE");  // force login splash page display
							if (sets != null && sets.VALUE.ToUpper() == "Y")
							{
								qry.Add("p", "EHS,QS");
							}
							sets = SQMSettings.GetSetting("ENV", "LOGINSTAT");
							if (sets != null && sets.VALUE.ToUpper() == "LTC")
							{
								qry.Add("t", "ltc");
							}
						}

						sets = SQMSettings.GetSetting("ENV", "LOGINMESSAGE");
						if (sets != null && !string.IsNullOrEmpty(sets.VALUE))
						{
							lblLoginMessage.Text = sets.VALUE;
							divLoginMessage.Visible = true;
						}


						if (Request.QueryString.Count > 0)
						{
							// AW 02/2016 - don't wipe out the current Collection, just add to it
							//qry = Request.QueryString;
							qry.Add(Request.QueryString);
						}

						if (qry.Get("t") != null)
						{
							string[] args = qry.Get("t").ToString().Split(':');

							COMPANY company = new COMPANY();
							decimal[] plantIDS = SQMModelMgr.SelectPlantList(entities, 1, 0).Where(l => l.LOCATION_TYPE == "P").OrderBy(l => l.PLANT_NAME).Select(l => l.PLANT_ID).ToArray();
							SQMMetricMgr stsmgr = new SQMMetricMgr().CreateNew(company, "0", DateTime.Now, DateTime.Now, plantIDS);
							stsmgr.ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange, "E");
							stsmgr.ehsCtl.ElapsedTimeSeries(plantIDS, new decimal[1] { 8 }, new decimal[1] { 63 }, "YES", true);

							GaugeDefinition tikCfg = new GaugeDefinition().Initialize();
							tikCfg.Width = 450; // 0;
							tikCfg.Unit = args.Length > 1 ? Convert.ToInt32(args[1]) : 80;
							tikCfg.Position = "none";
							tikCfg.NewRow = false;
							pnlPosting.Visible = true;
							divTicker.Visible = true;
							uclGauge.CreateTicker(tikCfg, stsmgr.ehsCtl.Results.metricSeries, divTicker);
						}

						if (qry.Get("p") != null)
						{
							string[] args = qry.Get("p").ToString().Split(',');
							if (args.Contains("EHS"))
							{
								pnlPosting.Visible = true;
								imgPostingEHS.Style.Add("MARGIN-TOP", "8px");
								imgPostingEHS.Src = SQM.Website.Classes.SQMDocumentMgr.GetImageSourceString(SQM.Website.Classes.SQMDocumentMgr.FindCurrentDocument("SYS", 31));
							}
							if (args.Contains("QS"))
							{
								pnlPosting.Visible = true;
								imgPostingQS.Style.Add("MARGIN-TOP", "8px");
								imgPostingQS.Src = SQM.Website.Classes.SQMDocumentMgr.GetImageSourceString(SQM.Website.Classes.SQMDocumentMgr.FindCurrentDocument("SYS", 32));
							}
						}
					}

					string externalLinks = System.Configuration.ConfigurationManager.AppSettings["ExternalLinks"];
					if (!string.IsNullOrEmpty(externalLinks))
					{
						string[] linkArray = externalLinks.Split(',');
						foreach (string link in linkArray)
						{
							divLinks.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;"));
							HyperLink hlk = new HyperLink();
							hlk.NavigateUrl = hlk.Text = link;
							hlk.Target = "_blank";
							hlk.CssClass = "linkUnderline";
							divLinks.Controls.Add(hlk);
						}
						divLinks.Visible = true;
					}
				}
				catch
				{
					divAnnouncements.Visible = false;
				}

				if (Request.QueryString["rdp"] != null)
					ViewState["RedirectPath"] = Request.QueryString["rdp"];
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				string ev = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(ev)  &&  ev.Contains("login"))
				{
					pnlLoginPasswordEdit.Visible = false;
					OnPasswordChanged(ev);
				}
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			string SSOID = tbUsername.Text;
			string pwd = tbPassword.Text.Trim();
			bool calcStatOfTheDay = true;

			++LoginAttempts;

			lblLoginError_out.Visible = lblSessionError_out.Visible = false;

			string calcStat = System.Configuration.ConfigurationManager.AppSettings["CalcStatOfTheDay"];
			if (!string.IsNullOrEmpty(calcStat) && calcStat.ToLower().Contains("false"))
				calcStatOfTheDay = false;

			LoginStatus status = status = SessionManager.InitializeUser(SSOID, pwd, true, calcStatOfTheDay);

			if (status != LoginStatus.Success)
			{
				if (status == LoginStatus.SessionInUse)
					lblSessionError_out.Visible = true;
				else
				{
					int loginAttemptLimit = 5;
					try
					{
						string limitStr = System.Configuration.ConfigurationManager.AppSettings["LoginAttemptLimit"];
						if (!string.IsNullOrEmpty(limitStr))
						{
							int limit;
							if (int.TryParse(limitStr, out limit) && limit > 0)
								loginAttemptLimit = limit;
						}
					}
					catch { }
					if (LoginAttempts >= loginAttemptLimit)
					{
						tbUsername.Enabled = tbPassword.Enabled = btnLogin.Enabled = false;
						lblLoginAttemptError_out.Visible = true;
					}
					else
					{
						lblLoginError_out.Visible = true;
					}
				}
			}
			else if (SessionManager.UserContext.Credentials.STATUS == "P" || SessionManager.UserContext.Credentials.STATUS == "N") // AW 201310 - force password reset if forgot password
			{
				pnlLogin.Visible = false;
				pnlLoginPasswordEdit.Visible = true;
				uclPassEdit.BindPwdEdit(true);
			}
			else
			{
				if (status == LoginStatus.SessionInUse)
					lblSessionError_out.Visible = true;

				SessionManager.LoginURL = Request.Url.PathAndQuery;
				// get browser type and version 
				System.Web.HttpBrowserCapabilities browser = Request.Browser;
				SessionManager.Browser = (object)browser;
				SessionManager.UserAgent = (string)HttpContext.Current.Request.UserAgent.ToLower();

				string redirectPath = (string)ViewState["RedirectPath"];
				if (string.IsNullOrEmpty(redirectPath))
				{
					SessionManager.ClearReturns();
					Response.Redirect("/Home/Calendar.aspx");
				}
				else
				{
					try
					{
						ViewState["RedirectPath"] = "";
					}
					catch { }
					Response.Redirect(redirectPath);
				}
			}
		}

		protected void lnkForgotPassword_Click(object sender, EventArgs e)
		{
			pnlLoginPasswordEdit.Visible = true;
			uclPassEdit.BindPwdForgot();
		}

		protected void lnkLogin_Click(object sender, EventArgs e)
		{
			;
		}

		private void OnPasswordChanged(string cmd)
		{
			if (cmd == "loginContinue")
			{
				Response.Redirect("/Home/Calendar.aspx");
			}
			else
			{
				SessionManager.Clear();
				Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
				Response.Redirect("/Login.aspx");
			}
		}
	}
}