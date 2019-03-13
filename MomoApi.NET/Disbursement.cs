using System;
using MomoApi.NET.Exceptions;
using Newtonsoft.Json;

namespace MomoApi.NET
{
    public class Disbursement
    {
        [JsonConverter(typeof(StringDecimalConverter))]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string FinancialTransactionId { get; set; }
        public string ExternalId { get; set; }
        public Party Payee { get; set; }
        public Status Status { get; set; }
        public ErrorCode Reason { get; set; }
    }
}