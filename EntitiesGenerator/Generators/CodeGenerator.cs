using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeneralDAC;

namespace EntitiesGenerator.Generators
{
    public class CodeGenerator
    {
        #region common data definition 
        public static readonly string TEMPLATES_PATHNAME = "Templates";
        public static readonly string PROPERTIES_PATHNAME = "Properties";

        public static readonly string DATAOBJECT_FILENAME = "DATAOBJECT.cs";
        public static readonly string NEWTYPE_FILENAME = "NEWTYPE.cs";
        public static readonly string DATAOBJECT_DESIGNER_FILENAME = "DATAOBJECT.Designer.cs";
        public static readonly string DATAOBJECT_ATTRIBUTE_DESIGNER_FILENAME = "DATAOBJECT_ATTRIBUTE.Designer.cs";
        public static readonly string DATAOBJECT_DEFINITION_FILENAME = "DATAOBJECT.Expression.cs";
        public static readonly string DATAOBJECT_PROXY_FILENAME = "DATAOBJECT.Proxy.cs";
        public static readonly string DATAOBJECTCOLUMN_FILENAME = "DATAOBJECTCOLUMN.cs";
        public static readonly string DATAOBJECTCOLUMN_ATTRIBUTE_FILENAME = "DATAOBJECTCOLUMN_ATTRIBUTE.cs";

        public static readonly string PROJECT_FILENAME = "NAMESPACE.csproj";
        public static readonly string PROJECT_08_FILENAME = "NAMESPACE_08.csproj";
        public static readonly string PROJECT_10_FILENAME = "NAMESPACE_10.csproj";

        public static readonly string ASSEMBLYINFO_FILENAME = "AssemblyInfo.cs";

        public static readonly string NAMESPACE_MARK = "$NAMESPACE$";
        public static readonly string OBJECTNAME_MARK = "$OBJECTNAME$";
        public static readonly string NEWTYPENAME_MARK = "$NEWTYPENAME$";

        public static readonly string DATAOBJECTCOLUMN_MARK = "$$$DATAOBJECTCOLUMN$$$";

        public static readonly string COLUMNDESCRIPTION_MARK = "$COLUMNDESCRIPTION$";
        public static readonly string COLUMNNAME_MARK = "$COLUMNNAME$";
        public static readonly string COLUMNTYPE_MARK = "$COLUMNTYPE$";

        public static readonly string DATATABLE_ATTRIBUTE_MARK = "$$DATATABLE_ATTRIBUTE$$";
        public static readonly string DATACOLUMN_ATTRIBUTE_MARK = "$$DATACOLUMN_ATTRIBUTE$$";
      
        // for proxy.
        public static readonly string TYPENAME_MARK = "$TYPENAME$";

        public static readonly string GET_CASE_MARK = "$$GET_CASE$$";
        public static readonly string SET_CASE_MARK = "$$SET_CASE$$";

        public static readonly string PROPERTYNAME_MARK = "$PROPERTYNAME$";
        public static readonly string PROPERTYTYPE_MARK = "$PROPERTYTYPE$";

        public static readonly string GET_PROPERTYTYPE_CASE_MARK = "$$GET_PROPERTYTYPE_CASE$$";
    
        public static readonly string GETCASE_TEMPLATE_CODE = @"
                case " + "\"$PROPERTYNAME$\":" + @"
                   {
                       return _instance.$PROPERTYNAME$;
                   }";

        public static readonly string SETCASE_TEMPLATE_CODE = @"
                case " + "\"$PROPERTYNAME$\":" + @"
                    {
                        _instance.$PROPERTYNAME$ = ($PROPERTYTYPE$)value;
                        return;
                    }";

        public static readonly string GET_PROPERTYTYPE_CASE_TEMPLATE_CODE = @"
                case " + "\"$PROPERTYNAME$\":" + @"
                   {
                       return typeof($PROPERTYTYPE$);
                   }";
        #endregion

        private string codeNamespace;
        public string CodeNamespace
        {
            get
            {
                return this.codeNamespace;
            }
            set
            {
                this.codeNamespace = value;
            }
        }

