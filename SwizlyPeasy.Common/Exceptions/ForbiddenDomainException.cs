using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class ForbiddenDomainException : DomainException
{
    /// <summary>
    ///     Status 403
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public ForbiddenDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }

    /// <summary>
    ///     For serialization purposes
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ForbiddenDomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // ...
    }
}