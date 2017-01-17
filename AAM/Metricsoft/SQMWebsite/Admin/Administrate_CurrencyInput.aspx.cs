using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Telerik.Web.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.XSSF.UserModel;

namespace SQM.Website
{
    public partial class Administrate_CurrencyInput : SQMBasePage
    {

		DataSet dsCurrency;
		int periodYear;
		int periodMonth;

		protected void Page_Prerender(object sender, EventArgs e)
        {
			radPeriodSelect.MaxDate = GetMaxDate();

            if (IsPostBack)
            {
                ;
            }
            else
            {
                lblMessage.Visible = false;
                if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
                {   // page invoked from users inbox or calendar
                    string[] args = SessionManager.ReturnObject.ToString().Split('~');
                    if (args.Length > 1)
                    {
                        try
                        {
                            radPeriodSelect.SelectedDate = new DateTime(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), 1);
                            radPeriodSelect.DateInput.SelectedDate = new DateTime(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), 1);
                        }
                        catch { }
                    }
                    SessionManager.ClearReturns();
                }
                else
                {
                    radPeriodSelect.SelectedDate = DateTime.UtcNow.AddMonths(-1);
                    radPeriodSelect.DateInput.SelectedDate = DateTime.UtcNow.AddMonths(-1);
                }
            }
			SetupPage();
        }				

        private void SetupPage()
        {
			string baseCode = SessionManager.UserContext.WorkingLocation.BusinessOrg.PREFERRED_CURRENCY_CODE;

			DateTime selectedDate = radPeriodSelect.SelectedDate ?? DateTime.MinValue;
			if (selectedDate > DateTime.MinValue)
			{
				DateTime previousDate = selectedDate.AddMonths(-1);

				var entities = new PSsqmEntities();
				var currencyTypes = (from c in entities.CURRENCY
									 where c.CURRENCY_CODE != baseCode
									 select new
									 {
										 currencyCode = c.CURRENCY_CODE,
										 currencyName = c.CURRENCY_NAME,
										 baseCurrencyCode = baseCode,
										 currentValue = (from cv in entities.CURRENCY_XREF
														 where
															 cv.EFF_YEAR == selectedDate.Year &&
															 cv.EFF_MONTH == selectedDate.Month &&
															 cv.CURRENCY_CODE == c.CURRENCY_CODE
														 select cv.BASE_CURRENCY_RATE).FirstOrDefault(),
										 previousValue = (from pv in entities.CURRENCY_XREF
														  where
															  pv.EFF_YEAR == previousDate.Year &&
															  pv.EFF_MONTH == previousDate.Month &&
															  pv.CURRENCY_CODE == c.CURRENCY_CODE
														  select pv.BASE_CURRENCY_RATE).FirstOrDefault()
									 }).ToList();


				ltrPrevMonth.Text = previousDate.ToString("MMMM yyyy");
				ltrCurrentMonth.Text = selectedDate.ToString("MMMM yyyy");
				
				rptCurrency.DataSource = currencyTypes.OrderBy(c=> c.currencyName).ToList();
				rptCurrency.DataBind();
			}
        }

		protected void rbSave_Click(object sender, EventArgs e)
		{
			int currentMonth = ((DateTime)radPeriodSelect.SelectedDate).Month;
			int currentYear = ((DateTime)radPeriodSelect.SelectedDate).Year;

			foreach (RepeaterItem ri in rptCurrency.Items)
			{
				HiddenField hf = (HiddenField)ri.FindControl("hfCurrencyCode");
				RadNumericTextBox tb = (RadNumericTextBox)ri.FindControl("tbRate");

				if (hf != null && tb != null)
				{
					string currencyCode = hf.Value;
					//decimal rate = 0;
					//Decimal.TryParse(tb.Text, out rate);
					double rateVal = 0;
					Double.TryParse(tb.Text, out rateVal);

					if (rateVal > 0)
					{
						//rate = (decimal)rateVal;
						var entities = new PSsqmEntities();
						var queryObject = (from cx in entities.CURRENCY_XREF where cx.EFF_YEAR == currentYear &&
											cx.EFF_MONTH == currentMonth &&
											cx.CURRENCY_CODE == currencyCode
											select cx).FirstOrDefault();

						if (queryObject == null)
						{
							CURRENCY_XREF cur = new CURRENCY_XREF()
							{
								EFF_YEAR = currentYear,
								EFF_MONTH = currentMonth,
								CURRENCY_CODE = currencyCode,
								BASE_CURRENCY_RATE = (decimal)rateVal
							};
							entities.CURRENCY_XREF.AddObject(cur);
						}
						else
						{
							queryObject.BASE_CURRENCY_RATE = (decimal)rateVal;
						}
						entities.SaveChanges();
					}
				}
			}
			lblMessage.Visible = true;
			lblMessage.Text = "<div style=\"padding: 12px 0;\">Changes have been saved.</div>";
		}

		private DateTime GetMaxDate()
		{
			//var maxDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month + 1, 1);
            DateTime maxDate = DateTime.UtcNow.AddMonths(1);
			maxDate = maxDate.AddDays(-1); // Last day of current month
			return maxDate;
		}

		protected string DisplayExchangeRate(decimal exchangeRate)
		{
			return (exchangeRate == 0) ? "n/a" : exchangeRate.ToString();
		}


		protected void radPeriodSelect_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
		{
			lblMessage.Visible = false;
		}

		

	}
}