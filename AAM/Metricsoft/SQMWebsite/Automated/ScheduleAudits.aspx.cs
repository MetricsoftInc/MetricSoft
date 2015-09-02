using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace SQM.Website.Automated
{
	public partial class ScheduleAudits : System.Web.UI.Page
	{
		static PSsqmEntities entities;
		static StringBuilder output;

		protected void Page_Load(object sender, EventArgs e)
		{
			output = new StringBuilder();
			entities = new PSsqmEntities();

			WriteLine("Started: " + DateTime.Now.ToString("hh:mm MM/dd/yyyy"));

			try
			{
				ScheduleAllAudits();
			}
			catch (Exception ex)
			{
				WriteLine("Main ScheduleAudits Error: " + ex.ToString());
				WriteLine("Main ScheduleAudits Detailed Error: " + ex.InnerException.ToString());
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.Now.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();
		}

		static void ScheduleAllAudits()
		{
			List<AUDIT_SCHEDULER> scheduler = EHSAuditMgr.SelectActiveAuditSchedulers(0, null); // currently, we will select all schedules for all plants
			AUDIT audit = null;
			List<EHSAuditQuestion> questions = null;
			AUDIT_ANSWER answer = null;
			decimal auditId = 0;

			foreach (AUDIT_SCHEDULER schedule in scheduler)
			{
				AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, (decimal)schedule.AUDIT_TYPE_ID);
				// check that the audit is still active
				if (type != null)
				{
					if (!type.INACTIVE)
					{
						WriteLine("");
						WriteLine("The following " + type.TITLE + " audits were created for Audit Scheduler " + schedule.AUDIT_SCHEDULER_ID + ": ");
						// determine the date to schedule, by finding the next occurance of the selected day of the week after the current day
						DateTime auditDate = DateTime.Now;
						while ((int)auditDate.DayOfWeek != schedule.DAY_OF_WEEK)
						{
							auditDate = auditDate.AddDays(1);
						}
						// get the plant
						PLANT auditPlant = SQMModelMgr.LookupPlant((decimal)schedule.PLANT_ID);
						// for the location, select all people that should get the audit
						List<PERSON> auditors = SQMModelMgr.SelectPlantPrivgroupPersonList(auditPlant.PLANT_ID, new string[1] { schedule.JOBCODE_CD });
						foreach (PERSON person in auditors)
						{
							// create audit header
							auditId = 0;
							audit = new AUDIT()
							{
								DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
								DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
								DETECT_PLANT_ID = auditPlant.PLANT_ID,
								AUDIT_TYPE = "EHS",
								CREATE_DT = DateTime.Now,
								CREATE_BY = "Automated Scheduler",
								DESCRIPTION = type.TITLE,
								// CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID, // do we want to set this to admin?
								AUDIT_DT = auditDate,
								AUDIT_TYPE_ID = type.AUDIT_TYPE_ID,
								AUDIT_PERSON = person.PERSON_ID,
								CURRENT_STATUS = "A",
								PERCENT_COMPLETE = 0,
								TOTAL_SCORE = 0
							};

							entities.AddToAUDIT(audit);
							entities.SaveChanges();
							auditId = audit.AUDIT_ID;

							// create audit answer records
							questions = EHSAuditMgr.SelectAuditQuestionList(type.AUDIT_TYPE_ID, 0, 0); // do not specify the audit ID
							foreach (var q in questions)
							{
								answer = new AUDIT_ANSWER()
								{
									AUDIT_ID = auditId,
									AUDIT_QUESTION_ID = q.QuestionId,
									ANSWER_VALUE = q.AnswerText,
									ORIGINAL_QUESTION_TEXT = q.QuestionText,
									COMMENT = q.AnswerComment
								};
								entities.AddToAUDIT_ANSWER(answer);
							}
							entities.SaveChanges();
							// create task record for their calendar
							EHSAuditMgr.CreateOrUpdateTask(auditId, person.PERSON_ID, 50, auditDate.AddDays(type.DAYS_TO_COMPLETE));

							// send an email
							EHSNotificationMgr.NotifyOnAuditCreate(auditId, person.PERSON_ID);

							WriteLine(person.LAST_NAME + ", " + person.FIRST_NAME);
						}
					}
					else
					{
						WriteLine("Adit Type " + schedule.AUDIT_TYPE_ID + " inactive. Audits not created for Scheduler Record " + schedule.AUDIT_SCHEDULER_ID.ToString());
					}
				}
				else
				{
					WriteLine("Adit Type " + schedule.AUDIT_TYPE_ID + " not found. Audits not created for Scheduler Record " + schedule.AUDIT_SCHEDULER_ID.ToString());
				}
			}
			
		}

		static void WriteLogFile()
		{
			try
			{
				string logPath = HttpContext.Current.Server.MapPath("~") + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.Now);
				File.WriteAllText(fullPath, output.ToString());

				// Keep only last 100 log files
				int maxFiles = 100;
				var info = new DirectoryInfo(logPath);
				FileInfo[] files = info.GetFiles("*.txt").OrderBy(f => f.CreationTime).ToArray();
				if (files.Count() > maxFiles)
					for (int i = 0; i < files.Count() - maxFiles; i++)
						File.Delete(logPath + files[i].Name);
			}
			catch (Exception ex)
			{
				WriteLine("WriteLogFile Error: " + ex.ToString());
				WriteLine("WriteLogFile Detailed Error: " + ex.InnerException.ToString());
			}
		}

		static void WriteLine(string text)
		{
			output.AppendLine(text);
		}

	}
}