using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResultType;

namespace ResultTypeTest
{
    using SimpleIntResult = Result<int, string>;
    using SimpleStringResult = Result<string, string>;

    [TestClass]
    public class ResultCore
    {
        private static int SIMPLE_OKAY_VALUE_1 = 5;
        private static int SIMPLE_OKAY_VALUE_2 = SIMPLE_OKAY_VALUE_1 + 1;
        private static string SIMPLE_OKAY_STRING_1 = "some ok value";
        private static string SIMPLE_OKAY_STRING_2 = "some other ok value";
        private static string SIMPLE_ERROR_MESSAGE_1 = "some error";
        private static string SIMPLE_ERROR_MESSAGE_2 = "some other error";

        private SimpleIntResult MakeSimpleOk()
        {
            return SimpleIntResult.Ok(SIMPLE_OKAY_VALUE_1);
        }

        private SimpleIntResult MakeSimpleOk2()
        {
            return SimpleIntResult.Ok(SIMPLE_OKAY_VALUE_2);
        }

        private SimpleStringResult MakeSimpleStringOk()
        {
            return SimpleStringResult.Ok(SIMPLE_OKAY_STRING_1);
        }

        private SimpleStringResult MakeOtherSimpleStringOk()
        {
            return SimpleStringResult.Ok(SIMPLE_OKAY_STRING_1);
        }

        private SimpleIntResult MakeSimpleError()
        {
            return SimpleIntResult.Error(SIMPLE_ERROR_MESSAGE_1);
        }

        private SimpleIntResult MakeSimpleError2()
        {
            return SimpleIntResult.Error(SIMPLE_ERROR_MESSAGE_2);
        }

        {
        }

        {
        }

        {
            public string message;
        }

        {
        }

        // -------------------------------------------------------------

        [TestMethod]
        public void AnOk_IsOk_ReturnsTrue()
        {
            var result = MakeSimpleOk();
            Assert.IsTrue(result.IsOk());
        }

        [TestMethod]
        public void AnError_IsOk_ReturnsFalse()
        {
            var result = MakeSimpleError();
            Assert.IsFalse(result.IsOk());
        }

        [TestMethod]
        public void AnOk_IsError_ReturnsFalse()
        {
            var result = MakeSimpleOk();
            Assert.IsFalse(result.IsError());
        }

        [TestMethod]
        public void AnError_IsError_ReturnsTrue()
        {
            var result = MakeSimpleError();
            Assert.IsTrue(result.IsError());
        }

        [TestMethod]
        public void AnOk_Unwrap_UnwrapsFine()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.Unwrap());
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorException))]
        public void AnError_Unwrap_Throws()
        {
            var result = MakeSimpleError();
            result.Unwrap();
        }

        [TestMethod]
        public void AnOk_UnwrapOr_ReturnsTheOkayValue()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [TestMethod]
        public void AnError_UnwrapOr_ReturnsTheOtherValue()
        {
            var result = MakeSimpleError();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorOfOkException))]
        public void AnOk_UnwrapError_Throws()
        {
            var result = MakeSimpleOk();
            result.UnwrapError();
        }

        [TestMethod]
        public void AnError_UnwrapError_ReturnsTheError()
        {
            var result = MakeSimpleError();
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, result.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, andThennedResult.Unwrap());
        }

        [TestMethod]
        public void AnOk_AndThen_WithError_ReturnsNewError()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleError());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_WithTheIdentityFunction_ReturnsTheOriginalOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => SimpleIntResult.Ok(value));
            Assert.AreEqual(result.Unwrap(), andThennedResult.Unwrap());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, andThennedResult.Unwrap());
        }

        [TestMethod]
        public void AnError_AndThen_WithOk_ReturnsError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnError_AndThen_WithAnotherError_ReturnsTheFirstError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapError());
        }

        [TestMethod]
        public void AnOk_AndThen_EvenWithAnOk_ExecutesTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return MakeSimpleOk2(); });
            Assert.AreEqual(1, capturedValue);
        }

        [TestMethod]
        public void AnError_AndThen_EvenWithAnOk_DoesNotExecuteTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return MakeSimpleOk2(); });
            Assert.AreEqual(0, capturedValue);
        }

        [TestMethod]
        public void AnOk_Mapped_ToAnOkOfNewType_ReturnsTheNewOk()
        {
            var result = MakeSimpleOk();
            var mappedResult = result.Map<string>((value) => MakeSimpleStringOk());
            Assert.AreEqual(SIMPLE_OKAY_STRING_1, mappedResult.Unwrap());
        }
    }
}
