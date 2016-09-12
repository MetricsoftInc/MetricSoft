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
		public event GridActionCommand2 OnTaskAdd;
		public event EditItemClick OnTaskListClick;


		private List<XLAT> TaskXLATList
		{
			get { return ViewState["TaskXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["TaskXLATList"]; }
			set { ViewState["TaskXLATList"] = value; }
		}

		private TASK_STATUS CurrentTask
		{
			get { return ViewState["CurrentTask"] == null ? null : (TASK_STATUS)ViewState["CurrentTask"]; }
			set { ViewState["CurrentTask"] = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclAttachWin.AttachmentEvent += OnAttachmentsUpdate;
		}

		public void BindTaskUpdate(TASK_STATUS task, string context)
		{
			PSsqmEntities ctx = new PSsqmEntities();
			if (TaskXLATList == null || TaskXLATList.Count == 0)
				TaskXLATList = SQMBasePage.SelectXLATList(new string[5] { "TASK_STATUS", "RECORD_TYPE", "INCIDENT_STATUS", "NOTIFY_SCOPE_TASK", "ACTION_CATEGORY" });

			CurrentTask = task;

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
				case TaskRecordType.PreventativeAction:
					lblTaskTypeValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == task.RECORD_TYPE.ToString()).FirstOrDefault().DESCRIPTION;
					if (task.TASK_STEP == "350")
					{	// corrective action task
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}
					else
					{
						lblTaskTypeValue.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == task.TASK_STEP).FirstOrDefault().DESCRIPTION);
					}
					btnTaskLink.CommandArgument = task.RECORD_ID.ToString();
					btnTaskLink.Visible = true;
					btnTaskAssign.Visible = btnTaskComplete.Visible = false;
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
			lblTaskDetailValue.Text = task.DETAIL;              // incident description or audit question 

			// get the Create By person name and display
			PERSON createBy = SQMModelMgr.LookupPersonByEmpID(ctx, task.CREATE_ID.ToString());
			if (createBy == null)
			{
				lblCreatedByValue.Text = Resources.LocalizedText.AutomatedScheduler;
			}
			else
			{
				lblCreatedByValue.Text = SQMModelMgr.FormatPersonListItem(createBy, false, "LF");
			}
			
			rdpTaskDueDT.SelectedDate = (DateTime)task.DUE_DT;
			lblTaskStatusValue.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == ((int)TaskMgr.CalculateTaskStatus(task)).ToString()).FirstOrDefault().DESCRIPTION;
			tbTaskComments.Text = task.COMMENTS;

			int attachCount = SQM.Website.Classes.SQMDocumentMgr.GetAttachmentCountByRecord(CurrentTask.RECORD_TYPE, CurrentTask.RECORD_ID, CurrentTask.TASK_STEP, "");
			lnkAttachments.Text = attachCount == 0 ? Resources.LocalizedText.Attachments : (Resources.LocalizedText.Attachments + " (" + attachCount.ToString() + ")");
			lnkAttachments.Visible = true;
		}

		public void BindTaskAdd(int recordType, decimal recordID, decimal recordSubID, string taskStep, string taskType, string originalDetail, decimal plantID, string context)
		{
			PSsqmEntities ctx = new PSsqmEntities();
			if (TaskXLATList == null || TaskXLATList.Count == 0)
				TaskXLATList = SQMBasePage.SelectXLATList(new string[5] { "TASK_STATUS", "RECORD_TYPE", "INCIDENT_STATUS", "NOTIFY_SCOPE_TASK","ACTION_CATEGORY" });

			pnlUpdateTask.Visible = false;
			pnlAddTask.Visible = true;
			btnTaskAdd.CommandArgument = recordType.ToString() + "~" + recordID.ToString() + "~" + recordSubID.ToString() + "~" + taskStep + "~" + taskType + "~" + plantID.ToString();

			lblTaskTypeValueAdd.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "RECORD_TYPE" && l.XLAT_CODE == recordType.ToString()).FirstOrDefault().DESCRIPTION;

			switch ((TaskRecordType)recordType)
			{
				case TaskRecordType.Audit:
					if ((recordSubID > 0) || taskStep == "350")
					{	// action required if subid references a specific audit question
						lblTaskTypeValueAdd.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == taskStep).FirstOrDefault().DESCRIPTION);
					}
					else
					{
						lblTaskTypeValueAdd.Text += (" - " + TaskXLATList.Where(l => l.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && l.XLAT_CODE == "300").FirstOrDefault().DESCRIPTION);
					}
					break;
				default:
					return;
					break;
			}

			lblTaskDetailValueAdd.Text = originalDetail;  // cause of the requirement
			rdpTaskDueDTAdd.SelectedDate = SessionManager.UserContext.LocalTime; // default to today?
			lblTaskStatusValueAdd.Text = TaskXLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == (0).ToString()).FirstOrDefault().DESCRIPTION; // default to the "Open" status
			List<PERSON> personList = SQMModelMgr.SelectPlantPersonList(1, plantID).Where(l => !string.IsNullOrEmpty(l.EMAIL)).OrderBy(l => l.LAST_NAME).ToList();
			SQMBasePage.SetPersonList(ddlAssignPersonAdd, personList, "", 0, false, "LF");

			List<TaskItem> tasklist = TaskMgr.ExceptionTaskListByRecord(recordType, recordID, recordSubID);
			rptTaskList.DataSource = tasklist;
			rptTaskList.DataBind();
			if (tasklist.Count > 0)
				pnlListTasks.Visible = true;
			else
				pnlListTasks.Visible = false;

		}

		protected void TaskItem_Click(object sender, EventArgs e)
		{
			lnkTask_Click(sender, e);
		}

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

			//if (OnTaskListClick != null)
			//{
			//	OnTaskListClick(cmd);
			//}
			//else
			//{
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
							task.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
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
						Response.Redirect("/EHS/EHS_PrevActions.aspx?s=1");
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
							//Response.Redirect("/EHS/EHS_Audits.aspx");
							Response.Redirect("/EHS/EHS_Assessments.aspx");
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
			//}
		}

		public void rptTaskList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
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

		protected void rptTaskList_OnItemCreate(object sender, RepeaterItemEventArgs e)
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

		protected void btnTaskAdd_Click(object sender, EventArgs e)
		{
			lblErrorMessage.Text = "";
			Button btn = (Button)sender;
			if (btn == null || string.IsNullOrEmpty(btn.CommandArgument))
			{
				return;
			}

			string[] cmd = btn.CommandArgument.Split('~'); // recordType, recordID, recordSubID, taskStep, taskType
			int recordType = Convert.ToInt32(cmd[0]);
			decimal recordID = Convert.ToDecimal(cmd[1]);
			decimal recordSubID = Convert.ToDecimal(cmd[2]);
			string taskStep = cmd[3];
			string taskType = cmd[4];

			decimal plantID = 0;
			decimal.TryParse(cmd[5], out plantID);
	
			// make sure that the Assign To Employee has been selected
			if (ddlAssignPersonAdd.SelectedValue.ToString().Equals(""))
			{
				BindTaskAdd(recordType, recordID, recordSubID, "350", "T", lblTaskDetailValueAdd.Text.ToString(), plantID, "");
				lblErrorMessage.Text = lblErrRequiredInputs.Text.ToString();
				string script = "function f(){OpenUpdateTaskWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}
			else
			{

				TaskStatusMgr taskMgr = new TaskStatusMgr();
				taskMgr.Initialize(recordType, recordID);
				TASK_STATUS task = new TASK_STATUS();
				task.RECORD_TYPE = recordType;
				task.RECORD_ID = recordID;
				task.RECORD_SUBID = recordSubID;
				task.TASK_STEP = taskStep;
				task.TASK_TYPE = taskType;
				task.TASK_SEQ = 0;
				task.DUE_DT = rdpTaskDueDTAdd.SelectedDate;
				task.RESPONSIBLE_ID = Convert.ToDecimal(ddlAssignPersonAdd.SelectedValue.ToString());
				task.DETAIL = lblTaskDetailValueAdd.Text.ToString();
				task.DESCRIPTION = tbTaskDescriptionAdd.Text.ToString();
				task.STATUS = ((int)TaskStatus.New).ToString();
				task.CREATE_DT = SessionManager.UserContext.LocalTime != null ? SessionManager.UserContext.LocalTime : DateTime.UtcNow;
				task.CREATE_ID = SessionManager.UserContext.Person.PERSON_ID;

				taskMgr.CreateTask(task);
				taskMgr.UpdateTaskList(task.RECORD_ID);
				// send email
				EHSNotificationMgr.NotifyTaskAssigment(task, plantID);

				// reset the fields for the next add
				ddlAssignPersonAdd.SelectedIndex = 0;
				tbTaskDescriptionAdd.Text = "";

				if (OnTaskAdd != null)
				{
					OnTaskAdd("added", task.RECORD_ID, (decimal)task.RECORD_SUBID);
				}
				if (recordType == (int)TaskRecordType.Audit) // update the Question Status when adding tasks for an audit followup.
				{
					EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(recordID, recordSubID);

					if (auditQuestion != null)
					{
						auditQuestion.Status = "02";
						EHSAuditMgr.UpdateAnswer(auditQuestion);
					}
					SessionManager.ReturnRecordID = task.RECORD_ID;
					SessionManager.ReturnObject = "AddTask";
					SessionManager.ReturnStatus = true;
				}
			}
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

		protected void btnTaskLink_Click(object sender, EventArgs e)
		{
			SessionManager.ReturnObject = btnTaskLink.CommandArgument;
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnPath = Request.Path.ToString();
			Response.Redirect("/EHS/EHS_PrevActions.aspx?s=1");
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
				// send email
				EHSNotificationMgr.NotifyTaskAssigment(task, 0);
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

		protected string formatName(object responsiblePerson)
		{
			string fullname = "";
			PERSON person = new PERSON();
			try
			{
				person = (PERSON)responsiblePerson;
				fullname = SQMModelMgr.FormatPersonListItem(person, false, "LF");
			}
			catch
			{
				fullname = "";
			}
			return fullname;
		}


		protected void lnkAddAttach(object sender, EventArgs e)
		{
			int recordType = CurrentTask.RECORD_TYPE;
			uclAttachWin.OpenManageAttachmentsWindow(CurrentTask.RECORD_TYPE, CurrentTask.RECORD_ID, CurrentTask.TASK_STEP, "Upload Attachments", "Upload or view task evidence", PageUseMode.EditEnabled);
		}

		private void OnAttachmentsUpdate(string cmd)
		{
			if (CurrentTask != null && lnkAttachments.Visible == true)
			{
				int attachCount = SQM.Website.Classes.SQMDocumentMgr.GetAttachmentCountByRecord(CurrentTask.RECORD_TYPE, CurrentTask.RECORD_ID, CurrentTask.TASK_STEP, "");
				lnkAttachments.Text = attachCount == 0 ? Resources.LocalizedText.Attachments : (Resources.LocalizedText.Attachments + " (" + attachCount.ToString() + ")");
			}
			HiddenField hf = (HiddenField)this.Page.Master.FindControl("hfSubmitReset");
			if (hf != null)
				hf.Value = "true";
		}

	}
}