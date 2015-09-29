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

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				decimal incidentID = 0;
				string key = SQMModelMgr.GetPasswordKey();
				uclIncidentForm.Mode = IncidentMode.Incident;

				try
				{
					string targetIncident = Request.QueryString["i"];  // INCIDENT ID will be encrypted
					string newIncidentLocation = Request.QueryString["l"];
					string newIncidentType = Request.QueryString["t"];
					if (!string.IsNullOrEmpty(newIncidentLocation) && !string.IsNullOrEmpty(newIncidentType))
					{
						// edit existing incident
						newIncidentType = WebSiteCommon.Decrypt(newIncidentType, key);
						newIncidentLocation = WebSiteCommon.Decrypt(newIncidentLocation, key);
						uclIncidentForm.InitNewIncident(Convert.ToDecimal(newIncidentType), Convert.ToDecimal(newIncidentLocation));
					}
					else if (!string.IsNullOrEmpty(targetIncident))
					{
						// open existing incident
						targetIncident = WebSiteCommon.Decrypt(targetIncident, key);
						uclIncidentForm.BindIncident(Convert.ToDecimal(targetIncident));
					}
					else
					{
						; // display error
					}
				}
				catch
				{
				}

				uclIncidentForm.Visible = true;
			}

		}
	}
}