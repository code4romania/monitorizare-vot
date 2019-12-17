using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class FetchAllOptionsCommand : IRequest<List<OptionDto>>
    {

    }
}
