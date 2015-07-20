using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Compliance_COA : SQMBasePage
    {
        static bool loaded = false;
        static bool isEditMode;
        static COAMgr coaReport;
        static List<PLANT> plantList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
                hfld.Value = SessionManager.CurrentAdminTab = "lbCOA";

                if (coaReport == null)
                {
                    coaReport = new COAMgr();
                    coaReport.AddBusOrg(SessionManager.SessionContext.ActiveCompany().COMPANY_ID, SessionManager.UserContext.BusinessOrg.BUS_ORG_ID);
                    coaReport.AddPart(6, 1, true); //////////////////////////////////
                }

                if ((bool)SessionManager.ReturnStatus)
                {
                    string s = SessionManager.ReturnObject.GetType().ToString();
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("PLANTPART"))
                    {
                        vw_CustPlantPart plantPart = (vw_CustPlantPart)SessionManager.ReturnObject;
                        coaReport.AddPart(plantPart.PART_ID, plantPart.CUST_COMPANY_ID, true);
                    }
                    /*
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("QI_OCCUR"))
                    {
                        QI_OCCUR qiOccur = (QI_OCCUR)SessionManager.ReturnObject;
                        qualityIssue = new QualityIssue().Load(qiOccur.QIO_ID);
                    }
                    */
                    SessionManager.ReturnObject = null;
                    SessionManager.ReturnStatus = false;
                }
                SetupPage();
            }
        }

        private void SetupPage()
        {
            ddlFormType.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("COAForm","long"));

            if (coaReport.BusOrg != null)
            {
                SetFindControlValue("lblParentBU_out", hfBase, coaReport.BusOrg.ORG_NAME);
                ddlPlant.DataSource = coaReport.PlantList;
                ddlPlant.DataTextField = "PLANT_NAME";
                ddlPlant.DataValueField = "PLANT_ID";
                ddlPlant.DataBind();
                ddlPlant.Items.Insert(0, new ListItem("", "0"));
            }

            if (coaReport.Part == null)
            {
                pnlPartDetail.Visible = pnlPartBOM.Visible = pnlCtlResult.Visible = false;
            }
            else 
            {
                tbPartNumber.Text = coaReport.Part.PART_NUM;
                pnlPartDetail.Visible = pnlPartBOM.Visible = pnlCtlResult.Visible = true;
                SetFindControlValue("lblPartNum_out", hfBase, coaReport.Part.PART_NUM);
                SetFindControlValue("lblPartNumFull_out", hfBase, SQMModelMgr.GetFullPartNumber(coaReport.Part));
                SetFindControlValue("lblPartName_out", hfBase, coaReport.Part.PART_NAME);
                SetFindControlValue("lblPartSerial_out", hfBase, coaReport.Part.SERIAL_NUM);
                SetFindControlValue("lblDrawing_out", hfBase, coaReport.Part.DRAWING_REF);
                SetFindControlValue("lblPartRevisionLevel_out", hfBase, coaReport.Part.REVISION_LEVEL);
                SetFindControlValue("lblPartStatus_out", hfBase, WebSiteCommon.GetStatusString(coaReport.Part.STATUS));

                GridView gv = (GridView)hfBase.FindControl("gvPartAssyList");
                gv.DataSource = coaReport.Part.PART_COMPONENT;
                gv.DataBind();
                gv.Columns[5].Visible = true;
                gv.Columns[4].Visible = false;
                SetGridViewDisplay(gv, (Label)hfBase.FindControl("lblPartAssyListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divGVScrollBOM2"), 0);

                GridView gv2 = (GridView)hfBase.FindControl("gvPartProcList");
                gv2.DataSource = coaReport.Part.PART_PROCESS;
                gv2.DataBind();
                gv2.Columns[4].Visible = true;
                SetGridViewDisplay(gv2, (Label)hfBase.FindControl("lblPartProcListEmpty"), (System.Web.UI.HtmlControls.HtmlGenericControl)hfBase.FindControl("divGVScrollBOM1"), 0);

                DoCtlPlanSteps();
            }

        }

        public void gvPartAssyList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblBOMPartID");
                    PART compPart = SQMModelMgr.LookupPart(Convert.ToDecimal(lbl.Text), SessionManager.SessionContext.ActiveCompany().COMPANY_ID);  // mt - todo: get the component part info from the original query
                    if (compPart != null)
                    {
                        lbl = (Label)e.Row.Cells[0].FindControl("lblCompPartNum");
                        lbl.Text = compPart.PART_NUM;
                        lbl = (Label)e.Row.Cells[0].FindControl("lblCompPartName");
                        lbl.Text = compPart.PART_NAME;
                        lbl = (Label)e.Row.Cells[0].FindControl("lblCompSerial");
                        lbl.Text = compPart.SERIAL_NUM;
                    }
                }
                catch
                {
                }
            }
        }

        public void gvPartProcList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblPartProcType");
                    lbl.Text = WebSiteCommon.GetXlatValue("partProcess", lbl.Text);
                }
                catch
                {
                }
            }
        }

        public void DoCtlPlanSteps()
        {
            GridView gv = (GridView)hfBase.FindControl("gvCOAResult");
            gv.DataSource = coaReport.CtlPlan.CtlPlan.CTL_PLAN_STEP.OrderBy(p => p.STEP_SEQ);
            gv.RowStyle.VerticalAlign = VerticalAlign.Top;
            gv.DataBind();
            //SetGridViewDisplay(gv, partList.Count, "lblPartPlantListEmpty");
        }

        public void gvCOAResult_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblPlanStepID");
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMeasureGrid");

                    CTL_PLAN_STEP step = coaReport.CtlPlan.CtlPlan.CTL_PLAN_STEP.First(s => s.CTLPLANSTEP_ID == Convert.ToInt32(lbl.Text));
                    gv.DataSource = step.CTL_PLAN_MEASURE;
                    gv.RowStyle.VerticalAlign = VerticalAlign.Top;
                    gv.DataBind();
                }
                catch
                {
                }
            }
        }

        public void gvMeasure_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblSpecValues");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfSpecLSL");
                    lbl.Text = hf.Value;
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfSpecUSL");
                    lbl.Text += (" / " + hf.Value);
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfUOM");
                    lbl.Text += (" " + hf.Value);

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfStepID");
                    HiddenField hf2 = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureID");
                    CTL_PLAN_MEASURE measure = coaReport.CtlPlan.CtlPlan.CTL_PLAN_STEP.First(s => s.CTLPLANSTEP_ID == Convert.ToInt32(hf.Value)).CTL_PLAN_MEASURE.First(m => m.CTLMEASURE_ID == Convert.ToInt32(hf2.Value));
                    CTL_PLAN_MEASURE_SUM stats = measure.CTL_PLAN_MEASURE_SUM.First();
 
                    List<MetricString> statsList = new List<MetricString>();
                    statsList.Add(new MetricString().New("Samples:", stats.SAMPLES));
                    statsList.Add(new MetricString().New("Mean:", stats.MEAN));
                    statsList.Add(new MetricString().New("Sdev:", stats.SDEV));
                    if (measure.MEASURE_TYPE == "VAR")
                    {
                        statsList.Add(new MetricString().New("CP:", stats.CP));
                        statsList.Add(new MetricString().New("CPk:", stats.CPK));
                    }

                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMetricGrid");
                    gv.DataSource = statsList;
                    gv.DataBind();
                }
                catch
                {
                }
            }
        }

        public void gvResult_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblStepID");
                    Label lbl2 = (Label)e.Row.Cells[0].FindControl("lblMeasureID");
                    CTL_PLAN_MEASURE measure = coaReport.CtlPlan.CtlPlan.CTL_PLAN_STEP.First(s => s.CTLPLANSTEP_ID == Convert.ToInt32(lbl.Text)).CTL_PLAN_MEASURE.First(m => m.CTLMEASURE_ID == Convert.ToInt32(lbl2.Text));
                    CTL_PLAN_MEASURE_SUM stats = measure.CTL_PLAN_MEASURE_SUM.First();
                    /*
                    lbl = (Label)e.Row.Cells[0].FindControl("lblResults");
                    lbl.Text = ("N: " + stats.SAMPLES.ToString()
                        + " XB: " + stats.MEAN.ToString()
                        + " CP: " + stats.CP.ToString()
                        + " CPk: " + stats.CPK.ToString());
                    */
                    List<MetricString> statsList = new List<MetricString>();
                    statsList.Add(new MetricString().New("Samples",Convert.ToDouble(stats.SAMPLES)));
                    statsList.Add(new MetricString().New("Samples", stats.MEAN));

                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMetricGrid");
                    gv.DataSource = statsList;
                    gv.DataBind();
                }
                catch
                {
                }
            }
        }

        public void gvMetric_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {

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