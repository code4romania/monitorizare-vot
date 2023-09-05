using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Exceptions;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Models.ResultValues;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers;

public class PollingStationHandler : IRequestHandler<ImportPollingStationsCommand, PollingStationImportResultValue>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public PollingStationHandler(VoteMonitorContext context, ILogger<PollingStationHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PollingStationImportResultValue> Handle(ImportPollingStationsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                var countiesFromDatabase = _context.Counties.ToList();

                var pollingStations = ParseUploadedPollingStations(request.File);
                var newPollingStations = CreatePollingStationEntitiesFromDto(pollingStations, countiesFromDatabase);
                await  _context.PollingStations.BulkInsertAsync(newPollingStations, cancellationToken);

                await _context.BulkSaveChangesAsync(cancellationToken);

                transaction.Commit();
                return PollingStationImportResultValue.SuccessValue;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while importing polling station information ");
            return new PollingStationImportResultValue(ex);
        }
    }

    private List<PollingStationCsvModel> ParseUploadedPollingStations(IFormFile requestFile)
    {
        using var reader = new StreamReader(requestFile.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var counties = csv.GetRecords<PollingStationCsvModel>()
            .ToList();

        return counties;
    }

    private List<PollingStation> CreatePollingStationEntitiesFromDto(List<PollingStationCsvModel> pollingStationDtos, List<County> countiesFromDatabase)
    {
        var startingPsId = _context.PollingStations.Any() ? _context.PollingStations.Max(ps => ps.Id) + 1 : 1;

        var newPollingStations = new List<PollingStation>();
        foreach (var record in pollingStationDtos)
        {
            var countyForPollingStation = countiesFromDatabase.FirstOrDefault(x => x.Code.Equals(record.CountyCode, StringComparison.OrdinalIgnoreCase));
            if (countyForPollingStation == null)
            {
                throw new PollingStationImportException($"County {record.CountyCode} not found in the database");
            }

            var pollingStation = new PollingStation
            {
                Id = startingPsId++,
                IdCounty = countyForPollingStation.Id,
                Address = record.Address,
                Number = record.Number
            };

            newPollingStations.Add(pollingStation);

        }

        return newPollingStations;
    }
}
