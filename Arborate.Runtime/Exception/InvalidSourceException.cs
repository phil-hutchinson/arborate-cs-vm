using System;
using System.Collections.Generic;
using System.Text;

namespace ArborateVirtualMachine.Exception
{
    public class InvalidSourceException: System.Exception
    {
        public InvalidSourceDetail DetailCode { get; }

        public InvalidSourceException() : base()
        {
        }

        public InvalidSourceException(InvalidSourceDetail detailCode) : base()
        {
            DetailCode = detailCode;
        }

        public InvalidSourceException(string message) : base(message)
        {
        }

        public InvalidSourceException(InvalidSourceDetail detailCode, string message) : base(message)
        {
            DetailCode = detailCode;
        }

        public InvalidSourceException(string message, System.Exception inner): base(message, inner)
        {
        }

        public InvalidSourceException(InvalidSourceDetail detailCode, string message, System.Exception inner) : base(message, inner)
        {
            DetailCode = detailCode;
        }
    }
}
