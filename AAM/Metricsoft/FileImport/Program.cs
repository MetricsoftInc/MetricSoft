using SQM.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.IO;

namespace FileImport
{
    class Program
    {
		static PSsqmEntities entities;
		static StringBuilder output;
		static List<SETTINGS> sets;
		static SQMFileReader fileReader;
		static char[] fileDelimiter;
		static double plantDataMultiplier;
		static int primaryCompany;

        static void Main(string[] args)
        {
			output = new StringBuilder();
			plantDataMultiplier = 1.0;

			WriteLine("Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			entities = new PSsqmEntities();

			sets = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", "");
			primaryCompany = Convert.ToInt32(sets.Find(x => x.SETTING_CD == "CompanyID").VALUE);
			fileDelimiter = sets.Find(x => x.SETTING_CD == "FileDelimiter1").VALUE.ToCharArray();
			sets = SQMSettings.SelectSettingsGroup("AUTOMATE", "");

			try
			{
				// arguments:
				// no arguments supplied == process all files
				// ref == import references files only
				// person == import person files only

				if (args.Length == 0 || args.Contains("ref"))
				{
					// Process resource file
					ProcessFile("Reference");
				}
			}
			catch (Exception ex)
			{
				WriteLine("Error Processing Reference File: " + ex.ToString());
			}

			try
			{
				if (args.Length == 0 || args.Contains("person"))
				{
					// Process Person Input file from PeopleSoft
					ProcessFile("Person");
				}
			}
			catch (Exception ex)
			{
				WriteLine("Error Processing Person File: " + ex.ToString());
			}


			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			WriteLogFile();
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
				//string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\";
				string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				logPath = logPath.Substring(0, logPath.IndexOf("\\bin"));
				logPath = logPath + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("FileImport_{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
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
				try
				{
					WriteLine("WriteLogFile Detailed Error: " + ex.StackTrace.ToString());
				}
				catch { }
			}
		}

		static void WriteLine(string text)
		{
			output.AppendLine(text);
		}
    }
}
