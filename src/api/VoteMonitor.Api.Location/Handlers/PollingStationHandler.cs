using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;
using EFCore.BulkExtensions;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationHandler : AsyncRequestHandler<PollingStationCommand, int>
    {
        private VoteMonitorContext _context;
        private IMapper _mapper;
        private ILogger _logger;

        public PollingStationHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            this._context = context;
            this._mapper = mapper;
            this._logger = logger;
        }

        protected override async Task<int> HandleCore(PollingStationCommand request)
        {
            var random = new Random();

            try
            {
                //import the new entities
                using (var transaction = await _context.Database.BeginTransactionAsync())
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

                    var result = await _context.SaveChangesAsync();

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