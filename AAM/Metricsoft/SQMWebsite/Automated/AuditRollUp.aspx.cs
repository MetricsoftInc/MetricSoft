using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website.Automated
{
	public partial class AuditRollUp : Page
	{
		StringBuilder output = new StringBuilder();

		protected void Page_Load(object sender, EventArgs e)
		{
			output.AppendFormat("Started: {0:hh:mm MM/dd/yyyy}", DateTime.Now);

			try
			{
				using (var entities = new PSsqmEntities())
				{
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
							int plantManagerAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "Plant Manager Audit");
							int ehsAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "DGA Safety Audit");
							int supervisorAudits = closedAuditsForDay.Count(a => a.AUDIT_TYPE1.TITLE == "AAM Supervisor Safety Audit");
						}
					}
				}
			}
			catch (Exception ex)
			{
				output.AppendFormat("Main Incident RollUp Error - {0}", ex);
			}
		}
	}
}
