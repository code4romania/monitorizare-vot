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
using Microsoft.Extensions.PlatformAbstractions;

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
        public void SetPasswords()
        {
            var hashService = new HashService((Microsoft.Extensions.Options.IOptions<VotingIrregularities.Api.Models.HashOptions>)new ConfigureOptions<HashOptions>((options) => Configuration.GetSection("HashOptions").Bind(options)));

            var pathToFile = Directory.GetCurrentDirectory()
               + Path.DirectorySeparatorChar.ToString()
               + "conturi.txt";

            using (var newfile = File.Create("conturi-cu-parole.txt"))
            {
                using (var logWriter = new System.IO.StreamWriter(newfile))
                using (StreamReader reader = File.OpenText(pathToFile))
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
