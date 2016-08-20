using System;
using System.Collections.Generic;

namespace ResultType
{
    public interface IResult<T, Error>
    {
        bool IsOk();
        bool IsError();

        T Unwrap();
        T UnwrapOr(T other);
        T Expect(string message);
        Error UnwrapError();

        IResult<U, Error> Map<U>(Func<T, U> ToCall);
        IResult<U, Error> AndThen<U>(Func<T, IResult<U, Error>> ToCall);
        List<T> ToList();

        IResult<T, Error> IfThenElse(Predicate<T> predicate, Error error);

        IResult<U, Error> And<U>(IResult<U, Error> other);
        IResult<U, Error> Or<U>(IResult<U, Error> other);

        T UnwrapUnsafe();
        Error UnwrapErrorUnsafe();
    }
}
