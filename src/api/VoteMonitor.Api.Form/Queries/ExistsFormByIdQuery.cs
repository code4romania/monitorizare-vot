using MediatR;

namespace VoteMonitor.Api.Form.Queries;

public record GetFormExistsByIdQuery(int Id) : IRequest<bool>;
