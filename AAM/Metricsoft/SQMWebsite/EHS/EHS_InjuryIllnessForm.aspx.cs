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
	public partial class EHS_InjuryIllnessForm : SQMBasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.EHSIncidents;
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				uclIncidentForm.Mode = IncidentMode.Incident;

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
						string context = SessionManager.ReturnContext;
						SessionManager.ClearReturns();
						SessionManager.SetIncidentLocation((decimal)incident.DETECT_PLANT_ID);
						if (incident.INCIDENT_ID > 0)
						{
							// edit existing incident
							if (context == "a")
								uclIncidentForm.BindIncidentAlert(incident.INCIDENT_ID);
							else 
								uclIncidentForm.BindIncident(incident.INCIDENT_ID);
						}
						else
						{
							// create new
							uclIncidentForm.InitNewIncident((decimal)incident.ISSUE_TYPE_ID, (decimal)incident.DETECT_PLANT_ID);
						}

						uclIncidentForm.Visible = true;
					}
				}
				catch
				{
				}
			}
		}
	}
}