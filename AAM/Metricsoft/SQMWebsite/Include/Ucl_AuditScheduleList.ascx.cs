using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_AuditScheduleList : System.Web.UI.UserControl
	{
		static List<QI_OCCUR> issueList;
		static string staticAppContext;
		static int baseRowIndex;

		RadPersistenceManager persistenceManager;

		public event GridItemClick OnQualityIssueClick;
		public event GridItemClick OnQualityIssueListCloseClick;
		public event GridItemClick OnProblemCaseClick;
		public event GridItemClick OnAuditClick;
		public event EditItemClick OnTaskClick;
		public event EditItemClick OnCaseTaskClick;
		public event CommandClick OnSearchClick;
		public event CommandClick OnSearchReceiptsClick;

		public bool LinksDisabled
		{
			get;
			set;
		}

		#region audit
		public RadGrid AuditScheduleListEhsGrid
		{
			get { return rgAuditScheduleList; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected void lbAudit_Click(Object sender, EventArgs e)
		{
			if (OnAuditClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnAuditClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void lnkEditAuditSchedule(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			//string[] args = lnk.CommandArgument.ToString().Split('~');
			//if (args[1].Equals("C"))
			//	SessionManager.ReturnObject = "Closed";
			//else
			//	SessionManager.ReturnObject = "Notification";
			//SessionManager.ReturnRecordID = Convert.ToDecimal(args[0]);
			SessionManager.ReturnObject = "Notification";
			SessionManager.ReturnRecordID = Convert.ToDecimal(lnk.CommandArgument.ToString());
			SessionManager.ReturnStatus = true;
		}

		protected void lnkAuditRedirect(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = lnk.CommandArgument;
			SessionManager.ReturnStatus = true;
			Response.Redirect("/EHS/EHS_AuditScheduler.aspx");
		}

		#endregion

		#region qualityissue

		public RadComboBox DDLPlantSelect
		{
			get { return ddlPlantSelect; }
		}
		public Button BTNSearch
		{
			get { return btnSearch; }
		}
		public DateTime FromDate
		{
			get { return new DateTime(Convert.ToDateTime(dmPeriodFrom.SelectedDate).Year, Convert.ToDateTime(dmPeriodFrom.SelectedDate).Month, 1); }
		}
		public DateTime ToDate
		{
			get { return new DateTime(Convert.ToDateTime(dmPeriodTo.SelectedDate).Year, Convert.ToDateTime(dmPeriodTo.SelectedDate).Month, DateTime.DaysInMonth(Convert.ToDateTime(dmPeriodTo.SelectedDate).Year, Convert.ToDateTime(dmPeriodTo.SelectedDate).Month)); }
		}

		public decimal[] DDLPlantSelectIDS()
		{
			string[] plantSels = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => i.Value).ToArray();
			return Array.ConvertAll(plantSels, new Converter<string, decimal>(decimal.Parse));
		}
		public string[] DDLPlantSelectNames()
		{
			string[] plantSels = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => i.Text).ToArray();
			return plantSels;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblPeriodTo.Text = Resources.LocalizedText.To + ":";

			if (!Page.IsPostBack)
			{

			}
		}

		protected void btnIssueListClose_Click(object sender, EventArgs e)
		{
			if (OnQualityIssueListCloseClick != null)
			{
				OnQualityIssueListCloseClick(0);
			}
		}

		protected void ddlDateSpanChange(object sender, EventArgs e)
		{

		}

		protected void btnAuditsSearchClick(object sender, EventArgs e)
		{
			if (OnSearchClick != null)
			{
				Button btn = (Button)sender;
				OnSearchClick(btn.CommandArgument);
			}
		}

		#endregion

		#region ehsaudit

		public void BindAuditListRepeater(object theList, string appContext)
		{
			pnlAuditListRepeater.Visible = true;
			staticAppContext = appContext;

			rgAuditScheduleList.DataSource = theList;
			rgAuditScheduleList.DataBind();
		}

		protected void rgAuditScheduleList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				EHSAuditSchedulerData data = (EHSAuditSchedulerData)e.Item.DataItem;

				LinkButton lnk = (LinkButton)e.Item.FindControl("lbAuditScheduleId");
				lnk.Text = WebSiteCommon.FormatID(data.AuditScheduler.AUDIT_SCHEDULER_ID, 6);

				lbl = (Label)e.Item.FindControl("lblAuditScheduleStatus");

				if (data.AuditScheduler.INACTIVE)
				{
					lbl.Text = Resources.LocalizedText.Inactive;
				}
				else
				{
					lbl.Text = Resources.LocalizedText.Active;
				}

				lbl = (Label)e.Item.FindControl("lblDayOfWeek");
				DayOfWeek day = (DayOfWeek)data.AuditScheduler.DAY_OF_WEEK;
				lbl.Text = day.ToString();

				lnk = (LinkButton)e.Item.FindControl("lbAuditScheduleId");
				lnk.CommandArgument = data.AuditScheduler.AUDIT_SCHEDULER_ID.ToString();

			}
		}


		protected void rgAuditScheduleList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void rgAuditScheduleList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}
		protected void rgAuditScheduleList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}


		#endregion

		#region common
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

		public string GetFullAuditName(string typeCode)
		{
			return WebSiteCommon.GetXlatValue("auditType", typeCode);
		}

		public string EvaluateStatus(DateTime? closeDate)
		{
			return (closeDate == null) ?
				"<span style=\"color: #A3461F;\">Active</span>" :
				"<span style=\"color: #008800;\">Closed " + ((DateTime)closeDate).ToShortDateString() + "</span>";
		}

		#endregion

	}
}
