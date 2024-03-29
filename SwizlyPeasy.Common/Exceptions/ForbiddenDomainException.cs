﻿using System.Runtime.Serialization;

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
}