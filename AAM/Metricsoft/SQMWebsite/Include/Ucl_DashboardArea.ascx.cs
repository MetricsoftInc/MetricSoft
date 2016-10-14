using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;
using Telerik.Charting;


namespace SQM.Website
{
    public delegate void UclDashboardModeChanged(Ucl_DashboardArea ucldashboard, string mode);

    public enum ViewCriteriaOption { NoDefaults, EnableOverride, NoOverride, SelectLocs, SelectSpan, DefaultView };

    [Flags]
    public enum DashboardOpts {None = 0, TotalsOnly=2 };

    public partial class DashboardCriteria
    {
        public DateTime FromDate
        {
            get;
            set;
        }
        public DateTime ToDate
        {
            get;
            set;
        }
        public int FiscalYear
        {
            get;
            set;
        }
        public DateSpanOption DateSpanType
        {
            get;
            set;
        }
        public DateIntervalType DateInterval
        {
            get;
            set;
        }
        public DashboardOpts Options
        {
            get;
            set;
        }
        public List<BusinessLocation> PlantList
        {
            get;
            set;
        }
        public decimal[] PlantArray
        {
            get;
            set;
        }
        public decimal[] MetricArray
        {
            get;
            set;
        }
        public string[] MetricNameArray
        {
            get;
            set;
        }
        public List<EHS_MEASURE> MeasureList
        {
            get;
            set;
        }

        public DashboardCriteria Initialize()
        {
            this.DateInterval = DateIntervalType.fuzzy;
            this.Options = new DashboardOpts();
            return this;
        }
        public DashboardOpts ResetOptions()
        {
            this.Options = new DashboardOpts();
            return this.Options;
        }
        public DashboardOpts SetOption(DashboardOpts option)
        {
            this.Options = this.Options | option;
            return this.Options;
        }
        public DashboardOpts RemoveOption(DashboardOpts option)
        {
            this.Options &= ~option;
            return this.Options;
        }
    }

    public partial class Ucl_DashboardArea : System.Web.UI.UserControl
    {
        static COMPANY localCompany;
        static PERSPECTIVE_VIEW localView;
        static PERSPECTIVE_VIEW tempView;
        static PSsqmEntities localCtx;
        static DashboardCriteria localCriteria;
        static List<RadComboBoxItem> scopeList;
 
        #region events

        public event EditItemClick OnViewModeChange;
        public event UclDashboardModeChanged OnDashboardModeChange;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblPeriodFrom.Text = this.lblYearFrom.Text = Resources.LocalizedText.From + ": ";
			this.lblPeriodTo.Text = this.lblYearTo.Text = Resources.LocalizedText.To + ": ";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        private void MessageDisplay(Label displayLabel)
        {
           LblViewSaveError.Visible = lblWorking.Visible = lblViewLoadError.Visible = false;

            if (displayLabel != null)
                displayLabel.Visible = true;
        }

        protected void ddlViewList_Select(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            LoadView();
        }

        protected void btnRefreshDashboard_Click(object sender, EventArgs e)
        {
            if (localView == null)
            {
                MessageDisplay(lblViewLoadError);
                return;
            }

            uclProgress.BindProgressDisplay(100, "Updating View: ");
            uclProgress.UpdateDisplay(1, 10, GetLocalResourceObject("Loading").ToString());

            string[] plantSels = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(l => l.Value).ToArray(); // ddlPlantSelect.Items.Where(i => i.Checked == true).Select(i => i.Value).ToArray();
            string[] plantNameArray = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(l => l.Text).ToArray();  // ddlPlantSelect.Items.Where(i => i.Checked == true).Select(i => i.Text).ToArray();
            decimal[] plantArray = Array.ConvertAll(plantSels, new Converter<string, decimal>(decimal.Parse));

            if (ddlYearFrom.Visible)
            {
                if (localCriteria.DateSpanType == DateSpanOption.FYYearOverYear)
                {
                    dmPeriodFrom.SelectedDate = new DateTime(Convert.ToInt32(ddlYearFrom.SelectedValue)-1, SessionManager.FYStartDate().Month, 1);
                    DateTime toDate = new DateTime(Convert.ToInt32(ddlYearTo.SelectedValue), SessionManager.FYStartDate().Month == 1 ? 12 : SessionManager.FYStartDate().Month - 1, 1);
                    dmPeriodTo.SelectedDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));
                }
                else
                {
                    dmPeriodFrom.SelectedDate = new DateTime(Convert.ToInt32(ddlYearFrom.SelectedValue), 1, 1);
                    dmPeriodTo.SelectedDate = new DateTime(Convert.ToInt32(ddlYearTo.SelectedValue), 12, 31);
                }
            }
     
            localCriteria.FromDate = new DateTime(dmPeriodFrom.SelectedDate.Value.Year, dmPeriodFrom.SelectedDate.Value.Month, 1);
            localCriteria.ToDate = new DateTime(dmPeriodTo.SelectedDate.Value.Year, dmPeriodTo.SelectedDate.Value.Month, DateTime.DaysInMonth(dmPeriodTo.SelectedDate.Value.Year, dmPeriodTo.SelectedDate.Value.Month));
            localCriteria.DateInterval = (DateIntervalType)Enum.Parse(typeof(DateIntervalType), ddlDateInterval.SelectedValue);
            localCriteria.FiscalYear = localCriteria.FromDate.Year + 1;
            localCriteria.PlantArray = plantArray;
            localCriteria.MetricArray = new decimal[0];
            localCriteria.MetricNameArray = new string[0];

			// final 'from' date reset when querying for FY YTD
			if (localCriteria.DateSpanType == DateSpanOption.FYYearToDate)
			{
				dmPeriodFrom.SelectedDate = WebSiteCommon.PriorFYPeriod((DateTime)dmPeriodTo.SelectedDate).FromDate;
				localCriteria.FromDate = Convert.ToDateTime(dmPeriodFrom.SelectedDate).AddYears(1);
			}

            string[] optionSels = ddlOptions.Items.Where(i => i.Checked == true).Select(i => i.Value).ToArray();

            if (optionSels.Contains(Convert.ToInt32(Enum.Parse(typeof(DashboardOpts), DashboardOpts.TotalsOnly.ToString())).ToString()))
                localCriteria.SetOption(DashboardOpts.TotalsOnly);
            else
                localCriteria.RemoveOption(DashboardOpts.TotalsOnly);
  
            DisplayView("");

