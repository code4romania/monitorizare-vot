using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class DeleteNgoAdmin : IRequest<Result>
    {
        public int IdNgo { get; }
        public int IdNgoAdmin { get; }

        public DeleteNgoAdmin(int idNgo, int idNgoAdmin)
        {
            IdNgo = idNgo;
            IdNgoAdmin = idNgoAdmin;
        }
    }
}