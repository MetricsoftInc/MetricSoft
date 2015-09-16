using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace SQM.Website

{
    public enum CaseUpdateStatus { Success, SaveError, IncidentError, RequiredInputs, Incomplete, CompleteError, RootCauseError};

    public class ProblemCaseCtl
    {
        public ProblemCase problemCase
        {
            get;
            set;
        }
        public string Context
        {
            get;
            set;
        }
        public PageUseMode PageMode
        {
            get;
            set;
        }
        public QSCalcsCtl CalcsCtl
        {
            get;
            set;
        }
        public bool isAutoCreated
        {
            get;
            set;
        }
        public bool isDirected
        {
            get;
            set;
        }
        //public ProblemCase tempCase
        //{
        //    get;
        //    set;
        //}
        public List<PROB_CASE> CaseList
        {
            get;
            set;
        }
        //public List<SQM.Shared.CorrectiveAction> actionList
        //{
        //    get;
        //    set;
        //}
        public PSsqmEntities Entities
        {
            get;
            set;
        }
        public bool isNew 
        {
            get { return this.problemCase.IsNew; }
        }
        public ProblemCaseCtl Initialize(PSsqmEntities entities, string context)
        {

            this.Context = context;
            this.PageMode = PageUseMode.ViewOnly;
            this.CaseList = new List<PROB_CASE>();
            if (entities != null)
                this.Entities = entities;

            this.problemCase = new ProblemCase().Initialize();

            InitializeCalcs();

            return this;
        }

        public QSCalcsCtl InitializeCalcs()
        {
            return this.CalcsCtl = new QSCalcsCtl().CreateNew();
        }

        public ProblemCase CreateNew(string caseType, decimal companyID, PERSON createPerson)
        {
			if (this.problemCase == null)
				this.problemCase = new ProblemCase().Initialize();

            this.problemCase.CreateNew(caseType, companyID);

            if (createPerson != null)
                this.problemCase.ProbCase.CREATE_BY = SQMModelMgr.FormatPersonListItem(createPerson);

            return this.problemCase;
        }

        public ProblemCase Load(decimal caseID)
        {
            this.problemCase = new ProblemCase();
            this.problemCase.Load(caseID);
            return this.problemCase;
        }

        public ProblemCase Load(decimal caseID, string context)
        {
            this.Load(caseID);
            this.Context = context;
            return this.problemCase;
        }

        public void Clear()
        {
            this.problemCase = null;
        }
        public bool IsClear()
        {
            return (this.problemCase == null ? true : false);
        }

        //public void CopyTempCase()
        //{
        //    this.tempCase = this.problemCase;
        //}

        public int PersonSelectListCount()
        {
            if (this.problemCase.PersonSelectList == null)
                return 0;
            else 
                return this.problemCase.PersonSelectList.Count;
        }
        public List<PERSON> PersonSelectList()
        {
            return this.problemCase.PersonSelectList;
        }

        public List<PERSON> LoadPersonSelectList(bool anyLocation)
        {
            return this.problemCase.LoadPersonSelectList(anyLocation, this.Context);
        }

        public ProblemCase Update()
        {
            this.problemCase = ProblemCase.UpdateProblemCase(this.problemCase);
            return this.problemCase;
        }
    }

    public class ProblemCase
    {
        public PROB_CASE ProbCase
        {
            get;
            set;
        }
        public bool IsNew
        {
            get;
            set;
        }
        public string CaseID
        {
            get
            {
                return (WebSiteCommon.FormatID((int)this.AliasID, 6));
            }
            set
            {
                ;
            }
        }
        public decimal AliasID
        {
            get;
            set;
        }

        public string StepName
        {
            get;
            set;
        }

        public List<INCIDENT> IncidentList
        {
            get;
            set;
        }

        public List<object> IssueList
        {
            get;
            set;
        }
        public List<ATTACHMENT> AttachmentsList
        {
            get;
            set;
        }
        public List<PartIssueItem> PartIssueItemList
        {
            get;
            set;
        }

        public PROB_DEFINE Define
        {
            get;
            set;
        }

        public string StepComplete
        {
            get;
            set;
        }
        public TaskStatusMgr TeamTask 
        {
            get;
            set;
        }
        public List<PERSON> PersonSelectList
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public  CaseUpdateStatus UpdateStatus
        {
            get;
            set;
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }

        public ProblemCase CreateNew(string caseType, decimal companyID)
        {
            this.Initialize();
            this.ProbCase.STATUS = "A";
            this.ProbCase.PROBCASE_TYPE = caseType;
            this.ProbCase.COMPANY_ID = companyID;
            this.ProbCase.CREATE_DT = this.ProbCase.LAST_UPD_DT = DateTime.UtcNow;
            this.ProbCase.CREATE_BY = this.ProbCase.LAST_UPD_BY = SessionManager.UserContext.UserName();
            this.ProbCase.PROGRESS = 0;
            this.IsNew = true;
            this.AttachmentsList = new List<ATTACHMENT>();
            this.CreateProblemDefinition();
            this.ProbCase.PROB_OCCUR = new System.Data.Objects.DataClasses.EntityCollection<PROB_OCCUR>();
            this.CreateProblemContainment();
            this.CreateProblemRootCause();
            this.CreateProblemVerify();
            this.ProbCase.PROB_PREVENT = new PROB_PREVENT();
            this.CreateProblemRisk();
            this.CreateProblemClose();

            return this;
        }

        public ProblemCase Initialize()
        {
            this.ProbCase = new PROB_CASE();
            this.IncidentList = new List<INCIDENT>();
            this.IssueList = new List<object>();
            this.PartIssueItemList = new List<PartIssueItem>();
            this.Entities = new PSsqmEntities();
            this.TeamTask = new TaskStatusMgr().Initialize(21, 0);
            this.StepComplete = "";
            this.AttachmentsList = new List<ATTACHMENT>();
            this.PersonSelectList = new List<PERSON>();

            return this;
        }

        public ProblemCase LoadAttachments()
        {
            this.AttachmentsList = SQM.Website.Classes.SQMDocumentMgr.SelectAttachmentListByRecord(21, this.ProbCase.PROBCASE_ID, "", "");
            return this;
        }

        public int CreateStepCompleteTasks()
        {
            int taskCount = 0;

            TASK_STATUS task = null;
            Dictionary<string, string> stepList = WebSiteCommon.GetXlatList("caseStep", "", "short");
            foreach (KeyValuePair<string, string> step in stepList)
            {
                if ((task = this.TeamTask.FindTask(step.Key, "C", 0)) == null)
                {
                    this.TeamTask.CreateTask(step.Key, "C", 0, step.Value, DateTime.MinValue, 0);
                }
            }

            return taskCount;
        }

        public ProblemCase Load(decimal caseID)
        {
            this.Initialize();
            if ((this.ProbCase = LookupCase(this.Entities, caseID)) != null)
            {
                this.IsNew = false;
                foreach (PROB_OCCUR occur in this.ProbCase.PROB_OCCUR)
                {
                    this.IncidentList.Add(LookupIncident(occur.INCIDENT_ID));
                    LoadIncidentIssueInfo(occur.INCIDENT_ID);
                }
 
                this.LoadAttachments();

                this.TeamTask.LoadTaskList(21, this.ProbCase.PROBCASE_ID);

                SetAliasID();
            }
            return this;
        }

        public void AddIncident(decimal incidentID)
        {
            INCIDENT incident = LookupIncident(incidentID);
            if (incident != null)
            {
                this.IncidentList.Add(incident);
                PROB_OCCUR occur = new PROB_OCCUR();
                occur.INCIDENT_ID = incidentID;
                occur.STATUS = "A";
                this.ProbCase.PROB_OCCUR.Add(occur);
                LoadIncidentIssueInfo(incidentID);
                SetAliasID();
            }
            return;
        }

        public void SetAliasID()
        {
            if (this.ProbCase.PROB_OCCUR.Count > 0 && this.ProbCase.PROBCASE_TYPE == "EHS")
            {
                this.AliasID = this.ProbCase.PROB_OCCUR.ElementAt(0).INCIDENT_ID;
            }
            else
            {
                this.AliasID = this.ProbCase.PROBCASE_ID;
            }
        }

        public List<PERSON> LoadPersonSelectList(bool anyLocation, string appContext)
        {
            this.PersonSelectList = new List<PERSON>();
                // limit the list to those people having access to the plant where the incident (if defined) occurred
            INCIDENT incident = this.IncidentList.FirstOrDefault();
            if (incident != null)
            {
                List<BusinessLocation> locationList = new List<BusinessLocation>();
                locationList.Add(new BusinessLocation().Initialize((decimal)incident.DETECT_PLANT_ID));
                if (incident.RESP_PLANT_ID.HasValue)
                    locationList.Add(new BusinessLocation().Initialize((decimal)incident.RESP_PLANT_ID));
                this.PersonSelectList = SQMModelMgr.SelectPlantPersonList(locationList, appContext == "SQM" ? "211,212" : "312" , AccessMode.Update);
            }
       
            return this.PersonSelectList;
        }

        public BusinessLocation ProblemBusinessLocation()
        {
            BusinessLocation problemLocation = new BusinessLocation();

            if (this.IncidentList != null && this.IncidentList.Count > 0)
            {
                INCIDENT incident = this.IncidentList[0];
                if (incident.INCIDENT_TYPE == "QI")
                {
                    problemLocation.Company = SQMModelMgr.LookupCompany((decimal)incident.RESP_COMPANY_ID);
                    problemLocation.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)incident.RESP_BUS_ORG_ID);
                    problemLocation.Plant = SQMModelMgr.LookupPlant((decimal)incident.RESP_PLANT_ID);
                }
                else
                {
                    problemLocation.Company = SQMModelMgr.LookupCompany((decimal)incident.DETECT_COMPANY_ID);
                    problemLocation.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)incident.DETECT_BUS_ORG_ID);
                    problemLocation.Plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
                }
            }
            else
            {
                problemLocation.Company = this.ProbCase.COMPANY;
            }
 
            return problemLocation;
        }

        public PROB_DEFINE CreateProblemDefinition()
        {
            this.ProbCase.PROB_DEFINE = new PROB_DEFINE();
            this.ProbCase.PROB_DEFINE.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            return this.ProbCase.PROB_DEFINE;
        }

        public PROB_CONTAIN CreateProblemContainment()
        {
            this.ProbCase.PROB_CONTAIN = new PROB_CONTAIN();
            this.ProbCase.PROB_CONTAIN.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            this.ProbCase.PROB_CONTAIN.RISK1_SEVERITY = this.ProbCase.PROB_CONTAIN.RISK1_OCCUR = this.ProbCase.PROB_CONTAIN.RISK1_DETECT = this.ProbCase.PROB_CONTAIN.RISK1_INDEX = 1;
            this.ProbCase.PROB_CONTAIN.RISK2_SEVERITY = this.ProbCase.PROB_CONTAIN.RISK2_OCCUR = this.ProbCase.PROB_CONTAIN.RISK2_DETECT = this.ProbCase.PROB_CONTAIN.RISK2_INDEX = 1;
            return this.ProbCase.PROB_CONTAIN;
        }

        public PROB_RISK CreateProblemRisk()
        {
            this.ProbCase.PROB_RISK = new PROB_RISK();
            this.ProbCase.PROB_RISK.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            this.ProbCase.PROB_RISK.RISK1_STATE = this.ProbCase.PROB_RISK.RISK2_STATE = "";
            this.ProbCase.PROB_RISK.RISK1_SEVERITY = this.ProbCase.PROB_RISK.RISK1_OCCUR = this.ProbCase.PROB_RISK.RISK1_DETECT = this.ProbCase.PROB_RISK.RISK1_INDEX = 1;
            this.ProbCase.PROB_RISK.RISK2_SEVERITY = this.ProbCase.PROB_RISK.RISK2_OCCUR = this.ProbCase.PROB_RISK.RISK2_DETECT = this.ProbCase.PROB_RISK.RISK2_INDEX = 1;

            return this.ProbCase.PROB_RISK;
        }


        public PROB_CAUSE CreateProblemRootCause()
        {
            this.ProbCase.PROB_CAUSE = new PROB_CAUSE();
            this.ProbCase.PROB_CAUSE.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            return this.ProbCase.PROB_CAUSE;
        }

        public SQM.Shared.CorrectiveAction CreateCorrectiveAction(PROB_CAUSE_STEP cause, PROB_CAUSE_ACTION existingAction)
        {
            SQM.Shared.CorrectiveAction action = CreateCorrectiveAction(cause);
            action.ActionNo = existingAction.ACTION_NO;
            action.ActionCode = existingAction.ACTION_CD;
            action.Action = existingAction.ACTION_DESC;
            action.ActionCode = existingAction.ACTION_CD;
            if (existingAction.EFF_DT.HasValue)
                action.EffDate = Convert.ToDateTime(existingAction.EFF_DT);
            action.Responsible1 = existingAction.RESPONSIBLE1;
            if (existingAction.RESPONSIBLE1_PERSON.HasValue)
                action.Responsible1ID = (decimal)existingAction.RESPONSIBLE1_PERSON;
            action.Responsible2 = existingAction.RESPONSIBLE2;
            action.VerifyStatus = existingAction.VERIFY_STATUS;
            action.VerifyObservations = existingAction.VERIFY_OBSERVATIONS;

            return action;
        }
        public SQM.Shared.CorrectiveAction CreateCorrectiveAction(PROB_CAUSE_STEP cause)
        {
            SQM.Shared.CorrectiveAction action = new SQM.Shared.CorrectiveAction();
            action.RelatedCauseNo = cause.ITERATION_NO;
            action.RelatedCause = cause.WHY_OCCUR;
            action.RelatedCauseType = cause.CAUSE_TYPE;
            action.Action = action.ActionCode = "";
            action.VerifyStatus = "";
            action.EffDate = DateTime.UtcNow;

            return action;
        }

        public PROB_VERIFY CreateProblemVerify()
        {
            this.ProbCase.PROB_VERIFY = new PROB_VERIFY();
            this.ProbCase.PROB_VERIFY.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            this.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT = DateTime.UtcNow;  // mt - this should default to the step due date from the task assignements

            return this.ProbCase.PROB_VERIFY;
        }

        public PROB_CLOSE CreateProblemClose()
        {
            this.ProbCase.PROB_CLOSE = new PROB_CLOSE();
            this.ProbCase.PROB_CLOSE.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            this.ProbCase.PROB_CLOSE.CONCLUSIONS = "";
            return this.ProbCase.PROB_CLOSE;
        }

        public PROB_PREVENT CreateProblemPrevent(ProblemCase theCase)
        {
            this.ProbCase.PROB_PREVENT = new PROB_PREVENT();
            this.ProbCase.PROB_PREVENT.PROBCASE_ID = this.ProbCase.PROBCASE_ID;
            TASK_STATUS task = theCase.TeamTask.TaskList.Where(l => l.TASK_STEP == "7" && l.TASK_TYPE == "C").FirstOrDefault();

            int itemseq = 0;
            foreach (PLANT plant in SQMModelMgr.SelectPlantList(theCase.Entities, theCase.ProbCase.COMPANY_ID, 0))
            {
                PROB_PREVENT_LIST item = new PROB_PREVENT_LIST();
                item.PROBCASE_ID = theCase.ProbCase.PROBCASE_ID;
                item.PREVENT_ITEM = ++itemseq;
                item.PREVENT_ITEM_TYPE = "L";
                item.PREVENT_ITEM_REF = plant.PLANT_ID;
                if (task != null && task.DUE_DT.HasValue)
                    item.TARGET_DT = task.DUE_DT;
                else
                    item.TARGET_DT = DateTime.Now.AddDays(10);
                this.ProbCase.PROB_PREVENT_LIST.Add(item);
            }

            PROB_PREVENT_LIST docitem = new PROB_PREVENT_LIST();
            docitem.PROBCASE_ID = theCase.ProbCase.PROBCASE_ID;
            docitem.PREVENT_ITEM_TYPE = "D";
            docitem.PREVENT_ITEM = 1;
            if (task != null && task.DUE_DT.HasValue)
                docitem.TARGET_DT = task.DUE_DT;
            else
                docitem.TARGET_DT = DateTime.Now.AddDays(10);
            this.ProbCase.PROB_PREVENT_LIST.Add(docitem);

            return this.ProbCase.PROB_PREVENT;
        }

        public static PROB_CASE LookupCase(SQM.Website.PSsqmEntities ctx, decimal caseID)
        {
            PROB_CASE probCase = null;
            try
            {
                probCase = (from o in ctx.PROB_CASE.Include("PROB_OCCUR").Include("PROB_DEFINE").Include("PROB_CONTAIN").Include("PROB_CAUSE").Include("PROB_RISK").Include("PROB_VERIFY").Include("PROB_PREVENT").Include("PROB_CLOSE")
                                where (o.PROBCASE_ID == caseID)
                                select o).Single();

                if (probCase.PROB_CONTAIN != null)
                    probCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Load();
                if (probCase.PROB_CAUSE != null)
                {
                    probCase.PROB_CAUSE.PROB_CAUSE_STEP.Load();
                    if (probCase.PROB_CAUSE.PROB_CAUSE_STEP != null && probCase.PROB_CAUSE.PROB_CAUSE_STEP.Count > 0)
                        probCase.PROB_CAUSE_ACTION.Load();
                    if (probCase.PROB_VERIFY != null)
                    {
                        probCase.PROB_VERIFY.PROB_VERIFY_VERS.Load();
                    }
                }
                if (probCase.PROB_PREVENT != null)
                {
                    probCase.PROB_PREVENT_LIST.Load();
                }
            }
            catch (Exception e)
            {
              //  SQMLogger.LogException(e);
            }

            return probCase;
        }

        public static PROB_CASE LookupCaseByIncident(decimal incidentID)
        {
            PROB_CASE theCase = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    theCase = (from c in entities.PROB_CASE
                               join i in entities.PROB_OCCUR on c.PROBCASE_ID equals i.PROBCASE_ID into c_i
                                from i in c_i.DefaultIfEmpty()
                               where (i.INCIDENT_ID == incidentID)
                                select c).Single();
                }
                catch (Exception e)
                {
                    //   SQMLogger.LogException(e);
                }
            }

            return theCase;
        }

        public static INCIDENT LookupIncident(decimal incidentID)
        {
            INCIDENT incident = null;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    incident = (from i in entities.INCIDENT 
                                where (i.INCIDENT_ID == incidentID)
                                select i).Single();
                }
                catch (Exception e)
                {
                 //   SQMLogger.LogException(e);
                }
            }

            return incident;
        }

        private void LoadIncidentIssueInfo(decimal incidentID)
        {
            // for quality incidents, load the part/item identifications
            if (this.ProbCase.PROBCASE_TYPE == "QI")
            {
                QualityIssue qualityIssue = new QualityIssue().Load(incidentID);
                this.IssueList.Add(qualityIssue);
                if (qualityIssue.IssueOccur == null)
                {
                    qualityIssue.IssueOccur = new QI_OCCUR();
                }
                else
                {
                    foreach (QI_OCCUR_ITEM item in qualityIssue.IssueOccur.QI_OCCUR_ITEM)
                    {
                        PartIssueItem partItem = new PartIssueItem();
                        if (qualityIssue.Partdata != null && qualityIssue.Partdata.Part.PART_ID > 0)
                        {
                            partItem.PART_NUM = qualityIssue.Partdata.Part.PART_NUM;
                            partItem.LOT_NUM = item.LOT_NUM;
                            partItem.CONTAINER_NUM = item.CONTAINER_NUM;
                            partItem.NC_QTY = Math.Max(0, (decimal)item.INSPECT_NC_QTY);
                            this.PartIssueItemList.Add(partItem);
                        }
                    }
                }
            }
			else if (this.ProbCase.PROBCASE_TYPE == "EHS")
			{
                ;
			}
        }

        public static List<PROB_CASE> SelectProblemCaseList(decimal companyID, string caseType, string status)
        {
            List<PROB_CASE> caseList = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(caseType))
                        caseList = (from p in entities.PROB_CASE 
                                   where (p.COMPANY_ID == companyID)
                                   orderby p.PROBCASE_ID
                                   select p).ToList();
                    else
                        caseList = (from p in entities.PROB_CASE
                                    where (p.COMPANY_ID == companyID && p.PROBCASE_TYPE.ToUpper() == caseType.ToUpper())
                                    orderby p.PROBCASE_ID
                                    select p).ToList();

                    if (!String.IsNullOrEmpty(status))
                    {
                        if (status == "C")
                        {
                            caseList = caseList.FindAll(l => l.CLOSE_DT != null);
                        }
                        else
                        {
                            caseList = caseList.FindAll(l => l.STATUS.ToUpper() == status.ToUpper()  &&  l.CLOSE_DT == null);
                        }
                    }
                }
                catch (Exception ex)
                {
                  //  SQMLogger.LogException(ex);
                }
            }

            return caseList;
        }

		public static List<PROB_CASE> SelectUserCaseList(List<PROB_CASE> problemCaseList)
		{
			var userCaseList = new List<PROB_CASE>();

			using (PSsqmEntities entities = new PSsqmEntities())
            {
				try
				{
					foreach (PROB_CASE probCase in problemCaseList)
					{
						List<INCIDENT> incidentList = ProblemCase.LookupProbIncidentList(entities, probCase);
						foreach (INCIDENT incident in incidentList)
						{
							if (SessionManager.PlantAccess((decimal)incident.DETECT_PLANT_ID) || (incident.RESP_PLANT_ID.HasValue && SessionManager.PlantAccess((decimal)incident.RESP_PLANT_ID)))
								userCaseList.Add(probCase);
						}
					}
				}
				catch (Exception ex)
				{
					//  SQMLogger.LogException(ex);
				}
            }

			return userCaseList;
		}

        public static List<ProblemCase> QualifyCaseList(List<PROB_CASE> problemCaseList, decimal[] plantIDS)
        {
            var qualCaseList = new List<ProblemCase>();
            PLANT plant = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    foreach (PROB_CASE probCase in problemCaseList)
                    {
                        INCIDENT incident = ProblemCase.LookupProbIncidentList(entities, probCase).FirstOrDefault();
                        if (incident != null  &&  ((incident.DETECT_PLANT_ID.HasValue  &&  plantIDS.Contains((decimal)incident.DETECT_PLANT_ID))  ||  (incident.RESP_PLANT_ID.HasValue && plantIDS.Contains((decimal)incident.RESP_PLANT_ID)) ))
                        {
                            if (plant == null || plant.PLANT_ID != incident.DETECT_PLANT_ID)
                                plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
                            ProblemCase problemCase = new ProblemCase();
                            problemCase.IncidentList = new List<INCIDENT>();
                            problemCase.ProbCase = probCase;
                            problemCase.IncidentList.Add(incident);
                            problemCase.Plant = plant;
                            if (incident.INCIDENT_TYPE == "QI")
                            {
                                ;  // todo get qi_occur and part number
                            }
                            qualCaseList.Add(problemCase);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //  SQMLogger.LogException(ex);
                }
            }

            return qualCaseList;
        }

        public static List<INCIDENT> LookupProbIncidentList(PSsqmEntities ctx, PROB_CASE theCase)
        {
            List<INCIDENT> incidentList = new List<INCIDENT>();

            try 
            {
                incidentList = (from o in ctx.PROB_OCCUR
                                join i in ctx.INCIDENT on o.INCIDENT_ID equals i.INCIDENT_ID
                                where (o.PROBCASE_ID == theCase.PROBCASE_ID) select i).ToList();
            }
            catch 
            {
            }

            return incidentList;
        }

        public static ProblemCase UpdateProblemCase(ProblemCase theCase)
        {
            ProblemCase retCase = null;
            try
            {
                theCase.ProbCase= (PROB_CASE)SQMModelMgr.SetObjectTimestamp((object)theCase.ProbCase, SessionManager.UserContext.UserName(), CaseState(theCase));
                if (theCase.ProbCase.EntityState == System.Data.EntityState.Added || theCase.ProbCase.EntityState == System.Data.EntityState.Detached)
                {
                    theCase.Entities.AddToPROB_CASE(theCase.ProbCase);
                }
                if (!string.IsNullOrEmpty(theCase.StepComplete))
                {
                    theCase.TeamTask.SetTaskComplete(theCase.StepComplete, "C", 0, true);
                    if (theCase.StepComplete == "8" && theCase.ProbCase.CLOSE_DT.HasValue  == false)
                    {
                        DateTime closeDate = DateTime.UtcNow;
                        theCase.ProbCase.PROB_CLOSE.NOTIFY_DT = theCase.ProbCase.CLOSE_DT = closeDate;
                        theCase.ProbCase.PROB_CLOSE.STATUS = "C";
                        foreach (PROB_OCCUR occur in theCase.ProbCase.PROB_OCCUR)
                        {
                            theCase.Entities.ExecuteStoreCommand("UPDATE INCIDENT SET CLOSE_DATE_8D = {0} WHERE INCIDENT_ID = " + occur.INCIDENT_ID, closeDate);
                        }
                    }
                }
                int progress;

                if (theCase.ProbCase.PROB_CONTAIN != null)
                {
                    for (int n = 0; n < theCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Count; n++)
                    {
                        PROB_CONTAIN_ACTION contain = theCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.ElementAt(n);
                        if (contain.STATUS == "D")
                            theCase.Entities.DeleteObject(contain);
                    }
                }
                if (theCase.ProbCase.PROB_CAUSE_ACTION != null)
                {
                    for (int n = 0; n < theCase.ProbCase.PROB_CAUSE_ACTION.Count; n++)
                    {
                        PROB_CAUSE_ACTION action = theCase.ProbCase.PROB_CAUSE_ACTION.ElementAt(n);
                        if (action.STATUS == "D")
                            theCase.Entities.DeleteObject(action);
                    }
                }
                if (theCase.ProbCase.PROB_PREVENT != null)
                {
                    for (int n = 0; n < theCase.ProbCase.PROB_PREVENT_LIST.Count; n++)
                    {
                        PROB_PREVENT_LIST prevent = theCase.ProbCase.PROB_PREVENT_LIST.ElementAt(n);
                        if (prevent.CONFIRM_STATUS == "D")
                            theCase.Entities.DeleteObject(prevent);
                    }
                }

                theCase.ProbCase.PROGRESS = theCase.CheckCaseStatus(); // theCase.CheckCaseNextStep();
                
                theCase.Entities.SaveChanges();
                
                if (theCase.IsNew)
                {
                    if (SessionManager.DocumentContext == null)
                        SessionManager.DocumentContext = new SQM.Shared.DocumentScope().CreateNew(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "", SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0);
                    SQM.Website.Classes.SQMDocumentMgr.UpdateAttachmentRecordID(theCase.Entities, 21, SessionManager.DocumentContext.SessionID, theCase.ProbCase.PROBCASE_ID);
                }

                theCase.TeamTask.UpdateTaskList(theCase.ProbCase.PROBCASE_ID);

                theCase.UpdateStatus = CaseUpdateStatus.Success;
                retCase = theCase;
                retCase.IsNew = false;
                theCase.StepComplete = "";

            }
            catch (Exception e)
            {
                theCase.UpdateStatus = CaseUpdateStatus.SaveError;
                retCase = theCase;
               // SQMLogger.LogException(e);
            }
            return retCase;
        }


        public static int DeleteProblemCase(decimal probcaseID)
        {
            int status = 0;
            string delCmd = " IN ("+probcaseID.ToString()+") ";

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                try
                {
                    status = ctx.ExecuteStoreCommand("DELETE FROM TASK WHERE TASKLIST_ID IN (SELECT TASKLIST_ID FROM TASKLIST WHERE RECORD_TYPE = 21 AND RECORD_ID " + delCmd + " )");
                    status = ctx.ExecuteStoreCommand("DELETE FROM TASKLIST WHERE RECORD_TYPE = 21 AND RECORD_ID " + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE FROM PROB_CLOSE WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_PREVENT_LIST WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_PREVENT WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_VERIFY_VERS WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_VERIFY WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CAUSE_ACTION_VERS WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CAUSE_ACTION WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CAUSE_STEP WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CAUSE WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CONTAIN_ACTION WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CONTAIN WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE FROM PROB_RISK WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_DEFINE WHERE PROBCASE_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_OCCUR WHERE PROBCASE_ID" + delCmd);

                    status = ctx.ExecuteStoreCommand("DELETE  FROM PROB_CASE WHERE PROBCASE_ID" + delCmd);
                }
                catch (Exception ex)
                {
                   // SQMLogger.LogException(ex);
                }
            }

            return status;
        }


        public PROB_VERIFY_VERS AddVerifyTrial()
        {
            PROB_VERIFY_VERS newTrial = new PROB_VERIFY_VERS();
            newTrial = (PROB_VERIFY_VERS)SQMModelMgr.CopyObjectValues(newTrial, this.ProbCase.PROB_VERIFY, false);
            newTrial.VERIFY_TRIAL_NO = this.ProbCase.PROB_VERIFY.PROB_VERIFY_VERS.Count + 1;
            this.Entities.AddToPROB_VERIFY_VERS(newTrial);
            foreach (PROB_CAUSE_ACTION action in this.ProbCase.PROB_CAUSE_ACTION)
            {
                PROB_CAUSE_ACTION_VERS trial = new PROB_CAUSE_ACTION_VERS();
                trial = (PROB_CAUSE_ACTION_VERS)SQMModelMgr.CopyObjectValues(trial, action, false);
                trial.VERIFY_TRIAL_NO = newTrial.VERIFY_TRIAL_NO;
                this.Entities.AddToPROB_CAUSE_ACTION_VERS(trial);
            }
            try
            {
                this.Entities.SaveChanges();
            }
            catch (Exception e)
            {
              //  SQMLogger.LogException(e);
                newTrial = null;
            }

            return newTrial;
        }

        private static System.Data.EntityState CaseState(ProblemCase theCase)
        {
            // check if any parts of the case have changed.  do this to ensure we update the case0 record time stamp when children are updated
            System.Data.EntityState theState = theCase.ProbCase.EntityState;

            if (theCase.ProbCase.EntityState != System.Data.EntityState.Unchanged)
                return theState;
            if (theCase.ProbCase.PROB_DEFINE != null  &&  theCase.ProbCase.PROB_DEFINE.EntityState != System.Data.EntityState.Unchanged)
                theState = theCase.ProbCase.PROB_DEFINE.EntityState;
            if (theCase.ProbCase.PROB_CONTAIN != null && theCase.ProbCase.PROB_CONTAIN.EntityState != System.Data.EntityState.Unchanged)
                theState = theCase.ProbCase.PROB_CONTAIN.EntityState;
            if (theCase.ProbCase.PROB_CAUSE != null && theCase.ProbCase.PROB_CAUSE.EntityState != System.Data.EntityState.Unchanged)
                theState = theCase.ProbCase.PROB_CAUSE.EntityState;
            if (theCase.ProbCase.PROB_CAUSE_ACTION != null  && theCase.ProbCase.PROB_CAUSE_ACTION.Count > 0)
            {
                foreach (PROB_CAUSE_ACTION action in theCase.ProbCase.PROB_CAUSE_ACTION)
                {
                    if (action.EntityState != System.Data.EntityState.Unchanged)
                        return action.EntityState;
                }
            }
            if (theCase.ProbCase.PROB_VERIFY != null && theCase.ProbCase.PROB_VERIFY.EntityState != System.Data.EntityState.Unchanged)
                theState = theCase.ProbCase.PROB_VERIFY.EntityState;

            return theState;
        }

        public PROB_DEFINE UpdateProblemDefinition(PROB_DEFINE prDefine)
        {
			foreach (INCIDENT incident in this.IncidentList)
			{
				if (this.ProbCase.PROBCASE_TYPE == "QI")
				{

					QualityIssue qualityIssue = new QualityIssue().Load(incident.INCIDENT_ID);
					string str = "";

					// who reported the problem
					str = incident.CREATE_BY;
					if (incident.CREATE_PERSON != null && incident.CREATE_PERSON > 0)
					{
						PERSON person = SQMModelMgr.LookupPerson((decimal)incident.CREATE_PERSON, "");
						COMPANY company = SQMModelMgr.LookupCompany(person.COMPANY_ID);
						str += " (" + company.COMPANY_NAME + ")";
					}
					if (string.IsNullOrEmpty(prDefine.WHO_IS) || !prDefine.WHO_IS.Contains(str))
					{
						if (!string.IsNullOrEmpty(prDefine.WHO_IS))
							prDefine.WHO_IS += ", ";
						prDefine.WHO_IS += str;
					}

					// where did the problem occur
					str = qualityIssue.DetectedLocation.Company.COMPANY_NAME;
					if (incident.DETECT_BUS_ORG_ID != null && incident.DETECT_BUS_ORG_ID > 0)
						str += (" /  " + qualityIssue.DetectedLocation.BusinessOrg.ORG_NAME);
					if (incident.DETECT_PLANT_ID != null && incident.DETECT_PLANT_ID > 0)
						str += (" / " + qualityIssue.DetectedLocation.Plant.PLANT_NAME);
					if (string.IsNullOrEmpty(prDefine.WHERE_IS) || !prDefine.WHERE_IS.Contains(str))
					{
						if (!string.IsNullOrEmpty(prDefine.WHERE_IS))
							prDefine.WHERE_IS += ", ";
						prDefine.WHERE_IS += str;
					}


					// where detected
					str = WebSiteCommon.GetXlatValueLong("issueResponsible", qualityIssue.IssueOccur.SOURCE);
					if (string.IsNullOrEmpty(prDefine.DETECTED_IS) || !prDefine.DETECTED_IS.Contains(str))
					{
						if (!string.IsNullOrEmpty(prDefine.DETECTED_IS))
							prDefine.DETECTED_IS += ", ";
						prDefine.DETECTED_IS += str;
					}

					// who or where is impacted
					if (!string.IsNullOrEmpty(prDefine.IMPACT_IS))
						prDefine.IMPACT_IS += ", ";

					prDefine.IMPACT_IS += WebSiteCommon.GetXlatValueLong("issueResponsible", qualityIssue.IssueOccur.SOURCE);

					// when did the problem occur
					str = WebSiteCommon.FormatDateString(WebSiteCommon.LocalTime(incident.INCIDENT_DT, SessionManager.UserContext.TimeZoneID), false);
					if (string.IsNullOrEmpty(prDefine.WHEN_IS) || !prDefine.WHEN_IS.Contains(str))
					{
						if (!string.IsNullOrEmpty(prDefine.WHEN_IS))
							prDefine.WHEN_IS += ", ";
						prDefine.WHEN_IS += str;
					}

					// what is the problem
					if (!string.IsNullOrEmpty(prDefine.WHAT_IS))
						prDefine.WHAT_IS += ", ";
					prDefine.WHAT_IS += (" " + qualityIssue.Partdata.Part.PART_NUM + "(" + qualityIssue.Partdata.Part.PART_NAME + ")");

					// how many how often detected 
					double qty = 0;
					if (!string.IsNullOrEmpty(prDefine.HOW_MANY))
						qty = double.Parse(prDefine.HOW_MANY);
					foreach (QI_OCCUR_ITEM item in qualityIssue.IssueOccur.QI_OCCUR_ITEM)
					{
						qty += Convert.ToDouble(item.INSPECT_NC_QTY);
						foreach (QI_OCCUR_NC sample in item.QI_OCCUR_NC)
						{
							if (!string.IsNullOrEmpty(prDefine.NC_IS))
								prDefine.NC_IS += ", ";
							NONCONFORMANCE nc = SQMResourcesMgr.LookupNonconf(this.Entities, (decimal)sample.NONCONF_ID, "");
							if (nc != null)
								prDefine.NC_IS += (qualityIssue.IssueOccur.OCCUR_DESC + ": " + nc.NONCONF_DESC);
						}
					}
					prDefine.HOW_MANY = qty.ToString();

					str = qualityIssue.IssueOccur.OCCUR_DESC;
					if (string.IsNullOrEmpty(prDefine.WHY_IS) || !prDefine.WHY_IS.Contains(str))
					{
						if (!string.IsNullOrEmpty(prDefine.WHY_IS))
							prDefine.WHY_IS += ", ";
						prDefine.WHY_IS += str;
					}

					prDefine.URGENT_IS = prDefine.MEASURE_IS = prDefine.OFTEN_IS = "TBD";
				}
			}

            return prDefine;
        }

        public int CheckCaseNextStep()
        {
            int nextStep = 0;

            if (this.IsNew)
                nextStep = 0;
            else
            {
                nextStep = 1;   // team
                if (this.ProbCase.PROB_OCCUR != null  &&  this.ProbCase.PROB_OCCUR.Count > 0)
                    nextStep = 2;  // definition
                if (this.ProbCase.PROB_DEFINE != null  &&  this.ProbCase.PROB_DEFINE.EntityState != System.Data.EntityState.Added)
                    nextStep = 3;   // containment
                if (this.ProbCase.PROB_CONTAIN != null && this.ProbCase.PROB_CONTAIN.EntityState != System.Data.EntityState.Added)
                    nextStep = 4;     // root cause
                if (this.ProbCase.PROB_CAUSE != null && this.ProbCase.PROB_CAUSE.EntityState != System.Data.EntityState.Added)
                    nextStep = 5;     // corrective action
                if (this.ProbCase.PROB_CAUSE_ACTION != null &&  this.ProbCase.PROB_CAUSE_ACTION.Count > 0)
                    nextStep = 6;     // verification
                if (this.ProbCase.PROB_VERIFY != null)
                    nextStep = 7;  // prevention
                if (this.ProbCase.PROB_PREVENT != null)
                    nextStep = 8;  // summary
            }

            return nextStep;
        }

        public int CheckCaseStatus()
        {
            int isComplete= 0;

            if (this.TeamTask != null)
            {
                foreach (TASK_STATUS task in this.TeamTask.TaskList)
                {
                    switch (TaskMgr.CalculateTaskStatus(task))
                    {
                        case TaskStatus.Complete:
                            ++isComplete;
                            break;
                        default:
                            break;
                    }
                }
            }

            return isComplete;
        }

        public int UpdateContainmentRisk(int severityFactor, int occurFactor, int detectFactor)
        {
            // td - 
            int riskIndex = 0;

            riskIndex = (severityFactor * occurFactor * detectFactor);

            return riskIndex;
        }


        public List<SQM.Shared.CorrectiveAction> CreateActionList()
        {
            // correlate actions with causes.  todo: reconcile deleted causes if/when that occurs in step 4
            List<SQM.Shared.CorrectiveAction> actionList = new List<SQM.Shared.CorrectiveAction>();
            if (this.ProbCase.PROB_CAUSE != null && this.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP != null)
            {
                foreach (PROB_CAUSE_STEP cause in this.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP)
                {
                    if (cause.IS_ROOTCAUSE.HasValue && (bool)cause.IS_ROOTCAUSE == true)
                    {
                        foreach (PROB_CAUSE_ACTION existingAction in this.ProbCase.PROB_CAUSE_ACTION)
                        {
                            if (existingAction.CAUSE_NO == cause.ITERATION_NO)
                            {
                                SQM.Shared.CorrectiveAction action = new SQM.Shared.CorrectiveAction();
                                action.RelatedCauseNo = cause.ITERATION_NO;
                                action.RelatedCause = cause.WHY_OCCUR;
                                action.ActionNo = existingAction.ACTION_NO;
                                action.Action = existingAction.ACTION_DESC;
                                action.ActionCode = existingAction.ACTION_CD;
                                action.EffDate = Convert.ToDateTime(existingAction.EFF_DT);
                                action.Responsible1 = existingAction.RESPONSIBLE1;
                                action.Responsible2 = existingAction.RESPONSIBLE2;
                                action.VerifyStatus = existingAction.VERIFY_STATUS;
                                action.VerifyObservations = existingAction.VERIFY_OBSERVATIONS;
                                actionList.Add(action);
                            }
                        }
                    }
                }
            }

            actionList = OrderActionList(actionList);
            return actionList;
        }

        public List<SQM.Shared.CorrectiveAction> OrderActionList(List<SQM.Shared.CorrectiveAction> inputList)
        {
            List<SQM.Shared.CorrectiveAction> actionList = new List<SQM.Shared.CorrectiveAction>();
            actionList.AddRange(inputList.OrderBy(l => l.RelatedCauseNo).ThenBy(l => l.ActionNo));
            return actionList;
        }
    }
}