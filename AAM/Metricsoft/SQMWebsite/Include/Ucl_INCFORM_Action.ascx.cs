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
	public partial class Ucl_INCFORM_Action : System.Web.UI.UserControl
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

		protected bool UpdateAccess
		{
			get { return ViewState["UpdateAccess"] == null ? false : (bool)ViewState["UpdateAccess"]; }
			set { ViewState["UpdateAccess"] = value; }
		}

		protected bool ActionAccess
		{
			get { return ViewState["ActionAccess"] == null ? false : (bool)ViewState["ActionAccess"]; }
			set { ViewState["ActionAccess"] = value; }
		}

		protected bool ApproveAccess
		{
			get { return ViewState["ApproveAccess"] == null ? false : (bool)ViewState["ApproveAccess"]; }
			set { ViewState["ApproveAccess"] = value; }
		}


		public string ValidationGroup
		{
			get { return ViewState["ValidationGroup"] == null ? " " : (string)ViewState["ValidationGroup"]; }
			set { ViewState["ValidationGroup"] = value; }
		}


		protected void Page_Init(object sender, EventArgs e)
		{
			UpdateAccess = SessionManager.CheckUserPrivilege(SysPriv.update, SysScope.incident);
			ActionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
			ApproveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);

			if (IsFullPagePostback)
				rptAction.DataBind();
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
							(this.Page.FindControl(targetID).ID == "btnAddFinal"))
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
			//	PopulateInitialForm();

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

			pnlAction.Visible = true;
			rptAction.DataSource = EHSIncidentMgr.GetFinalActionList(IncidentId);
			rptAction.DataBind();
		}

		protected void rddlActionPerson_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}


		public void rptAction_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				int minRowsToValidate = 1;

				try
				{
					INCFORM_ACTION action = (INCFORM_ACTION)e.Item.DataItem;

					TextBox tbca = (TextBox)e.Item.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)e.Item.FindControl("rddlActionPerson");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)e.Item.FindControl("rdpFinalStartDate");
					RadDatePicker cd = (RadDatePicker)e.Item.FindControl("rdpFinalCompleteDate");
					CheckBox ic = (CheckBox)e.Item.FindControl("cbFinalIsComplete");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					RequiredFieldValidator rvfca = (RequiredFieldValidator)e.Item.FindControl("rfvFinalAction");
					RequiredFieldValidator rvfcp = (RequiredFieldValidator)e.Item.FindControl("rfvActionPerson");
					RequiredFieldValidator rvfsd = (RequiredFieldValidator)e.Item.FindControl("rvfFinalStartDate");

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

					if (action.ASSIGNED_PERSON_ID != null)
						rddlp.SelectedValue = action.ASSIGNED_PERSON_ID.ToString();

					lb.Text = action.ITEM_SEQ.ToString();
					tbca.Text = action.ITEM_DESCRIPTION;
					sd.SelectedDate = action.START_DATE;
					cd.SelectedDate = action.COMPLETION_DATE;
					ic.Checked = action.IsCompleted;

					// Set user access:
					tbca.Enabled = ActionAccess;
					rddlp.Enabled = ActionAccess;
					sd.Enabled = ActionAccess;
					cd.Enabled = ActionAccess;
					ic.Enabled = ActionAccess;
					itmdel.Visible = ActionAccess;
		
					rvfca.Enabled = ActionAccess;
					rvfcp.Enabled = ActionAccess;
					rvfsd.Enabled = ActionAccess;

					if (action.ITEM_SEQ > minRowsToValidate)
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
				Button addanother = (Button)e.Item.FindControl("btnAddFinal");
				addanother.Visible = ActionAccess;
			}
		}

		public int AddUpdateINCFORM_ACTION(decimal incidentId)
		{
			var itemList = new List<INCFORM_ACTION>();
			int seqnumber = 0;
			int status = 0;

			foreach (RepeaterItem containtem in rptAction.Items)
			{
				var item = new INCFORM_ACTION();

				TextBox tbca = (TextBox)containtem.FindControl("tbFinalAction");
				RadDropDownList rddlp = (RadDropDownList)containtem.FindControl("rddlActionPerson");
				Label lb = (Label)containtem.FindControl("lbItemSeq");
				RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpFinalStartDate");
				RadDatePicker cd = (RadDatePicker)containtem.FindControl("rdpFinalCompleteDate");
				CheckBox ic = (CheckBox)containtem.FindControl("cbFinalIsComplete");

				seqnumber = seqnumber + 1;

				item.ITEM_DESCRIPTION = tbca.Text;
				item.ASSIGNED_PERSON_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0 : Convert.ToInt32(rddlp.SelectedValue);
				item.ITEM_SEQ = seqnumber;
				item.START_DATE = sd.SelectedDate;
				item.COMPLETION_DATE = cd.SelectedDate;
				item.IsCompleted = ic.Checked;

				itemList.Add(item);
			}

			if (itemList.Count > 0)
				status  = SaveActions(incidentId, itemList);

			return status;

		}

		private int SaveActions(decimal incidentId, List<INCFORM_ACTION> itemList)
		{

			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;
	
			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ACTION WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_ACTION item in itemList)
			{
				var newItem = new INCFORM_ACTION();

				if (!string.IsNullOrEmpty(item.ITEM_DESCRIPTION))
				{
					seq = seq + 1;

					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_SEQ = seq;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.ASSIGNED_PERSON_ID = item.ASSIGNED_PERSON_ID;
					newItem.START_DATE = item.START_DATE;
					newItem.COMPLETION_DATE = item.COMPLETION_DATE;
					newItem.IsCompleted = item.IsCompleted;
					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_ACTION(newItem);
					status = entities.SaveChanges();
				}
			}
			return status;
		}


		protected void rptAction_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{

				var itemList = new List<INCFORM_ACTION>();
				int seqnumber = 0;

				foreach (RepeaterItem actionitem in rptAction.Items)
				{
					var item = new INCFORM_ACTION();

					TextBox tbca = (TextBox)actionitem.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)actionitem.FindControl("rddlActionPerson");
					Label lb = (Label)actionitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)actionitem.FindControl("rdpFinalStartDate");
					RadDatePicker cd = (RadDatePicker)actionitem.FindControl("rdpFinalCompleteDate");
					CheckBox ic = (CheckBox)actionitem.FindControl("cbFinalIsComplete");

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
					item.COMPLETION_DATE = cd.SelectedDate;
					item.IsCompleted = ic.Checked;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_ACTION();

				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.ITEM_SEQ = seqnumber + 1;
				emptyItem.ASSIGNED_PERSON_ID = null;
				emptyItem.START_DATE = null;
				emptyItem.COMPLETION_DATE = null;
				emptyItem.IsCompleted = false;


				itemList.Add(emptyItem);

				rptAction.DataSource = itemList;
				rptAction.DataBind();
			}
			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;
				var itemList = new List<INCFORM_ACTION>();
				int seqnumber = 0;

				foreach (RepeaterItem actionitem in rptAction.Items)
				{
					var item = new INCFORM_ACTION();

					TextBox tbca = (TextBox)actionitem.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)actionitem.FindControl("rddlActionPerson");
					Label lb = (Label)actionitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)actionitem.FindControl("rdpFinalStartDate");
					RadDatePicker cd = (RadDatePicker)actionitem.FindControl("rdpFinalCompleteDate");
					CheckBox ic = (CheckBox)actionitem.FindControl("cbFinalIsComplete");

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
						item.COMPLETION_DATE = cd.SelectedDate;
						item.IsCompleted = ic.Checked;

						itemList.Add(item);
					}
				}

				rptAction.DataSource = itemList;
				rptAction.DataBind();

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
				int status = SaveActions(incidentId, itemList);
			
			}

		}
	}
}