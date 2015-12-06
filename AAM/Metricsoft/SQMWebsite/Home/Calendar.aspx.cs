using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Calendar : SQMBasePage
    {
        private List<decimal> respForList;
		private List<decimal> respPlantList;

		protected override void OnInit(EventArgs e)
        {
			base.OnInit(e);

			uclTaskList.OnTaskListCommand += UpdateTaskList;
			uclTaskList.OnTaskListItemClick += UpdateSelectedTask;
			uclTask.OnTaskUpdate += UpdateTaskList;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
                hf.Value = SessionManager.CurrentMenuItem = "lbHomeMain";
                IsCurrentPage();
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
					ucl.BindDocumentSelect("SYS", 10, true, false, hfDocviewMessage.Value);
                    //ucl.BindDocumentSelect("EHS", 2, true, false, hfDocviewMessage.Value);
                }

                SetupPage();
                DisplayCalendar(DateTime.Now);

				if (!string.IsNullOrEmpty(Request.QueryString["v"]))   // initial view override
				{
					string viewOption = Request.QueryString["v"];
					switch (viewOption.ToUpper())
					{
						case "T":
							btnChangeView_Click(btnTaskView, null);
							break;
						case "E":
							btnChangeView_Click(btnEscalateView, null);
							break;
						default:
							btnChangeView_Click(btnCalendarView, null);
							break;
					}
				}
            }
        }

		protected void btnChangeView_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;

			switch (btn.CommandArgument)
			{
				case "T":
					UpdateTaskList("");
					divCalendar.Visible = divEscalate.Visible = false;
					divTaskList.Visible = true;
					break;
				case "E":
					divCalendar.Visible = divTaskList.Visible = false;
					divEscalate.Visible = true;
					break;
				default:
					divTaskList.Visible = divEscalate.Visible = false;
					uclTaskSchedule.SetTaskScheduleCulture("");
					divCalendar.Visible = true;
					break;
			}
		}


        protected void ScheduleScope_Select(object sender, EventArgs e)
        {
            if (sender is RadSlider)
                DisplayCalendar(DateTime.Now.AddMonths(Convert.ToInt32(sldScheduleRange.Value)));
            else if (sender is RadMenu)
            {
                mnuScheduleScope.Items[0].Text = mnuScheduleScope.SelectedItem.Text;
                DisplayCalendar(uclTaskSchedule.TaskScheduleSelectedDate);
            }
            else
                DisplayCalendar(uclTaskSchedule.TaskScheduleSelectedDate);
        }

		private void UpdateTaskList(string cmd)
		{
			TaskStatusMgr myTasks = new TaskStatusMgr().CreateNew(0, 0);
			myTasks.SelectTaskList(new int[2] { (int)TaskRecordType.Audit, (int)TaskRecordType.HealthSafetyIncident }, new string[1] {((int)SysPriv.action).ToString()}, SessionManager.UserContext.Person.PERSON_ID, true);
			uclTaskList.BindTaskList(myTasks.TaskList, "");
		}

		private void UpdateSelectedTask(decimal taskID)
		{
			TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
			TASK_STATUS task = taskMgr.SelectTask(taskID);

			SessionManager.ReturnObject = task;
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnPath = (Request.Path.ToString() + "?v=T");  // return to action/task view
			Response.Redirect("/Home/TaskAction.aspx");
		}

        private void SetupPage()
        {
            ddlScheduleScope.Items.Clear();

			SysPriv maxPriv = UserContext.GetMaxScopePrivilege(SysScope.busloc);
			if (maxPriv <= SysPriv.config)  // is a plant admin or greater ?
            {
                List<BusinessLocation> locationList = SessionManager.PlantList;
                locationList = UserContext.FilterPlantAccessList(locationList);

                if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1  &&  SessionManager.IsUserAgentType("ipad,iphone") == false)
                {
                    ddlScheduleScope.Visible = false;
                    mnuScheduleScope.Visible = true;
                    SQMBasePage.SetLocationList(mnuScheduleScope, locationList, 0, SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME, "TOP", true);
                    RadMenuItem mi = new RadMenuItem();
                    mi.Text = (SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME);
                    mi.Value = "0";
                    mi.ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
                    mnuScheduleScope.Items[0].Items.Insert(0, mi);
                }
                else 
                {
                    ddlScheduleScope.Visible = true;
                    mnuScheduleScope.Visible = false;
                    SQMBasePage.SetLocationList(ddlScheduleScope, locationList, 0, true);
                    ddlScheduleScope.Items.Insert(0, new RadComboBoxItem((SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME), "0"));
                    ddlScheduleScope.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
                }
            }
            else
            {
                ddlScheduleScope.Visible = true;
                mnuScheduleScope.Visible = false;
                ddlScheduleScope.Items.Insert(0, new RadComboBoxItem((SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME), "0"));
                ddlScheduleScope.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
            }
 
            ++SessionManager.UserContext.InboxReviews;
        }

        private void DisplayCalendar(DateTime selectedDate)
        {
            // don't display task strip if tasklist is empty
      

            bool enableItemLinks = false;

			// get scheduled tasks
            respForList = new List<decimal>();
            respForList.Add(SessionManager.UserContext.Person.PERSON_ID);
			DateTime toDate = DateTime.Now.AddMonths(2);
			DateTime fromDate = DateTime.Now.AddMonths(-3);

            string selectedValue = "0";
            if (ddlScheduleScope.SelectedIndex > -1)
                selectedValue =  ddlScheduleScope.SelectedValue;
            else if (mnuScheduleScope.SelectedItem != null)
                selectedValue = mnuScheduleScope.SelectedItem.Value;

			respForList = new List<decimal>();
			respPlantList = new List<decimal>();
			List<TaskItem> taskList = new List<TaskItem>();
            List<TaskItem> taskScheduleList = new List<TaskItem>();

            if (selectedValue == "0" ||  selectedValue == "TOP")
            {
				respForList.Add(SessionManager.UserContext.Person.PERSON_ID);
            }
            else
            {
                if (selectedValue.All(c => c >= '0' && c <= '9') == false)
                {   // all accessible plants for a selected BU
                    decimal busOrgID = Convert.ToDecimal(selectedValue.Substring(2, selectedValue.Length-2));
                    decimal[] busOrgPlantIDS = SQMModelMgr.SelectPlantList(entities, SessionManager.PrimaryCompany().COMPANY_ID, busOrgID).Select(l => l.PLANT_ID).ToArray();
                    foreach (decimal plantID in busOrgPlantIDS)
                    {
                        if (ddlScheduleScope.Visible  &&  ddlScheduleScope.Items.FindItemByValue(plantID.ToString()) != null)
                        {
                            respPlantList.Add(plantID);
                        }
                        else if (mnuScheduleScope.Visible)
                        {
                            RadMenuItem miTop = mnuScheduleScope.Items[0];
                            foreach (RadMenuItem miBU in miTop.Items)
                            {
                                foreach (RadMenuItem miLoc in miBU.Items)
                                {
                                    if (miLoc.Value == plantID.ToString())
                                        respPlantList.Add(plantID);
                                }
                            }
                        }
                    }
                }
                else
                {   // specific plant
                    respPlantList.Add(Convert.ToDecimal(selectedValue));
                }
 
                if (SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.busorg))
                    enableItemLinks = true;
            }

			if (UserContext.CheckUserPrivilege(SysPriv.view, SysScope.inbox))
			{
				taskList.AddRange(TaskMgr.ProfileInputStatus(new DateTime(fromDate.Year, fromDate.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), respForList, respPlantList));
				taskList.AddRange(TaskMgr.IncidentTaskStatus(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, respForList, respPlantList, false));
			}
			taskScheduleList.AddRange(taskList);
			taskScheduleList.AddRange(TaskMgr.IncidentTaskSchedule(SessionManager.PrimaryCompany().COMPANY_ID, DateTime.Now, toDate, respForList, respPlantList.ToArray(), false));
			taskScheduleList.AddRange(TaskMgr.ProfileInputSchedule(DateTime.Now, toDate, respForList, respPlantList.ToArray(), SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.busorg)));
			enableItemLinks = true;

            uclTaskSchedule.BindTaskSchedule(taskScheduleList, selectedDate, enableItemLinks);

			// get task escalations 
			respForList = new List<decimal>();
			respForList.AddRange(SQMModelMgr.SelectPersonListBySupvID(SessionManager.UserContext.Person.EMP_ID).Select(l => l.PERSON_ID).ToList());
			if (respForList.Count > 0)
			{
				// has escalation persons
				List<TaskItem> escalateList = TaskMgr.IncidentTaskStatus(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, respForList, new List<decimal>(), false);
				uclTaskStrip.BindTaskStrip(escalateList.Where(l => !String.IsNullOrEmpty(l.LongTitle)).OrderBy(l => l.Task.DUE_DT).ToList());
			}

			divTaskList.Visible = divEscalate.Visible = false;
			divCalendar.Visible = true;
        }
    }
}