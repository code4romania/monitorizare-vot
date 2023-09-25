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

public class UpdatePollingStationsHandlerTests
{
    private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
    private readonly Mock<ILogger<UpdatePollingStationsHandler>> _mockLogger;

    public UpdatePollingStationsHandlerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _mockLogger = new Mock<ILogger<UpdatePollingStationsHandler>>();
    }

    [Fact]
    public async Task Handle_WhenPollingStationNotFound_ReturnsFalse()
    {
        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new UpdatePollingStationsHandler(context, _mockLogger.Object);

            var requestNonExistingPollingStation = new UpdatePollingStation(5, "new address", 1);
            var result = await sut.Handle(requestNonExistingPollingStation, new CancellationToken());

            result.Should().Be(false);
        }
    }
        
    [Fact]
    public async Task Handle_WhenPollingStationFound_ReturnsTrue()
    {
        SetupContextWithPollingStation(new PollingStationBuilder().WithId(5).Build());

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new UpdatePollingStationsHandler(context, _mockLogger.Object);

            var requestExistingPollingStation = new UpdatePollingStation(5, "new address", 1);
            var result = await sut.Handle(requestExistingPollingStation, new CancellationToken());

            result.Should().Be(true);
        }
    }

    [Fact]
    public async Task Handle_WhenContextThrowsException_ReturnsNull()
    {
        var mockContext = new Mock<VoteMonitorContext>(_dbContextOptions);
        mockContext.Setup(m => m.PollingStations).Throws(new Exception());

        var sut = new UpdatePollingStationsHandler(mockContext.Object, _mockLogger.Object);

        var requestNonExistingPollingStation = new UpdatePollingStation(5, "new address", 1);

        bool? result = false;
        await Record.ExceptionAsync(async () => result = await sut.Handle(requestNonExistingPollingStation, new CancellationToken()));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenContextThrowsException_ErrorIsLogged()
    {
        var mockContext = new Mock<VoteMonitorContext>(_dbContextOptions);
        mockContext.Setup(m => m.PollingStations).Throws(new Exception());

        var sut = new UpdatePollingStationsHandler(mockContext.Object, _mockLogger.Object);

        var requestNonExistingPollingStation = new UpdatePollingStation(5, "new address", 1);

        await Record.ExceptionAsync(async () => await sut.Handle(requestNonExistingPollingStation, new CancellationToken()));

        _mockLogger.Verify(x => x.Log(
            LogLevel.Error, 
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPollingStationFound_UpdatesAddress()
    {
        var id = 3;
        SetupContextWithPollingStation(new PollingStationBuilder().WithId(id).WithAddress("old address").Build());

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new UpdatePollingStationsHandler(context, _mockLogger.Object);

            var requestExistingPollingStation = new UpdatePollingStation(id, "new address", 1);
            await sut.Handle(requestExistingPollingStation, new CancellationToken());

            var updatedPollingStation = context.PollingStations.First(p => p.Id == id);
            updatedPollingStation.Address.Should().Be("new address");
        }
    }

    [Fact]
    public async Task Handle_WhenPollingStationFound_UpdatesNumber()
    {
        var id = 3;
        SetupContextWithPollingStation(new PollingStationBuilder().WithId(id).WithNumber(13).Build());

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new UpdatePollingStationsHandler(context, _mockLogger.Object);

            var requestExistingPollingStation = new UpdatePollingStation(id, "new address", 27);
            await sut.Handle(requestExistingPollingStation, new CancellationToken());

            var updatedPollingStation = context.PollingStations.First(p => p.Id == id);
            updatedPollingStation.Number.Should().Be(27);
        }
    }
        
    private void SetupContextWithPollingStation(Entities.PollingStation pollingStation)
    {
        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            context.PollingStations.Add(pollingStation);
            context.SaveChanges();
        }
    }
}
