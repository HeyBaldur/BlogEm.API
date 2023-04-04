using System;

namespace blog_api.services.Exceptions
{
    /// <summary>
    /// Throw this exception if there is any XSS attack
    /// </summary>
    internal class CorruptedException: Exception
    {
        public CorruptedException(string message)
        : base(message)
        {
        }

        public CorruptedException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }

    internal class CathedException : Exception
    {
        public CathedException(string message)
        : base(message)
        {
        }

        public CathedException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
