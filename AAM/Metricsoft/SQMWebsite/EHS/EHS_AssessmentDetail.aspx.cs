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

namespace SQM.Website.EHS
{
	public partial class EHS_AssessmentDetail : System.Web.UI.Page
	{
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


		protected bool IsEditContext;
		protected int CurrentStep;
		protected decimal EditAuditId;
		protected decimal EditAuditTypeId;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

		}

		protected void Page_Init(object sender, EventArgs e)
		{
			//uclAuditForm.OnAttachmentListItemClick += OpenFileUpload;
			//uclAuditForm.OnExceptionListItemClick += AddTask;
			uclAttachWin.AttachmentEvent += OnAttachmentsUpdate;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			entities = new PSsqmEntities();
			if (!Page.IsPostBack)
			{
				this.Title = Resources.LocalizedText.EHSAudits;
				companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
				EditAuditId = 4;
				EditAuditTypeId = 1;
				UpdateDisplayState(DisplayState.AuditNotificationEdit);
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

				//	if (SessionManager.ReturnStatus == true && SessionManager.ReturnObject is string)
				//	{
				//		try
				//		{
				//			// from inbox
				//			DisplayNonPostback();
				//			SessionManager.ReturnRecordID = Convert.ToDecimal(SessionManager.ReturnObject.ToString());
				//			SessionManager.ReturnObject = "Notification";
				//			SessionManager.ReturnStatus = true;
				//			isDirected = true;

				//			StringBuilder sbScript = new StringBuilder();
				//			ClientScriptManager cs = Page.ClientScript;

				//			sbScript.Append("<script language='JavaScript' type='text/javascript'>\n");
				//			sbScript.Append("<!--\n");
				//			sbScript.Append(cs.GetPostBackEventReference(this, "PBArg") + ";\n");
				//			sbScript.Append("// -->\n");
				//			sbScript.Append("</script>\n");

				//			cs.RegisterStartupScript(this.GetType(), "AutoPostBackScript", sbScript.ToString());
				//		}
				//		catch
				//		{
				//			// not a number, parse as type
				//			DisplayNonPostback();
				//		}
				//	}
				//	else
				//	{
				//		DisplayNonPostback();
				//	}
			}
			//else
			//{
			//	if (SessionManager.ReturnStatus == true)
			//	{
			//		if (SessionManager.ReturnObject is string)
			//		{
			//			string type = SessionManager.ReturnObject as string;
			//			switch (type)
			//			{
			//				case "DisplayAudits":
			//					UpdateDisplayState(DisplayState.AuditList);
			//					break;

			//				case "Notification":
			//					//uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
			//					// need to determine if the Audit is past due and force it into display mode (probelm when coming from Calendar)
			//					string auditStatus = EHSAuditMgr.SelectAuditStatus(SessionManager.ReturnRecordID);
			//					if (auditStatus == "C")
			//						UpdateDisplayState(DisplayState.AuditNotificationDisplay);
			//					else
			//						UpdateDisplayState(DisplayState.AuditNotificationEdit);
			//					//if (isDirected)
			//					//{
			//					//	rbNew.Visible = false;
			//					//	uclAuditForm.EnableReturnButton(false);
			//					//}
			//					break;

			//				case "Closed":
			//					// display only mode
			//					//uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
			//					UpdateDisplayState(DisplayState.AuditNotificationClosed);
			//					//if (isDirected)
			//					//{
			//					//	rbNew.Visible = false;
			//					//	uclAuditForm.EnableReturnButton(false);
			//					//}
			//					break;
			//				case "DisplayOnly":
			//					//uclAuditForm.EditAuditId = SessionManager.ReturnRecordID;
			//					UpdateDisplayState(DisplayState.AuditNotificationDisplay);
			//					//if (isDirected)
			//					//{
			//					//	rbNew.Visible = false;
			//					//	uclAuditForm.EnableReturnButton(false);
			//					//}
			//					break;
			//			}
			//		}
			//		SessionManager.ClearReturns();
			//	}
			//}
		}

		protected void UpdateDisplayState(DisplayState state)
		{
			bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.config, SysScope.audit);

