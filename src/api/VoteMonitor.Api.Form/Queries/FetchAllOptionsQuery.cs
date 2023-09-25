using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Queries;

public record FetchAllOptionsQuery : IRequest<List<OptionDTO>>;
