using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries
{
    public class GetNgoDetails:IRequest<Result<NgoModel>>
    {
        public int NgoId { get; }

        public GetNgoDetails(int ngoId)
        {
            NgoId = ngoId;
        }
    }
}