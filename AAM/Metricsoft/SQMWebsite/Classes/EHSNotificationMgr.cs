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

		public static List<PERSON> GetNotifyPersonList(PLANT plant, string notifyScope, string notifyOnTask)
		{
			List<PERSON> notifyPersonList = new List<PERSON>();
			List<NOTIFYACTION> notifyList = new List<NOTIFYACTION>();
			
			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), null, null).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask).ToList();  // corp level
			notifyPersonList.AddRange(SQMModelMgr.SelectPrivgroupPersonList(ParseNotifyGroups(notifyList).ToArray()));

			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), (decimal)plant.BUS_ORG_ID, null).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask).ToList();  // BU level
			notifyPersonList.AddRange(SQMModelMgr.SelectBusOrgPrivgroupPersonList((decimal)plant.BUS_ORG_ID, ParseNotifyGroups(notifyList).ToArray()));

			notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), null, (decimal)plant.PLANT_ID).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask).ToList();  // plant level
			notifyPersonList.AddRange(SQMModelMgr.SelectPlantPrivgroupPersonList((decimal)plant.PLANT_ID, ParseNotifyGroups(notifyList).ToArray()));

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

		public static int NotifyIncidentStatus(INCIDENT incident, string scopeAction, string comment)
		{
			int status = 0;
			string notifyScope;
			string incidentLabel;
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS" });

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
			notifyPersonList.AddRange(GetNotifyPersonList(plant, notifyScope, scopeAction));

			if (notifyPersonList.Count > 0)
			{
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeAction).FirstOrDefault().DESCRIPTION;
				string emailSubject = "Health/Safety Incident " + actionText + ": " + incidentLabel + " (" + plant.PLANT_NAME + ")";
				string emailBody = "The following Health/Safety incident has been " + actionText + " - <br/>" +
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
								"Please log in to " + (appUrl+incidentPath+incident.INCIDENT_ID.ToString()) + " to view this incident.";

				string emailTo = "";
				foreach (PERSON person in notifyPersonList.Where(l => !string.IsNullOrEmpty(l.EMAIL)).ToList())
				{
					emailTo += string.IsNullOrEmpty(emailTo) ? person.EMAIL : ("," + person.EMAIL);
				}

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			//WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null);

			return status;
		}

		public static int NotifyIncidentTaskAssigment(INCIDENT incident, TASK_STATUS theTask, string scopeAction)
		{
			int status = 0;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			PERSON person = SQMModelMgr.LookupPerson((decimal)theTask.RESPONSIBLE_ID, "");

			if (person != null  &&  !string.IsNullOrEmpty(person.EMAIL))
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS" });
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailTo = person.EMAIL;
				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeAction).FirstOrDefault().DESCRIPTION;
				string emailSubject = "Health/Safety Incident " + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = "You have been assigned to one or more tasks regarding the following Incident: <br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								incident.DESCRIPTION + "<br/>" +
								"<br/>" +
								theTask.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Due : " + theTask.DUE_DT.ToString() + "<br/>" +
								"<br/>" +
								"Please log in to " + (appUrl+incidentActionPath) + " to view this task.";

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		public static void NotifyOnAuditCreate(decimal auditId, decimal personId)
		{
			var entities = new PSsqmEntities();

			//decimal companyId = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
			//decimal busOrgId = SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID;
			var emailIds = new HashSet<decimal>();

			AUDIT audit = EHSAuditMgr.SelectAuditById(entities, auditId);
			string auditType = EHSAuditMgr.SelectAuditTypeByAuditId(auditId);
			emailIds.Add((decimal)audit.AUDIT_PERSON);

			if (emailIds.Count > 0)
			{
				string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = "Audit Created: " + auditType;
				string emailBody = "A new audit has been created:<br/>" +
								"<br/>" +
								auditType + "<br/>" +
								"<br/>" +
								"Please log in to " + appUrl+auditPath + " to view the audit.";

				foreach (decimal eid in emailIds)
				{
					string emailAddress = (from p in entities.PERSON where p.PERSON_ID == eid select p.EMAIL).FirstOrDefault();

					Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "", "web"));
					thread.IsBackground = true;
					thread.Start();

					//WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");
				}
			}
		}

	}

}