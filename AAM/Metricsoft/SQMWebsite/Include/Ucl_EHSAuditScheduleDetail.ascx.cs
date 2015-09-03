using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using Telerik.Web.UI;
using System.Globalization;
using System.Threading;
using System.Drawing;

namespace SQM.Website
{
	public partial class Ucl_EHSAuditScheduleDetail : System.Web.UI.UserControl
	{
		const Int32 MaxTextLength = 4000;

		protected decimal companyId;
		protected decimal selectedPlantId = 0;
		protected decimal auditPlantId = 0;

		protected AccessMode accessLevel;
		bool controlQuestionChanged;

		List<EHSAuditQuestion> questions;
		PSsqmEntities entities;

		protected RadDropDownList rddlLocation;

		protected decimal auditTypeId;
		protected string auditType;

		public void EnableReturnButton(bool bEnabled)
		{
			ahReturn.Visible = bEnabled;
		}

		public bool IsEditContext
		{
			get { return ViewState["IsEditContext"] == null ? false : (bool)ViewState["IsEditContext"]; }
			set
			{
				ViewState["IsEditContext"] = value;
				RefreshPageContext();
			}
		}

		public int CurrentStep
		{
			get { return ViewState["CurrentStep"] == null ? 0 : (int)ViewState["CurrentStep"]; }
			set { ViewState["CurrentStep"] = value; }
		}

		public decimal EditAuditScheduleId
		{
			get { return ViewState["EditAuditScheduleId"] == null ? 0 : (decimal)ViewState["EditAuditScheduleId"]; }
			set { ViewState["EditAuditScheduleId"] = value; }
		}

		public decimal InitialPlantId
		{
			get { return ViewState["InitialPlantId"] == null ? 0 : (decimal)ViewState["InitialPlantId"]; }
			set { ViewState["InitialPlantId"] = value; }
		}

		protected decimal EditAuditTypeId
		{
			get { return EditAuditScheduleId == null ? 0 : EHSAuditMgr.SelectAuditTypeIdByAuditScheduleId(EditAuditScheduleId); }
		}

		protected decimal SelectedTypeId
		{
			get { return ViewState["SelectedTypeId"] == null ? 0 : (decimal)ViewState["SelectedTypeId"]; }
			set { ViewState["SelectedTypeId"] = value; }
		}

		protected string SelectedTypeText
		{
			get { return ViewState["SelectedTypeText"] == null ? " " : (string)ViewState["SelectedTypeText"]; }
			set { ViewState["SelectedTypeText"] = value; }
		}

		protected decimal SelectedLocationId
		{
			get { return ViewState["SelectedLocationId"] == null ? 0 : (decimal)ViewState["SelectedLocationId"]; }
			set { ViewState["SelectedLocationId"] = value; }
		}



		protected void Page_Load(object sender, EventArgs e)
		{
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			//accessLevel = UserContext.CheckAccess("EHS", "312");
			//accessLevel = UserContext.CheckAccess("EHS", "");
			accessLevel = AccessMode.Admin;  // mt - temporary
			entities = new PSsqmEntities();

			bool returnFromClick = false;
			var sourceId = Page.Request[Page.postEventSourceID];
			if (sourceId != null && sourceId.EndsWith("btnSaveReturn"))
			{
				// Stop extra script warning in when not actually editing a form
				string script = string.Format("$(window).unbind('beforeunload'); unsaved = false;");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "unload", script, true);
				returnFromClick = true;
			}

			if (IsPostBack)
			{
				divAuditForm.Visible = true;
				if (sourceId != null && sourceId.EndsWith("btnSaveReturn"))
				{
					// not sure what we need to do here
				}
				else
				{
					LoadInformation();
				}
			}
			else
			{
				RefreshPageContext();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				//var sourceId = Page.Request[Page.postEventSourceID];

				//if ((sourceId != null && (sourceId.EndsWith("btnSaveReturn"))) || sourceId == "")
				//{
				//	UpdateControlledQuestions();
				//}
			}
		}

		#region Form

		protected void UpdateAuditTypes()
		{
			if (rddlAuditType.Items.Count == 0)
			{
				var auditTypeList = new List<AUDIT_TYPE>();
				string selectString = "";
				auditTypeList = EHSAuditMgr.SelectAuditTypeList(companyId, true);
				selectString = "[Select An Audit Type]";
				if (auditTypeList.Count > 1)
					auditTypeList.Insert(0, new AUDIT_TYPE() { AUDIT_TYPE_ID = 0, TITLE = selectString });

				if (accessLevel < AccessMode.Admin)
					auditTypeList = (from i in auditTypeList where i.AUDIT_TYPE_ID != 10 select i).ToList();

				rddlAuditType.DataSource = auditTypeList;
				rddlAuditType.DataTextField = "TITLE";
				rddlAuditType.DataValueField = "AUDIT_TYPE_ID";
				rddlAuditType.DataBind();
			}
		}

