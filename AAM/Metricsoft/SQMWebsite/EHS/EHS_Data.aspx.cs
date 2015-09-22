using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace SQM.Website.EHS
{
	public partial class EHS_Data : Page
	{
		// These are assigned on the page's load, so as long as the session is still current, these should work in the Page Methods below.
		static System.Globalization.Calendar calendar = null;
		static CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstDay;
		static DayOfWeek firstDayOfWeek = DayOfWeek.Monday;

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

		[WebMethod]
		public static dynamic GetDailyData(decimal plantID, DateTime day)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "D"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfWeek = FirstDayOfWeek(day, calendar, calendarWeekRule, firstDayOfWeek);
				var endOfWeek = startOfWeek.AddDays(6);
				var dateHeaders = new Dictionary<string, string>();
				var allData = new Dictionary<string, dynamic>();
				for (int i = 0; i < 7; ++i, startOfWeek = startOfWeek.AddDays(1))
				{
					string dayName = startOfWeek.ToString("ddd");
					dateHeaders.Add(dayName, startOfWeek.ToString("d") + "<br>" + dayName);
					var dayData = entities.EHS_DATA.Where(d => EntityFunctions.TruncateTime(d.DATE) == startOfWeek.Date && d.PLANT_ID == plantID);
					foreach (var measure in measures)
					{
						string value = "";
						if (dayData.Any())
						{
							var data = dayData.FirstOrDefault(d => d.MEASURE_ID == measure.MEASURE_ID);
							if (data != null)
							{
								if (measure.DATA_TYPE == "V")
									value = data.VALUE.ToString();
								else if (measure.DATA_TYPE == "A")
									value = data.ATTRIBUTE;
							}
						}
						string type = measure.DATA_TYPE == "V" ? "Integer" : "String";
						string toolTip = "Value must be a " + (measure.DATA_TYPE == "V" ? "number" : "string") + ".";
						allData.Add(dayName + "|" + measure.MEASURE_ID, new { value, validatorType = type, validatorToolTip = toolTip });
					}
				}
				return new
				{
					plantID,
					endOfWeek = endOfWeek.ToString("O"),
					dates = dateHeaders,
					allData
				};
			}
		}

		[WebMethod]
		public static dynamic GetWeeklyData(decimal plantID, DateTime day)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "W"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfWeek = FirstDayOfWeek(day, calendar, calendarWeekRule, firstDayOfWeek);
				var endOfWeek = startOfWeek.AddDays(6);
				var allData = new Dictionary<string, dynamic>();
				foreach (var measure in measures)
				{
					string value = "";
					var data = entities.EHS_DATA.FirstOrDefault(d => EntityFunctions.TruncateTime(d.DATE) == endOfWeek.Date && d.PLANT_ID == plantID && d.MEASURE_ID == measure.MEASURE_ID);
					if (data != null)
					{
						if (measure.DATA_TYPE == "V")
							value = data.VALUE.ToString();
						else if (measure.DATA_TYPE == "A")
							value = data.ATTRIBUTE;
					}
					string type = measure.DATA_TYPE == "V" ? "Integer" : "String";
					string toolTip = "Value must be a " + (measure.DATA_TYPE == "V" ? "number" : "string") + ".";
					allData.Add(measure.MEASURE_ID.ToString(), new { value, validatorType = type, validatorToolTip = toolTip });
				}
				return new
				{
					plantID,
					endOfWeek = endOfWeek.ToString("O"),
					date = "End of Week " + endOfWeek.ToString("d"),
					allData
				};
			}
		}

		[WebMethod]
		public static dynamic GetMonthlyData(decimal plantID, DateTime day)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "M"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfMonth = new DateTime(day.Year, day.Month, 1);
				var allData = new Dictionary<string, dynamic>();
				foreach (var measure in measures)
				{
					string value = "";
					var data = entities.EHS_DATA.FirstOrDefault(d => EntityFunctions.TruncateTime(d.DATE) == startOfMonth.Date && d.PLANT_ID == plantID && d.MEASURE_ID == measure.MEASURE_ID);
					if (data != null)
					{
						if (measure.DATA_TYPE == "V")
							value = data.VALUE.ToString();
						else if (measure.DATA_TYPE == "A")
							value = data.ATTRIBUTE;
					}
					string type = measure.DATA_TYPE == "V" ? "Integer" : "String";
					string toolTip = "Value must be a " + (measure.DATA_TYPE == "V" ? "number" : "string") + ".";
					allData.Add(measure.MEASURE_ID.ToString(), new { value, validatorType = type, validatorToolTip = toolTip });
				}
				return new
				{
					plantID,
					startOfMonth = startOfMonth.ToString("O"),
					date = "Month of " + startOfMonth.ToString("Y"),
					allData
				};
			}
		}

		[WebMethod]
		public static void SaveDailyData(decimal plantID, DateTime day, Dictionary<string, string> allData)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "D"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfWeek = FirstDayOfWeek(day, calendar, calendarWeekRule, firstDayOfWeek);
				for (int i = 0; i < 7; ++i, startOfWeek = startOfWeek.AddDays(1))
				{
					string dayName = startOfWeek.ToString("ddd");
					var dayData = entities.EHS_DATA.Where(d => EntityFunctions.TruncateTime(d.DATE) == startOfWeek.Date && d.PLANT_ID == plantID);
					foreach (var measure in measures)
					{
						string text = allData[dayName + "|" + measure.MEASURE_ID];
						bool hasText = !string.IsNullOrWhiteSpace(text);
						// We determine if we need to add a new entry into the database by looking for if there is any data.
						bool addNew = true;
						if (dayData.Any())
						{
							var data = dayData.FirstOrDefault(d => d.MEASURE_ID == measure.MEASURE_ID);
							if (data != null)
							{
								addNew = false;
								// If we had some text in the RadTextBox, then we'll update the entry, otherwise we'll delete it.
								if (hasText)
								{
									if (measure.DATA_TYPE == "V")
										data.VALUE = decimal.Parse(text);
									else if (measure.DATA_TYPE == "A")
										data.ATTRIBUTE = text;
								}
								else
									entities.DeleteObject(data);
							}
						}
						// This will only add a new entry if there was no entry found already and we had some text in the RadTextBox.
						if (addNew && hasText)
						{
							var newData = new EHS_DATA()
							{
								MEASURE_ID = measure.MEASURE_ID,
								PLANT_ID = plantID,
								DATE = startOfWeek
							};
							if (measure.DATA_TYPE == "V")
								newData.VALUE = decimal.Parse(text);
							else if (measure.DATA_TYPE == "A")
								newData.ATTRIBUTE = text;
							entities.EHS_DATA.AddObject(newData);
						}
					}
				}
				// Save the changes we made to the database.
				entities.SaveChanges();
			}
		}

		[WebMethod]
		public static void SaveWeeklyData(decimal plantID, DateTime day, Dictionary<string, string> allData)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "W"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfWeek = FirstDayOfWeek(day, calendar, calendarWeekRule, firstDayOfWeek);
				var endOfWeek = startOfWeek.AddDays(6);
				foreach (var measure in measures)
				{
					string text = allData[measure.MEASURE_ID.ToString()];
					bool hasText = !string.IsNullOrWhiteSpace(text);
					// We determine if we need to add a new entry into the database by looking for if there is any data.
					bool addNew = true;
					var data = entities.EHS_DATA.FirstOrDefault(d => EntityFunctions.TruncateTime(d.DATE) == endOfWeek.Date && d.PLANT_ID == plantID && d.MEASURE_ID == measure.MEASURE_ID);
					if (data != null)
					{
						addNew = false;
						// If we had some text in the RadTextBox, then we'll update the entry, otherwise we'll delete it.
						if (hasText)
						{
							if (measure.DATA_TYPE == "V")
								data.VALUE = decimal.Parse(text);
							else if (measure.DATA_TYPE == "A")
								data.ATTRIBUTE = text;
						}
						else
							entities.DeleteObject(data);
					}
					// This will only add a new entry if there was no entry found already and we had some text in the RadTextBox.
					if (addNew && hasText)
					{
						var newData = new EHS_DATA()
						{
							MEASURE_ID = measure.MEASURE_ID,
							PLANT_ID = plantID,
							DATE = endOfWeek
						};
						if (measure.DATA_TYPE == "V")
							newData.VALUE = decimal.Parse(text);
						else if (measure.DATA_TYPE == "A")
							newData.ATTRIBUTE = text;
						entities.EHS_DATA.AddObject(newData);
					}
				}
				// Save the changes we made to the database.
				entities.SaveChanges();
			}
		}

		[WebMethod]
		public static void SaveMonthlyData(decimal plantID, DateTime day, Dictionary<string, string> allData)
		{
			using (var entities = new PSsqmEntities())
			{
				var measures = from m in entities.EHS_MEASURE
							   where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == "M"
							   select new { m.MEASURE_ID, m.DATA_TYPE };
				var startOfMonth = new DateTime(day.Year, day.Month, 1);
				foreach (var measure in measures)
				{
					string text = allData[measure.MEASURE_ID.ToString()];
					bool hasText = !string.IsNullOrWhiteSpace(text);
					// We determine if we need to add a new entry into the database by looking for if there is any data.
					bool addNew = true;
					var data = entities.EHS_DATA.FirstOrDefault(d => EntityFunctions.TruncateTime(d.DATE) == startOfMonth.Date && d.PLANT_ID == plantID && d.MEASURE_ID == measure.MEASURE_ID);
					if (data != null)
					{
						addNew = false;
						// If we had some text in the RadTextBox, then we'll update the entry, otherwise we'll delete it.
						if (hasText)
						{
							if (measure.DATA_TYPE == "V")
								data.VALUE = decimal.Parse(text);
							else if (measure.DATA_TYPE == "A")
								data.ATTRIBUTE = text;
						}
						else
							entities.DeleteObject(data);
					}
					// This will only add a new entry if there was no entry found already and we had some text in the RadTextBox.
					if (addNew && hasText)
					{
						var newData = new EHS_DATA()
						{
							MEASURE_ID = measure.MEASURE_ID,
							PLANT_ID = plantID,
							DATE = startOfMonth
						};
						if (measure.DATA_TYPE == "V")
							newData.VALUE = decimal.Parse(text);
						else if (measure.DATA_TYPE == "A")
							newData.ATTRIBUTE = text;
						entities.EHS_DATA.AddObject(newData);
					}
				}
				// Save the changes we made to the database.
				entities.SaveChanges();
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			var cal = this.rdpEndOfWeek.Calendar;
			calendar = cal.Calendar;
			calendarWeekRule = cal.DateTimeFormat.CalendarWeekRule;
			firstDayOfWeek = (DayOfWeek)cal.FirstDayOfWeek;

			// The RadGrid is created here, due to the number of columns varying depending on the frequency being used. I was unable to remove columns from an existing
			// RadGrid in Page_Init or Page_Load as Telerik's code would fail when doing that.
			var rgData = new RadGrid()
			{
				ID = "rgData",
				AutoGenerateColumns = false,
				BorderStyle = BorderStyle.None
			};
			var measureNameColumn = new GridBoundColumn()
			{
				DataField = "MEASURE_NAME",
			};
			measureNameColumn.ItemStyle.CssClass = "tanCell";
			measureNameColumn.ItemStyle.Width = new Unit("30%");
			measureNameColumn.ItemStyle.Font.Bold = true;
			rgData.MasterTableView.Columns.Add(measureNameColumn);
			var measureIDColumn = new GridBoundColumn()
			{
				DataField = "MEASURE_ID",
				UniqueName = "MeasureID"
			};
			measureIDColumn.HeaderStyle.CssClass = measureIDColumn.ItemStyle.CssClass = "displayNone";
			rgData.MasterTableView.Columns.Add(measureIDColumn);
			if (this.Request.QueryString["type"] == "Weekly" || this.Request.QueryString["type"] == "Monthly")
				rgData.MasterTableView.Columns.Add(new GridTemplateColumn()
				{
					UniqueName = "gtcFull",
					ItemTemplate = new DataTemplate("Full", 1200)
				});
			else
			{
				var startOfWeek = FirstDayOfWeek(DateTime.Today, calendar, calendarWeekRule, firstDayOfWeek);
				for (int i = 0; i < 7; ++i, startOfWeek = startOfWeek.AddDays(1))
					rgData.MasterTableView.Columns.Add(new GridTemplateColumn()
					{
						UniqueName = "gtc" + startOfWeek.ToString("ddd"),
						ItemTemplate = new DataTemplate(startOfWeek.ToString("ddd"), 100)
					});
			}
			foreach (var column in rgData.MasterTableView.Columns.OfType<GridTemplateColumn>())
			{
				column.HeaderStyle.HorizontalAlign = column.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
				column.HeaderStyle.CssClass = "dataHeader";
				column.HeaderStyle.Width = column.ItemStyle.Width = new Unit(this.Request.QueryString["type"] == "Weekly" || this.Request.QueryString["type"] == "Monthly" ? "70%" : "10%");
				column.ItemStyle.CssClass = "greyCell";
			}
			// The minus 1 here is because the control before the save button is a <br> literal, we want the grid to be before that.
			this.dataPanel.Controls.AddAt(this.dataPanel.Controls.IndexOf(this.btnSave) - 1, rgData);
		}

		// Stores a reference to the entities so it can persist for the entire page's life cycle.
		PSsqmEntities entities;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				string frequency;
				string frequencyName = this.Request.QueryString["type"];
				if (frequencyName == null)
					frequencyName = "Daily";
				if (frequencyName == "Weekly")
					frequency = "W";
				else if (frequencyName == "Monthly")
					frequency = "M";
				else
					frequency = "D";

				if (frequency != "M")
					this.spanMonth.Visible = false;
				else
					this.spanEndOfWeek.Visible = false;

				this.entities = new PSsqmEntities();

				var rgData = this.dataPanel.FindControl("rgData") as RadGrid;
				rgData.DataSource = from m in this.entities.EHS_MEASURE
									where m.MEASURE_CATEGORY == "SAFE" && m.MEASURE_SUBCATEGORY == "SAFE1" && m.STATUS == "A" && m.FREQUENCY == frequency
									orderby m.MEASURE_CD
									select new { m.MEASURE_NAME, m.MEASURE_ID };
				rgData.DataBind();

				SQMBasePage.SetLocationList(this.rcbPlant,
					UserContext.FilterPlantAccessList(SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true)),
					SessionManager.UserContext.HRLocation.Plant.PLANT_ID);

				this.rdpEndOfWeek.SelectedDate = this.rmypMonth.SelectedDate = DateTime.Today;

				this.rcbPlant.OnClientSelectedIndexChanged = this.rdpEndOfWeek.DateInput.OnClientDateChanged = this.rmypMonth.DateInput.OnClientDateChanged = "get" + frequencyName + "Data";
				this.btnSave.Attributes.Add("onclick", "save" + frequencyName + "Data()");

				foreach (var freq in new[] { "Daily", "Weekly", "Monthly" })
				{
					var btn = this.dataPanel.FindControl("btn" + freq) as HtmlInputButton;
					if (frequencyName == freq)
					{
						btn.Disabled = true;
						btn.Attributes.Add("class", "frequencyButtonDisabled");
					}
					else
						btn.Attributes.Add("class", "frequencyButton");
				}
			}
		}

		/// <summary>
		/// A template class that is used to create the text box and validator for the columns used in the RadGrid.
		/// </summary>
		class DataTemplate : ITemplate
		{
			string Suffix { get; set; }
			Unit Width { get; set; }

			public DataTemplate(string suffix, Unit width)
			{
				this.Suffix = suffix;
				this.Width = width;
			}

			public void InstantiateIn(Control container)
			{
				var rtbData = new RadTextBox()
				{
					ID = "rtb" + this.Suffix,
					Skin = "Metro",
					Width = this.Width
				};
				container.Controls.Add(rtbData);
				container.Controls.Add(new CompareValidator()
				{
					ID = "cmp" + this.Suffix,
					ControlToValidate = rtbData.ID,
					Display = ValidatorDisplay.Dynamic,
					Text = "*",
					ForeColor = Color.Red,
					Operator = ValidationCompareOperator.DataTypeCheck,
					EnableClientScript = true
				});
			}
		}
	}
}
