using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class UpdateOptionCommand : IRequest<int>
    {
        public OptionDto Option { get; }

        public UpdateOptionCommand(OptionDto option)
        {
            Option = option;
        }
    }
}