using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries
{
    public class GetAllNgoAdmins : IRequest<Result<List<NgoAdminModel>>>
    {
        public int IdNgo { get; }

        public GetAllNgoAdmins(int idNgo)
        {
            IdNgo = idNgo;
        }
    }
}