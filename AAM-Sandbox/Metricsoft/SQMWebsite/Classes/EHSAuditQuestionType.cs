using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQM.Website
{
	public enum EHSAuditQuestionType
	{
		TextField = 1,
		TextBox = 2,
		Radio = 3,
		CheckBox = 4,
		Dropdown = 5,
		Date = 6,
		Time = 7,
		DateTime = 8,
		BooleanCheckBox = 9,
		Attachment = 10,
		CurrencyTextBox = 11,
		PercentTextBox = 12,
		StandardsReferencesDropdown = 13,
		LocationDropdown = 14,
		DocumentAttachment = 15,
		ImageAttachment = 16,
		UsersDropdown = 17,
		RequiredYesNoRadio = 18,
		PageOneAttachment = 19,
		UsersDropdownLocationFiltered = 20,
		RichTextBox = 21,
		RadioPercentage = 22,
		NativeLangTextBox = 24
	}
}