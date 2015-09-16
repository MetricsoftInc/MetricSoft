using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Quality_CostRecord : SQMBasePage
    {
        static bool isEditMode;
        static IncidentCostReport costReport;
        static PLANT_LINE staticPlantLine;
        static LABOR_TYPE staticLaborType;
        static IncidentCostReportStatus currentStatus;
        static decimal staticIncidentID;
        bool initSearch;
        bool isAutoCreated;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            /*
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
          
           
            uclSearchBar.OnEditClick += uclSearchBar_OnEditClick;
            uclSearchBar.OnSaveClick += uclSearchBar_OnSaveClick;

            uclIssueList.OnQualityIssueListCloseClick += btnIssueListCancel_Click;
            uclIssueList.OnQualityIssueClick += OnIssue_Click;

            uclAttach1.OnUpdateAttachment += uclAttach_OnUpdateAttachment;
            */
            uclSearchBar.OnSearchClick += OnListClick;
            uclSearchBar.OnNewClick += OnNewClick;
            uclSearchBar.OnCancelClick += OnCancelClick;
            uclIncidents.OnQualityIssueClick += OnIssue_Click;
            uclSearchBar.OnSaveClick += OnSaveClick;
            uclReportList.OnReportClick += OnReportClick;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //uclSearchBar.SetButtonsVisible(true, false, UserContext.CheckAccess("SQM", "201") >= AccessMode.Update ? true : false, UserContext.CheckAccess("SQM", "201") >= AccessMode.Update ? true : false, false, false);
                uclSearchBar.SetButtonsEnabled(true, false, true, false, false, false);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("SQM", 2, true, true, "");
                }

                uclSearchBar.PageTitle.Text = lblCRTitle.Text;
                SetupPage();
                // from quality incident entry page
                if ((bool)SessionManager.ReturnStatus)
                {
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("QI_OCCUR"))
                    {
                        QI_OCCUR qiIssue = (QI_OCCUR)SessionManager.ReturnObject;

                        OnNewClick();
                        costReport.CostReport.COST_REPORT_DESC = qiIssue.INCIDENT.DESCRIPTION;
                        costReport.AddIncident(qiIssue.INCIDENT_ID);
                        uclIncidents.BindQualityIssueList(costReport.IncidentList, false);
                        DisplayReport();
                        BindIncident(qiIssue.INCIDENT_ID);
                        isAutoCreated = true;
                    }
                    SessionManager.ClearReturns();
                }
                else
                {
                    if (costReport == null)
                    {
                        OnListClick();
                    }
                }
            }
            else
            {
                // from incident popop selector
                if ((bool)SessionManager.ReturnStatus)
                {
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("INCIDENT"))
                    {
                        List<INCIDENT> incidentList = (List<INCIDENT>)SessionManager.ReturnObject;
                        costReport.AddIncident(incidentList[0].INCIDENT_ID);
                        uclIncidents.BindQualityIssueList(costReport.IncidentList, false);
                        BindIncident(incidentList[0].INCIDENT_ID);
                    }
                    SessionManager.ClearReturns();
                }
            }
        }

        private void OnListClick()
        {
            divNavArea.Visible = divWorkArea.Visible = false;
            divSearchList.Visible = true;
            uclReportList.BindCRList(IncidentCostReport.SelectCostReportList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0));
        }
        private void OnNewClick()
        {
            divSearchList.Visible = false;
            divWorkArea.Visible = divNavArea.Visible = true;
            EnableControls(divNavArea.Controls, true);
            EnableControls(divWorkArea.Controls, true);
            EnableControls(divCostTable.Controls, false);
            uclSearchBar.SetButtonsEnabled(true, false, true, true, false, false);
            ResetControlValues(divPageBody.Controls);
            costReport = new IncidentCostReport().CreateNew(Session.SessionID, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID);
            tbCRName.Focus();
        }
        private void OnCancelClick()
        {
            costReport = null;
            staticIncidentID = 0;
            ResetControlValues(divPageBody.Controls);
            EnableControls(divNavArea.Controls, false);
            EnableControls(divWorkArea.Controls, false);
            EnableControls(divCostTable.Controls, false);
            uclSearchBar.SetButtonsEnabled(true, false, true, false, false, false);
        }
        protected void OnReportClick(decimal reportID)
        {
            costReport = new IncidentCostReport().Initialize();
            costReport.Load(reportID);
            staticIncidentID = 0;
            divSearchList.Visible = false;
            divWorkArea.Visible = divNavArea.Visible =  true;
            EnableControls(divNavArea.Controls, true);
            EnableControls(divWorkArea.Controls, true);
            EnableControls(divCostTable.Controls, false);
            uclSearchBar.SetButtonsEnabled(true, false, true, true, false, false);
            DisplayReport();
        }
        protected void OnIssue_Click(decimal issueID)
        {
            EnableControls(divCostTable.Controls, true);
            BindIncident(issueID);
        }

        private void OnSaveClick()
        {
            if (costReport.IncidentList.Count == 0)
            {
                MessageDisplay(IncidentCostReportStatus.InputError);
            }
            else
            {
                SaveReport();
            }
        }

        private void SetupPage()
        {
            if (ddlCRType.Items.Count == 0)
                ddlCRType.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("issueResponsible"));
        }
       
        protected void ddlSelectChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            decimal recordID = 0;

            if (ddlSender.ID.Contains("PlantLine"))
            {
                if (staticPlantLine == null  ||  staticPlantLine.PLANT_LINE_ID != Convert.ToDecimal(ddlSender.SelectedValue))
                    staticPlantLine = SQMModelMgr.LookupPlantLine(costReport.Entities, 0, Convert.ToDecimal(ddlSender.SelectedValue), "", false);
            }
            if (ddlSender.ID.Contains("LaborType"))
            {
                if (staticLaborType == null || staticLaborType.LABOR_TYP_ID != Convert.ToDecimal(ddlSender.SelectedValue))
                    staticLaborType = SQMModelMgr.LookupLaborType(costReport.Entities, 0, 0, 0, Convert.ToDecimal(ddlSender.SelectedValue), "", false);
            }

            switch (ddlSender.ID)
            {
                case "ddlCRPlantLineActual":
                   
                    if (staticPlantLine != null)
                    {
                        tbCRDowntimeRateActual.Text = staticPlantLine.DOWNTIME_RATE.ToString();
                        udpBurdenActual.Update();
                    }
                    break;
                case "ddlCRPlantLineAvoid":
                    if (staticPlantLine != null)
                    {
                        tbCRDowntimeRateAvoid.Text = staticPlantLine.DOWNTIME_RATE.ToString();
                        udpBurdenAvoid.Update();
                    }
                    break;
                case "ddlCRLaborTypeActual":

                    if (staticLaborType != null)
                    {
                        tbCRLaborRateActual.Text = staticLaborType.LABOR_RATE.ToString();
                        udpLaborActual.Update();
                    }
                    break;
                case "ddlCRLaborTypeAvoid":
                    if (staticLaborType != null)
                    {
                        tbCRLaborRateAvoid.Text = staticLaborType.LABOR_RATE.ToString();
                        udpLaborAvoid.Update();
                    }
                    break;
                default:
                    break;
            }
        }

        private void MessageDisplay(IncidentCostReportStatus status)
        {
            lblInputError.Visible = lblNoIncidents.Visible = false;

            currentStatus = status;
            switch (status)
            {
                case IncidentCostReportStatus.InputError:
                    lblInputError.Visible = true;
                    break;
                case IncidentCostReportStatus.NoIncidents:
                    lblNoIncidents.Visible = true;
                    break;
                default:
                    break;
            }
        }

        protected void btnCalculateClick(object sender, EventArgs e)
        {
            try
            {
                COST_REPORT_ITEM item = costReport.CostReport.COST_REPORT_ITEM.Where(l => l.INCIDENT_ID == staticIncidentID).FirstOrDefault();
                
                item.NCM_ACT_QTY = Convert.ToDecimal(tbCRItemQtyActual.Text);
                item.UNIT_COST = Convert.ToDecimal(tbCRUnitCostActual.Text);
                item.NCM_ACT_COST = item.NCM_ACT_QTY * item.UNIT_COST;
                item.NCM_POT_QTY = Convert.ToDecimal(tbCRItemQtyAvoid.Text);
                item.NCM_POT_COST = item.NCM_POT_QTY * item.UNIT_COST;
                item.NCM_COMMENTS = tbCRItemNote.Text;

                item.DWN_ACT_LINE_ID = Convert.ToDecimal(ddlCRPlantLineActual.SelectedValue);
                item.DWN_ACT_RATE = Convert.ToDecimal(tbCRDowntimeRateActual.Text);
                item.DWN_ACT = Convert.ToDecimal(tbCRDowntimeActual.Text);
                item.DWN_ACT_COST = item.DWN_ACT_RATE * item.DWN_ACT;
                item.DWN_POT_LINE_ID = Convert.ToDecimal(ddlCRPlantLineAvoid.SelectedValue);
                item.DWN_POT_RATE = Convert.ToDecimal(tbCRDowntimeRateAvoid.Text);
                item.DWN_POT = Convert.ToDecimal(tbCRDowntimeAvoid.Text);
                item.DWN_POT_COST = item.DWN_POT_RATE * item.DWN_POT;
                item.DWN_COMMENTS = tbCRDowntimeNote.Text;

                item.LABOR_ACT_DEPT_ID = Convert.ToDecimal(ddlCRLaborDeptActual.SelectedValue);
                item.LABOR_ACT_TYPE_ID = Convert.ToDecimal(ddlCRLaborTypeActual.SelectedValue);
                item.LABOR_ACT_RATE = Convert.ToDecimal(tbCRLaborCostActual.Text);
                item.LABOR_ACT = Convert.ToDecimal(tbCRLaborActual.Text);
                item.LABOR_ACT_COST = item.LABOR_ACT_RATE * item.LABOR_ACT;
                item.LABOR_POT_DEPT_ID = Convert.ToDecimal(ddlCRLaborDeptAvoid.SelectedValue);
                item.LABOR_POT_TYPE_ID = Convert.ToDecimal(ddlCRLaborTypeAvoid.SelectedValue);
                item.LABOR_POT_RATE = Convert.ToDecimal(tbCRLaborCostAvoid.Text);
                item.LABOR_POT = Convert.ToDecimal(tbCRLaborAvoid.Text);
                item.LABOR_POT_COST = item.LABOR_POT_RATE * item.LABOR_POT;
                item.LABOR_COMMENTS = tbCRLaborNote.Text;

                item.SHIP_ACT_TYPE = tbCRShipDescActual.Text;
                item.SHIP_ACT_COST = Convert.ToDecimal(tbCRShipCostActual.Text);
                item.SHIP_POT_TYPE = tbCRShipDescAvoid.Text;
                item.SHIP_POT_COST = Convert.ToDecimal(tbCRShipCostAvoid.Text);
                item.SHIP_COMMENTS = tbCRShipNote.Text;

                item.TOTAL_ACT_COST = item.NCM_ACT_COST + item.DWN_ACT_COST + item.LABOR_ACT_COST + item.SHIP_ACT_COST;
                item.TOTAL_POT_COST = item.NCM_POT_COST + item.DWN_POT_COST + item.LABOR_POT_COST + item.SHIP_POT_COST;
                item.TOTAL_COMMENTS = tbCRTotalNote.Text;

                tbCRIncidentCostActual.Text = FormatValue((decimal)item.TOTAL_ACT_COST, 2);
                tbCRIncidentCostAvoid.Text = FormatValue((decimal)item.TOTAL_POT_COST, 2);

                costReport.CostReport.SUM_ACT_COST = costReport.CostReport.SUM_POT_COST = 0;
                foreach (COST_REPORT_ITEM critem in costReport.CostReport.COST_REPORT_ITEM)
                {
                    costReport.CostReport.SUM_ACT_COST += critem.TOTAL_ACT_COST;
                    costReport.CostReport.SUM_POT_COST += critem.TOTAL_POT_COST;
                }
            }
            catch
            {
                MessageDisplay(IncidentCostReportStatus.InputError);
                return;
            }

            udpTotals.Update();
        }

        private void DisplayReport()
        {
            EnableControls(divNavArea.Controls, true);
            EnableControls(divWorkArea.Controls, true);
            EnableControls(divCostTable.Controls, false);
            uclSearchBar.TitleItem.Text = costReport.ReportID;
            tbCRName.Text = costReport.CostReport.COST_REPORT_NAME;
            tbCRDesc.Text = costReport.CostReport.COST_REPORT_DESC;
            lblCRTotalCostActual.Text = FormatValue((decimal)costReport.CostReport.SUM_ACT_COST, 2);
            lblCRTotalCostAvoid.Text = FormatValue((decimal)costReport.CostReport.SUM_POT_COST, 2);
            lblUpdateBy.Text = costReport.CostReport.LAST_UPD_BY;
            lblUpdateDate.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)costReport.CostReport.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID), "d", false);
            uclIncidents.BindQualityIssueList(costReport.IncidentList, false);
        }

        public void BindIncident(decimal incidentID)
        {
            staticIncidentID = incidentID;
            try
            {
                COST_REPORT_ITEM item = costReport.CostReport.COST_REPORT_ITEM.Where(l=> l.INCIDENT_ID == incidentID).FirstOrDefault();
                QualityIncidentData qiData = costReport.IncidentList.Where(l => l.Incident.INCIDENT_ID == item.INCIDENT_ID).FirstOrDefault();

                lblCRIncidentIDHdr.Text = WebSiteCommon.FormatID(qiData.Incident.INCIDENT_ID, 6);

                tbCRItemQtyActual.Text = item.NCM_ACT_QTY.ToString();
                tbCRItemQtyActual.Focus();
                tbCRItemQtyAvoid.Text = item.NCM_POT_QTY.ToString();
                tbCRUnitCostActual.Text = item.UNIT_COST.ToString();
                tbCRUnitCostAvoid.Text = item.UNIT_COST.ToString();
                tbCRItemCostActual.Text = item.NCM_ACT_COST.ToString();
                tbCRItemCostAvoid.Text = item.NCM_POT_COST.ToString();
                tbCRItemNote.Text = item.NCM_COMMENTS;

                List<PLANT_LINE> lineList = SQMModelMgr.SelectPlantLineList(this.entities, (decimal)qiData.Incident.DETECT_PLANT_ID);
                ddlCRPlantLineActual.DataSource = lineList;
                ddlCRPlantLineActual.DataValueField = "PLANT_LINE_ID";
                ddlCRPlantLineActual.DataTextField = "PLANT_LINE_NAME";
                ddlCRPlantLineActual.DataBind();
                ddlCRPlantLineActual.Items.Insert(0, new ListItem("", "0"));
                ddlCRPlantLineAvoid.DataSource = lineList;
                ddlCRPlantLineAvoid.DataValueField = "PLANT_LINE_ID";
                ddlCRPlantLineAvoid.DataTextField = "PLANT_LINE_NAME";
                ddlCRPlantLineAvoid.DataBind();
                ddlCRPlantLineAvoid.Items.Insert(0, new ListItem("", "0"));

                if (ddlCRPlantLineActual.Items.FindByValue(item.DWN_ACT_LINE_ID.ToString()) != null)
                    ddlCRPlantLineActual.SelectedValue = item.DWN_ACT_LINE_ID.ToString();
                tbCRDowntimeRateActual.Text = item.DWN_ACT_RATE.ToString();
                tbCRDowntimeRateAvoid.Text = item.DWN_POT_RATE.ToString();
                tbCRDowntimeActual.Text = item.DWN_ACT.ToString();
                if (ddlCRPlantLineAvoid.Items.FindByValue(item.DWN_POT_LINE_ID.ToString()) != null)
                    ddlCRPlantLineAvoid.SelectedValue = item.DWN_POT_LINE_ID.ToString();
                tbCRDowntimeAvoid.Text = item.DWN_POT.ToString();
                tbCRDowntimeCostActual.Text = item.DWN_ACT_COST.ToString();
                tbCRDowntimeCostAvoid.Text = item.DWN_POT_COST.ToString();
                tbCRDowntimeNote.Text = item.DWN_COMMENTS;

                List<DEPARTMENT> deptList = SQMModelMgr.SelectDepartmentList(this.entities, (decimal)qiData.Incident.DETECT_COMPANY_ID, 0, (decimal)qiData.Incident.DETECT_PLANT_ID);
                ddlCRLaborDeptActual.DataSource = deptList;
                ddlCRLaborDeptActual.DataValueField = "DEPT_ID";
                ddlCRLaborDeptActual.DataTextField = "DEPT_NAME";
                ddlCRLaborDeptActual.DataBind();
                ddlCRLaborDeptActual.Items.Insert(0, new ListItem("", "0"));
                ddlCRLaborDeptAvoid.DataSource = deptList;
                ddlCRLaborDeptAvoid.DataValueField = "DEPT_ID";
                ddlCRLaborDeptAvoid.DataTextField = "DEPT_NAME";
                ddlCRLaborDeptAvoid.DataBind();
                ddlCRLaborDeptAvoid.Items.Insert(0, new ListItem("", "0"));

                List<LABOR_TYPE> laborList = SQMModelMgr.SelectLaborTypeList(this.entities, (decimal)qiData.Incident.DETECT_COMPANY_ID, 0, (decimal)qiData.Incident.DETECT_PLANT_ID);
                ddlCRLaborTypeActual.DataSource = laborList;
                ddlCRLaborTypeActual.DataValueField = "LABOR_TYP_ID";
                ddlCRLaborTypeActual.DataTextField = "LABOR_NAME";
                ddlCRLaborTypeActual.DataBind();
                ddlCRLaborTypeActual.Items.Insert(0, new ListItem("", "0"));
                ddlCRLaborTypeAvoid.DataSource = laborList;
                ddlCRLaborTypeAvoid.DataValueField = "LABOR_TYP_ID";
                ddlCRLaborTypeAvoid.DataTextField = "LABOR_NAME";
                ddlCRLaborTypeAvoid.DataBind();
                ddlCRLaborTypeAvoid.Items.Insert(0, new ListItem("", "0"));

                if (ddlCRLaborDeptActual.Items.FindByValue(item.LABOR_ACT_DEPT_ID.ToString()) != null)
                    ddlCRLaborDeptActual.SelectedValue = item.LABOR_ACT_DEPT_ID.ToString();
                if (ddlCRLaborTypeActual.Items.FindByValue(item.LABOR_ACT_TYPE_ID.ToString()) != null)
                    ddlCRLaborTypeActual.SelectedValue = item.LABOR_ACT_TYPE_ID.ToString();
                tbCRLaborRateActual.Text = item.LABOR_ACT_RATE.ToString();
                tbCRLaborRateAvoid.Text = item.LABOR_POT_RATE.ToString();
                tbCRLaborActual.Text = item.LABOR_ACT.ToString();
                if (ddlCRLaborDeptAvoid.Items.FindByValue(item.LABOR_POT_DEPT_ID.ToString()) != null)
                    ddlCRLaborDeptAvoid.SelectedValue = item.LABOR_POT_DEPT_ID.ToString();
                if (ddlCRLaborTypeAvoid.Items.FindByValue(item.LABOR_POT_TYPE_ID.ToString()) != null)
                    ddlCRLaborTypeAvoid.SelectedValue = item.LABOR_POT_TYPE_ID.ToString();
                tbCRLaborAvoid.Text = item.LABOR_POT.ToString();
                tbCRLaborCostActual.Text = item.LABOR_ACT_COST.ToString();
                tbCRLaborCostAvoid.Text = item.LABOR_POT_COST.ToString();
                tbCRLaborNote.Text = item.LABOR_COMMENTS;

                tbCRShipDescActual.Text = item.SHIP_ACT_TYPE.ToString();
                tbCRShipDescAvoid.Text = item.SHIP_POT_TYPE.ToString();
                tbCRShipCostActual.Text = item.SHIP_ACT_COST.ToString();
                tbCRShipCostAvoid.Text = item.SHIP_POT_COST.ToString();
                tbCRShipNote.Text = item.SHIP_COMMENTS;

                tbCRIncidentCostActual.Text = FormatValue((decimal)item.TOTAL_ACT_COST, 2);
                tbCRIncidentCostAvoid.Text = FormatValue((decimal)item.TOTAL_POT_COST, 2);
                tbCRIncidentCostActual.Enabled = tbCRIncidentCostAvoid.Enabled = false;
                btnCRCalculate.Enabled = true;
                tbCRTotalNote.Text = item.TOTAL_COMMENTS;

                // copy relevant incident data to the cost report when creating new
                if (costReport.IsNew  ||  item.EntityState == System.Data.EntityState.Added  ||  item.EntityState == System.Data.EntityState.Detached)
                {
                    QI_OCCUR_ITEM insp = qiData.QIIssue.QI_OCCUR_ITEM.FirstOrDefault();
                    tbCRItemQtyActual.Text = FormatValue((decimal)insp.TOTAL_NC_QTY, 2);
                    tbCRUnitCostActual.Text =  tbCRUnitCostAvoid.Text = "1.0";  // td
                    tbCRItemCostActual.Text = FormatValue((decimal)insp.TOTAL_NC_QTY, 2);
                    if (qiData.Incident.DETECT_PLANT_LINE_ID.HasValue && ddlCRPlantLineActual.Items.FindByValue(qiData.Incident.DETECT_PLANT_LINE_ID.ToString()) != null)
                    {
                        PLANT_LINE line = lineList.Where(l => l.PLANT_LINE_ID == qiData.Incident.DETECT_PLANT_LINE_ID).FirstOrDefault();
                        ddlCRPlantLineActual.SelectedValue = qiData.Incident.DETECT_PLANT_LINE_ID.ToString();
                        tbCRDowntimeRateActual.Text = FormatValue((decimal)line.DOWNTIME_RATE, 2);
                    }
                }
            }
            catch
            {
            }
        }

        protected void SaveReport()
        {
            costReport.CostReport.COST_REPORT_NAME = tbCRName.Text;
            costReport.CostReport.COST_REPORT_DESC = tbCRDesc.Text;
            costReport.CostReport.COST_REPORT_TYPE = ddlCRType.SelectedValue;

            IncidentCostReport retReport = null;
            if ((retReport = IncidentCostReport.UpdateCostReport(costReport)) != null)
            {
                costReport = retReport;
                MessageDisplay(IncidentCostReportStatus.Normal);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                DisplayReport();
            }
        }
    }
}