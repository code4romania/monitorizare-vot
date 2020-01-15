using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.County.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.County.Handlers
{
    public class CountiesCommandHandler : IRequestHandler<GetCountiesForExport, Result<List<CountyCsvModel>>>,
        IRequestHandler<CreateOrUpdateCounties, Result>

    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public CountiesCommandHandler(VoteMonitorContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<CountyCsvModel>>> Handle(GetCountiesForExport request, CancellationToken cancellationToken)
        {
            return await Result.Try(async () =>
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
            },ex => { 
                _logger.LogError("Error retrieving counties", ex);
                return "Cannot retrieve counties.";
            });
        }

        public async Task<Result> Handle(CreateOrUpdateCounties request, CancellationToken cancellationToken)
        {
            var result = await ReadFromCsv(request)
                .Ensure(x=>x !=null && x.Count >0,"No counties to add or update")
                .Bind(ValidateData)
                .Tap(async x=>await InsertOrUpdateCounties(x, cancellationToken));
            
            return result;
        }

        private async Task<Result> InsertOrUpdateCounties(List<CountyCsvModel> counties, CancellationToken cancellationToken)
        {
            List<int> countiesIdUpdated = new List<int>();
            var countiesDictionary = counties.ToDictionary(c => c.Id, y => y);

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
            catch (Exception exception)
            {
                _logger.LogError("Cannot add/update counties", exception);
                return Result.Failure("Cannot add/update counties");
            }

            return Result.Ok();
        }

        private Result<List<CountyCsvModel>> ValidateData(List<CountyCsvModel> counties)
        {
            if (counties.Count != counties.Select(x=>x.Id).Distinct().Count())
            {
                return Result.Failure<List<CountyCsvModel>>("Duplicated id in csv found");
            }

            var invalidCounty = counties.FirstOrDefault(x =>
                x == null
                || string.IsNullOrEmpty(x.Code)
                || string.IsNullOrEmpty(x.Name)
                || x.Name.Length > 100
                || x.Code.Length > 20);

            if (invalidCounty == null)
            {
                
                return Result.Ok(counties);
            }

            return Result.Failure<List<CountyCsvModel>>($"Invalid county entry found: {JsonConvert.SerializeObject(invalidCounty)}");
        }

        private Result<List< CountyCsvModel>> ReadFromCsv(CreateOrUpdateCounties request)
        {
            List<CountyCsvModel> counties;

            try
            {
                using (var reader = new StreamReader(request.File.OpenReadStream()))
                using (var csv = new CsvReader(reader))
                {
                    counties = csv.GetRecords<CountyCsvModel>()
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to read csv file", e);
                return Result.Failure<List<CountyCsvModel>>("Cannot read csv file provided");
            }

            return Result.Ok(counties);
        }
    }
}