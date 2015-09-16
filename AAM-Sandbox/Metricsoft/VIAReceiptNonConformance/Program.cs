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

namespace VIAReceiptInspection
{
	class Program
	{
		static StringBuilder output;
		static SQMFileReader fileReader;
		static FileInfo uploadFile;
		static FileStream fileStream;
		static byte[] fileContent;
		static int companyID;
		static char[] fileDelimiter;
		static string originalFile;
		static string timestamp;
		static string fileLocation;
		static string receiptFile;
		static string inspectionFile;
		static string emailBody;
		static string logFileEmail;
		static string mailServer;
		static string mailFrom;
		static string mailSMTPPort;
		static string mailEnableSSL;
		static string mailURL;
		static string mailPassword;
		static int fileCount;
		static SQM.Website.PSsqmEntities entities;
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

			List<SETTINGS> ReceiptImportSettings = SQMSettings.SelectSettingsGroup("IMPORT_RECEIPT", "");
			foreach (SETTINGS ris in ReceiptImportSettings)
			{
				switch (ris.SETTING_CD)
				{
					case "CompanyID":
						companyID = Convert.ToInt16(ris.VALUE);
						break;
					case "FileLocation":
						fileLocation = ris.VALUE;
						break;
					case "FileDelimiter":
						fileDelimiter = ris.VALUE.ToCharArray();
						break;
					case "LogFileEmail":
						logFileEmail = ris.VALUE;
						break;
					case "ReceiptFile":
						receiptFile = ris.VALUE + "*";
						break;
					case "InspectionFile":
						inspectionFile = ris.VALUE + "*";
						break;
					default:
						break;
				}
			}

			timestamp = string.Format("{0:yyyy-MM-dd-HHmm}", DateTime.Now);
			emailBody = "";

			WriteLine("Processing files in: " + fileLocation);
			string[] fileEntries;
			try
			{
				// check for each file separately and process
				WriteLine("Start Receipt file processing.");
				// Process each file type
				fileEntries = Directory.GetFiles(fileLocation, receiptFile);
				fileCount = 0;
				foreach (string fileName in fileEntries)
				{
					fileCount += 1;
					ProcessInputFile("RECEIPT", fileName);
				}
				WriteLine("<br/>Start Inspection file processing.");
				//originalFile = fileLocation + ConfigurationSettings.AppSettings["PlantFile"];
				fileEntries = Directory.GetFiles(fileLocation, inspectionFile);
				fileCount = 0;
				foreach (string fileName in fileEntries)
				{
					fileCount += 1;
					ProcessInputFile("INSPECT", fileName);
				}
				WriteLine("<br/><br/>");
			}
			catch (Exception ex)
			{
				WriteLine("Error: " + ex.ToString());
			}

			WriteLogFile();
		}

		static void ProcessInputFile(string fileType, string oldFile)
		{
			// check for a file
			string newFile = "";
			switch (fileType)
			{
				case "RECEIPT":
					newFile = fileLocation + @"processed\RECEIPT_" + timestamp + "_" + fileCount + ".txt";
					break;
				case "INSPECT":
					newFile = fileLocation + @"processed\INSPECTION_" + timestamp + "_" + fileCount + ".txt";
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
			fileReader = new SQMFileReader().InitializeCSV(companyID, fileType + ".TXT", fileContent, fileDelimiter, 0, 0, 0, "");
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
