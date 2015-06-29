using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Problem : System.Web.UI.MasterPage
    {
        public string PageGroup = "problem";

        protected void Page_PreRender(object sender, EventArgs e)
        {
          HiddenField hf = (HiddenField)this.Master.FindControl("hdCurrentActiveMenu");
          hf.Value = SessionManager.CurrentMenuItem = "lbProblemMain";
        }

    }
}