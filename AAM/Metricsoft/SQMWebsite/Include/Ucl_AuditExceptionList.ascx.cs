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
		public Panel AuditListPanel
		{
			get { return pnlAuditList; }
		}
		//public RadGrid AuditListGrid
		//{
		//	get { return rgCaseList; }
		//}

		public RadGrid AuditListEhsGrid
		{
			get { return rgAuditList; }
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

		protected void lnkEditAudit(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] args = lnk.CommandArgument.ToString().Split('~');
			if (args[1].Equals("C"))
				SessionManager.ReturnObject = "Closed";
			else if (args[1].Equals("D"))
				SessionManager.ReturnObject = "DisplayOnly";
			else
				SessionManager.ReturnObject = "Notification";
			SessionManager.ReturnRecordID = Convert.ToDecimal(args[0]);
			SessionManager.ReturnStatus = true;
		}

		protected void lnkAuditRedirect(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = lnk.CommandArgument;
			SessionManager.ReturnStatus = true;
			Response.Redirect("/EHS/EHS_Audits.aspx");
		}

		//protected void lnkProblemCaseRedirect(Object sender, EventArgs e)
		//{
		//	//try
		//	//{
		//	//	LinkButton lnk = (LinkButton)sender;
		//	//	PROB_CASE probCase = ProblemCase.LookupCaseByAudit(Convert.ToDecimal(lnk.CommandArgument));
		//	//	if (probCase != null)
		//	//	{
		//	//		SessionManager.ReturnObject = probCase.PROBCASE_ID;
		//	//		SessionManager.ReturnStatus = true;
		//	//		Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
		//	//	}
		//	//}
		//	//catch { ; }
		//}

		//public void BindAuditList(object theList)
		//{
		//	pnlAuditList.Visible = true;
		//	gvAuditList.DataSource = theList;
		//	gvAuditList.DataBind();
		//}

		public void gvAuditList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
			{
				try
				{
					HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfAuditDate");
					Label lbl = (Label)e.Row.Cells[0].FindControl("lblAuditDate");
					lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(hf.Value), "d", false);

					hf = (HiddenField)e.Row.Cells[0].FindControl("hfAuditID");
					lbl = (Label)e.Row.Cells[0].FindControl("lblAuditID");
					lbl.Text = WebSiteCommon.FormatID(Convert.ToInt32(hf.Value), 6);
				}
				catch
				{
				}
			}
		}

		protected void gvAuditList_ItemCreated(object sender, GridItemEventArgs e)
		{
			//if (e.Item is GridNestedViewItem)
			//{
			//	(e.Item.FindControl("rgAuditAnswers") as RadGrid).NeedDataSource += new GridNeedDataSourceEventHandler(rgAuditAnswers_NeedDataSource);
			//}
		}

		protected void rgAuditList_ItemCommand(object sender, GridCommandEventArgs e)
		{
			if (e.CommandName == RadGrid.ExpandCollapseCommandName)
			{
				foreach (GridItem item in e.Item.OwnerTableView.Items)
				{
					if (item.Expanded && item != e.Item)
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
					List<EHSAuditQuestion> questions = (List<EHSAuditQuestion>)EHSAuditMgr.SelectAuditQuestionExceptionList(auditID);

					(sender as RadGrid).DataSource = questions;
				}
			}
			catch (Exception ex) {  }
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
		public Button BTNReceiptSearch
		{
			get { return btnReceiptSearch; }
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

		protected void btnReceiptsSearchClick(object sender, EventArgs e)
		{
			if (OnSearchReceiptsClick != null)
			{
				Button btn = (Button)sender;
				OnSearchReceiptsClick(btn.CommandArgument);
			}
		}

		//protected void lnkIssue_Click(object sender, EventArgs e)
		//{
		//	if (OnQualityIssueClick != null)
		//	{
		//		LinkButton lnk = (LinkButton)sender;
		//		OnQualityIssueClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
		//	}
		//}

		//public void gvIssueList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		//{
		//	if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
		//	{
		//		try
		//		{
		//			Label lbl;
		//			HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfIssueID");

		//			LinkButton lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkViewIssue_out");
		//			lnk.Text = WebSiteCommon.FormatID(Convert.ToInt32(hf.Value), 6);

		//			lbl = (Label)e.Row.Cells[0].FindControl("lblDisposition_Out");
		//			string tempDisposition = lbl.Text;
		//			lbl.Text = WebSiteCommon.GetXlatValue("NCDisposition", lbl.Text);

		//			lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkIssueDate_Out");
		//			lnk.Text = WebSiteCommon.LocalTime(Convert.ToDateTime(lnk.Text), SessionManager.UserContext.TimeZoneID).ToShortDateString();

		//			lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkIssueTask_out");
		//			lnk.Text = WebSiteCommon.GetXlatValueLong("taskType", lnk.Text);

		//			TASK_STATUS task = new TASK_STATUS();
		//			hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskStatus");
		//			task.TASK_ID = Convert.ToDecimal(hf.Value);
		//			hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskDueDate");
		//			task.DUE_DT = WebSiteCommon.LocalTime(Convert.ToDateTime(hf.Value), SessionManager.UserContext.TimeZoneID);
		//			Image img = (Image)e.Row.Cells[0].FindControl("imgTaskStatus");
		//			TaskStatus status = TaskMgr.CalculateTaskStatus(task);
		//			img.ImageUrl = TaskMgr.TaskStatusImage(status);
		//			img.ToolTip = status.ToString();
		//		}
		//		catch
		//		{
		//		}
		//	}
		//}

		#endregion


		#region problemcase

		//protected void lbAuditId_Click(Object sender, EventArgs e)
		//{
		//	if (OnProblemCaseClick != null)
		//	{
		//		LinkButton lnk = (LinkButton)sender;
		//		OnProblemCaseClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
		//	}
		//}

		//protected void lnkCase_Click(object sender, EventArgs e)
		//{
		//	if (OnProblemCaseClick != null)
		//	{
		//		LinkButton lnk = (LinkButton)sender;
		//		OnProblemCaseClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
		//	}
		//}

		//protected void lbReport_Click(object sender, EventArgs e)
		//{
		//	LinkButton btn = (LinkButton)sender;
		//	SessionManager.ReturnObject = btn.CommandArgument;
		//	SessionManager.ReturnStatus = true;

		//	string caseType = btn.Attributes["CaseType"];

		//	Response.Redirect("/Problem/Problem_Rpt.aspx?c=" + caseType);
		//}

		//protected void btnCaseReport_Click(object sender, EventArgs e)
		//{
		//	Button btn = (Button)sender;
		//	SessionManager.ReturnObject = btn.CommandArgument;
		//	SessionManager.ReturnStatus = true;

		//	Response.Redirect("/Problem/Problem_Rpt.aspx?c=" + btn.Attributes["ProbCaseType"]);
		//}

		#endregion

		#region ehsaudit

		//public void BindAuditListHeader(AUDIT audit, TaskItem taskItem)
		//{
		//	pnlAuditTaskHdr.Visible = true;
		//	lblCaseDescription.Visible = lblAuditDescription.Visible = lblActionDescription.Visible = false;

		//	lblAuditDescription.Visible = true;


		//	if (taskItem.Plant != null)
		//		lblCasePlant_out.Text = taskItem.Plant.PLANT_NAME;
		//	lblResponsible_out.Text = SQMModelMgr.FormatPersonListItem(taskItem.Person);
		//	lblCase2ID_out.Text = WebSiteCommon.FormatID(audit.AUDIT_ID, 6);
		//	// lblCase2Desc_out.Text = audit.ISSUE_TYPE;
		//	lblCase2Desc_out.Text = taskItem.Task.DESCRIPTION;
		//}

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


		//public void BindAuditActionList(object theList, string appContext)
		//{
		//	pnlAuditListRepeater.Visible = false;
		//	pnlAuditActionList.Visible = true;
		//	staticAppContext = appContext;

		//	rgAuditActionList.DataSource = theList;
		//	rgAuditActionList.DataBind();
		//}

		//protected void rgAuditActionList_ItemDataBound(object sender, GridItemEventArgs e)
		//{

		//	if (e.Item is GridDataItem)
		//	{
		//		GridDataItem item = (GridDataItem)e.Item;
		//		HiddenField hf;
		//		Label lbl;

		//		EHSAuditData data = (EHSAuditData)e.Item.DataItem;

		//		try
		//		{
		//			lbl = (Label)e.Item.FindControl("lblAuditId");
		//			lbl.Text = WebSiteCommon.FormatID(data.Audit.AUDIT_ID, 6);


		//			if (data.Audit.DESCRIPTION.Length > 200)
		//			{
		//				lbl = (Label)e.Item.FindControl("lblDescription");
		//				lbl.Text = data.Audit.DESCRIPTION.Substring(0, 200) + "...";
		//			}

		//			lbl = (Label)e.Item.FindControl("lblDueDT");
		//			AUDIT_ANSWER entry = data.Audit.AUDIT_ANSWER.Where(l => l.AUDIT_QUESTION_ID == 65).FirstOrDefault();  // due date
		//			if (entry != null && !string.IsNullOrEmpty(entry.ANSWER_VALUE))
		//			{
		//				lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(entry.ANSWER_VALUE), "d", false);
		//				entry = data.Audit.AUDIT_ANSWER.Where(l => l.AUDIT_QUESTION_ID == 64).FirstOrDefault(); // responsible person
		//				if (entry != null && !string.IsNullOrEmpty(entry.ANSWER_VALUE))
		//				{
		//					lbl = (Label)e.Item.FindControl("lblResponsible");
		//					lbl.Text = entry.ANSWER_VALUE;
		//				}
		//			}

		//			RadGrid gv = (RadGrid)e.Item.FindControl("rgAuditActions");
		//			List<AUDIT_ANSWER> auditActionList = new List<AUDIT_ANSWER>();
		//			auditActionList.AddRange(data.Audit.AUDIT_ANSWER.Where(l => l.AUDIT_QUESTION_ID == 24 || l.AUDIT_QUESTION_ID == 27).ToList());
		//			if (auditActionList.Count > 0)
		//			{
		//				baseRowIndex = e.Item.RowIndex;
		//				gv.DataSource = auditActionList;
		//				gv.DataBind();
		//				gv.Visible = true;
		//			}

		//			LinkButton lb8d = (LinkButton)e.Item.FindControl("lb8d");
		//			if (lb8d != null && UserContext.RoleAccess() <= AccessMode.Partner)
		//				lb8d.Visible = false;
		//		}

		//		catch
		//		{
		//		}

		//	}
		//}

		//protected void rgAuditActions_ItemDataBound(object sender, GridItemEventArgs e)
		//{

		//	if (e.Item is GridDataItem)
		//	{
		//		GridDataItem item = (GridDataItem)e.Item;
		//		if ((baseRowIndex % 4) == 0)
		//			e.Item.Cells[0].BackColor = e.Item.Cells[1].BackColor = e.Item.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#ededed");
		//	}
		//}

		//protected void rgAuditActionList_SortCommand(object sender, GridSortCommandEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayAudits";
		//}
		//protected void rgAuditActionList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayAudits";
		//}
		//protected void rgAuditActionList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayAudits";
		//}
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
