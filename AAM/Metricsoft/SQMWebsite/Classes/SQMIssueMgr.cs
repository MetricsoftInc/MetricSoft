using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;


namespace SQM.Website
{

	[Serializable]
	public class PartIssueItem
    {
        public string PART_NUM
        {
            get;
            set;
        }
        public string LOT_NUM
        {
            get;
            set;
        }
        public string CONTAINER_NUM
        {
            get;
            set;
        }
        public decimal NC_QTY
        {
            get;
            set;
        }
    }


	[Serializable]
	public class QualityIncidentData
    {
        public decimal INCIDENT_ID
        {
            get;
            set;
        }
        public INCIDENT Incident
        {
            get;
            set;
        }
        public QI_OCCUR QIIssue
        {
            get;
            set;
        }
        public QI_OCCUR_ITEM QIItem
        {
            get;
            set;
        }
        public QI_OCCUR_NC QISample
        {
            get;
            set;
        }
        public NONCONFORMANCE Nonconform
        {
            get;
            set;
        }
        public PART Part
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public PLANT PlantResponsible
        {
            get;
            set;
        }
        public TASK_STATUS Task
        {
            get;
            set;
        }
        public PERSON Person
        {
            get;
            set;
        }
        public string ListAggregate
        {
            get;
            set;
        }
        public List<ATTACHMENT> AttachList
        {
            get;
            set;
        }
 
        public QI_OCCUR LoadIssue(PSsqmEntities ctx)
        {
            this.QIIssue = QualityIssue.LookupIssue(ctx, this.Incident.INCIDENT_ID);
            return this.QIIssue;
        }
    }

	#region qualityissue

	[Serializable]
	public class QualityIssueCtl
    {
        public QualityIssue qualityIssue
        {
            get;
            set;
        }
        public string Context
        {
            get;
            set;
        }
        public PageUseMode PageMode 
        {
            get;
            set;
        }
        public QSCalcsCtl CalcsCtl
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }

        public QualityIssueCtl Initialize(PSsqmEntities entities, string context)
        {
            this.Context = context;
            this.PageMode = PageUseMode.ViewOnly;
            if (entities != null)
                this.Entities = entities;
            InitializeCalcs();

            return this;
        }

        public QSCalcsCtl InitializeCalcs()
        {
            return this.CalcsCtl = new QSCalcsCtl().CreateNew();
        }

        public QualityIssue Load(decimal issueID)
        {
            this.qualityIssue = new QualityIssue().Load(issueID);
            return this.qualityIssue;
        }

        public QualityIssue CreateNew(string sessionID, string context, decimal createPersonID)
        {
            this.qualityIssue = new QualityIssue().CreateNew(sessionID, context);
            if (this.qualityIssue != null)
                this.qualityIssue.Incident.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;
            return this.qualityIssue;
        }

        public void Clear()
        {
            this.qualityIssue = null;
        }

        public QualityIssue Update()
        {
            this.qualityIssue = QualityIssue.UpdateIssue(this.qualityIssue);
            return this.qualityIssue;
        }

