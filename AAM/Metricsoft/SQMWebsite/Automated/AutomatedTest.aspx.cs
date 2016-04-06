using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Net;

namespace SQM.Website.Automated
{
	public partial class AutomatedTest : System.Web.UI.Page
	{
		static PSsqmEntities entities;
		static StringBuilder output;

		protected void Page_Load(object sender, EventArgs e)
		{
			output = new StringBuilder();

			//string currentIP = GetIP4Address();
			string currentIP = GetIPAddress();
			WriteLine("IP: " + currentIP);

			ltrStatus.Text = output.ToString().Replace("\n", "<br/>");
			WriteLogFile();

			//System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
		}

		public string GetIP4Address()
		{
			string IP4Address = String.Empty;

			foreach (IPAddress IPA in Dns.GetHostAddresses(Request.ServerVariables["REMOTE_ADDR"].ToString()))
			{
				if (IPA.AddressFamily.ToString() == "InterNetwork")
				{
					IP4Address = IPA.ToString();
					break;
				}
			}

			if (IP4Address != String.Empty)
			{
				return IP4Address;
			}

			foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
			{
				try
				{
					WriteLine("IP: " + IPA.ToString() + "; Family: " + IPA.AddressFamily.ToString() + "<br>");
				}
				catch { }
				if (IPA.AddressFamily.ToString() == "InterNetwork")
				{
					IP4Address = IPA.ToString();
					//break;
				}
			}

			return IP4Address;
		}

		public string GetIPAddress ()
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			// Get the IP
			string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
			WriteLine("IP: " + myIP.ToString() + "; Host: " + hostName.ToString());
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