using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.RaspunsAggregate;

namespace VoteMonitor.Api.Answer.Handlers {
    public class FillInAnswerQueryHandler : AsyncRequestHandler<CompleteazaRaspunsCommand, int> {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public FillInAnswerQueryHandler(VoteMonitorContext context, IMapper mapper, ILogger logger) {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        protected override async Task<int> HandleCore(CompleteazaRaspunsCommand message) {
            try {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var raspunsuriNoi = GetFlatListOfAnswers(message, lastModified);

                // stergerea este pe fiecare sectie
                var sectii = message.Answers.Select(a => a.PollingSectionId).Distinct().ToList();

                using (var tran = await _context.Database.BeginTransactionAsync()) {
                    foreach (var sectie in sectii) {
                        var intrebari = message.Answers.Select(a => a.QuestionId).Distinct().ToList();

                        // delete existing answers for posted questions on this 'sectie'
                        var todelete = _context.Answers
                                .Include(a => a.OptionAnswered)
                                .Where(
                                    a =>
                                        a.IdObserver == message.ObserverId &&
                                        a.IdPollingStation == sectie)
                                //.Where(a => intrebari.Contains(a.OptionAnswered.IdQuestion))
                                .WhereRaspunsContains(intrebari)
                            ;
                        //.Delete();
                        _context.Answers.RemoveRange(todelete);

                        await _context.SaveChangesAsync();
                    }

                    _context.Answers.AddRange(raspunsuriNoi);

                    var result = await _context.SaveChangesAsync();

                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex) {
                _logger.LogError(typeof(CompleteazaRaspunsCommand).GetHashCode(), ex, ex.Message);
            }

            return await Task.FromResult(-1);
        }

        public static List<Entities.Answer> GetFlatListOfAnswers(CompleteazaRaspunsCommand command, DateTime lastModified) {
            var list = command.Answers.Select(a => new
            {
                flat = a.Options.Select(o => new Entities.Answer {
                    IdObserver = command.ObserverId,
                    IdPollingStation = a.PollingSectionId,
                    IdOptionToQuestion = o.OptionId,
                    Value = o.Value,
                    CountyCode = a.CountyCode,
                    PollingStationNumber = a.PollingStationNumber,
                    LastModified = lastModified
                })
            })
                .SelectMany(a => a.flat)
                .GroupBy(k => k.IdOptionToQuestion,
                    (g, o) => new Entities.Answer {
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
