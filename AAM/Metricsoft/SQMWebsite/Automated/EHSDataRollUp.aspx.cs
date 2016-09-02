using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using SQM.Website;
using SQM.Website.Shared;

namespace SQM.Website.Automated
{
	public partial class EHSDataRollUp : System.Web.UI.Page
	{
		static StringBuilder output;
		static DateTime fromDate;
		static List<SETTINGS> sets;

		/// <summary>
		/// Gets all the types for an ordinal from the XLAT table.
		/// </summary>
		/// <param name="entities">The entities to get the table from.</param>
		/// <param name="groupName">The group name to get the types for.</param>
		/// <returns>An IEnumerable of an anonymous type as a dynamic.</returns>
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

		protected void Page_Load(object sender, EventArgs e)
		{
			string pageMode = "";
			if (!string.IsNullOrEmpty(Request.QueryString["m"]))   // .../...aspx?p=xxxxx
			{
				pageMode = Request.QueryString["m"].ToLower();  // page mode (web == running manually from the menu)
			}

			output = new StringBuilder();
			output.AppendFormat("Started: {0:hh:mm MM/dd/yyyy}", DateTime.UtcNow);
			output.AppendLine();

			try
			{
				using (var entities = new PSsqmEntities())
				{
					long updateIndicator = DateTime.UtcNow.Ticks;
					SETTINGS setting = null;

					// get any AUTOMATE settings
					sets = SQMSettings.SelectSettingsGroup("AUTOMATE", "TASK");

					DateTime rollupFromDate = DateTime.UtcNow.AddMonths(-6);	// this should be a setting 
					// set end date to end of current month to clear spurrious entries ?
					DateTime rollupToDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month));

					/*
					int rollupMonthsAhead = 0;
					setting = sets.Where(x => x.SETTING_CD == "ROLLUP_MONTHS_AHEAD").FirstOrDefault();
					if (setting != null && !string.IsNullOrEmpty(setting.VALUE))
					{
						int.TryParse(setting.VALUE, out rollupMonthsAhead);
						rollupToDate = rollupToDate.AddMonths(rollupMonthsAhead);
					}
					*/

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
					decimal otherIncidentsID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20001").MEASURE_ID;
					decimal closedInvestigationMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S20007").MEASURE_ID;
					var incidentMeasureIDs = new List<decimal>()
					{
						nearMissMeasureID,
						firstAidMeasureID,
						recordableMeasureID,
						lostTimeCaseMeasureID,
						fatalityMeasureID,
						otherIncidentsID,
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


					List<PLANT> plantList = SQMModelMgr.SelectPlantList(entities, 1, 0).Where(l => l.STATUS == "A").ToList();
					PLANT_ACTIVE pact = null;

					var closedAudits = entities.AUDIT.Where(a => a.CURRENT_STATUS == "C");
					var incidents = entities.INCIDENT.Include("INCFORM_INJURYILLNESS").Where(i => i.ISSUE_TYPE_ID == injuryIllnessIssueTypeID || i.ISSUE_TYPE_ID == nearMissIssueTypeID);
					var activePlants = entities.PLANT_ACTIVE.Where(p => p.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident &&
						closedAudits.Select(a => a.DETECT_PLANT_ID).Concat(incidents.Select(i => i.DETECT_PLANT_ID)).Distinct().Contains(p.PLANT_ID));


					// AUDITS
					foreach (var plant in plantList)
					{
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == plant.PLANT_ID && a.RECORD_TYPE == (int)TaskRecordType.Audit select a).SingleOrDefault();
						if (pact != null  &&  pact.EFF_START_DATE.HasValue)	// plant is active
						{
							var closedAuditsForPlant = closedAudits.Where(a => a.DETECT_PLANT_ID == plant.PLANT_ID);

							// new timespan logic 
							DateTime fromDate = rollupFromDate > (DateTime)pact.EFF_START_DATE ? rollupFromDate : (DateTime)pact.EFF_START_DATE;
							DateTime toDate = pact.EFF_END_DATE.HasValue && (DateTime)pact.EFF_END_DATE < rollupToDate ? (DateTime)pact.EFF_END_DATE : rollupToDate;

							WriteLine("AUDIT Rollup For Plant " + pact.PLANT_ID + "  from date = " + fromDate.ToShortDateString() + "  to date = " + toDate.ToShortDateString());

							for (var currDate = fromDate; currDate <= toDate; currDate = currDate.AddDays(1))
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

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, plant.PLANT_ID, currDate, auditMeasureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, plantManagerAuditsMeasureID, plantManagerAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, ehsAuditsMeasureID, ehsAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, supervisorAuditsMeasureID, supervisorAudits, updateIndicator);
								foreach (var data in dataList)
									if (data.EntityState == EntityState.Detached && data.VALUE != 0)
										entities.EHS_DATA.AddObject(data);
									else if (data.EntityState != EntityState.Detached && data.VALUE == 0)
										entities.DeleteObject(data);
							}
						}
					}

