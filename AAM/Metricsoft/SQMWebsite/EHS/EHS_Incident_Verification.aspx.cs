using System;
using System.Linq;
using System.Text;


namespace SQM.Website
{
	public partial class EHS_Incident_Verification : SQMBasePage
    {
        decimal companyId = 0;
		decimal incidentId = 0;
		decimal personId = 0;
		decimal plantId = 0;

		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			if (Request.QueryString["inid"] != null && Request.QueryString["peid"] != null && Request.QueryString["plid"] != null)
			{
				incidentId = Convert.ToDecimal(Request.QueryString["inid"]);
				personId = Convert.ToDecimal(Request.QueryString["peid"]);
				plantId = Convert.ToDecimal(Request.QueryString["plid"]);

				if (personId == SessionManager.UserContext.Person.PERSON_ID)
				{
					INCIDENT incident = (from v in entities.INCIDENT_VERIFICATION
									where
										v.INCIDENT_ID == incidentId &&
										v.PERSON_ID == personId &&
										v.PLANT_ID == plantId
									select (from i in entities.INCIDENT
											where
												i.INCIDENT_ID == incidentId
											select i).FirstOrDefault()
											).FirstOrDefault();
					PLANT plant = (from p in entities.PLANT where p.PLANT_ID == plantId select p).FirstOrDefault();

					lblPlantLocation.Text = plant.DISP_PLANT_NAME;
					lblIncidentDate.Text = incident.INCIDENT_DT.ToShortDateString();
					lblIncidentInstructions.Text = incident.DESCRIPTION;

					decimal probCaseId = (decimal)incident.VERIFY_PROBCASE_ID;

					PopulateComments();

					lblFullName.Text = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;

					ltrDownloadReport.Text = BuildReport(probCaseId);
					CheckSubmitDisabled();
				}
				else
				{
					lblPageInstructions.Text = "INVALID ACCESS LEVEL.";
					divPageBody.Visible = false;
				}
			}
			
		}
		
		void PopulateComments()
		{
			var comments = (from n in entities.INCIDENT_VERIFICATION_COMMENT
								 where
									n.INCIDENT_ID == incidentId &&
									n.PLANT_ID == plantId
								 select new
								 {
									 Comment = n.COMMENT,
									 PersonId = n.PERSON_ID,
									 FirstName = (from p in entities.PERSON
												   where p.PERSON_ID == n.PERSON_ID
												   select p.FIRST_NAME
												   ).FirstOrDefault(),
									 LastName = (from p in entities.PERSON
												  where p.PERSON_ID == n.PERSON_ID
												  select p.LAST_NAME
												   ).FirstOrDefault(),
									 CommentDate = n.COMMENT_DATE
								 }).ToList();

			comments = comments.OrderBy(n => n.CommentDate).ToList();
			rptComments.DataSource = comments;
					rptComments.DataBind();
		}

		string BuildReport(decimal problemCaseId)
		{
			var sb = new StringBuilder();
			sb.AppendFormat("<a href=\"/EHS/EHS_Alert_PDF.aspx?pcid={0}\" target=\"_blank\" style=\"border: none;\">", EncryptionManager.Encrypt(problemCaseId.ToString()));
			sb.Append("<img src=\"/images/ico-download-100x125.png\" alt=\"download\" style=\"border: none;\" /><br /><br />");
			sb.Append("<strong>View Incident Details</strong><br />");
			sb.Append("(Printable page, opens in new window)");
			sb.Append("</a>");
			return sb.ToString();
		}

		protected void rbAddNote_Click(object sender, EventArgs e)
		{
			if (incidentId > 0 && plantId > 0 && personId > 0 && !string.IsNullOrEmpty(rtbNewNote.Text.Trim()))
			{
				var comment = new INCIDENT_VERIFICATION_COMMENT()
				{
					INCIDENT_ID = incidentId,
					PLANT_ID = plantId,
					PERSON_ID = personId,
					COMMENT = rtbNewNote.Text,
					COMMENT_DATE = SessionManager.UserContext.LocalTime
				};
				entities.INCIDENT_VERIFICATION_COMMENT.AddObject(comment);
				entities.SaveChanges();

				PopulateComments();
			}
		}


		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			INCIDENT_VERIFICATION verification = (from iv in entities.INCIDENT_VERIFICATION
												  where
													  iv.INCIDENT_ID == incidentId &&
													  iv.PERSON_ID == personId &&
													  iv.PLANT_ID == plantId
												  select iv).FirstOrDefault();
			verification.HAS_RESPONDED = true;
			entities.SaveChanges();
			CheckSubmitDisabled();

		}

		protected void CheckSubmitDisabled()
		{
			INCIDENT_VERIFICATION verification = (from iv in entities.INCIDENT_VERIFICATION
												  where
													  iv.INCIDENT_ID == incidentId &&
													  iv.PERSON_ID == personId &&
													  iv.PLANT_ID == plantId
												  select iv).FirstOrDefault();

			if (verification.HAS_RESPONDED == true)
			{
				btnSubmit.Text = "Incident Acknowledged.";
				btnSubmit.Enabled = false;
			}
		}

		public string Capitalize(string word)
		{
			string output = "";
			if (word != null && word.Length > 0)
			{
				if (word.Length > 1)
					output = word[0].ToString().ToUpper() + word.Substring(1);
				else
					output = word.ToUpper();
			}
			return output;
		}
	}
}
