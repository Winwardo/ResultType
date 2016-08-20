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

            if (value == null || error != null)
            {
                ok = false;
            }
            else
            {
                ok = true;
            }
        }

        public bool IsError()
        {
            return !ok;
        }

        public bool IsOk()
        {
            return ok;
        }

        public T Unwrap()
        {
            if (ok)
            {
                return value;
            }
            else
            {
                throw new AttemptedToUnwrapErrorException();
            }
        }

        public T UnwrapOr(T other)
        {
            if (ok)
            {
                return value;
            }
            else
            {
                return other;
            }
        }

        public E UnwrapError()
        {
            if (ok)
            {
                throw new AttemptedToUnwrapErrorOfOkException();
            }
            else
            {
                return error;
            }
        }

        public IResult<T, E> AndThen(Func<T, IResult<T, E>> toCall)
        {
            if (ok)
            {
                return toCall(value);
            }
            else
            {
                return Result<T, E>.Error(error);
            }
        }
    }
}
