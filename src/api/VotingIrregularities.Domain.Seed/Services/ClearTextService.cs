namespace VotingIrregularities.Domain.Seed.Services;

public class ClearTextService : IHashService
{
    public string GetHash(string clearString) => clearString;
}