using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

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
                        IdIntrebare = a.IdIntrebare,
                        IdOptiune = o.IdOptiune,
                        Value = o.Value,
                        DataUltimeiModificari = lastModified
                    })
                }).SelectMany(a => a.flat)
                .Distinct()
                .ToList();

                // load all related existing answers in one call
                var merge = (from l in raspunsuriNoi
                             join d in _context.Raspuns on
                                 new { l.IdObservator, l.IdSectieDeVotare, l.IdIntrebare, l.IdOptiune } equals
                                 new { d.IdObservator, d.IdSectieDeVotare, d.IdIntrebare, d.IdOptiune }
                                 into raspunsuriExistente
                             from potrivite in raspunsuriExistente.DefaultIfEmpty()
                             select new
                             {
                                 IdObservator = message.IdObservator,
                                 IdSectieDeVotare = l.IdSectieDeVotare,
                                 IdIntrebare = l.IdIntrebare,
                                 IdOptiune = l.IdOptiune,
                                 Value = l.Value,
                                 DataUltimeiModificari = lastModified,
                                 IsNew = potrivite == null
                             }).ToList();

                // if the answer is already in db update it, if new then insert it
                foreach (var raspuns in merge)
                {
                    if (raspuns.IsNew)
                        _context.Raspuns.Add(new Raspuns
                        {
                            IdObservator = message.IdObservator,
                            IdSectieDeVotare = raspuns.IdSectieDeVotare,
                            IdIntrebare = raspuns.IdIntrebare,
                            IdOptiune = raspuns.IdOptiune,
                            Value = raspuns.Value,
                            DataUltimeiModificari = lastModified,
                        });

                    else
                        _context.Entry(raspuns).State = EntityState.Added;
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(typeof(CompleteazaRaspunsCommand).GetHashCode(), ex, ex.Message);
            }

            return await Task.FromResult(-1);
        }
    }
}
