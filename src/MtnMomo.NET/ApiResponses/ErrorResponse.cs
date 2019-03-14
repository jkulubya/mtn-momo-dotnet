using MtnMomo.NET.Exceptions;

namespace MtnMomo.NET.ApiResponses
{
    internal class ErrorResponse
    {
        public string Message { get; set; }
        
        
        public ErrorCode Code { get; set; }
    }
}