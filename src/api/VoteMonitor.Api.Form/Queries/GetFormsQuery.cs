using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries;

public record GetFormsQuery: IRequest<IReadOnlyList<FormResponseModel>>;
