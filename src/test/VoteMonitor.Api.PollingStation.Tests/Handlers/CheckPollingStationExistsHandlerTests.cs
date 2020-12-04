using System;
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
    public class CheckPollingStationExistsHandlerTests
    {
        private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
        private readonly Mock<ILogger<CheckPollingStationExistsHandler>> _mockLogger;

        public CheckPollingStationExistsHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mockLogger = new Mock<ILogger<CheckPollingStationExistsHandler>>();
        }

        [Fact]
        public async Task Handler_WhenPollingStationExists_ReturnsTrue()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.PollingStations.Add(new PollingStationBuilder().WithId(3).Build());
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var sut = new CheckPollingStationExistsHandler(context, _mockLogger.Object);
                var checkPollingStationExists = new CheckPollingStationExists()
                {
                    PollingStationId = 3
                };

                var result = await sut.Handle(checkPollingStationExists, new CancellationToken());

                result.Should().Be(true);
            }
        }

        [Fact]
        public async Task Handler_WhenPollingStationDoes_NotExist_ReturnsFalse()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var sut = new CheckPollingStationExistsHandler(context, _mockLogger.Object);
                var checkPollingStationExists = new CheckPollingStationExists()
                {
                    PollingStationId = 3
                };

                var result = await sut.Handle(checkPollingStationExists, new CancellationToken());

                result.Should().Be(false);
            }
        }
    }
}
