using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Configuration;


namespace SQM.Website
{
	public static class EHSNotificationMgr
	{

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