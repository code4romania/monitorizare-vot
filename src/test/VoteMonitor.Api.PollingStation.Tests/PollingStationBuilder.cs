namespace VoteMonitor.Api.PollingStation.Tests;

internal class PollingStationBuilder
{
    private int _id = 1;
    private int _municipalityId = 10;
    private string _address = "str X no 5";
    private int _number = 15;

    public Entities.PollingStation Build()
    {
        return new Entities.PollingStation
        {
            Id = _id,
            MunicipalityId = _municipalityId,
            Address = _address,
            Number = _number
        };
    }

    public PollingStationBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PollingStationBuilder WithMunicipalityId(int municipalityId)
    {
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
