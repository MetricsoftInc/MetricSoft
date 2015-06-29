using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;

namespace SQM.Website
{
	public partial class CurrencyTest : SQMBasePage
    {
        decimal _companyId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

            if (IsPostBack)
            {
				string fromCurrency = ddlFromCurrency.SelectedValue;
				string toCurrency = ddlToCurrency.SelectedValue;
				string baseCurrency = ddlBaseCurrency.SelectedValue;

				double currencyValue = CurrencyConverter.Convert(
					fromCurrency,
					toCurrency,
					baseCurrency,
					Convert.ToDouble(tbQuantity.Text),
					Convert.ToInt32(ddlYear.SelectedValue),
					Convert.ToInt32(ddlMonth.SelectedValue)
					);

				lblResults.Text = string.Format(
					"{0} {1} in {2} = {3:0.00000}<br/>on {4}-{5}",
					tbQuantity.Text, fromCurrency, toCurrency,
					currencyValue, ddlYear.SelectedValue, ddlMonth.SelectedValue);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
			
        }

    }
}
