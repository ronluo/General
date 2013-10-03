using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EntitiesGenerator.Definitions
{
    [Serializable]
    [DefaultProperty("SourceName")]
    public class DataColumn
    {
        public event EventHandler PropertyChanged;

        public virtual void OnPropertyChanged(EventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                if (value != _columnName)
                {
                    _columnName = value;
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private string _type = "string";

        public string Type
        {
            get { return _type; }
            set
            {
                if (value != _type)
                {
                    _type = value;
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

        private string _dataType = "string";

        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (value != _dataType)
                {
                    _dataType = value;
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private string _length;

        public string Length
        {
            get { return _length; }
            set
            {
                if (value != _length)
                {
                    _length = value;
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private bool _primaryKey = false;

        public bool PrimaryKey
        {
            get { return _primaryKey; }
            set
            {
                if (value != _primaryKey)
                {
                    _primaryKey = value;
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private bool _allowDBNull = true;

        public bool AllowDBNull
        {
            get { return _allowDBNull; }
            set
            {
                if (value != _allowDBNull)
                {
                    _allowDBNull = value; 
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private bool _autoIncrement = false;

        public bool AutoIncrement
        {
            get { return _autoIncrement; }
            set
            {
                if (value != _autoIncrement)
                {
                    _autoIncrement = value; 
                    OnPropertyChanged(new EventArgs());
                }
            }
        }
        
    

        public DataColumn()
        {

        }
        
        public DataColumn(string columnName, string sourceName)
            : this()
        {
            this._columnName = columnName;
            this._sourceName = sourceName;
        }

        public DataColumn(string columnName)
            : this(columnName, columnName)
        {
        }
    }
}
