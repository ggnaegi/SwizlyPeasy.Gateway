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
        public ConflictDomainException(string msg) : base(msg)
        {
        }

        public ConflictDomainException(string msg, object? context) : base(msg, context)
        {
        }
    }
}