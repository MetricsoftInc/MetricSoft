﻿using System;
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
	public partial class EHS_Incidents : SQMBasePage
	{
		protected enum DisplayState
		{
			IncidentList,
			IncidentNotificationNew,
			IncidentNotificationEdit,
			IncidentReportEdit
		}

		// Mode should be "incident" (standard) or "prevent" (RMCAR)
		public IncidentMode Mode 
		{
			get { return ViewState["Mode"] == null ? IncidentMode.Incident : (IncidentMode)ViewState["Mode"]; }
			set	{ ViewState["Mode"] = value; }
		}
		public bool isDirected
		{
			get { return ViewState["isDirected"] == null ? false : (bool)ViewState["isDirected"]; }
			set { ViewState["isDirected"] = value; }
		}

		decimal companyId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclExport.OnExportClick += ExportClick;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbFindingsSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlChartType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclIncidentList.IncidentListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbIncidentType);
		  
			this.Mode = IncidentMode.Incident;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool createIncidentAccess = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);
			if (rbNew.Visible)
				rbNew.Visible = createIncidentAccess;


			if (IsPostBack)
			{
				RadPersistenceManager1.SaveState();

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayIncidents":
								UpdateDisplayState(DisplayState.IncidentList, 0);
								break;

							case "Notification":
								UpdateDisplayState(DisplayState.IncidentNotificationEdit, SessionManager.ReturnRecordID);
								if (isDirected)
								{
									rbNew.Visible = false;
								}
								break;
							default:
								break;
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
						isDirected = true;

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

			if (SessionManager.ReturnStatus == null  ||  SessionManager.ReturnStatus != true)
			//if ( SessionManager.ReturnObject == null)
				SearchIncidents();      // suppress list when invoking page from inbox

			Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
			if (ucl != null)
			{
				ucl.BindDocumentSelect("EHS", 2, true, true, "");
			}

		}

		protected void UpdateDisplayState(DisplayState state, decimal incidentID)
		{
			string key;
			switch (state)
			{
				case DisplayState.IncidentList:
					SearchIncidents();
					SessionManager.ClearReturns();
					break;
				case DisplayState.IncidentNotificationNew:
					SessionManager.ClearReturns();
					key = SQMModelMgr.GetPasswordKey();
					if (rddlNewIncidentType.SelectedItem != null  &&  EHSIncidentMgr.IsUseCustomForm(Convert.ToDecimal(rddlNewIncidentType.SelectedValue)))
					{
						Response.Redirect("/EHS/EHS_InjuryIllnessForm.aspx?i=0&l=" + WebSiteCommon.Encrypt(ddlIncidentLocation.SelectedValue, key) + "&t=" + WebSiteCommon.Encrypt(rddlNewIncidentType.SelectedValue, key));
					}
					else
					{
						Response.Redirect("/EHS/EHS_IncidentForm.aspx?i=0&l=" + WebSiteCommon.Encrypt(ddlIncidentLocation.SelectedValue, key) + "&t=" + WebSiteCommon.Encrypt(rddlNewIncidentType.SelectedValue, key));
					}
					break;
				case DisplayState.IncidentNotificationEdit:
					SessionManager.ClearReturns();
					INCIDENT theIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentID);
					if (theIncident != null)
					{
						SessionManager.SetIncidentLocation((decimal)theIncident.DETECT_PLANT_ID);
						key = SQMModelMgr.GetPasswordKey();
						if (EHSIncidentMgr.IsUseCustomForm((decimal)theIncident.ISSUE_TYPE_ID))
						{
							Response.Redirect("/EHS/EHS_InjuryIllnessForm.aspx?i=" +  WebSiteCommon.Encrypt(incidentID.ToString(), key));
						}
						else
						{
							Response.Redirect("/EHS/EHS_IncidentForm.aspx?i=" +  WebSiteCommon.Encrypt(incidentID.ToString(), key));
						}
					}

					break;
				default:
					SessionManager.ClearReturns();
					break;
			}
		}
		
		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList), 0);
			 
				rcbStatusSelect.SelectedValue = "A";
				rcbFindingsSelect.FindItemByValue("A").Checked = true;
			}
			divIncidentList.Visible = true;
			pnlChartSection.Style.Add("display", "none");
			lblChartType.Visible = ddlChartType.Visible = false;

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			dmFromDate.SelectedDate = DateTime.Now.AddMonths(-11);
			dmToDate.SelectedDate = DateTime.Now;


			lblViewEHSRezTitle.Text = "Manage Environmental Health &amp; Safety Incidents";
			lblPageInstructions.Text = "Add or update EH&amp;S Incidents below.";
			//lblStatus.Text = "Incident Status:";
			rbNew.Text = "New Incident";
			lblIncidentDate.Visible = true;
			lblInspectionDate.Visible = false;
			phPrevent.Visible = false;
			phIncident.Visible = true;

			SETTINGS sets = SQMSettings.GetSetting("EHS", "INCIDENTSEARCHFROM");
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

			foreach (INCIDENT_TYPE ip in EHSIncidentMgr.SelectIncidentTypeList(SessionManager.PrimaryCompany().COMPANY_ID))
			{
				RadComboBoxItem item = new RadComboBoxItem(ip.TITLE, ip.INCIDENT_TYPE_ID.ToString());
				item.Checked = true;
				rcbIncidentType.Items.Add(item);
			}

			// lookup charts defined for this module & app context
			PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);
			if (view != null)
			{
				ddlChartType.Items.Clear();
				ddlChartType.Items.Add(new RadComboBoxItem("", ""));
				foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.Where(l => l.STATUS != "I").OrderBy(l => l.ITEM_SEQ).ToList())
				{
					RadComboBoxItem item = new RadComboBoxItem();
					item.Text = vi.TITLE;
					item.Value = vi.ITEM_SEQ.ToString();
					item.ImageUrl = ViewModel.GetViewItemImageURL(vi);
					ddlChartType.Items.Add(item);
				}
			}

			if (UserContext.GetMaxScopePrivilege(SysScope.incident) <= SysPriv.action)
				uclExport.Visible = true;
			else
				uclExport.Visible = false;
		}

		protected void rbNew_Click(object sender, EventArgs e)
		{
			List<BusinessLocation> locationList = SessionManager.PlantList;
			locationList = UserContext.FilterPlantAccessList(locationList);
			SQMBasePage.SetLocationList(ddlIncidentLocation, locationList, 0, true);
			ddlIncidentLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";

			var incidentTypeList = EHSIncidentMgr.SelectIncidentTypeList(companyId);
			rddlNewIncidentType.DataSource = incidentTypeList;
			rddlNewIncidentType.DataTextField = "TITLE";
			rddlNewIncidentType.DataValueField = "INCIDENT_TYPE_ID";
			rddlNewIncidentType.DataBind();

			string script = "function f(){OpenNewIncidentWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnNewIncidentCreate_Click(object sender, EventArgs e)
		{
			if (ddlIncidentLocation.SelectedItem != null && rddlNewIncidentType.SelectedItem != null)
			{
				UpdateDisplayState(DisplayState.IncidentNotificationNew, 0);
			}
			else
			{
				SearchIncidents();
			}
		}
		protected void btnNewIncidentCancel_Click(object sender, EventArgs e)
		{
			SearchIncidents(); 
		}

		#region incidentaging
		
		protected void btnIncidentsSearchClick(object sender, EventArgs e)
		{
			//SearchIncidents();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}

		protected void btnFindingsSearchClick(object sender, EventArgs e)
		{
			SearchIncidents();
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}
		
		protected void lnkCloseDetails(object sender, EventArgs e)
		{
			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;
			if (ddlChartType.SelectedValue != "")
				ddlChartTypeChange(null, null);
		}

		protected void lnkCloseChart(object sender, EventArgs e)
		{
			ddlChartType.SelectedValue = "";
			lnkChartClose.Visible = lnkPrint.Visible = false;
			ddlChartTypeChange(null, null);
		}

		protected void ddlChartTypeChange(object sender, EventArgs e)
		{
			divChart.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
			if (ddlChartType.SelectedValue == "" || HSCalcs().ehsCtl.IncidentHst == null || HSCalcs().ehsCtl.IncidentHst.Count == 0)
			{
				pnlChartSection.Style.Add("display", "none");
				lnkChartClose.Visible = lnkPrint.Visible =  false;
			}
			else
			{
				PERSPECTIVE_VIEW view = null;
				divChart.Controls.Clear();

				view = ViewModel.LookupView(entities, "HSIR", "HSIR", 0);

				if (view != null)
				{
					PERSPECTIVE_VIEW_ITEM vi = view.PERSPECTIVE_VIEW_ITEM.Where(i => i.ITEM_SEQ.ToString() == ddlChartType.SelectedValue).FirstOrDefault();
					if (vi != null)
					{
						GaugeDefinition ggCfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
						ggCfg.Position = null;
						HSCalcs().ehsCtl.SetCalcParams(vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, (int)vi.SERIES_ORDER).IncidentSeries((EHSCalcsCtl.SeriesOrder)vi.SERIES_ORDER, SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToArray(), new DateTime(1900, 1, 1), DateTime.Now.AddYears(100), HSCalcs().ehsCtl.GetIncidentTopics());
						uclChart.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, HSCalcs().ehsCtl.Results, divChart);
						pnlChartSection.Style.Add("display", "inline");
						lnkChartClose.Visible = lnkPrint.Visible = true;
					   // return;
					}
				}
			}
		}

		private void SearchIncidents()
		{
			string selectedValue = "";
			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate); 
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
		   
			List<decimal>  plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();
			
			var typeList = new List<decimal>();
			List<string> statusList = new List<string>();

			if (HSCalcs() == null)
			{
				foreach (RadComboBoxItem item in rcbIncidentType.Items)
					item.Checked = true;
			}
			typeList = rcbIncidentType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
			selectedValue = rcbStatusSelect.SelectedValue;

			SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			HSCalcs().ObjAny = cbShowImage.Checked;


			HSCalcs().ehsCtl.SelectIncidentList(plantIDS, typeList, fromDate, toDate, selectedValue, cbShowImage.Checked);
				
			if (!UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident))
				HSCalcs().ehsCtl.IncidentHst = (from i in HSCalcs().ehsCtl.IncidentHst where i.Incident.ISSUE_TYPE_ID != 10 select i).ToList();

			if (HSCalcs().ehsCtl.IncidentHst != null)
			{
				uclIncidentList.BindIncidentListRepeater(HSCalcs().ehsCtl.IncidentHst.OrderByDescending(x => x.Incident.INCIDENT_ID).ToList(), "EHS", cbShowImage.Checked, false);
			}


			if (HSCalcs().ehsCtl.IncidentHst != null && HSCalcs().ehsCtl.IncidentHst.Count > 0)
				lblChartType.Visible = ddlChartType.Visible = true;

			pnlIncidentDetails.Visible = lnkIncidentDetailsClose.Visible = false;

			if (ddlChartType.SelectedValue != "")
				ddlChartTypeChange(null, null);

		}

		#endregion


		#region formatting helpers

		protected string DisplayProblemIcon(decimal incidentId)
		{
			string content = "";

			var occurs = entities.PROB_OCCUR.Where(p => p.INCIDENT_ID == incidentId);
			if (occurs.Count() > 0)
				content = "<img src=\"/images/ico-8d.png\" alt=\"Problem Case has been created for this incident\" />";

			return content;
		}

		protected string GetPlantName(decimal incidentId)
		{
			string content = "";
			content = EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(incidentId);
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
			uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}

		protected void lnkExportClick(object sender, EventArgs e)
		{
			uclExport.GenerateIncidentExportExcel(entities, HSCalcs().ehsCtl.IncidentHst);
		}
		#endregion

	}

}
