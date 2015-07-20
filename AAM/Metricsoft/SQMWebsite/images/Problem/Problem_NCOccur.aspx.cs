using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Problem_NCOccur : SQMBasePage
    {
        static bool loaded = false;
        static bool isEditMode;
        static QualityIssue qualityIssue;
        static List<PLANT> plantList;
        static List<PLANT_LINE> lineList;
        static List<vw_CustPlantPart> custList;
        static List<PR_QUALIFIER> qualifierList;
        static List<vw_CompanyNonConformance> NCList;
        static List<CTL_PLAN_MEASURE> measureList;
        static List<QI_OCCUR> issueList;
        bool initSearch;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnCancelClick += uclSearchBar_OnCancelClick;
            uclSearchBar.OnNewClick += uclSearchBar_OnNewClick;
            uclSearchBar.OnEditClick += uclSearchBar_OnEditClick;
            uclSearchBar.OnSaveClick += uclSearchBar_OnSaveClick;

            uclIssueList.OnQualityIssueListCloseClick += btnIssueListCancel_Click;
            uclIssueList.OnQualityIssueClick += OnIssue_Click;
        }
        private void uclSearchBar_OnCancelClick()
        {
            qualityIssue = null;
            uclSearchBar.SetButtonsEnabled(true, false, true, false, false, false);
            uclSearchBar.PageTitle.Text = lblNCOccurTitle.Text;
            ResetControlValues(divPageBody.Controls);
            divPageBody.Visible = false;
        }
        private void uclSearchBar_OnNewClick()
        {
            qualityIssue = new QualityIssue().CreateNew();
            qualityIssue.AddItem();
            qualityIssue.AddSample();
            qualityIssue.AddSampleMeasure(1);
            qualityIssue.AddSampleMeasure(1);
            pnlSearchList.Visible = false;
            uclSearchBar.SetButtonsEnabled(true, false, true, true, false, false);
            EnableControls(divPageBody.Controls, true);
            SetupPage();
        }
        private void uclSearchBar_OnEditClick()
        {
            uclSearchBar.SetButtonsEnabled(true, true, false, true, false, false);
            EnableControls(divPageBody.Controls, true);
        }
        private void uclSearchBar_OnSaveClick()
        {
            SaveIssue();
        }

        private void uclSearchBar_OnSearchClick()
        {
            initSearch = true;
            divPageBody.Visible = false;
            pnlSearchList.Visible = true;
            uclSearchBar.TitleItem.Visible = false;
            uclIssueList.BindQualityIssueList(QualityIssue.SelectIncidentList(SessionManager.SessionContext.ActiveCompany().COMPANY_ID, "QI", uclSearchBar.SearchText.Text, false));
            uclIssueList.QualityIssueListGrid.Columns[0].Visible = false;
        }

        protected void btnIssueListCancel_Click(decimal unused)
        {
            pnlSearchList.Visible = false;
            divPageBody.Visible = false;
            uclSearchBar.SetButtonsNotClicked();
        }

        protected void OnIssue_Click(decimal issueID)
        {
            qualityIssue = new QualityIssue().CreateNew().Load(issueID);
            pnlSearchList.Visible = false;
            uclSearchBar.SetButtonsEnabled(true, true, true, false, false, false);
            uclSearchBar.SetButtonsNotClicked();
            EnableControls(divPageBody.Controls, false);
            SetupPage();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (rblResponsible.SelectedIndex < 0)
                {
                    rblResponsible.SelectedIndex = 0;
                    rblSource.SelectedIndex = 0;
                    uclSearchBar.SetButtonsVisible(true, true, true, true, false, false);
                    uclSearchBar.SetButtonsEnabled(true, false, true, false, false, false);
                    uclSearchBar_OnSearchClick();
                }
                /*
                if (!initSearch)
                {
                    if (QualityIssue.IssueCount(SessionManager.SessionContext.ActiveCompany().COMPANY_ID, DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)), DateTime.Now) < 51)
                    {
                        uclSearchBar_OnSearchClick();
                    }
                }
                */
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
                hfld.Value = SessionManager.CurrentAdminTab = "lbNCOccur";
                uclSearchBar.PageTitle.Text = lblNCOccurTitle.Text;
                divPageBody.Visible = false;

                if ((bool)SessionManager.ReturnStatus)
                {
                    string s = SessionManager.ReturnObject.GetType().ToString();
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("QI_OCCUR"))
                    {
                        QI_OCCUR qiOccur = (QI_OCCUR)SessionManager.ReturnObject;
                        qualityIssue = new QualityIssue().Load(qiOccur.QIO_ID);
                        SetupPage();
                        uclSearchBar.SetButtonsEnabled(true, true, true, false, false, false);
                        uclSearchBar.SetButtonsNotClicked();
                    }
                }
            }
            else
            {
                if ((bool)SessionManager.ReturnStatus)
                {
                    string s = SessionManager.ReturnObject.GetType().ToString();
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("PLANTPART"))
                    {
                        vw_CustPlantPart plantPart = (vw_CustPlantPart)SessionManager.ReturnObject;
                        if (qualityIssue != null)
                        {
                            qualityIssue.AddPartInfo(plantPart.PART_ID, plantPart.PART_NUM, plantPart.PART_NAME);
                            qualityIssue.AddSupplierInfo(plantPart.SUPP_COMPANY_ID, plantPart.SUPP_COMPANY_NAME);
                        }
                        SetupPage();
                    }
                }
            }
            SessionManager.ReturnObject = null;
            SessionManager.ReturnStatus = false;
        }

        private void SetupPage()
        {
            COMPANY company = SQMModelMgr.LookupCompany(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, "", false);

            divPageBody.Visible = true;

            if (NCList == null || NCList.Count == 0)
            {
                NCList = SQMModelMgr.SelectCompanyNCList(company.COMPANY_ID, SessionManager.UserContext.BusinessOrg.BUS_ORG_ID, true);
                plantList = SQMModelMgr.SelectPlantList(entities, company.COMPANY_ID, SessionManager.UserContext.BusinessOrg.BUS_ORG_ID);
                lineList = new List<PLANT_LINE>();
                foreach (PLANT plant in plantList)
                {
                    plant.PLANT_LINE.Load();
                    foreach (PLANT_LINE line in plant.PLANT_LINE)
                    {
                        lineList.Add(line);
                    }
                }
                custList = SQMModelMgr.SelectCustomerPlantPartList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, 0, 0);
            }

            if (ddlQualityIssueType.Items.Count == 0)
            {
                ddlDisposition.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("NCDisposition"));

                ddlQualityIssueType.DataSource = NCList.GroupBy(l => l.ProblemTypeID).Select(l => l.First());
                ddlQualityIssueType.DataTextField = "ProblemType";
                ddlQualityIssueType.DataValueField = "ProblemTypeID";
                ddlQualityIssueType.DataBind();
            }

            ddlPlant.DataSource = plantList;
            ddlPlant.DataTextField = "PLANT_NAME";
            ddlPlant.DataValueField = "PLANT_ID";
            ddlPlant.DataBind();
            ddlPlant.Items.Insert(0, new ListItem("", "0"));

            ddlPlantLine.DataSource = lineList;
            ddlPlantLine.DataTextField = "PLANT_LINE_NAME";
            ddlPlantLine.DataValueField = "PLANT_LINE_ID";
            ddlPlantLine.DataBind();
            ddlPlantLine.Items.Insert(0, new ListItem("", "0"));

            ddlCustomer.DataSource = custList.GroupBy(l => l.CUST_COMPANY_ID).Select(l => l.First());
            ddlCustomer.DataTextField = "CUST_COMPANY_NAME";
            ddlCustomer.DataValueField = "CUST_COMPANY_ID";
            ddlCustomer.DataBind();
            ddlCustomer.Items.Insert(0, new ListItem("", "0"));

            ddlCustomerPlant.DataSource = custList.GroupBy(l => l.CUST_COMPANY_ID).Select(l => l.First());
            ddlCustomerPlant.DataTextField = "CUST_PLANT_NAME";
            ddlCustomerPlant.DataValueField = "CUSTOMER_PLANT_ID";
            ddlCustomerPlant.DataBind();
            ddlCustomerPlant.Items.Insert(0, new ListItem("", "0"));

            if (qualityIssue.IsNew)
            {
                uclSearchBar.TitleItem.Visible = false;
                lblIssueDate_out.Text = WebSiteCommon.LocalTime(DateTime.Now, SessionManager.UserContext.TimeZoneID).ToString();
                lblOriginator_out.Text = (SessionManager.UserContext.UserName() + "  (" + SessionManager.UserContext.BusinessOrg.ORG_NAME + ")");
            }

            //if (qualityIssue.Part != null && qualityIssue.Part.PART_ID > 0)
            // uclSearchBar.SetButtonsEnabled(true, true, true, false, false, false);

            DisplayIssue();

            isEditMode = true;
        }

        protected void DisplayIssue()
        {
            tbPartNumber.Text = qualityIssue.Part.PART_NUM;
            tbSupplier.Text = qualityIssue.Supplier.COMPANY_NAME;

            gvQISamples.DataSource = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE;
            gvQISamples.DataBind();

            if (!qualityIssue.IsNew)
            {
               // PROB_DEFINE probDefine = new PROB_DEFINE();
               // probDefine = qualityIssue.UpdateProblemDefinition(probDefine);

                lblIssueDate_out.Text = WebSiteCommon.LocalTime((DateTime)qualityIssue.Incident.CREATE_DT, SessionManager.UserContext.TimeZoneID).ToString();
                uclSearchBar.TitleItem.Text = qualityIssue.IssueID;
                uclSearchBar.TitleItem.Visible = lblIssueDate_out.Visible = true;

                tbDateDetected.Text = WebSiteCommon.FormatDateString(WebSiteCommon.LocalTime(qualityIssue.Incident.INCIDENT_DT, SessionManager.UserContext.TimeZoneID), false);
                tbIssueDesc.Text = qualityIssue.Incident.DESCRIPTION;

                SetListSelectedTextValue(ddlQualityIssueType, qualityIssue.IssueOccur.PROBLEM_TYPE);
                if (!string.IsNullOrEmpty(qualityIssue.IssueOccur.SOURCE))
                    rblSource.SelectedValue = qualityIssue.IssueOccur.SOURCE;

                BUSINESS_ORG orginBusOrg = SQMModelMgr.LookupBusOrg((decimal)qualityIssue.Incident.BUS_ORG_ID);
                if (orginBusOrg == null)
                {
                    lblOriginator_out.Text = qualityIssue.Incident.CREATE_BY;
                }
                else
                {
                    lblOriginator_out.Text = qualityIssue.Incident.CREATE_BY + "  (" + orginBusOrg.ORG_NAME + ")";
                }

                ddlPlant.SelectedValue = qualityIssue.Incident.PLANT_ID.ToString();
                ddlPlantLine.SelectedValue = qualityIssue.Incident.PLANT_ID.ToString();
                if (qualityIssue.Incident.PLANT_LINE_ID > -1)
                    ddlPlantLine.SelectedValue = qualityIssue.Incident.PLANT_LINE_ID.ToString();

                if (qualityIssue.IssueOccur.CUSTOMER_ID > 0)
                    ddlCustomer.SelectedValue = qualityIssue.IssueOccur.CUSTOMER_ID.ToString();
                if (qualityIssue.IssueOccur.CUSTOMER_PLANT_ID > 0)
                    ddlCustomerPlant.SelectedValue = qualityIssue.IssueOccur.CUSTOMER_PLANT_ID.ToString();

                tbNCLotNum.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().LOT_NUM;
                tbNCContainer.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().CONTAINER_NUM;
                tbNCTotalQty.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY.ToString();
                tbNCSampleQty.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY.ToString();
                tbNCNonConformQty.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY.ToString();
                tbTotalEstNCQty.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_NC_QTY.ToString();

                int nRow = -1;
                foreach (QI_OCCUR_SAMPLE sample in qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE)
                {
                    GridViewRow row = gvQISamples.Rows[++nRow];
                    DropDownList ddl = (DropDownList)row.FindControl("ddlPrimaryNC");
                    SetListSelectedTextValue(ddl, qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_PRIMARY);
                    ddl = (DropDownList)row.FindControl("ddlSecondaryNC");
                    SetListSelectedTextValue(ddl, qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_SECONDARY);
                    TextBox tb = (TextBox)row.FindControl("tbNCCount");
                    tb.Text = Math.Max(1,Convert.ToDecimal(qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_COUNT)).ToString();
                }

                tbObservations.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().SAMPLE_COMMENTS;
                ddlDisposition.SelectedValue = qualityIssue.IssueOccur.DISPOSITION;
                if (!string.IsNullOrEmpty(qualityIssue.IssueOccur.RESPONSIBLE))
                    rblResponsible.SelectedValue = qualityIssue.IssueOccur.RESPONSIBLE;
                cbActionRequired.Checked = (bool)qualityIssue.IssueOccur.ACTION_REQD;
                tbComments.Text = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().ITEM_COMMENTS;

                btnPrintLabel.OnClientClick = "Popup('../Problem/QualityIssue_Label.aspx?issue=" + qualityIssue.IssueOccur.QIO_ID.ToString() + "', 'newPage', 600, 450); return false;";
            }
        }

        public void gvQISamples_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfPrimaryNC");
                    DropDownList ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlPrimaryNC");
                    ddl.DataSource = NCList.Where(l => l.ProblemTypeID == 40).GroupBy(l => l.PrimaryNCID).Select(l => l.FirstOrDefault());
                    ddl.DataTextField = "PrimaryNC";
                    ddl.DataValueField = "PrimaryNCID";
                    ddl.DataBind();

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfSecondaryNC");
                    ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlSecondaryNC");
                    ddl.DataSource = NCList.Where(l => l.ProblemTypeID == 40 && l.PrimaryNCID == 1901); // .GroupBy(l => l.PrimaryNCID).Select(l => l.FirstOrDefault());
                    ddl.DataTextField = "SecondaryNC";
                    ddl.DataValueField = "SecondaryNCID";
                    ddl.DataBind();

                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMeasureGrid");
                    QI_OCCUR_SAMPLE sample = qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First();
                    gv.DataSource = sample.QI_OCCUR_MEASURE;
                    gv.DataBind();
                }
                catch
                {
                }
            }
        }

        public void gvMeasure_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    HiddenField hf;

                    if (measureList == null || measureList.Count == 0)
                    {
                        measureList = QualityIssue.SelectMeasuresList(0);
                    }
                    DropDownList ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlMeasure");
                    ddl.DataSource = measureList;
                    ddl.DataTextField = "MEASURE_NAME";
                    ddl.DataValueField = "CTLMEASURE_ID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("", "0"));
                }
                catch
                {
                }
            }
        }

        protected void tab_Click(object sender, EventArgs e)
        {
        }


        protected void SaveIssue()
        {
            bool success;
            decimal decVal;

            // update the INCIDENT record
            qualityIssue.Incident.INCIDENT_DT = WebSiteCommon.ConvertDateFromString(tbDateDetected.Text, Convert.ToDateTime(WebSiteCommon.GetXlatValue("effDates", "MIN")));
            qualityIssue.Incident.DESCRIPTION = tbIssueDesc.Text.Trim();
            qualityIssue.Incident.COMPANY_ID = SessionManager.SessionContext.ActiveCompany().COMPANY_ID;
            qualityIssue.Incident.BUS_ORG_ID = SessionManager.UserContext.BusinessOrg.BUS_ORG_ID;
            qualityIssue.Incident.PLANT_ID = Convert.ToDecimal(ddlPlant.SelectedValue);
            if (ddlPlantLine.SelectedIndex > -1)
                qualityIssue.Incident.PLANT_LINE_ID = Convert.ToDecimal(ddlPlantLine.SelectedValue);

            // QUALITY ISSUE records
            qualityIssue.IssueOccur.PROBLEM_TYPE = ddlQualityIssueType.SelectedItem.Text;
            qualityIssue.IssueOccur.SOURCE = rblSource.SelectedValue;
            if (ddlCustomer.SelectedIndex > -1)
                qualityIssue.IssueOccur.CUSTOMER_ID = Convert.ToDecimal(ddlCustomer.SelectedValue);
            if (ddlCustomerPlant.SelectedIndex > -1)
                qualityIssue.IssueOccur.CUSTOMER_PLANT_ID = Convert.ToDecimal(ddlCustomerPlant.SelectedValue);

            qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().LOT_NUM = tbNCLotNum.Text;
            qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().CONTAINER_NUM = tbNCContainer.Text;
            if (decimal.TryParse(tbNCTotalQty.Text.Trim(), out decVal))
                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY = decVal;
            if (decimal.TryParse(tbNCSampleQty.Text.Trim(), out decVal))
                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY = decVal;
            if (decimal.TryParse(tbNCNonConformQty.Text.Trim(), out decVal))
                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY = decVal;
            qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_NC_QTY = (qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY / qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY) * qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY;

            int nrow = -1;
            DropDownList ddl;
            HiddenField hf;
            foreach (GridViewRow row in gvQISamples.Rows)
            {
                ++nrow;
                ddl = (DropDownList)row.FindControl("ddlPrimaryNC");
                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_PRIMARY = ddl.SelectedItem.Text;
                ddl = (DropDownList)row.FindControl("ddlSecondaryNC");
                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_SECONDARY = ddl.SelectedItem.Text;
                TextBox tb = (TextBox)row.FindControl("tbNCCount");
                int count = 1;
                if (int.TryParse(tb.Text.Trim(), out count))
                    qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().PROBLEM_COUNT = count;
                hf = (HiddenField)row.FindControl("hfAttachmentID");
                if (!String.IsNullOrEmpty(hf.Value))
                    qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().ATTACHMENT_ID = Convert.ToInt32(hf.Value);
                else
                    qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().ATTACHMENT_ID = 0;
            }
            qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_SAMPLE.First().SAMPLE_COMMENTS = tbObservations.Text.Trim();

            qualityIssue.IssueOccur.DISPOSITION = ddlDisposition.SelectedValue;
            qualityIssue.IssueOccur.RESPONSIBLE = rblResponsible.SelectedValue;
            qualityIssue.IssueOccur.ACTION_REQD = cbActionRequired.Checked;
            qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().ITEM_COMMENTS = tbComments.Text.Trim();

            if (QualityIssue.AddIssue(qualityIssue) != null)
            {
                btnPrintLabel.OnClientClick = "Popup('../Problem/QualityIssue_Label.aspx?issue=" + qualityIssue.IssueOccur.QIO_ID.ToString() + "', 'newPage', 600, 450); return false;";
                DisplayIssue();
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
            }
        }
    }
}