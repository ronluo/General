using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWebUI.Models
{
    public class ModuleInfo
    {
        private int moduleId;
        private string moduleName;

        public int ModuleID
        {
            get { return moduleId; }
            set { moduleId = value; }
        }

        public string ModuleName
        {
            get { return moduleName; }
            set { moduleName = value; }
        }
    }
}