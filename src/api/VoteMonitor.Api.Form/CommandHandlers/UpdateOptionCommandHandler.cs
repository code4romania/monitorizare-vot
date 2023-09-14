using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers;

public class UpdateOptionCommandHandler : IRequestHandler<UpdateOptionCommand, int>
{
    private readonly VoteMonitorContext _context;

    public UpdateOptionCommandHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _context.Options
            .FirstOrDefaultAsync(a => a.Id == request.OptionId, cancellationToken);

        if (option == null)
        {
            throw new ArgumentException($"Can't find this option by id = {request.OptionId}");
        }

        option.IsFreeText = request.IsFreeText;
        option.Text = request.Text;
        option.Hint = request.Hint;
        
        _context.Update(option);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
