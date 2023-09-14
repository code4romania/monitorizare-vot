using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Queries;

public record FormAnswersQuery(int ObserverId, int PollingStationId) : IRequest<PollingStationInfoDto>;
