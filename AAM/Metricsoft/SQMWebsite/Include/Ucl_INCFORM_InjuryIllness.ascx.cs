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
	public partial class Ucl_INCFORM_InjuryIllness : System.Web.UI.UserControl
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

		// InjuryIllness-specific custom Form Fields 
		protected string localDescription;
		//protected string productImpact;
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
			accessLevel = UserContext.CheckAccess("EHS", "");

			lblResults.Text = "";

			if (IsPostBack)
			{
				// Since IsPostBack is always TRUE for every invocation of this user control we need some way 
				// to determine whether or not to refresh its page controls, or just data bind instead.  
				// Here we are using the "__EVENTTARGET" form event property to see if this user control is loading 
				// because of certain parent page control events that are NOT supposed to be fired off as actual postbacks.  

				IsFullPagePostback = true;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);

					if (targetControl != null)
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
				var injuryIllnessDetails = EHSIncidentMgr.SelectInjuryIllnessDetailsById(entities, EditIncidentId);

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

					PopulateSupervisorDropDown();
					rddlSupervisor.SelectedValue = Convert.ToString(injuryIllnessDetails.SUPERVISOR_PERSON_ID);
		
					PopulateShiftDropDown();
					PopulateOperationDropDown();
					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();


					if (injuryIllnessDetails != null)
					{
						rtpIncidentTime.SelectedTime = injuryIllnessDetails.INCIDENT_TIME;
						rddlShift.SelectedValue = injuryIllnessDetails.SHIFT;
						if (injuryIllnessDetails.DESCRIPTION_LOCAL != null)
							tbLocalDescription.Text = injuryIllnessDetails.DESCRIPTION_LOCAL;
						tbDepartment.Text = injuryIllnessDetails.DEPARTMENT;
						rddlOperation.SelectedValue = injuryIllnessDetails.OPERATION;
						tbInvolvedPerson.Text = injuryIllnessDetails.INVOLVED_PERSON_NAME;
						tbInvPersonStatement.Text = injuryIllnessDetails.INVOLVED_PERSON_STATEMENT;
						rdpSupvInformedDate.SelectedDate = injuryIllnessDetails.SUPERVISOR_INFORMED_DT;
						rddlSupervisor.SelectedValue = Convert.ToString(injuryIllnessDetails.SUPERVISOR_PERSON_ID);
						tbSupervisorStatement.Text = injuryIllnessDetails.SUPERVISOR_STATEMENT;
						tbInsideOutside.Text = injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG;
						rdoDirectSupv.SelectedValue = Convert.ToString(injuryIllnessDetails.COMPANY_SUPERVISED);
						rdoErgConcern.SelectedValue = Convert.ToString(injuryIllnessDetails.ERGONOMIC_CONCERN);
						rdoStdProcsFollowed.SelectedValue = Convert.ToString(injuryIllnessDetails.STD_PROCS_FOLLOWED);
						rdoTrainingProvided.SelectedValue = Convert.ToString(injuryIllnessDetails.TRAINING_PROVIDED);
						tbTaskYears.Text = injuryIllnessDetails.YEARS_DOING_JOB.ToString();
						tbTaskMonths.Text = injuryIllnessDetails.MONTHS_DOING_JOB.ToString();
						tbTaskDays.Text = injuryIllnessDetails.DAYS_DOING_JOB.ToString();
						rdoFirstAid.SelectedValue = Convert.ToString(injuryIllnessDetails.FIRST_AID);
						rdoRecordable.SelectedValue = Convert.ToString(injuryIllnessDetails.RECORDABLE);
						rdoLostTime.SelectedValue = Convert.ToString(injuryIllnessDetails.LOST_TIME);
						rdpExpectReturnDT.SelectedDate = injuryIllnessDetails.EXPECTED_RETURN_WORK_DT;
						rddlInjuryType.SelectedValue = injuryIllnessDetails.INJURY_TYPE;
						rddlBodyPart.SelectedValue = injuryIllnessDetails.INJURY_BODY_PART;

						// ToDo:    PopulateWitnesss();
						
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
					tbLocalDescription.Text = "";
					rddlLocation.Items.Clear();
					rddlShift.Items.Clear();
					tbDepartment.Text = "";
					rddlOperation.Items.Clear();
					tbInvolvedPerson.Text = "";
					tbInvPersonStatement.Text = "";
					rdpSupvInformedDate.Clear();
					rddlSupervisor.Items.Clear();
					tbSupervisorStatement.Text = "";
					tbInsideOutside.Text = "";
					rdoDirectSupv.Items.Clear();
					rdoErgConcern.Items.Clear();
					rdoStdProcsFollowed.Items.Clear();
					rdoTrainingProvided.Items.Clear();
					tbTaskYears.Text = "";
					tbTaskMonths.Text = "";
					tbTaskDays.Text = "";
					rdoFirstAid.Items.Clear();
					rdoRecordable.Items.Clear();
					rdoLostTime.Items.Clear();
					rdpExpectReturnDT.Clear();
					rddlInjuryType.Items.Clear();
					rddlBodyPart.Items.Clear();

					// ToDo:    clear witness repeater

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
					PopulateOperationDropDown();
					PopulateSupervisorDropDown();
					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();
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

			btnNext.Text = (i + 1 <= formSteps.Count() - 1) ? formSteps[i + 1].StepHeadingText.Trim() + "  >" : "Next  >";
			btnPrev.Text = (i - 1 >= 0) ? "<  " + formSteps[i - 1].StepHeadingText.Trim() : "<  Prev";

			SetUserAccess(currentFormName);

			switch (currentFormName)
			{
				case "INCFORM_INJURYILLNESS":
					pnlBaseForm.Visible = true;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					btnPrev.Visible = false;
					btnNext.Visible = true;
					btnClose.Visible = false;
					rptWitness.DataSource = EHSIncidentMgr.GetWitnessList(IncidentId);
					rptWitness.DataBind();
					break;
				case "INCFORM_CONTAIN":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = true;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_ROOT5Y":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = true;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_ACTION":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = true;
					uclapproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_APPROVAL":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = true;
					btnPrev.Visible = true;
					btnNext.Visible = false;
					break;
			}

		}

		public void LoadDependantForm(string formName)
		{

			//uclcontain.Controls.Clear();
			//uclroot5y.Controls.Clear();
			//uclaction.Controls.Clear();
			//uclapproval.Controls.Clear();

			string validationGroup = "Val_InjuryIllness";

			switch (formName)
			{

				case "INCFORM_CONTAIN":
					uclcontain.IsEditContext = IsEditContext;
					uclcontain.IncidentId = EditIncidentId;
					uclcontain.EditIncidentId = EditIncidentId;
					uclcontain.SelectedTypeId = SelectedTypeId;
					uclcontain.NewIncidentId = NewIncidentId;
					uclcontain.ValidationGroup = validationGroup;
					uclcontain.Visible = true;
					uclcontain.PopulateInitialForm();
					break;
				case "INCFORM_ROOT5Y":
					uclroot5y.IsEditContext = IsEditContext;
					uclroot5y.IncidentId = EditIncidentId;
					uclroot5y.EditIncidentId = EditIncidentId;
					uclroot5y.SelectedTypeId = SelectedTypeId;
					uclroot5y.NewIncidentId = NewIncidentId;
					uclroot5y.ValidationGroup = validationGroup;
					uclroot5y.Visible = true;
					uclroot5y.PopulateInitialForm();
					break;
				case "INCFORM_ACTION":
					uclaction.IsEditContext = IsEditContext;
					uclaction.IncidentId = EditIncidentId;
					uclaction.EditIncidentId = EditIncidentId;
					uclaction.SelectedTypeId = SelectedTypeId;
					uclaction.NewIncidentId = NewIncidentId;
					uclaction.ValidationGroup = validationGroup;
					uclaction.Visible = true;
					uclaction.PopulateInitialForm();
					break;
				case "INCFORM__APPROVAL":
					uclapproval.IsEditContext = IsEditContext;
					uclapproval.IncidentId = EditIncidentId;
					uclapproval.EditIncidentId = EditIncidentId;
					uclapproval.SelectedTypeId = SelectedTypeId;
					uclapproval.NewIncidentId = NewIncidentId;
					uclapproval.ValidationGroup = validationGroup;
					uclapproval.Visible = true;
					uclapproval.PopulateInitialForm();
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
				case "INCFORM_INJURYILLNESS":
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

					tbDepartment.Enabled = updateAccess;
					rfvDepartment.Enabled = updateAccess;

					rddlOperation.Enabled = updateAccess;
					rfvOperation.Enabled = updateAccess;

					tbInvolvedPerson.Enabled = updateAccess;
					rfvInvolvedPerson.Enabled = updateAccess;

					tbInvPersonStatement.Enabled = updateAccess;
					rfvInvPersonStatement.Enabled = updateAccess;

					rdpSupvInformedDate.Enabled = updateAccess;
					rfvSupvInformedDate.Enabled = updateAccess;

					rddlSupervisor.Enabled = updateAccess;
					rfvSupervisor.Enabled = updateAccess;

					tbSupervisorStatement.Enabled = updateAccess;
					rfvSupervisorStatement.Enabled = updateAccess;

					tbInsideOutside.Enabled = updateAccess;
					rfvInsideOutside.Enabled = updateAccess;

					rdoDirectSupv.Enabled = updateAccess;
					rfvDirectSupv.Enabled = updateAccess;

					rdoErgConcern.Enabled = updateAccess;
					rfvErgConcern.Enabled = updateAccess;

					rdoStdProcsFollowed.Enabled = updateAccess;
					rfvStdProcsFollowed.Enabled = updateAccess;

					rdoTrainingProvided.Enabled = updateAccess;
					rfvTrainingProvided.Enabled = updateAccess;

					tbTaskYears.Enabled = updateAccess;
					tbTaskMonths.Enabled = updateAccess;
					tbTaskDays.Enabled = updateAccess;
					rfvTaskDays.Enabled = updateAccess;

					rdoFirstAid.Enabled = updateAccess;
					rfvFirstAid.Enabled = updateAccess;

					rdoRecordable.Enabled = updateAccess;
					rfvRecordable.Enabled = updateAccess;

					rdoLostTime.Enabled = updateAccess;
					rfvLostTime.Enabled = updateAccess;

					rdpExpectReturnDT.Enabled = updateAccess;
					rfvExpectReturnDT.Enabled = updateAccess;

					rddlInjuryType.Enabled = updateAccess;
					rfvInjuryType.Enabled = updateAccess;

					rddlBodyPart.Enabled = updateAccess;
					rfvBodyPart.Enabled = updateAccess;

					// ToDo add Witness repeater user access

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
						rddlShift.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}

			rddlShift.SelectedIndexChanged += rddlShift_SelectedIndexChanged;
			rddlShift.AutoPostBack = true;
		}


		void PopulateOperationDropDown()
		{
			List<EHSMetaData> ops = EHSMetaDataMgr.SelectMetaDataList("OPERATION");

			rddlOperation.Items.Add(new DropDownListItem("[Select One]", ""));

			if (ops != null && ops.Count > 0)
			{
				foreach (var s in ops)
				{
					{
						rddlOperation.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
			
			rddlOperation.SelectedIndexChanged += rddlOperation_SelectedIndexChanged;
			rddlOperation.AutoPostBack = true;
		}


		void PopulateInjuryTypeDropDown()
		{
			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");

			rddlInjuryType.Items.Add(new DropDownListItem("[Select One]", ""));

			if (injtype != null && injtype.Count > 0)
			{
				foreach (var s in injtype)
				{
					{
						rddlInjuryType.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}

			rddlInjuryType.SelectedIndexChanged += rddlInjuryType_SelectedIndexChanged;
			rddlInjuryType.AutoPostBack = true;
		}


		void PopulateBodyPartDropDown()
		{
			List<EHSMetaData> parts = EHSMetaDataMgr.SelectMetaDataList("INJURY_PART");

			rddlBodyPart.Items.Add(new DropDownListItem("[Select One]", ""));

			if (parts != null && parts.Count > 0)
			{
				foreach (var s in parts)
				{
					{
						rddlBodyPart.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}

			rddlBodyPart.SelectedIndexChanged += rddlInjuryType_SelectedIndexChanged;
			rddlBodyPart.AutoPostBack = true;
		}

		void PopulateSupervisorDropDown()
		{
			rddlSupervisor.Items.Add(new DropDownListItem("[Select One]", ""));
			var personList = new List<PERSON>();
			personList = EHSIncidentMgr.SelectCompanyPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID);
			foreach (PERSON p in personList)
			{
				string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
				rddlSupervisor.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
			}
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

		protected void rddlSupervisor_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}

		void rddlShift_SelectedIndexChanged(object sender, EventArgs e)
		{
			//
		}

		void rddlOperation_SelectedIndexChanged(object sender, EventArgs e)
		{
			//
		}

		void rddlInjuryType_SelectedIndexChanged(object sender, EventArgs e)
		{
			//
		}

		void rddlBodyPart_SelectedIndexChanged(object sender, EventArgs e)
		{
			//
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


		public void rptWitness_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			bool updateAccess = SessionManager.CheckUserPrivilege(SysPriv.update, SysScope.incident);

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				int minRowsToValidate = 1;

				try
				{
							INCFORM_WITNESS witness = (INCFORM_WITNESS)e.Item.DataItem;

							TextBox tbw = (TextBox)e.Item.FindControl("tbWitnessName");
							TextBox tbws = (TextBox)e.Item.FindControl("tbWitnessStatement");

							Label lb = (Label)e.Item.FindControl("lbItemSeq");
							Label lb2 = (Label)e.Item.FindControl("lbItemSeq2");

							Label rqd1 = (Label)e.Item.FindControl("lbRqd1");
							Label rqd2 = (Label)e.Item.FindControl("lbRqd2");

							RequiredFieldValidator rvfw = (RequiredFieldValidator)e.Item.FindControl("rfvWitnessName");
							RequiredFieldValidator rvfws = (RequiredFieldValidator)e.Item.FindControl("rfvWitnessStatement");

							lb.Text = witness.WITNESS_NO.ToString();
							lb2.Text = witness.WITNESS_NO.ToString();
							tbw.Text = witness.WITNESS_NAME;
							tbws.Text = witness.WITNESS_STATEMENT;

							rqd1.Visible = true;
							rqd2.Visible = true;

							if (witness.WITNESS_NO > 1)
							{
								rqd1.Visible = false;
								rqd2.Visible = false;
							}
					
							// Set user access:

							tbw.Enabled = updateAccess;
							rvfw.Enabled = updateAccess;
							tbws.Enabled = updateAccess;
							rvfws.Enabled = updateAccess;
							
							if (witness.WITNESS_NO > minRowsToValidate)
							{
								rvfw.Enabled = false;
								rvfws.Enabled = false;
							}

				}
				catch
				{
				}

				if (e.Item.ItemType == ListItemType.Footer)
				{
				Button addanother = (Button)e.Item.FindControl("btnAddWitness");
				addanother.Visible = updateAccess;
				}

			}
		}
		

		protected void rptWitness_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandArgument == "AddAnother")
			{

				var itemList = new List<INCFORM_WITNESS>();
				int seqnumber = 0;

				foreach (RepeaterItem witnessitem in rptWitness.Items)
				{
					var item = new INCFORM_WITNESS();

					TextBox tbw = (TextBox)witnessitem.FindControl("tbWitnessName");
					TextBox tbws = (TextBox)witnessitem.FindControl("tbWitnessStatement");

					Label lb = (Label)witnessitem.FindControl("lbItemSeq");
					Label lb2 = (Label)witnessitem.FindControl("lbItemSeq2");

					Label rqd1 = (Label)witnessitem.FindControl("lbRqd1");
					Label rqd2 = (Label)witnessitem.FindControl("lbRqd2");

		
					seqnumber = Convert.ToInt32(lb.Text);

					item.WITNESS_NO = seqnumber;
					item.WITNESS_NAME = tbSupervisorStatement.Text;
					item.WITNESS_STATEMENT = tbws.Text;

					itemList.Add(item);
				}

				var emptyItem = new INCFORM_WITNESS();

				emptyItem.WITNESS_NO = seqnumber + 1;
				emptyItem.WITNESS_NAME = "";
				emptyItem.WITNESS_STATEMENT = "";
				
				itemList.Add(emptyItem);

				rptWitness.DataSource = itemList;
				rptWitness.DataBind();

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
				//productImpact = tbProdImpact.Text;

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
				//productImpact = tbProdImpact.Text;

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
				case "INCFORM_INJURYILLNESS":
					NewIncidentId = AddUpdateINCFORM_INJURYILLNESS(incidentId);
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
					uclaction.AddUpdateINCFORM_ACTION(incidentId);
					break;
				case "INCFORM_APPROVAL":
					if (incidentId == 0)
						incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
					uclapproval.AddUpdateINCFORM_APPROVAL(incidentId);
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

		protected decimal AddUpdateINCFORM_INJURYILLNESS(decimal incidentId)
		{

			INCIDENT theIncident = null;
			INCFORM_INJURYILLNESS theInjuryIllnessForm = null;

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
				//thePowerOutageForm = CreateNewPowerOutageDetails(incidentId);
				theInjuryIllnessForm = CreateNewInjuryIllnessDetails(incidentId);

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
					theInjuryIllnessForm = UpdateInjuryIllnessDetails(incidentId);

					if (Mode == IncidentMode.Incident)
					{
						EHSIncidentMgr.TryCloseIncident(incidentId);
					}
				}

				theincidentId = incidentId;
			}

			return theincidentId;

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

		protected INCFORM_INJURYILLNESS CreateNewInjuryIllnessDetails(decimal incidentId)
		{
			var newInjryIllnessDetails = new INCFORM_INJURYILLNESS()
			{
				INCIDENT_ID = incidentId,
				//PRODUCTION_IMPACT = productImpact,
				SHIFT = selectedShift,
				INCIDENT_TIME = incidentTime,
				DESCRIPTION_LOCAL = localDescription
			};

			entities.AddToINCFORM_INJURYILLNESS(newInjryIllnessDetails);

			entities.SaveChanges();
			//incidentId = newIncident.INCIDENT_ID;

			return newInjryIllnessDetails;
		}

		protected INCFORM_INJURYILLNESS UpdateInjuryIllnessDetails(decimal incidentId)
		{
			INCFORM_INJURYILLNESS injuryIllnessDetails = (from po in entities.INCFORM_INJURYILLNESS where po.INCIDENT_ID == incidentId select po).FirstOrDefault();

			if (injuryIllnessDetails != null)
			{
				//injuryIllnessDetails.PRODUCTION_IMPACT = productImpact;
				injuryIllnessDetails.SHIFT = selectedShift;
				injuryIllnessDetails.INCIDENT_TIME = incidentTime;
				injuryIllnessDetails.DESCRIPTION_LOCAL = localDescription;

				entities.SaveChanges();
			}

			return injuryIllnessDetails;
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

		protected void btnBrowseAttach_Click(object sender, EventArgs e)
		{

		}

		protected void btnUploadAttach_Click(object sender, EventArgs e)
		{

		}
	}
}