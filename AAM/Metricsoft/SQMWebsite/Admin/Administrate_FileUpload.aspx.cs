using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Telerik.Web.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.XSSF.UserModel;
using Telerik.Web.UI;

namespace SQM.Website
{
    public partial class Administrate_FileUpload : SQMBasePage
    {
        static string selectedFile;
        static SQMFileReader fileReader;
		static char[] fileDelimiter;
		static double plantDataMultiplier;

		HSSFWorkbook hssfworkbook;
		XSSFWorkbook xssfworkbook;
		IWorkbook wb;

		DataSet dsCurrency;
		int periodYear;
		int periodMonth;


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                tbFileSelected.Text = selectedFile;
            }
            else
            {
                SetupPage();
            }
        }

        private void SetupPage()
        {
            selectedFile = "";
            btnUpload.Enabled = false;
            btnPreview.Enabled = false;
            gvErrorList.Visible = gvUpdateList.Visible = false;
            lblSummaryList.Visible = false;
            fileReader = null;
        }

        public void gvList_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                }
                catch
                {
                }
            }
        }

        private void SetStatusMessage(string msg)
        {
            gvErrorList.Visible = gvUpdateList.Visible = false;
            lblSummaryList.Text = msg;
            lblSummaryList.Visible = true;
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            byte[] fileContent;
			if (ddlDataType.SelectedValue.Equals(""))
			{
				SetStatusMessage("You must select a data type from the drop down list");
			}
			else
			{
				// validate the date in the select control
				periodYear = 0;
				periodMonth = 0;

                List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", ""); // ABW 20140805

                int primaryCompany = Convert.ToInt32(sets.Find(x => x.SETTING_CD == "CompanyID").VALUE);
				PSsqmEntities Entities = new PSsqmEntities();
				string fileName = "";
				lblSummaryList.Visible = false;

				if (flUpload.HasFile)
				{
					btnUpload.Enabled = false;

					selectedFile = tbFileSelected.Text = flUpload.FileName;
					int fileLen = flUpload.PostedFile.ContentLength;

					if (fileLen < 5)
					{
						SetStatusMessage("The file does not contain relevant data: " + flUpload.FileName);
						return;
					}
                    fileDelimiter = sets.Find(x => x.SETTING_CD == "FileDelimiter1").VALUE.ToCharArray(); 
					plantDataMultiplier = 1;
					
					fileName = ddlDataType.SelectedValue.ToString() + ".TXT";
					try
					{
						string fileType = flUpload.PostedFile.ContentType;
						fileContent = new byte[Convert.ToInt32(fileLen)];
						int nBytes = flUpload.PostedFile.InputStream.Read(fileContent, 0, Convert.ToInt32(fileLen));
					}
					catch
					{
						SetStatusMessage("Error encountered opening or acquiring the file: " + flUpload.FileName);
						return;
					}
					//fileReader = new SQMFileReader().InitializeCSV(1, flUpload.PostedFile.FileName, fileContent, fileDelimiter, plantDataMultiplier);
					fileReader = new SQMFileReader().InitializeCSV(primaryCompany, fileName, fileContent, fileDelimiter, plantDataMultiplier, periodYear, periodMonth, "USD");
                  
                    if (fileReader.Status < 0)
					{
						SetStatusMessage("Error encountered loading the file: " + flUpload.FileName);
						return;
					}
				
					ProcessFile();

				}
				else
				{
					SetStatusMessage("The selected file is empty: " + flUpload.FileName);
				}
			}
        }

        protected void ProcessFile()
        {
			btnUpload.Enabled = false;
			btnPreview.Enabled = false;

		    fileReader.LoadCSV();

			if (fileReader.UpdateList.Count > 0)
			{
				gvUpdateList.DataSource = fileReader.UpdateList;
				gvUpdateList.DataBind();
				gvUpdateList.Visible = true;
			}

			if (fileReader.ErrorList.Count > 0)
			{
				gvErrorList.DataSource = fileReader.ErrorList;
				gvErrorList.DataBind();
				gvErrorList.Visible = true;
			}
		}

		protected void PreviewFile(object sender, EventArgs e)
		{
			byte[] fileContent;
			if (ddlDataType.SelectedValue.Equals(""))
			{
				SetStatusMessage("You must select a file type from the drop down list");
			}
			else
			{
				// validate the date in the select control
				periodYear = 0;
				periodMonth = 0;

                List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", ""); // ABW 20140805
				
				string fileName = ddlDataType.SelectedValue.ToString() + ".TXT";
                int primaryCompany = Convert.ToInt32(sets.Find(x => x.SETTING_CD == "CompanyID").VALUE);
				PSsqmEntities Entities = new PSsqmEntities();
				BUSINESS_ORG busOrg = SQMModelMgr.LookupBusOrg(Entities, primaryCompany, "", true, false);
				selectedFile = tbFileSelected.Text = flUpload.FileName;
				int fileLen = flUpload.PostedFile.ContentLength;
				
				fileContent = new byte[Convert.ToInt32(fileLen)];
				int nBytes = flUpload.PostedFile.InputStream.Read(fileContent, 0, Convert.ToInt32(fileLen));
                fileDelimiter = sets.Find(x => x.SETTING_CD == "FileDelimiter1").VALUE.ToCharArray(); 
				plantDataMultiplier = 1;
				
				//fileReader = new SQMFileReader().InitializeCSV(1, flUpload.PostedFile.FileName, fileContent, fileDelimiter, plantDataMultiplier);
				fileReader = new SQMFileReader().InitializeCSV(primaryCompany, fileName, fileContent, fileDelimiter, plantDataMultiplier, periodYear, periodMonth, "USD");
				using (StreamReader sr = new StreamReader(fileReader.FileStream))
				{
					string line;
					int lineNo = 0;

					while ((line = sr.ReadLine()) != null)
					{
						fileReader.PreviewList.Add(line);
					}
					gvPreview.DataSource = fileReader.PreviewList;
					gvPreview.DataBind();
					gvPreview.Visible = true;
					gvExcelPreview.Visible = false;
				}
			
			}
		}

        public void gvPreview_OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if ((!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Header.ToString())) & (!e.Row.RowType.ToString().Trim().Equals(System.Web.UI.WebControls.ListItemType.Footer.ToString())))
            {
                try
                {
                    Label lbl = (Label)e.Row.Cells[0].FindControl("lblLine");
						lbl.Text = e.Row.DataItem.ToString();
                }
                catch
                {
                }
            }
        }

		protected void PeriodMenuItemClick(Object sender, EventArgs e)
		{
			//RadMonthYearPicker dmSelect = (RadMonthYearPicker)sender;
			//if (dmSelect.SelectedDate == null)
			//{
			//	return;
			//}
			//else
			//{
			//	btnCalculate.Enabled = true;
			//	if (OnProfilePeriodClick != null)
			//	{
			//		OnProfilePeriodClick(new DateTime(dmSelect.SelectedDate.Value.Year, dmSelect.SelectedDate.Value.Month, 1));
			//	}
			//}
			
		}

		void PreviewExcelFile()
		{
			ISheet sheet;
			if (flUpload.FileName.Contains(".xlsx"))
			{
				using (Stream file = flUpload.PostedFile.InputStream)
				{
					try
					{
						//hssfworkbook = new HSSFWorkbook(file);
						//sheet = hssfworkbook.GetSheetAt(0);
						xssfworkbook = new XSSFWorkbook(file);
						sheet = xssfworkbook.GetSheetAt(0);
					}
					catch
					{
						return;
					}
				}
			}
			else
			{
				using (Stream file = flUpload.PostedFile.InputStream)
				{
					try
					{
						hssfworkbook = new HSSFWorkbook(file);
						sheet = hssfworkbook.GetSheetAt(0);
					}
					catch
					{
						return;
					}
				}
			}
			IRow row;
			System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

			bool addHeader = true;
			DataTable dt = new DataTable();

			while (rows.MoveNext())
			{
				if (flUpload.FileName.Contains(".xlsx"))
					row = (XSSFRow)rows.Current;
				else
					row = (HSSFRow)rows.Current;

				if (addHeader)
				{
					int iRows = row.Cells.Count;
					for (int j = 0; j < iRows; j++)
					{
						dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
					}
					addHeader = false;
				}

				DataRow dr = dt.NewRow();

				for (int i = 0; i < row.LastCellNum; i++)
				{
					ICell cell = row.GetCell(i);

					if (cell == null)
					{
						dr[i] = null;
					}
					else
					{
						dr[i] = cell.ToString();
					}
				}
				dt.Rows.Add(dr);
			}
			dsCurrency = new DataSet();
			dsCurrency.Tables.Add(dt);
		}
	}
}