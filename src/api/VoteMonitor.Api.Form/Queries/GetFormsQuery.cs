using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class GetFormsQuery: IRequest<IReadOnlyList<FormResponseModel>>
    {

    }
}
