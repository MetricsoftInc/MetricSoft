using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_PerformanceReport_TRIRPlant : UserControl
	{
		public int Year { get; set; }
		public List<dynamic> Data { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			this.rgTRIRPlant.MasterTableView.GetColumn("ImprovedOrDeclined").HeaderText =
				"Improved <span style=\"color: green; font-size: 18px\">&#8659;</span><br/>or<br/>Declined <span style=\"color: red; font-size: 18px\">&#8657;</span>";

			this.rgTRIRPlant.MasterTableView.GetColumn("TRIR2YearsAgo").HeaderText = "TRIR<br/>" + (this.Year - 2);
			this.rgTRIRPlant.MasterTableView.GetColumn("TRIRPreviousYear").HeaderText = "TRIR<br/>" + (this.Year - 1);
			this.rgTRIRPlant.MasterTableView.GetColumn("TRIRYTD").HeaderText = "TRIR YTD<br/>" + this.Year;
			this.rgTRIRPlant.MasterTableView.GetColumn("PercentChange").HeaderText = "% Change<br/>" + this.Year + " vs. " + (this.Year - 1);

			this.rgTRIRPlant.DataSource = this.Data;
			this.rgTRIRPlant.DataBind();
		}

		protected void rgTRIRPlant_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				dynamic dataItem = e.Item.DataItem;
				var row = e.Item as GridDataItem;

				if (row["BusinessUnit"].Text == "&nbsp;")
					row.Style.Add("background-color", "#d9d9d9");

				var percentChangeCell = row["PercentChange"];
				var label = new Label();
				if (dataItem.PercentChange < 0)
				{
					label.Text = "&#8659;";
					label.ForeColor = percentChangeCell.BackColor = Color.Green;
					percentChangeCell.ForeColor = Color.White;
				}
				else if (dataItem.PercentChange > 0)
				{
					label.Text = "&#8657;";
					label.ForeColor = percentChangeCell.BackColor = Color.Red;
					percentChangeCell.ForeColor = Color.White;
				}
				else
					label.Text = "=";
				row["ImprovedOrDeclined"].Controls.Add(label);

				var progressToGoalCell = row["ProgressToGoal"];
				if (dataItem.ProgressToGoal < 0)
				{
					progressToGoalCell.BackColor = Color.Green;
					progressToGoalCell.ForeColor = Color.White;
				}
				else if (dataItem.ProgressToGoal > 0)
				{
					progressToGoalCell.BackColor = Color.Red;
					progressToGoalCell.ForeColor = Color.White;
				}
			}
		}

		protected void rgTRIRPlant_PreRender(object sender, EventArgs e)
		{
			var rg = sender as RadGrid;
			for (int i = rg.Items.Count - 2; i >= 0; --i)
			{
				var rowBU = rg.Items[i]["BusinessUnit"];
				var nextRow = rg.Items[i + 1];
				var nextRowBU = nextRow["BusinessUnit"];
				if (rowBU.Text == nextRowBU.Text)
				{
					rowBU.RowSpan = nextRowBU.RowSpan < 2 ? 2 : nextRowBU.RowSpan + 1;
					nextRowBU.Visible = false;
					nextRow["Plant"].Style.Add("border-left-width", "1px");
				}
			}
		}
	}
}
