using Microsoft.Extensions.Configuration;
using System.IO;

namespace VotingIrregularities.Domain.Seed
{
    public class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
