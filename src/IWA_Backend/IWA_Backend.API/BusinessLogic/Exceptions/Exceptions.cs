using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorisedException : Exception
    {
        public UnauthorisedException(string message) : base(message) { }
    }

    public class InvalidEntityException : Exception
    {
        public InvalidEntityException(string message) : base(message) { }
    }

    public class AlreadyBookedException : Exception
    {
        public AlreadyBookedException(string message) : base(message) { }
    }

    public class NotBookedException : Exception
    {
        public NotBookedException(string message) : base(message) { }
    }
}