        private string applicationStartupPath;
        public string ApplicationStartupPath
        {
            get { return applicationStartupPath; }
            set { applicationStartupPath = value; }
        }

        private List<string> wellKnownDataTypes;
        public CodeGenerator()
        {
            this.wellKnownDataTypes = new List<string>(new string[]{
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
        }

        public string ReadTemplateFile(string fileName)
        {            
			return System.IO.File.ReadAllText(Path.Combine(Path.Combine(applicationStartupPath, TEMPLATES_PATHNAME), fileName));
        }

        public string GenerateDataObject(EntitiesGenerator.Definitions.DataTable table, bool xmlMapping, out string designerCode, out string definitionCode, out string proxyCode, List<string> newTypeList)
        {
            #region Designer code
            // read the content of the template file.
            string dataObjectText = xmlMapping ? ReadTemplateFile(DATAOBJECT_DESIGNER_FILENAME) : ReadTemplateFile(DATAOBJECT_ATTRIBUTE_DESIGNER_FILENAME);
            string dataObjectColumnText = xmlMapping ? ReadTemplateFile(DATAOBJECTCOLUMN_FILENAME) : ReadTemplateFile(DATAOBJECTCOLUMN_ATTRIBUTE_FILENAME);

            string dataObjectCode = dataObjectText;
            System.Text.StringBuilder dataObjectColumnSB = new System.Text.StringBuilder();
            System.Text.StringBuilder dataObjectColumnDefinitionSB = new System.Text.StringBuilder();
            System.Text.StringBuilder columnsDeclareSB = new System.Text.StringBuilder();
            System.Text.StringBuilder objectColumnsDeclareSB = new System.Text.StringBuilder();
            string dataObjectName = table.TableName.Trim();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                // get column name.
                string columnName = column.ColumnName.Trim();
                // get type of column.
                string dataType = "string";
                if (!string.IsNullOrEmpty(column.Type))
                {
                    dataType = GetDataType(column.Type.Trim(), column.AllowDBNull, newTypeList);
                }

                string dataObjectColumnCode = dataObjectColumnText;
                dataObjectColumnCode = dataObjectColumnCode.Replace(COLUMNNAME_MARK, columnName);
                string caption = columnName;
                dataObjectColumnCode = dataObjectColumnCode.Replace(COLUMNDESCRIPTION_MARK, caption);
                dataObjectColumnCode = dataObjectColumnCode.Replace(COLUMNTYPE_MARK, dataType);
                
                string dataColumnAttribute = GetDataColumnAttribute(column);
                dataObjectColumnCode = dataObjectColumnCode.Replace(DATACOLUMN_ATTRIBUTE_MARK, dataColumnAttribute);

                dataObjectColumnSB.Append(dataObjectColumnCode);

                // Definition code.
                dataObjectColumnDefinitionSB.AppendFormat("        public static readonly RaisingStudio.Data.Expressions.ColumnExpression _{0} = \"{0}\";", columnName);
                dataObjectColumnDefinitionSB.AppendLine();
                //dataObjectColumnDefinitionSB.AppendFormat("        public readonly RaisingStudio.Data.Expressions.ColumnExpression {0} = _{0};", columnName);
                dataObjectColumnDefinitionSB.AppendFormat("        public RaisingStudio.Data.Expressions.ColumnExpression {0} {{ get {{ return _{0}; }} }}", columnName);
                dataObjectColumnDefinitionSB.AppendLine();

                columnsDeclareSB.AppendFormat("                            _{0}._{1},", dataObjectName, columnName);
                columnsDeclareSB.AppendLine();

                objectColumnsDeclareSB.AppendFormat("                            {0}.{1},", dataObjectName, columnName);
                objectColumnsDeclareSB.AppendLine();
            }
            string dataObjectColumn = dataObjectColumnSB.ToString();
            // replace the entity.
            dataObjectCode = dataObjectCode.Replace(DATAOBJECTCOLUMN_MARK, dataObjectColumn);

            // replace the namespace.
            dataObjectCode = dataObjectCode.Replace(NAMESPACE_MARK, codeNamespace);
            // replace the entity name.
            dataObjectCode = dataObjectCode.Replace(OBJECTNAME_MARK, dataObjectName);

            string dataTableAttribute = GetDataTableAttribute(table);
            dataObjectCode = dataObjectCode.Replace(DATATABLE_ATTRIBUTE_MARK, dataTableAttribute);

            designerCode = dataObjectCode;
            #endregion
            #region Definition code
            // Definition code.
            string definitionCodeText = ReadTemplateFile(DATAOBJECT_DEFINITION_FILENAME);
            definitionCode = definitionCodeText;
            // TODO: generate definition code.
            definitionCode = definitionCode.Replace("$$DATAOBJECTCOLUMNDEFINITION$$", dataObjectColumnDefinitionSB.ToString());
            definitionCode = definitionCode.Replace("$$COLUMNSDECLARE$$", columnsDeclareSB.ToString());
            definitionCode = definitionCode.Replace("$$OBJECTCOLUMNSDECLARE$$", objectColumnsDeclareSB.ToString());
            // replace the namespace.
            definitionCode = definitionCode.Replace(NAMESPACE_MARK, codeNamespace);
            // replace the entity name.
            definitionCode = definitionCode.Replace(OBJECTNAME_MARK, dataObjectName);
            #endregion
            #region Proxy code
            // Proxy code.
            string proxyCodeText = ReadTemplateFile(DATAOBJECT_PROXY_FILENAME);
            proxyCode = proxyCodeText;
            string typeName = dataObjectName;
            System.Text.StringBuilder getValueCodeStringBuilder = new StringBuilder();
            System.Text.StringBuilder setValueCodeStringBuilder = new StringBuilder();
            System.Text.StringBuilder getPropertyTypeCodeStringBuilder = new StringBuilder();
            foreach (EntitiesGenerator.Definitions.DataColumn column in table.Columns)
            {
                // get column name.
                string columnName = column.ColumnName.Trim();
                // get type of column.
                string dataType = "string";
                if (!string.IsNullOrEmpty(column.Type))
                {
                    dataType = GetDataType(column.Type.Trim(), column.AllowDBNull, newTypeList);
                }

                string propertyName = columnName;
                string propertyTypeCode = dataType;

                string getValueCode = GETCASE_TEMPLATE_CODE;
                getValueCode = getValueCode.Replace(PROPERTYNAME_MARK, propertyName);
                getValueCodeStringBuilder.Append(getValueCode);

                string setValueCode = SETCASE_TEMPLATE_CODE;
                setValueCode = setValueCode.Replace(PROPERTYNAME_MARK, propertyName);
                setValueCode = setValueCode.Replace(PROPERTYTYPE_MARK, propertyTypeCode);
                setValueCodeStringBuilder.Append(setValueCode);

                string getPropertyTypeCode = GET_PROPERTYTYPE_CASE_TEMPLATE_CODE;
                getPropertyTypeCode = getPropertyTypeCode.Replace(PROPERTYNAME_MARK, propertyName);
                getPropertyTypeCode = getPropertyTypeCode.Replace(PROPERTYTYPE_MARK, propertyTypeCode);
                getPropertyTypeCodeStringBuilder.Append(getPropertyTypeCode);
            }
            string getValueFinallyCode = getValueCodeStringBuilder.ToString();
            string setValueFinallyCode = setValueCodeStringBuilder.ToString();
            string getPropertyTypeFinallyCode = getPropertyTypeCodeStringBuilder.ToString();
            proxyCode = proxyCode.Replace(TYPENAME_MARK, typeName);
            proxyCode = proxyCode.Replace(GET_CASE_MARK, getValueFinallyCode);
            proxyCode = proxyCode.Replace(SET_CASE_MARK, setValueFinallyCode);
            proxyCode = proxyCode.Replace(GET_PROPERTYTYPE_CASE_MARK, getPropertyTypeFinallyCode);
            // replace the namespace.
            proxyCode = proxyCode.Replace(NAMESPACE_MARK, codeNamespace);
            // replace the entity name.
            proxyCode = proxyCode.Replace(OBJECTNAME_MARK, dataObjectName);
            #endregion
            #region Main code
            string mainCodeText = ReadTemplateFile(DATAOBJECT_FILENAME);
            string mainCode = mainCodeText;
            // replace the namespace.
            mainCode = mainCode.Replace(NAMESPACE_MARK, codeNamespace);
            // replace the entity name.
            mainCode = mainCode.Replace(OBJECTNAME_MARK, dataObjectName);
            #endregion
            return mainCode;
        }

        private string GetDataTableAttribute(EntitiesGenerator.Definitions.DataTable table)
        {
            return string.Format("[RaisingStudio.Data.DataTable(\"{0}\")]", table.SourceName);
        }

        private string GetDataColumnAttribute(EntitiesGenerator.Definitions.DataColumn column)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("[RaisingStudio.Data.DataColumn(\"{0}\"", column.SourceName);

            string typeNameOfType = GetDbType(column.Type);
            string typeNameOfDataType = GetDbType(column.DataType);
            if (typeNameOfType != typeNameOfDataType)
            {
                stringBuilder.AppendFormat(", DbType = {0}", typeNameOfDataType);
            }
            if (!column.AllowDBNull)
            {
                stringBuilder.Append(", AllowDBNull = false");
            }
            if (column.PrimaryKey)
            {
                stringBuilder.Append(", IsPrimaryKey = true");
            }
            if (column.AutoIncrement)
            {
                stringBuilder.Append(", AutoIncrement = true");
            }
            stringBuilder.Append(")]");
            return stringBuilder.ToString();
        }

