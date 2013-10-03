using System;
using System.Collections.Generic;
using System.Text;
using GeneralDAC;

namespace EntitiesGenerator.Generators
{
    public class SqlGenerator
    {
        private string GetColumnsText(List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns)
        {
            StringBuilder columnsStringBuilder = new StringBuilder();
            for (int i = 0; i < primaryKeyColumns.Count; i++)
            {
                if (i < primaryKeyColumns.Count - 1)
                {
                    columnsStringBuilder.AppendFormat("{0}, ", primaryKeyColumns[i].SourceName.Trim());
                }
                else
                {
                    columnsStringBuilder.AppendFormat("{0}", primaryKeyColumns[i].SourceName.Trim());
                }
            }
            return columnsStringBuilder.ToString();
        }


        public string GenerateForMSSQL(EntitiesGenerator.Definitions.DataTable table)
        {
            StringBuilder sqlStringBuilder = new StringBuilder();
            string tableName = table.SourceName.Trim();
            sqlStringBuilder.AppendFormat("CREATE TABLE {0}", tableName);
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append("(");
            sqlStringBuilder.AppendLine();
            List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                string columnName = column.SourceName.Trim();
                string dataType = column.DataType.Trim();
                if (column.AllowDBNull)
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}    {2}   NULL,", columnName, GetSqlTypeForMSSQL(dataType, column.Length), column.AutoIncrement ? "IDENTITY(1,1)" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                else
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}    {2}   NOT NULL,", columnName, GetSqlTypeForMSSQL(dataType, column.Length), column.AutoIncrement ? "IDENTITY(1,1)" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                if (column.PrimaryKey)
                {
                    primaryKeyColumns.Add(column);
                }
            }
            if (primaryKeyColumns.Count > 0)
            {
                string primaryKeyColumnsText = GetColumnsText(primaryKeyColumns);
                sqlStringBuilder.AppendFormat("    CONSTRAINT PK_{0} PRIMARY KEY ({1})", tableName, primaryKeyColumnsText);
                sqlStringBuilder.AppendLine();
            }
            else
            {
                sqlStringBuilder.Remove(sqlStringBuilder.ToString().LastIndexOf(","), 1);
            }
            sqlStringBuilder.Append(")");
            sqlStringBuilder.AppendLine();
            return sqlStringBuilder.ToString();
        }

        private string GetSqlTypeForMSSQL(string dataType, string length)
        {
            System.Data.DbType dbType = TypeManager.GetWellKnownDbType(dataType);
            switch (dbType)
            {
                case System.Data.DbType.Int16:
                    {
                        return "smallint";
                    }
                case System.Data.DbType.Int32:
                    {
                        return "int";
                    }
                case System.Data.DbType.Int64:
                    {
                        return "bigint";
                    }
                case System.Data.DbType.Byte:
                    {
                        return "tinyint";
                    }
                case System.Data.DbType.Binary:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "binary(50)";
                        }
                        else
                        {
                            return string.Format("binary({0})", length);
                        }
                    }
                case System.Data.DbType.Boolean:
                    {
                        return "bit";
                    }
                case System.Data.DbType.Single:
                    {
                        return "float";
                    }
                case System.Data.DbType.Double:
                    {
                        return "real";
                    }
                case System.Data.DbType.Decimal:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "decimal";
                        }
                        else
                        {
                            return string.Format("decimal({0})", length);
                        }
                    }
                case System.Data.DbType.DateTime:
                    {
                        return "datetime";
                    }
                case System.Data.DbType.Guid:
                    {
                        return "uniqueidentifier";
                    }
                case System.Data.DbType.String:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "nvarchar(50)";
                        }
                        else
                        {
                            return string.Format("nvarchar({0})", length);
                        }
                    }
                case System.Data.DbType.Object:
                    {
                        return "sql_variant";
                    }
            }
            return dataType;
        }


        public string GenerateForMYSQL(EntitiesGenerator.Definitions.DataTable table)
        {
            StringBuilder sqlStringBuilder = new StringBuilder();
            string tableName = table.SourceName.Trim();
            sqlStringBuilder.AppendFormat("CREATE TABLE {0}", tableName);
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append("(");
            sqlStringBuilder.AppendLine();
            List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                string columnName = column.SourceName.Trim();
                string dataType = column.DataType.Trim();
                if (column.AllowDBNull)
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NULL    {2},", columnName, GetSqlTypeForMYSQL(dataType, column.Length), column.AutoIncrement ? "AUTO_INCREMENT" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                else
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NOT NULL    {2},", columnName, GetSqlTypeForMYSQL(dataType, column.Length), column.AutoIncrement ? "AUTO_INCREMENT" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                if (column.PrimaryKey)
                {
                    primaryKeyColumns.Add(column);
                }
            }
            if (primaryKeyColumns.Count > 0)
            {
                string primaryKeyColumnsText = GetColumnsText(primaryKeyColumns);
                sqlStringBuilder.AppendFormat("    CONSTRAINT PK_{0} PRIMARY KEY ({1})", tableName, primaryKeyColumnsText);
                sqlStringBuilder.AppendLine();
            }
            else
            {
                sqlStringBuilder.Remove(sqlStringBuilder.ToString().LastIndexOf(","), 1);
            }
            sqlStringBuilder.Append(")");
            sqlStringBuilder.AppendLine();
            return sqlStringBuilder.ToString();
        }

        private string GetSqlTypeForMYSQL(string dataType, string length)
        {
            System.Data.DbType dbType = TypeManager.GetWellKnownDbType(dataType);
            switch (dbType)
            {
                case System.Data.DbType.Int16:
                    {
                        return "smallint";
                    }
                case System.Data.DbType.Int32:
                    {
                        return "int";
                    }
                case System.Data.DbType.Int64:
                    {
                        return "bigint";
                    }
                case System.Data.DbType.Byte:
                    {
                        return "tinyint";
                    }
                case System.Data.DbType.Binary:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "binary(50)";
                        }
                        else
                        {
                            return string.Format("binary({0})", length);
                        }
                    }
                case System.Data.DbType.Boolean:
                    {
                        return "bit";
                    }
                case System.Data.DbType.Single:
                    {
                        return "float";
                    }
                case System.Data.DbType.Double:
                    {
                        return "real";
                    }
                case System.Data.DbType.Decimal:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "decimal";
                        }
                        else
                        {
                            return string.Format("decimal({0})", length);
                        }
                    }
                case System.Data.DbType.DateTime:
                    {
                        return "datetime";
                    }
                case System.Data.DbType.Guid:
                    {
                        return "uniqueidentifier";
                    }
                case System.Data.DbType.String:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "nvarchar(50)";
                        }
                        else
                        {
                            return string.Format("nvarchar({0})", length);
                        }
                    }
                case System.Data.DbType.Object:
                    {
                        return "sql_variant";
                    }
            }
            return dataType;
        }


        public string GenerateForOracle(EntitiesGenerator.Definitions.DataTable table)
        {
            StringBuilder sqlStringBuilder = new StringBuilder();
            string tableName = table.SourceName.Trim();
            sqlStringBuilder.AppendFormat("CREATE TABLE {0}", tableName);
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append("(");
            sqlStringBuilder.AppendLine();
            List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            List<EntitiesGenerator.Definitions.DataColumn> autoIncrementColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                string columnName = column.SourceName.Trim();
                string dataType = column.DataType;
                if (column.AllowDBNull)
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NULL,", columnName, GetSqlTypeForOracle(dataType, column.Length));
                    sqlStringBuilder.AppendLine();
                }
                else
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NOT NULL,", columnName, GetSqlTypeForOracle(dataType, column.Length));
                    sqlStringBuilder.AppendLine();
                }
                if (column.PrimaryKey)
                {
                    primaryKeyColumns.Add(column);
                }
                if(column.AutoIncrement)
                {
                    autoIncrementColumns.Add(column);
                }
            }
            if (primaryKeyColumns.Count > 0)
            {
                string primaryKeyColumnsText = GetColumnsText(primaryKeyColumns);
                sqlStringBuilder.AppendFormat("    CONSTRAINT PK_{0} PRIMARY KEY ({1})", tableName, primaryKeyColumnsText);
                sqlStringBuilder.AppendLine();
            }
            else
            {
                sqlStringBuilder.Remove(sqlStringBuilder.ToString().LastIndexOf(","), 1);
            }
            sqlStringBuilder.Append(")");
            sqlStringBuilder.AppendLine();

            if (autoIncrementColumns.Count > 0)
            {
                sqlStringBuilder.AppendLine("/");
                sqlStringBuilder.AppendLine();
                foreach (EntitiesGenerator.Definitions.DataColumn column in autoIncrementColumns)
                {
                    GenerateAutoIncrementForOracle(sqlStringBuilder, tableName, column);
                }
            }

            return sqlStringBuilder.ToString();
        }
                
        private void GenerateAutoIncrementForOracle(StringBuilder sqlStringBuilder, string tableName, EntitiesGenerator.Definitions.DataColumn column)
        {
            string template = @"CREATE SEQUENCE $TABLENAME$_$COLUMNNAME$_SEQ
START WITH 1
INCREMENT BY 1
/

CREATE OR REPLACE TRIGGER $TABLENAME$_$COLUMNNAME$_TRIGGER
  BEFORE INSERT
  ON $TABLENAME$
  REFERENCING OLD AS OLD NEW AS NEW
  FOR EACH ROW   
BEGIN   
  SELECT $TABLENAME$_$COLUMNNAME$_SEQ.NEXTVAL INTO :NEW.$COLUMNNAME$ FROM DUAL;
END;";
            string sql = template.Replace("$TABLENAME$", tableName);
            sql = sql.Replace("$COLUMNNAME$", column.SourceName.Trim());
            sqlStringBuilder.AppendLine(sql);
        }

        private string GetSqlTypeForOracle(string dataType, string length)
        {
            System.Data.DbType dbType = TypeManager.GetWellKnownDbType(dataType);
            switch (dbType)
            {
                case System.Data.DbType.Int16:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(6)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Int32:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(10)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Int64:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(20)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Byte:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(4)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Binary:
                    {
                        return "BLOB";
                    }
                case System.Data.DbType.Boolean:
                    {
                        return "NUMBER(1)";
                    }
                case System.Data.DbType.Single:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(30)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Double:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(30)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.Decimal:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "NUMBER(30)";
                        }
                        else
                        {
                            return string.Format("NUMBER({0})", length);
                        }
                    }
                case System.Data.DbType.DateTime:
                    {
                        return "DATE";
                    }
                case System.Data.DbType.Guid:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "VARCHAR2(36)";
                        }
                        else
                        {
                            return string.Format("VARCHAR2({0})", length);
                        }
                    }
                case System.Data.DbType.String:
                    {
                        if(string.IsNullOrEmpty(length))
                        {
                            return "VARCHAR2(50)";
                        }
                        else
                        {
                            return string.Format("VARCHAR2({0})", length);
                        }
                    }
                case System.Data.DbType.Object:
                    {
                        return "BLOB";
                    }
            }
            return dataType;
        }


        public string GenerateForPostgreSQL(EntitiesGenerator.Definitions.DataTable table)
        {
            StringBuilder sqlStringBuilder = new StringBuilder();
            string tableName = table.SourceName.Trim();
            sqlStringBuilder.AppendFormat("CREATE TABLE {0}", tableName);
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append("(");
            sqlStringBuilder.AppendLine();
            List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                string columnName = column.SourceName.Trim();
                string dataType = column.DataType.Trim();
                if (column.AllowDBNull)
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NULL    {2},", columnName, GetSqlTypeForPostgreSQL(dataType, column.Length), column.AutoIncrement ? "SERIAL" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                else
                {
                    sqlStringBuilder.AppendFormat("  {0}    {1}   NOT NULL    {2},", columnName, GetSqlTypeForPostgreSQL(dataType, column.Length), column.AutoIncrement ? "SERIAL" : string.Empty);
                    sqlStringBuilder.AppendLine();
                }
                if (column.PrimaryKey)
                {
                    primaryKeyColumns.Add(column);
                }
            }
            if (primaryKeyColumns.Count > 0)
            {
                string primaryKeyColumnsText = GetColumnsText(primaryKeyColumns);
                sqlStringBuilder.AppendFormat("    CONSTRAINT PK_{0} PRIMARY KEY ({1})", tableName, primaryKeyColumnsText);
                sqlStringBuilder.AppendLine();
            }
            else
            {
                sqlStringBuilder.Remove(sqlStringBuilder.ToString().LastIndexOf(","), 1);
            }
            sqlStringBuilder.Append(")");
            sqlStringBuilder.AppendLine();
            return sqlStringBuilder.ToString();
        }

        private string GetSqlTypeForPostgreSQL(string dataType, string length)
        {
            System.Data.DbType dbType = TypeManager.GetWellKnownDbType(dataType);
            switch (dbType)
            {
                case System.Data.DbType.Int16:
                    {
                        return "INTEGER";
                    }
                case System.Data.DbType.Int32:
                    {
                        return "INTEGER";
                    }
                case System.Data.DbType.Int64:
                    {
                        return "INTEGER";
                    }
                case System.Data.DbType.Byte:
                    {
                        return "INTEGER";
                    }
                case System.Data.DbType.Binary:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "binary(50)";
                        }
                        else
                        {
                            return string.Format("binary({0})", length);
                        }
                    }
                case System.Data.DbType.Boolean:
                    {
                        return "INTEGER";
                    }
                case System.Data.DbType.Single:
                    {
                        return "NUMERIC";
                    }
                case System.Data.DbType.Double:
                    {
                        return "NUMERIC";
                    }
                case System.Data.DbType.Decimal:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "NUMERIC";
                        }
                        else
                        {
                            return string.Format("NUMERIC({0})", length);
                        }
                    }
                case System.Data.DbType.DateTime:
                    {
                        return "DATE";
                    }
                case System.Data.DbType.Guid:
                    {
                        return "uniqueidentifier";
                    }
                case System.Data.DbType.String:
                    {
                        if (string.IsNullOrEmpty(length))
                        {
                            return "VARCHAR(50)";
                        }
                        else
                        {
                            return string.Format("VARCHAR({0})", length);
                        }
                    }
                case System.Data.DbType.Object:
                    {
                        return "sql_variant";
                    }
            }
            return dataType;
        }


        public string GenerateForSQLite(EntitiesGenerator.Definitions.DataTable table)
        {
            StringBuilder sqlStringBuilder = new StringBuilder();
            string tableName = table.SourceName.Trim();
            sqlStringBuilder.AppendFormat("CREATE TABLE [{0}]", tableName);
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append("(");
            sqlStringBuilder.AppendLine();
            List<EntitiesGenerator.Definitions.DataColumn> primaryKeyColumns = new List<EntitiesGenerator.Definitions.DataColumn>();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                string columnName = column.SourceName.Trim();
                string dataType = column.DataType.Trim();
                sqlStringBuilder.AppendFormat("  [{0}] {1} {2} {3} {4},", columnName, GetSqlTypeForSQLite(dataType, column.Length), column.AllowDBNull ? "NULL" : "NOT NULL", column.PrimaryKey ? "PRIMARY KEY" : string.Empty, column.AutoIncrement ? "AUTOINCREMENT" : string.Empty);
                sqlStringBuilder.AppendLine();
            }

            sqlStringBuilder.Remove(sqlStringBuilder.Length - 4, 4);            
            sqlStringBuilder.AppendLine();
            sqlStringBuilder.Append(")");
            return sqlStringBuilder.ToString();
        }

        private string GetSqlTypeForSQLite(string dataType, string length)
        {
            if (TypeManager.WellKnownDbTypes.ContainsKey(dataType))
            {
                System.Data.DbType dbType = TypeManager.GetWellKnownDbType(dataType);
                switch (dbType)
                {
                    case System.Data.DbType.Int16:
                        {
                            return "INTEGER";
                        }
                    case System.Data.DbType.Int32:
                        {
                            return "INTEGER";
                        }
                    case System.Data.DbType.Int64:
                        {
                            return "INTEGER";
                        }
                    case System.Data.DbType.Byte:
                        {
                            return "INTEGER";
                        }
                    case System.Data.DbType.Binary:
                        {
                            return "BLOB";
                        }
                    case System.Data.DbType.Boolean:
                        {
                            return "BOOLEAN";
                        }
                    case System.Data.DbType.Single:
                        {
                            return "FLOAT";
                        }
                    case System.Data.DbType.Double:
                        {
                            return "REAL";
                        }
                    case System.Data.DbType.Decimal:
                        {
                            return "NUMERIC";
                        }
                    case System.Data.DbType.DateTime:
                        {
                            return "DATE";
                        }
                    case System.Data.DbType.Guid:
                        {
                            return "NVARCHAR(255)";
                        }
                    case System.Data.DbType.String:
                        {
                            if (string.IsNullOrEmpty(length))
                            {
                                return "NVARCHAR(50)";
                            }
                            else
                            {
                                return string.Format("NVARCHAR({0})", length);
                            }
                        }
                    case System.Data.DbType.Object:
                        {
                            return "BLOB";
                        }
                }
            }
            return dataType;
        }
    }
}
