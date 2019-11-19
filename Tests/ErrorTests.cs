using System;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using MtnMomo.NET;
using MtnMomo.NET.Exceptions;
using Tests.Base;
using Xunit;

namespace Tests
{
    public class ErrorTests : BaseTests
    {
        private const string TokenPath = "/collection/token/";


        [Fact]
        public async Task NetworkErrorIsThrownWhenInvalidResponseIsReceived()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest
                    .RespondWithJson(new
                    {
                        access_token = Settings.AccessToken,
                        expires_in = 3600,
                        token_type = "access_token"
                    })
                    .RespondWithJson(new
                    {
                        amount = "xx25000.00xx",
                        currency = "UGX",
                        financialTransactionId ="XXXX",
                        externalid = "YYYY",
                        payer = new
                        {
                            partyIdType =  "MSISDN",
                            partyId = "0777000000"
                        }
                    });

                var config = new MomoConfig
                {
                    UserId = Settings.UserId,
                    UserSecret = Settings.UserSecretKey,
                    SubscriptionKey = Settings.SubscriptionKey
                };

                var collections = new CollectionsClient(config);

                var guid = Guid.NewGuid();

                var result = await Record.ExceptionAsync(async () =>
                {
                    await collections.GetCollection(guid);
                });
                
                Assert.NotNull(result);
                Assert.IsType<NetworkException>(result);
            }
        }
    }
}