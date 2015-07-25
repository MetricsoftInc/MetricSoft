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
		protected string UIcultureLanguage;
		protected bool IsFullPagePostback = false;
		 
		protected int currentFormStep;
		protected int totalFormSteps;
		
		// PowerOutage Custom Form Fields - speicific
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
		public RadGrid EditControlGrid { get; set; }
		//public bool IsNewIncident { get; set; }

		//public Ucl_EHSIncidentDetails IncidentDetails
		//{
		//	get { return uclIncidentDetails; }
		//}

		//public void EnableReturnButton(bool bEnabled)
		//{
		//	ahReturn.Visible = bEnabled;
		//}

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

			UIcultureLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName.Split('-')[0];

			lblResults.Text = "";

			if (IsPostBack)
			{
				// Since this user control is called from another (parent) user control via postback, every Page_Load event here will be a postback.  So we need some way 
				// to determine whether or not to refresh the page controls within this user control. Here we are using the "__EVENTTARGET" form event property
				// to see if the "rddlIncidentType" dropdown list control triggered the load event.  If it did we can assume the postback should be a full
				// page postback since the "rddlIncidentType" dropdown list is the control defined in the calling (parent) user control that calls this control.
				IsFullPagePostback = false;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);
					if (this.Page.FindControl(targetID).ID == "rddlIncidentType")
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

			//if (IsFullPagePostback)
			PopulateForm();


		}
		


		// Needed to move javascript to parent page - problem with ajax panel?
		//protected override void OnPreRender(EventArgs e)
		//{
		//	//Page.ClientScript.RegisterClientScriptInclude("PreventionLocation", this.ResolveClientUrl("~/scripts/prevention_location.js"));
		//	base.OnPreRender(e);
		//}


		//public void BuildCaseComboBox()
		//{
		//	PSsqmEntities entities = new PSsqmEntities();

		//	if (rcbCases.Items.Count == 0)
		//	{
		//		List<PROB_CASE> caseList = ProblemCase.SelectProblemCaseList(SessionManager.PrimaryCompany().COMPANY_ID, "EHS", "A");
		//		List<PROB_CASE> userCaseList = ProblemCase.SelectUserCaseList(caseList);

		//		var userCaseListSorted = userCaseList.OrderByDescending(x => x.PROBCASE_ID);

		//		rcbCases.Items.Clear();
		//		rcbCases.Items.Add(new Telerik.Web.UI.RadComboBoxItem("[Select a Problem Case]", ""));
		//		foreach (PROB_CASE c in userCaseListSorted)
		//		{
		//			var incidentId = (from po in entities.PROB_OCCUR where po.PROBCASE_ID == c.PROBCASE_ID select po.INCIDENT_ID).FirstOrDefault();
		//			string descriptor = string.Format("{0:000000} - {1} ({2})", incidentId, c.DESC_SHORT, ((DateTime)c.CREATE_DT).ToShortDateString());
		//			rcbCases.Items.Add(new Telerik.Web.UI.RadComboBoxItem(descriptor, c.PROBCASE_ID.ToString()));
		//		}
		//	}
		//}


		public void PopulateForm()
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

					
					if (UIcultureLanguage != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					rdpReportDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					rtpIncidentTime.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;		
					
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

					//if (CurrentFormStep <= 1)
					//	btnPrev.Visible = false;

					//if (CurrentFormStep >= formSteps.Count())
					//	btnNext.Visible = false;

					//btnSubmit.Text = "Save Incident";
					//btnClose.Visible = true;		

				}

			}
			else
			{
				if (IsFullPagePostback)
				{
					rdpIncidentDate.Clear();
					rdpReportDate.Clear();
					rtpIncidentTime.Clear();
					tbDescription.Text = "";
					tbProdImpact.Text = "";
					rddlLocation.Items.Clear();
					rddlShift.Items.Clear();

					CurrentFormStep = 1;

					//btnPrev.Visible = false;

					//lblFormStepNumber.Text = "Step " + CurrentFormStep.ToString() + " of " + totalFormSteps.ToString() + ":";

					if (UIcultureLanguage != "en")
						pnlLocalDesc.Visible = true;

					rdpIncidentDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					rdpIncidentDate.SelectedDate = DateTime.Now;

					rdpReportDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					rdpReportDate.SelectedDate = DateTime.Now;

					rtpIncidentTime.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

					PopulateLocationDropDown();
					PopulateShiftDropDown();
				}

				//UpdateButtonText();


				// Set submit button text according to viewport size
				//	var viewPortWidth = Convert.ToInt32(Session["vpWidth"]);
				//	if (viewPortWidth <= 768)
				//	{
				//		btnSubmit.Height = 45;
				//		btnSubmit.Text = "Add Incident and \n Send Notifications";
				//	}
				//	else
				//		btnSubmit.Text = "Add Incident and Send Notifications";
				//}

				//plantList = SQMModelMgr.SelectPlantList(entities, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0);

				//pnlSelect.Visible = true;
				//gvPreventLocationsList.DataSource = plantList;
				//gvPreventLocationsList.DataBind();
			}

			InitializeForm(CurrentStep);
		}

		//void SetControlValidators(bool activate, int currentStep)
		//{

		//	switch (currentStep)
		//	{
		//		case 1:
		//			if (activate)
		//				RequiredFieldValidator2.Enabled = true;
		//			else
		//				RequiredFieldValidator2.Enabled = false;
		//			break;
		//		case 2:
		//			if (activate)
		//				RequiredFieldValidator3.Enabled = true;
		//			else
		//				RequiredFieldValidator3.Enabled = false;
		//			break;
		//		case 3:
		//			if (activate)
		//				RequiredFieldValidator4.Enabled = true;
		//			else
		//				RequiredFieldValidator4.Enabled = false;
		//			break;
		//		case 4:
		//			if (activate)
		//				RequiredFieldValidator5.Enabled = true;
		//			else
		//				RequiredFieldValidator5.Enabled = false;
		//			break;
		//	}
		//}


		void InitializeForm(int currentStep)
		{

			int displayStep = currentStep + 1;
			lblFormStepNumber.Text = "Step " + displayStep.ToString() + " of " + totalFormSteps.ToString() + ":";

			int i = Convert.ToInt32(currentStep);
			string currentFormName = formSteps[i].StepFormName;
			lblFormTitle.Text = formSteps[i].StepHeadingText; ;

			switch (currentFormName)
			{
				case "INCFORM_POWEROUTAGE":
					pnlBaseForm.Visible = true;
					pnlContain.Visible = false;
					pnlRoot5Y.Visible = false;
					pnlAction.Visible = false;
					pnlApproval.Visible = false;
					btnPrev.Visible = false;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_CONTAIN":
					pnlBaseForm.Visible = false;
					pnlContain.Visible = true;
					pnlRoot5Y.Visible = false;
					pnlAction.Visible = false;
					pnlApproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_ROOT5Y":
					pnlBaseForm.Visible = false;
					pnlContain.Visible = false;
					pnlRoot5Y.Visible = true;
					pnlAction.Visible = false;
					pnlApproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_ACTION":
					pnlBaseForm.Visible = false;
					pnlContain.Visible = false;
					pnlRoot5Y.Visible = false;
					pnlAction.Visible = true;
					pnlApproval.Visible = false;
					btnPrev.Visible = true;
					btnNext.Visible = true;
					btnClose.Visible = false;
					break;
				case "INCFORM_APPROVAL":
					pnlBaseForm.Visible = false;
					pnlContain.Visible = false;
					pnlRoot5Y.Visible = false;
					pnlAction.Visible = false;
					pnlApproval.Visible = true;
					btnPrev.Visible = true;
					btnNext.Visible = false;
					btnClose.Visible = true;
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
				//if (shouldPopulate)
				//{
				//	rddlLocation.SelectedValue = q.AnswerText;
				//	this.SelectedLocationId = Convert.ToDecimal(q.AnswerText);
				//}
				//rddlLocation.SelectedIndexChanged += rddlLocation_SelectedIndexChanged;
				rddlLocation.AutoPostBack = true;
		}

		void PopulateShiftDropDown()
		{

			List<EHSMetaData> shifts = EHSMetaDataMgr.SelectMetaDataList(System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.CultureName, "SHIFT");

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

			//accessLevel = UserContext.CheckAccess("EHS", "312");
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

		//void rddlLocation_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	BuildFilteredUsersDropdownList();
		//}

		protected void rddlLocation_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			BuildFilteredUsersDropdownList();
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

		void UpdateButtonText()
		{
		//	btnSaveReturn.Text = "Save & Return";

		//	int chInt = (int)EHSQuestionId.Create8D;
		//	string chString = chInt.ToString();
		//	CheckBox create8dCh = (CheckBox)pnlForm.FindControl(chString);

		//	if (create8dCh != null && create8dCh.Checked == true)
		//	{
		//		if (IsEditContext)
		//			btnSaveContinue.Text = "Save & Edit 8D";
		//		else
		//			btnSaveContinue.Text = "Save & Create 8D";
		//	}
		//	else
		//	{
		//		if (IsEditContext)
		//			btnSaveContinue.Text = "Save & Edit Report";
		//		else
		//			btnSaveContinue.Text = "Save & Create Report";
		//	}
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

				lblResults.Text = "Incident information was successfully saved";

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
			//SetControlValidators(false, CurrentStep);
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

				CurrentStep = CurrentStep + 1;
					
					//SetControlValidators(true, CurrentStep);
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
			INCIDENT theIncident = null;
			INCFORM_POWEROUTAGE thePowerOutageForm = null;

			decimal incidentId = 0;

			bool shouldCreate8d = false;
			string result = "<h3>EHS Incident " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";
			if (Mode == IncidentMode.Prevent)
				result = "<h3>Recommendation " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

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
				incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(EditIncidentId);
			}
			
			//questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, CurrentStep);
			//UpdateAnswersFromForm();
			//GetIncidentInfoFromQuestions(questions);

			if (incidentDate == null || incidentDate < DateTime.Now.AddYears(-100))
				incidentDate = DateTime.Now;

			if (incidentDescription.Length > MaxTextLength)
				incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			if (InitialPlantId == 0)
				InitialPlantId = selectedPlantId;


			if (CurrentStep == 0)
			{
				//GetIncidentInfoFromQuestions(questions);
				if (!IsEditContext)
				{

					theIncident = CreateNewIncident();
					incidentId = theIncident.INCIDENT_ID;
					thePowerOutageForm = CreateNewPowerOutageDetails(incidentId);
					
					EHSNotificationMgr.NotifyOnCreate(incidentId, selectedPlantId);
				}
				else
				{
					// Edit context - step 0
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
				}
				//if (incidentId > 0)
				//{
				//	shouldCreate8d = AddOrUpdateAnswers(questions, incidentId);
				//	SaveAttachments(incidentId);
				//}

				//if (Mode == IncidentMode.Prevent)
				//	UpdateTaskInfo(questions, incidentId, (DateTime)theIncident.CREATE_DT);
			}
			else if (CurrentStep > 0)
			{
				int i = Convert.ToInt32(CurrentStep);
				string savingForm = formSteps[i].StepFormName;

				switch (savingForm)
				{
					case "INCFORM_CONTAIN":
						AddUpdateINCFORM_CONTAIN(incidentId);
						break;
					case "INCFORM_ROOT5Y":
						AddUpdateINCFORM_ROOT5Y(incidentId);
						break;
					case "INCFORM_ACTION":
						AddUpdateINCFORM_ACTION(incidentId);
						break;
					case "INCFORM_APPROVAL":
						AddUpdateINCFORM_APPROVAL(incidentId);
						break;
				}

				InitializeForm(CurrentStep);

	
				// Edit context - step 1
				//incidentId = EditIncidentId;
				//if (incidentId > 0)
				//{
				//	//AddOrUpdateAnswers(questions, incidentId);
				//	//shouldCreate8d = false;
				//	SaveAttachments(incidentId);

				//	if (Mode == IncidentMode.Incident)
				//	{
				//		UpdateTaskInfo(questions, incidentId, DateTime.Now);
				//		EHSIncidentMgr.TryCloseIncident(incidentId);
				//	}
				//	else
				//	{
				//		EHSIncidentMgr.TryClosePrevention(incidentId, SessionManager.UserContext.Person.PERSON_ID);
				//	}
				//}
			}

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


			//if (shouldCreate8d == true && shouldReturn == false)
			//	Create8dAndRedirect(incidentId);
			//else 
			//if (CurrentStep == 0 && shouldReturn == false)
				//GoToNextStep(incidentId);
			//$$ else
				//$$ ShowIncidentDetails(incidentId, result);

		}


		protected void AddUpdateINCFORM_CONTAIN(decimal incidentId)
		{
			INCFORM_CONTAIN newContainActions = null;

			if (!IsEditContext)   // Add New
			{

				int nextSeq = EHSIncidentMgr.GetNextContainSequence(incidentId);

				newContainActions = new INCFORM_CONTAIN()
				{
					INCIDENT_ID = incidentId,
					ITEM_SEQ = nextSeq,
					//PRODUCTION_IMPACT = productImpact,
					//SHIFT = selectedShift,
					//INCIDENT_TIME = incidentTime,
					//DESCRIPTION_LOCAL = localDescription
				};

				entities.AddToINCFORM_CONTAIN(newContainActions);

				entities.SaveChanges();
			}
			else
			{
				if (incidentId > 0)  // Update
				{

					INCFORM_CONTAIN containActions = (from po in entities.INCFORM_CONTAIN where po.INCIDENT_ID == incidentId select po).FirstOrDefault();

					if (containActions != null)
					{
						//powerOutageDetails.PRODUCTION_IMPACT = productImpact;
						//powerOutageDetails.SHIFT = selectedShift;
						//powerOutageDetails.INCIDENT_TIME = incidentTime;
						//powerOutageDetails.DESCRIPTION_LOCAL = localDescription;

						entities.SaveChanges();
					}
				}
			}
		}

		protected void AddUpdateINCFORM_ROOT5Y(decimal incidentId)
		{

		}

		protected void AddUpdateINCFORM_ACTION(decimal incidentId)
		{

		}

		protected void AddUpdateINCFORM_APPROVAL(decimal incidentId)
		{

		}



		#region Save Methods

		//protected void UpdateAnswersFromForm()
		//{
			// Save to dates & times to database using US culture info
			//DateTimeFormatInfo fromDtfi = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
			//DateTimeFormatInfo toDtfi = new CultureInfo("en-US", false).DateTimeFormat;

			//foreach (var q in questions)
			//{
			//	var control = pnlForm.FindControl(q.QuestionId.ToString());
			//	string answer = "";
			//	if (control != null)
			//	{
			//		switch (q.QuestionType)
			//		{
			//			case EHSIncidentQuestionType.TextField:
			//				answer = (control as RadTextBox).Text;
			//				break;

			//			case EHSIncidentQuestionType.TextBox:
			//				answer = (control as RadTextBox).Text;
			//				if (q.QuestionId == (decimal)EHSQuestionId.Description)
			//					incidentDescription = answer;
			//				break;

			//			case EHSIncidentQuestionType.RichTextBox:
			//				answer = (control as RadEditor).Content;
			//				if (q.QuestionId == (decimal)EHSQuestionId.RecommendationSummary)
			//					incidentDescription = answer;
			//				break;

			//			case EHSIncidentQuestionType.Radio:
			//				answer = (control as RadioButtonList).SelectedValue;
			//				break;

			//			case EHSIncidentQuestionType.CheckBox:
			//				foreach (var item in (control as CheckBoxList).Items)
			//					if ((item as ListItem).Selected)
			//						answer += (item as ListItem).Value + "|";
			//				break;

			//			case EHSIncidentQuestionType.Dropdown:
			//				answer = (control as RadDropDownList).SelectedValue;
			//				break;

			//			case EHSIncidentQuestionType.Date:
			//				DateTime? fromDate = (control as RadDatePicker).SelectedDate;
			//				if (fromDate != null)
			//				{
			//					answer = ((DateTime)fromDate).ToString(CultureInfo.GetCultureInfo("en-US"));
			//					if (q.QuestionId == (decimal)EHSQuestionId.IncidentDate || q.QuestionId == (decimal)EHSQuestionId.InspectionDate)
			//						incidentDate = (DateTime)fromDate;
			//				}
			//				break;

			//			case EHSIncidentQuestionType.Time:
			//				TimeSpan? fromTime = (control as RadTimePicker).SelectedTime;
			//				if (fromTime != null)
			//					answer = ((TimeSpan)fromTime).ToString("c", CultureInfo.GetCultureInfo("en-US"));
			//				break;

			//			case EHSIncidentQuestionType.DateTime:
			//				DateTime? fromDateTime = (control as RadDateTimePicker).SelectedDate;
			//				if (fromDateTime != null)
			//					answer = ((DateTime)fromDateTime).ToString(CultureInfo.GetCultureInfo("en-US"));
			//				break;

			//			case EHSIncidentQuestionType.BooleanCheckBox:
			//				answer = (control as CheckBox).Checked ? "Yes" : "No";
			//				break;

			//			case EHSIncidentQuestionType.Attachment:
			//				uploader = (control as Ucl_RadAsyncUpload);
			//				//foreach (var file1 in uploader.RAUpload.fil
			//				foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
			//					answer += file.FileName + "|";
			//				break;

			//			case EHSIncidentQuestionType.CurrencyTextBox:
			//				answer = (control as RadNumericTextBox).Text;
			//				break;

			//			case EHSIncidentQuestionType.PercentTextBox:
			//				answer = (control as RadNumericTextBox).Text;
			//				break;

			//			case EHSIncidentQuestionType.StandardsReferencesDropdown:
			//				answer = (control as RadComboBox).SelectedValue;
			//				break;

			//			case EHSIncidentQuestionType.LocationDropdown:
			//				answer = (control as RadDropDownList).SelectedValue;
			//				if (!string.IsNullOrEmpty(answer))
			//					selectedPlantId = Convert.ToDecimal(answer);
			//				break;

			//			case EHSIncidentQuestionType.UsersDropdown:
			//				answer = (control as RadDropDownList).SelectedValue;
			//				break;

			//			case EHSIncidentQuestionType.RequiredYesNoRadio:
			//				answer = (control as RadioButtonList).SelectedValue;
			//				break;

			//			case EHSIncidentQuestionType.PageOneAttachment:
			//				uploader = (control as Ucl_RadAsyncUpload);
			//				foreach (UploadedFile file in uploader.RAUpload.UploadedFiles)
			//					answer += file.FileName + "|";
			//				break;

			//			case EHSIncidentQuestionType.UsersDropdownLocationFiltered:
			//				answer = (control as RadDropDownList).SelectedValue;
			//				break;
			//		}
			//	}
			//	q.AnswerText = answer;
			//}

			//$$ if (incidentDate == null || incidentDate < DateTime.Now.AddYears(-100))
			//$$ incidentDate = DateTime.Now;

			//$$ if (incidentDescription.Length > MaxTextLength)
			//$$ incidentDescription = incidentDescription.Substring(0, MaxTextLength);

			//if (InitialPlantId == 0)
				//InitialPlantId = selectedPlantId;
		//}

		//protected void GetIncidentInfoFromQuestions(List<EHSIncidentQuestion> questions)
		//{
			//foreach (var q in questions)
			//{
			//	string answer = q.AnswerText;

			//	if (answer != null)
			//	{
			//		// Special case values to populate in incident table
			//		if (q.QuestionType == EHSIncidentQuestionType.TextBox && q.QuestionId == (decimal)EHSQuestionId.Description)
			//			incidentDescription = answer;
			//		if (q.QuestionType == EHSIncidentQuestionType.Date && q.QuestionId == (decimal)EHSQuestionId.IncidentDate)
			//			incidentDate = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US"));
			//	}

			//	if (answer.Length > MaxTextLength)
			//		answer = answer.Substring(0, MaxTextLength);
			//	if (incidentDescription.Length > MaxTextLength)
			//		incidentDescription = incidentDescription.Substring(0, MaxTextLength);
			//}
		//}

		protected bool AddOrUpdateAnswers(List<EHSIncidentQuestion> questions, decimal incidentId)
		{
			bool shouldCreate8d = false;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var incidentAnswer = (from ia in entities.INCIDENT_ANSWER
									  where ia.INCIDENT_ID == incidentId
										  && ia.INCIDENT_QUESTION_ID == thisQuestion.QuestionId
									  select ia).FirstOrDefault();
				if (incidentAnswer != null)
				{
					incidentAnswer.ANSWER_VALUE = q.AnswerText;
					incidentAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText;
				}
				else
				{
					incidentAnswer = new INCIDENT_ANSWER()
					{
						INCIDENT_ID = incidentId,
						INCIDENT_QUESTION_ID = q.QuestionId,
						ANSWER_VALUE = q.AnswerText,
						ORIGINAL_QUESTION_TEXT = q.QuestionText
					};
					entities.AddToINCIDENT_ANSWER(incidentAnswer);
				}

				// Check if Quality Issue (8D) question set to true
				if (q.QuestionId == (decimal)EHSQuestionId.Create8D && q.AnswerText == "Yes")
					shouldCreate8d = true;
			}
			entities.SaveChanges();

			return shouldCreate8d;
		}

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

				// AddUpdateContainment(context, incidentId)
				// AddUpdateContainmentDetails(context, incidentId);

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

		protected void UpdateTaskInfo(List<EHSIncidentQuestion> questions, decimal incidentId, DateTime createDate)
		{
			DateTime dueDate = DateTime.MinValue;
			decimal responsiblePersonId = 0;

			//foreach (var q in questions)
			//{
			//	var thisQuestion = q;
			//	var incidentAnswer = (from ia in entities.INCIDENT_ANSWER
			//						  where ia.INCIDENT_ID == incidentId
			//							  && ia.INCIDENT_QUESTION_ID == thisQuestion.QuestionId
			//						  select ia).FirstOrDefault();

			//	if (incidentAnswer != null && !String.IsNullOrEmpty(incidentAnswer.ANSWER_VALUE))
			//	{
			//		if (Mode == IncidentMode.Incident)
			//		{
			//			if (q.QuestionId == (decimal)EHSQuestionId.DateDue)
			//				dueDate = DateTime.Parse(incidentAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US"));
			//			else if (q.QuestionId == (decimal)EHSQuestionId.ResponsiblePersonDropdown) 
			//				responsiblePersonId = Convert.ToDecimal(incidentAnswer.ANSWER_VALUE);
			//		}
			//		else if (Mode == IncidentMode.Prevent)
			//		{
			//			if (q.QuestionId == (decimal)EHSQuestionId.ReportDate)
			//				dueDate = createDate.AddDays(30);  // mt - per TI the due date should be based on the incident CREATE date instead of the inspection date
			//				// dueDate = DateTime.Parse(incidentAnswer.ANSWER_VALUE, CultureInfo.GetCultureInfo("en-US")).AddDays(30);
			//			else if (q.QuestionId == (decimal)EHSQuestionId.AssignToPerson) 
			//				responsiblePersonId = Convert.ToDecimal(incidentAnswer.ANSWER_VALUE);
			//		}
			//	}
			//}
			
			if (dueDate > DateTime.MinValue && responsiblePersonId > 0)
			{
				int recordTypeId = (Mode == IncidentMode.Prevent) ? 45 : 40;
				EHSIncidentMgr.CreateOrUpdateTask(incidentId, responsiblePersonId, recordTypeId, dueDate);
			}
		}

		

		//protected void Create8dAndRedirect(decimal incidentId)
		//{
		//	SessionManager.ReturnStatus = true;

		//	PROB_CASE probCase = ProblemCase.LookupCaseByIncident(incidentId);
		//	if (probCase != null)
		//	{
		//		// If 8D problem case exists, redirect with problem case ID
		//		SessionManager.ReturnObject = probCase.PROBCASE_ID;
		//	}
		//	else
		//	{
		//		// Otherwise, redirect with the intent of creating a new problem case
		//		var entities = new PSsqmEntities();
		//		SessionManager.ReturnObject = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
		//	}

		//	Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
		//}


		protected String GetNextStepInfo(decimal currentStep, decimal incidentTypeId)
		{
			formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(incidentTypeId);

			string nextFormStepName = null;

			int i = Convert.ToInt32(currentStep);

			if (i < formSteps.Count())
			{
				nextFormStepName = formSteps[i].StepFormName;
				CurrentFormStep = formSteps[i].StepNumber;
			}

			return nextFormStepName;
		}


		protected void GoToNextStep(decimal incidentId)
		{
			// Go to next step (report)
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = incidentId;
		}


		#endregion



	
		//////////////////////////////////////////////////////////////////////
		// $$$$$$$$$$$$$$$$$$$  OLD PREVENTION LOCATION STUFF $$$$$$$$$$$$$$$$
		//////////////////////////////////////////////////////////////////////
	
		//protected void btnSubmit_Click(object sender, EventArgs e)
		//{
			//var verifications = new List<INCIDENT_VERIFICATION>();

			//if (tbDescription.Text.Trim().Length == 0 || !rdpDueDate.SelectedDate.HasValue)
			//{
			//	lblRequired.Visible = true;
			//	return;
			//}
			//lblRequired.Visible = false;

			//PSsqmEntities entities = new PSsqmEntities();

			//if (IsEditContext == true)
			//{
				//if (IncidentId != null)
				//{
				//	// Update description, date
				//	INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
				//	incident.DESCRIPTION = tbDescription.Text;
				//	if (rdpDueDate.SelectedDate.HasValue)
				//		incident.INCIDENT_DT = (DateTime)rdpDueDate.SelectedDate;
				//	if (!string.IsNullOrEmpty(rcbCases.SelectedValue))
				//		incident.VERIFY_PROBCASE_ID = Convert.ToDecimal(rcbCases.SelectedValue);
				//	entities.SaveChanges();

				// Add notified people and plants to database
				//foreach (GridViewRow gvr in gvPreventLocationsList.Rows)
				//{
				//	decimal plantId = (decimal)gvPreventLocationsList.DataKeys[gvr.RowIndex].Value;

				//	if (plantId != null)
				//	{
				//		RadGrid currentGridView = (RadGrid)gvr.FindControl("rgPlantContacts");
				//		foreach (GridDataItem item in currentGridView.Items)
				//		{
				//			decimal personId = (decimal)item.GetDataKeyValue("PERSON_ID");
				//			if (personId != null)
				//			{
				//				var incidentVerification = (from iv in entities.INCIDENT_VERIFICATION
				//											where iv.INCIDENT_ID == IncidentId &&
				//											iv.PLANT_ID == plantId &&
				//											iv.PERSON_ID == personId
				//											select iv).FirstOrDefault();

				//				if (item.Selected == true)
				//				{
				//					var newVerification = new INCIDENT_VERIFICATION()
				//					{
				//						INCIDENT_ID = IncidentId,
				//						PLANT_ID = plantId,
				//						PERSON_ID = personId,
				//						DATE_NOTIFIED = DateTime.Now
				//					};
				//					// Add to list to use for emails
				//					verifications.Add(newVerification);

				//					// Add to database if it does not exist
				//					if (incidentVerification == null)
				//					{
				//						entities.INCIDENT_VERIFICATION.AddObject(newVerification);
				//						entities.SaveChanges();
				//					}
				//				}
				//				else
				//				{
				//					// Delete if exists
				//					if (incidentVerification != null)
				//					{
				//						entities.INCIDENT_VERIFICATION.DeleteObject(incidentVerification);
				//						entities.SaveChanges();
				//					}
				//				}
				//			}
				//		}
				//	}
				//}
				//}
			//}
			//else // Is add context
			//{
				//decimal verifyProbcaseId = 0;
				//if (!string.IsNullOrEmpty(rcbCases.SelectedValue))
				//verifyProbcaseId = Convert.ToDecimal(rcbCases.SelectedValue);

				// Add incident to database
				//var incident = new INCIDENT()
				//{
				//	DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
				//	DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
				//	DETECT_PLANT_ID = SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID,
				//	INCIDENT_TYPE = "EHS",
				//	CREATE_DT = DateTime.Now,
				//	CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				//	DESCRIPTION = tbDescription.Text,
				//	CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				//	//INCIDENT_DT = rdpDueDate.SelectedDate.Value,
				//	ISSUE_TYPE = "Power Outage",
				//	ISSUE_TYPE_ID = 2,
				//	VERIFY_PROBCASE_ID = verifyProbcaseId
				//};
				//entities.INCIDENT.AddObject(incident);
				////entities.SaveChanges();
				//decimal incidentId = incident.INCIDENT_ID;


				// Add notified people and plants to database
				//foreach (GridViewRow gvr in gvPreventLocationsList.Rows)
				//{
				//	decimal plantId = (decimal)gvPreventLocationsList.DataKeys[gvr.RowIndex].Value;

				//	if (plantId != null)
				//	{
				//		RadGrid currentGridView = (RadGrid)gvr.FindControl("rgPlantContacts");
				//		foreach (GridDataItem item in currentGridView.Items)
				//		{
				//			decimal personId = (decimal)item.GetDataKeyValue("PERSON_ID");
				//			if (personId != null)
				//			{
				//				if (item.Selected == true)
				//				{
				//					var incidentVerification = new INCIDENT_VERIFICATION()
				//					{
				//						INCIDENT_ID = incidentId,
				//						PLANT_ID = plantId,
				//						PERSON_ID = personId,
				//						DATE_NOTIFIED = DateTime.Now
				//					};
				//					verifications.Add(incidentVerification);
				//					entities.INCIDENT_VERIFICATION.AddObject(incidentVerification);
				//					entities.SaveChanges();
				//				}
				//			}
				//		}
				//	}
				//}
			//}
			
				// Send email(s)

		//	foreach (var v in verifications)
		//	{
		//		var thisVerification = v;
		//		PERSON emailPerson = (from p in entities.PERSON where p.PERSON_ID == thisVerification.PERSON_ID select p).FirstOrDefault();

		//		string emailSubject = SessionManager.PrimaryCompany().COMPANY_NAME + " Issue Acknowledgement Notification"; // AW20140129 - use company name variable instead of hard coding.

		//		string path = "http://" + HttpContext.Current.Request.Url.Authority + "/EHS/EHS_Incident_Verification.aspx";

		//		path += string.Format("?inid={0}&plid={1}&peid={2}", v.INCIDENT_ID, v.PLANT_ID, emailPerson.PERSON_ID);
		//		var sb = new StringBuilder();
		//		sb.AppendLine("<p>You have been sent an issue acknowledgement notification from " + SessionManager.PrimaryCompany().COMPANY_NAME + ".</p>");
		//		sb.AppendLine();
		//		sb.AppendLine("<p><b>DETAILS</b></p>");
		//		sb.AppendLine();
		//		//sb.AppendLine("<p>Date: " + rdpDueDate.SelectedDate.Value.ToShortDateString() + "</p>");
		//		sb.AppendLine();
		//		sb.AppendLine("<p>Instructions: " + tbDescription.Text + "</p>");
		//		sb.AppendLine();
		//		sb.AppendLine("<p>Please go here to acknowledge receipt of this issue:<br/>");
		//		sb.AppendLine("<a href=\"" + path + "\">" + path + "</a></p>");
		//		sb.AppendLine(); // AW20140129
		//		sb.AppendLine(); // AW20140129
		//		sb.AppendLine("Please Do Not Reply To This Message"); // AW20140129

		//		string emailBody = sb.ToString();
		//		string emailAddress = emailPerson.EMAIL;
		//		WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");

		//	}

		//	Response.Redirect("EHS_Incidents.aspx");

		//}


		//protected void btnClose_Click(object sender, EventArgs e)
		//{
		//	PSsqmEntities entities = new PSsqmEntities();

		//	if (IncidentId != null)
		//	{
		//		INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
		//		if (incident.CLOSE_DATE == null || incident.CLOSE_DATE_DATA_COMPLETE == null)
		//		{
		//			incident.CLOSE_DATE = DateTime.Now;
		//			incident.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
		//		}
		//		else
		//		{
		//			incident.CLOSE_DATE = null; // Reopen
		//			incident.CLOSE_DATE_DATA_COMPLETE = null;
		//		}

		//		entities.SaveChanges();
		//	}

		//	Response.Redirect("EHS_Incidents.aspx");
		//}

		//public string Capitalize(string word)
		//{
		//	string output = "";
		//	if (word.Length > 0)
		//	{
		//		if (word.Length > 1)
		//			output = word[0].ToString().ToUpper() + word.Substring(1);
		//		else
		//			output = word.ToUpper();
		//	}
		//	return output;
		//}


	}
}