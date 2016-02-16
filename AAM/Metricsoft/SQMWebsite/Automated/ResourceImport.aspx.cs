using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace SQM.Website.Automated
{
	public partial class ResourceImport : System.Web.UI.Page
	{
		static PSsqmEntities entities;
		static StringBuilder output;

		protected void Page_Load(object sender, EventArgs e)
		{
			output = new StringBuilder();
			entities = new PSsqmEntities();
			bool validIP = false;
			WriteLine("Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			try
			{
				string currentIP = GetIPAddress();
				List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

				string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();

				if (strValidIP.Equals(currentIP))
				{
					WriteLine("Main ScheduleAssessments being accessed from a valid IP address " + currentIP);
					validIP = true;

					if (Request.QueryString["validation"] != null)
					{
						if (Request.QueryString["validation"].ToString().Equals("Vb12M11a4"))
							validIP = true;
					}
					else
					{
						WriteLine("Main ScheduleAssessments requested from incorrect source.");
						validIP = false;
					}
				}
				else
				{
					WriteLine("Main ScheduleAssessments being accessed from invalid IP address " + currentIP);
					validIP = false;
				}
			}
			catch (Exception ex)
			{
				validIP = false;
				WriteLine("Main ScheduleAssessments Error validating IP Address: " + ex.ToString());
			}

			// make sure this code is NOT moved to production
			//validIP = true;

			if (validIP)
			{
				try
				{
					// Update the status of all audits that have a date past due
					UpdatePastDueAuditStatus();
				}
				catch (Exception ex)
				{
					WriteLine("Main UpdatePastDueAssessmentStatus Error: " + ex.ToString());
				}

				try
				{
					// Schedule new audits
					ScheduleAllAudits();
				}
				catch (Exception ex)
				{
					WriteLine("Main ScheduleAssessments Error: " + ex.ToString());
					//WriteLine("Main ScheduleAudits Detailed Error: " + ex.InnerException.ToString());
				}
			}
			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();
		}

		public string GetIPAddress()
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			
			string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString(); // Get the IP

			return myIP;
		}

		static void ScheduleAllAudits()
		{
			List<AUDIT_SCHEDULER> scheduler = EHSAuditMgr.SelectActiveAuditSchedulers(0, null); // currently, we will select all schedules for all plants
			AUDIT audit = null;
			List<EHSAuditQuestion> questions = null;
			AUDIT_ANSWER answer = null;
			decimal auditId = 0;
			TimeSpan start = new TimeSpan(04, 45, 0); 
			TimeSpan end = new TimeSpan(05, 15, 0); 

			foreach (AUDIT_SCHEDULER schedule in scheduler)
			{
				AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, (decimal)schedule.AUDIT_TYPE_ID);
				// check that the audit is still active
				if (type != null)
				{
					if (!type.INACTIVE)
					{
						// ABW 1/5/16 - changing the scheduler from scheduling on week of audits to creating audits that are to be scheduled that day.  
						//     All audits will be scheduled at 5am local plant time for the day.
						//WriteLine("");
						//WriteLine("The following " + type.TITLE + " assessments were created for Assessment Scheduler " + schedule.AUDIT_SCHEDULER_ID + ": ");
						//// determine the date to schedule, by finding the next occurance of the selected day of the week after the current day
						//DateTime auditDate = DateTime.Today;
						//while ((int)auditDate.DayOfWeek != schedule.DAY_OF_WEEK)
						//{
						//	auditDate = auditDate.AddDays(1);
						//}

						// get the plant
						PLANT auditPlant = SQMModelMgr.LookupPlant((decimal)schedule.PLANT_ID);
						// check the local plant time to see if it is almost 5am.  If so, schedule the audit. If not, do nothing.
						DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, auditPlant.LOCAL_TIMEZONE);
						if ((int)localTime.DayOfWeek == schedule.DAY_OF_WEEK && ((localTime.TimeOfDay > start) && (localTime.TimeOfDay < end)))
						{
							WriteLine("");
							WriteLine("The following " + type.TITLE + " assessments were created for Assessment Scheduler " + schedule.AUDIT_SCHEDULER_ID + ": ");
							// for the location, select all people that should get the audit
							List<PERSON> auditors = SQMModelMgr.SelectPlantPrivgroupPersonList(auditPlant.PLANT_ID, new string[1] { schedule.JOBCODE_CD }, true);
							foreach (PERSON person in auditors)
							{
								// check to see if there is already an audit for this plant/type/date/person
								audit = EHSAuditMgr.SelectAuditForSchedule(auditPlant.PLANT_ID, type.AUDIT_TYPE_ID, person.PERSON_ID, localTime);
								if (audit == null)
								{
									// create audit header
									auditId = 0;
									audit = new AUDIT()
									{
										DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
										DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
										DETECT_PLANT_ID = auditPlant.PLANT_ID,
										AUDIT_TYPE = "EHS",
										CREATE_DT = localTime,
										CREATE_BY = "Automated Scheduler",
										DESCRIPTION = type.TITLE,
										// CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID, // do we want to set this to admin?
										AUDIT_DT = localTime,
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
									questions = EHSAuditMgr.SelectAuditQuestionListByType(type.AUDIT_TYPE_ID);
									foreach (var q in questions)
									{
										answer = new AUDIT_ANSWER()
										{
											AUDIT_ID = auditId,
											AUDIT_QUESTION_ID = q.QuestionId,
											//ANSWER_VALUE = q.AnswerText,
											ORIGINAL_QUESTION_TEXT = q.QuestionText,
											//COMMENT = q.AnswerComment
										};
										entities.AddToAUDIT_ANSWER(answer);
									}
									entities.SaveChanges();
									// create task record for their calendar
									EHSAuditMgr.CreateOrUpdateTask(auditId, person.PERSON_ID, 50, localTime.AddDays(type.DAYS_TO_COMPLETE), "A", 0);

									// send an email
									EHSNotificationMgr.NotifyOnAuditCreate(auditId, person.PERSON_ID);

									WriteLine(person.LAST_NAME + ", " + person.FIRST_NAME + " - assessment added");
								}
								else
								{
									// ABW 1/5/16 - Since this will be running once every hour now, we don't want to see this message
									//WriteLine(person.LAST_NAME + ", " + person.FIRST_NAME + " - assessment already exists for this date");
								}
							}
						}
						else
						{
							// ABW 1/5/16 - Do we need to write any message out to explaing why the audit wasn't created?  I don't think so
							//WriteLine("Assessment Type " + schedule.AUDIT_TYPE_ID + " - assessment already exists for this date OR is not scheduled to be created");
						}
					}
					else
					{
						WriteLine("Assessment Type " + schedule.AUDIT_TYPE_ID + " inactive. Assessments not created for Scheduler Record " + schedule.AUDIT_SCHEDULER_ID.ToString());
					}
				}
				else
				{
					WriteLine("Assessment Type " + schedule.AUDIT_TYPE_ID + " not found. Assessments not created for Scheduler Record " + schedule.AUDIT_SCHEDULER_ID.ToString());
				}
			}
			
		}

		static void UpdatePastDueAuditStatus()
		{
			string status = "";
			// get a list of all audits that do not have a close date
			List<AUDIT> openAudits = EHSAuditMgr.SelectOpenAudits(0, null);
			foreach (AUDIT audit in openAudits)
			{
				AUDIT_TYPE type = EHSAuditMgr.SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
				PLANT plant = SQMModelMgr.LookupPlant((decimal)audit.DETECT_PLANT_ID);
				DateTime closeDT = Convert.ToDateTime(audit.AUDIT_DT.AddDays(type.DAYS_TO_COMPLETE + 1));  // add one to the date and it will default to the next day at 00:00:00, which means midnight
				DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, plant.LOCAL_TIMEZONE);
				if (closeDT.CompareTo(localTime) < 0)
				{
					// close the audit
					// valid status codes... A = active, C = complete, I = incomplete/in-process, E = Expired. We are closing audits that are past due, so Expired.
					status = audit.CURRENT_STATUS;
					if (status != "C")
						status = "E";
					EHSAuditMgr.CloseAudit(audit.AUDIT_ID, status, closeDT.AddDays(-1)); // now take the one day back off so that the close date sets correctly
					// now mark the Task as expired too!
					EHSAuditMgr.CreateOrUpdateTask(audit.AUDIT_ID, (decimal)audit.AUDIT_PERSON, 50, closeDT.AddDays(-1), status, 0);

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
				string fullPath = logPath + string.Format("{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
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