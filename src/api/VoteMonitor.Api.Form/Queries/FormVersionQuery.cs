using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries;

public record FormVersionQuery(bool? Diaspora, bool? Draft) : IRequest<List<FormDetailsModel>>;
