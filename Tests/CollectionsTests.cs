using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using MtnMomo.NET;
using Tests.Base;
using Xunit;

namespace Tests
{
    public class CollectionsTests : BaseTests
    {
        private const string TokenPath = "/collection/token/";
        [Fact]
        public void ThrowsOnInvalidConfig()
        {
            var result = Record.Exception(() =>
            {
                var _ = new CollectionsClient(new MomoConfig());
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
                        access_token = Settings.AccessToken,
                        expires_in = 3600,
                        token_type = "access_token"
                    })
                    .RespondWithJson(new
                    {
                        availableBalance = "20000",
                        currency = "UGX"
                    });
                var config = new MomoConfig
                {
                    UserId = Settings.UserId,
                    UserSecret = Settings.UserSecretKey,
                    SubscriptionKey = Settings.SubscriptionKey
                };

                var collections = new CollectionsClient(config);

                var result = await collections.GetBalance();

                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment("/collection/v1_0/account/balance"))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
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
                        access_token = Settings.AccessToken,
                        expires_in = 3600,
                        token_type = "access_token"
                    })
                    .RespondWithJson(new {result = true});
                var config = new MomoConfig
                {
                    UserId = Settings.UserId,
                    UserSecret = Settings.UserSecretKey,
                    SubscriptionKey = Settings.SubscriptionKey
                };

                var collections = new CollectionsClient(config);

                var result = await collections.IsAccountHolderActive(new Party("0777000000", PartyIdType.Msisdn));

                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        Settings.BaseUri.AppendPathSegment($"/collection/v1_0/accountholder/msisdn/0777000000/active"))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
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
                        access_token = Settings.AccessToken,
                        expires_in = 3600,
                        token_type = "access_token"
                    })
                    .RespondWith(status: 202)
                    .RespondWith(status: 202);

                var config = new MomoConfig
                {
                    UserId = Settings.UserId,
                    UserSecret = Settings.UserSecretKey,
                    SubscriptionKey = Settings.SubscriptionKey
                };

                var collections = new CollectionsClient(config);

                var resultWithCallback = await collections.RequestToPay(
                    25000.00M,
                    "UGX",
                    "XX",
                    new Party("0777000000", PartyIdType.Msisdn),
                    "YY",
                    "ZZ",
                    new Uri("http://www.example.com")
                );

                var resultWithoutCallback = await collections.RequestToPay(
                    5000.00M,
                    "UGX",
                    "XX",
                    new Party("0777000001", PartyIdType.Msisdn),
                    "MM",
                    "DD"
                );

                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        Settings.BaseUri.AppendPathSegment("/collection/v1_0/requesttopay"))
                    .WithVerb(HttpMethod.Post)
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("X-Reference-Id", resultWithCallback.ToString())
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithHeader("X-Callback-Url", "http://www.example.com")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
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
                        payeeNote = "ZZ"
                    });
                httpTest.ShouldHaveCalled(
                        Settings.BaseUri.AppendPathSegment("/collection/v1_0/requesttopay"))
                    .WithVerb(HttpMethod.Post)
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("X-Reference-Id", resultWithoutCallback.ToString())
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithoutHeader("X-Callback-Url")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey)
                    .WithRequestJson(new
                    {
                        amount = "5000.00",
                        currency = "UGX",
                        externalId = "XX",
                        payer = new
                        {
                            partyIdType = "MSISDN",
                            partyId = "0777000001"
                        },
                        payerMessage = "MM",
                        payeeNote = "DD"
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
                        access_token = Settings.AccessToken,
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

                var config = new MomoConfig
                {
                    UserId = Settings.UserId,
                    UserSecret = Settings.UserSecretKey,
                    SubscriptionKey = Settings.SubscriptionKey
                };

                var collections = new CollectionsClient(config);

                var guid = Guid.NewGuid();
                
                var result = await collections.GetCollection(guid);
                
                httpTest.ShouldHaveCalled(Settings.BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        Settings.BaseUri.AppendPathSegment("/collection/v1_0/requesttopay").AppendPathSegment(guid))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {Settings.AccessToken}")
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithHeader("Ocp-Apim-Subscription-Key", Settings.SubscriptionKey);
                
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