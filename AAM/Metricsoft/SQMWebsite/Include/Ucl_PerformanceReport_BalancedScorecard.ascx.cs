using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_PerformanceReport_BalancedScorecard : UserControl
	{
		public int Year { get; set; }
		public int Width { get; set; }
		public List<dynamic> Data { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			this.rptBalancedScorecard.DataSource = this.Data;
			this.rptBalancedScorecard.DataBind();
		}

		protected void rptBalancedScorecard_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				var rgBalancedScorescardHeader = e.Item.FindControl("rgBalancedScorescardHeader") as RadGrid;
				if (this.Year == DateTime.Today.Year)
				{
					int nextMonth = DateTime.Today.Month + 1;
					for (int i = nextMonth; i < 13; ++i)
						rgBalancedScorescardHeader.MasterTableView.GetColumn("Month" + i).Visible = false;
				}
				rgBalancedScorescardHeader.MasterTableView.Width = new Unit(this.Width, UnitType.Pixel);
				rgBalancedScorescardHeader.DataSource = new List<dynamic>();
				rgBalancedScorescardHeader.DataBind();
			}
			else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				dynamic dataItem = e.Item.DataItem;
				var rgBalancedScorecardItem = e.Item.FindControl("rgBalancedScorecardItem") as RadGrid;
				if (this.Year == DateTime.Today.Year)
				{
					int nextMonth = DateTime.Today.Month + 1;
					for (int i = nextMonth; i < 13; ++i)
						rgBalancedScorecardItem.MasterTableView.GetColumn("Month" + i).Visible = false;
				}
				rgBalancedScorecardItem.MasterTableView.Width = new Unit(this.Width, UnitType.Pixel);
				rgBalancedScorecardItem.DataSource = new List<dynamic>()
				{
					new
					{
						ItemType = dataItem.Name
					},
					dataItem.TRIR,
					dataItem.FrequencyRate,
					dataItem.SeverityRate
				};
				rgBalancedScorecardItem.DataBind();
			}
		}

		bool didFirstHeader_rgBalancedScorescardHeader = false;

		protected void rgBalancedScorescardHeader_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Header)
			{
				if (!this.didFirstHeader_rgBalancedScorescardHeader)
					this.didFirstHeader_rgBalancedScorescardHeader = true;
				else
				{
					int width = 200;
					if (this.Year == DateTime.Today.Year)
						width += 100 * (12 - DateTime.Today.Month);
					(sender as RadGrid).MasterTableView.GetColumn("Target").HeaderStyle.Width = new Unit(width, UnitType.Pixel);
				}
			}
		}

		protected void rgBalancedScorescardHeader_PreRender(object sender, EventArgs e)
		{
			var cells = (sender as RadGrid).MasterTableView.GetItems(GridItemType.Header)[0].Cells;
			cells[cells.Cast<GridTableHeaderCell>().Select(c => c.Text).ToList().IndexOf("Year")].Text = this.Year.ToString();
		}

		protected void rgBalancedScorecardItem_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
			{
				int width = 150;
				if (this.Year == DateTime.Today.Year)
					width += 100 * (12 - DateTime.Today.Month);
				(sender as RadGrid).MasterTableView.GetColumn("ItemType").HeaderStyle.Width = new Unit(width, UnitType.Pixel);

				var item = e.Item as GridDataItem;
				if (item["Target"].Text == "&nbsp;")
				{
					item["ItemType"].ColumnSpan = (this.Year == DateTime.Today.Year ? DateTime.Today.Month : 12) + 3;
					item["ItemType"].Font.Bold = true;
					item.Cells.Remove(item["YTD"]);
					for (int i = 12; i > 0; --i)
						item.Cells.Remove(item["Month" + i]);
					item.Cells.Remove(item["Target"]);
				}
				else
				{
					dynamic dataItem = e.Item.DataItem as dynamic;
					decimal target = dataItem.Target;
					var values = new decimal[]
					{
						dataItem.Jan, dataItem.Feb, dataItem.Mar, dataItem.Apr, dataItem.May, dataItem.Jun, dataItem.Jul, dataItem.Aug, dataItem.Sep, dataItem.Oct, dataItem.Nov, dataItem.Dec
					};
					TableCell cell;
					for (int i = 0; i < 12; ++i)
					{
						cell = item["Month" + (i + 1)];
						if (values[i] > target)
							cell.BackColor = Color.Red;
						else
							cell.BackColor = Color.Green;
					}
					cell = item["YTD"];
					if (dataItem.YTD > target)
						cell.BackColor = Color.Red;
					else
						cell.BackColor = Color.Green;
				}
			}
		}
	}
}
