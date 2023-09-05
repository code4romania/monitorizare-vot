using MediatR;

namespace VoteMonitor.Api.Form.Queries;

public class ExistsFormByCodeOrIdQuery : IRequest<bool>
{
    public int Id { get; set; }
    public string Code { get; set; }
}