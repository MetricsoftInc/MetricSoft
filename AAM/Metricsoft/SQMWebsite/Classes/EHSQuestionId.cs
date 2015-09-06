using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public enum EHSQuestionId
	{
		IncidentDate = 1,
		ReportDate = 2,
		ExpectedReturnDate = 3,
		TimeOfDay = 5,
		Department = 7,
		InjuryType = 12,
		BodyPart = 13,
		FirstAid = 16,
		Description = 17,
		RootCause = 24,
		CorrectiveActions = 27,
		Create8D = 54, 
		ActualReturnDate = 55,
		Location = 56,
		Recordable = 62,
		LostTimeCase = 63,
		ResponsiblePerson = 64,
		DateDue = 65,
		CompletionDate = 66,
		CompletedBy = 67,
		Containment = 69,
		Verification = 70,
		CloseIncident = 72,
		RootCauseOperationalControl = 78,
		ResponsiblePersonDropdown = 79,
		InspectionDate = 80,
		InspectionCategory = 81,
		RecommendationType = 83,
		AssignToPerson = 84,
		PreventionAppliedDate = 85,
		PreventionActionsSummary = 86,
		FinalAuditStepResolved = 88,
		ProjectedCompletionDate = 92,
		CorrectiveActionsStatus = 93,
		CostToImplement = 94,
		RecommendationSummary = 95,
		ReportedBy	= 96,
		NativeLangComment = 107,
		Operation = 109
	}
}