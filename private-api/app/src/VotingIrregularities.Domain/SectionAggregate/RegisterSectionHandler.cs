using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.SectionAggregate
{
    public class RegisterSectionHandler : IAsyncRequestHandler<RegisterSectionCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public RegisterSectionHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(RegisterSectionCommand message)
        {
            try
            {
                //TODO[DH] this can be moved to a previous step, before the command is executed
                int sectionId = await _context.VotingSections
                    .Where(a =>
                        a.SectionNumber == message.NumarSectie &&
                        a.CountyNavigationId.CountyCode == message.CodJudet).Select(a => a.VotingSectionId)
                        .FirstOrDefaultAsync();

                if (sectionId == 0)
                    throw new ArgumentException("Sectia nu exista");

                var form = await _context.FormAnswers
                    .FirstOrDefaultAsync(a =>
                        a.ObserverId == message.ObserverId &&
                        a.VotingSectionId == sectionId);

                if (form == null)
                {
                    form = _mapper.Map<FormAnswer>(message);

                    form.VotingSectionId = sectionId;

                    _context.Add(form);
                }
                else
                {
                    _mapper.Map(message, form);
                }

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}