			switch (state)
			{
				case DisplayState.AuditNotificationNew:
					divAuditDetails.Visible = true;
					CurrentStep = 0;
					//EditAuditId = 0;
					IsEditContext = false;
					LoadHeaderInformation();
					BuildForm();
					//uclAuditForm.Visible = true;
					//uclAuditForm.IsEditContext = false;
					//uclAuditForm.ClearControls();
					//uclAuditForm.CheckForSingleType();
					break;

				case DisplayState.AuditNotificationEdit:
					divAuditDetails.Visible = true;
					CurrentStep = 1;
					//EditAuditId = SessionManager.ReturnRecordID;
					IsEditContext = true;
					LoadHeaderInformation();
					BuildForm();
					//uclAuditForm.CurrentStep = 0;
					//uclAuditForm.IsEditContext = true;
					//uclAuditForm.Visible = true;
					//uclAuditForm.BuildForm();
					break;

				case DisplayState.AuditNotificationDisplay:
				case DisplayState.AuditNotificationClosed:
					divAuditDetails.Visible = true;
					CurrentStep = 2;
					IsEditContext = true;
					//EditAuditId = SessionManager.ReturnRecordID;
					LoadHeaderInformation();
					BuildForm();
					//uclAuditForm.CurrentStep = 1;
					//uclAuditForm.IsEditContext = false;
					//uclAuditForm.Visible = true;
					break;

			}

			// for now, we will only let 'admin' create audits
			//if (!SessionManager.UserContext.Person.SSO_ID.ToLower().Equals("admin"))
			//	rbNew.Visible = false;

