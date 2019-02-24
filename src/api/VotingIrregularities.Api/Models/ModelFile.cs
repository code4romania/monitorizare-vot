using MediatR;
using Microsoft.AspNetCore.Http;

namespace VotingIrregularities.Api.Models
{
    public class ModelFile : IRequest<string>
    {
        public IFormFile File { get; set; }
    }
}
