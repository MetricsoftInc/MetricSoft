using System.Drawing;
using System.IO;
using System.Web;
using SelectPdf;

namespace SQM.Website.Shared
{
	/// <summary>
	/// Generates a PDF for downloading.
	/// </summary>
	public class PdfDownloader : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			if (context.Request["html"] == null)
				throw new InvalidDataException("Requires 'html' value in POST data.");
			using (var ms_out = new MemoryStream())
			{
				var converter = new HtmlToPdf();
				converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
				converter.Options.PdfPageSize = PdfPageSize.Letter;
				converter.Options.MarginBottom = 32;
				converter.Options.MarginLeft = 32;
				converter.Options.MarginRight = 32;
				converter.Options.MarginTop = 32;
				converter.ConvertHtmlString(context.Request["html"]).Save(ms_out);
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
