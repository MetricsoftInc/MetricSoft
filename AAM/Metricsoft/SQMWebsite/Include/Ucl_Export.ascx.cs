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

        //only incident which have incident_id greater than Maxincident can have newly added columns in exported report. 
        int maxIncidentforInjuryType = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncidentIDforInjuryType"]);
        int maxINCIDENT = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncident"]);

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

            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange, "E");
            esMgr.LoadMetricHistory(plantIDs, dtFrom, dtTo, DateIntervalType.month, true);
            List<MetricData> metric_history = esMgr.MetricHst.OrderBy(l => l.MetricRec.PLANT_ID).ThenBy(l => l.MetricRec.PERIOD_YEAR).ThenBy(l => l.MetricRec.PERIOD_MONTH).ThenBy(l => l.Measure.MEASURE_CATEGORY).ThenBy(l => l.Measure.MEASURE_CD).ToList();

            GenerateExportHistoryExcel(entities, metric_history, true);
        }

        public void GenerateExportHistoryExcel(PSsqmEntities entities, List<MetricData> metric_history, bool appendApprovals)
        {
            string uom_cd;
            string uom_input_cd;
            decimal uom_id = 0;

            PLANT plant = null;
            EHSProfile profile = new EHSProfile();
            EHS_MEASURE measure = null;
            EHS_PROFILE_MEASURE prmr = null;
            List<EHS_PROFILE_MEASURE> prmrList = null;
            string measureName = "";

            string[] notWasteCategories = new string[] { "ENGY", "EUTL", "SAFE", "PROD" };

            List<EHS_PROFILE_MEASURE> profileMeasureList = new List<EHS_PROFILE_MEASURE>();

            List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("EHS", "USER");
            string altDunsLabel = "";
            if (sets.Where(s => s.SETTING_CD == "EXPORT_ALTDUNS").FirstOrDefault() != null)
            {
                altDunsLabel = sets.Where(s => s.SETTING_CD == "EXPORT_ALTDUNS").First().VALUE;
            }

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
                row.CreateCell(17).SetCellValue("Metric Inputs");
                if (!string.IsNullOrEmpty(altDunsLabel))
                {
                    row.CreateCell(18).SetCellValue(altDunsLabel);
                }

                int rownum = 0;
                foreach (MetricData ms in metric_history)
                {
                    // create a column for each field we want
                    measure = null;
                    prmr = null;
                    if (plant == null || plant.PLANT_ID != ms.MetricRec.PLANT_ID)
                    {
                        plant = SQMModelMgr.LookupPlant(entities, ms.MetricRec.PLANT_ID, "");
                        profile.Profile = (from p in entities.EHS_PROFILE.Include("EHS_PROFILE_MEASURE")
                                           where (p.PLANT_ID == plant.PLANT_ID)
                                           select p).SingleOrDefault();
                    }

                    try
                    {
                        if (profile.Profile != null)
                        {
                            measure = ms.Measure as EHS_MEASURE;
                            prmrList = profile.Profile.EHS_PROFILE_MEASURE.Where(m => m.MEASURE_ID == ms.Measure.MEASURE_ID).ToList();
                            prmr = prmrList.FirstOrDefault();
                            measureName = measure.MEASURE_NAME;
                            /*
							if (prmrList.Count > 1)
							{
								measureName += (" (" + prmrList.Count.ToString() + ")");
							}
							*/
                        }
                    }
                    catch
                    {
                        ;
                    }
                    if (measure == null || prmr == null || measure.MEASURE_CATEGORY == "PROD")
                    {
                        continue;
                    }

                    ++rownum;
                    UOM uom = null;
                    uom_id = ms.MetricRec.UOM_ID;
                    uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == uom_id);
                    if (uom != null)
                        uom_cd = uom.UOM_CD;
                    else
                        uom_cd = "";
                    try
                    {
                        uom_id = Convert.ToDecimal(ms.MetricRec.INPUT_UOM_ID.ToString());
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
                        row.CreateCell(3).SetCellValue(measureName);
                    }
                    catch
                    {
                        row.CreateCell(3).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(4).SetCellValue(ms.MetricRec.PERIOD_YEAR);
                    }
                    catch
                    {
                        row.CreateCell(4).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(5).SetCellValue(ms.MetricRec.PERIOD_MONTH);
                    }
                    catch
                    {
                        row.CreateCell(5).SetCellValue("");
                    }
                    try
                    {
                        cellNumeric = row.CreateCell(6);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MetricRec.MEASURE_VALUE));
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
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MetricRec.INPUT_VALUE));
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
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MetricRec.MEASURE_COST));
                    }
                    catch
                    {
                        row.CreateCell(10).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(11).SetCellValue(ms.MetricRec.CURRENCY_CODE);
                    }
                    catch
                    {
                        row.CreateCell(11).SetCellValue("");
                    }
                    try
                    {
                        cellNumeric = row.CreateCell(12);
                        cellNumeric.CellStyle = cellStyleNumeric;
                        cellNumeric.SetCellValue(Convert.ToDouble(ms.MetricRec.INPUT_COST));
                    }
                    catch
                    {
                        row.CreateCell(12).SetCellValue("");
                    }
                    try
                    {
                        row.CreateCell(13).SetCellValue(ms.MetricRec.INPUT_CURRENCY_CODE);
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

                        string inputNames = "";
                        foreach (EHS_PROFILE_MEASURE pr in prmrList)
                        {
                            if (!string.IsNullOrEmpty(pr.MEASURE_PROMPT))
                                inputNames += inputNames.Length == 0 ? pr.MEASURE_PROMPT : (", " + pr.MEASURE_PROMPT);
                            else
                                inputNames += inputNames.Length == 0 ? measureName : (", " + measureName);
                        }
                        try
                        {
                            row.CreateCell(17).SetCellValue(inputNames);
                            row.Cells[17].CellStyle.WrapText = true;
                        }
                        catch
                        {
                            row.CreateCell(17).SetCellValue("");
                        }

                        if (!string.IsNullOrEmpty(altDunsLabel))
                        {
                            try
                            {
                                row.CreateCell(18).SetCellValue(plant.ALT_DUNS_CODE);
                            }
                            catch
                            {
                                row.CreateCell(18).SetCellValue("");
                            }
                        }
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
                sheet1.AutoSizeColumn(17);
                if (!string.IsNullOrEmpty(altDunsLabel))
                {
                    sheet1.AutoSizeColumn(18);
                }

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
                foreach (MetricData ms in metric_history.Where(l => l.Measure.MEASURE_CATEGORY == "PROD").ToList())
                {
                    try
                    {
                        if ((ms.MetricRec.PERIOD_YEAR + ms.MetricRec.PERIOD_MONTH) != periodYrMo || ms.MetricRec.PLANT_ID != plantID)
                        {
                            periodYrMo = ms.MetricRec.PERIOD_YEAR + ms.MetricRec.PERIOD_MONTH;
                            plantID = ms.MetricRec.PLANT_ID;
                            plant = SQMModelMgr.LookupPlant(entities, ms.MetricRec.PLANT_ID, "");
                            ++rownum;
                            row2 = sheet2.CreateRow(rownum);
                            if (appendApprovals)
                            {
                                pac = EHSModel.LookupPlantAccounting(entities, ms.MetricRec.PLANT_ID, ms.MetricRec.PERIOD_YEAR, ms.MetricRec.PERIOD_MONTH, false);
                            }
                        }

                        try
                        {

                            row2.CreateCell(0).SetCellValue(plant.PLANT_NAME);
                        }
                        catch
                        {
                            row2.CreateCell(0).SetCellValue("");
                        }
                        try
                        {
                            row2.CreateCell(1).SetCellValue(ms.MetricRec.PERIOD_YEAR);
                        }
                        catch
                        {
                            row2.CreateCell(1).SetCellValue("");
                        }
                        try
                        {
                            row2.CreateCell(2).SetCellValue(ms.MetricRec.PERIOD_MONTH);
                        }
                        catch
                        {
                            row2.CreateCell(2).SetCellValue("");
                        }

                        cellNumeric = null;
                        switch (ms.MetricRec.MEASURE_ID.ToString())
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
                                cellNumeric.SetCellValue(Convert.ToDouble(ms.MetricRec.MEASURE_VALUE));
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

            EHSCalcsCtl esMgr = new EHSCalcsCtl().CreateNew(SessionManager.FYStartDate().Month, DateSpanOption.SelectRange, "E");
            esMgr.LoadMetricInputs(dtFrom, dtTo, plantIDs, "");

            List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("EHS", "USER");
            string altDunsLabel = "";
            if (sets.Where(s => s.SETTING_CD == "EXPORT_ALTDUNS").FirstOrDefault() != null)
            {
                altDunsLabel = sets.Where(s => s.SETTING_CD == "EXPORT_ALTDUNS").First().VALUE;
            }

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
                if (!string.IsNullOrEmpty(altDunsLabel))
                {
                    row.CreateCell(10).SetCellValue(altDunsLabel);
                }


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
                    if (measure == null || prmr == null || input.EHS_PROFILE_MEASURE.EHS_MEASURE.MEASURE_CATEGORY == "PROD")
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

                    if (!string.IsNullOrEmpty(altDunsLabel))
                    {
                        try
                        {
                            row.CreateCell(10).SetCellValue(plant.ALT_DUNS_CODE);
                        }
                        catch
                        {
                            row.CreateCell(10).SetCellValue("");
                        }
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
                if (!string.IsNullOrEmpty(altDunsLabel))
                {
                    sheet1.AutoSizeColumn(10);
                }

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
            //Declare variables for files.
            string strEmployeeDate = "";
            string strEmployeeTaskDate = "";
            string strJobTitle = "";


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
                string XLAT_LANGUAGE = "en";
                bool isRecordable;


                string strTimeOfIncident;//time of Incident
                string strShift;//Shift
                string strDepartment;
                string strInsideOrOutsideBuilding;
                string strEmployeeStatus;

                bool isErgonomicConcerns;
                bool isStandardWorkProcedures;
                string strProceduresFollowed;
                bool isTrainingProvided;
                string strDateAssociateBegan;

                bool isReoccur;
                bool isFirstAid;
                bool isFatality;
                bool isLostTime;
                bool isRestrictedTime;

                bool maxIncident = false;//If incident id is greater than 'maxIncident' then following columns will be visible.

                string strTNSK;
                string strBusinessType;
                string strMacroProcessType;
                string strSpecificProcessType;
                string strEquipmentManufacturerName;
                string strEquipmentManufacturerDate;
                string strDesignNumber;
                string strAssetNumber;
                string strAgeofAssociate;
                string strTypeofIncident;
                string strInitialTreatmentGiven;
                string strChangeMedicalStatus;

                string Macro_Process;

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
                row1.CreateCell(5).SetCellValue("First Aid?");//First Aid?             
                row1.CreateCell(6).SetCellValue("Recordable?");//Recordable
                row1.CreateCell(7).SetCellValue("Lost Time?");//Lost Time
                row1.CreateCell(8).SetCellValue(" Initial Treatment Given");// Initial Treatment Given

                row1.CreateCell(9).SetCellValue("Fatality?");// Fatality?
                row1.CreateCell(10).SetCellValue("Severity Level ");//Severity Level
                row1.CreateCell(11).SetCellValue("Injury Type");
                row1.CreateCell(12).SetCellValue("Body Part Affected");
                row1.CreateCell(13).SetCellValue("How long has the associate been employed ?");
                row1.CreateCell(14).SetCellValue("How long has the associate been doing this specific job/task?");
                row1.CreateCell(15).SetCellValue("Occupation/Job title");
                row1.CreateCell(16).SetCellValue(Resources.LocalizedText.Description);
                row1.CreateCell(17).SetCellValue("Root Cause Operational Control");
                row1.CreateCell(18).SetCellValue(Resources.LocalizedText.CorrectiveAction);
                row1.CreateCell(19).SetCellValue(Resources.LocalizedText.ResponsiblePerson);
                row1.CreateCell(20).SetCellValue(Resources.LocalizedText.DueDate);

                row1.CreateCell(21).SetCellValue("Time Of Incident");//time of Incident
                row1.CreateCell(22).SetCellValue("Shift");//Shift
                row1.CreateCell(23).SetCellValue("Department");//Department         
                row1.CreateCell(24).SetCellValue("Inside or Outside Building");//Inside or Outside Building"
                row1.CreateCell(25).SetCellValue("Employee Status");//Employee Status"

                row1.CreateCell(26).SetCellValue("Ergonomic Concerns");//Ergonomic Concerns
                row1.CreateCell(27).SetCellValue("Standard Work Procedures Followed ?");//Standard Work Procedures Followed ?
                row1.CreateCell(28).SetCellValue("Procedures Followed");//Procedures Followed
                row1.CreateCell(29).SetCellValue("Was Training for this Task Provided?");//Was Training for this Task Provided?
                row1.CreateCell(30).SetCellValue("Date Associate Began Doing This Task ? ");//Date Associate Began Doing This Task ? 
            
                row1.CreateCell(31).SetCellValue("Reoccurrence");//Reoccurrence

                row1.CreateCell(32).SetCellValue("Restricted Time?");// Restricted Time?

                row1.CreateCell(33).SetCellValue("TNSK# ( Defined  by TNSK)");//TNSK#(Definded by TNSK)
                row1.CreateCell(34).SetCellValue("Business Type");//Business Type
                row1.CreateCell(35).SetCellValue("Macro Process Type");//Macro Process Type
                row1.CreateCell(36).SetCellValue("Specific Process Type");//Specific Process Type
                row1.CreateCell(37).SetCellValue("Equipment Manufacturer Name");//Equipment Manufacturer Name
                row1.CreateCell(38).SetCellValue("Equipment Manufacturer Date(MM / DD / YYYY)");//Equipment Manufacturer Date(MM / DD / YYYY)
                row1.CreateCell(39).SetCellValue("Design Number(for NSK designs only)");//Design Number(for NSK designs only
                row1.CreateCell(40).SetCellValue("Asset Number");// Asset Number
                row1.CreateCell(41).SetCellValue("Age of Associate (US and Europe  - DO NOT ENTER)");// Age of Associate (US and Europe  - DO NOT ENTER)
                row1.CreateCell(42).SetCellValue("Type of Incident");// Type of Incident(what happened ?)
                row1.CreateCell(43).SetCellValue(" Change in Medical Status");// Change in Medical Status ?



                int rownum1 = 0;
                for (int irows = 0; irows < incidentList.Count; irows++)
                {
                    INCIDENT incident = incidentList[irows].Incident;

                    //maxIncident
                    if (incident.INCIDENT_ID > maxIncidentforInjuryType)
                    {
                        maxIncident = true;
                    }

                    if (incident.INCIDENT_DT != null)
                    {
                        try
                        {
                            strIncidentDate = incident.INCIDENT_DT.ToString("MM/dd/yyyy");
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

                    if (incident.CREATE_DT.HasValue)
                    {
                        try
                        {
                            strReportDate = Convert.ToDateTime(incident.CREATE_DT).ToString("MM/dd/yyyy");
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
                        strDescription = StringHtmlExtensions.TruncateHtml(incident.DESCRIPTION, 10000, "...");
                        strDescription = WebSiteCommon.StripHTML(strDescription);
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
                                    row1.CreateCell(5).SetCellValue("");
                                    row1.CreateCell(6).SetCellValue("");
                                    row1.CreateCell(7).SetCellValue("");
                                    row1.CreateCell(8).SetCellValue("");
                                    row1.CreateCell(9).SetCellValue("");
                                    row1.CreateCell(10).SetCellValue("");
                                    row1.CreateCell(11).SetCellValue("");
                                    row1.CreateCell(12).SetCellValue("");
                                    row1.CreateCell(13).SetCellValue("");
                                    row1.CreateCell(14).SetCellValue("");
                                    row1.CreateCell(15).SetCellValue("");
                                    row1.CreateCell(16).SetCellValue(strDescription);
                                    row1.Cells[16].CellStyle.WrapText = true;                                    
                                    row1.CreateCell(17).SetCellValue("");
                                    row1.CreateCell(18).SetCellValue("");
                                    row1.CreateCell(19).SetCellValue("");
                                    row1.CreateCell(20).SetCellValue("");
                                    row1.CreateCell(21).SetCellValue("");
                                    row1.CreateCell(22).SetCellValue("");
                                    row1.CreateCell(23).SetCellValue("");
                                    row1.CreateCell(24).SetCellValue("");
                                    row1.CreateCell(25).SetCellValue("");
                                    row1.CreateCell(26).SetCellValue("");
                                    row1.CreateCell(27).SetCellValue("");
                                    row1.CreateCell(28).SetCellValue("");
                                    row1.CreateCell(29).SetCellValue("");
                                    row1.CreateCell(30).SetCellValue("");
                                    row1.CreateCell(31).SetCellValue("");
                                    row1.CreateCell(32).SetCellValue("");


                                    //If incident is newly created
                                    if (maxIncident)
                                    {                                       
                                        row1.CreateCell(33).SetCellValue("");
                                        row1.CreateCell(34).SetCellValue("");
                                        row1.CreateCell(35).SetCellValue("");
                                        row1.CreateCell(36).SetCellValue("");
                                        row1.CreateCell(37).SetCellValue("");
                                        row1.CreateCell(38).SetCellValue("");
                                        row1.CreateCell(39).SetCellValue("");
                                        row1.CreateCell(40).SetCellValue("");
                                        row1.CreateCell(41).SetCellValue("");
                                        row1.CreateCell(42).SetCellValue("");
                                        row1.CreateCell(43).SetCellValue("");
                                    }
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
                                row1.CreateCell(5).SetCellValue("");
                                row1.CreateCell(6).SetCellValue("");
                                row1.CreateCell(7).SetCellValue("");
                                row1.CreateCell(8).SetCellValue("");
                                row1.CreateCell(9).SetCellValue("");
                                row1.CreateCell(10).SetCellValue("");
                                row1.CreateCell(11).SetCellValue("");
                                row1.CreateCell(12).SetCellValue("");
                                row1.CreateCell(13).SetCellValue("");
                                row1.CreateCell(14).SetCellValue("");
                                row1.CreateCell(15).SetCellValue("");
                                row1.CreateCell(16).SetCellValue(strDescription);
                                row1.Cells[16].CellStyle.WrapText = true;
                                row1.CreateCell(17).SetCellValue("");
                                row1.CreateCell(18).SetCellValue("");
                                row1.CreateCell(19).SetCellValue("");
                                row1.CreateCell(20).SetCellValue("");
                                row1.CreateCell(21).SetCellValue("");
                                row1.CreateCell(22).SetCellValue("");
                                row1.CreateCell(23).SetCellValue("");
                                row1.CreateCell(24).SetCellValue("");
                                row1.CreateCell(25).SetCellValue("");
                                row1.CreateCell(26).SetCellValue("");
                                row1.CreateCell(27).SetCellValue("");
                                row1.CreateCell(28).SetCellValue("");
                                row1.CreateCell(29).SetCellValue("");
                                row1.CreateCell(30).SetCellValue("");
                                row1.CreateCell(31).SetCellValue("");
                                row1.CreateCell(32).SetCellValue("");

                                //If incident id greater than maxIncident
                                if (maxIncident)
                                {                                   
                                    row1.CreateCell(33).SetCellValue("");
                                    row1.CreateCell(34).SetCellValue("");
                                    row1.CreateCell(35).SetCellValue("");
                                    row1.CreateCell(36).SetCellValue("");
                                    row1.CreateCell(37).SetCellValue("");
                                    row1.CreateCell(38).SetCellValue("");
                                    row1.CreateCell(39).SetCellValue("");
                                    row1.CreateCell(40).SetCellValue("");
                                    row1.CreateCell(41).SetCellValue("");
                                    row1.CreateCell(42).SetCellValue("");
                                    row1.CreateCell(43).SetCellValue("");
                                }
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
                            row1.CreateCell(5).SetCellValue("");
                            row1.CreateCell(6).SetCellValue("");
                            row1.CreateCell(7).SetCellValue("");
                            row1.CreateCell(8).SetCellValue("");
                            row1.CreateCell(9).SetCellValue("");
                            row1.CreateCell(10).SetCellValue("");
                            row1.CreateCell(11).SetCellValue("");
                            row1.CreateCell(12).SetCellValue("");
                            row1.CreateCell(13).SetCellValue("");
                            row1.CreateCell(14).SetCellValue("");
                            row1.CreateCell(15).SetCellValue("");                           
                            row1.CreateCell(16).SetCellValue(strDescription);
                            row1.Cells[16].CellStyle.WrapText = true;
                            row1.CreateCell(17).SetCellValue("");
                            row1.CreateCell(18).SetCellValue("");
                            row1.CreateCell(19).SetCellValue("");
                            row1.CreateCell(20).SetCellValue("");
                            row1.CreateCell(21).SetCellValue("");
                            row1.CreateCell(22).SetCellValue("");
                            row1.CreateCell(23).SetCellValue("");
                            row1.CreateCell(24).SetCellValue("");
                            row1.CreateCell(25).SetCellValue("");
                            row1.CreateCell(26).SetCellValue("");
                            row1.CreateCell(27).SetCellValue("");
                            row1.CreateCell(28).SetCellValue("");
                            row1.CreateCell(29).SetCellValue("");
                            row1.CreateCell(30).SetCellValue("");
                            row1.CreateCell(31).SetCellValue("");
                            row1.CreateCell(32).SetCellValue("");

                            //If incident id greater than maxIncident
                            if (maxIncident)
                            {                                
                                row1.CreateCell(33).SetCellValue("");
                                row1.CreateCell(34).SetCellValue("");
                                row1.CreateCell(35).SetCellValue("");
                                row1.CreateCell(36).SetCellValue("");
                                row1.CreateCell(37).SetCellValue("");
                                row1.CreateCell(38).SetCellValue("");
                                row1.CreateCell(39).SetCellValue("");
                                row1.CreateCell(40).SetCellValue("");
                                row1.CreateCell(41).SetCellValue("");
                                row1.CreateCell(42).SetCellValue("");
                                row1.CreateCell(43).SetCellValue("");
                            }
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

                        //new columns are added.

                        INCFORM_INJURYILLNESS injuryIncident = (from II in entities.INCFORM_INJURYILLNESS where II.INCIDENT_ID == incident.INCIDENT_ID select II).FirstOrDefault();
                       
                        try
                        {
                            string strisFirstAid = "";
                            if (injuryIncident.FIRST_AID != null)
                                isFirstAid = injuryIncident.FIRST_AID;
                            else
                                isFirstAid = false;

                            if (isFirstAid == true)
                            {
                                strisFirstAid = "Yes";
                            }
                            else
                            {
                                strisFirstAid = "No";
                            }
                            row1.CreateCell(5).SetCellValue(strisFirstAid);

                        }
                        catch
                        {
                            row1.CreateCell(5).SetCellValue("");
                        }

                        try
                        {
                            if (injuryIncident.RECORDABLE)
                                row1.CreateCell(6).SetCellValue("Yes");
                            else
                                row1.CreateCell(6).SetCellValue("No");
                        }
                        catch
                        {
                            row1.CreateCell(6).SetCellValue("");
                        }

                        try
                        {
                            if (injuryIncident.LOST_TIME)
                                row1.CreateCell(7).SetCellValue("Yes");
                            else
                                row1.CreateCell(7).SetCellValue("No");
                        }
                        catch
                        {
                            row1.CreateCell(7).SetCellValue("");
                        }

                        try
                        {
                            if (!(string.IsNullOrEmpty(injuryIncident.INITIAL_TREATMENT_GIVEN)))
                            {
                                strInitialTreatmentGiven = injuryIncident.INITIAL_TREATMENT_GIVEN;
                                string[] treatment_Given = strInitialTreatmentGiven.Split(',');
                                string result = "";
                                foreach (var word in treatment_Given)
                                {
                                    if (word == "2")
                                    {
                                        result = result + " " + "Employee sent to outside medical facility" + ",";
                                    }
                                    else
                                        result = result + " " + Convert.ToString((from x in entities.XLAT where x.XLAT_GROUP == "ITG" && x.XLAT_CODE == word select x.DESCRIPTION).FirstOrDefault()) + ",";

                                }
                                string Treatment = (result.TrimEnd(',')).Trim();
                                row1.CreateCell(8).SetCellValue(Treatment);
                            }
                            else
                            {
                                strInitialTreatmentGiven = "";
                                row1.CreateCell(8).SetCellValue(strInitialTreatmentGiven);// Initial Treatment Given
                            }
                        }
                        catch
                        {
                            row1.CreateCell(8).SetCellValue("");// Initial Treatment Given
                        }

                        try
                        {
                            if (injuryIncident.FATALITY.HasValue)
                            {
                                string strisFatality = "";
                                isFatality = bool.Parse((injuryIncident.FATALITY).ToString());

                                if (isFatality == true)
                                {
                                    strisFatality = "Yes";
                                }
                                else
                                {
                                    strisFatality = "No";
                                }
                                row1.CreateCell(9).SetCellValue(strisFatality);
                            }
                            else
                            {
                                isFatality = false;
                                row1.CreateCell(9).SetCellValue("No");
                            }

                        }
                        catch
                        {
                            row1.CreateCell(9).SetCellValue("");
                        }

                        try
                        {
                            string XLAT_GROUP = "HS_L2REPORT";

                            string XLAT_CODE = (from II in entities.INCFORM_APPROVAL
                                                where II.INCIDENT_ID == incident.INCIDENT_ID
                                                select II.SEVERITY_LEVEL).FirstOrDefault();
                            if (XLAT_CODE != null)
                            {
                                strAnswerValue = (from X in entities.XLAT
                                                  where X.XLAT_GROUP == XLAT_GROUP && X.XLAT_CODE == XLAT_CODE
                                                  select X.DESCRIPTION).FirstOrDefault();
                                if (strAnswerValue != null)
                                {
                                    row1.CreateCell(10).SetCellValue(strAnswerValue);

                                }
                                else
                                    row1.CreateCell(10).SetCellValue("");
                            }
                            else
                                row1.CreateCell(10).SetCellValue("");

                        }
                        catch
                        {
                            row1.CreateCell(10).SetCellValue("");
                        }

                        try
                        {
                            //section for Injury Type data.
                            // Injury Type data.

                            string XLAT_GROUP = "INJURY_TYPE";

                            strAnswerValue = (from II in entities.INCFORM_INJURYILLNESS
                                              join XT in entities.XLAT on II.INJURY_TYPE equals XT.XLAT_CODE
                                              where II.INCIDENT_ID == incident.INCIDENT_ID && XT.XLAT_GROUP == XLAT_GROUP && XT.XLAT_LANGUAGE == XLAT_LANGUAGE
                                              select XT.DESCRIPTION).FirstOrDefault();


                            if (strEmployeeDate != null)
                            {
                                row1.CreateCell(11).SetCellValue(strAnswerValue);
                            }
                            else
                                row1.CreateCell(11).SetCellValue("");
                        }
                        catch { row1.CreateCell(11).SetCellValue(""); }


                        try
                        {
                            //section for Body Part Affected data.
                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 13); // Body Part Affected data.

                            string XLAT_GROUP = "INJURY_PART";

                            strAnswerValue = (from II in entities.INCFORM_INJURYILLNESS
                                              join XT in entities.XLAT on II.INJURY_BODY_PART equals XT.XLAT_CODE
                                              where II.INCIDENT_ID == incident.INCIDENT_ID && XT.XLAT_GROUP == XLAT_GROUP && XT.XLAT_LANGUAGE == XLAT_LANGUAGE
                                              select XT.DESCRIPTION).FirstOrDefault();

                            if (strEmployeeDate != null)
                            {
                                row1.CreateCell(12).SetCellValue(strAnswerValue);

                            }
                            else
                                row1.CreateCell(12).SetCellValue("");
                        }
                        catch { row1.CreateCell(12).SetCellValue(""); }


                        //section for Employee date.
                        try
                        {

                            DateTime? EmployeeDate = new DateTime();
                            PERSON ObjPerson = new PERSON();
                            //Get the values from task status.
                            EmployeeDate = (from P in entities.PERSON
                                            join TS in entities.TASK_STATUS on P.PERSON_ID equals TS.RESPONSIBLE_ID
                                            where TS.RECORD_ID == incident.INCIDENT_ID
                                            select P.CREATE_DT).FirstOrDefault();

                            ObjPerson.CREATE_DT = EmployeeDate;
                            strEmployeeDate = Convert.ToDateTime(ObjPerson.CREATE_DT).ToString("MM/dd/yyyy");

                            if (strEmployeeDate == "01/01/0001")
                            {
                                strEmployeeDate = "";
                            }



                            //test = EmployeeDate;
                            // strEmployeeDate = Convert.ToString(EmployeeDate.ToString("MM/dd/yyyy"));

                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 69); // corrective actions
                            if (strEmployeeDate != null)
                            {
                                row1.CreateCell(13).SetCellValue(strEmployeeDate);
                            }
                            else
                                row1.CreateCell(13).SetCellValue("");
                        }
                        catch { row1.CreateCell(13).SetCellValue(""); }

                        //section for Employee task date.
                        try
                        {
                            DateTime? EmployeeTaskDate = new DateTime();

                            TASK_STATUS ObjTaskStatus = new TASK_STATUS();
                            //Get the values from task status.
                            EmployeeTaskDate = (from TS in entities.TASK_STATUS
                                                where TS.RECORD_ID == incident.INCIDENT_ID
                                                select TS.CREATE_DT).FirstOrDefault();

                            ObjTaskStatus.CREATE_DT = EmployeeTaskDate;
                            strEmployeeTaskDate = Convert.ToDateTime(ObjTaskStatus.CREATE_DT).ToString("MM/dd/yyyy");

                            if (strEmployeeTaskDate == "01/01/0001")
                            {
                                strEmployeeTaskDate = "";
                            }

                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 69); // corrective actions
                            if (strEmployeeTaskDate != null)
                            {
                                row1.CreateCell(14).SetCellValue(strEmployeeTaskDate);

                            }
                            else
                                row1.CreateCell(14).SetCellValue("");
                        }

                        catch { row1.CreateCell(14).SetCellValue(""); }


                        //section for Employee task date.
                        try
                        {

                            //Get the values from task status.
                            strJobTitle = (from P in entities.PERSON
                                           join TS in entities.TASK_STATUS on P.PERSON_ID equals TS.RESPONSIBLE_ID
                                           where TS.RECORD_ID == incident.INCIDENT_ID
                                           select P.JOB_TITLE).FirstOrDefault();

                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 69); // corrective actions
                            if (strJobTitle != null)
                            {
                                row1.CreateCell(15).SetCellValue(strJobTitle);
                            }
                            else
                                row1.CreateCell(15).SetCellValue("");
                        }

                        catch { row1.CreateCell(15).SetCellValue(""); }



                        row1.CreateCell(16).SetCellValue(strDescription);
                        row1.Cells[16].CellStyle.WrapText = true;

                        try
                        {
                            // root cause operational control
                            //Declare the varible and get the values for INJURY_CAUSE only.
                            string XLAT_GROUP = "INJURY_CAUSE";

                            strAnswerValue = (from IR in entities.INCFORM_ROOT5Y
                                              join IC in entities.INCFORM_CAUSATION on IR.INCIDENT_ID equals IC.INCIDENT_ID
                                              join XT in entities.XLAT on IC.CAUSEATION_CD equals XT.XLAT_CODE
                                              where IR.INCIDENT_ID == incident.INCIDENT_ID && XT.XLAT_GROUP == XLAT_GROUP && IR.IS_ROOTCAUSE == true && XT.XLAT_LANGUAGE == XLAT_LANGUAGE
                                              select XT.DESCRIPTION).FirstOrDefault();




                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 78); // root cause operational control
                            if (strAnswerValue != null)
                                row1.CreateCell(17).SetCellValue(strAnswerValue);
                            else
                                row1.CreateCell(17).SetCellValue("");
                        }
                        catch { row1.CreateCell(17).SetCellValue(""); }
                        try
                        {

                            string answerText = null;

                            //Get the values from task status.
                            strAnswerValue = (from a in entities.TASK_STATUS
                                              where a.RECORD_TYPE == 40 &&
                                              a.RECORD_ID == incident.INCIDENT_ID
                                              select a.DESCRIPTION).FirstOrDefault();

                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 69); // corrective actions
                            if (strAnswerValue != null)
                            {
                                row1.CreateCell(18).SetCellValue(strAnswerValue);
                                row1.Cells[18].CellStyle.WrapText = true;
                            }
                            else
                                row1.CreateCell(18).SetCellValue("");
                        }
                        catch { row1.CreateCell(18).SetCellValue(""); }
                        try
                        {
                            //get the responsible person name with the help of relation with task status reponsible id.
                            strAnswerValue = (from TS in entities.TASK_STATUS
                                              join P in entities.PERSON on TS.RESPONSIBLE_ID equals P.PERSON_ID
                                              where TS.RECORD_ID == incident.INCIDENT_ID
                                              select P.FIRST_NAME.Trim() + " " + P.LAST_NAME.Trim()
                                              ).FirstOrDefault();

                            //strAnswerValue = EHSIncidentMgr.SelectIncidentAnswer(incident, 64); // responsible person
                            if (strAnswerValue != null)
                                row1.CreateCell(19).SetCellValue(strAnswerValue);
                            else
                                row1.CreateCell(19).SetCellValue("");
                        }
                        catch { row1.CreateCell(19).SetCellValue(""); }

                        row1.CreateCell(20).SetCellValue(strDueDate);

                        if (injuryIncident != null)
                        {
                            try
                            {
                                if (!(string.IsNullOrEmpty(injuryIncident.INCIDENT_TIME.ToString())))
                                    strTimeOfIncident = injuryIncident.INCIDENT_TIME.ToString();
                                else
                                    strTimeOfIncident = "";
                                row1.CreateCell(21).SetCellValue(strTimeOfIncident);
                            }
                            catch
                            {
                                row1.CreateCell(21).SetCellValue("");
                            }

                            try
                            {
                                if (!(string.IsNullOrEmpty(injuryIncident.SHIFT)))
                                {
                                    strShift = injuryIncident.SHIFT;
                                    if (strShift == "01" || strShift == "1")
                                    {
                                        row1.CreateCell(22).SetCellValue("First Shift");
                                    }
                                    else if (strShift == "02" || strShift == "2")
                                    {
                                        row1.CreateCell(22).SetCellValue("Second Shift");
                                    }
                                    else if (strShift == "03" || strShift == "3")
                                    {
                                        row1.CreateCell(22).SetCellValue("Third Shift");
                                    }
                                }

                                else
                                {
                                    strShift = "";
                                    row1.CreateCell(22).SetCellValue(strShift);
                                }
                            }
                            catch
                            {
                                row1.CreateCell(22).SetCellValue("");
                            }

                            try
                            {
                                if (!(string.IsNullOrEmpty(injuryIncident.DEPARTMENT)))
                                    strDepartment = injuryIncident.DEPARTMENT;
                                else
                                    strDepartment = "";
                                row1.CreateCell(23).SetCellValue(strDepartment);
                            }
                            catch
                            {
                                row1.CreateCell(23).SetCellValue("");
                            }
                            
                            try
                            {
                                if (!(string.IsNullOrEmpty(injuryIncident.INSIDE_OUTSIDE_BLDNG)))
                                    strInsideOrOutsideBuilding = injuryIncident.INSIDE_OUTSIDE_BLDNG;
                                else
                                    strInsideOrOutsideBuilding = "";
                                row1.CreateCell(24).SetCellValue(strInsideOrOutsideBuilding);
                            }
                            catch
                            {
                                row1.CreateCell(24).SetCellValue("");
                            }


                            try
                            {
                                if (incident.INCIDENT_ID > maxINCIDENT)
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.EMP_STATUS.ToString())))
                                    {
                                        strEmployeeStatus = injuryIncident.EMP_STATUS.ToString();
                                        if (strEmployeeStatus == "2")
                                        {
                                            row1.CreateCell(25).SetCellValue("Temporary Employee");
                                        }
                                        else if (strEmployeeStatus == "0")
                                        {
                                            row1.CreateCell(25).SetCellValue("Contractor Employee");
                                        }
                                        else if (strEmployeeStatus == "1")
                                        {
                                            row1.CreateCell(25).SetCellValue("Permanent Employee");
                                        }
                                    }
                                    else
                                    {
                                        strEmployeeStatus = "";
                                        row1.CreateCell(25).SetCellValue(strEmployeeStatus);
                                    }
                                }
                                else
                                {
                                    if (injuryIncident.COMPANY_SUPERVISED != null)
                                    {
                                        strEmployeeStatus = Convert.ToString(injuryIncident.COMPANY_SUPERVISED);
                                        if (strEmployeeStatus == "True")
                                        {
                                            row1.CreateCell(25).SetCellValue("Yes");
                                        }
                                        else if (strEmployeeStatus == "False")
                                        {
                                            row1.CreateCell(25).SetCellValue("No");
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                row1.CreateCell(25).SetCellValue("");
                            }

                            try
                            {
                                if (injuryIncident.ERGONOMIC_CONCERN)
                                {
                                    isErgonomicConcerns = injuryIncident.ERGONOMIC_CONCERN;
                                    row1.CreateCell(26).SetCellValue("Yes");
                                }
                                else
                                {
                                    isErgonomicConcerns = false;
                                    row1.CreateCell(26).SetCellValue("No");
                                }
                            }
                            catch
                            {
                                row1.CreateCell(26).SetCellValue("");
                            }

                            try
                            {
                                if (injuryIncident.STD_PROCS_FOLLOWED != null)
                                {
                                    isStandardWorkProcedures = injuryIncident.STD_PROCS_FOLLOWED;
                                    if (isStandardWorkProcedures)
                                    { row1.CreateCell(27).SetCellValue("Standard"); }
                                    else
                                        row1.CreateCell(27).SetCellValue("Non-Standard");
                                }
                                else
                                {
                                    //  isStandardWorkProcedures = false;
                                    row1.CreateCell(27).SetCellValue("");
                                }
                            }
                            catch
                            {
                                row1.CreateCell(27).SetCellValue("");
                            }

                            try
                            {
                                if (!(string.IsNullOrEmpty(injuryIncident.STD_PROCS_DESC)))
                                    strProceduresFollowed = injuryIncident.STD_PROCS_DESC;
                                else
                                    strProceduresFollowed = "";
                                row1.CreateCell(28).SetCellValue(strProceduresFollowed);
                            }
                            catch
                            {
                                row1.CreateCell(28).SetCellValue("");
                            }

                            try
                            {
                                if (injuryIncident.TRAINING_PROVIDED)
                                {
                                    isTrainingProvided = injuryIncident.TRAINING_PROVIDED;
                                    row1.CreateCell(29).SetCellValue("Yes");
                                }
                                else
                                {
                                    isTrainingProvided = false;
                                    row1.CreateCell(29).SetCellValue("No");
                                }
                            }
                            catch
                            {
                                row1.CreateCell(29).SetCellValue("");
                            }

                            try
                            {
                                if ((injuryIncident.ASSOCIATE_MONTHS != null) && (injuryIncident.ASSOCIATE_YEAR != null))
                                { strDateAssociateBegan = (injuryIncident.ASSOCIATE_MONTHS).ToString() + "/" + (injuryIncident.ASSOCIATE_YEAR).ToString(); }
                                else
                                    strDateAssociateBegan = "";
                                row1.CreateCell(30).SetCellValue(strDateAssociateBegan);
                            }
                            catch
                            {
                                row1.CreateCell(30).SetCellValue("");
                            }

                            try
                            {
                                string StrREOCCUR = "";
                                if ((injuryIncident.REOCCUR).HasValue)
                                {
                                    isReoccur = bool.Parse((injuryIncident.REOCCUR).ToString());

                                    if (isReoccur == true)
                                    {
                                        StrREOCCUR = "Yes";
                                    }
                                    else
                                    {
                                        StrREOCCUR = "No";
                                    }
                                    row1.CreateCell(31).SetCellValue(StrREOCCUR);
                                }


                                else
                                { row1.CreateCell(31).SetCellValue(""); }
                            }
                            catch
                            {
                                row1.CreateCell(31).SetCellValue("");
                            }


                            try
                            {
                                string StrrestrictedTime = "";
                                if (injuryIncident.RESTRICTED_TIME != null)
                                    isRestrictedTime = injuryIncident.RESTRICTED_TIME;
                                else
                                    isRestrictedTime = false;
                                if (isRestrictedTime == true)
                                {
                                    StrrestrictedTime = "Yes";
                                }
                                else
                                {
                                    StrrestrictedTime = "No";
                                }
                                row1.CreateCell(32).SetCellValue(StrrestrictedTime);
                            }
                            catch
                            {
                                row1.CreateCell(32).SetCellValue("");
                            }

                            if (incident.INCIDENT_ID > maxIncidentforInjuryType)
                            {
                                //try
                                //{
                                INCIDENT incidnt = (from I in entities.INCIDENT where I.INCIDENT_ID == incident.INCIDENT_ID select I).FirstOrDefault();

                                if (incidnt != null)
                                {
                                    try
                                    {
                                        if (!(string.IsNullOrEmpty(incidnt.TNSKNumber)))
                                            strTNSK = incidnt.TNSKNumber;
                                        else
                                            strTNSK = "";

                                        row1.CreateCell(33).SetCellValue(strTNSK);//TNSK#(Definded by TNSK)                                      
                                    }
                                    catch
                                    {
                                        row1.CreateCell(33).SetCellValue("");
                                    }
                                }
                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.BUSINESS_TYPE)))
                                    {
                                        strBusinessType = injuryIncident.BUSINESS_TYPE;
                                        string BT = Convert.ToString((from X in entities.XLAT where X.XLAT_GROUP == "BusinessType" && X.XLAT_CODE == strBusinessType select X.DESCRIPTION).FirstOrDefault());
                                        row1.CreateCell(34).SetCellValue(BT);
                                    }
                                    else
                                    {
                                        strBusinessType = "";
                                        row1.CreateCell(34).SetCellValue(strBusinessType);//Business Type
                                    }

                                }
                                catch
                                {
                                    row1.CreateCell(34).SetCellValue("");//Business Type
                                }

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.MACRO_PROCESS_TYPE)))
                                    {
                                        strMacroProcessType = injuryIncident.MACRO_PROCESS_TYPE;
                                        if (injuryIncident.BUSINESS_TYPE != null)
                                        {
                                            strBusinessType = injuryIncident.BUSINESS_TYPE;
                                            //string BT = Convert.ToString((from X in entities.XLAT where X.XLAT_GROUP == "BusinessType" && X.XLAT_CODE == strBusinessType select X.DESCRIPTION).FirstOrDefault());

                                            Macro_Process = Convert.ToString((from X in entities.XLAT where X.XLAT_GROUP == strBusinessType && X.XLAT_CODE == strMacroProcessType select X.DESCRIPTION).FirstOrDefault());
                                            row1.CreateCell(35).SetCellValue(Macro_Process);
                                        }
                                        else
                                        {
                                            strMacroProcessType = "";
                                            row1.CreateCell(35).SetCellValue(strMacroProcessType);//Macro Process Type
                                        }
                                    }
                                    else
                                    {
                                        strMacroProcessType = "";
                                        row1.CreateCell(35).SetCellValue(strMacroProcessType);//Macro Process Type
                                    }

                                }
                                catch
                                {
                                    row1.CreateCell(35).SetCellValue("");//Macro Process Type
                                }

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.SPECIFIC_PROCESS_TYPE)))
                                    {
                                        strSpecificProcessType = injuryIncident.SPECIFIC_PROCESS_TYPE;
                                        if (!(string.IsNullOrEmpty(injuryIncident.MACRO_PROCESS_TYPE)))
                                        {
                                            strMacroProcessType = injuryIncident.MACRO_PROCESS_TYPE;

                                            {
                                                string SPT = Convert.ToString((from X in entities.XLAT where X.XLAT_GROUP == strMacroProcessType && X.XLAT_CODE == strSpecificProcessType select X.DESCRIPTION).FirstOrDefault());
                                                row1.CreateCell(36).SetCellValue(SPT);
                                            }
                                        }
                                        else
                                        {
                                            row1.CreateCell(36).SetCellValue("");
                                        }
                                    }
                                    else
                                    {
                                        strSpecificProcessType = "";
                                        row1.CreateCell(36).SetCellValue(strSpecificProcessType);//Specific Process Type
                                    }
                                }
                                catch
                                {
                                    row1.CreateCell(36).SetCellValue("");//Specific Process Type
                                }

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.EQUIPMENT_MANUFACTURER_NAME)))
                                        strEquipmentManufacturerName = injuryIncident.EQUIPMENT_MANUFACTURER_NAME;
                                    else
                                        strEquipmentManufacturerName = "";
                                    row1.CreateCell(37).SetCellValue(strEquipmentManufacturerName);//Equipment Manufacturer Name
                                }
                                catch
                                {
                                    row1.CreateCell(37).SetCellValue("");//Equipment Manufacturer Name
                                }

                                try
                                {
                                    if (injuryIncident.EQUIPEMENT_MANUFACTURER_DATE != null)
                                    {
                                        DateTime Manufature_Date = DateTime.Parse((injuryIncident.EQUIPEMENT_MANUFACTURER_DATE).ToString());
                                        strEquipmentManufacturerDate = Manufature_Date.ToString("MM-dd-yyyy");
                                    }
                                    else
                                        strEquipmentManufacturerDate = "";

                                    row1.CreateCell(38).SetCellValue(strEquipmentManufacturerDate);//Equipment Manufacturer Date(MM / DD / YYYY)

                                }
                                catch
                                {
                                    row1.CreateCell(38).SetCellValue("");//Equipment Manufacturer Date(MM / DD / YYYY)

                                }
                               

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.DESIGN_NUMBER)))
                                        strDesignNumber = injuryIncident.DESIGN_NUMBER;
                                    else
                                        strDesignNumber = "";
                                    row1.CreateCell(39).SetCellValue(strDesignNumber);//Design Number(for NSK designs only                                           
                                }
                                catch
                                {
                                    row1.CreateCell(39).SetCellValue("");//Design Number(for NSK designs only                                          
                                }

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.ASSET_NUMBER)))
                                        strAssetNumber = injuryIncident.ASSET_NUMBER;
                                    else
                                        strAssetNumber = "";
                                    row1.CreateCell(40).SetCellValue(strAssetNumber);// Asset Number

                                }
                                catch
                                {
                                    row1.CreateCell(40).SetCellValue("");// Asset Number

                                }
                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.AGE_OF_ASSOCIATE)))
                                        strAgeofAssociate = injuryIncident.AGE_OF_ASSOCIATE;
                                    else
                                        strAgeofAssociate = "";
                                    row1.CreateCell(41).SetCellValue(strAgeofAssociate);// Age of Associate (US and Europe  - DO NOT ENTER)                                         
                                }
                                catch
                                {
                                    row1.CreateCell(41).SetCellValue("");// Age of Associate (US and Europe  - DO NOT ENTER)
                                }

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.TYPE_OF_INCIDENT)))
                                    {
                                        strTypeofIncident = injuryIncident.TYPE_OF_INCIDENT;

                                        string Incident_type = Convert.ToString((from x in entities.XLAT where x.XLAT_GROUP == "INCIDENT_TYPE" && x.XLAT_CODE == strTypeofIncident select x.DESCRIPTION).FirstOrDefault());
                                        row1.CreateCell(42).SetCellValue(Incident_type);
                                    }
                                    else
                                    {
                                        strTypeofIncident = "";
                                        row1.CreateCell(42).SetCellValue(strTypeofIncident);// Type of Incident(what happened ?)
                                    }
                                }
                                catch
                                {
                                    row1.CreateCell(42).SetCellValue("");// Type of Incident(what happened ?)                                         
                                }                                

                                try
                                {
                                    if (!(string.IsNullOrEmpty(injuryIncident.CHANGE_MEDICAL_STATUS)))
                                    {
                                        strChangeMedicalStatus = injuryIncident.CHANGE_MEDICAL_STATUS;
                                        string[] Medical_status = strChangeMedicalStatus.Split(',');
                                        string result = "";
                                        foreach (var word in Medical_status)
                                        {
                                            result = result + " " + Convert.ToString((from x in entities.XLAT where x.XLAT_GROUP == "CMS" && x.XLAT_CODE == word select x.DESCRIPTION).FirstOrDefault()) + ",";
                                        }
                                        string Medical_Sta = (result.TrimEnd(',')).Trim();
                                        row1.CreateCell(43).SetCellValue(Medical_Sta);
                                    }
                                    else
                                    {
                                        strChangeMedicalStatus = "";

                                        row1.CreateCell(43).SetCellValue(strChangeMedicalStatus);// Change in Medical Status ?
                                    }
                                }
                                catch
                                {
                                    row1.CreateCell(43).SetCellValue("");// Change in Medical Status ?
                                }

                            }

                        }




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
                sheet1.SetColumnWidth(16, 10000); // text fields should not auto size
                sheet1.AutoSizeColumn(17);
                sheet1.AutoSizeColumn(18);
                sheet1.AutoSizeColumn(19);
                sheet1.AutoSizeColumn(20);
                sheet1.AutoSizeColumn(21);
                sheet1.AutoSizeColumn(22);
                sheet1.AutoSizeColumn(23);
                sheet1.AutoSizeColumn(24);
                sheet1.AutoSizeColumn(25);
                sheet1.AutoSizeColumn(26);
                sheet1.AutoSizeColumn(27);
                sheet1.AutoSizeColumn(28);
                sheet1.AutoSizeColumn(29);
                sheet1.AutoSizeColumn(30);
                sheet1.AutoSizeColumn(31);
                sheet1.AutoSizeColumn(32);
                sheet1.AutoSizeColumn(33);
                sheet1.AutoSizeColumn(34);
                sheet1.AutoSizeColumn(35);
                sheet1.AutoSizeColumn(36);
                sheet1.AutoSizeColumn(37);
                sheet1.AutoSizeColumn(38);
                sheet1.AutoSizeColumn(39);
                sheet1.AutoSizeColumn(40);
                sheet1.AutoSizeColumn(41);
                sheet1.AutoSizeColumn(42);
                sheet1.AutoSizeColumn(43);
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
                        strDescription = StringHtmlExtensions.TruncateHtml(data.Incident.DESCRIPTION, 10000, "...");
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
                        row1.CreateCell(10).SetCellValue(WebSiteCommon.GetXlatValue("incidentStatus", data.Status));
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
                        strDescription = StringHtmlExtensions.TruncateHtml(EHSIncidentMgr.SelectIncidentAnswer(data.Incident, 86), 10000, "...");
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

        #region GHGreport
        public void ExportGHGReportExcel(EHSModel.GHGResultList GHGTable)
        {

            try
            {
                string filename = "GHGReport.xls";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.Clear();
                HSSFWorkbook hssfworkbook = InitializeWorkbook();
                ISheet sheet1 = hssfworkbook.CreateSheet("GHG Emissions");

                ICellStyle cellStyleNumeric = hssfworkbook.CreateCellStyle();
                cellStyleNumeric.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");

                //// create the header - Plant Name, DUNS Code, Measure, Measure Name, Period Year, Period Month, Period from Date, Period To Date, Value, UOM, UOM Name, Input Value, Cost, Currency, Input Cost, Input Currency

                int rownum = 0;
                IRow row = sheet1.CreateRow(rownum);

                row.CreateCell(0).SetCellValue("Plant Name");
                row.CreateCell(1).SetCellValue("Emissions Scope");
                row.CreateCell(2).SetCellValue("Fuel");
                row.CreateCell(3).SetCellValue("Quantity");
                row.CreateCell(4).SetCellValue("UOM");
                row.CreateCell(5).SetCellValue("Gas");
                row.CreateCell(6).SetCellValue("GWP Factor");
                row.CreateCell(7).SetCellValue("GHG Factor");
                row.CreateCell(8).SetCellValue("Emissions");
                row.CreateCell(9).SetCellValue("UOM");


                foreach (PLANT plant in GHGTable.ResultList.Select(l => l.Plant).Distinct().ToList())
                {
                    List<EHSModel.GHGResult> fuelList = GHGTable.ResultList.Where(l => l.Plant.PLANT_ID == plant.PLANT_ID).ToList();

                    // scope 1
                    foreach (EHSModel.GHGResult ghgRrec in fuelList.Where(l => l.EFMType != "P" && l.EFMType != "HW" && l.EFMType != "STEAM" && l.GasSeq == 1).Distinct().ToList())
                    {
                        foreach (EHSModel.GHGResult rslt in fuelList.Where(r => r.Plant.PLANT_ID == plant.PLANT_ID && r.EFMType == ghgRrec.EFMType).ToList())
                        {
                            row = sheet1.CreateRow(++rownum);
                            try
                            {
                                row.CreateCell(0).SetCellValue(plant.PLANT_NAME);
                                row.CreateCell(1).SetCellValue("1");
                                row.CreateCell(2).SetCellValue(SessionManager.EFMList.Where(l => l.EFM_TYPE == rslt.EFMType).Select(l => l.DESCRIPTION).FirstOrDefault());
                                row.CreateCell(3).SetCellValue(SQMBasePage.FormatValue(rslt.MetricValue, 4));
                                row.CreateCell(4).SetCellValue(SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == rslt.MetricUOM).UOM_CD);
                                row.CreateCell(5).SetCellValue(rslt.GasCode);
                                row.CreateCell(6).SetCellValue(SQMBasePage.FormatValue(rslt.GWPFactor, 4));
                                row.CreateCell(7).SetCellValue(SQMBasePage.FormatValue(rslt.GHGFactor, 9));
                                row.CreateCell(8).SetCellValue(SQMBasePage.FormatValue(rslt.GHGValue, 4));
                                row.CreateCell(9).SetCellValue(SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == rslt.GHGUOM).UOM_CD);
                            }
                            catch
                            {
                            }
                        }
                    }

                    // scope 2
                    foreach (EHSModel.GHGResult ghgRec in fuelList.Where(l => l.EFMType == "P" || l.EFMType == "HW" || l.EFMType == "STEAM" && l.GasSeq == 1).Distinct().ToList())
                    {
                        foreach (EHSModel.GHGResult rslt in fuelList.Where(r => r.Plant.PLANT_ID == plant.PLANT_ID && r.EFMType == ghgRec.EFMType).ToList())
                        {
                            row = sheet1.CreateRow(++rownum);
                            try
                            {
                                row.CreateCell(0).SetCellValue(plant.PLANT_NAME);
                                row.CreateCell(1).SetCellValue("2");
                                row.CreateCell(2).SetCellValue(SessionManager.EFMList.Where(l => l.EFM_TYPE == rslt.EFMType).Select(l => l.DESCRIPTION).FirstOrDefault());
                                row.CreateCell(3).SetCellValue(SQMBasePage.FormatValue(rslt.MetricValue, 4));
                                row.CreateCell(4).SetCellValue(SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == rslt.MetricUOM).UOM_CD);
                                row.CreateCell(5).SetCellValue(rslt.GasCode);
                                row.CreateCell(6).SetCellValue(SQMBasePage.FormatValue(rslt.GWPFactor, 4));
                                row.CreateCell(7).SetCellValue(SQMBasePage.FormatValue(rslt.GHGFactor, 9));
                                row.CreateCell(8).SetCellValue(SQMBasePage.FormatValue(rslt.GHGValue, 4));
                                row.CreateCell(9).SetCellValue(SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == rslt.GHGUOM).UOM_CD);
                            }
                            catch
                            {
                            }
                        }
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