using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Telerik.Web.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.XSSF.UserModel;

namespace SQM.Website
{
	public partial class Administrate_SettingInput : SQMBasePage
	{
		static SETTINGS selectedCode;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			uclSearchBar.OnCancelClick += uclSearchBar_OnCancelClick;
			uclSearchBar.OnSaveClick += uclSearchBar_OnSaveClick;
		}

		private void ClearPage()
		{
			lblCodeValue.Text = "";
			lblShortDescription.Text = "";
			lblLongDescription.Text = "";
			tbValue.Enabled = false;
			tbValue.TextMode = TextBoxMode.MultiLine;
			tbValue.Attributes.Add("value", "");
			tbValue.Text = "";
			tbValueConfirm.Attributes.Add("value", "");
			tbValueConfirm.Text = "";
			trConfirm.Visible = false;
			selectedCode = null;
			uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
			uclSearchBar.SetButtonsVisible(false, false, false, true, false, false);
			uclSearchBar.SetButtonsNotClicked();
			uclSearchBar.TitleItem.Text = "";
			hdnEncrypt.Value = "False";
		}

		private void uclSearchBar_OnCancelClick()
		{
			ClearPage();
		}

		private void uclSearchBar_OnSaveClick()
		{
			lblErrorUpdating.Visible = false;
			lblConfirmMustMatch.Visible = false;
			if (hdnEncrypt.Value.ToString().Equals("True") && !tbValue.Text.ToString().Equals(tbValueConfirm.Text.ToString()))
			{
				lblConfirmMustMatch.Visible = true;
				return;
			}
			bool success = SaveSettings(true);

			if (success)
			{
				ClearPage();
				List<SETTINGS> set = SQMSettings.SelectSettingsGroupExposed(ddlSettingGroup.SelectedValue.ToString(), ddlSettingFamily.SelectedValue.ToString());
				rptSettingList.DataSource = set;
				rptSettingList.DataBind();
			}
			else
			{
				lblErrorUpdating.Visible = true;
			}
		}

		protected void Page_Prerender(object sender, EventArgs e)
		{

			if (!IsPostBack)
			{
				SetupPage();
			}
		}				

		private void SetupPage()
		{
			string[] familyArray;
			string[] groupArray;
			List<SETTINGS> settings = SQMSettings.SelectSettingsGroupExposed("", "");
			// select all of the Families
			familyArray = settings.Select(s => s.SETTING_FAMILY).Distinct().ToArray();
			// Select all of the groups for the default family
			if (familyArray.Count() > 0)
			{
				ddlSettingFamily.DataSource = familyArray;
				ddlSettingFamily.DataBind();

				groupArray = settings.Where(s => s.SETTING_FAMILY == familyArray[0]).Select(s => s.SETTING_GROUP).Distinct().ToArray();
				ddlSettingGroup.DataSource = groupArray;
				ddlSettingGroup.DataBind();

				if (groupArray.Count() > 0)
				{
					// we need to populate the repeater
					List<SETTINGS> set = SQMSettings.SelectSettingsGroupExposed(groupArray[0], familyArray[0]);
					rptSettingList.DataSource = set;
					rptSettingList.DataBind();
				}
			}
			ClearPage();
		}

		protected void DisplaySetting(string encrypt)
		{
			lblCodeValue.Text = selectedCode.SETTING_CD;
			lblShortDescription.Text = selectedCode.XLAT_SHORT;
			lblLongDescription.Text = selectedCode.XLAT_LONG;
			hdnEncrypt.Value = encrypt;
			if (encrypt.Equals("True"))
			{
				trConfirm.Visible = true;
				tbValue.TextMode = TextBoxMode.SingleLine;
				tbValue.TextMode = TextBoxMode.Password;
				tbValueConfirm.Attributes.Add("value", selectedCode.VALUE);
				tbValue.Attributes.Add("value", selectedCode.VALUE);
			}
			else
			{
				tbValueConfirm.Visible = false;
				tbValue.TextMode = TextBoxMode.MultiLine;
				tbValue.Attributes.Add("value", selectedCode.VALUE);
				tbValue.Text = selectedCode.VALUE;
			}
		}

		protected bool SaveSettings(bool updateSetting)
		{
			PSsqmEntities Entities = new PSsqmEntities();
			string strCode = selectedCode.SETTING_CD;
			SETTINGS settings = SQMSettings.SelectSettingByCode(Entities, ddlSettingGroup.SelectedValue.ToString(), ddlSettingFamily.SelectedValue.ToString(), strCode);

			bool success;

			// AW20131106 - need to verify that the SSO_ID and email address are unique in the system 
			bool bErrors = false;
			settings.VALUE = tbValue.Text.ToString();
			// update the code
			selectedCode = SQMSettings.UpdateSettings(Entities, settings, SessionManager.UserContext.UserName());
			return true;
		}


		protected void ddlSettingFamily_SelectedIndexChanged(object sender, EventArgs e)
		{
			string[] groupArray;
			List<SETTINGS> settings = SQMSettings.SelectSettingsGroupExposed("", ddlSettingFamily.SelectedValue.ToString());

			groupArray = settings.Select(s => s.SETTING_GROUP).Distinct().ToArray();
			ddlSettingGroup.DataSource = groupArray;
			ddlSettingGroup.DataBind();

			//if (groupArray.Count() > 0)
			//{
			//	// we need to populate the repeater
			//	List<SETTINGS> set = SQMSettings.SelectSettingsGroup(groupArray[0], "");
			//	rptSettingList.DataSource = set;
			//	rptSettingList.DataBind();
			//}

		}

		protected void lnkEditCode_Click(object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] args = lnk.CommandArgument.ToString().Split('~');
			selectedCode = SQMSettings.SelectSettingByCode(entities, ddlSettingGroup.SelectedValue.ToString(), ddlSettingFamily.SelectedValue.ToString(), args[0]);
			uclSearchBar.SetButtonsEnabled(false, false, false, true, false, false);
			uclSearchBar.SetButtonsVisible(false, false, false, true, false, false);
			uclSearchBar.SetButtonsNotClicked();
			uclSearchBar.TitleItem.Text = selectedCode.SETTING_CD;
			tbValue.Enabled = true;
			DisplaySetting(args[1]);
		}

		protected void rptSettingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					SETTINGS settings = (SETTINGS)e.Item.DataItem;
					LinkButton lnk = (LinkButton)e.Item.FindControl("lnkEditCode");
					if (settings.ENCRYPT_VALUE)
					{
						Label lblValue = (Label)e.Item.FindControl("lblValue");
						lblValue.Text = "";
						foreach (char c in settings.VALUE)
							lblValue.Text += "*";
						lnk.CommandArgument = settings.SETTING_CD + "~True";
					}
					else
						lnk.CommandArgument = settings.SETTING_CD + "~False";
				}
				catch { }
			}
		}

		protected void btnSettingSearch_Click(object sender, EventArgs e)
		{
			// we need to populate the repeater
			List<SETTINGS> set = SQMSettings.SelectSettingsGroupExposed(ddlSettingGroup.SelectedValue.ToString(), ddlSettingFamily.SelectedValue.ToString());
			rptSettingList.DataSource = set;
			rptSettingList.DataBind();
		}

	}
}