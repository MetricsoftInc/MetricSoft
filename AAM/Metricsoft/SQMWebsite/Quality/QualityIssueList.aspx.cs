using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website
{
    public partial class QualityIssueList : SQMBasePage
    {

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclQISearch.OnSearchItemSelect += OnIssue_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SessionManager.ClearReturns();
                divIssueList.Visible = true;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            SessionManager.ClearReturns();
            CloseWindow();
        }

        protected void OnIssue_Click(string issueID)
        {
            List<INCIDENT> incidentList = new List<INCIDENT>();
            INCIDENT incident = new INCIDENT();
            incident.INCIDENT_ID = Convert.ToDecimal(issueID);
            incidentList.Add(incident);
            SessionManager.ReturnObject = incidentList;
            SessionManager.ReturnStatus = true;
            CloseWindow();
        }

        private void CloseWindow()
        {
            // close the selector window ...
            string javaScript = "<script language=JavaScript>\n" + "window.close();\n" + "</script>";
           
            System.Threading.Thread.Sleep(250);
            if (SessionManager.ReturnStatus)
                javaScript = "<script language=JavaScript>\n" + "window.opener.document.forms[0].submit(); window.close();\n" + "</script>";
            RegisterStartupScript("lnkIssue_Click", javaScript);
        }

        #endregion

    }
}