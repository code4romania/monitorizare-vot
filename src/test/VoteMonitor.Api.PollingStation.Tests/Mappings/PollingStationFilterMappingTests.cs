using AutoMapper;
using FluentAssertions;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Profiles;
using VoteMonitor.Api.PollingStation.Queries;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Mappings
{
    public class PollingStationFilterMappingTests
    {
        private readonly IMapper _mapper;

        public PollingStationFilterMappingTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PollingStationProfile>();
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void PollingStationsFilter_WhenMappingToGetPollingStations_ReturnsNonNull()
        {
            var pollingStationFilter = new PollingStationsFilterModel();

            var getPollingStations = _mapper.Map<GetPollingStations>(pollingStationFilter);

            getPollingStations.Should().NotBeNull();
        }

        [Fact]
        public void PollingStationsFilter_WhenMappingToGetPollingStations_MapsIdCountyCorrectly()
        {
            var pollingStationFilter = new PollingStationsFilterModel
            {
                CountyId = 15
            };

            var getPollingStations = _mapper.Map<GetPollingStations>(pollingStationFilter);

            getPollingStations.CountyId.Should().Be(15);
        }

        [Fact]
        public void PollingStationsFilter_WhenMappingToGetPollingStations_MapsPageCorrectly()
        {
            var pollingStationFilter = new PollingStationsFilterModel
            {
                Page = 2
            };

            var getPollingStations = _mapper.Map<GetPollingStations>(pollingStationFilter);

            getPollingStations.Page.Should().Be(2);
        }

        [Fact]
        public void PollingStationsFilter_WhenMappingToGetPollingStations_MapsPageSizeCorrectly()
        {
            var pollingStationFilter = new PollingStationsFilterModel
            {
                PageSize = 20
            };

            var getPollingStations = _mapper.Map<GetPollingStations>(pollingStationFilter);

            getPollingStations.PageSize.Should().Be(20);
        }
    }
}
