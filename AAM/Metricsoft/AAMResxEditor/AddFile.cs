using System;
using System.Windows.Forms;

namespace AAMResxEditor
{
	public partial class AddFile : Form
	{
		public string Filename
		{
			get { return this.txtName.Text; }
		}

		public AddFile()
		{
			this.InitializeComponent();
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.Filename))
			{
				this.errorProvider.SetError(this.txtName, "Filename is required!");
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

		void AddFile_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.canceling && string.IsNullOrWhiteSpace(this.Filename))
				e.Cancel = true;
		}
	}
}
