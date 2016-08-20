using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    public interface IResult<T, Error>
    {
        bool IsOk();
        bool IsError();

        T Unwrap();
        T UnwrapOr(T other);
        Error UnwrapError();

        IResult<U, Error> AndThen<U>(Func<T, IResult<U, Error>> ToCall);
        IResult<U, Error> Map<U>(Func<T, IResult<U, Error>> ToCall);
        List<T> ToList();

        T UnwrapUnsafe();
        Error UnwrapErrorUnsafe();
    }
}
