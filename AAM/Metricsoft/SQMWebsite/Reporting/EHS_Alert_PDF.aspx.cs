using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SQM.Website.EHS
{
	public class AlertDataEHS
	{
		public string incidentDateTime;
		public string incidentLocation;
		public string incidentNumber;
		public string incidentType;

		public string incidentDescription;
		public List<string> incidentRootCause;
		public List<string> incidentContainment;
		public List<string> incidentCorrectiveActions;

		public List<byte[]> photoData;
		public List<string> photoCaptions;

		public AlertDataEHS()
		{
			incidentDateTime = "N/A";
			incidentLocation = "N/A";
			incidentNumber = "N/A";
			incidentType = "N/A";
			incidentDescription = "N/A";
			incidentRootCause = new List<string> { "N/A" };
			incidentContainment = new List<string> { "N/A" };
			incidentCorrectiveActions = new List<string> { "N/A" };
		}
	}

	public partial class EHS_Alert_PDF : System.Web.UI.Page
	{
		string baseApplicationUrl;

		protected void Page_Load(object sender, EventArgs e)
		{
			ShowPdf(BuildPdf());
		}

		private void ShowPdf(byte[] strS)
		{
			Response.ClearContent();
			Response.ClearHeaders();
			Response.ContentType = "application/pdf";
			Response.AddHeader("Content-Disposition", "attachment; filename=IncidentAlert-" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf");

			Response.BinaryWrite(strS);
			Response.End();
			Response.Flush();
			Response.Clear();
		}

		private byte[] BuildPdf()
		{
			AlertDataEHS pageData;

			baseApplicationUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

			if (Request.QueryString["pcid"] != null)
			{
				string query = Request.QueryString["pcid"];
				query = query.Replace(" ", "+");
				string pcid = EncryptionManager.Decrypt(query);
				pageData = PopulateByProblemCaseId(Convert.ToDecimal(pcid));
			}
			else if (Request.QueryString["iid"] != null)
			{
				string query = Request.QueryString["iid"];
				query = query.Replace(" ", "+");
				string iid = EncryptionManager.Decrypt(query);
				pageData = PopulateByIncidentId(Convert.ToDecimal(iid));
			}
			else
			{
				pageData = PopulateAlertDataTest();
			}

			string customerLogo = "";
			customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"].ToString();
			if (string.IsNullOrEmpty(customerLogo))
				customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();
			if (string.IsNullOrEmpty(customerLogo))
				customerLogo = "MetricsoftLogo.png";

			string logoUrl = baseApplicationUrl + "images/company/" + customerLogo;

			Font textFont = GetTextFont();
			Font headerFont = GetHeaderFont();

			BaseColor darkGrayColor = new BaseColor(0.25f, 0.25f, 0.25f);
			BaseColor lightGrayColor = new BaseColor(0.5f, 0.5f, 0.5f);
			BaseColor whiteColor = new BaseColor(1.0f, 1.0f, 1.0f);
			BaseColor blackColor = new BaseColor(0.0f, 0.0f, 0.0f);

			// Create new PDF document
			Document document = new Document(PageSize.A4, 35f, 35f, 35f, 35f);
			using (MemoryStream output = new MemoryStream())
			{

				PdfWriter.GetInstance(document, output);

				document.Open();

				//
				// Table 1 - Header
				//

				var table1 = new PdfPTable(new float[] { 162f, 378f });
				table1.TotalWidth = 540f;
				table1.LockedWidth = true;
				table1.DefaultCell.Border = 0;
				table1.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
				table1.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
				table1.SpacingAfter = 12f;

				iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
				img.ScaleToFit(162f, 81f);
				var imgCell = new PdfPCell() { Border = 0 };
				imgCell.AddElement(img);
				table1.AddCell(imgCell);

				var hdrFont = new Font(headerFont.BaseFont, 38, 0, darkGrayColor);
				table1.AddCell(new PdfPCell(new Phrase("EH&S INCIDENT ALERT", hdrFont))
				{
					HorizontalAlignment = Element.ALIGN_RIGHT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					Border = 0
				});


				//
				// Table 2 - Information block
				//

				var infoFont = new Font(headerFont.BaseFont, 14, 0, whiteColor);

				var table2 = new PdfPTable(new float[] { 240f, 300f });
				table2.TotalWidth = 540f;
				table2.LockedWidth = true;
				table2.SpacingAfter = 12f;

				table2.AddCell(new PdfPCell(new Phrase(pageData.incidentDateTime, infoFont)) { BackgroundColor = darkGrayColor, Padding = 10f, Border = 0 });
				table2.AddCell(new PdfPCell(new Phrase(pageData.incidentNumber, infoFont)) { BackgroundColor = darkGrayColor, Padding = 10f, HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 });
				table2.AddCell(new PdfPCell(new Phrase(pageData.incidentLocation, infoFont)) { BackgroundColor = darkGrayColor, Padding = 10f, PaddingTop = 0, Border = 0 });
				table2.AddCell(new PdfPCell(new Phrase(pageData.incidentType, infoFont)) { BackgroundColor = darkGrayColor, Padding = 10f, PaddingTop = 0, HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0 });
				table2.Complete = true;

				//
				// Table 3 - Description and details
				//

				var detailHdrFont = new Font(headerFont.BaseFont, 14, 0, lightGrayColor);
				var detailTxtFont = new Font(textFont.BaseFont, 12, 0, blackColor);

				var table3 = new PdfPTable(new float[] { 135f, 405f });
				table3.TotalWidth = 540f;
				table3.LockedWidth = true;
				table3.DefaultCell.SetLeading(40f, 40f);
				table3.SpacingAfter = 12f;
				PdfPCell cell;

				cell = new PdfPCell(new Paragraph("DESCRIPTION", detailHdrFont)) { Padding = 12f, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
				cell.SetLeading(17f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell(new Phrase(pageData.incidentDescription, detailTxtFont)) { Padding = 8f, PaddingBottom = 12f, Border = 0 };
				cell.SetLeading(20f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell(new Paragraph("ROOT CAUSE", detailHdrFont)) { Padding = 12f, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
				cell.SetLeading(17f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell() { Padding = 8f, PaddingBottom = 12f, Border = 0 };
				foreach (var lineItem in pageData.incidentRootCause)
				{
					cell.AddElement(new Paragraph(lineItem, detailTxtFont));
				}
				cell.SetLeading(20f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell(new Paragraph("CONTAINMENT", detailHdrFont)) { Padding = 12f, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
				cell.SetLeading(17f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell() { Padding = 8f, PaddingBottom = 12f, Border = 0 };
				foreach (var lineItem in pageData.incidentContainment)
				{
					cell.AddElement(new Paragraph(lineItem, detailTxtFont));
				}
				cell.SetLeading(20f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell(new Paragraph("CORRECTIVE ACTIONS", detailHdrFont)) { Padding = 12f, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
				cell.SetLeading(17f, 0f);
				table3.AddCell(cell);

				cell = new PdfPCell() { Padding = 8f, PaddingBottom = 12f, Border = 0 };
				foreach (var lineItem in pageData.incidentCorrectiveActions)
				{
					cell.AddElement(new Paragraph(lineItem, detailTxtFont));
				}
				cell.SetLeading(20f, 0f);
				table3.AddCell(cell);


				//
				// Table 4 - Photos
				//

				var table4 = new PdfPTable(new float[] { 180f, 180f, 180f });
				table4.TotalWidth = 540f;
				table4.LockedWidth = true;

				if (pageData.photoData != null && pageData.photoData.Count() > 0)
				{
					table4.AddCell(new PdfPCell(new Phrase("Photos", infoFont)) { BackgroundColor = darkGrayColor, Padding = 10f, Border = 0, Colspan = 3 });

					var captionFont = new Font(textFont.BaseFont, 11, 0, darkGrayColor);

					int i = 0;
					for (i = 0; i < pageData.photoData.Count; i++)
					{
						var photoCell = new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 };

						iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(pageData.photoData[i]);
						photo.ScaleToFit(176f, 132f);
						photoCell.AddElement(photo);

						photoCell.AddElement(new Phrase(pageData.photoCaptions[i], captionFont));

						table4.AddCell(photoCell);
					}
					// pad remaining cells in row or else table will be corrupt
					int currentCol = i % 3;
					for (int j = 0; j < 3 - currentCol; j++)
						table4.AddCell(new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 });
				}

				document.Add(table1);
				document.Add(table2);
				document.Add(table3);
				document.Add(table4);

				document.Close();

				return output.ToArray();
			}
		}

		AlertDataEHS PopulateAlertDataTest()
		{
			AlertDataEHS d = new AlertDataEHS();

			d.incidentDateTime = "Tuesday, Mar 19, 2014 4:00 PM";
			d.incidentLocation = "Location: Clamart";
			d.incidentNumber = "Incident #: 135";
			d.incidentType = "Near Miss";

			d.incidentDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam sit amet lacinia sapien. Ut nec luctus leo. Mauris a lacus tellus. Nunc quis ultricies eros, ac molestie justo. Ut pellentesque libero commodo tempus aliquet. Vivamus molestie venenatis elit sed imperdiet.";
			d.incidentRootCause = new List<string> { "The root cause for the incident described above." };
			d.incidentContainment = new List<string> { "Here is a description of the various steps we used to contain the incident that is listed." };
			d.incidentCorrectiveActions = new List<string> { "Step 1: stop this from happening again.  Step 2: Make sure it never happens at a later date." };

			return d;
		}


		AlertDataEHS PopulateByProblemCaseId(decimal pcid)
		{
			AlertDataEHS d = new AlertDataEHS();
			var entities = new PSsqmEntities();

			PROB_CASE probCase = ProblemCase.LookupCase(entities, pcid);

			if (probCase != null)
			{
				List<INCIDENT> incidentList = ProblemCase.LookupProbIncidentList(entities, probCase);

				if (incidentList.Count > 0)
				{
					var incident = incidentList[0];

					string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)incident.DETECT_PLANT_ID);
					d.incidentLocation = String.Format("Location: {0}", plantName);
					d.incidentNumber = String.Format("Incident #: {0}", incident.INCIDENT_ID);

					string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(incident.INCIDENT_ID);
					decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(incident.INCIDENT_ID);
					decimal companyId = incident.DETECT_COMPANY_ID;
					var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
					questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

					d.incidentDateTime = incidentList[0].INCIDENT_DT.ToLongDateString();

					var timeQuestion = questions.FirstOrDefault(q => q.QuestionId == 5);
					if (timeQuestion != null)
					{
						string timeAnswer = (from a in entities.INCIDENT_ANSWER
											 where
												 a.INCIDENT_ID == incident.INCIDENT_ID &&
												 a.INCIDENT_QUESTION_ID == 5
											 select a.ANSWER_VALUE).FirstOrDefault();

						if (!string.IsNullOrEmpty(timeAnswer))
							d.incidentDateTime += " " + Convert.ToDateTime(timeAnswer).ToShortTimeString();
					}

					d.incidentType = incidentType;
					d.incidentDescription = probCase.DESC_LONG;


					// Root Cause(s)

					List<PROB_CAUSE_STEP> probCauses = (from i in entities.PROB_CAUSE_STEP
														where
															i.PROBCASE_ID == pcid &&
															i.IS_ROOTCAUSE == true
														select i).ToList();

					if (probCauses.Count > 0)
						d.incidentRootCause = (from rc in probCauses select rc.HOW_CONFIRMED).ToList();

					// Containment

					var containment = (from i in entities.PROB_CONTAIN
									   where i.PROBCASE_ID == pcid
									   select new
									   {
										   Recommendation = i.CONTAINMENT_DESC,
										   Action = i.INITIAL_ACTION,
										   Results = i.INITIAL_RESULTS
									   }).FirstOrDefault();

					var actions = (from i in entities.PROB_CONTAIN_ACTION
								   where i.PROBCASE_ID == pcid
								   select i.ACTION_ITEM).ToList();

					if (containment != null)
					{
						d.incidentContainment = new List<string>();

						//if (!string.IsNullOrEmpty(containment.Recommendation))
						//{
						//	d.incidentContainment.Add("RECOMMENDATION: " + containment.Recommendation);
						//	d.incidentContainment.Add(" ");
						//}

						if (actions != null)
						{
							string strActions;
							int i = 1;
							foreach (var actionItem in actions)
							{
								strActions = i++ + ") " + actionItem;
								d.incidentContainment.Add(strActions);
							}
						}

						if (!string.IsNullOrEmpty(containment.Results))
						{
							d.incidentContainment.Add(" ");
							d.incidentContainment.Add("RESULTS: " + containment.Results);
						}
					}

					// Corrective Actions

					var correctiveActions = (from i in entities.PROB_CAUSE_ACTION
											 where i.PROBCASE_ID == pcid
											 select i.ACTION_DESC).ToList();

					if (correctiveActions.Count() > 0)
					{
						d.incidentCorrectiveActions = new List<string>();
						int i = 1;
						foreach (var ca in correctiveActions)
						{
							d.incidentCorrectiveActions.Add(i++ + ") " + ca);
						}
					}

					// Photos

					var files = (from a in entities.ATTACHMENT
								 where
									((a.RECORD_ID == incident.INCIDENT_ID && a.RECORD_TYPE == 40) || (a.RECORD_ID == pcid && a.RECORD_TYPE == 21)) &&
									(a.DISPLAY_TYPE > 0) &&
									(a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
									a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
									a.FILE_NAME.ToLower().Contains(".bmp"))
								 orderby a.RECORD_TYPE, a.FILE_NAME
								 select new
								 {
									 Data = (from f in entities.ATTACHMENT_FILE where f.ATTACHMENT_ID == a.ATTACHMENT_ID select f.ATTACHMENT_DATA).FirstOrDefault(),
									 Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
								 }).ToList();

					if (files.Count > 0)
					{
						d.photoData = new List<byte[]>();
						d.photoCaptions = new List<string>();

						foreach (var f in files)
						{
							d.photoData.Add(f.Data);
							d.photoCaptions.Add(f.Description);
						}
					}
				}
			}

			return d;
		}


		AlertDataEHS PopulateByIncidentId(decimal iid)
		{
			AlertDataEHS d = new AlertDataEHS();
			var entities = new PSsqmEntities();

			var incident = EHSIncidentMgr.SelectIncidentById(entities, iid);

			if (incident != null)
			{
				string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)incident.DETECT_PLANT_ID);
				d.incidentLocation = String.Format("Location: {0}", plantName);
				d.incidentNumber = String.Format("Incident #: {0}", iid);

				string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(iid);
				decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(iid);
				decimal companyId = incident.DETECT_COMPANY_ID;
				var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
				questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

				// Date/Time

				d.incidentDateTime = incident.INCIDENT_DT.ToLongDateString();

				var timeQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.TimeOfDay);
				if (timeQuestion != null)
				{
					string timeAnswer = (from a in entities.INCIDENT_ANSWER
										 where
											 a.INCIDENT_ID == incident.INCIDENT_ID &&
											 a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay
										 select a.ANSWER_VALUE).FirstOrDefault();

					if (!string.IsNullOrEmpty(timeAnswer))
						d.incidentDateTime += " " + Convert.ToDateTime(timeAnswer).ToShortTimeString();
				}

				// Incident Type

				d.incidentType = incidentType;

				// Description

				d.incidentDescription = incident.DESCRIPTION;

				// Root Cause(s)

				var rootCauseQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.RootCause);
				if (rootCauseQuestion != null)
				{
					string rootCauseAnswer = (from a in entities.INCIDENT_ANSWER
											  where
												  a.INCIDENT_ID == iid &&
												  a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.RootCause
											  select a.ANSWER_VALUE).FirstOrDefault();

					if (!string.IsNullOrEmpty(rootCauseAnswer))
						d.incidentRootCause = new List<string> { rootCauseAnswer };
				}


				// Containment

				var containmentQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.Containment);
				if (containmentQuestion != null)
				{
					string containmentAnswer = (from a in entities.INCIDENT_ANSWER
												where
													a.INCIDENT_ID == iid &&
													a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Containment
												select a.ANSWER_VALUE).FirstOrDefault();

					if (!string.IsNullOrEmpty(containmentAnswer))
						d.incidentContainment = new List<string> { containmentAnswer };
				}

				// Corrective Actions

				var correctiveQuestion = questions.FirstOrDefault(q => q.QuestionId == (decimal)EHSQuestionId.CorrectiveActions);
				if (correctiveQuestion != null)
				{
					string correctiveAnswer = (from a in entities.INCIDENT_ANSWER
											   where
												   a.INCIDENT_ID == iid &&
												   a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.CorrectiveActions
											   select a.ANSWER_VALUE).FirstOrDefault();

					if (!string.IsNullOrEmpty(correctiveAnswer))
						d.incidentCorrectiveActions = new List<string> { correctiveAnswer };
				}


				var files = (from a in entities.ATTACHMENT
							 where
								(a.RECORD_ID == iid && a.RECORD_TYPE == 40 && a.DISPLAY_TYPE > 0) &&
								(a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
								a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
								a.FILE_NAME.ToLower().Contains(".bmp"))
							 orderby a.RECORD_TYPE, a.FILE_NAME
							 select new
							 {
								 Data = (from f in entities.ATTACHMENT_FILE where f.ATTACHMENT_ID == a.ATTACHMENT_ID select f.ATTACHMENT_DATA).FirstOrDefault(),
								 Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
							 }).ToList();


				if (files.Count > 0)
				{
					d.photoData = new List<byte[]>();
					d.photoCaptions = new List<string>();

					foreach (var f in files)
					{
						d.photoData.Add(f.Data);
						d.photoCaptions.Add(f.Description);
					}
				}
			}

			return d;
		}


		public iTextSharp.text.Font GetHeaderFont()
		{
			var fontName = "Oswald-Regular";
			if (!FontFactory.IsRegistered(fontName))
			{
				var fontPath = Server.MapPath("~") + "images\\fonts\\Oswald-Regular.ttf";
				FontFactory.Register(fontPath);
			}
			return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		}

		public iTextSharp.text.Font GetTextFont()
		{
			var fontName = "Ubuntu";
			if (!FontFactory.IsRegistered(fontName))
			{
				var fontPath = Server.MapPath("~") + "images\\fonts\\Ubuntu-Regular.ttf";
				FontFactory.Register(fontPath);
			}
			return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		}

	}
}