using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Data;
using SQM.Shared;


namespace SQM.Website
{
	public enum AccessMode { None, Limited, View, Partner, Update, Plant, Admin, SA };
	public enum LoginStatus { Success, SSOUndefined, PasswordMismatch, Inactive, Locked, PersonUndefined, CompanyUndefined, SessionError, SessionInUse};
	public enum SysPriv { sysadmin=1, admin=100, config=200, originate=300, update=320, action=350, approve=380, approve1=381, approve2=382, notify=400, view=500, none=900 }
	public enum SysScope { system, busorg, busloc, dashboard, inbox, envdata, console, incident, prevent, audit, ehsdata }

	public static class CultureSettings
	{
		public static string baseNLS = "en";
		public static string[] gregorianCalendarOverrides = new string[] { "th" };
	}

	public class SessionManager
	{
		public static bool IsLoggedIn()
		{
			bool status = false;

			if (SessionManager.SessionContext != null && SessionManager.UserContext != null)
				status = true;

			return status;
		}

		public static LoginStatus InitializeUser(string SSOID, string pwd, bool activeOnly, bool calcStatOfTheDay)
		{
			LoginStatus sessionStatus = LoginStatus.Success;
		   
			SessionManager.UserContext = new UserContext().Initialize(SSOID, pwd, true);
			if (SessionManager.UserContext.LoginStatus == LoginStatus.Success)
			{
				try
				{
					if (calcStatOfTheDay)
					{
						// only option is currently # days since lost time case 
						SQMMetricMgr stsmgr = new SQMMetricMgr().CreateNew(SessionManager.PrimaryCompany(), "0", DateTime.UtcNow, DateTime.UtcNow, new decimal[1] { SessionManager.UserContext.HRLocation.Plant.PLANT_ID });
						stsmgr.ehsCtl = new EHSCalcsCtl().CreateNew(1, DateSpanOption.SelectRange);
						stsmgr.ehsCtl.ElapsedTimeSeries(new decimal[1] { SessionManager.UserContext.HRLocation.Plant.PLANT_ID }, new decimal[1] { 8 }, new decimal[1] { 63 }, "YES", true);
						if (stsmgr.ehsCtl.Results.ValidResult)
						{
							SessionManager.StatOfTheDay = new AttributeValue().CreateNew("", stsmgr.ehsCtl.Results.metricSeries[0].ItemList[0].YValue ?? 0);
						}
					}
					else
					{
						SessionManager.StatOfTheDay = null;
					}
				}
				catch (Exception ex)
				{
					SessionManager.StatOfTheDay = null;
					//SQMLogger.LogException(ex);
				}
			}
			else
			{
				;
			}

			return SessionManager.UserContext.LoginStatus;
		}

		public static bool CheckUserPrivilege(SysPriv priv,  SysScope scope)
		{
			return UserContext.CheckUserPrivilege(priv, scope);
		}

		public static List<PRIVLIST> GetScopePrivileges(SysScope scope)
		{
			return UserContext.GetScopePrivileges(scope);
		}


	   
		public SessionManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static AttributeValue StatOfTheDay
		{
			get { return ((AttributeValue)HttpContext.Current.Session["STATOFTHEDAY"]); }
			set { HttpContext.Current.Session["STATOFTHEDAY"] = value; }
		}

		public static Object Browser 
		{
			get { return ((object)HttpContext.Current.Session["BROWSER"]); }
			set { HttpContext.Current.Session["BROWSER"] = value; }
		}
		public static string UserAgent
		{
			get { return ((string)HttpContext.Current.Session["USERAGENT"]); }
			set { HttpContext.Current.Session["USERAGENT"] = value; }
		}
		public static int PageWidth
		{
			get 
			{ 
				if (HttpContext.Current.Session["PAGEWIDTH"] == null)
					HttpContext.Current.Session["PAGEWIDTH"] = 0;
				return ((int)HttpContext.Current.Session["PAGEWIDTH"]); 
			}
			set { HttpContext.Current.Session["PAGEWIDTH"] = value; }
		}

