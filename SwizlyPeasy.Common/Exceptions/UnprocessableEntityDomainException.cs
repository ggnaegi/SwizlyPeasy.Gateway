namespace SwizlyPeasy.Common.Exceptions
{
    public class UnprocessableEntityDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 422
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public UnprocessableEntityDomainException(string? msg, Exception? innerException) : base(msg, innerException) { }
    }
}