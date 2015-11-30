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
			DateTime priorPeriod = DateTime.UtcNow.AddMonths(-1);
			DateTime thisPeriod = DateTime.UtcNow;
			decimal updateIndicator = thisPeriod.Ticks;
			decimal locationID = 0;

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

				// get all EHS_MEASURE's that map to EHS DATA (ne. PLANT_ACCOUNTING)
				List<EHS_MEASURE> measureList = new List<EHS_MEASURE>();
				measureList = (from m in entities.EHS_MEASURE
							   where
							   m.MEASURE_CATEGORY == "SAFE"  && 
							   !string.IsNullOrEmpty(m.PLANT_ACCT_FIELD) &&
							   m.FREQUENCY == "M" 
							   select m).ToList();

				// fetch all incidents occurring after the minimum reporting date
				List<INCIDENT> incidentList = new List<INCIDENT>();
				incidentList = (from i in entities.INCIDENT.Include("INCFORM_INJURYILLNESS") 
								where 
								i.INCIDENT_DT >= fromDate  
								select i).ToList();

				// pre allocate monthly periods for the overall accounting span
				EHSIncidentTimeAccounting accountingPeriod = null;
				DateTime effDate = fromDate;
				DateTime effToDate = DateTime.UtcNow.AddMonths(1);
				List<EHSIncidentTimeAccounting> accountingSpan = new List<EHSIncidentTimeAccounting>();
				while (effDate <= effToDate)
				{
					accountingSpan.Add(new EHSIncidentTimeAccounting().CreateNew(effDate.Year, effDate.Month, 0));
					effDate = effDate.AddMonths(1);
				}

				foreach (INCIDENT incident in incidentList)
				{
					WriteLine("Incident ID: " + incident.INCIDENT_ID.ToString() + "  Occur Date: " + Convert.ToDateTime(incident.INCIDENT_DT).ToShortDateString());
					locationID = (decimal)incident.DETECT_PLANT_ID;
					if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
					{
						incident.INCFORM_LOSTTIME_HIST.Load();
					}
					List<EHSIncidentTimeAccounting> incidentSpan = EHSIncidentMgr.CalculateIncidentAccounting(entities, incident, priorPeriod, thisPeriod);
					if (incidentSpan != null && incidentSpan.Count > 0)
					{
						List<EHS_DATA> dataList = new List<EHS_DATA>();
						foreach (EHSIncidentTimeAccounting period in incidentSpan)
						{
							dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, locationID, new DateTime(period.PeriodYear, period.PeriodMonth, 1), measureList.Select(m => m.MEASURE_ID).ToList(), true, updateIndicator);
							EHSDataMapping.SetEHSDataValue(dataList, EHSDataMapping.GetMappedMeasure(measureList, "TIME_LOST"), (decimal)period.LostTime, updateIndicator);
							EHSDataMapping.SetEHSDataValue(dataList, EHSDataMapping.GetMappedMeasure(measureList, "RESTRICTED_TIME"), (decimal)period.RestrictedTime, updateIndicator);
							EHSDataMapping.SetEHSDataValue(dataList, EHSDataMapping.GetMappedMeasure(measureList, "TIME_LOST_CASES"), (decimal)period.LostTimeCase, updateIndicator);
							EHSDataMapping.SetEHSDataValue(dataList, EHSDataMapping.GetMappedMeasure(measureList, "RECORDED_CASES"), (decimal)period.RecordableCase, updateIndicator);
							EHSDataMapping.UpdateEHSDataList(entities, dataList);

							// update PLANT_ACCOUNTING table stats for backwards compatibility
							PLANT_ACCOUNTING pa = EHSModel.LookupPlantAccounting(entities, locationID, period.PeriodYear, period.PeriodMonth, true);
							pa.TIME_LOST = period.LostTime;
							pa.TOTAL_DAYS_RESTRICTED = period.RestrictedTime;
							pa.TIME_LOST_CASES = period.LostTimeCase;
							pa.RECORDED_CASES = period.RecordableCase;
							EHSModel.UpdatePlantAccounting(entities, pa);
						}
					}
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