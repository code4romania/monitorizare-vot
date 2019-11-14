using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.DataExport.Model;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Handlers
{
	public class DataExportQueryHandler: IRequestHandler<GetDataForExport, List<ExportModel>>
	{
		public Task<List<ExportModel>> Handle(GetDataForExport request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<ExportModel>());
        }
	}
}