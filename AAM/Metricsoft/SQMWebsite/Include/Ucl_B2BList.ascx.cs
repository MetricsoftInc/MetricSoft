using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{

    public partial class Ucl_B2BList : System.Web.UI.UserControl
    {
        public event GridItemClick OnReceiptClick;

        private List<PERSON> staticPersonList
        {
            get { return ViewState["B2BPersonList"] == null ? new List<PERSON>() : (List<PERSON>)ViewState["B2BPersonList"]; }
            set { ViewState["B2BPersonList"] = value; }
        }

        public Repeater CustPartRepeater
        {
            get { return rptCustPartHeader; }
        }
        public Repeater SuppPartRepeater
        {
            get { return rptSuppPartHeader; }
        }

        #region customer
        public void BindCustPartList(List<PartData> b2bList, List<PERSON> notifyPersonList)
        {
            ToggleVisible(pnlCustPartList);
            staticPersonList = notifyPersonList;

            rptCustPartHeader.DataSource = b2bList;
            rptCustPartHeader.DataBind();
            if (b2bList.Count == 0)
            {
                lblCustListEmpty.Visible = true;
                rptCustPartHeader.Visible = false;
            }
        }

        public void rptCustPartHeader_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                PartData partData = (PartData)e.Item.DataItem;
                List<PartData> partList = new List<PartData>();
                partList.Add(partData);
                Label lbl = (Label)e.Item.FindControl("lblLocation");
                lbl.Text = partData.CustomerPlant.PLANT_NAME;

                Repeater rpt = (Repeater)e.Item.FindControl("rptCustPartDetail");
                rpt.DataSource = partList;
                rpt.DataBind();
                LinkButton lnk = (LinkButton)rpt.Items[0].FindControl("lnkPartList");
                lnk.CommandArgument = partData.CustomerPlant.PLANT_ID.ToString();
                lnk = (LinkButton)rpt.Items[0].FindControl("lnkSaveCust");
                lnk.CommandArgument = partData.CustomerPlant.PLANT_ID.ToString();

                lbl = (Label)rpt.Items[0].FindControl("lblLocationCode");
                lbl.Text = partData.CustomerPlant.DUNS_CODE;
                lbl = (Label)rpt.Items[0].FindControl("lblParentCompany");
                lbl.Text = partData.PartnerCompany.COMPANY_NAME;

                HiddenField hf = (HiddenField)rpt.Items[0].FindControl("hfQSNotify");
                RadComboBox ddl1 = (RadComboBox)rpt.Items[0].FindControl("ddlQSNotify1");
                SQMBasePage.SetPersonList(ddl1, staticPersonList, "", 20);
                RadComboBox ddl2 = (RadComboBox)rpt.Items[0].FindControl("ddlQSNotify2");
                SQMBasePage.SetPersonList(ddl2, staticPersonList, "", 20);
            }
        }

        protected void lnkDisplayCustParts(object sender, EventArgs e)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            CheckBox cb;
            LinkButton lnk = (LinkButton)sender;
            string cmdID = lnk.CommandArgument;

            foreach (RepeaterItem r in rptCustPartHeader.Items)
            {
                Repeater rd = (Repeater)r.FindControl("rptCustPartDetail");
                lnk = (LinkButton)rd.Items[0].FindControl("lnkPartList");
                if (lnk.CommandArgument == cmdID)
                {
                    Ucl_PartList uclPartList = (Ucl_PartList)rd.Items[0].FindControl("uclPartList");
                    cb = (CheckBox)rd.Items[0].FindControl("cbPartListSelect");
                    cb.Checked = cb.Checked == false ? true : false;
                    if (cb.Checked && !string.IsNullOrEmpty(lnk.CommandArgument))
                    {
                        uclPartList.BindPartList(SQMModelMgr.SelectTradingRelationshipList(new PSsqmEntities(), SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, plant.PLANT_ID, 0, 1, Convert.ToDecimal(lnk.CommandArgument)));
                    }
                    else
                    {
                        uclPartList.BindPartList(null);
                    }
                }
            }
        }

        protected void lnkSaveCust(object sender, EventArgs e)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LinkButton lnk = (LinkButton)sender;
            string cmdID = lnk.CommandArgument;
            PSsqmEntities ctx = new PSsqmEntities();

            foreach (RepeaterItem r in rptCustPartHeader.Items)
            {
                Repeater rd = (Repeater)r.FindControl("rptCustPartDetail");
                lnk = (LinkButton)rd.Items[0].FindControl("lnkSaveCust");
                if (lnk.CommandArgument == cmdID)
                {
                    RadComboBox ddl1 = (RadComboBox)rd.Items[0].FindControl("ddlQSNotify1");
                    RadComboBox ddl2 = (RadComboBox)rd.Items[0].FindControl("ddlQSNotify2");
                    HiddenField hf = (HiddenField)rd.Items[0].FindControl("hfQSNotify");
                }
            }
        }

        #endregion

        #region supplier
        public void BindSuppPartList(List<PartData> b2bList, List<PERSON> notifyPersonList)
        {
            ToggleVisible(pnlSuppPartList);
            staticPersonList = notifyPersonList;

            rptSuppPartHeader.DataSource = b2bList;
            rptSuppPartHeader.DataBind();
            if (b2bList.Count == 0)
            {
                lblSuppListEmpty.Visible = true;
                rptSuppPartHeader.Visible = false;
            }
        }

        public void rptSuppPartHeader_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                PartData partData = (PartData)e.Item.DataItem;
                try
                {
                    List<PartData> partList = new List<PartData>();
                    partList.Add(partData);
                    Label lbl = (Label)e.Item.FindControl("lblLocation");
                    lbl.Text = partData.SupplierPlant.PLANT_NAME;

                    Repeater rpt = (Repeater)e.Item.FindControl("rptSuppPartDetail");
                    rpt.DataSource = partList;
                    rpt.DataBind();
                    LinkButton lnk = (LinkButton)rpt.Items[0].FindControl("lnkPartList");
                    lnk.CommandArgument = partData.SupplierPlant.PLANT_ID.ToString();

                    lbl = (Label)rpt.Items[0].FindControl("lblLocationCode");
                    lbl.Text = partData.SupplierPlant.DUNS_CODE;
                    lbl = (Label)rpt.Items[0].FindControl("lblParentCompany");
                    lbl.Text = partData.PartnerCompany.COMPANY_NAME;

                    HiddenField hf = (HiddenField)rpt.Items[0].FindControl("hfQSNotify");
                    RadComboBox ddl1 = (RadComboBox)rpt.Items[0].FindControl("ddlQSNotify1");
                    SQMBasePage.SetPersonList(ddl1, staticPersonList, "", 20);
                    RadComboBox ddl2 = (RadComboBox)rpt.Items[0].FindControl("ddlQSNotify2");
                    SQMBasePage.SetPersonList(ddl2, staticPersonList, "", 20);
                }
                catch { }
            }
        }

        protected void lnkSaveSupp(object sender, EventArgs e)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            LinkButton lnk = (LinkButton)sender;
            string cmdID = lnk.CommandArgument;
            PSsqmEntities ctx = new PSsqmEntities();

            foreach (RepeaterItem r in rptCustPartHeader.Items)
            {
                Repeater rd = (Repeater)r.FindControl("rptSuppPartDetail");
                lnk = (LinkButton)rd.Items[0].FindControl("lnkSaveSupp");
                if (lnk.CommandArgument == cmdID)
                {
                    RadComboBox ddl1 = (RadComboBox)rd.Items[0].FindControl("ddlQSNotify1");
                    RadComboBox ddl2 = (RadComboBox)rd.Items[0].FindControl("ddlQSNotify2");
                    HiddenField hf = (HiddenField)rd.Items[0].FindControl("hfQSNotify");
                }
            }
        }

        protected void lnkDisplaySuppParts(object sender, EventArgs e)
        {
            PLANT plant = (PLANT)SessionManager.EffLocation.Plant;
            CheckBox cb;
            LinkButton lnk = (LinkButton)sender;
            string cmdID = lnk.CommandArgument;

            foreach (RepeaterItem r in rptSuppPartHeader.Items)
            {
                Repeater rd = (Repeater)r.FindControl("rptSuppPartDetail");
                lnk = (LinkButton)rd.Items[0].FindControl("lnkPartList");
                if (lnk.CommandArgument == cmdID)
                {
                    Ucl_PartList uclPartList = (Ucl_PartList)rd.Items[0].FindControl("uclPartList");
                    cb = (CheckBox)rd.Items[0].FindControl("cbPartListSelect");
                    cb.Checked = cb.Checked == false ? true : false;
                    if (cb.Checked && !string.IsNullOrEmpty(lnk.CommandArgument))
                    {
                        uclPartList.BindPartList(SQMModelMgr.SelectTradingRelationshipList(new PSsqmEntities(), SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, plant.PLANT_ID, 0, 2, Convert.ToDecimal(lnk.CommandArgument)));
                    }
                    else
                    {
                        uclPartList.BindPartList(null);
                    }
                }
            }
        }
        #endregion

        #region receipt

        public void BindReceiptList(List<ReceiptData> receiptList)
        {
            ToggleVisible(pnlReceiptList);

            rgReceiptList.DataSource = receiptList;
            rgReceiptList.DataBind();
        }

        protected void rgReceiptList_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                Label lbl;
                if (e.Item is GridDataItem)
                {
                    ReceiptData data = (ReceiptData)e.Item.DataItem;

                    LinkButton lnk = (LinkButton)e.Item.FindControl("lbIssueID");
                    if (data.IssueList != null && data.IssueList[0] != null)
                    {
                        lnk.Text = WebSiteCommon.FormatID(data.IssueList[0].INCIDENT_ID, 6);
                        lnk.CommandArgument = lnk.Text;
                    }
                    else
                    {
                        lnk.Visible = false;
                    }
                }
            }
        }

        protected void lnkReceipt_Click(object sender, EventArgs e)
        {
            if (OnReceiptClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnReceiptClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }

        protected void rgReceiptList_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            SessionManager.ReturnStatus = true;
            SessionManager.ReturnObject = "DisplayReceipts";
        }
        protected void rgReceiptList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            SessionManager.ReturnStatus = true;
            SessionManager.ReturnObject = "DisplayReceipts";
        }
        protected void rgReceiptList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            SessionManager.ReturnStatus = true;
            SessionManager.ReturnObject = "DisplayReceipts";
        }
        #endregion

        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            pnlCustPartList.Visible =  pnlSuppPartList.Visible = pnlReceiptList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        #endregion
    }
}