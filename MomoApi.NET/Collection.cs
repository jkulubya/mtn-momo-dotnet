using Flurl.Http.Configuration;
using MomoApi.NET.Exceptions;

namespace MomoApi.NET
{
    public class Collection
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Party Payer { get; set; }
        public Status Status { get; set; }
        public ErrorCode Reason { get; set; }
    }
}