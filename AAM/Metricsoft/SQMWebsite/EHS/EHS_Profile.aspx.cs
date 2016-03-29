using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class EHS_Profile : SQMBasePage
    {
        public static int dfltRowsToScroll = 12;

        static List<EHS_MEASURE> measureList;
      
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclInputHdr.OnPlantSelect += OnLocationSelect;

			hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclInputHdr.LoadProfileSelectHdr(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true, true);
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

                uclSearchBar.SetButtonsVisible(false, false, false, false, false, false);
                uclSearchBar.PageTitle.Text = lblTitle.Text;
				btnMetricSave.Enabled = lnkMeasureAdd.Enabled = btnMetricCancel.Enabled = UserContext.CheckUserPrivilege(SysPriv.config, SysScope.envdata);
            }
        }

        private void OnLocationSelect(decimal plantID)
        {
            PLANT plant = SQMModelMgr.LookupPlant(plantID);
            if (plant == null)
            {
                DisplayProfileMessage(lblProfileNotExist);
            }
            else 
            {
                List<BusinessLocation> locationList = new List<BusinessLocation>();
                locationList.Add(new BusinessLocation().Initialize(plantID));

				List<PERSON> responsibleList = SQMModelMgr.SelectPrivGroupPersonList(SysPriv.approve, SysScope.envdata, plant.PLANT_ID);
				responsibleList.AddRange(SQMModelMgr.SelectPrivGroupPersonList(SysPriv.admin, SysScope.system, 0).Where(l=> l.STATUS == "A").ToList());  // append any system administrators to the approval list
				SQMBasePage.SetPersonList(ddlFinalApprover, responsibleList, "", true);
				SQMBasePage.SetPersonList(ddlMetricResponsible, SQMModelMgr.SelectPrivGroupPersonList(SysPriv.originate, SysScope.envdata, plant.PLANT_ID).Where(l => l.STATUS == "A").ToList(), "", true);
      
                LoadPlantProfile(plant);

                if (LocalProfile() != null && (LocalProfile().Profile.EHS_PROFILE_MEASURE == null || LocalProfile().Profile.EHS_PROFILE_MEASURE.Count == 0))
                {
                    if (UserContext.GetMaxScopePrivilege(SysScope.envdata) <= SysPriv.config)
                    {
                        List<EHS_PROFILE> profileList = EHSModel.SelectPlantProfileList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID);
                        SQMBasePage.SetLocationList(ddlCopyProfile, SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true).Where(l => profileList.Select(p => p.PLANT_ID).ToArray().Contains(l.Plant.PLANT_ID)).ToList(), 0);
                        ddlCopyProfile.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("", ""));

						responsibleList = SQMModelMgr.SelectPrivGroupPersonList(SysPriv.originate, SysScope.envdata, plant.PLANT_ID);
						SQMBasePage.SetPersonList(ddlDefaultResponsible, responsibleList, "");
                        ddlDefaultResponsible.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("", ""));
                        pnlCopyProfile.Visible = true;
                    }
                }

                pnlProfileEdit.Style.Add("display", "none");
            }
        }

        #region profile

        public EHSProfile LoadPlantProfile(PLANT plant)
        {
            SetLocalProfile(new EHSProfile().Load(plant.PLANT_ID, true, false));
            if (LocalProfile().Profile != null)
            {
                uclInputHdr.BindProfileSelectHdr(LocalProfile());
                SetupProfilePanel();
                BindProfile(LocalProfile());
                DisplayProfileMessage(null);
            }
            else
            {
                DisplayProfileMessage(lblProfileError);
            }
            return LocalProfile();
        }

        private void BindProfile(EHSProfile profile)
        {
            if (profile != null)
            {
                ddlDayDue.SelectedValue = LocalProfile().Profile.DAY_DUE.ToString();
                ddlWarningDays.SelectedValue = LocalProfile().Profile.REMINDER_DAYS.ToString();
                if (LocalProfile().Profile.APPROVER_ID > 0 && ddlFinalApprover.Items.FindByValue(LocalProfile().Profile.APPROVER_ID.ToString()) != null)
                    ddlFinalApprover.SelectedValue = LocalProfile().Profile.APPROVER_ID.ToString();
                else
                    ddlFinalApprover.SelectedIndex = 0;

                if (LocalProfile().Profile.DISPLAY_OPTION.HasValue)
                    ddlDisplayOrder.SelectedIndex = (int)LocalProfile().Profile.DISPLAY_OPTION;

                if (LocalProfile().Profile.EHS_PROFILE_FACT != null && LocalProfile().Profile.EHS_PROFILE_FACT.Count > 0)
                {
                    if (ddlNormFact.Items.FindByValue(LocalProfile().Profile.EHS_PROFILE_FACT.FirstOrDefault().FACTOR_ID.ToString()) != null)
                        ddlNormFact.SelectedValue = LocalProfile().Profile.EHS_PROFILE_FACT.FirstOrDefault().FACTOR_ID.ToString();
                }
                else
                {
					EHS_PROFILE_MEASURE factMetric = null;
					try
					{
						factMetric = profile.Profile.EHS_PROFILE_MEASURE.Where(l => l.EHS_MEASURE.MEASURE_CATEGORY == "FACT").FirstOrDefault();
					}
					catch { }
                    if (factMetric != null  &&  ddlNormFact.Items.FindByValue(factMetric.EHS_MEASURE.MEASURE_ID.ToString()) != null)
                    {
                        ddlNormFact.SelectedValue = factMetric.EHS_MEASURE.MEASURE_ID.ToString();
                    }
                    else 
                    {
                        ddlNormFact.SelectedIndex = 0;
                    }
                }

                if (ddlMetricCurrency.Items.FindByValue(LocalProfile().Plant.CURRENCY_CODE) != null)
                    ddlMetricCurrency.SelectedValue = LocalProfile().Plant.CURRENCY_CODE;

                pnlMetricEdit.Enabled = btnMetricCancel.Enabled = btnMetricSave.Enabled = false;
				lnkMeasureAdd.Enabled = btnMetricSave.Enabled = UserContext.CheckUserPrivilege(SysPriv.config, SysScope.envdata); 

                UpdateMetricList(profile);

                pnlMetricEdit.Visible = false;
                //pnlProfileEdit.Visible = true;
            }
        }

        private void UpdateMetricList(EHSProfile profile)
        {
            if (profile != null)
            {
                switch (profile.Profile.DISPLAY_OPTION)
                {
                    case 1:
                        gvMetricList.DataSource = profile.Profile.EHS_PROFILE_MEASURE.OrderBy(l => l.EHS_MEASURE.MEASURE_CD);
                        break;
                    case 2:
                        gvMetricList.DataSource = profile.Profile.EHS_PROFILE_MEASURE.OrderBy(l => l.EHS_MEASURE.MEASURE_NAME);
                        break;
                    default:
                        gvMetricList.DataSource = profile.Profile.EHS_PROFILE_MEASURE.OrderBy(l => l.EHS_MEASURE.MEASURE_CATEGORY).ThenBy(l => l.EHS_MEASURE.MEASURE_CD);
                        break;
                }

                gvMetricList.DataBind();
                SQMBasePage.SetGridViewDisplay(gvMetricList, lblMetricListEmpty, divMetricListGVScroll, -1, 0);
            }
        }

        private void SetupProfilePanel()
        {
            if (ddlMetricDisposalCode.Items.Count == 0)
            {
                ddlDayDue.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 31));
                ddlWarningDays.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(0, 11));

                ddlMetricDisposalCode.DataSource = SessionManager.DisposalCodeList;
                ddlMetricDisposalCode.DataValueField = "UN_CODE";
                ddlMetricDisposalCode.DataTextField = "UN_CODE";
                ddlMetricDisposalCode.DataBind();
                ddlMetricDisposalCode.Items.Insert(0, new ListItem(""));

                ddlMetricRegStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("regulatoryStatus"));
                ddlMetricRegStatus.Items.Insert(0, new ListItem(""));

                SQMBasePage.FillCurrencyDDL(ddlMetricCurrency, "EUR");
                ddlMetricCurrency.Items.Insert(0, "");

                ddlMetricCost.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("costType", "long"));
                ddlMetricCost.Items.Insert(0, "");

                if (UserContext.CheckUserPrivilege(SysPriv.config, SysScope.envdata))
                    ddlMetricStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCodeDelete"));
                else
                    ddlMetricStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCode"));

                ddlMetricCategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureCategoryEHS", 2, ""));
                ddlMetricCategory.Items.Insert(0, "");

                measureList = EHSModel.SelectEHSMeasureList("", true).OrderBy(l=> l.MEASURE_NAME).ToList();
                foreach (EHS_MEASURE measure in measureList)
                {
                    ddlMetricID.Items.Add(new ListItem(measure.MEASURE_NAME.Trim(), WebSiteCommon.PackItemValue(measure.MEASURE_CATEGORY, measure.EFM_TYPE, measure.MEASURE_ID.ToString())));
                    if (measure.MEASURE_CATEGORY == "FACT")
                    {
                        ListItem item = new ListItem(measure.MEASURE_NAME.Trim(), measure.MEASURE_ID.ToString());
                        item.Attributes.Add("title", measure.MEASURE_DESC);
                        ddlNormFact.Items.Add(item);
                    }
                }
                ddlMetricID.Items.Insert(0, "");
                ddlNormFact.Items.Insert(0, "");

                if (ddlNormFact.Items.Count == 1)       // don't display normalize selects if no factors defined
                    phNormFact.Visible = false;
               
                foreach (UOM uom in SessionManager.UOMList.Where(l=> l.OWNER_ID == null).OrderBy(l=> l.UOM_NAME).ToList())
                {
                    ddlMetricUOM.Items.Add(new ListItem(uom.UOM_NAME, WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.EFM_TYPE, uom.UOM_ID.ToString())));
                }
                ddlMetricUOM.Items.Insert(0, "");
                ddlMetricUOM.SelectedIndex = 0;
            }
            divEHSProfile.Visible = true;
            pnlCopyProfile.Visible = false;
            UpdateListTitles();
        }

        private void UpdateUOMSelects(string metricCategory, string efmType)
        {
            string[] categoryArray = new string[2];
            string useEfmType = efmType;
            switch (metricCategory)
            {
                case "PROD":
                case "FACT":
                    categoryArray[0] = "NONE";
                    break;
                case "SAFE":
                    categoryArray[0] = "SAFE";
                    break;
                case "ENGY":
                    categoryArray[0] = "ENGY";
                    if (string.IsNullOrEmpty(efmType))
                        useEfmType = "NG";       // set default for unknown energy types
                    break;
                case "EUTL":
                    categoryArray[0] = "VOL";
                    break;
                default:
                    categoryArray[0] = "WEIT";  categoryArray[1] = "CUST";
                break;
            }

            int nItem = -1;
            foreach (ListItem item in ddlMetricUOM.Items)
            {
                ++nItem;
                if (nItem == 0)
                {
                    item.Enabled = true;
                    ddlMetricUOM.SelectedIndex = nItem;
                }
                else
                {
                    string[] args = item.Value.Split('|');
                    if (args.Length > 1  &&  categoryArray.Contains(args[0]))
                    {
                        if (string.IsNullOrEmpty(useEfmType) || (!string.IsNullOrEmpty(useEfmType) && args[1] == useEfmType))
                        {
                            item.Enabled = true;
                            //ddlMetricUOM.SelectedIndex = nItem;
                        }
                        else
                            item.Enabled = false;
                    }
                    else
                        item.Enabled = false;
                }
            }
        }

        protected void UpdateMain(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            Label lblTarget = null;

            if (ddlSender.ID == "ddlMetricCategory")
            {
                ddlCategoryChanged(sender, e);
                lblMetricName.Text = "";
                ddlMetricUOM.Enabled = false;
            }

            if (ddlSender.ID == "ddlMetricID")
            {
                lblTarget = lblMetricName;
                string[] args = ddlMetricID.SelectedValue.Split('|');
                if (args.Length > 1)
                    UpdateUOMSelects(ddlMetricCategory.SelectedValue, args[1]);
                ddlMetricUOM.Enabled = true;
            }

            if (ddlSender.ID == "ddlMetricDisposalCode")
            {
                lblTarget = lblDisposalDesc;
            }

            UpdateListTitles();

            ListItem item = ddlSender.SelectedItem;
            if (item != null &&  lblTarget != null)
            {
                lblTarget.Text = item.Attributes["title"];
            }

            udpMain.Update();
        }

        protected void ddlCategoryChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            string key = ddlSender.SelectedValue;
            bool enableDefaults = true;

            ddlMetricID.Enabled = ddlMetricResponsible.Enabled = true;
            // disable country waste code for non-waste measure categories
            tbWasteCode.Enabled = true;
            ddlMetricDisposalCode.Enabled = true;
            ddlMetricRegStatus.Enabled = true;
            ddlMetricUOM.Enabled = true;
            ddlMetricCurrency.Enabled = true;
            ddlMetricUOM.Enabled = true;
            ddlMetricCurrency.Enabled = true;
            ddlMetricCost.Enabled = true;

            ddlMetricDisposalCode.Visible = ddlMetricRegStatus.Visible = tbWasteCode.Visible = true;
            tdRegStatusHdr.Visible = tdRegStatus.Visible = ddlMetricRegStatus.Visible = true;
            tdWasteCodeHdr.Visible = tdDisposalHdr.Visible = true;
            phCostWaste.Visible = true;
            phMetricExt.Visible = false;

			SETTINGS sets = SQMSettings.GetSetting("EHS", "UNCODEREQ");
			if (sets == null || sets.VALUE.ToUpper() != "N")
				tdDisposal.Attributes.Add("Class", "required");
			else
				tdDisposal.Attributes.Remove("Class");
			tdRegStatus.Attributes.Add("Class", "required");
            tdUOM.Attributes.Add("Class", "required");
            tdCurrency.Attributes.Add("Class", "required");
            tdMetricCost.Attributes.Add("Class", "required");

            if (LocalProfile().CurrentEHSMeasure == null)
                UpdateUOMSelects(key, "");
            else
                UpdateUOMSelects(key, LocalProfile().CurrentEHSMeasure.EFM_TYPE);

            switch (key)
            {
                case "PROD":
                case "FACT":
                    tbWasteCode.Text = "";
                    ddlMetricDisposalCode.SelectedIndex = 0;
                    ddlMetricRegStatus.SelectedIndex = 0;
                    ddlMetricCurrency.SelectedIndex = 0;
                    ddlMetricCost.SelectedIndex = 0;
                    ddlMetricUOM.SelectedIndex = ddlMetricCost.SelectedIndex = ddlMetricCurrency.SelectedIndex = 0;
                    tdRegStatusHdr.Visible = tdRegStatus.Visible = ddlMetricRegStatus.Visible = false;
                    phCostWaste.Visible = false;
                    phMetricExt.Visible = enableDefaults;
                    break;
                case "SAFE":
                    tbWasteCode.Text = "";
                    ddlMetricDisposalCode.SelectedIndex = 0;
                    ddlMetricRegStatus.SelectedIndex = 0;
                    ddlMetricCurrency.SelectedIndex = 0;
                    ddlMetricCost.SelectedIndex = 0;
                    ddlMetricUOM.SelectedIndex = ddlMetricCost.SelectedIndex = ddlMetricCurrency.SelectedIndex = 0;
                    tdRegStatusHdr.Visible = tdRegStatus.Visible = ddlMetricRegStatus.Visible = false;
                    phCostWaste.Visible = false;
                    break;
                case "ENGY":
                case "EUTL":
                    tbWasteCode.Enabled = false;
                    tbWasteCode.Text = "";
                    ddlMetricDisposalCode.SelectedIndex = 0;
                    ddlMetricDisposalCode.Enabled = false;
                    ddlMetricRegStatus.Enabled = false;
                    ddlMetricRegStatus.SelectedIndex = 0;
                    tdDisposal.Attributes.Remove("Class");
                    tdRegStatus.Attributes.Remove("Class");
                    ddlMetricDisposalCode.Visible = ddlMetricRegStatus.Visible = tbWasteCode.Visible = false;
                    tdRegStatusHdr.Visible = tdWasteCodeHdr.Visible = tdDisposalHdr.Visible = false;
					phMetricExt.Visible = enableDefaults;
                    break;
                default:        // wastes
                    phMetricExt.Visible = enableDefaults;
                    break;
            }
 
            foreach (ListItem item in ddlMetricID.Items)
            {
                string[] parms = item.Value.Split('|');
                if (string.IsNullOrEmpty(item.Value) || parms[0] == key)
                {
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }
            }

            if (e == null)      // from  measure binding
            {
                UpdateListTitles();
                udpMain.Update();
            }
        }

        protected void ddlUOMChanged(object sender, EventArgs e)
        {
            if (ddlMetricUOM.SelectedValue.Contains("CUST"))
            {
                spUOMFactor.Visible = true;
            }
            else
            {
                tbUOMFactor.Text = "";
                spUOMFactor.Visible = false;
            }

            udpMain.Update();
        }

        protected void UpdateListTitles()
        {
            string title = "";
            EHS_MEASURE measure = null;
            foreach (ListItem item in ddlMetricID.Items)
            {
                if (item.Enabled && !string.IsNullOrEmpty(item.Text))
                {
                    measure = measureList.FirstOrDefault(l => l.MEASURE_NAME == item.Text);
                    if (measure != null)
                        item.Attributes.Add("title", measure.MEASURE_CD);
                }
            }

            foreach (ListItem item in ddlMetricDisposalCode.Items)
            {
                if (item.Enabled && !string.IsNullOrEmpty(item.Text))
                    item.Attributes.Add("title", SessionManager.DisposalCodeList.FirstOrDefault(l => l.UN_CODE == item.Value).DESCRIPTION);
            }
        }

        protected void lnkMetricList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal prmrID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindProfileMeasure(LocalProfile().Profile.EHS_PROFILE_MEASURE.FirstOrDefault(l => l.PRMR_ID == prmrID));
            ddlMetricCategory.Focus();
        }

        protected void ddlCopyProfileSelect(object sender, EventArgs e)
        {
            if (ddlCopyProfile.SelectedIndex > 0)
                btnCopyProfile.Enabled = true;
            else
                btnCopyProfile.Enabled = false;
        }

        protected void btnCopyProfile_Click(object sender, EventArgs e)
        {
            if (ddlCopyProfile.SelectedIndex > -1)
            {
                try
                {
                    EHSProfile profile = new EHSProfile();
                    profile.Profile = profile.LookupProfile(LocalProfile().Entities, Convert.ToDecimal(ddlCopyProfile.SelectedValue));
                    foreach (EHS_PROFILE_MEASURE metric in profile.Profile.EHS_PROFILE_MEASURE)
                    {
                        EHS_PROFILE_MEASURE newMetric = new EHS_PROFILE_MEASURE();
                        decimal defaultResponible = ddlDefaultResponsible.SelectedIndex > 0 ? Convert.ToDecimal(ddlDefaultResponsible.SelectedValue) : 0;
                        LocalProfile().CopyMeasure(metric, LocalProfile().Plant.PLANT_ID, defaultResponible);
                    }
                    if (!LocalProfile().Profile.APPROVER_ID.HasValue)
                        LocalProfile().Profile.APPROVER_ID = profile.Profile.APPROVER_ID;
                    EHSProfile.UpdateProfile(LocalProfile());
                    LoadPlantProfile(LocalProfile().Plant);
                }
                catch 
                {
                    DisplayProfileMessage(lblCopyError);
                }
            }
        }

        protected void btnMetricAdd_Click(object sender, EventArgs e)
        {
            hfOper.Value = "add";
            divEHSProfile.Visible = true;
            pnlCopyProfile.Visible = false;
            BindProfileMeasure(null);
            ddlMetricCategory.Focus();
        }

        protected void btnMetricClear_Click(object sender, EventArgs e)
        {
            hfOper.Value = "";
            btnMetricCancel.Enabled = false;
            pnlMetricEdit.Visible = false;

            string script = "function f(){CloseMetricEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);  
        }

        public int BindProfileMeasure(EHS_PROFILE_MEASURE pm)
        {
            int status = 0;
            pnlMetricEdit.Visible = true;
            spUOMFactor.Visible = false;
            pnlMetricEdit.Visible = true;
            btnMetricCancel.Enabled = true;
            DisplayErrorMessage(null);

			try
			{
				if (pm == null)
				{
					ddlMetricID.Enabled = ddlMetricCost.Enabled = ddlMetricDisposalCode.Enabled = ddlMetricRegStatus.Enabled = ddlMetricUOM.Enabled = ddlMetricCurrency.Enabled = ddlMetricResponsible.Enabled = false;
					ddlMetricCategory.SelectedIndex = ddlMetricID.SelectedIndex = ddlMetricDisposalCode.SelectedIndex = ddlMetricRegStatus.SelectedIndex = ddlMetricUOM.SelectedIndex = ddlMetricCost.SelectedIndex = ddlMetricResponsible.SelectedIndex = 0;
					if (ddlMetricCurrency.Items.FindByValue(LocalProfile().Plant.CURRENCY_CODE) != null)
						ddlMetricCurrency.SelectedValue = LocalProfile().Plant.CURRENCY_CODE;
					lblMetricName.Text = lblDisposalDesc.Text = "";
					tbMetricPrompt.Text = tbUOMFactor.Text = tbWasteCode.Text = "";
					winMetricEdit.Title = hfAddMetric.Value;
					tbValueDflt.Text = tbCostDflt.Text = "";
					cbEnableOverride.Checked = false;
					cbMetricRequired.Checked = true;
				}
				else
				{
					winMetricEdit.Title = hfUpdateMetric.Value;
					LocalProfile().CurrentProfileMeasure = pm;
					LocalProfile().CurrentEHSMeasure = pm.EHS_MEASURE;

					if (pm.EHS_MEASURE != null && ddlMetricCategory.Items.FindByValue(pm.EHS_MEASURE.MEASURE_CATEGORY) != null)
					{
						ddlMetricCategory.SelectedValue = pm.EHS_MEASURE.MEASURE_CATEGORY;
						ddlCategoryChanged(ddlMetricCategory, null);
						ddlMetricID.SelectedValue = WebSiteCommon.PackItemValue(pm.EHS_MEASURE.MEASURE_CATEGORY, pm.EHS_MEASURE.EFM_TYPE, pm.EHS_MEASURE.MEASURE_ID.ToString());
						lblMetricName.Text = pm.EHS_MEASURE.MEASURE_CD;

						if (pm.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && pm.EHS_MEASURE.MEASURE_CATEGORY != "SAFE" && pm.EHS_MEASURE.MEASURE_CATEGORY != "FACT" && ddlMetricCurrency.Items.FindByValue(pm.DEFAULT_CURRENCY_CODE) != null)
							ddlMetricCurrency.SelectedValue = pm.DEFAULT_CURRENCY_CODE;

						if (pm.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && pm.EHS_MEASURE.MEASURE_CATEGORY != "SAFE" && pm.EHS_MEASURE.MEASURE_CATEGORY != "FACT" && pm.DEFAULT_UOM > 0)
						{
							UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == pm.DEFAULT_UOM);
							if (uom != null)
							{
								if (ddlMetricUOM.Items.FindByValue(WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.EFM_TYPE, uom.UOM_ID.ToString())) != null)
									ddlMetricUOM.SelectedValue = WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.EFM_TYPE, uom.UOM_ID.ToString());
								else
									ddlMetricUOM.SelectedIndex = 0;

								if (uom.UOM_CATEGORY == "CUST")
								{
									spUOMFactor.Visible = true;
								}
							}

							if (pm.UOM_FACTOR.HasValue)
								tbUOMFactor.Text = SQMBasePage.FormatValue((decimal)pm.UOM_FACTOR, 5);
						}

						if (pm.EHS_MEASURE.MEASURE_CATEGORY != "PROD" && pm.EHS_MEASURE.MEASURE_CATEGORY != "SAFE" && pm.EHS_MEASURE.MEASURE_CATEGORY != "FACT")
						{
							if (pm.NEG_VALUE_ALLOWED.HasValue && (bool)pm.NEG_VALUE_ALLOWED)
								ddlMetricCost.SelectedValue = "CREDIT";
							else
								ddlMetricCost.SelectedValue = "COST";
						}
					}

					tbMetricPrompt.Text = pm.MEASURE_PROMPT;
					ddlMetricRegStatus.SelectedValue = pm.REG_STATUS;
					ddlMetricDisposalCode.SelectedValue = pm.UN_CODE;
					if (!string.IsNullOrEmpty(pm.UN_CODE))
						lblDisposalDesc.Text = SessionManager.DisposalCodeList.FirstOrDefault(l => l.UN_CODE == pm.UN_CODE).DESCRIPTION;
					else
						lblDisposalDesc.Text = "";

					tbWasteCode.Text = pm.WASTE_CODE;

					if (pm.RESPONSIBLE_ID > 0 && ddlMetricResponsible.Items.FindByValue(pm.RESPONSIBLE_ID.ToString()) != null)
						ddlMetricResponsible.SelectedValue = pm.RESPONSIBLE_ID.ToString();
					else
						ddlMetricResponsible.SelectedIndex = 0;

					ddlUOMChanged(ddlMetricUOM, null);
					ddlMetricStatus.SelectedValue = pm.STATUS;
					cbMetricRequired.Checked = (bool)pm.IS_REQUIRED;

					tbValueDflt.Text = tbCostDflt.Text = "";
					cbEnableOverride.Checked = false;
					// radEffEndDate.ShowPopupOnFocus = true;
					//radEffEndDate.SelectedDate = null;
					if (pm.EHS_PROFILE_MEASURE_EXT != null && pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue)
						tbValueDflt.Text = SQMBasePage.FormatValue((decimal)pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT, 2);
					if (pm.EHS_PROFILE_MEASURE_EXT != null && pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue)
						tbCostDflt.Text = SQMBasePage.FormatValue((decimal)pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT, 2);
					if (pm.EHS_PROFILE_MEASURE_EXT != null && pm.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED.HasValue)
						cbEnableOverride.Checked = (bool)pm.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED;
					//if (pm.EHS_PROFILE_MEASURE_EXT != null && pm.EHS_PROFILE_MEASURE_EXT.EFF_END_DT.HasValue)
					//    radEffEndDate.SelectedDate = pm.EHS_PROFILE_MEASURE_EXT.EFF_END_DT;
				}

				UpdateListTitles();
				pnlMetricEdit.Enabled = btnMetricCancel.Enabled = btnMetricSave.Enabled = UserContext.CheckUserPrivilege(SysPriv.config, SysScope.envdata);

				string script = "function f(){OpenMetricEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}

			catch
			{
			}

            return status;
        }

        protected void btnProfileCancel_Click(object sender, EventArgs e)
        {
            pnlProfileEdit.Style.Add("display", "none");
        }

        protected void btnProfileSave_Click(object sender, EventArgs e)
        {
            LocalProfile().Profile.DAY_DUE = Convert.ToInt32(ddlDayDue.SelectedValue);
            LocalProfile().Profile.REMINDER_DAYS = Convert.ToInt32(ddlWarningDays.SelectedValue);
            LocalProfile().Profile.APPROVER_ID = Convert.ToInt32(ddlFinalApprover.SelectedValue);
            LocalProfile().Profile.DISPLAY_OPTION = ddlDisplayOrder.SelectedIndex;

            if (LocalProfile().Profile.EHS_PROFILE_FACT != null)
                LocalProfile().Profile.EHS_PROFILE_FACT.Clear();

            if (ddlNormFact.SelectedIndex > 0)
            {
                LocalProfile().Profile.EHS_PROFILE_FACT.Add(LocalProfile().AddFactor("norm", "", Convert.ToDecimal(ddlNormFact.SelectedValue)));
                LocalProfile().Profile.EHS_PROFILE_FACT.Add(LocalProfile().AddFactor("normCost", "", Convert.ToDecimal(ddlNormFact.SelectedValue)));
            }
            
            LocalProfile().Profile.UTIL_MONTH_SPAN = LocalProfile().Profile.WASTE_MONTH_SPAN = Convert.ToInt32(WebSiteCommon.GetXlatValue("invoiceSpan", "MINDATE"));

			if (EHSProfile.UpdateProfile(LocalProfile()) >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				//ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
			}
            BindProfile(LocalProfile());
        }

        protected void btnMetricSave_Click(object sender, EventArgs e)
        {
            bool success;
            bool measureChanged = false;
            bool reqdError = false;
            EHS_PROFILE_MEASURE pm = null;

            if (hfOper.Value == "add")
            {
                pm = new EHS_PROFILE_MEASURE();
                pm.EHS_PROFILE_MEASURE_EXT = new EHS_PROFILE_MEASURE_EXT();
            }
            else
            {
                pm = LocalProfile().Profile.EHS_PROFILE_MEASURE.FirstOrDefault(l => l.PRMR_ID == LocalProfile().CurrentProfileMeasure.PRMR_ID);
                if (pm.EHS_PROFILE_MEASURE_EXT == null)
                {
                    pm.EHS_PROFILE_MEASURE_EXT = new EHS_PROFILE_MEASURE_EXT();
                    pm.EHS_PROFILE_MEASURE_EXT.PRMR_ID = pm.PRMR_ID;
                }
            }

            pm.PLANT_ID = LocalProfile().Profile.PLANT_ID;

            decimal measureID = 0;
            if (!string.IsNullOrEmpty(ddlMetricID.SelectedValue))
            {
                measureID = Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricID.SelectedValue));
                if (pm.MEASURE_ID != measureID)
                    measureChanged = true;
            }

            pm.MEASURE_ID = measureID;
            pm.MEASURE_PROMPT = tbMetricPrompt.Text;
            pm.REG_STATUS = ddlMetricRegStatus.SelectedValue;
            pm.UN_CODE = ddlMetricDisposalCode.SelectedValue;
            pm.WASTE_CODE = tbWasteCode.Text;

            pm.DEFAULT_CURRENCY_CODE = ddlMetricCurrency.SelectedValue;
            if (ddlMetricResponsible.SelectedIndex > 0)
            {
                decimal personID = Convert.ToDecimal(ddlMetricResponsible.SelectedValue);
                if (personID != pm.RESPONSIBLE_ID)
                    pm = LocalProfile().UpdateMeasureResponsible(pm, personID);
                pm.RESPONSIBLE_ID = personID;
            }
            else
                pm.RESPONSIBLE_ID = Convert.ToDecimal(null);

            if (ddlMetricCost.SelectedValue == "CREDIT")
                pm.NEG_VALUE_ALLOWED = true;
            else
                pm.NEG_VALUE_ALLOWED = false;

            pm.STATUS = ddlMetricStatus.SelectedValue;

            pm.IS_REQUIRED = cbMetricRequired.Checked;

            decimal uomID = 0;

            if (ddlMetricCategory.SelectedValue == "FACT")
            {
                pm.DEFAULT_UOM = EHSModel.LookupEHSMeasure(new PSsqmEntities(), pm.MEASURE_ID, "").STD_UOM;
            }
            else
            {
                if (SQMBasePage.ParseToDecimal(WebSiteCommon.ParseItemValue(ddlMetricUOM.SelectedValue), out uomID))
                    pm.DEFAULT_UOM = uomID;

                decimal UOMFactor = 0;
                if (decimal.TryParse(tbUOMFactor.Text, out UOMFactor))
                    pm.UOM_FACTOR = UOMFactor;
                else
                    pm.UOM_FACTOR = null;
            }

            if (phMetricExt.Visible)
            {
                decimal decimalValue;
                if (SQMBasePage.ParseToDecimal(tbValueDflt.Text, out decimalValue))
                    pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT = decimalValue;
                else
                    pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT = null;

                if (SQMBasePage.ParseToDecimal(tbCostDflt.Text, out decimalValue))
                    pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT = decimalValue;
                else
                    pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT = null;

                if ((pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue || pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue))
                    pm.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED = cbEnableOverride.Checked;
                else
                    pm.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED = false;
                /*
                if ((pm.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue || pm.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue) && radEffEndDate.SelectedDate != null)
                    pm.EHS_PROFILE_MEASURE_EXT.EFF_END_DT = (DateTime)radEffEndDate.SelectedDate;
                else
                    pm.EHS_PROFILE_MEASURE_EXT.EFF_END_DT = null;
                */
            }

            // validate
           
            switch (ddlMetricCategory.SelectedValue)
            {
                case "ENGY":
                case "EUTL":
                    if (string.IsNullOrEmpty(ddlMetricCategory.SelectedValue) || string.IsNullOrEmpty(ddlMetricID.SelectedValue) || string.IsNullOrEmpty(ddlMetricUOM.SelectedValue) || string.IsNullOrEmpty(ddlMetricResponsible.SelectedValue))
                        reqdError = true;
                    break;
                case "PROD":
                case "FACT":
                    if (string.IsNullOrEmpty(ddlMetricCategory.SelectedValue) || string.IsNullOrEmpty(ddlMetricID.SelectedValue) || string.IsNullOrEmpty(ddlMetricResponsible.SelectedValue))
                        reqdError = true;
                    break;
                case "SAFE":
                    if (string.IsNullOrEmpty(ddlMetricCategory.SelectedValue) || string.IsNullOrEmpty(ddlMetricID.SelectedValue) || string.IsNullOrEmpty(ddlMetricResponsible.SelectedValue))
                        reqdError = true;
                    break;
                default:
					if (tdDisposal.Attributes["Class"] == null || tdDisposal.Attributes["Class"] != "required")  // UN disposal code not required
					{
						if (string.IsNullOrEmpty(ddlMetricCategory.SelectedValue) || string.IsNullOrEmpty(ddlMetricID.SelectedValue) || string.IsNullOrEmpty(ddlMetricUOM.SelectedValue) || string.IsNullOrEmpty(ddlMetricResponsible.SelectedValue)
							|| string.IsNullOrEmpty(ddlMetricRegStatus.SelectedValue))
							reqdError = true;
					}
					else
					{
						if (string.IsNullOrEmpty(ddlMetricCategory.SelectedValue) || string.IsNullOrEmpty(ddlMetricID.SelectedValue) || string.IsNullOrEmpty(ddlMetricUOM.SelectedValue) || string.IsNullOrEmpty(ddlMetricResponsible.SelectedValue)
						|| string.IsNullOrEmpty(ddlMetricDisposalCode.SelectedValue) || string.IsNullOrEmpty(ddlMetricRegStatus.SelectedValue))
							reqdError = true;
					}
                    if (tbUOMFactor.Visible && string.IsNullOrEmpty(tbUOMFactor.Text))
                        reqdError = true;
                    break;
            }

			// AW01/2016 - if trying to delete, verify that there is no historical data associated with the Metric by checking for a EHS_PROFILE_INPUT
			if (pm.STATUS.Equals("D"))
			{
				if (EHSProfile.ValidateProfileMeasureForDelete(LocalProfile(), pm.PRMR_ID) > 0)
				{
					reqdError = true;
					BindProfileMeasure(pm);
					DisplayErrorMessage(hfErrMetricHasHistory);
					return;
				}
				else
				{

				}
			}
			if (reqdError && !pm.STATUS.Equals("D")) // AW 01/2016 - don't show the errors if we are deleting the record
            {
                BindProfileMeasure(pm);
                DisplayErrorMessage(hfErrRequiredInputs);
				hfOper.Value = "";
                return;
            }

            if (hfOper.Value == "add")  // add measure to list
                pm = LocalProfile().AddMeasure(pm, Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricID.SelectedValue)));

            EHSProfile.UpdateProfile(LocalProfile());

            if (pm.STATUS == "D")
            {
				EHSProfile.DeleteProfileMeasureNoHistory(LocalProfile(), pm.PRMR_ID); // AW 01/2016 - Delete record by PRMR_ID, not MEASURE_ID
                measureChanged = true;
            }
  
            if (measureChanged)
                SetLocalProfile(new EHSProfile().Load(LocalProfile().Plant.PLANT_ID, true, false));

            btnMetricClear_Click(null, null);
            BindProfile(LocalProfile());
        }

        private void DisplayErrorMessage(HiddenField hfMessage)
        {
            if (hfMessage == null)
                lblErrorMessage.Text = "";
            else
                lblErrorMessage.Text = hfMessage.Value;
        }

        private void DisplayProfileMessage(Label lblError)
        {
            if (lblError == null)
                lblProfileNotExist.Visible = lblProfileError.Visible = lblNoMetrics.Visible = lblCopyError.Visible = false;
            else
            {
                lblError.Visible = true;
                if (lblError == lblProfileNotExist || lblError == lblProfileError)
                {
                    divEHSProfile.Visible = false;
                    uclInputHdr.BindProfileSelectHdr(null);
                }
            }
        }
 
        public void gvOnProfileRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    CheckBox cb;
                    Label lbl;
                    Image img = (Image)e.Row.Cells[0].FindControl("imgHazardType");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricPrompt");

                    if (!string.IsNullOrEmpty(hf.Value))
                    {
                        lbl = (Label)e.Row.Cells[0].FindControl("lblMetricPrompt");
                        lbl.Visible = true;
                        lbl.Text = "<br>" + hf.Value ;
                    }

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricCategory");
                    if (hf.Value == "ENGY"  ||  hf.Value == "EUTL")
                    {
                        e.Row.Cells[0].CssClass = e.Row.Cells[1].CssClass = "energyColor";
                        img.ImageUrl = "~/images/status/energy.png";
                    }
                    else if (hf.Value == "PROD" || hf.Value == "SAFE" || hf.Value == "FACT")
                    {
                        img.ImageUrl = "~/images/status/inputs.png";
                        img.ToolTip = WebSiteCommon.GetXlatValueLong("measureCategoryEHS", hf.Value);
                    }
                    else
                    {
                        e.Row.Cells[0].CssClass = e.Row.Cells[1].CssClass = "wasteColor";
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricRegStatus");

                        if (hf.Value == "HZ")
                        {
                            img.ImageUrl = "~/images/status/hazardous.png";
                        }
                        else
                        {
                            img.ImageUrl = "~/images/status/waste.png";
                        }
                        img.ToolTip = WebSiteCommon.GetXlatValueLong("regulatoryStatus", hf.Value);
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfDisposalCode");
                        if (!string.IsNullOrEmpty(hf.Value))
                            img.ToolTip += (".  " + SessionManager.DisposalCodeList.FirstOrDefault(l => l.UN_CODE == hf.Value).DESCRIPTION);
                    }
                    //      e.Row.Cells[0].Attributes.Add("Style", "background: wheat;");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricStatus");
                    if (hf.Value == "I")
                    {
                        img = (Image)e.Row.Cells[0].FindControl("imgStatus");
                        img.ImageUrl = "/images/defaulticon/16x16/no.png";
                        img.Visible = true;
                        cb = (CheckBox)e.Row.Cells[0].FindControl("cbMetricRequired");
                        cb.Visible = false;
                    }

                    LinkButton lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkMetricCD");
                    LinkButton lnk2 = (LinkButton)e.Row.Cells[0].FindControl("lnkMetricName");
                    lnk.ToolTip = lnk2.ToolTip = WebSiteCommon.GetXlatValue("measureCategoryEHS", hf.Value);

                    cb = (CheckBox)e.Row.Cells[0].FindControl("cbMetricRequired");
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricRequired");
                    if (!string.IsNullOrEmpty(hf.Value))
                    {
                        try
                        {
                            cb.Checked = Convert.ToBoolean(hf.Value);
                        }
                        catch
                        { }
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblInvoiceType");
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricCategory");
                    if (hf.Value == "SAFE" || hf.Value == "PROD" || hf.Value == "FACT")
                        lbl.Text = "";
                    else
                    {
                        if (lbl.Text == "True")
                            lbl.Text = WebSiteCommon.GetXlatValue("costType", "CREDIT", "short");
                        else
                            lbl.Text = WebSiteCommon.GetXlatValue("costType", "COST", "short");
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblInvoiceUOM");
                    if (!string.IsNullOrEmpty(lbl.Text))
                    {
                        decimal uomID = Convert.ToDecimal(lbl.Text);
                        lbl.Text = SessionManager.UOMList.Where(l => l.UOM_ID == uomID).Select(u => u.UOM_NAME).FirstOrDefault().ToString(); 
                    }
                }
                catch
                {
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

    }
}