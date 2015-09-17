using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace SQM.Website.EHS
{
	public partial class EHS_Data : Page
	{
		// Stores a reference to the entities so it can persist for the entire page's life cycle.
		PSsqmEntities entities;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.entities = new PSsqmEntities();

			if (!this.IsPostBack)
			{
				this.rgData.DataSource = from m in this.entities.EHS_MEASURE
										 where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "D"
										 orderby m.MEASURE_CD
										 select new { m.MEASURE_NAME, m.MEASURE_ID, m.DATA_TYPE };
				this.rgData.DataBind();

				SQMBasePage.SetLocationList(this.rcbPlant,
					UserContext.FilterPlantAccessList(SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true)),
					SessionManager.UserContext.HRLocation.Plant.PLANT_ID);

				this.rdpEndOfWeek.SelectedDate = DateTime.Today;
				this.rdpEndOfWeek_SelectedDateChanged(this.rdpEndOfWeek, new SelectedDateChangedEventArgs(null, DateTime.Today));

				// This is to get around Telerik's RadGrid applying its own class to the rows, by having jQuery remove the classes when the page loads (even with an AJAX call).
				// Also used to fade out the saved label.
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "jQuery", @"function pageLoad(sender, args)
				{
					$('.rgHeader, .rgRow, .rgAltRow').removeClass('rgHeader rgRow rgAltRow');
					var lblSaved = $('#" + this.lblSaved.ClientID + @"');
					if (lblSaved.length > 0)
						window.setTimeout(function() { lblSaved.fadeOut(); }, 2000);
				}", true);
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
		/// Updates the headers of the RadGrid to have the day and day of week.
		/// </summary>
		/// <param name="daysOfWeek">The list of days for the week.</param>
		void updateHeaders(List<DateTime> daysOfWeek)
		{
			var header = this.rgData.MasterTableView.GetItems(GridItemType.Header)[0] as GridHeaderItem;
			for (int i = 0; i < 7; ++i)
			{
				string dayName = daysOfWeek[i].ToString("ddd");
				header["gtc" + dayName].Text = daysOfWeek[i].ToString("d") + "<br>" + dayName;
			}
		}

		/// <summary>
		/// Updates the data based on the chosen plant and week.
		/// </summary>
		void updateData()
		{
			var rows = this.rgData.MasterTableView.GetItems(GridItemType.Item, GridItemType.AlternatingItem).Cast<GridDataItem>();
			// Retrieve the days of the week that we saved earlier.
			var daysOfWeek = this.hfDaysOfWeek.Value.Split(',').Select(day => DateTime.Parse(day)).ToList();
			decimal plantID = decimal.Parse(this.rcbPlant.SelectedValue);
			for (int i = 0; i < 7; ++i)
			{
				var dayOfWeek = daysOfWeek[i];
				string dayName = dayOfWeek.ToString("ddd");
				// Here we check if there was any data already in the database for the given days of the week.
				var dayData = this.entities.EHS_DATA.Where(d => EntityFunctions.TruncateTime(d.DATE) == dayOfWeek.Date && d.PLANT_ID == plantID);
				foreach (var row in rows)
				{
					var rtb = row["gtc" + dayName].FindControl("rtb" + dayName) as RadTextBox;
					rtb.Text = null;
					decimal measureID = (decimal)this.rgData.MasterTableView.DataKeyValues[row.ItemIndex]["MEASURE_ID"];
					if (dayData.Any())
					{
						var data = dayData.FirstOrDefault(d => d.MEASURE_ID == measureID);
						if (data != null)
							rtb.Text = data.VALUE.ToString();
					}
					string dataType = row["DataType"].Text;
					foreach (var cell in row.Cells.Cast<GridTableCell>().Where(c => c.Column is GridTemplateColumn))
					{
						var cmp = cell.Controls.OfType<CompareValidator>().First();
						cmp.Type = dataType == "V" ? ValidationDataType.Integer : ValidationDataType.String;
						cmp.ToolTip = "Value must be a " + (dataType == "V" ? "number" : "string") + ".";
					}
				}
			}
			this.updateHeaders(daysOfWeek);
		}

		/// <summary>
		/// Called whenever the day in the date picker is changed, so it'll always select the last day of the week and update the data accordingly.
		/// </summary>
		protected void rdpEndOfWeek_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
		{
			var cal = this.rdpEndOfWeek.Calendar;
			var startOfWeek = FirstDayOfWeek(e.NewDate.Value, cal.Calendar, cal.DateTimeFormat.CalendarWeekRule, (DayOfWeek)cal.FirstDayOfWeek);
			this.rdpEndOfWeek.SelectedDate = startOfWeek.AddDays(6);
			var daysOfWeek = new List<DateTime>();
			for (int i = 0; i < 7; ++i, startOfWeek = startOfWeek.AddDays(1))
				daysOfWeek.Add(startOfWeek);
			// The days of this week are stored in a hidden field to be retrieved later when saving.
			this.hfDaysOfWeek.Value = string.Join(",", daysOfWeek.Select(day => day.ToString("d")));
			this.updateData();
		}

		protected void rcbPlant_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			this.updateData();
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			// Retrieve the days of the week that we saved earlier.
			var daysOfWeek = this.hfDaysOfWeek.Value.Split(',').Select(day => DateTime.Parse(day)).ToList();
			// Only continue if the page's validators say the page is valid (this is just in case the client-side validators fail to register).
			if (this.Page.IsValid)
			{
				decimal plantID = decimal.Parse(this.rcbPlant.SelectedValue);
				var rows = this.rgData.MasterTableView.GetItems(GridItemType.Item, GridItemType.AlternatingItem).Cast<GridDataItem>();
				for (int i = 0; i < 7; ++i)
				{
					// The day of the week has to be stored before we use it in the LINQ query, otherwise LINQ to Entities throws an exception due to trying to pull in the entire daysOfWeek list.
					var dayOfWeek = daysOfWeek[i];
					string dayName = dayOfWeek.ToString("ddd");
					var dayData = this.entities.EHS_DATA.Where(d => EntityFunctions.TruncateTime(d.DATE) == dayOfWeek.Date && d.PLANT_ID == plantID);
					foreach (var row in rows)
					{
						decimal measureID = (decimal)this.rgData.MasterTableView.DataKeyValues[row.ItemIndex]["MEASURE_ID"];
						string dataType = row["DataType"].Text;
						string text = (row["gtc" + dayName].FindControl("rtb" + dayName) as RadTextBox).Text;
						bool hasText = !string.IsNullOrWhiteSpace(text);
						// We determine if we need to add a new entry into the database by looking for if there is any data.
						bool addNew = true;
						if (dayData.Any())
						{
							var data = dayData.FirstOrDefault(d => d.MEASURE_ID == measureID);
							if (data != null)
							{
								addNew = false;
								// If we had some text in the RadTextBox, then we'll update the entry, otherwise we'll delete it.
								if (hasText)
								{
									if (dataType == "V")
										data.VALUE = decimal.Parse(text);
									else if (dataType == "A")
										data.ATTRIBUTE = text;
								}
								else
									this.entities.DeleteObject(data);
							}
						}
						// This will only add a new entry if there was no entry found already and we had some text in the RadTextBox.
						if (addNew && hasText)
						{
							var newData = new EHS_DATA()
							{
								MEASURE_ID = measureID,
								PLANT_ID = plantID,
								DATE = dayOfWeek
							};
							if (dataType == "V")
								newData.VALUE = decimal.Parse(text);
							else if (dataType == "A")
								newData.ATTRIBUTE = text;
							this.entities.EHS_DATA.AddObject(newData);
						}
					}
				}
				// Save the changes we made to the database.
				this.entities.SaveChanges();
				// Show the label to say that the changes were saved.
				this.lblSaved.Visible = true;
			}
			this.updateHeaders(daysOfWeek);
		}
	}
}
