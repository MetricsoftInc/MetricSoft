using System;
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

			//if (this.Audit.ISSUE_TYPE_ID == 13)
			//{
			//	if (this.EntryList.Where(l => l.AUDIT_QUESTION_ID == (int)EHSQuestionId.CorrectiveActionsStatus && l.ANSWER_VALUE == "In Progress").Count() > 0)
			//		this.Status = "P";  // in-progress
			//	else if (this.EntryList.Where(l => l.AUDIT_QUESTION_ID == (int)EHSQuestionId.CorrectiveActionsStatus && l.ANSWER_VALUE == "Closed").Count() > 0)
			//		this.Status = "C";  // actions complete
			//	//if (this.Audit.CLOSE_DATE.HasValue && !this.Audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
			//	//    this.Status = "C";  // actions complete
			//	if (this.Audit.CLOSE_DATE.HasValue && this.Audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
			//	{
			//		this.Status = "U";  // audited and closed
			//		if (this.EntryList.Where(l => l.AUDIT_QUESTION_ID == (int)EHSQuestionId.FinalAuditStepResolved && l.ANSWER_VALUE.ToLower().Contains("fund")).Count() > 0)
			//			this.Status = "F";
			//	}
			//}
			//else
			//{
				if (this.Audit.CLOSE_DATE_DATA_COMPLETE.HasValue && this.Audit.CLOSE_DATE.HasValue)
					this.Status = "C";  // audit closed
				else if (this.Audit.CLOSE_DATE.HasValue)
					this.Status = "N";
			//}

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

			if (this.Audit.CLOSE_DATE.HasValue)
			{
				DateTime closeDT = Convert.ToDateTime(this.Audit.CLOSE_DATE);
				days = this.DaysToClose = (int)Math.Abs(Math.Truncate(closeDT.Subtract(this.Audit.AUDIT_DT).TotalDays));
				this.DaysOpen = 0;
			}
			else
			{
				days = this.DaysOpen = (int)Math.Abs(Math.Truncate(DateTime.Now.Subtract(this.Audit.AUDIT_DT).TotalDays));
				this.DaysToClose = 0;
			}

			return days;
		}
	}

	public static class EHSAuditMgr
	{
		public static AUDIT SelectAuditById(PSsqmEntities entities, decimal auditId)
		{
			return (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
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

		//public static decimal SelectProblemCaseIdByAuditId(decimal auditId)
		//{
		//	decimal? problemCaseId;
		//	var entities = new PSsqmEntities();
		//	problemCaseId = (from po in entities.PROB_OCCUR where po.AUDIT_ID == auditId select po.PROBCASE_ID).FirstOrDefault();
		//	if (problemCaseId == null)
		//		problemCaseId = 0;
		//	return (decimal)problemCaseId;
		//}

		public static List<AUDIT_TYPE> SelectAuditTypeList(decimal companyId)
		{
			var auditTypeList = new List<AUDIT_TYPE>();

			try
			{
				var entities = new PSsqmEntities();

				//if (companyId > 0)
				//	auditTypeList = (from itc in entities.AUDIT_TYPE_COMPANY
				//						join it in entities.AUDIT_TYPE on itc.AUDIT_TYPE_ID equals it.AUDIT_TYPE_ID
				//						where itc.COMPANY_ID == companyId && itc.SORT_ORDER < 1000
				//						orderby it.TITLE
				//						select it).ToList();
				//else
				//{
					auditTypeList = (from itc in entities.AUDIT_TYPE
										orderby itc.TITLE
										select itc).ToList();
				//}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}

			return auditTypeList;
		}

		//public static List<AUDIT_TYPE> SelectPreventativeTypeList(decimal companyId)
		//{
		//	var preventativeTypeList = new List<AUDIT_TYPE>();

		//	try
		//	{
		//		var entities = new PSsqmEntities();

		//		preventativeTypeList = (from itc in entities.AUDIT_TYPE_COMPANY
		//								join it in entities.AUDIT_TYPE on itc.AUDIT_TYPE_ID equals it.AUDIT_TYPE_ID
		//								where itc.COMPANY_ID == companyId && itc.SORT_ORDER >= 1000
		//								orderby it.TITLE
		//								select it).ToList();
		//	}
		//	catch (Exception e)
		//	{
		//		//SQMLogger.LogException(e);
		//	}

		//	return preventativeTypeList;
		//}


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


		//public static List<AUDIT> SelectInjuryIllnessAudits(decimal plantId, DateTime date)
		//{
		//	var audits = new List<AUDIT>();

		//	try
		//	{
		//		var entities = new PSsqmEntities();
		//		audits = (from i in entities.AUDIT
		//					 where
		//						 i.AUDIT_TYPE.ToUpper() == "EHS" &&
		//						 i.ISSUE_TYPE_ID == (decimal)EHSAuditTypeId.InjuryIllness &&
		//						 i.DETECT_PLANT_ID == plantId &&
		//						 i.AUDIT_DT.Year == date.Year && i.AUDIT_DT.Month == date.Month
		//					 select i).ToList();

		//	}
		//	catch (Exception e)
		//	{
		//		//SQMLogger.LogException(e);
		//	}

		//	return audits;
		//}

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

		/// <summary>
		/// Returns boolean indicating whether 8D should be selected for audit type by default
		/// </summary>
		//public static bool IsTypeDefault8D(decimal selectedTypeId)
		//{
		//	var entities = new PSsqmEntities();
		//	return (from it in entities.AUDIT_TYPE where it.AUDIT_TYPE_ID == selectedTypeId select it.DEFAULT_8D).FirstOrDefault();
		//}


		/// <summary>
		/// Select a list of all audit questions by company and audit type
		/// </summary>
		//public static List<EHSAuditQuestion> SelectAuditQuestionList(decimal auditTypeId, decimal companyId, int step)
		//{
		//	var questionList = new List<EHSAuditQuestion>();

		//	try
		//	{
		//		var entities = new PSsqmEntities();
		//		var activeQuestionList = (from q in entities.AUDIT_TYPE_COMPANY_QUESTION
		//								  where q.AUDIT_TYPE_ID == auditTypeId && q.COMPANY_ID == companyId && q.STEP == step
		//								  orderby q.SORT_ORDER
		//								  select q
		//						).ToList();

		//		foreach (var aq in activeQuestionList)
		//		{
		//			var questionInfo = (from qi in entities.AUDIT_QUESTION
		//								where qi.AUDIT_QUESTION_ID == aq.AUDIT_QUESTION_ID
		//								select qi).FirstOrDefault();

		//			var typeInfo = (from ti in entities.AUDIT_QUESTION_TYPE
		//							where questionInfo.AUDIT_QUESTION_TYPE_ID == ti.AUDIT_QUESTION_TYPE_ID
		//							select ti).FirstOrDefault();

		//			var newQuestion = new EHSAuditQuestion()
		//			{
		//				QuestionId = questionInfo.AUDIT_QUESTION_ID,
		//				QuestionText = questionInfo.QUESTION_TEXT,
		//				QuestionType = (EHSAuditQuestionType)questionInfo.AUDIT_QUESTION_TYPE_ID,
		//				HasMultipleChoices = typeInfo.HAS_MULTIPLE_CHOICES,
		//				IsRequired = questionInfo.IS_REQUIRED,
		//				IsRequiredClose = questionInfo.IS_REQUIRED_CLOSE,
		//				HelpText = questionInfo.HELP_TEXT,
		//				StandardType = questionInfo.STANDARD_TYPE
		//			};

		//			if (newQuestion.HasMultipleChoices)
		//			{
		//				List<EHSAuditAnswerChoice> choices = (from qc in entities.AUDIT_QUESTION_CHOICE
		//														 where qc.AUDIT_QUESTION_ID == questionInfo.AUDIT_QUESTION_ID
		//														 orderby qc.SORT_ORDER
		//														 select new EHSAuditAnswerChoice
		//														 {
		//															 Value = qc.QUESTION_CHOICE_VALUE,
		//															 IsCategoryHeading = qc.IS_CATEGORY_HEADING
		//														 }).ToList();
		//				if (choices.Count > 0)
		//					newQuestion.AnswerChoices = choices;
		//			}

		//			// Question control logic
		//			newQuestion.QuestionControls = (from qc in entities.AUDIT_QUESTION_CONTROL
		//											where qc.AUDIT_TYPE_ID == auditTypeId &&
		//											qc.COMPANY_ID == companyId &&
		//											qc.AUDIT_QUESTION_ID == newQuestion.QuestionId
		//											orderby qc.PROCESS_ORDER
		//											select qc).ToList();

		//			questionList.Add(newQuestion);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		//SQMLogger.LogException(e);
		//	}

		//	return questionList;
		//}

		/// <summary>
		/// Select a list of all audit questions by topic 
		/// </summary>
		public static List<EHSAuditQuestion> SelectAuditQuestionList(decimal auditTypeId, decimal auditTopicId)
		{
			var questionList = new List<EHSAuditQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var activeQuestionList = new List<AUDIT_TYPE_TOPIC_QUESTION>();
				if (auditTopicId > 0)
					activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
										  where q.AUDIT_TYPE_ID == auditTypeId && q.AUDIT_TOPIC_ID == auditTopicId
										  orderby q.SORT_ORDER
										  select q
								).ToList();
				else
					activeQuestionList = (from q in entities.AUDIT_TYPE_TOPIC_QUESTION
										  where q.AUDIT_TYPE_ID == auditTypeId
										  orderby q.AUDIT_TOPIC_ID, q.SORT_ORDER
										  select q
										  ).ToList();


				foreach (var aq in activeQuestionList)
				{
					var questionInfo = (from qi in entities.AUDIT_QUESTION
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
						QuestionText = questionInfo.QUESTION_TEXT,
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
																  IsCategoryHeading = qc.IS_CATEGORY_HEADING
															  }).ToList();
						if (choices.Count > 0)
							newQuestion.AnswerChoices = choices;
					}

					// Question control logic
					newQuestion.QuestionControls = (from qc in entities.AUDIT_QUESTION_CONTROL
													where qc.AUDIT_TYPE_ID == auditTypeId &&
													qc.AUDIT_TOPIC_ID == auditTopicId &&
													qc.AUDIT_QUESTION_ID == newQuestion.QuestionId
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
		/// Select a list of all possible audit questions
		/// </summary>
		public static List<EHSAuditQuestion> SelectAuditQuestionList()
		{
			var questionList = new List<EHSAuditQuestion>();

			try
			{
				var entities = new PSsqmEntities();
				var allQuestions = (from q in entities.AUDIT_QUESTION select q).ToList();

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
				topicList = (from q in entities.AUDIT_QUESTION where questionIds.Contains(q.AUDIT_QUESTION_ID) select q).ToList();
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

		//public static void CreateOrUpdateTask(decimal auditId, decimal responsiblePersonId, int recordTypeId, DateTime dueDate)
		//{
		//	var entities = new PSsqmEntities();

		//	AUDIT audit = SelectAuditById(entities, auditId);

		//	var taskMgr = new TaskStatusMgr();
		//	taskMgr.Initialize(recordTypeId, auditId);
		//	taskMgr.LoadTaskList(recordTypeId, auditId);
		//	TASK_STATUS task = taskMgr.FindTask("0", "T", 0);

		//	if (task == null)
		//	{
		//		task = taskMgr.CreateTask("0", "T", 0, audit.ISSUE_TYPE, dueDate, responsiblePersonId);
		//		task.STATUS = ((int)TaskMgr.CalculateTaskStatus(task)).ToString();
		//	}
		//	else
		//	{
		//		task = taskMgr.UpdateTask(task, dueDate, responsiblePersonId, audit.ISSUE_TYPE);
		//	}

		//	taskMgr.UpdateTaskList(auditId);

		//}

		//public static void SetTaskComplete(decimal auditId, int recordTypeId)
		//{
		//	var taskMgr = new TaskStatusMgr();
		//	taskMgr.Initialize(recordTypeId, auditId);
		//	taskMgr.LoadTaskList(recordTypeId, auditId);
		//	TASK_STATUS task = taskMgr.FindTask("0", "T", 0);

		//	if (task != null)
		//	{
		//		taskMgr.UpdateTaskStatus(task, TaskMgr.CalculateTaskStatus(task));
		//		taskMgr.SetTaskComplete("0", "T", 0, true);
		//		taskMgr.UpdateTaskList(auditId);
		//	}
		//}


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



		public static void TryCloseAudit(decimal auditId)
		{
			var entities = new PSsqmEntities();

			AUDIT audit = SelectAuditById(entities, auditId);

			if (ShouldAuditReportClose(audit))
			{
				audit.CLOSE_DATE = DateTime.Now;
				//SetTaskComplete(auditId, 40);
			}
			else
			{
				audit.CLOSE_DATE = null;
			}

			if (ShouldAuditCloseDataComplete(audit))
				audit.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
			else
				audit.CLOSE_DATE_DATA_COMPLETE = null;

			entities.SaveChanges();
		}


		public static bool ShouldAuditReportClose(AUDIT audit)
		{
			var entities = new PSsqmEntities();
			int auditClosedScore = 0;

			var questionList = SelectAuditQuestionList((decimal)audit.AUDIT_TYPE_ID, 0);
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


		public static bool ShouldAuditCloseDataComplete(AUDIT audit)
		{
			//var entities = new PSsqmEntities();

			//var questionList = SelectAuditQuestionList((decimal)audit.ISSUE_TYPE_ID, audit.DETECT_COMPANY_ID, 0);
			//var requiredQuestionIds = (from q in questionList where q.IsRequiredClose == true select q.QuestionId).ToList();

			//// Remove lost time date questions from required questions if not a lost time case
			//if (audit.ISSUE_TYPE_ID == (decimal)EHSAuditTypeId.InjuryIllness)
			//{
			//	var lostTimeQuestion = (from q in questionList where q.QuestionId == (decimal)EHSQuestionId.LostTimeCase select q).FirstOrDefault();
			//	if (lostTimeQuestion != null)
			//	{
			//		var answerText = SelectAuditAnswer(audit, (decimal)EHSQuestionId.LostTimeCase);

			//		if (answerText != "Yes")
			//		{
			//			requiredQuestionIds.Remove((decimal)EHSQuestionId.ExpectedReturnDate);
			//			requiredQuestionIds.Remove((decimal)EHSQuestionId.ActualReturnDate);
			//		}
			//	}
			//}

			//var answers = (from a in entities.AUDIT_ANSWER
			//			   where a.AUDIT_ID == audit.AUDIT_ID && requiredQuestionIds.Contains(a.AUDIT_QUESTION_ID)
			//			   select a.ANSWER_VALUE).ToList();

			//bool shouldClose = true;
			//foreach (var a in answers)
			//{
			//	if (string.IsNullOrEmpty(a))
			//	{
			//		shouldClose = false;
			//		break;
			//	}
			//}
			//return shouldClose;
			return true; // until we determine if we need this
		}

		public static void TryClosePrevention(decimal auditId, decimal personId)
		{
			var entities = new PSsqmEntities();

			AUDIT audit = SelectAuditById(entities, auditId);
			bool shouldUpdateAuditPerson = true;

			if (ShouldPreventionClose(audit))
			{
				audit.CLOSE_DATE = DateTime.Now;
				audit.CLOSE_PERSON = personId;
				//SetTaskComplete(auditId, 45);
				shouldUpdateAuditPerson = false;
			}
			else
			{
				audit.CLOSE_DATE = null;
			}

			if (ShouldPreventionCloseAudited(audit))
			{
				audit.CLOSE_DATE_DATA_COMPLETE = DateTime.Now;
				if (shouldUpdateAuditPerson == true)
					audit.AUDIT_PERSON = personId;
			}
			else
			{
				audit.CLOSE_DATE_DATA_COMPLETE = null;
			}

			entities.SaveChanges();
		}

		public static bool ShouldPreventionClose(AUDIT audit)
		{
			//var entities = new PSsqmEntities();
			//bool shouldClose = false;

			//var questionList = SelectAuditQuestionList((decimal)audit.AUDIT_TYPE_ID, 0);
			//foreach (var q in questionList)
			//{
			//	string answer = (from a in entities.AUDIT_ANSWER
			//					 where a.AUDIT_ID == audit.AUDIT_ID && a.AUDIT_QUESTION_ID == q.QuestionId
			//					 select a.ANSWER_VALUE).FirstOrDefault();

			//	if (q.QuestionId == (decimal)EHSQuestionId.CorrectiveActionsStatus && !string.IsNullOrEmpty(answer))
			//		if (answer.ToLower() == "closed")
			//			shouldClose = true;
			//}

			//return shouldClose;
			return true;
		}


		public static bool ShouldPreventionCloseAudited(AUDIT audit)
		{
			//bool shouldClose = false;

			//var entities = new PSsqmEntities();

			//var questionList = SelectAuditQuestionList((decimal)audit.ISSUE_TYPE_ID, audit.DETECT_COMPANY_ID, 1);
			//foreach (var q in questionList)
			//{
			//	string answer = (from a in entities.AUDIT_ANSWER
			//					 where a.AUDIT_ID == audit.AUDIT_ID && a.AUDIT_QUESTION_ID == q.QuestionId
			//					 select a.ANSWER_VALUE).FirstOrDefault();

			//	if (q.QuestionId == (decimal)EHSQuestionId.FinalAuditStepResolved && !string.IsNullOrEmpty(answer))
			//		if (answer.ToLower() == "yes" || answer.ToLower().Contains("funding"))
			//			shouldClose = true;
			//}

			//return shouldClose;
			return true;
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
	}
}