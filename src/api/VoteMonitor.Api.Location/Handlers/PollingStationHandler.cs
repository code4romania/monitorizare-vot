using AutoMapper;
using EFCore.BulkExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationHandler : IRequestHandler<PollingStationCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PollingStationHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(PollingStationCommand request, CancellationToken cancellationToken)
        {
            var random = new Random();

            try
            {
                //import the new entities
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var id = 100;
                    var newPollingStations = new List<PollingStation>();
                    var counties = _context.Counties.ToDictionary(c => c.Code, c => c.Id);

                    foreach (var record in request.PollingStationsDTOs)
                    {
                        var pollingStation = _mapper.Map<PollingStation>(record);
                        pollingStation.Id = id++;
                        pollingStation.IdCounty = counties[record.CodJudet];//county.Id;
                        pollingStation.Coordinates = null;
                        pollingStation.TerritoryCode = random.Next(10000).ToString();

                        newPollingStations.Add(pollingStation);
                    }

                    _context.BulkInsert(newPollingStations);

                    foreach (var county in _context.Counties)
                    {
                        if (!_context.PollingStations.Any(p => p.IdCounty == county.Id))
                            continue;

                        var maxPollingStation = _context.PollingStations
                            .Where(p => p.IdCounty == county.Id)
                            .Max(p => p.Number);
                        county.NumberOfPollingStations = maxPollingStation;
                        _context.Counties.Update(county);
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken);

                    transaction.Commit();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while importing polling station information ", ex);
            }

            return -1;
        }
    }
}