using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultType;

namespace ResultTypeTest
{
    using SimpleResult = Result<int, string>;

    [TestClass]
    public class ResultCore
    {
        [TestMethod]
        public void AnOk_IsOk_ReturnsTrue()
        {
            var result = SimpleResult.Ok(5);
            Assert.IsTrue(result.isOk());
        }
    }
}
