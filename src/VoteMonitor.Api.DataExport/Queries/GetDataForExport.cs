using System;
using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.DataExport.Model;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GetDataForExport : IRequest<List<ExportModel>>
    {
        public int? NgoId { get; }
        public int? ObserverId { get; }
        public int? PollingStationNumber { get; }
        public string County { get; }
        public DateTime? From { get; }
        public DateTime? To { get; }
        public bool ApplyFilters { get; }

        private GetDataForExport(bool applyFilters)
        {
            ApplyFilters = applyFilters;
        }

        public GetDataForExport() : this(false)
        {

        }

        public GetDataForExport(int? ngoId, int? observerId, int? pollingStationNumber, string county, DateTime? from, DateTime? to) : this(true)
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