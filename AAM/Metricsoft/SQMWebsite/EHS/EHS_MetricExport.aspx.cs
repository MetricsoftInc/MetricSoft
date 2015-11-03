using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

namespace SQM.Website.EHS
{
	public partial class EHS_MetricExport : System.Web.UI.Page
	{
		HSSFWorkbook hssfworkbook;

		protected void Page_Load(object sender, EventArgs e)
		{
			string delimStr = "~";
			char[] delimiter = delimStr.ToCharArray();
			string[] exportParms = null;
			string[] plants = null;
			int loop1;
			exportParms = SessionManager.ExportCriteria.Split(delimiter);

			delimStr = ",";
			delimiter = null;
			delimiter = delimStr.ToCharArray();
			plants = exportParms[1].Split(delimiter);
			decimal[] plantIDs = new decimal[plants.Length];
			for (loop1 = 0; loop1 < plants.Length; loop1++)
			{
				plantIDs[loop1] = Convert.ToDecimal(plants[loop1]);
			}

			DateTime dtFrom = Convert.ToDateTime(exportParms[2]);
			DateTime dtTo = Convert.ToDateTime(exportParms[3]);
            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange);
			esMgr.LoadMetricHistory(plantIDs, dtFrom, dtTo, DateIntervalType.month, false);
			List<EHS_METRIC_HISTORY> metric_history = esMgr.MetricHst.OrderBy(l => l.PLANT).ThenBy(l => l.PERIOD_YEAR).ThenBy(l => l.PERIOD_MONTH).ThenBy(l => l.EHS_MEASURE.MEASURE_CATEGORY).ThenBy(l => l.EHS_MEASURE.MEASURE_CD).ToList();
			string uom_cd;
			string uom_input_cd;
			decimal uom_id = 0;
			// need to cycle thru and populate the UOM and other formatting
			try
			{
				string filename = exportParms[0].Trim() + ".xls";
				Response.ContentType = "application/vnd.ms-excel";
				Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
				Response.Clear();
				InitializeWorkbook();
				ISheet sheet1 = hssfworkbook.CreateSheet("Metric History");

				//// create the header - Plant Name, DUNS Code, Measure, Measure Name, Period Year, Period Month, Period from Date, Period To Date, Value, UOM, UOM Name, Input Value, Cost, Currency, Input Cost, Input Currency
				IRow row = sheet1.CreateRow(0);

				row.CreateCell(0).SetCellValue("Plant Name");
				row.CreateCell(1).SetCellValue("DUNS Code");
				row.CreateCell(2).SetCellValue("Measure");
				row.CreateCell(3).SetCellValue("Measure Name");
				row.CreateCell(4).SetCellValue("Period Year");
				row.CreateCell(5).SetCellValue("Period Month");
				row.CreateCell(6).SetCellValue(Resources.LocalizedText.Value);
				row.CreateCell(7).SetCellValue("UOM");
				row.CreateCell(8).SetCellValue("Input Value");
				row.CreateCell(9).SetCellValue("Input UOM");
				row.CreateCell(10).SetCellValue(Resources.LocalizedText.Cost);
				row.CreateCell(11).SetCellValue("Currency");
				row.CreateCell(12).SetCellValue("Input Cost");
				row.CreateCell(13).SetCellValue("Input Currency");

				ICell cellNumeric;
				ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
				cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

				for (int irows = 0; irows < metric_history.Count; irows++)
				{
					int rownum = irows + 1;
					UOM uom = null;
					uom_id = metric_history[irows].UOM_ID;
					uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
					if (uom != null)
						uom_cd = uom.UOM_CD;
					else
						uom_cd = "";
					try
					{
						uom_id = Convert.ToDecimal(metric_history[irows].INPUT_UOM_ID.ToString());
						uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
						if (uom != null)
							uom_input_cd = uom.UOM_CD;
						else
							uom_input_cd = "";
					}
					catch { uom_input_cd = ""; }
					// create a column for each field we want

					PLANT plant = SQMModelMgr.LookupPlant(metric_history[irows].PLANT_ID);
					EHS_MEASURE measure = metric_history[irows].EHS_MEASURE as EHS_MEASURE;

					row = sheet1.CreateRow(rownum);
					try
					{
						row.CreateCell(0).SetCellValue(plant.PLANT_NAME);
					}
					catch
					{
						row.CreateCell(0).SetCellValue("");
					}
					try
					{
						row.CreateCell(1).SetCellValue(plant.DUNS_CODE);
					}
					catch
					{
						row.CreateCell(1).SetCellValue("");
					}
					try
					{
						row.CreateCell(2).SetCellValue(measure.MEASURE_CD);
					}
					catch
					{
						row.CreateCell(2).SetCellValue("");
					}
					try
					{
						row.CreateCell(3).SetCellValue(measure.MEASURE_NAME);
					}
					catch
					{
						row.CreateCell(3).SetCellValue("");
					}
					try
					{
						row.CreateCell(4).SetCellValue(metric_history[irows].PERIOD_YEAR);
					}
					catch
					{
						row.CreateCell(4).SetCellValue("");
					}
					try
					{
						row.CreateCell(5).SetCellValue(metric_history[irows].PERIOD_MONTH);
					}
					catch
					{
						row.CreateCell(5).SetCellValue("");
					}
					try
					{
						cellNumeric = row.CreateCell(6);
						cellNumeric.CellStyle = cellStyleNumeric;
						cellNumeric.SetCellValue(Convert.ToDouble(metric_history[irows].MEASURE_VALUE));
					}
					catch
					{
						row.CreateCell(6).SetCellValue("");
					}
					try
					{
						row.CreateCell(7).SetCellValue(uom_cd);
					}
					catch
					{
						row.CreateCell(7).SetCellValue("");
					}
					try
					{
						cellNumeric = row.CreateCell(8);
						cellNumeric.CellStyle = cellStyleNumeric;
						cellNumeric.SetCellValue(Convert.ToDouble(metric_history[irows].INPUT_VALUE));
					}
					catch
					{
						row.CreateCell(8).SetCellValue("");
					}
					try
					{
						row.CreateCell(9).SetCellValue(uom_input_cd);
					}
					catch
					{
						row.CreateCell(9).SetCellValue("");
					}
					try
					{
						cellNumeric = row.CreateCell(10);
						cellNumeric.CellStyle = cellStyleNumeric;
						cellNumeric.SetCellValue(Convert.ToDouble(metric_history[irows].MEASURE_COST));
					}
					catch
					{
						row.CreateCell(10).SetCellValue("");
					}
					try
					{
						row.CreateCell(11).SetCellValue(metric_history[irows].CURRENCY_CODE);
					}
					catch
					{
						row.CreateCell(11).SetCellValue("");
					}
					try
					{
						cellNumeric = row.CreateCell(12);
						cellNumeric.CellStyle = cellStyleNumeric;
						cellNumeric.SetCellValue(Convert.ToDouble(metric_history[irows].INPUT_COST));
					}
					catch
					{
						row.CreateCell(12).SetCellValue("");
					}
					try
					{
						row.CreateCell(13).SetCellValue(metric_history[irows].INPUT_CURRENCY_CODE);
					}
					catch
					{
						row.CreateCell(13).SetCellValue("");
					}
				}
				sheet1.AutoSizeColumn(0);
				sheet1.AutoSizeColumn(1);
				sheet1.AutoSizeColumn(2);
				sheet1.AutoSizeColumn(3);
				sheet1.AutoSizeColumn(4);
				sheet1.AutoSizeColumn(5);
				sheet1.AutoSizeColumn(6);
				sheet1.AutoSizeColumn(7);
				sheet1.AutoSizeColumn(8);
				sheet1.AutoSizeColumn(9);
				sheet1.AutoSizeColumn(10);
				sheet1.AutoSizeColumn(11);
				sheet1.AutoSizeColumn(12);
				sheet1.AutoSizeColumn(13);

				GetExcelStream().WriteTo(Response.OutputStream);
				Response.End();
			}
			catch (Exception ex)
			{
				//Response.Write("Error processing the file:" + ex.Message.ToString());
				Response.End();
			}
		}

		MemoryStream GetExcelStream()
		{
			//Write the stream data of workbook to the root directory
			MemoryStream file = new MemoryStream();
			hssfworkbook.Write(file);
			return file;
		}

		void InitializeWorkbook()
		{
			hssfworkbook = new HSSFWorkbook();

			////create a entry of DocumentSummaryInformation
			DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
			dsi.Company = "Metricsoft";
			hssfworkbook.DocumentSummaryInformation = dsi;

			////create a entry of SummaryInformation
			SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
			si.Subject = "Data Export";
			hssfworkbook.SummaryInformation = si;
		}
	}
}