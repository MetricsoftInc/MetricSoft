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
	public partial class Ucl_INCFORM_Root5Y : System.Web.UI.UserControl
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
					rptRootCause.DataBind();
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
							(this.Page.FindControl(targetID).ID == "btnNext") || 
							(this.Page.FindControl(targetID).ID == "btnAddRootCause"))
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

			pnlRoot5Y.Visible = true;
			rptRootCause.DataSource = EHSIncidentMgr.GetRootCauseList(IncidentId);
			rptRootCause.DataBind();
		}

		public void rptRootCause_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_ROOT5Y rootCause = (INCFORM_ROOT5Y)e.Item.DataItem;

					TextBox tb = (TextBox)e.Item.FindControl("tbRootCause");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");
					RequiredFieldValidator rvf = (RequiredFieldValidator)e.Item.FindControl("rfvRootCause");

					rvf.ValidationGroup = ValidationGroup;

					lb.Text = rootCause.ITEM_SEQ.ToString();
					tb.Text = rootCause.ITEM_DESCRIPTION;

					// Set user access:
					tb.Enabled = ActionAccess;
					itmdel.Visible = ActionAccess;
					rvf.Enabled = ActionAccess;

					if (rootCause.ITEM_SEQ > minRowsToValidate)
						rvf.Enabled = false;
				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				Button addanother = (Button)e.Item.FindControl("btnAddRootCause");
				addanother.Visible = ActionAccess;
			}

		}

		public int AddUpdateINCFORM_ROOT5Y(decimal incidentId)
		{

			var itemList = new List<INCFORM_ROOT5Y>();
			int seqnumber = 0;
			int status = 0;

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

			status = SaveRootCauses(incidentId, itemList);
			return status;
		}


		protected int SaveRootCauses(decimal incidentId, List<INCFORM_ROOT5Y> itemList)
		{

			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;

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
					status = entities.SaveChanges();
				}
			}
			return status;
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
			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;
				var itemList = new List<INCFORM_ROOT5Y>();
				int seqnumber = 0;

				foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
				{
					var item = new INCFORM_ROOT5Y();

					TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
					Label lb = (Label)rootcauseitem.FindControl("lbItemSeq");


					if (Convert.ToInt32(lb.Text) != delId + 1)
					{
						seqnumber = seqnumber + 1;
						item.ITEM_DESCRIPTION = tb.Text;
						item.ITEM_SEQ = seqnumber;
						itemList.Add(item);
					}
				}

				rptRootCause.DataSource = itemList;
				rptRootCause.DataBind();

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
				int status = SaveRootCauses(incidentId, itemList);
	
			}
		}
	}
}