using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class Ucl_QISearch : System.Web.UI.UserControl
	{

        public event EditItemClick OnSearchItemSelect;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclIssueSearch.OnSearchClick += SearchList;
            uclIssueList.OnQualityIssueClick += OnIssue_Click;
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
                uclIssueSearch.BindCSTIssueSearch(true, "CST", new PSsqmEntities());
                List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
                SQMBasePage.SetLocationList(uclIssueSearch.DDLPlantSelect, UserContext.FilterPlantAccessList(locationList), -1);
			}
           
		}

        public void Load(bool visible)
        {
            pnlQISearch.Visible = visible;
        }

        private void SearchList(string cmd)
        {
            uclIssueList.Visible = true;
            string context = "CST";

            QSCalcsCtl calcsCtl = new QSCalcsCtl().CreateNew();
            uclIssueList.BindCSTIssueList(calcsCtl.SetIncidentHistory(QualityIssue.SelectIncidentDataList(uclIssueSearch.DDLPlantSelectIDS(), uclIssueSearch.FromDate, uclIssueSearch.ToDate, context, "", false, uclIssueSearch.ShowImages)), context, uclIssueSearch.ShowImages);
        }

        protected void OnIssue_Click(decimal issueID)
        {
            if (OnSearchItemSelect != null)
            {
                OnSearchItemSelect(issueID.ToString());
            }
        }

	}
}