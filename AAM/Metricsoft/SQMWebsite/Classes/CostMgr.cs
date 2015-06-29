using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Reflection;
using SQM.Shared;
using SQM.Website.Classes;

namespace SQM.Website
{
    public enum CostMgrStatus { Normal, NonExist };

    public class CostRptModel
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
        public CostMgrStatus Status
        {
            get;
            set;
        }
        public List<COST_REPORT_ITEM> ItemList
        {
            get;
            set;
        }
    }
}