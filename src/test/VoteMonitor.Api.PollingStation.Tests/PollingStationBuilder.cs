using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Tests;

internal class PollingStationBuilder
{
    private int _id = 1;
    private int _municipalityId = 10;
    private string _address = "str X no 5";
    private int _number = 15;
    private int _countyId = 1;

    public Entities.PollingStation Build()
    {
        return new Entities.PollingStation
        {
            Id = _id,
            MunicipalityId = _municipalityId,
            Municipality = new Municipality
            {
                Id = _municipalityId,
                Code = $"{_municipalityId}Code",
                Name = $"{_municipalityId}Name",
                County = new County
                {
                    Id = _countyId,
                    Code = $"{_countyId}Code",
                    Name = $"{_countyId}Name",
                }
            },
            Address = _address,
            Number = _number
        };
    }

    public PollingStationBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PollingStationBuilder WithMunicipalityId(int countyId, int municipalityId)
    {
        _countyId = countyId;
        _municipalityId = municipalityId;
        return this;
    }

    public PollingStationBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public PollingStationBuilder WithNumber(int number)
    {
        _number = number;
        return this;
    }
}
