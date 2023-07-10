namespace SwizlyPeasy.Common.Exceptions
{
    /// <summary>
    /// </summary>
    public class ConflictDomainException : DomainException
    {
        /// <summary>
        /// Error 409
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public ConflictDomainException(string? msg, Exception? innerException) : base(msg, innerException) { }
    }
}