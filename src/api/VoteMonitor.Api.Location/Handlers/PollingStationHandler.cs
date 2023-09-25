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
                var municipalities = _context.Municipalities.ToList();

                var pollingStations = ParseUploadedPollingStations(request.File);
                var newPollingStations = CreatePollingStationEntitiesFromDto(pollingStations, municipalities);
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
        var pollingStationCsvModels = csv.GetRecords<PollingStationCsvModel>()
            .ToList();

        return pollingStationCsvModels;
    }

    private List<PollingStation> CreatePollingStationEntitiesFromDto(List<PollingStationCsvModel> pollingStationDtos, List<Municipality> municipalities)
    {
        var startingPsId = _context.PollingStations.Any() ? _context.PollingStations.Max(ps => ps.Id) + 1 : 1;

        var newPollingStations = new List<PollingStation>();
        foreach (var record in pollingStationDtos)
        {
            var municipality = municipalities.FirstOrDefault(x => x.Code.Equals(record.MunicipalityCode, StringComparison.OrdinalIgnoreCase));
            if (municipality == null)
            {
                throw new PollingStationImportException($"Municipality {record.MunicipalityCode} not found in the database");
            }

            var pollingStation = new PollingStation
            {
                Id = startingPsId++,
                MunicipalityId = municipality.Id,
                Address = record.Address,
                Number = record.Number
            };

            newPollingStations.Add(pollingStation);

        }

        return newPollingStations;
    }
}
