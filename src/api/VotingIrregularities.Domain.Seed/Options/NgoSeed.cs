using System.Collections.Generic;

namespace VotingIrregularities.Domain.Seed.Options
{
    public class NgoSeed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOrganizer { get; set; }

        public Dictionary<string, AdminSeed> Admins { get; set; } = new Dictionary<string, AdminSeed>();
        public Dictionary<string, ObserverSeed> Observers { get; set; } = new Dictionary<string, ObserverSeed>();
    }
}
