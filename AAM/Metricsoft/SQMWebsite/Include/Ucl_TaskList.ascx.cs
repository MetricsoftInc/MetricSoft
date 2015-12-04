using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_TaskList : System.Web.UI.UserControl
    {
		public event EditItemClick OnTaskListClick;

		private List<XLAT> TaskXLATList
		{
			get { return ViewState["TaskXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["TaskXLATList"]; }
			set { ViewState["TaskXLATList"] = value; }
		}

        public DateTime TaskScheduleSelectedDate
        {
            get { return Convert.ToDateTime(scdTaskSchedule.SelectedDate); }
        }

        #region events

        protected void OnEHSIncident_Click(string cmd)
        {
            SessionManager.ReturnObject = cmd;
            SessionManager.ReturnStatus = true;
            Response.Redirect("/EHS/EHS_Incidents.aspx");
        }

        #endregion

        public event EditItemClick OnTaskClick;
		public event GridItemClick OnTaskListItemClick;
		public event GridActionCommand OnTaskListCommand;

        #region task
        protected void lnkTask_Click(object sender, EventArgs e)
        {
            string cmd = "";
            if (sender is ImageButton)
            {
                ImageButton btn = (ImageButton)sender;
                cmd = btn.CommandArgument.ToString().Trim();
            }
            else 
            {
                LinkButton lnk = (LinkButton)sender;
                cmd = lnk.CommandArgument.ToString().Trim();
            }

            if (OnTaskListClick != null)
            {
                OnTaskListClick(cmd);
            }
            else
            {
                string[] args = cmd.Split('|');
                TaskRecordType taskType = (TaskRecordType)Enum.Parse(typeof(TaskRecordType), args[0]);

                switch (taskType)
                {
                    case TaskRecordType.ProfileInput:
                    case TaskRecordType.ProfileInputApproval:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
						SessionManager.ReturnPath = Request.Path.ToString();
                        Response.Redirect("/EHS/EHS_MetricInput.aspx");
                        break;
                    case TaskRecordType.ProfileInputFinalize:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
						SessionManager.ReturnPath = Request.Path.ToString();
                        Response.Redirect("/EHS/EHS_Console.aspx");
                        break;
                        break;
                    case TaskRecordType.HealthSafetyIncident:
						if (args.Length == 4 && args[3] == ((int)SysPriv.action).ToString())  // incident action
						{
							TASK_STATUS task = new TASK_STATUS();
							task.RECORD_TYPE = (int)TaskRecordType.Audit;
							task.TASK_ID = Convert.ToDecimal(args[2]);
							task.TASK_STEP = args[3];
							SessionManager.ReturnObject = task;
							SessionManager.ReturnStatus = true;
							SessionManager.ReturnPath = Request.Path.ToString();
							Response.Redirect("/Home/TaskAction.aspx");
						}
						else
						{
							SessionManager.ReturnObject = args[1];
							SessionManager.ReturnStatus = true;
							SessionManager.ReturnPath = Request.Path.ToString();
							Response.Redirect("/EHS/EHS_Incidents.aspx");
						}
                        break;
                    case TaskRecordType.PreventativeAction:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
						SessionManager.ReturnPath = Request.Path.ToString();
                        Response.Redirect("/EHS/EHS_Incidents.aspx?mode=prevent");
                        break;
					case TaskRecordType.Audit:
						if (args.Length == 4 && args[3] == ((int)SysPriv.action).ToString())  // audit action
						{
							TASK_STATUS task = new TASK_STATUS();
							task.RECORD_TYPE = (int)TaskRecordType.Audit;
							task.TASK_ID = Convert.ToDecimal(args[2]);
							task.TASK_STEP = args[3];
							SessionManager.ReturnObject = task;
							SessionManager.ReturnStatus = true;
							SessionManager.ReturnPath = Request.Path.ToString();
							Response.Redirect("/Home/TaskAction.aspx");
						}
						else
						{
							SessionManager.ReturnObject = args[1];
							SessionManager.ReturnStatus = true;
							SessionManager.ReturnPath = Request.Path.ToString();
							Response.Redirect("/EHS/EHS_Audits.aspx");
						}
						break;
                    case TaskRecordType.CurrencyInput:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
						SessionManager.ReturnPath = Request.Path.ToString();
                        Response.Redirect("/Admin/Administrate_CurrencyInput.aspx");
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region scehedule
        public void BindTaskSchedule(List<TaskItem> taskList, DateTime selectedDate, bool enableItemLinks)
        {
            pnlTaskSchedule.Visible = true;
            hfScheduleScope.Value = enableItemLinks.ToString().ToLower();

			string uicult = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
			string language = (!string.IsNullOrEmpty(uicult)) ? uicult.Substring(0, 2) : "en";
			if (language == "th")
				scdTaskSchedule.Culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();

            scdTaskSchedule.SelectedDate = selectedDate;
  
            foreach (TaskItem taskItem in taskList)
            {
                taskItem.StartDate = Convert.ToDateTime(taskItem.Task.DUE_DT).AddHours(9);  // want task to display at 9 am
                if (taskItem.Task.STATUS != ((int)TaskStatus.Complete).ToString() && taskItem.Task.STATUS != ((int)TaskStatus.Pending).ToString())
                {
                    taskItem.EndDate = taskItem.StartDate;
                }
                else
                {
                    taskItem.EndDate = taskItem.StartDate.AddDays(1);
                }
            }

            scdTaskSchedule.DataSource = taskList;
            scdTaskSchedule.DataBind();
        }

        protected void scdTaskSchedule_OnDataBound(object sender, Telerik.Web.UI.SchedulerEventArgs e)
        {
			try
			{
				e.Appointment.AllowDelete = false;
				e.Appointment.AllowEdit = true;
				e.Appointment.Description = StringHtmlExtensions.TruncateHtml(e.Appointment.Description, 1000, "...");
				e.Appointment.Description = WebSiteCommon.StripHTML(e.Appointment.Description);

				if (e.Appointment.End.Date >= e.Appointment.Start.Date && e.Appointment.Start.Date < DateTime.Now.Date)
				{
					// past due active link
					e.Appointment.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffe6e6");  // light pink
				}
				else if (string.IsNullOrEmpty(e.Appointment.ID.ToString()))
				{
					// inactive link
					e.Appointment.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0FFFF");   // azure
				}
				else
				{
					// active link
					e.Appointment.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFE0");   // yellow  
				}

				e.Appointment.End = e.Appointment.Start.AddHours(2);
			}
			catch
			{
				;
			}
        }

        protected void scdTaskSchedule_OnCreated(object sender, AppointmentCreatedEventArgs e)
        {
            try
            {
                if (SessionManager.IsUserAgentType("ipad,iphone"))
                {
                    ImageButton btn = (ImageButton)e.Container.FindControl("btnTaskDetails");
                    btn.Visible = true;
                }

                if (hfScheduleScope.Value == false.ToString().ToLower()  || string.IsNullOrEmpty(e.Appointment.ID.ToString()))       // only enable task link if schedule scope is by USER  (i.e. disable if PLANT view)
					{
						//Control div = (Control)e.Container.FindControl("divInactive");
						//div.Visible = true;
						LinkButton lnk = (LinkButton)e.Container.FindControl("lnkScheduleItem");
						lnk.Enabled = false;
						//lnk.Visible = false; 
						lnk = (LinkButton)e.Container.FindControl("lnkScheduleItem2");
						//lnk.Visible = false;
						lnk.Enabled = false;
					}
            }
            catch {
				;
			}
        }

        // todo:  rename and make generic for use by both calendar and taskstrip
        protected void TaskItem_Click(object sender, EventArgs e)
        {
            lnkTask_Click(sender, e);
        }


        public void BindTaskStrip(List<TaskItem> taskList)
        {
            pnlTaskStrip.Visible = true;
            rptTaskStrip.DataSource = taskList;
            rptTaskStrip.DataBind();

            lblTaskStripCount.Text = taskList.Count.ToString();
            // SetRepeaterDisplay(rptTaskStrip, lblTaskStripEmpty, divTaskStripRepeater, 12, rptTaskStrip.Items.Count * 2, "scrollArea");
        }
        public void rptTaskStrip_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    TaskItem item = (TaskItem)e.Item.DataItem;

                    Label lbl = (Label)e.Item.FindControl("lblDueDate");
                    lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);

                    if (item.Taskstatus == TaskStatus.EscalationLevel1 || item.Taskstatus == TaskStatus.EscalationLevel2)
                    {
                        ImageButton img = (ImageButton)e.Item.FindControl("imgTaskStatus");
                        img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                        img.ToolTip = item.Taskstatus.ToString();
                        img.Visible = true;
                    }

                    if (SessionManager.IsUserAgentType("ipad,iphone"))
                    {
                        ImageButton btn = (ImageButton)e.Item.FindControl("btnTaskDetails");
                        btn.Visible = true;
                    }
                }
                catch
                {

                }
            }
        }

        protected void rptTaskStrip_OnItemCreate(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
					if (e.Item.DataItem != null)
					{
						TaskItem item = (TaskItem)e.Item.DataItem;
						if (item.Person != null && (item.NotifyType == TaskNotification.Escalation || item.NotifyType == TaskNotification.Delegate))
						{
							item.Description += " (" + SQMModelMgr.FormatPersonListItem(item.Person) + ")";
						}
						item.Description = StringHtmlExtensions.TruncateHtml(item.Description, 1000, "...");
						item.Description = WebSiteCommon.StripHTML(item.Description);
					}
                }
                catch
                { }
            }
        }
        #endregion

		#region tasklist

		public void BindTaskList(List<TASK_STATUS> taskList, string context)
		{
			if (TaskXLATList == null || TaskXLATList.Count == 0)
				TaskXLATList = SQMBasePage.SelectXLATList(new string[4] { "TASK_STATUS", "RECORD_TYPE", "INCIDENT_STATUS", "NOTIFY_SCOPE_TASK" });

			pnlTaskSchedule.Visible = pnlTaskStrip.Visible = false;
			pnlTaskList.Visible = true;

			rgTaskList.DataSource = taskList;
			rgTaskList.DataBind();

			if (taskList.Count > 0)
			{
				rgTaskList.Visible = true;
				lblTaskListEmpty.Visible = false;
			}
			else
			{
				rgTaskList.Visible = false;
				lblTaskListEmpty.Visible = true;
			}
		}

		protected void rgTaskList_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				HiddenField hf;
				Label lbl;

				try
				{
					TASK_STATUS task = (TASK_STATUS)e.Item.DataItem;

					LinkButton lnk = (LinkButton)e.Item.FindControl("lbTaskId");
					lnk.Text = WebSiteCommon.FormatID(task.TASK_ID, 6);

					if (task.DESCRIPTION.Length > 120)
					{
						lbl = (Label)e.Item.FindControl("lblDescription");
						lbl.Text = task.DESCRIPTION.Substring(0, 117) + "...";
					}

					lbl = (Label)e.Item.FindControl("lblTaskType");

					lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == task.RECORD_TYPE.ToString()).FirstOrDefault().DESCRIPTION;
					if (task.TASK_STEP == "350")
					{
						if (task.RECORD_TYPE == (int)TaskRecordType.Audit && task.RECORD_SUBID > 0)
						{
							lbl.Text += ("&nbsp;Exception");
						}
						lbl.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}

					lbl = (Label)e.Item.FindControl("lblCreateDT");
					if (task.CREATE_DT.HasValue)
					{
						lbl.Text = Convert.ToDateTime(task.CREATE_DT).ToShortDateString();
					}
					lbl = (Label)e.Item.FindControl("lblDueDT");
					if (task.DUE_DT.HasValue)
					{
						lbl.Text = Convert.ToDateTime(task.DUE_DT).ToShortDateString();
					}

					lbl = (Label)e.Item.FindControl("lblStatus");
					lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == ((int)TaskMgr.CalculateTaskStatus(task)).ToString()).FirstOrDefault().DESCRIPTION_SHORT;
					//lbl.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == task.STATUS).FirstOrDefault().DESCRIPTION_SHORT;
				}
				catch
				{
				}
			}
		}

		protected void lbTaskListItem_Click(Object sender, EventArgs e)
		{
			if (OnTaskListItemClick != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnTaskListItemClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
			}
		}

		protected void rgTaskList_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			if (OnTaskListCommand != null)
			{
				OnTaskListCommand("sort");
			}
		}
		protected void rgTaskList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			if (OnTaskListCommand != null)
			{
				OnTaskListCommand("index");
			}
		}
		protected void rgTaskList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			if (OnTaskListCommand != null)
			{
				OnTaskListCommand("size");
			}
		}
		#endregion

		#region common
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
        #endregion
    }
}