using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries;

public record FormQuestionQuery(int FormId, int CacheHours, int CacheMinutes, int CacheSeconds) : IRequest<IEnumerable<FormSectionDTO>>;
