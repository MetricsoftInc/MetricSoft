using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Telerik.Web.UI;

namespace SQM.Website
{
    public enum Status { SUCCESS };

	public enum DateIntervalType { fuzzy, year, month, quarter, FYyear, span };
    public enum DateSpanOption { SelectRange, YearToDate, YearOverYear, PreviousYear, FYYearToDate, FYYearOverYear, FYEffTimespan };

    class UserContextError : System.Exception { }
    class CompanyUndefinedError : System.Exception { }
    class BusinessOrgUndefinedError : System.Exception { }
    class PlantUndefinedError : System.Exception { }
    class ParentEntityUndefinedError : System.Exception { }

    public static class WebSiteCommon
    {
        public static string Sanitize(string sourceText, char subChar, bool skip)
        {
            string admitted = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!-_+*'@#$^.()[]& ";
            StringBuilder output = new StringBuilder(sourceText.Length);
            bool found = false;

            foreach (char c in sourceText)
            {
                found = false;
                foreach (char adm in admitted)
                {
                    if (c == adm)
                    {
                        found = true;
                        output.Append(c);
                    }
                }

                if (found == false)
                {
                    if (!skip)
                        output.Append(subChar);
                }
            }

            return output.ToString();
        }

        public static string StripHTML(string htmlString)
        {
            string pattern = @"<(.|\n)*?>";
            return System.Text.RegularExpressions.Regex.Replace(htmlString, pattern, string.Empty);
        }


        public static string Decrypt(string encryptedText, string KeyString)
        {
            try
            {
                string encryptedTextLocal = encryptedText.Replace("===", "+");
                RijndaelManaged aesEncryption = new RijndaelManaged();
                aesEncryption.KeySize = 256;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.ECB;
                aesEncryption.Padding = PaddingMode.ANSIX923;
                byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
                aesEncryption.Key = KeyInBytes;
                ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedTextLocal.ToCharArray(), 0, encryptedTextLocal.Length);
                return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return "";
            }
        }

