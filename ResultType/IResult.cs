﻿using System;
using System.Collections.Generic;

namespace ResultType
{
    public interface IResult<T, Error>
    {
        bool IsOk();
        bool IsError();

        T Unwrap();
        T UnwrapOr(T other);
        Error UnwrapError();

        IResult<U, Error> Map<U>(Func<T, U> ToCall);
        IResult<U, Error> AndThen<U>(Func<T, IResult<U, Error>> ToCall);
        List<T> ToList();

        IResult<T, Error> IfThenElse(Predicate<T> predicate, Error error);

        T UnwrapUnsafe();
        Error UnwrapErrorUnsafe();
    }
}
