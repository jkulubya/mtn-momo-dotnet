using System.IO;
using Microsoft.Extensions.Configuration;

namespace Tests.Base
{
    public abstract class BaseTests
    {
        // keys from mtn momo docs
        protected MtnMomoSettings Settings { get;  }

        protected BaseTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            Settings = new MtnMomoSettings();
            config.GetSection("MtnMomoSettings").Bind(Settings);
        }
    }
}