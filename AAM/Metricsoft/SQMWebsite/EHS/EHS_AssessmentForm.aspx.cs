using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;
using System.Web.UI.HtmlControls;

namespace SQM.Website.EHS
{
	public partial class EHS_AssessmentForm : System.Web.UI.Page
	{
		const Int32 MaxTextLength = 4000;

		protected enum DisplayState
		{
			AuditList,
			AuditNotificationNew,
			AuditNotificationEdit,
			AuditNotificationClosed,
			AuditNotificationDisplay,
			AuditReportEdit
		}

		int[] requiredToCloseFields = new int[] {
				(int)EHSAuditQuestionId.Containment,
				(int)EHSAuditQuestionId.RootCause,
				(int)EHSAuditQuestionId.RootCauseOperationalControl,
				(int)EHSAuditQuestionId.CorrectiveActions,
				(int)EHSAuditQuestionId.Verification,
				(int)EHSAuditQuestionId.ResponsiblePersonDropdown,
				(int)EHSAuditQuestionId.DateDue
			};

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected decimal auditPlantId = 0;

		bool controlQuestionChanged;

		List<EHSAuditQuestion> questions;
		PSsqmEntities entities;

		// Special answers used in AUDIT table
		string auditDescription = "";
		protected DateTime auditDate;
		protected decimal auditTypeId;
		protected string auditType;
		protected bool allQuestionsAnswered;

		protected string previousTopic;
		protected decimal previousTopicId;
		protected int percentInTopic; // used to determine if the percentage value should show for the specific topic.
		protected bool showTotals; // are we showing totals at all for the audit?
		protected int totalTopicQuestions;
		protected int totalQuestions;
		protected decimal totalTopicPositive;
		protected decimal totalTopicWeightScore;
		protected decimal totalWeightScore;
		protected decimal possibleScore;
		protected decimal totalPossibleScore;
		protected decimal totalTopicPossibleScore;
		protected decimal totalPercent;
		protected decimal totalPositive;


		protected bool IsEditContext;
		protected int CurrentStep;
		protected decimal EditAuditId;
		protected decimal EditAuditTypeId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			//uclAuditForm.OnAttachmentListItemClick += OpenFileUpload;
			//uclAuditForm.OnExceptionListItemClick += AddTask;
			uclAttachWin.AttachmentEvent += OnAttachmentsUpdate;
			uclAttachVideoWin.AttachmentEvent += OnVideoUpdate;
			uclTask.OnTaskAdd += UpdateTaskList;
			uclVideoUpload.AttachmentEvent += OnVideoUpdate;
		}

		/*
		protected void Page_Init(object sender, EventArgs e)
		{
			//uclAuditForm.OnAttachmentListItemClick += OpenFileUpload;
			//uclAuditForm.OnExceptionListItemClick += AddTask;
			uclAttachWin.AttachmentEvent += OnAttachmentsUpdate;
			uclAttachVideoWin.AttachmentEvent += OnVideoUpdate;
			uclTask.OnTaskAdd += UpdateTaskList;
			uclVideoUpload.AttachmentEvent += OnVideoUpdate;
		}
		*/

		protected void Page_Load(object sender, EventArgs e)
		{
			entities = new PSsqmEntities();
			if (!Page.IsPostBack)
			{
				this.Title = Resources.LocalizedText.EHSAudits;
				companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				if (SessionManager.ReturnStatus == true)
				{
					try
					{
						EditAuditId = Convert.ToDecimal(SessionManager.ReturnRecordID);
						DisplayState state = (DisplayState)SessionManager.ReturnObject;
						SessionManager.ClearReturns();
						UpdateDisplayState(state);
					}
					catch
					{
						SessionManager.ReturnStatus = false;
						SessionManager.ReturnObject = "DisplayAudits";
						Response.Redirect("/EHS/EHS_Assessments.aspx"); // assume you really shouldn't be here
					}
				}
				else
				{
					SessionManager.ReturnStatus = false;
					SessionManager.ReturnObject = "DisplayAudits";
					Response.Redirect("/EHS/EHS_Assessments.aspx"); // assume you really shouldn't be here
				}
			}

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			//bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.audit);
			//rbNew.Visible = createAuditAccess;

			if (!Page.IsPostBack)
			{
				Ucl_DocMgr ucl = (Ucl_DocMgr)this.Master.FindControl("uclDocSelect");
				if (ucl != null)
				{
					ucl.BindDocumentSelect("EHS", 2, true, true, "");
				}
			}
		}

