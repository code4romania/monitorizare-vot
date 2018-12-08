using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.AnswerAggregate.Commands;
using VotingIrregularities.Domain.Models;
using Z.EntityFramework.Plus;

namespace VotingIrregularities.Domain.AnswerAggregate
{
    public class SendAnswerHandler : IAsyncRequestHandler<SendAnswerCommand, int>
    {
        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SendAnswerHandler(VotingContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<int> Handle(SendAnswerCommand message)
        {
            try
            {
                //flat answers
                var lastModified = DateTime.UtcNow;

                var newAnswers = message.Answers.Select(a => new
                {
                    flat = a.Options.Select(o => new Answer
                    {
                        ObserverId = message.ObserverId,
                        VotingSectionId = a.SectionId,
                        AvailableAnswerId = o.OptionId,
                        Value = o.Value,
                        CountyCode = a.CountyCode,
                        SectionNumber = a.SectionNumber,
                        LastChangeDate = lastModified
                    })
                }).SelectMany(a => a.flat)
                .Distinct()
                .ToList();

                // stergerea este pe fiecare sectie
                var sections = message.Answers.Select(a => a.SectionId).Distinct().ToList();

                using (var tran = await _context.Database.BeginTransactionAsync())
                {
                    foreach (var sectie in sections)
                    {

                        var questions = message.Answers.Select(a => a.QuestionId).Distinct().ToList();

                        // delete existing answers for posted questions on this 'sectie'
                        _context.Answers
                            .Include(a => a.AvailableAnswerNavigationId)
                            .Where(
                                a =>
                                    a.ObserverId == message.ObserverId &&
                                    a.VotingSectionId == sectie)
                                   .WhereAnswerContains(questions)
                            .Delete();
                    }

                    _context.Answers.AddRange(newAnswers);

                    var result =  await _context.SaveChangesAsync();

                    tran.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(typeof(SendAnswerCommand).GetHashCode(), ex, ex.Message);
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
        public static IQueryable<Answer> WhereAnswerContains(this IQueryable<Answer> source, IList<int> contains)
        {
            var ors = contains
                .Aggregate<int, Expression<Func<Answer, bool>>>(null, (expression, id) => 
                expression == null 
                    ? (a => a.AvailableAnswerNavigationId.QuestionId == id) 
                    : expression.Or(a => a.AvailableAnswerNavigationId.QuestionId == id));

            return source.Where(ors);
        }
    }
}
