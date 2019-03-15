using System;

namespace MtnMomo.NET.Exceptions
{
    public class MomoException : Exception
    {
        public MomoException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
        
        public ErrorCode ErrorCode { get; }
    }
}