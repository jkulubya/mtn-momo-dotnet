using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using MomoApi.NET;
using Xunit;

namespace Tests
{
    public class RemittancesTests : BaseTestClass
    
    {
        private const string SubscriptionKey = "d484a1f0d34f4301916d0f2c9e9106a2";
        private const string TokenPath = "/remittance/token/";

        [Fact]
        public void ThrowsOnInvalidConfig()
        {
            var momo = new Momo(new MomoConfig());
            var result = Record.Exception(() =>
            {
                var remittances = momo.Remittances;
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
                config.SubscriptionKeys.Remittances = SubscriptionKey;
                
                var momo = new Momo(config);
                var remittances = momo.Remittances;

                var result = await remittances.GetBalance();

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment("/remittance/v1_0/account/balance"))
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
                config.SubscriptionKeys.Remittances = SubscriptionKey;
                
                var momo = new Momo(config);
                var remittances = momo.Remittances;

                var result = await remittances.IsAccountHolderActive(new Party("0777000000", PartyIdType.Msisdn));

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        BaseUri.AppendPathSegment($"/remittance/v1_0/accountholder/msisdn/0777000000/active"))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
                    .WithHeader("X-Target-Environment", "sandbox");
                Assert.True(result);
            }
        }

        [Fact]
        public async Task TransferSendsProperRequest()
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
                config.SubscriptionKeys.Remittances = SubscriptionKey;
                
                var momo = new Momo(config);
                var remittances = momo.Remittances;

                var result = await remittances.Transfer(
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
                        BaseUri.AppendPathSegment("/remittance/v1_0/transfer"))
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
                        payee = new
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
        public async Task GetRemittanceSendsProperRequest()
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
                        payee = new
                        {
                            partyIdType =  "MSISDN",
                            partyId = "0777000000"
                        }
                    });

                var config = new MomoConfig();
                config.UserId = UserId;
                config.UserSecret = UserSecretKey;
                config.SubscriptionKeys.Remittances = SubscriptionKey;

                var momo = new Momo(config);
                var remittances = momo.Remittances;

                var guid = Guid.NewGuid();
                
                var result = await remittances.GetRemittance(guid);

                httpTest.ShouldHaveCalled(BaseUri.AppendPathSegment(TokenPath))
                    .WithVerb(HttpMethod.Post);
                httpTest.ShouldHaveCalled(
                        BaseUri.AppendPathSegment("/remittance/v1_0/transfer").AppendPathSegment(guid))
                    .WithVerb(HttpMethod.Get)
                    .WithHeader("Authorization", $"Bearer {AccessToken}")
                    .WithHeader("X-Target-Environment", "sandbox")
                    .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
                
                Assert.Equal(25000M, result.Amount);
                Assert.Equal("UGX", result.Currency);
                Assert.Equal("XXXX", result.FinancialTransactionId);
                Assert.Equal("YYYY", result.ExternalId);
                Assert.NotNull(result.Payee);
                Assert.Equal(PartyIdType.Msisdn, result.Payee.PartyIdType);
                Assert.Equal("0777000000", result.Payee.PartyId);
            }
        }
    }
}