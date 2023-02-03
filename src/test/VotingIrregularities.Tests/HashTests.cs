using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.HashingService;
using Xunit;

namespace VotingIrregularities.Tests
{
    public class HashTests
    {
        public HashTests()
        {

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", true, true)
               .AddJsonFile("appsettings.hash.json", true, true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; set; }

     
#pragma warning disable xUnit1004 // Test methods should not be skipped
        [Fact(Skip = "Used for generating the password files")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
        public void SetPasswords()
        {
            var hashOptions = new HashOptions();
            Configuration.GetSection("HashOptions").Bind(hashOptions);

            var optionsList = Options.Create(hashOptions);

            var hashService = new HashService(optionsList);

            var pathToFile = Directory.GetCurrentDirectory()
               + Path.DirectorySeparatorChar
               + "conturi.txt";

            using var newfile = File.Create("conturi-cu-parole.txt");
            using var logWriter = new StreamWriter(newfile);
            using var reader = File.OpenText(pathToFile);
            while (reader.Peek() >= 0)
            {
                var fileContent = reader.ReadLine();

                var data = fileContent.Split('\t');
                var hashed = hashService.GetHash(data[1]);

                logWriter.WriteLine(fileContent + '\t' + hashed);
            }
        }
        }
    }
