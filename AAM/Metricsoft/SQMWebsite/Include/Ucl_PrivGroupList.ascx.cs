using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace SQM.Website
{

	public partial class Ucl_PrivGroupList : System.Web.UI.UserControl
    {
		public event GridActionCommand OnPrivGroupCommand;
		public event GridItemClick OnPrivGroupClick;

		string[] scopes;
		int priv;

		private List<XLAT> XLATList
		{
			get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
			set { ViewState["XLATList"] = value; }
		}

		private List<SETTINGS> defaultPrivs
		{
			get { return ViewState["defaultPrivs"] == null ? new List<SETTINGS>() : (List<SETTINGS>)ViewState["defaultPrivs"]; }
			set { ViewState["defaultPrivs"] = value; }
		}

		#region privgroup

		public void BindPrivGroups(List<PRIVGROUP> privGroupList, BusinessLocation businessLocation, string context)
		{
			hfPrivGroupContext.Value = context;

			if (ddlPrivGroupStatus.Items.Count == 0)
			{
				XLATList = SQMBasePage.SelectXLATList(new string[3] { "ACTIVE_STATUS", "PRIV_PRIV", "PRIV_SCOPE" });

				ddlPrivGroupStatus.DataSource = XLATList.Where(x => x.XLAT_GROUP == "ACTIVE_STATUS").ToList();
				ddlPrivGroupStatus.DataValueField = "XLAT_CODE";
				ddlPrivGroupStatus.DataTextField = "DESCRIPTION";
				ddlPrivGroupStatus.DataBind();
			}

			if (ddlScope.Items.Count == 0)
			{
				ddlScope.DataSource = XLATList.Where(x => x.XLAT_GROUP == "PRIV_SCOPE").ToList();
				ddlScope.DataValueField = "XLAT_CODE";
				ddlScope.DataTextField = "DESCRIPTION";
				ddlScope.DataBind();
			}

			if (ddlPriviledge.Items.Count==0)
			{
				ddlPriviledge.DataSource = XLATList.Where(x => x.XLAT_GROUP == "PRIV_PRIV").ToList();
				ddlPriviledge.DataValueField = "XLAT_CODE";
				ddlPriviledge.DataTextField = "DESCRIPTION";
				ddlPriviledge.DataBind();
				RadComboBoxItem item = new RadComboBoxItem("", "select a priviledge level");
				ddlPriviledge.Items.Insert(0, item);
				try
				{
					item = ddlPriviledge.FindItemByValue("100");
					item.Enabled = false;
				}
				catch { }
			}

			defaultPrivs = SQMSettings.SelectSettingsGroup("DEFAULT_PRIVS", "");

			pnlPrivGroups.Visible = true;

			rgPrivGroup.DataSource = privGroupList;
			rgPrivGroup.DataBind();
		}

		protected void rgPrivGroup_ItemDataBound(object sender, GridItemEventArgs e)
		{
			// is there anything that we need to do here?
			if (e.Item is GridDataItem)
			{
				try
				{
					GridDataItem item = (GridDataItem)e.Item;
					PRIVGROUP privGroup = (PRIVGROUP)e.Item.DataItem;

					Label lbl;

					LinkButton lnk = (LinkButton)item.FindControl("lnkPrivGroupItem");

					lbl = (Label)item.FindControl("lblGroupStatus");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "ACTIVE_STATUS" && x.XLAT_CODE == privGroup.STATUS).FirstOrDefault().DESCRIPTION_SHORT;

					lnk = (LinkButton)e.Item.FindControl("lnkAddPriv");
					lnk.CommandArgument = privGroup.PRIV_GROUP.ToString();
				}
				catch
				{
				}
			}
		}

		protected void rgPrivGroup_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			if (OnPrivGroupCommand != null)
			{
				OnPrivGroupCommand("sort");
			}
		}
		protected void rgPrivGroup_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			if (OnPrivGroupCommand != null)
			{
				OnPrivGroupCommand("index");
			}
		}
		protected void rgPrivGroup_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			if (OnPrivGroupCommand != null)
			{
				OnPrivGroupCommand("size");
			}
		}

		protected void lnklPrivGroupItem_Click(object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;

			try
			{
				PRIVGROUP privGroup = SQMModelMgr.LookupPrivGroup(new PSsqmEntities(), lnk.CommandArgument.ToString(), false);
				if (privGroup != null)
				{
					hfPrivGroupID.Value = privGroup.PRIV_GROUP;
					tbEditPrivGroup.Text = privGroup.PRIV_GROUP;
					tbEditPrivGroup.Enabled = false;
					tbEditDescription.Text = privGroup.DESCRIPTION;
					if (ddlPrivGroupStatus.FindItemByValue(privGroup.STATUS) != null)
						ddlPrivGroupStatus.SelectedValue = privGroup.STATUS;
					ddlEdit_OnIndexChanged(null, null);
					//btnDelete.Visible = true;
				}

				string script = "function f(){OpenPrivGroupEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}
			catch
			{
			}
		}

		protected void ddlEdit_OnIndexChanged(object sender, EventArgs e)
		{
			// do we actually need to do anything here?
			//if (new string[2] { "350", "380" }.Contains(ddlScopeTask.SelectedValue))
			//{
			//	ddlScopeStatus.Enabled = true;
			//}
			//else
			//{
			//	ddlScopeStatus.SelectedValue = "380";
			//	ddlScopeStatus.Enabled = false;
			//	ddlScopeTiming.SelectedValue = "0";
			//	ddlScopeTiming.Enabled = false;
			//}

		}

		private void SavePrivGroupItem()
		{
			PSsqmEntities ctx = new PSsqmEntities();
			PRIVGROUP privGroup = null;
			bool isNew = false;

			if (string.IsNullOrEmpty(hfPrivGroupID.Value))  // add new item
			{
				privGroup = new PRIVGROUP();

				isNew = true;
			}
			else
			{
				privGroup = SQMModelMgr.LookupPrivGroup(ctx, hfPrivGroupID.Value.ToString(), false);
			}

			privGroup.PRIV_GROUP = tbEditPrivGroup.Text.ToString().Trim();
			privGroup.DESCRIPTION = tbEditDescription.Text.ToString().Trim();
			privGroup.STATUS = ddlPrivGroupStatus.SelectedValue.ToString().Trim();

			if ((privGroup = SQMModelMgr.UpdatePrivGroup(ctx, privGroup)) != null)
			{
				if (isNew)
				{
					// We need to add the default privs
					PRIVLIST privList = null;
					if (defaultPrivs == null)
						defaultPrivs = SQMSettings.SelectSettingsGroup("DEFAULT_PRIVS", "");
					if (defaultPrivs.Count > 0)
					{
						foreach (SETTINGS setting in defaultPrivs)
						{
							try
							{
								priv = Convert.ToInt16(setting.SETTING_CD.ToString());
								scopes = setting.VALUE.ToString().Split(',');
								for (int i = 0; i < scopes.Count(); i++)
								{
									privList = SQMModelMgr.LookupPrivList(privGroup.PRIV_GROUP, priv, scopes[i], true);
									privList.PRIV_GROUP = privGroup.PRIV_GROUP;
									privList.PRIV = priv;
									privList.SCOPE = scopes[i].Trim();
									privList = SQMModelMgr.UpdatePrivList(ctx, privList);
								}
							}
							catch
							{ }
						}
					}
					else
					{
						// if there are no defult settings, we are going to force dashboard, inbox and incident
					}
					if (OnPrivGroupCommand != null)
					{
						OnPrivGroupCommand("add");
					}
				}
				else
				{
					foreach (GridDataItem item in rgPrivGroup.Items)
					{
						LinkButton lnk = (LinkButton)item.FindControl("lnkPrivGroupItem");
						if (lnk.CommandArgument == hfPrivGroupID.Value)
						{
							// update the list item without doing a complete refresh of the list
							Label lbl = new Label();
							lbl = (Label)item.FindControl("lblDescription");
							lbl.Text = privGroup.DESCRIPTION;

							lbl = (Label)item.FindControl("lblGroupStatus");
							lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "ACTIVE_STATUS" && x.XLAT_CODE == privGroup.STATUS.ToString()).FirstOrDefault().DESCRIPTION_SHORT;
						}
					}
				}
			}
		}

		protected void btnPrivGroupAdd_Click(object sender, EventArgs e)
		{
			hfPrivGroupID.Value = "";
			tbEditPrivGroup.Text = "";
			tbEditPrivGroup.Enabled = true;
			tbEditDescription.Text = "";
			ddlPrivGroupStatus.SelectedIndex = 0;

			//btnDelete.Visible = false;

			string script = "function f(){OpenPrivGroupEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void OnCancelPrivGroup_Click(object sender, EventArgs e)
		{
			hfPrivGroupID.Value = "";
		}

		protected void OnSavePrivGroup_Click(object sender, EventArgs e)
		{
			SavePrivGroupItem();
		}

		//protected void OnDeletePrivGroup_Click(object sender, EventArgs e)
		//{
		//	if (!string.IsNullOrEmpty(hfPrivGroupID.Value))  // delete if an existing record
		//	{
		//		SQMModelMgr.DeletePrivGroup(new PSsqmEntities(), hfPrivGroupID.Value);
		//		hfPrivGroupID.Value = "";
		//		if (OnPrivGroupCommand != null)
		//		{
		//			OnPrivGroupCommand("delete");
		//		}
		//	}
		//}
		#endregion

		#region privlist
		protected void rgPrivList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			try
			{
				GridDataItem parentItem = ((sender as RadGrid).NamingContainer as GridNestedViewItem).ParentItem as GridDataItem;
				if (parentItem != null)
				{
					string privGroup = parentItem.GetDataKeyValue("PRIV_GROUP").ToString();

					List<PRIVLIST> privlist = SQMModelMgr.SelectPrivList(privGroup);

					(sender as RadGrid).DataSource = privlist;
				}
			}
			catch (Exception ex) { }
		}

		protected void rgPrivList_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				if (defaultPrivs == null)
					defaultPrivs = SQMSettings.SelectSettingsGroup("DEFAULT_PRIVS", "");

				GridDataItem item = (GridDataItem)e.Item;
				Label lbl;
				LinkButton lnk;

				PRIVLIST data = (PRIVLIST)item.DataItem;

				if (data.PRIV_GROUP != null)
				{
					lbl = (Label)item.FindControl("lblPrivDesc");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "PRIV_PRIV" && x.XLAT_CODE == data.PRIV.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

					lbl = (Label)item.FindControl("lblScope");
					lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "PRIV_SCOPE" && x.XLAT_CODE == data.SCOPE.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

					lnk = (LinkButton)item.FindControl("lnkDeletePriv");
					lnk.CommandArgument = data.PRIV_GROUP.ToString() + "~" + data.PRIV.ToString() + "~" + data.SCOPE.ToString();
					// hide the link for the default privs that can't be removed
					if (defaultPrivs.Count > 0)
					{
						foreach (SETTINGS setting in defaultPrivs)
						{
							try
							{
								priv = Convert.ToInt16(setting.SETTING_CD.ToString());
								scopes = setting.VALUE.ToString().Split(',');
								for (int i = 0; i < scopes.Count(); i++)
								{
									if (data.PRIV == Convert.ToInt16(setting.SETTING_CD.Trim()) && data.SCOPE == scopes[i].Trim())
									{
										lnk.Visible = false;
									}
								}
							}
							catch { }
						}
					}
				}
			}
		}

		protected void rgPrivList_ItemCommand(object sender, GridCommandEventArgs e)
		{
			// add this back to the grid to hit this code... OnItemCommand="rgPrivList_ItemCommand" 
			if (e.CommandName == RadGrid.ExpandCollapseCommandName)
			{
				foreach (GridItem item in e.Item.OwnerTableView.Items)
				{
					//if (item.Expanded && item != e.Item && item.Parent.ID != e.Item.Parent.ID)
					if (item.Expanded && item != e.Item)
					{
						item.Expanded = false;
					}
				}
			}
		}

		protected void ddlPriviledge_OnIndexChanged(object sender, EventArgs e)
		{
			// do we actually need to do anything here?
			//if (new string[2] { "350", "380" }.Contains(ddlScopeTask.SelectedValue))
			//{
			//	ddlScopeStatus.Enabled = true;
			//}
			//else
			//{
			//	ddlScopeStatus.SelectedValue = "380";
			//	ddlScopeStatus.Enabled = false;
			//	ddlScopeTiming.SelectedValue = "0";
			//	ddlScopeTiming.Enabled = false;
			//}

		}

		protected void ddlScope_OnIndexChanged(object sender, EventArgs e)
		{
			RadComboBox ddl = (RadComboBox)sender;
			switch (ddl.SelectedValue.ToString().Trim().ToLower())
			{
				case "system":
					ddlPriviledge.SelectedValue = "100";
					ddlPriviledge.Enabled = false;
					break;
				case "busorg":
				case "busloc":
					ddlPriviledge.SelectedValue = "200";
					ddlPriviledge.Enabled = false;
					break;
				default:
					try
					{
						RadComboBoxItem item = ddlPriviledge.FindItemByValue("100");
						item.Enabled = false;
					}
					catch { }
					ddlPriviledge.SelectedIndex = 0;
					ddlPriviledge.Enabled = true;
					break;
			}
		}

		protected void lnkAddPriv_Click(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			hfPrivPrivGroupID.Value = lnk.CommandArgument.ToString();
			ddlPriviledge.SelectedIndex = 0;
			ddlScope.SelectedIndex = 0;

			//btnDeletePriv.Visible = false;

			string script = "function f(){OpenPrivEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void OnCancelPriv_Click(object sender, EventArgs e)
		{
			hfPrivGroupID.Value = "";
		}

		protected void OnSavePriv_Click(object sender, EventArgs e)
		{
			SavePrivItem();
		}

		private void SavePrivItem()
		{
			PSsqmEntities ctx = new PSsqmEntities();
			PRIVLIST privList = null;
			bool isNew = false;

			// should we always check to see if the privgroup is there??? because we don't want to add a duplicate...
			if (string.IsNullOrEmpty(hfPrivPrivGroupID.Value))  // add new item
			{
				//privList = new PRIVLIST();

				isNew = true;
			}
			//else
			//{
				privList = SQMModelMgr.LookupPrivList(hfPrivPrivGroupID.Value.ToString(), Convert.ToInt16(ddlPriviledge.SelectedValue.ToString().Trim()), ddlScope.SelectedValue.ToString().Trim(), true);
			//}

			privList.PRIV_GROUP = hfPrivPrivGroupID.Value.ToString().Trim();
			privList.PRIV = Convert.ToInt16(ddlPriviledge.SelectedValue.ToString().Trim());
			privList.SCOPE = ddlScope.SelectedValue.ToString().Trim();

			if ((privList = SQMModelMgr.UpdatePrivList(ctx, privList)) != null)
			{
				//if (isNew) // they are all new for now... no updates, just deletes
				//{
					if (OnPrivGroupCommand != null)
					{
						OnPrivGroupCommand("add");
					}
				//}
				//else
				//{
					//foreach (GridDataItem item in rgPrivList.Items)
					//{
					//	LinkButton lnk = (LinkButton)item.FindControl("lnkPrivGroupItem");
					//	if (lnk.CommandArgument == hfPrivGroupID.Value)
					//	{
					//		// update the list item without doing a complete refresh of the list
					//		Label lbl = new Label();
					//		lbl = (Label)item.FindControl("lblDescription");
					//		lbl.Text = privGroup.DESCRIPTION;

					//		lbl = (Label)item.FindControl("lblGroupStatus");
					//		lbl.Text = XLATList.Where(x => x.XLAT_GROUP == "ACTIVE_STATUS" && x.XLAT_CODE == privGroup.STATUS.ToString()).FirstOrDefault().DESCRIPTION_SHORT;
					//	}
					//}
				//}
			}
		}

		protected void OnDeletePriv_Click(object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] args = lnk.CommandArgument.Split('~');
			int cnt = args.Count();
			if (cnt > 2 && args[0].Trim().Length > 0 && args[1].Trim().Length > 0 && args[2].Trim().Length > 0)  // delete if an existing record
			{
				SQMModelMgr.DeletePrivList(args[0].Trim(), Convert.ToInt16(args[1].Trim()), args[2].Trim());
				hfPrivGroupID.Value = "";
				if (OnPrivGroupCommand != null)
				{
					OnPrivGroupCommand("delete");
				}
			}
		}
		#endregion
	}

}