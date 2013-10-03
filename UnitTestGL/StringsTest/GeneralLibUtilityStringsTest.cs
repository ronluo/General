using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneralLib.Utility;

namespace UnitTestGL.StringsTest
{
    [TestClass]
    public class GeneralLibUtilityStringsTest
    {
        [TestCategory("StringsUnitTest"), TestMethod]
        [Priority(1)]
        [Owner("ron_luo")]
        [WorkItem(1)]
        [Description("Test IsLowerCase method")]
        public void IsLowerCaseTest()
        {
            var strings = StringsTestManager.GetIsLowerCaseTestData();
            foreach (var str in strings)
            {
                Assert.IsTrue(Strings.IsLowerCase(str));
            }
        }
    }
}
