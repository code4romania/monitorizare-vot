using MediatR;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GetExcelDbCommand : IRequest<byte[]>
    {
    }
}
