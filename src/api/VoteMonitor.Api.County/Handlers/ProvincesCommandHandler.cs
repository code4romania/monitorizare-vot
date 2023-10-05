using CSharpFunctionalExtensions;
using CsvHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.County.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.County.Handlers;

public class ProvincesCommandHandler : IRequestHandler<GetProvincesForExport, Result<List<ProvinceCsvModel>>>,
    IRequestHandler<CreateOrUpdateProvinces, Result>,
    IRequestHandler<GetAllProvinces, Result<List<ProvinceModel>>>,
    IRequestHandler<GetProvinceById, Result<ProvinceModel>>,
    IRequestHandler<UpdateProvince, Result>,
    IRequestHandler<GetAllCountiesByProvinceCode, Result<List<CountyModel>>>

{
    private readonly VoteMonitorContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    private const int NameMaxLength = 100;
    private const int CodeMaxLength = 20;

    public ProvincesCommandHandler(VoteMonitorContext context, ICacheService cacheService, ILogger<ProvincesCommandHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<List<ProvinceCsvModel>>> Handle(GetProvincesForExport request, CancellationToken cancellationToken)
    {
        return await Result.Try(async () =>
        {
            return await _context.Provinces
                .OrderBy(c => c.Order)
                .Select(c => new ProvinceCsvModel
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                })
                .ToListAsync(cancellationToken);
        }, ex =>
        {
            _logger.LogError(ex, "Error retrieving provinces");
            return "Cannot retrieve provinces.";
        });
    }

    public async Task<Result> Handle(CreateOrUpdateProvinces request, CancellationToken cancellationToken)
    {
        var result = await ReadFromCsv(request)
            .Ensure(x => x != null && x.Count > 0, "No provinces to add or update")
            .Bind(x => ValidateData(x))
            .Check(async x => await InsertOrUpdateProvinces(x, cancellationToken));

        return result;
    }

    private async Task<Result> InsertOrUpdateProvinces(List<ProvinceCsvModel> provinces, CancellationToken cancellationToken)
    {
        var provincesIdUpdated = new List<int>();
        var provincesDictionary = provinces.ToDictionary(c => c.Id, y => y);

        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            foreach (var province in _context.Provinces)
            {
                if (provincesDictionary.TryGetValue(province.Id, out var csvModel))
                {
                    province.Code = csvModel.Code;
                    province.Name = csvModel.Name;
                    province.Order = csvModel.Order;

                    provincesIdUpdated.Add(province.Id);
                }
            }

            foreach (var id in provincesDictionary.Keys.Except(provincesIdUpdated))
            {
                var csvModel = provincesDictionary[id];

                var newProvince = new Province
                {
                    Id = csvModel.Id,
                    Code = csvModel.Code,
                    Name = csvModel.Name,
                    Order = csvModel.Order
                };

                _context.Provinces.Add(newProvince);
            }

            await _context.SaveChangesAsync(cancellationToken);

            transaction.Commit();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Cannot add/update provinces");
            return Result.Failure("Cannot add/update provinces");
        }

        return Result.Success();
    }

    private Result<List<ProvinceCsvModel>> ValidateData(List<ProvinceCsvModel> provinces)
    {
        if (provinces.Count != provinces.Select(x => x.Id).Distinct().Count())
        {
            return Result.Failure<List<ProvinceCsvModel>>("Duplicated id in csv found");
        }

        var invalidProvince = provinces.FirstOrDefault(x =>
            x == null
            || string.IsNullOrEmpty(x.Code)
            || string.IsNullOrEmpty(x.Name)
            || x.Name.Length > NameMaxLength
            || x.Code.Length > CodeMaxLength);

        if (invalidProvince == null)
        {
            return Result.Success(provinces);
        }

        return Result.Failure<List<ProvinceCsvModel>>($"Invalid province entry found: {JsonConvert.SerializeObject(invalidProvince)}");
    }

    private Result<List<ProvinceCsvModel>> ReadFromCsv(CreateOrUpdateProvinces request)
    {
        List<ProvinceCsvModel> provinces;

        try
        {
            using var reader = new StreamReader(request.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            provinces = csv.GetRecords<ProvinceCsvModel>()
                .ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to read csv file");
            return Result.Failure<List<ProvinceCsvModel>>("Cannot read csv file provided");
        }

        return Result.Success(provinces);
    }

    public async Task<Result<List<ProvinceModel>>> Handle(GetAllProvinces request, CancellationToken cancellationToken)
    {
        List<ProvinceModel> provinces;

        try
        {
            provinces = await _cacheService.GetOrSaveDataInCacheAsync("provinces", async () => await _context.Provinces
                .OrderBy(c => c.Order)
                .Select(x => new ProvinceModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Order = x.Order
                })
                .ToListAsync(cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to load all provinces");
            return Result.Failure<List<ProvinceModel>>("Unable to load all provinces");
        }

        return Result.Success(provinces);
    }

    public async Task<Result<ProvinceModel>> Handle(GetProvinceById request, CancellationToken cancellationToken)
    {
        try
        {
            var province = await _context.Provinces
                .Where(x => x.Id == request.ProvinceId)
                .Select(c => new ProvinceModel
                {
                    Id = request.ProvinceId,
                    Code = c.Code,
                    Name = c.Name,
                    Order = c.Order,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (province == null)
            {
                return Result.Failure<ProvinceModel>($"Could not find province with id = {request.ProvinceId}");
            }

            return Result.Success(province);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to load province {request.ProvinceId}", e);
            return Result.Failure<ProvinceModel>($"Unable to load province {request.ProvinceId}");
        }
    }

    public async Task<Result> Handle(UpdateProvince request, CancellationToken cancellationToken)
    {
        try
        {
            var province = await _context.Provinces.FirstOrDefaultAsync(x => x.Id == request.ProvinceId, cancellationToken);
            if (province == null)
            {
                return Result.Failure($"Could not find province with id = {request.ProvinceId}");
            }

            province.Code = request.Code;
            province.Name = request.Name;
            province.Order = request.Order;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to update province {request.ProvinceId}", e);
            return Result.Failure($"Unable to update province {request.ProvinceId}");
        }
    }

    public async Task<Result<List<CountyModel>>> Handle(GetAllCountiesByProvinceCode request, CancellationToken cancellationToken)
    {
        List<CountyModel> counties;

        try
        {
            counties = await _cacheService.GetOrSaveDataInCacheAsync($"province-{request.ProvinceCode}/counties", async () => await _context.Counties
                .Where(x => x.Province.Code == request.ProvinceCode)
                .OrderBy(c => c.Order)
                .Select(x => new CountyModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    ProvinceCode = x.Province.Code,
                    Diaspora = x.Diaspora,
                    Name = x.Name,
                    NumberOfPollingStations = x.Municipalities.Sum(x => x.PollingStations.Count),
                    Order = x.Order
                })
                .ToListAsync(cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to load all counties");
            return Result.Failure<List<CountyModel>>("Unable to load all counties");
        }

        return counties;
    }
}
