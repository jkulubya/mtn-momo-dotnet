using System;
using System.Threading.Tasks;
using Flurl.Http;

namespace MomoApi.NET
{
    public abstract class BaseApi
    {
        private readonly MomoConfig _config;
        protected readonly FlurlClient Client;
        private AccessToken Token { get; set; }
        protected abstract string _tokenPath { get; }
        protected abstract string _subscriptionKey { get; }
        
        internal BaseApi(HttpClientFactory clientFactory, MomoConfig config)
        {
            _config = config;
            Client = clientFactory.GetClient();

            Client.Configure(settings =>
            {
                settings.BeforeCallAsync = async x =>
                {
                    x.Request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                    if (Token == null || Token.Expires > DateTimeOffset.Now)
                    {
                        var newToken = await RefreshAccessToken();
                        Token = newToken;
                        x.Request.Headers.Add("Authorization", $"Bearer {Token.Token}");
                    }
                    else
                    {
                        x.Request.Headers.Add("Authorization", $"Bearer {Token.Token}");
                    }
                };
            });
        }
        
        private async Task<AccessToken> RefreshAccessToken()
        {
            var response = await Client.Request(_tokenPath)
                .WithHeader("Authorization", $"Basic {_config.ClientAuthToken}")
                .PostJsonAsync("")
                .ReceiveJson();

            var newToken = new AccessToken()
            {
                Expires = DateTimeOffset.UtcNow.AddSeconds((int) response["expires_in"]),
                Token = (string) response["token"]
            };

            return newToken;
        }
    }
}