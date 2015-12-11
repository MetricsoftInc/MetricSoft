namespace AAMResxEditor
{
	partial class MainForm
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
			this.btnProcess = new System.Windows.Forms.Button();
			this.btnSearchFolder = new System.Windows.Forms.Button();
			this.tbAAMProject = new System.Windows.Forms.TextBox();
			this.lblAAMProject = new System.Windows.Forms.Label();
			this.lblLanguages = new System.Windows.Forms.Label();
			this.clbLanguages = new System.Windows.Forms.CheckedListBox();
			this.lblStrings = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.cbFiles = new System.Windows.Forms.ComboBox();
			this.lblLanguageFile = new System.Windows.Forms.Label();
			this.tlpStrings = new System.Windows.Forms.TableLayoutPanel();
			this.lblFile = new System.Windows.Forms.Label();
			this.btnAddNewValues = new System.Windows.Forms.Button();
			this.tlpCopy = new System.Windows.Forms.TableLayoutPanel();
			this.btnCopyAll = new System.Windows.Forms.Button();
			this.cbCopyFrom = new System.Windows.Forms.ComboBox();
			this.lblTo = new System.Windows.Forms.Label();
			this.cbCopyTo = new System.Windows.Forms.ComboBox();
			this.btnCopyMissing = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnAddNewFile = new System.Windows.Forms.Button();
			this.tlpStrings.SuspendLayout();
			this.tlpCopy.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnProcess
			// 
			this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnProcess.Location = new System.Drawing.Point(768, 10);
			this.btnProcess.Name = "btnProcess";
			this.btnProcess.Size = new System.Drawing.Size(75, 23);
			this.btnProcess.TabIndex = 7;
			this.btnProcess.Text = "Process";
			this.btnProcess.UseVisualStyleBackColor = true;
			this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
			// 
			// btnSearchFolder
			// 
			this.btnSearchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSearchFolder.Location = new System.Drawing.Point(737, 10);
			this.btnSearchFolder.Name = "btnSearchFolder";
			this.btnSearchFolder.Size = new System.Drawing.Size(25, 23);
			this.btnSearchFolder.TabIndex = 6;
			this.btnSearchFolder.Text = "...";
			this.btnSearchFolder.UseVisualStyleBackColor = true;
			this.btnSearchFolder.Click += new System.EventHandler(this.btnSearchFolder_Click);
			// 
			// tbAAMProject
			// 
			this.tbAAMProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAAMProject.Location = new System.Drawing.Point(131, 12);
			this.tbAAMProject.Name = "tbAAMProject";
			this.tbAAMProject.Size = new System.Drawing.Size(600, 20);
			this.tbAAMProject.TabIndex = 5;
			// 
			// lblAAMProject
			// 
			this.lblAAMProject.AutoSize = true;
			this.lblAAMProject.Location = new System.Drawing.Point(12, 15);
			this.lblAAMProject.Name = "lblAAMProject";
			this.lblAAMProject.Size = new System.Drawing.Size(113, 13);
			this.lblAAMProject.TabIndex = 4;
			this.lblAAMProject.Text = "AAM Project Location:";
			// 
			// lblLanguages
			// 
			this.lblLanguages.AutoSize = true;
			this.lblLanguages.Location = new System.Drawing.Point(12, 35);
			this.lblLanguages.Name = "lblLanguages";
			this.lblLanguages.Size = new System.Drawing.Size(97, 13);
			this.lblLanguages.TabIndex = 10;
			this.lblLanguages.Text = "Languages to Use:";
			// 
			// clbLanguages
			// 
			this.clbLanguages.CheckOnClick = true;
			this.clbLanguages.FormattingEnabled = true;
			this.clbLanguages.Location = new System.Drawing.Point(12, 51);
			this.clbLanguages.Name = "clbLanguages";
			this.clbLanguages.Size = new System.Drawing.Size(200, 154);
			this.clbLanguages.TabIndex = 9;
			// 
			// lblStrings
			// 
			this.lblStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStrings.AutoSize = true;
			this.lblStrings.Location = new System.Drawing.Point(218, 62);
			this.lblStrings.Name = "lblStrings";
			this.lblStrings.Size = new System.Drawing.Size(93, 13);
			this.lblStrings.TabIndex = 14;
			this.lblStrings.Text = "Language Strings:";
			// 
			// cbFiles
			// 
			this.cbFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFiles.FormattingEnabled = true;
			this.cbFiles.Location = new System.Drawing.Point(301, 38);
			this.cbFiles.Name = "cbFiles";
			this.cbFiles.Size = new System.Drawing.Size(225, 21);
			this.cbFiles.Sorted = true;
			this.cbFiles.TabIndex = 17;
			this.cbFiles.SelectedIndexChanged += new System.EventHandler(this.cbFiles_SelectedIndexChanged);
			// 
			// lblLanguageFile
			// 
			this.lblLanguageFile.AutoSize = true;
			this.lblLanguageFile.Location = new System.Drawing.Point(218, 41);
			this.lblLanguageFile.Name = "lblLanguageFile";
			this.lblLanguageFile.Size = new System.Drawing.Size(77, 13);
			this.lblLanguageFile.TabIndex = 18;
			this.lblLanguageFile.Text = "Language File:";
			// 
			// tlpStrings
			// 
			this.tlpStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tlpStrings.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tlpStrings.ColumnCount = 1;
			this.tlpStrings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpStrings.Controls.Add(this.lblFile, 0, 0);
			this.tlpStrings.Controls.Add(this.btnAddNewValues, 0, 2);
			this.tlpStrings.Controls.Add(this.tlpCopy, 0, 3);
			this.tlpStrings.Location = new System.Drawing.Point(218, 78);
			this.tlpStrings.Name = "tlpStrings";
			this.tlpStrings.RowCount = 4;
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			this.tlpStrings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
			this.tlpStrings.Size = new System.Drawing.Size(625, 300);
			this.tlpStrings.TabIndex = 19;
			// 
			// lblFile
			// 
			this.lblFile.AutoSize = true;
			this.lblFile.BackColor = System.Drawing.Color.Gray;
			this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblFile.Location = new System.Drawing.Point(1, 1);
			this.lblFile.Margin = new System.Windows.Forms.Padding(0);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(623, 26);
			this.lblFile.TabIndex = 0;
			this.lblFile.Text = "label1";
			this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblFile.Visible = false;
			// 
			// btnAddNewValues
			// 
			this.btnAddNewValues.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnAddNewValues.Location = new System.Drawing.Point(1, 246);
			this.btnAddNewValues.Margin = new System.Windows.Forms.Padding(0);
			this.btnAddNewValues.Name = "btnAddNewValues";
			this.btnAddNewValues.Size = new System.Drawing.Size(623, 23);
			this.btnAddNewValues.TabIndex = 1;
			this.btnAddNewValues.Text = "Add New Value(s)";
			this.btnAddNewValues.UseVisualStyleBackColor = true;
			this.btnAddNewValues.Visible = false;
			this.btnAddNewValues.Click += new System.EventHandler(this.btnAddNewValues_Click);
			// 
			// tlpCopy
			// 
			this.tlpCopy.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.tlpCopy.ColumnCount = 5;
			this.tlpCopy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpCopy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpCopy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpCopy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpCopy.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpCopy.Controls.Add(this.btnCopyAll, 0, 0);
			this.tlpCopy.Controls.Add(this.cbCopyFrom, 2, 0);
			this.tlpCopy.Controls.Add(this.lblTo, 3, 0);
			this.tlpCopy.Controls.Add(this.cbCopyTo, 4, 0);
			this.tlpCopy.Controls.Add(this.btnCopyMissing, 1, 0);
			this.tlpCopy.Location = new System.Drawing.Point(51, 270);
			this.tlpCopy.Margin = new System.Windows.Forms.Padding(0);
			this.tlpCopy.Name = "tlpCopy";
			this.tlpCopy.RowCount = 1;
			this.tlpCopy.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpCopy.Size = new System.Drawing.Size(522, 29);
			this.tlpCopy.TabIndex = 2;
			this.tlpCopy.Visible = false;
			// 
			// btnCopyAll
			// 
			this.btnCopyAll.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCopyAll.Location = new System.Drawing.Point(3, 3);
			this.btnCopyAll.Name = "btnCopyAll";
			this.btnCopyAll.Size = new System.Drawing.Size(75, 23);
			this.btnCopyAll.TabIndex = 0;
			this.btnCopyAll.Text = "Copy All";
			this.btnCopyAll.UseVisualStyleBackColor = true;
			this.btnCopyAll.Click += new System.EventHandler(this.btnCopyAll_Click);
			// 
			// cbCopyFrom
			// 
			this.cbCopyFrom.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbCopyFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCopyFrom.FormattingEnabled = true;
			this.cbCopyFrom.Location = new System.Drawing.Point(190, 4);
			this.cbCopyFrom.Name = "cbCopyFrom";
			this.cbCopyFrom.Size = new System.Drawing.Size(150, 21);
			this.cbCopyFrom.TabIndex = 1;
			this.cbCopyFrom.SelectedIndexChanged += new System.EventHandler(this.cbCopy_SelectedIndexChanged);
			// 
			// lblTo
			// 
			this.lblTo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblTo.AutoSize = true;
			this.lblTo.Location = new System.Drawing.Point(346, 8);
			this.lblTo.Name = "lblTo";
			this.lblTo.Size = new System.Drawing.Size(16, 13);
			this.lblTo.TabIndex = 2;
			this.lblTo.Text = "to";
			// 
			// cbCopyTo
			// 
			this.cbCopyTo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbCopyTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCopyTo.FormattingEnabled = true;
			this.cbCopyTo.Location = new System.Drawing.Point(368, 4);
			this.cbCopyTo.Name = "cbCopyTo";
			this.cbCopyTo.Size = new System.Drawing.Size(150, 21);
			this.cbCopyTo.TabIndex = 1;
			this.cbCopyTo.SelectedIndexChanged += new System.EventHandler(this.cbCopy_SelectedIndexChanged);
			// 
			// btnCopyMissing
			// 
			this.btnCopyMissing.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCopyMissing.Location = new System.Drawing.Point(84, 3);
			this.btnCopyMissing.Name = "btnCopyMissing";
			this.btnCopyMissing.Size = new System.Drawing.Size(100, 23);
			this.btnCopyMissing.TabIndex = 0;
			this.btnCopyMissing.Text = "Copy Missing";
			this.btnCopyMissing.UseVisualStyleBackColor = true;
			this.btnCopyMissing.Click += new System.EventHandler(this.btnCopyMissing_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(768, 384);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 20;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnAddNewFile
			// 
			this.btnAddNewFile.Enabled = false;
			this.btnAddNewFile.Location = new System.Drawing.Point(532, 38);
			this.btnAddNewFile.Name = "btnAddNewFile";
			this.btnAddNewFile.Size = new System.Drawing.Size(100, 23);
			this.btnAddNewFile.TabIndex = 21;
			this.btnAddNewFile.Text = "Add New File";
			this.btnAddNewFile.UseVisualStyleBackColor = true;
			this.btnAddNewFile.Click += new System.EventHandler(this.btnAddNewFile_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(855, 419);
			this.Controls.Add(this.btnAddNewFile);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.tlpStrings);
			this.Controls.Add(this.lblLanguageFile);
			this.Controls.Add(this.cbFiles);
			this.Controls.Add(this.lblStrings);
			this.Controls.Add(this.lblLanguages);
			this.Controls.Add(this.clbLanguages);
			this.Controls.Add(this.btnProcess);
			this.Controls.Add(this.btnSearchFolder);
			this.Controls.Add(this.tbAAMProject);
			this.Controls.Add(this.lblAAMProject);
			this.MinimumSize = new System.Drawing.Size(871, 458);
			this.Name = "MainForm";
			this.Text = "AAM Resx Editor";
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.tlpStrings.ResumeLayout(false);
			this.tlpStrings.PerformLayout();
			this.tlpCopy.ResumeLayout(false);
			this.tlpCopy.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnProcess;
		private System.Windows.Forms.Button btnSearchFolder;
		private System.Windows.Forms.TextBox tbAAMProject;
		private System.Windows.Forms.Label lblAAMProject;
		private System.Windows.Forms.Label lblLanguages;
		private System.Windows.Forms.CheckedListBox clbLanguages;
		private System.Windows.Forms.Label lblStrings;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.ComboBox cbFiles;
		private System.Windows.Forms.Label lblLanguageFile;
		private System.Windows.Forms.TableLayoutPanel tlpStrings;
		private System.Windows.Forms.Label lblFile;
		private System.Windows.Forms.Button btnAddNewValues;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.TableLayoutPanel tlpCopy;
		private System.Windows.Forms.Button btnCopyAll;
		private System.Windows.Forms.ComboBox cbCopyFrom;
		private System.Windows.Forms.Label lblTo;
		private System.Windows.Forms.ComboBox cbCopyTo;
		private System.Windows.Forms.Button btnCopyMissing;
		private System.Windows.Forms.Button btnAddNewFile;
	}
}

