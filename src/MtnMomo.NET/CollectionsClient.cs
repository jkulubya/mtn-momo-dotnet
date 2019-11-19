using System;
using System.Globalization;
using System.Threading.Tasks;
using Flurl.Http;
using MtnMomo.NET.ApiResponses;

namespace MtnMomo.NET
{
    public class CollectionsClient : BaseApi
    {
        protected override string TokenPath { get; } = "/collection/token/";
        protected override string SubscriptionKey { get; }

        public CollectionsClient(MomoConfig config) : base(config)
        {
            SubscriptionKey = config.SubscriptionKey;
        }
        
        public async Task<Guid> RequestToPay(
            decimal amount,
            string currency,
            string externalId,
            Party payer,
            string payerMessage,
            string payeeNote,
            Uri callbackUrl = null)
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
            };

            var request = Client.Request("/collection/v1_0/requesttopay")
                .WithHeader("X-Reference-Id", referenceId);

            if(callbackUrl != null)
            {
                request.WithHeader("X-Callback-Url", callbackUrl);
            }

            await request.PostJsonAsync(transaction);

            return referenceId;
        }

        public async Task<Collection> GetCollection(Guid referenceId)
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