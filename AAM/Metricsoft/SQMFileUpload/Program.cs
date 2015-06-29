using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQM.Website;

namespace SQMFileUpload
{
	class Program
	{
		static StringBuilder output;
		static SQMFileReader fileReader;
		static FileInfo uploadFile;
		static FileStream fileStream;
		static byte[] fileContent;
		static int companyID;
		static int uploadSource;
		static char[] fileDelimiter;
		static double plantDataMultiplier;
		static string originalFile;
		static string timestamp;
		static string companyFile;
		static string plantFile;
		static string partFile;
		static string plantDataFile;
		static string currencyFile;
		static string defaultPartProgram;
		static string fileLocation;
		static string fileType;
		static string emailBody;
		static SQM.Website.PSsqmEntities entities;
		static string mailServer;
		static string mailFrom;
		static string mailSMTPPort;
		static string mailEnableSSL;
		static string mailURL;
		static string mailPassword;
		static string logFileEmail;
		static int fileCount;
		public enum ListStyle
		{
			Unix,
			Windows
		}
 
		static void Main(string[] args)
		{
			output = new StringBuilder();
			entities = new PSsqmEntities();

			List<SETTINGS> MailSettings = SQMSettings.SelectSettingsGroup("MAIL", "");
			foreach (SETTINGS set in MailSettings)
			{
				switch (set.SETTING_CD)
				{
					case "MailServer":
						mailServer = set.VALUE;
						break;
					case "MailFrom":
						mailFrom = set.VALUE;
						break;
					case "MailSMTPPort":
						mailSMTPPort = set.VALUE;
						break;
					case "MailEnableSSL":
						mailEnableSSL = set.VALUE;
						break;
					case "MailURL":
						mailURL = set.VALUE;
						break;
					case "MailPassword":
						mailPassword = set.VALUE;
						break;
					default:
						break;
				}
			}

			List<SETTINGS> ImportSettings = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", "");
			foreach (SETTINGS ris in ImportSettings)
			{
				switch (ris.SETTING_CD)
				{
					case "CompanyID":
						companyID = Convert.ToInt16(ris.VALUE);
						break;
					case "FileLocation":
						fileLocation = ris.VALUE;
						break;
					case "UploadSources":
						try
						{
							uploadSource = Convert.ToInt32(ris.VALUE);
						}
						catch
						{
							WriteLine("Source file locations have not been defined correctly in the configuration file. No files were processed.");
							uploadSource = 0;
						}
						break;
					case "CompanyFile":
						companyFile = ris.VALUE;
						break;
					case "PlantFile":
						plantFile = ris.VALUE;
						break;
					case "PartFile":
						partFile = ris.VALUE;
						break;
					case "PlantDataFile":
						plantDataFile = ris.VALUE;
						break;
					case "CurrencyFile":
						currencyFile = ris.VALUE;
						break;
					case "DefaultPartProgram":
						defaultPartProgram = ris.VALUE;
						break;
					default:
						break;
				}
			}

			timestamp = string.Format("{0:yyyy-MM-dd-HHmm}", DateTime.Now);
			emailBody = "";

			for (int i = 1; i <= uploadSource; i++)
			{
				string fileLocationKey = "UploadFileLocation" + i.ToString();
				SETTINGS result = ImportSettings.Find(x => x.SETTING_CD == fileLocationKey);
				fileLocation = result.VALUE;
				result = ImportSettings.Find(x => x.SETTING_CD == "FileDelimiter" + i.ToString());
				fileDelimiter = result.VALUE.ToCharArray();
				result = ImportSettings.Find(x => x.SETTING_CD == "UploadFileType" + i.ToString());
				fileType = result.VALUE;
				result = ImportSettings.Find(x => x.SETTING_CD == "PlantDataMultiplier" + i.ToString());
				plantDataMultiplier = Convert.ToDouble(result.VALUE);
				WriteLine("Processing files in: " + fileLocation);
				if (fileType.ToLower().Equals("ftp"))
				{
					try
					{
						// check for each file separately and process
						WriteLine("Start Company file processing.");
						// Process each file type
						ProcessFTPInputFile("COMPANY");
						WriteLine("<br/>Start Plant file processing.");
						ProcessFTPInputFile("PLANT");
						WriteLine("<br/>Start Part file processing.");
						ProcessFTPInputFile("PART");
						WriteLine("<br/>Start Plant Data file processing.");
						ProcessFTPInputFile("PLANT_DATA");
						//WriteLine("<br/>Start Currency Data file processing.");
						//ProcessFTPInputFile("CURRENCY_DATA");
						WriteLine("<br/><br/>");
					}
					catch (Exception ex)
					{
						WriteLine("Error: " + ex.ToString());
					}
				}
				else
				{
					string[] fileEntries;
					try
					{
						// check for each file separately and process
						WriteLine("Start Company file processing.");
						// Process each file type
						//originalFile = fileLocation + ConfigurationSettings.AppSettings["CompanyFile"];
						fileEntries = Directory.GetFiles(fileLocation, companyFile);
						fileCount = 0;
						foreach (string fileName in fileEntries)
						{
							fileCount += 1;
							ProcessInputFile("COMPANY", fileName);
						}
						WriteLine("<br/>Start Plant file processing.");
						//originalFile = fileLocation + ConfigurationSettings.AppSettings["PlantFile"];
						fileEntries = Directory.GetFiles(fileLocation, plantFile);
						fileCount = 0;
						foreach (string fileName in fileEntries)
						{
							if (!fileName.ToLower().Contains("_data"))
							{
								fileCount += 1;
								ProcessInputFile("PLANT", fileName);
							}
						}
						WriteLine("<br/>Start Part file processing.");
						//originalFile = fileLocation + ConfigurationSettings.AppSettings["PartFile"];
						fileEntries = Directory.GetFiles(fileLocation, partFile);
						fileCount = 0;
						foreach (string fileName in fileEntries)
						{
							fileCount += 1;
							ProcessInputFile("PART", fileName);
						}
						WriteLine("<br/>Start Plant Data file processing.");
						//originalFile = fileLocation + ConfigurationSettings.AppSettings["PlantDataFile"];
						fileEntries = Directory.GetFiles(fileLocation, plantDataFile);
						fileCount = 0;
						foreach (string fileName in fileEntries)
						{
							fileCount += 1;
							ProcessInputFile("PLANT_DATA", fileName);
						}
						//WriteLine("<br/>Start Currency Data file processing.");
						//originalFile = fileLocation + currencyFile;
						//ProcessInputFile("CURRENCY_DATA", originalFile);
						WriteLine("<br/><br/>");
					}
					catch (Exception ex)
					{
						WriteLine("Error: " + ex.ToString());
					}
				}
			}
			
			WriteLogFile();
		}

