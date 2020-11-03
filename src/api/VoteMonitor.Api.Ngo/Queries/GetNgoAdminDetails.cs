using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries
{
    public class GetNgoAdminDetails : IRequest<Result<NgoAdminModel>>
    {
        public int NgoId { get; }
        public int AdminId { get; }


        public GetNgoAdminDetails(int ngoId, int adminId)
        {
            NgoId = ngoId;
            AdminId = adminId;
        }
    }
}