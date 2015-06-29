using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class QAILogin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string customerLogo = "";
                customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"];
                if (string.IsNullOrEmpty(customerLogo) || customerLogo.Contains("Metricsoft"))
                {
                    imgLogo.ImageUrl = "~/images/company/MetricsoftLogoSmall.png";
                }
                else
                {
                    imgLogo.ImageUrl = "~/images/company/" + customerLogo;
                }
                 
                string title = System.Configuration.ConfigurationManager.AppSettings["MainTitle"];
                if (!string.IsNullOrEmpty(title))
                    lblMainTitle.Text = title;
                /*
                string info = System.Configuration.ConfigurationManager.AppSettings["MainInfo"];
                if (!string.IsNullOrEmpty(info))
                {
                    lblMainInfo.Text = info;
                    lblMainInfo.Visible = true;
                }
                */
                SetupPage();
            }

            
        }

        protected void SetupPage()
        {
            ;
        }
    }
}