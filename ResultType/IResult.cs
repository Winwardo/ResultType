using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    interface IResult<T, Error>
    {
        bool isOk();
        bool isError();
        T unwrapOr(T other);
        T unwrap();
    }
}
