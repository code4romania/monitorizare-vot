using MediatR;
using Microsoft.AspNetCore.Http;
using VotingIrregularities.Domain.FileAggregate;

namespace VotingIrregularities.Api.Models
{
    public class ModelFile : IAsyncRequest<AdaugaFileCommand>
    {
        public IFormFile File { get; set; }
    }
}
