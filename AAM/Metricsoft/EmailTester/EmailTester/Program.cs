using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace EmailTester
{
	class Program
	{
		static void Main(string[] args)
		{
			string server = "";
			string fromAddress = "";
			string fromPassword = "";
			string toAddress = "";
			int port = 0;
			bool useCredentials = false;
			bool enableSsl = false;

			try
			{
				//
				// Read config settings
				//
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["server"]))
					server = ConfigurationManager.AppSettings["server"];
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["fromAddress"]))
					fromAddress = ConfigurationManager.AppSettings["fromAddress"];
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["fromPassword"]))
					fromPassword = ConfigurationManager.AppSettings["fromPassword"];
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["toAddress"]))
					toAddress = ConfigurationManager.AppSettings["toAddress"];
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["port"]))
					port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["useCredentials"]))
					useCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["useCredentials"]);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["enableSsl"]))
					enableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSsl"]);

				//
				// Build message
				//
				MailMessage msg = new MailMessage();
				msg.To.Add(toAddress);
				msg.From = new MailAddress(fromAddress);
				msg.Subject = "Test email subject";
				msg.Body = "Test email body";
				msg.IsBodyHtml = true;

				Console.WriteLine("From: " + fromAddress);
				Console.WriteLine("To: " + toAddress);
				Console.WriteLine("Subject: " + msg.Subject);
				Console.WriteLine("Body: " + msg.Body);
				Console.WriteLine("Server: " + server);
				Console.WriteLine("Use Credentials: " + useCredentials);
				Console.WriteLine("Port: " + port);
				Console.WriteLine("Enable SSL: " + enableSsl);

				//
				// Send email
				//
				var client = new SmtpClient(server);
				if (useCredentials == true)
					client.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword);
				else
					client.UseDefaultCredentials = true;

				if (port > 0)
					client.Port = port;
				client.EnableSsl = enableSsl;
				client.Send(msg);

				File.WriteAllText("error.txt", "No errors");
			}
			catch (Exception ex)
			{
				string exception = ex.ToString();
				File.WriteAllText("error.txt", exception);
				Console.WriteLine("Error");
				Console.WriteLine(exception);
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}