        public static string Encrypt(string plainStr, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.ECB;
            aesEncryption.Padding = PaddingMode.ANSIX923;
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText).Replace("+", "===");
        }

        public static string EncryptWithKey(string plainStr, string KeyString)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.ECB;
            aesEncryption.Padding = PaddingMode.ANSIX923;
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(KeyString);
            aesEncryption.Key = KeyInBytes;
            byte[] plainText = ASCIIEncoding.UTF8.GetBytes(plainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText).Replace("+", "===");
        }

        public static T Deserialize<T>(this string serialized)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = new StringReader(serialized))
            using (var stm = new XmlTextReader(reader))
                try
                {
                    {
                        return (T)serializer.ReadObject(stm);
                    }
                }
                catch
                {
                    return (T)serializer.ReadObject(stm);
                }
        }

        public static T DBNullToValue<T>(object val)
        {
            Type type = typeof(T);
            TypeCode typeCode = Type.GetTypeCode(type);

            if (val is System.DBNull)
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                        return (T)((object)0);
                    case TypeCode.DateTime:
                        return (T)((object)"01/01/0001");
                    default:
                        return (T)((object)"");
                }
            }
            else
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        val = Convert.ToBoolean(val);
                        break;
                    case TypeCode.Decimal:
                        val = Convert.ToDecimal(val);
                        break;
                    case TypeCode.Double:
                        val = Convert.ToDouble(val);
                        break;
                    case TypeCode.Int16:
                        val = Convert.ToInt16(val);
                        break;
                    case TypeCode.Int32:
                        val = Convert.ToInt32(val);
                        break;
                    case TypeCode.Int64:
                        val = Convert.ToInt64(val);
                        break;
                    case TypeCode.Single:
                        val = Convert.ToSingle(val);
                        break;
                    case TypeCode.DateTime:
                        val = Convert.ToDateTime(val);
                        break;
                    default:
                        val = Convert.ToString(val);
                        break;
                }
            }
            return (T)((object)val);
        }

        public static XmlDocument GetAppSettingsDoc()
        {
            XmlDocument xDoc;

            if (HttpContext.Current.Session["AppSettingsDoc"] == null)
            {
                xDoc = new XmlDocument();
                xDoc.Load(HttpContext.Current.Server.MapPath("/settings.xml"));
                HttpContext.Current.Session["AppSettingsDoc"] = xDoc;
            }
            else
                xDoc = (XmlDocument)HttpContext.Current.Session["AppSettingsDoc"];

            return xDoc;
        }

        public static string PackItemValue(string val0, string val1)
        {
            return val0 + "|" + val1;
        }
        public static string PackItemValue(string val0, string val1, string val3)
        {
            return val0 + "|" + val1 + "|" + val3;
        }

        public static string ParseItemValue(string itemValue)
        {
            string[] vals = itemValue.Split('|');
            string ret = "";

            if (vals.Length > 0)
                ret = vals[vals.Length-1];
 
            return ret;
        }

        public static string[] SplitString(string str, char delimiter)
        {
            try
            {
                string[] vals = str.Split(delimiter);
                return vals;
            }
            catch
            {
                return new string[1] {str};
            }
        }

        public static string FormatDateString(DateTime dtDateTime, bool bApendTime)
        {
            // format date string IF input date is valid
            string strDateTime = "";

            DateTime dateMin = Convert.ToDateTime(GetXlatValue("effDates", "MIN"));
            DateTime dateMax = Convert.ToDateTime(GetXlatValue("effDates", "MAX"));

            if (dtDateTime != null && (dtDateTime > dateMin && dtDateTime < dateMax))
            {
                strDateTime = dtDateTime.ToShortDateString();
                if (bApendTime)
                {
                    strDateTime += "  " + dtDateTime.ToLongTimeString();
                }
            }
            return strDateTime;
        }

        public static DateTime ConvertDateFromString(string strDateTime, DateTime dtDefault)
        {
            // calculate date if input string was valid
            DateTime dtDateTime = dtDefault;

            if (!string.IsNullOrEmpty(strDateTime))
            {
                try
                {
                    dtDateTime = Convert.ToDateTime(strDateTime);
                }
                catch
                { ;}
            }
            return dtDateTime;
        }

        public static DateTime FormatDateCA(DateTime dtDateTime, bool bApendTime)
        {
            // format date string IF input date is valid
            DateTime strDateTime = Convert.ToDateTime("01/01/0001");

            DateTime dateMin = Convert.ToDateTime(GetXlatValue("effDates", "MIN"));
            DateTime dateMax = Convert.ToDateTime(GetXlatValue("effDates", "MAX"));

            if (dtDateTime != null && (dtDateTime > dateMin && dtDateTime < dateMax))
            {
                strDateTime = dtDateTime;
                //if (bApendTime)
                //{
                //    strDateTime += "  " + dtDateTime.ToLongTimeString();
                //}
            }
            return strDateTime;
        }

        public static DateTime ConvertDateFromStringCA(string strDateTime, DateTime dtDefault)
        {
            // calculate date if input string was valid
            DateTime dtDateTime = dtDefault;

            if (!string.IsNullOrEmpty(strDateTime) && !strDateTime.Equals("1/1/0001 12:00:00 AM"))
            {
                try
                {
                    dtDateTime = Convert.ToDateTime(strDateTime);
                }
                catch
                { ;}
            }
            return dtDateTime;
        }

        public static bool IsValidDateString(string strDate)
        {
            bool status = false;
            try
            {
                DateTime theDate = Convert.ToDateTime(strDate);
                status = IsValidDate(theDate);
            }
            catch
            {
            }

            return status;
        }

        public static bool IsValidDate(DateTime theDate)
        {
            bool isValid = false;

            if (theDate != null)
            {
                if (theDate >= Convert.ToDateTime(GetXlatValue("effDates", "MIN")) &&  theDate <= Convert.ToDateTime(GetXlatValue("effDates", "MAX")))
                    isValid = true;
            }

            return isValid;
        }

        public static DateTime CurrentUTCTime()
        {
            return DateTime.UtcNow;
        }

        public static DateTime LocalTime(DateTime utcDate, string localTimeZone)
        {
			// convert UTC time to local based on the local timezone code
            DateTime localDate;

            try
            {
				TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(TimezoneID(localTimeZone));
                localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, tz);
            }
            catch
            {
                localDate = utcDate;
            }

            return localDate;
        }

		public static DateTime ConvertToUTC(DateTime localDate, string localTimeZone)
		{
			// convert local time to UTC based on local timezone code

			return (TimeZoneInfo.ConvertTimeToUtc(localDate, TimeZoneInfo.FindSystemTimeZoneById(TimezoneID(localTimeZone))));
		}

		public static DateTime ConvertFromToTimezone(DateTime dateIN, string tzIN, string tzOUT)
		{
			// convert between two timezones - not necessarily the server time
			// timezoneIN and OUT are time zone id's as returned by the TimezoneID("035") function below
			DateTime dateOUT = new DateTime();

			DateTime utc = TimeZoneInfo.ConvertTimeToUtc(dateIN, TimeZoneInfo.FindSystemTimeZoneById(TimezoneID(tzIN)));
			dateOUT = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(TimezoneID(tzOUT)));

			return dateOUT;
		}

		public static string TimezoneID(string tzValue)
		{
			string tzID = tzValue;

			if (tzValue.Length < 5)
			{
				// convert timezode codes to MSDN timezone ID
				tzID = WebSiteCommon.GetXlatValue("timeZone", tzValue);
			}

			if (string.IsNullOrEmpty(tzID))
				tzID = "GMT Standard Time";  // use UTC as default

			return tzID;
		}


        // period date functions.  period dates span 1st day of start month thru last day of end month
        public static DateTime PeriodFromDate(DateTime startDate)
        {
            DateTime fromDate = DateTime.MinValue;

            if (startDate != null && startDate > DateTime.MinValue)
            {
                fromDate = new DateTime(startDate.Year, startDate.Month, 1);
            }
            return fromDate;
        }
        
        public static DateTime PeriodToDate(DateTime endDate)
        {
            DateTime toDate = DateTime.MinValue;

            if (endDate != null && endDate > DateTime.MinValue)
            {
                toDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
            }
            return toDate;
        }


        public static ListItem[] PopulateDropDownListItems(string strListContext, string xlatOption)
        {
            ListItem[] listItems;

            Dictionary<string, string> dcXlat = WebSiteCommon.GetXlatList(strListContext, "", xlatOption);
            listItems = new ListItem[dcXlat.Count];
            int nItem = -1;
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                ListItem lItem = new ListItem(xItem.Value, xItem.Key);
                listItems[++nItem] = lItem;
            }
            return listItems;
        }

        public static ListItem[] PopulateDropDownListItems(string strListContext)
        {
            return PopulateDropDownListItems(strListContext, 0, "");
        }

        public static ListItem[] PopulateDropDownListItems(string strListContext, int orderOption, string xlatOption)
        {
            ListItem[] listItems;
            Dictionary<string, string> dcXlat = null;
            string shortlong = "short";

            if (xlatOption == "long")
                shortlong = "long";

            if (orderOption == 2)
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong).OrderBy(l => l.Value).ToDictionary(l => l.Key, l => l.Value);
            else if (orderOption == 1)
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong).OrderBy(l => l.Key).ToDictionary(l => l.Key, l => l.Value);
            else 
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong);

            listItems = new ListItem[dcXlat.Count];
            int nItem = -1;
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                ListItem lItem = new ListItem(xItem.Value, xItem.Key);
                listItems[++nItem] = lItem;
            }
            return listItems;
        }

        public static RadComboBoxItem[] PopulateRadListItems(string strListContext, int orderOption, string xlatOption)
        {
            RadComboBoxItem[] listItems;
            Dictionary<string, string> dcXlat = null;
            string shortlong = "short";

            if (xlatOption == "long")
                shortlong = "long";

            if (orderOption == 2)
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong).OrderBy(l => l.Value).ToDictionary(l => l.Key, l => l.Value);
            else if (orderOption == 1)
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong).OrderBy(l => l.Key).ToDictionary(l => l.Key, l => l.Value);
            else
                dcXlat = WebSiteCommon.GetXlatList(strListContext, "", shortlong);

            listItems = new RadComboBoxItem[dcXlat.Count];
            int nItem = -1;
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                RadComboBoxItem lItem = new RadComboBoxItem(xItem.Value, xItem.Key);
                listItems[++nItem] = lItem;
            }
            return listItems;
        }

        public static List<SelectItem> PopulateListItems(string strListContext)
        {
            List<SelectItem> lstItems = new List<SelectItem>();

            Dictionary<string, string> dcXlat = WebSiteCommon.GetXlatList(strListContext, "", "short");
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                SelectItem siItem = new SelectItem().CreateNew(strListContext, xItem.Key, xItem.Value);
                lstItems.Add(siItem);
            }
            return lstItems;
        }

        public static List<RadComboBoxItem> PopulateRadListItems(string strListContext)
        {
            List<RadComboBoxItem> lstItems = new List<RadComboBoxItem>();

            Dictionary<string, string> dcXlat = WebSiteCommon.GetXlatList(strListContext, "", "short");
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                RadComboBoxItem siItem = new RadComboBoxItem(xItem.Value, xItem.Key);
                lstItems.Add(siItem);
            }
            return lstItems;
        }

        public static ListItem[] PopulateDropDownListNums(int intStart, int intCount)
        {
            ListItem[] listItems;

            int nItem = 0;
            listItems = new ListItem[intCount];
            for (int n = intStart; n < (intStart + intCount); n++)
            {
                ListItem lItem = new ListItem(n.ToString(), n.ToString());
                listItems[nItem++] = lItem;
            }

            return listItems;
        }

        public static RadComboBoxItem[] PopulateComboBoxListNums(int intStart, int intCount, string suffix)
        {
            RadComboBoxItem[] listItems;

            int nItem = 0;
            listItems = new RadComboBoxItem[intCount];
            RadComboBoxItem lItem;

            for (int n = intStart; n < (intStart + intCount); n++)
            {
                if (!string.IsNullOrEmpty(suffix))
                    lItem = new RadComboBoxItem((n.ToString() + " " + suffix), n.ToString());
                else 
                    lItem = new RadComboBoxItem(n.ToString(), n.ToString());
                listItems[nItem++] = lItem;
            }

            return listItems;
        }

        public static ListItem[] PopulateDropDownListNums(int intStart, int intCount, int increment)
        {
            ListItem[] listItems;

            int nItem = 0;
            int nCount = 0;
            listItems = new ListItem[intCount/increment];
            for (int n = intStart; n < (intStart + intCount); n++)
            {
                if (++nCount == increment)
                {
                    ListItem lItem = new ListItem(n.ToString(), n.ToString());
                    listItems[nItem++] = lItem;
                    nCount = 0;
                }
            }

            return listItems;
        }

        public static string GetXlatListString(string strListContext, string xlatOption)
        {
            string listString = "";

            Dictionary<string, string> dcXlat = WebSiteCommon.GetXlatList(strListContext, "", xlatOption);
            int nItem = -1;
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                if (++nItem > 0)
                    listString += "|";
                listString += (xItem.Key + "," + xItem.Value);
            }
            return listString;
        }

        public static String GetXlatValue(string xlatContext, string xlatCode)
        {
            return GetXlatValue(xlatContext, xlatCode, "short");
        }

        public static String GetXlatValue(string xlatContext, string xlatCode, string xlatOption)
        {
            // retrieve xlat for a specific code
            string xlatValue = "";

            try
            {
                Dictionary<string, string> dcXlat = new Dictionary<string, string>();
                dcXlat = GetXlatList(xlatContext, xlatCode, xlatOption);
                if (dcXlat != null && dcXlat.Count > 0)
                {
                    dcXlat.TryGetValue(xlatCode, out xlatValue);
                }
            }
            catch
            {
            }

            return xlatValue;
        }

        public static String GetXlatValueLong(string xlatContext, string xlatCode)
        {
            return GetXlatValue(xlatContext, xlatCode, "long");
        }

        public static Dictionary<string, string> GetXlatList(string xlatContext, string xlatCode, string xlatOption)
        {
            // return a list of translate values for a specified field or page control
            // values are matched pairs of codes and text - either short or long(2) strings
            Dictionary<string, string> dcXlat = new Dictionary<string, string>();

            try
            {
                XmlDocument xDoc = GetAppSettingsDoc();
                //xDoc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("LUX.PLXWebSite.Settings.xml"));
                XmlNodeList nodes = xDoc.SelectNodes("/settings/" + xlatContext + "/xlat");
                foreach (XmlNode node in nodes)
                {
                    string attCode = "";
                    foreach (XmlAttribute att in node.Attributes)
                    {
                        if (att.Name.ToLower() == "code")
                        {
                            attCode = att.Value.ToString();
                            // if a specific xlat code was requested, check if this node matches
                            if (!string.IsNullOrEmpty(xlatCode) && xlatCode.ToLower() != attCode.ToLower())
                            {
                                break;
                            }
                        }
                        if (att.Name.ToLower() == xlatOption.ToLower())
                        {
                            dcXlat.Add(attCode, att.Value.ToString());
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
                // handle error
            }

            return dcXlat;
        }

        public static DataSet CreateDataSet<T>(List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            var obj = list[0].GetType();

            //now grab all properties
            var properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            var dataSet = new DataSet();
            var dataTable = new DataTable();

            //now build the columns from the properties
            var columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        public static string GetStatusString(string statusCode)
        {
            return GetXlatValue("statusCode", statusCode);
        }

        public static string GetTimezoneStringLong(string localTimezone)
        {
            return GetXlatValue("timeZone", localTimezone, "long");
        }

        public static string CleanPageName(string pageName)
        {
            // remove all leading "/" and trailing "." chars
            string cleanPageName = pageName;
            //cleanPageName = pageName.Replace("/", "");
            int pos = cleanPageName.LastIndexOf('/');
            if (pos > -1)
                cleanPageName = cleanPageName.Substring(pos+1, pageName.Length - (pos+1));
            if (cleanPageName.Contains("ASP."))
                cleanPageName = cleanPageName.Replace("ASP.", "");
            else
                cleanPageName = cleanPageName.Substring(0,cleanPageName.LastIndexOf('.'));
            return cleanPageName;
        }

        public static string GetPageID(string pageName)
        {
            // return a specific page ID from the <pageName> list
            string attCode = "";

            try
            {
                XmlDocument xDoc = GetAppSettingsDoc();
                XmlNodeList nodes = xDoc.SelectNodes("/settings/pageName/xlat");
                foreach (XmlNode node in nodes)
                {
                    foreach (XmlAttribute att in node.Attributes)
                    {
                        if (att.Name.ToLower() == "code")
                        {
                            attCode = att.Value.ToString();
                        }
                        else
                        {
                            if (att.Value.ToLower() == pageName.ToLower())
                            {
                                return attCode;
                            }
                        }
                    }
                    attCode = "";
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
                // handle error
            }

            return attCode;
        }

        
        // Returns the human-readable file size for an arbitrary, 64-bit file size
        //  The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string GetFileSizeReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (double)(i >> 50);
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (double)(i >> 40);
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (double)(i >> 30);
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (double)(i >> 20);
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (double)(i >> 10);
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = (double)i;
            }
            else
            {
                return i.ToString(sign + "0 B"); // Byte
            }
            readable = readable / 1024;

            return sign + readable.ToString("0.### ") + suffix;
        }

        public static bool OpenReadFile(string fileSpec, out string readStr)
        {
            bool status = false;
            readStr = "";

            FileStream fr = null;

            try
            {
                fr = File.Open(fileSpec, FileMode.Open, FileAccess.Read);
                byte[] b = new byte[12000];

                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                UTF8Encoding temp = new UTF8Encoding(true);

                fr.Read(b, 0, 12000);
                readStr = temp.GetString(b, 0, (int)fr.Length).Trim();
                status = true;
                fr.Close();
            }
            catch
            {
            }

                try
                {
                    if (fr != null)
                        fr.Close();
                }
                catch
                {
                }

            return status;
        }

        public static string FormatID(decimal anyID, int len)
        {
            if (anyID != null)
                return (anyID.ToString().PadLeft(len, '0'));
            else
                return (0.ToString().PadLeft(len, '0'));
        }
        public static string FormatID(decimal anyID, int len, string prefix)
        {
            if (anyID != null)
                return (prefix + anyID.ToString().PadLeft(len, '0'));
            else
                return (prefix + 0.ToString().PadLeft(len, '0'));
        }

        public class SelectItem
        {
            public string Context
            {
                get;
                set;
            }

            public string Value
            {
                get;
                set;
            }

            public string Text
            {
                get;
                set;
            }

            public bool IsDefault
            {
                get;
                set;
            }

            public SelectItem CreateNew(string strContext, string strValue, string strText)
            {
                this.Context = strContext;
                this.Value = strValue;
                this.Text = strText;

                return this;
            }

            public void SetAsDefault()
            {
                this.IsDefault = true;
            }

        }

		public static int RecoverPassword(string emailAddress, string ssoID, string emailSubject, string emailBody1, string emailBody2, string emailBody3)
		{
			int msg = 10;
			string emailBody = "";
			string newPassword = "";
			SQM.Website.PSsqmEntities ctx = new PSsqmEntities();
			// ABW 20140117 - first lookup the person by email, and then the access by sso_id
			//SQM_ACCESS access = SQMModelMgr.LookupCredentialsByEmail(ctx, emailAddress, true);
            PERSON personRecover = null;
            if (!string.IsNullOrEmpty(ssoID))
                personRecover = SQMModelMgr.LookupPerson(ctx, 0, ssoID, false);   // used when calling from user admin page
            else
	            personRecover = SQMModelMgr.LookupPersonByEmail(ctx, emailAddress);  // called from login recover 
           
			if (personRecover != null)
			{
				SQM_ACCESS access = SQMModelMgr.LookupCredentials(ctx, personRecover.SSO_ID, "", true);
				if (access != null)
				{
					//newPassword = System.Web.Security.Membership.GeneratePassword(8, 2);
					newPassword = GeneratePassword(8, 8);
                    string key = SQMModelMgr.GetPasswordKey();
                    access.PASSWORD = Encrypt(newPassword, key);
                    access.STATUS = "P";
                    int upd = ctx.SaveChanges();
                    if (upd != 1)
                    {
                        msg = 30;  // person update error
                    }
                    else 
                    {
					    emailBody = emailBody1 + " " + access.SSO_ID + emailBody2 + " " + newPassword + emailBody3;
					    string emailStatus = SendEmail(emailAddress, emailSubject, emailBody, "");
                        if (emailStatus.Length > 0)
                        {
                            msg = 20;  // email error
                        }
                        else
                        {
                            msg = 0;  /// success
                        }
                    }
				}
			}

			return msg;
		}

		public static string SendEmail(string emailAddress, string emailSubject, string emailBody, string bcc)
		{
			string strStatus = "";
			// ABW 20150826 send emails to a default email if this is a development environment
			string environment = "";
			string altEmail = "";
			try
			{
				environment = System.Configuration.ConfigurationManager.AppSettings["environment"].ToString();
				altEmail = System.Configuration.ConfigurationManager.AppSettings["altEmail"].ToString();
			}
			catch { }

			// ABW 20140805 - get the parameters from the SETTINGS table instead of Web.Config
			//string _mailServer = WebConfigurationManager.AppSettings["MailServer"];
			//string _mailFrom = WebConfigurationManager.AppSettings["MailFrom"];
			//string _mailPassword = WebConfigurationManager.AppSettings["MailPassword"];
			//string _strSSL = "true";
			//bool _mailEnableSsl = true;
			//int _mailSmtpPort = 587;

			//try { _mailSmtpPort = Convert.ToInt16(WebConfigurationManager.AppSettings["MailSmtpPort"]);
			//	  _strSSL = WebConfigurationManager.AppSettings["MailEnableSSL"];
			//	  if (_strSSL.ToLower().Contains("false"))
			//		  _mailEnableSsl = false;
			//}
			//catch { 
			//}

			string _mailServer = "";
			string _mailFrom = "";
			string _mailPassword = "";
			string _strSSL = "true";
			bool _mailEnableSsl = true;
			int _mailSmtpPort = 587;

			List<SETTINGS> MailSettings = SQMSettings.SelectSettingsGroup("MAIL", "");
			SETTINGS setting = new SETTINGS();
			setting = MailSettings.Find(x => x.SETTING_CD == "MailServer");
			if (setting != null)
				_mailServer = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailFrom");
			if (setting != null)
				_mailFrom = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailPassword");
			if (setting != null)
				_mailPassword = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailSMTPPort");
			if (setting != null)
				_mailSmtpPort = Convert.ToInt16(setting.VALUE);
			setting = MailSettings.Find(x => x.SETTING_CD == "MailEnableSSL");
			if (setting != null)
			{
				_strSSL = setting.VALUE;
				if (_strSSL.ToLower().Contains("false"))
					_mailEnableSsl = false;
			}

			try
			{
				MailMessage msg = new MailMessage();
				// ABW 20150826 send emails to a default email if this is a development environment
				if (environment.ToLower().Equals("dev"))
				{
					msg.To.Add(altEmail.Trim());
				}
				else
				{
					msg.To.Add(emailAddress.Trim());
					if (!string.IsNullOrEmpty(bcc))
						msg.Bcc.Add(bcc.Trim());
				}
				msg.From = new MailAddress(_mailFrom);
				msg.Subject = emailSubject;
				msg.Body = emailBody;
				msg.Priority = MailPriority.Normal;
				msg.IsBodyHtml = true;

				SmtpClient client = new SmtpClient();
                if (!string.IsNullOrEmpty(_mailPassword))
                    client.Credentials = new System.Net.NetworkCredential(_mailFrom, _mailPassword);
                else
                    client.UseDefaultCredentials = true;
				client.Port = _mailSmtpPort; // Gmail works on this port
				client.Host = _mailServer;
				client.EnableSsl = _mailEnableSsl;

				client.Send(msg);
			}
			catch (Exception ex)
			{
				//SQMLogger.LogException(ex);
				strStatus = "Error";
			}

			return strStatus;
		}


        public static string SendEmail(string emailAddress, string emailSubject, string emailBody, string cc, string context)
        {
            return SendEmail(emailAddress, emailSubject, emailBody, cc, context, null);
        }

        public static string SendEmail(string emailAddress, string emailSubject, string emailBody, string cc, string context, List<ATTACHMENT> attachList)
        {
            string strStatus = "";
            string _mailServer ="";
			string _mailFrom = "";
			string _mailPassword = "";
			string _strSSL = "true";
            bool _mailEnableSsl = true;
            int _mailSmtpPort = 587;

			// ABW 20150826 send emails to a default email if this is a development environment
			string environment = "";
			string altEmail = "";
			try
			{
				environment = System.Configuration.ConfigurationManager.AppSettings["environment"].ToString();
				altEmail = System.Configuration.ConfigurationManager.AppSettings["altEmail"].ToString();
			}
			catch { }

			// ABW 20140805 - get the parameters from the SETTINGS table instead of Web or App Config
			//if (context == "web")
			//{
			//	_mailServer = WebConfigurationManager.AppSettings["MailServer"];
			//	_mailFrom = WebConfigurationManager.AppSettings["MailFrom"];
			//	_mailPassword = WebConfigurationManager.AppSettings["MailPassword"];
			//	try { _mailSmtpPort = Convert.ToInt16(WebConfigurationManager.AppSettings["MailSmtpPort"]);
			//		  _strSSL = WebConfigurationManager.AppSettings["MailEnableSSL"];
			//		  if (_strSSL.ToLower().Contains("false"))
			//			  _mailEnableSsl = false;
			//	}
			//	catch { }
			//}
			//else
			//{
			//	_mailServer = ConfigurationSettings.AppSettings["MailServer"];
			//	_mailFrom = ConfigurationSettings.AppSettings["MailFrom"];
			//	_mailPassword = ConfigurationSettings.AppSettings["MailPassword"];
			//	try { _mailSmtpPort = Convert.ToInt16(ConfigurationSettings.AppSettings["MailSmtpPort"]);
			//		  _strSSL = WebConfigurationManager.AppSettings["MailEnableSSL"];
			//		  if (_strSSL.ToLower().Contains("false"))
			//			  _mailEnableSsl = false;
			//	}
			//	catch { }
			//}
			List<SETTINGS> MailSettings = SQMSettings.SelectSettingsGroup("MAIL", "");
			SETTINGS setting = new SETTINGS();
			setting = MailSettings.Find(x => x.SETTING_CD == "MailServer");
			if (setting != null)
				_mailServer = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailFrom");
			if (setting != null)
				_mailFrom = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailPassword");
			if (setting != null)
				_mailPassword = setting.VALUE;
			setting = MailSettings.Find(x => x.SETTING_CD == "MailSMTPPort");
			if (setting != null)
				_mailSmtpPort = Convert.ToInt16(setting.VALUE);
			setting = MailSettings.Find(x => x.SETTING_CD == "MailEnableSSL");
			if (setting != null)
			{
				_strSSL = setting.VALUE;
				if (_strSSL.ToLower().Contains("false"))
					_mailEnableSsl = false;
			}

            try
            {
                MailMessage msg = new MailMessage();
				// ABW 20150826 send emails to a default email if this is a development environment
				if (environment.ToLower().Equals("dev"))
				{
					msg.To.Add(altEmail.Trim());
				}
				else
				{
					foreach (string mailto in emailAddress.Split(','))
					{
						msg.To.Add(mailto.Trim());
					}

					if (!string.IsNullOrEmpty(cc))
					{
						foreach (string ccto in cc.Split(','))
						{
							msg.CC.Add(ccto.Trim());
						}
					}
				}
                msg.From = new MailAddress(_mailFrom);
                msg.Subject = emailSubject;
                msg.Body = emailBody;
                msg.Priority = MailPriority.Normal;
                msg.IsBodyHtml = true;

                if (attachList != null && attachList.Count > 0)
                {
                    decimal attachSize = 0;
                    foreach (ATTACHMENT attach in attachList)
                    {
                        attachSize += (decimal)attach.FILE_SIZE;
                        if (attachSize <= 9000000)   // limit to 9 mb
                        {
                            MemoryStream ms = new MemoryStream(SQM.Website.Classes.SQMDocumentMgr.GetAttachmentByteArray(attach.ATTACHMENT_ID));
                            if (ms != null)
                                msg.Attachments.Add(new Attachment(ms, attach.FILE_NAME));
                        }
                    }
                }

                SmtpClient client = new SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential(_mailFrom, _mailPassword);
                if (!string.IsNullOrEmpty(_mailPassword))
                    client.Credentials = new System.Net.NetworkCredential(_mailFrom, _mailPassword);
                else
                    client.UseDefaultCredentials = true;
                client.Port = _mailSmtpPort; // Gmail works on this port
                client.Host = _mailServer;
                client.EnableSsl = _mailEnableSsl;

                client.Send(msg);
            }
            catch (Exception ex)
            {
                //SQMLogger.LogException(ex);
                strStatus = "Error";
            }

            return strStatus;
        }

		public static string GeneratePassword(int minLength, int maxLength)
		{
			if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
				return null;

			string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
			string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
			string PASSWORD_CHARS_NUMERIC = "23456789";
			string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

			// Create a local array containing supported password characters
			// grouped by types. You can remove character groups from this
			// array, but doing so will weaken the password strength.
			char[][] charGroups = new char[][] 
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            PASSWORD_CHARS_SPECIAL.ToCharArray()
        };

			// Use this array to track the number of unused characters in each
			// character group.
			int[] charsLeftInGroup = new int[charGroups.Length];

			// Initially, all characters in each group are not used.
			for (int i = 0; i < charsLeftInGroup.Length; i++)
				charsLeftInGroup[i] = charGroups[i].Length;

			// Use this array to track (iterate through) unused character groups.
			int[] leftGroupsOrder = new int[charGroups.Length];

			// Initially, all character groups are not used.
			for (int i = 0; i < leftGroupsOrder.Length; i++)
				leftGroupsOrder[i] = i;

			// Because we cannot use the default randomizer, which is based on the
			// current time (it will produce the same "random" number within a
			// second), we will use a random number generator to seed the
			// randomizer.

			// Use a 4-byte array to fill it with random bytes and convert it then
			// to an integer value.
			byte[] randomBytes = new byte[4];

			// Generate 4 random bytes.
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(randomBytes);

			// Convert 4 bytes into a 32-bit integer value.
			int seed = (randomBytes[0] & 0x7f) << 24 |
						randomBytes[1] << 16 |
						randomBytes[2] << 8 |
						randomBytes[3];

			// Now, this is real randomization.
			Random random = new Random(seed);

			// This array will hold password characters.
			char[] password = null;

			// Allocate appropriate memory for the password.
			if (minLength < maxLength)
				password = new char[random.Next(minLength, maxLength + 1)];
			else
				password = new char[minLength];

			// Index of the next character to be added to password.
			int nextCharIdx;

			// Index of the next character group to be processed.
			int nextGroupIdx;

			// Index which will be used to track not processed character groups.
			int nextLeftGroupsOrderIdx;

			// Index of the last non-processed character in a group.
			int lastCharIdx;

			// Index of the last non-processed group.
			int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

			// Generate password characters one at a time.
			for (int i = 0; i < password.Length; i++)
			{
				// If only one character group remained unprocessed, process it;
				// otherwise, pick a random character group from the unprocessed
				// group list. To allow a special character to appear in the
				// first position, increment the second parameter of the Next
				// function call by one, i.e. lastLeftGroupsOrderIdx + 1.
				if (lastLeftGroupsOrderIdx == 0)
					nextLeftGroupsOrderIdx = 0;
				else
					nextLeftGroupsOrderIdx = random.Next(0,
														 lastLeftGroupsOrderIdx);

				// Get the actual index of the character group, from which we will
				// pick the next character.
				nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

				// Get the index of the last unprocessed characters in this group.
				lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

				// If only one unprocessed character is left, pick it; otherwise,
				// get a random character from the unused character list.
				if (lastCharIdx == 0)
					nextCharIdx = 0;
				else
					nextCharIdx = random.Next(0, lastCharIdx + 1);

				// Add this character to the password.
				password[i] = charGroups[nextGroupIdx][nextCharIdx];

				// If we processed the last character in this group, start over.
				if (lastCharIdx == 0)
					charsLeftInGroup[nextGroupIdx] =
											  charGroups[nextGroupIdx].Length;
				// There are more unprocessed characters left.
				else
				{
					// Swap processed character with the last unprocessed character
					// so that we don't pick it until we process all characters in
					// this group.
					if (lastCharIdx != nextCharIdx)
					{
						char temp = charGroups[nextGroupIdx][lastCharIdx];
						charGroups[nextGroupIdx][lastCharIdx] =
									charGroups[nextGroupIdx][nextCharIdx];
						charGroups[nextGroupIdx][nextCharIdx] = temp;
					}
					// Decrement the number of unprocessed characters in
					// this group.
					charsLeftInGroup[nextGroupIdx]--;
				}

				// If we processed the last group, start all over.
				if (lastLeftGroupsOrderIdx == 0)
					lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
				// There are more unprocessed groups left.
				else
				{
					// Swap processed group with the last unprocessed group
					// so that we don't pick it until we process all groups.
					if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
					{
						int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
						leftGroupsOrder[lastLeftGroupsOrderIdx] =
									leftGroupsOrder[nextLeftGroupsOrderIdx];
						leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
					}
					// Decrement the number of unprocessed groups.
					lastLeftGroupsOrderIdx--;
				}
			}

			// Convert password characters into a string and return the result.
			return new string(password);

		}

        public static string GetUTC(string code)
        {
            string tz = code;
            int id;

            if (int.TryParse(code, out id))   // check if utc tinezone or abreviation
            {
                return tz;
            }
            
            // convert abreviations to UTC timezones
            switch (code)
            {
                case "A":
                case "BST":
                case "CET":
                case "MEZ":
                case "WEST":
                case "WST":
                    tz = "110";  // GMT + 1
                    break;
                case "CST":
                case "EAST":
                case "HNC":
                    tz = "033";  // GMT - 6
                    break;
                case "I":
                    tz = "190";  // GMT+5:30
                    break;
                default:
                    tz = "GMT";
                    break;
            }

            return tz;
        }

		public static bool FormatCost(string inValue, out double outValue)
		{
			bool status = false;
			outValue = 0.00;
			string tmp = inValue;
			tmp = tmp.Replace("(", "");
			tmp = tmp.Replace(")", "");
			tmp = tmp.Replace("-", "");
			try
			{
				outValue = Convert.ToDouble(tmp);
				status = true;
			}
			catch
			{
				status = false;
			}
			return status;
		}

        public partial class DatePeriod
        {
            public DateTime FromDate
            {
                get;
                set;
            }
            public DateTime ToDate
            {
                get;
                set;
            }
            public string Label
            {
                get;
                set;
            }
        }

		public static List<DatePeriod> CalcDatePeriods(DateTime fromDate, DateTime toDate, DateIntervalType periodType, DateSpanOption dateSpanType, string label)
		{
			List<DatePeriod> periodList = new List<DatePeriod>();
			DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, 1);
			DateTime endDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));
			int numMonths = toDate.Month - fromDate.Month;
			if (toDate.Month < fromDate.Month)
				numMonths = Math.Max(11, ((startDate.Year - endDate.Year) * 12) + startDate.Month - endDate.Month);

			DatePeriod period = null;

			switch (periodType)
			{
				case DateIntervalType.year:
				case DateIntervalType.FYyear:       // assume fromdate is start of FY
					while (startDate < endDate)
					{
						period = new DatePeriod();
						period.FromDate = new DateTime(startDate.Year, startDate.Month, 1);
						period.ToDate = period.FromDate.AddMonths(numMonths);
						period.ToDate = period.ToDate.AddDays(DateTime.DaysInMonth(period.ToDate.Year, period.ToDate.Month) - 1);
						// period.ToDate = startDate.AddMonths(11); period.ToDate = new DateTime(period.ToDate.Year, period.ToDate.Month, DateTime.DaysInMonth(period.ToDate.Year, period.ToDate.Month));
						if (period.ToDate > endDate)
							period.ToDate = endDate;
						period.Label = dateSpanType == DateSpanOption.FYYearOverYear || dateSpanType == DateSpanOption.FYYearToDate || dateSpanType == DateSpanOption.FYEffTimespan ? "FY" + (period.FromDate.Year + 1).ToString() : period.FromDate.Year.ToString();
						periodList.Add(period);
						startDate = startDate.AddMonths(12);
					}
					break;
				case DateIntervalType.span:
					period = new DatePeriod();
					period.FromDate = fromDate;
					period.ToDate = toDate;
					periodList.Add(period);
					break;
				case DateIntervalType.month:
				default:
					while (startDate < endDate)
					{
						period = new DatePeriod();
						period.FromDate = new DateTime(startDate.Year, startDate.Month, 1);
						period.ToDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
						period.Label = period.FromDate.Year.ToString() + "/" + period.FromDate.Month.ToString();
						periodList.Add(period);
						startDate = startDate.AddMonths(1);
					}
					break;
			}

			return periodList;
		}

		/// <summary>
		/// Replaces multiple values in a string with a single call.
		/// </summary>
		/// <typeparam name="T">This can be any type, it should be inferred from the replacements argument.</typeparam>
		/// <param name="origString">The original string.</param>
		/// <param name="replacements">A dictionary of replacements with the keys being the string to replace and the values being what to replace with.</param>
		/// <returns>The string with the replacements made.</returns>
		public static string Replace<T>(this string origString, Dictionary<string, T> replacements)
		{
			string newString = origString;
			foreach (var replacement in replacements)
				newString = newString.Replace(replacement.Key, replacement.Value.ToString());
			return newString;
		}

		/// <summary>
		/// Determines if an flag enum contains a certain flag. NOTE: This makes no checks if an enum is passed in or not.
		/// Comes from: http://www.codeproject.com/Tips/441086/NETs-Enum-HasFlag-and-performance-costs (solution #3)
		/// </summary>
		/// <typeparam name="T">This should be an enum type, but it can technically work with any integer type.</typeparam>
		/// <param name="val">The value being checked for the flag.</param>
		/// <param name="flag">The flag to check for.</param>
		/// <returns>true if the flag is in the value, false otherwise.</returns>
		public static bool Has<T>(this T val, T flag) where T : IConvertible
		{
			var valFlag = Convert.ToUInt64(val);
			var flagFlag = Convert.ToUInt64(flag);
			return (valFlag & flagFlag) == flagFlag;
		}

		/// <summary>
		/// Sets the scale min and max of a gauge definition based on the series data given.
		/// </summary>
		/// <param name="gd">The gauge definition to set the scale of.</param>
		/// <param name="series">The list of series data to base the scale on.</param>
		public static void SetScale(GaugeDefinition gd, List<GaugeSeries> series)
		{
			decimal min = 0, max = 0;
			foreach (var ser in series)
			{
				min = Math.Min(min, ser.ItemList.Min(i => i.YValue) ?? 0);
				max = Math.Max(max, ser.ItemList.Max(i => i.YValue) ?? 0);
			}
			gd.ScaleMin = min == 0 && max == 0 ? 0 : (decimal?)null;
			gd.ScaleMax = min == 0 && max == 0 ? 1 : 0;
		}

		/// <summary>
		/// Loads a Control object from a file based on a specified virtual path, but also casts the result.
		/// </summary>
		/// <typeparam name="T">The Control's type.</typeparam>
		/// <param name="page">The page to load the control for.</param>
		/// <param name="virtualPath">The virtual path to the control's file.</param>
		/// <returns>The Control object that was loaded, cast to <typeparamref name="T" />.</returns>
		public static T LoadControl<T>(this Page page, string virtualPath) where T : Control
		{
			return page.LoadControl(virtualPath) as T;
		}
	}
}