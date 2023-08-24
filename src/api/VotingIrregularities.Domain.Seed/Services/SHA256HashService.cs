using System;
using System.Security.Cryptography;
using System.Text;

namespace VotingIrregularities.Domain.Seed.Services
{
    public class SHA256HashService : IHashService
    {
        private readonly string _salt;

        public SHA256HashService(string salt)
        {
            _salt = salt;
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
