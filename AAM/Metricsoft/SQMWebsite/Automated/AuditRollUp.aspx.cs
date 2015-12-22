using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace SQM.Website.Automated
{
	public partial class AuditRollUp : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var output = new StringBuilder();
			output.AppendFormat("Started: {0:hh:mm MM/dd/yyyy}", DateTime.Now);
			output.AppendLine();

			try
			{
				using (var entities = new PSsqmEntities())
				{
					long updateIndicator = DateTime.UtcNow.Ticks;

					decimal plantManagerAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30003").MEASURE_ID;
					decimal ehsAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30001").MEASURE_ID;
					decimal supervisorAuditsMeasureID = entities.EHS_MEASURE.First(m => m.MEASURE_CD == "S30002").MEASURE_ID;
					var measureIDs = new List<decimal>()
					{
						plantManagerAuditsMeasureID,
						ehsAuditsMeasureID,
						supervisorAuditsMeasureID
					};

					var closedAudits = entities.AUDIT.Where(a => a.CURRENT_STATUS == "C");
					var activePlants = entities.PLANT_ACTIVE.Where(p => closedAudits.Select(a => a.DETECT_PLANT_ID).Distinct().Contains(p.PLANT_ID));
					foreach (var activePlant in activePlants)
					{
						var closedAuditsForPlant = closedAudits.Where(a => a.DETECT_PLANT_ID == activePlant.PLANT_ID);
						var minDate = new[] { activePlant.EFF_START_DATE, closedAuditsForPlant.Min(a => a.CLOSE_DATE_DATA_COMPLETE) }.Max();
						var maxDate = new[] { activePlant.EFF_END_DATE, closedAuditsForPlant.Max(a => a.CLOSE_DATE_DATA_COMPLETE) }.Min();

						if (!minDate.HasValue || !maxDate.HasValue)
							continue;

						for (var currDate = minDate.Value; currDate <= maxDate; currDate = currDate.AddDays(1))
						{
							var closedAuditsForDay = closedAuditsForPlant.Where(a => EntityFunctions.TruncateTime(a.CLOSE_DATE_DATA_COMPLETE) == currDate.Date);
							if (closedAuditsForDay.Any())
							{
								int plantManagerAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "Plant Manager Audit");
								int ehsAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "DGA Safety Audit");
								int supervisorAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "AAM Supervisor Safety Audit");

								var dataList = EHSDataMapping.SelectEHSDataPeriodList(entities, activePlant.PLANT_ID, currDate, measureIDs, true, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, plantManagerAuditsMeasureID, plantManagerAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, ehsAuditsMeasureID, ehsAudits, updateIndicator);
								EHSDataMapping.SetEHSDataValue(dataList, supervisorAuditsMeasureID, supervisorAudits, updateIndicator);
								foreach (var data in dataList)
									if (data.EntityState == EntityState.Detached && data.VALUE != 0)
										entities.EHS_DATA.AddObject(data);
							}
						}
					}

					entities.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				output.AppendFormat("Audit RollUp Error - {0}", ex);
			}

			output.AppendLine();
			output.AppendFormat("Completed: {0:hh:mm MM/dd/yyyy}", DateTime.Now);

			this.lblOutput.Text = output.ToString().Replace("\n", "<br>");
		}
	}
}
