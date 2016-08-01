using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{

    public partial class Ucl_MetricList : System.Web.UI.UserControl
    {
        protected EHSProfile currentProfile
        {
            get { return (SessionManager.TempObject != null && SessionManager.TempObject is EHSProfile) ? (EHSProfile)SessionManager.TempObject : null; }
            set { SessionManager.TempObject = value; }
        }


        #region inputslist

        public int BindInputsList(EHSProfile profile)
        {
            int status = 0;

            if (profile != null && profile.InputPeriod.InputsList.Count > 0)
            {
                currentProfile = profile;
                pnlInputsList.Visible = true;
                hfInputsListPeriodDate.Value = profile.InputPeriod.PeriodDate.ToShortDateString();
                hfInputsListPlantID.Value = profile.Plant.PLANT_ID.ToString();

                List<EHS_PROFILE_INPUT> inputsList = new List<EHS_PROFILE_INPUT>();
                foreach (EHS_PROFILE_MEASURE metric in profile.Profile.EHS_PROFILE_MEASURE.OrderBy(l => l.EHS_MEASURE.MEASURE_CATEGORY).ThenBy(l => l.EHS_MEASURE.MEASURE_CD).ToList())
                {
                    inputsList.AddRange(profile.InputPeriod.GetPeriodInputList(metric.PRMR_ID));
                }

                gvInputsList.DataSource = inputsList;
                gvInputsList.DataBind();
                if (gvInputsList.Rows.Count > 15)
                {
                    divInputsGVScroll.Attributes["class"] = "scrollArea";
                }
            }
            else
            {
                pnlInputsList.Visible = false;
                gvInputsList.DataSource = null;
                gvInputsList.DataBind();
            }

            return status;
        }

        public void BindProfile(EHSProfile profile)
        {
            currentProfile = profile;
        }

        public int BindInputsList(EHSProfilePeriod period, string context)
        {
            int status = 0;

            if (period != null && period.InputsList.Count > 0)
            {
                pnlInputsList.Visible = true;
                hfInputsListPeriodDate.Value = period.PeriodDate.ToShortDateString();
                if (currentProfile != null)
                    hfInputsListPlantID.Value = currentProfile.Plant.PLANT_ID.ToString();

                gvInputsList.DataSource = period.InputsList;
                gvInputsList.DataBind();
            }
            else
            {
                pnlInputsList.Visible = false;
                gvInputsList.DataSource = null;
                gvInputsList.DataBind();
            }

            return status;
        }

        public void gvInputsList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                Label lbl;
                DateTime dt;
                UOM uom = null;

                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfPRMRID");
                    lbl = (Label)e.Row.Cells[0].FindControl("lblMetricName");

                    EHS_PROFILE_MEASURE measure = currentProfile.Profile.EHS_PROFILE_MEASURE.Where(l => l.PRMR_ID == Convert.ToDecimal(hf.Value)).FirstOrDefault();
                    lbl.Text = measure.EHS_MEASURE.MEASURE_NAME.Trim();
                    lbl = (Label)e.Row.Cells[0].FindControl("lblMetricCode");
                    lbl.Text = measure.EHS_MEASURE.MEASURE_CD;

                    if ((bool)measure.IS_REQUIRED)
                    {
                        e.Row.Cells[1].Attributes.Add("Class", "required");
                    }

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfStatus");
                    if (hf.Value == "D")
                    {
                        Image img = (Image)e.Row.Cells[0].FindControl("imgStatus");
                        img.ImageUrl = "~/images/defaulticon/16x16/delete.png";
                        img.Visible = true;
                    }

                    if (measure.EHS_MEASURE.MEASURE_CATEGORY == "ENGY" || measure.EHS_MEASURE.MEASURE_CATEGORY == "EUTL")
                        e.Row.Cells[0].Attributes.Add("Class", "textStd energyColor");
                    else if (measure.EHS_MEASURE.MEASURE_CATEGORY == "PROD" || measure.EHS_MEASURE.MEASURE_CATEGORY == "SAFE")
                    { ; }
                    else
                        e.Row.Cells[0].Attributes.Add("Class", "textStd wasteColor");

                    lbl = (Label)e.Row.Cells[0].FindControl("lblInvoiceDateFrom");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        dt = Convert.ToDateTime(lbl.Text);
                        lbl.Text = SQMBasePage.FormatDate(dt, "d", false);
                    }
                    lbl = (Label)e.Row.Cells[0].FindControl("lblInvoiceDateTo");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        dt = Convert.ToDateTime(lbl.Text);
                        lbl.Text = SQMBasePage.FormatDate(dt, "d", false);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblValue");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        decimal val;
                        if (Decimal.TryParse(lbl.Text, out val))
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblCost");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        decimal val;
                        if (Decimal.TryParse(lbl.Text, out val))
                        {
                            if (val < 0)
                            {
                                lbl.Text = "";
                                lbl = (Label)e.Row.Cells[0].FindControl("lblCredit");
                            }
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                        }
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblValueUOM");
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == Convert.ToDecimal(lbl.Text));
                    if (uom != null)
                        lbl.Text = uom.UOM_CD;


                }
                catch
                {
                }
            }
        }

        #endregion

        #region inputsresults

        protected void lnkSelectMetric(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            DisplayResults(lnk.CommandArgument);
            lnkChartClose.Focus();
        }

        protected void lnkCloseMetric(object sender, EventArgs e)
        {
            divInputsListReviewArea.Visible = false;
        }

        private int DisplayResults(string cmdID)
        {
            int status = 0;
            SQMMetricMgr metricMgr = null;

            try
            {
                if (!string.IsNullOrEmpty(cmdID))
                {
                    EHS_PROFILE_MEASURE metric = EHSModel.LookupEHSProfileMeasure(new PSsqmEntities(), Convert.ToDecimal(cmdID));
                    decimal calcScopeID = EHSModel.ConvertPRODMeasure(metric.EHS_MEASURE, metric.PRMR_ID);
                    decimal plantID = Convert.ToDecimal(hfInputsListPlantID.Value);
                    DateTime periodDate = Convert.ToDateTime(hfInputsListPeriodDate.Value);

                    divInputsListReviewArea.Visible = true;
                    divInputsListReviewArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
                    if (metricMgr == null)
                    {
                        metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "I", periodDate.AddMonths(-12), periodDate, new decimal[1] { plantID });
                        metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
                    }

                    GaugeDefinition ggCfg = new GaugeDefinition().Initialize();
                    ggCfg.Title = metric.EHS_MEASURE.MEASURE_NAME.Trim() + " - Input History";
                    ggCfg.Height = 250; ggCfg.Width = 650;
                    ggCfg.NewRow = true;
                    ggCfg.DisplayLabel = true;
                    ggCfg.DisplayLegend = false;

                    ggCfg.LabelV = "Quantity";
                    status = uclGauge.CreateControl(SQMChartType.MultiLine, ggCfg, metricMgr.CalcsMethods(new decimal[1] { plantID }, "I", calcScopeID.ToString(), "sum", 32, (int)EHSCalcsCtl.SeriesOrder.PeriodMeasure, ""), divInputsListReviewArea);

                    if (string.IsNullOrEmpty(metric.EHS_MEASURE.PLANT_ACCT_FIELD) && metric.EHS_MEASURE.MEASURE_CATEGORY != "FACT")
                    {
                        ggCfg.Height = 165; ggCfg.Width = 650;
                        ggCfg.Title = "";
                        ggCfg.DisplayLabel = false;
                        ggCfg.LabelV = "Cost";
                        status = uclGauge.CreateControl(SQMChartType.MultiLine, ggCfg, metricMgr.CalcsMethods(new decimal[1] { plantID }, "I", calcScopeID.ToString(), "cost", 32, (int)EHSCalcsCtl.SeriesOrder.PeriodMeasure, ""), divInputsListReviewArea);
                    }
                }
            }
            catch
            {
                ;
            }

            return status;
        }

        #endregion

        #region historylist

        public int BindHistoryList(EHSProfile profile, List<MetricData> dataList)
        {
            int status = 0;

            if (profile != null && dataList.Count > 0)
            {
                currentProfile = profile;
                pnlHSTMetricsList.Visible = true;

				gvHSTMetricsList.DataSource = dataList.OrderBy(l => l.Measure.MEASURE_CATEGORY).ThenBy(l => l.Measure.MEASURE_CD).Select(l => l.MetricRec).ToList();
                gvHSTMetricsList.DataBind();
                if (gvHSTMetricsList.Rows.Count > 15)
                {
                    divHSTMetricsGVScroll.Attributes["class"] = "scrollArea";
                }
            }
            else
            {
                pnlHSTMetricsList.Visible = false;
                gvHSTMetricsList.DataSource = null;
                gvHSTMetricsList.DataBind();
            }

            return status;
        }
        public void gvHSTMetricsList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                Label lbl;
                decimal val;
                UOM uom = null;

                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfHSTMetricID");
                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTMetricName");

                    EHS_PROFILE_MEASURE measure = currentProfile.Profile.EHS_PROFILE_MEASURE.Where(l => l.MEASURE_ID == Convert.ToDecimal(hf.Value)).FirstOrDefault();
                    lbl.Text = measure.EHS_MEASURE.MEASURE_NAME.Trim();
                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTMetricCode");
                    lbl.Text = measure.EHS_MEASURE.MEASURE_CD;

                    if ((bool)measure.IS_REQUIRED)
                    {
                        // System.Web.UI.HtmlControls.HtmlTableCell cell1 = (System.Web.UI.HtmlControls.HtmlTableCell)e.Row.Cells[0].FindControl("lblInvoiceDateFrom");
                        e.Row.Cells[1].Attributes.Add("Class", "required");
                    }

                    if (measure.EHS_MEASURE.MEASURE_CATEGORY == "ENGY" || measure.EHS_MEASURE.MEASURE_CATEGORY == "EUTL")
                        e.Row.Cells[0].Attributes.Add("Class", "textStd energyColor");
                    else
                        e.Row.Cells[0].Attributes.Add("Class", "textStd wasteColor");

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTValue");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        if (Decimal.TryParse(lbl.Text, out val))
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTInputValue");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        if (Decimal.TryParse(lbl.Text, out val))
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTCost");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        if (Decimal.TryParse(lbl.Text, out val))
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTInputCost");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        if (Decimal.TryParse(lbl.Text, out val))
                            lbl.Text = SQMBasePage.FormatValue(val, 2);
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTValueUOM");
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == Convert.ToDecimal(lbl.Text));
                    if (uom != null)
                        lbl.Text = uom.UOM_CD;

                    lbl = (Label)e.Row.Cells[0].FindControl("lblHSTInputUOM");
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == Convert.ToDecimal(lbl.Text));
                    if (uom != null)
                        lbl.Text = uom.UOM_CD;
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}