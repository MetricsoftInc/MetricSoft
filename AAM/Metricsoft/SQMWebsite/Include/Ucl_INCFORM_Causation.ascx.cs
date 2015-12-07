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
		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
			}
		}
		public decimal EditIncidentId
		{
			get { return ViewState["EditIncidentId"] == null ? 0 : (decimal)ViewState["EditIncidentId"]; }
			set { ViewState["EditIncidentId"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		public int PopulateInitialForm(PSsqmEntities ctx)
		{
			int status = 0;

			INCIDENT incident = EHSIncidentMgr.SelectIncidentById(ctx, EditIncidentId, true);
			BindCausation(incident);

			return status;
		}

		public void BindCausation(INCIDENT incident)
		{
			try
			{
				if (SessionManager.SessionContext != null)
				{
					// do we really need to do this on user controls ???
					String selectedLanguage = SessionManager.SessionContext.Language().NLS_LANGUAGE;
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

					base.FrameworkInitialize();
				}

				pnlCausation.Visible = true;

				if (incident != null)
					lblIncidentDesc.Text = incident.DESCRIPTION;

				if (incident == null  || incident.INCFORM_ROOT5Y == null  ||  incident.INCFORM_ROOT5Y.Count == 0)
				{
					lblNoneRootCause.Visible = true;
					divCausation.Visible = false;
				}
				else
				{
					lblNoneRootCause.Visible = false;
					divCausation.Visible = true;
					rptRootCause.DataSource = incident.INCFORM_ROOT5Y.ToList();
					rptRootCause.DataBind();

					INCFORM_CAUSATION causation = incident.INCFORM_CAUSATION == null || incident.INCFORM_CAUSATION.Count == 0 ? null : incident.INCFORM_CAUSATION.ElementAt(0);

					ddlCausation.Items.Clear();
					ddlCausation.Items.Add(new RadComboBoxItem("", ""));
					foreach (EHSMetaData xlat in EHSMetaDataMgr.SelectMetaDataList("INJURY_CAUSE").ToList())
					{
						ddlCausation.Items.Add(new Telerik.Web.UI.RadComboBoxItem(xlat.TextLong, xlat.Value));
					}

					if (causation != null)
					{
						if (ddlCausation.FindItemByValue(causation.CAUSEATION_CD) != null)
						{
							ddlCausation.SelectedValue = causation.CAUSEATION_CD;
						}
					}
				}
			}
			catch { }
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
					causation.LAST_UPD_BY = SessionManager.UserContext.UserName();
					causation.LAST_UPD_DT = DateTime.UtcNow;
					ctx.AddToINCFORM_CAUSATION(causation);

					status = ctx.SaveChanges();

					EHSIncidentMgr.UpdateIncidentStatus(incidentID, IncidentStepStatus.rootcauseComplete);
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

					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					lb.Text = rootCause.ITEM_SEQ.ToString();
					lb = (Label)e.Item.FindControl("lblRootCause");
					lb.Text = rootCause.ITEM_DESCRIPTION;
				}
				catch { }
			}
		}
	}
}