		protected void UpdateDisplayState(DisplayState state)
		{
			bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.audit);
			string strConfirmText = "return confirm('" + Resources.LocalizedText.AssessmentReturn.ToString() + "')";
			switch (state)
			{
				case DisplayState.AuditNotificationNew:
					lnkReturn.OnClientClick = strConfirmText;
					divAuditDetails.Visible = true;
					CurrentStep = 0;
					EditAuditId = 0;
					IsEditContext = true;
					LoadHeaderInformation();
					//BuildForm();
					break;

				case DisplayState.AuditNotificationEdit:
					lnkReturn.OnClientClick = strConfirmText;
					divAuditDetails.Visible = true;
					CurrentStep = 1;
					IsEditContext = true;
					LoadHeaderInformation();
					BuildForm();
					break;

				case DisplayState.AuditNotificationDisplay:
				case DisplayState.AuditNotificationClosed:
					lnkReturn.OnClientClick = "";
					divAuditDetails.Visible = true;
					CurrentStep = 2;
					IsEditContext = false;
					LoadHeaderInformation();
					BuildForm();
					break;

			}

		}


		#region click events

		protected void rbNew_Click(object sender, EventArgs e)
		{
			UpdateDisplayState(DisplayState.AuditNotificationNew);
		}

		protected void lnkAddTask_Click(Object sender, EventArgs e)
		{
			int recordType = (int)TaskRecordType.Audit;
			LinkButton lnk = (LinkButton)sender;
			string[] cmd = lnk.CommandArgument.Split(',');
			EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), Convert.ToDecimal(cmd[0].ToString()));
			//uclTaskList.TaskWindow(recordType, auditQuestion.AuditId, auditQuestion.QuestionId, "350", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID);
			uclTask.BindTaskAdd(recordType, auditQuestion.AuditId, auditQuestion.QuestionId, "350", "T", auditQuestion.QuestionText, (decimal)audit.DETECT_PLANT_ID, "");
			string script = "function f(){OpenTaskWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void lnkAddAttach(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] cmd = lnk.CommandArgument.Split(',');
			EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), Convert.ToDecimal(cmd[0].ToString()));
			int recordType = (int)TaskRecordType.Audit;
			// before we call the radWindow, we need to update the page?
			hdnAttachClick.Value = lnk.CommandArgument;
			uclAttachWin.OpenManageAttachmentsWindow(recordType, audit.AUDIT_ID, auditQuestion.QuestionId.ToString(), "Upload Attachments", "Upload or view files associated with this assessment question");
		}

		private void OnAttachmentsUpdate(string cmd)
		{
			// we want to be able to update the attachment count on the specific button
			LinkButton lnk;
			Repeater rptQuestions;
			HiddenField hdn;
			decimal quesionId;
			string[] args = hdnAttachClick.Value.ToString().Split(',');
			decimal recordID;
			decimal recordSubID;

			try
			{
				recordID = Convert.ToDecimal(args[0].ToString());
				recordSubID = Convert.ToDecimal(args[1].ToString());
				if (recordSubID > 0)
				{
					foreach (RepeaterItem riTopic in rptAuditFormTopics.Items)
					{
						rptQuestions = (Repeater)riTopic.FindControl("rptAuditFormQuestions");
						foreach (RepeaterItem riQuestion in rptQuestions.Items)
						{
							hdn = (HiddenField)riQuestion.FindControl("hdnQuestionId");
							args = hdn.Value.ToString().Split(',');
							try
							{
								quesionId = Convert.ToDecimal(args[1].ToString());
							}
							catch
							{
								quesionId = 0;
							}
							if (quesionId == recordSubID)
							{
								try
								{
									EHSAuditQuestion q = EHSAuditMgr.SelectAuditQuestion(recordID, recordSubID);
									lnk = (LinkButton)riQuestion.FindControl("LnkAttachment");
									string buttonText = Resources.LocalizedText.Attachments + "(" + q.FilesAttached.ToString() + ")";
									lnk.Text = buttonText;
									lnk.Focus();
								}
								catch
								{
								}
							}
						}
					}
				}
				else
				{
					int attachCount = SQM.Website.Classes.SQMDocumentMgr.GetAttachmentCountByRecord(50, recordID, "0", "");
					LnkAuditAttachment.Text = attachCount == 0 ? Resources.LocalizedText.Attachments : (Resources.LocalizedText.Attachments + " (" + attachCount.ToString() + ")");
				}
			}
			catch { }

		}

		protected void lnkAddVideo(Object sender, EventArgs e)
		{
			LinkButton lnk = (LinkButton)sender;
			string[] cmd = lnk.CommandArgument.Split(',');
			EHSAuditQuestion auditQuestion = EHSAuditMgr.SelectAuditQuestion(Convert.ToDecimal(cmd[0].ToString()), Convert.ToDecimal(cmd[1].ToString()));
			AUDIT audit = EHSAuditMgr.SelectAuditById(new PSsqmEntities(), Convert.ToDecimal(cmd[0].ToString()));
			int recordType = (int)TaskRecordType.Audit;
			// before we call the radWindow, we need to update the page?
			hdnVideoClick.Value = lnk.CommandArgument;

			if (lnk.ID == "LnkVideosAlt")
			{
				hfVideoOption.Value = lnk.ID;
				uclVideoUpload.OpenManageVideosWindow(recordType, audit.AUDIT_ID, auditQuestion.QuestionId.ToString(), (decimal)audit.DETECT_PLANT_ID, "Upload Video", "Upload or view videos associated with this assessment question", "", "", "", PageUseMode.EditEnabled, true);
				string script = "function f(){OpenVideoUploadWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
			}
			else
			{
				uclAttachVideoWin.OpenManageVideosWindow(recordType, audit.AUDIT_ID, auditQuestion.QuestionId.ToString(), "Upload Videos", "Upload or view videos associated with this assessment question", "", "", "", (decimal)audit.DETECT_PLANT_ID);
			}
		}

		private void OnVideoUpdate(string cmd)
		{
			// we want to be able to update the attachment count on the specific button
			LinkButton lnk;
			Repeater rptQuestions;
			HiddenField hdn;
			decimal quesionId;
			string[] args = hdnVideoClick.Value.ToString().Split(',');
			decimal recordID;
			decimal recordSubID;

			if (hfVideoOption.Value == "LnkVideosAlt")
			{
				string script = "function f(){CloseVideoUploadWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);

				if (cmd != "save")
					return;
			}

			try
			{
				recordID = Convert.ToDecimal(args[0].ToString());
				recordSubID = Convert.ToDecimal(args[1].ToString());
				if (recordSubID > 0)
				{
					foreach (RepeaterItem riTopic in rptAuditFormTopics.Items)
					{
						rptQuestions = (Repeater)riTopic.FindControl("rptAuditFormQuestions");
						foreach (RepeaterItem riQuestion in rptQuestions.Items)
						{
							hdn = (HiddenField)riQuestion.FindControl("hdnQuestionId");
							args = hdn.Value.ToString().Split(',');
							try
							{
								quesionId = Convert.ToDecimal(args[1].ToString());
							}
							catch
							{
								quesionId = 0;
							}
							if (quesionId == recordSubID)
							{
								try
								{
									EHSAuditQuestion q = EHSAuditMgr.SelectAuditQuestion(recordID, recordSubID);
									lnk = (LinkButton)riQuestion.FindControl("LnkVideos");
									string buttonText = Resources.LocalizedText.Videos + "(" + q.VideosAttached.ToString() + ")";
									lnk.Text = buttonText;
									lnk.Focus();
								}
								catch
								{
								}
							}
						}
					}
				}
				else
				{
					int attachCount = SQM.Website.Classes.SQMDocumentMgr.GetVideoCountByRecord(50, recordID, "0", "");
					LnkAuditVideo.Text = attachCount == 0 ? Resources.LocalizedText.Videos : (Resources.LocalizedText.Videos + " (" + attachCount.ToString() + ")");
				}
			}
			catch { }

		}

		private void UpdateTaskList(string cmd, decimal recordID, decimal recordSubID)
		{
			// we want to be able to update the attachment count on the specific button
			LinkButton lnk;
			Repeater rptQuestions;
			HiddenField hdn;
			string[] args;
			decimal quesionId;

			foreach (RepeaterItem riTopic in rptAuditFormTopics.Items)
			{
				rptQuestions = (Repeater)riTopic.FindControl("rptAuditFormQuestions");
				foreach (RepeaterItem riQuestion in rptQuestions.Items)
				{
					hdn = (HiddenField)riQuestion.FindControl("hdnQuestionId");
					args = hdn.Value.ToString().Split(',');
					try
					{
						quesionId = Convert.ToDecimal(args[1].ToString());
					}
					catch
					{
						quesionId = 0;
					}
					if (quesionId == recordSubID)
					{
						try
						{
							EHSAuditQuestion q = EHSAuditMgr.SelectAuditQuestion(recordID, recordSubID);
							lnk = (LinkButton)riQuestion.FindControl("lnkAddTask");
							string buttonText = Resources.LocalizedText.AssignTask + "(" + q.TasksAssigned.ToString() + ")";
							lnk.Text = buttonText;
							lnk.Focus();
						}
						catch
						{
						}
					}
				}
			}

		}

		#endregion

		#region AuditDetails

		protected void UpdateAuditTypes()
		{
			if (rddlAuditType.Items.Count == 0)
			{
				var auditTypeList = new List<AUDIT_TYPE>();
				string selectString = "";
				auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId, true);
				selectString = "[Select An Assessment Type]";
				if (auditTypeList.Count > 0)
					auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });

				rddlAuditType.DataSource = auditTypeList;
				rddlAuditType.DataTextField = "TITLE";
				rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
				rddlAuditType.DataBind();
			}
		}

		protected void UpdateDepartments(decimal selectedplantID)
		{
			rddlDepartment.Items.Clear();
			var departmentList = new List<DEPARTMENT>();
			string selectString = "";
			try
			{
				departmentList = SQMModelMgr.SelectDepartmentList(entities, selectedplantID);
			}
			catch
			{ }
			selectString = "[Select A Department]";
			departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = 0, DEPT_NAME = "Plant Wide" });
			departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = -1, DEPT_NAME = selectString });

			rddlDepartment.DataSource = departmentList;
			rddlDepartment.DataTextField = "DEPT_NAME";
			rddlDepartment.DataValueField = "DEPT_ID";
			rddlDepartment.DataBind();
		}

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info
			UpdateAuditTypes();
			string typeString = "";
			hdnAuditId.Value = EditAuditId.ToString();

			if (CurrentStep > 0)
			{
				// in edit mode, load the header field values and make all fields display only
				AUDIT audit = EHSAuditMgr.SelectAuditById(entities, EditAuditId);
				EditAuditTypeId = audit.AUDIT_TYPE_ID;
				BusinessLocation location = new BusinessLocation().Initialize((decimal)audit.DETECT_PLANT_ID);
				rddlAuditType.Enabled = false;
				rddlAuditType.Visible = false;

				lblAuditLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblAuditLocation.Visible = true;
				ddlAuditLocation.Visible = false;
				mnuAuditLocation.Visible = false;

				lblAuditDescription.Text = audit.DESCRIPTION;
				lblAuditDescription.Visible = true;
				tbDescription.Visible = false;

				// build the audit user list
				lblAuditPersonName.Text = EHSAuditMgr.SelectUserNameById((Decimal)audit.AUDIT_PERSON);
				lblAuditPersonName.Visible = true;
				rddlAuditUsers.Visible = false;

				lblAuditDueDate.Text = audit.AUDIT_DT.ToString("MM/dd/yyyy");
				lblAuditDueDate.Visible = true;
				dmAuditDate.Enabled = false;
				dmAuditDate.Visible = false;

				typeString = " Assessment";
				btnSaveReturn.CommandArgument = "0";

				lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

				rddlAuditType.Visible = false;
				lblAuditType.Text = Resources.LocalizedText.AssessmentType + ": ";

				lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
				lblAuditType.Visible = true;
				bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
				btnDelete.Visible = createAuditAccess;
				btnDelete.CommandArgument = EditAuditId.ToString();

				cbClose.Checked = false;
				LnkAuditAttachment.CommandArgument = EditAuditId.ToString() + ",0";
				int attachCount = SQM.Website.Classes.SQMDocumentMgr.GetAttachmentCountByRecord(50, EditAuditId, "0", "");
				LnkAuditAttachment.Text = attachCount == 0 ? Resources.LocalizedText.Attachments : (Resources.LocalizedText.Attachments + " (" + attachCount.ToString() + ")");
				LnkAuditAttachment.Visible = true;

				if (SessionManager.GetUserSetting("MODULE", "MEDIA") != null && SessionManager.GetUserSetting("MODULE", "MEDIA").VALUE.ToUpper() == "A")
				{
					LnkAuditVideo.CommandArgument = EditAuditId.ToString() + ",0";
					attachCount = SQM.Website.Classes.SQMDocumentMgr.GetVideoCountByRecord(50, EditAuditId, "0", "");
					LnkAuditVideo.Text = attachCount == 0 ? Resources.LocalizedText.Videos : (Resources.LocalizedText.Videos + " (" + attachCount.ToString() + ")");
					LnkAuditVideo.Visible = true;
				}
				else
					LnkAuditVideo.Visible = false;

				if (IsEditContext && CurrentStep < 2)
				{
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;
					btnSaveReturn.CommandArgument = "1";
					if (CurrentStep > 0)
						cbClose.Visible = true;
					else
						cbClose.Visible = false;
				}
				else
				{
					btnSaveReturn.Enabled = false;
					btnSaveReturn.Visible = false;
					cbClose.Visible = false;
				}

				if (rddlDepartment.Items.Count == 0)
				{
					UpdateDepartments((decimal)audit.DETECT_PLANT_ID);
					rddlDepartment.SelectedValue = "-1";
				}

				if (audit.DEPT_ID != null)
				{
					//if (rddlDepartment.SelectedIndex == 0)
					try
					{
						rddlDepartment.SelectedValue = audit.DEPT_ID.ToString();
						lblDepartment.Text = rddlDepartment.SelectedText.ToString();
					}
					catch
					{
						rddlDepartment.SelectedValue = "0";
						lblDepartment.Text = rddlDepartment.SelectedText.ToString();
					}
				}

				if (CurrentStep > 1) // display only
				{
					lblDepartment.Visible = true;
					rddlDepartment.Enabled = false;
					rddlDepartment.Visible = false;
					btnDelete.Visible = false;
				}
				else if (SessionManager.UserContext.Person.PERSON_ID == audit.AUDIT_PERSON && !audit.CURRENT_STATUS.Equals("C")) // person resp for audit & not closed
				{
					lblDepartment.Visible = false;
					rddlDepartment.Enabled = true;
					rddlDepartment.Visible = true;
				}
				else // person not responsible for audit or audit closed
				{
					lblDepartment.Visible = true;
					rddlDepartment.Enabled = false;
					rddlDepartment.Visible = false;
					IsEditContext = false; // reset this, just in case we got here in edit mode
					btnDelete.Visible = false;
					lnkReturn.OnClientClick = "";
				}
			}
			else // Step 0 = add mode
			{
				if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.config)
				{
					// List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
					List<BusinessLocation> locationList = SessionManager.PlantList;
					locationList = UserContext.FilterPlantAccessList(locationList);
					if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone,android") == false)
					{
						if (mnuAuditLocation.Items.Count == 0)
						{
							mnuAuditLocation.Items.Clear();

							ddlAuditLocation.Visible = false;
							mnuAuditLocation.Visible = true;
							mnuAuditLocation.Enabled = true;
							SQMBasePage.SetLocationList(mnuAuditLocation, locationList, 0, "[Select a Location]", "", true);
						}
					}
					else
					{
						if (ddlAuditLocation.Items.Count == 0)
						{
							ddlAuditLocation.Items.Clear();
							ddlAuditLocation.Visible = true;
							ddlAuditLocation.Enabled = true;
							mnuAuditLocation.Visible = false;
							SQMBasePage.SetLocationList(ddlAuditLocation, locationList, 0, true);
							//ddlAuditLocation.Items.Insert(0, new RadComboBoxItem("[Select a Location]", ""));
						}
					}
				}
				// set defaults for add mode
				rddlAuditType.Enabled = true;
				rddlAuditType.Visible = true;
				lblAuditLocation.Visible = false;
				lblAuditDescription.Visible = false;
				tbDescription.Visible = true;
				rddlAuditUsers.Enabled = true;
				rddlAuditUsers.Visible = true;
				lblAuditPersonName.Visible = false;
				lblAuditDueDate.Visible = false;
				dmAuditDate.Visible = true;
				dmAuditDate.Enabled = true;
				dmAuditDate.ShowPopupOnFocus = true;
				if (!dmAuditDate.SelectedDate.HasValue)
					dmAuditDate.SelectedDate = SessionManager.UserContext.LocalTime;

				rddlDepartment.Enabled = true;
				rddlDepartment.Visible = true;

				btnSaveReturn.Enabled = true;
				btnSaveReturn.Visible = true;
				btnSaveReturn.CommandArgument = "0";
				lblAddOrEditAudit.Text = "<strong>Add a New Assessment:</strong>";
				lblAuditType.Visible = false;
				btnDelete.Visible = false;
				cbClose.Checked = false;
				cbClose.Visible = false;
				LnkAuditAttachment.Visible = false;
				LnkAuditVideo.Visible = false;
			}
		}

		//protected void rddlAuditType_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	string selectedTypeId = rddlAuditType.SelectedValue;

		//	// in add mode, don't show the questions until the save button is selected
		//	//if (!string.IsNullOrEmpty(selectedTypeId))
		//	//{
		//	//	EditAuditTypeId = Convert.ToDecimal(selectedTypeId);
		//	//	IsEditContext = true;
		//	//	BuildForm();
		//	//	//divAuditDetails.Visible = false;
		//	//}
		//}

		protected void AuditLocation_Select(object sender, EventArgs e)
		{
			string location = "0";
			if (sender is RadMenu)
			{
				location = mnuAuditLocation.SelectedItem.Value;
				mnuAuditLocation.Items[0].Text = mnuAuditLocation.SelectedItem.Text;
			}
			else if (sender is RadComboBox)
			{
				location = ddlAuditLocation.SelectedValue;
			}
			BuildAuditUsersDropdownList(location);
			hdnAuditLocation.Value = location;
			// rebuild the department list
			if (!location.ToLower().Equals("top"))
			{
				UpdateDepartments(Convert.ToDecimal(location));
			}

			// need to rebuild the form - WHY???
			string selectedTypeId = rddlAuditType.SelectedValue;
			//if (!string.IsNullOrEmpty(selectedTypeId))
			//{
			//	SelectedTypeId = Convert.ToDecimal(selectedTypeId);
			//	IsEditContext = false;
			//	BuildForm();
			//}

		}

		void BuildAuditUsersDropdownList(string location)
		{
			if (location != "")
			{
				rddlAuditUsers.Items.Clear();
				rddlAuditUsers.Items.Add(new DropDownListItem("", ""));
				var locationPersonList = new List<PERSON>();
				try
				{
					decimal locationId = Convert.ToDecimal(location);
					if (locationId > 0)
					{
						locationPersonList = SQMModelMgr.SelectPrivGroupPersonList(SysPriv.originate, SysScope.audit, locationId);
						locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
					}
				}
				catch
				{ }
				if (locationPersonList.Count > 0)
				{
					foreach (PERSON p in locationPersonList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						// Check for duplicate list items
						var ddli = rddlAuditUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
						if (ddli == null)
							rddlAuditUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					// If only one user, select by default
					if (rddlAuditUsers.Items.Count() == 2)
						rddlAuditUsers.SelectedIndex = 1;
				}
				else
				{
					rddlAuditUsers.Items[0].Text = "[No valid users - please change location]";
				}
			}
		}

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				//CurrentSubnav = (sender as RadButton).CommandArgument;
				try
				{
					CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
				}
				catch
				{
					CurrentStep = 0;
				}
				if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0) || (rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0) || (rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
				{
					string requiredFields = "";
					if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += Resources.LocalizedText.BusinessLocation;
					}
					if ((rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += "Assessment Person";
					}
					if ((rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
					{
						if (requiredFields.Trim().Length > 0)
							requiredFields += ", ";
						requiredFields += Resources.LocalizedText.Department;
					}
					if (requiredFields.Trim().Length > 0)
						requiredFields = " (" + requiredFields + ")";
					string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg + requiredFields);
					ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				}
				else
					Save(true);
			}
			else
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			try
			{
				EditAuditId = Convert.ToDecimal(btnDelete.CommandArgument.ToString());
			}
			catch
			{
				EditAuditId = 0;
			}
			if (EditAuditId > 0)
			{
				divAuditDetails.Visible = false;

				btnSaveReturn.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSAuditMgr.DeleteAudit(EditAuditId);
				// delete the task
				if (delStatus == 1)
				{
					EHSAuditMgr.DeleteAuditTask(EditAuditId, 50);
				}
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Assessment deleted." : "Error deleting assessment.";
				lblResults.Text += "</div>";
				if (delStatus == 1)
				{
					SessionManager.ReturnStatus = false;
					SessionManager.ReturnObject = "DisplayAudits";
					Response.Redirect("/EHS/EHS_Assessments.aspx");
				}
			}

			rddlAuditType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
			AUDIT theAudit = null;
			decimal auditPerson = 0;
			decimal auditId = 0;
			if (CurrentStep == 0)
			{
				// Add context
				EditAuditTypeId = Convert.ToDecimal(rddlAuditType.SelectedValue.ToString());
				auditTypeId = EditAuditTypeId;
				auditType = rddlAuditType.SelectedText;
				theAudit = CreateNewAudit();
				auditId = theAudit.AUDIT_ID;
				EditAuditId = auditId;
				hdnAuditId.Value = auditId.ToString();
				BuildForm();
			}
			else
			{
				// Edit context
				try
				{
					EditAuditId = Convert.ToDecimal(hdnAuditId.Value.ToString());
				}
				catch
				{
					EditAuditId = 0;
				}
				auditId = EditAuditId;
				if (auditId > 0)
				{
					theAudit = UpdateAudit(auditId);
				}
				auditType = theAudit.AUDIT_TYPE;
				EditAuditTypeId = theAudit.AUDIT_TYPE_ID;
				auditTypeId = EditAuditTypeId;
			}

			try { auditPerson = (decimal)theAudit.AUDIT_PERSON; }
			catch { auditPerson = 0; }

			UpdateAnswersFromForm();

			if (auditId > 0)
			{
				shouldReturn = AddOrUpdateAnswers(questions, auditId);
				if (CurrentStep == 0)
				{
					// notify the user of a new audit
					EHSNotificationMgr.NotifyOnAuditCreate(auditId, (decimal)theAudit.AUDIT_PERSON);
				}
			}


			if (shouldReturn == true)
			{
				SessionManager.ReturnStatus = false;
				SessionManager.ReturnObject = "DisplayAudits";
				Response.Redirect("/EHS/EHS_Assessments.aspx");
			}
			else
			{
				EditAuditId = auditId;
				SessionManager.ReturnRecordID = auditId;
				bool showMsg = true;
				if (CurrentStep == 0)
					showMsg = false;
				// determine if we should be in edit mode or display mode
				if (auditPerson == SessionManager.UserContext.Person.PERSON_ID)
					UpdateDisplayState(DisplayState.AuditNotificationEdit);
				else
					UpdateDisplayState(DisplayState.AuditNotificationDisplay);

				// and send a message that the field need to be entered
				if (showMsg)
				{
					string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsIndicatedMsg);
					ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				}
			}

		}

		protected void UpdateAnswersFromForm()
		{
			// step through the repeater and update the info
			questions = new List<EHSAuditQuestion>();
			EHSAuditQuestion newQuestion;
			Repeater rptQuestions;
			decimal auditId;
			decimal questionId;
			string[] cmd;
			RadioButtonList rbl;
			RadTextBox rtb;
			HiddenField hdn;
			foreach (RepeaterItem riTopic in rptAuditFormTopics.Items)
			{
				rptQuestions = (Repeater)riTopic.FindControl("rptAuditFormQuestions");
				foreach (RepeaterItem riQuestion in rptQuestions.Items)
				{
					hdn = (HiddenField)riQuestion.FindControl("hdnQuestionId");
					cmd = hdn.Value.ToString().Split(',');
					try
					{
						auditId = Convert.ToDecimal(cmd[0].ToString());
						if (auditId == 0)
							auditId = Convert.ToDecimal(hdnAuditId.Value.ToString());
						questionId = Convert.ToDecimal(cmd[1].ToString());
					}
					catch
					{
						questionId = 0;
						auditId = 0;
					}
					if (questionId > 0)
					{
						newQuestion = EHSAuditMgr.SelectAuditQuestion(auditId, questionId);
						if (newQuestion.QuestionType == EHSAuditQuestionType.Radio || newQuestion.QuestionType == EHSAuditQuestionType.RadioCommentLeft
							|| newQuestion.QuestionType == EHSAuditQuestionType.RadioPercentage || newQuestion.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft
							|| newQuestion.QuestionType == EHSAuditQuestionType.RequiredYesNoRadio)
						{
							rbl = (RadioButtonList)riQuestion.FindControl("rblAnswers");
							foreach (ListItem li in rbl.Items)
							{
								if (li.Selected)
								{
									newQuestion.AnswerValue = li.Value;
									newQuestion.AnswerText = li.Text;
								}
							}
							if (newQuestion.QuestionType == EHSAuditQuestionType.RadioCommentLeft || newQuestion.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
							{
								rtb = (RadTextBox)riQuestion.FindControl("rtbCommentLeft");
								newQuestion.AnswerComment = rtb.Text;
							}
							else
							{
								rtb = (RadTextBox)riQuestion.FindControl("rtbCommentRight");
								newQuestion.AnswerComment = rtb.Text;
							}
						}
						questions.Add(newQuestion);
					}
				}
			}
		}

		protected AUDIT CreateNewAudit()
		{
			decimal auditId = 0;
			PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, auditPlant.LOCAL_TIMEZONE);
			var newAudit = new AUDIT()
			{
				DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
				DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
				DETECT_PLANT_ID = auditPlant.PLANT_ID,
				AUDIT_TYPE = "EHS",
				CREATE_DT = localTime,
				CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
				DESCRIPTION = tbDescription.Text.ToString(),
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				AUDIT_DT = (DateTime)dmAuditDate.SelectedDate,
				AUDIT_TYPE_ID = auditTypeId,
				AUDIT_PERSON = Convert.ToDecimal(rddlAuditUsers.SelectedValue),
				DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue)
			};
			entities.AddToAUDIT(newAudit);
			entities.SaveChanges();
			auditId = newAudit.AUDIT_ID;

			// create task record for their calendar
			AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
			EHSAuditMgr.CreateOrUpdateTask(auditId, Convert.ToDecimal(rddlAuditUsers.SelectedValue), 50, ((DateTime)dmAuditDate.SelectedDate).AddDays(audittype.DAYS_TO_COMPLETE), "A", SessionManager.UserContext.Person.PERSON_ID);

			return newAudit;
		}

		protected AUDIT UpdateAudit(decimal auditId)
		{
			// we are not going to let them update any of this info.
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();

			//if (audit != null && rddlDepartment.SelectedIndex > 0)
			//{
			//	audit.DETECT_COMPANY_ID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//	audit.DETECT_BUS_ORG_ID = SessionManager.UserContext.WorkingLocation.BusinessOrg.BUS_ORG_ID;
			//	audit.DETECT_PLANT_ID = selectedPlantId;
			//	audit.AUDIT_TYPE = "EHS";
			//	//audit.CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
			//	audit.DESCRIPTION = auditDescription;
			//	//audit.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
			//	audit.AUDIT_DT = auditDate;
			//	//audit.ISSUE_TYPE = auditType;
			//	audit.AUDIT_TYPE_ID = auditTypeId;

			//	audit.DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue);

			//	entities.SaveChanges();
			//}

			return audit;

		}

		protected void BuildForm()
		{
			// build repeater for the topics
			List<AUDIT_TOPIC> topics = new List<AUDIT_TOPIC>();
			AUDIT_TOPIC topic = new AUDIT_TOPIC();

			AUDIT audit = null;
			audit = (from aud in entities.AUDIT where aud.AUDIT_ID == EditAuditId select aud).FirstOrDefault();

			if (CurrentStep == 0)
			{
				questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, 0, 0);
			}
			else
			{
				questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, 0, EditAuditId);
			}


			previousTopicId = 0;
			foreach (EHSAuditQuestion question in questions)
			{
				if (question.TopicId != previousTopicId)
				{
					topic = new AUDIT_TOPIC();
					topic.AUDIT_TOPIC_ID = question.TopicId;
					topic.TITLE = question.TopicTitle;
					topics.Add(topic);
					previousTopicId = question.TopicId;
				}
			}
			if (topics.Count > 0)
			{
				totalQuestions = 0;
				totalPositive = 0;
				totalPossibleScore = 0;
				totalWeightScore = 0;
				rptAuditFormTopics.DataSource = topics;
				rptAuditFormTopics.DataBind();
				divForm.Visible = true;
				divFormRepeater.Visible = true;
			}
			else
			{
				divForm.Visible = false;
			}
		}

		protected bool AddOrUpdateAnswers(List<EHSAuditQuestion> questions, decimal auditId)
		{
			bool negativeTextComplete = true;
			decimal totalQuestions = 0;
			decimal totalAnswered = 0;
			decimal totalPositive = 0;
			decimal totalPercent = 0;
			decimal totalWeightScore = 0;
			decimal totalPossibleScore = 0;
			decimal possibleScore = 0;
			bool answerIsPositive = false;

			foreach (var q in questions)
			{
				var thisQuestion = q;
				var auditAnswer = (from ia in entities.AUDIT_ANSWER
								   where ia.AUDIT_ID == auditId
									   && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
								   select ia).FirstOrDefault();
				// calculate the total percentages
				totalQuestions += 1;
				if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
				{
					answerIsPositive = false;
					//if (!q.AnswerText.ToString().Equals(""))
					//{
					possibleScore = 0;
					foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
					{
						if (choice.ChoiceWeight > possibleScore)
							possibleScore = choice.ChoiceWeight;
						if (choice.Value.Equals(q.AnswerValue) && choice.ChoicePositive)
							answerIsPositive = true;
						if (choice.Value.Equals(q.AnswerValue))
						{
							totalWeightScore += choice.ChoiceWeight;
							totalAnswered += 1;
						}
					}
					totalPossibleScore += possibleScore;
					if (answerIsPositive)
					{
						q.ChoicePositive = true;
						totalPositive += 1;
					}
					else
					{
						if (q.AnswerValue != null && !q.AnswerValue.ToString().Equals(""))
						{
							q.ChoicePositive = false;
							if (q.AnswerComment.ToString().Trim().Length == 0)
								negativeTextComplete = false;
						}
					}
					//}
				}
				else if (q.QuestionType == EHSAuditQuestionType.Radio || q.QuestionType == EHSAuditQuestionType.RadioCommentLeft)
				{
					answerIsPositive = false;
					foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
					{
						if (choice.Value.Equals(q.AnswerValue) && choice.ChoicePositive)
							answerIsPositive = true;
						if (choice.Value.Equals(q.AnswerValue))
						{
							totalAnswered += 1;
						}
					}
					if (answerIsPositive)
					{
						totalPositive += 1;
						q.ChoicePositive = true;
					}
					else
					{
						if (q.AnswerValue != null && !q.AnswerValue.ToString().Equals(""))
						{
							q.ChoicePositive = false;
							if (q.AnswerComment.ToString().Trim().Length == 0)
								negativeTextComplete = false;
						}
					}
				}
				else if (q.AnswerValue != null && !q.AnswerValue.ToString().Equals(""))
				{
					totalAnswered += 1;
				}

				if (q.CommentRequired && q.AnswerComment.ToString().Trim().Length == 0)
					negativeTextComplete = false;
				if (auditAnswer != null)
				{
					auditAnswer.ANSWER_VALUE = q.AnswerValue;
					//auditAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText; // don't want to update text after the audit has been created
					auditAnswer.COMMENT = q.AnswerComment;
					auditAnswer.CHOICE_POSITIVE = q.ChoicePositive;
					if (q.ChoicePositive)
						auditAnswer.STATUS = "03";
					else
						auditAnswer.STATUS = "01";
				}
				else
				{
					auditAnswer = new AUDIT_ANSWER()
					{
						AUDIT_ID = auditId,
						AUDIT_QUESTION_ID = q.QuestionId,
						ANSWER_VALUE = q.AnswerText,
						ORIGINAL_QUESTION_TEXT = q.QuestionText,
						COMMENT = q.AnswerComment,
						CHOICE_POSITIVE = q.ChoicePositive
					};

					if (q.ChoicePositive)
						auditAnswer.STATUS = "03";
					else
						auditAnswer.STATUS = "01";

					entities.AddToAUDIT_ANSWER(auditAnswer);
				}
			}
			// now update the header info
			AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			if (totalQuestions > 0)
				totalPercent = Math.Round((totalAnswered / totalQuestions), 2) * 100;
			else
				totalPercent = 0;
			audit.PERCENT_COMPLETE = totalPercent;
			if (totalPercent >= 100 && negativeTextComplete && cbClose.Checked)
			{
				audit.CURRENT_STATUS = "C";
				if (!audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
				{
					PLANT plant = SQMModelMgr.LookupPlant((decimal)audit.DETECT_PLANT_ID);
					DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, plant.LOCAL_TIMEZONE);
					audit.CLOSE_DATE_DATA_COMPLETE = localTime;
					audit.CLOSE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				}
			}
			else if (totalPercent > 0)
			{
				audit.CURRENT_STATUS = "I";
			}
			else
				audit.CURRENT_STATUS = "A";

			//if (totalQuestions > 0)
			//	totalPercent = Math.Round((totalPositive / totalQuestions), 2) * 100;
			//else
			//	totalPercent = 0;
			if (totalPossibleScore > 0)
				totalPercent = Math.Round((totalWeightScore / totalPossibleScore), 2) * 100;
			else
				totalPercent = 0;
			audit.TOTAL_SCORE = totalPercent;

			if (rddlDepartment.SelectedIndex > 0)
				audit.DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue);

			// save all the changes
			entities.SaveChanges();

			if (audit.CURRENT_STATUS.Equals("C"))
			{
				AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
				EHSAuditMgr.CreateOrUpdateTask(auditId, SessionManager.UserContext.Person.PERSON_ID, 50, audit.AUDIT_DT.AddDays(audittype.DAYS_TO_COMPLETE), "C", SessionManager.UserContext.Person.PERSON_ID);
			}

			if (CurrentStep == 0)
			{
				// always stay in on the page in add mode, so they can see the questions
				negativeTextComplete = false;
			}
			return negativeTextComplete;
		}

		public void rptAuditFormTopics_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					AUDIT_TOPIC topic = (AUDIT_TOPIC)e.Item.DataItem;
					if (CurrentStep == 0)
					{
						questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, topic.AUDIT_TOPIC_ID, 0);
					}
					else
					{
						questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, topic.AUDIT_TOPIC_ID, EditAuditId);
					}

					totalTopicQuestions = 0;
					totalTopicPositive = 0;
					totalTopicWeightScore = 0;
					totalTopicPossibleScore = 0;
					Repeater rpt = (Repeater)e.Item.FindControl("rptAuditFormQuestions");
					rpt.DataSource = questions;
					rpt.DataBind();

					Label lbl = (Label)e.Item.FindControl("lblTopicTotal");
					if (totalTopicQuestions > 0)
					{
						lbl.Visible = true;
						if (totalTopicPossibleScore > 0)
							totalPercent = totalTopicWeightScore / totalTopicPossibleScore;
						else
							totalPercent = 0;
						lbl.Text = string.Format("{0:0%}", totalPercent);
					}
					else
						lbl.Visible = false;

					//RadAjaxManager1.AjaxSettings.AddAjaxSetting(rpt, RadAjaxPanel1);
					// after repeater is bound, need to determine if the topic total row needs to be displayed or hidden
				}
				catch
				{
				}
			}
			if (e.Item.ItemType == ListItemType.Footer)
			{
				Label lbl = (Label)e.Item.FindControl("lblTotalPossiblePoints");
				if (lbl != null)
				{
					lbl.Text = string.Format("Total Possible Points:   {0:0}", totalPossibleScore);
				}
				lbl = (Label)e.Item.FindControl("lblTotalPointsAchieved");
				if (lbl != null)
				{
					lbl.Text = string.Format("Total Points Achieved:   {0:0}", totalWeightScore);
				}
				lbl = (Label)e.Item.FindControl("lblTotalPointsPercentage");
				if (lbl != null)
				{
					if (totalPossibleScore > 0)
						totalPercent = totalWeightScore / totalPossibleScore;
					else
						totalPercent = 0;
					lbl.Text = string.Format("Percentage of Points Achieved:   {0:0%}", totalPercent);
				}
			}
		}

		public void rptAuditFormQuestions_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					EHSAuditQuestion q = (EHSAuditQuestion)e.Item.DataItem;

					// populate the number of tasks/attachments
					LinkButton lnk = (LinkButton)e.Item.FindControl("lnkAddTask");
					string buttonText = Resources.LocalizedText.AssignTask + "(" + q.TasksAssigned.ToString() + ")";
					lnk.Text = buttonText;
					lnk = (LinkButton)e.Item.FindControl("LnkAttachment");
					buttonText = Resources.LocalizedText.Attachments + "(" + q.FilesAttached.ToString() + ")";
					lnk.Text = buttonText;
					lnk = (LinkButton)e.Item.FindControl("LnkVideos");
					if (SessionManager.GetUserSetting("MODULE", "MEDIA") != null && SessionManager.GetUserSetting("MODULE", "MEDIA").VALUE.ToUpper() == "A")
					{
						buttonText = Resources.LocalizedText.Videos + "(" + q.VideosAttached.ToString() + ")";
						lnk.Text = buttonText;
						lnk.Visible = true;
					}
					else
					{
						lnk.Visible = false;
					}

					var validator = new RequiredFieldValidator();
					//bool shouldPopulate = ((IsEditContext && !string.IsNullOrEmpty(q.AnswerValue)) || !IsEditContext);
					bool shouldPopulate = (!string.IsNullOrEmpty(q.AnswerValue));

					bool answerIsPositive = true;
					bool questionAnswered = false;
					RadTextBox rtbActive;
					RadTextBox rtbInactive;
					Label lblActive;
					Label lblInactive;
					HtmlTableCell td;
					Literal lit;
					HiddenField hdn;
					Label lbl;

					// populate the possible answers to radiobuttonlist
					RadioButtonList rbl = (RadioButtonList)e.Item.FindControl("rblAnswers");
					answerIsPositive = false;
					possibleScore = 0;
					foreach (var choice in q.AnswerChoices)
					{
						var li = new ListItem(choice.Text, choice.Value);
						// Don't try to explicitly set SelectedValue in case answer choice text changed in database
						if (shouldPopulate)
						{
							if (choice.Value == q.AnswerValue)
								li.Selected = true;
						}
						rbl.Items.Add(li);
						if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
						{
							if (choice.ChoiceWeight > possibleScore)
								possibleScore = choice.ChoiceWeight;
							if (choice.Value == q.AnswerValue)
							{
								if (choice.ChoicePositive)
									answerIsPositive = true;
								totalWeightScore += choice.ChoiceWeight;
								totalTopicWeightScore += choice.ChoiceWeight;
							}
						}
					}


					if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
					{
						totalQuestions += 1;
						totalTopicQuestions += 1;
						totalPossibleScore += possibleScore;
						totalTopicPossibleScore += possibleScore;
						if (answerIsPositive)
						{
							totalPositive += 1;
							totalTopicPositive += 1;
						}
					}

					// populate  & format comment texts
					if (q.QuestionType == EHSAuditQuestionType.RadioCommentLeft || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
					{
						rtbActive = (RadTextBox)e.Item.FindControl("rtbCommentLeft");
						//lblActive = (Label)e.Item.FindControl("lblCommentLeft");
						td = (HtmlTableCell)e.Item.FindControl("tdCommentRight");
						rtbInactive = (RadTextBox)e.Item.FindControl("rtbCommentRight");
						//lblInactive = (Label)e.Item.FindControl("lblCommentRight");
					}
					else
					{
						rtbActive = (RadTextBox)e.Item.FindControl("rtbCommentRight");
						//lblActive = (Label)e.Item.FindControl("lblCommentRight");
						td = (HtmlTableCell)e.Item.FindControl("tdCommentLeft");
						rtbInactive = (RadTextBox)e.Item.FindControl("rtbCommentLeft");
						//lblInactive = (Label)e.Item.FindControl("lblCommentLeft");
					}

					rtbInactive.Enabled = false;
					rtbInactive.Visible = false;
					//lblInactive.Visible = false;
					td.Visible = false;
					rtbActive.Enabled = true;
					rtbActive.Visible = true;
					//lblActive.Visible = false;
					rtbActive.MaxLength = MaxTextLength;
					rtbActive.Text = q.AnswerComment;
					//lblActive.Text = q.AnswerComment;

					answerIsPositive = true;
					if (q.AnswerValue != null && !q.AnswerValue.ToString().Equals(""))
					{
						foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
						{
							if (choice.Value.Equals(q.AnswerValue) && !choice.ChoicePositive)
								answerIsPositive = false;
							if (choice.Value.Equals(q.AnswerValue))
								questionAnswered = true;
						}
					}
					if (((q.CommentRequired && questionAnswered) || !answerIsPositive) && (q.AnswerComment == null || q.AnswerComment.Trim().Length == 0))
					{
						rtbActive.CssClass = "audittextrequired";
					}
					else
					{
						rtbActive.CssClass = "WarnIfChanged";
						rtbActive.Skin = "Metro";
					}

					// populate tooltips
					RadToolTip rtt = (RadToolTip)e.Item.FindControl("rttToolTip");
					Image img = (Image)e.Item.FindControl("imgHelp");
					if (!string.IsNullOrEmpty(q.HelpText))
					{
						img.Visible = true;
						rtt.TargetControlID = img.ID;
						rtt.Text = "<div style=\"font-size: 11px; line-height: 1.5em;\" data-html=\"true\" >" + q.HelpText + "</div>";

					}
					else
					{
						img.Visible = false;
					}

					// populate required indicators
					lit = (Literal)e.Item.FindControl("litRequiredStar");
					if (q.IsRequired)
						lit.Text = "<span class=\"requiredStar\">&bull;</span>";
					if (q.IsRequiredClose)
						lit.Text += "<span class=\"requiredCloseStar\">&bull;</span>";

					if (IsEditContext)
					{
						rtbActive.Enabled = true;
						rtbActive.Visible = true;
						rbl.Enabled = true;
						//lblActive.Visible = false;
					}
					else
					{
						rtbActive.Visible = true;
						rtbActive.Enabled = false;
						rtbActive.Rows = 6;
						//lblActive.Visible = true;
						rbl.Enabled = false;
					}

				}
				catch (Exception ex)
				{
					;
				}
			}
		}

		#endregion

		//// manage current session object  (formerly was page static variable)
		//SQMMetricMgr HSCalcs()
		//{
		//	if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is SQMMetricMgr)
		//		return (SQMMetricMgr)SessionManager.CurrentObject;
		//	else
		//		return null;
		//}
		//SQMMetricMgr SetHSCalcs(SQMMetricMgr hscalcs)
		//{
		//	SessionManager.CurrentObject = hscalcs;
		//	return HSCalcs();
		//}

		protected string linkArgs(object auditID, object questionID)
		{
			string args = auditID.ToString().Trim() + "," + questionID.ToString().Trim();
			return args;
		}

		protected void lnkReturn_Click(object sender, EventArgs e)
		{
			SessionManager.ReturnStatus = false;
			SessionManager.ReturnObject = "DisplayAudits";
			Response.Redirect("/EHS/EHS_Assessments.aspx");
		}
	}


}
