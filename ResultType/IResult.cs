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

        IResult<T, Error> AndThen(Func<T, IResult<T, Error>> ToCall);
        IResult<U, Error> Map<U>(Func<T, IResult<U, Error>> ToCall);
        List<IResult<T, Error>> ToList();

        T UnwrapUnsafe();
        Error UnwrapErrorUnsafe();
    }
}
