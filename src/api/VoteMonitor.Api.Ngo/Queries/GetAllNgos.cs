using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries
{
    public class GetAllNgos : IRequest<Result<List<NgoModel>>>
    {

    }
}