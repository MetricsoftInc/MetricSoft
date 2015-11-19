using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Configuration;


namespace SQM.Website
{
	public static class EHSNotificationMgr
	{
		public static string incidentPath = "/EHS/EHS_Incidents.aspx?r=";
		public static string incidentActionPath = "/Home/Calendar.aspx?v=T";
		public static string auditPath = "/EHS/EHS_Audits.aspx";
		public static string auditActionPath = "/Home/Calendar.aspx?v=T";

		#region helpers

		public static List<PERSON> GetNotifyPersonList(PLANT plant, string notifyScope, string notifyOnTask, string notifyOnTaskStatus)
		{
			List<PERSON> notifyPersonList = new List<PERSON>();
			List<NOTIFYACTION> notifyList = new List<NOTIFYACTION>();
			
			//todo: filter notifyList by task status

			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), null, null).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask && (string.IsNullOrEmpty(notifyOnTaskStatus) || l.TASK_STATUS == notifyOnTaskStatus)).ToList();  // corp level
			notifyPersonList.AddRange(SQMModelMgr.SelectPrivgroupPersonList(ParseNotifyGroups(notifyList).ToArray()));

			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), (decimal)plant.BUS_ORG_ID, null).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask && (string.IsNullOrEmpty(notifyOnTaskStatus) || l.TASK_STATUS == notifyOnTaskStatus)).ToList();  // BU level
			notifyPersonList.AddRange(SQMModelMgr.SelectBusOrgPrivgroupPersonList((decimal)plant.BUS_ORG_ID, ParseNotifyGroups(notifyList).ToArray()));

			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), null, (decimal)plant.PLANT_ID).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask && (string.IsNullOrEmpty(notifyOnTaskStatus) || l.TASK_STATUS == notifyOnTaskStatus)).ToList();  // plant level
			notifyPersonList.AddRange(SQMModelMgr.SelectPlantPrivgroupPersonList((decimal)plant.PLANT_ID, ParseNotifyGroups(notifyList).ToArray(), true));

			return notifyPersonList.GroupBy(l => l.PERSON_ID).Select(l => l.First()).ToList();
		}

		static List<string> ParseNotifyGroups(List<NOTIFYACTION> notifyList)
		{
			List<string> notifyGroupList = new List<string>();

			foreach (NOTIFYACTION notify in notifyList.GroupBy(l => l.NOTIFYACTION_ID).Select(l => l.First()).ToList())
			{
				foreach (string gp in notify.NOTIFY_DIST.Split(','))
				{
					notifyGroupList.Add(gp);
				}
			}

			return notifyGroupList;
		}

		public static List<PERSON> InvolvedPersonList(INCIDENT incident)
		{
			List<PERSON> involvedList = new List<PERSON>();

			if (incident.ISSUE_TYPE_ID == (int)EHSIncidentTypeId.InjuryIllness)
			{
				PSsqmEntities ctx = new PSsqmEntities();
				INCFORM_INJURYILLNESS iiDetail = EHSIncidentMgr.SelectInjuryIllnessDetailsById(ctx, incident.INCIDENT_ID);
				if (iiDetail != null && iiDetail.INVOLVED_PERSON_ID.HasValue)
				{
					involvedList = SQMModelMgr.GetSupvHierarchy(ctx, SQMModelMgr.LookupPerson(ctx, (decimal)iiDetail.INVOLVED_PERSON_ID, "", false), 2, true);
				}
			}

			return involvedList;
		}

		#endregion

		#region incidentemail
		public static int NotifyIncidentStatus(INCIDENT incident, string scopeTask, string comment)
		{
			return NotifyIncidentStatus(incident, scopeTask, "", comment);
		}

		public static int NotifyIncidentStatus(INCIDENT incident, string scopeTask, string taskStatus, string comment)
		{
			// send email for INCIDENT status change or activity update
			int status = 0;
			string notifyScope;
			string incidentLabel;
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "INCIDENT_NOTIFY", "NOTIFY_TASK_ASSIGN" });

			if ((EHSIncidentTypeId)incident.ISSUE_TYPE_ID == EHSIncidentTypeId.InjuryIllness)
			{
				notifyScope = "IN-" + incident.ISSUE_TYPE_ID.ToString();
				INCFORM_INJURYILLNESS injuryIllnessDetail = EHSIncidentMgr.SelectInjuryIllnessDetailsById(new PSsqmEntities(), incident.INCIDENT_ID);
				if (injuryIllnessDetail != null)
				{
					if ((bool)injuryIllnessDetail.FATALITY == true)
						notifyScope += "-X";
					else if ((bool)injuryIllnessDetail.LOST_TIME == true)
						notifyScope += "-T";
					else if ((bool)injuryIllnessDetail.RECORDABLE == true)
						notifyScope += "-R";
				}
				incidentLabel = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE" && x.XLAT_CODE == notifyScope).FirstOrDefault().DESCRIPTION;
			}
			else
			{
				notifyScope = "IN-" + ((int)EHSIncidentTypeId.Any).ToString();
				incidentLabel = incident.ISSUE_TYPE;
			}

			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			List<PERSON> notifyPersonList = InvolvedPersonList(incident);
			notifyPersonList.AddRange(GetNotifyPersonList(plant, notifyScope, scopeTask, taskStatus));

			if (notifyPersonList.Count > 0)
			{
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeTask).FirstOrDefault().DESCRIPTION;
				if (taskStatus == ((int)SysPriv.notify).ToString())  // how to use enum instead of literal ?
				{
					actionText = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == ((int)TaskStatus.Overdue).ToString()).FirstOrDefault().DESCRIPTION;
				}
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION_SHORT + actionText + ": " + incidentLabel + " (" + plant.PLANT_NAME + ")";
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION + actionText + " - <br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incidentLabel + "<br/>" +
								incident.DESCRIPTION + "<br/>" +
								(!string.IsNullOrEmpty(comment) ? "<br/>"+comment  : "") + 
								 "<br/>" +
								"On : " + DateTime.Now.ToString() +
								"<br/>" +
								"By : " + incident.LAST_UPD_BY +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION + (appUrl + incidentPath + incident.INCIDENT_ID.ToString()) + XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION_SHORT;

				string emailTo = "";
				foreach (PERSON person in notifyPersonList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList())
				{
					emailTo += string.IsNullOrEmpty(emailTo) ? person.EMAIL : ("," + person.EMAIL);
				}

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		public static int NotifyIncidentTaskAssigment(INCIDENT incident, TASK_STATUS theTask, string scopeAction)
		{
			// send email notify of new task assigned
			int status = 0;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			PERSON person = SQMModelMgr.LookupPerson((decimal)theTask.RESPONSIBLE_ID, "");

			if (person != null  &&  !string.IsNullOrEmpty(person.EMAIL))
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "INCIDENT_NOTIFY", "NOTIFY_TASK_ASSIGN" });
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailTo = person.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeAction).FirstOrDefault().DESCRIPTION;
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";

				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == theTask.STATUS).FirstOrDefault().DESCRIPTION + "<br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								incident.DESCRIPTION + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION + (appUrl + incidentActionPath) + XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		public static int NotifyIncidentTaskStatus(INCIDENT incident, TaskItem theTaskItem, string scopeAction)
		{
			// send email reminders for tasks due, past due or escalated
			int status = 0;
			TASK_STATUS theTask = theTaskItem.Task;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);

			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "INCIDENT_NOTIFY", "NOTIFY_TASK_ASSIGN" });
			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			// 1st send to the person responsible
			if (theTaskItem.Person != null && !string.IsNullOrEmpty(theTaskItem.Person.EMAIL))
			{
				string assignedTo = "";
				string emailTo = theTaskItem.Person.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == ((int)theTaskItem.Taskstatus).ToString()).FirstOrDefault().DESCRIPTION;
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == ((int)theTaskItem.Taskstatus).ToString()).FirstOrDefault().DESCRIPTION + "<br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION + (appUrl + incidentActionPath) + XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			// send to supervisor if this is an escalation
			if (theTaskItem.EscalatePerson != null && !string.IsNullOrEmpty(theTaskItem.EscalatePerson.EMAIL))
			{
				string assignedTo = SQMModelMgr.FormatPersonListItem(theTaskItem.Person, false);
				string emailTo = theTaskItem.EscalatePerson.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == ((int)TaskStatus.EscalationLevel1).ToString()).FirstOrDefault().DESCRIPTION;
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "INCIDENT_NOTIFY" && x.XLAT_CODE == ((int)TaskStatus.EscalationLevel1).ToString()).FirstOrDefault().DESCRIPTION + "<br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION + (appUrl + incidentActionPath) + XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		#endregion
		#region auditemail
		public static void NotifyOnAuditCreate(decimal auditId, decimal personId)
		{
			var entities = new PSsqmEntities();

			//decimal companyId = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
			//decimal busOrgId = SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID;
			var emailIds = new HashSet<decimal>();

			AUDIT audit = EHSAuditMgr.SelectAuditById(entities, auditId);
			AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			string auditType = type.TITLE;
			emailIds.Add((decimal)audit.AUDIT_PERSON);
			DateTime dueDate = audit.AUDIT_DT.AddDays(type.DAYS_TO_COMPLETE);
			if (emailIds.Count > 0)
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[1] { "NOTIFY_AUDIT_ASSIGN" });
				string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_AUDIT_ASSIGN" && x.XLAT_CODE == "EMAIL_SUBJECT").FirstOrDefault().DESCRIPTION + ": " + auditType;
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_AUDIT_ASSIGN" && x.XLAT_CODE == "EMAIL_01").FirstOrDefault().DESCRIPTION + " " + dueDate.ToString("dddd MM/dd/yyyy") + ".<br/>" +
								"<br/>" +
								auditType + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_AUDIT_ASSIGN" && x.XLAT_CODE == "EMAIL_02").FirstOrDefault().DESCRIPTION + " (" + appUrl + auditPath + ")";

				foreach (decimal eid in emailIds)
				{
					string emailAddress = (from p in entities.PERSON where p.PERSON_ID == eid select p.EMAIL).FirstOrDefault();

					if (!string.IsNullOrEmpty(emailAddress))
					{
						Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "", "web"));
						thread.IsBackground = true;
						thread.Start();

						//WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");
					}
				}
			}
		}

		public static void NotifyAuditStatus(AUDIT audit, TaskItem taskItem)
		{
			var entities = new PSsqmEntities();
			var emailIds = new HashSet<decimal>();
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_AUDIT_ASSIGN", "AUDIT_NOTIFY", "AUDIT_EXCEPTION_STATUS" });

			AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			string auditType = type.TITLE;
			emailIds.Add((decimal)audit.AUDIT_PERSON);
			DateTime dueDate = audit.AUDIT_DT.AddDays(type.DAYS_TO_COMPLETE);
			if (emailIds.Count > 0)
			{

				string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "AUDIT_NOTIFY" && x.XLAT_CODE == ((int)taskItem.Taskstatus).ToString()).FirstOrDefault().DESCRIPTION + ": " + auditType;
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "AUDIT_NOTIFY" && x.XLAT_CODE == ((int)taskItem.Taskstatus).ToString()).FirstOrDefault().DESCRIPTION + " " + dueDate.ToString("dddd MM/dd/yyyy") + ".<br/>" +
								"<br/>" +
								auditType + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_AUDIT_ASSIGN" && x.XLAT_CODE == "EMAIL_02").FirstOrDefault().DESCRIPTION + " (" + appUrl + auditPath + ")";

				foreach (decimal eid in emailIds)
				{
					string emailAddress = (from p in entities.PERSON where p.PERSON_ID == eid select p.EMAIL).FirstOrDefault();

					if (!string.IsNullOrEmpty(emailAddress))
					{
						Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "", "web"));
						thread.IsBackground = true;
						thread.Start();

						//WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");
					}
				}
			}
		}

		public static int NotifyAuditTaskStatus(AUDIT audit, TaskItem theTaskItem, string scopeAction)
		{
			// send email reminders for tasks due, past due or escalated
			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;
			TASK_STATUS theTask = theTaskItem.Task;
			AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)audit.DETECT_PLANT_ID, "");
			DEPARTMENT department = new DEPARTMENT();
			if (audit.DEPT_ID.HasValue)
			{
				if ((department = SQMModelMgr.LookupDepartment(entities, (decimal)audit.DEPT_ID)) == null)
					department = new DEPARTMENT();
			}

			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[7] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "NOTIFY_TASK_ASSIGN", "NOTIFY_AUDIT_ASSIGN", "AUDIT_NOTIFY", "AUDIT_EXCEPTION_STATUS" });
			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			// 1st send to the person responsible
			if (theTaskItem.Person != null && !string.IsNullOrEmpty(theTaskItem.Person.EMAIL))
			{
				string assignedTo = "";
				string emailTo = theTaskItem.Person.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "AUDIT_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION;
				string emailSubject = actionText + ": " + type.DESCRIPTION;
				string emailBody = emailSubject + "<br/>" +
								"<br/>" +
								"Audit ID: " + WebSiteCommon.FormatID(audit.AUDIT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "  (" + department.DEPT_NAME + ")"+"<br/>" +
								type.DESCRIPTION + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION + (appUrl + incidentActionPath) + XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_03").FirstOrDefault().DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		#endregion

		public static int NotifyTaskAssigment(TASK_STATUS task)
		{
			int status = 0;
			int recordType = task.RECORD_TYPE;
			decimal recordID = task.RECORD_ID;
			//decimal recordSubID = (decimal)task.RECORD_SUBID;
			string taskStep = task.TASK_STEP;
			string taskType = task.TASK_TYPE;
			decimal personId = (decimal)task.RESPONSIBLE_ID;
			DateTime dueDate = (DateTime)task.DUE_DT;

			PERSON person = SQMModelMgr.LookupPerson(personId, "");

			if (person != null && !string.IsNullOrEmpty(person.EMAIL))
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_TASK_ASSIGN", "RECORD_TYPE", "NOTIFY_SCOPE_TASK" });
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string recordTypeValue = XLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == recordType.ToString()).FirstOrDefault().DESCRIPTION;
				string taskStepValue = XLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == taskStep).FirstOrDefault().DESCRIPTION;

				string emailTo = person.EMAIL;
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_SUBJECT").FirstOrDefault().DESCRIPTION + " " + recordTypeValue + " " + taskStepValue;
				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_01").FirstOrDefault().DESCRIPTION + ":<br/>" +
					"Task Type: " + recordTypeValue + " " + taskStepValue +
								"<br/>" +
					"Due Date: " + dueDate.ToString("dddd MM/dd/yyyy") +
								"<br/>" +
								XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_ASSIGN" && x.XLAT_CODE == "EMAIL_02").FirstOrDefault().DESCRIPTION + "(" + appUrl + auditPath + ")";

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

	}

}