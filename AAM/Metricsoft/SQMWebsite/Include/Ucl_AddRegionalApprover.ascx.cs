using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace SQM.Website.Include
{
    public partial class Ucl_AddRegionalApprover : System.Web.UI.UserControl
    {
        public event GridActionCommand OnRegionalApproverActionCommand;
        public event GridItemClick OnRegionalApproverActionClick;

        private List<XLAT> XLATList
        {
            get { return ViewState["XLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["XLATList"]; }
            set { ViewState["XLATList"] = value; }
        }

        #region RegionalApproveraction

        public void BindRegionalApproverList(List<INCFORMREGIONALAPPROVERLIST> RegionalApproverItemList, BusinessLocation businessLocation, string context)
        {
            hfRegionalApproverActionContext.Value = context;
            if (context != "company")
                hfRegionalApproverActionBusLoc.Value = context == "plant" ? businessLocation.Plant.PLANT_ID.ToString() : businessLocation.BusinessOrg.BUS_ORG_ID.ToString();

            List<PERSON> personList = null;
            List<PERSON> personListGlobalSafety = null;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                if (businessLocation.BusinessOrg.BUS_ORG_ID > 0)
                    personList = (from P in entities.PERSON
                                  where (P.BUS_ORG_ID == businessLocation.BusinessOrg.BUS_ORG_ID && P.ROLE > 1)
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
                ddlRegionalApprover.Items.Clear();
                ddlRegionalApprover.Items.Add(new ListItem("Please select an regional approver from list.", "0"));
                foreach (PERSON person in personList)
                {
                    ddlRegionalApprover.Items.Add(new ListItem(SQMModelMgr.FormatPersonListItemWithEmail(person, false, "LF"), person.PERSON_ID.ToString()));
                }
                foreach (PERSON person in personListGlobalSafety)
                {
                    ddlRegionalApprover.Items.Add(new ListItem(SQMModelMgr.FormatPersonListItemWithEmail(person, false, "LF"), person.PERSON_ID.ToString()));
                }
            }
            pnlRegionalApproverAction.Visible = true;
            rgRegionalApproverAction.DataSource = RegionalApproverItemList;
            rgRegionalApproverAction.DataBind();
        }

        protected void rgRegionalApproverAction_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    INCFORM_APPROVER_LIST RegionalApproverAction = (INCFORM_APPROVER_LIST)e.Item.DataItem;

                    Label lbl;

                    HiddenField hf = (HiddenField)item.FindControl("hfRegionalApproverItemID");
                    hf.Value = RegionalApproverAction.INCFORM_APPROVER_LIST_ID.ToString();

                    LinkButton lnk = (LinkButton)item.FindControl("lnkRegionalApproverItem");
                    lnk.Text = "";// RegionalApproverAction.DESCRIPTION_SHORT.ToString();// XLATList.Where(x => x.XLAT_GROUP == "RegionalApprover_SCOPE" && x.XLAT_CODE == RegionalApproverAction.RegionalApprover_SCOPE).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeTask");
                    lbl.Text = RegionalApproverAction.DESCRIPTION.ToString();//XLATList.Where(x => x.XLAT_GROUP == "RegionalApprover_SCOPE_TASK" && x.XLAT_CODE == RegionalApproverAction.SCOPE_TASK).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblScopeStatus");
                    lbl.Text = RegionalApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "RegionalApprover_TASK_STATUS" && x.XLAT_CODE == RegionalApproverAction.TASK_STATUS).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblRegionalApproverTiming");
                    lbl.Text = RegionalApproverAction.DESCRIPTION.ToString(); //XLATList.Where(x => x.XLAT_GROUP == "RegionalApprover_TIMING" && x.XLAT_CODE == RegionalApproverAction.RegionalApprover_TIMING.ToString()).FirstOrDefault().DESCRIPTION_SHORT;

                    lbl = (Label)item.FindControl("lblRegionalApproverDist");
                    lbl.Text = RegionalApproverAction.DESCRIPTION.ToString(); //RegionalApproverAction.RegionalApprover_DIST;
                }
                catch
                {
                }
            }
        }

        protected void rgRegionalApproverAction_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            if (OnRegionalApproverActionCommand != null)
            {
                OnRegionalApproverActionCommand("sort");
            }
        }
        protected void rgRegionalApproverAction_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            if (OnRegionalApproverActionCommand != null)
            {
                OnRegionalApproverActionCommand("index");
            }
        }
        protected void rgRegionalApproverAction_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            if (OnRegionalApproverActionCommand != null)
            {
                OnRegionalApproverActionCommand("size");
            }
        }

        protected void lnklRegionalApproverItem_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;

            try
            {
                INCFORMREGIONALAPPROVERLIST RegionalApproverAction = SQMModelMgr.LookupINCFORMREGIONALAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(lnk.CommandArgument));
                if (RegionalApproverAction != null)
                {
                    hfRegionalApproverActionID.Value = RegionalApproverAction.INCFORM_REGIONAL_APPROVER_LIST_ID.ToString();
                    ddlRegionalApprover.SelectedValue = RegionalApproverAction.PERSON_ID.ToString();
                    txtDescription.Text = RegionalApproverAction.DESCRIPTION;
                    //ddlPriv.SelectedValue = RegionalApproverAction.PRIV.ToString();
                    //ddlStep.SelectedValue = RegionalApproverAction.STEP.ToString();
                    //txtDescriptionQuestion.Text = RegionalApproverAction.DESCRIPTION_QUESTION;
                    btnDelete.Visible = true;

                }

                string script = "function f(){OpenRegionalApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
            catch
            {
            }
        }


        private void SaveRegionalApproverItem()
        {
            PSsqmEntities ctx = new PSsqmEntities();
            INCFORM_APPROVER_LIST RegionalApproverAction = null;
            bool isNew = false;
            decimal personid = Convert.ToDecimal(ddlRegionalApprover.SelectedValue);
            decimal step = Convert.ToDecimal("5.50");//ddlStep.SelectedValue);
            //int priv = Convert.ToInt32(ddlPriv.SelectedValue);
            if (string.IsNullOrEmpty(hfRegionalApproverActionID.Value))  // add new item
            {
                //PERSON person = (from p in ctx.PERSON
                //                 where p.PERSON_ID == personid
                //                 select p).FirstOrDefault();
                RegionalApproverAction = new INCFORM_APPROVER_LIST();
                RegionalApproverAction.COMPANY_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.COMPANY_ID.ToString());//person.COMPANY_ID;//
                RegionalApproverAction.BUS_ORG_ID = Convert.ToDecimal(SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID.ToString());//person.BUS_ORG_ID;//
                RegionalApproverAction.PLANT_ID = Convert.ToDecimal(SessionManager.EffLocation.Plant.PLANT_ID.ToString());
                RegionalApproverAction.CREATED_DATE = System.DateTime.Now;

                isNew = true;
            }
            else
            {
                RegionalApproverAction = SQMModelMgr.SelectINCFORMREGIONALAPPROVERSLIST(ctx, Convert.ToDecimal(hfRegionalApproverActionID.Value));
            }

            RegionalApproverAction.PERSON_ID = personid;

            RegionalApproverAction.SSO_ID = (from P in ctx.PERSON
                                             where (P.PERSON_ID == personid)
                                             select P.SSO_ID).FirstOrDefault();
            RegionalApproverAction.DESCRIPTION = txtDescription.Text;
            RegionalApproverAction.DESCRIPTION_QUESTION = "I approve this report for distribution";// txtDescriptionQuestion.Text;
            RegionalApproverAction.STEP = step;
            RegionalApproverAction.PRIV = 391;// priv;
            RegionalApproverAction.TYPE = "R";
            RegionalApproverAction.REQUIRED_COMPLETE = true;
            if ((RegionalApproverAction = SQMModelMgr.UpdateRegionalApproverAction(ctx, RegionalApproverAction)) != null)
            {

                if (isNew)
                {
                    if (OnRegionalApproverActionCommand != null)
                    {
                        OnRegionalApproverActionCommand("add");
                    }
                    ddlRegionalApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    txtDescription.Text = "";
                    //txtDescriptionQuestion.Text = "";
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindRegionalApproverList(SQMModelMgr.SelectINCFORMREGIONALAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID,SessionManager.EffLocation.Plant.PLANT_ID).ToList(), SessionManager.EffLocation, "busorg");
                }
                else
                {
                    ddlRegionalApprover.SelectedValue = "0";
                    //ddlStep.SelectedValue = "0";
                    //ddlPriv.SelectedValue = "0";
                    txtDescription.Text = "";
                    //txtDescriptionQuestion.Text = "";
                    rgRegionalApproverAction.DataSource = null;
                    rgRegionalApproverAction.DataBind();
                    BindRegionalApproverList(SQMModelMgr.SelectINCFORMREGIONALAPPROVERLIST(ctx, SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID, SessionManager.EffLocation.Plant.PLANT_ID).ToList(), SessionManager.EffLocation, "busorg");
                }
            }
        }

        protected void btnRegionalApproverItemAdd_Click(object sender, EventArgs e)
        {
            hfRegionalApproverActionID.Value = "";



            btnDelete.Visible = false;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                int count = (from p in entities.INCFORM_APPROVER_LIST where p.BUS_ORG_ID == SessionManager.EffLocation.BusinessOrg.BUS_ORG_ID && p.PLANT_ID == SessionManager.EffLocation.Plant.PLANT_ID && p.TYPE=="R" select p).Count();
                txtDescription.Text = "Regional Approver " + (count + 1).ToString();
            }

            string script = "function f(){OpenRegionalApproverEditWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

        protected void OnCancelRegionalApproverAction_Click(object sender, EventArgs e)
        {
            hfRegionalApproverActionID.Value = "";
        }

        protected void OnSaveRegionalApproverAction_Click(object sender, EventArgs e)
        {

            SaveRegionalApproverItem();
        }

        //private bool ValidateControl()
        //{
        //    bool val = true;
        //    try
        //    {
        //        if (txtDescription.Text == "")
        //        {
        //            val = false;
        //            throw new Exception("Enter description.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string script = "function f(){alert('" + ex.Message.ToString() + "');";
        //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        //    }
        //    return val;
        //}
        protected void OnDeleteRegionalApproverAction_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfRegionalApproverActionID.Value))  // delete if an existing record
            {
                SQMModelMgr.DeleteINCFORMREGIONALAPPROVERLIST(new PSsqmEntities(), Convert.ToDecimal(hfRegionalApproverActionID.Value));
                hfRegionalApproverActionID.Value = "";
                if (OnRegionalApproverActionCommand != null)
                {
                    OnRegionalApproverActionCommand("delete");
                }
            }
        }
        #endregion

    }

}