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
	public partial class Ucl_AuditExceptionList : System.Web.UI.UserControl
	{
		static List<QI_OCCUR> issueList;
		static string staticAppContext;
		static int baseRowIndex;

		RadPersistenceManager persistenceManager;

		//public event GridItemClick OnAuditClick;
		public event EditItemClick OnTaskClick;
		public event CommandClick OnSearchClick;
		//public event CommandClick OnSearchReceiptsClick;
		public event GridItemClick2 OnExceptionListItemClick;
		public event GridItemClick2 OnExceptionChangeStatusClick;

		public bool LinksDisabled
		{
			get;
			set;
		}

		private List<XLAT> TaskXLATList
		{
			get { return ViewState["TaskXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["TaskXLATList"]; }
			set { ViewState["TaskXLATList"] = value; }
		}

		#region grids
		public RadGrid AuditListEhsGrid
		{
			get { return rgAuditList; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		#endregion

		#region search criteria

		public RadComboBox DDLPlantSelect
		{
			get { return ddlPlantSelect; }
		}
		public Button BTNSearch
		{
			get { return btnSearch; }
		}
		//public Button BTNReceiptSearch
		//{
		//	get { return btnReceiptSearch; }
		//}

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
			this.lblPeriodFrom.Text = Resources.LocalizedText.From + ":";
			this.lblPeriodTo.Text = Resources.LocalizedText.To + ":";

			if (!Page.IsPostBack)
			{

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

			rgAuditList.DataSource = theList;
			rgAuditList.DataBind();
		}

		protected void rgAuditList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				EHSAuditData data = (EHSAuditData)e.Item.DataItem;

				lbl = (Label)e.Item.FindControl("lblAuditId");
				lbl.Text = WebSiteCommon.FormatID(data.Audit.AUDIT_ID, 6);

				if (data.Audit.DESCRIPTION.Length > 120)
				{
					lbl = (Label)e.Item.FindControl("lblDescription");
					lbl.Text = data.Audit.DESCRIPTION.Substring(0, 117) + "...";
				}

				//lbl = (Label)e.Item.FindControl("lblDescription");
				//lbl.Text = HttpUtility.HtmlEncode(lbl.Text);

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblAuditBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
					hdnAuditPerson.Value = data.Person.PERSON_ID.ToString();
				}

				lbl = (Label)e.Item.FindControl("lblAuditStatus");

				if (data.Audit.CURRENT_STATUS == "C")
				{
					DateTime clsDate = (DateTime)data.Audit.CLOSE_DATE_DATA_COMPLETE;
					lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "C") + " " + SQMBasePage.FormatDate(clsDate, "d", false);
				}
				else
				{
					if (data.DaysToClose == 0)
					{
						DateTime tmp = ((DateTime)data.Audit.AUDIT_DT).AddDays(data.AuditType.DAYS_TO_COMPLETE);
						lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "X") + "<br/>(" + SQMBasePage.FormatDate(tmp, "d", false) + ")";
					}
					else if (data.Audit.PERCENT_COMPLETE > 0)
						lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "I") + "<br/>(" + data.DaysToClose + ")";
					else
						lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "A") + "<br/>(" + data.DaysToClose + ")";
				}

				//LinkButton lnk = (LinkButton)e.Item.FindControl("lbAuditId");

				//if (SessionManager.UserContext.Person.PERSON_ID == data.Person.PERSON_ID)
				//	lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~" + data.Status;
				//else if (!data.Status.Equals("C"))
				//	lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~D";
				//else
				//	lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~C";

			}
		}

		protected void rgAuditList_ItemCommand(object sender, GridCommandEventArgs e)
		{
			// add this back to the grid to hit this code... OnItemCommand="rgAuditList_ItemCommand"  
			if (e.CommandName == RadGrid.ExpandCollapseCommandName)
			{
				foreach (GridItem item in e.Item.OwnerTableView.Items)
				{
					if (item.Expanded && item != e.Item && item.Parent.ID != e.Item.Parent.ID)
					{
						item.Expanded = false;
					}
				}
			}
		}

		protected void rgAuditAnswers_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			try
			{
				GridDataItem parentItem = ((sender as RadGrid).NamingContainer as GridNestedViewItem).ParentItem as GridDataItem;
				if (parentItem != null)
				{
					// we only want to select the audit answers that were adverse
					decimal auditID = Convert.ToDecimal(parentItem.GetDataKeyValue("Audit.Audit_ID").ToString());
					HiddenField hdnAuditTypeId = (HiddenField)parentItem.FindControl("hdnAuditTypeID");
					decimal auditTypeID = Convert.ToDecimal(hdnAuditTypeId.Value.ToString());
					List<EHSAuditQuestion> questions = (List<EHSAuditQuestion>)EHSAuditMgr.SelectAuditQuestionExceptionList(auditID, auditTypeID);

					(sender as RadGrid).DataSource = questions;
				}
			}
			catch (Exception ex) { }
		}

		protected void rgAuditAnswers_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				LinkButton lnk;
				Label lbl;
				EHSAuditQuestion data = (EHSAuditQuestion)e.Item.DataItem;

				lnk = (LinkButton)e.Item.FindControl("lnkAddTask");
				lnk.CommandArgument = data.AuditId.ToString() + "," + data.QuestionId.ToString();

				lnk = (LinkButton)e.Item.FindControl("lnkUpdateStatus");
				lnk.CommandArgument = data.AuditId.ToString() + "," + data.QuestionId.ToString();
				if (!hdnAuditPerson.Value.ToString().Equals(SessionManager.UserContext.Person.PERSON_ID.ToString()))
					lnk.Visible = false;

				if (TaskXLATList == null || TaskXLATList.Count == 0)
					TaskXLATList = SQMBasePage.SelectXLATList(new string[1] { "AUDIT_EXCEPTION_STATUS" });

				lbl = (Label)e.Item.FindControl("lblAnswerStatus");
				if (data.Status == null)
					lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_EXCEPTION_STATUS" && l.XLAT_CODE == "01").FirstOrDefault().DESCRIPTION;
				else
					lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_EXCEPTION_STATUS" && l.XLAT_CODE == data.Status.ToString()).FirstOrDefault().DESCRIPTION;

				lbl = (Label)e.Item.FindControl("lblResolutionDate");
				if (data.CompleteDate == null || data.CompleteDate.Year == 1)
					lbl.Text = "";
				else
					lbl.Text = data.CompleteDate.ToString("MM/dd/yyyy");

			}
		}

		protected void rgAuditAnswers_ItemCommand(object sender, GridCommandEventArgs e)
		{
			// add this back to the grid to hit this code... OnItemCommand="rgAuditAnswers_ItemCommand" 
			if (e.CommandName == RadGrid.ExpandCollapseCommandName)
			{
				foreach (GridItem item in e.Item.OwnerTableView.Items)
				{
					if (item.Expanded && item != e.Item && item.Parent.ID != e.Item.Parent.ID)
					{
						item.Expanded = false;
					}
				}
			}
		}

		protected void rgTasks_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			try
			{
				GridDataItem parentItem = ((sender as RadGrid).NamingContainer as GridNestedViewItem).ParentItem as GridDataItem;
				if (parentItem != null)
				{
					decimal questionID = Convert.ToDecimal(parentItem.GetDataKeyValue("QuestionId").ToString());
					HiddenField hdn = (HiddenField)parentItem.FindControl("hdnAuditID");
					decimal auditID = Convert.ToDecimal(hdn.Value.ToString());

					TaskStatusMgr myTasks = new TaskStatusMgr().CreateNew(0, 0);

					//myTasks.LoadTaskList((int)TaskRecordType.Audit, auditID, questionID);
					List<TaskItem> tasks = TaskMgr.ExceptionTaskListByRecord((int)TaskRecordType.Audit, auditID, questionID);

					(sender as RadGrid).DataSource = tasks;
				}
			}
			catch (Exception ex) { }
		}

		protected void rgTasks_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				TaskItem data = (TaskItem)e.Item.DataItem;

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblTaskAssignedTo");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}
			}
		}

		protected void lnkAddTask_Click(Object sender, EventArgs e)
		{
			if (OnExceptionListItemClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				string[] cmd = lnk.CommandArgument.Split(',');
				// call sending AuditID, QuestionID
				OnExceptionListItemClick(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			}
		}

		protected void lnkUpdateStatus_Click(object sender, EventArgs e)
		{
			if (OnExceptionChangeStatusClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				string[] cmd = lnk.CommandArgument.Split(',');
				// call sending AuditID, QuestionID
				OnExceptionChangeStatusClick(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			}
		}

		protected void lbEditReport_Click(object sender, EventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = Convert.ToDecimal((sender as LinkButton).CommandArgument);
		}

		protected void rgAuditList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void rgPreventativeList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}

		protected void rgAuditList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayAudits";
		}
		protected void rgAuditList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
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
