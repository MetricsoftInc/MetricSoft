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
			SETTINGS setting = null;
			bool validIP = true;
			int workdays = 7;
			string pageURI = HttpContext.Current.Request.Url.AbsoluteUri;
			string nextPage = "";
			fromDate = DateTime.UtcNow.AddMonths(-12);    // set the incident 'select from' date.  TODO: get this from SETTINGS table

			WriteLine("Incident Rollup Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			WriteLine(pageURI);

			try
			{
				string currentIP = GetIPAddress();
				List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

				string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();
				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_WORKDAYS").FirstOrDefault();
				if (setting != null  && !string.IsNullOrEmpty(setting.VALUE))
				{
					if (!int.TryParse(setting.VALUE, out workdays))
					workdays = 7;
				}
				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_NEXTPAGE").FirstOrDefault();
				if (setting != null  &&  !string.IsNullOrEmpty(setting.VALUE)  &&  setting.VALUE.Length > 1)
				{
					nextPage = setting.VALUE;
				}

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
						  EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) <= DateTime.UtcNow
						  select a).OrderBy(l=> l.PLANT_ID).ThenBy(l=> l.PERIOD_YEAR).ThenBy(l=> l.PERIOD_MONTH).ToList();
				
				List<EHSIncidentTimeAccounting> summaryList = new List<EHSIncidentTimeAccounting>();

				foreach (INCIDENT incident in incidentList)
				{
					WriteLine("Incident ID: " + incident.INCIDENT_ID.ToString() + "  Occur Date: " + Convert.ToDateTime(incident.INCIDENT_DT).ToShortDateString());
					incident.INCFORM_CAUSATION.Load();
					if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
						incident.INCFORM_LOSTTIME_HIST.Load();
					plant = plantList.Where(l => l.PLANT_ID == (decimal)incident.DETECT_PLANT_ID).FirstOrDefault();
					summaryList = EHSIncidentMgr.SummarizeIncidentAccounting(summaryList, EHSIncidentMgr.CalculateIncidentAccounting(incident, plant.LOCAL_TIMEZONE, workdays));
				}

				plant = null;
				PLANT_ACTIVE pact = null;
				DateTime periodDate;

				foreach (PLANT_ACCOUNTING pah in paList.OrderBy(l=> l.PLANT_ID).ToList())
				{
					if (pact == null || pact.PLANT_ID != pah.PLANT_ID)
					{
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == pah.PLANT_ID && a.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident select a).SingleOrDefault();
					}
					if (pact != null && new DateTime(pah.PERIOD_YEAR, pah.PERIOD_MONTH, 1).Date >= ((DateTime)pact.EFF_START_DATE).Date)
					{
						pah.TIME_LOST = pah.TOTAL_DAYS_RESTRICTED = 0;
						pah.TIME_LOST_CASES = pah.RECORDED_CASES = pah.FIRST_AID_CASES = 0;
					}
				}

				plant = null;
				pact = null;
				foreach (EHSIncidentTimeAccounting period in summaryList.OrderBy(l => l.PlantID).ThenBy(l => l.PeriodYear).ThenBy(l => l.PeriodMonth).ToList())
				{
					if (plant == null  ||  plant.PLANT_ID != period.PlantID)
					{
						plant = plantList.Where(l => l.PLANT_ID == period.PlantID).FirstOrDefault();
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == plant.PLANT_ID &&  a.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident select a).SingleOrDefault();
					}
					periodDate = new DateTime(period.PeriodYear, period.PeriodMonth, 1);
					if (pact != null && periodDate >= pact.EFF_START_DATE)
					{
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
						pa.LAST_UPD_DT = DateTime.UtcNow;
						pa.LAST_UPD_BY = "automated";
						EHSModel.UpdatePlantAccounting(entities, pa);
					}
				}
			}
			catch (Exception ex)
			{
				WriteLine("Main Incident RollUp Error - " + ex.ToString());
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();

			try
			{
				if (!string.IsNullOrEmpty(nextPage))
				{
					int s1 = pageURI.LastIndexOf('/');
					int s2 = pageURI.LastIndexOf('.') > -1 ? pageURI.LastIndexOf('.') : pageURI.Length;
					string nextPageURI = pageURI.Substring(0, s1 + 1) + nextPage + pageURI.Substring(s2, pageURI.Length - s2);
					Response.Redirect(nextPageURI);
				}
			}
			catch (Exception ex)
			{
				output = new StringBuilder();
				WriteLine("RollUp Redirect Error - " + ex.ToString());
				WriteLogFile();
			}
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