            uclProgress.UpdateDisplay(3, 90, "Drawing...");
        }

        protected void ddlDateSpanChange(object sender, EventArgs e)
        {
            localCriteria.DateSpanType = (DateSpanOption)Convert.ToInt32(ddlDateSpan.SelectedValue);

            DateTime lastYear;
            int fyMonth =  Convert.ToInt32(Math.Max(1,(decimal)SessionManager.PrimaryCompany().FYSTART_MONTH));

            //dmPeriodFrom.Visible = dmPeriodTo.Visible = true;
            //ddlYearFrom.Visible = ddlYearTo.Visible = false;

            phPeriodSpan.Visible = true;
            phYearSpan.Visible = false;

            switch (localCriteria.DateSpanType)
            {
                case DateSpanOption.YearToDate:      // ytd
                    dmPeriodFrom.SelectedDate = new DateTime(SessionManager.UserContext.LocalTime.Year, 1, 1);
                    dmPeriodTo.SelectedDate = new DateTime(SessionManager.UserContext.LocalTime.Year, SessionManager.UserContext.LocalTime.Month, 1);
                    break;
                case DateSpanOption.YearOverYear:      // year over year
                    dmPeriodFrom.SelectedDate = SessionManager.UserContext.LocalTime.AddYears(-1);
					dmPeriodTo.SelectedDate = SessionManager.UserContext.LocalTime;
                    ddlYearFrom.SelectedValue = (SessionManager.UserContext.LocalTime.Year - 1).ToString();
                    ddlYearTo.SelectedValue = SessionManager.UserContext.LocalTime.Year.ToString();
                    //dmPeriodFrom.Visible = dmPeriodTo.Visible = false;
                    //ddlYearFrom.Visible = ddlYearTo.Visible = true;
                    phPeriodSpan.Visible = false;
                    phYearSpan.Visible = true;
                    break;
                case DateSpanOption.FYYearOverYear:  // fy uear over year
                    dmPeriodFrom.SelectedDate = SessionManager.UserContext.LocalTime.AddYears(-1);
					dmPeriodTo.SelectedDate = SessionManager.UserContext.LocalTime;
                    ddlYearFrom.SelectedValue = (SessionManager.UserContext.LocalTime.Year - 1).ToString();
                    ddlYearTo.SelectedValue = SessionManager.UserContext.LocalTime.Year.ToString();
                    //dmPeriodFrom.Visible = dmPeriodTo.Visible = false;
                    //ddlYearFrom.Visible = ddlYearTo.Visible = true;
                    phPeriodSpan.Visible = false;
                    phYearSpan.Visible = true;
                    break;
                case DateSpanOption.PreviousYear:  // prev year
                    lastYear = SessionManager.UserContext.LocalTime.AddYears(-1);
                    dmPeriodFrom.SelectedDate = new DateTime(lastYear.Year, 1, 1);
                    dmPeriodTo.SelectedDate = new DateTime(lastYear.Year, 12, 1);
                    break;
				case DateSpanOption.FYYearToDate:       // fy ytd
					dmPeriodTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
					dmPeriodFrom.SelectedDate = WebSiteCommon.PriorFYPeriod((DateTime)dmPeriodTo.SelectedDate).FromDate;
					lblPeriodFrom.Visible = dmPeriodFrom.Visible = false;

					if (localView.PERSPECTIVE == "0" || localView.PERSPECTIVE == "E")
					{
						dmPeriodTo.SelectedDate = localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT < dmPeriodTo.SelectedDate ? localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT : dmPeriodTo.SelectedDate;
						if (dmPeriodTo.SelectedDate < dmPeriodFrom.SelectedDate)
						{
							DateTime dt = Convert.ToDateTime(localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT).AddYears(-1);
							dt = new DateTime(dt.Year, (int)localCompany.FYSTART_MONTH, 1);
							dmPeriodFrom.SelectedDate = dt;
						}
					}
					break;
                case DateSpanOption.FYEffTimespan:
                    if (localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_FROM_DT.HasValue && localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT.HasValue)
                    {
                        dmPeriodFrom.SelectedDate = localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_FROM_DT;
                        dmPeriodTo.SelectedDate = localCompany.COMPANY_ACTIVITY.EFF_EHS_METRIC_DT;
                    }
                    else
                    {
                        dmPeriodFrom.SelectedDate = SessionManager.FYStartDate();
                        dmPeriodTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                    break;
                default:
                    if (localView.DFLT_TIMEFRAME > 1900)	// specific year
                    {
						dmPeriodFrom.Visible = dmPeriodTo.Visible = true;
                        dmPeriodFrom.SelectedDate = new DateTime((int)localView.DFLT_TIMEFRAME, SessionManager.FYStartDate().Month, 1);
                        dmPeriodTo.SelectedDate = new DateTime((int)localView.DFLT_TIMEFRAME, SessionManager.FYEndDate((DateTime)dmPeriodFrom.SelectedDate).Month, 28);
                    }
					else if (localView.DFLT_TIMEFRAME < 0)	// starting n months in the past
					{
						dmPeriodTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
						dmPeriodFrom.SelectedDate = ((DateTime)dmPeriodTo.SelectedDate).AddMonths((int)localView.DFLT_TIMEFRAME);
					}
					else
					{
						dmPeriodFrom.SelectedDate = SessionManager.FYStartDate();
						dmPeriodTo.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
					}
                    break;
            }
        }

		protected void btnLocationSelect_Click(object sender, EventArgs e)
		{
			if (ddlBusOrg.Items.Count == 0)
			{
				PSsqmEntities ctx = new PSsqmEntities();
				ddlBusOrg.DataSource = localCriteria.PlantList.Select(l => l.BusinessOrg).Distinct().ToList();
				ddlBusOrg.DataValueField = "BUS_ORG_ID";
				ddlBusOrg.DataTextField = "ORG_NAME";
				ddlBusOrg.DataBind();

				Dictionary<string, string> regionDCL = WebSiteCommon.GetXlatList("countryCode", "", "long");
				foreach (string region in localCriteria.PlantList.Select(l => l.Plant).Select(l => l.LOCATION_CODE).Distinct().ToList())
				{
					if (!string.IsNullOrEmpty(region))
					{
						ddlRegion.Items.Add(new RadComboBoxItem(regionDCL[region], region));
					}
				}
			}

			string script = "function f(){OpenLocationSelectWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnLocationApply_Click(object sender, EventArgs e)
		{
			List<decimal> plantList = new List<decimal>();

			if (ddlBusOrg.CheckedItems.Count > 0)
			{
				plantList.AddRange(localCriteria.PlantList.Where(l => ddlBusOrg.CheckedItems.Select(i=> i.Value).Contains(l.Plant.BUS_ORG_ID.ToString())).Select(l => l.Plant.PLANT_ID).ToList());
			}
			else
			{
				plantList.AddRange(localCriteria.PlantList.Select(l => l.Plant.PLANT_ID).ToList());
			}

			if (ddlRegion.CheckedItems.Count > 0)
			{
				List<decimal> prList = localCriteria.PlantList.Where(l => ddlRegion.CheckedItems.Select(i=> i.Value).Contains(l.Plant.LOCATION_CODE)).Select(l => l.Plant.PLANT_ID).ToList();
				plantList = plantList.Intersect(prList).ToList();
			}

			foreach (RadComboBoxItem item in ddlPlantSelect.Items.Where(i=> i.Enabled == true).ToList())
			{
				item.Checked = false;
				decimal plantID = 0;
				if (decimal.TryParse(item.Value, out plantID)  &&  plantList.Contains(plantID))
				{
					item.Checked = true;
				}
			}
		}
		protected void btnLocationCancel_Click(object sender, EventArgs e)
		{
			;
		}

        protected void onViewLayoutClick(object o, EventArgs e)
        {
            Button btn = (Button)o;
            MessageDisplay(null);

            if (btn.CommandArgument == "new")
            {
                BindViewLayout((localView = ViewModel.CreateView(ddlPerspective.SelectedValue, 
                    SessionManager.UserContext.HRLocation.Company.COMPANY_ID, SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID, SessionManager.UserContext.HRLocation.Plant.PLANT_ID,
                    SessionManager.UserContext.Person.PERSON_ID)), btn.CommandArgument);
                hfViewMode.Value = "l";
            }
            else
            {
				BindViewLayout((localView = ViewModel.LookupView(localCtx, "", "", Convert.ToDecimal(ddlViewList.SelectedValue), SessionManager.UserContext.Language.NLS_LANGUAGE)), btn.CommandArgument);
                hfViewMode.Value = "l";
            }

            ddlViewList.Enabled = btnRefreshDashboard.Enabled = btnNewView.Enabled = btnViewLayout.Enabled = false;
            btnViewLayout.Enabled = btnNewView.Enabled = false;
            btnTestView.Visible = btnViewCancel.Visible = btnViewSave.Visible = true;
            divDashboardArea.Visible = false;
            divLayoutArea.Visible = true;

            if (OnDashboardModeChange != null)
            {
                OnDashboardModeChange(this, hfViewMode.Value);
            }
        }

        protected void onViewEditClick(object o, EventArgs e)
        {
            Button btn = (Button)o;
            btnViewLayout.Enabled = btnNewView.Enabled = true;
            MessageDisplay(null);

            if (btn.CommandArgument.Contains("save"))
            {
                if (SaveView(true) == 0)
                {
                    hfViewStatus.Value = localView.STATUS;
                    ddlViewList.Items.Add(new RadComboBoxItem(localView.VIEW_NAME, localView.VIEW_ID.ToString()));
                    ddlViewList.SelectedValue = localView.VIEW_ID.ToString();
                }
                else 
                {
                    ; // report error
                }
            }

            hfViewMode.Value = "a";
            ddlViewList.Enabled = btnRefreshDashboard.Enabled = btnNewView.Enabled = btnViewLayout.Enabled = true;
            btnTestView.Visible = btnViewCancel.Visible = btnViewSave.Visible = false;
            divLayoutArea.Visible = false;
            divDashboardArea.Visible = true;
            if (OnDashboardModeChange != null)
            {
                OnDashboardModeChange(this, hfViewMode.Value);
            }
        }

        protected void onAddChartClick(object o, EventArgs e)
        {
            SaveView(false);  // preserve any exsting edits
            PERSPECTIVE_VIEW_ITEM vi = ViewModel.CreateViewItem(localView.VIEW_ID);
            if (localView.PERSPECTIVE_VIEW_ITEM.Count == 0)
                vi.ITEM_SEQ = 1;
            else
                vi.ITEM_SEQ = localView.PERSPECTIVE_VIEW_ITEM.Select(l => l.ITEM_SEQ).Max() + 1;
            localView.PERSPECTIVE_VIEW_ITEM.Add(vi);
            BindViewItemsLayout(localView);
        }

        protected void onTestViewClick(object o, EventArgs e)
        {
            SaveView(false);  // preserve any exsting edits
            tempView = localView;
            DisplayView("");
        }

 
        #endregion

        #region setup
        public Label DashboardTitle
        {
            get { return lblDashboardTitle; }
        }
        public System.Web.UI.HtmlControls.HtmlGenericControl DashboardArea
        {
            get { return divDashboardArea; }
        }
        public Ucl_RadGauge GaugeControl
        {
            get { return uclGauge; }
        }
        public string ViewMode
        {
            get { return hfViewMode.Value; }
        }

        public Ucl_DashboardArea Initialize(bool displaySelects)
        {
            localCtx = new PSsqmEntities();
            localView = null;
            ddlDateSpan.SelectedIndex = 1;
            dmPeriodFrom.SelectedDate = new DateTime(SessionManager.UserContext.LocalTime.Year, 1, 1);
			dmPeriodTo.SelectedDate = SessionManager.UserContext.LocalTime;
            dmPeriodFrom.ShowPopupOnFocus = dmPeriodTo.ShowPopupOnFocus = true;
            ddlYearFrom.Items.AddRange(WebSiteCommon.PopulateComboBoxListNums(2000, (SessionManager.UserContext.LocalTime.Year - 2000)+1, ""));
            ddlYearTo.Items.AddRange(WebSiteCommon.PopulateComboBoxListNums(2000, (SessionManager.UserContext.LocalTime.Year - 2000)+2, ""));
            btnNewView.Enabled = true;
            localCriteria = new DashboardCriteria().Initialize();

            uclExport.BindExport("", false, "");

            btnNewView.Visible = btnViewLayout.Visible = false;

            if (!displaySelects)
                divPerspective.Visible = divDashboardSelects.Visible = false;

            localCriteria.PlantList = new List<BusinessLocation>();
            localCriteria.PlantList = SessionManager.PlantList;
           // localCriteria.PlantList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
            SQMBasePage.SetLocationList(ddlPlantSelect, localCriteria.PlantList, 0);
            
            List<PERSPECTIVE_VIEW> viewList = ViewModel.SelectFilteredViewList("", SessionManager.UserContext.Person.PERSON_ID,
                 SessionManager.UserContext.HRLocation.Company.COMPANY_ID,
                  SessionManager.UserContext.HRLocation.BusinessOrg.BUS_ORG_ID,
				 SessionManager.UserContext.HRLocation.Plant.PLANT_ID, true, SessionManager.UserContext.Language.NLS_LANGUAGE);

            string perspective = "0";
            RadComboBoxItem li;
            foreach (PERSPECTIVE_VIEW view in viewList.Where(l=> l.AVAILABILTY > 0).OrderBy(l => l.DISPLAY_SEQ).ThenBy(l => l.VIEW_NAME).ToList())
            {
                if (viewList.Count > 8)
                {
                    if (view.PERSPECTIVE != perspective)
                    {
                        li = new RadComboBoxItem(WebSiteCommon.GetXlatValueLong("viewPerspective", view.PERSPECTIVE), WebSiteCommon.GetXlatValue("viewPerspective", view.PERSPECTIVE));
                        li.IsSeparator = true;
                        ddlViewList.Items.Add(li);
                        perspective = view.PERSPECTIVE;
                    }
                }
                
                li = new RadComboBoxItem(view.VIEW_NAME, view.VIEW_ID.ToString());
                li.ToolTip = view.VIEW_DESC;
                ddlViewList.Items.Add(li);
            }

            ddlViewList.SelectedIndex = 0;
            ddlPlantSelect.Enabled = ddlDateSpan.Enabled = btnRefreshDashboard.Enabled = true;
            LoadView();
          

            return this;
        }

        private int LoadView()
        {
            int status = 0;
            hfViewMode.Value = "a";
            hfActive.Value = true.ToString();
            divDashboardArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;

            MessageDisplay(lblWorking);
          
            try
            {
                decimal viewID = Convert.ToDecimal(ddlViewList.SelectedValue);

				localView = ViewModel.LookupView(localCtx, "", "", viewID, SessionManager.UserContext.Language.NLS_LANGUAGE);
                if (localView == null)
                {
                    MessageDisplay(lblViewLoadError);
                    return -1;
                }
                localCompany = SQMModelMgr.LookupCompany(localView.COMPANY_ID);

                if (localView.STATUS == "A")
                    btnViewLayout.Enabled = true;
                else
                    btnViewLayout.Enabled = false;

                if (localView.PERSPECTIVE == "E")
                {
                    SQMBasePage.SetLocationList(ddlPlantSelect, localCriteria.PlantList.Where(l => l.Plant.TRACK_EW_DATA == true).ToList(), 0);
                        
                }
                else
                {
                    SQMBasePage.SetLocationList(ddlPlantSelect, localCriteria.PlantList, 0);
                        
                }
              
                hfPerspective.Value = localView.PERSPECTIVE;
                ddlPlantSelect.ClearCheckedItems();
                ddlPlantSelect.EnableCheckAllItemsCheckBox = true;

                RadComboBoxItem cbi = null;
                string[] sels = localView.DFLT_LOCATIONS.Split(',');
                foreach (string loc in sels)
                {
                    try
                    {
                        if (loc == "0")  // select all
                        {
                            foreach (RadComboBoxItem item in ddlPlantSelect.Items.Where(l => l.IsSeparator == false))
                                item.Checked = true;
                        }
                        else if (loc.Contains("B"))   // select a business unit
                        {
                            decimal buID = Convert.ToDecimal(loc.Substring(1));
                            SQMBasePage.SetLocationList(ddlPlantSelect, SessionManager.PlantList.Where(l => l.Plant.BUS_ORG_ID == buID).ToList(), 0);
                            //ddlPlantSelect.EnableCheckAllItemsCheckBox = false;
                            foreach (RadComboBoxItem ci in ddlPlantSelect.Items)
                            {
                                ci.Checked = ci.Enabled = ci.Visible = true;
                            }
                        }
                        else  // specific location
                        {
                            if (loc.Contains("-"))  // remove a specific location
                            {
                                if ((cbi = ddlPlantSelect.Items.FindItemByValue(loc.Substring(1))) != null)
                                {
                                    cbi.Checked = false;
                                    ddlPlantSelect.EnableCheckAllItemsCheckBox = false;
                                }
                            }
                            else
                            {
                                if ((cbi = ddlPlantSelect.Items.FindItemByValue(loc)) != null)
                                    cbi.Checked = true;
                            }
                        }
                    }
                    catch { ; }
                }

                foreach (RadComboBoxItem ci in ddlPlantSelect.Items)
                {
                    if (!ci.IsSeparator)
                        ci.Enabled = true;
                }

				SETTINGS sets = SQMSettings.GetSetting("DASHBOARD", "DATEOPTIONS");
				if (sets != null)
				{
					ddlDateSpan.Items.Select(i => { i.Visible = false; return i; }).ToList();
					foreach (string opt in WebSiteCommon.SplitString(sets.VALUE, ','))
					{
						if (ddlDateSpan.Items.FindItemByValue(opt) != null)
						{
							ddlDateSpan.Items.FindItemByValue(opt).Visible = true;
						}
					}
					if (ddlDateSpan.Items.Where(i => i.Visible == true).Count() < 2)
					{
						ddlDateSpan.Visible = false;
					}
				}

                if (localView.DFLT_TIMEFRAME.HasValue)
                {
                    if (localView.DFLT_TIMEFRAME > 1900 || localView.DFLT_TIMEFRAME < 0)
                    {
                        ddlDateSpan.SelectedIndex = 0;

                    }
                    else
                    {
                        ddlDateSpan.SelectedValue = localView.DFLT_TIMEFRAME.ToString();
                    }
                    ddlDateSpanChange(ddlDateSpan, null);
					sets = SQMSettings.GetSetting("DASHBOARD", "DATEOFFSET");
					if (sets != null  &&  !string.IsNullOrEmpty(sets.VALUE))
					{
						dmPeriodTo.SelectedDate = ((DateTime)dmPeriodTo.SelectedDate).AddMonths(Convert.ToInt32(sets.VALUE));
					}
                }

                bool autoDisplay = false;

                switch ((ViewCriteriaOption)localView.DFLT_OPTION)
                {
                    case ViewCriteriaOption.NoDefaults:
                        ddlPlantSelect.Enabled = ddlDateSpan.Enabled = false;
                        dmPeriodFrom.Enabled = dmPeriodTo.Enabled = false;
                        ddlPlantSelect.ClearCheckedItems();
                        //pnlDashboardSelects.Style.Add("display", "inline");
                        break;
                    case ViewCriteriaOption.NoOverride:
                        ddlPlantSelect.Enabled = ddlDateSpan.Enabled = false;
                        dmPeriodFrom.Enabled = dmPeriodTo.Enabled = false;
                        //pnlDashboardSelects.Style.Add("display", "none");
                        autoDisplay = true;
                        break;
                    case ViewCriteriaOption.SelectLocs:
                        ddlPlantSelect.Enabled = true;
                        ddlDateSpan.Enabled = dmPeriodFrom.Enabled = dmPeriodTo.Enabled = false;
                        //pnlDashboardSelects.Style.Add("display", "inline");
                        break;
                    case ViewCriteriaOption.SelectSpan:
                        ddlPlantSelect.Enabled = false;
                        ddlDateSpan.Enabled = dmPeriodFrom.Enabled = dmPeriodTo.Enabled = true;
                        //pnlDashboardSelects.Style.Add("display", "inline");
                        break;
                    case ViewCriteriaOption.EnableOverride:
                    default:
                        ddlPlantSelect.Enabled = ddlDateSpan.Enabled = true;
                        dmPeriodFrom.Enabled = dmPeriodTo.Enabled = true;
                        //pnlDashboardSelects.Style.Add("display", "inline");
                        break;
                }

				if (localCriteria.DateSpanType == DateSpanOption.FYYearToDate)
				{
					dmPeriodFrom.Enabled = false;
				}

                // auto display w/ options section closed if the default view
                if (localView.PERSPECTIVE == "0")
                {
                    autoDisplay = true;
                   // pnlDashboardSelects.Style.Add("display", "none");
                }

                if (ddlPlantSelect.CheckedItems.Count > 0 && dmPeriodFrom.SelectedDate != null && dmPeriodTo.SelectedDate != null)
                    autoDisplay = true;

                localCriteria.ResetOptions();
                ddlOptions.Items.FindItemByValue(Convert.ToInt32(Enum.Parse(typeof(DashboardOpts), DashboardOpts.TotalsOnly.ToString())).ToString()).Checked = false;
                ddlOptions.Items.FindItemByValue(Convert.ToInt32(Enum.Parse(typeof(DashboardOpts), DashboardOpts.TotalsOnly.ToString())).ToString()).Enabled = ViewModel.HasArrays(localView);

                 MessageDisplay(null);

                // auto display view 
                if (autoDisplay)
                    btnRefreshDashboard_Click(btnRefreshDashboard, null);
            }

            catch (Exception ex)
            {
                MessageDisplay(lblViewLoadError);
            }

            return status;
        }

        #endregion

        #region displayview
       
        protected void lnkExportClick(object sender, EventArgs e)
        {
            DisplayView("export");
        }

        public int  DisplayView(string context)
        {
            uclProgress.UpdateDisplay(2, 30, "Calculating...");
            
            divDashboardArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            if (hfViewMode.Value == "a")
            {
                divLayoutArea.Visible = false;
                divDashboardArea.Visible = true;
                lblDashboardTitle.Text = SQMBasePage.InsertDateCriteria(localView.VIEW_DESC, localCriteria.FromDate, localCriteria.ToDate);
            }
            else
            {
                divLayoutArea.Visible = true;
                divDashboardArea.Visible = true;
            }

            int status = 0;
            GaugeDefinition ggCfg = new GaugeDefinition();

            SQMMetricMgr metricMgr = new SQMMetricMgr(); 
           
            switch (localView.PERSPECTIVE)
            {
                case "QS":
                case "CST":
                case "RCV":
                    lnkExport.Visible = false;
                    break;

                default:
					if (localView.PERSPECTIVE == "XD")
					{
						// for ehs_data dashboards, try to determine a finite list of measure ID's used to populate all the views/graphs in the dashboard ..
						// .. in order to reduce the number of records processed. 
						// ehs_data table contains a shit-load of records for any given month and seems slow to process 
						List<EHS_MEASURE> measureList = EHSModel.SelectEHSMeasureList("", false);
						List<decimal> measureIDS = new List<decimal>();
						decimal measureID = 0;
						foreach (PERSPECTIVE_VIEW_ITEM vi in localView.PERSPECTIVE_VIEW_ITEM)
						{
							switch (vi.CALCS_SCOPE)
							{
								case "ltcr":
								case "ltsr":
								case "rcr":
									measureIDS.Add(measureList.Where(l => l.MEASURE_CD == "S60002").FirstOrDefault().MEASURE_ID);	// time worked
									measureIDS.Add(measureList.Where(l => l.MEASURE_CD == "S20004").FirstOrDefault().MEASURE_ID);	// recordable cases
									measureIDS.Add(measureList.Where(l => l.MEASURE_CD == "S20005").FirstOrDefault().MEASURE_ID);	// time lost cases
									measureIDS.Add(measureList.Where(l => l.MEASURE_CD == "S60001").FirstOrDefault().MEASURE_ID);	// time lost days
									break;
								case "INJURY_TYPE":
								case "INJURY_PART":
								case "INJURY_TENURE":
								case "INJURY_CAUSE":
								case "INJURY_DAYS_TO_CLOSE":
									measureIDS.AddRange(measureList.Where(l => l.DATA_TYPE == "O").Select(l => l.MEASURE_ID).ToList());	// all ordinal measures
									break;
								default:
									if (vi.CALCS_SCOPE.Contains('|'))
									{
										string scopes = vi.CALCS_SCOPE.Replace("|", ",");
										foreach (string s in scopes.Split(','))
										{
											if (decimal.TryParse(s, out measureID))
												measureIDS.Add(measureID);
										}
									}
									else
									{
										if (decimal.TryParse(vi.CALCS_SCOPE, out measureID))
											measureIDS.Add(measureID);
									}
									break;
							}
						}
						metricMgr.CreateNew(SessionManager.PrimaryCompany(), localView.PERSPECTIVE, localCriteria.FromDate, localCriteria.ToDate, ViewModel.AddFromYear(localView), localCriteria.PlantArray, measureIDS.Distinct().ToArray());
					}
					else
					{
						metricMgr.CreateNew(SessionManager.PrimaryCompany(), localView.PERSPECTIVE, localCriteria.FromDate, localCriteria.ToDate, ViewModel.AddFromYear(localView), localCriteria.PlantArray);
					}
                     
					 metricMgr.Load(localCriteria.DateInterval, localCriteria.DateSpanType);
                    //if (UserContext.RoleAccess() >= AccessMode.Plant)
                          lnkExport.Visible = true;
						  if (context == "export")
						  {
							  uclProgress.ProgressComplete();
							  uclProgress.BindProgressDisplay(100, "Exporting...");
							  uclProgress.UpdateDisplay(2, 50, "Exporting...");

							  List<WebSiteCommon.DatePeriod> pdList;
							  if (localCriteria.DateSpanType == DateSpanOption.FYYearToDate)
							  {
								  pdList = WebSiteCommon.CalcDatePeriods(localCriteria.FromDate, localCriteria.ToDate, DateIntervalType.FYyear, localCriteria.DateSpanType, "");
								  List<MetricData> exportList = new List<MetricData>();
								  foreach (WebSiteCommon.DatePeriod pd in pdList)
								  {
									  exportList.AddRange(metricMgr.ehsCtl.MetricHst.Where(l => new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, 1) >= pd.FromDate && new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, DateTime.DaysInMonth(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH)) <= pd.ToDate).ToList());
								  }
								  uclExport.GenerateExportHistoryExcel(new PSsqmEntities(), exportList, false);
							  }
							  else
							  {
								  pdList = WebSiteCommon.CalcDatePeriods(localCriteria.FromDate, localCriteria.ToDate, DateIntervalType.year, localCriteria.DateSpanType, "");
								  uclExport.GenerateExportHistoryExcel(new PSsqmEntities(),
									  metricMgr.ehsCtl.MetricHst.Where(l => new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, 1) >= pdList.ElementAt(0).FromDate && new DateTime(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH, DateTime.DaysInMonth(l.MetricRec.PERIOD_YEAR, l.MetricRec.PERIOD_MONTH)) <= pdList.ElementAt(0).ToDate).ToList(),
									  false);
							  }

							  // uclExport.GenerateExportHistoryExcel(new PSsqmEntities(), metricMgr.ehsCtl.MetricHst, false);
							  uclProgress.ProgressComplete();
							  return 1;
						  }
                     break;
            }

            string[] plantNameArray;

            foreach (PERSPECTIVE_VIEW_ITEM vi in localView.PERSPECTIVE_VIEW_ITEM.Where(l=> l.STATUS != "I").OrderBy(l => l.ITEM_SEQ))
            {
                object controlData = null;
                try
                {
                    plantNameArray = localCriteria.PlantList.Where(l => localCriteria.PlantArray.Contains(l.Plant.PLANT_ID)).Select(l => l.Plant.PLANT_NAME).ToArray();

					// array of graphs
                    if (vi.DISPLAY_TYPE == 2)
                    {
                        if (!localCriteria.Options.HasFlag(DashboardOpts.TotalsOnly))
                        {
							int nitem = -1;
							bool showEmpty = true;
							decimal[] plantArray;
							switch (vi.FILTER)
							{
								case "B":	// business org
									foreach (BUSINESS_ORG org in localCriteria.PlantList.Select(l => l.BusinessOrg).Distinct().ToList())
									{
										plantArray = localCriteria.PlantList.Where(p => p.Plant.BUS_ORG_ID == org.BUS_ORG_ID).Select(p => p.Plant.PLANT_ID).ToArray();
										++nitem;
										ggCfg.ConfigureControl(vi, metricMgr.TargetsCtl, org.ORG_NAME, nitem == 0 ? true : false, 0, 0);
										status = uclGauge.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, metricMgr.CalcsMethods(plantArray, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER, vi.FILTER, vi.OPTIONS), "divDashboardArea");
									}
									break;
								case "L":	// location code  (country ?)
									foreach (string locationCode in localCriteria.PlantList.Select(l => l.Plant.LOCATION_CODE).Distinct().ToList())
									{
										if (string.IsNullOrEmpty(locationCode))
										{
											if (!showEmpty)
												continue;
											plantArray = localCriteria.PlantList.Where(p => p.Plant.LOCATION_CODE == "" || p.Plant.LOCATION_CODE == null).Select(p => p.Plant.PLANT_ID).ToArray();
											showEmpty = false;
										}
										else
											plantArray = localCriteria.PlantList.Where(p => p.Plant.LOCATION_CODE == locationCode).Select(p => p.Plant.PLANT_ID).ToArray();
										++nitem;
										ggCfg.ConfigureControl(vi, metricMgr.TargetsCtl, WebSiteCommon.GetXlatValueLong("countryCode", string.IsNullOrEmpty(locationCode) ? Resources.LocalizedText.Undefined : locationCode), nitem == 0 ? true : false, 0, 0);
										status = uclGauge.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, metricMgr.CalcsMethods(plantArray, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER, vi.FILTER, vi.OPTIONS), "divDashboardArea");
									}
									break;
								case "R":	// region
									foreach (string regionCode in localCriteria.PlantList.Select(l => l.Plant.COMP_INT_ID).Distinct().ToList())
									{
										if (string.IsNullOrEmpty(regionCode))
										{
											if (!showEmpty)
												continue;
											plantArray = localCriteria.PlantList.Where(p => p.Plant.COMP_INT_ID == "" || p.Plant.COMP_INT_ID == null).Select(p => p.Plant.PLANT_ID).ToArray();
											showEmpty = false;
										}
										else
											plantArray = localCriteria.PlantList.Where(p => p.Plant.COMP_INT_ID == regionCode).Select(p => p.Plant.PLANT_ID).ToArray();
										++nitem;
										ggCfg.ConfigureControl(vi, metricMgr.TargetsCtl, string.IsNullOrEmpty(regionCode) ? Resources.LocalizedText.Undefined : regionCode, nitem == 0 ? true : false, 0, 0);
										status = uclGauge.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, metricMgr.CalcsMethods(plantArray, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER, vi.FILTER, vi.OPTIONS), "divDashboardArea");
									}
									break;
								default:	// plant
									foreach (decimal plantID in localCriteria.PlantArray)
									{
										++nitem;
										ggCfg.ConfigureControl(vi, metricMgr.TargetsCtl, plantNameArray[nitem], nitem == 0 ? true : false, 0, 0);
										status = uclGauge.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, metricMgr.CalcsMethods(new decimal[1] { plantID }, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER, vi.FILTER, vi.OPTIONS), "divDashboardArea");
									}
									break;
							}
                        }
                    }
                    else
                    {
                        ggCfg.ConfigureControl(vi, metricMgr.TargetsCtl,  "", false, 0, 0);
                        if (vi.CONTROL_TYPE == 1 && localCriteria.Options.HasFlag(DashboardOpts.TotalsOnly))
                            ggCfg.NewRow = false;
						status = uclGauge.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, metricMgr.CalcsMethods(localCriteria.PlantArray, vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, vi.CONTROL_TYPE, (int)vi.SERIES_ORDER, vi.FILTER, vi.OPTIONS), "divDashboardArea");
                    }

                    MessageDisplay(null);
                }
                catch (Exception e)
                {
                    MessageDisplay(lblViewLoadError);
                   //SQMLogger.LogException(e);
                }
            }

            return 1;
        }

        #endregion

        #region gauges
 
        #endregion

        #region layout

        public int BindViewLayout(PERSPECTIVE_VIEW view, string oper)
        {
            int status = 0;
            if (view == null)
                return -1;

            hfViewStatus.Value = view.STATUS;

            if (localCriteria.MeasureList == null)
                localCriteria.MeasureList = EHSModel.SelectEHSMeasureList("", true);

            tbViewName.Text = view.VIEW_NAME;
            tbViewDesc.Text = view.VIEW_DESC;
            ddlViewAvailability.SelectedValue = view.AVAILABILTY.ToString();
            ddlDfltCriteria.SelectedValue = view.DFLT_OPTION.ToString();
            lblLastUpdate_out.Text = view.LAST_UPD_BY + "  " + SQMBasePage.FormatDate(view.LAST_UPD_DT, "d", true);
            if (oper == "new")
                ddlPerspective.Enabled = true;
            else
            {
                ddlPerspective.SelectedValue = view.PERSPECTIVE;
                ddlPerspective.Enabled = false;
            }

            BindViewItemsLayout(view);

            return status;
        }

        public int BindViewItemsLayout(PERSPECTIVE_VIEW view)
        {
            int status = 0;

            SetupScopeList(view.PERSPECTIVE);  // create metric scope pick lists
            rptViewItem.DataSource = view.PERSPECTIVE_VIEW_ITEM.OrderBy(l => l.ITEM_SEQ);
            rptViewItem.DataBind();

            return status;
        }


        private void FilterStatList(string perspective, RadComboBox ddlstat)
        {
            string statList = "";
            switch (perspective)
            {
                case "E":
                    statList = "sum,cost,sumCost";
                    break;
                case "EP":
                    statList = "pctChange";
                    break;
                case "HS":
                    statList = "sum";
                    break;
                case "HSP":
                    statList = "pctChange,deltaDy";
                    break;
                case "QS":
                    statList = "pctChange";
                    break;
                default:
                    break;
            }

            string[] sels = statList.Split(',');
            if (sels.Length > 0)
            {
                foreach (RadComboBoxItem item in ddlstat.Items)
                {
                    if (sels.Contains(item.Value))
                        item.Visible = true;
                    else
                        item.Visible = false;
                }
            }
        }

        private void SetupScopeList(string perspective)
        {
            RadComboBoxItem cbi = null;
            switch (perspective)
            {
                case "E":
                case "EP":
                    scopeList = new List<RadComboBoxItem>();
                    foreach (WebSiteCommon.SelectItem si in WebSiteCommon.PopulateListItems("statScopeE"))
                    {
                        if (string.IsNullOrEmpty(si.Value))
                        {
                            cbi = new RadComboBoxItem(si.Text, si.Text);
                            cbi.IsSeparator = true;
                            scopeList.Add(cbi);
                        }
                        else 
                            scopeList.Add(new RadComboBoxItem(si.Text, si.Value));   
                    }
                    foreach (System.Collections.Generic.KeyValuePair<string, string> xlat in WebSiteCommon.GetXlatList("measureCategoryEHS", "", "short"))
                    {
                        if (xlat.Key != "PROD" && xlat.Key != "SAFE")
                        {
                            cbi = new RadComboBoxItem(xlat.Value.ToUpper(), xlat.Key);
                            cbi.IsSeparator = true;
                            scopeList.Add(cbi);
                            foreach (EHS_MEASURE measure in localCriteria.MeasureList.Where(l => l.MEASURE_CATEGORY == xlat.Key).ToList())
                            {
                                string str = (measure.MEASURE_NAME.Trim());
                                cbi = new RadComboBoxItem(str, measure.MEASURE_ID.ToString());
                                scopeList.Add(cbi);
                            }
                        }
                    }
                    break;
                case "HS":
                case "HSP":
                    scopeList = new List<RadComboBoxItem>();
                    foreach (WebSiteCommon.SelectItem si in WebSiteCommon.PopulateListItems("statScopeHS"))
                    {
                        if (string.IsNullOrEmpty(si.Value))
                        {
                            cbi = new RadComboBoxItem(si.Text, si.Text);
                            cbi.IsSeparator = true;
                            scopeList.Add(cbi);
                        }
                        else 
                            scopeList.Add(new RadComboBoxItem(si.Text, si.Value));
                    }
                    foreach (System.Collections.Generic.KeyValuePair<string, string> xlat in WebSiteCommon.GetXlatList("measureCategoryEHS", "", "short"))
                    {
                        if (xlat.Key == "PROD" || xlat.Key == "SAFE")
                        {
                            cbi = new RadComboBoxItem(xlat.Value.ToUpper(), xlat.Key);
                            cbi.IsSeparator = true;
                            cbi.Enabled = false;
                            scopeList.Add(cbi);
                            foreach (EHS_MEASURE measure in localCriteria.MeasureList.Where(l => l.MEASURE_CATEGORY == xlat.Key).ToList())
                            {
                                string str = (measure.MEASURE_NAME.Trim());
                                cbi = new RadComboBoxItem(str, EHSModel.ConvertPRODMeasure(measure, 0).ToString());
                                scopeList.Add(cbi);
                            }
                        }
                    }

                    cbi = new RadComboBoxItem("INCIDENT TOPICS", "IN");
                    cbi.IsSeparator = true;
                    cbi.Enabled = false;
                    scopeList.Add(cbi);
                    foreach (INCIDENT_QUESTION topic in EHSIncidentMgr.SelectIncidentQuestionList(new decimal[] {12,13,62,63}))
                    {
                        cbi = new RadComboBoxItem(topic.QUESTION_TEXT.TrimEnd('?'), topic.INCIDENT_QUESTION_ID.ToString());
                        scopeList.Add(cbi);
                    }
                    break;
                case "QS":
                    scopeList = new List<RadComboBoxItem>();
                    foreach (WebSiteCommon.SelectItem si in WebSiteCommon.PopulateListItems("statScopeQS"))
                    {
                        if (string.IsNullOrEmpty(si.Value))
                        {
                            cbi = new RadComboBoxItem(si.Text, si.Text);
                            cbi.IsSeparator = true;
                            cbi.Enabled = false;
                            scopeList.Add(cbi);
                        }
                        else
                            scopeList.Add(new RadComboBoxItem(si.Text, si.Value));
                    }
                    break;
                default:
                    scopeList = new List<RadComboBoxItem>();
                    break;
            }
        }

        public int SaveView(bool commitChanges)
        {
            int status = 0;
            RadTextBox tb;
            RadComboBox cdl;
            PERSPECTIVE_VIEW_ITEM vi = null;
            decimal decVal = 0;

            MessageDisplay(null);
            
            if (commitChanges  &&  localView.STATUS != "N")
				localView = ViewModel.LookupView(localCtx, "", "", localView.VIEW_ID, SessionManager.UserContext.Language.NLS_LANGUAGE);

            localView.VIEW_NAME = tbViewName.Text;
            localView.VIEW_DESC = tbViewDesc.Text;
            localView.PERSPECTIVE = ddlPerspective.SelectedValue;
            localView.DFLT_OPTION = Convert.ToInt16(ddlDfltCriteria.SelectedValue);
            localView.AVAILABILTY = Convert.ToInt16(ddlViewAvailability.SelectedValue);
            localView.DFLT_TIMEFRAME = Convert.ToInt32(ddlDateSpan.SelectedValue);
            localView.DFLT_LOCATIONS = "";
            if (localView.DFLT_OPTION == 1 || localView.DFLT_OPTION == 2)
            {
                foreach (RadComboBoxItem plantitem in SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect))
                {
                    localView.DFLT_LOCATIONS += (plantitem.Value + ",");
                }
                localView.DFLT_LOCATIONS.TrimEnd(',');
            }

            int numVI = 0;
            foreach (RepeaterItem item in rptViewItem.Items)
            {
                vi = localView.PERSPECTIVE_VIEW_ITEM.ElementAtOrDefault(numVI++);

                cdl = (RadComboBox)item.FindControl("ddlVISeq");
                vi.ITEM_SEQ = Convert.ToInt16(cdl.SelectedItem.Text);

                cdl = (RadComboBox)item.FindControl("ddlVIGaugeType");
                vi.DISPLAY_TYPE = Convert.ToInt16(cdl.SelectedValue) > 200 ? 2 : 1;  // array or single gauge
                vi.CONTROL_TYPE = Convert.ToInt16(cdl.SelectedValue) > 200 ? Convert.ToInt16(cdl.SelectedValue) - 200 : Convert.ToInt16(cdl.SelectedValue);
                RadButton cb = (RadButton)item.FindControl("cbVINewRow");
                vi.NEW_ROW = cb.Checked;
 
                int intval = 0;
                tb = (RadTextBox)item.FindControl("tbVIHeight");
                if (int.TryParse(tb.Text, out intval))
                    vi.ITEM_HEIGHT = Math.Abs(intval);
                tb = (RadTextBox)item.FindControl("tbVIWidth");
                if (int.TryParse(tb.Text, out intval))
                    vi.ITEM_WIDTH = Math.Abs(intval);
                tb = (RadTextBox)item.FindControl("tbVITitle");
                vi.TITLE = tb.Text;

                cdl = (RadComboBox)item.FindControl("ddlVIStat");
                vi.CALCS_STAT = cdl.SelectedValue;
                cdl = (RadComboBox)item.FindControl("ddlVIScope");
                vi.CALCS_SCOPE = "";
                foreach (RadComboBoxItem cdi in cdl.Items)
                {
                    if (cdi.Checked ||  cdi.Selected)
                    {
                        if (vi.CALCS_SCOPE.Length > 0)
                            vi.CALCS_SCOPE += ",";
                        vi.CALCS_SCOPE += cdi.Value;
                    }
                }

                cdl = (RadComboBox)item.FindControl("ddlVISeriesOrder");
                vi.SERIES_ORDER = Convert.ToInt32(cdl.SelectedValue);

                tb = (RadTextBox)item.FindControl("tbVIXAxisLabel");
                vi.SCALE_LABEL = tb.Text;
                cdl = (RadComboBox)item.FindControl("ddlVIXAxisScale");
                if (cdl.SelectedIndex == 0)
                    vi.SCALE_MIN = vi.SCALE_MAX = vi.SCALE_UNIT = 0;
                else
                {
                    tb = (RadTextBox)item.FindControl("tbVIXAxisMin");
                    if (decimal.TryParse(tb.Text, out decVal))
                        vi.SCALE_MIN = decVal;
                    tb = (RadTextBox)item.FindControl("tbVIXAxisMax");
                    if (decimal.TryParse(tb.Text, out decVal))
                        vi.SCALE_MAX = decVal;
                    tb = (RadTextBox)item.FindControl("tbVIXAxisUnit");
                    if (decimal.TryParse(tb.Text, out decVal))
                        vi.SCALE_UNIT = decVal;
                }

                cdl = (RadComboBox)item.FindControl("ddlVITarget");
                if (cdl.SelectedIndex > 0)
                    vi.DISPLAY_TARGET_ID = Convert.ToDecimal(cdl.SelectedValue);
                else
                    vi.DISPLAY_TARGET_ID = 0;

                tb = (RadTextBox)item.FindControl("tbVIYAxisLabel");
                vi.A_LABEL = tb.Text;
            }

            if (commitChanges)
            {
                localView = ViewModel.UpdateView(localCtx, localView, SessionManager.UserContext.UserName());
                if (localView == null)
                    MessageDisplay(LblViewSaveError);
                else
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
            }

            return status;
        }

        protected void ddlPerspectiveChange(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            localView.PERSPECTIVE = ddlPerspective.SelectedValue;
        }

        protected void ddlVIStatChange(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox sender = (RadComboBox)o;
            RadComboBox cdl = (RadComboBox)sender.Parent.FindControl("ddlVIScope");
            if (cdl == null)
                return;

            cdl.Enabled = true;
            switch (localView.PERSPECTIVE)
            {
                case "E":
                    if (sender.SelectedValue.Contains("eeff") || sender.SelectedValue.Contains("eSeff")  ||  sender.SelectedValue.Contains("o2"))
                    {
                        foreach (RadComboBoxItem item in cdl.Items)
                            item.Checked = false;
                        cdl.Items.FindItemByValue("ENGY").Checked = true;
                    }
                    break;
                case "HS":
                    if (sender.SelectedValue == "rcr" || sender.SelectedValue == "ltcr"  ||  sender.SelectedValue == "ltsr")
                    {
                        foreach (RadComboBoxItem item in cdl.Items)
                            item.Checked = false;
                        cdl.Items.FindItemByValue("PROD").Checked = true;
                        cdl.Enabled = false;
                    }
                    if (sender.SelectedValue == "deltaDy")
                       foreach (RadComboBoxItem item in cdl.Items)
                            item.Checked = false;
                        cdl.Items.FindItemByValue("63").Checked = true;
                        break;
                    break;
                default: break;
            }
        }

        protected void ddlVIScaleChange(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox sender = (RadComboBox)o;
            RadTextBox tb1 = (RadTextBox)sender.Parent.FindControl("tbVIXAxisMin");
            RadTextBox tb2 = (RadTextBox)sender.Parent.FindControl("tbVIXAxisMax");
            RadTextBox tb3 = (RadTextBox)sender.Parent.FindControl("tbVIXAxisUnit");
            if (sender.SelectedValue == "0")
            {
                tb1.Text = tb2.Text = tb3.Text = "";
                tb1.Enabled = tb2.Enabled = tb3.Enabled = false;
            }
            else
            {
                tb1.Enabled = tb2.Enabled = tb3.Enabled = true;
            }
        }

        public void rptViewItem_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
				(e.Item.FindControl("lblTarget") as Label).Text = Resources.LocalizedText.Target + ":";

				try
                {
                    PERSPECTIVE_VIEW_ITEM vi = (PERSPECTIVE_VIEW_ITEM)e.Item.DataItem;
                    DropDownList ddl;
                    RadComboBox cdl;
                    RadComboBoxItem cbi;
                    RadTextBox tb;
                    int nc = 0;

                    cdl = (RadComboBox)e.Item.FindControl("ddlVISeq");
                    for (int n=1; n<=50; n++)
                        cdl.Items.Add(new RadComboBoxItem(n.ToString(),n.ToString()));
                    cdl.SelectedValue = vi.ITEM_SEQ.ToString();
                    if (vi.STATUS == "N")
                        cdl.Focus();

                    if (vi.ITEM_HEIGHT > 0)
                    {
                        tb = (RadTextBox)e.Item.FindControl("tbVIHeight");
                        tb.Text = vi.ITEM_HEIGHT.ToString();
                    }
                    if (vi.ITEM_WIDTH > 0)
                    {
                        tb = (RadTextBox)e.Item.FindControl("tbVIWidth");
                        tb.Text = vi.ITEM_WIDTH.ToString();
                    }

                    tb = (RadTextBox)e.Item.FindControl("tbVITitle");
                    tb.Text = vi.TITLE;

                    cdl = (RadComboBox)e.Item.FindControl("ddlVIGaugeType");
                    int val = vi.CONTROL_TYPE;
                    if (vi.DISPLAY_TYPE == 2)  // array
                        val = val + 200;
                    if (cdl.Items.FindItemByValue(val.ToString()) != null)
                        cdl.SelectedValue = val.ToString();
                    else
                        cdl.SelectedIndex = 0;

                    RadButton cb = (RadButton)e.Item.FindControl("cbVINewRow");
                    cb.Checked = vi.NEW_ROW;

                    cdl = (RadComboBox)e.Item.FindControl("ddlVIStat");
                    FilterStatList(localView.PERSPECTIVE, cdl);

                    if (cdl.Items.FindItemByValue(vi.CALCS_STAT) != null)
                        cdl.SelectedValue = vi.CALCS_STAT;

                    cdl = (RadComboBox)e.Item.FindControl("ddlVIScope");
                    if (scopeList != null && scopeList.Count > 0)
                    {
                        cdl.CheckBoxes = true;
                        cdl.Items.Clear();
                        foreach (RadComboBoxItem ci in scopeList)
                        {
                            cbi = new RadComboBoxItem(ci.Text, ci.Value);
                            cbi.IsSeparator = ci.IsSeparator;
                            cbi.Enabled = ci.Enabled;
                            cdl.Items.Add(cbi);
                        }
                    }
                    else
                    {
                        cdl.CheckBoxes = false;
                    }
 
                    string[] scopeSels = vi.CALCS_SCOPE.Split(',');
                    foreach (string sel in scopeSels)
                    {
                        if ((cbi = cdl.Items.FindItemByValue(sel)) != null)
                        {
                            if (cdl.CheckBoxes)
                                cbi.Checked = true;
                            else
                                cbi.Selected = true;
                        }
                    }
  
                    cdl = (RadComboBox)e.Item.FindControl("ddlVISeriesOrder");
                    if (cdl.Items.FindItemByValue(vi.SERIES_ORDER.ToString()) != null)
                        cdl.SelectedValue = vi.SERIES_ORDER.ToString();

                    if (!string.IsNullOrEmpty(vi.SCALE_LABEL))
                    {
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisLabel");
                        tb.Text = vi.SCALE_LABEL;
                    }
                    if (!string.IsNullOrEmpty(vi.A_LABEL))
                    {
                        tb = (RadTextBox)e.Item.FindControl("tbVIYAxisLabel");
                        tb.Text = vi.A_LABEL;
                    }

                    cdl = (RadComboBox)e.Item.FindControl("ddlVIXAxisScale");
                    if (vi.SCALE_MIN.HasValue && vi.SCALE_MAX.HasValue && vi.SCALE_MIN != vi.SCALE_MAX)     // scales set
                    {
                        cdl.SelectedIndex = 1;
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisMin");
                        tb.Text = SQMBasePage.FormatValue((decimal)vi.SCALE_MIN, 5, true);
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisMax");
                        tb.Text = SQMBasePage.FormatValue((decimal)vi.SCALE_MAX, 5, true);
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisUnit");
                        tb.Text = SQMBasePage.FormatValue((decimal)vi.SCALE_UNIT, 5, true);
                    }
                    else  //  auto scale
                    {
                        cdl.SelectedIndex = 0;
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisMin"); tb.Text = "";
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisMax"); tb.Text = "";
                        tb = (RadTextBox)e.Item.FindControl("tbVIXAxisUnit"); tb.Text = "";
                    }

                    cdl = (RadComboBox)e.Item.FindControl("ddlVITarget");
                    if (cdl.Items.FindItemByValue(vi.DISPLAY_TARGET_ID.ToString()) != null)
                        cdl.SelectedValue = vi.DISPLAY_TARGET_ID.ToString();
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}