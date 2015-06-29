using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.xml;
using System.Xml;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.xml.simpleparser;
using System.Web;
using System.Net;
using System.Collections.Generic;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.css;
using System.Text;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.parser; 

namespace SQM.Website
{
	public static class PDFUtils
	{

		public static void ServePDF(string url)
		{
			string page = "";
			string lastPart = "";

			string[] parts = url.Split('=');
			if (parts.Count() > 2)
				lastPart = "_" + parts[parts.Count() - 2];

			HttpContext.Current.Response.ContentType = "application/pdf"; 
			HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Alert" + lastPart + ".pdf");
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			// Download the page
			WebClient web = new WebClient();
			page = web.DownloadString(url);
	
			// Now we what ever is rendered on the page we will give it to the object of the String reader so that we can 
			StringReader srdr = new StringReader(page);

			// Creating the PDF DOCUMENT using the Document class from Itextsharp.pdf namespace
			Document pdfDoc = new Document(PageSize.LETTER, 40f, 40f, 40f, 40f);

			
			//PdfWriter pdfWrite = default(PdfWriter);
			//pdfWrite = PdfWriter.GetInstance(docWorkingDocument, new FileStream(strFileName, FileMode.Create));
			//srdDocToString = new StringReader(strHtml);
			//docWorkingDocument.Open();
			//addLogo.ScaleToFit(128, 37);
			//addLogo.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
			//docWorkingDocument.Add(addLogo);
			//XMLWorkerHelper.GetInstance().ParseXHtml(pdfWrite, docWorkingDocument, srdDocToString);
			
			PdfWriter pdfWrite = default(PdfWriter);
			//pdfWrite = PdfWriter.GetInstance(pdfDoc, new FileStream("NewAlert.pdf", FileMode.Open));
			pdfWrite = PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);
			pdfDoc.Open();
			
			XMLWorkerHelper.GetInstance().ParseXHtml(pdfWrite, pdfDoc, srdr);


			// HTML Worker allows us to parse the HTML Content to the PDF Document.To do this we will pass the object of Document class as a Parameter.
			//HTMLWorker hparse = new HTMLWorker(pdfDoc);
			// Finally we write data to PDF and open the Document
			//PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);
			//pdfDoc.Open();

			// Now we will pass the entire content that is stored in String reader to HTML Worker object to achieve the data from to String to HTML and then to PDF.
			//hparse.Parse(srdr);
			
			pdfDoc.Close();
			// Now finally we write to the PDF Document using the Response.Write method.
			HttpContext.Current.Response.Write(pdfDoc);
			HttpContext.Current.Response.End();
		}

		public static void ServePdfEmbeddedImages(string url)
		{
			using (var doc = new Document(PageSize.LETTER))
			{
				var writer = PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream);
				writer.InitialLeading = 12.5f;
				doc.Open();

				string lastPart = "";
				string[] parts = url.Split('=');
				if (parts.Count() > 2)
					lastPart = "_" + parts[parts.Count() - 2];

				HttpContext.Current.Response.ContentType = "application/pdf";
				HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Alert" + lastPart + ".pdf");
				HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

				WebClient web = new WebClient();
				var html = web.DownloadString(url);

				var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
				tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
				tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

				CssFilesImpl cssFiles = new CssFilesImpl();
				cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
				var cssResolver = new StyleAttrCSSResolver(cssFiles);
				cssResolver.AddCss(@"code { padding: 2px 4px; } img { padding: 4px 8px; }", "utf-8", true);
				var charset = Encoding.UTF8;
				var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
				hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors
				var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
				var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
				var worker = new XMLWorker(pipeline, true);
				var xmlParser = new XMLParser(true, worker, charset);
				xmlParser.Parse(new StringReader(html));

				HttpContext.Current.Response.Write(doc);
				HttpContext.Current.Response.End();
			}
			//Process.Start("test.pdf");
		}


		public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
		{
			public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
			{
				IDictionary<string, string> attributes = tag.Attributes;
				string src;
				if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
					return new List<IElement>(1);

				if (string.IsNullOrEmpty(src))
					return new List<IElement>(1);

				if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
				{
					// data:[<MIME-type>][;charset=<encoding>][;base64],<data>
					var base64Data = src.Substring(src.IndexOf(",") + 1);
					var imagedata = Convert.FromBase64String(base64Data);
					var image = iTextSharp.text.Image.GetInstance(imagedata);

					var list = new List<IElement>();
					var htmlPipelineContext = GetHtmlPipelineContext(ctx);
					list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
					return list;
				}
				else
				{
					src = @"http://library.corporate-ir.net/library/17/176/176060/mediaitems/93/a.com_logo_RGB.jpg";
					//return base.End(ctx, tag, currentContent);
					var image = iTextSharp.text.Image.GetInstance(src);

					var list = new List<IElement>();
					var htmlPipelineContext = GetHtmlPipelineContext(ctx);
					list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
					return list;
				}
			}
		}

	}
}