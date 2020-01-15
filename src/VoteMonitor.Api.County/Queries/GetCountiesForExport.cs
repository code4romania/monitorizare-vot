using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries
{
    public class GetCountiesForExport : IRequest<Result<List<CountyCsvModel>>>
    {
    }
}
