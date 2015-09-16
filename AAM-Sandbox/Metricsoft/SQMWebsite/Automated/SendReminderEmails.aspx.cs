using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace SQM.Website
{
	enum TaskStatusRecordType
	{
		ProblemCase = 21,
		Incident = 40
	}


	public partial class SendReminderEmails : System.Web.UI.Page
	{
		static PSsqmEntities entities;

		static TimeSpan initialReminderHours;
		static TimeSpan repeatReminderHours;
		static StringBuilder output;

		protected void Page_Load(object sender, EventArgs e)
		{
			output = new StringBuilder();

			WriteLine("Started.");

			if (ShouldSendNotification() == true)
			{
				try
				{
					TaskNotification();
				}
				catch (Exception ex)
				{
					WriteLine("Main TaskNotification Error: " + ex.ToString());
					WriteLine("Main TaskNotification Detailed Error: " + ex.InnerException.ToString());
				}
			}

			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();
		}


		protected bool ShouldSendNotification()
		{
			bool shouldSend = false;

			var entities = new PSsqmEntities();
			TimeSpan timeBetweenNotifications = new TimeSpan(24, 0, 0);
			DateTime lastSentDate;

			SETTINGS stHours = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "HoursBetweenNotifications");

			if (stHours != null)
			{
				if (!string.IsNullOrEmpty(stHours.VALUE))
				{
					int hours;
					if (Int32.TryParse(stHours.VALUE, out hours))
						timeBetweenNotifications = new TimeSpan(hours, 0, 0);
				}

				SETTINGS stLastSent = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "NotificationLastSent");

				if (stLastSent != null)
				{
					if (DateTime.TryParse(stLastSent.VALUE, out lastSentDate))
					{
						WriteLine("Last sent date: " + lastSentDate);
						shouldSend = (DateTime.Now.Subtract(timeBetweenNotifications) > lastSentDate);

						if (shouldSend == true)
						{
							// Set last sent date to now
							stLastSent.VALUE = DateTime.Now.ToString();
							SQMSettings.UpdateSettings(entities, stLastSent, "Automated Emailer");
						}
					}
					else
					{
						shouldSend = false;
					}
				}
				else
				{
					WriteLine("Missing value in SETTINGS table: NotificationLastSent");
					shouldSend = false;
				}
			}
			else
			{
				WriteLine("Missing value in SETTINGS table: HoursBetweenNotifications");
				shouldSend = false;
			}

			WriteLine("Should send: " + shouldSend);

			return shouldSend;
		}


		static void TaskNotification()
		{
			try
			{
				DateTime fromDate = DateTime.Now.AddMonths(-6);
				List<TaskItem> taskList = new List<TaskItem>();
				List<decimal> respForList = new List<decimal>();
				List<decimal> respPlantList = new List<decimal>();

				List<UserContext> assignedUserList = TaskMgr.AssignedUserList();

				WriteLine("");
				WriteLine("Executing TaskMgr.MailTaskList:");

				foreach (UserContext assignedUser in assignedUserList)
				{
					WriteLine(assignedUser.Person.SSO_ID);

					respForList.Clear();
					respForList.Add(assignedUser.Person.PERSON_ID);
					//respForList.AddRange(assignedUser.DelegateList);

					taskList.Clear();
					taskList.AddRange(TaskMgr.ProfileInputStatus(new DateTime(fromDate.Year, fromDate.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), respForList, respPlantList));
					taskList.AddRange(TaskMgr.IncidentTaskStatus(1, respForList, respPlantList, false));
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