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

        public Repeater TaskListRepeater
        {
            get { return rptTaskList; }
        }

        public DateTime TaskScheduleSelectedDate
        {
            get { return Convert.ToDateTime(scdTaskSchedule.SelectedDate); }
        }

        #region events

        public event EditItemClick OnTaskListClick;

        protected void OnEHSIncident_Click(string cmd)
        {
            SessionManager.ReturnObject = cmd;
            SessionManager.ReturnStatus = true;
            Response.Redirect("/EHS/EHS_Incidents.aspx");
        }

        #endregion

        public event EditItemClick OnTaskClick;

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
                    case TaskRecordType.QualityIssue:
                        QI_OCCUR qiOccur = new QI_OCCUR();
                        qiOccur.INCIDENT_ID = Convert.ToDecimal(args[1]);
                        SessionManager.ReturnObject = qiOccur;
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/Quality/Quality_Issue.aspx?c=" + args[2]);
                        break;
                    case TaskRecordType.ProblemCase:
                        try
                        {
                            //string[] datas = args[1].Split('~');
                            TASK_STATUS task = new TASK_STATUS();
                            task.RECORD_ID = Convert.ToDecimal(args[1]);
                            task.TASK_STEP = args[2];
                            SessionManager.ReturnObject = task;
                            SessionManager.ReturnStatus = true;
                            SessionManager.ReturnPath = Request.Url.PathAndQuery;
                            Response.Redirect("/Problem/Problem_Case.aspx");
                        }
                        catch (Exception ex)
                        {
                            //SQMLogger.LogException(ex);
                        }
                        break;
                    case TaskRecordType.ProfileInput:
                    case TaskRecordType.ProfileInputApproval:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/EHS/EHS_MetricInput.aspx");
                        break;
                    case TaskRecordType.ProfileInputFinalize:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/EHS/EHS_Console.aspx");
                        break;
                        break;
                    case TaskRecordType.HealthSafetyIncident:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/EHS/EHS_Incidents.aspx");
                        break;
                    case TaskRecordType.PreventativeAction:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/EHS/EHS_Incidents.aspx?mode=prevent");
                        break;
					case TaskRecordType.Audit:
						SessionManager.ReturnObject = args[1];
						SessionManager.ReturnStatus = true;
						SessionManager.ReturnPath = Request.Url.PathAndQuery;
						Response.Redirect("/EHS/EHS_Audits.aspx");
						break;
                    case TaskRecordType.CurrencyInput:
                        SessionManager.ReturnObject = args[1];
                        SessionManager.ReturnStatus = true;
                        SessionManager.ReturnPath = Request.Url.PathAndQuery;
                        Response.Redirect("/Admin/Administrate_CurrencyInput.aspx");
                        break;
                    default:
                        break;
                }
            }
        }


        public void BindTaskList(object taskList)
        {
            pnlTaskList.Visible = true;
            pnlTaskSchedule.Visible = false;
            rptTaskList.DataSource = taskList;
            rptTaskList.DataBind();
            SetRepeaterDisplay(rptTaskList, lblTaskListEmpty, divGVTaskListRepeater, 12, rptTaskList.Items.Count*2, "scrollArea");
        }

        public void rptTaskList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    TaskItem item = (TaskItem)e.Item.DataItem;
                    Ucl_IncidentList ucl;
                    Ucl_EHSList ecl;
                    LinkButton lnk;
                    Label lbl;
                    Image img;
                    //TaskStatus status;
                    TaskRecordType taskType = (TaskRecordType)item.RecordType;
                    switch (taskType)
                    {
                        case TaskRecordType.QualityIssue:    // quality issue
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            if (!string.IsNullOrEmpty(item.Task.DESCRIPTION))
                                lnk.Text = item.Title + "<br>(" + item.Task.DESCRIPTION + ")";
                            else
                                lnk.Text = item.Title;
                          
                            INCIDENT incident = (INCIDENT)item.Detail;
                            QI_OCCUR qiIssue = (QI_OCCUR)item.Reference;
                            lnk.CommandArgument = item.RecordKey;
                            lbl = (Label)e.Item.FindControl("lblDueDate");
                            lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);
                            img = (Image)e.Item.FindControl("imgTaskStatus");
                            img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                            img.ToolTip = item.Taskstatus.ToString();
                            QualityIncidentData issue = new QualityIncidentData();
                            issue.Incident = incident;
                            issue.QIIssue = qiIssue;
                            issue.Plant = item.Plant;
                            issue.PlantResponsible = item.PlantResponsible;
                            issue.Person = item.Person;
                            issue.Part = item.Part;
                            List<QualityIncidentData> incidentList = new List<QualityIncidentData>();
                            ucl = (Ucl_IncidentList)e.Item.FindControl("uclIssueList");
                            ucl.LinksDisabled = true;
                            ucl.BindQualityIncidentHeader(issue, true);
                            break;

                        case TaskRecordType.ProblemCase:    // 8D problem case
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            lnk.Text = item.Title;
                            PROB_CASE probCase = (PROB_CASE)item.Detail;
                            lnk.CommandArgument = item.RecordKey;
                            lbl = (Label)e.Item.FindControl("lblDueDate");
                            lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);
                            img = (Image)e.Item.FindControl("imgTaskStatus");
                            img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                            img.ToolTip = item.Taskstatus.ToString();
                            ProblemCase theCase = new ProblemCase();
                            theCase.ProbCase = probCase;
                            theCase.SetAliasID();
                            ucl = (Ucl_IncidentList)e.Item.FindControl("uclIssueList");
                            ucl.LinksDisabled = true;
                            ucl.BindProblemCaseHeader(theCase, item);
                            break;

                        case TaskRecordType.ProfileInput:    // Profile inputs
                        case TaskRecordType.ProfileInputApproval:   // approval
                        case TaskRecordType.ProfileInputFinalize:   // finalize
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            lnk.Text = item.Title;
                            lnk.CommandArgument = item.RecordKey;
                            lbl = (Label)e.Item.FindControl("lblDueDate");
                            lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);
                            img = (Image)e.Item.FindControl("imgTaskStatus");
                            img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                            img.ToolTip = item.Taskstatus.ToString();
                            ecl = (Ucl_EHSList)e.Item.FindControl("uclEHSPeriod");
                            ecl.BindProfilePeriodHdr(item);
                            break;

                        case TaskRecordType.HealthSafetyIncident:    // Health & safety incidents
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            lnk.Text = item.Title;
                            lnk.CommandArgument = item.RecordKey;
                            lbl = (Label)e.Item.FindControl("lblDueDate");
                            lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);
                            img = (Image)e.Item.FindControl("imgTaskStatus");
                            img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                            img.ToolTip = item.Taskstatus.ToString();
                            INCIDENT EHSIncident = (INCIDENT)item.Detail;
                            ucl = (Ucl_IncidentList)e.Item.FindControl("uclIssueList");
                            ucl.LinksDisabled = true;
                            ucl.BindIncidentListHeader(EHSIncident, item);
                            break;

                        case TaskRecordType.PreventativeAction:   // preventative action
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            lnk.Text = item.Title;
                            lnk.CommandArgument = item.RecordKey;
                            lbl = (Label)e.Item.FindControl("lblDueDate");
                            lbl.Text = SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false);
                            img = (Image)e.Item.FindControl("imgTaskStatus");
                            img.ImageUrl = TaskMgr.TaskStatusImage(item.Taskstatus);
                            img.ToolTip = item.Taskstatus.ToString();
                            INCIDENT EHSAction = (INCIDENT)item.Detail;
                            ucl = (Ucl_IncidentList)e.Item.FindControl("uclIssueList");
                            ucl.LinksDisabled = true;
                            ucl.BindIncidentListHeader(EHSAction, item);
                            break;
                        default:
                            break;
                    }

                    if (item.Taskstatus == TaskStatus.EscalationLevel1 || item.Taskstatus == TaskStatus.EscalationLevel2)
                    {
                        if (UserContext.RoleAccess() < AccessMode.Admin)
                        {
                            lnk = (LinkButton)e.Item.FindControl("lnkTask");
                            lbl = (Label)e.Item.FindControl("lblTask");
                            lbl.Text = lnk.Text;
                            lnk.Visible = false;
                            lbl.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
       
        protected void rptTaskList_OnItemCreate(object sender, RepeaterItemEventArgs e)
        {
            ;
        }
        #endregion

        #region scehedule
        public void BindTaskSchedule(List<TaskItem> taskList, DateTime selectedDate, bool enableItemLinks)
        {
            pnlTaskList.Visible = false;
            pnlTaskSchedule.Visible = true;
            hfScheduleScope.Value = enableItemLinks.ToString().ToLower();

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
            e.Appointment.AllowDelete = false;
            e.Appointment.AllowEdit = true;
            e.Appointment.Description = StringHtmlExtensions.TruncateHtml(e.Appointment.Description, 1000, "...");
            e.Appointment.Description = WebSiteCommon.StripHTML(e.Appointment.Description);

            if (e.Appointment.End.Date > e.Appointment.Start.Date && e.Appointment.Start.Date < DateTime.Now.Date)
            {
                e.Appointment.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffe6e6");  // light yellow  #ffd413  #ffc013
            }
            e.Appointment.End = e.Appointment.Start.AddHours(2);
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
                    Control div = (Control)e.Container.FindControl("divInactive");
                    div.Visible = true;
                    LinkButton lnk = (LinkButton)e.Container.FindControl("lnkScheduleItem");
                    lnk.Visible = false; 
                    lnk = (LinkButton)e.Container.FindControl("lnkScheduleItem2");
                    lnk.Visible = false;
                }
            }
            catch { }
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
                    TaskItem item = (TaskItem)e.Item.DataItem;
                    item.Description = StringHtmlExtensions.TruncateHtml(item.Description, 1000, "...");
                    item.Description = WebSiteCommon.StripHTML(item.Description);
                }
                catch
                { }
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