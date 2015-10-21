using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.Dynamic;
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
						bool measureIsValue = measure.DATA_TYPE == "V" || measure.DATA_TYPE == "O";
						string value = "";
						decimal dataID = -1;
						if (dayData.Any())
						{
							var data = dayData.FirstOrDefault(d => d.MEASURE_ID == measure.MEASURE_ID);
							if (data != null)
							{
								dataID = data.DATA_ID;
								if (measureIsValue)
									value = data.VALUE.ToString();
								else if (measure.DATA_TYPE == "A")
									value = data.ATTRIBUTE;
							}
						}
						string type = measureIsValue ? "Integer" : "String";
						string toolTip = "Value must be a " + (measureIsValue ? "number" : "string") + ".";
						dynamic dataToAdd = new ExpandoObject();
						dataToAdd.value = value;
						dataToAdd.validatorType = type;
						dataToAdd.validatorToolTip = toolTip;
						if (measure.DATA_TYPE == "O")
							dataToAdd.ordinal = GetOrdinalData(entities, dataID);
						allData.Add(dayName + "|" + measure.MEASURE_ID, ((ExpandoObject)dataToAdd).ToDictionary(x => x.Key, x => x.Value));
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

		static void AddOrdinalData(PSsqmEntities entities, EHS_DATA ehs_data, dynamic types, Dictionary<string, dynamic> type_data)
		{
			foreach (var t in types)
			{
				var value = type_data[t.DESCRIPTION.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")];
				if (value.ContainsKey("value"))
					entities.EHS_DATA_ORD.AddObject(new EHS_DATA_ORD()
					{
						EHS_DATA = ehs_data,
						XLAT_GROUP = t.XLAT_GROUP,
						XLAT_CODE = t.XLAT_CODE,
						VALUE = value["value"]
					});
			}
		}

		static void AddOrdinalData(PSsqmEntities entities, EHS_DATA ehs_data, Dictionary<string, dynamic> data)
		{
			// Get the types first.
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			AddOrdinalData(entities, ehs_data, types, data["type"]);
			AddOrdinalData(entities, ehs_data, bodyParts, data["bodyPart"]);
			AddOrdinalData(entities, ehs_data, rootCauses, data["rootCause"]);
			AddOrdinalData(entities, ehs_data, tenures, data["tenure"]);
			AddOrdinalData(entities, ehs_data, daysToCloses, data["daysToClose"]);
		}

		static void UpdateOrdinalData(PSsqmEntities entities, decimal dataID, dynamic types, Dictionary<string, dynamic> type_data)
		{
			foreach (var t in types)
			{
				var group = t.XLAT_GROUP as string;
				var code = t.XLAT_CODE as string;
				var data = entities.EHS_DATA_ORD.FirstOrDefault(d => d.DATA_ID == dataID && d.XLAT_GROUP == group && d.XLAT_CODE == code);
				var value = type_data[t.DESCRIPTION.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")];
				if (value.ContainsKey("value"))
				{
					if (data == null)
						entities.EHS_DATA_ORD.AddObject(new EHS_DATA_ORD()
						{
							DATA_ID = dataID,
							XLAT_GROUP = t.XLAT_GROUP,
							XLAT_CODE = t.XLAT_CODE,
							VALUE = value["value"]
						});
					else
						data.VALUE = value["value"];
				}
				else if (data != null)
					entities.DeleteObject(data);
			}
		}

		static void UpdateOrdinalData(PSsqmEntities entities, decimal dataID, Dictionary<string, dynamic> data)
		{
			// Get the types first.
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			UpdateOrdinalData(entities, dataID, types, data["type"]);
			UpdateOrdinalData(entities, dataID, bodyParts, data["bodyPart"]);
			UpdateOrdinalData(entities, dataID, rootCauses, data["rootCause"]);
			UpdateOrdinalData(entities, dataID, tenures, data["tenure"]);
			UpdateOrdinalData(entities, dataID, daysToCloses, data["daysToClose"]);
		}

		[WebMethod]
		public static void SaveDailyData(decimal plantID, DateTime day, Dictionary<string, Dictionary<string, dynamic>> allData)
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
						bool measureIsValue = measure.DATA_TYPE == "V" || measure.DATA_TYPE == "O";
						var measure_data = allData[dayName + "|" + measure.MEASURE_ID];
						string text = measure_data["value"];
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
									if (measureIsValue)
										data.VALUE = decimal.Parse(text);
									else if (measure.DATA_TYPE == "A")
										data.ATTRIBUTE = text;
									if (measure_data.ContainsKey("ordinal"))
										UpdateOrdinalData(entities, data.DATA_ID, measure_data["ordinal"]);
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
							if (measureIsValue)
								newData.VALUE = decimal.Parse(text);
							else if (measure.DATA_TYPE == "A")
								newData.ATTRIBUTE = text;
							entities.EHS_DATA.AddObject(newData);
							if (measure_data.ContainsKey("ordinal"))
								AddOrdinalData(entities, newData, measure_data["ordinal"]);
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

		/// <summary>
		/// Gets the ordinal data for the types requested for the given data ID.
		/// </summary>
		/// <param name="entities">The entities to get the data from.</param>
		/// <param name="dataID">The data ID to pull data for, -1 if we just want the descriptions with no data.</param>
		/// <param name="types">The types returned from GetOrdinalTypes.</param>
		/// <returns>A dictionary of the type descriptions as the key and the value (if any) as the value.</returns>
		static Dictionary<string, dynamic> GetOrdinalData(PSsqmEntities entities, decimal dataID, dynamic types)
		{
			var ret = new Dictionary<string, dynamic>();
			foreach (var t in types)
			{
				var group = t.XLAT_GROUP as string;
				var code = t.XLAT_CODE as string;
				var data = dataID == -1 ? null : entities.EHS_DATA_ORD.FirstOrDefault(d => d.DATA_ID == dataID && d.XLAT_GROUP == group && d.XLAT_CODE == code);
				var key = t.DESCRIPTION as string;
				if (data != null)
					ret.Add(key, new { value = data.VALUE });
				else
					ret.Add(key, new { });
			}
			return ret;
		}

		static dynamic GetOrdinalData(PSsqmEntities entities, decimal dataID)
		{
			// Get the types first.
			var types = GetOrdinalTypes(entities, "INJURY_TYPE");
			var bodyParts = GetOrdinalTypes(entities, "INJURY_PART");
			var rootCauses = GetOrdinalTypes(entities, "INJURY_CAUSE");
			var tenures = GetOrdinalTypes(entities, "INJURY_TENURE");
			var daysToCloses = GetOrdinalTypes(entities, "INJURY_DAYS_TO_CLOSE");

			return new
			{
				type = GetOrdinalData(entities, dataID, types),
				bodyPart = GetOrdinalData(entities, dataID, bodyParts),
				rootCause = GetOrdinalData(entities, dataID, rootCauses),
				tenure = GetOrdinalData(entities, dataID, tenures),
				daysToClose = GetOrdinalData(entities, dataID, daysToCloses)
			};
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
			// Place the grid just before the <br> literal we have set aside for it.
			this.dataPanel.Controls.AddAt(this.dataPanel.Controls.IndexOf(this.rgData_placeholder), rgData);
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
									select new { m.MEASURE_NAME, m.MEASURE_ID, m.DATA_TYPE };
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
						btn.Attributes.Add("class", "myButtonDisabled");
					}
					else
						btn.Attributes.Add("class", "myButton");
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
				dynamic dataItem = (container.Parent as GridDataItem).DataItem;
				if (dataItem.DATA_TYPE == "O")
				{
					var rtbTotal = new RadTextBox()
					{
						ID = "rtb" + this.Suffix,
						Skin = "Metro",
						Width = new Unit("35px")
					};
					rtbTotal.Style.Add("vertical-align", "middle");
					rtbTotal.Style.Add("margin-right", "5px");
					container.Controls.Add(rtbTotal);
					var btnDetails = new HtmlInputButton("button")
					{
						ID = "btnDetails" + this.Suffix,
						Value = "Details"
					};
					btnDetails.Attributes.Add("onclick", "rwDetails_open('" + this.Suffix + "', this)");
					btnDetails.Attributes.Add("class", "myButtonSmall");
					container.Controls.Add(btnDetails);
					var hfDetails = new HiddenField()
					{
						ID = "hfDetails" + this.Suffix
					};
					container.Controls.Add(hfDetails);
				}
				else
				{
					var rtbData = new RadTextBox()
					{
						ID = "rtb" + this.Suffix,
						Skin = "Metro",
						Width = dataItem.DATA_TYPE == "V" ? 100 : (dataItem.DATA_TYPE == "A" ? 1200 : this.Width)
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
}
