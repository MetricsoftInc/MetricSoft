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

	public partial class Ucl_IncidentList : System.Web.UI.UserControl
	{
		static List<QI_OCCUR> issueList;
		static string staticAppContext;
		static int baseRowIndex;

		RadPersistenceManager persistenceManager;

		public event GridItemClick OnQualityIssueClick;
		public event GridItemClick OnQualityIssueListCloseClick;
		public event GridItemClick OnProblemCaseClick;
		public event GridItemClick OnIncidentClick;
		public event EditItemClick OnTaskClick;
		public event EditItemClick OnCaseTaskClick;
		public event CommandClick OnSearchClick;
		public event CommandClick OnSearchReceiptsClick;

		public bool LinksDisabled
		{
			get;
			set;
		}

		#region incident
		public Panel IncidentListPanel
		{
			get { return pnlIncidentList; }
		}
		public RadGrid IncidentListGrid
		{
			get { return rgCaseList; }
		}

		public RadGrid IncidentListEhsGrid
		{
			get { return rgIncidentList; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected void lbIncident_Click(Object sender, EventArgs e)
		{
			if (OnIncidentClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnIncidentClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void lnkEditIncident(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;

			RadButton rbNew = (RadButton)this.Parent.FindControl("rbNew");
			rbNew.Visible = false;

			SessionManager.ReturnObject = "Notification";
			SessionManager.ReturnRecordID = Convert.ToDecimal(lnk.CommandArgument);
			SessionManager.ReturnStatus = true;
		}

		protected void lnkIncidentRedirect(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = lnk.CommandArgument;
			SessionManager.ReturnStatus = true;
			Response.Redirect("/EHS/EHS_Incidents.aspx");
		}

		protected void lnkProblemCaseRedirect(Object sender, EventArgs e)
		{
			try
			{
				LinkButton lnk = (LinkButton)sender;
				PROB_CASE probCase = ProblemCase.LookupCaseByIncident(Convert.ToDecimal(lnk.CommandArgument));
				if (probCase != null)
				{
					SessionManager.ReturnObject = probCase.PROBCASE_ID;
					SessionManager.ReturnStatus = true;
					Response.Redirect("/Problem/Problem_Case.aspx?c=EHS");
				}
			}
			catch { ; }
		}

		public void BindIncidentList(object theList)
		{
			pnlIncidentList.Visible = true;
			gvIncidentList.DataSource = theList;
			gvIncidentList.DataBind();
		}

		public void gvIncidentList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
			{
				try
				{
					HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfIncidentDate");
					Label lbl = (Label)e.Row.Cells[0].FindControl("lblIncidentDate");
					lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(hf.Value), "d", false);

					hf = (HiddenField)e.Row.Cells[0].FindControl("hfIncidentID");
					lbl = (Label)e.Row.Cells[0].FindControl("lblIncidentID");
					lbl.Text = WebSiteCommon.FormatID(Convert.ToInt32(hf.Value), 6);
				}
				catch
				{
				}
			}
		}

		#endregion

		#region qualityissue
		
		public Panel QualityIssueListPanel
		{
			get { return pnlQualityIssueList; }
		}
		public GridView QualityIssueListGrid
		{
			get { return gvIssueList; }
		}

		public int CSTListCount
		{
			get { return rgCSTIssueList.Items.Count; }
		}

		public RadComboBox DDLPlantSelect
		{
			get { return ddlPlantSelect; }
		}
		public RadComboBox DDLSeveritySelect
		{
			get { return ddlSeveritySelect; }
		}
		public RadTextBox TBPartSearch
		{
			get { return tbPartSearch; }
		}

		public string SeveritySelect
		{
			get { return ddlSeveritySelect.SelectedValue; }
		}
		public bool ShowImages
		{
			get { return cbShowImage.Checked; }
		}
		public string  PartSearch
		{
			get { return tbPartSearch.Text; }
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
			string [] plantSels = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => i.Value).ToArray();
			return  Array.ConvertAll(plantSels, new Converter<string, decimal>(decimal.Parse));
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
				ddlSeveritySelect.Items.AddRange(WebSiteCommon.PopulateRadListItems("incidentSeverity"));
				ddlSeveritySelect.Items.Insert(0, new RadComboBoxItem("", ""));
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

		protected void btnIncidentsSearchClick(object sender, EventArgs e)
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

		public void BindCSTIssueList(object theList, string context, bool showImages)
		{
			// quality customer incidents
			pnlCSTIssueList.Visible = true;
			rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = false;

			switch (context)
			{
				case "RCV":
					rgCSTIssueList.MasterTableView.GetColumn("ReceiptColumn").Visible = true;
					rgCSTIssueList.MasterTableView.GetColumn("PartColumn").Visible = false;
					if (showImages)
						rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = true;
					break;
				case "CST":
				default:
					rgCSTIssueList.MasterTableView.GetColumn("ReceiptColumn").Visible = false;
					rgCSTIssueList.MasterTableView.GetColumn("PartColumn").Visible = true;
					if (showImages)
						rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible = true;
					break;
			}
			rgCSTIssueList.DataSource = theList;
			rgCSTIssueList.DataBind();
		}

		protected void rgCSTIssueList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				try
				{
					QualityIncidentData data = (QualityIncidentData)e.Item.DataItem;

					if (data.Person != null)
					{
						lbl = (Label)e.Item.FindControl("lblReportedBy");
						lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
					}

					Image img = (Image)e.Item.FindControl("imgRespLocation");
					if (data.PlantResponsible.PLANT_ID != data.Plant.PLANT_ID)
						img.Visible = true;
					else
						img.Visible = false;

					if (!string.IsNullOrEmpty(data.QIIssue.SEVERITY))
					{
						lbl = (Label)e.Item.FindControl("lblSeverity");
						lbl.Text = WebSiteCommon.GetXlatValue("incidentSeverity", data.QIIssue.SEVERITY);
					}

					if (rgCSTIssueList.MasterTableView.GetColumn("Attach").Visible  &&  data.AttachList != null)
					{
						lbl = (Label)e.Item.FindControl("lblAttach");
						Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
						lbl.Parent.Controls.AddAt(lbl.Parent.Controls.IndexOf(lbl), attch);
						attch.BindListAttachment(data.AttachList, "1", 1);
					}
				}

				catch { }
			}
		}

		protected void rgCSTIssueList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIssues";
		}
		protected void rgCSTIssueList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIssues";
		}
		protected void rgCSTIssueList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIssues";
		}

		public void BindQualityIssueList(object theList, bool showCheckbox)
		{
			// issueList = theList;
			pnlQualityIssueList.Visible = true;
			gvIssueList.DataSource = theList;
			gvIssueList.DataBind();
			if (!showCheckbox)
				gvIssueList.Columns[6].Visible = false;
			SetGridViewDisplay(gvIssueList, lblIssueListEmpty, divGVIssueListScroll, 20, 0);
		}

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

		public void BindQualityIncidentHeader(QualityIncidentData qualityIncident, bool showAll)
		{
			pnlQualityIncidentHdr.Visible = true;
			
			lblDetectedLocation_out.Text = qualityIncident.Plant.PLANT_NAME;
			lblIssueID_out.Text = WebSiteCommon.FormatID(qualityIncident.Incident.INCIDENT_ID, 6);
			lblIssueDesc_out.Text = qualityIncident.Incident.DESCRIPTION;
			if (qualityIncident.Part != null)
				lblIssuePartNum_out.Text = qualityIncident.Part.PART_NUM;
			lblIssueResponsible_out.Text = SQMModelMgr.FormatPersonListItem(qualityIncident.Person);
			if (qualityIncident.PlantResponsible != null)
			{
				lblIssueResponsible_out.Text += " (" + qualityIncident.PlantResponsible.PLANT_NAME + ")";
			}
		}

		protected void rgCaseList_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridHeaderItem)
			{
				GridHeaderItem gh = e.Item as GridHeaderItem;
				if (staticAppContext == "QI")
				{
					//gh.Cells[4].Text = "";
					//gh.Cells[4].Visible = false;
					;
				}
				else
				{
					;
				}
			}
			
			if (e.Item is GridDataItem)
			{
				try
				{
					GridDataItem item = (GridDataItem)e.Item;
					HiddenField hf = (HiddenField)item["Reports"].FindControl("hfProblemCaseType");

					LinkButton lbReportQi = (LinkButton)item["Reports"].FindControl("lbReport");
					HyperLink hlReportEhs = (HyperLink)item["Reports"].FindControl("hlReport");

					lbReportQi.Attributes.Add("CaseType", hf.Value);

					//lbReportQi.Visible = (hf.Value != "EHS");
					lbReportQi.Visible = true;
					hlReportEhs.Visible = (hf.Value == "EHS");

					ProblemCase probCase = (ProblemCase)e.Item.DataItem;

					Label lbl = (Label)e.Item.FindControl("lblCaseID");
					lbl.Text = WebSiteCommon.FormatID(probCase.ProbCase.PROBCASE_ID, 6);
					LinkButton lnk = (LinkButton)e.Item.FindControl("lbCaseId");
					if (lnk != null && UserContext.RoleAccess() < AccessMode.Partner)
						lnk.Enabled = false;
						

					lbl = (Label)e.Item.FindControl("lblIncidentID");
					if (probCase.IncidentList != null && probCase.IncidentList.Count > 0)
					{
						lbl.Text = WebSiteCommon.FormatID(probCase.IncidentList[0].INCIDENT_ID, 6);
					}

					lbl = (Label)e.Item.FindControl("lblStatus");
					if (probCase.ProbCase.CLOSE_DT.HasValue)
						lbl.Text = WebSiteCommon.GetXlatValue("recordStatus", "C") + ": " + SQMBasePage.FormatDate((DateTime)probCase.ProbCase.CLOSE_DT, "d", false);
					else
					{
						lbl.Text = WebSiteCommon.GetXlatValue("caseStep", (Math.Max((decimal)probCase.ProbCase.PROGRESS, 1) - 1).ToString());
						hf = (HiddenField)e.Item.FindControl("hfStatus");
						if (hf.Value == "I")
						{
							Image img = (Image)e.Item.FindControl("imgStatus");
							img.ImageUrl = "/images/defaulticon/16x16/no.png";
							img.Visible = true;
						}
					}
				}
				catch
				{
				}
			}
		}


		#endregion


		#region problemcase

		protected void lbIncidentId_Click(Object sender, EventArgs e)
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

		public Panel ProblemCaseHdr
		{
			get { return pnlProbCaseHdr; }
		}

		public void BindProblemCaseHeader(ProblemCase problemCase, bool showAll)
		{
			pnlProbCaseHdr.Visible = true;
			if (problemCase.ProbCase.PROBCASE_TYPE == "EHS")
			{
				lblCaseID.Text = hfLblCaseIDEHS.Value;
			}
			lblCaseID_out.Text = problemCase.CaseID;
			lblCaseType_out.Text = WebSiteCommon.GetXlatValue("incidentType", problemCase.ProbCase.PROBCASE_TYPE);
			lblCaseDesc_out.Text = problemCase.ProbCase.DESC_SHORT;
		   // trProbCaseHdrRow2.Visible = showAll;
			if (showAll)
			{
				int progress = problemCase.CheckCaseStatus();
				lblCreateDate_out.Text = SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.CREATE_DT, "d", false);
				lblUpdateDate_out.Text = SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.LAST_UPD_DT, "d", false);
				//lblCaseProgress_out.Text = progress.ToString();
				if (problemCase.ProbCase.CLOSE_DT.HasValue)
					lblCaseStatus_out.Text = WebSiteCommon.GetXlatValue("recordStatus", "C") + ": " + SQMBasePage.FormatDate((DateTime)problemCase.ProbCase.CLOSE_DT, "d", false);
				else
				{
					lblCaseStatus_out.Text = WebSiteCommon.GetXlatValue("caseStep", (Math.Max((decimal)problemCase.ProbCase.PROGRESS, 1) - 1).ToString());
					if (problemCase.ProbCase.STATUS == "I")
					{
						imgCaseStatus.ImageUrl = "/images/defaulticon/16x16/no.png";
						imgCaseStatus.Visible = true;
					}
				}
			}
		}

		public void BindProblemCaseHeader(ProblemCase problemCase, TaskItem taskItem)
		{
			pnlIncidentTaskHdr.Visible = true;
			lblIncidentDescription.Visible = lblActionDescription.Visible = false;
			lblCaseDescription.Visible = true;

			if (taskItem.Plant != null)
				lblCasePlant_out.Text = taskItem.Plant.PLANT_NAME;
			lblResponsible_out.Text = SQMModelMgr.FormatPersonListItem(taskItem.Person);
			lblCase2ID_out.Text = problemCase.CaseID;
			lblCase2Desc_out.Text = problemCase.ProbCase.DESC_SHORT;
		}

		public void BindProblemCaseListRepeater(object theList, string appContext)
		{
			pnlProbCaseListRepeater.Visible = true;
			staticAppContext = appContext;
			
			rgCaseList.DataSource = theList;
			rgCaseList.DataBind();
		}

		#endregion

		#region ehsincident

		public void BindIncidentListHeader(INCIDENT incident, TaskItem taskItem)
		{
			pnlIncidentTaskHdr.Visible = true;
			lblCaseDescription.Visible = lblIncidentDescription.Visible = lblActionDescription.Visible = false;

			if (incident.ISSUE_TYPE_ID == 13)  //  preventative action
				lblActionDescription.Visible = true;
			else
				lblIncidentDescription.Visible = true;
			

			if (taskItem.Plant != null)
				lblCasePlant_out.Text = taskItem.Plant.PLANT_NAME;
			lblResponsible_out.Text = SQMModelMgr.FormatPersonListItem(taskItem.Person);
			lblCase2ID_out.Text = WebSiteCommon.FormatID(incident.INCIDENT_ID, 6);
		   // lblCase2Desc_out.Text = incident.ISSUE_TYPE;
			lblCase2Desc_out.Text = taskItem.Task.DESCRIPTION;
		}

		public void BindIncidentListRepeater(object theList, string appContext, bool showImages, bool showReports)
		{
			pnlIncidentActionList.Visible = false;
			pnlIncidentListRepeater.Visible = true;
			staticAppContext = appContext;

			rgIncidentList.MasterTableView.GetColumn("ViewReports").Display = showReports;

			if (showImages)
				rgIncidentList.MasterTableView.GetColumn("Attach").Visible = true;
			else
				rgIncidentList.MasterTableView.GetColumn("Attach").Visible = false;

			rgIncidentList.DataSource = theList;
			rgIncidentList.DataBind();
		}

		public void BindPreventativeListRepeater(object theList, string appContext, bool showImages)
		{
			pnlIncidentActionList.Visible = false;
			pnlPreventativeListRepeater.Visible = true;
			staticAppContext = appContext;

			if (showImages)
				rgPreventativeList.MasterTableView.GetColumn("Attach").Visible = true;
			else
				rgPreventativeList.MasterTableView.GetColumn("Attach").Visible = false;

			rgPreventativeList.DataSource = theList;
			rgPreventativeList.DataBind();
		}


		protected void rgIncidentList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				EHSIncidentData data = (EHSIncidentData)e.Item.DataItem;

				lbl = (Label)e.Item.FindControl("lblIncidentId");
				lbl.Text = WebSiteCommon.FormatID(data.Incident.INCIDENT_ID, 6);

				if (data.Incident.DESCRIPTION.Length > 120)
				{
					lbl = (Label)e.Item.FindControl("lblDescription");
					lbl.Text = data.Incident.DESCRIPTION.Substring(0, 117) + "...";
				}

				lbl = (Label)e.Item.FindControl("lblDescription");
				lbl.Text = HttpUtility.HtmlEncode(lbl.Text);

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblReportedBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}

				lbl = (Label)e.Item.FindControl("lblIncStatus");
				if (data.Status == "C")
				{
					lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", "C") + " " + SQMBasePage.FormatDate((DateTime)data.Incident.CLOSE_DATE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
				}
				else if (data.Status == "C8")
				{
					lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", "C8") + " " + SQMBasePage.FormatDate((DateTime)data.Incident.CLOSE_DATE_8D, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
				}
				else if (data.Status == "N")
				{
					lbl.Text = "<strong>" + WebSiteCommon.GetXlatValue("incidentStatus", "N") + "</strong>";
				}
				else
				{
					lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", "A") + "<br/>(" + data.DaysOpen + ")";
				}
				
				LinkButton lb8d = (LinkButton)e.Item.FindControl("lb8d");
				LinkButton lbEditReport = (LinkButton)e.Item.FindControl("lbEditReport");
				lb8d.Visible = lbEditReport.Visible = false;  // mt - for AAM

				HyperLink hlReport = (HyperLink)e.Item.FindControl("hlReport");
				hlReport.Visible = true;

				/*
				INCIDENT_ANSWER entry = data.Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Create8D).FirstOrDefault();
				if (entry != null && entry.ANSWER_VALUE == "Yes")
				{
					if (UserContext.RoleAccess() > AccessMode.View)
						lb8d.Visible = true;
					else
						lb8d.Visible = false;

					lbEditReport.Visible = false;

					var problemCaseId = EHSIncidentMgr.SelectProblemCaseIdByIncidentId(data.Incident.INCIDENT_ID);
					if (problemCaseId > 0)
					{

						hlReport.NavigateUrl = "/EHS/EHS_Alert_PDF.aspx?pcid=" + EncryptionManager.Encrypt(problemCaseId.ToString());

						LinkButton lbReport = (LinkButton)e.Item.FindControl("lbReport");
						lbReport.Visible = true;
						lbReport.CommandArgument = problemCaseId.ToString();
						lbReport.Attributes.Add("CaseType", data.Incident.INCIDENT_TYPE);
					}	
				}
				else
				{
					lb8d.Visible = false;
					lbEditReport.Visible = true;

					hlReport.NavigateUrl = "/EHS/EHS_Alert_PDF.aspx?iid=" + EncryptionManager.Encrypt(data.Incident.INCIDENT_ID.ToString());
				}
				*/

				if (data.Incident.ISSUE_TYPE_ID == 10) // Prevention Verification
				{
					lbEditReport.Visible = false;
				}

				if (rgIncidentList.MasterTableView.GetColumn("Attach").Visible && data.AttachList != null)
				{
					lbl = (Label)e.Item.FindControl("lblAttach");
					Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
					lbl.Parent.Controls.AddAt(lbl.Parent.Controls.IndexOf(lbl), attch);
					attch.BindListAttachment(data.AttachList, "", 1);
				}
			}
		}

		protected void rgPreventativeList_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				HiddenField hf;
				Label lbl;
				string val = "";

				EHSIncidentData data = (EHSIncidentData)e.Item.DataItem;

				lbl = (Label)e.Item.FindControl("lblIncidentId");
				lbl.Text = WebSiteCommon.FormatID(data.Incident.INCIDENT_ID, 6);

				lbl = (Label)e.Item.FindControl("lblDescription");
				lbl.Text = StringHtmlExtensions.TruncateHtml(data.Incident.DESCRIPTION, 100, "...");
				lbl.Text = lbl.Text.Replace("<a href", "<a target=\"blank\" href");

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblReportedBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}

				lbl = (Label)e.Item.FindControl("lblCategory");
				lbl.Text = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, (decimal)EHSQuestionId.InspectionCategory) + "<br/>" +
					EHSIncidentMgr.SelectIncidentAnswer(data.Incident, (decimal)EHSQuestionId.RecommendationType);

				lbl = (Label)e.Item.FindControl("lblIncStatus");
				try
				{
					if (data.Status == "U")
					{
						lbl.Text = "Audited " + SQMBasePage.FormatDate((DateTime)data.Incident.CLOSE_DATE_DATA_COMPLETE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
					}
					else if (data.Status == "F")
					{
						lbl.Text = "Awaiting Funding " + SQMBasePage.FormatDate((DateTime)data.Incident.CLOSE_DATE_DATA_COMPLETE, "d", false) + "<br/>(" + data.DaysToClose.ToString() + ")";
					}
					else if (data.Status == "C")
					{
						lbl.Text = "Closed  " + SQMBasePage.FormatDate((DateTime)data.Incident.CLOSE_DATE, "d", false) + "<br/><strong>Not Audited</strong>";
					}
					else
					{
						lbl.Text = WebSiteCommon.GetXlatValue("incidentStatus", data.Status) + "<br/>(" + data.DaysOpen + ")";
					}
				}
				catch
				{
					;
				}

				LinkButton lbEditReport = (LinkButton)e.Item.FindControl("lbEditReport");
				lbEditReport.Visible = true;

				try
				{
					lbl = (Label)e.Item.FindControl("lblIncidentDT");
					lbl.Text = SQMBasePage.FormatDate(data.Incident.INCIDENT_DT, "d", false);
					if ((val = data.EntryList.Where(l => l.INCIDENT_QUESTION_ID == 80).Select(l => l.ANSWER_VALUE).FirstOrDefault()) != null && !string.IsNullOrEmpty(val))
					{
						val = val.Substring(0, val.IndexOf(' '));
						DateTime parseDate;
						if (DateTime.TryParse(val, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
							lbl.Text = parseDate.ToShortDateString();
					}
				}
				catch {}
				try {
					if ((val = data.EntryList.Where(l => l.INCIDENT_QUESTION_ID == 92).Select(l => l.ANSWER_VALUE).FirstOrDefault()) != null && !string.IsNullOrEmpty(val))
					{
						val = val.Substring(0, val.IndexOf(' '));
						DateTime parseDate;
						if (DateTime.TryParse(val, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeLocal, out parseDate))
						{
							lbl = (Label)e.Item.FindControl("lblDueDT");
							lbl.Text = parseDate.ToShortDateString();
						}
					}
				}
				catch  { ; }

				if (data.RespPerson != null)
				{
					lbl = (Label)e.Item.FindControl("lblAssignedTo");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.RespPerson);
				}

				if (rgPreventativeList.MasterTableView.GetColumn("Attach").Visible  &&  data.AttachList != null)
				{
					lbl = (Label)e.Item.FindControl("lblAttach");
					Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
					lbl.Parent.Controls.AddAt(lbl.Parent.Controls.IndexOf(lbl), attch);
					attch.BindListAttachment(data.AttachList, "1", 1);
				}
			}
		}

		protected void lbEditReport_Click(object sender, EventArgs e)
		{

			//RadButton rbNew = (RadButton)Parent.FindControl("rbNew");
			//rbNew.Visible = false;
			

			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = Convert.ToDecimal((sender as LinkButton).CommandArgument);
		}

		protected void rgIncidentList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}

		protected void rgPreventativeList_SortCommand(object sender, GridSortCommandEventArgs e)
		{	
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}

		protected void rgIncidentList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
			RadButton rbNew = (RadButton)this.Parent.FindControl("rbNew");
			rbNew.Visible = true;
		}
		protected void rgIncidentList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}


		public void BindIncidentActionList(object theList, string appContext)
		{
			pnlIncidentListRepeater.Visible = false;
			pnlIncidentActionList.Visible = true;
			staticAppContext = appContext;

			rgIncidentActionList.DataSource = theList;
			rgIncidentActionList.DataBind();
		}

		protected void rgIncidentActionList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				EHSIncidentData data = (EHSIncidentData)e.Item.DataItem;

				try
				{
					lbl = (Label)e.Item.FindControl("lblIncidentId");
					lbl.Text = WebSiteCommon.FormatID(data.Incident.INCIDENT_ID, 6);


					if (data.Incident.DESCRIPTION.Length > 200)
					{
						lbl = (Label)e.Item.FindControl("lblDescription");
						lbl.Text = data.Incident.DESCRIPTION.Substring(0, 200) + "...";
					}

					lbl = (Label)e.Item.FindControl("lblDueDT");
					INCIDENT_ANSWER entry = data.Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == 65).FirstOrDefault();  // due date
					if (entry != null && !string.IsNullOrEmpty(entry.ANSWER_VALUE))
					{
						lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(entry.ANSWER_VALUE), "d", false);
						entry = data.Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == 64).FirstOrDefault(); // responsible person
						if (entry != null && !string.IsNullOrEmpty(entry.ANSWER_VALUE))
						{
							lbl = (Label)e.Item.FindControl("lblResponsible");
							lbl.Text = entry.ANSWER_VALUE;
						}
					}

					RadGrid gv = (RadGrid)e.Item.FindControl("rgIncidentActions");
					List<INCIDENT_ANSWER> incidentActionList = new List<INCIDENT_ANSWER>();
					incidentActionList.AddRange(data.Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == 24 || l.INCIDENT_QUESTION_ID == 27).ToList());
					if (incidentActionList.Count > 0)
					{
						baseRowIndex = e.Item.RowIndex;
						gv.DataSource = incidentActionList;
						gv.DataBind();
						gv.Visible = true;
					}

					LinkButton lb8d = (LinkButton)e.Item.FindControl("lb8d");
					if (lb8d != null  &&  UserContext.RoleAccess() <= AccessMode.Partner)
						lb8d.Visible = false;
				}

				catch
				{
				}

			}
		}

		protected void rgIncidentActions_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				if ((baseRowIndex % 4) == 0)
					e.Item.Cells[0].BackColor = e.Item.Cells[1].BackColor = e.Item.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#ededed");
			}
		}

		protected void rgIncidentActionList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}
		protected void rgIncidentActionList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
		}
		protected void rgIncidentActionList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayIncidents";
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

		public string GetFullIncidentName (string typeCode)
		{
			return WebSiteCommon.GetXlatValue("incidentType", typeCode);
		}

		public string EvaluateStatus(DateTime? closeDate)
		{
			return (closeDate == null) ?
				"<span style=\"color: #A3461F;\">Active</span>" :
				"<span style=\"color: #008800;\">Closed " + ((DateTime)closeDate).ToShortDateString() + "</span>";
		}

		protected void rgCaseList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayCases";
		}

		protected void rgCaseList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayCases";
		}

		protected void rgCaseList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayCases";
		}

		#endregion

	}
}
