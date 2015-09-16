using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Administrate_ViewCtlPlan : SQMBasePage 
    {
        static CTL_PLAN ctlPlan;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if ((bool)SessionManager.ReturnStatus)
                {
                    vw_CustPlantPart plantPart = (vw_CustPlantPart)SessionManager.ReturnObject;
                    SessionManager.Part = SQMModelMgr.LookupPart(plantPart.PART_ID, plantPart.CUST_COMPANY_ID);
                    SessionManager.ReturnObject = null;
                    SessionManager.ReturnStatus = false;
                }

               // SessionManager.Part = SQMModelMgr.LookupPart(6, 1); /////// mt - td
                if (SessionManager.Part != null)
                {
                    SetupPage();
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
 
            RegisterAppPage(WebSiteCommon.CleanPageName(Request.Path));

        }

        protected void lbPartAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Administrate_Part.aspx");
        }

        private void SetupPage()
        {
            PART part = (PART)SessionManager.Part;
            ctlPlan = CtlPlanMgr.LookupControlPlan(SessionManager.Part.PART_ID);
            if (ctlPlan != null)
            {
                // part summary area
                SetFindControlValue("lblCompany_out", hfBase, SessionManager.SessionContext.ActiveCompany().COMPANY_NAME);
                SetFindControlValue("lblPartNumFull_out", hfBase, SQMModelMgr.GetFullPartNumber(part));
                SetFindControlValue("lblPartName_out", hfBase, part.PART_NAME);

                SetFindControlValue("lblPlanName_out", hfBase, ctlPlan.CTLPLAN_NAME);
                SetFindControlValue("lblPlanVersion_out", hfBase, ctlPlan.VERSION);
                SetFindControlValue("lblPlanDesc_out", hfBase, ctlPlan.CTLPLAN_DESC);
                SetFindControlValue("lblPlanType_out", hfBase, WebSiteCommon.GetXlatValueLong("planType", ctlPlan.CTLPLAN_TYPE));
                SetFindControlValue("lblPlanRef_out", hfBase, ctlPlan.RTE_REF);
                SetFindControlValue("lblPlanResponsible_out", hfBase, ctlPlan.RESPONSIBILITY);
                SetFindControlValue("lblEffDate_out", hfBase, ctlPlan.EFF_DATE.ToString());

                DoCtlPlanSteps();
            }
        }

        protected void tab_Click(object sender, EventArgs e)
        {

        }

        protected void lbUploadData_Click(object sender, EventArgs e)
        {
            //SessionManager.BusinessOrg = null;
            Response.Redirect("/Admin/Administrate_FileUpload.aspx");
        }

        public void DoCtlPlanSteps()
        {

            GridView gv = (GridView)hfBase.FindControl("gvCtlPlan");
            gv.DataSource = ctlPlan.CTL_PLAN_STEP.OrderBy(p => p.STEP_SEQ);
            gv.DataBind();
            //SetGridViewDisplay(gv, partList.Count, "lblPartPlantListEmpty");
        }

        public void gvCtlPlan_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    /*
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
                    */
                    Label lbl;
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfStepInstructions");
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
                    CTL_PLAN_STEP step = ctlPlan.CTL_PLAN_STEP.First(s => s.CTLPLANSTEP_ID == Convert.ToInt32(lbl.Text));
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