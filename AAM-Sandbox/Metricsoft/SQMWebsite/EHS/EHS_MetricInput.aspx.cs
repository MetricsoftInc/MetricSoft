using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class EHS_MetricInput : SQMBasePage
    {
        public decimal directedPlantID
        {
            get { return ViewState["isDirected"] == null ? 0 : (decimal)ViewState["isDirected"]; }
            set { ViewState["isDirected"] = value; }
        }

        #region events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
  
            uclInputHdr.OnProfilePeriodClick += OnInputPeriodSelect;
            uclInputHdr.OnPlantSelect += OnPlantProfileSelect;

            hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DateTime directedDate = DateTime.Now;
                SetupPage();
                if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
                {   // page invoked from users inbox re: missing required inputs
                    string[] args = SessionManager.ReturnObject.ToString().Split('~');
                    if (args.Length > 2)
                    {
                        directedPlantID = Convert.ToDecimal(args[0]);
                        directedDate = new DateTime(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), 1);
                        OnPlantProfileSelect(Convert.ToDecimal(args[0]));
                        uclInputHdr.LoadProfileInputHdr(false, directedDate, directedDate, Convert.ToDecimal(args[0]), true, false);
                        LoadProfileInput(directedDate, EHSProfileStatus.Normal);
                    }
                    SessionManager.ClearReturns();
                }
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

        protected void lnkExport_Click(object sender, EventArgs e)
        {
            if (LocalProfile() != null && LocalProfile().Plant != null)
            {
                uclExport.ExportProfileInputsExcel(entities, LocalProfile().Plant.PLANT_ID.ToString(), LocalProfile().InputPeriod.PeriodDate.AddMonths(-3), LocalProfile().InputPeriod.PeriodDate);
            }
        }

        protected void OnCancel_Click(object sender, EventArgs e)
        {
            if (directedPlantID > 0 && !string.IsNullOrEmpty(SessionManager.ReturnPath))
                Response.Redirect(SessionManager.ReturnPath);

            ClearInput();
        }

        protected void ClearInput()
        {
            //SetLocalProfile(null);
            divProfilePeriodScrollRepeater.Visible = false;
            uclInputHdr.ClearPeriodDate();
            cbFinalApproval.Checked = false;
            lblFinalApprovalBy.Text = "";
            btnSave1.Enabled = btnSave2.Enabled = btnCancel1.Enabled = btnCancel2.Enabled = false;
        }

        protected void OnPlantProfileSelect(decimal plantID)
        {
            ClearInput();

            if (directedPlantID > 0)  // override plant select ddl when directed from inbox, etc..
                SetLocalProfile(new EHSProfile().Load(directedPlantID, false, true));
            else
                SetLocalProfile(new EHSProfile().Load(plantID, false, true));

            MessageDisplay(LocalProfile().Status);
            if (LocalProfile().Status != EHSProfileStatus.Normal)
            {
                return;  
            }

			if (UserContext.GetMaxScopePrivilege(SysScope.envdata) < SysPriv.originate)  // allow approvers and admins to see all metrics
            {
                LocalProfile().FilterByResponsibleID = 0;
                LocalProfile().MinPeriodDate = new DateTime(2001, 1, 1);
                uclInputHdr.LoadProfileInputHdr(false, LocalProfile().MinPeriodDate, DateTime.Now, SessionManager.UserContext.HRLocation.Plant.PLANT_ID, true, true);
                    
            }
            else
            {
                LocalProfile().FilterByResponsibleID = SessionManager.UserContext.Person.PERSON_ID;
                SETTINGS sets = SQMSettings.GetSetting("EHS", "INPUTLIMIT");
                LocalProfile().MinPeriodDate = DateTime.Now.AddMonths(sets != null ? Convert.ToInt32(sets.VALUE) * -1 : -5);
                uclInputHdr.LoadProfileInputHdr(false, LocalProfile().MinPeriodDate, DateTime.Now, SessionManager.UserContext.HRLocation.Plant.PLANT_ID, true, true);
            }

            if (directedPlantID > 0)  // override plant select ddl when directed from inbox, etc...
                uclInputHdr.SelectLocation(LocalProfile().Plant);

            if (LocalProfile() != null && LocalProfile().Status == EHSProfileStatus.Normal)
                OnInputPeriodSelect(uclInputHdr.SelectedPeriodDate);
          
        }

        protected void OnInputPeriodSelect(DateTime periodDate)
        {
            EHSProfileStatus selectStatus = EHSProfileStatus.Normal;
            if (new DateTime(periodDate.Year, periodDate.Month, 1) == new DateTime(LocalProfile().MinPeriodDate.Year, LocalProfile().MinPeriodDate.Month, 1))
            {
                selectStatus = EHSProfileStatus.PeriodLimit;
            }

            if (LocalProfile() != null && LocalProfile().Status == EHSProfileStatus.Normal)
            {
                LoadProfileInput(periodDate, selectStatus);
            }
            else
                uclInputHdr.TriggerPlantInputSelected();
        }

        protected void OnSave_Click(object sender, EventArgs e)
        {
            SaveInputs(0, true);
        }

        #endregion

        #region input
        private void MessageDisplay(EHSProfileStatus status)
        {
            lblInputError.Visible = lblPeriodLimit.Visible = lblProfileNotExist.Visible = lblNoMetrics.Visible = lblNoInputs.Visible = lblRangeWarning.Visible = lblIncompleteInputs.Visible = false;
            uclExport.Visible = phApproval.Visible = true;

            // suppress local approval input when auto-rollup settings are active
            SETTINGS sets = SQMSettings.GetSetting("EHS", "INPUTFINALIZE");
            if (sets != null &&  !string.IsNullOrEmpty(sets.VALUE))
            {
                phApproval.Visible = false;
            }

            if (LocalProfile() != null)
            {
                LocalProfile().CurrentStatus = status;
                BindSharedCalendars();
            }
            switch (status)
            {
                case EHSProfileStatus.NonExist:
                    lblProfileNotExist.Visible = true;
                    uclExport.Visible = phApproval.Visible = false;
                    break;
                case EHSProfileStatus.PeriodLimit:
                    lblPeriodLimit.Visible = true;
                    break;
                case EHSProfileStatus.InputError:
                    lblInputError.Visible = true;
                    break;
                case EHSProfileStatus.NoMeasures:
                    lblNoMetrics.Visible = true;
                    uclExport.Visible = phApproval.Visible =  false;
                    break;
                case EHSProfileStatus.NoInputs:
                    lblNoInputs.Visible = true;
                    break;
                case EHSProfileStatus.OutOFRange:
                    lblRangeWarning.Visible = true;
                    break;
                case EHSProfileStatus.Incomplete:
                    lblIncompleteInputs.Visible = true;
                    break;
                default:
                    break;
            }
        }

        public void LoadProfileInput(DateTime targetDate, EHSProfileStatus selectStatus)
        {
			UserContext.CheckUserPrivilege(SysPriv.originate, SysScope.envdata);
            LocalProfile().LoadPeriod(targetDate);
            if (LocalProfile().InputPeriod != null)
            {
                LocalProfile().MapPlantAccountingInputs(true, false);

                uclInputHdr.BindProfileInputHdr(LocalProfile());
				EHSProfileStatus status = BindProfileInputList(LocalProfile().MeasureList(true, UserContext.GetScopePrivileges(SysScope.envdata)).ToList(), LocalProfile().Profile);
                if (status == EHSProfileStatus.Normal)
                {
                    if (LocalProfile().Profile.APPROVER_ID == SessionManager.UserContext.Person.PERSON_ID || UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.envdata))
                    {
                        cbFinalApproval.Enabled = true;
                    }
                    if (LocalProfile().InputPeriod.PlantAccounting.APPROVER_ID.HasValue && LocalProfile().InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                    {
                        cbFinalApproval.Checked = true;
                        cbFinalApproval.Enabled = false;
                        hfWasApproved.Value = lblFinalApprovalBy.Text = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)LocalProfile().InputPeriod.PlantAccounting.APPROVER_ID, ""));
                        lblFinalApprovalBy.Text += ("  " + SQMBasePage.FormatDate((DateTime)LocalProfile().InputPeriod.PlantAccounting.APPROVAL_DT, "", false));
                    }
                    else
                    {
                        cbFinalApproval.Checked = false;
                        hfWasApproved.Value = lblFinalApprovalBy.Text = "";
                    }
                }
                MessageDisplay(selectStatus == EHSProfileStatus.Normal ? status : selectStatus);
                btnSave1.Enabled = btnSave2.Enabled = btnCancel1.Enabled = btnCancel2.Enabled = UserContext.GetMaxScopePrivilege(SysScope.envdata) < SysPriv.notify ? true : false;
            }
            else
            {
                MessageDisplay(EHSProfileStatus.NoInputs);
            }
        }

        protected EHSProfileStatus BindProfileInputList(List<EHS_PROFILE_MEASURE> metricList, EHS_PROFILE profile)
        {
            lblProfileNotExist.Visible = false;
            hfNumDelete.Value = hfNumChanged.Value = "0";

            if (metricList.Count == 0)
            {
                return EHSProfileStatus.NoMeasures;
            }
            else
            {
                divProfilePeriodScrollRepeater.Visible = true;
                switch (profile.DISPLAY_OPTION)
                {
                    case 1:
                        rptProfilePeriod.DataSource = metricList.OrderBy(l => l.EHS_MEASURE.MEASURE_CD);
                        break;
                    case 2:
                        rptProfilePeriod.DataSource = metricList.OrderBy(l => l.EHS_MEASURE.MEASURE_NAME);
                        break;
                    default:
                        rptProfilePeriod.DataSource = metricList.OrderBy(l => l.EHS_MEASURE.MEASURE_CATEGORY).ThenBy(l => l.EHS_MEASURE.MEASURE_CD);
                        break;
                }
                rptProfilePeriod.DataBind();
            }
            return EHSProfileStatus.Normal;
        }

        private void SetupPage()
        {
            uclExport.BindExport("", false, hfExportText.Value);

            btnSave1.Enabled = btnSave2.Enabled = UserContext.GetMaxScopePrivilege(SysScope.envdata) < SysPriv.notify ? true : false;
            if (LocalProfile() == null || IsCurrentPage() == false)
            {
                btnSave1.Enabled = btnSave2.Enabled = false;
            }

            uclInputHdr.LoadProfileInputHdr(true, DateTime.MinValue, DateTime.MinValue, SessionManager.UserContext.HRLocation.Plant.PLANT_ID, true, true);
        }

        protected void lnkMetricInput_Click(object sender, EventArgs e)
        {
            string cmdID = "";
            LinkButton lnk;
            if (sender.GetType().ToString().ToUpper().Contains("LINK"))
            {
                lnk = (LinkButton)sender;
                cmdID = lnk.CommandArgument;
            }
            else
            {
                ImageButton btn = (ImageButton)sender;
                cmdID = btn.CommandArgument;
            }

            EHS_PROFILE_INPUT input = LocalProfile().InputPeriod.InputsList[0];
            input.LAST_UPD_DT = DateTime.UtcNow;
            EHSProfile.UpdateProfile(LocalProfile());

            foreach (RepeaterItem item in rptProfilePeriod.Items)
            {
                lnk = (LinkButton)item.FindControl("lnkMetricCD");
                if (lnk.CommandArgument == cmdID)
                {
                    SaveInputs(Convert.ToDecimal(cmdID), false);

                    Repeater rpt = (Repeater)item.FindControl("rptProfileInput");
                    EHS_PROFILE_MEASURE metric = LocalProfile().GetMeasure(Convert.ToDecimal(lnk.CommandArgument));
                    LocalProfile().CreatePeriodInput(LocalProfile().InputPeriod, metric, true);
                    rpt.DataSource = LocalProfile().InputPeriod.GetPeriodInputList(metric.PRMR_ID);
                    rpt.DataBind();
                }
            }

            BindSharedCalendars();
        }

        private int SaveInputs(decimal targetPrmrID, bool commitChanges)
        {
            int status = 0;
            LinkButton lnk;
            HiddenField hf;
            TextBox tbValue, tbCost, tbCredit;
            CheckBox cbDelete;
            decimal decimalValue;
            DateTime dateValue;
            decimal prmrID;
            bool hasReqdInputs = true;
            bool hasSaveWarning = false;
            bool hasSaveError = false;
            EHSProfileStatus saveStatus = EHSProfileStatus.Normal;

            try
            {
                foreach (RepeaterItem item in rptProfilePeriod.Items)
                {
                    lnk = (LinkButton)item.FindControl("lnkMetricCD");
                    prmrID = Convert.ToDecimal(lnk.CommandArgument);
                    EHS_PROFILE_MEASURE metric = LocalProfile().GetMeasure((decimal)prmrID);

                    if (prmrID == targetPrmrID || commitChanges)
                    {
                        Repeater rpt = (Repeater)item.FindControl("rptProfileInput");
                        int numInput = 0;
                        foreach (RepeaterItem inputItem in rpt.Items)
                        {
                            hf = (HiddenField)inputItem.FindControl("hfInputDate");
                            EHS_PROFILE_INPUT input = LocalProfile().InputPeriod.GetPeriodInput(prmrID, Convert.ToDateTime(hf.Value));
                            if (input != null)
                            {
                                RadDatePicker dtpFrom = (RadDatePicker)inputItem.FindControl("radDateFrom");
                                RadDatePicker dtpTo = (RadDatePicker)inputItem.FindControl("radDateTo");
                                tbValue = (TextBox)inputItem.FindControl("tbMetricValue");
                                tbCost = (TextBox)inputItem.FindControl("tbMetricCost");
                                tbCredit = (TextBox)inputItem.FindControl("tbMetricCredit");
                                cbDelete = (CheckBox)inputItem.FindControl("cbDelete");
                                hf = (HiddenField)inputItem.FindControl("hfStatus");
                                Control tr = new Control();

                                EHSProfileStatus inputStatus = EHSProfileStatus.Normal;
                                switch (metric.EHS_MEASURE.MEASURE_CATEGORY)
                                {
                                    case "PROD":
                                    case "SAFE":
                                    case "FACT":
                                        if (dtpFrom.SelectedDate == null || dtpTo.SelectedDate == null)
                                            inputStatus = EHSProfileStatus.Incomplete;
                                        else if (string.IsNullOrEmpty(tbValue.Text.Trim()))
                                            inputStatus = EHSProfileStatus.NoInputs;
                                        break;
                                    default:
                                        if (string.IsNullOrEmpty(tbValue.Text.Trim()))
                                        {
                                            if (string.IsNullOrEmpty(tbCost.Text.Trim() + tbCredit.Text.Trim()))
                                                inputStatus = EHSProfileStatus.NoInputs;
                                            else
                                                inputStatus = EHSProfileStatus.Incomplete;
                                        }
                                        else if (string.IsNullOrEmpty(tbCost.Text.Trim() + tbCredit.Text.Trim()))
                                        {
                                            inputStatus = EHSProfileStatus.Incomplete;
                                        }
                                        else if (dtpFrom.SelectedDate == null || dtpTo.SelectedDate == null)
                                        {
                                            inputStatus = EHSProfileStatus.Incomplete;
                                        }
                                       
                                        if (metric.EHS_PROFILE_MEASURE_EXT != null  &&  (metric.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue || metric.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue))
                                        {
                                            if (inputStatus == EHSProfileStatus.Incomplete  &&  (dtpFrom.SelectedDate == null || dtpTo.SelectedDate == null))
                                                inputStatus = EHSProfileStatus.NoInputs;
                                        }
                                       
                                        break;
                                }

                                if (inputStatus == EHSProfileStatus.NoInputs)
                                {
                                    LocalProfile().InputPeriod.DeletePeriodInput(input);
                                }
                                else if (inputStatus == EHSProfileStatus.Incomplete)
                                {
                                    hasSaveError = true;
                                    saveStatus = EHSProfileStatus.Incomplete;
                                    dtpFrom.DateInput.Style.Add("BACKGROUND-COLOR", "LIGHTCORAL");
                                    dtpTo.DateInput.Style.Add("BACKGROUND-COLOR", "LIGHTCORAL");
                                    tbValue.Style.Add("BACKGROUND-COLOR", "LIGHTCORAL");
                                    if (tbCost.Enabled)
                                        tbCost.Style.Add("BACKGROUND-COLOR", "LIGHTCORAL");
                                    if (tbCredit.Enabled)
                                        tbCredit.Style.Add("BACKGROUND-COLOR", "LIGHTCORAL");
                                }

                                //if ((string.IsNullOrEmpty(tbValue.Text) || dtpFrom.SelectedDate == null || dtpTo.SelectedDate == null) || (string.IsNullOrEmpty(tbCost.Text + tbCredit.Text) && metric.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && metric.EHS_MEASURE.MEASURE_CATEGORY != "SAFE" && metric.EHS_MEASURE.MEASURE_CATEGORY != "FACT"))
                                //{
                                //    LocalProfile().InputPeriod.DeletePeriodInput(input);
                                //}
                                else
                                {
                                    ++numInput;

                                    if (cbDelete.Checked)
                                        input.STATUS = "D";
                                    else if (input.STATUS == "D")
                                        input.STATUS = "A";

                                    if (input.EFF_FROM_DT > DateTime.MinValue || input.EFF_FROM_DT != dtpFrom.SelectedDate)
                                        input.EFF_FROM_DT = (DateTime)dtpFrom.SelectedDate;

                                    if (input.EFF_TO_DT > DateTime.MinValue || input.EFF_TO_DT != dtpTo.SelectedDate)
                                        input.EFF_TO_DT = (DateTime)dtpTo.SelectedDate;

                                    if (SQMBasePage.ParseToDecimal(tbValue.Text, out decimalValue))
                                    {
                                        if (input.MEASURE_VALUE != decimalValue)
                                            input.MEASURE_VALUE = decimalValue;
                                    }
                                    if (!string.IsNullOrEmpty(tbCredit.Text))
                                    {
                                        SQMBasePage.ParseToDecimal(tbCredit.Text, out decimalValue);
                                        decimalValue = Math.Abs(decimalValue) * -1;
                                        if (!input.MEASURE_COST.HasValue || input.MEASURE_COST != decimalValue)
                                            input.MEASURE_COST = decimalValue;
                                    }
                                    else
                                    {
                                        SQMBasePage.ParseToDecimal(tbCost.Text, out decimalValue);
                                        decimalValue = Math.Abs(decimalValue);
                                        if (!input.MEASURE_COST.HasValue || input.MEASURE_COST != decimalValue)
                                            input.MEASURE_COST = decimalValue;
                                    }

                                    if (commitChanges && metric.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && metric.EHS_MEASURE.MEASURE_CATEGORY != "SAFE" && metric.EHS_MEASURE.MEASURE_CATEGORY != "FACT" && LocalProfile().CurrentStatus != EHSProfileStatus.OutOFRange)
                                    {
                                        if (!EHSModel.IsMeasureValueInRange(metric, (double)input.MEASURE_VALUE, 1.0))
                                        {
                                            hasSaveWarning = true;
                                            saveStatus = EHSProfileStatus.OutOFRange;
                                            tbValue.Style.Add("BACKGROUND-COLOR", "CORNSILK");
                                        }
                                    }
                                }
                            }
                            if (metric != null && (bool)metric.IS_REQUIRED && numInput == 0)
                                hasReqdInputs = false;
                        }
                    }
                }

                if (commitChanges)
                {
                    if (hasSaveError)
                    {
                        MessageDisplay(saveStatus);
                        return 0;
                    }

                    if (hasSaveWarning)
                    {
                        MessageDisplay(saveStatus);
                        return 0;
                    }

                    // deleted inputs after approval
                    if ((!string.IsNullOrEmpty(hfNumDelete.Value)  &&  hfNumDelete.Value != "0") && LocalProfile().InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                    {
                        cbFinalApproval.Checked = false;
                    }
                    // changed inputs after approval
                    if ((!string.IsNullOrEmpty(hfNumChanged.Value) && hfNumChanged.Value != "0") && LocalProfile().InputPeriod.PlantAccounting.APPROVAL_DT.HasValue)
                    {
                        cbFinalApproval.Checked = false;
                    }
                   
                    status = LocalProfile().UpdatePeriod(true, "", cbFinalApproval.Checked, SessionManager.UserContext.UserName());

                    if (status >= 0)
                    {
                        // option to finalize metrics
                        SETTINGS sets = SQMSettings.GetSetting("EHS", "INPUTFINALIZE");
                        if (sets != null)
                        {
                            bool doRollup = false;
                            DateTime lastUpdateDate;
                            LocalProfile().PeriodStatus(new string[0] { }, false, out lastUpdateDate);
                            switch (sets.VALUE.ToUpper())
                            {
                                case "ANY":     // finalize any inputs
                                    doRollup = true;
                                    break;
                                case "ANY_CURRENCY":       // finalize any inputs and if the exchange rate for the period exists
                                    if (LocalProfile().InputPeriod.PeriodExchangeRate(LocalProfile().Plant) != null)
                                        doRollup = true;
                                    break;
                                case "REQD":    // finalize only when all required inputs have been entered
                                    if (LocalProfile().InputPeriod.IsRequiredComplete())
                                        doRollup = true;
                                    break;
                                case "REQD_CURRENCY":       // finalize only when all required inputs are entered and exchange rate for the period exists
                                    if (LocalProfile().InputPeriod.IsRequiredComplete()  &&  LocalProfile().InputPeriod.PeriodExchangeRate(LocalProfile().Plant) != null)                                   
                                        doRollup = true;
                                    break;
                                default:
                                    break;
                            }
                            if (doRollup  &&  LocalProfile().ValidPeriod())
                            {
                                status = LocalProfile().UpdateMetricHistory(LocalProfile().InputPeriod.PeriodDate);  // new roll-up logic 
                            }
                        }
                    }

                    if (status >= 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                        hasSaveWarning = false;  // cancel warning to allow re-save 
                        MessageDisplay(0);
                        LoadProfileInput(LocalProfile().InputPeriod.PeriodDate, EHSProfileStatus.Normal);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveError');", true);
                        MessageDisplay(0);
                        ClearInput();
                    }
                }
            }
            catch (Exception ex)
            {
             //   SQMLogger.LogException(ex);
                status = -1;
            }

            BindSharedCalendars();

            return status;
        }

        private void BindSharedCalendars()
        {
            foreach (RepeaterItem item in rptProfilePeriod.Items)
            {
                // for some reason we need to rebind the shared calendar control to all the metric calendars ??
                Repeater rpt = (Repeater)item.FindControl("rptProfileInput");
                foreach (RepeaterItem msi in rpt.Items)
                {
                    sharedCalendar.Visible = true;
                    RadDatePicker dtp = (RadDatePicker)msi.FindControl("radDateFrom");
                    dtp.SharedCalendar = sharedCalendar;
                    dtp = (RadDatePicker)msi.FindControl("radDateTo");
                    dtp.SharedCalendar = sharedCalendar;
                }
            }
        }

        public void rptProfilePeriod_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHS_PROFILE_MEASURE metric = (EHS_PROFILE_MEASURE)e.Item.DataItem;
                    Label lbl;
                    LinkButton lnk;
                    TextBox tb;

                    //bool enabled = LocalProfile().InputPeriod.IsPeriodInputClosed(metric.PRMR_ID) == true ? false : true;
                    bool enabled = true;

                    if (string.IsNullOrEmpty(metric.MEASURE_PROMPT))
                    {
                        lbl = (Label)e.Item.FindControl("lblMetricName");
                        lbl.Text = metric.EHS_MEASURE.MEASURE_NAME.Trim();
                    }

                    ImageButton ib = (ImageButton)e.Item.FindControl("ibAddInput");
                    ib.Enabled = enabled;
                    if (enabled)
                        ib.ImageUrl = "~/images/plus.png";
                    else
                        ib.ImageUrl = "~/images/status/checked.png";

                    Image img = (Image)e.Item.FindControl("imgHazardType");
                    System.Web.UI.HtmlControls.HtmlTableCell cell1 = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdMetricName");
                    if (metric.EHS_MEASURE.MEASURE_CATEGORY == "ENGY"  ||  metric.EHS_MEASURE.MEASURE_CATEGORY == "EUTL")
                    {
                        cell1.Attributes.Add("Class", "rptInputTable energyColor");
                        img.ImageUrl = "~/images/status/energy.png";
                    }
                    else if (metric.EHS_MEASURE.MEASURE_CATEGORY == "PROD" || metric.EHS_MEASURE.MEASURE_CATEGORY == "SAFE" || metric.EHS_MEASURE.MEASURE_CATEGORY == "FACT")
                    {
                        img.ImageUrl = "~/images/status/inputs.png";
                        img.ToolTip = WebSiteCommon.GetXlatValueLong("measureCategoryEHS", metric.EHS_MEASURE.MEASURE_CATEGORY);
                        ib = (ImageButton)e.Item.FindControl("ibAddInput");
                        ib.Visible = false;
                    }
                    else 
                    {
                        cell1.Attributes.Add("Class", "rptInputTable wasteColor");
                        if (metric.REG_STATUS == "HZ")
                        {
                            img.ImageUrl = "~/images/status/hazardous.png";
                        }
                        else
                        {
                            img.ImageUrl = "~/images/status/waste.png";
                        }
                        img.ToolTip = WebSiteCommon.GetXlatValueLong("regulatoryStatus", metric.REG_STATUS);
                        if (!string.IsNullOrEmpty(metric.UN_CODE))
                            img.ToolTip += (".  " + SessionManager.DisposalCodeList.FirstOrDefault(l => l.UN_CODE == metric.UN_CODE).DESCRIPTION);
                    }

                    cell1 = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("tdMetricReqd");
                    if ((bool)metric.IS_REQUIRED)
                        cell1.Attributes.Add("Class", "rptInputTable required");

                    if (LocalProfile().InputPeriod.CountPeriodInputList(metric.PRMR_ID) == 0)
                        LocalProfile().CreatePeriodInput(LocalProfile().InputPeriod, metric, false);

                    Repeater rpt = (Repeater)e.Item.FindControl("rptProfileInput");
                    rpt.DataSource = LocalProfile().InputPeriod.GetPeriodInputList(metric.PRMR_ID);
                    rpt.DataBind();
                }
                catch
                {
                }
            }
        }

        public void rptProfileInput_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHS_PROFILE_INPUT input = (EHS_PROFILE_INPUT)e.Item.DataItem;
                    EHS_PROFILE_MEASURE metric = LocalProfile().GetMeasure((decimal)input.PRMR_ID);

                    Label lbl;
                    LinkButton lnk;
                    //TextBox tb;
                    DropDownList ddl;

                   // bool enabled = input.STATUS == "C" ? false : true;
                    bool enabled = true;
					sharedCalendar.Visible = true;

                    RadDatePicker dtp1 = (RadDatePicker)e.Item.FindControl("radDateFrom");
					dtp1.SharedCalendar = sharedCalendar;
                    dtp1.Enabled = enabled;
                    dtp1.ShowPopupOnFocus = true;

                    RadDatePicker dtp2 = (RadDatePicker)e.Item.FindControl("radDateTo");
                    dtp2.SharedCalendar = sharedCalendar;
                    dtp2.Enabled = enabled;
                    dtp2.ShowPopupOnFocus = true;

                    SETTINGS sets = SQMSettings.GetSetting("EHS", "INPUTSPAN");
                    int inputspan = 0;
                    int monthSpan1 = Convert.ToInt32(WebSiteCommon.GetXlatValue("invoiceSpan", "MINDATE"));
                    int monthSpan2 = monthSpan1;
                    if (sets != null  &&  int.TryParse(sets.VALUE, out inputspan))
                    {   
                        monthSpan2 = monthSpan1 = inputspan;
                    }
                    dtp1.MinDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(monthSpan1 * -1);
                    dtp2.MinDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(monthSpan2 * -1);

                    if (inputspan > 0)
                        dtp1.MaxDate = dtp2.MaxDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(inputspan);
                    else
                        dtp1.MaxDate = dtp2.MaxDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(Convert.ToInt32(WebSiteCommon.GetXlatValue("invoiceSpan", "MAXDATE")));

                    dtp1.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                    if (input != null)
                    {
                         if (input.STATUS == "N")
                             dtp1.Focus();
                         if (input.EFF_FROM_DT > DateTime.MinValue)
                             dtp1.SelectedDate = input.EFF_FROM_DT;
                         else
                             dtp1.FocusedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);
                    }

                    dtp2.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                    if (input != null  &&  input.EFF_TO_DT > DateTime.MinValue)
                        dtp2.SelectedDate = input.EFF_TO_DT;
                    else
                        dtp2.FocusedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);


                   UOM  uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == input.UOM);
                    if (uom != null)
                    {
                        lbl = (Label)e.Item.FindControl("lblMetricUOM");
                        lbl.Text = uom.UOM_CD;
                    }

                    lbl = (Label)e.Item.FindControl("lblMetricCurrency");
                    lbl.Text = metric.DEFAULT_CURRENCY_CODE;

                    if (input != null)
                        lbl.Text = input.CURRENCY_CODE;

                    TextBox tbValue = (TextBox)e.Item.FindControl("tbMetricValue");
                    tbValue.Enabled = enabled;
                    if (input != null  && (dtp1.SelectedDate != null  &&  dtp2.SelectedDate != null))
                    //if (input != null && input.MEASURE_VALUE != null)
                        tbValue.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_VALUE, 2);

                    TextBox tbCost = (TextBox)e.Item.FindControl("tbMetricCost");
                    TextBox tbCredit = (TextBox)e.Item.FindControl("tbMetricCredit");

                    if ((bool)metric.NEG_VALUE_ALLOWED)
                    {
                        tbCredit.Visible = tbCredit.Enabled = enabled;
                        tbCost.Enabled = false;
                    }
                    else
                    {
                        tbCredit.Visible = false;
                        tbCost.Enabled = true;
                    }

                    if (input != null && input.MEASURE_COST.HasValue && input.MEASURE_COST < 0)
                        tbCredit.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_COST * -1, 2);

                    if (input != null && input.MEASURE_COST.HasValue && input.MEASURE_COST >= 0)
                        tbCost.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_COST, 2);

                    if (metric.EHS_MEASURE.MEASURE_CATEGORY == "PROD" || metric.EHS_MEASURE.MEASURE_CATEGORY == "SAFE" || metric.EHS_MEASURE.MEASURE_CATEGORY == "FACT")
                    {
                        dtp1.SelectedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);
                        dtp1.Enabled = false;
                        dtp2.SelectedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, DateTime.DaysInMonth(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth));
                        dtp2.Enabled = false;
                        tbCost.Visible = false;
                        tbCredit.Visible = false;
                        lbl = (Label)e.Item.FindControl("lblMetricCurrency");
                        lbl.Visible = false;
                    }

                    if (LocalProfile().GetMeasureExt(metric, DateTime.Now) != null && metric.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue)
                    {
                        tbValue.CssClass = "defaultText";
                        tbValue.ToolTip = hfDefaultValue.Value + metric.EHS_PROFILE_MEASURE_EXT.NOTE;
                        tbValue.ReadOnly = metric.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED == true ? false : true;
                        if (string.IsNullOrEmpty(tbValue.Text))
                            tbValue.Text = SQMBasePage.FormatValue((decimal)metric.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT, 2);
                    }
                    if (LocalProfile().GetMeasureExt(metric, DateTime.Now) != null && metric.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue)
                    {
                        tbCost.CssClass = "defaultText";
                        tbCost.ToolTip = hfDefaultValue.Value + metric.EHS_PROFILE_MEASURE_EXT.NOTE;
                        tbCost.ReadOnly = metric.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED == true ? false : true;
                        if (string.IsNullOrEmpty(tbCost.Text))
                            tbCost.Text = SQMBasePage.FormatValue((decimal)metric.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT, 2);
                    }
              
                    CheckBox cbDelete = (CheckBox)e.Item.FindControl("cbDelete");
                    //string cbId = "ctl00_ContentPlaceHolder_Body_rptProfilePeriod_ctl06_rptProfileInput_ctl01_cbDelete";
                    cbDelete.Attributes.Add("onClick", "CheckInputDelete('" + cbDelete.ClientID + "');");
                    if (input.STATUS == "A" || input.STATUS == "D")
                        cbDelete.Enabled = true;
                    
                    if (input.STATUS == "D")
                    {
                        cbDelete.Checked = true;
                        cbDelete.ToolTip = hfDeleteText.Value;
                        hfNumDelete.Value = (Convert.ToInt32(hfNumDelete.Value) + 1).ToString();
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        #endregion

        // manage current session object  (formerly was page static variable)
        EHSProfile LocalProfile()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is EHSProfile)
                return (EHSProfile)SessionManager.CurrentObject;
            else
                return null;
        }
        EHSProfile SetLocalProfile(EHSProfile profile)
        {
            SessionManager.CurrentObject = profile;
            return LocalProfile();
        }

        #region results

        protected void lnkCloseResults(object sender, EventArgs e)
        {
            LinkButton lnkClose = (LinkButton)sender;
            string cmd = lnkClose.CommandArgument;
            lnkClose.Visible = false;
            
            foreach (RepeaterItem item in rptProfilePeriod.Items)
            {
                try
                {
                    LinkButton lnk = (LinkButton)item.FindControl("lnkMetricCD");
                    if (lnk.CommandArgument == cmd)
                    {
                        lnk.Visible = true;
                        CheckBox cb = (CheckBox)item.FindControl("cbMetricSelect");
                        cb.Checked = false;
                    }
                }
                catch
                {
                }
            }
            DisplayResults("");  
        }

        protected void lnkSelectMetric(object sender, EventArgs e)
        {
            string cmdID = "";
            LinkButton lnk = (LinkButton)sender;
            cmdID = lnk.CommandArgument;

            foreach (RepeaterItem item in rptProfilePeriod.Items)
            {
                lnk = (LinkButton)item.FindControl("lnkMetricCD");
                if (lnk.CommandArgument == cmdID)
                {
                    CheckBox cb = (CheckBox)item.FindControl("cbMetricSelect");
                    cb.Checked = true;
                }
            }
            DisplayResults(cmdID);  
        }
 
        private int DisplayResults(string cmdID)
        {
            int status = 0;
            SQMMetricMgr metricMgr = null;
  
            foreach (RepeaterItem item in rptProfilePeriod.Items)
            {
                try
                {
                    LinkButton lnk = (LinkButton)item.FindControl("lnkMetricCD");
                    CheckBox cb = (CheckBox)item.FindControl("cbMetricSelect");
                    if (cb.Checked) //  ||  cmdID == "0")
                    {
                        EHS_PROFILE_MEASURE metric = LocalProfile().GetMeasure(Convert.ToDecimal(lnk.CommandArgument));
                        decimal calcScopeID = EHSModel.ConvertPRODMeasure(metric.EHS_MEASURE, metric.PRMR_ID);
                        System.Web.UI.HtmlControls.HtmlGenericControl reviewArea = (System.Web.UI.HtmlControls.HtmlGenericControl)item.FindControl("divReviewArea");
                        LinkButton lnkClose = (LinkButton)item.FindControl("lnkReviewAreaClose");
                        lnkClose.Visible = reviewArea.Visible = true;
                        lnk.Visible = false;
                        reviewArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
                        if (metricMgr == null)
                        {
                            metricMgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "I", LocalProfile().InputPeriod.PeriodDate.AddMonths(-12), LocalProfile().InputPeriod.PeriodDate, new decimal[1] { LocalProfile().Profile.PLANT_ID });
                            metricMgr.Load(DateIntervalType.month, DateSpanOption.SelectRange);
                        }

                        GaugeDefinition ggCfg = new GaugeDefinition().Initialize();
                        ggCfg.Title = metric.EHS_MEASURE.MEASURE_NAME.Trim() + " - Input History";
                        ggCfg.Height = 250; ggCfg.Width = 700;
                        ggCfg.NewRow = true;
                        ggCfg.DisplayLabel = true;
                        ggCfg.DisplayLegend = false;

                        ggCfg.LabelV = "Quantity";
                        status = uclGauge.CreateControl(SQMChartType.MultiLine, ggCfg, metricMgr.CalcsMethods(new decimal[1] { LocalProfile().Profile.PLANT_ID }, "I", calcScopeID.ToString(), "sum", 32, (int)EHSCalcsCtl.SeriesOrder.PeriodMeasure), reviewArea);
                       
                        if (string.IsNullOrEmpty(metric.EHS_MEASURE.PLANT_ACCT_FIELD)  &&  metric.EHS_MEASURE.MEASURE_CATEGORY != "FACT")
                        {
                            ggCfg.Height = 180; ggCfg.Width = 700;
                            ggCfg.Title = "";
                            ggCfg.DisplayLabel = false;
                            ggCfg.LabelV = "Cost";
                            status = uclGauge.CreateControl(SQMChartType.MultiLine, ggCfg, metricMgr.CalcsMethods(new decimal[1] { LocalProfile().Profile.PLANT_ID }, "I", calcScopeID.ToString(), "cost", 32, (int)EHSCalcsCtl.SeriesOrder.PeriodMeasure), reviewArea);
                        }
                    }
                }
                catch
                {
                    ;
                }
            }

            BindSharedCalendars();

            return status;
        }

        #endregion
    }
}