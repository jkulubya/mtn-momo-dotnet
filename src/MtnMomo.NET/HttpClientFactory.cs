using Flurl.Http;

namespace MtnMomo.NET
{
    public class HttpClientFactory
    {
        private readonly MomoConfig _config;

        public HttpClientFactory(MomoConfig config)
        {
            _config = config;
        }
        
        public FlurlClient GetClient()
        {
            return new FlurlClient(_config.BaseUri).WithHeaders(new
            {
                X_Target_Environment = _config.Environment == MomoEnvironment.Sandbox ? "sandbox" : "production"
            });
        }
    }
}