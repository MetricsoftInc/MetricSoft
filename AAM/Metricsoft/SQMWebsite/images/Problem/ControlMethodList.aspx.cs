using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
   public partial class ControlMethodList : SQMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetupPage();
        }

        private void SetupPage()
        {
            ddlVarChart.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("chartTypeVAR"));
            ddlAttChart.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("chartTypeATT"));

            ddlVarChart.SelectedValue = "XBR";
            ddlAttChart.SelectedValue = "C";

            tbTest01.Text = tbTest11.Text  = tbTest21.Text = "1";
            tbTest13.Text = tbTest23.Text = "7";
            tbTest14.Text = tbTest24.Text = "2"; tbTest14P.Text = tbTest24P.Text = "3";
            tbTest15.Text = tbTest25.Text = "4"; tbTest15P.Text = tbTest25P.Text = "5";
            tbTest16.Text = tbTest26.Text = "8"; tbTest16P.Text = tbTest26P.Text = "8";
            tbTest17.Text = tbTest27.Text = "8";
            tbTest18.Text = tbTest28.Text = "14";
            tbTest19.Text = tbTest29.Text = "15";

            cbTest01.Checked = cbTest11.Checked = cbTest13.Checked = cbTest14.Checked = cbTest15.Checked = cbTest16.Checked = true;
            cbTest21.Checked = cbTest23.Checked = cbTest24.Checked = cbTest25.Checked = cbTest26.Checked = true;
        }
    }
}