using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services
{
    /// <inheritdoc />
    public class HashService : IHashService
    {
        public HashService(IOptions<HashOptions> options)
        {
            Salt = options.Value.Salt;
        }

        public string Salt { get; set; }
        public string GetHash(string clearString)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(clearString + Salt));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
