using System;
using System.Collections.Generic;
using System.Linq;


namespace SQM.Website
{
	public static class CurrencyMgr
	{
		static PSsqmEntities entities;

		public static void CheckForUpdates(string baseCurrencyCode)
		{
			entities = new PSsqmEntities();

			//var currencyCodes = (from c in entities.CURRENCY where c.CURRENCY_CODE != baseCurrencyCode select c.CURRENCY_CODE).ToList();
			var currencyCodes = (from c in entities.CURRENCY select c.CURRENCY_CODE).ToList();

			DateTime lastMonth = DateTime.UtcNow.AddMonths(-1);

			foreach (string quoteCurrencyCode in currencyCodes)
			{
				DateTime latestDate = new DateTime(2000, 1, 1);
				
				CURRENCY_XREF latestEntry = (from cx in entities.CURRENCY_XREF
											 where cx.CURRENCY_CODE == quoteCurrencyCode
											 orderby cx.EFF_YEAR descending, cx.EFF_MONTH descending
											 select cx).FirstOrDefault();

				if (latestEntry != null)
					latestDate = new DateTime(latestEntry.EFF_YEAR, latestEntry.EFF_MONTH, 1);

				if (latestDate < lastMonth)
					UpdateCurrency(baseCurrencyCode, quoteCurrencyCode, latestDate);
			}
			entities.SaveChanges();
		}


		static void UpdateCurrency(string baseCurrencyCode, string quoteCurrencyCode, DateTime startDate)
		{
			// Need a minimum span of months to force Oanda to display monthly data vs. weekly data
			DateTime oandaStartDate = startDate;
			if (DateTime.UtcNow.AddMonths(-12) < oandaStartDate)
				oandaStartDate = DateTime.UtcNow.AddMonths(-12);

			string url = BuildRequestUrl(baseCurrencyCode, quoteCurrencyCode, oandaStartDate);
			
			string csvString = "";
			try
			{
				var client = new System.Net.WebClient();
				csvString = client.DownloadString(url);
			}
			catch (Exception)
			{
			}
			
			string[] allRows = csvString.Split('\n');
			string[] dateRows = GetDateRows(allRows);

			foreach (string row in dateRows)
			{
				string[] lineData = LineSplitter(row).ToArray();

				if (lineData.Count() >= 1)
				{
					DateTime conversionDate = DateTime.Parse(lineData[0]);
					decimal conversionFactor = 1 / Convert.ToDecimal(lineData[1]);

					if (conversionDate != null && conversionFactor != null)
					{
						CURRENCY_XREF entry = (from cx in entities.CURRENCY_XREF
											   where cx.CURRENCY_CODE == quoteCurrencyCode &&
											   cx.EFF_YEAR == conversionDate.Year &&
											   cx.EFF_MONTH == conversionDate.Month
											   select cx).FirstOrDefault();

						if (entry == null)
						{
							entry = new CURRENCY_XREF()
							{
								EFF_YEAR = conversionDate.Year,
								EFF_MONTH = conversionDate.Month,
								CURRENCY_CODE = quoteCurrencyCode,
								BASE_CURRENCY_RATE = conversionFactor
							};
							entities.CURRENCY_XREF.AddObject(entry);
						}
					}
				}
			}
		}

		static string BuildRequestUrl(string baseCurrencyCode, string quoteCurrencyCode, DateTime startDate)
		{
			string url = "";

			// Flip base and quote currency codes because we are losing precision (e.g. KRW rounds to .0007)
			// Later, use the inverse value

			url = string.Format("http://www.oanda.com/currency/historical-rates/download?" +
								"quote_currency={0}&" + 
								"start_date={1:yyyy-MM-dd}&" +
								"end_date={2:yyyy-MM-01}&" + 
								"period=monthly&display=absolute&rate=0&data_range=c&price=bid&view=table&" +
								"base_currency_0={3}" +
								"&download=csv",
								baseCurrencyCode, startDate, DateTime.UtcNow, quoteCurrencyCode);

			return url;
		}

		static string[] GetDateRows(string[] allRows)
		{
			var dateLines = new List<string>();

			bool datesStarted = false;
			foreach (string row in allRows)
			{
				if (datesStarted == true)
				{
					if (string.IsNullOrEmpty(row.Replace('"', ' ').Trim()))
						break;
					else
						dateLines.Add(row);
				}

				if (row.ToLower().Contains("end date"))
					datesStarted = true;
			}
			return dateLines.ToArray();
		}

		static IEnumerable<string> LineSplitter(string line)
		{
			int fieldStart = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == ',')
				{    
					yield return line.Substring(fieldStart + 1, Math.Max(0, i - fieldStart - 2));
					fieldStart = i + 1;
				}
				if (line[i] == '"')
				{
					for (i++; line[i] != '"'; i++) { }
				}
			}
		}

	}
}