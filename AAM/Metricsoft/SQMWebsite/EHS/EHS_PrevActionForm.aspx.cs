using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;
using System.Drawing;

namespace SQM.Website
{
	public partial class EHS_PrevActionForm : SQMBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.EHSIncidents;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("EHS", 2, true, true, "");
				}

				try
				{
					if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is INCIDENT)
					{
						INCIDENT incident = SessionManager.ReturnObject as INCIDENT;
						SessionManager.ClearReturns();
						SessionManager.SetIncidentLocation((decimal)incident.DETECT_PLANT_ID);

						if (incident.INCIDENT_ID > 0)
						{
							// edit existing incident
							int step = 0;
							string returnOverride = "";
							if (!string.IsNullOrEmpty(Request.QueryString["s"]))   // from inbox/calendar assume this is a task assignment. direct to corrective actions page
							{
								int.TryParse(Request.QueryString["s"], out step);
								returnOverride = "/Home/Calendar.aspx";
							}
							uclActionForm.BindIncident(incident.INCIDENT_ID, step, returnOverride);
						}
						else
						{
							// create new 
							uclActionForm.InitNewIncident((decimal)incident.ISSUE_TYPE_ID, incident.ISSUE_TYPE, (decimal)incident.DETECT_PLANT_ID);
						}

						uclActionForm.Visible = true;
					}
				}
				catch
				{
				}
			}

		}
	}
}