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
using Microsoft.Extensions.Logging.Internal;
using Moq;
using VoteMonitor.Api.PollingStation.Handlers;
using VoteMonitor.Api.PollingStation.Profiles;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests
{
    public class GetPollingStationsHandlerTests
    {
        private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly Mock<ILogger> _mockLogger;

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

            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public async Task Handle_WhenRetrievingPollingStationEntities_MapsResults()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.PollingStations.Add(new Entities.PollingStation
                {
                    Id = 1,
                    Address = "str X no 5",
                    AdministrativeTerritoryCode = "code",
                    Coordinates = "90.91",
                    IdCounty = 1,
                    Number = 3
                });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var sut = new GetPollingStationsHandler(context, new Mapper(_mapperConfiguration), _mockLogger.Object);

                var result = await sut.Handle(new GetAllPollingStations(), new CancellationToken());

                result.Should().BeOfType<List<Models.PollingStation>>();
                result.Count().Should().Be(1);
            }
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldLogError()
        {
            var mockContext = new Mock<VoteMonitorContext>(_dbContextOptions);
            mockContext.Setup(m => m.PollingStations).Throws(new Exception());
            var sut = new GetPollingStationsHandler(mockContext.Object, new Mapper(_mapperConfiguration), _mockLogger.Object);

            await Record.ExceptionAsync(async () => await sut.Handle(new GetAllPollingStations(), new CancellationToken()));

            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
    }
}
