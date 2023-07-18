using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

public abstract class DomainException : Exception
{
    /// <summary>
    ///     Should implement this constructor to be compliant
    ///     with standards.
    ///     From Microsoft's source code.
    ///     -> Creates a new Exception.  All derived classes should
    ///     provide this constructor.
    ///     Note: the stack trace is not started until the exception
    ///     is thrown
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    protected DomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }

    /// <summary>
    ///     For serialization purposes
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected DomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // ...
    }


    public object? Context { get; set; }
}