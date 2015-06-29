using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_NC : System.Web.UI.UserControl
    {
        static private string problemDescSelect;
        static private PageUseMode pageMode;


        private List<NONCONFORMANCE> nonconfList
        {
            get { return ViewState["nonconfList"] == null ? null : (List<NONCONFORMANCE>)ViewState["nonconfList"]; }
            set { ViewState["nonconfList"] = value; }
        }

        public string ProblemArea
        {
            get { return string.IsNullOrEmpty(ddlProblemArea.SelectedValue) ? "" : ddlProblemArea.SelectedValue; }
        }
        public string ProblemAreaDesc
        {
            get { return string.IsNullOrEmpty(problemDescSelect) ? "" : problemDescSelect ; }
        }
        public string NCCategory
        {
            get { return ddlNCCategory.SelectedValue; }
        }
        public string NCDefect
        {
            get { return ddlNC.SelectedValue; }
        }
        public string NCCount
        {
            get { return tbCount.Text; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        public void Initialize(string problemArea, decimal nonconfID, int ncCount, PageUseMode mode)
        {
            pageMode = mode;
            FillLists(true, false);
            if (!string.IsNullOrEmpty(problemArea))
            {
                //ddlProblemArea.SelectedValue = problemArea;
                SQMBasePage.DisplayControlValue(ddlProblemArea, problemArea, pageMode, "textStd");

                FillLists(false, true);
                if (nonconfID > 0)
                {
                    NONCONFORMANCE nconf = SQMResourcesMgr.LookupNonconf(new PSsqmEntities(), nonconfID, "");
                    if (nconf != null && ddlNCCategory.Items.FindItemByValue(nconf.NONCONF_CATEGORY) != null)
                    {
                       // ddlNCCategory.SelectedValue = nconf.NONCONF_CATEGORY;
                        SQMBasePage.DisplayControlValue(ddlNCCategory, nconf.NONCONF_CATEGORY, pageMode, "textStd");
                        SelectNCCategory(null, null);
                       // ddlNC.SelectedValue = nconf.NONCONF_ID.ToString();
                        SQMBasePage.DisplayControlValue(ddlNC, nconf.NONCONF_ID.ToString(), pageMode, "textStd");
                       // tbCount.Text = ncCount.ToString();
                        SQMBasePage.DisplayControlValue(tbCount, ncCount.ToString(), pageMode, "textStd");
                    }
                }
            }
        }

        private void FillLists(bool bArea, bool bCat)
        {
            string problemSelect = ddlProblemArea.SelectedValue;
            if (ddlProblemArea.Items.Count == 0  ||  bArea)
            {
                ddlProblemArea.Items.Clear();
                ddlProblemArea.Items.AddRange(WebSiteCommon.PopulateRadListItems("problemArea"));
                List<string> catArea = SQMResourcesMgr.SelectNonconfCategoryList("").Select(c => c.PROBLEM_AREA).Distinct().ToList();
                foreach (RadComboBoxItem item in ddlProblemArea.Items)
                {
                    if (catArea.Contains(item.Value) == false)
                        item.Enabled = item.Visible = false;
                }
               // ddlProblemArea.Items.Remove(0);
                ddlProblemArea.Items.Insert(0, new RadComboBoxItem("", ""));
                ddlProblemArea.SelectedValue = problemSelect;
            }
            if (ddlProblemArea.SelectedItem != null)
                problemDescSelect = ddlProblemArea.SelectedItem.Text;

            string category = ddlNCCategory.SelectedValue;
            if (bCat)
            {
                ddlNCCategory.Items.Clear();
                ddlNCCategory.DataSource = SQMResourcesMgr.SelectNonconfCategoryList(ddlProblemArea.SelectedValue);
                ddlNCCategory.DataTextField = "NONCONF_NAME";
                ddlNCCategory.DataValueField = "NONCONF_CD";
                ddlNCCategory.DataBind();
                ddlNCCategory.SelectedValue = category;
            }
        }

        protected void SelectProblemArea(object sender, EventArgs e)
        {
            SelectNCCategory(ddlNCCategory, null);
        }

        protected void SelectNCCategory(object sender, EventArgs e)
        {
            if (sender != null)
                FillLists(true, true);

            nonconfList = SQMResourcesMgr.SelectNonconfList(ddlProblemArea.SelectedValue, true).Where(l => l.NONCONF_CATEGORY == ddlNCCategory.SelectedValue).OrderBy(l => l.NONCONF_NAME).ToList();
            ddlNC.Items.Clear();
            foreach (NONCONFORMANCE nc in nonconfList)
            {
                ddlNC.Items.Add(new RadComboBoxItem(nc.NONCONF_NAME, nc.NONCONF_ID.ToString()));
            }
            ddlNC.Items.Insert(0, new RadComboBoxItem("", ""));

            ddlNC.SelectedIndex = 0;
        }

    }
}