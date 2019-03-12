using MomoApi.NET.Exceptions;

namespace MomoApi.NET.ApiResponses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        
        
        public ErrorCode Code { get; set; }
    }
}