using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;
using System.Text;
using System.Web;
using System.Globalization;
using System.Threading;

namespace SQM.Website
{
	public partial class Ucl_INCFORM_Root5Y : System.Web.UI.UserControl
	{

		const Int32 MaxTextLength = 4000;

		protected decimal companyId;

		protected int totalFormSteps;

		protected decimal incidentTypeId;
		protected string incidentType;
		protected bool IsFullPagePostback = false;

		protected int currentProblemSeries = 0;
		protected int currentItemSeq = 0;

		public PageUseMode PageMode { get; set; }

		PSsqmEntities entities;
		List<EHSFormControlStep> formSteps;


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

		public decimal IncidentId
		{
			get { return ViewState["EditIncidentId"] == null ? 0 : (decimal)ViewState["EditIncidentId"]; }
			set { ViewState["EditIncidentId"] = value; }
		}
		protected string IncidentLocationTZ
		{
			get { return ViewState["IncidentLocationTZ"] == null ? "GMT" : (string)ViewState["IncidentLocationTZ"]; }
			set { ViewState["IncidentLocationTZ"] = value; }
		}

		protected decimal EditIncidentTypeId
		{
			get { return IncidentId == null ? 0 : EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(IncidentId); }
		}

		public INCIDENT LocalIncident
		{
			get { return ViewState["LocalIncident"] == null ? null : (INCIDENT)ViewState["LocalIncident"]; }
			set { ViewState["LocalIncident"] = value; }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			if (SessionManager.SessionContext != null)
			{
				if (IsFullPagePostback)
					rptRootCause.DataBind();
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
							(this.Page.FindControl(targetID).ID == "btnAddStatement"))
								IsFullPagePostback = true;
				}
			}
		}


		protected override void FrameworkInitialize()
		{
			if (SessionManager.SessionContext != null)
			{
				String selectedLanguage = SessionManager.SessionContext.Language().NLS_LANGUAGE;
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

				base.FrameworkInitialize();
			}
		}

		public void PopulateInitialForm()
		{
			PSsqmEntities entities = new PSsqmEntities();
			decimal typeId = (IsEditContext) ? EditIncidentTypeId : SelectedTypeId;

			if (IncidentId > 0)
				try
				{
					LocalIncident = (from i in entities.INCIDENT where i.INCIDENT_ID == IncidentId select i).SingleOrDefault();
					PLANT plant = SQMModelMgr.LookupPlant(entities, (decimal)LocalIncident.DETECT_PLANT_ID, "");
					if (plant != null)
						IncidentLocationTZ = plant.LOCAL_TIMEZONE;


					if (PageMode == PageUseMode.ViewOnly)
					{
						divTitle.Visible = true;
						lblFormTitle.Text = Resources.LocalizedText.RootCause;
					}

					pnlRoot5Y.Enabled = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.originate, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
				}
				catch { }

			InitializeForm();
		}

		void InitializeForm()
		{
			List<INCFORM_ROOT5Y> rootCauseList = EHSIncidentMgr.GetRootCauseList(IncidentId, false);

			rootCauseList = EHSIncidentMgr.FormatRootCauseList(LocalIncident, rootCauseList);

			rptRootCause.DataSource = rootCauseList.OrderBy(l => l.PROBLEM_SERIES).ThenBy(l => l.ITEM_SEQ).ToList();
			rptRootCause.DataBind();
		}


		protected int RootCauseLevels()
		{
			// determine if multiple level root cause (problem statment + causes)
			int levels = 1;

			SETTINGS setting = SessionManager.GetUserSetting("EHS", "ROOTCAUSE_LEVELS"); //SessionManager.GetUserSetting("EHS", "ROOTCAUSE_LEVELS"); // EHSSettings.Where(s => s.SETTING_CD == "ROOTCAUSE_LEVELS").FirstOrDefault();
			if (setting != null)
			{
				int.TryParse(setting.VALUE, out levels);
			}

			return levels;
		}

		public void rptRootCause_OnItemDataBound(object sender, RepeaterItemEventArgs e)
		{

			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				try
				{
					INCFORM_ROOT5Y rootCause = (INCFORM_ROOT5Y)e.Item.DataItem;

					RadButton itmdel = (RadButton)e.Item.FindControl("btnItemDelete");

					TextBox tb = (TextBox)e.Item.FindControl("tbRootCause");
					Label lp = (Label)e.Item.FindControl("lblProblemStatement");
					tb.Text = lp.Text = rootCause.ITEM_DESCRIPTION;
					HiddenField hfSeq = (HiddenField)e.Item.FindControl("hfItemSeq");

					hfSeq.Value = rootCause.ITEM_SEQ.ToString();
					HiddenField hf = (HiddenField)e.Item.FindControl("hfItemType");
					hf.Value = rootCause.ITEM_TYPE.HasValue ? rootCause.ITEM_TYPE.ToString() : "0";
					hf = (HiddenField)e.Item.FindControl("hfProblemSeries");
					hf.Value = rootCause.PROBLEM_SERIES.HasValue ? rootCause.PROBLEM_SERIES.ToString() : "0";

					Label lb = (Label)e.Item.FindControl("lbWhyPrompt");
					if (rootCause.ITEM_TYPE == 1)		// problem statement
					{
						tb.Visible = true;
						lp.Visible = false;
						lb.Text = Resources.LocalizedText.ProblemStatement;
						Image img = (Image)e.Item.FindControl("imgProblem");
						img.ImageUrl = "~/images/defaulticon/16x16/alert-alt.png";
						//img.Visible = true;
						Panel pnl = (Panel)e.Item.FindControl("pnlIsRootCause");
						pnl.Visible = false;
						System.Web.UI.HtmlControls.HtmlGenericControl div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divPrompt");
						//div.Style.Add("BACKGROUND-COLOR", "#FFFFE0");
						div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("divRootCause");
						tb.Style.Add("BACKGROUND-COLOR", "#FFFFE0");
						if (rootCause.PROBLEM_SERIES < 1)
						{
							itmdel.Visible = false;	// don't allow deleting 1st problem series
						}
						Button btn = (Button)e.Item.FindControl("btnAddRootCause");
						if (PageMode == PageUseMode.ViewOnly)
							btn.Visible = false;
					}
					else
					{
						tb.Visible = true;			// 'why' cause
						lp.Visible = false;
						lb = (Label)e.Item.FindControl("lbItemSeq");
						if (currentProblemSeries != rootCause.PROBLEM_SERIES)
						{
							currentProblemSeries = rootCause.PROBLEM_SERIES.HasValue ? (int)rootCause.PROBLEM_SERIES : 0;
							currentItemSeq = 0;
						}
						lb.Text = (++currentItemSeq).ToString();
						Button btn = (Button)e.Item.FindControl("btnAddRootCause");
						btn.Visible = false;

						if (rootCause.IS_ROOTCAUSE.HasValue && (bool)rootCause.IS_ROOTCAUSE == true)
						{
							CheckBox cb = (CheckBox)e.Item.FindControl("cbIsRootCause");
							cb.Checked = true;
						}
					}

					if (itmdel.Visible)
						itmdel.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.action, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
				}
				catch { }
			}

			if (e.Item.ItemType == ListItemType.Footer)
			{
				Button btnAdd = (Button)e.Item.FindControl("btnAddStatement");
				RadButton btnSave = (RadButton)e.Item.FindControl("btnSave");
				btnSave.Visible = btnAdd.Visible = PageMode == PageUseMode.ViewOnly ? false : EHSIncidentMgr.CanUpdateIncident(LocalIncident, IsEditContext, SysPriv.action, LocalIncident.INCFORM_LAST_STEP_COMPLETED);
				if (RootCauseLevels() < 2)
					btnAdd.Visible = false;
			}

			if (e.Item.ItemType == ListItemType.Header)
			{
				Label lp = (Label)e.Item.FindControl("lblProblemDesc");
				lp.Text = LocalIncident.DESCRIPTION;
			}
		}

		public int AddUpdateINCFORM_ROOT5Y(decimal incidentId)
		{

			var itemList = new List<INCFORM_ROOT5Y>();
			int seqnumber = 0;
			int status = 0;

			foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
			{
				var item = new INCFORM_ROOT5Y();

				TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
				HiddenField hfSeq = (HiddenField)rootcauseitem.FindControl("hfItemSeq");
				HiddenField hf = (HiddenField)rootcauseitem.FindControl("hfItemType");
				CheckBox cb = (CheckBox)rootcauseitem.FindControl("cbIsRootCause");


				if (!String.IsNullOrEmpty(tb.Text))
				{
					item.ITEM_TYPE = Convert.ToInt32(hf.Value);
					hf = (HiddenField)rootcauseitem.FindControl("hfProblemSeries");
					item.PROBLEM_SERIES = Convert.ToInt32(hf.Value);
					item.IS_ROOTCAUSE = cb.Checked;

					seqnumber = seqnumber + 1;

					item.ITEM_DESCRIPTION = tb.Text;
					item.ITEM_SEQ = seqnumber;

					itemList.Add(item);
				}
			}

			status = SaveRootCauses(incidentId, itemList);
			return status;
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			AddUpdateINCFORM_ROOT5Y(LocalIncident.INCIDENT_ID);
			string script = string.Format("alert('{0}');", Resources.LocalizedText.SaveSuccess);
			ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", script, true);
			InitializeForm();
		}

		protected int SaveRootCauses(decimal incidentId, List<INCFORM_ROOT5Y> itemList)
		{

			PSsqmEntities entities = new PSsqmEntities();
			int status = 0;

			using (var ctx = new PSsqmEntities())
			{
				ctx.ExecuteStoreCommand("DELETE FROM INCFORM_ROOT5Y WHERE INCIDENT_ID = {0}", incidentId);
			}

			int seq = 0;

			foreach (INCFORM_ROOT5Y item in itemList)
			{
				var newItem = new INCFORM_ROOT5Y();

				if (!string.IsNullOrEmpty(item.ITEM_DESCRIPTION))
				{
					seq = seq + 1;

					newItem.INCIDENT_ID = incidentId;
					newItem.ITEM_SEQ = seq;
					newItem.ITEM_TYPE = item.ITEM_TYPE;
					newItem.PROBLEM_SERIES = item.PROBLEM_SERIES;
					newItem.ITEM_DESCRIPTION = item.ITEM_DESCRIPTION;
					newItem.IS_ROOTCAUSE = item.IS_ROOTCAUSE;
					newItem.LAST_UPD_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
					newItem.LAST_UPD_DT = WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ);

					entities.AddToINCFORM_ROOT5Y(newItem);
					status = entities.SaveChanges();
				}
			}

			if (seq > 0)
			{
				EHSIncidentMgr.UpdateIncidentStatus(incidentId, IncidentStepStatus.rootcause, WebSiteCommon.LocalTime(DateTime.UtcNow, IncidentLocationTZ));
			}

			return status;
		}


		protected void rptRootCause_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int itemId = e.Item.ItemIndex;

			if (e.CommandArgument == "AddAnother"  ||  e.CommandArgument == "AddStatement")
			{

				var itemList = new List<INCFORM_ROOT5Y>();
				int seqnumber = 0;
				int maxProblemSeries = -1;
				int problemSeries = -1;
				string defaultProblemStatement = "";

				foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
				{
					INCFORM_ROOT5Y item = new INCFORM_ROOT5Y();

					TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
					HiddenField hfSeq = (HiddenField)rootcauseitem.FindControl("hfItemSeq");
					HiddenField hf = (HiddenField)rootcauseitem.FindControl("hfItemType");
					CheckBox cb = (CheckBox)rootcauseitem.FindControl("cbIsRootCause");

					item.ITEM_TYPE = Convert.ToInt32(hf.Value);
					hf = (HiddenField)rootcauseitem.FindControl("hfProblemSeries");
					item.PROBLEM_SERIES = Convert.ToInt32(hf.Value);

					seqnumber = Convert.ToInt32(hfSeq.Value);

					item.ITEM_DESCRIPTION = tb.Text;
					item.ITEM_SEQ = seqnumber;
					item.IS_ROOTCAUSE = cb.Checked;

					itemList.Add(item);

					// add problem series
					maxProblemSeries = Math.Max(maxProblemSeries, (int)item.PROBLEM_SERIES);

					if (seqnumber == itemId + 1)
					{
						// insert into problem series 
						problemSeries = (int)item.PROBLEM_SERIES;
					}

					if (item.ITEM_TYPE == 0)
						defaultProblemStatement = item.ITEM_DESCRIPTION;
				}

				INCFORM_ROOT5Y emptyItem = new INCFORM_ROOT5Y();

				if (e.CommandArgument == "AddStatement")
				{
					emptyItem.ITEM_SEQ = seqnumber + 1;
					emptyItem.ITEM_DESCRIPTION = defaultProblemStatement;
					emptyItem.ITEM_TYPE = 1;
					emptyItem.PROBLEM_SERIES = (problemSeries = ++maxProblemSeries);
					emptyItem.IS_ROOTCAUSE = false;
					itemList.Add(emptyItem);
				}

				emptyItem = new INCFORM_ROOT5Y();
				emptyItem.ITEM_SEQ = seqnumber + 1;
				emptyItem.ITEM_DESCRIPTION = "";
				emptyItem.ITEM_TYPE = 0;
				emptyItem.IS_ROOTCAUSE = false;
				emptyItem.PROBLEM_SERIES = Math.Max(problemSeries, 0);
				itemList.Add(emptyItem);

				rptRootCause.DataSource = itemList.OrderBy(l => l.PROBLEM_SERIES).ThenBy(l => l.ITEM_SEQ).ToList();
				rptRootCause.DataBind();

			}
			else if (e.CommandArgument.ToString() == "Delete")
			{
				var itemList = new List<INCFORM_ROOT5Y>();
				int seqnumber = 0;
				int problemSeries = -1;
				bool shouldDelete = false;

				foreach (RepeaterItem rootcauseitem in rptRootCause.Items)
				{
					var item = new INCFORM_ROOT5Y();

					TextBox tb = (TextBox)rootcauseitem.FindControl("tbRootCause");
					HiddenField hfSeq = (HiddenField)rootcauseitem.FindControl("hfItemSeq");
					HiddenField hfType = (HiddenField)rootcauseitem.FindControl("hfItemType");
					HiddenField hfSeries = (HiddenField)rootcauseitem.FindControl("hfProblemSeries");
					CheckBox cb = (CheckBox)rootcauseitem.FindControl("cbIsRootCause");

					if (Convert.ToInt32(hfSeries.Value) != problemSeries)
						shouldDelete = false;

					problemSeries = Convert.ToInt32(hfSeries.Value);
					if (Convert.ToInt32(hfSeq.Value) == itemId + 1  ||  shouldDelete)
					{
						if (hfType.Value == "1")	// deleting an entire problem series
						{
							shouldDelete = true;
						}
					}
					else
					{
						shouldDelete = false;
						seqnumber = seqnumber + 1;
						item.ITEM_DESCRIPTION = tb.Text;
						item.ITEM_SEQ = seqnumber;
						item.ITEM_TYPE = Convert.ToInt32(hfType.Value);
						item.IS_ROOTCAUSE = cb.Checked;
						item.PROBLEM_SERIES = problemSeries;

						itemList.Add(item);
					}
				}

				rptRootCause.DataSource = itemList.OrderBy(l=> l.PROBLEM_SERIES).ThenBy(l=> l.ITEM_SEQ).ToList();
				rptRootCause.DataBind();

				int status = SaveRootCauses(IncidentId, itemList);
			}
		}
	}
}