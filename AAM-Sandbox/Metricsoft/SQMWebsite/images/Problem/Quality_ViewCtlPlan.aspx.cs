using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Quality_ViewCtlPlan : SQMBasePage 
    {
        static CtlPlanMgr ctlPlanMgr;
        static List<PART> planList;
        bool initSearch;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnCancelClick += uclSearchBar_OnCancelClick;
            uclSearchBar.OnNewClick += uclSearchBar_OnNewClick;
            uclSearchBar.OnSaveClick += uclSearchBar_OnSaveClick;
            uclSearchBar.OnEditClick += uclSearchBar_OnEditClick;
            uclSearchBar.OnUploadClick += uclSearchBar_OnUploadClick;
            uclAdminList.OnPartClick += uclAdminList_OnPartClick;
            uclAdminList.OnPartListCloseClick += uclAdminList_OnPartListCloseClick;
        }
        private void uclSearchBar_OnCancelClick()
        {
            SessionManager.Part = null;
            divPageBody.Visible = false;
            uclSearchBar.SetButtonsEnabled(true, false, true, false, false, false);
            EnableControls(divPageBody.Controls, false);
        }
        private void uclSearchBar_OnNewClick()
        {
            uclSearchBar.SetButtonsEnabled(true, false, true, true, false, false);
            EnableControls(divPageBody.Controls, true);
        }
        private void uclSearchBar_OnEditClick()
        {
            uclSearchBar.SetButtonsEnabled(true, true, false, true, false, false);
            EnableControls(divPageBody.Controls, true);
            tab_Click(uclSearchBar.EditButton, null);
        }
        private void uclSearchBar_OnSaveClick()
        {
            
        }
        private void uclSearchBar_OnSearchClick()
        {
            initSearch = true;
            divPageBody.Visible = false;
            pnlSearchList.Visible = uclAdminList.PartListPanel.Visible = true;
            if (uclSearchBar.SearchCriteriaChanged() || planList == null)
                planList = SQMModelMgr.SearchPartList(entities, SessionManager.SessionContext.ActiveCompany().COMPANY_ID, uclSearchBar.SearchText.Text, true);

            uclAdminList.BindPartList(planList);
            uclAdminList.PartListGrid.Columns[2].Visible = false;
            SetGridViewDisplay(uclAdminList.PartListGrid, uclAdminList.PartListLabel, uclAdminList.PartListDiv, 20);
        }

        private void uclSearchBar_OnUploadClick()
        {
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }

        protected void uclAdminList_OnPartListCloseClick(decimal unused)
        {
            pnlSearchList.Visible = uclAdminList.PartListPanel.Visible = false;
            divPageBody.Visible = false;
            uclSearchBar.SetButtonsNotClicked();
        }
        protected void uclAdminList_OnPartClick(decimal partID)
        {
            SessionManager.Part = SQMModelMgr.LookupPart(entities, partID, "", SessionManager.SessionContext.ActiveCompany().COMPANY_ID, false);
            ctlPlanMgr = new CtlPlanMgr();
            pnlSearchList.Visible = uclAdminList.PartListPanel.Visible = false;
            uclSearchBar.SetButtonsEnabled(true, true, true, false, false, false);
            uclSearchBar.SetButtonsNotClicked();
            EnableControls(divPageBody.Controls, false);
            SetupPage();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ClearTempData();
                HiddenField hfld = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("ContentPlaceHolder1").FindControl("hdCurrentActiveTab");
                hfld.Value = SessionManager.CurrentAdminTab = "lbCtlPlan";
                uclSearchBar.PageTitle.Text = lblCtlPlanTitle.Text;
                divPageBody.Visible = pnlSearchList.Visible = false;
                uclSearchBar.SetButtonsVisible(true, true, true, true, false, false);
                uclSearchBar.SetButtonsEnabled(true, false, false, false, false, false);
                if (!initSearch)
                {
                    if (SQMModelMgr.PartCount(SessionManager.SessionContext.ActiveCompany().COMPANY_ID) < 51)
                    {
                        uclSearchBar_OnSearchClick();
                    }
                }
            }
        }

        private void ClearTempData()
        {
            if (planList != null)
            {
                planList.Clear();
                planList = null;
            }
        }

        protected void lbPartAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_Part.aspx");
        }

        private void SetupPage()
        {
            PART part = (PART)SessionManager.Part;
            ctlPlanMgr.Load(SessionManager.Part.PART_ID);
            divPageBody.Visible = true;

            if (ctlPlanMgr.CtlPlan != null)
            {
                uclItemHdr.DisplayCtlPlan(part, ctlPlanMgr.CtlPlan);

                DoCtlPlanSteps();
            }
        }

        protected void tab_Click(object sender, EventArgs e)
        {
            // placeholder method
        }

        public void gvList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                }
                catch
                {
                }
            }
        }

        public void DoCtlPlanSteps()
        {

            GridView gv = (GridView)hfBase.FindControl("gvCtlPlan");
            gv.DataSource = ctlPlanMgr.CtlPlan.CTL_PLAN_STEP.OrderBy(p => p.STEP_SEQ);
            gv.DataBind();
            //SetGridViewDisplay(gv, partList.Count, "lblPartPlantListEmpty");
        }

        public void gvCtlPlan_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                   
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfSampleRate");
                    DropDownList ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlSampleRate");
                    ddl.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("sampleRate"));
                    if (!String.IsNullOrEmpty(hf.Value))
                        ddl.SelectedValue = hf.Value;
                   
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfSampleUnit");
                    ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlSampleUnit");
                    ddl.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("sampleUnit"));
                    if (!String.IsNullOrEmpty(hf.Value))
                        ddl.SelectedValue = hf.Value;
                  
                    Label lbl;
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfStepInstructions");
                    Image img = (Image)e.Row.Cells[0].FindControl("imgStepType");
                    img.ToolTip = hf.Value;

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfPlanStepLocation");
                    if (!string.IsNullOrEmpty(hf.Value))
                    {
                        decimal plantID = Convert.ToDecimal(hf.Value);
                        PLANT plant = SQMModelMgr.LookupPlant(entities, plantID, "");
                        if (plant != null)
                        {
                             img = (Image)e.Row.Cells[0].FindControl("imgPlanStepLocation");
                             if (plant.COMPANY_ID == SessionManager.SessionContext.ActiveCompany().COMPANY_ID)
                             {
                                 img.ImageUrl = "~/images/icon_customer2.gif";
                                 img.ToolTip = "";   // mt - todo: remove constant
                             }
                             else
                             {
                                 img.ImageUrl = "~/images/icon_supplier2.gif";
                                 img.ToolTip = "Supplier";   // mt - todo: remove constant
                             }
                        }
                    }

                    lbl = (Label)e.Row.Cells[0].FindControl("lblSampleUnit_out");
                    lbl.Text = WebSiteCommon.GetXlatValueLong("sampleUnit", lbl.Text);
                    lbl = (Label)e.Row.Cells[0].FindControl("lblSampleRate_out");
                    lbl.Text = WebSiteCommon.GetXlatValueLong("sampleRate", lbl.Text);

                    lbl = (Label)e.Row.Cells[0].FindControl("lblPlanStepID");
                    GridView gv = (GridView)e.Row.Cells[0].FindControl("gvMeasureGrid");
                    CTL_PLAN_STEP step = ctlPlanMgr.CtlPlan.CTL_PLAN_STEP.First(s => s.CTLPLANSTEP_ID == Convert.ToInt32(lbl.Text));
                    gv.DataSource = step.CTL_PLAN_MEASURE;
                    gv.DataBind();
                    gv = (GridView)e.Row.Cells[0].FindControl("gvMethodGrid");
                    gv.DataSource = step.CTL_PLAN_MEASURE;
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
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureType");
                    DropDownList ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlMeasureType");
                    ddl.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("measureType"));
                    if (!String.IsNullOrEmpty(hf.Value))
                        ddl.SelectedValue = hf.Value;

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureClass");
                    if (hf.Value == "SPC")
                    {
                        Image img = (Image)e.Row.Cells[0].FindControl("imgMeasureClass");
                        img.ImageUrl = "~/images/class_01.gif";
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfMeasureInstructions");
                        img.ToolTip = hf.Value;
                    }
                }
                catch
                {
                }
            }
        }

        public void gvMethod_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfSpecType");
                    DropDownList ddl = (DropDownList)e.Row.Cells[0].FindControl("ddlSpecType");
                    ddl.Items.AddRange(WebSiteCommon.PopulateDropDownListItems("specType"));
                    if (!String.IsNullOrEmpty(hf.Value))
                    {
                        ddl.SelectedValue = hf.Value;
                    }
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbSpecValues");
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfSpecLSL");
                    tb.Text = hf.Value;
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfSpecUSL");
                    tb.Text += (" / " + hf.Value);
                }
                catch
                {
                }
            }
        }
    }
}