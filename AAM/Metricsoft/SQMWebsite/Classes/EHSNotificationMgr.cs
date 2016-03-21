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


		public static bool ShouldNotifyPlant(decimal plantID, TaskRecordType recordType)
		{
			PLANT plant = SQMModelMgr.LookupPlant(new PSsqmEntities(), plantID, "");

			return ShouldNotifyPlant(plant, recordType);
		}

		public static bool ShouldNotifyPlant(PLANT plant, TaskRecordType recordType)
		{
			bool shouldNotify = true;

			if (plant != null  &&  plant.PLANT_ACTIVE != null && plant.PLANT_ACTIVE.Count > 0)
			{
				PLANT_ACTIVE plantActive = plant.PLANT_ACTIVE.Where(a=> a.RECORD_TYPE == (int)recordType).FirstOrDefault();
				if (plantActive != null &&  plantActive.ENABLE_EMAIL.HasValue && (bool)plantActive.ENABLE_EMAIL == false)
				{
					shouldNotify = false;
				}
			}

			return shouldNotify;
		}

		public static bool ShouldNotifyPersonPlant(decimal personID, TaskRecordType recordType)
		{
			bool shouldNotify = false;
			PERSON person = SQMModelMgr.LookupPerson(personID, "");

			if (person != null)
			{
				shouldNotify = ShouldNotifyPlant(person.PLANT_ID, recordType);
			}

			return shouldNotify;			
		}

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
					involvedList = SQMModelMgr.GetSupvHierarchy(ctx, SQMModelMgr.LookupPerson(ctx, (decimal)iiDetail.INVOLVED_PERSON_ID, "", false), 1, true);
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
			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;
			EHSIncidentTypeId typeID = (EHSIncidentTypeId)incident.ISSUE_TYPE_ID;
			string notifyScope;
			string incidentLabel;
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "INCIDENT_NOTIFY", "NOTIFY_TASK_ASSIGN" }, 0);

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
			}
			else
			{
				notifyScope = "IN-" + ((int)EHSIncidentTypeId.Any).ToString();
			}

			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)incident.DETECT_PLANT_ID, "");
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			List<PERSON> notifyPersonList = InvolvedPersonList(incident);
			notifyPersonList.AddRange(GetNotifyPersonList(plant, notifyScope, scopeTask, taskStatus));
			notifyPersonList = notifyPersonList.Where(n=> !string.IsNullOrEmpty(n.EMAIL)).GroupBy(l => l.EMAIL).Select(p => p.First()).ToList();

			foreach (PERSON person in notifyPersonList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList())
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, person);
				if (typeID == EHSIncidentTypeId.InjuryIllness)
				{
					incidentLabel = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE", notifyScope, lang.NLS_LANGUAGE).DESCRIPTION;
				}
				else
				{
					incidentLabel = incident.ISSUE_TYPE;
				}
				string actionText = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE_TASK", scopeTask, lang.NLS_LANGUAGE).DESCRIPTION;
				if (taskStatus == ((int)SysPriv.notify).ToString())  // how to use enum instead of literal ?
				{
					actionText = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", ((int)TaskStatus.Overdue).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				}
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incidentLabel + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION + actionText + " - <br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incidentLabel + "<br/>" +
								incident.DESCRIPTION + "<br/>" +
								(!string.IsNullOrEmpty(comment) ? "<br/>"+comment  : "") + 
								 "<br/>" +
								"On : " + DateTime.UtcNow.ToString() +
								"<br/>" +
								"By : " + incident.LAST_UPD_BY +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentPath + incident.INCIDENT_ID.ToString()) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, emailSubject, emailBody, "", "web", null));
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
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

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
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "INCIDENT_NOTIFY", "NOTIFY_TASK_ASSIGN" }, 0);
			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			// 1st send to the person responsible
			if (theTaskItem.Person != null && !string.IsNullOrEmpty(theTaskItem.Person.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(new PSsqmEntities(), theTaskItem.Person);
				string assignedTo = "";
				string emailTo = theTaskItem.Person.EMAIL;
				string actionText = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", ((int)theTaskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", ((int)theTaskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + "<br/>" +
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
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentActionPath) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			// send to supervisor if this is an escalation
			if (theTaskItem.EscalatePerson != null && !string.IsNullOrEmpty(theTaskItem.EscalatePerson.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(new PSsqmEntities(), theTaskItem.EscalatePerson);
				string assignedTo = SQMModelMgr.FormatPersonListItem(theTaskItem.Person, false);
				string emailTo = theTaskItem.EscalatePerson.EMAIL;
				string actionText = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", ((int)TaskStatus.EscalationLevel1).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "INCIDENT_NOTIFY", ((int)TaskStatus.EscalationLevel1).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + "<br/>" +
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
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentActionPath) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;
				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		#endregion


		#region prevactionemail
		public static int NotifyPrevActionStatus(INCIDENT incident, string scopeTask, string comment)
		{
			return NotifyPrevActionStatus(incident, scopeTask, "", comment);
		}

		public static int NotifyPrevActionStatus(INCIDENT incident, string scopeTask, string taskStatus, string comment)
		{
			// send email for INCIDENT status change or activity update
			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;
			EHSIncidentTypeId typeID = (EHSIncidentTypeId)incident.ISSUE_TYPE_ID;
			string notifyScope;
			string incidentLabel;
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "PREVACTION_NOTIFY", "NOTIFY_TASK_ASSIGN" }, 0);

			if ((EHSIncidentTypeId)incident.ISSUE_TYPE_ID == EHSIncidentTypeId.PreventativeAction)
			{
				notifyScope = "RM-" + incident.ISSUE_TYPE_ID.ToString();
			}
			else
			{
				return -1;
			}

			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)incident.DETECT_PLANT_ID, "");
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			List<PERSON> notifyPersonList = GetNotifyPersonList(plant, notifyScope, scopeTask, taskStatus);
			notifyPersonList = notifyPersonList.Where(n => !string.IsNullOrEmpty(n.EMAIL)).GroupBy(l => l.EMAIL).Select(p => p.First()).ToList();

			foreach (PERSON person in notifyPersonList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList())
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, person);
				if (typeID == EHSIncidentTypeId.InjuryIllness)
				{
					incidentLabel = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE", notifyScope, lang.NLS_LANGUAGE).DESCRIPTION;
				}
				else
				{
					incidentLabel = incident.ISSUE_TYPE;
				}
				string actionText = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE_TASK", scopeTask, lang.NLS_LANGUAGE).DESCRIPTION;
				if (taskStatus == ((int)SysPriv.notify).ToString())  // how to use enum instead of literal ?
				{
					actionText = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", ((int)TaskStatus.Overdue).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				}
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incidentLabel + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION + actionText + " - <br/>" +
								"<br/>" +
								"Preventative Action ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incidentLabel + "<br/>" +
								incident.DESCRIPTION + "<br/>" +
								(!string.IsNullOrEmpty(comment) ? "<br/>" + comment : "") +
								 "<br/>" +
								"On : " + DateTime.UtcNow.ToString() +
								"<br/>" +
								"By : " + incident.LAST_UPD_BY +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentPath + incident.INCIDENT_ID.ToString()) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		public static int NotifyPrevActionTaskAssigment(INCIDENT incident, TASK_STATUS theTask, string scopeAction)
		{
			// send email notify of new task assigned
			int status = 0;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			PERSON person = SQMModelMgr.LookupPerson((decimal)theTask.RESPONSIBLE_ID, "");
			if (person != null && !string.IsNullOrEmpty(person.EMAIL))
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "PREVACTION_NOTIFY", "NOTIFY_TASK_ASSIGN" });
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailTo = person.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeAction).FirstOrDefault().DESCRIPTION;
				string emailSubject = XLATList.Where(x => x.XLAT_GROUP == "PREVACTION_NOTIFY" && x.XLAT_CODE == "UPDATE").FirstOrDefault().DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";

				string emailBody = XLATList.Where(x => x.XLAT_GROUP == "PREVACTION_NOTIFY" && x.XLAT_CODE == theTask.STATUS).FirstOrDefault().DESCRIPTION + "<br/>" +
								"<br/>" +
								"Preventative Action ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
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

		public static int NotifyPrevActionTaskStatus(INCIDENT incident, TaskItem theTaskItem, string scopeAction)
		{
			// send email reminders for tasks due, past due or escalated
			int status = 0;
			TASK_STATUS theTask = theTaskItem.Task;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[5] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "PREVACTION_NOTIFY", "NOTIFY_TASK_ASSIGN" }, 0);
			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			// 1st send to the person responsible
			if (theTaskItem.Person != null && !string.IsNullOrEmpty(theTaskItem.Person.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(new PSsqmEntities(), theTaskItem.Person);
				string assignedTo = "";
				string emailTo = theTaskItem.Person.EMAIL;
				string actionText = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", ((int)theTaskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", ((int)theTaskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + "<br/>" +
								"<br/>" +
								"Preventative Action ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentActionPath) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			// send to supervisor if this is an escalation
			if (theTaskItem.EscalatePerson != null && !string.IsNullOrEmpty(theTaskItem.EscalatePerson.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(new PSsqmEntities(), theTaskItem.EscalatePerson);
				string assignedTo = SQMModelMgr.FormatPersonListItem(theTaskItem.Person, false);
				string emailTo = theTaskItem.EscalatePerson.EMAIL;
				string actionText = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", ((int)TaskStatus.EscalationLevel1).ToString(), lang.NLS_LANGUAGE).DESCRIPTION;
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION_SHORT + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = SQMBasePage.GetXLAT(XLATList, "PREVACTION_NOTIFY", ((int)TaskStatus.EscalationLevel1).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + "<br/>" +
								"<br/>" +
								"Preventative Action ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentActionPath) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;
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

			AUDIT audit = EHSAuditMgr.SelectAuditById(entities, auditId);
			AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			string auditType = type.TITLE;
			DateTime dueDate = audit.AUDIT_DT.AddDays(type.DAYS_TO_COMPLETE);

			if (ShouldNotifyPlant((decimal)audit.DETECT_PLANT_ID, TaskRecordType.Audit) == false)
			{
				return;
			}

			PERSON person = SQMModelMgr.LookupPerson(entities, (decimal)audit.AUDIT_PERSON, "", false);
			if (person != null  &&  !string.IsNullOrEmpty(person.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, person);
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[1] { "NOTIFY_AUDIT_ASSIGN" }, lang.LANGUAGE_ID);
				string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = SQMBasePage.GetXLAT(XLATList, "NOTIFY_AUDIT_ASSIGN", "EMAIL_SUBJECT").DESCRIPTION + ": " + auditType;
				string emailBody = SQMBasePage.GetXLAT(XLATList, "NOTIFY_AUDIT_ASSIGN", "EMAIL_01").DESCRIPTION + " " + dueDate.ToString("dddd MM/dd/yyyy") + ".<br/>" +
								"<br/>" +
								auditType + "<br/>" +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_AUDIT_ASSIGN", "EMAIL_02").DESCRIPTION + " (" + appUrl + auditPath + ")";

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, emailSubject, emailBody, "", "web"));
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public static void NotifyAuditStatus(AUDIT audit, TaskItem taskItem)
		{
			var entities = new PSsqmEntities();
			var emailIds = new HashSet<decimal>();
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_AUDIT_ASSIGN", "AUDIT_NOTIFY", "AUDIT_EXCEPTION_STATUS" }, 0);

			AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			string auditType = type.TITLE;
			DateTime dueDate = audit.AUDIT_DT.AddDays(type.DAYS_TO_COMPLETE);

			if (ShouldNotifyPlant((decimal)audit.DETECT_PLANT_ID, TaskRecordType.Audit) == false)
			{
				return;
			}

			PERSON person = SQMModelMgr.LookupPerson(entities, (decimal)audit.AUDIT_PERSON, "", false);
			if (person != null)
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, person);
				string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = SQMBasePage.GetXLAT(XLATList, "AUDIT_NOTIFY", ((int)taskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + ": " + auditType;
				string emailBody = SQMBasePage.GetXLAT(XLATList, "AUDIT_NOTIFY", ((int)taskItem.Taskstatus).ToString(), lang.NLS_LANGUAGE).DESCRIPTION + " " + dueDate.ToString("dddd MM/dd/yyyy") + ".<br/>" +
								"<br/>" +
								auditType + "<br/>" +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_AUDIT_ASSIGN", "EMAIL_02", lang.NLS_LANGUAGE).DESCRIPTION + " (" + appUrl + auditPath + ")";

				if (!string.IsNullOrEmpty(person.EMAIL))
				{
					Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, emailSubject, emailBody, "", "web"));
					thread.IsBackground = true;
					thread.Start();
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
			if (ShouldNotifyPlant(plant, TaskRecordType.HealthSafetyIncident) == false)
			{
				return status;
			}

			DEPARTMENT department = new DEPARTMENT();
			if (audit.DEPT_ID.HasValue)
			{
				if ((department = SQMModelMgr.LookupDepartment(entities, (decimal)audit.DEPT_ID)) == null)
				{
					department = new DEPARTMENT();
					department.DEPT_NAME = "N/A";  // todo get from xlat
				}
			}

			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[7] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "NOTIFY_TASK_ASSIGN", "NOTIFY_AUDIT_ASSIGN", "AUDIT_NOTIFY", "AUDIT_EXCEPTION_STATUS" }, 0);
			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			// 1st send to the person responsible
			if (theTaskItem.Person != null && !string.IsNullOrEmpty(theTaskItem.Person.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, theTaskItem.Person);
				string assignedTo = "";
				string emailTo = theTaskItem.Person.EMAIL;
				string actionText = SQMBasePage.GetXLAT(XLATList, "AUDIT_NOTIFY", "UPDATE", lang.NLS_LANGUAGE).DESCRIPTION;
				string emailSubject = actionText + ": " + type.DESCRIPTION;
				string emailBody = emailSubject + "<br/>" +
								"<br/>" +
								"Audit ID: " + WebSiteCommon.FormatID(audit.AUDIT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "  (" + department.DEPT_NAME + ")" + "<br/>" +
								type.DESCRIPTION + "<br/>" +
								"<br/>" +
								theTask.DETAIL + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + SQMBasePage.FormatDate(Convert.ToDateTime(theTask.DUE_DT), "d", false) + "&nbsp;&nbsp;" + assignedTo + "<br/>" +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION + (appUrl + incidentActionPath) + SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_03", lang.NLS_LANGUAGE).DESCRIPTION_SHORT;

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		#endregion

		public static int NotifyTaskAssigment(TASK_STATUS task, decimal plantID)
		{
			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;
			int recordType = task.RECORD_TYPE;
			decimal recordID = task.RECORD_ID;
			string taskStep = task.TASK_STEP;
			string taskType = task.TASK_TYPE;
			DateTime dueDate = (DateTime)task.DUE_DT;

			if (plantID > 0)  // plant where task originates was supplied.  check if email is active for this plant
			{
				if (ShouldNotifyPlant(plantID, (TaskRecordType)task.RECORD_TYPE) == false)
				{
					return status;
				}
			}
			else
			{
				// use the responsible person's plant to deterime if email is active
				if (ShouldNotifyPersonPlant((decimal)task.RESPONSIBLE_ID, (TaskRecordType)task.RECORD_TYPE) == false)
				{
					return status;
				}
			}

			PERSON person = SQMModelMgr.LookupPerson(entities, (decimal)task.RESPONSIBLE_ID, "", false);
			if (person != null && !string.IsNullOrEmpty(person.EMAIL))
			{
				LOCAL_LANGUAGE lang = SQMModelMgr.LookupPersonLanguage(entities, person);
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_TASK_ASSIGN", "RECORD_TYPE", "NOTIFY_SCOPE_TASK" }, lang.LANGUAGE_ID);
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string recordTypeValue = SQMBasePage.GetXLAT(XLATList, "RECORD_TYPE", recordType.ToString()).DESCRIPTION;
				string taskStepValue = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE_TASK", taskStep).DESCRIPTION;
				string emailTo = person.EMAIL;
				string emailSubject = SQMBasePage.GetXLAT(XLATList, "NOTIFY_SCOPE_TASK", "EMAIL_SUBJECT") + " " + recordTypeValue + " " + taskStepValue;
				string emailBody = SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_01").DESCRIPTION + ":<br/>" +
					"Task Type: " + recordTypeValue + " " + taskStepValue +
								"<br/>" +
					"Due Date: " + dueDate.ToString("dddd MM/dd/yyyy") +
								"<br/>" +
								SQMBasePage.GetXLAT(XLATList, "NOTIFY_TASK_ASSIGN", "EMAIL_02").DESCRIPTION + "(" + appUrl + auditPath + ")";

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

	}

}