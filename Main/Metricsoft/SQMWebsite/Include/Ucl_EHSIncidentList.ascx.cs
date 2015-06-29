using System;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using SQM.Website.Classes;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	public partial class Ucl_EHSIncidentList : System.Web.UI.UserControl
	{

		public bool ShowClosedIncidents { set; get; }

		public event EventHandler IncidentSelected;

		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		protected void OnIncidentSelected(EventArgs e)
		{
			if (IncidentSelected != null)
			{
				IncidentSelected(this, e);
			}
		}
		
		protected void rgIncidents_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (rgIncidents.Enabled)
			//{
			//	var iid = rgIncidents.SelectedValues["INCIDENT_ID"].ToString();
			//	EditIncidentId = Convert.ToDecimal(iid);
			//	IsEditContext = true;
			//}

			OnIncidentSelected(e);
		}

		protected void rgIncidents_SortCommand(object sender, EventArgs e)
		{
			ResetToChooseNew();
		}

		protected void rgIncidents_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			var ehsIncidents = (ShowClosedIncidents) ? EHSIncidentMgr.SelectIncidents(companyId, plantIdList) : EHSIncidentMgr.SelectOpenIncidents(companyId, plantIdList);
			rgIncidents.DataSource = ehsIncidents;
		}

		protected void rgIncidents_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem dataItem = e.Item as GridDataItem;
				ImageButton btn = dataItem["ClosedButtonColumn"].Controls[0] as ImageButton;
				var incident = e.Item.DataItem as INCIDENT;
				btn.Visible = (incident.CLOSE_DATE != null && incident.CLOSE_DATE > DateTime.Parse("01/01/1900 00:00:00"));
			}
		}

		protected void cbShowClosed_CheckedChanged(object sender, EventArgs e)
		{
			rgIncidents.Rebind();
		}
	}
}