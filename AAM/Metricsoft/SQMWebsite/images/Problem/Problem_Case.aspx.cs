using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.Configuration;

namespace SQM.Website
{
    public partial class Problem_Case : SQMBasePage
    {
        //static ProblemCase problemCase;
      
        // manage current session object  (formerly was page static variable)
        ProblemCaseCtl CaseCtl()
        {
            if (SessionManager.CurrentProblemCase != null && SessionManager.CurrentProblemCase is ProblemCaseCtl)
                return (ProblemCaseCtl)SessionManager.CurrentProblemCase;
            else
            {
                return null;
            }
        }
        ProblemCaseCtl SetCaseCtl(ProblemCaseCtl caseCtl)
        {
            SessionManager.CurrentProblemCase = caseCtl;
            return CaseCtl();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclPageTabs.OnPageTabClick += PageTabClicked;
            uclSearchBar.OnNewClick += uclSearchBar_OnNewClick;
            uclSearchBar.OnSearchClick += uclSearchBar_OnSearchClick;
            uclSearchBar.OnReturnClick += uclSearchBar_OnReturnClick;
            uclCaseEdit.OnDeleteClick += uclCaseDelete;
            uclCaseEdit.OnEditSaveClick += uclCaseEdit_OnSaveClick;
            uclCaseEdit.OnEditApplyClick += uclCaseEdit_OnApplyClick;
            uclCaseEdit.OnEditAddClick += uclCaseEdit_OnEditAddClick;
           // uclCaseEdit.OnTrialNoClick += uclCaseEdit_OnTrialNoClick;
            uclCaseEdit.OnEditCancelClick += cancelClick;
            uclCaseList.OnProblemCaseClick += uclCaseList_Click;

            hfTimeout.Value = SQMBasePage.GetSessionTimeout().ToString();
        }

        private void uclSearchBar_OnNewClick()
        {
            SetCaseCtl(new ProblemCaseCtl().Initialize(entities, ""));
            CaseCtl().CreateNew("QI", SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.UserContext.Person);

            uclSearchBar.NewButton.Visible = false;
            pnlSearchList.Visible = false;
            divPageBody.Visible = true;
            ResetControlValues(divPageBody.Controls);
            SetupPage();
            PageTabClicked("lbTab0", "0");
        }

        protected void ddlStatusSelectChange(object sender, EventArgs e)
        {
            uclSearchBar_OnSearchClick();
        }
        protected void btnSearchClick(object sender, EventArgs e)
        {
            uclSearchBar_OnSearchClick();
        }


        private void uclSearchBar_OnReturnClick()
        {
            if (CaseCtl().isDirected  &&  !string.IsNullOrEmpty(SessionManager.ReturnPath))
            {
                CaseCtl().isDirected = false;
                Response.Redirect(SessionManager.ReturnPath);
            }
            else
            {
                if (CaseCtl().Context == "EHS")
                    Response.Redirect("/EHS/EHS_Incidents.aspx");
                else
                    uclSearchBar_OnSearchClick();
            }
        }

        private void uclSearchBar_OnSearchClick()
        {
			decimal caseID = 0;

            pnlSearchList.Visible = true;
            divPageBody.Visible = divNavArea.Visible = false;

            if (CaseCtl().Context == "EHS")
            {
                uclCaseList.LinksDisabled = UserContext.CheckAccess("EHS", "321") < AccessMode.Update ? true : false;
            }
            else
            {
                uclCaseList.LinksDisabled = UserContext.CheckAccess("SQM", "221") < AccessMode.Update ? true : false;
                uclSearchBar.NewButton.Visible = true;
            }

            string[] plantArray =  ddlPlantSelect.Items.Where(i => i.Checked == true).Select(i => i.Value).ToArray();
            decimal[] plantIDS = Array.ConvertAll(plantArray, new Converter<string, decimal>(decimal.Parse));

            CaseCtl().CaseList = ProblemCase.SelectProblemCaseList(SessionManager.PrimaryCompany().COMPANY_ID, CaseCtl().Context, ddlStatusSelect.SelectedValue);
            uclCaseList.BindProblemCaseListRepeater(ProblemCase.QualifyCaseList(CaseCtl().CaseList, plantIDS).OrderByDescending(l => l.ProbCase.CREATE_DT).ToList(), CaseCtl().Context);
            
            uclSearchBar.ReturnButton.Visible = false;
		}

