using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        IRequestHandler<CreateOrUpdateCounties, Result>,
        IRequestHandler<GetAllCounties, Result<List<CountyModel>>>,
        IRequestHandler<GetCounty, Result<CountyModel>>,
        IRequestHandler<UpdateCounty, Result>

    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        private const int NameMaxLength = 100;
        private const int CodeMaxLength = 20;

        public CountiesCommandHandler(VoteMonitorContext context, ILogger<CountiesCommandHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<List<CountyCsvModel>>> Handle(GetCountiesForExport request, CancellationToken cancellationToken)
        {
            return await Result.Try(async () =>
            {
                return await _context.Counties
                    .OrderBy(c => c.Order)
                    .Select(c => _mapper.Map<CountyCsvModel>(c))
                    .ToListAsync(cancellationToken);
            }, ex =>
            {
                _logger.LogError("Error retrieving counties", ex);
                return "Cannot retrieve counties.";
            });
        }

        public async Task<Result> Handle(CreateOrUpdateCounties request, CancellationToken cancellationToken)
        {
            var result = await ReadFromCsv(request)
                .Ensure(x => x != null && x.Count > 0, "No counties to add or update")
                .Bind(x => ValidateData(x))
                .Tap(async x => await InsertOrUpdateCounties(x, cancellationToken));

            return result;
        }

        private async Task<Result> InsertOrUpdateCounties(List<CountyCsvModel> counties, CancellationToken cancellationToken)
        {
            var countiesIdUpdated = new List<int>();
            var countiesDictionary = counties.ToDictionary(c => c.Id, y => y);

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                foreach (var county in _context.Counties)
                {
                    if (countiesDictionary.TryGetValue(county.Id, out var csvModel))
                    {
                        county.Code = csvModel.Code;
                        county.Name = csvModel.Name;
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
                        Diaspora = csvModel.Diaspora,
                        Order = csvModel.Order
                    };

                    _context.Counties.Add(newCounty);
                }

                await _context.SaveChangesAsync(cancellationToken);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogError("Cannot add/update counties", exception);
                return Result.Failure("Cannot add/update counties");
            }

            return Result.Success();
        }

        private Result<List<CountyCsvModel>> ValidateData(List<CountyCsvModel> counties)
        {
            if (counties.Count != counties.Select(x => x.Id).Distinct().Count())
            {
                return Result.Failure<List<CountyCsvModel>>("Duplicated id in csv found");
            }

            var invalidCounty = counties.FirstOrDefault(x =>
                x == null
                || string.IsNullOrEmpty(x.Code)
                || string.IsNullOrEmpty(x.Name)
                || x.Name.Length > NameMaxLength
                || x.Code.Length > CodeMaxLength);

            if (invalidCounty == null)
            {
                return Result.Success(counties);
            }

            return Result.Failure<List<CountyCsvModel>>($"Invalid county entry found: {JsonConvert.SerializeObject(invalidCounty)}");
        }

        private Result<List<CountyCsvModel>> ReadFromCsv(CreateOrUpdateCounties request)
        {
            List<CountyCsvModel> counties;

            try
            {
                using var reader = new StreamReader(request.File.OpenReadStream());
                using var csv = new CsvReader(reader);
                counties = csv.GetRecords<CountyCsvModel>()
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to read csv file", e);
                return Result.Failure<List<CountyCsvModel>>("Cannot read csv file provided");
            }

            return Result.Success(counties);
        }

        public async Task<Result<List<CountyModel>>> Handle(GetAllCounties request, CancellationToken cancellationToken)
        {
            List<CountyModel> counties;

            try
            {
                counties = await _context.Counties
                    .OrderBy(c => c.Order)
                    .Select(x => new CountyModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        NumberOfPollingStations = x.PollingStations.Count(),
                        Diaspora = x.Diaspora,
                        Order = x.Order
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to load all counties", e);
                return Result.Failure<List<CountyModel>>("Unable to load all counties");
            }

            return Result.Success(counties);
        }

        public async Task<Result<CountyModel>> Handle(GetCounty request, CancellationToken cancellationToken)
        {
            try
            {
                var county = await _context.Counties.Select(c => new CountyModel
                {
                    Id = request.CountyId,
                    Code = c.Code,
                    Name = c.Name,
                    Order = c.Order,
                    Diaspora = c.Diaspora,
                    NumberOfPollingStations = c.PollingStations.Count()
                }).FirstOrDefaultAsync(x => x.Id == request.CountyId, cancellationToken);

                if (county == null)
                {
                    return Result.Failure<CountyModel>($"Could not find county with id = {request.CountyId}");
                }

                return Result.Success(county);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to load county {request.CountyId}", e);
                return Result.Failure<CountyModel>($"Unable to load county {request.CountyId}");
            }
        }

        public async Task<Result> Handle(UpdateCounty request, CancellationToken cancellationToken)
        {
            try
            {
                var county = await _context.Counties.FirstOrDefaultAsync(x => x.Id == request.CountyId, cancellationToken);
                if (county == null)
                {
                    return Result.Failure($"Could not find county with id = {request.CountyId}");
                }

                county.Code = request.Code;
                county.Name = request.Name;
                county.Diaspora = request.Diaspora;
                county.Order = request.Order;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to update county {request.CountyId}", e);
                return Result.Failure($"Unable to update county {request.CountyId}");
            }
        }
    }
}
