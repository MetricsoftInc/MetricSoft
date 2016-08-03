using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AAMResxEditor
{
	public partial class MainForm : AsyncBaseDialog
	{
		class Language
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		public MainForm()
		{
			this.InitializeComponent();

			(this.clbLanguages as ListBox).DisplayMember = "Name";

			this.clbLanguages.Items.Clear();
			this.clbLanguages.Items.Add(new Language() { Name = "English", Value = "" }, true);

			using (var db = new Data.AAMContext())
				this.clbLanguages.Items.AddRange(db.LOCAL_LANGUAGE.Where(l => l.NLS_LANGUAGE != "en").Select(l => new Language() { Name = l.LANGUAGE_NAME, Value = l.NLS_LANGUAGE }).ToArray());
		}

		void btnSearchFolder_Click(object sender, EventArgs e)
		{
			if (this.folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
				this.tbAAMProject.Text = this.folderBrowserDialog.SelectedPath;
		}

		static Dictionary<string, bool> cachedValidLanguageTags = new Dictionary<string, bool>();

		static bool validLanguageTag(string tag)
		{
			bool valid = false;
			if (!cachedValidLanguageTags.TryGetValue(tag, out valid))
				using (var client = new WebClient())
				{
					valid = (bool)JArray.Parse(client.DownloadString(string.Format(CultureInfo.InvariantCulture, "http://schneegans.de/lv/?tags={0}&format=json", tag)))[0]["Valid"];

					cachedValidLanguageTags[tag] = valid;
				}
			return valid;
		}

		Dictionary<string, string> folders = new Dictionary<string, string>();
		Dictionary<string, Dictionary<string, Dictionary<string, string>>> strings = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

		void btnProcess_Click(object sender, EventArgs e)
		{
			if (this.clbLanguages.CheckedItems.Count == 0)
			{
				MessageBox.Show(this, "You must select at least 1 language!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			string directory = this.tbAAMProject.Text;
			if (!Directory.Exists(directory))
			{
				MessageBox.Show(this, "Invalid AAM project directory!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			directory = Path.GetFullPath(directory);

			this.folders.Clear();
			this.strings.Clear();

			var files = new List<string>();
			var resxFiles = Directory.GetFiles(directory, "*.resx", SearchOption.AllDirectories);
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();

			this.RunAsyncOperation(() =>
			{
				this.cbFiles.Items.Clear();
				this.lblFile.Visible = this.tlpAddNewValues.Visible = this.tlpCopy.Visible = false;
				var dgv = this.tlpStrings.GetControlFromPosition(0, 1);
				if (dgv != null)
					this.tlpStrings.Controls.Remove(dgv);
			}, () =>
			{
				foreach (var resxFile in resxFiles)
				{
					string baseName = Path.GetFileNameWithoutExtension(resxFile);
					string languageTag = "";
					int period = baseName.LastIndexOf('.');
					if (period != -1)
					{
						string possibleLanguageTag = baseName.Substring(period + 1);
						bool valid = validLanguageTag(possibleLanguageTag);
						if (valid)
						{
							languageTag = possibleLanguageTag;
							baseName = baseName.Substring(0, period);
						}
					}
					string resxFolder = Path.GetDirectoryName(resxFile).Replace(directory + Path.DirectorySeparatorChar, "");
					string baseNameFolder = resxFolder.Replace("App_GlobalResources", "").Replace("App_LocalResources", "").TrimEnd(Path.DirectorySeparatorChar);
					baseName = Path.Combine(baseNameFolder, baseName);
					if (resxFolder.Equals("App_GlobalResources", StringComparison.InvariantCultureIgnoreCase))
						baseName = "!" + baseName;
					this.folders[baseName] = resxFolder;

					if (!checkedLanguages.Any(lang => lang.Value == languageTag))
						continue;

					files.Add(baseName);

					if (!this.strings.ContainsKey(baseName))
						this.strings[baseName] = new Dictionary<string, Dictionary<string, string>>();
					var baseNameStrings = this.strings[baseName];

					using (var reader = new ResXResourceReader(resxFile))
						foreach (DictionaryEntry de in reader)
						{
							string key = de.Key as string;
							if (!baseNameStrings.ContainsKey(key))
								baseNameStrings[key] = new Dictionary<string, string>();
							baseNameStrings[key][languageTag] = de.Value as string;
						}
				}
			}, () =>
			{
				this.cbFiles.Items.AddRange(files.Distinct().ToArray());
				this.btnAddNewFile.Enabled = true;
			});
		}

		void dgvStrings_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var dgvStrings = sender as DataGridView;
			if (dgvStrings.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
			{
				var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
				string originalKey = dgvStrings[1, e.RowIndex].Value as string;
				using (var editValues = new AddEditValues(this, checkedLanguages.Select(l => l.Name).ToArray(), originalKey,
					Enumerable.Range(2, checkedLanguages.Count).Select(i => dgvStrings[i, e.RowIndex].Value as string).ToArray()))
					if (editValues.ShowDialog(this) == DialogResult.OK)
					{
						string newKey = editValues.Values["Key"];
						var fileStrings = this.strings[this.lblFile.Text];
						if (newKey != originalKey)
						{
							fileStrings.Remove(originalKey);
							dgvStrings[1, e.RowIndex].Value = newKey;
						}
						if (!fileStrings.ContainsKey(newKey))
							fileStrings[newKey] = new Dictionary<string, string>();
						for (int i = 0; i < checkedLanguages.Count; ++i)
						{
							var cell = dgvStrings[i + 2, e.RowIndex];
							string langString = editValues.Values[checkedLanguages[i].Name];
							cell.Value = string.IsNullOrWhiteSpace(langString) ? null : langString;
							if (string.IsNullOrWhiteSpace(langString))
							{
								cell.Style.BackColor = Color.Yellow;
								fileStrings[newKey].Remove(checkedLanguages[i].Value);
							}
							else
							{
								cell.Style.BackColor = Color.White;
								fileStrings[newKey][checkedLanguages[i].Value] = langString;
							}
						}
						dgvStrings.Sort(dgvStrings.Columns[1], ListSortDirection.Ascending);
					}
			}
		}

		void dgvStrings_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			this.strings[this.lblFile.Text].Remove(e.Row.Cells[1].Value as string);
		}

		void cbFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			string file = this.cbFiles.SelectedItem as string;
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();

			this.lblFile.Text = file;
			this.cbCopyFrom.Items.Clear();
			this.cbCopyTo.Items.Clear();
			var checkedLanguageNames = checkedLanguages.Select(l => l.Name);
			this.cbCopyFrom.Items.AddRange(checkedLanguageNames.ToArray());
			this.cbCopyTo.Items.AddRange(checkedLanguageNames.ToArray());
			this.cbCopyFrom.SelectedIndex = this.cbCopyTo.SelectedIndex = 0;
			this.btnCopyAll.Enabled = this.btnCopyMissing.Enabled = false;
			this.lblFile.Visible = this.tlpAddNewValues.Visible = this.tlpCopy.Visible = true;

			var dgvStrings = new DataGridView()
			{
				AllowUserToAddRows = false,
				AllowUserToResizeColumns = false,
				AllowUserToResizeRows = false,
				Columns =
				{
					new DataGridViewButtonColumn()
					{
						AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
						SortMode = DataGridViewColumnSortMode.NotSortable,
						Text = "Edit",
						Width = 50,
						UseColumnTextForButtonValue = true
					},
					new DataGridViewTextBoxColumn()
					{
						AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
						FillWeight = 20f,
						HeaderText = "Key",
						ReadOnly = true,
						SortMode = DataGridViewColumnSortMode.NotSortable
					}
				},
				Dock = DockStyle.Fill,
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect
			};
			dgvStrings.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			dgvStrings.CellContentClick += this.dgvStrings_CellContentClick;
			dgvStrings.UserDeletingRow += this.dgvStrings_UserDeletingRow;

			foreach (var checkedLanguage in checkedLanguages)
				dgvStrings.Columns.Add(new DataGridViewTextBoxColumn()
				{
					AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
					FillWeight = 80f / checkedLanguages.Count,
					HeaderText = checkedLanguage.Name,
					ReadOnly = true,
					SortMode = DataGridViewColumnSortMode.NotSortable
				});

			foreach (var stringValues in this.strings[file].OrderBy(v => v.Key))
			{
				int rowNum = dgvStrings.Rows.Add();
				dgvStrings[1, rowNum].Value = stringValues.Key;
				for (int i = 0; i < checkedLanguages.Count; ++i)
				{
					var cell = dgvStrings[i + 2, rowNum];
					if (stringValues.Value.ContainsKey(checkedLanguages[i].Value))
						cell.Value = stringValues.Value[checkedLanguages[i].Value];
					else
						cell.Style.BackColor = Color.Yellow;
				}
			}

			this.tlpStrings.Controls.Remove(this.tlpStrings.GetControlFromPosition(0, 1));
			this.tlpStrings.Controls.Add(dgvStrings, 0, 1);

			dgvStrings.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
		}

		void btnAddNewFile_Click(object sender, EventArgs e)
		{
			using (var addNewFile = new AddFile())
				if (addNewFile.ShowDialog(this) == DialogResult.OK)
				{
					this.cbFiles.Items.Add(addNewFile.Filename);
					this.strings[addNewFile.Filename] = new Dictionary<string, Dictionary<string, string>>();
					this.cbFiles.SelectedItem = addNewFile.Filename;
				}
		}

		void btnAddNewValues_Click(object sender, EventArgs e)
		{
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
			var dgvStrings = this.tlpStrings.GetControlFromPosition(0, 1) as DataGridView;
			using (var addNewValues = new AddEditValues(this, checkedLanguages.Select(l => l.Name).ToArray()))
				if (addNewValues.ShowDialog(this) == DialogResult.OK)
				{
					int rowNum = dgvStrings.Rows.Add();
					string key = addNewValues.Values["Key"];
					var fileStrings = this.strings[this.lblFile.Text];
					fileStrings[key] = new Dictionary<string, string>();
					dgvStrings[1, rowNum].Value = key;
					for (int i = 0; i < checkedLanguages.Count; ++i)
					{
						var cell = dgvStrings[i + 2, rowNum];
						string langString = addNewValues.Values[checkedLanguages[i].Name];
						if (string.IsNullOrWhiteSpace(langString))
							cell.Style.BackColor = Color.Yellow;
						else
						{
							cell.Value = langString;
							fileStrings[key][checkedLanguages[i].Value] = langString;
						}
					}
					dgvStrings.Sort(dgvStrings.Columns[1], ListSortDirection.Ascending);
				}
		}

		void btnAddNewValuesDev_Click(object sender, EventArgs e)
		{
			if (this.lblFile.Text[0] == '!')
			{
				MessageBox.Show(this, "The current file is located in App_GlobalResources and does not have a corresponding file. You cannot use this action on it.", "ERROR", MessageBoxButtons.OK,
					MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			string filename = Path.Combine(this.tbAAMProject.Text, this.lblFile.Text);
			if (!File.Exists(filename))
			{
				MessageBox.Show(this, "The file you have selected does not exist. It must exist before this action can be used.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error,
					MessageBoxDefaultButton.Button1);
				return;
			}
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
			var dgvStrings = this.tlpStrings.GetControlFromPosition(0, 1) as DataGridView;
			using (var addNewValues = new AddValuesDev(this, checkedLanguages.Select(l => l.Name).ToArray()))
				if (addNewValues.ShowDialog(this) == DialogResult.OK)
				{
					int rowNum = dgvStrings.Rows.Add();
					string key = addNewValues.Values["Key"];
					var fileStrings = this.strings[this.lblFile.Text];
					fileStrings[key] = new Dictionary<string, string>();
					dgvStrings[1, rowNum].Value = key;
					for (int i = 0; i < checkedLanguages.Count; ++i)
					{
						var cell = dgvStrings[i + 2, rowNum];
						string langString = addNewValues.Values[checkedLanguages[i].Name];
						if (string.IsNullOrWhiteSpace(langString))
							cell.Style.BackColor = Color.Yellow;
						else
						{
							cell.Value = langString;
							fileStrings[key][checkedLanguages[i].Value] = langString;
						}
					}
					dgvStrings.Sort(dgvStrings.Columns[1], ListSortDirection.Ascending);
					string fileContents = File.ReadAllText(filename);
					var regex = new Regex(string.Format(@"id\s*=\s*""{0}""", addNewValues.ControlID), RegexOptions.IgnoreCase);
					var match = regex.Match(fileContents);
					int tagStart = fileContents.LastIndexOf('<', match.Index);
					int tagEnd = fileContents.IndexOf('>', match.Index);
					string tag = fileContents.Substring(tagStart, tagEnd - tagStart + 1);
					// TODO: Actual replacement
					fileContents = fileContents.Substring(0, tagStart) + tag + fileContents.Substring(tagEnd + 1);
					// TODO: Actually re-save file
				}
		}

		void btnCopyAll_Click(object sender, EventArgs e)
		{
			this.copyLanguages(false);
		}

		void btnCopyMissing_Click(object sender, EventArgs e)
		{
			this.copyLanguages(true);
		}

		void copyLanguages(bool onlyMissing)
		{
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
			string languageTagFrom = checkedLanguages.First(l => l.Name == this.cbCopyFrom.SelectedItem as string).Value;
			dynamic languageTo = checkedLanguages.First(l => l.Name == this.cbCopyTo.SelectedItem as string);
			string languageTagTo = languageTo.Value;
			int languageTagToIndex = checkedLanguages.IndexOf(languageTo);
			var dgvStrings = this.tlpStrings.GetControlFromPosition(0, 1) as DataGridView;
			var fileStrings = this.strings[this.lblFile.Text];
			for (int r = 0; r < dgvStrings.Rows.Count; ++r)
			{
				string key = dgvStrings[1, r].Value as string;
				if (fileStrings[key].ContainsKey(languageTagFrom) && (onlyMissing ? !fileStrings[key].ContainsKey(languageTagTo) : true))
				{
					var cell = dgvStrings[languageTagToIndex + 2, r];
					cell.Value = fileStrings[key][languageTagTo] = fileStrings[key][languageTagFrom];
					cell.Style.BackColor = Color.White;
				}
			}
		}

		void cbCopy_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.btnCopyAll.Enabled = this.btnCopyMissing.Enabled = this.cbCopyFrom.SelectedIndex != this.cbCopyTo.SelectedIndex;
		}

		class ResXWriter
		{
			public string Filename { get; set; }
			public ResXResourceWriter Writer { get; set; }
			public bool WroteAnything { get; set; }
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			string directory = Path.GetFullPath(this.tbAAMProject.Text);
			var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
			foreach (var file in this.folders)
			{
				string origFilename = file.Key;
				string filename = origFilename;
				if (filename[0] == '!')
					filename = filename.Substring(1);
				string filenameBase = Path.Combine(directory, file.Value, Path.GetFileName(filename));
				var writers = checkedLanguages.Select(lang =>
				{
					string Filename = filenameBase + (lang.Value != "" ? "." + lang.Value : "") + ".resx";
					return new
					{
						Key = lang.Value as string,
						Filename,
						Writer = new ResXResourceWriter(Filename)
					};
				}).ToDictionary(x => x.Key, x => new ResXWriter() { Filename = x.Filename, Writer = x.Writer, WroteAnything = false });

				foreach (var stringValues in this.strings[origFilename].OrderBy(v => v.Key))
					foreach (var lang in checkedLanguages)
						if (stringValues.Value.ContainsKey(lang.Value))
						{
							writers[lang.Value].Writer.AddResource(stringValues.Key, stringValues.Value[lang.Value]);
							writers[lang.Value].WroteAnything = true;
						}

				foreach (var writer in writers)
				{
					if (writer.Value.WroteAnything)
						writer.Value.Writer.Generate();
					writer.Value.Writer.Close();
					if (!writer.Value.WroteAnything)
						File.Delete(writer.Value.Filename);
				}
			}

			MessageBox.Show(this, "All .resx Resource files have been saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}

		void MainForm_Resize(object sender, EventArgs e)
		{
			var dgvStrings = this.tlpStrings.GetControlFromPosition(0, 1) as DataGridView;
			if (dgvStrings != null)
				dgvStrings.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
		}

		public bool KeyExists(string key)
		{
			return this.strings[this.lblFile.Text].ContainsKey(key);
		}

		void btnExcelExport_Click(object sender, EventArgs e)
		{
			if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				var checkedLanguages = this.clbLanguages.CheckedItems.Cast<Language>().ToList();
				var newFile = new FileInfo(this.saveFileDialog.FileName);
				if (newFile.Exists)
					newFile.Delete();

				using (var excel = new ExcelPackage(newFile))
				{
					foreach (var file in this.folders)
					{
						var ws = excel.Workbook.Worksheets.Add(file.Key);

						ws.Cells["A1"].Value = "Key";
						ws.Cells["A1"].Style.Font.Bold = true;
						ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
						ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
						int col = 2;
						foreach (var lang in checkedLanguages)
						{
							ws.Cells[1, col].Value = lang.Name;
							ws.Cells[1, col].Style.Font.Bold = true;
							ws.Cells[1, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
							ws.Cells[1, col++].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
						}

						int row = 2;
						foreach (var stringValues in this.strings[file.Key].OrderBy(v => v.Key))
						{
							ws.Cells[row, 1].Value = stringValues.Key;
							for (int i = 0; i < checkedLanguages.Count; ++i)
							{
								ws.Cells[row, i + 2].Style.WrapText = true;
								if (stringValues.Value.ContainsKey(checkedLanguages[i].Value))
									ws.Cells[row, i + 2].Value = stringValues.Value[checkedLanguages[i].Value];
							}
							++row;
						}

						ws.Column(1).AutoFit();
						for (int i = 0; i < checkedLanguages.Count; ++i)
							ws.Column(i + 2).Width = 100;
					}
					excel.Save();
				}

				MessageBox.Show(this, $"Excel file was created at {this.saveFileDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
			}
		}
	}
}
