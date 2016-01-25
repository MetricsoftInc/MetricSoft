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

		public INCIDENT ActionIncident
		{
			get { return ViewState["ActionINCIDENT"] == null ? null : (INCIDENT)ViewState["ActionINCIDENT"]; }
			set { ViewState["ActionINCIDENT"] = value; }
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
					rptAction.DataBind();
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
							(this.Page.FindControl(targetID).ID == "btnAddFinal"))
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
					ActionIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).Single();
					PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)ActionIncident.DETECT_PLANT_ID, "");
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

			LocalIncident = EHSIncidentMgr.SelectIncidentById(new PSsqmEntities(), IncidentId);
			if (LocalIncident == null)
			{
				return;
			}

			pnlAction.Visible = true;
			//rptAction.DataSource = EHSIncidentMgr.GetFinalActionList(IncidentId);
			rptAction.DataSource = EHSIncidentMgr.GetCorrectiveActionList(IncidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
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
					//INCFORM_ACTION action = (INCFORM_ACTION)e.Item.DataItem;
					TASK_STATUS action = (TASK_STATUS)e.Item.DataItem;

					TextBox tbca = (TextBox)e.Item.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)e.Item.FindControl("rddlActionPerson");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)e.Item.FindControl("rdpFinalStartDate");
					sd = SQMBasePage.SetRadDateCulture(sd, "");
					RadDatePicker cd = (RadDatePicker)e.Item.FindControl("rdpFinalCompleteDate");
					cd = SQMBasePage.SetRadDateCulture(cd, "");
					CheckBox ic = (CheckBox)e.Item.FindControl("cbFinalIsComplete");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					RequiredFieldValidator rvfca = (RequiredFieldValidator)e.Item.FindControl("rfvFinalAction");
					RequiredFieldValidator rvfcp = (RequiredFieldValidator)e.Item.FindControl("rfvActionPerson");
					RequiredFieldValidator rvfsd = (RequiredFieldValidator)e.Item.FindControl("rvfFinalStartDate");

					rvfca.ValidationGroup = ValidationGroup;
					rvfcp.ValidationGroup = ValidationGroup;
					rvfsd.ValidationGroup = ValidationGroup;
					
					rddlp.Items.Add(new DropDownListItem("", ""));
					List<PERSON> personList = personList = EHSIncidentMgr.SelectIncidentPersonList(LocalIncident, true);
					foreach (PERSON p in personList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						rddlp.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					lb.Text = action.TASK_SEQ.ToString();
					tbca.Text = action.DESCRIPTION;

					if (action.RESPONSIBLE_ID != null)
						rddlp.SelectedValue = action.RESPONSIBLE_ID.ToString();
					if (action.DUE_DT.HasValue)
						sd.SelectedDate = action.DUE_DT;

					HiddenField hf = (HiddenField)e.Item.FindControl("hfTaskStatus");
					hf.Value = action.STATUS;
					hf = (HiddenField)e.Item.FindControl("hfTaskID");
					hf.Value = action.TASK_ID.ToString();
					hf = (HiddenField)e.Item.FindControl("hfRecordType");
					hf.Value = action.RECORD_TYPE.ToString();
					hf = (HiddenField)e.Item.FindControl("hfRecordID");
					hf.Value = action.RECORD_ID.ToString();
					hf = (HiddenField)e.Item.FindControl("hfTaskStep");
					hf.Value = action.TASK_STEP;
					hf = (HiddenField)e.Item.FindControl("hfTaskType");
					hf.Value = action.TASK_TYPE;
					hf = (HiddenField)e.Item.FindControl("hfDetail");
					hf.Value = action.DETAIL;
					hf = (HiddenField)e.Item.FindControl("hfComments");
					hf.Value = action.COMMENTS;

					if (action.COMPLETE_DT.HasValue)
					{
						cd.SelectedDate = action.COMPLETE_DT;
						hf = (HiddenField)e.Item.FindControl("hfCompleteID");
						if (action.COMPLETE_ID.HasValue)
							hf.Value = action.COMPLETE_ID.ToString();
						ic.Checked = true;
					}

					if (action.CREATE_DT.HasValue)
					{
						hf = (HiddenField)e.Item.FindControl("hfCreateDT");
						DateTime dt = Convert.ToDateTime(action.CREATE_DT);
						hf.Value = dt.ToString("M/d/yyyy");
					}

					if (action.TASK_SEQ > minRowsToValidate)
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
				//addanother.Visible = ActionAccess;
			}
		}

		protected List<TASK_STATUS> GetActionListFromGrid()
		{
			List<TASK_STATUS> actionList = new List<TASK_STATUS>();
			int seqnumber = 0;

			foreach (RepeaterItem containtem in rptAction.Items)
			{
				TASK_STATUS action = new TASK_STATUS();

				TextBox tbca = (TextBox)containtem.FindControl("tbFinalAction");
				RadDropDownList rddlp = (RadDropDownList)containtem.FindControl("rddlActionPerson");
				Label lb = (Label)containtem.FindControl("lbItemSeq");
				RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpFinalStartDate");
				RadDatePicker cd = (RadDatePicker)containtem.FindControl("rdpFinalCompleteDate");
				CheckBox ic = (CheckBox)containtem.FindControl("cbFinalIsComplete");

				action.DESCRIPTION = tbca.Text;
				action.RESPONSIBLE_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0 : Convert.ToInt32(rddlp.SelectedValue);
				action.TASK_SEQ = Convert.ToInt32(lb.Text);
				action.DUE_DT = sd.SelectedDate;
				action.COMPLETE_DT = cd.SelectedDate;

				HiddenField hf = (HiddenField)containtem.FindControl("hfTaskStatus");
				action.STATUS = hf.Value;
				hf = (HiddenField)containtem.FindControl("hfTaskID");
				action.TASK_ID = Convert.ToDecimal(hf.Value);
				hf = (HiddenField)containtem.FindControl("hfCreateDT");
				if (!string.IsNullOrEmpty(hf.Value))
				{
					action.CREATE_DT = DateTime.ParseExact(hf.Value, "M/d/yyyy", null);
				}
				hf = (HiddenField)containtem.FindControl("hfRecordID");
				action.RECORD_ID = Convert.ToDecimal(hf.Value);
				hf = (HiddenField)containtem.FindControl("hfRecordType");
				action.RECORD_TYPE = Convert.ToInt32(hf.Value);
				hf = (HiddenField)containtem.FindControl("hfTaskType");
				action.TASK_TYPE = hf.Value;
				hf = (HiddenField)containtem.FindControl("hfTaskStep");
				action.TASK_STEP = hf.Value;
				hf = (HiddenField)containtem.FindControl("hfDetail");
				action.DETAIL = hf.Value;
				hf = (HiddenField)containtem.FindControl("hfComments");
				action.COMMENTS = hf.Value;
				if (action.COMPLETE_DT.HasValue)
				{
					hf = (HiddenField)containtem.FindControl("hfCompleteID");
					if (!string.IsNullOrEmpty(hf.Value))
						action.COMPLETE_ID = Convert.ToDecimal(hf.Value);
				}

				actionList.Add(action);
			}


			return actionList;
		}

		public int AddUpdateINCFORM_ACTION(decimal incidentId)
		{
			List<TASK_STATUS> actionList = GetActionListFromGrid();

			return SaveActions(incidentId, actionList);
		}

		private int SaveActions(decimal incidentId, List<TASK_STATUS> actionList)
		{

			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;

			foreach (TASK_STATUS action in actionList)
			{
				if (!string.IsNullOrEmpty(action.DESCRIPTION)  &&  action.DUE_DT.HasValue &&  action.RESPONSIBLE_ID.HasValue)
				{
					EHSIncidentMgr.CreateOrUpdateTask(ActionIncident, action, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
				}
			}

			if (status > -1)
			{
				EHSNotificationMgr.NotifyIncidentStatus(ActionIncident, ((int)SysPriv.update).ToString(), "Corrective action specified");
			}

			EHSIncidentMgr.UpdateIncidentStatus(incidentId, IncidentStepStatus.correctiveaction, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));

			return status;
		}


		protected void rptAction_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			List<TASK_STATUS> actionList = GetActionListFromGrid();

			if (e.CommandArgument == "AddAnother")
			{
				int newSeq = actionList.Max(l => l.TASK_SEQ).Value + 1;
				actionList.Add(EHSIncidentMgr.CreateEmptyTask(ActionIncident.INCIDENT_ID, ((int)SysPriv.action).ToString(), newSeq, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ)));
				rptAction.DataSource = actionList;
				rptAction.DataBind();
			}
			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;

				TASK_STATUS action = actionList.ElementAt(delId);
				if (action != null)
				{
					if (action.TASK_ID > 0)  // only delete existing actions
					{
						using (PSsqmEntities entities = new PSsqmEntities())
						{
							entities.ExecuteStoreCommand("DELETE FROM TASK_STATUS WHERE TASK_ID = " + action.TASK_ID.ToString());
						}
					}
					actionList.Remove(action);
					if (actionList.Count == 0)
					{
						actionList.Add(EHSIncidentMgr.CreateEmptyTask(ActionIncident.INCIDENT_ID, ((int)SysPriv.action).ToString(), 1, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ)));
					}
				}

				rptAction.DataSource = actionList;
				rptAction.DataBind();

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
			}

		}
	}
}