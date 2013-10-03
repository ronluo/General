using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EntitiesGenerator.Generators
{
    public class RazorCodeGenerator
    {
        public static readonly string TEMPLATES_PATHNAME = Path.Combine("Templates", "Razor");
        
        public static readonly string DATAOBJECT_FILENAME = "DATAOBJECT.cs";
        public static readonly string DATAOBJECT_DESIGNER_FILENAME = "DATAOBJECT.Designer.cs";

        public static readonly string NEWTYPE_FILENAME = "NEWTYPE.cs";
        
        public static readonly string PROJECT_FILENAME = "NAMESPACE.csproj";
        public static readonly string PROJECT_08_FILENAME = "NAMESPACE_08.csproj";
        public static readonly string PROJECT_10_FILENAME = "NAMESPACE_10.csproj";

        public string ApplicationStartupPath { get; set; }

        public string CodeNamespace { get; set; }

        public RazorCodeGenerator()
        {
            bool loaded = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly != null;
            loaded = typeof(GeneralDAC.TypeManager).Assembly != null; 
            Console.WriteLine(loaded);            
        }

        public string ReadTemplateFile(string fileName)
        {
            return System.IO.File.ReadAllText(Path.Combine(Path.Combine(ApplicationStartupPath, TEMPLATES_PATHNAME), fileName));
        }

        private string tableTemplate;
        private string designerTemplate;
        private string newtypeTemplate;
        //private string projectTemplate;

        public string GenerateDataObject(Definitions.DataTable table, out string designerCode, List<string> newTypeList)
        {
            var model = new
            {
                ApplicationStartupPath = ApplicationStartupPath,
                CodeNamespace = CodeNamespace,
                Table = table,
                NewTypeList = newTypeList
            };
            if (string.IsNullOrEmpty(this.tableTemplate))
            {
                string tableTemplate = ReadTemplateFile(DATAOBJECT_FILENAME);                
                RazorEngine.Razor.Compile(tableTemplate, "tableTemplate");
                this.tableTemplate = tableTemplate;
            }
            string tableCode = RazorEngine.Razor.Run(model, "tableTemplate");   
            //string tableCode = RazorEngine.Razor.Parse(tableTemplate, model);
            if (string.IsNullOrEmpty(this.designerTemplate))
            {
                string designerTemplate = ReadTemplateFile(DATAOBJECT_DESIGNER_FILENAME);
                RazorEngine.Razor.Compile(designerTemplate, "designerTemplate");
                this.designerTemplate = designerTemplate;
            }
            //designerCode = RazorEngine.Razor.Parse(designerTemplate, model);
            designerCode = RazorEngine.Razor.Run(model, "designerTemplate");
            return tableCode;
        }

        public string GenerateEnumCode(string newType)
        {
            var model = new
            {
                ApplicationStartupPath = ApplicationStartupPath,
                CodeNamespace = CodeNamespace,
                Type = newType
            };
            if (string.IsNullOrEmpty(this.newtypeTemplate))
            {
                string template = ReadTemplateFile(NEWTYPE_FILENAME);
                RazorEngine.Razor.Compile(template, "newTypeTemplate");
                this.newtypeTemplate = template;
            }
            //string code = RazorEngine.Razor.Parse(template, model);
            string code = RazorEngine.Razor.Run(model, "newTypeTemplate");
            return code;
        }

        public bool GenerateCSProject(string projectDirectory, string[] tables, string[] newTypes, ProjectVesion projectVesion, out string projectFileName)
        {
            projectFileName = string.Format("{0}.csproj", CodeNamespace);
            var model = new
            {
                ApplicationStartupPath = ApplicationStartupPath,
                ProjectGuid = "{" + Guid.NewGuid() + "}",
                CodeNamespace = CodeNamespace,
                Tables = tables,
                NewTypes = newTypes
            };
            string template = ReadTemplateFile(projectVesion == ProjectVesion.VS2005 ? PROJECT_FILENAME : (projectVesion == ProjectVesion.VS2008 ? PROJECT_08_FILENAME : PROJECT_10_FILENAME));
            string projectContent = RazorEngine.Razor.Parse(template, model);
            if (!System.IO.Directory.Exists(projectDirectory))
            {
                System.IO.Directory.CreateDirectory(projectDirectory);
            }
            System.IO.File.WriteAllText(Path.Combine(projectDirectory, projectFileName), projectContent, System.Text.Encoding.UTF8);
            return true;
        }
    }
}
