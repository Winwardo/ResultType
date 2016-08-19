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

        [TestMethod]
        public void AnError_IsOk_ReturnsFalse()
        {
            var result = SimpleResult.Error("error");
            Assert.IsFalse(result.isOk());
        }

        [TestMethod]
        public void AnOk_IsError_ReturnsFalse()
        {
            var result = SimpleResult.Ok(5);
            Assert.IsFalse(result.isError());
        }

        [TestMethod]
        public void AnError_IsError_ReturnsTrue()
        {
            var result = SimpleResult.Error("error");
            Assert.IsTrue(result.isError());
        }
    }
}
