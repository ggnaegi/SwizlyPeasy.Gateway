using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class InternalDomainException : DomainException
{
    /// <summary>
    ///     Exception with status 500
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public InternalDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }
}