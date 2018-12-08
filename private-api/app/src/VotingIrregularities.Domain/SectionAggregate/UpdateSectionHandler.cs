using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.SectionAggregate
{
    public class UpdateSectionHandler : IAsyncRequestHandler<SectionUpdateCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UpdateSectionHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(SectionUpdateCommand message)
        {
            try
            {
                var form = await _context.FormAnswers
                    .FirstOrDefaultAsync(a =>
                        a.ObserverId == message.ObserverId &&
                        a.VotingSectionId == message.VotingSectionId);

                if (form == null)
                    throw new ArgumentException("FormAnswers nu exista");
               
                _mapper.Map(message, form);
                _context.Update(form);

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
