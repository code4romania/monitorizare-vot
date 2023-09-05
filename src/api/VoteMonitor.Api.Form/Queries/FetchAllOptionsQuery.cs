using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Queries;

public class FetchAllOptionsQuery : IRequest<List<OptionDTO>>
{

}