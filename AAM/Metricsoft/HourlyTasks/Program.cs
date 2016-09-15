using SQM.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.IO;

namespace HourlyTasks
{
	class Program
	{
		static PSsqmEntities entities;
		static StringBuilder output;
		static string startRange;
		static string endRange;
		static int timer = 2000;

		static void Main(string[] args)
		{
			output = new StringBuilder();
			entities = new PSsqmEntities();
			DateTime currentTime = DateTime.UtcNow;

			WriteLine("Started: " + currentTime.ToString("hh:mm MM/dd/yyyy"));

			//OverdueTaskNotifications(currentTime);
			//return;

			// arguments:
			// no arguments supplied == exec all tasks
			// audit == schedule audits only
			// notify == send notifications only
			if (args.Length == 0 || args.Contains("audit"))
			{
				// Close & Schedule Audits from the Scheduler, based on local plant date & time
				AuditScheduler();
			}

			if (args.Length == 0 || args.Contains("notify"))
			{
				OverdueTaskNotifications(currentTime);
			}

			// After all Hourly processes are complete, wrap up the output log
			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			WriteLogFile();
		}

		#region ScheduleAudits
		static void AuditScheduler()
		{
			// 1. Update the status of all audits that have a date past due 
			try
			{
				UpdatePastDueAuditStatus();
			}
			catch (Exception ex)
			{
				WriteLine("Main UpdatePastDueAssessmentStatus Error: " + ex.ToString());
			}

			// 2. Schedule new audits
			try
			{
				ScheduleAllAudits();
			}
			catch (Exception ex)
			{
				WriteLine("Main ScheduleAssessments Error: " + ex.ToString());
				//WriteLine("Main ScheduleAudits Detailed Error: " + ex.StackTrace.ToString());
			}
		}
		static void ScheduleAllAudits()
		{
			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805
			int startRangeHours = 04;
			int startRangeMinutes = 45;
			int endRangeHours = 05;
			int endRangeMinutes = 15;

			try
			{
				startRange = sets.Find(x => x.SETTING_CD == "AuditScheduleStart").VALUE.ToString();
				startRangeHours = Convert.ToInt16(startRange.Substring(0, 2));
				startRangeMinutes = Convert.ToInt16(startRange.Substring(3, 2));
			}
			catch { }
			try
			{
				endRange = sets.Find(x => x.SETTING_CD == "AuditScheduleEnd").VALUE.ToString();
				endRangeHours = Convert.ToInt16(endRange.Substring(0, 2));
				endRangeMinutes = Convert.ToInt16(endRange.Substring(3, 2));
			}
			catch { }

			List<AUDIT_SCHEDULER> scheduler = EHSAuditMgr.SelectActiveAuditSchedulers(0, null); // currently, we will select all schedules for all plants
			AUDIT audit = null;
			List<EHSAuditQuestion> questions = null;
			AUDIT_ANSWER answer = null;
			decimal auditId = 0;
			TimeSpan start = new TimeSpan(startRangeHours, startRangeMinutes, 0);
			TimeSpan end = new TimeSpan(endRangeHours, endRangeMinutes, 0);
			WriteLine("Audits will be created for locations with a local time of " + startRangeHours + ":" + startRangeMinutes + " through " + endRangeHours + ":" + endRangeMinutes);
			foreach (AUDIT_SCHEDULER schedule in scheduler)
			{
				AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, (decimal)schedule.AUDIT_TYPE_ID);
				// check that the audit is still active
				if (audittype != null)
				{
					if (!audittype.INACTIVE)
					{
						// ABW 1/5/16 - changing the scheduler from scheduling one week of audits to creating audits that are to be scheduled that day.  
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
							WriteLine("The following " + audittype.TITLE + " assessments were created for Assessment Scheduler " + schedule.AUDIT_SCHEDULER_ID + ": ");
							// for the location, select all people that should get the audit
							List<PERSON> auditors = SQMModelMgr.SelectPlantPrivgroupPersonList(auditPlant.PLANT_ID, new string[1] { schedule.JOBCODE_CD }, true);
							foreach (PERSON person in auditors)
							{
								// check to see if there is already an audit for this plant/type/date/person
								audit = EHSAuditMgr.SelectAuditForSchedule(auditPlant.PLANT_ID, audittype.AUDIT_TYPE_ID, person.PERSON_ID, localTime);
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
										DESCRIPTION = audittype.TITLE,
										// CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID, // do we want to set this to admin?
										AUDIT_DT = localTime,
										AUDIT_TYPE_ID = audittype.AUDIT_TYPE_ID,
										AUDIT_PERSON = person.PERSON_ID,
										CURRENT_STATUS = "A",
										PERCENT_COMPLETE = 0,
										TOTAL_SCORE = 0
									};

									entities.AddToAUDIT(audit);
									entities.SaveChanges();
									auditId = audit.AUDIT_ID;

									// create audit answer records
									questions = EHSAuditMgr.SelectAuditQuestionListByType(audittype.AUDIT_TYPE_ID);
									foreach (var q in questions)
									{
										answer = new AUDIT_ANSWER()
										{
											AUDIT_ID = auditId,
											AUDIT_QUESTION_ID = q.QuestionId,
											ANSWER_VALUE = "",
											ORIGINAL_QUESTION_TEXT = q.QuestionText,
											//COMMENT = q.AnswerComment
										};
										entities.AddToAUDIT_ANSWER(answer);
									}
									entities.SaveChanges();
									// create task record for their calendar
									EHSAuditMgr.CreateOrUpdateTask(auditId, person.PERSON_ID, 50, localTime.AddDays(audittype.DAYS_TO_COMPLETE), "A", 0);

									// send an email
									EHSNotificationMgr.NotifyOnAuditCreate(auditId, person.PERSON_ID);
									System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests

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
					try
					{
						status = audit.CURRENT_STATUS;
						if (status != "C")
							status = "E";
					}
					catch
					{
						status = "E";
					}
					EHSAuditMgr.CloseAudit(audit.AUDIT_ID, status, closeDT.AddDays(-1)); // now take the one day back off so that the close date sets correctly
																						 // now mark the Task as expired too!
					EHSAuditMgr.CreateOrUpdateTask(audit.AUDIT_ID, (decimal)audit.AUDIT_PERSON, 50, closeDT.AddDays(-1), status, 0);

				}
			}
		}
		#endregion

