﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SQM.Website.Reports
{

    public partial class EHS_48HourReport : System.Web.UI.Page
    {
        int maxINCIDENT = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncident"]);
        int maxIncidentforInjuryType = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxIncidentIDforInjuryType"]);
        static float baseWidth = 540f;
        static float mult = 1.0f;

        public List<XLAT> reportXLAT
        {
            get { return ViewState["ReportXLAT"] == null ? null : (List<XLAT>)ViewState["ReportXLAT"]; }
            set { ViewState["ReportXLAT"] = value; }
        }

        public iTextSharp.text.Font detailHdrFont;
        public iTextSharp.text.Font detailLblFont;
        public iTextSharp.text.Font infoFont;
        public iTextSharp.text.Font detailTxtFont;
        public iTextSharp.text.Font detailTxtBoldFont;
        public iTextSharp.text.Font detailTxtBoldItalicFont;
        public iTextSharp.text.Font detailTxtItalicFont;
        public iTextSharp.text.Font labelTxtFont;
        public iTextSharp.text.Font colHdrFont;
        string baseApplicationUrl;
        private bool isVideoAvailable = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowPdf(BuildPdf());
        }

        private void ShowPdf(byte[] strS)
        {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=Incident-48HourReport-" + SessionManager.UserContext.LocalTime.ToString("yyyy-MM-dd") + ".pdf");

            Response.BinaryWrite(strS);
            Response.End();
            Response.Flush();
            Response.Clear();
        }

        private byte[] BuildPdf()
        {
            reportXLAT = SQMBasePage.SelectXLATList(new string[28] { "HS_5PHASE", "CMS", "BusinessType", "BT_01", "BT_02", "BT_03", "BT_04", "MPT_S_01", "MPT_S_16", "MPT_S_18", "MPT_S_19", "MPT_B_09", "MPT_B_10", "MPT_B_23", "MPT_P_05", "MPT_P_06", "MPT_P_10", "MPT_P_11", "MPT_P_13", "MPT_AEL_1", "MPT_AEL_2", "HS_L2REPORT", "HS_L4REPORT", "TRUEFALSE", "INJURY_CAUSE", "ACTION_CATEGORY", "TASK_STATUS", "INCIDENT_APPROVALS" }, 1);

            AlertData pageData;

            baseApplicationUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");

            if (Request.QueryString["iid"] != null)
            {
                string query = Request.QueryString["iid"];
                query = query.Replace(" ", "+");
                string iid = EncryptionManager.Decrypt(query);
                pageData = PopulateByIncidentId(Convert.ToDecimal(iid));
            }
            else
            {
                return null;
            }

            if (pageData.incidentType == "Fire/Small Fire")
            {
                return BuildFirePdf(pageData);
            }
            else
            {
                return BuildInjuryIllenessPdf(pageData);
            }

        }

        #region Fire/SmallFire Report
        private byte[] BuildFirePdf(AlertData pageData)
        {
            string customerLogo = "";
            if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"] != null)
                customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"].ToString();
            if (string.IsNullOrEmpty(customerLogo))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"] != null)
                    customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();
            }
            if (string.IsNullOrEmpty(customerLogo))
                customerLogo = "MetricsoftLogo.png";

            string logoUrl = baseApplicationUrl + "images/company/" + customerLogo;

            BaseColor darkGrayColor = new BaseColor(0.25f, 0.25f, 0.25f);
            BaseColor lightGrayColor = new BaseColor(0.5f, 0.5f, 0.5f);
            BaseColor whiteColor = new BaseColor(1.0f, 1.0f, 1.0f);
            BaseColor blackColor = new BaseColor(0.0f, 0.0f, 0.0f);
            BaseColor redColor = new BaseColor(1.0f, 0f, 0f);

            Font textFont = GetTextFont();
            Font headerFont = GetHeaderFont();
            Font labelFont = GetLabelFont();
            Font colHeaderFont = GetTextFont();
            Font textItalicFont = GetTextFont();

            detailHdrFont = new Font(headerFont.BaseFont, 13, 0, lightGrayColor);
            detailTxtFont = new Font(textFont.BaseFont, 10, 0, blackColor);
            labelTxtFont = new Font(labelFont.BaseFont, 12, 0, blackColor);
            colHdrFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.UNDERLINE + iTextSharp.text.Font.BOLD, blackColor);
            detailTxtItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
            detailTxtBoldItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD + iTextSharp.text.Font.ITALIC, redColor);
            detailTxtBoldFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD, blackColor);

            // Create new PDF document
            //Document document = new Document(PageSize.A4, 35f, 35f, 35f, 35f);
            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 10, 10);  // landscape document ?
            mult = 1.3f;

            using (MemoryStream output = new MemoryStream())
            {

                PdfWriter.GetInstance(document, output);

                try
                {
                    document.Open();

                    //
                    // Table 1 - Header
                    //

                    var table1 = new PdfPTable(new float[] { 162f, 378f });
                    table1.TotalWidth = baseWidth * mult;
                    table1.LockedWidth = true;
                    table1.DefaultCell.Border = 0;
                    table1.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    table1.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    table1.SpacingAfter = 5f;

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
                    img.ScaleToFit(102f, 51f);
                    var imgCell = new PdfPCell() { Border = 0 };
                    imgCell.AddElement(img);
                    table1.AddCell(imgCell);
                    PdfPCell cell = null;
                    var hdrFont = new Font(headerFont.BaseFont, 18, 0, darkGrayColor);

                    BaseColor RedColor = new BaseColor(255, 0, 0);

                    var TitleFont = new Font(textItalicFont.BaseFont, 18, 0, RedColor);
                    cell = new PdfPCell { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, };
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "FireTitle").DESCRIPTION, TitleFont));
                    table1.AddCell(cell);

                    document.Add(table1);
                    document.Add(FireIncidentSection(pageData));
                    document.Add(FireDiscription(pageData));
                    document.Add(FireTimeline(pageData));
                    document.Add(ContainmentSection(pageData));
                    document.Add(CauseSection(pageData));
                    document.Add(ActionSection(pageData));
                    document.Add(FirePhotoSection(pageData));
                    document.Close();
                }
                catch
                {
                }

                return output.ToArray();
            }
        }
        PdfPTable FireIncidentSection(AlertData pageData)
        {
            PdfPTable tableHeader = new PdfPTable(3);
            tableHeader.TotalWidth = baseWidth * mult;
            tableHeader.LockedWidth = true;
            tableHeader.SpacingBefore = 15f;
            PdfPCell cell;
            INCIDENT_ANSWER answer = null;

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "INCIDENTTYPE").DESCRIPTION, detailHdrFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 2;
            cell.AddElement(new Paragraph(pageData.incidentType + "(" + pageData.incidentNumber + ")", labelTxtFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "PLANT").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentLocation, detailTxtFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 2;
            cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "LOCATION").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentDept, detailTxtFont));
            tableHeader.AddCell(cell);

            //FireDate 		

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "FIREINCIDENTDATE").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentDate, detailTxtFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthRight = cell.BorderWidthBottom = cell.BorderWidthTop = .25f;
            cell.AddElement(new Paragraph("Time", detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentTime, detailTxtFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.AddElement(new Paragraph("Shift", detailTxtBoldFont));
            cell = FormatHeaderCell(pageData, (decimal)EHSQuestionId.Shift);
            cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "EQUIPMENTINVOLVED").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.equipmentInvolved, detailTxtFont));
            tableHeader.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            cell.Colspan = 2;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INCIDENT&POSITION").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentAndPositionTitle, detailTxtFont));
            tableHeader.AddCell(cell);

            return tableHeader;
        }
        PdfPTable FireDiscription(AlertData pageData)
        {
            PdfPTable tblDescription = new PdfPTable(new float[] { 162f, 378f });
            tblDescription.TotalWidth = baseWidth * mult;
            tblDescription.LockedWidth = true;
            tblDescription.SpacingAfter = 5f;
            PdfPCell cell;

            INCIDENT_ANSWER answer = null;
            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 3;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "DESCRIPTION").DESCRIPTION, detailHdrFont));
            tblDescription.AddCell(cell);

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 9f, Border = 0 };
            cell.Colspan = 3;
            cell.AddElement(new Paragraph(pageData.incidentDescription, detailTxtFont));

            tblDescription.AddCell(cell);

            return tblDescription;
        }
        PdfPTable FireTimeline(AlertData pageData)
        {
            PdfPTable tblTimeLine = new PdfPTable(3);
            tblTimeLine.TotalWidth = baseWidth * mult;
            tblTimeLine.LockedWidth = true;
            tblTimeLine.SpacingAfter = 5f;
            PdfPCell cell;

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 3;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TIMLINE").DESCRIPTION, detailHdrFont));
            tblTimeLine.AddCell(cell);


            foreach (Timeline timeline in pageData.objTimeLine.ToList())
            {

                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(string.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "DATE").DESCRIPTION_SHORT), detailTxtBoldFont));
                cell.AddElement(new Paragraph(timeline.Date, detailTxtFont));
                tblTimeLine.AddCell(cell);

                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(string.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TIME").DESCRIPTION_SHORT), detailTxtBoldFont));
                cell.AddElement(new Paragraph(timeline.Time, detailTxtFont));
                tblTimeLine.AddCell(cell);

                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(string.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "DESCRIPTION").DESCRIPTION_SHORT), detailTxtBoldFont));
                cell.AddElement(new Paragraph(timeline.Text, detailTxtFont));
                tblTimeLine.AddCell(cell);
            }



            return tblTimeLine;
        }
        PdfPTable FirePhotoSection(AlertData pageData)
        {
            BaseColor darkGrayColor = new BaseColor(0.25f, 0.25f, 0.25f);
            Font textFont = GetTextFont();

            var tblPhoto = new PdfPTable(new float[] { baseWidth });
            tblPhoto.TotalWidth = baseWidth * mult;
            tblPhoto.LockedWidth = true;
            tblPhoto.SpacingBefore = 15f;
            try
            {
                if (pageData.photoData != null && pageData.photoData.Count() > 0)
                {
                    tblPhoto.AddCell(new PdfPCell(new Phrase("Photos", detailHdrFont)) { Padding = 5f, Border = 0, Colspan = 3 });
                    tblPhoto.SpacingBefore = 5f;
                    var captionFont = new Font(textFont.BaseFont, 11, 0, darkGrayColor);

                    //Section for showing the image is attached or not.
                    int i = 0;
                    for (i = 0; i < pageData.photoData.Count; i++)
                    {
                        //if (i == 0)
                        //{
                        //    tblPhoto.AddCell(new PdfPCell(new Phrase("(Choose 1 photo)", detailTxtBoldFont)));
                        //}
                        var photoCell = new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 };

                        iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(pageData.photoData[i]);
                        //photo.ScaleToFit(176f, 132f);
                        //photo.ScaleToFit(264f, 198f);
                        photoCell.AddElement(photo);

                        photoCell.AddElement(new Phrase(pageData.photoCaptions[i], captionFont));

                        tblPhoto.AddCell(photoCell);
                    }
                    // pad remaining cells in row or else table will be corrupt
                    int currentCol = i % 3;
                    for (int j = 0; j < 3 - currentCol; j++)
                        tblPhoto.AddCell(new PdfPCell() { PaddingLeft = 0, PaddingRight = 4, PaddingTop = 8, PaddingBottom = 8, Border = 0 });
                }
                //Condition for validate the video available or not in the report section.
                if (isVideoAvailable == true)
                {
                    tblPhoto.AddCell(new PdfPCell(new Phrase("(Is video available? Yes)", detailTxtBoldFont)));
                }
                else
                {
                    tblPhoto.AddCell(new PdfPCell(new Phrase("(Is video available? No)", detailTxtBoldFont)));
                }
            }
            catch { }
            return tblPhoto;
        }
        #endregion End Fire/SmallFire Report

        private byte[] BuildInjuryIllenessPdf(AlertData pageData)

        {
            string customerLogo = "";
            if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"] != null)
                customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogoLarge"].ToString();
            if (string.IsNullOrEmpty(customerLogo))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"] != null)
                    customerLogo = System.Configuration.ConfigurationManager.AppSettings["CustomerLogo"].ToString();
            }
            if (string.IsNullOrEmpty(customerLogo))
                customerLogo = "MetricsoftLogo.png";

            string logoUrl = baseApplicationUrl + "images/company/" + customerLogo;

            BaseColor darkGrayColor = new BaseColor(0.25f, 0.25f, 0.25f);
            BaseColor lightGrayColor = new BaseColor(0.5f, 0.5f, 0.5f);
            BaseColor whiteColor = new BaseColor(1.0f, 1.0f, 1.0f);
            BaseColor blackColor = new BaseColor(0.0f, 0.0f, 0.0f);
            BaseColor redColor = new BaseColor(1.0f, 0f, 0f);

            Font textFont = GetTextFont();
            Font headerFont = GetHeaderFont();
            Font labelFont = GetLabelFont();
            Font colHeaderFont = GetTextFont();
            Font textItalicFont = GetTextFont();

            detailHdrFont = new Font(headerFont.BaseFont, 13, 0, lightGrayColor);
            detailTxtFont = new Font(textFont.BaseFont, 10, 0, blackColor);
            labelTxtFont = new Font(labelFont.BaseFont, 12, 0, blackColor);
            colHdrFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.UNDERLINE + iTextSharp.text.Font.BOLD, blackColor);
            detailTxtItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.ITALIC, blackColor);
            detailTxtBoldItalicFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD + iTextSharp.text.Font.ITALIC, redColor);
            detailTxtBoldFont = new Font(colHeaderFont.BaseFont, 10, iTextSharp.text.Font.BOLD, blackColor);

            // Create new PDF document
            //Document document = new Document(PageSize.A4, 35f, 35f, 35f, 35f);
            Document document = new Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 10, 10);  // landscape document ?
            mult = 1.3f;

            using (MemoryStream output = new MemoryStream())
            {

                PdfWriter.GetInstance(document, output);

                try
                {
                    document.Open();

                    //
                    // Table 1 - Header
                    //

                    var table1 = new PdfPTable(new float[] { 162f, 378f });
                    table1.TotalWidth = baseWidth * mult;
                    table1.LockedWidth = true;
                    table1.DefaultCell.Border = 0;
                    table1.DefaultCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    table1.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    table1.SpacingAfter = 5f;

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(logoUrl);
                    img.ScaleToFit(102f, 51f);
                    var imgCell = new PdfPCell() { Border = 0 };
                    imgCell.AddElement(img);
                    table1.AddCell(imgCell);
                    PdfPCell cell = null;
                    var hdrFont = new Font(headerFont.BaseFont, 18, 0, darkGrayColor);

                    cell = new PdfPCell { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "TITLE").DESCRIPTION, hdrFont));
                    table1.AddCell(cell);

                    var versionFont = new Font(textItalicFont.BaseFont, 8, 0, darkGrayColor);
                    cell = new PdfPCell() { Padding = 2f, PaddingBottom = 2, Border = 0 };
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "VERSION").DESCRIPTION, versionFont));
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    //adding the table and cells in the PDF report.
                    PdfPTable plantTable = new PdfPTable(3);
                    cell = new PdfPCell() { Padding = 1f, Border = 0 };
                    cell.AddElement(new Paragraph(String.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "PLANT").DESCRIPTION_SHORT + ":  {0}", pageData.incidentLocation), labelTxtFont));
                    plantTable.AddCell(cell);

                    cell = new PdfPCell() { Padding = 2f, Border = 0 };
                    cell.AddElement(new Paragraph("Date", detailTxtBoldFont));
                    cell.AddElement(new Paragraph(pageData.incidentDate, detailTxtFont));
                    plantTable.AddCell(cell);

                    cell = new PdfPCell() { Padding = 2f, Border = 0 };
                    cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "INCIDENT_OCCUR").DESCRIPTION_SHORT, detailHdrFont));
                    cell.AddElement(new Paragraph(string.Format(pageData.incidentType + " - Incident ID: {0}", pageData.incidentNumber), detailTxtFont));
                    plantTable.AddCell(cell);


                    document.Add(table1);
                    document.Add(plantTable);
                    document.Add(ReviewSection(pageData));

                    #region FIELDS FOR NEW DATA 

                    if (pageData.incident.INCIDENT_ID > maxIncidentforInjuryType && pageData.incident.ISSUE_TYPE == "Injury/Illness")
                    {
                        document.Add(IDSection(pageData));
                        document.Add(HeaderSection(pageData));

                    }
                    #endregion

                    document.Add(ContainmentSection(pageData));
                    document.Add(CauseSection(pageData));
                    document.Add(ActionSection(pageData));
                    document.Add(AlertSection(pageData));

                    document.Close();
                }
                catch
                {
                }

                return output.ToArray();
            }
        }
        PdfPTable IDSection(AlertData pageData)
        {
            PdfPTable tableContain = new PdfPTable(new float[] { 540f });
            tableContain.TotalWidth = baseWidth * mult;
            tableContain.LockedWidth = true;
            tableContain.SpacingAfter = 5f;
            PdfPCell cell;
            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "IncidentLable").DESCRIPTION_SHORT, detailHdrFont));
            tableContain.AddCell(cell);
            return tableContain;
        }
        PdfPTable HeaderSection(AlertData pageData)
        {
            PdfPTable tableIncident = new PdfPTable(new float[] { 180f, 180f, 180f });
            tableIncident.TotalWidth = baseWidth * mult;
            tableIncident.LockedWidth = true;
            tableIncident.SpacingAfter = 2f;
            PdfPCell cell;
            INCIDENT_ANSWER answer = null;
            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            //cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
            //cell.AddElement(new Paragraph(String.Format("Date" + ":  {0}", pageData.incidentDate), detailTxtFont));
            //cell.AddElement(new Paragraph("Date", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "ACTION_DT").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentDate, detailTxtFont));
            tableIncident.AddCell(cell);


            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
            // cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
            var TNSK = pageData.incident.TNSKNumber;
            if (string.IsNullOrEmpty(TNSK))
            {
                TNSK = "NA";
            }
            // cell.AddElement(new Paragraph("Age of Associate (US and Europe - DO NOT ENTER)", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "TNSK").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(TNSK, detailTxtFont));
            tableIncident.AddCell(cell);


            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var bt_value = pageData.incident.INCFORM_INJURYILLNESS.BUSINESS_TYPE;

            bt_value = SQMBasePage.GetXLAT(reportXLAT, "BusinessType", bt_value).DESCRIPTION;

            if (string.IsNullOrEmpty(bt_value))
            {
                bt_value = "NA";
            }
            //  cell.AddElement(new Paragraph("Business Type", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "BT").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(bt_value, detailTxtFont));
            tableIncident.AddCell(cell);


            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            //   cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var mpt_value = SQMBasePage.GetXLAT(reportXLAT, pageData.incident.INCFORM_INJURYILLNESS.BUSINESS_TYPE, pageData.incident.INCFORM_INJURYILLNESS.MACRO_PROCESS_TYPE).DESCRIPTION;
            if (string.IsNullOrEmpty(mpt_value))
            {
                mpt_value = "NA";
            }
            // cell.AddElement(new Paragraph("Macro Process Type", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "MPT").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(mpt_value, detailTxtFont));
            tableIncident.AddCell(cell);



            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var spt_value = SQMBasePage.GetXLAT(reportXLAT, pageData.incident.INCFORM_INJURYILLNESS.MACRO_PROCESS_TYPE, pageData.incident.INCFORM_INJURYILLNESS.SPECIFIC_PROCESS_TYPE).DESCRIPTION;
            if (string.IsNullOrEmpty(spt_value))
            {
                spt_value = "NA";
            }
            // cell.AddElement(new Paragraph("Specific Process Type", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "SPT").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(spt_value, detailTxtFont));
            tableIncident.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            //  cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var EMN_value = pageData.incident.INCFORM_INJURYILLNESS.EQUIPMENT_MANUFACTURER_NAME;

            if (string.IsNullOrEmpty(EMN_value))
            {
                EMN_value = "NA";
            }
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "EMN").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(EMN_value, detailTxtFont));
            tableIncident.AddCell(cell);


            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            string ShowDate;
            var EMD_value = pageData.incident.INCFORM_INJURYILLNESS.EQUIPEMENT_MANUFACTURER_DATE;
            if (!EMD_value.HasValue)
            {
                ShowDate = "";
            }
            else
            {
                ShowDate = EMD_value.Value.ToShortDateString();
            }
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "EMD").DESCRIPTION, detailTxtBoldFont));
            cell.AddElement(new Paragraph(ShowDate, detailTxtFont));
            tableIncident.AddCell(cell);

            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var DN_value = pageData.incident.INCFORM_INJURYILLNESS.DESIGN_NUMBER;
            if (string.IsNullOrEmpty(DN_value))
            {
                DN_value = "NA";
            }
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "DN").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(DN_value, detailTxtFont));
            tableIncident.AddCell(cell);


            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthBottom = cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
            var ASN_value = pageData.incident.INCFORM_INJURYILLNESS.ASSET_NUMBER;
            if (string.IsNullOrEmpty(ASN_value))
            {
                ASN_value = "NA";
            }
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "AN").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(ASN_value, detailTxtFont));
            tableIncident.AddCell(cell);



            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 3;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "MedicalStatus").DESCRIPTION, detailTxtBoldFont));

            string CMS = pageData.incident.INCFORM_INJURYILLNESS.CHANGE_MEDICAL_STATUS;
            string[] value = new string[4];
            if (!string.IsNullOrEmpty(CMS))
            {
                var data = CMS.Split(',');
                int index = 0;
                foreach (var item in data)
                {

                    value[index] = SQMBasePage.GetXLAT(reportXLAT, "CMS", item).DESCRIPTION;
                    index++;
                }
            }


            string result = string.Join(", \n", value.Where(x => x != null).ToList());

            cell.AddElement(new Paragraph(result, detailTxtFont));
            tableIncident.AddCell(cell);



            cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            // cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
            var Age_value = pageData.incident.INCFORM_INJURYILLNESS.ASSET_NUMBER;
            if (string.IsNullOrEmpty(Age_value))
            {
                Age_value = "NA";
            }
            // cell.AddElement(new Paragraph("Age of Associate (US and Europe - DO NOT ENTER)", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "AgeAsso").DESCRIPTION_SHORT, detailTxtBoldFont));
            cell.AddElement(new Paragraph(Age_value, detailTxtFont));
            tableIncident.AddCell(cell);

            return tableIncident;
        }
        PdfPCell FormatHeaderCell(AlertData pageData, decimal questionID)
        {
            PdfPCell cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
            if (questionID == 0)
            {
                cell.AddElement(new Paragraph(" ", detailTxtFont));
            }
            else
            {
                INCIDENT_ANSWER answer = pageData.answerList.Where(a => a.INCIDENT_QUESTION_ID == questionID).FirstOrDefault();
                if (answer != null)
                {
                    cell.AddElement(new Paragraph(answer.ORIGINAL_QUESTION_TEXT, detailTxtBoldFont));
                    cell.AddElement(new Paragraph(answer.ANSWER_VALUE, detailTxtFont));
                }
                else
                {
                    cell.AddElement(new Paragraph(" ", detailTxtFont));
                }
            }

            return cell;
        }
        PdfPTable ContainmentSection(AlertData pageData)
        {
            PdfPTable tableContain = new PdfPTable(new float[] { 420f, 110f, 110f });
            tableContain.TotalWidth = baseWidth * mult;
            tableContain.LockedWidth = true;
            tableContain.SpacingAfter = 5f;
            PdfPCell cell;

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 3;
            //cell.BorderWidthTop = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "INTERIM_TITLE").DESCRIPTION, detailHdrFont));
            tableContain.AddCell(cell);

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "CONTAINMENT").DESCRIPTION_SHORT, colHdrFont));
            tableContain.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "RESPONSIBLE").DESCRIPTION_SHORT, colHdrFont));
            tableContain.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "ACTION_DT").DESCRIPTION_SHORT, colHdrFont));
            tableContain.AddCell(cell);

            foreach (var cc in pageData.containList)
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(cc.ITEM_DESCRIPTION, detailTxtFont));
                tableContain.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(cc.ASSIGNED_PERSON, detailTxtFont));
                tableContain.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                if (cc.START_DATE.HasValue)
                {
                    cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)cc.START_DATE, "d", false), detailTxtFont));
                }
                else
                {
                    cell.AddElement(new Paragraph(" ", detailTxtFont));
                }
                tableContain.AddCell(cell);
            }

            return tableContain;
        }
        PdfPTable CauseSection(AlertData pageData)
        {
            PdfPTable tableCause = new PdfPTable(new float[] { 270f, 270f });
            tableCause.TotalWidth = baseWidth * mult;
            tableCause.LockedWidth = true;
            tableCause.SpacingBefore = 15f;

            PdfPCell cell;
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.BorderWidthTop = .25f;
            cell.Colspan = 2;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "ROOTCAUSE_TITLE").DESCRIPTION, detailHdrFont));
            tableCause.AddCell(cell);

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.Colspan = 2;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "TEAMLIST").DESCRIPTION, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.causation == null ? "" : pageData.causation.TEAM_LIST, detailTxtFont));
            tableCause.AddCell(cell);

            if (pageData.root5YList.Where(l => l.ITEM_TYPE == 1).Count() == 0)
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.Colspan = 2;
                cell.AddElement(new Paragraph(pageData.incidentDescription, detailTxtItalicFont));
                tableCause.AddCell(cell);
            }

            cell = new PdfPCell() { Padding = 3f, Border = 0, PaddingTop = 3f };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "PROBLEM").DESCRIPTION, detailTxtBoldFont));
            tableCause.AddCell(cell);
            cell = new PdfPCell() { Padding = 3f, Border = 0, PaddingTop = 3f };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "WHY").DESCRIPTION, detailTxtBoldFont));
            tableCause.AddCell(cell);

            int row = 0;
            foreach (INCFORM_ROOT5Y ps in pageData.root5YList.Where(l => l.ITEM_TYPE == 1).ToList())
            {
                ++row;
                cell = new PdfPCell() { Padding = 3f, Border = 0, PaddingBottom = 6f };
                if (row == 1)
                    cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
                else
                    cell.BorderWidthLeft = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;

                cell.AddElement(new Paragraph(ps.ITEM_DESCRIPTION, detailTxtFont));
                tableCause.AddCell(cell);

                cell = new PdfPCell() { Padding = 3f, Border = 0, PaddingBottom = 6f };
                if (row == 1)
                    cell.BorderWidthTop = cell.BorderWidthRight = cell.BorderWidthBottom = .25f;
                else
                    cell.BorderWidthRight = cell.BorderWidthBottom = .25f;

                iTextSharp.text.List lst = new List();
                lst.SetListSymbol("\u2022");
                foreach (INCFORM_ROOT5Y rc in pageData.root5YList.Where(l => l.PROBLEM_SERIES == ps.PROBLEM_SERIES && l.ITEM_TYPE != 1).ToList())
                {
                    Phrase ph = new Phrase();
                    ph.Add(new Chunk(string.Format("  {0}", rc.ITEM_DESCRIPTION), detailTxtFont));
                    ph.Add(new Chunk(string.Format("    {0}", rc.IS_ROOTCAUSE == true ? "(" + SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "ISROOTCAUSE").DESCRIPTION + ")" : " "), detailTxtBoldItalicFont));
                    iTextSharp.text.ListItem li = new ListItem();
                    li.Add(ph);
                    lst.Add(li);
                }
                cell.AddElement(lst);
                tableCause.AddCell(cell);
            }

            if (pageData.causation != null)
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0, PaddingTop = 3f };
                cell.Colspan = 2;
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "CAUSATION").DESCRIPTION, detailHdrFont));
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "INJURY_CAUSE", pageData.causation.CAUSEATION_CD).DESCRIPTION, detailTxtFont));
                tableCause.AddCell(cell);
            }

            return tableCause;
        }
        PdfPTable ActionSection(AlertData pageData)
        {
            PdfPTable tableAction = new PdfPTable(new float[] { 220f, 150f, 120f, 110f, 200 });
            tableAction.TotalWidth = baseWidth * mult;
            tableAction.LockedWidth = true;
            tableAction.SpacingBefore = 15f;
            PdfPCell cell;
            Phrase ph = new Phrase();
            Paragraph empty = new Paragraph(" ", detailTxtFont);

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.Colspan = 5;
            cell.BorderWidthTop = .25f;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "ACTION_TITLE").DESCRIPTION, detailHdrFont));
            tableAction.AddCell(cell);

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(empty);
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "ACTION").DESCRIPTION, colHdrFont));
            tableAction.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(empty);
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "ACTIONTYPE").DESCRIPTION, colHdrFont));
            tableAction.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(empty);
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "RESPONSIBLE").DESCRIPTION, colHdrFont));
            tableAction.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "DUE_DT").DESCRIPTION + " /", detailTxtBoldFont));
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "COMPLETION_DT").DESCRIPTION, colHdrFont));
            tableAction.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(empty);
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "VERIFICATION").DESCRIPTION, colHdrFont));
            tableAction.AddCell(cell);

            foreach (var ac in pageData.actionList)
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(ac.DESCRIPTION, detailTxtFont));
                tableAction.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "ACTION_CATEGORY", ac.TASK_CATEGORY).DESCRIPTION, detailTxtItalicFont));
                tableAction.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(ac.COMMENTS, detailTxtFont));     // responsible ?
                tableAction.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                if (ac.DUE_DT.HasValue)
                {
                    cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)ac.DUE_DT, "d", false), detailTxtFont));
                }
                else
                {
                    cell.AddElement(empty);
                }
                if (ac.COMPLETE_DT.HasValue)
                {
                    cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)ac.COMPLETE_DT, "d", false), detailTxtFont));
                }
                else
                {
                    cell.AddElement(empty);
                }
                tableAction.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(ac.TASK_VERIFICATION, detailTxtFont));
                tableAction.AddCell(cell);
            }

            return tableAction;
        }
        PdfPTable AlertSection(AlertData pageData)
        {
            PdfPTable tableAlert = new PdfPTable(new float[] { 300f, 120f, 110f, 110f });
            tableAlert.TotalWidth = baseWidth * mult;
            tableAlert.LockedWidth = true;
            tableAlert.SpacingBefore = 15f;
            PdfPCell cell;

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.BorderWidthTop = .25f;
            cell.Colspan = 4;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "PREVENT_TITLE").DESCRIPTION, detailHdrFont));
            tableAlert.AddCell(cell);

            if (pageData.incidentAlert == null)
            {
                return tableAlert;
            }

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.Colspan = 4;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "AFFECTED_PROCESSES").DESCRIPTION, detailTxtBoldFont));
            cell.AddElement(new Paragraph(pageData.incidentAlert.ALERT_DESC, detailTxtFont));
            tableAlert.AddCell(cell);

            if (!string.IsNullOrEmpty(pageData.incidentAlert.COMMENTS))
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.Colspan = 4;
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "PREVENT_VERIFICATION").DESCRIPTION, detailTxtBoldFont));
                cell.AddElement(new Paragraph(pageData.incidentAlert.COMMENTS, detailTxtFont));
                tableAlert.AddCell(cell);
            }

            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "PREVENT_LOCATION").DESCRIPTION, colHdrFont));
            tableAlert.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "RESPONSIBLE").DESCRIPTION_SHORT, colHdrFont));
            tableAlert.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "DUE_DT").DESCRIPTION_SHORT, colHdrFont));
            tableAlert.AddCell(cell);
            cell = new PdfPCell() { Padding = 1f, Border = 0 };
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L4REPORT", "PREVENT_STATUS").DESCRIPTION_SHORT, colHdrFont));
            tableAlert.AddCell(cell);

            foreach (IncidentAlertItem item in pageData.alertList)
            {
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(item.Location.PLANT_NAME, detailTxtFont));
                tableAlert.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                if (item.Person != null)
                {
                    cell.AddElement(new Paragraph(SQMModelMgr.FormatPersonListItem(item.Person), detailTxtFont));
                }
                else
                {
                    cell.AddElement(new Paragraph(" ", detailTxtFont));
                }
                tableAlert.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                if (item.Task.DUE_DT.HasValue)
                {
                    cell.AddElement(new Paragraph(SQMBasePage.FormatDate((DateTime)item.Task.DUE_DT, "d", false), detailTxtFont));
                }
                else
                {
                    cell.AddElement(new Paragraph(" ", detailTxtFont));
                }
                tableAlert.AddCell(cell);
                cell = new PdfPCell() { Padding = 1f, Border = 0 };
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "TASK_STATUS", item.Task.STATUS).DESCRIPTION_SHORT, detailTxtFont));
                tableAlert.AddCell(cell);
            }

            return tableAlert;
        }
        PdfPTable ReviewSection(AlertData pageData)
        {
            PdfPTable tableReview = new PdfPTable(new float[] { 180f, 180f, 180f });
            tableReview.TotalWidth = baseWidth * mult;
            tableReview.LockedWidth = true;
            tableReview.SpacingAfter = 5f;
            PdfPCell cell;
            int approvalCount = pageData.approvalList.Where(l => l.approval.APPROVER_PERSON_ID.HasValue).Count();
            int row = 0;

            cell = new PdfPCell() { Padding = 1f, PaddingBottom = 5f, Border = 0 };
            cell.Colspan = 3;
            cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "HS_L2REPORT", "REVIEW").DESCRIPTION, detailHdrFont));
            tableReview.AddCell(cell);

            foreach (EHSIncidentApproval approval in pageData.approvalList.Where(l => l.approval.APPROVER_PERSON_ID.HasValue).ToList())
            {
                ++row;
                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
                if (row == approvalCount)
                    cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_APPROVALS", approval.approval.ITEM_SEQ.ToString()).DESCRIPTION_SHORT, detailTxtBoldFont));
                tableReview.AddCell(cell);

                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = .25f;
                if (row == approvalCount)
                    cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(approval.approval.APPROVER_PERSON, detailTxtFont));
                tableReview.AddCell(cell);

                cell = new PdfPCell() { Padding = 2f, PaddingBottom = 5f, Border = 0 };
                cell.BorderWidthTop = cell.BorderWidthLeft = cell.BorderWidthRight = .25f;
                if (row == approvalCount)
                    cell.BorderWidthBottom = .25f;
                cell.AddElement(new Paragraph(string.Format(SQMBasePage.GetXLAT(reportXLAT, "HS_5PHASE", "DATED").DESCRIPTION_SHORT + ":  {0}", approval.approval.APPROVAL_DATE.HasValue ? SQMBasePage.FormatDate((DateTime)approval.approval.APPROVAL_DATE, "d", false) : ""), detailTxtFont));
                tableReview.AddCell(cell);
            }

            return tableReview;
        }
        AlertData PopulateByIncidentId(decimal iid)
        {
            AlertData d = new AlertData();
            var entities = new PSsqmEntities();

            d.incident = EHSIncidentMgr.SelectIncidentById(entities, iid);

            if (d.incident != null)
            {
                try
                {
                    string plantName = EHSIncidentMgr.SelectPlantNameById((decimal)d.incident.DETECT_PLANT_ID);
                    d.incidentLocation = plantName;
                    d.incidentNumber = WebSiteCommon.FormatID(iid, 6);

                    string incidentType = EHSIncidentMgr.SelectIncidentTypeByIncidentId(iid);
                    decimal incidentTypeId = EHSIncidentMgr.SelectIncidentTypeIdByIncidentId(iid);
                    decimal companyId = d.incident.DETECT_COMPANY_ID;
                    var questions = EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 0);
                    questions.AddRange(EHSIncidentMgr.SelectIncidentQuestionList(incidentTypeId, companyId, 1));

                    d.answerList = EHSIncidentMgr.GetIncidentAnswerList(d.incident.INCIDENT_ID);
                    INCIDENT_ANSWER answer = null;

                    // Date/Time

                    d.incidentDate = d.incident.INCIDENT_DT.ToShortDateString();
                    if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.TimeOfDay).SingleOrDefault()) != null)
                    {
                        if (!string.IsNullOrEmpty(answer.ANSWER_VALUE))
                            d.incidentTime = Convert.ToDateTime(answer.ANSWER_VALUE).ToShortTimeString();
                    }

                    //if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Shift).SingleOrDefault()) != null)
                    //{
                    //    answer.ANSWER_VALUE = SQMBasePage.GetXLAT(reportXLAT, "SHIFT", answer.ANSWER_VALUE).DESCRIPTION;
                    //}

                    if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Shift).SingleOrDefault()) != null)
                    {
                        if (d.incident.ISSUE_TYPE == "Fire/Small Fire")
                        {
                            answer.ANSWER_VALUE = answer.ANSWER_VALUE;
                        }
                        else
                            answer.ANSWER_VALUE = SQMBasePage.GetXLAT(reportXLAT, "SHIFT", answer.ANSWER_VALUE).DESCRIPTION;
                    }

                    if ((answer = d.answerList.Where(a => a.INCIDENT_QUESTION_ID == (decimal)EHSQuestionId.Department).SingleOrDefault()) != null)
                    {
                        d.incidentDept = answer.ANSWER_VALUE;
                        decimal deptID = 0;
                        if (decimal.TryParse(answer.ANSWER_VALUE, out deptID))
                        {
                            DEPARTMENT dept = SQMModelMgr.LookupDepartment(entities, deptID);
                            if (dept != null)
                                d.incidentDept = dept.DEPT_NAME;
                        }
                    }

                    // Incident Type

                    d.incidentType = incidentType;

                    // Description

                    d.incidentDescription = d.incident.DESCRIPTION;

                    d.detectPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.incident.CREATE_PERSON, "", false);
                    if (d.detectPerson != null)
                    {
                        d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.detectPerson.SUPV_EMP_ID);
                    }

                    if (d.incident.ISSUE_TYPE_ID == (decimal)EHSIncidentTypeId.InjuryIllness)
                    {
                        if (d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID.HasValue)
                        {
                            d.involvedPerson = SQMModelMgr.LookupPerson(entities, (decimal)d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_ID, "", false);
                            if (d.involvedPerson != null)
                                d.supervisorPerson = SQMModelMgr.LookupPersonByEmpID(entities, d.involvedPerson.SUPV_EMP_ID);
                        }
                        else
                        {
                            d.involvedPerson = new PERSON();
                            d.involvedPerson.FIRST_NAME = d.incident.INCFORM_INJURYILLNESS.INVOLVED_PERSON_NAME;
                        }

                        try
                        {
                            d.jobDescription = SQMModelMgr.LookupJobcode(entities, d.incident.INCFORM_INJURYILLNESS.JOBCODE_CD).JOB_DESC;
                        }
                        catch { }

                        d.jobTenure = SQMBasePage.GetXLAT(reportXLAT, "INJURY_TENURE", d.incident.INCFORM_INJURYILLNESS.JOB_TENURE).DESCRIPTION;
                        d.injuryType = SQMBasePage.GetXLAT(reportXLAT, "INJURY_TYPE", d.incident.INCFORM_INJURYILLNESS.INJURY_TYPE).DESCRIPTION;
                        d.bodyPart = SQMBasePage.GetXLAT(reportXLAT, "INJURY_PART", d.incident.INCFORM_INJURYILLNESS.INJURY_BODY_PART).DESCRIPTION_SHORT;
                        d.specificBodyPart = SQMBasePage.GetXLAT(reportXLAT, "INJURY_PART", d.incident.INCFORM_INJURYILLNESS.INJURY_BODY_PART).DESCRIPTION;
                        d.employeeType = d.incident.INCFORM_INJURYILLNESS.COMPANY_SUPERVISED ? "Employee" : "Non-Employee";  // td - make XLAT

                        if ((bool)d.incident.INCFORM_INJURYILLNESS.FIRST_AID)
                        {
                            d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "FIRSTAID").DESCRIPTION;
                        }
                        else if ((bool)d.incident.INCFORM_INJURYILLNESS.RECORDABLE)
                        {
                            d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "RECORDABLE").DESCRIPTION;
                            if ((bool)d.incident.INCFORM_INJURYILLNESS.LOST_TIME)
                            {
                                d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "LOSTTIME").DESCRIPTION;
                            }
                            if ((bool)d.incident.INCFORM_INJURYILLNESS.FATALITY)
                            {
                                d.severity = SQMBasePage.GetXLAT(reportXLAT, "INCIDENT_SEVERITY", "FATALITY").DESCRIPTION;
                            }
                        }
                    }

                    // Containment
                    foreach (INCFORM_CONTAIN cc in EHSIncidentMgr.GetContainmentList(iid, null, false))
                    {
                        if (cc.ASSIGNED_PERSON_ID.HasValue)
                        {
                            cc.ASSIGNED_PERSON = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)cc.ASSIGNED_PERSON_ID, ""));
                        }
                        d.containList.Add(cc);
                    }

                    // Root Cause(s)
                    d.root5YList = EHSIncidentMgr.GetRootCauseList(iid).Where(l => !string.IsNullOrEmpty(l.ITEM_DESCRIPTION)).ToList();
                    if (d.root5YList != null && d.root5YList.Count > 0)
                    {
                        d.incident.INCFORM_CAUSATION.Load();
                        if (d.incident.INCFORM_CAUSATION != null && d.incident.INCFORM_CAUSATION.Count > 0)
                        {
                            d.causation = d.incident.INCFORM_CAUSATION.ElementAt(0);
                        }
                    }

                    // Corrective Actions
                    foreach (TASK_STATUS ac in EHSIncidentMgr.GetCorrectiveActionList(iid, null, false))
                    {
                        if (ac.RESPONSIBLE_ID.HasValue)
                        {
                            ac.COMMENTS = SQMModelMgr.FormatPersonListItem(SQMModelMgr.LookupPerson((decimal)ac.RESPONSIBLE_ID, ""));
                        }
                        d.actionList.Add(ac);
                    }

                    // incident alert

                    d.incidentAlert = EHSIncidentMgr.LookupIncidentAlert(entities, d.incident.INCIDENT_ID);
                    d.alertList = EHSIncidentMgr.GetAlertItemList(entities, d.incident.INCIDENT_ID);

                    string equpmentInvolved = d.answerList.Where(p => p.ORIGINAL_QUESTION_TEXT == "Equipment Involved" && p.INCIDENT_ID == d.incident.INCIDENT_ID).Select(p => p.ANSWER_VALUE).FirstOrDefault();

                    if (string.IsNullOrEmpty(equpmentInvolved))
                    {
                        equpmentInvolved = "NA";
                    }
                    d.equipmentInvolved = equpmentInvolved;

                    string incidentAndPositionTitle = d.answerList.Where(p => p.ORIGINAL_QUESTION_TEXT == "Person who found the incident and position title" && p.INCIDENT_ID == d.incident.INCIDENT_ID).Select(p => p.ANSWER_VALUE).FirstOrDefault();

                    if (string.IsNullOrEmpty(incidentAndPositionTitle))
                    {
                        incidentAndPositionTitle = "NA";
                    }

                    d.incidentAndPositionTitle = incidentAndPositionTitle;


                    var files = (from a in entities.ATTACHMENT
                                 where
                                    (a.RECORD_ID == iid && a.RECORD_TYPE == 40 && a.DISPLAY_TYPE > 0) &&
                                    (a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
                                    a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
                                    a.FILE_NAME.ToLower().Contains(".bmp"))
                                 orderby a.RECORD_TYPE, a.FILE_NAME
                                 select new
                                 {
                                     Data = (from f in entities.ATTACHMENT_FILE where f.ATTACHMENT_ID == a.ATTACHMENT_ID select f.ATTACHMENT_DATA).FirstOrDefault(),
                                     Description = (string.IsNullOrEmpty(a.FILE_DESC)) ? "" : a.FILE_DESC,
                                 }).ToList();


                    d.approvalList = EHSIncidentMgr.GetApprovalList(entities, (decimal)d.incident.ISSUE_TYPE_ID, 5.5m, iid, null, 0);

                    List<INCIDENT_TIMELINE_ANSWER> incidentTimeLine = entities.INCIDENT_TIMELINE_ANSWER.Where(p => p.INCIDENT_ID == d.incident.INCIDENT_ID).ToList();
                    List<INCIDENT_ANSWER> incidentAnswer = entities.INCIDENT_ANSWER.Where(p => p.INCIDENT_ID == d.incident.INCIDENT_ID && p.ORIGINAL_QUESTION_TEXT == "Timeline").ToList();

                    List<Timeline> objTimeLine = new List<Timeline>();

                    for (int i = 0; i < incidentAnswer.Count(); i += 3)
                    {
                        DateTime _date = Convert.ToDateTime(incidentAnswer[i + 0].ANSWER_VALUE);
                        string date = _date.ToString(format: "MM/dd/yyyy");

                        DateTime time = Convert.ToDateTime(incidentAnswer[i + 1].ANSWER_VALUE);
                        string displayTime = time.ToString("hh:mm tt");

                        var obj = new Timeline()
                        {
                            Date = date.ToString()
                        ,
                            Time = displayTime
                         ,
                            Text = incidentAnswer[i + 2].ANSWER_VALUE
                        };

                        objTimeLine.Add(obj);

                    }
                    for (int i = 0; i < incidentTimeLine.Count(); i += 3)
                    {
                        DateTime _date = Convert.ToDateTime(incidentTimeLine[i + 2].ANSWER_VALUE);
                        string date = _date.ToString(format: "MM/dd/yyyy");

                        DateTime time = Convert.ToDateTime(incidentTimeLine[i + 0].ANSWER_VALUE);
                        string displayTime = time.ToString("hh:mm tt");

                        var obj = new Timeline()
                        {
                            Time = displayTime
                        ,
                            Text = incidentTimeLine[i + 1].ANSWER_VALUE
                         ,
                            Date = date
                        };

                        objTimeLine.Add(obj);

                    }


                    d.objTimeLine = objTimeLine.Select(p => new Timeline()
                    {
                        Date = p.Date,
                        Text = p.Text,
                        Time = p.Time
                    }).ToList();



                    if (files.Count > 0)
                    {
                        d.photoData = new List<byte[]>();
                        d.photoCaptions = new List<string>();

                        foreach (var f in files)
                        {
                            d.photoData.Add(f.Data);
                            d.photoCaptions.Add(f.Description);
                        }
                    }
                }
                catch
                {
                }
            }

            return d;
        }
        public iTextSharp.text.Font GetHeaderFont()
        {
            var fontName = "Oswald-Regular";
            if (!FontFactory.IsRegistered(fontName))
            {
                var fontPath = Server.MapPath("~") + "images\\fonts\\Oswald-Regular.ttf";
                FontFactory.Register(fontPath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        public iTextSharp.text.Font GetLabelFont()
        {
            var fontName = "Ubuntu";
            if (!FontFactory.IsRegistered(fontName))
            {
                var fontPath = Server.MapPath("~") + "images\\fonts\\Ubuntu-Regular.ttf";
                FontFactory.Register(fontPath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        public iTextSharp.text.Font GetTextFont()
        {
            var fontName = "Ubuntu";
            if (!FontFactory.IsRegistered(fontName))
            {
                var fontPath = Server.MapPath("~") + "images\\fonts\\Ubuntu-Regular.ttf";
                FontFactory.Register(fontPath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }

    }
}

