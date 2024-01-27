using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class BadRequestDomainException : DomainException
{
    /// <summary>
    ///     Status 400
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public BadRequestDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }
}