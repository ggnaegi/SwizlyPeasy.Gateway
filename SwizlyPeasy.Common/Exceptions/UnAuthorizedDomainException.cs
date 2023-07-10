namespace SwizlyPeasy.Common.Exceptions
{
    public class UnAuthorizedDomainException : DomainException
    {
        /// <summary>
        /// Exception with status 401
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public UnAuthorizedDomainException(string? msg, Exception? innerException) : base(msg, innerException) { }
    }
}