using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Handlers;

public class FillInAnswerQueryHandler : IRequestHandler<FillInAnswerCommand, int>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public FillInAnswerQueryHandler(VoteMonitorContext context, ILogger<FillInAnswerQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(FillInAnswerCommand message, CancellationToken cancellationToken)
    {
        try
        {
            var lastModified = DateTime.UtcNow;


            using (var tran = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                await WriteAnswers(message.ObserverId, message.Answers, lastModified, cancellationToken);
                await WriteCorruptedAnswers(message.ObserverId, message.CorruptedAnswers, lastModified, cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken);

                await tran.CommitAsync(cancellationToken);

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to save in db answers @{message}", message);
        }

        return await Task.FromResult(-1);
    }

    private async Task WriteAnswers(int observerId, IReadOnlyCollection<AnswerDto> answers, DateTime lastModified, CancellationToken cancellationToken)
    {
        var newAnswers = GetFlatListOfAnswers(observerId, answers, lastModified);

        var pollingStationIds = answers.Select(a => a.PollingStationId).Distinct().ToList();

        foreach (var pollingStationId in pollingStationIds)
        {
            var questionIds = answers.Select(a => a.QuestionId).Distinct().ToList();

            var oldAnswersToBeDeleted = _context.Answers
                    .Include(a => a.OptionAnswered)
                    .Where(
                        a =>
                            a.IdObserver == observerId &&
                            a.IdPollingStation == pollingStationId)
                    .WhereRaspunsContains(questionIds)
                ;
            _context.Answers.RemoveRange(oldAnswersToBeDeleted);

            await _context.SaveChangesAsync(cancellationToken);
        }

        await _context.Answers.AddRangeAsync(newAnswers, cancellationToken);
    }
    private async Task WriteCorruptedAnswers(int observerId, IReadOnlyCollection<CorruptedAnswerDto> corruptedAnswers, DateTime lastModified, CancellationToken cancellationToken)
    {
        var newCorruptedAnswers = GetFlatListOfCorruptedAnswers(observerId, corruptedAnswers, lastModified);
        var pollingStationIds = corruptedAnswers.Select(a => (a.CountyCode, a.MunicipalityCode, a.PollingStationNumber)).Distinct().ToList();

        foreach (var pollingStationId in pollingStationIds)
        {
            var questionIds = corruptedAnswers.Select(a => a.QuestionId).Distinct().ToList();

            var oldAnswersToBeDeleted = _context.CorruptedAnswers
                    .Include(a => a.OptionAnswered)
                    .Where(
                        a =>
                            a.IdObserver == observerId &&
                            a.CountyCode == pollingStationId.CountyCode &&
                            a.MunicipalityCode == pollingStationId.MunicipalityCode &&
                            a.PollingStationNumber == pollingStationId.PollingStationNumber)
                    .WhereRaspunsContains(questionIds);

            _context.CorruptedAnswers.RemoveRange(oldAnswersToBeDeleted);

            await _context.SaveChangesAsync(cancellationToken);
        }

        await _context.CorruptedAnswers.AddRangeAsync(newCorruptedAnswers, cancellationToken);
    }

    public static List<Entities.Answer> GetFlatListOfAnswers(int observerId, IReadOnlyCollection<AnswerDto> answers, DateTime lastModified)
    {
        var list = answers.Select(a => new
        {
            flat = a.Options.Select(o => new Entities.Answer
            {
                IdObserver = observerId,
                IdPollingStation = a.PollingStationId,
                IdOptionToQuestion = o.OptionId,
                Value = o.Value,
                CountyCode = a.CountyCode,
                PollingStationNumber = a.PollingStationNumber,
                LastModified = lastModified
            })
        })
            .SelectMany(a => a.flat)
            .GroupBy(k => k.IdOptionToQuestion,
                (g, o) =>
                {
                    var enumerable = o as Entities.Answer[] ?? o.ToArray();

                    return new Entities.Answer
                    {
                        IdObserver = observerId,
                        IdPollingStation = enumerable.Last().IdPollingStation,
                        IdOptionToQuestion = g,
                        Value = enumerable.Last().Value,
                        CountyCode = enumerable.Last().CountyCode,
                        PollingStationNumber = enumerable.Last().PollingStationNumber,
                        LastModified = lastModified
                    };
                })
            .Distinct()
            .ToList();

        return list;
    }

    public static List<Entities.AnswerCorrupted> GetFlatListOfCorruptedAnswers(int observerId, IReadOnlyCollection<CorruptedAnswerDto> corruptedAnswers, DateTime lastModified)
    {
        var list = corruptedAnswers.Select(a => new
        {
            flat = a.Options.Select(o => new Entities.AnswerCorrupted()
            {
                IdObserver = observerId,
                IdOptionToQuestion = o.OptionId,
                Value = o.Value,
                CountyCode = a.CountyCode,
                MunicipalityCode = a.MunicipalityCode,
                PollingStationNumber = a.PollingStationNumber,
                LastModified = lastModified
            })
        })
            .SelectMany(a => a.flat)
            .GroupBy(k => k.IdOptionToQuestion,
                (g, o) =>
                {
                    var enumerable = o as Entities.AnswerCorrupted[] ?? o.ToArray();

                    return new Entities.AnswerCorrupted
                    {
                        IdObserver = observerId,
                        IdOptionToQuestion = g,
                        Value = enumerable.Last().Value,
                        CountyCode = enumerable.Last().CountyCode,
                        MunicipalityCode = enumerable.Last().MunicipalityCode,
                        PollingStationNumber = enumerable.Last().PollingStationNumber,
                        LastModified = lastModified
                    };
                })
            .Distinct()
            .ToList();

        return list;
    }
}
