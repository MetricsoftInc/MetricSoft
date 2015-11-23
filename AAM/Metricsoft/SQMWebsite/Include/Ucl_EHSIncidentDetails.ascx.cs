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
    public partial class Ucl_EHSIncidentDetails : System.Web.UI.UserControl
    {
		// "Mini 8D" fields at the end of every incident form - to be hidden in the 8D Definition step
		//List<decimal> problemFieldIds = new List<decimal>() { 69, 24, 27, 70, 64, 65, 72, 66, 67 };

		//public bool HideProblemFields = false;

		public void Refresh(decimal incidentId, int[] steps)
		{
			var entities = new PSsqmEntities();
			var companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			decimal? incidentTypeId = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i.ISSUE_TYPE_ID).FirstOrDefault();

			var sb = new StringBuilder();

			if (incidentTypeId != null)
			{
				foreach (int step in steps)
				{
					var questions = EHSIncidentMgr.SelectIncidentQuestionList((decimal)incidentTypeId, companyId, step);

					sb.AppendLine("<table class=\"lightTable\" cellspacing=\"0\" style=\"width: 100%\">");
					foreach (var q in questions)
					{
						string answer = (from a in entities.INCIDENT_ANSWER
										 where a.INCIDENT_ID == incidentId && a.INCIDENT_QUESTION_ID == q.QuestionId
										 select a.ANSWER_VALUE).FirstOrDefault();
						answer = (answer == null) ? "" : answer;
						answer = answer.Replace("<a href", "<a target=\"blank\" href");
						string answerText = "";
						if (!string.IsNullOrEmpty(answer) ||
							q.QuestionType == EHSIncidentQuestionType.Attachment ||
							q.QuestionType == EHSIncidentQuestionType.PageOneAttachment)
						{
							switch (q.QuestionType)
							{
								case EHSIncidentQuestionType.Date:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToShortDateString();
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.Time:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToShortTimeString();
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.DateTime:
									answer = DateTime.Parse(answer, CultureInfo.GetCultureInfo("en-US")).ToString();
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.LocationDropdown:
									answer = EHSIncidentMgr.SelectPlantNameById(Convert.ToDecimal(answer));
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.UsersDropdown:
									answer = EHSIncidentMgr.SelectUserNameById(Convert.ToDecimal(answer));
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.UsersDropdownLocationFiltered:
									answer = EHSIncidentMgr.SelectUserNameById(Convert.ToDecimal(answer));
									answerText = Server.HtmlEncode(answer);
									break;

								case EHSIncidentQuestionType.Attachment:
									answerText = GetUploadedFiles(40, incidentId, (step + 1).ToString());
									break;

								case EHSIncidentQuestionType.PageOneAttachment:
									answerText = GetUploadedFiles(40, incidentId, (step + 1).ToString());
									break;
								default:
									if (answer == "Yes")
										answerText = Resources.LocalizedText.Yes;
									else if (answer == "No")
										answerText = Resources.LocalizedText.No;
									else
										answerText = q.AnswerChoices.Where(l => l.Value == answer).FirstOrDefault() != null ? q.AnswerChoices.Where(l => l.Value == answer).FirstOrDefault().Text : answer;
									break;
							}
						}

						sb.AppendLine(string.Format("<tr><td style=\"width: 33%;\">{0}</td><td>{1}</td></tr>", q.QuestionText, answerText));
					}
				}
				sb.AppendLine("</table>");
			}
			
			litIncidentDetails.Text = sb.ToString();
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
				if (recordType == 40 && recordStep == "2") // Special case to cover second page incident attachments originally entered with no recordstep
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
			litIncidentDetails.Text = "";
		}
    }
}