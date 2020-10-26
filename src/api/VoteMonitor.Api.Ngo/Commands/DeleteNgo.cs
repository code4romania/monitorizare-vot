using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class DeleteNgo : IRequest<Result>
    {
        public int Id { get; }

        public DeleteNgo(int id)
        {
            Id = id;
        }
    }
}