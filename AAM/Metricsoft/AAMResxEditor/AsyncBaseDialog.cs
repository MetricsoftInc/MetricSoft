// Copyright © 2008, Nathan B. Evans
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   - Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   - Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
// OF SUCH DAMAGE.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MRG.Controls.UI;

namespace AAMResxEditor
{
	/// <summary>
	/// Used for dialogs that need to indicate potentially long-duration asynchronous operations to the user.
	/// Comes from http://www.codeproject.com/Articles/24044/AJAX-style-Asynchronous-Progress-Dialog-for-WinFor
	/// </summary>
	[ToolboxItem(false)]
	public class AsyncBaseDialog : Form
	{
		#region Private Fields
		FlickerFreePanel asyncPanel = new FlickerFreePanel();
		LoadingCircle barberPole = new LoadingCircle();
		int reference_count = 0;

		SolidBrush fillBrush = new SolidBrush(Color.FromArgb(225, SystemColors.Window));
		Bitmap background = null;
		#endregion

		/// <summary>
		/// Determines if any asynchronous tasks are still running.
		/// </summary>
		protected bool IsAsyncBusy
		{
			get { return this.reference_count > 0; }
		}

		protected AsyncBaseDialog()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
		}

		void OnPaintAsyncPanel(object sender, PaintEventArgs e)
		{
			if (this.background != null)
				// Paint background image...
				e.Graphics.DrawImageUnscaled(this.background, 0, 0, this.background.Width, this.background.Height);
			else
				// No background available (BeginAsyncOperation was probably called during the subclass constructor)...
				// So just fill with the normal back colour instead...
				e.Graphics.FillRectangle(this.fillBrush, 0, 0, this.asyncPanel.Width, this.asyncPanel.Height);
		}

		#region Begin/End AsyncIndication Methods
		/// <summary>
		/// Begin indicating that an asynchronous operation is occuring.
		/// </summary>
		protected void BeginAsyncIndication()
		{
			// Ensure that we aren't in DesignMode...
			if (this.DesignMode)
				return;

			if (!this.IsAsyncBusy)
			{
				// Capture snapshot of the form...
				if (this.IsHandleCreated)
				{
					// Get DC of the form...
					var srcDc = NativeMethods.GetDC(this.Handle);

					// Create bitmap to store image of form...
					var bmp = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);

					// Create a GDI+ context from the created bitmap...
					using (var g = Graphics.FromImage(bmp))
					{
						// Copy image of form into bitmap...
						var bmpDc = g.GetHdc();
						NativeMethods.BitBlt(bmpDc, 0, 0, bmp.Width, bmp.Height, srcDc, 0, 0, NativeMethods.TernaryRasterOperations.SRCCOPY);

						// Release resources...
						NativeMethods.ReleaseDC(this.Handle, srcDc);
						g.ReleaseHdc(bmpDc);

						Grayscale(bmp);

						// Apply translucent overlay...
						g.FillRectangle(this.fillBrush, 0, 0, bmp.Width, bmp.Height);
					}

					// Store bitmap so that it can be painted by asyncPanel...
					this.background = bmp;
				}

				// Show asyncPanel...
				this.asyncPanel.Visible = this.barberPole.Active = true;
			}

			// Increment reference count...
			++this.reference_count;
		}

		/// <summary>
		/// Ends the current asynchronous indication.
		/// </summary>
		protected void EndAsyncIndication()
		{
			// Decrement reference count if it is still > 0...
			if (this.IsAsyncBusy)
				--this.reference_count;

			// If the reference count == 0 then stop the async indication...
			if (!this.IsAsyncBusy)
			{
				this.asyncPanel.Visible = this.barberPole.Active = false;

				if (this.background != null)
				{
					this.background.Dispose();
					this.background = null;
				}
			}
		}
		#endregion

		#region Min/Max Size and WndProc Filtering
		public override Size MaximumSize
		{
			get
			{
				if (!this.IsAsyncBusy)
					return base.MaximumSize;
				else
					return this.Size;
			}
			set { base.MaximumSize = value; }
		}

		public override Size MinimumSize
		{
			get
			{
				if (!this.IsAsyncBusy)
					return base.MinimumSize;
				else
					return this.Size;
			}
			set { base.MinimumSize = value; }
		}

		protected override void WndProc(ref Message m)
		{
			// Filter out maximize/minimize/restore commands when IsAsyncBusy==TRUE.
			// Also filter out double-clicks of the titlebar to prevent the user min/max'ing that way.
			// Otherwise there will bad bad graphical glitches...
			if (this.IsAsyncBusy)
			{
				if (m.Msg == 0x112 /* WM_SYSCOMMAND */)
				{
					int w = m.WParam.ToInt32();

					if (w == 0xf120 /* SC_RESTORE */ || w == 0xf030 /* SC_MAXIMIZE */ || w == 0xf020 /* SC_MINIMIZE */)
						return; // short circuit

				}
				else if (m.Msg == 0xa3 /* WM_NCLBUTTONDBLCLK */)
					return; // short circuit
			}

			base.WndProc(ref m);
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			if (this.DesignMode)
				return; // prevent controls being added when in design mode

			this.SuspendLayout();

			this.barberPole.BackColor = Color.Transparent;
			this.barberPole.Dock = DockStyle.Fill;
			this.barberPole.Color = SystemColors.ControlDark;
			this.barberPole.RotationSpeed = 35;
			this.barberPole.StylePreset = LoadingCircle.StylePresets.Firefox;

			this.asyncPanel.Dock = DockStyle.Fill;
			this.asyncPanel.Controls.Add(this.barberPole);
			this.Controls.Add(this.asyncPanel);

			this.asyncPanel.BringToFront();

			this.ResumeLayout(false);

			this.asyncPanel.Paint += this.OnPaintAsyncPanel;
		}

		/// <summary>
		/// Used internally as the surface to render the background bitmap on and a container for the barber pole.
		/// </summary>
		sealed class FlickerFreePanel : Control
		{
			public FlickerFreePanel()
			{
				this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
				this.Visible = false;
			}
		}

		#region Run Asynchronous Operation Method and Delegate
		/// <summary>
		/// Queues a method for execution in a non-GUI thread and indicates on the form that an asynchronous operation is occuring.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="afterCallback"></param>
		protected void RunAsyncOperation(Action beforeCallback, Action action, Action afterCallback)
		{
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					this.Invoke((Action)(() =>
					{
						this.Update();
						this.BeginAsyncIndication();
						if (beforeCallback != null)
							beforeCallback.Invoke();
					}));
					action.Invoke();
				}
				finally
				{
					this.Invoke((Action)this.EndAsyncIndication);
					if (afterCallback != null)
						this.Invoke(afterCallback);
				}
			});
		}
		#endregion

		#region Bitmap Manipulation (acquired from http://www.codeproject.com/KB/GDI-plus/csharpfilters.aspx)
		static bool Grayscale(Bitmap b)
		{
			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int size = bmData.Stride * b.Height;
			int nOffset = bmData.Stride - b.Width * 3;

			var data = new byte[size];

			Marshal.Copy(bmData.Scan0, data, 0, size);

			for (int i = 0, y = 0; y < b.Height; ++y, i += nOffset)
				for (int x = 0; x < b.Width; ++x, i += 3)
					data[i] = data[i + 1] = data[i + 2] = (byte)(0.299 * data[i + 2] + 0.587 * data[i + 1] + 0.114 * data[i]);

			Marshal.Copy(data, 0, bmData.Scan0, size);

			b.UnlockBits(bmData);

			return true;
		}
		#endregion
	}
}
