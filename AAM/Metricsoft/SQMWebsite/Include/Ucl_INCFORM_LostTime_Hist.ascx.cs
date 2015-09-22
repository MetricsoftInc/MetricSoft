﻿using System;
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


		public decimal IncidentId { get; set; }
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

		public decimal EditIncidentId
		{
			get { return ViewState["EditIncidentId"] == null ? 0 : (decimal)ViewState["EditIncidentId"]; }
			set { ViewState["EditIncidentId"] = value; }
		}

		public decimal NewIncidentId
		{
			get { return ViewState["NewIncidentId"] == null ? 0 : (decimal)ViewState["NewIncidentId"]; }
			set { ViewState["NewIncidentId"] = value; }
		}

		protected decimal EditIncidentTypeId
		{
			get { return EditIncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(EditIncidentId); }
		}

		public string ValidationGroup
		{
			get { return ViewState["ValidationGroup"] == null ? " " : (string)ViewState["ValidationGroup"]; }
			set { ViewState["ValidationGroup"] = value; }
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

			if (IncidentId != null)
			{
				INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
				//if (incident != null)
				//if (incident.CLOSE_DATE != null && incident.CLOSE_DATE_DATA_COMPLETE != null)
				//btnClose.Text = "Reopen Power Outage Incident";


			}

			if (!IsFullPagePostback)
				PopulateInitialForm();

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
			PSsqmEntities entities = new PSsqmEntities();
			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(typeId);
			totalFormSteps = formSteps.Count();

			InitializeForm();

		}


		void InitializeForm()
		{
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			//SetUserAccess("INCFORM_LOSTTIME_HIST");

			if (IncidentId > 0)
			{
				pnlLostTime.Visible = true;
				rptLostTime.DataSource = EHSIncidentMgr.GetLostTimeList(IncidentId);
				rptLostTime.DataBind();
				EHSIncidentMgr.CalculateWorkStatusSummary(EHSIncidentMgr.CalculateWorkStatusAccounting(new PSsqmEntities(), IncidentId, null, null));
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
					//RadDatePicker rd = (RadDatePicker)e.Item.FindControl("rdpReturnDate");
					RadDatePicker md = (RadDatePicker)e.Item.FindControl("rdpNextMedDate");
					RadDatePicker ed = (RadDatePicker)e.Item.FindControl("rdpExpectedReturnDT");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");


					RequiredFieldValidator rvfw = (RequiredFieldValidator)e.Item.FindControl("rfvWorkStatus");
					RequiredFieldValidator rvfr = (RequiredFieldValidator)e.Item.FindControl("rfvRestrictDesc");
					RequiredFieldValidator rvfbd = (RequiredFieldValidator)e.Item.FindControl("rvfBeginDate");
					//RequiredFieldValidator rvfrd = (RequiredFieldValidator)e.Item.FindControl("rfvReturnDate");
					RequiredFieldValidator rvfmd = (RequiredFieldValidator)e.Item.FindControl("rfvNextMedDate");
					RequiredFieldValidator rvfed = (RequiredFieldValidator)e.Item.FindControl("rfvExpectedReturnDT");

					rvfw.ValidationGroup = ValidationGroup;
					rvfr.ValidationGroup = ValidationGroup;
					rvfbd.ValidationGroup = ValidationGroup;
					//rvfrd.ValidationGroup = ValidationGroup;
					rvfmd.ValidationGroup = ValidationGroup;
					rvfed.ValidationGroup = ValidationGroup;

					rddlw.Items.Add(new DropDownListItem("[Select One]", ""));
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
					rddlw.Enabled = tbr.Enabled = bd.Enabled = md.Enabled = ed.Enabled = itmdel.Visible = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);

					rvfw.Enabled = false;
					rvfr.Enabled = false;
					rvfbd.Enabled = false;
					//rvfrd.Enabled = false;
					rvfmd.Enabled = false;
					rvfed.Enabled = false;

					switch (rddlw.SelectedValue)
					{
						case "":
							tbr.Visible = false;
							bd.Visible = false;
							//rd.Visible = false;
							md.Visible = false;
							ed.Visible = false;
							rvfr.Enabled = false;
							rvfbd.Enabled = false;
							//rvfrd.Enabled = false;
							rvfmd.Enabled = false;
							rvfed.Enabled = false;
							break;
						case "01":      // Return Restricted Duty
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = true;
							ed.Visible = false;
							//rvfr.Enabled = true;
							rvfr.Enabled = false;
							//rvfbd.Enabled = true;
							rvfbd.Enabled = false;
							//rvfrd.Enabled = false;
							//rvfmd.Enabled = true;
							rvfmd.Enabled = false;
							rvfed.Enabled = false;
							break;
						case "02":      // Return to Work
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = false;
							ed.Visible = false;
							rvfr.Enabled = false;
							rvfbd.Enabled = false;
							//rvfrd.Enabled = true;
							//rvfrd.Enabled = false;
							rvfmd.Enabled = false;
							rvfed.Enabled = false;
							break;
						case "03":      // Additional Lost Time
							tbr.Visible = true;
							bd.Visible = true;
							//rd.Visible = false;
							md.Visible = true;
							ed.Visible = true;
							//rvfr.Enabled = true;
							rvfr.Enabled = false;
							rvfbd.Enabled = false;
							//rvfrd.Enabled = false;
							//rvfmd.Enabled = true;
							rvfmd.Enabled = false;
							//rvfed.Enabled = true;
							rvfed.Enabled = false;
							break;
					}
				}
				catch { }
			}


			if (e.Item.ItemType == ListItemType.Footer)
			{
				Button addanother = (Button)e.Item.FindControl("btnAddLostTime");
				addanother.Visible = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);
			}
		}

		public int AddUpdateINCFORM_LOSTTIME_HIST(decimal incidentId)
		{
			var itemList = new List<INCFORM_LOSTTIME_HIST>();
			int status = 0;

			foreach (RepeaterItem losttimeitem in rptLostTime.Items)
			{
				var item = new INCFORM_LOSTTIME_HIST();

				Label lb = (Label)losttimeitem.FindControl("lbItemSeq");
				RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
				TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
				RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
				//RadDatePicker rd = (RadDatePicker)losttimeitem.FindControl("rdpReturnDate");
				RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
				RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

				item.WORK_STATUS = rddlw.SelectedValue;
				item.ITEM_DESCRIPTION = tbr.Text;
				item.BEGIN_DT = bd.SelectedDate;
				//item.RETURN_TOWORK_DT = rd.SelectedDate;
				item.NEXT_MEDAPPT_DT = md.SelectedDate;
				item.RETURN_EXPECTED_DT = ed.SelectedDate;

				itemList.Add(item);
			}

			if (itemList.Count > 0)
				status = SaveLostTime(incidentId, itemList);

			return status;

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

				if (!String.IsNullOrEmpty(item.WORK_STATUS) && item.WORK_STATUS != "[Select One]")
				{
					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.WORK_STATUS = item.WORK_STATUS;
					newItem.BEGIN_DT = item.BEGIN_DT;
					newItem.RETURN_TOWORK_DT = item.RETURN_TOWORK_DT;
					newItem.NEXT_MEDAPPT_DT = item.NEXT_MEDAPPT_DT;
					newItem.RETURN_EXPECTED_DT = item.RETURN_EXPECTED_DT;

					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_LOSTTIME_HIST(newItem);
					status = entities.SaveChanges();
				}
			}

			return status;
		}


		protected void rptLostTime_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
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
					//RadDatePicker rd = (RadDatePicker)losttimeitem.FindControl("rdpReturnDate");
					RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
					RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

					rddlw.Items.Add(new DropDownListItem("[Select One]", ""));
					List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
					foreach (var s in statuses)
					{
						rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
					}

					if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != "[Select One]"))
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

			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;
				var itemList = new List<INCFORM_LOSTTIME_HIST>();

				foreach (RepeaterItem losttimeitem in rptLostTime.Items)
				{
					var item = new INCFORM_LOSTTIME_HIST();

					Label lb = (Label)losttimeitem.FindControl("lbItemSeq");

					RadDropDownList rddlw = (RadDropDownList)losttimeitem.FindControl("rddlWorkStatus");
					//rddlw.SelectedIndexChanged += rddlw_SelectedIndexChanged;

					TextBox tbr = (TextBox)losttimeitem.FindControl("tbRestrictDesc");
					RadDatePicker bd = (RadDatePicker)losttimeitem.FindControl("rdpBeginDate");
					//RadDatePicker rd = (RadDatePicker)losttimeitem.FindControl("rdpReturnDate");
					RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
					RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

					rddlw.Items.Add(new DropDownListItem("[Select One]", ""));
					List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
					foreach (var s in statuses)
					{
						rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
					}

					if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != "[Select One]"))
						item.WORK_STATUS = rddlw.SelectedValue;

					if (Convert.ToInt32(lb.Text) != delId + 1)
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

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
				SaveLostTime(incidentId, itemList);

			}
		}

		protected void rddlw_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			//RadDropDownList rddl = (RadDropDownList)sender;
		
			//string selectedvalue = rddl.SelectedValue;
			
			//// Cast the parent to type RepeaterItem
			//RepeaterItem repeaterRow = (RepeaterItem)rddl.Parent;
			//int rptChangeIndex = repeaterRow.ItemIndex;

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
				//RadDatePicker rd = (RadDatePicker)losttimeitem.FindControl("rdpReturnDate");
				RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
				RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

				rddlw.Items.Add(new DropDownListItem("[Select One]", ""));
				List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
				foreach (var s in statuses)
				{
					rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
				}

				if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != "[Select One]"))
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

		private void ReBindRepeater()
		{
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
				//RadDatePicker rd = (RadDatePicker)losttimeitem.FindControl("rdpReturnDate");
				RadDatePicker md = (RadDatePicker)losttimeitem.FindControl("rdpNextMedDate");
				RadDatePicker ed = (RadDatePicker)losttimeitem.FindControl("rdpExpectedReturnDT");

				rddlw.Items.Add(new DropDownListItem("[Select One]", ""));
				List<EHSMetaData> statuses = EHSMetaDataMgr.SelectMetaDataList("WORK_STATUS");
				foreach (var s in statuses)
				{
					rddlw.Items.Add(new DropDownListItem(s.Text, s.Value));
				}

				if (!string.IsNullOrEmpty(rddlw.SelectedValue) && (rddlw.SelectedValue != "[Select One]"))
					item.WORK_STATUS = rddlw.SelectedValue;

				seqnumber = Convert.ToInt32(lb.Text);

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