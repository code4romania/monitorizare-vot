using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class CreateNgoAdmin : IRequest<Result>
    {
        public NgoAdminModel NgoAdmin { get; }
        public CreateNgoAdmin(int idNgo, CreateUpdateNgoAdminModel admin)
        {
            NgoAdmin = new NgoAdminModel
            {
               Account = admin.Account,
               IdNgo = idNgo,
               Password = admin.Password
            };
        }
    }
}