		public static string LoginURL
		{
			get { return ((String)HttpContext.Current.Session["LOGIN_URL"]); }
			set { HttpContext.Current.Session["LOGIN_URL"] = value; }
		}

		public static  UserContext UserContext
		{
			get { return ((UserContext)HttpContext.Current.Session["USER_CONTEXT"]); }
			set { HttpContext.Current.Session["USER_CONTEXT"] = value; }
		}
		public static SessionContext SessionContext
		{
			get { return ((SessionContext)HttpContext.Current.Session["SESSION_CONTEXT"]); }
			set { HttpContext.Current.Session["SESSION_CONTEXT"] = value; }
		}

		public static String CurrentMenuItem
		{
			get { return ((String)HttpContext.Current.Session["CURRENT_MENU_ITEM"]); }
			set { HttpContext.Current.Session["CURRENT_MENU_ITEM"] = value; }
		}
		public static String CurrentAdminTab
		{
			get { return ((String)HttpContext.Current.Session["CURRENT_ADMIN_TAB"]); }
			set { HttpContext.Current.Session["CURRENT_ADMIN_TAB"] = value; }
		}
		public static String CurrentSecondaryTab
		{
			get { return ((String)HttpContext.Current.Session["CURRENT_SECONDARY_TAB"]); }
			set { HttpContext.Current.Session["CURRENT_SECONDARY_TAB"] = value; }
		}
		public static String CurrentAdminPage
		{
			get { return ((String)HttpContext.Current.Session["CURRENT_ADMIN_PAGE"]); }
			set { HttpContext.Current.Session["CURRENT_ADMIN_PAGE"] = value; }
		}
		public static PERSON SelectedUser
		{
			get { return ((PERSON)HttpContext.Current.Session["SELECTED_USER"]); }
			set { HttpContext.Current.Session["SELECTED_USER"] = value; }
		}
		public static String SearchCriteria
		{
			get { return ((String)HttpContext.Current.Session["SEARCH_CRITERIA"]); }
			set { HttpContext.Current.Session["SEARCH_CRITERIA"] = value; }
		}
		public static String ExportCriteria
		{
			get { return ((String)HttpContext.Current.Session["EXPORT_CRITERIA"]); }
			set { HttpContext.Current.Session["EXPORT_CRITERIA"] = value; }
		}

		public static BusinessLocation EffLocation
		{
			get { return ((BusinessLocation)HttpContext.Current.Session["SESSION_LOCATION"]); }
			set { HttpContext.Current.Session["SESSION_LOCATION"] = value; }
		}

		public static BusinessLocation B2BLocation
		{
			get { return ((BusinessLocation)HttpContext.Current.Session["B2B_LOCATION"]); }
			set { HttpContext.Current.Session["B2B_LOCATION"] = value; }
		}

		public static BusinessLocation IncidentLocation
		{
			get { return ((BusinessLocation)HttpContext.Current.Session["IncidentLocation"]); }
			set { HttpContext.Current.Session["IncidentLocation"] = value; }
		}

		public static void ClearIncidentLocation()
		{
			try
			{
				HttpContext.Current.Session["IncidentLocation"] = null;
			}
			catch { }
		}

		public static BUSINESS_ORG ParentBusinessOrg
		{
			get { return ((BUSINESS_ORG)HttpContext.Current.Session["PARENT_BUSINESS_ORG"]); }
			set { HttpContext.Current.Session["PARENT_BUSINESS_ORG"] = value; }
		}

		public static PartData Part
		{
			get { return ((PartData)HttpContext.Current.Session["PART"]); }
			set { HttpContext.Current.Session["PART"] = value; }
		}

		public static List<CURRENCY> CurrencyList
		{
			get
			{
				if (HttpContext.Current.Session["CURRENCYLIST"] == null)
				{
					HttpContext.Current.Session["CURRENCYLIST"] = SQMResourcesMgr.SelectCurrencyList().OrderBy(l=> l.CURRENCY_NAME).ToList();
				}
				return ((List<CURRENCY>)HttpContext.Current.Session["CURRENCYLIST"]);
			}
			set { HttpContext.Current.Session["CURRENCYLIST"] = value; }
		}

