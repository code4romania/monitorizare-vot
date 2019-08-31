using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Core.Services;
using VotingIrregularities.Domain.Models;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ObserverRequestsHandler :
        IRequestHandler<ImportObserversRequest, int>,
        IRequestHandler<NewObserverRequest, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private IHashService _hashService;

        public ObserverRequestsHandler(VotingContext context, ILogger logger, IHashService hashService)
        {
            _context = context;
            _logger = logger;
            _hashService = hashService;
        }

        private int GetMaxIdObserver()
        {
            return _context.Observers.Max(o => o.Id) + 1;
        }

        public Task<int> Handle(ImportObserversRequest message, CancellationToken token)
        {
            var pathToFile = message.FilePath;
            var counter = 0;
            var startId = GetMaxIdObserver();

            using (var reader = File.OpenText(pathToFile))
            {
                while (reader.Peek() >= 0)
                {
                    var fileContent = reader.ReadLine();

                    var data = fileContent.Split('\t');
                    var hashed = _hashService.GetHash(data[1]);

                    var observer = new VotingIrregularities.Domain.Models.Observer
                    {
                        Id = startId + counter,
                        IdNgo = message.IdOng,
                        Phone = data[0],
                        Name = data[message.NameIndexInFile],
                        Pin = hashed
                    };
                    _context.Observers.Add(observer);
                    counter++;
                }
                _context.SaveChanges();
            }

            return Task.FromResult(counter);
        }

        public Task<int> Handle(NewObserverRequest message, CancellationToken token)
        {
            var id = GetMaxIdObserver();
            var observer = new VotingIrregularities.Domain.Models.Observer
            {
                Id = id,
                IdNgo = message.IdOng,
                Phone = message.NumarTelefon,
                Name = message.Nume,
                Pin = _hashService.GetHash(message.PIN)
            };
            _context.Observers.Add(observer);
            return _context.SaveChangesAsync();
        }
    }
}