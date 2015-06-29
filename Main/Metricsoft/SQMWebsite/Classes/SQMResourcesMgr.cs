using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Reflection;
using SQM.Shared;

namespace SQM.Website
{

    public class SQMResourcesMgr
    {

        public static int updateStatus;
        public static Exception updateException;


        public static List<CURRENCY> SelectCurrencyList()
        {
            List<CURRENCY> currencyList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                        currencyList = (from c in ctx.CURRENCY 
                                   select c).OrderBy(l => l.CURRENCY_CODE).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return currencyList;
        }

        public static List<UN_DISPOSAL> SelectDisposalCodeList(bool activeOnly)
        {
            List<UN_DISPOSAL> codeList = new List<UN_DISPOSAL>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                if (activeOnly)
                    codeList = (from c in entities.UN_DISPOSAL where c.STATUS == "A" select c).ToList();
                else
                    codeList = (from c in entities.UN_DISPOSAL select c).ToList();
            }

            return codeList;
        }

        public static List<EFM_REFERENCE> SelectEFMTypeList()
        {
            List<EFM_REFERENCE> typeList = new List<EFM_REFERENCE>();

            using (PSsqmEntities entities = new PSsqmEntities())
            {
                typeList = (from c in entities.EFM_REFERENCE select c).ToList();
            }

            return typeList;
        }

        #region measures

        public static List<UOM> GetCompanyStdUnits(decimal companyID)
        {
            List<UOM> stdUOMList = new List<UOM>();

            Dictionary<string, string> dcXlat =  WebSiteCommon.GetXlatList("stdUnits", "", "short");
            foreach (KeyValuePair<string, string> xItem in dcXlat)
            {
                UOM uom = new UOM();
                uom.UOM_CATEGORY = xItem.Key;
                uom.UOM_CD = xItem.Value;
                decimal id;
                if (decimal.TryParse(xItem.Value, out id))
                    uom.UOM_ID = id;
                stdUOMList.Add(uom);
            }

            return stdUOMList;
        }

        public static decimal ZeroIfNull(decimal ? value)
        {
            return value.HasValue ? (decimal)value : 0;
        }

