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
        private List<BusinessLocation> locationList;
        private List<decimal> respForList;
        private int pageWidth;

        /*
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }
        */
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               // if (SessionManager.SessionContext == null)
              //      throw new UserContextError();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
                hf.Value = SessionManager.CurrentMenuItem = "lbHomeMain";
                IsCurrentPage();

                hfWidth.Value = SessionManager.PageWidth > 0 ? SessionManager.PageWidth.ToString() : "";
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("SQM", 10, true, true, hfDocviewMessage.Value);
                    ucl.BindDocumentSelect("EHS", 10, true, false, hfDocviewMessage.Value);
                }

                SetupPage();
                DisplayCalendar(DateTime.Now);
            }
            else
            {
                ;
            }
                
            pnlCalendar.Visible = true;
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

        private void SetupPage()
        {
            ddlScheduleScope.Items.Clear();

			if (UserContext.CheckUserPrivilege(SysPriv.config, SysScope.busloc))
            {
               // List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
                List<BusinessLocation> locationList = SessionManager.PlantList;
                locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");
                locationList = UserContext.FilterPlantAccessList(locationList, "SQM", "");

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
 
			// get tasks - due or escalated
            respForList = new List<decimal>();
            respForList.Add(SessionManager.UserContext.Person.PERSON_ID);
            respForList.AddRange(SessionManager.UserContext.DelegateList);
            SessionManager.UserContext.TaskList.Clear();
            SessionManager.UserContext.TaskList = new List<TaskItem>();
            DateTime fromDate = DateTime.Now.AddMonths(-6);

			if (UserContext.CheckUserPrivilege(SysPriv.view, SysScope.inbox))
            {
                SessionManager.UserContext.TaskList.AddRange(TaskMgr.ProfileInputStatus(new DateTime(fromDate.Year, fromDate.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), respForList, SessionManager.UserContext.EscalationAssignments));
                SessionManager.UserContext.TaskList.AddRange(TaskMgr.IncidentTaskStatus(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, respForList, SessionManager.UserContext.EscalationAssignments, true));
				if (UserContext.CheckUserPrivilege(SysPriv.approve, SysScope.envdata))
                {
                    SessionManager.UserContext.TaskList.AddRange(TaskMgr.ProfileFinalizeStatus(new DateTime(fromDate.Year, fromDate.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), respForList, SessionManager.UserContext.EscalationAssignments, SessionManager.UserContext.Person));
                }
            }

            ++SessionManager.UserContext.InboxReviews;
        }

        private void DisplayCalendar(DateTime selectedDate)
        {
            // don't display task strip if tasklist is empty
            if (SessionManager.UserContext.TaskList.Count == 0)
            {
                divCalendar.Attributes.Remove("class");
                divTasks.Attributes.Remove("class");
                divTasks.Visible = false;
            }

            bool enableItemLinks = false;

			// get scheduled tasks
            respForList = new List<decimal>();
            respForList.Add(SessionManager.UserContext.Person.PERSON_ID);
            respForList.AddRange(SessionManager.UserContext.DelegateList);
            DateTime toDate = DateTime.Now.AddMonths(Convert.ToInt32(sldScheduleRange.Value));

            string selectedValue = "0";
            if (ddlScheduleScope.SelectedIndex > -1)
                selectedValue =  ddlScheduleScope.SelectedValue;
            else if (mnuScheduleScope.SelectedItem != null)
                selectedValue = mnuScheduleScope.SelectedItem.Value;
            
            List<TaskItem> taskScheduleList = new List<TaskItem>();

            if (selectedValue == "0" ||  selectedValue == "TOP")
            {
                taskScheduleList.AddRange(SessionManager.UserContext.TaskList.Where(l => l.Task.DUE_DT < DateTime.Now).ToList());
                taskScheduleList.AddRange(TaskMgr.IncidentTaskSchedule(SessionManager.PrimaryCompany().COMPANY_ID, DateTime.Now, toDate, respForList, new decimal[0] { }, true));
                taskScheduleList.AddRange(TaskMgr.ProfileInputSchedule(DateTime.Now, toDate, respForList, new decimal[0] { }, SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.busorg)));
                enableItemLinks = true;
            }
            else
            {
                List<decimal> plantIDS = new List<decimal>();
                if (selectedValue.All(c => c >= '0' && c <= '9') == false)
                {   // all accessible plants for a selected BU
                    decimal busOrgID = Convert.ToDecimal(selectedValue.Substring(2, selectedValue.Length-2));
                    decimal[] busOrgPlantIDS = SQMModelMgr.SelectPlantList(entities, SessionManager.PrimaryCompany().COMPANY_ID, busOrgID).Select(l => l.PLANT_ID).ToArray();
                    foreach (decimal plantID in busOrgPlantIDS)
                    {
                        if (ddlScheduleScope.Visible  &&  ddlScheduleScope.Items.FindItemByValue(plantID.ToString()) != null)
                        {
                            plantIDS.Add(plantID);
                        }
                        else if (mnuScheduleScope.Visible)
                        {
                            RadMenuItem miTop = mnuScheduleScope.Items[0];
                            foreach (RadMenuItem miBU in miTop.Items)
                            {
                                foreach (RadMenuItem miLoc in miBU.Items)
                                {
                                    if (miLoc.Value == plantID.ToString())
                                        plantIDS.Add(plantID);
                                }
                            }
                        }
                    }
                }
                else
                {   // specific plant
                    plantIDS.Add(Convert.ToDecimal(selectedValue));
                }

                taskScheduleList.AddRange(SessionManager.UserContext.TaskList.Where(l => l.Task.DUE_DT < DateTime.Now &&
                    (plantIDS.Contains(l.Plant.PLANT_ID) || (l.PlantResponsible != null  &&  plantIDS.Contains((decimal)l.PlantResponsible.PLANT_ID)))));
                taskScheduleList.AddRange(TaskMgr.IncidentTaskSchedule(SessionManager.PrimaryCompany().COMPANY_ID, DateTime.Now, toDate, new List<decimal>(), plantIDS.ToArray(), true));
                taskScheduleList.AddRange(TaskMgr.ProfileInputSchedule(DateTime.Now, toDate, new List<decimal>(), plantIDS.ToArray(), false));
                if (SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.busorg))
                    enableItemLinks = true;
            }

            uclTaskSchedule.BindTaskSchedule(taskScheduleList, selectedDate, enableItemLinks);

            uclTaskStrip.BindTaskStrip(SessionManager.UserContext.TaskList.Where(l=> !String.IsNullOrEmpty(l.LongTitle)).OrderBy(l=> l.Task.DUE_DT).ToList());
        }

    }
}