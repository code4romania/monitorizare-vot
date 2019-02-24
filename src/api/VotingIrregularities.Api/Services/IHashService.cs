namespace VotingIrregularities.Api.Services
{
    public interface IHashService
    {
        string Salt { get; set; }
        string GetHash(string clearString);
    }
}
