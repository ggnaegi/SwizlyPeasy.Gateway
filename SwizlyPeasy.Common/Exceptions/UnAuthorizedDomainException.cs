﻿using System.Runtime.Serialization;

namespace SwizlyPeasy.Common.Exceptions;

[Serializable]
public class UnAuthorizedDomainException : DomainException
{
    /// <summary>
    ///     Exception with status 401
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="innerException"></param>
    public UnAuthorizedDomainException(string? msg, Exception? innerException) : base(msg, innerException)
    {
    }
}