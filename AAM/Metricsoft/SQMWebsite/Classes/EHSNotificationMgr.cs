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

		public static int NotifyIncidentStatus(INCIDENT incident, string notifyScope, string notifyOnTask)
		{
			int status = 0;
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[4] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "NOTIFY_TIMING" });
			PLANT plant = SQMModelMgr.LookupPlant((decimal)incident.DETECT_PLANT_ID);

			List<PERSON> notifyPersonList = GetNotifyPersonList(plant, notifyScope, notifyOnTask);

			string appUrl = SQMSettings.SelectSettingByCode(new PSsqmEntities(), "MAIL", "TASK", "MailURL").VALUE;
			if (string.IsNullOrEmpty(appUrl))
				appUrl = "the website";

			string emailSubject = "Health/Safety Incident Created: " + incident.ISSUE_TYPE + " (" + plant.PLANT_NAME + ")";
			string emailBody = "A new Health/Safety incident has been created:<br/>" +
							"<br/>" +
							plant.PLANT_NAME + "<br/>" +
							incident.ISSUE_TYPE + "<br/>" +
							"<br/>" +
							incident.DESCRIPTION + "<br/>" +
							"<br/>" +
							"Please log in to " + appUrl + " to view this incident.";

			string emailTo = "";
			foreach (PERSON person in notifyPersonList.Where(l=> !string.IsNullOrEmpty(l.EMAIL)).ToList())
			{
				emailTo += string.IsNullOrEmpty(emailTo) ? person.EMAIL : (","+person.EMAIL);
			}

			//WebSiteCommon.SendEmail(emailTo, emailSubject, emailBody, "", "web", null);

			return status;
		}

		public static void NotifyOnCreate(decimal incidentId, decimal plantId)
		{
			var entities = new PSsqmEntities();

			decimal companyId = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
			decimal busOrgId = SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID;
			var emailIds = new HashSet<decimal>();

			INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);
			string incidentLocation = EHSIncidentMgr.SelectIncidentLocationNameByIncidentId(incidentId);
			string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(incidentId);

            List<ATTACHMENT> attachList = SQM.Website.Classes.SQMDocumentMgr.SelectAttachmentListByRecord(40, incidentId, "", "");

			List<NOTIFY> notifications = SQMModelMgr.SelectNotifyHierarchy(companyId, busOrgId, plantId, 0, TaskRecordType.HealthSafetyIncident);

			foreach (NOTIFY n in notifications)
			{
				if (n.NOTIFY_PERSON1 != null)
					emailIds.Add((decimal)n.NOTIFY_PERSON1);
				if (n.NOTIFY_PERSON2 != null)
					emailIds.Add((decimal)n.NOTIFY_PERSON2);
			}

			if (emailIds.Count > 0)
			{
                string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
				if (string.IsNullOrEmpty(appUrl))
					appUrl = "the website";

				string emailSubject = "Incident Created: " + incidentType + " (" + incidentLocation + ")";
				string emailBody = "A new incident has been created:<br/>" +
								"<br/>" +
								incidentLocation + "<br/>" +
								incidentType + "<br/>" +
								"<br/>" +
								incident.DESCRIPTION + "<br/>" +
								"<br/>" +
								"Please log in to " + appUrl + " to view the incident.";

				foreach (decimal eid in emailIds)
				{
					string emailAddress = (from p in entities.PERSON where p.PERSON_ID == eid select p.EMAIL).FirstOrDefault();

					Thread thread = new Thread(() => WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "", "web", attachList));
					thread.IsBackground = true;
					thread.Start();

					//WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");
				}
			}
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