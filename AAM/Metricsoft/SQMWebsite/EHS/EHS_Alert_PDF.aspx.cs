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
	public class AlertData
	{
		public string incidentDate;
		public string incidentTime;
		public string incidentLocation;
		public string incidentDept;
		public string incidentNumber;
		public string incidentType;
		public PERSON detectPerson;
		public PERSON involvedPerson;
		public PERSON supervisorPerson;
		public string incidentDescription;
		public List<string> incidentRootCause;
		public List<string> incidentContainment;
		public List<string> incidentCorrectiveActions;

		public List<byte[]> photoData;
		public List<string> photoCaptions;

		public List<INCIDENT_ANSWER> answerList;
		public List<INCFORM_CONTAIN> containList;
		public List<INCFORM_ROOT5Y> root5YList;
		public List<TASK_STATUS> actionList;

		public AlertData()
		{
			incidentDate = "N/A";
			incidentTime = "N/A";
			incidentLocation = "N/A";
			incidentDept = "N/A";
			incidentNumber = "N/A";
			incidentType = "N/A";
			incidentDescription = "N/A";
			detectPerson = null;
			involvedPerson = null;
			supervisorPerson = null;
			incidentRootCause = new List<string> { "N/A" };
			incidentContainment = new List<string> { "N/A" };
			incidentCorrectiveActions = new List<string> { "N/A" };
			answerList = new List<INCIDENT_ANSWER>();
			containList = new List<INCFORM_CONTAIN>();
			root5YList = new List<INCFORM_ROOT5Y>();
			actionList = new List<TASK_STATUS>();
		}
	}

	public partial class EHS_Alert_PDF : System.Web.UI.Page
	{
		public List<XLAT> reportXLAT
		{
			get { return ViewState["ReportXLAT"] == null ? null : (List<XLAT>)ViewState["ReportXLAT"]; }
			set { ViewState["ReportXLAT"] = value; }
		}
		public Font detailHdrFont
		{
			get { return ViewState["DetailHdrFont"] == null ? null : (Font)ViewState["DetailHdrFont"]; }
			set { ViewState["DetailHdrFont"] = value; }
		}
		public Font infoFont
		{
			get { return ViewState["InfoFont"] == null ? null : (Font)ViewState["InfoFont"]; }
			set { ViewState["InfoFont"] = value; }
		}
		public Font detailTxtFont
		{
			get { return ViewState["DetailTxtFont"] == null ? null : (Font)ViewState["DetailTxtFont"]; }
			set { ViewState["DetailTxtFont"] = value; }
		}
		public Font detailTxtItalicFont
		{
			get { return ViewState["DetailTxtItalicFont"] == null ? null : (Font)ViewState["DetailTxtItalicFont"]; }
			set { ViewState["DetailTxtItalicFont"] = value; }
		}
		public Font labelTxtFont
		{
			get { return ViewState["LabelTxtFont"] == null ? null : (Font)ViewState["LabelTxtFont"]; }
			set { ViewState["LabelTxtFont"] = value; }
		}
		public Font colHdrFont
		{
			get { return ViewState["ColHdrFont"] == null ? null : (Font)ViewState["ColHdrFont"]; }
			set { ViewState["ColHdrFont"] = value; }
		}

		string baseApplicationUrl;

		protected void Page_Load(object sender, EventArgs e)
		{
			ShowPdf(BuildPdf());
		}

		protected  XLAT GetXLAT(string xlatGroup, string xlatCode)
		{
			XLAT menu = reportXLAT.Where(l => l.XLAT_GROUP == xlatGroup && l.XLAT_CODE == xlatCode).FirstOrDefault();
			if (menu == null)
			{
				menu = new XLAT();
			}
			return menu;
		}

		private void ShowPdf(byte[] strS)
		{
			Response.ClearContent();
			Response.ClearHeaders();
			Response.ContentType = "application/pdf";
			Response.AddHeader("Content-Disposition", "attachment; filename=Incident-5PhaseReport-" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf");

			Response.BinaryWrite(strS);
			Response.End();
			Response.Flush();
			Response.Clear();
		}

		private byte[] BuildPdf()
		{
			reportXLAT = SQMBasePage.SelectXLATList(new string[2] { "HS_5PHASE", "SHIFT" });

			AlertData pageData;
			
			baseApplicationUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

			if (Request.QueryString["iid"] != null)
			{
				string query = Request.QueryString["iid"];
				query = query.Replace(" ", "+");
				string iid = EncryptionManager.Decrypt(query);
				pageData = PopulateByIncidentId(Convert.ToDecimal(iid));
			}
			else
			{
				return null;
			}

			string customerLogo = "";
			customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"].ToString();
			if (string.IsNullOrEmpty(customerLogo))
				customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();
			if (string.IsNullOrEmpty(customerLogo))
				customerLogo = "MetricsoftLogo.png";
			
			string logoUrl = baseApplicationUrl + "images/company/" + customerLogo;
			
			BaseColor darkGrayColor = new BaseColor(0.25f, 0.25f, 0.25f);
			BaseColor lightGrayColor = new BaseColor(0.5f, 0.5f, 0.5f);
			BaseColor whiteColor = new BaseColor(1.0f, 1.0f, 1.0f);
			BaseColor blackColor = new BaseColor(0.0f, 0.0f, 0.0f);

			Font textFont = GetTextFont();
			Font headerFont = GetHeaderFont();
			Font labelFont = GetLabelFont();
			Font colHeaderFont = GetTextFont();
			Font textItalicFont = GetTextFont();

			detailHdrFont = new Font(headerFont.BaseFont, 13, 0, lightGrayColor);
			detailTxtFont = new Font(textFont.BaseFont, 10, 0, blackColor);
			labelTxtFont = new Font(labelFont.BaseFont, 12, 0, blackColor);
			colHdrFont = new Font(colHeaderFont.BaseFont, 10,  iTextSharp.text.Font.UNDERLINE, blackColor);
			detailTxtItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);

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
				table1.SpacingAfter = 10f;

				iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
				img.ScaleToFit(102f, 51f);
				var imgCell = new PdfPCell() { Border = 0 };
				imgCell.AddElement(img);
				table1.AddCell(imgCell);

				var hdrFont = new Font(headerFont.BaseFont, 24, 0, darkGrayColor);
				table1.AddCell(new PdfPCell(new Phrase(GetXLAT("HS_5PHASE", "TITLE").DESCRIPTION, hdrFont))
				{
					HorizontalAlignment = Element.ALIGN_RIGHT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					Border = 0
				});
				

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
				document.Add(IDSection(pageData));
				document.Add(HeaderSection(pageData));
				document.Add(IncidentSection(pageData));
				document.Add(ContainmentSection(pageData));
				document.Add(CauseSection(pageData));
				document.Add(ActionSection(pageData));
				document.Add(ReviewSection(pageData));
				document.Add(table4);

				document.Close();

				return output.ToArray();
			}
		}

		PdfPTable IDSection(AlertData pageData)
		{
			PdfPTable tableIncident = new PdfPTable(new float[] { 90f, 450f,});
			tableIncident.TotalWidth = 540f;
			tableIncident.LockedWidth = true;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 6, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "INCIDENTTYPE").DESCRIPTION_SHORT + ":", detailHdrFont));
			tableIncident.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 4, Border = 0 };
			cell.AddElement(new Paragraph(string.Format(pageData.incidentType + "   ( # {0} )", pageData.incidentNumber), labelTxtFont));
			tableIncident.AddCell(cell);

			return tableIncident;
		}

		PdfPTable HeaderSection(AlertData pageData)
		{
			PdfPTable tableHeader = new PdfPTable(new float[] { 220f, 160f, 160f });
			tableHeader.TotalWidth = 540f;
			tableHeader.LockedWidth = true;
			PdfPCell cell;
			INCIDENT_ANSWER answer = null;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(String.Format(GetXLAT("HS_5PHASE", "PLANT").DESCRIPTION_SHORT + ":  {0}",pageData.incidentLocation), detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.Colspan = 2;
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(GetXLAT("HS_5PHASE", "LOCATION").DESCRIPTION_SHORT + ":  {0}", pageData.incidentDept), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(String.Format("Date" + ":  {0}", pageData.incidentDate), detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(String.Format("Time" + ":  {0}", pageData.incidentTime), detailTxtFont));
			tableHeader.AddCell(cell);
			cell = FormatHeaderCell(pageData, (decimal)EHSQuestionId.Shift);
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(String.Format(GetXLAT("HS_5PHASE", "SUPERVISOR").DESCRIPTION_SHORT + ":  {0}", pageData.supervisorPerson == null ? "" : SQMModelMgr.FormatPersonListItem(pageData.supervisorPerson)), detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.Colspan = 2;
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(String.Format(GetXLAT("HS_5PHASE", "CONTACT_NO").DESCRIPTION_SHORT + ":  {0}", pageData.supervisorPerson == null ? "" : pageData.supervisorPerson.PHONE), detailTxtFont));
			tableHeader.AddCell(cell);
			return tableHeader;
		}

		PdfPCell FormatHeaderCell(AlertData pageData, decimal questionID)
		{
			PdfPCell cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			if (questionID == 0)
			{
				cell.AddElement(new Paragraph(" ", detailTxtFont));
			}
			else
			{
				INCIDENT_ANSWER answer = pageData.answerList.Where(a => a.INCIDENT_QUESTION_ID == questionID).FirstOrDefault();
				if (answer != null)
				{
					cell.AddElement(new Paragraph(String.Format(answer.ORIGINAL_QUESTION_TEXT + ": {0}", answer.ANSWER_VALUE), detailTxtFont));
				}
				else
				{
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
			}

			return cell;
		}

		PdfPTable IncidentSection(AlertData pageData)
		{
			PdfPTable tableIncident = new PdfPTable(new float[] { 540f });
			tableIncident.TotalWidth = 540f;
			tableIncident.LockedWidth = true;
			tableIncident.SpacingBefore = 5f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph("Description:", detailHdrFont));
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(pageData.incidentDescription, detailTxtFont));
			tableIncident.AddCell(cell);

			return tableIncident;
		}

		PdfPTable ContainmentSection(AlertData pageData)
		{
			PdfPTable tableContain = new PdfPTable(new float[] { 420f, 110f, 110f });
			tableContain.TotalWidth = 540f;
			tableContain.LockedWidth = true;
			tableContain.SpacingBefore = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.Colspan = 3;
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "CONTAINMENT").DESCRIPTION, detailHdrFont));
			tableContain.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "CONTAINMENT").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "RESPONSIBLE").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ACTION_DT").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);

			foreach (var cc in pageData.containList)
			{
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(cc.ITEM_DESCRIPTION, detailTxtFont));
				tableContain.AddCell(cell);
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(cc.ASSIGNED_PERSON, detailTxtFont));
				tableContain.AddCell(cell);
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				if (cc.COMPLETION_DATE.HasValue)
				{
					cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)cc.COMPLETION_DATE, "d", false), detailTxtFont));
				}
				else
				{
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
				tableContain.AddCell(cell);
			}

			return tableContain;
		}

		PdfPTable CauseSection(AlertData pageData)
		{
			PdfPTable tableCause = new PdfPTable(new float[] { 540f });
			tableCause.TotalWidth = 540f;
			tableCause.LockedWidth = true;
			tableCause.SpacingBefore = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.Colspan = 3;
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ROOTCAUSE").DESCRIPTION, detailHdrFont));
			tableCause.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(pageData.incidentDescription.Substring(0, Math.Min(120,pageData.incidentDescription.Length)), detailTxtItalicFont));
			tableCause.AddCell(cell);

			//cell = new PdfPCell() { Padding = 1f, PaddingTop = 4f, Border = 0 };
			//cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ROOTCAUSE").DESCRIPTION_SHORT, colHdrFont));
			//tableCause.AddCell(cell);

			foreach (var rc in pageData.root5YList)
			{
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(String.Format(GetXLAT("HS_5PHASE", "ROOTCAUSE").DESCRIPTION_SHORT + "{0}", rc.ITEM_DESCRIPTION), detailTxtFont));
				tableCause.AddCell(cell);
			}

			return tableCause;
		}

		PdfPTable ActionSection(AlertData pageData)
		{
			PdfPTable tableAction = new PdfPTable(new float[] { 420f, 110f, 110f });
			tableAction.TotalWidth = 540f;
			tableAction.LockedWidth = true;
			tableAction.SpacingBefore = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.Colspan = 3;
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ACTION").DESCRIPTION, detailHdrFont));
			tableAction.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ACTION").DESCRIPTION_SHORT, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "RESPONSIBLE").DESCRIPTION_SHORT, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "ACTION_DT").DESCRIPTION_SHORT, colHdrFont));
			tableAction.AddCell(cell);

			foreach (var ac in pageData.actionList)
			{
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(ac.DESCRIPTION, detailTxtFont));
				tableAction.AddCell(cell);
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(ac.COMMENTS, detailTxtFont));
				tableAction.AddCell(cell);
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				if (ac.COMPLETE_DT.HasValue)
				{
					cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)ac.COMPLETE_DT, "d", false), detailTxtFont));
				}
				else
				{
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
				tableAction.AddCell(cell);
			}

			return tableAction;
		}

		PdfPTable ReviewSection(AlertData pageData)
		{
			PdfPTable tableReview = new PdfPTable(new float[] { 170f, 210f, 160f });
			tableReview.TotalWidth = 540f;
			tableReview.LockedWidth = true;
			tableReview.SpacingBefore = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 4f, Border = 0 };
			cell.Colspan = 3;
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "REVIEW").DESCRIPTION_SHORT, detailHdrFont));
			tableReview.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 4f, Border = 0 };
			cell.Colspan = 3;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "REVIEW").DESCRIPTION, detailTxtItalicFont));
			tableReview.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "REVIEW_1").DESCRIPTION_SHORT, detailTxtFont));
			tableReview.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph("joe safety", detailTxtFont));
			tableReview.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft =  cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(string.Format("Dated:  {0}", "1/1/2015"), detailTxtFont));
			tableReview.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthBottom = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(GetXLAT("HS_5PHASE", "REVIEW_2").DESCRIPTION_SHORT, detailTxtFont));
			tableReview.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthBottom = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph("joe manager", detailTxtFont));
			tableReview.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthBottom = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(string.Format("Dated:  {0}","1/1/2015"), detailTxtFont));
			tableReview.AddCell(cell);

			return tableReview;
		}

		AlertData PopulateByIncidentId(decimal iid)
		{
			AlertData d = new AlertData();
			var entities = new PSsqmEntities();

			var incident = EHSIncidentMgr.SelectIncidentById(entities, iid);

			if (incident != null)
			{
				string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)incident.DETECT_PLANT_ID);
				d.incidentLocation = plantName;
				d.incidentNumber = iid.ToString();

				string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(iid);
				decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(iid);
				decimal companyId = incident.DETECT_COMPANY_ID;
				var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
				questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

				d.answerList = EHSIncidentMgr.GetIncidentAnswerList(incident.INCIDENT_ID);
				INCIDENT_ANSWER answer = null;

				// Date/Time

				d.incidentDate = incident.INCIDENT_DT.ToShortDateString();
				if ((answer = d.answerList.Where(a=> a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay).SingleOrDefault()) != null)
				{
					if (!string.IsNullOrEmpty(answer.ANSWER_VALUE))
						d.incidentTime = Convert.ToDateTime(answer.ANSWER_VALUE).ToShortTimeString();
				}

				if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Department).SingleOrDefault()) != null)
				{
					d.incidentDept = answer.ANSWER_VALUE;
				}

				// Incident Type

				d.incidentType = incidentType;

				// Description

				d.incidentDescription = incident.DESCRIPTION;

				d.detectPerson = SQMModelMgr.LookupPerson(entities, (decimal)incident.CREATE_PERSON, "", false);
				if (d.detectPerson != null)
				{
					d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.detectPerson.SUPV_EMP_ID);
				}
				if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
				{
					answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Shift).SingleOrDefault();
					answer.ANSWER_VALUE = GetXLAT("SHIFT", answer.ANSWER_VALUE).DESCRIPTION;

					answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.InvolvedPerson).SingleOrDefault();
					if (answer != null && !string.IsNullOrEmpty(answer.ANSWER_VALUE))
					{
						d.involvedPerson = SQMModelMgr.LookupPerson(entities, Convert.ToDecimal(answer.ANSWER_VALUE), "", false);
						if (d.involvedPerson != null)
							d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.involvedPerson.SUPV_EMP_ID);
					}
					else
					{
						d.involvedPerson = SQMModelMgr.LookupPerson(entities, (decimal)incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID, "", false);
						if (d.involvedPerson != null)
							d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.involvedPerson.SUPV_EMP_ID);
					}
				}

				// Containment
				d.incidentContainment = new List<string>();
				foreach (INCFORM_CONTAIN cc in EHSIncidentMgr.GetContainmentList(iid))
				{
					d.incidentContainment.Add(cc.ITEM_DESCRIPTION);
					if (cc.ASSIGNED_PERSON_ID.HasValue)
					{
						cc.ASSIGNED_PERSON = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)cc.ASSIGNED_PERSON_ID, ""));
					}
					d.containList.Add(cc);
				}

				// Root Cause(s)
				d.root5YList = EHSIncidentMgr.GetRootCauseList(iid).Where(l=> !string.IsNullOrEmpty(l.ITEM_DESCRIPTION)).ToList();

				// Corrective Actions
				d.incidentCorrectiveActions = new List<string>();
				foreach (TASK_STATUS ac in EHSIncidentMgr.GetCorrectiveActionList(iid))
				{
					d.incidentCorrectiveActions.Add(ac.DESCRIPTION);
					if (ac.RESPONSIBLE_ID.HasValue)
					{
						ac.COMMENTS = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)ac.RESPONSIBLE_ID, ""));
					}
					d.actionList.Add(ac);
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
				var fontPath = Server.MapPath("~") +"images\\fonts\\Oswald-Regular.ttf";
				FontFactory.Register(fontPath);
			}
			return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		}

		public iTextSharp.text.Font GetLabelFont()
		{
			var fontName = "Ubuntu";
			if (!FontFactory.IsRegistered(fontName))
			{
				var fontPath = Server.MapPath("~") + "images\\fonts\\Ubuntu-Regular.ttf";
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