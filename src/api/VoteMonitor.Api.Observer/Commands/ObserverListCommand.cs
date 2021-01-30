using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverListCommand : IRequest<ApiListResponse<ObserverModel>>
    {
        public int NgoId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