		#region notifications

		static string OverdueTaskNotifications(DateTime currentTime)
		{
			string nextStep = "";
			DateTime openFromDate = new DateTime(2000, 1, 1);
			DateTime localTime;
			int execAtHour = 10;	// set exec time default to 5 am EST

			WriteLine("OVERDUE TASK NOTIFICATIONS Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			SETTINGS setting = null;
			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", "");

			try
			{
				// execute at specified hour of day (e.g. TASKNOTIFY_TIME == 01:00)
				if ((setting = sets.Where(x => x.SETTING_CD == "TASKNOTIFY_TIME").FirstOrDefault()) != null)
				{
					// throwing an error here if the setting format was incorrect will be helpful for debugging
					execAtHour  =  int.Parse(WebSiteCommon.SplitString(setting.VALUE, ':').First());
				}

				entities = new PSsqmEntities();

				List<PLANT> plantList = SQMModelMgr.SelectPlantList(entities, 1, 0);
				PLANT plant = null;

				List<TaskItem> openTaskList = TaskMgr.SelectOpenTasks(currentTime, openFromDate);
				if (openTaskList.Count > 0)
				{
					WriteLine("Open Tasks ...");
					foreach (TaskItem taskItem in openTaskList)
					{
						try
						{
							if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident)
							{
								INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, taskItem.Task.RECORD_ID);
								if (incident != null &&  (plant=plantList.Where(l=> l.PLANT_ID == incident.DETECT_PLANT_ID).FirstOrDefault()) != null)
								{
									if (execAtHour == WebSiteCommon.LocalTime(currentTime, plant.LOCAL_TIMEZONE).Hour)
									{
										WriteLine("Task: " + taskItem.Task.TASK_ID.ToString() + " RecordType:  " + taskItem.Task.RECORD_TYPE.ToString() + "  " + "RecordID:" + taskItem.Task.RECORD_ID.ToString() + "  Status = " + taskItem.Task.STATUS);
										// notify assigned person and escalation person if over-over due
										EHSNotificationMgr.NotifyIncidentTaskStatus(incident, taskItem, ((int)SysPriv.action).ToString());
										System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests

										if (taskItem.Taskstatus >= SQM.Website.TaskStatus.Overdue)
										{
											// send to notification list for plant, BU, ...
											//EHSNotificationMgr.NotifyIncidentStatus(incident, taskItem.Task.TASK_STEP, ((int)SysPriv.notify).ToString(), "");
											//System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests
										}
									}
								}
							}
							else if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.PreventativeAction)
							{
								INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, taskItem.Task.RECORD_ID);
								if (incident != null  &&  (plant=plantList.Where(l=> l.PLANT_ID == incident.DETECT_PLANT_ID).FirstOrDefault()) != null)
								{
									if (execAtHour == WebSiteCommon.LocalTime(currentTime, plant.LOCAL_TIMEZONE).Hour)
									{
										WriteLine("Task: " + taskItem.Task.TASK_ID.ToString() + " RecordType:  " + taskItem.Task.RECORD_TYPE.ToString() + "  " + "RecordID:" + taskItem.Task.RECORD_ID.ToString() + "  Status = " + taskItem.Task.STATUS);
										// notify assigned person and escalation person if over-over due
										EHSNotificationMgr.NotifyPrevActionTaskStatus(incident, taskItem, ((int)SysPriv.action).ToString());
										System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests
									}
								}
							}
							else if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.Audit)  
							{
								AUDIT audit = EHSAuditMgr.SelectAuditById(entities, taskItem.Task.RECORD_ID);
								if ((plant = plantList.Where(l => l.PLANT_ID == audit.DETECT_PLANT_ID).FirstOrDefault()) != null)
								{
									if (execAtHour == WebSiteCommon.LocalTime(currentTime, plant.LOCAL_TIMEZONE).Hour)
									{
										WriteLine("Task: " + taskItem.Task.TASK_ID.ToString() + " RecordType:  " + taskItem.Task.RECORD_TYPE.ToString() + "  " + "RecordID:" + taskItem.Task.RECORD_ID.ToString() + "  Status = " + taskItem.Task.STATUS);
										if (taskItem.Task.TASK_STEP == "0")
										{
											EHSNotificationMgr.NotifyAuditStatus(audit, taskItem);
											System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests
										}
										else
										{
											EHSNotificationMgr.NotifyAuditTaskStatus(audit, taskItem, ((int)SysPriv.action).ToString());
											System.Threading.Thread.Sleep(timer); //will wait for 2 seconds to allow Google Mail to process email requests
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							WriteLine("Error: " + ex.ToString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				WriteLine("OVERDUE TASK NOTIFICATIONS Error - " + ex.ToString());
			}

			WriteLine("OVERDUE TASK NOTIFICATIONS Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			return nextStep;

		}

		#endregion

		#region logs
		static void WriteLogFile()
		{
			try
			{
				string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("HourlyTask_{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
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
				try
				{
					WriteLine("WriteLogFile Detailed Error: " + ex.StackTrace.ToString());
				}
				catch { }
			}
		}

		static void WriteLine(string text)
		{
			output.AppendLine(text);
		}
		#endregion

	}
}
