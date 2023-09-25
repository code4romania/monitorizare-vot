namespace VotingIrregularities.Domain.Seed.Options;

public class NgoSeed
{
    public string ShortName { get; set; }
    public string Name { get; set; }
    public bool IsOrganizer { get; set; }

    /// <summary>
    /// A dictionary containing admins to be seeded. Dictionary key is the admin id
    /// </summary>
    public Dictionary<string, AdminSeed> Admins { get; set; } = new Dictionary<string, AdminSeed>();

    /// <summary>
    /// A dictionary containing observers to be seeded. Dictionary key is the observer id
    /// </summary>
    public Dictionary<string, ObserverSeed> Observers { get; set; } = new Dictionary<string, ObserverSeed>();
}