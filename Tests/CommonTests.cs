using System;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using MtnMomo.NET;
using Tests.Base;
using Xunit;

namespace Tests
{
    public class CommonTests : BaseTests
    {
        private const string TokenPath = "/collection/token/";

        [Fact]
        public async Task MakingMultipleRequestsDoesNotRequestNewToken()
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
                        amount = "20000.00",
                        currency = "UGX",
                        financialTransactionId ="XXXX",
                        externalid = "YYYY",
                        payer = new
                        {
                            partyIdType =  "MSISDN",
                            partyId = "0777000000"
                        }
                    })
                    .RespondWithJson(new
                    {
                        amount = "30000.00",
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

                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();

                var result1 = await collections.GetTransaction(guid1);
                var result2 = await collections.GetTransaction(guid2);

                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment(TokenPath))
                    .WithHeader("Authorization",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.UserId}:{config.UserSecret}")))
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey);
                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment("/collection/v1_0/requesttopay").AppendPathSegment(guid1))
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
                    .WithHeader("X-Target-Environment", "sandbox");         
                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment("/collection/v1_0/requesttopay").AppendPathSegment(guid2))
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
                    .WithHeader("X-Target-Environment", "sandbox");
                
                Assert.Equal(20000, result1.Amount);
                Assert.Equal(30000, result2.Amount);
            }
        }
    }
}