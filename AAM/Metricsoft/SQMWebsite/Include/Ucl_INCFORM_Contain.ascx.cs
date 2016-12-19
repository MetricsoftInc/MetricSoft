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

		protected int totalFormSteps;

		protected decimal incidentTypeId;
		protected string incidentType;
		protected bool IsFullPagePostback = false;

		PSsqmEntities entities;

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

		public INCIDENT LocalIncident
		{
			get { return ViewState["LocalIncident"] == null ? null : (INCIDENT)ViewState["LocalIncident"]; }
			set { ViewState["LocalIncident"] = value; }
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

		protected decimal EditIncidentTypeId
		{
			get { return IncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(IncidentId); }
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
					rptContain.DataBind();
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{

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

			}
		}

		protected override void FrameworkInitialize()
		{
			//String selectedLanguage = "es";
			if (SessionManager.SessionContext != null)
			{
				String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

				base.FrameworkInitialize();
			}
		}

		public void PopulateInitialForm()
		{
			InitializeForm();
		}


		void InitializeForm()
		{
			lblStatusMsg.Visible = false;
			pnlContain.Visible = true;

			LocalIncident = EHSIncidentMgr.SelectIncidentById(new PSsqmEntities(), IncidentId);
			if (LocalIncident == null)
			{
				return;
			}

			PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)LocalIncident.DETECT_PLANT_ID, "");
			if (plant != null)
				IncidentLocationTZ = plant.LOCAL_TIMEZONE;

			pnlContain.Visible = true;
			if (PageMode == PageUseMode.ViewOnly)
			{
				divTitle.Visible = true;
				lblFormTitle.Text = Resources.LocalizedText.InitialAction;
			}

			rptContain.DataSource = EHSIncidentMgr.GetContainmentList(IncidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ), PageMode == PageUseMode.ViewOnly ? false : true);
			rptContain.DataBind();

			pnlContain.Enabled = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.originate, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
		}

		public void rptContain_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{

				int minRowsToValidate = 1;

				try
				{
					INCFORM_CONTAIN contain = (INCFORM_CONTAIN)e.Item.DataItem;

					TextBox tbca = (TextBox)e.Item.FindControl("tbContainAction");
					RadComboBox rddlp = (RadComboBox)e.Item.FindControl("rddlContainPerson");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)e.Item.FindControl("rdpStartDate");
					sd = SQMBasePage.SetRadDateCulture(sd, "");
					//RadDatePicker cd = (RadDatePicker)e.Item.FindControl("rdpCompleteDate");
					//CheckBox ic = (CheckBox)e.Item.FindControl("cbIsComplete");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					RequiredFieldValidator rvfca = (RequiredFieldValidator)e.Item.FindControl("rfvContainAction");
					RequiredFieldValidator rvfcp = (RequiredFieldValidator)e.Item.FindControl("rfvContainPerson");
					RequiredFieldValidator rvfsd = (RequiredFieldValidator)e.Item.FindControl("rvfStartDate");

					rvfca.ValidationGroup = ValidationGroup;
					rvfcp.ValidationGroup = ValidationGroup;
					rvfsd.ValidationGroup = ValidationGroup;

					rddlp.Items.Add(new RadComboBoxItem("", ""));

					var personList = new List<PERSON>();
					personList = SQMModelMgr.SelectPlantPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, (decimal)LocalIncident.DETECT_PLANT_ID);
					foreach (PERSON p in personList)
					{
						if (!String.IsNullOrEmpty(p.EMAIL))
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddlp.Items.Add(new RadComboBoxItem(displayName, Convert.ToString(p.PERSON_ID)));
						}
					}

					if (contain.ASSIGNED_PERSON_ID != null)
						rddlp.SelectedValue = contain.ASSIGNED_PERSON_ID.ToString();
					lb.Text = contain.ITEM_SEQ.ToString();
					tbca.Text = contain.ITEM_DESCRIPTION;
					sd.SelectedDate = contain.START_DATE;
					//cd.SelectedDate = contain.COMPLETION_DATE;
					//ic.Checked = contain.IsCompleted;

					if ((tbca = (TextBox)e.Item.FindControl("tbComments")) != null)
					{
						tbca.Text = contain.COMMENTS;
					}

					// Set user access:
					tbca.Enabled = rddlp.Enabled = sd.Enabled = itmdel.Visible = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);

					rvfca.Enabled = rvfcp.Enabled = rvfsd.Enabled = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
	
					if (contain.ITEM_SEQ > minRowsToValidate)
					{
						rvfca.Enabled = false;
						rvfcp.Enabled = false;
						rvfsd.Enabled = false;
					}

					itmdel.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.originate, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
	
				}
				catch { }
			}

			btnSave.Visible = btnAddContain.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.originate, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
		}


		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (AddUpdateINCFORM_CONTAIN(LocalIncident.INCIDENT_ID) >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				InitializeForm();
			}
		}

		public int AddUpdateINCFORM_CONTAIN(decimal incidentId)
		{
			lblStatusMsg.Visible = false;
			var itemList = new List<INCFORM_CONTAIN>();
			int seqnumber = 0;
			int status = 0;
			bool allFieldsComplete = true;

			foreach (RepeaterItem containtem in rptContain.Items)
			{
				var item = new INCFORM_CONTAIN();

				TextBox tbca = (TextBox)containtem.FindControl("tbContainAction");
				RadComboBox rddlp = (RadComboBox)containtem.FindControl("rddlContainPerson");
				Label lb = (Label)containtem.FindControl("lbItemSeq");
				RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpStartDate");

				seqnumber = seqnumber + 1;

				if (string.IsNullOrEmpty(tbca.Text.Trim()) ||  !sd.SelectedDate.HasValue)
				{
					allFieldsComplete = false;
				}
				else
				{
					item.ITEM_DESCRIPTION = tbca.Text.Trim();
					item.ASSIGNED_PERSON_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0 : Convert.ToInt32(rddlp.SelectedValue);
					item.ITEM_SEQ = seqnumber;
					item.START_DATE = sd.SelectedDate;

					if ((tbca = (TextBox)containtem.FindControl("tbComments")) != null)
					{
						item.COMMENTS = tbca.Text;
					}
				}

				itemList.Add(item);

			}

			if (itemList.Count > 0)
			{
				if (allFieldsComplete)
				{
					status = SaveContainment(incidentId, itemList);
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

		protected void AddDelete_Click(object sender, EventArgs e)
		{
			rptContain_ItemCommand(sender, null);
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
					newItem.COMMENTS = item.COMMENTS;
					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);

					entities.AddToINCFORM_CONTAIN(newItem);
					status = entities.SaveChanges();
				}
			}

			if (seq > 0)
			{
				EHSIncidentMgr.UpdateIncidentStatus(incidentId, IncidentStepStatus.containment, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
			}

			return status;
		}


		protected void rptContain_ItemCommand(object source, RepeaterCommandEventArgs e)
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

				var itemList = new List<INCFORM_CONTAIN>();
				int seqnumber = 0;

				foreach (RepeaterItem containitem in rptContain.Items)
				{
					var item = new INCFORM_CONTAIN();

					TextBox tbca = (TextBox)containitem.FindControl("tbContainAction");
					RadComboBox rddlp = (RadComboBox)containitem.FindControl("rddlContainPerson");
					Label lb = (Label)containitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)containitem.FindControl("rdpStartDate");
					sd = SQMBasePage.SetRadDateCulture(sd, "");

					rddlp.Items.Add(new RadComboBoxItem("", ""));
					var personList = new List<PERSON>();
					personList = SQMModelMgr.SelectPlantPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, (decimal)LocalIncident.DETECT_PLANT_ID);
					foreach (PERSON p in personList)
					{
						if (!String.IsNullOrEmpty(p.EMAIL))
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddlp.Items.Add(new RadComboBoxItem(displayName, Convert.ToString(p.PERSON_ID)));
						}
					}

					if (!string.IsNullOrEmpty(rddlp.SelectedValue) && (rddlp.SelectedValue != ""))
						item.ASSIGNED_PERSON_ID = Convert.ToInt32(rddlp.SelectedValue);

					seqnumber = Convert.ToInt32(lb.Text);

					item.ITEM_DESCRIPTION = tbca.Text;
					item.ITEM_SEQ = seqnumber;
					item.START_DATE = sd.SelectedDate;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_CONTAIN();

				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.ITEM_SEQ = seqnumber + 1;
				emptyItem.ASSIGNED_PERSON_ID = null;
				emptyItem.START_DATE = null;

				itemList.Add(emptyItem);

				rptContain.DataSource = itemList;
				rptContain.DataBind();

			}
			else if (cmd == "Delete")
			{
				int delId = e.Item.ItemIndex; 
				var itemList = new List<INCFORM_CONTAIN>();
				int seqnumber = 0;

				foreach (RepeaterItem containitem in rptContain.Items)
				{
					var item = new INCFORM_CONTAIN();

					TextBox tbca = (TextBox)containitem.FindControl("tbContainAction");
					RadComboBox rddlp = (RadComboBox)containitem.FindControl("rddlContainPerson");
					Label lb = (Label)containitem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)containitem.FindControl("rdpStartDate");
					sd = SQMBasePage.SetRadDateCulture(sd, "");

					rddlp.Items.Add(new RadComboBoxItem("", ""));

					var personList = new List<PERSON>();
					personList = SQMModelMgr.SelectPlantPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, (decimal)LocalIncident.DETECT_PLANT_ID);
					foreach (PERSON p in personList)
					{
						if (!String.IsNullOrEmpty(p.EMAIL))
						{
							string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
							rddlp.Items.Add(new RadComboBoxItem(displayName, Convert.ToString(p.PERSON_ID)));
						}
					}

					if (!string.IsNullOrEmpty(rddlp.SelectedValue) && (rddlp.SelectedValue != ""))
						item.ASSIGNED_PERSON_ID = Convert.ToInt32(rddlp.SelectedValue);

					if (Convert.ToInt32(lb.Text) != delId + 1)
					{
						seqnumber = seqnumber + 1;
						item.ITEM_DESCRIPTION = tbca.Text;
						item.ITEM_SEQ = seqnumber;
						item.START_DATE = sd.SelectedDate;
						itemList.Add(item);
					}
				}

				rptContain.DataSource = itemList;
				rptContain.DataBind();

				decimal incidentId = (IsEditContext) ? IncidentId : NewIncidentId;
				int status = SaveContainment(incidentId, itemList);
			}
		}

	}
}
