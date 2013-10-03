using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EntitiesGenerator.Generators
{
    public class XmlGenerator
    {
        public System.Xml.XmlElement GenerateDefinitionXmlElement(EntitiesGenerator.Definitions.DataTable table)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            System.Xml.XmlElement definitionElement = xmlDocument.CreateElement("Definition");
            System.Xml.XmlElement xmlElement = xmlDocument.CreateElement("DataTable");

            System.Xml.XmlAttribute tableNameXmlAttribute = xmlDocument.CreateAttribute("TableName");
            tableNameXmlAttribute.Value = table.TableName.Trim();
            xmlElement.Attributes.Append(tableNameXmlAttribute);

            System.Xml.XmlAttribute sourceNameXmlAttribute = xmlDocument.CreateAttribute("SourceName");
            sourceNameXmlAttribute.Value = table.SourceName.Trim();
            xmlElement.Attributes.Append(sourceNameXmlAttribute);

            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                System.Xml.XmlElement dataColumnXmlElement = xmlDocument.CreateElement("DataColumn");

                System.Xml.XmlAttribute dataColumnColumnNameXmlAttribute = xmlDocument.CreateAttribute("ColumnName");
                dataColumnColumnNameXmlAttribute.Value = column.ColumnName.Trim();
                dataColumnXmlElement.Attributes.Append(dataColumnColumnNameXmlAttribute);

                System.Xml.XmlAttribute dataColumnSourceNameXmlAttribute = xmlDocument.CreateAttribute("SourceName");
                dataColumnSourceNameXmlAttribute.Value = column.SourceName.Trim();
                dataColumnXmlElement.Attributes.Append(dataColumnSourceNameXmlAttribute);

                System.Xml.XmlAttribute dataColumnDataTypeXmlAttribute = xmlDocument.CreateAttribute("DataType");
                dataColumnDataTypeXmlAttribute.Value = column.DataType.Trim();
                dataColumnXmlElement.Attributes.Append(dataColumnDataTypeXmlAttribute);

                if (column.PrimaryKey)
                {
                    System.Xml.XmlAttribute dataColumnPrimaryKeyXmlAttribute = xmlDocument.CreateAttribute("PrimaryKey");
                    dataColumnPrimaryKeyXmlAttribute.Value = column.PrimaryKey ? "true" : "false";
                    dataColumnXmlElement.Attributes.Append(dataColumnPrimaryKeyXmlAttribute);
                }

                if (!column.AllowDBNull)
                {
                    System.Xml.XmlAttribute dataColumnAllowDBNullXmlAttribute = xmlDocument.CreateAttribute("AllowDBNull");
                    dataColumnAllowDBNullXmlAttribute.Value = column.AllowDBNull ? "true" : "false";
                    dataColumnXmlElement.Attributes.Append(dataColumnAllowDBNullXmlAttribute);
                }

                if (column.AutoIncrement)
                {
                    System.Xml.XmlAttribute dataColumnAutoIncrementXmlAttribute = xmlDocument.CreateAttribute("AutoIncrement");
                    dataColumnAutoIncrementXmlAttribute.Value = column.AutoIncrement ? "true" : "false";
                    dataColumnXmlElement.Attributes.Append(dataColumnAutoIncrementXmlAttribute);
                }

                // TODO: caption.

                xmlElement.AppendChild(dataColumnXmlElement);
            }
            definitionElement.AppendChild(xmlElement);
            xmlDocument.AppendChild(definitionElement);
            return xmlDocument.DocumentElement;
        }

        public bool GenerateDefinitionXmlFile(EntitiesGenerator.Definitions.DataTable table, string pathName)
        {
            System.Xml.XmlElement definitionXmlElement = GenerateDefinitionXmlElement(table);
            System.Xml.XmlDocument xmlDocument = definitionXmlElement.OwnerDocument;
            System.Xml.XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", null, "yes");
            xmlDocument.InsertBefore(xmlDeclaration, definitionXmlElement);
            string fileName = string.Format("{0}.definition.xml", table.TableName.Trim());
            fileName = Path.Combine(pathName, fileName);
            if (!System.IO.Directory.Exists(pathName))
            {
                System.IO.Directory.CreateDirectory(pathName);
            }
            xmlDocument.Save(fileName);
            return true;
        }
    }
}
