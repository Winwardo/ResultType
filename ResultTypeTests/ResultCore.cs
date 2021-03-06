﻿using System;
using ResultType;

namespace ResultTypeTest
{
    using NUnit.Framework;
    using SimpleIntResult = Result<int, string>;
    using SimpleStringResult = Result<string, string>;

    [TestFixture]
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

        private SimpleStringResult MakeSimpleStringError()
        {
            return SimpleStringResult.Error(SIMPLE_ERROR_MESSAGE_1);
        }

        // -------------------------------------------------------------

        [Test]
        public void AnOk_IsOk_ReturnsTrue()
        {
            var result = MakeSimpleOk();
            Assert.IsTrue(result.IsOk());
        }

        [Test]
        public void AnError_IsOk_ReturnsFalse()
        {
            var result = MakeSimpleError();
            Assert.IsFalse(result.IsOk());
        }

        [Test]
        public void AnOk_IsError_ReturnsFalse()
        {
            var result = MakeSimpleOk();
            Assert.IsFalse(result.IsError());
        }

        [Test]
        public void AnError_IsError_ReturnsTrue()
        {
            var result = MakeSimpleError();
            Assert.IsTrue(result.IsError());
        }

        [Test]
        public void AnOk_UnwrapUnsafe_UnwrapsFine()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.UnwrapUnsafe());
        }

        [Test]
        public void AnError_UnwrapUnsafe_Throws()
        {
            var result = MakeSimpleError();
            Assert.Throws(typeof(AttemptedToUnwrapErrorException), delegate { result.UnwrapUnsafe(); });
        }

        [Test]
        public void AnOk_UnwrapOr_ReturnsTheOkayValue()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [Test]
        public void AnError_UnwrapOr_ReturnsTheOtherValue()
        {
            var result = MakeSimpleError();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, result.UnwrapOr(SIMPLE_OKAY_VALUE_2));
        }

        [Test]
        public void AnOk_UnwrapErrorUnsafe_Throws()
        {
            var result = MakeSimpleOk();
            Assert.Throws(typeof(AttemptedToUnwrapErrorOfOkException), delegate { result.UnwrapErrorUnsafe(); });
        }

        [Test]
        public void AnError_UnwrapErrorUnsafe_ReturnsTheError()
        {
            var result = MakeSimpleError();
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, result.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnOk_AndThen_WithOk_ReturnsNewOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, andThennedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_AndThen_WithError_ReturnsNewError()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => MakeSimpleError());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnOk_AndThen_WithTheIdentityFunction_ReturnsTheOriginalOk()
        {
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => SimpleIntResult.Ok(value));
            Assert.AreEqual(result.UnwrapUnsafe(), andThennedResult.UnwrapUnsafe());
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, andThennedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_AndThenned_ToANewType_HasTheNewTypeSignature()
        {
            IResult<int, string> result = MakeSimpleOk();
            IResult<string, string> mappedResult = result.AndThen<string>((value) => MakeSimpleStringOk());
        }

        [Test]
        public void AnError_AndThen_WithOk_ReturnsError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleOk2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_AndThen_WithAnotherError_ReturnsTheFirstError()
        {
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => MakeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, andThennedResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnOk_AndThen_EvenWithAnOk_ExecutesTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = MakeSimpleOk();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return MakeSimpleOk2(); });
            Assert.AreEqual(1, capturedValue);
        }

        [Test]
        public void AnError_AndThen_EvenWithAnOk_DoesNotExecuteTheAndThenFunction()
        {
            var capturedValue = 0;
            var result = MakeSimpleError();
            var andThennedResult = result.AndThen((value) => { capturedValue++; return MakeSimpleOk2(); });
            Assert.AreEqual(0, capturedValue);
        }

        [Test]
        public void AnOk_Mapped_ToAnOkOfNewType_ReturnsTheNewOk()
        {
            var result = MakeSimpleOk();
            var mappedResult = result.Map<string>((value) => SIMPLE_OKAY_STRING_1);
            Assert.AreEqual(SIMPLE_OKAY_STRING_1, mappedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_Mapped_WithTheIdentityFunction_ReturnsTheNewOk()
        {
            var result = MakeSimpleOk();
            var mappedResult = result.Map(value => value);
            Assert.AreEqual(result.UnwrapUnsafe(), mappedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_MappedToTheSameOkType_SharesTypeSignature()
        {
            IResult<int, string> result = MakeSimpleOk();
            IResult<int, string> mappedResult = result.Map(value => SIMPLE_OKAY_VALUE_2);
            Assert.AreEqual(SIMPLE_OKAY_VALUE_2, mappedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_MappedToANewOkType_HasTheNewTypeSignature()
        {
            IResult<int, string> result = MakeSimpleOk();
            IResult<string, string> mappedResult = result.Map<string>(value => SIMPLE_OKAY_STRING_1);
            Assert.AreEqual(SIMPLE_OKAY_STRING_1, mappedResult.UnwrapUnsafe());
        }

        [Test]
        public void AnError_MappedToTheSameOkType_WithOk_ContainsTheOriginalError()
        {
            IResult<int, string> result = MakeSimpleError();
            IResult<int, string> mappedResult = result.Map(value => SIMPLE_OKAY_VALUE_1);
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, mappedResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_MappedToTheSomeOkType_AndThenned_WithNewError_ContainsTheOriginalError()
        {
            IResult<int, string> result = MakeSimpleError();
            IResult<int, string> mappedResult = result.Map(value => SIMPLE_OKAY_VALUE_1).AndThen(value => MakeSimpleError2());
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, mappedResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void ResultOk_PassedANull_Throws()
        {
            Assert.Throws(typeof(ResultWasGivenNullException), delegate { Result<Object, string>.Ok(null); });
        }

        [Test]
        public void ResultError_PassedANull_Throws()
        {
            Assert.Throws(typeof(ResultWasGivenNullException), delegate { Result<Object, string>.Error(null); });
        }

        [Test]
        public void ResultOkOrNull_PassedANull_UnwrapsToNull()
        {
            var result = Result<Object, string>.OkOrNull(null);
            Assert.AreEqual(null, result.UnwrapUnsafe());
        }

        [Test]
        public void ResultErrorOrNull_PassedANull_UnwrapsToNull()
        {
            var result = Result<string, Object>.ErrorOrNull(null);
            Assert.AreEqual(null, result.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnUncheckedResult_Unwrap_Throws()
        {
            var result = MakeSimpleOk();
            Assert.Throws(typeof(AttemptedToUnwrapUncheckedResultException), delegate { result.Unwrap(); });
        }

        [Test]
        public void AnUncheckedResult_UnwrapError_Throws()
        {
            var result = MakeSimpleOk();
            Assert.Throws(typeof(AttemptedToUnwrapUncheckedResultException), delegate { result.UnwrapError(); });
        }

        [Test]
        public void AnIsOkCheckedOkResult_Unwrap_ReturnsTheValue()
        {
            var result = MakeSimpleOk();
            if (result.IsOk())
            {
                Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.Unwrap());
            }
        }

        [Test]
        public void AnIsErrorCheckedOkResult_Unwrap_ReturnsTheValue()
        {
            var result = MakeSimpleOk();
            if (!result.IsError())
            {
                Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.Unwrap());
            }
        }

        [Test]
        public void AnOk_ToList_ReturnsAListWithTheUnwrappedOk()
        {
            var result = MakeSimpleOk();
            var asList = result.ToList();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, asList[0]);
            Assert.AreEqual(1, asList.Count);
        }

        [Test]
        public void AnError_ToList_ReturnsAnEmptyList()
        {
            var result = MakeSimpleError();
            var asList = result.ToList();
            Assert.AreEqual(0, asList.Count);
        }

        [Test]
        public void AnOk_IfThenElse_PredicateIsTrue_ContainsTheOkValue()
        {
            var result = MakeSimpleOk();
            var afterPred = result.IfThenElse((value) => true, SIMPLE_ERROR_MESSAGE_1);
            Assert.AreEqual(result.UnwrapUnsafe(), afterPred.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_IfThenElse_PredicateIsFalse_ContainsTheErrorValue()
        {
            var result = MakeSimpleOk();
            var afterPred = result.IfThenElse((value) => false, SIMPLE_ERROR_MESSAGE_1);
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, afterPred.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_IfThenElse_EvenIfPredicateIsTrue_ContainsTheOriginalErrorValue()
        {
            var result = MakeSimpleError();
            var afterPred = result.IfThenElse((value) => true, SIMPLE_ERROR_MESSAGE_2);
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, afterPred.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_IfThenElse_EvenIfPredicateIsFalse_ContainsTheOriginalErrorValue()
        {
            var result = MakeSimpleError();
            var afterPred = result.IfThenElse((value) => false, SIMPLE_ERROR_MESSAGE_2);
            Assert.AreEqual(SIMPLE_ERROR_MESSAGE_1, afterPred.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnOk_Expect_ReturnsOkValue()
        {
            var result = MakeSimpleOk();
            Assert.AreEqual(SIMPLE_OKAY_VALUE_1, result.Expect("Some message"));
        }

        [Test]
        public void AnError_Expect_ThrowsWithErrorMessage()
        {
            var result = MakeSimpleError();
            Assert.Throws(typeof(ExpectedAnOkException), () => result.Expect("Some message"));
        }

        [Test]
        public void AnOk_And_AnOk_ContainsTheSecondOk()
        {
            SimpleIntResult result = MakeSimpleOk();
            SimpleStringResult other = MakeSimpleStringOk();
            var andResult = result.And(other);
            Assert.AreEqual(other.UnwrapUnsafe(), andResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_And_AnError_ContainsTheError()
        {
            SimpleIntResult result = MakeSimpleOk();
            SimpleStringResult other = MakeSimpleStringError();
            var andResult = result.And(other);
            Assert.AreEqual(other.UnwrapErrorUnsafe(), andResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_And_AnOk_ContainsTheError()
        {
            SimpleIntResult result = MakeSimpleError();
            SimpleStringResult other = MakeSimpleStringOk();
            var andResult = result.And(other);
            Assert.AreEqual(result.UnwrapErrorUnsafe(), andResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnError_And_AnError_ContainsTheFirstError()
        {
            SimpleIntResult result = MakeSimpleError();
            SimpleStringResult other = MakeSimpleStringError();
            var andResult = result.And(other);
            Assert.AreEqual(result.UnwrapErrorUnsafe(), andResult.UnwrapErrorUnsafe());
        }

        [Test]
        public void AnOk_Or_AnOk_ContainsTheFirstOk()
        {
            SimpleIntResult result = MakeSimpleOk();
            SimpleIntResult other = MakeSimpleOk2();
            var orResult = result.Or(other);
            Assert.AreEqual(result.UnwrapUnsafe(), orResult.UnwrapUnsafe());
        }

        [Test]
        public void AnOk_Or_AnError_ContainsTheOk()
        {
            SimpleIntResult result = MakeSimpleOk();
            SimpleIntResult other = MakeSimpleOk2();
            var orResult = result.Or(other);
            Assert.AreEqual(result.UnwrapUnsafe(), orResult.UnwrapUnsafe());
        }

        [Test]
        public void AnError_Or_AnOk_ContainsTheOk()
        {
            SimpleIntResult result = MakeSimpleError();
            SimpleIntResult other = MakeSimpleOk2();
            var orResult = result.Or(other);
            Assert.AreEqual(other.UnwrapUnsafe(), orResult.UnwrapUnsafe());
        }

        [Test]
        public void AnError_Or_AnError_ContainsTheSecondError()
        {
            SimpleIntResult result = MakeSimpleError();
            SimpleIntResult other = MakeSimpleError2();
            var orResult = result.Or(other);
            Assert.AreEqual(other.UnwrapErrorUnsafe(), orResult.UnwrapErrorUnsafe());
        }
    }
}