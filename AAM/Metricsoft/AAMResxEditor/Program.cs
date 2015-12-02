using System;
using System.Windows.Forms;

namespace AAMResxEditor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.AddMessageFilter(new MouseWheelRedirector());
			Application.Run(new MainForm());
		}
	}
}
