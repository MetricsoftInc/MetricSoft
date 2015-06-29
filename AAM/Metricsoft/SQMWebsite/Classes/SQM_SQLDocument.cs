using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQM.Website.Interfaces;

namespace SQM.Website.Classes
{
    public class SQM_SQLDocument : IFile
    {
       
        private SQM.Website.DOCUMENT mySqlDoc;
        public SQM.Website.DOCUMENT SqlDocument
        {
            get { return mySqlDoc; }
            set {   mySqlDoc = value;
                    if (value.DOCUMENT_FILE != null)
                    {
                        SqlDocument.DOCUMENT_FILE = value.DOCUMENT_FILE;
                    }
                    Name = SqlDocument.FILE_NAME;
                    Description = SqlDocument.FILE_DESC;
                    DisplayType = SqlDocument.DISPLAY_TYPE;
                    ID = SqlDocument.DOCUMENT_ID.ToString();
                    Size = SqlDocument.FILE_SIZE;
                }
        }

        public String Name { get; set; }
        public String Description { get; set; }
        public decimal? DisplayType { get; set; }
        public String ID { get; set; }
        public decimal? Size { get; set; }
        public DateTime UploadedDateTime { get; set; }

        public SQM_SQLDocument()
        {
            
        }

        public SQM_SQLDocument(SQM.Website.DOCUMENT newDoc)
        {
            SqlDocument = newDoc;
            SqlDocument.DOCUMENT_FILE = newDoc.DOCUMENT_FILE;
            Name = SqlDocument.FILE_NAME;
            Description = SqlDocument.FILE_DESC;
            DisplayType = SqlDocument.DISPLAY_TYPE;
            ID = SqlDocument.DOCUMENT_ID.ToString();
            UploadedDateTime = SqlDocument.UPLOADED_DT.Value;

            Size = SqlDocument.FILE_SIZE;
        }

        public byte[] GetData()
        {
            return SqlDocument.DOCUMENT_FILE.DOCUMENT_DATA;
        }

     
    }
}