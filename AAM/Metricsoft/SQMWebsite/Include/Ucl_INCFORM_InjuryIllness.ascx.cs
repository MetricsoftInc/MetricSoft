﻿using System;
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
        #region Variables

        int maxINCIDENT = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncident"]);
        int maxIncidentforInjuryType = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncidentIDforInjuryType"]);
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

        protected string localDescription;
        //protected string productImpact;
        protected string selectedShift;
        protected TimeSpan incidentTime;


        // Special answers used in INCIDENT table
        string incidentDescription = "";
        protected DateTime incidentDate;

        PSsqmEntities entities;
        List<EHSIncidentQuestion> questions;


        //public bool IsEditContext { get; set; }
        public decimal IncidentId { get; set; }
        public decimal theincidentId { get; set; }
        public RadGrid EditControlGrid { get; set; }

        public PageUseMode PageMode { get; set; }

        protected static List<INCFORM_TYPE_CONTROL> incidentStepList;

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

        protected string IncidentLocationTZ
        {
            get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
            set { ViewState["IncidentLocationTZ"] = value; }
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

        protected List<SETTINGS> EHSSettings
        {
            get { return ViewState["EHSSettings"] == null ? SQMSettings.SelectSettingsGroup("EHS", "") : (List<SETTINGS>)ViewState["EHSSettings"]; }
            set { ViewState["EHSSettings"] = value; }
        }

        protected List<XLAT> XLATList
        {
            get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
            set { ViewState["XLATList"] = value; }
        }


        protected override void OnInit(EventArgs e)
        {
            uclRecordableHist.LostTimeUpdateEvent += LostTimeUpdate;
            base.OnInit(e);
        }
        #endregion
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
                else
                {
                    incidentStepList = EHSIncidentMgr.SelectIncidentSteps(entities, -1m);
                    XLATList = SQMBasePage.SelectXLATList(new string[1] { "INCIDENT_STEP" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
                }

                IncidentLocationId = SessionManager.IncidentLocation.Plant.PLANT_ID;
                IncidentLocationTZ = SessionManager.IncidentLocation.Plant.LOCAL_TIMEZONE;

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
                String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
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
                IncidentLocationTZ = SessionManager.IncidentLocation.Plant.LOCAL_TIMEZONE;
                SelectedTypeId = Convert.ToDecimal(newTypeID);
                SelectedTypeText = EHSIncidentMgr.SelectIncidentType(newTypeID, SessionManager.UserContext.Language.NLS_LANGUAGE).TITLE;
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
            PageMode = PageUseMode.EditEnabled;
            PopulateInitialForm();
            SetSubnav("edit");
        }

        public void BindIncidentAlert(decimal incidentID)
        {
            IsEditContext = true;
            EditIncidentId = incidentID;
            IncidentStepCompleted = 0;
            PageMode = PageUseMode.ViewOnly;
            PopulateInitialForm();
            SetSubnav("alert");
        }

        public void PopulateInitialForm()
        {

            entities = new PSsqmEntities();
            decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;
            INCIDENT incident = null;

            string psersonSelect = EHSSettings.Where(s => s.SETTING_CD == "PERSONINPUT").FirstOrDefault() == null ? "" : EHSSettings.Where(s => s.SETTING_CD == "PERSONINPUT").FirstOrDefault().VALUE;
            string deptSelect = EHSSettings.Where(s => s.SETTING_CD == "DEPTINPUT").FirstOrDefault() == null ? "" : EHSSettings.Where(s => s.SETTING_CD == "DEPTINPUT").FirstOrDefault().VALUE;
            string addFields = EHSSettings.Where(s => s.SETTING_CD == "INCIDENT_ADD_FIELDS").FirstOrDefault() == null ? "" : EHSSettings.Where(s => s.SETTING_CD == "INCIDENT_ADD_FIELDS").FirstOrDefault().VALUE;

            if (EditIncidentId > maxINCIDENT || EditIncidentId == 0)
            {
                belowMAX.Visible = false;
                aboveMAX.Visible = true;
                divJobTenure.Visible = false;
                divEmploymentTenure.Visible = false;
                divAssociateDate.Visible = true;
                divHireDate.Visible = true;
            }
            else
            {
                belowMAX.Visible = true;
                aboveMAX.Visible = false;
                divJobTenure.Visible = true;
                //divEmploymentTenure.Visible = true;
                if (addFields.Contains("employment"))
                {
                    divEmploymentTenure.Visible = true;
                }
                divAssociateDate.Visible = false;
                divHireDate.Visible = false;
            }


            if (EditIncidentId > maxIncidentforInjuryType || EditIncidentId == 0)
            {

                PopulateNewInjuryTypeDropDown();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text1", "javascript:TNSKRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test2", "javascript:EquipmentManufacturerNameRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test3", "javascript:EquipmentManufacturerDateRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test4", "javascript:DesignNumberRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test5", "javascript:AssetNumberRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test6", "javascript:AgeOfAssociateRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test7", "javascript:TypeOfIncidentRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test8", "javascript:InitialTreatmentGivenRow();", true);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "test9", "javascript:ChangeInMedicalStatusRow();", true);

                RadAjaxPanel1.Visible = false;


                RTXT_TNSK.Visible = false;
                lblTNSKSM.Visible = false;
                lblTNSKXS.Visible = false;
                // lblTNSKSM.Style.Add("display", "none");
                // lblTNSKXS.Style.Add("display", "none");
                lblBusinessTypeSM.Style.Add("display", "none");
                lblSpecificProcessTypeSM.Style.Add("display", "none");
                lblMacroProcessTypeSM.Style.Add("display", "none");
                RDDL_BusinessType.Style.Add("display", "none");
                RDDL_MacroProcessType.Style.Add("display", "none");
                RDDL_SpecificProcessType.Style.Add("display", "none");

                lblEquipmentManufacturerNameSM.Style.Add("display", "none");
                lblEquipmentManufacturerDateSM.Style.Add("display", "none");
                lblDesignNumberSM.Style.Add("display", "none");
                lblAssetNumberSM.Style.Add("display", "none");

                TXT_EquipmentManufacturerName.Style.Add("display", "none");
                RDP_EquipmentManufacturerDate.Style.Add("display", "none");
                RTXT_DesignNumber.Style.Add("display", "none");
                RTXT_AssetNumber.Style.Add("display", "none");

                lblAgeOfAssociateSM.Style.Add("display", "none");
                RtxtAgeOfAssociate.Style.Add("display", "none");
                lblTypeOfIncidentSM.Style.Add("display", "none");


                RDDL_TypeOfIncident.Style.Add("display", "none");
                lblInitialTreatmentGivenSM.Style.Add("display", "none");
                CBL_InitialTreatmentGiven.Style.Add("display", "none");
                lblChangeInMedicalStatusSM.Style.Add("display", "none");
                CBL_ChangeInMedicalStatus.Style.Add("display", "none");


                PopulateInjuryTypeDropDown();
            }


            if (deptSelect.ToLower() == "text")
            {
                tbDepartment.Visible = true;
                rddlDeptTest.Visible = false;
            }
            if (psersonSelect.ToLower() == "text")
            {
                rajxInvolvedPerson.Visible = false;
                pnlInvolvedPerson.Visible = true;
            }

            if (addFields.Contains("jobcode"))
            {
                divJobCode.Visible = true;
            }
            if (addFields.Contains("procs"))
            {
                divProcedures.Visible = true;
            }

            if (IsEditContext == true)
            {
                incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
                SelectedTypeId = (decimal)incident.ISSUE_TYPE_ID;
                SelectedTypeText = EHSIncidentMgr.SelectIncidentType((decimal)incident.ISSUE_TYPE_ID, SessionManager.UserContext.Language.NLS_LANGUAGE).TITLE;
                CreatePersonId = (decimal)incident.CREATE_PERSON;
                IncidentStepCompleted = incident.INCFORM_LAST_STEP_COMPLETED;

                var injuryIllnessDetails = EHSIncidentMgr.SelectInjuryIllnessDetailsById(entities, EditIncidentId);

                if (incident != null)
                {
                    string lang;
                    lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
                    //if ((lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()) != "en")
                        pnlLocalDesc.Visible = true;

                    IncidentLocationId = Convert.ToDecimal(incident.DETECT_PLANT_ID);
                    IncidentLocationTZ = SQMModelMgr.LookupPlant(entities, (decimal)incident.DETECT_PLANT_ID, "").LOCAL_TIMEZONE;

                    rdpIncidentDate = SQMBasePage.SetRadDateCulture(rdpIncidentDate, "");
                    rdpReportDate = SQMBasePage.SetRadDateCulture(rdpReportDate, "");

                    tbDescription.Text = incident.DESCRIPTION;
                    rdpIncidentDate.SelectedDate = incidentDate = incident.INCIDENT_DT;


                    if (incident.TNSKNumber == null)
                        RTXT_TNSK.Text = string.Empty;
                    else
                        RTXT_TNSK.Text = incident.TNSKNumber;




                    rdpReportDate.SelectedDate = incident.CREATE_DT;

                    PopulateDepartmentDropDown((decimal)incident.DETECT_PLANT_ID);
                    PopulateRDDL_BusinessType();
                    PopulateJobcodeDropDown();
                    if (EditIncidentId <= maxINCIDENT)
                    {
                        PopulateJobTenureDropDown();
                    }
                    PopulateShiftDropDown();
                    if (EditIncidentId > maxIncidentforInjuryType || EditIncidentId == 0)
                    {
                        PopulateNewInjuryTypeDropDown();
                    }
                    else
                    {
                        PopulateInjuryTypeDropDown();
                    }
                    PopulateBodyPartDropDown();
                    PopulateTypeOfIncident();

                    if (injuryIllnessDetails != null)
                    {

                        rtpIncidentTime.SelectedTime = injuryIllnessDetails.INCIDENT_TIME;
                        rddlShiftID.SelectedValue = injuryIllnessDetails.SHIFT;

                        if (injuryIllnessDetails.DESCRIPTION_LOCAL != null)
                            tbLocalDescription.Text = injuryIllnessDetails.DESCRIPTION_LOCAL;

                        if (!string.IsNullOrEmpty(injuryIllnessDetails.DEPARTMENT))
                        {
                            tbDepartment.Text = injuryIllnessDetails.DEPARTMENT;
                        }

                        //Involved Person :
                        PERSON invp = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.INVOLVED_PERSON_ID select p).FirstOrDefault();
                        string involvedPerson = (invp != null) ? string.Format("{0}, {1}", invp.LAST_NAME, invp.FIRST_NAME) : "";
                        if (!String.IsNullOrEmpty(involvedPerson))
                        {
                            rsbInvolvedPerson.Text = involvedPerson;
                            lbSupervisorLabel.Visible = true;
                        }

                        //if (!string.IsNullOrEmpty(injuryIllnessDetails.INVOLVED_PERSON_NAME))
                        //{
                            tbInvolvedPerson.Text = injuryIllnessDetails.INVOLVED_PERSON_NAME;
                        //}

                        if (rddlDeptTest.FindItemByValue(injuryIllnessDetails.DEPT_ID.ToString()) != null)
                            rddlDeptTest.SelectedValue = injuryIllnessDetails.DEPT_ID.ToString();
                        tbInvPersonStatement.Text = injuryIllnessDetails.INVOLVED_PERSON_STATEMENT;
                        rdpSupvInformedDate.SelectedDate = injuryIllnessDetails.SUPERVISOR_INFORMED_DT;

                        PERSON supv = (PERSON)(from p in entities.PERSON where p.PERSON_ID == injuryIllnessDetails.SUPERVISOR_PERSON_ID select p).FirstOrDefault();
                        lbSupervisor.Text = (supv != null) ? string.Format("{0}, {1}", supv.LAST_NAME, supv.FIRST_NAME) : "[ supervisor not found ]";

                        rdoInside.SelectedValue = (!string.IsNullOrEmpty(injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG) && injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG.ToUpper() == "INSIDE BUILDING") ? "1" : "0";

                        tbSupervisorStatement.Text = injuryIllnessDetails.SUPERVISOR_STATEMENT;

                        //check for null value for emp_status.
                        if (injuryIllnessDetails.EMP_STATUS != null)
                        {
                            if (EditIncidentId > maxINCIDENT)
                            {
                                rdoEmpStatus.SelectedValue = Convert.ToString(injuryIllnessDetails.EMP_STATUS);
                            }
                            else
                            {
                                rdoDirectSupv.SelectedValue = (injuryIllnessDetails.COMPANY_SUPERVISED == true) ? "1" : "0";
                            }
                        }
                        rdoErgConcern.SelectedValue = (injuryIllnessDetails.ERGONOMIC_CONCERN == true) ? "1" : "0"; ;
                        rdoStdProcsFollowed.SelectedValue = (injuryIllnessDetails.STD_PROCS_FOLLOWED == true) ? "1" : "0";
                        rdoTrainingProvided.SelectedValue = (injuryIllnessDetails.TRAINING_PROVIDED == true) ? "1" : "0";
                        if (EditIncidentId > maxINCIDENT)
                        {
                            if (injuryIllnessDetails.ASSOCIATE_YEAR != null)
                            {
                                int AssociateMonth = Convert.ToInt32(injuryIllnessDetails.ASSOCIATE_MONTHS);
                                int AssociateYear = Convert.ToInt32(injuryIllnessDetails.ASSOCIATE_YEAR);
                                radAssociateSelect.SelectedDate = Convert.ToDateTime(AssociateMonth + "/1/" + AssociateYear);
                            }
                            if (injuryIllnessDetails.HIRE_YEAR != null)
                            {
                                int HireMonth = Convert.ToInt32(injuryIllnessDetails.HIRE_MONTHS);
                                int HireYear = Convert.ToInt32(injuryIllnessDetails.HIRE_YEAR);
                                radHireSelect.SelectedDate = Convert.ToDateTime(HireMonth + "/1/" + HireYear);
                            }
                        }
                        else
                        {
                            if (rddlJobTenure.FindItemByValue(injuryIllnessDetails.JOB_TENURE) != null)
                                rddlJobTenure.SelectedValue = injuryIllnessDetails.JOB_TENURE;
                            if (rddlEmploymentTenure.FindItemByValue(injuryIllnessDetails.EMPLOYMENT_TENURE) != null)
                                rddlEmploymentTenure.SelectedValue = injuryIllnessDetails.EMPLOYMENT_TENURE;
                        }

                        RDDL_BusinessType.SelectedValue = injuryIllnessDetails.BUSINESS_TYPE;

                        PopulateMacroProcessType(injuryIllnessDetails.BUSINESS_TYPE);


                        RDDL_MacroProcessType.SelectedValue = injuryIllnessDetails.MACRO_PROCESS_TYPE;

                        PopulateSpecificProcessType(injuryIllnessDetails.MACRO_PROCESS_TYPE);


                        RDDL_SpecificProcessType.SelectedValue = injuryIllnessDetails.SPECIFIC_PROCESS_TYPE;



                        TXT_EquipmentManufacturerName.Text = injuryIllnessDetails.EQUIPMENT_MANUFACTURER_NAME;
                        RDP_EquipmentManufacturerDate.SelectedDate = injuryIllnessDetails.EQUIPEMENT_MANUFACTURER_DATE;
                        RTXT_DesignNumber.Text = injuryIllnessDetails.DESIGN_NUMBER;
                        RTXT_AssetNumber.Text = injuryIllnessDetails.ASSET_NUMBER;
                        RtxtAgeOfAssociate.Text = injuryIllnessDetails.AGE_OF_ASSOCIATE;
                        RDDL_TypeOfIncident.SelectedValue = injuryIllnessDetails.TYPE_OF_INCIDENT;

                        if (!string.IsNullOrEmpty(injuryIllnessDetails.INITIAL_TREATMENT_GIVEN) && injuryIllnessDetails.INITIAL_TREATMENT_GIVEN.Contains(','))
                        {
                            var data = injuryIllnessDetails.INITIAL_TREATMENT_GIVEN.Split(',');
                            foreach (var item in data)
                            {
                                CBL_InitialTreatmentGiven.Items.FindByValue(item).Selected = true;

                            }
                        }
                    
                        else if(!string.IsNullOrEmpty(injuryIllnessDetails.INITIAL_TREATMENT_GIVEN))
                        {
                            CBL_InitialTreatmentGiven.SelectedValue = injuryIllnessDetails.INITIAL_TREATMENT_GIVEN;
                        }

                        // = injuryIllnessDetails.INITIAL_TREATMENT_GIVEN;
                        if (!string.IsNullOrEmpty(injuryIllnessDetails.CHANGE_MEDICAL_STATUS) && injuryIllnessDetails.CHANGE_MEDICAL_STATUS.Contains(','))
                        {
                            var data = injuryIllnessDetails.CHANGE_MEDICAL_STATUS.Split(',');
                            foreach (var item in data)
                            {
                                CBL_ChangeInMedicalStatus.Items.FindByValue(item).Selected = true;
                            }
                        }
                     
                        else if(!string.IsNullOrEmpty(injuryIllnessDetails.CHANGE_MEDICAL_STATUS))
                        {
                            CBL_ChangeInMedicalStatus.SelectedValue = injuryIllnessDetails.CHANGE_MEDICAL_STATUS;
                        }

                        // CBL_ChangeInMedicalStatus.SelectedValue = injuryIllnessDetails.CHANGE_MEDICAL_STATUS;

                        rdoFirstAid.SelectedValue = (injuryIllnessDetails.FIRST_AID == true) ? "1" : "0";
                        rdoRecordable.SelectedValue = (injuryIllnessDetails.RECORDABLE == true) ? "1" : "0";
                        rdoFatality.SelectedValue = (injuryIllnessDetails.FATALITY == true) ? "1" : "0";

                        rddlInjuryType.SelectedValue = injuryIllnessDetails.INJURY_TYPE;
                        rddlBodyPart.SelectedValue = injuryIllnessDetails.INJURY_BODY_PART;
                        rdoReoccur.SelectedValue = (injuryIllnessDetails.REOCCUR == true) ? "1" : "0";




                        if (divJobCode.Visible == true)
                        {
                            if (rddlJobCode.FindItemByValue(injuryIllnessDetails.JOBCODE_CD) != null)
                            {
                                rddlJobCode.SelectedValue = injuryIllnessDetails.JOBCODE_CD;
                            }
                        }

                        if (divProcedures.Visible == true)
                        {
                            tbProcedures.Text = injuryIllnessDetails.STD_PROCS_DESC;
                        }

                        SetSeverityControls(injuryIllnessDetails);
                        RecordableHist(injuryIllnessDetails.RECORDABLE == true ? true : false);
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
                    rdoEmpStatus.SelectedValue = "";

                    rdoErgConcern.SelectedValue = "";
                    rdoStdProcsFollowed.SelectedValue = "";
                    rdoTrainingProvided.SelectedValue = "";
                    rdoFirstAid.SelectedValue = "";
                    rdoRecordable.SelectedValue = "";
                    cbLostTime.Checked = cbRestrictedTime.Checked = false;
                    rddlJobTenure.Items.Clear();
                    rddlEmploymentTenure.Items.Clear();
                    radAssociateSelect.SelectedDate = null;
                    radHireSelect.SelectedDate = null;
                    rddlInjuryType.Items.Clear();
                    rddlBodyPart.Items.Clear();
                    lbSupervisorLabel.Visible = false;

                    rdoFirstAid.SelectedValue = "1";
                    Severity_Changed(rdoFirstAid, null);

                    string lang;
                    lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() ;
                    //if ((lang = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString()) != "en")
                        pnlLocalDesc.Visible = true;

                    rdpIncidentDate = SQMBasePage.SetRadDateCulture(rdpIncidentDate, "");
                    rdpReportDate = SQMBasePage.SetRadDateCulture(rdpReportDate, "");

                    rdpIncidentDate.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
                    //rdpReportDate.Culture = rdpIncidentDate.Culture;
                    rdpReportDate.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
                    //rtpIncidentTime.Culture = rdpIncidentDate.Culture;

                    PopulateJobcodeDropDown();
                    //PopulateJobTenureDropDown();
                    PopulateRDDL_BusinessType();
                    PopulateShiftDropDown();
                    PopulateTypeOfIncident();

                    PopulateDepartmentDropDown(IncidentLocationId);

                    if (EditIncidentId > maxIncidentforInjuryType || EditIncidentId == 0)
                    {
                        PopulateNewInjuryTypeDropDown();
                    }
                    else
                    {
                        PopulateInjuryTypeDropDown();
                    }
                    PopulateBodyPartDropDown();
                    GetAttachments(0);

                    RecordableHist(false);
                }
            }

            CurrentStep = (int)EHSFormId.INCFORM_INJURYILLNESS;
            InitializeForm(CurrentStep);

            RefreshPageContext();

            if (PageMode == PageUseMode.ViewOnly)
            {
                pnlBaseForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = false;
            }
            else
            {
                bool isClosed = EHSIncidentMgr.IsIncidentClosed(incident);
                bool canUPdate = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.originate, IncidentStepCompleted, false);
                if (isClosed)
                {
                    if (canUPdate)
                    {
                        SQMBasePage.DisableControls(divBaseForm, new string[1] { "Telerik.Web.UI.RadAjaxPanel" }, new string[2] { "uclRecordableHist", "divRecordableHist" });
                        SQMBasePage.DisableControls(rapAttach, new string[] { }, new string[] { });
                    }
                    else
                        pnlBaseForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = false;
                }
                else
                {
                    pnlBaseForm.Enabled = btnSubnavSave.Visible = btnSubnavSave.Enabled = canUPdate;
                }
            }
        }



        void InitializeForm(int currentStep)
        {

            entities = new PSsqmEntities();

            IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
            decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

            pnlBaseForm.Visible = true;
            uclroot5y.Visible = false;
            uclCausation.Visible = false;
            uclcontain.Visible = false;
            uclaction.Visible = false;
            uclapproval.Visible = false;
            lblFormTitle.Text = Resources.LocalizedText.Incident;
            btnSubnavIncident.Enabled = false;
            btnSubnavIncident.CssClass = "buttonLinkDisabled";
            rptWitness.DataSource = EHSIncidentMgr.GetWitnessList(Math.Max(IncidentId, EditIncidentId));
            rptWitness.DataBind();

        }


        protected void RefreshPageContext()
        {
            string typeString = Resources.LocalizedText.Incident;

            if (!IsEditContext)
            {
                lblAddOrEditIncident.Text = "New" + "&nbsp" + typeString;
                lblIncidentType.Text = SelectedTypeText;
                lblIncidentLocation.Text = SessionManager.IncidentLocation.Plant.PLANT_NAME;
            }
            else
            {

                lblAddOrEditIncident.Text = typeString + "&nbsp" + WebSiteCommon.FormatID(EditIncidentId, 6);
                lblIncidentType.Text = SelectedTypeText;
                lblIncidentLocation.Text = EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(EditIncidentId);
            }
        }

        protected void Severity_Changed(object sender, EventArgs e)
        {
            string rblId = "";
            RadioButtonList rbl = null;
            if (sender != null)
            {
                rbl = (RadioButtonList)sender;
                rblId = rbl.ID;
            }

            lblIncidentMsg.Visible = false;

            switch (rblId)
            {
                case "rdoFirstAid":
                    if (rbl.SelectedValue == "1")
                    {
                        if (uclRecordableHist.CheckForkWorkStatus("01") > 0 || uclRecordableHist.CheckForkWorkStatus("03") > 0)
                        {
                            rdoFirstAid.SelectedValue = "0";
                            lblIncidentMsg.Visible = true;
                            lblIncidentMsg.Text = Resources.LocalizedText.RecordableIncidentMsg;
                            return;
                        }

                        rdoRecordable.Enabled = rdoFatality.Enabled = false;
                        rdoRecordable.SelectedValue = "0";
                        rdoFatality.SelectedValue = "0";
                        cbLostTime.Checked = cbRestrictedTime.Checked = false;
                    }
                    else
                    {
                        rdoRecordable.Enabled = true;
                        rdoRecordable.SelectedValue = "1";
                        RecordableHist(true);
                        rdoFatality.Enabled = true;
                    }
                    break;
                case "rdoRecordable":
                    if (rbl.SelectedValue == "1")
                    {
                        rdoFatality.Enabled = true;
                        RecordableHist(true);
                    }
                    else
                    {
                        if (uclRecordableHist.CheckForkWorkStatus("01") > 0 || uclRecordableHist.CheckForkWorkStatus("03") > 0)
                        {
                            rdoRecordable.SelectedValue = "1";
                            lblIncidentMsg.Visible = true;
                            lblIncidentMsg.Text = lblIncidentMsg.Text = Resources.LocalizedText.RecordableIncidentMsg;
                            return;
                        }
                        rdoFirstAid.Enabled = true;
                        rdoFirstAid.SelectedValue = "1";
                        rdoFatality.Enabled = false;
                        rdoFatality.SelectedValue = "0";
                        cbLostTime.Checked = cbRestrictedTime.Checked = false;
                        RecordableHist(false);
                    }
                    break;
                case "rdoFatality":
                    break;
                default:
                    break;
            }

        }

        protected void SetSeverityControls(INCFORM_INJURYILLNESS incformDetails)
        {
            if (incformDetails.FIRST_AID == true)
            {
                rdoFirstAid.SelectedValue = "1";
                rdoFirstAid.Enabled = true;
                rdoRecordable.SelectedValue = rdoFatality.SelectedValue = "0";
                rdoRecordable.Enabled = rdoFatality.Enabled = false;
                cbLostTime.Checked = cbRestrictedTime.Checked = false;
            }
            else
            {
                rdoFirstAid.SelectedValue = "0";
                if (incformDetails.RECORDABLE == true)
                {
                    rdoFirstAid.Enabled = false;
                    rdoRecordable.SelectedValue = "1";
                    rdoRecordable.Enabled = rdoFatality.Enabled = true;
                    if (incformDetails.LOST_TIME == true)
                        cbLostTime.Checked = true;
                    if (incformDetails.RESTRICTED_TIME == true)
                        cbRestrictedTime.Checked = true;
                }
                else
                {
                    rdoFirstAid.Enabled = true;
                    rdoFatality.Enabled = false;
                }
            }
        }

        protected void LostTimeUpdate(string cmd)
        {
            if (uclRecordableHist.CheckForkWorkStatus("01") > 0)
                cbRestrictedTime.Checked = true;
            else
                cbRestrictedTime.Checked = false;

            if (uclRecordableHist.CheckForkWorkStatus("03") > 0)
                cbLostTime.Checked = true;
            else
                cbLostTime.Checked = false;

            hfChangeUpdate.Value = "1";
        }

        void RecordableHist(bool enabled)
        {
            divRecordableHist.Visible = enabled;
            uclRecordableHist.Visible = enabled;
            if (enabled)
            {
                uclRecordableHist.IsEditContext = true;
                uclRecordableHist.IncidentId = EditIncidentId;
                uclRecordableHist.PopulateInitialForm("embeded");
            }
        }

        private INCFORM_INJURYILLNESS SetLostTime(INCFORM_INJURYILLNESS injuryIllnessDetails)
        {
            injuryIllnessDetails.LOST_TIME = injuryIllnessDetails.RESTRICTED_TIME = false;

            if (uclRecordableHist.CheckForkWorkStatus("03") > 0)
            {
                injuryIllnessDetails.LOST_TIME = true;
            }

            if (uclRecordableHist.CheckForkWorkStatus("01") > 0)
            {
                injuryIllnessDetails.RESTRICTED_TIME = true;
            }

            return injuryIllnessDetails;
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

        void PopulateJobcodeDropDown()
        {
            rddlJobCode.DataValueField = "JOBCODE_CD";
            rddlJobCode.DataTextField = "JOB_DESC";
            rddlJobCode.DataSource = SQMModelMgr.SelectJobcodeList("A", "").OrderBy(l => l.JOB_DESC).ToList();
            rddlJobCode.DataBind();
            rddlJobCode.Items.Insert(0, new DropDownListItem("", ""));
        }

        void PopulateJobTenureDropDown()
        {
            bool categorize = EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault() != null && EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault().VALUE.ToUpper() == "Y" ? true : false;
            List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[2] { "INJURY_TENURE", "EMPLOYMENT_TENURE" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);

            SQMBasePage.SetCategorizedDropDownItems(rddlJobTenure, xlatList.Where(l => l.XLAT_GROUP == "INJURY_TENURE").ToList(), categorize);

            SQMBasePage.SetCategorizedDropDownItems(rddlEmploymentTenure, xlatList.Where(l => l.XLAT_GROUP == "EMPLOYMENT_TENURE").ToList(), categorize);
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
            rddlInjuryType.Items.Clear();

            List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE").OrderBy(P=>P.SortOrder).ToList();

            injtype = injtype.Where(x => !x.Value.Contains("IT_NEW_")).ToList();


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

        void PopulateNewInjuryTypeDropDown()
        {
            rddlInjuryType.Items.Clear();
            List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE", "IT_NEW_").OrderBy(P => P.SortOrder).ToList();
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
            bool categorize = EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault() != null && EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault().VALUE.ToUpper() == "Y" ? true : false;
            List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[1] { "INJURY_PART" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
            SQMBasePage.SetCategorizedDropDownItems(rddlBodyPart, xlatList.Where(l => l.XLAT_GROUP == "INJURY_PART").ToList(), categorize);
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
            uploader.SetReportOption(false);
            uploader.SetDescription(false);
            // Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
            uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete", "btnDeleteInc", "btnSubnavIncident", "btnSubnavContainment", "btnSubnavRootCause", "btnSubnavAction", "btnSubnavApproval" };

            int attCnt = EHSIncidentMgr.AttachmentCount(incidentId);
            int px = 128;

            if (attCnt > 0)
            {
                px = px + (attCnt * 30) + 35;
                uploader.GetUploadedFilesIncidentSection(40, incidentId, "", 0);//values 0 denotes for incident section.
            }

            /*

			*/
            // Set the html Div height based on number of attachments to be displayed in the grid:
            //dvAttachLbl.Style.Add("height", px.ToString() + "px !important");
            //dvAttach.Style.Add("height", px.ToString() + "px !important");
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

                    if (witness.WITNESS_PERSON.HasValue)
                    {
                        PERSON prsn = (from p in entities.PERSON where p.PERSON_ID == witness.WITNESS_PERSON select p).FirstOrDefault();
                        if (prsn != null)
                            rsbw.Text = string.Format("{0}-{1}, {2}", Convert.ToString(prsn.PERSON_ID), prsn.LAST_NAME, prsn.FIRST_NAME);
                    }

                    if (pnlInvolvedPerson.Visible == true)
                    {
                        RadAjaxPanel ajx = (RadAjaxPanel)e.Item.FindControl("rajxWitness");
                        ajx.Visible = false;
                        Panel pnl = (Panel)e.Item.FindControl("pnlWitness");
                        pnl.Visible = true;
                        TextBox tbwit = (TextBox)e.Item.FindControl("tbWitness");
                        tbwit.Text = witness.WITNESS_NAME;
                    }

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

                        if (pnlInvolvedPerson.Visible == true)
                        {
                            RadAjaxPanel ajx = (RadAjaxPanel)witnessitem.FindControl("rajxWitness");
                            ajx.Visible = false;
                            Panel pnl = (Panel)witnessitem.FindControl("pnlWitness");
                            pnl.Visible = true;
                            TextBox tbwit = (TextBox)witnessitem.FindControl("tbWitness");
                            item.WITNESS_NAME = tbwit.Text.Trim();
                        }

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

                        if (pnlInvolvedPerson.Visible == true)
                        {
                            RadAjaxPanel ajx = (RadAjaxPanel)witnessitem.FindControl("rajxWitness");
                            ajx.Visible = false;
                            Panel pnl = (Panel)witnessitem.FindControl("pnlWitness");
                            pnl.Visible = true;
                            TextBox tbwit = (TextBox)witnessitem.FindControl("tbWitness");
                            item.WITNESS_NAME = tbwit.Text.Trim();
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
                btnSubnavContainment.Visible = false;
                btnSubnavRootCause.Visible = false;
                btnSubnavCausation.Visible = false;
                btnSubnavAction.Visible = false;
                btnSubnavApproval.Visible = false;

                btnDeleteInc.Visible = false;
                lblResults.Visible = true;
                int delStatus = EHSIncidentMgr.DeleteIncident(EditIncidentId);
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
                uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = uclVideoPanel.Visible = false;
                btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavAlert.Visible=btnSubnavCEOComment.Visible = btnSubnavVideo.Visible = false;
                btnSubnavInitialActionApproval.Visible = btnSubnavCorrectiveActionApproval.Visible = false;
                btnDeleteInc.Visible = false;
                uploader.SetViewMode(true);
            }
            else if (context == "alert")
            {
                btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavAlert.Visible=btnSubnavCEOComment.Visible = btnSubnavVideo.Visible = false;
                btnSubnavInitialActionApproval.Visible = btnSubnavCorrectiveActionApproval.Visible = false;
                uclcontain.Visible = uclroot5y.Visible = uclCausation.Visible = uclaction.Visible = uclapproval.Visible = true;
                uploader.SetViewMode(false);
                uploader.SetReportOption(false);

                uclcontain.IsEditContext = true;
                uclcontain.IncidentId = EditIncidentId;
                uclcontain.PageMode = PageUseMode.ViewOnly;
                uclcontain.PopulateInitialForm();

                uclroot5y.IsEditContext = true;
                uclroot5y.IncidentId = EditIncidentId;
                uclroot5y.PageMode = PageUseMode.ViewOnly;
                uclroot5y.PopulateInitialForm();

                uclCausation.IsEditContext = true;
                uclCausation.IncidentId = EditIncidentId;
                uclCausation.PageMode = PageUseMode.ViewOnly;
                uclCausation.PopulateInitialForm(entities);

                uclaction.IsEditContext = true;
                uclaction.IncidentId = EditIncidentId;
                uclaction.PageMode = PageUseMode.ViewOnly;
                uclaction.PopulateInitialForm();

                uclapproval.IsEditContext = true;
                uclapproval.IncidentId = EditIncidentId;
                uclapproval.PageMode = PageUseMode.ViewOnly;
                uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness && s.STEP == 10.0m).FirstOrDefault());
            }
            else
            {
                uclcontain.Visible = uclroot5y.Visible = uclaction.Visible = uclapproval.Visible = uclVideoPanel.Visible = false;
                btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = btnSubnavVideo.Visible = true;
                btnSubnavIncident.Visible = true;
                btnSubnavIncident.Enabled = false;
                btnSubnavIncident.CssClass = "buttonLinkDisabled";
                btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
                uploader.SetViewMode(btnSubnavSave.Enabled);
                uploader.SetReportOption(false);
                btnDeleteInc.Visible = EHSIncidentMgr.CanDeleteIncident(CreatePersonId, IncidentStepCompleted);

                btnSubnavVideo.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 1.1m);
                btnSubnavVideo.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavVideo.Text;
                btnSubnavCEOComment.Visible= btnSubnavAlert.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 11.0m);
                btnSubnavCEOComment.Visible= btnSubnavAlert.Visible = true;
                //btnSubnavAlert.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "AttachVideo").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "PreventativeMeasure").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavAlert.Text;
                btnSubnavAlert.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "PreventativeMeasure").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "PreventativeMeasure").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavAlert.Text;

                btnSubnavInitialActionApproval.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 2.5m);
                btnSubnavInitialActionApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "InitialActionApproval").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "InitialActionApproval").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavInitialActionApproval.Text;
                btnSubnavCorrectiveActionApproval.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 5.5m);
                btnSubnavCorrectiveActionApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "CorrectiveActionApproval").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "CorrectiveActionApproval").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavCorrectiveActionApproval.Text;

                btnSubnavApproval.Text = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "Approvals").Count() > 0 ? XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_STEP" && x.XLAT_CODE == "Approvals").FirstOrDefault().DESCRIPTION_SHORT : btnSubnavApproval.Text;
            }
        }

        protected void btnSubnavSave_Click(object sender, EventArgs e)
        {
            int status = 0;
            bool isEdit = IsEditContext;

            btnSubnavIncident.Visible = btnSubnavApproval.Visible = btnSubnavAction.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavContainment.Visible = btnSubnavVideo.Visible = true;

            decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

            switch (CurrentSubnav)
            {
                case "2":
                    if ((status = uclcontain.AddUpdateINCFORM_CONTAIN(incidentId)) >= 0)
                        btnSubnav_Click(btnSubnavContainment, null);
                    break;
                case "2.5":
                    if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
                        btnSubnav_Click(btnSubnavInitialActionApproval, null);
                    break;
                case "3":
                    if ((status = uclroot5y.AddUpdateINCFORM_ROOT5Y(incidentId)) >= 0)
                        btnSubnav_Click(btnSubnavRootCause, null);
                    break;
                case "4":
                    if ((status = uclCausation.UpdateCausation(EditIncidentId)) >= 0)
                        btnSubnav_Click(btnSubnavCausation, null);
                    break;
                case "5":
                    if ((status = uclaction.AddUpdateINCFORM_ACTION(incidentId)) >= 0)
                        btnSubnav_Click(btnSubnavAction, null);
                    break;
                case "5.5":
                    if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
                        btnSubnav_Click(btnSubnavInitialActionApproval, null);
                    break;
                case "10":
                    if ((status = uclapproval.AddUpdateINCFORM_APPROVAL(incidentId, "save")) >= 0)
                        btnSubnav_Click(btnSubnavApproval, null);
                    break;
                case "11":
                    // save cross-plant alerts
                    break;
                default:

                    List<String> selected_InitialTreatmentList = new List<string>();
                    foreach (ListItem item in CBL_InitialTreatmentGiven.Items)
                    {
                        if (item.Selected)
                        {
                            selected_InitialTreatmentList.Add(item.Value);
                        }
                    }
                    // Join the string together using the ; delimiter.
                    selected_InitialTreatment = String.Join(",", selected_InitialTreatmentList.ToArray());

                    if ((selected_InitialTreatment.Length == 0) && ( EditIncidentId > maxIncidentforInjuryType || EditIncidentId == 0))
                    {
                        lblStatusMsg.Text = "Please Select Atleast One Treatment Pattern.";
                    }
                    else
                    {
                        if (AddUpdateINCFORM_INJURYILLNESS() > 0)
                            btnSubnav_Click(btnSubnavIncident, null);
                    }
                    break;
            }

            if (status >= 0)
            {
                string script;
                if (selected_InitialTreatment.Length == 0 && EditIncidentId > maxIncidentforInjuryType || EditIncidentId == 0)
                {
                    string lblStatusMsg = "Please Select Atleast One Treatment Pattern.";
                    script = string.Format("alert('{0}');", lblStatusMsg);
                }
                else
                {
                    script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);

                }

                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
            }
        }

        protected void btnSubnav_Click(object sender, EventArgs e)
        {


            lblStatusMsg.Text = "";
            LinkButton btn = (LinkButton)sender;

            pnlBaseForm.Visible = uclcontain.Visible = uclroot5y.Visible = uclCausation.Visible = uclaction.Visible = uclapproval.Visible = uclAlert.Visible = uclVideoPanel.Visible = false;
            btnSubnavIncident.Visible = btnSubnavContainment.Visible = btnSubnavRootCause.Visible = btnSubnavCausation.Visible = btnSubnavAction.Visible = btnSubnavApproval.Visible = true;
            CurrentSubnav = btn.CommandArgument;

            btnSubnavIncident.Enabled = btnSubnavApproval.Enabled = btnSubnavAction.Enabled = btnSubnavRootCause.Enabled = btnSubnavCausation.Enabled = btnSubnavContainment.Enabled = btnSubnavVideo.Enabled = btnSubnavAlert.Enabled=btnSubnavCEOComment.Enabled = true;
            btnSubnavIncident.CssClass = btnSubnavContainment.CssClass = btnSubnavRootCause.CssClass = btnSubnavCausation.CssClass = btnSubnavAction.CssClass = btnSubnavApproval.CssClass = btnSubnavAlert.CssClass= btnSubnavCEOComment.CssClass = btnSubnavVideo.CssClass = "buttonLink";
            btnSubnavSave.Visible = btnDeleteInc.Visible = false;

            btnSubnavVideo.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 1.1m);
            btnSubnavCEOComment.Visible= btnSubnavAlert.Visible = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 11.0m);

            btnSubnavInitialActionApproval.Visible = btnSubnavInitialActionApproval.Enabled = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 2.5m);
            btnSubnavInitialActionApproval.CssClass = "buttonLink";
            btnSubnavCorrectiveActionApproval.Visible = btnSubnavCorrectiveActionApproval.Enabled = EHSIncidentMgr.IsStepActive(incidentStepList, (decimal)EHSIncidentTypeId.InjuryIllness, 5.5m);
            btnSubnavCorrectiveActionApproval.CssClass = "buttonLink";

            lblFormTitle.Text = Resources.LocalizedText.Incident;

            decimal incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

            switch (btn.CommandArgument)
            {
                case "1.1":
                    lblFormTitle.Text = btnSubnavVideo.Text;
                    uclVideoPanel.Visible = true;
                    btnSubnavVideo.Enabled = false;
                    btnSubnavVideo.CssClass = "buttonLinkDisabled";
                    INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, EditIncidentId);
                    PageUseMode viewMode = EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.originate, incident.INCFORM_LAST_STEP_COMPLETED) == true ? PageUseMode.EditEnabled : PageUseMode.ViewOnly;
                    uclVideoPanel.OpenManageVideosWindow((int)TaskRecordType.HealthSafetyIncident, EditIncidentId, "1", (decimal)incident.DETECT_PLANT_ID, Resources.LocalizedText.VideoUpload, Resources.LocalizedText.VideoForIncident, "", rddlInjuryType.SelectedValue, rddlBodyPart.SelectedValue, viewMode, false, "");
                    break;
                case "2":
                    lblFormTitle.Text = btnSubnavContainment.Text;
                    btnSubnavContainment.Enabled = false;
                    btnSubnavContainment.CssClass = "buttonLinkDisabled";
                    uclcontain.Visible = true;
                    uclcontain.IsEditContext = true;
                    uclcontain.IncidentId = EditIncidentId;
                    uclcontain.PopulateInitialForm();
                    break;
                case "3":
                    lblFormTitle.Text = btnSubnavRootCause.Text;
                    btnSubnavRootCause.Enabled = false;
                    btnSubnavRootCause.CssClass = "buttonLinkDisabled";
                    uclroot5y.Visible = true;
                    uclroot5y.IsEditContext = true;
                    uclroot5y.IncidentId = EditIncidentId;
                    uclroot5y.PopulateInitialForm();
                    break;
                case "4":
                    lblFormTitle.Text = btnSubnavCausation.Text;
                    btnSubnavCausation.Enabled = false;
                    btnSubnavCausation.CssClass = "buttonLinkDisabled";
                    uclCausation.Visible = true;
                    uclCausation.IsEditContext = true;
                    uclCausation.IncidentId = EditIncidentId;
                    uclCausation.PopulateInitialForm(entities);
                    break;
                case "5":
                    lblFormTitle.Text = btnSubnavAction.Text;
                    btnSubnavAction.Enabled = false;
                    btnSubnavAction.CssClass = "buttonLinkDisabled";
                    uclaction.Visible = true;
                    uclaction.IsEditContext = true;
                    uclaction.IncidentId = EditIncidentId;
                    uclaction.PopulateInitialForm();


                    break;
                // approval steps
                case "2.5":
                    lblFormTitle.Text = btnSubnavInitialActionApproval.Text;
                    btnSubnavInitialActionApproval.Enabled = false;
                    btnSubnavInitialActionApproval.CssClass = "buttonLinkDisabled";
                    uclapproval.IsEditContext = true;
                    uclapproval.IncidentId = EditIncidentId;
                    uclapproval.Visible = true;
                    uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness && s.STEP == 2.5m).FirstOrDefault());

                    break;
                case "5.5":
                    lblFormTitle.Text = btnSubnavCorrectiveActionApproval.Text;
                    btnSubnavCorrectiveActionApproval.Enabled = false;
                    btnSubnavCorrectiveActionApproval.CssClass = "buttonLinkDisabled";
                    uclapproval.IsEditContext = true;
                    uclapproval.IncidentId = EditIncidentId;
                    uclapproval.Visible = true;
                    uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness && s.STEP == 5.5m).FirstOrDefault());
                    break;
                case "10":
                    lblFormTitle.Text = btnSubnavApproval.Text;
                    btnSubnavApproval.Enabled = false;
                    btnSubnavApproval.CssClass = "buttonLinkDisabled";
                    uclapproval.IsEditContext = true;
                    uclapproval.IncidentId = EditIncidentId;
                    uclapproval.Visible = true;
                    uclapproval.PopulateInitialForm(incidentStepList.Where(s => s.INCIDENT_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness && s.STEP == 10.0m).FirstOrDefault());
                    break;
                case "11":
                    lblFormTitle.Text = btnSubnavAlert.Text;
                    btnSubnavAlert.Enabled = false;
                    btnSubnavAlert.CssClass = "buttonLinkDisabled";
                    uclAlert.IncidentId = EditIncidentId;
                    uclAlert.Visible = true;
                    uclAlert.PopulateInitialForm(entities);
                    break;
		
		//CEO Comment link
                case "12":
                    lblFormTitle.Text = btnSubnavAlert.Text;
                    btnSubnavCEOComment.Enabled = false;
                    btnSubnavCEOComment.CssClass = "buttonLinkDisabled";
                    uclAlert.IncidentId = EditIncidentId;
                    uclAlert.Visible = true;
                    uclAlert.PopulateInitialForm(entities);
                    break;
                case "0":
                default:
                    lblFormTitle.Text = btnSubnavIncident.Text;
                    btnSubnavIncident.Visible = true;
                    btnSubnavIncident.Enabled = false;
                    btnSubnavIncident.CssClass = "buttonLinkDisabled";
                    if (pnlBaseForm.Visible == false)
                        pnlBaseForm.Visible = true;
                    PopulateInitialForm();
                    btnSubnavSave.Visible = btnSubnavSave.Enabled = EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.originate, IncidentStepCompleted);
                    btnDeleteInc.Visible = EHSIncidentMgr.CanDeleteIncident(CreatePersonId, IncidentStepCompleted);
                    uploader.SetViewMode(btnSubnavSave.Enabled);
                    uploader.SetReportOption(false);
                    break;
            }
        }


        protected decimal AddUpdateINCFORM_INJURYILLNESS()
        {
            decimal theincidentId = 0;
            decimal incidentId = 0;
            int status = 0;

            if (!IsEditContext)
            {
                incidentDescription = tbDescription.Text;
                localDescription = tbLocalDescription.Text;
            }
            else
            {
                incidentDescription = tbDescription.Text;
                localDescription = tbLocalDescription.Text;
                incidentId = EditIncidentId;
            }

            if (incidentDescription.Length > MaxTextLength)
                incidentDescription = incidentDescription.Substring(0, MaxTextLength);

            if (InitialPlantId == 0)
                InitialPlantId = IncidentLocationId;

            decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

            IncidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;

            entities = new PSsqmEntities();

            if (!IsEditContext)
            {
                incidentDescription = tbDescription.Text;
                localDescription = tbLocalDescription.Text;
                TheIncident = CreateNewIncident();
                EditIncidentId = incidentId = TheIncident.INCIDENT_ID;
                theincidentId = TheIncident.INCIDENT_ID;

                TheINCFORM = CreateNewInjuryIllnessDetails(incidentId);
                SaveAttachments(incidentId);

                EHSNotificationMgr.NotifyIncidentStatus(TheIncident, ((int)SysPriv.originate).ToString(), "");
            }
            else
            {
                incidentDescription = tbDescription.Text;
                localDescription = tbLocalDescription.Text;
                incidentId = EditIncidentId;
                if (incidentId > 0)
                {
                    TheIncident = UpdateIncident(incidentId);
                    TheINCFORM = UpdateInjuryIllnessDetails(incidentId);
                    SaveAttachments(incidentId);
                }

                theincidentId = incidentId;
                EHSNotificationMgr.NotifyIncidentStatus(TheIncident, ((int)SysPriv.update).ToString(), "");
            }

            if (status >= 0)
            {
                if (incidentId == 0)
                    incidentId = (IsEditContext) ? EditIncidentId : NewIncidentId;
                IsEditContext = true;

                if (TheINCFORM != null)
                {
                    if (TheINCFORM.LOST_TIME == true || TheINCFORM.RESTRICTED_TIME == true)
                    {
                        uclRecordableHist.IncidentId = TheIncident.INCIDENT_ID;
                        uclRecordableHist.WorkStatusIncident = TheIncident;
                        uclRecordableHist.AddUpdateINCFORM_LOSTTIME_HIST(incidentId);
                    }
                }
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
                CREATE_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
                CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
                LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
                LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ),
                DESCRIPTION = incidentDescription,
                CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
                ISSUE_TYPE = SelectedTypeText,
                ISSUE_TYPE_ID = SelectedTypeId,
                TNSKNumber = RTXT_TNSK.Text.Trim(),
                INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined
            };

            entities.AddToINCIDENT(newIncident);

            if (entities.SaveChanges() > 0)
            {
                incidentId = newIncident.INCIDENT_ID;
            }

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
                incident.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
                if (incident.INCFORM_LAST_STEP_COMPLETED < (int)IncidentStepStatus.defined)
                    incident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined;
                incident.TNSKNumber = RTXT_TNSK.Text.Trim();
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

            // involved person input
            if (!string.IsNullOrEmpty(tbDepartment.Text.Trim()))
            {
                newInjryIllnessDetails.DEPARTMENT = tbDepartment.Text.Trim();
            }

            // involved person input
            //if (!string.IsNullOrEmpty(tbInvolvedPerson.Text.Trim()))
            //{
                newInjryIllnessDetails.INVOLVED_PERSON_NAME = tbInvolvedPerson.Text.Trim();
            //}

            newInjryIllnessDetails.INVOLVED_PERSON_STATEMENT = tbInvPersonStatement.Text;

            if (rdpSupvInformedDate.SelectedDate != null)
                newInjryIllnessDetails.SUPERVISOR_INFORMED_DT = rdpSupvInformedDate.SelectedDate;

            if (!String.IsNullOrEmpty(tbSupervisorStatement.Text))
                newInjryIllnessDetails.SUPERVISOR_STATEMENT = tbSupervisorStatement.Text;

            if (rdoInside.SelectedValue == "0")
                newInjryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Outside Building";
            else
                newInjryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Inside Building";

            if (EditIncidentId > maxINCIDENT)
            {

                if (!String.IsNullOrEmpty(rdoEmpStatus.SelectedValue))
                    newInjryIllnessDetails.EMP_STATUS = (Convert.ToInt32(rdoEmpStatus.SelectedValue));
                if (radAssociateSelect.SelectedDate != null)
                {
                    string AssociateDate = radAssociateSelect.SelectedDate.ToString();
                    var strMonthYear = AssociateDate.Split('/');
                    newInjryIllnessDetails.ASSOCIATE_MONTHS = Convert.ToInt32(strMonthYear[0]);
                    var AssociateYear = strMonthYear[2].Split(' ');
                    newInjryIllnessDetails.ASSOCIATE_YEAR = Convert.ToInt32(AssociateYear[0]);
                }

                if (radHireSelect.SelectedDate != null)
                {
                    string HireDate = radHireSelect.SelectedDate.ToString();
                    var strMonthYear = HireDate.Split('/');
                    newInjryIllnessDetails.HIRE_MONTHS = Convert.ToInt32(strMonthYear[0]);
                    var HireYear = strMonthYear[2].Split(' ');
                    newInjryIllnessDetails.HIRE_YEAR = Convert.ToInt32(HireYear[0]);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(rdoDirectSupv.SelectedValue))
                    newInjryIllnessDetails.COMPANY_SUPERVISED = Convert.ToBoolean((Convert.ToInt32(rdoDirectSupv.SelectedValue)));
                if (!string.IsNullOrEmpty(rddlJobTenure.SelectedValue))
                    newInjryIllnessDetails.JOB_TENURE = rddlJobTenure.SelectedValue;
                if (!string.IsNullOrEmpty(rddlEmploymentTenure.SelectedValue))
                    newInjryIllnessDetails.EMPLOYMENT_TENURE = rddlEmploymentTenure.SelectedValue;
            }


            if (!String.IsNullOrEmpty(rdoErgConcern.SelectedValue))
                newInjryIllnessDetails.ERGONOMIC_CONCERN = Convert.ToBoolean((Convert.ToInt32(rdoErgConcern.SelectedValue)));

            if (!String.IsNullOrEmpty(rdoStdProcsFollowed.SelectedValue))
                newInjryIllnessDetails.STD_PROCS_FOLLOWED = Convert.ToBoolean((Convert.ToInt32(rdoStdProcsFollowed.SelectedValue)));

            if (!String.IsNullOrEmpty(rdoTrainingProvided.SelectedValue))
                newInjryIllnessDetails.TRAINING_PROVIDED = Convert.ToBoolean((Convert.ToInt32(rdoTrainingProvided.SelectedValue)));

            if (!String.IsNullOrEmpty(rdoFirstAid.SelectedValue))
                newInjryIllnessDetails.FIRST_AID = Convert.ToBoolean((Convert.ToInt32(rdoFirstAid.SelectedValue)));

            if (!String.IsNullOrEmpty(rdoRecordable.SelectedValue))
                newInjryIllnessDetails.RECORDABLE = Convert.ToBoolean((Convert.ToInt32(rdoRecordable.SelectedValue)));

            if (!String.IsNullOrEmpty(rdoFatality.SelectedValue))
                newInjryIllnessDetails.FATALITY = Convert.ToBoolean((Convert.ToInt32(rdoFatality.SelectedValue)));

            if (!String.IsNullOrEmpty(rddlInjuryType.SelectedValue))
                newInjryIllnessDetails.INJURY_TYPE = rddlInjuryType.SelectedValue;

            if (!String.IsNullOrEmpty(rddlBodyPart.SelectedValue))
                newInjryIllnessDetails.INJURY_BODY_PART = rddlBodyPart.SelectedValue;

            if (!String.IsNullOrEmpty(rdoReoccur.SelectedValue))
                newInjryIllnessDetails.REOCCUR = Convert.ToBoolean((Convert.ToInt32(rdoReoccur.SelectedValue)));

            if (divJobCode.Visible == true)
            {
                newInjryIllnessDetails.JOBCODE_CD = rddlJobCode.SelectedValue;
            }

            if (divProcedures.Visible == true)
            {
                newInjryIllnessDetails.STD_PROCS_DESC = tbProcedures.Text.Trim();
            }

            newInjryIllnessDetails = SetLostTime(newInjryIllnessDetails);

            newInjryIllnessDetails.BUSINESS_TYPE = RDDL_BusinessType.SelectedValue.ToString();
            newInjryIllnessDetails.MACRO_PROCESS_TYPE = RDDL_MacroProcessType.SelectedValue.ToString();
            newInjryIllnessDetails.SPECIFIC_PROCESS_TYPE = RDDL_SpecificProcessType.SelectedValue.ToString();
            newInjryIllnessDetails.EQUIPMENT_MANUFACTURER_NAME = TXT_EquipmentManufacturerName.Text.Trim();
            newInjryIllnessDetails.EQUIPEMENT_MANUFACTURER_DATE = RDP_EquipmentManufacturerDate.SelectedDate;
            newInjryIllnessDetails.DESIGN_NUMBER = RTXT_DesignNumber.Text.Trim();
            newInjryIllnessDetails.ASSET_NUMBER = RTXT_AssetNumber.Text.Trim();
            newInjryIllnessDetails.AGE_OF_ASSOCIATE = RtxtAgeOfAssociate.Text.Trim();
            newInjryIllnessDetails.TYPE_OF_INCIDENT = RDDL_TypeOfIncident.SelectedValue.ToString();
            newInjryIllnessDetails.INITIAL_TREATMENT_GIVEN = selected_InitialTreatment;//CBL_InitialTreatmentGiven.SelectedValue.ToString();
            newInjryIllnessDetails.CHANGE_MEDICAL_STATUS = selected_ChangeInMedicalStatus;//CBL_ChangeInMedicalStatus.SelectedValue.ToString();



            entities.AddToINCFORM_INJURYILLNESS(newInjryIllnessDetails);

            entities.SaveChanges();

            AddUpdate_Witnesses(incidentId);

            UpdateInicidentAnswers(incidentId, newInjryIllnessDetails);

            lblIncidentMsg.Visible = false;



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
                TextBox tbwit = (TextBox)witnessitem.FindControl("tbWitness");
                TextBox tbws = (TextBox)witnessitem.FindControl("tbWitnessStatement");

                if (rsbw != null && !String.IsNullOrEmpty(rsbw.Text))
                {
                    decimal personID = 0;
                    string[] split = rsbw.Text.Split('-');
                    if (split.Length > 0 && decimal.TryParse(split[0], out personID))
                    {
                        item.WITNESS_PERSON = personID;
                    }

                    seqnumber = seqnumber + 1;
                    item.WITNESS_NO = seqnumber;
                    item.WITNESS_STATEMENT = tbws.Text;

                    itemList.Add(item);
                }
                else if (!string.IsNullOrEmpty(tbwit.Text))
                {
                    item.WITNESS_NAME = tbwit.Text.Trim();
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

                if (item.WITNESS_PERSON != null || !string.IsNullOrEmpty(item.WITNESS_NAME))
                {
                    newItem.INCIDENT_ID = incidentId;
                    newItem.WITNESS_NO = item.WITNESS_NO;
                    newItem.WITNESS_PERSON = item.WITNESS_PERSON;
                    newItem.WITNESS_NAME = item.WITNESS_NAME;
                    newItem.WITNESS_STATEMENT = item.WITNESS_STATEMENT;

                    newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
                    newItem.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);

                    entities.AddToINCFORM_WITNESS(newItem);
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

                if (!string.IsNullOrEmpty(tbDepartment.Text.Trim()))
                {
                    injuryIllnessDetails.DEPARTMENT = tbDepartment.Text.Trim();
                }

                // involved person input
                //if (!string.IsNullOrEmpty(tbInvolvedPerson.Text.Trim()))
                //{
                injuryIllnessDetails.INVOLVED_PERSON_NAME = tbInvolvedPerson.Text.Trim();
                //}

                injuryIllnessDetails.INVOLVED_PERSON_STATEMENT = tbInvPersonStatement.Text;

                if (!String.IsNullOrEmpty(tbSupervisorStatement.Text))
                    injuryIllnessDetails.SUPERVISOR_STATEMENT = tbSupervisorStatement.Text;

                if (rdoInside.SelectedValue == "0")
                    injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Outside Building";
                else
                    injuryIllnessDetails.INSIDE_OUTSIDE_BLDNG = "Inside Building";

                if (EditIncidentId > maxINCIDENT)
                {
                    if (!String.IsNullOrEmpty(rdoEmpStatus.SelectedValue))
                        injuryIllnessDetails.EMP_STATUS = (Convert.ToInt32(rdoEmpStatus.SelectedValue));
                    if (radAssociateSelect.SelectedDate != null)
                    {
                        string AssociateDate = radAssociateSelect.SelectedDate.ToString();
                        var strMonthYear = AssociateDate.Split('/');
                        injuryIllnessDetails.ASSOCIATE_MONTHS = Convert.ToInt32(strMonthYear[0]);
                        var AssociateYear = strMonthYear[2].Split(' ');
                        injuryIllnessDetails.ASSOCIATE_YEAR = Convert.ToInt32(AssociateYear[0]);
                    }
                    else
                    {
                        injuryIllnessDetails.ASSOCIATE_MONTHS = null;
                        injuryIllnessDetails.ASSOCIATE_YEAR = null;
                    }

                    if (radHireSelect.SelectedDate != null)
                    {
                        string HireDate = radHireSelect.SelectedDate.ToString();
                        var strMonthYear = HireDate.Split('/');
                        injuryIllnessDetails.HIRE_MONTHS = Convert.ToInt32(strMonthYear[0]);
                        var HireYear = strMonthYear[2].Split(' ');
                        injuryIllnessDetails.HIRE_YEAR = Convert.ToInt32(HireYear[0]);
                    }
                    else
                    {
                        injuryIllnessDetails.HIRE_MONTHS = null;
                        injuryIllnessDetails.HIRE_YEAR = null;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(rdoDirectSupv.SelectedValue))
                        injuryIllnessDetails.COMPANY_SUPERVISED = Convert.ToBoolean((Convert.ToInt32(rdoDirectSupv.SelectedValue)));
                    if (!string.IsNullOrEmpty(rddlJobTenure.SelectedValue))
                        injuryIllnessDetails.JOB_TENURE = rddlJobTenure.SelectedValue;
                    if (!string.IsNullOrEmpty(rddlEmploymentTenure.SelectedValue))
                        injuryIllnessDetails.EMPLOYMENT_TENURE = rddlEmploymentTenure.SelectedValue;
                }




                if (!String.IsNullOrEmpty(rdoErgConcern.SelectedValue))
                    injuryIllnessDetails.ERGONOMIC_CONCERN = Convert.ToBoolean((Convert.ToInt32(rdoErgConcern.SelectedValue)));

                if (!String.IsNullOrEmpty(rdoStdProcsFollowed.SelectedValue))
                    injuryIllnessDetails.STD_PROCS_FOLLOWED = Convert.ToBoolean((Convert.ToInt32(rdoStdProcsFollowed.SelectedValue)));

                if (!String.IsNullOrEmpty(rdoTrainingProvided.SelectedValue))
                    injuryIllnessDetails.TRAINING_PROVIDED = Convert.ToBoolean((Convert.ToInt32(rdoTrainingProvided.SelectedValue)));



                if (!String.IsNullOrEmpty(rdoFirstAid.SelectedValue))
                    injuryIllnessDetails.FIRST_AID = Convert.ToBoolean((Convert.ToInt32(rdoFirstAid.SelectedValue)));

                if (!String.IsNullOrEmpty(rdoRecordable.SelectedValue))
                    injuryIllnessDetails.RECORDABLE = Convert.ToBoolean((Convert.ToInt32(rdoRecordable.SelectedValue)));

                if (!String.IsNullOrEmpty(rdoFatality.SelectedValue))
                    injuryIllnessDetails.FATALITY = Convert.ToBoolean((Convert.ToInt32(rdoFatality.SelectedValue)));

                if (!String.IsNullOrEmpty(rddlInjuryType.SelectedValue))
                    injuryIllnessDetails.INJURY_TYPE = rddlInjuryType.SelectedValue;

                if (!String.IsNullOrEmpty(rddlBodyPart.SelectedValue))
                    injuryIllnessDetails.INJURY_BODY_PART = rddlBodyPart.SelectedValue;

                if (!String.IsNullOrEmpty(rdoReoccur.SelectedValue))
                    injuryIllnessDetails.REOCCUR = Convert.ToBoolean((Convert.ToInt32(rdoReoccur.SelectedValue)));

                if (divJobCode.Visible == true)
                {
                    injuryIllnessDetails.JOBCODE_CD = rddlJobCode.SelectedValue;
                }

                if (divProcedures.Visible == true)
                {
                    injuryIllnessDetails.STD_PROCS_DESC = tbProcedures.Text.Trim();
                }
                injuryIllnessDetails.BUSINESS_TYPE = RDDL_BusinessType.SelectedValue.ToString();
                injuryIllnessDetails.MACRO_PROCESS_TYPE = RDDL_MacroProcessType.SelectedValue.ToString();
                injuryIllnessDetails.SPECIFIC_PROCESS_TYPE = RDDL_SpecificProcessType.SelectedValue.ToString();
                injuryIllnessDetails.EQUIPMENT_MANUFACTURER_NAME = TXT_EquipmentManufacturerName.Text.Trim();
                injuryIllnessDetails.EQUIPEMENT_MANUFACTURER_DATE = RDP_EquipmentManufacturerDate.SelectedDate;
                injuryIllnessDetails.DESIGN_NUMBER = RTXT_DesignNumber.Text.Trim();
                injuryIllnessDetails.ASSET_NUMBER = RTXT_AssetNumber.Text.Trim();
                injuryIllnessDetails.AGE_OF_ASSOCIATE = RtxtAgeOfAssociate.Text.Trim();
                injuryIllnessDetails.TYPE_OF_INCIDENT = RDDL_TypeOfIncident.SelectedValue.ToString();
                injuryIllnessDetails.INITIAL_TREATMENT_GIVEN = selected_InitialTreatment;// CBL_InitialTreatmentGiven.SelectedValue.ToString();
                injuryIllnessDetails.CHANGE_MEDICAL_STATUS = selected_ChangeInMedicalStatus;// CBL_ChangeInMedicalStatus.SelectedValue.ToString();
                injuryIllnessDetails = SetLostTime(injuryIllnessDetails);
                entities.SaveChanges();
                AddUpdate_Witnesses(incidentId);

                UpdateInicidentAnswers(incidentId, injuryIllnessDetails);

                lblIncidentMsg.Visible = false;
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
                    ia.ANSWER_VALUE = injtype.Where(m => m.Value == injuryIllnessDetail.INJURY_TYPE).Select(m => m.Text).FirstOrDefault();
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
                    ia.ANSWER_VALUE = injuryIllnessDetail.FIRST_AID == true ? "Yes" : "No";
                    ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
                    entities.AddToINCIDENT_ANSWER(ia);

                    ia = new INCIDENT_ANSWER();
                    ia.INCIDENT_ID = incidentId;
                    ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Department);

                    ia.ANSWER_VALUE = injuryIllnessDetail.DEPT_ID.HasValue ? injuryIllnessDetail.DEPT_ID.ToString() : injuryIllnessDetail.DEPARTMENT;
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
                    if (injuryIllnessDetail.INVOLVED_PERSON_ID > 0)
                    {
                        ia.ANSWER_VALUE = injuryIllnessDetail.INVOLVED_PERSON_ID.ToString();
                    }
                    else
                    {
                        ia.ANSWER_VALUE = injuryIllnessDetail.INVOLVED_PERSON_NAME;
                    }
                    ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
                    entities.AddToINCIDENT_ANSWER(ia);

                    ia = new INCIDENT_ANSWER();
                    ia.INCIDENT_ID = incidentId;
                    ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.Shift);
                    ia.ANSWER_VALUE = injuryIllnessDetail.SHIFT;
                    ia.ORIGINAL_QUESTION_TEXT = qList.Where(l => l.INCIDENT_QUESTION_ID == ia.INCIDENT_QUESTION_ID).Select(l => l.QUESTION_TEXT).FirstOrDefault();
                    entities.AddToINCIDENT_ANSWER(ia);

                    ia = new INCIDENT_ANSWER();
                    ia.INCIDENT_ID = incidentId;
                    ia.INCIDENT_QUESTION_ID = Convert.ToInt32(EHSQuestionId.TimeOfDay);
                    ia.ANSWER_VALUE = injuryIllnessDetail.INCIDENT_TIME.ToString();
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

        private void PopulateTypeOfIncident()
        {
            //RDDL_TypeOfIncident
            List<EHSMetaData> SpecificProcessType = EHSMetaDataMgr.SelectMetaDataList("INCIDENT_TYPE");

            if (SpecificProcessType != null && SpecificProcessType.Count > 0)
            {
                RDDL_TypeOfIncident.Items.Clear();

                RDDL_TypeOfIncident.Items.Add(new DropDownListItem("", ""));

                foreach (var s in SpecificProcessType)
                {

                    RDDL_TypeOfIncident.Items.Add(new DropDownListItem(s.Text, s.Value));

                }
            }
        }

        protected void RDDL_MacroProcessType_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        {

            string selectedValue = RDDL_MacroProcessType.SelectedValue.ToString();
            PopulateSpecificProcessType(selectedValue);

        }

        private void PopulateSpecificProcessType(string selectedValue)
        {
            List<EHSMetaData> SpecificProcessType = EHSMetaDataMgr.SelectMetaDataList(selectedValue);

            if (SpecificProcessType != null && SpecificProcessType.Count > 0)
            {
                RDDL_SpecificProcessType.Items.Clear();
                RDDL_SpecificProcessType.Items.Add(new DropDownListItem("", ""));
                RDDL_SpecificProcessType.Enabled = true;
                foreach (var s in SpecificProcessType)
                {

                    RDDL_SpecificProcessType.Items.Add(new DropDownListItem(s.Text, s.Value));

                }
            }
            else
            {
                RDDL_SpecificProcessType.Items.Clear();

                RDDL_SpecificProcessType.Items.Add(new DropDownListItem("", ""));
                RDDL_SpecificProcessType.Enabled = false;
            }
        }

        private void PopulateRDDL_BusinessType()
        {

            RDDL_BusinessType.Items.Clear();
            List<EHSMetaData> BusinessType = EHSMetaDataMgr.SelectMetaDataList("BusinessType");

            if (BusinessType != null && BusinessType.Count > 0)
            {
                RDDL_BusinessType.Items.Add(new DropDownListItem("", ""));

                foreach (var s in BusinessType)
                {
                    {
                        RDDL_BusinessType.Items.Add(new DropDownListItem(s.Text, s.Value));
                    }
                }
            }
        }
        protected void RDDL_BusinessType_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        {
            string selectedValue = RDDL_BusinessType.SelectedValue.ToString();

            PopulateMacroProcessType(selectedValue);
        }

        private void PopulateMacroProcessType(string selectedValue)
        {
            //RDDL_TypeOfIncident
            List<EHSMetaData> MacroProcessType = EHSMetaDataMgr.SelectMetaDataList(selectedValue).OrderBy(P => P.SortOrder).ToList(); ;

            RDDL_SpecificProcessType.Items.Clear();
            RDDL_SpecificProcessType.Items.Add(new DropDownListItem("", ""));
            RDDL_SpecificProcessType.Enabled = false;

            if (MacroProcessType != null && MacroProcessType.Count > 0)
            {
                RDDL_MacroProcessType.Items.Clear();
                RDDL_MacroProcessType.Items.Add(new DropDownListItem("", ""));
                RDDL_MacroProcessType.Enabled = true;
                foreach (var s in MacroProcessType)
                {
                    RDDL_MacroProcessType.Items.Add(new DropDownListItem(s.Text, s.Value));
                }
            }
            else
            {
                RDDL_MacroProcessType.Items.Clear();
                RDDL_MacroProcessType.Items.Add(new DropDownListItem("", ""));
                RDDL_MacroProcessType.Enabled = false;
            }
        }
        string selected_InitialTreatment, selected_ChangeInMedicalStatus;
        protected void CBL_InitialTreatmentGiven_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<String> selected_InitialTreatmentList = new List<string>();
            foreach (ListItem item in CBL_InitialTreatmentGiven.Items)
            {
                if (item.Selected)
                {
                    selected_InitialTreatmentList.Add(item.Value);
                }

            }
            // Join the string together using the ; delimiter.
            selected_InitialTreatment = String.Join(",", selected_InitialTreatmentList.ToArray());

        }

        protected void CBL_ChangeInMedicalStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<String> selected_ChangeInMedicalStatusList = new List<string>();
            foreach (ListItem item in CBL_ChangeInMedicalStatus.Items)
            {
                if (item.Selected)
                {
                    selected_ChangeInMedicalStatusList.Add(item.Value);
                }

            }
            // Join the string together using the ; delimiter.
            selected_ChangeInMedicalStatus = String.Join(",", selected_ChangeInMedicalStatusList.ToArray());

        }
    }
}
