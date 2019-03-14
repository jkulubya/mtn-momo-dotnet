namespace MtnMomo.NET
{
    public class Momo
    {
        private readonly MomoConfig _config = new MomoConfig();
        private readonly HttpClientFactory _clientFactory;
        public Momo()
        {
            _clientFactory = new HttpClientFactory(_config);
        }
        
        public Momo(MomoConfig config): this()
        {
            _config = config;
        }

        public Collections Collections => new Collections(_clientFactory, _config);
        public Disbursements Disbursements => new Disbursements(_clientFactory, _config);
        public Remittances Remittances => new Remittances(_clientFactory, _config);
    }
}