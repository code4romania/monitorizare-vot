using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class SetNgoStatusFlag : IRequest<Result>
    {
        public int Id { get; }
        public bool IsActive { get; }

        public SetNgoStatusFlag(int id, bool isActive)
        {
            Id = id;
            IsActive = isActive;
        }
    }
}
