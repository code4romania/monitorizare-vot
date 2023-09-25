using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Commands;

public record AddFormCommand(FormDTO Form) : IRequest<FormDTO>;