        private string GetDbType(string dataType)
        {
            System.Data.DbType dbType = System.Data.DbType.String;
            if (TypeManager.WellKnownDbTypes.ContainsKey(dataType.Trim()))
            {
                dbType = TypeManager.GetWellKnownDbType(dataType);
            }
            else
            {
                switch (dataType.ToLower().Trim())
                {
                    case "char":
                    case "varchar":
                    case "nvarchar":
                        {
                            dbType = System.Data.DbType.String;
                            break;
                        }
                }
            }
            return string.Format("System.Data.DbType.{0}", dbType);
        }

        private string GetDataType(string type, bool isNullable, List<string> newTypeList)
        {
            if (isNullable)
            {
                if (this.wellKnownDataTypes.Contains(type))
                {
                    if (type == "string" || type == "object" || type == "byte[]")
                    {
                        return type;
                    }
                    else
                    {
                        return string.Format("{0}?", type);
                    }
                }
                else
                {
                    if (!type.Contains("."))
                    {
                        if (!newTypeList.Contains(type))
                        {
                            newTypeList.Add(type);
                        }
                        return string.Format("{0}?", type);
                    }
                    return type;
                }
            }
            else
            {
                if (this.wellKnownDataTypes.Contains(type))
                {
                    return type;
                }
                else
                {
                    if (!type.Contains("."))
                    {
                        if (!newTypeList.Contains(type))
                        {
                            newTypeList.Add(type);
                        }
                    }
                    return type;
                }
            }
        }


