using System;
using System.Text;

namespace MtnMomo.NET
{
    public class MomoConfig
    {
        private string _baseUri = new Uri("https://ericssonbasicapi2.azure-api.net").ToString();

        public string BaseUri
        {
            get => _baseUri;
            set
            {
                var item = new Uri(value);
                _baseUri = item.ToString();
            }
        }

        public string UserId { get; set; }
        public string UserSecret { get; set; }
        public SubscriptionKeys SubscriptionKeys { get; set; } = new SubscriptionKeys();

        internal string ClientAuthToken
        {
            get
            {
                if (UserId == null) throw new Exception("User Id not set");
                if (UserSecret == null) throw new Exception("User secret not set");

                return System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserId}:{UserSecret}"));
            }
        }

        public MomoEnvironment Environment { get; set; } = MomoEnvironment.Sandbox;
    }
}