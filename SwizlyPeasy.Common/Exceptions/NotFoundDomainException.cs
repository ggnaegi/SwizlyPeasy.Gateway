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
}