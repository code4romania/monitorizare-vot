using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Queries;

public record GetOptionByIdQuery(int OptionId) : IRequest<OptionDTO>;
