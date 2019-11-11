using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
	public class FormVersionQueryHandler : AsyncRequestHandler<FormVersionQuery, List<Entities.Form>>
	{

		private readonly VoteMonitorContext _context;
		private readonly IMapper _mapper;
		private readonly ICacheService _cacheService;

		public FormVersionQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
		{
			_context = context;
			_mapper = mapper;
			_cacheService = cacheService;
		}

		protected override async Task<List<Entities.Form>> HandleCore(FormVersionQuery request)
		{
			bool bringAllForms = request.Diaspora == null || request.Diaspora == true;

			var result = await _context.Forms
				.AsNoTracking()
				.Where(x => bringAllForms || x.Diaspora == false)
				.Where(x=>x.Draft == false)
				.ToListAsync();


			// quick and dirty fix better/cleaner logic will be done in /apo/v2/form

			var sortedForms = result.Select(x => new { FormLetter = GetFormLetter(x.Code), VotingDay = GetVotingDay(x.Code), Form = x })
					.OrderBy(x => x.VotingDay)
					.ThenBy(x => x.FormLetter)
					.Select(x => x.Form)
					.ToList();

			return sortedForms;
		}

		private int GetVotingDay(string code)
		{
			int defaultVotingDay = 9999;

			if (code.Length == 1)
			{
				return defaultVotingDay;
			}

			if (int.TryParse(code.Substring(1, 1), out var votingDay))
			{
				return votingDay;
			}

			return defaultVotingDay;

		}

		private string GetFormLetter(string code)
		{
			if (code.Length == 1)
				return code;

			return code.Substring(0, 1);
		}
	}
}
