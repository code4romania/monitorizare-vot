using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.DataExport.Model;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Handlers
{
	public class DataExportQueryHandler: IRequestHandler<GetDataForExport, ExportModel>
	{
		public Task<ExportModel> Handle(GetDataForExport request, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}