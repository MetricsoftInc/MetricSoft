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
			btnTaskComplete.CommandArgument = task.TASK_ID.ToString();
			btnTaskAssign.CommandArgument = task.TASK_ID.ToString();
			if (task.STATUS == ((int)TaskStatus.Complete).ToString())
			{
				btnTaskComplete.Visible = btnTaskAssign.Visible = false;
			}
			else
			{
				btnTaskComplete.Visible = btnTaskAssign.Visible = true;
			}

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
			lblTaskStatusValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == ((int)TaskMgr.CalculateTaskStatus(task)).ToString()).FirstOrDefault().DESCRIPTION;
			tbTaskComments.Text = task.COMMENTS;
		}

		protected void btnTaskComplete_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
			{
				return;
			}

			TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
			TASK_STATUS task = taskMgr.SelectTask(Convert.ToDecimal(btn.CommandArgument));
			task.COMMENTS = tbTaskComments.Text;
			task.STATUS = ((int)TaskStatus.Complete).ToString();
			task = taskMgr.SetTaskComplete(task, SessionManager.UserContext.Person.PERSON_ID);
			taskMgr.UpdateTask(task);

			if (OnTaskUpdate != null)
			{
				OnTaskUpdate("update");
			}
		}

		protected void btnTaskAssign_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
			{
				return;
			}

			TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
			TASK_STATUS task = taskMgr.SelectTask(Convert.ToDecimal(btn.CommandArgument));
			btnAssignSave.CommandArgument = task.TASK_ID.ToString();

			List<PERSON> personList = SQMModelMgr.SelectPlantPersonList(1, GetTaskLocation(task)).Where(l=> !string.IsNullOrEmpty(l.EMAIL)).OrderBy(l=> l.LAST_NAME).ToList();
			SQMBasePage.SetPersonList(ddlAssignPerson, personList, "", 0, false, "LF");
			tbAssignComment.Text = task.COMMENTS;

			string script = "function f(){OpenAssignTaskWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void btnTaskAssignUpdate_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
			{
				return;
			}

			if (ddlAssignPerson.SelectedItem != null)
			{
				TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
				TASK_STATUS task = taskMgr.SelectTask(Convert.ToDecimal(btn.CommandArgument));
				task.COMMENTS = tbAssignComment.Text;
				task.STATUS = ((int)TaskStatus.New).ToString();
				task.RESPONSIBLE_ID = Convert.ToDecimal(ddlAssignPerson.SelectedValue);
				taskMgr.UpdateTask(task);
				// send email ?
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

		private decimal GetTaskLocation(TASK_STATUS task)
		{
			decimal plantID = SessionManager.UserContext.HRLocation.Plant.PLANT_ID;

			switch ((TaskRecordType)task.RECORD_TYPE)
			{
				case TaskRecordType.HealthSafetyIncident:
				case TaskRecordType.PreventativeAction:
					INCIDENT incident = EHSIncidentMgr.SelectIncidentById(new PSsqmEntities(), task.RECORD_ID);
					if (incident != null  &&  incident.DETECT_PLANT_ID.HasValue)
						plantID = (decimal)incident.DETECT_PLANT_ID;
					break;
				case TaskRecordType.Audit:
					AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), task.RECORD_ID);
					if (audit != null  &&  audit.DETECT_PLANT_ID.HasValue)
						plantID =(decimal) audit.DETECT_PLANT_ID;
					break;
			}

			return plantID;
		}
	}
}