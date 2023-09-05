using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Entities;
using BulkAnswers = VoteMonitor.Api.Answer.Commands.BulkAnswers;

namespace VoteMonitor.Api.Answer.Handlers;

public class AnswerQueryHandler :
    IRequestHandler<BulkAnswers, FillInAnswerCommand>
{
    private readonly VoteMonitorContext _context;

    public AnswerQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<FillInAnswerCommand> Handle(BulkAnswers message, CancellationToken cancellationToken)
    {
        var countyPollingStations = message.Answers
            .Select(a => new { a.PollingStationNumber, a.CountyCode })
            .Distinct()
            .ToList();

        var command = new FillInAnswerCommand { ObserverId = message.ObserverId };


        foreach (var pollingStation in countyPollingStations)
        {
            var pollingStationId = (await _context
                    .PollingStations
                    .FirstAsync(p => p.County.Code == pollingStation.CountyCode && p.Number == pollingStation.PollingStationNumber, cancellationToken: cancellationToken))
                !.Id;

            command.Answers.AddRange(message.Answers
                .Where(a => a.PollingStationNumber == pollingStation.PollingStationNumber && a.CountyCode == pollingStation.CountyCode)
                .Select(a => new AnswerDto
                {
                    QuestionId = a.QuestionId,
                    PollingStationId = pollingStationId,
                    Options = a.Options,
                    PollingStationNumber = a.PollingStationNumber,
                    CountyCode = a.CountyCode
                }));
        }

        return command;
    }
}
