using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Handlers
{
    public class AnswerQueryHandler :
        AsyncRequestHandler<BulkAnswers, CompleteazaRaspunsCommand>
    {
        private readonly VoteMonitorContext _context;

        public AnswerQueryHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        protected override async Task<CompleteazaRaspunsCommand> HandleCore(BulkAnswers message)
        {
            // se identifica sectiile in care observatorul a raspuns
            var sectii = message.Answers
                .Select(a => new { a.PollingStationNumber, a.CountyCode })
                .Distinct()
                .ToList();

            var command = new CompleteazaRaspunsCommand { ObserverId = message.ObserverId };


            foreach (var sectie in sectii)
            {
                var idSectie = (await _context
                    .PollingStations
                    .FirstOrDefaultAsync(p => p.County.Code == sectie.CountyCode && p.Number == sectie.PollingStationNumber))
                    .Id;
                //(sectie.PollingStationNumber, sectie.CountyCode);

                command.Answers.AddRange(message.Answers
                    .Where(a => a.PollingStationNumber == sectie.PollingStationNumber && a.CountyCode == sectie.CountyCode)
                    .Select(a => new AnswerDTO
                    {
                        QuestionId = a.QuestionId,
                        PollingSectionId = idSectie,
                        Options = a.Options,
                        PollingStationNumber = a.PollingStationNumber,
                        CountyCode = a.CountyCode
                    }));
            }

            return command;
        }
    }
}