        private void uclCaseEdit_OnTrialNoClick(decimal trialNo)
        {
            uclCaseEdit.BindCase6();
        }

        private void uclCaseList_Click(decimal caseID)
        {
            pnlSearchList.Visible = false;
            divPageBody.Visible = true;
            
            CaseCtl().Load(caseID);

            SetupPage();
            uclCaseEdit.BindIncidentList(CaseCtl().problemCase.IncidentList);
            uclCaseEdit.BindPartIssueItemList(CaseCtl().problemCase.PartIssueItemList);
            PageTabClicked("lbTab0", "0");
        }

        private void uclCaseDelete(string cmdArg)
        {
            int status = ProblemCase.DeleteProblemCase(CaseCtl().problemCase.ProbCase.PROBCASE_ID);
            if (status == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveError');", true);
            }
            else
            {
                CaseCtl().Clear();
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
            }

            uclSearchBar_OnSearchClick();
        }

        private void cancelClick(string cmdArg)
        {
            if (SessionManager.CurrentSecondaryTab == "lbTab0")
                uclSearchBar_OnSearchClick();
            else 
                PageTabClicked("lbTab0", "0");
        }

        private void ErrorAlert(CaseUpdateStatus updateStatus)
        {
            HiddenField hf = (HiddenField)hfBase.FindControl("hfErr"+updateStatus.ToString());
            if (hf != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('"+ hf.Value + "');", true);
            }
        }

