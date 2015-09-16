using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public class SQMStandardsReferencesMgr
	{
		public static List<STANDARDS_REFERENCES> SelectReferencesByStandard(string standard)
		{
			var refs = new List<STANDARDS_REFERENCES>();
			var entities = new PSsqmEntities();
			refs = (from r in entities.STANDARDS_REFERENCES where r.STANDARD == standard select r).ToList();
			return refs;
		}
	}
}