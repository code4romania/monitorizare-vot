using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Commands;

public record UpdateFormCommand(FormDTO Form, int Id) : IRequest<FormDTO>;
