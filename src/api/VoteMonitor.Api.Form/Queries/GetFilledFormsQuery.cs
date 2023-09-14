using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries;

public record GetFilledFormsQuery: IRequest<IReadOnlyList<FilledFormResponseModel>>;
