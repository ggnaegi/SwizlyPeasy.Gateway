using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class NotFoundDomainException : DomainException
{
    /// <summary>
    ///     Exception with status 404
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public NotFoundDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }

    /// <summary>
    ///     For serialization purposes
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected NotFoundDomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // ...
    }
}