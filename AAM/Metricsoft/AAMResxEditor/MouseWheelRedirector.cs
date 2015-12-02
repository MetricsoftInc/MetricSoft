using System;
using System.Drawing;
using System.Windows.Forms;

namespace AAMResxEditor
{
	/// <summary>
	/// Redirects mouse wheel events to the control under the cursor.
	/// Comes from http://stackoverflow.com/a/4769961
	/// </summary>
	public class MouseWheelRedirector : IMessageFilter
	{
		const int WM_MOUSEWHEEL = 0x020A;

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == WM_MOUSEWHEEL)
			{
				var pos = new Point(m.LParam.ToInt32() & 0xFFFF, m.LParam.ToInt32() >> 16);
				var hWnd = NativeMethods.WindowFromPoint(pos);
				if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
				{
					NativeMethods.SendMessage(hWnd, (uint)m.Msg, m.WParam, m.LParam);
					return true;
				}
			}
			return false;
		}
	}
}
