using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.PollingStation.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class ClearAllPollingStationHandler : IRequestHandler<ClearAllPollingStationsCommand, Result>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger<ClearAllPollingStationHandler> _logger;

        public ClearAllPollingStationHandler(VoteMonitorContext context, ILogger<ClearAllPollingStationHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> Handle(ClearAllPollingStationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    if (request.IncludeRelatedData)
                    {
                        await DeleteAnswersData(cancellationToken);
                        await DeleteNotesAttachments(cancellationToken);
                        await DeleteNotes(cancellationToken);
                        await DeletePollingStationsInfo(cancellationToken);
                    }

                    await ResetCountiesPollingStationCounter(cancellationToken);
                    await DeletePollingStations(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return Result.Success();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while removing polling stations.", ex);
                return Result.Failure("Cannot remove polling stations.");
            }
        }

        private async Task DeleteAnswersData(CancellationToken cancellationToken)
        {
            await DeleteDataFromTable(nameof(_context.Answers), cancellationToken);
        }
        private async Task DeleteNotesAttachments(CancellationToken cancellationToken)
        {
            await DeleteDataFromTable(nameof(_context.NotesAttachments), cancellationToken);
        }
        private async Task DeleteNotes(CancellationToken cancellationToken)
        {
            await DeleteDataFromTable(nameof(_context.Notes), cancellationToken);
        }
        private async Task DeletePollingStationsInfo(CancellationToken cancellationToken)
        {
            await DeleteDataFromTable(nameof(_context.PollingStationInfos), cancellationToken);
        }
        private async Task ResetCountiesPollingStationCounter(CancellationToken cancellationToken)
        {
            await _context.Database.ExecuteSqlRawAsync($"UPDATE {nameof(_context.Counties)} SET {nameof(County.NumberOfPollingStations)} = 0"
                                        , cancellationToken);
        }
        private async Task DeletePollingStations(CancellationToken cancellationToken)
        {
            await DeleteDataFromTable(nameof(_context.PollingStations), cancellationToken);
        }

        private async Task DeleteDataFromTable(string tableName, CancellationToken cancellationToken)
        {
            //if there are any performance issues, DELETE can be changed to TRUNCATE - but then we need to tackle a way to 
            //bypass foreign key relationship while removing data
            await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}"
                                    , cancellationToken);
        }
    }
}
