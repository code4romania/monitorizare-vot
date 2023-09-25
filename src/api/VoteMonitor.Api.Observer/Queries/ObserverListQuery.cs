using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Observer.Queries;

public class ObserverListQuery : PagingModel
{
    [FromQuery] public string Number { get; set; }
    [FromQuery] public string Name { get; set; }
}
