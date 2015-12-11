namespace AAMResxEditor
{
	partial class AddEditValues
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tlpStrings = new System.Windows.Forms.TableLayoutPanel();
			this.lblKey = new System.Windows.Forms.Label();
			this.tbKey = new System.Windows.Forms.TextBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tlpStrings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpStrings
			// 
			this.tlpStrings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tlpStrings.AutoSize = true;
			this.tlpStrings.ColumnCount = 2;
			this.tlpStrings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpStrings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpStrings.Controls.Add(this.lblKey, 0, 0);
			this.tlpStrings.Controls.Add(this.tbKey, 1, 0);
			this.tlpStrings.Location = new System.Drawing.Point(12, 12);
			this.tlpStrings.Name = "tlpStrings";
			this.tlpStrings.RowCount = 1;
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tlpStrings.Size = new System.Drawing.Size(360, 26);
			this.tlpStrings.TabIndex = 0;
			// 
			// lblKey
			// 
			this.lblKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblKey.AutoSize = true;
			this.lblKey.Location = new System.Drawing.Point(3, 6);
			this.lblKey.Name = "lblKey";
			this.lblKey.Size = new System.Drawing.Size(28, 13);
			this.lblKey.TabIndex = 0;
			this.lblKey.Text = "Key:";
			// 
			// tbKey
			// 
			this.tbKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.errorProvider.SetIconPadding(this.tbKey, 3);
			this.tbKey.Location = new System.Drawing.Point(37, 3);
			this.tbKey.Name = "tbKey";
			this.tbKey.Size = new System.Drawing.Size(200, 20);
			this.tbKey.TabIndex = 1;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(216, 44);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(297, 44);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// AddEditValues
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(384, 79);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tlpStrings);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddEditValues";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add/Edit Values";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddNewValues_FormClosing);
			this.tlpStrings.ResumeLayout(false);
			this.tlpStrings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpStrings;
		private System.Windows.Forms.Label lblKey;
		private System.Windows.Forms.TextBox tbKey;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}