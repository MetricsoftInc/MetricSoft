using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;
using System.Web.UI.HtmlControls;

namespace SQM.Website
{
	public partial class EHS_Audit_Scheduler : SQMBasePage
	{
		protected enum DisplayState
		{
			AuditScheduleList,
			AuditScheduleNew,
			AuditScheduleEdit,
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.EHSAudits;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ":";
			this.lblAuditType.Text = Resources.LocalizedText.AssessmentType + ":";

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclAuditScheduleList.AuditScheduleListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbAuditType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbScheduleDay);
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

			bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.audit);
			if (rbNew.Visible)
				rbNew.Visible = createAuditAccess;


			if (IsPostBack)
			{
				if (!uclAuditScheduleDetail.IsEditContext)
				{
					RadPersistenceManager1.StorageProviderKey = SessionManager.UserContext.Person.PERSON_ID.ToString();
					RadPersistenceManager1.SaveState();
				}

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayAuditSchedules":
								UpdateDisplayState(DisplayState.AuditScheduleList);
								break;

							case "Notification":
								uclAuditScheduleDetail.EditAuditScheduleId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.AuditScheduleEdit);
									rbNew.Visible = false;
									uclAuditScheduleDetail.EnableReturnButton(true);
								break;

							case "Closed":
								uclAuditScheduleDetail.EditAuditScheduleId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.AuditScheduleNew);
									rbNew.Visible = false;
									uclAuditScheduleDetail.EnableReturnButton(false);
								break;
						}
					}
					SessionManager.ClearReturns();
				}
			}
			else
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("EHS", 2, true, true, "");
				}

				if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
				{
					try
					{
						// from inbox
						DisplayNonPostback();
						SessionManager.ReturnRecordID = Convert.ToDecimal(SessionManager.ReturnObject.ToString());
						SessionManager.ReturnObject = "Notification";
						SessionManager.ReturnStatus = true;

						StringBuilder sbScript = new StringBuilder();
						ClientScriptManager cs = Page.ClientScript;

						sbScript.Append("<script language='JavaScript' type='text/javascript'>\n");
						sbScript.Append("<!--\n");
						sbScript.Append(cs.GetPostBackEventReference(this, "PBArg") + ";\n");
						sbScript.Append("// -->\n");
						sbScript.Append("</script>\n");

						cs.RegisterStartupScript(this.GetType(), "AutoPostBackScript", sbScript.ToString());
					}
					catch
					{
						// not a number, parse as type
						DisplayNonPostback();
					}
				}
				else
				{
					DisplayNonPostback();
				}

			}
		}

		protected void DisplayNonPostback()
		{
			SetupPage();

			try
			{
				RadPersistenceManager1.StorageProviderKey = SessionManager.UserContext.Person.PERSON_ID.ToString();
				RadPersistenceManager1.LoadState();
			}
			catch
			{
			}

			if (SessionManager.ReturnStatus == null || SessionManager.ReturnStatus != true)
				//if ( SessionManager.ReturnObject == null)
				SearchAuditSchedulers();      // suppress list when invoking page from inbox

		}

		protected void UpdateDisplayState(DisplayState state)
		{
			switch (state)
			{
				case DisplayState.AuditScheduleList:
					SearchAuditSchedulers();
					uclAuditScheduleDetail.Visible = false;
					rbNew.Visible = true;
					break;

				case DisplayState.AuditScheduleNew:
					divAuditList.Visible = false;
					uclAuditScheduleDetail.Visible = true;
					uclAuditScheduleDetail.IsEditContext = false;
					uclAuditScheduleDetail.ClearControls();
					rbNew.Visible = false;
					//uclAuditScheduleDetail.CheckForSingleType();
					break;

				case DisplayState.AuditScheduleEdit:
					divAuditList.Visible = false;
					uclAuditScheduleDetail.CurrentStep = 0;
					uclAuditScheduleDetail.IsEditContext = true;
					uclAuditScheduleDetail.Visible = true;
					rbNew.Visible = false;
					break;

				//case DisplayState.AuditNotificationClosed:
				//	divAuditList.Visible = false;
				//	uclAuditForm.CurrentStep = 1;
				//	uclAuditForm.IsEditContext = false;
				//	rbNew.Visible = false;
				//	uclAuditForm.Visible = true;
				//	//uclAuditForm.BuildForm();
				//	break;

			}

			SessionManager.ClearReturns();
		}

		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList), 0);

				rcbStatusSelect.SelectedValue = "A";
			}

			if (rcbAuditType.Items.Count < 1)
			{
				foreach (AUDIT_TYPE ip in EHSAuditMgr.SelectAuditTypeList(SessionManager.PrimaryCompany().COMPANY_ID, false))
				{
					RadComboBoxItem item = new RadComboBoxItem(ip.TITLE, ip.AUDIT_TYPE_ID.ToString());
					item.Checked = true;
					rcbAuditType.Items.Add(item);
				}
			}
			if (rcbScheduleDay.Items.Count < 1)
			{
				IDictionary<int, string> days = GetAll<DayOfWeek>();
				foreach(var day in days )
				{
					RadComboBoxItem item = new RadComboBoxItem(day.Value, day.Key.ToString());
					item.Checked = true;
					rcbScheduleDay.Items.Add(item);
				}
				//rcbScheduleDay.DataSource = GetAll<DayOfWeek>();
				//rcbScheduleDay.DataTextField = "Value";
				//rcbScheduleDay.DataValueField = "Key";
				//rcbScheduleDay.DataBind();
			}

			divAuditList.Visible = true;

			lblViewEHSRezTitle.Text = GetLocalResourceObject("lblViewEHSRezTitleResource1.Text").ToString();
			//lblPageInstructions.Text = "Add or update EH&amp;S Assessment Schedule below.";
			rbNew.Text = GetLocalResourceObject("rbNewResource1.Text").ToString();
			phAudit.Visible = true;
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			rbNew.Visible = false;
			UpdateDisplayState(DisplayState.AuditScheduleNew);
		}

		#region auditaging

		protected void btnAuditsSearchClick(object sender, EventArgs e)
		{
			SearchAuditSchedulers();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void lnkCloseDetails(object sender, EventArgs e)
		{
			pnlAuditDetails.Visible = lnkAuditDetailsClose.Visible = false;
		}

		private void SearchAuditSchedulers()
		{
			string selectedValue = "";
			List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();

			var typeList = new List<decimal>();
				typeList = rcbAuditType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
				
			selectedValue = rcbStatusSelect.SelectedValue;
			List<int> dayofweekList = SQMBasePage.GetComboBoxCheckedItems(rcbScheduleDay).Select(i => Convert.ToInt32(i.Value)).ToList();
			EHSCalcsCtl calcs = new EHSCalcsCtl();
			List<EHSAuditSchedulerData> scheduler = calcs.SelectAuditSchedulerList(plantIDS, typeList, dayofweekList, selectedValue);
			uclAuditScheduleList.BindAuditListRepeater(scheduler, "EHS");
			pnlAuditDetails.Visible = lnkAuditDetailsClose.Visible = false;
			HtmlGenericControl div = (HtmlGenericControl)uclAuditScheduleDetail.FindControl("divAuditForm");
			div.Visible = false;
		}

		#endregion


		#region formatting helpers

		protected string GetPlantName(decimal auditId)
		{
			string content = "";
			content = EHSAuditMgr.SelectAuditLocationNameByAuditId(auditId);
			return content;
		}

		protected string TruncatedText(string text, int maxLength)
		{
			if (text.Length > maxLength)
				return text.Substring(0, maxLength - 2) + "…";
			else
				return text;
		}

		#endregion

		// manage current session object  (formerly was page static variable)
		SQMMetricMgr HSCalcs()
		{
			if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is SQMMetricMgr)
				return (SQMMetricMgr)SessionManager.CurrentObject;
			else
				return null;
		}
		SQMMetricMgr SetHSCalcs(SQMMetricMgr hscalcs)
		{
			SessionManager.CurrentObject = hscalcs;
			return HSCalcs();
		}

		public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
		{
			var enumerationType = typeof(TEnum);

			if (!enumerationType.IsEnum)
				throw new ArgumentException("Enumeration type is expected.");

			var dictionary = new Dictionary<int, string>();

			foreach (int value in Enum.GetValues(enumerationType))
			{
				var name = Enum.GetName(enumerationType, value);
				dictionary.Add(value, name);
			}

			return dictionary;
		}

	}

}
