
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Globalization;
using System.Threading;


namespace SQM.Website
{
    public partial class Ucl_INCFORM_Approval : System.Web.UI.UserControl
    {
        const Int32 MaxTextLength = 4000;

        protected decimal companyId;

        protected int totalFormSteps;

        protected decimal incidentTypeId;
        protected string incidentType;
        protected bool IsFullPagePostback = false;
        protected bool canApproveAny = false;

        protected int status = 0;//for Severity level section
        PSsqmEntities entities;

        protected static List<INCFORM_TYPE_CONTROL> incidentStepList;

        public PageUseMode PageMode { get; set; }

        public decimal theincidentId { get; set; }

        /*
		public decimal ApprovalStep
		{
			get { return ViewState["ApprovalStep"] == null ? 0 : (decimal)ViewState["ApprovalStep"]; }
			set { ViewState["ApprovalStep"] = value; }
		}
		*/
        public INCFORM_TYPE_CONTROL ApprovalStep
        {
            get { return ViewState["ApprovalStep"] == null ? null : (INCFORM_TYPE_CONTROL)ViewState["ApprovalStep"]; }
            set { ViewState["ApprovalStep"] = value; }
        }

        public bool IsEditContext
        {
            get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
            set
            {
                ViewState["IsEditContext"] = value;
            }
        }

        public decimal SelectedTypeId
        {
            get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
            set { ViewState["SelectedTypeId"] = value; }
        }

        public decimal IncidentId
        {
            get { return ViewState["IncidentId"] == null ? 0 : (decimal)ViewState["IncidentId"]; }
            set { ViewState["IncidentId"] = value; }
        }

        protected string IncidentLocationTZ
        {
            get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
            set { ViewState["IncidentLocationTZ"] = value; }
        }

        protected decimal EditIncidentTypeId
        {
            get { return IncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(IncidentId); }
        }

        public INCIDENT LocalIncident
        {
            get { return ViewState["LocalIncident"] == null ? null : (INCIDENT)ViewState["LocalIncident"]; }
            set { ViewState["LocalIncident"] = value; }
        }

        public string ValidationGroup
        {
            get { return ViewState["ValidationGroup"] == null ? " " : (string)ViewState["ValidationGroup"]; }
            set { ViewState["ValidationGroup"] = value; }
        }

