using System;
using System.Collections.Generic;
using System.Text;
using EntitiesGenerator.Definitions;

namespace EntitiesGenerator.Readers
{
    public class SqlFileReader
    {
        public const string CREATETABLE_MARK = "CREATE TABLE ";
        public const string PRIMARYKEY_MARK = "CONSTRAINT {0} PRIMARY KEY ({1})";

        public EntitiesGenerator.Definitions.Definition ReadFile(System.IO.FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                Definition definition = new Definition();
                System.IO.StreamReader  streamReader  = fileInfo.OpenText();
                string textLine = streamReader.ReadLine();
                while (textLine != null)
                {
                    string text = textLine.TrimStart();
                    if (text.StartsWith("CREATE TABLE ", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string tableText;
                        string tableName = ReadTableName(text, streamReader, out tableText);
                        EntitiesGenerator.Definitions.DataTable table = new EntitiesGenerator.Definitions.DataTable(tableName);
                        ProcessColumns(table, tableText);
                    }
                    textLine = streamReader.ReadLine();
                }
                return definition;
            }
            else
            {
                return null;
            }
        }

        private string ReadTableName(string text, System.IO.StreamReader streamReader, out string tableText)
        {
            tableText = null;
            string tableName = null;
            while (!text.Contains("("))
            {
                text = text + streamReader.ReadLine();
            }
            // TODO: use regular expression to match out the table name and output the columns text.

            return tableName;
        }

        private void ProcessColumns(DataTable table, string tableText)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
