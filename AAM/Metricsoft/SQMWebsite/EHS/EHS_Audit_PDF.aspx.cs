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
	public class AuditData
	{
		public string auditNumber;
		public string auditDate;
		public string auditLocation;
		public string auditDepartment;
		public string auditType;
		public string auditDescription;
		public PERSON auditPerson;
		public string auditCloseDate;
		public string auditPercentComplete;
		public string auditTotalScore;
		public string auditTotalPossibleScore;
		public string auditTotalPercentAchieved;

		public AUDIT audit;
		public List<AUDIT_TOPIC> topicList;
		public List<EHSAuditQuestion> questionList;
		public List<TASK_STATUS> actionList;

		public AuditData()
		{
			auditNumber = "N/A";
			auditDate = "N/A";
			auditLocation = "N/A";
			auditDepartment = "N/A";
			auditType = "N/A";
			auditDescription = "N/A";
			auditCloseDate = "N/A";
			auditPercentComplete = "0%";
			auditTotalScore = "0";
			auditTotalPossibleScore = "0";
			auditTotalPercentAchieved = "0%";
			auditPerson = null;
			audit = null;
			topicList = new List<AUDIT_TOPIC>();
			questionList = new List<EHSAuditQuestion>();
			actionList = new List<TASK_STATUS>();
		}
	}

	public partial class EHS_Audit_PDF : System.Web.UI.Page
	{
		public int totalQuestions;
		public decimal totalPossibleScore;
		public decimal totalWeightScore;
		public int totalTopicQuestions;
		public int totalTopicPositive;
		public decimal totalTopicWeightScore;
		public decimal totalTopicPossibleScore;
		public decimal totalPercent;
		public decimal possibleScore;
		public int totalPositive;
		public bool answerIsPositive;
		public bool questionAnswered;
		public string query;


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
		public Font detailTxtFontBold
		{
			get { return ViewState["DetailTxtFontBold"] == null ? null : (Font)ViewState["DetailTxtFontBold"]; }
			set { ViewState["DetailTxtFontBold"] = value; }
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
		public Font helpTxtFont
		{
			get { return ViewState["HelpTxtFont"] == null ? null : (Font)ViewState["HelpTxtFont"]; }
			set { ViewState["HelpTxtFont"] = value; }
		}

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
			Response.AddHeader("Content-Disposition", "attachment; filename=Audit-Report-" + query.Trim() + "-" + SessionManager.UserContext.LocalTime.ToString("yyyy-MM-dd") + ".pdf");

			Response.BinaryWrite(strS);
			Response.End();
			Response.Flush();
			Response.Clear();
		}

		private byte[] BuildPdf()
		{
			reportXLAT = SQMBasePage.SelectXLATList(new string[5] { "AUDIT_PRINT", "TRUEFALSE", "SHIFT", "INJURY_TENURE", "INJURY_CAUSE" }, 1);

			AuditData pageData;
			
			baseApplicationUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

			if (Request.QueryString["aid"] != null)
			{
				query = Request.QueryString["aid"];
				//query = query.Replace(" ", "+");
				//string aid = EncryptionManager.Decrypt(query);
				string aid = query;
				pageData = PopulateByAuditId(Convert.ToDecimal(aid));
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
			colHdrFont = new Font(colHeaderFont.BaseFont, 10,  iTextSharp.text.Font.UNDERLINE, blackColor);
			detailTxtFontBold = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD, blackColor);
			detailTxtItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
			helpTxtFont = new Font(textFont.BaseFont, 8, 0, blackColor);

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

					var hdrFont = new Font(headerFont.BaseFont, 24, 0, darkGrayColor);
					table1.AddCell(new PdfPCell(new Phrase(pageData.auditType, hdrFont))
					{
						HorizontalAlignment = Element.ALIGN_RIGHT,
						VerticalAlignment = Element.ALIGN_MIDDLE,
						Border = 0
					});


					//
					// Table 4 - Photos
					//
					// we are not printing attachments at this time, but leave the logic in, just in case
					//
					//var table4 = new PdfPTable(new float[] { 180f, 180f, 180f });
					//table4.TotalWidth = 540f;
					//table4.LockedWidth = true;

					//try
					//{
					//	if (pageData.photoData != null && pageData.photoData.Count() > 0)
					//	{
					//		table4.AddCell(new PdfPCell(new Phrase("Photos", detailHdrFont)) { Padding = 5f, Border = 0, Colspan = 3 });
					//		table4.SpacingBefore = 5f;
					//		var captionFont = new Font(textFont.BaseFont, 11, 0, darkGrayColor);

					//		int i = 0;
					//		for (i = 0; i < pageData.photoData.Count; i++)
					//		{
					//			var photoCell = new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 };

					//			iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(pageData.photoData[i]);
					//			photo.ScaleToFit(176f, 132f);
					//			photoCell.AddElement(photo);

					//			photoCell.AddElement(new Phrase(pageData.photoCaptions[i], captionFont));

					//			table4.AddCell(photoCell);
					//		}
					//		// pad remaining cells in row or else table will be corrupt
					//		int currentCol = i % 3;
					//		for (int j = 0; j < 3 - currentCol; j++)
					//			table4.AddCell(new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 });
					//	}
					//}
					//catch { }

					document.Add(table1); // adds the report header (audit type)
					document.Add(HeaderSection(pageData)); // audit information
					totalQuestions = 0;
					totalPositive = 0;
					totalPossibleScore = 0;
					foreach (AUDIT_TOPIC topic in pageData.topicList)
					{
						totalTopicQuestions = 0;
						totalTopicPositive = 0;
						totalTopicWeightScore = 0;
						totalTopicPossibleScore = 0;
						document.Add(TopicSection(topic));
						List<EHSAuditQuestion> questions = pageData.questionList.Where(a => a.TopicId == topic.AUDIT_TOPIC_ID).ToList();
						foreach (EHSAuditQuestion question in questions)
						{
							answerIsPositive = true;
							questionAnswered = false;
							possibleScore = 0;
							List<TASK_STATUS> tasks = pageData.actionList.Where(a => a.RECORD_SUBID == question.QuestionId).ToList();
							document.Add(QuestionSection(question, tasks)); // questions - actions will be listed with the associated questions
							// update the topic totals
							if (question.QuestionType == EHSAuditQuestionType.RadioPercentage || question.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
							{
								totalQuestions += 1;
								totalTopicQuestions += 1;
								totalPossibleScore += possibleScore;
								totalTopicPossibleScore += possibleScore;
								if (answerIsPositive)
								{
									totalPositive += 1;
									totalTopicPositive += 1;
								}
							}

						}
						if (totalTopicQuestions > 0)
						{
							if (totalTopicPossibleScore > 0)
								totalPercent = totalTopicWeightScore / totalTopicPossibleScore;
							else
								totalPercent = 0;
							document.Add(TopicFooterSection());
						}
						else
						{
							document.Add(new Paragraph(" ", detailTxtFont));
						}
					}
					//document.Add(ActionSection(pageData)); // these are added after each question
					//document.Add(table4); // attachments

					if (totalPossibleScore > 0)
						document.Add(FooterSection());

					document.Close();
				}
				catch (Exception ex)
				{
					// just close the document
					document.Close();
				}

				return output.ToArray();
			}
		}

		PdfPTable HeaderSection(AuditData pageData)
		{
			PdfPTable tableHeader = new PdfPTable(new float[] { 540f });
			tableHeader.TotalWidth = 540f;
			tableHeader.LockedWidth = true;
			PdfPCell cell;
			INCIDENT_ANSWER answer = null;

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT,"AUDIT_PRINT", "PLANT").DESCRIPTION_SHORT + ":  {0}", pageData.auditLocation), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "AUDIT_PRINT", "DEPARTMENT").DESCRIPTION_SHORT + ":  {0}", pageData.auditDepartment), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "AUDIT_PRINT", "DESCRIPTION").DESCRIPTION_SHORT + ":  {0}", pageData.auditDescription), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "AUDIT_PRINT", "DATE").DESCRIPTION_SHORT + ":  {0}", pageData.auditDate), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "AUDIT_PRINT", "CLOSE_DATE").DESCRIPTION_SHORT + ":  {0}", pageData.auditCloseDate), detailTxtFont));
			tableHeader.AddCell(cell);

			cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2f, Border = 0 };
			//cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight =.25f;
			cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "AUDIT_PRINT", "AUDIT_PERSON").DESCRIPTION_SHORT + ":  {0}", pageData.auditPerson == null ? "" : SQMModelMgr.FormatPersonListItem(pageData.auditPerson)), detailTxtFont));
			tableHeader.AddCell(cell);

			return tableHeader;
		}

		PdfPTable FooterSection()
		{
			PdfPTable tableTopic = new PdfPTable(new float[] { 540f });
			tableTopic.TotalWidth = 540f;
			tableTopic.LockedWidth = true;
			tableTopic.SpacingBefore = 5f;
			tableTopic.SpacingAfter = 10f;
			tableTopic.HorizontalAlignment = Element.ALIGN_RIGHT;

			Paragraph p = new Paragraph(); 
			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			p = new Paragraph(string.Format("Total Possible Points:   {0:0}", totalPossibleScore), detailTxtFontBold); 
			p.Alignment = Element.ALIGN_RIGHT;
			cell.AddElement(p);
			tableTopic.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			p = new Paragraph(string.Format("Total Points Achieved:   {0:0}", totalWeightScore), detailTxtFontBold); 
			p.Alignment = Element.ALIGN_RIGHT;
			cell.AddElement(p);
			tableTopic.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			if (totalPossibleScore > 0)
				totalPercent = totalWeightScore / totalPossibleScore;
			else
				totalPercent = 0;
			p = new Paragraph(string.Format("Percentage of Points Achieved:   {0:0%}", totalPercent), detailTxtFontBold); 
			p.Alignment = Element.ALIGN_RIGHT;
			cell.AddElement(p);
			tableTopic.AddCell(cell);

			return tableTopic;
		}

		PdfPCell FormatAnswerCell(AuditData pageData, decimal questionID)
		{
			PdfPCell cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
			if (questionID == 0)
			{
				cell.AddElement(new Paragraph(" ", detailTxtFont));
			}
			else
			{
				//INCIDENT_ANSWER answer = pageData.answerList.Where(a => a.INCIDENT_QUESTION_ID == questionID).FirstOrDefault();
				//if (answer != null)
				//{
				//	cell.AddElement(new Paragraph(String.Format(answer.ORIGINAL_QUESTION_TEXT + ": {0}", answer.ANSWER_VALUE), detailTxtFont));
				//}
				//else
				//{
				//	cell.AddElement(new Paragraph(" ", detailTxtFont));
				//}
			}

			return cell;
		}

		PdfPTable TopicSection(AUDIT_TOPIC topic)
		{
			PdfPTable tableTopic = new PdfPTable(new float[] { 540f });
			tableTopic.TotalWidth = 540f;
			tableTopic.LockedWidth = true;
			tableTopic.SpacingBefore = 5f;
			BaseColor lightGrayColor = new BaseColor(0.5f, 0.5f, 0.5f);

			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 1, BackgroundColor = lightGrayColor };
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			cell.PaddingBottom = 2f;
			cell.AddElement(new Paragraph(topic.TITLE, labelTxtFont));
			tableTopic.AddCell(cell);

			return tableTopic;
		}

		PdfPTable TopicFooterSection()
		{
			PdfPTable tableTopic = new PdfPTable(new float[] { 540f });
			tableTopic.TotalWidth = 540f;
			tableTopic.LockedWidth = true;
			tableTopic.SpacingBefore = 5f;
			tableTopic.SpacingAfter = 10f;
			tableTopic.HorizontalAlignment = Element.ALIGN_RIGHT;

			PdfPCell cell;

			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			Paragraph p = new Paragraph(string.Format("{0:0%}", totalPercent), detailTxtFontBold);
			p.Alignment = Element.ALIGN_RIGHT;
			cell.AddElement(p);
			tableTopic.AddCell(cell);

			return tableTopic;
		}

		PdfPTable QuestionSection(EHSAuditQuestion question, List<TASK_STATUS> tasks)
		{
			PdfPTable tableQuestion = new PdfPTable(new float[] { 202f, 203f, 135f });
			tableQuestion.TotalWidth = 540f;
			tableQuestion.LockedWidth = true;
			//tableQuestion.SpacingBefore = 5f;
			PdfPCell cell;
			Paragraph p = new Paragraph();

			cell = new PdfPCell() { Padding = 1f, PaddingBottom = 2f, BorderWidthBottom = 1 };
			p.Add(new Phrase(FormatText(question.QuestionText), detailTxtFont));
			try
			{
				if (!question.HelpText.Equals(null) || question.HelpText.Trim().Length > 0)
				{
					//String encodedString = HttpUtility.HtmlEncode(question.HelpText);
					string encodedString = FormatText(question.HelpText);
					p.Add(new Phrase("\n" + encodedString, detailTxtFont));
				}
			}
			catch
			{
				// for some reason, the error trapping isn't catching the nulls
			}
			cell.AddElement(p);
			tableQuestion.AddCell(cell); 

			cell = new PdfPCell() { Padding = 1f, BorderWidthBottom = 1 };
			p = new Paragraph();
			p.Add(new Phrase(FormatText(question.AnswerComment), detailTxtFont));
			try
			{
				if (!question.ResolutionComment.Equals(null) || question.ResolutionComment.Trim().Length > 0)
				{
					string encodedString = FormatText(question.ResolutionComment);
					p.Add(new Phrase("\n \nResolution: " + encodedString, detailTxtFont));
				}
			}
			catch
			{
				// for some reason, the error trapping isn't catching the nulls
			}
			cell.AddElement(p);
			tableQuestion.AddCell(cell);

			cell = new PdfPCell() { Padding = 1f, BorderWidthBottom = 1 };
			if (question.QuestionType == EHSAuditQuestionType.RadioPercentage || question.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft
				 || question.QuestionType == EHSAuditQuestionType.Radio || question.QuestionType == EHSAuditQuestionType.RadioCommentLeft)
			{
				int i = 0;
				PdfPTable answerTable = new PdfPTable(new float[] { 20f, 114f });
				answerTable.TotalWidth = 134f;
				answerTable.LockedWidth = true;
				PdfPCell answerCell = new PdfPCell();
				foreach (var choice in question.AnswerChoices)
				{
					string imageURL = Server.MapPath("~");
					if (choice.Value == question.AnswerValue)
						imageURL += "\\images\\RadioOn.jpg";
					else
						imageURL += "\\images\\RadioOff.jpg";
					iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageURL);
					image.ScaleToFit(15f, 15f);
					answerCell = new PdfPCell() { Padding = 1f, BorderWidth = 0 };
					answerCell.AddElement(new Chunk(image, 0, 0));
					answerTable.AddCell(answerCell);
					answerCell = new PdfPCell() { Padding = 1f, BorderWidth = 0 };
					answerCell.AddElement(new Phrase(" " + choice.Text, detailTxtFont));
					answerTable.AddCell(answerCell);

					//p = new Paragraph();
					//p.Add(new Chunk(image, 0, 0));
					//p.Add(new Phrase(" " + choice.Text, detailTxtFont));
					//cell.AddElement(p);
					//try
					//{
					//	iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
					//	jpg.ScaleToFit(20f, 20f);
					//	cell.AddElement(jpg);
					//}
					//catch (Exception ex)
					//{
					//	// what it the error?
					//}
					//cell.AddElement(new Chunk(choice.Text, detailTxtFont));
					if (question.QuestionType == EHSAuditQuestionType.RadioPercentage || question.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
					{
						if (choice.ChoiceWeight > possibleScore)
							possibleScore = choice.ChoiceWeight;
						if (choice.Value == question.AnswerValue)
						{
							if (choice.ChoicePositive)
								answerIsPositive = true;
							totalWeightScore += choice.ChoiceWeight;
							totalTopicWeightScore += choice.ChoiceWeight;
						}
					}
				}
				cell.AddElement(answerTable);
			}
			else
				cell.AddElement(new Paragraph(question.AnswerValue));
			tableQuestion.AddCell(cell);

			if (question.TasksAssigned > 0)
			{
				cell = new PdfPCell() { Padding = 1f, BorderWidthBottom = 1 };
				cell.Colspan = 3;
				// get the tasks
				// add the new table to the main question table
				cell.AddElement(ActionSection(question.QuestionId, tasks));
				tableQuestion.AddCell(cell);
			}
			return tableQuestion;
		}

		PdfPTable ActionSection(decimal questionId, List<TASK_STATUS> actions)
		{
			PdfPTable tableAction = new PdfPTable(new float[] { 80f, 50f, 150f, 50f, 150f, 55f });
			tableAction.TotalWidth = 535f;
			tableAction.LockedWidth = true;

			// add a action header
			PdfPCell cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.ResponsiblePerson, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.DueDate, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.Description, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.Status, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.Comments, colHdrFont));
			tableAction.AddCell(cell);
			cell = new PdfPCell() { Padding = 1f, Border = 0 };
			cell.AddElement(new Paragraph(Resources.LocalizedText.Completed, colHdrFont));
			tableAction.AddCell(cell);

			foreach (TASK_STATUS ac in actions)
			{
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				string person = "";
				if (ac.RESPONSIBLE_ID.HasValue)
				{
					person = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)ac.RESPONSIBLE_ID, ""));
				}
				cell.AddElement(new Paragraph(person, detailTxtFont));
				tableAction.AddCell(cell);

				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				if (ac.DUE_DT.HasValue)
					cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)ac.DUE_DT, "d", false), detailTxtFont));
				else
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				tableAction.AddCell(cell);
				
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				try
				{
					if (!ac.DESCRIPTION.Equals(null))
						cell.AddElement(new Paragraph(FormatText(ac.DESCRIPTION), detailTxtFont));
					else
						cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
				catch { cell.AddElement(new Paragraph(" ", detailTxtFont)); }
				tableAction.AddCell(cell);
				
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				if (!ac.STATUS.Equals(null))
				{
					try
					{
						int value = Convert.ToInt16(ac.STATUS);
						TaskStatus status = (TaskStatus)value;
						cell.AddElement(new Paragraph(status.ToString(), detailTxtFont));
					}
					catch { cell.AddElement(new Paragraph(ac.STATUS, detailTxtFont)); }
				}
				else
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				tableAction.AddCell(cell);
				
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				try
				{
					if (!ac.COMMENTS.Equals(null))
						cell.AddElement(new Paragraph(FormatText(ac.COMMENTS), detailTxtFont));
					else
						cell.AddElement(new Paragraph(" ", detailTxtFont));
				}
				catch { cell.AddElement(new Paragraph(" ", detailTxtFont)); }
				tableAction.AddCell(cell);
				
				cell = new PdfPCell() { Padding = 1f, Border = 0 };
				if (ac.COMPLETE_DT.HasValue)
					cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)ac.COMPLETE_DT, "d", false), detailTxtFont));
				else
					cell.AddElement(new Paragraph(" ", detailTxtFont));
				tableAction.AddCell(cell);
			}

			return tableAction;
		}

		AuditData PopulateByAuditId(decimal aid)
		{
			AuditData d = new AuditData();
			var entities = new PSsqmEntities();

			d.audit = EHSAuditMgr.SelectAuditById(entities, aid);

			if (d.audit != null)
			{
				try
				{
					string plantName = EHSAuditMgr.SelectPlantNameById((decimal)d.audit.DETECT_PLANT_ID);
					d.auditLocation = plantName;
					d.auditNumber = aid.ToString();

					AUDIT_TYPE auditType = EHSAuditMgr.SelectAuditTypeById(entities, d.audit.AUDIT_TYPE_ID);
					d.auditType = auditType.TITLE; // do I need this?
					decimal auditTypeId = d.audit.AUDIT_TYPE_ID; // if I have all this, why am I redefining it?
					decimal companyId = d.audit.DETECT_COMPANY_ID;
					if (d.audit.DEPT_ID != null)
					{
						if (d.audit.DEPT_ID == 0)
							d.auditDepartment = "Plant Wide";
						else
						{
							DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, (decimal)d.audit.DEPT_ID);
							d.auditDepartment = dept.DEPT_NAME;
						}
					}

					var questions = EHSAuditMgr.SelectAuditQuestionList(auditTypeId, 0, aid);
					d.questionList = questions;

					List<AUDIT_TOPIC> topics = new List<AUDIT_TOPIC>();
					AUDIT_TOPIC topic = new AUDIT_TOPIC();
					decimal previousTopicId = 0;
					foreach (EHSAuditQuestion question in questions)
					{
						if (question.TopicId != previousTopicId)
						{
							topic = new AUDIT_TOPIC();
							topic.AUDIT_TOPIC_ID = question.TopicId;
							topic.TITLE = question.TopicTitle;
							topics.Add(topic);
							previousTopicId = question.TopicId;
						}
					}
					d.topicList = topics;

					// Date/Time

					d.auditDate = d.audit.AUDIT_DT.ToShortDateString();
					DateTime closeDate = d.audit.AUDIT_DT.AddDays(auditType.DAYS_TO_COMPLETE);
					d.auditCloseDate = closeDate.ToShortDateString();

					// Description

					d.auditDescription = d.audit.DESCRIPTION;

					d.auditPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.audit.AUDIT_PERSON, "", false);

					// Audit Exception Actions 

					foreach (TASK_STATUS ac in EHSAuditMgr.GetAuditActionList(aid, 0))
					{
						//if (ac.RESPONSIBLE_ID.HasValue)
						//{
						//	ac.COMMENTS = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)ac.RESPONSIBLE_ID, ""));
						//}
						d.actionList.Add(ac);
					}

					// not showing the attachments at this point, but not deleting code... just in case
					//var files = (from a in entities.ATTACHMENT
					//			 where
					//				(a.RECORD_ID == aid && a.RECORD_TYPE == 40 && a.DISPLAY_TYPE > 0) &&
					//				(a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
					//				a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
					//				a.FILE_NAME.ToLower().Contains(".bmp"))
					//			 orderby a.RECORD_TYPE, a.FILE_NAME
					//			 select new
					//			 {
					//				 Data = (from f in entities.ATTACHMENT_FILE where f.ATTACHMENT_ID == a.ATTACHMENT_ID select f.ATTACHMENT_DATA).FirstOrDefault(),
					//				 Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
					//			 }).ToList();


					//if (files.Count > 0)
					//{
					//	d.photoData = new List<byte[]>();
					//	d.photoCaptions = new List<string>();

					//	foreach (var f in files)
					//	{
					//		d.photoData.Add(f.Data);
					//		d.photoCaptions.Add(f.Description);
					//	}
					//}
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

		protected string FormatText(string inputText)
		{
			string outputText;
			outputText = inputText.Replace("\r", "").Replace("\n", "");
			outputText = outputText.Replace("<br>", "\n");
			return outputText;
		}
	}
}