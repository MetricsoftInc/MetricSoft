using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SQM.Website
{
	public class SQMSettings
	{
		public static string SettingsFile = "settings.xml"; //should probably go in web.config eventually

	   // private List<String> currencyCode;

		public static List<Settings> TimeZone
		{
			get
			{
				List<Settings> codes = (from c in XDocument.Load(HttpContext.Current.Server.MapPath("~/" + SettingsFile)).Descendants("timeZone").Descendants("xlat")
										select new Settings
										{
											code = c.Attribute("code").Value,
											short_desc = c.Attribute("short").Value,
											long_desc = c.Attribute("long").Value
										}).ToList();
				return codes;
			}
		}

		public static List<Settings> Status
		{
			get
			{
				List<Settings> codes = (from c in XDocument.Load(HttpContext.Current.Server.MapPath("~/" + SettingsFile)).Descendants("statusCode").Descendants("xlat")
										select new Settings
										{
											code = c.Attribute("code").Value,
											short_desc = c.Attribute("short").Value,
											long_desc = c.Attribute("long").Value
										}).ToList();
				return codes;
			}
		}

		public static List<SysModule> SystemModuleItems()
		{
			List<SysModule> moduleList = (from c in XDocument.Load(HttpContext.Current.Server.MapPath("~/" + SettingsFile)).Descendants("sysModule").Descendants("sys")
										select new SysModule 
										{
											prod = c.Attribute("prod").Value,
                                            mod = c.Attribute("mod").Value,
											topic = c.Attribute("topic").Value,
											role = c.Attribute("role").Value,
											desc = c.Attribute("desc").Value
										}).ToList();
				return moduleList;
		}

		public static List<SETTINGS> SelectSettingsGroup(string settingGroup, string settingFamily)
		{
			List<SETTINGS> settingsList = null;
			
			using (PSsqmEntities entities = new PSsqmEntities())
			{
				try
				{
					if (string.IsNullOrEmpty(settingFamily))
					{
						if (settingGroup.Contains("*"))  // wildcard search
						{
							string groupLike = settingGroup.Substring(0, settingGroup.Length - 1).ToUpper();
							settingsList = (from s in entities.SETTINGS
											where (s.SETTING_GROUP.ToUpper().Contains(groupLike))
											orderby s.SETTING_CD
											select s).ToList();
						}
						else
						{
							settingsList = (from s in entities.SETTINGS
											where (s.SETTING_GROUP.ToUpper() == settingGroup.ToUpper())
											orderby s.SETTING_CD
											select s).ToList();
						}
					}
					else
					{
						settingsList = (from s in entities.SETTINGS
										where (s.SETTING_FAMILY.ToUpper() == settingFamily.ToUpper())
										orderby s.SETTING_GROUP,s.SETTING_CD
										select s).ToList();
					}
				}
				catch (Exception ex)
				{
					//SQMLogger.LogException(ex);
				}
			}
			return settingsList;
		}
		
		public static List<SETTINGS> SelectSettingsGroupExposed(string settingGroup, string settingFamily)
		{
			List<SETTINGS> settingsList = null;

			using (PSsqmEntities entities = new PSsqmEntities())
			{
				try
				{
					if (string.IsNullOrEmpty(settingFamily))
					{
						if (string.IsNullOrEmpty(settingGroup))
						{
							// we need to return a list of all settings when no group or family is specified, but we will only select the settings that are viewable to the customer
							settingsList = (from s in entities.SETTINGS
											where (s.EXPOSE_TO_CLIENT)
											orderby s.SETTING_FAMILY, s.SETTING_GROUP, s.SETTING_CD
											select s).ToList();
						}
						else
						{
							settingsList = (from s in entities.SETTINGS
											where (s.SETTING_GROUP.ToUpper() == settingGroup.ToUpper() && s.EXPOSE_TO_CLIENT)
											orderby s.SETTING_CD
											select s).ToList();
						}
					}
					else
					{
						if (string.IsNullOrEmpty(settingGroup))
						{
							settingsList = (from s in entities.SETTINGS
											where (s.SETTING_FAMILY.ToUpper() == settingFamily.ToUpper() && s.EXPOSE_TO_CLIENT)
											orderby s.SETTING_GROUP, s.SETTING_CD
											select s).ToList();
						}
						else
						{
							settingsList = (from s in entities.SETTINGS
											where (s.SETTING_GROUP.ToUpper() == settingGroup.ToUpper() && s.SETTING_FAMILY.ToUpper() == settingFamily.ToUpper() && s.EXPOSE_TO_CLIENT)
											orderby s.SETTING_GROUP, s.SETTING_CD
											select s).ToList();
						}
					}
				}
				catch (Exception ex)
				{
					//SQMLogger.LogException(ex);
				}
			}
			return settingsList;
		}

		public static SETTINGS SelectSettingByCode(SQM.Website.PSsqmEntities ctx, string settingGroup, string settingFamily, string settingCode)
		{
			SETTINGS setting = null;

				try
				{
					setting = (from s in ctx.SETTINGS
											where (s.SETTING_GROUP.ToUpper() == settingGroup.ToUpper() && s.SETTING_FAMILY.ToUpper() == settingFamily.ToUpper() && s.SETTING_CD.ToUpper() == settingCode.ToUpper())
											orderby s.SETTING_GROUP, s.SETTING_CD
											select s).SingleOrDefault();
				}
				catch (Exception ex)
				{
					//SQMLogger.LogException(ex);
				}
			return setting;
		}

		public static SETTINGS GetSetting(string settingGroup, string settingCode)
		{
			SETTINGS sets;
			try
			{
				sets = SessionManager.UserSettings.Where(s => s.SETTING_GROUP == settingGroup && s.SETTING_CD == settingCode).FirstOrDefault();
			}
			catch
			{
				sets = null;
			}

			return sets;
		}

		public static SETTINGS UpdateSettings(SQM.Website.PSsqmEntities ctx, SETTINGS settings, string updateBy)
		{
			settings = (SETTINGS)SQMModelMgr.SetObjectTimestamp((object)settings, updateBy, settings.EntityState);
			if (settings.EntityState == EntityState.Detached)
				ctx.AddToSETTINGS(settings);

			ctx.SaveChanges();
			return settings;
		}


		public static List<InboxType> InboxTypes()
		{
			List<InboxType> inboxTypeList = (from c in XDocument.Load(HttpContext.Current.Server.MapPath("~/" + SettingsFile)).Descendants("inboxType").Descendants("xlat")
										  select new InboxType
										  {
											  taskCode = c.Attribute("code").Value,
											  taskType = c.Attribute("long").Value
										  }).ToList();
			return inboxTypeList;
		}
   
	}

	public class Settings
	{
		public string group { get; set; }
		public string code { get; set; }
		public string short_desc { get; set; }
		public string long_desc { get; set; }
		public string value { get; set; }
	}

	public class SysModule
	{
		public string prod { get; set; }
        public string mod { get; set; }
		public string topic { get; set; }
		public string role { get; set; }
		public string desc { get; set; }
		public bool view { get; set; }
		public bool update { get; set; }
	}

	public class InboxType
	{
		public string taskCode { get; set; }
		public string taskType { get; set; }
	}

  
}