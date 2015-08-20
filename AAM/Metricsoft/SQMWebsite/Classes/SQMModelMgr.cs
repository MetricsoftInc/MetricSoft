using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Reflection;
using SQM.Shared;
using System.Web.Configuration;

namespace SQM.Website
{
    #region helperclasses

    public class OrgData
    {
        public COMPANY Company
        {
            get;
            set;
        }
        public BUSINESS_ORG BusinessOrg
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public List<DEPARTMENT> DeptList
        {
            get;
            set;
        }
        public List<PLANT_LINE> LineList
        {
            get;
            set;
        }
        public List<PRODUCT_LINE> ProdLineList
        {
            get;
            set;
        }
        public List<LABOR_TYPE> LaborList
        {
            get;
            set;
        }
        public List<PERSON> PersonList
        {
            get;
            set;
        }
        public object EditObject
        {
            get;
            set;
        }
        public bool IsChanged
        {
            get;
            set;
        }

        public OrgData Initialize()
        {
            this.DeptList = new List<DEPARTMENT>();
            this.LaborList = new List<LABOR_TYPE>();
            this.PersonList = new List<PERSON>();
            this.IsChanged = false;

            return this;
        }
    }

    public class BusinessLocation
    {
        public COMPANY Company
        {
            get;
            set;
        }
        public BUSINESS_ORG BusinessOrg
        {
            get;
            set;
        }
        public PLANT Plant
        {
            get;
            set;
        }
        public ADDRESS Address
        {
            get;
            set;
        }
        public BusinessLocation Initialize(decimal plantID)
        {
            this.Company = null;
            this.BusinessOrg = null;
            this.Plant = null;

            if ((this.Plant = SQMModelMgr.LookupPlant(plantID)) != null)
            {
                this.Company = SQMModelMgr.LookupCompany((decimal)this.Plant.COMPANY_ID);
                this.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)this.Plant.BUS_ORG_ID);
            }
            return this;
        }
        public BusinessLocation Initialize(COMPANY company, BUSINESS_ORG busOrg, PLANT plant)
        {
            this.Company = company;
            this.BusinessOrg = busOrg;
            this.Plant = plant;
            return this;
        }
        public BusinessLocation Initialize(decimal companyID, decimal busOrgID, decimal plantID)
        {
            this.Company = null;
            this.BusinessOrg = null;
            this.Plant = null;
            this.Company = SQMModelMgr.LookupCompany(companyID);
            if (busOrgID > 0) {
                this.BusinessOrg = SQMModelMgr.LookupBusOrg(busOrgID);
                if (plantID > 0)
                    this.Plant = SQMModelMgr.LookupPlant(plantID);
            }
            return this;
        }
        public bool IsSupplierCompany(bool excludePrimary)
        {
            // is the user'S HR company a supplier ?
            if (excludePrimary)
                return this.Company.IS_SUPPLIER == true &&  !IsPrimaryCompany() ? true : false;
            else
                return this.Company.IS_SUPPLIER == true ? true : false;
        }
        public bool IsCustomerCompany(bool excludePrimary)
        {
            // is the user'S HR company a supplier ?
            if (excludePrimary)
                return this.Company.IS_CUSTOMER == true  &&  !IsPrimaryCompany() ? true : false;
            else
                return this.Company.IS_CUSTOMER == true ? true : false;
        }
        public bool IsPrimaryCompany()
        {
            // is the user'S HR company a supplier ?
            return this.Company.COMPANY_ID == SessionManager.SessionContext.PrimaryCompany.COMPANY_ID ? true : false;
        }
        public string FormatLocationListItem()
        {
            return (this.Plant.PLANT_NAME);
        }
        public string FormatExternalLocationListItem()
        {
            if (this.Company.COMPANY_NAME == this.Plant.PLANT_NAME)
                return (this.Plant.PLANT_NAME);
            else 
                return (this.Company.COMPANY_NAME + ", " + this.Plant.PLANT_NAME);
        }
    }

    [Serializable]
    public class PartData
    {
        public PART Part
        {
            get;
            set;
        }
        public PART_PROGRAM Program
        {
            get;
            set;
        }
        public STREAM Used
        {
            get;
            set;
        }
        public COMPANY PartnerCompany
        {
            get;
            set;
        }
        public PLANT CustomerPlant
        {
            get;
            set;
        }
        public PLANT SupplierPlant
        {
            get;
            set;
        }
        public decimal ListSeq
        {
            get;
            set; 
        }
        public bool IsNew
        {
            get;
            set;
        }
        public List<BusinessLocation> B2BList 
        {
            get;
            set;
        }
        public NOTIFY Notify
        {
            get;
            set;
        }
        public string PartDisplayNum(string partPerspective)
        {
            if (partPerspective == "CST"  &&  !string.IsNullOrEmpty(this.Part.DRAWING_REF))
                return this.Part.DRAWING_REF;
            else
                return this.Part.PART_NUM;
        }
        public PartData Load(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal partID, bool createNew)
        {
            this.ListSeq = 0;
            PartData data = SQMModelMgr.LookupPartData(ctx, companyID, partID);
            this.Part = data.Part;
            this.Program = data.Program;

            return this;
        }
        public List<BusinessLocation> Locations()
        {
            // get locations where part is used (customer) and supplied from (supplier)
            this.B2BList = new List<BusinessLocation>();

            BusinessLocation location = new BusinessLocation();
            location.Plant = SQMModelMgr.LookupPlant((decimal)this.Used.PLANT_ID);
            location.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)location.Plant.BUS_ORG_ID);
            location.Company = SQMModelMgr.LookupCompany((decimal)location.Plant.COMPANY_ID);
            this.B2BList.Add(location);

            if (this.Used.SUPP_PLANT_ID > 0)
            {
                location = new BusinessLocation();
                location.Plant = SQMModelMgr.LookupPlant((decimal)this.Used.SUPP_PLANT_ID);
                location.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)location.Plant.BUS_ORG_ID);
                location.Company = SQMModelMgr.LookupCompany((decimal)location.Plant.COMPANY_ID);
                this.B2BList.Add(location);
            }

            if (this.Used.CUST_PLANT_ID > 0)
            {
                location = new BusinessLocation();
                location.Plant = SQMModelMgr.LookupPlant((decimal)this.Used.CUST_PLANT_ID);
                location.BusinessOrg = SQMModelMgr.LookupBusOrg((decimal)location.Plant.BUS_ORG_ID);
                location.Company = SQMModelMgr.LookupCompany((decimal)location.Plant.COMPANY_ID);
                this.B2BList.Add(location);
            }

            return this.B2BList;
        }
    }

    public class ReceiptData
    {
        public RECEIPT Receipt
        {
            get;
            set;
        }
        public PART Part
        {
            get;
            set;
        }
        public PLANT CustomerPlant
        {
            get;
            set;
        }
        public PLANT SupplierPlant
        {
            get;
            set;
        }
        public List<QI_OCCUR> IssueList
        {
            get;
            set;
        }
    }
    #endregion


    public class SQMModelMgr
    {
        public static int updateStatus;
        public static Exception updateException;

        #region reflection
        public static object SetObjectValue(object oTarget, string name, string value)
        {
            if (oTarget == null)
                return oTarget;

            string nameUpper = name.ToUpper();
            //Type type = oTarget.GetType();
            var PropertyInfos = oTarget.GetType().GetProperties();

            foreach (System.Reflection.PropertyInfo pInfo in PropertyInfos)
            {
                if (pInfo.Name.ToUpper() == nameUpper)
                {
                    string ss = pInfo.PropertyType.ToString();
                    try
                    {
                        if (pInfo.PropertyType.ToString().Contains("String"))
                            pInfo.SetValue(oTarget, value, null);
                        if (pInfo.PropertyType.ToString().Contains("Bool"))
                            pInfo.SetValue(oTarget, Convert.ToBoolean(value), null);
                        if (pInfo.PropertyType.ToString().Contains("Decimal"))
                            pInfo.SetValue(oTarget, Convert.ToDecimal(value), null);
                        if (pInfo.PropertyType.ToString().Contains("Float"))
                            pInfo.SetValue(oTarget, float.Parse(value), null);
                        if (pInfo.PropertyType.ToString().Contains("Double"))
                            pInfo.SetValue(oTarget, Convert.ToDouble(value), null);
                        if (pInfo.PropertyType.ToString().Contains("Date"))
                            pInfo.SetValue(oTarget, Convert.ToDateTime(value), null);
                    }
                    catch
                    {
                        ;
                    }
                    break;
                }
            }

            return oTarget;
        }

        public static object GetObjectValue(object oTarget, string name)
        {
            if (oTarget == null)
                return oTarget;

            object ret = null;

            string nameUpper = name.ToUpper();
            //Type type = oTarget.GetType();
            var PropertyInfos = oTarget.GetType().GetProperties();

            foreach (System.Reflection.PropertyInfo pInfo in PropertyInfos)
            {
                if (pInfo.Name.ToUpper() == nameUpper)
                {
                    string ss = pInfo.PropertyType.ToString();
                    try
                    {
                        ret = pInfo.GetValue(oTarget, null);
                    }
                    catch
                    {
                        ;
                    }
                    break;
                }
            }

            return ret;
        }

        public static object CopyObjectValues(object oTarget, object oSource, bool unused)
        {
            if (oTarget == null)
                return oTarget;

            Type targetType = oTarget.GetType();
            var SourcePropertyInfos = oSource.GetType().GetProperties();
            var TargetPropertyInfos = oTarget.GetType().GetProperties();
            string name = "";

            try
            {
                foreach (System.Reflection.PropertyInfo pSource in SourcePropertyInfos)
                {
                    name = pSource.Name;
                    System.Reflection.PropertyInfo pTarget = targetType.GetProperty(name);
                    if (pTarget != null)
                        pTarget.SetValue(oTarget, pSource.GetValue(oSource, null), null);
                }
            }
            catch 
            {
                ;
            }

            return oTarget;
        }

        public static object SetObjectTimestamp(object oTarget, string updBy, System.Data.EntityState state)
        {
            // set the record create and update time stamps
            // naming conventions appear to be standardized accross all SQM tables
            if (oTarget == null)
                return oTarget;
            
            DateTime utcDate = DateTime.UtcNow;

            Type type = oTarget.GetType();

            if (state != System.Data.EntityState.Unchanged)
            {
                System.Reflection.PropertyInfo pInfo = type.GetProperty("LAST_UPD_DT");
                if (pInfo != null)
                    pInfo.SetValue(oTarget, utcDate, null);
                pInfo = type.GetProperty("LAST_UPD_BY");
                if (pInfo != null)
                    pInfo.SetValue(oTarget, updBy, null);
            }

            if (state == System.Data.EntityState.Detached)
            {
                System.Reflection.PropertyInfo pInfo = type.GetProperty("CREATE_DT");
                if (pInfo != null)
                    pInfo.SetValue(oTarget, utcDate, null);
                pInfo = type.GetProperty("CREATE_BY");
                if (pInfo != null)
                    pInfo.SetValue(oTarget, updBy, null);
            }

            return oTarget;
        }
        #endregion  

        #region session

        public static LOCAL_LANGUAGE LookupLanguage(SQM.Website.PSsqmEntities ctx, string langCode, int langID, bool createNew)
        {
            LOCAL_LANGUAGE lang = null;

            try
            {
                if (langID > 0)
                    lang = (from l in ctx.LOCAL_LANGUAGE 
                             where (l.LANGUAGE_ID == langID)
                            select l).Single();
                else
                    lang = (from l in ctx.LOCAL_LANGUAGE 
                             where (l.LANGUAGE_CD == langCode)
                            select l).Single();
            }
            catch
            {
                if (createNew  &&  (langID <= 0  &&  !string.IsNullOrEmpty(langCode)))
                {
                    lang = new LOCAL_LANGUAGE();
                    int newID = (from l in ctx.LOCAL_LANGUAGE select (l.LANGUAGE_ID)).Max();
                    lang.LANGUAGE_ID = newID + 1;
                    lang.LANGUAGE_CD = langCode;
                }
            }
            
            return lang;
        }

        public static List<LOCAL_LANGUAGE> SelectLanguageList(SQM.Website.PSsqmEntities ctx, bool activeOnly)
        {
            List<LOCAL_LANGUAGE> langList = null;
            try
            {
                if (activeOnly)
                    langList = (from l in ctx.LOCAL_LANGUAGE
                                where (l.STATUS == "A")
                                select l).ToList();
                else
                    langList = (from l in ctx.LOCAL_LANGUAGE
                                select l).ToList();
            }
            catch
            {
                ;
            }

             return langList;
        }

        #endregion

        #region person

		public static List<JOBCODE> SelectJobcodeList(string status, string privGroup)
		{
			List<JOBCODE> jobcodeList = new List<JOBCODE>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				jobcodeList = (from j in ctx.JOBCODE
							   where (
								(string.IsNullOrEmpty(status) || j.STATUS == status)
								&& (string.IsNullOrEmpty(privGroup)  ||  !string.IsNullOrEmpty(j.PRIV_GROUP))
							   )
							   select j).ToList();
			}

			return jobcodeList;
		}

		public static JOBCODE LookupJobcode(PSsqmEntities ctx, string jobcode)
		{
			return (from j in ctx.JOBCODE where (j.JOBCODE_CD == jobcode) select j).SingleOrDefault();
		}

		public static List<PERSON> SelectPrivGroupPersonList(SysPriv priv, SysScope scope, decimal plantID)
		{
			List<PERSON> personList = new List<PERSON>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				string privScope = scope.ToString();
				personList = (from p in ctx.PERSON
							  join j in ctx.JOBCODE on p.JOBCODE_CD equals j.JOBCODE_CD 
							  join v in ctx.PRIVGROUP on j.PRIV_GROUP equals v.PRIV_GROUP
							  where (j.JOBCODE_CD == p.JOBCODE_CD && v.PRIV == (int)priv && v.SCOPE == privScope  &&  (plantID == 0  || p.PLANT_ID == plantID))
							  select p).ToList();
			}

			return personList;
		}
		public static List<PERSON> SelectPrivGroupPersonList(SysPriv[] privList, SysScope scope, decimal[] plantIDList)
		{
			List<PERSON> personList = new List<PERSON>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				string privScope = scope.ToString();
				int[] privs = Array.ConvertAll(privList, value => (int)value);
				personList = (from p in ctx.PERSON
							  join j in ctx.JOBCODE on p.JOBCODE_CD equals j.JOBCODE_CD
							  join v in ctx.PRIVGROUP on j.PRIV_GROUP equals v.PRIV_GROUP
							  where (j.JOBCODE_CD == p.JOBCODE_CD && privs.Contains(v.PRIV) && v.SCOPE == privScope  &&  (plantIDList.Count() == 0  ||  plantIDList.Contains(p.PLANT_ID)))
							  select p).ToList();
			}

			return personList;
		}

		public static List<SQM_ACCESS> SearchUserList(string[] statusList)
		{
			List<SQM_ACCESS> userList = new List<SQM_ACCESS>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				userList = (from u in ctx.SQM_ACCESS where (statusList.Count() == 0 ||  statusList.Contains(u.STATUS)) select u).ToList();
			}

			return userList;
		}

        public static SQM_ACCESS LookupCredentials(SQM.Website.PSsqmEntities ctx, string SSOID, string pwd, bool activeOnly)
        {
            SQM_ACCESS access = null;

            try
            {
                if (activeOnly)
                    access = (from A in ctx.SQM_ACCESS
							  where (A.SSO_ID.ToUpper() == SSOID.ToUpper() && (A.STATUS == "A" || A.STATUS == "P"))
                            select A).Single();
                else
                    access = (from A in ctx.SQM_ACCESS
                              where (A.SSO_ID.ToUpper() == SSOID.ToUpper())
                              select A).Single();
            }
            catch (Exception ex)
            {
                ;
            }
            return access;
        }

		public static SQM_ACCESS LookupCredentialsByEmail(SQM.Website.PSsqmEntities ctx, string email, bool activeOnly)
		{
			SQM_ACCESS access = null;

			try
			{
				if (activeOnly)
					access = (from A in ctx.SQM_ACCESS
							  where (A.RECOVERY_EMAIL.ToLower() == email.ToLower() && (A.STATUS == "A" || A.STATUS == "P"))
							  select A).Single();
				else
					access = (from A in ctx.SQM_ACCESS
							  where (A.RECOVERY_EMAIL.ToLower() == email.ToLower())
							  select A).Single();
			}
			catch
			{
			}
			return access;
		}

        public static PERSON LookupPerson(decimal personID, string SSOID)
        {
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                return (LookupPerson(entities, personID, SSOID, false));
            }
        }

        public static PERSON LookupPerson(SQM.Website.PSsqmEntities ctx, decimal personID, string SSOID, bool createNew)
        {
            PERSON person = null;

            try
            {
                if (personID == 0)
					person = (from P in ctx.PERSON.Include("PERSON_ACCESS").Include("PERSON_RESP").Include("JOBCODE")
                                where (P.SSO_ID.ToUpper() == SSOID.ToUpper())
                                select P).Single();
                else
					person = (from P in ctx.PERSON.Include("PERSON_ACCESS").Include("PERSON_RESP").Include("JOBCODE")
                                where (P.PERSON_ID == personID)
                                select P).Single();

                if (person.PERSON_RESP == null)
                    person = AddPersonResp(person);

            }
            catch
            {
                if (createNew)
                {
                    person = NewPerson(SSOID, 0);
                }
            }

            return person;
        }

		public static PERSON LookupPersonByEmail(SQM.Website.PSsqmEntities ctx, string email)
		{
			PERSON person = null;

			try
			{
					person = (from P in ctx.PERSON
							  where (P.EMAIL == email)
							  select P).Single();
			}
			catch
			{
			}

			return person;
		}

		public static PERSON LookupPersonByEmpID(SQM.Website.PSsqmEntities ctx, string empID)
		{
			PERSON person = null;

			try
			{
				person = (from P in ctx.PERSON
						  where (P.EMP_ID == empID)
						  select P).Single();
			}
			catch
			{
			}

			return person;
		}

		public static List<PRIVGROUP> SelectPrivGroup(string privGroup)
		{
			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				return (from v in ctx.PRIVGROUP where (v.PRIV_GROUP == privGroup) select v).ToList();
			}
		}
		public static List<PRIVGROUP> SelectPrivGroupJobcode(string jobCode, string commonGroup)
		{
			List<PRIVGROUP> privList = new List<PRIVGROUP>();

			using (PSsqmEntities ctx = new PSsqmEntities())
			{
				try
				{
					privList = (from v in ctx.PRIVGROUP
								join j in ctx.JOBCODE on v.PRIV_GROUP equals j.PRIV_GROUP
								where (j.JOBCODE_CD == jobCode)
								select v).ToList();
					// append common privs ...
					if (!string.IsNullOrEmpty(commonGroup))
					{
						privList.AddRange((from v in ctx.PRIVGROUP
										   where (v.PRIV_GROUP == commonGroup)
										   select v).ToList());
					}
				}
				catch { }
			}

			return privList;
		}

		public static PERSON NewPerson(string SSOID, decimal companyID)
        {
            PERSON person = new PERSON();
            person.SSO_ID = SSOID;
            person.COMPANY_ID = companyID;
            person.ROLE = 300;
            person.PREFERRED_TIMEZONE = WebSiteCommon.GetXlatValue("timeZone", "035");
            person = (PERSON)SQMModelMgr.SetObjectTimestamp((object)person, "", person.EntityState);
            person = AddPersonResp(person);

            return person;
        }

        public static int PersonCount(decimal companyID)
        {
            int count = 0;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                count = (from P in entities.PERSON where (P.COMPANY_ID == companyID &&  P.ROLE > 1) select P).Count();
            }

            return count;
        }

        public static string FormatPersonListItem(PERSON person)
        {
            if (person != null)
                return (person.LAST_NAME + ", " + person.FIRST_NAME);
            else
                return "";
        }

        public static bool CheckProductModuleAccess(PERSON person, string module)
        {
            bool canAccess = false;

            if (person.ROLE <= 100  || (person.PERSON_ACCESS != null  &&  person.PERSON_ACCESS.Where(a => a.ACCESS_PROD == module).Count() > 0))
                canAccess = true;

            return canAccess;
        }

        public static bool CheckModuleTopicAccess(PERSON person, string topic)
        {
            bool canAccess = false;
           // string baseTopic = string.IsNullOrEmpty(topic) ? "" : topic.Substring(0, 1) + "01";  // do this for backwards compatibility with single product access settings

            if (person.ROLE <= 100)
            {
                canAccess = true;
            }
            else 
            {
                string[] args = topic.Split(',');
                foreach (string ac in args)
                {
                    string baseTopic = string.IsNullOrEmpty(ac) ? "" : ac.Substring(0, 1) + "01";  // do this for backwards compatibility with single product access settings
                    if (person.PERSON_ACCESS != null && (person.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == ac).Count() > 0 || person.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == baseTopic).Count() > 0))
                        canAccess = true;
                }
            }

            return canAccess;
        }

        public static bool CanDelegate(PERSON fromPerson, PERSON toPerson)
        {
            bool canDelegate = false;

            if (fromPerson.PERSON_ID  != toPerson.PERSON_ID  &&  (toPerson.ROLE > 1 && toPerson.ROLE <= 300))
            {
                canDelegate = true;
                PERSON_ACCESS toAccess = null;
                foreach (PERSON_ACCESS fromAccess in fromPerson.PERSON_ACCESS.Where(a=> Convert.ToInt32(a.ACCESS_TOPIC) > 200))
                {
                    if ((toAccess = toPerson.PERSON_ACCESS.Where(a => a.ACCESS_TOPIC == fromAccess.ACCESS_TOPIC && a.ACCESS_LEVEL > 0).FirstOrDefault()) == null)
                    {
                        canDelegate = false;
                        break;
                    }
                }
            }

            return canDelegate;
        }

        public static List<PERSON> SelectPersonList(decimal companyID, decimal busOrgID, bool activeOnly, bool rcvEscalationOnly)
        {
            List<PERSON> personList = null;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                if (companyID > 0)
					personList = (from P in entities.PERSON.Include("PERSON_ACCESS").Include("Person_Resp").Include("JOBCODE")
                              where (P.COMPANY_ID == companyID  &&  P.ROLE > 1)
                              select P).ToList();
                else if (busOrgID > 0)
					personList = (from P in entities.PERSON.Include("PERSON_ACCESS").Include("Person_Resp").Include("JOBCODE")
                                  where (P.BUS_ORG_ID == busOrgID && P.ROLE > 1)
                              select P).ToList();

                if (activeOnly)
                    personList = personList.Where(l => l.STATUS == "A").ToList();
                if (rcvEscalationOnly)
                    personList = personList.Where(l => l.RCV_ESCALATION == true).ToList();
            }
            return personList;
        }

        public static List<PERSON> SelectBusOrgPersonList(decimal companyID, decimal busOrgID)
        {
            List<PERSON> personList = new List<PERSON>();

            foreach (PERSON person in SelectPersonList(companyID, 0, true, false))
            {
                if (person.ROLE <= 100 ||  person.BUS_ORG_ID == busOrgID)
                    personList.Add(person);
            }

            return personList;
        }

        public static List<PERSON> FilterPersonListByAppContext(List<PERSON> personList, string appContext)
        {
            List<PERSON> theList = new List<PERSON>();

            foreach (PERSON person in personList)
            {
                if (string.IsNullOrEmpty(appContext) || SQMModelMgr.CheckProductModuleAccess(person, appContext))
                    theList.Add(person);
            }

            return theList;
        }

        public static List<PERSON> SelectPlantPersonList(decimal companyID, decimal plantID, string appContext)
        {
            List<PERSON> personList = new List<PERSON>();

            foreach (PERSON person in SelectPersonList(companyID, 0, true, false))
            {
                if (PersonPlantAccess(person, plantID)  &&  (string.IsNullOrEmpty(appContext) || SQMModelMgr.CheckProductModuleAccess(person, appContext)))
                    personList.Add(person);
            }

            return personList;
        }

        // get persons across mutliple trading partner companies and/or plants
        public static List<PERSON> SelectPlantPersonList(List<BusinessLocation> locationList, string appContext, AccessMode minRoleAccess)
        {
            List<PERSON> personList = new List<PERSON>();
            decimal companyID = 0;
            List<PERSON> companyPersonList = new List<PERSON>();

            foreach (BusinessLocation location in locationList)
            {
                if (location.Company.COMPANY_ID != companyID)
                {
                    companyID = location.Company.COMPANY_ID;
                    companyPersonList = SelectPersonList(location.Company.COMPANY_ID, 0, true, false);
                }
                foreach (PERSON person in companyPersonList)
                {
                    if (PersonPlantAccess(person, location.Plant.PLANT_ID) && (string.IsNullOrEmpty(appContext) || SQMModelMgr.CheckModuleTopicAccess(person, appContext)))
                    {
                        if (minRoleAccess == null || person.ROLE <= SessionManager.AccessModeRoleXREF(minRoleAccess))
                        {
                            if (!personList.Contains(person))
                                personList.Add(person);
                        }
                    }
                }
            }

            return personList.Distinct().ToList();
        }

        public static bool PersonPlantAccess(PERSON person, decimal plantID)
        {
            bool canAccess = person.ROLE <= 100 ||  person.PLANT_ID == plantID ? true : false;    // company admin and person's hr location have access automatically

            if (!canAccess &&  !string.IsNullOrEmpty(person.NEW_LOCATION_CD))
            {
                string[] locs = person.NEW_LOCATION_CD.Split(',');
                foreach (string locid in locs)
                {
                    if (locid.Trim() == plantID.ToString().Trim())
                    {
                        canAccess = true;
                        break;
                    }
                }
            }

            return canAccess;
        }

        public static PERSON CreatePerson(SQM.Website.PSsqmEntities ctx, decimal companyID)
        {
            PERSON person = new PERSON();
            person.COMPANY_ID = companyID;
            person.ROLE = 300;
            person = (PERSON)SQMModelMgr.SetObjectTimestamp((object)person, "", person.EntityState);
            person = AddPersonResp(person);
            ctx.PERSON.AddObject(person);

            return person;
        }

        public static List<PERSON> SearchPersonList(SQM.Website.PSsqmEntities ctx, decimal companyID, string searchCriteria, bool activeOnly)
        {
            List<PERSON> personList = null;

            if (string.IsNullOrEmpty(searchCriteria) || searchCriteria == "%")
            {
                if (activeOnly)
					personList = (from p in ctx.PERSON.Include("Person_Access").Include("Person_Resp").Include("JOBCODE")
                                  where (p.COMPANY_ID == companyID &&  p.ROLE > 1 && p.STATUS == "A")
                               select p).ToList();
                else
					personList = (from p in ctx.PERSON.Include("Person_Access").Include("Person_Resp").Include("JOBCODE")
                                  where (p.COMPANY_ID == companyID  &&  p.ROLE > 1)
                               select p).ToList();
            }
            else
            {
                if (activeOnly)
					personList = (from p in ctx.PERSON.Include("Person_Access").Include("Person_Resp").Include("JOBCODE")
                                  where (p.COMPANY_ID == companyID  &&  p.ROLE > 1) && (p.STATUS == "A") && ((p.SSO_ID.ToUpper().Contains(searchCriteria.ToUpper())) || (p.LAST_NAME.ToUpper().Contains(searchCriteria.ToUpper())))
                               select p).ToList();
                else
					personList = (from p in ctx.PERSON.Include("Person_Access").Include("Person_Resp").Include("JOBCODE")
                                  where (p.COMPANY_ID == companyID &&  p.ROLE > 1) && ((p.SSO_ID.ToUpper().Contains(searchCriteria.ToUpper())) || (p.LAST_NAME.ToUpper().Contains(searchCriteria.ToUpper())))
                               select p).ToList();
            }

            return personList;
        }

        public static PERSON UpdatePerson(SQM.Website.PSsqmEntities ctx, PERSON person, string updateBy)
        {
            PERSON updatedPerson = null;
            person = (PERSON)SQMModelMgr.SetObjectTimestamp((object)person, updateBy, person.EntityState);

            if (person.EntityState == EntityState.Detached ||  person.EntityState == EntityState.Added)
            {
                SQM_ACCESS access = LookupCredentials(ctx, person.SSO_ID, "", false);
                if (access == null)     // user access not defined (ie. new user)
                {
                    access = new SQM_ACCESS();
                    access.SSO_ID = person.SSO_ID;
					// access.PASSWORD = person.SSO_ID;    // temporary !! need to set up random password
					string key = GetPasswordKey();
					access.PASSWORD = WebSiteCommon.Encrypt(WebSiteCommon.GeneratePassword(8, 8), key);
                    access.RECOVERY_EMAIL = person.EMAIL;
					// AW 201310 - we want to update the access status when updating the person status, but not if they are being forced to update password
					if (person.STATUS == "I")
						access.STATUS = person.STATUS;
					else
						access.STATUS = "P"; // force password update on login
                    ctx.SQM_ACCESS.AddObject(access);
                }
            }

            if (ctx.SaveChanges() > 0)
            {
                updatedPerson = person;
            }

            return updatedPerson;
        }

        public static PERSON UpdatePerson(SQM.Website.PSsqmEntities ctx, PERSON person, string updateBy, bool isBuyer, string currentSSOID, string defaultPwd)
        {
            person = (PERSON)SQMModelMgr.SetObjectTimestamp((object)person, updateBy, person.EntityState);

			if (person.EntityState == EntityState.Detached  || person.EntityState == EntityState.Added)
			{
				if (!person.PREFERRED_LANG_ID.HasValue)
					person.PREFERRED_LANG_ID = 1;
				if (string.IsNullOrEmpty(person.PREFERRED_TIMEZONE))
					person.PREFERRED_TIMEZONE = "035";
				if (string.IsNullOrEmpty(person.STATUS))
					person.STATUS = "A";
				ctx.AddToPERSON(person);
			}

            SQM_ACCESS access = null;
			string key = "";

            // todo check if person SSOID has changed 
            access = LookupCredentials(ctx, person.SSO_ID, "", false);

            if (access == null)     // user access not defined (ie. new user)
            {
                access = new SQM_ACCESS();
                access.SSO_ID = person.SSO_ID;
				key = GetPasswordKey();
				if (string.IsNullOrEmpty(defaultPwd))
				{
					access.PASSWORD = WebSiteCommon.Encrypt(WebSiteCommon.GeneratePassword(8, 8), key);
					access.STATUS = "P"; // force password update on login
				}
				else 
				{
					access.PASSWORD = WebSiteCommon.Encrypt(defaultPwd, key);
					access.STATUS = "A"; // assume user is enabled
				}

                access.RECOVERY_EMAIL = person.EMAIL;

                ctx.SQM_ACCESS.AddObject(access);
            }
            else
            {
                access.SSO_ID = person.SSO_ID;
                access.RECOVERY_EMAIL = person.EMAIL;
                // AW 201310 - we want to update the access status when updating the person status, but not if they are being forced to update password
                if (access.STATUS != "P" || person.STATUS == "I")
                    access.STATUS = person.STATUS;
            }
            try
            {

                updateStatus = ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return person;
        }

        public static PERSON AddPersonResp(PERSON person)
        {
            if (person.PERSON_RESP == null)
            {
                person.PERSON_RESP = new PERSON_RESP();
                // enable delegate assignment by default for any new user
                person = AddPersonAccess(person, "CQM","92", true);
            }

            return person;
        }

        public static PERSON AddPersonAccess(PERSON person, string prod, string topic, bool canView)
        {
            PERSON_ACCESS access = new PERSON_ACCESS();

            access.PERSON_ID = person.PERSON_ID;
            access.ACCESS_PROD = prod;
            access.ACCESS_TOPIC = topic;
            access.ACCESS_MODULE = "";
            if (canView)
                access.ACCESS_LEVEL = 1;
            access.ACCESS = person.PERSON_ACCESS.Count + 1;
            person.PERSON_ACCESS.Add(access);

            return person;
        }

		public static SQM_ACCESS UpdateCredentials(SQM.Website.PSsqmEntities ctx, SQM_ACCESS access, string updateBy, out int status)
        {
            access = (SQM_ACCESS)SQMModelMgr.SetObjectTimestamp((object)access, updateBy, access.EntityState);
			status = ctx.SaveChanges();
            return access;
        }

		public static string GetPasswordKey()
		{
			string pwdKey = "";
			List<SETTINGS> settings = SQMSettings.SelectSettingsGroup("ENCRYPT", "ENCRYPT");
			if (settings != null)
			{
				pwdKey = settings[0].VALUE;
			}
			return pwdKey;
		}

		public static int ChangeUserPassword(string SSOID, string userName, string curPassword, string strPassword)
		{
			string newPassword = strPassword.Trim();

			// encrypt the new password for storage and query the useractivity table if/when it had been used in the past
			string KeyString = GetPasswordKey();
			string password = WebSiteCommon.Encrypt(newPassword, KeyString);
			
			SQM.Website.PSsqmEntities ctx = new PSsqmEntities();
			SQM_ACCESS access = SQMModelMgr.LookupCredentials(ctx, SSOID, curPassword, true);

			// AW - for now, we want to allow if the password = the password OR the encrypted password
			if ((WebSiteCommon.Encrypt(curPassword, KeyString) != access.PASSWORD) && (curPassword != access.PASSWORD))
				return 10;

			int passComplexity = checkPasswordComplexity(newPassword);
			if (passComplexity > 0)
				return passComplexity;

			access.PASSWORD = password;
			if (access.STATUS == "P")
				access.STATUS = "A";
			int ctxstatus = 0;
			SQM_ACCESS newaccess = SQMModelMgr.UpdateCredentials(ctx, access, userName, out ctxstatus);

			if (ctxstatus != 1)
				return ctxstatus;
			else
			{
				SessionManager.UserContext.Credentials = newaccess;
				return 0;
			}
		}

		public static int checkPasswordComplexity(string password)
		{
			int passError = 0;

			string strength =  SQMSettings.SelectSettingByCode(new PSsqmEntities(), "COMPANY", "TASK", "PasswordComplexity").VALUE; // WebConfigurationManager.AppSettings["PasswordComplexity"];

			int specialCount = System.Text.RegularExpressions.Regex.Matches(password, @"\W").Count;
			int numericCount = System.Text.RegularExpressions.Regex.Matches(password, @"\d").Count;
			int upperCount = System.Text.RegularExpressions.Regex.Matches(password, @"[A-Z]").Count;
			int lowerCount = System.Text.RegularExpressions.Regex.Matches(password, @"[a-z]").Count;
			switch (strength)
			{
				case "0": // weak
					if (password.Length < 3)
						passError = 100;
					break;
				case "1": // simple
					if (password.Length < 6 || (upperCount + lowerCount) < 1 || (numericCount + specialCount) < 1)
						passError = 110;
					break;
				case "2": // moderate
					if (password.Length < 6 || lowerCount < 1 || numericCount < 1 || (upperCount + specialCount) < 1)
						passError = 120;
					break;
				case "3": // strong
					if (password.Length < 8 || lowerCount < 1 || upperCount < 1 || numericCount < 1 || specialCount < 1)
						passError = 130;
					break;
				case "4": // extreme
					if (password.Length < 8 || lowerCount < 2 || upperCount < 2 || numericCount < 2 || specialCount < 2)
						passError = 140;
					break;
				default: // simple
					if (password.Length < 6 || (upperCount + lowerCount) < 1 || (numericCount + specialCount) < 1) 
						passError = 110;
					break;
			}
			return passError;
		}

        public static List<PERSON> SelectDelegateList(PSsqmEntities ctx, decimal personID)
        {
            // get list of persons for which the supplied personID is a delgate
            List<PERSON> personList = null;
            try
            {
                personList = (from P in ctx.PERSON
                              join r in ctx.PERSON_RESP on P.PERSON_ID equals r.PERSON_ID
                              where (r.DELEGATE_1 == personID)
                              select P).ToList();
            }
            catch { }

            return personList;
        }


        #endregion

        #region organization

        public static COMPANY LookupPrimaryCompany(PSsqmEntities entities)
        {
            COMPANY primaryCompany = null;
                try
                {
                    primaryCompany = (from c in entities.COMPANY.Include("COMPANY_ACTIVITY") 
                                      where (c.IS_PRIMARY == true)
                                      select c).Single();
                    if (primaryCompany.COMPANY_ACTIVITY == null)
                    {
                        primaryCompany.COMPANY_ACTIVITY = new COMPANY_ACTIVITY();
                        primaryCompany.COMPANY_ACTIVITY.COMPANY_ID = primaryCompany.COMPANY_ID;
                        primaryCompany.COMPANY_ACTIVITY.EFF_STARTUP_DT = primaryCompany.CREATE_DT;
                        entities.AddToCOMPANY_ACTIVITY(primaryCompany.COMPANY_ACTIVITY);
                    }
                }
                catch (Exception e)
                {
                    //SQMLogger.LogException(e);
                }
 
            return primaryCompany;
        }

        public static COMPANY LookupCompany(decimal companyID)
        {
            COMPANY company = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                company = LookupCompany(entities, companyID, "", false);
            }

            return company;
        }

        public static COMPANY LookupCompany(SQM.Website.PSsqmEntities ctx, decimal companyID, string companyCode, bool createNew)
        {
            COMPANY company = null;
            try
            {
                if (companyID == 0)
                    company = (from c in ctx.COMPANY.Include("COMPANY_ACTIVITY") 
                                   where (c.ULT_DUNS_CODE == companyCode)
                                   select c).Single();
                else
                    company = (from c in ctx.COMPANY.Include("COMPANY_ACTIVITY") 
                           where (c.COMPANY_ID == companyID)
                           select c).Single();

            }
            catch (Exception ex)
            {
                if (createNew)
                {
                    company = CreateCompany(ctx, companyCode);
                }
            }

            return company;
        }

        public static COMPANY CreateCompany(SQM.Website.PSsqmEntities ctx, string companyCode)
        {
            COMPANY company = new COMPANY();
            company = new COMPANY();
            company.ULT_DUNS_CODE = companyCode;
            company.FYSTART_MONTH = 1;

            return company;
        }

        public static List<COMPANY> SelectCompanyList(SQM.Website.PSsqmEntities ctx, bool isCustomer, bool isSupplier, bool activeOnly)
        {
            List<COMPANY> companyList = null;
            try
            {
                if (activeOnly)
                    companyList = (from c in ctx.COMPANY   
                                where (c.STATUS == "A")
                                select c).ToList();
                else
                    companyList = (from c in ctx.COMPANY 
                                    select c).ToList();
                if (isCustomer)
                    companyList = companyList.Where(l => l.IS_CUSTOMER == true).ToList();
                if (isSupplier) 
                    companyList  = companyList.Where(l => l.IS_SUPPLIER == true).ToList();
            }
            catch
            {
                ;
            }
            return companyList;
        }

        public static COMPANY UpdateCompany(SQM.Website.PSsqmEntities ctx, COMPANY company, string updateBy)
        {
            company = (COMPANY)SQMModelMgr.SetObjectTimestamp((object)company, updateBy, company.EntityState);
            if (company.EntityState == EntityState.Detached)
                ctx.AddToCOMPANY(company);

            int status = ctx.SaveChanges();
            return company;
        }

		public static ACTIVE_CUSTOMER LookupActiveCustomer(SQM.Website.PSsqmEntities ctx, string companyCode)
		{
			ACTIVE_CUSTOMER activeCustomer = null;
			try
			{
				activeCustomer = (from c in ctx.ACTIVE_CUSTOMER
						   where (c.ULT_DUNS_CODE == companyCode)
						   select c).SingleOrDefault();
			}
			catch (Exception ex)
			{
			}

			return activeCustomer;
		}


        public static BUSINESS_ORG LookupBusOrg(SQM.Website.PSsqmEntities ctx, decimal companyID, string busOrgCode, bool findTopLevel, bool createNew)
        {
            BUSINESS_ORG busOrg = null;
            try
            {
                if (findTopLevel)
                    busOrg = (from bu in ctx.BUSINESS_ORG 
                              where (bu.COMPANY_ID == companyID) && (bu.PARENT_BUS_ORG_ID == bu.BUS_ORG_ID)
                              select bu).Single();
                else
                    busOrg = (from bu in ctx.BUSINESS_ORG 
                              where (bu.DUNS_CODE == busOrgCode) && (bu.COMPANY_ID == companyID)
                              select bu).Single();
            }
            catch
            {
                if (createNew)
                {
                    busOrg = CreateBusOrg(ctx, companyID, busOrgCode);
                }
            }

            return busOrg;
        }

        public static BUSINESS_ORG LookupBusOrg(decimal busOrgID)
        {
            BUSINESS_ORG busOrg = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                busOrg = LookupBusOrg(entities, 0, busOrgID);
            }

            return busOrg;
        }

        public static BUSINESS_ORG LookupBusOrg(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID)
        {
            BUSINESS_ORG busOrg = null;
            try
            {
                busOrg = (from bu in ctx.BUSINESS_ORG 
                              where (bu.BUS_ORG_ID == busOrgID)
                              select bu).Single();
            }
            catch
            {

            }

            return busOrg;
        }

        public static BUSINESS_ORG FindBusOrg(SQM.Website.PSsqmEntities ctx, COMPANY company, decimal busOrgID, string busOrgCode, bool findTopLevel, bool createNew)
        {
            BUSINESS_ORG busOrg = null;
            try
            {
                if (findTopLevel)
                    busOrg = company.BUSINESS_ORG.FirstOrDefault(b => (b.PARENT_BUS_ORG_ID == b.BUS_ORG_ID));
                else
                {
                    if (busOrgID == 0)
                        busOrg = company.BUSINESS_ORG.FirstOrDefault(b => (b.DUNS_CODE == busOrgCode));
                    else
                        busOrg = company.BUSINESS_ORG.FirstOrDefault(b => (b.BUS_ORG_ID == busOrgID));
                }
            }
            catch
            {
            }

            if (busOrg == null  &&  createNew)
            {
               busOrg = CreateBusOrg(ctx, company.COMPANY_ID, busOrgCode);
            }

            return busOrg;
        }


        public static BUSINESS_ORG LookupParentBusOrg(SQM.Website.PSsqmEntities ctx, BUSINESS_ORG busOrg)
        {
            BUSINESS_ORG parentOrg = null;
            try
            {
                using (PSsqmEntities entities = new PSsqmEntities())
                {
                    parentOrg = (from bu in entities.BUSINESS_ORG 
                                 where (bu.COMPANY_ID == busOrg.COMPANY_ID) && (bu.BUS_ORG_ID == busOrg.PARENT_BUS_ORG_ID)
                                 select bu).Single();
                }
            }
            catch
            {

            }

            return parentOrg;
        }

        public static BUSINESS_ORG CreateBusOrg(SQM.Website.PSsqmEntities ctx, decimal companyID, string busOrgCode)
        {
            BUSINESS_ORG busOrg = new BUSINESS_ORG();
            busOrg.DUNS_CODE = busOrgCode;
            busOrg.COMPANY_ID = companyID;
            busOrg.TIMERSET_ID = 0;

            ctx.AddToBUSINESS_ORG(busOrg);

            return busOrg;
        }

        public static BUSINESS_ORG CreateTopLevelBusOrg(SQM.Website.PSsqmEntities ctx, COMPANY company, string currencyCode)
        {
            BUSINESS_ORG busOrg = CreateBusOrg(ctx, company.COMPANY_ID, company.ULT_DUNS_CODE);

            busOrg = (BUSINESS_ORG)CopyObjectValues((object)busOrg, (object)company, false);
            busOrg.ORG_NAME = company.COMPANY_NAME;
            busOrg.PREFERRED_CURRENCY_CODE = currencyCode;
            ctx.BUSINESS_ORG.AddObject(busOrg);
            ctx.SaveChanges();
            busOrg.PARENT_BUS_ORG_ID = busOrg.BUS_ORG_ID;     // toplevel bu references self as parent ?
            
            ctx.SaveChanges();

            return busOrg;
        }

         public static List<BUSINESS_ORG> SelectBusOrgList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal parentBusOrgID, bool activeOnly)
        {
            List<BUSINESS_ORG> orgList = null;
            try
            {
                if (parentBusOrgID == 0)
                {
                        if (activeOnly)
                            orgList = (from bu in ctx.BUSINESS_ORG 
                                       where (bu.COMPANY_ID == companyID) && (bu.STATUS == "A")
                                       select bu).ToList();
                        else
                            orgList = (from bu in ctx.BUSINESS_ORG 
                                       where (bu.COMPANY_ID == companyID)
                                       select bu).ToList();
                }
                else
                {
                    if (activeOnly)
                        orgList = (from bu in ctx.BUSINESS_ORG 
                                   where (bu.COMPANY_ID == companyID) && (bu.PARENT_BUS_ORG_ID == parentBusOrgID) && (bu.STATUS == "A")
                               select bu).ToList();
                    else
                        orgList = (from bu in ctx.BUSINESS_ORG 
                                   where (bu.COMPANY_ID == companyID) && (bu.PARENT_BUS_ORG_ID == parentBusOrgID)
                                   select bu).ToList();
                }
            }
            catch
            {
                ;
            }
            return orgList;
        }

         public static List<BUSINESS_ORG> SearchBusOrgList(SQM.Website.PSsqmEntities ctx, decimal companyID, string searchCriteria, bool activeOnly)
         {
             List<BUSINESS_ORG> orgList = null;

             if (string.IsNullOrEmpty(searchCriteria) || searchCriteria == "%")
             {
                 if (activeOnly)
                     orgList = (from bu in ctx.BUSINESS_ORG.Include("plant")
                                where (bu.COMPANY_ID == companyID) && (bu.STATUS == "A")
                                select bu).ToList();
                 else
                     orgList = (from bu in ctx.BUSINESS_ORG.Include("plant")
                                where (bu.COMPANY_ID == companyID) 
                                select bu).ToList();
             }
             else
             {
                 if (activeOnly)
                     orgList = (from bu in ctx.BUSINESS_ORG.Include("plant")
                                where (bu.COMPANY_ID == companyID) && (bu.STATUS == "A") && ((bu.ORG_NAME.ToUpper().Contains(searchCriteria.ToUpper())) || (bu.DUNS_CODE.ToUpper().Contains(searchCriteria.ToUpper())))
                                select bu).ToList();
                 else
                     orgList = (from bu in ctx.BUSINESS_ORG.Include("plant")
                                where (bu.COMPANY_ID == companyID) && ((bu.ORG_NAME.ToUpper().Contains(searchCriteria.ToUpper())) || (bu.DUNS_CODE.ToUpper().Contains(searchCriteria.ToUpper())))
                                select bu).ToList();
             }

             return orgList;
         }

        public static PLANT LookupPlant(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, decimal plantID, string plantCode, bool createNew)
        {
            PLANT plant = null;
            try
            {
                if (busOrgID == 0)
                {
                    if (plantID > 0)
                        plant = (from pl in ctx.PLANT.Include("Address")
                                where (pl.COMPANY_ID == companyID) && (pl.PLANT_ID == plantID)
                                select pl).Single();
                    else 
                        plant = (from pl in ctx.PLANT.Include("Address")
                                where (pl.COMPANY_ID == companyID) && (pl.DUNS_CODE == plantCode)
                                select pl).Single();
                }
                else
                {
                    if (plantID > 0)
                        plant = (from pl in ctx.PLANT.Include("Address")
                                where (pl.COMPANY_ID == companyID) && (pl.BUS_ORG_ID == busOrgID) &&  (pl.PLANT_ID == plantID)
                                select pl).Single();
                    else
                        plant = (from pl in ctx.PLANT.Include("Address")
                                where (pl.COMPANY_ID == companyID) && (pl.BUS_ORG_ID == busOrgID) && (pl.DUNS_CODE == plantCode)
                                select pl).Single();
                }
                plant.PLANT_LINE.Load();
            }
            catch
            {
                if (createNew)
                {
                    plant = new PLANT();
					plant.STATUS = "A";
                    plant.COMPANY_ID = companyID;
                    plant.BUS_ORG_ID = busOrgID;
                    plant.DUNS_CODE = plantCode;
					plant.LOCAL_LANGUAGE = 1;
					plant.LOCATION_CODE = "";
					plant.TRACK_EW_DATA = true;
					plant.TRACK_FIN_DATA = true;

                }
            }
            return plant;
        }

        public static PLANT LookupPlant(decimal plantID)
        {
            PLANT plant = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                plant = LookupPlant(entities, plantID, "");
            }

            return plant;
        }

        public static PLANT LookupPlant(SQM.Website.PSsqmEntities ctx, decimal plantID, string dunsCode)
        {
            PLANT plant = null;

            try
            {
                if (String.IsNullOrEmpty(dunsCode))
                    plant = (from pl in ctx.PLANT.Include("Address")
                             where (pl.PLANT_ID == plantID)
                             select pl).Single();
                else
                    plant = (from pl in ctx.PLANT.Include("Address")
                             where (pl.DUNS_CODE == dunsCode)
                             select pl).Single();
            }
            catch (Exception e)
            {
              //  SQMLogger.LogException(e);
            }

            return plant;
        }

		public static PLANT LookupPlant(SQM.Website.PSsqmEntities ctx, string dunsCode, string altDunsCode)
		{
			PLANT plant = null;

			try
			{
				plant = (from pl in ctx.PLANT.Include("Address")
							where (pl.DUNS_CODE == dunsCode  || pl.ALT_DUNS_CODE == altDunsCode)
							select pl).SingleOrDefault();
			}
			catch (Exception e)
			{
				//  SQMLogger.LogException(e);
			}

			return plant;
		}

        public static List<PLANT> LoadPlantList(BUSINESS_ORG busOrg)
        {
            List<PLANT> plantList = null;
            using (PSsqmEntities entities = new PSsqmEntities())
            {
                plantList = SelectPlantList(entities, busOrg.COMPANY_ID, busOrg.BUS_ORG_ID);
            }

            return plantList;
        }

        public static List<PLANT> SelectPlantList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID)
        {
            List<PLANT> plantList = null;
            try
            {
                 if (busOrgID == 0)
                        plantList = (from pl in ctx.PLANT.Include("Address")
                                     where (pl.COMPANY_ID == companyID  &&  pl.BUS_ORG_ID != null)
                                     select pl).ToList();
                    else
                        plantList = (from pl in ctx.PLANT.Include("Address")
                                     where (pl.BUS_ORG_ID == busOrgID)
                                     select pl).ToList();
            }
            catch
            {
                ;
            }
            return plantList;
        }

        public static List<PLANT> SearchPlantList(SQM.Website.PSsqmEntities ctx, decimal companyID, string searchCriteria, bool activeOnly, bool unAssocOnly)
        {
            List<PLANT> plantList = null;

            if (string.IsNullOrEmpty(searchCriteria) || searchCriteria == "%")
            {
                if (activeOnly)
                    plantList = (from pl in ctx.PLANT
                                 where (pl.COMPANY_ID == companyID) && (pl.BUS_ORG_ID != null) &&  (pl.STATUS == "A")
                                 select pl).ToList();
                else
                {
                    if (unAssocOnly)
                        plantList = (from pl in ctx.PLANT
                                     where (pl.COMPANY_ID == companyID) && (pl.BUS_ORG_ID == null)
                                     select pl).ToList();
                    else
                        plantList = (from pl in ctx.PLANT
                                    where (pl.COMPANY_ID == companyID)
                                    select pl).ToList();
                }
            }
            else
            {
                if (activeOnly)
                    plantList = (from pl in ctx.PLANT 
                               where (pl.COMPANY_ID == companyID) && (pl.STATUS == "A") && ((pl.PLANT_NAME.ToUpper().Contains(searchCriteria.ToUpper())) || (pl.DUNS_CODE.ToUpper().Contains(searchCriteria.ToUpper())))
                               select pl).ToList();
                else
                    plantList = (from pl in ctx.PLANT 
                               where (pl.COMPANY_ID == companyID) && ((pl.PLANT_NAME.ToUpper().Contains(searchCriteria.ToUpper())) || (pl.DUNS_CODE.ToUpper().Contains(searchCriteria.ToUpper())))
                               select pl).ToList();
            }

            return plantList;
        }

        public static PLANT UpdatePlant(SQM.Website.PSsqmEntities ctx, PLANT plant, string updateBy)
        {
            plant = (PLANT)SQMModelMgr.SetObjectTimestamp((object)plant, updateBy, plant.EntityState);
            if (plant.EntityState == EntityState.Detached)
                ctx.AddToPLANT(plant);

            ctx.SaveChanges();
            return plant;
        }

        public static List<PLANT> FilterPlantListByProperty(List<PLANT> plantListIn, string property)
        {
            List<PLANT> plantListOut = new List<PLANT>();

            foreach (PLANT plant in plantListIn)
            {
                switch (property)
                {
                    case "TRACK_EW_DATA":
                        if (plant.TRACK_EW_DATA.HasValue && plant.TRACK_EW_DATA == true)
                            plantListOut.Add(plant);
                        break;
                    case "TRACK_FIN_DATA":
                        if (plant.TRACK_FIN_DATA.HasValue && plant.TRACK_FIN_DATA == true)
                            plantListOut.Add(plant);
                        break;
                    default:
                        plantListOut.Add(plant);
                        break;
                }
            }

            return plantListOut;
        }

        public static List<BusinessLocation> SelectBusinessLocationList(decimal companyID, decimal busOrgID, bool activeOnly)
        {
            if (companyID > 0)
                return SelectBusinessLocationList(companyID, busOrgID, false, false, activeOnly);
            else
                return new List<BusinessLocation>();
        }

        public static List<BusinessLocation> SelectBusinessLocationList(decimal companyID, decimal busOrgID, bool isSupplier, bool isCustomer, bool activeOnly)
        {
            List<BusinessLocation> locationList = new List<BusinessLocation>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    if (companyID == 0)
                    {
                        if (isSupplier)
                        {
                            locationList = (from p in entities.PLANT
                                            join c in entities.COMPANY on p.COMPANY_ID equals c.COMPANY_ID
                                            join b in entities.BUSINESS_ORG on p.BUS_ORG_ID equals b.BUS_ORG_ID into b_p
                                            where (c.IS_SUPPLIER == true &&  c.COMPANY_ID > 1  && p.BUS_ORG_ID > 0)
                                            from b in b_p.DefaultIfEmpty()
                                            join a in entities.ADDRESS on p.PLANT_ID equals a.PLANT_ID into a_p
                                            from a in a_p.DefaultIfEmpty()
                                            select new BusinessLocation
                                            {
                                                Company = c,
                                                Plant = p,
                                                BusinessOrg = b,
                                                Address = a
                                            }).OrderBy(l => l.Company.COMPANY_ID).ToList();
                        }
                        else if (isCustomer)
                        {
                            locationList = (from p in entities.PLANT
                                            join c in entities.COMPANY on p.COMPANY_ID equals c.COMPANY_ID
                                            join b in entities.BUSINESS_ORG on p.BUS_ORG_ID equals b.BUS_ORG_ID into b_p
                                            where (c.IS_CUSTOMER == true && c.COMPANY_ID > 1 &&  p.BUS_ORG_ID > 0)
                                            from b in b_p.DefaultIfEmpty()
                                            join a in entities.ADDRESS on p.PLANT_ID equals a.PLANT_ID into a_p
                                            from a in a_p.DefaultIfEmpty()
                                            select new BusinessLocation
                                            {
                                                Company = c,
                                                Plant = p,
                                                BusinessOrg = b,
                                                Address = a
                                            }).OrderBy(l => l.Company.COMPANY_ID).ToList();
                        }
                    }
                    else
                    {
                        if (busOrgID == 0)
                        {
                            locationList = (from p in entities.PLANT
                                            join c in entities.COMPANY on p.COMPANY_ID equals c.COMPANY_ID 
                                            join b in entities.BUSINESS_ORG on p.BUS_ORG_ID equals b.BUS_ORG_ID into b_p
                                            where (p.COMPANY_ID == companyID && p.BUS_ORG_ID > 0)
                                            from b in b_p.DefaultIfEmpty()
                                            join a in entities.ADDRESS on p.PLANT_ID equals a.PLANT_ID into a_p
                                            from a in a_p.DefaultIfEmpty()
                                            select new BusinessLocation
                                            {
                                                Company = c,
                                                Plant = p,
                                                BusinessOrg = b,
                                                Address = a 
                                            }).ToList();
                        }
                        else
                        {
                            locationList = (from p in entities.PLANT.Include("Address")
                                            join c in entities.COMPANY on p.COMPANY_ID equals c.COMPANY_ID 
                                            join b in entities.BUSINESS_ORG on p.BUS_ORG_ID equals b.BUS_ORG_ID
                                            where (b.COMPANY_ID == companyID && b.BUS_ORG_ID == busOrgID)
                                            join a in entities.ADDRESS on p.PLANT_ID equals a.PLANT_ID into a_p
                                            from a in a_p.DefaultIfEmpty()
                                            select new BusinessLocation
                                            {
                                                Company = c,
                                                Plant = p,
                                                BusinessOrg = b,
                                                Address = a
                                            }).ToList();
                        }
                    }
                }

                catch
                {

                }
            }

            if (activeOnly)
                return locationList.Where(l => l.Company.STATUS == "A" && l.BusinessOrg.STATUS == "A" && l.Plant.STATUS == "A").ToList();
            else
                return locationList;
        }

        public static List<BusinessLocation> UserAccessibleLocations(PERSON person, List<BusinessLocation> locationList, bool checkInternal, bool checkExternal, string companyContext)
        {
            List<BusinessLocation> userLocationList = new List<BusinessLocation>();

            if (checkInternal)
            {
                if (person.ROLE <= 100 ||  string.IsNullOrEmpty(person.NEW_LOCATION_CD))
                {
                    userLocationList.AddRange(locationList.Where(l => l.Company.COMPANY_ID == SessionManager.PrimaryCompany().COMPANY_ID).ToList());
                }
                else
                {
                    try
                    {
                        string[] locs = person.NEW_LOCATION_CD.Split(',');
                        decimal[] plantArray = Array.ConvertAll(locs, new Converter<string, decimal>(decimal.Parse));
                        userLocationList.AddRange(locationList.Where(l => plantArray.Contains(l.Plant.PLANT_ID)).ToList());
                    }
                    catch { ; }
                }
            }

            if (checkExternal)
            {
                switch (companyContext)
                {
                    case "CST":   // customers
                        if (!string.IsNullOrEmpty(person.OLD_LOCATION_CD))
                        {
                            string[] locs = person.OLD_LOCATION_CD.Split(',');
                            decimal[] plantArray = Array.ConvertAll(locs, new Converter<string, decimal>(decimal.Parse));
                            userLocationList.AddRange(locationList.Where(l => plantArray.Contains(l.Plant.PLANT_ID)).ToList());
                        }
                        else
                        {
                            userLocationList.AddRange(locationList.Where(l => l.Company.IS_CUSTOMER == true).ToList());
                        }
                        break;
                    case "RCV":   // suppliers
                        userLocationList.AddRange(locationList.Where(l => l.Company.IS_SUPPLIER == true).ToList());
                        break;
                    default:
                        break;
                }
            }

            return userLocationList;
        }

        public static DEPARTMENT LookupDepartment(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, decimal plantID, decimal deptID, string deptCode, bool createNew)
        {
            DEPARTMENT dept = null;
            try
            {
                if (plantID > 0)
                {
                    if (deptID > 0)
                        dept = (from d in ctx.DEPARTMENT
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == plantID) && (d.DEPT_ID == deptID)
                                select d).Single();
                    else
                        dept = (from d in ctx.DEPARTMENT
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == plantID) && (d.DEPT_CODE == deptCode)
                                select d).Single();
                }
                else
                {
                    if (deptID > 0)
                        dept = (from d in ctx.DEPARTMENT
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == null) && (d.DEPT_ID == deptID)
                                select d).Single();
                    else
                        dept = (from d in ctx.DEPARTMENT
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == null) && (d.DEPT_CODE == deptCode)
                                select d).Single();
                }
            }
            catch
            {
                if (createNew)
                {
                    dept = new DEPARTMENT();
                    dept.COMPANY_ID = companyID;
                }
            }

            return dept;
        }

        public static DEPARTMENT FindDepartment(SQM.Website.PSsqmEntities ctx, List<DEPARTMENT> deptList, COMPANY company, BUSINESS_ORG busOrg, PLANT plant, string deptCode, bool createNew)
        {
            DEPARTMENT dept = null;
            try
            {
                if (plant != null  &&  plant.PLANT_ID > 0)     // plant-level dept (pass busOrg as null)
                    dept = deptList.FirstOrDefault(d => (d.DEPT_CODE == deptCode) && (d.PLANT_ID == plant.PLANT_ID));
                else     // bu-level dept  (pass plant as null)
                    dept = deptList.FirstOrDefault(d => (d.DEPT_CODE == deptCode) && (d.BUS_ORG_ID == busOrg.BUS_ORG_ID)  &&  (d.PLANT_ID == null));

            }
            catch
            {
            }

            if (dept == null && createNew)
            {
                dept = new DEPARTMENT();
                dept.DEPT_CODE = deptCode;
                deptList.Add(dept);
            }

            return dept;
        }

		public static List<DEPARTMENT> SelectDepartmentList(SQM.Website.PSsqmEntities ctx, decimal plantID)
		{
			return SelectDepartmentList(ctx, 0, 0, plantID);
		}

        public static List<DEPARTMENT> SelectDepartmentList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, decimal plantID)
        {
            List<DEPARTMENT> deptList = null;
            try
            {
                if (plantID > 0)
                        deptList = (from d in ctx.DEPARTMENT
                                    where (d.PLANT_ID == plantID)
                                    select d).ToList();
                else 
                    deptList = (from d in ctx.DEPARTMENT
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) &&  (d.PLANT_ID == null) 
                                select d).ToList();
            }
            catch
            {
                ;
            }
            return deptList;
        }

        public static int UpdateDeptList(SQM.Website.PSsqmEntities ctx, List<DEPARTMENT> deptList, COMPANY company, BUSINESS_ORG busOrg, PLANT plant)
        {
            int status = 0;

            if (deptList.Count > 0)
            {
                for (int d = 0; d < deptList.Count; d++)
                {
                    DEPARTMENT dept = deptList[d];
                    if (dept.EntityState == EntityState.Detached)
                    {
                        dept = (DEPARTMENT)SetObjectTimestamp((object)dept, "sqmload", dept.EntityState);
                        dept.COMPANY_ID = company.COMPANY_ID;
                        if (busOrg != null)
                            dept.BUS_ORG_ID = busOrg.BUS_ORG_ID;
                        if (plant != null)
                            dept.PLANT_ID = plant.PLANT_ID;

                        company.DEPARTMENT.Add(dept);
                    }
                }
                ctx.SaveChanges();
                deptList.Clear();
            }

            return status;
        }

        public static DEPARTMENT CreateDepartment(SQM.Website.PSsqmEntities ctx, BUSINESS_ORG busOrg, DEPARTMENT dept, string createBy)
        {
            dept.COMPANY_ID = busOrg.COMPANY_ID;
            dept.BUS_ORG_ID = busOrg.BUS_ORG_ID;
            dept = (DEPARTMENT)SQMModelMgr.SetObjectTimestamp((object)dept, createBy, dept.EntityState);
            ctx.DEPARTMENT.AddObject(dept);
            ctx.SaveChanges();
            return dept;
        }

        public static DEPARTMENT CreateDepartment(SQM.Website.PSsqmEntities ctx, PLANT plant, DEPARTMENT dept, string createBy)
        {
            dept.COMPANY_ID = plant.COMPANY_ID;
            dept.BUS_ORG_ID = plant.BUS_ORG_ID;
            dept.PLANT_ID = plant.PLANT_ID;
            dept = (DEPARTMENT)SQMModelMgr.SetObjectTimestamp((object)dept, createBy, dept.EntityState);
            ctx.DEPARTMENT.AddObject(dept);
            ctx.SaveChanges();
            return dept;
        }

        public static DEPARTMENT UpdateDepartment(SQM.Website.PSsqmEntities ctx, DEPARTMENT dept, string updateBy)
        {
            dept = (DEPARTMENT)SQMModelMgr.SetObjectTimestamp((object)dept, updateBy, dept.EntityState);
            ctx.SaveChanges();
            return dept;
        }


        public static List<LABOR_TYPE> SelectLaborTypeList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, decimal plantID)
        {
            List<LABOR_TYPE> laborList = null;
            try
            {
                if (plantID > 0)
                {
                    laborList = (from l in ctx.LABOR_TYPE
                                 where (l.COMPANY_ID == companyID) && (l.PLANT_ID == plantID)
                                 select l).ToList();
                }
                else 
                {
                    laborList = (from l in ctx.LABOR_TYPE
                                 where (l.COMPANY_ID == companyID) && (l.BUS_ORG_ID == busOrgID) && (l.PLANT_ID == null)
                                 select l).ToList();
                }
            }
            catch
            {
                ;
            }
            return laborList;
        }

        public static LABOR_TYPE LookupLaborType(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, decimal plantID, decimal laborID, string laborCode, bool createNew)
        {
            LABOR_TYPE labor = null;
            try
            {
                if (plantID > 0)
                {
                    if (laborID > 0)
                        labor = (from d in ctx.LABOR_TYPE
                                 where (d.PLANT_ID == plantID && d.LABOR_TYP_ID == laborID)
                                 select d).Single();
                    else
                        labor = (from d in ctx.LABOR_TYPE
                                 where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == plantID) && (d.LABOR_CODE == laborCode)
                                 select d).Single();
                }
                else
                {
                    if (laborID > 0)
                        labor = (from d in ctx.LABOR_TYPE 
                                where (d.LABOR_TYP_ID == laborID)
                                select d).Single();
                    else
                        labor = (from d in ctx.LABOR_TYPE 
                                where (d.COMPANY_ID == companyID) && (d.BUS_ORG_ID == busOrgID) && (d.PLANT_ID == null) && (d.LABOR_CODE == laborCode)
                                select d).Single();
                }
            }
            catch
            {
                if (createNew)
                {
                    labor = new LABOR_TYPE();
                    labor.COMPANY_ID = companyID;
                    labor.BUS_ORG_ID = busOrgID;
                }
            }

            return labor;
        }

        public static LABOR_TYPE FindLaborType(SQM.Website.PSsqmEntities ctx, List<LABOR_TYPE> laborList, COMPANY company, BUSINESS_ORG busOrg, PLANT plant, string laborCode, bool createNew)
        {
            LABOR_TYPE labor = null;
            try
            {
                if (plant != null  &&  plant.PLANT_ID > 0)     // plant-level labor 
                    labor = laborList.FirstOrDefault(l => (l.LABOR_CODE == laborCode) && (l.BUS_ORG_ID == busOrg.BUS_ORG_ID) && (l.PLANT_ID == plant.PLANT_ID));
                else     // bu-level labor
                    labor = laborList.FirstOrDefault(l => (l.LABOR_CODE == laborCode) && (l.BUS_ORG_ID == busOrg.BUS_ORG_ID) && (l.PLANT_ID == null));
   
            }
            catch
            {
            }

            if (labor == null  &&  createNew)
            {
                labor = new LABOR_TYPE();
                labor.LABOR_CODE = laborCode;
                laborList.Add(labor);
            }

            return labor;
        }

        public static int UpdateLaborList(SQM.Website.PSsqmEntities ctx, List<LABOR_TYPE> laborList, COMPANY company, BUSINESS_ORG busOrg, PLANT plant)
        {
            int status = 0;

            if (laborList.Count > 0)
            {
                for (int l = 0; l<laborList.Count; l++)
                {
                    LABOR_TYPE labor = laborList[l];
                    labor = (LABOR_TYPE)SetObjectTimestamp((object)labor, "sqmload", labor.EntityState);
                    if (labor.EntityState == EntityState.Detached ||  labor.EntityState == EntityState.Modified)
                    {
                        labor.COMPANY_ID = company.COMPANY_ID;
                        labor.BUS_ORG_ID = busOrg.BUS_ORG_ID;
                        if (plant != null)
                            labor.PLANT_ID = plant.PLANT_ID;
                        if (labor.EntityState == EntityState.Detached)
                            company.LABOR_TYPE.Add(labor);
                    }
                }
                ctx.SaveChanges();
                laborList.Clear();
            }

            return status;
        }

        public static LABOR_TYPE CreateLaborType(SQM.Website.PSsqmEntities ctx, BUSINESS_ORG busOrg, LABOR_TYPE labor, string createBy)
        {
            labor.COMPANY_ID = busOrg.COMPANY_ID;
            labor.BUS_ORG_ID = busOrg.BUS_ORG_ID;
            labor = (LABOR_TYPE)SQMModelMgr.SetObjectTimestamp((object)labor, createBy, labor.EntityState);
            ctx.LABOR_TYPE.AddObject(labor);
            ctx.SaveChanges();
            return labor;
        }

        public static LABOR_TYPE CreateLaborType(SQM.Website.PSsqmEntities ctx, PLANT plant, LABOR_TYPE labor, string createBy)
        {
            labor.COMPANY_ID = plant.COMPANY_ID;
            labor.BUS_ORG_ID = (decimal)plant.BUS_ORG_ID;
            labor.PLANT_ID = plant.PLANT_ID;
            labor = (LABOR_TYPE)SQMModelMgr.SetObjectTimestamp((object)labor, createBy, labor.EntityState);
            ctx.LABOR_TYPE.AddObject(labor);
            ctx.SaveChanges();
            return labor;
        }

        public static LABOR_TYPE UpdateLaborType(SQM.Website.PSsqmEntities ctx, LABOR_TYPE labor, string updateBy)
        {
            labor = (LABOR_TYPE)SQMModelMgr.SetObjectTimestamp((object)labor, updateBy, labor.EntityState);
            int status = ctx.SaveChanges();
            return labor;
        }

        public static PLANT_LINE LookupPlantLine(SQM.Website.PSsqmEntities ctx, decimal plantID, decimal lineID, string lineName, bool createNew)
        {
            PLANT_LINE line = null;
            try
            {
                if (lineID > 0)
                    line = (from d in ctx.PLANT_LINE 
                             where (d.PLANT_LINE_ID == lineID)
                             select d).Single();
                else
                    line = (from d in ctx.PLANT_LINE
                            where (d.PLANT_ID == plantID) && (d.PLANT_LINE_NAME == lineName)
                            select d).Single();
            }
            catch
            {
            }

            if (line == null && createNew)
            {
                line = new PLANT_LINE();
                line.PLANT_ID = plantID;
            }

            return line;
        }

        public static PLANT_LINE FindPlantLine(SQM.Website.PSsqmEntities ctx, PLANT plant, decimal lineID, string lineName, bool createNew)
        {
            PLANT_LINE line = null;
            try
            {
                if (lineID > 0)
                    line = plant.PLANT_LINE.FirstOrDefault(l => (l.PLANT_LINE_ID == lineID));
                else    
                     line = plant.PLANT_LINE.FirstOrDefault(l => (l.PLANT_LINE_NAME == lineName));

            }
            catch
            {
            }

            if (line == null && createNew)
            {
                line = new PLANT_LINE();
                line.PLANT_ID = plant.PLANT_ID;
            }

            return line;
        }

        public static PLANT_LINE CreatePlantLine(SQM.Website.PSsqmEntities ctx, PLANT plant, PLANT_LINE line, string createBy)
        {
            line.PLANT_ID = plant.PLANT_ID;
            line = (PLANT_LINE)SQMModelMgr.SetObjectTimestamp((object)line, createBy, line.EntityState);
            ctx.PLANT_LINE.AddObject(line);
            ctx.SaveChanges();
            return line;
        }

        public static PLANT_LINE UpdatePlantLine(SQM.Website.PSsqmEntities ctx, PLANT_LINE line, string updateBy)
        {
            line = (PLANT_LINE)SQMModelMgr.SetObjectTimestamp((object)line, updateBy, line.EntityState);
            ctx.SaveChanges();
            return line;
        }

        public static List<PLANT_LINE> SelectPlantLineList(SQM.Website.PSsqmEntities ctx, decimal plantID)
        {
            List<PLANT_LINE> lineList = new List<PLANT_LINE>();

            lineList = (from pl in ctx.PLANT_LINE 
                            where (pl.PLANT_ID == plantID)
                            select pl).ToList();

            return lineList;
        }

        public static List<PRODUCT_LINE> SelectProductLineList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID)
        {
            List<PRODUCT_LINE> prodlineList = new List<PRODUCT_LINE>();
            try
            {
                prodlineList = (from pl in ctx.PRODUCT_LINE 
                                  where (pl.COMPANY_ID == companyID && pl.BUS_ORG_ID == busOrgID)
                                  select pl).ToList();
            }
            catch (Exception ex)
            {
                ;
            }
            return prodlineList;
        }

        public static int UpdateProductLineList(SQM.Website.PSsqmEntities ctx, decimal companyID, decimal busOrgID, List<PRODUCT_LINE> prodlineList)
        {
            updateStatus = 0;

            try
            {
                string delCmd = "";
                foreach (PRODUCT_LINE prod in prodlineList)
                {
                    if (prod.EntityState == EntityState.Deleted || prod.STATUS == true.ToString()  ||  String.IsNullOrEmpty(prod.PRODUCT_LINE_CODE))
                        delCmd += ("," + prod.PROD_LINE_ID.ToString());
                    else
                    {
                        prod.COMPANY_ID = companyID;
                        prod.BUS_ORG_ID = busOrgID;
                        if (prod.EntityState == EntityState.Detached)
                            ctx.PRODUCT_LINE.AddObject(prod);
                        else
                        {
                           PRODUCT_LINE updateProd = (from pl in ctx.PRODUCT_LINE 
                                  where (pl.PROD_LINE_ID == prod.PROD_LINE_ID)
                                  select pl).Single();

                            updateProd = (PRODUCT_LINE) CopyObjectValues(updateProd, prod, false);
                        }

                    }
                }
                if (!String.IsNullOrEmpty(delCmd))
                {
                    delCmd = delCmd.Substring(1);
                    ctx.ExecuteStoreCommand("DELETE FROM PRODUCT_LINE WHERE PROD_LINE_ID IN (" + delCmd + ")");
                }
                updateStatus = ctx.SaveChanges();
            }
            catch
            {
            }

            return updateStatus;
        }

        #endregion

        #region teams

        public static List<NOTIFY> SelectNotifyList(SQM.Website.PSsqmEntities ctx, decimal busorgID, decimal plantID, decimal b2bID, List<string> scopeList)
        {
            List<NOTIFY> notifyList = new List<NOTIFY>();
            try
            {
                if (plantID > 0)
                {
                    if (b2bID > 0)
                        notifyList = (from m in ctx.NOTIFY
                                      where (m.PLANT_ID == plantID && m.B2B_ID == b2bID && scopeList.Contains(m.NOTIFY_SCOPE))
                                      select m).ToList();
                    else
                        notifyList = (from m in ctx.NOTIFY
                                      where (m.PLANT_ID == plantID && m.B2B_ID == null && scopeList.Contains(m.NOTIFY_SCOPE))
                                      select m).ToList();
                }
                else
                {
                    notifyList = (from m in ctx.NOTIFY
                                  where (m.BUS_ORG_ID == busorgID && m.PLANT_ID == null && scopeList.Contains(m.NOTIFY_SCOPE))
                                  select m).ToList();
                }
            }
            catch (Exception ex)
            {
                ;
            }
            return notifyList;
        }

        public static NOTIFY CreateNotifyRecord(decimal companyID, decimal busorgID, decimal plantID, TaskRecordType scope, decimal b2bID)
        {
            NOTIFY notify = new NOTIFY();
            notify.STATUS = "A";
            notify.COMPANY_ID = companyID;
            if (busorgID > 0)
                notify.BUS_ORG_ID = busorgID;
            if (plantID > 0)
                notify.PLANT_ID = plantID;
            if (b2bID > 0)
                notify.B2B_ID = b2bID;
            notify.NOTIFY_SCOPE = ((int)scope).ToString();

            return notify;
        }


        public static NOTIFY LookupNotifyRecord(SQM.Website.PSsqmEntities ctx, decimal notifyID)
        {
            NOTIFY notify = null;

            try
            {
                notify = (from m in ctx.NOTIFY where m.NOTIFY_ID == notifyID select m).SingleOrDefault();
            }
            catch { }

            return notify;
        }

        public static List<NOTIFY> SelectNotifyHierarchy(decimal companyID, decimal busorgID, decimal plantID, decimal b2bID, TaskRecordType scope)
        {
            List<NOTIFY> notificationList = new List<NOTIFY>();
            NOTIFY notify = null;

            using (PSsqmEntities ctx = new PSsqmEntities())
            {
                try
                {
                    string notifyScope = ((int)scope).ToString();
                    if (busorgID > 0)
                    {   // get bus org level assignments
                        notify = (from m in ctx.NOTIFY where m.BUS_ORG_ID == busorgID && m.PLANT_ID == null && m.NOTIFY_SCOPE == notifyScope select m).SingleOrDefault();
                        if (notify != null)
                            notificationList.Add(notify);
                    }
                    if (plantID > 0)
                    {   // get plant level assignments
                        notify = (from m in ctx.NOTIFY where m.PLANT_ID == plantID && m.B2B_ID == null && m.NOTIFY_SCOPE == notifyScope select m).SingleOrDefault();
                        if (notify != null)
                            notificationList.Add(notify);
                    }
                    if (b2bID > 0)
                    {   // get trading partner assignments
                        notify = (from m in ctx.NOTIFY where m.PLANT_ID == plantID && m.B2B_ID == b2bID && m.NOTIFY_SCOPE == notifyScope select m).SingleOrDefault();
                        if (notify != null)
                            notificationList.Add(notify);
                    }
                }
                catch { }
            }

            return notificationList;
        }

        public static HashSet<string> DistinctNotifyEmailList(List<NOTIFY> notifyList)
        {
            var hashList = new HashSet<string>();
            foreach (NOTIFY notify in notifyList)
            {
                try
                {
                    hashList.Add(SQMModelMgr.LookupPerson((decimal)notify.NOTIFY_PERSON1, "").EMAIL);
                    hashList.Add(SQMModelMgr.LookupPerson((decimal)notify.NOTIFY_PERSON2, "").EMAIL);
                }
                catch { }
            }

            return hashList;
        }

        public static int UpdateNotifyRecord(SQM.Website.PSsqmEntities ctx, NOTIFY notify)
        {
            int status = 0;

            NOTIFY srcNotify = (from m in ctx.NOTIFY where m.NOTIFY_ID == notify.NOTIFY_ID select m).SingleOrDefault();

            if (srcNotify == null)
            {
                srcNotify = new NOTIFY();
                ctx.AddToNOTIFY(srcNotify);
            }

            srcNotify = (NOTIFY)CopyObjectValues(srcNotify, notify, false);

            status = ctx.SaveChanges();
      
            return status;
        }

        public static int UpdateNotifyList(SQM.Website.PSsqmEntities ctx, List<NOTIFY> notifyList)
        {
            int status = 0;

            try
            {
                for (int n = 0; n < notifyList.Count; n++)
                {
                    NOTIFY notify = notifyList.ElementAt(n);
                    if (notify.STATUS == "D")
                        ctx.DeleteObject(notify);
                    else if (notify.EntityState == EntityState.Added || notify.EntityState == EntityState.Detached)
                        ctx.AddToNOTIFY(notify);
                }

                status = ctx.SaveChanges();
            }

            catch (Exception ex)
            {
               // SQMLogger.LogException(ex);
                status = -1;
            }

            return status;
        }

        public static List<NOTIFY> SelectPersonEscalationList(SQM.Website.PSsqmEntities ctx, decimal personID)
        {
            // get all escalations that the person is assigned to
            List<NOTIFY> escalateList = new List<NOTIFY>();
            try
            {
                escalateList = (from m in ctx.NOTIFY 
                                where (m.ESCALATE_PERSON1 == personID  ||  m.ESCALATE_PERSON2 == personID)
                                select m).ToList();
            }
            catch (Exception ex)
            {
                ;
            }
            return escalateList;
        }

        #endregion

        #region part

        public static PART_PROGRAM LookupPartProgram(decimal programID, string programCode)
        {
            PART_PROGRAM program = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    if (programID > 0)
                    {
                        program = (from pp in entities.PART_PROGRAM
                                   where (pp.PROGRAM_ID == programID)
                                   select pp).Single();
                    }
                    else
                    {
                        program = (from pp in entities.PART_PROGRAM
                                   where (pp.PROGRAM_CODE == programCode)
                                   select pp).Single();
                    }
                }
                catch
                {
                }
            }

            return program;
        }

        public static List<PART_ATTRIBUTE> SelectPartAttributeList(string attributeCategory, string filterByCode, bool activeOnly)
        {
            List<PART_ATTRIBUTE> attributeList = new List<PART_ATTRIBUTE>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                try
                {
                    if (!string.IsNullOrEmpty(filterByCode))
                        attributeList = (from pa in entities.PART_ATTRIBUTE
                                     where (pa.ATTRIBUTE_CATEGORY == attributeCategory  &&  pa.ATTRIBUTE_CD.ToUpper().Contains(filterByCode.ToUpper()))
                                     select pa).ToList();
                    else 
                        attributeList = (from pa in entities.PART_ATTRIBUTE
                                     where (pa.ATTRIBUTE_CATEGORY == attributeCategory)
                                     select pa).ToList();

                    if (activeOnly)
                        attributeList = attributeList.Where(l => l.STATUS != "I").ToList();

                }
                catch { ; }
            }

            return attributeList;
        }

        public static List<PART_PROGRAM> SelectPartProgramList(decimal customerID, decimal busorgID)
        {
            List<PART_PROGRAM> programList = null;

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                if (customerID > 0)
                {
                    programList = (from pp in entities.PART_PROGRAM
                                   where (pp.CUSTOMER_ID == customerID)
                                   select pp).ToList();
                }
                else
                {
                    programList = (from pp in entities.PART_PROGRAM 
                                   select pp).ToList();
                }

                if (busorgID > 0)
                    programList = programList.FindAll(l => l.BUS_ORG_ID == busorgID  ||  l.BUS_ORG_ID == null  ||  l.BUS_ORG_ID < 0);
            }

            return programList;
        }

        public static PartData LookupPartData(PSsqmEntities entities, decimal companyID, decimal partID)
        {
            PartData part = null;

            part = SelectPartDataList(entities, companyID, partID, 0, 0, -1).FirstOrDefault();

            return part;
        }

        public static List<PartData> SelectTradingRelationshipList(PSsqmEntities entities, decimal companyID, decimal plantID, decimal partID, int relationshipType, decimal partnerID)
        {
            // get parts based on:  0 = internal  1 = customers of this plant  2 = supplierrs to this plant
            List<PartData> partList = new List<PartData>();

            switch (relationshipType)
            {
                case 1:
                    if (partnerID > 0)
                        partList = (from p in entities.PART
                                    join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                    join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                    where (p.COMPANY_ID == companyID)
                                    from m in m_p.DefaultIfEmpty()
                                    from u in u_p.DefaultIfEmpty()
                                    where u.PLANT_ID == plantID && u.CUST_PLANT_ID == partnerID 
                                    join l in entities.PLANT on u.CUST_PLANT_ID equals l.PLANT_ID into l_u
                                    from l in l_u.DefaultIfEmpty()
                                    join c in entities.COMPANY on l.COMPANY_ID equals c.COMPANY_ID into c_l
                                    from c in c_l.DefaultIfEmpty()
                                    join n in entities.NOTIFY on new { u.PLANT_ID, cust = u.CUST_PLANT_ID } equals new { n.PLANT_ID, cust = n.B2B_ID } into tmp
                                    from nu in tmp.DefaultIfEmpty() 
                                    select new PartData
                                    {
                                        Part = p,
                                        Program = m,
                                        Used = u,
                                        CustomerPlant = l,
                                        PartnerCompany = c,
                                        Notify = nu
                                    }).ToList();
                    else
                        partList = (from p in entities.PART
                                    join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                    join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                    where (p.COMPANY_ID == companyID)
                                    from m in m_p.DefaultIfEmpty()
                                    from u in u_p.DefaultIfEmpty()
                                    where u.PLANT_ID == plantID && u.CUST_PLANT_ID != null
                                    join l in entities.PLANT on u.CUST_PLANT_ID equals l.PLANT_ID into l_u
                                    from l in l_u.DefaultIfEmpty()
                                    join c in entities.COMPANY on l.COMPANY_ID equals c.COMPANY_ID into c_l
                                    from c in c_l.DefaultIfEmpty()
                                    join n in entities.NOTIFY on new { u.PLANT_ID, cust =u.CUST_PLANT_ID } equals new {n.PLANT_ID, cust = n.B2B_ID } into tmp 
                                    from nu in tmp.DefaultIfEmpty() 
                                    select new PartData
                                    {
                                        Part = p,
                                        Program = m,
                                        Used = u,
                                        CustomerPlant = l,
                                        PartnerCompany = c,
                                        Notify = nu
                                    }).GroupBy(g => new { g.Used.CUST_PLANT_ID }).Select(l => l.FirstOrDefault()).ToList();
                    break;
                case 2:
                    if (partnerID > 0)
                        partList = (from p in entities.PART
                                    join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                    join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                    where (p.COMPANY_ID == companyID)
                                    from m in m_p.DefaultIfEmpty()
                                    from u in u_p.DefaultIfEmpty()
                                    where u.PLANT_ID == plantID && u.SUPP_PLANT_ID == partnerID 
                                    join l in entities.PLANT on u.SUPP_PLANT_ID equals l.PLANT_ID into l_u
                                    from l in l_u.DefaultIfEmpty()
                                    join c in entities.COMPANY on l.COMPANY_ID equals c.COMPANY_ID into c_l
                                    from c in c_l.DefaultIfEmpty()
                                    join n in entities.NOTIFY on new { u.PLANT_ID, cust = u.SUPP_PLANT_ID } equals new { n.PLANT_ID, cust = n.B2B_ID } into tmp
                                    from nu in tmp.DefaultIfEmpty() 
                                    select new PartData
                                    {
                                        Part = p,
                                        Program = m,
                                        Used = u,
                                        SupplierPlant = l,
                                        PartnerCompany = c,
                                        Notify = nu 
                                    }).ToList();
                    else
                        partList = (from p in entities.PART
                                    join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                    join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                    where (p.COMPANY_ID == companyID)
                                    from m in m_p.DefaultIfEmpty()
                                    from u in u_p.DefaultIfEmpty()
                                    where u.PLANT_ID == plantID && u.SUPP_PLANT_ID != null
                                    join l in entities.PLANT on u.SUPP_PLANT_ID equals l.PLANT_ID into l_u
                                    from l in l_u.DefaultIfEmpty()
                                    join c in entities.COMPANY on l.COMPANY_ID equals c.COMPANY_ID into c_l
                                    from c in c_l.DefaultIfEmpty()
                                    join n in entities.NOTIFY on new { u.PLANT_ID, cust = u.SUPP_PLANT_ID } equals new { n.PLANT_ID, cust = n.B2B_ID } into tmp
                                    from nu in tmp.DefaultIfEmpty() 
                                    select new PartData
                                    {
                                        Part = p,
                                        Program = m,
                                        Used = u,
                                        SupplierPlant = l,
                                        PartnerCompany = c,
                                        Notify = nu 
                                    }).GroupBy(g => new { g.Used.SUPP_PLANT_ID }).Select(l => l.FirstOrDefault()).ToList();
                    break;
                default:
                    partList = (from p in entities.PART
                                join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                where (p.COMPANY_ID == companyID)
                                from m in m_p.DefaultIfEmpty()
                                from u in u_p.DefaultIfEmpty()
                                select new PartData
                                {
                                    Part = p,
                                    Program = m,
                                    Used = u
                                }).ToList();
                    break;
            }

            return partList;
        }

        public static List<PartData> SelectPartDataList(PSsqmEntities entities, decimal companyID, decimal partID, decimal programID, decimal plantID, int relationshipType)
        {
            List<PartData> partList = new List<PartData>();

            try
            {
                if (partID > 0)
                {
                    // get all occurences of specific part (where used)
                    partList = (from p in entities.PART
                                join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                where (p.COMPANY_ID == companyID && p.PART_ID == partID)
                                from m in m_p.DefaultIfEmpty()
                                from u in u_p.DefaultIfEmpty()
                                select new PartData
                                {
                                    Part = p,
                                    Program = m,
                                    Used = u
                                }).ToList();
                }
                else if (programID > 0)
                {
                    // get all parts for a specific program
                    partList = (from p in entities.PART 
                                join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID
                                join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                where (p.COMPANY_ID == companyID && p.PROGRAM_ID == programID)
                                from u in u_p.DefaultIfEmpty()
                                select new PartData
                                {
                                    Part = p,
                                    Program = m,
                                    Used = u
                                }).ToList();
                }
                else
                {
                    // get all parts
                    partList = (from p in entities.PART
                                join m in entities.PART_PROGRAM on p.PROGRAM_ID equals m.PROGRAM_ID into m_p
                                join u in entities.STREAM on p.PART_ID equals u.PART_ID into u_p
                                where (p.COMPANY_ID == companyID)
                                from m in m_p.DefaultIfEmpty()
                                from u in u_p.DefaultIfEmpty()
                                select new PartData
                                {
                                    Part = p,
                                    Program = m,
                                    Used = u
                                }).ToList();
                }

                if (plantID > 0)
                {
                    partList = partList.FindAll(l => l.Used != null  &&  (l.Used.PLANT_ID == plantID  ||  l.Used.SUPP_PLANT_ID == plantID  ||  l.Used.CUST_PLANT_ID == plantID));
                }

 
                switch (relationshipType)
                {
                    case 0:  // internal
                        partList = partList.Where(l => l.Used.SUPP_PLANT_ID == null).ToList();
                        break;
                    case 1:  // customer
                        partList = partList.Where(l => l.Used.CUST_PLANT_ID != null).ToList();
                        break;
                    case 2:  // supplier
                        partList = partList.Where(l => l.Used.SUPP_PLANT_ID != null).ToList();
                        break;
                    default:
                        break;
                }
 
                decimal seq = 0;
                foreach (PartData part in partList)
                {
                    part.ListSeq = ++seq;
                }
            }
            catch
            {
            }

            return partList;
        }
        
        public static PART LookupPart(SQM.Website.PSsqmEntities ctx, decimal partID, string partNum, decimal companyID, bool createNew)
        {
            PART part = null;
            try
            {
                if (partID > 0)
                {
                    if (companyID == 0)
                            part = (from pt in ctx.PART
                                where (pt.PART_ID == partID)
                                select pt).Single();
                        else
                            part = (from pt in ctx.PART
                                where (pt.PART_ID == partID) && (pt.COMPANY_ID == companyID)
                                select pt).Single();
                }
                else
                    part = (from pt in ctx.PART
                            where (pt.PART_NUM == partNum) && (pt.COMPANY_ID == companyID)
                            select pt).Single();
            }
            catch
            {
                if (createNew)
                {
                    part = new PART();
                    part.PART_NUM = partNum;
                    part.COMPANY_ID = companyID;
                }
            }

            return part;
        }

        public static PART CreatePart(decimal companyID, string updateBy)
        {
            PART part = new PART();
            part.COMPANY_ID = companyID;
            part = (PART)SQMModelMgr.SetObjectTimestamp((object)part, updateBy, part.EntityState);
            return part;
        }

        public static STREAM CreatePartStream(decimal companyID, decimal partID)
        {
            STREAM stream = new STREAM();
            stream.COMPANY_ID = companyID;
            stream.PART_ID = partID;
            stream.UNIT_COST = 0;
            return stream;
        }

        public static PART UpdatePart(SQM.Website.PSsqmEntities ctx, PART part, string updateBy)
        {
            if (part.EntityState == EntityState.Detached)
            {
                part.LAST_UPD_DT = part.CREATE_DT = DateTime.UtcNow;
                part.LAST_UPD_BY = part.CREATE_BY = updateBy;
                ctx.AddToPART(part);
            }
            part = (PART)SQMModelMgr.SetObjectTimestamp((object)part, updateBy, part.EntityState);
            ctx.SaveChanges();
            return part;
        }

        public static STREAM UpdatePartStream(SQM.Website.PSsqmEntities ctx, STREAM stream, string updateBy)
        {
            if (stream.EntityState == EntityState.Detached)
                ctx.AddToSTREAM(stream);
            stream = (STREAM)SQMModelMgr.SetObjectTimestamp((object)stream, updateBy, stream.EntityState);
            ctx.SaveChanges();
            return stream;
        }

        public static string GetFullPartNumber(PART part)
        {
            return GetFullPartNumber(part.PART_NUM, part.PART_PREFIX, part.PART_SUFFIX, part.PART_NUM_SEPARATOR);
        }

        public static string GetFullPartNumber(string partNum, string partPrefix, string partSuffix, string partSeparator)
        {
            string fullPartNo = "";
            if (!string.IsNullOrEmpty(partPrefix))
                fullPartNo = partPrefix;
            if (!string.IsNullOrEmpty(partSeparator) && !string.IsNullOrEmpty(partPrefix))
                fullPartNo += partSeparator;
            fullPartNo += partNum;
            if (!string.IsNullOrEmpty(partSeparator) && !string.IsNullOrEmpty(partSuffix))
                fullPartNo += partSeparator;
            if (!string.IsNullOrEmpty(partSuffix))
                fullPartNo = partSuffix;

            return fullPartNo;
        }

        #endregion

        #region receipt

        public static decimal ReceiptCount(PSsqmEntities ctx)
        {
            decimal count = 0;

            try
            {
                count = (from r in ctx.RECEIPT select r).Count();
            }
            catch { ; }

            return count;
        }

        public static List<ReceiptData> SelectReceiptList(PSsqmEntities ctx, DateTime fromDate, DateTime toDate, decimal[] custPlantIDS, decimal[] suppPlantIDS, decimal[] partIDS)
        {
            List<ReceiptData> receiptList = new List<ReceiptData>();

            try
            {
                if (custPlantIDS.Length > 0 && suppPlantIDS.Length > 0)
                    receiptList = (from r in ctx.RECEIPT
                                   join p in ctx.PART on r.PART_ID equals p.PART_ID 
                                   join cl in ctx.PLANT on r.CUST_LOCATION equals cl.PLANT_ID
                                   join sl in ctx.PLANT on r.SUPP_LOCATION equals sl.PLANT_ID
                                   join o in ctx.QI_OCCUR on r.RECEIPT_NUMBER equals o.REF_OPERATION into ro
                                   where (r.RECEIPT_DT >= fromDate && r.RECEIPT_DT <= toDate
                                       && custPlantIDS.Contains(r.CUST_LOCATION) && suppPlantIDS.Contains(r.SUPP_LOCATION))
                                   select new ReceiptData
                                   {
                                       Receipt = r,
                                       Part = p,
                                       CustomerPlant = cl,
                                       SupplierPlant = sl,
                                       IssueList = new List<QI_OCCUR> { ro.FirstOrDefault() }
                                   }).ToList();
                else if (custPlantIDS.Length > 0)
                    receiptList = (from r in ctx.RECEIPT
                                   join p in ctx.PART on r.PART_ID equals p.PART_ID
                                   join cl in ctx.PLANT on r.CUST_LOCATION equals cl.PLANT_ID
                                   join sl in ctx.PLANT on r.SUPP_LOCATION equals sl.PLANT_ID 
                                   join o in ctx.QI_OCCUR on r.RECEIPT_NUMBER equals o.REF_OPERATION into ro
                                    where (r.RECEIPT_DT >= fromDate && r.RECEIPT_DT <= toDate
                                        && custPlantIDS.Contains(r.CUST_LOCATION))
                                   select new ReceiptData
                                   {
                                       Receipt = r,
                                       Part = p,
                                       CustomerPlant = cl,
                                       SupplierPlant = sl,
                                       IssueList =  new List<QI_OCCUR> {ro.FirstOrDefault()}
                                   }).ToList();
                else if (suppPlantIDS.Length > 0)
                    receiptList = (from r in ctx.RECEIPT
                                   join p in ctx.PART on r.PART_ID equals p.PART_ID
                                   join cl in ctx.PLANT on r.CUST_LOCATION equals cl.PLANT_ID
                                   join sl in ctx.PLANT on r.SUPP_LOCATION equals sl.PLANT_ID
                                   join o in ctx.QI_OCCUR on r.RECEIPT_NUMBER equals o.REF_OPERATION into ro
                                    where (r.RECEIPT_DT >= fromDate && r.RECEIPT_DT <= toDate
                                        &&  suppPlantIDS.Contains(r.SUPP_LOCATION))
                                   select new ReceiptData
                                   {
                                       Receipt = r,
                                       Part = p,
                                       CustomerPlant = cl,
                                       SupplierPlant = sl,
                                       IssueList = new List<QI_OCCUR> { ro.FirstOrDefault() }
                                   }).ToList();
                else
                    receiptList = (from r in ctx.RECEIPT
                                   join p in ctx.PART on r.PART_ID equals p.PART_ID
                                   join cl in ctx.PLANT on r.CUST_LOCATION equals cl.PLANT_ID
                                   join sl in ctx.PLANT on r.SUPP_LOCATION equals sl.PLANT_ID
                                   join o in ctx.QI_OCCUR on r.RECEIPT_NUMBER equals o.REF_OPERATION into ro
                                    where (r.RECEIPT_DT >= fromDate && r.RECEIPT_DT <= toDate)
                                   select new ReceiptData
                                   {
                                       Receipt = r,
                                       Part = p,
                                       CustomerPlant = cl,
                                       SupplierPlant = sl,
                                       IssueList = new List<QI_OCCUR> { ro.FirstOrDefault() }
                                   }).ToList();
               
                if (partIDS.Length > 0)
                    receiptList = receiptList.Where(r => partIDS.Contains(r.Receipt.PART_ID)).ToList();
            }
            catch { ; }

            return receiptList;
        }


        public static RECEIPT LookupReceipt(SQM.Website.PSsqmEntities ctx, decimal receiptID, decimal partID, string receiptNum)
        {
            RECEIPT receipt = null;
            try
            {
                if (receiptID > 0)
                {
                    receipt = (from r in ctx.RECEIPT
                               where (r.RECEIPT_ID == receiptID)
                               select r).Single();
                }
                else
                {
                    if (partID > 0)
                        receipt = (from r in ctx.RECEIPT
                                   where (r.RECEIPT_NUMBER == receiptNum  &&  r.PART_ID == partID)
                                   select r).FirstOrDefault();
                    else
                        receipt = (from r in ctx.RECEIPT
                                   where (r.RECEIPT_NUMBER == receiptNum)
                                   select r).FirstOrDefault();
                }
            }
            catch
            {
                ;
            }

            return receipt;
        }

        public static RECEIPT CreateReceipt(DateTime receiptDate, string receiptNum, decimal custLocID, decimal suppLocID, decimal partID)
        {
            RECEIPT receipt = new RECEIPT();
            receipt.RECEIPT_DT = receiptDate;
            receipt.RECEIPT_NUMBER = receiptNum;
            receipt.CUST_LOCATION = custLocID;
            receipt.SUPP_LOCATION = suppLocID;
            receipt.PART_ID = partID;

            return receipt;
        }

        public static RECEIPT UpdateReceipt(SQM.Website.PSsqmEntities ctx, RECEIPT receipt, string updateBy)
        {
            if (receipt.EntityState == EntityState.Detached)
                ctx.AddToRECEIPT(receipt);
            receipt = (RECEIPT)SQMModelMgr.SetObjectTimestamp((object)receipt, updateBy, receipt.EntityState);

            try
            {
                ctx.SaveChanges();
            }
            catch { receipt = null; }

            return receipt;
        }
        #endregion
    }
}