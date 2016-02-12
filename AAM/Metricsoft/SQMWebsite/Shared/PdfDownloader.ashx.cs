using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using SelectPdf;

namespace SQM.Website.Shared
{
	/// <summary>
	/// Generates a PDF for downloading.
	/// </summary>
	public class PdfDownloader : IHttpHandler, IRequiresSessionState
	{
		public void ProcessRequest(HttpContext context)
		{
			if (context.Request["start_batch"] != null)
			{
				context.Session["in_batch"] = true;
				context.Session["batch"] = new List<PdfDocument>();
				return;
			}

			if (context.Request["end_batch"] != null)
			{
				// Handle batching.
				var batch = context.Session["batch"] as List<PdfDocument>;
				var finalDocument = new PdfDocument();
				foreach (var doc in batch)
					finalDocument.Append(doc);
				using (var ms_out = new MemoryStream())
				{
					finalDocument.Save(ms_out);
					string outputFilename = "Test.pdf";
					if (context.Request["filename"] != null)
						outputFilename = context.Request["filename"];
					context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", outputFilename));
					context.Response.AddHeader("Content-Length", ms_out.Length.ToString());
					context.Response.ContentType = "application/pdf";
					context.Response.BinaryWrite(ms_out.GetBuffer());
				}
				context.Session.Remove("in_batch");
				context.Session.Remove("batch");
				return;
			}

			bool inBatch = (bool?)context.Session["in_batch"] ?? false;

			if (context.Request["html"] == null)
				throw new InvalidDataException("Requires 'html' value in POST data.");
			var converter = new HtmlToPdf();
			converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
			converter.Options.PdfPageSize = context.Request["pageSize"] != null && context.Request["pageSize"] == "11x17" ? PdfPageSize.Letter11x17 : PdfPageSize.Letter;
			converter.Options.MarginBottom = 32;
			converter.Options.MarginLeft = 32;
			converter.Options.MarginRight = 32;
			converter.Options.MarginTop = 32;
			var document = converter.ConvertHtmlString(context.Request["html"]);
			if (context.Request["bookmarkName"] != null)
				document.AddBookmark(context.Request["bookmarkName"], new PdfDestination(document.Pages[0], new PointF(0, 0)));
			if (inBatch)
				(context.Session["batch"] as List<PdfDocument>).Add(document);
			else
				using (var ms_out = new MemoryStream())
				{
					document.Save(ms_out);
					string outputFilename = "Test.pdf";
					if (context.Request["filename"] != null)
						outputFilename = context.Request["filename"];
					context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", outputFilename));
					context.Response.AddHeader("Content-Length", ms_out.Length.ToString());
					context.Response.ContentType = "application/pdf";
					context.Response.BinaryWrite(ms_out.GetBuffer());
				}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
