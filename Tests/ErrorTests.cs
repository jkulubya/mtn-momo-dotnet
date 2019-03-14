using System;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using MtnMomo.NET;
using MtnMomo.NET.Exceptions;
using Xunit;

namespace Tests
{
    public class ErrorTests : BaseTestClass
    {
        private const string SubscriptionKey = "d484a1f0d34f4301916d0f2c9e9106a2";
        private const string TokenPath = "/collection/token/";
        
        [Fact]
        public async Task NetworkErrorIsThrownWhenInvalidResponseIsReceived()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest
                    .RespondWithJson(new
                    {
                        access_token = AccessToken,
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
                var config = new MomoConfig();
                config.UserId = UserId;
                config.UserSecret = UserSecretKey;
                config.SubscriptionKeys.Collections = SubscriptionKey;

                var momo = new Momo(config);
                var collections = momo.Collections;

                var guid = Guid.NewGuid();

                var result = await Record.ExceptionAsync(async () =>
                {
                    await collections.GetTransaction(guid);
                });
                
                Assert.NotNull(result);
                Assert.IsType<NetworkException>(result);
            }
        }
    }
}