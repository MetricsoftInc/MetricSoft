using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Quality_Issue : SQMBasePage
    {
        // manage current session object  (formerly was page static variable)
        QualityIssueCtl IssueCtl()
        {
            if (SessionManager.CurrentIncident != null && SessionManager.CurrentIncident is QualityIssueCtl)
                return (QualityIssueCtl)SessionManager.CurrentIncident;
            else
                return null;
        }
        QualityIssueCtl SetIssueCtl(QualityIssueCtl issueCtl)
        {
            SessionManager.CurrentIncident = issueCtl;
            return IssueCtl();
        }

        public string appContext
        {
            get { return ViewState["AppContext"] == null ? "" : (string)ViewState["AppContext"]; }
            set { ViewState["AppContext"] = value; }
        }
        public bool isDirected
        {
            get { return ViewState["isDirected"] == null ? false : (bool)ViewState["isDirected"]; }
            set { ViewState["isDirected"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            uclSearchBar1.OnReturnClick += uclSearchBar_OnListClick;
            uclSearchBar1.OnCancelClick += uclSearchBar_OnCancelClick;
            uclSearchBar1.OnNewClick += uclSearchBar_OnNewClick;
            uclSearchBar1.OnEditClick += uclSearchBar_OnEditClick;
            uclSearchBar1.OnSaveClick += uclSearchBar_OnSaveClick;

            uclSearchBar2.OnReturnClick += uclSearchBar_OnListClick;
            uclSearchBar2.OnCancelClick += uclSearchBar_OnCancelClick;
            uclSearchBar2.OnNewClick += uclSearchBar_OnNewClick;
            uclSearchBar2.OnEditClick += uclSearchBar_OnEditClick;
            uclSearchBar2.OnSaveClick += uclSearchBar_OnSaveClick;
            uclIssueList.OnQualityIssueClick += OnIssue_Click;

            uclIssueSearch.OnSearchClick += SearchList;
            uclIssueSearch.OnSearchReceiptsClick += SearchReceipts;

            uclReceiptList.OnReceiptClick += OnIssue_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            decimal issueID = 0;
            if (!Page.IsPostBack)
            {
                //string appContext;
                if (!string.IsNullOrEmpty(Request.QueryString["c"]))
                    appContext = Request.QueryString["c"];
                else
                    appContext = "RCV";

                if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                {
                    try
                    {
                        issueID = Convert.ToDecimal(EncryptionManager.Decrypt(Request.QueryString["i"]));
                    }
                    catch { }
                }

                SetIssueCtl(new QualityIssueCtl().Initialize(null, appContext));
                
                if (SessionManager.EffLocation.BusinessOrg == null)
                    SessionManager.EffLocation = SessionManager.UserContext.WorkingLocation;
                if (uclIssueSearch.DDLPlantSelect.Items.Count == 0)
                {
                    List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
                    SQMBasePage.SetLocationList(uclIssueSearch.DDLPlantSelect, UserContext.FilterPlantAccessList(locationList), -1);
                }

                PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, appContext, appContext, 0);
                if (view != null)
                {
                    ddlChartType.Items.Clear();
                    ddlChartType.Items.Add(new RadComboBoxItem("", ""));
                    foreach (PERSPECTIVE_VIEW_ITEM vi in view.PERSPECTIVE_VIEW_ITEM.Where(l => l.STATUS != "I").OrderBy(l => l.ITEM_SEQ).ToList())
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        item.Text = vi.TITLE;
                        item.Value = vi.ITEM_SEQ.ToString();
                        item.ImageUrl = ViewModel.GetViewItemImageURL(vi);
                        ddlChartType.Items.Add(item);
                    }
                }

                pnlChartSection.Style.Add("display", "none");
                lblChartType.Visible = ddlChartType.Visible = false;

                uclSearchBar_OnListClick();  // display list options upon page entry
            }
            
            switch (IssueCtl().Context)
            {
                case "CST":
                    uclSearchBar1.PageTitle.Text = lblQICSTTitle.Text;
                    uclSearchBar1.NewButton.ToolTip = "Create a new " + lblQICSTTitle.Text;
                    foreach (RadComboBoxItem ci in ddlChartType.Items)
                    {
                        if (ci.Value.Contains("RCV"))
                            ci.Visible = false;
                    }
                    break;
                case "RCV":
                    uclSearchBar1.PageTitle.Text = lblQIRCVTitle.Text;
                    uclSearchBar1.NewButton.ToolTip = "Create a new " + lblQIRCVTitle.Text;
                    foreach (RadComboBoxItem ci in ddlChartType.Items)
                    {
                        if (ci.Value.Contains("CST"))
                            ci.Visible = false;
                    }
                    break;
                default:
                    uclSearchBar1.PageTitle.Text = lblQIPRQTitle.Text;
                    break;
            }

            if (issueID > 0)
                OnIssue_Click(issueID);
            
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
                if (ucl != null)
                {
                    ucl.BindDocumentSelect("SQM", 2, true, true, "");
                }

                if (SessionManager.ReturnStatus == true  &&  SessionManager.ReturnObject is QI_OCCUR)
                {
                    // from inbox
                    isDirected = true;
                    OnIssue_Click(((QI_OCCUR)SessionManager.ReturnObject).INCIDENT_ID);
                    SessionManager.ClearReturns();   
                }
            }
            else
            {
                if (SessionManager.ReturnObject != null)
                {
                    if (SessionManager.ReturnObject == "DisplayIssues")
                    {
                        SearchList("");
                        SessionManager.ClearReturns();
                    }
                    else if (SessionManager.ReturnObject == "DisplayReceipts")
                    {
                        SearchReceipts("");
                        SessionManager.ClearReturns();
                    }
                }
            }
        }

        private void SearchReceipts(string cmd)
        {
            uclIssueList.Visible = ddlChartType.Enabled = false;
            uclReceiptList.Visible = true;
            uclReceiptList.BindReceiptList(SQMModelMgr.SelectReceiptList(entities, uclIssueSearch.FromDate, uclIssueSearch.ToDate, uclIssueSearch.DDLPlantSelectIDS(), new decimal[0] { }, new decimal[0] { }));
        }

        private void SearchList(string cmd)
        {
            uclReceiptList.Visible = false;
            uclIssueList.Visible = ddlChartType.Enabled = true;
          
            IssueCtl().InitializeCalcs();
            uclIssueList.BindCSTIssueList(IssueCtl().CalcsCtl.SetIncidentHistory(QualityIssue.SelectIncidentDataList(uclIssueSearch.DDLPlantSelectIDS(), uclIssueSearch.FromDate, uclIssueSearch.ToDate, IssueCtl().Context, "", false, uclIssueSearch.ShowImages)), IssueCtl().Context, uclIssueSearch.ShowImages);
           
            if (uclIssueList.CSTListCount > 0)
                lblChartType.Visible = ddlChartType.Visible = true;

            ddlChartType.SelectedIndex = 0;
            ddlChartTypeChange(ddlChartType, null);
        }

        private void uclSearchBar_OnListClick()
        {
            uclQualityIssue.ToggleDisplay(false);
            divSearchList.Visible = true;
            uclSearchBar1.TitleItem.Text = "";
            uclIssueList.LinksDisabled = false;
            uclSearchBar1.SetButtonsVisible(false, false, true, false, false, false);
            uclSearchBar1.SetButtonsEnabled(false, false, true, false, false, false);
            uclSearchBar1.NewButton.Text = "New Issue ";
            uclSearchBar2.SetButtonsVisible(false, false, false, false, false, false);
            uclSearchBar2.SetButtonsEnabled(false, false, false, false, false, false);

            uclIssueSearch.BindCSTIssueSearch(true, IssueCtl().Context, entities);
            uclIssueSearch.BTNReceiptSearch.Visible = true;
           
            SearchList("");
        }

        protected void OnIssue_Click(decimal issueID)
        {
            IssueCtl().Load(issueID);
            //IssueCtl().UserPageMode(SessionManager.UserContext.Person, UserContext.CheckAccess("SQM", ""));
           
            divSearchList.Visible = false;
            uclQualityIssue.ToggleDisplay(true);
            uclSearchBar1.TitleItem.Text = IssueCtl().qualityIssue.IssueID;
            if (isDirected)
            {
                uclSearchBar1.SetButtonsVisible(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, false, lblReturnLabel.Text);
                uclSearchBar1.SetButtonsEnabled(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, false);
                uclSearchBar2.SetButtonsVisible(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, false, lblReturnLabel.Text);
                uclSearchBar2.SetButtonsEnabled(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, false);
            }
            else
            {
                uclSearchBar1.SetButtonsVisible(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, true, lblReturnLabel.Text);
                uclSearchBar1.SetButtonsEnabled(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, true);
                uclSearchBar2.SetButtonsVisible(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, true, lblReturnLabel.Text);
                uclSearchBar2.SetButtonsEnabled(false, false, false, IssueCtl().PageMode == PageUseMode.ViewOnly ? false : true, false, true);
            }
            uclQualityIssue.BindIssue();
        }

        private void uclSearchBar_OnNewClick()
        {
            IssueCtl().CreateNew(Session.SessionID, IssueCtl().Context, SessionManager.UserContext.Person.PERSON_ID);
            //IssueCtl().UserPageMode(SessionManager.UserContext.Person, UserContext.CheckAccess("SQM", ""));
           
            uclSearchBar1.TitleItem.Text = "";
            uclSearchBar1.SetButtonsVisible(false, false, false, true, false, true, lblReturnLabel.Text);
            uclSearchBar1.SetButtonsEnabled(false, false, false, true, false, true);
            uclSearchBar2.SetButtonsVisible(false, false, false, true, false, true, lblReturnLabel.Text);
            uclSearchBar2.SetButtonsEnabled(false, false, false, true, false, true);
            divSearchList.Visible = false;
            uclQualityIssue.ToggleDisplay(true);

            uclQualityIssue.NewIssue();
        }

        private void uclSearchBar_OnEditClick()
        {
            uclSearchBar1.SetButtonsVisible(false, false, false, true, false, true, lblReturnLabel.Text);
            uclSearchBar1.SetButtonsEnabled(false, false, false, true, false, true);
            uclSearchBar2.SetButtonsVisible(false, false, false, true, false, true, lblReturnLabel.Text);
            uclSearchBar2.SetButtonsEnabled(false, false, false, true, false, true);
        }

        private void uclSearchBar_OnCancelClick()
        {
            IssueCtl().Clear();
            uclSearchBar1.TitleItem.Text = "";
            uclQualityIssue.CancelIssue();
            if (isDirected  &&  !string.IsNullOrEmpty(SessionManager.ReturnPath))
                Response.Redirect(SessionManager.ReturnPath);
            else 
                uclSearchBar_OnListClick();
        }

        private void uclSearchBar_OnSaveClick()
        {
            IssueCtl().qualityIssue = uclQualityIssue.UpdateIssue(IssueCtl().qualityIssue);
            if (IssueCtl().qualityIssue.Status == QualityIssue.UpdateStatus.Success)
            {
                if (IssueCtl().Update() != null)
                {
                    uclQualityIssue.SaveAttachments(IssueCtl().qualityIssue);
                    if (IssueCtl().qualityIssue.ShouldSendMail())
                    {
                        IssueCtl().qualityIssue.Incident.RESP_TEAMID = IssueCtl().qualityIssue.MailNotify(SQMSettings.SelectSettingByCode(entities, "MAIL", "TASK", "MailURL").VALUE);
                        IssueCtl().Update();
                    }
                    IssueCtl().qualityIssue.ResetIndicators(false);
                    uclQualityIssue.BindIssue();
                    uclSearchBar1.TitleItem.Text = IssueCtl().qualityIssue.IssueID;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alertResult('hfAlertSaveSuccess');", true);
                }
                else
                {
                    ErrorAlert(QualityIssue.UpdateStatus.SaveError);
                }
            }
            else
            {
                ErrorAlert(IssueCtl().qualityIssue.Status);
            }
        }

        private void ErrorAlert(QualityIssue.UpdateStatus updateStatus)
        {
            HiddenField hf = (HiddenField)hfBase.FindControl("hfErr" + updateStatus.ToString());
            if (hf != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + hf.Value + "');", true);
            }
        }

        protected void lnkCloseChart(object sender, EventArgs e)
        {
            lnkChartClose.Visible = lnkPrint.Visible = false;
            ddlChartType.SelectedValue = "";
            ddlChartTypeChange(null, null);
        }

        protected void ddlChartTypeChange(object sender, EventArgs e)
        {
            divChart.ViewStateMode = System.Web.UI.ViewStateMode.Disabled;
            pnlChartSection.Style.Add("display", "none");
            lnkChartClose.Visible = lnkPrint.Visible = false;
            if (ddlChartType.SelectedValue == "")
            {
                ;
            }
            else
            {
                divChart.Controls.Clear();
                PERSPECTIVE_VIEW view = ViewModel.LookupView(entities, IssueCtl().Context, IssueCtl().Context, 0);
                if (view != null)
                {
                    PERSPECTIVE_VIEW_ITEM vi = view.PERSPECTIVE_VIEW_ITEM.Where(i => i.ITEM_SEQ.ToString() == ddlChartType.SelectedValue).FirstOrDefault();
                    if (vi != null)
                    {
                        GaugeDefinition ggCfg = new GaugeDefinition().Initialize().ConfigureControl(vi, null, "", false, !string.IsNullOrEmpty(hfwidth.Value) ? Convert.ToInt32(hfwidth.Value) - 62 : 0, 0);
                        ggCfg.Position = null;
                        IssueCtl().CalcsCtl.SetCalcParams(vi.CALCS_METHOD, vi.CALCS_SCOPE, vi.CALCS_STAT, (int)vi.SERIES_ORDER).MetricSeries(uclIssueSearch.FromDate, uclIssueSearch.ToDate, uclIssueSearch.DDLPlantSelectIDS());
                        uclChart.CreateControl((SQMChartType)vi.CONTROL_TYPE, ggCfg, IssueCtl().CalcsCtl.Results, divChart);
                        pnlChartSection.Style.Add("display", "inline");
                        lnkChartClose.Visible = lnkPrint.Visible = true;
                       // return;
                    }
                }
            }
          
        }
    }
}