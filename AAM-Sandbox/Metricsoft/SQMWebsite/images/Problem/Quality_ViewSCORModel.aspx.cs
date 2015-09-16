using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Quality_ViewSCORModel : SQMBasePage
    {
        static SCOR_MODEL theModel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
                hfld.Value = SessionManager.CurrentAdminTab = "lbSCORModel";

                SetupPage();
            }
        }

        private void SetupPage()
        {
            theModel = MetricsMgr.LookupSCORModel(SessionManager.SessionContext.ActiveCompany().COMPANY_ID);
            if (theModel == null)
            {
               // throw and error here
                return;
            }

            lblModel_out.Text = theModel.MODEL_DESC;
            lblUpdateBy_out.Text = theModel.LAST_UPD_BY;
            lblUpdateDate_out.Text = WebSiteCommon.LocalTime((DateTime)theModel.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();

            gvMetrics.DataSource = theModel.SCOR_METRIC;
            gvMetrics.DataBind();

        }

        public void gvMetrics_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblMetricID");

                    SCOR_METRIC metric = theModel.SCOR_METRIC.First(l=> l.SCOR_METRIC_ID == Convert.ToInt32(lbl.Text));
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvFactorGrid");
                    gv.DataSource = metric.SCOR_METRIC_FACTOR;
                    gv.DataBind();
                }
                catch
                {
                }
            }
        }

        public void gvFactor_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblMetricID");
                    SCOR_METRIC metric = theModel.SCOR_METRIC.First(l => l.SCOR_METRIC_ID == Convert.ToInt32(lbl.Text));
                    lbl = (Label)e.Row.Cells[0].FindControl("lblMetricFactorID");
                    SCOR_METRIC_FACTOR fact = metric.SCOR_METRIC_FACTOR.First(f => f.METRIC_FACTOR_ID == Convert.ToInt32(lbl.Text));
                    if (fact.FACTOR_METRIC_ID > 0)
                    {
                        SCOR_METRIC factorMetric = theModel.SCOR_METRIC.First(l => l.SCOR_METRIC_ID == fact.FACTOR_METRIC_ID);
                        lbl = (Label)e.Row.Cells[0].FindControl("lbFactorName_out");
                        lbl.Text = factorMetric.METRIC_NAME;
                        lbl = (Label)e.Row.Cells[0].FindControl("lbFactorDesc_out");
                        lbl.Text = factorMetric.METRIC_DESC;
                    }
                    else
                    {
                        lbl = (Label)e.Row.Cells[0].FindControl("lbFactorName_out");
                        lbl.Text = fact.SCOR_FACTOR.FACTOR_NAME;
                        lbl = (Label)e.Row.Cells[0].FindControl("lbFactorDesc_out");
                        lbl.Text = fact.SCOR_FACTOR.FACTOR_DESC;
                    }
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbFactorWeight");
                    tb.Text = fact.WEIGHING.ToString();
                }
                catch
                {
                }
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
           // Response.Redirect("/Home.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
           // Response.Redirect("/Home.aspx");
        }
    }
}