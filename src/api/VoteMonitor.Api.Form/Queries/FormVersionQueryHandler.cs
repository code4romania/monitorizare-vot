﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
	public class FormVersionQueryHandler : AsyncRequestHandler<FormVersionQuery, List<Entities.Form>>
	{
		private readonly VoteMonitorContext _context;

		public FormVersionQueryHandler(VoteMonitorContext context)
		{
			_context = context;
		}

		protected override async Task<List<Entities.Form>> HandleCore(FormVersionQuery request)
		{
			var bringAllForms = request.Diaspora == null || request.Diaspora == true;

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

		private static int GetVotingDay(string code)
		{
			const int defaultVotingDay = 9999;

			if (code.Length == 1)
				return defaultVotingDay;

			return int.TryParse(code.Substring(1, 1), out var votingDay) ? votingDay : defaultVotingDay;
        }

		//private static string GetFormLetter(string code) => code.Length == 1 ? code : code.Substring(0, 1);
		private static string GetFormLetter(string code) => string.IsNullOrEmpty(code) ? "" : code.Substring(0, 1);
    }
}
