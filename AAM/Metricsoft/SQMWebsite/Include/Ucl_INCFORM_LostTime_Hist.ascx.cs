using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Globalization;
using System.Threading;

namespace SQM.Website
{
	public partial class Ucl_INCFORM_LostTime_Hist : System.Web.UI.UserControl
	{

		const Int32 MaxTextLength = 4000;

		protected decimal companyId;

		protected int totalFormSteps;

		protected decimal incidentTypeId;
		protected string incidentType;
		protected bool IsFullPagePostback = false;

		PSsqmEntities entities;
		List<EHSFormControlStep> formSteps;

		public PageUseMode PageMode { get; set; }

		public decimal theincidentId { get; set; }

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
			}
		}

		public decimal SelectedTypeId
		{
			get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
			set { ViewState["SelectedTypeId"] = value; }
		}

		public decimal IncidentId
		{
			get { return ViewState["IncidentId"] == null ? 0 : (decimal)ViewState["IncidentId"]; }
			set { ViewState["IncidentId"] = value; }
		}
		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
		}
		public decimal NewIncidentId
		{
			get { return ViewState["NewIncidentId"] == null ? 0 : (decimal)ViewState["NewIncidentId"]; }
			set { ViewState["NewIncidentId"] = value; }
		}

		public INCIDENT WorkStatusIncident
		{
			get { return ViewState["WorkStatusINCIDENT"] == null ? null : (INCIDENT)ViewState["WorkStatusINCIDENT"]; }
			set { ViewState["WorkStatusINCIDENT"] = value; }
		}


		protected decimal EditIncidentTypeId
		{
			get { return IncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(IncidentId); }
		}


		protected void Page_Init(object sender, EventArgs e)
		{
			if (SessionManager.SessionContext != null)
			{
				if (IsFullPagePostback)
					rptLostTime.DataBind();
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			if (IsPostBack)
			{
				// Since IsPostBack is always TRUE for every invocation of this user control we need some way 
				// to determine whether or not to refresh its page controls, or just data bind instead.  
				// Here we are using the "__EVENTTARGET" form event property to see if this user control is loading 
				// because of certain page control events that are supposed to be fired off as actual postbacks.  

				IsFullPagePostback = false;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);

					if (targetControl != null)
						if ((this.Page.FindControl(targetID).ID == "rddlWorkStatus") ||
							(this.Page.FindControl(targetID).ID == "btnSubnavSave") ||
							(this.Page.FindControl(targetID).ID == "btnAddFinal"))
							IsFullPagePostback = true;
				}
				else
					// The postback is coming from btnSubnavSave 
					IsFullPagePostback = true;
			}																			

			//if (!IsFullPagePostback)
			//	PopulateInitialForm();
		}


		protected override void FrameworkInitialize()
		{
			//String selectedLanguage = "es";
			if (SessionManager.SessionContext != null)
			{
				String selectedLanguage = SessionManager.SessionContext.Language().NLS_LANGUAGE;
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

				base.FrameworkInitialize();
			}
		}


		public void PopulateInitialForm()
		{
			lblStatusMsg.Visible = false;
			PSsqmEntities entities = new PSsqmEntities();

			IncidentId = (IsEditContext) ? IncidentId : NewIncidentId;

			if (IncidentId > 0)
				try
				{
					WorkStatusIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).Single();
					PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)WorkStatusIncident.DETECT_PLANT_ID, "");
					if (plant != null)
						IncidentLocationTZ = plant.LOCAL_TIMEZONE;

				}
				catch { }

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(typeId);
			totalFormSteps = formSteps.Count();

			InitializeForm();
		}


		void InitializeForm()
		{
			IncidentId = (IsEditContext) ? IncidentId : NewIncidentId;

			//SetUserAccess("INCFORM_LOSTTIME_HIST");

			if (IncidentId > 0)
			{
				pnlLostTime.Visible = true;
				if (PageMode == PageUseMode.ViewOnly)
				{
					divTitle.Visible = true;
					lblFormTitle.Text = Resources.LocalizedText.LostTimeHistory;
				}
				rptLostTime.DataSource = EHSIncidentMgr.GetLostTimeList(IncidentId);
				rptLostTime.DataBind();
				//EHSIncidentMgr.CalculateWorkStatusSummary(EHSIncidentMgr.CalculateWorkStatusAccounting(new PSsqmEntities(), IncidentId, null, null));
			}
		}

		public void rptLostTime_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				//int minRowsToValidate = 1;

				try
				{
					INCFORM_LOSTTIME_HIST losttime = (INCFORM_LOSTTIME_HIST)e.Item.DataItem;

					RadDropDownList rddlw = (RadDropDownList)e.Item.FindControl("rddlWorkStatus");
					//rddlw.SelectedIndexChanged += rddlw_SelectedIndexChanged;

					TextBox tbr = (TextBox)e.Item.FindControl("tbRestrictDesc");
					RadDatePicker bd = (RadDatePicker)e.Item.FindControl("rdpBeginDate");
					bd = SQMBasePage.SetRadDateCulture(bd, "");
					RadDatePicker md = (RadDatePicker)e.Item.FindControl("rdpNextMedDate");
					md = SQMBasePage.SetRadDateCulture(md, "");
					RadDatePicker ed = (RadDatePicker)e.Item.FindControl("rdpExpectedReturnDT");
					ed = SQMBasePage.SetRadDateCulture(ed, "");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					System.Web.UI.HtmlControls.HtmlTableRow trMd = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trNextMedDate");
					System.Web.UI.HtmlControls.HtmlTableRow trEd = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trExpectedReturnDate");

					rddlw.Items.Add(new DropDownListItem("", ""));
					List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
					foreach (var s in statuses)
					{
						rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
					}

					if (losttime.WORK_STATUS != null)
						rddlw.SelectedValue = losttime.WORK_STATUS;

					rddlw.SelectedValue = losttime.WORK_STATUS;
					tbr.Text = losttime.ITEM_DESCRIPTION;
					bd.SelectedDate = losttime.BEGIN_DT;
					//rd.SelectedDate = losttime.RETURN_TOWORK_DT;
					md.SelectedDate = losttime.NEXT_MEDAPPT_DT;
					ed.SelectedDate = losttime.RETURN_EXPECTED_DT;

					// Set user access:
					rddlw.Enabled = tbr.Enabled = bd.Enabled = md.Enabled = ed.Enabled = itmdel.Visible = PageMode == PageUseMode.ViewOnly ? false : SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);

					switch (rddlw.SelectedValue)
					{
						case "":
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = trMd.Visible = false;
							ed.Visible = trEd.Visible = false;
							break;
						case "01":      // Return Restricted Duty
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = true;
							ed.Visible = trEd.Visible = false;
							//rvfr.Enabled = true;
							break;
						case "02":      // Return to Work
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = trMd.Visible = false;
							ed.Visible = trEd.Visible = false;
							break;
						case "03":      // Additional Lost Time
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = true;
							ed.Visible = true;
							//rvfr.Enabled = true;
							break;
					}
				}
				catch { }
			}

			// btnAddLostTime.Visible = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);

			btnSave.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, WorkStatusIncident.INCFORM_LAST_STEP_COMPLETED);  // can log lost time ?
			if (btnSave.Visible == false)
				btnSave.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.config, WorkStatusIncident.INCFORM_LAST_STEP_COMPLETED, (int)IncidentStepStatus.workstatus);  // check if has closed incident priv
			btnAddLostTime.Visible = btnSave.Visible;
		}

		protected void AddDelete_Click(object sender, EventArgs e)
		{
			rptLostTime_ItemCommand(sender, null);
		}

		public int AddUpdateINCFORM_LOSTTIME_HIST(decimal incidentId)
		{
			lblStatusMsg.Visible = false;
			var itemList = new List<INCFORM_LOSTTIME_HIST>();
			int status = 0;
			bool allFieldsComplete = true;

			foreach (RepeaterItem losttimeitem in rptLostTime.Items)
			{
				if (losttimeitem.ItemType == ListItemType.AlternatingItem || losttimeitem.ItemType == ListItemType.Item)
				{
					INCFORM_LOSTTIME_HIST item = new INCFORM_LOSTTIME_HIST();

					Label lb = (Label)losttimeitem.FindControl("lbItemSeq");
					RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
					TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
					RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
					RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
					RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

					if (rddlw.SelectedItem == null || string.IsNullOrEmpty(rddlw.SelectedValue) || bd.SelectedDate.HasValue == false)
					{
						allFieldsComplete = false;
					}
					else
					{
						item.WORK_STATUS = rddlw.SelectedValue;
						item.ITEM_DESCRIPTION = tbr.Text;
						item.BEGIN_DT = bd.SelectedDate;
						//item.RETURN_TOWORK_DT = rd.SelectedDate;
						item.NEXT_MEDAPPT_DT = md.SelectedDate;
						item.RETURN_EXPECTED_DT = ed.SelectedDate;
					}

					itemList.Add(item);
				}
			}

			if (itemList.Count > 0)
			{
				if (allFieldsComplete)
				{
					status = SaveLostTime(incidentId, itemList);
				}
				else
				{
					lblStatusMsg.Text = Resources.LocalizedText.ENVProfileRequiredsMsg;
					lblStatusMsg.Visible = true;
					status = -1;
				}
			}

			return status;
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (AddUpdateINCFORM_LOSTTIME_HIST(IncidentId) >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				InitializeForm();
			}
		}

		private int SaveLostTime(decimal incidentId, List<INCFORM_LOSTTIME_HIST> itemList)
		{

			int status = 0;

			PSsqmEntities entities = new PSsqmEntities();

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_LOSTTIME_HIST WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_LOSTTIME_HIST item in itemList)
			{
				var newItem = new INCFORM_LOSTTIME_HIST();

				if (!String.IsNullOrEmpty(item.WORK_STATUS) && item.WORK_STATUS != "")
				{
					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.WORK_STATUS = item.WORK_STATUS;
					newItem.BEGIN_DT = item.BEGIN_DT;
					newItem.RETURN_TOWORK_DT = item.RETURN_TOWORK_DT;
					newItem.NEXT_MEDAPPT_DT = item.NEXT_MEDAPPT_DT;
					newItem.RETURN_EXPECTED_DT = item.RETURN_EXPECTED_DT;

					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);

					entities.AddToINCFORM_LOSTTIME_HIST(newItem);
					status = entities.SaveChanges();
				}
			}

			if (seq > 0)
			{
				EHSIncidentMgr.UpdateIncidentStatus(incidentId, IncidentStepStatus.workstatus, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
			}

			if (status > -1)
			{
				EHSNotificationMgr.NotifyIncidentStatus(WorkStatusIncident, ((int)SysPriv.update).ToString(), "Work status updated");
			}

			return status;
		}


		protected void rptLostTime_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			string cmd = "";

			if (source is Button)
			{
				Button btn = (Button)source;
				cmd = btn.CommandArgument;
			}
			else
			{
				cmd = e.CommandArgument.ToString();
			}

			if (cmd == "AddAnother")
			{
				var itemList = new List<INCFORM_LOSTTIME_HIST>();

				foreach (RepeaterItem losttimeitem in rptLostTime.Items)
				{
					var item = new INCFORM_LOSTTIME_HIST();

					Label lb = (Label)losttimeitem.FindControl("lbItemSeq");

					RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
					//rddlw.SelectedIndexChanged += rddlw_SelectedIndexChanged;

					TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
					RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
					bd = SQMBasePage.SetRadDateCulture(bd, "");
					RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
					md = SQMBasePage.SetRadDateCulture(md, "");
					RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");
					ed = SQMBasePage.SetRadDateCulture(ed, "");

					rddlw.Items.Add(new DropDownListItem("", ""));
					List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
					foreach (var s in statuses)
					{
						rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
					}

					if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != ""))
						item.WORK_STATUS = rddlw.SelectedValue;

					item.ITEM_DESCRIPTION = tbr.Text;
					item.BEGIN_DT = bd.SelectedDate;
					//item.RETURN_TOWORK_DT = rd.SelectedDate;
					item.NEXT_MEDAPPT_DT = md.SelectedDate;
					item.RETURN_EXPECTED_DT = ed.SelectedDate;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_LOSTTIME_HIST();

				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.WORK_STATUS = null;
				emptyItem.BEGIN_DT = null;
				emptyItem.RETURN_TOWORK_DT = null;
				emptyItem.NEXT_MEDAPPT_DT = null;
				emptyItem.RETURN_EXPECTED_DT = null;

				itemList.Add(emptyItem);

				rptLostTime.DataSource = itemList;
				rptLostTime.DataBind();

				Label lbResultsCtl = (Label)this.Page.FindControl("lbResults");
				if (lbResultsCtl != null)
					lbResultsCtl.Text = "";
			}

			else if (cmd == "Delete")
			{
				int delId = e.Item.ItemIndex;
				int sequence = -1;
				var itemList = new List<INCFORM_LOSTTIME_HIST>();

				foreach (RepeaterItem losttimeitem in rptLostTime.Items)
				{
					var item = new INCFORM_LOSTTIME_HIST();
					++sequence;
					Label lb = (Label)losttimeitem.FindControl("lbItemSeq");

					RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
					//rddlw.SelectedIndexChanged += rddlw_SelectedIndexChanged;

					TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
					RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
					bd = SQMBasePage.SetRadDateCulture(bd, "");
					RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
					md = SQMBasePage.SetRadDateCulture(md, "");
					RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");
					ed = SQMBasePage.SetRadDateCulture(ed, "");

					rddlw.Items.Add(new DropDownListItem("", ""));
					List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
					foreach (var s in statuses)
					{
						rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
					}

					if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != ""))
						item.WORK_STATUS = rddlw.SelectedValue;

					if (sequence != delId)
					{
						item.ITEM_DESCRIPTION = tbr.Text;
						item.BEGIN_DT = bd.SelectedDate;
						//item.RETURN_TOWORK_DT = rd.SelectedDate;
						item.NEXT_MEDAPPT_DT = md.SelectedDate;
						item.RETURN_EXPECTED_DT = ed.SelectedDate;
						itemList.Add(item);
					}
				}

				rptLostTime.DataSource = itemList;
				rptLostTime.DataBind();

				decimal incidentId = (IsEditContext) ? IncidentId : NewIncidentId;
				SaveLostTime(incidentId, itemList);

			}
		}

		protected void rddlw_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Now rebuild the datasource

			int seqnumber = 0;
			var itemList = new List<INCFORM_LOSTTIME_HIST>();

			foreach (RepeaterItem losttimeitem in rptLostTime.Items)
			{
				var item = new INCFORM_LOSTTIME_HIST();

				Label lb = (Label)losttimeitem.FindControl("lbItemSeq");

				RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
				//rddlw.SelectedIndexChanged += rddlw_SelectedIndexChanged;

				TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
				RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
				RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
				RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

				rddlw.Items.Add(new DropDownListItem("", ""));
				List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
				foreach (var s in statuses)
				{
					rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
				}

				if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != ""))
					item.WORK_STATUS = rddlw.SelectedValue;

				item.ITEM_DESCRIPTION = tbr.Text;
				item.BEGIN_DT = bd.SelectedDate;
				//item.RETURN_TOWORK_DT = rd.SelectedDate;
				item.NEXT_MEDAPPT_DT = md.SelectedDate;
				item.RETURN_EXPECTED_DT = ed.SelectedDate;

				itemList.Add(item);
			}

			rptLostTime.DataSource = itemList;
			rptLostTime.DataBind();
		}
	}
}