using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Queries;

public class GetOptionByIdQuery : IRequest<OptionDTO>
{
    public int OptionId { get; }

    public GetOptionByIdQuery(int optionId)
    {
        OptionId = optionId;
    }
}