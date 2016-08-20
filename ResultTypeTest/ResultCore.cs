using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultType;

namespace ResultTypeTest
{
    using SimpleResult = Result<int, string>;

    [TestClass]
    public class ResultCore
    {
        private static int SIMPLE_OKAY_VALUE = 5;
        private static int SIMPLE_OKAY_OTHER_VALUE = SIMPLE_OKAY_VALUE + 1;
        private static string SIMPLE_ERROR_MESSAGE = "some error";

        private SimpleResult makeSimpleOk()
        {
            return SimpleResult.Ok(SIMPLE_OKAY_VALUE);
        }

        private SimpleResult makeAnotherSimpleOk()
        {
            return SimpleResult.Ok(SIMPLE_OKAY_VALUE + 1);
        }

        private SimpleResult makeSimpleError()
        {
            return SimpleResult.Error(SIMPLE_ERROR_MESSAGE);
        }

        [TestMethod]
        public void AnOk_IsOk_ReturnsTrue()
        {
            var result = makeSimpleOk();
            Assert.IsTrue(result.isOk());
        }

        [TestMethod]
        public void AnError_IsOk_ReturnsFalse()
        {
            var result = makeSimpleError();
            Assert.IsFalse(result.isOk());
        }

        [TestMethod]
        public void AnOk_IsError_ReturnsFalse()
        {
            var result = makeSimpleOk();
            Assert.IsFalse(result.isError());
        }

        [TestMethod]
        public void AnError_IsError_ReturnsTrue()
        {
            var result = makeSimpleError();
            Assert.IsTrue(result.isError());
        }

        [TestMethod]
        public void AnOk_Unwrap_UnwrapsFine()
        {
            var result = makeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE, result.unwrap());
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorException))]
        public void AnError_Unwrap_Throws()
        {
            var result = makeSimpleError();
            result.unwrap();
        }

        [TestMethod]
        public void AnOk_UnwrapOr_ReturnsTheOkayValue()
        {
            var result = makeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE, result.unwrapOr(SIMPLE_OKAY_OTHER_VALUE));
        }

        [TestMethod]
        public void AnError_UnwrapOr_ReturnsTheOtherValue()
        {
            var result = makeSimpleError();
            Assert.AreEqual(SIMPLE_OKAY_OTHER_VALUE, result.unwrapOr(SIMPLE_OKAY_OTHER_VALUE));
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorOfOkException))]
        public void AnOk_UnwrapError_Throws()
        {
            var result = makeSimpleOk();
            result.unwrapError();
        }

        [TestMethod]
        public void AnError_UnwrapError_ReturnsTheError()
        {
            var result = makeSimpleError();
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE, result.unwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.andThen((value) => makeAnotherSimpleOk());
            Assert.AreEqual(SIMPLE_OKAY_OTHER_VALUE, andThennedResult.unwrap());
        }
    }
}
