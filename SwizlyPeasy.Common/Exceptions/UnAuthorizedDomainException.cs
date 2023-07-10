namespace SwizlyPeasy.Common.Exceptions
{
    public class UnAuthorizedDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 401
        /// </summary>
        /// <param name="msg"></param>
        public UnAuthorizedDomainException(string msg) : base(msg)
        {
        }

        public UnAuthorizedDomainException(string msg, object? context) : base(msg, context)
        {
        }

        public UnAuthorizedDomainException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}