using System.Collections.Generic;

namespace VotingIrregularities.Domain.Seed.Options
{
    public class SeedOption
    {
        public const string SectionKey = "SeedOptions";

        /// <summary>
        /// A flag to specify if Seed app should override existing data in db
        /// </summary>
        public bool OverrideExistingData { get; set; }

        public string PasswordSalt { get; set; }

        /// <summary>
        /// Can be set to `Hash` or `ClearText`
        /// `Hash` will use the HashService (that needs the Salt setting) to generate hashes for the password
        /// :warning: `ClearText` will allow your development environment to create and store clear text passwords in the database. Please only use this in development to speed up things.
        /// </summary>
        public HashServiceType HashServiceType { get; set; } = HashServiceType.ClearText;


        /// <summary>
        /// A dictionary containing NGOs to be seeded. Dictionary key is the ngo id
        /// </summary>
        public Dictionary<string, NgoSeed> Ngos { get; set; } = new Dictionary<string, NgoSeed>();
    }
}
