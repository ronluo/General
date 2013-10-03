using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EntitiesGenerator.Definitions;
using System.Xml.Serialization;
using EntitiesGenerator.Readers;
using EntitiesGenerator.Generators;
using GeneralDAC;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Data.SqlServerCe;
using System.Data.SQLite;
using System.Data.SharePoint;

namespace EntitiesGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            if (Properties.Settings.Default.UseRazorEngine)
            {
                this.toolStripComboBox2.Text = "Attribute";
                this.toolStripComboBox2.Visible = false;
            }
        }

        public FormMain(string fileName)
            : this()
        {
            try
            {
                this.currentFile = new System.IO.FileInfo(fileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private Definition definition;

        private void FormMain_Load(object sender, EventArgs e)
        {
            SetupDataGrid();
            UpdateDataGridViewStyle();
            SetupToolBar();
            SetupContextMenu();
            if (this.currentFile != null)
            {
                bool result = OpenXmlFile(this.currentFile);
                if (!result)
                {
                    NewDefinition();
                }
            }
            else
            {
                NewDefinition();
            }
            this.modified = false;
        }

        private void SetupToolBar()
        {
            this.toolStripComboBox1.SelectedIndex = 2;
            this.toolStripComboBox2.SelectedIndex = 1;
        }

        private ComboBox typeComboBox;
        private ComboBox dataTypeComboBox;

        private void SetupDataGrid()
        {
            this.dataGridView1.AutoGenerateColumns = false;

            this.typeComboBox = new ComboBox();
            this.typeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            this.typeComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.typeComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.typeComboBox.Items.AddRange(new object[] {
            "short",
            "int",
            "long",
            "byte",
            "byte[]",
            "bool",
            "float",
            "double",
            "decimal",
            "DateTime",
            "Guid",
            "string",
            "object"});
            this.typeComboBox.Visible = false;
            this.typeComboBox.DropDownHeight = 200;
            this.dataGridView1.Controls.Add(this.typeComboBox);
            this.typeComboBox.TextChanged += new EventHandler(comboBox_TextChanged);

            this.dataTypeComboBox = new ComboBox();
            this.dataTypeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            this.dataTypeComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.dataTypeComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.dataTypeComboBox.Items.AddRange(new object[] {
            "short",
            "int",
            "long",
            "byte",
            "image",
            "binary",
            "bool",
            "float",
            "single",
            "double",
            "decimal",
            "datetime",
            "guid",
            "string",
            "object"});
            this.dataTypeComboBox.Visible = false;
            this.dataTypeComboBox.DropDownHeight = 200;
            this.dataGridView1.Controls.Add(this.dataTypeComboBox);
            this.dataTypeComboBox.TextChanged += new EventHandler(comboBox_TextChanged);
        }

        private void comboBox_TextChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = (sender as ComboBox).Text;
        }

        private void SetupComboBoxForDataGrid()
        {
            try
            {
                if (this.dataGridView1.CurrentCell.RowIndex >= 0)
                {
                    #region DataType
                    // DataType
                    if (this.dataGridView1.CurrentCell.ColumnIndex == this.Column_DataType.Index)
                    {
                        Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(this.dataGridView1.CurrentCell.ColumnIndex, this.dataGridView1.CurrentCell.RowIndex, false);
                        this.dataTypeComboBox.Left = rect.Left;
                        this.dataTypeComboBox.Top = rect.Top;
                        this.dataTypeComboBox.Width = rect.Width - 1;
                        //this.dataTypeComboBox.Height = rect.Height - 1;
                        this.dataTypeComboBox.Visible = true;

                        string value = this.dataGridView1.CurrentCell.Value as string;
                        this.dataTypeComboBox.Text = value;
                        this.dataTypeComboBox.Focus();
                    }
                    else
                    {
                        this.dataTypeComboBox.Visible = false;
                    }
                    #endregion

                    #region Type
                    // Type
                    if (this.dataGridView1.CurrentCell.ColumnIndex == this.Column_Type.Index)
                    {
                        Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(this.dataGridView1.CurrentCell.ColumnIndex, this.dataGridView1.CurrentCell.RowIndex, false);
                        this.typeComboBox.Left = rect.Left;
                        this.typeComboBox.Top = rect.Top;
                        this.typeComboBox.Width = rect.Width - 1;
                        //this.typeComboBox.Height = rect.Height - 1;
                        this.typeComboBox.Visible = true;

                        string value = this.dataGridView1.CurrentCell.Value as string;
                        this.typeComboBox.Text = value;
                        this.typeComboBox.Focus();
                    }
                    else
                    {
                        this.typeComboBox.Visible = false;
                    }
                    #endregion
                }
                else
                {
                    this.typeComboBox.Visible = false;
                    this.dataTypeComboBox.Visible = false;
                }
            }
            catch
            {
                // TODO: exception.
            }
        }

        private void SetupContextMenu()
        {
            this.fromSQLToolStripMenuItem.Enabled = false;

            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem1.Enabled = false;
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem1.Enabled = false;
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem1.Enabled = false;

            this.copyToolStripButton.Enabled = false;
            this.copyToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripButton.Enabled = false;
            this.pasteToolStripButton.Enabled = false;
            this.pasteToolStripMenuItem.Enabled = false;

            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
            this.selectAllToolStripMenuItem.Enabled = false;

            this.customizeToolStripMenuItem.Enabled = false;
            this.optionsToolStripMenuItem.Enabled = false;

            this.copyToolStripMenuItem2.Enabled = false;
            this.cutToolStripMenuItem2.Enabled = false;
            this.pasteToolStripMenuItem2.Enabled = false;

            this.printPreviewToolStripMenuItem.Enabled = false;
            this.printToolStripButton.Enabled = false;
            this.printToolStripMenuItem.Enabled = false;

            this.contentsToolStripMenuItem.Enabled = false;
            this.indexToolStripMenuItem.Enabled = false;
            this.searchToolStripMenuItem.Enabled = false;
            this.helpToolStripButton.Enabled = false;
        }

        private void NewDefinition()
        {
            this.definition = CreateTemplateDefinition();

            RefreshDefinition();
        }

        private void RefreshDefinition()
        {
            RefreshToolBar();
            RefreshTreeView();
        }

        private void RefreshToolBar()
        {
            this.toolStripTextBox1.Text = this.definition.Namespace;
            this.toolStripTextBox2.Text = this.definition.ProjectLocation;
        }

        private bool _showColumnNodes = true;

        public bool ShowColumnNodes
        {
            get { return _showColumnNodes; }
            set { _showColumnNodes = value; }
        }

        #region Create tree nodes.
        private TreeNode CreateDataColumnNode(EntitiesGenerator.Definitions.DataColumn column)
        {
            TreeNode treeNode = new TreeNode();
            if (this.showSourceName)
            {
                treeNode.Text = column.SourceName;
            }
            else
            {
                treeNode.Text = column.ColumnName;
            }
            treeNode.ImageKey = "Microsoft.VisualStudio.DataTools.Column.ico";
            treeNode.SelectedImageKey = treeNode.ImageKey;
            treeNode.Tag = column;
            column.PropertyChanged += new EventHandler(column_PropertyChanged);
            return treeNode;
        }

        private void column_PropertyChanged(object sender, EventArgs e)
        {
            UpdateColumnTreeNode(sender as EntitiesGenerator.Definitions.DataColumn);
            this.modified = true;
        }

        private TreeNode CreateDataTableNode(EntitiesGenerator.Definitions.DataTable table)
        {
            TreeNode treeNode = new TreeNode();
            if (this.showSourceName)
            {
                treeNode.Text = table.SourceName;
            }
            else
            {
                treeNode.Text = table.TableName;
            }
            treeNode.ImageKey = "Microsoft.VisualStudio.DataTools.Table.ico";
            treeNode.SelectedImageKey = treeNode.ImageKey;
            treeNode.Tag = table;
            table.PropertyChanged += new EventHandler(table_PropertyChanged);
            return treeNode;
        }

        private void table_PropertyChanged(object sender, EventArgs e)
        {
            UpdateTableTreeNode(sender as EntitiesGenerator.Definitions.DataTable);
            this.modified = true;
        }

        private TreeNode CreateDefinitionNode(Definition definition)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Tables";
            treeNode.ImageKey = "Microsoft.VisualStudio.DataTools.Database.ico";
            treeNode.SelectedImageKey = treeNode.ImageKey;
            treeNode.Tag = definition;
            return treeNode;
        }
        #endregion

        #region Private members
        private Definition CreateTemplateDefinition()
        {
            Definition definition = new Definition();
            definition.Namespace = "General.Data.Entities";
            definition.ProjectLocation = Application.StartupPath;
            //EntitiesGenerator.Definitions.DataTable table = new EntitiesGenerator.Definitions.DataTable("Table1");
            //table.Columns.Add(new EntitiesGenerator.Definitions.DataColumn("Column1"));
            //definition.Tables.Add(table);
            return definition;
        }

        private void RefreshTreeView()
        {
            this.treeView1.Nodes.Clear();

            TreeNode rootNode = CreateDefinitionNode(this.definition);
            this.treeView1.Nodes.Add(rootNode);

            foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
            {
                TreeNode tableNode = CreateDataTableNode(table);
                rootNode.Nodes.Add(tableNode);

                if (this._showColumnNodes)
                {
                    foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
                    {
                        TreeNode columnNode = CreateDataColumnNode(column);
                        tableNode.Nodes.Add(columnNode);
                    }
                }
            }

            // TODO: default selection.
            if (rootNode != null)
            {
                if (this.treeView1.SelectedNode == null)
                {
                    rootNode.Expand();
                    if (rootNode.Nodes.Count > 0)
                    {
                        TreeNode defaultNode = rootNode.Nodes[0];
                        defaultNode.Expand();
                        this.treeView1.SelectedNode = defaultNode;
                    }
                    else
                    {
                        this.treeView1.SelectedNode = rootNode;
                    }
                }
            }
        }

        private void UpdateTreeView()
        {
            if (this.treeView1.Nodes.Count > 0)
            {
                TreeNode definitionNode = this.treeView1.Nodes[0];
                if (definitionNode != null)
                {
                    foreach (TreeNode treeNode in definitionNode.Nodes)
                    {
                        UpdateTreeNode(treeNode);
                        foreach (TreeNode columnNode in treeNode.Nodes)
                        {
                            UpdateTreeNode(columnNode);
                        }
                    }
                }
            }
        }

        private void RefreshSelectedNode()
        {
            TreeNode selectedNode = this.treeView1.SelectedNode;
            RefreshTreeNode(selectedNode);
        }

        private void RefreshTreeNode(TreeNode treeNode)
        {
            UpdateTreeNode(treeNode);

            if (treeNode.Tag is Definition)
            {
                treeNode.Nodes.Clear();
                Definition definition = treeNode.Tag as Definition;
                foreach (EntitiesGenerator.Definitions.DataTable table in definition.Tables)
                {
                    TreeNode tableNode = CreateDataTableNode(table);
                    treeNode.Nodes.Add(tableNode);

                    if (this._showColumnNodes)
                    {
                        foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
                        {
                            TreeNode columnNode = CreateDataColumnNode(column);
                            tableNode.Nodes.Add(columnNode);
                        }
                    }
                }
            }
            else if (treeNode.Tag is EntitiesGenerator.Definitions.DataTable)
            {
                RefreshTableNode(treeNode);
            }
        }

        private void RefreshTableNode(TreeNode treeNode)
        {
            treeNode.Nodes.Clear();
            EntitiesGenerator.Definitions.DataTable table = treeNode.Tag as EntitiesGenerator.Definitions.DataTable;
            if (this._showColumnNodes)
            {
                foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
                {
                    TreeNode columnNode = CreateDataColumnNode(column);
                    treeNode.Nodes.Add(columnNode);
                }
            }
        }

        public void UpdateTreeNode(TreeNode treeNode)
        {
            if (treeNode.Tag is EntitiesGenerator.Definitions.DataTable)
            {
                if (this.showSourceName)
                {
                    treeNode.Text = (treeNode.Tag as EntitiesGenerator.Definitions.DataTable).SourceName;
                }
                else
                {
                    treeNode.Text = (treeNode.Tag as EntitiesGenerator.Definitions.DataTable).TableName;
                }
            }
            else if (treeNode.Tag is EntitiesGenerator.Definitions.DataColumn)
            {
                if (this.showSourceName)
                {
                    treeNode.Text = (treeNode.Tag as EntitiesGenerator.Definitions.DataColumn).SourceName;
                }
                else
                {
                    treeNode.Text = (treeNode.Tag as EntitiesGenerator.Definitions.DataColumn).ColumnName;
                }
            }
        }

        private void RefreshTreeNode(EntitiesGenerator.Definitions.DataTable table)
        {
            TreeNode definitionNode = this.treeView1.Nodes[0];
            if (definitionNode != null)
            {
                foreach (TreeNode treeNode in definitionNode.Nodes)
                {
                    if (treeNode.Tag == table)
                    {
                        RefreshTableNode(treeNode);
                        break;
                    }
                }
            }
        }

        private void UpdateTableTreeNode(EntitiesGenerator.Definitions.DataTable table)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            this.treeView1.BeginUpdate();
            TreeNode definitionNode = this.treeView1.Nodes[0];
            if (definitionNode != null)
            {
                foreach (TreeNode treeNode in definitionNode.Nodes)
                {
                    if (treeNode.Tag == table)
                    {
                        UpdateTreeNode(treeNode);
                        foreach (TreeNode columnNode in treeNode.Nodes)
                        {
                            UpdateTreeNode(columnNode);
                        }
                        break;
                    }
                }
            }
            this.treeView1.EndUpdate();
            sw.Stop();
            Console.WriteLine("UpdateTableTreeNode: {0}", sw.ElapsedMilliseconds);
            //MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }

        private void UpdateColumnTreeNode(EntitiesGenerator.Definitions.DataColumn column)
        {
            TreeNode definitionNode = this.treeView1.Nodes[0];
            if (definitionNode != null)
            {
                foreach (TreeNode treeNode in definitionNode.Nodes)
                {
                    foreach (TreeNode columnNode in treeNode.Nodes)
                    {
                        if (treeNode.Tag == column)
                        {
                            UpdateTreeNode(columnNode);
                            break;
                        }
                    }
                }
            }
        }

        private System.IO.FileInfo currentFile;
        private bool modified = false;

        private void OpenFile(string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            if (fileInfo.Exists)
            {
                switch (fileInfo.Extension)
                {
                    case ".xml":
                        {
                            OpenXmlFile(fileInfo);
                            break;
                        }
                    case ".sql":
                        {
                            OpenSqlFile(fileInfo);
                            break;
                        }
                }
            }
        }

        private void OpenSqlFile(System.IO.FileInfo fileInfo)
        {
            SqlFileReader reader = new SqlFileReader();
            this.definition = reader.ReadFile(fileInfo);
            if (this.definition != null)
            {
                RefreshDefinition();
            }
            else
            {
                string message = "Reading sql caused an exception, please check the file \"{0}\".";
                MessageBox.Show(message, "Open", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private bool OpenXmlFile(System.IO.FileInfo fileInfo)
        {
            try
            {
                System.IO.FileStream fileStream = fileInfo.OpenRead();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Definition));
                this.definition = xmlSerializer.Deserialize(fileStream) as Definition;
                fileStream.Close();

                // refresh tree view.
                RefreshDefinition();

                UpdateCurrentFile(fileInfo);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public const string APPLICATION_TITLE = "Entities Generator";
        private void UpdateCurrentFile(System.IO.FileInfo fileInfo)
        {
            // current fileinfo.
            this.currentFile = fileInfo;
            if (this.currentFile != null)
            {
                this.Text = string.Format("{0} - {1}", fileInfo.Name, APPLICATION_TITLE);
            }
            else
            {
                this.Text = APPLICATION_TITLE;
            }
            this.modified = false;
        }

        private void SaveFile(string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            SaveFile(fileInfo);
        }

        // TODO: in sql server it should be ";".
        public const string SQLSEPARATINGCHARACTER = "/";

        private void SaveSqlFile(System.IO.FileInfo fileInfo)
        {
            try
            {
                SqlGenerator sqlGenerator = new SqlGenerator();
                System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                {
                    string sqlText = sqlGenerator.GenerateForMSSQL(table);
                    streamWriter.Write(sqlText);
                    streamWriter.WriteLine(SQLSEPARATINGCHARACTER);
                }
                streamWriter.Flush();
                streamWriter.Close();

                UpdateCurrentFile(fileInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveXmlFile(System.IO.FileInfo fileInfo)
        {
            try
            {
                System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Definition));
                xmlSerializer.Serialize(streamWriter, this.definition);
                streamWriter.Flush();
                streamWriter.Close();

                UpdateCurrentFile(fileInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDataGridView()
        {
            this.dataGridView1.Tag = null;
            this.dataGridView1.DataSource = null;

            this.typeComboBox.Visible = false;
            this.dataTypeComboBox.Visible = false;
        }

        private void SelectDataGridViewRow(EntitiesGenerator.Definitions.DataColumn dataColumn)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.DataBoundItem == dataColumn)
                {
                    DataGridViewCell dataGridViewCell = this.dataGridView1[this.dataGridView1.CurrentCell.ColumnIndex, row.Index];
                    if (this.dataGridView1.CurrentCell != dataGridViewCell)
                    {
                        this.dataGridView1.CurrentCell = dataGridViewCell;
                    }
                    break;
                }
            }
        }

        private void RefreshDataGridView(EntitiesGenerator.Definitions.DataTable table)
        {
            ClearDataGridView();

            this.dataGridView1.Tag = table;
            this.dataGridView1.DataSource = table.Columns;

            //this.dataGridView1.Refresh();
        }

        private void UpdateDataGridViewStyle()
        {
            if (this.showSourceName)
            {
                this.Column_SourceName.HeaderCell.Style.ForeColor = Color.Blue;
                //this.Column_SourceName.HeaderCell.Style.Font = new Font(this.Column_SourceName.HeaderCell.Style.Font, FontStyle.Bold);
                this.Column_DataType.HeaderCell.Style.ForeColor = Color.Blue;
                //this.Column_DataType.HeaderCell.Style.Font = new Font(this.Column_DataType.HeaderCell.Style.Font, FontStyle.Bold);

                this.Column_ColumnName.HeaderCell.Style.ForeColor = Color.Black;
                //this.Column_ColumnName.HeaderCell.Style.Font = new Font(this.Column_ColumnName.HeaderCell.Style.Font, FontStyle.Regular);
                this.Column_Type.HeaderCell.Style.ForeColor = Color.Black;
                //this.Column_Type.HeaderCell.Style.Font = new Font(this.Column_Type.HeaderCell.Style.Font, FontStyle.Regular);
            }
            else
            {
                this.Column_SourceName.HeaderCell.Style.ForeColor = Color.Black;
                //this.Column_SourceName.HeaderCell.Style.Font = new Font(this.Column_SourceName.HeaderCell.Style.Font, FontStyle.Regular);
                this.Column_DataType.HeaderCell.Style.ForeColor = Color.Black;
                //this.Column_DataType.HeaderCell.Style.Font = new Font(this.Column_DataType.HeaderCell.Style.Font, FontStyle.Regular);

                this.Column_ColumnName.HeaderCell.Style.ForeColor = Color.Blue;
                //this.Column_ColumnName.HeaderCell.Style.Font = new Font(this.Column_ColumnName.HeaderCell.Style.Font, FontStyle.Bold);
                this.Column_Type.HeaderCell.Style.ForeColor = Color.Blue;
                //this.Column_Type.HeaderCell.Style.Font = new Font(this.Column_Type.HeaderCell.Style.Font, FontStyle.Bold);
            }
        }

        private void ShowPropertyOfSelectedRow()
        {
            DataGridViewRow row = this.dataGridView1.CurrentRow;
            if (row != null)
            {
                this.propertyGrid1.SelectedObject = row.DataBoundItem;
            }
        }

        private void GenerateProject()
        {
            if (Properties.Settings.Default.UseRazorEngine)
            {
                GenerateProjectWithRazorEngine();
            }
            else
            {
                CodeGenerator codeGenerator = new CodeGenerator();
                codeGenerator.ApplicationStartupPath = Application.StartupPath;
                codeGenerator.CodeNamespace = this.definition.Namespace;
                string projectDirectory = Path.Combine(this.definition.ProjectLocation, this.definition.Namespace.Trim());
                XmlGenerator xmlGenerator = new XmlGenerator();
                List<string> tables = new List<string>();
                List<string> newTypeList = new List<string>();
                foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                {
                    string designerCode, definitionCode, proxyCode;
                    string tableCode = codeGenerator.GenerateDataObject(table, this.toolStripComboBox2.Text == "XML", out designerCode, out definitionCode, out proxyCode, newTypeList);

                    string dataObjectName = table.TableName.Trim();

                    if (!System.IO.Directory.Exists(projectDirectory))
                    {
                        System.IO.Directory.CreateDirectory(projectDirectory);
                    }
                    System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".cs"), tableCode, System.Text.Encoding.UTF8);
                    System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".Designer.cs"), designerCode, System.Text.Encoding.UTF8);
                    System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".Expression.cs"), definitionCode, System.Text.Encoding.UTF8);
                    System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".Proxy.cs"), proxyCode, System.Text.Encoding.UTF8);

                    if (this.toolStripComboBox2.Text == "XML")
                    {
                        xmlGenerator.GenerateDefinitionXmlFile(table, projectDirectory);
                    }

                    tables.Add(dataObjectName);
                }
                foreach (string newType in newTypeList)
                {
                    string newTypeCode = codeGenerator.GenerateEnumCode(newType);
                    System.IO.File.WriteAllText(Path.Combine(projectDirectory, newType + ".cs"), newTypeCode, System.Text.Encoding.UTF8);
                }
                string projectFileName;
                bool result = codeGenerator.GenerateCSProject(projectDirectory, tables.ToArray(), newTypeList.ToArray(), (ProjectVesion)(Enum.Parse(typeof(ProjectVesion), this.toolStripComboBox1.Text)), this.toolStripComboBox2.Text == "XML", out projectFileName);
                if (result && !string.IsNullOrEmpty(projectFileName))
                {
                    string fullPathProjectFileName = Path.Combine(projectDirectory, projectFileName);
                    // TODO: message.
                    string message = "Do you want to open the created project?";
                    if (MessageBox.Show(message, "Open project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(fullPathProjectFileName);
                    }
                }
            }
        }

        private void ShowPropertyOfSelectedNode()
        {
            TreeNode selectedNode = this.treeView1.SelectedNode;
            if (selectedNode != null)
            {
                this.propertyGrid1.SelectedObject = selectedNode.Tag;
            }
        }

        private void ShowPropertyOfNode(TreeNode treeNode)
        {
            this.propertyGrid1.SelectedObject = treeNode.Tag;
        }

        private int tableIndex = 0;
        public const string DEFAULT_TABLENAME = "Table";

        private string GenerateNewTableName()
        {
            // TODO: generate table name for new table.
            return DEFAULT_TABLENAME + (++tableIndex).ToString();
        }

        private int columnIndex = 0;
        public const string DEFAULT_COLUMNNAME = "Column";

        private string GenerateNewColumnName()
        {
            // TODO: generate a column name for new column.
            return DEFAULT_COLUMNNAME + (++columnIndex).ToString();
        }

        private Definition GetParentDefinition(TreeNode treeNode)
        {
            TreeNode parentNode = treeNode.Parent;
            if ((parentNode != null) && (parentNode.Tag is Definition))
            {
                return parentNode.Tag as Definition;
            }
            return null;
        }
        #endregion

        #region events
        private bool treenode_selected_processing = false;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!this.treenode_selected_processing)
            {
                if (e.Node != null)
                {
                    TreeNode treeNode = e.Node;

                    UpdateFormDisplay(treeNode);
                }
            }
        }

        private void UpdateFormDisplay(TreeNode treeNode)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ShowPropertyOfNode(treeNode);

            if (treeNode.Tag is EntitiesGenerator.Definitions.DataTable)
            {
                EntitiesGenerator.Definitions.DataTable table = treeNode.Tag as EntitiesGenerator.Definitions.DataTable;

                RefreshDataGridView(table);
                ShowPropertyOfNode(treeNode);
            }
            else if (treeNode.Tag is EntitiesGenerator.Definitions.DataColumn)
            {
                EntitiesGenerator.Definitions.DataTable table = treeNode.Parent.Tag as EntitiesGenerator.Definitions.DataTable;

                this.treenode_selected_processing = true;
                RefreshDataGridView(table);
                SelectDataGridViewRow(treeNode.Tag as EntitiesGenerator.Definitions.DataColumn);
                this.treenode_selected_processing = false;
            }
            else
            {
                ClearDataGridView();
            }
            sw.Stop();
            Console.WriteLine("UpdateFormDisplay: {0}", sw.ElapsedMilliseconds);
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            SetupComboBoxForDataGrid();
            ShowPropertyOfSelectedRow();
            UpdateTreeViewSelectedNode();
        }

        private void UpdateTreeViewSelectedNode()
        {
            if (!this.treenode_selected_processing)
            {
                DataGridViewRow row = this.dataGridView1.CurrentRow;
                if (row != null)
                {
                    TreeNode selectedNode = this.treeView1.SelectedNode;
                    if (selectedNode.Tag is EntitiesGenerator.Definitions.DataColumn)
                    {
                        EntitiesGenerator.Definitions.DataColumn selectedNodeDataColumn = selectedNode.Tag as EntitiesGenerator.Definitions.DataColumn;
                        EntitiesGenerator.Definitions.DataColumn selectedRowDataColumn = row.DataBoundItem as EntitiesGenerator.Definitions.DataColumn;
                        if ((selectedNodeDataColumn != null) && (selectedRowDataColumn != null))
                        {
                            if (selectedNodeDataColumn != selectedRowDataColumn)
                            {
                                foreach (TreeNode childNode in selectedNode.Parent.Nodes)
                                {
                                    if (childNode.Tag is EntitiesGenerator.Definitions.DataColumn)
                                    {
                                        EntitiesGenerator.Definitions.DataColumn childNodeDataColumn = childNode.Tag as EntitiesGenerator.Definitions.DataColumn;
                                        if (childNodeDataColumn == selectedRowDataColumn)
                                        {
                                            this.treenode_selected_processing = true;
                                            if (this.treeView1.SelectedNode != childNode)
                                            {
                                                this.treeView1.SelectedNode = childNode;
                                            }
                                            this.treenode_selected_processing = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Forms.TreeNode _targetNode = e.Node;
                if (_targetNode != null)
                {
                    this.treeView1.SelectedNode = _targetNode;
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if ((e.RowIndex >= 0) && (e.ColumnIndex >= 0))
                {
                    this.dataGridView1.CurrentCell = this.dataGridView1[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // TODO: refresh tree view.
            if (this._showColumnNodes)
            {
                UpdateTableTreeNode(this.dataGridView1.Tag as EntitiesGenerator.Definitions.DataTable);
            }
            // TODO: update the column name.

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if ((e.RowIndex >= 0) && ((e.ColumnIndex == 2) || (e.ColumnIndex == 3)))
            //{
            //    this.dataGridView1.BeginEdit(true);
            //    ComboBox comboBox = (ComboBox)this.dataGridView1.EditingControl;
            //    comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            //    //comboBox.DroppedDown = true;
            //}
        }


        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            this.dataTypeComboBox.Visible = false;
            this.typeComboBox.Visible = false;
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (this.dataTypeComboBox != null)
            {
                this.dataTypeComboBox.Visible = false;
            }
            if (this.typeComboBox != null)
            {
                this.typeComboBox.Visible = false;
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }
        #endregion

        #region menu and toolbars
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataGridView1.DataSource == null)
            {
                this.toolStripMenuItem7.Enabled = false;
                this.newToolStripMenuItem2.Enabled = false;
                this.copyToolStripMenuItem2.Enabled = false;
                this.cutToolStripMenuItem2.Enabled = false;
                this.pasteToolStripMenuItem2.Enabled = false;
                this.deleteToolStripMenuItem.Enabled = false;
                this.refreshToolStripMenuItem1.Enabled = false;
                this.propertyToolStripMenuItem1.Enabled = false;
            }
            else
            {
                this.toolStripMenuItem7.Enabled = true;
                this.newToolStripMenuItem2.Enabled = true;
                //this.copyToolStripMenuItem2.Enabled = true;
                //this.cutToolStripMenuItem2.Enabled = true;
                //this.pasteToolStripMenuItem2.Enabled = true;
                this.deleteToolStripMenuItem.Enabled = true;
                this.refreshToolStripMenuItem1.Enabled = true;
                this.propertyToolStripMenuItem1.Enabled = true;
            }
        }

        private void refreshTreeView(object sender, EventArgs e)
        {
            RefreshSelectedNode();
        }

        private void refreshGridView(object sender, EventArgs e)
        {
            EntitiesGenerator.Definitions.DataTable table = this.dataGridView1.Tag as EntitiesGenerator.Definitions.DataTable;
            RefreshDataGridView(table);
        }

        private void propertyOfTreeView(object sender, EventArgs e)
        {
            ShowPropertyOfSelectedNode();
        }

        private void propertyOfGridView(object sender, EventArgs e)
        {
            ShowPropertyOfSelectedRow();
        }

        private void createProject(object sender, EventArgs e)
        {
            // check table and column name.
            if (CheckTableAndColumnNames())
            {
                System.Windows.Forms.Cursor currentCursor = System.Windows.Forms.Cursor.Current;
                try
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    GenerateProject();
                }
                finally
                {
                    System.Windows.Forms.Cursor.Current = currentCursor;
                }
            }
        }

        private bool CheckTableAndColumnNames()
        {
            List<string> tableNames = new List<string>();
            foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
            {
                string dataObjectName = table.TableName.Trim();
                if (!tableNames.Contains(dataObjectName))
                {
                    tableNames.Add(dataObjectName);
                }
                else
                {
                    string message = string.Format("Table [{0}] has been exist, do you want to continue?", dataObjectName);
                    if (MessageBox.Show(message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                List<string> columnNames = new List<string>();
                foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
                {
                    string columnName = column.ColumnName;
                    if (columnName == dataObjectName)
                    {
                        string message = string.Format("Column name [{1}] is same with table name [{0}], do you want to continue?", dataObjectName, columnName);
                        if (MessageBox.Show(message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!columnNames.Contains(columnName))
                        {
                            columnNames.Add(columnName);
                        }
                        else
                        {
                            string message = string.Format("Column [{1}] in table [{0}] has been exist, do you want to continue?", dataObjectName, columnName);
                            if (MessageBox.Show(message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void newTable(object sender, EventArgs e)
        {
            TreeNode definitionNode = this.treeView1.Nodes[0];
            if (definitionNode != null)
            {
                Definition definition = definitionNode.Tag as Definition;
                if (definition != null)
                {
                    string tableName = GenerateNewTableName();
                    EntitiesGenerator.Definitions.DataTable table = new EntitiesGenerator.Definitions.DataTable(tableName);
                    string columnName = GenerateNewColumnName();
                    EntitiesGenerator.Definitions.DataColumn column = new EntitiesGenerator.Definitions.DataColumn(columnName);
                    table.Columns.Add(column);
                    definition.Tables.Add(table);

                    // Create new table tree node.
                    TreeNode tableNode = CreateDataTableNode(table);
                    TreeNode columnNode = CreateDataColumnNode(column);
                    tableNode.Nodes.Add(columnNode);
                    definitionNode.Nodes.Add(tableNode);
                    if (!definitionNode.IsExpanded)
                    {
                        definitionNode.Expand();
                    }
                    if (!tableNode.IsExpanded)
                    {
                        tableNode.Expand();
                    }
                    this.treeView1.SelectedNode = tableNode;

                    this.modified = true;
                }
            }
        }

        private void newColumn(object sender, EventArgs e)
        {
            if (this.dataGridView1.DataSource != null)
            {
                List<EntitiesGenerator.Definitions.DataColumn> columns = this.dataGridView1.DataSource as List<EntitiesGenerator.Definitions.DataColumn>;
                if (columns != null)
                {
                    string columnName = GenerateNewColumnName();
                    EntitiesGenerator.Definitions.DataColumn column = new EntitiesGenerator.Definitions.DataColumn(columnName);
                    columns.Add(column);

                    RefreshDataGridView(this.dataGridView1.Tag as EntitiesGenerator.Definitions.DataTable);
                    SelectDataGridViewRow(column);

                    // TODO: refresh tree view.
                    if (this._showColumnNodes)
                    {
                        RefreshTreeNode(this.dataGridView1.Tag as EntitiesGenerator.Definitions.DataTable);
                    }

                    this.modified = true;
                }
            }
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = this.treeView1.SelectedNode;
            if (selectedNode != null)
            {
                if (selectedNode.Tag is EntitiesGenerator.Definitions.DataTable)
                {
                    EntitiesGenerator.Definitions.DataTable table = selectedNode.Tag as EntitiesGenerator.Definitions.DataTable;
                    string message = string.Format("Table [{0}] will be deleted.", this.showSourceName ? table.SourceName : table.TableName);
                    if (MessageBox.Show(message, "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        Definition definition = GetParentDefinition(selectedNode);
                        definition.Tables.Remove(table);
                        // remove the tree node.
                        selectedNode.Remove();

                        this.modified = true;
                    }
                }
                else if (selectedNode.Tag is EntitiesGenerator.Definitions.DataColumn)
                {
                    // TODO: delete column from tree view, refresh the datagrid view.
                    EntitiesGenerator.Definitions.DataTable table = selectedNode.Parent.Tag as EntitiesGenerator.Definitions.DataTable;
                    EntitiesGenerator.Definitions.DataColumn column = selectedNode.Tag as EntitiesGenerator.Definitions.DataColumn;
                    if ((table != null) && (column != null))
                    {
                        if (table.Columns.Count > 1)
                        {
                            string message = string.Format("Column [{0}] in table [{1}] will be deleted.", this.showSourceName ? column.SourceName : column.ColumnName, this.showSourceName ? table.SourceName : table.TableName);
                            if (MessageBox.Show(message, "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                            {
                                table.Columns.Remove(column);
                                // remove the tree node.
                                selectedNode.Remove();

                                // refresh the datagrid view.
                                RefreshDataGridView(table);

                                this.modified = true;
                            }
                        }
                        else
                        {
                            string message = "There should be at least one column in a table.";
                            MessageBox.Show(message, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Tag is EntitiesGenerator.Definitions.DataTable)
            {
                EntitiesGenerator.Definitions.DataTable table = this.dataGridView1.Tag as EntitiesGenerator.Definitions.DataTable;
                if (table.Columns.Count > 1)
                {
                    if (this.dataGridView1.CurrentRow != null)
                    {
                        EntitiesGenerator.Definitions.DataColumn column = this.dataGridView1.CurrentRow.DataBoundItem as EntitiesGenerator.Definitions.DataColumn;
                        if (column != null)
                        {
                            string message = string.Format("Column [{0}] in table [{1}] will be deleted.", this.showSourceName ? column.SourceName : column.ColumnName, this.showSourceName ? table.SourceName : table.TableName);
                            if (MessageBox.Show(message, "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                            {
                                table.Columns.Remove(column);
                                RefreshDataGridView(table);

                                // TODO: refresh tree view.
                                if (this._showColumnNodes)
                                {
                                    RefreshTreeNode(table);
                                }
                                this.modified = true;
                            }
                        }
                    }
                }
                else
                {
                    string message = "There should be at least one column in a table.";
                    MessageBox.Show(message, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void SetPrimaryKey(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow != null)
            {
                EntitiesGenerator.Definitions.DataColumn column = this.dataGridView1.CurrentRow.DataBoundItem as EntitiesGenerator.Definitions.DataColumn;
                if (column != null)
                {
                    column.PrimaryKey = true;
                    column.AllowDBNull = false;

                    // TODO: refresh the datagrid view.
                    this.dataGridView1.EndEdit();
                    this.dataGridView1.Refresh();
                }
            }
        }

        private void newFile(object sender, EventArgs e)
        {
            if (this.modified)
            {
                DialogResult dialogResult = ShowSavingMessageBox();
                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    this.saveFile(sender, e);
                }
            }

            NewDefinition();
            UpdateCurrentFile(null);
        }

        private static DialogResult ShowSavingMessageBox()
        {
            string message = "The definition in this file has been changed. \n\nDo you want to save the changes?";
            DialogResult dialogResult = MessageBox.Show(message, "Saving", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            return dialogResult;
        }

        private void openFile(object sender, EventArgs e)
        {
            if (this.modified)
            {
                DialogResult dialogResult = ShowSavingMessageBox();
                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    this.saveFile(sender, e);
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "xml files (*.xml)|*.xml|sql files (*.sql)|*.sql";
            openFileDialog.Filter = "xml files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                OpenFile(fileName);
            }
        }

        private void ImportFromDatabase(object sender, EventArgs e)
        {
            DatabaseConnectionDialog dialog = new DatabaseConnectionDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // TODO: import from database.
                string databaseType = dialog.DatabaseType;
                string connectionString = dialog.ConnectionString;
                if (databaseType == "SharePoint")
                {
                    ImportFromSharePoint(connectionString);
                }
                else
                {
                    DatabaseReader reader = new DatabaseReader();
                    Definitions.Definition definition = reader.ReadDatabase(databaseType, connectionString);
                    AppendDefinition(definition);
                }
            }
        }

        private void AppendDefinition(Definition definition)
        {
            if (definition != null)
            {
                if (this.definition != null)
                {
                    // TODO:
                    foreach (EntitiesGenerator.Definitions.DataTable dataTable in definition.Tables)
                    {
                        this.definition.Tables.Add(dataTable);
                    }
                }
                else
                {
                    this.definition = definition;
                }

                RefreshDefinition();
            }
        }

        private void saveFile(object sender, EventArgs e)
        {
            if (this.currentFile != null)
            {
                SaveFile(this.currentFile);
            }
            else
            {
                this.saveAsFile(sender, e);
            }
        }

        private void SaveFile(System.IO.FileInfo fileInfo)
        {
            switch (fileInfo.Extension)
            {
                case ".xml":
                    {
                        SaveXmlFile(fileInfo);
                        break;
                    }
                //case ".sql":
                //    {
                //        SaveSqlFile(fileInfo);
                //        break;
                //    }
                default:
                    {
                        fileInfo = new System.IO.FileInfo(string.Format("{0}{1}", fileInfo.FullName, ".xml"));
                        SaveXmlFile(fileInfo);
                        break;
                    }
            }
        }

        private void saveAsFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "xml files (*.xml)|*.xml|sql files (*.sql)|*.sql";
            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                SaveFile(fileName);
            }
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.definition.Namespace = this.toolStripTextBox1.Text;
            this.modified = true;
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            this.definition.ProjectLocation = this.toolStripTextBox2.Text;
            this.modified = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = this.definition.ProjectLocation;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                this.toolStripTextBox2.Text = folderDialog.SelectedPath;
            }
        }

        private bool showSourceName = false;
        private void showSourceNameToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (this.toolStripButton3.Checked != this.showSourceNameToolStripMenuItem.Checked)
            {
                this.toolStripButton3.Checked = this.showSourceNameToolStripMenuItem.Checked;
            }
            this.showSourceName = this.showSourceNameToolStripMenuItem.Checked;
            UpdateTreeView();
        }

        private void toolStripButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.showSourceNameToolStripMenuItem.Checked != this.toolStripButton3.Checked)
            {
                this.showSourceNameToolStripMenuItem.Checked = this.toolStripButton3.Checked;
            }
            this.showSourceName = this.showSourceNameToolStripMenuItem.Checked;
            UpdateTreeView();
            UpdateDataGridViewStyle();
        }

        public const string SQLSERVER_SQLSEPARATINGCHARACTER = ";";
        private void sQLForSQLServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sql files (*.sql)|*.sql";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForMSSQL(table);
                        streamWriter.Write(sqlText);
                        streamWriter.WriteLine(SQLSERVER_SQLSEPARATINGCHARACTER);
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public const string MYSQL_SQLSEPARATINGCHARACTER = ";";
        private void sQLForMySQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sql files (*.sql)|*.sql";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForMYSQL(table);
                        streamWriter.Write(sqlText);
                        streamWriter.WriteLine(MYSQL_SQLSEPARATINGCHARACTER);
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public const string ORACLE_SQLSEPARATINGCHARACTER = "/";
        private void sQLForOracelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sql files (*.sql)|*.sql";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForOracle(table);
                        streamWriter.Write(sqlText);
                        streamWriter.WriteLine(ORACLE_SQLSEPARATINGCHARACTER);
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public const string POSTGRE_SQLSEPARATINGCHARACTER = "/g";
        private void sQLForPostgreSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sql files (*.sql)|*.sql";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForPostgreSQL(table);
                        streamWriter.Write(sqlText);
                        streamWriter.WriteLine(POSTGRE_SQLSEPARATINGCHARACTER);
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void fromSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.modified)
            {
                DialogResult dialogResult = ShowSavingMessageBox();
                if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    this.saveFile(sender, e);
                }
            }
        }

        private void ReplaceTextBoxSelectedText(TextBox textBox, string selectedText)
        {
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;
            textBox.Text = ReplaceText(textBox.Text, selectionStart, selectionLength, selectedText);
            textBox.SelectionStart = selectionStart;
            textBox.SelectionLength = selectionLength;
        }

        private string ReplaceText(string text, int selectionStart, int selectionLength, string selectedText)
        {
            return string.Concat(text.Substring(0, selectionStart), selectedText, text.Substring(selectionStart + selectionLength, text.Length - (selectionStart + selectionLength)));
        }

        private void makeUppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCell != null)
            {
                if (this.dataGridView1.CurrentCell.ColumnIndex < 2)
                {
                    if (this.dataGridView1.IsCurrentCellInEditMode)
                    {
                        if (this.dataGridView1.EditingControl != null)
                        {
                            if (this.dataGridView1.EditingControl is TextBox)
                            {
                                TextBox textBox = this.dataGridView1.EditingControl as TextBox;
                                string selectedText = textBox.SelectedText;
                                if (!string.IsNullOrEmpty(selectedText))
                                {
                                    selectedText = selectedText.ToUpper();
                                    ReplaceTextBoxSelectedText(textBox, selectedText);
                                    return;
                                }
                            }
                            this.dataGridView1.EditingControl.Text = this.dataGridView1.EditingControl.Text.ToUpper();
                        }
                    }
                    else
                    {
                        this.dataGridView1.CurrentCell.Value = (this.dataGridView1.CurrentCell.Value as string).ToUpper();
                    }
                }
            }
        }

        private void makeLowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCell != null)
            {
                if (this.dataGridView1.CurrentCell.ColumnIndex < 2)
                {
                    if (this.dataGridView1.IsCurrentCellInEditMode)
                    {
                        if (this.dataGridView1.EditingControl != null)
                        {
                            if (this.dataGridView1.EditingControl is TextBox)
                            {
                                TextBox textBox = this.dataGridView1.EditingControl as TextBox;
                                string selectedText = textBox.SelectedText;
                                if (!string.IsNullOrEmpty(selectedText))
                                {
                                    selectedText = selectedText.ToLower();
                                    ReplaceTextBoxSelectedText(textBox, selectedText);
                                    return;
                                }
                            }
                            this.dataGridView1.EditingControl.Text = this.dataGridView1.EditingControl.Text.ToLower();
                        }
                    }
                    else
                    {
                        this.dataGridView1.CurrentCell.Value = (this.dataGridView1.CurrentCell.Value as string).ToLower();
                    }
                }
            }
        }

        #region LabelEdit
        public const uint TV_FIRST = 0x1100;
        public const uint TVM_EDITLABELA = (TV_FIRST + 14);
        public const uint TVM_EDITLABELW = (TV_FIRST + 65);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, HandleRef lParam);

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                try
                {
                    System.Windows.Forms.TreeView treeView = this.treeView1;
                    System.Windows.Forms.TreeNode node = this.treeView1.SelectedNode;
                    SendMessage(new HandleRef(treeView, treeView.Handle), TVM_EDITLABELA, IntPtr.Zero, new HandleRef(node, node.Handle));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (e.Label != null)
                {
                    TreeNode treeNode = e.Node;
                    if (treeNode.Tag is EntitiesGenerator.Definitions.DataTable)
                    {
                        EntitiesGenerator.Definitions.DataTable table = treeNode.Tag as EntitiesGenerator.Definitions.DataTable;
                        if (this.showSourceName)
                        {
                            table.SourceName = e.Label;
                        }
                        else
                        {
                            table.TableName = e.Label;
                        }
                    }
                    else if (treeNode.Tag is EntitiesGenerator.Definitions.DataColumn)
                    {
                        EntitiesGenerator.Definitions.DataColumn column = treeNode.Tag as EntitiesGenerator.Definitions.DataColumn;
                        if (this.showSourceName)
                        {
                            column.SourceName = e.Label;
                        }
                        else
                        {
                            column.ColumnName = e.Label;
                        }
                    }
                    UpdateFormDisplay(treeNode);
                }
            }
        }
        #endregion

        #region DataSet
        private void fromDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DataSet files (*.xsd)|*.xsd";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    System.Data.DataSet dataSet = new System.Data.DataSet();
                    dataSet.ReadXmlSchema(fileName);

                    Definitions.Definition definition = ConvertToDefinition(dataSet);
                    AppendDefinition(definition);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Definition ConvertToDefinition(System.Data.DataSet dataSet)
        {
            Definition definition = new Definition();
            if (dataSet != null)
            {
                foreach (System.Data.DataTable dataTable in dataSet.Tables)
                {
                    EntitiesGenerator.Definitions.DataTable dataTableDefinition = new EntitiesGenerator.Definitions.DataTable();
                    dataTableDefinition.TableName = dataTable.TableName;
                    dataTableDefinition.SourceName = dataTable.TableName;
                    List<System.Data.DataColumn> primaryKeys = new List<System.Data.DataColumn>(dataTable.PrimaryKey);
                    foreach (System.Data.DataColumn dataColumn in dataTable.Columns)
                    {
                        EntitiesGenerator.Definitions.DataColumn dataColumnDefinition = new EntitiesGenerator.Definitions.DataColumn();
                        dataColumnDefinition.ColumnName = dataColumn.ColumnName;
                        dataColumnDefinition.SourceName = dataColumn.ColumnName;
                        dataColumnDefinition.AllowDBNull = dataColumn.AllowDBNull;
                        dataColumnDefinition.AutoIncrement = dataColumn.AutoIncrement;
                        dataColumnDefinition.DataType = TypeManager.GetWellKnownDataTypeName(dataColumn.DataType);
                        dataColumnDefinition.Type = TypeManager.GetWellKnownDataTypeName(dataColumn.DataType);
                        dataColumnDefinition.PrimaryKey = primaryKeys.Contains(dataColumn);
                        dataTableDefinition.Columns.Add(dataColumnDefinition);
                    }
                    definition.Tables.Add(dataTableDefinition);
                }
            }
            return definition;
        }


        private void toDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DataSet files (*.xsd)|*.xsd";
            saveFileDialog.FileName = "DataSet.xsd";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.Data.DataSet dataSet = ConvertToDataSet(this.definition, this.showSourceName);
                    dataSet.WriteXmlSchema(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private System.Data.DataSet ConvertToDataSet(Definition definition, bool sourceName)
        {
            System.Data.DataSet dataSet = new System.Data.DataSet();
            if (definition != null)
            {
                foreach (EntitiesGenerator.Definitions.DataTable dataTableDefinition in definition.Tables)
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.TableName = sourceName ? dataTableDefinition.SourceName : dataTableDefinition.TableName;
                    List<System.Data.DataColumn> primaryKeys = new List<System.Data.DataColumn>();
                    foreach (EntitiesGenerator.Definitions.DataColumn dataColumnDefinition in dataTableDefinition.Columns)
                    {
                        System.Data.DataColumn dataColumn = new System.Data.DataColumn();
                        dataColumn.ColumnName = sourceName ? dataColumnDefinition.SourceName : dataColumnDefinition.ColumnName;
                        dataColumn.AllowDBNull = dataColumnDefinition.AllowDBNull;
                        dataColumn.AutoIncrement = dataColumnDefinition.AutoIncrement;
                        if (sourceName)
                        {
                            Type dataType = TypeManager.GetWellKnownDataType(dataColumnDefinition.DataType);
                            if (dataType != null)
                            {
                                dataColumn.DataType = dataType;
                            }
                        }
                        else
                        {
                            Type dataType = TypeManager.GetWellKnownDataType(dataColumnDefinition.Type);
                            if (dataType != null)
                            {
                                dataColumn.DataType = dataType;
                            }
                        }
                        if (dataColumnDefinition.PrimaryKey)
                        {
                            primaryKeys.Add(dataColumn);
                        }
                        dataTable.Columns.Add(dataColumn);
                    }
                    dataTable.PrimaryKey = primaryKeys.ToArray();
                    dataSet.Tables.Add(dataTable);
                }
            }
            return dataSet;
        }
        #endregion

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataGridView1.DataSource == null)
            {
                this.toolStripMenuItem16.Visible = false;
            }
            else
            {
                this.toolStripMenuItem16.Visible = true;
            }
        }

        public const string SQLITE_SQLSEPARATINGCHARACTER = ";";
        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sql files (*.sql)|*.sql";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    System.IO.StreamWriter streamWriter = fileInfo.CreateText();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForSQLite(table);
                        streamWriter.Write(sqlText);
                        streamWriter.WriteLine(SQLITE_SQLSEPARATINGCHARACTER);
                        streamWriter.WriteLine();
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toSQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO:
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite3 DB (*.db)|*.db";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                    //fileInfo.Create();
                    SQLiteConnection.CreateFile(fileInfo.FullName);

                    string connectionString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", fileInfo.FullName);
                    var connection = new System.Data.SQLite.SQLiteConnection(connectionString);
                    connection.Open();
                    int count = 0;
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForSQLite(table);

                        var command = connection.CreateCommand();
                        command.CommandText = sqlText;
                        count += command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        System.Data.DataTable ExecuteForDataTable(SQLiteConnection connection, string commandText)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
            dataAdapter.SelectCommand = connection.CreateCommand();
            dataAdapter.SelectCommand.CommandText = commandText;
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        private void fromSQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite3 DB (*.db)|*.db";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    string connectionString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", fileName);
                    string commandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name <> 'sqlite_sequence'";
                    var connection = new System.Data.SQLite.SQLiteConnection(connectionString);
                    System.Data.DataTable schema = ExecuteForDataTable(connection, commandText);
                    Definitions.Definition definition = ConvertToDefinitionFromSchema(connection, schema);
                    AppendDefinition(definition);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Definition ConvertToDefinitionFromSchema(SQLiteConnection connection, System.Data.DataTable schema)
        {
            // TODO:
            Definition definition = new Definition();
            if (schema != null)
            {
                foreach (System.Data.DataRow dataRow in schema.Rows)
                {
                    string tableName = dataRow["tbl_name"] as string;
                    string sql = dataRow["sql"] as string;
                    if (!string.IsNullOrEmpty(sql))
                    {
                        EntitiesGenerator.Definitions.DataTable dataTableDefinition = ConvertToDefinition(connection, tableName, sql);
                        if (dataTableDefinition != null)
                        {
                            definition.Tables.Add(dataTableDefinition);
                        }
                    }
                }
            }
            return definition;
        }

        private EntitiesGenerator.Definitions.DataTable ConvertToDefinition(SQLiteConnection connection, string tableName, string sql)
        {
            string commandText = string.Format("PRAGMA table_info('{0}')", tableName);
            System.Data.DataTable tableInfo = ExecuteForDataTable(connection, commandText);

            EntitiesGenerator.Definitions.DataTable dataTableDefinition = new EntitiesGenerator.Definitions.DataTable();
            dataTableDefinition.TableName = tableName;
            dataTableDefinition.SourceName = tableName;
            List<System.Data.DataColumn> primaryKeys = new List<System.Data.DataColumn>(tableInfo.PrimaryKey);
            foreach (System.Data.DataRow dataRow in tableInfo.Rows)
            {
                string columnName = dataRow["name"] as string;
                string type = dataRow["type"] as string;

                EntitiesGenerator.Definitions.DataColumn dataColumnDefinition = new EntitiesGenerator.Definitions.DataColumn();
                dataColumnDefinition.ColumnName = columnName;
                dataColumnDefinition.SourceName = columnName;
                dataColumnDefinition.AllowDBNull = Convert.ToInt32(dataRow["notnull"]) == 0 ? true : false;
                string expression = @"\W\[{0}\]\s[^,]*AUTOINCREMENT|\W{0}\s[^,]*AUTOINCREMENT";
                bool autoIncrement = new Regex(string.Format(expression, columnName.ToUpper())).IsMatch(sql);
                dataColumnDefinition.AutoIncrement = autoIncrement;
                dataColumnDefinition.DataType = type;
                dataColumnDefinition.Type = GetWellKnownDataTypeName(type);
                dataColumnDefinition.PrimaryKey = Convert.ToInt32(dataRow["pk"]) == 0 ? false : true;
                dataTableDefinition.Columns.Add(dataColumnDefinition);
            }
            return dataTableDefinition;
        }

        private string GetWellKnownDataTypeName(string type)
        {
            DbType dbType = DbType.String;
            switch (type)
            {
                case "INTEGER":
                    {
                        dbType = DbType.Int32;
                        break;
                    }
                case "FLOAT":
                    {
                        dbType = DbType.Single;
                        break;
                    }
                case "REAL":
                    {
                        dbType = DbType.Double;
                        break;
                    }
                case "NUMERIC":
                    {
                        dbType = DbType.Decimal;
                        break;
                    }
                case "BOOLEAN":
                    {
                        dbType = DbType.Boolean;
                        break;
                    }
                case "TIME":
                    {
                        dbType = DbType.DateTime;
                        break;
                    }
                case "DATE":
                    {
                        dbType = DbType.DateTime;
                        break;
                    }
                case "TIMESTAMP":
                    {
                        dbType = DbType.DateTime;
                        break;
                    }
                case "BLOB":
                    {
                        dbType = DbType.Binary;
                        break;
                    }
            }
            return TypeManager.GetWellKnownDbTypeName(dbType);
        }

        private void fromSharePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseConnectionDialog dialog = new DatabaseConnectionDialog();
            dialog.DatabaseType = "SharePoint";
            dialog.DatabaseTypeComboBoxEnabled = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // TODO: import from sharepoint.                
                string connectionString = dialog.ConnectionString;
                ImportFromSharePoint(connectionString);
            }
        }

        private void ImportFromSharePoint(string connectionString)
        {
            EntitiesGenerator.Definitions.Definition definition = GetDefinitionFromSharePoint(connectionString);
            if ((definition != null) && (definition.Tables.Count > 0))
            {
                bool useNamingConvetion = false;
                string message = "Do you want to apply Naming Convention to Table Name(s) and Column Name(s)?";
                if (MessageBox.Show(message, "Naming Convention", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    useNamingConvetion = true;
                }
                if (this.definition != null)
                {
                    // TODO:
                    foreach (EntitiesGenerator.Definitions.DataTable dataTable in definition.Tables)
                    {
                        if (useNamingConvetion)
                        {
                            dataTable.TableName = DatabaseReader.GetConventionName(dataTable.TableName);
                            foreach (EntitiesGenerator.Definitions.DataColumn dataColumn in dataTable.Columns)
                            {
                                dataColumn.ColumnName = DatabaseReader.GetConventionName(dataColumn.ColumnName);
                            }
                        }
                        this.definition.Tables.Add(dataTable);
                    }
                }
                else
                {
                    if (useNamingConvetion)
                    {
                        foreach (EntitiesGenerator.Definitions.DataTable dataTable in definition.Tables)
                        {
                            dataTable.TableName = DatabaseReader.GetConventionName(dataTable.TableName);
                            foreach (EntitiesGenerator.Definitions.DataColumn dataColumn in dataTable.Columns)
                            {
                                dataColumn.ColumnName = DatabaseReader.GetConventionName(dataColumn.ColumnName);
                            }
                        }
                    }
                    this.definition = definition;
                }

                RefreshDefinition();
            }
        }

        private static EntitiesGenerator.Definitions.Definition GetDefinitionFromSharePoint(string connectionString)
        {
            EntitiesGenerator.Definitions.Definition definition = new EntitiesGenerator.Definitions.Definition();
            SharePointConnection sharePointConnection = new SharePointConnection(connectionString);
            System.Data.DataTable tables = sharePointConnection.GetTables();
            SelectTableForm selectTableForm = new SelectTableForm();
            foreach (System.Data.DataRow dataRow in tables.Rows)
            {
                string tableName = dataRow["Title"] as string;
                ListViewItem item = new ListViewItem();
                item.Text = tableName;
                item.Tag = dataRow;
                item.ImageIndex = 0;
                item.Checked = true;
                selectTableForm.AddListViewItem(item);
            }
            if (selectTableForm.ShowDialog() == DialogResult.OK)
            {
                System.Windows.Forms.ListView.CheckedListViewItemCollection checkedItems = selectTableForm.GetCheckedItems();
                foreach (ListViewItem item in checkedItems)
                {
                    System.Data.DataRow dataRow = item.Tag as System.Data.DataRow;
                    string listUrl = (dataRow["RootFolderUrl"] as string);
                    EntitiesGenerator.Definitions.DataTable dataTable = new EntitiesGenerator.Definitions.DataTable();
                    dataTable.SourceName = listUrl;
                    dataTable.TableName = dataRow["Title"] as string;
                    System.Data.DataTable tableColumns = sharePointConnection.GetTable(listUrl);
                    foreach (System.Data.DataRow tableColumn in tableColumns.Rows)
                    {
                        EntitiesGenerator.Definitions.DataColumn dataColumn = new EntitiesGenerator.Definitions.DataColumn();
                        string columnName = tableColumn["InternalName"] as string;
                        Type dataType = tableColumn["FieldType"] as Type;
                        dataColumn.ColumnName = columnName;
                        dataColumn.SourceName = columnName;
                        dataColumn.DataType = TypeManager.GetWellKnownDataTypeName(dataType);
                        dataColumn.Type = dataColumn.DataType;
                        dataColumn.AllowDBNull = !((bool)tableColumn["Required"]);
                        dataColumn.PrimaryKey = (dataColumn.ColumnName == "ID");
                        //dataColumn.Length = GetDataColumnLength(column);
                        dataTable.Columns.Add(dataColumn);

                    }
                    definition.Tables.Add(dataTable);
                }
            }
            return definition;
        }

        private void toSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO:
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Server Compact 4.0 Local Database (*.sdf)|*.sdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    string templateDatabaseFileName  =  Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"), "sqlce.sdf");
                    File.Copy(templateDatabaseFileName, fileName, true);

                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);                    
                    string connectionString = string.Format("Data Source={0}", fileInfo.FullName);
                    var connection = new SqlCeConnection(connectionString);
                    connection.Open();
                    int count = 0;
                    SqlGenerator sqlGenerator = new SqlGenerator();
                    foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
                    {
                        string sqlText = sqlGenerator.GenerateForMSSQL(table);

                        var command = connection.CreateCommand();
                        command.CommandText = sqlText;
                        count += command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void fromAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Assembly Files (*.dll)|*.dll|(*.exe)|*.exe|(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Definitions.Definition definition = GenerateDefinitionFromAssembly(dialog.FileName);
                AppendDefinition(definition);
            }
        }

        private Definition GenerateDefinitionFromAssembly(string fileName)
        {
            Definition difinition = new Definition();
            Assembly assembly = Assembly.LoadFile(fileName);
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                Definitions.DataTable table = new Definitions.DataTable();
                table.TableName = type.Name;
                table.SourceName = type.Name;
                if ((type.GetCustomAttributes(typeof(TableAttribute), true) != null) && (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0))
                {
                    table.SourceName = (type.GetCustomAttributes(typeof(TableAttribute), true)[0] as TableAttribute).Name;
                }
                foreach (var property in type.GetProperties())
                {
                    Type propertyType = property.PropertyType;
                    if (propertyType.IsGenericType)
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }
                    string typeName = TypeManager.GetWellKnownDataTypeName(propertyType);
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        Definitions.DataColumn column = new Definitions.DataColumn
                        {
                            ColumnName = property.Name,
                            SourceName = property.Name,
                            Type = typeName,
                            DataType = typeName
                        };
                        if ((property.GetCustomAttributes(typeof(ColumnAttribute), false) != null) && (property.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0))
                        {
                            ColumnAttribute columnAttribute = property.GetCustomAttributes(typeof(ColumnAttribute), false)[0] as ColumnAttribute;
                            column.SourceName = columnAttribute.Name;
                            column.PrimaryKey = columnAttribute.IsPrimaryKey;
                            column.AllowDBNull = columnAttribute.CanBeNull;
                            if (!string.IsNullOrWhiteSpace(columnAttribute.DbType))
                            {
                                column.DataType = columnAttribute.DbType;
                            }
                        }
                        table.Columns.Add(column);
                    }
                }
                difinition.Tables.Add(table);
            }
            return difinition;
        }
        
        private void GenerateProjectWithRazorEngine()
        {
            RazorCodeGenerator codeGenerator = new RazorCodeGenerator();
            codeGenerator.ApplicationStartupPath = Application.StartupPath;
            codeGenerator.CodeNamespace = this.definition.Namespace;
            string projectDirectory = Path.Combine(this.definition.ProjectLocation, this.definition.Namespace.Trim());
            List<string> tables = new List<string>();
            List<string> newTypeList = new List<string>();
            foreach (EntitiesGenerator.Definitions.DataTable table in this.definition.Tables)
            {
                string designerCode;
                string tableCode = codeGenerator.GenerateDataObject(table, out designerCode, newTypeList);

                string dataObjectName = table.TableName.Trim();

                if (!System.IO.Directory.Exists(projectDirectory))
                {
                    System.IO.Directory.CreateDirectory(projectDirectory);
                }
                System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".cs"), tableCode, System.Text.Encoding.UTF8);
                System.IO.File.WriteAllText(Path.Combine(projectDirectory, dataObjectName + ".Designer.cs"), designerCode, System.Text.Encoding.UTF8);

                tables.Add(dataObjectName);
            }
            foreach (string newType in newTypeList)
            {
                string newTypeCode = codeGenerator.GenerateEnumCode(newType);
                System.IO.File.WriteAllText(Path.Combine(projectDirectory, newType + ".cs"), newTypeCode, System.Text.Encoding.UTF8);
            }
            string projectFileName;
            bool result = codeGenerator.GenerateCSProject(projectDirectory, tables.ToArray(), newTypeList.ToArray(), (ProjectVesion)(Enum.Parse(typeof(ProjectVesion), this.toolStripComboBox1.Text)), out projectFileName);
            if (result && !string.IsNullOrEmpty(projectFileName))
            {
                string fullPathProjectFileName = Path.Combine(projectDirectory, projectFileName);
                // TODO: message.
                string message = "Do you want to open the created project?";
                if (MessageBox.Show(message, "Open project", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(fullPathProjectFileName);
                }
            }
        }
    }
}
