using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Handlers
{
    public class FillInAnswerQueryHandler : IRequestHandler<FillInAnswerCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public FillInAnswerQueryHandler(VoteMonitorContext context, IMapper mapper, ILogger<FillInAnswerQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(FillInAnswerCommand message, CancellationToken cancellationToken)
        {
            try
            {
                var lastModified = DateTime.UtcNow;
                var newAnswers = GetFlatListOfAnswers(message, lastModified);

                var pollingStationIds = message.Answers.Select(a => a.PollingStationId).Distinct().ToList();

                using (var tran = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    foreach (var pollingStationId in pollingStationIds)
                    {
                        var questionIds = message.Answers.Select(a => a.QuestionId).Distinct().ToList();

                        var oldAnswersToBeDeleted = _context.Answers
                                .Include(a => a.OptionAnswered)
                                .Where(
                                    a =>
                                        a.IdObserver == message.ObserverId &&
                                        a.IdPollingStation == pollingStationId)
                                .WhereRaspunsContains(questionIds)
                            ;
                        _context.Answers.RemoveRange(oldAnswersToBeDeleted);

                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    await _context.Answers.AddRangeAsync(newAnswers, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken);

                    await tran.CommitAsync(cancellationToken);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(typeof(FillInAnswerCommand).GetHashCode(), ex, ex.Message);
            }

            return await Task.FromResult(-1);
        }

        public static List<Entities.Answer> GetFlatListOfAnswers(FillInAnswerCommand command, DateTime lastModified)
        {
            var list = command.Answers.Select(a => new
            {
                flat = a.Options.Select(o => new Entities.Answer
                {
                    IdObserver = command.ObserverId,
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
                    (g, o) => new Entities.Answer
                    {
                        IdObserver = command.ObserverId,
                        IdPollingStation = o.Last().IdPollingStation,
                        IdOptionToQuestion = g,
                        Value = o.Last().Value,
                        CountyCode = o.Last().CountyCode,
                        PollingStationNumber = o.Last().PollingStationNumber,
                        LastModified = lastModified
                    })
                .Distinct()
                .ToList();

            return list;
        }
    }
}
