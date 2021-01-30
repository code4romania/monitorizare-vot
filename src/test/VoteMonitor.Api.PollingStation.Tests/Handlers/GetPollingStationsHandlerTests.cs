using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using VoteMonitor.Api.PollingStation.Handlers;
using VoteMonitor.Api.PollingStation.Profiles;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Handlers
{
    public class GetPollingStationsHandlerTests
    {
        private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly Mock<ILogger<GetPollingStationsHandler>> _mockLogger;

        public GetPollingStationsHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PollingStationProfile>();
            });

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
                var sut = new GetPollingStationsHandler(context, new Mapper(_mapperConfiguration), _mockLogger.Object);

                var getPollingStations = new GetPollingStations
                {
                    PageSize = 10
                };
                var result = await sut.Handle(getPollingStations, new CancellationToken());

                result.Count().Should().Be(1);
                result.First().Id.Should().Be(1);
            }
        }

        [Fact]
        public async Task Handle_WithSecondPage_ReturnsSecondPageResults()
        {
            SetupContextWithPollingStations(new List<Entities.PollingStation>
            {
                new PollingStationBuilder().WithId(1).Build(),
                new PollingStationBuilder().WithId(2).Build()
            });

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var sut = new GetPollingStationsHandler(context, new Mapper(_mapperConfiguration), _mockLogger.Object);

                var getPollingStations = new GetPollingStations
                {
                    Page = 2,
                    PageSize = 1
                };
                var result = await sut.Handle(getPollingStations, new CancellationToken());

                result.Count().Should().Be(1);
                result.First().Id.Should().Be(2);
            }
        }

        [Fact]
        public async Task Handle_WithIdCounty_ReturnsCorrectResults()
        {
            SetupContextWithPollingStations(new List<Entities.PollingStation>
            {
                new PollingStationBuilder().WithId(1).WithIdCounty(5).Build(),
                new PollingStationBuilder().WithId(2).WithIdCounty(20).Build()
            });

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var sut = new GetPollingStationsHandler(context, new Mapper(_mapperConfiguration), _mockLogger.Object);

                var getPollingStations = new GetPollingStations
                {
                    CountyId = 20,
                    Page = 1,
                    PageSize = 1
                };
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
            var sut = new GetPollingStationsHandler(mockContext.Object, new Mapper(_mapperConfiguration), _mockLogger.Object);

            await Record.ExceptionAsync(async () => await sut.Handle(new GetPollingStations(), new CancellationToken()));

            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
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
}
