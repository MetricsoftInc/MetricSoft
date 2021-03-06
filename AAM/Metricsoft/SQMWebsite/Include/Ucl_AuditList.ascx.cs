﻿using System;
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

		public string allowReAudits;

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
		//public Panel AuditListPanel
		//{
		//	get { return pnlAuditList; }
		//}
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

		//protected void btnReceiptsSearchClick(object sender, EventArgs e)
		//{
		//	if (OnSearchReceiptsClick != null)
		//	{
		//		Button btn = (Button)sender;
		//		OnSearchReceiptsClick(btn.CommandArgument);
		//	}
		//}

		#endregion


		#region ehsaudit

		public void BindAuditListRepeater(object theList, string appContext)
		{
			pnlAuditListRepeater.Visible = true;
			staticAppContext = appContext;

			// first check settings to see if the company allows ReAudits
			List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("AUDIT", ""); // ABW 20140805
			allowReAudits = "";
			try
			{
				allowReAudits = sets.Find(x => x.SETTING_CD == "AllowReAudit").VALUE;
			}
			catch { }


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

				LinkButton lnk = (LinkButton)e.Item.FindControl("lbAuditId");
				lnk.Text = WebSiteCommon.FormatID(data.Audit.AUDIT_ID, 6);

				/*
				if (data.Audit.DESCRIPTION.Length > 120)
				{
					lbl = (Label)e.Item.FindControl("lblDescription");
					lbl.Text = data.Audit.DESCRIPTION.Substring(0, 117) + "...";
				}
				*/
				//lbl = (Label)e.Item.FindControl("lblDescription");
				//lbl.Text = HttpUtility.HtmlEncode(lbl.Text);

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblAuditBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}

				lbl = (Label)e.Item.FindControl("lblAuditStatus");

				List<XLAT> TaskXLATList = SQMBasePage.SelectXLATList(new string[1] { "AUDIT_STATUS" });

				if (data.Audit.CURRENT_STATUS == "C")
				{
					// TODO: This throws a null reference error when the database field is NULL (which is valid)
					DateTime clsDate = (DateTime)data.Audit.CLOSE_DATE_DATA_COMPLETE;
					lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_STATUS" && l.XLAT_CODE == "C").FirstOrDefault().DESCRIPTION + " " + SQMBasePage.FormatDate(clsDate, "d", false);

					// TODO: Possible fix
					//DateTime? clsDate = data.Audit.CLOSE_DATE;
					//if (data.Audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
					//{
					//	clsDate = data.Audit.CLOSE_DATE_DATA_COMPLETE;
					//}
					
					//if (clsDate.HasValue)
					//{
					//	lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_STATUS" && l.XLAT_CODE == "C").FirstOrDefault().DESCRIPTION + " " + SQMBasePage.FormatDate(clsDate.Value, "d", false);
					//}
				}
				else
				{
					if (data.DaysToClose == 0)
					{
						DateTime tmp = ((DateTime)data.Audit.AUDIT_DT).AddDays(data.AuditType.DAYS_TO_COMPLETE);
						lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_STATUS" && l.XLAT_CODE == "X").FirstOrDefault().DESCRIPTION + "<br/>(" + SQMBasePage.FormatDate(tmp, "d", false) + ")";
					}
					else if (data.Audit.PERCENT_COMPLETE > 0)
						lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_STATUS" && l.XLAT_CODE == "I").FirstOrDefault().DESCRIPTION + "<br/>(" + data.DaysToClose + ")";
					else
						lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "AUDIT_STATUS" && l.XLAT_CODE == "A").FirstOrDefault().DESCRIPTION + "<br/>(" + data.DaysToClose + ")";
				}

				lnk = (LinkButton)e.Item.FindControl("lbAuditId");
				LinkButton lnkReAudit = (LinkButton)e.Item.FindControl("lbReAudit");
				Label lblAuditingId = (Label)e.Item.FindControl("lblAuditingId");
				HiddenField hdnId = (HiddenField)e.Item.FindControl("hdnAuditingId");

				if (allowReAudits.ToUpper().Equals("Y"))
				{
					if (SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit))
					{
						if (hdnId.Value.ToString().Trim().Equals("0") || hdnId.Value.ToString().Trim().Equals(""))
						{
							lblAuditingId.Visible = false;
						}
						else
						{
							lnkReAudit.Visible = false;
							lblAuditingId.Text = Resources.LocalizedText.ReAuditing + " " + hdnId.Value.ToString();
						}
					}
					else
					{
						lnkReAudit.Visible = false;
						lblAuditingId.Visible = false;
					}
				}
				else
				{
					lnkReAudit.Visible = false;
					lblAuditingId.Visible = false;
				}

				if (SessionManager.UserContext.Person.PERSON_ID == data.Person.PERSON_ID)
				{
					lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~" + data.Status;
					lnkReAudit.Visible = false;
					//lblAuditingId.Visible = false;
				}
				else if (!data.Status.Equals("C"))
					lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~D";
				else
					lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~C";

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

		protected void lbReAudit_Click(object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = "ReAudit";
			SessionManager.ReturnRecordID = Convert.ToDecimal(lnk.CommandArgument.ToString());
			SessionManager.ReturnStatus = true;
		}
	}
}
