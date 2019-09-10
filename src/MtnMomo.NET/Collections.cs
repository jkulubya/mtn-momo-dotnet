using System;
using System.Globalization;
using System.Threading.Tasks;
using Flurl.Http;
using MtnMomo.NET.ApiResponses;

namespace MtnMomo.NET
{
    public class Collections : BaseApi
    {
        protected override string TokenPath { get; } = "/collection/token/";
        protected override string SubscriptionKey { get; }

        internal Collections(HttpClientFactory clientFactory, MomoConfig config) : base(clientFactory, config)
        {
            var key = config?.SubscriptionKeys?.Collections;
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("The collections subscription key cannot be null");
            SubscriptionKey = key;
        }
        
        public async Task<Guid> RequestToPay(
            decimal amount,
            string currency,
            string externalId,
            Party payer,
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
                .WithHeader("X-Callback-Url", callbackUrl)
                .WithHeader("X-Reference-Id", referenceId)
                .PostJsonAsync(transaction);

            return referenceId;
        }

        public async Task<Collection> GetTransaction(Guid referenceId)
        {
            return await Client.Request($"/collection/v1_0/requesttopay/{referenceId}")
                .GetJsonAsync<Collection>();
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

        public async Task<bool> IsAccountHolderActive(Party party)
        {
            var response = await Client
                .Request(
                    $"/collection/v1_0/accountholder/{party.PartyIdType.ToString().ToLowerInvariant()}/{party.PartyId}/active")
                .GetJsonAsync<PayerActiveResponse>();
            
            return response.Result;
        }
    }
}