        private void uclCaseEdit_OnSaveClick(string cmdArg)
        {
            bool bNotify;
            CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.Success;
            switch (cmdArg)
            {
                case "0":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase0(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase0();
                    }
                    break;
                case "1":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase1(CaseCtl().problemCase, out bNotify);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase1(bNotify);
                        if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success && bNotify)
                        {
                            try
                            {
                                string appUrl = SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE;
                                PERSON person = null;
                                PLANT plant = SQMModelMgr.LookupPlant((decimal)CaseCtl().problemCase.IncidentList[0].DETECT_PLANT_ID);
                                int emailStatus;
                                bool bUpdateTasks = false;
                                foreach (TASK_STATUS task in CaseCtl().problemCase.TeamTask.TaskList.Where(t => t.TASK_TYPE == "C" && t.TASK_SEQ == 0).ToList())
                                {
                                    if (task.RESPONSIBLE_ID.HasValue  &&  (person = SQMModelMgr.LookupPerson((decimal)task.RESPONSIBLE_ID, "")) != null && !task.NOTIFY_DT.HasValue)
                                    {
                                        task.NOTIFY_DT = DateTime.UtcNow;
                                        List<TaskItem> taskList = new List<TaskItem>();
                                        TaskItem taskItem = new TaskItem();
                                        taskItem.RecordID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                                        taskItem.RecordKey = CaseCtl().problemCase.ProbCase.PROBCASE_ID.ToString();
                                        taskItem.RecordType = 21;
                                        taskItem.NotifyType = TaskNotification.Owner;
                                        taskItem.Taskstatus = TaskMgr.CalculateTaskStatus(task);
                                        taskItem.Detail = SQMModelMgr.FormatPersonListItem(person);
                                        taskItem.Description = WebSiteCommon.FormatID(CaseCtl().problemCase.ProbCase.PROBCASE_ID, 6) + " / " + WebSiteCommon.GetXlatValue("caseStep", task.TASK_STEP);
                                        taskItem.Plant = plant;
                                        taskItem.Task = task;
                                      //  taskItem.Task = task;
                                        taskList.Add(taskItem);
                                        Thread thread = new Thread(() => TaskMgr.MailTaskList(taskList, person.EMAIL, "web"));
                                        thread.IsBackground = true;
                                        thread.Start();
                                        bUpdateTasks = true;
                                      //  if ((emailStatus = TaskMgr.MailTaskList(taskList, person.EMAIL, "web")) > 0)
                                      //      bUpdateTasks = true;
                                    }
                                }
                                if (bUpdateTasks)
                                    CaseCtl().Update(); // save task notify dates
                            }
                            catch (Exception ex)
                            {
                                //  SQMLogger.LogException(ex);
                            }
                        }
                    }
                    break;
                case "2":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase2(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase2();
                    }
                    break;
                case "3":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase3(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase3();
                    }
                    break;
                case "4":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase4(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase4();
                    }
                    break;
                case "5":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase5(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase5();
                    }
                    break;
                case "6":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase6(CaseCtl().problemCase);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase6();
                    }
                    break;
                case "7":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase7(CaseCtl().problemCase, true);
                    if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
                    {
                        CaseCtl().Update();
                        uclCaseEdit.BindCase7();
                    }
                    break;
                case "8":
                    CaseCtl().problemCase = uclCaseEdit.UpdateCase8(CaseCtl().problemCase, out bNotify);
                    CaseCtl().Update();
                    uclCaseEdit.BindCase8(bNotify);
                    if (bNotify && CaseCtl().problemCase != null)
                    {
                        List<decimal?> teamList = new List<decimal?>();
                        teamList = CaseCtl().problemCase.TeamTask.TaskList.Where(l => l.TASK_TYPE == "C" && l.RESPONSIBLE_ID > 0).Select(l => l.RESPONSIBLE_ID).Distinct().ToList();
                        PERSON person = null;
                       // string emailStatus;
                        foreach (decimal? personID in teamList)
                        {
                            if ((person = SQMModelMgr.LookupPerson((decimal)personID, "")) != null)
                            {
                                Thread thread = new Thread(() => WebSiteCommon.SendEmail(person.EMAIL, CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE_TITLE, CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE, ""));
                                thread.IsBackground = true;
                                thread.Start();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            if (CaseCtl().problemCase.UpdateStatus == CaseUpdateStatus.Success)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                SetupPage();
            }
            else
            {
                ErrorAlert(CaseCtl().problemCase.UpdateStatus);
            }
        }

        private void uclCaseEdit_OnEditAddClick(string cmdArg)
        {
            switch (cmdArg)
            {
                case "trial":
                    CaseCtl().problemCase.AddVerifyTrial();
                    break;
            }
        }

        private void uclCaseEdit_OnApplyClick(string cmdArg)
        {
            if (Page.IsPostBack)
            {
                ;
            }
        }

        private void PageTabClicked(string tabID, string cmdArg)
        {
           // divWorkArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            SetActiveTab(SessionManager.CurrentSecondaryTab = tabID);
            CaseCtl().problemCase.StepName = uclPageTabs.GetTabLabel(SessionManager.CurrentSecondaryTab);

            pnlCaseEdit.Visible = divNavArea.Visible = true;
            uclPageTabs.SetTitle(lblTabsTitle.Text);
            uclCaseEdit.ToggleVisible(null);
            uclSearchBar.NewButton.Visible = false;
            uclSearchBar.ReturnButton.Visible = uclSearchBar.ReturnButton.Enabled = true;
            if (CaseCtl().isDirected)
                uclSearchBar.ReturnButton.Text = lblReturnInbox.Text;
            else
                uclSearchBar.ReturnButton.Text = lblReturnCaseList.Text;

            if (CaseCtl().PersonSelectListCount() == 0)
                CaseCtl().LoadPersonSelectList(false);

            CaseCtl().PageMode = PageUseMode.EditEnabled;
            uclCaseEdit.SetPageMode();

            switch (cmdArg)
            {
                case "0":
                    uclCaseEdit.BindCase0();
                    CaseCtl().isAutoCreated = false;
                    break;
                case "1":
                    uclCaseEdit.BindCase1(false);
                    break;
                case "2":
                    uclCaseEdit.BindCase2();
                    break;
                case "3":
                    uclCaseEdit.BindCase3();
                    break;
                case "4":
                    uclCaseEdit.BindCase4();
                    break;
                case "5":
                    uclCaseEdit.BindCase5();
                    break;
                case "6":
                    uclCaseEdit.BindCase6();
                    break;
                case "7":
                    uclCaseEdit.BindCase7();
                    break;
                case "8":
                    uclCaseEdit.BindCase8(false);
                    break;
                default:
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string appContext;
                if (Request.QueryString != null && Request.QueryString.Get("c") != null)
                    appContext = Request.QueryString.Get("c").ToString();
                else
                    appContext = "";

                SetCaseCtl(new ProblemCaseCtl().Initialize(null, appContext));

                IsCurrentPage();
                uclSearchBar.SetButtonsVisible(false, false, (UserContext.CheckAccess("SQM", "221") >= AccessMode.Update || UserContext.CheckAccess("EHS", "321") >= AccessMode.Update) && CaseCtl().Context != "EHS" ? true : false, false, false, false);
				uclSearchBar.SetButtonsEnabled(false, false, CaseCtl().Context == "EHS" ? false : true, false, false, false);

                lblProbCaseInstructions.Visible = (CaseCtl().Context == "EHS") ? false : true;
                lblProbCaseInstructionsEHS.Visible = (CaseCtl().Context == "EHS") ? true : false;

                if (string.IsNullOrEmpty(SessionManager.CurrentSecondaryTab))
                    CaseCtl().Clear();
                if (CaseCtl().IsClear())
                {
                    uclCaseHdr.ProblemCaseHdr.Visible = false;
                    uclPageTabs.TabsPanel.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                uclSearchBar.PageTitle.Text = lblProbCaseTitle.Text;
                CaseCtl().isDirected = false;

                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect(CaseCtl().Context == "EHS" ? CaseCtl().Context : "SQM", 0, true);
                }

                if (ddlPlantSelect.Items.Count < 1)
                {
                    List<PLANT> plantList = SQMModelMgr.SelectPlantList(entities, SQMModelMgr.LookupPrimaryCompany(entities).COMPANY_ID, 0);
                    ddlPlantSelect.Items.Clear();
                    foreach (PLANT plant in plantList)
                    {
                        if (SessionManager.PlantAccess(plant.PLANT_ID))
                        {
                            RadComboBoxItem item = new RadComboBoxItem(plant.PLANT_NAME, plant.PLANT_ID.ToString());
                            item.CssClass = "prompt";
                            item.Checked = true;
                            ddlPlantSelect.Items.Add(item);
                        }
                    }
                    ddlStatusSelect.SelectedValue = "A";
                }

                // create new case from quality issue 
                if ((bool)SessionManager.ReturnStatus)
                {
                    if (SessionManager.ReturnObject is QI_OCCUR)
                    {
                        QI_OCCUR qiIssue = (QI_OCCUR)SessionManager.ReturnObject;
                        CaseCtl().CreateNew("QI", SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.UserContext.Person);
                        CaseCtl().isAutoCreated = true;
                        CaseCtl().problemCase.ProbCase.DESC_LONG = qiIssue.INCIDENT.DESCRIPTION;
                        CaseCtl().problemCase.AddIncident(qiIssue.INCIDENT_ID);
                        uclCaseEdit.BindIncidentList(CaseCtl().problemCase.IncidentList);
                        uclCaseEdit.BindPartIssueItemList(CaseCtl().problemCase.PartIssueItemList);
                    }
                    else if (SessionManager.ReturnObject is TASK_STATUS)
                    {
                        // from inbox
                        CaseCtl().isDirected = true;
                        uclSearchBar.ReturnButton.Text = lblReturnInbox.Text;
                        TASK_STATUS task = (TASK_STATUS)SessionManager.ReturnObject;
                        uclCaseList_Click(task.RECORD_ID);
                        if (CaseCtl().problemCase.CheckCaseNextStep() >= Convert.ToInt32(task.TASK_STEP))
                            PageTabClicked("lbTab" + task.TASK_STEP, task.TASK_STEP);
                        else
                            PageTabClicked("lbTab0", "0");
                    }
                    else if (SessionManager.ReturnObject is decimal)
                    {
                        // from console list
                        uclCaseList_Click((decimal)SessionManager.ReturnObject);
                        PageTabClicked("lbTab0", "0");
                    }
                    else if (SessionManager.ReturnObject is INCIDENT)
                    {
                        // Problem case from EHS Incident
                        INCIDENT incident = (INCIDENT)SessionManager.ReturnObject;
                        decimal locationId = EHSIncidentMgr.SelectIncidentLocationIdByIncidentId(incident.INCIDENT_ID);
                        CaseCtl().CreateNew("EHS", SessionManager.PrimaryCompany().COMPANY_ID, SessionManager.UserContext.Person);
                        CaseCtl().problemCase.ProbCase.DESC_SHORT = incident.ISSUE_TYPE;
                        CaseCtl().problemCase.ProbCase.DESC_LONG = incident.DESCRIPTION;
                        CaseCtl().problemCase.ProbCase.PROBCASE_TYPE = "EHS";
                        CaseCtl().problemCase.AddIncident(incident.INCIDENT_ID);
                        uclCaseEdit.BindIncidentList(CaseCtl().problemCase.IncidentList);
                        if (CaseCtl().problemCase.ProbCase.PROB_DEFINE == null)
                            CaseCtl().problemCase.CreateProblemDefinition();
                        CaseCtl().isAutoCreated = true;
                    }
                    else
                    {
                        uclSearchBar_OnSearchClick();
                    }
                }
                else
                {
                    uclSearchBar_OnSearchClick();
                }
            }
            else
            {
                /* add incidents from qi issue popup list */
                if ((bool)SessionManager.ReturnStatus)
                {
                    if (SessionManager.ReturnObject.GetType().ToString().ToUpper().Contains("INCIDENT"))
                    {
                        List<INCIDENT> incidentList = (List<INCIDENT>)SessionManager.ReturnObject;
                        foreach (INCIDENT incident in incidentList)
                        {
                            CaseCtl().problemCase.AddIncident(incident.INCIDENT_ID);
                        }
                        uclCaseEdit.BindIncidentList(CaseCtl().problemCase.IncidentList);
                        uclCaseEdit.BindPartIssueItemList(CaseCtl().problemCase.PartIssueItemList);
                        PageTabClicked("lbTab0", "0");
                    }

                    else if (SessionManager.ReturnObject == "DisplayCases")
                    {
                        uclSearchBar_OnSearchClick();
                        SessionManager.ClearReturns();
                    }
				}
            }
            SessionManager.ClearReturns();

            if (CaseCtl().isAutoCreated)
            {
                CaseCtl().Update();
				SetupPage();
                PageTabClicked("lbTab0", "0");
            }
        }

        private void SetupPage()
        {

            int nextStep = CaseCtl().problemCase.CheckCaseNextStep();
            int progress = CaseCtl().problemCase.CheckCaseStatus();
            if (CaseCtl().isDirected)
                uclSearchBar.SetButtonsEnabled(false, false, false, false, false, false);
            uclPageTabs.TabsPanel.Visible = true;
            uclPageTabs.SetTabLabelsFromList(WebSiteCommon.GetXlatList("caseStep", "", "short"));
            uclPageTabs.SetAllTabsEnabled(true);

            if (CaseCtl().problemCase.TeamTask != null)
            {
                TaskStatus status;
                foreach (TASK_STATUS task in CaseCtl().problemCase.TeamTask.TaskList.Where(l => l.TASK_TYPE == "C" && l.TASK_SEQ == 0))
                {
                    if ((status = TaskMgr.CalculateTaskStatus(task)) < TaskStatus.New)
                    {
                        string imageURL = TaskMgr.TaskStatusImage(status);
                        uclPageTabs.SetTabImage(Convert.ToInt32(task.TASK_STEP), imageURL, status.ToString());
                    }
                }
            }

            SessionManager.EffLocation = SessionManager.UserContext.WorkingLocation;  /// is this true ??

            if (CaseCtl().isNew)
            {
                uclCaseHdr.ProblemCaseHdr.Visible = true;
            }
            else
            {
                uclCaseHdr.ProblemCaseHdr.Visible = true;
                uclCaseHdr.BindProblemCaseHeader(CaseCtl().problemCase, true);
            }
        }
    }

}
