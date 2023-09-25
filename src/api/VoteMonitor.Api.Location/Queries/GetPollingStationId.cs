using MediatR;

namespace VoteMonitor.Api.Location.Queries;

public record GetPollingStationId(string CountyCode, string MunicipalityCode, int PollingStationNumber) : IRequest<int>;
