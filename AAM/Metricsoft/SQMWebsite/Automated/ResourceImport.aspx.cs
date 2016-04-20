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
		static List<SETTINGS> sets;
		static SQMFileReader fileReader;
		static char[] fileDelimiter;
		static double plantDataMultiplier;
		static int primaryCompany;

		protected void Page_Load(object sender, EventArgs e)
		{
			output = new StringBuilder();
			entities = new PSsqmEntities();

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

			sets = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", "");
			primaryCompany = Convert.ToInt32(sets.Find(x => x.SETTING_CD == "CompanyID").VALUE);
			fileDelimiter = sets.Find(x => x.SETTING_CD == "FileDelimiter1").VALUE.ToCharArray();

			sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); 
			bool validIP = false;
			WriteLine("Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			try
			{
				string currentIP = GetIPAddress();

				string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();
				if (strValidIP.Equals(currentIP))
				{
					WriteLine("Resource Import being accessed from a valid IP address " + currentIP);
					validIP = true;

					if (Request.QueryString["validation"] != null)
					{
						if (Request.QueryString["validation"].ToString().Equals("Vb12M11a4"))
							validIP = true;
					}
					else
					{
						WriteLine("Resource Import requested from incorrect source.");
						validIP = false;
					}
				}
				else
				{
					WriteLine("Resource Import being accessed from invalid IP address " + currentIP);
					validIP = false;
				}
			}
			catch (Exception ex)
			{
				validIP = false;
				WriteLine("Resource Import Error validating IP Address: " + ex.ToString());
			}

			// make sure this code is NOT moved to production
			//validIP = true;

			if (validIP)
			{
				try
				{
					// Process resource file
					ProcessFile("Reference");
				}
				catch (Exception ex)
				{
					WriteLine("Error Processing Reference File: " + ex.ToString());
				}

				try
				{
					// Process Person Input file from PeopleSoft
					ProcessFile("Person");
				}
				catch (Exception ex)
				{
					WriteLine("Error Processing Person File: " + ex.ToString());
					//WriteLine("Main ScheduleAudits Detailed Error: " + ex.InnerException.ToString());
				}
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();

			if (pageMode != "web")
				System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
		}

		public string GetIPAddress()
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			
			string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString(); // Get the IP

			return myIP;
		}

		static void ProcessFile(string fileType)
		{
			string fileSearch = fileType.ToUpper() + "_*";
			string strftpLocation = sets.Find(x => x.SETTING_CD == "FTP_LOCATION").VALUE.ToString();
			DirectoryInfo dir = new DirectoryInfo(strftpLocation);
			FileInfo[] files = dir.GetFiles(fileSearch).OrderByDescending(p => p.LastWriteTime).ToArray();
			bool fileProcessed;
			if (files != null && files.Count() > 0)
			{
				byte[] buff;
				fileProcessed = false;
				foreach (FileInfo file in files)
				{
					//_file = new FileInfo(file);
					//_fileName = _fileName + @"<br/>" + _file.Name;
					buff = File.ReadAllBytes(file.FullName);
					fileReader = new SQMFileReader().InitializeCSV(primaryCompany, file.Name, buff, fileDelimiter, plantDataMultiplier, 0, 0, "USD");
					if (fileReader.Status < 0)
					{
						WriteLine("Error encountered loading the file: " + file);
					}
					else
					{
						if (!fileProcessed) // only process the most recent file
						{
							fileReader.FileName = fileType.ToUpper();
							fileReader.LoadCSV();
							fileProcessed = true;
							WriteLine(file.Name + " processed " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
						}
						// move the file
						string sourceFile = file.FullName;
						string destinationFile = strftpLocation + @"\Processed\" + file.Name;
						System.IO.File.Move(sourceFile, destinationFile);
					}
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
				string fullPath = logPath + "ResourceImportLog_" + string.Format("{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
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