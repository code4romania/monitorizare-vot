using MediatR;

namespace VotingIrregularities.Domain.FileAggregate
{
    public class AdaugaFileCommand : IAsyncRequest<string>
    {
        public string Url { get; set; }
    }
}
