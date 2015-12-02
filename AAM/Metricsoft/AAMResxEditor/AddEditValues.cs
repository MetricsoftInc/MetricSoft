using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AAMResxEditor
{
	public partial class AddEditValues : Form
	{
		public Dictionary<string, string> Values
		{
			get
			{
				var ret = new Dictionary<string, string>()
				{
					{ "Key", this.tbKey.Text }
				};
				for (int i = 1; i < this.tlpStrings.RowCount; ++i)
					ret.Add((this.tlpStrings.GetControlFromPosition(0, i) as Label).Text.TrimEnd(':'), (this.tlpStrings.GetControlFromPosition(1, i) as TextBox).Text);
				return ret;
			}
		}

		bool Editing { get; set; }
		MainForm MainForm { get; set; }

		public AddEditValues(MainForm mainForm, params string[] languages)
		{
			this.InitializeComponent();

			this.Editing = false;
			this.MainForm = mainForm;

			int i = 1;
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

			this.tlpStrings.RowCount = languages.Length + 1;

			this.Height = 118 + 26 * languages.Length;
		}

		public AddEditValues(MainForm mainForm, string[] languages, string key, string[] values) : this(mainForm, languages)
		{
			this.Editing = true;
			this.tbKey.Text = key;
			int i = 1;
			foreach (var value in values)
				(this.tlpStrings.GetControlFromPosition(1, i++) as TextBox).Text = value;
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.tbKey.Text))
			{
				this.errorProvider.SetError(this.tbKey, "Key is required!");
				return;
			}
			if (!this.Editing && this.MainForm.KeyExists(this.tbKey.Text))
			{
				this.errorProvider.SetError(this.tbKey, "Key already exists!");
				return;
			}
			this.Close();
		}

		bool canceling = false;

		void btnCancel_Click(object sender, EventArgs e)
		{
			this.canceling = true;
			this.Close();
		}

		void AddNewValues_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.canceling && (string.IsNullOrWhiteSpace(this.tbKey.Text) || (!this.Editing && this.MainForm.KeyExists(this.tbKey.Text))))
				e.Cancel = true;
		}
	}
}
