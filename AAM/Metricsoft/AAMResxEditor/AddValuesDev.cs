using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AAMResxEditor
{
	public partial class AddValuesDev : Form
	{
		public string ControlID
		{
			get { return this.tbControlID.Text; }
		}

		public string ResourceKey
		{
			get { return this.tbResourceKey.Text; }
		}

		public Dictionary<string, string> Values
		{
			get
			{
				return Enumerable.Range(2, this.tlpStrings.RowCount - 2).Select(i => new
				{
					key = (this.tlpStrings.GetControlFromPosition(0, i) as Label).Text.TrimEnd(':'),
					value = (this.tlpStrings.GetControlFromPosition(1, i) as TextBox).Text
				}).ToDictionary(x => x.key, x => x.value);
			}
		}

		MainForm MainForm { get; set; }

		public AddValuesDev(MainForm mainForm, params string[] languages)
		{
			this.InitializeComponent();

			this.MainForm = mainForm;

			int i = 3;
			foreach (var language in languages)
			{
				this.tlpStrings.Controls.Add(new Label()
				{
					Anchor = AnchorStyles.Left,
					AutoSize = true,
					Text = language + ":"
				}, 0, i);
				this.tlpStrings.Controls.Add(new TextBox()
				{
					Anchor = AnchorStyles.Left,
					Size = new Size(200, 20)
				}, 1, i);
				this.tlpStrings.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
				++i;
			}

			this.tlpStrings.RowCount = languages.Length + 3;

			this.Height = 169 + 26 * languages.Length;
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			bool close = true;
			if (string.IsNullOrWhiteSpace(this.tbControlID.Text))
			{
				this.errorProvider.SetError(this.tbControlID, "Control ID is required!");
				close = false;
			}
			if (string.IsNullOrWhiteSpace(this.tbResourceKey.Text))
			{
				this.errorProvider.SetError(this.tbResourceKey, "Resource Key is required!");
				close = false;
			}
			if (string.IsNullOrWhiteSpace(this.tbKey.Text))
			{
				this.errorProvider.SetError(this.tbKey, "Key is required!");
				close = false;
			}
			if (close)
				this.Close();
		}

		bool canceling = false;

		void btnCancel_Click(object sender, EventArgs e)
		{
			this.canceling = true;
			this.Close();
		}

		void AddValuesDev_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.canceling && (string.IsNullOrWhiteSpace(this.tbControlID.Text) || string.IsNullOrWhiteSpace(this.tbResourceKey.Text) || string.IsNullOrWhiteSpace(this.tbKey.Text)))
				e.Cancel = true;
		}
	}
}
