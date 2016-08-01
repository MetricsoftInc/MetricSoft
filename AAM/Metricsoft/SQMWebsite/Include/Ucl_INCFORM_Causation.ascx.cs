using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;

namespace SQM.Website
{
	public partial class Ucl_INCFORM_Causation : System.Web.UI.UserControl
	{
		protected int currentProblemSeries = 0;
		protected int currentItemSeq = 0;

		public PageUseMode PageMode { get; set; }

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
			}
		}
		public decimal IncidentId
		{
			get { return ViewState["IncidentId"] == null ? 0 : (decimal)ViewState["IncidentId"]; }
			set { ViewState["IncidentId"] = value; }
		}
		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
		}
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public int PopulateInitialForm(PSsqmEntities ctx)
		{
			int status = 0;

			INCIDENT incident = EHSIncidentMgr.SelectIncidentById(ctx, IncidentId, true);
			PLANT plant = SQMModelMgr.LookupPlant(ctx, (decimal)incident.DETECT_PLANT_ID, "");
			if (plant != null)
				IncidentLocationTZ = plant.LOCAL_TIMEZONE;

			BindCausation(incident);

			pnlCausation.Enabled = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(incident, IsEditContext, SysPriv.originate, incident.INCFORM_LAST_STEP_COMPLETED);

			return status;
		}

		public void BindCausation(INCIDENT incident)
		{
			try
			{
				if (SessionManager.SessionContext != null)
				{
					// do we really need to do this on user controls ???
					String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

					base.FrameworkInitialize();
				}

				pnlCausation.Visible = true;
				if (PageMode == PageUseMode.ViewOnly)
				{
					divTitle.Visible = true;
					lblFormTitle.Text = Resources.LocalizedText.Causation;
				}

				if (incident != null)
				{
					lblIncidentDesc.Text = incident.DESCRIPTION;
					IncidentId = incident.INCIDENT_ID;
				}

				if (incident == null  || incident.INCFORM_ROOT5Y == null  ||  incident.INCFORM_ROOT5Y.Count == 0)
				{
					lblNoneRootCause.Visible = true;
					divCausation.Visible = false;
				}
				else
				{
					List<INCFORM_ROOT5Y> rootCauseList = new List<INCFORM_ROOT5Y>();
					rootCauseList = EHSIncidentMgr.FormatRootCauseList(incident, incident.INCFORM_ROOT5Y.ToList());

					lblNoneRootCause.Visible = false;
					divCausation.Visible = true;
					rptRootCause.DataSource = rootCauseList;
					rptRootCause.DataBind();

					INCFORM_CAUSATION causation = incident.INCFORM_CAUSATION == null || incident.INCFORM_CAUSATION.Count == 0 ? null : incident.INCFORM_CAUSATION.ElementAt(0);

					ddlCausation.Items.Clear();
					ddlCausation.Items.Add(new RadComboBoxItem("", ""));
					foreach (EHSMetaData xlat in EHSMetaDataMgr.SelectMetaDataList("INJURY_CAUSE").ToList())
					{
						ddlCausation.Items.Add(new Telerik.Web.UI.RadComboBoxItem(xlat.TextLong, xlat.Value));
					}

					if (SessionManager.GetUserSetting("EHS", "CAUSATION_ADD_FIELDS") != null)
					{
						if (SessionManager.GetUserSetting("EHS", "CAUSATION_ADD_FIELDS").VALUE.Contains("team"))
						{
							divTeam.Visible = true;
						}
					}

					if (causation != null)
					{
						if (ddlCausation.FindItemByValue(causation.CAUSEATION_CD) != null)
						{
							ddlCausation.SelectedValue = causation.CAUSEATION_CD;
						}

						tbTeam.Text = causation.TEAM_LIST;
					}

					btnSave.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(null, true, SysPriv.action, incident.INCFORM_LAST_STEP_COMPLETED);

				}
			}
			catch { }
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (UpdateCausation(IncidentId) >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				PopulateInitialForm(new PSsqmEntities());
			}
		}

		public int UpdateCausation(decimal incidentID)
		{
			int status = 0;

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				status = ctx.ExecuteStoreCommand("DELETE FROM INCFORM_CAUSATION WHERE INCIDENT_ID = " + incidentID.ToString());

				if (!string.IsNullOrEmpty(ddlCausation.SelectedValue))
				{
					INCFORM_CAUSATION causation = new INCFORM_CAUSATION();
					causation.INCIDENT_ID = incidentID;
					causation.CAUSEATION_CD = ddlCausation.SelectedValue;
					causation.TEAM_LIST = tbTeam.Text.Trim();
					causation.LAST_UPD_BY = SessionManager.UserContext.UserName();
					causation.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);
					ctx.AddToINCFORM_CAUSATION(causation);

					status = ctx.SaveChanges();

					EHSIncidentMgr.UpdateIncidentStatus(incidentID, IncidentStepStatus.rootcauseComplete, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
				}
				else
				{

				}

			}

			return status;
		}

		public void rptRootCause_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					INCFORM_ROOT5Y rootCause = (INCFORM_ROOT5Y)e.Item.DataItem;

					Label lbPrompt = (Label)e.Item.FindControl("lbWhyPrompt");
					Label lbSeq = (Label)e.Item.FindControl("lbItemSeq");
					Label lbCause = (Label)e.Item.FindControl("lblRootCause");

					if (rootCause.ITEM_TYPE == 1)
					{
						lbPrompt.Text = Resources.LocalizedText.ProblemStatement;
						lbSeq.Visible = false;
						lbCause.CssClass = "refText";
						System.Web.UI.HtmlControls.HtmlGenericControl div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divPrompt");
						//div.Style.Add("BACKGROUND-COLOR", "#FFFFE0");
						div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divRootCause");
						div.Style.Add("BACKGROUND-COLOR", "#FFFFE0");
						Image img = (Image)e.Item.FindControl("imgProblem");
						img.ImageUrl = "~/images/defaulticon/16x16/alert-alt.png";
						//img.Visible = true;
					}
					else
					{
						if (currentProblemSeries != rootCause.PROBLEM_SERIES)
						{
							currentProblemSeries = rootCause.PROBLEM_SERIES.HasValue ? (int)rootCause.PROBLEM_SERIES : 0;
							currentItemSeq = 0;
						}
						lbSeq.Text = (++currentItemSeq).ToString();
						if (rootCause.IS_ROOTCAUSE.HasValue && (bool)rootCause.IS_ROOTCAUSE == true)
						{
							Label lbIsRoot = (Label)e.Item.FindControl("lblIsRootCause");
							lbIsRoot.Text = Resources.LocalizedText.RootCause;
							Image img = (Image)e.Item.FindControl("imgIsRootCause");
							img.Visible = true;
						}
					}

					lbCause.Text = rootCause.ITEM_DESCRIPTION;
				}
				catch { }
			}
		}
	}
}