using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.County.Handlers
{
    public class CountiesCommandHandler : IRequestHandler<GetCountiesForExport, List<CountyCsvModel>>,
        IRequestHandler<CreateOrUpdateCounties, object>

    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public CountiesCommandHandler(VoteMonitorContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CountyCsvModel>> Handle(GetCountiesForExport request, CancellationToken cancellationToken)
        {
            return await _context.Counties
                     .OrderBy(c => c.Order)
                     .Select(c => new CountyCsvModel
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         NumberOfPollingStations = c.NumberOfPollingStations,
                         Diaspora = c.Diaspora,
                         Order = c.Order
                     })
                     .ToListAsync(cancellationToken);
        }

        public async Task<object> Handle(CreateOrUpdateCounties request, CancellationToken cancellationToken)
        {
            Dictionary<int, CountyCsvModel> countiesDictionary = new Dictionary<int, CountyCsvModel>();

            try
            {
                using (var reader = new StreamReader(request.File.OpenReadStream()))
                using (var csv = new CsvReader(reader))
                {
                    countiesDictionary = csv.GetRecords<CountyCsvModel>()
                        .ToList()
                        .ToDictionary(c => c.Id, y => y); ;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to read csv file", e);
            }


            List<int> countiesIdUpdated = new List<int>();

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    foreach (var county in _context.Counties)
                    {
                        if (countiesDictionary.TryGetValue(county.Id, out var csvModel))
                        {
                            county.Code = csvModel.Code;
                            county.Name = csvModel.Name;
                            county.NumberOfPollingStations = csvModel.NumberOfPollingStations;
                            county.Diaspora = csvModel.Diaspora;
                            county.Order = csvModel.Order;

                            countiesIdUpdated.Add(county.Id);
                        }
                    }

                    foreach (var id in countiesDictionary.Keys.Except(countiesIdUpdated))
                    {
                        var csvModel = countiesDictionary[id];

                        var newCounty = new Entities.County
                        {
                            Id = csvModel.Id,
                            Code = csvModel.Code,
                            Name = csvModel.Name,
                            NumberOfPollingStations = csvModel.NumberOfPollingStations,
                            Diaspora = csvModel.Diaspora,
                            Order = csvModel.Order
                        };

                        _context.Counties.Add(newCounty);
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken);

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot add/update counties", e);
            }

            return null;
        }
    }
}