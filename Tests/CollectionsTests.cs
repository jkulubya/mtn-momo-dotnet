using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using Xunit;
using MomoApi.NET;

namespace Tests
{
    public class CollectionsTests : BaseTestClass
    {
        private const string SubscriptionKey = "d484a1f0d34f4301916d0f2c9e9106a2";
        private const string TokenPath = "/collection/token/";

        [Fact]
        public void ThrowsOnInvalidConfig()
        {
            var momo = new Momo(new MomoConfig());
            var result = Record.Exception(() =>
            {
                var collections = momo.Collections;
            });

            Assert.NotNull(result);
            Assert.IsType<ArgumentException>(result);
        }

        [Fact]
        public async Task GetBalanceSendsProperRequest()
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
                        availableBalance = "20000",
                        currency = "UGX"
                    });
                var config = new MomoConfig();
                config.UserId = UserId;
                config.UserSecret = UserSecretKey;
                config.SubscriptionKeys.Collections = SubscriptionKey;
                
                var momo = new Momo(config);
                var collections = momo.Collections;

                var result = await collections.GetBalance();

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment("/collection/v1_0/account/balance"))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
                    .WithHeader("X-Target-Environment", "sandbox");
                
                Assert.Equal(20000, result.AvailableBalance);
                Assert.Equal("UGX", result.Currency);
            }
        }

        [Fact]
        public async Task CheckIfPayerIsActiveSendsProperRequest()
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
                    .RespondWithJson(new {result = true});
                var config = new MomoConfig();
                config.UserId = UserId;
                config.UserSecret = UserSecretKey;
                config.SubscriptionKeys.Collections = SubscriptionKey;
                
                var momo = new Momo(config);
                var collections = momo.Collections;

                var result = await collections.IsAccountHolderActive(new Party("0777000000", PartyIdType.Msisdn));

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        BaseUri.AppendPathSegment($"/collection/v1_0/accountholder/msisdn/0777000000/active"))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
                    .WithHeader("X-Target-Environment", "sandbox");
                Assert.True(result);
            }
        }

        [Fact]
        public async Task RequestToPaySendsProperRequest()
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
                        amount= "25000",
                        currency = "UGX"
                    }, 202);
                
                var config = new MomoConfig();
                config.UserId = UserId;
                config.UserSecret = UserSecretKey;
                config.SubscriptionKeys.Collections = SubscriptionKey;
                
                var momo  =  new Momo(config);
                var collections = momo.Collections;

                var result = await collections.RequestToPay(
                    25000.00M,
                    "UGX",
                    "XX",
                    new Party("0777000000", PartyIdType.Msisdn),
                    "YY",
                    "ZZ",
                    new Uri("http://www.example.com")
                );

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        BaseUri.AppendPathSegment("/collection/v1_0/requesttopay"))
                    .WithVerb(HttpMethod.Post)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("X-Reference-Id", result.ToString())
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
                    .WithRequestJson(new
                    {
                        amount = "25000.00",
                        currency = "UGX",
                        externalId = "XX",
                        payer = new
                        {
                            partyIdType = "MSISDN",
                            partyId = "0777000000"
                        },
                        payerMessage = "YY",
                        payeeNote = "ZZ",
                        callbackUrl = "http://www.example.com/"
                    });
            }
        }

        [Fact]
        public async Task GetTransactionSendsProperRequest()
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
                        amount = "25000.00",
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
                
                var result = await collections.GetTransaction(guid);
                
                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        BaseUri.AppendPathSegment("/collection/v1_0/requesttopay").AppendPathSegment(guid))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
                
                Assert.Equal(25000M, result.Amount);
                Assert.Equal("UGX", result.Currency);
                Assert.Equal("XXXX", result.FinancialTransactionId);
                Assert.Equal("YYYY", result.ExternalId);
                Assert.NotNull(result.Payer);
                Assert.Equal(PartyIdType.Msisdn, result.Payer.PartyIdType);
                Assert.Equal("0777000000", result.Payer.PartyId);
            }
        }
    }
}