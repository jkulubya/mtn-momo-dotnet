using Flurl.Http;

namespace MtnMomo.NET
{
    internal class HttpClientFactory
    {
        private readonly MomoConfig _config;

        internal HttpClientFactory(MomoConfig config)
        {
            _config = config;
        }
        
        internal FlurlClient GetClient()
        {
            return new FlurlClient(_config.BaseUri).WithHeaders(new
            {
                X_Target_Environment = _config.Environment == MomoEnvironment.Sandbox ? "sandbox" : "production"
            });
        }
    }
}