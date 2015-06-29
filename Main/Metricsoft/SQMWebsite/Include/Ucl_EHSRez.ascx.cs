using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SQM.Website
{

    public partial class Ucl_EHSRez : System.Web.UI.UserControl
    {
        public static int dfltRowsToScroll = 12;

        static PLANT staticPlant;
        static EHS_MEASURE staticMeasure;
        static EHS_PROFILE_MEASURE staticProfileMeasure;
        static List<EHS_MEASURE> measureList;
        static List<UN_DISPOSAL> disposalList;
        static EHSProfile staticProfile;
        static UOM staticUOM;

        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            pnlEHSProfile.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }
        #endregion

        #region profile

        public EHSProfile LoadPlantProfile(PLANT plant)
        {
            staticProfile = new EHSProfile().Load(plant.PLANT_ID, true);
            ToggleVisible(pnlEHSProfile);
            SetupProfilePanel();
            BindProfile(staticProfile);
            return staticProfile;
        }

        private void BindProfile(EHSProfile profile)
        {
            if (profile != null)
            {
                gvMetricList.DataSource = profile.Profile.EHS_PROFILE_MEASURE.OrderBy(l=> l.EHS_MEASURE.MEASURE_CD);
                gvMetricList.DataBind();
                SQMBasePage.SetGridViewDisplay(gvMetricList, lblMetricListEmpty, divMetricListGVScroll, -1, 0);

                ddlDayDue.SelectedValue = staticProfile.Profile.DAY_DUE.ToString();
                ddlWarningDays.SelectedValue = staticProfile.Profile.REMINDER_DAYS.ToString();

                ddlMetricCurrency.SelectedValue = staticProfile.Plant.CURRENCY_CODE;
                pnlMetricEdit.Enabled = btnMetricCancel.Enabled = btnMetricSave.Enabled = false;
                btnProfileMeasureNew.Enabled = true;
            }
        }

        private void SetupProfilePanel()
        {
            if (ddlMetricDisposalCode.Items.Count == 0)
            {

                ddlDayDue.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 31));
                ddlWarningDays.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(0, 11));

                disposalList = EHSModel.SelectDisposalCodeList(true);
                ddlMetricDisposalCode.DataSource = disposalList;
                ddlMetricDisposalCode.DataValueField = "UN_CODE";
                ddlMetricDisposalCode.DataTextField = "UN_CODE";
                ddlMetricDisposalCode.DataBind();
                ddlMetricDisposalCode.Items.Insert(0, new ListItem(""));

                ddlMetricRegStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("regulatoryStatus"));
                ddlMetricRegStatus.Items.Insert(0, new ListItem(""));

                ddlMetricResponsible.DataSource = SQMModelMgr.SelectPersonList(SessionManager.ActiveCompany().COMPANY_ID, 0, false);
                ddlMetricResponsible.DataValueField = "PERSON_ID";
                ddlMetricResponsible.DataTextField = "EMAIL";
                ddlMetricResponsible.DataBind();
                ddlMetricResponsible.Items.Insert(0, new ListItem(""));
                ddlMetricResponsible.SelectedIndex = 0;

                ddlMetricCurrency.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("currencyCode", "long"));
                ddlMetricCurrency.SelectedValue = "EUR";

                ddlMetricStatus.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("statusCode"));

                ddlMetricCategory.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureCategoryEHS"));
                ddlMetricCategory.Items.Insert(0, "");

                measureList = EHSModel.SelectEHSMeasureList("", true);
                foreach (EHS_MEASURE measure in measureList)
                {
                    ddlMetricID.Items.Add(new ListItem(measure.MEASURE_NAME, WebSiteCommon.PackItemValue(measure.MEASURE_CATEGORY, measure.MEASURE_ID.ToString())));
                }
                ddlMetricID.Items.Insert(0, "");


                foreach (UOM uom in SessionManager.UOMList)
                {
                    if (uom.OWNER_ID.HasValue == false)
                    {
                        ddlMetricUOM.Items.Add(new ListItem(uom.UOM_NAME, WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.UOM_ID.ToString())));
                        ddlUserUOMConvertTo.Items.Add(new ListItem(uom.UOM_NAME, WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.UOM_ID.ToString())));
                    }
                }
                ListItem separator = new ListItem("---------------------", "");
                separator.Attributes.Add("disabled", "true");
                ddlMetricUOM.Items.Add(separator);
                foreach (UOM uom in SessionManager.UOMList)
                {
                    if ((uom.OWNER_ID.HasValue  &&  uom.OWNER_ID == staticProfile.Plant.PLANT_ID))
                        ddlMetricUOM.Items.Add(new ListItem(uom.UOM_NAME, WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.UOM_ID.ToString())));
                }
                ddlMetricUOM.Items.Insert(0, "");
                ddlMetricUOM.SelectedIndex = 0;

            }
            UpdateListTitles();
        }

        protected void ddlUpdateLabel(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            Label lblTarget = null;
            UpdatePanel udpTarget = null;

            switch (ddlSender.ID)
            {
                case "ddlMetricID":
                    lblTarget = lblMetricName;
                    udpTarget = udpMetricID;
                    break;
                case "ddlMetricDisposalCode":
                    lblTarget = lblDisposalDesc;
                    udpTarget = udpDisposal;
                    break;
                default:
                    break;
            }

            UpdateListTitles();
            ListItem item = ddlSender.SelectedItem;
            if (item != null)
            {
                lblTarget.Text = item.Attributes["title"];
            }

            udpTarget.Update();
        }

        protected void ddlCategoryChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            string key = ddlSender.SelectedValue;

            DropDownList ddlTarget = null;
            UpdatePanel udpTarget = null;

            switch (ddlSender.ID)
            {
                case "ddlMetricCategory":
                    ddlTarget = ddlMetricID;
                    udpTarget = udpMetricID;

                    // disable country waste code for non-waste measure categories
                    if (key == "EUTL" || key == "PROD")
                    {
                        tbWasteCode.Enabled = false;
                        tbWasteCode.Text = "";
                    }
                    else
                        tbWasteCode.Enabled = true;

                    udpDisposal.Update();

                    break;
                default:
                    break;
            }

            foreach (ListItem item in ddlTarget.Items)
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
            UpdateListTitles();
            udpTarget.Update();
        }

        protected void ddlUOMChanged(object sender, EventArgs e)
        {
            if (ddlMetricUOM.SelectedValue.Contains("CUST"))
            {
                UOM uom = null;
                if (!string.IsNullOrEmpty(ddlMetricUOM.SelectedValue))
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricUOM.SelectedValue)));
                if (uom != null)
                {
                    tbUserUOMCode.Text = uom.UOM_CD;
                    tbUserUOMName.Text = uom.UOM_DESC;
                }

                udpMetricUOM.Update();
            }
            else
            {
                return;
            }
        }

        protected void btnAddUOM_Click(object sender, EventArgs e)
        {
            divMetricUOM.Visible = false;
            divUserUOM.Visible = true;
            staticUOM = new UOM();

            udpMetricUOM.Update();
        }

        protected void UpdateListTitles()
        {
            foreach (ListItem item in ddlMetricID.Items)
            {
                if (item.Enabled && !string.IsNullOrEmpty(item.Text))
                    item.Attributes.Add("title", measureList.FirstOrDefault(l => l.MEASURE_NAME == item.Text).MEASURE_CD);
            }
            foreach (ListItem item in ddlMetricDisposalCode.Items)
            {
                if (item.Enabled && !string.IsNullOrEmpty(item.Text))
                    item.Attributes.Add("title", disposalList.FirstOrDefault(l => l.UN_CODE == item.Value).DESCRIPTION);
            }
        }

        protected void lnkMetricList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            decimal prmrID = Convert.ToDecimal(lnk.CommandArgument.ToString().Trim());
            BindProfileMeasure(staticProfile.Profile.EHS_PROFILE_MEASURE.FirstOrDefault(l => l.PRMR_ID == prmrID));
            ddlMetricCategory.Focus();
        }

        protected void btnMetricAdd_Click(object sender, EventArgs e)
        {
            hfOper.Value = "add";
            BindProfileMeasure(null);
        }

        public int BindProfileMeasure(EHS_PROFILE_MEASURE pm)
        {
            int status = 0;

            divMetricUOM.Visible = true;
            divUserUOM.Visible = false;
            if (pm == null)
            {
                ddlMetricCategory.SelectedIndex = ddlMetricID.SelectedIndex = ddlMetricDisposalCode.SelectedIndex = ddlMetricUOM.SelectedIndex = ddlMetricResponsible.SelectedIndex = 0;
                ddlMetricCurrency.SelectedValue = staticProfile.Plant.CURRENCY_CODE;
                lblMetricName.Text = lblDisposalDesc.Text = "";
                tbMetricPrompt.Text = "";
            }
            else
            {
                staticProfileMeasure = pm;
                staticMeasure = pm.EHS_MEASURE;

                //  ScriptManager.RegisterStartupScript(this, GetType(), "enablelist", "enableListItems('ddlMetricCategory','ddlMetricID'); enableListItems('ddlMetricUOMCategory','ddlMetricUOM');", true);

                ddlMetricCategory.SelectedValue = pm.EHS_MEASURE.MEASURE_CATEGORY;
                ddlMetricID.SelectedValue = pm.EHS_MEASURE.MEASURE_CATEGORY + "|" + pm.EHS_MEASURE.MEASURE_ID.ToString();
                lblMetricName.Text = pm.EHS_MEASURE.MEASURE_CD;
                tbMetricPrompt.Text = pm.MEASURE_PROMPT;
                ddlMetricRegStatus.SelectedValue = pm.REG_STATUS;
                ddlMetricDisposalCode.SelectedValue = pm.UN_CODE;
                if (!string.IsNullOrEmpty(pm.UN_CODE))
                    lblDisposalDesc.Text = disposalList.FirstOrDefault(l => l.UN_CODE == pm.UN_CODE).DESCRIPTION;
                else
                    lblDisposalDesc.Text = "";

                if (pm.EHS_MEASURE.MEASURE_CATEGORY == "EUTL" || pm.EHS_MEASURE.MEASURE_CATEGORY == "PROD")
                {
                    tbWasteCode.Enabled = false;
                    tbWasteCode.Text = "";
                }
                else
                {
                    tbWasteCode.Enabled = true;
                    tbWasteCode.Text = pm.WASTE_CODE;
                }

                ddlMetricCurrency.SelectedValue = pm.DEFAULT_CURRENCY_CODE;
                if (pm.RESPONSIBLE_ID > 0)
                    ddlMetricResponsible.SelectedValue = pm.RESPONSIBLE_ID.ToString();
                if (pm.DEFAULT_UOM > 0)
                {
                    UOM uom = staticUOM = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == pm.DEFAULT_UOM);
                    if (uom != null)
                    {
                        ddlMetricUOM.SelectedValue = WebSiteCommon.PackItemValue(uom.UOM_CATEGORY, uom.UOM_ID.ToString());
                        if (uom.UOM_CATEGORY == "CUST")
                        {
                            divUserUOM.Visible = true;
                            tbUserUOMName.Text = uom.UOM_NAME;
                            tbUserUOMCode.Text = uom.UOM_CD;
                            UOM_XREF xref = uom.UOM_XREF.FirstOrDefault();
                            if (xref != null)
                            {
                                UOM refUOM = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == xref.UOM_TO);
                                if (refUOM != null)
                                {
                                    ddlUserUOMConvertTo.SelectedValue = WebSiteCommon.PackItemValue(refUOM.UOM_CATEGORY, refUOM.UOM_ID.ToString());
                                    tbUserUOMConversionFactor.Text = SQMBasePage.FormatValue(xref.CONVERSION, 4);
                                }
                            }
                        }
                    }
                }

                ddlMetricStatus.SelectedValue = pm.STATUS;

                cbMetricNegValue.Checked = (bool)pm.NEG_VALUE_ALLOWED;
                cbMetricRequired.Checked = (bool)pm.IS_REQUIRED;
            }

            UpdateListTitles();
            pnlMetricEdit.Enabled = btnMetricCancel.Enabled = btnMetricSave.Enabled = true;

            return status;
        }

        protected void btnMetricSave_Click(object sender, EventArgs e)
        {
            bool success;
            EHS_PROFILE_MEASURE pm = null;

            if (hfOper.Value == "add")
            {
                pm = staticProfile.AddMeasure(Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricID.SelectedValue)), Convert.ToDecimal(ddlMetricResponsible.SelectedValue));
            }
            else
            {
                pm =  staticProfile.Profile.EHS_PROFILE_MEASURE.FirstOrDefault(l => l.PRMR_ID == staticProfileMeasure.PRMR_ID);
            }

            pm.PLANT_ID = staticProfile.Profile.PLANT_ID;
            pm.MEASURE_ID = Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricID.SelectedValue));
            //pm.EHS_MEASURE = EHSModel.LookupEHSMeasure(new PSsqmEntities(), pm.MEASURE_ID, "");
            pm.MEASURE_PROMPT = tbMetricPrompt.Text;
            pm.REG_STATUS = ddlMetricRegStatus.SelectedValue;
            pm.UN_CODE = ddlMetricDisposalCode.SelectedValue;
            pm.DEFAULT_CURRENCY_CODE = ddlMetricCurrency.SelectedValue;
            if (ddlMetricResponsible.SelectedIndex > 0)
                pm.RESPONSIBLE_ID = Convert.ToDecimal(ddlMetricResponsible.SelectedValue);
            else
                pm.RESPONSIBLE_ID = Convert.ToDecimal(null);

            pm.IS_REQUIRED = cbMetricRequired.Checked;
            pm.NEG_VALUE_ALLOWED = cbMetricNegValue.Checked;

            if (cbUserUOM.Checked)
            {
                staticUOM.UOM_CATEGORY = "CUST";
                staticUOM.UOM_CD = tbUserUOMCode.Text;
                staticUOM.UOM_NAME = staticUOM.UOM_DESC = tbUserUOMName.Text;
                staticUOM.OWNER_ID = staticProfile.Plant.PLANT_ID;
                double conversionFactor = 0;
                double.TryParse(tbUserUOMConversionFactor.Text.Trim(), out conversionFactor);
                if ((staticUOM = SQMResourcesMgr.UpdateUOM(new PSsqmEntities(), staticUOM, Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlUserUOMConvertTo.SelectedValue)), conversionFactor)) != null)
                {
                    SessionManager.UOMList.Add(staticUOM);
                    ddlMetricUOM.Items.Add(new ListItem(staticUOM.UOM_NAME, WebSiteCommon.PackItemValue(staticUOM.UOM_CATEGORY, staticUOM.UOM_ID.ToString())));
                    pm.DEFAULT_UOM = staticUOM.UOM_ID;
                }
                cbUserUOM.Checked = false;
            }
            else
            {
                pm.DEFAULT_UOM = Convert.ToDecimal(WebSiteCommon.ParseItemValue(ddlMetricUOM.SelectedValue));
            }

            BindProfile(staticProfile);
        }

        protected void btnProfileSave_Click(object sender, EventArgs e)
        {
            staticProfile.Profile.DAY_DUE = Convert.ToInt32(ddlDayDue.SelectedValue);
            staticProfile.Profile.REMINDER_DAYS = Convert.ToInt32(ddlWarningDays.SelectedValue);

            staticProfile = EHSProfile.UpdateProfile(staticProfile);
            BindProfile(staticProfile);
        }

        public void gvOnProfileRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Image img = (Image)e.Row.Cells[0].FindControl("imgHazardType");
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfMetricCategory");
                    if (hf.Value == "EUTL")
                    {
                        e.Row.Cells[0].CssClass = e.Row.Cells[1].CssClass = "energyColor";
                        img.ImageUrl = "~/images/status/energy.png";
                    }
                    else if (hf.Value == "PROD")
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
                            img.ToolTip += (".  " + disposalList.FirstOrDefault(l => l.UN_CODE == hf.Value).DESCRIPTION);
                    }
                  //      e.Row.Cells[0].Attributes.Add("Style", "background: wheat;");
                    LinkButton lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkMetricCD");
                    LinkButton lnk2 = (LinkButton)e.Row.Cells[0].FindControl("lnkMetricName");
                    lnk.ToolTip = lnk2.ToolTip = WebSiteCommon.GetXlatValue("measureCategoryEHS", hf.Value);
 
                }
                catch
                {
                }
            }
        }
        #endregion

    }
}