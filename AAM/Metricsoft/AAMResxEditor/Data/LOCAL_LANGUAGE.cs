using System.ComponentModel.DataAnnotations;

namespace AAMResxEditor.Data
{
	public class LOCAL_LANGUAGE
	{
		[Key]
		public int LANGUAGE_ID { get; set; }
		public string LANGUAGE_CD { get; set; }
		public string ISO_LOCALE { get; set; }
		public string NLS_LANGUAGE { get; set; }
		public string NLS_TERRITORY { get; set; }
		public string LANGUAGE_NAME { get; set; }
		public string STATUS { get; set; }
	}
}
