using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using SQM.Website.Classes;
using SQM.Shared;

namespace SQM.Website
{
    public partial class Media_Upload : SQMBasePage
    {
        private List<DOCUMENT> files = new List<DOCUMENT>();

		public string UploadSource
		{
			get { return ViewState["UploadSource"] == null ? "" : (string)ViewState["UploadSource"]; }
			set { ViewState["UploadSource"] = value; }
		}

		public decimal SourceId
		{
			get { return ViewState["SourceId"] == null ? 0 : (decimal)ViewState["SourceId"]; }
			set { ViewState["SourceId"] = value; }
		}

		public string SourceStep
		{
			get { return ViewState["SourceStep"] == null ? "" : (string)ViewState["SourceStep"]; }
			set { ViewState["SourceStep"] = value; }
		}

		public DateTime SourceDate
		{
			get { return ViewState["SourceDate"] == null ? DateTime.Now : (DateTime)ViewState["SourceDate"]; }
			set { ViewState["SourceDate"] = value; }
		}

		public int SourceType // incident/audit type - we will determine the correct list to access based on the UploadSource
		{
			get { return ViewState["SourceType"] == null ? 0 : (int)ViewState["SourceType"]; }
			set { ViewState["SourceType"] = value; }
		}

		public int BodyPart 
		{
			get { return ViewState["BodyPart"] == null ? 0 : (int)ViewState["BodyPart"]; }
			set { ViewState["BodyPart"] = value; }
		}

		public int InjuryType
		{
			get { return ViewState["InjuryType"] == null ? 0 : (int)ViewState["InjuryType"]; }
			set { ViewState["InjuryType"] = value; }
		}

		#region Event Handlers

		protected void Page_Load(object sender, EventArgs e)
        {
            //gvUploadedFiles.Visible = false;
            //Bind_gvUploadedFiles();
        }

        protected void lbUpload_Click(object sender, EventArgs e)
        {
            string name = "";
			string fileExtension = "";
			string fileLocation = "~/Videos/";

			if (flFileUpload.HasFile)
			{
				name = flFileUpload.FileName;
				fileExtension = flFileUpload.FileName.Substring(flFileUpload.FileName.IndexOf(".") + 1);
				// check to see if this is a video?

				Stream stream = flFileUpload.FileContent;
				// first we need to create the video header (String fileLocation, String fileExtention, String description, string videoTitle, int sourceType, decimal sourceId, string sourceStep, string injuryType, string bodyPart, string docScope, DateTime videoDate, DateTime incidentDate)
				VIDEO video = MediaVideoMgr.Add(name, fileExtension, tbFileDescription.Text.ToString(), tbTitle.Text.ToString(), SourceType, SourceId, SourceStep, ddlInjuryType.SelectedValue.ToString(), rdlBodyPart.SelectedValue.ToString(), "", (DateTime)dmFromDate.SelectedDate, SourceDate);
				// next, save the video to the server; file name = VIDEO_ID
				string fileName = "";
				DateTime dtIncidentDate = new DateTime();
				// next we create the video header
				if (video != null)
				{
					// Bind_gvUploadedFiles(); - we are NOT going to bind the video, because we are only allowing ONE at a time
					// mt - put the new document and upload status in session so that we can retrieve it (if necessary) from the calling page
					try
					{
						flFileUpload.SaveAs(video.FILE_NAME);
						SessionManager.ReturnObject = video;
						SessionManager.ReturnStatus = true;
					}
					catch (Exception ex)
					{
						// put up an error
					}
				}
				else
				{
					SessionManager.ClearReturns();
				}

			}
        }

       
        protected void Page_PreRender(object sender, EventArgs e)
        {
            lbUpload.Attributes.Add("onmouseup", "ShowModalDialog();");
            if (ddlInjuryType.Items.Count == 0)
            {
				PopulateInjuryTypeDropDown();
				PopulateBodyPartDropDown();
			}
			dmFromDate.SelectedDate = SourceDate;
			if (InjuryType == 0)
				ddlInjuryType.SelectedValue = "";
			else
				ddlInjuryType.SelectedValue = InjuryType.ToString();
			if (BodyPart == 0)
				rdlBodyPart.SelectedValue = "";
			else
				rdlBodyPart.SelectedValue = BodyPart.ToString();

		}

		protected void gvUploadedFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            decimal document_id = (decimal)e.Keys[0];
            SQMDocumentMgr.Delete(document_id);
            Bind_gvUploadedFiles();
        }

        #endregion

        #region Helper methods

        protected string FormatFilesize(Object d)
        {
            if (d == null)
            {
                return "";
            }
            string f = WebSiteCommon.GetFileSizeReadable(long.Parse(d.ToString()));
            return f;
        }

        protected void Bind_gvUploadedFiles()
        {
            //files = null;

            //switch (SessionManager.DocumentContext.Scope)
            //{
            //    case "USR":
            //        files = SQMDocumentMgr.SelectDocListByOwner(SessionManager.UserContext.Person.PERSON_ID, 15);
            //        break;
            //    default:
            //        files = SQMDocumentMgr.SelectDocList(SessionManager.EffLocation.Company.COMPANY_ID, SessionManager.DocumentContext);
            //        break;
            //}

            //if (files != null  &&  files.Count > 0)
            //{
            //    gvUploadedFiles.DataSource = files;
            //    gvUploadedFiles.DataKeyNames = new string[] { "DOCUMENT_ID" };
            //    gvUploadedFiles.DataBind();
            //    gvUploadedFiles.Visible = true;
            //}

        }

        public void gvUploadedFiles_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {

                try
                {
                    HiddenField hfField = (HiddenField)e.Row.Cells[0].FindControl("hfFileName");
                    string ext = hfField.Value.Substring(hfField.Value.LastIndexOf('.') + 1).ToLower();
                    if (!string.IsNullOrEmpty(ext))
                    {
                        Image img = (Image)e.Row.Cells[0].FindControl("imgFileType");
                        img.ImageUrl = "~/images/filetype/icon_" + ext + ".gif";
                    }
                }
                catch
                {
                }
            }
        }
		#endregion
		void PopulateInjuryTypeDropDown()
		{
			List<EHSMetaData> injtype = EHSMetaDataMgr.SelectMetaDataList("INJURY_TYPE");
			if (injtype != null && injtype.Count > 0)
			{
				ddlInjuryType.Items.Add(new ListItem("", ""));

				foreach (var s in injtype)
				{
					{
						ddlInjuryType.Items.Add(new ListItem(s.Text, s.Value));
					}
				}
			}
		}


		void PopulateBodyPartDropDown()
		{
			//bool categorize = EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault() != null && EHSSettings.Where(s => s.SETTING_CD == "INJURYPART_CATEGORIZE").FirstOrDefault().VALUE.ToUpper() == "Y" ? true : false;
			bool categorize = false;
			List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[1] { "INJURY_PART" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);
			SQMBasePage.SetCategorizedDropDownItems(rdlBodyPart, xlatList.Where(l => l.XLAT_GROUP == "INJURY_PART").ToList(), categorize);
		}


	}
}