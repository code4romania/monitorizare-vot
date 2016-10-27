using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;
using Z.EntityFramework.Plus;

namespace VotingIrregularities.Domain.RaspunsAggregate
{
    public class CompleteazaRaspunsHandler : IAsyncRequestHandler<CompleteazaRaspunsCommand, int>
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
        public async Task<int> Handle(CompleteazaRaspunsCommand message)
        {
            try
            {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var raspunsuriNoi = message.Raspunsuri.Select(a => new
                {
                    flat = a.Optiuni.Select(o => new Raspuns
                    {
                        IdObservator = message.IdObservator,
                        IdSectieDeVotare = a.IdSectie,
                        IdRaspunsDisponibil = o.IdOptiune,
                        Value = o.Value,
                        DataUltimeiModificari = lastModified
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
                        var raspunsuri = raspunsuriNoi.Where(a=> a.IdSectieDeVotare == sectie).Select(a => a.IdRaspunsDisponibil).ToList();

                        // delete existing answers for posted questions on this 'sectie'
                        _context.Raspuns
                            .Where(
                                a =>
                                    a.IdObservator == message.IdObservator &&
                                    a.IdSectieDeVotare == sectie)
                                   .WhereRaspunsContains(raspunsuri)
                            .Delete();
                    }

                    _context.Raspuns.AddRange(raspunsuriNoi);

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
        public static IQueryable<Raspuns> WhereRaspunsContains(this IQueryable<Raspuns> source, IList<int> contains)
        {
            var ors = contains
                .Aggregate<int, Expression<Func<Raspuns, bool>>>(null, (expression, id) => 
                expression == null 
                    ? (a => a.IdRaspunsDisponibil == id) 
                    : expression.Or(a => a.IdRaspunsDisponibil == id));

            return source.Where(ors);
        }
    }
}

