using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace SQM.Website.EHS
{
	public partial class EHS_HistoricalImport : Page
	{
		PSsqmEntities entities;

		protected void Page_Load(object sender, EventArgs e)
		{
			using (this.entities = new PSsqmEntities())
			{
				var measures = (from m in this.entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1"
							   select new { m.MEASURE_ID, m.MEASURE_CD }).ToDictionary(m => m.MEASURE_CD, m => m.MEASURE_ID);

				foreach (var history in this.entities.PLANT_ACCOUNTING)
				{
					var date = new DateTime(history.PERIOD_YEAR, history.PERIOD_MONTH, 1);

					this.AddToData(history.PLANT_ID, measures["S60002"], date, history.TIME_WORKED);
					this.AddToData(history.PLANT_ID, measures["S60001"], date, history.TIME_LOST);
					this.AddToData(history.PLANT_ID, measures["S20005"], date, history.TIME_LOST_CASES);
					this.AddToData(history.PLANT_ID, measures["S60003"], date, history.TOTAL_DAYS_RESTRICTED);
					this.AddToData(history.PLANT_ID, measures["S20004"], date, history.RECORDED_CASES);
					this.AddToData(history.PLANT_ID, measures["S20003"], date, history.FIRST_AID_CASES);
					this.AddToData(history.PLANT_ID, measures["S30002"], date, history.LEADERSHIP_WALKS);
					this.AddToData(history.PLANT_ID, measures["S40003"], date, history.JOB_SILL_ANALYSIS);
					this.AddToData(history.PLANT_ID, measures["S50001"], date, history.SAFETY_TRAIN_SESSIONS);
				}

				this.entities.SaveChanges();
			}
		}

		void AddToData(decimal plantID, decimal measureID, DateTime date, decimal? value)
		{
			if (value.HasValue && value.Value != 0.0m)
				this.entities.EHS_DATA.AddObject(new EHS_DATA()
				{
					MEASURE_ID = measureID,
					PLANT_ID = plantID,
					DATE = date,
					VALUE = value
				});
		}
	}
}