		protected void LoadInformation()
		{
			// set up for adding the header info
			AccessMode accessmode = UserContext.RoleAccess();

			List<PRIVGROUP> pl = SQMModelMgr.SelectPrivGroupList(new SysPriv[1] { SysPriv.originate }, SysScope.audit, ""); // SQMModelMgr.SelectPrivGroupJobcodeList(SysPriv.originate, SysScope.audit);
			DropDownListItem item = new DropDownListItem();
			UpdateAuditTypes();

			if (rddlAuditJobcodes.Items.Count == 0)
			{
				rddlAuditJobcodes.DataSource = pl;
				rddlAuditJobcodes.DataTextField = "DESCRIPTION";
				rddlAuditJobcodes.DataValueField = "PRIV_GROUP";
				rddlAuditJobcodes.DataBind();
				item = new DropDownListItem("[Select a Group]", "");
				rddlAuditJobcodes.Items.Insert(0, item);
			}

			if (rddlDayOfWeek.Items.Count == 0)
			{
				rddlDayOfWeek.Items.Clear();
				rddlDayOfWeek.DataSource = EHS_Audit_Scheduler.GetAll<DayOfWeek>();
				rddlDayOfWeek.DataTextField = "Value";
				rddlDayOfWeek.DataValueField = "Key";
				rddlDayOfWeek.DataBind();
				item = new DropDownListItem("[Select a Day]", "");
				rddlDayOfWeek.Items.Insert(0, item);
			}

			if (IsEditContext || CurrentStep > 0)
			{
				// in edit mode, load the header field values and make all fields display only
				AUDIT_SCHEDULER scheduler = EHSAuditMgr.SelectAuditSchedulerById(entities, EditAuditScheduleId);
				BusinessLocation location = new BusinessLocation().Initialize((decimal)scheduler.PLANT_ID);
				rddlAuditType.SelectedValue = scheduler.AUDIT_TYPE_ID.ToString();
				rddlAuditType.Enabled = false;
				rddlAuditType.Visible = false;
				lblScheduleAuditType.Visible = true;
				lblScheduleAuditType.Text = rddlAuditType.SelectedText.ToString();

				hdnAuditLocation.Value = location.Plant.PLANT_ID.ToString();

				lblAuditLocation.Text = location.Plant.PLANT_NAME + " " + location.BusinessOrg.ORG_NAME;
				lblAuditLocation.Visible = true;
				ddlAuditLocation.Visible = false;
				mnuAuditLocation.Visible = false;

				rddlDayOfWeek.SelectedValue = scheduler.DAY_OF_WEEK.ToString();
				cbInactive.Checked = scheduler.INACTIVE;

				// build the audit jobcode list
				PRIVGROUP pv = pl.Where(i => i.PRIV_GROUP.ToString() == scheduler.JOBCODE_CD).FirstOrDefault();
				lblAuditJobcode.Text = pv.DESCRIPTION;
				lblAuditJobcode.Visible = true;
				rddlAuditJobcodes.SelectedValue = scheduler.JOBCODE_CD.ToString();
				rddlAuditJobcodes.Visible = false;
			}
			else
			{
				if (accessmode >= AccessMode.Plant)
				{
					List<BusinessLocation> locationList = SessionManager.PlantList;
					locationList = UserContext.FilterPlantAccessList(locationList, "EHS", "");
					locationList = UserContext.FilterPlantAccessList(locationList, "SQM", "");
					if (locationList.Select(l => l.Plant.BUS_ORG_ID).Distinct().Count() > 1 && SessionManager.IsUserAgentType("ipad,iphone") == false)
					{
						if (mnuAuditLocation.Items.Count == 0)
						{
							mnuAuditLocation.Items.Clear();

							ddlAuditLocation.Visible = false;
							mnuAuditLocation.Visible = true;
							mnuAuditLocation.Enabled = true;
							SQMBasePage.SetLocationList(mnuAuditLocation, locationList, 0, "Select a Location", "", true);
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
							ddlAuditLocation.Items[0].ImageUrl = "~/images/defaulticon/16x16/user-alt-2.png";
						}
					}
				}
				// set defaults for add mode
				rddlAuditType.Enabled = true;
				rddlAuditType.Visible = true;
				lblAuditLocation.Visible = false;
				rddlAuditJobcodes.Enabled = true;
				rddlAuditJobcodes.Visible = true;
				lblAuditJobcode.Visible = false;
				lblScheduleAuditType.Visible = false;
			}
		}


