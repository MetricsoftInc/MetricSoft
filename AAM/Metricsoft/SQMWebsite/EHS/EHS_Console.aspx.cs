using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class EHS_Console : SQMBasePage
    {
        SQMMetricMgr HSCalcs;

        #region events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			this.lblExportPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblExportToDate.Text = Resources.LocalizedText.To + ": ";

			if (!Page.IsPostBack)
            {
                SetupPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("EHS", 2, true, true, "");
                }
            }
        }

        protected void ddlReportList_Select(object sender, EventArgs e)
        {
            phBasicCriteria.Visible = true;
            radExportDateSelect2.Enabled = true;
            divGHGReport.Visible = divProfilePeriodScrollRepeater.Visible = divMetricHistory.Visible = divMetricHistory.Visible = phIncidentCriteria.Visible = divMetricsTimespan.Visible = false;

            radExportDateSelect1.MinDate = new DateTime(2001, 1, 1);
            radExportDateSelect1.MaxDate = DateTime.Now.AddMonths(1);
            radExportDateSelect1.SelectedDate = radExportDateSelect2.SelectedDate = DateTime.Now;
            radExportDateSelect1.ShowPopupOnFocus = radExportDateSelect2.ShowPopupOnFocus = true;

            ddlExportPlantSelect.Items.Clear();
            switch (ddlReportList.SelectedValue)
            {
                case "1":
                    SQMBasePage.SetLocationList(ddlExportPlantSelect, LocationList().Where(l => l.Plant.TRACK_EW_DATA == true || l.Plant.TRACK_FIN_DATA == true).ToList(), 0);
                    break;
                case "2":  // incident export
                    SQMBasePage.SetLocationList(ddlExportPlantSelect, LocationList(), 0);
                    phIncidentCriteria.Visible = true;
                    List<INCIDENT_TYPE> typeList = EHSIncidentMgr.SelectIncidentTypeList(SessionManager.PrimaryCompany().COMPANY_ID);
					ddlExportIncidentType.Items.Clear();
                    foreach (INCIDENT_TYPE intype in typeList)
                    {
                        RadComboBoxItem item = new RadComboBoxItem(intype.TITLE, intype.INCIDENT_TYPE_ID.ToString());
                        item.Checked = true;
						ddlExportIncidentType.Items.Add(item);
                    }
                    break;
                case "3":  // metric status
                     SQMBasePage.SetLocationList(ddlExportPlantSelect, LocationList().Where(l => l.Plant.TRACK_EW_DATA == true || l.Plant.TRACK_FIN_DATA == true).ToList(), 0);
                     radExportDateSelect2.Enabled = false;
                    break;
                case "4":  // dashboard eff date
                    phBasicCriteria.Visible = false;
                    divMetricsTimespan.Visible = true;
                    lblTimespanDateError.Visible = false;
                    COMPANY company = SQMModelMgr.LookupCompany(SessionManager.UserContext.HRLocation.Company.COMPANY_ID);
                    if (company.COMPANY_ACTIVITY != null)
                    {
                        try
                        {
                            if (company.COMPANY_ACTIVITY.EFF_EHS_METRIC_FROM_DT.HasValue)
                                radEffFrom.SelectedDate = (DateTime)company.COMPANY_ACTIVITY.EFF_EHS_METRIC_FROM_DT;
                            if (company.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT.HasValue)
                                radEffTo.SelectedDate = (DateTime)company.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT;
                        }
                        catch 
                        {
                            radEffFrom.SelectedDate = DateTime.Now.AddMonths(-1);
                            radEffTo.SelectedDate = DateTime.Now;
                        }
                    }
                    break;
                case "11": // CO2 reports
                    SQMBasePage.SetLocationList(ddlExportPlantSelect, LocationList().Where(l => l.Plant.TRACK_EW_DATA == true || l.Plant.TRACK_FIN_DATA == true).ToList(), 0);
                    break;
                default:
                    phBasicCriteria.Visible = phIncidentCriteria.Visible = false;
                    break;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            switch (ddlReportList.SelectedValue)
            {
                case "2":
                    LoadIncidentExportInput();
                    break;
                case "3":
                    divProfilePeriodScrollRepeater.Visible = true;
                    LoadProfileInput();
                    break;
                case "4":
                    break;
                case "11":
                    divGHGReport.Visible = true;
                    GenerateEmissionsReport();
                    break;
				default:  // metric export
                    LoadExportInput();
                    break;
            }
        }

        #endregion

        #region incidentaging
        protected void ddlStatusSelectChange(object sender, EventArgs e)
        {
            ;
        }

		protected void btnIncidentsExportClick(object sender, EventArgs e)
		{
			LoadIncidentExportInput();
		}

		#endregion

        #region inputstatus
        private void MessageDisplay(EHSProfileStatus status)
        {
 
        }

        public void LoadProfileInput()
        {
            divProfilePeriodScrollRepeater.Visible = true;
            SetLocalProfileList(new List<EHSProfile>());

            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlExportPlantSelect))
            {
                if (radExportDateSelect1.SelectedDate != null)
                {
                    EHSProfile profile = new EHSProfile().Load(Convert.ToDecimal(item.Value), false, true);
                    if (profile.Profile != null)
                    {
                        profile.LoadPeriod((DateTime)radExportDateSelect1.SelectedDate);
                        profile.MapPlantAccountingInputs(true, true);
                        LocalProfileList().Add(profile);
                    }
                }
            }
            rptProfile.DataSource = LocalProfileList();
            rptProfile.DataBind();

            if (UserContext.GetMaxScopePrivilege(SysScope.envdata) <= SysPriv.config)
                btnRollupAll.Visible = true;
        }

		public void LoadExportInput()
		{
			string searchcriteria = "";
			string plantList = "";
			DateTime dtFrom = DateTime.Today;
			DateTime dtTo = DateTime.Today;

            uclProgress.BindProgressDisplay(100, "Exporting: ");
            uclProgress.UpdateDisplay(1, 10, "Exporting...");

            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlExportPlantSelect))
			{
                if (radExportDateSelect1.SelectedDate != null)
				{
					if (plantList.Length > 0)
						plantList += ", ";
					plantList += item.Value;
				}
			}
			try
			{
                dtFrom = radExportDateSelect1.SelectedDate.Value;
                dtTo = radExportDateSelect2.SelectedDate.Value;
			}
			catch { }
			
			GenerateExportHistoryExcel(plantList, dtFrom, dtTo);
            uclProgress.ProgressComplete();
		}

		public void LoadIncidentExportInput()
		{
			string searchcriteria = "";
			string plantList = "";
			DateTime dtFrom = DateTime.Today;
			DateTime dtTo = DateTime.Today;

			uclProgress.BindProgressDisplay(100, "Exporting: ");
            uclProgress.UpdateDisplay(1, 10, "Exporting...");

			try
			{
				dtFrom = new DateTime(radExportDateSelect1.SelectedDate.Value.Year, radExportDateSelect1.SelectedDate.Value.Month, 1);
			}
			catch { dtFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); }
			try
			{
				dtTo = new DateTime(radExportDateSelect2.SelectedDate.Value.Year, radExportDateSelect2.SelectedDate.Value.Month, 1).AddMonths(1).AddDays(-1);
			}
			catch { dtTo = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1); }

            List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlExportPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();
			List<decimal> typeList = ddlExportIncidentType.Items.Where(i => i.Checked == true).Select(i => Convert.ToDecimal(i.Value)).ToList();

			HSCalcs = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", dtFrom, dtTo, new decimal[0]);
			HSCalcs.ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
            HSCalcs.ehsCtl.SelectIncidentList(plantIDS, typeList, dtFrom, dtTo, ddlExportStatusSelect.SelectedValue, false, 0);
		    GenerateIncidentExportExcel(HSCalcs.ehsCtl.IncidentHst);
            uclProgress.ProgressComplete();
		}

		public void LoadMetricHistory()
        {
            SetLocalProfileList(new List<EHSProfile>());

            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlExportPlantSelect))
            {
                if (radExportDateSelect1.SelectedDate != null)
                {
                    EHSProfile profile = new EHSProfile().Load(Convert.ToDecimal(item.Value), false, true);
                    if (profile.Profile != null)
                    {
                        LocalProfileList().Add(profile);

                        DateTime fromDate = (DateTime)radExportDateSelect1.SelectedDate;
                        DateTime toDate = new DateTime(radExportDateSelect2.SelectedDate.Value.Year, radExportDateSelect2.SelectedDate.Value.Month, DateTime.DaysInMonth(radExportDateSelect1.SelectedDate.Value.Year, radExportDateSelect1.SelectedDate.Value.Month));

                        EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange).LoadMetricHistory(new decimal[1] { profile.Plant.PLANT_ID }, fromDate, toDate, DateIntervalType.month, false);
                        uclHistoryList.BindHistoryList(profile, esMgr.MetricHst);
                    }
                }
            }
        }

        private void SetupPage()
        {
            uclExport.BindExport("", false, "");
            ddlReportList.ClearSelection();
        }

        private List<BusinessLocation> LocationList()
        {
            List<BusinessLocation> locationList = new List<BusinessLocation>();
            locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true).ToList();
            locationList = UserContext.FilterPlantAccessList(locationList);
            return locationList;
        }

        protected void btnRollup_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            uclProgress.BindProgressDisplay(100, "Finalizing: ");
            int progressValue = 10;
            uclProgress.UpdateDisplay(1, progressValue, "Calculating...");

            if (LocalProfileList() != null  && LocalProfileList().Count > 0)
            {
                try
                {
                    EHSProfile profile = LocalProfileList().Where(l => l.Plant.PLANT_ID == Convert.ToDecimal(btn.CommandArgument)).FirstOrDefault();
                    DateTime endDate = profile.InputPeriod.PeriodDate;
                    DateTime periodDate = profile.InputPeriod.PeriodDate;
     
                    if (btn.ClientID.Contains("YTD"))
                    {
                        periodDate = new DateTime(profile.InputPeriod.PeriodDate.Year, 1, 1);
                    }

                    int progressDelta = 70 / Math.Max(1,((endDate.Year - periodDate.Year) * 12 + endDate.Month - periodDate.Month));

                    while (periodDate <= endDate)
                    {
                        progressValue += progressDelta;
                        uclProgress.UpdateDisplay(1, progressValue, periodDate.ToShortDateString());

                        if (profile.InputPeriod.PeriodDate != periodDate)
                            profile.LoadPeriod(periodDate);

                        if (profile.ValidPeriod())
                        {
                            if (!profile.InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                            {
                                profile.InputPeriod.PlantAccounting.APPROVAL_DT = DateTime.UtcNow;
                                profile.InputPeriod.PlantAccounting.APPROVER_ID = SessionManager.UserContext.Person.PERSON_ID;
                            }
                            //profile.UpdateMetricHistory();
                            profile.UpdateMetricHistory(periodDate);  // new roll-up logic 
                            periodDate = periodDate.AddMonths(1);
                        }
                    }
                    uclProgress.ProgressComplete();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                    LoadProfileInput();
                   
                }
                catch { }
            }
        }

        protected void btnRollupAll_Click(object sender, EventArgs e)
        {
            uclProgress.BindProgressDisplay(100, "Finalizing: ");
            int progressValue = 10;
            uclProgress.UpdateDisplay(1, progressValue, "Calculating...");

            if (LocalProfileList() != null && LocalProfileList().Count > 0)
            {
                try
                {
                    foreach (EHSProfile profile in LocalProfileList())
                    {
                        DateTime endDate = profile.InputPeriod.PeriodDate;
                        DateTime periodDate = periodDate = new DateTime(profile.InputPeriod.PeriodDate.Year, 1, 1);

                        int progressDelta = 70 / Math.Max(1, ((endDate.Year - periodDate.Year) * 12 + endDate.Month - periodDate.Month));

                        while (periodDate <= endDate)
                        {
                            progressValue += progressDelta;
                            uclProgress.UpdateDisplay(1, progressValue, periodDate.ToShortDateString());

                            if (profile.InputPeriod.PeriodDate != periodDate)
                                profile.LoadPeriod(periodDate);

                            if (profile.ValidPeriod())
                            {
                                if (!profile.InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                                {
                                    profile.InputPeriod.PlantAccounting.APPROVAL_DT = DateTime.UtcNow;
                                    profile.InputPeriod.PlantAccounting.APPROVER_ID = SessionManager.UserContext.Person.PERSON_ID;
                                }
                                profile.UpdateMetricHistory(periodDate);  // new roll-up logic 
                                periodDate = periodDate.AddMonths(1);
                            }
                        }
                    }

                    uclProgress.ProgressComplete();
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                    LoadProfileInput();

                }
                catch { }
            }
        }

        protected void btnEffDateSelect(object sender, EventArgs e)
        {
            if (radEffFrom.SelectedDate > radEffTo.SelectedDate)
            {
                lblTimespanDateError.Visible = true;
                return;
            }

            COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, "", false);
            company.COMPANY_ACTIVITY.EFF_EHS_METRIC_FROM_DT = new DateTime(Convert.ToDateTime(radEffFrom.SelectedDate).Year, Convert.ToDateTime(radEffFrom.SelectedDate).Month, 1);
            company.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT = new DateTime(Convert.ToDateTime(radEffTo.SelectedDate).Year, Convert.ToDateTime(radEffTo.SelectedDate).Month, DateTime.DaysInMonth(Convert.ToDateTime(radEffTo.SelectedDate).Year, Convert.ToDateTime(radEffTo.SelectedDate).Month));
            company.COMPANY_ACTIVITY.EFF_EHS_METRIC_UPD_BY = SessionManager.UserContext.UserName();
            if (SQMModelMgr.UpdateCompany(entities, company, SessionManager.UserContext.UserName()) != null)
            {
                lblTimespanDateError.Visible = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveError');", true);
            }
        }

        public void rptProfile_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHSProfile profile = (EHSProfile)e.Item.DataItem;
                    Label lbl = (Label)e.Item.FindControl("lblLocation");
                    lbl.Text = profile.Plant.PLANT_NAME;

                    List<EHSProfile> periodList = new List<EHSProfile>();
                    periodList.Add(profile);
                    Repeater rpt = (Repeater)e.Item.FindControl("rptProfileStatus");
                    rpt.DataSource = periodList;
                    rpt.DataBind();
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        public void rptProfileStatus_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Label lbl;
                try
                {
                    EHSProfile profile = (EHSProfile)e.Item.DataItem;

                    Button btn = (Button)e.Item.FindControl("btnRollup");
                    btn.CommandArgument = profile.Plant.PLANT_ID.ToString();
                    btn = (Button)e.Item.FindControl("btnRollupYTD");
                    btn.CommandArgument = profile.Plant.PLANT_ID.ToString();

                    DateTime lastUpdateDate;
                    TaskStatus status = profile.PeriodStatus(new string[0] { }, true, out lastUpdateDate);

                    lbl = (Label)e.Item.FindControl("lblInputs");
                    lbl.Text += profile.InputPeriod.NumComplete + " of " + profile.MeasureList(false).Count + " (Total)";
                    lbl = (Label)e.Item.FindControl("lblReqdInputs");
                    lbl.Text = profile.InputPeriod.NumRequiredComplete.ToString() + " of " + profile.InputPeriod.NumRequired.ToString() + " (Required)";
                    LinkButton lnk = (LinkButton)e.Item.FindControl("lnkInputs");
                    lnk.CommandArgument = profile.Plant.PLANT_ID.ToString();

                    lbl = (Label)e.Item.FindControl("lblRollupStatus");
                    lnk = (LinkButton)e.Item.FindControl("lnkHistory");
                    lnk.CommandArgument = profile.Plant.PLANT_ID.ToString();
                    if (profile.InputPeriod.PlantAccounting != null  &&  profile.InputPeriod.PlantAccounting.FINALIZE_DT != null)
                    {
                        lbl.Text = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)profile.InputPeriod.PlantAccounting.FINALIZE_ID, ""));
                        lbl.Text += " ";
                        lbl.Text += SQMBasePage.FormatDate((DateTime)profile.InputPeriod.PlantAccounting.FINALIZE_DT, "d", false);
                        lnk.Visible = true;
                    }

                    string currencyCode = SQMModelMgr.LookupPlant(profile.Plant.PLANT_ID).CURRENCY_CODE;
                    lbl = (Label)e.Item.FindControl("lblRateStatus");
                    lbl.Text = currencyCode + ": ";
                    CURRENCY_XREF currentRate = CurrencyConverter.CurrentRate(currencyCode, profile.InputPeriod.PeriodYear, profile.InputPeriod.PeriodMonth);
                    if (currentRate != null && currentRate.EFF_MONTH > 0)
                    {
                        lbl.Text += (System.Environment.NewLine + SQMBasePage.FormatDate(new DateTime(currentRate.EFF_YEAR, currentRate.EFF_MONTH, DateTime.DaysInMonth(currentRate.EFF_YEAR, currentRate.EFF_MONTH)), "d", false));
                    }

                    if (profile.InputPeriod.PlantAccounting != null)
                    {
                        SETTINGS sets = SQMSettings.GetSetting("EHS", "ACCTFIELDS");  // try to retrieve fields to display for this client
                        Ucl_EHSList ucl = (Ucl_EHSList)e.Item.FindControl("uclProdList");
                        ucl.BindProdFieldsList(profile , sets == null ? "" : sets.VALUE);

                        if (profile.InputPeriod.PlantAccounting.APPROVER_ID.HasValue && profile.InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                        {
                            lbl = (Label)e.Item.FindControl("lblFinalStatus");
                            lbl.Text = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)profile.InputPeriod.PlantAccounting.APPROVER_ID, ""));
                            lbl.Text += " ";
                            lbl.Text += SQMBasePage.FormatDate((DateTime)profile.InputPeriod.PlantAccounting.APPROVAL_DT, "", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        #endregion

        #region export

        public void GenerateExportHistoryExcel(string plantList, DateTime dtFrom, DateTime dtTo)
		{
             uclExport.GenerateExportHistoryExcel(entities, plantList, dtFrom, dtTo);
		}

		public void GenerateIncidentExportExcel(List<EHSIncidentData> incidentList)
		{
            uclExport.GenerateIncidentExportExcel(entities, incidentList);
		}

        public void GenerateEmissionsReport()
        {
            string plantList = "";
            foreach (RadComboBoxItem item in SQMBasePage.GetComboBoxCheckedItems(ddlExportPlantSelect))
            {
                if (radExportDateSelect1.SelectedDate != null)
                {
                    if (plantList.Length > 0)
                        plantList += ", ";
                    plantList += item.Value;
                }
            }
            try
            {
                decimal[] plantArray = Array.ConvertAll(plantList.Split(','), new Converter<string, decimal>(decimal.Parse));
                SQMMetricMgr metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "E", radExportDateSelect1.SelectedDate.Value, radExportDateSelect2.SelectedDate.Value, plantArray); 
                metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
                CalcsResult rslt = metricMgr.CalcsMethods(plantArray, "E", "ghg|co2,ch4,n2o", "gwp100|sum", 22, (int)EHSCalcsCtl.SeriesOrder.YearMeasurePlant);
                EHSModel.GHGResultList ghgTable = (EHSModel.GHGResultList)rslt.ResultObj;
                uclGHGReport.BindGHGReport(ghgTable);
            }
            catch { }
        }

        #endregion

        #region inputs
        protected void lnkDisplayInputs(object sender, EventArgs e)
        {
            EHSProfile profile = null;
            LinkButton lnk = (LinkButton)sender;
            string cmdID = lnk.CommandArgument;
            string id = lnk.ID;
            CheckBox cb;
            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange);

            foreach (RepeaterItem r in rptProfile.Items)
            {
                Repeater rptStatus = (Repeater)r.FindControl("rptProfileStatus");
                foreach (RepeaterItem item in rptStatus.Items)
                {
                    lnk = (LinkButton)item.FindControl("lnkInputs");
                    if (lnk.CommandArgument == cmdID)
                    {
                        profile = LocalProfileList().Where(l => l.Plant.PLANT_ID == Convert.ToDecimal(cmdID)).FirstOrDefault();
                        Ucl_MetricList ucl = (Ucl_MetricList)item.FindControl("uclInputsList");
                        if (id.Contains("Inputs"))
                        {
                            cb = (CheckBox)item.FindControl("cbInputsSelect");
                            cb.Checked = cb.Checked == false ? true : false;
                            if (cb.Checked)
                            {
                                ucl.BindInputsList(profile);
                                ucl.BindHistoryList(null, null);
                              //  cb.Focus();
                            }
                            else
                                ucl.BindInputsList(null);
                        }
                        else
                        {
                            cb = (CheckBox)item.FindControl("cbHistorySelect");
                            cb.Checked = cb.Checked == false ? true : false;
                            if (cb.Checked)
                            {
                                esMgr.LoadMetricHistory(new decimal[1] { profile.Plant.PLANT_ID }, profile.InputPeriod.PeriodDate, profile.InputPeriod.PeriodDate, DateIntervalType.month, false);
                                ucl.BindHistoryList(profile, esMgr.MetricHst);
                                ucl.BindInputsList(null);
                              //  cb.Focus();
                            }
                            else
                            {
                                ucl.BindHistoryList(null, null);
                            }
                        }

                        break;
                    }
                }
            }

            ((LinkButton)sender).Focus();
        }
        #endregion


        // manage current session object  (formerly was page static variable)
        List<EHSProfile> LocalProfileList()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is List<EHSProfile>)
                return (List<EHSProfile>)SessionManager.CurrentObject;
            else
                return null;
        }
        List<EHSProfile> SetLocalProfileList(List<EHSProfile> profileList)
        {
            SessionManager.CurrentObject = profileList;
            return LocalProfileList();
        }
    }
}