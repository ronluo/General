using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EntitiesGenerator.Definitions
{
    [Serializable]
    public class Definition
    {
        private List<DataTable> _tables;
        [Browsable(false)]
        public List<DataTable> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        private string _namespace;

        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        private string _projectLocation;

        public string ProjectLocation
        {
            get { return _projectLocation; }
            set { _projectLocation = value; }
        }

        public Definition()
        {
            this._tables = new List<DataTable>();
        }
    }
}
