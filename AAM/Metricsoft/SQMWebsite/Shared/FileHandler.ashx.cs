using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.SessionState;
using System.IO;
using SQM.Website.Classes;

namespace SQM.Website.Shared
{
    /// <summary>
    /// Summary description for WFPImageHandler
    /// </summary>
    public class FileHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.Clear();

                if (!String.IsNullOrEmpty(context.Request.QueryString["DOC_ID"]))
                {
                    String document_id = context.Request.QueryString["DOC_ID"];
					String fileName = "attachment";
										
                    Decimal doc_id = decimal.Parse(document_id);
                    string fileType = "";
                    String mimeType = "";

                    if (!string.IsNullOrEmpty(context.Request.QueryString["DOC"]))
                    {
                        switch (context.Request.QueryString["DOC"])
                        {
                            case "a": // attachment
                                ATTACHMENT a = SQMDocumentMgr.GetAttachment(doc_id);
                                fileType = Path.GetExtension(a.FILE_NAME);

								if (!string.IsNullOrEmpty(context.Request.QueryString["FILE_NAME"]))
									fileName = context.Request.QueryString["FILE_NAME"];
								else
									fileName += fileType;

								mimeType = SQM.Website.Classes.FileExtensionConverter.ToMIMEType(fileType);
								context.Response.ContentType = mimeType;
								//context.Response.BinaryWrite(a.ATTACHMENT_FILE.ATTACHMENT_DATA);  
								
								// mt  - use below for video streams ?
								context.Response.AddHeader("content-disposition", "inline; filename=" + fileName);
								context.Response.AddHeader("content-length", a.ATTACHMENT_FILE.ATTACHMENT_DATA.Length.ToString());
								context.Response.OutputStream.Write(a.ATTACHMENT_FILE.ATTACHMENT_DATA, 0, a.ATTACHMENT_FILE.ATTACHMENT_DATA.Length);
								context.Response.Flush();
                                break;
							case "v": // video
									  // the following is the code for finding the file in the database
								VIDEO v = MediaVideoMgr.SelectVideoById(doc_id);
								VIDEO_FILE vf = MediaVideoMgr.SelectVideoFileById(doc_id);

								fileType = Path.GetExtension(v.FILE_NAME);

								if (!string.IsNullOrEmpty(context.Request.QueryString["FILE_NAME"]))
									fileName = context.Request.QueryString["FILE_NAME"];
								else
									fileName += fileType;

								mimeType = SQM.Website.Classes.FileExtensionConverter.ToMIMEType(fileType);
								context.Response.ContentType = mimeType;

								// mt  - use below for video streams ?
								context.Response.AddHeader("content-disposition", "inline; filename=" + fileName);
								context.Response.AddHeader("content-length", vf.VIDEO_DATA.Length.ToString());
								//context.Response.OutputStream.Write(vf.VIDEO_DATA, 0, vf.VIDEO_DATA.Length);
								context.Response.BinaryWrite(vf.VIDEO_DATA);

								context.Response.Flush();

								// the following is the code for finding the file on the server
								//VIDEO v = MediaVideoMgr.SelectVideoById(doc_id);
								//fileType = Path.GetExtension(v.FILE_NAME);

								//fileName = context.Server.MapPath(v.FILE_NAME);
								//System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);

								//try
								//{
								//	if (fileInfo.Exists)
								//	{
								//		context.Response.Clear();
								//		context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileInfo.Name + "\"");
								//		context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
								//		context.Response.ContentType = "application/octet-stream";
								//		context.Response.TransmitFile(fileInfo.FullName);
								//		context.Response.Flush();
								//	}
								//	else
								//	{
								//		throw new Exception("File not found");
								//	}
								//}
								//catch (Exception ex)
								//{
								//	context.Response.ContentType = "text/plain";
								//	context.Response.Write(ex.Message);
								//}
								//finally
								//{
								//	context.Response.End();
								//}
								break;
							case "va": // video attachment
								VIDEO_ATTACHMENT va = SQMDocumentMgr.GetVideoAttachment(doc_id);
								fileType = Path.GetExtension(va.FILE_NAME);

								if (!string.IsNullOrEmpty(context.Request.QueryString["FILE_NAME"]))
									fileName = context.Request.QueryString["FILE_NAME"];
								else
									fileName += fileType;

								mimeType = SQM.Website.Classes.FileExtensionConverter.ToMIMEType(fileType);
								context.Response.ContentType = mimeType;
								//context.Response.BinaryWrite(a.ATTACHMENT_FILE.ATTACHMENT_DATA);  

								// mt  - use below for video streams ?
								context.Response.AddHeader("content-disposition", "inline; filename=" + fileName);
								context.Response.AddHeader("content-length", va.VIDEO_ATTACHMENT_FILE.VIDEO_ATTACH_DATA.Length.ToString());
								context.Response.OutputStream.Write(va.VIDEO_ATTACHMENT_FILE.VIDEO_ATTACH_DATA, 0, va.VIDEO_ATTACHMENT_FILE.VIDEO_ATTACH_DATA.Length);
								context.Response.Flush();
								break;
							default: // document
                                DOCUMENT d = SQMDocumentMgr.GetDocument(doc_id);
                                fileType = Path.GetExtension(d.FILE_NAME);
                                // set this to whatever your format is of the image
                                context.Response.ContentType = fileType;
                                mimeType = SQM.Website.Classes.FileExtensionConverter.ToMIMEType(fileType);
								context.Response.ContentType = mimeType;
                                //context.Response.AddHeader("content-length", d.DOCUMENT_DATA.Length.ToString());
                                //context.Response.OutputStream.Write(d.DOCUMENT_DATA, 0, d.DOCUMENT_DATA.Length);
                                context.Response.AddHeader("content-length", d.DOCUMENT_FILE.DOCUMENT_DATA.Length.ToString());
                                context.Response.OutputStream.Write(d.DOCUMENT_FILE.DOCUMENT_DATA, 0, d.DOCUMENT_FILE.DOCUMENT_DATA.Length);
                                context.Response.Flush();
                                break;
                        }
                    }
                }

                else
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<p>Document not found</p>");
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

       
    }


}