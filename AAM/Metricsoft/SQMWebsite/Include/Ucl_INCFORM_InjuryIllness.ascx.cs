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
		protected int IncidentStepCompleted
		{
			get { return ViewState["IncidentStepCompleted"] == null ? 0 : (int)ViewState["IncidentStepCompleted"]; }
			set { ViewState["IncidentStepCompleted"] = value; }
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

		protected decimal CreatePersonId
		{
			get { return ViewState["CreatePersonId"] == null ? 0 : (decimal)ViewState["CreatePersonId"]; }
			set { ViewState["CreatePersonId"] = value; }
		}

		protected decimal SelectInvolvedPersonId
		{
			get { return ViewState["SelectInvolvedPersonId"] == null ? 0 : (decimal)ViewState["SelectInvolvedPersonId"]; }
			set { ViewState["SelectInvolvedPersonId"] = value; }
		}


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			//try
			//{
			//	RadAjaxManager rajaxmgr = null;
			//	Control ctl = this.Parent;
			//	while (true)
			//	{
			//		rajaxmgr = (RadAjaxManager)ctl.FindControl("RadAjaxManager1");
			//		if (rajaxmgr == null)
			//		{
			//			ctl = ctl.Parent;
			//			if (ctl.Parent == null)
			//			{
			//				return;
			//			}
			//			continue;
			//		}
			//		break;
			//	}

			//	if (rajaxmgr != null)
			//		rajaxmgr.AjaxSettings.AddAjaxSetting(rsbInvolvedPerson, lbSupervisor);
			//}
			//catch
			//{
			//}
		}
		
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				entities = new PSsqmEntities();
				companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				lblResults.Text = "";

				//Label lbTitle = (Label)this.Parent.FindControl("lblPageTitle");
				//lbTitle.Visible = false;

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
					}
				}

				IncidentLocationId = SessionManager.IncidentLocation.Plant.PLANT_ID;

				//RadSearchBox controls must be bound on Page_Load
				PopulateInvolvedPersonRSB(IncidentLocationId);
				PopulateWitnessNameRSB(IncidentLocationId);
			}
			catch
			{
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
				if (CultureSettings.gregorianCalendarOverrides.Contains(selectedLanguage))
					System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();

				base.FrameworkInitialize();
			}
		}

		public void InitNewIncident(decimal newTypeID, decimal newLocationID)
		{
			if (newTypeID > 0)
			{
				SessionManager.SetIncidentLocation(newLocationID);
				IncidentLocationId = newLocationID;
				SelectedTypeId = Convert.ToDecimal(newTypeID);
				SelectedTypeText = EHSIncidentMgr.SelectIncidentType(newTypeID).TITLE;
				CreatePersonId = 0;
				EditIncidentId = 0;
				IncidentStepCompleted = 0;
				IsEditContext = false;
				PopulateInitialForm();
				SetSubnav("new");
			}
		}

		public void BindIncident(decimal incidentID)
		{
			IsEditContext = true;
			EditIncidentId = incidentID;
			IncidentStepCompleted = 0;
			PopulateInitialForm();
			SetSubnav("edit");
		}

		public void PopulateInitialForm()
		{

			entities = new PSsqmEntities();
			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
			INCIDENT incident = null;

			if (IsEditContext == true)
			{
				incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
				SelectedTypeId = (decimal)incident.ISSUE_TYPE_ID;
				SelectedTypeText = incident.ISSUE_TYPE;
				CreatePersonId = (decimal)incident.CREATE_PERSON;
				IncidentStepCompleted = incident.INCFORM_LAST_STEP_COMPLETED;

				var injuryIllnessDetails = EHSIncidentMgr.SelectInjuryIllnessDetailsById(entities, EditIncidentId);

				if (incident != null)
				{
					string lang;
					if ((lang=System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()) != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate = SQMBasePage.SetRadDateCulture(rdpIncidentDate, "");
					rdpReportDate = SQMBasePage.SetRadDateCulture(rdpReportDate, "");

					tbDescription.Text = incident.DESCRIPTION;
					rdpIncidentDate.SelectedDate = incidentDate = incident.INCIDENT_DT;

					rdpReportDate.SelectedDate = incident.CREATE_DT;

					IncidentLocationId = Convert.ToDecimal(incident.DETECT_PLANT_ID);

					PopulateDepartmentDropDown((decimal)incident.DETECT_PLANT_ID);

					PopulateJobTenureDropDown();
					PopulateShiftDropDown();
					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();

					if (injuryIllnessDetails != null)
					{

						rtpIncidentTime.SelectedTime = injuryIllnessDetails.INCIDENT_TIME;
						rddlShiftID.SelectedValue = injuryIllnessDetails.SHIFT;

						if (injuryIllnessDetails.DESCRIPTION_LOCAL != null)
							tbLocalDescription.Text = injuryIllnessDetails.DESCRIPTION_LOCAL;

						//Involved Person :
						PERSON invp = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.INVOLVED_PERSON_ID select p).FirstOrDefault();
						string involvedPerson = (invp != null) ? string.Format("{0}, {1}", invp.LAST_NAME, invp.FIRST_NAME) : "";
						if (!String.IsNullOrEmpty(involvedPerson))
						{
							rsbInvolvedPerson.Text = involvedPerson;
							lbSupervisorLabel.Visible = true;
						}

						if (rddlDeptTest.FindItemByValue(injuryIllnessDetails.DEPT_ID.ToString()) != null)
							rddlDeptTest.SelectedValue = injuryIllnessDetails.DEPT_ID.ToString();
						tbInvPersonStatement.Text = injuryIllnessDetails.INVOLVED_PERSON_STATEMENT;
						rdpSupvInformedDate.SelectedDate = injuryIllnessDetails.SUPERVISOR_INFORMED_DT;
						
						PERSON supv = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.SUPERVISOR_PERSON_ID select p).FirstOrDefault();
						lbSupervisor.Text = (supv != null) ? string.Format("{0}, {1}", supv.LAST_NAME, supv.FIRST_NAME) : "[ supervisor not found ]";

						rdoInside.SelectedValue = (!string.IsNullOrEmpty(injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG) && injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG.ToUpper() == "INSIDE") ? "1" : "0";

						tbSupervisorStatement.Text = injuryIllnessDetails.SUPERVISOR_STATEMENT;
						rdoDirectSupv.SelectedValue = (injuryIllnessDetails.COMPANY_SUPERVISED == true) ? "1" : "0";
						rdoErgConcern.SelectedValue = (injuryIllnessDetails.ERGONOMIC_CONCERN == true) ? "1" : "0"; ;
						rdoStdProcsFollowed.SelectedValue = (injuryIllnessDetails.STD_PROCS_FOLLOWED == true) ? "1" : "0";
						rdoTrainingProvided.SelectedValue = (injuryIllnessDetails.TRAINING_PROVIDED == true) ? "1" : "0";
						if (rddlJobTenure.FindItemByValue(injuryIllnessDetails.JOB_TENURE) != null)
							rddlJobTenure.SelectedValue = injuryIllnessDetails.JOB_TENURE;
						rdoFirstAid.SelectedValue = (injuryIllnessDetails.FIRST_AID == true) ? "1" : "0";
						rdoRecordable.SelectedValue = (injuryIllnessDetails.RECORDABLE == true) ? "1" : "0";
						rdoFatality.SelectedValue = (injuryIllnessDetails.FATALITY == true) ? "1" : "0";

						rddlInjuryType.SelectedValue = injuryIllnessDetails.INJURY_TYPE;
						rddlBodyPart.SelectedValue = injuryIllnessDetails.INJURY_BODY_PART;

						SetLostTime(IsFullPagePostback);

					}
					GetAttachments(EditIncidentId);
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
					rddlShiftID.Items.Clear();
					rddlDeptTest.Items.Clear();
					tbInvPersonStatement.Text = "";
					rdpSupvInformedDate.Clear();
					lbSupervisor.Text = "";
					tbSupervisorStatement.Text = "";
					rdoInside.SelectedValue = "";
					rdoDirectSupv.SelectedValue = "";
					rdoErgConcern.SelectedValue = "";
					rdoStdProcsFollowed.SelectedValue = "";
					rdoTrainingProvided.SelectedValue = "";
					rdoFirstAid.SelectedValue = "";
					rdoRecordable.SelectedValue = "";
					rdoLostTime.SelectedValue = "";
					rdpExpectReturnDT.Clear();
					rddlJobTenure.Items.Clear();
					rddlInjuryType.Items.Clear();
					rddlBodyPart.Items.Clear();
					lbSupervisorLabel.Visible = false;

					rdoFirstAid.SelectedValue = "1";
					Severity_Changed(rdoFirstAid, null);

					string lang;
					if ((lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()) != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate = SQMBasePage.SetRadDateCulture(rdpIncidentDate, "");
					rdpReportDate = SQMBasePage.SetRadDateCulture(rdpReportDate, "");

					rdpIncidentDate.SelectedDate = DateTime.Now;
					//rdpReportDate.Culture = rdpIncidentDate.Culture;
					rdpReportDate.SelectedDate = DateTime.Now;
					//rtpIncidentTime.Culture = rdpIncidentDate.Culture;

					PopulateJobTenureDropDown();
					PopulateShiftDropDown();

					PopulateDepartmentDropDown(IncidentLocationId);

					SetLostTime(IsFullPagePostback);

					PopulateInjuryTypeDropDown();
					PopulateBodyPartDropDown();
					GetAttachments(0);
				}
			}

			CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
			InitializeForm(CurrentStep);

			RefreshPageContext();

			pnlBaseForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.originate, IncidentStepCompleted);
		}


		void InitializeForm(int currentStep)
		{

			entities = new PSsqmEntities();
			
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			string currentFormName = (from tc in entities.INCFORM_TYPE_CONTROL where tc.INCIDENT_TYPE_ID == typeId && tc.STEP_NUMBER == currentStep select tc.STEP_FORM).FirstOrDefault();


			SetUserAccess(currentFormName);

			switch (currentFormName)
			{
				case "INCFORM_INJURYILLNESS":
					pnlBaseForm.Visible = true;
					uclroot5y.Visible = false;
					uclCausation.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					lblFormTitle.Text = Resources.LocalizedText.Incident;
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
					uclCausation.Visible = false;
					uclcontain.Visible = true;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					break;
				case "INCFORM_ROOT5Y":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = true;
					uclCausation.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					break;
				case "INCFORM_CAUSATION":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclCausation.Visible = true;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					break;
				case "INCFORM_ACTION":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclCausation.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = true;
					uclapproval.Visible = false;
					ucllosttime.Visible = false;
					break;
				case "INCFORM_APPROVAL":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclCausation.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = true;
					ucllosttime.Visible = false;
					break;
				case "INCFORM_LOSTTIME_HIST":
					LoadDependantForm(currentFormName);
					pnlBaseForm.Visible = false;
					uclroot5y.Visible = false;
					uclroot5y.Visible = false;
					uclcontain.Visible = false;
					uclaction.Visible = false;
					uclapproval.Visible = false;
					ucllosttime.Visible = true;
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

		protected void RefreshPageContext()
		{
			string typeString = Resources.LocalizedText.Incident;

			if (!IsEditContext)
			{
				lblAddOrEditIncident.Text = "New" + "&nbsp" + typeString;
				lblIncidentType.Text = Resources.LocalizedText.IncidentType + ": ";
				lblIncidentType.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SelectedTypeText);
				lblIncidentLocation.Text = "Incident Location: ";
				lblIncidentLocation.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SessionManager.IncidentLocation.Plant.PLANT_NAME);
			}
			else
			{

				lblAddOrEditIncident.Text = typeString + "&nbsp" + WebSiteCommon.FormatID(EditIncidentId, 6);
				lblIncidentType.Text = Resources.LocalizedText.IncidentType + ": ";
				lblIncidentLocation.Text = "Incident Location: ";
				lblIncidentType.Text += ("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + SelectedTypeText);
				lblIncidentLocation.Text += EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(EditIncidentId);
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
				rdoLostTime.Enabled = (lthCount != null && lthCount > 0) ? rdoLostTime.Enabled == false : true;  // ???
		}

		private void SetUserAccess(string currentFormName)
		{
			switch (currentFormName)
			{
				case "INCFORM_INJURYILLNESS":
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
				rddlShiftID.Items.Add(new DropDownListItem("", ""));

				foreach (var s in shifts)
				{
					{
						rddlShiftID.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}

		void PopulateJobTenureDropDown()
		{
			List<EHSMetaData> shifts = EHSMetaDataMgr.SelectMetaDataList("INJURY_TENURE");

			if (shifts != null && shifts.Count > 0)
			{
				rddlJobTenure.Items.Add(new DropDownListItem("", ""));

				foreach (var s in shifts)
				{
					{
						rddlJobTenure.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}

		void PopulateDepartmentDropDown(decimal plantId)
		{
			rddlDeptTest.Items.Clear();

			if (plantId > 0)
			{
				PSsqmEntities entities = new PSsqmEntities();
				List<DEPARTMENT> depts = SQMModelMgr.SelectDepartmentList(entities, plantId);

				if (depts != null && depts.Count > 0)
				{
					rddlDeptTest.Items.Add(new DropDownListItem("", ""));
		
					foreach (var s in depts)
					{
						{
							rddlDeptTest.Items.Add(new DropDownListItem(s.DEPT_NAME, s.DEPT_ID.ToString()));
						}
					}
				}
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
		}

		void PopulateInjuryTypeDropDown()
		{
			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");
			if (injtype != null && injtype.Count > 0)
			{
				rddlInjuryType.Items.Add(new DropDownListItem("", ""));	

				foreach (var s in injtype)
				{
					{
						rddlInjuryType.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}


		void PopulateBodyPartDropDown()
		{
			List<EHSMetaData> parts = EHSMetaDataMgr.SelectMetaDataList("INJURY_PART");

			if (parts != null && parts.Count > 0)
			{
				rddlBodyPart.Items.Add(new DropDownListItem("", ""));

				foreach (var s in parts)
				{
					{
						rddlBodyPart.Items.Add(new DropDownListItem(s.Text, s.Value));
					}
				}
			}
		}


		PERSON GetSupervisor(decimal invPersonId)
		{

			PSsqmEntities entities = new PSsqmEntities();
			var empID = (from p in entities.PERSON where p.PERSON_ID == invPersonId select p.SUPV_EMP_ID).FirstOrDefault();		

			var supv = new PERSON();
			supv = SQMModelMgr.LookupPersonByEmpID(entities, empID);

			return supv;

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
				(e.Item.FindControl("lbWitNamePrompt") as Label).Text = Resources.LocalizedText.Name + ": ";

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
							itmdel.Text = Resources.LocalizedText.DeleteItem;

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
				}
				catch
				{
				}

				if (e.Item.ItemType == ListItemType.Footer)
				{
				Button addanother = (Button)e.Item.FindControl("btnAddWitness");
				addanother.Visible = addanother.Enabled = true;
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
				btnSubnavCausation.Visible = false;
				btnSubnavAction.Visible = false;
				btnSubnavApproval.Visible = false;

				btnDeleteInc.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSIncidentMgr.DeleteCustomIncident(EditIncidentId, typeId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Incident deleted." : "Error deleting incident.";
				lblResults.Text += "</div>";

				ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + hfIncidentDeletedMsg.Value + "');", true);
				Response.Redirect("/EHS/EHS_Incidents.aspx");
			}
		}

		private void SetSubnav(string context)
		{
			if (context == "new")
			{
				ucllosttime.Visible = uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = false;
				btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = false;
				btnDeleteInc.Visible = false;
				uploader.SetViewMode(true);
			}
			else
			{
				ucllosttime.Visible = uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = false;
				btnSubnavLostTime.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
				btnSubnavIncident.Visible = true;
				btnSubnavIncident.Enabled = false;
				btnSubnavIncident.CssClass = "buttonLinkDisabled";
				btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
				uploader.SetViewMode(btnSubnavSave.Enabled);
				btnDeleteInc.Visible = EHSIncidentMgr.CanDeleteIncident(CreatePersonId, IncidentStepCompleted);
			}
		}

		protected void btnSubnavSave_Click(object sender, EventArgs e)
		{
			int status = 0;

			btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavApproval.Visible = btnSubnavAction.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavContainment.Visible = true;

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
				case "35":
					//status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId);
					CurrentStep = (int)EHSFormId.INCFORM_APPROVAL;
					uclCausation.UpdateCausation(EditIncidentId);
					btnSubnav_Click(btnSubnavCausation, null);
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
				//string script = string.Format("alert('{0}');", "Your updates have been saved.");
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
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
			LinkButton btn = (LinkButton)sender;

			pnlBaseForm.Visible = ucllosttime.Visible = uclcontain.Visible = uclroot5y.Visible = uclCausation.Visible = uclaction.Visible = uclapproval.Visible = false;   //divSubnavPage.Visible =
			btnSubnavLostTime.Visible = btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
			CurrentSubnav = btn.CommandArgument;

			btnSubnavLostTime.Enabled = btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = btnSubnavRootCause.Enabled = btnSubnavCausation.Enabled = btnSubnavContainment.Enabled = true;
			btnSubnavLostTime.CssClass = btnSubnavIncident.CssClass = btnSubnavContainment.CssClass = btnSubnavRootCause.CssClass = btnSubnavCausation.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = "buttonLink";

			lblFormTitle.Text = Resources.LocalizedText.Incident;

			decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			switch (btn.CommandArgument)
			{
				case "2":
					lblFormTitle.Text = "Initial Corrective Actions";
					btnSubnavContainment.Enabled = false;
					btnSubnavContainment.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_CONTAIN;
					InitializeForm(CurrentStep);
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, IncidentStepCompleted);
					break;
				case "3":
					lblFormTitle.Text = Resources.LocalizedText.RootCause;
					btnSubnavRootCause.Enabled = false;
					btnSubnavRootCause.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_ROOT5Y;
					InitializeForm(CurrentStep);
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, IncidentStepCompleted);
					break;
				case "35":
					lblFormTitle.Text = "Causation";
					btnSubnavCausation.Enabled = false;
					btnSubnavCausation.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_CAUSATION;
					uclCausation.Visible = true;
					uclCausation.IsEditContext = true;
					uclCausation.EditIncidentId = EditIncidentId;
					uclCausation.PopulateInitialForm(entities);
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, IncidentStepCompleted);
					break;
				case "4":
					lblFormTitle.Text = Resources.LocalizedText.CorrectiveAction;
					btnSubnavAction.Enabled = false;
					btnSubnavAction.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_ACTION;
					InitializeForm(CurrentStep);
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, IncidentStepCompleted);
					break;
				case "5":
					lblFormTitle.Text = Resources.LocalizedText.Approvals;
					btnSubnavApproval.Enabled = false;
					btnSubnavApproval.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_APPROVAL;
					InitializeForm(CurrentStep);
					if ((btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.approve1, IncidentStepCompleted)) == false)
						btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.approve2, IncidentStepCompleted);
					break;
				case "6":
					lblFormTitle.Text = "Lost Time History";
					btnSubnavLostTime.Enabled = false;
					btnSubnavLostTime.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_LOSTTIME_HIST;
					InitializeForm(CurrentStep);
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, IncidentStepCompleted);
					break;
				case "0":
				default:
					lblFormTitle.Text = Resources.LocalizedText.Incident;
					btnSubnavIncident.Visible = true;
					btnSubnavIncident.Enabled = false;
					btnSubnavIncident.CssClass = "buttonLinkDisabled";
					CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
					if (pnlBaseForm.Visible == false)
						pnlBaseForm.Visible = true;
					PopulateInitialForm();
					btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
					uploader.SetViewMode(btnSubnavSave.Enabled);
					break;
			}

		}

		protected void Save(bool shouldReturn)
		{
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
				incidentDescription = tbDescription.Text;
				//currentFormStep = CurrentFormStep;
			}
			else
			{
				incidentDescription = tbDescription.Text;
				incidentId = EditIncidentId;
			}

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = IncidentLocationId;

			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
	
			IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

			entities = new PSsqmEntities();
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
				case "INCFORM_CAUSATION":
					uclCausation.UpdateCausation(EditIncidentId);
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
				incidentDescription = tbDescription.Text;

				//currentFormStep = CurrentFormStep;

				TheIncident = CreateNewIncident();
				EditIncidentId = incidentId = TheIncident.INCIDENT_ID;
				theincidentId = TheIncident.INCIDENT_ID;

				TheINCFORM = CreateNewInjuryIllnessDetails(incidentId);
				SaveAttachments(incidentId);

				//EHSNotificationMgr.NotifyOnCreate(incidentId, IncidentLocationId);
				EHSNotificationMgr.NotifyIncidentStatus(TheIncident, ((int)SysPriv.originate).ToString(), "");
			}
			else
			{
				incidentDescription = tbDescription.Text;

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
			entities = new PSsqmEntities();

			decimal incidentId = 0;
			var newIncident = new INCIDENT()
			{
				INCIDENT_DT = (DateTime)rdpIncidentDate.SelectedDate,
				DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
				DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
				DETECT_PLANT_ID = IncidentLocationId,
				INCIDENT_TYPE = "EHS",
				CREATE_DT = DateTime.UtcNow,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				LAST_UPD_DT = DateTime.Now,
				DESCRIPTION = incidentDescription,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				ISSUE_TYPE = SelectedTypeText,
				ISSUE_TYPE_ID = SelectedTypeId,
				INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined
			};

			entities.AddToINCIDENT(newIncident);

			entities.SaveChanges();
			incidentId = newIncident.INCIDENT_ID;

			return newIncident;
		}

		protected INCIDENT UpdateIncident(decimal incidentId)
		{
			entities = new PSsqmEntities();

			INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				incident.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
				incident.DETECT_PLANT_ID = IncidentLocationId;
				incident.INCIDENT_TYPE = "EHS";
				incident.DESCRIPTION = incidentDescription;
				incident.INCIDENT_DT = (DateTime)rdpIncidentDate.SelectedDate;
				incident.ISSUE_TYPE = SelectedTypeText;
				incident.ISSUE_TYPE_ID = SelectedTypeId;
				incident.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
				incident.LAST_UPD_DT = DateTime.Now;
				if (incident.INCFORM_LAST_STEP_COMPLETED < (int)IncidentStepStatus.defined)
					incident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined;

				entities.SaveChanges();
			}

			return incident;

		}

		protected INCFORM_INJURYILLNESS CreateNewInjuryIllnessDetails(decimal incidentId)
		{

			entities = new PSsqmEntities();

			var newInjryIllnessDetails = new INCFORM_INJURYILLNESS();
			
			newInjryIllnessDetails.INCIDENT_ID = incidentId;
			newInjryIllnessDetails.SHIFT = selectedShift;
			newInjryIllnessDetails.INCIDENT_TIME = incidentTime;
			newInjryIllnessDetails.DESCRIPTION_LOCAL = localDescription;
			newInjryIllnessDetails.INCIDENT_TIME = rtpIncidentTime.SelectedTime;

			if (!string.IsNullOrEmpty(rddlDeptTest.SelectedValue))
			{
				newInjryIllnessDetails.DEPT_ID = Convert.ToInt32(rddlDeptTest.SelectedValue);
				newInjryIllnessDetails.DEPARTMENT = rddlDeptTest.SelectedText;
			}

			newInjryIllnessDetails.SHIFT = rddlShiftID.SelectedValue;

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

			if (!string.IsNullOrEmpty(rddlJobTenure.SelectedValue))
				newInjryIllnessDetails.JOB_TENURE = rddlJobTenure.SelectedValue;

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

				if (item.WORK_STATUS != "")
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
			entities = new PSsqmEntities();

			INCFORM_INJURYILLNESS injuryIllnessDetails = (from po in entities.INCFORM_INJURYILLNESS where po.INCIDENT_ID == incidentId select po).FirstOrDefault();

			if (injuryIllnessDetails != null)
			{
				injuryIllnessDetails.SHIFT = selectedShift;
				injuryIllnessDetails.INCIDENT_TIME = incidentTime;
				injuryIllnessDetails.DESCRIPTION_LOCAL = localDescription;
				injuryIllnessDetails.INCIDENT_TIME = rtpIncidentTime.SelectedTime;

				if (!string.IsNullOrEmpty(rddlDeptTest.SelectedValue))
				{
					injuryIllnessDetails.DEPT_ID = Convert.ToInt32(rddlDeptTest.SelectedValue);
					injuryIllnessDetails.DEPARTMENT = rddlDeptTest.SelectedText;
				}

				injuryIllnessDetails.SHIFT = rddlShiftID.SelectedValue;

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

				if (!string.IsNullOrEmpty(rddlJobTenure.SelectedValue))
					injuryIllnessDetails.JOB_TENURE = rddlJobTenure.SelectedValue;

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
					entities.ExecuteStoreCommand("DELETE FROM INCIDENT_ANSWER WHERE INCIDENT_ID = {0}", incidentId);

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
					ia.ANSWER_VALUE = injuryIllnessDetail.RECORDABLE == true ? "Yes" : "No";
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.LostTimeCase);
					ia.ANSWER_VALUE = injuryIllnessDetail.LOST_TIME == true ? "Yes" : "No";
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Fatality);
					ia.ANSWER_VALUE = injuryIllnessDetail.FATALITY == true ? "Yes" : "No";
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
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.JobTenure);
					ia.ANSWER_VALUE = injuryIllnessDetail.JOB_TENURE;
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.InvolvedPerson);
					ia.ANSWER_VALUE = injuryIllnessDetail.INVOLVED_PERSON_ID.ToString();
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

					ia = new INCIDENT_ANSWER();
					ia.INCIDENT_ID = incidentId;
					ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Shift);
					ia.ANSWER_VALUE = injuryIllnessDetail.SHIFT;
					ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
					entities.AddToINCIDENT_ANSWER(ia);

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
				SessionManager.DocumentContext.RecordStep = "1";
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
					lbSupervisorLabel.Visible = true;
					PERSON supv = (PERSON)GetSupervisor(involvedPersonId);
					lbSupervisor.Text = (supv != null) ? string.Format("{0}, {1}", supv.LAST_NAME, supv.FIRST_NAME) : "[ supervisor not found ]";
				}
				else
				{
					lbSupervisorLabel.Visible = false;
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