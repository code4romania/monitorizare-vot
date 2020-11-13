using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class UpdateNgoAdmin : IRequest<Result>
    {
        public NgoAdminModel NgoAdmin { get; }

        public UpdateNgoAdmin(int ngoId, int ngoAdminId, CreateUpdateNgoAdminModel admin)
        {
            NgoAdmin = new NgoAdminModel
            {
                Id = ngoAdminId,
                IdNgo = ngoId,
                Account = admin.Account,
                Password = admin.Password
            };
        }
    }
}