﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace SQM.Website.Classes
{
    public static class SQMDocumentMgr
    {
        #region document
        public static DOCUMENT Add(String filename, String description, decimal? display_type, string docScope, decimal recordID, Stream file)
        {
            DOCUMENT ret = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    DOCUMENT d = new DOCUMENT();
                    d.FILE_NAME = filename;
                    d.FILE_DESC = description;

                    //To-do: what do we do when company_id is not set, like when they choose this
                    //       from the Business Org master screen?
                    d.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
                    d.OWNER_ID = SessionManager.UserContext.Person.PERSON_ID;
                    d.RECORD_ID = recordID;
                    d.UPLOADED_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
                    d.UPLOADED_DT = WebSiteCommon.CurrentUTCTime();
                    d.LANGUAGE_ID = (int) SessionManager.UserContext.Person.PREFERRED_LANG_ID;
                    d.DOCUMENT_SCOPE = docScope;
                    if (display_type.HasValue)
                    {
                        d.DISPLAY_TYPE = display_type.Value;
                    }

                    if (d.DOCUMENT_FILE == null)
                    {
                        d.DOCUMENT_FILE = new DOCUMENT_FILE();
                    }

                    //read in the file contents
                    file.Seek(0, SeekOrigin.Begin);
                    byte[] bytearray = new byte[file.Length];
                    int count = 0;
                    while (count < file.Length)
                    {
                        bytearray[count++] = Convert.ToByte(file.ReadByte());
                    }


                    d.DOCUMENT_FILE.DOCUMENT_DATA = bytearray;
                    d.FILE_SIZE = file.Length;

                   // d.DISPLAY_TYPE = Path.GetExtension(filename);

                    entities.AddToDOCUMENT(d);
                    entities.SaveChanges();

                    ret = d;
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                ret = null;
            }

            return ret;
        }

        public static List<DOCUMENT> SelectDocList(decimal company_id, SQM.Shared.DocumentScope docContext)
        {
           List<DOCUMENT> ret = new List<DOCUMENT>();
           try
           {
               using (PSsqmEntities entities = new PSsqmEntities())
               {
                   if (docContext.RecordID > 0)
                       ret = (from d in entities.DOCUMENT
                              where (d.COMPANY_ID == company_id && d.DOCUMENT_SCOPE == docContext.Scope && d.RECORD_ID == docContext.RecordID)
                              select d).ToList();
                   else
                   {
                       if (docContext.DisplayTypes != null  &&  docContext.DisplayTypes.Length > 0)
                           ret = (from d in entities.DOCUMENT
                                  where (d.COMPANY_ID == company_id && d.DOCUMENT_SCOPE == docContext.Scope  &&  docContext.DisplayTypes.Contains(d.DISPLAY_TYPE))
                                  select d).ToList();
                       else 
                           ret = (from d in entities.DOCUMENT
                                  where (d.COMPANY_ID == company_id && d.DOCUMENT_SCOPE == docContext.Scope)
                                  select d).ToList();
                   }
               }
           }
           catch (Exception e)
           {
               //SQMLogger.LogException(e);
               //ret = null;
           }

           return ret;
        }

        public static DOCUMENT GetDocument(decimal doc_id)
        {
            DOCUMENT ret = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    decimal company_id;
                    if (SessionManager.EffLocation != null)
                        company_id = SessionManager.EffLocation.Company.COMPANY_ID;
                    else
                        company_id = SQMModelMgr.LookupPrimaryCompany(entities).COMPANY_ID;

                    ret = (from d in entities.DOCUMENT.Include("DOCUMENT_FILE")
                           where ( 
                                    (d.COMPANY_ID == company_id) //filter by company id as well, for security
                                    && 
                                    (d.DOCUMENT_ID == doc_id)
                                    )
                           select d).Single();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                ret = null;
            }

            return ret;
        }

        public static DOCUMENT FindCurrentDocument(string docScope, int displayType)
        {
            DOCUMENT ret = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    decimal company_id;
                    if (SessionManager.EffLocation != null)
                        company_id = SessionManager.EffLocation.Company.COMPANY_ID;
                    else
                        company_id = SQMModelMgr.LookupPrimaryCompany(entities).COMPANY_ID;

                    ret = (from d in entities.DOCUMENT.Include("DOCUMENT_FILE")
                           select d).OrderByDescending(d => d.UPLOADED_DT).Where(l => l.COMPANY_ID == company_id && l.DISPLAY_TYPE == displayType).First();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                ret = null;
            }

            return ret;
        }

        public static List<DOCUMENT> SelectDocListByOwner(decimal ownerID, decimal? displayType)
        {
            List<DOCUMENT> ret = new List<DOCUMENT>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    ret = (from d in entities.DOCUMENT
                           where (d.OWNER_ID == ownerID  &&  d.DISPLAY_TYPE == displayType)
                           select d).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return ret;
        }

        public static List<DOCUMENT> GetSharedDocs(decimal personID)
        {
            // get documents that this user's delegators have shared
            List<DOCUMENT> ret = new List<DOCUMENT>();

            List<PERSON> delegateList = SQMModelMgr.SelectDelegateList(new PSsqmEntities(), SessionManager.UserContext.Person.PERSON_ID);
            foreach (PERSON person in delegateList)
            {
                ret.AddRange(SelectDocListByOwner(person.PERSON_ID, 15));
            }

            return ret;
        }

        public static void Delete(decimal Document_ID)
        {
            
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {

                    DOCUMENT_FILE this_docfile = (from d in entities.DOCUMENT_FILE 
                                         where (d.DOCUMENT_ID == Document_ID)
                                         select d).Single();
                    if (this_docfile != null)
                    {
                        entities.DeleteObject(this_docfile);
                        entities.SaveChanges();
                    }

                    DOCUMENT this_doc = (from d in entities.DOCUMENT
                                         where (d.DOCUMENT_ID == Document_ID)
                           select d).Single();
                    if (this_doc != null)
                    {
                        entities.DeleteObject(this_doc);
                        entities.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return;
        }

        public static string GetImageSourceString(decimal docID)
        {
            DOCUMENT doc = GetDocument(docID);
            if (doc != null && doc.DOCUMENT_FILE != null)
            {
                return GetImageSourceString(doc);
            }
            else
                return "";
        }
        public static string GetImageSourceString(DOCUMENT doc)
        {
            string src = "";
            if (doc != null)
            {
                src = "data:image/"; //jpg;base64,";
                switch (Path.GetExtension(doc.FILE_NAME))
                {
                    case ".bmp":
                        src += "bmp";
                        break;
                    case ".jpg":
                        src += "jpg";
                        break;
                    case ".jpeg":
                        src += "jpeg";
                        break;
                    case ".gif":
                        src += "gif";
                        break;
                    default:
                        src += "png";
                        break;
                }
                src += ";base64," + Convert.ToBase64String(doc.DOCUMENT_FILE.DOCUMENT_DATA);
            }

            return src;
        }

        #endregion

        #region attachment
        public static ATTACHMENT AddAttachment(String filename, String description, decimal? display_type, string docScope, int recordType, decimal recordID, string recordStep, string sessionID, Stream file)
        {
            ATTACHMENT ret = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    ATTACHMENT d = new ATTACHMENT();
                    d.FILE_NAME = filename;
                    d.FILE_DESC = description;

                    //To-do: what do we do when company_id is not set, like when they choose this
                    //       from the Business Org master screen?
                    d.COMPANY_ID = SessionManager.EffLocation.Company.COMPANY_ID;
                    d.OWNER_ID = SessionManager.UserContext.Person.PERSON_ID;
                    d.UPLOADED_BY = SessionManager.UserContext.Person.FIRST_NAME + " " + SessionManager.UserContext.Person.LAST_NAME;
                    d.UPLOADED_DT = WebSiteCommon.CurrentUTCTime();
                    d.LANGUAGE_ID = (int)SessionManager.UserContext.Person.PREFERRED_LANG_ID;
                    d.ATTACHMENT_SCOPE = docScope;


                    d.RECORD_TYPE = recordType;
                    d.RECORD_ID = recordID; // we might not have the record id when the attaachment is created 
                    d.RECORD_STEP = recordStep;
                    d.SESSION_ID = sessionID;

                    if (display_type.HasValue)
                    {
                        d.DISPLAY_TYPE = display_type.Value;
                    }

                    if (d.ATTACHMENT_FILE == null)
                    {
                        d.ATTACHMENT_FILE = new ATTACHMENT_FILE();
                    }

                    //read in the file contents
                    file.Seek(0, SeekOrigin.Begin);
                    byte[] bytearray = new byte[file.Length];
                    int count = 0;
                    while (count < file.Length)
                    {
                        bytearray[count++] = Convert.ToByte(file.ReadByte());
                    }


                    d.ATTACHMENT_FILE.ATTACHMENT_DATA = bytearray;
                    d.FILE_SIZE = file.Length;

                    // d.DISPLAY_TYPE = Path.GetExtension(filename);

                    entities.AddToATTACHMENT(d);
                    entities.SaveChanges();

                    ret = d;
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                ret = null;
            }

            return ret;
        }

        public static ATTACHMENT GetAttachment(decimal attachmentID)
        {
            ATTACHMENT ret = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    ret = (from d in entities.ATTACHMENT.Include("ATTACHMENT_FILE")
                           where (
                                    (d.ATTACHMENT_ID == attachmentID)
                                    )
                           select d).Single();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                ret = null;
            }

            return ret;
        }

        public static List<ATTACHMENT> SelectAttachmentListByRecord(int recordType, decimal recordID, string recordStep, string sessionID)
        {
            // get all attachments related to a specific record (ie quality issue, ehs incident, etc...)
            List<ATTACHMENT> ret = new List<ATTACHMENT>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    if (recordID == 0)
                    {
                        ret = (from d in entities.ATTACHMENT
                               where (d.RECORD_TYPE == recordType && d.SESSION_ID == sessionID)
                               select d).ToList();
                    }
                    else
                    {
                        ret = (from d in entities.ATTACHMENT
                               where (d.RECORD_TYPE == recordType && d.RECORD_ID == recordID)
                               select d).ToList();
                    }

                    if (!string.IsNullOrEmpty(recordStep))
                        ret = ret.Where(l => l.RECORD_STEP == recordStep).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return ret;
        }

        public static ATTACHMENT DeleteAttachment(decimal attachmentID)
        {
            ATTACHMENT this_at = null;

            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {

                    ATTACHMENT_FILE this_atfile = (from d in entities.ATTACHMENT_FILE
                                                  where (d.ATTACHMENT_ID == attachmentID)
                                                  select d).Single();
                    if (this_atfile != null)
                    {
                        entities.DeleteObject(this_atfile);
                        entities.SaveChanges();
                    }

                    this_at = (from d in entities.ATTACHMENT
                                         where (d.ATTACHMENT_ID == attachmentID)
                                         select d).Single();
                    if (this_at != null)
                    {
                        entities.DeleteObject(this_at);
                        entities.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return this_at;
        }

        public static int UpdateAttachmentRecordID(PSsqmEntities entities, int recordType, string sessionID, decimal recordID)
        {
            int status = 0;
            try
            {
                status = entities.ExecuteStoreCommand("UPDATE ATTACHMENT SET RECORD_ID = " + recordID.ToString() + " WHERE RECORD_TYPE = " + recordType.ToString() + " AND SESSION_ID = '" + sessionID + "'");
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }
            return status;
        }

		public static int UpdateAttachmentDisplayType(decimal attachmentID, decimal displayType)
		{
			int status = 0;
			try
			{
				using (PSsqmEntities entities = new PSsqmEntities())
				{
					status = entities.ExecuteStoreCommand("UPDATE ATTACHMENT SET DISPLAY_TYPE = " + displayType + " WHERE ATTACHMENT_ID = " + attachmentID);
				}
			}
			catch (Exception e)
			{
				//SQMLogger.LogException(e);
			}
			return status;
		}

        public static byte[] GetAttachmentByteArray(decimal attachmentID)
        {
            ATTACHMENT doc = GetAttachment(attachmentID);
            byte[] bytes = doc.ATTACHMENT_FILE.ATTACHMENT_DATA;
            return bytes;
        }

        public static string GetAttachImageSourceString(decimal attachmentID)
        {
            ATTACHMENT doc = GetAttachment(attachmentID);
            if (doc != null && doc.ATTACHMENT_FILE != null)
            {
                return GetAttachImageSourceString(doc);
            }
            else
                return "";
        }
        public static string GetAttachImageSourceString(ATTACHMENT doc)
        {
            string src = "data:image/"; //jpg;base64,";
            switch (Path.GetExtension(doc.FILE_NAME))
            {
                case ".bmp":
                    src += "bmp";
                    break;
                case ".jpg":
                    src += "jpg";
                    break;
                case ".jpeg":
                    src += "jpeg";
                    break;
                case ".gif":
                    src += "gif";
                    break;
                default:
                    src += "png";
                    break;
            }
            src += ";base64," + Convert.ToBase64String(doc.ATTACHMENT_FILE.ATTACHMENT_DATA);
            return src;
        }
        #endregion

        #region static
        public static int DISP_TYPE_INITIAL_RESP  = 1;
        public static int DISP_TYPE_FINAL_RESP_S1 = 2;
        public static int DISP_TYPE_FINAL_RESP_S2 = 3;
        public static int DISP_TYPE_FINAL_RESP_S3 = 4;
        public static int DISP_TYPE_FINAL_RESP_S4 = 5;

        public static String DISP_TYPE_INITIAL_RESPONSE  = "Initial Response";
        public static String DISP_TYPE_FINAL_RESPONSE_S1 = "Final Response Step 1";
        public static String DISP_TYPE_FINAL_RESPONSE_S2 = "Final Response Step 2";
        public static String DISP_TYPE_FINAL_RESPONSE_S3 = "Final Response Step 3";
        public static String DISP_TYPE_FINAL_RESPONSE_S4 = "Final Response Step 4";

	    public static String DISP_TYPE_INITIAL_RESP_LABEL = "IT_INIT_RESP_DOC";
	    public static String DISP_TYPE_FINAL_RESP_S1_LABEL = "IT_FINAL_RESP_S1_DOC";
	    public static String DISP_TYPE_FINAL_RESP_S2_LABEL = "IT_FINAL_RESP_S2_DOC";
	    public static String DISP_TYPE_FINAL_RESP_S3_LABEL = "IT_FINAL_RESP_S3_DOC";
	    public static String DISP_TYPE_FINAL_RESP_S4_LABEL = "IT_FINAL_RESP_S4_DOC";
	    public static String DISP_TYPE_UNKNOWN_LABEL = "IT_UNKNOWN_DOC";
        #endregion
    }
}