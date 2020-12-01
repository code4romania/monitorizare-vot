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
using VoteMonitor.Api.Location.Exceptions;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Models.ResultValues;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationHandler : IRequestHandler<PollingStationCommand, PollingStationImportResultValue>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PollingStationHandler(VoteMonitorContext context, IMapper mapper, ILogger<PollingStationHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PollingStationImportResultValue> Handle(PollingStationCommand request, CancellationToken cancellationToken)
        {

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var countiesFromDatabase = _context.Counties.ToList();

                    List<PollingStation> newPollingStations = CreatePollingStationEntitiesFromDto(request.PollingStationsDTOs, countiesFromDatabase);
                    _context.BulkInsert(newPollingStations);

                    UpdateCountiesPollingStationCounter(countiesFromDatabase, newPollingStations);

                    var result = await _context.SaveChangesAsync(cancellationToken);

                    transaction.Commit();
                    return PollingStationImportResultValue.SuccessValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while importing polling station information ", ex);
                return new PollingStationImportResultValue(ex);
            }
        }

        private void UpdateCountiesPollingStationCounter(List<County> countiesFromDatabase, List<PollingStation> newPollingStations)
        {
            var idsOfCountiesToBeUpdated = newPollingStations.Select(x => x.IdCounty).Distinct();
            var countiesToBeUpdated = countiesFromDatabase.Where(c => idsOfCountiesToBeUpdated.Any(id => c.Id == id));

            foreach (var county in countiesToBeUpdated)
            {
                county.NumberOfPollingStations = _context.PollingStations
                    .Where(p => p.IdCounty == county.Id)
                    .Count();
                _context.Counties.Update(county);
            }
        }

        private List<PollingStation> CreatePollingStationEntitiesFromDto(List<PollingStationDTO> pollingStationDtos, List<County> countiesFromDatabase)
        {
            var random = new Random();
            var startingPsId = _context.PollingStations.Any() ? _context.PollingStations.Max(ps => ps.Id) + 1 : 1;

            var newPollingStations = new List<PollingStation>();
            foreach (var record in pollingStationDtos)
            {
                var countyForPollingStation = countiesFromDatabase.FirstOrDefault(x => x.Code.Equals(record.CodJudet, StringComparison.OrdinalIgnoreCase));
                if (countyForPollingStation == null)
                {
                    throw new PollingStationImportException($"County {record.CodJudet} not found in the database");
                }

                var pollingStation = _mapper.Map<PollingStation>(record);
                pollingStation.Id = startingPsId++;
                pollingStation.IdCounty = countyForPollingStation.Id;
                pollingStation.Coordinates = null;
                pollingStation.TerritoryCode = random.Next(10000).ToString();

                newPollingStations.Add(pollingStation);

            }

            return newPollingStations;
        }
    }
}