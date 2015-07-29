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
	public partial class Ucl_AuditList : System.Web.UI.UserControl
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
			SessionManager.ReturnObject = "Notification";
			SessionManager.ReturnRecordID = Convert.ToDecimal(lnk.CommandArgument);
			SessionManager.ReturnStatus = true;
		}

		protected void lnkAuditRedirect(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = lnk.CommandArgument;
			SessionManager.ReturnStatus = true;
			Response.Redirect("/EHS/EHS_Audits.aspx");
		}

		protected void lnkProblemCaseRedirect(Object sender, EventArgs e)
		{
			//try
			//{
			//	LinkButton lnk = (LinkButton)sender;
			//	PROB_CASE probCase = ProblemCase.LookupCaseByAudit(Convert.ToDecimal(lnk.CommandArgument));
			//	if (probCase != null)
			//	{
			//		SessionManager.ReturnObject = probCase.PROBCASE_ID;
			//		SessionManager.ReturnStatus = true;
			//		Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
			//	}
			//}
			//catch { ; }
		}

		public void BindAuditList(object theList)
		{
			pnlAuditList.Visible = true;
			gvAuditList.DataSource = theList;
			gvAuditList.DataBind();
		}

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

		#endregion

		#region qualityissue

		//public Panel QualityIssueListPanel
		//{
		//	get { return pnlQualityIssueList; }
		//}
		//public GridView QualityIssueListGrid
		//{
		//	get { return gvIssueList; }
		//}

		//public int CSTListCount
		//{
		//	get { return rgCSTIssueList.Items.Count; }
		//}

		public RadComboBox DDLPlantSelect
		{
			get { return ddlPlantSelect; }
		}
		//public RadComboBox DDLSeveritySelect
		//{
		//	get { return ddlSeveritySelect; }
		//}
		//public RadTextBox TBPartSearch
		//{
		//	get { return tbPartSearch; }
		//}

		//public string SeveritySelect
		//{
		//	get { return ddlSeveritySelect.SelectedValue; }
		//}
		//public bool ShowImages
		//{
		//	get { return cbShowImage.Checked; }
		//}
		//public string PartSearch
		//{
		//	get { return tbPartSearch.Text; }
		//}
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
				//ddlSeveritySelect.Items.AddRange(WebSiteCommon.PopulateRadListItems("auditSeverity"));
				//ddlSeveritySelect.Items.Insert(0, new RadComboBoxItem("", ""));
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

		protected void lnkIssue_Click(object sender, EventArgs e)
		{
			if (OnQualityIssueClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnQualityIssueClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		public void BindCSTIssueSearch(bool visible, string context, PSsqmEntities ctx)
		{
			pnlCSTIssueSearch.Visible = visible;
			dmPeriodFrom.SelectedDate = DateTime.Now.AddMonths(-6);
			dmPeriodTo.SelectedDate = DateTime.Now.AddMonths(1);
			dmPeriodFrom.ShowPopupOnFocus = dmPeriodTo.ShowPopupOnFocus = true;
			switch (context)
			{
				case "RCV":
					lblPlantSelect.Text = hfRCVPlantSelect.Value;
					if (SQMModelMgr.ReceiptCount(ctx) > 0)
						btnReceiptSearch.Visible = true;
					break;
				default:
					lblPlantSelect.Text = hfCSTPlantSelect.Value;
					btnReceiptSearch.Visible = false;
					break;
			}
		}

		//public void BindCSTIssueList(object theList, string context, bool showImages)
		//{
		//	// quality customer audits
		//	pnlCSTIssueList.Visible = true;
		//	rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = false;

		//	switch (context)
		//	{
		//		case "RCV":
		//			rgCSTIssueList.MasterTableView.GetColumn("ReceiptColumn").Visible = true;
		//			rgCSTIssueList.MasterTableView.GetColumn("PartColumn").Visible = false;
		//			if (showImages)
		//				rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = true;
		//			break;
		//		case "CST":
		//		default:
		//			rgCSTIssueList.MasterTableView.GetColumn("ReceiptColumn").Visible = false;
		//			rgCSTIssueList.MasterTableView.GetColumn("PartColumn").Visible = true;
		//			if (showImages)
		//				rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = true;
		//			break;
		//	}
		//	rgCSTIssueList.DataSource = theList;
		//	rgCSTIssueList.DataBind();
		//}

		//protected void rgCSTIssueList_ItemDataBound(object sender, GridItemEventArgs e)
		//{

		//	//if (e.Item is GridDataItem)
		//	//{
		//	//	GridDataItem item = (GridDataItem)e.Item;
		//	//	HiddenField hf;
		//	//	Label lbl;

		//	//	try
		//	//	{
		//	//		QualityAuditData data = (QualityAuditData)e.Item.DataItem;

		//	//		if (data.Person != null)
		//	//		{
		//	//			lbl = (Label)e.Item.FindControl("lblReportedBy");
		//	//			lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
		//	//		}

		//	//		Image img = (Image)e.Item.FindControl("imgRespLocation");
		//	//		if (data.PlantResponsible.PLANT_ID != data.Plant.PLANT_ID)
		//	//			img.Visible = true;
		//	//		else
		//	//			img.Visible = false;

		//	//		if (!string.IsNullOrEmpty(data.QIIssue.SEVERITY))
		//	//		{
		//	//			lbl = (Label)e.Item.FindControl("lblSeverity");
		//	//			lbl.Text = WebSiteCommon.GetXlatValue("auditSeverity", data.QIIssue.SEVERITY);
		//	//		}

		//	//		if (rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible && data.AttachList != null)
		//	//		{
		//	//			lbl = (Label)e.Item.FindControl("lblAttach");
		//	//			Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
		//	//			lbl.Parent.Controls.AddAt(lbl.Parent.Controls.IndexOf(lbl), attch);
		//	//			attch.BindListAttachment(data.AttachList, "1", 1);
		//	//		}
		//	//	}

		//	//	catch { }
		//	//}
		//}

		//protected void rgCSTIssueList_SortCommand(object sender, GridSortCommandEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayIssues";
		//}
		//protected void rgCSTIssueList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayIssues";
		//}
		//protected void rgCSTIssueList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayIssues";
		//}

		//public void BindQualityIssueList(object theList, bool showCheckbox)
		//{
		//	// issueList = theList;
		//	pnlQualityIssueList.Visible = true;
		//	gvIssueList.DataSource = theList;
		//	gvIssueList.DataBind();
		//	if (!showCheckbox)
		//		gvIssueList.Columns[6].Visible = false;
		//	SetGridViewDisplay(gvIssueList, lblIssueListEmpty, divGVIssueListScroll, 20, 0);
		//}

		public void gvIssueList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
			{
				try
				{
					Label lbl;
					HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfIssueID");

					LinkButton lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkViewIssue_out");
					lnk.Text = WebSiteCommon.FormatID(Convert.ToInt32(hf.Value), 6);

					lbl = (Label)e.Row.Cells[0].FindControl("lblDisposition_Out");
					string tempDisposition = lbl.Text;
					lbl.Text = WebSiteCommon.GetXlatValue("NCDisposition", lbl.Text);

					lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkIssueDate_Out");
					lnk.Text = WebSiteCommon.LocalTime(Convert.ToDateTime(lnk.Text), SessionManager.UserContext.TimeZoneID).ToShortDateString();

					lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkIssueTask_out");
					lnk.Text = WebSiteCommon.GetXlatValueLong("taskType", lnk.Text);

					TASK_STATUS task = new TASK_STATUS();
					hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskStatus");
					task.TASK_ID = Convert.ToDecimal(hf.Value);
					hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskDueDate");
					task.DUE_DT = WebSiteCommon.LocalTime(Convert.ToDateTime(hf.Value), SessionManager.UserContext.TimeZoneID);
					Image img = (Image)e.Row.Cells[0].FindControl("imgTaskStatus");
					TaskStatus status = TaskMgr.CalculateTaskStatus(task);
					img.ImageUrl = TaskMgr.TaskStatusImage(status);
					img.ToolTip = status.ToString();
				}
				catch
				{
				}
			}
		}

		//public void BindQualityAuditHeader(QualityAuditData qualityAudit, bool showAll)
		//{
		//	pnlQualityAuditHdr.Visible = true;

		//	lblDetectedLocation_out.Text = qualityAudit.Plant.PLANT_NAME;
		//	lblIssueID_out.Text = WebSiteCommon.FormatID(qualityAudit.Audit.AUDIT_ID, 6);
		//	lblIssueDesc_out.Text = qualityAudit.Audit.DESCRIPTION;
		//	if (qualityAudit.Part != null)
		//		lblIssuePartNum_out.Text = qualityAudit.Part.PART_NUM;
		//	lblIssueResponsible_out.Text = SQMModelMgr.FormatPersonListItem(qualityAudit.Person);
		//	if (qualityAudit.PlantResponsible != null)
		//	{
		//		lblIssueResponsible_out.Text += " (" + qualityAudit.PlantResponsible.PLANT_NAME + ")";
		//	}
		//}

		//protected void rgCaseList_ItemDataBound(object sender, GridItemEventArgs e)
		//{
		//	//if (e.Item is GridHeaderItem)
		//	//{
		//	//	GridHeaderItem gh = e.Item as GridHeaderItem;
		//	//	if (staticAppContext == "QI")
		//	//	{
		//	//		//gh.Cells[4].Text = "";
		//	//		//gh.Cells[4].Visible = false;
		//	//		;
		//	//	}
		//	//	else
		//	//	{
		//	//		;
		//	//	}
		//	//}

		//	//if (e.Item is GridDataItem)
		//	//{
		//	//	try
		//	//	{
		//	//		GridDataItem item = (GridDataItem)e.Item;
		//	//		HiddenField hf = (HiddenField)item["Reports"].FindControl("hfProblemCaseType");

		//	//		LinkButton lbReportQi = (LinkButton)item["Reports"].FindControl("lbReport");
		//	//		HyperLink hlReportEhs = (HyperLink)item["Reports"].FindControl("hlReport");

		//	//		lbReportQi.Attributes.Add("CaseType", hf.Value);

		//	//		//lbReportQi.Visible = (hf.Value != "EHS");
		//	//		lbReportQi.Visible = true;
		//	//		hlReportEhs.Visible = (hf.Value == "EHS");

		//	//		ProblemCase probCase = (ProblemCase)e.Item.DataItem;

		//	//		Label lbl = (Label)e.Item.FindControl("lblCaseID");
		//	//		lbl.Text = WebSiteCommon.FormatID(probCase.ProbCase.PROBCASE_ID, 6);
		//	//		LinkButton lnk = (LinkButton)e.Item.FindControl("lbCaseId");
		//	//		if (lnk != null && UserContext.RoleAccess() < AccessMode.Partner)
		//	//			lnk.Enabled = false;


		//	//		lbl = (Label)e.Item.FindControl("lblAuditID");
		//	//		if (probCase.AuditList != null && probCase.AuditList.Count > 0)
		//	//		{
		//	//			lbl.Text = WebSiteCommon.FormatID(probCase.AuditList[0].AUDIT_ID, 6);
		//	//		}

		//	//		lbl = (Label)e.Item.FindControl("lblStatus");
		//	//		if (probCase.ProbCase.CLOSE_DT.HasValue)
		//	//			lbl.Text = WebSiteCommon.GetXlatValue("recordStatus", "C") + ": " + SQMBasePage.FormatDate((DateTime)probCase.ProbCase.CLOSE_DT, "d", false);
		//	//		else
		//	//		{
		//	//			lbl.Text = WebSiteCommon.GetXlatValue("caseStep", (Math.Max((decimal)probCase.ProbCase.PROGRESS, 1) - 1).ToString());
		//	//			hf = (HiddenField)e.Item.FindControl("hfStatus");
		//	//			if (hf.Value == "I")
		//	//			{
		//	//				Image img = (Image)e.Item.FindControl("imgStatus");
		//	//				img.ImageUrl = "/images/defaulticon/16x16/no.png";
		//	//				img.Visible = true;
		//	//			}
		//	//		}
		//	//	}
		//	//	catch
		//	//	{
		//	//	}
		//	//}
		//}


		#endregion


		#region problemcase

		protected void lbAuditId_Click(Object sender, EventArgs e)
		{
			if (OnProblemCaseClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnProblemCaseClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void lnkCase_Click(object sender, EventArgs e)
		{
			if (OnProblemCaseClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnProblemCaseClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void lbReport_Click(object sender, EventArgs e)
		{
			LinkButton btn = (LinkButton)sender;
			SessionManager.ReturnObject = btn.CommandArgument;
			SessionManager.ReturnStatus = true;

			string caseType = btn.Attributes["CaseType"];

			Response.Redirect("/Problem/Problem_Rpt.aspx?c=" + caseType);
		}

		protected void btnCaseReport_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			SessionManager.ReturnObject = btn.CommandArgument;
			SessionManager.ReturnStatus = true;

			Response.Redirect("/Problem/Problem_Rpt.aspx?c=" + btn.Attributes["ProbCaseType"]);
		}

		//public Panel ProblemCaseHdr
		//{
		//	get { return pnlProbCaseHdr; }
		//}

		//public void BindProblemCaseHeader(ProblemCase problemCase, bool showAll)
		//{
		//	pnlProbCaseHdr.Visible = true;
		//	if (problemCase.ProbCase.PROBCASE_TYPE == "EHS")
		//	{
		//		lblCaseID.Text = hfLblCaseIDEHS.Value;
		//	}
		//	lblCaseID_out.Text = problemCase.CaseID;
		//	lblCaseType_out.Text = WebSiteCommon.GetXlatValue("auditType", problemCase.ProbCase.PROBCASE_TYPE);
		//	lblCaseDesc_out.Text = problemCase.ProbCase.DESC_SHORT;
		//	// trProbCaseHdrRow2.Visible = showAll;
		//	if (showAll)
		//	{
		//		int progress = problemCase.CheckCaseStatus();
		//		lblCreateDate_out.Text = SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.CREATE_DT, "d", false);
		//		lblUpdateDate_out.Text = SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.LAST_UPD_DT, "d", false);
		//		//lblCaseProgress_out.Text = progress.ToString();
		//		if (problemCase.ProbCase.CLOSE_DT.HasValue)
		//			lblCaseStatus_out.Text = WebSiteCommon.GetXlatValue("recordStatus", "C") + ": " + SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.CLOSE_DT, "d", false);
		//		else
		//		{
		//			lblCaseStatus_out.Text = WebSiteCommon.GetXlatValue("caseStep", (Math.Max((decimal)problemCase.ProbCase.PROGRESS, 1) - 1).ToString());
		//			if (problemCase.ProbCase.STATUS == "I")
		//			{
		//				imgCaseStatus.ImageUrl = "/images/defaulticon/16x16/no.png";
		//				imgCaseStatus.Visible = true;
		//			}
		//		}
		//	}
		//}

		//public void BindProblemCaseHeader(ProblemCase problemCase, TaskItem taskItem)
		//{
		//	pnlAuditTaskHdr.Visible = true;
		//	lblAuditDescription.Visible = lblActionDescription.Visible = false;
		//	lblCaseDescription.Visible = true;

		//	if (taskItem.Plant != null)
		//		lblCasePlant_out.Text = taskItem.Plant.PLANT_NAME;
		//	lblResponsible_out.Text = SQMModelMgr.FormatPersonListItem(taskItem.Person);
		//	lblCase2ID_out.Text = problemCase.CaseID;
		//	lblCase2Desc_out.Text = problemCase.ProbCase.DESC_SHORT;
		//}

		//public void BindProblemCaseListRepeater(object theList, string appContext)
		//{
		//	pnlProbCaseListRepeater.Visible = true;
		//	staticAppContext = appContext;

		//	rgCaseList.DataSource = theList;
		//	rgCaseList.DataBind();
		//}

		#endregion

		#region ehsaudit

		public void BindAuditListHeader(AUDIT audit, TaskItem taskItem)
		{
			pnlAuditTaskHdr.Visible = true;
			lblCaseDescription.Visible = lblAuditDescription.Visible = lblActionDescription.Visible = false;

			lblAuditDescription.Visible = true;


			if (taskItem.Plant != null)
				lblCasePlant_out.Text = taskItem.Plant.PLANT_NAME;
			lblResponsible_out.Text = SQMModelMgr.FormatPersonListItem(taskItem.Person);
			lblCase2ID_out.Text = WebSiteCommon.FormatID(audit.AUDIT_ID, 6);
			// lblCase2Desc_out.Text = audit.ISSUE_TYPE;
			lblCase2Desc_out.Text = taskItem.Task.DESCRIPTION;
		}

		public void BindAuditListRepeater(object theList, string appContext, bool showImages)
		{
			pnlAuditListRepeater.Visible = true;
			staticAppContext = appContext;

			rgAuditList.DataSource = theList;
			rgAuditList.DataBind();
		}

		//public void BindPreventativeListRepeater(object theList, string appContext, bool showImages)
		//{
		//	pnlAuditActionList.Visible = false;
		//	pnlPreventativeListRepeater.Visible = true;
		//	staticAppContext = appContext;

		//	if (showImages)
		//		rgPreventativeList.MasterTableView.GetColumn("Attach").Visible = true;
		//	else
		//		rgPreventativeList.MasterTableView.GetColumn("Attach").Visible = false;

		//	rgPreventativeList.DataSource = theList;
		//	rgPreventativeList.DataBind();
		//}


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

				lbl = (Label)e.Item.FindControl("lblDescription");
				lbl.Text = HttpUtility.HtmlEncode(lbl.Text);

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblAuditBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}

				lbl = (Label)e.Item.FindControl("lblAuditStatus");
				if (data.Status == "C")
				{
					lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", "C") + " " + SQMBasePage.FormatDate((DateTime)data.Audit.CLOSE_DATE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
				}
				else if (data.Status == "N")
				{
					lbl.Text = "<strong>" + WebSiteCommon.GetXlatValue("incidentStatus", "N") + "</strong>";
				}
				else
				{
					lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", "A") + "<br/>(" + data.DaysOpen + ")";
				}

			}
		}

		//protected void rgPreventativeList_ItemDataBound(object sender, GridItemEventArgs e)
		//{
		//	if (e.Item is GridDataItem)
		//	{
		//		HiddenField hf;
		//		Label lbl;
		//		string val = "";

		//		EHSAuditData data = (EHSAuditData)e.Item.DataItem;

		//		lbl = (Label)e.Item.FindControl("lblAuditId");
		//		lbl.Text = WebSiteCommon.FormatID(data.Audit.AUDIT_ID, 6);

		//		lbl = (Label)e.Item.FindControl("lblDescription");
		//		lbl.Text = StringHtmlExtensions.TruncateHtml(data.Audit.DESCRIPTION, 100, "...");
		//		lbl.Text = lbl.Text.Replace("<a href", "<a target=\"blank\" href");

		//		if (data.Person != null)
		//		{
		//			lbl = (Label)e.Item.FindControl("lblReportedBy");
		//			lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
		//		}

		//		lbl = (Label)e.Item.FindControl("lblCategory");
		//		lbl.Text = EHSAuditMgr.SelectAuditAnswer(data.Audit, (decimal)EHSQuestionId.InspectionCategory) + "<br/>" +
		//			EHSAuditMgr.SelectAuditAnswer(data.Audit, (decimal)EHSQuestionId.RecommendationType);

		//		lbl = (Label)e.Item.FindControl("lblIncStatus");
		//		try
		//		{
		//			if (data.Status == "U")
		//			{
		//				lbl.Text = "Audited " + SQMBasePage.FormatDate((DateTime)data.Audit.CLOSE_DATE_DATA_COMPLETE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
		//			}
		//			else if (data.Status == "F")
		//			{
		//				lbl.Text = "Awaiting Funding " + SQMBasePage.FormatDate((DateTime)data.Audit.CLOSE_DATE_DATA_COMPLETE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
		//			}
		//			else if (data.Status == "C")
		//			{
		//				lbl.Text = "Closed  " + SQMBasePage.FormatDate((DateTime)data.Audit.CLOSE_DATE, "d", false) + "<br/><strong>Not Audited</strong>";
		//			}
		//			else
		//			{
		//				lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", data.Status) + "<br/>(" + data.DaysOpen + ")";
		//			}
		//		}
		//		catch
		//		{
		//			;
		//		}

		//		LinkButton lbEditReport = (LinkButton)e.Item.FindControl("lbEditReport");
		//		lbEditReport.Visible = true;

		//		try
		//		{
		//			lbl = (Label)e.Item.FindControl("lblAuditDT");
		//			lbl.Text = SQMBasePage.FormatDate(data.Audit.AUDIT_DT, "d", false);
		//			if ((val = data.EntryList.Where(l => l.AUDIT_QUESTION_ID == 80).Select(l => l.ANSWER_VALUE).FirstOrDefault()) != null && !string.IsNullOrEmpty(val))
		//			{
		//				val = val.Substring(0, val.IndexOf(' '));
		//				DateTime parseDate;
		//				if (DateTime.TryParse(val, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
		//					lbl.Text = parseDate.ToShortDateString();
		//			}
		//		}
		//		catch { }
		//		try
		//		{
		//			if ((val = data.EntryList.Where(l => l.AUDIT_QUESTION_ID == 92).Select(l => l.ANSWER_VALUE).FirstOrDefault()) != null && !string.IsNullOrEmpty(val))
		//			{
		//				val = val.Substring(0, val.IndexOf(' '));
		//				DateTime parseDate;
		//				if (DateTime.TryParse(val, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
		//				{
		//					lbl = (Label)e.Item.FindControl("lblDueDT");
		//					lbl.Text = parseDate.ToShortDateString();
		//				}
		//			}
		//		}
		//		catch { ; }

		//		if (data.RespPerson != null)
		//		{
		//			lbl = (Label)e.Item.FindControl("lblAssignedTo");
		//			lbl.Text = SQMModelMgr.FormatPersonListItem(data.RespPerson);
		//		}

		//		if (rgPreventativeList.MasterTableView.GetColumn("Attach").Visible && data.AttachList != null)
		//		{
		//			lbl = (Label)e.Item.FindControl("lblAttach");
		//			Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
		//			lbl.Parent.Controls.AddAt(lbl.Parent.Controls.IndexOf(lbl), attch);
		//			attch.BindListAttachment(data.AttachList, "1", 1);
		//		}
		//	}
		//}

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

		//protected void rgCaseList_SortCommand(object sender, GridSortCommandEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayCases";
		//}

		//protected void rgCaseList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayCases";
		//}

		//protected void rgCaseList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "DisplayCases";
		//}

		#endregion

	}
}
