using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EntitiesGenerator
{
    public partial class DatabaseConnectionDialog : Form
    {
        public DatabaseConnectionDialog()
        {
            InitializeComponent();

            this.defaultConnectionStrings = new List<string>();
            this.defaultConnectionStrings.Add(SQLSERVER_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(SQLSERVER2005_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(ORACEL_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(ORACEL_DATAACCESS_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(MYSQL_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(SHAREPOINT_DEFAULT_CONNECTIONSTRING);
            this.defaultConnectionStrings.Add(SQLCE_DEFAULT_CONNECTIONSTRING);
        }

        public string DatabaseType
        {
            get
            {
                return this.comboBox_DatabaseType.Text;
            }
            set
            {
                this.comboBox_DatabaseType.Text = value;
            }
        }

        public bool DatabaseTypeComboBoxEnabled
        {
            get
            {
                return this.comboBox_DatabaseType.Enabled;
            }
            set
            {
                this.comboBox_DatabaseType.Enabled = false;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.textBox_ConnectionString.Text;
            }
        }

        public const string SQLSERVER_DEFAULT_CONNECTIONSTRING = "Data Source=localhost;Initial Catalog=master;Integrated Security=True";
        public const string SQLSERVER2005_DEFAULT_CONNECTIONSTRING = "Data Source=localhost;Initial Catalog=master;User ID=sa";
        public const string ORACEL_DEFAULT_CONNECTIONSTRING = "Data source={0}; User ID={1}; Password={2}";
        public const string ORACEL_DATAACCESS_DEFAULT_CONNECTIONSTRING = "Data Source= (DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = 1522))  (CONNECT_DATA = (SERVER = DEDICATED)  (SERVICE_NAME = XE) ) );User Id={1};Password={2};";
        public const string MYSQL_DEFAULT_CONNECTIONSTRING = "Server={0};Uid={1};Pwd={2};Database={3};";
        public const string SQLITE_DEFAULT_CONNECTIONSTRING = "Data source={0};Pooling=true;FailIfMissing=false";
        public const string SHAREPOINT_DEFAULT_CONNECTIONSTRING = "http://localhost/";
        public const string SQLCE_DEFAULT_CONNECTIONSTRING = "Data Source={0}";

        List<string> defaultConnectionStrings;
        private void DatabaseConnectionDialog_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comboBox_DatabaseType.Text))
            {
                this.comboBox_DatabaseType.SelectedIndex = 0;
            }
        }


        private void comboBox_DatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox_ConnectionString.Text) || this.defaultConnectionStrings.Contains(this.textBox_ConnectionString.Text))
            {
                this.textBox_ConnectionString.Text = this.defaultConnectionStrings[this.comboBox_DatabaseType.SelectedIndex];
            }
        }
    }
}
