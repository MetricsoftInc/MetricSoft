using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Drawing;
using System.IO;
using System.Net;
using SQM.Website.Classes;

namespace SQM.Website
{
	public partial class EHS_Alert : System.Web.UI.Page
	{
		PSsqmEntities entities;
		string baseAppUrl = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			entities = new PSsqmEntities();
			baseAppUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

			decimal id = 0;

			string customerLogo = "";
			customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();

			string imageUrl = "";

			if (!string.IsNullOrEmpty(customerLogo))
				imageUrl = baseAppUrl + "images/company/" + customerLogo;
			else
                imageUrl = baseAppUrl + "images/company/MetricsoftLogo.png";

			imgLogo.ImageUrl = "data:image/jpg;base64," + ImageUrlToBase64String(imageUrl);

			try
			{
				ltrCaseName.Text = "EH&amp;S Incident Alert";

				if (Request.QueryString["pcid"] != null)
				{
					string query = Request.QueryString["pcid"];
					query = query.Replace(" ", "+");
					string pcid = EncryptionManager.Decrypt(query);
					id = Convert.ToInt32(pcid);
					PopulateByProblemCaseId(id);


				}
				else if (Request.QueryString["iid"] != null)
				{
					string query = Request.QueryString["iid"];
					query = query.Replace(" ", "+");
					string pcid = EncryptionManager.Decrypt(query);
					id = Convert.ToInt32(pcid);
					PopulateByIncidentId(id);


				}



				pnlContent.Visible = true;
				pnlError.Visible = false;
			}
			catch
			{
				pnlContent.Visible = false;
				pnlError.Visible = true;
			}
		}
	
