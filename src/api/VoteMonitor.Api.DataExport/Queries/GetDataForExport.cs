using MediatR;
using System;
using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GetDataForExport : IRequest<IEnumerable<ExportModel>>
    {
        public int? NgoId { get; set; }
        public int? ObserverId { get; set; }
        public int? PollingStationNumber { get; set; }
        public string County { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public bool ApplyFilters => NgoId.HasValue || ObserverId.HasValue || PollingStationNumber.HasValue ||
                                    From.HasValue || To.HasValue || !string.IsNullOrEmpty(County);
    }
}