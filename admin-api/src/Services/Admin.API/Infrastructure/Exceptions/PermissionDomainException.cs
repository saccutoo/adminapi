using System;

namespace API.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class PermissionDomainException : Exception
    {
        public PermissionDomainException()
        { }

        public PermissionDomainException(string message)
            : base(message)
        { }

        public PermissionDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
