namespace VotingIrregularities.Domain.Seed.Services;

public interface IHashService
{
    string GetHash(string clearString);
}