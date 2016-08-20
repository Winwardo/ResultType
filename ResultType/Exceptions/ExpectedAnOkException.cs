using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultType
{
    public class ExpectedAnOkException : Exception
    {
        private ExpectedAnOkException() { }

        public ExpectedAnOkException(string message)
            : base(message)
        {
        }
    }
}
