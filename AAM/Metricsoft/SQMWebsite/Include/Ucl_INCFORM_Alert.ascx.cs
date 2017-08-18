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
using SQM.Shared;

namespace SQM.Website
{
    public partial class Ucl_INCFORM_Alert : System.Web.UI.UserControl
    {
        const Int32 MaxTextLength = 4000;

        protected PSsqmEntities localCtx;

        public decimal IncidentId
        {
            get { return ViewState["IncidentId"] == null ? 0 : (decimal)ViewState["IncidentId"]; }
            set { ViewState["IncidentId"] = value; }
        }
        protected bool IsFullPagePostback = false;//To get focus on CEOcomment section
        public INCIDENT LocalIncident
        {
            get { return ViewState["LocalIncident"] == null ? null : (INCIDENT)ViewState["LocalIncident"]; }
            set { ViewState["LocalIncident"] = value; }
        }
        public int CurrentStep
        {
            get { return ViewState["CurrentStep"] == null ? 0 : (int)ViewState["CurrentStep"]; }
            set { ViewState["CurrentStep"] = value; }
        }
        public List<XLAT> XLATList
        {
            get { return ViewState["AlertXLATList"] == null ? null : (List<XLAT>)ViewState["AlertXLATList"]; }
            set { ViewState["AlertXLATList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                // Since IsPostBack is always TRUE for every invocation of this user control we need some way 
                // to determine whether or not to refresh its page controls, or just data bind instead.  
                // Here we are using the "__EVENTTARGET" form event property to see if this user control is loading 
                // because of certain page control events that are supposed to be fired off as actual postbacks.  

                //code segment added for CEOComment and Preventative Measures
                IsFullPagePostback = false;
                var targetID = Request.Form["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(targetID))
                {
                    var targetControl = this.Page.FindControl(targetID);

                    if (targetControl != null)
                        if ((this.Page.FindControl(targetID).ID == "btnSave") ||
                            (this.Page.FindControl(targetID).ID == "btnNext"))
                            IsFullPagePostback = true;

                    if (!IsFullPagePostback)
                    {



                        // If link is CEO comment then focus on CEO Comment Section.
                        if (targetID.Contains("CEOComment"))
                        {
                            tbCeoComments.Focus();
                            //If Priv Group is other than CEO GROUP then CEO Comment box will be highlighted
                            string PrivInfo = SessionManager.UserContext.Person.PRIV_GROUP;
                            if (!(PrivInfo == "CEO-GROUP"))
                            {
                                tbCeoComments.BorderColor = System.Drawing.Color.Gray;
                            }
                        }
                        else
                        {
                            tbCeoComments.BorderColor = System.Drawing.Color.Empty;
                        }

                    }
                }
            }


        }


        protected override void FrameworkInitialize()
        {
            //String selectedLanguage = "es";
            if (SessionManager.SessionContext != null)
            {
                String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

                base.FrameworkInitialize();
            }
        }

        public void PopulateInitialForm(PSsqmEntities ctx)
        {
            XLATList = SQMBasePage.SelectXLATList(new string[20] { "TASK_STATUS", "BusinessType", "BT_01", "BT_02", "BT_03", "BT_04", "MPT_S_01", "MPT_S_16", "MPT_S_18", "MPT_S_19", "MPT_B_09", "MPT_B_10", "MPT_B_23", "MPT_P_05", "MPT_P_06", "MPT_P_10", "MPT_P_11", "MPT_P_13", "MPT_AEL_1", "MPT_AEL_2" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);

            InitializeForm(ctx);
        }


        void InitializeForm(PSsqmEntities ctx)
        {
            lblStatusMsg.Visible = false;
            pnlAlert.Visible = true;
            //pnlBaseForm2.Visible = true;
            localCtx = ctx;
            LocalIncident = EHSIncidentMgr.SelectIncidentById(localCtx, IncidentId);
            //populate form when LocalIncident is not null 
            //if (LocalIncident == null)
            //{
            //    return;
            //}
            if (LocalIncident != null)
            {
                GetAttachments(IncidentId);

                //To get Privilege information and if Privilege is CEO-Group then CEO-Comments section is editable.
                string PrivInfo = SessionManager.UserContext.Person.PRIV_GROUP.ToString();
                if (PrivInfo == "CEO-GROUP")
                {
                    tbCeoComments.ReadOnly = false;

                }

                INCFORM_ALERT incidentAlert = EHSIncidentMgr.LookupIncidentAlert(localCtx, IncidentId);

                SQMBasePage.SetLocationList(ddlLocations, SQMModelMgr.SelectBusinessLocationList(1m, 0, true), 0);

                ddlNotifyGroup.DataSource = SQMModelMgr.SelectPrivGroupList("A", true);
                ddlNotifyGroup.DataValueField = "PRIV_GROUP";
                ddlNotifyGroup.DataTextField = "DESCRIPTION";
                ddlNotifyGroup.DataBind();

                ddlResponsibleGroup.DataSource = SQMModelMgr.SelectPrivGroupList("A", true);
                ddlResponsibleGroup.DataValueField = "PRIV_GROUP";
                ddlResponsibleGroup.DataTextField = "DESCRIPTION";
                ddlResponsibleGroup.DataBind();

                rdpDueDate = SQMBasePage.SetRadDateCulture(rdpDueDate, "");
                rdpDueDate.SelectedDate = DateTime.UtcNow.AddDays(1);

                List<NOTIFYACTION> notifyList = SQMModelMgr.SelectNotifyActionList(localCtx, null, null);
                NOTIFYACTION dfltNotify = notifyList.Where(l => l.NOTIFY_SCOPE == "IN-0" && l.SCOPE_TASK == "400").FirstOrDefault();  // get default alert notification groups

                btnSave.Enabled = false;

                pnlAlert.Enabled = btnSave.Visible = EHSIncidentMgr.CanUpdateIncident(LocalIncident, true, SysPriv.admin, LocalIncident.INCFORM_LAST_STEP_COMPLETED);

                if (incidentAlert == null)
                {
                    if (dfltNotify != null)
                    {
                        ddlNotifyGroup.Items.Where(i => dfltNotify.NOTIFY_DIST.Split(',').Contains(i.Value)).ToList().ForEach(i => i.Checked = true);
                    }
                    lblAlertStatus.Text = "";
                }
                else
                {
                    ddlLocations.Items.Where(i => incidentAlert.LOCATION_LIST.Split(',').Contains(i.Value)).ToList().ForEach(i => i.Checked = true);
                    //  tbAlertDesc.Text = incidentAlert.ALERT_DESC;

                    tbComments.Text = incidentAlert.COMMENTS;

                    // Get CEO-comments value from database and display it.
                    tbCeoComments.Text = incidentAlert.CEO_COMMENTS;

                    ddlNotifyGroup.Items.Where(i => incidentAlert.ALERT_GROUP.Split(',').Contains(i.Value)).ToList().ForEach(i => i.Checked = true);
                    if ((ddlResponsibleGroup.FindItemByValue(incidentAlert.RESPONSIBLE_GROUP)) != null)
                    {
                        ddlResponsibleGroup.SelectedValue = incidentAlert.RESPONSIBLE_GROUP;
                    }
                    rdpDueDate.SelectedDate = incidentAlert.DUE_DT;
                    lblAlertStatus.Text = XLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == "0").FirstOrDefault().DESCRIPTION;
                }

                lblNotifyGroup.Text = "";
                //Apply condotional code to desplay data in grid of Process Description
                if (LocalIncident.INCFORM_INJURYILLNESS != null)
                {
                    if (string.IsNullOrEmpty(LocalIncident.INCFORM_INJURYILLNESS.BUSINESS_TYPE))
                    {
                        BusinessType.Style.Add("display", "none");
                    }
                    else
                    {
                        lblBusinessTypeVal.Text = XLATList.Where(p => p.XLAT_CODE == LocalIncident.INCFORM_INJURYILLNESS.BUSINESS_TYPE).Select(p => p.DESCRIPTION_SHORT).FirstOrDefault();
                        //  display: block;
                    //    BusinessType.Style.Add("display", "block");
                    }
                }
                else
                {
                    BusinessType.Style.Add("display", "none");
                }


                if (LocalIncident.INCFORM_INJURYILLNESS != null)
                {
                    if (string.IsNullOrEmpty(LocalIncident.INCFORM_INJURYILLNESS.MACRO_PROCESS_TYPE))
                    {
                        MacroProcessType.Style.Add("display", "none");
                    }
                    else
                    {
                        lblMacroProcessTypeVal.Text = XLATList.Where(p => p.XLAT_CODE == LocalIncident.INCFORM_INJURYILLNESS.MACRO_PROCESS_TYPE).Select(p => p.DESCRIPTION_SHORT).FirstOrDefault();
                      //  MacroProcessType.Style.Add("display", "block");
                    }
                }
                else
                {
                    MacroProcessType.Style.Add("display", "none");
                }

                if (LocalIncident.INCFORM_INJURYILLNESS != null)
                {
                    if (string.IsNullOrEmpty(LocalIncident.INCFORM_INJURYILLNESS.SPECIFIC_PROCESS_TYPE))
                    {
                        SpecificProcessType.Style.Add("display", "none");

                    }
                    else
                    {
                        lblSpecificProcessTypeVal.Text = XLATList.Where(p => p.XLAT_CODE == LocalIncident.INCFORM_INJURYILLNESS.SPECIFIC_PROCESS_TYPE).Select(p => p.DESCRIPTION_SHORT).FirstOrDefault();
                        //SpecificProcessType.Style.Add("display", "block");
                    }
                }
                else
                {
                    SpecificProcessType.Style.Add("display", "none");
                }

                if (LocalIncident.INCFORM_INJURYILLNESS != null)
                {
                    if (string.IsNullOrEmpty(LocalIncident.INCFORM_INJURYILLNESS.EQUIPMENT_MANUFACTURER_NAME))
                    {
                        EquipmentManufacturerName.Style.Add("display", "none");
                    }
                    else
                    {
                        lblEquipmentManufacturerNameVal.Text = LocalIncident.INCFORM_INJURYILLNESS.EQUIPMENT_MANUFACTURER_NAME;
                       // EquipmentManufacturerName.Style.Add("display", "block");
                    }
                }
                else
                {
                    EquipmentManufacturerName.Style.Add("display", "none");
                }


                foreach (RadComboBoxItem item in ddlNotifyGroup.Items.Where(i => i.Checked == true).ToList())
                {
                    lblNotifyGroup.Text += string.IsNullOrEmpty(lblNotifyGroup.Text) ? item.Text : (", " + item.Text);
                }

                if (BindTAlertaskList(EHSIncidentMgr.GetAlertTaskList(localCtx, LocalIncident.INCIDENT_ID)) > 0)
                {
                    btnSave.Enabled = true;
                }
            }
            else
            {
                btnSave.Visible = false;
                pnlAlert.Visible = false;
            }
        }

        private void GetAttachments(decimal incidentId)
        {
            uploaderPreventativeMeasures.SetAttachmentRecordStep("1");
            uploaderPreventativeMeasures.SetReportOption(false);
            uploaderPreventativeMeasures.SetDescription(false);
            // Specifying postback triggers allows uploaderPreventativeMeasures to persist on other postbacks (e.g. 8D checkbox toggle)
            //  uploaderPreventativeMeasures.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete", "btnDeleteInc", "btnSubnavIncident", "btnSubnavContainment", "btnSubnavRootCause", "btnSubnavAction", "btnSubnavApproval", "btnSubnavAlert" };

            int attCnt = EHSIncidentMgr.AttachmentCount(incidentId); //add values 2 for getting the PreventativeMeasures attachment.
            int px = 128;

            if (attCnt > 0)
            {
                px = px + (attCnt * 30) + 35;
                //uploaderPreventativeMeasures.GetUploadedFilesforPreventativeMeasures(40, incidentId, "");
                uploaderPreventativeMeasures.GetUploadedFilesIncidentSection(40, incidentId, "", (int)Incident_Section.PreventativeMeasuresAttachment);
            }
            else
            {
                uploaderPreventativeMeasures.GetBlinkATTACHMENT();
            }

            /*

			*/
            // Set the html Div height based on number of attachments to be displayed in the grid:
            //dvAttachLbl.Style.Add("height", px.ToString() + "px !important");
            //dvAttach.Style.Add("height", px.ToString() + "px !important");
        }

        protected void SaveAttachments(decimal incidentId)
        {
            if (uploaderPreventativeMeasures != null)
            {
                string recordStep = (this.CurrentStep + 1).ToString();

                // Add files to database
                SessionManager.DocumentContext = new DocumentScope().CreateNew(
                    SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
                    SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
                    );
                SessionManager.DocumentContext.RecordType = 40;
                SessionManager.DocumentContext.RecordID = incidentId;
                SessionManager.DocumentContext.RecordStep = "1";

                SessionManager.DocumentContext.incident_section = (int)Incident_Section.PreventativeMeasuresAttachment;
                uploaderPreventativeMeasures.SaveFilesPreventativeMeasures();
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            AddUpdateINCFORM_ALERT(LocalIncident.INCIDENT_ID);
            string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
            InitializeForm(new PSsqmEntities());
        }

        public int AddUpdateINCFORM_ALERT(decimal incidentId)
        {

            lblStatusMsg.Visible = false;
            int status = 0;
            bool allFieldsComplete = true;

            localCtx = new PSsqmEntities();

            INCFORM_ALERT incidentAlert = EHSIncidentMgr.LookupIncidentAlert(localCtx, IncidentId);
            if (incidentAlert == null)  // new alert
            {
                incidentAlert = new INCFORM_ALERT();
                incidentAlert.INCIDENT_ID = LocalIncident.INCIDENT_ID;
                incidentAlert.ALERT_TYPE = ((int)TaskRecordType.HealthSafetyIncident).ToString();
                incidentAlert.CREATE_DT = DateTime.UtcNow;
                incidentAlert.CREATE_BY = SessionManager.UserContext.UserName();
                localCtx.AddToINCFORM_ALERT(incidentAlert);
                lblAlertStatus.Text = XLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == "0").FirstOrDefault().DESCRIPTION;

            }
            SaveAttachments(incidentId);
            incidentAlert.LOCATION_LIST = "";
            foreach (RadComboBoxItem item in ddlLocations.Items.Where(i => i.Checked == true && i.Value.Contains("BU") == false).ToList())
            {
                incidentAlert.LOCATION_LIST += string.IsNullOrEmpty(incidentAlert.LOCATION_LIST) ? item.Value : (item.Value + ",");
            }

            incidentAlert.ALERT_GROUP = "";
            foreach (RadComboBoxItem item in ddlNotifyGroup.Items.Where(i => i.Checked == true).ToList())
            {
                incidentAlert.ALERT_GROUP += string.IsNullOrEmpty(incidentAlert.ALERT_GROUP) ? item.Value : (item.Value + ",");
            }

            incidentAlert.RESPONSIBLE_GROUP = ddlResponsibleGroup.SelectedValue;
            //  incidentAlert.ALERT_DESC = tbAlertDesc.Text;
            incidentAlert.COMMENTS = tbComments.Text;
            incidentAlert.DUE_DT = rdpDueDate.SelectedDate;



            // Update CEO-Comments value.
            incidentAlert.CEO_COMMENTS = tbCeoComments.Text.Trim();

            // send general notifications
            if (incidentAlert.INCIDENT_ALERT_ID < 1)
            {
                EHSNotificationMgr.NotifyIncidentAlert(LocalIncident, ((int)SysPriv.notify).ToString(), "380",
                ddlLocations.Items.Where(i => i.Checked == true && i.Value.Contains("BU") == false).Select(i => Convert.ToDecimal(i.Value)).ToList());
            }

            List<TASK_STATUS> alertTaskList = UpdateAlertTaskList(EHSIncidentMgr.GetAlertTaskList(localCtx, LocalIncident.INCIDENT_ID));
            status = localCtx.SaveChanges();

            // send specific task assignments
            EHSNotificationMgr.NotifyIncidentAlertTaskAssignment(LocalIncident, alertTaskList.Where(l => l.RESPONSIBLE_ID.HasValue).ToList());

            return status;
        }

        protected void Delete_Click(object sender, EventArgs e)
        {

            localCtx.ExecuteStoreCommand("DELETE FROM INCFORM_ALERT WHERE INCIDENT_ID = {0}", IncidentId.ToString());
        }


        protected void lnkCreateTasks_Click(object sender, EventArgs e)
        {
            TASK_STATUS task = null;

            localCtx = new PSsqmEntities();
            List<TASK_STATUS> alertTaskList = EHSIncidentMgr.GetAlertTaskList(localCtx, LocalIncident.INCIDENT_ID);

            string[] plantSels = SQMBasePage.GetComboBoxCheckedItems(ddlLocations).Select(l => l.Value).ToArray();
            foreach (string sel in plantSels)
            {
                if ((task = alertTaskList.Where(l => l.RECORD_SUBID == Convert.ToDecimal(sel)).FirstOrDefault()) == null)
                {
                    task = new TASK_STATUS();
                    task.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
                    task.RECORD_ID = LocalIncident.INCIDENT_ID;
                    //  task.DESCRIPTION = tbAlertDesc.Text;
                    task.DETAIL = tbComments.Text;
                    task.RECORD_SUBID = Convert.ToDecimal(sel);
                    task.DUE_DT = rdpDueDate.SelectedDate;

                    List<PERSON> responsibleList = SQMModelMgr.SelectPlantPrivgroupPersonList((decimal)task.RECORD_SUBID, new string[1] { ddlResponsibleGroup.SelectedValue }, false);
                    if (responsibleList.Count > 0)
                    {
                        task.RESPONSIBLE_ID = responsibleList.First().PERSON_ID;
                    }

                    alertTaskList.Add(task);
                }
            }

            if (BindTAlertaskList(alertTaskList) > 0)
            {
                btnSave.Enabled = true;
            }
        }

        protected int BindTAlertaskList(List<TASK_STATUS> alertTaskList)
        {
            bool tasksComplete = alertTaskList.Count == 0 ? false : true;
            foreach (TASK_STATUS task in alertTaskList)
            {
                if (task.COMPLETE_DT.HasValue == false)
                {
                    tasksComplete = false;
                }
            }

            if (tasksComplete)
            {
                lblAlertStatus.Text = XLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == "2").FirstOrDefault().DESCRIPTION;
                pnlAlert.Enabled = btnSave.Visible = false;
            }

            rgAlertTaskList.DataSource = alertTaskList;
            rgAlertTaskList.DataBind();

            return alertTaskList.Count;
        }



        protected void rgAlertTaskList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HiddenField hf;
                Label lbl;
                RadComboBox ddl = null;
                PERSON responsiblePerson = null;

                try
                {
                    TASK_STATUS task = (TASK_STATUS)e.Item.DataItem;

                    hf = (HiddenField)e.Item.FindControl("hfTaskID");
                    hf.Value = task.TASK_ID > 0 ? task.TASK_ID.ToString() : "";

                    hf = (HiddenField)e.Item.FindControl("hfLocation");
                    hf.Value = task.RECORD_SUBID.ToString();

                    lbl = (Label)e.Item.FindControl("lblLocation");
                    PLANT plant = SQMModelMgr.LookupPlant((decimal)task.RECORD_SUBID);
                    if (plant != null)
                    {
                        lbl.Text = plant.PLANT_NAME;
                        hf = (HiddenField)e.Item.FindControl("hfLocationTZ");
                        hf.Value = plant.LOCAL_TIMEZONE;
                    }
                    else
                    {
                        lbl.Text = task.RECORD_SUBID.ToString();
                    }

                    lbl = (Label)e.Item.FindControl("lblTaskComments");
                    lbl.Text = task.COMMENTS;

                    if (task.COMPLETE_DT.HasValue)
                    {
                        lbl = (Label)e.Item.FindControl("lblTaskCompleteDT");
                        lbl.Text = Convert.ToDateTime(task.COMPLETE_DT).ToShortDateString();
                    }

                    ddl = (RadComboBox)e.Item.FindControl("ddlResponsible");
                    List<PERSON> responsibleList = SQMModelMgr.SelectPlantPrivgroupPersonList((decimal)task.RECORD_SUBID, new string[1] { ddlResponsibleGroup.SelectedValue }, false);
                    if (responsibleList.Count > 0)
                    {
                        foreach (PERSON person in responsibleList)
                        {
                            ddl.Items.Add(new RadComboBoxItem(SQMModelMgr.FormatPersonListItem(person), person.PERSON_ID.ToString()));
                        }
                    }
                    if (task.RESPONSIBLE_ID.HasValue)
                    {
                        if ((responsiblePerson = SQMModelMgr.LookupPerson((decimal)task.RESPONSIBLE_ID, "")) != null)
                        {
                            if (ddl.Items.FindItemByValue(responsiblePerson.PERSON_ID.ToString()) != null)
                            {
                                ddl.SelectedValue = responsiblePerson.PERSON_ID.ToString();
                            }
                            else
                            {
                                ddl.Items.Add(new RadComboBoxItem(SQMModelMgr.FormatPersonListItem(responsiblePerson), responsiblePerson.PERSON_ID.ToString()));
                            }
                        }
                    }
                    if (task.COMPLETE_DT.HasValue)
                    {
                        ddl.Enabled = false;
                    }
                }
                catch
                {
                }
            }
        }

