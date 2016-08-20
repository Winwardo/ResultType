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
        public void AnOk_UnwrapUnsafe_UnwrapsFine()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.UnwrapUnsafe());
        }

        [TestMethod]
        [ExpectedException(typeof(AttemptedToUnwrapErrorException))]
        public void AnError_UnwrapUnsafe_Throws()
        {
            var result = MakeSimpleError();
            result.UnwrapUnsafe();
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
        public void AnOk_UnwrapErrorUnsafe_Throws()
        {
            var result = MakeSimpleOk();
            result.UnwrapErrorUnsafe();
        }

        [TestMethod]
        public void AnError_UnwrapErrorUnsafe_ReturnsTheError()
        {
            var result = MakeSimpleError();
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, result.UnwrapErrorUnsafe());
        }

        [TestMethod]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, andThennedResult.UnwrapUnsafe());
        }

        [TestMethod]
        public void AnOk_AndThen_WithError_ReturnsNewError()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleError());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
        }

        [TestMethod]
        public void AnOk_AndThen_WithTheIdentityFunction_ReturnsTheOriginalOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => SimpleIntResult.Ok(value));
            Assert.AreEqual(result.UnwrapUnsafe(), andThennedResult.UnwrapUnsafe());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, andThennedResult.UnwrapUnsafe());
        }

        [TestMethod]
        public void AnError_AndThen_WithOk_ReturnsError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
        }

        [TestMethod]
        public void AnError_AndThen_WithAnotherError_ReturnsTheFirstError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
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
            Assert.AreEqual(SIMPLE_OKAY_STRING_1, mappedResult.UnwrapUnsafe());
        }

        [TestMethod]
        public void AnOk_MappedToTheSameOkType_SharesTypeSignature()
        {
            IResult<int, string> result = MakeSimpleOk();
            IResult<int, string> mappedResult = result.Map(value => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, mappedResult.UnwrapUnsafe());
        }

        [TestMethod]
        public void AnOk_MappedToANewOkType_HasTheNewTypeSignature()
        {
            IResult<int, string> result = MakeSimpleOk();
            IResult<string, string> mappedResult = result.Map<string>(value => MakeSimpleStringOk());
            Assert.AreEqual(SIMPLE_OKAY_STRING_1, mappedResult.UnwrapUnsafe());
        }

        [TestMethod]
        public void AnError_MappedToTheSameOkType_WithOk_ContainsTheOriginalError()
        {
            IResult<int, string> result = MakeSimpleError();
            IResult<int, string> mappedResult = result.Map(value => MakeSimpleOk());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, mappedResult.UnwrapErrorUnsafe());
        }

        [TestMethod]
        public void AnError_MappedToTheSameOkType_WithNewError_ContainsTheOriginalError()
        {
            IResult<int, string> result = MakeSimpleError();
            IResult<int, string> mappedResult = result.Map(value => MakeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, mappedResult.UnwrapErrorUnsafe());
        }

        [TestMethod]
        [ExpectedException(typeof(ResultWasGivenNullException))]
        public void ResultOk_PassedANull_Throws()
        {
            Result<Object, string>.Ok(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ResultWasGivenNullException))]
        public void ResultError_PassedANull_Throws()
        {
            Result<Object, string>.Error(null);
        }

        [TestMethod]
        public void ResultOkOrNull_PassedANull_UnwrapsToNull()
        {
            var result = Result<Object, string>.OkOrNull(null);
            Assert.AreEqual(null, result.UnwrapUnsafe());
        }

        [TestMethod]
        public void ResultErrorOrNull_PassedANull_UnwrapsToNull()
        {
            var result = Result<string, Object>.ErrorOrNull(null);
            Assert.AreEqual(null, result.UnwrapErrorUnsafe());
        }
    }
}
