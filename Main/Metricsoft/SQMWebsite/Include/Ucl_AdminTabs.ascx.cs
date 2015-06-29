using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public delegate void TabItemClick(string tabID, string cmdArg);

    public partial class Ucl_AdminTabs : System.Web.UI.UserControl
    {
        public event TabItemClick OnTabClick;

        public Panel CompanyPanel
        {
            get { return pnlCompanyTabs; }
        }
        public Panel BusOrgPanel
        {
            get { return pnlOrgTabs; }
        }
        public Panel PlantPanel
        {
            get { return pnlPlantTabs; }
        }
        public Panel PartPanel
        {
            get { return pnlPartTabs; }
        }
        public Panel UserPanel
        {
            get { return pnlUserTabs; }
        }
        public Panel QualityResourcePanel
        {
            get { return pnlQualityTabs; }
        }
		public Panel EHSResourcePanel
		{
			get { return pnlEHSTabs; }
		}

        protected void tab_Click(object sender, EventArgs e)
        {
            if (OnTabClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnTabClick(lnk.ClientID, lnk.CommandArgument.ToString());
            }
        }
    }
}