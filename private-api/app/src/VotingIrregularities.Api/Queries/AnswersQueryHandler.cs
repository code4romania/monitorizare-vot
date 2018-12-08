using System.Linq;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.AnswerAggregate.Commands;

namespace VotingIrregularities.Api.Queries
{
    /// <summary>
    /// Hidrateaza sectiile de votare din comanda data de observator.
    /// </summary>
    public class AnswersQueryHandler : 
        IAsyncRequestHandler<AnswersBulk, SendAnswerCommand>
    {
        private readonly IVotingSectionService _svService;

        public AnswersQueryHandler(IVotingSectionService svService)
        {
            _svService = svService;
        }

        public async Task<SendAnswerCommand> Handle(AnswersBulk message)
        {
            // se identifica sectiile in care observatorul a raspuns
            var sections = message.AnswersModelBulk
                .Select(a => new {SectionNumber = a.SectionNumber, CountyCode = a.CountyCode})
                .Distinct()
                .ToList();

            var command = new SendAnswerCommand { ObserverId = message.ObserverId };

            
            foreach (var section in sections)
            {
                var sectionId = await _svService.GetSingleVotingSection(section.CountyCode, section.SectionNumber);

                command.Answers.AddRange(message.AnswersModelBulk
                    .Where(a => a.SectionNumber == section.SectionNumber && a.CountyCode == section.CountyCode)
                    .Select(a => new AnswerModel
                {
                    FormCode = a.FormCode,
                    QuestionId = a.QuestionId,
                    SectionId = sectionId,
                    Options = a.Options,
                    SectionNumber = a.SectionNumber,
                    CountyCode = a.CountyCode
                }));
            }

            return command;
        }
    }
}
