using System;
using System.Globalization;
using System.Threading.Tasks;
using Flurl.Http;
using MtnMomo.NET.ApiResponses;

namespace MtnMomo.NET
{
    public class Disbursements : BaseApi
    {
        protected override string TokenPath { get; } = "/disbursement/token/";
        protected override string SubscriptionKey { get; }

        internal Disbursements(HttpClientFactory clientFactory, MomoConfig config) : base(clientFactory, config)
        {
            var key = config?.SubscriptionKeys?.Disbursements;
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("The disbursements subscription key cannot be null");
            SubscriptionKey = key;
        }
        
        public async Task<Guid> Transfer(
            decimal amount,
            string currency,
            string externalId,
            Party recipient,
            string payerMessage,
            string payeeNote,
            Uri callbackUrl = null)
        {
            var referenceId = Guid.NewGuid();
            var transaction = new
            {
                Amount = amount.ToString(CultureInfo.InvariantCulture),
                Currency = currency,
                ExternalId = externalId,
                Payee = recipient,
                PayerMessage = payerMessage,
                PayeeNote = payeeNote,
            };

            var request = Client.Request("/disbursement/v1_0/transfer")
                .WithHeader("X-Reference-Id", referenceId);

            if(callbackUrl != null)
            {
                request.WithHeader("X-Callback-Url", callbackUrl);
            }

            await request.PostJsonAsync(transaction);

            return referenceId;
        }

        public async Task<Disbursement> GetDisbursement(Guid referenceId)
        {
            return await Client.Request($"/disbursement/v1_0/transfer/{referenceId}")
                .GetJsonAsync<Disbursement>();
        }

        public async Task<AccountBalance> GetBalance()
        {
            var response = await Client
                .Request("/disbursement/v1_0/account/balance")
                .GetJsonAsync<AccountBalanceResponse>();

            return new AccountBalance
            {
                AvailableBalance = decimal.Parse(response.AvailableBalance),
                Currency = response.Currency
            };
        }

        public async Task<bool> IsAccountHolderActive(Party party)
        {
            var response = await Client
                .Request(
                    $"/disbursement/v1_0/accountholder/{party.PartyIdType.ToString().ToLowerInvariant()}/{party.PartyId}/active")
                .GetJsonAsync<PayerActiveResponse>();
            
            return response.Result;
        }
    }
}