        public bool GenerateCSProject(string projectDirectory, string[] tables, string[] newTypes, ProjectVesion projectVersion, bool xmlMapping, out string projectFileName)
        {
            string projectGuidText = System.Guid.NewGuid().ToString();
            string compileText = GenerateCompileText(tables, newTypes);
            string embeddedResourceText = xmlMapping ? GenerateEmbeddedResourceText(tables) : string.Empty;

            string projectTemplateFileName = (projectVersion == ProjectVesion.VS2008) ? PROJECT_08_FILENAME : ((projectVersion == ProjectVesion.VS2010) ?  PROJECT_10_FILENAME : PROJECT_FILENAME);
            string projectText = ReadTemplateFile(projectTemplateFileName);
            projectText = projectText.Replace(NAMESPACE_MARK, this.codeNamespace);
            projectText = projectText.Replace("$PROJECTGUID$", projectGuidText);
            projectText = projectText.Replace("$COMPILE$", compileText);
            projectText = projectText.Replace("$EMBEDDEDRESOURCE$", embeddedResourceText);

            if (!System.IO.Directory.Exists(projectDirectory))
            {
                System.IO.Directory.CreateDirectory(projectDirectory);
            }
            projectFileName = string.Format("{0}.csproj", this.codeNamespace);
            System.IO.File.WriteAllText(Path.Combine(projectDirectory, projectFileName), projectText, System.Text.Encoding.UTF8);

            string assemblyInfoText = this.ReadTemplateFile(Path.Combine(PROPERTIES_PATHNAME, ASSEMBLYINFO_FILENAME));
            assemblyInfoText = assemblyInfoText.Replace(NAMESPACE_MARK, this.codeNamespace);
            assemblyInfoText = assemblyInfoText.Replace("$NEWGUIDFORCOM$", System.Guid.NewGuid().ToString());

            string propertyPropertiesDirectory = Path.Combine(projectDirectory, PROPERTIES_PATHNAME);
            if (!System.IO.Directory.Exists(propertyPropertiesDirectory))
            {
                System.IO.Directory.CreateDirectory(propertyPropertiesDirectory);
            }
            System.IO.File.WriteAllText(Path.Combine(propertyPropertiesDirectory, ASSEMBLYINFO_FILENAME), assemblyInfoText, System.Text.Encoding.UTF8);
            return true;
        }

