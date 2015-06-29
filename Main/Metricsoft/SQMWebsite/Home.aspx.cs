using System;
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
    public partial class Home : SQMBasePage
    {
        static List<LOCAL_LANGUAGE> langList;
        static List<DOCUMENT> docList;
        List<BusinessLocation> locationList;
        List<PERSON> respForList;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclPrefsEdit.OnEditSaveClick += SaveUserPrefs;
            uclPrefsEdit.OnEditCancelClick += CancelUserPrefs;
            uclPrefsEdit.OnPersonUpdate += ApplyUserPrefs;
            uclPrefsEdit.OnBusinessLocationChanged += ApplyWorkingLocation;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SessionManager.SessionContext == null)
                    throw new UserContextError();

                SetupPage();

                if (SessionManager.ReturnStatus != null && SessionManager.ReturnStatus == true)
                {
                    SessionManager.ClearReturns();
 
                }
                else
                {
                    if (string.IsNullOrEmpty(hdCurrentActiveSecondaryTab.Value))
                        tab_Click(lbHome2_tab, null);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
            hf.Value = SessionManager.CurrentMenuItem = "lbHomeMain";
            IsCurrentPage();
        }

        protected void OnPreRender(object sender, EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected void tab_Click(object sender, EventArgs e)
        {
            SessionManager.ClearReturns();

            LinkButton lnk = (LinkButton)sender;
            string cmd = lnk.CommandArgument.ToString();
            SetActiveTab(SessionManager.CurrentSecondaryTab = lnk.ClientID);
            switch (cmd)
            {
                case "2":
                    uclDashbd0.Initialize(true);
                    pnlHome1.Visible = pnlHome3.Visible = pnlHome4.Visible = false;
                    pnlHome2.Visible = true;
                    break;
                case "3":
                    pnlHome1.Visible = pnlHome2.Visible = pnlHome4.Visible = false;
                    pnlHome3.Visible = true;
                    break;
                case "4":
                    pnlHome1.Visible = pnlHome2.Visible = pnlHome3.Visible = false;
                    pnlHome4.Visible = true;
                    Bind_gvUploadedFiles();
                    break;
                default:
                    pnlHome2.Visible = pnlHome3.Visible = pnlHome4.Visible = false;
                    pnlHome1.Visible = true;
                    break;
            }
        }

        private void SaveUserPrefs(string cmd)
        {
            PERSON updatedUser = uclPrefsEdit.UpdateUser(null, entities);
        }

        private void CancelUserPrefs(string cmd)
        {
            uclPrefsEdit.BindUser(null, null, null);
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

            uclPrefsEdit.BindUser(null, SessionManager.UserContext.HRLocation, SessionManager.UserContext.WorkingLocation);

            if (UserContext.CheckAccess("CQM", "home", "92") == AccessMode.None)
                uclPrefsEdit.DelegateList.Attributes.Add("disabled", "true");

            // quality tasks
            if (uclTaskList.TaskListRepeater.Items.Count < 1)
            {
                bool hasDelegates;
                List<TaskItem> taskList = new List<TaskItem>();
               // taskList = QualityIssue.IncidentTaskList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, SessionManager.UserContext.Person.PERSON_ID, false, out hasDelegates);

                // EHS inputs 
                if (UserContext.CheckAccess("EHS", "input", "301") > AccessMode.View)
                {
                    if (respForList == null || respForList.Count == 0)
                    {
                        respForList = SQMModelMgr.SelectDelegateList(SessionManager.UserContext.Person.PERSON_ID);
                        respForList.Insert(0, SessionManager.UserContext.Person);
                    }
                    decimal[] pids = respForList.Select(l => l.PERSON_ID).ToArray();
                    taskList.AddRange(EHSModel.ProfileTaskList(EHSModel.GetIncompleteInputs(pids, DateTime.Now.AddMonths(-12))));
                }
                // Problem cases
                if (UserContext.CheckAccess("CQM", "prob", "151") > AccessMode.View)
                {
                    taskList.AddRange(ProblemCase.CaseTaskList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, SessionManager.UserContext.Person.PERSON_ID, false, out hasDelegates));
                }

               // taskList = taskList.OrderBy(l => l.RecordType).ThenBy(l => l.RecordID).ToList();
                uclTaskList.BindTaskList(taskList);
            }
        }

        protected void Bind_gvUploadedFiles()
        {

            DocumentScope docScope = null;
            // get HR company docs
            if (SessionManager.UserContext.HRLocation.IsPrimaryCompany())
                docScope = new DocumentScope().CreateNew(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, "SYS", 0, "", 0, "", 10);
            else if (SessionManager.UserContext.HRLocation.IsSupplierCompany(false))
                docScope = new DocumentScope().CreateNew(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, "SYS", 0, "", 0, "", 11);
            else if (SessionManager.UserContext.HRLocation.IsCustomerCompany(false))
                docScope = new DocumentScope().CreateNew(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, "SYS", 0, "", 0, "", 12);
            docList = SQMDocumentMgr.SelectDocListFromContext(docScope);

            // get working location docs
            docScope = new DocumentScope().CreateNew(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "", SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0, 15);
            docList.AddRange(SQMDocumentMgr.SelectDocListFromContext(docScope));

            uclDocList.BindRadDocsList(docList);

            // setup for doc upload if enabled for this user
            SessionManager.DocumentContext = new DocumentScope().CreateNew(SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "", SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0);
            lblPostedDocuments.Text = "";
            foreach (DOCUMENT doc in docList)
            {
                if (doc.DOCUMENT_SCOPE == "BLI" && doc.RECORD_ID == SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID)
                    lblPostedDocuments.Text += (doc.FILE_NAME + ", ");
            }
        }
    }
}