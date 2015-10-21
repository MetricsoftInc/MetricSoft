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
	public partial class EHS_Audits_Questions : SQMBasePage
    {
		/*
        decimal _companyId;

		protected void Page_Load(object sender, EventArgs e)
		{
			_companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			BuildAuditTypeDDL();
			if (ViewState["SelectedValue"] != null)
				BuildForm(ViewState["SelectedValue"].ToString());
		}

		#region Form Creation

		protected void BuildAuditTypeDDL()
		{
			if (ddlAuditType.Items.Count == 0)
			{
				try
				{
					ddlAuditType.Items.Add(new ListItem("", ""));
					var auditTypeList = EHSAuditMgr.SelectAuditTypeList(_companyId);
					foreach (var t in auditTypeList)
						ddlAuditType.Items.Add(new ListItem(t.TITLE, t.AUDIT_TYPE_ID.ToString()));
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

			rgCurrentQuestions.DataSource = EHSAuditMgr.SelectAuditQuestionList(Convert.ToInt32(selectedValue), _companyId, 0);
			rgCurrentQuestions.DataBind();

			rgAllQuestions.Skin = "Metro";
			rgAllQuestions.DataSource = EHSAuditMgr.SelectAuditQuestionList();
			rgAllQuestions.DataBind();
		}

		#endregion

		#region Events

		protected void ddlAuditType_SelectedIndexChanged(object sender, EventArgs e)
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
