using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace GeneralLib.Utility
{
    public class JsonUtility
    {
        /// <summary>
        /// convert type T to JSON.
        /// </summary>
        /// <typeparam name="T">concrate type</typeparam>
        /// <param name="t">instance of the type.</param>
        /// <returns>JSON string.</returns>
        public static string ConvertToJSON<T>(T t)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(t);
        }

        public static T ConvertToType<T>(string json)
        {
            JavaScriptSerializer sel = new JavaScriptSerializer();
            return sel.Deserialize<T>(json);
        }
    }
}