		void PopulateByProblemCaseId (decimal problemCaseId)
		{
			PROB_CASE probCase = ProblemCase.LookupCase(entities, problemCaseId);
			
			if (probCase != null)
			{
				List<INCIDENT> incidentList = ProblemCase.LookupProbIncidentList(entities, probCase);

				ltrDate.Text = probCase.CREATE_DT.ToString();
				if (incidentList.Count > 0)
				{
					var incident = incidentList[0];

					string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)incident.DETECT_PLANT_ID);
					lblPlantName.Text = String.Format("Location: {0}", plantName);
					lblIncidentId.Text = String.Format("Incident ID: {0}", incident.INCIDENT_ID);
					//lblCaseId.Text = String.Format("Problem Case ID: {0}", probCase.PROBCASE_ID);

					string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(incident.INCIDENT_ID);
					decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(incident.INCIDENT_ID);
					decimal companyId = incident.DETECT_COMPANY_ID;
					var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
					questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

					// Date/Time

					ltrDate.Text = incidentList[0].INCIDENT_DT.ToLongDateString();

					var timeQuestion = questions.FirstOrDefault(q => q.QuestionId == 5);
					if (timeQuestion != null)
					{
						string timeAnswer = (from a in entities.INCIDENT_ANSWER
											 where
												 a.INCIDENT_ID == incident.INCIDENT_ID &&
												 a.INCIDENT_QUESTION_ID == 5
											 select a.ANSWER_VALUE).FirstOrDefault();

						if (!string.IsNullOrEmpty(timeAnswer))
							ltrTime.Text = Convert.ToDateTime(timeAnswer).ToShortTimeString();
					}

					// Incident Type

					ltrIncidentType.Text = incidentType;

					// Description

					ltrDescription.Text = "<div style=\"width: 600px; word-wrap: break-word;\">" + Server.HtmlEncode(probCase.DESC_LONG) + "</div>";


					// Root Cause(s)

					List<PROB_CAUSE_STEP> probCauses = (from i in entities.PROB_CAUSE_STEP
														where
															i.PROBCASE_ID == problemCaseId &&
															i.IS_ROOTCAUSE == true
														select i).ToList();
					if (probCauses.Count > 0)
					{
						ltrRootCause.Text = "<ul>";
						foreach (var pc in probCauses)
							ltrRootCause.Text += "<li>" + Server.HtmlEncode(pc.WHY_OCCUR) + "</li>";
						ltrRootCause.Text += "</ul>";
					}

					// Containment

					var containment = (from i in entities.PROB_CONTAIN
									   where
										   i.PROBCASE_ID == problemCaseId
									   select new
									   {
										   Disposition = i.INITIAL_DISPOSITION,
										   Action = i.INITIAL_ACTION,
										   Results = i.INITIAL_RESULTS
									   }).FirstOrDefault();

					if (containment != null)
					{
						ltrContainment.Text = "<ul><li>Initial Disposition: " + Server.HtmlEncode(containment.Disposition) + "</li>" +
							"<li>Action: " + Server.HtmlEncode(containment.Action) + "</li>" +
							"<li>Results: " + Server.HtmlEncode(containment.Results) + "</li>" +
							"</ul>";
					}

					// Corrective Actions

					var correctiveActions = (from i in entities.PROB_CAUSE_ACTION
											 where
												 i.PROBCASE_ID == problemCaseId
											 select i.ACTION_DESC).ToList();
					if (correctiveActions.Count > 0)
					{
						ltrCorrectiveActions.Text = "<ul>";
						foreach (var caDesc in correctiveActions)
							ltrCorrectiveActions.Text += "<li>" + Server.HtmlEncode(caDesc) + "</li>";
						ltrCorrectiveActions.Text += "</ul>";
					}

					// Photos

					BindAttachmentsProbCase(incident.INCIDENT_ID, probCase.PROBCASE_ID);
				}
				else
				{
					pnlContent.Visible = false;
					pnlError.Visible = true;
				}

			}

				
		}

		void PopulateByIncidentId (decimal incidentId)
		{
			var entities = new PSsqmEntities();

			var incident = EHSIncidentMgr.SelectIncidentById(entities, incidentId);

			string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)incident.DETECT_PLANT_ID);
			lblPlantName.Text = String.Format("Location: {0}", plantName);
			lblIncidentId.Text = String.Format("Incident ID: {0}", incidentId);
			//lblCaseId.Text = String.Format("Incident ID: {0}", incidentId);

			string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(incidentId);
			decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(incidentId);
			decimal companyId = incident.DETECT_COMPANY_ID;
			var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
			questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

			// Date/Time

			ltrDate.Text = incident.INCIDENT_DT.ToLongDateString();

			var timeQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.TimeOfDay);
			if (timeQuestion != null)
			{
				string timeAnswer = (from a in entities.INCIDENT_ANSWER
									 where
										 a.INCIDENT_ID == incident.INCIDENT_ID &&
										 a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay
									 select a.ANSWER_VALUE).FirstOrDefault();

				if (!string.IsNullOrEmpty(timeAnswer))
					ltrTime.Text = Convert.ToDateTime(timeAnswer).ToShortTimeString();
			}

			// Incident Type

			ltrIncidentType.Text = incidentType;

			// Description

			ltrDescription.Text = "<div style=\"width: 600px; word-wrap: break-word;\">" + Server.HtmlEncode(incident.DESCRIPTION) + "</div>";

			// Root Cause(s)

			var rootCauseQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.RootCause);
			if (rootCauseQuestion != null)
			{
				string rootCauseAnswer = (from a in entities.INCIDENT_ANSWER
									 where
										 a.INCIDENT_ID == incidentId &&
										 a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.RootCause
									 select a.ANSWER_VALUE).FirstOrDefault();

				if (!string.IsNullOrEmpty(rootCauseAnswer))
					ltrRootCause.Text = "<div style=\"width: 600px; word-wrap: break-word;\">" + Server.HtmlEncode(rootCauseAnswer) + "</div>";
			}

			
			// Containment

			var containmentQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.Containment);
			if (containmentQuestion != null)
			{
				string containmentAnswer = (from a in entities.INCIDENT_ANSWER
										  where
											  a.INCIDENT_ID == incidentId &&
											  a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Containment
										  select a.ANSWER_VALUE).FirstOrDefault();

				if (!string.IsNullOrEmpty(containmentAnswer))
					ltrContainment.Text = "<div style=\"width: 600px; word-wrap: break-word;\">" + Server.HtmlEncode(containmentAnswer) + "</div>";
			}

			// Corrective Actions

			var correctiveQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.CorrectiveActions);
			if (correctiveQuestion != null)
			{
				string correctiveAnswer = (from a in entities.INCIDENT_ANSWER
											where
												a.INCIDENT_ID == incidentId &&
												a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.CorrectiveActions
											select a.ANSWER_VALUE).FirstOrDefault();

				if (!string.IsNullOrEmpty(correctiveAnswer))
					ltrCorrectiveActions.Text = "<div style=\"width: 600px; word-wrap: break-word;\">" + Server.HtmlEncode(correctiveAnswer) + "</div>";
			}

			// Photos

			BindAttachmentsIncident(incident.INCIDENT_ID);
		}


		public void BindAttachmentsIncident(decimal incidentId)
		{
			var entities = new PSsqmEntities();
			var files = (from a in entities.ATTACHMENT
						 where
							(a.RECORD_ID == incidentId && a.RECORD_TYPE == 40 && a.DISPLAY_TYPE > 0) &&
							(a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
							a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") || a.FILE_NAME.ToLower().Contains(".bmp"))
						 orderby a.RECORD_TYPE, a.FILE_NAME
						 select new
						 {
							 AttachmentId = a.ATTACHMENT_ID,
							 FileName = a.FILE_NAME,
							 Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
							 Size = a.FILE_SIZE
						 }).ToList();

			if (files.Count > 0)
			{
				rptAttachments.DataSource = files;
				rptAttachments.DataBind();
			}

		}

		public void BindAttachmentsProbCase(decimal incidentId, decimal probCaseId)
		{
			var entities = new PSsqmEntities();
			var files = (from a in entities.ATTACHMENT
						 where
							((a.RECORD_ID == incidentId && a.RECORD_TYPE == 40) || (a.RECORD_ID == probCaseId && a.RECORD_TYPE == 21)) &&
							(a.DISPLAY_TYPE > 0) &&
							(a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
							a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png")
							|| a.FILE_NAME.ToLower().Contains(".bmp"))
						 orderby a.RECORD_TYPE, a.FILE_NAME
						 select new
						 {
							 AttachmentId = a.ATTACHMENT_ID,
							 FileName = a.FILE_NAME,
							 Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
							 Size = a.FILE_SIZE
						 }).ToList();

			if (files.Count > 0)
			{
				rptAttachments.DataSource = files;
				rptAttachments.DataBind();
			}

		}

		public string AttachmentIdToEncodedImage(decimal attachmentId)
		{
			//ATTACHMENT a = SQMDocumentMgr.GetAttachment(attachmentId);
			var entities = new PSsqmEntities();
			var attachmentData = (from f in entities.ATTACHMENT_FILE
								  where f.ATTACHMENT_ID == attachmentId
								  select f.ATTACHMENT_DATA).FirstOrDefault();

			//byte[] bytes = a.ATTACHMENT_FILE.ATTACHMENT_DATA;
			if (attachmentData.Count() > 0)
			{
				MemoryStream ms = new MemoryStream(attachmentData);
				return "data:image/jpg;base64," + Convert.ToBase64String(ms.ToArray());
			}
			else
			{
				return "";
			}
		}

		public string ImageUrlToBase64String(string url)
		{
			string returnString = "";

			WebClient client = new WebClient();
			//client.Headers["Accept-Encoding"] = "image/jpeg";
			//client.Headers["Accept-Encoding"] = "image/jpg";
			//client.Headers["Accept-Encoding"] = "image/gif";
			//client.Headers["Accept-Encoding"] = "image/png";

			byte[] bytes = client.DownloadData(url);
			MemoryStream ms = new MemoryStream(bytes);

			returnString = Convert.ToBase64String(ms.ToArray());

			//System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

			//using (MemoryStream stream = new MemoryStream())
			//{
			//	img.Save(stream, img.RawFormat);
			//	returnString = Convert.ToBase64String(stream.ToArray());
			//}

			return returnString;
		}



		public string GetFileHandlerUrl()
		{
			return baseAppUrl + "Shared/FileHandler.ashx?DOC=a&DOC_ID=";
		}
	}
}