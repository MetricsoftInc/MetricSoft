﻿using System;
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
	public partial class Ucl_INCFORM_Root5Y : System.Web.UI.UserControl
	{

		const Int32 MaxTextLength = 4000;

		protected decimal companyId;
		protected AccessMode accessLevel;

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


		protected void Page_Init(object sender, EventArgs e)
		{
			if (IsFullPagePostback)
				rptRootCause.DataBind();
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			accessLevel = UserContext.CheckAccess("EHS", "");

			if (IsPostBack)
			{
				// Since IsPostBack is always TRUE for every invocation of this control we need some way 
				// to determine whether or not to refresh the page controls or just data bind instead.  
				// Here we are using the "__EVENTTARGET" form event property to see if this control is posting 
				// back because of parent calls from either of the two SAVE buttons.  If so then that IS a real postback
				// and we need to retain any data that may have been enetered by the user.  If not then we initialize
				// all the page controls here.

				IsFullPagePostback = false;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);
					if ((this.Page.FindControl(targetID).ID == "btnSave") || (this.Page.FindControl(targetID).ID == "btnNext"))
						IsFullPagePostback = true;
				}
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
			String selectedLanguage = SessionManager.SessionContext.Language().NLS_LANGUAGE;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

			base.FrameworkInitialize();
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

			SetUserAccess("INCFORM_ROOT5Y");

			pnlRoot5Y.Visible = true;
			rptRootCause.DataSource = EHSIncidentMgr.GetRootCauseList(IncidentId);
			rptRootCause.DataBind();
		}

		private void SetUserAccess(string currentFormName)
		{

			// Privilege "update"	= Main incident description (1st page) can be maintained/upadted to db
			// Privilege "action"	= Initial Actions page, 5-Why's page, and Final Actions page can be maintained/upadted to db
			// Privilege "approve"	= Approval page can be maintained/upadted to db.  "Close Incident" button is enabled.

			bool updateAccess = SessionManager.CheckUserPrivilege(SysPriv.update, SysScope.incident);
			bool actionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
			bool approveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);

		}

		public void rptRootCause_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			bool actionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_ROOT5Y rootCause = (INCFORM_ROOT5Y)e.Item.DataItem;

					TextBox tb = (TextBox)e.Item.FindControl("tbRootCause");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RequiredFieldValidator rvf = (RequiredFieldValidator)e.Item.FindControl("rfvRootCause");

					lb.Text = rootCause.ITEM_SEQ.ToString();
					tb.Text = rootCause.ITEM_DESCRIPTION;

					// Set user access:
					tb.Enabled = actionAccess;
					rvf.Enabled = actionAccess;

					if (rootCause.ITEM_SEQ > minRowsToValidate)
						rvf.Enabled = false;
				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				Button addanother = (Button)e.Item.FindControl("btnAddRootCause");
				addanother.Visible = actionAccess;
			}

		}

		public void AddUpdateINCFORM_ROOT5Y(decimal incidentId)
		{

			var itemList = new List<INCFORM_ROOT5Y>();
			int seqnumber = 0;

			foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
			{
				var item = new INCFORM_ROOT5Y();

				TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
				Label lb = (Label)rootcauseitem.FindControl("lbItemSeq");

				if (!String.IsNullOrEmpty(tb.Text))
				{
					seqnumber = seqnumber + 1;

					item.ITEM_DESCRIPTION = tb.Text;
					item.ITEM_SEQ = seqnumber;

					itemList.Add(item);
				}
			}

			SaveRootCauses(incidentId, itemList);
		}


		protected void SaveRootCauses(decimal incidentId, List<INCFORM_ROOT5Y> itemList)
		{

			PSsqmEntities entities = new PSsqmEntities();

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_ROOT5Y item in itemList)
			{
				var newItem = new INCFORM_ROOT5Y();

				if (!string.IsNullOrEmpty(item.ITEM_DESCRIPTION))
				{
					seq = seq + 1;

					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_SEQ = seq;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_ROOT5Y(newItem);
					entities.SaveChanges();
				}
			}
		}


		protected void rptRootCause_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{

				var itemList = new List<INCFORM_ROOT5Y>();
				int seqnumber = 0;

				foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
				{
					var item = new INCFORM_ROOT5Y();

					TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
					Label lb = (Label)rootcauseitem.FindControl("lbItemSeq");

					seqnumber = Convert.ToInt32(lb.Text);

					item.ITEM_DESCRIPTION = tb.Text;
					item.ITEM_SEQ = seqnumber;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_ROOT5Y();

				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.ITEM_SEQ = seqnumber + 1;
				itemList.Add(emptyItem);

				rptRootCause.DataSource = itemList;
				rptRootCause.DataBind();

			}
		}
	}
}