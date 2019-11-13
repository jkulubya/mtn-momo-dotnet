using System;
using System.Globalization;
using System.Threading.Tasks;
using Flurl.Http;
using MtnMomo.NET.ApiResponses;

namespace MtnMomo.NET
{
    public class RemittancesClient : BaseApi
    {
        protected override string TokenPath { get; } = "/remittance/token/";
        protected override string SubscriptionKey { get; }

        public RemittancesClient(MomoConfig config) : base(config)
        {
            SubscriptionKey = config.SubscriptionKey;
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

            var request = Client.Request("/remittance/v1_0/transfer")
                .WithHeader("X-Reference-Id", referenceId);

            if (callbackUrl != null)
            {
                request.WithHeader("X-Callback-Url", callbackUrl);
            }

            await request.PostJsonAsync(transaction);

            return referenceId;
        }

        public async Task<Remittance> GetRemittance(Guid referenceId)
        {
            return await Client.Request($"/remittance/v1_0/transfer/{referenceId}")
                .GetJsonAsync<Remittance>();
        }

        public async Task<AccountBalance> GetBalance()
        {
            var response = await Client
                .Request("/remittance/v1_0/account/balance")
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
                    $"/remittance/v1_0/accountholder/{party.PartyIdType.ToString().ToLowerInvariant()}/{party.PartyId}/active")
                .GetJsonAsync<PayerActiveResponse>();
            
            return response.Result;
        }
    }
}