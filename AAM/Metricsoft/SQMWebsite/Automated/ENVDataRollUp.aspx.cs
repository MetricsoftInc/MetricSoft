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
	public partial class ENVDataRollUp : System.Web.UI.Page
	{
		static StringBuilder output;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				//System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
				return;
			}

			output = new StringBuilder();
			SETTINGS setting = null;
			bool validIP = true;
			string pageURI = HttpContext.Current.Request.Url.AbsoluteUri;
			string nextPage = "";
			DateTime toDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
			DateTime fromDate = toDate.AddMonths(-3);

			WriteLine("ENV Data Rollup Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			WriteLine(pageURI);

			try
			{
				string currentIP = GetIPAddress();
				List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

				string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_ENV_PERIODSPAN").FirstOrDefault();
				if (setting != null)
					fromDate = toDate.AddMonths(Convert.ToInt32(setting.VALUE) * -1);

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_ENV_NEXTPAGE").FirstOrDefault();
				if (setting != null && !string.IsNullOrEmpty(setting.VALUE) && setting.VALUE.Length > 1)
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
				WriteLine("Main ENV Data RollUp Error validating IP Address: " + ex.ToString());
			}

			// make sure this code is NOT moved to production
			//validIP = true;

			if (!validIP)
			{
				WriteLine("Main ENV Data RollUp Invalid IP Address");
				ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
				WriteLogFile();

				//System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
				return;
			}

			try
			{
				PSsqmEntities entities = new PSsqmEntities();

				int status = 0;
				DateTime currencyDate = DateTime.MinValue;

				CURRENCY_XREF latestCurrency = CurrencyMgr.GetLatestRecord(entities);
				if (latestCurrency != null)
				{
					currencyDate = new DateTime(latestCurrency.EFF_YEAR, latestCurrency.EFF_MONTH, DateTime.DaysInMonth(latestCurrency.EFF_YEAR, latestCurrency.EFF_MONTH));
				}
				WriteLine("Max Currency Date = " + currencyDate.ToShortDateString());


				List<EHSProfile> profileList = new List<EHSProfile>();
				foreach (decimal plantID in (from p in entities.EHS_PROFILE select p.PLANT_ID).ToList())
				{
					profileList.Add(new EHSProfile().Load(Convert.ToDecimal(plantID), false, true));
				}

				foreach (EHSProfile profile in profileList)		// do each plant having a metric profile
				{
					WriteLine(profile.Plant.PLANT_NAME);
					DateTime periodDate = fromDate;

					while (periodDate <= toDate)				// do each month within the rollup span
					{
						WriteLine(" " + periodDate.Year.ToString() + "/" + periodDate.Month.ToString());
						if (profile.InputPeriod == null  ||  profile.InputPeriod.PeriodDate != periodDate)
							profile.LoadPeriod(periodDate);

						if (profile.ValidPeriod())
						{
							if (!profile.InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
							{
								profile.InputPeriod.PlantAccounting.APPROVAL_DT = toDate;
								profile.InputPeriod.PlantAccounting.APPROVER_ID = 1m;   // default to the sysadmin user
							}
							status = profile.UpdateMetricHistory(periodDate);  // new roll-up logic 
							WriteLine(" ... " + status.ToString());
							periodDate = periodDate.AddMonths(1);
						}
					}
				}
			}

			catch (Exception ex)
			{
				WriteLine("Main ENV Data RollUp Error - " + ex.ToString());
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

			//System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
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