#define SERIALIZATION_DATACONTRACT

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace $NAMESPACE$
{
    /// <summary>
    /// $OBJECTNAME$.
    /// </summary>
    [Serializable]
#if SERIALIZATION_DATACONTRACT
    [DataContract]
#endif
    [RaisingStudio.Data.DefinitionName("$OBJECTNAME$")]
    partial class $OBJECTNAME$
    {
        #region Members
		$$$DATAOBJECTCOLUMN$$$
		#endregion
    }
}
