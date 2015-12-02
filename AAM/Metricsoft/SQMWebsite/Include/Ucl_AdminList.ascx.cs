using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using System.Data;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;

namespace SQM.Website
{
	public delegate void GridActionCommand(string cmd);
	public delegate void GridActionCommand2(string cmd, decimal id, decimal id2);
	public delegate void GridItemClick(decimal id);
    public delegate void GridItemClick2(decimal id, decimal id2);
    public delegate void ItemUpdateID(decimal id);
    public delegate void ItemSelectDate(DateTime dt);

    public partial class Ucl_AdminList : System.Web.UI.UserControl
    {

        // manage current session object  (formerly was page static variable)
        List<PLANT> PlantList()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is List<PLANT>)
                return (List<PLANT>)SessionManager.CurrentObject;
            else
                return null;
        }
        List<PLANT> SetPlantList(List<PLANT> plantList)
        {
            SessionManager.CurrentObject = plantList;
            return PlantList();
        }

        List<PERSON> UserList()
        {
            if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is List<PERSON>)
                return (List<PERSON>)SessionManager.CurrentObject;
            else
                return null;
        }
        List<PERSON> SetUserList(List<PERSON> userList)
        {
            SessionManager.CurrentObject = userList;
            return UserList();
        }

        public event GridItemClick OnUserListClick;

        public event GridItemClick OnCompanyClick;
        public event GridItemClick OnCompanyCloseClick;
        public event GridItemClick OnOrgClick;
        public event GridItemClick OnPlantClick;

        public event GridItemClick OnPartClick;
        public event GridItemClick OnPartListCloseClick;

        public event GridItemClick OnPlantListClick;
        public event GridItemClick2 OnAddPlantClick;

        public event GridItemClick OnDeptListClick;
        public event GridItemClick OnAddDeptClick;

        public event GridItemClick OnLaborListClick;
        public event GridItemClick OnAddLaborClick;

        public event GridItemClick OnLineListClick;
        public event GridItemClick OnAddLineClick;

        public event GridItemClick OnSaveNonConfClick;
        public event GridItemClick OnCancelNonConfClick;

        private int totalRowCount 
        {
            get { return ViewState["rowCount"] == null ? 0 : (int)ViewState["rowCount"]; }
            set { ViewState["rowCount"] = value; }
        }

        public int TotalRowCount
        {
            get { return totalRowCount; }
            set { ;}
        }

        public void ToggleVisible(Panel pnlTarget)
        {
            pnlUserList.Visible = pnlOrgListRepeater.Visible = pnlCompanyListRepeater.Visible = pnlDeptList.Visible = pnlLaborList.Visible = pnlLineList.Visible = false;
            if (pnlTarget != null)
                pnlTarget.Visible = true;
        }

        #region company
        public Panel CompanyListPanel
        {
            get { return pnlCompanyListRepeater; }
        }
        public Button CloseCompanyListButton
        {
            get { return btnCompanyListClose; }
        }
        protected void btnCompanyListClose_Click(object sender, EventArgs e)
        {
            if (OnCompanyCloseClick != null)
            {
                OnCompanyCloseClick(0);
            }
        }

        public void BindCompanyList(List<COMPANY> companyList)
        {
            ToggleVisible(pnlCompanyListRepeater);
            totalRowCount = 0;
            rptCompanyList.DataSource = companyList;
            rptCompanyList.DataBind();
            SetRepeaterDisplay(rptCompanyList, lblCompanyListEmptyRepeater, divCompanyGVScrollRepeater, 10, (TotalRowCount += rptCompanyList.Items.Count), "scrollAreaLarge");
        }

        protected void lnkCompanyList_Click(object sender, EventArgs e)
        {
            if (OnCompanyClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                decimal companyID;
                if (decimal.TryParse(lnk.CommandArgument.ToString(), out companyID))
                    OnCompanyClick(companyID);
            }
        }

        public void rptCompanyList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                COMPANY company = (COMPANY)e.Item.DataItem;
                CheckBox cb;
              
                try
                {
                    cb = (CheckBox)e.Item.FindControl("cbCompanyIsSupplier");
                    cb.Checked = Convert.ToBoolean(company.IS_SUPPLIER);
                    
                    cb = (CheckBox)e.Item.FindControl("cbCompanyIsCustomer");
                    cb.Checked = Convert.ToBoolean(company.IS_CUSTOMER);
                   
                    Label lbl = (Label)e.Item.FindControl("lblStatusOut");
                    lbl.Text =  WebSiteCommon.GetXlatValue("statusCode", company.STATUS);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region BusOrg

		public Panel OrgListPanelRepeater
		{
			get { return pnlOrgListRepeater; }
		}

		public Repeater OrgListRepeater
		{
			get { return rptBusOrgList; }
		}
		public Label OrgListLabelRepeater
		{
			get { return lblBusOrgListEmptyRepeater; }
		}
		public System.Web.UI.HtmlControls.HtmlGenericControl OrgListDivRepeater
		{
			get { return divGVScrollOrgListRepeater; }
		}


        protected void lnkBusOrgList_Click(object sender, EventArgs e)
        {
            if (OnOrgClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnOrgClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }
 
        protected void lnkPlant_Click(object sender, EventArgs e)
        {
            if (OnPlantClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnPlantClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }

        protected void btnPlantAdd_Click(object sender, EventArgs e)
        {
            if (OnAddPlantClick != null)
            {
               Button btn = (Button)sender;
               OnAddPlantClick(Convert.ToDecimal(btn.CommandArgument.ToString().Trim()), 0);
            }
        }

		public void BindOrgListRepeater(List<BUSINESS_ORG> theList)
		{
            if (theList.Count > 0)
            {
                SETTINGS sets = SQMSettings.GetSetting("COMPANY", "PLANTUSER");
                if (sets == null || sets.VALUE.ToUpper() != "N")
                    SetUserList(SQMModelMgr.SelectPersonList(theList[0].COMPANY_ID, 0, true, false));
            }
            else
            {
                SetUserList(new List<PERSON>());
            }
			ToggleVisible(pnlOrgListRepeater);
			totalRowCount = 0;
			rptBusOrgList.DataSource = theList;
			rptBusOrgList.DataBind();
			SetRepeaterDisplay(rptBusOrgList, lblBusOrgListEmptyRepeater, divGVScrollOrgListRepeater, 8, (TotalRowCount += rptBusOrgList.Items.Count), "scrollAreaLarge");
		}

		public void rptOrgList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
                Label lbl;
                HiddenField hfField;

				try
				{
                    BUSINESS_ORG busOrg = (BUSINESS_ORG)e.Item.DataItem;

                    lbl = (Label)e.Item.FindControl("lblParentBUHdr_out");
                    if (busOrg.PARENT_BUS_ORG_ID == busOrg.BUS_ORG_ID || busOrg.PARENT_BUS_ORG_ID < 1)
                        lbl.Text = "Top Level";
                    else
                    {
                        BUSINESS_ORG parentOrg = null;
                        if ((parentOrg = SQMModelMgr.LookupParentBusOrg(null, busOrg)) != null)
                        {
                            lbl.Text = parentOrg.ORG_NAME;
                        }
                    }

					lbl = (Label)e.Item.FindControl("lblStatus");
					hfField = (HiddenField)e.Item.FindControl("hfStatus");
					lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);

                    GridView gv = (GridView)e.Item.FindControl("gvPlantList");
                    gv.DataSource = busOrg.PLANT.OrderBy(l=> l.PLANT_NAME).ToList();  // order by plant name
                    gv.DataBind();

                    Label divLabel = (Label)e.Item.FindControl("lblPlantListEmpty");
                    HtmlGenericControl div = (HtmlGenericControl)e.Item.FindControl("divPlantGVScroll");
                    SetGridViewDisplay(gv, divLabel, div, 20, gv.Rows.Count, "scrollArea");

					if (UserContext.GetMaxScopePrivilege(SysScope.busloc) <= SysPriv.admin)
                    {
                        if (busOrg.PLANT.Count > 0)
                        {
                            Button btnAddPlant = (Button)gv.HeaderRow.FindControl("btnAddPlant");
                            btnAddPlant.CommandArgument = busOrg.BUS_ORG_ID.ToString();
                            btnAddPlant.Visible = true;
                        }
                        else
                        {
                            Button btnAddPlant = (Button)e.Item.FindControl("btnAddPlantEmpty");
                            btnAddPlant.CommandArgument = busOrg.BUS_ORG_ID.ToString();
                            btnAddPlant.Visible = true;
                        }
                    }
				}
				catch
				{
				}
			}
		}

		public void rptPlantGrid_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				System.Web.UI.WebControls.Label lbl = new Label();
				System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

				try
				{
					lbl = (Label)e.Item.FindControl("lblLocationType_out");
                    hfField = (HiddenField)e.Item.FindControl("hdnLocationType");
                    lbl.Text = WebSiteCommon.GetXlatValue("locationType", hfField.Value);
				}
				catch
				{
				}
			}
		}

        #endregion  

        #region plant
 
        public void gvPlantList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblLocationType_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfLocationType");
                    lbl.Text = WebSiteCommon.GetXlatValue("locationType", hfField.Value);

                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);

                    SETTINGS sets = SQMSettings.GetSetting("COMPANY", "PLANTUSER");
                    if (sets == null || sets.VALUE.ToUpper() != "N")
                    {
                        LinkButton lnk = (LinkButton)e.Row.Cells[0].FindControl("lnkView_out");
                        decimal plantID = Convert.ToDecimal(lnk.CommandArgument);
                        HiddenField hf = (HiddenField)e.Row.Cells[0].FindControl("hfPlantCompanyID");
                        lnk.ToolTip = "";
                        foreach (PERSON person in UserList())
                        {
                            if (person.ROLE > 1  &&  SQMModelMgr.PersonPlantAccess(person, plantID))
                                lnk.ToolTip += (" " + person.FIRST_NAME + " " + person.LAST_NAME + ",");
                        }
                        lnk.ToolTip = lnk.ToolTip.TrimEnd(',');
                    }
                }
                catch
                {
                }
            }
        }
        #endregion

        #region dept
        public Panel DeptListPanel
        {
            get { return pnlDeptList; }
        }
        public GridView DeptListGrid
        {
            get { return gvDeptList; }
        }

        public void BindDeptList(List<DEPARTMENT> theList)
        {
            ToggleVisible(pnlDeptList);
            gvDeptList.DataSource = theList;
            gvDeptList.DataBind();
            SetGridViewDisplay(gvDeptList, lblDeptListEmpty, divDeptGVScroll, 20, 0, "scrollArea");
        }
        protected void lnkDeptList_Click(object sender, EventArgs e)
        {
            if (OnDeptListClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnDeptListClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }
        protected void btnDeptAdd_Click(object sender, EventArgs e)
        {
            if (OnAddDeptClick != null)
            {
                OnAddDeptClick(0);
            }
        }
        #endregion

        #region labor
        public Panel LaborListPanel
        {
            get { return pnlLaborList; }
        }
        public GridView LaborListGrid
        {
            get { return gvLaborList; }
        }

        public void BindLaborList(List<LABOR_TYPE> theList)
        {
            ToggleVisible(pnlLaborList);
            gvLaborList.DataSource = theList;
            gvLaborList.DataBind();
            SetGridViewDisplay(gvLaborList, lblLaborListEmpty, divLaborGVScroll, 20, 0, "scrollArea");
        }
        protected void lnkLaborList_Click(object sender, EventArgs e)
        {
            if (OnLaborListClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnLaborListClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }
        protected void btnLaborAdd_Click(object sender, EventArgs e)
        {
            if (OnAddLaborClick != null)
            {
                OnAddLaborClick(0);
            }
        }
        #endregion

        #region plantline
        public Panel LineListPanel
        {
            get { return pnlLineList; }
        }
        public GridView LineListGrid
        {
            get { return gvLineList; }
        }

        public void BindLineList(List<PLANT_LINE> lineList)
        {
            ToggleVisible(pnlLineList);
            gvLineList.DataSource = lineList;
            gvLineList.DataBind();
            SetGridViewDisplay(gvLineList, lblLineListEmpty, divLineGVScroll, 20, 0, "scrollArea");
        }
        protected void lnkLineList_Click(object sender, EventArgs e)
        {
            if (OnLineListClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnLineListClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
            }
        }
        protected void btnLineAdd_Click(object sender, EventArgs e)
        {
            if (OnAddLineClick != null)
            {
                OnAddLineClick(0);
            }
        }
        #endregion

        #region users
        public int BindUserList(List<PERSON> userList, decimal companyID)
        {
            int status = 0;

            if (companyID > 0)
            {
                SETTINGS sets = SQMSettings.GetSetting("COMPANY", "PLANTUSER");
                if (sets == null || sets.VALUE.ToUpper() != "N")
                    SetPlantList(SQMModelMgr.SelectPlantList(new PSsqmEntities(), companyID, 0));
            }
            else
            {
                SetPlantList(new List<PLANT>());
            }

            ToggleVisible(pnlUserList);
            rgUserList.DataSource = userList;
            rgUserList.DataBind();
            if (userList.Count == 0)
                lblUserListEmpty.Visible = true;
			else
				lblUserListEmpty.Visible = false;

            return status;
        }

        protected void rgUserList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    PERSON user = (PERSON)e.Item.DataItem;

                    if (user.STATUS == "I")
                    {
                        Image img = (Image)e.Item.FindControl("imgStatus");
                        img.ImageUrl = "/images/defaulticon/16x16/no.png";
                        img.Visible = true;
                    }

                    SETTINGS sets = SQMSettings.GetSetting("COMPANY", "PLANTUSER");
                    if (sets == null || sets.VALUE.ToUpper() != "N")
                    {
                        Label lbl = (Label)e.Item.FindControl("lblUserStatus");
                        lbl.Text = WebSiteCommon.GetXlatValue("statusCodeDelete", lbl.Text);

                        lbl = (Label)e.Item.FindControl("lblJobCode");
						if (user.JOBCODE != null)
						{
							lbl.Text = (user.JOBCODE_CD + " ("+user.JOBCODE.JOB_DESC + ")");
						}
						lbl = (Label)e.Item.FindControl("lblUserRole");
						if (user.PRIVGROUP != null)
						{
							lbl.Text = user.PRIVGROUP.DESCRIPTION;
						}
                        //lbl.Text = WebSiteCommon.GetXlatValue("userRole", user.ROLE.ToString());

                        LinkButton lnk = (LinkButton)e.Item.FindControl("lnkHRLocation");
                        lnk.ToolTip = "";
                        PLANT plant = null;

                        if ((plant = PlantList().Where(l => l.PLANT_ID == user.PLANT_ID).FirstOrDefault()) != null)
                        {
                            lnk.Text = plant.PLANT_NAME;
                        }

                        if (user.ROLE == 1 || user.ROLE == 100)
                        {
                            if ((plant = PlantList().Where(l => l.PLANT_ID == user.PLANT_ID).FirstOrDefault()) != null)
                            {
                                lnk.ToolTip = plant.PLANT_NAME + " ...";
                            }
                        }
                        else
                        {
                            string[] args = (user.PLANT_ID.ToString() + user.NEW_LOCATION_CD).Split(',');
                            foreach (string loc in args.Distinct().ToArray())
                            {
                                if ((plant = PlantList().Where(l => l.PLANT_ID.ToString() == loc).FirstOrDefault()) != null)
                                {
                                    lnk.ToolTip += (" " + plant.PLANT_NAME + ",");
                                }
                            }
                        }
                        lnk.ToolTip = lnk.ToolTip.TrimEnd(',');
                    }
                }
                catch
                {
                    ;
                }
            }
        }

        protected void rgUserList_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            SessionManager.ReturnStatus = true;
            SessionManager.ReturnObject = "DisplayUsers";
        }
		protected void rgUserList_PageIndexChanged(object sender, GridPageChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayUsers";
		}
		protected void rgUserList_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayUsers";
		}

        protected void lnkUserView_Click(object sender, EventArgs e)
        {
            if (OnUserListClick != null)
            {
                try
                {
                    LinkButton lnk = (LinkButton)sender;
                    OnUserListClick(Convert.ToDecimal(lnk.CommandArgument.ToString().Trim()));
                }
                catch { }
            }

        }

        #endregion

        #region common
       
        public void gvList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.Label lbl = new Label();
                System.Web.UI.WebControls.HiddenField hfField = new HiddenField();

                try
                {
                    lbl = (Label)e.Row.Cells[0].FindControl("lblStatus_out");
                    hfField = (HiddenField)e.Row.Cells[0].FindControl("hfStatus_out");
                    lbl.Text = WebSiteCommon.GetStatusString(hfField.Value);
                }
                catch
                {
                }
            }
        }

        public void SetGridViewDisplay(GridView gv, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount, string cssScroll)
        {
            if (gv.Rows.Count == 0)
            {
                gv.Visible = false;
                lblAlert.Visible = true;
            }
            else
            {
                gv.Visible = true;
                lblAlert.Visible = false;
                int gridRows = gridRowCount;
                if (gridRows == 0)
                    gridRows = gv.Rows.Count;
                int rowLimit = rowsToScroll;
                if (rowLimit == 0)
                    rowLimit = 12; // dfltRowsToScroll;
                if (gridRows > rowLimit && divScroll != null)
                {
                    divScroll.Attributes["class"] = cssScroll;
                }
            }
        }

		public void SetRepeaterDisplay(Repeater rpt, Label lblAlert, System.Web.UI.HtmlControls.HtmlGenericControl divScroll, int rowsToScroll, int gridRowCount, string className)
		{
			if (rpt.Items.Count == 0)
			{
				rpt.Visible = false;
				lblAlert.Visible = true;
			}
			else
			{
				rpt.Visible = true;
				lblAlert.Visible = false;
				int gridRows = gridRowCount;
				if (gridRows == 0)
					gridRows = rpt.Items.Count;
				int rowLimit = rowsToScroll;
				if (rowLimit == 0)
					rowLimit = 12; // dfltRowsToScroll;
				if (gridRows > rowLimit && divScroll != null)
				{
					divScroll.Attributes["class"] = className;
				}
			}
		}
		#endregion


    }
}
