using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{

    public partial class Ucl_EHSList : System.Web.UI.UserControl
    {
        public event ItemSelectDate OnProfilePeriodClick;
        public event ItemSelectDate OnCalculateClick;
        public event EditItemClick OnInputSelect;
        public event EditItemClick OnMenuItemClick;
        public event GridItemClick OnPlantSelect;
        public event EditItemClick OnSearchClick;
        public event EditItemClick OnExportClick;
 
        public decimal PlantSelect
        {
            get { return Convert.ToDecimal(ddlPlantSelect.SelectedValue); }
        }

        public DateTime SelectedPeriodDate
        {
            get { return (DateTime)radPeriodSelect.SelectedDate; }
        }
        public RadMonthYearPicker PeriodDateSelect0
        {
            get { return radDateSelect0; }
        }
        public RadMonthYearPicker PeriodDateSelect1
        {
            get { return radDateSelect1; }
        }
        public RadMonthYearPicker PeriodDateSelect2
        {
            get { return radDateSelect2; }
        }

        public RadMenu BusLocMenu
        {
            get { return mnuBusLocSelect; }
        }

        public void ClearPeriodDate()
        {
            radPeriodSelect.Clear();
        }

        #region profileselect

        public void LoadProfileSelectHdr(decimal companyID, decimal busOrgID, bool indicateUndefined, bool showExpanded)
        {
            ToggleVisible(pnlProfileSelectHdr);

            List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(companyID, busOrgID, true).Where(l => l.Plant.TRACK_EW_DATA == true).ToList();
            locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");

            if (showExpanded && locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1)
            {
                ddlBusLocSelect.Visible = false;
                mnuBusLocSelect.Visible = true;
                SQMBasePage.SetLocationList(mnuBusLocSelect, locationList, 0, "select a location ...", "TOP", false);
                if (indicateUndefined)
                {
                    int undefinedCount = 0;
                    List<string> profileList = EHSModel.SelectPlantProfileList(companyID).Select(l => l.PLANT_ID.ToString()).ToList();
                    RadMenuItem miTop = mnuBusLocSelect.Items[0];
                    foreach (RadMenuItem miBU in miTop.Items)
                    {
                        foreach (RadMenuItem miLoc in miBU.Items)
                        {
                            if (!profileList.Contains(miLoc.Value))
                            {
                                ++undefinedCount;
                               // miLoc.ImageUrl = "~/images/defaulticon/16x16/grid-dot.png";
                                miLoc.CssClass = "rcbItemEmphasis";
                            }
                            else
                                miLoc.ImageUrl = "~/images/defaulticon/16x16/document.png";
                        }
                    }
                }
            }
            else
            {
                ddlBusLocSelect.Visible = true;
                mnuBusLocSelect.Visible = false;
                SQMBasePage.SetLocationList(ddlBusLocSelect, locationList, 0);
                if (indicateUndefined)
                {
                    int undefinedCount = 0;
                    List<string> profileList = EHSModel.SelectPlantProfileList(companyID).Select(l => l.PLANT_ID.ToString()).ToList();
                    foreach (RadComboBoxItem item in ddlBusLocSelect.Items)
                    {
                        if (!item.IsSeparator)
                        {
                            if (!profileList.Contains(item.Value))
                            {
                                ++undefinedCount;
                                item.ImageUrl = "~/images/defaulticon/16x16/blank.png";
                                item.CssClass = "rcbItemEmphasis";
                                item.ToolTip = "Metrics not defined";
                            }
                            else
                                item.ImageUrl = "~/images/defaulticon/16x16/document.png";
                        }
                    }
                    if (undefinedCount > 0)
                        ddlBusLocSelect.ToolTip += hfBusLocProfileUndefined.Value;
                }
                ddlBusLocSelect.Items.Insert(0, new RadComboBoxItem("", ""));
            }

        }

        public void ResetProfileSelectHdr()
        {
            ddlBusLocSelect.SelectedIndex = 0;
            lblLocCodePlant_out.Text = lblLocationType_out.Text = lblProfileUpdateBy_out.Text = lblProfileUpdate_out.Text = "";
        }

        public void BindProfileSelectHdr(EHSProfile profile)
        {
            ToggleVisible(pnlProfileSelectHdr);
            if (profile == null)
            {
                lblLocCodePlant_out.Text = lblLocationType_out.Text = lblProfileUpdateBy_out.Text = lblProfileUpdate_out.Text = "";
            }
            else
            {
                lblLocCodePlant_out.Text = profile.Plant.DUNS_CODE;
                lblLocationType_out.Text = WebSiteCommon.GetXlatValue("locationType", profile.Plant.LOCATION_TYPE);
                lblProfileUpdateBy_out.Text = profile.Profile.LAST_UPD_BY;
                lblProfileUpdate_out.Text = SQMBasePage.FormatDate((DateTime)profile.Profile.LAST_UPD_DT, "d", false);
            }
        }

        #endregion

        #region period


        public void BindPeriodtHdr(List<BusinessLocation> locationList, DateTime dateFrom, bool checkAll, bool toDateVisible, string commandArg)
        {
            ToggleVisible(pnlPeriodHdr);
            SQMBasePage.SetLocationList(ddlPlantFilter, locationList, 0);
            if (checkAll)
            {
                foreach (RadComboBoxItem item in ddlPlantFilter.Items)
                {
                    item.Checked = true;
                }
            }

            if (toDateVisible)
            {
                divFromDate.Visible = false;
                divToDate.Visible = true;
                radDateSelect1.MinDate = radDateSelect2.MinDate = new DateTime(2001, 1, 1);
                radDateSelect1.MaxDate = radDateSelect2.MaxDate = DateTime.Now.AddMonths(1);
                radDateSelect1.SelectedDate = radDateSelect2.SelectedDate = DateTime.Now;
                radDateSelect1.ShowPopupOnFocus = radDateSelect2.ShowPopupOnFocus = true;
            }
            else
            {
                divFromDate.Visible = true;
                divToDate.Visible = false;
                radDateSelect0.MinDate = new DateTime(2001, 1, 1);
                radDateSelect0.MaxDate = DateTime.Now.AddMonths(1);
                radDateSelect0.SelectedDate = radDateSelect1.SelectedDate = radDateSelect2.SelectedDate = DateTime.Now;
                radDateSelect0.ShowPopupOnFocus = true;
            }

            btnSearchMetrics.CommandArgument = commandArg;

        }
        public RadComboBox PlantFilterSelect
        {
            get { return ddlPlantFilter; }
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            if (OnCalculateClick != null)
            {
                OnCalculateClick((DateTime)radDateSelect1.SelectedDate);
            }
        }

        #endregion

        #region inputheader

        public void LoadProfileInputHdr(bool loadPlants, DateTime dateFrom, DateTime dateTo, decimal defaultPlantID, bool hideUndefined, bool showExpanded)
        {
            ToggleVisible(pnlProfileInputHdr);

            if (loadPlants && ddlPlantSelect.Items.Count == 0)
            {
                List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true).Where(l => l.Plant.TRACK_EW_DATA == true).ToList();
                locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");

                if (locationList.Count > 1 && showExpanded && locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1)
                {
                    ddlPlantSelect.Visible = false;
                    tdLocation.Visible = false;
                    mnuPlantSelect.Visible = true;
                    SQMBasePage.SetLocationList(mnuPlantSelect, locationList, 0, "select a location...", "TOP", false);

                    int undefinedCount = 0;
                    List<string> profileList = EHSModel.SelectPlantProfileList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID).Select(l => l.PLANT_ID.ToString()).ToList();
                    RadMenuItem miTop = mnuPlantSelect.Items[0];
                    foreach (RadMenuItem miBU in miTop.Items)
                    {
                        foreach (RadMenuItem miLoc in miBU.Items)
                        {
                            if (!profileList.Contains(miLoc.Value))
                            {
                                ++undefinedCount;
                                miLoc.ImageUrl = "~/images/defaulticon/16x16/grid-dot.png";
                                miLoc.CssClass = "rcbItemEmphasis";
                                if (hideUndefined)
                                    miLoc.Visible = false;
                            }
                        }
                    }
                    if (mnuPlantSelect.FindItemByValue(defaultPlantID.ToString()) != null)
                    {
                        mnuPlantSelect.FindItemByValue(defaultPlantID.ToString()).Selected = true;
                        LocationSelect_Click(mnuPlantSelect, null);
                    }
                }
                else
                {
                    ddlPlantSelect.Visible = true;
                    mnuPlantSelect.Visible = false;
                    SQMBasePage.SetLocationList(ddlPlantSelect, locationList, 0);
                    if (ddlPlantSelect.Items.Count == 1)
                    {
                        tdLocationSelect.Visible = false;
                        tdLocation.Visible = true;
                        lblPlantName_out.Text = ddlPlantSelect.SelectedItem.Text;
                        LocationSelect_Click(ddlPlantSelect, null);
                    }
                    else
                    {
                        List<string> profileList = EHSModel.SelectPlantProfileList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID).Select(l => l.PLANT_ID.ToString()).ToList();
                        int undefinedCount = 0;
                        foreach (RadComboBoxItem item in ddlPlantSelect.Items)
                        {
                            if (!item.IsSeparator && !profileList.Contains(item.Value))
                            {
                                ++undefinedCount;
                                //item.ImageUrl = "~/images/defaulticon/16x16/grid-dot.png";
                                item.CssClass = "rcbItemEmphasis";
                                item.ToolTip = "Metrics not defined";
                                if (hideUndefined)
                                    item.Visible = false;
                                else
                                    item.Enabled = false;
                            }
                        }
                        if (undefinedCount > 0 && !hideUndefined)
                            ddlPlantSelect.ToolTip += hfPlantProfileUndefined.Value;

                        tdLocationSelect.Visible = true;
                        tdLocation.Visible = false;
                        if (ddlPlantSelect.FindItemByValue(defaultPlantID.ToString()) != null)
                        {
                            ddlPlantSelect.SelectedValue = defaultPlantID.ToString();
                            LocationSelect_Click(ddlPlantSelect, null);
                        }
                        else
                        {
                            LocationSelect_Click(ddlPlantSelect, null);
                        }
                    }
                }
            }

            if (dateFrom > DateTime.MinValue)
            {
                radPeriodSelect.Visible = true;
                // radPeriodSelect.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                radPeriodSelect.MinDate = dateFrom;
                radPeriodSelect.MaxDate = dateTo;
                radPeriodSelect.ShowPopupOnFocus = true;
                if (dateFrom == dateTo)
                {
                    radPeriodSelect.SelectedDate = dateFrom;
                    radPeriodSelect.Enabled = false;
                }
                else
                    radPeriodSelect.SelectedDate = DateTime.Now;

                radPeriodSelect.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                tdPeriod.Visible = false;
                tdPeriodSelect.Visible = true;
            }
        }

        public void BindProfilePeriodHdr(TaskItem taskItem)
        {
            pnlEHSItemHdr.Visible = true;

            if (taskItem.Plant != null)
                lblProfilePlant_out.Text = taskItem.Plant.PLANT_NAME;

            lblResponsible_out.Text = (string)taskItem.Detail.ToString();

            if (taskItem.RecordID == 0)
            {
                trEHSIncident.Visible = false;
                trEHSInputPeriod.Visible = trEHSInput.Visible = true;
                lblInputPeriod_out.Text = SQMBasePage.FormatDate(taskItem.TaskDate, "MMMM yyyy", false);
                lblInput_out.Text = taskItem.Description;
            }
            else
            {
                trEHSInput.Visible = trEHSInputPeriod.Visible = false;
                trEHSIncident.Visible = true;
                lblIncidentID_out.Text = WebSiteCommon.FormatID(taskItem.RecordID, 6);
                lblDescription_out.Text = taskItem.Description;
            }
        }

        public void BindProfileInputHdr(EHSProfile profile)
        {
            ToggleVisible(pnlProfileInputHdr);
            lblPlantName_out.Text = profile.Plant.PLANT_NAME;
            lblPeriodFrom_out.Text = SQMBasePage.FormatDate(profile.InputPeriod.PeriodDate, "MM/yyyy", false);
            lblDueDate_out.Text = SQMBasePage.FormatDate(profile.InputPeriod.DueDate.AddMonths(1), "d", false);
            DateTime lastUpdateDate;
            TaskStatus status = profile.PeriodStatus(new string[0] { }, true, out lastUpdateDate);

            lblInputStatus1_out.Text = profile.InputPeriod.NumRequiredComplete.ToString();
            lblInputStatus2_out.Text = profile.InputPeriod.NumRequired.ToString();

            lblLastUpdateBy_out.Text = lblLastUpdate_out.Text = "";
            EHS_PROFILE_INPUT lastinput = profile.InputPeriod.GetLastInput();
            if (lastinput == null || lastinput.LAST_UPD_DT.HasValue == false)
            {
                if (profile.InputPeriod.PlantAccounting != null && !string.IsNullOrEmpty(profile.InputPeriod.PlantAccounting.LAST_UPD_BY))
                {
                    lblLastUpdateBy_out.Text = profile.InputPeriod.PlantAccounting.LAST_UPD_BY;
                    if (profile.InputPeriod.PlantAccounting.LAST_UPD_DT.HasValue)
                        lblLastUpdate_out.Text = SQMBasePage.FormatDate((DateTime)profile.InputPeriod.PlantAccounting.LAST_UPD_DT, "", false);
                }
            }
            else
            {
                lblLastUpdateBy_out.Text = lastinput.LAST_UPD_BY;
                lblLastUpdate_out.Text = SQMBasePage.FormatDate((DateTime)lastinput.LAST_UPD_DT, "", false);
            }

            CURRENCY_XREF exchangeRate = profile.InputPeriod.PeriodExchangeRate(profile.Plant);
            if (exchangeRate != null && exchangeRate.EFF_MONTH > 0)
            {
                phRateStatus.Visible = true;
                lblCurrency.Text = profile.Plant.CURRENCY_CODE;
                lblRateStatus.Text = (System.Environment.NewLine + SQMBasePage.FormatDate(new DateTime(exchangeRate.EFF_YEAR, exchangeRate.EFF_MONTH, DateTime.DaysInMonth(exchangeRate.EFF_YEAR, exchangeRate.EFF_MONTH)), "d", false));
            }
        }

        public void SelectLocation(PLANT plant)
        {
            tdLocationSelect.Visible = false;
            tdLocation.Visible = true;
            lblPlantName_out.Text = plant.PLANT_NAME;
        }

        public void TriggerPlantInputSelected()
        {
            LocationSelect_Click(ddlPlantSelect, null);
        }

        protected void LocationSelect_Click(object sender, EventArgs e)
        {
            if (OnPlantSelect != null)
            {
                decimal plantID = 0;
                try
                {
                    if (sender is RadComboBox)
                    {
                        RadComboBox ddl = (RadComboBox)sender;
                        if (ddl.SelectedValue.All(c => c >= '0' && c <= '9') == true)
                            plantID = Convert.ToDecimal(ddl.SelectedValue);
                    }
                    else if (sender is RadMenu)
                    {
                        RadMenu mnu = (RadMenu)sender;
                        mnu.Items[0].Text = mnu.SelectedItem.Text;
                        if (mnu.SelectedValue.All(c => c >= '0' && c <= '9') == true)
                            plantID = Convert.ToDecimal(mnu.SelectedValue);
                    }
                    OnPlantSelect(plantID);
                }
                catch { }
            }
        }

        protected void radDateSelect1Click(Object sender, EventArgs e)
        {
            RadMonthYearPicker dmSelect = (RadMonthYearPicker)sender;

            if (dmSelect.SelectedDate == null)
            {
                return;
            }
            else
            {
                btnCalculate.Enabled = true;
                if (OnProfilePeriodClick != null)
                {
                    OnProfilePeriodClick(new DateTime(dmSelect.SelectedDate.Value.Year, dmSelect.SelectedDate.Value.Month, 1));
                }
            }
        }

        protected void btnSearchMetricsClick(Object sender, EventArgs e)
        {
            if (OnSearchClick != null)
            {
                Button btn = (Button)sender;
                if (btn.CommandArgument.ToString().Equals("export"))
                {
                    lblMessage.Text = "";
                    // verify that at least the from date is entered
                    if (radDateSelect1.SelectedDate.HasValue)
                    {
                        try
                        {
                            DateTime tmp = radDateSelect1.SelectedDate.Value;
                        }
                        catch
                        {
                            lblMessage.Text = "Invalid value in Reporting Month From date.";
                            pnlMessage.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        lblMessage.Text = "You must select a Reporting Month From date.";
                        pnlMessage.Visible = true;
                        return;
                    }
                    OnExportClick(btn.ID);
                }
                else
                    OnSearchClick(btn.ID);
            }
        }

        #endregion

        public void BindProdFieldsList(EHSProfile profile, string acctFields)
        {
            Dictionary<string, string> fieldList = new Dictionary<string, string>();
            string[] fieldNames = { "OPER_COST", "REVENUE", "TIME_WORKED", "RECORDED_CASES", "TIME_LOST_CASES" }; // default fields to display

            if (!string.IsNullOrEmpty(acctFields))  // desired fields to display 
            {
                fieldNames = acctFields.Split(',');
                if (profile.InputPeriod.PlantAccounting != null)
                {
                    var PropertyInfos = profile.InputPeriod.PlantAccounting.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo pInfo in PropertyInfos)
                    {
                        if (fieldNames.Contains(pInfo.Name))
                        {
                            object obj = pInfo.GetValue(profile.InputPeriod.PlantAccounting, null);
                            if (obj != null)
                            {
                                fieldList.Add(pInfo.Name, SQMBasePage.FormatValue((decimal)obj, 2));
                            }
                            else
                                fieldList.Add(pInfo.Name, "");
                        }
                    }
                    pnlProdList.Visible = true;
                    gvProdList.DataSource = fieldList;
                    gvProdList.DataBind();
                }
            }
        }


        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            pnlProfileInputHdr.Visible = pnlProfileSelectHdr.Visible = pnlPeriodHdr.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }
        #endregion


        #region profileperiod
        public List<EHSProfilePeriod> SelectProfilePeriodList(EHSProfile profile, DateTime fromDate, DateTime toDate)
        {
            List<EHSProfilePeriod> periodList = new List<EHSProfilePeriod>();

            foreach (WebSiteCommon.DatePeriod pd in WebSiteCommon.CalcDatePeriods(fromDate, toDate, DateIntervalType.month, DateSpanOption.SelectRange, ""))
            {
                profile.LoadPeriod(pd.FromDate);
                // profile.MapPlantAccountingInputs(true, true);
                periodList.Add(profile.InputPeriod);
            }

            return periodList;
        }

        public int BindPeriodList(List<EHSProfilePeriod> periodList)
        {
            int rowCount = 0;

            if (periodList != null)
            {
                pnlProfilePeriodList.Visible = true;
                rgProfilePeriodList.DataSource = periodList;
                rgProfilePeriodList.DataBind();
                rowCount = periodList.Count;
            }

            return rowCount;
        }

        protected void rgProfilePeriodList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HiddenField hf;
                Label lbl;

                try
                {
                    EHSProfilePeriod period = (EHSProfilePeriod)e.Item.DataItem;

                    LinkButton lnk = (LinkButton)e.Item.FindControl("lnkPeriod");
                    lnk.Text = SQMBasePage.FormatDate(period.PeriodDate, "MMMM yyyy", false);
                    lnk.CommandArgument = period.PeriodDate.ToShortDateString();

                    Ucl_MetricList ucl = (Ucl_MetricList)e.Item.FindControl("uclInputsList");
                    ucl.BindInputsList(period, "");
                }
                catch (Exception ex)
                {

                }
            }
        }

        protected void lnkPeriod_Click(object sender, EventArgs e)
        {
            ;
        }
        #endregion
    }


}