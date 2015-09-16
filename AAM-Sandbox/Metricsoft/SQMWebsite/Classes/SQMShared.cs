using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;

namespace SQM.Shared
{
    [Serializable]
    public class   DocumentScope  
    {
        public string Scope
        {
            get;
            set;
        }
        public string SessionID
        {
            get;
            set;
        }
        public int RecordType
        {
            get;
            set;
        }
        public decimal CompanyID
        {
            get;
            set;
        }
        public decimal RecordID
        {
            get;
            set;
        }
        public string RecordStep
        {
            get;
            set;
        }
        public decimal?[] DisplayTypes
        {
            get;
            set;
        }
        public DocumentScope CreateNew(string scope, int recordType, string sessionID, decimal recordID, string recordStep)
        {
            this.Scope = scope;
            this.RecordType = recordType;
            this.SessionID = sessionID;
            this.RecordID = recordID;
            this.RecordStep = recordStep;
            this.DisplayTypes = null;
            
            return this;
        }
        public DocumentScope CreateNew(decimal companyID, string scope, int recordType, string sessionID, decimal recordID, string recordStep, params decimal[] args)
        {
            this.CompanyID = companyID;
            this.Scope = scope;
            this.RecordType = recordType;
            this.SessionID = sessionID;
            this.RecordID = recordID;
            this.RecordStep = recordStep;
            this.DisplayTypes = new decimal?[args.Count()];
            for (int n = 0; n < args.Count(); n++)
            {
                this.DisplayTypes[n] = args[n];
            }
            return this;
        }
    }

    public class SearchContext 
    {
        public object ContextType 
        {
            get;
            set;
        }
        public decimal CompanyID
        {
            get;
            set;
        }
        public decimal BusOrgID
        {
            get;
            set;
        }
        public decimal PlantID
        {
            get;
            set;
        }
        public decimal PartID
        {
            get;
            set;
        }
        public string SearchString
        {
            get;
            set;
        }
        public bool ActiveOnly
        {
            get;
            set;
        }
    }
 
    public class B2BPartner
    {
        public decimal CompanyID
        {
            get;
            set;
        }
        public string CompanyName
        {
            get;
            set;
        }
        public string CompanyDunsCode
        {
            get;
            set;
        }
        public decimal BusorgID
        {
            get;
            set;
        }
        public string BusorgName
        {
            get;
            set;
        }
        public string BusorgDunsCode
        {
            get;
            set;
        }
        public decimal PlantID
        {
            get;
            set;
        }
        public string PlantName
        {
            get;
            set;
        }
        public string PlantDunsCode
        {
            get;
            set;
        }
        public string PlantLocation
        {
            get;
            set;
        }
        public string PONumber
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public bool IsSupplier
        {
            get;
            set;
        }
        public bool IsCustomer
        {
            get;
            set;
        }
        public bool IsSelected
        {
            get;
            set;
        }
        public object oObject
        {
            get;
            set;
        }
        public B2BPartner Initialize()
        {
            this.CompanyID = this.CompanyID = this.PlantID = 0;
            oObject = null;
            return this;
        }
    }

    public class LaborType
    {
        public int labor_type_id
        {
            get;
            set;
        }

        public int company_id
        {
            get;
            set;
        }

        public int bus_org_id
        {
            get;
            set;
        }

        public int plant_id
        {
            get;
            set;
        }

        public string labor_name
        {
            get;
            set;
        }

        public string labor_code
        {
            get;
            set;
        }

        public string status
        {
            get;
            set;
        }

        public bool IsNew
        {
            get;
            set;
        }

        public bool IsChanged
        {
            get;
            set;
        }

        public bool IsDelete
        {
            get;
            set;
        }

        public LaborType CreateNew()
        {
            this.IsNew = true;
            this.IsChanged = this.IsDelete = false;
            return (this);
        }
    }

    public class PartUsed
    {

        public int part_id
        {
            get;
            set;
        }

        public string buyer_code
        {
            get;
            set;
        }

        public string supplier_plant_code
        {
            get;
            set;
        }

        public string customer_plant_code
        {
            get;
            set;
        }

        public int supplier_plant_id
        {
            get;
            set;
        }

        public int customer_plant_id
        {
            get;
            set;
        }

        public string status
        {
            get;
            set;
        }

        public DateTime last_upd_dt
        {
            get;
            set;
        }

        public int last_upd_by
        {
            get;
            set;
        }

        public bool IsNew
        {
            get;
            set;
        }

        public bool IsChanged
        {
            get;
            set;
        }

        public bool IsDelete
        {
            get;
            set;
        }

        public PartUsed CreateNew()
        {
            this.IsNew = true;
            this.IsChanged = this.IsDelete = false;
            return (this);
        }
    }


    public class ProblemType
    {
        public string TYPE_LABEL
        {
            get;
            set;
        }

        public string NEW_TYPE_LABEL
        {
            get;
            set;
        }

        public string STATUS
        {
            get;
            set;
        }

        public string INACTIVATED_BY
        {
            get;
            set;
        }
    }

    public class NonConf
    {
        public decimal PR_TYPE_ID
        {
            get;
            set;
        }

        public string NON_CONF_LABEL
        {
            get;
            set;
        }

        public string NEW_NON_CONF_LABEL
        {
            get;
            set;
        }

        public decimal PARENT_NCONF_ID
        {
            get;
            set;
        }

        public string PARENT
        {
            get;
            set;
        }

        public string STATUS
        {
            get;
            set;
        }

        public string INACTIVATED_BY
        {
            get;
            set;
        }
    }

    public class CorrectiveAction
    {
        public string RelatedCause
        {
            get;
            set;
        }
        public int RelatedCauseNo
        {
            get;
            set;
        }
        public string RelatedCauseType
        {
            get;
            set;
        }
        public string Action
        {
            get;
            set;
        }
        public int ActionNo
        {
            get;
            set;
        }
        public string ActionCode
        {
            get;
            set;
        }
        public DateTime EffDate
        {
            get;
            set;
        }
        public string Responsible1
        {
            get;
            set;
        }
        public decimal Responsible1ID
        {
            get;
            set;
        }
        public string Responsible2
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string VerifyStatus
        {
            get;
            set;
        }
        public string VerifyObservations
        {
            get;
            set;
        }
    }

    public class PRTimer
    {
        public string ColName
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
        public bool IsStringValue
        {
            get;
            set;
        }

        public PRTimer CreateNew(string colName, string descr, string value, bool isString)
        {
            this.ColName = colName;
            this.Description = descr;
            this.Value = value;
            this.IsStringValue = isString;
            return (this);
        }
    }

    public class FileReaderError
    {
        public int LineNo
        {
            get;
            set;
        }
        public string Node
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
		public string DataLine
		{
			get;
			set;
		}
        public int Count
        {
            get;
            set;
        }

        public FileReaderError CreateNew(int lineNo, string node, string message, string value, int count, string dataline)
        {
            this.LineNo = lineNo;
            this.Node = node;
            this.Message = message;
            this.Value = value;
            this.Count = count;
			this.DataLine = dataline;
            return this;
        }
    }

    public class FileReaderUpdate
    {
        public string Object
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
        public string Action
        {
            get;
            set;
        }

        public FileReaderUpdate CreateNew(string obj, string value, string action)
        {
            this.Object = obj;
            this.Value = value;
            this.Action = action;
            return this;
        }
    }
}