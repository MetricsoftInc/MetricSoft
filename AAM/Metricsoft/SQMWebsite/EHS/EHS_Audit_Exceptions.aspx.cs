using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;

namespace SQM.Website
{
	public partial class EHS_Audit_Exceptions : SQMBasePage
	{
		protected enum DisplayState
		{
			AuditExceptionList,
			AuditExceptionClosed
		}

		// Mode should be "audit" (standard) or "prevent" (RMCAR)
		public AuditMode Mode
		{
			get { return ViewState["Mode"] == null ? AuditMode.Audit : (AuditMode)ViewState["Mode"]; }
			set { ViewState["Mode"] = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

		}

		protected void Page_Load(object sender, EventArgs e)
		{

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclAuditExceptionList.AuditListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbAuditType);

			if (Request.QueryString["mode"] != null)
			{
				string mode = Request.QueryString["mode"].ToString().ToLower();
				if (!string.IsNullOrEmpty(mode))
				{
					if (mode == "audit")
						this.Mode = AuditMode.Audit;
				}
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

			bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);


			if (IsPostBack)
			{
				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayAudits":
								UpdateDisplayState(DisplayState.AuditExceptionList);
								break;

							//case "Notification":
							//	//UpdateDisplayState(DisplayState.AuditNotificationEdit); 
							//	//UpdateDisplayState(DisplayState.AuditReportEdit);
							//	uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
							//	UpdateDisplayState(DisplayState.AuditNotificationEdit);
							//	if (isDirected)
							//	{
							//		rbNew.Visible = false;
							//		uclAuditForm.EnableReturnButton(false);
							//	}
							//	break;

							//case "Closed":
							//	uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
							//	UpdateDisplayState(DisplayState.AuditNotificationClosed);
							//	if (isDirected)
							//	{
							//		rbNew.Visible = false;
							//		uclAuditForm.EnableReturnButton(false);
							//	}
							//	break;
							//case "DisplayOnly":
							//	uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
							//	UpdateDisplayState(DisplayState.AuditNotificationDisplay);
							//	if (isDirected)
							//	{
							//		rbNew.Visible = false;
							//		uclAuditForm.EnableReturnButton(false);
							//	}
							//	break;
						}
					}
					SessionManager.ClearReturns();
				}
			}
			else
			{
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
				RadPersistenceManager1.LoadState();
			}
			catch
			{
			}

			if (SessionManager.ReturnStatus == null || SessionManager.ReturnStatus != true)
				//if ( SessionManager.ReturnObject == null)
				SearchAudits();      // suppress list when invoking page from inbox

		}

		protected void UpdateDisplayState(DisplayState state)
		{
			switch (state)
			{
				case DisplayState.AuditExceptionList:
					SearchAudits();
					break;

				case DisplayState.AuditExceptionClosed: // ??
					divAuditList.Visible = false;
					break;

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
			divAuditList.Visible = true;

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;

			// we want the start date to be the previous week Monday
			//dmFromDate.SelectedDate = DateTime.Now.AddDays(-1);
			//dmToDate.SelectedDate = DateTime.Now.AddMonths(1);

			int dayofweek = (int)DateTime.Now.DayOfWeek;
			DateTime fromDate = DateTime.Now.AddDays(-7);
			while ((int)(fromDate.DayOfWeek) > 1)
			{
				fromDate = fromDate.AddDays(-1);
			}
			// we want the end date to be the current week Monday
			DateTime toDate = DateTime.Now;
			while ((int)(toDate.DayOfWeek) > 1)
			{
				toDate = toDate.AddDays(-1);
			}

			dmFromDate.SelectedDate = fromDate;
			dmToDate.SelectedDate = toDate;

			if (Mode == AuditMode.Audit)
			{

				lblViewEHSRezTitle.Text = "Environmental Health &amp; Safety Audit Exceptions";
				lblPageInstructions.Text = "Review and update EH&amp;S Audit Exceptions below.";
				//lblStatus.Text = "Audit Status:";
				lblAuditDate.Visible = true;
				phAudit.Visible = true;

				SETTINGS sets = SQMSettings.GetSetting("EHS", "AUDITEXCEPTIONSEARCHFROM");
				if (sets != null)
				{
					try
					{
						string[] args = sets.VALUE.Split('-');
						if (args.Length > 1)
						{
							dmFromDate.SelectedDate = new DateTime(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
						}
						else
						{
							dmFromDate.SelectedDate = DateTime.Now.AddMonths(Convert.ToInt32(args[0]) * -1);
						}
					}
					catch { }
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
			}
		}

		#region auditaging

		protected void btnAuditsSearchClick(object sender, EventArgs e)
		{
			//SearchAudits();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void btnFindingsSearchClick(object sender, EventArgs e)
		{
			SearchAudits();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		private void SearchAudits()
		{
			string selectedValue = "";
			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate);
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

			List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();

			var typeList = new List<decimal>();
			List<string> statusList = new List<string>();

			if (Mode == AuditMode.Audit)
			{
				if (HSCalcs() == null)
				{
					foreach (RadComboBoxItem item in rcbAuditType.Items)
						item.Checked = true;
				}
				typeList = rcbAuditType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
				selectedValue = rcbStatusSelect.SelectedValue;
			}

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			//HSCalcs().ObjAny = cbShowImage.Checked;

			HSCalcs().ehsCtl.SelectAuditExceptionList(plantIDS, typeList, fromDate, toDate);

			// may want to access only the ones assigned to that person
			//if (accessLevel < AccessMode.Admin)
			//	HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.ISSUE_TYPE_ID != 10 select i).ToList();
			bool allAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
			if (!allAuditAccess)
				HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.AUDIT_PERSON == SessionManager.UserContext.Person.PERSON_ID select i).ToList();

			if (HSCalcs().ehsCtl.AuditHst != null)
			{
				HSCalcs().ehsCtl.AuditHst.OrderByDescending(x => x.Audit.AUDIT_DT);
				uclAuditExceptionList.BindAuditListRepeater(HSCalcs().ehsCtl.AuditHst, "EHS");
			}
			//}

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

		#region export

		protected void ExportClick(string cmd)
		{
			//if (Mode == AuditMode.Prevent)
			//	uclExport.GeneratePreventativeActionExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
			//else

			//uclExport.GenerateAuditExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
		}
		protected void lnkExportClick(object sender, EventArgs e)
		{
			//uclExport.GenerateAuditExportExcel(entities, HSCalcs().ehsCtl.AuditHst);
		}
		#endregion

	}

}
