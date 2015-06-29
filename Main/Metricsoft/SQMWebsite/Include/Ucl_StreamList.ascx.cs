using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using System.Data;
using System.Web.UI.HtmlControls;

namespace SQM.Website
{

    public partial class Ucl_StreamList : System.Web.UI.UserControl
    {
        public event GridItemClick OnStreamCloseClick;
        public event GridItemClick OnStreamClick;
        public event GridItemClick OnReportClick;

        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            pnlSuppStreamList.Visible = pnlCustStreamList.Visible = pnlStreamRecList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
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

        protected void btnStreamListClose_Click(object sender, EventArgs e)
        {
            if (OnStreamCloseClick != null)
            {
                OnStreamCloseClick(0);
            }
        }
        protected void lnkStreamList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            if (OnStreamClick != null)
            {
                OnStreamClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }

        public void BindCRList(object costReportList)
        {
            pnlCRList.Visible = true;
            rptCRList.DataSource = costReportList;
            rptCRList.DataBind();
            SetRepeaterDisplay(rptCRList, lbCRListEmptyRepeater, divGVCRListRepeater, 5, rptCRList.Items.Count, "scrollAreaLarge");
        }
        public void rptCRList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    IncidentCostReport rpt = (IncidentCostReport)e.Item.DataItem;
                    LinkButton lnk = (LinkButton)e.Item.FindControl("lnkViewCRID");
                    lnk.Text = WebSiteCommon.FormatID(Convert.ToDecimal(lnk.CommandArgument), 6);
                    Label lbl = (Label)e.Item.FindControl("lblCRCreateDT");
                    lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(lbl.Text), "d", false);
                    lbl = (Label)e.Item.FindControl("lblCRType");
                    lbl.Text = WebSiteCommon.GetXlatValue("issueResponsible", lbl.Text);
                    Ucl_IncidentList uclIncidents = (Ucl_IncidentList)e.Item.FindControl("uclIncidents");
                    uclIncidents.BindQualityIssueList(rpt.IncidentList, false);
                }
                catch
                {
                }
            }
        }
        protected void lnkCRList_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            if (OnReportClick != null)
            {
                OnReportClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }

        public void BindSuppStreamList(object streamList)
        {
            rptSuppStreamList.DataSource = streamList;
            rptSuppStreamList.DataBind();
            ToggleVisible(pnlSuppStreamList);
            SetRepeaterDisplay(rptSuppStreamList, lblSuppStreamListEmpty, divSuppStreamGVScroll, 10, 0, "scrollAreaLarge");
        }

        public void BindCustStreamList(object streamList)
        {
            rptCustStreamList.DataSource = streamList;
            rptCustStreamList.DataBind();
            ToggleVisible(pnlCustStreamList);
            SetRepeaterDisplay(rptCustStreamList, lblCustStreamListEmpty, divCustStreamGVScroll, 10, 0, "scrollAreaLarge");
        }

        public void BindStreamRecList(object recList)
        {
            gvStreamRecList.DataSource = recList;
            gvStreamRecList.DataBind();
            ToggleVisible(pnlStreamRecList);
            if (gvStreamRecList.Rows.Count == 0)
                lblStreamRecListEmpty.Visible = true;
        }

        public void BindStreamRecHdr(StreamData stream)
        {
            pnlStreamHdr.Visible = true;
            lblSuppName_out.Text = stream.Supplier.COMPANY_NAME;
            lblSuppPlantName_out.Text = stream.SupplierPlant.PLANT_NAME;
            lblPartNum_out.Text = stream.Partdata.Part.PART_NUM;
        }

        public void rptSuppStreamList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    LinkButton lnk;
                    StreamData stream = (StreamData)e.Item.DataItem;
                    if (stream.Supplier != null)
                    {
                        lnk = (LinkButton)e.Item.FindControl("lnkSuppName");
                        lnk.Text = stream.Supplier.COMPANY_NAME;
                        if (stream.SupplierPlant != null)
                        {
                            lnk = (LinkButton)e.Item.FindControl("lnkSuppPlantName");
                            lnk.Text = stream.SupplierPlant.PLANT_NAME;
                        }
                    }
                    if (stream.Part != null)
                    {
                        lnk = (LinkButton)e.Item.FindControl("lnkPartNum");
                        lnk.Text = stream.Part.PART_NUM;
                    }
                }
                catch
                {
                }
            }
        }

        public void rptCustStreamList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    LinkButton lnk;
                    StreamData stream = (StreamData)e.Item.DataItem;
                    if (stream.Customer != null)
                    {
                        lnk = (LinkButton)e.Item.FindControl("lnkCustName");
                        lnk.Text = stream.Customer.COMPANY_NAME;
                        if (stream.CustomerPlant != null)
                        {
                            lnk = (LinkButton)e.Item.FindControl("lnkCustPlantName");
                            lnk.Text = stream.CustomerPlant.PLANT_NAME;
                        }
                    }
                    if (stream.Part != null)
                    {
                        lnk = (LinkButton)e.Item.FindControl("lnkPartNum");
                        lnk.Text = stream.Part.PART_NUM;
                    }
                }
                catch
                {
                }
            }
        }
    }
}