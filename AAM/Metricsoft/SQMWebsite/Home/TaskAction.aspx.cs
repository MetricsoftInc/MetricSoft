using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Website.Classes;
using SQM.Shared;
using Telerik.Web.UI;

namespace SQM.Website
{
	public partial class TaskAction : SQMBasePage
	{
		public string ReturnURL
		{
			get { return ViewState["ReturnURL"] == null ? "/Home/Calendar.aspx" : (string)ViewState["ReturnURL"]; }
			set { ViewState["ReturnURL"] = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			uclTask.OnTaskUpdate += ReturnRedirect;
		}

		protected void Page_Load(object sender, EventArgs e)
		{

			if (!IsPostBack)
			{
				HiddenField hf = (HiddenField)this.Form.Parent.FindControl("form1").FindControl("hdCurrentActiveMenu");
				hf.Value = SessionManager.CurrentMenuItem = "lbHomeMain";
				IsCurrentPage();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("SQM", 10, true, true, hfDocviewMessage.Value);
					ucl.BindDocumentSelect("EHS", 10, true, false, hfDocviewMessage.Value);
				}

				if (SessionManager.ReturnObject is TASK_STATUS)
				{
					TASK_STATUS task = SessionManager.ReturnObject as TASK_STATUS;
					ReturnURL = SessionManager.ReturnPath;
					SessionManager.ClearReturns();
					if (task.TASK_STEP == ((int)SysPriv.action).ToString())
					{
						UpdateSelectedTask(task.TASK_ID);
					}
				}
			}
		}

		private void UpdateSelectedTask(decimal taskID)
		{
			TaskStatusMgr taskMgr = new TaskStatusMgr().CreateNew(0, 0);
			TASK_STATUS task = taskMgr.SelectTask(taskID);
			uclTask.BindTaskUpdate(task, "");
		}

		private void ReturnRedirect(string cmd)
		{
			Response.Redirect(ReturnURL);
		}
	}
}