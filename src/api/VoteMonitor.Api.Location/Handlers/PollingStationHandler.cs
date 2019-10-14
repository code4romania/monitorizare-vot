using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MonitorizareVot.Api.Location.Commands;
using VoteMonitor.Entities;

namespace MonitorizareVot.Api.Location.Handlers
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
            var records = request.PollingStationsDTOs;
            Random random = new Random();
            try
            {
                //remove all previous entries in the db set
                foreach(var pollingStation in _context.PollingStations)
                    _context.PollingStations.Remove(pollingStation);
                _context.SaveChanges();

                //import the new entities
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    int id = 0;
                    foreach(var record in records)
                    {
                        PollingStation pollingStation = _mapper.Map<PollingStation>(record);
                        pollingStation.Id = id++;
                        pollingStation.IdCounty = _context.Counties
                                            .Where(c => c.Code == record.CodJudet)
                                            .First()
                                            .Id;
                        pollingStation.Coordinates = null;
                        pollingStation.County = _context.Counties
                                            .Where(c => c.Code == record.CodJudet)
                                            .First();
                        pollingStation.TerritoryCode = random.Next(10000).ToString();
                        _context.PollingStations.Add(pollingStation);
                    }

                    var result = await _context.SaveChangesAsync();
                    transaction.Commit();

                    return result;
                }
            } catch(Exception ex)
            {
                _logger.LogError("Error while importing polling station information ", ex);
            }

            return -1;
        }
    }
}