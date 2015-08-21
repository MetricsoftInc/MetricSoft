using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQM.Shared;
using Telerik.Web.UI;
namespace SQM.Website
{
	public partial class Ucl_PersonSearch : System.Web.UI.UserControl
	{
		static PSsqmEntities _entities;
		//static B2BPartner _searchMgr;
		protected decimal companyId;
		protected decimal plantId;

		public event EditItemClick OnSearchItemSelect;

		public string Text
		{
			get { return rsbPart.Text; }
		}

		public RadSearchBox PartTextBox
		{
			get { return rsbPart; }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			uclPartList.OnPartClick += OnPartListClick;
		}

		protected void OnPartListClick(decimal partID)
		{
			if (partID > 0)
			{
				PART part = SQMModelMgr.LookupPart(_entities, partID, "", 0, false);
				rsbPart.Text = part.PART_NUM;
				if (OnSearchItemSelect != null)
				{
					OnSearchItemSelect(partID.ToString());
				}
			}
		}

		protected void OnSearchServer(object sender, SearchBoxEventArgs e)
		{
			if (OnSearchItemSelect != null)
			{
				string[] split = e.Text.Split('|');
				if (split.Length > 0)
					OnSearchItemSelect(split[0]);
			}
		}

		protected void OnOpenPartListWindow_Click(object sender, EventArgs e)
		{
			if (hfPartNumberPerspective.Value == "CST")
			{
				uclPartList.BindPartList(SQMModelMgr.SelectPartDataList(_entities, companyId, 0, 0, plantId, 1).Where(l=> l.Part.STATUS != "I").ToList());
			}
			else
			{
				uclPartList.BindPartList(SQMModelMgr.SelectPartDataList(_entities, companyId, 0, 0, plantId, 2).Where(l => l.Part.STATUS != "I").ToList());
			}
			string script = "function f(){OpenPartListWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{

			}
			//if (rddlLocation.Items.Count > 0)
			SetupPage();
		}
	   
		public void Initialize(string partText, bool enableProgram, bool enableLocation, bool enableStatus, string partNumberPerspective)
		{
			_entities = new PSsqmEntities();
			companyId = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;

			var pid = SessionManager.UserContext.WorkingLocation.Company.PLANT;
			//_searchMgr = new B2BPartner().Initialize();
			//_searchMgr.CompanyID = SessionManager.SessionContext.PrimaryCompany.COMPANY_ID;
			//hfPartNumberPerspective.Value = partNumberPerspective;   // show customer part number or internal part number

			//if (SessionManager.B2BLocation != null)
			//{
			//	_searchMgr.BusorgID = SessionManager.B2BLocation.BusinessOrg.BUS_ORG_ID;
			//	if (SessionManager.B2BLocation.Plant != null)
			//		_searchMgr.PlantID = SessionManager.B2BLocation.Plant.PLANT_ID;

			//	decimal customerID = SessionManager.SessionContext.PrimaryCompany.COMPANY_ID;
			//	if (SessionManager.B2BLocation.IsCustomerCompany(true))     // show part programs for user's customer location
			//		customerID = SessionManager.B2BLocation.Company.COMPANY_ID;

				//rddlProgram.DataSource = SQMModelMgr.SelectPartProgramList(customerID, 0);
				//rddlProgram.DataTextField = "PROGRAM_NAME";
				//rddlProgram.DataValueField = "PROGRAM_ID";
				//rddlProgram.DataBind();
				//rddlProgram.Items.Insert(0, new DropDownListItem("Any Program", "0"));

				// Bind location dropdown
				//if (enableLocation)
				//{
					//rddlLocation.DataSource = SQMModelMgr.SelectPlantList(_entities, _searchMgr.CompanyID, _searchMgr.BusorgID);
					//rddlLocation.DataTextField = "PLANT_NAME";
					//rddlLocation.DataValueField = "PLANT_ID";
					//rddlLocation.DataBind();
					//rddlLocation.Items.Insert(0, new DropDownListItem("Any Location", "0"));
					//rddlLocation.SelectedValue = _searchMgr.PlantID.ToString();
				//}
				//else
				//{
					//rddlLocation.DataSource = new List<PLANT>() { SessionManager.B2BLocation.Plant };
					//rddlLocation.DataTextField = "PLANT_NAME";
					//rddlLocation.DataValueField = "PLANT_ID";
					//rddlLocation.DataBind();
					//rddlLocation.SelectedIndex = 0;
				//}

				//rddlProgram.Visible = enableProgram;
				//rddlLocation.Visible = enableLocation;
				//rddlStatus.Visible = enableStatus;
				if (!string.IsNullOrEmpty(partText))
					rsbPart.Text = partText;

				SetupPage();
			//}
		}

