using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class GetCountiesForExport : IRequest<List<CountyCsvModel>>
    {
    }
}
