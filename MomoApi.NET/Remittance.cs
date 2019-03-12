using MomoApi.NET.Exceptions;

namespace MomoApi.NET
{
    public class Remittance
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Party Payee { get; set; }
        public Status Status { get; set; }
        public ErrorCode Reason { get; set; }
    }
}