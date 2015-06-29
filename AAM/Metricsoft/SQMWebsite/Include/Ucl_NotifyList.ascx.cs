using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace SQM.Website
{

    public partial class Ucl_NotifyList : System.Web.UI.UserControl
    {
        public event ItemUpdateID OnListSaveClick;
        public event EditItemClick OnListSelectClick;


        private List<string> scopeList
        {
            get { return ViewState["scopeList"] == null ? new List<string>() : (List<string>)ViewState["scopeList"]; }
            set { ViewState["scopeList"] = value; }
        }
        private List<PERSON> staticPersonList
        {
            get { return ViewState["notifyPersonList"] == null ? new List<PERSON>() : (List<PERSON>)ViewState["notifyPersonList"]; }
            set { ViewState["notifyPersonList"] = value; }
        }

 
        #region notifylist
        protected void btnNotifySave_Click(object sender, EventArgs e)
        {
            UpdateNotifyList();

            if (OnListSaveClick != null)
            {
                OnListSaveClick(0);
            }
        }

        public void BindNotifyList(PSsqmEntities ctx, decimal companyID, decimal busorgID, decimal plantID, List<TaskRecordType> recordTypeList) 
        {
            ToggleVisible(pnlNotifyList);
            NOTIFY notify = null;

            scopeList = recordTypeList.Select(l => ((int)l).ToString()).ToList();
            List<NOTIFY> notifyList = new List<NOTIFY>();

            hfNotifyCompanyID.Value = SessionManager.EffLocation.Company.COMPANY_ID.ToString();
            hfNotifyBusorgID.Value = busorgID.ToString();
            hfNotifyPlantID.Value = plantID.ToString();
            hfNotifyB2BID.Value = "0";

            if (plantID > 0)
            {
                staticPersonList = SQMModelMgr.SelectPlantPersonList(SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.EffLocation.Plant.PLANT_ID, "");
                notifyList = SQMModelMgr.SelectNotifyList(ctx, 0, SessionManager.EffLocation.Plant.PLANT_ID, 0, scopeList);
            }
            else
            {
                staticPersonList = SQMModelMgr.SelectBusOrgPersonList(SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID);
                notifyList = SQMModelMgr.SelectNotifyList(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, 0, 0, scopeList);
            }
      
            foreach (string scope in scopeList)
            {
                if ((notify = notifyList.Where(l => l.NOTIFY_SCOPE == scope).FirstOrDefault()) == null)
                {
                    notify = new NOTIFY();
                    notify.NOTIFY_SCOPE = scope; // xlat.Key;
                    notify.COMPANY_ID = companyID;
                    if (busorgID > 0)
                        notify.BUS_ORG_ID = busorgID;
                    if (plantID > 0)
                        notify.PLANT_ID = plantID;
                    notifyList.Add(notify);
                }
            }

            notifyList = (from n in scopeList join s in notifyList on n equals s.NOTIFY_SCOPE select s).ToList();
            gvNotifyList.DataSource = notifyList;
            gvNotifyList.DataBind();
        }

        public void gvNotifyList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                RadComboBox rdl1, rdl2;

                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();
                try
                {
                    HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfScope");
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblScope");
                    lbl.Text = WebSiteCommon.GetXlatValue("taskRecordType", hf.Value);
                    lbl = (Label)e.Row.Cells[0].FindControl("lblScopeDesc");
                    lbl.Text = WebSiteCommon.GetXlatValueLong("taskRecordType", hf.Value);

                    TaskRecordType scope = (TaskRecordType)Enum.Parse(typeof(TaskRecordType), hf.Value, true);

                    if (scope == TaskRecordType.InternalQualityIncident ||
                        scope == TaskRecordType.CustomerQualityIncident ||
                        scope == TaskRecordType.SupplierQualityIncident ||
                        scope == TaskRecordType.HealthSafetyIncident    ||
                        scope == TaskRecordType.PreventativeAction)
                    {
                        rdl1 = (RadComboBox)e.Row.Cells[0].FindControl("ddlNotify1");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfNotify1");
                        if (scope == TaskRecordType.HealthSafetyIncident  ||  scope == TaskRecordType.PreventativeAction)
                            SQMBasePage.SetPersonList(rdl1, SQMModelMgr.FilterPersonListByAppContext(staticPersonList, "EHS"), "", 20);
                        else
                            SQMBasePage.SetPersonList(rdl1, SQMModelMgr.FilterPersonListByAppContext(staticPersonList, "SQM"), "", 20);

                        SQMBasePage.DisplayControlValue(rdl1, hf.Value, PageUseMode.EditEnabled, "");
                        rdl1.Visible = true;

                        rdl2 = (RadComboBox)e.Row.Cells[0].FindControl("ddlNotify2");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfNotify2");
                        if (scope == TaskRecordType.HealthSafetyIncident  ||  scope == TaskRecordType.PreventativeAction)
                            SQMBasePage.SetPersonList(rdl2, SQMModelMgr.FilterPersonListByAppContext(staticPersonList, "EHS"), "", 20);
                        else
                            SQMBasePage.SetPersonList(rdl2, SQMModelMgr.FilterPersonListByAppContext(staticPersonList, "SQM"), "", 20);
                        SQMBasePage.DisplayControlValue(rdl2, hf.Value, PageUseMode.EditEnabled, "");
                        rdl2.Visible = true;
                    }

                    if (scope == TaskRecordType.ProfileInput ||
                        scope == TaskRecordType.ProfileInputApproval ||
                        scope == TaskRecordType.HealthSafetyIncident ||
                         scope == TaskRecordType.PreventativeAction  ||
                        scope == TaskRecordType.ProblemCase)
                    {
                        SETTINGS sets = SQMSettings.GetSetting("COMPANY", "ESCALATEANYUSER");

                        rdl1 = (RadComboBox)e.Row.Cells[0].FindControl("ddlEscalate1");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfEscalate1");
                        SQMBasePage.SetPersonList(rdl1, staticPersonList.Where(l => l.RCV_ESCALATION == true  ||  (sets != null  &&  sets.VALUE.ToUpper() == "Y")).ToList(), "", 20);
                        SQMBasePage.DisplayControlValue(rdl1, hf.Value, PageUseMode.EditEnabled, "");
                        rdl1.Visible = true;

                        rdl2 = (RadComboBox)e.Row.Cells[0].FindControl("ddlEscalate2");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfEscalate2");
                        SQMBasePage.SetPersonList(rdl2, staticPersonList.Where(l => l.RCV_ESCALATION == true || (sets != null && sets.VALUE.ToUpper() == "Y")).ToList(), "", 20);
                        SQMBasePage.DisplayControlValue(rdl2, hf.Value, PageUseMode.EditEnabled, "");
                        rdl2.Visible = true;

                        rdl1 = (RadComboBox)e.Row.Cells[0].FindControl("ddlEscalateDays1");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfEscalateDays1");
                        rdl1.Items.Add(new RadComboBoxItem("", ""));
                        rdl1.Items.AddRange(WebSiteCommon.PopulateComboBoxListNums(1, 14, rdl1.EmptyMessage));
                        SQMBasePage.DisplayControlValue(rdl1, hf.Value, PageUseMode.EditEnabled, "");
                        rdl1.Visible = true;

                        rdl2 = (RadComboBox)e.Row.Cells[0].FindControl("ddlEscalateDays2");
                        hf = (HiddenField)e.Row.Cells[0].FindControl("hfEscalateDays2");
                        rdl2.Items.Add(new RadComboBoxItem("", ""));
                        rdl2.Items.AddRange(WebSiteCommon.PopulateComboBoxListNums(1, 14, rdl2.EmptyMessage));
                        SQMBasePage.DisplayControlValue(rdl2, hf.Value, PageUseMode.EditEnabled, "");
                        rdl2.Visible = true;
                    }
                }
                catch
                {
                }
            }
        }

        public int UpdateNotifyList()
        {
            int status = 0;
            PSsqmEntities ctx = new PSsqmEntities();
           
            List<NOTIFY> notifyList = new List<NOTIFY>();

            if (Convert.ToDecimal(hfNotifyPlantID.Value) > 0)
            {
                notifyList = SQMModelMgr.SelectNotifyList(ctx, 0, SessionManager.EffLocation.Plant.PLANT_ID, 0, scopeList);
            }
            else
            {
                notifyList = SQMModelMgr.SelectNotifyList(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, 0, 0, scopeList);
            }

            NOTIFY nf = null;
            foreach (string scope in scopeList)
            {
                if ((nf = notifyList.Where(l => l.NOTIFY_SCOPE == scope).FirstOrDefault()) == null)
                {
                    nf = new NOTIFY();
                    nf.NOTIFY_SCOPE = scope; // xlat.Key;
                    notifyList.Add(nf);
                }
            }

            RadComboBox rdl1, rdl2;
            HiddenField hf;
            int nrow = -1;
            foreach (NOTIFY notify in notifyList)
            {
                GridViewRow row = gvNotifyList.Rows[++nrow];

                notify.COMPANY_ID = Convert.ToDecimal(hfNotifyCompanyID.Value);
                if (!string.IsNullOrEmpty(hfNotifyBusorgID.Value))
                    notify.BUS_ORG_ID = Convert.ToDecimal(hfNotifyBusorgID.Value);
                if (hfNotifyPlantID.Value != "0")
                    notify.PLANT_ID = Convert.ToDecimal(hfNotifyPlantID.Value);
                if (hfNotifyB2BID.Value != "0")
                    notify.B2B_ID = Convert.ToDecimal(hfNotifyB2BID.Value);
                hf = (HiddenField)row.FindControl("hfScope");
                notify.NOTIFY_SCOPE = hf.Value;

                rdl1 = (RadComboBox)row.FindControl("ddlNotify1");
                if (!string.IsNullOrEmpty(rdl1.SelectedValue))
                    notify.NOTIFY_PERSON1 = Convert.ToDecimal(rdl1.SelectedValue);
                else
                    notify.NOTIFY_PERSON1 = null;

                rdl2 = (RadComboBox)row.FindControl("ddlNotify2");
                if (!string.IsNullOrEmpty(rdl2.SelectedValue))
                    notify.NOTIFY_PERSON2 = Convert.ToDecimal(rdl2.SelectedValue);
                else
                    notify.NOTIFY_PERSON2 = null;

                rdl1 = (RadComboBox)row.FindControl("ddlEscalate1");
                if (!string.IsNullOrEmpty(rdl1.SelectedValue))
                {
                    notify.ESCALATE_PERSON1 = Convert.ToDecimal(rdl1.SelectedValue);
                    rdl1 = (RadComboBox)row.FindControl("ddlEscalateDays1");
                    notify.ESCALATE_DAYS1 = Convert.ToInt32(rdl1.SelectedValue);
                }
                else
                {
                    notify.ESCALATE_PERSON1 = null;
                    notify.ESCALATE_DAYS1 = 0;
                }

                rdl2 = (RadComboBox)row.FindControl("ddlEscalate2");
                if (!string.IsNullOrEmpty(rdl2.SelectedValue))
                {
                    notify.ESCALATE_PERSON2 = Convert.ToDecimal(rdl2.SelectedValue);
                    rdl2 = (RadComboBox)row.FindControl("ddlEscalateDays2");
                    notify.ESCALATE_DAYS2 = Convert.ToInt32(rdl2.SelectedValue);
                }
                else
                {
                    notify.ESCALATE_PERSON2 = null;
                    notify.ESCALATE_DAYS2 = 0;
                }
            }

            status = SQMModelMgr.UpdateNotifyList(ctx, notifyList);

            if (status < 0)
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveError');", true);
            else
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);

            return status;
        }

        #endregion


        #region common
        public void ToggleVisible(Panel pnlTarget)
        {
           pnlTasksResponsibleList.Visible = pnlNotifyList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        #endregion
    }

}