using MediatR;
using VoteMonitor.Api.Form.Models.Options;

namespace VoteMonitor.Api.Form.Commands;

public class AddOptionCommand : IRequest<OptionDTO>
{
    public OptionDTO Option { get; set; }
}