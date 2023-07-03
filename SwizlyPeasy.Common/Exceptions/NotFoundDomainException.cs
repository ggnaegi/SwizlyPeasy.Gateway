namespace SwizlyPeasy.Common.Exceptions
{
    /// <summary>
    /// </summary>
    public class NotFoundDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 404
        /// </summary>
        /// <param name="msg"></param>
        public NotFoundDomainException(string msg) : base(msg) { }

        public NotFoundDomainException(string msg, object? context) : base(msg, context)
        {
        }
    }
}