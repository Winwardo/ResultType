using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultType;

namespace ResultTypeTest
{
    [TestClass]
    public class ResultCore
    {
        [TestMethod]
        public void TestMethod1()
        {
            var r = new ResultType.Result<int, int>();
            Assert.IsTrue(r.test());
        }

    }
}
