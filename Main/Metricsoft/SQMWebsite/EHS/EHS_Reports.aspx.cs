using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website
{
    public partial class EHS_Reports : SQMBasePage
    {
        decimal _companyId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _companyId = SessionManager.UserContext.HRLocation.Company.COMPANY_ID;
            SQM.Website.PSsqmEntities entities = new PSsqmEntities();

            CreateChart(ref RadHtmlChart1, 2, 2011, "Electricity");
            CreateChart(ref RadHtmlChart2, 3, 2011, "Natural Gas");
            CreateChart(ref RadHtmlChart3, 26, 2011, "Construction Debris");
            CreateChart(ref RadHtmlChart4, 27, 2011, "Iron and Steel Scrap"); 
            
        }


        protected void CreateChart(ref RadHtmlChart chart, int measureId, int year, string measure)
        {
            chart.Skin = "Default";

            chart.ChartTitle.Text = year + " " + measure;
            var line1a = new Telerik.Web.UI.LineSeries();
            var line1b = new Telerik.Web.UI.LineSeries();

            line1a.Name = "Value";
            line1b.Name = "Cost";

            line1a.LabelsAppearance.DataFormatString = "#,##0";
            line1b.LabelsAppearance.DataFormatString = "#,##0";

            // Bind to database
            var data1 = from metrics in entities.EHS_PROFILE_MEASURE
                        join data in entities.EHS_PROFILE_INPUT on metrics.PRMR_ID equals data.PRMR_ID
                        where metrics.PLANT_ID == 36 &&  metrics.MEASURE_ID == measureId && data.PERIOD_YEAR == year
                        orderby data.PERIOD_MONTH
                        select new
                        {
                            Month = data.PERIOD_MONTH,
                            Value = data.MEASURE_VALUE,
                            Cost = data.MEASURE_COST,
                        };

            if (data1.Count() > 0)
            {
                line1a.DataFieldY = "Value";
                line1b.DataFieldY = "Cost";

                chart.DataSource = data1;
                chart.DataBind();
            }


            chart.Legend.Appearance.Position = ChartLegendPosition.Top;
            chart.Legend.Appearance.Visible = true;

            chart.PlotArea.XAxis.DataLabelsField = "Month";
            chart.PlotArea.XAxis.TitleAppearance.Text = "Month";
            chart.PlotArea.YAxis.LabelsAppearance.DataFormatString = "#,##0";
            chart.PlotArea.YAxis.TitleAppearance.Text = "Value/Cost";

            chart.PlotArea.Series.Add(line1a);
            chart.PlotArea.Series.Add(line1b);
        }

    }
}
