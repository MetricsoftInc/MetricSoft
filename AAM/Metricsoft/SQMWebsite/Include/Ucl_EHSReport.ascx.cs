using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using Telerik.Web.UI;


namespace SQM.Website
{
    public partial class Ucl_EHSReport : System.Web.UI.UserControl
    {
        // manage current session object  (formerly was page static variable)
        EHSModel.GHGResultList LocalGHGResultList()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is EHSModel.GHGResultList)
                return (EHSModel.GHGResultList)SessionManager.CurrentObject;
            else
                return null;
        }
        EHSModel.GHGResultList SetLocalGHGResultList(EHSModel.GHGResultList GHGResultList)
        {
            SessionManager.CurrentObject = GHGResultList;
            return LocalGHGResultList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ;
        }

        public int BindGHGReport(EHSModel.GHGResultList GHGTable)
        {
            pnlCO2Report.Visible = true;
            SetLocalGHGResultList(GHGTable);
            rptCO2Report.DataSource = GHGTable.ResultList.Select(l => l.Plant).Distinct().ToList();
            rptCO2Report.DataBind();

            return 0;
        }

        public void rptCO2Report_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    PLANT plant = (PLANT)e.Item.DataItem;

                    Label lbl = (Label)e.Item.FindControl("lblLocation");
                    lbl.Text = plant.PLANT_NAME;

                    Repeater rpt = (Repeater)e.Item.FindControl("rptScope1Fuel");
                    List<EHSModel.GHGResult> fuelList = LocalGHGResultList().ResultList.Where(l => l.Plant.PLANT_ID == plant.PLANT_ID).ToList();
                    rpt.DataSource = fuelList.Where(l => l.EFMType != "P" && l.EFMType != "STEAM" &&  l.GasSeq == 1).Distinct().ToList();
                    rpt.DataBind();

                    rpt = (Repeater)e.Item.FindControl("rptScope2Fuel");
                    rpt.DataSource = fuelList.Where(l => l.EFMType == "P" || l.EFMType == "STEAM" && l.GasSeq == 1).Distinct().ToList();
                    rpt.DataBind();
                }
                catch
                {
                    ;
                }
            }
        }

        public void rptScope1Fuel_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHSModel.GHGResult ghgRrec = (EHSModel.GHGResult)e.Item.DataItem;

                    Label lbl = (Label)e.Item.FindControl("lblScope1Fuel");
                    lbl.Text = SessionManager.EFMList.Where(l => l.EFM_TYPE == ghgRrec.EFMType).Select(l => l.DESCRIPTION).FirstOrDefault();
                    lbl = (Label)e.Item.FindControl("lblScope1FuelQtyHdr");
                    lbl.Text = lbl.Text + "<i>(" + SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == ghgRrec.MetricUOM).UOM_CD + ")</i>";

                    decimal efmQty = LocalGHGResultList().ResultList.Where(l => l.Plant.PLANT_ID == ghgRrec.Plant.PLANT_ID && l.EFMType == ghgRrec.EFMType).Select(l => l.MetricValue).FirstOrDefault();
                    lbl = (Label)e.Item.FindControl("lblScope1FuelQty");
                    lbl.Text = SQMBasePage.FormatValue(efmQty, 2);

                    GridView gv = (GridView)e.Item.FindControl("gvGasList1");
                    gv.Columns[3].HeaderText = gv.Columns[3].HeaderText + "<i>(" + SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == ghgRrec.GHGUOM).UOM_CD + ")</i>";
                    List<EHSModel.GHGResult> gasList = LocalGHGResultList().ResultList.Where(l => l.Plant.PLANT_ID == ghgRrec.Plant.PLANT_ID && l.EFMType == ghgRrec.EFMType).ToList();
                    gv.DataSource = gasList.Distinct().ToList();
                    gv.DataBind();
                }
                catch
                {
                    ;
                }
            }
        }

        public void gvGasList1_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblCO2Factor");
                    if (lbl.Text.Contains("0.000"))
                        lbl.Text = "";
                    else
                        lbl.Text = lbl.Text.Substring(0, lbl.Text.IndexOf('.') + 5);

                    lbl = (Label)e.Row.Cells[0].FindControl("lblEmitValue");
                    lbl.Text = lbl.Text.Substring(0, lbl.Text.IndexOf('.') + 3);
                }
                catch
                {
                    ; 
                }
            }
        }

        public void rptScope2Fuel_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHSModel.GHGResult ghgRrec = (EHSModel.GHGResult)e.Item.DataItem;

                    Label lbl = (Label)e.Item.FindControl("lblScope2Fuel");
                    lbl.Text = SessionManager.EFMList.Where(l => l.EFM_TYPE == ghgRrec.EFMType).Select(l => l.DESCRIPTION).FirstOrDefault();
                    lbl = (Label)e.Item.FindControl("lblScope2FuelQtyHdr");
                    lbl.Text = lbl.Text + "<i>(" + SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == ghgRrec.MetricUOM).UOM_CD + ")</i>";

                    decimal efmQty = LocalGHGResultList().ResultList.Where(l => l.Plant.PLANT_ID == ghgRrec.Plant.PLANT_ID && l.EFMType == ghgRrec.EFMType).Select(l => l.MetricValue).FirstOrDefault();
                    lbl = (Label)e.Item.FindControl("lblScope2FuelQty");
                    lbl.Text = SQMBasePage.FormatValue(efmQty, 2);
                    GridView gv = (GridView)e.Item.FindControl("gvGasList2");
                    gv.Columns[3].HeaderText = gv.Columns[3].HeaderText + "<i>(" + SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == ghgRrec.GHGUOM).UOM_CD + ")</i>";
                    List<EHSModel.GHGResult> gasList = LocalGHGResultList().ResultList.Where(l => l.Plant.PLANT_ID == ghgRrec.Plant.PLANT_ID && l.EFMType == ghgRrec.EFMType).ToList();
                    gv.DataSource = gasList.Distinct().ToList();
                    gv.DataBind();
                }
                catch
                {
                    ;
                }
            }
        }

        public void gvGasList2_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblCO2Factor");
                    if (lbl.Text.Contains("0.000"))
                        lbl.Text = "";
                    else
                        lbl.Text = lbl.Text.Substring(0, lbl.Text.IndexOf('.') + 5);

                    lbl = (Label)e.Row.Cells[0].FindControl("lblEmitValue");
                    lbl.Text = lbl.Text.Substring(0, lbl.Text.IndexOf('.') + 3);
                }
                catch
                {
                    ;
                }
            }
        }

    }
}