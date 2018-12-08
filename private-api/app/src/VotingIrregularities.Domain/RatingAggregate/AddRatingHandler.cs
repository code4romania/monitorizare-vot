using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.RatingAggregate
{
    public class AddRatingHandler : IAsyncRequestHandler<AddRatingCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AddRatingHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddRatingCommand message)
        {
            try
            {
                if (message.QuestionId.HasValue && message.QuestionId.Value > 0)
                {
                    var existaIntrebare = await _context.Questions.AnyAsync(i => i.QuestionId == message.QuestionId.Value);

                    if(!existaIntrebare)
                        throw new ArgumentException("Intrebarea nu exista");
                }

                var nota = _mapper.Map<Rating>(message);
                _context.Add(nota);

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
