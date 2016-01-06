using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace SQM.Website
{
	public static class EHSAccountingMgr
	{

		public static void RollupPlantAccounting(decimal initialPlantId, decimal finalPlantId)
		{
			var entities = new PSsqmEntities();

			var plantIds = new HashSet<decimal> {initialPlantId, finalPlantId};
			plantIds.Remove(0);

			// Default to January 1 of current year if web.config value not found
			DateTime startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);

			string plantAccountingCalcStartDate = System.Configuration.ConfigurationManager.AppSettings["PlantAccountingCalcStartDate"];
			if (!string.IsNullOrEmpty(plantAccountingCalcStartDate))
			{
				DateTime result;
				if (DateTime.TryParse(plantAccountingCalcStartDate, out result))
					startDate = result;
			}

			
			// Step 1 - zero out incident plant accounting values or create new records

			foreach (decimal pid in plantIds)
			{
				DateTime incDate = startDate;

				while (incDate < DateTime.UtcNow ||
					(incDate.Month == DateTime.UtcNow.Month && incDate.Year == DateTime.UtcNow.Year))
				{

					var pa = EHSModel.LookupPlantAccounting(entities, pid, incDate.Year, incDate.Month, true);
					pa.RECORDED_CASES = 0;
					pa.TIME_LOST_CASES = 0;
					pa.TIME_LOST = 0;
					EHSModel.UpdatePlantAccounting(entities, pa);

					incDate = incDate.AddMonths(1);
				}
			}


			// Step 2 - update records incident by incident

			foreach (decimal pid in plantIds)
			{
				DateTime incDate = startDate;

				while (incDate < DateTime.UtcNow ||
					(incDate.Month == DateTime.UtcNow.Month && incDate.Year == DateTime.UtcNow.Year))
				{
					var pa = EHSModel.LookupPlantAccounting(entities, pid, incDate.Year, incDate.Month, true);

					var incidentList = EHSIncidentMgr.SelectInjuryIllnessIncidents(pid, incDate);  // this might be wrong ??
					foreach (INCIDENT incident in incidentList)
					{
						string recordableAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.Recordable);
						if (!string.IsNullOrEmpty(recordableAnswerValue) && recordableAnswerValue == "Yes")
							pa.RECORDED_CASES++;

						string ltcAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.LostTimeCase);
						if (!string.IsNullOrEmpty(ltcAnswerValue) && ltcAnswerValue == "Yes")
							pa.TIME_LOST_CASES++;
						
						string someReturnDate = ""; // expected or actual return date

						string erdAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.ExpectedReturnDate);
						if (!string.IsNullOrEmpty(erdAnswerValue))
							someReturnDate = erdAnswerValue;

						string ardAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, (decimal)EHSQuestionId.ActualReturnDate);
						if (!string.IsNullOrEmpty(ardAnswerValue))
							someReturnDate = ardAnswerValue;

						if (!string.IsNullOrEmpty(someReturnDate))
							UpdateIncidentLostTimeDays(incident, someReturnDate);
						
					}

					EHSModel.UpdatePlantAccounting(entities, pa);
					incDate = incDate.AddMonths(1);
				}
			}
		}

		private static void UpdateIncidentLostTimeDays(INCIDENT incident, string someReturnDate)
		{
			var entities = new PSsqmEntities();

			DateTime startDate = incident.INCIDENT_DT.AddDays(1);

			// Default to current date if no actual return date set
			DateTime endDate;

			try
			{
				// Try to parse return date (expected or actual)
				endDate = DateTime.Parse(someReturnDate, CultureInfo.GetCultureInfo("en-US"));
			}
			catch
			{
				return;
			}

			if (startDate >= endDate)
				return;

			DateTime incDate = startDate;
			decimal plantId = (decimal)incident.DETECT_PLANT_ID;

			while (incDate < endDate ||
					(incDate.Month == endDate.Month && incDate.Year == endDate.Year))
			{
				int lostTimeThisMonth = 0;
				int daysInThisMonth = DateTime.DaysInMonth(incDate.Year, incDate.Month);
				DateTime monthStart = new DateTime(incDate.Year, incDate.Month, 1, 0, 0, 0); // first second of the month
				DateTime monthEnd = monthStart.AddDays(daysInThisMonth).Subtract(new TimeSpan(0, 0, 1)); // last second of the month

				if (startDate < monthStart && endDate > monthEnd)
				{
					// Incident starts before and ends after this month
					lostTimeThisMonth = daysInThisMonth;
				}
				else if (startDate >= monthStart && endDate > monthEnd)
				{
					// Incident starts but doesn't end this month
					lostTimeThisMonth = (int)Math.Ceiling((monthEnd - startDate).TotalDays);
				}
				else if (startDate < monthStart && endDate <= monthEnd)
				{
					// Incident ends but didn't start this month
					lostTimeThisMonth = (int)Math.Ceiling((endDate - monthStart).TotalDays);
				}
				else if (endDate > startDate)
				{
					// Incident both starts and ends this month
					lostTimeThisMonth = (int)Math.Ceiling((endDate - startDate).TotalDays);
				}
					
				var pa = EHSModel.LookupPlantAccounting(entities, plantId, incDate.Year, incDate.Month, true);

				pa.TIME_LOST += lostTimeThisMonth;

				EHSModel.UpdatePlantAccounting(entities, pa);
				incDate = incDate.AddMonths(1);
			}
		}

	}

}