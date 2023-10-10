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

public class MunicipalityCommandHandler : IRequestHandler<GetMunicipalitiesForExport, Result<List<MunicipalityCsvModel>>>,
    IRequestHandler<CreateOrUpdateMunicipalities, Result>,
    IRequestHandler<GetAllMunicipalitiesByCountyCode, Result<List<MunicipalityModel>>>,
    IRequestHandler<GetAllMunicipalities, Result<List<MunicipalityModelV2>>>,
    IRequestHandler<GetMunicipalityById, Result<MunicipalityModel>>,
    IRequestHandler<UpdateMunicipality, Result>

{
    private readonly VoteMonitorContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    private const int NameMaxLength = 100;
    private const int CodeMaxLength = 20;

    public MunicipalityCommandHandler(VoteMonitorContext context, ICacheService cacheService, ILogger<MunicipalityCommandHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<List<MunicipalityCsvModel>>> Handle(GetMunicipalitiesForExport request, CancellationToken cancellationToken)
    {
        return await Result.Try(async () =>
        {
            return await _context.Municipalities
                .OrderBy(m => m.Order)
                .Select(m => new MunicipalityCsvModel
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    CountyCode = m.County.Code,
                    Order = m.Order
                })
                .ToListAsync(cancellationToken);
        }, ex =>
        {
            _logger.LogError(ex, "Error retrieving municipalities");
            return "Cannot retrieve municipalities.";
        });
    }

    public async Task<Result> Handle(CreateOrUpdateMunicipalities request, CancellationToken cancellationToken)
    {
        var result = await ReadFromCsv(request)
            .Ensure(x => x != null && x.Count > 0, "No municipalities to add or update")
            .Bind(x => ValidateData(x))
            .Check(async x => await InsertOrUpdateMunicipalities(x, cancellationToken));

        return result;
    }

    private async Task<Result> InsertOrUpdateMunicipalities(List<MunicipalityCsvModel> municipalities, CancellationToken cancellationToken)
    {
        var municipalitiesIdUpdated = new List<int>();
        var municipalityCsvModels = municipalities.ToDictionary(c => c.Id, y => y);
        var countiesDictionary = _context.Counties.ToDictionary(x => x.Code, y => y.Id);
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            foreach (var municipality in _context.Municipalities)
            {
                if (municipalityCsvModels.TryGetValue(municipality.Id, out var csvModel))
                {
                    municipality.Code = csvModel.Code;
                    municipality.Name = csvModel.Name;
                    municipality.Order = csvModel.Order;
                    municipality.CountyId = countiesDictionary[csvModel.CountyCode];
                    municipalitiesIdUpdated.Add(municipality.Id);
                }
            }

            foreach (var id in municipalityCsvModels.Keys.Except(municipalitiesIdUpdated))
            {
                var csvModel = municipalityCsvModels[id];

                var municipality = new Municipality
                {
                    Id = csvModel.Id,
                    Code = csvModel.Code,
                    Name = csvModel.Name,
                    Order = csvModel.Order,
                    CountyId = countiesDictionary[csvModel.CountyCode]

                };

                _context.Municipalities.Add(municipality);
            }

            await _context.SaveChangesAsync(cancellationToken);

            transaction.Commit();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Cannot add/update municipalities");
            return Result.Failure("Cannot add/update municipalities");
        }

        return Result.Success();
    }

    private Result<List<MunicipalityCsvModel>> ValidateData(List<MunicipalityCsvModel> municipalities)
    {
        if (municipalities.Count != municipalities.Select(x => x.Id).Distinct().Count())
        {
            return Result.Failure<List<MunicipalityCsvModel>>("Duplicated id in csv found");
        }

        var invalidMunicipality = municipalities.FirstOrDefault(x =>
            x == null
            || string.IsNullOrEmpty(x.CountyCode)
            || string.IsNullOrEmpty(x.Code)
            || string.IsNullOrEmpty(x.Name)
            || x.Name.Length > NameMaxLength
            || x.Code.Length > CodeMaxLength
            || x.CountyCode.Length > CodeMaxLength);

        if (invalidMunicipality != null)
        {
            return Result.Failure<List<MunicipalityCsvModel>>($"Municipality with invalid fields: {JsonConvert.SerializeObject(invalidMunicipality)}");
        }

        var counties = _context.Counties.Select(x => x.Code).ToHashSet();
        var municipalityWithInvalidCountyCode = municipalities.FirstOrDefault(x => !counties.Contains(x.CountyCode));

        if (municipalityWithInvalidCountyCode != null)
        {
            return Result.Failure<List<MunicipalityCsvModel>>($"Municipality with invalid county code: {JsonConvert.SerializeObject(municipalityWithInvalidCountyCode)}");
        }
        return Result.Success(municipalities);
    }

    private Result<List<MunicipalityCsvModel>> ReadFromCsv(CreateOrUpdateMunicipalities request)
    {
        List<MunicipalityCsvModel> municipalities;

        try
        {
            using var reader = new StreamReader(request.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            municipalities = csv.GetRecords<MunicipalityCsvModel>()
                .ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to read csv file");
            return Result.Failure<List<MunicipalityCsvModel>>("Cannot read csv file provided");
        }

        return Result.Success(municipalities);
    }

    public async Task<Result<List<MunicipalityModel>>> Handle(GetAllMunicipalitiesByCountyCode request, CancellationToken cancellationToken)
    {
        List<MunicipalityModel> municipalities;

        try
        {
            municipalities = await _cacheService.GetOrSaveDataInCacheAsync(
                $"county-{request.CountyCode}/municipalities", async () => await _context.Municipalities
                    .AsNoTracking()
                    .Where(x => x.County.Code == request.CountyCode)
                    .OrderBy(c => c.Order)
                    .Select(x => new MunicipalityModel
                    {
                        Id = x.Id,
                        Code = x.Code,
                        CountyCode = x.County.Code,
                        Name = x.Name,
                        Order = x.Order,
                        NumberOfPollingStations = x.PollingStations.Count
                    })
                    .ToListAsync(cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to load all municipalities");
            return Result.Failure<List<MunicipalityModel>>("Unable to load all municipalities");
        }

        return Result.Success(municipalities);
    }

    public async Task<Result<List<MunicipalityModelV2>>> Handle(GetAllMunicipalities request, CancellationToken cancellationToken)
    {
        List<MunicipalityModelV2> municipalities;

        try
        {
            municipalities = await _cacheService.GetOrSaveDataInCacheAsync("municipalities", async () =>
            {
                return await _context.Municipalities
                    .AsNoTracking()
                    .Include(m => m.County)
                    .OrderBy(c => c.Order)
                    .Select(x => new MunicipalityModelV2
                    {
                        Id = x.Id,
                        Code = x.Code,
                        CountyId = x.CountyId,
                        CountyCode = x.County.Code,
                        Diaspora = x.County.Diaspora,
                        CountyName = x.County.Name,
                        CountyOrder = x.County.Order,
                        Name = x.Name,
                        Order = x.Order,
                        NumberOfPollingStations = x.PollingStations.Count
                    })
                    .ToListAsync(cancellationToken);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to load all municipalities");
            return Result.Failure<List<MunicipalityModelV2>>("Unable to load all municipalities");
        }

        return Result.Success(municipalities);
    }

    public async Task<Result<MunicipalityModel>> Handle(GetMunicipalityById request, CancellationToken cancellationToken)
    {
        try
        {
            var municipality = await _context.Municipalities
                .Where(x => x.Id == request.MunicipalityId)
                .Select(c => new MunicipalityModel
                {
                    Id = request.MunicipalityId,
                    Code = c.Code,
                    Name = c.Name,
                    Order = c.Order,
                    NumberOfPollingStations = c.PollingStations.Count
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (municipality == null)
            {
                return Result.Failure<MunicipalityModel>($"Could not find municipality with id = {request.MunicipalityId}");
            }

            return Result.Success(municipality);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to load municipality {municipalityId}", request.MunicipalityId);
            return Result.Failure<MunicipalityModel>($"Unable to load municipality {request.MunicipalityId}");
        }
    }

    public async Task<Result> Handle(UpdateMunicipality request, CancellationToken cancellationToken)
    {
        try
        {
            var municipality = await _context.Municipalities.FirstOrDefaultAsync(x => x.Id == request.MunicipalityId, cancellationToken);
            if (municipality == null)
            {
                return Result.Failure($"Could not find municipality with id = {request.MunicipalityId}");
            }

            municipality.Code = request.Code;
            municipality.Name = request.Name;
            municipality.Order = request.Order;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to update municipality {municipalityId}", request.MunicipalityId);
            return Result.Failure($"Unable to update municipality {request.MunicipalityId}");
        }
    }
}
