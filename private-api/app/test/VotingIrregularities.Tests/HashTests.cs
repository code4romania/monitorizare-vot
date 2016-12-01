using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;
using Xunit;

namespace VotingIrregularities.Tests
{
    public class HashTests
    {
        public HashTests()
        {

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.hash.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        [Fact]
        public void HashSizeSmallerThan100()
        {
            // Arrange
            var hashService = new HashService(new OptionsManager<HashOptions>(new List<IConfigureOptions<HashOptions>>
                {
                    new ConfigureFromConfigurationOptions<HashOptions>(
                        Configuration.GetSection("HashOptions"))
                }));

            // Act
            var hash = hashService.GetHash("123456");

            // Assert
            Assert.True(hash.Length <= 100);
            Assert.Equal("dda90869c6fa8be5fde0991da5b37d48c0dd31b8fc63f13f62c2b45ad7555780", hash);
        }
    }
}
