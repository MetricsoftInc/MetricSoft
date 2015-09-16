using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;

namespace SQM.Website
{
	public partial class Ucl_PreventionLocation : System.Web.UI.UserControl
	{
		static List<PLANT> plantList;
		static List<PERSON> personList;

		public bool IsEditContext { get; set; }
		public decimal IncidentId { get; set; }
		public RadGrid EditControlGrid { get; set; }


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();

			if (IncidentId != null)
			{
				INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
				if (incident != null)
					if (incident.CLOSE_DATE != null && incident.CLOSE_DATE_DATA_COMPLETE != null)
						btnClose.Text = "Reopen Incident Verification";
			}
		}


		// Needed to move javascript to parent page - problem with ajax panel?
		//protected override void OnPreRender(EventArgs e)
		//{
		//	//Page.ClientScript.RegisterClientScriptInclude("PreventionLocation", this.ResolveClientUrl("~/scripts/prevention_location.js"));
		//	base.OnPreRender(e);
		//}


		public void BuildCaseComboBox()
		{
			PSsqmEntities entities = new PSsqmEntities();

			if (rcbCases.Items.Count == 0)
			{
				List<PROB_CASE> caseList = ProblemCase.SelectProblemCaseList(SessionManager.PrimaryCompany().COMPANY_ID, "EHS", "A");
				List<PROB_CASE> userCaseList = ProblemCase.SelectUserCaseList(caseList);

				var userCaseListSorted = userCaseList.OrderByDescending(x => x.PROBCASE_ID);

				rcbCases.Items.Clear();
				rcbCases.Items.Add(new Telerik.Web.UI.RadComboBoxItem("[Select a Problem Case]", ""));
				foreach (PROB_CASE c in userCaseListSorted)
				{
					var incidentId = (from po in entities.PROB_OCCUR where po.PROBCASE_ID == c.PROBCASE_ID select po.INCIDENT_ID).FirstOrDefault();
					string descriptor = string.Format("{0:000000} - {1} ({2})", incidentId, c.DESC_SHORT, ((DateTime)c.CREATE_DT).ToShortDateString());
					rcbCases.Items.Add(new Telerik.Web.UI.RadComboBoxItem(descriptor, c.PROBCASE_ID.ToString()));
				}
			}
		}


		public void PopulateForm()
		{
			PSsqmEntities entities = new PSsqmEntities();

			if (IsEditContext == true)
			{

				INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();

				if (incident != null)
				{
					if (incident.VERIFY_PROBCASE_ID != null && rcbCases.Items.Count > 0)
					{
						if (rcbCases.Items.Count > 0)
							if (rcbCases.Items.FindItemByValue(incident.VERIFY_PROBCASE_ID.ToString()) != null)
								rcbCases.SelectedValue = incident.VERIFY_PROBCASE_ID.ToString();

						tbInstructions.Text = incident.DESCRIPTION;
						rdpDueDate.SelectedDate = incident.INCIDENT_DT;
						btnSubmit.Text = "Save Incident";
						btnClose.Visible = true;					
					}
				}
			}
			else
			{
				tbInstructions.Text = "";
				rdpDueDate.Clear();
				btnSubmit.Text = "Add Incident and Send Notifications";
			}

			plantList = SQMModelMgr.SelectPlantList(entities, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0);

			pnlSelect.Visible = true;
			gvPreventLocationsList.DataSource = plantList;
			gvPreventLocationsList.DataBind();
		}