		static void ProcessFTPInputFile(string fileType)
		{
			// check for a file (look for all files that contain the first part of the file name.
			DateTime cutOff = DateTime.Now.AddDays(-1);
			List<FTPLineResult> results;
			string newFile = "";
			switch (fileType)
			{
				case "COMPANY":
					Regex matchExpression = new Regex("^company$", RegexOptions.IgnoreCase);
					results = FTPHelper.GetFilesListSortedByDate(fileLocation, matchExpression, cutOff);
					newFile = fileLocation + "COMPANY_" + timestamp + ".txt";
					break;
				case "PLANT":
					newFile = fileLocation + "PLANT_" + timestamp + ".txt";
					break;
				case "PART":
					newFile = fileLocation + "PART_" + timestamp + ".txt";
					break;
				case "PLANT_DATA":
					newFile = fileLocation + "PLANT_DATA_" + timestamp + ".txt";
					break;
				case "CURRENCY_DATA":
					newFile = fileLocation + "CURRENCY_DATA_" + timestamp + ".txt";
					break;
				default:
					break;
			}

			if (newFile.Length == 0)
			{
				WriteLine("Invalid file type: " + fileType);
				return;
			}

			// need to figure out what we are doing...

			//if (File.Exists(oldFile))
			//{
			//	try
			//	{
			//		// rename the file
			//		File.Move(oldFile, newFile);
			//	}
			//	catch
			//	{
			//		WriteLine("Error renaming the data file: " + oldFile + " to " + newFile);
			//		return;
			//	}
			//}
			//else
			//{
			//	WriteLine("File does not exist: " + oldFile);
			//	return;
			//}

			// process the renamed file
			uploadFile = new FileInfo(newFile);
			fileStream = new FileStream(newFile, FileMode.Open, FileAccess.Read);
			long fileLen = uploadFile.Length;
			fileContent = new byte[fileStream.Length];
			if (fileLen < 5)
			{
				WriteLine("The file does not contain relevant data: " + newFile);
				return;
			}
			int nBytes = fileStream.Read(fileContent, 0, Convert.ToInt32(fileLen));
			// if we ever decide to upload currency file through this program, we will need to set the last three parameters to Year, Month & Pref Currency
			fileReader = new SQMFileReader().InitializeCSV(companyID, fileType + ".TXT", fileContent, fileDelimiter, plantDataMultiplier, 0, 0, "");
			if (fileReader.Status < 0)
			{
				WriteLine("Error encountered loading the file: " + newFile);
				return;
			}
			ProcessFile(fileType);
		}

