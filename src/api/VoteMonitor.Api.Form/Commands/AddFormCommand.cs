using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Commands;

public class AddFormCommand : IRequest<FormDTO>
{
    public FormDTO Form { get; set; }
}