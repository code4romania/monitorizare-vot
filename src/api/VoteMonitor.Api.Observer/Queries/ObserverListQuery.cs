using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Observer.Queries;

public class ObserverListQuery : PagingModel
{
    public string Number { get; set; }
    public string Name { get; set; }
}