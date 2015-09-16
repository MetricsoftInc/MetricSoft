using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace SQM.Website
{
    public class StreamData
    {
        public decimal STREAM_ID
        {
            get;
            set;
        }
        public decimal COMPANY_ID
        {
            get;
            set;
        }
        public decimal? PLANT_ID
        {
            get;
            set;
        }
        public string PLANT_NAME
        {
            get;
            set;
        }
        public decimal? PLANT_LINE_ID
        {
            get;
            set;
        }
        public string PLANT_LINE_NAME
        {
            get;
            set;
        }
        public COMPANY Supplier
        {
            get;
            set;
        }
        public PLANT SupplierPlant
        {
            get;
            set;
        }
        public COMPANY Customer
        {
            get;
            set;
        }
        public PLANT CustomerPlant
        {
            get;
            set;
        }
        public PART Part
        {
            get;
            set;
        }
        public PartData Partdata
        {
            get;
            set;
        }
    }

    public class SQMStream
    {
        public STREAM Stream
        {
            get;
            set;
        }
        public StreamData Data
        {
            get;
            set;
        }
        public List<STREAM_REC> RecList
        {
            get;
            set;
        }
        public List<STREAM_NC> NCList
        {
            get;
            set;
        }
        public List<STREAM_MEASURE> MeasureList
        {
            get;
            set;
        }
        public bool isNew
        {
            get;
            set;
        }
        public string SessionID
        {
            get;
            set;
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }

        public SQMStream CreateNew(string sessionID)
        {
            this.Stream = new STREAM();
            this.Data = new StreamData();
            this.RecList = new List<STREAM_REC>();
            this.NCList = new List<STREAM_NC>();
            this.MeasureList = new List<STREAM_MEASURE>();
            this.isNew = true;
            this.SessionID = sessionID;
            this.Entities = new PSsqmEntities();

            return this;
        }

        public SQMStream Load(decimal streamID)
        {
            this.Entities = new PSsqmEntities();
            this.isNew = false;
            this.Stream = LookupStream(this.Entities, streamID);
            if (this.Stream.PART_ID > 0)
                this.Data.Partdata = SQMModelMgr.LookupPartData(this.Entities, (decimal)this.Stream.COMPANY_ID, (decimal)this.Stream.PART_ID);
            if (this.Stream.SUPP_COMPANY_ID > 0)
            {
                this.Data.Supplier = SQMModelMgr.LookupCompany((decimal)this.Stream.SUPP_COMPANY_ID);
                this.Data.SupplierPlant = SQMModelMgr.LookupPlant((decimal)this.Stream.SUPP_PLANT_ID);
            }
            if (this.Stream.CUST_COMPANY_ID > 0)
            {
                this.Data.Customer = SQMModelMgr.LookupCompany((decimal)this.Stream.CUST_COMPANY_ID);
                this.Data.CustomerPlant = SQMModelMgr.LookupPlant((decimal)this.Stream.CUST_PLANT_ID);
            }
            this.RecList = FillHistory(12);

            return this;
        }

        public static STREAM LookupStream(SQM.Website.PSsqmEntities ctx, decimal streamID)
        {
            STREAM stream = null;
            try
            {
                stream = (from s in ctx.STREAM 
                                where (s.STREAM_ID == streamID)
                                select s).Single();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return stream;
        }

        protected List<STREAM_REC> FillHistory(int numRecs)
        {
            List<STREAM_REC> recList = new List<STREAM_REC>();
            // fetch the last 10 records
            recList = (from r in this.Entities.STREAM_REC.Include("STREAM_NC").Include("STREAM_MEASURE").Take(numRecs)
                       where (r.STREAM_ID == this.Stream.STREAM_ID)
                       select r).OrderByDescending(l=> l.EFF_DT).ToList();

            return recList;
        }


        public static List<StreamData> SelectPartStreamList(decimal companyID, decimal plantID, decimal partID, string option)
        {
            // get all part number streams related to suppliers, customers and internal production
            List<StreamData> streamList = new List<StreamData>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    if (partID > 0)
                    {
                        if (option.ToUpper() == "S")
                        {   // supplier part streams
                            streamList = (from s in entities.STREAM
                                          join c in entities.COMPANY on s.SUPP_COMPANY_ID equals c.COMPANY_ID
                                          join l in entities.PLANT on s.SUPP_PLANT_ID equals l.PLANT_ID
                                          join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                          where (s.PART_ID == partID  &&  s.SUPP_COMPANY_ID != null)
                                          from p in p_s.DefaultIfEmpty()
                                          select new StreamData
                                          {
                                              STREAM_ID = s.STREAM_ID,
                                              COMPANY_ID = s.COMPANY_ID,
                                              PLANT_ID = s.PLANT_ID,
                                              PLANT_LINE_ID = s.PLANT_LINE_ID,
                                              Supplier = c,
                                              SupplierPlant = l,
                                              Part = p
                                          }).ToList();
                        }
                        if (option.ToUpper() == "C")
                        {   // customer parts
                            streamList = (from s in entities.STREAM
                                          join c in entities.COMPANY on s.CUST_COMPANY_ID equals c.COMPANY_ID
                                          join l in entities.PLANT on s.CUST_PLANT_ID equals l.PLANT_ID
                                          join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                          where (s.PART_ID == partID && s.CUST_COMPANY_ID != null)
                                          from p in p_s.DefaultIfEmpty()
                                          select new StreamData
                                          {
                                              STREAM_ID = s.STREAM_ID,
                                              COMPANY_ID = s.COMPANY_ID,
                                              PLANT_ID = s.PLANT_ID,
                                              PLANT_LINE_ID = s.PLANT_LINE_ID,
                                              Customer = c,
                                              CustomerPlant = l,
                                              Part = p
                                          }).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return streamList;
        }

        public static List<StreamData> SelectSuppStreamList(decimal companyID, decimal plantID, string suppString, string partString)
        {
            // get all part number streams related to suppliers, customers and internal production
            List<StreamData> streamList = new List<StreamData>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    string suppUpper = suppString.ToUpper();
                    string partUpper = partString.ToUpper();

                    if (!string.IsNullOrEmpty(suppString) &&  !string.IsNullOrEmpty(partString))
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.SUPP_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.SUPP_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.SUPP_COMPANY_ID != null &&  c.COMPANY_NAME.ToUpper().Contains(suppUpper))
                                      from p in p_s.DefaultIfEmpty().Where(p => p.PART_NUM.ToUpper().Contains(partUpper) &&  p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Supplier = c,
                                          SupplierPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(suppString))
                    {
                        streamList = (from s in entities.STREAM
                                        join c in entities.COMPANY on s.SUPP_COMPANY_ID equals c.COMPANY_ID
                                        join l in entities.PLANT on s.SUPP_PLANT_ID equals l.PLANT_ID
                                        join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                        where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.SUPP_COMPANY_ID != null && c.COMPANY_NAME.ToUpper().Contains(suppUpper))
                                        from p in p_s.DefaultIfEmpty().Where(p => p.STATUS == "A")
                                        select new StreamData
                                        {
                                            STREAM_ID = s.STREAM_ID,
                                            COMPANY_ID = s.COMPANY_ID,
                                            PLANT_ID = s.PLANT_ID,
                                            PLANT_LINE_ID = s.PLANT_LINE_ID,
                                            Supplier = c,
                                            SupplierPlant = l,
                                            Part = p
                                        }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(partString))
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.SUPP_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.SUPP_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.SUPP_COMPANY_ID != null)
                                      from p in p_s.DefaultIfEmpty().Where(p => p.PART_NUM.ToUpper().Contains(partUpper) &&  p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Supplier = c,
                                          SupplierPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                    else
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.SUPP_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.SUPP_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.SUPP_COMPANY_ID != null)
                                      from p in p_s.DefaultIfEmpty().Where(p => p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Supplier = c,
                                          SupplierPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return streamList;
        }

        public static List<StreamData> SelectCustStreamList(decimal companyID, decimal plantID, string custString, string partString)
        {
            // get all part number streams related to customers
            List<StreamData> streamList = new List<StreamData>();
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    string custUpper = custString.ToUpper();
                    string partUpper = partString.ToUpper();

                    if (!string.IsNullOrEmpty(custString) && !string.IsNullOrEmpty(partString))
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.CUST_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.CUST_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.CUST_COMPANY_ID != null && c.COMPANY_NAME.ToUpper().Contains(custUpper))
                                      from p in p_s.DefaultIfEmpty().Where(p => p.PART_NUM.ToUpper().Contains(partUpper) && p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Customer = c,
                                          CustomerPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(custString))
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.CUST_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.CUST_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.CUST_COMPANY_ID != null && c.COMPANY_NAME.ToUpper().Contains(custUpper))
                                      from p in p_s.DefaultIfEmpty().Where(p => p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Customer = c,
                                          CustomerPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(partString))
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.CUST_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.CUST_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.CUST_COMPANY_ID != null)
                                      from p in p_s.DefaultIfEmpty().Where(p => p.PART_NUM.ToUpper().Contains(partUpper) && p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Customer = c,
                                          CustomerPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                    else
                    {
                        streamList = (from s in entities.STREAM
                                      join c in entities.COMPANY on s.CUST_COMPANY_ID equals c.COMPANY_ID
                                      join l in entities.PLANT on s.CUST_PLANT_ID equals l.PLANT_ID
                                      join p in entities.PART on s.PART_ID equals p.PART_ID into p_s
                                      where (s.COMPANY_ID == companyID && s.PLANT_ID == plantID && s.CUST_COMPANY_ID != null)
                                      from p in p_s.DefaultIfEmpty().Where(p => p.STATUS == "A")
                                      select new StreamData
                                      {
                                          STREAM_ID = s.STREAM_ID,
                                          COMPANY_ID = s.COMPANY_ID,
                                          PLANT_ID = s.PLANT_ID,
                                          PLANT_LINE_ID = s.PLANT_LINE_ID,
                                          Customer = c,
                                          CustomerPlant = l,
                                          Part = p
                                      }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return streamList;
        }
    }
}
