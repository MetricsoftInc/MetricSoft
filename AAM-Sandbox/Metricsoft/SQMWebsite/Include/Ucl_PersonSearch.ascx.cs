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
	//public delegate void EditPersonClick(string cmd);

	public partial class Ucl_PersonSearch : System.Web.UI.UserControl 
	{
		static PSsqmEntities _entities;
	
		public List<PersonData> personslist;

		public decimal PlantID
		{
			get { return ViewState["PlantID"] == null ? 0 : (decimal)ViewState["PlantID"]; }
			set { ViewState["PlantID"] = value; }
		}


		public decimal SelectedPersonID
		{
			get { return ViewState["SelectedPersonID"] == null ? 0 : (decimal)ViewState["SelectedPersonID"]; }
			set { ViewState["SelectedPersonID"] = value; }
		}

		public event EditItemClick OnSearchItemSelect;


		public string Text
		{
			get { return rsbPerson.Text; }
		}


		public RadSearchBox PersonTextBox
		{
			get { return rsbPerson; }
		}

		public bool Access
		{
			get { return ViewState["Access"] == null ? false : (bool)ViewState["Access"]; }
			set { ViewState["Access"] = value; }
		}


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			//uclPersonList.OnPersonClick += OnPersonListClick;

		}

		protected void OnPersonListClick(decimal personID)
		{
			if (personID > 0)
			{
				PERSON person = SQMModelMgr.LookupPerson(personID, "");
				rsbPerson.Text = string.Format("{0}, {1}", person.LAST_NAME, person.FIRST_NAME);
				if (OnSearchItemSelect != null)
				{
					OnSearchItemSelect(personID.ToString());
				}
			}
		}

		protected void OnSearchServer(object sender, SearchBoxEventArgs e)
		{
			string pname = " ";

			if (OnSearchItemSelect != null)
			{
				if (e.Value != null)
				{
					SelectedPersonID = Convert.ToDecimal(e.Value.ToString());

					pname = e.Text;
				}

				OnSearchItemSelect(pname);
			}

		}

		protected void OnOpenPersonListWindow_Click(object sender, EventArgs e)
		{
			Page page = HttpContext.Current.CurrentHandler as Page;

			//if (hfPartNumberPerspective.Value == "CST")
			//{
				//uclPartList.BindPartList(SQMModelMgr.SelectPartDataList(_entities, companyId, 0, 0, plantId, 1).Where(l=> l.Part.STATUS != "I").ToList());
			//}
			//else
			//{
//			uclPersonList.BindPersonList(SQMModelMgr.SelectPlantPersonDataList(CompanyID, PlantID));
			//}
//			winPersonList.Visible = true;
			//string script = "function f(){OpenPersonListWindow(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
			//ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{

			}

			PlantID = SessionManager.IncidentLocation.Plant.PLANT_ID;

			BindDataSource(rsbPerson);

			}

		//}

		void BindDataSource(RadSearchBox searchBox)
		{



			var companyID = SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID;
			var personDataList = SQMModelMgr.SelectPlantPersonDataList(companyID, PlantID);

			searchBox.DataSource = personDataList;

			searchBox.DataValueField = "PersonID";
			searchBox.DataTextField = "PersonName";
			searchBox.DataBind();
			searchBox.EnableAutoComplete = true;

			searchBox.Enabled = Access;

		}

		public void Initialize(string personText)
		{



			if (!string.IsNullOrEmpty(personText))
				rsbPerson.Text = personText;
		}

		//public string ReturnProgramName(string programName)
		//{
		//	string programString = "<span style=\"color: #c00;\">-</span>";
		//	if (!string.IsNullOrEmpty(programName))
		//		programString = programName;
		//	return programString;
		//}

		//public string ReturnActiveStatus(string status)
		//{
		//	string activeString = "";
		//	activeString = (status.ToLower().Trim() == "a") ?
		//		"<span style=\"color: #4c0;\">&#10004;</span>" :
		//		"<span style=\"color: #c00;\">-</span>";
		//	return activeString;
		//}

		protected void rsbPerson_OnButtonCommand(object sender, SearchBoxButtonEventArgs e)
		{
			string[] split = rsbPerson.Text.Split('|');
			if (split.Length > 1)
				rsbPerson.Text = split[1];
		}

		protected void rsbPerson_Search(object sender, SearchBoxEventArgs e)
		{
			string[] split = rsbPerson.Text.Split('|');
			if (split.Length > 1)
				rsbPerson.Text = split[1];
		}
	
	}
}
