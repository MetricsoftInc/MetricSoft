using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website
{
    public partial class QualityIssueLabel : SQMBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["issue"] == null)
            {
                return;
            }

            try
            {
                int issueID = Convert.ToInt32(Request.QueryString["issue"]);
                QualityIssue qs = new QualityIssue().Load(issueID);
                string strHtml = qs.FormatLabelContent("testlabel.txt");
                if (!String.IsNullOrEmpty(strHtml))
                    divLabel.InnerHtml += strHtml;
            }
            catch (Exception ex)
            {
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }
    }
}