using System;

namespace MomoApi.NET.Exceptions
{
    public class MomoException : Exception
    {
        public MomoException(ErrorCode errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
        
        public ErrorCode ErrorCode { get; }
        public string Message { get; }
    }
}