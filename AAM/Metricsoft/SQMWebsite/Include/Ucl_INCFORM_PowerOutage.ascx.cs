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
	public partial class Ucl_INCFORM_PowerOutage : System.Web.UI.UserControl
	{

		const Int32 MaxTextLength = 4000;

		static List<PLANT> plantList;
		static List<PERSON> personList;

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected AccessMode accessLevel;
		protected RadDropDownList rddlFilteredUsers;
		protected bool IsFullPagePostback = false;

		protected int currentFormStep;
		protected int totalFormSteps;

		// PowerOutage-specific custom Form Fields 
		protected string localDescription;
		protected string productImpact;
		protected string selectedShift;
		protected TimeSpan incidentTime;


		// Special answers used in INCIDENT table
		string incidentDescription = "";
		protected DateTime incidentDate;
		protected decimal incidentTypeId;
		protected string incidentType;
		
		PSsqmEntities entities;
		List<EHSIncidentQuestion> questions;
		List<EHSFormControlStep> formSteps;
		

		//public bool IsEditContext { get; set; }
		public decimal IncidentId { get; set; }
		public decimal theincidentId { get; set; }
		public RadGrid EditControlGrid { get; set; }
		
		// Mode should be "incident" (standard) or "prevent" (RMCAR)
		public IncidentMode Mode
		{
			get { return ViewState["Mode"] == null ? IncidentMode.Incident : (IncidentMode)ViewState["Mode"]; }
			set { ViewState["Mode"] = value; }
		}

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
				//RefreshPageContext();
			}
		}

		public int CurrentStep
		{
			get { return ViewState["CurrentStep"] == null ? 0 : (int)ViewState["CurrentStep"]; }
			set { ViewState["CurrentStep"] = value; }
		}

		public int CurrentFormStep
		{
			get { return ViewState["CurrentFormStep"] == null ? 0 : (int)ViewState["CurrentFormStep"]; }
			set { ViewState["CurrentFormStep"] = value; }
		}

		public decimal EditIncidentId
		{
			get { return ViewState["EditIncidentId"] == null ? 0 : (decimal)ViewState["EditIncidentId"]; }
			set { ViewState["EditIncidentId"] = value; }
		}

		public decimal InitialPlantId
		{
			get { return ViewState["InitialPlantId"] == null ? 0 : (decimal)ViewState["InitialPlantId"]; }
			set { ViewState["InitialPlantId"] = value; }
		}

		public decimal SelectedTypeId
		{
			get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
			set { ViewState["SelectedTypeId"] = value; }
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
	
		
		public string SelectedTypeText
		{
			get { return ViewState["SelectedTypeText"] == null ? " " : (string)ViewState["SelectedTypeText"]; }
			set { ViewState["SelectedTypeText"] = value; }
		}

		protected decimal SelectedLocationId
		{
			get { return ViewState["SelectedLocationId"] == null ? 0 : (decimal)ViewState["SelectedLocationId"]; }
			set { ViewState["SelectedLocationId"] = value; }
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//accessLevel = UserContext.CheckAccess("EHS", "312");
			accessLevel = UserContext.CheckAccess("EHS", "");

			lblResults.Text = "";

			if (IsPostBack)
			{
				// Since IsPostBack is always TRUE for every invocation of this control we need some way 
				// to determine whether or not to refresh the page controls.  Here we are using the 
				// "__EVENTTARGET" form event property to see if this control is posting back because 
				// of parent calls from the rddlIncidentType or lbIncidentId controls. If so then 
				// thats NOT a real postback and so we need to initialize all the page controls.

				IsFullPagePostback = true;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);
					if ((this.Page.FindControl(targetID).ID == "rddlIncidentType") || (this.Page.FindControl(targetID).ID == "lbIncidentId"))
						IsFullPagePostback = false;
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

			if (IsEditContext == true)
			{

				var incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
				var poweroutageDetails = EHSIncidentMgr.SelectPowerOurageDetailsById(entities, EditIncidentId);

				if (incident != null)
				{

					CurrentFormStep = CurrentStep + 1;  // incident.INCFORM_LAST_STEP_COMPLETED;


					if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString(), true);
					rdpReportDate.Culture = rdpIncidentDate.Culture;
					rtpIncidentTime.Culture = rdpIncidentDate.Culture;

					
					tbDescription.Text = incident.DESCRIPTION;
					rdpIncidentDate.SelectedDate = incident.INCIDENT_DT;

					PopulateLocationDropDown();
					rddlLocation.SelectedValue = Convert.ToString(incident.DETECT_PLANT_ID);

					PopulateShiftDropDown();
					rdpReportDate.SelectedDate = incident.CREATE_DT;

					if (poweroutageDetails != null)
					{
						rtpIncidentTime.SelectedTime = poweroutageDetails.INCIDENT_TIME;

						rddlShift.SelectedValue = poweroutageDetails.SHIFT;

						if (poweroutageDetails.PRODUCTION_IMPACT != null)
							tbProdImpact.Text = poweroutageDetails.PRODUCTION_IMPACT;

						if (poweroutageDetails.DESCRIPTION_LOCAL != null)
							tbLocalDescription.Text = poweroutageDetails.DESCRIPTION_LOCAL;
					}
				}

			}
			else
			{
				if (!IsFullPagePostback)
				{
					rdpIncidentDate.Clear();
					rdpReportDate.Clear();
					rtpIncidentTime.Clear();
					tbDescription.Text = "";
					tbProdImpact.Text = "";
					rddlLocation.Items.Clear();
					rddlShift.Items.Clear();

					CurrentFormStep = 1;

					if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString(), true);
					rdpIncidentDate.SelectedDate = DateTime.Now;

					rdpReportDate.Culture = rdpIncidentDate.Culture;
					rdpReportDate.SelectedDate = DateTime.Now;

					rtpIncidentTime.Culture = rdpIncidentDate.Culture;

					PopulateLocationDropDown();
					PopulateShiftDropDown();
				}
			}

			InitializeForm(CurrentStep);
		}

		void InitializeForm(int currentStep)
		{

			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(typeId);

			int displayStep = currentStep + 1;
			lblFormStepNumber.Text = "Step " + displayStep.ToString() + " of " + formSteps.Count().ToString() + ":";

			int i = Convert.ToInt32(currentStep);
			string currentFormName = formSteps[i].StepFormName;
			lblFormTitle.Text = formSteps[i].StepHeadingText; ;

			btnNext.Text = (i + 1 <= formSteps.Count()-1) ? formSteps[i + 1].StepHeadingText.Trim() + "  >" : "Next  >";
			btnPrev.Text = (i - 1 >= 0) ? "<  " + formSteps[i - 1].StepHeadingText.Trim() : "<  Prev";

			SetUserAccess(currentFormName);

			switch (currentFormName)
			{
				case "INCFORM_POWEROUTAGE":
					pnlBaseForm.Visible = true;
					//pnlContain.Visible = false;
					//pnlRoot5Y.Visible = false;
					uclroot5y.Visible = false; // New Control
					uclcontain.Visible = false; // New Control
					pnlAction.Visible = false;
					pnlApproval.Visible = false;
					btnPrev.Visible = false;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_CONTAIN":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					//pnlContain.Visible =false;  // remove when done
					//pnlRoot5Y.Visible = false;
					uclroot5y.Visible = false; // New Control
					uclcontain.Visible = true; // New Control
					pnlAction.Visible = false;
					pnlApproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					//rptContain.DataSource = EHSIncidentMgr.GetContainmentList(IncidentId);
					//rptContain.DataBind();
					break;
				case "INCFORM_ROOT5Y":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					//pnlContain.Visible = false;	// remove when testing is completed
					//pnlRoot5Y.Visible = false;  // *****  remove when testing is completed
					uclroot5y.Visible = true; // New Control
					uclcontain.Visible = false; // New Control
					pnlAction.Visible = false;	// remove when testing is completed
					pnlApproval.Visible = false;// remove when testing is completed
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					//rptRootCause.DataSource = EHSIncidentMgr.GetRootCauseList(IncidentId);
					//rptRootCause.DataBind();
					break;
				case "INCFORM_ACTION":
					pnlBaseForm.Visible = false;
					//pnlContain.Visible = false;
					//pnlRoot5Y.Visible = false;
					uclroot5y.Visible = false; // New Control
					uclcontain.Visible = false; // New Control
					pnlAction.Visible = true;
					pnlApproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					rptAction.DataSource = EHSIncidentMgr.GetFinalActionList(IncidentId);
					rptAction.DataBind();
					break;
				case "INCFORM_APPROVAL":
					pnlBaseForm.Visible = false;
					//pnlContain.Visible = false;
					//pnlRoot5Y.Visible = false;
					uclroot5y.Visible = false; // New Control
					uclcontain.Visible = false; // New Control
					pnlAction.Visible = false;
					pnlApproval.Visible = true;
					btnPrev.Visible = true;
					btnNext.Visible = false;
					rptApprovals.DataSource = EHSIncidentMgr.GetApprovalList(IncidentId);
					rptApprovals.DataBind();
					break;
			}

		}

		public void LoadDependantForm(string formName)
		{

			//uclroot5y.Controls.Clear();
			//ucontain.Controls.Clear();

			switch (formName)
			{
				
				case "INCFORM_ROOT5Y":
					uclroot5y.IsEditContext = IsEditContext;
					uclroot5y.IncidentId = EditIncidentId;
					uclroot5y.EditIncidentId = EditIncidentId;
					uclroot5y.SelectedTypeId = SelectedTypeId;
					uclroot5y.NewIncidentId = NewIncidentId;
					uclroot5y.Visible = true;
					uclroot5y.PopulateInitialForm();
					break;
				case "INCFORM_CONTAIN":
					uclcontain.IsEditContext = IsEditContext;
					uclcontain.IncidentId = EditIncidentId;
					uclcontain.EditIncidentId = EditIncidentId;
					uclcontain.SelectedTypeId = SelectedTypeId;
					uclcontain.NewIncidentId = NewIncidentId;
					uclcontain.Visible = true;
					uclcontain.PopulateInitialForm();
					break;
			}
		}


		private void SetUserAccess(string currentFormName)
		{

			// Privilege "update"	= Main incident description (1st page) can be maintained/upadted to db
			// Privilege "action"	= Initial Actions page, 5-Why's page, and Final Actions page can be maintained/upadted to db
			// Privilege "approve"	= Approval page can be maintained/upadted to db.  "Close Incident" button is enabled.

			bool updateAccess = SessionManager.CheckUserPrivilege(SysPriv.update, SysScope.incident);
			bool actionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
			bool approveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);

			switch (currentFormName)
			{
				case "INCFORM_POWEROUTAGE":
					rdpIncidentDate.Enabled = updateAccess;
					rfvIncidentDate.Enabled = updateAccess;
					rdpReportDate.Enabled = updateAccess;
					rddlLocation.Enabled = updateAccess;
					rfvLocation.Enabled = updateAccess;
					tbDescription.Enabled = updateAccess;
					rfvDescription.Enabled = updateAccess;
					tbLocalDescription.Enabled = updateAccess;
					rfvLocalDescription.Enabled = updateAccess;
					rtpIncidentTime.Enabled = updateAccess;
					rfvIncidentTime.Enabled = updateAccess;
					rddlShift.Enabled = updateAccess;
					rfvShift.Enabled = updateAccess;
					tbProdImpact.Enabled = updateAccess;
					btnSave.Enabled = updateAccess;
					break;
				case "INCFORM_CONTAIN":
					btnSave.Enabled = actionAccess;
					break;
				case "INCFORM_ROOT5Y":
					btnSave.Enabled = actionAccess;
					break;
				case "INCFORM_ACTION":
					btnSave.Enabled = actionAccess;
					break;
				case "INCFORM_APPROVAL":
					btnSave.Enabled = approveAccess;
					btnClose.Enabled = approveAccess;
					btnClose.Visible = approveAccess;
					break;
			}
		}

		void PopulateLocationDropDown()
		{
				var plantIdList = SelectPlantIdsByAccessLevel();
				if (plantIdList.Count > 1)
					rddlLocation.Items.Add(new DropDownListItem("[Select One]", ""));
				foreach (decimal pid in plantIdList)
				{
					string plantName = EHSIncidentMgr.SelectPlantNameById(pid);
					rddlLocation.Items.Add(new DropDownListItem(plantName, Convert.ToString(pid)));
				}

				rddlLocation.AutoPostBack = true;
		}

		void PopulateShiftDropDown()
		{

			List<EHSMetaData> shifts = EHSMetaDataMgr.SelectMetaDataList("SHIFT");

			rddlShift.Items.Add(new DropDownListItem("[Select One]", ""));

			if (shifts != null && shifts.Count > 0)
			{
				foreach (var s in shifts)
				{
					{
						rddlShift.Items.Add(new DropDownListItem(s.Text, s.Value ));
					}
				}
			}
			else
			{
				rddlShift.Items.Add(new DropDownListItem("1st Shift", "1"));
				rddlShift.Items.Add(new DropDownListItem("2nd Shift", "2"));
				rddlShift.Items.Add(new DropDownListItem("3rd Shift", "3"));
			}

			rddlShift.SelectedIndexChanged += rddlShift_SelectedIndexChanged;
			rddlShift.AutoPostBack = true;
		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			accessLevel = UserContext.CheckAccess("EHS", "");

			if (accessLevel >= AccessMode.Admin)
			{
				plantIdList = EHSIncidentMgr.SelectPlantIdsByCompanyId(companyId);
			}
			else
			{
				plantIdList = SessionManager.UserContext.PlantAccessList;
				if (plantIdList == null || plantIdList.Count == 0)
				{
					plantIdList.Add(SessionManager.UserContext.HRLocation.Plant.PLANT_ID);
				}
			}

			return plantIdList;
		}

		protected void rddlLocation_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			BuildFilteredUsersDropdownList();
		}

		protected void rddlContainPerson_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}

		protected void rddlActionPerson_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}

		void rddlShift_SelectedIndexChanged(object sender, EventArgs e)
		{
			//selectedShift = rddlShift.SelectedValue;
		}

		void BuildFilteredUsersDropdownList()
		{
			if (rddlLocation != null)
			{
				if (!string.IsNullOrEmpty(rddlLocation.SelectedValue))
					this.SelectedLocationId = Convert.ToDecimal(rddlLocation.SelectedValue);
				else
					this.SelectedLocationId = 0;
			}

			if (rddlFilteredUsers != null)
			{
				rddlFilteredUsers.Items.Clear();
				rddlFilteredUsers.Items.Add(new DropDownListItem("[Select One]", ""));

				var locationPersonList = new List<PERSON>();
				if (this.SelectedLocationId > 0)
				{
					locationPersonList = EHSIncidentMgr.SelectEhsDataOriginatorsAtPlant(this.SelectedLocationId);
					locationPersonList.AddRange(EHSIncidentMgr.SelectDataOriginatorAdditionalPlantAccess(this.SelectedLocationId));
					locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
				}
				//else
				//	locationPersonList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);

				if (locationPersonList.Count > 0)
				{
					foreach (PERSON p in locationPersonList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						// Check for duplicate list items
						var ddli = rddlFilteredUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
						if (ddli == null)
							rddlFilteredUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					// If only one user, select by default
					if (rddlFilteredUsers.Items.Count() == 2)
						rddlFilteredUsers.SelectedIndex = 1;
				}
				else
				{
					rddlFilteredUsers.Items[0].Text = "[No valid users - please change location]";
				}
			}
		}

		public void rptAction_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			bool actionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);

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
					RequiredFieldValidator rvfca = (RequiredFieldValidator)e.Item.FindControl("rfvFinalAction");
					RequiredFieldValidator rvfcp = (RequiredFieldValidator)e.Item.FindControl("rfvActionPerson");
					RequiredFieldValidator rvfsd = (RequiredFieldValidator)e.Item.FindControl("rvfFinalStartDate");

					rddlp.Items.Add(new DropDownListItem("[Select One]", ""));
					var personList = new List<PERSON>();
					//if (CurrentStep == 1)
					//personList = EHSIncidentMgr.SelectIncidentPersonList(EditIncidentId);
					//else if (CurrentStep == 0)
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
					tbca.Enabled = actionAccess;
					rddlp.Enabled = actionAccess;
					sd.Enabled = actionAccess;
					cd.Enabled = actionAccess;
					ic.Enabled = actionAccess;
					rvfca.Enabled = actionAccess;
					rvfcp.Enabled = actionAccess;
					rvfsd.Enabled = actionAccess;

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
				addanother.Visible = actionAccess;
			}

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
					CheckBox cba = (CheckBox)e.Item.FindControl("cbIsAccepted");
					RadDatePicker rda = (RadDatePicker)e.Item.FindControl("rdpAcceptDate");
					
					lb.Text = approval.ITEM_SEQ.ToString();
					lba.Text = approval.APPROVER_PERSON;
					lbm.Text = approval.APPROVAL_MESSAGE;
					cba.Checked = approval.IsAccepted;
					rda.SelectedDate = approval.APPROVAL_DATE;

					// Set user access:
					cba.Enabled = approveAccess;
					rda.Enabled = approveAccess;

					//if (rootCause.ITEM_SEQ > minRowsToValidate)
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


		protected void btnSave_Click(object sender, EventArgs e)
		{

			if (Page.IsValid)
			{
				entities = new PSsqmEntities();

				// Get custom form values
				selectedShift = rddlShift.SelectedValue;
				SelectedLocationId = Convert.ToInt32(rddlLocation.SelectedValue);
				incidentTime = (TimeSpan)rtpIncidentTime.SelectedTime;
				localDescription = "";
				if (!string.IsNullOrEmpty(tbLocalDescription.Text))
					localDescription = tbLocalDescription.Text;
				productImpact = tbProdImpact.Text;

				Save(false);

				formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(incidentTypeId);

				if (btnSave.Enabled)
					lblResults.Text = formSteps[CurrentStep].StepHeadingText + " information was saved";
				else
					lblResults.Text = "";

				InitializeForm(CurrentStep);
			}
			else
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}

		}

		protected void btnPrev_Click(object sender, EventArgs e)
		{
			lblResults.Text = "";
			CurrentStep = CurrentStep - 1;

			InitializeForm(CurrentStep);
		}

		protected void btnNext_Click(object sender, EventArgs e)
		{

			if (Page.IsValid)
			{
				lblResults.Text = "";
				entities = new PSsqmEntities();

				// Get custom form values
				selectedShift = rddlShift.SelectedValue;
				SelectedLocationId = Convert.ToInt32(rddlLocation.SelectedValue);
				incidentTime = (TimeSpan)rtpIncidentTime.SelectedTime;
				localDescription = "";
				if (!string.IsNullOrEmpty(tbLocalDescription.Text))
					localDescription = tbLocalDescription.Text;
				productImpact = tbProdImpact.Text;

				Save(false);

				formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(incidentTypeId);

				if (btnSave.Enabled)
					lblResults.Text = formSteps[CurrentStep].StepHeadingText + " information was saved";
				else
					lblResults.Text = "";


				CurrentStep = CurrentStep + 1;
				InitializeForm(CurrentStep);
				
			}
			else
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}

		}

		protected void Save(bool shouldReturn)
		{

			bool shouldCreate8d = false;
			string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			if (Mode == IncidentMode.Prevent)
				result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			decimal incidentId = 0;

			if (shouldReturn == true)
			{
				Panel pnlForm = (Panel)this.Parent.FindControl("pnlForm");
				pnlForm.Visible = false;

				Panel pnlAddEdit = (Panel)this.Parent.FindControl("pnlAddEdit");
				pnlAddEdit.Visible = false;

				RadButton btnSaveReturn = (RadButton)this.Parent.FindControl("btnSaveReturn");
				btnSaveReturn.Visible = false;

				RadButton btnSaveContinue = (RadButton)this.Parent.FindControl("btnSaveContinue");
				btnSaveContinue.Visible = false;

				RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
				if (rcbWarnNavigate != null)
					rcbWarnNavigate.Visible = false;

				lblResults.Visible = true;
			}

			if (!IsEditContext)
			{
				incidentTypeId = SelectedTypeId;
				incidentType = SelectedTypeText;
				incidentDescription = tbDescription.Text;
				selectedPlantId = SelectedLocationId;
				currentFormStep = CurrentFormStep;
			}
			else
			{
				incidentDescription = tbDescription.Text;
				selectedPlantId = SelectedLocationId;
				incidentTypeId = EditIncidentTypeId;
				incidentId = EditIncidentId;
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);
			}
			
			if (incidentDate == null || incidentDate < DateTime.Now.AddYears(-100))
				incidentDate = DateTime.Now;

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = selectedPlantId;

				decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
				formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(typeId);

				int i = Convert.ToInt32(CurrentStep);
				string savingForm = formSteps[i].StepFormName;

				switch (savingForm)
				{
					case "INCFORM_POWEROUTAGE":
						NewIncidentId = AddUpdateINCFORM_POWEROUTAGE(incidentId);
						break;
					case "INCFORM_CONTAIN":
						if (incidentId == 0)
							incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
						uclcontain.AddUpdateINCFORM_CONTAIN(incidentId);
						break;
					case "INCFORM_ROOT5Y":
						if (incidentId == 0)
							incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
						uclroot5y.AddUpdateINCFORM_ROOT5Y(incidentId);
						break;
					case "INCFORM_ACTION":
						if (incidentId == 0)
							incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
						AddUpdateINCFORM_ACTION(incidentId);
						break;
					case "INCFORM_APPROVAL":
						if (incidentId == 0)
							incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
						AddUpdateINCFORM_APPROVAL(incidentId);
						break;
				}

			InitializeForm(CurrentStep);

			decimal finalPlantId = 0;
			var finalIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			if (finalIncident != null)
				finalPlantId = (decimal)finalIncident.DETECT_PLANT_ID;
			else
				finalPlantId = selectedPlantId;

			// Start plant accounting rollup in a background thread
			Thread thread = new Thread(() => EHSAccountingMgr.RollupPlantAccounting(InitialPlantId, finalPlantId));
			thread.IsBackground = true;
			thread.Start();

			//Thread obj = new Thread(new ThreadStart(EHSAccountingMgr.RollupPlantAccounting(initialPlantId, finalPlantId)));
			//obj.IsBackground = true;
		}

		protected decimal AddUpdateINCFORM_POWEROUTAGE(decimal incidentId)
		{

			INCIDENT theIncident = null;
			INCFORM_POWEROUTAGE thePowerOutageForm = null;

			decimal theincidentId = 0;

			if (!IsEditContext)
			{
				incidentTypeId = SelectedTypeId;
				incidentType = SelectedTypeText;
				incidentDescription = tbDescription.Text;
				selectedPlantId = SelectedLocationId;
				currentFormStep = CurrentFormStep;

				theIncident = CreateNewIncident();
				incidentId = theIncident.INCIDENT_ID;
				theincidentId = theIncident.INCIDENT_ID;
				thePowerOutageForm = CreateNewPowerOutageDetails(incidentId);

				EHSNotificationMgr.NotifyOnCreate(incidentId, selectedPlantId);
			}
			else
			{
				incidentDescription = tbDescription.Text;
				selectedPlantId = SelectedLocationId;
				incidentTypeId = EditIncidentTypeId;
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);

				incidentId = EditIncidentId;
				if (incidentId > 0)
				{
					theIncident = UpdateIncident(incidentId);
					thePowerOutageForm = UpdatePowerOutageDetails(incidentId);

					if (Mode == IncidentMode.Incident)
					{
						EHSIncidentMgr.TryCloseIncident(incidentId);
					}
				}

				theincidentId = incidentId;
			}

			return theincidentId;

		}

		protected void AddUpdateINCFORM_ACTION(decimal incidentId)
		{
			var itemList = new List<INCFORM_ACTION>();
			int seqnumber = 0;

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
				item.ASSIGNED_PERSON_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0: Convert.ToInt32(rddlp.SelectedValue);
				item.ITEM_SEQ = seqnumber;
				item.START_DATE = sd.SelectedDate;
				item.COMPLETION_DATE = cd.SelectedDate;
				item.IsCompleted = ic.Checked;

				itemList.Add(item);
			}

			if (itemList.Count > 0)
				SaveActions(incidentId, itemList);

		}

		protected void AddUpdateINCFORM_APPROVAL(decimal incidentId)
		{
			//var itemList = new List<INCFORM_ACTION>();
			//int seqnumber = 0;

			//foreach (RepeaterItem containtem in rptAction.Items)
			//{
			//	var item = new INCFORM_ACTION();

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
			//}

			//SaveActions(incidentId, itemList);
		}

		#region Save Methods


		protected INCIDENT CreateNewIncident()
		{
			decimal incidentId = 0;
			var newIncident = new INCIDENT()
			{
				DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
				DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
				DETECT_PLANT_ID = selectedPlantId,
				INCIDENT_TYPE = "EHS",
				CREATE_DT = DateTime.Now,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME, 
				LAST_UPD_DT = DateTime.Now,
				DESCRIPTION = incidentDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				INCIDENT_DT = incidentDate,
				ISSUE_TYPE = incidentType,
				ISSUE_TYPE_ID = incidentTypeId,
				INCFORM_LAST_STEP_COMPLETED = currentFormStep
			};

			entities.AddToINCIDENT(newIncident);
						
			entities.SaveChanges();
			incidentId = newIncident.INCIDENT_ID;
			
			return newIncident;
		}

		protected INCIDENT UpdateIncident(decimal incidentId)
		{
			INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				incident.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				incident.DETECT_PLANT_ID = selectedPlantId;
				incident.INCIDENT_TYPE = "EHS";
				incident.DESCRIPTION = incidentDescription;
				incident.INCIDENT_DT = incidentDate;
				incident.ISSUE_TYPE = incidentType;
				incident.ISSUE_TYPE_ID = incidentTypeId;
				incident.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				incident.LAST_UPD_DT = DateTime.Now;

				entities.SaveChanges();
			}

			return incident;

		}

		protected INCFORM_POWEROUTAGE CreateNewPowerOutageDetails(decimal incidentId)
		{

			var newPowerOutageDetails = new INCFORM_POWEROUTAGE()
			{
				INCIDENT_ID = incidentId,
				PRODUCTION_IMPACT = productImpact,
				SHIFT = selectedShift,
				INCIDENT_TIME = incidentTime,
				DESCRIPTION_LOCAL = localDescription
			};

			entities.AddToINCFORM_POWEROUTAGE(newPowerOutageDetails);

			entities.SaveChanges();
			//incidentId = newIncident.INCIDENT_ID;

			return newPowerOutageDetails;
		}

		protected INCFORM_POWEROUTAGE UpdatePowerOutageDetails(decimal incidentId)
		{
			INCFORM_POWEROUTAGE powerOutageDetails = (from po in entities.INCFORM_POWEROUTAGE where po.INCIDENT_ID == incidentId select po).FirstOrDefault();

			if (powerOutageDetails != null)
			{
				powerOutageDetails.PRODUCTION_IMPACT = productImpact;
				powerOutageDetails.SHIFT = selectedShift;
				powerOutageDetails.INCIDENT_TIME = incidentTime;
				powerOutageDetails.DESCRIPTION_LOCAL = localDescription;

				entities.SaveChanges();
			}

			return powerOutageDetails;
		}


		private void SaveActions(decimal incidentId, List<INCFORM_ACTION> itemList)
		{
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
					entities.SaveChanges();
				}
			}
		}

		protected void SaveAttachments(decimal incidentId)
		{
			//if (uploader != null)
			//{
			//	string recordStep = (this.CurrentStep + 1).ToString();

			//	// Add files to database
			//	SessionManager.DocumentContext = new DocumentScope().CreateNew(
			//		SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
			//		SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
			//		);
			//	SessionManager.DocumentContext.RecordType = 40;
			//	SessionManager.DocumentContext.RecordID = incidentId;
			//	SessionManager.DocumentContext.RecordStep = recordStep;
			//	uploader.SaveFiles();
			//}
		}

	
		#endregion


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
					//if (CurrentStep == 1)
					//personList = EHSIncidentMgr.SelectIncidentPersonList(EditIncidentId);
					//else if (CurrentStep == 0)
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
		}
	}
}