using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using SQM.Website;

namespace QAEmailSender
{
	enum TaskListRecordType
	{
		Incident = 20,
		ProblemCase = 21
	}

	class Program
	{
		static SQM.Website.PSsqmEntities entities;

		static TimeSpan initialReminderHours;
		static TimeSpan repeatReminderHours;
		static StringBuilder output;

		static void Main(string[] args)
		{
			initialReminderHours = new TimeSpan(48, 0, 0); // Number of hours to remind before due date
			repeatReminderHours = new TimeSpan(24, 0, 0); // Number of hours to wait before repeating reminder
			output = new StringBuilder();

			WriteLine("Started.");
			WriteLine("Reading pending emails...");

			entities = new SQM.Website.PSsqmEntities();
			List<TASK_STATUS> pendingTasks = (from t in entities.TASK_STATUS
										where t.COMPLETE_DT != null 
										&& t.RESPONSIBLE_ID != null
										select t).ToList();

			foreach (var task in pendingTasks)
			{
				try
				{

					var responsiblePerson = (from p in entities.PERSON.Include("PERSON_RESP") where p.PERSON_ID == task.RESPONSIBLE_ID select p).FirstOrDefault();

					WriteLine("----------");
					WriteLine(string.Format("Tasklist ID: {0} | Task Num: {1} | Due: {2:yyyy-MM-dd} | Last Notified: {3:yyyy-MM-dd}",
						0, task.TASK_ID, task.DUE_DT, task.NOTIFY_DT));
					if (responsiblePerson != null)
						if (responsiblePerson.PERSON_RESP != null)
							if (responsiblePerson.PERSON_RESP.DELEGATE_1 != null)
								WriteLine(string.Format("Responsible ID: {0} | Delegate ID: {1}",
									task.RESPONSIBLE_ID, responsiblePerson.PERSON_RESP.DELEGATE_1));

					if (!task.NOTIFY_DT.HasValue)
					{
						// First time email sent
						WriteLine("No last notification date");
						PrepareRecipient(task);
					}
					else if (task.DUE_DT.HasValue)
					{
						if (((DateTime)task.DUE_DT).Subtract(DateTime.Now) < initialReminderHours)
						{
							// First time reminder email sent (e.g. 48 hours before due date)
							WriteLine("Within 48 hours or past due date");
							if (DateTime.Now.Subtract(((DateTime)task.NOTIFY_DT)) >= repeatReminderHours)
							{
								PrepareRecipient(task);
							}
							else
							{
								WriteLine("- Already notified within past 24 hours - skipping");
							}
						}
					}
					//else if (DateTime.Now.Subtract(((DateTime)task.LAST_NOTIFICATION_DT)) < _repeatReminderHours)
					//{
					//	// Subsequent reminder sent (e.g. 24 hours after previous reminder)
					//	WriteLine("24 Hours after last notification date");
					//	PrepareRecipient(task);
					//}
				}
				catch (Exception ex)
				{
					WriteLine("pendingTasks Error: " + ex.ToString());
					WriteLine("pendingTasks Detailed Error: " + ex.InnerException.ToString());
				}
			}


			try
			{
				// New task notification method
				TaskNotification();
			}
			catch (Exception ex)
			{
				WriteLine("Main TaskNotification Error: " + ex.ToString());
				WriteLine("Main TaskNotification Detailed Error: " + ex.InnerException.ToString());
			}

			WriteLogFile();
		}

		static void PrepareRecipient(TASK_STATUS task)
		{
			try
			{
				// Mark as notified
				task.NOTIFY_DT = DateTime.Now;
				entities.SaveChanges();

				// Get recipient information
				var responsiblePerson = (from p in entities.PERSON.Include("PERSON_RESP") where p.PERSON_ID == task.RESPONSIBLE_ID select p).FirstOrDefault();
				// var delegatePerson = (from p in entities.PERSON where p.PERSON_ID == task.DELEGATE_ID select p).FirstOrDefault();
				PERSON delegatePerson = null;
				if (responsiblePerson != null)
					if (responsiblePerson.PERSON_RESP != null)
						if (responsiblePerson.PERSON_RESP.DELEGATE_1 != null)
							delegatePerson = (from p in entities.PERSON where p.PERSON_ID == (decimal)responsiblePerson.PERSON_RESP.DELEGATE_1 select p).FirstOrDefault();

				if (responsiblePerson != null)
				{
					PrepareEmail(task, responsiblePerson);
					WriteLine("  -> Sending notification to: " + responsiblePerson.EMAIL);

				}
				if (delegatePerson != null)
				{
					PrepareEmail(task, delegatePerson);
					WriteLine("  -> Sending notification to delegate: " + delegatePerson.EMAIL);
				}
			}
			catch (Exception ex)
			{
				WriteLine("PrepareRecipient Error: " + ex.ToString());
				WriteLine("PrepareRecipient Detailed Error: " + ex.InnerException.ToString());
			}
		}

