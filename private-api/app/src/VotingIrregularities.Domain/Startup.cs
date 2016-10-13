using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace VotingIrregularities.Domain
{
    public class Startup
    {
        public static IConfigurationRoot RegisterConfiguration()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile("appsettings.development.json", optional: true);

            return builder.Build();
        }
    }
}
