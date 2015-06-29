using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{
    /* common user control for managing page label translation lists */
    public partial class Ucl_PageLabelList : System.Web.UI.UserControl
    {
        public GridView PageLabelListGrid
        {
            get { return gvPageLabelList; }
        }

        public GridView GetPageLabelListGrid()
        {
            return gvPageLabelList;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        public void gvPageLabelList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                System.Web.UI.WebControls.HiddenField hf;
                System.Web.UI.WebControls.Label lbl;

                try
                {
                    hf = (HiddenField)e.Row.Cells[0].FindControl("hfPageID");
                    if (Convert.ToInt32(hf.Value) > 100)
                    {
                        e.Row.Cells[1].Style.Add("FONT-WEIGHT", "BOLD");
                    }

                    if (SessionManager.PageMode == PageMode.TranslatePage || SessionManager.PageMode == PageMode.TranslateAll)
                    {
                        lbl = (Label)e.Row.Cells[0].FindControl("lblLabelName");
                        PAGE_LABELS baseLabel = (PAGE_LABELS)SessionManager.SessionContext.AppPage(Convert.ToInt32(hf.Value)).BaseLabelSet.Get(hf.Value + "~" + lbl.Text);
                        if (baseLabel != null)
                        {
                            lbl = (Label)e.Row.Cells[0].FindControl("lblBaseLabelText");
                            lbl.Text = baseLabel.TEXT;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        protected void btnSavePageLabels_Click(object sender, EventArgs e)
        {
            SessionContext sessionContext = SessionManager.SessionContext;
            PageContext thePage = null;
            GridView gv = gvPageLabelList;
            TextBox tbText;
            Label lblName;
            Label lblPageID;
            int pageID = 0;

            for (int intRow = 0; intRow < gv.Rows.Count; intRow++)
            {
                if (gv.Rows[intRow].Visible)
                {
                    lblPageID = (Label)gv.Rows[intRow].FindControl("lblPageID_out");
                    pageID = Convert.ToInt32(lblPageID.Text);

                    if (thePage != null && pageID != thePage.AppPage.PAGE_ID)
                    {
                        // save label changes for each page having controls in the list
                        SQMModelMgr.UpdatePageContext(sessionContext, thePage.AppPage.PAGE_NAME);
                    }

                    lblName = (Label)gv.Rows[intRow].FindControl("lblLabelName");
                    thePage = sessionContext.AppPage(Convert.ToInt32(lblPageID.Text));
                    tbText = (TextBox)gv.Rows[intRow].FindControl("tbLabelText");
                    thePage.Label(lblName.Text).TEXT = tbText.Text;
                    thePage.Label(lblName.Text).HELP_TEXT = tbText.Text;
                }
            }

            if (thePage != null)
                SQMModelMgr.UpdatePageContext(sessionContext, thePage.AppPage.PAGE_NAME);

            SessionManager.SessionContext = sessionContext;
        }
    }
}