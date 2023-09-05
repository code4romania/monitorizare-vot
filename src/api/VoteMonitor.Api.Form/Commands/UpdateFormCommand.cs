using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Commands;

public class UpdateFormCommand : IRequest<FormDTO> 
{
    public FormDTO Form { get; set; }
    public int Id { get; set; }
}