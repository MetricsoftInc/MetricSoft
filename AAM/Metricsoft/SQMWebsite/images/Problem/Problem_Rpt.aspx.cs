using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public partial class Problem_Rpt : SQMBasePage
    {
        ProblemCaseCtl CaseCtl()
        {
            if (SessionManager.CurrentProblemCase != null && SessionManager.CurrentProblemCase is ProblemCaseCtl)
                return (ProblemCaseCtl)SessionManager.CurrentProblemCase;
            else
                return null;
        }
        ProblemCaseCtl SetCaseCtl(ProblemCaseCtl caseCtl)
        {
            SessionManager.CurrentProblemCase = caseCtl;
            return CaseCtl();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
  
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string customerLogo = "";
                customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"];
                if (string.IsNullOrEmpty(customerLogo) || customerLogo.Contains("Metric"))
                {
                    imgLogo.ImageUrl = "~/images/company/MetricsoftLogo.png";
                }
                else
                {
                    int pos = customerLogo.IndexOf('.');
                    customerLogo = customerLogo.Substring(0, pos) + "Small." + customerLogo.Substring(pos + 1, customerLogo.Length - pos - 1);
                    imgLogo.ImageUrl = "~/images/company/" + customerLogo;
                }

                IsCurrentPage();
                if (string.IsNullOrEmpty(SessionManager.CurrentSecondaryTab)  &&  CaseCtl() != null)
                    CaseCtl().Clear();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string appContext;
                if (Request.QueryString != null && Request.QueryString.Get("c") != null)
                    appContext = Request.QueryString.Get("c").ToString();
                else
                    appContext = "";

                divWorkArea.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
                // display the problem report per the ID passed
                if ((bool)SessionManager.ReturnStatus)
                {
                    divPageBody.Visible = true;
                    decimal caseID;
                    if (Decimal.TryParse(SessionManager.ReturnObject.ToString(), out caseID))
                    {
                        SetCaseCtl(new ProblemCaseCtl().Initialize(null, appContext));
                        CaseCtl().Load(caseID, appContext);
                        if (CaseCtl().IsClear() == false)
                        {
                            bool bNotify = false;
                            if (CaseCtl().problemCase.ProbCase.PROBCASE_TYPE == "EHS")
                            {
                                lblCasePrefixEHS.Visible = true;
                                lblCasePrefix.Visible = false;
                            }
                            CaseCtl().LoadPersonSelectList(true);
                           
                            lblCaseID.Text = CaseCtl().problemCase.CaseID;
                            CaseCtl().PageMode = PageUseMode.ViewOnly;
                            uclCaseRpt.SetPageMode();
                            uclCaseRpt.BindCase0();
                            uclCaseRpt.BindCase1(bNotify);
                            uclCaseRpt.BindCase2();
                            uclCaseRpt.BindCase3();
                            uclCaseRpt.BindCase4();
                            uclCaseRpt.BindCase5();
                            uclCaseRpt.BindCase6();
                            uclCaseRpt.BindCase7();
                            uclCaseRpt.BindCase8(bNotify);
                            uclCaseRpt.BindIncidentList(CaseCtl().problemCase.IncidentList);
                            uclCaseRpt.BindPartIssueItemList(CaseCtl().problemCase.PartIssueItemList);
                            SQMBasePage.EnableControls(divWorkArea.Controls, false);
                            SQMBasePage.DisplayButtons(divWorkArea.Controls, false);
                        }
                    }
                    // display error message ...
                    SessionManager.ClearReturns();
                }
            }
            else
            {
 
            }
           
        }

        protected void lnkProblemListReturn_Click(object sender, EventArgs e)
        {
            if (CaseCtl().Context == "EHS")
            {
                CaseCtl().Clear();
                Response.Redirect("/EHS/EHS_Incidents.aspx");
            }
            else
            {
                string appContext = CaseCtl().Context;
                CaseCtl().Clear();
                Response.Redirect("/Problem/Problem_Case.aspx?c=" + appContext);
            }
        }
    }

}
