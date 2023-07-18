using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class ConflictDomainException : DomainException
{
    /// <summary>
    ///     Error 409
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public ConflictDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }

    /// <summary>
    ///     For serialization purposes
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ConflictDomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // ...
    }
}