        public static decimal GetStdUOM(string measureCategory, decimal  fromUOM, List<UOM> stdUOMList)
        {
            decimal stdUOM = (decimal)fromUOM;

            UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == fromUOM);
            if (uom != null)
            {
                switch (uom.UOM_CATEGORY)
                {
                    case "ENGY":
                        stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "ENGY").FirstOrDefault().UOM_ID;
                        break;
                    case "WEIT":
                        stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "WEIT").FirstOrDefault().UOM_ID;
                        break;
                    case "VOL":
                        stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "VOL").FirstOrDefault().UOM_ID;
                        break;
                    case "LVOL":        // not currently used 
                        stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "EUTL").FirstOrDefault().UOM_ID;
                        break;
                    case "CUST":    // if a custom uom, use the assumed only convert-to uom from the xref list
                        switch (measureCategory)
                        {
                            case "ENGY":    // energy
                                stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "ENGY").FirstOrDefault().UOM_ID;
                                break;
                            case "EUTL":      // water
                                stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "EUTL").FirstOrDefault().UOM_ID;
                                break;
                            default:    // waste streams
                                stdUOM = stdUOMList.Where(u => u.UOM_CATEGORY == "WEIT").FirstOrDefault().UOM_ID;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            return stdUOM;
        }

        public static double ConvertUOM(string measureCategory, decimal fromUOM, double fromValue, decimal? UOMFactor, List<UOM> stdUOMList, out decimal useUOM)
        {
            double toValue = fromValue;
            decimal toUOM = useUOM = fromUOM;

            useUOM = toUOM = GetStdUOM(measureCategory, fromUOM, stdUOMList);

            if (UOMFactor != null && UOMFactor != 0)
            {
                toValue = fromValue * (double)UOMFactor;
            }
            else
            {
                toValue = ConvertUOM(fromUOM, toUOM, fromValue);
            }

            return toValue;
        }

        public static List<UOM> SelectUOMList(string UOMCategory)
        {
            List<UOM> UOMList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    if (string.IsNullOrEmpty(UOMCategory))
                        UOMList = (from u in ctx.UOM.Include("UOM_XREF") 
                                   select u).OrderBy(l => l.UOM_CATEGORY).ThenBy(l => l.UOM_CD).ToList();
                    else
                        UOMList = (from u in ctx.UOM.Include("UOM_XREF") 
                                   where (u.UOM_CATEGORY == UOMCategory)
                                   select u).OrderBy(l => l.UOM_CD).ToList();
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return UOMList;
        }

        public static UOM UpdateUOM(PSsqmEntities entities, UOM uom, decimal convertToUOMID, double conversionFactor)
        {
            UOM ret;

            try
            {
                if (uom.UOM_ID > 0)
                {
                    ret = (from u in entities.UOM.Include("UOM_XREF") where u.UOM_ID == uom.UOM_ID select u).Single();
                    if (ret != null)
                    {
                        ret = (UOM)SQMModelMgr.CopyObjectValues(ret, uom, false);
                        ret.UOM_XREF.FirstOrDefault().UOM_TO = convertToUOMID;
                        ret.UOM_XREF.FirstOrDefault().CONVERSION = Convert.ToDecimal(conversionFactor);
                        entities.SaveChanges();
                    }
                }
                else
                {
                    entities.AddToUOM(uom);
                    entities.SaveChanges();
                    if (convertToUOMID > 0 && uom.UOM_ID > 0)
                    {
                        UOM_XREF xref = new UOM_XREF();
                        xref.UOM_FROM = uom.UOM_ID;
                        xref.UOM_TO = convertToUOMID;
                        xref.CONVERSION = Convert.ToDecimal(conversionFactor);
                        xref.OPERATOR = "MULT";
                        entities.AddToUOM_XREF(xref);
                        entities.SaveChanges();
                    }
                }

                ret = uom;
            }
            catch
            {
                ret = null;
            }

            return ret;
        }

        public static double ConvertUOM(decimal fromUOM, decimal toUOM, double fromValue)
        {
            return ConvertUOM(fromUOM, toUOM, fromValue, false);
        }

        public static double ConvertUOM(decimal fromUOM, decimal toUOM, double fromValue, bool allowSameUOM)
        {
            double toValue = fromValue;

            if (fromUOM > 0 &&  (allowSameUOM || fromUOM != toUOM))
            {
                UOM uom = SessionManager.UOMList.FirstOrDefault(l => l.UOM_ID == fromUOM);
                UOM_XREF xref = uom.UOM_XREF.FirstOrDefault(x => x.UOM_FROM == fromUOM && x.UOM_TO == toUOM);
                if (xref != null)
                {
                    toValue = fromValue * (double)xref.CONVERSION;
                }
            }

            return toValue;
        }



        // EHS

        // QUALITY

        public static List<SQM_MEASURE> SelectSQMMeasureSubCategoryList(string measureCategory)
        {
            List<SQM_MEASURE> subList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    subList = (from m in ctx.SQM_MEASURE
                               where (m.MEASURE_SUBCATEGORY == null)
                               select m).OrderBy(l => l.MEASURE_CATEGORY).ThenBy(l => l.MEASURE_CD).ToList();

                    if (!string.IsNullOrEmpty(measureCategory))
                        subList = subList.ToList().FindAll(l => l.MEASURE_CATEGORY == measureCategory);
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return subList;
        }

        public static List<SQM_MEASURE> SelectSQMMeasureList(string measureCategory, bool activeOnly)
        {
            List<SQM_MEASURE> measList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    if (string.IsNullOrEmpty(measureCategory))
                        measList = (from m in ctx.SQM_MEASURE
                                    where (m.MEASURE_SUBCATEGORY != null)
                                    select m).OrderBy(l => l.MEASURE_CATEGORY).ThenBy(l => l.MEASURE_CD).ToList();
                    else
                        measList = (from m in ctx.SQM_MEASURE
                                    where (m.MEASURE_SUBCATEGORY != null && m.MEASURE_CATEGORY == measureCategory)
                                    select m).OrderBy(l => l.MEASURE_CD).ToList();

                    if (activeOnly)
                        measList = measList.ToList().FindAll(l => l.STATUS == "A");
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return measList;
        }

        public static SQM_MEASURE LookupSQMMeasure(SQM.Website.PSsqmEntities ctx, decimal measureID, string measureCode)
        {
            SQM_MEASURE measure = null;

            if (measureID == 0)
                measure = (from m in ctx.SQM_MEASURE
                           where (m.MEASURE_CD.ToUpper() == measureCode.ToUpper())
                           select m).Single();
            else
                measure = (from m in ctx.SQM_MEASURE
                           where (m.MEASURE_ID == measureID)
                           select m).Single();

            return measure;
        }

        public static SQM_MEASURE UpdateSQMMeasure(SQM.Website.PSsqmEntities ctx, SQM_MEASURE measure, string updateBy)
        {
            try
            {
                measure = (SQM_MEASURE)SQMModelMgr.SetObjectTimestamp((object)measure, updateBy, measure.EntityState);

                if (measure.EntityState == EntityState.Detached || measure.EntityState == EntityState.Added)
                    ctx.AddToSQM_MEASURE(measure);

                if (measure.STATUS == "D")
                    ctx.DeleteObject(measure);

                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return measure;
        }
        #endregion

        #region nonconforms

        public static List<NONCONFORMANCE> SelectNonconfCategoryList(string problemArea)
        {
            List<NONCONFORMANCE> catList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    catList = (from m in ctx.NONCONFORMANCE
                               where (m.NONCONF_CATEGORY == null)
                               select m).OrderBy(l => l.PROBLEM_AREA).ThenBy(l => l.NONCONF_CD).ToList();

                    if (!string.IsNullOrEmpty(problemArea))
                        catList = catList.ToList().FindAll(l => l.PROBLEM_AREA == problemArea);
                }
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return catList;
        }

        public static List<NONCONFORMANCE> SelectNonconfList(string problemArea, bool activeOnly)
        {
            List<NONCONFORMANCE> nonconfList = null;
            try
            {
                using (PSsqmEntities ctx = new PSsqmEntities())
                {
                    if (string.IsNullOrEmpty(problemArea))
                        nonconfList = (from m in ctx.NONCONFORMANCE
                                    where (m.NONCONF_CATEGORY != null)
                                    select m).OrderBy(l => l.PROBLEM_AREA).ThenBy(l => l.NONCONF_CD).ToList();
                    else
                        nonconfList = (from m in ctx.NONCONFORMANCE
                                    where (m.NONCONF_CATEGORY != null && m.PROBLEM_AREA == problemArea)
                                    select m).OrderBy(l => l.NONCONF_CD).ToList();

                    if (activeOnly)
                        nonconfList = nonconfList.ToList().FindAll(l => l.STATUS == "A");
                }
            }
            catch (Exception e)
            {
                SQMLogger.LogException(e);
            }

            return nonconfList;
        }

        public static NONCONFORMANCE LookupNonconf(SQM.Website.PSsqmEntities ctx, decimal nonconfID, string nonconfCode)
        {
            NONCONFORMANCE nonconf = null;

            try
            {
                if (nonconfID == 0)
                {
                    nonconf = (from m in ctx.NONCONFORMANCE
                                where (m.NONCONF_CD.ToUpper() == nonconfCode.ToUpper())
                                select m).SingleOrDefault();
                    if (nonconf == null)
                        nonconf = (from m in ctx.NONCONFORMANCE
                                   where (m.NONCONF_NAME.ToUpper() == nonconfCode.ToUpper())
                                   select m).SingleOrDefault();
                }
                else
                    nonconf = (from m in ctx.NONCONFORMANCE
                               where (m.NONCONF_ID == nonconfID)
                               select m).SingleOrDefault();
            }
            catch { }

            return nonconf;
        }

        public static NONCONFORMANCE UpdateNonconf(SQM.Website.PSsqmEntities ctx, NONCONFORMANCE nonconf, string updateBy)
        {
            try
            {
                nonconf = (NONCONFORMANCE)SQMModelMgr.SetObjectTimestamp((object)nonconf, updateBy, nonconf.EntityState);

                if (nonconf.EntityState == EntityState.Detached || nonconf.EntityState == EntityState.Added)
                    ctx.AddToNONCONFORMANCE(nonconf);

                if (nonconf.STATUS == "D")
                    ctx.DeleteObject(nonconf);

                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //SQMLogger.LogException(e);
            }

            return nonconf;
        }
        #endregion
    }
}