using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_Status : System.Web.UI.UserControl
    {

        public CheckBox ButtonComplete
        {
            get { return cbStatusComplete; }
        }

        protected void AddToolTip(string id, string helpText)
        {
            var imgHelp = new Image()
            {
                ID = "help_" + id,
                ImageUrl = "/images/ico-question.png",
            };

            //string txt = "<table class='borderSoft'><tr><td>Parameter</td><td>Value</td></tr><tr><td>energy</td><td>123456</td></tr></table>";

            var rttHelp = new RadToolTip()
            {
                Text = "<div style=\"font-size: 11px; line-height: 1.5em;\">" + helpText + "</div>",
                TargetControlID = imgHelp.ID,
                IsClientID = false,
                RelativeTo = ToolTipRelativeDisplay.Element,
                Width = 320,
                Height = 160,
                Animation = ToolTipAnimation.Fade,
                Position = ToolTipPosition.MiddleRight,
                ContentScrolling = ToolTipScrolling.Auto,
                Skin = "Metro",
                AutoCloseDelay = 0
            };

            trStatusInfo.Controls.Add(new LiteralControl("<span style=\"float: right;\">"));
            trStatusInfo.Controls.Add(imgHelp);
            trStatusInfo.Controls.Add(new LiteralControl("</span>"));
            trStatusInfo.Controls.Add(rttHelp);
        }

        public bool BindTaskComplete(TASK_STATUS task)
        {
            return BindTaskComplete(task, "");
        }

        public bool BindTaskComplete(TASK_STATUS task, string helpInfo)
        {
            pnlStatusComplete.Visible = true;

            bool isComplete = false;
            if (task != null) 
            {
                if (task.STATUS == Convert.ToInt32(TaskStatus.Complete).ToString())
                {
                    cbStatusComplete.Checked = isComplete = true;
                    cbStatusComplete.Enabled = false;
                    PERSON person = SQMModelMgr.LookupPerson((decimal)task.COMPLETE_ID, "");
                    if (person != null)
                        cbStatusComplete.Text = "Completed by " + person.FIRST_NAME + " " + person.LAST_NAME;
                }

                if (!string.IsNullOrEmpty(helpInfo))
                    AddToolTip(task.TASK_STEP, helpInfo);
            }

            return isComplete;
        }

        public bool UpdateTaskComplete(TASK_STATUS task)
        {

            return UpdateTaskComplete(task, true);
        }

        public bool UpdateTaskComplete(TASK_STATUS task, bool checkPriorComplete)
        {
            bool nowComplete = false;

                if (cbStatusComplete.Checked)
                    nowComplete = true;

            return nowComplete;
        }
    }
}