		public static List<SETTINGS> UserSettings
		{
			get
			{
				if (HttpContext.Current.Session["SETTINGS"] == null)
				{
					HttpContext.Current.Session["SETTINGS"] = SQMSettings.SelectSettingsGroup("", "USER");
					((List<SETTINGS>)HttpContext.Current.Session["SETTINGS"]).AddRange(SQMSettings.SelectSettingsGroup("", "MENU"));
				}
				return ((List<SETTINGS>)HttpContext.Current.Session["SETTINGS"]);
			}
			set { HttpContext.Current.Session["SETTINGS"] = value; }
		}

		public static List<BusinessLocation> PlantList
		{
			get
			{
				if (HttpContext.Current.Session["PLANTLIST"] == null)
				{
					HttpContext.Current.Session["PLANTLIST"] = SQMModelMgr.SelectBusinessLocationList(SessionManager.UserContext.HRLocation.Company.COMPANY_ID, 0, true);
				}
				return ((List<BusinessLocation>)HttpContext.Current.Session["PLANTLIST"]);
			}
			set { HttpContext.Current.Session["PLANTLIST"] = value; }
		}

		public static List<UOM> UOMList
		{
			get
			{
				if (HttpContext.Current.Session["UOMLIST"] == null)
				{
					HttpContext.Current.Session["UOMLIST"] = SQMResourcesMgr.SelectUOMList("");
				}
				return ((List<UOM>)HttpContext.Current.Session["UOMLIST"]);
			}
			set { HttpContext.Current.Session["UOMLIST"] = value; }
		}

		public static List<UN_DISPOSAL> DisposalCodeList
		{
			get
			{
				if (HttpContext.Current.Session["DISPOSALCODELIST"] == null)
				{
					HttpContext.Current.Session["DISPOSALCODELIST"] = SQMResourcesMgr.SelectDisposalCodeList(true);
				}
				return ((List<UN_DISPOSAL>)HttpContext.Current.Session["DISPOSALCODELIST"]);
			}
			set { HttpContext.Current.Session["DISPOSALCODELIST"] = value; }
		}

		public static List<EFM_REFERENCE> EFMList
		{
			get
			{
				if (HttpContext.Current.Session["EFMLIST"] == null)
				{
					HttpContext.Current.Session["EFMLIST"] = SQMResourcesMgr.SelectEFMTypeList();
				}
				return ((List<EFM_REFERENCE>)HttpContext.Current.Session["EFMLIST"]);
			}
			set { HttpContext.Current.Session["EFMLIST"] = value; }
		}

		public static List<UOM> StdUOMList
		{
			get
			{
				if (HttpContext.Current.Session["STDUOMLIST"] == null)
				{
					HttpContext.Current.Session["STDUOMLIST"] = SQMResourcesMgr.GetCompanyStdUnits(1);
				}
				return ((List<UOM>)HttpContext.Current.Session["STDUOMLIST"]);
			}
			set { HttpContext.Current.Session["STDUOMLIST"] = value; }
		}

