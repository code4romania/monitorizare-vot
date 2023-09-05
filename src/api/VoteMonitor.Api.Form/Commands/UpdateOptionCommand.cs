using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Commands;

public class UpdateOptionCommand : IRequest<int>
{
    public OptionDTO Option { get; set; }
}