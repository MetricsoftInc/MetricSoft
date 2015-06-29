using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class  Administrate_ViewPart : SQMBasePage 
    {
        static bool editEnabled;
        static object buyerPartList;
        static object editObject;
        static PartData staticPartData;
        bool initSearch;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnCancelClick += OnCancelClick;
            uclSearchList.OnPartClick += OnPartDataClick;
        }

        protected void OnPartAdd(object sender, EventArgs e)
        {
            staticPartData = new PartData();
            staticPartData.IsNew = true;
            staticPartData.Part = SQMModelMgr.CreatePart(SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.UserContext.UserName());
           // ClearTempData();
            DisplayPart();
        }

        private void OnCancelClick()
        {
            //uclSearchBar.SetButtonsEnabled(true, true, true, false, false, false);
            string script = "function f(){ClosePartDetailWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);  
        }

        private void uclSearchBar_OnSearchClick()
        {
            initSearch = true;
            int allowAdd = 0;
            if (ddlSourceSelect.SelectedIndex > 0 && ddlUsedSelect.CheckedItems.Count > 0)
            {
                try
                {
                    int relationshipType = Convert.ToInt32(ddlSourceSelect.SelectedValue);
                    string usedSelect = ddlUsedSelect.SelectedValue; 

                    List<PartData> partList = SQMModelMgr.SelectPartDataList(entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, 0, 0, 0, relationshipType).OrderBy(p => p.Part.PART_NUM).ToList();

                    if (!string.IsNullOrEmpty(ddlSourceSelect.SelectedValue))
                    {
                        if (ddlSourceSelect.SelectedValue == "-1")  // all parts
                            ;
                        else if (ddlSourceSelect.SelectedValue == "0")  // produced at all internal locations
                        {
                            partList = partList.Where(l => l.Used != null && l.Used.SUPP_PLANT_ID.HasValue == false).ToList();
                        }
                        else  // produced at specific plant
                        {
                            ++allowAdd;
                            partList = partList.Where(l => l.Used != null && l.Used.PLANT_ID == Convert.ToDecimal(ddlSourceSelect.SelectedValue) || l.Used.SUPP_PLANT_ID == Convert.ToDecimal(ddlSourceSelect.SelectedValue)).ToList();
                        }
                    }

                    if (ddlUsedSelect.SelectedValue == "-1")      // all locations
                        ;
                    else if (ddlUsedSelect.SelectedValue == "0")  // all internal locations
                        partList = partList.Where(l => l.Used != null && l.Used.CUST_PLANT_ID.HasValue == false).ToList();
                    else if (ddlUsedSelect.SelectedValue == "-2")  // all customer locations
                        partList = partList.Where(l => l.Used != null && l.Used.CUST_PLANT_ID.HasValue == true).ToList();
                    else
                    {
                        decimal[] custSels = ddlUsedSelect.Items.Where(i => i.Checked == true).Select(i => Convert.ToDecimal(i.Value)).ToArray();
                        if (custSels.Length == 1)       // mt - debugging odd behavior on Varroc site
                        {
                            ++allowAdd;
                            decimal? id = custSels[0];
                            partList = partList.Where(l => l.Used != null && l.Used.CUST_PLANT_ID == id).ToList();
                        }
                        else
                        {
                             partList = partList.Where(l => l.Used != null && custSels.Contains((decimal)l.Used.CUST_PLANT_ID)).ToList();
                        }
                    }

                    lblPartCount.Text = partList.Count.ToString();
                    uclSearchList.BindProgramPartList(partList);
                    if (allowAdd == 2)
                        btnPartNew.Visible = true;
                    else
                        btnPartNew.Visible = false;
                }
                catch (Exception ex)
                {
                    //SQMLogger.LogException(ex);
                }
            }
		}

        protected void OnPartSearch(object sender, EventArgs e)
        {
            uclSearchBar_OnSearchClick();
        }

        protected void OnPartDataClick(decimal partID)
        {
            staticPartData = SQMModelMgr.LookupPartData(entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, partID);
            DisplayPart();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.PageTitle.Text = lblViewPartTitle.Text;
                uclSearchBar.SetButtonsVisible(false, false, false, false, false, false);
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
                btnPartNew.Visible = false;
                editEnabled = true;
                SetupPage();
                if (!initSearch)
                {
                    uclSearchBar_OnSearchClick();
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
                    ucl.BindDocumentSelect("SQM", 2, true, true, "");
                }

                RadComboBoxItem item;

                if (ddlSourceSelect.Items.Count == 5)
                {
                    List<BusinessLocation> supplierLocations = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true, true, false);
                    supplierLocations.AddRange(SQMModelMgr.SelectBusinessLocationList(0, 0, true, false, false));
                    foreach (BusinessLocation supp in supplierLocations)
                    {
                        item = new RadComboBoxItem(supp.Plant.PLANT_NAME, supp.Plant.PLANT_ID.ToString());
                        if (supp.Address != null)
                            item.ToolTip = supp.Address.STREET1 + " " + supp.Address.CITY;

                        ddlSourceSelect.Items.Add(item);
                    }
                }
                if (ddlUsedSelect.Items.Count == 5)
                {
                    List<BusinessLocation> customerLocations = new List<BusinessLocation>(); // SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true, true);
                    customerLocations.AddRange(SQMModelMgr.SelectBusinessLocationList(0, 0, false, true, false).OrderBy(l=> l.Plant.PLANT_NAME).ToList());
                    foreach (BusinessLocation cust in customerLocations.GroupBy(l => l.Plant.PLANT_ID).Select(l => l.FirstOrDefault()).ToList())
                    {
                        item = new RadComboBoxItem(cust.Plant.PLANT_NAME, cust.Plant.PLANT_ID.ToString());
                        if (cust.Address != null)
                            item.ToolTip = cust.Address.STREET1 + " " + cust.Address.CITY;
                   
                        ddlUsedSelect.Items.Add(item);
                    }
                }
            }
        }

        protected void lbCancelPart_Click(object sender, EventArgs e)
        {
            uclSearchBar.SetButtonsNotClicked();
           //string script = "function f(){ClosePartDetailWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);  
        }

        protected void lbSavePart_Click(object sender, EventArgs e)
        {
            uclSearchBar.SetButtonsNotClicked();
            SavePart();
            string script = "function f(){ClosePartDetailWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);  
        }

        private void SetupPage()
        {
            editObject = null;

            if (ddlPartProgram.Items.Count == 0)
            {
                ddlPartProgram.DataSource = SQMModelMgr.SelectPartProgramList(0, 0);
                ddlPartProgram.DataTextField = "PROGRAM_NAME";
                ddlPartProgram.DataValueField = "PROGRAM_ID";
                ddlPartProgram.DataBind();
                ddlPartProgram.Items.Insert(0, new ListItem("", "0"));
            }
        }

        #region part
        protected void DisplayPart()
        {
            tbPartNumber.Text = staticPartData.Part.PART_NUM;
            tbPartPrefix.Text = staticPartData.Part.PART_PREFIX;
            tbPartSuffix.Text = staticPartData.Part.PART_SUFFIX;
            tbPartSep.Text = staticPartData.Part.PART_NUM_SEPARATOR;
            tbPartName.Text = staticPartData.Part.PART_NAME;
            tbPartSerialNum.Text = staticPartData.Part.SERIAL_NUM;
            tbPartRevision.Text = staticPartData.Part.REVISION_LEVEL;
            lblPartLastUpdate.Text = staticPartData.Part.LAST_UPD_BY;
            lblPartLastUpdateDate.Text = WebSiteCommon.LocalTime((DateTime)staticPartData.Part.LAST_UPD_DT, SessionManager.UserContext.TimeZoneID).ToString();
            if (ddlPartProgram.Items.FindByValue(staticPartData.Part.PROGRAM_ID.ToString()) != null)
                ddlPartProgram.SelectedValue = staticPartData.Part.PROGRAM_ID.ToString();

            ddlPartStatus.DataSource = SQMSettings.Status;
            ddlPartStatus.DataTextField = "short_desc";
            ddlPartStatus.DataValueField = "code";
            ddlPartStatus.DataBind();
            if (ddlPartStatus.Items.FindByValue(staticPartData.Part.STATUS) != null)
                ddlPartStatus.SelectedValue = staticPartData.Part.STATUS;
          
            editObject = staticPartData;

            // enable part number field only when creating new or if NOT created by the upload process
            if (string.IsNullOrEmpty(staticPartData.Part.CREATE_BY) || staticPartData.Part.CREATE_BY.IndexOf(' ') > 0)
            {
                tbPartNumber.ReadOnly = false;
            }

            string script = "function f(){OpenPartDetailWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        protected void SavePart()
        {
            bool partAdded = false;

            if (string.IsNullOrEmpty(tbPartNumber.Text))
            {
                DisplayPart();
                DisplayErrorMessage(hfRequiredInputs);
                return;
            }

            if (!staticPartData.IsNew)
            {
                staticPartData = SQMModelMgr.LookupPartData(entities, (decimal)staticPartData.Part.COMPANY_ID, staticPartData.Part.PART_ID);
            }

            staticPartData.Part.PART_NUM = tbPartNumber.Text;
            staticPartData.Part.PART_PREFIX = tbPartPrefix.Text;
            staticPartData.Part.PART_SUFFIX = tbPartSuffix.Text;
            staticPartData.Part.PART_NUM_SEPARATOR = tbPartSep.Text;
            staticPartData.Part.PART_NAME = tbPartName.Text;
            staticPartData.Part.REVISION_LEVEL = tbPartRevision.Text;
            if (!string.IsNullOrEmpty(ddlPartProgram.SelectedValue))
                staticPartData.Part.PROGRAM_ID = Convert.ToDecimal(ddlPartProgram.SelectedValue);
            else
                staticPartData.Part.PROGRAM_ID = null;

            staticPartData.Part.SERIAL_NUM = tbPartSerialNum.Text;
            staticPartData.Part.STATUS = ddlPartStatus.SelectedValue;
            staticPartData.Part = SQMModelMgr.UpdatePart(entities, staticPartData.Part, SessionManager.UserContext.UserName());

            if (staticPartData.IsNew)
            {
                try
                {
                    PLANT suppPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(ddlSourceSelect.SelectedValue));
                    string custSel = ddlUsedSelect.CheckedItems.FirstOrDefault().Value;
                    PLANT custPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(custSel));
                    STREAM stream = SQMModelMgr.CreatePartStream(SessionManager.PrimaryCompany().COMPANY_ID, staticPartData.Part.PART_ID);
                    if (suppPlant.COMPANY_ID == SessionManager.PrimaryCompany().COMPANY_ID)
                    {
                        // primary company is supplier to customer
                        stream.COMPANY_ID = (decimal)suppPlant.COMPANY_ID;
                        stream.PLANT_ID = suppPlant.PLANT_ID;
                        stream.CUST_COMPANY_ID = custPlant.COMPANY_ID;
                        stream.CUST_PLANT_ID = custPlant.PLANT_ID;
                        stream.CUST_PART_NUM = staticPartData.Part.PART_NUM;
                        stream.SUPP_COMPANY_ID = stream.SUPP_PLANT_ID = null;
                        stream = SQMModelMgr.UpdatePartStream(entities, stream, SessionManager.UserContext.UserName());
                    }
                    else
                    {
                        // primary company is customer
                        stream.COMPANY_ID = (decimal)custPlant.COMPANY_ID;
                        stream.PLANT_ID = custPlant.PLANT_ID;
                        stream.SUPP_COMPANY_ID = suppPlant.COMPANY_ID;
                        stream.SUPP_PLANT_ID = suppPlant.PLANT_ID;
                        stream.SUPP_PART_NUM = staticPartData.Part.PART_NUM;
                        stream.CUST_COMPANY_ID = stream.CUST_PLANT_ID = null;
                    }
                    partAdded = true;
                }
                catch
                {
                    ;
                }
            }
 
            staticPartData.IsNew = false;
            editObject = null;

            if (partAdded)
                uclSearchBar_OnSearchClick();
        }

        private void DisplayErrorMessage(HiddenField hfMessage)
        {
            if (hfMessage == null)
                lblErrorMessage.Text = "";
            else
                lblErrorMessage.Text = hfMessage.Value;
        }

        #endregion
    }
}