		protected void AuditLocation_Select(object sender, EventArgs e)
		{
			string location = "0";
			if (sender is RadMenu)
			{
				location = mnuAuditLocation.SelectedItem.Value;
				mnuAuditLocation.Items[0].Text = mnuAuditLocation.SelectedItem.Text;
			}
			else if (sender is RadSlider)
			{
				location = ddlAuditLocation.SelectedValue;
			}
			hdnAuditLocation.Value = location;

			// need to rebuild the form
			string selectedTypeId = rddlAuditType.SelectedValue;
			if (!string.IsNullOrEmpty(selectedTypeId))
			{
				SelectedTypeId = Convert.ToDecimal(selectedTypeId);
				IsEditContext = false;
			}

		}

		void rddlLocation_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		void UpdateButtonText()
		{
			btnSaveReturn.Text = "Save & Return";

			if (IsEditContext)
				btnSaveReturn.Text = "Save Audit Scheduler";
			else
				btnSaveReturn.Text = "Create Audit Scheduler";
		}

		#endregion


		#region Form Events

		protected void rddlAuditType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedTypeId = rddlAuditType.SelectedValue;
			//if (!string.IsNullOrEmpty(selectedTypeId))
			//{
			//	SelectedTypeId = Convert.ToDecimal(selectedTypeId);
			//	IsEditContext = false;
			//	BuildForm();
			//}
		}

		protected void btnSaveReturn_Click(object sender, EventArgs e)
		{
			if (hdnAuditLocation.Value.ToString().Trim().Equals("") || rddlDayOfWeek.SelectedIndex == 0 || rddlAuditJobcodes.SelectedIndex == 0 || rddlAuditType.SelectedIndex == 0)
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
			else if (Page.IsValid)
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
				Save(true);
			}
			else
			{
				string script = string.Format("alert('{0}');", "You must complete all required fields on this page to save.");
				ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			}
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if (EditAuditScheduleId > 0)
			{
				pnlForm.Visible = false;

				btnSaveReturn.Visible = false;
				btnDelete.Visible = false;
				lblResults.Visible = true;
				int delStatus = EHSAuditMgr.DeleteAuditScheduler(EditAuditScheduleId);
				lblResults.Text = "<div style=\"text-align: center; font-weight: bold; padding: 10px;\">";
				lblResults.Text += (delStatus == 1) ? "Audit Scheduler deleted." : "Error deleting audit scheduler.";
				lblResults.Text += "</div>";
			}

			rddlAuditType.SelectedIndex = 0;
		}

		protected void Save(bool shouldReturn)
		{
			AUDIT_SCHEDULER scheduler = null;
			decimal auditScheduleId = 0;
			string result = "<h3>EHS Audit Scheduler " + ((IsEditContext) ? "Updated" : "Created") + ":</h3>";

			if (shouldReturn == true)
			{
				divForm.Visible = false;

				pnlAddEdit.Visible = false;
				btnSaveReturn.Visible = false;

				RadCodeBlock rcbWarnNavigate = (RadCodeBlock)this.Parent.Parent.FindControl("rcbWarnNavigate");
				if (rcbWarnNavigate != null)
					rcbWarnNavigate.Visible = false;

				lblResults.Visible = true;
			}

			if (!IsEditContext)
			{
				auditTypeId = SelectedTypeId;
				auditType = rddlAuditType.SelectedText;
			}
			else
			{
				auditTypeId = EditAuditTypeId;
				auditType = EHSAuditMgr.SelectAuditTypeByAuditScheduleId(EditAuditScheduleId);
			}

			if (!IsEditContext)
			{
				// Add context
				scheduler = CreateNewAuditScheduler();
				auditScheduleId = scheduler.AUDIT_SCHEDULER_ID;
			}
			else
			{
				// Edit context
				auditScheduleId = EditAuditScheduleId;
				if (auditScheduleId > 0)
				{
					scheduler = UpdateAuditScheduler(auditScheduleId);
				}
			}
			if (shouldReturn)
			{
				SessionManager.ReturnStatus = false;
				SessionManager.ReturnObject = "DisplayAuditSchedules";
				Response.Redirect("/EHS/EHS_Audit_Scheduler.aspx");  // mt - temporary
			}

		}

		#endregion


		#region Save Methods

		protected AUDIT_SCHEDULER CreateNewAuditScheduler()
		{
			decimal auditScheduleId = 0;
			PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			var newAuditScheduler = new AUDIT_SCHEDULER()
			{
				DAY_OF_WEEK = Convert.ToInt32(rddlDayOfWeek.SelectedValue.ToString()),
				INACTIVE = false,
				JOBCODE_CD = rddlAuditJobcodes.SelectedValue.ToString(),
				PLANT_ID = auditPlant.PLANT_ID,
				CREATE_DT = DateTime.Now,
				CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID,
				AUDIT_TYPE_ID = auditTypeId
			};
			entities.AddToAUDIT_SCHEDULER(newAuditScheduler);
			entities.SaveChanges();
			auditScheduleId = newAuditScheduler.AUDIT_SCHEDULER_ID;

			return newAuditScheduler;
		}

		protected AUDIT_SCHEDULER UpdateAuditScheduler(decimal auditScheduleId)
		{
			AUDIT_SCHEDULER scheduler = (from i in entities.AUDIT_SCHEDULER where i.AUDIT_SCHEDULER_ID == auditScheduleId select i).FirstOrDefault();
			PLANT auditPlant = SQMModelMgr.LookupPlant(Convert.ToDecimal(hdnAuditLocation.Value.ToString()));
			if (scheduler != null)
			{
				scheduler.PLANT_ID = auditPlant.PLANT_ID;
				scheduler.AUDIT_TYPE_ID = Convert.ToDecimal(rddlAuditType.SelectedValue.ToString());
				scheduler.DAY_OF_WEEK = Convert.ToInt32(rddlDayOfWeek.SelectedValue.ToString());
				scheduler.JOBCODE_CD = rddlAuditJobcodes.SelectedValue.ToString();
				scheduler.INACTIVE = cbInactive.Checked;
				scheduler.UPDATE_DT = DateTime.Now;
				scheduler.UPDATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
				entities.SaveChanges();
			}

			return scheduler;

		}

		protected void GoToNextStep(decimal auditId)
		{
			// Go to next step (report)
			SessionManager.ReturnStatus = true;
			SessionManager.ReturnObject = "Report";
			SessionManager.ReturnRecordID = auditId;
		}

		protected void ShowAuditDetails(decimal auditId, string result)
		{
			rddlAuditType.SelectedIndex = 0;

			// Display audit details control
			btnDelete.Visible = false;
			lblResults.Text = result.ToString();
			var displaySteps = new int[] { CurrentStep };
		}

		#endregion


		public void ClearControls()
		{
			pnlForm.Controls.Clear();
		}

		protected void RefreshPageContext()
		{
			string typeString = "";
			if (accessLevel > AccessMode.View && CurrentStep == 0)
			{
				pnlAddEdit.Visible = true;
				if (!IsEditContext)
				{
					// Add
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;
					rddlAuditType.Visible = (rddlAuditType.Items.Count == 1) ? false : true;
					lblAddOrEditAudit.Text = "<strong>Add a New Audit Schedule:</strong>";

					btnDelete.Visible = false;
				}
				else
				{
					// Edit
					typeString = " Audit Schedule";
					btnSaveReturn.CommandArgument = "0";
					SelectedTypeId = 0;
					btnSaveReturn.Enabled = true;
					btnSaveReturn.Visible = true;

					lblAddOrEditAudit.Text = "<strong>Editing " + WebSiteCommon.FormatID(EditAuditScheduleId, 6) + typeString + "</strong><br/>";

					btnDelete.Visible = true;
					LoadInformation();
				}

				UpdateButtonText();

				// Only admin and higher can delete audits
				if (accessLevel < AccessMode.Admin)
					btnDelete.Visible = false;
			}
			else
			{
				// View only
				typeString = " Audit Schedule";
				SelectedTypeId = 0;
				btnSaveReturn.Enabled = false;
				btnSaveReturn.Visible = false;

				lblAddOrEditAudit.Text = "<strong>" + WebSiteCommon.FormatID(EditAuditScheduleId, 6) + typeString + " Closed</strong><br/>";

				rddlAuditType.Visible = false;
				btnDelete.Visible = false;
				LoadInformation();
				var displaySteps = new int[] { CurrentStep };
				pnlAddEdit.Visible = false;
			}

		}

		List<decimal> SelectPlantIdsByAccessLevel()
		{
			List<decimal> plantIdList = new List<decimal>();

			//accessLevel = UserContext.CheckAccess("EHS", "312");
			accessLevel = UserContext.CheckAccess("EHS", "");

			if (accessLevel >= AccessMode.Admin)
			{
				plantIdList = EHSAuditMgr.SelectPlantIdsByCompanyId(companyId);
			}
			else
			{
				plantIdList = SessionManager.UserContext.PlantAccessList;
				if (plantIdList == null || plantIdList.Count == 0)
				{
					plantIdList.Add(SessionManager.UserContext.HRLocation.Plant.PLANT_ID);
				}
			}

			return plantIdList;
		}

	}
}