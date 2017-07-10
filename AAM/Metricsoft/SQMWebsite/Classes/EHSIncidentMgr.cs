using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
    public enum IncidentMode
    {
        Incident,
        Prevent
    }

    public enum IncidentStepStatus { unknown = 0, defined = 100, workstatus = 105, containment = 110, containmentComplete = 115, rootcause = 120, rootcauseComplete = 125, correctiveaction = 130, correctiveactionComplete = 135, signoff = 150, signoff1 = 151, signoff2 = 152, signoff3 = 153, signoff4 = 154, signoffComplete = 155, awaitingFunding = 156 }

    public class EHSIncidentQuestion
    {
        public decimal QuestionId { get; set; }
        public string QuestionText { get; set; }
        public EHSIncidentQuestionType QuestionType { get; set; }
        public bool HasMultipleChoices { get; set; }
        public List<EHSMetaData> AnswerChoices { get; set; }
        public bool IsRequired { get; set; }
        public bool IsRequiredClose { get; set; }
        public string HelpText { get; set; }
        public string AnswerText { get; set; }
        public string StandardType { get; set; }
        public List<INCIDENT_QUESTION_CONTROL> QuestionControls { get; set; }
    }

    public class EHSIncidentAnswerChoice
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool IsCategoryHeading { get; set; }
    }

    public class EHSIncidentComment
    {
        public string PersonName { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
    }


    [Serializable]
    public class EHSIncidentApproval
    {
        public INCFORM_APPROVAL approval { get; set; }
        public INCFORM_STEP_PRIV stepPriv { get; set; }
    }


    public class EHSIncidentTimeAccounting
    {
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public decimal PlantID { get; set; }
        public decimal IncidentType { get; set; }
        public int NearMiss { get; set; }
        public int FirstAidCase { get; set; }
        public int RecordableCase { get; set; }
        public int LostTimeCase { get; set; }
        public int FatalityCase { get; set; }
        public int OtherCase { get; set; }
        public decimal LostTime { get; set; }
        public decimal RestrictedTime { get; set; }
        public decimal WorkTime { get; set; }

        public EHSIncidentTimeAccounting CreateNew(int periodYear, int periodMonth, decimal incidentType, decimal plantID)
        {
            this.PeriodYear = periodYear;
            this.PeriodMonth = periodMonth;
            this.IncidentType = incidentType;
            this.PlantID = plantID;
            this.LostTime = this.RestrictedTime = this.WorkTime = 0;
            this.NearMiss = this.FirstAidCase = this.RecordableCase = LostTimeCase = FatalityCase = OtherCase = 0;
            return this;
        }
    }

    [Serializable]
    public class RootCauseSeries
    {
        public int ProblemSeq
        {
            get;
            set;
        }
        public string ProblemStatement
        {
            get;
            set;
        }
        public List<INCFORM_ROOT5Y> RootCauseList
        {
            get;
            set;
        }
    }

    [Serializable]
    public class IncidentAlertItem
    {
        public TASK_STATUS Task
        {
            get;
            set;
        }
        public PLANT Location
        {
            get;
            set;
        }
        public PERSON Person
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ActionType
    {
        public string ActionTypeCode
        {
            get;
            set;
        }
        public string ActionDescription
        {
            get;
            set;
        }
        public int ActionSeq
        {
            get;
            set;
        }
        public string Criteria
        {
            get;
            set;
        }
        public bool IsSelected
        {
            get;
            set;
        }
    }

    public class EHSIncidentData
    {
        public INCFORM_APPROVAL Approval { get; set; }
        public INCIDENT Incident
        {
            get;
            set;
        }
        public PERSON Person
        {
            get;
            set;
        }
        public PERSON RespPerson
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public List<INCIDENT_QUESTION> TopicList
        {
            get;
            set;
        }
        public List<INCIDENT_ANSWER> EntryList
        {
            get;
            set;
        }
        public INCFORM_INJURYILLNESS InjuryDetail
        {
            get;
            set;
        }
        public PROB_CASE ProbCase
        {
            get;
            set;
        }
        public List<ATTACHMENT> AttachList
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string DeriveStatus()
        {
            this.Status = "A"; // open;

            if (this.Incident.ISSUE_TYPE_ID == 13)
            {
                switch (this.Incident.INCFORM_LAST_STEP_COMPLETED)
                {
                    case 130:
                        this.Status = "P";  // in progress
                        break;
                    case 135:
                        this.Status = "C";  // closed
                        break;
                    case 151:
                    case 152:
                    case 155:
                        this.Status = "U";  // audited
                        if (this.EntryList.Where(l => l.INCIDENT_QUESTION_ID == (int)EHSQuestionId.FinalAuditStepResolved && l.ANSWER_VALUE.ToLower().Contains("fund")).Count() > 0)
                            this.Status = "F";   // awaiting funding
                        break;
                    default:
                        this.Status = "A";  // open/new
                        break;
                }
            }
            else
            {
                if (this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue && this.Incident.CLOSE_DATE.HasValue)
                    this.Status = "C";  // incident closed
                else
                    this.Status = "A";  // incident open
            }

            return this.Status;
        }

        public bool IsDependentStatus(decimal? dependentStatus)
        {
            return EHSIncidentMgr.IsDependentStatus(this.Incident, dependentStatus);
        }

        public int DaysOpen
        {
            get;
            set;
        }
        public int DaysToClose
        {
            get;
            set;
        }
        public int DaysElapsed()
        {
            int days = 0;
            if (this.Incident.ISSUE_TYPE_ID == 13)
            {
                if (this.Incident.CLOSE_DATE.HasValue)
                {
                    DateTime closeDT = Convert.ToDateTime(this.Incident.CLOSE_DATE);
                    days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract((DateTime)this.Incident.CREATE_DT).TotalDays));
                    this.DaysOpen = 0;
                }
                else
                {
                    days = this.DaysOpen = (int)Math.Abs(Math.Truncate(WebSiteCommon.LocalTime(DateTime.UtcNow, this.Plant.LOCAL_TIMEZONE).Subtract((DateTime)this.Incident.CREATE_DT).TotalDays));
                    this.DaysToClose = 0;
                }
            }
            else
            {
                if (this.Incident.CLOSE_DATE.HasValue)
                {
                    DateTime closeDT = Convert.ToDateTime(this.Incident.CLOSE_DATE);
                    days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract(this.Incident.INCIDENT_DT).TotalDays));
                    this.DaysOpen = 0;
                }
                else
                {
                    days = this.DaysOpen = (int)Math.Abs(Math.Truncate(WebSiteCommon.LocalTime(DateTime.UtcNow, this.Plant.LOCAL_TIMEZONE).Subtract(this.Incident.INCIDENT_DT).TotalDays));
                    this.DaysToClose = 0;
                }
            }

            return days;
        }

        public bool MatchSeverity(List<string> severityList)
        {
            bool status = severityList == null || severityList.Count == 0 ? true : false;

            if (this.InjuryDetail != null)
            {
                foreach (string severity in severityList)
                {
                    switch (severity)
                    {
                        case "FIRSTAID":
                            if (this.InjuryDetail.FIRST_AID == true)
                                status = true;
                            break;
                        case "RECORDABLE":
                            if (this.InjuryDetail.RECORDABLE == true)
                                status = true;
                            break;
                        case "LOSTTIME":
                            if (this.InjuryDetail.LOST_TIME == true)
                                status = true;
                            break;
                        case "RESTRICTEDTIME":
                            if (this.InjuryDetail.RESTRICTED_TIME == true)
                                status = true;
                            break;
                        case "FATALITY":
                            if (this.InjuryDetail.FATALITY == true)
                                status = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            return status;
        }

        public EHSIncidentTimeAccounting IncidentAccounting(int workdays)
        {
            EHSIncidentTimeAccounting incidentAccounting = new EHSIncidentTimeAccounting();

            foreach (EHSIncidentTimeAccounting period in EHSIncidentMgr.CalculateIncidentAccounting(this.Incident, this.Plant.LOCAL_TIMEZONE, workdays))
            {
                incidentAccounting.LostTimeCase = Math.Max(incidentAccounting.LostTimeCase, period.LostTimeCase);
                incidentAccounting.FirstAidCase = Math.Max(incidentAccounting.FirstAidCase, period.FirstAidCase);
                incidentAccounting.RecordableCase = Math.Max(incidentAccounting.RecordableCase, period.RecordableCase);
                incidentAccounting.LostTime += period.LostTime;
                incidentAccounting.RestrictedTime += period.RestrictedTime;
                incidentAccounting.WorkTime += period.WorkTime;
            }

            return incidentAccounting;
        }
    }

    public static class EHSIncidentMgr
    {
        public static List<INCFORM_TYPE_CONTROL> SelectIncidentSteps(PSsqmEntities entities, decimal typeID)
        {
            List<INCFORM_TYPE_CONTROL> incidentStepList = new List<INCFORM_TYPE_CONTROL>();

            if (typeID == -1)   // get all
                incidentStepList = (from s in entities.INCFORM_TYPE_CONTROL select s).ToList();
            else
            {
                // get specific
                incidentStepList = (from s in entities.INCFORM_TYPE_CONTROL where s.INCIDENT_TYPE_ID == (decimal)typeID select s).ToList();
                if (incidentStepList.Count == 0)
                {
                    // if specific incident type not found, get common (0)
                    incidentStepList = (from s in entities.INCFORM_TYPE_CONTROL where s.INCIDENT_TYPE_ID == 0 select s).ToList();
                }
            }

            return incidentStepList;
        }

        public static List<INCFORM_TYPE_CONTROL> GetIncidentSteps(List<INCFORM_TYPE_CONTROL> stepList, decimal typeID)
        {
            List<INCFORM_TYPE_CONTROL> incidentStepList = new List<INCFORM_TYPE_CONTROL>();

            incidentStepList = stepList.Where(l => l.INCIDENT_TYPE_ID == typeID).ToList();
            if (incidentStepList.Count == 0)
            {
                incidentStepList = stepList.Where(l => l.INCIDENT_TYPE_ID == 0).ToList();
            }

            return incidentStepList;
        }

        public static bool IsStepActive(List<INCFORM_TYPE_CONTROL> stepList, decimal typeID, decimal step)
        {
            bool isActive = false;

            isActive = GetIncidentSteps(stepList, typeID).Where(l => l.STEP == step).Count() > 0 ? true : false;

            return isActive;
        }

        public static INCIDENT SelectIncidentById(PSsqmEntities entities, decimal incidentId)
        {
            return (SelectIncidentById(entities, incidentId, false));
        }

        public static INCIDENT SelectIncidentById(PSsqmEntities entities, decimal incidentId, bool loadChildren)
        {
            INCIDENT incident = null;

            try
            {
                incident = (from i in entities.INCIDENT.Include("INCFORM_INJURYILLNESS") where i.INCIDENT_ID == incidentId select i).FirstOrDefault();

                if (loadChildren)
                {
                    incident.INCFORM_CONTAIN.Load();
                    incident.INCFORM_ROOT5Y.Load();
                    incident.INCFORM_CAUSATION.Load();
                    incident.INCFORM_APPROVAL.Load();
                }
            }
            catch
            {
            }

            return incident;
        }

        public static INCIDENT_TYPE SelectIncidentType(decimal incidentTypeID, string nlsLanguage)
        {
            INCIDENT_TYPE inType = new INCIDENT_TYPE();

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                inType = (from t in ctx.INCIDENT_TYPE where t.INCIDENT_TYPE_ID == incidentTypeID select t).SingleOrDefault();

                if (!string.IsNullOrEmpty(nlsLanguage) && nlsLanguage != "en")
                {
                    INCIDENT_TYPE_LANG lang = (from t in ctx.INCIDENT_TYPE_LANG where t.NLS_LANGUAGE == nlsLanguage && t.INCIDENT_TYPE_ID == incidentTypeID select t).SingleOrDefault();
                    if (lang != null)
                        inType.TITLE = lang.LANG_TEXT;
                }
            }

            return inType;
        }
                
        //Function to fetch value of severity level.
        public static List<XLAT> PopulateSeverityLevel()
        {
            List<XLAT> lstSeverityLevel = new List<XLAT>();
            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[1] { "HS_L2REPORT" }, 1);
                var severityLevel = new string[] { "l1", "l2", "l3", "l4", "first_add" };
                lstSeverityLevel = xlatList.Where(l => l.XLAT_GROUP == "HS_L2REPORT" && severityLevel.Contains(l.XLAT_CODE)).ToList();
            }
            return lstSeverityLevel;
        }
        public static decimal SelectIncidentTypeIdByIncidentId(decimal incidentId)
        {
            decimal? incidentTypeId;
            var entities = new PSsqmEntities();
            incidentTypeId = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i.ISSUE_TYPE_ID).FirstOrDefault();
            if (incidentTypeId == null)
                incidentTypeId = 0;
            return (decimal)incidentTypeId;
        }

        public static string SelectIncidentTypeByIncidentId(decimal incidentId)
        {
            string incidentType = "";
            var entities = new PSsqmEntities();
            decimal incidentTypeId = SelectIncidentTypeIdByIncidentId(incidentId);
            incidentType = (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == incidentTypeId select it.TITLE).FirstOrDefault();
            return incidentType;
        }

        public static decimal SelectProblemCaseIdByIncidentId(decimal incidentId)
        {
            decimal? problemCaseId;
            var entities = new PSsqmEntities();
            problemCaseId = (from po in entities.PROB_OCCUR where po.INCIDENT_ID == incidentId select po.PROBCASE_ID).FirstOrDefault();
            if (problemCaseId == null)
                problemCaseId = 0;
            return (decimal)problemCaseId;
        }

        public static bool IsDependentStatus(INCIDENT incident, decimal? dependentStatus)
        {
            bool status = false;

            if (!dependentStatus.HasValue || dependentStatus == 0)
                return true;

            if ((dependentStatus % 1) == 0)
            {
                if (incident.INCFORM_LAST_STEP_COMPLETED >= (int)dependentStatus)
                {
                    status = true;
                }
            }
            else
            {
                if (incident.LAST_APPROVAL_STEP.HasValue && incident.LAST_APPROVAL_STEP >= dependentStatus)
                {
                    status = true;
                }
            }

            return status;
        }

        public static int UpdateIncidentApprovalStatus(decimal incidentID, decimal approvalStep)
        {
            int status = -1;
            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                INCIDENT incident = (from i in ctx.INCIDENT where i.INCIDENT_ID == incidentID select i).SingleOrDefault();
                if (incident != null)
                {
                    if (incident.LAST_APPROVAL_STEP.HasValue == false || approvalStep > incident.LAST_APPROVAL_STEP)
                    {
                        incident.LAST_APPROVAL_STEP = approvalStep;
                        status = ctx.SaveChanges();
                    }
                }
            }

            return status;
        }

        public static IncidentStepStatus UpdateIncidentStatus(decimal incidentID, IncidentStepStatus currentStepStatus, DateTime? defaultDate)
        {
            return UpdateIncidentStatus(incidentID, currentStepStatus, false, defaultDate);
        }


        public static IncidentStepStatus UpdateIncidentStatus(decimal incidentID, IncidentStepStatus currentStepStatus, bool closeIncident, DateTime? defaultDate)
        {
            INCIDENT incident = null;
            bool isUpdated = false;
            IncidentStepStatus calcStatus = IncidentStepStatus.unknown;
            bool closeByApproval = false;
            string localTZ = "";

            SETTINGS sets = SQMSettings.GetSetting("EHS", "INCIDENT_APPROVALS");
            int finalApprovalLevel = (int)SysPriv.approve2;  // default approval level requiured to close incident
            if (!string.IsNullOrEmpty(sets.VALUE.Split(',').LastOrDefault()))
            {
                if (int.TryParse(sets.VALUE.Split(',').Last(), out finalApprovalLevel) == false)
                    finalApprovalLevel = (int)SysPriv.approve2;
            }

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                incident = (from i in ctx.INCIDENT where i.INCIDENT_ID == incidentID select i).SingleOrDefault();
                if (incident != null)
                {
                    calcStatus = IncidentStepStatus.defined;

                    if ((from c in ctx.INCFORM_CONTAIN where c.INCIDENT_ID == incidentID select c).Count() > 0)
                    {
                        calcStatus = IncidentStepStatus.containment;
                    }
                    if ((from c in ctx.INCFORM_ROOT5Y where c.INCIDENT_ID == incidentID select c).Count() > 0)
                    {
                        calcStatus = IncidentStepStatus.rootcause;
                    }
                    if ((from c in ctx.INCFORM_CAUSATION where c.INCIDENT_ID == incidentID select c).Count() > 0)
                    {
                        calcStatus = IncidentStepStatus.rootcauseComplete;
                    }
                    List<TASK_STATUS> actionList = (from c in ctx.TASK_STATUS where c.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident && c.RECORD_ID == incidentID select c).ToList();
                    if (actionList != null && actionList.Count() > 0)
                    {
                        calcStatus = IncidentStepStatus.correctiveaction;
                        if (actionList.Where(l => l.COMPLETE_DT != null).ToList().Count > 0)
                        {
                            calcStatus = IncidentStepStatus.correctiveactionComplete;
                        }
                    }
                    List<INCFORM_APPROVAL> approvalList = (from c in ctx.INCFORM_APPROVAL where c.INCIDENT_ID == incidentID select c).ToList();
                    {
                        if (approvalList != null && approvalList.Where(l => l.ITEM_SEQ == (int)SysPriv.approve1).ToList().Count > 0)
                        {
                            closeByApproval = finalApprovalLevel == (int)SysPriv.approve1 ? true : closeByApproval;
                            if (closeByApproval)
                                calcStatus = IncidentStepStatus.signoff2;
                            else
                                calcStatus = IncidentStepStatus.signoff1;
                        }
                        if (approvalList != null && approvalList.Where(l => l.ITEM_SEQ == (int)SysPriv.approve2).ToList().Count > 0)
                        {
                            closeByApproval = finalApprovalLevel == (int)SysPriv.approve2 ? true : closeByApproval;
                            calcStatus = IncidentStepStatus.signoff2;
                        }

                        if ((closeIncident || closeByApproval))
                        {
                            PLANT plant = SQMModelMgr.LookupPlant(ctx, (decimal)incident.DETECT_PLANT_ID, "");
                            incident.CLOSE_DATE = incident.CLOSE_DATE_DATA_COMPLETE = defaultDate != null ? defaultDate : DateTime.UtcNow;
                            calcStatus = IncidentStepStatus.signoffComplete;
                            incident.LAST_APPROVAL_STEP = 10.0m;
                            isUpdated = true;
                        }
                    }

                    if (calcStatus != IncidentStepStatus.unknown)
                    {
                        incident.INCFORM_LAST_STEP_COMPLETED = (int)calcStatus;
                        int status = ctx.SaveChanges();
                    }
                }
            }

            return calcStatus;
        }

        public static List<INCIDENT_TYPE> SelectIncidentTypeList(decimal companyId)
        {
            return SelectIncidentTypeList(companyId, "en");
        }

        public static List<INCIDENT_TYPE> SelectIncidentTypeList(decimal companyId, string nlsLanguage)
        {
            var incidentTypeList = new List<INCIDENT_TYPE>();

            try
            {
                var entities = new PSsqmEntities();

                if (companyId > 0)
                    incidentTypeList = (from itc in entities.INCIDENT_TYPE_COMPANY
                                        join it in entities.INCIDENT_TYPE on itc.INCIDENT_TYPE_ID equals it.INCIDENT_TYPE_ID
                                        where itc.COMPANY_ID == companyId && itc.STATUS != "I" && itc.SORT_ORDER < 1000
                                        orderby it.TITLE
                                        select it).ToList();
                else
                {
                    incidentTypeList = (from itc in entities.INCIDENT_TYPE
                                        where itc.STATUS != "I"
                                        orderby itc.TITLE
                                        select itc).ToList();
                }

                if (!string.IsNullOrEmpty(nlsLanguage) && nlsLanguage != "en")
                {
                    List<INCIDENT_TYPE_LANG> langList = (from l in entities.INCIDENT_TYPE_LANG
                                                         where l.NLS_LANGUAGE == nlsLanguage
                                                         select l).ToList();
                    foreach (var item in incidentTypeList)
                    {
                        if (langList.Where(l => l.INCIDENT_TYPE_ID == item.INCIDENT_TYPE_ID).FirstOrDefault() != null)
                        {
                            item.TITLE = langList.Where(l => l.INCIDENT_TYPE_ID == item.INCIDENT_TYPE_ID).First().LANG_TEXT;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }
            return incidentTypeList;
        }


        public static List<INCIDENT_TYPE> SelectPreventativeTypeList(decimal companyId)
        {
            var preventativeTypeList = new List<INCIDENT_TYPE>();

            try
            {
                var entities = new PSsqmEntities();

                preventativeTypeList = (from itc in entities.INCIDENT_TYPE_COMPANY
                                        join it in entities.INCIDENT_TYPE on itc.INCIDENT_TYPE_ID equals it.INCIDENT_TYPE_ID
                                        where itc.COMPANY_ID == companyId && itc.SORT_ORDER >= 1000
                                        orderby it.TITLE
                                        select it).ToList();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return preventativeTypeList;
        }


        public static bool CanUpdateIncident(INCIDENT incident, bool IsEditContext, SysPriv privNeeded, int stepCompleted)
        {
            return CanUpdateIncident(incident, IsEditContext, privNeeded, stepCompleted, true);
        }

        public static bool CanUpdateIncident(INCIDENT incident, bool IsEditContext, SysPriv privNeeded, int stepCompleted, bool checkIfClosed)
        {
            bool canUpdate = false;

            if (IsEditContext)
            {
                if ((incident != null && incident.CREATE_PERSON == SessionManager.UserContext.Person.PERSON_ID) || SessionManager.CheckUserPrivilege(privNeeded, SysScope.incident))
                {
                    canUpdate = true;
                }

                if (checkIfClosed)
                {
                    if (incident != null && incident.CLOSE_DATE.HasValue)
                    {
                        canUpdate = false;
                    }
                }
            }
            else  // assume edit context will be false for new incidents an any user can create/save a new incident
            {
                canUpdate = true;
            }

            return canUpdate;
        }

        public static bool IsIncidentClosed(INCIDENT incident)
        {
            bool status = incident != null && incident.CLOSE_DATE.HasValue ? true : false;

            return status;
        }


        public static bool CanDeleteIncident(decimal createPersonID, int stepCompleted)
        {
            bool canDelete = false;

            if (UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.incident) ||
                UserContext.CheckUserPrivilege(SysPriv.approve1, SysScope.incident) ||
                UserContext.CheckUserPrivilege(SysPriv.approve2, SysScope.incident) ||
                UserContext.CheckUserPrivilege(SysPriv.approve3, SysScope.incident) ||
                UserContext.CheckUserPrivilege(SysPriv.approve4, SysScope.incident) ||
                UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident) ||
                SessionManager.UserContext.Person.PERSON_ID == createPersonID)
            {
                canDelete = true;
            }

            return canDelete;
        }


        /// <summary>
        /// Select a list of all EHS incidents by company
        /// </summary>
        public static List<INCIDENT> SelectIncidents(decimal companyId, List<decimal> plantIdList)
        {
            var incidents = new List<INCIDENT>();

            try
            {
                var entities = new PSsqmEntities();
                if (plantIdList == null)
                {
                    incidents = (from i in entities.INCIDENT
                                 where i.INCIDENT_TYPE.ToUpper() == "EHS"
                                 orderby i.INCIDENT_ID descending
                                 select i).ToList();
                }
                else
                {
                    incidents = (from i in entities.INCIDENT
                                 where i.INCIDENT_TYPE.ToUpper() == "EHS"
                                     && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
                                 orderby i.INCIDENT_ID descending
                                 select i).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return incidents;
        }


        public static List<INCIDENT> SelectInjuryIllnessIncidents(decimal plantId, DateTime date)
        {
            var incidents = new List<INCIDENT>();

            try
            {
                var entities = new PSsqmEntities();
                incidents = (from i in entities.INCIDENT
                             where
                                 i.INCIDENT_TYPE.ToUpper() == "EHS" &&
                                 i.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness &&
                                 i.DETECT_PLANT_ID == plantId &&
                                 i.INCIDENT_DT.Year == date.Year && i.INCIDENT_DT.Month == date.Month
                             select i).ToList();

            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return incidents;
        }

        /// <summary>
        /// Select a list of open EHS incidents by company
        /// </summary>
        public static List<INCIDENT> SelectOpenIncidents(decimal companyId, List<decimal> plantIdList)
        {
            var incidents = new List<INCIDENT>();

            try
            {
                var date1900 = DateTime.Parse("01/01/1900 00:00:00");
                var entities = new PSsqmEntities();
                if (plantIdList == null)
                {
                    incidents = (from i in entities.INCIDENT
                                 where i.INCIDENT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
                                 orderby i.INCIDENT_ID descending
                                 select i).ToList();
                }
                else
                {
                    incidents = (from i in entities.INCIDENT
                                 where i.INCIDENT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
                                 && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
                                 orderby i.INCIDENT_ID descending
                                 select i).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return incidents;
        }

        public static void CloseIncident(decimal incidentId, DateTime? defaultDate)
        {
            var entities = new PSsqmEntities();

            var incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
            if (incident != null)
            {
                incident.CLOSE_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
                entities.SaveChanges();
            }
        }

        /// <summary>
        /// Returns boolean indicating whether 8D should be selected for incident type by default
        /// </summary>
        public static bool IsTypeDefault8D(decimal selectedTypeId)
        {
            var entities = new PSsqmEntities();
            return (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == selectedTypeId select it.DEFAULT_8D).FirstOrDefault();
        }

        /// <summary>
        /// Returns boolean indicating whether a custom form should be used for the incident type
        /// </summary>
        public static bool IsUseCustomForm(decimal selectedTypeId)
        {
            var entities = new PSsqmEntities();
            return (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == selectedTypeId select it.USE_CUSTOM_FORM).FirstOrDefault();
        }

        public static bool EnableNativeLangQuestion(string nlsLanguage)
        {
            return string.IsNullOrEmpty(nlsLanguage) || nlsLanguage.Contains("en") ? false : true;
        }

        public static string IncidentQuestionText(INCIDENT_QUESTION question, string nlsLanguage)
        {
            string text;

            if (question.INCIDENT_QUESTION_LANG != null && question.INCIDENT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).FirstOrDefault() != null)
            {
                text = question.INCIDENT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).First().LANG_TEXT;
            }
            else
            {
                text = question.QUESTION_TEXT;
            }

            return text;
        }

        /// <summary>
        /// Select a list of all incident questions by company and incident type
        /// </summary>
        public static List<EHSIncidentQuestion> SelectIncidentQuestionList(decimal incidentTypeId, decimal companyId, int step)
        {
            var questionList = new List<EHSIncidentQuestion>();

            try
            {
                var entities = new PSsqmEntities();
                var activeQuestionList = (from q in entities.INCIDENT_TYPE_COMPANY_QUESTION
                                          where q.INCIDENT_TYPE_ID == incidentTypeId && q.COMPANY_ID == companyId && q.STEP == step
                                          orderby q.SORT_ORDER
                                          select q
                                ).ToList();

                foreach (var aq in activeQuestionList)
                {
                    var questionInfo = (from qi in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG")
                                        where qi.INCIDENT_QUESTION_ID == aq.INCIDENT_QUESTION_ID
                                        select qi).FirstOrDefault();

                    var typeInfo = (from ti in entities.INCIDENT_QUESTION_TYPE
                                    where questionInfo.INCIDENT_QUESTION_TYPE_ID == ti.INCIDENT_QUESTION_TYPE_ID
                                    select ti).FirstOrDefault();

                    var newQuestion = new EHSIncidentQuestion()
                    {
                        QuestionId = questionInfo.INCIDENT_QUESTION_ID,
                        //QuestionText = questionInfo.QUESTION_TEXT,
                        QuestionText = IncidentQuestionText(questionInfo, SessionManager.UserContext.Language.NLS_LANGUAGE),
                        QuestionType = (EHSIncidentQuestionType)questionInfo.INCIDENT_QUESTION_TYPE_ID,
                        HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
                        IsRequired = questionInfo.IS_REQUIRED,
                        IsRequiredClose = questionInfo.IS_REQUIRED_CLOSE,
                        HelpText = questionInfo.HELP_TEXT,
                        StandardType = questionInfo.STANDARD_TYPE
                    };

                    if (newQuestion.HasMultipleChoices)
                    {
                        List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + questionInfo.INCIDENT_QUESTION_ID.ToString()).OrderBy(l => l.SortOrder).ToList();
                        if (choices.Count > 0)
                            newQuestion.AnswerChoices = choices;
                    }

                    // Question control logic
                    newQuestion.QuestionControls = (from qc in entities.INCIDENT_QUESTION_CONTROL
                                                    where qc.INCIDENT_TYPE_ID == incidentTypeId &&
                                                    qc.COMPANY_ID == companyId &&
                                                    qc.INCIDENT_QUESTION_ID == newQuestion.QuestionId
                                                    orderby qc.PROCESS_ORDER
                                                    select qc).ToList();

                    questionList.Add(newQuestion);
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return questionList;
        }

        /// <summary>
        /// Select a list of all possible incident questions
        /// </summary>
        public static List<EHSIncidentQuestion> SelectIncidentQuestionList()
        {
            var questionList = new List<EHSIncidentQuestion>();

            try
            {
                var entities = new PSsqmEntities();
                var allQuestions = (from q in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG") select q).ToList();

                foreach (var q in allQuestions)
                {

                    var typeInfo = (from ti in entities.INCIDENT_QUESTION_TYPE
                                    where q.INCIDENT_QUESTION_TYPE_ID == ti.INCIDENT_QUESTION_TYPE_ID
                                    select ti).FirstOrDefault();

                    var newQuestion = new EHSIncidentQuestion()
                    {
                        QuestionId = q.INCIDENT_QUESTION_ID,
                        QuestionText = q.QUESTION_TEXT,
                        QuestionType = (EHSIncidentQuestionType)q.INCIDENT_QUESTION_TYPE_ID,
                        HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
                        HelpText = q.HELP_TEXT
                    };

                    if (newQuestion.HasMultipleChoices)
                    {
                        List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + q.INCIDENT_QUESTION_ID.ToString()).OrderBy(l => l.SortOrder).ToList();
                        if (choices.Count > 0)
                            newQuestion.AnswerChoices = choices;
                    }
                    questionList.Add(newQuestion);
                }

                questionList.OrderBy(field => field.QuestionText);
                questionList.OrderBy(field => field.QuestionType);

            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return questionList;
        }

        public static List<INCIDENT_QUESTION> SelectIncidentQuestionList(decimal[] questionIds)
        {
            List<INCIDENT_QUESTION> topicList = new List<INCIDENT_QUESTION>();
            try
            {
                var entities = new PSsqmEntities();
                topicList = (from q in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG") where questionIds.Contains(q.INCIDENT_QUESTION_ID) select q).ToList();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return topicList;
        }

        public static List<EHSMetaData> SelectIncidentQuestionChoices(decimal questionID)
        {
            List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + questionID.ToString()).OrderBy(l => l.SortOrder).ToList();
            return choices;
        }

        public static decimal SelectIncidentLocationIdByIncidentId(decimal incidentId)
        {
            var entities = new PSsqmEntities();

            decimal? locationId = (from i in entities.INCIDENT
                                   where i.INCIDENT_ID == incidentId
                                   select i.DETECT_PLANT_ID).FirstOrDefault();

            locationId = locationId ?? 0;

            return (decimal)locationId;
        }

        public static string SelectIncidentLocationNameByIncidentId(decimal incidentId)
        {
            string locationName = "";

            decimal locationId = SelectIncidentLocationIdByIncidentId(incidentId);
            if (locationId > 0)
                locationName = SelectPlantNameById(locationId);

            return locationName;
        }

        public static string SelectPlantNameById(decimal plantId)
        {
            var entities = new PSsqmEntities();
            return (from p in entities.PLANT where p.PLANT_ID == plantId select p.PLANT_NAME).FirstOrDefault();
        }

        public static List<decimal> SelectPlantIdsByCompanyId(decimal companyId)
        {
            var entities = new PSsqmEntities();
            return (from p in entities.PLANT where p.COMPANY_ID == companyId orderby p.PLANT_NAME select p.PLANT_ID).ToList();
        }

        public static List<PERSON> SelectPrevActionPersonList(decimal plantID, SysPriv[] privList, bool emailOnly)
        {
            List<PERSON> personList = SQMModelMgr.SelectPrivGroupPersonList(privList, SysScope.prevent, plantID, true);
            if (emailOnly)
            {
                personList = personList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList();
            }

            return personList;
        }

        public static List<PERSON> SelectIncidentPersonList(INCIDENT incident, bool emailOnly)
        {
            List<PERSON> personList = SQMModelMgr.SelectPlantPersonList(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, (decimal)incident.DETECT_PLANT_ID);
            if (emailOnly)
            {
                personList = personList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList();
            }

            return personList;
        }

        public static List<PERSON> SelectEhsPeopleAtPlant(decimal plantId)
        {
            List<PERSON> people = new List<PERSON>();

            people = SQMModelMgr.SelectPrivGroupPersonList(new SysPriv[3] { SysPriv.originate, SysPriv.update, SysPriv.action }, SysScope.incident, plantId, false);

            return people;
        }

        public static List<EHSIncidentComment> SelectIncidentComments(decimal incidentId, decimal plantId)
        {
            var comments = new List<EHSIncidentComment>();
            PSsqmEntities entities = new PSsqmEntities();

            comments = (from c in entities.INCIDENT_VERIFICATION_COMMENT
                        join p in entities.PERSON on c.PERSON_ID equals p.PERSON_ID
                        where c.INCIDENT_ID == incidentId && c.PLANT_ID == plantId
                        select new EHSIncidentComment()
                        {
                            PersonName = p.FIRST_NAME + " " + p.LAST_NAME,
                            CommentText = c.COMMENT,
                            CommentDate = (DateTime)c.COMMENT_DATE
                        }).ToList();

            return comments;
        }

        /// <summary>
        /// Function for getting the data for attachment for FinalCorrectiveAction & Preventative Measures.
        /// </summary>
        /// <param name="incidentId"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static int AttachmentCounts(decimal incidentId, int record)
        {
            int count = 0;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                count = (from a in entities.ATTACHMENT where (a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId && a.INCIDENT_SECTION == record) select a).Count();
            }

            return count;
        }

        public static int AttachmentCount(decimal incidentId)
        {
            int count = 0;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                count = (from a in entities.ATTACHMENT where (a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId) select a).Count();
            }

            return count;
        }


        public static List<INCIDENT_ANSWER> GetIncidentAnswerList(decimal incidentId)
        {
            PSsqmEntities entities = new PSsqmEntities();
            List<INCIDENT_ANSWER> answerList = new List<INCIDENT_ANSWER>();

            answerList = (from c in entities.INCIDENT_ANSWER
                          where c.INCIDENT_ID == incidentId
                          select c).ToList();

            return answerList;
        }

        public static List<INCFORM_ROOT5Y> GetRootCauseList(decimal incidentId)
        {
            return GetRootCauseList(incidentId, false);
        }

        public static List<INCFORM_ROOT5Y> GetRootCauseList(decimal incidentId, bool getMinRows)
        {

            PSsqmEntities entities = new PSsqmEntities();
            var rootcauses = new List<INCFORM_ROOT5Y>();

            int minRowsThisForm = 1;

            rootcauses = (from c in entities.INCFORM_ROOT5Y
                          where c.INCIDENT_ID == incidentId
                          select c).ToList();

            if (getMinRows)
            {
                int itemsNeeded = 0;
                if (rootcauses.Count() < minRowsThisForm)
                    itemsNeeded = minRowsThisForm - rootcauses.Count();

                INCFORM_ROOT5Y rootcause = null;

                int seq = rootcauses.Count();

                for (int i = 1; i < itemsNeeded + 1; i++)
                {
                    rootcause = new INCFORM_ROOT5Y();
                    seq = seq + 1;
                    rootcause.ITEM_SEQ = seq;
                    rootcause.ITEM_DESCRIPTION = "";
                    rootcauses.Add(rootcause);
                }
            }

            return rootcauses;
        }

        public static List<INCFORM_ROOT5Y> FormatRootCauseList(INCIDENT incident, List<INCFORM_ROOT5Y> rootCauseList)
        {
            foreach (INCFORM_ROOT5Y rc in rootCauseList)
            {
                if (!rc.ITEM_TYPE.HasValue)
                    rc.ITEM_TYPE = 0;
                if (!rc.PROBLEM_SERIES.HasValue)
                    rc.PROBLEM_SERIES = 0;
            }

            if (rootCauseList.Where(l => l.ITEM_TYPE == 1).Count() == 0)
            {
                INCFORM_ROOT5Y cause = new INCFORM_ROOT5Y();
                cause.ITEM_TYPE = 1;
                cause.PROBLEM_SERIES = 0;
                cause.ITEM_SEQ = 0;
                //Get ITEM_DESCRIPTION if incident is not null
                if (incident != null)
                {
                    cause.ITEM_DESCRIPTION = incident.DESCRIPTION;
                    rootCauseList.Insert(0, cause);
                }
            }

            return rootCauseList;
        }

        public static List<INCFORM_CONTAIN> GetContainmentList(decimal incidentId, DateTime? defaultDate, bool createEmpty)
        {
            PSsqmEntities entities = new PSsqmEntities();
            var containments = new List<INCFORM_CONTAIN>();

            int minRowsThisForm = 1;

            containments = (from c in entities.INCFORM_CONTAIN
                            where c.INCIDENT_ID == incidentId
                            select c).ToList();

            int itemsNeeded = 0;
            if (containments.Count() < minRowsThisForm && createEmpty)
                itemsNeeded = minRowsThisForm - containments.Count();

            int seq = containments.Count(); ;
            INCFORM_CONTAIN contain = null;

            for (int i = 1; i < itemsNeeded + 1; i++)
            {
                contain = new INCFORM_CONTAIN();

                seq = seq + 1;
                contain.ITEM_SEQ = seq;
                contain.ITEM_DESCRIPTION = "";
                contain.ASSIGNED_PERSON = "";
                contain.START_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
                contain.COMPLETION_DATE = null;
                contain.IsCompleted = false;

                containments.Add(contain);
            }

            return containments;
        }

        public static List<INCFORM_WITNESS> GetWitnessList(decimal incidentId)
        {
            PSsqmEntities entities = new PSsqmEntities();
            var witnesses = new List<INCFORM_WITNESS>();

            int minRowsThisForm = 1;

            witnesses = (from w in entities.INCFORM_WITNESS
                         where w.INCIDENT_ID == incidentId
                         select w).ToList();

            int itemsNeeded = 0;
            if (witnesses.Count() < minRowsThisForm)
                itemsNeeded = minRowsThisForm - witnesses.Count();

            int seq = witnesses.Count(); ;
            INCFORM_WITNESS witness = null;

            for (int i = 1; i < itemsNeeded + 1; i++)
            {
                witness = new INCFORM_WITNESS();

                seq = seq + 1;
                witness.WITNESS_NO = seq;
                witness.WITNESS_NAME = "";
                witness.WITNESS_STATEMENT = "";

                witnesses.Add(witness);
            }

            return witnesses;
        }


        public static List<TASK_STATUS> GetCorrectiveActionList(decimal incidentId, DateTime? defaultDate, bool createEmpty)
        {
            PSsqmEntities entities = new PSsqmEntities();

            string taskStep = ((int)SysPriv.action).ToString();
            List<TASK_STATUS> actionList = (from t in entities.TASK_STATUS
                                            where t.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident && t.TASK_STEP == taskStep && t.RECORD_ID == incidentId
                                            select t).ToList();
            if (actionList.Count == 0 && createEmpty)
            {
                actionList.Add(CreateEmptyTask(incidentId, ((int)SysPriv.action).ToString(), 1, defaultDate));
            }

            return actionList;
        }

        public static INCFORM_ALERT LookupIncidentAlert(PSsqmEntities ctx, decimal incidentId)
        {
            INCFORM_ALERT incidentAlert = null;
            try
            {
                incidentAlert = (from a in ctx.INCFORM_ALERT where a.INCIDENT_ID == incidentId select a).SingleOrDefault();
            }
            catch
            {
            }

            return incidentAlert;
        }
        public static List<TASK_STATUS> GetAlertTaskList(PSsqmEntities ctx, decimal incidentId)
        {
            string taskStep = ((int)SysPriv.notify).ToString();
            List<TASK_STATUS> alertTaskList = (from t in ctx.TASK_STATUS
                                               where t.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident && t.TASK_STEP == taskStep && t.RECORD_ID == incidentId
                                               select t).ToList();
            return alertTaskList;
        }

        public static List<IncidentAlertItem> GetAlertItemList(PSsqmEntities ctx, decimal incidentId)
        {
            List<IncidentAlertItem> alertItemList = new List<IncidentAlertItem>();

            foreach (TASK_STATUS task in GetAlertTaskList(ctx, incidentId).ToList())
            {
                IncidentAlertItem item = new IncidentAlertItem();
                item.Task = task;
                if (task.RESPONSIBLE_ID.HasValue)
                {
                    item.Person = SQMModelMgr.LookupPerson(ctx, (decimal)task.RESPONSIBLE_ID, "", false);
                }
                if (task.RECORD_SUBID.HasValue)
                {
                    item.Location = SQMModelMgr.LookupPlant(ctx, (decimal)task.RECORD_SUBID, "");
                }
                alertItemList.Add(item);
            }

            return alertItemList;
        }

        public static TASK_STATUS CreateEmptyTask(decimal incidentId, string taskStep, int taskSeq, DateTime? defaultDate)
        {
            TASK_STATUS task = new TASK_STATUS();
            task.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
            task.RECORD_ID = incidentId;
            task.TASK_STEP = taskStep;
            task.TASK_TYPE = "T";
            task.CREATE_DT = defaultDate != null ? defaultDate : DateTime.UtcNow;
            task.STATUS = ((int)TaskStatus.New).ToString();
            task.TASK_SEQ = taskSeq;

            return task;
        }

        public static List<INCFORM_LOSTTIME_HIST> GetLostTimeList(decimal incidentId)
        {
            PSsqmEntities entities = new PSsqmEntities();
            var losttimelist = new List<INCFORM_LOSTTIME_HIST>();

            int minRowsThisForm = 1;

            losttimelist = (from c in entities.INCFORM_LOSTTIME_HIST
                            where c.INCIDENT_ID == incidentId
                            select c).OrderBy(l => l.BEGIN_DT).ToList();

            int itemsNeeded = 0;
            if (losttimelist.Count() < minRowsThisForm)
                itemsNeeded = minRowsThisForm - losttimelist.Count();

            int seq = losttimelist.Count(); ;
            INCFORM_LOSTTIME_HIST losttime = null;

            for (int i = 1; i < itemsNeeded + 1; i++)
            {
                losttime = new INCFORM_LOSTTIME_HIST();

                losttime.ITEM_DESCRIPTION = "";
                losttime.WORK_STATUS = "";
                losttime.BEGIN_DT = null;
                //losttime.BEGIN_DT = DateTime.UtcNow;
                losttime.NEXT_MEDAPPT_DT = null;
                losttime.RETURN_EXPECTED_DT = null;
                losttime.RETURN_TOWORK_DT = null;

                losttimelist.Add(losttime);
            }

            return losttimelist;
        }

        public static List<EHSIncidentTimeAccounting> CalculateIncidentAccounting(INCIDENT incident, string localeTimezone, int workdays)
        {
            List<EHSIncidentTimeAccounting> periodList = new List<EHSIncidentTimeAccounting>();

            // basic incident info gets accrued in the month the incident occurred
            EHSIncidentTimeAccounting period1 = new EHSIncidentTimeAccounting().CreateNew(Convert.ToDateTime(incident.INCIDENT_DT).Year, Convert.ToDateTime(incident.INCIDENT_DT).Month, (decimal)incident.ISSUE_TYPE_ID, (decimal)incident.DETECT_PLANT_ID);
            periodList.Add(period1);

            period1.NearMiss = incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.NearMiss ? 1 : 0;

            if (incident.INCFORM_INJURYILLNESS == null)
            {
                return periodList;
            }

            period1.RecordableCase = incident.INCFORM_INJURYILLNESS.RECORDABLE ? 1 : 0;
            period1.FirstAidCase = incident.INCFORM_INJURYILLNESS.FIRST_AID ? 1 : 0;
            period1.LostTimeCase = incident.INCFORM_INJURYILLNESS.LOST_TIME ? 1 : 0;
            period1.FatalityCase = incident.INCFORM_INJURYILLNESS.FATALITY.HasValue && (bool)incident.INCFORM_INJURYILLNESS.FATALITY ? 1 : 0;
            if (period1.RecordableCase + period1.FirstAidCase == 0)
                period1.OtherCase = 1;

            if (incident.INCFORM_INJURYILLNESS.LOST_TIME != true || incident.INCFORM_LOSTTIME_HIST == null || incident.INCFORM_LOSTTIME_HIST.Count == 0)
            {
                return periodList;
            }

            // determine incident time span
            // assume incident is still open and extend timespan to NOW if last status was not return to work
            List<INCFORM_LOSTTIME_HIST> histList = incident.INCFORM_LOSTTIME_HIST.OrderBy(h => h.BEGIN_DT).ToList();

            try
            {
                INCFORM_LOSTTIME_HIST hist = histList.First();
                INCFORM_LOSTTIME_HIST histLast = histList.Last();
                string workStatus = hist.WORK_STATUS;

                //DateTime startDate = hist.BEGIN_DT.HasValue ? WebSiteCommon.LocalTime((DateTime)hist.BEGIN_DT, localeTimezone).Date : WebSiteCommon.LocalTime((DateTime)incident.INCIDENT_DT, localeTimezone).Date;
                //DateTime endDate = histLast.BEGIN_DT.HasValue ? WebSiteCommon.LocalTime((DateTime)histLast.BEGIN_DT, localeTimezone).Date : startDate;

                DateTime startDate = ((DateTime)hist.BEGIN_DT).Date;
                DateTime endDate = histLast.INCIDENT_LOSTTIME_HIST_ID == hist.INCIDENT_LOSTTIME_HIST_ID ? DateTime.UtcNow : ((DateTime)histLast.BEGIN_DT).Date;

                /*
				if (histList.Last().WORK_STATUS != "02")  // if last record is not a return to work, assume last work status is still in effect
				{
					endDate = WebSiteCommon.LocalTime(DateTime.UtcNow.AddDays(-1), localeTimezone);
				}
				*/

                // truncate time accural to current day in case of erroneous lost/restricted time entry

                if (endDate > DateTime.UtcNow)
                {
                    endDate = WebSiteCommon.LocalTime(DateTime.UtcNow, localeTimezone).Date;
                }

                int numDays = Convert.ToInt32((endDate - startDate).TotalDays);     // get total # days of the incident timespan
                DateTime effDate;
                bool countDay = true;
                EHSIncidentTimeAccounting period;
                for (int n = 0; n <= numDays; n++)
                {
                    effDate = startDate.AddDays(n);

                    // accrue or add accounting periods as needed per the incident timespan
                    if ((period = periodList.Where(p => p.PeriodYear == effDate.Year && p.PeriodMonth == effDate.Month).FirstOrDefault()) == null)
                    {
                        periodList.Add((period = new EHSIncidentTimeAccounting().CreateNew(effDate.Year, effDate.Month, (decimal)incident.ISSUE_TYPE_ID, (decimal)incident.DETECT_PLANT_ID)));
                    }
                    // check if new work status occurred on this date
                    if ((hist = histList.Where(l => l.BEGIN_DT == effDate).FirstOrDefault()) != null)
                    {
                        workStatus = hist.WORK_STATUS;
                    }

                    countDay = true;
                    switch (effDate.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:  // count if 7 day work week
                            if (workdays < 7)
                                countDay = false;
                            break;
                        case DayOfWeek.Saturday:   // count if 6 day work week
                            if (workdays < 6)
                                countDay = false;
                            break;
                        default:
                            break;
                    }

                    if (countDay)
                    {
                        switch (workStatus)
                        {
                            case "01":
                                ++period.RestrictedTime;
                                break;
                            case "03":
                                ++period.LostTime;
                                break;
                            default:
                                ++period.WorkTime;
                                break;
                        }
                    }
                }
            }
            catch
            {
                // what to do with errors here ?
            }

            return periodList;
        }

        public static List<EHSIncidentTimeAccounting> SummarizeIncidentAccounting(List<EHSIncidentTimeAccounting> summaryList, List<EHSIncidentTimeAccounting> periodList)
        {
            EHSIncidentTimeAccounting period = null;

            foreach (EHSIncidentTimeAccounting pa in periodList)
            {
                if ((period = summaryList.Where(p => p.PeriodYear == pa.PeriodYear && p.PeriodMonth == pa.PeriodMonth && p.PlantID == pa.PlantID).FirstOrDefault()) == null)
                {
                    summaryList.Add((period = new EHSIncidentTimeAccounting().CreateNew(pa.PeriodYear, pa.PeriodMonth, 0, pa.PlantID)));
                }
                period.NearMiss += pa.NearMiss;
                period.FatalityCase += pa.FatalityCase;
                period.FirstAidCase += pa.FirstAidCase;
                period.LostTime += pa.LostTime;
                period.LostTimeCase += pa.LostTimeCase;
                period.RecordableCase += pa.RecordableCase;
                period.OtherCase += pa.OtherCase;
                period.RestrictedTime += pa.RestrictedTime;
                period.WorkTime += pa.WorkTime;
            }

            return summaryList;
        }

        public static List<EHSIncidentTimeAccounting> SelectTimePeriodSpan(List<EHSIncidentTimeAccounting> periodList, DateTime selectFromDate, DateTime selectToDate)
        {
            if (selectFromDate > DateTime.MinValue)  // return specific time slice, if specified
            {
                periodList = periodList.Where(l => new DateTime(l.PeriodYear, l.PeriodMonth, 28) >= new DateTime(selectFromDate.Year, selectFromDate.Month, 1) && new DateTime(l.PeriodYear, l.PeriodMonth, 1) <= new DateTime(selectToDate.Year, selectToDate.Month, 28)).ToList();
            }

            return periodList;
        }


        public static List<EHSIncidentApproval> GetApprovalList(PSsqmEntities ctx, decimal typeID, decimal step, decimal incidentID, DateTime? defaultDate, int approvalLevel)
        {
            // get required approvals for this step
            decimal incidentTypeID = typeID;

            // use default 0 incident type if specific type not defined in the priv table
            if ((from p in ctx.INCFORM_STEP_PRIV where p.INCIDENT_TYPE == typeID select p.PRIV).Count() == 0)
            {
                incidentTypeID = 0;
            }

            List<EHSIncidentApproval> approvalList = (from p in ctx.INCFORM_STEP_PRIV
                                                      join a in ctx.INCFORM_APPROVAL on p.STEP equals a.STEP into p_a
                                                      from a in p_a.Where(a => a.INCIDENT_ID == incidentID && a.ITEM_SEQ == p.PRIV).DefaultIfEmpty()
                                                      where p.INCIDENT_TYPE == incidentTypeID && p.STEP == step
                                                      select new EHSIncidentApproval
                                                      {
                                                          stepPriv = p,
                                                          approval = a
                                                      }).OrderBy(l => l.stepPriv.SIGN_ORDER).ToList();

            // seed the step approvals with existing and required placeholders
            foreach (EHSIncidentApproval rec in approvalList)
            {
                if (rec.approval == null)
                {
                    INCFORM_APPROVAL approval = new INCFORM_APPROVAL();
                    approval.INCIDENT_ID = incidentID;
                    approval.STEP = step;
                    approval.ITEM_SEQ = (int)rec.stepPriv.PRIV;
                    approval.APPROVAL_LEVEL = 0;
                    approval.APPROVAL_MESSAGE = "";
                    approval.APPROVER_TITLE = "";
                    approval.APPROVAL_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
                    approval.IsAccepted = false;
                    rec.approval = approval;
                }
            }

            return approvalList;
        }


        public static void CreateOrUpdateTask(INCIDENT incident, TASK_STATUS task, DateTime? defaultDate)
        {
            TaskStatusMgr taskMgr = new TaskStatusMgr();
            taskMgr.Initialize(task.RECORD_TYPE, task.RECORD_ID);
            TASK_STATUS theTask = taskMgr.SelectTask(task.TASK_ID);
            if (theTask == null)
            {
                task.CREATE_DT = defaultDate != null ? defaultDate : DateTime.UtcNow;
                task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;
                task.DETAIL = incident.DESCRIPTION;
                taskMgr.CreateTask(task);
                EHSNotificationMgr.NotifyIncidentTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
            }
            else
            {
                task.DETAIL = incident.DESCRIPTION;
                theTask = (TASK_STATUS)SQMModelMgr.CopyObjectValues(theTask, task, false);
                if (!task.CREATE_ID.HasValue)
                    task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;
                taskMgr.CreateTask(theTask);
            }

            taskMgr.UpdateTaskList(task.RECORD_ID);
        }

        public static void CreateOrUpdateTask(decimal incidentId, string taskStep, int taskSeq, decimal responsiblePersonId, int recordTypeId, DateTime dueDate, string taskDescription, string detail)
        {
            var entities = new PSsqmEntities();

            INCIDENT incident = SelectIncidentById(entities, incidentId);

            var taskMgr = new TaskStatusMgr();
            taskMgr.Initialize(recordTypeId, incidentId);
            taskMgr.LoadTaskList(recordTypeId, incidentId);
            TASK_STATUS task = taskMgr.FindTask(((int)SysPriv.action).ToString(), "T", taskSeq, responsiblePersonId);

            if (task == null)
            {
                task = taskMgr.CreateTask(((int)SysPriv.action).ToString(), "T", taskSeq, !string.IsNullOrEmpty(taskDescription) ? taskDescription : incident.ISSUE_TYPE, dueDate, responsiblePersonId);
                //task.DETAIL = detail;
                task.DETAIL = incident.ISSUE_TYPE;
                task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;
                if (recordTypeId == (int)TaskRecordType.PreventativeAction)
                {
                    EHSNotificationMgr.NotifyPrevActionTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
                }
                else
                {
                    EHSNotificationMgr.NotifyIncidentTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
                }
            }
            else
            {
                task = taskMgr.UpdateTask(task, dueDate, responsiblePersonId, taskDescription);
            }

            taskMgr.UpdateTaskList(incidentId);
        }

        public static void SetTaskComplete(decimal incidentId, int recordTypeId)
        {
            var taskMgr = new TaskStatusMgr();
            taskMgr.Initialize(recordTypeId, incidentId);
            taskMgr.LoadTaskList(recordTypeId, incidentId);
            TASK_STATUS task = taskMgr.FindTask(((int)SysPriv.action).ToString(), "T", 0);

            if (task != null)
            {
                taskMgr.UpdateTaskStatus(task, TaskMgr.CalculateTaskStatus(task));
                taskMgr.SetTaskComplete("0", "T", 0, true);
                taskMgr.UpdateTaskList(incidentId);
            }
        }


        public static int UpdateIncidentMediaAttachments(PSsqmEntities ctx, decimal tempID, decimal incidentID)
        {
            int status = 0;

            if (incidentID > 0)
                status = ctx.ExecuteStoreCommand("UPDATE VIDEO SET SOURCE_ID = " + incidentID + " WHERE SOURCE_TYPE = 40 AND SOURCE_ID = " + tempID.ToString());
            else
            {
                //status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO_FILE WHERE VIDEO_ID IN (SELECT VIDEO_ID FROM VIDEO WHERE SOURCE_TYPE = 40 AND SOURCE_ID = " + tempID.ToString() + ")");
                //status = ctx.ExecuteStoreCommand("DELETE FROM VIDEO WHERE SOURCE_TYPE = 40 AND SOURCE_ID = " + tempID.ToString());
                status = MediaVideoMgr.DeleteAllSourceVideos(tempID, 40, "");
            }

            return status;
        }

        public static int DeleteIncident(decimal incidentId)
        {
            int status = 0;
            string delCmd = " IN (" + incidentId + ") ";

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                try
                {
                    List<decimal> attachmentIds = (from a in ctx.ATTACHMENT
                                                   where a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId
                                                   select a.ATTACHMENT_ID).ToList();

                    if (attachmentIds != null && attachmentIds.Count > 0)
                    {
                        status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT_FILE WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
                        status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
                    }
                    status = ctx.ExecuteStoreCommand("DELETE FROM TASK_STATUS WHERE RECORD_TYPE = 40 AND RECORD_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_LOSTTIME_HIST WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CONTAIN WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ACTION WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CAUSATION WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_WITNESS WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_INJURYILLNESS WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ALERT WHERE INCIDENT_ID" + delCmd);
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCIDENT_ANSWER WHERE INCIDENT_ID" + delCmd);
                    status = MediaVideoMgr.DeleteAllSourceVideos(incidentId, 40, "");
                    status = ctx.ExecuteStoreCommand("DELETE FROM INCIDENT WHERE INCIDENT_ID" + delCmd);

                }
                catch (Exception ex)
                {
                    SQMLogger.LogException(ex);
                }
            }

            return status;
        }

        public static string SelectIncidentAnswer(INCIDENT incident, decimal questionId)
        {
            string answerText = null;
            var entities = new PSsqmEntities();
            answerText = (from a in entities.INCIDENT_ANSWER
                          where a.INCIDENT_ID == incident.INCIDENT_ID &&
                          a.INCIDENT_QUESTION_ID == questionId
                          select a.ANSWER_VALUE).FirstOrDefault();
            return answerText;
        }

        public static string SelectUserNameById(decimal personId)
        {
            var entities = new PSsqmEntities();
            var person = (from p in entities.PERSON
                          where p.PERSON_ID == personId
                          select p).FirstOrDefault();
            return person.LAST_NAME + ", " + person.FIRST_NAME;
        }

        public static INCFORM_POWEROUTAGE SelectPowerOutageDetailsById(PSsqmEntities entities, decimal incidentId)
        {
            return (from po in entities.INCFORM_POWEROUTAGE where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
        }

        public static INCFORM_INJURYILLNESS SelectInjuryIllnessDetailsById(PSsqmEntities entities, decimal incidentId)
        {
            return (from po in entities.INCFORM_INJURYILLNESS where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
        }

        public static INCFORM_LOSTTIME_HIST SelectLostTimeDetailsById(PSsqmEntities entities, decimal incidentId)
        {
            return (from po in entities.INCFORM_LOSTTIME_HIST where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
        }

        public static int GetNextContainSequence(decimal incidentId)
        {
            var entities = new PSsqmEntities();
            var lastcontain = new INCFORM_CONTAIN();
            lastcontain = (from c in entities.INCFORM_CONTAIN where c.INCIDENT_ID == incidentId select c).OrderByDescending(c => c.ITEM_SEQ).First();

            int nextSeq = 1;

            if (lastcontain != null)
                nextSeq = lastcontain.ITEM_SEQ + 1;

            return nextSeq;
        }

        public static int GetNextRoot5YSequence(decimal incidentId)
        {
            var entities = new PSsqmEntities();
            var lastroot5y = new INCFORM_ROOT5Y();
            lastroot5y = (from r in entities.INCFORM_ROOT5Y where r.INCIDENT_ID == incidentId select r).OrderByDescending(r => r.ITEM_SEQ).First();

            int nextSeq = 1;

            if (lastroot5y != null)
                nextSeq = lastroot5y.ITEM_SEQ + 1;

            return nextSeq;
        }

        public static int GetNextActionSequence(decimal incidentId)
        {
            var entities = new PSsqmEntities();
            var lastaction = new INCFORM_ACTION();
            lastaction = (from a in entities.INCFORM_ACTION where a.INCIDENT_ID == incidentId select a).OrderByDescending(a => a.ITEM_SEQ).First();

            int nextSeq = 1;

            if (lastaction != null)
                nextSeq = lastaction.ITEM_SEQ + 1;

            return nextSeq;
        }

        public static int GetNextApprovalSequence(decimal incidentId)
        {
            var entities = new PSsqmEntities();
            var lastapproval = new INCFORM_APPROVAL();
            lastapproval = (from ap in entities.INCFORM_APPROVAL where ap.INCIDENT_ID == incidentId select ap).OrderByDescending(ap => ap.ITEM_SEQ).First();

            int nextSeq = 1;

            if (lastapproval != null)
                nextSeq = lastapproval.ITEM_SEQ + 1;

            return nextSeq;
        }

        #region prevactions

        public static bool CanUpdatePrevAction(INCIDENT incident, bool IsEditContext, SysPriv[] privNeededList, int stepCompleted)
        {
            bool canUpdate = false;

            if (stepCompleted >= (int)IncidentStepStatus.signoff1)  // action is closed
            {
                canUpdate = false;
            }
            else if (incident != null && incident.CREATE_PERSON == SessionManager.UserContext.Person.PERSON_ID)
            {
                canUpdate = true;
            }
            else
            {
                foreach (SysPriv priv in privNeededList)
                {
                    if (SessionManager.CheckUserPrivilege(priv, SysScope.prevent))
                        canUpdate = true;
                }
            }

            return canUpdate;
        }


        public static bool CanDeletePrevAction(decimal createPersonID, int stepCompleted)
        {
            bool canDelete = false;

            if (UserContext.CheckUserPrivilege(SysPriv.approve1, SysScope.prevent) ||
                UserContext.CheckUserPrivilege(SysPriv.approve2, SysScope.prevent) ||
                UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.prevent) ||
                SessionManager.UserContext.Person.PERSON_ID == createPersonID)
            {
                canDelete = true;
            }

            return canDelete;
        }

        public static INCIDENT UpdatePrevActionStatus(INCIDENT theIncident, List<EHSIncidentQuestion> questions, int currentStep, DateTime? defaultDate)
        {
            int status = 0;
            EHSIncidentQuestion completionQuestion = null;

            // assume questions collection is for the current step 
            bool stepComplete = true;
            foreach (EHSIncidentQuestion q in questions)
            {
                if (q.IsRequired && string.IsNullOrEmpty(q.AnswerText))
                {
                    stepComplete = false;
                    break;
                }
            }

            if (stepComplete)
            {
                switch (currentStep)
                {
                    case 0:
                        if (theIncident.INCFORM_LAST_STEP_COMPLETED <= (int)IncidentStepStatus.defined)
                        {
                            theIncident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.defined;
                            theIncident.LAST_UPD_DT = defaultDate.HasValue ? defaultDate : DateTime.UtcNow;
                        }
                        break;
                    case 1:
                        completionQuestion = questions.Where(l => l.QuestionId == (decimal)EHSQuestionId.CorrectiveActionsStatus).FirstOrDefault();
                        if (completionQuestion.AnswerText.ToLower() == "in progress")
                        {
                            theIncident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.correctiveaction;
                        }
                        else if (completionQuestion.AnswerText.ToLower() == "closed")
                        {
                            if (theIncident.INCFORM_LAST_STEP_COMPLETED < (int)IncidentStepStatus.correctiveactionComplete)
                            {
                                theIncident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.correctiveactionComplete;
                                theIncident.CLOSE_DATE_DATA_COMPLETE = theIncident.LAST_UPD_DT = defaultDate.HasValue ? defaultDate : DateTime.UtcNow;
                                SetTaskComplete(theIncident.INCIDENT_ID, 45);
                            }
                        }
                        break;
                    case 2:
                        completionQuestion = questions.Where(l => l.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved).FirstOrDefault();
                        if (completionQuestion.AnswerText.ToLower() == "yes" || completionQuestion.AnswerText.ToLower().Contains("funding"))
                        {
                            if (theIncident.INCFORM_LAST_STEP_COMPLETED < (int)IncidentStepStatus.signoffComplete)
                            {
                                theIncident.INCFORM_LAST_STEP_COMPLETED = (int)IncidentStepStatus.signoffComplete;
                                theIncident.CLOSE_DATE = theIncident.LAST_UPD_DT = defaultDate.HasValue ? defaultDate : DateTime.UtcNow;
                                theIncident.AUDIT_PERSON = theIncident.CLOSE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return theIncident;
        }

        #endregion
    }


    #region metadata

    public class Timeline
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Text { get; set; }
    }


    public class EHSMetaData
    {
        public string Language { get; set; }
        public string MetaDataType { get; set; }
        public string Text { get; set; }
        public string TextLong { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public int? SortOrder { get; set; }
        public bool IsHeading { get; set; }

    }

    public static class EHSMetaDataMgr
    {
        public static List<EHSMetaData> SelectMetaDataList(string metaDataType)
        {
            var entities = new PSsqmEntities();
            var metaList = new List<EHSMetaData>();

            string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";

            if (language == "en")
            {
                metaList = (from x in entities.XLAT
                            where x.XLAT_LANGUAGE == language && x.XLAT_GROUP == metaDataType && x.STATUS == "A"
                            orderby x.XLAT_CODE
                            select new EHSMetaData()
                            {
                                Language = x.XLAT_LANGUAGE,
                                MetaDataType = x.XLAT_GROUP,
                                Text = !string.IsNullOrEmpty(x.DESCRIPTION_SHORT) ? x.DESCRIPTION_SHORT.Trim().Replace("\r\n", "") : "",
                                TextLong = !string.IsNullOrEmpty(x.DESCRIPTION) ? x.DESCRIPTION.Trim().Replace("\r\n", "") : "",
                                Value = !string.IsNullOrEmpty(x.XLAT_CODE) ? x.XLAT_CODE.Trim().Replace("\r\n", "") : "",
                                Status = x.STATUS,
                                SortOrder = x.SORT_ORDER,
                                IsHeading = (bool)x.IS_HEADING
                            }).ToList();
            }
            else
            {
                var tempList = (from x in entities.XLAT
                                where (x.XLAT_LANGUAGE == language || x.XLAT_LANGUAGE == "en") && x.XLAT_GROUP == metaDataType && x.STATUS == "A"
                                orderby x.XLAT_CODE
                                select new EHSMetaData()
                                {
                                    Language = x.XLAT_LANGUAGE,
                                    MetaDataType = x.XLAT_GROUP,
                                    Text = !string.IsNullOrEmpty(x.DESCRIPTION_SHORT) ? x.DESCRIPTION_SHORT.Trim().Replace("\r\n", "") : "",
                                    TextLong = !string.IsNullOrEmpty(x.DESCRIPTION) ? x.DESCRIPTION.Trim().Replace("\r\n", "") : "",
                                    Value = !string.IsNullOrEmpty(x.XLAT_CODE) ? x.XLAT_CODE.Trim().Replace("\r\n", "") : "",
                                    Status = x.STATUS,
                                    SortOrder = x.SORT_ORDER,
                                    IsHeading = (bool)x.IS_HEADING
                                }).ToList();

                EHSMetaData XLATlang = null;
                foreach (EHSMetaData xlat in tempList.Where(x => x.Language == "en").ToList())
                {
                    XLATlang = tempList.Where(l => l.MetaDataType == xlat.MetaDataType && l.Value == xlat.Value && l.Language == language).FirstOrDefault();
                    if (XLATlang != null)
                        metaList.Add(XLATlang);
                    else
                    {
                        XLATlang = new EHSMetaData();
                        XLATlang = (EHSMetaData)SQMModelMgr.CopyObjectValues(XLATlang, xlat, false);
                        XLATlang.Language = language;  // substitute english xlat if localized version does not exist
                        metaList.Add(xlat);
                    }
                }
            }

            return metaList;
        }
        /// <summary>
        /// To call value of nested dropdown list.
        /// </summary>
        /// <param name="GroupValue"></param>
        /// <param name="XLAT_CODE"></param>
        /// <returns></returns>
        public static List<EHSMetaData> SelectMetaDataList(string GroupValue, string XLAT_CODE)
        {
            var entities = new PSsqmEntities();
            var metaList = new List<EHSMetaData>();
            string XLAT_GROUP;


            string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";

            if (language == "en")
            {

                metaList = (from x in entities.XLAT
                            where x.XLAT_LANGUAGE == language && x.XLAT_GROUP == GroupValue && x.STATUS == "A" && x.XLAT_CODE.Contains(XLAT_CODE)
                            orderby x.XLAT_CODE
                            select new EHSMetaData()
                            {
                                Language = x.XLAT_LANGUAGE,
                                MetaDataType = x.XLAT_GROUP,
                                Text = !string.IsNullOrEmpty(x.DESCRIPTION_SHORT) ? x.DESCRIPTION_SHORT.Trim().Replace("\r\n", "") : "",
                                TextLong = !string.IsNullOrEmpty(x.DESCRIPTION) ? x.DESCRIPTION.Trim().Replace("\r\n", "") : "",
                                Value = !string.IsNullOrEmpty(x.XLAT_CODE) ? x.XLAT_CODE.Trim().Replace("\r\n", "") : "",
                                Status = x.STATUS,
                                SortOrder = x.SORT_ORDER,
                                IsHeading = (bool)x.IS_HEADING
                            }).ToList();
            }
            else
            {
                var tempList = (from x in entities.XLAT
                                where (x.XLAT_LANGUAGE == language || x.XLAT_LANGUAGE == "en") && x.XLAT_CODE == GroupValue && x.STATUS == "A" && x.XLAT_CODE.Contains(XLAT_CODE)
                                orderby x.XLAT_CODE
                                select new EHSMetaData()
                                {
                                    Language = x.XLAT_LANGUAGE,
                                    MetaDataType = x.XLAT_GROUP,
                                    Text = !string.IsNullOrEmpty(x.DESCRIPTION_SHORT) ? x.DESCRIPTION_SHORT.Trim().Replace("\r\n", "") : "",
                                    TextLong = !string.IsNullOrEmpty(x.DESCRIPTION) ? x.DESCRIPTION.Trim().Replace("\r\n", "") : "",
                                    Value = !string.IsNullOrEmpty(x.XLAT_CODE) ? x.XLAT_CODE.Trim().Replace("\r\n", "") : "",
                                    Status = x.STATUS,
                                    SortOrder = x.SORT_ORDER,
                                    IsHeading = (bool)x.IS_HEADING
                                }).ToList();

                EHSMetaData XLATlang = null;
                foreach (EHSMetaData xlat in tempList.Where(x => x.Language == "en").ToList())
                {
                    XLATlang = tempList.Where(l => l.MetaDataType == xlat.MetaDataType && l.Value == xlat.Value && l.Language == language).FirstOrDefault();
                    if (XLATlang != null)
                        metaList.Add(XLATlang);
                    else
                    {
                        XLATlang = new EHSMetaData();
                        XLATlang = (EHSMetaData)SQMModelMgr.CopyObjectValues(XLATlang, xlat, false);
                        XLATlang.Language = language;  // substitute english xlat if localized version does not exist
                        metaList.Add(xlat);
                    }
                }
            }

            return metaList;
        }
        #endregion
    }
}