			//SessionManager.ClearReturns();
		}


		#region click events

		protected void rbNew_Click(object sender, EventArgs e)
		{
			UpdateDisplayState(DisplayState.AuditNotificationNew);
		}

		protected void btnAuditsSearchClick(object sender, EventArgs e)
		{
			//SessionManager.ReturnStatus = true;
			//SessionManager.ReturnObject = "DisplayAudits";
			UpdateDisplayState(DisplayState.AuditList);
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
			uclAttachWin.OpenManageAttachmentsWindow(recordType, audit.AUDIT_ID, auditQuestion.QuestionId.ToString(), "Upload Attachments", "Upload or view files associated with this assessment question");
		}

		private void OnAttachmentsUpdate(string cmd)
		{
			// not sure if we need to do anything here...
		}
		#endregion

		#region AuditDetails

		protected void UpdateAuditTypes()
		{
			//if (rddlAuditType.Items.Count == 0)
			//{
			//	var auditTypeList = new List<AUDIT_TYPE>();
			//	string selectString = "";
			//	auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId, true);
			//	selectString = "[Select An Assessment Type]";
			//	if (auditTypeList.Count > 0)
			//		auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });

			//	rddlAuditType.DataSource = auditTypeList;
			//	rddlAuditType.DataTextField = "TITLE";
			//	rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
			//	rddlAuditType.DataBind();
			//}
		}

		protected void UpdateDepartments(decimal selectedplantID)
		{
			//rddlDepartment.Items.Clear();
			//var departmentList = new List<DEPARTMENT>();
			//string selectString = "";
			//try
			//{
			//	departmentList = SQMModelMgr.SelectDepartmentList(entities, selectedplantID);
			//}
			//catch
			//{ }
			//selectString = "[Select A Department]";
			//departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = 0, DEPT_NAME = "Plant Wide" });
			//departmentList.Insert(0, new DEPARTMENT() { DEPT_ID = -1, DEPT_NAME = selectString });

			//rddlDepartment.DataSource = departmentList;
			//rddlDepartment.DataTextField = "DEPT_NAME";
			//rddlDepartment.DataValueField = "DEPT_ID";
			//rddlDepartment.DataBind();
		}

		protected void LoadHeaderInformation()
		{
			// set up for adding the header info
			UpdateAuditTypes();
			string typeString = "";

			//if (CurrentStep > 0)
			//{
			//	// in edit mode, load the header field values and make all fields display only
			//	AUDIT audit = EHSAuditMgr.SelectAuditById(entities, EditAuditId);
			//	BusinessLocation location = new BusinessLocation().Initialize((decimal)audit.DETECT_PLANT_ID);
			//	rddlAuditType.Enabled = false;
			//	rddlAuditType.Visible = false;

			//	lblAuditLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
			//	lblAuditLocation.Visible = true;
			//	ddlAuditLocation.Visible = false;
			//	mnuAuditLocation.Visible = false;

			//	lblAuditDescription.Text = audit.DESCRIPTION;
			//	lblAuditDescription.Visible = true;
			//	tbDescription.Visible = false;

			//	// build the audit user list
			//	lblAuditPersonName.Text = EHSAuditMgr.SelectUserNameById((Decimal)audit.AUDIT_PERSON);
			//	lblAuditPersonName.Visible = true;
			//	rddlAuditUsers.Visible = false;

			//	lblAuditDueDate.Text = audit.AUDIT_DT.ToString("MM/dd/yyyy");
			//	lblAuditDueDate.Visible = true;
			//	dmAuditDate.Enabled = false;
			//	dmAuditDate.Visible = false;


			//	typeString = " Assessment";
			//	btnSaveReturn.CommandArgument = "0";

			//	lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditId, 6) + typeString + "</strong><br/>";

			//	rddlAuditType.Visible = false;
			//	lblAuditType.Text = Resources.LocalizedText.AssessmentType + ": ";

			//	lblAuditType.Text += EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
			//	lblAuditType.Visible = true;
			//	bool createAuditAccess = SessionManager.CheckUserPrivilege(SysPriv.admin, SysScope.audit);
			//	btnDelete.Visible = createAuditAccess;
			//	if (IsEditContext && CurrentStep < 2)
			//	{
			//		btnSaveReturn.Enabled = true;
			//		btnSaveReturn.Visible = true;
			//	}
			//	else
			//	{
			//		btnSaveReturn.Enabled = false;
			//		btnSaveReturn.Visible = false;
			//	}

			//	if (rddlDepartment.Items.Count == 0)
			//	{
			//		UpdateDepartments((decimal)audit.DETECT_PLANT_ID);
			//		rddlDepartment.SelectedValue = "-1";
			//	}

			//	if (audit.DEPT_ID != null && rddlDepartment.SelectedIndex == 0)
			//	{
			//		rddlDepartment.SelectedValue = audit.DEPT_ID.ToString();
			//		lblDepartment.Text = rddlDepartment.SelectedText.ToString();
			//	}

			//	if (CurrentStep > 0)
			//	{
			//		lblDepartment.Visible = true;
			//		rddlDepartment.Enabled = false;
			//		rddlDepartment.Visible = false;
			//	}
			//	else if (SessionManager.UserContext.Person.PERSON_ID == audit.AUDIT_PERSON && !audit.CURRENT_STATUS.Equals("C"))
			//	{
			//		lblDepartment.Visible = false;
			//		rddlDepartment.Enabled = true;
			//		rddlDepartment.Visible = true;
			//	}
			//	else
			//	{
			//		lblDepartment.Visible = true;
			//		rddlDepartment.Enabled = false;
			//		rddlDepartment.Visible = false;
			//	}
			//}
			//else
			//{
			//	if (UserContext.GetMaxScopePrivilege(SysScope.audit) <= SysPriv.config)
			//	{
			//		// List<BusinessLocation> locationList = SQMModelMgr.SelectBusinessLocationList(SessionManager.PrimaryCompany().COMPANY_ID, 0, true);
			//		List<BusinessLocation> locationList = SessionManager.PlantList;
			//		locationList = UserContext.FilterPlantAccessList(locationList);
			//		if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone") == false)
			//		{
			//			if (mnuAuditLocation.Items.Count == 0)
			//			{
			//				mnuAuditLocation.Items.Clear();

			//				ddlAuditLocation.Visible = false;
			//				mnuAuditLocation.Visible = true;
			//				mnuAuditLocation.Enabled = true;
			//				SQMBasePage.SetLocationList(mnuAuditLocation, locationList, 0, "Select a Location", "", true);
			//			}
			//		}
			//		else
			//		{
			//			if (ddlAuditLocation.Items.Count == 0)
			//			{
			//				ddlAuditLocation.Items.Clear();
			//				ddlAuditLocation.Visible = true;
			//				ddlAuditLocation.Enabled = true;
			//				mnuAuditLocation.Visible = false;
			//				SQMBasePage.SetLocationList(ddlAuditLocation, locationList, 0, true);
			//				ddlAuditLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
			//			}
			//		}
			//	}
			//	// set defaults for add mode
			//	rddlAuditType.Enabled = true;
			//	rddlAuditType.Visible = true;
			//	lblAuditLocation.Visible = false;
			//	lblAuditDescription.Visible = false;
			//	tbDescription.Visible = true;
			//	rddlAuditUsers.Enabled = true;
			//	rddlAuditUsers.Visible = true;
			//	lblAuditPersonName.Visible = false;
			//	lblAuditDueDate.Visible = false;
			//	dmAuditDate.Visible = true;
			//	dmAuditDate.Enabled = true;
			//	dmAuditDate.ShowPopupOnFocus = true;
			//	if (!dmAuditDate.SelectedDate.HasValue)
			//		dmAuditDate.SelectedDate = SessionManager.UserContext.LocalTime;
				
			//	rddlDepartment.Enabled = true;
			//	rddlDepartment.Visible = true;

			//	btnSaveReturn.Enabled = true;
			//	btnSaveReturn.Visible = true;
			//	lblAddOrEditAudit.Text = "<strong>Add a New Assessment:</strong>";
			//	lblAuditType.Visible = false;
			//	btnDelete.Visible = false;
			//}
		}

		//protected void rddlAuditType_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	string selectedTypeId = rddlAuditType.SelectedValue;


		//	//if (!string.IsNullOrEmpty(selectedTypeId))
		//	//{
		//	//	SelectedTypeId = Convert.ToDecimal(selectedTypeId);
		//	//	IsEditContext = false;
		//	//	BuildForm();
		//	//}
		//}

		//protected void AuditLocation_Select(object sender, EventArgs e)
		//{
		//	string location = "0";
		//	if (sender is RadMenu)
		//	{
		//		location = mnuAuditLocation.SelectedItem.Value;
		//		mnuAuditLocation.Items[0].Text = mnuAuditLocation.SelectedItem.Text;
		//	}
		//	else if (sender is RadComboBox)
		//	{
		//		location = ddlAuditLocation.SelectedValue;
		//	}
		//	BuildAuditUsersDropdownList(location);
		//	hdnAuditLocation.Value = location;
		//	// rebuild the department list
		//	if (!location.ToLower().Equals("top"))
		//	{
		//		UpdateDepartments(Convert.ToDecimal(location));
		//	}

		//	// need to rebuild the form
		//	string selectedTypeId = rddlAuditType.SelectedValue;
		//	//if (!string.IsNullOrEmpty(selectedTypeId))
		//	//{
		//	//	SelectedTypeId = Convert.ToDecimal(selectedTypeId);
		//	//	IsEditContext = false;
		//	//	BuildForm();
		//	//}

		//}

		//void BuildAuditUsersDropdownList(string location)
		//{
		//	if (location != "")
		//	{
		//		rddlAuditUsers.Items.Clear();
		//		rddlAuditUsers.Items.Add(new DropDownListItem("", ""));
		//		var locationPersonList = new List<PERSON>();
		//		try
		//		{
		//			decimal locationId = Convert.ToDecimal(location);
		//			if (locationId > 0)
		//			{
		//				locationPersonList = SQMModelMgr.SelectPrivGroupPersonList(SysPriv.originate, SysScope.audit, locationId);
		//				locationPersonList = (from p in locationPersonList orderby p.LAST_NAME, p.FIRST_NAME select p).ToList();
		//			}
		//		}
		//		catch
		//		{ }
		//		if (locationPersonList.Count > 0)
		//		{
		//			foreach (PERSON p in locationPersonList)
		//			{
		//				string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
		//				// Check for duplicate list items
		//				var ddli = rddlAuditUsers.FindItemByValue(Convert.ToString(p.PERSON_ID));
		//				if (ddli == null)
		//					rddlAuditUsers.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
		//			}

		//			// If only one user, select by default
		//			if (rddlAuditUsers.Items.Count() == 2)
		//				rddlAuditUsers.SelectedIndex = 1;
		//		}
		//		else
		//		{
		//			rddlAuditUsers.Items[0].Text = "[No valid users - please change location]";
		//		}
		//	}
		//}

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			//if (Page.IsValid)
			//{
			//	//CurrentSubnav = (sender as RadButton).CommandArgument;
			//	try
			//	{
			//		CurrentStep = Convert.ToInt32((sender as RadButton).CommandArgument);
			//	}
			//	catch
			//	{
			//		CurrentStep = 0;
			//	}
			//	if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0) || (rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0) || (rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
			//	{
			//		string requiredFields = "";
			//		if ((hdnAuditLocation.Value.ToString().Trim().Length == 0 && lblAuditLocation.Text.ToString().Length == 0))
			//		{
			//			if (requiredFields.Trim().Length > 0)
			//				requiredFields += ", ";
			//			requiredFields += Resources.LocalizedText.BusinessLocation;
			//		}
			//		if ((rddlAuditUsers.SelectedIndex <= 0 && lblAuditPersonName.Text.ToString().Length == 0))
			//		{
			//			if (requiredFields.Trim().Length > 0)
			//				requiredFields += ", ";
			//			requiredFields += "Assessment Person";
			//		}
			//		if ((rddlDepartment.SelectedIndex == 0 && lblDepartment.Text.ToString().Length == 0))
			//		{
			//			if (requiredFields.Trim().Length > 0)
			//				requiredFields += ", ";
			//			requiredFields += Resources.LocalizedText.Department;
			//		}
			//		if (requiredFields.Trim().Length > 0)
			//			requiredFields = " (" + requiredFields + ")";
			//		string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg + requiredFields);
			//		ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			//	}
			//	else
			//		Save(true);
			//}
			//else
			//{
			//	string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsMsg);
			//	ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			//}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			//if (EditAuditId > 0)
			//{
			//	divAuditDetails.Visible = false;

			//	btnSaveReturn.Visible = false;
			//	btnDelete.Visible = false;
			//	lblResults.Visible = true;
			//	int delStatus = EHSAuditMgr.DeleteAudit(EditAuditId);
			//	// delete the task
			//	if (delStatus == 1)
			//	{
			//		EHSAuditMgr.DeleteAuditTask(EditAuditId, 50);
			//	}
			//	lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
			//	lblResults.Text += (delStatus == 1) ? "Assessment deleted." : "Error deleting assessment.";
			//	lblResults.Text += "</div>";
			//}

			//rddlAuditType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
			//AUDIT theAudit = null;
			//decimal auditId = 0;
			//if (CurrentStep == 0)
			//{
			//	auditTypeId = EditAuditTypeId;
			//	auditType = rddlAuditType.SelectedText;
			//}
			//else
			//{
			//	auditTypeId = EditAuditTypeId;
			//	auditType = EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
			//}

			//UpdateAnswersFromForm();

			//if (!IsEditContext)
			//{
			//	// Add context
			//	theAudit = CreateNewAudit();
			//	auditId = theAudit.AUDIT_ID;
			//}
			//else
			//{
			//	// Edit context
			//	auditId = EditAuditId;
			//	if (auditId > 0)
			//	{
			//		theAudit = UpdateAudit(auditId);
			//	}
			//}
			//if (auditId > 0)
			//{
			//	shouldReturn = AddOrUpdateAnswers(questions, auditId);
			//	if (!IsEditContext)
			//	{
			//		// notify the user of a new audit
			//		EHSNotificationMgr.NotifyOnAuditCreate(auditId, (decimal)theAudit.AUDIT_PERSON);
			//	}
			//}


			//if (shouldReturn == true)
			//{
			//	divForm.Visible = false;
			//	divAuditDetails.Visible = false;
			//	btnSaveReturn.Visible = false;

			//	RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
			//	if (rcbWarnNavigate != null)
			//		rcbWarnNavigate.Visible = false;

			//	lblResults.Visible = true;
			//}
			//else
			//{
			//	EditAuditId = auditId;
			//	SessionManager.ReturnRecordID = auditId;
			//	CurrentStep = 0;
			//	IsEditContext = true;
			//	Visible = true;
			//	// need to redraw the page with text boxes highlighted
			//	BuildForm();
			//	// and send a message that the field need to be entered
			//	string script = string.Format("alert('{0}');", Resources.LocalizedText.AssessmentRequiredsIndicatedMsg);
			//	ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			//}

			//if (shouldReturn)
			//{
			//	SessionManager.ReturnStatus = false;
			//	SessionManager.ReturnObject = "DisplayAudits";
			//	Response.Redirect("/EHS/EHS_Audits.aspx");  // mt - temporary
			//}

		}

		protected void UpdateAnswersFromForm()
		{

		}

		protected AUDIT CreateNewAudit()
		{
			//decimal auditId = 0;
			//PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			//DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, auditPlant.LOCAL_TIMEZONE);
			//var newAudit = new AUDIT()
			//{
			//	DETECT_COMPANY_ID = Convert.ToDecimal(auditPlant.COMPANY_ID),
			//	DETECT_BUS_ORG_ID = auditPlant.BUS_ORG_ID,
			//	DETECT_PLANT_ID = auditPlant.PLANT_ID,
			//	AUDIT_TYPE = "EHS",
			//	CREATE_DT = localTime,
			//	CREATE_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME,
			//	DESCRIPTION = tbDescription.Text.ToString(),
			//	CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
			//	AUDIT_DT = (DateTime)dmAuditDate.SelectedDate,
			//	AUDIT_TYPE_ID = auditTypeId,
			//	AUDIT_PERSON = Convert.ToDecimal(rddlAuditUsers.SelectedValue),
			//	DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue)
			//};
			//entities.AddToAUDIT(newAudit);
			//entities.SaveChanges();
			//auditId = newAudit.AUDIT_ID;

			//// create task record for their calendar
			//AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
			//EHSAuditMgr.CreateOrUpdateTask(auditId, Convert.ToDecimal(rddlAuditUsers.SelectedValue), 50, ((DateTime)dmAuditDate.SelectedDate).AddDays(audittype.DAYS_TO_COMPLETE), "A", SessionManager.UserContext.Person.PERSON_ID);

			AUDIT newAudit = new AUDIT();
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
			if (EditAuditId > 0)
			{
				audit = (from aud in entities.AUDIT where aud.AUDIT_ID == EditAuditId select aud).FirstOrDefault();
			}
			else
			{
				// what to do when it is new? I am thinking... nothing?
			}

			auditType = EHSAuditMgr.SelectAuditTypeByAuditId(EditAuditId);
			questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, 0, EditAuditId);

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
			//decimal totalQuestions = 0;
			//decimal totalAnswered = 0;
			//decimal totalPositive = 0;
			//decimal totalPercent = 0;
			//decimal totalWeightScore = 0;
			//decimal totalPossibleScore = 0;
			//decimal possibleScore = 0;
			//bool answerIsPositive = false;

			//foreach (var q in questions)
			//{
			//	var thisQuestion = q;
			//	var auditAnswer = (from ia in entities.AUDIT_ANSWER
			//					   where ia.AUDIT_ID == auditId
			//						   && ia.AUDIT_QUESTION_ID == thisQuestion.QuestionId
			//					   select ia).FirstOrDefault();
			//	// calculate the total percentages
			//	totalQuestions += 1;
			//	if (q.QuestionType == EHSAuditQuestionType.RadioPercentage || q.QuestionType == EHSAuditQuestionType.RadioPercentageCommentLeft)
			//	{
			//		answerIsPositive = false;
			//		//if (!q.AnswerText.ToString().Equals(""))
			//		//{
			//		possibleScore = 0;
			//		foreach (EHSAuditAnswerChoice choice in q.AnswerChoices)
			//		{
			//			if (choice.ChoiceWeight > possibleScore)
			//				possibleScore = choice.ChoiceWeight;
			//			if (choice.Value.Equals(q.AnswerText) && choice.ChoicePositive)
			//				answerIsPositive = true;
			//			if (choice.Value.Equals(q.AnswerText))
			//			{
			//				totalWeightScore += choice.ChoiceWeight;
			//				totalAnswered += 1;
			//			}
			//		}
			//		totalPossibleScore += possibleScore;
			//		if (answerIsPositive)
			//		{
			//			q.ChoicePositive = true;
			//			totalPositive += 1;
			//		}
			//		else
			//		{
			//			if (!q.AnswerText.ToString().Equals(""))
			//			{
			//				q.ChoicePositive = false;
			//				if (q.AnswerComment.ToString().Trim().Length == 0)
			//					negativeTextComplete = false;
			//			}
			//		}
			//		//}
			//	}
			//	else if (!q.AnswerText.ToString().Equals(""))
			//	{
			//		totalAnswered += 1;
			//	}

			//	if (q.CommentRequired && q.AnswerComment.ToString().Trim().Length == 0)
			//		negativeTextComplete = false;
			//	if (auditAnswer != null)
			//	{
			//		auditAnswer.ANSWER_VALUE = q.AnswerText;
			//		//auditAnswer.ORIGINAL_QUESTION_TEXT = q.QuestionText; // don't want to update text after the audit has been created
			//		auditAnswer.COMMENT = q.AnswerComment;
			//		auditAnswer.CHOICE_POSITIVE = q.ChoicePositive;
			//		if (q.ChoicePositive)
			//			auditAnswer.STATUS = "03";
			//		else
			//			auditAnswer.STATUS = "01";
			//	}
			//	else
			//	{
			//		auditAnswer = new AUDIT_ANSWER()
			//		{
			//			AUDIT_ID = auditId,
			//			AUDIT_QUESTION_ID = q.QuestionId,
			//			ANSWER_VALUE = q.AnswerText,
			//			ORIGINAL_QUESTION_TEXT = q.QuestionText,
			//			COMMENT = q.AnswerComment,
			//			CHOICE_POSITIVE = q.ChoicePositive
			//		};

			//		if (q.ChoicePositive)
			//			auditAnswer.STATUS = "03";
			//		else
			//			auditAnswer.STATUS = "01";

			//		entities.AddToAUDIT_ANSWER(auditAnswer);
			//	}
			//}
			//// now update the header info
			//AUDIT audit = (from i in entities.AUDIT where i.AUDIT_ID == auditId select i).FirstOrDefault();
			//if (totalQuestions > 0)
			//	totalPercent = Math.Round((totalAnswered / totalQuestions), 2) * 100;
			//else
			//	totalPercent = 0;
			//audit.PERCENT_COMPLETE = totalPercent;
			//if (totalPercent >= 100 && negativeTextComplete)
			//{
			//	audit.CURRENT_STATUS = "C";
			//	if (!audit.CLOSE_DATE_DATA_COMPLETE.HasValue)
			//	{
			//		PLANT plant = SQMModelMgr.LookupPlant((decimal)audit.DETECT_PLANT_ID);
			//		DateTime localTime = WebSiteCommon.LocalTime(DateTime.UtcNow, plant.LOCAL_TIMEZONE);
			//		audit.CLOSE_DATE_DATA_COMPLETE = localTime;
			//		audit.CLOSE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
			//	}
			//}
			//else if (totalPercent > 0)
			//{
			//	audit.CURRENT_STATUS = "I";
			//}
			//else
			//	audit.CURRENT_STATUS = "A";

			////if (totalQuestions > 0)
			////	totalPercent = Math.Round((totalPositive / totalQuestions), 2) * 100;
			////else
			////	totalPercent = 0;
			//if (totalPossibleScore > 0)
			//	totalPercent = Math.Round((totalWeightScore / totalPossibleScore), 2) * 100;
			//else
			//	totalPercent = 0;
			//audit.TOTAL_SCORE = totalPercent;

			//if (rddlDepartment.SelectedIndex > 0)
			//	audit.DEPT_ID = Convert.ToDecimal(rddlDepartment.SelectedValue);

			//// save all the changes
			//entities.SaveChanges();

			//if (audit.CURRENT_STATUS.Equals("C"))
			//{
			//	AUDIT_TYPE audittype = EHSAuditMgr.SelectAuditTypeById(entities, auditTypeId);
			//	EHSAuditMgr.CreateOrUpdateTask(auditId, SessionManager.UserContext.Person.PERSON_ID, 50, audit.AUDIT_DT.AddDays(audittype.DAYS_TO_COMPLETE), "C", SessionManager.UserContext.Person.PERSON_ID);
			//}
			return negativeTextComplete;
		}

		public void rptAuditFormTopics_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					AUDIT_TOPIC topic = (AUDIT_TOPIC)e.Item.DataItem;
					questions = EHSAuditMgr.SelectAuditQuestionList(EditAuditTypeId, topic.AUDIT_TOPIC_ID, EditAuditId);

					Repeater rpt = (Repeater)e.Item.FindControl("rptAuditFormQuestions");
					rpt.DataSource = questions;
					rpt.DataBind();

					RadAjaxManager1.AjaxSettings.AddAjaxSetting(rpt, RadAjaxPanel1);
					// after repeater is bound, need to determine if the topic total row needs to be displayed or hidden
				}
				catch
				{
				}
			}
		}

		public void rptAuditFormQuestions_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					LinkButton lnk = (LinkButton)e.Item.FindControl("lnkAddTask");
					//RadAjaxManager1.AjaxSettings.AddAjaxSetting(lnk, RadAjaxPanel1);
					//EHS_PROFILE_INPUT input = (EHS_PROFILE_INPUT)e.Item.DataItem;
					//EHS_PROFILE_MEASURE metric = LocalProfile().GetMeasure((decimal)input.PRMR_ID);

					//Label lbl;
					//LinkButton lnk;
					////TextBox tb;
					//DropDownList ddl;

					//// bool enabled = input.STATUS == "C" ? false : true;
					//bool enabled = true;
					//sharedCalendar.Visible = true;

					//RadDatePicker dtp1 = (RadDatePicker)e.Item.FindControl("radDateFrom");
					//dtp1.SharedCalendar = sharedCalendar;
					//dtp1.Enabled = enabled;
					//dtp1.ShowPopupOnFocus = true;

					//RadDatePicker dtp2 = (RadDatePicker)e.Item.FindControl("radDateTo");
					//dtp2.SharedCalendar = sharedCalendar;
					//dtp2.Enabled = enabled;
					//dtp2.ShowPopupOnFocus = true;

					//SETTINGS sets = SQMSettings.GetSetting("EHS", "INPUTSPAN");
					//int inputspan = 0;
					//int monthSpan1 = Convert.ToInt32(WebSiteCommon.GetXlatValue("invoiceSpan", "MINDATE"));
					//int monthSpan2 = monthSpan1;
					//if (sets != null && int.TryParse(sets.VALUE, out inputspan))
					//{
					//	monthSpan2 = monthSpan1 = inputspan;
					//}
					//dtp1.MinDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(monthSpan1 * -1);
					//dtp2.MinDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(monthSpan2 * -1);

					//if (inputspan > 0)
					//	dtp1.MaxDate = dtp2.MaxDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(inputspan);
					//else
					//	dtp1.MaxDate = dtp2.MaxDate = LocalProfile().InputPeriod.PeriodDate.AddMonths(Convert.ToInt32(WebSiteCommon.GetXlatValue("invoiceSpan", "MAXDATE")));

					//dtp1.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					//if (input != null)
					//{
					//	if (input.STATUS == "N")
					//		dtp1.Focus();
					//	if (input.EFF_FROM_DT > DateTime.MinValue)
					//		dtp1.SelectedDate = input.EFF_FROM_DT;
					//	else
					//		dtp1.FocusedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);
					//}

					//dtp2.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
					//if (input != null && input.EFF_TO_DT > DateTime.MinValue)
					//	dtp2.SelectedDate = input.EFF_TO_DT;
					//else
					//	dtp2.FocusedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);


					//UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == input.UOM);
					//if (uom != null)
					//{
					//	lbl = (Label)e.Item.FindControl("lblMetricUOM");
					//	lbl.Text = uom.UOM_CD;
					//}

					//lbl = (Label)e.Item.FindControl("lblMetricCurrency");
					//lbl.Text = metric.DEFAULT_CURRENCY_CODE;

					//if (input != null)
					//	lbl.Text = input.CURRENCY_CODE;

					//TextBox tbValue = (TextBox)e.Item.FindControl("tbMetricValue");
					//tbValue.Enabled = enabled;
					//if (input != null && (dtp1.SelectedDate != null && dtp2.SelectedDate != null))
					//	//if (input != null && input.MEASURE_VALUE != null)
					//	tbValue.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_VALUE, 2);

					//TextBox tbCost = (TextBox)e.Item.FindControl("tbMetricCost");
					//TextBox tbCredit = (TextBox)e.Item.FindControl("tbMetricCredit");

					//if ((bool)metric.NEG_VALUE_ALLOWED)
					//{
					//	tbCredit.Visible = tbCredit.Enabled = enabled;
					//	tbCost.Enabled = false;
					//}
					//else
					//{
					//	tbCredit.Visible = false;
					//	tbCost.Enabled = true;
					//}

					//if (input != null && input.MEASURE_COST.HasValue && input.MEASURE_COST < 0)
					//	tbCredit.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_COST * -1, 2);

					//if (input != null && input.MEASURE_COST.HasValue && input.MEASURE_COST >= 0)
					//	tbCost.Text = SQMBasePage.FormatValue((decimal)input.MEASURE_COST, 2);

					//if (metric.EHS_MEASURE.MEASURE_CATEGORY == "PROD" || metric.EHS_MEASURE.MEASURE_CATEGORY == "SAFE" || metric.EHS_MEASURE.MEASURE_CATEGORY == "FACT")
					//{
					//	dtp1.SelectedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, 1);
					//	dtp1.Enabled = false;
					//	dtp2.SelectedDate = new DateTime(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth, DateTime.DaysInMonth(LocalProfile().InputPeriod.PeriodYear, LocalProfile().InputPeriod.PeriodMonth));
					//	dtp2.Enabled = false;
					//	tbCost.Visible = false;
					//	tbCredit.Visible = false;
					//	lbl = (Label)e.Item.FindControl("lblMetricCurrency");
					//	lbl.Visible = false;
					//}

					//if (LocalProfile().GetMeasureExt(metric, WebSiteCommon.LocalTime(DateTime.UtcNow, LocationTZ)) != null && metric.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT.HasValue)
					//{
					//	tbValue.CssClass = "defaultText";
					//	tbValue.ToolTip = hfDefaultValue.Value + metric.EHS_PROFILE_MEASURE_EXT.NOTE;
					//	tbValue.ReadOnly = metric.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED == true ? false : true;
					//	if (string.IsNullOrEmpty(tbValue.Text))
					//		tbValue.Text = SQMBasePage.FormatValue((decimal)metric.EHS_PROFILE_MEASURE_EXT.VALUE_DEFAULT, 2);
					//}
					//if (LocalProfile().GetMeasureExt(metric, WebSiteCommon.LocalTime(DateTime.UtcNow, LocationTZ)) != null && metric.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT.HasValue)
					//{
					//	tbCost.CssClass = "defaultText";
					//	tbCost.ToolTip = hfDefaultValue.Value + metric.EHS_PROFILE_MEASURE_EXT.NOTE;
					//	tbCost.ReadOnly = metric.EHS_PROFILE_MEASURE_EXT.OVERRIDE_ALLOWED == true ? false : true;
					//	if (string.IsNullOrEmpty(tbCost.Text))
					//		tbCost.Text = SQMBasePage.FormatValue((decimal)metric.EHS_PROFILE_MEASURE_EXT.COST_DEFAULT, 2);
					//}

					//CheckBox cbDelete = (CheckBox)e.Item.FindControl("cbDelete");
					////string cbId = "ctl00_ContentPlaceHolder_Body_rptProfilePeriod_ctl06_rptProfileInput_ctl01_cbDelete";
					//cbDelete.Attributes.Add("onClick", "CheckInputDelete('" + cbDelete.ClientID + "');");
					//if (input.STATUS == "A" || input.STATUS == "D")
					//	cbDelete.Enabled = true;

					//if (input.STATUS == "D")
					//{
					//	cbDelete.Checked = true;
					//	cbDelete.ToolTip = hfDeleteText.Value;
					//	hfNumDelete.Value = (Convert.ToInt32(hfNumDelete.Value) + 1).ToString();
					//}
				}
				catch (Exception ex)
				{
					;
				}
			}
		}

		#endregion

		// manage current session object  (formerly was page static variable)
		SQMMetricMgr HSCalcs()
		{
			if (SessionManager.CurrentObject != null && SessionManager.CurrentObject is SQMMetricMgr)
				return (SQMMetricMgr)SessionManager.CurrentObject;
			else
				return null;
		}
		SQMMetricMgr SetHSCalcs(SQMMetricMgr hscalcs)
		{
			SessionManager.CurrentObject = hscalcs;
			return HSCalcs();
		}

		protected string linkArgs(object auditID, object questionID)
		{
			string args = auditID.ToString().Trim() + "," + questionID.ToString().Trim();
			return args;
		}
	}


}
