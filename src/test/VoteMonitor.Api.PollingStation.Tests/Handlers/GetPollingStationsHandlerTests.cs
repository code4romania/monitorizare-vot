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

public class GetPollingStationsHandlerTests
{
    private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
    private readonly Mock<ILogger<GetPollingStationsHandler>> _mockLogger;

    public GetPollingStationsHandlerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _mockLogger = new Mock<ILogger<GetPollingStationsHandler>>();
    }

    [Fact]
    public async Task Handle_WithDefaultPage_ReturnsFirstPageResults()
    {
        SetupContextWithPollingStations(new List<Entities.PollingStation>
        {
            new PollingStationBuilder().WithId(1).Build()
        });

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new GetPollingStationsHandler(context, _mockLogger.Object);

            var getPollingStations = new GetPollingStations(CountyId: 0, Page: 1, PageSize: 10);
            var result = await sut.Handle(getPollingStations, new CancellationToken());

            result.Count().Should().Be(1);
            result.First().Id.Should().Be(1);
        }
    }

    [Fact]
    public async Task Handle_WithSecondPage_ReturnsSecondPageResults()
    {
        SetupContextWithMunicipalities(new[]
        {
            new Municipality()
            {
                Id = 13,
                Code = "m",
                Name = "municipality",
                County = new County()
                {
                    Id = 10,
                    Code = "c",
                    Name = "county",
                }
            }
        });
        SetupContextWithPollingStations(new List<Entities.PollingStation>
        {
            new PollingStationBuilder().WithId(1).WithMunicipalityId(13).Build(),
            new PollingStationBuilder().WithId(2).WithMunicipalityId(13).Build()
        });

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new GetPollingStationsHandler(context, _mockLogger.Object);
            var getPollingStations = new GetPollingStations(CountyId: 10, Page: 2, PageSize: 1);
            var result = await sut.Handle(getPollingStations, new CancellationToken());

            result.Count().Should().Be(1);
            result.First().Id.Should().Be(2);
        }
    }

    [Fact]
    public async Task Handle_WithIdCounty_ReturnsCorrectResults()
    {
        SetupContextWithMunicipalities(new[]
        {
            new Municipality()
            {
                Id = 13,
                Code = "m1",
                Name = "municipality1",
                County = new County()
                {
                    Id = 10,
                    Code = "c1",
                    Name = "county1",
                }
            },
            new Municipality()
            {
                Id = 14,
                Code = "m2",
                Name = "municipality2",
                County = new County()
                {
                    Id = 20,
                    Code = "c2",
                    Name = "county2",
                }
            }
        });

        SetupContextWithPollingStations(new List<Entities.PollingStation>
        {
            new PollingStationBuilder().WithId(1).WithMunicipalityId(13).Build(),
            new PollingStationBuilder().WithId(2).WithMunicipalityId(14).Build()
        });

        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            var sut = new GetPollingStationsHandler(context, _mockLogger.Object);

            var getPollingStations = new GetPollingStations(CountyId: 20, Page: 1, PageSize: 1);
            var result = await sut.Handle(getPollingStations, new CancellationToken());

            result.Count().Should().Be(1);
            result.First().Id.Should().Be(2);
        }
    }

    [Fact]
    public async Task Handle_WhenExceptionIsThrown_ShouldLogError()
    {
        var mockContext = new Mock<VoteMonitorContext>(_dbContextOptions);
        mockContext.Setup(m => m.PollingStations).Throws(new Exception());
        var sut = new GetPollingStationsHandler(mockContext.Object, _mockLogger.Object);

        await Record.ExceptionAsync(async () => await sut.Handle(new GetPollingStations(CountyId: 0, Page: 1, PageSize: 10), new CancellationToken()));

        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    private void SetupContextWithMunicipalities(IEnumerable<Municipality> municipalities)
    {
        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            context.Municipalities.AddRange(municipalities);
            context.SaveChanges();
        }
    }
    private void SetupContextWithPollingStations(IEnumerable<Entities.PollingStation> pollingStations)
    {
        using (var context = new VoteMonitorContext(_dbContextOptions))
        {
            context.PollingStations.AddRange(pollingStations);
            context.SaveChanges();
        }
    }
}
