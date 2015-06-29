using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;

namespace SQM.Website
{
	public partial class EHS_Incidents_Questions : SQMBasePage
    {
		/*
        decimal _companyId;

		protected void Page_Load(object sender, EventArgs e)
		{
			_companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			BuildIncidentTypeDDL();
			if (ViewState["SelectedValue"] != null)
				BuildForm(ViewState["SelectedValue"].ToString());
		}

		#region Form Creation

		protected void BuildIncidentTypeDDL()
		{
			if (ddlIncidentType.Items.Count == 0)
			{
				try
				{
					ddlIncidentType.Items.Add(new ListItem("[Select One]", ""));
					var incidentTypeList = EHSIncidentMgr.SelectIncidentTypeList(_companyId);
					foreach (var t in incidentTypeList)
						ddlIncidentType.Items.Add(new ListItem(t.TITLE, t.INCIDENT_TYPE_ID.ToString()));
				}
				catch (Exception e)
				{
					//SQMLogger.LogException(e);
				}
			}
		}

		protected void BuildForm(string selectedValue)
		{
			rgCurrentQuestions.Skin = "Metro";

			rgCurrentQuestions.DataSource = EHSIncidentMgr.SelectIncidentQuestionList(Convert.ToInt32(selectedValue), _companyId, 0);
			rgCurrentQuestions.DataBind();

			rgAllQuestions.Skin = "Metro";
			rgAllQuestions.DataSource = EHSIncidentMgr.SelectIncidentQuestionList();
			rgAllQuestions.DataBind();
		}

		#endregion

		#region Events

		protected void ddlIncidentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedValue = (sender as DropDownList).SelectedValue;
			if (!string.IsNullOrEmpty(selectedValue))
			{
				ViewState["SelectedValue"] = selectedValue;
				lblResults.Text = selectedValue;
				BuildForm(selectedValue);
				//btnSubmit.Visible = true;
			}
		}

		#endregion
		*/
	}
}