        private string GenerateCompileText(string[] dataObjectNames, string[] newTypes)
        {
            System.Text.StringBuilder compileTextStringBuilder = new StringBuilder();
            foreach (string dataObjectName in dataObjectNames)
            {
                compileTextStringBuilder.AppendFormat("    <Compile Include=\"{0}.cs\" />\n", dataObjectName);

                compileTextStringBuilder.AppendFormat("    <Compile Include=\"{0}.Designer.cs\">\n", dataObjectName);
                compileTextStringBuilder.AppendFormat("      <DependentUpon>{0}.cs</DependentUpon>\n", dataObjectName);
                compileTextStringBuilder.Append("    </Compile>\n");

                compileTextStringBuilder.AppendFormat("    <Compile Include=\"{0}.Expression.cs\">\n", dataObjectName);
                compileTextStringBuilder.AppendFormat("      <DependentUpon>{0}.cs</DependentUpon>\n", dataObjectName);
                compileTextStringBuilder.Append("    </Compile>\n");

                compileTextStringBuilder.AppendFormat("    <Compile Include=\"{0}.Proxy.cs\">\n", dataObjectName);
                compileTextStringBuilder.AppendFormat("      <DependentUpon>{0}.cs</DependentUpon>\n", dataObjectName);
                compileTextStringBuilder.Append("    </Compile>\n");
            }
            foreach (string newType in newTypes)
            {
                compileTextStringBuilder.AppendFormat("    <Compile Include=\"{0}.cs\" />\n", newType);
            }
            return compileTextStringBuilder.ToString();
        }

        private string GenerateEmbeddedResourceText(string[] dataObjectNames)
        {
            System.Text.StringBuilder embeddedResourceStringBuilder = new StringBuilder();
            foreach (string dataObjectName in dataObjectNames)
            {
                embeddedResourceStringBuilder.AppendFormat("    <EmbeddedResource Include=\"{0}.definition.xml\">\n", dataObjectName);
                embeddedResourceStringBuilder.AppendFormat("      <DependentUpon>{0}.cs</DependentUpon>\n", dataObjectName);
                embeddedResourceStringBuilder.AppendFormat("    </EmbeddedResource>\n", dataObjectName);
            }
            return embeddedResourceStringBuilder.ToString();
        }

        public string GenerateEnumCode(string newType)
        {
            string newTypeText = ReadTemplateFile(NEWTYPE_FILENAME);
            string newTypeCode = newTypeText;
            // replace the namespace.
            newTypeCode = newTypeCode.Replace(NAMESPACE_MARK, codeNamespace);
            // replace the type name.
            newTypeCode = newTypeCode.Replace(NEWTYPENAME_MARK, newType);
            return newTypeCode;
        }
    }

    public enum ProjectVesion
    {
        VS2005,
        VS2008,
        VS2010
    }
}
