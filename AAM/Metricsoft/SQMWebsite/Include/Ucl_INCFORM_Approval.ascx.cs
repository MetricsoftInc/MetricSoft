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

		public INCIDENT ApprovalIncident
		{
			get { return ViewState["ApprovalINCIDENT"] == null ? null : (INCIDENT)ViewState["ApprovalINCIDENT"]; }
			set { ViewState["ApprovalINCIDENT"] = value; }
		}

		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
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
			if (SessionManager.SessionContext != null)
			{
				UpdateAccess = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);
				ActionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
				ApproveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);

				if (IsFullPagePostback)
					rptApprovals.DataBind();
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
						if ((this.Page.FindControl(targetID).ID == "btnSave") || 
							(this.Page.FindControl(targetID).ID == "btnNext"))
								IsFullPagePostback = true;
				}
			}
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
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			if (IncidentId > 0)
				try
				{
					ApprovalIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).Single();
					PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)ApprovalIncident.DETECT_PLANT_ID, "");
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
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			SetUserAccess("INCFORM_APPROVAL");

			pnlApproval.Visible = true;
			rptApprovals.DataSource = EHSIncidentMgr.GetApprovalList(IncidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
			rptApprovals.DataBind();
		}

		private void SetUserAccess(string currentFormName)
		{

		}

		public void rptApprovals_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_APPROVAL approval = (INCFORM_APPROVAL)e.Item.DataItem;

					HiddenField hf = (HiddenField)e.Item.FindControl("hfItemSeq");
					Label lba = (Label)e.Item.FindControl("lbApprover");
					Label lbm = (Label)e.Item.FindControl("lbApproveMessage");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					Label lbjobd = (Label)e.Item.FindControl("lbApproverJob");
					CheckBox cba = (CheckBox)e.Item.FindControl("cbIsAccepted");
					RadDatePicker rda = (RadDatePicker)e.Item.FindControl("rdpAcceptDate");

					hf.Value = approval.ITEM_SEQ.ToString();
					hf = (HiddenField)e.Item.FindControl("hfPersonID");
					hf.Value = approval.APPROVER_PERSON_ID.ToString();
					lb.Visible = false;
					lbjobd.Text = approval.APPROVER_TITLE;
					lba.Text = !string.IsNullOrEmpty(approval.APPROVER_PERSON) ? approval.APPROVER_PERSON : "";
					lbm.Text = approval.APPROVAL_MESSAGE;
					cba.Checked = approval.IsAccepted;
					rda.SelectedDate = approval.APPROVAL_DATE;

					// Set user access:
					if (SessionManager.CheckUserPrivilege((SysPriv)approval.ITEM_SEQ, SysScope.incident))
					{
						lba.Text = SessionManager.UserContext.UserName();
						cba.Enabled = true;
					}
					else
					{
						cba.Enabled = false;
					}

				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				//Button addanother = (Button)e.Item.FindControl("btnAddApproval");
				//addanother.Visible = ApproveAccess;
			}

		}

		public int AddUpdateINCFORM_APPROVAL(decimal incidentId)
		{
			var itemList = new List<INCFORM_APPROVAL>();
			int status = 0;
			int seq = 150;
			List<IncidentStepStatus> approvalList = new List<IncidentStepStatus>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID = " + incidentId.ToString());

				foreach (RepeaterItem item in rptApprovals.Items)
				{
					++seq;
					HiddenField hf = (HiddenField)item.FindControl("hfItemSeq");
					Label lba = (Label)item.FindControl("lbApprover");
					Label lbm = (Label)item.FindControl("lbApproveMessage");
					Label lb = (Label)item.FindControl("lbItemSeq");
					Label lbjobd = (Label)item.FindControl("lbApproverJob");
					CheckBox cba = (CheckBox)item.FindControl("cbIsAccepted");
					RadDatePicker rda = (RadDatePicker)item.FindControl("rdpAcceptDate");

					if (cba.Checked == true)
					{
						INCFORM_APPROVAL approval = new INCFORM_APPROVAL();
						approval.INCIDENT_ID = incidentId;
						approval.ITEM_SEQ = Convert.ToInt32(hf.Value);
						approval.IsAccepted = true;
						approval.APPROVAL_MESSAGE = lbm.Text;
						approval.APPROVER_TITLE = lbjobd.Text;
						approval.APPROVAL_DATE = rda.SelectedDate;
						hf = (HiddenField)item.FindControl("hfPersonID");
						if (string.IsNullOrEmpty(hf.Value) || hf.Value == "0")
						{
							approval.APPROVER_PERSON_ID = SessionManager.UserContext.Person.PERSON_ID;
							approval.APPROVER_PERSON = SessionManager.UserContext.UserName();
						}
						else
						{
							approval.APPROVER_PERSON_ID = Convert.ToDecimal(hf.Value);
							approval.APPROVER_PERSON = lba.Text;
						}

						approvalList.Add((IncidentStepStatus)seq);

						ctx.AddToINCFORM_APPROVAL(approval);
					}
				}

				status = ctx.SaveChanges();

				if (approvalList.Count > 0)
				{
					foreach (IncidentStepStatus stat in approvalList)
					{
						if (stat == IncidentStepStatus.signoff2  ||  stat == IncidentStepStatus.signoffComplete)  // set status to CLOSED if final sign-off
							EHSIncidentMgr.UpdateIncidentStatus(incidentId, stat, true, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
						else
							EHSIncidentMgr.UpdateIncidentStatus(incidentId, stat, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
					}
				}

				if (status > -1)
				{
					EHSNotificationMgr.NotifyIncidentStatus(ApprovalIncident, ((int)SysPriv.approve).ToString(), "");
				}
			}

			return status;
		}

		protected void rptApprovals_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{
			}
		}
	}
}