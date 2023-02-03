using Microsoft.Extensions.Configuration;
using System.IO;

namespace VotingIrregularities.Domain.Seed
{
    public class Settings
    {
        private static IConfigurationRoot _configuration = null;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var apiFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "VoteMonitor.Api");

                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(apiFolder)
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build();
                }

                return _configuration;
            }
        }
    }
}
