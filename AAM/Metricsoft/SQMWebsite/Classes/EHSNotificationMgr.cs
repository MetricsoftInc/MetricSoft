using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Configuration;


namespace SQM.Website
{
	public static class EHSNotificationMgr
	{
		#region helpers

		public static List<PERSON> GetNotifyPersonList(PLANT plant, string notifyScope, string notifyOnTask)
		{
			List<NOTIFYACTION> notifyList = SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), (decimal)plant.BUS_ORG_ID, 0).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask).ToList();
			notifyList.AddRange(SQMModelMgr.SelectNotifyActionList(new PSsqmEntities(), 0, (decimal)plant.PLANT_ID).Where(l => l.NOTIFY_SCOPE == notifyScope && l.SCOPE_TASK == notifyOnTask).ToList());

			List<string> notifyGroupList = new List<string>();
			foreach (NOTIFYACTION notify in notifyList.GroupBy(l=> l.NOTIFYACTION_ID).Select(l=> l.First()).ToList())
			{
				foreach (string gp in notify.NOTIFY_DIST.Split(','))
				{
					notifyGroupList.Add(gp);
				}
			}

			List<PERSON> notifyPersonList = SQMModelMgr.SelectPlantPrivgroupPersonList((decimal)plant.PLANT_ID, notifyGroupList.ToArray());
			notifyPersonList.AddRange(SQMModelMgr.SelectBusOrgPrivgroupPersonList((decimal)plant.BUS_ORG_ID, notifyGroupList.ToArray()));

			return notifyPersonList.GroupBy(l => l.PERSON_ID).Select(l => l.First()).ToList();
		}

		#endregion

		public static int NotifyIncidentStatus(INCIDENT incident, string notifyScope, string scopeAction)
		{
			int status = 0;
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);
			List<PERSON> notifyPersonList = GetNotifyPersonList(plant, notifyScope, scopeAction);

			if (notifyPersonList.Count > 0)
			{
				List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[3] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS" });
				string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string actionText = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == scopeAction).FirstOrDefault().DESCRIPTION;
				string emailSubject = "Health/Safety Incident " + actionText + ": " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
				string emailBody = "A new Health/Safety incident has been " + actionText + " :<br/>" +
								"<br/>" +
								"Incident ID: " + WebSiteCommon.FormatID(incident.INCIDENT_ID, 6) + "<br/>" +
								plant.PLANT_NAME + "<br/>" +
								incident.ISSUE_TYPE + "<br/>" +
								"<br/>" +
								incident.DESCRIPTION + "<br/>" +
								"<br/>" +
								"On : " + DateTime.Now.ToString() +
								"<br/>" +
								"By : " + incident.LAST_UPD_BY +
								"<br/>" +
								"Please log in to " + appUrl + " to view this incident.";

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
								"Please log in to " + appUrl + " to view this incident.";

				Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null));
				thread.IsBackground = true;
				thread.Start();
			}

			return status;
		}

		public static void NotifyOnAuditCreate(decimal auditId, decimal personId)
		{
			var entities = new PSsqmEntities();

			decimal companyId = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
			decimal busOrgId = SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID;
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
								"Please log in to " + appUrl + " to view the audit.";

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