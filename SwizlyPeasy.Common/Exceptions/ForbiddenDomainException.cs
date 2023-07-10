namespace SwizlyPeasy.Common.Exceptions
{
    public class ForbiddenDomainException : DomainException
    {
        /// <summary>
        /// Status 403
        /// </summary>
        /// <param name="msg"></param>
        public ForbiddenDomainException(string msg) : base(msg)
        {

        }
        public ForbiddenDomainException(string msg, object? context) : base(msg, context)
        {
        }

        public ForbiddenDomainException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}