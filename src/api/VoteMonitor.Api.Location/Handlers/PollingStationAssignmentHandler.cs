using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationAssignmentHandler : IRequestHandler<PollingStationsAssignmentQuery, IEnumerable<CountyPollingStationLimit>>
	{
		private readonly IPollingStationService _pollingStationService;
		private readonly PollingStationsOptions _options;

		public PollingStationAssignmentHandler(IPollingStationService pollingStationService, IOptions<PollingStationsOptions> options)
		{
			_pollingStationService = pollingStationService;
			_options = options.Value;
		}

		public async Task<IEnumerable<CountyPollingStationLimit>> Handle(PollingStationsAssignmentQuery message, CancellationToken cancellationToken)
		{
			var counties = await _pollingStationService.GetPollingStationsAssignmentsForAllCounties(message.Diaspora);

			if (_options.OverrideDefaultSorting)
			{
				return counties.OrderByDescending(o => o.Code == _options.CodeOfFirstToDisplayCounty).ThenBy(o => o.Code);
			}

			return counties;
		}
	}
}
