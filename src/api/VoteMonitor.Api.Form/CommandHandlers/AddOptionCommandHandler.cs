using MediatR;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers;

public class AddOptionCommandHandler : IRequestHandler<AddOptionCommand, OptionDTO>
{
    private readonly VoteMonitorContext _context;

    public AddOptionCommandHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<OptionDTO> Handle(AddOptionCommand request, CancellationToken cancellationToken)
    {
        var optionEntity = new Option { Text = request.Text, Hint = request.Hint, IsFreeText = request.IsFreeText, };

        _context.Options.Add(optionEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new OptionDTO
        {
            Id = optionEntity.Id,
            Hint = optionEntity.Hint,
            IsFreeText = optionEntity.IsFreeText,
            Text = optionEntity.Text
        };
    }
}
