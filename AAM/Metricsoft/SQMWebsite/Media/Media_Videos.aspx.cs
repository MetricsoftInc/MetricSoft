using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace SQM.Website	
{
	public partial class Media_Videos : SQMBasePage
	{
		protected enum DisplayState
		{
			VideoList,
			VideoNew,
			VideoEdit,
			VideoDisplay
		}

		protected override void OnInit(EventArgs e)
		{
			uclVideoUpload.AttachmentEvent += AddVideoResponse;
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.MediaVideos;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ":";
			this.lblToDate.Text = Resources.LocalizedText.To + ":";
			this.lblVideoSource.Text = Resources.LocalizedText.VideoSourceType + ":";
			this.lblVideoOwner.Text = Resources.LocalizedText.VideoOwner + ":";
			this.lblVideoType.Text = Resources.LocalizedText.VideoType + ":";
			this.lblBodyPart.Text = Resources.LocalizedText.BodyPart + ":";
			this.lblInjuryType.Text = Resources.LocalizedText.InjuryType + ":";


			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			//RadPersistenceManager1.PersistenceSettings.AddSetting(uclVideoList.VideoListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbVideoSource);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbVideoOwner);

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			SetSubnav("setup");

			if (IsPostBack)
			{
				RadPersistenceManager1.SaveState();

				if (SessionManager.ReturnStatus == true)
				{
					if (SessionManager.ReturnObject is string)
					{
						string type = SessionManager.ReturnObject as string;
						switch (type)
						{
							case "DisplayVideos":
								UpdateDisplayState(DisplayState.VideoList);
								break;

							case "AddVideo": // we added the video, now we want to go into edit mode
							case "Notification": // this is when we select the video to edit from the list
								uclVideoForm.EditVideoId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.VideoEdit);
								// need to determine if the Audit is past due and force it into display mode (probelm when coming from Calendar)
								//string auditStatus = EHSAuditMgr.SelectAuditStatus(SessionManager.ReturnRecordID);
								//if (auditStatus == "C")
								//	UpdateDisplayState(DisplayState.AuditNotificationDisplay);
								//else
								//	UpdateDisplayState(DisplayState.AuditNotificationEdit);
								//if (isDirected)
								//{
								//	rbNew.Visible = false;
								//	uclAuditForm.EnableReturnButton(false);
								//}
								break;

							case "DisplayOnly":
								uclVideoForm.EditVideoId = SessionManager.ReturnRecordID;
								UpdateDisplayState(DisplayState.VideoDisplay);
								//if (isDirected)
								//{
								//	rbNew.Visible = false;
								//	uclAuditForm.EnableReturnButton(false);
								//}
								break;
						}
					}
					SessionManager.ClearReturns();
				}
			}
			else
			{
				//Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				//if (ucl != null)
				//{
				//	ucl.BindDocumentSelect("EHS", 2, true, true, "");
				//}

				if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
				{
					try
					{
						// from inbox
						DisplayNonPostback();
						SessionManager.ReturnRecordID = Convert.ToDecimal(SessionManager.ReturnObject.ToString());
						SessionManager.ReturnObject = "Notification";
						SessionManager.ReturnStatus = true;
						//isDirected = true;

						StringBuilder sbScript = new StringBuilder();
						ClientScriptManager cs = Page.ClientScript;

						sbScript.Append("<script language='JavaScript' type='text/javascript'>\n");
						sbScript.Append("<!--\n");
						sbScript.Append(cs.GetPostBackEventReference(this, "PBArg") + ";\n");
						sbScript.Append("// -->\n");
						sbScript.Append("</script>\n");

						cs.RegisterStartupScript(this.GetType(), "AutoPostBackScript", sbScript.ToString());
					}
					catch
					{
						// not a number, parse as type
						DisplayNonPostback();
					}
				}
				else
				{
					DisplayNonPostback();
				}

			}

		}

		protected void DisplayNonPostback()
		{
			SetupPage();

			try
			{
				RadPersistenceManager1.LoadState();
			}
			catch
			{
			}

			if (SessionManager.ReturnStatus == null || SessionManager.ReturnStatus != true)
				SearchVideos();      // suppress list when invoking page from inbox

			//Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
			//if (ucl != null)
			//{
			//	ucl.BindDocumentSelect("EHS", 2, true, true, "");
			//}

		}

		protected void SetSubnav(string context)
		{
			bool uploadVideoAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.media);
			rbNew.Enabled = uploadVideoAccess;
		}

		protected void AddVideoResponse(string cmd)
		{
			//string script = "function f(){CloseVideoUploadWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			//UpdateDisplayState(DisplayState.VideoList);
			divVideoUpload.Visible = false;
			//UpdateDisplayState(DisplayState.VideoList);
			rbNew.Visible = true;
			SetSubnav("list");
			if (cmd == "save")
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.VideoSaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void UpdateDisplayState(DisplayState state)
		{
			int recordType = 0;
			switch (state)
			{
				case DisplayState.VideoList:
					SearchVideos();
					uclVideoForm.Visible = false;
					rbNew.Visible = true;
					divVideoList.Visible = true;
					break;

				case DisplayState.VideoNew:
					//divVideoList.Visible = false;
					//uclVideoForm.Visible = false;
					recordType = (int)TaskRecordType.Media;
					//uclAttachVideo.OpenManageVideosWindow(recordType, 0, "", "Upload Video", "Upload new video", "", "", "", 0);

					uclVideoUpload.OpenManageVideosWindow(recordType, 0, "", SessionManager.UserContext.HRLocation.Plant.PLANT_ID, "Upload Video", "Upload New Video", "", "", "", PageUseMode.EditEnabled, true, "mediapage");
					divVideoUpload.Visible = true;
					divVideoList.Visible = false; 
					rbNew.Visible = false;
					//string script = "function f(){OpenVideoUploadWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
					//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);


					//uclVideoForm.Visible = true;
					//uclVideoForm.IsEditContext = false;
					//uclVideoForm.ClearControls();
					//rbNew.Visible = false;
					//uclVideoForm.CheckForSingleType();
					break;

				case DisplayState.VideoEdit:
					divVideoList.Visible = false;
					uclVideoForm.CurrentStep = 0;
					uclVideoForm.IsEditContext = true;
					uclVideoForm.Visible = true;
					rbNew.Visible = false;
					//uclVideoForm.LoadVideoInformation();
					//uclAuditForm.BuildForm();
					break;

				case DisplayState.VideoDisplay:
					divVideoList.Visible = false;
					//uclAuditForm.CurrentStep = 1;
					//uclAuditForm.IsEditContext = false;
					//rbNew.Visible = false;
					//uclAuditForm.Visible = true;
					break;

			}

			SessionManager.ClearReturns();
		}

		private void SetupPage()
		{
			if (ddlPlantSelect.Items.Count < 1)
			{
				List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				SQMBasePage.SetLocationList(ddlPlantSelect, UserContext.FilterPlantAccessList(locationList), 0);

				List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[5] { "INJURY_PART", "INJURY_TYPE", "MEDIA_VIDEO_SOURCE", "MEDIA_VIDEO_STATUS", "MEDIA_VIDEO_TYPE" }, 1);
				rcbVideoSource = SQMBasePage.SetComboBoxItemsFromXLAT(rcbVideoSource, xlatList.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_SOURCE" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbVideoSource.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbVideoSource.SelectedIndex = 0;
				rcbStatusSelect = SQMBasePage.SetComboBoxItemsFromXLAT(rcbStatusSelect, xlatList.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_STATUS" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbStatusSelect.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbStatusSelect.SelectedIndex = 0;
				rcbVideoType = SQMBasePage.SetComboBoxItemsFromXLAT(rcbVideoType, xlatList.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_TYPE" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				//rcbStatusSelect.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				//rcbStatusSelect.SelectedIndex = 0;
				rcbInjuryType = SQMBasePage.SetComboBoxItemsFromXLAT(rcbInjuryType, xlatList.Where(l => l.XLAT_GROUP == "INJURY_TYPE" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbInjuryType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbInjuryType.SelectedIndex = 0;
				rcbBodyPart = SQMBasePage.SetComboBoxItemsFromXLAT(rcbBodyPart, xlatList.Where(l => l.XLAT_GROUP == "INJURY_PART" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbBodyPart.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbBodyPart.SelectedIndex = 0;
			}

			divVideoList.Visible = true;
			rbNew.Visible = true;

			dmFromDate.ShowPopupOnFocus = dmToDate.ShowPopupOnFocus = true;
			// ABW 1/5/16 - use user's default plant local time for search default
			DateTime localTime = SessionManager.UserContext.LocalTime;
			dmFromDate.SelectedDate = localTime.AddMonths(-1);
			dmToDate.SelectedDate = localTime.AddMonths(1);


			//lblStatus.Text = "Assessment Status:";

			lblVideoDate.Visible = true;
			phVideo.Visible = true;

			//SETTINGS sets = SQMSettings.GetSetting("EHS", "AUDITSEARCHFROM");
			//if (sets != null)
			//{
			//	try
			//	{
			//		string[] args = sets.VALUE.Split('-');
			//		if (args.Length > 1)
			//		{
			//			dmFromDate.SelectedDate = new DateTime(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
			//		}
			//		else
			//		{
			//			dmFromDate.SelectedDate = SessionManager.UserContext.LocalTime.AddMonths(Convert.ToInt32(args[0]) * -1);
			//		}
			//	}
			//	catch { }
			//}

		}

		#region click events
		protected void rbNew_Click(object sender, EventArgs e)
		{
			rbNew.Visible = false;
			rbNew.Enabled = false;

			UpdateDisplayState(DisplayState.VideoNew);
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "DisplayVideos";
		}

		protected void lnkVideoDetailsClose_Click(object sender, EventArgs e)
		{

		}
		#endregion

		#region video selection
		private void SearchVideos()
		{
			string selectedValue = "";
			DateTime fromDate = Convert.ToDateTime(dmFromDate.SelectedDate);
			DateTime toDate = Convert.ToDateTime(dmToDate.SelectedDate);
			if (toDate < fromDate)
				return;

			toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

			List<decimal> plantIDS = SQMBasePage.GetComboBoxCheckedItems(ddlPlantSelect).Select(i => Convert.ToDecimal(i.Value)).ToList();
			List<string> videoTypes = SQMBasePage.GetComboBoxCheckedItems(rcbVideoType).Select(i => i.Value).ToList();

			string selectedType = rcbVideoSource.SelectedValue.ToString();
			List<decimal> types = new List<decimal>();
			if (rcbVideoSource.SelectedValue.ToString().Equals(""))
				types.Add(0);
			else
			{
				foreach (RadComboBoxItem item in rcbVideoSource.Items)
				{
					if (!item.Value.Equals("") && item.Selected)
						types.Add(Convert.ToDecimal(item.Value.ToString()));
				}
			}

			List<string> injuryTypes = new List<string>();
			if (rcbInjuryType.SelectedValue.ToString().Equals(""))
				injuryTypes.Add("0");
			else
			{
				foreach (RadComboBoxItem item in rcbInjuryType.Items)
				{
					if (!item.Value.Equals("") && item.Selected)
						injuryTypes.Add(item.Value.ToString());
				}
			}

			List<string> bodyParts = new List<string>();
			if (rcbBodyPart.SelectedValue.ToString().Equals(""))
				bodyParts.Add("0");
			else
			{
				foreach (RadComboBoxItem item in rcbBodyPart.Items)
				{
					if (!item.Value.Equals("") && item.Selected)
						bodyParts.Add(item.Value.ToString());
				}
			}

			string videoOwner = rcbVideoOwner.SelectedValue.ToString();
			decimal videoOwnerId = 0;
			if (videoOwner == "own")
				videoOwnerId = SessionManager.UserContext.Person.PERSON_ID;

			List<MediaVideoData> videos = MediaVideoMgr.SelectVideoList(plantIDS, types, fromDate, toDate, rcbStatusSelect.SelectedValue.ToString(), tbKeyWord.Text.ToString().ToLower(), injuryTypes, bodyParts, videoTypes, videoOwnerId);
			// we don't want to list any videos that have a negative source id. these could be videos in process or orphaned on the Incident pages
			videos = videos.Where(q => q.Video.SOURCE_ID >= 0).ToList();
			uclVideoList.BindVideoListRepeater(videos, "Media");
			//List<string> statusList = new List<string>();

			//	typeList = rcbVideoType.Items.Where(c => c.Checked).Select(c => Convert.ToDecimal(c.Value)).ToList();
			//	selectedValue = rcbStatusSelect.SelectedValue;

			//SetHSCalcs(new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", fromDate, toDate, new decimal[0]));
			//HSCalcs().ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
			////HSCalcs().ObjAny = cbShowImage.Checked;

			//HSCalcs().ehsCtl.SelectAuditList(plantIDS, typeList, fromDate, toDate, selectedValue);

			//// may want to access only the ones assigned to that person
			////if (accessLevel < AccessMode.Admin)
			////	HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.ISSUE_TYPE_ID != 10 select i).ToList();
			//bool allAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
			//if (!allAuditAccess)
			//	HSCalcs().ehsCtl.AuditHst = (from i in HSCalcs().ehsCtl.AuditHst where i.Audit.AUDIT_PERSON == SessionManager.UserContext.Person.PERSON_ID select i).ToList();

			//if (HSCalcs().ehsCtl.AuditHst != null)
			//{
			//	HSCalcs().ehsCtl.AuditHst.OrderByDescending(x => x.Audit.AUDIT_DT);
			//	uclAuditList.BindAuditListRepeater(HSCalcs().ehsCtl.AuditHst, "EHS");
			//}
			////}

			//pnlAuditDetails.Visible = lnkAuditDetailsClose.Visible = false;

		}

		#endregion
	}
}