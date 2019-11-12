using System;
using MediatR;
using VoteMonitor.Api.DataExport.Model;

namespace VoteMonitor.Api.DataExport.Queries
{
	public class GetDataForExport : IRequest<ExportModel>
	{
		public int? NgoId { get; }
		public int? ObserverId { get; }
		public int? PollingStationNumber { get; }
		public string County { get; }
		public DateTime? From { get; }
		public DateTime? To { get; }

		public GetDataForExport(int? ngoId, int? observerId, int? pollingStationNumber, string county, DateTime? from, DateTime? to)
		{
			NgoId = ngoId;
			ObserverId = observerId;
			PollingStationNumber = pollingStationNumber;
			County = county;
			From = from;
			To = to;
		}
	}
}