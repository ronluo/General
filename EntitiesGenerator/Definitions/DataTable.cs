using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EntitiesGenerator.Definitions
{
    [Serializable]
    [DefaultProperty("SourceName")]
    public class DataTable
    {
        public event EventHandler PropertyChanged;

        public virtual void OnPropertyChanged(EventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        
        private string _tableName;

        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (value != _tableName)
                {
                    _tableName = value; 
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private string _sourceName;

        public string SourceName
        {
            get { return _sourceName; }
            set
            {
                if (value != _sourceName)
                {
                    _sourceName = value;
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private List<DataColumn> _columns;
        [Browsable(false)]
        public List<DataColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public DataTable()
        {
            this._columns = new List<DataColumn>();
        }

        public DataTable(string tableName, string sourceName)
            : this()
        {
            this._tableName = tableName;
            this._sourceName = sourceName;
        }

        public DataTable(string tableName)
            : this(tableName, tableName)
        {
        }
    }
}
