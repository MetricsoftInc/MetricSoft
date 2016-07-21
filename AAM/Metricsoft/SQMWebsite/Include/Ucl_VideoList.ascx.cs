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
	public partial class Ucl_VideoList : System.Web.UI.UserControl
	{
		static List<QI_OCCUR> issueList;
		static string staticAppContext;
		static int baseRowIndex;
		List<XLAT> listXLAT;


		RadPersistenceManager persistenceManager;

		public event GridItemClick OnQualityIssueClick;
		public event GridItemClick OnQualityIssueListCloseClick;
		public event GridItemClick OnProblemCaseClick;
		public event GridItemClick OnVideoClick;
		public event EditItemClick OnTaskClick;
		public event EditItemClick OnCaseTaskClick;
		public event CommandClick OnSearchClick;
		public event CommandClick OnSearchReceiptsClick;

		public bool LinksDisabled
		{
			get;
			set;
		}

		#region video
		public RadGrid VideoListEhsGrid
		{
			get { return rgVideoList; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected void lbVideo_Click(Object sender, EventArgs e)
		{
			if (OnVideoClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnVideoClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void lnkEditVideo(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] args = lnk.CommandArgument.ToString().Split('~');
			//if (args[1].Equals("C"))
			//	SessionManager.ReturnObject = "Closed";
			//else if (args[1].Equals("D"))
			//	SessionManager.ReturnObject = "DisplayOnly";
			//else
				SessionManager.ReturnObject = "Notification";
			SessionManager.ReturnRecordID = Convert.ToDecimal(args[0]);
			SessionManager.ReturnStatus = true;
		}

		protected void lnkVideoRedirect(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			SessionManager.ReturnObject = lnk.CommandArgument;
			SessionManager.ReturnStatus = true;
			Response.Redirect("/Media/Media_Videos.aspx");
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

		protected void btnVideoSearchClick(object sender, EventArgs e)
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


		#region mediavideo

		public void BindVideoListRepeater(object theList, string appContext)
		{
			pnlVideoListRepeater.Visible = true;
			staticAppContext = appContext;

			listXLAT = SQMBasePage.SelectXLATList(new string[1] { "MEDIA_VIDEO_STATUS" }, 1);
			rgVideoList.DataSource = theList;
			rgVideoList.DataBind();
		}

		protected void rgVideoList_ItemDataBound(object sender, GridItemEventArgs e)
		{

			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				MediaVideoData data = (MediaVideoData)e.Item.DataItem;

				LinkButton lnk = (LinkButton)e.Item.FindControl("lbVideoId");
				lnk.Text = WebSiteCommon.FormatID(data.Video.VIDEO_ID, 6);
				lnk.CommandArgument = data.Video.VIDEO_ID.ToString();
				//if (SessionManager.UserContext.Person.PERSON_ID == data.Person.PERSON_ID)
				//	lnk.CommandArgument = data.Video.VIDEO_ID.ToString() + "~" + data.Status;
				//else if (!data.Status.Equals("C"))
				//	lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~D";
				//else
				//	lnk.CommandArgument = data.Audit.AUDIT_ID.ToString() + "~C";

				lnk = (LinkButton)e.Item.FindControl("lbDelete");
				if (data.Video.SOURCE_TYPE == (int)TaskRecordType.Audit || data.Video.SOURCE_TYPE == (int)TaskRecordType.HealthSafetyIncident)
					lnk.Visible = false;
				else
					lnk.Visible = true;

				if (data.Video.TITLE.Length > 120)
				{
					lbl = (Label)e.Item.FindControl("lblVideoTitle");
					lbl.Text = data.Video.TITLE.Substring(0, 117) + "...";
				}

				if (data.Person != null)
				{
					lbl = (Label)e.Item.FindControl("lblVideoBy");
					lbl.Text = SQMModelMgr.FormatPersonListItem(data.Person);
				}

				lbl = (Label)e.Item.FindControl("lblVideoStatus"); // what to do with this?
				try
				{
					lbl.Text = SQMBasePage.GetXLAT(listXLAT, "MEDIA_VIDEO_STATUS", data.Video.VIDEO_STATUS).DESCRIPTION;
				} 
				catch { }
				
				//if (data.Video.VIDEO_STATUS == "C")
				//{
				//	DateTime clsDate = (DateTime)data.Audit.CLOSE_DATE_DATA_COMPLETE;
				//	lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "C") + " " + SQMBasePage.FormatDate(clsDate, "d", false);
				//}
				//else
				//{
				//	if (data.DaysToClose == 0)
				//	{
				//		DateTime tmp = ((DateTime)data.Audit.AUDIT_DT).AddDays(data.AuditType.DAYS_TO_COMPLETE);
				//		lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "X") + "<br/>(" + SQMBasePage.FormatDate(tmp, "d", false) + ")";
				//	}
				//	else if (data.Audit.PERCENT_COMPLETE > 0)
				//		lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "I") + "<br/>(" + data.DaysToClose + ")";
				//	else
				//		lbl.Text = WebSiteCommon.GetXlatValue("auditStatus", "A") + "<br/>(" + data.DaysToClose + ")";
				//}

			}
		}


		protected void lbDelete_Click(object sender, EventArgs e)
		{
			LinkButton lb = (LinkButton)sender;

			decimal videoId = 0;
			try
			{
				videoId = Convert.ToDecimal(lb.CommandArgument.ToString());
			}
			catch { }
			if (videoId > 0)
			{

				string videoname = Server.MapPath(lb.CommandName.ToString());
				int status = MediaVideoMgr.DeleteVideo(videoId, videoname);
			}
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayVideos";
			SessionManager.ReturnRecordID = 0;
		}
		//protected void lbEditReport_Click(object sender, EventArgs e)
		//{
		//	SessionManager.ReturnStatus = true;
		//	SessionManager.ReturnObject = "Report";
		//	SessionManager.ReturnRecordID = Convert.ToDecimal((sender as LinkButton).CommandArgument);
		//}

		protected void rgVideoList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayVideos";
		}

		protected void rgVideoList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayVideos";
		}
		protected void rgVideoList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayVideos";
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

		//public string GetFullAuditName(string typeCode)
		//{
		//	return WebSiteCommon.GetXlatValue("auditType", typeCode);
		//}

		public string EvaluateStatus(DateTime? closeDate)
		{
			return (closeDate == null) ?
				"<span style=\"color: #A3461F;\">Active</span>" :
				"<span style=\"color: #008800;\">Closed " + ((DateTime)closeDate).ToShortDateString() + "</span>";
		}

		#endregion

	}
}
