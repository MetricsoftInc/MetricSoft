using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Xml;
using System.Reflection;
using SQM.Shared;
using System.IO;
using System.Web.Configuration;
using System.Configuration;
using System.Globalization;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace SQM.Website
{
    public class SQMFileReader
    {
        class KeyAttributeMissingError : System.Exception { }

        public int CompanyID
        {
            get;
            set;
        }
        public string LocationCode
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public XmlReader Reader
        {
            get;
            set;
        }
        public Stream FileStream
        {
            get;
            set;
        }
        public int NodeNo
        {
            get;
            set;
        }
        public int Status
        {
            get;
            set;
        }
		public char[] Delimiter
		{
			get;
			set;
		}
		public double PlantDataMultiplier
		{
			get;
			set;
		}
		public int PeriodYear
		{
			get;
			set;
		}
		public int PeriodMonth
		{
			get;
			set;
		}
		public string DefaultCurrency
		{
			get;
			set;
		}
		public string FileType
		{
			get;
			set;
		}
		public List<String> PreviewList
        {
            get;
            set;
        }
        public List<FileReaderUpdate> UpdateList
        {
            get;
            set;
        }
        public List<FileReaderError> ErrorList
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }

        public SQMFileReader Initialize(int companyID, byte[] fileContent)
        {
            try
            {
                this.Status = 0;
                this.NodeNo = 0;
                this.UpdateList = new List<FileReaderUpdate>();
                this.ErrorList = new List<FileReaderError>();
                this.PreviewList = new List<string>();
                this.CompanyID = companyID;

                XmlDocument doc = new XmlDocument();
                string xml = Encoding.UTF8.GetString(fileContent);
                doc.LoadXml(xml);
                Reader = XmlReader.Create(new System.IO.StringReader(xml));
            }
            catch
            {
                Status = -11;       // Error encountered reading or parsing the file
            }

            return this;
        }

        public SQMFileReader InitializeCSV(int companyID, string fileName,  byte[] fileContent, char[] delimiter, double plantDataMultiplier, int year, int month, string currency)
        {
            try
            {
                this.Status = 0;
                this.NodeNo = 0;
                this.UpdateList = new List<FileReaderUpdate>();
                this.ErrorList = new List<FileReaderError>();
                this.PreviewList = new List<string>();
                this.CompanyID = companyID;
                this.Entities = new PSsqmEntities();
				this.Delimiter = delimiter;
				this.PlantDataMultiplier = plantDataMultiplier;
				this.PeriodYear = year;
				this.PeriodMonth = month;
                this.DefaultCurrency = "EUR";// currency;
				this.FileType = fileName.Substring(fileName.IndexOf('.'));

                //string fileType = flUpload.PostedFile.ContentType;
                //fileContent = new byte[Convert.ToInt32(fileLen)];
                //int nBytes = flUpload.PostedFile.InputStream.Read(fileContent, 0, Convert.ToInt32(fileLen));


                this.FileName = fileName.Substring(0, fileName.IndexOf('.'));
                this.FileStream = new MemoryStream(fileContent);
            }
            catch
            {
                Status = -11;       // Error encountered reading or parsing the file
            }

            return this;
        }

		public SQMFileReader InitializeExcel(int companyID, string fileName, Stream fileContent, char[] delimiter, double plantDataMultiplier, int year, int month, string currency)
		{
			try
			{
				this.Status = 0;
				this.NodeNo = 0;
				this.UpdateList = new List<FileReaderUpdate>();
				this.ErrorList = new List<FileReaderError>();
				this.PreviewList = new List<string>();
				this.CompanyID = companyID;
				this.Entities = new PSsqmEntities();
				this.Delimiter = delimiter;
				this.PlantDataMultiplier = plantDataMultiplier;
				this.PeriodYear = year;
				this.PeriodMonth = month;
				this.DefaultCurrency = currency;
				this.FileType = fileName.Substring(fileName.IndexOf('.'));
				this.FileName = fileName.Substring(0, fileName.IndexOf('.'));
				this.FileStream = fileContent;
			}
			catch
			{
				Status = -11;       // Error encountered reading or parsing the file
			}

			return this;
		}


        public int LoadCSV()
        {
			Entities = new PSsqmEntities();
            Status = 0;
            int lastLineNo = 0;
            string lastLine = "";
            List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("FILE_UPLOAD", ""); // ABW 20140805
            List<SETTINGS> recpts = SQMSettings.SelectSettingsGroup("IMPORT_RECEIPT", ""); // ABW 20140805
			List<PLANT> plantList = new List<PLANT>();  // mt 
			var accessPlantList = new[] { new { plant = "", assoc = "" } }.ToList(); // mt
			string accessLocations = "";
			// use  settings below to limit processing to a discrete list of plants
			SETTINGS dfltPlant = sets.Where(s => s.SETTING_CD == "PlantCode").FirstOrDefault() == null ? new SETTINGS() : sets.Where(s => s.SETTING_CD == "PlantCode").FirstOrDefault();
			string[] dfltPlantList = string.IsNullOrEmpty(dfltPlant.VALUE) ? new string[0] : dfltPlant.VALUE.Split(',');

            try
            {
                using (StreamReader sr = new StreamReader(this.FileStream, Encoding.Default, true))
                {
                    string line;
                    int lineNo = 0;

					List<JOBCODE> jobcodeList = new List<JOBCODE>();
					if (this.FileName == "PERSON")
					{
						jobcodeList = SQMModelMgr.SelectJobcodeList("A", "");
						plantList = SQMModelMgr.SelectPlantList(new PSsqmEntities(), 1, 0);
						// compile list of plants that a PERSON can have access to based on a primary HR location
						// e.g. plant code GG also grants access to plants 31,32,33 (accessible locations)
						SETTINGS assocPlant = sets.Where(s => s.SETTING_CD == "AssocPlant").FirstOrDefault();
						if (assocPlant != null)
						{
							string[] plist = assocPlant.VALUE.Split('|');
							foreach (string p in plist)
							{
								string[] s = p.Split(':');
								accessPlantList.Add(new { plant = s[0], assoc = s[1] });
							}
						}
					}
 
                    while ((line = sr.ReadLine()) != null)
                    {
                        EntityState state;
                        decimal primaryCompanyID = 0;
                        COMPANY company = null;
                        BUSINESS_ORG busOrg = null;
                        PLANT plant = null;
                        PLANT suppPlant = null;
                        PLANT custPlant = null;
                        ADDRESS adr = null;
						PERSON person = null;
						JOBCODE jobcode = null;
                        PART part = null;
                        STREAM stream = null;
                        RECEIPT receipt = null;
						ACTIVE_CUSTOMER activeCustomer = null; // AW20140414 - for Varroc - only load 
                        lastLineNo =  (++lineNo);
                        lastLine = line;

                        string[] fldArray = line.Split(this.Delimiter);
                        if (fldArray.Length < 2)
                            break;
						for (int i = 0; i < fldArray.Length; i++)
						{
							string tmp = (fldArray[i].Replace("\"", ""));
							fldArray[i] = tmp.Trim();
						}

                        // todo: move this to top of CASE since check for primary company should be common to all files
                        try
                        {
                            primaryCompanyID = SessionManager.PrimaryCompany().COMPANY_ID;
                        }
                        catch
                        {
                            try
                            {
                                primaryCompanyID = Convert.ToInt32(sets.Find(x => x.SETTING_CD == "CompanyID").VALUE);
                            }
                            catch
                            {
                                this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "primary company undefined", "primary company undefined" + this.FileName, this.FileName, 1, line));
                                break;
                            }
                        }

						switch (this.FileName)
						{
							case "REFERENCE":
								string reftype = fldArray[0];
								string itemCD = fldArray[1];
								string itemDesc = fldArray[2];
								bool doSave = false;
								switch (reftype)
								{
									case "JOBCODE":
										jobcode = (from j in Entities.JOBCODE where j.JOBCODE_CD == itemCD select j).SingleOrDefault();
										if (jobcode == null)
										{
											jobcode = new JOBCODE();
											jobcode.JOBCODE_CD = itemCD;
											jobcode.JOB_DESC = itemDesc;
											jobcode.DEFAULT_ROLE = 0;
											Entities.AddToJOBCODE(jobcode);
										}
										else
										{
											jobcode.JOB_DESC = itemDesc;
										}
										doSave = true;
										break;
									case "LOCATION":
										doSave = true;
										break;
									default:
										break;
								}
								try
								{
									Entities.SaveChanges();
								}
								catch
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "REFERENCE", "Reference item update error: " + itemCD, itemDesc, 1, line));
									break;
								}
								break;
							case "PERSON":
								string empID = fldArray[0].Trim();
								string status = fldArray[1].Trim().ToUpper();
								string firstName = fldArray[2].Trim();
								string lastName = fldArray[3].Trim();
								string middleName = !string.IsNullOrEmpty(fldArray[4].Trim()) ? fldArray[4].Trim() : "";
								string emailAddress = !string.IsNullOrEmpty(fldArray[5].Trim()) ? fldArray[5].Trim() : "";
								string phone1 = !string.IsNullOrEmpty(fldArray[6].Trim()) ? fldArray[6].Trim() : "";
								string phone2 = !string.IsNullOrEmpty(fldArray[7].Trim()) ? fldArray[7].Trim() : "";
								string jobCode = !string.IsNullOrEmpty(fldArray[8].Trim()) ? fldArray[8].Trim() : "";
								string HRLocation = fldArray[9].Trim();
								string supvEmpID = !string.IsNullOrEmpty(fldArray[10].Trim()) ? fldArray[10].Trim() : "";

								if (dfltPlantList.Length > 0  && !dfltPlantList.Contains(HRLocation))  // check if a default plant is assigned if skip person if hr location is not that plant
								{
									break;
								}

								if ((plant = plantList.Where(l => l.DUNS_CODE == HRLocation || l.ALT_DUNS_CODE == HRLocation).FirstOrDefault()) == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PERSON", "HR Location does not exist: " + HRLocation, HRLocation, 1, line));
									break;	
								}

								person = SQMModelMgr.LookupPersonByEmpID(Entities, empID);

								if (person == null)
								{
									if (status != "A")  // don't create person if inactive
									{
										break;
									}
									person = new PERSON();
									person.EMP_ID = empID;
									person.SSO_ID = empID;
									person.ROLE = (int)SysPriv.view;
									person.NEW_LOCATION_CD = "";
								}

								person.STATUS = status;
								person.FIRST_NAME = firstName;
								person.LAST_NAME = lastName;
								person.MIDDLE_NAME = middleName;
								person.EMAIL = emailAddress;
								person.PHONE = phone1;
								person.PHONE2 = phone2;
								person.JOBCODE_CD = jobCode;
								person.COMPANY_ID = (decimal)plant.COMPANY_ID;
								person.BUS_ORG_ID = (decimal)plant.BUS_ORG_ID;
								person.PLANT_ID = (decimal)plant.PLANT_ID;
								person.SUPV_EMP_ID = supvEmpID;
								if (string.IsNullOrEmpty(person.PRIV_GROUP))
								{
									jobcode = jobcodeList.Where(l => l.JOBCODE_CD == jobCode).FirstOrDefault();  // apply default privgroup for the person's jobcode, if defined
									if (jobcode != null && !string.IsNullOrEmpty(jobcode.PRIV_GROUP))
										person.PRIV_GROUP = jobcode.PRIV_GROUP;
								}
								if ((accessLocations = accessPlantList.Where(l => l.plant == HRLocation).Select(l => l.assoc).FirstOrDefault()) != null)
								{
									person.NEW_LOCATION_CD = ("," + accessLocations + ",");
								}

								try
								{
									person = SQMModelMgr.UpdatePerson(Entities, person, "upload", false, person.SSO_ID, person.LAST_NAME);
								}
								catch (Exception ex)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PERSON", "update failure: " + empID + "; " + ex.InnerException.Message, empID, 1, line));
									break;
								}
								if (person == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PERSON", "update failure: " + empID, empID, 1, line));
									break;
								}
								state = person.EntityState;
								CreateUpdateRecord("PERSON", person.EMP_ID + ": " + line, state);
								break;

                            case "INSPECT":
                                string inspectNo = fldArray[0];
                                string rcvPoNumber = fldArray[1];
                                string NCType = fldArray[4];
                                string rcvPartNumber = fldArray[5];
                                string NCCount = fldArray[6];
                                string probType = fldArray[7];
                                string NCUom = fldArray[8];
                                string actionReqdStatus = fldArray[9];
                                string rejectBy = fldArray[10];
                                string approveBy = fldArray[11];
                                string closedStatus = fldArray[12];

                                part = SQMModelMgr.LookupPart(Entities, 0, rcvPartNumber, primaryCompanyID, true);
                                if (part == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "INSPECT", "part does not exist: " + rcvPartNumber, rcvPartNumber, 1, line));
                                    break;
                                }

                                receipt = SQMModelMgr.LookupReceipt(Entities, 0, part.PART_ID, rcvPoNumber);
                                if (receipt == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "INSPECT", "cannot find matching material receipt record: " + rcvPoNumber, rcvPoNumber, 1, line));
                                    break;
                                }

                                QualityIssue qualityIssue = new QualityIssue().CreateNew("0", "RCV");
                                state = qualityIssue.Incident.EntityState;
                                qualityIssue.Incident.DESCRIPTION = inspectNo;
                                qualityIssue.SetDetectedLocation(receipt.CUST_LOCATION);
                                qualityIssue.SetResponsibleLocation(receipt.SUPP_LOCATION);
                                qualityIssue.IssueOccur.PART_ID = part.PART_ID;
                                qualityIssue.IssueOccur.PART_TYPE = "PARTPO";  // purchased part
                                qualityIssue.Incident.CREATE_DT = DateTime.UtcNow;
                                qualityIssue.Incident.INCIDENT_DT = receipt.RECEIPT_DT;
                                qualityIssue.Incident.CREATE_BY = "upload";
                                //qualityIssue.Incident.CREATE_PERSON = ?
                                if (closedStatus.ToUpper().Contains("N"))
                                    qualityIssue.IssueOccur.STATUS = "A";

                                int NCQty = (int)System.Math.Truncate(Convert.ToDouble(NCCount));
                                qualityIssue.IssueOccur.OCCUR_DESC = ""; 
                                qualityIssue.IssueOccur.SEVERITY = "IR";
                                qualityIssue.IssueOccur.REF_OPERATION = rcvPoNumber;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_QTY = receipt.RECEIPT_QTY;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_QTY = receipt.RECEIPT_QTY;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().INSPECT_NC_QTY = NCQty;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().TOTAL_NC_QTY = NCQty;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_AREA = qualityIssue.IssueOccur.PROBLEM_AREA = "";
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = null;
                                qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT = 0;

                                if (!string.IsNullOrEmpty(probType))
                                {
                                    qualityIssue.IssueOccur.OCCUR_DESC = WebSiteCommon.GetXlatValue("problemArea","PRQ");
                                    qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_AREA = qualityIssue.IssueOccur.PROBLEM_AREA = "PRQ";
                                    NONCONFORMANCE nconf = SQMResourcesMgr.LookupNonconf(new PSsqmEntities(), 0, probType);
                                    if (nconf != null)
                                        qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().NONCONF_ID = nconf.NONCONF_ID;
                                    qualityIssue.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First().PROBLEM_COUNT = NCQty;
                                }

                                qualityIssue.IssueOccur.DISPOSITION =  actionReqdStatus.ToUpper() == "NO" ?  "NO" : "PND"; 

                                if (QualityIssue.UpdateIssue(qualityIssue) == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "INSPECT", "error saving Inspection record: " + inspectNo, inspectNo, 1, line));
                                    break;
                                }

                                CreateUpdateRecord("INSPECT", inspectNo + ": " + line, state);

                                break;

                            case "RECEIPT":
                                string poNumber = fldArray[0];
                                string rcvDate = fldArray[1];
                                string supplierID = fldArray[2];
                                string supplierName = fldArray[3];
                                string supplierCity = fldArray[4];
                                string supplierCountry = fldArray[5];
                                string partNumber = fldArray[6];
                                string partDesc = fldArray[7];
                                string rcvQty = fldArray[8];
                                string unitPrice = fldArray[9];
                                string unitCurrency = fldArray[10];

                                // todo: get receiving plant from the data file
                                PLANT rcvLocation = null; 
                                if (fldArray.Length > 11)
                                {
                                    rcvLocation = SQMModelMgr.LookupPlant(Entities, 0, fldArray[11]);
                                }
                                else
                                {
                                    rcvLocation = SQMModelMgr.LookupPlant(Entities, 0, this.LocationCode);
                                }

                                state = EntityState.Unchanged;

                                if ((company = SQMModelMgr.LookupCompany(Entities, 0, supplierID, true)) == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "create error", "cannot create supplier " + supplierID, supplierID, 1, line));
                                    break;
                                }

                                if (company.EntityState == EntityState.Detached || company.COMPANY_NAME != supplierName)
                                {
                                    company.COMPANY_NAME = supplierName;
                                    company.STATUS = "A";
                                    company.IS_PRIMARY = company.IS_CUSTOMER = false;
                                    company.IS_SUPPLIER = true;
                                    if ((company = SQMModelMgr.UpdateCompany(Entities, company, "upload")) == null)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "COMPANY", "supplier update failure: " + supplierID, supplierID, 1, line));
                                        break;
                                    }
                                }

                                if ((busOrg = SQMModelMgr.LookupBusOrg(Entities, company.COMPANY_ID, company.ULT_DUNS_CODE, false, false)) == null)
                                {
                                    if ((busOrg = SQMModelMgr.CreateTopLevelBusOrg(Entities, company, unitCurrency)) == null)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "BUSINESS_ORG", "supplier update failure: " + supplierID, supplierID, 1, line));
                                        break;
                                    }
                                }

                                suppPlant = null;
                                if ((suppPlant = SQMModelMgr.LookupPlant(Entities, company.COMPANY_ID, busOrg.BUS_ORG_ID, 0, supplierID, true)) == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "create error", "cannot create supplier location " + supplierID, supplierID, 1, line));
                                    break;
                                }

                                if (suppPlant.EntityState == EntityState.Detached)
                                {
                                    suppPlant.STATUS = "A";
                                    suppPlant.ULT_DUNS_CODE = company.ULT_DUNS_CODE;
                                    suppPlant.PLANT_NAME = supplierName;
                                    suppPlant.DISP_PLANT_NAME = supplierName;
                                    suppPlant.CURRENCY_CODE = unitCurrency;
                                    suppPlant.LOCAL_TIMEZONE = "GMT";
                                    suppPlant.LOCATION_TYPE = "P";

                                    adr = new ADDRESS();
                                    adr.ADDRESS_TYPE = "S";
                                    adr.COMPANY_ID = company.COMPANY_ID;
                                    adr.PLANT_ID = suppPlant.PLANT_ID;
                                    adr.STREET1 = supplierCity;
                                    adr.CITY = supplierCity;
                                    adr.COUNTRY = supplierCountry;
                                    Entities.AddToADDRESS(adr);

                                    try
                                    {
                                        state = suppPlant.EntityState;
                                        suppPlant = SQMModelMgr.UpdatePlant(Entities, suppPlant, "upload");
                                    }
                                    catch (Exception ex)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "supplier location update failure: " + supplierID + "; " + ex.InnerException.Message, supplierID, 1, line));
                                        break;
                                    }
                                    if (suppPlant == null)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "supplier location update failure: " + supplierID,  supplierID, 1, line));
                                        break;
                                    }
                                }

                                CreateUpdateRecord("RECEIPT", company.COMPANY_NAME + ": " + line, state);

                                part = SQMModelMgr.LookupPart(Entities, 0, partNumber, primaryCompanyID, true);
                                if (part.EntityState == EntityState.Detached)
                                {   // create new part
                                    PART_PROGRAM program = null; // SQMModelMgr.LookupPartProgram(0, ConfigurationSettings.AppSettings["DefaultPartProgram"]);
                                    //if (program == null)
                                    //{
                                    ////    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "Part Program not defined", partNumber, 1, line));
                                    //    break;
                                    //}
                                   
                                    if (program != null)
                                        part.PROGRAM_ID = program.PROGRAM_ID;
                                    part.PART_NUM = partNumber;
                                    part.PART_NAME = partDesc;
                                    part.REVISION_LEVEL = "";
                                    part.DRAWING_REF = "";
                                    part.STATUS = "A";
                                    state = part.EntityState;
                                    part = SQMModelMgr.UpdatePart(Entities, part, "upload");
                                    if (part == null)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "cannot create part: " + partNumber, partNumber, 1, line));
                                        break;
                                    }
                                    else
                                    {
                                        CreateUpdateRecord("PART", part.PART_NUM + ": " + line, state);
                                    }
                                }

                                stream = null;
                                part.STREAM.Load();  // get all streams for this part and lookup supplier/received location 
                                stream = part.STREAM.Where(s => s.PART_ID == part.PART_ID && s.PLANT_ID == rcvLocation.PLANT_ID  &&  s.SUPP_PLANT_ID == suppPlant.PLANT_ID).FirstOrDefault();
                              
                                if (stream == null)
                                {
                                    stream = SQMModelMgr.CreatePartStream((decimal)part.COMPANY_ID, (decimal)part.PART_ID);
                                    // primary company is customer
                                    stream.COMPANY_ID = (decimal)rcvLocation.COMPANY_ID;
                                    stream.PLANT_ID = rcvLocation.PLANT_ID;
                                    stream.SUPP_COMPANY_ID = suppPlant.COMPANY_ID;
                                    stream.SUPP_PLANT_ID = suppPlant.PLANT_ID;
                                    stream.SUPP_PART_NUM = part.PART_NUM;
                                    stream.CUST_COMPANY_ID = stream.CUST_PLANT_ID = null;
                                    
                                    try
                                    {
                                        stream = SQMModelMgr.UpdatePartStream(Entities, stream, "admin");
                                    }
                                    catch (Exception ex)
                                    {
                                        this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "Part Stream not created; " + ex.InnerException.Message, partNumber, 1, line));
                                        break;
                                    }
                                }
                 
					            // if we got to this point then we can create a receipt record
                                DateTime receiptDate;
                                try
                                {
                                    receiptDate = DateTime.ParseExact(rcvDate, recpts.Find(x => x.SETTING_CD == "ReceiptDateFormat").VALUE, null);
                                }
                                catch
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "RECEIPT", "Receipt data error; " + "receipt date not parsed", partNumber, 1, line));
                                    break;
                                }

                                decimal qty = 0;
                                decimal.TryParse(rcvQty, out qty);
                                decimal price = 0;
                                decimal.TryParse(unitPrice, out price);

                                // does receipt already exist for this po and part ?
                                receipt = SQMModelMgr.LookupReceipt(Entities, 0, part.PART_ID, poNumber);
                                if (receipt == null)
                                {
                                    receipt = SQMModelMgr.CreateReceipt(receiptDate, poNumber, rcvLocation.PLANT_ID, suppPlant.PLANT_ID, part.PART_ID);
                                    receipt.PO_NUMBER = poNumber;
                                }
                                state = receipt.EntityState;
                                receipt.RECEIPT_QTY = qty;
                                receipt.RECEIPT_COST = qty * price;
                                receipt.CURRENCY_CODE = unitCurrency;
                                
                                if ((receipt = SQMModelMgr.UpdateReceipt(Entities, receipt, SessionManager.UserContext.UserName())) == null)
                                {
                                    this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "RECEIPT", "Error creating receipt record " + "error creating receipt record", poNumber, 1, line));
                                    break;
                                }
                                else
                                {
                                    CreateUpdateRecord("RECEIPT", poNumber + ": " + line, state);
                                }
                                break;
							case "COMPANY":
							case "SUPPLIER":
							case "CUSTOMER":
								// company record: Status,Company Name,Company Identifier,Company Type
								string companyStatus = fldArray[0];
								string companyName = fldArray[1];
								string companyIdentifier = fldArray[2];
								string companyType = fldArray[3];
								// AW20140414 - VARROC - only process companies in the ACTIVE_CUSTOMER; bow out if it is not there
								if ((activeCustomer = SQMModelMgr.LookupActiveCustomer(Entities, companyIdentifier)) == null)
									break;
								if ((company = SQMModelMgr.LookupCompany(Entities, 0, companyIdentifier, true)) == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "create error", "cannot create company " + companyIdentifier, companyIdentifier, 1, line));
									break;
								}
								company.COMPANY_NAME = companyName;
								company.STATUS = companyStatus;
								company.IS_PRIMARY = company.IS_CUSTOMER = company.IS_SUPPLIER = false;
								if (this.FileName == "SUPPLIER" || companyType == "2" || companyType.ToUpper() == "S")
									company.IS_SUPPLIER = true;
								else if (companyType == "3" || companyType.ToUpper() == "B")
								{
									company.IS_SUPPLIER = true;
									company.IS_CUSTOMER = true;
								}
								else
									company.IS_CUSTOMER = true;
								state = company.EntityState;

								if ((company = SQMModelMgr.UpdateCompany(Entities, company, "upload")) == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "COMPANY", "update failure: " + companyIdentifier, companyIdentifier, 1, line));
									break;
								}

								if ((busOrg = SQMModelMgr.LookupBusOrg(Entities, company.COMPANY_ID, company.ULT_DUNS_CODE, false, false)) == null)
								{
									if ((busOrg = SQMModelMgr.CreateTopLevelBusOrg(Entities, company, "USD")) == null)
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "BUSINESS_ORG", "update failure: " + companyIdentifier, companyIdentifier, 1, line));
										break;
									}
								}

								CreateUpdateRecord("COMPANY", company.COMPANY_NAME + ": " + line, state);

								break;
							case "PLANT":
								//Status,Company Identifier,Plant Name,Plant Identifier,Street Address1,Street Address2,City,State/Province,
								//Postal Code,Country Code,UTC timezone code,ISO currency code
								string plantStatus = fldArray[0];
								string plantCompanyIdentifier = fldArray[1];
								string plantName = fldArray[2];
								string plantIdentifier = fldArray[3];
								string plantAddr1 = fldArray[4];
								string plantAddr2 = fldArray[5];
								string plantCity = fldArray[6];
								string plantState = fldArray[7];
								string plantPostCode = fldArray[8];
								string plantCountry = fldArray[9];
								string plantUTC = fldArray[10];
								string plantCurrency = "";
								try
								{
									plantCurrency = fldArray[11];
								}
								catch
								{
									plantCurrency = this.DefaultCurrency;
								}
								// AW20140414 - VARROC - only process companies in the ACTIVE_CUSTOMER; bow out if it is not there
								if ((activeCustomer = SQMModelMgr.LookupActiveCustomer(Entities, plantCompanyIdentifier)) == null)
									break;
								if ((company = SQMModelMgr.LookupCompany(Entities, 0, plantCompanyIdentifier, false)) == null) // we don't want to create a new one at this time
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "Company " + plantCompanyIdentifier + " not found", plantCompanyIdentifier, 1, line));
									break;
								}
								if ((busOrg = SQMModelMgr.LookupBusOrg(Entities, company.COMPANY_ID, company.ULT_DUNS_CODE, false, false)) == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "Business Organization not found for " + plantCompanyIdentifier, plantCompanyIdentifier, 1, line));
									break;
								}
								plant = SQMModelMgr.LookupPlant(Entities, company.COMPANY_ID, busOrg.BUS_ORG_ID, 0, plantIdentifier, true);
								plant.STATUS = plantStatus;
								plant.ULT_DUNS_CODE = company.ULT_DUNS_CODE;
								plant.PLANT_NAME = plantName;
								plant.DISP_PLANT_NAME = plantName;
								plant.CURRENCY_CODE = plantCurrency;
								plant.LOCAL_TIMEZONE = WebSiteCommon.GetUTC(plantUTC);
								plant.LOCATION_TYPE = "P";
								//plant.TRACK_FIN_DATA = true;
								adr = plant.ADDRESS.FirstOrDefault();
								if (adr == null)
								{
									adr = new ADDRESS();
									adr.ADDRESS_TYPE = "S";
									adr.COMPANY_ID = company.COMPANY_ID;
									adr.PLANT_ID = plant.PLANT_ID;
									Entities.AddToADDRESS(adr);
								}
								adr.STREET1 = plantAddr1;
								adr.STREET2 = plantAddr2;
								adr.CITY = plantCity;
								adr.STATE_PROV = plantState;
								adr.POSTAL_CODE = plantPostCode;
								adr.COUNTRY = plantCountry;
								try
								{
									plant = SQMModelMgr.UpdatePlant(Entities, plant, "upload");
								}
								catch (Exception ex)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "update failure: " + plantIdentifier + "; " + ex.InnerException.Message, plantIdentifier, 1, line));
									break;
								}
								if (plant == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT", "update failure: " + plantIdentifier, plantIdentifier, 1, line));
									break;
								}
								state = plant.EntityState;
								CreateUpdateRecord("PLANT", plant.PLANT_NAME + ": " + line, state);

								break;
							case "PART":
								// Status,Part Number,Supplied-from Plant,Customer Identifier,Customer Plant Identifier,Program Code,Customer Part Number,Revision number
								string partStatus = fldArray[0];
								string partNum = fldArray[1];
								
								string supp = fldArray[2];
								string cust = fldArray[3];
								string custPlantID = fldArray[4]; // if this is not supplied, use the ULT_DUNS
								string programCode = fldArray[5];
								string custPartNum = fldArray[6];
								string revNum = "";
                                string partName = "";
                                //try
                                //{
                                //    revNum = fldArray[7];
                                //}
                                //catch { }
                                try
                                {
                                    if (fldArray.Length == 8)
                                        partName = fldArray[7];
                                    else if (fldArray.Length == 9)
                                        partName = fldArray[8];
                                }
                                catch { partName = partNum; }

								string suppPartNum = "";

								if (custPlantID == "")
									custPlantID = cust;

								suppPlant = SQMModelMgr.LookupPlant(Entities, 0, supp);
								custPlant = SQMModelMgr.LookupPlant(Entities, 0, custPlantID);
								// AW20140414 - VARROC - only process customers in the ACTIVE_CUSTOMER; bow out if it is not there
								if (custPlant == null || (activeCustomer = SQMModelMgr.LookupActiveCustomer(Entities, custPlant.ULT_DUNS_CODE)) == null)
									break;
								if (suppPlant == null || custPlant == null)
								{
									if (suppPlant == null)
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "supplied-from plant undefined: " + supp, "record not found", supp, 1, line));
									if (custPlant == null)
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "customer plant undefined: " + cust, "record not found", cust, 1, line));
								}
								else
								{
									if (programCode == "")
									{
                                        programCode = sets.Find(x => x.SETTING_CD == "DefaultPartProgram").VALUE;
										if (programCode == "")
										{
											this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "Part Program not supplied", partNum, 1, line));
											break;
										}
									}
									PART_PROGRAM program = SQMModelMgr.LookupPartProgram(0, programCode);
									if (program == null)
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "part program undefined: " + programCode, "record not found", programCode, 1, line));
									}
									else
									{
										part = SQMModelMgr.LookupPart(Entities, 0, partNum, primaryCompanyID, true);
										if (part != null)
										{
											part.STATUS = partStatus;
											part.PROGRAM_ID = program.PROGRAM_ID;
											part.PART_NAME = partName;
											part.REVISION_LEVEL = revNum;
                                            part.DRAWING_REF = custPartNum;
											state = part.EntityState;
											part = SQMModelMgr.UpdatePart(Entities, part, "upload");
											if (part == null)
											{
												this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "update failure: " + partNum, partNum, 1, line));
												break;
											}
											else
											{
												CreateUpdateRecord("PART", part.PART_NUM + ": " + line, state);
											}

											stream = null;
											part.STREAM.Load();  // get all streams for this part
											if (custPlant.COMPANY_ID != primaryCompanyID)
												stream = part.STREAM.Where(s => s.PART_ID == part.PART_ID && s.CUST_PLANT_ID == custPlant.PLANT_ID && s.PLANT_ID == suppPlant.PLANT_ID).FirstOrDefault();
											else
												stream = part.STREAM.Where(s => s.PART_ID == part.PART_ID && s.SUPP_PLANT_ID == suppPlant.PLANT_ID).FirstOrDefault();
											if (stream == null)
											{
												stream = SQMModelMgr.CreatePartStream((decimal)part.COMPANY_ID, (decimal)part.PART_ID);

												if (custPlant.COMPANY_ID != primaryCompanyID)
												{   // primary company is supplier to customer
													stream.COMPANY_ID = (decimal)suppPlant.COMPANY_ID;
													stream.PLANT_ID = suppPlant.PLANT_ID;
													stream.CUST_COMPANY_ID = custPlant.COMPANY_ID;
													stream.CUST_PLANT_ID = custPlant.PLANT_ID;
													stream.CUST_PART_NUM = custPartNum;
													stream.SUPP_COMPANY_ID = stream.SUPP_PLANT_ID = null;
												}
												else
												{       // primary company is customer
													stream.COMPANY_ID = (decimal)custPlant.COMPANY_ID;
													stream.PLANT_ID = custPlant.PLANT_ID;
													stream.SUPP_COMPANY_ID = suppPlant.COMPANY_ID;
													stream.SUPP_PLANT_ID = suppPlant.PLANT_ID;
													stream.SUPP_PART_NUM = suppPartNum;
													stream.CUST_COMPANY_ID = stream.CUST_PLANT_ID = null;
												}
												try
												{
                                                    stream = SQMModelMgr.UpdatePartStream(Entities, stream, "upload");
												}
												catch (Exception ex)
												{
													this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PART", "Part Stream not updated; " + ex.InnerException.Message, partNum, 1, line));
													break;
												}
											}
										}
									}
								}
								break;
							case "PLANT_DATA":
								// Plant Identifier,Period year,peroid month,Material Cost,Revenue
								string plantdataIdentifier = fldArray[0];
								string peroidYear = fldArray[1];
                                string periodMonth = fldArray[2];
                                string strPlantRevenue = fldArray[3];
								string strPlantMaterialCost = fldArray[4];
								double plantdataMultiplier = this.PlantDataMultiplier;
								
								plant = SQMModelMgr.LookupPlant(Entities, 0, plantdataIdentifier);
								if (plant == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "plant undefined", "record not found", plantdataIdentifier, 1, line));
								}
								else
								{
									double plantMaterialCost = 0.00;
									// strip out signed indicators and convert to numeric values
									bool bNumeric = WebSiteCommon.FormatCost(strPlantMaterialCost, out plantMaterialCost);
									if (!bNumeric)
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "plant data", "Plant Material Cost invalid", plantdataIdentifier, 1, line));
										break;
									}
									plantMaterialCost = plantMaterialCost * plantdataMultiplier;
									double plantRevenue = 0.00;
									bNumeric = WebSiteCommon.FormatCost(strPlantRevenue, out plantRevenue);
									if (!bNumeric)
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "plant data", "Plant Revenue invalid", plantdataIdentifier, 1, line));
										break;
									}
									plantRevenue = plantRevenue * plantdataMultiplier;

									int year = 0;
									int month = 0;
									try
							        {
                                        year = Convert.ToInt32(peroidYear);
                                        month = Convert.ToInt32(periodMonth);
                                        DateTime dt = new DateTime(year, month, 1);
									}
									catch
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "plant data", "Plant Period invalid", plantdataIdentifier, 1, line));
										break;
									}
									PLANT_ACCOUNTING pa = EHSModel.LookupPlantAccounting(Entities, plant.PLANT_ID, year, month, true);
									if (pa != null)
									{
										pa.OPER_COST = Convert.ToDecimal(plantMaterialCost);
										pa.REVENUE = Convert.ToDecimal(plantRevenue);
										pa.LAST_UPD_BY = "upload";
										state = pa.EntityState;
										pa = EHSModel.UpdatePlantAccounting(Entities, pa);
										//double rate = Convert.ToDouble(fldArray[5]);
										//if (!string.IsNullOrEmpty(plant.CURRENCY_CODE) && rate > 0)
										//	CurrencyConverter.InsertRate(plant.CURRENCY_CODE, year, month, rate);
										if (pa == null)
										{
											this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "PLANT_DATA", "update failure", plantdataIdentifier, 1, line));
										}
										else
										{
											CreateUpdateRecord("PLANT_DATA", plant.PLANT_NAME + ": " + line, state);
										}
									}
								}
								break;
							case "CURRENCY_DATA":
								//default currency code, currency code, rate
								string currencyCode = fldArray[0];
								string currencyCodeTo = fldArray[1];
								int currencyPeriodYear = this.PeriodYear;
								int currencyPeriodMonth = this.PeriodMonth;
								double currencyRate = 0;
								if (!currencyCodeTo.ToString().ToUpper().Equals(this.DefaultCurrency.ToUpper()))
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "The parent business organization currency code (" + this.DefaultCurrency + ") does not match " + currencyCodeTo, currencyCode, 1, line));
									break;
								}
								try
								{
									currencyRate = Convert.ToDouble(fldArray[2]);
								}
								catch
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "invalid currency rate (" + fldArray[2] + ") for currency " + currencyCode, currencyCode, 1, line));
									break;
								}
								try
								{
									DateTime dtTmp = new DateTime(currencyPeriodYear, currencyPeriodMonth, 1);
								}
								catch
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "invalid period (" + currencyPeriodMonth.ToString() + "/" + currencyPeriodYear.ToString() + ") for currency " + currencyCode, currencyCode, 1, line));
									break;
								}
								try
								{
									CURRENCY_XREF xref = CurrencyConverter.InsertRate(currencyCode, currencyPeriodYear, currencyPeriodMonth, currencyRate);
									if (xref == null)
									{
										this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "update failure on currency " + currencyCode, currencyCode, 1, line));
										break;
									}
									else
									{
										state = xref.EntityState;
										CreateUpdateRecord("CURRENCY_DATA", currencyCode + ": " + line, state);
									}
								}
								catch (Exception ex)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "update failure on currency " + currencyCode + ": " + ex.Message, currencyCode, 1, line));
									break;
								}
								break;
							default:
								break;
						}
                    }
                }
            }
            catch (Exception ex)
            {
				this.ErrorList.Add(new FileReaderError().CreateNew(lastLineNo, "processing error", ex.Message, lastLine, 1, ""));
                Status = -12;
            }

            return Status;
        }

		public int LoadExcel()
		{
			Status = 0;
			int lastLineNo = 0;
			string lastLine = "";
			HSSFWorkbook hssfworkbook;
			XSSFWorkbook xssfworkbook;
			ISheet sheet;
			if (this.FileType.Equals(".xlsx"))
			{
				using (Stream file = this.FileStream)
				{
					try
					{
						xssfworkbook = new XSSFWorkbook(file);
						sheet = xssfworkbook.GetSheetAt(0);
					}
					catch
					{
						this.ErrorList.Add(new FileReaderError().CreateNew(0, "read error", "cannot open xlsx file", "", 1, ""));
						return 1;
					}
				}
			}
			else
			{
				using (Stream file = this.FileStream)
				{
					try
					{
						hssfworkbook = new HSSFWorkbook(file);
						sheet = hssfworkbook.GetSheetAt(0);
					}
					catch
					{
						this.ErrorList.Add(new FileReaderError().CreateNew(0, "read error", "cannot open xls file", "", 1, ""));
						return 1;
					}
				}
			}
			IRow row;
			System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
			string line = "";
			int lineNo = 0;

			try
			{
				while (rows.MoveNext())
				{
					if (this.FileType.Equals(".xlsx"))
						row = (XSSFRow)rows.Current;
					else
						row = (HSSFRow)rows.Current;

					EntityState state;
					COMPANY company = null;
					BUSINESS_ORG busOrg = null;
					PLANT plant = null;
					lastLineNo = (++lineNo);
					lastLine = line;


					switch (this.FileName)
					{
						case "CURRENCY_DATA":
							//Effective Date, currency code, rate
							string currencyCode = row.GetCell(0).ToString();
							string currencyCodeTo = row.GetCell(1).ToString();
							int currencyPeriodYear = this.PeriodYear;
							int currencyPeriodMonth = this.PeriodMonth;
							double currencyRate = 0;
							if (!currencyCodeTo.ToString().ToUpper().Equals(this.DefaultCurrency.ToUpper()))
							{
								this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "The parent business organization currency code (" + this.DefaultCurrency + ") does not match " + currencyCodeTo, currencyCode, 1, line));
								break;
							}
							try
							{
								currencyRate = Convert.ToDouble(row.GetCell(2).ToString());
							}
							catch
							{
								this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "invalid currency rate (" + row.GetCell(2).ToString() + ") for currency " + currencyCode, currencyCode, 1, line));
								break;
							}
							try
							{
								DateTime dtTmp = new DateTime(currencyPeriodYear, currencyPeriodMonth, 1);
							}
							catch
							{
								this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "invalid period (" + currencyPeriodMonth.ToString() + "/" + currencyPeriodYear.ToString() + ") for currency " + currencyCode, currencyCode, 1, line));
								break;
							}
							try
							{
								CURRENCY_XREF xref = CurrencyConverter.InsertRate(currencyCode, currencyPeriodYear, currencyPeriodMonth, currencyRate);
								if (xref == null)
								{
									this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "update failure on currency " + currencyCode, currencyCode, 1, line));
									break;
								}
								else
								{
									state = xref.EntityState;
									CreateUpdateRecord("CURRENCY_DATA", currencyCode + ", " + currencyRate.ToString(), state);
								}
							}
							catch (Exception ex)
							{
								this.ErrorList.Add(new FileReaderError().CreateNew(lineNo, "CURRENCY_DATA", "update failure on currency " + currencyCode + ": " + ex.Message, currencyCode, 1, line));
								break;
							}
							break;
						default:
							break;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorList.Add(new FileReaderError().CreateNew(lastLineNo, "processing error", ex.Message, lastLine, 1, ""));
				Status = -12;
			}

			return Status;
		}


        private bool ValidKey(string keyValue)
        {
            bool isValid = false;

            if (string.IsNullOrEmpty(keyValue))
                throw new KeyAttributeMissingError();
            else
                isValid = true;

            return isValid;
        }

        private FileReaderUpdate CreateUpdateRecord(string obj, string value, EntityState objState)
        {
            FileReaderUpdate updRec = new FileReaderUpdate().CreateNew(obj, value, "");
            switch (objState)
            {
                case EntityState.Detached:
                    updRec.Action = "Add";
                    break;
                case EntityState.Modified:
                    updRec.Action = "Upate";
                    break;
                case EntityState.Deleted:
                    updRec.Action = "Delete";
                    break;
                default:
                    updRec.Action = "None";
                    break;
            }
            
            this.UpdateList.Add(updRec);

            return updRec;
        }


        private int ReadToNodeEnd(string nodeName)
        {
            while (this.Reader.Read())
            {
                if (this.Reader.NodeType == XmlNodeType.Element)
                    ++NodeNo;
                if (this.Reader.NodeType == XmlNodeType.EndElement && this.Reader.Name.ToUpper() == nodeName)
                    break;
            }
            return NodeNo;
        }

        #region plants


        #endregion

        #region parts

 
        #endregion
    }
}