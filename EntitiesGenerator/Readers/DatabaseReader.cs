using System;
using System.Collections.Generic;
using System.Text;
using GeneralDAC;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SmartCodeGen.Providers;

namespace EntitiesGenerator.Readers
{
    public class DatabaseReader
    {
        public const string SQLSERVER_PROVIDER_NAME = "MSSqlDBSchemaProvider";
        public const string SQLSERVER2005_PROVIDER_NAME = "MSSql2005DBSchemaProvider";
        public const string ORACLE_PROVIDER_NAME = "OracleNativeDBSchemaProvider";
        public const string ORACLE_DATAACCESS_PROVIDER_NAME = "OracleDataAccessDBSchemaProvider";
        public const string MYSQL_PROVIDER_NAME = "MySqlDBSchemaProvider";
        public const string SQLCE_PROVIDER_NAME = "SqlCeDBSchemaProvider";

        private bool autoApplyNamingConvention = false;
        public bool AutoApplyNamingConvention
        {
            get { return autoApplyNamingConvention; }
            set { autoApplyNamingConvention = value; }
        }

        public EntitiesGenerator.Definitions.Definition ReadDatabase(string databaseType, string connectionString)
        {
            try
            {
                string dbSchemaProviderName = SQLSERVER2005_PROVIDER_NAME;
                switch (databaseType)
                {
                    case "SQL Server":
                        {
                            dbSchemaProviderName = SQLSERVER_PROVIDER_NAME;
                            break;
                        }
                    case "SQL Server 2005":
                        {
                            dbSchemaProviderName = SQLSERVER2005_PROVIDER_NAME;
                            break;
                        }
                    case "Oracle":
                        {
                            dbSchemaProviderName = ORACLE_PROVIDER_NAME;
                            break;
                        }
                    case "Oracle DataAccess":
                        {
                            dbSchemaProviderName = ORACLE_DATAACCESS_PROVIDER_NAME;
                            break;
                        }
                    case "MySQL":
                        {
                            dbSchemaProviderName = MYSQL_PROVIDER_NAME;
                            break;
                        }
                    case "SqlCe":
                        {
                            dbSchemaProviderName = SQLCE_PROVIDER_NAME;
                            break;
                        }
                }
                DBSchemaProvider dbSchemaProvider = GetDBSchemaProvider(dbSchemaProviderName);
                DatabaseSchema schema = new DatabaseSchema(dbSchemaProvider, connectionString);
                if (dbSchemaProvider != null)
                {
                    EntitiesGenerator.Definitions.Definition definition = new EntitiesGenerator.Definitions.Definition();
                    SelectTableForm selectTableForm = new SelectTableForm();
                    string databaseName = dbSchemaProvider.GetDatabaseName(schema);
                    TableSchemaCollection allTables = dbSchemaProvider.GetTables(schema, databaseName);
                    if (allTables != null)
                    {
                        foreach (TableSchema table in allTables)
                        {
                            EntitiesGenerator.Definitions.DataTable dataTable = new EntitiesGenerator.Definitions.DataTable();
                            if (AutoApplyNamingConvention)
                            {
                                dataTable.TableName = GetConventionName(table.Name);
                            }
                            else
                            {
                                dataTable.TableName = table.Name;
                            }
                            dataTable.SourceName = table.Name;
                            if (table.Columns != null)
                            {
                                foreach (ColumnSchema column in table.Columns)
                                {
                                    EntitiesGenerator.Definitions.DataColumn dataColumn = new EntitiesGenerator.Definitions.DataColumn();
                                    if (AutoApplyNamingConvention)
                                    {
                                        dataColumn.ColumnName = GetConventionName(column.Name);
                                    }
                                    else
                                    {
                                        dataColumn.ColumnName = column.Name;
                                    }
                                    dataColumn.SourceName = column.Name;
                                    dataColumn.DataType = GetDataColumnDataType(column);
                                    dataColumn.Type = GetDataColumnType(dataColumn, column);
                                    dataColumn.AllowDBNull = column.AllowDBNull;
                                    dataColumn.PrimaryKey = column.IsPrimaryKeyMember;
                                    dataColumn.Length = GetDataColumnLength(column);

                                    dataTable.Columns.Add(dataColumn);
                                }
                            }
                            AddListViewItem(selectTableForm, dataTable, 0);
                            //definition.Tables.Add(dataTable);
                        }
                    }
                    try
                    {
                        ViewSchemaCollection allViews = dbSchemaProvider.GetViews(schema);
                        if (allViews != null)
                        {
                            foreach (ViewSchema view in allViews)
                            {
                                EntitiesGenerator.Definitions.DataTable dataTable = new EntitiesGenerator.Definitions.DataTable();
                                dataTable.TableName = view.Name;
                                dataTable.SourceName = view.Name;
                                if (view.Columns != null)
                                {
                                    foreach (ViewColumnSchema column in view.Columns)
                                    {
                                        EntitiesGenerator.Definitions.DataColumn dataColumn = new EntitiesGenerator.Definitions.DataColumn();
                                        dataColumn.ColumnName = column.Name;
                                        dataColumn.SourceName = column.Name;
                                        dataColumn.DataType = GetDataColumnDataType(column);
                                        dataColumn.Type = GetDataColumnType(dataColumn, column);
                                        dataColumn.AllowDBNull = column.AllowDBNull;

                                        dataColumn.Length = GetDataColumnLength(column);

                                        dataTable.Columns.Add(dataColumn);
                                    }
                                }
                                AddListViewItem(selectTableForm, dataTable, 1);
                                //definition.Tables.Add(dataTable);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    if (selectTableForm.ShowDialog() == DialogResult.OK)
                    {
                        System.Windows.Forms.ListView.CheckedListViewItemCollection checkedItems = selectTableForm.GetCheckedItems();
                        foreach (ListViewItem item in checkedItems)
                        {
                            definition.Tables.Add(item.Tag as EntitiesGenerator.Definitions.DataTable);
                        }

                        string message = "Do you want to apply Naming Convention to Table Name(s) and Column Name(s)?";
                        if (MessageBox.Show(message, "Naming Convention", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            foreach (EntitiesGenerator.Definitions.DataTable dataTable in definition.Tables)
                            {
                                dataTable.TableName = GetConventionName(dataTable.TableName);
                                foreach (EntitiesGenerator.Definitions.DataColumn dataColumn in dataTable.Columns)
                                {
                                    dataColumn.ColumnName = GetConventionName(dataColumn.ColumnName);
                                }
                            }
                        }
                    }

                    return definition;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                // TODO:
                return null;
            }
            return null;
        }

        public static bool IsUpperString(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if(char.IsLetter(c) && (!char.IsUpper(c)))
                {
                    return false;
                }
            }
            return true;
        }
        
        public static string GetConventionName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if ((name.Length > 2) && IsUpperString(name))
                {
                    return GetConventionName(name, true, false);
                }
                else
                {
                    return GetConventionName(name, false, true);
                }
            }
            return name;
        }

        public static string GetConventionName(string name, bool toLower, bool keepUnderscore)
        {
            if (!string.IsNullOrEmpty(name))
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (toLower)
                {
                    name = name.ToLower();
                }
                bool capitalize = true;
                for (int i = 0; i < name.Length; i++)
                {
                    char c = name[i];
                    if (char.IsLetter(c))
                    {
                        if (capitalize)
                        {
                            stringBuilder.Append(char.ToUpper(c));
                            capitalize = false;
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }
                    }
                    else if (char.IsDigit(c) || (c == '_' && keepUnderscore))
                    {
                        stringBuilder.Append(c);
                        capitalize = true;
                    }
                    else
                    {
                        capitalize = true;
                    }
                }
                return stringBuilder.ToString();
            }
            return name;
        }

        private static void AddListViewItem(SelectTableForm selectTableForm, EntitiesGenerator.Definitions.DataTable dataTable, int imageIndex)
        {
            ListViewItem item = new ListViewItem();
            item.Text = dataTable.SourceName;
            item.Tag = dataTable;
            item.ImageIndex = imageIndex;
            item.Checked = true;
            selectTableForm.AddListViewItem(item);
        }

        private string GetDataColumnLength(ViewColumnSchema column)
        {
            if (column.Size > 0)
            {
                return column.Size.ToString();
            }
            return null;
        }

        private string GetDataColumnType(EntitiesGenerator.Definitions.DataColumn dataColumn, ViewColumnSchema column)
        {
            return TypeManager.GetWellKnownDbTypeName(column.DataType);
        }

        private string GetDataColumnDataType(ViewColumnSchema column)
        {
            string dbTypeName = TypeManager.GetWellKnownDbTypeName(column.DataType);
            if (!TypeManager.WellKnownDataTypes.ContainsKey(dbTypeName))
            {
                return column.NativeType;
            }
            return dbTypeName;
        }

        private string GetDataColumnType(EntitiesGenerator.Definitions.DataColumn dataColumn, ColumnSchema column)
        {
            string dataType = TypeManager.GetWellKnownDbTypeName(column.DataType);
            switch (dataType.ToLower())
            {
                case "ansistring":
                case "ansistringfixedlength":
                case "stringfixedlength":
                    {
                        return "string";
                    }
                case "currency":
                    {
                        return "decimal";
                    }
            }
            return dataType;
        }

        private string GetDataColumnLength(ColumnSchema column)
        {
            if (column.Size > 0)
            {
                return column.Size.ToString();
            }
            return null;
        }

        private string GetDataColumnDataType(ColumnSchema column)
        {
            string dbTypeName = TypeManager.GetWellKnownDbTypeName(column.DataType);
            if (!TypeManager.WellKnownDataTypes.ContainsKey(dbTypeName))
            {
                return column.NativeType;
            }
            return dbTypeName;
        }

        private DBSchemaProvider GetDBSchemaProvider(string dbSchemaProviderName)
        {
            switch (dbSchemaProviderName)
            {
                case SQLSERVER_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.MSSqlDBSchemaProvider();
                        break;
                    }
                case SQLSERVER2005_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.MSSql2005DBSchemaProvider();
                        break;
                    }
                case ORACLE_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.OracleNativeDBSchemaProvider();
                        break;
                    }
                case ORACLE_DATAACCESS_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.OracleDataAccessDBSchemaProvider();
                        break;
                    }
                case MYSQL_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.MySqlDBSchemaProvider();
                        break;
                    }
                case SQLCE_PROVIDER_NAME:
                    {
                        return new SmartCodeGen.ImplementedProviders.SqlCeDBSchemaProvider();
                        break;
                    }
            }
            return null;
        }
    }
}
