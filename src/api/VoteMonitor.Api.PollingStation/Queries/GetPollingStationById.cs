using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Queries;

public record GetPollingStationById(int PollingStationId) : IRequest<GetPollingStationModel>;
