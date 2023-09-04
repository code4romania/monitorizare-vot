using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ObserverRequestsHandler :
        IRequestHandler<ImportObserversRequest, int>,
        IRequestHandler<NewObserverCommand, int>,
        IRequestHandler<EditObserverCommand, int>,
        IRequestHandler<DeleteObserverCommand, bool>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private IHashService _hashService;

        public ObserverRequestsHandler(VoteMonitorContext context, ILogger<ObserverRequestsHandler> logger, IHashService hashService)
        {
            _context = context;
            _logger = logger;
            _hashService = hashService;
        }

        private int GetMaxIdObserver()
        {
            if (_context.Observers.Any())
            {
                return _context.Observers.Max(o => o.Id) + 1;
            }

            return 1;
        }

        public async Task<int> Handle(ImportObserversRequest message, CancellationToken token)
        {
            var counter = 0;
            var startId = GetMaxIdObserver();

            using var reader = new StreamReader(message.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var observers = csv.GetRecords<ObserversImportModel>()
                .ToList();

            foreach (var importedObserver in observers)
            {
                var observer = new Entities.Observer
                {
                    Id = startId + counter,
                    IdNgo = message.NgoId,
                    Phone = importedObserver.Phone,
                    Name = importedObserver.Name,
                    Pin = _hashService.GetHash(importedObserver.Pin)
                };
                _context.Observers.Add(observer);
                counter++;
            }

            await _context.SaveChangesAsync();

            return counter;
        }

        public async Task<int> Handle(NewObserverCommand message, CancellationToken token)
        {
            var id = GetMaxIdObserver();
            var observer = new Entities.Observer
            {
                Id = id,
                IdNgo = message.NgoId,
                Phone = message.Phone,
                Name = message.Name,
                Pin = _hashService.GetHash(message.Pin)
            };
            _context.Observers.Add(observer);
            await _context.SaveChangesAsync();
            return observer.Id;
        }

        public async Task<int> Handle(EditObserverCommand request, CancellationToken cancellationToken)
        {

            var observer = await _context.Observers.FirstOrDefaultAsync(o => o.Id == request.ObserverId);
            if (observer == null)
            {
                return -1;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                observer.Name = request.Name;
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                observer.Phone = request.Phone;
            }

            if (!string.IsNullOrWhiteSpace(request.Pin))
            {
                observer.Pin = _hashService.GetHash(request.Pin);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> Handle(DeleteObserverCommand request, CancellationToken cancellationToken)
        {
            var observer = await _context.Observers.FirstOrDefaultAsync(o => o.Id == request.ObserverId);
            if (observer == null)
            {
                return false;
            }
            _context.Observers.Remove(observer);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
