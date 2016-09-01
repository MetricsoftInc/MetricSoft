using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.IO;
using SQM.Website.Classes;


namespace SQM.Website.Shared
{
	public partial class VideoHandler : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(Request.QueryString["DOC_ID"]))
				{
					string document_id = Request.QueryString["DOC_ID"];
					decimal doc_id = decimal.Parse(document_id);
					VIDEO v = MediaVideoMgr.SelectVideoById(doc_id);

					List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("MEDIA_UPLOAD", "");
					string storageContainer = sets.Find(x => x.SETTING_CD == "STORAGE_CONTAINER").VALUE.ToString();
					string storageURL = sets.Find(x => x.SETTING_CD == "STORAGE_URL").VALUE.ToString();
					string storageQueryString = sets.Find(x => x.SETTING_CD == "STORAGE_QUERY").VALUE.ToString();

					int index = v.FILE_NAME.ToString().IndexOf(".");
					string fileType = v.FILE_NAME.Substring(index);
					string videoSrc = storageURL + storageContainer + "/" + v.VIDEO_ID.ToString() + fileType + storageQueryString;
					srcControl.Attributes.Add("src", videoSrc);

					if (fileType.ToLower().Equals(".mp4"))
					{
						srcControl.Attributes.Add("type", "video/mp4");
					}
				}
			}
			catch (Exception ex)
			{
				//
				Response.Write("Unable to stream video: " + ex.Message.ToString()); Response.End();
			}
		}
	}
}