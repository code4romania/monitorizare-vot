using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

namespace VotingIrregularities.Domain.RaspunsAggregate
{
    public class CompleteazaRaspunsHandler : IRequestHandler<CompleteazaRaspunsCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CompleteazaRaspunsHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CompleteazaRaspunsCommand message, CancellationToken cancellationToken)
        {
            try
            {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var raspunsuriNoi = GetFlatListOfAnswers(message, lastModified);

                // stergerea este pe fiecare sectie
                var sectii = message.Raspunsuri.Select(a => a.IdSectie).Distinct().ToList();

                using (var tran = await _context.Database.BeginTransactionAsync())
                {
                    foreach (var sectie in sectii)
                    {
                        var intrebari = message.Raspunsuri.Select(a => a.IdIntrebare).Distinct().ToList();

                        // delete existing answers for posted questions on this 'sectie'
                        var todelete = _context.Answers
                                .Include(a => a.OptionAnswered)
                                .Where(
                                    a =>
                                        a.IdObserver == message.IdObservator &&
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
            catch (Exception ex)
            {
                _logger.LogError(typeof(CompleteazaRaspunsCommand).GetHashCode(), ex, ex.Message);
            }

            return await Task.FromResult(-1);
        }

        public static List<Answer> GetFlatListOfAnswers(CompleteazaRaspunsCommand command, DateTime lastModified)
        {
            var list = command.Raspunsuri.Select(a => new
                {
                    flat = a.Optiuni.Select(o => new Answer
                    {
                        IdObserver = command.IdObservator,
                        IdPollingStation = a.IdSectie,
                        IdOptionToQuestion = o.IdOptiune,
                        Value = o.Value,
                        CountyCode = a.CodJudet,
                        PollingStationNumber = a.NumarSectie,
                        LastModified = lastModified
                    })
                })
                .SelectMany(a => a.flat)
                .GroupBy(k => k.IdOptionToQuestion,
                    (g, o) => new Answer
                    {
                        IdObserver = command.IdObservator,
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