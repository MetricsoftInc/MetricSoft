using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Globalization;
using System.Threading;
using SQM.Shared;

namespace SQM.Website
{
	public partial class Ucl_INCFORM_Action : System.Web.UI.UserControl
	{

		const Int32 MaxTextLength = 4000;

		protected decimal companyId;

		protected int totalFormSteps;

		protected decimal incidentTypeId;
		protected string incidentType;
		protected bool IsFullPagePostback = false;

		public PageUseMode PageMode { get; set; }

		PSsqmEntities entities;

		public decimal theincidentId { get; set; }

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
			}
		}

		public decimal SelectedTypeId
		{
			get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
			set { ViewState["SelectedTypeId"] = value; }
		}
        public int CurrentStep
        {
            get { return ViewState["CurrentStep"] == null ? 0 : (int)ViewState["CurrentStep"]; }
            set { ViewState["CurrentStep"] = value; }
        }
        public decimal IncidentId
		{
			get { return ViewState["IncidentId"] == null ? 0 : (decimal)ViewState["IncidentId"]; }
			set { ViewState["IncidentId"] = value; }
		}
		public INCIDENT LocalIncident
		{
			get { return ViewState["LocalIncident"] == null ? null : (INCIDENT)ViewState["LocalIncident"]; }
			set { ViewState["LocalIncident"] = value; }
		}
		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
		}
		public decimal NewIncidentId
		{
			get { return ViewState["NewIncidentId"] == null ? 0 : (decimal)ViewState["NewIncidentId"]; }
			set { ViewState["NewIncidentId"] = value; }
		}

		public INCIDENT ActionIncident
		{
			get { return ViewState["ActionINCIDENT"] == null ? null : (INCIDENT)ViewState["ActionINCIDENT"]; }
			set { ViewState["ActionINCIDENT"] = value; }
		}

		protected decimal EditIncidentTypeId
		{
			get { return IncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(IncidentId); }
		}

		public string ValidationGroup
		{
			get { return ViewState["ValidationGroup"] == null ? " " : (string)ViewState["ValidationGroup"]; }
			set { ViewState["ValidationGroup"] = value; }
		}

		public List<XLAT> XLATList
		{
			get { return ViewState["ActionXLATList"] == null ? null : (List<XLAT>)ViewState["ActionXLATList"]; }
			set { ViewState["ActionXLATList"] = value; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			if (SessionManager.SessionContext != null)
			{
				if (IsFullPagePostback)
					rptAction.DataBind();
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			PSsqmEntities entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			if (IsPostBack)
			{
				// Since IsPostBack is always TRUE for every invocation of this user control we need some way 
				// to determine whether or not to refresh its page controls, or just data bind instead.  
				// Here we are using the "__EVENTTARGET" form event property to see if this user control is loading 
				// because of certain page control events that are supposed to be fired off as actual postbacks.  

				IsFullPagePostback = false;
				var targetID = Request.Form["__EVENTTARGET"];
				if (!string.IsNullOrEmpty(targetID))
				{
					var targetControl = this.Page.FindControl(targetID);

					if (targetControl != null)
						if ((this.Page.FindControl(targetID).ID == "btnSave") || 
							(this.Page.FindControl(targetID).ID == "btnNext") || 
							(this.Page.FindControl(targetID).ID == "btnAddFinal"))
								IsFullPagePostback = true;
				}
			}
		}


		protected override void FrameworkInitialize()
		{
			//String selectedLanguage = "es";
			if (SessionManager.SessionContext != null)
			{
				String selectedLanguage = SessionManager.UserContext.Language.NLS_LANGUAGE;
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

				base.FrameworkInitialize();
			}
		}


		public void PopulateInitialForm()
		{
			PSsqmEntities entities = new PSsqmEntities();
			IncidentId = (IsEditContext) ? IncidentId : NewIncidentId;

			if (IncidentId > 0)
			{
				try
				{
					ActionIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).Single();
					PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)ActionIncident.DETECT_PLANT_ID, "");
					if (plant != null)
						IncidentLocationTZ = plant.LOCAL_TIMEZONE;
				}
				catch { }

				XLATList = SQMBasePage.SelectXLATList(new string[1] { "ACTION_CATEGORY" }, SessionManager.UserContext.Person.PREFERRED_LANG_ID.HasValue ? (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID : 1);

				decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

				// what the fuck does this do ?
				//formSteps = EHSIncidentMgr.GetStepsForincidentTypeId(typeId);
				//totalFormSteps = formSteps.Count();

				InitializeForm();
			}
		}

        protected void SaveAttachments(decimal incidentId)
        {
            if (uploaderFinalCorrectiveAction != null)
            {
                string recordStep = (this.CurrentStep + 1).ToString();

                // Add files to database
                SessionManager.DocumentContext = new DocumentScope().CreateNew(
                    SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID, "BLI", 0, "",
                    SessionManager.UserContext.WorkingLocation.Plant.PLANT_ID, "", 0
                    );
                SessionManager.DocumentContext.RecordType = 40;
                SessionManager.DocumentContext.RecordID = incidentId;
                SessionManager.DocumentContext.RecordStep = "1";
                SessionManager.DocumentContext.incident_section = (int)Incident_Section.FinalCorrectiveAttachment;

                uploaderFinalCorrectiveAction.SaveFilesFinalCorrectiveAction();
            }
        }
        private void GetAttachments(decimal incidentId)
        {
            uploaderFinalCorrectiveAction.SetAttachmentRecordStep("1");
            uploaderFinalCorrectiveAction.SetReportOption(false);
            uploaderFinalCorrectiveAction.SetDescription(false);
            // Specifying postback triggers allows uploader to persist on other postbacks (e.g. 8D checkbox toggle)
            //uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete", "btnDeleteInc", "btnSubnavIncident", "btnSubnavContainment", "btnSubnavRootCause", "btnSubnavAction", "btnSubnavApproval" };
            //uploader.RAUpload.PostbackTriggers = new string[] { "btnSubnavSave", "btnSaveReturn", "btnSaveContinue", "btnDelete", "btnDeleteInc", "btnSubnavIncident", "btnSubnavContainment", "btnSubnavRootCause", "btnSubnavAction", "btnSubnavApproval" };

            int attCnt = EHSIncidentMgr.AttachmentCounts(incidentId, 1);//Apply 1 for getting the attachment for  Final Corrective Action section.
            int px = 128;

            if (attCnt > 0)
            {
                px = px + (attCnt * 30) + 35;
                uploaderFinalCorrectiveAction.GetUploadedFilesIncidentSection(40, incidentId, "", (int)Incident_Section.FinalCorrectiveAttachment);
            }
            else
            {

                uploaderFinalCorrectiveAction.GetBlinkATTACHMENT();
            }

            /*

   */
            // Set the html Div height based on number of attachments to be displayed in the grid:
            //dvAttachLbl.Style.Add("height", px.ToString() + "px !important");
            //dvAttach.Style.Add("height", px.ToString() + "px !important");
        }

        void InitializeForm()
		{
            //call function for get the attachment files for  Final Corrective Action.
            GetAttachments(IncidentId);

            IncidentId = (IsEditContext) ? IncidentId : NewIncidentId;
			lblStatusMsg.Visible = false;

			LocalIncident = EHSIncidentMgr.SelectIncidentById(new PSsqmEntities(), IncidentId);
			if (LocalIncident == null)
			{
				return;
			}

			pnlAction.Visible = true;
			if (PageMode == PageUseMode.ViewOnly)
			{
				divTitle.Visible = true;
				lblFormTitle.Text = Resources.LocalizedText.CorrectiveAction;
			}

			rptAction.DataSource = EHSIncidentMgr.GetCorrectiveActionList(IncidentId, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ), PageMode == PageUseMode.ViewOnly ? false : true);
			rptAction.DataBind();

			pnlAction.Enabled = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.action, LocalIncident.INCFORM_LAST_STEP_COMPLETED);

			if (PageMode == PageUseMode.ViewOnly  && rptAction.Items.Count == 0)
			{
				rptAction.Visible = false;
			}
		}

		protected void rddlActionPerson_SelectedIndexChanged(object sender, DropDownListEventArgs e)
		{
			// Add JobCode and any other related logic
		}


		public void rptAction_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				int minRowsToValidate = 1;

				try
				{
					TASK_STATUS action = (TASK_STATUS)e.Item.DataItem;

					string addFields = SessionManager.GetUserSetting("EHS", "ACTION_ADD_FIELDS") == null ? "" : SessionManager.GetUserSetting("EHS", "ACTION_ADD_FIELDS").VALUE;  //EHSSettings.Where(s => s.SETTING_CD == "ACTION_ADD_FIELDS").FirstOrDefault() == null ? "" : EHSSettings.Where(s => s.SETTING_CD == "ACTION_ADD_FIELDS").FirstOrDefault().VALUE;

					HiddenField hf;
					System.Web.UI.HtmlControls.HtmlTableRow tr;
					TextBox tbca = (TextBox)e.Item.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)e.Item.FindControl("rddlActionPerson");
					Label lb = (Label)e.Item.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)e.Item.FindControl("rdpFinalStartDate");
					sd = SQMBasePage.SetRadDateCulture(sd, "");
					RadDatePicker cd = (RadDatePicker)e.Item.FindControl("rdpFinalCompleteDate");
					cd = SQMBasePage.SetRadDateCulture(cd, "");
					//CheckBox ic = (CheckBox)e.Item.FindControl("cbFinalIsComplete");
					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					if (addFields.Contains("type"))
					{
						tr = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trActionType");
						tr.Visible = true;

						hf = (HiddenField)e.Item.FindControl("hfActionType");
						hf.Value = action.TASK_CATEGORY;
						RadGrid rgActionType = (RadGrid)e.Item.FindControl("rgActionTypeList");
						BindActionTypeSelect(rgActionType, action);

						XLAT xlat = XLATList.Where(l => l.XLAT_GROUP == "ACTION_CATEGORY" && l.XLAT_CODE == action.TASK_CATEGORY).FirstOrDefault();
						if (xlat != null)
						{
							lb = (Label)e.Item.FindControl("lblActionType");
							lb.Text = xlat.DESCRIPTION;
							Image img = (Image)e.Item.FindControl("imgActionType");
							switch (xlat.SORT_ORDER)
							{
								case 1: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-up.png"; break;
								case 2: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-up-right.png"; break;
								case 3: img.ImageUrl = "~/images/defaulticon/16x16/3D-plane.png"; break;
								case 4: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-down-right.png"; break;
								default: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-down.png"; break;
							}
						}

					}

					rddlp.Items.Add(new DropDownListItem("", ""));
					List<PERSON> personList = personList = EHSIncidentMgr.SelectIncidentPersonList(LocalIncident, true);
					foreach (PERSON p in personList)
					{
						string displayName = string.Format("{0}, {1} ({2})", p.LAST_NAME, p.FIRST_NAME, p.EMAIL);
						rddlp.Items.Add(new DropDownListItem(displayName, Convert.ToString(p.PERSON_ID)));
					}

					lb = (Label)e.Item.FindControl("lbItemSeq");
					lb.Text = action.TASK_SEQ.ToString();
					tbca.Text = action.DESCRIPTION;

					if (action.RESPONSIBLE_ID != null)
						rddlp.SelectedValue = action.RESPONSIBLE_ID.ToString();
					if (action.DUE_DT.HasValue)
						sd.SelectedDate = action.DUE_DT;

					hf = (HiddenField)e.Item.FindControl("hfTaskStatus");
					hf.Value = action.STATUS;
					hf = (HiddenField)e.Item.FindControl("hfTaskID");
					hf.Value = action.TASK_ID.ToString();
					hf = (HiddenField)e.Item.FindControl("hfRecordType");
					hf.Value = action.RECORD_TYPE.ToString();
					hf = (HiddenField)e.Item.FindControl("hfRecordID");
					hf.Value = action.RECORD_ID.ToString();
					hf = (HiddenField)e.Item.FindControl("hfTaskStep");
					hf.Value = action.TASK_STEP;
					hf = (HiddenField)e.Item.FindControl("hfTaskType");
					hf.Value = action.TASK_TYPE;
					hf = (HiddenField)e.Item.FindControl("hfDetail");
					hf.Value = action.DETAIL;
					hf = (HiddenField)e.Item.FindControl("hfComments");
					hf.Value = action.COMMENTS;
					hf = (HiddenField)e.Item.FindControl("hfVerification");
					hf.Value = action.TASK_VERIFICATION;

					if (action.COMPLETE_DT.HasValue)
					{
						cd.SelectedDate = action.COMPLETE_DT;
						hf = (HiddenField)e.Item.FindControl("hfCompleteID");
						if (action.COMPLETE_ID.HasValue)
							hf.Value = action.COMPLETE_ID.ToString();
					}

					if (action.CREATE_DT.HasValue)
					{
						hf = (HiddenField)e.Item.FindControl("hfCreateDT");
						DateTime dt = Convert.ToDateTime(action.CREATE_DT);
						hf.Value = dt.ToString("M/d/yyyy");
					}

					if (addFields.Contains("comment"))
					{
						tr = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trComments");
						tr.Visible = true;
						tbca = (TextBox)e.Item.FindControl("tbComments");
						tbca.Text = action.COMMENTS;
					}

					if (addFields.Contains("verify"))
					{
						tr = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("trVerification");
						tr.Visible = true;
						tbca = (TextBox)e.Item.FindControl("tbVerification");
						tbca.Text = action.TASK_VERIFICATION;
					}

					itmdel.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.action, LocalIncident.INCFORM_LAST_STEP_COMPLETED);

				}
				catch { }
			}

			btnSave.Visible = btnAddFinal.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.action, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
		}

		protected List<TASK_STATUS> GetActionListFromGrid()
		{
			List<TASK_STATUS> actionList = new List<TASK_STATUS>();
			int seqnumber = 0;

			foreach (RepeaterItem containtem in rptAction.Items)
			{
				try
				{
					TASK_STATUS action = new TASK_STATUS();

					TextBox tbca = (TextBox)containtem.FindControl("tbFinalAction");
					RadDropDownList rddlp = (RadDropDownList)containtem.FindControl("rddlActionPerson");
					Label lb = (Label)containtem.FindControl("lbItemSeq");
					RadDatePicker sd = (RadDatePicker)containtem.FindControl("rdpFinalStartDate");
					RadDatePicker cd = (RadDatePicker)containtem.FindControl("rdpFinalCompleteDate");
					//CheckBox ic = (CheckBox)containtem.FindControl("cbFinalIsComplete");

					action.DESCRIPTION = tbca.Text;
					action.RESPONSIBLE_ID = (String.IsNullOrEmpty(rddlp.SelectedValue)) ? 0 : Convert.ToInt32(rddlp.SelectedValue);
					action.TASK_SEQ = Convert.ToInt32(lb.Text);
					action.DUE_DT = sd.SelectedDate;
					action.COMPLETE_DT = cd.SelectedDate;

					HiddenField hf = (HiddenField)containtem.FindControl("hfTaskStatus");
					action.STATUS = hf.Value;
					hf = (HiddenField)containtem.FindControl("hfTaskID");
					action.TASK_ID = Convert.ToDecimal(hf.Value);
					hf = (HiddenField)containtem.FindControl("hfCreateDT");
					if (!string.IsNullOrEmpty(hf.Value))
					{
						action.CREATE_DT = DateTime.ParseExact(hf.Value, "M/d/yyyy", null);
					}
					hf = (HiddenField)containtem.FindControl("hfRecordID");
					action.RECORD_ID = Convert.ToDecimal(hf.Value);
					hf = (HiddenField)containtem.FindControl("hfRecordType");
					action.RECORD_TYPE = Convert.ToInt32(hf.Value);
					hf = (HiddenField)containtem.FindControl("hfTaskType");
					action.TASK_TYPE = hf.Value;
					hf = (HiddenField)containtem.FindControl("hfTaskStep");
					action.TASK_STEP = hf.Value;
					hf = (HiddenField)containtem.FindControl("hfDetail");
					action.DETAIL = hf.Value;
					hf = (HiddenField)containtem.FindControl("hfComments");
					action.COMMENTS = hf.Value;
					hf = (HiddenField)containtem.FindControl("hfVerification");
					action.TASK_VERIFICATION = hf.Value;
					if (action.COMPLETE_DT.HasValue)
					{
						hf = (HiddenField)containtem.FindControl("hfCompleteID");
						if (!string.IsNullOrEmpty(hf.Value))
							action.COMPLETE_ID = Convert.ToDecimal(hf.Value);
						else
							action.COMPLETE_ID = action.RESPONSIBLE_ID;
					}

					hf = (HiddenField)containtem.FindControl("hfActionType");
					action.TASK_CATEGORY = hf.Value;

					if ((tbca = (TextBox)containtem.FindControl("tbComments")) != null && tbca.Visible == true)
					{
						action.COMMENTS = tbca.Text;
					}

					if ((tbca = (TextBox)containtem.FindControl("tbVerification")) != null && tbca.Visible == true)
					{
						action.TASK_VERIFICATION = tbca.Text;
					}

					action.TASK_CRITERIA = "";
					string criteriaText = "";
					RadGrid rg = (RadGrid)containtem.FindControl("rgActionTypeList");
					foreach (GridItem item in rg.Items)
					{
						hf = (HiddenField)item.FindControl("hfActionType");
						tbca = (TextBox)item.FindControl("tbActionCriteria");
						criteriaText = tbca.Text.Replace('~', ' ');  criteriaText = criteriaText.Replace('|',' ');
						action.TASK_CRITERIA += string.IsNullOrEmpty(action.TASK_CRITERIA) ? (hf.Value + "~" + criteriaText) : ("|" + hf.Value + "~" + criteriaText);
					}

					actionList.Add(action);
				}
				catch
				{
				}
			}

			return actionList;
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (AddUpdateINCFORM_ACTION(LocalIncident.INCIDENT_ID) >= 0)
			{
				string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
				InitializeForm();
			}
		}

		public int AddUpdateINCFORM_ACTION(decimal incidentId)
		{          
            int status = 0;
			lblStatusMsg.Visible = false;
			List<TASK_STATUS> actionList = GetActionListFromGrid();
			bool allFieldsComplete = true;
          
            foreach (TASK_STATUS action in actionList)
			{
				if (string.IsNullOrEmpty(action.DESCRIPTION) || !action.DUE_DT.HasValue || !action.RESPONSIBLE_ID.HasValue)
				{
					allFieldsComplete = false;
					break;
				}
			}

			if (!allFieldsComplete)
			{
				lblStatusMsg.Text = Resources.LocalizedText.ENVProfileRequiredsMsg;
				lblStatusMsg.Visible = true;
				status = -1;
			}
			else 
			{
				status = SaveActions(incidentId, actionList);
			}
            //Apply condition if the data will not save into the DB.
            if (allFieldsComplete)
            {
                SaveAttachments(incidentId);
            }
            
            return status;
		}

        private int SaveActions(decimal incidentId, List<TASK_STATUS> actionList)
		{

			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;

			foreach (TASK_STATUS action in actionList)
			{
				if (!string.IsNullOrEmpty(action.DESCRIPTION)  &&  action.DUE_DT.HasValue &&  action.RESPONSIBLE_ID.HasValue)
				{
					EHSIncidentMgr.CreateOrUpdateTask(ActionIncident, action, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
				}
			}

			if (status > -1)
			{
				EHSNotificationMgr.NotifyIncidentStatus(ActionIncident, ((int)SysPriv.update).ToString(), "Corrective action specified");
			}

			EHSIncidentMgr.UpdateIncidentStatus(incidentId, IncidentStepStatus.correctiveaction, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));

			return status;
		}


		protected void AddDelete_Click(object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			if (btn.CommandArgument == "AddAnother")
			{
				List<TASK_STATUS> actionList = GetActionListFromGrid();
				int newSeq = actionList.Max(l => l.TASK_SEQ).Value + 1;
				actionList.Add(EHSIncidentMgr.CreateEmptyTask(ActionIncident.INCIDENT_ID, ((int)SysPriv.action).ToString(), newSeq, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ)));
				rptAction.DataSource = actionList;
				rptAction.DataBind();
			}
		}

		protected void rptAction_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			List<TASK_STATUS> actionList = GetActionListFromGrid();

			if (e.CommandArgument.ToString() == "ActionType")
			{
				Panel pnl = (Panel)e.Item.FindControl("pnlActionType");
				if (pnl.Visible)
				{
					pnl.Visible = false;
					pnl = (Panel)e.Item.FindControl("pnlActionTypeSelect");
					pnl.Visible = true;
					HiddenField hf = (HiddenField)e.Item.FindControl("hfActionType");
					XLAT xlat = XLATList.Where(l => l.XLAT_GROUP == "ACTION_CATEGORY" && l.XLAT_CODE == hf.Value).FirstOrDefault();
					if (xlat != null)
					{
						Label lb = (Label)e.Item.FindControl("lblActionType");
						lb.Text = xlat.DESCRIPTION;
					}
				}
				else
				{
					pnl.Visible = true;
					pnl = (Panel)e.Item.FindControl("pnlActionTypeSelect");
					pnl.Visible = false;
				}
			}

			if (e.CommandArgument.ToString() == "Delete")
			{
				int delId = e.Item.ItemIndex;

				TASK_STATUS action = actionList.ElementAt(delId);
				if (action != null)
				{
					if (action.TASK_ID > 0)  // only delete existing actions
					{
						using (PSsqmEntities entities = new PSsqmEntities())
						{
							entities.ExecuteStoreCommand("DELETE FROM TASK_STATUS WHERE TASK_ID = " + action.TASK_ID.ToString());
						}
					}
					actionList.Remove(action);
					if (actionList.Count == 0)
					{
						actionList.Add(EHSIncidentMgr.CreateEmptyTask(ActionIncident.INCIDENT_ID, ((int)SysPriv.action).ToString(), 1, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ)));
					}
				}

				rptAction.DataSource = actionList;
				rptAction.DataBind();

				decimal incidentId = (IsEditContext) ? IncidentId : NewIncidentId;
			}
		}

		#region actiontype

		protected string GetActionTypeCriteria(TASK_STATUS action, string actionCategory)
		{
			string criteria = "";
			try 
			{
				List<ActionType> actionTypeList = new List<ActionType>();
				string[] segments = action.TASK_CRITERIA.Split('|');
				var criteriaList = new Dictionary<string, string>();
				foreach (string s in segments)
				{
					string[] args = s.Split('~');
					if (args.Length > 1)
					{
						criteriaList.Add(args[0], args[1]);
					}
				}
					criteria = criteriaList[actionCategory];
			}
			catch
			{
			}

			return criteria;
		}


		protected void BindActionTypeSelect(RadGrid rgActionType, TASK_STATUS action)
		{
			List<ActionType> actionTypeList = new List<ActionType>();

			foreach (XLAT xlat in XLATList.Where(l => l.XLAT_GROUP == "ACTION_CATEGORY").OrderBy(l=> l.SORT_ORDER).ToList())
			{
				ActionType actionType = new ActionType();
				actionType.ActionTypeCode = xlat.XLAT_CODE;
				actionType.ActionDescription = xlat.DESCRIPTION;
				actionType.ActionSeq = (int)xlat.SORT_ORDER;
				actionType.Criteria = GetActionTypeCriteria(action, xlat.XLAT_CODE);
				if (xlat.XLAT_CODE == action.TASK_CATEGORY)
				{
					actionType.IsSelected = true;
				}
				actionTypeList.Add(actionType);
			}

			rgActionType.DataSource = actionTypeList;
			rgActionType.DataBind();
		}

		protected void rgActionTypeList_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				GridDataItem item = (GridDataItem)e.Item;
				Image img;

				try
				{
					ActionType actionType = (ActionType)e.Item.DataItem;

					Label lbl = (Label)e.Item.FindControl("lblActionType");
					lbl.Text = actionType.ActionDescription;

					TextBox tb = (TextBox)e.Item.FindControl("tbActionCriteria");
					tb.Text = actionType.Criteria;

					img = (Image)e.Item.FindControl("imgEffectiveness");
					lbl = (Label)e.Item.FindControl("lblEffectiveness");
					switch (actionType.ActionSeq)
					{
						case 1: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-up.png"; lbl.Text = "High"; break;
						case 2: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-up-right.png"; lbl.Text = "Moderate";  break;
						case 3: img.ImageUrl = "~/images/defaulticon/16x16/3D-plane.png"; lbl.Text = "Neutral"; break;
						case 4: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-down-right.png"; lbl.Text = "Minimal"; break;
						case 5: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-down.png"; lbl.Text = "Low"; break;
						default: img.ImageUrl = "~/images/defaulticon/16x16/arrow-1-down.png"; lbl.Text = "Indeterminate"; break;
					}

					HiddenField hf = (HiddenField)e.Item.FindControl("hfActionType");
					hf.Value = actionType.ActionTypeCode;

					if (actionType.IsSelected)
					{
						CheckBox cb = (CheckBox)e.Item.FindControl("cbSelectType");
						cb.Checked = true;
						lbl = (Label)cb.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.FindControl("lblActionCriteria");
						lbl.Text = actionType.Criteria;
					}
				}
				catch
				{
				}
			}
		}

		protected void ActionTypeSelect_Checked(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;

			if (cb.Checked)
			{
				RadGrid rg = (RadGrid)cb.Parent.Parent.Parent.Parent.Parent;	// inner grid
				{
					foreach (GridItem item in rg.Items)
					{
						CheckBox cx = (CheckBox)item.FindControl("cbSelectType");
						if (cx == cb)
						{
							cx.Checked = true;
							HiddenField hfx = (HiddenField)item.FindControl("hfActionType");
							HiddenField hf = (HiddenField)rg.Parent.Parent.FindControl("hfActionType");
							hf.Value = hfx.Value;
							TextBox tb = (TextBox)item.FindControl("tbActionCriteria");
							Label lbl = (Label)rg.Parent.Parent.FindControl("lblActionCriteria");
							lbl.Text = tb.Text;
						}
						else
						{
							cx.Checked = false;
						}
					}
				}
				//cb.Checked = true;
			}
		}
		#endregion

	}
}