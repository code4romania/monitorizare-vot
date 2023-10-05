using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;
using BulkAnswers = VoteMonitor.Api.Answer.Commands.BulkAnswers;

namespace VoteMonitor.Api.Answer.Handlers;

public class AnswerQueryHandler :
    IRequestHandler<BulkAnswers, FillInAnswerCommand>
{
    private readonly IPollingStationService _pollingPollingStationService;
    private readonly VoteMonitorContext _context;

    public AnswerQueryHandler(IPollingStationService pollingPollingStationService, VoteMonitorContext context)
    {
        _pollingPollingStationService = pollingPollingStationService;
        _context = context;
    }

    public async Task<FillInAnswerCommand> Handle(BulkAnswers message, CancellationToken cancellationToken)
    {
        var answersBuilder = new List<AnswerDto>();

        foreach (var answer in message.Answers)
        {
            var pollingStationId = await _pollingPollingStationService.GetPollingStationId(answer.CountyCode, answer.MunicipalityCode, answer.PollingStationNumber);
            if (pollingStationId != -1)
            {
                answersBuilder.Add(new AnswerDto
                {
                    QuestionId = answer.QuestionId,
                    PollingStationId = pollingStationId,
                    Options = answer.Options,
                    PollingStationNumber = answer.PollingStationNumber,
                    CountyCode = answer.CountyCode,
                });
            }
        }
        var command = new FillInAnswerCommand(message.ObserverId, answersBuilder);
        return command;
    }
}
