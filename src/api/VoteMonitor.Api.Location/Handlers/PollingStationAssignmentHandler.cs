using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Options;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
	public class PollingStationAssignmentHandler : AsyncRequestHandler<PollingStationsAssignmentQuery, IEnumerable<CountyPollingStationLimit>>
	{
		private readonly IPollingStationService _pollingStationService;
		private readonly PollingStationsOptions _options;

		public PollingStationAssignmentHandler(IPollingStationService pollingStationService, IOptions<PollingStationsOptions> options)
		{
			_pollingStationService = pollingStationService;
			_options = options.Value;
		}

		protected override async Task<IEnumerable<CountyPollingStationLimit>> HandleCore(PollingStationsAssignmentQuery message)
		{
			var counties = await _pollingStationService.GetPollingStationsAssignmentsForAllCounties();

			if (_options.OverrideDefaultSorting)
			{
				return counties.OrderByDescending(o => o.Code == _options.CodeOfFirstToDisplayCounty).ThenBy(o => o.Code);
			}

			return counties;
		}
	}
}
