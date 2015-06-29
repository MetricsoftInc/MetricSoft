using System;
using System.Linq;
using System.Net.Mail;

namespace SQM.Website
{
	public static class EmailMgr
	{
		const string MailServer = "smtp.gmail.com";
		const string MailFrom = "steve.taylor@qualityanalytics.com";
		const string MailPassword = "Corrina7010";
		const int MailSmtpPort = 587; // Gmail works on this port
		const bool MailEnableSsl = true;

		public static void SendEmailToPerson(PERSON emailRecipient, string subject, string body)
		{
			try
			{
				MailMessage msg = new MailMessage();
				msg.To.Add(emailRecipient.EMAIL.Trim());
				msg.From = new MailAddress(MailFrom);
				msg.Subject = subject;
				msg.Body = body;
				msg.Priority = MailPriority.Normal;
				msg.IsBodyHtml = true;

				SmtpClient client = new SmtpClient();
				client.Credentials = new System.Net.NetworkCredential(MailFrom, MailPassword);
				client.Port = MailSmtpPort; 
				client.Host = MailServer;
				client.EnableSsl = MailEnableSsl;

				client.Send(msg);
			}
			catch (Exception ex)
			{
				
			}
		}

	}
}