using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.Objects;
using System.Configuration;
using System.Web.Configuration;


namespace SQM.Website
{
    public enum TaskStatus { Pending, Complete, Due, Overdue, EscalationLevel1, EscalationLevel2, unused1, unused2, AwaitingClosure, New, Delete };
    public enum TaskNotification { Owner, Delegate, Escalation };
    public enum TaskRecordType { InternalQualityIncident = 10, CustomerQualityIncident = 11, SupplierQualityIncident = 12,
                                    QualityIssue = 20, ProblemCase = 21,
                                    ProfileInput = 30, ProfileInputApproval = 31, ProfileInputFinalize = 33, CurrencyInput = 36, 
                                    HealthSafetyIncident = 40, PreventativeAction = 45, 
									Audit = 50 }


    public class ResponseItem
    {
        public RESPONSE Response
        {
            get;
            set;
        }
        public List<ATTACHMENT> AttachmentList
        {
            get;
            set;
        }

        public ResponseItem Initialize()
        {
            this.AttachmentList = new List<ATTACHMENT>();
            return this;
        }

        public ResponseItem CreateNew()
        {
            this.Initialize();
            this.Response = new RESPONSE();
            return this;
        }
    }

    public class ResponseMgr
    {
        public List<ResponseItem> ResponseList
        {
            get;
            set;
        }

        public PSsqmEntities Entities
        {
            get;
            set;
        }
 
        public ResponseMgr Initialize(PSsqmEntities entities)
        {
            if (entities != null)
                this.Entities = entities;
            else
                this.Entities = new PSsqmEntities();

            this.ResponseList = new List<ResponseItem>();
           
            return this;
        }

        public List<ResponseItem> Load(int recordType, decimal recordID, string recordStep)
        {
            this.ResponseList = new List<ResponseItem>();
            try
            {
               this.ResponseList = (from r in this.Entities.RESPONSE 
                                 where (r.RECORD_TYPE == recordType && r.RECORD_ID == recordID)
                                 select new ResponseItem 
                                 {
                                    Response = r
                                 }
                                 ).OrderBy(i => i.Response.RECORD_STEP).ToList();


               decimal[] ids = this.ResponseList.Select(i => i.Response.RESPONSE_ID).Distinct().ToArray();
               List<ATTACHMENT> attachList = (from a in this.Entities.ATTACHMENT
                                              where
                                               a.RECORD_TYPE == 111 && ids.Contains(a.RECORD_ID)
                                              select a).OrderBy(l => l.ATTACHMENT_ID).ToList();
               foreach (ResponseItem ri in this.ResponseList)
               {
                   ri.AttachmentList = new List<ATTACHMENT>();
                   ri.AttachmentList.AddRange(attachList.Where(a => a.RECORD_ID == ri.Response.RESPONSE_ID).ToList());
               }
            }
            catch (Exception ex)
            {
                ;
            }

            return this.ResponseList;
        }

        public ResponseItem CreateResponse(int recordType, decimal recordID, string recordStep, decimal personID, string responseText, string referenceData)
        {
            ResponseItem response = new ResponseItem().CreateNew();

            response.Response.RECORD_TYPE = recordType;
            response.Response.RECORD_ID = recordID;
            response.Response.RECORD_STEP = recordStep;
            response.Response.RESPONSE_DT = DateTime.UtcNow;
            response.Response.PERSON_ID = personID;
            response.Response.REFERENCE_DATA = referenceData;
            response.Response.RESPONSE_TEXT = responseText.Trim();
            response.Response.STATUS = "";

            this.ResponseList.Add(response);
            this.Entities.AddToRESPONSE(response.Response);
          
            return response;
        }

        public int UpdateResponses(decimal parentRecordID)
        {
            int status = 0;

            try
            {
                foreach (ResponseItem ri in this.ResponseList)
                {
                    if (ri.Response.RECORD_ID <= 0)
                        ri.Response.RECORD_ID = parentRecordID;
                }
                status = this.Entities.SaveChanges();
            }
            catch (Exception ex)
            {
                ;
            }

            return status;
        }
    }
 

    public class TaskItem
    {
        public TASK_STATUS Task
        {
            get;
            set;
        }
        public string TaskSeq
        {
            get;
            set;
        }
        public int RecordType
        {
            get;
            set;
        }
        public decimal RecordID
        {
            get;
            set;
        }
        public string RecordKey
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string LongTitle
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
 
