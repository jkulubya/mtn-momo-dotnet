using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using MtnMomo.NET.ApiResponses;
using MtnMomo.NET.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MtnMomo.NET
{
    public abstract class BaseApi
    {
        private readonly MomoConfig _config;
        protected readonly FlurlClient Client;
        private AccessToken Token { get; set; }
        protected abstract string TokenPath { get; }
        protected abstract string SubscriptionKey { get; }
        
        internal BaseApi(MomoConfig config)
        {
            ValidateConfig(config);
            _config = config;
            Client = new HttpClientFactory(_config).GetClient();

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
            var response =  await _config.BaseUri.AppendPathSegment(TokenPath)
                .WithHeader("Authorization", $"Basic {_config.ClientAuthToken}")
                .WithHeader("Ocp-Apim-Subscription-Key", SubscriptionKey)
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
                .Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                        
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
                
                Console.WriteLine(response);
                
                throw new MomoException(response.Code, response.Message);
            }

            throw new NetworkException(exception);
        }

        private void ValidateConfig(MomoConfig config)
        {
            if (string.IsNullOrEmpty(config.SubscriptionKey))
                throw new ArgumentException("The subscription key cannot be null");
            
            if (string.IsNullOrEmpty(config.UserId))
                throw new ArgumentException("The user id cannot be null");
            
            if (string.IsNullOrEmpty(config.UserSecret))
                throw new ArgumentException("The user secret cannot be null");
        }
    }
}