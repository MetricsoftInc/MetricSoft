using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SQM.Shared;

namespace SQM.Website
{
    public partial class Ucl_TaskStatus : System.Web.UI.UserControl
    {
		public event GridActionCommand OnTaskUpdate;

		private List<XLAT> TaskXLATList
		{
			get { return ViewState["TaskXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["TaskXLATList"]; }
			set { ViewState["TaskXLATList"] = value; }
		}

		public void BindTaskUpdate(TASK_STATUS task, string context)
		{
			PSsqmEntities ctx = new PSsqmEntities();
			if (TaskXLATList == null || TaskXLATList.Count == 0)
				TaskXLATList = SQMBasePage.SelectXLATList(new string[4] { "TASK_STATUS", "RECORD_TYPE", "INCIDENT_STATUS", "NOTIFY_SCOPE_TASK" });

			pnlUpdateTask.Visible = true;
			btnTaskUpdate.CommandArgument = task.TASK_ID.ToString();

			switch ((TaskRecordType)task.RECORD_TYPE)
			{
				case TaskRecordType.HealthSafetyIncident:
					lblTaskTypeValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == task.RECORD_TYPE.ToString()).FirstOrDefault().DESCRIPTION;
					if (task.TASK_STEP == "350")
					{	// corrective action task
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}
					else
					{
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}
					break;
				case TaskRecordType.Audit:
					//AUDIT audit = EHSAuditMgr.SelectAuditById(ctx, task.RECORD_ID);
					lblTaskTypeValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == task.RECORD_TYPE.ToString()).FirstOrDefault().DESCRIPTION;
					if ((task.RECORD_SUBID.HasValue && task.RECORD_SUBID > 0)  ||  task.TASK_STEP == "350")
					{	// action required if subid references a specific audit question
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}
					else
					{
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == "300").FirstOrDefault().DESCRIPTION);
					}
					break;
				default:
					return;
					break;
			}

			lblTaskDescriptionValue.Text = task.DESCRIPTION;  // command of what to do
			lblTaskDetailValue.Text = task.DETAIL;				// incident description or audit question 
			rdpTaskDueDT.SelectedDate = (DateTime)task.DUE_DT;
			lblTaskStatusValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == ((int)TaskMgr.CalculateTaskStatus(task)).ToString()).FirstOrDefault().DESCRIPTION; ;

			tbTaskComments.Text = task.COMMENTS;
		}

		protected void btnTaskUpdate_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
			{
				return;
			}

			TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
			TASK_STATUS task = taskMgr.SelectTask(Convert.ToDecimal(btn.CommandArgument));
			task.COMMENTS = tbTaskComments.Text;

			if (ddlTaskStatus.SelectedValue == ((int)TaskStatus.Complete).ToString())
			{
				task.STATUS = ((int)TaskStatus.Complete).ToString();
				task = taskMgr.SetTaskComplete(task, SessionManager.UserContext.Person.PERSON_ID);
				taskMgr.UpdateTask(task);
			}
			else
			{
				task = taskMgr.SetTaskOpen(task, task.DUE_DT, null);
				taskMgr.UpdateTask(task);
			}

			if (OnTaskUpdate != null)
			{
				OnTaskUpdate("update");
			}
		}

		protected void btnTaskCancel_Click(object sender, EventArgs e)
		{
			if (OnTaskUpdate != null)
			{
				OnTaskUpdate("cancel");
			}
		}
	}
}