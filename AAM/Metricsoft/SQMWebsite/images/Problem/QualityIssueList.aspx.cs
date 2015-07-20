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
        static B2BPartner searchMgr;
        static List<QI_OCCUR> issueList;

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (searchMgr == null)
                {
                    searchMgr = new B2BPartner().Initialize();
                    searchMgr.CompanyID = SessionManager.SessionContext.ActiveCompany().COMPANY_ID;
                    searchMgr.BusorgID = SessionManager.UserContext.BusinessOrg.BUS_ORG_ID;
                    if (SessionManager.UserContext.Plant != null)
                        searchMgr.PlantID = SessionManager.UserContext.Plant.PLANT_ID;
                }
                SetupPage();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

        }

        private void SetupPage()
        {
            SessionManager.ReturnStatus = false;
            SessionManager.ReturnObject = null;

            ddlPlantWhereUsed.DataSource = SQMModelMgr.SelectPlantList(entities, searchMgr.CompanyID, searchMgr.BusorgID);
            ddlPlantWhereUsed.DataTextField = "PLANT_NAME";
            ddlPlantWhereUsed.DataValueField = "PLANT_ID";
            ddlPlantWhereUsed.DataBind();
            ddlPlantWhereUsed.Items.Insert(0, new ListItem("", "0"));
            ddlPlantWhereUsed.SelectedValue = searchMgr.PlantID.ToString();
            searchMgr.PlantID = Convert.ToInt32(ddlPlantWhereUsed.SelectedValue);

            uclIssueList.BindQualityIssueList(QualityIssue.SelectIncidentList(SessionManager.SessionContext.ActiveCompany().COMPANY_ID, "QI", "", false));
            uclIssueList.QualityIssueListGrid.Columns[0].Visible = false;
            divIssueList.Visible = true;

            uclIssueList.OnQualityIssueClick += lnkIssue_Click;
        }


        protected void lnkIssue_Click(decimal issueID)
        {
            QI_OCCUR qiOccur = new QI_OCCUR();
            qiOccur.QIO_ID = Convert.ToDecimal(issueID);
            SessionManager.ReturnObject = qiOccur;
            SessionManager.ReturnStatus = true;

            // close the selector window ...
            System.Threading.Thread.Sleep(250);
           // string javaScript = "<script language=JavaScript>\n" + "window.close(); window.opener.location.reload(true);\n" + "</script>";
            string javaScript = "<script language=JavaScript>\n" + "window.opener.document.forms[0].submit(); window.close();\n" + "</script>";
            RegisterStartupScript("lnkIssue_Click", javaScript);
        }

        #endregion

    }
}