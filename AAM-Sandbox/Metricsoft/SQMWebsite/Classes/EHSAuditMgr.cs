﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public enum AuditMode
	{
		Audit,
		Prevent
	}

	public class EHSAuditQuestion
	{
		public decimal QuestionId { get; set; }
		public string QuestionText { get; set; }
		public EHSAuditQuestionType QuestionType { get; set; }
		public bool HasMultipleChoices { get; set; }
		public List<EHSAuditAnswerChoice> AnswerChoices { get; set; }
		public bool IsRequired { get; set; }
		public bool IsRequiredClose { get; set; }
		public string HelpText { get; set; }
		public string AnswerText { get; set; }
		public string StandardType { get; set; }
		public List<AUDIT_QUESTION_CONTROL> QuestionControls { get; set; }
		public decimal TopicId { get; set; }
		public string TopicTitle { get; set; }
		public string AnswerComment { get; set; }
	}

	public class EHSAuditAnswerChoice
	{
		public string Value { get; set; }
		public bool IsCategoryHeading { get; set; }
		public decimal ChoiceWeight { get; set; }
		public bool ChoicePositive { get; set; }
	}

	public class EHSAuditComment
	{
		public string PersonName { get; set; }
		public string CommentText { get; set; }
		public DateTime CommentDate { get; set; }
	}

	public class EHSAuditData
	{
		public AUDIT Audit
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
		public AUDIT_TYPE AuditType
		{
			get;
			set;
		}
		public List<AUDIT_QUESTION> TopicList
		{
			get;
			set;
		}
		public List<AUDIT_ANSWER> EntryList
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

			if (this.Audit.CLOSE_DATE_DATA_COMPLETE.HasValue && this.Audit.CLOSE_DATE.HasValue)
				this.Status = "C";  // audit closed

			DateTime closeDT = Convert.ToDateTime(this.Audit.AUDIT_DT.AddDays(this.AuditType.DAYS_TO_COMPLETE));

			if (closeDT.CompareTo(DateTime.Now) < 0)
				this.Status = "C";

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

			DateTime closeDT = Convert.ToDateTime(this.Audit.AUDIT_DT.AddDays(this.AuditType.DAYS_TO_COMPLETE));

			if (closeDT.CompareTo(DateTime.Now) < 0)
			{
				// date has passed
				//DateTime closeDT = Convert.ToDateTime(this.Audit.AUDIT_DT.AddDays(this.AuditType.DAYS_TO_COMPLETE));
				//days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract(this.Audit.AUDIT_DT).TotalDays));
				days = this.DaysOpen = this.DaysToClose = 0;
			}
			else 
			{
				days = this.DaysOpen = (int)Math.Abs(Math.Truncate(DateTime.Now.Subtract(this.Audit.AUDIT_DT).TotalDays));
				this.DaysToClose = (int)Math.Abs(Math.Truncate(DateTime.Now.Subtract(closeDT).TotalDays));
			}

			return days;
		}
	}

	public class EHSAuditSchedulerData
	{
		public AUDIT_SCHEDULER AuditScheduler
		{
			get;
			set;
		}
		public PLANT Plant
		{
			get;
			set;
		}
		public AUDIT_TYPE AuditType
		{
			get;
			set;
		}
		public PRIVGROUP Privgroup
		{
			get;
			set;
		}
	}

	public static class EHSAuditMgr
	{
		public static AUDIT SelectAuditById(PSsqmEntities entities, decimal auditId)
		{
			return (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
		}

		public static AUDIT SelectAuditForSchedule(decimal plantId, decimal typeId, decimal personId, DateTime date)
		{
			var entities = new PSsqmEntities();
			return (from i in entities.AUDIT where i.DETECT_PLANT_ID == plantId && i.AUDIT_TYPE_ID == typeId && i.AUDIT_PERSON == personId && i.AUDIT_DT == date select i).FirstOrDefault();
		}

		public static decimal SelectAuditTypeIdByAuditId(decimal auditId)
		{
			decimal? auditTypeId;
			var entities = new PSsqmEntities();
			auditTypeId = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i.AUDIT_TYPE_ID).FirstOrDefault();
			if (auditTypeId == null)
				auditTypeId = 0;
			return (decimal)auditTypeId;
		}

		public static string SelectAuditTypeByAuditId(decimal auditId)
		{
			string auditType = "";
			var entities = new PSsqmEntities();
			decimal auditTypeId = SelectAuditTypeIdByAuditId(auditId);
			auditType = (from it in entities.AUDIT_TYPE where it.AUDIT_TYPE_ID == auditTypeId select it.TITLE).FirstOrDefault();
			return auditType;
		}

		public static AUDIT_TYPE SelectAuditTypeById(PSsqmEntities entities, decimal auditTypeId)
		{
			return (from i in entities.AUDIT_TYPE where i.AUDIT_TYPE_ID == auditTypeId select i).FirstOrDefault();
		}

		public static List<AUDIT_TYPE> SelectAuditTypeList(decimal companyId, bool activeOnly)
		{
			var auditTypeList = new List<AUDIT_TYPE>();

			try
			{
				var entities = new PSsqmEntities();

				if (activeOnly)
					auditTypeList = (from itc in entities.AUDIT_TYPE
										where !itc.INACTIVE
										orderby itc.TITLE
										select itc).ToList();
				else
				{
					auditTypeList = (from itc in entities.AUDIT_TYPE
										orderby itc.TITLE
										select itc).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return auditTypeList;
		}

		/// <summary>
		/// Select a list of all EHS audits by company
		/// </summary>
		public static List<AUDIT> SelectAudits(decimal companyId, List<decimal> plantIdList)
		{
			var audits = new List<AUDIT>();

			try
			{
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					audits = (from i in entities.AUDIT
								 where i.AUDIT_TYPE.ToUpper() == "EHS"
								 orderby i.AUDIT_ID descending
								 select i).ToList();
				}
				else
				{
					audits = (from i in entities.AUDIT
								 where i.AUDIT_TYPE.ToUpper() == "EHS"
									 && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
								 orderby i.AUDIT_ID descending
								 select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return audits;
		}

		/// <summary>
		/// Select a list of open EHS audits by company
		/// </summary>
		public static List<AUDIT> SelectOpenAudits(decimal companyId, List<decimal> plantIdList)
		{
			var audits = new List<AUDIT>();

			try
			{
				var date1900 = DateTime.Parse("01/01/1900 00:00:00");
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					audits = (from i in entities.AUDIT
								 where i.AUDIT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
								 orderby i.AUDIT_ID descending
								 select i).ToList();
				}
				else
				{
					audits = (from i in entities.AUDIT
								 where i.AUDIT_TYPE.ToUpper() == "EHS" && (i.CLOSE_DATE == date1900 || i.CLOSE_DATE == null)
								 && plantIdList.Contains((decimal)i.DETECT_PLANT_ID)
								 orderby i.AUDIT_ID descending
								 select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return audits;
		}

		public static void CloseAudit(decimal auditId)
		{
			var entities = new PSsqmEntities();

			var audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			if (audit != null)
			{
				audit.CLOSE_DATE = DateTime.Now;
				entities.SaveChanges();
			}
		}

		public static bool EnableNativeLangQuestion(string nlsLanguage)
		{
			return string.IsNullOrEmpty(nlsLanguage) || nlsLanguage.Contains("en") ? false : true;
		}

		public static string AuditQuestionText(AUDIT_QUESTION question, string nlsLanguage)
		{
			string text;

			if (question.AUDIT_QUESTION_LANG != null && question.AUDIT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).FirstOrDefault() != null)
			{
				text = question.AUDIT_QUESTION_LANG.Where(l => l.NLS_LANGUAGE == nlsLanguage).First().LANG_TEXT;
			}
			else
			{
				text = question.QUESTION_TEXT;
			}

			return text;
		}

		/// <summary>
		/// Select a list of all audit questions by topic 
		/// </summary>
		public static List<EHSAuditQuestion> SelectAuditQuestionList(decimal auditTypeId, decimal auditTopicId, decimal auditId)
		{
			var questionList = new List<EHSAuditQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var activeQuestionList = new List<AUDIT_TYPE_TOPIC_QUESTION>();
				var auditAnswers = new List<decimal>();
				if (auditId > 0)
				{
					auditAnswers = (from a in entities.AUDIT_ANSWER
										where a.AUDIT_ID == auditId
										select a.AUDIT_QUESTION_ID).ToList();
				}
				if (auditTopicId > 0)
				{
					if (auditId == 0)
					{
						// no audit id means add mode, so we only want to get active questions
						activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
											  where q.AUDIT_TYPE_ID == auditTypeId && q.AUDIT_TOPIC_ID == auditTopicId && !q.INACTIVE 
											  orderby q.SORT_ORDER
											  select q
									).ToList();
					}
					else
					{
						// need to only select questions that appear in the specific audit
						activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
											  where q.AUDIT_TYPE_ID == auditTypeId && q.AUDIT_TOPIC_ID == auditTopicId && auditAnswers.Contains(q.AUDIT_QUESTION_ID)
											  orderby q.SORT_ORDER
											  select q
									).ToList();
					}
				}
				else
				{
					if (auditId == 0)
					{
						// no audit id means add mode, so we only want to get active questions
						activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
											  where q.AUDIT_TYPE_ID == auditTypeId && !q.INACTIVE
											  orderby q.AUDIT_TOPIC_ID, q.SORT_ORDER
											  select q
											  ).ToList();
					}
					else
					{
						// need to only select questions that appear in the specific audit
						activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
											  where q.AUDIT_TYPE_ID == auditTypeId && auditAnswers.Contains(q.AUDIT_QUESTION_ID)
											  orderby q.AUDIT_TOPIC_ID, q.SORT_ORDER
											  select q
					  ).ToList();
					}
				}

				foreach (var aq in activeQuestionList)
				{
					var questionInfo = (from qi in entities.AUDIT_QUESTION.Include("AUDIT_QUESTION_LANG")
										where qi.AUDIT_QUESTION_ID == aq.AUDIT_QUESTION_ID
										select qi).FirstOrDefault();

					var typeInfo = (from ti in entities.AUDIT_QUESTION_TYPE
									where questionInfo.AUDIT_QUESTION_TYPE_ID == ti.AUDIT_QUESTION_TYPE_ID
									select ti).FirstOrDefault();

					var topicInfo = (from tp in entities.AUDIT_TOPIC
									 where aq.AUDIT_TOPIC_ID == tp.AUDIT_TOPIC_ID
									select tp).FirstOrDefault();

					var newQuestion = new EHSAuditQuestion()
					{
						QuestionId = questionInfo.AUDIT_QUESTION_ID,
						//QuestionText = questionInfo.QUESTION_TEXT,
						QuestionText = AuditQuestionText(questionInfo, SessionManager.SessionContext.Language().NLS_LANGUAGE),
						QuestionType = (EHSAuditQuestionType)questionInfo.AUDIT_QUESTION_TYPE_ID,
						HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
						IsRequired = questionInfo.IS_REQUIRED,
						IsRequiredClose = questionInfo.IS_REQUIRED_CLOSE,
						HelpText = questionInfo.HELP_TEXT,
						StandardType = questionInfo.STANDARD_TYPE,
						TopicId = topicInfo.AUDIT_TOPIC_ID,
						TopicTitle = topicInfo.TITLE
					};

					if (newQuestion.HasMultipleChoices)
					{
						List<EHSAuditAnswerChoice> choices = (from qc in entities.AUDIT_QUESTION_CHOICE
															  where qc.AUDIT_QUESTION_ID == questionInfo.AUDIT_QUESTION_ID
															  orderby qc.SORT_ORDER
															  select new EHSAuditAnswerChoice
															  {
																  Value = qc.QUESTION_CHOICE_VALUE,
																  IsCategoryHeading = qc.IS_CATEGORY_HEADING,
																  ChoiceWeight = qc.CHOICE_WEIGHT,
																  ChoicePositive = qc.CHOICE_POSITIVE
															  }).ToList();
						if (choices.Count > 0)
							newQuestion.AnswerChoices = choices;
					}

					//// Question control logic
					//newQuestion.QuestionControls = (from qc in entities.AUDIT_QUESTION_CONTROL
					//								where qc.AUDIT_TYPE_ID == auditTypeId &&
					//								qc.AUDIT_TOPIC_ID == auditTopicId &&
					//								qc.AUDIT_QUESTION_ID == newQuestion.QuestionId
					//								orderby qc.PROCESS_ORDER
					//								select qc).ToList();

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
		/// Select a list of all possible audit questions
		/// </summary>
		public static List<EHSAuditQuestion> SelectAuditQuestionList()
		{
			var questionList = new List<EHSAuditQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var allQuestions = (from q in entities.AUDIT_QUESTION.Include("AUDIT_QUESTION_LANG") select q).ToList();

				foreach (var q in allQuestions)
				{

					var typeInfo = (from ti in entities.AUDIT_QUESTION_TYPE
									where q.AUDIT_QUESTION_TYPE_ID == ti.AUDIT_QUESTION_TYPE_ID
									select ti).FirstOrDefault();

					var newQuestion = new EHSAuditQuestion()
					{
						QuestionId = q.AUDIT_QUESTION_ID,
						QuestionText = q.QUESTION_TEXT,
						QuestionType = (EHSAuditQuestionType)q.AUDIT_QUESTION_TYPE_ID,
						HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
						HelpText = q.HELP_TEXT
					};

					if (newQuestion.HasMultipleChoices)
					{
						List<EHSAuditAnswerChoice> choices = (from qc in entities.AUDIT_QUESTION_CHOICE
																 where qc.AUDIT_QUESTION_ID == q.AUDIT_QUESTION_ID
																 orderby qc.SORT_ORDER
																 select new EHSAuditAnswerChoice
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

		public static List<AUDIT_QUESTION> SelectAuditQuestionList(decimal[] questionIds)
		{
			List<AUDIT_QUESTION> topicList = new List<AUDIT_QUESTION>();
			try
			{
				var entities = new PSsqmEntities();
				topicList = (from q in entities.AUDIT_QUESTION.Include("AUDIT_QUESTION_LANG") where questionIds.Contains(q.AUDIT_QUESTION_ID) select q).ToList();
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return topicList;
		}

		public static decimal SelectAuditLocationIdByAuditId(decimal auditId)
		{
			var entities = new PSsqmEntities();

			decimal? locationId = (from i in entities.AUDIT
								   where i.AUDIT_ID == auditId
								   select i.DETECT_PLANT_ID).FirstOrDefault();

			locationId = locationId ?? 0;

			return (decimal)locationId;
		}

		public static string SelectAuditLocationNameByAuditId(decimal auditId)
		{
			string locationName = "";

			decimal locationId = SelectAuditLocationIdByAuditId(auditId);
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

		public static List<PERSON> SelectAuditPersonList(decimal auditId)
		{
			var personSelectList = new List<PERSON>();
			var entities = new PSsqmEntities();

			AUDIT audit = SelectAuditById(entities, auditId);
			decimal companyId = 0;
			if (audit.DETECT_COMPANY_ID != null)
				companyId = audit.DETECT_COMPANY_ID;

			// start with all data originators for the company
			List<PERSON> personList = SQMModelMgr.SelectPersonList(companyId, 0, true, false).Where(l => l.ROLE <= 300).OrderBy(p => p.LAST_NAME).ToList();
			personList = SQMModelMgr.FilterPersonListByAppContext(personList, "EHS");
			// limit the list to those people having access to the plant where the audit (if defined) occurred
			if (audit != null)
			{
				foreach (PERSON person in personList)
				{
					if (SQMModelMgr.PersonPlantAccess(person, (decimal)audit.DETECT_PLANT_ID))
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

		public static List<EHSAuditComment> SelectAuditComments(decimal auditId, decimal plantId)
		{
			var comments = new List<EHSAuditComment>();
			PSsqmEntities entities = new PSsqmEntities();

			comments = (from c in entities.AUDIT_COMMENT
						join p in entities.PERSON on c.PERSON_ID equals p.PERSON_ID
						where c.AUDIT_ID == auditId && c.PLANT_ID == plantId
						select new EHSAuditComment()
						{
							PersonName = p.FIRST_NAME + " " + p.LAST_NAME,
							CommentText = c.COMMENT,
							CommentDate = (DateTime)c.COMMENT_DATE
						}).ToList();

			return comments;
		}

		public static int DeleteAudit(decimal auditId)
		{
			int status = 0;
			string delCmd = " IN (" + auditId + ") ";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					//decimal probCaseId = (from po in ctx.PROB_OCCUR where po.AUDIT_ID == auditId select po.PROBCASE_ID).FirstOrDefault();

					//if (probCaseId > 0)
					//	status = ProblemCase.DeleteProblemCase(probCaseId);

					//List<decimal> attachmentIds = (from a in ctx.ATTACHMENT
					//							   where a.RECORD_TYPE == 40 && a.RECORD_ID == auditId
					//							   select a.ATTACHMENT_ID).ToList();

					//if (attachmentIds != null && attachmentIds.Count > 0)
					//{
					//	status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT_FILE WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
					//	status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
					//}

					//status = ctx.ExecuteStoreCommand("DELETE FROM PROB_OCCUR WHERE AUDIT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM AUDIT_ANSWER WHERE AUDIT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM AUDIT WHERE AUDIT_ID" + delCmd);
				}
				catch (Exception ex)
				{
					SQMLogger.LogException(ex);
				}
			}

			return status;
		}

		public static bool ShouldAuditReportClose(AUDIT audit)
		{
			var entities = new PSsqmEntities();
			int auditClosedScore = 0;

			var questionList = SelectAuditQuestionList((decimal)audit.AUDIT_TYPE_ID, 0, audit.AUDIT_ID);
			foreach (var q in questionList)
			{
				string answer = (from a in entities.AUDIT_ANSWER
								 where a.AUDIT_ID == audit.AUDIT_ID && a.AUDIT_QUESTION_ID == q.QuestionId
								 select a.ANSWER_VALUE).FirstOrDefault();

				if (q.QuestionId == (decimal)EHSQuestionId.CompletionDate && !string.IsNullOrEmpty(answer))
					auditClosedScore++;

				if (q.QuestionId == (decimal)EHSQuestionId.CompletedBy && !string.IsNullOrEmpty(answer))
					auditClosedScore++;
			}

			return (auditClosedScore >= 2);
		}

		public static string SelectAuditAnswer(AUDIT audit, decimal questionId)
		{
			string answerText = null;
			var entities = new PSsqmEntities();
			answerText = (from a in entities.AUDIT_ANSWER
						  where a.AUDIT_ID == audit.AUDIT_ID &&
						  a.AUDIT_QUESTION_ID == questionId
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

		public static AUDIT_SCHEDULER SelectAuditSchedulerById(PSsqmEntities entities, decimal auditScheduleId)
		{
			return (from i in entities.AUDIT_SCHEDULER where i.AUDIT_SCHEDULER_ID == auditScheduleId select i).FirstOrDefault();
		}

		public static decimal SelectAuditTypeIdByAuditScheduleId(decimal auditScheduleId)
		{
			decimal? auditTypeId;
			var entities = new PSsqmEntities();
			auditTypeId = (from i in entities.AUDIT_SCHEDULER where i.AUDIT_SCHEDULER_ID == auditScheduleId select i.AUDIT_TYPE_ID).FirstOrDefault();
			if (auditTypeId == null)
				auditTypeId = 0;
			return (decimal)auditTypeId;
		}

		public static string SelectAuditTypeByAuditScheduleId(decimal auditScheduleId)
		{
			string auditType = "";
			var entities = new PSsqmEntities();
			decimal auditTypeId = SelectAuditTypeIdByAuditScheduleId(auditScheduleId);
			auditType = (from it in entities.AUDIT_TYPE where it.AUDIT_TYPE_ID == auditTypeId select it.TITLE).FirstOrDefault();
			return auditType;
		}

		/// <summary>
		/// Select a list of all EHS audits by company
		/// </summary>
		public static List<AUDIT_SCHEDULER> SelectActiveAuditSchedulers(decimal companyId, List<decimal> plantIdList)
		{
			var auditschedules = new List<AUDIT_SCHEDULER>();

			try
			{
				var entities = new PSsqmEntities();
				if (plantIdList == null)
				{
					auditschedules = (from i in entities.AUDIT_SCHEDULER
							  where i.INACTIVE == false
							  orderby i.AUDIT_SCHEDULER_ID descending
							  select i).ToList();
				}
				else
				{
					auditschedules = (from i in entities.AUDIT_SCHEDULER
									  where i.INACTIVE == false
								  && plantIdList.Contains((decimal)i.PLANT_ID)
							  orderby i.AUDIT_SCHEDULER_ID descending
							  select i).ToList();
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return auditschedules;
		}

		public static int DeleteAuditScheduler(decimal auditScheduleId)
		{
			int status = 0;
			string delCmd = " IN (" + auditScheduleId + ") ";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					status = ctx.ExecuteStoreCommand("DELETE FROM AUDIT_SCHEDULER WHERE AUDIT_SCHEDULER_ID" + delCmd);
				}
				catch (Exception ex)
				{
					SQMLogger.LogException(ex);
				}
			}

			return status;
		}

		public static void CreateOrUpdateTask(decimal auditId, decimal responsiblePersonId, int recordTypeId, DateTime dueDate, string status)
		{
			var entities = new PSsqmEntities();

			AUDIT audit = SelectAuditById(entities, auditId);
			AUDIT_TYPE type = SelectAuditTypeById(entities, audit.AUDIT_TYPE_ID);
			var taskMgr = new TaskStatusMgr();
			taskMgr.Initialize(recordTypeId, auditId);
			taskMgr.LoadTaskList(recordTypeId, auditId);
			TASK_STATUS task = taskMgr.FindTask("0", "T", responsiblePersonId);

			if (task == null)
			{
				task = taskMgr.CreateTask("0", "T", 0, type.TITLE.ToString(), dueDate, responsiblePersonId);
				task.STATUS = ((int)TaskMgr.CalculateTaskStatus(task)).ToString();
			}
			else
			{
				switch (status)
				{
					case "C":
						task.STATUS = ((int)TaskStatus.Complete).ToString();
						taskMgr.SetTaskComplete(task, responsiblePersonId);
						break;
				}
				//task = taskMgr.UpdateTask(task, dueDate, responsiblePersonId, audit.AUDIT_TYPE_ID.ToString());
			}

			taskMgr.UpdateTaskList(auditId);

		}

		public static void DeleteAuditTask(decimal auditId, int recordTypeId)
		{
			var entities = new PSsqmEntities();
			var taskMgr = new TaskStatusMgr();
			taskMgr.DeleteTask(recordTypeId, auditId);
		}

	}
}