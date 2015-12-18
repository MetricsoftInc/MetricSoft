namespace AAMResxEditor
{
	partial class AddValuesDev
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tlpStrings = new System.Windows.Forms.TableLayoutPanel();
			this.tbControlID = new System.Windows.Forms.TextBox();
			this.lblControlID = new System.Windows.Forms.Label();
			this.lblKey = new System.Windows.Forms.Label();
			this.tbKey = new System.Windows.Forms.TextBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.lblResourceKey = new System.Windows.Forms.Label();
			this.tbResourceKey = new System.Windows.Forms.TextBox();
			this.tlpStrings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(297, 96);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(216, 96);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// tlpStrings
			// 
			this.tlpStrings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tlpStrings.AutoSize = true;
			this.tlpStrings.ColumnCount = 2;
			this.tlpStrings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpStrings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpStrings.Controls.Add(this.tbResourceKey, 1, 1);
			this.tlpStrings.Controls.Add(this.tbControlID, 1, 0);
			this.tlpStrings.Controls.Add(this.lblControlID, 0, 0);
			this.tlpStrings.Controls.Add(this.lblKey, 0, 2);
			this.tlpStrings.Controls.Add(this.tbKey, 1, 2);
			this.tlpStrings.Controls.Add(this.lblResourceKey, 0, 1);
			this.tlpStrings.Location = new System.Drawing.Point(12, 12);
			this.tlpStrings.Name = "tlpStrings";
			this.tlpStrings.RowCount = 3;
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tlpStrings.Size = new System.Drawing.Size(360, 78);
			this.tlpStrings.TabIndex = 5;
			// 
			// tbControlID
			// 
			this.tbControlID.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.errorProvider.SetIconPadding(this.tbControlID, 3);
			this.tbControlID.Location = new System.Drawing.Point(86, 3);
			this.tbControlID.Name = "tbControlID";
			this.tbControlID.Size = new System.Drawing.Size(200, 20);
			this.tbControlID.TabIndex = 7;
			// 
			// lblControlID
			// 
			this.lblControlID.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblControlID.AutoSize = true;
			this.lblControlID.Location = new System.Drawing.Point(3, 6);
			this.lblControlID.Name = "lblControlID";
			this.lblControlID.Size = new System.Drawing.Size(57, 13);
			this.lblControlID.TabIndex = 6;
			this.lblControlID.Text = "Control ID:";
			// 
			// lblKey
			// 
			this.lblKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblKey.AutoSize = true;
			this.lblKey.Location = new System.Drawing.Point(3, 58);
			this.lblKey.Name = "lblKey";
			this.lblKey.Size = new System.Drawing.Size(28, 13);
			this.lblKey.TabIndex = 0;
			this.lblKey.Text = "Key:";
			// 
			// tbKey
			// 
			this.tbKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.errorProvider.SetIconPadding(this.tbKey, 3);
			this.tbKey.Location = new System.Drawing.Point(86, 55);
			this.tbKey.Name = "tbKey";
			this.tbKey.Size = new System.Drawing.Size(200, 20);
			this.tbKey.TabIndex = 1;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// lblResourceKey
			// 
			this.lblResourceKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblResourceKey.AutoSize = true;
			this.lblResourceKey.Location = new System.Drawing.Point(3, 32);
			this.lblResourceKey.Name = "lblResourceKey";
			this.lblResourceKey.Size = new System.Drawing.Size(77, 13);
			this.lblResourceKey.TabIndex = 8;
			this.lblResourceKey.Text = "Resource Key:";
			// 
			// tbResourceKey
			// 
			this.tbResourceKey.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.errorProvider.SetIconPadding(this.tbResourceKey, 3);
			this.tbResourceKey.Location = new System.Drawing.Point(86, 29);
			this.tbResourceKey.Name = "tbResourceKey";
			this.tbResourceKey.Size = new System.Drawing.Size(200, 20);
			this.tbResourceKey.TabIndex = 9;
			// 
			// AddValuesDev
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(384, 131);
			this.Controls.Add(this.tlpStrings);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddValuesDev";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add New Values";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddValuesDev_FormClosing);
			this.tlpStrings.ResumeLayout(false);
			this.tlpStrings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TableLayoutPanel tlpStrings;
		private System.Windows.Forms.Label lblKey;
		private System.Windows.Forms.TextBox tbKey;
		private System.Windows.Forms.TextBox tbControlID;
		private System.Windows.Forms.Label lblControlID;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox tbResourceKey;
		private System.Windows.Forms.Label lblResourceKey;
	}
}