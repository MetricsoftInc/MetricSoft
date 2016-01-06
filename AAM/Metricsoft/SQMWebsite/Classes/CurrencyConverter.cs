using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public static class CurrencyConverter
	{

        public static CURRENCY_XREF InsertRate(string currencyCode, int year, int month, double rate)
        {
            CURRENCY_XREF xref;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    xref = (from x in entities.CURRENCY_XREF
                            where (x.EFF_YEAR == year && x.EFF_MONTH == month && x.CURRENCY_CODE == currencyCode)
                            select x).Single();
                }
                catch
                {
                    xref = new CURRENCY_XREF();
                    xref.EFF_YEAR = year;
                    xref.EFF_MONTH = month;
                    entities.AddToCURRENCY_XREF(xref);
                }

                xref.CURRENCY_CODE = currencyCode;
                xref.BASE_CURRENCY_RATE = (decimal)rate;

                entities.SaveChanges();
            }

            return xref;
        }


        public static CURRENCY_XREF CurrentRate(string currencyCode, int effYear, int effMonth)
        {
            CURRENCY_XREF currentRate = new CURRENCY_XREF();

            try
            {
               using (PSsqmEntities entities = new PSsqmEntities())
               {
                   if (effYear > 0 && effMonth > 0)
                   {
                       if (string.IsNullOrEmpty(currencyCode))
                       {
                           currentRate = (from r in entities.CURRENCY_XREF
                                          where (r.EFF_YEAR == effYear && r.EFF_MONTH == effMonth)
                                          select r).FirstOrDefault();
                       }
                       else
                       {
                           currentRate = (from r in entities.CURRENCY_XREF
                                          where (r.CURRENCY_CODE == currencyCode && r.EFF_YEAR == effYear && r.EFF_MONTH == effMonth)
                                          select r).FirstOrDefault();
                       }
                   }
                   else
                   {
                       currentRate = (from r in entities.CURRENCY_XREF
                                      select r).OrderByDescending(r => r.EFF_YEAR).ThenByDescending(r => r.EFF_MONTH).Where(l => l.CURRENCY_CODE == currencyCode).First();
                   }
               }

            }
            catch (Exception e)
            {
               // SQMLogger.LogException(e);
            }

            return currentRate;
        }

		/// <summary>
		/// Convert currency using most current rate available.
		/// </summary>
		public static double Convert(string fromCurrencyCode, string toCurrencyCode, string baseCurrencyCode, double fromValue)
		{
			int year = DateTime.UtcNow.Year;
			int month = DateTime.UtcNow.Month;
			return Convert(fromCurrencyCode, toCurrencyCode, baseCurrencyCode, fromValue, year, month);
		}

		/// <summary>
		/// Convert currency using conversion rate from specified year/month.
		/// </summary>
		public static double Convert(string fromCurrencyCode, string toCurrencyCode, string baseCurrencyCode, double fromValue, int year, int month)
		{
			fromCurrencyCode = fromCurrencyCode.ToUpper().Trim();
			toCurrencyCode = toCurrencyCode.ToUpper().Trim();
			baseCurrencyCode = baseCurrencyCode.ToUpper().Trim();

			if (fromCurrencyCode == toCurrencyCode)
				return fromValue;

			double fromBaseRate = 0;
			double returnValue = 0;
			string searchCode = "";
			
			try
			{
				SQM.Website.PSsqmEntities entities = new PSsqmEntities();

				if (fromCurrencyCode == baseCurrencyCode)
					searchCode = toCurrencyCode; // return reciprocal of result later
				else
					searchCode = fromCurrencyCode;

				int i = 0;
				while (fromBaseRate == 0 && i < 12) // Stop searching if nothing found within 12 months
				{
					fromBaseRate = (double)(from cur in entities.CURRENCY_XREF
											where cur.CURRENCY_CODE == searchCode &&
											cur.EFF_YEAR == year &&
											cur.EFF_MONTH == month
											select cur.BASE_CURRENCY_RATE).FirstOrDefault();
					if (fromBaseRate == 0)
					{
						if (month == 1)
						{
							year--;
							month = 12;
						}
						else
						{
							month--;
						}
						i++;
					}
				}

				if (fromBaseRate == 0)
					return 0;

				if (fromCurrencyCode == baseCurrencyCode)
				{
					returnValue = fromValue / fromBaseRate;
				}
				else if (toCurrencyCode == baseCurrencyCode)
				{
					returnValue = fromValue * fromBaseRate;
				}
				else
				{
					string toCode = toCurrencyCode.ToString();
					double toBaseRate = (double)(from cur in entities.CURRENCY_XREF
												where cur.CURRENCY_CODE == toCode &&
												cur.EFF_YEAR == year &&
												cur.EFF_MONTH == month
												select cur.BASE_CURRENCY_RATE).FirstOrDefault();

					if (toBaseRate == 0)
						return 0;

					returnValue = fromValue * (fromBaseRate / toBaseRate);
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}
			return returnValue;
		}
	}
}