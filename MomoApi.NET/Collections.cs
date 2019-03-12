using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl.Http;
using MomoApi.NET.ApiResponses;

namespace MomoApi.NET
{
    public class Collections : BaseApi
    {
        protected override string _tokenPath { get; } = "/collection/token/";
        protected override string _subscriptionKey { get; }

        internal Collections(HttpClientFactory clientFactory, MomoConfig config) : base(clientFactory, config)
        {
            var key = config?.SubscriptionKeys?.Collections;
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("The collections subscription key cannot be null");
            _subscriptionKey = key;
        }
        
        public async Task<Guid> RequestToPay(
            decimal amount,
            string currency,
            string externalId,
            Payer payer,
            string payerMessage,
            string payeeNote,
            Uri callbackUrl
            )
        {
            
            // TODO validate inputs
            var referenceId = Guid.NewGuid();
            var transaction = new
            {
                Amount = amount.ToString(CultureInfo.InvariantCulture),
                Currency = currency,
                ExternalId = externalId,
                Payer = payer,
                PayerMessage = payerMessage,
                PayeeNote = payeeNote,
                CallbackUrl = callbackUrl.ToString()
            };

            await Client.Request("/collection/v1_0/requesttopay")
                    .WithHeader("X-Reference-Id", referenceId)
                    .PostJsonAsync(transaction);

            return referenceId;
        }

        public async Task<Transaction> GetTransaction(Guid referenceId)
        {
            return await Client.Request($"/collection/v1_0/requesttopay/{referenceId}")
                .GetJsonAsync<Transaction>();
        }

        public async Task<AccountBalance> GetBalance()
        {
            var response = await Client
                .Request("/collection/v1_0/account/balance")
                .GetJsonAsync<AccountBalanceResponse>();

            return new AccountBalance
            {
                AvailableBalance = decimal.Parse(response.AvailableBalance),
                Currency = response.Currency
            };
        }

        public async Task<bool> IsPayerActive(Payer payer)
        {
            var response = await Client
                .Request(
                    $"/collection/v1_0/accountholder/{payer.PartyIdType.ToString().ToLowerInvariant()}/{payer.PartyId}/active")
                .GetJsonAsync<PayerActiveResponse>();
            
            return response.Result;
        }
    }
}