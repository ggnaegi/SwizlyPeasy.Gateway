namespace SwizlyPeasy.Common.Exceptions
{
    /// <summary>
    /// </summary>
    public class InternalDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 500
        /// </summary>
        /// <param name="msg"></param>
        public InternalDomainException(string msg) : base(msg)
        {
        }

        public InternalDomainException(string msg, object? context) : base(msg, context)
        {
        }

        public InternalDomainException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}