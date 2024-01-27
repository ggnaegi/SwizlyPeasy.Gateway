namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class TooManyRequestsException : DomainException
{
    /// <summary>
    ///     Too Many Requests Exception, exception with status 429
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public TooManyRequestsException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }
}