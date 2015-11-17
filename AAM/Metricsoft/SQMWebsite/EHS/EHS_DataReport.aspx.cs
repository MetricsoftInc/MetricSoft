using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace SQM.Website.EHS
{
	public partial class EHS_DataReport : SQMBasePage
	{
		// Stores a reference to the entities so it can persist for the entire page's life cycle.
		PSsqmEntities entities;

		// Stores the measure headers to use later in the dateSelection_SelectedDateChanged method.
		Dictionary<string, string> measureHeaders = new Dictionary<string, string>();

		protected void Page_Load(object sender, EventArgs e)
		{
			this.entities = new PSsqmEntities();

			// This is done outside of IsPostBack because we need to get the header names every time, Telerik's RadGrid loses the header text on postback.
			var measures = from m in this.entities.EHS_MEASURE
						   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.FREQUENCY != "M"
						   orderby m.MEASURE_CD
						   select new { m.MEASURE_NAME, m.MEASURE_ID, m.DATA_TYPE };
			this.measureHeaders = measures.ToDictionary(m => string.Format("measure_{0}_{1}", m.DATA_TYPE, m.MEASURE_ID), m => m.MEASURE_NAME);

			if (!this.IsPostBack)
			{
				// This creates all the measure columns in the RadGrid.
				foreach (var measure in measures)
					this.rgData.MasterTableView.Columns.Add(new GridBoundColumn()
					{
						HeaderText = measure.MEASURE_NAME,
						UniqueName = string.Format("measure_{0}_{1}", measure.DATA_TYPE, measure.MEASURE_ID)
					});

				var data = (from pl in this.entities.PLANT
							join pers in this.entities.PERSON on pl.PLANT_ID equals pers.PLANT_ID into per
							from pe in per.DefaultIfEmpty()
							where pe == null || pe.PRIV_GROUP == "EHS-MANAGER"
							select new { PlantID = pl.PLANT_ID, PlantName = pl.PLANT_NAME, PersonLastName = pe.LAST_NAME }).ToList();
				data.Add(new { PlantID = -1m, PlantName = "TOTALS", PersonLastName = "" });

				this.rgData.DataSource = data;
				this.rgData.DataBind();

				this.rdpEndOfWeek.SelectedDate = DateTime.Today;
				this.rdpEndOfWeek_SelectedDateChanged(this.rdpEndOfWeek, new SelectedDateChangedEventArgs(null, DateTime.Today));
			}
		}

		/// <summary>
		/// Gets the first day of a week, modified from http://stackoverflow.com/a/19901870
		/// </summary>
		/// <param name="date">The date that will be used (week number will be calculated from this).</param>
		/// <param name="cal">The calendar to use to help finding the day.</param>
		/// <param name="calendarWeekRule">The calendar week rule to use to help finding the day.</param>
		/// <param name="firstDayOfWeek">The first day of the week to use to help finding the day.</param>
		/// <returns>The first day of the given week for the given year.</returns>
		static DateTime FirstDayOfWeek(DateTime date, System.Globalization.Calendar cal, CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
		{
			int year = date.Year;
			int weekOfYear = cal.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);
			var jan1 = new DateTime(year, 1, 1);
			int daysOffset = (int)firstDayOfWeek - (int)jan1.DayOfWeek;
			var firstWeekDay = jan1.AddDays(daysOffset);
			int firstWeek = cal.GetWeekOfYear(jan1, calendarWeekRule, firstDayOfWeek);
			if (firstWeek <= 1 || firstWeek > 50)
				--weekOfYear;
			return firstWeekDay.AddDays(weekOfYear * 7);
		}

		/// <summary>
		/// Force an update on the RadGrid's header text for the measures, as it loses the header text on postback.
		/// </summary>
		void updateHeaders()
		{
			foreach (var measure in this.rgData.MasterTableView.Columns.OfType<GridBoundColumn>().Where(c => !string.IsNullOrWhiteSpace(c.UniqueName) && c.UniqueName.StartsWith("measure_")))
				measure.HeaderText = this.measureHeaders[measure.UniqueName];
		}

		/// <summary>
		/// Called whenever the day in the date picker is changed, so it'll always select the last day of the week and update the data accordingly.
		/// </summary>
		protected void rdpEndOfWeek_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
		{
			var cal = this.rdpEndOfWeek.Calendar;
			var startOfWeek = FirstDayOfWeek(e.NewDate.Value, cal.Calendar, cal.DateTimeFormat.CalendarWeekRule, (DayOfWeek)cal.FirstDayOfWeek);
			this.rdpEndOfWeek.SelectedDate = startOfWeek.AddDays(6);
			var startOfNextWeek = startOfWeek.AddDays(7);
			var measure_columns = from c in this.rgData.MasterTableView.Columns.OfType<GridBoundColumn>()
								  where !string.IsNullOrWhiteSpace(c.UniqueName) && c.UniqueName.StartsWith("measure_")
								  select new { c.UniqueName, DataType = c.UniqueName[8].ToString(), MeasureID = decimal.Parse(c.UniqueName.Substring(10)) };
			var measure_totals = measure_columns.ToDictionary(m => m.UniqueName, m => 0m);
			foreach (var row in this.rgData.MasterTableView.GetItems(GridItemType.Item, GridItemType.AlternatingItem).Cast<GridDataItem>())
			{
				// For each row, we get its plant ID and narrow down the data by the plant ID and date range.
				decimal plantID = (decimal)this.rgData.MasterTableView.DataKeyValues[row.ItemIndex]["PlantID"];
				// Skip over plant ID -1, this is a dummy entry created for the totals.
				if (plantID == -1m)
					continue;

				var valueData = from d in this.entities.EHS_DATA
								where d.PLANT_ID == plantID && EntityFunctions.TruncateTime(d.DATE) >= startOfWeek && EntityFunctions.TruncateTime(d.DATE) < startOfNextWeek &&
								d.EHS_MEASURE.FREQUENCY != "M" && (d.EHS_MEASURE.DATA_TYPE == "V" || d.EHS_MEASURE.DATA_TYPE == "O")
								group d by d.MEASURE_ID into m
								select new { MeasureID = m.Key, DataTotal = m.Sum(d => d.VALUE) };
				var attributeData = from d in this.entities.EHS_DATA
									where d.PLANT_ID == plantID && EntityFunctions.TruncateTime(d.DATE) >= startOfWeek && EntityFunctions.TruncateTime(d.DATE) < startOfNextWeek &&
									d.EHS_MEASURE.FREQUENCY != "M" && d.EHS_MEASURE.DATA_TYPE == "A"
									//group d by d.MEASURE_ID into m
									//select new { MeasureID = m.Key, Attribute = m };
									select new { MeasureID = d.MEASURE_ID, Attribute = d.ATTRIBUTE };
				bool hadValueData = valueData.Any();
				bool hadAttributeData = attributeData.Any();
				foreach (var measure in measure_columns)
				{
					// Here we check if there was any data from above as well as data for this specific measure.
					if (hadValueData)
					{
						decimal value = 0;
						var measure_data = valueData.FirstOrDefault(d => d.MeasureID == measure.MeasureID);
						if (measure_data != null && measure_data.DataTotal.HasValue)
							value = measure_data.DataTotal.Value;
						row[measure.UniqueName].Text = value.ToString();
						// Add the value to the totals to update later.
						measure_totals[measure.UniqueName] += value;
					}
					// This is to handle attribute data, as it is a string instead of a value
					if (hadAttributeData)
					{
						var measure_data = attributeData.FirstOrDefault(d => d.MeasureID == measure.MeasureID);
						if (measure_data != null && measure_data.Attribute != null)
							row[measure.UniqueName].Text = measure_data.Attribute;
					}
					if (!hadValueData && !hadAttributeData)
						row[measure.UniqueName].Text = measure.DataType == "A" ? "" : "0";
				}
			}
			// Update the totals row.
			var totalRow = this.rgData.MasterTableView.Items.Cast<GridDataItem>().First(i => (decimal)this.rgData.MasterTableView.DataKeyValues[i.ItemIndex]["PlantID"] == -1m);
			totalRow.ControlStyle.Font.Bold = true;
			foreach (var measure in measure_columns)
			{
				if (measure.DataType == "A")
					totalRow[measure.UniqueName].Text = "";
				else
					totalRow[measure.UniqueName].Text = measure_totals[measure.UniqueName].ToString();
			}
			this.updateHeaders();
		}
	}
}