        protected List<TASK_STATUS> UpdateAlertTaskList(List<TASK_STATUS> alertTaskList)
        {
            // create task for each location and/or update existing task
            TASK_STATUS task = null;
            int taskSeq = 0;
            bool isNew = false;
            RadComboBox ddl = null;
            HiddenField hf = null;

            foreach (GridDataItem item in rgAlertTaskList.Items)
            {
                hf = (HiddenField)item.FindControl("hfTaskID");
                ddl = (RadComboBox)item.FindControl("ddlResponsible");

                if (string.IsNullOrEmpty(hf.Value)) // create new
                {
                    isNew = true;
                    task = new TASK_STATUS();
                    hf = (HiddenField)item.FindControl("hfLocationTZ");
                    task = EHSIncidentMgr.CreateEmptyTask(LocalIncident.INCIDENT_ID, ((int)SysPriv.notify).ToString(), ++taskSeq, WebSiteCommon.LocalTime(DateTime.UtcNow, hf.Value));
                    task.RECORD_TYPE = (int)TaskRecordType.HealthSafetyIncident;
                    hf = (HiddenField)item.FindControl("hfLocation");
                    task.RECORD_SUBID = Convert.ToDecimal(hf.Value);
                    task.RECORD_ID = LocalIncident.INCIDENT_ID;
                    alertTaskList.Add(task);
                    localCtx.AddToTASK_STATUS(task);
                }
                else
                {
                    isNew = false;
                    task = alertTaskList.Where(l => l.TASK_ID.ToString() == hf.Value).FirstOrDefault();  // existing
                }

                //  task.DESCRIPTION = tbAlertDesc.Text;
                task.DETAIL = tbComments.Text;
                task.DUE_DT = rdpDueDate.SelectedDate;
                if (!string.IsNullOrEmpty(ddl.SelectedValue))
                {
                    task.RESPONSIBLE_ID = Convert.ToDecimal(ddl.SelectedValue);
                }
                else
                {
                    alertTaskList.Remove(task);
                    localCtx.DeleteObject(task);
                }
            }

            return alertTaskList;
        }

    }
}