		static void ProcessInputFile(string fileType, string oldFile)
		{
			// check for a file
			string newFile = "";
			switch (fileType)
			{
				case "COMPANY":
					newFile = fileLocation + "COMPANY_" + timestamp + "_" + fileCount + ".txt";
					break;
				case "PLANT":
					newFile = fileLocation + "PLANT_" + timestamp + "_" + fileCount + ".txt";
					break;
				case "PART":
					newFile = fileLocation + "PART_" + timestamp + "_" + fileCount + ".txt";
					break;
				case "PLANT_DATA":
					newFile = fileLocation + "PLANT_DATA_" + timestamp + "_" + fileCount + ".txt";
					break;
				case "CURRENCY_DATA":
					newFile = fileLocation + "CURRENCY_DATA_" + timestamp + "_" + fileCount + ".txt";
					break;
				default:
					break;
			}

			if (newFile.Length == 0)
			{
				WriteLine("Invalid file type: " + fileType);
				return;
			}

			if (File.Exists(oldFile))
			{
				try
				{
					// rename the file
					File.Move(oldFile, newFile);
				}
				catch
				{
					WriteLine("Error renaming the data file: " + oldFile + " to " + newFile);
					return;
				}
			}
			else
			{
				WriteLine("File does not exist: " + oldFile);
				return;
			}

			// process the renamed file
			uploadFile = new FileInfo(newFile);
			fileStream = new FileStream(newFile, FileMode.Open, FileAccess.Read);
			long fileLen = uploadFile.Length;
			fileContent = new byte[fileStream.Length];
			if (fileLen < 5)
			{
				WriteLine("The file does not contain relevant data: " + newFile);
				return;
			}
			int nBytes = fileStream.Read(fileContent, 0, Convert.ToInt32(fileLen));
			// if we ever decide to upload currency file through this program, we will need to set the last three parameters to Year, Month & Pref Currency
			fileReader = new SQMFileReader().InitializeCSV(companyID, fileType + ".TXT", fileContent, fileDelimiter, plantDataMultiplier, 0, 0, "");
			if (fileReader.Status < 0)
			{
				WriteLine("Error encountered loading the file: " + newFile);
				return;
			}
			ProcessFile(fileType);
		}

		static void ProcessFile(string fileType)
		{
			fileReader.LoadCSV();

			if (fileReader.UpdateList.Count > 0)
			{
				WriteLine(fileReader.FileName + " records processed: " + fileReader.UpdateList.Count.ToString());
				// no need to write these out to a file
			}

			if (fileReader.ErrorList.Count > 0)
			{
				WriteLine(fileReader.FileName + " records with errors: " + fileReader.ErrorList.Count.ToString());
				// we need to create an error file... 
				StringBuilder dataFile = new StringBuilder();
				for (int i = 0; i < fileReader.ErrorList.Count; i++)
				{
					WriteLine("Line: " + fileReader.ErrorList[i].LineNo + "; Node: " + fileReader.ErrorList[i].Node + "; Message: " + fileReader.ErrorList[i].Message);
					dataFile.AppendLine(fileReader.ErrorList[i].DataLine);
				}
				string fullPath = fileLocation + fileType + "_ERRORS_" + timestamp + ".txt";
				File.WriteAllText(fullPath, dataFile.ToString());
			}
		}

		static void WriteLogFile()
		{
			// write the log file out to the log directory, but also send the file in an email.
			string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\";

			string body = "";
			body += "The SQM File Upload program processed at " + timestamp;
			body += "<br/><br/>If there were any errors encountered, the records can be found in the corresponding error data files in the following folder: " + fileLocation;
			body += "<br/><br/>The following log information can be found in: " + logPath + "yyyy-mm-dd-hhmm.txt";
			body += "<br/><br/>Log: <br/><br/>";
			body += emailBody;

			try
			{
				MailMessage msg = new MailMessage();
				msg.To.Add(logFileEmail);
				msg.From = new MailAddress(mailFrom);
				msg.Subject = "MetricSoft File Upload Log";
				msg.Body = body;
				msg.Priority = MailPriority.Normal;
				msg.IsBodyHtml = true;

				SmtpClient client = new SmtpClient();
				client.Credentials = new System.Net.NetworkCredential(mailFrom, mailPassword);
				client.Port = Convert.ToInt32(mailSMTPPort);
				client.Host = mailServer;
				if (mailEnableSSL.ToLower().Equals("true"))
					client.EnableSsl = true;
				else
					client.EnableSsl = false;

				client.Send(msg);
			}
			catch (Exception ex)
			{
				WriteLine("Error: " + ex.ToString());
			}

			try
			{
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + timestamp + ".txt";
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
				WriteLine("Error: " + ex.ToString());
			}

		}

		static void WriteLine(string text)
		{
			Console.WriteLine(text);
			output.AppendLine(text);
			emailBody += text + "<br/>";
		}

		#region FTP file access
		#endregion
	}
}
