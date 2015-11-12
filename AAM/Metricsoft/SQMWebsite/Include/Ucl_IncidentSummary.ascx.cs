using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SQM.Shared;

namespace SQM.Website
{
	public partial class Ucl_IncidentSummary : System.Web.UI.UserControl
	{
		public event GridActionCommand OnTopicSelect;

		private List<XLAT> XLATList
		{
			get { return ViewState["IncidentSummaryXLATList"] == null ? new List<XLAT>() : (List<XLAT>)ViewState["IncidentSummaryXLATList"]; }
			set { ViewState["IncidentSummaryXLATList"] = value; }
		}

		protected void lnkTopic_Click(object sender, EventArgs e)
		{
			if (OnTopicSelect != null)
			{
				LinkButton lnk = (LinkButton)sender;
				OnTopicSelect(lnk.CommandArgument);
			}
		}

		public void BindIncidentSummary(decimal incidentID)
		{
			PSsqmEntities ctx = new PSsqmEntities();

			XLATList = SQMBasePage.SelectXLATList(new string[2] { "INCIDENT_STATUS", "TASK_STATUS"});
			string none = "None entered"; // TODO:  get this from XLAT table
			string more = " ...";

			pnlIncidentSummary.Visible = true;
			INCIDENT incident = EHSIncidentMgr.SelectIncidentById(ctx, incidentID, true);

			if (incident == null)
			{
				; // error
				return;
			}

			lblDetail.Text = incident.DESCRIPTION;

			if (incident.INCFORM_CONTAIN != null && incident.INCFORM_CONTAIN.Count > 0)
			{
				lblContainment.Text = incident.INCFORM_CONTAIN.Count > 1 ? incident.INCFORM_CONTAIN.ElementAt(0).ITEM_DESCRIPTION + more : incident.INCFORM_CONTAIN.ElementAt(0).ITEM_DESCRIPTION;
			}
			else
			{
				lblContainment.Text = none;
			}

			if (incident.INCFORM_ROOT5Y != null && incident.INCFORM_ROOT5Y.Count > 0)
			{
				lblContainment.Text = incident.INCFORM_ROOT5Y.Count > 1 ? incident.INCFORM_ROOT5Y.ElementAt(0).ITEM_DESCRIPTION + more : incident.INCFORM_ROOT5Y.ElementAt(0).ITEM_DESCRIPTION;
			}
			else
			{
				lblContainment.Text = none;
			}

			if (incident.INCFORM_ACTION != null && incident.INCFORM_ACTION.Count > 0)
			{
				lblContainment.Text = incident.INCFORM_ACTION.Count > 1 ? incident.INCFORM_ACTION.ElementAt(0).ITEM_DESCRIPTION + more : incident.INCFORM_ACTION.ElementAt(0).ITEM_DESCRIPTION;
			}
			else
			{
				lblContainment.Text = none;
			}

			if (incident.INCFORM_APPROVAL != null && incident.INCFORM_APPROVAL.Count > 0)
			{
				lblSignoff.Text = incident.INCFORM_APPROVAL.ElementAt(0).APPROVER_PERSON + SQMBasePage.FormatDate((DateTime)incident.INCFORM_APPROVAL.ElementAt(0).APPROVAL_DATE, "d", false);
				if (incident.INCFORM_APPROVAL.Count > 1)
					lblSignoff.Text = incident.INCFORM_APPROVAL.ElementAt(1).APPROVER_PERSON + SQMBasePage.FormatDate((DateTime)incident.INCFORM_APPROVAL.ElementAt(1).APPROVAL_DATE, "d", false);
			}
			else
			{
				lblContainment.Text = XLATList.Where(l => l.XLAT_GROUP == "TASK_STATUS" && l.XLAT_CODE == "1").FirstOrDefault().DESCRIPTION_SHORT;  // pending 
			}
		}
	}
}