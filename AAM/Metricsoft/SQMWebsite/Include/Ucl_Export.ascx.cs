using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web.UI.HtmlControls;

namespace SQM.Website
{
    public delegate void ExportClick(string cmd);

    public partial class Ucl_Export : System.Web.UI.UserControl
    {

        public event ExportClick OnExportClick;

        public LinkButton LnkExport
        {
            get { return lnkExport; }
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void BindExport(string option, bool showButton, string toolTip)
        {
            lnkExport.CommandArgument = option;
            if (!string.IsNullOrEmpty(toolTip))
                lnkExport.ToolTip = toolTip;

            lnkExport.Visible = showButton;
        }

        #region common

        MemoryStream GetExcelStream(HSSFWorkbook hssfworkbook)
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            return file;
        }

        HSSFWorkbook InitializeWorkbook()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Metricsoft";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Data Export";
            hssfworkbook.SummaryInformation = si;

            return hssfworkbook;
        }

        #endregion

        protected void lnkExportClick(object sender, EventArgs e)
        {
            if (OnExportClick != null)
            {
                LinkButton lnk = (LinkButton)sender;
                OnExportClick(lnk.CommandArgument.ToString().Trim());
            }
        }

        #region environmental

        public void GenerateExportHistoryExcel(PSsqmEntities entities, string plantlist, DateTime dtFrom, DateTime dtTo)
        {
            string[] plants = null;
            plants = plantlist.Split(',');
            decimal[] plantIDs = new decimal[plants.Length];
            for (int loop1 = 0; loop1 < plants.Length; loop1++)
            {
                plantIDs[loop1] = Convert.ToDecimal(plants[loop1]);
            }

            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange);
            esMgr.LoadMetricHistory(plantIDs, dtFrom, dtTo, DateIntervalType.month, true);
            List<EHS_METRIC_HISTORY> metric_history = esMgr.MetricHst.OrderBy(l => l.PLANT_ID).ThenBy(l => l.PERIOD_YEAR).ThenBy(l => l.PERIOD_MONTH).ThenBy(l => l.EHS_MEASURE.MEASURE_CATEGORY).ThenBy(l => l.EHS_MEASURE.MEASURE_CD).ToList();

