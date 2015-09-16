using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website.Classes
{
   
    public static class FileExtensionConverter
    {
        private static IDictionary<string, string> extensionMIMETypeMapping;
        private static IDictionary<string, string> imageTypeMapping;
        private static string defaultMIMEType = "application/octet-stream";
        private static string defaultImage = "icon_html.gif"; //to-do: get better default icon file

        static FileExtensionConverter()
         {
           extensionMIMETypeMapping = new Dictionary<string, string>()
           {
               { "txt", "text/plain" },
               { "rtf", "text/richtext" },
               { "wav", "audio/wav" },
               { "gif", "image/gif" },
               { "jpeg", "image/jpeg" },
               { "jpg", "image/jpeg" },
               { "png", "image/png" },
               { "tiff", "image/tiff" },
               { "bmp", "image/bmp" },
               { "avi", "video/avi" },
               { "mpeg", "video/mpeg" },
               { "pdf", "application/pdf" },
               { "doc", "application/msword" },
               { "dot", "application/msword" },
               { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
               { "dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
               { "xls", "application/vnd.ms-excel" },
                { "xlt", "application/vnd.ms-excel" },
                { "csv", "application/vnd.ms-excel" },
                { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
               { "xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
                { "ppt", "application/vnd.ms-powerpoint" },
                { "pot", "application/vnd.ms-powerpoint" },
                { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { "potx", "application/vnd.openxmlformats-officedocument.presentationml.template" }
             };

           imageTypeMapping = new Dictionary<string, string>()
           {
               { "txt", "icon_text.gif" },
               { "rtf", "icon_word.gif" },
               { "wav", "icon_quick.gif" },
               { "gif", "icon_gif.gif" },
               { "jpeg", "icon_jpg.gif" },
               { "jpg", "icon_jpg.gif" },
               { "png", "image/png" },
               { "tiff", "image/tiff" },
               { "bmp", "icon_bmp.gif" },
               { "avi", "video/avi" },
               { "mpeg", "video/mpeg" },
               { "pdf", "icon_pdf.gif" },
               { "doc", "icon_word.gif" },
               { "dot", "icon_word.gif" },
               { "docx", "icon_word.gif" },
               { "dotx", "icon_word.gif" },
               { "xls", "icon_xls.gif" },
                { "xlt", "icon_xls.gif" },
                { "csv", "icon_xls.gif" },
                { "xlsx", "icon_xls.gif" },
               { "xltx", "icon_xls.gif" },
                { "ppt", "icon_ppt.gif" },
                { "pot", "icon_ppt.gif" },
                { "pptx", "icon_ppt.gif" },
                { "potx", "icon_ppt.gif" }
             };
         }
 

         public static string ToMIMEType(string extension)
         {
             if (extension == null || extension.Length == 0)
             {
                 return defaultMIMEType;
             }

             string lowerExtension = extension.ToLower();
             //strip any periods, if it contains any
             lowerExtension = lowerExtension.Replace('.', ' ').Trim();
             string mime;
             if (!extensionMIMETypeMapping.TryGetValue(lowerExtension, out mime))
             {
                 mime = defaultMIMEType;
             }

             return mime;
         }

         public static string ToImage(string extension)
         {
             if (extension == null || extension.Length == 0)
             {
                 return defaultImage;
             }

             string lowerExtension = extension.ToLower();
             //strip any periods, if it contains any
             lowerExtension = lowerExtension.Replace('.', ' ').Trim();
             string image;
             if (!imageTypeMapping.TryGetValue(lowerExtension, out image))
             {
                 image = defaultImage;
             }
                      
             return image;
         }
     }

}