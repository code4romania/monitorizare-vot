using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class SetNgoOrganizerFlag : IRequest<Result>
    {
        public int Id { get; }
        public bool IsOrganizer { get; }

        public SetNgoOrganizerFlag(int id, bool isOrganizer)
        {
            Id = id;
            IsOrganizer = isOrganizer;
        }
    }
}