					// INCIDENTS
					foreach (var plant in plantList)
					{
						pact = (from a in entities.PLANT_ACTIVE where a.PLANT_ID == plant.PLANT_ID && a.RECORD_TYPE == (int)TaskRecordType.HealthSafetyIncident select a).SingleOrDefault();
						if (pact != null && pact.EFF_START_DATE.HasValue)	// plant is active
						{
							var incidentsForPlant = incidents.Where(i => i.DETECT_PLANT_ID == plant.PLANT_ID);
							//var minDate = new[] { pact.EFF_START_DATE, incidentsForPlant.Min(i => (DateTime?)i.INCIDENT_DT) }.Max();
							//var maxDate = new[] { pact.EFF_END_DATE, incidentsForPlant.Max(i => (DateTime?)i.INCIDENT_DT) }.Min();
							// new timespan logic 
							DateTime fromDate = rollupFromDate > (DateTime)pact.EFF_START_DATE ? rollupFromDate : (DateTime)pact.EFF_START_DATE;
							DateTime toDate = pact.EFF_END_DATE.HasValue && (DateTime)pact.EFF_END_DATE < rollupToDate ? (DateTime)pact.EFF_END_DATE : rollupToDate;

							WriteLine("INCIDENT Rollup For Plant " + pact.PLANT_ID + "  from date = " + fromDate.ToShortDateString() + "  to date = " + toDate.ToShortDateString());

							for (var currDate = fromDate; currDate <= toDate; currDate = currDate.AddDays(1))
							{
								int nearMisses = 0;
								int firstAidCases = 0;
								int recordables = 0;
								int lostTimeCases = 0;
								int fatalities = 0;
								int otherIncidents = 0;
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
									otherIncidents = incidentsForDay.Where(i => i.ISSUE_TYPE_ID != injuryIllnessIssueTypeID).Count();
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

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, plant.PLANT_ID, currDate, incidentMeasureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, nearMissMeasureID, nearMisses, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, firstAidMeasureID, firstAidCases, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, recordableMeasureID, recordables, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, lostTimeCaseMeasureID, lostTimeCases, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, fatalityMeasureID, fatalities, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, otherIncidentsID, otherIncidents, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, closedInvestigationMeasureID, closedInvestigations, updateIndicator);
								foreach (var data in dataList)
								{
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
							}

							// MONTHLY INCIDENTS (from PLANT_ACCOUNTING)
							var accountingForPlant = entities.PLANT_ACCOUNTING.Where(a => a.PLANT_ID == plant.PLANT_ID);
							List<EHS_DATA> ehsdataList;

							for (var currDate = fromDate; currDate <= toDate; currDate = currDate.AddDays(1))
							{
								decimal timeLost = 0;
								decimal timeRestricted = 0;

								if (currDate.Day == 1)
								{
									// get or create data records for the 1st day of the month
									ehsdataList = EHSDataMapping.SelectEHSDataPeriodList(entities, plant.PLANT_ID, currDate, incidentMonthlyMeasureIDs, true, updateIndicator);
									var accountingForMonth = accountingForPlant.FirstOrDefault(a => a.PERIOD_YEAR == currDate.Year && a.PERIOD_MONTH == currDate.Month);
									if (accountingForMonth != null)
									{
										timeLost = accountingForMonth.TIME_LOST ?? 0;
										timeRestricted = accountingForMonth.TOTAL_DAYS_RESTRICTED ?? 0;
									}
									EHSDataMapping.SetEHSDataValue(ehsdataList, timeLostMeasureID, timeLost, updateIndicator);
									EHSDataMapping.SetEHSDataValue(ehsdataList, timeRestrictedMeasureID, timeRestricted, updateIndicator);
									WriteLine("ACCOUNTING Rollup For Plant " + pact.PLANT_ID + " date = " + currDate.ToShortDateString());
								}
								else
								{
									// get any spurrious data that might have been entered manually. we will want to delete these
									ehsdataList = EHSDataMapping.SelectEHSDataPeriodList(entities, plant.PLANT_ID, currDate, incidentMonthlyMeasureIDs, false, updateIndicator);
								}

								foreach (var data in ehsdataList)
								{
									if (data.EntityState == EntityState.Detached && data.VALUE.HasValue && currDate.Day == 1)
										entities.EHS_DATA.AddObject(data);
									else if (data.EntityState != EntityState.Detached && currDate.Day != 1)
										entities.DeleteObject(data);
								}
							}
						}
					}

					entities.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				output.AppendFormat("EHS Data RollUp Error - {0}", ex);
			}

			output.AppendLine();
			output.AppendFormat("Completed: {0:hh:mm MM/dd/yyyy}", DateTime.UtcNow);

			WriteLogFile();

			this.lblOutput.Text = output.ToString().Replace("\n", "<br>");
			if (pageMode != "web")
			{
				System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "closePage", "window.onunload = CloseWindow();", true);
			}
		}

		static void WriteLine(string text)
		{
			output.AppendLine(text);
		}

		static void WriteLogFile()
		{
			try
			{
				string logPath = HttpContext.Current.Server.MapPath("~") + "\\log\\";
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);

				// Write log file
				string fullPath = logPath + string.Format("{0:yyyy-MM-dd-HHmmssfff}.txt", DateTime.UtcNow);
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
				WriteLine("WriteLogFile Detailed Error: " + ex.InnerException.ToString());
			}
		}
	}
}
