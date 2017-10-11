using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace SQM.Website.Include
{
    public partial class Ucl_AddApprover : System.Web.UI.UserControl
    {
        public event GridActionCommand OnApproverActionCommand;
        public event GridItemClick OnApproverActionClick;

        private List<XLAT> XLATList
        {
            get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
            set { ViewState["XLATList"] = value; }
        }

        #region Approveraction

        public void BindApproverListA(List<INCFORMAPPROVERLIST> ApproverItemList, BusinessLocation businessLocation, string context)
        {
            hfApproverActionContext.Value = context;
            if (context != "company")
                hfApproverActionBusLoc.Value = context == "plant" ? businessLocation.Plant.PLANT_ID.ToString() : businessLocation.BusinessOrg.BUS_ORG_ID.ToString();


            pnlApproverAction.Visible = true;
            rgApproverAction.DataSource = ApproverItemList;
            rgApproverAction.DataBind();
        }
        public void BindApproverListR(List<INCFORMAPPROVERLIST> ApproverItemList, BusinessLocation businessLocation, string context)
        {
            hfApproverActionContext.Value = context;
            if (context != "company")
                hfApproverActionBusLoc.Value = context == "plant" ? businessLocation.Plant.PLANT_ID.ToString() : businessLocation.BusinessOrg.BUS_ORG_ID.ToString();


            pnlApproverAction.Visible = true;
            rgRegionalApproverAction.DataSource = ApproverItemList;
            rgRegionalApproverAction.DataBind();
        }
        private void FillApproverList(DropDownList ddl)
        {
            List<PERSON> personList = null;
            List<PERSON> personListGlobalSafety = null;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                if (SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID > 0)
                    personList = (from P in entities.PERSON
                                  where (P.BUS_ORG_ID == SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID && P.ROLE > 1)
                                  select P).ToList();

                personList = personList.Where(l => l.STATUS == "A").ToList();
                List<decimal?> pid = new List<decimal?>();
                foreach (PERSON person in personList)
                {
                    pid.Add(person.PERSON_ID);
                }
                personListGlobalSafety = (from P in entities.PERSON
                                          where (P.PRIV_GROUP.ToUpper() == "GLOBAL SAFETY GROUP" && P.ROLE > 1)
                                          select P).ToList();

                personListGlobalSafety = personListGlobalSafety.Where(l => l.STATUS == "A").ToList();
                personListGlobalSafety = personListGlobalSafety.Except(personList).ToList();
            }



            if (personList.Count > 0)
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("Please select an approver from list.", "0"));
                foreach (PERSON person in personList)
                {
                    ddl.Items.Add(new ListItem(SQMModelMgr.FormatPersonListItemWithEmail(person, false, "LF"), person.PERSON_ID.ToString()));
                }
                foreach (PERSON person in personListGlobalSafety)
                {
                    ddl.Items.Add(new ListItem(SQMModelMgr.FormatPersonListItemWithEmail(person, false, "LF"), person.PERSON_ID.ToString()));
                }
            }
        }
        protected void rgApproverAction_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    INCFORMAPPROVERLIST ApproverAction = (INCFORMAPPROVERLIST)e.Item.DataItem;

                    Label lbl;

                    HiddenField hf = (HiddenField)item.FindControl("hfApproverItemID");
                    HiddenField hfApproverPerson_Id = (HiddenField)item.FindControl("hfApproverPerson_Id");
                    HiddenField hfApproverType = (HiddenField)item.FindControl("hfApproverType");
                    hf.Value = ApproverAction.INCFORM_APPROVER_LIST_ID.ToString();
                    DropDownList ddlApproverList = (DropDownList)item.FindControl("ddlApproverList");
                    FillApproverList(ddlApproverList);
                    ddlApproverList.SelectedValue = ApproverAction.PERSON_ID.ToString();
                    hfApproverPerson_Id.Value = ApproverAction.PERSON_ID.ToString();
                    hfApproverType.Value = ApproverAction.TYPE;
                    //LinkButton lnk = (LinkButton)item.FindControl("lnkApproverItem");
                    //lnk.Text = "";// ApproverAction.DESCRIPTION_SHORT.ToString();// XLATList.Where(x => x.XLAT_GROUP == "Approver_SCOPE" && x.XLAT_CODE == ApproverAction.Approver_SCOPE).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeTask");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString();//XLATList.Where(x => x.XLAT_GROUP == "Approver_SCOPE_TASK" && x.XLAT_CODE == ApproverAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeStatus");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "Approver_TASK_STATUS" && x.XLAT_CODE == ApproverAction.TASK_STATUS).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblApproverTiming");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "Approver_TIMING" && x.XLAT_CODE == ApproverAction.Approver_TIMING.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblApproverDist");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //ApproverAction.Approver_DIST;
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected void rgApproverAction_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("sort");
            }
        }
        protected void rgApproverAction_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("index");
            }
        }
        protected void rgApproverAction_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("size");
            }
        }

        protected void lnklApproverItem_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            GridDataItem row = (GridDataItem)lnk.NamingContainer;
            HiddenField hfApproverType = (HiddenField)row.FindControl("hfApproverType");

            try
            {
                INCFORMAPPROVERLIST ApproverAction = SQMModelMgr.LookupINCFORMAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(lnk.CommandArgument), hfApproverType.Value);
                if (ApproverAction != null)
                {
                    hfApproverActionID.Value = ApproverAction.INCFORM_APPROVER_LIST_ID.ToString();
                    //ddlApprover.SelectedValue = ApproverAction.PERSON_ID.ToString();
                    ddlApproverType.SelectedValue = ApproverAction.TYPE;
                    txtDescription.Text = ApproverAction.DESCRIPTION;
                    //ddlPriv.SelectedValue = ApproverAction.PRIV.ToString();
                    //ddlStep.SelectedValue = ApproverAction.STEP.ToString();
                    //txtDescriptionQuestion.Text = ApproverAction.DESCRIPTION_QUESTION;
                    btnDelete.Visible = true;

                }

                string script = "function f(){OpenApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            catch (Exception ex)
            {
            }
        }
        protected void lnklRegionalApproverItem_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;

            try
            {
                INCFORMAPPROVERLIST ApproverAction = SQMModelMgr.LookupINCFORMAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(lnk.CommandArgument), "R");
                if (ApproverAction != null)
                {
                    hfApproverActionID.Value = ApproverAction.INCFORM_APPROVER_LIST_ID.ToString();
                    //ddlApprover.SelectedValue = ApproverAction.PERSON_ID.ToString();
                    ddlApproverType.SelectedValue = ApproverAction.TYPE;
                    txtDescription.Text = ApproverAction.DESCRIPTION;
                    //ddlPriv.SelectedValue = ApproverAction.PRIV.ToString();
                    //ddlStep.SelectedValue = ApproverAction.STEP.ToString();
                    //txtDescriptionQuestion.Text = ApproverAction.DESCRIPTION_QUESTION;
                    btnDelete.Visible = true;

                }

                string script = "function f(){OpenApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            catch (Exception ex)
            {
            }
        }
        protected void ddlApproverList_Click(object sender, EventArgs e)
        {
            DropDownList ddlApproverList = (DropDownList)sender;
            GridDataItem row = (GridDataItem)ddlApproverList.NamingContainer;
            HiddenField hfApproverItemID = (HiddenField)row.FindControl("hfApproverItemID");
            HiddenField hfApproverPerson_Id = (HiddenField)row.FindControl("hfApproverPerson_Id");
            string perid = ddlApproverList.SelectedValue;
            if (ddlApproverList.SelectedValue == "0")
            {
                ddlApproverList.Attributes.Add("class", "CSSHI");
                string script = "function ferrorMsg(){alert('You cannot select invalid option from approver list.'); Sys.Application.remove_load(ferrorMsg);}Sys.Application.add_load(ferrorMsg);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                ddlApproverList.SelectedValue = hfApproverPerson_Id.Value;
            }
            else
            {
                ddlApproverList.Attributes.Add("class", "CSSRHI");
                PSsqmEntities ctx = new PSsqmEntities();
                INCFORM_APPROVER_LIST ApproverAction = null;
                ApproverAction = SQMModelMgr.SelectINCFORMAPPROVERSLIST(ctx, Convert.ToDecimal(hfApproverItemID.Value));
                decimal personid = Convert.ToDecimal(ddlApproverList.SelectedValue);


                ApproverAction.PERSON_ID = personid;

                ApproverAction.SSO_ID = (from P in ctx.PERSON
                                         where (P.PERSON_ID == personid)
                                         select P.SSO_ID).FirstOrDefault();
                if ((ApproverAction = SQMModelMgr.UpdateApproverAction(ctx, ApproverAction, true)) != null)
                {

                    //ddlApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    txtDescription.Text = "";
                    //txtDescriptionQuestion.Text = "";
                    rgApproverAction.DataSource = null;
                    rgApproverAction.DataBind();
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                    BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");
                    //INCFORM_APPROVER_LIST appList = (from i in ctx.INCFORM_APPROVER_LIST where i.PLANT_ID == ApproverAction.PLANT_ID && i.TYPE == "N" select i).FirstOrDefault();
                    //EHSNotificationMgr.NotifyAddApprover(appList.PERSON_ID, ApproverAction.PERSON_ID, "Add/Update Approver");
                }
            }
            //string _text1 = txtECustCode.Text.ToString();

            //try
            //{
            //    INCFORMAPPROVERLIST ApproverAction = SQMModelMgr.LookupINCFORMAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(lnk.));
            //    if (ApproverAction != null)
            //    {
            //        hfApproverActionID.Value = ApproverAction.INCFORM_APPROVER_LIST_ID.ToString();
            //        //ddlApprover.SelectedValue = ApproverAction.PERSON_ID.ToString();
            //        ddlApproverType.SelectedValue = ApproverAction.TYPE;
            //        txtDescription.Text = ApproverAction.DESCRIPTION;
            //        //ddlPriv.SelectedValue = ApproverAction.PRIV.ToString();
            //        //ddlStep.SelectedValue = ApproverAction.STEP.ToString();
            //        //txtDescriptionQuestion.Text = ApproverAction.DESCRIPTION_QUESTION;
            //        btnDelete.Visible = true;

            //    }

            //    string script = "function f(){OpenApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            //}
            //catch
            //{
            //}
        }



        private void SaveApproverItem()
        {
            PSsqmEntities ctx = new PSsqmEntities();
            INCFORM_APPROVER_LIST ApproverAction = null;
            bool isNew = false;
            //decimal personid = Convert.ToDecimal(ddlApprover.SelectedValue);
            decimal step = Convert.ToDecimal("5.50");// Convert.ToDecimal(ddlStep.SelectedValue);
            decimal stepf = Convert.ToDecimal("2.50");
            //int priv = Convert.ToInt32(ddlPriv.SelectedValue);
            if (string.IsNullOrEmpty(hfApproverActionID.Value))  // add new item
            {
                ApproverAction = new INCFORM_APPROVER_LIST();
                ApproverAction.COMPANY_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.COMPANY_ID.ToString());//person.COMPANY_ID;//
                ApproverAction.BUS_ORG_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID.ToString());//person.BUS_ORG_ID;//
                ApproverAction.PLANT_ID = Convert.ToDecimal(SessionManager.EffLocation.Plant.PLANT_ID.ToString());
                ApproverAction.CREATED_DATE = System.DateTime.Now;
                isNew = true;
            }
            else
            {
                ApproverAction = SQMModelMgr.SelectINCFORMAPPROVERSLIST(ctx, Convert.ToDecimal(hfApproverActionID.Value));
            }

            //ApproverAction.PERSON_ID = personid;

            //ApproverAction.SSO_ID = (from P in ctx.PERSON
            //                         where (P.PERSON_ID == personid)
            //                         select P.SSO_ID).FirstOrDefault();
            ApproverAction.DESCRIPTION = txtDescription.Text;
            ApproverAction.DESCRIPTION_QUESTION = "I approve this report for distribution";// txtDescriptionQuestion.Text;
            ApproverAction.STEP = step;
            ApproverAction.STEPFLASH = stepf;
            ApproverAction.PRIV = 391;// priv;
            ApproverAction.TYPE = ddlApproverType.SelectedValue;// = ApproverAction.TYPE;
            ApproverAction.REQUIRED_COMPLETE = true;
            if ((ApproverAction = SQMModelMgr.UpdateApproverAction(ctx, ApproverAction, false)) != null)
            {
                if (isNew)
                {
                    if (OnApproverActionCommand != null)
                    {
                        OnApproverActionCommand("add");
                    }
                    // ddlApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    ddlApproverType.SelectedValue = "0";
                    txtDescription.Text = "";
                    // txtDescriptionQuestion.Text = "";
                    rgApproverAction.DataSource = null;
                    rgApproverAction.DataBind();
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                    BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");
                }
                else
                {
                    //ddlApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    ddlApproverType.SelectedValue = "0";
                    txtDescription.Text = "";
                    //txtDescriptionQuestion.Text = "";
                    rgApproverAction.DataSource = null;
                    rgApproverAction.DataBind();
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                    BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");
                }
            }
        }

        public void SaveFirstApproverItem()
        {
            PSsqmEntities ctx = new PSsqmEntities();
            INCFORM_APPROVER_LIST ApproverAction = null;
            decimal step = Convert.ToDecimal("5.50");

            ApproverAction = new INCFORM_APPROVER_LIST();
            ApproverAction.COMPANY_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.COMPANY_ID.ToString());//person.COMPANY_ID;//
            ApproverAction.BUS_ORG_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID.ToString());//person.BUS_ORG_ID;//
            ApproverAction.PLANT_ID = Convert.ToDecimal(SessionManager.EffLocation.Plant.PLANT_ID.ToString());
            ApproverAction.CREATED_DATE = System.DateTime.Now;

            //ApproverAction.PERSON_ID = personid;

            //ApproverAction.SSO_ID = (from P in ctx.PERSON
            //                         where (P.PERSON_ID == personid)
            //                         select P.SSO_ID).FirstOrDefault();
            ApproverAction.DESCRIPTION = "Plant Approver Notifications";
            ApproverAction.DESCRIPTION_QUESTION = "I approve this report for distribution";// txtDescriptionQuestion.Text;
            ApproverAction.STEP = step;
            ApproverAction.PRIV = 391;// priv;
            ApproverAction.TYPE = "N";// = ApproverAction.TYPE;
            ApproverAction.REQUIRED_COMPLETE = true;
            if ((ApproverAction = SQMModelMgr.UpdateApproverAction(ctx, ApproverAction, false)) != null)
            {
                if (OnApproverActionCommand != null)
                {
                    OnApproverActionCommand("add");
                }
                // ddlApprover.SelectedValue = "0";
                //ddlStep.SelectedValue = "0";
                //ddlPriv.SelectedValue = "0";
                txtDescription.Text = "";
                // txtDescriptionQuestion.Text = "";
                rgApproverAction.DataSource = null;
                rgApproverAction.DataBind();
                rgRegionalApproverAction.DataSource = null;
                rgRegionalApproverAction.DataBind();
                BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");

            }
        }

        protected void btnApproverItemAdd_Click(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (GridDataItem item in rgApproverAction.Items)
            {
                DropDownList ddlApproverList = (DropDownList)item.FindControl("ddlApproverList");
                if (ddlApproverList.SelectedValue == "0")
                {
                    flag = true;
                    //ddlApproverList.BackColor = System.Drawing.Color.Gray;
                    ddlApproverList.Attributes.Add("class", "CSSHI");
                    break;
                }
                else
                {
                    //ddlApproverList.BackColor = System.Drawing.Color.Empty;
                    ddlApproverList.Attributes.Add("class", "CSSRHI");
                }
            }
            if (flag)
            {
                string script = "function ferrorMsg(){alert('Please select an option from approver list.'); Sys.Application.remove_load(ferrorMsg);}Sys.Application.add_load(ferrorMsg);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            else
            {
                hfApproverActionID.Value = "";
                btnDelete.Visible = false;
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    int count = (from p in entities.INCFORM_APPROVER_LIST where p.PLANT_ID == SessionManager.EffLocation.Plant.PLANT_ID && p.TYPE == "A" select p).Count();
                    txtDescription.Text = "Approver " + (count + 1).ToString();
                    ddlApproverType.SelectedValue = "A";
                }
                string script = "function f(){OpenApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
        }
        protected void btnAddRegionalApproverAction_Click(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (GridDataItem item in rgRegionalApproverAction.Items)
            {
                DropDownList ddlRegionalApproverList = (DropDownList)item.FindControl("ddlRegionalApproverList");
                if (ddlRegionalApproverList.SelectedValue == "0")
                {
                    flag = true;
                    //ddlRegionalApproverList.BackColor = System.Drawing.Color.Gray;
                    ddlRegionalApproverList.Attributes.Add("class", "CSSHI");
                    break;
                }
                else
                {
                    //ddlRegionalApproverList.BackColor = System.Drawing.Color.Empty;
                    ddlRegionalApproverList.Attributes.Add("class", "CSSRHI");
                }
            }
            if (flag)
            {
                string script = "function ferrorMsg(){alert('Please select an option from approver list.'); Sys.Application.remove_load(ferrorMsg);}Sys.Application.add_load(ferrorMsg);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            else
            {
                hfApproverActionID.Value = "";
                btnDelete.Visible = false;
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    int count = (from p in entities.INCFORM_APPROVER_LIST where p.PLANT_ID == SessionManager.EffLocation.Plant.PLANT_ID && p.TYPE == "R" select p).Count();
                    txtDescription.Text = "Regional Approver " + (count + 1).ToString();
                    ddlApproverType.SelectedValue = "R";
                }

                string script = "function f(){OpenApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
        }

        protected void OnCancelApproverAction_Click(object sender, EventArgs e)
        {
            hfApproverActionID.Value = "";
        }

        protected void OnSaveApproverAction_Click(object sender, EventArgs e)
        {

            SaveApproverItem();
        }

        protected void OnDeleteApproverAction_Click(object sender, EventArgs e)
        {
            if (ddlApproverType.SelectedValue == "N")
            {
                string script = "function ferrorMsg(){alert('You cannot delete this approver. This is default approver.'); Sys.Application.remove_load(ferrorMsg);}Sys.Application.add_load(ferrorMsg);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            else
            {
                if (!string.IsNullOrEmpty(hfApproverActionID.Value))  // delete if an existing record
                {
                    SQMModelMgr.DeleteINCFORMAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(hfApproverActionID.Value));
                    hfApproverActionID.Value = "";
                    if (OnApproverActionCommand != null)
                    {
                        OnApproverActionCommand("delete");
                    }
                    BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(new PSsqmEntities(), SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");
                    BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(new PSsqmEntities(), SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                }
            }

        }

        protected void rgRegionalApproverAction_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    INCFORMAPPROVERLIST ApproverAction = (INCFORMAPPROVERLIST)e.Item.DataItem;

                    Label lbl;

                    HiddenField hf = (HiddenField)item.FindControl("hfApproverItemID");
                    HiddenField hfApproverPerson_Id = (HiddenField)item.FindControl("hfApproverPerson_Id");
                    HiddenField hfApproverType = (HiddenField)item.FindControl("hfApproverType");
                    hf.Value = ApproverAction.INCFORM_APPROVER_LIST_ID.ToString();
                    DropDownList ddlRegionalApproverList = (DropDownList)item.FindControl("ddlRegionalApproverList");
                    FillApproverList(ddlRegionalApproverList);
                    ddlRegionalApproverList.SelectedValue = ApproverAction.PERSON_ID.ToString();
                    hfApproverPerson_Id.Value = ApproverAction.PERSON_ID.ToString();
                    hfApproverType.Value = ApproverAction.TYPE;
                    //LinkButton lnk = (LinkButton)item.FindControl("lnkApproverItem");
                    //lnk.Text = "";// ApproverAction.DESCRIPTION_SHORT.ToString();// XLATList.Where(x => x.XLAT_GROUP == "Approver_SCOPE" && x.XLAT_CODE == ApproverAction.Approver_SCOPE).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeTask");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString();//XLATList.Where(x => x.XLAT_GROUP == "Approver_SCOPE_TASK" && x.XLAT_CODE == ApproverAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeStatus");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "Approver_TASK_STATUS" && x.XLAT_CODE == ApproverAction.TASK_STATUS).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblApproverTiming");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "Approver_TIMING" && x.XLAT_CODE == ApproverAction.Approver_TIMING.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblApproverDist");
                    lbl.Text = ApproverAction.DESCRIPTION.ToString(); //ApproverAction.Approver_DIST;
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected void rgRegionalApproverAction_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("sort");
            }
        }
        protected void rgRegionalApproverAction_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("index");
            }
        }
        protected void rgRegionalApproverAction_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            if (OnApproverActionCommand != null)
            {
                OnApproverActionCommand("size");
            }
        }

        protected void ddlRegionalApproverList_Click(object sender, EventArgs e)
        {
            DropDownList ddlRegionalApproverList = (DropDownList)sender;
            GridDataItem row = (GridDataItem)ddlRegionalApproverList.NamingContainer;
            HiddenField hfApproverItemID = (HiddenField)row.FindControl("hfApproverItemID");
            HiddenField hfApproverPerson_Id = (HiddenField)row.FindControl("hfApproverPerson_Id");
            string perid = ddlRegionalApproverList.SelectedValue;
            if (ddlRegionalApproverList.SelectedValue == "0")
            {
                ddlRegionalApproverList.Attributes.Add("class", "CSSHI");
                string script = "function ferrorMsg(){alert('You cannot select invalid option from approver list.'); Sys.Application.remove_load(ferrorMsg);}Sys.Application.add_load(ferrorMsg);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                ddlRegionalApproverList.SelectedValue = hfApproverPerson_Id.Value;
            }
            else
            {
                ddlRegionalApproverList.Attributes.Add("class", "CSSRHI");
                PSsqmEntities ctx = new PSsqmEntities();
                INCFORM_APPROVER_LIST ApproverAction = null;
                ApproverAction = SQMModelMgr.SelectINCFORMAPPROVERSLIST(ctx, Convert.ToDecimal(hfApproverItemID.Value));
                decimal personid = Convert.ToDecimal(ddlRegionalApproverList.SelectedValue);


                ApproverAction.PERSON_ID = personid;

                ApproverAction.SSO_ID = (from P in ctx.PERSON
                                         where (P.PERSON_ID == personid)
                                         select P.SSO_ID).FirstOrDefault();
                if ((ApproverAction = SQMModelMgr.UpdateApproverAction(ctx, ApproverAction, true)) != null)
                {

                    //ddlApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    txtDescription.Text = "";
                    //txtDescriptionQuestion.Text = "";
                    rgApproverAction.DataSource = null;
                    rgApproverAction.DataBind();
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindApproverListR(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "R").ToList(), SessionManager.EffLocation, "busorg");
                    BindApproverListA(SQMModelMgr.SelectINCFORMAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID, "A").ToList(), SessionManager.EffLocation, "busorg");
                }
            }
        }
        #endregion

    }

}