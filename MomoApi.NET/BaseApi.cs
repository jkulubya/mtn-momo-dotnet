using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using MomoApi.NET.ApiResponses;
using MomoApi.NET.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
                settings.BeforeCallAsync = BeforeCallAsync;
                settings.OnErrorAsync = ThrowMomoErrorInstead;
                settings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            });
        }
        
        private async Task<AccessToken> RefreshAccessToken()
        {
            var response = await _config.BaseUri.AppendPathSegment(_tokenPath)
                .WithHeader("Authorization", $"Basic {_config.ClientAuthToken}")
                .PostJsonAsync("")
                .ReceiveJson<GetAccessTokenResponse>();

            var newToken = new AccessToken()
            {
                Expires = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresInSeconds).AddSeconds(-5),
                Token = response.AccessToken
            };

            return newToken;
        }

        private async Task BeforeCallAsync(HttpCall httpCall)
        {
            httpCall.Request.Headers
                .Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                        
            if (Token == null || DateTimeOffset.UtcNow >= Token.Expires)
            {
                var newToken = await RefreshAccessToken();
                Token = newToken;
                httpCall.Request.Headers.Add("Authorization", $"Bearer {Token.Token}");
            }
            else
            {
                httpCall.Request.Headers.Add("Authorization", $"Bearer {Token.Token}");
            }
        }

        private async Task ThrowMomoErrorInstead(HttpCall httpCall)
        {
            var exception = httpCall.Exception;
            
            if(exception is FlurlParsingException parsingException)
                throw new NetworkException(parsingException);

            if (exception is FlurlHttpTimeoutException timeoutException)
                throw new NetworkException(timeoutException);
            
            if (exception is FlurlHttpException httpException)
            {
                ErrorResponse response;

                try
                {
                    response = await httpException.GetResponseJsonAsync<ErrorResponse>();
                }
                catch(Exception e)
                {
                    throw new NetworkException(e);
                }
                
                throw new MomoException(response.Code, response.Message);
            }

            throw new NetworkException(exception);
        }
    }
}