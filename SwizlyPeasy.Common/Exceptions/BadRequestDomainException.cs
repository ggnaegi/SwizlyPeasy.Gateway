namespace SwizlyPeasy.Common.Exceptions
{
    /// <summary>
    /// </summary>
    public class BadRequestDomainException : DomainException
    {
        /// <summary>
        /// Error 400
        /// </summary>
        /// <param name="msg"></param>
        public BadRequestDomainException(string msg) : base(msg)
        {
        }

        public BadRequestDomainException(string msg, object? context) : base(msg, context)
        {
        }
    }
}