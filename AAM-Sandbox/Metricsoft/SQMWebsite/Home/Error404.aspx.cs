using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	public partial class Error404 : SQMBasePage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (SessionManager.SessionContext == null)
					throw new UserContextError();

				lblPage.Text = "Page = " + Request.QueryString.Get("aspxerrorpath");
				if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.AbsolutePath))
				{
					lblReferrer.Text = "Calling page = " + Request.UrlReferrer.AbsolutePath;
				}

				HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
				hf.Value = SessionManager.CurrentMenuItem = "lbDashboard";
				IsCurrentPage();
			}
		}
	}
}