		static void PrepareEmail(TASK_STATUS task, PERSON recipient)
		{
			try
			{
				// Get task information
				//var taskList = (from t in entities.TASKLIST where t.TASKLIST_ID == task.TASKLIST_ID select t).FirstOrDefault();
				TaskListRecordType recordType = (TaskListRecordType)task.RECORD_TYPE;

				string typeName = "";
				string description = "";

				if (recordType == TaskListRecordType.Incident)
				{
					typeName = "Incident";
					description += (from i in entities.INCIDENT where i.INCIDENT_ID == task.RECORD_ID select i.ISSUE_TYPE + ": " + i.DESCRIPTION).FirstOrDefault();
				
					var qiOccurrance = (from qi in entities.QI_OCCUR where qi.INCIDENT_ID == task.RECORD_ID select qi).FirstOrDefault();

					string dispDetails = "";
					if (qiOccurrance != null)
					{
						description += "<ul>";
						if (qiOccurrance.PROBCASE_REQD == true)
							description += "A problem case has been created for this incident.<br/>";

						if (!string.IsNullOrEmpty(qiOccurrance.DISPOSITION))
						{
							dispDetails = (from dd in entities.QI_OCCUR_DISPOSITION
										   where dd.DISPOSITION_ID == qiOccurrance.DISPOSITION
										   select dd.TITLE + " - " + dd.DESCRIPTION).FirstOrDefault();
						}
						if (!string.IsNullOrEmpty(dispDetails))
							description += "<li>Disposition: " + dispDetails + "</li>";
						description += "</ul>";
					}
				}
				else if (recordType == TaskListRecordType.ProblemCase)
				{
					typeName = "Problem Case";
					description += (from p in entities.PROB_CASE where p.PROBCASE_ID == task.RECORD_ID select p.DESC_SHORT + ": " + p.DESC_LONG).FirstOrDefault();
				
					decimal stepId;
					if (decimal.TryParse(task.TASK_STEP, out stepId))
					{
						var step = (from s in entities.PROB_CASE_STEP where s.STEP_ID == stepId select s).FirstOrDefault();
						if (step != null)
							description += "Step " + stepId + ": " + step.STEP_DESCRIPTION + "<br/><br/>";
					}
				}

				string url = "http://pssqm.luxinteractive.com/";
				string body = "";

				body += "The following " + typeName + " is still open:<br/><br/>";
				body += description;
				body += "<br/><br/>";
				body += "To update this " + typeName + ", please visit:<br/>";
				body += "<a href=\"" + url + "\">" + url + "</a>";
				body += "<br/><br/>Please Do Not Reply To This Message"; // AW20140129

				string toEmail = recipient.EMAIL;
				string subject = WebSiteCommon.GetXlatValue("emailSettings", "companyName") + " Reminder Notification";
				string bcc = "";

				SQM.Website.WebSiteCommon.SendEmail(toEmail, subject, body, bcc);
				//SendEmail(recipient, body);
			}
			catch (Exception ex)
			{
				WriteLine("PrepareEmail Error: " + ex.ToString());
				WriteLine("PrepareEmail Detailed Error: " + ex.InnerException.ToString());
			}
		}

		//static void SendEmail(PERSON responsiblePerson, string body)
		//{
		//	try
		//	{
		//		MailMessage msg = new MailMessage();
		//		msg.To.Add(responsiblePerson.EMAIL.Trim());
		//		msg.From = new MailAddress(MailFrom);
		//		msg.Subject = WebSiteCommon.GetXlatValue("emailSettings", "companyName") + " Reminder Notification";
		//		msg.Body = body;
		//		msg.Priority = MailPriority.Normal;
		//		msg.IsBodyHtml = true;

		//		SmtpClient client = new SmtpClient();
		//		client.Credentials = new System.Net.NetworkCredential(MailFrom, MailPassword);
		//		client.Port = MailSmtpPort; // Gmail works on this port
		//		client.Host = MailServer;
		//		client.EnableSsl = MailEnableSsl;

		//		client.Send(msg);
		//	}
		//	catch (Exception ex)
		//	{
		//		WriteLine("Error: " + ex.ToString());
		//	}
		//}

		static void TaskNotification()
		{
			try
			{
				DateTime fromDate = DateTime.Now.AddMonths(-6);
				List<TaskItem> taskList = new List<TaskItem>();
				List<decimal> respForList = new List<decimal>();

				List<UserContext> assignedUserList = TaskMgr.AssignedUserList();

				foreach (UserContext assignedUser in assignedUserList)
				{
					WriteLine(assignedUser.Person.SSO_ID);

					respForList.Clear();
					respForList.Add(assignedUser.Person.PERSON_ID);
					respForList.AddRange(assignedUser.DelegateList);

					taskList.Clear();
					taskList.AddRange(TaskMgr.ProfileApprovalStatus(new DateTime(fromDate.Year, fromDate.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), respForList, assignedUser.EscalationAssignments));
                    taskList.AddRange(TaskMgr.IncidentTaskStatus(1, respForList, assignedUser.EscalationAssignments, true));
					WriteLine(taskList.Count.ToString());

					TaskMgr.MailTaskList(taskList, assignedUser.Person.EMAIL, "exe");
				}

			}
			catch (Exception ex)
			{
				WriteLine("TaskNotification Error: " + ex.ToString());
				WriteLine("TaskNotification Detailed Error: " + ex.InnerException.ToString());
			}
		}

		static void WriteLogFile()
		{
			try
			{
				string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.Now);
				File.WriteAllText(fullPath, output.ToString());
				Console.WriteLine("Wrote log file: " + fullPath);

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
			Console.WriteLine(text);
			output.AppendLine(text);
		}

	}
}
