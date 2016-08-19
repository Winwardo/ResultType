using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    public class Result<T, E> : IResult<T, E>
    {
        public static Result<T, E> Ok(T value)
        {
            throw new NotImplementedException();
        }

        public static Result<T, E> Error(E e)
        {
            throw new NotImplementedException();
        }

        public bool isError()
        {
            throw new NotImplementedException();
        }

        public bool isOk()
        {
            throw new NotImplementedException();
        }
    }
}
