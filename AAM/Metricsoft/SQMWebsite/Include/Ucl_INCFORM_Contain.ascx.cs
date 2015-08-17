using System;
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
	public partial class Ucl_INCFORM_Contain : System.Web.UI.UserControl
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

		public string ValidationGroup
		{
			get { return ViewState["ValidationGroup"] == null ? " " : (string)ViewState["ValidationGroup"]; }
			set { ViewState["ValidationGroup"] = value; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			if (IsFullPagePostback)
				rptContain.DataBind();
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			accessLevel = UserContext.CheckAccess("EHS", "");

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
						if ((this.Page.FindControl(targetID).ID == "btnSave") || 
							(this.Page.FindControl(targetID).ID == "btnNext") || 
							(this.Page.FindControl(targetID).ID == "btnAddContain"))
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

			//if (!IsFullPagePostback)
				//PopulateInitialForm();
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

			SetUserAccess("INCFORM_CONTAIN");

			pnlContain.Visible = true;
			rptContain.DataSource = EHSIncidentMgr.GetContainmentList(IncidentId);
			rptContain.DataBind();
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

		protected void rddlContainPerson_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}

		public void rptContain_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			bool actionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_CONTAIN contain = (INCFORM_CONTAIN)e.Item.DataItem;

					TextBox tbca = (TextBox)e.Item.FindControl("tbContainAction");
					RadDropDownList rddlp = (RadDropDownList)e.Item.FindControl("rddlContainPerson");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)e.Item.FindControl("rdpStartDate");
					//RadDatePicker cd = (RadDatePicker)e.Item.FindControl("rdpCompleteDate");
					//CheckBox ic = (CheckBox)e.Item.FindControl("cbIsComplete");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					RequiredFieldValidator rvfca = (RequiredFieldValidator)e.Item.FindControl("rfvContainAction");
					RequiredFieldValidator rvfcp = (RequiredFieldValidator)e.Item.FindControl("rfvContainPerson");
					RequiredFieldValidator rvfsd = (RequiredFieldValidator)e.Item.FindControl("rvfStartDate");

					rvfca.ValidationGroup = ValidationGroup;
					rvfcp.ValidationGroup = ValidationGroup;
					rvfsd.ValidationGroup = ValidationGroup;

					rddlp.Items.Add(new DropDownListItem("[Select One]", ""));

					var personList = new List<PERSON>();
					personList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);
					foreach (PERSON p in personList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						rddlp.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					if (contain.ASSIGNED_PERSON_ID != null)
						rddlp.SelectedValue = contain.ASSIGNED_PERSON_ID.ToString();
					lb.Text = contain.ITEM_SEQ.ToString();
					tbca.Text = contain.ITEM_DESCRIPTION;
					sd.SelectedDate = contain.START_DATE;
					//cd.SelectedDate = contain.COMPLETION_DATE;
					//ic.Checked = contain.IsCompleted;

					// Set user access:
					tbca.Enabled = actionAccess;
					rddlp.Enabled = actionAccess;
					sd.Enabled = actionAccess;
					//cd.Enabled = actionAccess;
					//ic.Enabled = actionAccess;
					itmdel.Visible = actionAccess;

					rvfca.Enabled = actionAccess;
					rvfcp.Enabled = actionAccess;
					rvfsd.Enabled = actionAccess;

					if (contain.ITEM_SEQ > minRowsToValidate)
					{
						rvfca.Enabled = false;
						rvfcp.Enabled = false;
						rvfsd.Enabled = false;
					}

				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				Button addanother = (Button)e.Item.FindControl("btnAddContain");
				addanother.Visible = actionAccess;
			}

		}

		public int AddUpdateINCFORM_CONTAIN(decimal incidentId)
		{
			var itemList = new List<INCFORM_CONTAIN>();
			int seqnumber = 0;
			int status = 0;

			foreach (RepeaterItem containtem in rptContain.Items)
			{
				var item = new INCFORM_CONTAIN();

				TextBox tbca = (TextBox)containtem.FindControl("tbContainAction");
				RadDropDownList rddlp = (RadDropDownList)containtem.FindControl("rddlContainPerson");
				Label lb = (Label)containtem.FindControl("lbItemSeq");
				RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpStartDate");
				//RadDatePicker cd = (RadDatePicker)containtem.FindControl("rdpCompleteDate");
				//CheckBox ic = (CheckBox)containtem.FindControl("cbIsComplete");

				seqnumber = seqnumber + 1;

				item.ITEM_DESCRIPTION = tbca.Text;
				item.ASSIGNED_PERSON_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0 : Convert.ToInt32(rddlp.SelectedValue);
				item.ITEM_SEQ = seqnumber;
				item.START_DATE = sd.SelectedDate;
				//item.COMPLETION_DATE = cd.SelectedDate;
				//item.IsCompleted = ic.Checked;

				itemList.Add(item);

			}

			if (itemList.Count > 0)
				status = SaveContainment(incidentId, itemList);

			return status;
		}

		private int SaveContainment(decimal incidentId, List<INCFORM_CONTAIN> itemList)
		{
			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CONTAIN WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_CONTAIN item in itemList)
			{
				var newItem = new INCFORM_CONTAIN();


				if (!string.IsNullOrEmpty(item.ITEM_DESCRIPTION))
				{
					seq = seq + 1;

					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_SEQ = seq;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.ASSIGNED_PERSON_ID = item.ASSIGNED_PERSON_ID;
					newItem.START_DATE = item.START_DATE;
					//newItem.COMPLETION_DATE = item.COMPLETION_DATE;
					//newItem.IsCompleted = item.IsCompleted;
					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_CONTAIN(newItem);
					status = entities.SaveChanges();
				}
			}
			return status;
		}


		protected void rptContain_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{

				var itemList = new List<INCFORM_CONTAIN>();
				int seqnumber = 0;

				foreach (RepeaterItem containitem in rptContain.Items)
				{
					var item = new INCFORM_CONTAIN();

					TextBox tbca = (TextBox)containitem.FindControl("tbContainAction");
					RadDropDownList rddlp = (RadDropDownList)containitem.FindControl("rddlContainPerson");
					Label lb = (Label)containitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)containitem.FindControl("rdpStartDate");
					//RadDatePicker cd = (RadDatePicker)containitem.FindControl("rdpCompleteDate");
					//CheckBox ic = (CheckBox)containitem.FindControl("cbIsComplete");

					rddlp.Items.Add(new DropDownListItem("[Select One]", ""));
					var personList = new List<PERSON>();

					personList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);
					foreach (PERSON p in personList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						rddlp.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					if (!string.IsNullOrEmpty(rddlp.SelectedValue) && (rddlp.SelectedValue != "[Select One]"))
						item.ASSIGNED_PERSON_ID = Convert.ToInt32(rddlp.SelectedValue);

					seqnumber = Convert.ToInt32(lb.Text);

					item.ITEM_DESCRIPTION = tbca.Text;
					item.ITEM_SEQ = seqnumber;
					item.START_DATE = sd.SelectedDate;
					//item.COMPLETION_DATE = cd.SelectedDate;
					//item.IsCompleted = ic.Checked;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_CONTAIN();

				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.ITEM_SEQ = seqnumber + 1;
				emptyItem.ASSIGNED_PERSON_ID = null;
				emptyItem.START_DATE = null;
				//emptyItem.COMPLETION_DATE = null;
				//emptyItem.IsCompleted = false;

				itemList.Add(emptyItem);

				rptContain.DataSource = itemList;
				rptContain.DataBind();

			}
			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex; 
				var itemList = new List<INCFORM_CONTAIN>();
				int seqnumber = 0;

				foreach (RepeaterItem containitem in rptContain.Items)
				{
					var item = new INCFORM_CONTAIN();

					TextBox tbca = (TextBox)containitem.FindControl("tbContainAction");
					RadDropDownList rddlp = (RadDropDownList)containitem.FindControl("rddlContainPerson");
					Label lb = (Label)containitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)containitem.FindControl("rdpStartDate");
					//RadDatePicker cd = (RadDatePicker)containitem.FindControl("rdpCompleteDate");
					//CheckBox ic = (CheckBox)containitem.FindControl("cbIsComplete");

					rddlp.Items.Add(new DropDownListItem("[Select One]", ""));

					var personList = new List<PERSON>();
					personList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);
					foreach (PERSON p in personList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						rddlp.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					if (!string.IsNullOrEmpty(rddlp.SelectedValue) && (rddlp.SelectedValue != "[Select One]"))
						item.ASSIGNED_PERSON_ID = Convert.ToInt32(rddlp.SelectedValue);

					if (Convert.ToInt32(lb.Text) != delId + 1)
					{
						seqnumber = seqnumber + 1;
						item.ITEM_DESCRIPTION = tbca.Text;
						item.ITEM_SEQ = seqnumber;
						item.START_DATE = sd.SelectedDate;
						//item.COMPLETION_DATE = cd.SelectedDate;
						//item.IsCompleted = ic.Checked;
						itemList.Add(item);
					}
				}

				rptContain.DataSource = itemList;
				rptContain.DataBind();

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
				int status = SaveContainment(incidentId, itemList);
			}
		}

	}
}
