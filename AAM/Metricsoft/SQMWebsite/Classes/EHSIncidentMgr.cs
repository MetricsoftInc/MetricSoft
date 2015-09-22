using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public enum IncidentMode
	{
		Incident,
		Prevent
	}


	public class EHSIncidentQuestion
	{
		public decimal QuestionId { get; set; }
		public string QuestionText { get; set; }
		public EHSIncidentQuestionType QuestionType { get; set; }
		public bool HasMultipleChoices { get; set; }
		public List<EHSIncidentAnswerChoice> AnswerChoices { get; set; }
		public bool IsRequired { get; set; }
		public bool IsRequiredClose { get; set; }
		public string HelpText { get; set; }
		public string AnswerText { get; set; }
		public string StandardType { get; set; }
		public List<INCIDENT_QUESTION_CONTROL> QuestionControls { get; set; }
	}

	public class EHSIncidentAnswerChoice
	{
		public string Value { get; set; }
		public bool IsCategoryHeading { get; set; }
	}

	public class EHSIncidentComment
	{
		public string PersonName { get; set; }
		public string CommentText { get; set; }
		public DateTime CommentDate { get; set; }
	}

	public class EHSFormControlStep
	{
		public decimal IncidentId { get; set; }
		public int StepNumber { get; set; }
		public string StepFormName { get; set; }
		public string StepHeadingText { get; set; }
	}

	public class EHSIncidentTimeAccounting
	{
		public int PeriodYear { get; set; }
		public int PeriodMonth { get; set; }
		public decimal LostTime { get; set; }
		public decimal RestrictedTime { get; set; }
		public decimal WorkTime { get; set; }

		public EHSIncidentTimeAccounting CreateNew(int periodYear, int periodMonth)
		{
			this.PeriodYear = periodYear;
			this.PeriodMonth = periodMonth;
			this.LostTime = this.RestrictedTime = this.WorkTime = 0;
			return this;
		}
	}

	public class EHSIncidentData
	{
		public INCIDENT Incident
		{
			get;
			set;
		}
		public PERSON Person
		{
			get;
			set;
		}
		public PERSON RespPerson
		{
			get;
			set;
		}
		public PLANT Plant
		{
			get;
			set;
		}
		public List<INCIDENT_QUESTION> TopicList
		{
			get;
			set;
		}
		public List<INCIDENT_ANSWER> EntryList
		{
			get;
			set;
		}
		public PROB_CASE ProbCase
		{
			get;
			set;
		}
		public List<ATTACHMENT> AttachList
		{
			get;
			set;
		}
		public string Status
		{
			get;
			set;
		}
		public string DeriveStatus()
		{
			this.Status = "A"; // open;

			if (this.Incident.ISSUE_TYPE_ID == 13)
			{
				if (this.EntryList.Where(l => l.INCIDENT_QUESTION_ID == (int)EHSQuestionId.CorrectiveActionsStatus && l.ANSWER_VALUE == "In Progress").Count() > 0)
					this.Status = "P";  // in-progress
				else if (this.EntryList.Where(l => l.INCIDENT_QUESTION_ID == (int)EHSQuestionId.CorrectiveActionsStatus && l.ANSWER_VALUE == "Closed").Count() > 0)
					this.Status = "C";  // actions complete
				//if (this.Incident.CLOSE_DATE.HasValue && !this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue)
				//    this.Status = "C";  // actions complete
				if (this.Incident.CLOSE_DATE.HasValue && this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue)
				{
					this.Status = "U";  // audited and closed
					if (this.EntryList.Where(l => l.INCIDENT_QUESTION_ID == (int)EHSQuestionId.FinalAuditStepResolved && l.ANSWER_VALUE.ToLower().Contains("fund")).Count() > 0)
						this.Status = "F";
				}
			}
			else
			{
				if (this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue && this.Incident.CLOSE_DATE.HasValue)
					this.Status = "C";  // incident closed
				else if (this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue && this.Incident.CLOSE_DATE_8D.HasValue)
					this.Status = "C8";  // incident and 8D closed
				else if (this.Incident.CLOSE_DATE.HasValue)
					this.Status = "N";
				else if (this.Incident.CLOSE_DATE_8D.HasValue)
					this.Status = "N";
			}

			return this.Status;
		}
		public int DaysOpen
		{
			get;
			set;
		}
		public int DaysToClose
		{
			get;
			set;
		}
		public int DaysElapsed()
		{
			int days = 0;
			if (this.Incident.ISSUE_TYPE_ID == 13)
			{
				if (this.Incident.CLOSE_DATE_DATA_COMPLETE.HasValue)
				{
					DateTime closeDT = Convert.ToDateTime(this.Incident.CLOSE_DATE_DATA_COMPLETE);
					days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract((DateTime)this.Incident.CREATE_DT).TotalDays));
					this.DaysOpen = 0;
				}
				else
				{
					days = this.DaysOpen = (int)Math.Abs(Math.Truncate(DateTime.Now.Subtract((DateTime)this.Incident.CREATE_DT).TotalDays));
					this.DaysToClose = 0;
				}
			}
			else
			{
				if (this.Incident.CLOSE_DATE.HasValue)
				{
					DateTime closeDT = Convert.ToDateTime(this.Incident.CLOSE_DATE);
					days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract(this.Incident.INCIDENT_DT).TotalDays));
					this.DaysOpen = 0;
				}
				else
				{
					days = this.DaysOpen = (int)Math.Abs(Math.Truncate(DateTime.Now.Subtract(this.Incident.INCIDENT_DT).TotalDays));
					this.DaysToClose = 0;
				}
			}

			return days;
		}
	}

	public static class EHSIncidentMgr
	{
		public static INCIDENT SelectIncidentById(PSsqmEntities entities, decimal incidentId)
		{
			return (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
		}

		public static decimal SelectIncidentTypeIdByIncidentId(decimal incidentId)
		{
			decimal? incidentTypeId;
			var entities = new PSsqmEntities();
			incidentTypeId = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i.ISSUE_TYPE_ID).FirstOrDefault();
			if (incidentTypeId == null)
				incidentTypeId = 0;
			return (decimal)incidentTypeId;
		}

		public static string SelectIncidentTypeByIncidentId(decimal incidentId)
		{
			string incidentType = "";
			var entities = new PSsqmEntities();
			decimal incidentTypeId = SelectIncidentTypeIdByIncidentId(incidentId);
			incidentType = (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == incidentTypeId select it.TITLE).FirstOrDefault();
			return incidentType;
		}

		public static decimal SelectProblemCaseIdByIncidentId(decimal incidentId)
		{
			decimal? problemCaseId;
			var entities = new PSsqmEntities();
			problemCaseId = (from po in entities.PROB_OCCUR where po.INCIDENT_ID == incidentId select po.PROBCASE_ID).FirstOrDefault();
			if (problemCaseId == null)
				problemCaseId = 0;
			return (decimal)problemCaseId;
		}

		public static string SelectBaseFormNameByIncidentTypeId(decimal incidentTypeId)
		{
			string baseFormName = "";
			var entities = new PSsqmEntities();
			baseFormName = (from itc in entities.INCFORM_TYPE_CONTROL 
							where itc.INCIDENT_TYPE_ID == incidentTypeId && itc.STEP_NUMBER == 1
							select itc.STEP_FORM).FirstOrDefault(); 
			if (baseFormName == null)
				baseFormName = "";
			return baseFormName;
		}


		public static List<EHSFormControlStep> GetStepsForincidentTypeId(decimal incidentTypeId)
		{
			var formStepList = new List<EHSFormControlStep>();

			try
			{
				var entities = new PSsqmEntities();
				formStepList = (from itc in entities.INCFORM_TYPE_CONTROL
								where itc.INCIDENT_TYPE_ID == incidentTypeId
								select new EHSFormControlStep()
								{	IncidentId = itc.INCIDENT_TYPE_ID,
									StepNumber = itc.STEP_NUMBER,
									StepFormName = itc.STEP_FORM,
									StepHeadingText = itc.STEP_HEADING_TEXT
								}).ToList();
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return formStepList;
		}


		public static List<INCIDENT_TYPE> SelectIncidentTypeList(decimal companyId)
		{
			var incidentTypeList = new List<INCIDENT_TYPE>();

			try
			{
				var entities = new PSsqmEntities();

				if (companyId > 0)
					incidentTypeList = (from itc in entities.INCIDENT_TYPE_COMPANY
										join it in entities.INCIDENT_TYPE on itc.INCIDENT_TYPE_ID equals it.INCIDENT_TYPE_ID
										where itc.COMPANY_ID == companyId && itc.STATUS != "I"  && itc.SORT_ORDER < 1000
										orderby it.TITLE
										select it).ToList();
				else
				{
					incidentTypeList = (from itc in entities.INCIDENT_TYPE 
										where itc.STATUS  != "I" 
										orderby itc.TITLE
										select itc).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return incidentTypeList;
		}

		public static List<INCIDENT_TYPE> SelectPreventativeTypeList(decimal companyId)
		{
			var preventativeTypeList = new List<INCIDENT_TYPE>();

			try
			{
				var entities = new PSsqmEntities();

				preventativeTypeList = (from itc in entities.INCIDENT_TYPE_COMPANY
										join it in entities.INCIDENT_TYPE on itc.INCIDENT_TYPE_ID equals it.INCIDENT_TYPE_ID
										where itc.COMPANY_ID == companyId && itc.SORT_ORDER >= 1000
										orderby it.TITLE
										select it).ToList();
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return preventativeTypeList;
		}

		public static bool CanUpdateIncident(INCIDENT incident, bool IsEditContext, SysPriv privNeeded)
		{
			bool canUpdate = false;

			if (IsEditContext)
			{
				if ((incident != null  && incident.CREATE_PERSON == SessionManager.UserContext.Person.PERSON_ID) || SessionManager.CheckUserPrivilege(privNeeded, SysScope.incident))
				{
					canUpdate = true;
				}
			}
			else  // assume edit context will be false for new incidents an any user can create/save a new incident
			{
				canUpdate = true;
			}

			return canUpdate;
		}


		/// <summary>
		/// Select a list of all EHS incidents by company
		/// </summary>
		public static List<INCIDENT> SelectIncidents(decimal companyId, List<decimal> plantIdList)
		{
			var incidents = new List<INCIDENT>();

			try
			{
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					incidents = (from i in entities.INCIDENT
								 where i.INCIDENT_TYPE.ToUpper() == "EHS"
								 orderby i.INCIDENT_ID descending
								 select i).ToList();
				}
				else
				{
					incidents = (from i in entities.INCIDENT
								 where i.INCIDENT_TYPE.ToUpper() == "EHS"
									 && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
								 orderby i.INCIDENT_ID descending
								 select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return incidents;
		}


		public static List<INCIDENT> SelectInjuryIllnessIncidents(decimal plantId, DateTime date)
		{
			var incidents = new List<INCIDENT>();

			try
			{
				var entities = new PSsqmEntities();
				incidents = (from i in entities.INCIDENT
							 where
								 i.INCIDENT_TYPE.ToUpper() == "EHS" &&
								 i.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness &&
								 i.DETECT_PLANT_ID == plantId &&
								 i.INCIDENT_DT.Year == date.Year && i.INCIDENT_DT.Month == date.Month
							 select i).ToList();

			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return incidents;
		}

		/// <summary>
		/// Select a list of open EHS incidents by company
		/// </summary>
		public static List<INCIDENT> SelectOpenIncidents(decimal companyId, List<decimal> plantIdList)
		{
			var incidents = new List<INCIDENT>();

			try
			{
				var date1900 = DateTime.Parse("01/01/1900 00:00:00");
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					incidents = (from i in entities.INCIDENT
								 where i.INCIDENT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
								 orderby i.INCIDENT_ID descending
								 select i).ToList();
				}
				else
				{
					incidents = (from i in entities.INCIDENT
								 where i.INCIDENT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
								 && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
								 orderby i.INCIDENT_ID descending
								 select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return incidents;
		}

		public static void CloseIncident(decimal incidentId)
		{
			var entities = new PSsqmEntities();

			var incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.CLOSE_DATE = DateTime.Now;
				entities.SaveChanges();
			}
		}

		/// <summary>
		/// Returns boolean indicating whether 8D should be selected for incident type by default
		/// </summary>
		public static bool IsTypeDefault8D(decimal selectedTypeId)
		{
			var entities = new PSsqmEntities();
			return (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == selectedTypeId select it.DEFAULT_8D).FirstOrDefault();
		}

		/// <summary>
		/// Returns boolean indicating whether a custom form should be used for the incident type
		/// </summary>
		public static bool IsUseCustomForm(decimal selectedTypeId)
		{
			var entities = new PSsqmEntities();
			return (from it in entities.INCIDENT_TYPE where it.INCIDENT_TYPE_ID == selectedTypeId select it.USE_CUSTOM_FORM).FirstOrDefault();
		}

		public static bool EnableNativeLangQuestion(string nlsLanguage)
		{
			return string.IsNullOrEmpty(nlsLanguage) || nlsLanguage.Contains("en") ? false : true;
		}

		public static string IncidentQuestionText(INCIDENT_QUESTION question, string nlsLanguage)
		{
			string text;

			if (question.INCIDENT_QUESTION_LANG != null && question.INCIDENT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).FirstOrDefault() != null)
			{
				text = question.INCIDENT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).First().LANG_TEXT;
			}
			else 
			{
				text = question.QUESTION_TEXT;
			}

			return text;
		}

		/// <summary>
		/// Select a list of all incident questions by company and incident type
		/// </summary>
		public static List<EHSIncidentQuestion> SelectIncidentQuestionList(decimal incidentTypeId, decimal companyId, int step)
		{
			var questionList = new List<EHSIncidentQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var activeQuestionList = (from q in entities.INCIDENT_TYPE_COMPANY_QUESTION
										  where q.INCIDENT_TYPE_ID == incidentTypeId && q.COMPANY_ID == companyId && q.STEP == step
										  orderby q.SORT_ORDER
										  select q
								).ToList();

				foreach (var aq in activeQuestionList)
				{
					var questionInfo = (from qi in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG")
										where qi.INCIDENT_QUESTION_ID == aq.INCIDENT_QUESTION_ID
										select qi).FirstOrDefault();

					var typeInfo = (from ti in entities.INCIDENT_QUESTION_TYPE
									where questionInfo.INCIDENT_QUESTION_TYPE_ID == ti.INCIDENT_QUESTION_TYPE_ID
									select ti).FirstOrDefault();

					var newQuestion = new EHSIncidentQuestion()
					{
						QuestionId = questionInfo.INCIDENT_QUESTION_ID,
						//QuestionText = questionInfo.QUESTION_TEXT,
						QuestionText = IncidentQuestionText(questionInfo, SessionManager.SessionContext.Language().NLS_LANGUAGE),
						QuestionType = (EHSIncidentQuestionType)questionInfo.INCIDENT_QUESTION_TYPE_ID,
						HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
						IsRequired = questionInfo.IS_REQUIRED,
						IsRequiredClose = questionInfo.IS_REQUIRED_CLOSE,
						HelpText = questionInfo.HELP_TEXT,
						StandardType = questionInfo.STANDARD_TYPE
					};

					if (newQuestion.HasMultipleChoices)
					{
						List<EHSIncidentAnswerChoice> choices = (from qc in entities.INCIDENT_QUESTION_CHOICE
																 where qc.INCIDENT_QUESTION_ID == questionInfo.INCIDENT_QUESTION_ID
																 orderby qc.SORT_ORDER
																 select new EHSIncidentAnswerChoice
																 {
																	 Value = qc.QUESTION_CHOICE_VALUE,
																	 IsCategoryHeading = qc.IS_CATEGORY_HEADING
																 }).ToList();
						if (choices.Count > 0)
							newQuestion.AnswerChoices = choices;
					}

					// Question control logic
					newQuestion.QuestionControls = (from qc in entities.INCIDENT_QUESTION_CONTROL
													where qc.INCIDENT_TYPE_ID == incidentTypeId &&
													qc.COMPANY_ID == companyId &&
													qc.INCIDENT_QUESTION_ID == newQuestion.QuestionId
													orderby qc.PROCESS_ORDER
													select qc).ToList();

					questionList.Add(newQuestion);
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return questionList;
		}

		/// <summary>
		/// Select a list of all possible incident questions
		/// </summary>
		public static List<EHSIncidentQuestion> SelectIncidentQuestionList()
		{
			var questionList = new List<EHSIncidentQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var allQuestions = (from q in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG") select q).ToList();

				foreach (var q in allQuestions)
				{

					var typeInfo = (from ti in entities.INCIDENT_QUESTION_TYPE
									where q.INCIDENT_QUESTION_TYPE_ID == ti.INCIDENT_QUESTION_TYPE_ID
									select ti).FirstOrDefault();

					var newQuestion = new EHSIncidentQuestion()
					{
						QuestionId = q.INCIDENT_QUESTION_ID,
						QuestionText = q.QUESTION_TEXT,
						QuestionType = (EHSIncidentQuestionType)q.INCIDENT_QUESTION_TYPE_ID,
						HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
						HelpText = q.HELP_TEXT
					};

					if (newQuestion.HasMultipleChoices)
					{
						List<EHSIncidentAnswerChoice> choices = (from qc in entities.INCIDENT_QUESTION_CHOICE
																 where qc.INCIDENT_QUESTION_ID == q.INCIDENT_QUESTION_ID
																 orderby qc.SORT_ORDER
																 select new EHSIncidentAnswerChoice
																 {
																	 Value = qc.QUESTION_CHOICE_VALUE,
																	 IsCategoryHeading = qc.IS_CATEGORY_HEADING
																 }).ToList();
						if (choices.Count > 0)
							newQuestion.AnswerChoices = choices;
					}
					questionList.Add(newQuestion);
				}

				questionList.OrderBy(field => field.QuestionText);
				questionList.OrderBy(field => field.QuestionType);

			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return questionList;
		}

		public static List<INCIDENT_QUESTION> SelectIncidentQuestionList(decimal[] questionIds)
		{
			List<INCIDENT_QUESTION> topicList = new List<INCIDENT_QUESTION>();
			try
			{
				var entities = new PSsqmEntities();
				topicList = (from q in entities.INCIDENT_QUESTION.Include("INCIDENT_QUESTION_LANG") where questionIds.Contains(q.INCIDENT_QUESTION_ID) select q).ToList();
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return topicList;
		}

		public static decimal SelectIncidentLocationIdByIncidentId(decimal incidentId)
		{
			var entities = new PSsqmEntities();

			decimal? locationId = (from i in entities.INCIDENT
								   where i.INCIDENT_ID == incidentId
								   select i.DETECT_PLANT_ID).FirstOrDefault();

			locationId = locationId ?? 0;

			return (decimal)locationId;
		}

		public static string SelectIncidentLocationNameByIncidentId(decimal incidentId)
		{
			string locationName = "";

			decimal locationId = SelectIncidentLocationIdByIncidentId(incidentId);
			if (locationId > 0)
				locationName = SelectPlantNameById(locationId);

			return locationName;
		}

		public static string SelectPlantNameById(decimal plantId)
		{
			var entities = new PSsqmEntities();
			return (from p in entities.PLANT where p.PLANT_ID == plantId select p.PLANT_NAME).FirstOrDefault();
		}

		public static List<decimal> SelectPlantIdsByCompanyId(decimal companyId)
		{
			var entities = new PSsqmEntities();
			return (from p in entities.PLANT where p.COMPANY_ID == companyId orderby p.PLANT_NAME select p.PLANT_ID).ToList();
		}

		public static List<PERSON> SelectIncidentPersonList(decimal incidentId)
		{
			var personSelectList = new List<PERSON>();
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);
			decimal companyId = 0;
			if (incident.DETECT_COMPANY_ID != null)
				companyId = incident.DETECT_COMPANY_ID;

			// start with all data originators for the company
			List<PERSON> personList = SQMModelMgr.SelectPersonList(companyId, 0, true, false).Where(l => l.ROLE <= 300).OrderBy(p => p.LAST_NAME).ToList();
			personList = SQMModelMgr.FilterPersonListByAppContext(personList, "EHS");
			// limit the list to those people having access to the plant where the incident (if defined) occurred
			if (incident != null)
			{
				foreach (PERSON person in personList)
				{
					if (SQMModelMgr.PersonPlantAccess(person, (decimal)incident.DETECT_PLANT_ID) || (incident.RESP_PLANT_ID.HasValue && SQMModelMgr.PersonPlantAccess(person, (decimal)incident.RESP_PLANT_ID)))
						personSelectList.Add(person);
				}
			}

			personSelectList = personSelectList.OrderBy(p => p.FIRST_NAME).ToList();
			personSelectList = personSelectList.OrderBy(p => p.LAST_NAME).ToList();

			return personSelectList;
		}


		public static List<PERSON> SelectCompanyPersonList(decimal companyId)
		{
			var personList = new List<PERSON>();

			// select admins for the company with EHS access
			personList = SQMModelMgr.SelectPersonList(companyId, 0, true, false).Where(l => l.ROLE <= 300).ToList();
			personList = SQMModelMgr.FilterPersonListByAppContext(personList, "EHS");

			personList = personList.OrderBy(p => p.FIRST_NAME).ToList();
			personList = personList.OrderBy(p => p.LAST_NAME).ToList();

			return personList;
		}

		public static List<PERSON> SelectEhsPeopleAtPlant(decimal plantId)
		{
			var people = new List<PERSON>();
			PSsqmEntities entities = new PSsqmEntities();

			people = (from p in entities.PERSON
					  join pa in entities.PERSON_ACCESS on p.PERSON_ID equals pa.PERSON_ID
					  where p.PLANT_ID == plantId
					  && pa.ACCESS_PROD == "EHS"
					  select p).Distinct().ToList();

			return people;
		}

		public static List<PERSON> SelectEhsAdminsAtPlant(decimal plantId)
		{
			var people = new List<PERSON>();

			people = SelectEhsPeopleAtPlant(plantId);
			people = (from p in people where p.ROLE <= 100 select p).ToList(); // Filter by company admins

			return people;
		}

		public static List<PERSON> SelectEhsDataOriginatorsAtPlant(decimal plantId)
		{
			var people = new List<PERSON>();

			people = SelectEhsPeopleAtPlant(plantId);
			people = (from p in people where p.ROLE <= 300 select p).ToList(); // Filter by data originators

			return people;
		}

		public static List<PERSON> SelectDataOriginatorAdditionalPlantAccess(decimal plantId)
		{
			var people = new List<PERSON>();
			PSsqmEntities entities = new PSsqmEntities();

			var allAdmins = (from p in entities.PERSON where p.ROLE <= 300 select p).ToList();
			foreach (var admin in allAdmins)
			{
				try
				{
					if (admin.NEW_LOCATION_CD != null)
					{
						var plants = admin.NEW_LOCATION_CD.Split(',');
						foreach (string locPlantId in plants)
						{
							if (!string.IsNullOrEmpty(locPlantId))
							{
								decimal thisId = 0;
								Decimal.TryParse(locPlantId, out thisId);
								if (thisId > 0 && thisId == plantId)
									people.Add(admin);
							}
						}
					}
				}
				catch
				{
				}
			}


			return people;
		}

		public static List<EHSIncidentComment> SelectIncidentComments(decimal incidentId, decimal plantId)
		{
			var comments = new List<EHSIncidentComment>();
			PSsqmEntities entities = new PSsqmEntities();

			comments = (from c in entities.INCIDENT_VERIFICATION_COMMENT
						join p in entities.PERSON on c.PERSON_ID equals p.PERSON_ID
						where c.INCIDENT_ID == incidentId && c.PLANT_ID == plantId
						select new EHSIncidentComment()
						{
							PersonName = p.FIRST_NAME + " " + p.LAST_NAME,
							CommentText = c.COMMENT,
							CommentDate = (DateTime)c.COMMENT_DATE
						}).ToList();

			return comments;
		}

		public static int AttachmentCount(decimal incidentId)
		{
			int count = 0;

			using (PSsqmEntities entities = new PSsqmEntities())
			{
				count = (from a in entities.ATTACHMENT where (a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId) select a).Count();
			}

			return count;
		}

		public static List<INCFORM_ROOT5Y> GetRootCauseList(decimal incidentId)
		{

			PSsqmEntities entities = new PSsqmEntities();
			var rootcauses = new List<INCFORM_ROOT5Y>();

			int minRowsThisForm = 5;

			rootcauses = (from c in entities.INCFORM_ROOT5Y
						where c.INCIDENT_ID == incidentId
						  select c).ToList();

			int itemsNeeded = 0;
			if (rootcauses.Count() < minRowsThisForm)
				itemsNeeded = minRowsThisForm - rootcauses.Count();
				
				INCFORM_ROOT5Y rootcause = null;

				int seq = rootcauses.Count();

				for (int i = 1; i < itemsNeeded + 1; i++)
				{
					rootcause = new INCFORM_ROOT5Y();

					seq = seq + 1;
					rootcause.ITEM_SEQ = seq;
					rootcause.ITEM_DESCRIPTION = "";

					rootcauses.Add(rootcause);
				}
			
			return rootcauses;
		}



		public static List<INCFORM_CONTAIN> GetContainmentList(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();
			var containments = new List<INCFORM_CONTAIN>();

			int minRowsThisForm = 1;

			containments = (from c in entities.INCFORM_CONTAIN
						  where c.INCIDENT_ID == incidentId
						  select c).ToList();

			int itemsNeeded = 0;
			if (containments.Count() < minRowsThisForm)
				itemsNeeded = minRowsThisForm -containments.Count();

				int seq = containments.Count(); ;
				INCFORM_CONTAIN contain = null;

				for (int i = 1; i < itemsNeeded + 1; i++)
				{
					contain = new INCFORM_CONTAIN();
			
					seq = seq + 1;
					contain.ITEM_SEQ = seq;
					contain.ITEM_DESCRIPTION = "";
					contain.ASSIGNED_PERSON = "";
					contain.START_DATE = DateTime.Now;
					contain.COMPLETION_DATE = null;
					contain.IsCompleted = false;

					containments.Add(contain);
				}

			return containments;
		}

		public static List<INCFORM_WITNESS> GetWitnessList(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();
			var witnesses = new List<INCFORM_WITNESS>();

			int minRowsThisForm = 1;

			witnesses = (from w in entities.INCFORM_WITNESS
							where w.INCIDENT_ID == incidentId
							select w).ToList();

			int itemsNeeded = 0;
			if (witnesses.Count() < minRowsThisForm)
				itemsNeeded = minRowsThisForm - witnesses.Count();

			int seq = witnesses.Count(); ;
			INCFORM_WITNESS witness = null;

			for (int i = 1; i < itemsNeeded + 1; i++)
			{
				witness = new INCFORM_WITNESS();

				seq = seq + 1;
				witness.WITNESS_NO = seq;
				witness.WITNESS_NAME = "";
				witness.WITNESS_STATEMENT = "";

				witnesses.Add(witness);
			}

			return witnesses;
		}


		public static List<INCFORM_ACTION> GetFinalActionList(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();
			var actions = new List<INCFORM_ACTION>();

			int minRowsThisForm = 1;

			actions = (from c in entities.INCFORM_ACTION
							where c.INCIDENT_ID == incidentId
							select c).ToList();

			int itemsNeeded = 0;
			if (actions.Count() < minRowsThisForm)
				itemsNeeded = minRowsThisForm - actions.Count();

				int seq = actions.Count(); ;
				INCFORM_ACTION action = null;

				for (int i = 1; i < itemsNeeded + 1; i++)
				{
					action = new INCFORM_ACTION();

					seq = seq + 1;
					action.ITEM_SEQ = seq;
					action.ITEM_DESCRIPTION = "";
					action.ASSIGNED_PERSON = "";
					action.START_DATE = DateTime.Now;
					action.COMPLETION_DATE = null;
					action.IsCompleted = false;

					actions.Add(action);
				}

			return actions;
		}

		public static List<INCFORM_LOSTTIME_HIST> GetLostTimeList(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();
			var losttimelist = new List<INCFORM_LOSTTIME_HIST>();

			int minRowsThisForm = 1;

			losttimelist = (from c in entities.INCFORM_LOSTTIME_HIST
					   where c.INCIDENT_ID == incidentId
					   select c).OrderBy(l=> l.BEGIN_DT).ToList();

			int itemsNeeded = 0;
			if (losttimelist.Count() < minRowsThisForm)
				itemsNeeded = minRowsThisForm - losttimelist.Count();

			int seq = losttimelist.Count(); ;
			INCFORM_LOSTTIME_HIST losttime = null;

			for (int i = 1; i < itemsNeeded + 1; i++)
			{
				losttime = new INCFORM_LOSTTIME_HIST();

				losttime.ITEM_DESCRIPTION = "";
				losttime.WORK_STATUS = "";
				losttime.BEGIN_DT = null;
				losttime.NEXT_MEDAPPT_DT = null;
				losttime.RETURN_EXPECTED_DT = null;
				losttime.RETURN_TOWORK_DT = null;

				losttimelist.Add(losttime);
			}

			return losttimelist;
		}

		public static List<EHSIncidentTimeAccounting> CalculateWorkStatusAccounting(PSsqmEntities ctx, decimal incidentID, DateTime ? fromDate, DateTime ? toDate)
		{
			List<EHSIncidentTimeAccounting> periodList = new List<EHSIncidentTimeAccounting>();

			List<INCFORM_LOSTTIME_HIST> histList = (from h in ctx.INCFORM_LOSTTIME_HIST
					   where h.INCIDENT_ID == incidentID
					   select h).OrderBy(l=> l.BEGIN_DT).ToList();

			if (histList == null || histList.Count == 0)
			{
				return periodList;
			}

			// determine incident time span
			// assume incident is still open and extend timespan to NOW if last status was not return to work
			DateTime effFromDate = Convert.ToDateTime(histList.First().BEGIN_DT);
			DateTime effDate = effFromDate;
			string workStatus = histList.First().WORK_STATUS;
			DateTime effToDate = histList.Last().WORK_STATUS == "02" ? Convert.ToDateTime(histList.Last().BEGIN_DT) : DateTime.Now;
			while (effDate <= effToDate)
			{
				periodList.Add(new EHSIncidentTimeAccounting().CreateNew(effDate.Year, effDate.Month));
				effDate = effDate.AddMonths(1);
			}

			// accumulate work status times per period
			EHSIncidentTimeAccounting period;
			foreach (INCFORM_LOSTTIME_HIST histItem in histList)
			{
				effDate = Convert.ToDateTime(histItem.BEGIN_DT);
				int ndays = Convert.ToInt32((effDate - effFromDate).TotalDays);
				for (int n = 0; n < ndays; n++)
				{
					period = periodList.Where(p => p.PeriodYear == effFromDate.Year && p.PeriodMonth == effFromDate.Month).FirstOrDefault();
					switch (workStatus)
					{
						case "01":
							++period.LostTime;
							break;
						case "03":
							++period.RestrictedTime;
							break;
						default:
							++period.WorkTime;
							break;
					}
					effFromDate = effFromDate.AddDays(1);
				}
				effFromDate = effDate;
				workStatus = histItem.WORK_STATUS;
			}

			if (fromDate.HasValue && toDate.HasValue)
			{	// return time slice, if specified
				periodList = periodList.Where(l => new DateTime(l.PeriodYear, l.PeriodMonth, 1) >= (DateTime)fromDate && new DateTime(l.PeriodYear, l.PeriodMonth, DateTime.DaysInMonth(l.PeriodYear, l.PeriodMonth)) <= (DateTime)toDate).ToList();
			}

			return periodList;
		}

		public static EHSIncidentTimeAccounting CalculateWorkStatusSummary(List<EHSIncidentTimeAccounting> periodList)
		{
			EHSIncidentTimeAccounting workStatusSummary = new EHSIncidentTimeAccounting().CreateNew(0, 0);

			foreach (EHSIncidentTimeAccounting period in periodList)
			{
				workStatusSummary.LostTime += period.LostTime;
				workStatusSummary.RestrictedTime += period.RestrictedTime;
				workStatusSummary.WorkTime += period.WorkTime;
			}

			return workStatusSummary;
		}

		public static List<INCFORM_APPROVAL> GetApprovalList(decimal incidentId)
		{

			PSsqmEntities entities = new PSsqmEntities();

			SETTINGS sets = SQMSettings.GetSetting("EHS", "INCIDENT_APPROVALS");
			List<XLAT> XLATList = SQMBasePage.SelectXLATList(new string[1] {"INCIDENT_APPROVALS"});

			var approvals = new List<INCFORM_APPROVAL>();
			approvals = (from c in entities.INCFORM_APPROVAL
						  where c.INCIDENT_ID == incidentId
						  select c).ToList();

			foreach (string approveLevel in sets.VALUE.Split(','))
			{
				if (approvals.Where(l => l.ITEM_SEQ.ToString() == approveLevel).FirstOrDefault() == null)
				{
					INCFORM_APPROVAL approval = new INCFORM_APPROVAL();
					approval.INCIDENT_APPROVAL_ID = incidentId;
					approval.ITEM_SEQ = Convert.ToInt32(approveLevel);
					approval.APPROVAL_MESSAGE = XLATList.Where(l => l.XLAT_CODE == approveLevel).FirstOrDefault().DESCRIPTION;
					approval.APPROVER_TITLE = XLATList.Where(l => l.XLAT_CODE == approveLevel).FirstOrDefault().DESCRIPTION_SHORT;
					approval.APPROVAL_DATE = DateTime.Now;
					approval.IsAccepted = false;
					approvals.Add(approval);
				}
			}

			return approvals;
		}

		public static void CreateOrUpdateTask(decimal incidentId, decimal responsiblePersonId, int recordTypeId, DateTime dueDate, string taskDescription)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);

			var taskMgr = new TaskStatusMgr();
			taskMgr.Initialize(recordTypeId, incidentId);
			taskMgr.LoadTaskList(recordTypeId, incidentId);
			TASK_STATUS task = taskMgr.FindTask(((int)SysPriv.action).ToString(), "T", responsiblePersonId);

			if (task == null)
			{
				task = taskMgr.CreateTask(((int)SysPriv.action).ToString(), "T", 0, !string.IsNullOrEmpty(taskDescription) ? taskDescription : incident.ISSUE_TYPE, dueDate, responsiblePersonId);
				task.STATUS = ((int)TaskMgr.CalculateTaskStatus(task)).ToString();
				EHSNotificationMgr.NotifyIncidentTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
			}
			else
			{
				task = taskMgr.UpdateTask(task, dueDate, responsiblePersonId, incident.ISSUE_TYPE);
			}

			taskMgr.UpdateTaskList(incidentId);

		}

		public static void SetTaskComplete(decimal incidentId, int recordTypeId)
		{
			var taskMgr = new TaskStatusMgr();
			taskMgr.Initialize(recordTypeId, incidentId);
			taskMgr.LoadTaskList(recordTypeId, incidentId);
			TASK_STATUS task = taskMgr.FindTask(((int)SysPriv.action).ToString(), "T", 0);

			if (task != null)
			{
				taskMgr.UpdateTaskStatus(task, TaskMgr.CalculateTaskStatus(task));
				taskMgr.SetTaskComplete("0", "T", 0, true);
				taskMgr.UpdateTaskList(incidentId);
			}
		}


		public static int DeleteIncident(decimal incidentId)
		{
			int status = 0;
			string delCmd = " IN (" + incidentId + ") ";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					decimal probCaseId = (from po in ctx.PROB_OCCUR where po.INCIDENT_ID == incidentId select po.PROBCASE_ID).FirstOrDefault();

					if (probCaseId > 0)
						status = ProblemCase.DeleteProblemCase(probCaseId);

					List<decimal> attachmentIds = (from a in ctx.ATTACHMENT
												   where a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId
												   select a.ATTACHMENT_ID).ToList();

					if (attachmentIds != null && attachmentIds.Count > 0)
					{
						status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT_FILE WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
						status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
					}

					status = ctx.ExecuteStoreCommand("DELETE FROM PROB_OCCUR WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCIDENT_ANSWER WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCIDENT WHERE INCIDENT_ID" + delCmd);
				}
				catch (Exception ex)
				{
					SQMLogger.LogException(ex);
				}
			}

			return status;
		}


		public static int DeleteCustomIncident(decimal incidentId, decimal typeId)
		{
			int status = 0;
			string delCmd = " IN (" + incidentId + ") ";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CONTAIN WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ACTION WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID" + delCmd);

					string customFormName = (from it in ctx.INCFORM_TYPE_CONTROL where it.INCIDENT_TYPE_ID == typeId && it.STEP_NUMBER == 1 select it.STEP_FORM ).FirstOrDefault();

					if (!String.IsNullOrEmpty(customFormName))
					{
						switch (customFormName)
						{
							case "INCFORM_INJURYILLNESS":
								status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_LOSTTIME_HIST WHERE INCIDENT_ID" + delCmd);
								status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_WITNESS WHERE INCIDENT_ID" + delCmd);
								status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_INJURYILLNESS WHERE INCIDENT_ID" + delCmd);
								break;
						}
					}

					DeleteIncident(incidentId);

				}
				catch (Exception ex)
				{
					SQMLogger.LogException(ex);
				}
			}

			return status;
		}


		public static void TryCloseIncident(decimal incidentId)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);

			if (ShouldIncidentReportClose(incident))
			{
				incident.CLOSE_DATE = DateTime.Now;
				SetTaskComplete(incidentId, 40);
			}
			else
			{
				incident.CLOSE_DATE = null;
			}

			if (ShouldIncidentCloseDataComplete(incident))
				incident.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
			else
				incident.CLOSE_DATE_DATA_COMPLETE = null;

			entities.SaveChanges();
		}


		public static bool ShouldIncidentReportClose(INCIDENT incident)
		{
			var entities = new PSsqmEntities();
			int incidentClosedScore = 0;

			var questionList = SelectIncidentQuestionList((decimal)incident.ISSUE_TYPE_ID, incident.DETECT_COMPANY_ID, 1);
			foreach (var q in questionList)
			{
				string answer = (from a in entities.INCIDENT_ANSWER
								 where a.INCIDENT_ID == incident.INCIDENT_ID && a.INCIDENT_QUESTION_ID == q.QuestionId
								 select a.ANSWER_VALUE).FirstOrDefault();

				if (q.QuestionId == (decimal)EHSQuestionId.CompletionDate && !string.IsNullOrEmpty(answer))
					incidentClosedScore++;

				if (q.QuestionId == (decimal)EHSQuestionId.CompletedBy && !string.IsNullOrEmpty(answer))
					incidentClosedScore++;
			}

			return (incidentClosedScore >= 2);
		}


		public static bool ShouldIncidentCloseDataComplete(INCIDENT incident)
		{
			var entities = new PSsqmEntities();

			var questionList = SelectIncidentQuestionList((decimal)incident.ISSUE_TYPE_ID, incident.DETECT_COMPANY_ID, 0);
			var requiredQuestionIds = (from q in questionList where q.IsRequiredClose == true select q.QuestionId).ToList();

			// Remove lost time date questions from required questions if not a lost time case
			if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
			{
				var lostTimeQuestion = (from q in questionList where q.QuestionId == (decimal)EHSQuestionId.LostTimeCase select q).FirstOrDefault();
				if (lostTimeQuestion != null)
				{
					var answerText = SelectIncidentAnswer(incident, (decimal)EHSQuestionId.LostTimeCase);

					if (answerText != "Yes")
					{
						requiredQuestionIds.Remove((decimal)EHSQuestionId.ExpectedReturnDate);
						requiredQuestionIds.Remove((decimal)EHSQuestionId.ActualReturnDate);
					}
				}
			}

			var answers = (from a in entities.INCIDENT_ANSWER
						   where a.INCIDENT_ID == incident.INCIDENT_ID && requiredQuestionIds.Contains(a.INCIDENT_QUESTION_ID)
						   select a.ANSWER_VALUE).ToList();

			bool shouldClose = true;
			foreach (var a in answers)
			{
				if (string.IsNullOrEmpty(a))
				{
					shouldClose = false;
					break;
				}
			}
			return shouldClose;
		}

		public static void TryClosePrevention(decimal incidentId, decimal personId)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);
			bool shouldUpdateAuditPerson = true;

			if (ShouldPreventionClose(incident))
			{
				incident.CLOSE_DATE = DateTime.Now;
				incident.CLOSE_PERSON = personId;
				SetTaskComplete(incidentId, 45);
				shouldUpdateAuditPerson = false;
			}
			else
			{
				incident.CLOSE_DATE = null;
			}

			if (ShouldPreventionCloseAudited(incident))
			{
				incident.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
				if (shouldUpdateAuditPerson == true)
					incident.AUDIT_PERSON = personId;
			}
			else
			{
				incident.CLOSE_DATE_DATA_COMPLETE = null;
			}

			entities.SaveChanges();
		}

		public static bool ShouldPreventionClose(INCIDENT incident)
		{
			var entities = new PSsqmEntities();
			bool shouldClose = false;

			var questionList = SelectIncidentQuestionList((decimal)incident.ISSUE_TYPE_ID, incident.DETECT_COMPANY_ID, 1);
			foreach (var q in questionList)
			{
				string answer = (from a in entities.INCIDENT_ANSWER
								 where a.INCIDENT_ID == incident.INCIDENT_ID && a.INCIDENT_QUESTION_ID == q.QuestionId
								 select a.ANSWER_VALUE).FirstOrDefault();

				if (q.QuestionId == (decimal)EHSQuestionId.CorrectiveActionsStatus && !string.IsNullOrEmpty(answer))
					if (answer.ToLower() == "closed")
						shouldClose = true;
			}

			return shouldClose;
		}


		public static bool ShouldPreventionCloseAudited(INCIDENT incident)
		{
			bool shouldClose = false;

			var entities = new PSsqmEntities();

			var questionList = SelectIncidentQuestionList((decimal)incident.ISSUE_TYPE_ID, incident.DETECT_COMPANY_ID, 1);
			foreach (var q in questionList)
			{
				string answer = (from a in entities.INCIDENT_ANSWER
								 where a.INCIDENT_ID == incident.INCIDENT_ID && a.INCIDENT_QUESTION_ID == q.QuestionId
								 select a.ANSWER_VALUE).FirstOrDefault();

				if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !string.IsNullOrEmpty(answer))
					if (answer.ToLower() == "yes" || answer.ToLower().Contains("funding"))
						shouldClose = true;
			}

			return shouldClose;
		}

		public static string SelectIncidentAnswer(INCIDENT incident, decimal questionId)
		{
			string answerText = null;
			var entities = new PSsqmEntities();
			answerText = (from a in entities.INCIDENT_ANSWER
						  where a.INCIDENT_ID == incident.INCIDENT_ID &&
						  a.INCIDENT_QUESTION_ID == questionId
						  select a.ANSWER_VALUE).FirstOrDefault();
			return answerText;
		}

		public static string SelectUserNameById(decimal personId)
		{
			var entities = new PSsqmEntities();
			var person = (from p in entities.PERSON
						  where p.PERSON_ID == personId
						  select p).FirstOrDefault();
			return person.LAST_NAME + ", " + person.FIRST_NAME;
		}

		public static INCFORM_POWEROUTAGE SelectPowerOutageDetailsById(PSsqmEntities entities, decimal incidentId)
		{
			return (from po in entities.INCFORM_POWEROUTAGE where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
		}

		public static INCFORM_INJURYILLNESS SelectInjuryIllnessDetailsById(PSsqmEntities entities, decimal incidentId)
		{
			return (from po in entities.INCFORM_INJURYILLNESS where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
		}

		public static INCFORM_LOSTTIME_HIST SelectLostTimeDetailsById(PSsqmEntities entities, decimal incidentId)
		{
			return (from po in entities.INCFORM_LOSTTIME_HIST where po.INCIDENT_ID == incidentId select po).FirstOrDefault();
		}

		public static int GetNextContainSequence(decimal incidentId)
		{
			var entities = new PSsqmEntities();
			var lastcontain = new INCFORM_CONTAIN();
			lastcontain = (from c in entities.INCFORM_CONTAIN where c.INCIDENT_ID == incidentId select c).OrderByDescending(c => c.ITEM_SEQ).First();

			int nextSeq = 1;
			
			if (lastcontain != null)
				nextSeq = lastcontain.ITEM_SEQ + 1;

			return nextSeq;
		}

		public static int GetNextRoot5YSequence(decimal incidentId)
		{
			var entities = new PSsqmEntities();
			var lastroot5y = new INCFORM_ROOT5Y();
			lastroot5y = (from r in entities.INCFORM_ROOT5Y where r.INCIDENT_ID == incidentId select r).OrderByDescending(r => r.ITEM_SEQ).First();

			int nextSeq = 1;

			if (lastroot5y != null)
				nextSeq = lastroot5y.ITEM_SEQ + 1;

			return nextSeq;
		}

		public static int GetNextActionSequence(decimal incidentId)
		{
			var entities = new PSsqmEntities();
			var lastaction = new INCFORM_ACTION();
			lastaction = (from a in entities.INCFORM_ACTION where a.INCIDENT_ID == incidentId select a).OrderByDescending(a => a.ITEM_SEQ).First();

			int nextSeq = 1;

			if (lastaction != null)
				nextSeq = lastaction.ITEM_SEQ + 1;

			return nextSeq;
		}

		public static int GetNextApprovalSequence(decimal incidentId)
		{
			var entities = new PSsqmEntities();
			var lastapproval = new INCFORM_APPROVAL();
			lastapproval = (from ap in entities.INCFORM_APPROVAL where ap.INCIDENT_ID == incidentId select ap).OrderByDescending(ap => ap.ITEM_SEQ).First();

			int nextSeq = 1;

			if (lastapproval != null)
				nextSeq = lastapproval.ITEM_SEQ + 1;

			return nextSeq;
		}


		//OrderByDescending(x => x.Status).First();
	}

	public class EHSMetaData
	{
		public string Language { get; set; }
		public string MetaDataType { get; set; }
		public string Text { get; set; }
		public string TextLong { get; set; }
		public string Value { get; set; }
		public string Status { get; set; }

	}

	public static class EHSMetaDataMgr
	{

		public static List<EHSMetaData> SelectMetaDataList(string metaDataType)
		{
			var entities = new PSsqmEntities();
			var metaList = new List<EHSMetaData>();

			string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
			string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";

			metaList = (from x in entities.XLAT
						where x.XLAT_LANGUAGE == language && x.XLAT_GROUP == metaDataType && x.STATUS == "A"
						orderby x.XLAT_CODE
						select new EHSMetaData()
						{
							Language = x.XLAT_LANGUAGE,
							MetaDataType = x.XLAT_GROUP,
							Text = x.DESCRIPTION_SHORT,
							TextLong = x.DESCRIPTION,
							Value = x.XLAT_CODE,
							Status = x.STATUS
						}).ToList();
			return metaList;
		}
	
	}
}