        public List<XLAT> XLATList
        {
            get { return ViewState["ApprovalXLATList"] == null ? null : (List<XLAT>)ViewState["ApprovalXLATList"]; }
            set { ViewState["ApprovalXLATList"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (SessionManager.SessionContext != null)
            {

                if (IsFullPagePostback)
                    rptApprovals.DataBind();
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            PSsqmEntities entities = new PSsqmEntities();
            companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

            if (IsPostBack)
            {
                // Since IsPostBack is always TRUE for every invocation of this user control we need some way 
                // to determine whether or not to refresh its page controls, or just data bind instead.  
                // Here we are using the "__EVENTTARGET" form event property to see if this user control is loading 
                // because of certain page control events that are supposed to be fired off as actual postbacks.  

                IsFullPagePostback = false;
                var targetID = Request.Form["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(targetID))
                {
                    var targetControl = this.Page.FindControl(targetID);

                    if (targetControl != null)
                        if ((this.Page.FindControl(targetID).ID == "btnSave") ||
                            (this.Page.FindControl(targetID).ID == "btnNext"))
                            IsFullPagePostback = true;

                    //Maintain SEVERITY-LEVEL section visible after full page postback.
                    if (!IsFullPagePostback)
                    {
                        //To get Privilege information and if Privilege is SEVERITY-LEVEL then SEVERITY-LEVEL section can be seen.
                        string PrivInfo = SessionManager.UserContext.Person.PRIV_GROUP;

                        if (PrivInfo == "Global Safety Group")
                        {
                            if (targetID.Contains("InitialActionApproval"))
                            {
                                status = 1;
                            }
                            else if (targetID.Contains("CorrectiveActionApproval"))
                            {
                                status = 2;
                            }
                            else
                            {
                                status = 0;
                            }
                        }
                        else
                        {
                            status = 0;
                        }

                        //Update SEVERITY_LEVEL value.


                    }
                }
            }
        }

        protected override void FrameworkInitialize()
        {
            if (SessionManager.SessionContext != null)
            {
                String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

                base.FrameworkInitialize();
            }
        }


        public void PopulateInitialForm(INCFORM_TYPE_CONTROL stepRecord)
        {
            PSsqmEntities entities = new PSsqmEntities();
            ApprovalStep = stepRecord;

            XLATList = SQMBasePage.SelectXLATList(new string[1] { "INCIDENT_APPROVALS" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
            string incident = "";
            if (IncidentId > 0)
                try
                {
                    LocalIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).FirstOrDefault();
                    PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)LocalIncident.DETECT_PLANT_ID, "");
                    if (plant != null)
                        IncidentLocationTZ = plant.LOCAL_TIMEZONE;

                    //Get Issue type.
                    if (LocalIncident != null)
                    {
                        incident = LocalIncident.ISSUE_TYPE;
                    }
                    if (incident == null)
                    {
                        incident = "";
                    }
                    //To Display severity-level section only for Injury/Illness type incidents.
                    if (incident == "Injury/Illness")
                    {
                        if (status == 0)
                        {
                            pnlSeverityDescription.Visible = false;
                            pnlSeverity.Visible = false;
                        }
                        else if (status == 1) //Display severity level and checkBox on Flash Report SignOff only.
                        {
                            pnlSeverity.Visible = true;
                            pnlSeverityDescription.Visible = false;
                        }
                        else if (status == 2)//Display severity level with description on Investigation Report Signoff only.
                        {
                            pnlSeverity.Visible = false;
                            pnlSeverityDescription.Visible = true;
                        }
                    }
                    //Get SEVERITY-LEVEL information from database, check related checkbox and display servity-level description.
                    string res = (from i in entities.INCFORM_APPROVAL where i.INCIDENT_ID == IncidentId select i.SEVERITY_LEVEL).FirstOrDefault();
                    if (res == null)
                    {
                        res = "";
                    }
                    else
                    {
                        chkSeverityLevel00.Enabled = false;

                        chkSeverityLevel01.Enabled = false;

                        chkSeverityLevel02.Enabled = false;

                        chkSeverityLevel03.Enabled = false;

                        chkSeverityLevel04.Enabled = false;
                    }
                    if (res == SysPriv.first_add.ToString())
                    {
                        chkSeverityLevel00.Checked = true;
                        lblSeverityLevelDescription.Text = chkSeverityLevel00.Text;
                    }
                    else if (res == SysPriv.l1.ToString())
                    {
                        chkSeverityLevel01.Checked = true;
                        lblSeverityLevelDescription.Text = chkSeverityLevel01.Text;
                    }
                    else if (res == SysPriv.l2.ToString())
                    {
                        chkSeverityLevel02.Checked = true;
                        lblSeverityLevelDescription.Text = chkSeverityLevel02.Text;
                    }
                    else if (res == SysPriv.l3.ToString())
                    {
                        chkSeverityLevel03.Checked = true;
                        lblSeverityLevelDescription.Text = chkSeverityLevel03.Text;
                    }
                    else if (res == SysPriv.l4.ToString())
                    {
                        chkSeverityLevel04.Checked = true;
                        lblSeverityLevelDescription.Text = chkSeverityLevel04.Text;
                    }

                }
                catch (Exception ex) { string s = ex.Message; }

            InitializeForm(entities);
        }


        void InitializeForm(PSsqmEntities entities)
        {
            SetUserAccess("INCFORM_APPROVAL");

            pnlApproval.Visible = true;
            divStatus.Visible = false;
            try
            {
                //If localIncident is not null then populate the form
                if (LocalIncident != null)
                {
                    // check if incident approval status is greater than this
                    bool result = LocalIncident.LAST_APPROVAL_STEP.HasValue && LocalIncident.LAST_APPROVAL_STEP > ApprovalStep.STEP;
                    if (result)
                         {
                            PageMode = PageUseMode.ViewOnly;
                         }

                     incidentStepList = EHSIncidentMgr.SelectIncidentSteps(entities, -1);
                     canApproveAny = false;
                    rptApprovals.DataSource = EHSIncidentMgr.GetApprovalList(entities, (decimal)LocalIncident.ISSUE_TYPE_ID, ApprovalStep.STEP, IncidentId, DateTime.UtcNow, 0);

                    rptApprovals.DataBind();
                }
               else
               {
                    btnSave.Visible = false;
               }
            }
            catch (Exception ex) { string s = ex.Message; }
        }

        private void SetUserAccess(string currentFormName)
        {

        }

        public void rptApprovals_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                try
                {
                    EHSIncidentApproval approvalRec = (EHSIncidentApproval)e.Item.DataItem;
                    bool canApprove = false;


                    Label lba = (Label)e.Item.FindControl("lbApprover");
                    Label lbm = (Label)e.Item.FindControl("lbApproveMessage");
                    Label lb = (Label)e.Item.FindControl("lbItemSeq");
                    Label lbjobd = (Label)e.Item.FindControl("lbApproverJob");
                    HiddenField hfrole = (HiddenField)e.Item.FindControl("hfRoleDesc");
                    CheckBox cba = (CheckBox)e.Item.FindControl("cbIsAccepted");
                    RadDatePicker rda = (RadDatePicker)e.Item.FindControl("rdpAcceptDate");

                    lbjobd.Text = hfrole.Value = XLATList.Where(l => l.XLAT_CODE == approvalRec.stepPriv.PRIV.ToString()).FirstOrDefault().DESCRIPTION_SHORT;
                    lbm.Text = XLATList.Where(l => l.XLAT_CODE == approvalRec.stepPriv.PRIV.ToString()).FirstOrDefault().DESCRIPTION;

                    HiddenField hf = (HiddenField)e.Item.FindControl("hfApprovalID");
                    hf.Value = approvalRec.approval.INCIDENT_APPROVAL_ID == null ? "0" : approvalRec.approval.INCIDENT_APPROVAL_ID.ToString();
                    hf = (HiddenField)e.Item.FindControl("hfItemSeq");
                    hf.Value = approvalRec.approval.ITEM_SEQ.ToString();
                    hf = (HiddenField)e.Item.FindControl("hfPersonID");
                    hf.Value = approvalRec.approval.APPROVER_PERSON_ID.ToString();
                    hf = (HiddenField)e.Item.FindControl("hfReqdComplete");
                    hf.Value = approvalRec.stepPriv.REQUIRED_COMPLETE.ToString();
                    lb.Visible = false;
                    lba.Text = !string.IsNullOrEmpty(approvalRec.approval.APPROVER_PERSON) ? approvalRec.approval.APPROVER_PERSON : "";
                    cba.Checked = approvalRec.approval.IsAccepted;
                    rda.SelectedDate = approvalRec.approval.APPROVAL_DATE;

                    canApprove = SessionManager.CheckUserPrivilege((SysPriv)approvalRec.stepPriv.PRIV, SysScope.incident);

                    if (cba.Checked)
                    {
                        cba.Enabled = false;        // don't allow removing approval once it was given
                    }
                    else
                    {
                        if (canApprove && PageMode == PageUseMode.Active)
                        {
                            canApproveAny = true;
                            lba.Text = SessionManager.UserContext.UserName();
                            cba.Enabled = true;
                            if ((SysPriv)approvalRec.stepPriv.PRIV == SysPriv.approve || (SysPriv)approvalRec.stepPriv.PRIV == SysPriv.release || approvalRec.stepPriv.SIGN_MULTIPLE == 1)  // can approve at top approval level
                            {
                                foreach (RepeaterItem item in rptApprovals.Items)
                                {
                                    PlaceHolder ph = (PlaceHolder)item.FindControl("phOnBehalfOf");
                                    ph.Visible = true;
                                    lba = (Label)item.FindControl("lbApprover");
                                    lba.Text = SessionManager.UserContext.UserName();
                                    cba = (CheckBox)item.FindControl("cbIsAccepted");
                                    if (cba.Checked)
                                        cba.Enabled = false;
                                    else
                                        cba.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            cba.Enabled = false;
                        }
                    }

                }
                catch { }
            }

            btnSave.Visible = pnlApproval.Enabled = false;

            if (PageMode == PageUseMode.Active && canApproveAny)
            {
                btnSave.Visible = pnlApproval.Enabled = EHSIncidentMgr.IsDependentStatus(LocalIncident, EHSIncidentMgr.GetIncidentSteps(incidentStepList, (decimal)LocalIncident.ISSUE_TYPE_ID).Where(l => l.STEP == ApprovalStep.STEP).Select(l => l.DEPENDENT_STATUS).FirstOrDefault());
                if (!btnSave.Visible)
                {
                    lblStatusMsg.Text = Resources.LocalizedText.IncidentReportIncomplete;
                    divStatus.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
                if (AddUpdateINCFORM_APPROVAL(IncidentId, "save") >= 0)
                {
                    string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
                }
            else
            {
                lblResponseMsg.Text = "Please approve the report for distribution and check the severity levels !";
            }

            InitializeForm(new PSsqmEntities());
            
        }

        public int AddUpdateINCFORM_APPROVAL(decimal incidentId, string option)
        {
            //To get approvertype from usercontext. -NIHARIKA SAXENA
            var approveerType = (SessionManager.UserContext.PrivList.Where(p => p.PRIV <= 100).FirstOrDefault()).PRIV_GROUP.ToString();
            //Check status of severityLevel of Global Safety Group
            bool severityLevel = false;

            var itemList = new List<INCFORM_APPROVAL>();
            int status = 0;
            int numRequired = 0;
            int requiredCount = 0;
            int approvalCount = 0;
            bool isRequired;
            bool isChanged = false;

           

            List<INCFORM_APPROVAL> approvalList = new List<INCFORM_APPROVAL>();

            using (PSsqmEntities ctx = new PSsqmEntities())
            {

                foreach (RepeaterItem item in rptApprovals.Items)
                {
                    isRequired = false;
                    HiddenField hfreq = (HiddenField)item.FindControl("hfReqdComplete");
                    if (hfreq.Value.ToLower() == "true")
                    {
                        ++numRequired;
                        isRequired = true;
                    }

                    INCFORM_APPROVAL approval = new INCFORM_APPROVAL();
                    approval.INCIDENT_ID = incidentId;
                    approval.APPROVAL_LEVEL = 0;
                    approval.STEP = ApprovalStep.STEP;
                    Label lba = (Label)item.FindControl("lbApprover");
                    Label lb = (Label)item.FindControl("lbItemSeq");
                    CheckBox cba = (CheckBox)item.FindControl("cbIsAccepted");
                    RadDatePicker rda = (RadDatePicker)item.FindControl("rdpAcceptDate");
                    HiddenField hfrole = (HiddenField)item.FindControl("hfRoleDesc");
                    HiddenField hf = (HiddenField)item.FindControl("hfItemSeq");
                    approval.ITEM_SEQ = Convert.ToInt32(hf.Value);
                    approval.APPROVER_TITLE = hfrole.Value;


                    //Update SEVERITY_LEVEL value. - 
                    if (chkSeverityLevel00.Checked)
                    {
                        severityLevel = chkSeverityLevel00.Checked;
                        approval.SEVERITY_LEVEL = SysPriv.first_add.ToString();
                    }
                    else if (chkSeverityLevel01.Checked)
                    {
                        severityLevel = chkSeverityLevel01.Checked;
                        approval.SEVERITY_LEVEL = SysPriv.l1.ToString();
                    }

                    else if (chkSeverityLevel02.Checked)
                    {
                        severityLevel = chkSeverityLevel02.Checked;
                        approval.SEVERITY_LEVEL = SysPriv.l2.ToString();
                    }
                    else if (chkSeverityLevel03.Checked)
                    {
                        severityLevel = chkSeverityLevel03.Checked;
                        approval.SEVERITY_LEVEL = SysPriv.l3.ToString();
                    }
                    else if (chkSeverityLevel04.Checked)
                    {
                        severityLevel = chkSeverityLevel04.Checked;
                        approval.SEVERITY_LEVEL = SysPriv.l4.ToString();
                    }

                    approvalList.Add(approval);

                    if (cba.Checked == true)    // is approved
                    {
                        ++approvalCount;

                        if (isRequired)
                        {
                            ++requiredCount;
                        }

                        hf = (HiddenField)item.FindControl("hfApprovalID");
                        if (string.IsNullOrEmpty(hf.Value) || hf.Value == "0")
                        {
                            // save new approval to db
                            isChanged = true;
                            approval.APPROVAL_DATE = rda.SelectedDate;
                            approval.IsAccepted = true;
                            hf = (HiddenField)item.FindControl("hfPersonID");
                            if (string.IsNullOrEmpty(hf.Value) || hf.Value == "0")
                            {
                                approval.APPROVER_PERSON_ID = SessionManager.UserContext.Person.PERSON_ID;
                                approval.APPROVER_PERSON = SessionManager.UserContext.UserName();
                            }
                            else
                            {
                                approval.APPROVER_PERSON_ID = Convert.ToDecimal(hf.Value);
                                approval.APPROVER_PERSON = lba.Text;
                            }

                            ctx.AddToINCFORM_APPROVAL(approval);
                        }
                    }
                }

                if (approveerType == "Global Safety Group")
                {
                   
                    if (severityLevel)
                    {
                        lblResponseMsg.Text = "";
                        chkSeverityLevel00.Enabled = false;

                        chkSeverityLevel01.Enabled = false;

                        chkSeverityLevel02.Enabled = false;

                        chkSeverityLevel03.Enabled = false;

                        chkSeverityLevel04.Enabled = false;

                        //status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_APPROVAL WHERE INCIDENT_ID = " + incidentId.ToString() + " AND STEP = " + ApprovalStep.ToString());
                        status = ctx.SaveChanges();
                    }
                    else
                    {
                        status = -1;
                    }
                }else
                {
                    status = ctx.SaveChanges();
                }

                bool notifyStepComplete = false;
                string incidentStep = "";

                // determine step status
                if (approvalCount > 0)
                {
                    if (ApprovalStep.STEP == 10.0m)
                    {
                        incidentStep = "Approvals";
                        IncidentStepStatus stat;
                        if ((numRequired > 0 && requiredCount == numRequired) || approvalCount == rptApprovals.Items.Count)
                        {
                            stat = (IncidentStepStatus)Math.Min(rptApprovals.Items.Count + 150, 155);
                            EHSIncidentMgr.UpdateIncidentStatus(incidentId, stat, true, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
                            notifyStepComplete = true;
                        }
                        else
                        {
                            incidentStep = ApprovalStep.STEP == 5.5m ? "CorrectiveActionApproval" : "InitialActionApproval";
                            stat = (IncidentStepStatus)Math.Min(approvalCount + 150, 154);
                            EHSIncidentMgr.UpdateIncidentStatus(incidentId, stat, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
                        }
                    }
                    else
                    {
                        if ((numRequired > 0 && requiredCount == numRequired) || approvalCount == rptApprovals.Items.Count)
                        {
                            EHSIncidentMgr.UpdateIncidentApprovalStatus(incidentId, ApprovalStep.STEP);
                            notifyStepComplete = true;
                        }
                    }
                }

                // only send notifications when approvals are added
                if (isChanged)
                {
                    NotifyCycle notifyCycle;
                    try
                    {
                        notifyCycle = ApprovalStep.NOTIFY_CYCLE.HasValue ? (NotifyCycle)ApprovalStep.NOTIFY_CYCLE : NotifyCycle.None;
                    }
                    catch
                    {
                        notifyCycle = NotifyCycle.None;
                    }

                    if (notifyStepComplete && ApprovalStep.STEP == 10.0m)
                    {
                        EHSNotificationMgr.NotifyIncidentStatus(LocalIncident, ((int)SysPriv.approve).ToString(), "");
                    }

                    if (notifyCycle == NotifyCycle.notifyNext)
                    {
                        INCFORM_APPROVAL priorApproval = new INCFORM_APPROVAL();
                        foreach (INCFORM_APPROVAL approval in approvalList)
                        {
                            // notify missing approvers when prior roles approved or the step was completed by 380 or 390 priv
                            if (!approval.APPROVAL_DATE.HasValue && (priorApproval.APPROVAL_DATE.HasValue || notifyStepComplete))
                            {
                                List<string> infoList = new List<string>();
                                infoList.Add(approval.APPROVER_TITLE);
                                infoList.Add(priorApproval.APPROVER_TITLE);

                             
                                    EHSNotificationMgr.NotifyIncidentSignoffRequired(LocalIncident, approval, ApprovalStep.STEP_HEADING_TEXT, approval.ITEM_SEQ >= 390 ? "SIGNOFF" : "APPROVAL", infoList);
                                
                            }
                            priorApproval = approval;
                        }
                    }
                }


            }

            return status;
        }

        protected void rptApprovals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == "AddAnother")
            {
            }
        }
    }
}
