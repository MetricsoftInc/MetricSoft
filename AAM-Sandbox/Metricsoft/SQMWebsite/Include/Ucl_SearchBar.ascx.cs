using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQM.Website
{

    //public delegate void ActionClick();
    public delegate void ActionClick();
    public delegate void CommandClick(string cmd);

    public partial class Ucl_SearchBar : System.Web.UI.UserControl
    {
        public event ActionClick OnSearchClick;
        public event ActionClick OnNewClick;
        public event ActionClick OnEditClick;
        public event ActionClick OnUploadClick;
        public event ActionClick OnCancelClick;
        public event ActionClick OnSaveClick;
        public event ActionClick OnReturnClick;

       
        public Label PageTitle
        {
            get { return lblPageTitle; }
        }
        public Label TitleItem
        {
            get { return lblTitleItem_out; }
        }
        public Button SaveButton
        {
            get { return btnSave; }
        }
        public Button NewButton
        {
            get { return btnNew; }
        }
        public Button EditButton
        {
            get { return btnEdit; }
        }
        public Button CancelButton
        {
            get { return btnCancel; }
        }
        public Button UploadButton
        {
            get { return btnUpload; }
        }
        public Button ReturnButton
        {
            get { return btnReturn; }
        }
        public Button SearchButton
        {
            get { return btnSearch; }
        }
        /*
        public TextBox SearchText
        {
            get { return tbSearch; }
        }
        */
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
             //  tbSearch.Text = searchCriteria = "";
               SetButtonsNotClicked();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (OnSearchClick != null)
            {
                SetButtonsNotClicked();
              //  btnSearch.CssClass = "buttonClicked";
                OnSearchClick();
            }
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (OnNewClick != null)
            {
                SetButtonsNotClicked();
                btnNew.CssClass = "buttonClicked";
                OnNewClick();
            }
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (OnEditClick != null)
            {
                SetButtonsNotClicked();
                btnEdit.CssClass = "buttonClicked";
                OnEditClick();
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (OnCancelClick != null)
            {
                SetButtonsNotClicked();
                OnCancelClick();
            }
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (OnUploadClick != null)
            {
                btnUpload.CssClass = "buttonClicked";
                SetButtonsNotClicked();
                OnUploadClick();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (OnSaveClick != null)
            {
                SetButtonsNotClicked();
                OnSaveClick();
            }
        }
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (OnReturnClick != null)
            {
                SetButtonsNotClicked();
                OnReturnClick();
            }
        }

        public void DisplaySearchBar(bool visible)
        {
            divSearchBar.Visible = Visible;
        }

        public void SetButtonsNotClicked()
        {
            btnSearch.CssClass =   btnEdit.CssClass = btnCancel.CssClass = btnUpload.CssClass = "buttonStd";
            btnNew.CssClass = "buttonAddLarge";
            btnSave.CssClass = "buttonEmphasis";
        }

        public void SetButtonsVisible(bool searchVisible, bool editVisible, bool newVisible, bool saveVisible, bool uploadVisible, bool returnVisible)
        {
            SetButtonsVisible(searchVisible, editVisible, newVisible, saveVisible, uploadVisible, returnVisible, "");
        }

        public void SetButtonsVisible(bool searchVisible, bool editVisible, bool newVisible, bool saveVisible, bool uploadVisible, bool returnVisible, string returnText)
        {
            btnSearch.Visible = searchVisible;
           // tbSearch.Visible = searchVisible;
            btnEdit.Visible = editVisible;
            btnNew.Visible = newVisible;
            btnSave.Visible = btnCancel.Visible = saveVisible;
            btnUpload.Visible = uploadVisible;
            btnReturn.Visible = returnVisible;
            if (!string.IsNullOrEmpty(returnText))
                btnReturn.Text = returnText;
        }

        public void SetButtonsEnabled(bool searchEnabled, bool editEnabled, bool newEnabled, bool saveEnabled, bool uploadEnabled, bool returnEnabled)
        {
            DisplaySearchBar(true);
            btnSearch.Enabled  = searchEnabled;
            //tbSearch.Enabled = searchEnabled;
            btnEdit.Enabled = editEnabled;
            btnNew.Enabled = newEnabled;
            btnSave.Enabled = btnCancel.Enabled = saveEnabled;
            if (!btnSave.Enabled)
                lblTitleItem_out.Text = "";
            btnUpload.Enabled = uploadEnabled;
            if (returnEnabled)
                btnReturn.Enabled = btnReturn.Visible = true;
            else
                btnReturn.Enabled = btnReturn.Visible = false;
        }

 
    }
}