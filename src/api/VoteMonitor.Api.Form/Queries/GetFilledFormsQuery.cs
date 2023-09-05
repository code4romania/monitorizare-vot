using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries;

public class GetFilledFormsQuery: IRequest<IReadOnlyList<FilledFormResponseModel>>
{

}