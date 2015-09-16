using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	public partial class GenericError404 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblPage.Text = "Page = " + Request.QueryString.Get("aspxerrorpath");
			if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.AbsolutePath))
			{
				lblReferrer.Text = "Calling page = " + Request.UrlReferrer.AbsolutePath;
			}
		}
	}
}