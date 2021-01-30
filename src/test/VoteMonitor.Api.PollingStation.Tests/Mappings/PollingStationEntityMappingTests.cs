using AutoMapper;
using FluentAssertions;
using VoteMonitor.Api.PollingStation.Profiles;
using Xunit;

namespace VoteMonitor.Api.PollingStation.Tests.Mappings
{
    public class PollingStationEntityMappingTests
    {
        private readonly MapperConfiguration _configuration;
        private readonly IMapper _mapper;

        public PollingStationEntityMappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PollingStationProfile>();
            });
            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void AutoMapperConfiguration_IsValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_ReturnsNonNull()
        {
            var pollingStationEntity = new Entities.PollingStation();

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.Should().NotBeNull();
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsIdCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                Id = 1
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.Id.Should().Be(1);
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsAddressCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                Address = "str X bloc 4"
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.Address.Should().Be("str X bloc 4");
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsAdministrativeTerritoryCodeCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                AdministrativeTerritoryCode = "IS"
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.AdministrativeTerritoryCode.Should().Be("IS");
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsTerritoryCodeCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                TerritoryCode = "code"
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.TerritoryCode.Should().Be("code");
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsCoordinatesCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                Coordinates = "90.91"
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.Coordinates.Should().Be("90.91");
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsNumberCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                Number = 5
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.Number.Should().Be(5);
        }

        [Fact]
        public void PollingStationEntity_WhenMappingToPollingStationModel_MapsIdCountyCorrectly()
        {
            var pollingStationEntity = new Entities.PollingStation
            {
                IdCounty = 3
            };

            var pollingStationModel = _mapper.Map<Models.GetPollingStationModel>(pollingStationEntity);

            pollingStationModel.IdCounty.Should().Be(3);
        }
    }
}
