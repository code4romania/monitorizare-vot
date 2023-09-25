using MediatR;
using VoteMonitor.Api.Statistics.Models;

namespace VoteMonitor.Api.Statistics.Queries;

public record CountiesVisitedQuery : IRequest<SimpleStatisticsModel>;
