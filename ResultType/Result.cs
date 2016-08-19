using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    public class Result<T, E> : IResult<T, E>
    {
        private readonly bool ok;
        T value;
        E error;

        public static Result<T, E> Ok(T value)
        {
            return new Result<T, E>(value, default(E));
        }

        public static Result<T, E> Error(E error)
        {
            return new Result<T, E>(default(T), error);
        }

        private Result(T value, E error)
        {
            this.value = value;
            this.error = error;

            if (value == null)
            {
                ok = false;
            }
            else
            {
                ok = true;
            }
        }

        public bool isError()
        {
            throw new NotImplementedException();
        }

        public bool isOk()
        {
            return ok;
        }
    }
}
