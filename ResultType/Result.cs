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
        private bool hasBeenChecked = false;
        T value;
        E error;

        public static Result<T, E> Ok(T value)
        {
            if (value != null)
            {
                return Result<T, E>.OkOrNull(value);
            }
            else
            {
                throw new ResultWasGivenNullException();
            }
        }

        public static Result<T, E> OkOrNull(T value)
        {
            return new Result<T, E>(value, default(E), true);
        }

        public static Result<T, E> Error(E error)
        {
            if (error != null)
            {
                return Result<T, E>.ErrorOrNull(error);
            }
            else
            {
                throw new ResultWasGivenNullException();
            }
        }

        public static Result<T, E> ErrorOrNull(E error)
        {
            return new Result<T, E>(default(T), error, false);
        }

        private Result(T value, E error, bool isOk)
        {
            this.value = value;
            this.error = error;
            this.ok = isOk;
        }

        public bool IsError()
        {
            return !IsOk();
        }

        public bool IsOk()
        {
            hasBeenChecked = true;
            return ok;
        }


        public T UnwrapUnsafe()
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

        public E UnwrapErrorUnsafe()
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

        public IResult<U, E> AndThen<U>(Func<T, IResult<U, E>> ToCall)
        {
            if (ok)
            {
                return ToCall(value);
            }
            else
            {
                return Result<U, E>.Error(error);
            }
        }

        public IResult<U, E> Map<U>(Func<T, U> ToCall)
        {
            if (ok)
            {
                return Result<U, E>.Ok(ToCall(value));
            }
            else
            {
                return Result<U, E>.Error(error);
            }
        }

        public T Unwrap()
        {
            if (hasBeenChecked)
            {
                return UnwrapUnsafe();
            }
            else
            {
                throw new AttemptedToUnwrapUncheckedResultException();
            }
        }

        public E UnwrapError()
        {
            if (hasBeenChecked)
            {
                return UnwrapErrorUnsafe();
            }
            else
            {
                throw new AttemptedToUnwrapUncheckedResultException();
            }
        }

        public List<T> ToList()
        {
            if (ok)
            {
                return new List<T> { value };
            }
            else
            {
                return new List<T>();
            }
        }

        public IResult<T, E> IfThenElse(Predicate<T> predicate, E newError)
        {
            if (ok)
            {
                if (predicate(value))
                {
                    return Result<T, E>.Ok(value);
                }
                else
                {
                    return Result<T, E>.Error(newError);
                }
            }
            else
            {
                return Result<T, E>.Error(error);
            }
        }

        public T Expect(string message)
        {
            if (ok)
            {
                return value;
            }
            else
            {
                throw new ExpectedAnOkException(message);
            }
        }

        public IResult<U, E> And<U>(IResult<U, E> other)
        {
            if (ok)
            {
                return other;
            }
            else
            {
                return Result<U, E>.Error(error);
            }
        }

        public IResult<U, E> Or<U>(IResult<U, E> other)
        {
            throw new NotImplementedException();
        }
    }
}
