using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SQM.Website.Reports
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
		public string jobDescription;
		public string jobTenure;
		public string employeeType;
		public string injuryType;
		public string bodyPart;
		public string specificBodyPart;
		public string severity;

		public List<byte[]> photoData;
		public List<string> photoCaptions;

		public INCIDENT incident;
		public List<INCIDENT_ANSWER> answerList;
		public List<INCFORM_CONTAIN> containList;
		public List<INCFORM_ROOT5Y> root5YList;
		public INCFORM_CAUSATION causation;
		public INCFORM_ALERT incidentAlert;
		public List<IncidentAlertItem> alertList;
		public List<TASK_STATUS> actionList;
		public List<EHSIncidentApproval> approvalList;

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
			employeeType = "";
			jobDescription = "";
			jobTenure = "";
			injuryType = "";
			bodyPart = "";
			specificBodyPart = "";
			severity = "N/A";
			incident = null;
			answerList = new List<INCIDENT_ANSWER>();
			containList = new List<INCFORM_CONTAIN>();
			root5YList = new List<INCFORM_ROOT5Y>();
			causation = null;
			actionList = new List<TASK_STATUS>();
			approvalList = new List<EHSIncidentApproval>();
			incidentAlert = new INCFORM_ALERT();
			alertList = new List<IncidentAlertItem>();
		}
	}

	public partial class EHS_12HourReport : System.Web.UI.Page
	{
		public List<XLAT> reportXLAT
		{
			get { return ViewState["ReportXLAT"] == null ? null : (List<XLAT>)ViewState["ReportXLAT"]; }
			set { ViewState["ReportXLAT"] = value; }
		}

		public iTextSharp.text.Font detailHdrFont;
		public iTextSharp.text.Font detailLblFont;
		public iTextSharp.text.Font infoFont;
		public iTextSharp.text.Font detailTxtFont;
		public iTextSharp.text.Font detailTxtBoldFont;
		public iTextSharp.text.Font detailTxtItalicFont;
		public iTextSharp.text.Font labelTxtFont;
		public iTextSharp.text.Font colHdrFont;
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
			Response.AddHeader("Content-Disposition", "attachment; filename=Incident-12HourReport-" + SessionManager.UserContext.LocalTime.ToString("yyyy-MM-dd") + ".pdf");

			Response.BinaryWrite(strS);
			Response.End();
			Response.Flush();
			Response.Clear();
		}

		private byte[] BuildPdf()
		{
			reportXLAT = SQMBasePage.SelectXLATList(new string[10] { "HS_5PHASE", "HS_L2REPORT", "TRUEFALSE", "SHIFT", "INJURY_TENURE", "INJURY_CAUSE", "INJURY_PART", "INJURY_TYPE","INCIDENT_SEVERITY", "INCIDENT_APPROVALS" }, 1);

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
			if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"] != null)
				customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"].ToString();
			if (string.IsNullOrEmpty(customerLogo))
			{
				if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"] != null)
					customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();
			}
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
			colHdrFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.UNDERLINE+iTextSharp.text.Font.BOLD, blackColor);
			detailTxtItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
			detailTxtBoldFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD, blackColor);

			// Create new PDF document
			Document document = new Document(PageSize.A4, 35f, 35f, 35f, 35f);
			using (MemoryStream output = new MemoryStream())
			{

				PdfWriter.GetInstance(document, output);

				try
				{
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
					table1.SpacingAfter = 5f;

					iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
					img.ScaleToFit(102f, 51f);
					var imgCell = new PdfPCell() { Border = 0 };
					imgCell.AddElement(img);
					table1.AddCell(imgCell);
					PdfPCell cell = null;
					var hdrFont = new Font(headerFont.BaseFont, 18, 0, darkGrayColor);

					cell = new PdfPCell { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
					//cell.AddElement(new Paragraph(WebSiteCommon.SplitString(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TITLE").DESCRIPTION, ',').ElementAt(0), hdrFont));
					//cell.AddElement(new Paragraph(WebSiteCommon.SplitString(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TITLE").DESCRIPTION, ',').ElementAt(1), hdrFont));
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TITLE").DESCRIPTION, hdrFont));
                    //cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TITLE").DESCRIPTION, hdrFont));
                    table1.AddCell(cell);

					var versionFont = new Font(textItalicFont.BaseFont, 8, 0, darkGrayColor);
					cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2, Border = 0 };
					//cell.AddElement(new Paragraph(WebSiteCommon.SplitString(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "VERSION").DESCRIPTION, ',').ElementAt(0), versionFont));
					//cell.AddElement(new Paragraph(WebSiteCommon.SplitString(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "VERSION").DESCRIPTION, ',').ElementAt(1), versionFont));
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "VERSION").DESCRIPTION, versionFont));
                    //cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "VERSION").DESCRIPTION, versionFont));
                    table1.AddCell(cell);
					cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2, Border = 0 };
					cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INSTRUCTION").DESCRIPTION, labelTxtFont));
					table1.AddCell(cell);

					cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2, Border = 0 };
					cell.Colspan = 2;
					cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "PLANT").DESCRIPTION_SHORT + ":  {0}", pageData.incidentLocation), labelTxtFont));
					table1.AddCell(cell);

					//
					// Table 4 - Photos
					//

					var table4 = new PdfPTable(new float[] { 540f, }); //new PdfPTable(new float[] { 180f, 180f, 180f });
					table4.TotalWidth = 540f;
					table4.LockedWidth = true;

					try
					{
						if (pageData.photoData != null && pageData.photoData.Count() > 0)
						{
							table4.AddCell(new PdfPCell(new Phrase("Photos", detailHdrFont)) { Padding = 5f, Border = 0, Colspan = 3 });
							table4.SpacingBefore = 5f;
							var captionFont = new Font(textFont.BaseFont, 11, 0, darkGrayColor);

							int i = 0;
							for (i = 0; i < pageData.photoData.Count; i++)
							{
								var photoCell = new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 };

								iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(pageData.photoData[i]);
								//photo.ScaleToFit(176f, 132f);
								//photo.ScaleToFit(264f, 198f);
								photoCell.AddElement(photo);

								photoCell.AddElement(new Phrase(pageData.photoCaptions[i], captionFont));

								table4.AddCell(photoCell);
							}
							// pad remaining cells in row or else table will be corrupt
							int currentCol = i % 3;
							for (int j = 0; j < 3 - currentCol; j++)
								table4.AddCell(new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 });
						}
					}
					catch { }

					document.Add(table1);
					document.Add(ReviewSection(pageData));
					document.Add(IDSection(pageData));
					document.Add(HeaderSection(pageData));
					document.Add(IncidentSection(pageData));
					document.Add(ContainmentSection(pageData));
					if (pageData.incident.INCFORM_INJURYILLNESS != null)
					{
						document.Add(ProcedureSection(pageData));
					}

					document.Add(table4);	// attachments

					document.Close();
				}
				catch(Exception Ex)
				{
                    Ex.Message.ToString();
				}

				return output.ToArray();
			}
		}

		PdfPTable IDSection(AlertData pageData)
		{
			PdfPTable tableIncident = new PdfPTable(new float[] { 540f, });
			tableIncident.TotalWidth = 540f;
			tableIncident.LockedWidth = true;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INCIDENT_OCCUR").DESCRIPTION_SHORT, detailHdrFont));
			cell.AddElement(new Paragraph(string.Format(pageData.incidentType + " - Incident ID: {0}", pageData.incidentNumber), detailTxtFont));
			tableIncident.AddCell(cell);

			return tableIncident;
		}

		PdfPTable HeaderSection(AlertData pageData)
		{
			PdfPTable tableHeader = new PdfPTable(new float[] { 180f, 180f, 180f });
			tableHeader.TotalWidth = 540f;
			tableHeader.LockedWidth = true;
			tableHeader.SpacingAfter = 5f;
			PdfPCell cell;
			INCIDENT_ANSWER answer = null;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			//cell.AddElement(new Paragraph(String.Format("Date" + ":  {0}", pageData.incidentDate), detailTxtFont));
			cell.AddElement(new Paragraph("Date", detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.incidentDate, detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph("Time", detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.incidentTime, detailTxtFont));
			tableHeader.AddCell(cell);
			cell = FormatHeaderCell(pageData, (decimal)EHSQuestionId.Shift);
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.Colspan = 3;
			cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "LOCATION").DESCRIPTION_SHORT, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.incidentDept, detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthLeft = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "EMPLOYEETYPE").DESCRIPTION_SHORT, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.employeeType, detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "JOBCODE").DESCRIPTION_SHORT, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.jobDescription, detailTxtFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INJURY_TENURE").DESCRIPTION_SHORT, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.jobTenure, detailTxtFont));
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
					cell.AddElement(new Paragraph(answer.ORIGINAL_QUESTION_TEXT, detailTxtBoldFont));
					cell.AddElement(new Paragraph(answer.ANSWER_VALUE, detailTxtFont));
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
			PdfPTable tableIncident = new PdfPTable(new float[] { 180f, 180f, 180f });
			tableIncident.TotalWidth = 540f;
			tableIncident.LockedWidth = true;
			tableIncident.SpacingAfter = 5f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "DESCRIPTION").DESCRIPTION, detailHdrFont));
			cell.Colspan = 3;
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 9f, Border = 0 };
			cell.AddElement(new Paragraph(pageData.incidentDescription, detailTxtFont));
			cell.Colspan = 3;
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INJURY_SEVERITY").DESCRIPTION, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.severity, detailTxtFont));
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INJURY_PART").DESCRIPTION, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.bodyPart, detailTxtFont));
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INJURY_PART_SPECIFIC").DESCRIPTION, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.specificBodyPart, detailTxtFont));
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INJURY_TYPE").DESCRIPTION, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.injuryType, detailTxtFont));
			tableIncident.AddCell(cell);

			return tableIncident;
		}

		PdfPTable ContainmentSection(AlertData pageData)
		{
			PdfPTable tableContain = new PdfPTable(new float[] { 420f, 110f, 110f });
			tableContain.TotalWidth = 540f;
			tableContain.LockedWidth = true;
			tableContain.SpacingAfter = 5f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
			cell.Colspan = 3;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "CONTAINMENT").DESCRIPTION, detailHdrFont));
			tableContain.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "CONTAINMENT").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "RESPONSIBLE").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "ACTION_DT").DESCRIPTION_SHORT, colHdrFont));
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
				if (cc.START_DATE.HasValue)
				{
					cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)cc.START_DATE, "d", false), detailTxtFont));
				}
				else
				{
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
				tableContain.AddCell(cell);
			}

			return tableContain;
		}


		PdfPTable ProcedureSection(AlertData pageData)
		{
			PdfPTable tableProc = new PdfPTable(new float[] { 180f, 360f });
			tableProc.TotalWidth = 540f;
			tableProc.LockedWidth = true;
			tableProc.SpacingAfter = 5f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "STD_PROCS_TITLE").DESCRIPTION, detailHdrFont));
			cell.Colspan = 2;
			tableProc.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "STD_PROCS").DESCRIPTION, detailTxtBoldFont));

			if (pageData.incident.INCFORM_INJURYILLNESS.STD_PROCS_FOLLOWED == true)
				cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "TRUEFALSE", "1").DESCRIPTION_SHORT, detailTxtFont));
			else
				cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "TRUEFALSE", "0").DESCRIPTION_SHORT, detailTxtFont));

			tableProc.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "STD_PROCS_DESC").DESCRIPTION, detailTxtBoldFont));
			cell.AddElement(new Paragraph(pageData.incident.INCFORM_INJURYILLNESS.STD_PROCS_DESC, detailTxtFont));
			tableProc.AddCell(cell);

			return tableProc;
		}


		PdfPTable ReviewSection(AlertData pageData)
		{
			PdfPTable tableReview = new PdfPTable(new float[] { 180f, 180f, 180f });
			tableReview.TotalWidth = 540f;
			tableReview.LockedWidth = true;
			tableReview.SpacingAfter = 5f;
			PdfPCell cell;
			int approvalCount = pageData.approvalList.Where(l => l.approval.APPROVER_PERSON_ID.HasValue).Count();
			int row = 0;

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
			cell.Colspan = 3;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "REVIEW").DESCRIPTION, detailHdrFont));
			tableReview.AddCell(cell);

			foreach (EHSIncidentApproval approval in pageData.approvalList.Where(l => l.approval.APPROVER_PERSON_ID.HasValue).ToList())
			{
				++row;
				cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
				cell.BorderWidthTop =  cell.BorderWidthLeft = .25f;
				if (row == approvalCount)
					cell.BorderWidthBottom = .25f;
				cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_APPROVALS", approval.approval.ITEM_SEQ.ToString()).DESCRIPTION_SHORT, detailTxtBoldFont));
				tableReview.AddCell(cell);

				cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
				cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
				if (row == approvalCount)
					cell.BorderWidthBottom = .25f;
				cell.AddElement(new Paragraph(approval.approval.APPROVER_PERSON, detailTxtFont));
				tableReview.AddCell(cell);

				cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
				cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
				if (row == approvalCount)
					cell.BorderWidthBottom = .25f;
				cell.AddElement(new Paragraph(string.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "DATED").DESCRIPTION_SHORT + ":  {0}", approval.approval.APPROVAL_DATE.HasValue ? SQMBasePage.FormatDate((DateTime)approval.approval.APPROVAL_DATE, "d", false) : ""), detailTxtFont));
				tableReview.AddCell(cell);
			}

			return tableReview;
		}

		AlertData PopulateByIncidentId(decimal iid)
		{
			AlertData d = new AlertData();
			var entities = new PSsqmEntities();

			d.incident = EHSIncidentMgr.SelectIncidentById(entities, iid);

			if (d.incident != null)
			{
				try
				{
					string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)d.incident.DETECT_PLANT_ID);
					d.incidentLocation = plantName;
					d.incidentNumber = WebSiteCommon.FormatID(iid, 6);

					string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(iid);
					decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(iid);
					decimal companyId = d.incident.DETECT_COMPANY_ID;
					var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
					questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

					d.answerList = EHSIncidentMgr.GetIncidentAnswerList(d.incident.INCIDENT_ID);
					INCIDENT_ANSWER answer = null;

					// Date/Time

					d.incidentDate = d.incident.INCIDENT_DT.ToShortDateString();
					if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay).SingleOrDefault()) != null)
					{
						if (!string.IsNullOrEmpty(answer.ANSWER_VALUE))
							d.incidentTime = Convert.ToDateTime(answer.ANSWER_VALUE).ToShortTimeString();
					}

					if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Shift).SingleOrDefault()) != null)
					{
						answer.ANSWER_VALUE = SQMBasePage.GetXLAT(reportXLAT, "SHIFT", answer.ANSWER_VALUE).DESCRIPTION;
					}

					if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Department).SingleOrDefault()) != null)
					{
						d.incidentDept = answer.ANSWER_VALUE;
						decimal deptID = 0;
						if (decimal.TryParse(answer.ANSWER_VALUE, out deptID))
						{
							DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, deptID);
							if (dept != null)
								d.incidentDept = dept.DEPT_NAME;
						}
					}

					// Incident Type

					d.incidentType = incidentType;

					// Description

					d.incidentDescription = d.incident.DESCRIPTION;

					d.detectPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.incident.CREATE_PERSON, "", false);
					if (d.detectPerson != null)
					{
						d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.detectPerson.SUPV_EMP_ID);
					}

					if (d.incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
					{
						if (d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID.HasValue)
						{
							d.involvedPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID, "", false);
							if (d.involvedPerson != null)
								d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.involvedPerson.SUPV_EMP_ID);
						}
						else
						{
							d.involvedPerson = new PERSON();
							d.involvedPerson.FIRST_NAME = d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_NAME;
						}

						try
						{
							d.jobDescription = SQMModelMgr.LookupJobcode(entities, d.incident.INCFORM_INJURYILLNESS.JOBCODE_CD).JOB_DESC;
						}
						catch { }

						d.jobTenure = SQMBasePage.GetXLAT(reportXLAT, "INJURY_TENURE",d.incident.INCFORM_INJURYILLNESS.JOB_TENURE).DESCRIPTION;
						d.injuryType = SQMBasePage.GetXLAT(reportXLAT, "INJURY_TYPE", d.incident.INCFORM_INJURYILLNESS.INJURY_TYPE).DESCRIPTION;
						d.bodyPart = SQMBasePage.GetXLAT(reportXLAT, "INJURY_PART", d.incident.INCFORM_INJURYILLNESS.INJURY_BODY_PART).DESCRIPTION_SHORT;
						d.specificBodyPart = SQMBasePage.GetXLAT(reportXLAT, "INJURY_PART", d.incident.INCFORM_INJURYILLNESS.INJURY_BODY_PART).DESCRIPTION;
						d.employeeType = d.incident.INCFORM_INJURYILLNESS.COMPANY_SUPERVISED ? "Employee" : "Non-Employee";  // td - make XLAT

						if ((bool)d.incident.INCFORM_INJURYILLNESS.FIRST_AID)
						{
							d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "FIRSTAID").DESCRIPTION;
						}
						else if ((bool)d.incident.INCFORM_INJURYILLNESS.RECORDABLE)
						{
							d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "RECORDABLE").DESCRIPTION;
							if ((bool)d.incident.INCFORM_INJURYILLNESS.LOST_TIME)
							{
								d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "LOSTTIME").DESCRIPTION;
							}
							if ((bool)d.incident.INCFORM_INJURYILLNESS.FATALITY)
							{
								d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "FATALITY").DESCRIPTION;
							}
						}
					}

					// Containment
					foreach (INCFORM_CONTAIN cc in EHSIncidentMgr.GetContainmentList(iid, null, false))
					{
						if (cc.ASSIGNED_PERSON_ID.HasValue)
						{
							cc.ASSIGNED_PERSON = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)cc.ASSIGNED_PERSON_ID, ""));
						}
						d.containList.Add(cc);
					}

					// Root Cause(s)
					d.root5YList = EHSIncidentMgr.GetRootCauseList(iid).Where(l => !string.IsNullOrEmpty(l.ITEM_DESCRIPTION)).ToList();
					if (d.root5YList != null && d.root5YList.Count > 0)
					{
						d.incident.INCFORM_CAUSATION.Load();
						if (d.incident.INCFORM_CAUSATION != null && d.incident.INCFORM_CAUSATION.Count > 0)
						{
							d.causation = d.incident.INCFORM_CAUSATION.ElementAt(0);
						}
					}

					// Corrective Actions
					foreach (TASK_STATUS ac in EHSIncidentMgr.GetCorrectiveActionList(iid, null, false))
					{
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


					d.approvalList = EHSIncidentMgr.GetApprovalList(entities, (decimal)d.incident.ISSUE_TYPE_ID, 2.5m, iid, null, 0);

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
				catch
				{
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