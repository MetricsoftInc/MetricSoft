using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Error : SQMBasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.Params["error_detail"]))
            {
                String error_detail = Request.Params["error_detail"];
                switch (error_detail)
                {
                    case "EXPIRED":
                        {
                            lblGenericError.Text = "The page you requested does not exist or has expired.";
                        }
                        break;
                }
            }

			Exception ex = (Exception)Session["LastError"];
			if (ex != null)
			{
				string strInnerException = "";
				try
				{
					strInnerException = " (" + ex.InnerException.Message + ")";
				}
				catch { }
				lblError.Text = ex.Message + strInnerException.Trim();
				Session["LastError"] = null;
			}
			else
			{
				lblError.Text = "An error has occurred during login.";
			}

			try
			{
				String errorIndex = Session["ErrorIndex"].ToString();
				lblErrorIndex.Text = "Error Index: " + errorIndex;
			}
			catch
			{
				lblErrorIndex.Text = "";
			}
        }
    }
}