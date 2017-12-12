using System.IO;
using Microsoft.Extensions.Configuration;
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
               .AddJsonFile("appsettings.json", true, true)
               .AddJsonFile("appsettings.hash.json", true, true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; set; }

     
        [Fact(Skip = "Used for generating the password files")]
        public void SetPasswords()
        {
            var optionsList = (IOptions<HashOptions>) new ConfigureOptions<HashOptions>(options =>
                Configuration.GetSection("HashOptions").Bind(options));

            var hashService = new HashService(optionsList);

            var pathToFile = Directory.GetCurrentDirectory()
               + Path.DirectorySeparatorChar
               + "conturi.txt";

            using (var newfile = File.Create("conturi-cu-parole.txt"))
            {
                using (var logWriter = new StreamWriter(newfile))
                using (var reader = File.OpenText(pathToFile))
                {
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
        }
    }
