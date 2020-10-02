using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class GetOptionByIdCommand : IRequest<OptionDto>
    {
        public int OptionId { get; }

        public GetOptionByIdCommand(int optionId)
        {
            OptionId = optionId;
        }
    }
}