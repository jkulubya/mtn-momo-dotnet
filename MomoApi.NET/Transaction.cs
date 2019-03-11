using Flurl.Http.Configuration;

namespace MomoApi.NET
{
    public class Transaction
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Payer Payer { get; set; }
        public Status Status { get; set; }
    }
}