            GenerateExportHistoryExcel(entities, metric_history, true);
        }

        public void GenerateExportHistoryExcel(PSsqmEntities entities, List<EHS_METRIC_HISTORY> metric_history, bool appendApprovals)
        {
            string uom_cd;
            string uom_input_cd;
            decimal uom_id = 0;

            PLANT plant = null;
            EHSProfile profile = new EHSProfile();
            EHS_MEASURE measure = null;
            EHS_PROFILE_MEASURE prmr = null;

            string[] notWasteCategories = new string[] { "ENGY", "EUTL", "SAFE", "PROD" };

            List<EHS_PROFILE_MEASURE> profileMeasureList = new List<EHS_PROFILE_MEASURE>();

            HSSFWorkbook hssfworkbook = InitializeWorkbook();

            try
            {
                string filename = "MetricHistory.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();
                ISheet sheet1 = hssfworkbook.CreateSheet("Metric History");
                ISheet sheet2 = hssfworkbook.CreateSheet("Production & Safety");

                ICell cellNumeric;
                ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
                cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

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
                row.CreateCell(14).SetCellValue("Waste Code");
                row.CreateCell(15).SetCellValue("UN Disposal Code");
                row.CreateCell(16).SetCellValue("Regulatory Status");

                int rownum = 0;
                foreach (EHS_METRIC_HISTORY ms in metric_history)
                {
                    // create a column for each field we want
                    measure = null;
                    prmr = null;
                    if (plant == null || plant.PLANT_ID != ms.PLANT_ID)
                    {
                        plant = SQMModelMgr.LookupPlant(ms.PLANT_ID);
                        profile.Profile = (from p in entities.EHS_PROFILE.Include("EHS_PROFILE_MEASURE")
                                           where (p.PLANT_ID == plant.PLANT_ID)
                                           select p).SingleOrDefault();
                    }

                    try
                    {
                        if (profile.Profile != null)
                        {
                            measure = ms.EHS_MEASURE as EHS_MEASURE;
                            prmr = profile.Profile.EHS_PROFILE_MEASURE.Where(m => m.MEASURE_ID == ms.MEASURE_ID).SingleOrDefault();
                        }
                    }
                    catch
                    {
                        ;
                    }
                    if (measure == null || prmr == null  ||  measure.MEASURE_CATEGORY == "PROD")
                    {
                        continue;
                    }

                    ++rownum;
                    UOM uom = null;
                    uom_id = ms.UOM_ID;
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
                    if (uom != null)
                        uom_cd = uom.UOM_CD;
                    else
                        uom_cd = "";
                    try
                    {
                        uom_id = Convert.ToDecimal(ms.INPUT_UOM_ID.ToString());
                        uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
                        if (uom != null)
                            uom_input_cd = uom.UOM_CD;
                        else
                            uom_input_cd = "";
                    }
                    catch { uom_input_cd = ""; }
                    

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
                        row.CreateCell(4).SetCellValue(ms.PERIOD_YEAR);
                    }
                    catch
                    {
                        row.CreateCell(4).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(5).SetCellValue(ms.PERIOD_MONTH);
                    }
                    catch
                    {
                        row.CreateCell(5).SetCellValue("");
                    }
                    try
                    {
                        cellNumeric = row.CreateCell(6);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MEASURE_VALUE));
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
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.INPUT_VALUE));
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
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MEASURE_COST));
                    }
                    catch
                    {
                        row.CreateCell(10).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(11).SetCellValue(ms.CURRENCY_CODE);
                    }
                    catch
                    {
                        row.CreateCell(11).SetCellValue("");
                    }
                    try
                    {
                        cellNumeric = row.CreateCell(12);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.INPUT_COST));
                    }
                    catch
                    {
                        row.CreateCell(12).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(13).SetCellValue(ms.INPUT_CURRENCY_CODE);
                    }
                    catch
                    {
                        row.CreateCell(13).SetCellValue("");
                    }

                    try
                    {
                        if (string.IsNullOrEmpty(prmr.WASTE_CODE) || notWasteCategories.Contains(measure.MEASURE_CATEGORY))
                            row.CreateCell(14).SetCellValue("");
                        else
                            row.CreateCell(14).SetCellValue(prmr.WASTE_CODE);

                        if (string.IsNullOrEmpty(prmr.UN_CODE) || notWasteCategories.Contains(measure.MEASURE_CATEGORY))
                            row.CreateCell(15).SetCellValue("");
                        else
                            row.CreateCell(15).SetCellValue(prmr.UN_CODE);

                        if (string.IsNullOrEmpty(prmr.REG_STATUS) || notWasteCategories.Contains(measure.MEASURE_CATEGORY))
                            row.CreateCell(16).SetCellValue("");
                        else
                            row.CreateCell(16).SetCellValue(WebSiteCommon.GetXlatValue("regulatoryStatus", prmr.REG_STATUS));
                    }
                    catch
                    {
                        ;
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
                sheet1.AutoSizeColumn(14);
                sheet1.AutoSizeColumn(15);
                sheet1.AutoSizeColumn(16);

                IRow row2 = sheet2.CreateRow(0);
                row2.CreateCell(0).SetCellValue("Plant Name");
                row2.CreateCell(1).SetCellValue("Peroid Year");
                row2.CreateCell(2).SetCellValue("Peroid Month");
                row2.CreateCell(3).SetCellValue("Material Cost");
                row2.CreateCell(4).SetCellValue("Revenue");
                row2.CreateCell(5).SetCellValue("Hours Worked");
                row2.CreateCell(6).SetCellValue("Recorded Cases");
                row2.CreateCell(7).SetCellValue("Lost Time Cases");
                row2.CreateCell(8).SetCellValue("Days Lost");
                if (appendApprovals)
                {
                    row2.CreateCell(9).SetCellValue("Approval Date");
                    row2.CreateCell(10).SetCellValue("Approved By");
                    row2.CreateCell(11).SetCellValue("Finalize Date");
                    row2.CreateCell(12).SetCellValue("Finalized By");
                }

                PLANT_ACCOUNTING pac = null;
                List<PERSON> personList = new List<PERSON>();
                decimal plantID = 0;
                int periodYrMo = 0;
                rownum = 0;
                int cell = 0;
                foreach (EHS_METRIC_HISTORY ms in metric_history.Where(l=> l.EHS_MEASURE.MEASURE_CATEGORY == "PROD").ToList())
                {
                    try
                    {
                        if ((ms.PERIOD_YEAR + ms.PERIOD_MONTH) != periodYrMo || ms.PLANT_ID != plantID)
                        {
                            periodYrMo = ms.PERIOD_YEAR + ms.PERIOD_MONTH;
                            plantID = ms.PLANT_ID;
                            ++rownum;
                            row2 = sheet2.CreateRow(rownum);
                            if (appendApprovals)
                            {
                                pac = EHSModel.LookupPlantAccounting(entities, ms.PLANT_ID, ms.PERIOD_YEAR, ms.PERIOD_MONTH, false);
                            }
                        }

                        try
                        {

                            row2.CreateCell(0).SetCellValue(ms.PLANT.PLANT_NAME);
                        }
                        catch
                        {
                            row2.CreateCell(0).SetCellValue("");
                        }
                        try
                        {
                            row2.CreateCell(1).SetCellValue(ms.PERIOD_YEAR);
                        }
                        catch
                        {
                            row2.CreateCell(1).SetCellValue("");
                        }
                        try
                        {
                            row2.CreateCell(2).SetCellValue(ms.PERIOD_MONTH);
                        }
                        catch
                        {
                            row2.CreateCell(2).SetCellValue("");
                        }

                        cellNumeric = null;
                        switch (ms.MEASURE_ID.ToString())
                        {
                            case "1000000":
                                cellNumeric = row2.CreateCell(cell = 3);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000001":
                                cellNumeric = row2.CreateCell(cell = 4);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000004":
                                cellNumeric = row2.CreateCell(cell = 5);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000005":
                                cellNumeric = row2.CreateCell(cell = 8);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000006":
                                cellNumeric = row2.CreateCell(cell = 7);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000007":
                                cellNumeric = row2.CreateCell(cell = 6);
                                cellNumeric.CellStyle = cellStyleNumeric;
                                break;
                            case "1000008":
                                if (pac != null)
                                {
                                    try
                                    {
                                        row2.CreateCell(9).SetCellValue(SQMBasePage.FormatDate((DateTime)pac.APPROVAL_DT, "d", false));
                                    }
                                    catch
                                    {
                                        row2.CreateCell(9).SetCellValue("");
                                    }
                                    try
                                    {
                                        PERSON person = null;
                                        if (pac.APPROVER_ID.HasValue)
                                        {
                                            if ((person = personList.Where(l => l.PERSON_ID == pac.APPROVER_ID).FirstOrDefault()) == null)
                                            {
                                                if ((person = SQMModelMgr.LookupPerson((decimal)pac.APPROVER_ID, "")) != null)
                                                {
                                                    personList.Add(person);
                                                }
                                            }
                                        }
                                        if (person != null)
                                            row2.CreateCell(10).SetCellValue(SQMModelMgr.FormatPersonListItem(person));
                                        else
                                            row2.CreateCell(10).SetCellValue("");
                                    }
                                    catch { row2.CreateCell(10).SetCellValue(""); }

                                    try
                                    {
                                        row2.CreateCell(11).SetCellValue(SQMBasePage.FormatDate((DateTime)pac.FINALIZE_DT, "d", false));
                                    }
                                    catch
                                    {
                                        row2.CreateCell(11).SetCellValue("");
                                    }
                                    try
                                    {
                                        PERSON person = null;
                                        if (pac.FINALIZE_ID.HasValue)
                                        {
                                            if ((person = personList.Where(l => l.PERSON_ID == pac.FINALIZE_ID).FirstOrDefault()) == null)
                                            {
                                                if ((person = SQMModelMgr.LookupPerson((decimal)pac.FINALIZE_ID, "")) != null)
                                                {
                                                    personList.Add(person);
                                                }
                                            }
                                        }
                                        if (person != null)
                                            row2.CreateCell(12).SetCellValue(SQMModelMgr.FormatPersonListItem(person));
                                        else
                                            row2.CreateCell(12).SetCellValue("");
                                    }
                                    catch { row2.CreateCell(12).SetCellValue(""); }
                                }
                                break;
                            default:
                                break;
                        }

                        if (cellNumeric != null)
                        {
                            try
                            {
                                cellNumeric.SetCellValue(Convert.ToDouble(ms.MEASURE_VALUE));
                            }
                            catch
                            {
                                row2.CreateCell(cell).SetCellValue("");
                            }
                        }
                    }
                    catch
                    {
                        ;
                    }
                }

                sheet2.AutoSizeColumn(0);
                sheet2.AutoSizeColumn(1);
                sheet2.AutoSizeColumn(2);
                sheet2.AutoSizeColumn(3);
                sheet2.AutoSizeColumn(4);
                sheet2.AutoSizeColumn(5);
                sheet2.AutoSizeColumn(6);
                sheet2.AutoSizeColumn(7);
                sheet2.AutoSizeColumn(8);
                if (appendApprovals)
                {
                    sheet2.AutoSizeColumn(9);
                    sheet2.AutoSizeColumn(10);
                    sheet2.AutoSizeColumn(11);
                    sheet2.AutoSizeColumn(12);
                }
                
                GetExcelStream(hssfworkbook).WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                //Response.Write("Error processing the file:" + ex.Message.ToString());
                //Response.End();
                GetExcelStream(hssfworkbook).WriteTo(Response.OutputStream);
            }

        }
        #endregion

        #region profileinputs
        public void ExportProfileInputsExcel(PSsqmEntities entities, string plantlist, DateTime dtFrom, DateTime dtTo)
        {
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] plants = null;
            int loop1;
            string uom_cd;
            decimal uom_id = 0;

            PLANT plant = null;
           // EHSProfile profile = new EHSProfile();
            EHS_MEASURE measure = null;
            EHS_PROFILE_MEASURE prmr = null;
            EHS_PROFILE_INPUT input = null;

            plants = plantlist.Split(delimiter);
            decimal[] plantIDs = new decimal[plants.Length];
            for (loop1 = 0; loop1 < plants.Length; loop1++)
            {
                plantIDs[loop1] = Convert.ToDecimal(plants[loop1]);
            }

            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange);
            esMgr.LoadMetricInputs(dtFrom, dtTo, plantIDs, "");
           
            try
            {
                string filename = "MetricInputs.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();
                HSSFWorkbook hssfworkbook = InitializeWorkbook();
                ISheet sheet1 = hssfworkbook.CreateSheet("Metric Inputs");
                ISheet sheet2 = hssfworkbook.CreateSheet("Production");

                ICell cellNumeric;
                ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
                cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                //// create the header - Plant Name, DUNS Code, Measure, Measure Name, Period Year, Period Month, Period from Date, Period To Date, Value, UOM, UOM Name, Input Value, Cost, Currency, Input Cost, Input Currency
                IRow row = sheet1.CreateRow(0);

                row.CreateCell(0).SetCellValue("Plant Name");
                row.CreateCell(1).SetCellValue("DUNS Code");
                row.CreateCell(2).SetCellValue("Measure");
                row.CreateCell(3).SetCellValue("Measure Name");
                row.CreateCell(4).SetCellValue("Period Year");
                row.CreateCell(5).SetCellValue("Period Month");
                row.CreateCell(6).SetCellValue("Input Value");
                row.CreateCell(7).SetCellValue("Input UOM");
                row.CreateCell(8).SetCellValue("Input Cost");
                row.CreateCell(9).SetCellValue("Input Currency");
              
                int rownum = 0;
                for (int irows = 0; irows < esMgr.InputsList.Count; irows++)
                {
                    try
                    {
                        input = esMgr.InputsList[irows];
                        prmr = input.EHS_PROFILE_MEASURE;
                        measure = prmr.EHS_MEASURE;
                    }
                    catch
                    {
                        ;
                    }
                    if (measure == null || prmr == null  || input.EHS_PROFILE_MEASURE.EHS_MEASURE.MEASURE_CATEGORY == "PROD")
                    {
                        continue;
                    }

                    if (plant == null || plant.PLANT_ID != prmr.PLANT_ID)
                    {
                        plant = SQMModelMgr.LookupPlant(prmr.PLANT_ID);
                    }

                    UOM uom = null;
                    uom_cd = "";
                    try
                    {
                        uom_id = (decimal)input.UOM;
                        uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
                        if (uom != null)
                        {
                            uom_cd = uom.UOM_CD;
                            uom_id = Convert.ToDecimal(input.UOM.ToString());
                        }
                    }
                    catch { uom_cd = ""; }

                    // create a column for each field we want
                    ++rownum;
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
                        row.CreateCell(4).SetCellValue(input.PERIOD_YEAR);
                    }
                    catch
                    {
                        row.CreateCell(4).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(5).SetCellValue(input.PERIOD_MONTH);
                    }
                    catch
                    {
                        row.CreateCell(5).SetCellValue("");
                    }
                    try
                    {
                        cellNumeric = row.CreateCell(6);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(input.MEASURE_VALUE));
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
                        cellNumeric.SetCellValue(Convert.ToDouble(input.MEASURE_COST));
                    }
                    catch
                    {
                        row.CreateCell(8).SetCellValue("");
                    }
                    
                    try
                    {
                        row.CreateCell(9).SetCellValue(input.CURRENCY_CODE);
                    }
                    catch
                    {
                        row.CreateCell(9).SetCellValue("");
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
                
                IRow row2 = sheet2.CreateRow(0);
                row2.CreateCell(0).SetCellValue("Plant Name");
                row2.CreateCell(1).SetCellValue("Peroid Year");
                row2.CreateCell(2).SetCellValue("Peroid Month");
                row2.CreateCell(3).SetCellValue("Hours Worked");
                row2.CreateCell(4).SetCellValue("Approval Date");
                row2.CreateCell(5).SetCellValue("Approved By");

                esMgr.LoadPlantAccounting(plantIDs, dtFrom, dtTo);
                List<PLANT_ACCOUNTING> plant_history = esMgr.PlantHst.OrderBy(l => l.PLANT_ID).ThenBy(l => l.PERIOD_YEAR).ThenBy(l => l.PERIOD_MONTH).ToList();
                List<PERSON> personList = new List<PERSON>();
                PERSON person = null;
                rownum = 0;
                for (int nrows = 0; nrows < plant_history.Count; nrows++)
                {
                    ++rownum;
                    row2 = sheet2.CreateRow(rownum);
                    try
                    {
                        row2.CreateCell(0).SetCellValue(plant_history[nrows].PLANT.PLANT_NAME);
                    }
                    catch
                    {
                        row2.CreateCell(0).SetCellValue("");
                    }
                    try
                    {
                        row2.CreateCell(1).SetCellValue(plant_history[nrows].PERIOD_YEAR);
                    }
                    catch
                    {
                        row2.CreateCell(1).SetCellValue("");
                    }
                    try
                    {
                        row2.CreateCell(2).SetCellValue(plant_history[nrows].PERIOD_MONTH);
                    }
                    catch
                    {
                        row2.CreateCell(2).SetCellValue("");
                    }

                    try
                    {
                        cellNumeric = row2.CreateCell(3);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(plant_history[nrows].TIME_WORKED));
                    }
                    catch
                    {
                        row2.CreateCell(3).SetCellValue("");
                    }

                    try
                    {
                        row2.CreateCell(4).SetCellValue(SQMBasePage.FormatDate((DateTime)plant_history[nrows].APPROVAL_DT, "d", false));
                    }
                    catch
                    {
                        row2.CreateCell(4).SetCellValue("");
                    }

                    try
                    {
                        person = null;
                        if (plant_history[nrows].APPROVER_ID.HasValue)
                        {
                            if ((person = personList.Where(l => l.PERSON_ID == plant_history[nrows].APPROVER_ID).FirstOrDefault()) == null)
                            {
                                if ((person = SQMModelMgr.LookupPerson((decimal)plant_history[nrows].APPROVER_ID, "")) != null)
                                {
                                    personList.Add(person);
                                }
                            }
                        }
                        if (person != null)
                            row2.CreateCell(5).SetCellValue(SQMModelMgr.FormatPersonListItem(person));
                        else
                            row2.CreateCell(5).SetCellValue("");
                    }
                    catch { row2.CreateCell(5).SetCellValue(""); }
                }

                sheet2.AutoSizeColumn(0);
                sheet2.AutoSizeColumn(1);
                sheet2.AutoSizeColumn(2);
                sheet2.AutoSizeColumn(3);
                sheet2.AutoSizeColumn(4);
                sheet2.AutoSizeColumn(5);

                GetExcelStream(hssfworkbook).WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                //Response.Write("Error processing the file:" + ex.Message.ToString());
                //Response.End();
            }
          
        }
        #endregion

        #region EHSincidents
        public void GenerateIncidentExportExcel(PSsqmEntities entities, List<EHSIncidentData> incidentList)
        {
            DateTime dtIncidentDate;
            DateTime dtReportDate;
            DateTime dtDueDate;
            string strAnswerValue;
            PLANT plant = null;

            try
            {
                string filename = "IncidentExport.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();
                HSSFWorkbook hssfworkbook = InitializeWorkbook();
                ISheet sheet1 = hssfworkbook.CreateSheet("Incidents");

                string strIncidentDate;
                string strReportDate;
                string strIncidentType;
                string strPlantname;
                string strDescription;
                string strRootCauseOC;
                string strCorrectiveAction;
                string strResponsiblePerson;
                string strDueDate;

                ICell cellNumeric;
                ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
                cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                //// create the headers 
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue(Resources.LocalizedText.IncidentID);
                row1.CreateCell(1).SetCellValue(Resources.LocalizedText.IncidentDate);
                row1.CreateCell(2).SetCellValue(Resources.LocalizedText.ReportDate);
                row1.CreateCell(3).SetCellValue(Resources.LocalizedText.IncidentType); // really the Issue Type
                row1.CreateCell(4).SetCellValue(Resources.LocalizedText.Location); // Really the plant name
                row1.CreateCell(5).SetCellValue(Resources.LocalizedText.Description);
                row1.CreateCell(6).SetCellValue("Root Cause Operational Control");
                row1.CreateCell(7).SetCellValue(Resources.LocalizedText.CorrectiveAction);
                row1.CreateCell(8).SetCellValue(Resources.LocalizedText.ResponsiblePerson);
                row1.CreateCell(9).SetCellValue(Resources.LocalizedText.DueDate);

                int rownum1 = 0;
                for (int irows = 0; irows < incidentList.Count; irows++)
                {
                    INCIDENT incident = incidentList[irows].Incident;

                    strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 1); // Incident date
                    if (strAnswerValue != null && strAnswerValue != "")
                    {
                        try
                        {
                            dtIncidentDate = Convert.ToDateTime(strAnswerValue);
                            if (dtIncidentDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                                strIncidentDate = "";
                            else
                                strIncidentDate = dtIncidentDate.ToString("MM/dd/yyyy");
                        }
                        catch { strIncidentDate = ""; }
                    }
                    else
                        strIncidentDate = "";

                    strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 2); // Due date
                    if (strAnswerValue != null && strAnswerValue != "")
                    {
                        try
                        {
                            dtDueDate = Convert.ToDateTime(strAnswerValue);
                            if (dtDueDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                                strDueDate = "";
                            else
                                strDueDate = dtDueDate.ToString("MM/dd/yyyy");
                        }
                        catch { strDueDate = ""; }
                    }
                    else
                        strDueDate = "";

                    strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 65); // Report date
                    if (strAnswerValue != null && strAnswerValue != "")
                    {
                        try
                        {
                            dtReportDate = Convert.ToDateTime(strAnswerValue);
                            if (dtReportDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                                strReportDate = "";
                            else
                                strReportDate = dtReportDate.ToString("MM/dd/yyyy");
                        }
                        catch { strReportDate = ""; }
                    }
                    else
                        strReportDate = "";

                    try { strIncidentType = incident.ISSUE_TYPE; }
                    catch { strIncidentType = ""; }

                    try { strPlantname = incidentList[irows].Plant.PLANT_NAME; }
                    catch { strPlantname = ""; }

                    try 
                    { 
                        strDescription = StringHtmlExtensions.TruncateHtml(incident.DESCRIPTION, 1000, "...");
                        strDescription =  WebSiteCommon.StripHTML(strDescription);
                    }
                    catch { strDescription = ""; }

                    if (incidentList[irows].Incident.INCIDENT_ANSWER.Where(l => l.INCIDENT_QUESTION_ID == 54 && l.ANSWER_VALUE == "Yes").Count() > 0)
                    {
                        // this will be the logic to process the 8D incidents
                        decimal problemCaseId = EHSIncidentMgr.SelectProblemCaseIdByIncidentId(incident.INCIDENT_ID);
                        if (problemCaseId > 0)
                        {
                            // Get RCOC from the PROB_CAUSE_STEP record that has been marked as the Root Cause
                            var problemCase = new ProblemCase().Initialize().Load(problemCaseId);
                            if (problemCase != null && problemCase.ProbCase != null && problemCase.ProbCase.PROB_CAUSE != null && problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP != null && problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP.Count > 0)
                            {
                                List<PROB_CAUSE_STEP> probCauses = (from i in problemCase.ProbCase.PROB_CAUSE.PROB_CAUSE_STEP
                                                                    where
                                                                        i.IS_ROOTCAUSE == true
                                                                    select i).OrderBy(h => h.ITERATION_NO).ToList();
                                strAnswerValue = "";
                                for (int xrows = 0; xrows < probCauses.Count; xrows++)
                                {
                                    // there shouldn't be more than one root cause, but if so, take the highest iteration_no
                                    strAnswerValue = WebSiteCommon.GetXlatValueLong("causeTypeEHS", probCauses[xrows].CAUSE_TYPE);
                                }
                                if (strAnswerValue != null)
                                    strRootCauseOC = strAnswerValue;
                                else
                                    strRootCauseOC = "";
                            }
                            else { strRootCauseOC = ""; }
                            // Find all of the Corrective Action records for the incident and add 1 record for each.  
                            if (problemCase != null && problemCase.ProbCase != null && problemCase.ProbCase.PROB_CAUSE_ACTION != null && problemCase.ProbCase.PROB_CAUSE_ACTION.Count > 0)
                            {
                                foreach (PROB_CAUSE_ACTION pca in problemCase.ProbCase.PROB_CAUSE_ACTION)
                                {
                                    if (pca.ACTION_DESC != null)
                                        strCorrectiveAction = pca.ACTION_DESC;
                                    else
                                        strCorrectiveAction = "";

                                    if (pca.RESPONSIBLE1_PERSON != null)
                                    {
                                        try
                                        {
                                            PERSON respPerson = SQMModelMgr.LookupPerson(entities, Convert.ToDecimal(pca.RESPONSIBLE1_PERSON), "", false);
                                            if (respPerson != null)
                                                strResponsiblePerson = respPerson.FIRST_NAME.Trim() + " " + respPerson.LAST_NAME.Trim();
                                            else
                                                strResponsiblePerson = "";
                                        }
                                        catch { strResponsiblePerson = ""; }
                                    }
                                    else
                                        strResponsiblePerson = "";

                                    if (pca.EFF_DT != null)
                                    {
                                        try
                                        {
                                            dtDueDate = Convert.ToDateTime(pca.EFF_DT);
                                            if (dtDueDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                                                strDueDate = "";
                                            else
                                                strDueDate = dtDueDate.ToString("MM/dd/yyyy");
                                        }
                                        catch { strDueDate = ""; }
                                    }
                                    else
                                        strDueDate = "";

                                    ++rownum1;
                                    row1 = sheet1.CreateRow(rownum1);
                                    row1.CreateCell(0).SetCellValue(incidentList[irows].Incident.INCIDENT_ID.ToString());
                                    row1.CreateCell(1).SetCellValue(strIncidentDate);
                                    row1.CreateCell(2).SetCellValue(strReportDate);
                                    row1.CreateCell(3).SetCellValue(strIncidentType);
                                    row1.CreateCell(4).SetCellValue(strPlantname);
                                    row1.CreateCell(5).SetCellValue(strDescription);
                                    row1.Cells[5].CellStyle.WrapText = true;
                                    row1.CreateCell(6).SetCellValue(strRootCauseOC);
                                    row1.CreateCell(7).SetCellValue(strCorrectiveAction);
                                    row1.Cells[7].CellStyle.WrapText = true;
                                    row1.CreateCell(8).SetCellValue(strResponsiblePerson);
                                    row1.CreateCell(9).SetCellValue(strDueDate);
                                }
                            }
                            else
                            {
                                ++rownum1;
                                row1 = sheet1.CreateRow(rownum1);
                                row1.CreateCell(0).SetCellValue(incidentList[irows].Incident.INCIDENT_ID.ToString());
                                row1.CreateCell(1).SetCellValue(strIncidentDate);
                                row1.CreateCell(2).SetCellValue(strReportDate);
                                row1.CreateCell(3).SetCellValue(strIncidentType);
                                row1.CreateCell(4).SetCellValue(strPlantname);
                                row1.CreateCell(5).SetCellValue(strDescription);
                                row1.Cells[5].CellStyle.WrapText = true;
                                row1.CreateCell(6).SetCellValue(strRootCauseOC);
                                row1.CreateCell(7).SetCellValue("");
                                row1.CreateCell(8).SetCellValue("");
                                row1.CreateCell(9).SetCellValue("");
                            }
                        }
                        else
                        {
                            row1 = sheet1.CreateRow(rownum1);
                            row1.CreateCell(0).SetCellValue(incidentList[irows].Incident.INCIDENT_ID.ToString());
                            row1.CreateCell(1).SetCellValue(strIncidentDate);
                            row1.CreateCell(2).SetCellValue(strReportDate);
                            row1.CreateCell(3).SetCellValue(strIncidentType);
                            row1.CreateCell(4).SetCellValue(strPlantname);
                            row1.CreateCell(5).SetCellValue(strDescription);
                            row1.Cells[5].CellStyle.WrapText = true;
                            row1.CreateCell(6).SetCellValue("");
                            row1.CreateCell(7).SetCellValue("");
                            row1.CreateCell(8).SetCellValue("");
                            row1.CreateCell(9).SetCellValue("");
                        }
                    }
                    else
                    {
                        // non 8D incidents
                        ++rownum1;
                        row1 = sheet1.CreateRow(rownum1);
                        row1.CreateCell(0).SetCellValue(incidentList[irows].Incident.INCIDENT_ID.ToString());
                        row1.CreateCell(1).SetCellValue(strIncidentDate);
                        row1.CreateCell(2).SetCellValue(strReportDate);
                        row1.CreateCell(3).SetCellValue(strIncidentType);
                        row1.CreateCell(4).SetCellValue(strPlantname);
                        row1.CreateCell(5).SetCellValue(strDescription);
                        row1.Cells[5].CellStyle.WrapText = true;
                        try
                        {
                            strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 78); // root cause operational control
                            if (strAnswerValue != null)
                                row1.CreateCell(6).SetCellValue(strAnswerValue);
                            else
                                row1.CreateCell(6).SetCellValue("");
                        }
                        catch { row1.CreateCell(6).SetCellValue(""); }
                        try
                        {
                            strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 69); // corrective actions
                            if (strAnswerValue != null)
                            {
                                row1.CreateCell(7).SetCellValue(strAnswerValue);
                                row1.Cells[7].CellStyle.WrapText = true;
                            }
                            else
                                row1.CreateCell(7).SetCellValue("");
                        }
                        catch { row1.CreateCell(7).SetCellValue(""); }
                        try
                        {
                            strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 64); // responsible person
                            if (strAnswerValue != null)
                                row1.CreateCell(8).SetCellValue(strAnswerValue);
                            else
                                row1.CreateCell(8).SetCellValue("");
                        }
                        catch { row1.CreateCell(8).SetCellValue(""); }

                        row1.CreateCell(9).SetCellValue(strDueDate);
                    }

                }

                sheet1.AutoSizeColumn(0);
                sheet1.AutoSizeColumn(1);
                sheet1.AutoSizeColumn(2);
                sheet1.AutoSizeColumn(3);
                sheet1.AutoSizeColumn(4);
                sheet1.SetColumnWidth(5, 10000); // text fields should not auto size
                sheet1.AutoSizeColumn(6);
                sheet1.SetColumnWidth(7, 10000); // text fields should not auto size
                sheet1.AutoSizeColumn(8);
                sheet1.AutoSizeColumn(9);

                GetExcelStream(hssfworkbook).WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                //Response.Write("Error processing the file:" + ex.Message.ToString());
                //Response.End();
            }

        }

        public void GeneratePreventativeActionExportExcel(PSsqmEntities entities, List<EHSIncidentData> incidentList)
        {
            DateTime dtIncidentDate;
            DateTime dtReportDate;
            DateTime dtDueDate;
            string strAnswerValue;
            PLANT plant = null;

            try
            {
                string filename = "PreventActionExport.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();
                HSSFWorkbook hssfworkbook = InitializeWorkbook();
                ISheet sheet1 = hssfworkbook.CreateSheet("Recommendations");

                string strIncidentDate;
                string strReportDate;
                string strIncidentType;
                string strRecommendType;
                string strPlantname;
                string strDescription;
                string strResponsiblePerson;
                string strDueDate;

                ICell cellNumeric;
                ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
                cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                //// create the headers 
                IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("Recommendation ID");
                row1.CreateCell(1).SetCellValue(Resources.LocalizedText.InspectionDate);
                row1.CreateCell(2).SetCellValue(Resources.LocalizedText.ReportDate);
                row1.CreateCell(3).SetCellValue(Resources.LocalizedText.Location); // Really the plant name
                row1.CreateCell(4).SetCellValue("Inspection Category"); 
                row1.CreateCell(5).SetCellValue("Recommendation Type");
                row1.CreateCell(6).SetCellValue("Recommendation Description"); 
                row1.CreateCell(7).SetCellValue(Resources.LocalizedText.Description);
                row1.CreateCell(8).SetCellValue(Resources.LocalizedText.ResponsiblePerson);
                row1.CreateCell(9).SetCellValue(Resources.LocalizedText.DueDate);
                row1.CreateCell(10).SetCellValue(Resources.LocalizedText.Status);
                row1.CreateCell(11).SetCellValue("Date Actions Applied");
                row1.CreateCell(12).SetCellValue("Corrective Actions Summary");
                row1.CreateCell(13).SetCellValue("Recorded By");
            
                int rownum1 = 0;
                for (int irows = 0; irows < incidentList.Count; irows++)
                {
                    EHSIncidentData data = incidentList[irows];

                    try
                    {
                        if (data.Incident.INCIDENT_DT.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                            strIncidentDate = "";
                        else
                            strIncidentDate = data.Incident.INCIDENT_DT.ToString("MM/dd/yyyy");
                    }
                    catch { strIncidentDate = ""; }

                    try
                    {
                        if (Convert.ToDateTime(data.Incident.CREATE_DT).ToString("MM/dd/yyyy").Equals("01/01/0001"))
                            strReportDate = "";
                        else
                            strReportDate = Convert.ToDateTime(data.Incident.CREATE_DT).ToString("MM/dd/yyyy");
                    }
                    catch { strReportDate = ""; }


                    strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 2); // Due date
                    if (strAnswerValue != null && strAnswerValue != "")
                    {
                        try
                        {
                            dtDueDate = Convert.ToDateTime(strAnswerValue);
                            if (dtDueDate.ToString("MM/dd/yyyy").Equals("01/01/0001"))
                                strDueDate = "";
                            else
                                strDueDate = dtDueDate.ToString("MM/dd/yyyy");
                        }
                        catch { strDueDate = ""; }
                    }
                    else
                        strDueDate = "";

                    try { strPlantname = incidentList[irows].Plant.PLANT_NAME; }
                    catch { strPlantname = ""; }

                    ++rownum1;
                    row1 = sheet1.CreateRow(rownum1);
                    row1.CreateCell(0).SetCellValue(incidentList[irows].Incident.INCIDENT_ID.ToString());
                    row1.CreateCell(1).SetCellValue(strIncidentDate);
                    row1.CreateCell(2).SetCellValue(strReportDate);
                    row1.CreateCell(3).SetCellValue(strPlantname);
                   
                    try
                    {
                        strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 81); // inspect category
                        if (strAnswerValue != null)
                            row1.CreateCell(4).SetCellValue(strAnswerValue);
                        else
                            row1.CreateCell(4).SetCellValue("");
                    }
                    catch { row1.CreateCell(4).SetCellValue(""); }

                    try
                    {
                        strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 83); // recommend Type
                        if (strAnswerValue != null)
                            row1.CreateCell(5).SetCellValue(strAnswerValue);
                        else
                            row1.CreateCell(5).SetCellValue("");
                    }
                    catch { row1.CreateCell(5).SetCellValue(""); }

                    try
                    {
                        strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 82); // recommend description
                        if (strAnswerValue != null)
                            row1.CreateCell(6).SetCellValue(strAnswerValue);
                        else
                            row1.CreateCell(6).SetCellValue("");
                    }
                    catch { row1.CreateCell(6).SetCellValue(""); }

                    try
                    {
                        strDescription = StringHtmlExtensions.TruncateHtml(data.Incident.DESCRIPTION, 1000, "...");
                        strDescription = WebSiteCommon.StripHTML(strDescription);
                    }
                    catch { strDescription = ""; }
                   
                    row1.CreateCell(7).SetCellValue(strDescription);
                    row1.Cells[7].CellStyle.WrapText = true;

                    try
                    {
                        strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 84); // assign person
                        if (strAnswerValue != null)
                        {
                            row1.CreateCell(8).SetCellValue(SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson(entities, Convert.ToDecimal(strAnswerValue), "", false)));
                        }
                        else
                            row1.CreateCell(8).SetCellValue("");
                    }
                    catch { row1.CreateCell(8).SetCellValue(""); }
                   
                   try
                   {
                        strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 92); // projected completion date
                        if (!string.IsNullOrEmpty(strAnswerValue))
                            row1.CreateCell(9).SetCellValue(Convert.ToDateTime(strAnswerValue).ToString("MM/dd/yyyy"));
                        else
                            row1.CreateCell(9).SetCellValue("");
                    }
                    catch { row1.CreateCell(9).SetCellValue(""); }

                   try
                   {
                       row1.CreateCell(10).SetCellValue(WebSiteCommon.GetXlatValue("incidentStatus",data.Status));
                   }
                   catch { row1.CreateCell(10).SetCellValue(""); }

                   try
                   {
                       strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 85); // action applied date
                       if (!string.IsNullOrEmpty(strAnswerValue))
                           row1.CreateCell(11).SetCellValue(Convert.ToDateTime(strAnswerValue).ToString("MM/dd/yyyy"));
                       else
                           row1.CreateCell(11).SetCellValue("");
                   }
                   catch { row1.CreateCell(11).SetCellValue(""); }

                   try
                   {    // actions summary
                       strDescription = StringHtmlExtensions.TruncateHtml(EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 86), 1000, "...");
                       strDescription = WebSiteCommon.StripHTML(strDescription);
                   }
                   catch { strDescription = ""; }

                   row1.CreateCell(12).SetCellValue(strDescription);
                   row1.Cells[12].CellStyle.WrapText = true;

                   try
                   {
                       if (data.Incident.CREATE_PERSON.HasValue)
                       {
                           row1.CreateCell(13).SetCellValue(SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson(entities, (decimal)data.Incident.CREATE_PERSON, "", false)));
                       }
                       else
                           row1.CreateCell(13).SetCellValue("");
                   }
                   catch { row1.CreateCell(13).SetCellValue(""); }
                }

                sheet1.AutoSizeColumn(0);
                sheet1.AutoSizeColumn(1);
                sheet1.AutoSizeColumn(2);
                sheet1.AutoSizeColumn(3);
                sheet1.AutoSizeColumn(4);
                sheet1.AutoSizeColumn(5);
                sheet1.AutoSizeColumn(6);
                sheet1.SetColumnWidth(7, 10000); // text fields should not auto size
                sheet1.AutoSizeColumn(8);
                sheet1.AutoSizeColumn(9);
                sheet1.AutoSizeColumn(10);
                sheet1.AutoSizeColumn(11);
                sheet1.SetColumnWidth(12, 10000); // text fields should not auto size
                sheet1.AutoSizeColumn(13);

                GetExcelStream(hssfworkbook).WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                //Response.Write("Error processing the file:" + ex.Message.ToString());
                //Response.End();
            }

        }
        #endregion
    }

}