using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using System.Web.Configuration;
using Telerik.Web.UI;

namespace SQM.Website
{

	public partial class Ucl_AdminPasswordEdit : System.Web.UI.UserControl
	{
		static SQM_ACCESS selectedAccess;
		public string strCurrentControl = "none";
		public String KeyString;
		public String StrongPassword;
		public String PasswordLength;

        public event CommandClick PasswordChanged;

		#region events

		protected override void OnInit(EventArgs e)
		{
			this.Label3.Text = Resources.LocalizedText.EmailAddress + ":";

			if (strCurrentControl.Equals("login"))
            {
                lblPassMustUpdate.Visible = true;
            }
			divErrorMsg.Visible = true;
			tblPassword.Attributes.Clear();
			tblPassword.Attributes.Add("border", "0");
			tblPassword.Attributes.Add("cellspacing", "0");
			tblPassword.Attributes.Add("cellpadding", "1");
			tblPassword.Style.Clear();
			tblPassword.Style.Add("background-color", "#FCFCFC");
			tdCurrentPassword.Attributes.Add("class", "");
			lblNewPassword.CssClass = "prompt";
			lblCurrentPassword.CssClass = "prompt";
			lblConfirmPassword.CssClass = "prompt";
			tdNewPassword.Attributes.Add("class", "");
			tdConfirmPassword.Attributes.Add("class", "");
			tdConfirmPassTB.Attributes.Add("class", "");
			tdCurrentPassTB.Attributes.Add("class", "");
			tdNewPassTB.Attributes.Add("class", "");
			req1.Style.Add("background-color", "#FCFCFC !important");
			req2.Style.Add("background-color", "#FCFCFC !important");
			req3.Style.Add("background-color", "#FCFCFC !important");

		}

        public void BindPwdEdit(bool bShowButtons)
        {
            string strength = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "COMPANY", "TASK", "PasswordComplexity").VALUE;
            lblPasswordPolicy.Text = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "PASSWORDCOMPLEXITY", "TASK", strength).VALUE;
 
            string script = "function f(){OpenPasswordEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        public void UpdatePwdEdit(bool doSave)
        {
            if (doSave)
                btnPassSave_Click(btnPassSave, null);
            else
            {
                tbCurrentPassword.Text = tbNewPassword.Text = tbConfirmPassword.Text = "";
            }
        }

		protected void btnPassCancel_Click(object sender, EventArgs e)
		{
            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "AlertPostback('" + "" + "','login');", true);
		}

        protected void btnCancelSend_Click(object sender, EventArgs e)
        {
            hideControl();
        }

		protected void btnPassSave_Click(object sender, EventArgs e)
		{
            int status = 0;

            lblPassPolicyFail.Visible = false;
            lblPasswordNoConfirm.Visible = false;
			lblPassFailUpdate.Visible = false;
			lblPassFailError.Visible = false;
			lblPassFail10.Visible = false;
			divErrorMsg.Visible = false;

			// validate the password criteria
            if (tbNewPassword.Text != tbConfirmPassword.Text)
            {
                status = -1;
            }
            else
            {
                status = SQMModelMgr.ChangeUserPassword(SessionManager.UserContext.Credentials.SSO_ID, "", tbCurrentPassword.Text.ToString().Trim(), tbNewPassword.Text.ToString().Trim());
            }

			if (status == 0)
			{
				// send a confirmation email
                string strEmailCompanyName = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailFromSystem").VALUE; // WebConfigurationManager.AppSettings["MailFromSystem"];
				string strEmailBody = lblPasswordEmailBody1a.Text.ToString() + strEmailCompanyName + lblPasswordEmailBody1b.Text.ToString() + "<br><br>" + lblPasswordEmailBody2.Text.ToString();
				// ABW 20140117 - we are now usig the email on the Person record
				//string strEmailSent = WebSiteCommon.SendEmail(SessionManager.UserContext.Credentials.RECOVERY_EMAIL, lblPasswordEmailSubject.Text.ToString(), strEmailBody.Trim(), "");
				string strEmailSent = WebSiteCommon.SendEmail(SessionManager.UserContext.Person.EMAIL, strEmailCompanyName + lblPasswordEmailSubject.Text.ToString(), strEmailBody.Trim(), "");
				EHSNotificationMgr.WriteEmailLog(new PSsqmEntities(), SessionManager.UserContext.Person.EMAIL, "", strEmailCompanyName + lblPasswordEmailSubject.Text.ToString(), strEmailBody, 0, SessionManager.UserContext.Person.PERSON_ID, "user password changed", "", "");

                if (strCurrentControl.Equals("login"))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "AlertPostback('" + hfPasswordChangedSucces.Value + "','loginContinue');", true);
                }
                else
                    hideControl();

				ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
			}
			else
			{
				divErrorMsg.Visible = true;
				switch (status)
				{
                    case -1:
                        lblPasswordNoConfirm.Visible = true;
                        break;
					case 10:
                        lblPassFail10.Visible = true;
						break;
					case 100:
                        lblPassPolicyFail.Visible = true;
						break;
					case 110:
                        lblPassPolicyFail.Visible = true;
						break;
					case 120:
                        lblPassPolicyFail.Visible = true;
						break;
					case 130:
                        lblPassPolicyFail.Visible = true;
						break;
					case 140:
                        lblPassPolicyFail.Visible = true;
						break;
					default:
						lblPassFailUpdate.Visible = true;
						lblPassFailError.Visible = true;
						lblPassFailError.Text = status.ToString();
						break;
				}
                BindPwdEdit(true);
			}
			// hide the control?
		}

        public void BindPwdForgot()
        {
            string script = "function f(){OpenPasswordForgotWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            // retrieve the email from SQM_ACCESS
            lblForgotInvalidEmail.Visible = false;
            lblForgotNotSent.Visible = false;
            lblForgotNotUpdated.Visible = false;
            string strength = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "COMPANY", "TASK", "PasswordComplexity").VALUE;
            string strEmailCompanyName = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailFromSystem").VALUE;
            int msg = WebSiteCommon.RecoverPassword(tbEmail.Text.ToString(), "", strEmailCompanyName + " " + lblForgotPasswordEmailSubject.Text.ToString(), lblForgotPasswordEmailBody1a.Text.ToString() + strEmailCompanyName + lblForgotPasswordEmailBody1b.Text.ToString(), lblForgotPasswordEmailBody2.Text.ToString(), "<br><br>" + WebSiteCommon.GetXlatValueLong("passwordComplexity", strength) + "<br><br>" + lblForgotPasswordEmailBody3.Text.ToString());
            switch (msg)
            {
                case 10:
                    tbEmail.Focus();
                    lblForgotInvalidEmail.Visible = true;
                    BindPwdForgot();
                    break;
                case 20:
                    tbEmail.Focus();
                    lblForgotNotSent.Visible = true;
                    BindPwdForgot();
                    break;
                case 30:
                    tbEmail.Focus();
                    lblForgotNotUpdated.Visible = true;
                    BindPwdForgot();
                    break;
                default:
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "AlertPostback('" + hfForgotPasswordSent.Value + "','login');", true);
                    break;
            }
        }

		protected void hideControl()
		{
			if (!strCurrentControl.Equals("none"))
			{
				// need to find the wrapper control and hide it
				if (strCurrentControl.Substring(0, 3).Equals("pnl"))
				{
					Panel pnl = (Panel)this.Parent.FindControl(strCurrentControl);
                    if (pnl != null)
					    pnl.Visible = false;
				}
			}
		}
		#endregion

		#region common

		#endregion

	}
}