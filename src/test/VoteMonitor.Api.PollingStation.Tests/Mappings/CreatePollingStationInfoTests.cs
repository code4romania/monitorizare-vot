using AutoMapper;
using FluentAssertions;
using VoteMonitor.Api.PollingStation.Profiles;
using VoteMonitor.Api.PollingStation.Queries;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Mappings
{
    public class CreatePollingStationInfoTests
    {
        private readonly IMapper _mapper;

        public CreatePollingStationInfoTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PollingStationInfoProfile>();
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void CreatePollingStationInfo_WhenMappingToPollingStationInfoEntity_ReturnsNotNull()
        {
            var pollingStationFilter = new CreatePollingStationInfo();

            var pollingStationInfo = _mapper.Map<Entities.PollingStationInfo>(pollingStationFilter);

            pollingStationInfo.Should().NotBeNull();
        }

        [Fact]
        public void CreatePollingStationInfo_WhenMappingToPollingStationInfoEntity_MapsCountryCode()
        {
            var pollingStationFilter = new CreatePollingStationInfo()
            {
                CountyCode = "RO"
            };

            var pollingStationInfo = _mapper.Map<Entities.PollingStationInfo>(pollingStationFilter);

            pollingStationInfo.Should().NotBeNull();
        }
    }
}