		public static object ReturnObject
		{
			get
			{
				if (HttpContext.Current.Session["RETURNOBJECT"] != null)
					return ((object)HttpContext.Current.Session["RETURNOBJECT"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["RETURNOBJECT"] = value; }
		}
		public static bool ReturnStatus
		{
			get
			{
				if (HttpContext.Current.Session["RETURNSTATUS"] != null)
					return ((bool)HttpContext.Current.Session["RETURNSTATUS"]);
				else
					return false;
			}
			set { HttpContext.Current.Session["RETURNSTATUS"] = value; }
		}
		public static string ReturnPath
		{
			get
			{
				if (HttpContext.Current.Session["RETURNPATH"] != null)
					return ((string)HttpContext.Current.Session["RETURNPATH"]);
				else
					return "";
			}
			set { HttpContext.Current.Session["RETURNPATH"] = value; }
		}
		public static void ClearReturns()
		{
			try
			{
				HttpContext.Current.Session["RETURNOBJECT"] = null;
				HttpContext.Current.Session["RETURNSTATUS"] = false;
				HttpContext.Current.Session["RETURNPATH"] = null;
			}
			catch { }
		}
		public static decimal ReturnRecordID
		{
			get { return ((decimal)HttpContext.Current.Session["RETURNRECORDID"]); }
			set { HttpContext.Current.Session["RETURNRECORDID"] = value; }
		}
		public static bool HasValue(String value)
		{
			if (HttpContext.Current.Session[value] != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public static DocumentScope DocumentContext
		{
			get { return ((DocumentScope)HttpContext.Current.Session["DOCUMENT_CONTEXT"]); }
			set { HttpContext.Current.Session["DOCUMENT_CONTEXT"] = value; }
		}

		public static object CurrentObject
		{
			get
			{
				if (HttpContext.Current.Session["CURRENTOBJECT"] != null)
					return ((object)HttpContext.Current.Session["CURRENTOBJECT"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["CURRENTOBJECT"] = value; }
		}

		public static object TempObject
		{
			get
			{
				if (HttpContext.Current.Session["TEMPOBJECT"] != null)
					return ((object)HttpContext.Current.Session["TEMPOBJECT"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["TEMPOBJECT"] = value; }
		}

		public static object CurrentDataset
		{
			get
			{
				if (HttpContext.Current.Session["CURRENTDATASET"] != null)
					return ((object)HttpContext.Current.Session["CURRENTDATASET"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["CURRENTDATASET"] = value; }
		}

		public static object CurrentIncident
		{
			get
			{
				if (HttpContext.Current.Session["CURRENTINCIDENT"] != null)
					return ((object)HttpContext.Current.Session["CURRENTINCIDENT"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["CURRENTINCIDENT"] = value; }
		}

		public static object CurrentProblemCase
		{
			get
			{
				if (HttpContext.Current.Session["CURRENTPROBLEMCASE"] != null)
					return ((object)HttpContext.Current.Session["CURRENTPROBLEMCASE"]);
				else
					return null;
			}
			set { HttpContext.Current.Session["CURRENTPROBLEMCASE"] = value; }
		}

		public static void Clear()
		{
			try
			{
				SessionManager.RemoveValue("USER_CONTEXT");
				SessionManager.UserContext = null;
				SessionManager.RemoveValue("SESSION_CONTEXT");
				SessionManager.SessionContext = null;

				SessionManager.RemoveValue("BROWSER");
				SessionManager.Browser = null;
				SessionManager.RemoveValue("USERAGENT");
				SessionManager.UserAgent = null;
				SessionManager.RemoveValue("PAGEWIDTH");

				SessionManager.RemoveValue("SETTINGS");
				SessionManager.UserSettings = null;
				SessionManager.RemoveValue("PLANTLIST");
				SessionManager.PlantList = null;
				SessionManager.RemoveValue("UOMLIST");
				SessionManager.UOMList = null;
				SessionManager.RemoveValue("STDUOMLIST");
				SessionManager.StdUOMList = null;
				SessionManager.RemoveValue("DISPOSALCODELIST");
				SessionManager.DisposalCodeList = null;
				SessionManager.RemoveValue("EFMLIST");
				SessionManager.EFMList = null;
				SessionManager.RemoveValue("CURRENCYLIST");
				SessionManager.CurrencyList = null;
				SessionManager.RemoveValue("RETURNOBJECT");
				SessionManager.ReturnObject = null;
				SessionManager.RemoveValue("CURRENTOBJECT");
				SessionManager.CurrentObject = null;
				SessionManager.RemoveValue("TEMPOBJECT");
				SessionManager.TempObject = null;
				SessionManager.RemoveValue("CURRENTDATASET");
				SessionManager.CurrentDataset = null;
				SessionManager.RemoveValue("CURRENTINCIDENT");
				SessionManager.CurrentIncident = null;
				SessionManager.RemoveValue("CURRENTPROBLEMCASE");
				SessionManager.CurrentProblemCase = null;
				SessionManager.RemoveValue("STATOFTHEDAY");
				SessionManager.StatOfTheDay = null;
				SessionManager.RemoveValue("RETURNPATH");
				SessionManager.ReturnPath = null;
				SessionManager.RemoveValue("LOGIN_URL");
				SessionManager.LoginURL = null;

				SessionManager.ClearReturns();
				HttpContext.Current.Session.Clear();
				HttpContext.Current.Session.RemoveAll();
				HttpContext.Current.Session.Abandon();
			}
			catch
			{
			}

			UserContext = null;
			SessionContext = null;
		}

		public static void RemoveValue(String value)
		{
			HttpContext.Current.Session.Remove(value);
		}

		public static SessionContext CreateSessionContext(UserContext userContext)
		{
			// create session if it doesn't currently exist 
			if (SessionManager.SessionContext == null)
			{
				try
				{
					SessionManager.SessionContext = new SessionContext();

					SessionManager.SessionContext.SetPrimaryCompany(SQMModelMgr.LookupPrimaryCompany(new PSsqmEntities()));   // get the primary company (QAI customer));

					// load the user's preferred language definition
					SessionManager.SessionContext.SetLanguage(userContext.Person.PREFERRED_LANG_ID.HasValue ?  (int)userContext.Person.PREFERRED_LANG_ID : 1);
				}
				catch (Exception e)
				{
					//SQMLogger.LogException(e);
					return null;
				}
 
			}

			return SessionManager.SessionContext;
		}

		public static bool IsUserAgentType(string agentTypes)
		{
			bool isType = false;

			string[] typeList = agentTypes.Split(',');
			foreach (string type in typeList)
			{
				if (UserAgent.ToLower().Contains(type))
				{
					isType = true;
					break;
				}
			}
			return isType;
		}

		public static BusinessLocation SetEffLocation(decimal plantID)
		{
			PLANT plant = SQMModelMgr.LookupPlant(plantID);
			return SetEffLocation(plant);
		}
		public static BusinessLocation SetEffLocation(PLANT plant)
		{
			return (SessionManager.EffLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)plant.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)plant.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)plant.PLANT_ID)) );
		}
		public static BusinessLocation SetEffLocationPlant(PLANT plant)
		{
			if (EffLocation != null  &&  plant != null)
			{
				EffLocation.Plant = plant;
			}
			return SessionManager.EffLocation;
		}

		public static BusinessLocation SetB2BLocation(decimal plantID)
		{
			PLANT plant = SQMModelMgr.LookupPlant(plantID);
			return SetB2BLocation(plant);
		}

		public static BusinessLocation SetIncidentLocation(decimal plantID)
		{
			PLANT plant = SQMModelMgr.LookupPlant(plantID);
			return SetIncidentLocation(plant);
		}


		public static BusinessLocation SetB2BLocation(PLANT plant)
		{
			return (SessionManager.B2BLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)plant.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)plant.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)plant.PLANT_ID)));
		}

		public static BusinessLocation SetIncidentLocation(PLANT plant)
		{
			return (SessionManager.IncidentLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)plant.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)plant.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)plant.PLANT_ID)));
		}

		public static COMPANY PrimaryCompany()
		{
			return SessionManager.SessionContext.PrimaryCompany;
		}

		public static bool IsHRLocationPrimary()
		{
			// does user belong to the primary company ?
			return SessionManager.UserContext.HRLocation.Company.COMPANY_ID == SessionManager.SessionContext.PrimaryCompany.COMPANY_ID ? true : false;
		}
		public static bool IsWorkingLocationPrimary()
		{
			// is the user's working location the primary company ?
			return SessionManager.UserContext.WorkingLocation.Company.COMPANY_ID == SessionManager.SessionContext.PrimaryCompany.COMPANY_ID ? true : false;
		}
		public static bool IsEffLocationPrimary()
		{
			// is the session location company the primary company ?
			return SessionManager.EffLocation.Company.COMPANY_ID == SessionManager.SessionContext.PrimaryCompany.COMPANY_ID ? true : false;
		}

		public static bool PlantAccess(decimal plantID)
		{
			// check if user can access the plant supplied
			if (plantID != null)
				return plantID == UserContext.HRLocation.Plant.PLANT_ID || SessionManager.UserContext.PlantAccessList.Contains(plantID)  ? true : false;
			else
				return false;
		}

		public static DateTime FYStartDate()
		{
			return FYStartDate(DateTime.UtcNow);
		}

		public static DateTime FYStartDate(DateTime targetDate)
		{
			DateTime fyStartDt;

			if (SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH <= 1)
			{
				fyStartDt = new DateTime(targetDate.Year, 1, 1);
			}
			else
			{
				if (targetDate.Month < SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH)
					fyStartDt = new DateTime(targetDate.Year - 1, (int)SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH, 1);
				else
					fyStartDt = new DateTime(targetDate.Year, (int)SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH, 1);
			}

			return fyStartDt;
		}

		public static DateTime FYEndDate(DateTime targetDate)
		{
			DateTime fyEndDt = targetDate;
			
			if (SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH <= 1)
			{
				fyEndDt = new DateTime(targetDate.Year, 12, 31);
			}
			else
			{
				fyEndDt = new DateTime(targetDate.Year, (int)SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH-1, DateTime.DaysInMonth(targetDate.Year, (int)SessionManager.SessionContext.PrimaryCompany.FYSTART_MONTH-1));
			}
		   
			if (fyEndDt > DateTime.UtcNow)
				fyEndDt = DateTime.UtcNow;
			

			return fyEndDt;
		}
	}

	#region usercontext

	public class UserContext
	{
		public SQM_ACCESS Credentials 
		{
			get;
			set;
		}
		public LoginStatus LoginStatus
		{
			set;
			get;
		}
		public PERSON Person
		{
			get;
			set;
		}
		public List<PRIVLIST> PrivList
		{
			get;
			set;
		}
		public BusinessLocation HRLocation
		{
			get;
			set;
		}
		public BusinessLocation WorkingLocation
		{
			get;
			set;
		}
		public List<decimal> PlantAccessList
		{
			get;
			set;
		}
		public string CurrentPwd
		{
			get;
			set;
		}
		public string TimeZoneID
		{
			get;
			set;
		}
		public DateTime LocalTime
		{
			get;
			set;
		}
		public int InboxReviews
		{
			get;
			set;
		}
		//public List<TaskItem> TaskList
		//{
		//	get;
		//	set;
		//}
		//public List<decimal> DelegateList
		//{
		//	get;
		//	set;
		//}
		public string UserName()
		{
			return (this.Person.FIRST_NAME + " " + this.Person.LAST_NAME);
		}

		public UserContext Initialize(string SSOID, string pwd, bool activeOnly)
		{
			SQM.Website.PSsqmEntities ctx = new PSsqmEntities();
			this.LoginStatus = LoginStatus.SSOUndefined;
			SQM_ACCESS access = SQMModelMgr.LookupCredentials(ctx, SSOID, pwd, true);
			if (access != null)
			{
				string key = SQMModelMgr.GetPasswordKey();
				// AW - for now, we want to allow if the password = the password OR the encrypted password
				string password = WebSiteCommon.Decrypt(access.PASSWORD, key);
				//string encrypt = WebSiteCommon.Encrypt(pwd, key);
				//string ss = encrypt;
				/*
				if ((string.IsNullOrEmpty(pwd) && (SSOID.ToLower() != "admin")) || (pwd != password && pwd != access.PASSWORD)) 
					this.LoginStatus = LoginStatus.PasswordMismatch;
				*/
				//if (!string.IsNullOrEmpty(access.PASSWORD)  && (pwd != password && pwd != access.PASSWORD))

				if (pwd != password && pwd != access.PASSWORD) 
					this.LoginStatus = LoginStatus.PasswordMismatch;
				else if (activeOnly && access.STATUS == "I")
					this.LoginStatus = LoginStatus.Inactive;
				else if (access.STATUS == "L")
					this.LoginStatus = LoginStatus.Locked;
				else
				{
					if ((this.Person = SQMModelMgr.LookupPerson(ctx, 0, access.SSO_ID, false)) == null)
					{
						this.LoginStatus = LoginStatus.PersonUndefined;
					}
					else
					{
						this.LoginStatus = LoginStatus.Success;
						this.HRLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)this.Person.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)this.Person.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)this.Person.PLANT_ID));
						
						if (this.HRLocation.Company == null)
						{
							this.LoginStatus = LoginStatus.CompanyUndefined;
						}
						else
						{
							this.PrivList = SQMModelMgr.SelectPrivGroupPerson(this.Person.PRIV_GROUP, "COMMON");

							SessionManager.EffLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)this.Person.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)this.Person.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)this.Person.PLANT_ID));

							this.WorkingLocation = new BusinessLocation();
							this.WorkingLocation = SessionManager.EffLocation;

							this.PlantAccessList = new List<decimal>();
							this.PlantAccessList.Add(this.WorkingLocation.Plant.PLANT_ID);
							if (!string.IsNullOrEmpty(Person.NEW_LOCATION_CD))
							{
								decimal plantID;
								 string[] locs = Person.NEW_LOCATION_CD.Split(',');
								 foreach (string locid in locs)
								 {
									 if (!string.IsNullOrEmpty(locid))
									 {
										 if (decimal.TryParse(locid, out plantID) && plantID != this.WorkingLocation.Plant.PLANT_ID)
											 this.PlantAccessList.Add(plantID);
									 }
								 }
							}

							access.LAST_LOGON_DT = WebSiteCommon.CurrentUTCTime();
							int ctxstatus = 0;
							SQMModelMgr.UpdateCredentials(ctx, access, "", out ctxstatus);
							this.Credentials = access;
							this.TimeZoneID = this.HRLocation.Plant.LOCAL_TIMEZONE;
							this.LocalTime = !string.IsNullOrEmpty(this.TimeZoneID) ? WebSiteCommon.LocalTime(DateTime.UtcNow, this.TimeZoneID) : DateTime.UtcNow;

							if (SessionManager.CreateSessionContext(this) == null)
							{
								this.LoginStatus = LoginStatus.SessionError;
							}

							this.InboxReviews = 0;
						}
					}
				}
			}
			return this;
		}

		public static BusinessLocation LookupHRLocation()
		{
			if ( SessionManager.UserContext.Person != null)
				SessionManager.UserContext.HRLocation = new BusinessLocation().Initialize(SQMModelMgr.LookupCompany((decimal)SessionManager.UserContext.Person.COMPANY_ID), SQMModelMgr.LookupBusOrg((decimal)SessionManager.UserContext.Person.BUS_ORG_ID), SQMModelMgr.LookupPlant((decimal)SessionManager.UserContext.Person.PLANT_ID));
		  
			return SessionManager.UserContext.HRLocation;
		}

		public static bool CheckUserPrivilege(SysPriv[] privList, SysScope scope)
		{
			foreach (SysPriv priv in privList)
			{
				if (CheckUserPrivilege(priv, scope) == true)
				{
					return true;
				}
			}

			return false;
		}

		public static bool CheckUserPrivilege(SysPriv priv, SysScope scope)
		{
			bool hasPriv = false;

			if (SessionManager.UserContext.PrivList != null)
			{
				if (SessionManager.UserContext.PrivList.Where(p => p.PRIV <= 100 && p.SCOPE.ToLower() == SysScope.system.ToString()).FirstOrDefault() != null)  // system admim or company admin has privs to any resource
				{
					hasPriv = true;
				}
				else
				{
					if (priv == SysPriv.approve)
					{
						if (SessionManager.UserContext.PrivList.Where(p => new int[3] { (int)SysPriv.approve, (int)SysPriv.approve1, (int)SysPriv.approve2 }.Contains(p.PRIV) && p.SCOPE.ToLower() == scope.ToString()).FirstOrDefault() != null)  // check for any approval level if base approval priv given
							hasPriv = true;
					}
					else
					{
						if (SessionManager.UserContext.PrivList.Where(p => p.PRIV == (int)priv && p.SCOPE.ToLower() == scope.ToString()).FirstOrDefault() != null)  // check specific priv & scope combination
							hasPriv = true;
					}
				}
			}

			return hasPriv;
		}

		public static List<PRIVLIST> GetScopePrivileges(SysScope scope)
		{
			// get all user privs related to the scope/function 
			List<PRIVLIST> privList = new List<PRIVLIST>();

			if (SessionManager.UserContext.PrivList != null)
			{
				PRIVLIST priv = new PRIVLIST();
				if ((priv = SessionManager.UserContext.PrivList.Where(p => p.PRIV <= 100).FirstOrDefault()) != null)  // system admon or company admin has privs to any resource
				{
					PRIVLIST adminPriv = new PRIVLIST();
					adminPriv.PRIV_GROUP = priv.PRIV_GROUP;
					adminPriv.PRIV = priv.PRIV;
					adminPriv.SCOPE = scope.ToString();
					privList.Add(adminPriv);
				}
				else
				{
					privList = SessionManager.UserContext.PrivList.Where(p => p.SCOPE.ToLower() == scope.ToString()).ToList();
				}
			}

			return privList;
		}

		public static SysPriv GetMaxScopePrivilege(SysScope scope)
		{
			SysPriv maxPriv = SysPriv.none;
			PRIVLIST adminPriv = null;

			if ((adminPriv = SessionManager.UserContext.PrivList.Where(p => p.PRIV <= 100 && p.SCOPE.ToLower() == SysScope.system.ToString()).FirstOrDefault()) != null)  // system admim or company admin has privs to any resource
			{
				maxPriv = (SysPriv)adminPriv.PRIV;
			}
			else 
			{
				foreach (PRIVLIST priv in SessionManager.UserContext.PrivList.Where(l=> l.SCOPE.ToLower() == scope.ToString()).ToList())
				{
					if (priv.PRIV < (int)maxPriv)
						maxPriv = (SysPriv)priv.PRIV;
				}
			}

			return maxPriv;
		}

		public static List<BusinessLocation> FilterPlantAccessList(List<BusinessLocation> locList)
		{
			for (int n = locList.Count - 1; n >= 0; n--)
			{
				if (CheckPlantAdmin(locList[n].Plant.PLANT_ID) == false)
					locList.RemoveAt(n);
			}

			return locList;
		}

		public static bool CheckPlantAdmin(decimal plantID)
		{
			bool access = false;
			if (CheckUserPrivilege(SysPriv.admin, SysScope.system) || SessionManager.PlantAccess(plantID))
				access = true;

			return access;
		}

	}
	#endregion  



	#region sessioncontext

	public class SessionContext : NameObjectCollectionBase
	{
		public COMPANY PrimaryCompany
		{
			get;
			set;
		}
		public SessionContext()
		{
		}
		public bool Add(string key, object obj)
		{
			bool success = false;
			try
			{
				this.BaseAdd(key, obj);
				success = true;
			}
			catch
			{
			}
			return success;
		}

		public bool SetLanguage(int langIDIn)
		{
		   // this.BaseClear(); // !!! clear all app pages when new language - to ensure a refresh from the db
			int langID = langIDIn;
			this.BaseRemove("Language");
			if (langIDIn < 1)
			{
				langID = 1;
			}
			return (this.Add("Language", SQMModelMgr.LookupLanguage(new PSsqmEntities(), "", langID, false)));
		}
		public LOCAL_LANGUAGE Language()
		{
			LOCAL_LANGUAGE lang = null;
			try
			{
				lang = (LOCAL_LANGUAGE)this.BaseGet("Language");
			}
			catch
			{
			}
			return lang;
		}
 
		public void SetPrimaryCompany(COMPANY company)
		{
			this.PrimaryCompany = company;
		}
	}

	public class IndexedSet : NameObjectCollectionBase
	{
		public IndexedSet()
		{
		}
		public bool Add(string key, object obj)
		{
			bool success = false;
			try
			{
				this.BaseAdd(key, obj);
				success = true;
			}
			catch
			{
			}
			return success;
		}
		public bool Remove(string key)
		{
			bool success = false;
			try
			{
				this.BaseRemove(key);
				success = true;
			}
			catch
			{
			}
			return success;
		}
		public object Get(string key)
		{
			object obj = null;
			try
			{
				obj = this.BaseGet(key);
			}
			catch
			{
			}
			return obj;
		}
	}
	#endregion

}