		void SetupPage ()
		{
			//if (_searchMgr == null)
			//{
			//	_searchMgr = new B2BPartner().Initialize();
			//	_entities = new PSsqmEntities();
			//}

			//int programId = 0;
			//if (!string.IsNullOrEmpty(rddlProgram.SelectedValue))
			//	programId = Convert.ToInt32(rddlProgram.SelectedValue);

			//if (!string.IsNullOrEmpty(rddlLocation.SelectedValue))
			//	_searchMgr.PlantID = Convert.ToInt32(rddlLocation.SelectedValue);

			//string status = rddlStatus.SelectedValue;

			//rddlProgram.SelectedValue = programId.ToString();

			// Bind search box

			//_searchMgr.CompanyID = SessionManager.SessionContext.PrimaryCompany.COMPANY_ID;
			try
			{
				//if (hfPartNumberPerspective.Value == "CST")
				//{
				//	var partDataList = SQMModelMgr.SelectPartDataList(_entities, _searchMgr.CompanyID, 0, programId, _searchMgr.PlantID, 1);
				//	var modifiedPartList = (from pdl in partDataList
				//							select new
				//							{
				//								PART_NUM = !string.IsNullOrEmpty(pdl.Part.DRAWING_REF) ? pdl.Part.DRAWING_REF : pdl.Part.PART_NUM,
				//								PART_NUM_SEPARATOR = pdl.Part.PART_NUM_SEPARATOR,
				//								PART_PREFIX = pdl.Part.PART_PREFIX,
				//								PART_SUFFIX = pdl.Part.PART_SUFFIX,
				//								PART_NAME = pdl.Part.PART_NAME,
				//								PROGRAM_NAME = (pdl.Part.PART_PROGRAM != null) ? pdl.Part.PART_PROGRAM.PROGRAM_NAME : "",
				//								REVISION_LEVEL = pdl.Part.REVISION_LEVEL,
				//								STATUS = pdl.Part.STATUS,
				//								SEARCH_AGGREGATE = pdl.Part.PART_ID.ToString() + "|" + (!string.IsNullOrEmpty(pdl.Part.DRAWING_REF) ? pdl.Part.DRAWING_REF : pdl.Part.PART_NUM) + "|" + pdl.Part.PART_NAME + "|" + pdl.Part.PART_PREFIX + "|" + pdl.Part.PART_NUM_SEPARATOR + "|" +
				//									pdl.Part.REVISION_LEVEL + "|" + ((pdl.Part.PART_PROGRAM != null) ? pdl.Part.PART_PROGRAM.PROGRAM_NAME : ""),
				//								FULL_PART_NUM = pdl.Part.PART_PREFIX + pdl.Part.PART_NUM_SEPARATOR + pdl.Part.PART_NUM
				//							}).Distinct();

				//	//if (status == "active")
				//	//	modifiedPartList = (from p in modifiedPartList where p.STATUS.Trim().ToLower() == "a" select p).ToList();

				//	rsbPart.DataSource = modifiedPartList;
				//}
				//else
				//{
				var partDataList = SQMModelMgr.SelectPartDataList(_entities, companyId, 0, 0, plantId, 2);
					var modifiedPartList = (from pdl in partDataList
											select new
											{
												PART_NUM = !string.IsNullOrEmpty(pdl.Part.DRAWING_REF) ? pdl.Part.DRAWING_REF : pdl.Part.PART_NUM,
												PART_NUM_SEPARATOR = pdl.Part.PART_NUM_SEPARATOR,
												PART_PREFIX = pdl.Part.PART_PREFIX,
												PART_SUFFIX = pdl.Part.PART_SUFFIX,
												PART_NAME = pdl.Part.PART_NAME,
												PROGRAM_NAME = (pdl.Part.PART_PROGRAM != null) ? pdl.Part.PART_PROGRAM.PROGRAM_NAME : "",
												REVISION_LEVEL = pdl.Part.REVISION_LEVEL,
												STATUS = pdl.Part.STATUS,
												SEARCH_AGGREGATE = pdl.Part.PART_ID.ToString() + "|" + (!string.IsNullOrEmpty(pdl.Part.DRAWING_REF) ? pdl.Part.DRAWING_REF : pdl.Part.PART_NUM) + "|" + pdl.Part.PART_NAME + "|" + pdl.Part.PART_PREFIX + "|" + pdl.Part.PART_NUM_SEPARATOR + "|" +
													pdl.Part.REVISION_LEVEL + "|" + ((pdl.Part.PART_PROGRAM != null) ? pdl.Part.PART_PROGRAM.PROGRAM_NAME : ""),
												FULL_PART_NUM = pdl.Part.PART_PREFIX + pdl.Part.PART_NUM_SEPARATOR + pdl.Part.PART_NUM
											}).Distinct();

					//if (status == "active")
					//	modifiedPartList = (from p in modifiedPartList where p.STATUS.Trim().ToLower() == "a" select p).ToList();

					rsbPart.DataSource = modifiedPartList;
				//}

				rsbPart.Filter = Telerik.Web.UI.SearchBoxFilter.Contains;
				rsbPart.DataTextField = "SEARCH_AGGREGATE";
				rsbPart.DataBind();
			}
			catch
			{
			}
		}

		public string ReturnProgramName(string programName)
		{
			string programString = "<span style=\"color: #c00;\">-</span>";
			if (!string.IsNullOrEmpty(programName))
				programString = programName;
			return programString;
		}

		public string ReturnActiveStatus(string status)
		{
			string activeString = "";
			activeString = (status.ToLower().Trim() == "a") ?
				"<span style=\"color: #4c0;\">&#10004;</span>" :
				"<span style=\"color: #c00;\">-</span>";
			return activeString;
		}

		protected void rsbPart_OnButtonCommand(object sender, SearchBoxButtonEventArgs e)
		{
			string[] split = rsbPart.Text.Split('|');
			if (split.Length > 1)
				rsbPart.Text = split[1];
		}

		protected void rsbPart_Search(object sender, SearchBoxEventArgs e)
		{
			string[] split = rsbPart.Text.Split('|');
			if (split.Length > 1)
				rsbPart.Text = split[1];
		}
	
	}
}
