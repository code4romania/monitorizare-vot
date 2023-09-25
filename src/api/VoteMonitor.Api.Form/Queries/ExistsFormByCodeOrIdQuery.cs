using MediatR;

namespace VoteMonitor.Api.Form.Queries;

public record ExistsFormByCodeOrIdQuery(int Id, string Code) : IRequest<bool>;
