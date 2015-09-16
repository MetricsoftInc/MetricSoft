using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Telerik.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Ucl_QualityIssue : System.Web.UI.UserControl
    {
      
        // manage current session object  (formerly was page static variable)
        QualityIssueCtl IssueCtl()
        {
            if (SessionManager.CurrentIncident != null && SessionManager.CurrentIncident is QualityIssueCtl)
                return (QualityIssueCtl)SessionManager.CurrentIncident;
            else
                return null;
        }
        QualityIssueCtl SetIssueCtl(QualityIssueCtl issueCtl)
        {
            SessionManager.CurrentIncident = issueCtl;
            return IssueCtl();
        }

        public bool SendNotify
        {
            get { return cbNotify.Checked; }
        }
        public bool Create8D
        {
            get { return cb8DRequired.Checked; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclPartSearch1.OnSearchItemSelect += OnPartSelect;
        }

        public int NewIssue()
        {
            int status = 0;
            if (IssueCtl().PageMode != PageUseMode.EditEnabled)
            {
                divWorkArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            }

            SetB2BLocation((decimal)SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID);
           
            divWorkArea.Visible = true;
            SQMBasePage.ResetControlValues(divWorkArea.Controls);
            lblIssueDate_out.Text = WebSiteCommon.LocalTime(DateTime.Now, SessionManager.UserContext.TimeZoneID).ToString();
            lblOriginator_out.Text = SessionManager.UserContext.UserName();
            DisplayIssue();

            return status;
        }

        public void CancelIssue()
        {
            IssueCtl().Clear();
            SQMBasePage.ResetControlValues(divPageBody.Controls);
        }

        public void ToggleDisplay(bool enable)
        {
            pnlQualityIssue.Visible = enable;
        }

        public int BindIssue()
        {
            int status = 0;
            divWorkArea.Visible = divDisposition.Visible = true;

            if (IssueCtl().PageMode != PageUseMode.EditEnabled)
            {
                divWorkArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            }

            DisplayIssue();

            return status;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               // uclQISearch1.Load(false);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ;
        }

        protected void OnLocationChanged(object sender, EventArgs e)
        {
            SetB2BLocation(Convert.ToDecimal(ddlReportedLocation.SelectedValue));
            OnResponsibleLocationChanged(null, null);
        }

        protected void OnResponsibleLocationChanged(object sender, EventArgs e)
        {
            GetResponsibleList();
        }

        private void SetB2BLocation(decimal plantID)
        {
            IssueCtl().qualityIssue.SetDetectedLocation(plantID);
            SessionManager.SetB2BLocation(plantID);
            if (ddlCurrency.Items.FindItemByValue(IssueCtl().qualityIssue.DetectedLocation.Plant.CURRENCY_CODE) != null)
            {
                ddlCurrency.SelectedValue = IssueCtl().qualityIssue.DetectedLocation.Plant.CURRENCY_CODE;
                updCurrency.Update();
            }
            uclPartSearch1.Initialize("", false, false, false, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY);
        }

        private void GetResponsibleList()
        {
            ddlResponseTime.Items.AddRange(WebSiteCommon.PopulateComboBoxListNums(1, 14, ddlResponseTime.EmptyMessage));
            if (!string.IsNullOrEmpty(ddlResponsibleLocation.SelectedValue))
            {
                IssueCtl().qualityIssue.SetResponsibleLocation(Convert.ToDecimal(ddlResponsibleLocation.SelectedValue));
                List<BusinessLocation> locationList = new List<BusinessLocation>();
               // locationList.Add(qualityIssue.DetectedLocation);
                locationList.Add(IssueCtl().qualityIssue.ResponsibleLocation);
                // td: fetch the supplier persons responsible
                List<PERSON> respList = SQMModelMgr.SelectPlantPersonList(locationList, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY == "RCV" ? "211" : "212", AccessMode.Partner);
                SQMBasePage.SetPersonList(ddlResponsible, respList, "", 20);
                updResponsible.Update();
            }
        }

        private void OnPartSelect(string partID)
        {
            PART part = SQMModelMgr.LookupPart(new PSsqmEntities(), Convert.ToDecimal(partID), "", SessionManager.PrimaryCompany().COMPANY_ID, false);
            if (part != null)
            {
                IssueCtl().qualityIssue.IssueOccur.PART_ID = part.PART_ID;
                PartData partData = SQMModelMgr.LookupPartData(new PSsqmEntities(), SessionManager.PrimaryCompany().COMPANY_ID, part.PART_ID);
                partData.Locations();
                IssueCtl().qualityIssue.AddPartInfo(partData);
                lblPartDesc.Text = part.PART_NAME;
                ddlResponsibleLocation.Items.Clear();
                ddlResponsibleLocation.Items.Add(new RadComboBoxItem(IssueCtl().qualityIssue.DetectedLocation.Company.COMPANY_NAME + ", " + IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_NAME, IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID.ToString()));
                if (IssueCtl().qualityIssue.Partdata.B2BList != null)
                {
                    foreach (BusinessLocation location in IssueCtl().qualityIssue.Partdata.B2BList)
                    {
                        if (ddlResponsibleLocation.Items.FindItemByValue(location.Plant.PLANT_ID.ToString()) == null)
                            ddlResponsibleLocation.Items.Add(new RadComboBoxItem(location.Company.COMPANY_NAME + ", " + location.Plant.PLANT_NAME, location.Plant.PLANT_ID.ToString()));
                    }
                    if (IssueCtl().qualityIssue.Incident.RESP_PLANT_ID.HasValue)
                        ddlResponsibleLocation.SelectedValue = IssueCtl().qualityIssue.Incident.RESP_PLANT_ID.ToString();
                    else if (ddlResponsibleLocation.Items.Count > 0)
                        ddlResponsibleLocation.SelectedIndex = 0;

                    GetResponsibleList();
                }
                updResponsible.Update();
            }
        }

        protected void btnDupIncident_Click(object sender, EventArgs e)
        {
            QualityIssue QIRef = new QualityIssue().CreateNew("", ddlIncidentType.SelectedValue);
            QIRef.IssueOccur.PART_TYPE = ddlPartType.SelectedValue;
            try
            {
                QIRef.IssueOccur.PROBLEM_AREA = uclNC.ProblemArea;
                QIRef.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = Convert.ToDecimal(uclNC.NCDefect);
            }
            catch
            {
                QIRef.IssueOccur.PROBLEM_AREA = "";
                QIRef.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = 0;
            }

           // uclQISearch1.Initialize("", IssueCtl().qualityIssue, IssueCtl().qualityIssue.Partdata.B2BList, DateTime.Now.AddYears(-2), DateTime.Now, false, IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS);
           // uclQISearch1.QITextBox.Focus();
           // udpDup.Update();
        }

        protected void SelectActivityType(object sender, EventArgs e)
        {
            RadComboBoxItem item = null;

            switch (ddlIncidentType.SelectedValue)
            {
                case "CST":
                    ddlReportedLocation.Items.Clear();
                    ddlReportedLocation.Items.Add(new RadComboBoxItem(SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME, SessionManager.UserContext.HRLocation.Plant.PLANT_ID.ToString()));
                    List<BusinessLocation> custLocations = SQMModelMgr.UserAccessibleLocations(SessionManager.UserContext.Person, SQMModelMgr.SelectBusinessLocationList(0, 0, false, true, true), false, true, ddlIncidentType.SelectedValue);
                    foreach (BusinessLocation loc in custLocations)
                    {
                        ddlReportedLocation.Items.Add(SQMBasePage.SetLocationItem(loc, true));
                    }
                    break;
                case "RCV":
                    ddlReportedLocation.Items.Clear();
                    List<BusinessLocation> locationList = UserContext.FilterPlantAccessList(SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true));
                    SQMBasePage.SetLocationList(ddlReportedLocation, locationList, 0);
                    break;
                default:
                    ddlReportedLocation.Items.Clear();
                    ddlReportedLocation.Items.Add(new RadComboBoxItem(SessionManager.UserContext.WorkingLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.WorkingLocation.Plant.PLANT_NAME, SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID.ToString()));
                    if (SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID != SessionManager.UserContext.HRLocation.Plant.PLANT_ID)
                        ddlReportedLocation.Items.Add(new RadComboBoxItem(SessionManager.UserContext.HRLocation.Company.COMPANY_NAME + ", " + SessionManager.UserContext.HRLocation.Plant.PLANT_NAME, SessionManager.UserContext.HRLocation.Plant.PLANT_ID.ToString()));
                    break;
            }
        }

        private void SetupPage()
        {
            if (ddlIncidentSeverity2.Items.Count == 0)
            {
                ddlIncidentSeverity2.Items.AddRange(WebSiteCommon.PopulateRadListItems("incidentSeverity"));
                ddlIncidentSeverity2.Items.Insert(0, new RadComboBoxItem("", ""));
                ddlIncidentSeverity2.SelectedIndex = 0;

                ddlDisposition.Items.AddRange(WebSiteCommon.PopulateRadListItems("NCDisposition"));
                ddlDisposition.Items.Insert(0, new RadComboBoxItem("", ""));
                ddlDisposition.SelectedIndex = 0;

                ddlStatus.Items.AddRange(WebSiteCommon.PopulateRadListItems("recordStatus"));
              
                SQMBasePage.FillCurrencyDDL(ddlCurrency, "EUR", false);
                ddlCurrency.Items.Insert(0, new RadComboBoxItem("", ""));

                List<PART_ATTRIBUTE> attributeList = SQMModelMgr.SelectPartAttributeList("TYPE", "", false);
                ddlPartType.Items.Add(new RadComboBoxItem("", ""));
                foreach (PART_ATTRIBUTE pat in attributeList)
                {
                    ddlPartType.Items.Add(new RadComboBoxItem(pat.ATTRIBUTE_VALUE, pat.ATTRIBUTE_CD));
                }
            }

            radIssueDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            radIssueDate.MinDate = new DateTime(2001, 1, 1);
            radIssueDate.MaxDate = DateTime.UtcNow.AddDays(7);
            radIssueDate.SelectedDate = WebSiteCommon.LocalTime(DateTime.UtcNow, SessionManager.UserContext.TimeZoneID);
            radIssueDate.ShowPopupOnFocus = true;

            uclPartSearch1.Initialize("", false, false, false, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY);
        }

        protected void DisplayIssue()
        {
            SetupPage();

            if (ddlIncidentType.Items.FindItemByValue(IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY) != null)
            {
                SQMBasePage.DisplayControlValue(ddlIncidentType, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY, PageUseMode.ViewOnly, "");
                SelectActivityType(null, null);
                trQIActivity.Visible = false;

                switch (IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY)
                {
                    case "CST":
                        trPartType.Visible = true;
                        ph8DRef.Visible = true;     // external problem control system reference number
                        trReceipt.Visible = false;
                        break;
                    case "RCV":
                        trPartType.Visible = false;
                        ph8DRef.Visible = false;
                        trReceipt.Visible = true;  // reference receipt or po number
                        break;
                    default:
                        trPartType.Visible = false; 
                        ph8DRef.Visible = false;
                        trReceipt.Visible = false;
                        break;
                }
            }

            tbTotalEstNCQty.ReadOnly = true;

            QI_OCCUR_NC sample = IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.FirstOrDefault();
            if (sample != null)
            {
                if (IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID.HasValue)
                    uclNC.Initialize(IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_AREA,
                    (decimal)IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID,
                    (int)IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT, IssueCtl().PageMode);
                else
                    uclNC.Initialize("", 0, 0, IssueCtl().PageMode);

                gvMeasureGrid.DataSource = sample.QI_OCCUR_MEASURE;
                gvMeasureGrid.DataBind();
            }

            SetB2BLocation(Convert.ToDecimal(IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID.ToString()));
            if (ddlReportedLocation.Items.FindItemByValue(IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID.ToString()) != null)
                SQMBasePage.DisplayControlValue(ddlReportedLocation, IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID.ToString(), IssueCtl().PageMode, "textStd");
            else
            {
                // add the reported location in case the viewing user doesn't have it assigned to him
                BusinessLocation reportLoc = new BusinessLocation().Initialize(IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID);
                if (reportLoc != null)
                {
                    ddlReportedLocation.Items.Add(SQMBasePage.SetLocationItem(reportLoc, true));
                    SQMBasePage.DisplayControlValue(ddlReportedLocation, IssueCtl().qualityIssue.DetectedLocation.Plant.PLANT_ID.ToString(), IssueCtl().PageMode, "textStd");
                }
            }
              
            ddlResponsibleLocation.Items.Clear();
            if (IssueCtl().qualityIssue.Partdata.B2BList != null)
            {
                foreach (BusinessLocation loc in IssueCtl().qualityIssue.Partdata.B2BList)
                {
                    ddlResponsibleLocation.Items.Add(SQMBasePage.SetLocationItem(loc, true));
                }
              //  ddlResponsibleLocation.SelectedValue = qualityIssue.Incident.RESP_PLANT_ID.ToString();
                SQMBasePage.DisplayControlValue(ddlResponsibleLocation, IssueCtl().qualityIssue.Incident.RESP_PLANT_ID.ToString(), IssueCtl().PageMode, "textStd");
            }

            if (IssueCtl().qualityIssue.Partdata != null && IssueCtl().qualityIssue.Partdata.Part != null)
            {
                SQMBasePage.DisplayControlValue(uclPartSearch1.PartTextBox, IssueCtl().qualityIssue.Partdata.PartDisplayNum(IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY), IssueCtl().PageMode, "textStd");
                lblPartDesc.Text = IssueCtl().qualityIssue.Partdata.Part.PART_NAME;
                SQMBasePage.DisplayControlValue(tbRelatedParts, IssueCtl().qualityIssue.IssueOccur.RELATED_PARTS, IssueCtl().PageMode, "textStd");
            }

            ddlDisposition.SelectedIndex = ddlStatus.SelectedIndex = 0;

            if (IssueCtl().qualityIssue.IsNew)
            {
                cbNotify.Checked = true;
                cbNotify.Enabled = false;
                ddlResponseTime.SelectedIndex = 1;
            }
            else
            {
                cbNotify.Checked = false;
                cbNotify.Enabled = true;

                lblIssueDate_out.Text = SQMBasePage.FormatDate(WebSiteCommon.LocalTime((DateTime)IssueCtl().qualityIssue.Incident.CREATE_DT, SessionManager.UserContext.TimeZoneID), "d", false);

                SQMBasePage.DisplayControlValue(radIssueDate, IssueCtl().qualityIssue.Incident.INCIDENT_DT.ToShortDateString(), IssueCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbIssueDesc, IssueCtl().qualityIssue.Incident.DESCRIPTION, IssueCtl().PageMode, "textStd");

                lblOriginator_out.Text = IssueCtl().qualityIssue.Incident.CREATE_BY;
                SQMBasePage.DisplayControlValue(ddlIncidentSeverity2, IssueCtl().qualityIssue.IssueOccur.SEVERITY, IssueCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbReceipt, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY == "RCV" ? IssueCtl().qualityIssue.IssueOccur.REF_OPERATION : "", IssueCtl().PageMode, "textStd");

                if (ddlPartType.Items.FindItemByValue(IssueCtl().qualityIssue.IssueOccur.PART_TYPE) != null)
                    SQMBasePage.DisplayControlValue(ddlPartType, IssueCtl().qualityIssue.IssueOccur.PART_TYPE, IssueCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbNCLotNum, IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().LOT_NUM, IssueCtl().PageMode, "textStd");

                try
                {
                    SQMBasePage.DisplayControlValue(tbNCTotalQty, IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY.ToString(), IssueCtl().PageMode, "textStd");
                    SQMBasePage.DisplayControlValue(tbNCSampleQty, IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY.ToString(), IssueCtl().PageMode, "textStd");
                    SQMBasePage.DisplayControlValue(tbNCNonConformQty, IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY.ToString(), IssueCtl().PageMode, "textStd");
                    SQMBasePage.DisplayControlValue(tbTotalEstNCQty, Math.Round((decimal)IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_NC_QTY, 1).ToString(), IssueCtl().PageMode, "labelEmphasis");
                    lnkCalculateNC.Visible = IssueCtl().PageMode == PageUseMode.EditEnabled ? true : false;
                    lblCalculateNC.Visible = IssueCtl().PageMode == PageUseMode.EditEnabled ? false : true;
                }
                catch
                {
                    tbNCTotalQty.Text = tbNCSampleQty.Text = tbNCNonConformQty.Text = tbTotalEstNCQty.Text = "";
                }

                SQMBasePage.DisplayControlValue(tbObservations, IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().SAMPLE_COMMENTS, IssueCtl().PageMode, "textStd");

                cb8DRequired.Checked = (bool)(IssueCtl().qualityIssue.IssueOccur.PROBCASE_REQD.HasValue == true ? IssueCtl().qualityIssue.IssueOccur.PROBCASE_REQD : false);
                if (IssueCtl().PageMode != PageUseMode.EditEnabled)
                    cb8DRequired.Enabled = phNotify.Visible = false;

                SQMBasePage.DisplayControlValue(tb8DRef, IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY == "CST" && !string.IsNullOrEmpty(IssueCtl().qualityIssue.IssueOccur.REF_OPERATION) ? IssueCtl().qualityIssue.IssueOccur.REF_OPERATION : "", IssueCtl().PageMode, "refText");

                SQMBasePage.DisplayControlValue(ddlDisposition, IssueCtl().qualityIssue.IssueOccur.DISPOSITION, IssueCtl().PageMode, "textStd");

                if (IssueCtl().PageMode == PageUseMode.Active)
                {
                    if (ddlStatus.Items.FindItemByValue(IssueCtl().qualityIssue.IssueOccur.STATUS) != null)
                        ddlStatus.SelectedValue = IssueCtl().qualityIssue.IssueOccur.STATUS;
                }
                else
                    SQMBasePage.DisplayControlValue(ddlStatus, IssueCtl().qualityIssue.IssueOccur.STATUS, IssueCtl().PageMode, "textStd");

                if (ddlCurrency.Items.FindItemByValue(IssueCtl().qualityIssue.IssueOccur.CURRENCY_CODE) != null)
                    SQMBasePage.DisplayControlValue(ddlCurrency, IssueCtl().qualityIssue.IssueOccur.CURRENCY_CODE, IssueCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbActCost, IssueCtl().qualityIssue.IssueOccur.EST_ACT_COST.HasValue ? SQMBasePage.FormatValue((decimal)IssueCtl().qualityIssue.IssueOccur.EST_ACT_COST, 2) : "", IssueCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbActCostNote, IssueCtl().qualityIssue.IssueOccur.ACT_COST_COMMENT, IssueCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbPotCost, IssueCtl().qualityIssue.IssueOccur.EST_POT_COST.HasValue ? SQMBasePage.FormatValue((decimal)IssueCtl().qualityIssue.IssueOccur.EST_POT_COST, 2) : "", IssueCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbPotCostNote, IssueCtl().qualityIssue.IssueOccur.POT_COST_COMMENT, IssueCtl().PageMode, "textStd");

                    if (IssueCtl().PageMode == PageUseMode.EditEnabled)
                {
                    uclRadAttach.SetReportOption(false);
                    uclRadAttach.GetUploadedFiles(20, IssueCtl().qualityIssue.Incident.INCIDENT_ID, "1");
                    
                }
                else
                {
                    Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
                    uclRadAttach.Parent.Controls.AddAt(uclRadAttach.Parent.Controls.IndexOf(uclRadAttach), attch);
                    attch.BindDisplayAttachments(20, IssueCtl().qualityIssue.Incident.INCIDENT_ID, "1", 0);
                    uclRadAttach.Visible = false;
                }

                btnDupIncident.Visible = IssueCtl().PageMode == PageUseMode.EditEnabled ? true : false;

                //uclQISearch1.Load(false);
                if (!string.IsNullOrEmpty(IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS))
                {
                    QualityIssue refIssue = new QualityIssue().Load(Convert.ToDecimal(IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS));
                    if (refIssue != null)
                        tbDupIssue.Text = IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS + " - " + refIssue.Incident.DESCRIPTION;
                }
                else
                    tbDupIssue.Text = "";

                // determine selectable list of responsible persons based on the trading partner locations
                GetResponsibleList();

                if (ddlResponseTime.Items.FindItemByValue(IssueCtl().qualityIssue.IssueOccur.INIT_ACTION) != null)
                    SQMBasePage.DisplayControlValue(ddlResponseTime, IssueCtl().qualityIssue.IssueOccur.INIT_ACTION, IssueCtl().PageMode, "textStd");

                // update the ddl based per selected persons in the task list
                foreach (TASK_STATUS task in IssueCtl().qualityIssue.TeamTask.TaskList)
                {
                    RadComboBoxItem resp = null;
                    if ((resp = ddlResponsible.FindItemByValue(task.RESPONSIBLE_ID.ToString())) != null)
                    {
                        resp.Checked = true;
                    }
                }
                if (IssueCtl().PageMode != PageUseMode.EditEnabled)
                    SQMBasePage.DisplayControlValue(ddlResponsible, "", IssueCtl().PageMode, "textSTd");

              //  btnPrintLabel.OnClientClick = "Popup('../Quality/QualityIssue_Label.aspx?issue=" + qualityIssue.IssueOccur.INCIDENT_ID.ToString() + "', 'newPage', 600, 450); return false;";
            }

            uclResponse.BindResponseList(IssueCtl().qualityIssue.TeamResponse.ResponseList, IssueCtl().PageMode == PageUseMode.EditEnabled || IssueCtl().PageMode == PageUseMode.EditPartial ? true : false, true);
            lblIssueResponseCount.Text = IssueCtl().qualityIssue.TeamResponse.ResponseList.Count.ToString();
            phResponseAlert.Visible = IssueCtl().PageMode == PageUseMode.EditEnabled ? true : false;
        }

        public void gvMeasure_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbMeasureName");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, IssueCtl().PageMode, "textStd");
                    tb = (TextBox)e.Row.Cells[0].FindControl("tbMeasureValue");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, IssueCtl().PageMode, "textStd");
                }
                catch
                {
                }
            }
        }

        public QualityIssue UpdateIssue(QualityIssue theIssue)
        {
            bool success;
            decimal decVal;
            RESPONSE response = null;
            theIssue.Status = QualityIssue.UpdateStatus.Pending;


            if (IssueCtl().PageMode == PageUseMode.Active)
            {
                // re-activate issue
                IssueCtl().qualityIssue.IssueOccur.STATUS = ddlStatus.SelectedValue;
                IssueCtl().qualityIssue.SetTasksOpen("C", DateTime.UtcNow.AddDays(14));
                IssueCtl().qualityIssue.StatusChanged = true;
                theIssue.Status = QualityIssue.UpdateStatus.Success;

                return (theIssue = IssueCtl().qualityIssue);
            }

            // trading partners or personse responsible can only add to the response list
            if (IssueCtl().PageMode == PageUseMode.EditPartial)
            {
                if (!string.IsNullOrEmpty(uclResponse.ResponseInput.Trim()))
                {
                    IssueCtl().qualityIssue.TeamResponse.CreateResponse(20, IssueCtl().qualityIssue.IssueOccur.QIO_ID, "", SessionManager.UserContext.Person.PERSON_ID, uclResponse.ResponseInput, SessionManager.UserContext.HRLocation.Company.COMPANY_NAME);
                    IssueCtl().qualityIssue.SetTasksStatus("R", TaskStatus.AwaitingClosure, SessionManager.UserContext.Person.PERSON_ID);
                    theIssue.Status = QualityIssue.UpdateStatus.Success;
                    return (theIssue = IssueCtl().qualityIssue);
                }
            }
        
            if (radIssueDate.SelectedDate.HasValue == false || 
                string.IsNullOrEmpty(ddlIncidentSeverity2.SelectedValue)  ||
                IssueCtl().qualityIssue.IssueOccur.PART_ID == null ||
                IssueCtl().qualityIssue.IssueOccur.PART_ID < 1 || 
                (theIssue.IssueOccur.QS_ACTIVITY == "CST" && string.IsNullOrEmpty(ddlPartType.SelectedValue)  || 
                string.IsNullOrEmpty(tbNCTotalQty.Text)  || 
                string.IsNullOrEmpty(tbNCSampleQty.Text)  ||
                string.IsNullOrEmpty(tbNCNonConformQty.Text) || 
                string.IsNullOrEmpty(ddlResponsibleLocation.SelectedValue)))
            {
                theIssue.Status = QualityIssue.UpdateStatus.RequiredInputs;
                return (theIssue);
            }

            if ((!string.IsNullOrEmpty(tbActCost.Text)  ||  !string.IsNullOrEmpty(tbPotCost.Text))  &&  string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                theIssue.Status = QualityIssue.UpdateStatus.Incomplete;
                return (theIssue);
            }

            // update the INCIDENT record
            IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY = ddlIncidentType.SelectedValue;
            IssueCtl().qualityIssue.Incident.INCIDENT_DT = (DateTime)radIssueDate.SelectedDate;
            IssueCtl().qualityIssue.Incident.DESCRIPTION = tbIssueDesc.Text.Trim();

            IssueCtl().qualityIssue.SetDetectedLocation(Convert.ToDecimal(ddlReportedLocation.SelectedValue));
            IssueCtl().qualityIssue.SetResponsibleLocation(Convert.ToDecimal(ddlResponsibleLocation.SelectedValue));

            IssueCtl().qualityIssue.IssueOccur.RELATED_PARTS = tbRelatedParts.Text;

            if (!string.IsNullOrEmpty(ddlPartType.SelectedValue))
                IssueCtl().qualityIssue.IssueOccur.PART_TYPE = ddlPartType.SelectedValue;
            else
                IssueCtl().qualityIssue.IssueOccur.PART_TYPE = null;

            IssueCtl().qualityIssue.IssueOccur.SEVERITY = ddlIncidentSeverity2.SelectedValue;
            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().LOT_NUM = tbNCLotNum.Text;

            if (decimal.TryParse(tbNCTotalQty.Text.Trim(), out decVal))
                IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY = decVal;
            if (decimal.TryParse(tbNCSampleQty.Text.Trim(), out decVal))
                IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY = decVal;
            if (decimal.TryParse(tbNCNonConformQty.Text.Trim(), out decVal))
                IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY = decVal;

            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_NC_QTY = (IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY / IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY) * IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY;

            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_AREA = "";
            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = null;
            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT = 0;
            if (!string.IsNullOrEmpty(uclNC.ProblemArea))
            {
                IssueCtl().qualityIssue.IssueOccur.OCCUR_DESC = uclNC.ProblemAreaDesc;
                IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_AREA = IssueCtl().qualityIssue.IssueOccur.PROBLEM_AREA = uclNC.ProblemArea;
                if (!string.IsNullOrEmpty(uclNC.NCDefect))
                {
                    IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = Convert.ToDecimal(uclNC.NCDefect);
                    int NCCount = 0;
                    if (int.TryParse(uclNC.NCCount, out NCCount) == true)
                        IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT = NCCount;
                    else
                        IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT = 1;
                }
            }

            foreach (GridViewRow row in gvMeasureGrid.Rows)
            {
                try
                {
                    QI_OCCUR_NC sample = IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.FirstOrDefault();
                    QI_OCCUR_MEASURE measure = sample.QI_OCCUR_MEASURE.FirstOrDefault();
                    if (sample != null && measure != null)
                    {
                        measure.QIO_SAMPLE_ID = sample.QIO_SAMPLE_ID;
                        measure.MEASURE_NUM = 1;
                        measure.MEASURE_NAME = ((TextBox)row.Cells[0].FindControl("tbMeasureName")).Text;
                        TextBox tb = (TextBox)row.Cells[0].FindControl("tbMeasureValue");
                        if (!string.IsNullOrEmpty(tb.Text))
                        {
                            measure.MEASURE_IND = true;
                            measure.MEASURE_VALUE = Convert.ToDecimal(tb.Text);
                        }
                    }
                }
                catch { }
            }

            IssueCtl().qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().SAMPLE_COMMENTS = tbObservations.Text.Trim();

            IssueCtl().qualityIssue.IssueOccur.DISPOSITION = ddlDisposition.SelectedValue;
            IssueCtl().qualityIssue.IssueOccur.PROBCASE_REQD = cb8DRequired.Checked;
            IssueCtl().qualityIssue.IssueOccur.REF_OPERATION = tb8DRef.Text;

            if (IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY == "CST")
                IssueCtl().qualityIssue.IssueOccur.REF_OPERATION = tb8DRef.Text;
            else if (IssueCtl().qualityIssue.IssueOccur.QS_ACTIVITY == "RCV")
                IssueCtl().qualityIssue.IssueOccur.REF_OPERATION = tbReceipt.Text;
            else
                IssueCtl().qualityIssue.IssueOccur.REF_OPERATION = "";
            
            decimal value = 0;           
            if (!string.IsNullOrEmpty(tbActCost.Text) && decimal.TryParse(tbActCost.Text, out value))
            {
                IssueCtl().qualityIssue.IssueOccur.EST_ACT_COST = value;
                IssueCtl().qualityIssue.IssueOccur.ACT_COST_COMMENT = tbActCostNote.Text;
            }
            else
            {
                IssueCtl().qualityIssue.IssueOccur.EST_ACT_COST = null;
                IssueCtl().qualityIssue.IssueOccur.ACT_COST_COMMENT = "";
            }

            IssueCtl().qualityIssue.IssueOccur.CURRENCY_CODE = ddlCurrency.SelectedValue;

            if (!string.IsNullOrEmpty(tbPotCost.Text) && decimal.TryParse(tbPotCost.Text, out value))
            {
                IssueCtl().qualityIssue.IssueOccur.EST_POT_COST = value;
                IssueCtl().qualityIssue.IssueOccur.POT_COST_COMMENT = tbPotCostNote.Text;
            }
            else
            {
                IssueCtl().qualityIssue.IssueOccur.EST_POT_COST = null;
                IssueCtl().qualityIssue.IssueOccur.POT_COST_COMMENT = "";
            }

            IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS = tbDupIssue.Text;
            /*
            string[] args = uclQISearch1.Text.Split(' ');
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]) && args[0] != WebSiteCommon.FormatID(IssueCtl().qualityIssue.Incident.INCIDENT_ID, 6))
                IssueCtl().qualityIssue.IssueOccur.RELATED_INCIDENTS = args[0]; 
            */
            // create person response tasks if not currently existing
            IssueCtl().qualityIssue.IssueOccur.INIT_ACTION = null;
            if (ddlResponsible.CheckedItems.Count > 0  &&  !string.IsNullOrEmpty(ddlResponseTime.SelectedValue))
                IssueCtl().qualityIssue.IssueOccur.INIT_ACTION = ddlResponseTime.SelectedValue;

            IssueCtl().qualityIssue.UpdateTasks(new decimal[1] { (decimal)IssueCtl().qualityIssue.Incident.CREATE_PERSON }, "C", 14, false);

            IssueCtl().qualityIssue.ResponseRequired = cbResponseAlert.Checked;

            IssueCtl().qualityIssue.UpdateTasks(Array.ConvertAll(ddlResponsible.Items.Where(l => l.Checked).Select(l => l.Value).ToArray(), new Converter<string, decimal>(decimal.Parse)),
                "R", Convert.ToInt32(IssueCtl().qualityIssue.IssueOccur.INIT_ACTION), IssueCtl().qualityIssue.ResponseRequired);

            if (!string.IsNullOrEmpty(uclResponse.ResponseInput.Trim()))
                IssueCtl().qualityIssue.TeamResponse.CreateResponse(20, IssueCtl().qualityIssue.IssueOccur.QIO_ID, "", SessionManager.UserContext.Person.PERSON_ID, uclResponse.ResponseInput, SessionManager.UserContext.HRLocation.Company.COMPANY_NAME);
           
            // complete all tasks when the issue is closed
            if (IssueCtl().qualityIssue.IssueOccur.STATUS != ddlStatus.SelectedValue && ddlStatus.SelectedValue == "C")
            {
                IssueCtl().qualityIssue.SetTasksStatus("C", TaskStatus.Complete, SessionManager.UserContext.Person.PERSON_ID);
                IssueCtl().qualityIssue.SetTasksStatus("R", TaskStatus.Complete, SessionManager.UserContext.Person.PERSON_ID);
                IssueCtl().qualityIssue.StatusChanged = true;
            }

            IssueCtl().qualityIssue.IssueOccur.STATUS = ddlStatus.SelectedValue;

            if (cbNotify.Checked)  // td: to test mail functions
            {
                IssueCtl().qualityIssue.ResetIndicators(true);
                IssueCtl().qualityIssue.StatusChanged = false;
            }

            //btnPrintLabel.OnClientClick = "Popup('../Quality/QualityIssue_Label.aspx?issue=" + qualityIssue.IssueOccur.QIO_ID.ToString() + "', 'newPage', 600, 450); return false;";

            IssueCtl().qualityIssue.Status = QualityIssue.UpdateStatus.Success;
            return (theIssue = IssueCtl().qualityIssue);
        }

        public int SaveAttachments(QualityIssue theIssue)
        {
            // save attachments
            int status = 0;

            if (uclRadAttach.RAUpload.UploadedFiles.Count > 0)
            {
                SessionManager.DocumentContext =
                    new SQM.Shared.DocumentScope().CreateNew(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
                                                    SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0);
                SessionManager.DocumentContext.RecordType = 20;
                SessionManager.DocumentContext.RecordID = theIssue.Incident.INCIDENT_ID;
                SessionManager.DocumentContext.RecordStep = "1";

                uclRadAttach.SaveFiles();
                theIssue.LoadAttachments();
            }

            uclResponse.SaveAttachments(theIssue.TeamResponse.ResponseList.LastOrDefault());

            return status;
        }

 
    }
}