using System;
using System.Collections.Generic;
using System.Text;

namespace $NAMESPACE$
{
    partial class $OBJECTNAME$ : RaisingStudio.Data.Common.IDataObjectProxy
    {
        #region IDataObjectProxy Members

        public Type GetPropertyType(string name)
        {
            switch (name)
            {                $$GET_PROPERTYTYPE_CASE$$
                default:
                    {
                        throw new System.ArgumentOutOfRangeException(name);
                    }
            }
        }

        #endregion

        #region IObjectAccessor Members

        public object GetValue(object instance, string name)
        {
            $TYPENAME$ _instance = instance as $TYPENAME$;
            switch (name)
            {                $$GET_CASE$$
                default:
                    {
                        throw new System.ArgumentOutOfRangeException(name);
                    }
            }
        }

        public void SetValue(object instance, string name, object value)
        {
            $TYPENAME$ _instance = instance as $TYPENAME$;
            switch (name)
            {                $$SET_CASE$$
                default:
                    {
                        throw new System.ArgumentOutOfRangeException(name);
                    }
            }
        }

        #endregion

        #region IObjectCreater Members

        public object CreateObject()
        {
            return new $TYPENAME$();
        }

        public System.Collections.IList CreateObjectList()
        {
            return new System.Collections.Generic.List<$TYPENAME$>();
        }

        #endregion
    }
}