		protected void rcbCases_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(rcbCases.SelectedValue))
			{
				lblRequired.Visible = false;
				BuildCaseComboBox();
				PopulateForm();

				PSsqmEntities entities = new PSsqmEntities();

				plantList = SQMModelMgr.SelectPlantList(entities, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0);

				pnlSelect.Visible = true;
				gvPreventLocationsList.DataSource = plantList;
				gvPreventLocationsList.DataBind();
			}
			else
			{
				pnlSelect.Visible = false;
			}
		}


		protected void gvPreventLocationsList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{

				decimal plantId = (decimal)gvPreventLocationsList.DataKeys[e.Row.RowIndex].Value;
				RadGrid rg = (RadGrid)e.Row.FindControl("rgPlantContacts");

				var personList = EHSIncidentMgr.SelectEhsPeopleAtPlant(plantId);
				if (personList.Count > 0)
				{
					rg.DataSource = personList;
					rg.DataBind();

					if (IsEditContext == true)
					{
						// Find and select people previously selected
						PSsqmEntities entities = new PSsqmEntities();
						foreach (GridDataItem dataItem in rg.Items)
						{
							decimal personId = (decimal)dataItem.GetDataKeyValue("PERSON_ID");
							if (personId != null)
							{
								var verificationLine = (from iv in entities.INCIDENT_VERIFICATION
														where iv.INCIDENT_ID == IncidentId
															&& iv.PLANT_ID == plantId
															&& iv.PERSON_ID == personId
														select iv).FirstOrDefault();

								if (verificationLine != null)
								{
									dataItem.Selected = true;
									if (verificationLine.HAS_RESPONDED == true)
									{
										Label confirmedLabel = (Label)dataItem.FindControl("lblConfirmed");
										confirmedLabel.Visible = true;
									}
								}
							}
						}
					}
				}
				else
				{
					e.Row.Visible = false;
				}

				var comments = EHSIncidentMgr.SelectIncidentComments(IncidentId, plantId);
				Panel pc = (Panel)e.Row.FindControl("pnlComments");
				RadGrid rgc = (RadGrid)e.Row.FindControl("rgPlantComments");
				if (comments.Count > 0)
				{
					pc.Visible = true;
					rgc.DataSource = comments;
					rgc.DataBind();
				}
			}
		}


		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			var verifications = new List<INCIDENT_VERIFICATION>();

			if (tbInstructions.Text.Trim().Length == 0 || !rdpDueDate.SelectedDate.HasValue)
			{
				lblRequired.Visible = true;
				return;
			}
			lblRequired.Visible = false;

			PSsqmEntities entities = new PSsqmEntities();

			if (IsEditContext == true)
			{
				if (IncidentId != null)
				{
					// Update description, date
					INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
					incident.DESCRIPTION = tbInstructions.Text;
					if (rdpDueDate.SelectedDate.HasValue)
						incident.INCIDENT_DT = (DateTime)rdpDueDate.SelectedDate;
					if (!string.IsNullOrEmpty(rcbCases.SelectedValue))
						incident.VERIFY_PROBCASE_ID = Convert.ToDecimal(rcbCases.SelectedValue);
					entities.SaveChanges();

					// Add notified people and plants to database
					foreach (GridViewRow gvr in gvPreventLocationsList.Rows)
					{
						decimal plantId = (decimal)gvPreventLocationsList.DataKeys[gvr.RowIndex].Value;

						if (plantId != null)
						{
							RadGrid currentGridView = (RadGrid)gvr.FindControl("rgPlantContacts");
							foreach (GridDataItem item in currentGridView.Items)
							{
								decimal personId = (decimal)item.GetDataKeyValue("PERSON_ID");
								if (personId != null)
								{
									var incidentVerification = (from iv in entities.INCIDENT_VERIFICATION
																where iv.INCIDENT_ID == IncidentId &&
																iv.PLANT_ID == plantId &&
																iv.PERSON_ID == personId
																select iv).FirstOrDefault();

									if (item.Selected == true)
									{
										var newVerification = new INCIDENT_VERIFICATION()
										{
											INCIDENT_ID = IncidentId,
											PLANT_ID = plantId,
											PERSON_ID = personId,
											DATE_NOTIFIED = DateTime.Now
										};
										// Add to list to use for emails
										verifications.Add(newVerification);

										// Add to database if it does not exist
										if (incidentVerification == null)
										{
											entities.INCIDENT_VERIFICATION.AddObject(newVerification);
											entities.SaveChanges();
										}
									}
									else
									{
										// Delete if exists
										if (incidentVerification != null)
										{
											entities.INCIDENT_VERIFICATION.DeleteObject(incidentVerification);
											entities.SaveChanges();
										}
									}
								}
							}
						}
					}
				}
			}
			else // Is add context
			{
				decimal verifyProbcaseId = 0;
				if (!string.IsNullOrEmpty(rcbCases.SelectedValue))
					verifyProbcaseId = Convert.ToDecimal(rcbCases.SelectedValue);

				// Add incident to database
				var incident = new INCIDENT()
				{
					DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID,
					DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID,
					DETECT_PLANT_ID = SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID,
					INCIDENT_TYPE = "EHS",
					CREATE_DT = DateTime.Now,
					CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
					DESCRIPTION = tbInstructions.Text,
					CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
					INCIDENT_DT = rdpDueDate.SelectedDate.Value,
					ISSUE_TYPE = "Prevention Verification",
					ISSUE_TYPE_ID = 10,
					VERIFY_PROBCASE_ID = verifyProbcaseId
				};
				entities.INCIDENT.AddObject(incident);
				entities.SaveChanges();
				decimal incidentId = incident.INCIDENT_ID;


				// Add notified people and plants to database
				foreach (GridViewRow gvr in gvPreventLocationsList.Rows)
				{
					decimal plantId = (decimal)gvPreventLocationsList.DataKeys[gvr.RowIndex].Value;

					if (plantId != null)
					{
						RadGrid currentGridView = (RadGrid)gvr.FindControl("rgPlantContacts");
						foreach (GridDataItem item in currentGridView.Items)
						{
							decimal personId = (decimal)item.GetDataKeyValue("PERSON_ID");
							if (personId != null)
							{
								if (item.Selected == true)
								{
									var incidentVerification = new INCIDENT_VERIFICATION()
									{
										INCIDENT_ID = incidentId,
										PLANT_ID = plantId,
										PERSON_ID = personId,
										DATE_NOTIFIED = DateTime.Now
									};
									verifications.Add(incidentVerification);
									entities.INCIDENT_VERIFICATION.AddObject(incidentVerification);
									entities.SaveChanges();
								}
							}
						}
					}
				}
			}

			// Send email(s)

			foreach (var v in verifications)
			{
				var thisVerification = v;
				PERSON emailPerson = (from p in entities.PERSON where p.PERSON_ID == thisVerification.PERSON_ID select p).FirstOrDefault();

				string emailSubject = SessionManager.PrimaryCompany().COMPANY_NAME + " Issue Acknowledgement Notification"; // AW20140129 - use company name variable instead of hard coding.

				string path = "http://" + HttpContext.Current.Request.Url.Authority + "/EHS/EHS_Incident_Verification.aspx";

				path += string.Format("?inid={0}&plid={1}&peid={2}", v.INCIDENT_ID, v.PLANT_ID, emailPerson.PERSON_ID);
				var sb = new StringBuilder();
				sb.AppendLine("<p>You have been sent an issue acknowledgement notification from " + SessionManager.PrimaryCompany().COMPANY_NAME + ".</p>");
				sb.AppendLine();
				sb.AppendLine("<p><b>DETAILS</b></p>");
				sb.AppendLine();
				sb.AppendLine("<p>Date: " + rdpDueDate.SelectedDate.Value.ToShortDateString() + "</p>");
				sb.AppendLine();
				sb.AppendLine("<p>Instructions: " + tbInstructions.Text + "</p>");
				sb.AppendLine();
				sb.AppendLine("<p>Please go here to acknowledge receipt of this issue:<br/>");
				sb.AppendLine("<a href=\"" + path + "\">" + path + "</a></p>");
				sb.AppendLine(); // AW20140129
				sb.AppendLine(); // AW20140129
				sb.AppendLine("Please Do Not Reply To This Message"); // AW20140129

				string emailBody = sb.ToString();
				string emailAddress = emailPerson.EMAIL;
				WebSiteCommon.SendEmail(emailAddress, emailSubject, emailBody, "");

			}

			Response.Redirect("EHS_Incidents.aspx");

		}


		protected void btnClose_Click(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();

			if (IncidentId != null)
			{
				INCIDENT incident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
				if (incident.CLOSE_DATE == null || incident.CLOSE_DATE_DATA_COMPLETE == null)
				{
					incident.CLOSE_DATE = DateTime.Now;
					incident.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
				}
				else
				{
					incident.CLOSE_DATE = null; // Reopen
					incident.CLOSE_DATE_DATA_COMPLETE = null;
				}

				entities.SaveChanges();
			}

			Response.Redirect("EHS_Incidents.aspx");
		}

		public string Capitalize(string word)
		{
			string output = "";
			if (word.Length > 0)
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