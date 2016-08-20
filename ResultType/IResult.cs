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

        T UnwrapOr(T other);
        Error UnwrapError();

        IResult<T, Error> AndThen(Func<T, IResult<T, Error>> toCall);

        T Unwrap();
    }
}
