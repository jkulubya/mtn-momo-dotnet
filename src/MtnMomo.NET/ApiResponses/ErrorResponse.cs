using MtnMomo.NET.Exceptions;

namespace MtnMomo.NET.ApiResponses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        
        
        public ErrorCode Code { get; set; }
    }
}