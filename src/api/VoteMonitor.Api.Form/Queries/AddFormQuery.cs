using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class AddFormQuery : IRequest<FormDTO>
    {
        public FormDTO Form { get; set; }
    }
}
