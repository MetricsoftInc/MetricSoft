using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;
using System.Globalization;

namespace SQM.Website
{
    public partial class Ucl_EHSAuditDetails : System.Web.UI.UserControl
    {
		public void Refresh(decimal auditId, int[] steps)
		{
			var entities = new PSsqmEntities();
			var companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			decimal? auditTypeId = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i.AUDIT_TYPE_ID).FirstOrDefault();

			var sb = new StringBuilder();

			if (auditTypeId != null)
			{
				foreach (int step in steps)
				{
					var questions = EHSAuditMgr.SelectAuditQuestionList((decimal)auditTypeId, 0);

					sb.AppendLine("<table class=\"lightTable\" cellspacing=\"0\" style=\"width: 100%\">");
					foreach (var q in questions)
					{
						//string answer = (from a in entities.AUDIT_ANSWER
						//				 where a.AUDIT_ID == auditId && a.AUDIT_QUESTION_ID == q.QuestionId
						//				 select a.ANSWER_VALUE).FirstOrDefault();
						var auditAnswer = (from a in entities.AUDIT_ANSWER
										   where a.AUDIT_ID == auditId && a.AUDIT_QUESTION_ID == q.QuestionId
										   select a).FirstOrDefault();
						string answer = (auditAnswer.ANSWER_VALUE == null) ? "" : auditAnswer.ANSWER_VALUE;
						string comment = (auditAnswer.COMMENT == null) ? "" : auditAnswer.COMMENT;

						answer = answer.Replace("<a href", "<a target=\"blank\" href");

						if (!string.IsNullOrEmpty(answer) ||
							q.QuestionType == EHSAuditQuestionType.Attachment ||
							q.QuestionType == EHSAuditQuestionType.PageOneAttachment)
						{
							switch (q.QuestionType)
							{
								case EHSAuditQuestionType.Date:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToShortDateString();
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.Time:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString();
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.DateTime:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToString();
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.LocationDropdown:
									answer = EHSAuditMgr.SelectPlantNameById(Convert.ToDecimal(answer));
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.UsersDropdown:
									answer = EHSAuditMgr.SelectUserNameById(Convert.ToDecimal(answer));
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.UsersDropdownLocationFiltered:
									answer = EHSAuditMgr.SelectUserNameById(Convert.ToDecimal(answer));
									answer = Server.HtmlEncode(answer);
									break;

								case EHSAuditQuestionType.Attachment:
									answer = GetUploadedFiles(40, auditId, (step + 1).ToString());
									break;

								case EHSAuditQuestionType.PageOneAttachment:
									answer = GetUploadedFiles(40, auditId, (step + 1).ToString());
									break;

							}
						}
						// Add a comment box that hides/shows via a link to certain field types
						if (q.QuestionType == EHSAuditQuestionType.BooleanCheckBox || q.QuestionType == EHSAuditQuestionType.CheckBox ||
							q.QuestionType == EHSAuditQuestionType.Dropdown || q.QuestionType == EHSAuditQuestionType.PercentTextBox ||
							q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio)
						{
							comment = Server.HtmlEncode(comment);
							sb.AppendLine(string.Format("<tr><td style=\"width: 33%;\">{0}</td><td>{1}</td><td>{2}</td></tr>", q.QuestionText, answer, comment));
						}
						else
						{
							sb.AppendLine(string.Format("<tr><td style=\"width: 33%;\">{0}</td><td>{1}</td><td></td></tr>", q.QuestionText, answer));
						}
					}
				}
				sb.AppendLine("</table>");
			}
			
			litAuditDetails.Text = sb.ToString();
		}

		public string GetUploadedFiles(int recordType, decimal recordId, string recordStep)
		{
			var entities = new PSsqmEntities();

			var files = (from a in entities.ATTACHMENT
						 where a.RECORD_TYPE == recordType && a.RECORD_ID == recordId
						 orderby a.FILE_NAME
						 select new
						 {
							 AttachmentId = a.ATTACHMENT_ID,
							 RecordStep = a.RECORD_STEP,
							 FileName = a.FILE_NAME,
							 Description = a.FILE_DESC,
							 Size = a.FILE_SIZE,
							 DisplayType = a.DISPLAY_TYPE
						 }).ToList();

			if (!string.IsNullOrEmpty(recordStep))
			{
				if (recordType == 40 && recordStep == "2") // Special case to cover second page audit attachments originally entered with no recordstep
					files = files.Where(f => (f.RecordStep == "2" || f.RecordStep == "")).ToList();
				else
					files = files.Where(f => f.RecordStep == recordStep).ToList();
			}

			var sb = new StringBuilder();
			sb.AppendLine("<table class=\"RadFileExplorer\" style=\"border: 0 !important;\" cellspacing=0>");
			foreach (var f in files)
			{
				sb.AppendLine("<tr><td style=\"border: 0;\">");
				sb.AppendLine("<div class=\"rfeFileExtension " + GetFileExtension(f.FileName) + "\">");
				sb.AppendLine("<a href=\"/Shared/FileHandler.ashx?DOC=a&DOC_ID=" + f.AttachmentId + "&FILE_NAME=" + f.FileName + "\" style=\"text-decoration: underline;\" target=\"_blank\">");
				sb.AppendLine(f.FileName);
				sb.AppendLine("</a>");
				sb.AppendLine("</div>");
				sb.AppendLine("</td><td style=\"border: 0;\">");
				if (!string.IsNullOrEmpty(f.Description))
					sb.AppendLine("<span style=\"color: #444;\">" + f.Description + "</span>");
				sb.AppendLine("</td></tr>");
			}
			sb.AppendLine("</table>");

			return sb.ToString();
		}
			
	
		public string GetFileExtension(string fileName)
		{
			string ext = "";

			string[] parts = fileName.ToLower().Split('.');
			if (parts.Count() > 0)
				ext = parts[parts.Count() - 1];

			return ext;
		}

		public void Clear()
		{
			litAuditDetails.Text = "";
		}
    }
}