using MtnMomo.NET.Exceptions;
using Newtonsoft.Json;

namespace MtnMomo.NET
{
    public class Collection
    {
        [JsonConverter(typeof(StringDecimalConverter))]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Party Payer { get; set; }
        public Status Status { get; set; }
        public ErrorCode Reason { get; set; }
    }
}