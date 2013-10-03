using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace @Model.CodeNamespace
{
    /// <summary>
    /// @Model.Table.TableName
    /// </summary>
    [Serializable]
    [DataContract]
    [Table(Name = "@Model.Table.SourceName")]
    partial class @Model.Table.TableName
    {
        #region Members
        @foreach(var column in @Model.Table.Columns)
        {
            var propertyName = column.ColumnName;
            var propertyType = column.Type;
            if(RaisingStudio.Data.TypeManager.GetWellKnownDataType(propertyType) != null)
            {
                propertyType = RaisingStudio.Data.TypeManager.GetWellKnownDataTypeName(RaisingStudio.Data.TypeManager.GetWellKnownDataType(propertyType));
            }
            else
            {
                Model.NewTypeList.Add(propertyType);
            }
            if((propertyType != "string") && (propertyType != "byte[]") && (propertyType != "object") && (column.AllowDBNull))
            {
                propertyType += "?";
            }
            var fieldName = "_" + propertyName;
            var columnAttribute = string.Format("[Column(Name = \"{0}\"", column.SourceName);
            if(!column.AllowDBNull)
            {
                columnAttribute += ", CanBeNull = false";
            }
            if(column.AutoIncrement)
            {
                columnAttribute += ", IsDbGenerated=true";
            }
            if(column.PrimaryKey)
            {
                columnAttribute += ", IsPrimaryKey = true";
            }
            columnAttribute += ")]";

@:
@:        private @propertyType @fieldName;
@:        /// <summary>
@:        /// @propertyName
@:        /// </summary>
@:        [Description("@propertyName")]
@:        [DataMember]
            if(column.PrimaryKey)
{
@:        [System.ComponentModel.DataAnnotations.Key]
}
@:        @columnAttribute
@:        public @propertyType @propertyName
@:        {
@:          get 
@:          {
@:              return this.@fieldName;
@:          }
@:          set 
@:          {
@:              this.@fieldName = value;
@:          }
@:        }
@:
        }
        #endregion
    }
}
