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
    public partial class Inbox : SQMBasePage
    {
        static List<LOCAL_LANGUAGE> langList;
        List<PERSON> respForList;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclPrefsEdit.OnEditCancelClick += CancelUserPrefs;
            uclPrefsEdit.OnPersonUpdate += ApplyUserPrefs;
            uclPrefsEdit.OnBusinessLocationChanged += ApplyWorkingLocation;

            hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetupPage();

                if (string.IsNullOrEmpty(hdCurrentActiveSecondaryTab.Value))
                    tab_Click(lbHome1_tab, null);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
                hf.Value = SessionManager.CurrentMenuItem = "lbHomeMain";
                IsCurrentPage();
                if (SessionManager.ReturnObject != null)
                {
                    tab_Click(lbHome3_tab, null);
                    SessionManager.ClearReturns();
                }
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
            }
        }
 
        protected void tab_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            string cmd = lnk.CommandArgument.ToString();
            SetActiveTab(SessionManager.CurrentSecondaryTab = lnk.ClientID);
            switch (cmd)
            {
                case "3":
                    pnlHome1.Visible = false;
                    pnlHome3.Visible = true;
                    uclPrefsEdit.BindUser(null, SessionManager.UserContext.HRLocation, SessionManager.UserContext.WorkingLocation);
                    break;
                default:
                    pnlHome3.Visible  = false;
                    pnlHome1.Visible = true;
                    break;
            }
        }

        private void CancelUserPrefs(string cmd)
        {
            tab_Click(lbHome1_tab, null);
        }
        private void ApplyUserPrefs(PERSON user)
        {
            if (user != null)
            {
                SessionManager.UserContext.Person = user;
                SessionManager.SessionContext.SetLanguage((int)user.PREFERRED_LANG_ID);
                uclPrefsEdit.BindUser(null, SessionManager.UserContext.HRLocation, SessionManager.UserContext.WorkingLocation);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
            }
        }

        private void ApplyWorkingLocation(BusinessLocation newLocation)
        {
            if (newLocation == null)
            {
                SessionManager.UserContext.WorkingLocation = SessionManager.UserContext.HRLocation;
            }
            else
            {
                SessionManager.UserContext.WorkingLocation = newLocation;
                uclPrefsEdit.BindUser(null, SessionManager.UserContext.HRLocation, SessionManager.UserContext.WorkingLocation);
            }
            Server.Transfer(Request.Path);
        }

        private void SetupPage()
        {
            if (langList == null || langList.Count == 0)
            {
                langList = SQMModelMgr.SelectLanguageList(new PSsqmEntities(), true);
                uclPrefsEdit.SetLanguageList(langList);
            }

            BusinessLocation businessLocation = new BusinessLocation(); businessLocation = SessionManager.UserContext.HRLocation;

            List<decimal> respForList = new List<decimal>();
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
 
            lblTaskCount0.Text = SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Owner).Count().ToString();
            uclTaskList0.BindTaskList(SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Owner).OrderBy(l => l.RecordType).ThenByDescending(l => l.Task.DUE_DT).ToList());

            lblTaskCount1.Text = SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Delegate).Count().ToString();
            if (lblTaskCount1.Text != "0")
            {
                divTaskList1.Visible =  pnlTaskList1.Visible = true;
                uclTaskList1.BindTaskList(SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Delegate).OrderBy(l => l.RecordType).ThenByDescending(l => l.Task.DUE_DT).ToList());
            }

            lblTaskCount2.Text = SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Escalation).Count().ToString();
            if (lblTaskCount2.Text  != "0")
            {
               divTaskList2.Visible = pnlTaskList2.Visible = true;
                uclTaskList2.BindTaskList(SessionManager.UserContext.TaskList.Where(l => l.NotifyType == TaskNotification.Escalation).OrderBy(l => l.RecordType).ThenByDescending(l => l.Task.DUE_DT).ToList());
            }

            ++SessionManager.UserContext.InboxReviews;
        }

    }
}