using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website.Automated
{
	public partial class CheckCurrency : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string baseCurrencyCode = "EUR"; // todo: read from database
			CurrencyMgr.CheckForUpdates(baseCurrencyCode);
		}
	}
}