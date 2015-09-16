using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public delegate void PartDataClick(PartData part);

    public partial class Ucl_PartList : System.Web.UI.UserControl
    {
        public event GridItemClick OnPartClick;
        public event PartDataClick OnPartDataClick;
        public event GridItemClick OnPartListCloseClick;

        static private bool isChanged;
        private List<PART_PROGRAM> staticProgramList
        {
            get { return ViewState["partProgramList"] == null ? new List<PART_PROGRAM>() : (List<PART_PROGRAM>)ViewState["partProgramList"]; }
            set { ViewState["partProgramList"] = value; }
        }
        private List<PartData> staticPartList
        {
            get { return ViewState["partDataList"] == null ? new List<PartData>() : (List<PartData>)ViewState["partDataList"]; }
            set { ViewState["partDataList"] = value; }
        }
        private List<PLANT> staticPlantList
        {
            get { return ViewState["partPlantList"] == null ? new List<PLANT>() : (List<PLANT>)ViewState["partPlantList"]; }
            set { ViewState["partPlantList"] = value; }
        }
 
        protected void wasChanged(object sender, EventArgs e)
        {
            isChanged = true;
        }

        protected void lnkPart_Click(object sender, EventArgs e)
        {
            if (OnPartClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnPartClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }


        #region program

        public void BindProgramList(List<PART_PROGRAM> programList)
        {
            ToggleVisible(pnlProgramList);
            staticProgramList = programList;
            gvProgramList.DataSource = programList.GroupBy(l => l.CUSTOMER_ID).Select(l => l.First());
            gvProgramList.DataBind();
        }

        public void gvProgramList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfCustomerID");
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblCustomer");
                    decimal customerID = Convert.ToDecimal(hf.Value);
                    COMPANY customer = SQMModelMgr.LookupCompany(customerID);
                    if (customer != null)
                    {
                        lbl.Text = customer.COMPANY_NAME;
                        GridView gv = (GridView)e.Row.Cells[0].FindControl("gvProgram");
                        gv.DataSource = staticProgramList.FindAll(l => l.CUSTOMER_ID == customerID);
                        gv.DataBind();
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

        #region programpart

        public void BindProgramPartList(List<PartData> partList)
        {
            if (staticPlantList == null  ||  staticPlantList.Count == 0)
                staticPlantList = new List<PLANT>(); 
            pnlProgramPartList.Visible = true;
            staticPartList = partList;
            rptProgramPart.DataSource = partList.GroupBy(l => new { l.Used.PLANT_ID, l.Used.SUPP_PLANT_ID, l.Used.CUST_PLANT_ID }).Select(l => l.First()).ToList();
            rptProgramPart.DataBind();
        }

        public void rptProgramPart_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    PartData partData = (PartData)e.Item.DataItem;
                    PLANT plant = null;
                    Label lbl1 = (Label)e.Item.FindControl("lblPartSource");
                    Label lbl2 = (Label)e.Item.FindControl("lblPartSourceCode");

                    if (partData.Used.SUPP_PLANT_ID.HasValue)
                    {
                        if ((plant = staticPlantList.Where(l => l.PLANT_ID == partData.Used.SUPP_PLANT_ID).SingleOrDefault()) == null)
                            staticPlantList.Add((plant = SQMModelMgr.LookupPlant((decimal)partData.Used.SUPP_PLANT_ID)));
                        lbl1.Text = plant.PLANT_NAME;
                        lbl2.Text = plant.ADDRESS.FirstOrDefault().CITY;
                    }
                    else
                    {
                        if ((plant = staticPlantList.Where(l => l.PLANT_ID == partData.Used.PLANT_ID).SingleOrDefault()) == null)
                            staticPlantList.Add((plant = SQMModelMgr.LookupPlant((decimal)partData.Used.PLANT_ID)));
                        lbl1.Text = plant.PLANT_NAME;
                        lbl2.Text = plant.DUNS_CODE;
                    }

                    lbl1 = (Label)e.Item.FindControl("lblPartUsed");
                    lbl2 = (Label)e.Item.FindControl("lblPartUsedCode");
                    if (partData.Used.CUST_PLANT_ID.HasValue)
                    {
                        if ((plant = staticPlantList.Where(l => l.PLANT_ID == partData.Used.CUST_PLANT_ID).SingleOrDefault()) == null)
                            staticPlantList.Add((plant = SQMModelMgr.LookupPlant((decimal)partData.Used.CUST_PLANT_ID)));
                        lbl1.Text = plant.PLANT_NAME;
                        lbl2.Text = plant.ADDRESS.FirstOrDefault().CITY;
                    }
                    else
                    {
                        if ((plant = staticPlantList.Where(l => l.PLANT_ID == partData.Used.PLANT_ID).SingleOrDefault()) == null)
                            staticPlantList.Add((plant = SQMModelMgr.LookupPlant((decimal)partData.Used.PLANT_ID)));
                        lbl1.Text = plant.PLANT_NAME;
                        lbl2.Text = plant.DUNS_CODE;
                    }
                 
                    GridView gv = (GridView)e.Item.FindControl("gvProgramPartList");
                    gv.DataSource = staticPartList.Where(l => l.Used.PLANT_ID == partData.Used.PLANT_ID && l.Used.SUPP_PLANT_ID == partData.Used.SUPP_PLANT_ID && l.Used.CUST_PLANT_ID == partData.Used.CUST_PLANT_ID).ToList();
                    gv.DataBind();
                }
                catch { }
            }
        }
        public void gvProgramPartList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            ;
        }

        #endregion

        #region part


        protected void lnkPartList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            PartData part = staticPartList.FirstOrDefault(l => l.ListSeq == Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            if (part != null)
            {
                if (OnPartClick != null)
                {
                    OnPartClick(part.Part.PART_ID);
                }
                if (OnPartDataClick != null)
                {
                    OnPartDataClick(part);
                }
            }
        }
        protected void btnPartListClose_Click(object sender, EventArgs e)
        {
            if (OnPartListCloseClick != null)
            {
                OnPartListCloseClick(0);
            }
        }

        public void BindPartList(List<PartData> theList)
        {
            if (theList != null)
            {
                ToggleVisible(pnlPartList);
                gvPartList.DataSource = theList;
                gvPartList.DataBind();
                SetGridViewDisplay(gvPartList, lblPartListEmpty, divPartListScroll, 50, 0);
            }
            else
            {
                ToggleVisible(null);
                gvPartList.DataSource = theList;
                gvPartList.DataBind();
            }
        }

        public void gvPartList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                
                try
                {
                    ;
                }
                catch
                {
                }
            }
        }
        #endregion

        #region partplant
        public void BindPartPlantList(List<PartData> theList)
        {
            staticPartList = theList;
            ToggleVisible(pnlPartPlantList);
            gvPartPlantList.DataSource = theList.GroupBy(l => l.Used.PLANT_ID).Select(l => l.First());
            gvPartPlantList.DataBind();
            SetGridViewDisplay(gvPartPlantList, lblPartPlantListEmpty, divPartPlantGVScroll, 20, 0);
        }

        public void gvPartPlant_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                Label lbl = (Label)e.Row.Cells[0].FindControl("lblCustPlantID");
                GridView gv = (GridView)e.Row.Cells[0].FindControl("gvPartSuppGrid");
                gv.DataSource = staticPartList.FindAll(l => l.Used.PLANT_ID == Convert.ToInt32(lbl.Text));
                gv.DataBind();
            }
        }
        #endregion


        #region common

        public void ToggleVisible(Panel pnlTarget)
        {
            pnlProgramList.Visible = pnlPartList.Visible = pnlPartPlantList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
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
                    if (lbl != null)
                    {
                        hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus");
                        lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                    }
                    CheckBox ckbox = (CheckBox)e.Row.Cells[0].FindControl("cbStatus");
                    if (ckbox != null)
                    {
                        hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus");
                        if (hfField.Value == "A")
                            ckbox.Checked = true;
                    }
                }
                catch
                {
                }
            }
        }

        public void SetGridViewDisplay(GridView gv, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount)
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
                int gridRows = gridRowCount;
                if (gridRows == 0)
                    gridRows = gv.Rows.Count;
                int rowLimit = rowsToScroll;
                if (rowLimit == 0)
                    rowLimit = 12; // dfltRowsToScroll;
                if (gridRows > rowLimit && divScroll != null)
                {
                    divScroll.Attributes["class"] = "scrollArea";
                }
            }
        }

        public void SetRepeaterDisplay(Repeater rpt, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount, string className)
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
                int gridRows = gridRowCount;
                if (gridRows == 0)
                    gridRows = rpt.Items.Count;
                int rowLimit = rowsToScroll;
                if (rowLimit == 0)
                    rowLimit = 12; // dfltRowsToScroll;
                if (gridRows > rowLimit && divScroll != null)
                {
                    divScroll.Attributes["class"] = className;
                }
            }
        }
        #endregion
    }
}