﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
	public class ObserverCountQueryHandler : IRequestHandler<ObserverCountCommand, int>
	{
		private readonly VoteMonitorContext _context;
		private readonly ILogger _logger;
		public ObserverCountQueryHandler(VoteMonitorContext context, ILogger logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task<int> Handle(ObserverCountCommand request, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Getting the total Observer count for the ngo with the id {request.IdNgo}");

            IQueryable<Entities.Observer> observers = _context.Observers;

            if (request.IdNgo > 0)
                observers = observers.Where(o => o.IdNgo == request.IdNgo);

            return await observers.CountAsync(cancellationToken);
        }

		private static IQueryable<Entities.Observer> GetPagedQuery(IQueryable<Entities.Observer> observers, int page, int pageSize)
		{
			if (pageSize > 0)
			{
				return observers
					.Skip(pageSize * (page - 1))
					.Take(pageSize);
			}

			return observers;
		}
	}
}
