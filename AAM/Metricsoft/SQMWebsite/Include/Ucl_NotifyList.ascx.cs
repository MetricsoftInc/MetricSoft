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
		public event GridActionCommand OnNotifyActionCommand;
		public event GridItemClick OnNotifyActionClick;

		private List<XLAT> XLATList
		{
			get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
			set { ViewState["XLATList"] = value; }
		}


		#region notifyaction

		public void BindNotfyPlan(List<NOTIFYACTION> notifyItemList, BusinessLocation businessLocation, string context)
		{

			XLATList = SQMBasePage.SelectXLATList(new string[4] { "NOTIFY_SCOPE", "NOTIFY_SCOPE_TASK", "NOTIFY_TASK_STATUS", "NOTIFY_TIMING" });

			hfNotifyActionContext.Value = context;
			if (context != "company")
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

			foreach (PRIVGROUP pg in SQMModelMgr.SelectPrivGroupList(new SysPriv[2] { SysPriv.admin, SysPriv.notify }, SysScope.incident, "A").ToList())
			{
				ddlNotifyPrivGroup.Items.Add(new RadComboBoxItem(SQMModelMgr.FormatPrivGroup(pg), pg.PRIV_GROUP));
			}

			pnlNotifyAction.Visible = true;

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
					lnk.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE" && x.XLAT_CODE == notifyAction.NOTIFY_SCOPE).FirstOrDefault().DESCRIPTION_SHORT;

					lbl = (Label)item.FindControl("lblScopeTask");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == notifyAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION_SHORT;

					lbl = (Label)item.FindControl("lblScopeStatus");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_STATUS" && x.XLAT_CODE == notifyAction.TASK_STATUS).FirstOrDefault().DESCRIPTION_SHORT;

					lbl = (Label)item.FindControl("lblNotifyTiming");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TIMING" && x.XLAT_CODE == notifyAction.NOTIFY_TIMING.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

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

					ddlNotifyPrivGroup.ClearCheckedItems();
					RadComboBoxItem ri = null;
					foreach (string sv in notifyAction.NOTIFY_DIST.Split(','))
					{
						if (!string.IsNullOrEmpty(sv)  &&  (ri = ddlNotifyPrivGroup.FindItemByValue(sv)) != null)
							ri.Checked = true;
					}

					ddlEdit_OnIndexChanged(null, null);
					btnDelete.Visible = true;
				}

				string script = "function f(){OpenNotifyEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}
			catch
			{
			}
		}

		protected void ddlEdit_OnIndexChanged(object sender, EventArgs e)
		{

			if (new string[2] { "350", "380" }.Contains(ddlScopeTask.SelectedValue))
			{
				ddlScopeStatus.Enabled = true;
			}
			else
			{
				ddlScopeStatus.SelectedValue = "380";
				ddlScopeStatus.Enabled = false;
				ddlScopeTiming.SelectedValue = "0";
				ddlScopeTiming.Enabled = false;
			}

			if (new string[1] { "400" }.Contains(ddlScopeStatus.SelectedValue))
			{
				ddlScopeTiming.Enabled = true;
			}
			else
			{
				ddlScopeTiming.SelectedValue = "0";
				ddlScopeTiming.Enabled = false;
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
				else if (hfNotifyActionContext.Value == "busorg")  // org level
				{  // org level
					notifyAction.BUS_ORG_ID = Convert.ToDecimal(hfNotifyActionBusLoc.Value);
				}
				else
				{
					; // assume comany level assignes null for both org and plant
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
			notifyAction.NOTIFY_DIST = "";
			foreach (string sv in SQMBasePage.GetComboBoxSelectedValues(ddlNotifyPrivGroup))
			{
				notifyAction.NOTIFY_DIST += (string.IsNullOrEmpty(notifyAction.NOTIFY_DIST) ? "" : ",") + sv;
			}

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
							lnk.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE" && x.XLAT_CODE == notifyAction.NOTIFY_SCOPE).FirstOrDefault().DESCRIPTION_SHORT;
							
							Label lbl = (Label)item.FindControl("lblScopeTask");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_SCOPE_TASK" && x.XLAT_CODE == notifyAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION_SHORT;

							lbl = (Label)item.FindControl("lblScopeStatus");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TASK_STATUS" && x.XLAT_CODE == notifyAction.TASK_STATUS).FirstOrDefault().DESCRIPTION_SHORT;

							lbl = (Label)item.FindControl("lblNotifyTiming");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "NOTIFY_TIMING" && x.XLAT_CODE == notifyAction.NOTIFY_TIMING.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

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

			ddlNotifyScope.SelectedIndex = 0;
			ddlScopeTask.SelectedIndex = 0;
			ddlScopeStatus.SelectedIndex = 0;
			ddlScopeTiming.SelectedIndex = 0;
			ddlNotifyPrivGroup.ClearCheckedItems();

			btnDelete.Visible = false;

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

		protected void OnDeleteNotifyAction_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(hfNotifyActionID.Value))  // delete if an existing record
			{
				SQMModelMgr.DeleteNotifyAction(new PSsqmEntities(), Convert.ToDecimal(hfNotifyActionID.Value));
				hfNotifyActionID.Value = "";
				if (OnNotifyActionCommand != null)
				{
					OnNotifyActionCommand("delete");
				}
			}
		}
		#endregion

    }

}