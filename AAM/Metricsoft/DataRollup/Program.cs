using SQM.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Objects;
using System.Web;
using System.IO;

namespace DataRollup
{
    class Program
    {
		static StringBuilder output;

        static void Main(string[] args)
        {
			output = new StringBuilder();
			string nextStep = "";
			WriteLine("Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			//string strValidIP = sets.Find(x => x.SETTING_CD == "ValidIP").VALUE.ToString();

			// arguments:
			// no arguments supplied == run all rollup processes
			// env == run environmental rollup only
			// inc  == run incidents rollup only  (will also chain the ehsdata rollup if the ROLLUP_NEXTPAGE setting = "EHSDataRollUp"
			// ehsdata == run ehsData rollup only

			if (args.Length == 0  ||  args.Contains("env"))
			{
				ProcessEnvironmental();
			}
			
			if (args.Length == 0  ||  args.Contains("inc"))
			{
				nextStep = ProcessIncidents();
				if (nextStep == "EHSDataRollUp")
				{
					ProcessEHSData();
				}
			}

			if (args.Length > 0  &&  args.Contains("ehsdata"))
			{
				ProcessEHSData();
			}

			WriteLine("");
			WriteLine("Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			WriteLogFile();
        }


		#region incident
		static string ProcessIncidents()
		{
			PSsqmEntities entities = new PSsqmEntities();
			SETTINGS setting = null;
			int workdays = 7;
			int rollupMonthsAhead = 0;
			string nextStep = "";
			DateTime fromDate = DateTime.UtcNow.AddMonths(-12);    // set the incident 'select from' date.  TODO: get this from SETTINGS table
			DateTime rollupToDate = DateTime.UtcNow;

			WriteLine("INCIDENT Rollup Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

			try
			{

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_WORKDAYS").FirstOrDefault();
				if (setting != null && !string.IsNullOrEmpty(setting.VALUE))
				{
					if (!int.TryParse(setting.VALUE, out workdays))
						workdays = 7;
				}

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_NEXTPAGE").FirstOrDefault();
				if (setting != null && !string.IsNullOrEmpty(setting.VALUE) && setting.VALUE.Length > 1)
				{
					nextStep = setting.VALUE;
				}

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_MONTHS_AHEAD").FirstOrDefault();
				if (setting != null && !string.IsNullOrEmpty(setting.VALUE))
				{
					int.TryParse(setting.VALUE, out rollupMonthsAhead);
					rollupToDate = rollupToDate.AddMonths(rollupMonthsAhead);
				}

				// fetch all incidents occurring after the minimum reporting date
				List<INCIDENT> incidentList = (from i in entities.INCIDENT.Include("INCFORM_INJURYILLNESS")
											   where
											   i.ISSUE_TYPE_ID != (decimal)EHSIncidentTypeId.PreventativeAction
											   && i.INCIDENT_DT >= fromDate && i.DETECT_PLANT_ID > 0
											   select i).OrderBy(l => l.DETECT_PLANT_ID).ThenBy(l => l.INCIDENT_DT).ToList();

				List<PLANT> plantList = SQMModelMgr.SelectPlantList(entities, 1, 0);
				PLANT plant = null;

				// fetch all the plant accounting records for the target timespan
				PLANT_ACCOUNTING pa = null;
				List<PLANT_ACCOUNTING> paList = (from a in entities.PLANT_ACCOUNTING
												 where
												 EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) >= fromDate && EntityFunctions.CreateDateTime(a.PERIOD_YEAR, a.PERIOD_MONTH, 1, 0, 0, 0) <= rollupToDate
												 select a).OrderBy(l => l.PLANT_ID).ThenBy(l => l.PERIOD_YEAR).ThenBy(l => l.PERIOD_MONTH).ToList();

				List<EHSIncidentTimeAccounting> summaryList = new List<EHSIncidentTimeAccounting>();

				foreach (INCIDENT incident in incidentList)
				{
					WriteLine("Incident ID: " + incident.INCIDENT_ID.ToString() + "  Occur Date: " + Convert.ToDateTime(incident.INCIDENT_DT).ToShortDateString());
					incident.INCFORM_CAUSATION.Load();
					if (incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
						incident.INCFORM_LOSTTIME_HIST.Load();
					plant = plantList.Where(l => l.PLANT_ID == (decimal)incident.DETECT_PLANT_ID).FirstOrDefault();
					summaryList = EHSIncidentMgr.SummarizeIncidentAccounting(summaryList, EHSIncidentMgr.CalculateIncidentAccounting(incident, plant.LOCAL_TIMEZONE, workdays));
				}

				plant = null;
				PLANT_ACTIVE pact = null;
				DateTime periodDate;

				foreach (PLANT_ACCOUNTING pah in paList.OrderBy(l => l.PLANT_ID).ToList())
				{
					if (pact == null || pact.PLANT_ID != pah.PLANT_ID)
					{
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == pah.PLANT_ID && a.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident select a).SingleOrDefault();
					}
					//if (pact != null && pact.EFF_END_DATE.HasValue && new DateTime(pah.PERIOD_YEAR, pah.PERIOD_MONTH, 1).Date >= ((DateTime)pact.EFF_START_DATE).Date)
					if (pact != null && pact.EFF_START_DATE.HasValue && new DateTime(pah.PERIOD_YEAR, pah.PERIOD_MONTH, 1).Date >= ((DateTime)pact.EFF_START_DATE).Date)
					{
						pah.TIME_LOST = pah.TOTAL_DAYS_RESTRICTED = 0;
						pah.TIME_LOST_CASES = pah.RECORDED_CASES = pah.FIRST_AID_CASES = 0;
					}
				}

				plant = null;
				pact = null;
				foreach (EHSIncidentTimeAccounting period in summaryList.OrderBy(l => l.PlantID).ThenBy(l => l.PeriodYear).ThenBy(l => l.PeriodMonth).ToList())
				{
					if (plant == null || plant.PLANT_ID != period.PlantID)
					{
						plant = plantList.Where(l => l.PLANT_ID == period.PlantID).FirstOrDefault();
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == plant.PLANT_ID && a.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident select a).SingleOrDefault();
					}
					periodDate = new DateTime(period.PeriodYear, period.PeriodMonth, 1);
					if (pact != null && pact.EFF_START_DATE.HasValue && periodDate >= pact.EFF_START_DATE)
					{
						// write PLANT_ACCOUNTING metrics
						if ((pa = paList.Where(l => l.PLANT_ID == period.PlantID && l.PERIOD_YEAR == period.PeriodYear && l.PERIOD_MONTH == period.PeriodMonth).FirstOrDefault()) == null)
						{
							paList.Add((pa = new PLANT_ACCOUNTING()));
							pa.PLANT_ID = period.PlantID;
							pa.PERIOD_YEAR = period.PeriodYear;
							pa.PERIOD_MONTH = period.PeriodMonth;
						}
						pa.TIME_LOST = period.LostTime;
						pa.TOTAL_DAYS_RESTRICTED = period.RestrictedTime;
						pa.TIME_LOST_CASES = period.LostTimeCase;
						pa.RECORDED_CASES = period.RecordableCase;
						pa.FIRST_AID_CASES = period.FirstAidCase;
						pa.LAST_UPD_DT = DateTime.UtcNow;
						pa.LAST_UPD_BY = "automated";

						EHSModel.UpdatePlantAccounting(entities, pa);
					}
				}

				WriteLine("INCIDENT Rollup Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			}
			catch (Exception ex)
			{
				WriteLine("INCIDENT RollUp Error: " + ex.ToString());
			}

			return nextStep;
		}
		#endregion

		#region environmental
		static string ProcessEnvironmental()
		{
			PSsqmEntities entities = new PSsqmEntities();
			SETTINGS setting = null;
			string nextStep = "";
			DateTime toDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
			DateTime fromDate = toDate.AddMonths(-3);
			int status = 0;
			DateTime currencyDate = DateTime.MinValue;

			WriteLine("ENVIRONMENTAL Rollup Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", ""); // ABW 20140805

			try
			{
				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_ENV_PERIODSPAN").FirstOrDefault();
				if (setting != null)
					fromDate = toDate.AddMonths(Convert.ToInt32(setting.VALUE) * -1);

				setting = sets.Where(x => x.SETTING_CD == "ROLLUP_ENV_NEXTPAGE").FirstOrDefault();
				if (setting != null && !string.IsNullOrEmpty(setting.VALUE) && setting.VALUE.Length > 1)
				{
					nextStep = setting.VALUE;
				}

				CURRENCY_XREF latestCurrency = CurrencyMgr.GetLatestRecord(entities);
				if (latestCurrency != null)
				{
					currencyDate = new DateTime(latestCurrency.EFF_YEAR, latestCurrency.EFF_MONTH, DateTime.DaysInMonth(latestCurrency.EFF_YEAR, latestCurrency.EFF_MONTH));
				}
				WriteLine("Max Currency Date = " + currencyDate.ToShortDateString());

				List<EHSProfile> profileList = new List<EHSProfile>();
				foreach (decimal plantID in (from p in entities.EHS_PROFILE select p.PLANT_ID).ToList())
				{
					profileList.Add(new EHSProfile().Load(Convert.ToDecimal(plantID), false, true));
				}

				foreach (EHSProfile profile in profileList)		// do each plant having a metric profile
				{
					WriteLine(profile.Plant.PLANT_NAME);
					DateTime periodDate = fromDate;

					while (periodDate <= toDate)				// do each month within the rollup span
					{
						WriteLine(" " + periodDate.Year.ToString() + "/" + periodDate.Month.ToString());
						if (profile.InputPeriod == null || profile.InputPeriod.PeriodDate != periodDate)
							profile.LoadPeriod(periodDate);

						if (profile.ValidPeriod())
						{
							if (!profile.InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
							{
								profile.InputPeriod.PlantAccounting.APPROVAL_DT = toDate;
								profile.InputPeriod.PlantAccounting.APPROVER_ID = 1m;   // default to the sysadmin user
							}
							status = profile.UpdateMetricHistory(periodDate);  // new roll-up logic 
							WriteLine(" ... " + status.ToString());
							periodDate = periodDate.AddMonths(1);
						}
					}
				}

				WriteLine("ENVIRONMENTAL Rollup Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			}
			catch (Exception ex)
			{
				WriteLine("ENVIRONMENTAL RollUp Error: " + ex.ToString());
			}

			return nextStep;
		}
		#endregion

		#region ehsdata
		static string ProcessEHSData()
		{
			string nextStep = "";

			WriteLine("EHSDATA Rollup Started: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));

			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUTOMATE", "TASK"); // ABW 20140805
			try
			{
				using (var entities = new PSsqmEntities())
				{
					long updateIndicator = DateTime.UtcNow.Ticks;

					decimal plantManagerAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30003").MEASURE_ID;
					decimal ehsAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30001").MEASURE_ID;
					decimal supervisorAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30002").MEASURE_ID;
					var auditMeasureIDs = new List<decimal>()
					{
						plantManagerAuditsMeasureID,
						ehsAuditsMeasureID,
						supervisorAuditsMeasureID
					};

					decimal nearMissMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20002").MEASURE_ID;
					decimal firstAidMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20003").MEASURE_ID;
					decimal recordableMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20004").MEASURE_ID;
					decimal lostTimeCaseMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20005").MEASURE_ID;
					decimal fatalityMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20006").MEASURE_ID;
					decimal closedInvestigationMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20007").MEASURE_ID;
					var incidentMeasureIDs = new List<decimal>()
					{
						nearMissMeasureID,
						firstAidMeasureID,
						recordableMeasureID,
						lostTimeCaseMeasureID,
						fatalityMeasureID,
						closedInvestigationMeasureID
					};

					decimal timeLostMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S60001").MEASURE_ID;
					decimal timeRestrictedMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S60003").MEASURE_ID;
					var incidentMonthlyMeasureIDs = new List<decimal>()
					{
						timeLostMeasureID,
						timeRestrictedMeasureID
					};

					decimal injuryIllnessIssueTypeID = entities.INCIDENT_TYPE.First(i => i.TITLE == "Injury/Illness").INCIDENT_TYPE_ID;
					decimal nearMissIssueTypeID = entities.INCIDENT_TYPE.First(i => i.TITLE == "Near Miss").INCIDENT_TYPE_ID;

					var closedAudits = entities.AUDIT.Where(a => a.CURRENT_STATUS == "C");
					var incidents = entities.INCIDENT.Include("INCFORM_INJURYILLNESS").Where(i => i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID || i.ISSUE_TYPE_ID == nearMissIssueTypeID);
					var activePlants = entities.PLANT_ACTIVE.Where(p => p.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident &&
						closedAudits.Select(a => a.DETECT_PLANT_ID).Concat(incidents.Select(i => i.DETECT_PLANT_ID)).Distinct().Contains(p.PLANT_ID));
					foreach (var activePlant in activePlants)
					{
						// AUDITS
						var closedAuditsForPlant = closedAudits.Where(a => a.DETECT_PLANT_ID == activePlant.PLANT_ID);
						var minDate = new[] { activePlant.EFF_START_DATE, closedAuditsForPlant.Min(a => a.CLOSE_DATE_DATA_COMPLETE) }.Max();
						var maxDate = new[] { activePlant.EFF_END_DATE, closedAuditsForPlant.Max(a => a.CLOSE_DATE_DATA_COMPLETE) }.Min();

						if (minDate.HasValue && maxDate.HasValue)
							for (var currDate = minDate.Value; currDate <= maxDate; currDate = currDate.AddDays(1))
							{
								int plantManagerAudits = 0;
								int ehsAudits = 0;
								int supervisorAudits = 0;

								var closedAuditsForDay = closedAuditsForPlant.Where(a => EntityFunctions.TruncateTime(a.CLOSE_DATE_DATA_COMPLETE) == currDate.Date);
								if (closedAuditsForDay.Any())
								{
									plantManagerAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE_ID == 1);
									ehsAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE_ID == 2);
									supervisorAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE_ID == 3);
								}

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, activePlant.PLANT_ID, currDate, auditMeasureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, plantManagerAuditsMeasureID, plantManagerAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, ehsAuditsMeasureID, ehsAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, supervisorAuditsMeasureID, supervisorAudits, updateIndicator);
								foreach (var data in dataList)
									if (data.EntityState == EntityState.Detached && data.VALUE != 0)
										entities.EHS_DATA.AddObject(data);
									else if (data.EntityState != EntityState.Detached && data.VALUE == 0)
										entities.DeleteObject(data);
							}

						// INCIDENTS
						var incidentsForPlant = incidents.Where(i => i.DETECT_PLANT_ID == activePlant.PLANT_ID);
						minDate = new[] { activePlant.EFF_START_DATE, incidentsForPlant.Min(i => (DateTime?)i.INCIDENT_DT) }.Max();
						maxDate = new[] { activePlant.EFF_END_DATE, incidentsForPlant.Max(i => (DateTime?)i.INCIDENT_DT) }.Min();

						if (minDate.HasValue && maxDate.HasValue)
							for (var currDate = minDate.Value; currDate <= maxDate; currDate = currDate.AddDays(1))
							{
								int nearMisses = 0;
								int firstAidCases = 0;
								int recordables = 0;
								int lostTimeCases = 0;
								int fatalities = 0;
								int closedInvestigations = 0;

								var firstAidOrdinals = new Dictionary<string, Dictionary<string, int>>()
								{
									{ "type", null },
									{ "bodyPart", null },
									{ "rootCause", null },
									{ "tenure", null },
									{ "daysToClose", null }
								};
								var recordableOrdinals = new Dictionary<string, Dictionary<string, int>>()
								{
									{ "type", null },
									{ "bodyPart", null },
									{ "rootCause", null },
									{ "tenure", null },
									{ "daysToClose", null }
								};

								var incidentsForDay = incidentsForPlant.Where(i => EntityFunctions.TruncateTime(i.INCIDENT_DT) == currDate.Date);
								if (incidentsForDay.Any())
								{
									var firstAidIncidents = incidentsForDay.Where(i => i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID && i.INCFORM_INJURYILLNESS.FIRST_AID);
									var recordableIncidents = incidentsForDay.Where(i => i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID && i.INCFORM_INJURYILLNESS.RECORDABLE);

									// Basic data
									nearMisses = incidentsForDay.Count(i => i.ISSUE_TYPE_ID == nearMissIssueTypeID);
									firstAidCases = firstAidIncidents.Count();
									recordables = recordableIncidents.Count();
									lostTimeCases = incidentsForDay.Count(i => i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID && i.INCFORM_INJURYILLNESS.LOST_TIME);
									fatalities = incidentsForDay.Count(i =>
										i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID && i.INCFORM_INJURYILLNESS.FATALITY.HasValue && i.INCFORM_INJURYILLNESS.FATALITY.Value);
									closedInvestigations = incidentsForDay.Count(i => i.CLOSE_DATE.HasValue);

									// First Aid ordinals
									// check which ordinal data we wish to capture
									SETTINGS setFirstAid = sets.Where(s => s.SETTING_CD == "FIRSTAID-ORDINALS").FirstOrDefault();
									if (setFirstAid != null && setFirstAid.VALUE.Contains("type"))
										firstAidOrdinals["type"] = firstAidIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.INJURY_TYPE).ToDictionary(t => t.Key ?? "", t => t.Count());
									if (setFirstAid != null && setFirstAid.VALUE.Contains("bodyPart"))
										firstAidOrdinals["bodyPart"] = firstAidIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.INJURY_BODY_PART).ToDictionary(b => b.Key ?? "", b => b.Count());
									if (setFirstAid != null && setFirstAid.VALUE.Contains("rootCause"))
										firstAidOrdinals["rootCause"] = firstAidIncidents.SelectMany(i => i.INCFORM_CAUSATION).GroupBy(c => c.CAUSEATION_CD).ToDictionary(c =>
										c.Key ?? "", c => c.Count());
									if (setFirstAid != null && setFirstAid.VALUE.Contains("tenure"))
										firstAidOrdinals["tenure"] = firstAidIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.JOB_TENURE).ToDictionary(t => t.Key ?? "", t => t.Count());
									if (setFirstAid != null && setFirstAid.VALUE.Contains("daysToClose"))
										firstAidOrdinals["daysToClose"] = firstAidIncidents.Where(i => i.CLOSE_DATE.HasValue).Select(i =>
										EntityFunctions.DiffDays(i.INCIDENT_DT, i.CLOSE_DATE)).Select(d => entities.XLAT_DAYS_TO_CLOSE_TRANS.FirstOrDefault(x =>
										(x.MIN_DAYS.HasValue ? d >= x.MIN_DAYS : true) && (x.MAX_DAYS.HasValue ? d <= x.MAX_DAYS : true)).XLAT_CODE).GroupBy(x => x).ToDictionary(x =>
										x.Key ?? "", x => x.Count());

									// Recordable ordinals
									// check which ordinal data we wish to capture
									SETTINGS setRecordable = sets.Where(s => s.SETTING_CD == "RECORDABLE-ORDINALS").FirstOrDefault();
									if (setRecordable != null && setRecordable.VALUE.Contains("type"))
										recordableOrdinals["type"] = recordableIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.INJURY_TYPE).ToDictionary(t => t.Key ?? "", t => t.Count());
									if (setRecordable != null && setRecordable.VALUE.Contains("bodyPart"))
										recordableOrdinals["bodyPart"] = recordableIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.INJURY_BODY_PART).ToDictionary(b => b.Key ?? "", b => b.Count());
									if (setRecordable != null && setRecordable.VALUE.Contains("rootCause"))
										recordableOrdinals["rootCause"] = recordableIncidents.SelectMany(i => i.INCFORM_CAUSATION).GroupBy(c => c.CAUSEATION_CD).ToDictionary(c =>
										c.Key ?? "", c => c.Count());
									if (setRecordable != null && setRecordable.VALUE.Contains("tenure"))
										recordableOrdinals["tenure"] = recordableIncidents.GroupBy(i => i.INCFORM_INJURYILLNESS.JOB_TENURE).ToDictionary(t => t.Key ?? "", t => t.Count());
									if (setRecordable != null && setRecordable.VALUE.Contains("daysToClose"))
										recordableOrdinals["daysToClose"] = recordableIncidents.Where(i => i.CLOSE_DATE.HasValue).Select(i =>
										EntityFunctions.DiffDays(i.INCIDENT_DT, i.CLOSE_DATE)).Select(d => entities.XLAT_DAYS_TO_CLOSE_TRANS.FirstOrDefault(x =>
										(x.MIN_DAYS.HasValue ? d >= x.MIN_DAYS : true) && (x.MAX_DAYS.HasValue ? d <= x.MAX_DAYS : true)).XLAT_CODE).GroupBy(x => x).ToDictionary(x =>
										x.Key ?? "", x => x.Count());
								}

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, activePlant.PLANT_ID, currDate, incidentMeasureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, nearMissMeasureID, nearMisses, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, firstAidMeasureID, firstAidCases, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, recordableMeasureID, recordables, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, lostTimeCaseMeasureID, lostTimeCases, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, fatalityMeasureID, fatalities, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, closedInvestigationMeasureID, closedInvestigations, updateIndicator);
								foreach (var data in dataList)
									if (data.VALUE != 0)
									{
										if (data.EntityState == EntityState.Detached)
											entities.EHS_DATA.AddObject(data);
										if (incidentsForDay.Any())
										{
											if (data.MEASURE_ID == firstAidMeasureID)
												UpdateOrdinalData(entities, data, firstAidOrdinals);
											else if (data.MEASURE_ID == recordableMeasureID)
												UpdateOrdinalData(entities, data, recordableOrdinals);
										}
									}
									else if (data.EntityState != EntityState.Detached && data.VALUE == 0)
									{
										if (data.MEASURE_ID == firstAidMeasureID)
										{
											foreach (var key in firstAidOrdinals.Keys.ToArray())
												firstAidOrdinals[key] = new Dictionary<string, int>();
											UpdateOrdinalData(entities, data, firstAidOrdinals);
										}
										else if (data.MEASURE_ID == recordableMeasureID)
										{
											foreach (var key in recordableOrdinals.Keys.ToArray())
												recordableOrdinals[key] = new Dictionary<string, int>();
											UpdateOrdinalData(entities, data, recordableOrdinals);
										}
										entities.DeleteObject(data);
									}
							}

						// MONTHLY INCIDENTS (from PLANT_ACCOUNTING)
						var accountingForPlant = entities.PLANT_ACCOUNTING.Where(a => a.PLANT_ID == activePlant.PLANT_ID);
						minDate = new[] { activePlant.EFF_START_DATE, accountingForPlant.AsEnumerable().Min(i => (DateTime?)new DateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1)) }.Max();
						maxDate = new[] { activePlant.EFF_END_DATE, accountingForPlant.AsEnumerable().Max(i => (DateTime?)new DateTime(i.PERIOD_YEAR, i.PERIOD_MONTH, 1)) }.Min();

