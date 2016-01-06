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

	public enum IncidentStepStatus { unknown=0, defined=100, workstatus=105, containment=110, containmentComplete=115,  rootcause=120, rootcauseComplete=125, correctiveaction=130, correctiveactionComplete=135, signoff1=151, signoff2=152, signoffComplete=155}

	public class EHSIncidentQuestion
	{
		public decimal QuestionId { get; set; }
		public string QuestionText { get; set; }
		public EHSIncidentQuestionType QuestionType { get; set; }
		public bool HasMultipleChoices { get; set; }
		public List<EHSMetaData> AnswerChoices { get; set; }
		public bool IsRequired { get; set; }
		public bool IsRequiredClose { get; set; }
		public string HelpText { get; set; }
		public string AnswerText { get; set; }
		public string StandardType { get; set; }
		public List<INCIDENT_QUESTION_CONTROL> QuestionControls { get; set; }
	}

	public class EHSIncidentAnswerChoice
	{
		public string Text { get; set; }
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
		public decimal PlantID { get; set; }
		public decimal IncidentType { get; set; }
		public int NearMiss { get; set; }
		public int FirstAidCase { get; set; }
		public int RecordableCase { get; set; }
		public int LostTimeCase { get; set; }
		public int FatalityCase { get; set; }
		public int OtherCase { get; set; }
		public decimal LostTime { get; set; }
		public decimal RestrictedTime { get; set; }
		public decimal WorkTime { get; set; }

		public EHSIncidentTimeAccounting CreateNew(int periodYear, int periodMonth, decimal incidentType, decimal plantID)
		{
			this.PeriodYear = periodYear;
			this.PeriodMonth = periodMonth;
			this.IncidentType = incidentType;
			this.PlantID = plantID;
			this.LostTime = this.RestrictedTime = this.WorkTime = 0;
			this.NearMiss = this.FirstAidCase = this.RecordableCase = LostTimeCase = FatalityCase = OtherCase = 0;
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
				else
					this.Status = "A";  // incident open
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
					days = this.DaysOpen = (int)Math.Abs(Math.Truncate(WebSiteCommon.LocalTime(DateTime.UtcNow, this.Plant.LOCAL_TIMEZONE).Subtract((DateTime)this.Incident.CREATE_DT).TotalDays));
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
					days = this.DaysOpen = (int)Math.Abs(Math.Truncate(WebSiteCommon.LocalTime(DateTime.UtcNow, this.Plant.LOCAL_TIMEZONE).Subtract(this.Incident.INCIDENT_DT).TotalDays));
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
			return (SelectIncidentById(entities, incidentId, false));
		}

		public static INCIDENT SelectIncidentById(PSsqmEntities entities, decimal incidentId, bool loadChildren)
		{
			INCIDENT incident = null;

			try
			{
				incident = (from i in entities.INCIDENT.Include("INCFORM_INJURYILLNESS") where i.INCIDENT_ID == incidentId select i).FirstOrDefault();

				if (loadChildren)
				{
					incident.INCFORM_CONTAIN.Load();
					incident.INCFORM_ROOT5Y.Load();
					incident.INCFORM_CAUSATION.Load();
					incident.INCFORM_APPROVAL.Load();
				}
			}
			catch
			{
			}

			return incident;
		}

		public static INCIDENT_TYPE SelectIncidentType(decimal incidentTypeID)
		{
			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				return (from t in ctx.INCIDENT_TYPE where t.INCIDENT_TYPE_ID == incidentTypeID select t).SingleOrDefault();
			}
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

		public static IncidentStepStatus UpdateIncidentStatus(decimal incidentID, IncidentStepStatus currentStepStatus, DateTime ? defaultDate)
		{
			return UpdateIncidentStatus(incidentID,  currentStepStatus, false, defaultDate);
		}


		public static IncidentStepStatus UpdateIncidentStatus(decimal incidentID, IncidentStepStatus currentStepStatus, bool closeIncident, DateTime ? defaultDate)
		{
			INCIDENT incident = null;
			bool isUpdated = false;
			IncidentStepStatus calcStatus = IncidentStepStatus.unknown;
			string localTZ = "";

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				incident = (from i in ctx.INCIDENT where i.INCIDENT_ID == incidentID select i).SingleOrDefault();
				if (incident != null)
				{
					calcStatus = IncidentStepStatus.defined;

					if ((from c in ctx.INCFORM_CONTAIN where c.INCIDENT_ID == incidentID select c).Count() > 0)
					{
						calcStatus = IncidentStepStatus.containment;
					}
					if ((from c in ctx.INCFORM_ROOT5Y where c.INCIDENT_ID == incidentID select c).Count() > 0)
					{
						calcStatus = IncidentStepStatus.rootcause;
					}
					if ((from c in ctx.INCFORM_CAUSATION where c.INCIDENT_ID == incidentID select c).Count() > 0)
					{
						calcStatus = IncidentStepStatus.rootcauseComplete;
					}
					List<TASK_STATUS> actionList = (from c in ctx.TASK_STATUS where c.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident && c.RECORD_ID == incidentID select c).ToList();
					if (actionList != null  &&  actionList.Count() > 0)
					{
						calcStatus = IncidentStepStatus.correctiveaction;
						if (actionList.Where(l => l.COMPLETE_DT != null).ToList().Count > 0)
						{
							calcStatus = IncidentStepStatus.correctiveactionComplete;
						}
					}
					List<INCFORM_APPROVAL> approvalList = (from c in ctx.INCFORM_APPROVAL where c.INCIDENT_ID == incidentID select c).ToList();
					{
						if (approvalList != null && approvalList.Where(l => l.ITEM_SEQ == (int)SysPriv.approve1).ToList().Count > 0)
						{
							calcStatus = IncidentStepStatus.signoff1;
						}
						if (approvalList != null && approvalList.Where(l => l.ITEM_SEQ == (int)SysPriv.approve2).ToList().Count > 0)
						{
							calcStatus = IncidentStepStatus.signoff2;
							if (closeIncident && !incident.CLOSE_DATE.HasValue)
							{
								PLANT plant = SQMModelMgr.LookupPlant(ctx, (decimal)incident.DETECT_PLANT_ID, "");
								incident.CLOSE_DATE = incident.CLOSE_DATE_DATA_COMPLETE = defaultDate != null ? defaultDate : DateTime.UtcNow;
								isUpdated = true;
							}
						}
					}

					if (calcStatus != IncidentStepStatus.unknown)
					{
						incident.INCFORM_LAST_STEP_COMPLETED = (int)calcStatus;
						int status = ctx.SaveChanges();
					}
				}
			}

			return calcStatus;
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

		public static bool CanUpdateIncident(INCIDENT incident, bool IsEditContext, SysPriv privNeeded, int stepCompleted)
		{
			bool canUpdate = false;

			if (IsEditContext)
			{
				if ((incident != null  && incident.CREATE_PERSON == SessionManager.UserContext.Person.PERSON_ID) || SessionManager.CheckUserPrivilege(privNeeded, SysScope.incident))
				{
					canUpdate = true;
				}

				if (stepCompleted == (int)IncidentStepStatus.signoff2)  // incident is closed
				{
					canUpdate = false;
				}
			}
			else  // assume edit context will be false for new incidents an any user can create/save a new incident
			{
				canUpdate = true;
			}

			return canUpdate;
		}


		public static bool CanDeleteIncident(decimal createPersonID, int stepCompleted)
		{
			bool canDelete = false;

			if (UserContext.CheckUserPrivilege(SysPriv.approve1, SysScope.incident) ||
				UserContext.CheckUserPrivilege(SysPriv.approve2, SysScope.incident) ||
				UserContext.CheckUserPrivilege(SysPriv.admin, SysScope.incident) ||
				SessionManager.UserContext.Person.PERSON_ID == createPersonID)
			{
				canDelete = true;
			}

			return canDelete;
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

		public static void CloseIncident(decimal incidentId, DateTime ? defaultDate)
		{
			var entities = new PSsqmEntities();

			var incident = (from i in entities.INCIDENT where i.INCIDENT_ID == incidentId select i).FirstOrDefault();
			if (incident != null)
			{
				incident.CLOSE_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
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
						List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + questionInfo.INCIDENT_QUESTION_ID.ToString()).OrderBy(l => l.SortOrder).ToList();
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
						List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + q.INCIDENT_QUESTION_ID.ToString()).OrderBy(l => l.SortOrder).ToList();
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

		public static List<EHSMetaData> SelectIncidentQuestionChoices(decimal questionID)
		{
			List<EHSMetaData> choices = EHSMetaDataMgr.SelectMetaDataList("IQ_" + questionID.ToString()).OrderBy(l => l.SortOrder).ToList();
			return choices;
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


		public static List<INCIDENT_ANSWER> GetIncidentAnswerList(decimal incidentId)
		{
			PSsqmEntities entities = new PSsqmEntities();
			List<INCIDENT_ANSWER> answerList = new List<INCIDENT_ANSWER>();

			answerList = (from c in entities.INCIDENT_ANSWER 
						  where c.INCIDENT_ID == incidentId
						  select c).ToList();

			return answerList;
		}

		public static List<INCFORM_ROOT5Y> GetRootCauseList(decimal incidentId)
		{
			return GetRootCauseList(incidentId, false);
		}

		public static List<INCFORM_ROOT5Y> GetRootCauseList(decimal incidentId, bool getMinRows)
		{

			PSsqmEntities entities = new PSsqmEntities();
			var rootcauses = new List<INCFORM_ROOT5Y>();

			int minRowsThisForm = 5;

			rootcauses = (from c in entities.INCFORM_ROOT5Y
						where c.INCIDENT_ID == incidentId
						  select c).ToList();

			if (getMinRows)
			{
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
			}
			
			return rootcauses;
		}



		public static List<INCFORM_CONTAIN> GetContainmentList(decimal incidentId, DateTime ? defaultDate)
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
					contain.START_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
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


		public static List<TASK_STATUS> GetCorrectiveActionList(decimal incidentId, DateTime ? defaultDate)
		{
			PSsqmEntities entities = new PSsqmEntities();

			List<TASK_STATUS> actionList = (from t in entities.TASK_STATUS
											where t.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident && t.RECORD_ID == incidentId
											select t).ToList();
			if (actionList.Count == 0)
			{
				actionList.Add(CreateEmptyTask(incidentId, ((int)SysPriv.action).ToString(), 1, defaultDate));
			}

			return actionList;
		}

		public static TASK_STATUS CreateEmptyTask(decimal incidentId, string taskStep, int taskSeq, DateTime ? defaultDate)
		{
			TASK_STATUS task = new TASK_STATUS();
			task.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
			task.RECORD_ID = incidentId;
			task.TASK_STEP = taskStep;
			task.TASK_TYPE = "T";
			task.CREATE_DT = defaultDate != null ? defaultDate : DateTime.UtcNow;
			task.STATUS = ((int)TaskStatus.New).ToString();
			task.TASK_SEQ = taskSeq;

			return task;
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

		public static List<EHSIncidentTimeAccounting> CalculateIncidentAccounting(PSsqmEntities ctx, INCIDENT incident, string localeTimezone, int workdays)
		{
			List<EHSIncidentTimeAccounting> periodList = new List<EHSIncidentTimeAccounting>();

			// basic incident info gets accrued in the month the incident occurred
			EHSIncidentTimeAccounting period1 = new EHSIncidentTimeAccounting().CreateNew(Convert.ToDateTime(incident.INCIDENT_DT).Year, Convert.ToDateTime(incident.INCIDENT_DT).Month, (decimal)incident.ISSUE_TYPE_ID, (decimal)incident.DETECT_PLANT_ID);
			periodList.Add(period1);

			period1.NearMiss = incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.NearMiss ? 1 : 0;

			if (incident.INCFORM_INJURYILLNESS == null)
			{
				return periodList;
			}

			period1.RecordableCase = incident.INCFORM_INJURYILLNESS.RECORDABLE ? 1 : 0;
			period1.FirstAidCase = incident.INCFORM_INJURYILLNESS.FIRST_AID ? 1 : 0;
			period1.LostTimeCase = incident.INCFORM_INJURYILLNESS.LOST_TIME ? 1 : 0;
			period1.FatalityCase = incident.INCFORM_INJURYILLNESS.FATALITY.HasValue  &&  (bool)incident.INCFORM_INJURYILLNESS.FATALITY ? 1 : 0;
			if (period1.RecordableCase + period1.FirstAidCase == 0)
				period1.OtherCase = 1;

			if (incident.INCFORM_LOSTTIME_HIST == null || incident.INCFORM_LOSTTIME_HIST.Count == 0)
			{
				return periodList;
			}

			// determine incident time span
			// assume incident is still open and extend timespan to NOW if last status was not return to work
			List<INCFORM_LOSTTIME_HIST> histList = incident.INCFORM_LOSTTIME_HIST.OrderBy(h => h.BEGIN_DT).ToList();

			INCFORM_LOSTTIME_HIST hist = histList.First();
			string workStatus = hist.WORK_STATUS;

			DateTime startDate = WebSiteCommon.LocalTime((DateTime)hist.BEGIN_DT, localeTimezone).Date;
			DateTime endDate = WebSiteCommon.LocalTime((DateTime)histList.Last().BEGIN_DT, localeTimezone).Date;
			if (histList.Last().WORK_STATUS != "02")  // if last record is not a return to work, assume last work status is still in effect
			{
				endDate = WebSiteCommon.LocalTime(DateTime.UtcNow.AddDays(-1), localeTimezone);
			}

			int numDays = Convert.ToInt32((endDate - startDate).TotalDays);		// get total # days of the incident timespan
			DateTime effDate;
			bool countDay = true;
			EHSIncidentTimeAccounting period;
			for (int n = 0; n <= numDays; n++)
			{
				effDate = startDate.AddDays(n);

				// accrue or add accounting periods as needed per the incident timespan
				if ((period = periodList.Where(p => p.PeriodYear == effDate.Year && p.PeriodMonth == effDate.Month).FirstOrDefault()) == null)
				{
					periodList.Add((period = new EHSIncidentTimeAccounting().CreateNew(effDate.Year, effDate.Month, (decimal)incident.ISSUE_TYPE_ID, (decimal)incident.DETECT_PLANT_ID)));
				}
				// check if new work status occurred on this date
				if ((hist = histList.Where(l => l.BEGIN_DT == effDate).FirstOrDefault()) != null)
				{
					workStatus = hist.WORK_STATUS;
				}

				countDay = true;
				switch (effDate.DayOfWeek)
				{
					case DayOfWeek.Sunday:	// count if 7 day work week
						if (workdays < 7)
							countDay = false;
						break;
					case DayOfWeek.Saturday:   // count if 6 day work week
						if (workdays < 6)
							countDay = false;
						break;
					default:
						break;
				}

				if (countDay)
				{
					switch (workStatus)
					{
						case "01":
							++period.RestrictedTime;
							break;
						case "03":
							++period.LostTime;
							break;
						default:
							++period.WorkTime;
							break;
					}
				}
			}

			return periodList;
		}

		public static List<EHSIncidentTimeAccounting> SummarizeIncidentAccounting(List<EHSIncidentTimeAccounting> summaryList, List<EHSIncidentTimeAccounting> periodList)
		{
			EHSIncidentTimeAccounting period = null;

			foreach (EHSIncidentTimeAccounting pa in periodList)
			{
				if ((period = summaryList.Where(p => p.PeriodYear == pa.PeriodYear && p.PeriodMonth == pa.PeriodMonth  &&  p.PlantID == pa.PlantID).FirstOrDefault()) == null)
				{
					summaryList.Add((period = new EHSIncidentTimeAccounting().CreateNew(pa.PeriodYear, pa.PeriodMonth, 0, pa.PlantID)));
				}
				period.NearMiss += pa.NearMiss;
				period.FatalityCase += pa.FatalityCase;
				period.FirstAidCase += pa.FirstAidCase;
				period.LostTime += pa.LostTime;
				period.LostTimeCase += pa.LostTimeCase;
				period.RecordableCase += pa.RecordableCase;
				period.OtherCase += pa.OtherCase;
				period.RestrictedTime += pa.RestrictedTime;
				period.WorkTime += pa.WorkTime;
			}

			return summaryList;
		}

		public static List<EHSIncidentTimeAccounting> SelectTimePeriodSpan(List<EHSIncidentTimeAccounting> periodList, DateTime selectFromDate, DateTime selectToDate)
		{
			if (selectFromDate > DateTime.MinValue)  // return specific time slice, if specified
			{
				periodList = periodList.Where(l => new DateTime(l.PeriodYear, l.PeriodMonth, 28) >= new DateTime(selectFromDate.Year, selectFromDate.Month, 1) && new DateTime(l.PeriodYear, l.PeriodMonth, 1) <= new DateTime(selectToDate.Year, selectToDate.Month, 28)).ToList();
			}

			return periodList;
		}


		public static List<INCFORM_APPROVAL> GetApprovalList(decimal incidentId, DateTime ? defaultDate)
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
					approval.APPROVAL_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
					approval.IsAccepted = false;
					approvals.Add(approval);
				}
			}

			return approvals;
		}


		public static void CreateOrUpdateTask(INCIDENT incident, TASK_STATUS task, DateTime ? defaultDate)
		{
			TaskStatusMgr taskMgr = new TaskStatusMgr();
			taskMgr.Initialize(task.RECORD_TYPE, task.RECORD_ID);
			TASK_STATUS theTask = taskMgr.SelectTask(task.TASK_ID);
			if (theTask == null)
			{
				task.CREATE_DT = defaultDate != null ? defaultDate : DateTime.UtcNow;
				task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;
				task.DETAIL = incident.DESCRIPTION;
				taskMgr.CreateTask(task);
				EHSNotificationMgr.NotifyIncidentTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
			}
			else
			{
				task.DETAIL = incident.DESCRIPTION;
				theTask = (TASK_STATUS)SQMModelMgr.CopyObjectValues(theTask, task, false);
				taskMgr.CreateTask(theTask);
			}

			taskMgr.UpdateTaskList(task.RECORD_ID);
		}

		public static void CreateOrUpdateTask(decimal incidentId, string taskStep, int taskSeq, decimal responsiblePersonId, int recordTypeId, DateTime dueDate, string taskDescription, string detail)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);

			var taskMgr = new TaskStatusMgr();
			taskMgr.Initialize(recordTypeId, incidentId);
			taskMgr.LoadTaskList(recordTypeId, incidentId);
			TASK_STATUS task = taskMgr.FindTask(((int)SysPriv.action).ToString(), "T", taskSeq, responsiblePersonId);

			if (task == null)
			{
				task = taskMgr.CreateTask(((int)SysPriv.action).ToString(), "T", taskSeq, !string.IsNullOrEmpty(taskDescription) ? taskDescription : incident.ISSUE_TYPE, dueDate, responsiblePersonId);
				task.DETAIL = detail;
				task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;
				EHSNotificationMgr.NotifyIncidentTaskAssigment(incident, task, ((int)SysPriv.action).ToString());
			}
			else
			{
				task = taskMgr.UpdateTask(task, dueDate, responsiblePersonId, taskDescription);
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
					List<decimal> attachmentIds = (from a in ctx.ATTACHMENT
												   where a.RECORD_TYPE == 40 && a.RECORD_ID == incidentId
												   select a.ATTACHMENT_ID).ToList();

					if (attachmentIds != null && attachmentIds.Count > 0)
					{
						status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT_FILE WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
						status = ctx.ExecuteStoreCommand("DELETE FROM ATTACHMENT WHERE ATTACHMENT_ID IN (" + String.Join(",", attachmentIds) + ")");
					}
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CONTAIN WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ACTION WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID" + delCmd);
					status = ctx.ExecuteStoreCommand("DELETE FROM TASK_STATUS WHERE RECORD_TYPE = 40 AND RECORD_ID" + delCmd);
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


		public static void TryCloseIncident(decimal incidentId, DateTime ? defaultDate)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);

			if (ShouldIncidentReportClose(incident))
			{
				incident.CLOSE_DATE = defaultDate != null ? defaultDate :  DateTime.UtcNow;
				SetTaskComplete(incidentId, 40);
			}
			else
			{
				incident.CLOSE_DATE = null;
			}

			if (ShouldIncidentCloseDataComplete(incident))
				incident.CLOSE_DATE_DATA_COMPLETE = defaultDate != null ? defaultDate : DateTime.UtcNow;
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

		public static void TryClosePrevention(decimal incidentId, decimal personId, DateTime defaultDate)
		{
			var entities = new PSsqmEntities();

			INCIDENT incident = SelectIncidentById(entities, incidentId);

			bool shouldUpdateAuditPerson = true;

			if (ShouldPreventionClose(incident))
			{
				incident.CLOSE_DATE = defaultDate != null ? defaultDate : DateTime.UtcNow;
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
				incident.CLOSE_DATE_DATA_COMPLETE = defaultDate != null ? defaultDate : DateTime.UtcNow;
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
		public int ? SortOrder { get; set; }
		public bool IsHeading { get; set; }

	}

	public static class EHSMetaDataMgr
	{

		public static List<EHSMetaData> SelectMetaDataList(string metaDataType)
		{
			var entities = new PSsqmEntities();
			var metaList = new List<EHSMetaData>();

			string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
			string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";

			if (language == "en")
			{
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
								Status = x.STATUS,
								SortOrder = x.SORT_ORDER,
								IsHeading = (bool)x.IS_HEADING
							}).ToList();
			}
			else
			{
				var tempList = (from x in entities.XLAT
							where (x.XLAT_LANGUAGE == language  ||  x.XLAT_LANGUAGE == "en") && x.XLAT_GROUP == metaDataType && x.STATUS == "A"
							orderby x.XLAT_CODE
							select new EHSMetaData()
							{
								Language = x.XLAT_LANGUAGE,
								MetaDataType = x.XLAT_GROUP,
								Text = x.DESCRIPTION_SHORT,
								TextLong = x.DESCRIPTION,
								Value = x.XLAT_CODE,
								Status = x.STATUS,
								SortOrder = x.SORT_ORDER,
								IsHeading = (bool)x.IS_HEADING
							}).ToList();

				EHSMetaData XLATlang = null;
				foreach (EHSMetaData xlat in tempList.Where(x => x.Language == "en").ToList())
				{
					XLATlang = tempList.Where(l => l.MetaDataType == xlat.MetaDataType && l.Value == xlat.Value && l.Language == language).FirstOrDefault();
					if (XLATlang != null)
						metaList.Add(XLATlang);
					else
					{
						XLATlang = new EHSMetaData();
						XLATlang = (EHSMetaData)SQMModelMgr.CopyObjectValues(XLATlang, xlat, false);
						XLATlang.Language = language;  // substitute english xlat if localized version does not exist
						metaList.Add(xlat);
					}
				}
			}

			return metaList;
		}
	
	}
}