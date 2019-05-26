using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;
using Z.EntityFramework.Plus;

namespace VotingIrregularities.Domain.RaspunsAggregate
{
    public class CompleteazaRaspunsHandler : AsyncRequestHandler<CompleteazaRaspunsCommand, int>
    {
        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CompleteazaRaspunsHandler(VotingContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        protected override async Task<int> HandleCore(CompleteazaRaspunsCommand message)
        {
            try
            {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var raspunsuriNoi = message.Raspunsuri.Select(a => new
                {
                    flat = a.Optiuni.Select(o => new Answer
                    {
                        IdObserver = message.IdObservator,
                        IdPollingStation = a.IdSectie,
                        IdOptionToQuestion = o.IdOptiune,
                        Value = o.Value,
                        CountyCode = a.CodJudet,
                        PollingStationNumber = a.NumarSectie,
                        LastModified = lastModified
                    })
                }).SelectMany(a => a.flat)
                .Distinct()
                .ToList();

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

                    var result =  await _context.SaveChangesAsync();

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
    }

    public static class EfBuilderExtensions
    {
        /// <summary>
        /// super simple and dumb translation of .Contains because is not supported pe EF plus
        /// this translates to contains in EF SQL
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static IQueryable<Answer> WhereRaspunsContains(this IQueryable<Answer> source, IList<int> contains)
        {
            var ors = contains
                .Aggregate<int, Expression<Func<Answer, bool>>>(null, (expression, id) => 
                expression == null 
                    ? (a => a.OptionAnswered.IdQuestion == id) 
                    : expression.Or(a => a.OptionAnswered.IdQuestion == id));

            return source.Where(ors);
        }
    }
}
