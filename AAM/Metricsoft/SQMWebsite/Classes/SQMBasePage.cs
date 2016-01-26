using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SQM.Shared;

namespace SQM.Website
{
    /// <summary>
    /// Base page for the entire SQM project; all pages should inherit from this
    /// </summary>
    ///
    public enum PageUseMode  {Active, EditEnabled, EditPartial, ViewOnly};

    public class SQMBasePage : System.Web.UI.Page
    {
        public SQM.Website.PSsqmEntities entities = new PSsqmEntities();
        public string masterContentContainer = null;
        public string moduleContentContainer = "ContentPlaceHolder1";
        public string pageContentContainer = "ContentPlaceHolder_Body";
        public static int dfltRowsToScroll = 20;
        public static int dfltRowsToScrollLong = (dfltRowsToScroll * 2);

        public static int GetSessionTimeout()
        {
            return HttpContext.Current.Session.Timeout;
        }

        protected override void InitializeCulture()
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.SessionContext != null)
                {
					try
					{
						string lang = SessionManager.SessionContext.Language().NLS_LANGUAGE;
						UICulture = lang;
						Culture = lang;
						System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
						System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
						if (CultureSettings.gregorianCalendarOverrides.Contains(lang))
							System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
					}
					catch { }
                }
                base.InitializeCulture();
            }
        }

        public static string CultureInfo(int option)
        {
            string cultureInfo = "";
            CultureInfo culture = new CultureInfo(SessionManager.SessionContext.Language().NLS_LANGUAGE);

            cultureInfo = culture.NativeName;
            return cultureInfo;
        }

        public bool IsCurrentPage()
        {
            return IsCurrentPage(this.Page.Request.Url.AbsolutePath);
        }
        public static bool IsCurrentPage(string urlAbsolutePath)
        {
            if (!string.IsNullOrEmpty(SessionManager.CurrentAdminPage) && urlAbsolutePath.Contains(SessionManager.CurrentAdminPage))
                return true;
            else
            {
                SessionManager.CurrentAdminPage = urlAbsolutePath;
                return false;
            }
        }

        public void SetActiveTab(string tabID)
        {
            HiddenField hf = (HiddenField)this.Master.FindControl("hdCurrentActiveSecondaryTab");
            if (hf != null)
                hf.Value = tabID;
        }


        public static string FormatValue(decimal value, int precision, bool truncateZeros)
        {
            string strValue = FormatValue(value, precision);
            if (truncateZeros)
            {
                int pos = strValue.Length;
                for (int n = strValue.Length - 1; n >= 0; n--)
                {
                    if (strValue[n] != '0')
                    {
                        if (n < strValue.Length-1  &&  strValue[n] == '.' || strValue[n] == ',')
                            pos = n + 2;
                        else
                            pos = n+1;
                        break;
                    }
                }
                strValue = strValue.Substring(0, pos);
            }

            return strValue;
        }

        public static string FormatValue(decimal value, int precision)
        {
            string strValue = "";
            try
            {
                if (value != null)
                {
                    CultureInfo culture;
                    if (SessionManager.SessionContext != null && SessionManager.SessionContext.Language() != null)
                        culture = new CultureInfo(SessionManager.SessionContext.Language().NLS_LANGUAGE);
                    else
                        culture = new CultureInfo("en");

                    strValue = value.ToString("N" + precision.ToString(), culture);
                }
            }
            catch
            { ; }
            return strValue;
        }

        public static string FormatDate(DateTime date, string fmtIn, bool includeTime)
        {
            string strValue = "";
            string fmt = "d";
            if (!string.IsNullOrEmpty(fmtIn))
                fmt = fmtIn;

            try
            {
                CultureInfo culture = new CultureInfo(SessionManager.SessionContext.Language().NLS_LANGUAGE);
                strValue = date.ToString(fmt, culture);
            }
            catch { }

            return strValue;
        }

        public static bool ParseToDateTime(string strValue, out DateTime date)
        {
            date = DateTime.MinValue;
            bool success = DateTime.TryParse(strValue, out date);
            return success;
        }
        public static bool ParseToDecimal(string strValue, out decimal decimalValue)
        {
            decimalValue = 0;
            bool success = Decimal.TryParse(strValue, out decimalValue);

            return success;
        }

        public static string InsertDateCriteria(string inString, DateTime fromDate, DateTime toDate)
        {
            string outString = inString;
            string arg, s2;
            int l1 = 0, l2;

            while (l1 > -1)
            {
                if ((l1 = outString.IndexOf("[")) > -1)
                {
                    l2 = outString.IndexOf("]", l1);
                    arg = outString.Substring(l1 + 1, l2 - l1 - 1);
                    string cmd = arg.Substring(0, 3);
                    string fmt = arg.Substring(4, arg.Length - 4);

                    outString = outString.Remove(l1, l2 - l1 + 1);

                    switch (cmd)
                    {
                        case "DT1":
                            outString = outString.Insert(l1, FormatDate(fromDate, fmt, false));
                            break;
                        case "DT2":
                            outString = outString.Insert(l1, FormatDate(toDate, fmt, false));
                            break;
                        case "FYY":
                            outString = outString.Insert(l1, (fromDate.Year+1).ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
 
            return outString;
        }


		public static RadDatePicker SetRadDateCulture(RadDatePicker rdp, string langOverride)
		{
			string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
			string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";
			if (CultureSettings.gregorianCalendarOverrides.Contains(language))
			{
				rdp.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
				rdp.DateInput.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
			}

			return rdp;
		}

        public static void FillCurrencyDDL(DropDownList ddl, string defaultCode)
        {
            // fill dropdown list with standard currency codes
            ddl.DataSource = SessionManager.CurrencyList;
            ddl.DataTextField = "CURRENCY_NAME";
            ddl.DataValueField = "CURRENCY_CODE";
            ddl.DataBind();
            if (!string.IsNullOrEmpty(defaultCode))
                ddl.SelectedValue = defaultCode;
        }

        public static void FillCurrencyDDL(RadComboBox ddl, string defaultCode, bool displayLong)
        {
            // fill dropdown list with standard currency codes
            if (displayLong)
            {
                ddl.DataSource = SessionManager.CurrencyList.OrderBy(l => l.CURRENCY_NAME);
                ddl.DataTextField = "CURRENCY_NAME";
                ddl.DataValueField = "CURRENCY_CODE";
                ddl.DataBind();
            }
            else
            {
                foreach (CURRENCY cur in SessionManager.CurrencyList.OrderBy(l=> l.CURRENCY_CODE))
                {
                    RadComboBoxItem item = new RadComboBoxItem(cur.CURRENCY_CODE, cur.CURRENCY_CODE);
                    item.ToolTip = cur.CURRENCY_NAME;
                    ddl.Items.Add(item);
                }
            }
            
            if (!string.IsNullOrEmpty(defaultCode))
                ddl.SelectedValue = defaultCode;
        }

        public static DropDownList SetPersonList(DropDownList ddl, List<PERSON> personSelectList, string personID, bool fullName)
        {
            ddl.Items.Clear();
            ListItem item = null;
            foreach (PERSON person in personSelectList)
            {
                item = new ListItem(SQMModelMgr.FormatPersonListItem(person, fullName), person.PERSON_ID.ToString());
                ddl.Items.Add(item);
            }
     
            ddl.Items.Insert(0, new ListItem("", ""));

            if (!string.IsNullOrEmpty(personID) && ddl.Items.FindByValue(personID) != null)
                ddl.SelectedValue = personID;
            else
                ddl.SelectedIndex = 0;

            return ddl;
        }

        public static RadComboBox SetPersonList(RadComboBox ddl, List<PERSON> personSelectList, string personID)
        {
            return SetPersonList(ddl, personSelectList, personID, 0, false);
        }
		public static RadComboBox SetPersonList(RadComboBox ddl, List<PERSON> personSelectList, string personID, int rowLimit)
		{
			return SetPersonList(ddl, personSelectList, personID, rowLimit, false);
		}
		public static RadComboBox SetPersonList(RadComboBox ddl, List<PERSON> personSelectList, string personID, int rowLimit, bool fullName)
		{
			return SetPersonList(ddl, personSelectList, personID, rowLimit, fullName, "LF");
		}
        public static RadComboBox SetPersonList(RadComboBox ddl, List<PERSON> personSelectList, string personID, int rowLimit, bool fullName, string nameOrder)
        {
            ddl.Items.Clear();
            RadComboBoxItem item = null;
            foreach (PERSON person in personSelectList)
            {
                item = new RadComboBoxItem(SQMModelMgr.FormatPersonListItem(person, fullName, nameOrder), person.PERSON_ID.ToString());
                item.ToolTip = person.EMAIL;
                if (ddl.CheckBoxes)
                {
                    item.Checked = false;
                }
                ddl.Items.Add(item);
            }
            if (!ddl.CheckBoxes)
                ddl.Items.Insert(0, new RadComboBoxItem("", ""));

            if (!string.IsNullOrEmpty(personID) && ddl.Items.FindItemByValue(personID) != null)
            {
                if (ddl.CheckBoxes)
                    ddl.Items.FindItemByValue(personID).Checked = true;
                else
                    ddl.SelectedValue = personID;
            }

            if (rowLimit > 0  &&  ddl.Items.Count > rowLimit)
            {
                ddl.DropDownCssClass = "multipleRowsColumns";
                ddl.DropDownWidth = new System.Web.UI.WebControls.Unit(560);
            }

            return ddl;
        }

        public static RadComboBoxItem SetLocationItem(BusinessLocation loc, bool formal)
        {
            RadComboBoxItem item = null;

            if (loc != null)
            {
                if (formal)
                {
                    item = new RadComboBoxItem(loc.Company.COMPANY_NAME + ", " + loc.Plant.PLANT_NAME, loc.Plant.PLANT_ID.ToString());
                }
                else
                {
                    item = new RadComboBoxItem(loc.Plant.PLANT_NAME, loc.Plant.PLANT_ID.ToString());
                }

                ADDRESS address = loc.Plant.ADDRESS.FirstOrDefault();
                if (address != null)
                    item.ToolTip = address.STREET1 + " " + address.CITY;
            }

            return item;
        }

        public static RadComboBox SetLocationList(RadComboBox ddl, List<BusinessLocation> locationList, decimal plantID)
        {
            return SetLocationList(ddl, locationList, plantID, false);
        }

        public static RadComboBox SetLocationList(RadComboBox ddl, List<BusinessLocation> locationList, decimal plantID, bool enableBUSelect)
        {
            ddl.Items.Clear();
            RadComboBoxItem item = null;
            ADDRESS address = null;
            decimal busOrgID = 0;

            int numOrgs = locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count();

            foreach (BusinessLocation loc in locationList.OrderBy(l=> l.Plant.BUS_ORG_ID).ThenBy(l=> l.Plant.PLANT_NAME).ToList())
            {
                if (numOrgs > 1  &&   loc.Plant.BUS_ORG_ID != busOrgID)
                {
                    busOrgID = (decimal)loc.Plant.BUS_ORG_ID;
                    item = new RadComboBoxItem(loc.BusinessOrg.ORG_NAME, ("BU"+loc.BusinessOrg.BUS_ORG_ID.ToString()));
                    if (enableBUSelect)
                    {
                        item.BackColor = System.Drawing.Color.Linen;
                        item.ToolTip = loc.BusinessOrg.DUNS_CODE;
                    }
                    else
                    {
                        item.IsSeparator = true;
                        if (ddl.CheckBoxes)
                        {
                            item.Checked = false;
                        }
                    }
                    item.ImageUrl = "~/images/defaulticon/16x16/sitemap.png";
                    ddl.Items.Add(item);
                }

                item = new RadComboBoxItem(loc.Plant.PLANT_NAME, loc.Plant.PLANT_ID.ToString());
                item.IsSeparator = false;
                if ((address = loc.Plant.ADDRESS.FirstOrDefault()) != null)
                    item.ToolTip = address.STREET1 + " " + address.CITY;
                if (plantID < 0)
                    item.Checked = true;
                ddl.Items.Add(item);
            }
            //ddl.Items.Insert(0, new RadComboBoxItem("", ""));

            if (plantID > 0 && ddl.Items.FindItemByValue(plantID.ToString()) != null)
            {
                if (ddl.CheckBoxes)
                {
                    ddl.FindItemByValue(plantID.ToString()).Checked = true;
                }
                else
                {
                    ddl.SelectedValue = plantID.ToString();
                }
            }

            return ddl;
        }

        public static RadComboBox SetPlantList(RadComboBox ddl, List<PLANT> plantList, decimal plantID)
        {
            ddl.Items.Clear();
            RadComboBoxItem item = null;
            ADDRESS address = null;
            foreach (PLANT plant in plantList.OrderBy(l=> l.BUS_ORG_ID).ThenBy(l=> l.PLANT_NAME).ToList())
            {
                item = new RadComboBoxItem(plant.PLANT_NAME, plant.PLANT_ID.ToString());
                if ((address = plant.ADDRESS.FirstOrDefault()) != null)
                    item.ToolTip = address.STREET1 + " " + address.CITY;
                if (plantID < 0)
                    item.Checked = true;
                ddl.Items.Add(item);
            }
            //ddl.Items.Insert(0, new RadComboBoxItem("", ""));

            if (plantID > 0 && ddl.Items.FindItemByValue(plantID.ToString()) != null)
            {
                ddl.SelectedValue = plantID.ToString();
            }

            return ddl;
        }

		public static RadComboBox SetComboBoxItemsFromXLAT(RadComboBox ddl, List<XLAT> xlatList, string shortLong)
		{
			ddl.Items.Clear();

			foreach (XLAT xlat in xlatList)
			{
				if (shortLong.ToUpper() == "SHORT")
					ddl.Items.Add(new RadComboBoxItem(xlat.DESCRIPTION_SHORT, xlat.XLAT_CODE));
				else
					ddl.Items.Add(new RadComboBoxItem(xlat.DESCRIPTION, xlat.XLAT_CODE));
			}

			return ddl;
		}

        public static List<RadComboBoxItem> GetComboBoxCheckedItems(RadComboBox ddl)
        {
            List<RadComboBoxItem> itemList = new List<RadComboBoxItem>();
            itemList.AddRange(ddl.Items.Where(l => l.Checked && l.IsSeparator == false).ToList());

            return itemList;
        }

        public static List<string> GetComboBoxSelectedValues(RadComboBox ddl)
        {
            List<string> valueList = new List<string>();

            if (ddl.CheckBoxes)
            {
                valueList.AddRange(GetComboBoxCheckedItems(ddl).Select(l=> l.Value).ToList());
            }
            else
            {
                valueList.Add(ddl.SelectedValue);
            }

            return valueList;
        }

        public static RadMenu SetLocationList(RadMenu mnu, List<BusinessLocation> locationList, decimal plantID, string topLabel, string topValue, bool enableBUSelect)
        {
            ADDRESS address = null;
            int numOrgs = locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count();

            Telerik.Web.UI.RadMenuItem miTop = new Telerik.Web.UI.RadMenuItem();
            miTop.Text = topLabel;
            miTop.Value = "TOP";
            miTop.Font.Bold = true;
            mnu.Items.Add(miTop);

            decimal busOrgID = 0;
            Telerik.Web.UI.RadMenuItem miBU = null;
            foreach (BusinessLocation loc in locationList.OrderBy(l => l.Plant.BUS_ORG_ID).ThenBy(l => l.Plant.PLANT_NAME).ToList())
            {
                if (loc.Plant.BUS_ORG_ID != busOrgID)
                {
                    if (miBU != null)
                        miTop.Items.Add(miBU);
                    busOrgID = (decimal)loc.Plant.BUS_ORG_ID;
                    miBU = new Telerik.Web.UI.RadMenuItem();
                    miBU.ImageUrl = "~/images/defaulticon/16x16/sitemap.png";
                    miBU.Text = loc.BusinessOrg.ORG_NAME; miBU.Value = "BU" + loc.BusinessOrg.BUS_ORG_ID.ToString();
                    if (!enableBUSelect)
                    {
                        ; // how to disable selecting ??
                    }
                }
                Telerik.Web.UI.RadMenuItem miLoc = new Telerik.Web.UI.RadMenuItem();
                miLoc.Text = loc.Plant.PLANT_NAME; 
                miLoc.Value = loc.Plant.PLANT_ID.ToString();
                if ((address = loc.Plant.ADDRESS.FirstOrDefault()) != null)
                    miLoc.ToolTip = address.STREET1 + " " + address.CITY;
                miBU.Items.Add(miLoc);
                if (plantID > 0 && plantID.ToString() == miLoc.Value)  // is default plant
                {
                    miLoc.Selected = true;
                    mnu.Items[0].Text = miLoc.Text;
                }
            }
            if (miBU != null)
            {
                miTop.Items.Add(miBU);
            }

            return mnu;
        }

        public static object DisplayControlValue(object oCtl, string value, PageUseMode mode, string cssClass)
        {
            return DisplayControlValue(oCtl, value, mode, cssClass, "");
        }

        public static object DisplayControlValue(object oCtl, string value, PageUseMode mode, string cssClass, string literalControl)
        {
            Label lbl = null;
            string lblText = value;

            try
            {
                Control ctl = (Control)oCtl;
                Control parent = ctl.Parent;
                int idx = parent.Controls.IndexOf(ctl);

                if (oCtl is TextBox)
                {
                    TextBox tb = (TextBox)oCtl;
                    tb.Text = lblText = value;
                }

                if (oCtl is DropDownList)
                {
                    DropDownList ddl = (DropDownList)oCtl;
                    if (!string.IsNullOrEmpty(value))
                        ddl.SelectedValue = value;
                    lblText = ddl.SelectedItem.Text;
                }

                if (oCtl is RadComboBox)
                {
                    RadComboBox rddl = (RadComboBox)oCtl;

                    if (rddl.CheckBoxes == true)
                    {
                        foreach (RadComboBoxItem item in rddl.CheckedItems)
                        {
                            lblText += (lblText.Length == 0 ? item.Text : (", " + item.Text));
                        }
                    }
                    else
                    {

                        if (rddl.Items.FindItemByValue(value) != null)
                        {
                            rddl.SelectedValue = value;
                            lblText = rddl.SelectedItem.Text;
                        }
                        else
                        {
                            lblText = "";
                        }
                    }
                }

                if (oCtl is RadDatePicker)
                {
                    RadDatePicker rd = (RadDatePicker)oCtl;
                    rd.SelectedDate = null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        DateTime dt = Convert.ToDateTime(value);
                        if (dt > DateTime.MinValue)
                            rd.SelectedDate = dt;
                    }
                    if (rd.SelectedDate != null)
                        lblText = FormatDate((DateTime)rd.SelectedDate, "d", false);
                }

                if (oCtl is RadSearchBox)
                {
                    ((RadSearchBox)oCtl).Text = value;
                }

                if (mode != PageUseMode.EditEnabled)
                {
                    lbl = new Label();
                    lbl.Text = lblText;
                    lbl.CssClass = cssClass;
                    parent.Controls.AddAt(idx, lbl);
                    ctl.Visible = false;
                }
                if (!string.IsNullOrEmpty(literalControl))
                {
                    parent.Controls.AddAt(idx, new LiteralControl(literalControl));
                }
            }

            catch (Exception ex)
            {
               // SQMLogger.LogException(ex);
                ;
            }

            return (object)lbl;
        }

        public static void EnableControls(ControlCollection controlCollection, bool enable)
        {
            string controlName;
            string controlType;

            foreach (Control control in controlCollection)
            {
                controlName = control.ID;
                controlType = control.GetType().ToString();
                controlType = controlType.Substring(controlType.LastIndexOf('.') + 1);

                if (controlName != null)
                {
                    switch (controlType)
                    {
                        case "LinkButton":
                            LinkButton lbtn = (LinkButton)control;
                            if (enable)
                            {
                                lbtn.CssClass = "buttonLink";
                            }
                            else
                            {
                                lbtn.CssClass = "optNavDisable";
                            }
                            lbtn.Enabled = enable;
                            break;
                        case "Button":
                            Button btn = (Button)control;
                            btn.Enabled = enable;
                            break;
                        case "TextBox":
                            TextBox tb = (TextBox)control;
                           // tb.Enabled = enable;
                            if (!enable)
                                tb.ReadOnly = true;
                            else
                            {
                                tb.ReadOnly = false;
                                tb.Enabled = true;
                            }
                            break;
                        case "CheckBox":
                            CheckBox cb = (CheckBox)control;
                            cb.Enabled = enable;
                            break;
                        case "RadComboBox":
                            RadComboBox cdl = (RadComboBox)control;
                            cdl.Enabled = enable;
                            break;
                        case "RadDatePicker":
                            RadDatePicker rd = (RadDatePicker)control;
                            //rd.Calendar.Visible = false;
                           // rd.DateInput.ReadOnly = true;
                            rd.Enabled = enable;
                            break;
                        case "DropDownList":
                            DropDownList ddl = (DropDownList)control;
                            ddl.Enabled = enable;
                            break;
                        case "ListBox":
                            ListBox lx = (ListBox)control;
                            if (enable)
                                lx.Attributes.Remove("disabled");
                            else
                                lx.Attributes.Add("disabled", "true");
                            break;
                        case "GridView":
                            GridView gv = (GridView)control;
                         //   gv.Enabled = enable;
                            EnableControls(gv.Controls, enable);
                            break;
                        case "rb":
                            RadioButtonList rb = (RadioButtonList)control;
                            rb.Enabled = enable;
                            break;
                        default:
                            break;
                    }
                }
                if (control.HasControls())
                    EnableControls(control.Controls, enable);
            }
        }

        public static void DisplayButtons(ControlCollection controlCollection, bool visible)
        {
            string controlName;
            string controlType;

            foreach (Control control in controlCollection)
            {
                controlName = control.ID;
                controlType = control.GetType().ToString();
                controlType = controlType.Substring(controlType.LastIndexOf('.') + 1);

                if (controlName != null)
                {
                    switch (controlType)
                    {
                        case "LinkButton":
                            LinkButton lbtn = (LinkButton)control;
                            lbtn.Visible = visible;
                            break;
                        case "Button":
                            Button btn = (Button)control;
                            btn.Visible = visible;
                            break;
                        default:
                            break;
                    }
                }
                if (control.HasControls())
                    DisplayButtons(control.Controls, visible);
            }
        }

        public static void ResetControlValues(ControlCollection controlCollection)
        {
            string controlName;
            string controlType;

            foreach (Control control in controlCollection)
            {
                try
                {
                    controlName = control.ID;
                    controlType = control.GetType().ToString();
                    controlType = controlType.Substring(controlType.LastIndexOf('.') + 1);

                    if (controlName != null)
                    {
                        switch (controlType)
                        {
                            case "TextBox":
                                TextBox tb = (TextBox)control;
                                tb.Text = "";
                                break;
                            case "CheckBox":
                                CheckBox cb = (CheckBox)control;
                                cb.Checked = false;
                                break;
                            case "DropDownList":
                                DropDownList ddl = (DropDownList)control;
                                ddl.SelectedIndex = 0;
                                break;
                            case "ListBox":
                                ListBox lx = (ListBox)control;
                                lx.SelectedIndex = 0;
                                break;
                            case "RadioButtonList":
                                RadioButtonList rb = (RadioButtonList)control;
                                rb.SelectedIndex = 0;
                                break;
                            case "GridView":
                                GridView gv = (GridView)control;
                                gv.DataSource = null;
                                gv.DataBind();
                                break;
                            default:
                                break;
                        }
                    }
                    if (control.HasControls())
                        ResetControlValues(control.Controls);
                }
                catch
                {
                }
            }

        }

        public void ScanPageControls(string pageName, ControlCollection controlCollection, bool includeMasterPages)
        {
            SessionContext sessionContext = SessionManager.SessionContext;
            string labelName;
            string labelText;
            string labelType;
            string containerID;
            foreach (Control control in controlCollection)
            {
                labelText = "";
                labelName = control.ID;
                labelType = control.GetType().ToString();
                labelType = labelType.Substring(labelType.LastIndexOf('.') + 1);
                containerID = control.NamingContainer.ID;

                if ((labelName != null  &&  !labelName.Contains("_out"))  &&  (includeMasterPages || containerID == pageContentContainer))
                {
                    switch (labelType)
                    {
                        case "Label":
                            Label lbl = (Label)control;
                            labelText = lbl.Text;
                            break;
                        case "LinkButton":
                            LinkButton lbtn = (LinkButton)control;
                            labelText = lbtn.Text;
                            break;
                        case "Button":
                            Button btn = (Button)control;
                            labelText = btn.Text;
                            break;
                        /*
                        case "TextBox":
                            TextBox tb = (TextBox)control;
                            labelText = tb.Text;
                            break;
                        */
                        case "CheckBox":
                            CheckBox cb = (CheckBox)control;
                            labelText = cb.Text;
                            break;
                        case "HiddenField":
                            HiddenField hf = (HiddenField)control;
                            labelText = hf.Value;
                            break;
                        case "DropDownList":
                            if (control.ID.Contains("_xlat"))
                            {
                                DropDownList ddl = (DropDownList)control;
                                labelText = ddl.Text;
                            }
                            break;
                        case "GridView":
                            GridView gv = (GridView)control;
                            int nFld = 0;
                            foreach (DataControlField fld in gv.Columns)
                            {
                                labelName = (gv.ID + "_" + nFld.ToString());
                                labelText = fld.HeaderText;
                               // SessionManager.SessionContext = UpdatePageLabel(sessionContext, pageName, containerID, labelName, labelType, labelText);
                                ++nFld;
                            }
                            labelText = "";
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(labelText))
                {
                   // SessionManager.SessionContext = UpdatePageLabel(sessionContext, pageName, containerID, labelName, labelType, labelText);
                }

                if (control.HasControls())
                    ScanPageControls(pageName, control.Controls, includeMasterPages);
            }
        }

        #region UIhelpers

        public bool SetFindControlValue(string ctlID, HiddenField hfBase, string value)
        {
            return SetFindControlValue(ctlID, hfBase, value, null);
        }

        public bool SetFindControlValue(string ctlID, HiddenField hfBase, string value, bool? enabled)
        {
            bool success = false;

            try
            {
                string ctlType = ctlID.Substring(0, 2);
                switch (ctlType)
                {
                    case "cb":
                        CheckBox cb = (CheckBox)hfBase.FindControl(ctlID);
                        cb.Checked = Convert.ToBoolean(value);
                        if (enabled != null)
                            cb.Enabled = (bool)enabled;
                        success = true;
                        break;
                    case "dd":
                        DropDownList ddl = (DropDownList)hfBase.FindControl(ctlID);
                        ddl.SelectedValue = value;
                        if (enabled != null)
                            ddl.Enabled = (bool)enabled;
                        success = true;
                        break;
                    case "hf":
                        HiddenField hf = (HiddenField)hfBase.FindControl(ctlID);
                        hf.Value = value;
                        success = true;
                        break;
                    case "lb":
                        Label lbl = (Label)hfBase.FindControl(ctlID);
                        lbl.Text = value;
                        success = true;
                        break;
                    case "rb":
                        RadioButtonList rb = (RadioButtonList)hfBase.FindControl(ctlID);
                        rb.SelectedValue = value;
                        if (enabled != null)
                        {
                            rb.Enabled = (bool)enabled;
                            foreach (ListItem item in rb.Items)
                            {
                                item.Enabled = (bool)enabled;
                            }
                        }
                        success = true;
                        break;
                    case "tb":
                        TextBox tb = (TextBox)hfBase.FindControl(ctlID);
                        tb.Text = value;
                        if (enabled != null)
                            tb.Enabled = (bool)enabled;
                        success = true;
                        break;
                    case "lx": // set single selection
                        ListBox lbx = (ListBox)hfBase.FindControl(ctlID);
                        foreach (ListItem item in lbx.Items)
                        {
                            item.Selected = false;
                            if (value == item.Value)
                            {
                                item.Selected = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch
            {
            }

            return success;
        }

        public string GetFindControlValue(string ctlID, HiddenField hfBase, out bool success)
        {
            success = false;
            string value = "";

            try
            {
                string ctlType = ctlID.Substring(0, 2);
                switch (ctlType)
                {
                    case "cb":
                        CheckBox cb = (CheckBox)hfBase.FindControl(ctlID);
                        value = cb.Checked.ToString();
                        success = true;
                        break;
                    case "dd":
                        DropDownList ddl = (DropDownList)hfBase.FindControl(ctlID);
                        if (ddl.SelectedIndex >= 0)
                        {
                            value = ddl.SelectedValue;
                            success = true;
                        }
                        break;
                    case "hf":
                        HiddenField hf = (HiddenField)hfBase.FindControl(ctlID);
                        value = hf.Value;
                        success = true;
                        break;
                    case "lb":
                        Label lbl = (Label)hfBase.FindControl(ctlID);
                        value = lbl.Text;
                        success = true;
                        break;
                    case "rb":
                        RadioButtonList rb = (RadioButtonList)hfBase.FindControl(ctlID);
                        value = rb.SelectedValue;
                        success = true;
                        break;
                    case "tb":
                        TextBox tb = (TextBox)hfBase.FindControl(ctlID);
                        value = tb.Text;
                        success = true;
                        break;
                    case "lx": // get single selected item 
                        ListBox lbx = (ListBox)hfBase.FindControl(ctlID);
                        value = lbx.SelectedValue;
                        success = true;
                        break;
                    default:
                        break;
                }
            }
            catch
            {
            }

            return value;
        }

        public void SetGridViewDisplay(GridView gv, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll)
        {
            SetGridViewDisplay(gv, lblAlert, divScroll, rowsToScroll, 0);
        }

        public static void SetGridViewDisplay(GridView gv, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount)
        {
            if (gv.Rows.Count == 0)
            {
                gv.Visible = false;
                lblAlert.Visible = true;
            }
            else
            {
                gv.Visible = true;
                lblAlert.Visible = false;
                if (rowsToScroll > -1)
                {
                    int gridRows = gridRowCount;
                    if (gridRows == 0)
                        gridRows = gv.Rows.Count;
                    int rowLimit = rowsToScroll;
                    if (rowLimit == 0)
                        rowLimit = dfltRowsToScroll;
                    if (gridRows > rowLimit && divScroll != null)
                    {
                        divScroll.Attributes["class"] = "scrollArea";
                    }
                }
            }
        }

		public void SetRepeaterViewDisplay(Repeater rpt, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, string className)
		{
			SetRepeaterViewDisplay(rpt, lblAlert, divScroll, rowsToScroll, 0, className);
		}

		public void SetRepeaterViewDisplay(Repeater rpt, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int repeatRowCount, string className)
		{
			if (rpt.Items.Count == 0)
			{
				rpt.Visible = false;
				lblAlert.Visible = true;
			}
			else
			{
				rpt.Visible = true;
				lblAlert.Visible = false;
				int repeatRows = repeatRowCount;
				if (repeatRows == 0)
					repeatRows = rpt.Items.Count;
				int rowLimit = rowsToScroll;
				if (rowLimit == 0)
					rowLimit = dfltRowsToScroll;
				if (repeatRows > rowLimit && divScroll != null)
				{
					divScroll.Attributes["class"] = className;
				}
			}
		}

		public int SetListSelectedTextValue(DropDownList ddl, string textValue)
        {
            int selectedIndex = -1;

            int nItem = -1;
            foreach (ListItem item in ddl.Items)
            {
                ++nItem;
                if (item.Text == textValue)
                {
                    item.Selected = true;
                    ddl.SelectedIndex = selectedIndex = nItem;
                    return selectedIndex;
                }
            }

            return selectedIndex;
        }

        public static DropDownList SetStatusList(DropDownList ddlStatus, string currentStatus)
        {
            if (ddlStatus.Items.Count == 0)
                ddlStatus.DataSource = SQMSettings.Status;
            ddlStatus.DataTextField = "short_desc";
            ddlStatus.DataValueField = "code";
            ddlStatus.DataBind();
            if (!string.IsNullOrEmpty(currentStatus))
            {
                ddlStatus.SelectedValue = currentStatus;
            }

            return ddlStatus;
        }

        public void AlertUpdateResult(int status)
        {
            string cmd;

            if (status >= 0)
                cmd = "alertResult('hfAlertSaveSuccess');";
            else
                cmd = "alertResult('hfAlertSaveError');";

            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", cmd, true);
        }

        public static void AddToolTip(Control container, string id, string helpText)
        {
            var imgHelp = new Image()
            {
                ID = "help_" + id,
                ImageUrl = "/images/ico-question.png",
            };

            var rttHelp = new RadToolTip()
            {
                Text = "<div style=\"font-size: 11px; line-height: 1.5em;\">" + helpText + "</div>",
                TargetControlID = imgHelp.ID,
                IsClientID = false,
                RelativeTo = ToolTipRelativeDisplay.Element,
                Width = 320,
                Height = 160,
                Animation = ToolTipAnimation.Fade,
                Position = ToolTipPosition.MiddleRight,
                ContentScrolling = ToolTipScrolling.Auto,
                Skin = "Metro",
                AutoCloseDelay = 0
            };

            container.Controls.Add(new LiteralControl("<span style=\"float: right;\">"));
            container.Controls.Add(imgHelp);
            container.Controls.Add(new LiteralControl("</span>"));
            container.Controls.Add(rttHelp);
        }

        #endregion

		#region XLATS

		public static List<XLAT> TrimXLAT(List<XLAT> inputList)
		{
			List<XLAT> outputList = new List<XLAT>();

			foreach (XLAT xlat in inputList)
			{
				xlat.XLAT_CODE = !string.IsNullOrEmpty(xlat.XLAT_CODE) ? xlat.XLAT_CODE.Trim().Replace("\r\n", "") : "";
				xlat.DESCRIPTION = !string.IsNullOrEmpty(xlat.DESCRIPTION) ? xlat.DESCRIPTION.Trim().Replace("\r\n", "") : "";
				xlat.DESCRIPTION_SHORT = !string.IsNullOrEmpty(xlat.DESCRIPTION_SHORT) ? xlat.DESCRIPTION_SHORT.Trim().Replace("\r\n", "") : "";
				outputList.Add(xlat);
			}

			return outputList;
		}

		public static List<XLAT> SelectXLATList(string[] XLATGroupArray)
		{
			List<XLAT> XLATList = new List<XLAT>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				string language = "en";

				try
				{
					string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
					language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";
				}
				catch
				{ }

				if (language == "en")
				{
					XLATList = (from x in ctx.XLAT
								where x.XLAT_LANGUAGE == language && XLATGroupArray.Contains(x.XLAT_GROUP) && x.STATUS == "A"
								orderby x.XLAT_GROUP, x.XLAT_CODE
								select x).ToList();
				}
				else
				{
					List<XLAT> tempList = (from x in ctx.XLAT
								where (x.XLAT_LANGUAGE == language || x.XLAT_LANGUAGE == "en") && XLATGroupArray.Contains(x.XLAT_GROUP) && x.STATUS == "A"
								orderby x.XLAT_GROUP, x.XLAT_CODE
								select x).ToList();

					XLAT XLATlang = null;
					foreach (XLAT xlat in tempList.Where(x=> x.XLAT_LANGUAGE == "en").ToList())
					{
						XLATlang = tempList.Where(l => l.XLAT_GROUP == xlat.XLAT_GROUP && l.XLAT_CODE == xlat.XLAT_CODE && l.XLAT_LANGUAGE == language).FirstOrDefault();
						if (XLATlang != null)
							XLATList.Add(XLATlang);
						else
						{
							XLATlang = new XLAT();
							XLATlang = (XLAT)SQMModelMgr.CopyObjectValues(XLATlang, xlat, false);
							XLATlang.XLAT_LANGUAGE = language;  // substitute english xlat if localized version does not exist
							XLATList.Add(xlat);
						}
					}
				}
			}
			return TrimXLAT(XLATList);
		}

		public static List<XLAT> SelectXLATList(string[] XLATGroupArray, int languageID)
		{
			List<XLAT> XLATList = new List<XLAT>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				if (languageID == 1)
				{
					XLATList = (from x in ctx.XLAT
								join l in ctx.LOCAL_LANGUAGE on x.XLAT_LANGUAGE equals l.NLS_LANGUAGE
								where (l.LANGUAGE_ID == languageID) && XLATGroupArray.Contains(x.XLAT_GROUP) && x.STATUS == "A"
								orderby x.XLAT_GROUP, x.XLAT_CODE
								select x).ToList();
				}
				else if (languageID == 0)
				{
					XLATList = (from x in ctx.XLAT
								join l in ctx.LOCAL_LANGUAGE on x.XLAT_LANGUAGE equals l.NLS_LANGUAGE
								where XLATGroupArray.Contains(x.XLAT_GROUP) && x.STATUS == "A"
								orderby x.XLAT_GROUP, x.XLAT_CODE
								select x).ToList();
				}
				else
				{
					LOCAL_LANGUAGE lang = (from l in ctx.LOCAL_LANGUAGE where l.LANGUAGE_ID == languageID select l).SingleOrDefault();
					if (lang == null)
					{
						return XLATList;
					}
					List<XLAT> tempList = (from x in ctx.XLAT
										   join l in ctx.LOCAL_LANGUAGE on x.XLAT_LANGUAGE equals l.NLS_LANGUAGE
										   where (l.LANGUAGE_ID == languageID  ||  l.LANGUAGE_ID == 1) && XLATGroupArray.Contains(x.XLAT_GROUP) && x.STATUS == "A"
										   orderby x.XLAT_GROUP, x.XLAT_CODE
										   select x).ToList();
					XLAT XLATlang = null;
					foreach (XLAT xlat in tempList.Where(x => x.XLAT_LANGUAGE == "en").ToList())
					{
						XLATlang = tempList.Where(l => l.XLAT_GROUP == xlat.XLAT_GROUP && l.XLAT_CODE == xlat.XLAT_CODE && l.XLAT_LANGUAGE == lang.NLS_LANGUAGE).FirstOrDefault();
						if (XLATlang != null)
							XLATList.Add(XLATlang);
						else
						{
							XLATlang = new XLAT();
							XLATlang = (XLAT)SQMModelMgr.CopyObjectValues(XLATlang, xlat, false);
							XLATlang.XLAT_LANGUAGE = lang.NLS_LANGUAGE;  // substitute english xlat if localized version does not exist
							XLATList.Add(xlat);
						}
					}
				}
			}
			return TrimXLAT(XLATList);
		}

		public static XLAT GetXLAT(List<XLAT> xlatList, string xlatGroup, string xlatCode)
		{
			XLAT xlat = xlatList.Where(l => l.XLAT_GROUP == xlatGroup && l.XLAT_CODE == xlatCode).FirstOrDefault();
			if (xlat == null)
			{
				xlat = new XLAT();
			}
			return xlat;
		}

		public static XLAT GetXLAT(List<XLAT> xlatList, string xlatGroup, string xlatCode, string language)
		{
			XLAT xlat = xlatList.Where(l => l.XLAT_GROUP == xlatGroup && l.XLAT_CODE == xlatCode  &&  (string.IsNullOrEmpty(language)  ||  l.XLAT_LANGUAGE == language)).FirstOrDefault();

			if (xlat == null)
			{
				if ((xlat = xlatList.Where(l => l.XLAT_GROUP == xlatGroup && l.XLAT_CODE == xlatCode && (string.IsNullOrEmpty(language) || l.XLAT_LANGUAGE == "en")).FirstOrDefault()) == null)
				xlat = new XLAT();
			}
			return xlat;
		}


		#endregion
	}

}