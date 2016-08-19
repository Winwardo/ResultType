using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    public interface IResult<T, Error>
    {
        bool isOk();
        bool isError();

        T unwrapOr(T other);
        Error unwrapError();

        IResult<T, Error> andThen(Func<T, IResult<T, Error>> toCall);

        T unwrap();
    }
}
