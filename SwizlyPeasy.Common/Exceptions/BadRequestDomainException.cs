namespace SwizlyPeasy.Common.Exceptions;

/// <summary>
/// </summary>
public class BadRequestDomainException : DomainException
{
    public BadRequestDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }
}