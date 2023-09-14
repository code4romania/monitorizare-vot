using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using VoteMonitor.Api.PollingStation.Handlers;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Handlers;

public class CreatePollingStationInfoHandlerTests
{
    private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
    private readonly Mock<ILogger<CreatePollingStationInfoHandler>> _mockLogger;

    public CreatePollingStationInfoHandlerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _mockLogger = new Mock<ILogger<CreatePollingStationInfoHandler>>();
    }

    [Fact]
    public async Task Handler_CreatesPollingStationInfo()
    {
        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new CreatePollingStationInfoHandler(context, _mockLogger.Object);

            var createPollingStationInfo = new CreatePollingStationInfo(ObserverId: 1,
                PollingStationId: 3,
                CountyCode: "county1",
                ObserverArrivalTime: DateTime.UtcNow,
                ObserverLeaveTime: DateTime.UtcNow,
                IsPollingStationPresidentFemale: false);

            await sut.Handle(createPollingStationInfo, new CancellationToken());

            var savedPollingStationInfo = context.PollingStationInfos.FirstOrDefault(p => p.IdPollingStation == createPollingStationInfo.PollingStationId);
            savedPollingStationInfo.Should().NotBeNull();
        }

    }
}
