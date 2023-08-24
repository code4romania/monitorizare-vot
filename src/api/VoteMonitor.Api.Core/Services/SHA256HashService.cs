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
        private readonly string _salt;

        public HashService(IOptions<HashOptions> options)
        {
            _salt = options.Value.Salt;
        }

        public string GetHash(string clearString)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(clearString + _salt));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
