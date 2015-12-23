using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using SQM.Website;
using SQM.Website.Shared;

namespace SQM.Website.Automated
{
	public partial class IncidentRollUp : System.Web.UI.Page
	{
		static StringBuilder output;
		static DateTime fromDate;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			output = new StringBuilder();
			bool validIP = true;
			fromDate = DateTime.UtcNow.AddMonths(-12);    // set the incident 'select from' date.  TODO: get this from SETTINGS table

			WriteLine("Started: " + DateTime.Now.ToString("hh:mm MM/dd/yyyy"));

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
				WriteLine("Main Incident RollUp Error validating IP Address: " + ex.ToString());
			}

			// make sure this code is NOT moved to production
			//validIP = true;

			if (!validIP)
			{
				WriteLine("Main Incident RollUp Invalid IP Address");
				ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
				WriteLogFile();
				return;
			}

			try
			{
				PSsqmEntities entities = new PSsqmEntities();

				// fetch all incidents occurring after the minimum reporting date
				List<INCIDENT> incidentList = (from i in entities.INCIDENT.Include("INCFORM_INJURYILLNESS") 
								where 
								i.INCIDENT_DT >= fromDate  &&  i.DETECT_PLANT_ID > 0 
								select i).OrderBy(l=> l.DETECT_PLANT_ID).ThenBy(l=> l.INCIDENT_DT).ToList();

				List<PLANT> plantList = SQMModelMgr.SelectPlantList(entities, 1, 0);
				PLANT plant = null;

				// fetch all the plant accounting records for the target timespan
				PLANT_ACCOUNTING pa = null;
				List<PLANT_ACCOUNTING> paList = (from a in entities.PLANT_ACCOUNTING 
						  where
						  EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) <= DateTime.Now
						  select a).OrderBy(l=> l.PLANT_ID).ThenBy(l=> l.PERIOD_YEAR).ThenBy(l=> l.PERIOD_MONTH).ToList();
				
				List<EHSIncidentTimeAccounting> summaryList = new List<EHSIncidentTimeAccounting>();

				foreach (INCIDENT incident in incidentList)
				{
					WriteLine("Incident ID: " + incident.INCIDENT_ID.ToString() + "  Occur Date: " + Convert.ToDateTime(incident.INCIDENT_DT).ToShortDateString());
					incident.INCFORM_CAUSATION.Load();
					if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
						incident.INCFORM_LOSTTIME_HIST.Load();
					plant = plantList.Where(l => l.PLANT_ID == (decimal)incident.DETECT_PLANT_ID).FirstOrDefault();
					summaryList = EHSIncidentMgr.SummarizeIncidentAccounting(summaryList, EHSIncidentMgr.CalculateIncidentAccounting(entities, incident, plant.LOCAL_TIMEZONE));
				}

				plant = null;
				foreach (EHSIncidentTimeAccounting period in summaryList.OrderBy(l => l.PlantID).ThenBy(l => l.PeriodYear).ThenBy(l => l.PeriodMonth).ToList())
				{
					// clear the incident acccounting values for the entire timespan upon processing a new plant ID. 
					// we do this in case previously accounted INCIDENTs were deleted or moved to another period
					if (plant == null  ||  plant.PLANT_ID != period.PlantID)
					{
						plant = plantList.Where(l => l.PLANT_ID == period.PlantID).FirstOrDefault();
						foreach (PLANT_ACCOUNTING pac in paList.Where(p => p.PLANT_ID == period.PlantID))
						{
							pac.TIME_LOST = pac.TOTAL_DAYS_RESTRICTED = pac.TIME_LOST_CASES = pac.RECORDED_CASES = pac.FIRST_AID_CASES = 0;
						}
					}
					// write PLANT_ACCOUNTING metrics
					if ((pa = paList.Where(l => l.PLANT_ID == period.PlantID && l.PERIOD_YEAR == period.PeriodYear && l.PERIOD_MONTH == period.PeriodMonth).FirstOrDefault()) == null)
					{
						paList.Add((pa = new PLANT_ACCOUNTING()));
						pa.PLANT_ID = period.PlantID;
						pa.PERIOD_YEAR = period.PeriodYear;
						pa.PERIOD_MONTH = period.PeriodMonth;
					}
					pa.TIME_LOST = period.LostTime;
					pa.TOTAL_DAYS_RESTRICTED = period.RestrictedTime;
					pa.TIME_LOST_CASES = period.LostTimeCase;
					pa.RECORDED_CASES = period.RecordableCase;
					pa.FIRST_AID_CASES = period.FirstAidCase;
					EHSModel.UpdatePlantAccounting(entities, pa);
				}
			}
			catch (Exception ex)
			{
				WriteLine("Main Incident RollUp Error - " + ex.ToString());
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.Now.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();
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