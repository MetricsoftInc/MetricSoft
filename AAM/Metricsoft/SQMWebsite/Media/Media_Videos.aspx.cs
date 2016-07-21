using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Title = Resources.LocalizedText.MediaVideos;
			this.lblPlantSelect.Text = Resources.LocalizedText.Locations + ":";
			this.lblStatus.Text = Resources.LocalizedText.Status + ":";
			this.lblToDate.Text = Resources.LocalizedText.To + ":";
			this.lblVideoType.Text = Resources.LocalizedText.VideoType + ":";

			RadPersistenceManager1.PersistenceSettings.AddSetting(ddlPlantSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbStatusSelect);
			RadPersistenceManager1.PersistenceSettings.AddSetting(uclVideoList.VideoListEhsGrid);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbVideoType);
			RadPersistenceManager1.PersistenceSettings.AddSetting(rcbVideoOwner);

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

			bool uploadVideoAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.media);
			rbNew.Visible = uploadVideoAccess;


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
					divVideoList.Visible = false;
					uclVideoForm.Visible = false;
					recordType = (int)TaskRecordType.Media;
					uclAttachVideo.OpenManageVideosWindow(recordType, 0, "", "Upload Video", "Upload new video", "", 0, 0);

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

				List<XLAT> xlatList = SQMBasePage.SelectXLATList(new string[2] { "MEDIA_VIDEO_TYPE", "MEDIA_VIDEO_STATUS" }, 1);
				rcbVideoType = SQMBasePage.SetComboBoxItemsFromXLAT(rcbVideoType, xlatList.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_TYPE" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbVideoType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbVideoType.SelectedIndex = 0;
				rcbStatusSelect = SQMBasePage.SetComboBoxItemsFromXLAT(rcbStatusSelect, xlatList.Where(l => l.XLAT_GROUP == "MEDIA_VIDEO_STATUS" && l.STATUS == "A").OrderBy(h => h.SORT_ORDER).ToList(), "SHORT");
				rcbStatusSelect.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("All", ""));
				rcbStatusSelect.SelectedIndex = 0;
			}

			divVideoList.Visible = true;

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

			string selectedType = rcbVideoType.SelectedValue.ToString();
			List<decimal> types = new List<decimal>();
			if (rcbVideoType.SelectedValue.ToString().Equals(""))
				types.Add(0);
			else
			{
				foreach (ListItem item in rcbVideoType.Items)
				{
					if (!item.Value.Equals(""))
						types.Add(Convert.ToDecimal(item.Value.ToString()));
				}
			}

			List<MediaVideoData> videos = MediaVideoMgr.SelectVideoList(plantIDS, types, fromDate, toDate, rcbStatusSelect.SelectedValue.ToString());
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