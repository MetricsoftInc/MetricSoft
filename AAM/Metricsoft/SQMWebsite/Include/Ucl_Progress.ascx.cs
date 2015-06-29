using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Ucl_Progress : System.Web.UI.UserControl
    {
        static private RadProgressContext staticProgressContext;

        protected void Page_Load(object sender, EventArgs e)
        {
           // radProgressArea.Localization.CurrentFileName = "";
        }

        public void BindProgressDisplay(int totalIndex, string progressLabel)
        {
            radProgressArea.Localization.CurrentFileName = progressLabel;
            staticProgressContext = RadProgressContext.Current;
            staticProgressContext.SecondaryTotal = 100;
        }

        public void UpdateDisplay(int stepNum, int progressValue, string progressText)
        {
            staticProgressContext.SecondaryValue = progressValue;
            staticProgressContext.SecondaryPercent = progressValue;
            staticProgressContext.CurrentOperationText = progressText;
            System.Threading.Thread.Sleep(50);
        }

        public void ProgressComplete()
        {
            staticProgressContext.OperationComplete = true;
        }
    }
}