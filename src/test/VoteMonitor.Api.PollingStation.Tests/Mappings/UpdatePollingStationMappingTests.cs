using AutoMapper;
using FluentAssertions;
using VoteMonitor.Api.PollingStation.Profiles;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Mappings;

public class UpdatePollingStationMappingTests
{
    private readonly IMapper _mapper;

    public UpdatePollingStationMappingTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollingStationProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void UpdatePollingStations_WhenMappingToUpdatePollingStationQuery_ReturnsNotNull()
    {
        var updatePollingStations = new Models.UpdatePollingStationModel();

        var result = _mapper.Map<Queries.UpdatePollingStation>(updatePollingStations);

        result.Should().NotBeNull();

    }

    [Fact]
    public void UpdatePollingStations_WhenMappingToUpdatePollingStationQuery_MapsAddress()
    {
        var updatePollingStations = new Models.UpdatePollingStationModel
        {
            Address = "street x"
        };

        var result = _mapper.Map<Queries.UpdatePollingStation>(updatePollingStations);

        result.Address.Should().Be("street x");
    }

    [Fact]
    public void UpdatePollingStations_WhenMappingToUpdatePollingStationQuery_MapsNumber()
    {
        var updatePollingStations = new Models.UpdatePollingStationModel
        {
            Number = 11
        };

        var result = _mapper.Map<Queries.UpdatePollingStation>(updatePollingStations);

        result.Number.Should().Be(11);
    }
}