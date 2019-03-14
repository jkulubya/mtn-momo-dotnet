using MtnMomo.NET.Exceptions;
using Newtonsoft.Json;

namespace MtnMomo.NET
{
    public class Disbursement
    {
        [JsonConverter(typeof(StringDecimalConverter))]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Party Payee { get; set; }
        public TransactionStatus Status { get; set; }
        public ErrorCode Reason { get; set; }
    }
}