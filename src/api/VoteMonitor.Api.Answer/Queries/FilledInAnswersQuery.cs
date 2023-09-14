using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Queries;

public record FilledInAnswersQuery(int ObserverId, int PollingStationId) : IRequest<List<QuestionDto<FilledInAnswerDto>>>;