						if (minDate.HasValue && maxDate.HasValue)
						{
							minDate = new DateTime(minDate.Value.Year, minDate.Value.Month, 1);
							maxDate = new DateTime(maxDate.Value.Year, maxDate.Value.Month, 1);
							for (var currDate = minDate.Value; currDate <= maxDate; currDate = currDate.AddMonths(1))
							{
								decimal timeLost = 0;
								decimal timeRestricted = 0;

								var accountingForMonth = accountingForPlant.FirstOrDefault(a => a.PERIOD_YEAR == currDate.Year && a.PERIOD_MONTH == currDate.Month);
								if (accountingForMonth != null)
								{
									timeLost = accountingForMonth.TIME_LOST ?? 0;
									timeRestricted = accountingForMonth.TOTAL_DAYS_RESTRICTED ?? 0;
								}

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, activePlant.PLANT_ID, currDate, incidentMonthlyMeasureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, timeLostMeasureID, timeLost, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, timeRestrictedMeasureID, timeRestricted, updateIndicator);
								foreach (var data in dataList)
									if (data.EntityState == EntityState.Detached && data.VALUE != 0)
										entities.EHS_DATA.AddObject(data);
									else if (data.EntityState != EntityState.Detached && data.VALUE == 0)
										entities.DeleteObject(data);
							}
						}
					}

					entities.SaveChanges();
				}

				WriteLine("EHSDATA Rollup Completed: " + DateTime.UtcNow.ToString("hh:mm MM/dd/yyyy"));
			}
			catch (Exception ex)
			{
				WriteLine("EHSDATA RollUp Error: " + ex.ToString());
			}

			return nextStep;
		}

		static dynamic GetOrdinalTypes(PSsqmEntities entities, string groupName)
		{
			return from x in entities.XLAT
				   where x.XLAT_GROUP == groupName && x.XLAT_LANGUAGE == "en" && x.STATUS == "A"
				   select new { x.XLAT_GROUP, x.XLAT_CODE, x.DESCRIPTION };
		}

		static void UpdateOrdinalData(PSsqmEntities entities, EHS_DATA ehs_data, dynamic types, Dictionary<string, int> type_data)
		{
			if (type_data != null)  // might be null if ordinal data type not activated in the FIRSTAID-ORDINALS or RECORDABLE-ORDINALS SETTINGS table
			{
				foreach (var t in types)
				{
					string group = t.XLAT_GROUP;
					string code = t.XLAT_CODE;
					var data = entities.EHS_DATA_ORD.FirstOrDefault(d => d.DATA_ID == ehs_data.DATA_ID && d.XLAT_GROUP == group && d.XLAT_CODE == code);
					if (type_data.ContainsKey(code))
					{
						if (data == null)
							entities.EHS_DATA_ORD.AddObject(new EHS_DATA_ORD()
							{
								EHS_DATA = ehs_data,
								XLAT_GROUP = t.XLAT_GROUP,
								XLAT_CODE = t.XLAT_CODE,
								VALUE = type_data[code]
							});
						else
							data.VALUE = type_data[code];
					}
					else if (data != null)
						entities.DeleteObject(data);
				}
			}
		}

		static void UpdateOrdinalData(PSsqmEntities entities, EHS_DATA ehs_data, Dictionary<string, Dictionary<string, int>> data)
		{
			// Get the types first.
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			UpdateOrdinalData(entities, ehs_data, types, data["type"]);
			UpdateOrdinalData(entities, ehs_data, bodyParts, data["bodyPart"]);
			UpdateOrdinalData(entities, ehs_data, rootCauses, data["rootCause"]);
			UpdateOrdinalData(entities, ehs_data, tenures, data["tenure"]);
			UpdateOrdinalData(entities, ehs_data, daysToCloses, data["daysToClose"]);
		}
		#endregion ehsdata

		#region logs
		static void WriteLogFile()
		{
			try
			{
				string logPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("DataRollup_{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
				File.WriteAllText(fullPath, output.ToString());

				// Keep only last 100 log files
				int maxFiles = 100;
				var info = new DirectoryInfo(logPath);
				FileInfo[] files = info.GetFiles("*.txt").OrderBy(f => f.CreationTime).ToArray();
				if (files.Count() > maxFiles)
					for (int i = 0; i < files.Count() - maxFiles; i++)
						File.Delete(logPath + files[i].Name);
			}
			catch (Exception ex)
			{
				WriteLine("WriteLogFile Error: " + ex.ToString());
				try
				{
					WriteLine("WriteLogFile Detailed Error: " + ex.StackTrace.ToString());
				}
				catch { }
			}
		}

		static void WriteLine(string text)
		{
			output.AppendLine(text);
		}

		#endregion
	}
}
