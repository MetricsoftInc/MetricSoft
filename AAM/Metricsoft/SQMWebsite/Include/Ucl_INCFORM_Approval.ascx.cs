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
	public partial class Ucl_INCFORM_Approval : System.Web.UI.UserControl
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
				rptApprovals.DataBind();
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
							(this.Page.FindControl(targetID).ID == "btnNext"))
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

			SetUserAccess("INCFORM_APPROVAL");

			pnlApproval.Visible = true;
			rptApprovals.DataSource = EHSIncidentMgr.GetApprovalList(IncidentId);
			rptApprovals.DataBind();
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

		public void rptApprovals_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			bool approveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_APPROVAL approval = (INCFORM_APPROVAL)e.Item.DataItem;

					Label lba = (Label)e.Item.FindControl("lbApprover");
					Label lbm = (Label)e.Item.FindControl("lbApproveMessage");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					Label lbjobd = (Label)e.Item.FindControl("lbApproverJob");
					CheckBox cba = (CheckBox)e.Item.FindControl("cbIsAccepted");
					RadDatePicker rda = (RadDatePicker)e.Item.FindControl("rdpAcceptDate");

					//lb.Text = approval.ITEM_SEQ.ToString();
					lb.Visible = false;
					switch (approval.ITEM_SEQ)
					{
						case 1:
							lbjobd.Text = "EHS Manager: ";
							break;
						case 2:
							lbjobd.Text = "Plant Manager: ";
							break;
						default:
							lbjobd.Text = "Approver: ";
							break;
					}
						
					lba.Text = approval.APPROVER_PERSON;
					lbm.Text = approval.APPROVAL_MESSAGE;
					cba.Checked = approval.IsAccepted;
					rda.SelectedDate = approval.APPROVAL_DATE;

					// Set user access:
					cba.Enabled = approveAccess;
					rda.Enabled = approveAccess;

					//if (approval.ITEM_SEQ > minRowsToValidate)
					//	rvf.Enabled = false;

				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				//Button addanother = (Button)e.Item.FindControl("btnAddApproval");
				//addanother.Visible = approveAccess;
			}

		}

		public void AddUpdateINCFORM_APPROVAL(decimal incidentId)
		{
			var itemList = new List<INCFORM_APPROVAL>();
			//int seqnumber = 0;

			foreach (RepeaterItem containtem in rptApprovals.Items)
			{
				var item = new INCFORM_ACTION();

			//	TextBox tbca = (TextBox)containtem.FindControl("tbFinalAction");
			//	TextBox tbcp = (TextBox)containtem.FindControl("tbFinalPerson");
			//	Label lb = (Label)containtem.FindControl("lbItemSeq");
			//	RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpFinalStartDate");
			//	RadDatePicker cd = (RadDatePicker)containtem.FindControl("rdpFinalCompleteDate");
			//	CheckBox ic = (CheckBox)containtem.FindControl("cbFinalIsComplete");

			//	seqnumber = Convert.ToInt32(lb.Text);

			//	item.ITEM_DESCRIPTION = tbca.Text;
			//	item.ASSIGNED_PERSON = tbcp.Text;
			//	item.ITEM_SEQ = seqnumber;
			//	item.START_DATE = sd.SelectedDate;
			//	item.COMPLETION_DATE = cd.SelectedDate;
			//	item.IsCompleted = ic.Checked;

			//	itemList.Add(item);
			}

			SaveApprovals(incidentId, itemList);
		}

		private void SaveApprovals(decimal incidentId, List<INCFORM_APPROVAL> itemList)
		{
			PSsqmEntities entities = new PSsqmEntities();

			//using (var ctx = new PSsqmEntities())
			//{
			//	ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVE WHERE INCIDENT_ID = {0}", incidentId);
			//}

			//int seq = 0;

			//foreach (INCFORM_APPROVAL item in itemList)
			//{
			//	var newItem = new INCFORM_APPROVAL();

			//	if (!string.IsNullOrEmpty(item.ITEM_DESCRIPTION))
			//	{
			//		seq = seq + 1;

			//		newItem.INCIDENT_ID = incidentId;
			//		newItem.ITEM_SEQ = seq;
			//		newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
			//		newItem.ASSIGNED_PERSON_ID = item.ASSIGNED_PERSON_ID;
			//		newItem.START_DATE = item.START_DATE;
			//		newItem.COMPLETION_DATE = item.COMPLETION_DATE;
			//		newItem.IsCompleted = item.IsCompleted;
			//		newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
			//		newItem.LAST_UPD_DT = DateTime.Now;

			//		entities.AddToINCFORM_APPROVAL(newItem);
			//		entities.SaveChanges();
			//	}
			//}
		}


		protected void rptApprovals_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{
			}
		}
	}
}