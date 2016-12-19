using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace SQM.Website.EHS
{
	[Serializable]
	public class ReportCell
	{
		public int Row
		{
			get;
			set;
		}
		public int Col
		{
			get;
			set;
		}
		public string Type
		{
			get;
			set;
		}
		public string Text
		{
			get;
			set;
		}
		public object Obj
		{
			get;
			set;
		}
	}

	public partial class EHS_Incident_Alert : System.Web.UI.Page
	{
		public string iid;
		public string exportOption;
		public List<ReportCell> exportList;
		public int exportRow;

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

			if (exportOption == "xls")
			{
				ExcelPackage export = new ExcelPackage();
				ExcelWorksheet exportSheet = export.Workbook.Worksheets.Add(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TITLE").DESCRIPTION);
				export.Workbook.Properties.Title = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TITLE").DESCRIPTION;

				int imageCount = 0;
				foreach (ReportCell c in exportList)
				{
					if (c.Type == "img")
					{
						++imageCount;
						ExcelPicture pic = exportSheet.Drawings.AddPicture(!string.IsNullOrEmpty(c.Text) ? c.Text : imageCount.ToString(), System.Drawing.Image.FromStream((MemoryStream)c.Obj));
						pic.SetSize(214, 161);
						pic.SetPosition((c.Row * 20) + ((imageCount - 1) * 170), 1);
					}
					else
					{
						exportSheet.Cells[c.Row, c.Col].Value = c.Text;
					}
					exportSheet.Cells[1, 1].Style.Font.Bold = true;
					exportSheet.Column(1).Width = 50;
					exportSheet.Column(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
					exportSheet.Column(2).Width = 75;
					exportSheet.Column(2).Style.WrapText = true;
					exportSheet.Column(2).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
				}

				Response.Clear();
				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.AddHeader(
							"content-disposition",
							string.Format("attachment;  filename={0}", " EHS-Incident-Alert-" + iid.ToString() + ".xlsx"));
				Response.BinaryWrite(export.GetAsByteArray());
				Response.Flush();
			}
			else
			{
				Response.ClearContent();
				Response.ClearHeaders();
				Response.ContentType = "application/pdf";
				Response.AddHeader("Content-Disposition", "attachment; filename=EHS-Incident-Alert-" + iid + ".pdf");
				Response.BinaryWrite(strS);
				Response.Flush();
			}

			Response.End();
		}

		private byte[] BuildPdf()
		{
			reportXLAT = SQMBasePage.SelectXLATList(new string[5] { "HS_ALERT", "TRUEFALSE", "SHIFT", "INJURY_TENURE", "INJURY_CAUSE" }, 1);

			AlertData pageData = new AlertData() 
				{
					incidentDate = "N/A",
					incidentTime = "N/A",
					incidentLocation = "N/A",
					locationNLS = "en",
					incidentDept = "N/A",
					incidentNumber = "N/A",
					incidentType = "N/A",
					incidentDescription = "N/A",
					detectPerson = null,
					involvedPerson = null,
					supervisorPerson = null,
					incident = null,
					answerList = new List<INCIDENT_ANSWER>(),
					containList = new List<INCFORM_CONTAIN>(),
					root5YList = new List<INCFORM_ROOT5Y>(),
					causation = null,
					actionList = new List<TASK_STATUS>(),
					approvalList = new List<EHSIncidentApproval>()
				};

			baseApplicationUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

			if (Request.QueryString["iid"] != null)
			{
				string query = Request.QueryString["iid"];
				query = query.Replace(" ", "+");
				iid = EncryptionManager.Decrypt(query);

				if (Request.QueryString["opt"] != null)
				{
					exportOption = Request.QueryString["opt"];
				}

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

			iTextSharp.text.Font textFont = GetTextFont();
			iTextSharp.text.Font headerFont = GetHeaderFont();
			iTextSharp.text.Font labelFont = GetLabelFont();
			iTextSharp.text.Font colHeaderFont = GetTextFont();
			iTextSharp.text.Font textItalicFont = GetTextFont();

			// Chinese text font
			iTextSharp.text.Font textFontZH = GetZHFont();

			detailHdrFont = new iTextSharp.text.Font(headerFont.BaseFont, 13, 0, lightGrayColor);
			detailLblFont = new iTextSharp.text.Font(textFont.BaseFont, 10, 0, blackColor);
			labelTxtFont = new iTextSharp.text.Font(labelFont.BaseFont, 12, 0, blackColor);
			colHdrFont = new iTextSharp.text.Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.UNDERLINE, blackColor);

			detailTxtBoldFont = new iTextSharp.text.Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD, blackColor);

			switch (pageData.locationNLS)
			{
				case "zh":
					detailTxtFont = new iTextSharp.text.Font(textFontZH.BaseFont, 10, 0, blackColor);
					detailTxtItalicFont = new iTextSharp.text.Font(textFontZH.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
					break;
				default:
					detailTxtFont = new iTextSharp.text.Font(textFont.BaseFont, 10, 0, blackColor);
					detailTxtItalicFont = new iTextSharp.text.Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
					break;
			}

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
					table1.SpacingAfter = 10f;

					iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
					img.ScaleToFit(102f, 51f);
					var imgCell = new PdfPCell() { Border = 0 };
					imgCell.AddElement(img);
					table1.AddCell(imgCell);

					var hdrFont = new iTextSharp.text.Font(headerFont.BaseFont, 24, 0, darkGrayColor);
					table1.AddCell(new PdfPCell(new Phrase(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TITLE").DESCRIPTION, hdrFont))
					{
						HorizontalAlignment = Element.ALIGN_LEFT,
						VerticalAlignment = Element.ALIGN_MIDDLE,
						Border = 0
					});


					// export to Excel
					if (exportOption == "xls")
					{
						exportList = new List<ReportCell>();
						exportRow = 1;
						exportList.Add(new ReportCell() { Row = 1, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TITLE").DESCRIPTION });
					}

					document.Add(table1);
					document.Add(IDSection(pageData));
					document.Add(HeaderSection(pageData));
					document.Add(IncidentSection(pageData));
					document.Add(ContainmentSection(pageData));
					document.Add(CauseSection(pageData));
					document.Add(ActionSection(pageData));

					//
					// Table 4 - Photos
					//
					var table4 = new PdfPTable(new float[] { 220f, 220f });
					table4.TotalWidth = 540f;
					table4.LockedWidth = true;
					table4.SpacingBefore = 15f;

					try
					{
						if (pageData.photoData != null && pageData.photoData.Count() > 0)
						{
							PdfPCell cell = new PdfPCell(new Phrase("Photos", detailHdrFont)) { Padding = 5f, Border = 0, Colspan = 2 };
							cell.BorderWidthTop = .25f;
							table4.AddCell(cell);
							table4.SpacingBefore = 5f;
							var captionFont = new iTextSharp.text.Font(textFont.BaseFont, 11, 0, darkGrayColor);

							if (exportOption == "xls")
							{
								exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
								exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "Photos"});
							}

							int i = 0;
							for (i = 0; i < pageData.photoData.Count; i++)
							{
								var photoCell = new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 };

								iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(pageData.photoData[i]);
								photo.ScaleToFit(176f, 132f);
								//photo.ScaleToFit(264f, 198f);
								photoCell.AddElement(photo);

								photoCell.AddElement(new Phrase(pageData.photoCaptions[i], captionFont));
								table4.AddCell(photoCell);

								if (exportOption == "xls")
								{
									MemoryStream ms = new MemoryStream(pageData.photoData[i]);
									exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Type = "img", Text = pageData.photoCaptions[i], Obj = ms });
								}

							}
							// pad remaining cells in row or else table will be corrupt
							int currentCol = i % 3;
							for (int j = 0; j < 3 - currentCol; j++)
								table4.AddCell(new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 });
						}
					}
					catch { }

					document.Add(table4);

					document.Close();
				}
				catch
				{
				}

				return output.ToArray();
			}
		}

		PdfPTable IDSection(AlertData pageData)
		{
			PdfPTable tableIncident = new PdfPTable(new float[] { 90f, 450f, });
			tableIncident.TotalWidth = 540f;
			tableIncident.LockedWidth = true;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 6, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "INCIDENTTYPE").DESCRIPTION_SHORT + ":", detailHdrFont));
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 4, Border = 0 };
			cell.AddElement(new Paragraph(string.Format(pageData.incidentType + "   ( # {0} )", pageData.incidentNumber), labelTxtFont));
			tableIncident.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "INCIDENTTYPE").DESCRIPTION_SHORT });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = string.Format(pageData.incidentType + "   ( # {0} )", pageData.incidentNumber) });
			}

			return tableIncident;
		}

		PdfPTable HeaderSection(AlertData pageData)
		{
			PdfPTable tableHeader = new PdfPTable(new float[] { 270f, 270f });
			tableHeader.TotalWidth = 540f;
			tableHeader.LockedWidth = true;
			PdfPCell cell;
			INCIDENT_ANSWER answer = null;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "PLANT").DESCRIPTION_SHORT + ":  {0}", pageData.incidentLocation), detailLblFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "LOCATION").DESCRIPTION_SHORT + ":  {0}", pageData.incidentDept), detailLblFont));
			tableHeader.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "PLANT").DESCRIPTION_SHORT });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.incidentLocation });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "LOCATION").DESCRIPTION_SHORT });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.incidentDept });
			}

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthBottom = cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(String.Format("{0}" + ":  {1}", SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "DATE").DESCRIPTION_SHORT,pageData.incidentDate), detailLblFont));
			tableHeader.AddCell(cell);
			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			cell.BorderWidthTop = cell.BorderWidthBottom = cell.BorderWidthRight = .25f;
			cell.AddElement(new Paragraph(String.Format("{0}" + ":  {1}", SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TIME").DESCRIPTION_SHORT, pageData.incidentTime), detailLblFont));
			tableHeader.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "DATE").DESCRIPTION_SHORT });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.incidentDate });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "TIME").DESCRIPTION_SHORT });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.incidentTime });
			}

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
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "DESCRIPTION").DESCRIPTION, detailHdrFont));
			tableIncident.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(pageData.incidentDescription, detailTxtFont));
			tableIncident.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "DESCRIPTION").DESCRIPTION });
				exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.incidentDescription });
			}

			return tableIncident;
		}

		PdfPTable ContainmentSection(AlertData pageData)
		{
			PdfPTable tableContain = new PdfPTable(new float[] { 540f });
			tableContain.TotalWidth = 540f;
			tableContain.LockedWidth = true;
			tableContain.SpacingBefore = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "CONTAINMENT").DESCRIPTION, detailHdrFont));
			tableContain.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "CONTAINMENT").DESCRIPTION });
			}

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "CONTAINMENT").DESCRIPTION_SHORT, colHdrFont));
			tableContain.AddCell(cell);

			int n = 0;
			foreach (var cc in pageData.containList)
			{
				List<string> descList = cc.ITEM_DESCRIPTION.Split('\r').ToList();
				foreach (string desc in descList)
				{
					cell = new PdfPCell() { Padding = 1f, Border = 0 };
					cell.AddElement(new Paragraph(desc.Replace("\n",""), detailTxtFont));
					tableContain.AddCell(cell);

					if (exportOption == "xls")
					{
						exportList.Add(new ReportCell() { Row = ++n == 1 ? exportRow : ++exportRow, Col = 2, Text = desc.Replace("\n","") });
					}
				}
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
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "ROOTCAUSE").DESCRIPTION, detailHdrFont));
			tableCause.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = "" });
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "ROOTCAUSE").DESCRIPTION });
			}

			if (pageData.root5YList != null && pageData.root5YList.Count > 0)
			{

				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				cell.AddElement(new Paragraph(pageData.root5YList.Last().ITEM_DESCRIPTION, detailTxtItalicFont));
				tableCause.AddCell(cell);

				if (exportOption == "xls")
				{
					exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = pageData.root5YList.Last().ITEM_DESCRIPTION });
				}
			}

			if (pageData.causation != null && !string.IsNullOrEmpty(pageData.causation.CAUSEATION_CD))
			{
				cell = new PdfPCell() { Padding = 1f, Border = 0, PaddingTop = 3f };
				cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "CAUSATION").DESCRIPTION, detailHdrFont));
				cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "INJURY_CAUSE", pageData.causation.CAUSEATION_CD).DESCRIPTION, detailTxtFont));
				tableCause.AddCell(cell);

				if (exportOption == "xls")
				{
					exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "CAUSATION").DESCRIPTION });
					exportList.Add(new ReportCell() { Row = exportRow, Col = 2, Text = SQMBasePage.GetXLAT(reportXLAT, "INJURY_CAUSE", pageData.causation.CAUSEATION_CD).DESCRIPTION });
				}
			}

			return tableCause;
		}

		PdfPTable ActionSection(AlertData pageData)
		{
			PdfPTable tableAction = new PdfPTable(new float[] { 540f });
			tableAction.TotalWidth = 540f;
			tableAction.LockedWidth = true;
			tableAction.SpacingBefore = 15f;
			tableAction.SpacingAfter = 15f;
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.BorderWidthTop = .25f;
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "ACTION").DESCRIPTION, detailHdrFont));
			tableAction.AddCell(cell);

			if (exportOption == "xls")
			{
				exportList.Add(new ReportCell() { Row = ++exportRow, Col = 1, Text = SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "ACTION").DESCRIPTION });
			}

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_ALERT", "ACTION").DESCRIPTION_SHORT, colHdrFont));
			tableAction.AddCell(cell);

			int n = 0;
			foreach (var ac in pageData.actionList)
			{
				List<string> descList = ac.DESCRIPTION.Split('\r').ToList();
				foreach (string desc in descList)
				{
					cell = new PdfPCell() { Padding = 1f, Border = 0 };
					cell.AddElement(new Paragraph(desc.Replace("\n",""), detailTxtFont));
					tableAction.AddCell(cell);

					if (exportOption == "xls")
					{
						exportList.Add(new ReportCell() { Row = ++n == 1 ? exportRow : ++exportRow, Col = 2, Text = desc.Replace("\n", "") });
					}
				}
			}

			return tableAction;
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
					//string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)d.incident.DETECT_PLANT_ID);
					PLANT plant = SQMModelMgr.LookupPlant((decimal)d.incident.DETECT_PLANT_ID);
					string plantName = plant.PLANT_NAME;
					d.incidentLocation = plantName;
					if (plant.LOCAL_LANGUAGE.HasValue)
						d.locationNLS = SQMModelMgr.LookupLanguage(entities, "", (int)plant.LOCAL_LANGUAGE, false).NLS_LANGUAGE;

					d.incidentNumber = WebSiteCommon.FormatID(iid, 6);
					string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(iid);
					decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(iid);
					decimal companyId = d.incident.DETECT_COMPANY_ID;
					var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
					questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

					d.answerList = EHSIncidentMgr.GetIncidentAnswerList(d.incident.INCIDENT_ID);
					INCIDENT_ANSWER answer = null;

					// Date/Time

					d.incidentDate = d.incident.INCIDENT_DT.ToString("MMMM dd, yyyy");
					if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay).SingleOrDefault()) != null)
					{
						if (!string.IsNullOrEmpty(answer.ANSWER_VALUE))
							d.incidentTime = Convert.ToDateTime(answer.ANSWER_VALUE).ToShortTimeString();
					}

					if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Shift).SingleOrDefault()) != null)
					{
						if (!string.IsNullOrEmpty(SQMBasePage.GetXLAT(reportXLAT, "SHIFT", answer.ANSWER_VALUE).DESCRIPTION))
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

					if (d.incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
					{
						if (d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID.HasValue)
						{
							d.involvedPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID, "", false);
						}
						else
						{
							d.involvedPerson = new PERSON();
							d.involvedPerson.FIRST_NAME = d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_NAME;
						}

						if (d.incident.INCFORM_INJURYILLNESS.DEPT_ID.HasValue)
						{
							DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, (decimal)d.incident.INCFORM_INJURYILLNESS.DEPT_ID);
							if (dept != null)
								d.incidentDept = dept.DEPT_NAME;
						}
						else
						{
							d.incidentDept = d.incident.INCFORM_INJURYILLNESS.DEPARTMENT;
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
									 ID = a.ATTACHMENT_ID
								 }).ToList();


					d.approvalList = EHSIncidentMgr.GetApprovalList(entities, (decimal)d.incident.ISSUE_TYPE_ID, 10.0m, iid, null, 0);

					if (files.Count > 0)
					{
						d.photoData = new List<byte[]>();
						d.photoCaptions = new List<string>();
						d.photoIDList = new List<decimal>();

						foreach (var f in files)
						{
							d.photoIDList.Add(f.ID);
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

		public iTextSharp.text.Font GetZHFont()		// chinese font
		{
			var fontName = "simhei";
			if (!FontFactory.IsRegistered(fontName))
			{
				var fontPath = Server.MapPath("~") + "images\\fonts\\simhei.ttf";
				FontFactory.Register(fontPath);
			}
			return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		}

	}
}