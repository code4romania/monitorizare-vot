using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class UpdateNgo : IRequest<Result>
    {
        public NgoModel Ngo { get; }
        public UpdateNgo(int id, CreateUpdateNgoModel ngo)
        {
            Ngo = new NgoModel
            {
                Id = id,
                Name = ngo.Name,
                IsActive = ngo.IsActive,
                Organizer = ngo.Organizer,
                ShortName = ngo.ShortName
            };
        }
    }
}