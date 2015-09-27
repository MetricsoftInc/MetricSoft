using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;
using SQM.Shared;
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
		protected decimal plantId;
		//protected decimal selectedPlantId;
		protected RadDropDownList rddlFilteredUsers;
		protected bool IsFullPagePostback = false;

		protected decimal involvedPersonId;
		protected decimal witnessPersonId;

		//protected int currentFormStep;
		//protected int totalFormSteps;

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

		//public int CurrentFormStep
		//{
		//	get { return ViewState["CurrentFormStep"] == null ? 0 : (int)ViewState["CurrentFormStep"]; }
		//	set { ViewState["CurrentFormStep"] = value; }
		//}

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

		public INCIDENT TheIncident
		{
			get { return ViewState["TheINCIDENT"] == null ? null : (INCIDENT)ViewState["TheINCIDENT"]; }
			set { ViewState["TheINCIDENT"] = value; }
		}
		public INCFORM_INJURYILLNESS TheINCFORM
		{
			get { return ViewState["TheINCFORM"] == null ? null : (INCFORM_INJURYILLNESS)ViewState["TheINCFORM"]; }
			set { ViewState["TheINCFORM"] = value; }
		}

		public string CurrentSubnav
		{
			get { return ViewState["CurrentSubnav"] == null ? "I" : (string)ViewState["CurrentSubnav"]; }
			set { ViewState["CurrentSubnav"] = value; }
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

		public string SelectedTypeText
		{
			get { return ViewState["SelectedTypeText"] == null ? " " : (string)ViewState["SelectedTypeText"]; }
			set { ViewState["SelectedTypeText"] = value; }
		}

		protected decimal IncidentLocationId
		{
			get { return ViewState["IncidentLocationId"] == null ? 0 : (decimal)ViewState["IncidentLocationId"]; }
			set { ViewState["IncidentLocationId"] = value; }
		}

		protected decimal SelectInvolvedPersonId
		{
			get { return ViewState["SelectInvolvedPersonId"] == null ? 0 : (decimal)ViewState["SelectInvolvedPersonId"]; }
			set { ViewState["SelectInvolvedPersonId"] = value; }
		}


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			RadAjaxManager rajaxmgr = null;
			Control ctl = this.Parent;
			while (true)
			{
				rajaxmgr = (RadAjaxManager)ctl.FindControl("RadAjaxManager1");
				if (rajaxmgr == null)
				{
					ctl = ctl.Parent;
					if (ctl.Parent == null)
					{
						return;
					}
					continue;
				}
				break;
			}

			if (rajaxmgr != null)
				rajaxmgr.AjaxSettings.AddAjaxSetting(rsbInvolvedPerson, lbSupervisor);
		 
		}
		
		protected void Page_Init(object sender, EventArgs e)
		{
			if (SessionManager.SessionContext != null)
			{
				UpdateAccess = SessionManager.CheckUserPrivilege(SysPriv.originate, SysScope.incident);
				ActionAccess = SessionManager.CheckUserPrivilege(SysPriv.action, SysScope.incident);
				ApproveAccess = SessionManager.CheckUserPrivilege(SysPriv.approve, SysScope.incident);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			lblResults.Text = "";

			Label lbTitle = (Label)this.Parent.FindControl("lblPageTitle");
			lbTitle.Visible = false;

			//var inctype = Session["IncidentTypeID"];

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

					if (targetControl is RadioButtonList)
					{
						return;  // we don't want to intercept radio button postbacks
					}

					if (targetControl != null)
						if ((this.Page.FindControl(targetID).ID == "rddlIncidentType") || (this.Page.FindControl(targetID).ID == "lbIncidentId"))
						{
							IsFullPagePostback = false;

							if (this.Page.FindControl(targetID).ID == "rddlIncidentType") // This is a new incident
								btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavApproval.Visible = btnSubnavAction.Visible = btnSubnavRootCause.Visible = btnSubnavContainment.Visible = false;
						}
				}
			}

			IncidentLocationId = SessionManager.IncidentLocation.Plant.PLANT_ID;

			INCIDENT incident = null;
			if (IncidentId != null)
			{
				incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
				//if (incident != null)
				//if (incident.CLOSE_DATE != null && incident.CLOSE_DATE_DATA_COMPLETE != null)
				//btnClose.Text = "Reopen Power Outage Incident";
				if (incident != null && Convert.ToDecimal(incident.DETECT_PLANT_ID) > 0)
					IncidentLocationId = Convert.ToDecimal(incident.DETECT_PLANT_ID);
			}
			
			//RadSearchBox controls must be bound on Page_Load
			PopulateInvolvedPersonRSB(IncidentLocationId);
			PopulateWitnessNameRSB(IncidentLocationId);

			if (!IsFullPagePostback)
			{
				PopulateInitialForm();
				pnlBaseForm.Enabled = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.action);
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
			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			//formSteps = GetFormSteps(typeId);
			//totalFormSteps = formSteps.Count();


			if (IsEditContext == true)
			{
				GetAttachments(EditIncidentId);

				var incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
				var injuryIllnessDetails = EHSIncidentMgr.SelectInjuryIllnessDetailsById(entities, EditIncidentId);

				if (incident != null)
				{
					//CurrentFormStep = CurrentStep + 1;  

					if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString(), true);
					rdpReportDate.Culture = rdpIncidentDate.Culture;
					rtpIncidentTime.Culture = rdpIncidentDate.Culture;

					tbDescription.Text = incident.DESCRIPTION;
					rdpIncidentDate.SelectedDate = incident.INCIDENT_DT;

					rdpReportDate.SelectedDate = incident.CREATE_DT;

					IncidentLocationId = Convert.ToDecimal(incident.DETECT_PLANT_ID);

					PopulateOperationDropDown(IncidentLocationId);
					PopulateDepartmentDropDown(IncidentLocationId);

					PopulateShiftDropDown();
					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();

					if (injuryIllnessDetails != null)
					{
						btnDeleteInc.Visible = true;

						rtpIncidentTime.SelectedTime = injuryIllnessDetails.INCIDENT_TIME;
						rddlShift.SelectedValue = injuryIllnessDetails.SHIFT;

						if (injuryIllnessDetails.DESCRIPTION_LOCAL != null)
							tbLocalDescription.Text = injuryIllnessDetails.DESCRIPTION_LOCAL;

						//Involved Person :
						PERSON invp = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.INVOLVED_PERSON_ID select p).FirstOrDefault();
						string involvedPerson = (invp != null) ? string.Format("{0}, {1}", invp.LAST_NAME, invp.FIRST_NAME) : "";
						if (!String.IsNullOrEmpty(involvedPerson))
							rsbInvolvedPerson.Text = involvedPerson;

						rddlDepartment.SelectedValue = injuryIllnessDetails.DEPT_ID.ToString();
						rddlOperation.SelectedValue = injuryIllnessDetails.PLANT_LINE_ID.ToString();
						tbInvPersonStatement.Text = injuryIllnessDetails.INVOLVED_PERSON_STATEMENT;
						rdpSupvInformedDate.SelectedDate = injuryIllnessDetails.SUPERVISOR_INFORMED_DT;
						
						PERSON supv = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.SUPERVISOR_PERSON_ID select p).FirstOrDefault();
						lbSupervisor.Text = (supv != null) ? string.Format("{0}, {1} ({2})", supv.LAST_NAME, supv.FIRST_NAME, supv.EMAIL) : "[ supervisor not found ]";

						rdoInside.SelectedValue = (!string.IsNullOrEmpty(injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG) && injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG.ToUpper() == "INSIDE") ? "1" : "0";

						tbSupervisorStatement.Text = injuryIllnessDetails.SUPERVISOR_STATEMENT;
						rdoDirectSupv.SelectedValue = (injuryIllnessDetails.COMPANY_SUPERVISED == true) ? "1" : "0";
						rdoErgConcern.SelectedValue = (injuryIllnessDetails.ERGONOMIC_CONCERN == true) ? "1" : "0"; ;
						rdoStdProcsFollowed.SelectedValue = (injuryIllnessDetails.STD_PROCS_FOLLOWED == true) ? "1" : "0";
						rdoTrainingProvided.SelectedValue = (injuryIllnessDetails.TRAINING_PROVIDED == true) ? "1" : "0";
						tbTaskYears.Text = injuryIllnessDetails.YEARS_DOING_JOB.ToString();
						tbTaskMonths.Text = injuryIllnessDetails.MONTHS_DOING_JOB.ToString();
						tbTaskDays.Text = injuryIllnessDetails.DAYS_DOING_JOB.ToString();
						rdoFirstAid.SelectedValue = (injuryIllnessDetails.FIRST_AID == true) ? "1" : "0";
						rdoRecordable.SelectedValue = (injuryIllnessDetails.RECORDABLE == true) ? "1" : "0";
						rdoFatality.SelectedValue = (injuryIllnessDetails.FATALITY == true) ? "1" : "0";

						rddlInjuryType.SelectedValue = injuryIllnessDetails.INJURY_TYPE;
						rddlBodyPart.SelectedValue = injuryIllnessDetails.INJURY_BODY_PART;

						SetLostTime(IsFullPagePostback);

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
					rddlShift.Items.Clear();
					rddlDepartment.Items.Clear();
					rddlOperation.Items.Clear();
					tbInvPersonStatement.Text = "";
					rdpSupvInformedDate.Clear();
					lbSupervisor.Text = "";
					tbSupervisorStatement.Text = "";
					rdoInside.SelectedValue = "";
					rdoDirectSupv.SelectedValue = "";
					rdoErgConcern.SelectedValue = "";
					rdoStdProcsFollowed.SelectedValue = "";
					rdoTrainingProvided.SelectedValue = "";
					tbTaskYears.Text = "0";
					tbTaskMonths.Text = "0";
					tbTaskDays.Text = "0";
					rdoFirstAid.SelectedValue = "";
					rdoRecordable.SelectedValue = "";
					rdoLostTime.SelectedValue = "";
					rdpExpectReturnDT.Clear();
					rddlInjuryType.Items.Clear();
					rddlBodyPart.Items.Clear();

					rdoFirstAid.SelectedValue = "1";
					Severity_Changed(rdoFirstAid, null);

					//CurrentFormStep = 1;

					btnDeleteInc.Visible = false;

					if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString(), true);
					rdpIncidentDate.SelectedDate = DateTime.Now;

					rdpReportDate.Culture = rdpIncidentDate.Culture;
					rdpReportDate.SelectedDate = DateTime.Now;

					rtpIncidentTime.Culture = rdpIncidentDate.Culture;

					PopulateShiftDropDown();

					PopulateOperationDropDown(IncidentLocationId);
					PopulateDepartmentDropDown(IncidentLocationId);

					SetLostTime(IsFullPagePostback);

					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();
					GetAttachments(0);
				}
			}

			CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
			InitializeForm(CurrentStep);
		}


		void InitializeForm(int currentStep)
		{

			PSsqmEntities entities = new PSsqmEntities();
			
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			string currentFormName = (from tc in entities.INCFORM_TYPE_CONTROL where tc.INCIDENT_TYPE_ID == typeId && tc.STEP_NUMBER == currentStep select tc.STEP_FORM).FirstOrDefault();


			SetUserAccess(currentFormName);

			switch (currentFormName)
			{
				case "INCFORM_INJURYILLNESS":
					pnlBaseForm.Visible = true;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					btnDeleteInc.Visible = true;
					lblFormTitle.Text = "Incident";
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					rptWitness.DataSource = EHSIncidentMgr.GetWitnessList(IncidentId);
					//if (!IsFullPagePostback)
						rptWitness.DataBind();
					break;
				case "INCFORM_CONTAIN":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = true;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					btnDeleteInc.Visible = false;
					break;
				case "INCFORM_ROOT5Y":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = true;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					btnDeleteInc.Visible = false;
					break;
				case "INCFORM_ACTION":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = true;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					btnDeleteInc.Visible = false;
					break;
				case "INCFORM_APPROVAL":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = true;
					ucllosttime.Visible = false;
					btnDeleteInc.Visible = false;
					break;
				case "INCFORM_LOSTTIME_HIST":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = true;
					btnDeleteInc.Visible = false;
					break;

			}

		}

		public void LoadDependantForm(string formName)
		{
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
				case "INCFORM_APPROVAL":
					uclapproval.IsEditContext = IsEditContext;
					uclapproval.IncidentId = EditIncidentId;
					uclapproval.EditIncidentId = EditIncidentId;
					uclapproval.SelectedTypeId = SelectedTypeId;
					uclapproval.NewIncidentId = NewIncidentId;
					uclapproval.ValidationGroup = validationGroup;
					uclapproval.Visible = true;
					uclapproval.PopulateInitialForm();
					break;
				case "INCFORM_LOSTTIME_HIST":
					ucllosttime.IsEditContext = IsEditContext;
					ucllosttime.IncidentId = EditIncidentId;
					ucllosttime.EditIncidentId = EditIncidentId;
					ucllosttime.SelectedTypeId = SelectedTypeId;
					ucllosttime.NewIncidentId = NewIncidentId;
					ucllosttime.ValidationGroup = validationGroup;
					ucllosttime.Visible = true;
					ucllosttime.PopulateInitialForm();
					break;
			}
		}

		protected void Severity_Changed(object sender, EventArgs e)
		{
			RadioButtonList rbl = (RadioButtonList)sender;

			switch (rbl.ID)
			{
				case "rdoFirstAid":
					if (rbl.SelectedValue == "1")
					{
						rdoRecordable.Enabled = rdoFatality.Enabled = rdoLostTime.Enabled = false;
						rdoRecordable.SelectedValue = "0";
						rdoFatality.SelectedValue = "0";
						rdoLostTime.SelectedValue = "0";
					}
					else
					{
						rdoRecordable.Enabled = rdoFatality.Enabled = rdoLostTime.Enabled = true;
					}
					break;
				case "rdoRecordable":
					if (rbl.SelectedValue == "1")
					{
						rdoFatality.Enabled = rdoLostTime.Enabled = true;
					}
					else
					{
						rdoFatality.SelectedValue = "0";
						rdoLostTime.SelectedValue = "0";
						rdoFatality.Enabled = rdoLostTime.Enabled = false;
					}
					break;
				case "rdoFatality":
					if (rbl.SelectedValue == "1")
					{
						rdoLostTime.Enabled = false;
						rdoLostTime.SelectedValue = "0";
					}
					else
					{
						rdoLostTime.Enabled = true;
					}
					break;
				case "rdoLostTime":
					break;
				default:
					break;
			}
			
		}

		void SetLostTime(bool isPostBack)
		{
			decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			PSsqmEntities entities = new PSsqmEntities();
			var injuryIllnessDetails = EHSIncidentMgr.SelectInjuryIllnessDetailsById(entities, incidentId);


			int lthCount = 0;
			if (injuryIllnessDetails != null)
				lthCount = (from lth in entities.INCFORM_LOSTTIME_HIST where (lth.INCIDENT_ID == injuryIllnessDetails.INCIDENT_ID) select lth).Count();

			if (!isPostBack)
			{
				rdoLostTime.SelectedValue = "0";
				rdpExpectReturnDT.Clear();
				pnlExpReturnDT.Visible = false;

				if (injuryIllnessDetails != null)
				{
					if (injuryIllnessDetails.LOST_TIME == true)
					{
						rdoLostTime.SelectedValue = "1";
						pnlExpReturnDT.Visible = true;
						if (injuryIllnessDetails.EXPECTED_RETURN_WORK_DT != null)
							rdpExpectReturnDT.SelectedDate = injuryIllnessDetails.EXPECTED_RETURN_WORK_DT;
						else
							rdpExpectReturnDT.SelectedDate = DateTime.Now;
					}
				}

			}
			else
			{

				if (lthCount != null && lthCount > 0)
				{

				}
				else
				{

					rdpExpectReturnDT.Clear();
					pnlExpReturnDT.Visible = false;

					if (rdoLostTime.SelectedValue == "1")
					{
						pnlExpReturnDT.Visible = true;

						if (injuryIllnessDetails != null)
						{
							if (injuryIllnessDetails.LOST_TIME == true)
								rdpExpectReturnDT.SelectedDate = (injuryIllnessDetails.EXPECTED_RETURN_WORK_DT != null) ? injuryIllnessDetails.EXPECTED_RETURN_WORK_DT : null;
							else
								rdpExpectReturnDT.SelectedDate = DateTime.Now;
						}
					}
				}
			}

			if (IsEditContext)
				rdoLostTime.Enabled = (lthCount != null && lthCount > 0) ? rdoLostTime.Enabled == false : UpdateAccess;
		}

		private void SetUserAccess(string currentFormName)
		{

			// Privilege "UpdateAccess"	= Main incident description (1st page) can be maintained/upadted to db
			// Privilege "ActionAccess"	= Initial Actions page, 5-Why's page, and Final Actions page can be maintained/upadted to db
			// Privilege "ApproveAccess"	= Approval page can be maintained/upadted to db.  "Close Incident" button is enabled.

			switch (currentFormName)
			{
				case "INCFORM_INJURYILLNESS":
					rdpIncidentDate.Enabled = UpdateAccess;
					rfvIncidentDate.Enabled = UpdateAccess;
					
					tbDescription.Enabled = UpdateAccess;
					rfvDescription.Enabled = UpdateAccess;
					
					tbLocalDescription.Enabled = UpdateAccess;
					rfvLocalDescription.Enabled = UpdateAccess;
					
					rtpIncidentTime.Enabled = UpdateAccess;
					rfvIncidentTime.Enabled = UpdateAccess;

					rddlShift.Enabled = UpdateAccess;
					rfvShift.Enabled = UpdateAccess;

					rsbInvolvedPerson.Enabled = UpdateAccess;
					tbInvPersonStatement.Enabled = UpdateAccess;
					//rfvInvPersonStatement.Enabled = UpdateAccess;

					rdpSupvInformedDate.Enabled = UpdateAccess;
					//rfvSupvInformedDate.Enabled = UpdateAccess;

					//rddlSupervisor.Enabled = UpdateAccess;
					//rfvSupervisor.Enabled = UpdateAccess;

					tbSupervisorStatement.Enabled = UpdateAccess;
					//rfvSupervisorStatement.Enabled = UpdateAccess;

					rdoInside.Enabled = UpdateAccess;
					//rfvInside.Enabled = UpdateAccess;

					rdoDirectSupv.Enabled = UpdateAccess;
					//rfvDirectSupv.Enabled = UpdateAccess;

					rdoErgConcern.Enabled = UpdateAccess;
					//rfvErgConcern.Enabled = UpdateAccess;

					rdoStdProcsFollowed.Enabled = UpdateAccess;
					//rfvStdProcsFollowed.Enabled = UpdateAccess;

					rdoTrainingProvided.Enabled = UpdateAccess;
					//rfvTrainingProvided.Enabled = UpdateAccess;

					tbTaskYears.Enabled = UpdateAccess;
					tbTaskMonths.Enabled = UpdateAccess;
					tbTaskDays.Enabled = UpdateAccess;

					rdpExpectReturnDT.Enabled = UpdateAccess;
					//rfvExpectReturnDT.Enabled = UpdateAccess;

					rddlInjuryType.Enabled = UpdateAccess;
					//rfvInjuryType.Enabled = UpdateAccess;

					rddlBodyPart.Enabled = UpdateAccess;
					//rfvBodyPart.Enabled = UpdateAccess;

					//btnSave.Enabled = UpdateAccess;
					//btnSubnavSave.Enabled = UpdateAccess;
					break;
				case "INCFORM_CONTAIN":
					//btnSave.Enabled = ActionAccess;
					//btnSubnavSave.Enabled = UpdateAccess;
					break;
				case "INCFORM_ROOT5Y":
					//btnSave.Enabled = ActionAccess;
					//btnSubnavSave.Enabled = UpdateAccess;
					break;
				case "INCFORM_ACTION":
					//btnSave.Enabled = ActionAccess;
					//btnSubnavSave.Enabled = UpdateAccess;
					break;
				case "INCFORM_LOSTTIME_HIST":
					//btnSave.Enabled = ApproveAccess;
					//btnSubnavSave.Enabled = UpdateAccess;
					break;
				case "INCFORM_APPROVAL":
					//btnSubnavSave.Enabled = UpdateAccess;
					//btnSave.Enabled = ApproveAccess;
					//btnClose.Enabled = ApproveAccess;
					//btnClose.Visible = ApproveAccess;
					break;
			}
		}


		void PopulateShiftDropDown()
		{
			List<EHSMetaData> shifts = EHSMetaDataMgr.SelectMetaDataList("SHIFT");

			if (shifts != null && shifts.Count > 0)
			{
				rddlShift.Items.Add(new DropDownListItem("[Select One]", ""));

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


		void PopulateOperationDropDown(decimal plantId)
		{
			rddlOperation.Items.Clear();

			if (plantId > 0)
			{
				PSsqmEntities entities = new PSsqmEntities();
				List<PLANT_LINE> ops = SQMModelMgr.SelectPlantLineList(entities, plantId);

				if (ops != null && ops.Count > 0)
				{
					rddlOperation.Items.Add(new DropDownListItem("[Select One]", ""));
		
					foreach (var s in ops)
					{
						{
							rddlOperation.Items.Add(new DropDownListItem(s.PLANT_LINE_NAME, s.PLANT_LINE_ID.ToString()));
						}
					}
				}

				rddlOperation.SelectedIndexChanged += rddlOperation_SelectedIndexChanged;
				rddlOperation.Enabled = UpdateAccess;
				rfvOperation.Enabled = UpdateAccess;
				rddlOperation.AutoPostBack = true;
			}
			else
			{
				rddlOperation.Enabled = false;
				rfvOperation.Enabled = false;
			}
		}



		void PopulateDepartmentDropDown(decimal plantId)
		{
			rddlDepartment.Items.Clear();

			if (plantId > 0)
			{
				PSsqmEntities entities = new PSsqmEntities();
				List<DEPARTMENT> depts = SQMModelMgr.SelectDepartmentList(entities, plantId);

				if (depts != null && depts.Count > 0)
				{
					rddlDepartment.Items.Add(new DropDownListItem("[Select One]", ""));
		
					foreach (var s in depts)
					{
						{
							rddlDepartment.Items.Add(new DropDownListItem(s.DEPT_NAME, s.DEPT_ID.ToString()));
						}
					}
				}

				rddlDepartment.SelectedIndexChanged += rddlOperation_SelectedIndexChanged;
				rddlDepartment.Enabled = UpdateAccess;
				rddlDepartment.AutoPostBack = true;

				rfvDepartment.Enabled = UpdateAccess;	
			}
			else
			{
				rddlDepartment.Enabled = false;
				rfvDepartment.Enabled = false;
			}
		}

		void PopulateInvolvedPersonRSB(decimal plantId)
		{
			if (plantId > 0)
			{
				rsbInvolvedPerson.Visible = true;
				BindPersonSearchBox(rsbInvolvedPerson, plantId);
			}
			else
				rsbInvolvedPerson.Visible = false;
		}


		void PopulateWitnessNameRSB(decimal plantId)
		{
			if (rptWitness != null && rptWitness.Items.Count > 0)
			{
				foreach (RepeaterItem witnessitem in rptWitness.Items)
				{
					RadSearchBox rsbw = (RadSearchBox)witnessitem.FindControl("rsbWitnessName");
					BindPersonSearchBox(rsbw, plantId);
				}
			}
		}
		

		void BindPersonSearchBox(RadSearchBox searchBox, decimal plantId)
		{
			var companyID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			var personDataList = SQMModelMgr.SelectPlantPersonDataList(companyID, plantId);

			searchBox.DataSource = personDataList;

			searchBox.DataValueField = "PersonID";
			searchBox.DataTextField = "PersonName";
			searchBox.DataBind();

			searchBox.Enabled = UpdateAccess;
		}

		void PopulateInjuryTypeDropDown()
		{
			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");

			if (injtype != null && injtype.Count > 0)
			{
				rddlInjuryType.Items.Add(new DropDownListItem("[Select One]", ""));	

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

			if (parts != null && parts.Count > 0)
			{
				rddlBodyPart.Items.Add(new DropDownListItem("[Select One]", ""));

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


		PERSON GetSupervisor(decimal invPersonId)
		{

			PSsqmEntities entities = new PSsqmEntities();
			var empID = (from p in entities.PERSON where p.PERSON_ID == invPersonId select p.SUPV_EMP_ID).FirstOrDefault();		

			var supv = new PERSON();
			supv = SQMModelMgr.LookupPersonByEmpID(entities, empID);

			return supv;

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


		private void GetAttachments(decimal incidentId)
		{
			uploader.SetAttachmentRecordStep("1");
			// Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
			uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete", "btnDeleteInc", "btnSubnavIncident", "btnSubnavLostTime", "btnSubnavContainment", "btnSubnavRootCause", "btnSubnavAction", "btnSubnavApproval"};


			int attCnt = EHSIncidentMgr.AttachmentCount(incidentId);
			int px = 128;

			if (attCnt > 0)
			{
				px = px + (attCnt * 30) + 35;
				uploader.GetUploadedFiles(40, incidentId, "1");
			}

			// Set the html Div height based on number of attachments to be displayed in the grid:
			dvAttachLbl.Style.Add("height", px.ToString() + "px !important");
			dvAttach.Style.Add("height", px.ToString() + "px !important");
		}


		public void rptWitness_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				int minRowsToValidate = 1;

				try
				{
							INCFORM_WITNESS witness = (INCFORM_WITNESS)e.Item.DataItem;

							RadSearchBox rsbw = (RadSearchBox)e.Item.FindControl("rsbWitnessName");
							TextBox tbws = (TextBox)e.Item.FindControl("tbWitnessStatement");
							RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");


							Label lb = (Label)e.Item.FindControl("lbItemSeq");
							Label lb2 = (Label)e.Item.FindControl("lbItemSeq2");

							Label rqd1 = (Label)e.Item.FindControl("lbRqd1");
							Label rqd2 = (Label)e.Item.FindControl("lbRqd2");

							rsbw.Visible = (IncidentLocationId > 0) ? true : false;
							lb.Text = witness.WITNESS_NO.ToString();
							lb2.Text = witness.WITNESS_NO.ToString();
							itmdel.Text = "Delete Item";

							//get the display name for the search box
							PERSON prsn = (from p in entities.PERSON where p.PERSON_ID == witness.WITNESS_PERSON select p).FirstOrDefault();
							rsbw.Text = string.Format("{0}-{1}, {2}", Convert.ToString(prsn.PERSON_ID), prsn.LAST_NAME, prsn.FIRST_NAME);
							
							tbws.Text = witness.WITNESS_STATEMENT;

							rqd1.Visible = true;
							rqd2.Visible = true;

							if (witness.WITNESS_NO > 1)
							{
								rqd1.Visible = false;
								rqd2.Visible = false;
							}
					
							// Set user access:
							rsbw.Enabled = UpdateAccess;
							tbws.Enabled = UpdateAccess;
							itmdel.Visible = UpdateAccess;
							//if (witness.WITNESS_NO > minRowsToValidate)
							//{
								//rvfw.Enabled = false;
								//rvfws.Enabled = false;
							//}

				}
				catch
				{
				}

				if (e.Item.ItemType == ListItemType.Footer)
				{
				Button addanother = (Button)e.Item.FindControl("btnAddWitness");
				addanother.Visible = UpdateAccess;
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

					RadSearchBox rsbw = (RadSearchBox)witnessitem.FindControl("rsbWitnessName");
					TextBox tbws = (TextBox)witnessitem.FindControl("tbWitnessStatement");
					Label lb = (Label)witnessitem.FindControl("lbItemSeq");
					Label lb2 = (Label)witnessitem.FindControl("lbItemSeq2");
					Label rqd1 = (Label)witnessitem.FindControl("lbRqd1");
					Label rqd2 = (Label)witnessitem.FindControl("lbRqd2");

					if (rsbw != null)
					{
						if (!string.IsNullOrEmpty(rsbw.Text))
						{
							string[] split = rsbw.Text.Split('-');
							if (split.Length > 0)
								item.WITNESS_PERSON = Convert.ToInt32(split[0]);
							item.WITNESS_NAME = rsbw.Text;
						}
						seqnumber = Convert.ToInt32(lb.Text);

						item.WITNESS_NO = seqnumber;
						item.WITNESS_STATEMENT = tbws.Text;

						itemList.Add(item);
					}
				}

				var emptyItem = new INCFORM_WITNESS();

				emptyItem.WITNESS_NO = seqnumber + 1;
				emptyItem.WITNESS_PERSON = null;
				emptyItem.WITNESS_STATEMENT = "";
				
				itemList.Add(emptyItem);

				rptWitness.DataSource = itemList;
				rptWitness.DataBind();

			}
			else if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;
				var itemList = new List<INCFORM_WITNESS>();
				int seqnumber = 0;

				foreach (RepeaterItem witnessitem in rptWitness.Items)
				{
					var item = new INCFORM_WITNESS();

					RadSearchBox rsbw = (RadSearchBox)witnessitem.FindControl("rsbWitnessName");
					TextBox tbws = (TextBox)witnessitem.FindControl("tbWitnessStatement");
					Label lb = (Label)witnessitem.FindControl("lbItemSeq");
					Label lb2 = (Label)witnessitem.FindControl("lbItemSeq2");
					Label rqd1 = (Label)witnessitem.FindControl("lbRqd1");
					Label rqd2 = (Label)witnessitem.FindControl("lbRqd2");


					if (Convert.ToInt32(lb.Text) != delId + 1)
					{
						seqnumber = seqnumber + 1;
						item.WITNESS_NO = seqnumber;


						if (rsbw != null && !String.IsNullOrEmpty(rsbw.Text))
						{
							string[] split = rsbw.Text.Split('-');
							if (split.Length > 0)
								item.WITNESS_PERSON = Convert.ToInt32(split[0]);
						}
		
						item.WITNESS_STATEMENT = tbws.Text;
						itemList.Add(item);
					}
				}

				rptWitness.DataSource = itemList;
				rptWitness.DataBind();

				decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
				SaveWitnesses(incidentId, itemList);

			}

		}

		protected void btnDeleteInc_Click(object sender, EventArgs e)
		{
			if (EditIncidentId > 0)
			{
				decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

				pnlBaseForm.Visible = false;

				btnSubnavSave.Visible = false;
				btnSubnavIncident.Visible = false;
				btnSubnavLostTime.Visible = false;
				btnSubnavContainment.Visible = false;
				btnSubnavRootCause.Visible = false;
				btnSubnavAction.Visible = false;
				btnSubnavApproval.Visible = false;

				btnDeleteInc.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSIncidentMgr.DeleteCustomIncident(EditIncidentId, typeId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Incident deleted." : "Error deleting incident.";
				lblResults.Text += "</div>";
			}

			RadDropDownList rddlInc = (RadDropDownList)this.Parent.FindControl("rddlIncidentType");
			rddlInc.SelectedIndex = 0;
		}

		protected void btnSubnavSave_Click(object sender, EventArgs e)
		{
			int status = 0;

			btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavApproval.Visible = btnSubnavAction.Visible = btnSubnavRootCause.Visible = btnSubnavContainment.Visible = true;

			decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			switch (CurrentSubnav)
			{
				case "2":
					//status = uclcontain.AddUpdateINCFORM_CONTAIN(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_CONTAIN;
					Save(false);
					btnSubnav_Click(btnSubnavContainment, null);
					break;
				case "3":
					//status = uclroot5y.AddUpdateINCFORM_ROOT5Y(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_ROOT5Y;
					Save(false);
					btnSubnav_Click(btnSubnavRootCause, null);
					break;
				case "4":
					//status = uclaction.AddUpdateINCFORM_ACTION(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_ACTION;
					Save(false);
					btnSubnav_Click(btnSubnavAction, null);
					break;
				case "5":
					//status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_APPROVAL;
					Save(false);
					btnSubnav_Click(btnSubnavApproval, null);
					break;
				case "6":
					//status = ucllosttime.AddUpdateINCFORM_LOSTTIME_HIST(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_LOSTTIME_HIST;
					Save(false);
					btnSubnav_Click(btnSubnavLostTime, null);
					break;
				default:
					CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
					Save(false);
					break;
			}
			if (status >= 0)
			{
				string script = string.Format("alert('{0}');", "Your updates have been saved.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);

				if(CurrentSubnav == "I"  &&  TheINCFORM != null)
				{
					if (TheINCFORM.LOST_TIME == true)
						btnSubnav_Click(btnSubnavLostTime, null);
					else
						btnSubnav_Click(btnSubnavContainment, null);
				}
			}
		}

		protected void btnSubnav_Click(object sender, EventArgs e)
		{
			//RadButton btn = (RadButton)sender;

			LinkButton btn = (LinkButton)sender;

			pnlBaseForm.Visible = ucllosttime.Visible = uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = false;   //divSubnavPage.Visible =
			btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
			CurrentSubnav = btn.CommandArgument;

			btnSubnavLostTime.Enabled = btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = btnSubnavRootCause.Enabled = btnSubnavContainment.Enabled = true;
			btnSubnavLostTime.CssClass = btnSubnavIncident.CssClass = btnSubnavContainment.CssClass = btnSubnavRootCause.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = "buttonLink";

			lblFormTitle.Text = "Incident";

			decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			switch (btn.CommandArgument)
			{
				case "2":
					btnDeleteInc.Visible = false;
					lblFormTitle.Text = "Initial Corrective Actions";
					btnSubnavContainment.Enabled = false;
					btnSubnavContainment.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_CONTAIN;
					InitializeForm(CurrentStep);
					btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action);
					break;
				case "3":
					btnDeleteInc.Visible = false;
					lblFormTitle.Text = "Root Cause";
					btnSubnavRootCause.Enabled = false;
					btnSubnavRootCause.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_ROOT5Y;
					InitializeForm(CurrentStep);
					btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action);
					break;
				case "4":
					btnDeleteInc.Visible = false;
					lblFormTitle.Text = "Corrective Action";
					btnSubnavAction.Enabled = false;
					btnSubnavAction.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_ACTION;
					InitializeForm(CurrentStep);
					btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action);
					break;
				case "5":
					btnDeleteInc.Visible = false;
					lblFormTitle.Text = "Approvals";
					btnSubnavApproval.Enabled = false;
					btnSubnavApproval.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_APPROVAL;
					InitializeForm(CurrentStep);
					if ((btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.approve1)) == false)
						btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.approve2);
					break;
				case "6":
					btnDeleteInc.Visible = false;
					lblFormTitle.Text = "Lost Time History";
					btnSubnavLostTime.Enabled = false;
					btnSubnavLostTime.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_LOSTTIME_HIST;
					InitializeForm(CurrentStep);
					btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action);
					break;
				case "0":
				default:
					lblFormTitle.Text = "Incident";
					btnDeleteInc.Visible = true;
					btnSubnavIncident.Visible = true;
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
					if (pnlBaseForm.Visible == false)
						pnlBaseForm.Visible = true;
					PopulateInitialForm();
					break;
			}

		}

		protected void Save(bool shouldReturn)
		{

			//bool shouldCreate8d = false;
			//string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			//if (Mode == IncidentMode.Prevent)
			//	result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

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
				//currentFormStep = CurrentFormStep;
			}
			else
			{
				incidentDescription = tbDescription.Text;
				incidentTypeId = EditIncidentTypeId;
				incidentId = EditIncidentId;
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);
			}

			if (incidentDate == null || incidentDate < DateTime.Now.AddYears(-100))
				incidentDate = DateTime.Now;

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = IncidentLocationId;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
	
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			PSsqmEntities entities = new PSsqmEntities();
			string savingForm = (from tc in entities.INCFORM_TYPE_CONTROL where tc.INCIDENT_TYPE_ID == typeId && tc.STEP_NUMBER == CurrentStep select tc.STEP_FORM).FirstOrDefault();

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
				case "INCFORM_LOSTTIME_HIST":
					if (incidentId == 0)
						incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
					ucllosttime.AddUpdateINCFORM_LOSTTIME_HIST(incidentId);
					break;
			}

			if (incidentId == 0)
				incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
			GetAttachments(incidentId);
						
			InitializeForm(CurrentStep);

			decimal finalPlantId = 0;
			var finalIncident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			if (finalIncident != null)
				finalPlantId = (decimal)finalIncident.DETECT_PLANT_ID;
			else
				finalPlantId = IncidentLocationId;

			// Start plant accounting rollup in a background thread
			/*
			Thread thread = new Thread(() => EHSAccountingMgr.RollupPlantAccounting(InitialPlantId, finalPlantId));
			thread.IsBackground = true;
			thread.Start();
			*/
			//Thread obj = new Thread(new ThreadStart(EHSAccountingMgr.RollupPlantAccounting(initialPlantId, finalPlantId)));
			//obj.IsBackground = true;
		}

		protected decimal AddUpdateINCFORM_INJURYILLNESS(decimal incidentId)
		{
			decimal theincidentId = 0;

			if (!IsEditContext)
			{
				incidentTypeId = SelectedTypeId;
				incidentType = SelectedTypeText;
				incidentDescription = tbDescription.Text;

				//currentFormStep = CurrentFormStep;

				TheIncident = CreateNewIncident();
				incidentId = TheIncident.INCIDENT_ID;
				theincidentId = TheIncident.INCIDENT_ID;

				TheINCFORM = CreateNewInjuryIllnessDetails(incidentId);
				SaveAttachments(incidentId);

				//EHSNotificationMgr.NotifyOnCreate(incidentId, IncidentLocationId);
				EHSNotificationMgr.NotifyIncidentStatus(TheIncident, ((int)SysPriv.originate).ToString(), "");
			}
			else
			{
				incidentDescription = tbDescription.Text;

				incidentTypeId = EditIncidentTypeId;
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);

				incidentId = EditIncidentId;
				if (incidentId > 0)
				{
					TheIncident = UpdateIncident(incidentId);
					TheINCFORM = UpdateInjuryIllnessDetails(incidentId);
					SaveAttachments(incidentId);

					if (Mode == IncidentMode.Incident)
					{
						EHSIncidentMgr.TryCloseIncident(incidentId);
					}
				}

				theincidentId = incidentId;
				EHSNotificationMgr.NotifyIncidentStatus(TheIncident, ((int)SysPriv.update).ToString(), "");
			}

			return theincidentId;

		}

		#region Save Methods


		protected INCIDENT CreateNewIncident()
		{
			PSsqmEntities entities = new PSsqmEntities();

			decimal incidentId = 0;
			var newIncident = new INCIDENT()
			{
				DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
				DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
				DETECT_PLANT_ID = IncidentLocationId,
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
				INCFORM_LAST_STEP_COMPLETED = CurrentStep //currentFormStep
			};

			entities.AddToINCIDENT(newIncident);

			entities.SaveChanges();
			incidentId = newIncident.INCIDENT_ID;

			return newIncident;
		}

		protected INCIDENT UpdateIncident(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();

			INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				incident.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				incident.DETECT_PLANT_ID = IncidentLocationId;
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

			PSsqmEntities entities = new PSsqmEntities();

			var newInjryIllnessDetails = new INCFORM_INJURYILLNESS();
			
			newInjryIllnessDetails.INCIDENT_ID = incidentId;
			newInjryIllnessDetails.SHIFT = selectedShift;
			newInjryIllnessDetails.INCIDENT_TIME = incidentTime;
			newInjryIllnessDetails.DESCRIPTION_LOCAL = localDescription;

			newInjryIllnessDetails.DEPT_ID = Convert.ToInt32(rddlDepartment.SelectedValue);
			newInjryIllnessDetails.DEPARTMENT = rddlDepartment.SelectedText;

			newInjryIllnessDetails.PLANT_LINE_ID = Convert.ToInt32(rddlOperation.SelectedValue);
			newInjryIllnessDetails.OPERATION = rddlOperation.SelectedText;

			involvedPersonId = SelectInvolvedPersonId;
			if (involvedPersonId != null && involvedPersonId != 0)
			{
				newInjryIllnessDetails.INVOLVED_PERSON_ID = involvedPersonId;

				PERSON supv = (PERSON)GetSupervisor(involvedPersonId);
				if (supv != null)
					newInjryIllnessDetails.SUPERVISOR_PERSON_ID = supv.PERSON_ID;
			}

			newInjryIllnessDetails.INVOLVED_PERSON_STATEMENT = tbInvPersonStatement.Text;

			if (rdpSupvInformedDate.SelectedDate != null)
				newInjryIllnessDetails.SUPERVISOR_INFORMED_DT = rdpSupvInformedDate.SelectedDate;
			
			if (!String.IsNullOrEmpty(tbSupervisorStatement.Text))
				newInjryIllnessDetails.SUPERVISOR_STATEMENT = tbSupervisorStatement.Text;

			if (rdoInside.SelectedValue == "1")
				newInjryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Inside";
			else
				newInjryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Outside";

			if (!String.IsNullOrEmpty(rdoDirectSupv.SelectedValue))
				newInjryIllnessDetails.COMPANY_SUPERVISED = Convert.ToBoolean((Convert.ToInt32(rdoDirectSupv.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoErgConcern.SelectedValue))
				newInjryIllnessDetails.ERGONOMIC_CONCERN = Convert.ToBoolean((Convert.ToInt32(rdoErgConcern.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoStdProcsFollowed.SelectedValue))
				newInjryIllnessDetails.STD_PROCS_FOLLOWED = Convert.ToBoolean((Convert.ToInt32(rdoStdProcsFollowed.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoTrainingProvided.SelectedValue))
				newInjryIllnessDetails.TRAINING_PROVIDED = Convert.ToBoolean((Convert.ToInt32(rdoTrainingProvided.SelectedValue)));

			if (!String.IsNullOrEmpty(tbTaskYears.Text))
				newInjryIllnessDetails.YEARS_DOING_JOB = (String.IsNullOrEmpty(tbTaskYears.Text)) ? 0 : Convert.ToInt32(tbTaskYears.Text);

			if (!String.IsNullOrEmpty(tbTaskMonths.Text))
				newInjryIllnessDetails.MONTHS_DOING_JOB = (String.IsNullOrEmpty(tbTaskMonths.Text)) ? 0 : Convert.ToInt32(tbTaskMonths.Text);

			if (!String.IsNullOrEmpty(tbTaskDays.Text))
				newInjryIllnessDetails.DAYS_DOING_JOB = (String.IsNullOrEmpty(tbTaskDays.Text)) ? 0 : Convert.ToInt32(tbTaskDays.Text);

			if (!String.IsNullOrEmpty(rdoFirstAid.SelectedValue))
				newInjryIllnessDetails.FIRST_AID = Convert.ToBoolean((Convert.ToInt32(rdoFirstAid.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoRecordable.SelectedValue))
				newInjryIllnessDetails.RECORDABLE = Convert.ToBoolean((Convert.ToInt32(rdoRecordable.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoFatality.SelectedValue))
				newInjryIllnessDetails.FATALITY = Convert.ToBoolean((Convert.ToInt32(rdoFatality.SelectedValue)));

			if (!String.IsNullOrEmpty(rdoLostTime.SelectedValue))
				newInjryIllnessDetails.LOST_TIME =  Convert.ToBoolean((Convert.ToInt32(rdoLostTime.SelectedValue)));

			if (rdpExpectReturnDT.SelectedDate != null)
				newInjryIllnessDetails.EXPECTED_RETURN_WORK_DT = rdpExpectReturnDT.SelectedDate;

			if (!String.IsNullOrEmpty(rddlInjuryType.SelectedValue))
				newInjryIllnessDetails.INJURY_TYPE = rddlInjuryType.SelectedValue;

			if (!String.IsNullOrEmpty(rddlBodyPart.SelectedValue))
				newInjryIllnessDetails.INJURY_BODY_PART = rddlBodyPart.SelectedValue; 

			entities.AddToINCFORM_INJURYILLNESS(newInjryIllnessDetails);

			entities.SaveChanges();

			AddUpdate_Witnesses(incidentId);

			UpdateInicidentAnswers(incidentId, newInjryIllnessDetails);

			return newInjryIllnessDetails;
		}


		public void AddUpdate_Witnesses(decimal incidentId)
		{
			var itemList = new List<INCFORM_WITNESS>();
			int seqnumber = 0;

			foreach (RepeaterItem witnessitem in rptWitness.Items)
			{
				var item = new INCFORM_WITNESS();

				RadSearchBox rsbw = (RadSearchBox)witnessitem.FindControl("rsbWitnessName");
				TextBox tbws = (TextBox)witnessitem.FindControl("tbWitnessStatement");

				if (rsbw != null && !String.IsNullOrEmpty(rsbw.Text))
				{
					string[] split = rsbw.Text.Split('-');
					if (split.Length > 0)
						item.WITNESS_PERSON = Convert.ToInt32(split[0]);

					seqnumber = seqnumber + 1;
					item.WITNESS_NO = seqnumber;
					item.WITNESS_STATEMENT = tbws.Text;

					itemList.Add(item);
				}
			}

			if (itemList.Count > 0)
				SaveWitnesses(incidentId, itemList);

		}

		private void SaveWitnesses(decimal incidentId, List<INCFORM_WITNESS> itemList)
		{
			PSsqmEntities entities = new PSsqmEntities();

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_WITNESS WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_WITNESS item in itemList)
			{
				var newItem = new INCFORM_WITNESS();

				if (item.WITNESS_PERSON != null)
				{
					newItem.INCIDENT_ID = incidentId;
					newItem.WITNESS_NO = item.WITNESS_NO;
					newItem.WITNESS_PERSON = item.WITNESS_PERSON;
					newItem.WITNESS_STATEMENT = item.WITNESS_STATEMENT;

					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_WITNESS(newItem);
					entities.SaveChanges();
				}
			}
		}


		private void SaveLostTime(decimal incidentId, List<INCFORM_LOSTTIME_HIST> itemList)
		{

			PSsqmEntities entities = new PSsqmEntities();

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_LOSTTIME_HIST WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_LOSTTIME_HIST item in itemList)
			{
				var newItem = new INCFORM_LOSTTIME_HIST();

				if (item.WORK_STATUS != "[Select One]")
				{
					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;

					newItem.WORK_STATUS = item.WORK_STATUS;
					newItem.BEGIN_DT = item.BEGIN_DT;
					newItem.RETURN_TOWORK_DT = item.RETURN_TOWORK_DT;
					newItem.NEXT_MEDAPPT_DT = item.NEXT_MEDAPPT_DT;
					newItem.RETURN_EXPECTED_DT = item.RETURN_EXPECTED_DT;

					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = DateTime.Now;

					entities.AddToINCFORM_LOSTTIME_HIST(newItem);
					entities.SaveChanges();
				}
			}
		}

		protected INCFORM_INJURYILLNESS UpdateInjuryIllnessDetails(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();

			INCFORM_INJURYILLNESS injuryIllnessDetails = (from po in entities.INCFORM_INJURYILLNESS where po.INCIDENT_ID == incidentId select po).FirstOrDefault();

			if (injuryIllnessDetails != null)
			{
				injuryIllnessDetails.SHIFT = selectedShift;
				injuryIllnessDetails.INCIDENT_TIME = incidentTime;
				injuryIllnessDetails.DESCRIPTION_LOCAL = localDescription;


				injuryIllnessDetails.DEPT_ID = Convert.ToInt32(rddlDepartment.SelectedValue);
				injuryIllnessDetails.DEPARTMENT = rddlDepartment.SelectedText;

				injuryIllnessDetails.PLANT_LINE_ID = Convert.ToInt32(rddlOperation.SelectedValue);
				injuryIllnessDetails.OPERATION = rddlOperation.SelectedText;

				if (!String.IsNullOrEmpty(tbInvPersonStatement.Text))
					injuryIllnessDetails.INVOLVED_PERSON_STATEMENT = tbInvPersonStatement.Text;

				if (rdpSupvInformedDate.SelectedDate != null)
					injuryIllnessDetails.SUPERVISOR_INFORMED_DT = rdpSupvInformedDate.SelectedDate;

				involvedPersonId = SelectInvolvedPersonId;
				if (involvedPersonId != null && involvedPersonId != 0)
				{
					injuryIllnessDetails.INVOLVED_PERSON_ID = involvedPersonId;

					PERSON supv = (PERSON)GetSupervisor(involvedPersonId);
					if (supv != null)
						injuryIllnessDetails.SUPERVISOR_PERSON_ID = supv.PERSON_ID;
				}

				injuryIllnessDetails.INVOLVED_PERSON_STATEMENT = tbInvPersonStatement.Text;
				
				if (!String.IsNullOrEmpty(tbSupervisorStatement.Text))
					injuryIllnessDetails.SUPERVISOR_STATEMENT = tbSupervisorStatement.Text;

				if (rdoInside.SelectedValue == "1")
					injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Inside";
				else
					injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Outside";

				if (!String.IsNullOrEmpty(rdoDirectSupv.SelectedValue))
					injuryIllnessDetails.COMPANY_SUPERVISED = Convert.ToBoolean((Convert.ToInt32(rdoDirectSupv.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoErgConcern.SelectedValue))
					injuryIllnessDetails.ERGONOMIC_CONCERN = Convert.ToBoolean((Convert.ToInt32(rdoErgConcern.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoStdProcsFollowed.SelectedValue))
					injuryIllnessDetails.STD_PROCS_FOLLOWED = Convert.ToBoolean((Convert.ToInt32(rdoStdProcsFollowed.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoTrainingProvided.SelectedValue))
					injuryIllnessDetails.TRAINING_PROVIDED = Convert.ToBoolean((Convert.ToInt32(rdoTrainingProvided.SelectedValue)));

				if (!String.IsNullOrEmpty(tbTaskYears.Text))
					injuryIllnessDetails.YEARS_DOING_JOB = (String.IsNullOrEmpty(tbTaskYears.Text)) ? 0 : Convert.ToInt32(tbTaskYears.Text);

				if (!String.IsNullOrEmpty(tbTaskMonths.Text))
					injuryIllnessDetails.MONTHS_DOING_JOB = (String.IsNullOrEmpty(tbTaskMonths.Text)) ? 0 : Convert.ToInt32(tbTaskMonths.Text);

				if (!String.IsNullOrEmpty(tbTaskDays.Text))
					injuryIllnessDetails.DAYS_DOING_JOB = (String.IsNullOrEmpty(tbTaskDays.Text)) ? 0 : Convert.ToInt32(tbTaskDays.Text);

				if (!String.IsNullOrEmpty(rdoFirstAid.SelectedValue))
					injuryIllnessDetails.FIRST_AID = Convert.ToBoolean((Convert.ToInt32(rdoFirstAid.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoRecordable.SelectedValue))
					injuryIllnessDetails.RECORDABLE = Convert.ToBoolean((Convert.ToInt32(rdoRecordable.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoFatality.SelectedValue))
					injuryIllnessDetails.FATALITY = Convert.ToBoolean((Convert.ToInt32(rdoFatality.SelectedValue)));

				if (!String.IsNullOrEmpty(rdoLostTime.SelectedValue))
					injuryIllnessDetails.LOST_TIME = Convert.ToBoolean((Convert.ToInt32(rdoLostTime.SelectedValue)));

				if (rdpExpectReturnDT.SelectedDate != null)
					injuryIllnessDetails.EXPECTED_RETURN_WORK_DT = rdpExpectReturnDT.SelectedDate;

				if (!String.IsNullOrEmpty(rddlInjuryType.SelectedValue))
					injuryIllnessDetails.INJURY_TYPE = rddlInjuryType.SelectedValue;

				if (!String.IsNullOrEmpty(rddlBodyPart.SelectedValue))
					injuryIllnessDetails.INJURY_BODY_PART = rddlBodyPart.SelectedValue; 

				entities.SaveChanges();
				AddUpdate_Witnesses(incidentId);

				UpdateInicidentAnswers(incidentId, injuryIllnessDetails);
			}
			return injuryIllnessDetails;
		}

		protected int UpdateInicidentAnswers(decimal incidentId, INCFORM_INJURYILLNESS injuryIllnessDetail)
		{
			// capture key values from the custom form to save in the 'standard' question/answer data structure
			// we do this to maintain commonality of dowstream analytics and graphing methods
			int status = 0;

			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");
			List<EHSMetaData> injPart = EHSMetaDataMgr.SelectMetaDataList("INJURY_PART");

			using (PSsqmEntities entities = new PSsqmEntities())
			{
				try
				{
					List<decimal> iqList = (from q in entities.INCIDENT_TYPE_COMPANY_QUESTION where q.INCIDENT_TYPE_ID == (int)EHSIncidentTypeId.InjuryIllness select q.INCIDENT_QUESTION_ID).ToList();
					List<INCIDENT_QUESTION> qList = (from q in entities.INCIDENT_QUESTION where iqList.Contains(q.INCIDENT_QUESTION_ID) select q).ToList();

					INCIDENT_ANSWER ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.InjuryType);
					ia.ANSWER_VALUE = injtype.Where(m=> m.Value == injuryIllnessDetail.INJURY_TYPE).Select(m=> m.Text).FirstOrDefault();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.BodyPart);
					ia.ANSWER_VALUE = injPart.Where(m => m.Value == injuryIllnessDetail.INJURY_BODY_PART).Select(m => m.Text).FirstOrDefault();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Recordable);
					ia.ANSWER_VALUE = injuryIllnessDetail.RECORDABLE.ToString();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Fatality);
					ia.ANSWER_VALUE = injuryIllnessDetail.FATALITY.ToString();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.LostTimeCase);
					ia.ANSWER_VALUE = injuryIllnessDetail.LOST_TIME.ToString();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.FirstAid);
					ia.ANSWER_VALUE = injuryIllnessDetail.FIRST_AID.ToString();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Department);
					ia.ANSWER_VALUE = injuryIllnessDetail.DEPARTMENT;
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Operation);
					ia.ANSWER_VALUE = injuryIllnessDetail.OPERATION;
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					entities.ExecuteStoreCommand("DELETE FROM INCIDENT_ANSWER WHERE INCIDENT_ID = {0}", incidentId);  // clear any existing values
					status = entities.SaveChanges();
				}
				catch
				{
				}
			}

			return status;
		}

		protected void SaveAttachments(decimal incidentId)
		{
			if (uploader != null)
			{
				string recordStep = (this.CurrentStep + 1).ToString();

				// Add files to database
				SessionManager.DocumentContext = new DocumentScope().CreateNew(
					SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
					SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
					);
				SessionManager.DocumentContext.RecordType = 40;
				SessionManager.DocumentContext.RecordID = incidentId;
				SessionManager.DocumentContext.RecordStep = recordStep;
				uploader.SaveFiles();
			}
		}

		#endregion

		protected void btnBrowseAttach_Click(object sender, EventArgs e)
		{

		}

		protected void btnUploadAttach_Click(object sender, EventArgs e)
		{

		}

		protected void rsbInvolvedPerson_Search(object sender, SearchBoxEventArgs e)
		{
			SelectInvolvedPersonId = 0;

			if (e.DataItem != null)
			{
				involvedPersonId = Convert.ToDecimal(e.Value.ToString());
				rsbInvolvedPerson.Text = e.Text;

				if (involvedPersonId != null)
				{
					PERSON supv = (PERSON)GetSupervisor(involvedPersonId);
					lbSupervisor.Text = (supv != null) ? string.Format("{0}, {1} ({2})", supv.LAST_NAME, supv.FIRST_NAME, supv.EMAIL) : "[ supervisor not found ]";
				}
			}

			SelectInvolvedPersonId = involvedPersonId; 
	
		}

		protected void rsbWitnessName_Search(object sender, SearchBoxEventArgs e)
		{
			RadSearchBox sb = sender as RadSearchBox;
			
			if (e.DataItem != null)
			{
				sb.Text = e.Text;
			}
		}

	}
}