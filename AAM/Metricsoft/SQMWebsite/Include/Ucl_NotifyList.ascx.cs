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
		public event GridActionCommand OnNotifyActionCommand;
		public event GridItemClick OnNotifyActionClick;


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
		private List<XLAT> XLATList
		{
			get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
			set { ViewState["XLATList"] = value; }
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

		#region notifyplan

		public void BindNotfyPlan(List<NOTIFYACTION> notifyItemList, BusinessLocation businessLocation, string context)
		{

			XLATList = SQMBasePage.SelectXLATList(new string[4] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "NOTIFY_TIMING" });

			hfNotifyActionContext.Value = context;
			hfNotifyActionBusLoc.Value = context == "plant" ? businessLocation.Plant.PLANT_ID.ToString() : businessLocation.BusinessOrg.BUS_ORG_ID.ToString();

			ddlNotifyScope.DataSource = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE").ToList();
			ddlNotifyScope.DataValueField = "XLAT_CODE";
			ddlNotifyScope.DataTextField = "DESCRIPTION";
			ddlNotifyScope.DataBind();

			ddlScopeTask.DataSource = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK").ToList();
			ddlScopeTask.DataValueField = "XLAT_CODE";
			ddlScopeTask.DataTextField = "DESCRIPTION";
			ddlScopeTask.DataBind();

			ddlScopeStatus.DataSource = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_STATUS").ToList();
			ddlScopeStatus.DataValueField = "XLAT_CODE";
			ddlScopeStatus.DataTextField = "DESCRIPTION";
			ddlScopeStatus.DataBind();

			ddlScopeTiming.DataSource = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TIMING").ToList();
			ddlScopeTiming.DataValueField = "XLAT_CODE";
			ddlScopeTiming.DataTextField = "DESCRIPTION";
			ddlScopeTiming.DataBind();

			if (ddlNotifyJobcode.Items.Count == 0)
			{
				ddlNotifyJobcode.Items.Insert(0, new RadComboBoxItem("", ""));
				foreach (JOBCODE jc in SQMModelMgr.SelectJobcodeList("A", "").OrderBy(j => j.JOB_DESC).ToList())
				{
					ddlNotifyJobcode.Items.Add(new RadComboBoxItem(SQMModelMgr.FormatJobcode(jc), jc.JOBCODE_CD));
				}
			}

			pnlNotifyAction.Visible = true;

			hfNotifyActionContext.Value = context;

			rgNotifyAction.DataSource = notifyItemList;
			rgNotifyAction.DataBind();
		}

		protected void rgNotifyAction_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				try
				{
					GridDataItem item = (GridDataItem)e.Item;
					NOTIFYACTION  notifyAction = (NOTIFYACTION)e.Item.DataItem;

					Label lbl;

					HiddenField hf = (HiddenField)item.FindControl("hfNotifyItemID");
					hf.Value = notifyAction.NOTIFYACTION_ID.ToString();

					LinkButton lnk = (LinkButton)item.FindControl("lnkNotifyItem");
					lnk.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE" && x.XLAT_CODE == notifyAction.NOTIFY_SCOPE).FirstOrDefault().DESCRIPTION;

					lbl = (Label)item.FindControl("lblScopeTask");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == notifyAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION;

					lbl = (Label)item.FindControl("lblScopeStatus");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_STATUS" && x.XLAT_CODE == notifyAction.TASK_STATUS).FirstOrDefault().DESCRIPTION;

					lbl = (Label)item.FindControl("lblNotifyTiming");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TIMING" && x.XLAT_CODE == notifyAction.NOTIFY_TIMING.ToString()).FirstOrDefault().DESCRIPTION;

					lbl = (Label)item.FindControl("lblNotifyDist");
					lbl.Text = notifyAction.NOTIFY_DIST;

				}
				catch
				{
				}
			}
		}

		protected void rgNotifyAction_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			if (OnNotifyActionCommand != null)
			{
				OnNotifyActionCommand("sort");
			}
		}
		protected void rgNotifyAction_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			if (OnNotifyActionCommand != null)
			{
				OnNotifyActionCommand("index");
			}
		}
		protected void rgNotifyAction_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			if (OnNotifyActionCommand != null)
			{
				OnNotifyActionCommand("size");
			}
		}

		protected void lnklNotifyItem_Click(object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;

			try
			{
				NOTIFYACTION notifyAction = SQMModelMgr.LookupNotifyAction(new PSsqmEntities(), Convert.ToDecimal(lnk.CommandArgument));
				if (notifyAction != null)
				{
					hfNotifyActionID.Value = notifyAction.NOTIFYACTION_ID.ToString();
					if (ddlNotifyScope.FindItemByValue(notifyAction.NOTIFY_SCOPE) != null)
						ddlNotifyScope.SelectedValue = notifyAction.NOTIFY_SCOPE;
					if (ddlScopeTask.FindItemByValue(notifyAction.SCOPE_TASK) != null)
						ddlScopeTask.SelectedValue = notifyAction.SCOPE_TASK;
					if (ddlScopeStatus.FindItemByValue(notifyAction.TASK_STATUS) != null)
						ddlScopeStatus.SelectedValue = notifyAction.TASK_STATUS;
					if (ddlScopeTiming.FindItemByValue(notifyAction.NOTIFY_TIMING.ToString()) != null)
						ddlScopeTiming.SelectedValue = notifyAction.NOTIFY_TIMING.ToString();
					if (ddlNotifyJobcode.FindItemByValue(notifyAction.NOTIFY_DIST) != null)
						ddlNotifyJobcode.SelectedValue = notifyAction.NOTIFY_DIST;
				}

				string script = "function f(){OpenNotifyEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}
			catch
			{
			}
		}

		private void SaveNotifyItem()
		{
			PSsqmEntities ctx = new PSsqmEntities();
			NOTIFYACTION notifyAction = null;
			bool isNew = false;

			if (string.IsNullOrEmpty(hfNotifyActionID.Value))  // add new item
			{
				notifyAction = new NOTIFYACTION();
				if (hfNotifyActionContext.Value == "plant")  // plant level
				{
					notifyAction.PLANT_ID = Convert.ToDecimal(hfNotifyActionBusLoc.Value);
				}
				else
				{  // plant level
					notifyAction.BUS_ORG_ID = Convert.ToDecimal(hfNotifyActionBusLoc.Value);
				}
				isNew = true;
			}
			else
			{
				notifyAction = SQMModelMgr.LookupNotifyAction(ctx, Convert.ToDecimal(hfNotifyActionID.Value));
			}

			notifyAction.NOTIFY_SCOPE = ddlNotifyScope.SelectedValue;
			notifyAction.SCOPE_TASK = ddlScopeTask.SelectedValue;
			notifyAction.TASK_STATUS = ddlScopeStatus.SelectedValue;
			notifyAction.NOTIFY_TIMING = Convert.ToInt32(ddlScopeTiming.SelectedValue);
			notifyAction.NOTIFY_DIST = ddlNotifyJobcode.SelectedValue;

			if ((notifyAction = SQMModelMgr.UpdateNotifyAction(ctx, notifyAction)) != null)
			{
				if (isNew)
				{
					if (OnNotifyActionCommand != null)
					{
						OnNotifyActionCommand("add");
					}
				}
				else
				{
					foreach (GridDataItem item in rgNotifyAction.Items)
					{
						LinkButton lnk = (LinkButton)item.FindControl("lnkNotifyItem");
						if (lnk.CommandArgument == hfNotifyActionID.Value)
						{
							lnk.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE" && x.XLAT_CODE == notifyAction.NOTIFY_SCOPE).FirstOrDefault().DESCRIPTION;
							Label lbl = (Label)item.FindControl("lblScopeTask");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == notifyAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION;

							lbl = (Label)item.FindControl("lblScopeStatus");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_STATUS" && x.XLAT_CODE == notifyAction.TASK_STATUS).FirstOrDefault().DESCRIPTION;

							lbl = (Label)item.FindControl("lblNotifyTiming");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TIMING" && x.XLAT_CODE == notifyAction.NOTIFY_TIMING.ToString()).FirstOrDefault().DESCRIPTION;

							lbl = (Label)item.FindControl("lblNotifyDist");
							lbl.Text = notifyAction.NOTIFY_DIST;
						}
					}
				}
			}
		}

		protected void btnNotifyItemAdd_Click(object sender, EventArgs e)
		{
			hfNotifyActionID.Value = "";

			string script = "function f(){OpenNotifyEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void OnCancelNotifyAction_Click(object sender, EventArgs e)
		{
			hfNotifyActionID.Value = "";
		}

		protected void OnSaveNotifyAction_Click(object sender, EventArgs e)
		{
			SaveNotifyItem();
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