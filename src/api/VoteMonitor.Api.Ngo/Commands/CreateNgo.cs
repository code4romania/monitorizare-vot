using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Commands
{
    public class CreateNgo : IRequest<Result>
    {
        public NgoModel Ngo { get; }

        public CreateNgo(CreateUpdateNgoModel ngo)
        {
            Ngo = new NgoModel
            {
                Name = ngo.Name,
                IsActive = ngo.IsActive,
                Organizer = ngo.Organizer,
                ShortName = ngo.ShortName
            };
        }
    }
}