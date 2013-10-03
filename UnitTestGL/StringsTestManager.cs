using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralLib.Utility;
using System.IO;

namespace UnitTestGL
{
    internal class StringsTestManager
    {
        public static string[] GetIsLowerCaseTestData()
        {
            List<String> strlist = new List<string>();

            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            path = path.Substring(0, path.LastIndexOf('\\'));
            path = path.Substring(0, path.LastIndexOf('\\'));
            path = path + @"\TestData\Strings\IsLowerCaseTestData.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    strlist.Add(line);
                }
            }
            return strlist.ToArray();
        }
    }
}
