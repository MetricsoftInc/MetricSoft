using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    public delegate void PageTabClick(string tabID, string cmdArg);

    public partial class Ucl_PageTabs : System.Web.UI.UserControl
    {
        public event PageTabClick OnPageTabClick;


        public Panel TabsPanel
        {
            get { return pnlPageTabs ; }
        }

        public LinkButton Tab0
        {
            get { return lbTab0; }
        }
        public LinkButton Tab1
        {
            get { return lbTab1; }
        }
        public LinkButton Tab2
        {
            get { return lbTab2; }
        }
        public LinkButton Tab3
        {
            get { return lbTab3; }
        }
        public LinkButton Tab4
        {
            get { return lbTab4; }
        }
        public LinkButton Tab5
        {
            get { return lbTab5; }
        }
        public LinkButton Tab6
        {
            get { return lbTab6; }
        }
        public LinkButton Tab7
        {
            get { return lbTab7; }
        }
        public LinkButton Tab8
        {
            get { return lbTab8; }
        }

        public void SetTitle(string titleText)
        {
            lblPageTabsTitle.Text = titleText;
        }
 
        protected void tab_Click(object sender, EventArgs e)
        {
            if (OnPageTabClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnPageTabClick(lnk.ClientID, lnk.CommandArgument.ToString());
            }
        }

        public void SetTabVisible(int tabNum, bool visible)
        {
            string tabID = "lbTab" + tabNum.ToString();
            LinkButton lbTab = (LinkButton)hfControl.FindControl(tabID);
            if (lbTab != null)
            {
                lbTab.Visible = visible;
            }
        }

        public void SetTabEnabled(int tabNum, bool enabled)
        {
            string tabID = "lbTab" + tabNum.ToString();
            LinkButton lbTab = (LinkButton)hfControl.FindControl(tabID);
            if (lbTab != null)
            {
              //  lbTab.Enabled = enabled;
                if (enabled)
                {
                    lbTab.Attributes.Remove("class");
                    lbTab.Attributes.Add("class", "optNav");
                    lbTab.Attributes.Add("onClick", "return true;");
                }
                else
                {
                    lbTab.Attributes.Remove("class");
                    lbTab.Attributes.Remove("href");
                    lbTab.Attributes.Add("class", "optNavDisable");
                    lbTab.Attributes.Add("onClick", "return false;");
                }
            }
        }

        public void SetAllTabsEnabled(bool enabled)
        {
            for (int i = 0; i < 9; i++)
                SetTabEnabled(i, enabled);
        }

        public void SetTabImage(int tabNum, string imageURL, string imageText)
        {
            string tabID = "imgTab" + tabNum.ToString();
            Image img = (Image)hfControl.FindControl(tabID);
            if (img != null)
            {
                img.ImageUrl = imageURL;
                img.ToolTip = imageText;
                img.Visible = true;
            }
        }

        public void SetTabLabel(int tabNum, string labelText)
        {
            string tabID = "lbTab" + tabNum.ToString();
            LinkButton lbTab = (LinkButton)hfControl.FindControl(tabID);
            if (lbTab != null)
            {
                lbTab.Text = labelText;
            }
        }

        public void SetTabLabelsFromList(Dictionary<string, string> tabList)
        {
            string tabID;
            LinkButton lbTab = null;
            foreach (KeyValuePair<string, string> label in tabList)
            {
                if ((lbTab = (LinkButton)hfControl.FindControl("lbTab" + label.Key)) != null)
                    lbTab.Text = label.Value;
            }
        }

        public string GetTabLabel(string tabID)
        {
            string label = "";
            try
            {
                LinkButton lbTab = (LinkButton)hfControl.FindControl(tabID);
                label = lbTab.Text;
            }
            catch
            {
            }

            return label;
        }
    }
}