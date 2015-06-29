using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website.Automated
{
	public partial class MailTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			


		}

		protected void btnEmail_Click(object sender, EventArgs e)
		{

			var mailMessage = new MailMessage();

			string fromAddress = "global_appsgen@varroclighting.com";
			string smtpServer = "smtp.office365.com";
			//int smtpPort = 587;
			int smtpPort = 25;

			//mailMessage.From = new MailAddress(fromAddress);
			//mailMessage.To.Add(new MailAddress("john@luxinteractive.com"));

			////mailMessage.Priority = MailPriority.High;
			//mailMessage.Subject = "Subject";
			//mailMessage.Body = "test";

			//var smtpClient = new System.Net.Mail.SmtpClient(smtpServer);
			//smtpClient.Port = smtpPort;

			////smtpClient.Port = 25;

			////smtpClient.Port = 993;
			////smtpClient.EnableSsl = true;
			////smtpClient.Credentials = new System.Net.NetworkCredential(fromAddress, "password");
			////smtpClient.UseDefaultCredentials = true;

			//smtpClient.Send(mailMessage);


			SmtpClient client = new SmtpClient("136.18.15.19");
			//SmtpClient client = new SmtpClient("smtp.office365.com", 587);
			//client.EnableSsl = true;
			//client.Credentials = new System.Net.NetworkCredential("global_appsgen@varroclighting.com", "");
			//client.UseDefaultCredentials = true;

			MailAddress from = new MailAddress("global_appsgen@varroclighting.com");
			MailAddress to = new MailAddress("john@luxinteractive.com");
			MailMessage message = new MailMessage(from, to);
			message.Body = "The message I want to send.";
			message.Subject = "The subject of the email";

			client.Send(message);

		}
	}
}