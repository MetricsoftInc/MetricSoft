using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AAMResxEditor
{
	internal static class NativeMethods
	{
		/// <summary>
		///    Performs a bit-block transfer of the color data corresponding to a
		///    rectangle of pixels from the specified source device context into
		///    a destination device context.
		/// </summary>
		/// <param name="hdc">Handle to the destination device context.</param>
		/// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
		/// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
		/// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
		/// <param name="hdcSrc">Handle to the source device context.</param>
		/// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
		/// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
		/// <param name="dwRop">A raster-operation code.</param>
		/// <returns>
		///    <c>true</c> if the operation succeedes, <c>false</c> otherwise. To get extended error information, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.
		/// </returns>
		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		/// <summary>
		///     Specifies a raster-operation code. These codes define how the color data for the
		///     source rectangle is to be combined with the color data for the destination
		///     rectangle to achieve the final color.
		/// </summary>
		internal enum TernaryRasterOperations : uint
		{
			/// <summary>dest = source</summary>
			SRCCOPY = 0x00CC0020,
			/// <summary>dest = source OR dest</summary>
			SRCPAINT = 0x00EE0086,
			/// <summary>dest = source AND dest</summary>
			SRCAND = 0x008800C6,
			/// <summary>dest = source XOR dest</summary>
			SRCINVERT = 0x00660046,
			/// <summary>dest = source AND (NOT dest)</summary>
			SRCERASE = 0x00440328,
			/// <summary>dest = (NOT source)</summary>
			NOTSRCCOPY = 0x00330008,
			/// <summary>dest = (NOT src) AND (NOT dest)</summary>
			NOTSRCERASE = 0x001100A6,
			/// <summary>dest = (source AND pattern)</summary>
			MERGECOPY = 0x00C000CA,
			/// <summary>dest = (NOT source) OR dest</summary>
			MERGEPAINT = 0x00BB0226,
			/// <summary>dest = pattern</summary>
			PATCOPY = 0x00F00021,
			/// <summary>dest = DPSnoo</summary>
			PATPAINT = 0x00FB0A09,
			/// <summary>dest = pattern XOR dest</summary>
			PATINVERT = 0x005A0049,
			/// <summary>dest = (NOT dest)</summary>
			DSTINVERT = 0x00550009,
			/// <summary>dest = BLACK</summary>
			BLACKNESS = 0x00000042,
			/// <summary>dest = WHITE</summary>
			WHITENESS = 0x00FF0062,
			/// <summary>
			/// Capture window as seen on screen.  This includes layered windows
			/// such as WPF windows with AllowsTransparency="true"
			/// </summary>
			CAPTUREBLT = 0x40000000
		}

		[DllImport("user32.dll")]
		internal static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern IntPtr WindowFromPoint(Point Point);
	}
}
