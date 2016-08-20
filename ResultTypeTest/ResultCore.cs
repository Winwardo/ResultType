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
            Assert.IsTrue(result.IsOk());
        }

        [TestMethod]
        public void AnError_IsOk_ReturnsFalse()
        {
            var result = makeSimpleError();
            Assert.IsFalse(result.IsOk());
        }

        [TestMethod]
        public void AnOk_IsError_ReturnsFalse()
        {
            var result = makeSimpleOk();
            Assert.IsFalse(result.IsError());
        }

        [TestMethod]
        public void AnError_IsError_ReturnsTrue()
        {
            var result = makeSimpleError();
            Assert.IsTrue(result.IsError());
        }

        [TestMethod]
        public void AnOk_Unwrap_UnwrapsFine()
        {
            var result = makeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.Unwrap());
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorException))]
        public void AnError_Unwrap_Throws()
        {
            var result = makeSimpleError();
            result.Unwrap();
        }

        [TestMethod]
        public void AnOk_UnwrapOr_ReturnsTheOkayValue()
        {
            var result = makeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [TestMethod]
        public void AnError_UnwrapOr_ReturnsTheOtherValue()
        {
            var result = makeSimpleError();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorOfOkException))]
        public void AnOk_UnwrapError_Throws()
        {
            var result = makeSimpleOk();
            result.UnwrapError();
        }

        [TestMethod]
        public void AnError_UnwrapError_ReturnsTheError()
        {
            var result = makeSimpleError();
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, result.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.AndThen((value) => makeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, andThennedResult.Unwrap());
        }

        [TestMethod]
        public void AnOk_AndThen_WithError_ReturnsNewError()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.AndThen((value) => makeSimpleError());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithTheIdentityFunction_ReturnsTheOriginalOk()
        {
            var result = makeSimpleOk();
            var andThennedResult = result.AndThen((value) => SimpleResult.Ok(value));
            Assert.AreEqual(result.Unwrap(), andThennedResult.Unwrap());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, andThennedResult.Unwrap());
        }

        [TestMethod]
        public void AnError_AndThen_WithOk_ReturnsError()
        {
            var result = makeSimpleError();
            var andThennedResult = result.AndThen((value) => makeSimpleOk2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnError_AndThen_WithAnotherError_ReturnsTheFirstError()
        {
            var result = makeSimpleError();
            var andThennedResult = result.AndThen((value) => makeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_EvenWithAnOk_ExecutesTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = makeSimpleOk();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return makeSimpleOk2(); });
            Assert.AreEqual(1, capturedValue);
        }

        [TestMethod]
        public void AnError_AndThen_EvenWithAnOk_DoesNotExecuteTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = makeSimpleError();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return makeSimpleOk2(); });
            Assert.AreEqual(0, capturedValue);
        }
    }
}
