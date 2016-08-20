using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultType;

namespace ResultTypeTest
{
    using SimpleResult = Result<int, string>;

    [TestClass]
    public class ResultCore
    {
        private static int SIMPLE_OKAY_VALUE_1 = 5;
        private static int SIMPLE_OKAY_VALUE_2 = SIMPLE_OKAY_VALUE_1 + 1;
        private static string SIMPLE_ERROR_MESSAGE_1 = "some error";
        private static string SIMPLE_ERROR_MESSAGE_2 = "some other error";

        private SimpleResult makeSimpleOk()
        {
            return SimpleResult.Ok(SIMPLE_OKAY_VALUE_1);
        }

        private SimpleResult makeSimpleOk2()
        {
            return SimpleResult.Ok(SIMPLE_OKAY_VALUE_1 + 1);
        }

        private SimpleResult makeSimpleError()
        {
            return SimpleResult.Error(SIMPLE_ERROR_MESSAGE_1);
        }

        private SimpleResult makeSimpleError2()
        {
            return SimpleResult.Error(SIMPLE_ERROR_MESSAGE_2);
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
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.unwrap());
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
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.unwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [TestMethod]
        public void AnError_UnwrapOr_ReturnsTheOtherValue()
        {
            var result = makeSimpleError();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, result.unwrapOr(SIMPLE_OKAY_VALUE_2));
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
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, result.unwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.andThen((value) => makeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, andThennedResult.unwrap());
        }

        [TestMethod]
        public void AnOk_AndThen_WithError_ReturnsNewError()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.andThen((value) => makeSimpleError());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.unwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithTheIdentityFunction_ReturnsTheOriginalOk()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.andThen((value) => SimpleResult.Ok(value));
            Assert.AreEqual(result.unwrap(), andThennedResult.unwrap());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, andThennedResult.unwrap());
        }

        [TestMethod]
        public void AnError_AndThen_WithOk_ReturnsError()
        {
            var result = makeSimpleError();
            var andThennedResult = result.andThen((value) => makeSimpleOk2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.unwrapError());
        }

        [TestMethod]
        public void AnError_AndThen_WithAnotherError_ReturnsTheFirstError()
        {
            var result = makeSimpleError();
            var andThennedResult = result.andThen((value) => makeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.unwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_EvenWithAnOk_ExecutesTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = makeSimpleOk();
            var andThennedResult = result.andThen((value) => { capturedValue++; return makeSimpleOk2(); });
            Assert.AreEqual(1, capturedValue);
        }

        [TestMethod]
        public void AnError_AndThen_EvenWithAnOk_DoesNotExecuteTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = makeSimpleError();
            var andThennedResult = result.andThen((value) => { capturedValue++; return makeSimpleOk2(); });
            Assert.AreEqual(0, capturedValue);
        }
    }
}
