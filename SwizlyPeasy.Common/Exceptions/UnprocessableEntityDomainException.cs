namespace SwizlyPeasy.Common.Exceptions
{
    /// <summary>
    /// </summary>
    public class UnprocessableEntityDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 422
        /// </summary>
        /// <param name="msg"></param>
        public UnprocessableEntityDomainException(string msg) : base(msg) { }

        public UnprocessableEntityDomainException(string msg, object? context) : base(msg, context) { }
    }
}