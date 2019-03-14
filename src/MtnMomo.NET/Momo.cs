namespace MtnMomo.NET
{
    public class Momo
    {
        private readonly MomoConfig _config;
        private readonly HttpClientFactory _clientFactory;
        public Momo()
        {
            _config = new MomoConfig();
            _clientFactory = new HttpClientFactory(_config);
        }
        
        public Momo(MomoConfig config)
        {
            _config = config;
            _clientFactory = new HttpClientFactory(_config);
        }

        public Collections Collections => new Collections(_clientFactory, _config);
        public Disbursements Disbursements => new Disbursements(_clientFactory, _config);
        public Remittances Remittances => new Remittances(_clientFactory, _config);
    }
}