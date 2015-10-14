using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Dashboard : SQMBasePage
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(hdCurrentActiveSecondaryTab.Value))
                    uclDashbd0.Initialize(true);

                HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
                hf.Value = SessionManager.CurrentMenuItem = "lbDashboard";
                IsCurrentPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
					ucl.BindDocumentSelect("SYS", 10, true, false, hfDocviewMessage.Value);
                }
            }
        }

    }
}