        public TaskStatus Taskstatus
        {
            get;
            set;
        }
        public TaskNotification NotifyType
        {
            get;
            set;
        }
        public object Detail
        {
            get;
            set;
        }
        public object Reference
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public PLANT PlantResponsible
        {
            get;
            set;
        }
        public PART Part
        {
            get;
            set;
        }
        public PERSON Person
        {
            get;
            set;
        }
        public DateTime TaskDate
        {
            get;
            set;
        }
        // added to enable rad scheduler control
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }
    }

    #region taskstatus
    public class TaskStatusMgr
    {
        public List<TASK_STATUS> TaskList
        {
            get;
            set;
        }
        public int RecordType
        {
            get;
            set;
        }
        public decimal RecordID
        {
            get;
            set;
        }
        public bool isNew
        {
            get;
            set;
        }
        public int UpdateStatus
        {
            get;
            set;
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }

        public TaskStatusMgr CreateNew(int recordType, decimal recordID)
        {
            this.Initialize(recordType, recordID);
            this.isNew = true;
            return this;
        }

        public TaskStatusMgr Initialize(int recordType, decimal recordID)
        {
            this.TaskList = new List<TASK_STATUS>();
            this.RecordType = recordType;
            this.RecordID = recordID;
            this.isNew = false;
            this.Entities = new PSsqmEntities();
            return this;
        }

        public TASK_STATUS CreateTask(string taskStep, string taskType, int taskSeq, string description, DateTime dueDate, decimal responsibleID)
        {
            TASK_STATUS task = new TASK_STATUS();
            task.RECORD_TYPE = this.RecordType;
            task.RECORD_ID = this.RecordID;
            task.TASK_STEP = taskStep;
            task.TASK_TYPE = taskType;
            task.TASK_SEQ = taskSeq;
            task.DESCRIPTION = description;
            if (dueDate != DateTime.MinValue)
                task.DUE_DT = dueDate;
            if (responsibleID > 0)
                task.RESPONSIBLE_ID = responsibleID;
            task.STATUS = ((int)TaskStatus.New).ToString();
            this.TaskList.Add(task);
            return task;
        }

        public TASK_STATUS FindTask(string taskStep, string taskType, int taskSeq)
        {
            TASK_STATUS task = null;
            task = this.TaskList.Where(l => l.TASK_STEP == taskStep && l.TASK_TYPE == taskType && l.TASK_SEQ == taskSeq).FirstOrDefault();
            return task;
        }

        public TASK_STATUS FindTask(string taskStep, string taskType, decimal responsibleID)
        {
            TASK_STATUS task = null;
            task = this.TaskList.Where(l => l.TASK_STEP == taskStep && l.TASK_TYPE == taskType && l.RESPONSIBLE_ID == responsibleID).FirstOrDefault();
            return task;
        }

        public TASK_STATUS UpdateTask(TASK_STATUS task, DateTime dueDate, decimal responsibleID, string description)
        {
            task.DUE_DT = dueDate;
            task.RESPONSIBLE_ID = responsibleID;
            task.DESCRIPTION = description;
            return task;
        }

        public TASK_STATUS UpdateTaskStatus(TASK_STATUS task, TaskStatus status)
        {
            task.STATUS = ((int)status).ToString();
            return task;
        }

        public TASK_STATUS SetTaskOpen(TASK_STATUS task, DateTime? newDueDate)
        {
            // re-open closed task
            if (task.COMPLETE_DT != null)
            {
                task.STATUS = ((int)TaskStatus.Pending).ToString();
                task.COMPLETE_DT = null;
                task.COMPLETE_ID = null;
                if (newDueDate != null)
                    task.DUE_DT = newDueDate;
            }
            return task;
        }

        public TASK_STATUS SetTaskComplete(TASK_STATUS task, decimal personID)
        {
            task.STATUS = Convert.ToInt32(TaskStatus.Complete).ToString();
            task.COMPLETE_DT = DateTime.UtcNow;
            task.COMPLETE_ID = personID;
            return task;
        }

        public TASK_STATUS SetTaskComplete(string taskStep, string taskType, int taskSeq, bool isComplete)
        {
            TASK_STATUS task = FindTask(taskStep, taskType, taskSeq);
            if (task != null)
            {
                task.STATUS = Convert.ToInt32(TaskStatus.Complete).ToString();
                task.COMPLETE_DT = DateTime.UtcNow;
                task.COMPLETE_ID = SessionManager.UserContext.Person.PERSON_ID;
            }
            return task;
        }

        public int UpdateTaskList(decimal recordID)
        {
            int status = 0;

            try
            {
                for (int n = 0; n < this.TaskList.Count; n++)
                {
                    TASK_STATUS task = this.TaskList[n];
                    if (task.RECORD_ID <= 0)
                        task.RECORD_ID = recordID;
                    if (task.EntityState == EntityState.Detached)
                        this.Entities.AddToTASK_STATUS(task);
                    if ((TaskStatus)Enum.Parse(typeof(TaskStatus), task.STATUS) >= TaskStatus.New)
                        this.Entities.DeleteObject(task);
                }

                status = this.UpdateStatus = this.Entities.SaveChanges();
            }

            catch (Exception ex)
            {
               // SQMLogger.LogException(ex);
            }

            return status;
        }

        public TaskStatusMgr LoadTaskList(int recordType, decimal recordID)
        {
            this.RecordType = RecordType;
            this.RecordID = recordID;
            this.TaskList = new List<TASK_STATUS>();
            try
            {
                this.TaskList = (from t in this.Entities.TASK_STATUS
                                 where (t.RECORD_TYPE == recordType && t.RECORD_ID == recordID)
                                 select t).OrderBy(l => l.TASK_STEP).ThenBy(l => l.TASK_SEQ).ToList();
            }
            catch (Exception ex)
            {
                ;
            }
            return this;
        }

    }
    #endregion

    #region taskmgr
    public class TaskMgr
    {
		private static List<SETTINGS> notifySettings; 

        public static TaskStatus CalculateTaskStatus(TASK_STATUS task)
        {
            TaskStatus status = (TaskStatus)Convert.ToInt32(task.STATUS);

            if (status == TaskStatus.AwaitingClosure)
            {
                return status;
            }
            else if (task.DUE_DT != null  &&  status != TaskStatus.Complete)
            {
                DateTime duedate = (DateTime)task.DUE_DT;
                TimeSpan delta = duedate.Subtract(DateTime.UtcNow);
                status = TaskStatus.Pending;
                if (delta.Days < 2)
                    status = TaskStatus.Due;
                if (delta.Days < 0)
                    status = TaskStatus.Overdue;
            }

            return status;
        }

        public static string TaskStatusImage(TaskStatus status)
        {
            string imageURL = "~/images/status/time1.png";
            switch (status)
            {
                case TaskStatus.Complete:
                    imageURL = "~/images/status/checked.png";
                    break;
                case TaskStatus.Due:
                    imageURL = "~/images/status/taskdue.png";
                    break;
                case TaskStatus.Overdue:
                    imageURL = "~/images/status/warning.png";
                    break;
                case TaskStatus.EscalationLevel1:
                    imageURL = "~/images/status/flag1.png";
                    break;
                case TaskStatus.EscalationLevel2:
                    imageURL = "~/images/status/flag2.png";
                    break;
                case TaskStatus.AwaitingClosure:
                    imageURL = "~/images/status/taskdue-bak.png";
                    break;
                default:
                    imageURL = "~/images/status/time1.png";
                    break;
            }

            return imageURL;
        }
        #endregion

        #region taskqueries

        public static List<TaskItem> IncidentTaskSchedule(decimal companyID, DateTime fromDate, DateTime toDate, List<decimal> responsibleIDS, decimal[] plantIDS, bool addProblemCases)
        {
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS" });
            List<TaskItem> taskList = new List<TaskItem>();
			INCIDENT incident;

            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    taskList = (from t in entities.TASK_STATUS
                                join i in entities.INCIDENT on t.RECORD_ID equals i.INCIDENT_ID
                                join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
                                join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID into l_i
                                join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID into r_i
                                join q in entities.QI_OCCUR on i.INCIDENT_ID equals q.INCIDENT_ID into q_i
                                where ((t.RECORD_TYPE == (int)TaskRecordType.QualityIssue || t.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident || t.RECORD_TYPE == (int)TaskRecordType.PreventativeAction) && (t.DUE_DT > fromDate  &&  t.DUE_DT <= toDate) && (responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) || plantIDS.Contains((decimal)i.DETECT_PLANT_ID) || plantIDS.Contains((decimal)i.RESP_PLANT_ID)))
                                from p in p_t.DefaultIfEmpty()
                                from q in q_i.DefaultIfEmpty()
                                from l in l_i.DefaultIfEmpty()
                                from r in r_i.DefaultIfEmpty()
                                select new TaskItem
                                {
                                    Task = t,
                                    RecordType = t.RECORD_TYPE,
                                    RecordID = t.RECORD_ID,
                                    Person = p,
                                    Detail = i,
                                    Plant = l,
                                    PlantResponsible = r,
                                    Reference = q
                                }).OrderBy(l => l.RecordID).ToList();

					taskList.AddRange((from t in entities.TASK_STATUS
									   join i in entities.AUDIT on t.RECORD_ID equals i.AUDIT_ID
									   join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
									   join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID into l_i
									   where (t.RECORD_TYPE == (int)TaskRecordType.Audit && (t.DUE_DT > fromDate && t.DUE_DT <= toDate) && (responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) || plantIDS.Contains((decimal)i.DETECT_PLANT_ID)))
									   from p in p_t.DefaultIfEmpty()
									   from l in l_i.DefaultIfEmpty()
									   select new TaskItem
									   {
										   Task = t,
										   RecordType = t.RECORD_TYPE,
										   RecordID = t.RECORD_ID,
										   Person = p,
										   Detail = i,
										   Plant = l,
										   PlantResponsible = l
									   }).OrderBy(l => l.RecordID).ToList());

                    decimal recordID = 0;
                    List<PLANT> plantList = new List<PLANT>();
                    foreach (TaskItem taskItem in taskList)
                    {
                        taskItem.Taskstatus = CalculateTaskStatus(taskItem.Task);
                        TaskRecordType recordType = (TaskRecordType)taskItem.RecordType;
						string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == taskItem.Task.TASK_STEP).FirstOrDefault() != null ? XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == taskItem.Task.TASK_STEP).FirstOrDefault().DESCRIPTION : WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());

                        switch (recordType)
                        {
                            case TaskRecordType.QualityIssue:
                                if (taskItem.Reference != null)
                                {
									incident = (INCIDENT)taskItem.Detail;
                                    try
                                    {
                                        if (((QI_OCCUR)taskItem.Reference).QS_ACTIVITY == "CST")
                                        {
                                            taskItem.Title = WebSiteCommon.GetXlatValueLong("qualityActivity", ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY);
                                            if (!string.IsNullOrEmpty(taskItem.Task.DESCRIPTION))
                                                taskItem.Title += (" (" + taskItem.Task.DESCRIPTION + ")");
                                            taskItem.LongTitle = taskItem.PlantResponsible.PLANT_NAME + " - " + taskItem.Title;
                                        }
                                        else
                                        {
                                            taskItem.Title = WebSiteCommon.GetXlatValueLong("qualityActivity", ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY);
                                            if (!string.IsNullOrEmpty(taskItem.Task.DESCRIPTION))
                                                taskItem.Title += (" (" + taskItem.Task.DESCRIPTION + ")");
                                            taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + taskItem.Title;
                                        }
                                        taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString() + "|" + ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY;
                                        taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Issue ") + ": " + incident.DESCRIPTION;
                                        taskItem.Part = SQMModelMgr.LookupPart(entities, ((QI_OCCUR)taskItem.Reference).PART_ID, "", 1, false);
                                    }
                                    catch { }
                                }
                                break;
                            case TaskRecordType.HealthSafetyIncident:
                            case TaskRecordType.PreventativeAction:
								incident = (INCIDENT)taskItem.Detail;
                                taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString();
								taskItem.Title = taskItem.Task.DESCRIPTION;
                                taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + actionText + ": " + taskItem.Task.DESCRIPTION;
                                taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Incident ") + ": " + incident.DESCRIPTION;
                                break;
							case TaskRecordType.Audit:
								AUDIT audit = (AUDIT)taskItem.Detail;
								taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString();
								taskItem.Title = WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());
								taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + actionText + ": " + taskItem.Task.DESCRIPTION;
								taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Audit ") + ": " + audit.DESCRIPTION;
								break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
            }

            if (addProblemCases)
            {
                List<TaskItem> probTaskList = new List<TaskItem>();
                try
                {
                    using (PSsqmEntities entities = new PSsqmEntities())
                    {
                        if (plantIDS.Length > 0)
                        {
                            // get cases for specific plants
                            probTaskList = (from t in entities.TASK_STATUS
                                            join c in entities.PROB_CASE on t.RECORD_ID equals c.PROBCASE_ID
                                            join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
                                            where (t.RECORD_TYPE == (int)TaskRecordType.ProblemCase && (t.DUE_DT > fromDate &&  t.DUE_DT <= toDate))
                                            from p in p_t.DefaultIfEmpty()
                                            select new TaskItem
                                            {
                                                Task = t,
                                                RecordType = t.RECORD_TYPE,
                                                RecordID = t.RECORD_ID,
                                                Person = p,
                                                Detail = c
                                            }).OrderBy(l => l.RecordID).ToList();

                            List<decimal> caseList = probTaskList.Select(l=> l.RecordID).Distinct().ToList();
                            List<PROB_OCCUR> occurList = (from o in entities.PROB_OCCUR.Include("INCIDENT")
                                                          where (caseList.Contains(o.PROBCASE_ID))
                                                          select o).ToList();
                            probTaskList = probTaskList.Where(l => occurList.Where(o => plantIDS.Contains((decimal)o.INCIDENT.DETECT_PLANT_ID) || plantIDS.Contains((decimal)o.INCIDENT.RESP_PLANT_ID)).Select(o => o.PROBCASE_ID).ToList().Contains(l.RecordID)).ToList();
                        }
                        else
                        {
                            probTaskList = (from t in entities.TASK_STATUS
                                            join c in entities.PROB_CASE on t.RECORD_ID equals c.PROBCASE_ID
                                            join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
                                            where (t.RECORD_TYPE == (int)TaskRecordType.ProblemCase && responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) && (t.DUE_DT > fromDate &&  t.DUE_DT <= toDate))
                                            from p in p_t.DefaultIfEmpty()
                                            select new TaskItem
                                            {
                                                Task = t,
                                                RecordType = t.RECORD_TYPE,
                                                RecordID = t.RECORD_ID,
                                                Person = p,
                                                Detail = c
                                            }).OrderBy(l => l.RecordID).ToList();
                        }
                        
                        decimal recordID = 0;
                        List<PLANT> plantList = new List<PLANT>();
                        foreach (TaskItem taskItem in probTaskList)
                        {
                            taskItem.Taskstatus = CalculateTaskStatus(taskItem.Task);
                            if (taskItem.RecordID != recordID)
                            {
                                recordID = taskItem.RecordID;
                                //taskItem.RecordKey = taskItem.Task.RECORD_ID.ToString();
                                PROB_CASE theCase = (PROB_CASE)taskItem.Detail;
                                try
                                {
                                    if (theCase.PROBCASE_TYPE == "EHS")     // EHS cases only track detected location
                                        plantList = (from o in entities.PROB_OCCUR
                                                     where (o.PROBCASE_ID == taskItem.RecordID)
                                                     join i in entities.INCIDENT on o.INCIDENT_ID equals i.INCIDENT_ID
                                                     join p in entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
                                                     select p).ToList();
                                    else     // quality cases track both detected and responsible  locations - report responsible location here
                                        plantList = (from o in entities.PROB_OCCUR
                                                     where (o.PROBCASE_ID == taskItem.RecordID)
                                                     join i in entities.INCIDENT on o.INCIDENT_ID equals i.INCIDENT_ID
                                                     join p in entities.PLANT on i.RESP_PLANT_ID equals p.PLANT_ID
                                                     select p).ToList();
                                }
                                catch
                                {
                                    plantList = null;
                                }
                            }
                            if (plantList != null)
                            {
                                taskItem.Plant = plantList.FirstOrDefault();
                            }
                            taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString() + "|" + taskItem.Task.TASK_STEP;
                            taskItem.Title = taskItem.LongTitle = "Problem Case " + WebSiteCommon.GetXlatValue("caseStep", taskItem.Task.TASK_STEP);
                            if (taskItem.Plant != null)
                                taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + taskItem.Title;
                            taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Case ") + ": " + ((PROB_CASE)taskItem.Detail).DESC_SHORT;
                        }
                    }

                    taskList.AddRange(probTaskList);
                }
                catch (Exception ex)
                {
                    //SQMLogger.LogException(ex);
                }
            }

            return taskList;
        }

        public static List<TaskItem> ProfileInputSchedule(DateTime fromDate, DateTime toDate, List<decimal> responsibleIDS, decimal[] plantIDS, bool addCurrencyXref)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            List<EHSProfile> plantProfileList = new List<EHSProfile>();
            List<EHS_PROFILE_MEASURE> reqList = new List<EHS_PROFILE_MEASURE>();

            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    // fetch profile measures that are: assiged to the user, assigned to the delegator, assigned to plants escalated to the user
                    reqList = (from i in entities.EHS_PROFILE_MEASURE.Include("EHS_MEASURE").Include("EHS_PROFILE").Include("EHS_PROFILE.PLANT").Include("PERSON")
                               where ((plantIDS.Contains(i.PLANT_ID) || responsibleIDS.Contains((decimal)i.RESPONSIBLE_ID))) /* && i.IS_REQUIRED == true) */
                               select i).OrderBy(l => l.PLANT_ID).ToList();

                    foreach (WebSiteCommon.DatePeriod period in WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, DateSpanOption.SelectRange, ""))
                    {
                        decimal[] plantArray = reqList.Select(l => l.PLANT_ID).Distinct().ToArray();
                        EHSProfile profile = null;
                        foreach (int plantID in plantArray)
                        {
                            string respMeasures = "";
                            string reqMeasures = "";
                            string reqResponsible = "";
                            decimal responsibleID = 0;
                            bool isResponsible = false;
                            bool isReqResponsible = false;
                            foreach (EHS_PROFILE_MEASURE pm in reqList.Where(l => l.PLANT_ID == plantID))
                            {
                                if (responsibleIDS.Contains((decimal)pm.RESPONSIBLE_ID))
                                {
                                    responsibleID = (decimal)pm.RESPONSIBLE_ID;
                                    if (string.IsNullOrEmpty(pm.MEASURE_PROMPT))
                                        respMeasures += (pm.EHS_MEASURE.MEASURE_NAME.Trim() + ",");
                                    else
                                        respMeasures += (pm.MEASURE_PROMPT.Trim() + ",");    // use measure prompt if exists
                                   
                                    isResponsible = true;
                                    if (string.IsNullOrEmpty(reqResponsible) || !reqResponsible.Contains(SQMModelMgr.FormatPersonListItem(pm.PERSON)))
                                        reqResponsible += (SQMModelMgr.FormatPersonListItem(pm.PERSON) + ",");
                                }

                                if ((profile = plantProfileList.Where(l => l.Plant.PLANT_ID == plantID).FirstOrDefault()) == null)
                                {
                                    profile = new EHSProfile();
                                    profile.Profile = reqList.Where(l => l.PLANT_ID == plantID).Select(l => l.EHS_PROFILE).FirstOrDefault();
                                    profile.Plant = reqList.Where(l => l.PLANT_ID == plantID).Select(l => l.EHS_PROFILE.PLANT).FirstOrDefault();
                                    profile.PeriodList = new List<EHSProfilePeriod>();
                                    plantProfileList.Add(profile);
                                }
                            }
                            // only display inputs beyond today's date
                            DateTime taskDate = new DateTime(period.FromDate.Year, period.FromDate.Month, profile.Profile.DAY_DUE);
                            if (taskDate.Date > DateTime.Now.Date)
                            {
                                TaskItem taskItem = new TaskItem();
                                taskItem.RecordType = Convert.ToInt32(TaskRecordType.ProfileInput);
                                taskItem.TaskDate = taskDate;
                                if (taskItem.TaskDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)))
                                    taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + profile.Plant.PLANT_ID + "~" + period.FromDate.Year + "~" + period.FromDate.Month;
                                else
                                    taskItem.RecordKey = "";  // don't allow data input beyond current month
                                taskItem.Taskstatus = TaskStatus.Pending;
                                taskItem.Plant = profile.Plant;
                                taskItem.Title = "Environmental Data Input";
                                taskItem.LongTitle = profile.Plant.PLANT_NAME + " - " + taskItem.Title;
                                taskItem.Description = respMeasures.TrimEnd(',');
                                taskItem.Detail = reqResponsible.TrimEnd(',');
                                taskItem.Task = new TASK_STATUS();
                                taskItem.Task.DUE_DT = new DateTime(period.FromDate.Year, period.FromDate.Month, profile.Profile.DAY_DUE);
                                taskItem.Task.COMPLETE_ID = 0;
                                taskItem.Task.STATUS = "0";
                                taskItem.Task.TASK_TYPE = "C";
                                if (responsibleIDS.Count > 0)
                                    taskItem.NotifyType = SetNotifyType(responsibleIDS[0], responsibleID, taskItem.Taskstatus);
                                taskList.Add(taskItem);
                            }
                        }
                    }
                }
            }
            catch
            {
                //  SQMLogger.LogException(ex);
            }

            // get approval schedule
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    var profileList = (from e in entities.EHS_PROFILE
                                       where (plantIDS.Contains(e.PLANT_ID) || responsibleIDS.Contains((decimal)e.APPROVER_ID))
                                       join p in entities.PLANT on e.PLANT_ID equals p.PLANT_ID
                                       join u in entities.PERSON on e.APPROVER_ID equals u.PERSON_ID into u_e
                                       from u in u_e.DefaultIfEmpty()
                                       select new
                                       {
                                           Profile = e,
                                           Plant = p,
                                           Person = u
                                       }).ToList();

                    if (profileList.Count() == 0)
                        return taskList;

                    foreach (WebSiteCommon.DatePeriod period in WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, DateSpanOption.SelectRange, ""))
                    {
                        foreach (var o in profileList)
                        {
                            EHS_PROFILE profile = (EHS_PROFILE)o.Profile;
                            DateTime taskDate = new DateTime(period.FromDate.Year, period.FromDate.Month, profile.DAY_DUE);
                            // only display approvals beyond today's date
                            if (taskDate.Date > DateTime.Now.Date)
                            {
                                PLANT plant = (PLANT)o.Plant;
                                PERSON person = (PERSON)o.Person;
                                TaskItem taskItem = new TaskItem();
                                taskItem.RecordType = Convert.ToInt32(TaskRecordType.ProfileInputApproval);
                                taskItem.TaskDate = new DateTime(period.FromDate.Year, period.FromDate.Month, profile.DAY_DUE);
                                if (taskItem.TaskDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)))
                                    taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + plant.PLANT_ID + "~" + period.FromDate.Year + "~" + period.FromDate.Month;
                                else
                                    taskItem.RecordKey = "";    // don't allow data input approval beyond current month
                                taskItem.Taskstatus = TaskStatus.Pending;
                                taskItem.Plant = plant;
                                taskItem.Task = new TASK_STATUS();
                                taskItem.Task.DUE_DT = new DateTime(period.FromDate.Year, period.FromDate.Month, profile.DAY_DUE);
                                taskItem.Task.COMPLETE_ID = 0;
                                taskItem.Task.STATUS = "0";
                                taskItem.Task.TASK_TYPE = "C";
                                if (responsibleIDS.Count > 0)
                                    taskItem.NotifyType = SetNotifyType(responsibleIDS[0], (decimal)profile.APPROVER_ID, taskItem.Taskstatus);
                                taskItem.Title = "Environmental Data Input Approval";
                                taskItem.LongTitle = plant.PLANT_NAME + " - " + taskItem.Title;
                                taskItem.Description = "Local Approval Due";
                                taskList.Add(taskItem);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //  SQMLogger.LogException(ex);
            }

            if (addCurrencyXref)
            {
                try
                {
                    SETTINGS sets = SQMSettings.GetSetting("COMPANY", "CURRENCY_XREF");
                    int inputDayDue;
                    if (sets == null || int.TryParse(sets.VALUE.Trim(), out inputDayDue) == false)
                    {
                        inputDayDue = 10;
                    }
                    int backMonths = Convert.ToInt32(Math.Round(toDate.Subtract(fromDate).Days / (365.25 / 12)));
                    foreach (WebSiteCommon.DatePeriod period in WebSiteCommon.CalcDatePeriods(fromDate.AddMonths(backMonths * -1), toDate, DateIntervalType.month, DateSpanOption.SelectRange, ""))
                    {
                        DateTime effDate = period.FromDate.AddMonths(-1);  // lookup the prior month's rate because we assume exchange rates are posted the following month
                        CURRENCY_XREF currentRate = CurrencyConverter.CurrentRate("", effDate.Year, effDate.Month);
                        if (currentRate == null)
                        {
                            TaskItem taskItem = new TaskItem();
                            taskItem.RecordType = Convert.ToInt32(TaskRecordType.CurrencyInput);
                            taskItem.TaskDate = new DateTime(period.FromDate.Year, period.FromDate.Month, inputDayDue);
                            if (taskItem.TaskDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)))
                                taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + effDate.Year + "~" + effDate.Month;
                            else
                                taskItem.RecordKey = "";
                            taskItem.Taskstatus = TaskStatus.Pending;
                            taskItem.Task = new TASK_STATUS();
                            taskItem.Task.DUE_DT = taskItem.TaskDate;
                            taskItem.Task.COMPLETE_ID = 0;
                            taskItem.Task.STATUS = "0";
                            taskItem.Task.TASK_TYPE = "C";
                            taskItem.Title = "Currency Exchange Rates Input";
                            taskItem.LongTitle = SessionManager.PrimaryCompany().COMPANY_NAME + " - " + taskItem.Title;
                            taskItem.Description = "For Month: " + SQMBasePage.FormatDate(effDate, "y", false);
                            taskList.Add(taskItem);
                        }

                    }
                }
                catch (Exception ex)
                {
                    //  SQMLogger.LogException(ex);
                }
            }

            return taskList;
        }

        public static TaskNotification SetNotifyType(decimal inboxID, decimal taskResponssibleID, TaskStatus taskStatus)
        {
            TaskNotification notifyType = TaskNotification.Owner;

            if (taskStatus == TaskStatus.EscalationLevel1 || taskStatus == TaskStatus.EscalationLevel2)
                notifyType = TaskNotification.Escalation;
            else if (taskResponssibleID != inboxID)
                notifyType = TaskNotification.Delegate;

            return notifyType;
        }

        public static TaskStatus SetEscalation(decimal inboxID, TaskItem taskItem)
        {
			if (notifySettings == null || notifySettings.Count == 0)
			{
				notifySettings = SQMSettings.SelectSettingsGroup("NOTIFY", ""); 
			}
			SETTINGS sets = null;

            TaskStatus status = taskItem.Taskstatus;
            TimeSpan delta = DateTime.Now - Convert.ToDateTime(taskItem.Task.DUE_DT);

            if ((sets = notifySettings.Where(s => s.SETTING_CD == "LEVEL1_DAYS").FirstOrDefault()) != null  &&  delta.Days >= Convert.ToInt32(sets.VALUE))
            {
                status = TaskStatus.EscalationLevel1;
            }
			if ((sets = notifySettings.Where(s => s.SETTING_CD == "LEVEL2_DAYS").FirstOrDefault()) != null && delta.Days >= Convert.ToInt32(sets.VALUE))
            {
                status = TaskStatus.EscalationLevel2;
            }

            return status;
        }

        public static List<TaskItem> ProfileInputStatus(DateTime fromDate, DateTime toDate, List<decimal> responsibleIDS, List<decimal> plantIDS)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            List<EHSProfile> plantProfileList = new List<EHSProfile>();
            List<EHS_PROFILE_MEASURE> reqList = new List<EHS_PROFILE_MEASURE>();

            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    // fetch profile measures that are: assiged to the user, assigned to the delegator, assigned to plants escalated to the user
                    reqList = (from i in entities.EHS_PROFILE_MEASURE.Include("EHS_MEASURE").Include("EHS_PROFILE").Include("EHS_PROFILE.PLANT").Include("PERSON")
                               where ((plantIDS.Contains(i.PLANT_ID) || responsibleIDS.Contains((decimal)i.RESPONSIBLE_ID))) /* && i.IS_REQUIRED == true) */
                               select i).OrderBy(l => l.PLANT_ID).ToList();
                }
                if (reqList.Count == 0)
                    return taskList;

                decimal[] plantArray = reqList.Select(l => l.PLANT_ID).Distinct().ToArray();
                SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "IR", fromDate, toDate, plantArray);
                metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
                EHSProfile profile = null;
                foreach (int plantID in plantArray)
                {
                    string calcsScope = "";  // todo: this is clumsy
                    string respMeasures = "";
                    string reqMeasures = "";
                    string reqResponsible = "";
                    decimal responsibleID = 0;
                    bool isResponsible = false;
                    bool isReqResponsible = false;
                    foreach (EHS_PROFILE_MEASURE pm in reqList.Where(l => l.PLANT_ID == plantID))
                    {
                        if (responsibleIDS.Contains((decimal)pm.RESPONSIBLE_ID))
                        {
                            responsibleID = (decimal)pm.RESPONSIBLE_ID;
                            respMeasures += (pm.EHS_MEASURE.MEASURE_NAME.Trim() + ",");
                            if ((bool)pm.IS_REQUIRED)
                            {
                                calcsScope += (EHSModel.ConvertPRODMeasure(pm.EHS_MEASURE, pm.PRMR_ID).ToString() + ",");
                                if (string.IsNullOrEmpty(pm.MEASURE_PROMPT))        // 
                                    reqMeasures += (pm.EHS_MEASURE.MEASURE_NAME.Trim() + ",");
                                else
                                    reqMeasures += (pm.MEASURE_PROMPT.Trim() + ",");    // use measure prompt if exists
                                if (responsibleIDS[0] == pm.RESPONSIBLE_ID)   // determine if user is responsible or is an escalation for required inputs
                                    isReqResponsible = true;
                            }
                        
                            if (string.IsNullOrEmpty(reqResponsible) || !reqResponsible.Contains(SQMModelMgr.FormatPersonListItem(pm.PERSON)))
                                reqResponsible += (SQMModelMgr.FormatPersonListItem(pm.PERSON) + ",");
                            if (responsibleIDS[0] == pm.RESPONSIBLE_ID)   // determine if user is responsible
                                isResponsible = true;
                        }
                    }

                    if ((profile = plantProfileList.Where(l => l.Plant.PLANT_ID == plantID).FirstOrDefault()) == null)
                    {
                        profile = new EHSProfile();
                        profile.Profile = reqList.Where(l => l.PLANT_ID == plantID).Select(l => l.EHS_PROFILE).FirstOrDefault();
                        profile.Plant = reqList.Where(l => l.PLANT_ID == plantID).Select(l => l.EHS_PROFILE.PLANT).FirstOrDefault();
                        profile.PeriodList = new List<EHSProfilePeriod>();
                        plantProfileList.Add(profile);
                    }

                    CalcsResult rslt = metricMgr.CalcsMethods(new decimal[1] { plantID }, "IR", calcsScope.TrimEnd(','), "count", 32, (int)EHSCalcsCtl.SeriesOrder.PeriodMeasurePlant);
                    if (rslt.ValidResult  &&  rslt.metricSeries.Count > 0)
                    {
                        foreach (GaugeSeriesItem item in rslt.metricSeries[0].ItemList)  // iterate by monthly period
                        {
                            bool alertPeriod = false;
                            TaskStatus periodStatus = TaskStatus.Pending;
                            string[] args = item.Text.Split('/');
                            DateTime recordDate = new DateTime(int.Parse(args[0]), int.Parse(args[1]), profile.Profile.DAY_DUE);
                            DateTime dueDate =  recordDate; // recordDate.AddMonths(1);
                            decimal numReq = reqList.Where(l => l.PLANT_ID == plantID  &&  l.IS_REQUIRED == true).Count();   // get num of required inputs for this plant

                            if (item.YValue < numReq) 
                            {
                                // input due within n days for prior month 
                                DateTime warnDate = dueDate.AddDays(profile.Profile.REMINDER_DAYS * -1);
                                if (DateTime.Now >= warnDate  &&  DateTime.Now <= dueDate)
                                {
                                    if (isResponsible)
                                    {
                                        periodStatus = TaskStatus.Due;
                                        alertPeriod = true;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                // inputs past due
                                if (dueDate.Date < DateTime.Now.Date)
                                {
                                    periodStatus = TaskStatus.Overdue;
									alertPeriod = true;
                                }

                                if (alertPeriod)
                                {
                                    // add an inbox task
                                    TaskItem taskItem = new TaskItem();
                                    taskItem.RecordType = Convert.ToInt32(TaskRecordType.ProfileInput);
                                    taskItem.TaskDate = recordDate;
                                    taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + profile.Plant.PLANT_ID + "~" + dueDate.Year + "~" + dueDate.Month;
                                    taskItem.Taskstatus = periodStatus;
                                    taskItem.Plant = profile.Plant;
									//taskItem.Person = person;
                                    taskItem.Title = "Environmental Data Input";
                                    taskItem.LongTitle = profile.Plant.PLANT_NAME + " - " + taskItem.Title;
                                    taskItem.Description = reqMeasures.TrimEnd(',');
                                    taskItem.Detail = reqResponsible.TrimEnd(',');
                                    taskItem.Task = new TASK_STATUS();
                                    taskItem.Task.DUE_DT = dueDate;
                                    taskItem.Task.COMPLETE_ID = 0;
                                    taskItem.Task.STATUS = "0";
                                    taskItem.Task.TASK_TYPE = "C";
									taskItem.Taskstatus = SetEscalation(responsibleIDS[0], taskItem);
                                    taskItem.NotifyType = SetNotifyType(responsibleIDS[0], responsibleID, taskItem.Taskstatus);
                                    taskList.Add(taskItem);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // SQMLogger.LogException(ex);
            }

            // get profile approval status
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    var profileList = (from e in entities.EHS_PROFILE
                                       where (plantIDS.Contains(e.PLANT_ID) || responsibleIDS.Contains((decimal)e.APPROVER_ID))
                                       join p in entities.PLANT on e.PLANT_ID equals p.PLANT_ID
                                       join a in entities.PLANT_ACCOUNTING on e.PLANT_ID equals a.PLANT_ID into a_e
                                       from a in a_e.DefaultIfEmpty()
                                       join u in entities.PERSON on e.APPROVER_ID equals u.PERSON_ID into u_e
                                       from u in u_e.DefaultIfEmpty()
                                       where (EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) <= toDate && a.APPROVER_ID == null)
                                       select new
                                       {
                                           Profile = e,
                                           Plant = p,
                                           Accounting = a,
                                           Person = u
                                       }).ToList();

                    if (profileList.Count() == 0)
                        return taskList;

                    foreach (var o in profileList)
                    {
                        EHS_PROFILE profile = (EHS_PROFILE)o.Profile;
                        PLANT plant = (PLANT)o.Plant;
                        PLANT_ACCOUNTING accounting = (PLANT_ACCOUNTING)o.Accounting;
                        PERSON person = (PERSON)o.Person;

                        bool alertPeriod = false;
                        TaskStatus periodStatus = TaskStatus.Overdue;

                        DateTime recordDate = new DateTime(accounting.PERIOD_YEAR, accounting.PERIOD_MONTH, profile.DAY_DUE);
                        //DateTime dueDate = recordDate.AddMonths(1);
                        DateTime dueDate = recordDate;

                        if (responsibleIDS[0] == ((decimal)profile.APPROVER_ID))   // user is responsible 
                        {
							alertPeriod = true;
							if (dueDate.Year == toDate.Year && dueDate.Month == toDate.Month)
							{
								// need to factor the plant profile due day if in the current month
								if (toDate.Date <= dueDate.Date)
								{
									if (toDate.Date < dueDate.AddDays(profile.REMINDER_DAYS * -1).Date)
										alertPeriod = false;
									else
										periodStatus = TaskStatus.Due;
								}
							}
                        }
                        else
                        {	// is an escallation
							if (dueDate.AddDays(1) <= toDate)
							{
								alertPeriod = true;
								periodStatus = TaskStatus.EscalationLevel1;
							}
							if (dueDate.AddDays(2) <= toDate)
							{
								alertPeriod = true;
								periodStatus = TaskStatus.EscalationLevel2;
							}
                        }

                        if (alertPeriod)
                        {
                            TaskItem taskItem = new TaskItem();
                            taskItem.RecordType = Convert.ToInt32(TaskRecordType.ProfileInputApproval);
                            taskItem.TaskDate = recordDate;
                            taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + plant.PLANT_ID + "~" + dueDate.Year + "~" + dueDate.Month;
                            taskItem.Taskstatus = periodStatus;
                            taskItem.Plant = plant;
							taskItem.Person = person;
                            taskItem.Title = "Environmental Data Input Approval";
                            taskItem.LongTitle = plant.PLANT_NAME + " - " + taskItem.Title;
                            taskItem.Description = "Local Approval";
                            taskItem.Detail = SQMModelMgr.FormatPersonListItem(person);
                            taskItem.Task = new TASK_STATUS();
                            taskItem.Task.DUE_DT = dueDate;
                            taskItem.Task.COMPLETE_ID = 0;
                            taskItem.Task.STATUS = "0";
                            taskItem.Task.TASK_TYPE = "C";
							taskItem.Taskstatus = SetEscalation(responsibleIDS[0], taskItem);
                            taskItem.NotifyType = SetNotifyType(responsibleIDS[0], person.PERSON_ID, taskItem.Taskstatus);
                            taskList.Add(taskItem);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                //  SQMLogger.LogException(ex);
            }

            return taskList;
        }

        public static List<TaskItem> IncidentTaskStatus(decimal companyID, List<decimal> responsibleIDS, List<decimal> plantIDS, bool addProblemCases)
        {
            string[] statusIDS = { ((int)TaskStatus.Pending).ToString(), ((int)TaskStatus.Due).ToString(), ((int)TaskStatus.Overdue).ToString(), ((int)TaskStatus.AwaitingClosure).ToString() };
            DateTime forwardDate = DateTime.Now;
			INCIDENT incident;

            List<TaskItem> taskList = new List<TaskItem>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    taskList = (from t in entities.TASK_STATUS
                                join i in entities.INCIDENT on t.RECORD_ID equals i.INCIDENT_ID 
                                join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
                                join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID into l_i
                                join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID into r_i
                                join q in entities.QI_OCCUR on i.INCIDENT_ID equals q.INCIDENT_ID into q_i
                                where ((t.RECORD_TYPE == (int)TaskRecordType.QualityIssue || t.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident || t.RECORD_TYPE == (int)TaskRecordType.PreventativeAction) && statusIDS.Contains(t.STATUS) &&  t.DUE_DT <= forwardDate  && (responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) || plantIDS.Contains((decimal)(i.DETECT_PLANT_ID))))
                                from p in p_t.DefaultIfEmpty()
                                from q in q_i.DefaultIfEmpty()
                                from l in l_i.DefaultIfEmpty()
                                from r in r_i.DefaultIfEmpty()
                                select new TaskItem
                                {
                                    Task = t,
                                    RecordType = t.RECORD_TYPE,
                                    RecordID = t.RECORD_ID,
                                    Person = p,
                                    Detail = i,
                                    Plant = l,
                                    PlantResponsible = r,
                                    Reference = q
                                }).OrderBy(l => l.RecordID).ToList();

					taskList.AddRange((from t in entities.TASK_STATUS
								join i in entities.AUDIT on t.RECORD_ID equals i.AUDIT_ID
								join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
								join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID into l_i
								where (t.RECORD_TYPE == (int)TaskRecordType.Audit && statusIDS.Contains(t.STATUS) && t.DUE_DT <= forwardDate && (responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) || plantIDS.Contains((decimal)(i.DETECT_PLANT_ID))))
								from p in p_t.DefaultIfEmpty()
								from l in l_i.DefaultIfEmpty()
								select new TaskItem
								{
									Task = t,
									RecordType = t.RECORD_TYPE,
									RecordID = t.RECORD_ID,
									Person = p,
									Detail = i,
									Plant = l,
									PlantResponsible = l
								}).OrderBy(l => l.RecordID).ToList());

                    decimal recordID = 0;
                    List<PLANT> plantList = new List<PLANT>();

                    foreach (TaskItem taskItem in taskList)
                    {
                        taskItem.Taskstatus = CalculateTaskStatus(taskItem.Task);
                        taskItem.Taskstatus = SetEscalation(responsibleIDS[0], taskItem);
                        taskItem.NotifyType = SetNotifyType(responsibleIDS[0], (decimal)taskItem.Task.RESPONSIBLE_ID, taskItem.Taskstatus);

                        TaskRecordType recordType = (TaskRecordType)taskItem.RecordType;
                        switch (recordType)
                        {
                            case TaskRecordType.QualityIssue:
								incident = (INCIDENT)taskItem.Detail;
                                if (taskItem.Reference != null)
                                {
                                    try
                                    {
                                        if (((QI_OCCUR)taskItem.Reference).QS_ACTIVITY == "CST")
                                        {
                                            taskItem.Title = WebSiteCommon.GetXlatValueLong("qualityActivity", ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY);
                                            if (!string.IsNullOrEmpty(taskItem.Task.DESCRIPTION))
                                                taskItem.Title += (" ("+taskItem.Task.DESCRIPTION+")");
                                            taskItem.LongTitle = taskItem.PlantResponsible.PLANT_NAME + " - " + taskItem.Title;
                                        }
                                        else
                                        {
                                            taskItem.Title = WebSiteCommon.GetXlatValueLong("qualityActivity", ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY);
                                            if (!string.IsNullOrEmpty(taskItem.Task.DESCRIPTION))
                                                taskItem.Title += (" (" + taskItem.Task.DESCRIPTION + ")");
                                            taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + taskItem.Title;
                                        }
                                        taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString() + "|" + ((QI_OCCUR)taskItem.Reference).QS_ACTIVITY;
                                        taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Issue ") + ": " + incident.DESCRIPTION;
                                        taskItem.Part = SQMModelMgr.LookupPart(entities, ((QI_OCCUR)taskItem.Reference).PART_ID, "", 1, false);
                                    }
                                    catch { } 
                                }
                                break;
                            case TaskRecordType.HealthSafetyIncident:
                            case TaskRecordType.PreventativeAction:
								incident = (INCIDENT)taskItem.Detail;
                                taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString();
                                taskItem.Title = WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());
                                taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());
                                taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Incident ") + ": " + incident.DESCRIPTION;
                                break;
							case TaskRecordType.Audit:
								AUDIT audit = (AUDIT)taskItem.Detail;
								taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString();
                                taskItem.Title = WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());
                                taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + WebSiteCommon.GetXlatValueLong("EHSIncidentActivity", taskItem.RecordType.ToString());
                                taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Audit ") + ": " + audit.DESCRIPTION;
								break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
            }

            if (addProblemCases)
            {
                List<TaskItem> probTaskList = new List<TaskItem>();
                try
                {
                    using (PSsqmEntities entities = new PSsqmEntities())
                    {
                        probTaskList = (from t in entities.TASK_STATUS
                                        join c in entities.PROB_CASE on t.RECORD_ID equals c.PROBCASE_ID
                                        join p in entities.PERSON on t.RESPONSIBLE_ID equals p.PERSON_ID into p_t
                                        where (t.RECORD_TYPE  == (int)TaskRecordType.ProblemCase && responsibleIDS.Contains((decimal)t.RESPONSIBLE_ID) && statusIDS.Contains(t.STATUS)  &&  t.DUE_DT <= forwardDate)
                                        from p in p_t.DefaultIfEmpty()
                                        select new TaskItem
                                        {
                                            Task = t,
                                            RecordType = t.RECORD_TYPE,
                                            RecordID = t.RECORD_ID,
                                            Person = p,
                                            Detail = c
                                        }).OrderBy(l => l.RecordID).ToList();

                        decimal recordID = 0;
                        List<PLANT> plantList = new List<PLANT>();
                        foreach (TaskItem taskItem in probTaskList)
                        {
                            taskItem.Taskstatus = CalculateTaskStatus(taskItem.Task);
                            if (taskItem.RecordID != recordID)
                            {
                                recordID = taskItem.RecordID;
                                //taskItem.RecordKey = taskItem.Task.RECORD_ID.ToString();
                                PROB_CASE theCase = (PROB_CASE)taskItem.Detail;
                                try
                                {
                                    if (theCase.PROBCASE_TYPE == "EHS")     // EHS cases only track detected location
                                        plantList = (from o in entities.PROB_OCCUR
                                                     where (o.PROBCASE_ID == taskItem.RecordID)
                                                     join i in entities.INCIDENT on o.INCIDENT_ID equals i.INCIDENT_ID
                                                     join p in entities.PLANT on i.DETECT_PLANT_ID equals p.PLANT_ID
                                                     select p).ToList();
                                    else     // quality cases track both detected and responsible  locations - report responsible location here
                                        plantList = (from o in entities.PROB_OCCUR
                                                     where (o.PROBCASE_ID == taskItem.RecordID)
                                                     join i in entities.INCIDENT on o.INCIDENT_ID equals i.INCIDENT_ID
                                                     join p in entities.PLANT on i.RESP_PLANT_ID equals p.PLANT_ID
                                                     select p).ToList();
                                }
                                catch
                                {
                                    plantList = null;
                                }
                            }
                            if (plantList != null)
                            {
                                taskItem.Plant = plantList.FirstOrDefault();
                                taskItem.Taskstatus = SetEscalation(responsibleIDS[0], taskItem);
                            }
                            taskItem.RecordKey = taskItem.RecordType.ToString() + "|" + taskItem.Task.RECORD_ID.ToString() + "|" + taskItem.Task.TASK_STEP;
                            taskItem.Title = taskItem.LongTitle = "Problem Case " + WebSiteCommon.GetXlatValue("caseStep", taskItem.Task.TASK_STEP);
                            if (taskItem.Plant != null)
                                taskItem.LongTitle = taskItem.Plant.PLANT_NAME + " - " + taskItem.Title;
                            //taskItem.Description = ((PROB_CASE)taskItem.Detail).DESC_SHORT;
                            taskItem.Description = WebSiteCommon.FormatID(taskItem.RecordID, 6, "Case ") + ": " + ((PROB_CASE)taskItem.Detail).DESC_SHORT;
                           
                            taskItem.NotifyType = SetNotifyType(responsibleIDS[0], (decimal)taskItem.Task.RESPONSIBLE_ID, taskItem.Taskstatus);
                        }
                    }

                    taskList.AddRange(probTaskList);
                }
                catch (Exception ex)
                {
                    //SQMLogger.LogException(ex);
                }
            }

            return taskList;
        }

        #endregion

        #region sender

        public static List<UserContext> AssignedUserList()
        {
            // get list of users assigned to various transactions (data input, incidents,  approvals, ...)
            List<UserContext> userList = new List<UserContext>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                userList = (from p in entities.PERSON
                            join m in entities.EHS_PROFILE on p.PERSON_ID equals m.APPROVER_ID into tmp
                            from mp in tmp.DefaultIfEmpty()
                            where (p.PERSON_ID == mp.APPROVER_ID)
                            select new UserContext { Person = p }).Distinct().ToList();
                userList.AddRange((from p in entities.PERSON
                                    join m in entities.EHS_PROFILE_MEASURE on p.PERSON_ID equals m.RESPONSIBLE_ID into tmp
                                   from mp in tmp.DefaultIfEmpty()
                                   where (p.PERSON_ID == mp.RESPONSIBLE_ID)
                                    select new UserContext { Person = p }).Distinct().ToList());
                userList.AddRange((from p in entities.PERSON
                                   join m in entities.NOTIFY on p.PERSON_ID equals m.ESCALATE_PERSON1 into tmp
                                   from mp in tmp.DefaultIfEmpty() 
                                   where(p.PERSON_ID == mp.ESCALATE_PERSON1)
                                   select new UserContext { Person = p }).Distinct().ToList());
                userList.AddRange((from p in entities.PERSON
                                   join m in entities.NOTIFY on p.PERSON_ID equals m.ESCALATE_PERSON2 into tmp
                                   from mp in tmp.DefaultIfEmpty()
                                   where (p.PERSON_ID == mp.ESCALATE_PERSON2)
                                   select new UserContext { Person = p }).Distinct().ToList());

                userList = userList.GroupBy(g => new { g.Person.PERSON_ID }).Select(l => l.FirstOrDefault()).ToList();
            }

            return userList;
        }

        public static int MailTaskList(List<TaskItem> taskList, string mailAddress, string context)
        {
            int status = 0;

            PSsqmEntities ctx = new PSsqmEntities();
            List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("", "TASK");
            string url = SQMSettings.SelectSettingByCode(ctx, "MAIL", "TASK", "MailURL").VALUE;

            foreach (TaskItem taskItem in taskList)
            {
                if (taskItem != null)
                {
                    string subject = sets.Where(s => s.SETTING_GROUP == "NOTIFY" && s.SETTING_CD == "SUBJECT").Select(s => s.XLAT_LONG).FirstOrDefault();
                    string body = sets.Where(s => s.SETTING_GROUP == "NOTIFY" && s.SETTING_CD == taskItem.NotifyType.ToString().ToUpper()).Select(s => s.XLAT_LONG).FirstOrDefault();
                    body += "<br /><br />";
                    body += "<b>" + sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "TASK").Select(s => s.XLAT_LONG).FirstOrDefault() + "</b>";
                    body += "<br />";
                    body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "PLANT").Select(s => s.XLAT_LONG).FirstOrDefault() + taskItem.Plant.PLANT_NAME;
                    body += "<br />";
                    body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "DESCR").Select(s => s.XLAT_LONG).FirstOrDefault() + taskItem.Description;
                    body += "<br />";
                    body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "RESPONSIBLE").Select(s => s.XLAT_LONG).FirstOrDefault() + taskItem.Detail;
                    body += "<br />";
                   // if (taskItem.Task != null)
                    //    body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "DUEDATE").Select(s => s.XLAT_LONG).FirstOrDefault() + Convert.ToDateTime(taskItem.Task.DUE_DT).ToShortDateString();
                    if (taskItem.Task != null)
                        body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "DUEDATE").Select(s => s.XLAT_LONG).FirstOrDefault() + Convert.ToDateTime(taskItem.Task.DUE_DT).ToShortDateString();
                    body += "<br />";
                    body += sets.Where(s => s.SETTING_GROUP == taskItem.RecordType.ToString() && s.SETTING_CD == "STATUS").Select(s => s.XLAT_LONG).FirstOrDefault() + taskItem.Taskstatus.ToString();
                    body += "<br /><br />";

                    body += sets.Where(s => s.SETTING_GROUP == "NOTIFY" && s.SETTING_CD == "URL").Select(s => s.XLAT_LONG).FirstOrDefault();

                    switch ((TaskRecordType)taskItem.RecordType)
                    {
                        case TaskRecordType.ProblemCase:
                            url += "Problem/Problem_Case.aspx?c=EHS";
                            break;
                        case TaskRecordType.ProfileInput:
                        case TaskRecordType.ProfileInputApproval:
                            url += "EHS/EHS_MetricInput.aspx";
                            break;
                        case TaskRecordType.HealthSafetyIncident:
                            url += "EHS/EHS_Incident.aspx";
                            break;
                        case TaskRecordType.PreventativeAction:
                            url += "EHS/EHS_Incident.aspx?mode=prevent";
                            break;
                        default:
                            break;
                    }
                    body += url;
                    body += "<br /><br />";
                    body += sets.Where(s => s.SETTING_GROUP == "NOTIFY" && s.SETTING_CD == "NOREPLY").Select(s => s.XLAT_LONG).FirstOrDefault() + "</b>";

                    string rslt = WebSiteCommon.SendEmail(mailAddress, subject, body, "", context);
                    if (string.IsNullOrEmpty(rslt))
                        ++status;
                }
            }

            return status;
        }

        #endregion
    }

}