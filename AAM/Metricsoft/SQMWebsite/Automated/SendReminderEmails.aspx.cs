using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using SQM.Website;
using SQM.Website.Shared;

namespace SQM.Website.Automated
{
	public partial class SendReminderEmails : System.Web.UI.Page
	{
		static StringBuilder output;
		static DateTime fromDate;

		protected void Page_Load(object sender, EventArgs e)
		{
			string pageMode = "";
			if (!string.IsNullOrEmpty(Request.QueryString["m"]))   // .../...aspx?p=xxxxx
			{
				pageMode = Request.QueryString["m"].ToLower();  // page mode (web == running manually from the menu)
			}

			if (IsPostBack)
			{
				if (pageMode != "web")
				{
					System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
				}
				return;
			}

			output = new StringBuilder();
			bool validIP = true;
			DateTime thisPeriod = DateTime.UtcNow;
			decimal updateIndicator = thisPeriod.Ticks;
			decimal locationID = 0;

			WriteLine("Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			try
			{
				string currentIP = GetIPAddress();
				List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

				string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();
				/*
				if (strValidIP.Equals(currentIP))
				{
					WriteLine("Main Incident RollUp being accessed from a valid IP address " + currentIP);
					validIP = true;

					if (Request.QueryString["validation"] != null)
					{
						if (Request.QueryString["validation"].ToString().Equals("Vb12M11a4"))
							validIP = true;
					}
					else
					{
						WriteLine("Main Incident RollUp requested from incorrect source.");
						validIP = false;
					}
				}
				else
				{
					WriteLine("Main Incident RollUp being accessed from invalid IP address " + currentIP);
					validIP = false;
				}
				*/
			}
			catch (Exception ex)
			{
				validIP = false;
				WriteLine("Main Email Notification Error validating IP Address: " + ex.ToString());
			}

			// make sure this code is NOT moved to production
			validIP = true;

			if (!validIP)
			{
				WriteLine("Main Incident RollUp Invalid IP Address");
				ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
				WriteLogFile();

				System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
				return;
			}

			try
			{
				PSsqmEntities entities = new PSsqmEntities();

				List<TaskItem> openAuditList = TaskMgr.SelectOpenAudits(DateTime.UtcNow, DateTime.UtcNow);
				if (openAuditList.Count > 0)
				{
					WriteLine("Open Audits ...");
					foreach (TaskItem taskItem in openAuditList)
					{
						WriteLine("Audit: " + taskItem.Task.RECORD_ID.ToString() + "  Status = " + taskItem.Task.STATUS);
						AUDIT audit = EHSAuditMgr.SelectAuditById(entities, taskItem.Task.RECORD_ID);
						if (audit != null)
						{
							EHSNotificationMgr.NotifyAuditStatus(audit, taskItem);
						}
					}
				}

				List<TaskItem> openTaskList = TaskMgr.SelectOpenTasks(DateTime.UtcNow, DateTime.UtcNow);
				if (openTaskList.Count > 0)
				{
				   WriteLine("Open Tasks ...");
					foreach (TaskItem taskItem in openTaskList)
					{
						WriteLine("Task: " + taskItem.Task.TASK_ID.ToString() + " RecordType:  " + taskItem.Task.RECORD_TYPE.ToString() + "  " + "RecordID:" + taskItem.Task.RECORD_ID.ToString() + "  Status = " + taskItem.Task.STATUS);
						if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident)
						{
							INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, taskItem.Task.RECORD_ID);
							if (incident != null)
							{
								// notify assigned person and escalation person if over-over due
								EHSNotificationMgr.NotifyIncidentTaskStatus(incident, taskItem, ((int)SysPriv.action).ToString());
								if (taskItem.Taskstatus >= TaskStatus.Overdue)
								{
									// send to notification list for plant, BU, ...
									EHSNotificationMgr.NotifyIncidentStatus(incident, taskItem.Task.TASK_STEP, ((int)SysPriv.notify).ToString(), "");
								}
							}
						}
						else if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.PreventativeAction)
						{
							INCIDENT incident = EHSIncidentMgr.SelectIncidentById(entities, taskItem.Task.RECORD_ID);
							if (incident != null)
							{
								// notify assigned person and escalation person if over-over due
								EHSNotificationMgr.NotifyPrevActionTaskStatus(incident, taskItem, ((int)SysPriv.action).ToString());
							}
						}
						else if (taskItem.Task.RECORD_TYPE == (int)TaskRecordType.Audit)
						{
							AUDIT audit = EHSAuditMgr.SelectAuditById(entities, taskItem.Task.RECORD_ID);
							if (audit != null)
							{
								EHSNotificationMgr.NotifyAuditTaskStatus(audit, taskItem, ((int)SysPriv.action).ToString());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WriteLine("Main Email Notificaion Error - " + ex.ToString());
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();

			System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
		}

		public string GetIPAddress()
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST

			string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString(); // Get the IP

			return myIP;
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