        public PageUseMode UserPageMode(PERSON person, AccessMode accessMode)
        {
            return (this.PageMode = this.qualityIssue.UserPageMode(person, accessMode));
        }
    }

	[Serializable]
	public class QualityIssue
    {
        public enum UpdateStatus { Success, Pending, SaveError, RequiredInputs, Incomplete};

        public INCIDENT Incident
        {
            get;
            set;
        }
        public QI_OCCUR IssueOccur
        {
            get;
            set;
        }
        public BusinessLocation DetectedLocation
        {
            get;
            set;
        }
        public BusinessLocation ResponsibleLocation
        {
            get;
            set;
        }
        public PartData Partdata
        {
            get;
            set;
        }
        public QualityIssue.UpdateStatus Status
        {
            get;
            set;
        }
        public bool IsNew
        {
            get;
            set;
        }
        public bool StatusChanged
        {
            get;
            set;
        }
        public bool DetectedChanged
        {
            get;
            set;
        }
        public bool NotifyRequired
        {
            get;
            set;
        }
        public bool ResponsibleChanged
        {
            get;
            set;
        }
        public bool ResponseRequired
        {
            get;
            set;
        }
        public string CreateSessionID
        {
            get;
            set;
        }
        public List<ATTACHMENT> AttachmentsList
        {
            get;
            set;
        }
        public ResponseMgr TeamResponse
        {
            get;
            set;
        }
        public TaskStatusMgr TeamTask
        {
            get;
            set;
        }
        public string IssueID
        {
            get
            {
                return (WebSiteCommon.FormatID((int)this.IssueOccur.INCIDENT_ID, 6));
            }
            set
            {
                ;
            }
        }
        public SQM.Website.PSsqmEntities Entities
        {
            get;
            set;
        }

        public QualityIssue CreateNew(string sessionID, string qsActivity)
        {
            this.Incident = new INCIDENT();
            this.Initialize();
            this.IsNew = this.NotifyRequired = true;
            this.Incident.CREATE_DT = DateTime.UtcNow;
            this.CreateSessionID = sessionID;
            this.IssueOccur.QS_ACTIVITY = qsActivity;
            this.AddItem();
            this.AddSample();
            this.AddSampleMeasure(1);

            return this;
        }

        public QualityIssue Initialize()
        {
            this.Entities = new PSsqmEntities();
            this.Incident.INCIDENT_TYPE = "QI";
            this.Incident.DESCRIPTION = "";
            this.Incident.CREATE_PERSON = SessionManager.UserContext.Person.PERSON_ID;

            this.IssueOccur = new QI_OCCUR();
            this.IssueOccur.STATUS = "A";
            this.IssueOccur.SOURCE = "IN";
            this.Partdata = new PartData();
            this.ResponsibleLocation = new BusinessLocation();
            this.DetectedLocation = new BusinessLocation();
            this.DetectedLocation.Plant = new PLANT();
            this.IsNew = false;
            this.StatusChanged = this.DetectedChanged = this.ResponsibleChanged = this.ResponseRequired = this.NotifyRequired = false;
            //this.ResponseTeam = null;
            this.TeamTask = new TaskStatusMgr().Initialize(20, 0);
            this.AttachmentsList = new List<ATTACHMENT>();
            this.TeamResponse = new ResponseMgr().Initialize(this.Entities);

            return this;
        }

        public void ResetIndicators(bool setValue)
        {
            this.ResponseRequired = this.DetectedChanged = this.ResponsibleChanged = this.NotifyRequired = setValue;
        }

        public QualityIssue Load(decimal incidentID)
        {
            this.Incident = new INCIDENT();
            this.Initialize();

            if ((this.IssueOccur = LookupIssue(this.Entities, incidentID)) != null)
            {
                this.Incident = LookupIncident(this.Entities, this.IssueOccur.INCIDENT_ID);
                this.IsNew = false;
                this.DetectedLocation = new BusinessLocation().Initialize((decimal)this.Incident.DETECT_PLANT_ID);
               
                this.Partdata = SQMModelMgr.LookupPartData(this.Entities, SessionManager.SessionContext.PrimaryCompany.COMPANY_ID, (decimal)this.IssueOccur.PART_ID);
                this.Partdata.Locations();

                this.ResponsibleLocation = new BusinessLocation().Initialize((decimal)this.Incident.RESP_PLANT_ID);

                this.AttachmentsList = SQM.Website.Classes.SQMDocumentMgr.SelectAttachmentListByRecord(20, incidentID, "", "");

                this.TeamTask.LoadTaskList(20, this.Incident.INCIDENT_ID);

                this.TeamResponse.Load(20, this.IssueOccur.QIO_ID, "");

            }

            return this;
        }

        public BusinessLocation SetDetectedLocation(decimal plantID)
        {
            this.DetectedLocation = new BusinessLocation().Initialize(plantID);
            return this.DetectedLocation;
        }

        public BusinessLocation SetResponsibleLocation(decimal plantID)
        {
            this.ResponsibleLocation = new BusinessLocation().Initialize(plantID);
            return this.ResponsibleLocation;
        }

        public string SetLocationSource(BusinessLocation location)
        {
            string src = "";
            if (location != null && location.Company != null)
            {
                if (location.IsCustomerCompany(false))
                    src = "CS";
                if (location.IsSupplierCompany(false))
                    src = "SP";
                if (location.IsPrimaryCompany())
                    src = "IN";
            }

            return src;
        }

        public QualityIssue LoadAttachments()
        {
            this.AttachmentsList = new List<ATTACHMENT>();
            this.AttachmentsList = SQM.Website.Classes.SQMDocumentMgr.SelectAttachmentListByRecord(20, this.IssueOccur.INCIDENT_ID, "", "");
            return this;
        }

        public bool ShouldSendMail()
        {
            bool sendMail = false;

            if (this.StatusChanged || this.DetectedChanged || this.ResponsibleChanged  || this.ResponseRequired  ||  this.NotifyRequired)
                sendMail = true;

            return sendMail;
        }

        public int MailNotify(string mailURL)
        {
            int status = 0;
            string group = "20" + this.IssueOccur.QS_ACTIVITY;
            string partNum = "";

            try
            {
                List<SETTINGS> sets = SQMSettings.SelectSettingsGroup("", "TASK");
                PERSON createPerson = SQMModelMgr.LookupPerson((decimal)this.Incident.CREATE_PERSON, "");
                PART part = SQMModelMgr.LookupPart(this.Entities, this.IssueOccur.PART_ID, "", SessionManager.PrimaryCompany().COMPANY_ID, false);
            }
            catch
            {
                status = -1;
            }
              
            return status;
        }

        public QI_OCCUR_ITEM AddItem()
        {
            QI_OCCUR_ITEM item = new QI_OCCUR_ITEM();
            IssueOccur.QI_OCCUR_ITEM.Add(item);

            return item;
        }

        public QI_OCCUR_NC AddSample()
        {
            QI_OCCUR_NC sample = null;
            QI_OCCUR_ITEM item = IssueOccur.QI_OCCUR_ITEM.First();
            if (item != null)
            {
                sample = new QI_OCCUR_NC();
                sample.PROBLEM_COUNT = 0;
                sample.PROBLEM_AREA = "";
                sample.SAMPLE_NUM = item.QI_OCCUR_NC.Count + 1;
                item.QI_OCCUR_NC.Add(sample);
            }

            return sample;
        }

        public QI_OCCUR_MEASURE AddSampleMeasure(int sampleNumber)
        {
            QI_OCCUR_MEASURE measure = null;
            QI_OCCUR_NC sample = IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First();
            if (sample != null)
            {
                measure = new QI_OCCUR_MEASURE();
                measure.MEASURE_NUM = sample.QI_OCCUR_MEASURE.Count + 1;
                sample.QI_OCCUR_MEASURE.Add(measure);
            }

            return measure;
        }

        public PartData AddPartInfo(PartData part)
        {
            try
            {
                this.Partdata = part;
                this.IssueOccur.PART_ID = part.Part.PART_ID;
            }
            catch (Exception ex)
            {
                ;
            }

            return this.Partdata;
        }

        public void AddPlantInfo(decimal plantID, string plantName, decimal lineID, string lineName)
        {
            if (this.DetectedLocation.Plant == null)
                this.DetectedLocation.Plant = new PLANT();
            this.DetectedLocation.Plant.PLANT_ID = plantID;
            this.DetectedLocation.Plant.PLANT_NAME = plantName;
            this.Incident.DETECT_PLANT_ID = plantID;
            if (lineID > 0)
            {
                if (this.DetectedLocation.Plant.PLANT_LINE.Count == 0)
                    this.DetectedLocation.Plant.PLANT_LINE.Add(new PLANT_LINE());
                this.DetectedLocation.Plant.PLANT_LINE.First().PLANT_LINE_ID = lineID;
                this.DetectedLocation.Plant.PLANT_LINE.First().PLANT_LINE_NAME = lineName;
            }
        }

        public string FormatLabelContent(string labelSpec)
        {
            string strHtml = "";
            string strRaw, strSub, token;
            int pos1 = 0, pos2;
            object oValue = null;

            string fileSpec = HttpContext.Current.Server.MapPath("~/html/testlabel.txt");
            if (!WebSiteCommon.OpenReadFile(fileSpec, out strRaw))
                return ("--cannot open label file: " + fileSpec);

            while (pos1 > -1)
            {
                if ((pos1 = strRaw.IndexOf('{')) > -1 && (pos2 = strRaw.IndexOf('}')) > -1)
                {
                    strSub = strRaw.Substring(pos1, (pos2 - pos1) + 1);
                    token = strSub.Substring(1, strSub.Length - 2);
                    try
                    {
                        oValue = SQMModelMgr.GetObjectValue(this.IssueOccur, token);
                        if (oValue == null)
                            oValue = SQMModelMgr.GetObjectValue(this.Incident, token);
                        if (oValue == null)
                        {
                            QI_OCCUR_ITEM item = this.IssueOccur.QI_OCCUR_ITEM.First();
                            if ((oValue = SQMModelMgr.GetObjectValue(this.IssueOccur.QI_OCCUR_ITEM.First(), token)) == null)
                            {
                                oValue = SQMModelMgr.GetObjectValue(this.IssueOccur.QI_OCCUR_ITEM.First().QI_OCCUR_NC.First(), token);
                            }
                        }
                        if (oValue == null)
                            oValue = SQMModelMgr.GetObjectValue(this.Partdata.Part, token);
                        if (oValue == null)
                            oValue = SQMModelMgr.GetObjectValue(this.ResponsibleLocation.Plant, token);
                        if (oValue == null)
                            oValue = SQMModelMgr.GetObjectValue(this.DetectedLocation.Plant, token);
                        if (oValue == null)
                            oValue = SQMModelMgr.GetObjectValue(this, token);
                        if (oValue != null)
                        {
                            if (oValue.GetType().ToString().ToUpper().Contains("DATE"))
                                oValue = Convert.ToDateTime(oValue).ToShortDateString();
                            if (oValue.GetType().ToString().ToUpper().Contains("BOOL"))
                                if ((bool)oValue == true)
                                    oValue = WebSiteCommon.GetXlatValueLong("checked", oValue.ToString());

                            strRaw = strRaw.Replace(strSub, oValue.ToString());
                        }
                        else
                            strRaw = strRaw.Replace(strSub, "!" + token + "!");
                    }
                    catch
                    {
                        strRaw = strRaw.Replace(strSub, "!" + token + "!");
                    }
                }
            }

            strHtml = strRaw;


            return strHtml;
        }

        public static INCIDENT LookupIncident(SQM.Website.PSsqmEntities ctx, decimal incidentID)
        {
            INCIDENT incident = null;
            try
            {
                incident = (from i in ctx.INCIDENT.Include("QI_OCCUR") 
                                where (i.INCIDENT_ID == incidentID)
                                select i).Single();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return incident;
        }

        public static QI_OCCUR LookupIssue(SQM.Website.PSsqmEntities ctx, decimal incidentID)
        {
            QI_OCCUR qualityIssue = null;
            try
            {
                qualityIssue = (from o in ctx.QI_OCCUR.Include("QI_OCCUR_ITEM")
                                where (o.INCIDENT_ID == incidentID)
                                select o).Single();
                foreach (QI_OCCUR_ITEM item in qualityIssue.QI_OCCUR_ITEM)
                {
                    item.QI_OCCUR_NC.Load();
                    foreach (QI_OCCUR_NC sample in item.QI_OCCUR_NC)
                    {
                        sample.QI_OCCUR_MEASURE.Load();
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return qualityIssue;
        }

        public static QI_OCCUR FillOccur(QI_OCCUR qiOccur)
        {
            qiOccur.QI_OCCUR_ITEM.Load();
            foreach (QI_OCCUR_ITEM item in qiOccur.QI_OCCUR_ITEM)
            {
                item.QI_OCCUR_NC.Load();
                foreach (QI_OCCUR_NC sample in item.QI_OCCUR_NC)
                {
                    sample.QI_OCCUR_MEASURE.Load();
                }
            }

            return qiOccur;
        }

        public static int IssueCount(decimal companyID, DateTime fromDate, DateTime toDate)
        {
            int count = 0;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                count = (from o in entities.INCIDENT
                         where (o.DETECT_COMPANY_ID == companyID && o.INCIDENT_TYPE == "QI" && (o.CREATE_DT >= fromDate && o.CREATE_DT <= toDate))
                         select o).Count();
            }

            return count;
        }

        public static List<QualityIncidentData> SelectIncidentDataList(decimal[] plantIDS, DateTime fromDate, DateTime toDate,  string qsActivity, string searchString, bool actionRequired, bool selectAttachments)
        {
            return SelectIncidentDataList(plantIDS, fromDate, toDate, qsActivity, searchString, actionRequired, 0, selectAttachments);
        }

        public static List<QualityIncidentData> SelectIncidentDataList(decimal[] plantIDS, DateTime fromDate, DateTime toDate, string qsActivity, string searchString, bool actionRequired, decimal incidentID, bool selectAttachments)
        {
            List<QualityIncidentData> theList = new List<QualityIncidentData>();
            try
            {
                string[] activityIDS; 
                if (string.IsNullOrEmpty(qsActivity))
                    activityIDS = new string[3] {"RCV","PRQ","CST"};        // get all qs activity types if specic type not supplied
                else
                    activityIDS = new string[1] {qsActivity};

                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    if (incidentID > 0)
                    {
                        theList = (from i in entities.INCIDENT
                                   join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                   join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                   join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID
                                   join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID
                                   join u in entities.PERSON on i.CREATE_PERSON equals u.PERSON_ID 
                                   join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                   where (i.INCIDENT_ID == incidentID)
                                   from p in p_i.DefaultIfEmpty()
                                   select new QualityIncidentData
                                   {
                                       Incident = i,
                                       QIIssue = o,
                                       QIItem = e,
                                       Part = p,
                                       Plant = l,
                                       PlantResponsible = r,
                                       Person = u
                                   }).OrderByDescending(i => i.Incident.CREATE_DT).ToList();
                    }
                    else 
                    {
                        theList = (from i in entities.INCIDENT
                                    join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                    join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                    join s in entities.QI_OCCUR_NC on e.QIO_ITEM_ID equals s.QIO_ITEM_ID into s_e 
                                    join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID
                                    join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID
                                    join u in entities.PERSON on i.CREATE_PERSON equals u.PERSON_ID 
                                    join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                    where ((plantIDS.Contains((decimal)i.DETECT_PLANT_ID) || plantIDS.Contains((decimal)i.RESP_PLANT_ID)) && (i.CREATE_DT >= fromDate && i.CREATE_DT <= toDate)
                                    && i.INCIDENT_TYPE == "QI" && activityIDS.Contains(o.QS_ACTIVITY) && o.STATUS != "I")
                                    from s in s_e.DefaultIfEmpty()
                                    from p in p_i.DefaultIfEmpty()
                                    join nc in entities.NONCONFORMANCE on s.NONCONF_ID equals nc.NONCONF_ID into nc_s
                                    from nc in nc_s.DefaultIfEmpty()
                                    select new QualityIncidentData
                                    {
                                        Incident = i,
                                        QIIssue = o,
                                        QIItem = e,
                                        QISample = s,
                                        Part = p,
                                        Plant = l,
                                        PlantResponsible = r,
                                        Person = u,
                                        Nonconform = nc 
                                    }).OrderByDescending(i => i.Incident.CREATE_DT).ToList();
                    }

                    if (!string.IsNullOrEmpty(searchString))
                        theList = theList.ToList().FindAll(l => l.Incident.DESCRIPTION.ToUpper().Contains(searchString.ToUpper()) || l.QIIssue.OCCUR_DESC.ToUpper().Contains(searchString.ToUpper()));

                    // fetch any attachments associated with the selected issues ...
                    if (selectAttachments  &&  theList != null)
                    {
                        decimal[] ids = theList.Select(i => i.Incident.INCIDENT_ID).Distinct().ToArray();
                        List<ATTACHMENT> attachList = (from a in entities.ATTACHMENT
                                                       where
                                                       (a.RECORD_TYPE == 20 && ids.Contains(a.RECORD_ID)
                                                       && a.FILE_NAME.ToLower().Contains(".jpg") || a.FILE_NAME.ToLower().Contains(".jpeg") ||
                                                       a.FILE_NAME.ToLower().Contains(".gif") || a.FILE_NAME.ToLower().Contains(".png") ||
                                                       a.FILE_NAME.ToLower().Contains(".bmp"))
                                                       select a).OrderBy(l => l.ATTACHMENT_ID).ToList();
                        foreach (QualityIncidentData data in theList)
                        {
                            data.AttachList = new List<ATTACHMENT>();
                            data.AttachList.AddRange(attachList.Where(l => l.RECORD_ID == data.Incident.INCIDENT_ID).ToList());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return theList;
        }

        public static List<QualityIncidentData> SelectIncidentDataList(decimal personID, string qsActivity, bool actionRequired, int rowsToFetch)
        {
		// only get incidents authored by the person 
            List<QualityIncidentData> theList = new List<QualityIncidentData>();
            try
            {
                string[] activityIDS; 
                if (string.IsNullOrEmpty(qsActivity))
                    activityIDS = new string[3] {"RCV","PRQ","CST"};        // get all qs activity types if specic type not supplied
                else
                    activityIDS = new string[1] {qsActivity};

                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    theList = (from i in entities.INCIDENT
                                join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                join s in entities.QI_OCCUR_NC on e.QIO_ITEM_ID equals s.QIO_ITEM_ID into s_e 
                                join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID
                                join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID
                                join u in entities.PERSON on i.CREATE_PERSON equals u.PERSON_ID 
                                join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                where ((i.CREATE_PERSON == personID) 
                                    && i.INCIDENT_TYPE == "QI" && activityIDS.Contains(o.QS_ACTIVITY) && o.STATUS == "A")
                                from p in p_i.DefaultIfEmpty()
                                from s in s_e.DefaultIfEmpty()
                                join nc in entities.NONCONFORMANCE on s.NONCONF_ID equals nc.NONCONF_ID into nc_s
                                from nc in nc_s.DefaultIfEmpty()
                                select new QualityIncidentData
                                {
                                    Incident = i,
                                    QIIssue = o,
                                    QIItem = e,
                                    QISample = s,
                                    Part = p,
                                    Plant = l,
                                    PlantResponsible = r,
                                    Person = u,
                                    Nonconform = nc 
                                }).OrderByDescending(i=> i.Incident.CREATE_DT).Take(rowsToFetch).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return theList;
        }


        public static List<QualityIncidentData> FindProblemIncidents(decimal[] plantIDS, DateTime fromDate, DateTime toDate, string qsActivity, string partType, string partSearch, string eventCategory, string problemArea, decimal nonconfID)
        {
            List<QualityIncidentData> theList = new List<QualityIncidentData>();
            try
            {
                string[] activityIDS; 
                if (string.IsNullOrEmpty(qsActivity))
                    activityIDS = new string[3] {"RCV","PRQ","CST"};        // get all qs activity types if specic type not supplied
                else
                    activityIDS = new string[1] {qsActivity};

                string[] eventIDS;
                if (string.IsNullOrEmpty(eventCategory))
                    eventIDS = WebSiteCommon.GetXlatList("incidentSeverity", "", "short").Select(l => l.Key).ToArray();
                else
                    eventIDS = new string[1] { eventCategory };

                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    if (nonconfID > 0)
                    {
                        theList = (from i in entities.INCIDENT
                                   join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                   join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                   join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID
                                   join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID
                                   join u in entities.PERSON on i.CREATE_PERSON equals u.PERSON_ID
                                   join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                   join t in entities.QI_OCCUR_ITEM on o.QIO_ID equals t.QIO_ID into t_o
                                   from t in t_o.DefaultIfEmpty()
                                   join n in entities.QI_OCCUR_NC on t.QIO_ITEM_ID equals n.QIO_ITEM_ID into n_t
                                   from n in n_t.DefaultIfEmpty()
                                   where (i.INCIDENT_TYPE == "QI" &&
                                        (plantIDS.Contains((decimal)i.DETECT_PLANT_ID) || plantIDS.Contains((decimal)i.RESP_PLANT_ID))  &&
                                           (i.CREATE_DT >= fromDate && i.CREATE_DT <= toDate) && 
                                        activityIDS.Contains(o.QS_ACTIVITY) &&
                                   //     o.PART_TYPE == partType &&
                                        eventIDS.Contains(o.SEVERITY) &&
                                        n.NONCONF_ID == nonconfID)
                                   from p in p_i.DefaultIfEmpty()
                                   select new QualityIncidentData
                                   {
                                       Incident = i,
                                       QIIssue = o,
                                       QIItem = e,
                                       QISample = n,
                                       Part = p,
                                       Plant = l,
                                       PlantResponsible = r,
                                       Person = u
                                   }).OrderByDescending(i => i.Incident.CREATE_DT).ToList();
                    }
                    else
                    {
                        theList = (from i in entities.INCIDENT
                                   join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                   join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                   join l in entities.PLANT on i.DETECT_PLANT_ID equals l.PLANT_ID
                                   join r in entities.PLANT on i.RESP_PLANT_ID equals r.PLANT_ID
                                   join u in entities.PERSON on i.CREATE_PERSON equals u.PERSON_ID
                                   join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                   join t in entities.QI_OCCUR_ITEM on o.QIO_ID equals t.QIO_ID into t_o
                                   from t in t_o.DefaultIfEmpty()
                                   join n in entities.QI_OCCUR_NC on t.QIO_ITEM_ID equals n.QIO_ITEM_ID into n_t
                                   from n in n_t.DefaultIfEmpty()
                                   where (i.INCIDENT_TYPE == "QI" &&
                                        (plantIDS.Contains((decimal)i.DETECT_PLANT_ID)  ||  plantIDS.Contains((decimal)i.RESP_PLANT_ID))  && 
                                        (i.CREATE_DT >= fromDate && i.CREATE_DT <= toDate)  && 
                                        activityIDS.Contains(o.QS_ACTIVITY) &&
                                     //   o.PART_TYPE == partType &&
                                        eventIDS.Contains(o.SEVERITY))
                                       // && problemIDS.Contains(n.PROBLEM_AREA))
                                   from p in p_i.DefaultIfEmpty()
                                   select new QualityIncidentData
                                   {
                                       Incident = i,
                                       QIIssue = o,
                                       QIItem = e,
                                       QISample = n,
                                       Part = p,
                                       Plant = l,
                                       PlantResponsible = r,
                                       Person = u
                                   }).OrderByDescending(i => i.Incident.CREATE_DT).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(partType))
                {
                    // refine search per part type
                    theList = theList.Where(l => l.QIIssue.PART_TYPE == partType).ToList();
                }

                if (!string.IsNullOrEmpty(partSearch))
                {
                    // refine search per part num matching
                    theList = theList.Where(l => l.Part.PART_NUM.Contains(partSearch)).ToList();
                }

                if (!string.IsNullOrEmpty(problemArea))
                    theList = theList.Where(l => l.QISample != null  &&   l.QISample.PROBLEM_AREA == problemArea).ToList();

            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return theList;
        }

        public PageUseMode UserPageMode(PERSON person, AccessMode accessMode)
        {
            PageUseMode pageMode = PageUseMode.ViewOnly;

            if (accessMode >= AccessMode.Admin) // ||  person.PERSON_ID == this.Incident.CREATE_PERSON)
            {
                pageMode = PageUseMode.EditEnabled;
            }
            else if (accessMode == AccessMode.Plant || accessMode == AccessMode.Update)
            {
                if (SQMModelMgr.PersonPlantAccess(person, this.DetectedLocation.Plant.PLANT_ID) || SQMModelMgr.PersonPlantAccess(person, this.ResponsibleLocation.Plant.PLANT_ID))
                    pageMode = PageUseMode.EditEnabled;
            }
            else if (accessMode == AccessMode.View)
            {
                if (SQMModelMgr.PersonPlantAccess(person, this.DetectedLocation.Plant.PLANT_ID) || SQMModelMgr.PersonPlantAccess(person, this.ResponsibleLocation.Plant.PLANT_ID))
                    pageMode = PageUseMode.ViewOnly;
            }
            else if (accessMode == AccessMode.Partner)
            {
               // if (SQMModelMgr.PersonPlantAccess(person, this.DetectedLocation.Plant.PLANT_ID) || SQMModelMgr.PersonPlantAccess(person, this.ResponsibleLocation.Plant.PLANT_ID))
               //     pageMode = PageUseMode.EditPartial;
                if (this.IssueOccur.STATUS == "A")
                {
                    foreach (TASK_STATUS task in this.TeamTask.TaskList)
                    {
                        if (person.PERSON_ID == task.RESPONSIBLE_ID)
                            pageMode = PageUseMode.EditPartial;
                    }
                }
            }

            // allow authorized users to re-activate an issue if it is closed
            if (pageMode == PageUseMode.EditEnabled && this.IssueOccur.STATUS == "C")
                pageMode = PageUseMode.Active;

            return pageMode;
        }

        public void UpdateTasks(decimal[] responsibleIDS, string taskType, int responseTime, bool setTaskOpen)
        {
            foreach (decimal respID in responsibleIDS)
            {
                TASK_STATUS task = null;
                if ((task=this.TeamTask.FindTask("1", taskType, respID)) == null)
                {
                    this.TeamTask.CreateTask("1", taskType, 0, WebSiteCommon.GetXlatValueLong("taskType",taskType), DateTime.UtcNow.AddDays(responseTime), respID);
                    if (taskType == "C")
                        this.DetectedChanged = true;
                    else if (taskType == "R")
                        this.ResponsibleChanged = true;
                }
                else
                {
                    if (setTaskOpen)
                    {
                        this.TeamTask.SetTaskOpen(task, DateTime.UtcNow.AddDays(responseTime), null);
                    }
                }
            }

            // remove person response tasks if no longer assigned (checked in the list)
            foreach (TASK_STATUS task in this.TeamTask.TaskList.Where(l=> l.TASK_TYPE == taskType).ToList())
            {
                if (!responsibleIDS.Contains((decimal)task.RESPONSIBLE_ID))
                {
                     this.TeamTask.UpdateTaskStatus(task, TaskStatus.Delete);
                     if (taskType == "C")
                         this.DetectedChanged = true;
                     else if (taskType == "R")
                         this.ResponsibleChanged = true;
                }
                else 
                {
                     this.TeamTask.UpdateTaskStatus(task, TaskMgr.CalculateTaskStatus(task));
                }
            }
        }

        public void DeleteTasks(string taskType)
        {
            if (!string.IsNullOrEmpty(taskType))
            {
                foreach (TASK_STATUS task in this.TeamTask.TaskList.Where(l => l.TASK_TYPE == taskType).ToList())
                {
                    this.TeamTask.UpdateTaskStatus(task, TaskStatus.Delete);
                }
            }
        }

        public void SetTasksStatus(string taskType, TaskStatus newStatus, decimal personID)
        {
            foreach (TASK_STATUS task in this.TeamTask.TaskList.Where(l => l.TASK_TYPE == taskType).ToList())
            {
                if (newStatus == TaskStatus.Complete)
                {
                    this.TeamTask.SetTaskComplete(task, personID);
                }
                else if (newStatus == TaskStatus.AwaitingClosure)
                {
                    this.TeamTask.SetTaskComplete(task, personID);
                    task.STATUS = task.STATUS = Convert.ToInt32(newStatus).ToString();
                    task.DESCRIPTION = WebSiteCommon.GetXlatValueLong("taskType", task.STATUS);
                }
                else
                {
                    task.STATUS = Convert.ToInt32(newStatus).ToString();
                }
            }
        }

        public void SetTasksOpen(string taskType, DateTime newDueDate)
        {
            foreach (TASK_STATUS task in this.TeamTask.TaskList.Where(l => l.TASK_TYPE == taskType).ToList())
            {
                this.TeamTask.SetTaskOpen(task, newDueDate, null);
            }
        }

        public static QualityIssue UpdateIssue(QualityIssue theIssue)
        {
            QualityIssue retIssue = null;
            try
            {
                theIssue.IssueOccur = (QI_OCCUR)SQMModelMgr.SetObjectTimestamp((object)theIssue.IssueOccur, SessionManager.UserContext.UserName(), theIssue.IssueOccur.EntityState);
                if (theIssue.IssueOccur.EntityState == System.Data.EntityState.Added || theIssue.IssueOccur.EntityState == System.Data.EntityState.Detached)
                {
                    theIssue.Incident = (INCIDENT)SQMModelMgr.SetObjectTimestamp((object)theIssue.Incident, SessionManager.UserContext.UserName(), theIssue.Incident.EntityState);
                    theIssue.IssueOccur = (QI_OCCUR)SQMModelMgr.CopyObjectValues(theIssue.IssueOccur, theIssue.Incident, false);
                    theIssue.Incident.ISSUE_TYPE = theIssue.IssueOccur.OCCUR_DESC;
                    theIssue.Entities.AddToINCIDENT(theIssue.Incident);
                    theIssue.Entities.AddToQI_OCCUR(theIssue.IssueOccur);
                }

                theIssue.Incident.DETECT_COMPANY_ID = theIssue.DetectedLocation.Company.COMPANY_ID;
                theIssue.Incident.DETECT_BUS_ORG_ID = theIssue.DetectedLocation.BusinessOrg.BUS_ORG_ID;
                theIssue.Incident.DETECT_PLANT_ID = theIssue.DetectedLocation.Plant.PLANT_ID;

                theIssue.Incident.RESP_COMPANY_ID = theIssue.ResponsibleLocation.Company.COMPANY_ID;
                theIssue.Incident.RESP_BUS_ORG_ID = theIssue.ResponsibleLocation.BusinessOrg.BUS_ORG_ID;
                theIssue.Incident.RESP_PLANT_ID = theIssue.ResponsibleLocation.Plant.PLANT_ID;

                theIssue.Entities.SaveChanges();

                theIssue.TeamTask.UpdateTaskList(theIssue.Incident.INCIDENT_ID);

                theIssue.TeamResponse.UpdateResponses(theIssue.Incident.INCIDENT_ID);

                retIssue = theIssue;
                retIssue.IsNew = false;
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                retIssue = null;
            }

            return retIssue;
        }

  
        public static void CreateTasks(QualityIssue theIssue)
        {
            
        }
    }
    #endregion

    #region costreport
    public enum IncidentCostReportStatus { Normal, NoIncidents, InputError, OutOFRange };

	[Serializable]
	public class IncidentCostReport
    {
        public COST_REPORT CostReport
        {
            get;
            set;
        }
        public bool IsNew
        {
            get;
            set;
        }
        public string CreateSessionID
        {
            get;
            set;
        }
        public List<QualityIncidentData> IncidentList
        {
            get;
            set;
        }
        public PSsqmEntities Entities
        {
            get;
            set;
        }
        public string ReportID
        {
            get
            {
                return (WebSiteCommon.FormatID((int)this.CostReport.COST_REPORT_ID, 6));
            }
            set
            {
                ;
            }
        }

        public IncidentCostReport CreateNew(string sessionID, decimal companyID, decimal busOrgID)
        {
            this.Initialize();
            this.CreateSessionID = sessionID;
            this.CostReport.COMPANY_ID = companyID;
            if (busOrgID > 0)
                this.CostReport.BUS_ORG_ID = busOrgID;
            this.IsNew = true;

            return this;
        }

        public IncidentCostReport Initialize()
        {
            this.CostReport = new COST_REPORT();
            this.Entities = new PSsqmEntities();
            this.IncidentList = new List<QualityIncidentData>();
            this.CostReport.SUM_ACT_COST = this.CostReport.SUM_POT_COST = 0;
            return this;
        }

        public COST_REPORT Load(decimal reportID)
        {
            this.Initialize();

            try
            {
                this.CostReport = (from r in this.Entities.COST_REPORT.Include("COST_REPORT_ITEM")
                                where (r.COST_REPORT_ID == reportID)
                                select r).Single();
                foreach (COST_REPORT_ITEM item in this.CostReport.COST_REPORT_ITEM)
                {
                    QualityIncidentData incidentData = QualityIssue.SelectIncidentDataList(new decimal[0] { }, new DateTime(1900, 1, 1), DateTime.Now, "RCV", "", false, item.INCIDENT_ID, false).FirstOrDefault();
                    if (incidentData != null)
                    {
                        this.IncidentList.Add(incidentData);
                    }
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return this.CostReport;
        }

        public COST_REPORT_ITEM AddIncident(decimal incidentID)
        {
            COST_REPORT_ITEM item = new COST_REPORT_ITEM();
            item.UNIT_COST = 0m;
            item.NCM_ACT_QTY = item.NCM_ACT_COST = item.NCM_POT_QTY = item.NCM_POT_COST = 0;
            item.DWN_ACT = item.DWN_ACT_RATE = item.DWN_ACT_COST = item.DWN_POT = item.DWN_POT_RATE = item.DWN_POT_COST = 0;
            item.LABOR_ACT = item.LABOR_ACT_RATE = item.LABOR_ACT_COST = item.LABOR_POT = item.LABOR_POT_RATE = item.LABOR_POT_COST = 0;
            item.SHIP_ACT_TYPE = item.SHIP_POT_TYPE = "";
            item.SHIP_ACT_COST = item.SHIP_POT_COST = 0;
            item.NCM_COMMENTS = item.DWN_COMMENTS = item.LABOR_COMMENTS = item.SHIP_COMMENTS = item.TOTAL_COMMENTS = "";

            QualityIncidentData incidentData = QualityIssue.SelectIncidentDataList(new decimal[0] { }, new DateTime(1900, 1, 1), DateTime.Now, "RCV", "", false, incidentID, false).FirstOrDefault();
            if (incidentData != null)
            {
                item.INCIDENT_ID = incidentID;
                item.STATUS = "A";
                this.CostReport.COST_REPORT_ITEM.Add(item);
                this.IncidentList.Add(incidentData);
            }

            return item;
        }

        public static IncidentCostReport UpdateCostReport(IncidentCostReport theCostReport)
        {
            IncidentCostReport retCostReport = null;
            try
            {
                theCostReport.CostReport = (COST_REPORT)SQMModelMgr.SetObjectTimestamp((object)theCostReport.CostReport, SessionManager.UserContext.UserName(), theCostReport.CostReport.EntityState);
                if (theCostReport.CostReport.EntityState == System.Data.EntityState.Added || theCostReport.CostReport.EntityState == System.Data.EntityState.Detached)
                {
                    ;
                }

                foreach (COST_REPORT_ITEM item in theCostReport.CostReport.COST_REPORT_ITEM)
                {
                    if (item.EntityState == System.Data.EntityState.Detached  &&  item.STATUS != "D")
                    theCostReport.Entities.AddToCOST_REPORT_ITEM(item);
                }

                theCostReport.Entities.SaveChanges();

                if (theCostReport.IsNew)
                {
                    SQM.Website.Classes.SQMDocumentMgr.UpdateAttachmentRecordID(theCostReport.Entities, 22, theCostReport.CreateSessionID, theCostReport.CostReport.COST_REPORT_ID);
                }

                retCostReport = theCostReport;
                retCostReport.IsNew = false;
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
                retCostReport = null;
            }

            return retCostReport;
        }

        public static List<IncidentCostReport> SelectCostReportList(decimal companyID, decimal busOrgID)
        {
            List<IncidentCostReport> reportList = new List<IncidentCostReport>();

            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    
                  List<COST_REPORT> clist = (from r in entities.COST_REPORT.Include("COST_REPORT_ITEM") 
                                  where (r.COMPANY_ID == companyID)
                                    select r).ToList();
                    /*
                    reportList = (from c in entities.COST_REPORT.Include("COST_REPORT_ITEM") 
                                  where (c.COMPANY_ID == companyID)
                                  select new IncidentCostReport 
                                  {
                                      CostReport = c,
                                  }).ToList();
                    */
                    if (clist != null)
                    {
                        foreach (COST_REPORT cr in clist)
                        {
                            IncidentCostReport rpt = new IncidentCostReport();
                            rpt.CostReport = cr;
                            decimal[] ids = cr.COST_REPORT_ITEM.Select(l => l.INCIDENT_ID).ToArray();
                            rpt.IncidentList = (from i in entities.INCIDENT
                                                join o in entities.QI_OCCUR on i.INCIDENT_ID equals o.INCIDENT_ID
                                                join e in entities.QI_OCCUR_ITEM on o.QIO_ID equals e.QIO_ID
                                                join p in entities.PART on o.PART_ID equals p.PART_ID into p_i
                                                where (ids.Contains(i.INCIDENT_ID)) 
                                                from p in p_i.DefaultIfEmpty()
                                                select new QualityIncidentData
                                                {
                                                    Incident = i,
                                                    QIIssue = o,
                                                    QIItem = e,
                                                    Part = p
                                                }).ToList();
                            reportList.Add(rpt);
                        }
                    }
                }
            }
            catch (Exception e)
            {
 
            }

            return reportList;
        }

    }
    #endregion

}