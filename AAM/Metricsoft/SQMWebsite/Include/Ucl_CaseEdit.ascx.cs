using SQM.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_CaseEdit : System.Web.UI.UserControl
    {
       // static bool isNew;
		static bool isEhsCase;
      //  static PageUseMode pageMode;
        static List<PROB_CAUSE_STEP> causeStepList;
        static string staticStep;

        // manage current session object  (formerly was page static variable)
        ProblemCaseCtl CaseCtl()
        {
            if (SessionManager.CurrentProblemCase != null && SessionManager.CurrentProblemCase is ProblemCaseCtl)
                return (ProblemCaseCtl)SessionManager.CurrentProblemCase;
            else
                return null;
        }
        ProblemCaseCtl SetCaseCtl(ProblemCaseCtl caseCtl)
        {
            SessionManager.CurrentProblemCase = caseCtl;
            return CaseCtl();
        }

        #region events
        public event EditItemClick OnDeleteClick;
        public event EditItemClick OnEditCancelClick;
        public event EditItemClick OnEditSaveClick;
        public event EditItemClick OnEditAddClick;
        public event EditItemClick OnEditApplyClick;
        public event EditItemClick OnEditSelectChange;

        public event GridItemClick OnTrialNoClick;
        public event ItemUpdateID OnTeamListChange;

       // public event UpdateAttachment OnUpdateAttachment;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            uclRadAsyncUpload3.OnAttachmentDelete += ReBindAttachmentsOnDelete;
            uclRadAsyncUpload5.OnAttachmentDelete += ReBindAttachmentsOnDelete;
            uclRadAsyncUpload6.OnAttachmentDelete += ReBindAttachmentsOnDelete;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			this.gvContainList.Columns[2].HeaderText = "Due Date /<br>" + Resources.LocalizedText.Status;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (OnEditCancelClick != null)
            {
                Button btn = (Button)sender;
                OnEditCancelClick(btn.CommandArgument);
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (OnEditSaveClick != null)
            {
                OnEditSaveClick(btn.CommandArgument);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (OnDeleteClick != null)
            {
                OnDeleteClick("");
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.ID)
            {
                case "btnAddContainAction":
                    AddCase3ActionItem();
                    break;
                case "btnAddCasuseStep":
                    AddCauseStep();
                    break;
                case "btnCase5AddAction":
                    AddCorrectiveAction(btn.CommandArgument);
                    break;
                case "btnCase6AddTrial2":
                    AddVerifyTrial();
                    break;
                default:
                    break;
            }
            if (OnEditAddClick != null)
            {
                OnEditAddClick(btn.CommandArgument);
            }
        }
        protected void ddlSelect_Change(object sender, EventArgs e)
        {
            if (OnEditSelectChange != null)
            {
                DropDownList ddl = (DropDownList)sender;
                OnEditSelectChange(ddl.SelectedValue);
            }
        }
        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (OnEditApplyClick != null)
            {
                Button btn = (Button)sender;
                OnEditApplyClick(btn.CommandArgument);
            }
        }

        protected void btnSearchIncidents(object sender, EventArgs e)
        {
           // List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0);

          //  uclQISearch.Initialize("", null, locationList, DateTime.Now.AddYears(-2), DateTime.Now, true);
           // pnlIncidentSearch.Visible = true;
            //udpAddIncident.Update();
        }

        #endregion

        private bool CheckTaskProgress(int caseStep, Ucl_Status statusUcl)
        {
            bool canApprove = true;
            if (CaseCtl().problemCase.CheckCaseStatus() < caseStep)
                canApprove = false;

            statusUcl.ButtonComplete.Enabled = canApprove;

            return canApprove;
        }

        public void SetPageMode()
        {
            bool visible = CaseCtl().PageMode == PageUseMode.ViewOnly ? false : true;

            trCase0OptionArea.Visible = trCase1OptionArea.Visible = trCase2OptionArea.Visible = trCase3OptionArea.Visible = trCase4OptionArea.Visible = trCase5OptionArea.Visible = trCase6OptionArea.Visible = trCase7OptionArea.Visible = trCase8OptionArea.Visible = visible;
            trCase1Notify.Visible = trCase8Notify.Visible = visible;
            uclRadAsyncUpload3.SetViewMode(visible);
            uclRadAsyncUpload5.SetViewMode(visible);
            uclRadAsyncUpload6.SetViewMode(visible);
        }

        private bool CanSave()
        {
            return CaseCtl().isNew || CaseCtl().problemCase.ProbCase.STATUS == "I" ? false : true;
        }

        #region case0
        public void BindCase0()
        {
            ToggleVisible(pnlCase0);
           // pnlIncidentSearch.Visible = false;
            pnlCase0.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
			isEhsCase = (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS");

            if (ddlCaseType.Items.Count == 0)
            {
                ddlCaseType.Items.AddRange(WebSiteCommon.PopulateRadListItems("incidentType", 0, "short"));
            }

            if (isEhsCase)
            {
                spAddIncident.Visible = false;
                ddlCaseType.SelectedIndex = 1;
				lblProbCase0Instruction.Visible = false;
				phCase0Form.Visible = false;
            }
            else
            {
                spAddIncident.Visible = true;
                ddlCaseType.SelectedIndex = 0;
            }

            SQMBasePage.DisplayControlValue(ddlCaseType, ddlCaseType.SelectedValue, PageUseMode.ViewOnly, "textStd");
            ddlCaseType.Visible = false;
            SQMBasePage.DisplayControlValue(tbCaseDesc, CaseCtl().problemCase.ProbCase.DESC_SHORT, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCaseDescLong, CaseCtl().problemCase.ProbCase.DESC_LONG , CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase0Program, CaseCtl().problemCase.ProbCase.AFFECTED_PROGRAM, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase0System, CaseCtl().problemCase.ProbCase.AFFECTED_SYSTEM, CaseCtl().PageMode, "textStd");

            uclIssueList.BindIncidentList(CaseCtl().problemCase.IncidentList);
            cbCaseInActive.Checked = CaseCtl().problemCase.ProbCase.STATUS == "I" ? true : false;
  
            uclStatus0.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("0", "C", 0));

			if (UserContext.GetMaxScopePrivilege(SysScope.incident) <= SysPriv.action && isEhsCase == false)
            {
                btnCase0Delete.Visible = btnCase0Delete.Enabled = true;
                phCase0InActive.Visible = true;
            }
            else
            {
                btnCase0Delete.Visible = btnCase0Delete.Enabled = false;
                phCase0InActive.Visible = false;
            }

            if (isEhsCase  &&  CaseCtl().problemCase.IncidentList.Count > 0)
            {
                trIncidentDetail1.Visible = true;
                uclIncidentDetails0.Refresh(CaseCtl().problemCase.IncidentList[0].INCIDENT_ID, new int[1] {0});
            }

            CaseCtl().problemCase.CreateStepCompleteTasks();
        }

        public int BindIncidentList(List<INCIDENT> incidentList)
        {
            int status = 0;

            uclIssueList.BindIncidentList(incidentList);
            if (incidentList.Count > 0)
                btnCase0Save.Enabled = true;

            return status;
        }

        public void BindPartIssueItemList(object partIssueItemList)
        {
            gvCasePartsList.DataSource = partIssueItemList;
            gvCasePartsList.DataBind();
        }

        public ProblemCase UpdateCase0(ProblemCase theCase)
        {
            pnlCase0.ViewStateMode = System.Web.UI.ViewStateMode.Enabled;
            if (string.IsNullOrEmpty(tbCaseDesc.Text)  ||  string.IsNullOrEmpty(tbCaseDescLong.Text))
            {
                theCase.UpdateStatus = CaseUpdateStatus.RequiredInputs;
                return theCase;
            }
            if (theCase.ProbCase.PROB_OCCUR.Count == 0)
            {
                theCase.UpdateStatus = CaseUpdateStatus.IncidentError;
                return theCase;
            }
            
            theCase.ProbCase.PROBCASE_TYPE = ddlCaseType.SelectedValue;
            theCase.ProbCase.DESC_SHORT = tbCaseDesc.Text;
            theCase.ProbCase.DESC_LONG = tbCaseDescLong.Text;
            theCase.ProbCase.AFFECTED_PROGRAM = tbCase0Program.Text;
            theCase.ProbCase.AFFECTED_SYSTEM = tbCase0System.Text;
            theCase.ProbCase.STATUS = cbCaseInActive.Checked == true ? "I" : "A";

            if (uclStatus0.UpdateTaskComplete(theCase.TeamTask.FindTask("0", "C", 0)))
            {
                theCase.StepComplete = "0";
                if (theCase.ProbCase.PROBCASE_TYPE == "EHS")
                {
                    theCase.TeamTask.SetTaskComplete("0", "C", 0, true);
                    uclStatus2.UpdateTaskComplete(theCase.TeamTask.FindTask("2", "C", 0));
                    theCase.StepComplete = "2";
                }
            }

            theCase.UpdateStatus = CaseUpdateStatus.Success;

            return theCase;
        }
        #endregion

        #region case1
        public void BindCase1(bool bNotify)
        {
            ToggleVisible(pnlCase1);
          
            if (isEhsCase)
                gvTeamList.DataSource = CaseCtl().problemCase.TeamTask.TaskList.Where(t => t.TASK_TYPE == "C" && t.TASK_STEP != "0" && t.TASK_STEP != "2").OrderBy(a => a.TASK_STEP).ToList();
            else
                gvTeamList.DataSource = CaseCtl().problemCase.TeamTask.TaskList.Where(t => t.TASK_TYPE == "C" && t.TASK_STEP != "0").OrderBy(a => a.TASK_STEP).ToList();
            gvTeamList.DataBind();

            if (bNotify)
                lblCase1NotifyConfirm.Visible = true;

            uclStatus1.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("1", "C", 0), hfHlpCase1Complete.Value);
            
            CheckTaskProgress(1, uclStatus1);

            cbCase1Notify.Checked = false;
            btnCase1Save.Enabled = CanSave();
        }

        public ProblemCase UpdateCase1(ProblemCase theCase, out bool bNotify)
        {
            bNotify = false;
            int responsibleAssigned = 0;
            HiddenField hf;
            RadDatePicker dtp;
            RadComboBox ddl;

            TASK_STATUS task = null;

            // validate team list
            int rowsComplete = 0;
            foreach (GridViewRow row in gvTeamList.Rows)
            {
                dtp = (RadDatePicker)row.FindControl("radDueDate");
                ddl = (RadComboBox)row.FindControl("ddlResponsible");
                if ((dtp.SelectedDate == null && !string.IsNullOrEmpty(ddl.SelectedValue)) || (dtp.SelectedDate != null && string.IsNullOrEmpty(ddl.SelectedValue)))
                {
                    theCase.UpdateStatus = CaseUpdateStatus.Incomplete;
                    return theCase;
                }
                if (dtp.SelectedDate != null  &&  !string.IsNullOrEmpty(ddl.SelectedValue))
                    ++rowsComplete;
            }
            if (uclStatus1.ButtonComplete.Checked && rowsComplete < 7)
            {
                theCase.UpdateStatus = CaseUpdateStatus.CompleteError;
                return theCase;
            }


            foreach (GridViewRow row in gvTeamList.Rows)
            {
                Label lbl = (Label)row.FindControl("lblTaskDesc");
                hf = (HiddenField)row.FindControl("hfTaskStep");
                if ((task = theCase.TeamTask.FindTask(hf.Value, "C", 0)) == null)
                    theCase.TeamTask.CreateTask(hf.Value, "C", 0, lbl.Text, DateTime.MinValue, 0);
                if (task != null)
                {
                    dtp = (RadDatePicker)row.FindControl("radDueDate");
                    ddl = (RadComboBox)row.FindControl("ddlResponsible");
                    if (dtp.SelectedDate != null && !string.IsNullOrEmpty(ddl.SelectedValue))  // valid task
                    {
                        theCase.TeamTask.UpdateTask(task, (DateTime)dtp.SelectedDate, Convert.ToDecimal(ddl.SelectedValue), lbl.Text);
                        theCase.TeamTask.UpdateTaskStatus(task,TaskMgr.CalculateTaskStatus(task));
                    }
                }
            }

            if (uclStatus1.UpdateTaskComplete(theCase.TeamTask.FindTask("1", "C", 0)))
            {
                theCase.StepComplete = "1";
            }

            bNotify = cbCase1Notify.Checked == true && uclStatus1.ButtonComplete.Checked == true ? true : false;

            return theCase;
        }

        public void gvTeamList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                bool canEdit = true;
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();
                try
                {
                    Label lbl;
                    HiddenField hf;
                    
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskStep");
                    lbl = (Label)e.Row.Cells[0].FindControl("lblTaskDesc");
                    string desc = WebSiteCommon.GetXlatValueLong("caseStep", hf.Value);
                    if (!string.IsNullOrEmpty(desc))
                        lbl.Text = desc;

                    RadDatePicker dtp = (RadDatePicker)e.Row.Cells[0].FindControl("radDueDate");
                    dtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                    dtp.MinDate = SetMinDate(CaseCtl().problemCase);
                    dtp.SelectedDate = null;
                    dtp.ShowPopupOnFocus = true;

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfResponsible");
                    RadComboBox ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlResponsible");
                    SetPersonList(ddl, CaseCtl().PersonSelectList(), "");
                    if (ddl.Items.FindItemByValue(hf.Value) != null)
                    {
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");
                    }

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfCompleteDate");
                    if (!string.IsNullOrEmpty(hf.Value))
                    {
                        lbl = (Label)e.Row.Cells[0].FindControl("lblCompleteDate");
                        lbl.Text = SQMBasePage.FormatDate(Convert.ToDateTime(hf.Value), "d", false);
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfCompleteBy");
                    }

                    RadDatePicker ddp = (RadDatePicker)e.Row.Cells[0].FindControl("radDueDate");
                    if (canEdit == false)
                    {
                        ddp.DatePopupButton.Visible = false;
                        dtp.DateInput.ReadOnly = true;
                    }

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfDueDate");
                    SQMBasePage.DisplayControlValue(dtp, hf.Value, CaseCtl().PageMode, "textStd");
                    TaskStatus status = TaskStatus.Pending;
                    if (dtp.SelectedDate != null)
                    {
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfTaskStatus");
                        lbl = (Label)e.Row.Cells[0].FindControl("LblTaskStatus");
                        status = (TaskStatus)Convert.ToInt32(hf.Value);
                        lbl.Text = status.ToString();
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

        #region case2

        public void BindCase2()
        {
            ToggleVisible(pnlCase2);

            // mt - testing display of the originating quality issue in lieu of the what is/is not inputs
            // we will use the what is/ is not inputs for QI problem cases but derivation of the logic below for EHS cases
			if (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "QI")
			{
                phCase2.Visible = true;
                phCase2EHS.Visible = false;

				PROB_DEFINE caseDefine = new PROB_DEFINE();
				CaseCtl().problemCase.Define = CaseCtl().problemCase.UpdateProblemDefinition(caseDefine);
				lblCaseDefineWhoDat1.Text = caseDefine.WHO_IS; imbWhoDat1.Visible = String.IsNullOrEmpty(caseDefine.WHO_IS) == true ? false : true;
				lblCaseDefineWhoDat2.Text = caseDefine.IMPACT_IS; imbWhoDat2.Visible = String.IsNullOrEmpty(caseDefine.IMPACT_IS) == true ? false : true;
				lblCaseDefineWhatDat1.Text = caseDefine.WHAT_IS; imbWhatDat1.Visible = String.IsNullOrEmpty(caseDefine.WHAT_IS) == true ? false : true;
				lblCaseDefineWhatDat2.Text = caseDefine.NC_IS; imbWhatDat2.Visible = String.IsNullOrEmpty(caseDefine.NC_IS) == true ? false : true;
				lblCaseDefineWhereDat1.Text = caseDefine.WHERE_IS; imbWhereDat1.Visible = String.IsNullOrEmpty(caseDefine.WHERE_IS) == true ? false : true;
				lblCaseDefineWhereDat2.Text = caseDefine.DETECTED_IS; imbWhereDat2.Visible = String.IsNullOrEmpty(caseDefine.DETECTED_IS) == true ? false : true;
				lblCaseDefineWhenDat1.Text = CaseCtl().problemCase.Define.WHEN_IS; imbWhenDat1.Visible = String.IsNullOrEmpty(caseDefine.WHEN_IS) == true ? false : true;
				lblCaseDefineWhyDat1.Text = caseDefine.WHY_IS; imbWhyDat1.Visible = String.IsNullOrEmpty(caseDefine.WHY_IS) == true ? false : true;
				lblCaseDefineWhyDat2.Text = caseDefine.URGENT_IS; imbWhyDat2.Visible = String.IsNullOrEmpty(caseDefine.URGENT_IS) == true ? false : true;
				lblCaseDefineHowDat1.Text = caseDefine.OFTEN_IS; imbHowDat1.Visible = String.IsNullOrEmpty(caseDefine.OFTEN_IS) == true ? false : true;
				lblCaseDefineHowDat2.Text = caseDefine.MEASURE_IS; imbHowDat2.Visible = String.IsNullOrEmpty(caseDefine.MEASURE_IS) == true ? false : true;

				if (CaseCtl().problemCase.ProbCase.PROB_DEFINE == null)
					CaseCtl().problemCase.CreateProblemDefinition();

                SQMBasePage.DisplayControlValue(tbCaseDefineWhoIs1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHO_IS, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhoIs2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.IMPACT_IS, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhatIs1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHAT_IS, CaseCtl().PageMode, "textStd");
                 SQMBasePage.DisplayControlValue(tbCaseDefineWhatIs2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.NC_IS, CaseCtl().PageMode, "textStd");
                 SQMBasePage.DisplayControlValue(tbCaseDefineWhereIs1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHERE_IS, CaseCtl().PageMode, "textStd");
                 SQMBasePage.DisplayControlValue(tbCaseDefineWhereIs2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.DETECTED_IS, CaseCtl().PageMode, "textStd");
                 SQMBasePage.DisplayControlValue(tbCaseDefineWhenIs1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHEN_IS, CaseCtl().PageMode, "textStd");
                 SQMBasePage.DisplayControlValue(tbCaseDefineWhyIs1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHY_IS, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhyIs2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.URGENT_IS, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineHowIs1,  CaseCtl().problemCase.ProbCase.PROB_DEFINE.OFTEN_IS, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineHowIs2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.MEASURE_IS, CaseCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbCaseDefineWhoIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHO_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhoIsNot2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.IMPACT_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhatIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHAT_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhatIsNot2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.NC_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhereIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHERE_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhereIsNot2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.DETECTED_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhenIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHEN_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhyIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.WHY_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineWhyIsNot2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.URGENT_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineHowIsNot1, CaseCtl().problemCase.ProbCase.PROB_DEFINE.OFTEN_NOT, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(tbCaseDefineHowIsNot2, CaseCtl().problemCase.ProbCase.PROB_DEFINE.MEASURE_NOT, CaseCtl().PageMode, "textStd");

                SQMBasePage.DisplayControlValue(tbCaseDefineSummary, CaseCtl().problemCase.ProbCase.PROB_DEFINE.PROBLEM_SUMMARY, CaseCtl().PageMode, "textStd");

			}
			else if (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS")
			{
                phCase2.Visible = false;
                phCase2EHS.Visible = true;
 
				PROB_DEFINE caseDefine = new PROB_DEFINE();
				
				CaseCtl().problemCase.Define = CaseCtl().problemCase.UpdateProblemDefinition(caseDefine);
				if (CaseCtl().problemCase.ProbCase.PROB_DEFINE == null)
					CaseCtl().problemCase.CreateProblemDefinition();

				if (CaseCtl().problemCase.IncidentList.Count > 0)
				{
					//uclIncidentDetails.HideProblemFields = true;
                    uclIncidentDetails.Refresh(CaseCtl().problemCase.IncidentList[0].INCIDENT_ID, new int[1] {0});
				}
			}

            uclStatus2.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("2", "C", 0));
            CheckTaskProgress(2, uclStatus2);
           
            btnCase2Save.Enabled = CanSave();
        }

        public ProblemCase UpdateCase2(ProblemCase theCase)
        {
			if (theCase.ProbCase.PROBCASE_TYPE == "QI")
			{
				theCase.ProbCase.PROB_DEFINE.WHO_IS = tbCaseDefineWhoIs1.Text;
				theCase.ProbCase.PROB_DEFINE.WHO_NOT = tbCaseDefineWhoIsNot1.Text;
				theCase.ProbCase.PROB_DEFINE.IMPACT_IS = tbCaseDefineWhoIs2.Text;
				theCase.ProbCase.PROB_DEFINE.IMPACT_NOT = tbCaseDefineWhoIsNot2.Text;
				theCase.ProbCase.PROB_DEFINE.WHAT_IS = tbCaseDefineWhatIs1.Text;
				theCase.ProbCase.PROB_DEFINE.WHAT_NOT = tbCaseDefineWhatIsNot1.Text;
				theCase.ProbCase.PROB_DEFINE.NC_IS = tbCaseDefineWhatIs2.Text;
				theCase.ProbCase.PROB_DEFINE.NC_NOT = tbCaseDefineWhatIsNot2.Text;
				theCase.ProbCase.PROB_DEFINE.WHERE_IS = tbCaseDefineWhereIs1.Text;
				theCase.ProbCase.PROB_DEFINE.WHERE_NOT = tbCaseDefineWhereIsNot1.Text;
				theCase.ProbCase.PROB_DEFINE.DETECTED_IS = tbCaseDefineWhereIs2.Text;
				theCase.ProbCase.PROB_DEFINE.DETECTED_NOT = tbCaseDefineWhereIsNot2.Text;
				theCase.ProbCase.PROB_DEFINE.WHY_IS = tbCaseDefineWhyIs1.Text;
				theCase.ProbCase.PROB_DEFINE.WHY_NOT = tbCaseDefineWhyIsNot1.Text;
				theCase.ProbCase.PROB_DEFINE.URGENT_IS = tbCaseDefineWhyIs2.Text;
				theCase.ProbCase.PROB_DEFINE.URGENT_NOT = tbCaseDefineWhyIsNot2.Text;
				theCase.ProbCase.PROB_DEFINE.OFTEN_IS = tbCaseDefineHowIs1.Text;
				theCase.ProbCase.PROB_DEFINE.OFTEN_NOT = tbCaseDefineHowIsNot1.Text;
				theCase.ProbCase.PROB_DEFINE.MEASURE_IS = tbCaseDefineHowIs2.Text;
				theCase.ProbCase.PROB_DEFINE.MEASURE_NOT = tbCaseDefineHowIsNot2.Text;

				theCase.ProbCase.PROB_DEFINE.ADDED_INFO = tbCaseDefineOther.Text;
				theCase.ProbCase.PROB_DEFINE.PROBLEM_SUMMARY = tbCaseDefineSummary.Text;

                if (uclStatus2.UpdateTaskComplete(theCase.TeamTask.FindTask("2", "C", 0)))
					theCase.StepComplete = "2";
			}
			else if (theCase.ProbCase.PROBCASE_TYPE == "EHS")
			{
				// theCase.StepComplete = "2";
			}

            return theCase;
        }

        #endregion

        #region case3
        public void BindCase3()
        {
            ToggleVisible(pnlCase3);
          
            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
            {
                gvContainList.GridLines = GridLines.Both;
                gvContainList.CssClass = "Grid";
            }

            if (CaseCtl().problemCase.ProbCase.PROB_CONTAIN == null)
                CaseCtl().problemCase.CreateProblemContainment();
            if (CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION == null || CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Count == 0)
                CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Add(new PROB_CONTAIN_ACTION());

            if (isEhsCase)
            {
                phCase3Initial.Visible = false;
            }
            else
            {
                SQMBasePage.DisplayControlValue(tbCase3InitialContain, CaseCtl().problemCase.ProbCase.PROB_CONTAIN.INITIAL_ACTION, CaseCtl().PageMode, "textStd");
            }

            SQMBasePage.DisplayControlValue(tbCase3InitialResult, CaseCtl().problemCase.ProbCase.PROB_CONTAIN.INITIAL_RESULTS, CaseCtl().PageMode, "textStd");
         
            gvContainList.DataSource = CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.OrderBy(l=> l.ITEM_SEQ).ToList();
            gvContainList.DataBind();

            uclStatus3.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("3", "C", 0), hfHlpCase3Complete.Value);
            CheckTaskProgress(3, uclStatus3);
           
            btnCase3Save2.Enabled = CanSave();

           
            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
            {
                Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
                uclRadAsyncUpload3.Parent.Controls.AddAt(uclRadAsyncUpload3.Parent.Controls.IndexOf(uclRadAsyncUpload3), attch);
                attch.BindDisplayAttachments(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "3", 0);
                uclRadAsyncUpload3.Visible = false;
            }
            else
            {
                uclRadAsyncUpload3.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "3");
            }

            staticStep = "3";
        }

        public void gvContainList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                HiddenField hf;
                Label lbl;
                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbContainAction");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfContainDueDate");
                    RadDatePicker rd = (RadDatePicker)e.Row.Cells[0].FindControl("radContainDueDate");
                    rd.MinDate = SetMinDate(CaseCtl().problemCase);
                    rd.ShowPopupOnFocus = true;
                    SQMBasePage.DisplayControlValue(rd, hf.Value, CaseCtl().PageMode, "textStd");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfContainResponsible");
                    RadComboBox ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlContainResponsible");
                    SetPersonList(ddl, CaseCtl().PersonSelectList(), "");
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");
                   
                    ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlContainStatus");
                    ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems("taskStatus", 2, "short"));
                    ddl.Items.Insert(0, new RadComboBoxItem("", ""));
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfContainStatus");
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "refText");
                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }

        public PROB_CONTAIN_ACTION AddCase3ActionItem()
        {
            SaveUploadedFiles(CaseCtl().problemCase, "3");
            uclRadAsyncUpload3.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "3");

            PROB_CONTAIN_ACTION action = null;
            List<PROB_CONTAIN_ACTION> actionList = new List<PROB_CONTAIN_ACTION>();
            foreach (GridViewRow row in gvContainList.Rows)
            {
                action = new PROB_CONTAIN_ACTION();
                TextBox tb = (TextBox)row.FindControl("tbContainAction");
                action.ACTION_ITEM = tb.Text;
                RadDatePicker dp = (RadDatePicker)row.FindControl("radContainDueDate");
                if (dp.SelectedDate != null)
                    action.DUE_DT = dp.SelectedDate;
                RadComboBox ddl = (RadComboBox)row.FindControl("ddlContainStatus");
                action.STATUS = ddl.SelectedValue;
                ddl = (RadComboBox)row.FindControl("ddlContainResponsible");
                if (!string.IsNullOrEmpty(ddl.SelectedValue))
                    action.RESPONSIBLE1_PERSON = Convert.ToDecimal(ddl.SelectedValue);
                actionList.Add(action);
            }

            action = new PROB_CONTAIN_ACTION();
            actionList.Add(action);
            gvContainList.DataSource = actionList;
            gvContainList.DataBind();

            if (gvContainList.Rows.Count > 0)
                gvContainList.Rows[gvContainList.Rows.Count - 1].FindControl("tbContainAction").Focus();

            return action;
        }

        public ProblemCase UpdateCase3(ProblemCase problemCase)
        {
            TextBox tb;
            RadDatePicker rd;
            RadComboBox ddl, ddl0;

            if (uclRadAsyncUpload3.RAUpload.UploadedFiles.Count > 0)
            {
                SaveUploadedFiles(problemCase, "3");
                uclRadAsyncUpload3.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "3");
            }

            foreach (GridViewRow arow in gvContainList.Rows)
            {
                ddl0 = (RadComboBox)arow.FindControl("ddlContainStatus");
                tb = (TextBox)arow.FindControl("tbContainAction");
                rd = (RadDatePicker)arow.FindControl("radContainDueDate");
                ddl = (RadComboBox)arow.FindControl("ddlContainResponsible");

                if (rd.SelectedDate == null && string.IsNullOrEmpty(ddl0.SelectedValue + ddl.SelectedValue + tb.Text))
                {
                    ;   // entire row is blank - we won't save this 
                }
                else if (string.IsNullOrEmpty(tb.Text) || rd.SelectedDate == null || string.IsNullOrEmpty(ddl0.SelectedValue)  || string.IsNullOrEmpty(ddl.SelectedValue))
                {
                    CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.Incomplete;
                    return problemCase;
                }
            }

            CaseCtl().problemCase.ProbCase.PROB_CONTAIN.INITIAL_RESULTS = tbCase3InitialResult.Text;

            if (!isEhsCase)
            {
                CaseCtl().problemCase.ProbCase.PROB_CONTAIN.INITIAL_ACTION = tbCase3InitialContain.Text;
            }

            if (CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION != null)
                CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Clear();

            int actionNo = 0;
            foreach (GridViewRow arow in gvContainList.Rows)
            {
                TASK_STATUS task = CaseCtl().problemCase.TeamTask.FindTask("3", "T", actionNo+1);
                ddl0 = (RadComboBox)arow.FindControl("ddlContainStatus");
                if (!string.IsNullOrEmpty(ddl0.SelectedValue))
                {
                    PROB_CONTAIN_ACTION action = new PROB_CONTAIN_ACTION();
                    action.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                    action.STATUS = ddl0.SelectedValue;

                    tb = (TextBox)arow.FindControl("tbContainAction");
                    action.ACTION_ITEM = tb.Text;
                    action.ITEM_SEQ = actionNo++;

                    rd = (RadDatePicker)arow.FindControl("radContainDueDate");
                    if (rd.SelectedDate != null)
                        action.DUE_DT = (DateTime)rd.SelectedDate;
                    else
                        action.DUE_DT = null;

                    ddl = (RadComboBox)arow.FindControl("ddlContainResponsible");
                    if (!string.IsNullOrEmpty(ddl.SelectedValue))
                        action.RESPONSIBLE1_PERSON = Convert.ToDecimal(ddl.SelectedValue);
                    else
                        action.RESPONSIBLE1_PERSON = null;

                    CaseCtl().problemCase.ProbCase.PROB_CONTAIN.PROB_CONTAIN_ACTION.Add(action);

                    if (task == null)
                    {
                        task = CaseCtl().problemCase.TeamTask.CreateTask("3", "T", actionNo, action.ACTION_ITEM, (DateTime)action.DUE_DT, (decimal)action.RESPONSIBLE1_PERSON);
                        task.STATUS = ((int)TaskMgr.CalculateTaskStatus(task)).ToString();
                    }
                    else
                    {
                        task = CaseCtl().problemCase.TeamTask.UpdateTask(task, (DateTime)action.DUE_DT, (decimal)action.RESPONSIBLE1_PERSON, action.ACTION_ITEM);
                    }
                }

                if ((string.IsNullOrEmpty(ddl0.SelectedValue) || ddl0.SelectedValue == "C" || ddl0.SelectedValue == "D") && task != null)
                    task.STATUS = ((int)TaskStatus.Delete).ToString();
            }

            if (uclStatus3.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("3", "C", 0)))
                CaseCtl().problemCase.StepComplete = "3";
 
            return problemCase;
        }

        #endregion

        #region case4
        public void BindCase4()
        {
            ToggleVisible(pnlCase4);

            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
            {
                gvCauseStepList.GridLines = GridLines.Both;
                gvCauseStepList.CssClass = "Grid";
            }

            if (CaseCtl().problemCase.ProbCase.PROB_CAUSE == null) 
                CaseCtl().problemCase.CreateProblemRootCause();

            if (string.IsNullOrEmpty(CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROBLEM_IS) && CaseCtl().problemCase.ProbCase.PROB_DEFINE != null)
                CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROBLEM_IS = CaseCtl().problemCase.ProbCase.PROB_DEFINE.PROBLEM_SUMMARY;

            if (CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP != null  &&  CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Count == 0)
            {   // add some steps 
                for (int n = 0; n < 2; n++)
                {
                    PROB_CAUSE_STEP step = new PROB_CAUSE_STEP();
                    step.ITERATION_NO = n + 1;
                    step.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROBCASE_ID;
                    CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Add(step);
                }
            }

            SQMBasePage.DisplayControlValue(tbCase4Problem, CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROBLEM_IS, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase4Comments, CaseCtl().problemCase.ProbCase.PROB_CAUSE.COMMENTS, CaseCtl().PageMode, "textStd");

            uclStatus4.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("4", "C", 0), hfHlpCase4Complete.Value);
            CheckTaskProgress(4, uclStatus4);
           
            btnCase4Save2.Enabled = CanSave();

            gvCauseStepList.DataSource = CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.OrderBy(s=> s.ITERATION_NO);
            gvCauseStepList.DataBind();
        }

        private void AddCauseStep()
        {
            UpdateCase4(null, false);  // save the existing rows

            PROB_CAUSE_STEP step = new PROB_CAUSE_STEP();
            step.ITERATION_NO = causeStepList.Count+1;
            causeStepList.Add(step);

            gvCauseStepList.DataSource = causeStepList.OrderBy(s => s.ITERATION_NO);
            gvCauseStepList.DataBind();
            gvCauseStepList.Rows[gvCauseStepList.Rows.Count - 1].FindControl("tbWhyOccur").Focus();
        }

        public ProblemCase UpdateCase4(ProblemCase problemCase)
        {
            return UpdateCase4(problemCase, true);
        }
        public ProblemCase UpdateCase4(ProblemCase problemCase, bool updateAndSave)
        {
            HiddenField hf;
            TextBox tb;
            RadComboBox ddl;

            // validate prior to save
            if (updateAndSave)
            {
                if (string.IsNullOrEmpty(tbCase4Problem.Text))
                {
                    CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.RequiredInputs;
                    return problemCase;
                }
                int rootCauseCount = 0;
                foreach (GridViewRow row in gvCauseStepList.Rows)
                {
                    ddl = (RadComboBox)row.FindControl("ddlCauseType");
                    if (!string.IsNullOrEmpty(ddl.SelectedValue))      // indicated as a root cause
                    {
                        ++rootCauseCount;
                        tb = (TextBox)row.FindControl("tbWhyOccur");
                        TextBox tba = (TextBox)row.FindControl("tbHowConfirmed");
                        if (string.IsNullOrEmpty(tb.Text) || string.IsNullOrEmpty(tba.Text))    // root cause needs 'why' and 'answer' fields to be legit
                        {
                            CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.Incomplete;
                        }
                    }
                }

                if (CaseCtl().problemCase.UpdateStatus != CaseUpdateStatus.Incomplete  &&  uclStatus4.ButtonComplete.Checked  &&  rootCauseCount < 1)
                    CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.RootCauseError;

                if (CaseCtl().problemCase.UpdateStatus != CaseUpdateStatus.Success)
                    return problemCase;
            }

            if (causeStepList == null)
                causeStepList = new List<PROB_CAUSE_STEP>();
            causeStepList.Clear();

            int stepNo = 0;
            foreach (GridViewRow row in gvCauseStepList.Rows)
            {
                tb = (TextBox)row.FindControl("tbWhyOccur");
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    PROB_CAUSE_STEP step = new PROB_CAUSE_STEP();
                    step.ITERATION_NO = ++stepNo;
                    step.WHY_OCCUR = tb.Text;

                    tb = (TextBox)row.FindControl("tbHowConfirmed");
                    step.HOW_CONFIRMED = tb.Text;

                    ddl = (RadComboBox)row.FindControl("ddlCauseType");
                    step.CAUSE_TYPE = ddl.SelectedValue;
                    if (!string.IsNullOrEmpty(ddl.SelectedValue))
                    {
                        step.IS_ROOTCAUSE = true;
                    }
                    causeStepList.Add(step);
                }
            }

            if (updateAndSave)
            {
                CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROBLEM_IS = tbCase4Problem.Text;
                CaseCtl().problemCase.ProbCase.PROB_CAUSE.COMMENTS = tbCase4Comments.Text;
                CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Clear();
                foreach (PROB_CAUSE_STEP step in causeStepList)
                {
                    CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Add(step);
                }

                if (uclStatus4.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("4", "C", 0)))
                    CaseCtl().problemCase.StepComplete = "4";
            }

            return problemCase;
        }

        public void gvCauseStepList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();
                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbWhyOccur");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");
                    tb = (TextBox)e.Row.Cells[0].FindControl("tbHowConfirmed");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");

                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfCauseStep");
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfCauseType");

                    Image img = (Image)e.Row.Cells[0].FindControl("imgRootCause");
                    if (!string.IsNullOrEmpty(hf.Value))
                        img.Visible = true;
                    else
                        img.Visible = false;

                    RadComboBox ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlCauseType");
                    ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems("causeTypeEHS", 2, "short"));
                    ddl.Items.Insert(0, new RadComboBoxItem("", ""));
                    if (!string.IsNullOrEmpty(hf.Value) && CaseCtl().PageMode == PageUseMode.ViewOnly)
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd", "<hr />");
                    else
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");
                }
                catch
                {
                }
            }
        }
        #endregion

        #region case5
        public void BindCase5()
        {
            ToggleVisible(pnlCase5);
            int probCauseCount = 0;
          
			isEhsCase = (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS");
            btnCase5Save2.Enabled = false;

            if (ddlCase5riskSeverity1.Items.Count == 0)
            {
                ddlCase5riskSeverity1.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
                ddlCase5riskSeverity2.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
                ddlCase5riskOccur1.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
                ddlCase5riskOccur2.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
                ddlCase5riskDetect1.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
                ddlCase5riskDetect2.Items.AddRange(WebSiteCommon.PopulateDropDownListNums(1, 10));
            }

            // correlate actions with causes.  todo: reconcile deleted causes if/when that occurs in step 4
            List<SQM.Shared.CorrectiveAction> actionList = new List<SQM.Shared.CorrectiveAction>();
            if (CaseCtl().problemCase.ProbCase.PROB_CAUSE != null && CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP != null)
            {
                foreach (PROB_CAUSE_STEP cause in CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Where(c => c.IS_ROOTCAUSE == true).ToList())
                {
                    if (CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION != null && CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.Where(l=> l.CAUSE_NO == cause.ITERATION_NO).ToList().Count == 0)
                    {
                        // add a corrective action if empty
                        PROB_CAUSE_ACTION action = new PROB_CAUSE_ACTION();
                        action.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                        action.CAUSE_NO = cause.ITERATION_NO;
                        CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.Add(action);
                    }
                }
                rptCauseAction.DataSource = CaseCtl().problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Where(c => c.IS_ROOTCAUSE == true).ToList();
                rptCauseAction.DataBind();

                if (CanSave())
                    btnCase5Save2.Enabled = true;
            }

            if (CaseCtl().problemCase.ProbCase.PROB_RISK != null)
            {
                SQMBasePage.DisplayControlValue(tbCase5riskState1, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_STATE, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(ddlCase5riskSeverity1, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_SEVERITY.ToString(), CaseCtl().PageMode, "prompt");
                SQMBasePage.DisplayControlValue(ddlCase5riskOccur1, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_OCCUR.ToString(), CaseCtl().PageMode, "prompt");
                SQMBasePage.DisplayControlValue(ddlCase5riskDetect1, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_DETECT.ToString(), CaseCtl().PageMode, "prompt");
                lblCase5riskIndex1.Text = hfCase5riskIndex1.Value = CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_INDEX.ToString();

                SQMBasePage.DisplayControlValue(tbCase5riskState2, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_STATE, CaseCtl().PageMode, "textStd");
                SQMBasePage.DisplayControlValue(ddlCase5riskSeverity2, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_SEVERITY.ToString(), CaseCtl().PageMode, "prompt");
                SQMBasePage.DisplayControlValue(ddlCase5riskOccur2, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_OCCUR.ToString(), CaseCtl().PageMode, "prompt");
                SQMBasePage.DisplayControlValue(ddlCase5riskDetect2, CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_DETECT.ToString(), CaseCtl().PageMode, "prompt");
                lblCase5riskIndex2.Text = hfCase5riskIndex2.Value = CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_INDEX.ToString();

                SQMBasePage.DisplayControlValue(tbCase3OtherRisk, CaseCtl().problemCase.ProbCase.PROB_RISK.OTHER_RISK, CaseCtl().PageMode, "textStd");
            }

            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
            {
                Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
                uclRadAsyncUpload5.Parent.Controls.AddAt(uclRadAsyncUpload5.Parent.Controls.IndexOf(uclRadAsyncUpload5), attch);
                attch.BindDisplayAttachments(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "5", 0);
                uclRadAsyncUpload5.Visible = false;
            }
            else
            {
                uclRadAsyncUpload5.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "5");
            }

            uclStatus5.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("5", "C", 0), hfHlpCase5Complete.Value);
            CheckTaskProgress(5, uclStatus5);
        }

        public void rptCauseAction_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    PROB_CAUSE_STEP rootCause = (PROB_CAUSE_STEP)e.Item.DataItem;
                    TextBox tb = (TextBox)e.Item.FindControl("tbRelatedCause");
                    SQMBasePage.DisplayControlValue(tb, rootCause.HOW_CONFIRMED, CaseCtl().PageMode, "textStd");
                    HiddenField hf = (HiddenField)e.Item.FindControl("hfRelatedCause");
                    hf.Value = rootCause.ITERATION_NO.ToString();
                    Button btn = (Button)e.Item.FindControl("btnCase5AddAction");
                    btn.CommandArgument = hf.Value;

                    GridView gv = (GridView)e.Item.FindControl("gvActionList");
					gv.Columns[2].HeaderText = "Due Date /<br>" + Resources.LocalizedText.Status;
                    if (CaseCtl().PageMode == PageUseMode.ViewOnly)
                    {
                        gv.GridLines = GridLines.Both;
                        gv.CssClass = "Grid";
                    }
                    gv.DataSource = CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.Where(l => l.CAUSE_NO == rootCause.ITERATION_NO).OrderBy(l => l.ACTION_NO).ToList();
                    gv.DataBind();
                }
                catch { }
            }
        }

        private void AddCorrectiveAction(string causeNum)
        {
            SaveUploadedFiles(CaseCtl().problemCase, "5");
            uclRadAsyncUpload5.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "5");
           
            lblCase5riskIndex1.Text = hfCase5riskIndex1.Value;
            lblCase5riskIndex2.Text = hfCase5riskIndex2.Value;
 
            PROB_CAUSE_ACTION action = null;
            List<PROB_CAUSE_ACTION> actionList = new List<PROB_CAUSE_ACTION>();

            foreach (RepeaterItem rt in rptCauseAction.Items)
            {
                // save the entered actions so we don't loose them on the postback
                HiddenField hfCause = (HiddenField)rt.FindControl("hfRelatedCause");
                GridView gv = (GridView)rt.FindControl("gvActionList");
                foreach (GridViewRow row in gv.Rows)
                {
                    action = new PROB_CAUSE_ACTION();
                    if (!string.IsNullOrEmpty(hfCause.Value))
                        action.CAUSE_NO = Convert.ToInt32(hfCause.Value);
                    TextBox tb = (TextBox)row.FindControl("tbCorrectiveAction");
                    action.ACTION_DESC = tb.Text;
                    RadDatePicker dp = (RadDatePicker)row.FindControl("radActionDueDate");
                    if (dp.SelectedDate != null)
                        action.EFF_DT = dp.SelectedDate;
                    RadComboBox ddl = (RadComboBox)row.FindControl("ddlActionStatus");
                    action.STATUS = ddl.SelectedValue;
                    ddl = (RadComboBox)row.FindControl("ddlActionResponsible");
                    if (!string.IsNullOrEmpty(ddl.SelectedValue))
                        action.RESPONSIBLE1_PERSON = Convert.ToDecimal(ddl.SelectedValue);
                    ddl = (RadComboBox)row.FindControl("ddlPPPType");
                    if (!string.IsNullOrEmpty(ddl.SelectedValue))
                        action.PPP_TYPE = ddl.SelectedValue;
                    actionList.Add(action);
                }

               
                if (!string.IsNullOrEmpty(hfCause.Value) && hfCause.Value == causeNum)
                {
                    action = new PROB_CAUSE_ACTION();
                    action.CAUSE_NO = Convert.ToInt32(hfCause.Value);
                    action.ACTION_NO = actionList.Count;
                    actionList.Add(action);
                    gv.DataSource = actionList.Where(l=> l.CAUSE_NO == action.CAUSE_NO).OrderBy(l => l.ACTION_NO).ToList();
                    gv.DataBind();

                    if (gv.Rows.Count > 0)
                        gv.Rows[gv.Rows.Count - 1].FindControl("tbCorrectiveAction").Focus();
                }

            }
        }

        public ProblemCase UpdateCase5(ProblemCase problemCase)
        {
            return UpdateCase5(problemCase, true);
        }

        public ProblemCase UpdateCase5(ProblemCase problemCase, bool updateAndSave)
        {
            TextBox tb;
            HiddenField hf, hfCause;
            RadDatePicker rd;
            RadComboBox ddl, ddl0;

            if (uclRadAsyncUpload5.RAUpload.UploadedFiles.Count > 0)
            {
                SaveUploadedFiles(problemCase, "5");
                uclRadAsyncUpload5.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "5");
            }

            if (!string.IsNullOrEmpty(ddlCase5riskSeverity1.SelectedValue))
            {
                lblCase5riskIndex1.Text = (Convert.ToInt32(ddlCase5riskSeverity1.SelectedValue) * Convert.ToInt32(ddlCase5riskOccur1.SelectedValue) * Convert.ToInt32(ddlCase5riskDetect1.SelectedValue)).ToString();
                lblCase5riskIndex2.Text = (Convert.ToInt32(ddlCase5riskSeverity2.SelectedValue) * Convert.ToInt32(ddlCase5riskOccur2.SelectedValue) * Convert.ToInt32(ddlCase5riskDetect2.SelectedValue)).ToString();
            }

            foreach (RepeaterItem rt in rptCauseAction.Items)
            {
                // validate the action entries
                GridView gv = (GridView)rt.FindControl("gvActionList");
                foreach (GridViewRow row in gv.Rows)
                {
                    ddl0 = (RadComboBox)row.FindControl("ddlActionStatus");
                    tb = (TextBox)row.FindControl("tbCorrectiveAction");
                    rd = (RadDatePicker)row.FindControl("radActionDueDate");
                    ddl = (RadComboBox)row.FindControl("ddlActionResponsible");
                    RadComboBox ddlp = (RadComboBox)row.FindControl("ddlPPPType");

                    if (rd.SelectedDate == null && string.IsNullOrEmpty(ddl0.SelectedValue + ddl.SelectedValue + tb.Text))
                    {
                        ;   // entire row is blank - we won't save this 
                    }
                    else if (string.IsNullOrEmpty(tb.Text) || rd.SelectedDate == null || string.IsNullOrEmpty(ddl0.SelectedValue) || string.IsNullOrEmpty(ddl.SelectedValue) || string.IsNullOrEmpty(ddlp.SelectedValue))
                    {
                        CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.Incomplete;
                        return problemCase;
                    }
                }
            }

            if (CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION != null)
                CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.Clear();

            int actionNo = 0;
            foreach (RepeaterItem rt in rptCauseAction.Items)
            {
                // validate the action entries
                hfCause = (HiddenField)rt.FindControl("hfRelatedCause");
                GridView gv = (GridView)rt.FindControl("gvActionList");
                foreach (GridViewRow arow in gv.Rows)
                {
                    TASK_STATUS task = CaseCtl().problemCase.TeamTask.FindTask("5", "T", actionNo + 1);
                    ddl0 = (RadComboBox)arow.FindControl("ddlActionStatus");
                    if (!string.IsNullOrEmpty(ddl0.SelectedValue))
                    {
                        PROB_CAUSE_ACTION action = new PROB_CAUSE_ACTION();
                        action.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                        action.CAUSE_NO = Convert.ToInt32(hfCause.Value);
                        action.STATUS = ddl0.SelectedValue;

                        tb = (TextBox)arow.FindControl("tbCorrectiveAction");
                        action.ACTION_DESC = tb.Text;
                        action.ACTION_NO = actionNo++;

                        rd = (RadDatePicker)arow.FindControl("radActionDueDate");
                        if (rd.SelectedDate != null)
                            action.EFF_DT = (DateTime)rd.SelectedDate;
                        else
                            action.EFF_DT = null;

                        ddl = (RadComboBox)arow.FindControl("ddlActionResponsible");
                        if (!string.IsNullOrEmpty(ddl.SelectedValue))
                            action.RESPONSIBLE1_PERSON = Convert.ToDecimal(ddl.SelectedValue);
                        else
                            action.RESPONSIBLE1_PERSON = null;

                        ddl = (RadComboBox)arow.FindControl("ddlPPPType");
                        action.PPP_TYPE = ddl.SelectedValue;

                        hf = (HiddenField)arow.FindControl("hfVerifyObservations");
                        action.VERIFY_OBSERVATIONS = hf.Value;
                        hf = (HiddenField)arow.FindControl("hfVerifyStatus");
                        action.VERIFY_STATUS = hf.Value;

                        CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.Add(action);

                        task = CaseCtl().problemCase.TeamTask.FindTask("5", "T", actionNo);
                        if (task == null)
                        {
                            task = CaseCtl().problemCase.TeamTask.CreateTask("5", "T", actionNo, action.ACTION_DESC, (DateTime)action.EFF_DT, (decimal)action.RESPONSIBLE1_PERSON);
                            task.STATUS = ((int)TaskMgr.CalculateTaskStatus(task)).ToString();
                        }
                        else
                        {
                           task = CaseCtl().problemCase.TeamTask.UpdateTask(task, (DateTime)action.EFF_DT, (decimal)action.RESPONSIBLE1_PERSON, action.ACTION_DESC);
                        }
                    }
                    if ((string.IsNullOrEmpty(ddl0.SelectedValue) || ddl0.SelectedValue == "C" || ddl0.SelectedValue == "D") && task != null)
                        task.STATUS = ((int)TaskStatus.Delete).ToString();
                }
            }

            if (CaseCtl().problemCase.ProbCase.PROB_RISK == null)
                CaseCtl().problemCase.CreateProblemRisk();
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_STATE = tbCase5riskState1.Text;
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_SEVERITY = Convert.ToInt32(ddlCase5riskSeverity1.SelectedValue);
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_OCCUR = Convert.ToInt32(ddlCase5riskOccur1.SelectedValue);
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_DETECT = Convert.ToInt32(ddlCase5riskDetect1.SelectedValue);
            if (!string.IsNullOrEmpty(hfCase5riskIndex1.Value))
                CaseCtl().problemCase.ProbCase.PROB_RISK.RISK1_INDEX = Convert.ToInt32(hfCase5riskIndex1.Value);

            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_STATE = tbCase5riskState2.Text;
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_SEVERITY = Convert.ToInt32(ddlCase5riskSeverity2.SelectedValue);
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_OCCUR = Convert.ToInt32(ddlCase5riskOccur2.SelectedValue);
            CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_DETECT = Convert.ToInt32(ddlCase5riskDetect2.SelectedValue);
            if (!string.IsNullOrEmpty(hfCase5riskIndex2.Value))
                CaseCtl().problemCase.ProbCase.PROB_RISK.RISK2_INDEX = Convert.ToInt32(hfCase5riskIndex2.Value);

            CaseCtl().problemCase.ProbCase.PROB_RISK.OTHER_RISK = tbCase3OtherRisk.Text;

            if (updateAndSave)
            {
                if (uclStatus5.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("5", "C", 0)))
                    CaseCtl().problemCase.StepComplete = "5";
            }

            return problemCase;
        }


        public void gvCauseActionSubList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                HiddenField hf;
                Label lbl;
                RadComboBox ddl;

                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbCorrectiveAction");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfActionDueDate");
                    RadDatePicker rd = (RadDatePicker)e.Row.Cells[0].FindControl("radActionDueDate");
                    rd.MinDate = SetMinDate(CaseCtl().problemCase);
                    rd.ShowPopupOnFocus = true;
                    try
                    {
                        if (!string.IsNullOrEmpty(hf.Value) && Convert.ToDateTime(hf.Value) > DateTime.MinValue)
                        {
                            SQMBasePage.DisplayControlValue(rd, hf.Value, CaseCtl().PageMode, "textStd");
                        }
                        else
                            rd.SelectedDate = null;
                    }
                    catch { }

                    
                    ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlActionStatus");
                    ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems("taskStatus", 2, "short"));
                    ddl.Items.Insert(0, new RadComboBoxItem("", ""));
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfActionStatus");
                    if (ddl.Items.FindItemByValue(hf.Value) != null)
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "refText");

                    ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlPPPType");
                    string causeType = "PPPType";
                    //string causeType = (isEhsCase) ? "causeTypeEHS" : "causeTypeQS";
                    ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems(causeType, 2, "short"));
                    ddl.Items.Insert(0, new RadComboBoxItem("", ""));
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfPPPType");
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfActionResponsible");
                    ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlActionResponsible");
                    SetPersonList(ddl, CaseCtl().PersonSelectList(), "");
                    if (!string.IsNullOrEmpty(hf.Value) && CaseCtl().PageMode == PageUseMode.ViewOnly)
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd", "<hr />");
                    else
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");

                }
                catch (Exception ex)
                {
                    ;
                }
            }
        }
        #endregion

        #region case6
        public void BindCase6()
        {
            ToggleVisible(pnlCase6);
			isEhsCase = (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS");

			if (isEhsCase)
			{
				lblProbCase6Instruction.Visible = false;
				lblProbCase6InstructionEhs.Visible = true;
                btnCase6AddTrial2.Visible = false;
				phCase6Trial.Visible = false;
                phCase6Identification.Visible = phCase6Attachments.Visible = false;
			}
			else
			{
                btnCase6AddTrial2.Visible = false;
                phCase6Trial.Visible = false;
				lblProbCase6Instruction.Visible = true;
				lblProbCase6InstructionEhs.Visible = false;
			}

            radCase6TargetDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            radCase6TargetDate.MinDate = SetMinDate(CaseCtl().problemCase);
            radCase6TargetDate.ShowPopupOnFocus = true;
            
			if (CaseCtl().problemCase.ProbCase.PROB_VERIFY == null)
			{
				CaseCtl().problemCase.CreateProblemVerify();
				var verifyTask = CaseCtl().problemCase.TeamTask.TaskList.FirstOrDefault(t => t.TASK_STEP == "6" && t.TASK_TYPE == "C");
			    CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT = verifyTask.DUE_DT;
			}

            if (CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT.HasValue && CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT >= radCase6TargetDate.MinDate)
            {
                //radCase6TargetDate.SelectedDate = CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT;
                SQMBasePage.DisplayControlValue(radCase6TargetDate, CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT.ToString(), CaseCtl().PageMode, "textStd");
            }

            SQMBasePage.DisplayControlValue(tbCase6VerifyMethod, CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_METHOD, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase6VerifyResult, CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_RESULT, CaseCtl().PageMode, "textStd");
            //if (CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT.HasValue)
            //{
            //    if (WebSiteCommon.IsValidDate((DateTime)CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT))
            //        radCase6TargetDate.SelectedDate = radCase5EffDate.SelectedDate = CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT;
            //}
            //if (CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_DT.HasValue)
            //{
            //    if (WebSiteCommon.IsValidDate((DateTime)CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_DT))
            //        radCase6VerifyDate.SelectedDate = radCase5EffDate.SelectedDate = CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_DT;
            //}
            SQMBasePage.DisplayControlValue(tbCase6Identification, CaseCtl().problemCase.ProbCase.PROB_VERIFY.POST_VERIFY_IDENTIFICATION, CaseCtl().PageMode, "textStd");

            uclStatus6.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("6", "C", 0), hfHlpCase6Complete.Value);
            CheckTaskProgress(6, uclStatus6);

            gvActionEfectiveList.DataSource = CaseCtl().problemCase.CreateActionList();
            gvActionEfectiveList.DataBind();

            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
            {
                Ucl_Attach attch = (Ucl_Attach)Page.LoadControl("/Include/Ucl_Attach.ascx");
                uclRadAsyncUpload6.Parent.Controls.AddAt(uclRadAsyncUpload6.Parent.Controls.IndexOf(uclRadAsyncUpload6), attch);
                attch.BindDisplayAttachments(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "6", 0);
                uclRadAsyncUpload3.Visible = false;
            }
            else
            {
                uclRadAsyncUpload6.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "6");
            }

            /*
            if (!CanSave(problemCase) || gvActionEfectiveList.Rows.Count < 1)
                btnCase6Save2.Enabled = false;
            else
            */
                btnCase6Save2.Enabled = true;

        }

        public ProblemCase UpdateCase6(ProblemCase problemCase)
        {
            if (uclRadAsyncUpload6.RAUpload.UploadedFiles.Count > 0)
            {
                SaveUploadedFiles(problemCase, "6");
                uclRadAsyncUpload6.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, "6");
            }

            // validate required fields 
            if (radCase6TargetDate.SelectedDate == null)
            {
                CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.RequiredInputs;
                return problemCase;
            }
            foreach (GridViewRow row in gvActionEfectiveList.Rows)
            {
                HiddenField hfCauseNo = (HiddenField)row.FindControl("hfRootCauseNo");
                HiddenField hfActionNo = (HiddenField)row.FindControl("hfActionNo");
                PROB_CAUSE_ACTION action = CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.FirstOrDefault(a => a.CAUSE_NO == Convert.ToInt32(hfCauseNo.Value) && a.ACTION_NO == Convert.ToInt32(hfActionNo.Value));
                if (action != null)
                {
                    TextBox tb = (TextBox)row.FindControl("tbVerifyObservations");
                    RadComboBox ddl = (RadComboBox)row.FindControl("ddlVerify");
                    if (string.IsNullOrEmpty(tb.Text) || string.IsNullOrEmpty(ddl.SelectedValue))
                    {
                        CaseCtl().problemCase.UpdateStatus = CaseUpdateStatus.Incomplete;
                        return problemCase;
                    }
                }
            }

            CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_METHOD = tbCase6VerifyMethod.Text;
            CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_RESULT = tbCase6VerifyResult.Text;
            if (radCase6TargetDate.SelectedDate != null)
                    CaseCtl().problemCase.ProbCase.PROB_VERIFY.VERIFY_TARGET_DT = radCase6TargetDate.SelectedDate;

            foreach (GridViewRow row in gvActionEfectiveList.Rows)
            {
                int actionNo = 0;
                HiddenField hfCauseNo = (HiddenField)row.FindControl("hfRootCauseNo");
                HiddenField hfActionNo = (HiddenField)row.FindControl("hfActionNo");

                PROB_CAUSE_ACTION action = CaseCtl().problemCase.ProbCase.PROB_CAUSE_ACTION.FirstOrDefault(a => a.CAUSE_NO == Convert.ToInt32(hfCauseNo.Value) && a.ACTION_NO == Convert.ToInt32(hfActionNo.Value));
                if (action != null)
                {
                    TextBox tb = (TextBox)row.FindControl("tbVerifyObservations");
                    action.VERIFY_OBSERVATIONS = tb.Text;
                    RadComboBox ddl = (RadComboBox)row.FindControl("ddlVerify");
                    action.VERIFY_STATUS = ddl.SelectedValue;
                }
            }
 
            CaseCtl().problemCase.ProbCase.PROB_VERIFY.POST_VERIFY_IDENTIFICATION = tbCase6Identification.Text;

            if (uclStatus6.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("6", "C", 0)))
                CaseCtl().problemCase.StepComplete = "6";

            return problemCase;
        }

        private void AddVerifyTrial()
        {
            ;
        }

        public void gvActionEffectiveList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                HiddenField hf;
                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbVerifyAction");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");
                    tb = (TextBox)e.Row.Cells[0].FindControl("tbVerifyObservations");
                    SQMBasePage.DisplayControlValue(tb, tb.Text, CaseCtl().PageMode, "textStd");

                    RadComboBox ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlVerify");
                    if (ddl != null)
                    {
                        ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems("verifyStatus", 0, "short"));
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfVerifyStatus");
                        SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "refText");
                    }
                }
                catch
                {
                }
            }
        }
        #endregion

    #region case7
        public void BindCase7()
        {
            ToggleVisible(pnlCase7);
           
            if (CaseCtl().problemCase.ProbCase.PROB_PREVENT == null)
            {
                CaseCtl().problemCase.CreateProblemPrevent(CaseCtl().problemCase);
            }

            SQMBasePage.DisplayControlValue(tbCase7Method, CaseCtl().problemCase.ProbCase.PROB_PREVENT.PREVENT_METHOD, CaseCtl().PageMode, "textStd");

            if (CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST != null)
            {
                try
                {
                    SQMBasePage.DisplayControlValue(tbCase7ImpactedAreas, CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Where(l => l.PREVENT_ITEM_TYPE == "A").Select(l => l.PREVENT_ITEM_DESC).FirstOrDefault(), CaseCtl().PageMode, "textStd");
                }
                catch
                {
                }
            }

            if (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS")
            {
                trImpactedLocs.Visible = false;
                trImpactedDocs.Visible = false;
            }
            else
            {
                trImpactedDocs.Visible = true;
                trImpactedLocs.Visible = true;
            }

            if (trImpactedLocs.Visible)
            {
                string locs = "";
                decimal?[] plantIDArray = CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Where(l => l.PREVENT_ITEM_TYPE == "L").Select(l => l.PREVENT_ITEM_REF).ToArray();
                PSsqmEntities ctx = new PSsqmEntities();
                List<PLANT> plantList = SQMModelMgr.SelectPlantList(ctx, SQMModelMgr.LookupPrimaryCompany(ctx).COMPANY_ID, 0);
                ddlPlantSelect.Items.Clear();
                foreach (PLANT plant in plantList)
                {
                    RadComboBoxItem item = new RadComboBoxItem(plant.PLANT_NAME, plant.PLANT_ID.ToString());
                    if (plantIDArray.Contains(plant.PLANT_ID))
                    {
                        item.Checked = true;
                        locs += (plant.PLANT_NAME + " ");
                    }
                    ddlPlantSelect.Items.Add(item);
                }

                SQMBasePage.DisplayControlValue(tbCase7ImpactedLocs, locs, CaseCtl().PageMode, "textStd");

                if (plantList == null || plantList.Count == 0)
                    plantList = SQMModelMgr.SelectPlantList(CaseCtl().problemCase.Entities, SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0);
                if (CaseCtl().PersonSelectListCount() == 0)
                    CaseCtl().problemCase.PersonSelectList = SQMModelMgr.SelectPersonList((decimal)CaseCtl().problemCase.ProbCase.COMPANY_ID, 0, true, false).OrderBy(l=> l.LAST_NAME).ToList();
            }

            if (trImpactedDocs.Visible)
            {
                gvDocumentationList.DataSource = CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Where(l => l.PREVENT_ITEM_TYPE == "D").OrderBy(l => l.PREVENT_ITEM).ToList();
                gvDocumentationList.DataBind();
            }

            uclStatus7.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("7", "C", 0), hfHlpCase7Complete.Value);
            CheckTaskProgress(7, uclStatus7);

            btnCase7Save.Enabled = CanSave();
        }


        protected void btnDocumentationAdd_Click(object sender, EventArgs e)
        {
            UpdateCase7(CaseCtl().problemCase, false);
            PROB_PREVENT_LIST item = new PROB_PREVENT_LIST();
            item.PREVENT_ITEM_TYPE = "D";
            if (CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST != null && CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Count > 0)
                item.PREVENT_ITEM = CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Max(l => l.PREVENT_ITEM) + 1;
            else
                item.PREVENT_ITEM = 1;

            TASK_STATUS task = CaseCtl().problemCase.TeamTask.TaskList.Where(l => l.TASK_STEP == "7" && l.TASK_TYPE == "C").FirstOrDefault();
            if (task != null && task.DUE_DT.HasValue)
                item.TARGET_DT = task.DUE_DT;
            CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Add(item);
            BindCase7();
            udpDocumentationList.Update();
        }

        public ProblemCase UpdateCase7(ProblemCase problemCase, bool updateAndSave)
        {
            CaseCtl().problemCase.ProbCase.PROB_PREVENT.PREVENT_METHOD = tbCase7Method.Text;

            CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Clear();

            PROB_PREVENT_LIST item = new PROB_PREVENT_LIST();
            item.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
            item.PREVENT_ITEM = 0;
            item.PREVENT_ITEM_TYPE = "A";
            item.CONFIRM_STATUS = "C";
            item.PREVENT_ITEM_DESC = tbCase7ImpactedAreas.Text;
            CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Add(item);

            int itemNo = 0;
            if (trImpactedLocs.Visible)
            {
                List<string> plantSelList = ddlPlantSelect.Items.Where(i => i.Checked == true).Select(i => i.Value).ToList();
                foreach (string plantID in plantSelList)
                {
                    item = new PROB_PREVENT_LIST();
                    item.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                    item.PREVENT_ITEM = itemNo++;
                    item.PREVENT_ITEM_TYPE = "L";
                    item.PREVENT_ITEM_REF = Convert.ToDecimal(plantID);
                    CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Add(item);
                }
            }

            if (trImpactedDocs.Visible)
            {
                foreach (GridViewRow row in gvDocumentationList.Rows)
                {
                    TextBox tb = (TextBox)row.FindControl("tbDocument");
                    if (!string.IsNullOrEmpty(tb.Text))
                    {
                        item = new PROB_PREVENT_LIST();
                        item.PROBCASE_ID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
                        item.PREVENT_ITEM = row.RowIndex;
                        HiddenField hf = (HiddenField)row.FindControl("hfItemType");
                        item.PREVENT_ITEM_TYPE = hf.Value;
                        item.PREVENT_ITEM_NAME = tb.Text;
                        DropDownList ddl = (DropDownList)row.FindControl("ddlResponsible");
                        item.RESPONSIBLE_PERSON = Convert.ToDecimal(ddl.SelectedValue);
                        item.RESPONSIBLE = ddl.SelectedItem.Text;
                        RadDatePicker rad = (RadDatePicker)row.FindControl("radTargetDate");
                        item.TARGET_DT = rad.SelectedDate;
                        ddl = (DropDownList)row.FindControl("ddlStatus");
                        item.CONFIRM_STATUS = ddl.SelectedValue;
                        CaseCtl().problemCase.ProbCase.PROB_PREVENT_LIST.Add(item);
                    }
                }
            }

            if (uclStatus7.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("7", "C", 0)))
                CaseCtl().problemCase.StepComplete = "7";

            return problemCase;
        }


        public void gvCase7Documentation_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                HiddenField hf;
                try
                {
                    TextBox tb = (TextBox)e.Row.Cells[0].FindControl("tbDocument");

                    RadComboBox ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlResponsible");
                    SetPersonList(ddl, CaseCtl().PersonSelectList(), "");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfResponsible");
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "textStd");

                    RadDatePicker dtp = (RadDatePicker)e.Row.Cells[0].FindControl("radTargetDate");
                    dtp.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                    dtp.MinDate = new DateTime(2001, 1, 1);
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfTargetDate");
                    if (!string.IsNullOrEmpty(hf.Value))
                        SQMBasePage.DisplayControlValue(dtp, hf.Value, CaseCtl().PageMode, "textStd");

                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfStatus");
                    ddl = (RadComboBox)e.Row.Cells[0].FindControl("ddlStatus");
                    ddl.Items.AddRange(WebSiteCommon.PopulateRadListItems("taskStatus", 2, "short"));
                    ddl.Items.Insert(0, new RadComboBoxItem("", ""));
                    SQMBasePage.DisplayControlValue(ddl, hf.Value, CaseCtl().PageMode, "refText");
                }
                catch
                {
                }
            }
        }

    #endregion

     #region case8
        public void BindCase8(bool bNotify)
        {
            ToggleVisible(pnlCase8);

            if (CaseCtl().problemCase.ProbCase.PROB_CLOSE == null)
            {
                CaseCtl().problemCase.CreateProblemClose();
            }

            SQMBasePage.DisplayControlValue(tbCase8Conclusion, CaseCtl().problemCase.ProbCase.PROB_CLOSE.CONCLUSIONS, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase8Message, CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE, CaseCtl().PageMode, "textStd");
            SQMBasePage.DisplayControlValue(tbCase8MessageTitle, CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE_TITLE, CaseCtl().PageMode, "textStd");

            if (bNotify)
                lblCase8NotifyConfirm.Visible = true;

            uclStatus8.BindTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("8", "C", 0), hfHlpCase8Complete.Value);
            CheckTaskProgress(8, uclStatus8);

            cbCase8Notify.Enabled = uclStatus8.ButtonComplete.Enabled;
        }

        public ProblemCase UpdateCase8(ProblemCase problemCase, out bool bNotify)
        {
            CaseCtl().problemCase.ProbCase.PROB_CLOSE.CONCLUSIONS = tbCase8Conclusion.Text;
            CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE = tbCase8Message.Text;
            CaseCtl().problemCase.ProbCase.PROB_CLOSE.MESSAGE_TITLE = tbCase8MessageTitle.Text;

            bNotify = false;
            if (uclStatus8.UpdateTaskComplete(CaseCtl().problemCase.TeamTask.FindTask("8", "C", 0), false))
            {
                CaseCtl().problemCase.StepComplete = "8";
                bNotify = cbCase8Notify.Checked;
            }

            return problemCase;
        }

        #endregion

        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
            if (CaseCtl().PageMode == PageUseMode.ViewOnly)
                pnlCase0.Visible = pnlCase1.Visible = pnlCase2.Visible = pnlCase3.Visible = pnlCase4.Visible = pnlCase5.Visible = pnlCase6.Visible = pnlCase7.Visible = pnlCase8.Visible = true;
            else
                pnlCase0.Visible = pnlCase1.Visible = pnlCase2.Visible = pnlCase3.Visible = pnlCase4.Visible = pnlCase5.Visible = pnlCase6.Visible = pnlCase7.Visible = pnlCase8.Visible = false;

            if (pnlTarget != null)
            {
                pnlTarget.Visible = true;
            }
        }

        private RadComboBox SetPersonList(RadComboBox ddl, List<PERSON> personSelectList, string personID)
        {
            return SQMBasePage.SetPersonList(ddl, personSelectList, personID, 20);
        }

        private DateTime SetMinDate(ProblemCase problemCase)
        {
            // sets the minimum allowabel due date based on the incident age
            DateTime dt = DateTime.Now.AddDays(-1);

            if (CaseCtl().problemCase.IncidentList.Count > 0)
            {
                dt = CaseCtl().problemCase.IncidentList[0].INCIDENT_DT.AddDays(-1);
            }
            else if (CaseCtl().problemCase.ProbCase.CREATE_DT.HasValue)
            {
                dt = (DateTime)CaseCtl().problemCase.ProbCase.CREATE_DT;
                dt = dt.AddDays(-1);
            }

            return dt;
        }

        private DropDownList SetStatusList(DropDownList ddlStatus, string currentStatus)
        {
            List<Settings> status_codes = SQMSettings.Status;
            ddlStatus.DataSource = status_codes;
            ddlStatus.DataTextField = "short_desc";
            ddlStatus.DataValueField = "code";
            ddlStatus.DataBind();
            if (!string.IsNullOrEmpty(currentStatus))
            {
                ddlStatus.SelectedValue = currentStatus;
            }

            return ddlStatus;
        }

		private void SaveUploadedFiles(ProblemCase problemCase, string recordStep)
		{
			// Add files to database
			SessionManager.DocumentContext = new DocumentScope().CreateNew(
					SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
					SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
					);
			SessionManager.DocumentContext.RecordType = 21;
			SessionManager.DocumentContext.RecordID = CaseCtl().problemCase.ProbCase.PROBCASE_ID;
			SessionManager.DocumentContext.RecordStep = recordStep;
			if (recordStep == "3")
				uclRadAsyncUpload3.SaveFiles();
			else if (recordStep == "5")
				uclRadAsyncUpload5.SaveFiles();
            else if (recordStep == "6")
                uclRadAsyncUpload6.SaveFiles();
		}

        private void ReBindAttachmentsOnDelete(ATTACHMENT attach)
        {
            if (CaseCtl().problemCase != null && attach != null)
            {
                switch (attach.RECORD_STEP)
                {
                    case "3":
                        uclRadAsyncUpload3.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, attach.RECORD_STEP);
                        break;
                    case "5":
                        uclRadAsyncUpload5.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, attach.RECORD_STEP);
                        break;
                    case "6":
                        uclRadAsyncUpload6.GetUploadedFiles(21, CaseCtl().problemCase.ProbCase.PROBCASE_ID, attach.RECORD_STEP);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

    }
}