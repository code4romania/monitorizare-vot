using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Api.Statistics.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Statistics.Handlers
{
    public class StatisticsQueryHandler :
        IRequestHandler<StatisticsObserverNumberQuery, ApiListResponse<SimpleStatisticsModel>>,
        IRequestHandler<StatisticsTopIrregularitiesQuery, ApiListResponse<SimpleStatisticsModel>>,
        IRequestHandler<StatisticsOptionsQuery, StatisticsOptionsModel>
    {
        private readonly VoteMonitorContext _context;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public StatisticsQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<StatisticsOptionsModel> Handle(StatisticsOptionsQuery message, CancellationToken token)
        {
            var queryBuilder = new StatisticsQueryBuilder
            {
                Query = $@"SELECT O.Text AS Label, O.Id AS Code, OQ.Flagged AS Flagged, COUNT(*) as Value
                  FROM Answers AS A
                  INNER JOIN OptionsToQuestions AS OQ ON OQ.Id = A.IdOptionToQuestion
                  INNER JOIN Options AS O ON O.Id = OQ.IdOption
                  INNER JOIN Observers Obs ON Obs.Id = A.IdObserver
                  INNER JOIN Ngos N ON O.IdNgo = N.Id
                  WHERE OQ.Id = {message.QuestionId} AND N.IsActive =1 AND Obs.IsTestObserver = 0",
                CacheKey = $"StatisticiOptiuni-{message.QuestionId}"
            };

            queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
            queryBuilder.Append("GROUP BY O.Text, O.Id, OQ.Flagged");

            return await _cacheService.GetOrSaveDataInCacheAsync(queryBuilder.CacheKey,
                async () =>
                {
                    var records = await _context.OptionsStatistics
                        .FromSqlRaw(queryBuilder.Query)
                        .ToListAsync(cancellationToken: token);

                    return new StatisticsOptionsModel
                    {
                        QuestionId = message.QuestionId,
                        Options = records.Select(s => new OptionStatisticsModel
                        {
                            OptionId = s.Code,
                            Label = s.Label,
                            Value = s.Value.ToString(),
                            IsFlagged = s.Flagged
                        })
                        .ToList(),
                        Total = records.Sum(s => s.Value)
                    };
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                }
            );
        }

        public async Task<ApiListResponse<SimpleStatisticsModel>> Handle(StatisticsObserverNumberQuery message, CancellationToken token)
        {
            var queryBuilder = new StatisticsQueryBuilder
            {
                Query = @"SELECT COUNT(distinct a.IdObserver) as [Value], CountyCode as Label
                          FROM Answers a (nolock) 
                          INNER JOIN Observers o on a.IdObserver = o.Id
                          INNER JOIN Ngos N ON O.IdNgo = N.Id
                          WHERE N.IsActive = 1 AND o.IsTestObserver = 0",
                CacheKey = "StatisticiObservatori"
            };

            queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
            queryBuilder.Append("group by CountyCode order by [Value] desc");

            var records = await _cacheService.GetOrSaveDataInCacheAsync(
                queryBuilder.CacheKey,
                async () => await _context.SimpleStatistics
                    .FromSqlRaw(queryBuilder.Query)
                    .ToListAsync(cancellationToken: token),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                }
            );

            var pagedList = records.Paginate(message.Page, message.PageSize);

            return new ApiListResponse<SimpleStatisticsModel>
            {
                Data = pagedList.Select(x => _mapper.Map<SimpleStatisticsModel>(x)).ToList(),
                Page = message.Page,
                PageSize = message.PageSize,
                TotalItems = records.Count
            };
        }

        public async Task<ApiListResponse<SimpleStatisticsModel>> Handle(StatisticsTopIrregularitiesQuery message, CancellationToken token)
        {
            return message.GroupType == StatisticsGroupingTypes.County
                ? await GetCountyIrregularities(message, token)
                : await GetPollingStationIrregularities(message, token);
        }

        private async Task<ApiListResponse<SimpleStatisticsModel>> GetCountyIrregularities(StatisticsTopIrregularitiesQuery message, CancellationToken token)
        {
            var queryBuilder = new StatisticsQueryBuilder
            {
                Query = @"SELECT R.CountyCode AS Label, COUNT(*) as Value
                  FROM Answers AS R 
                  INNER JOIN OptionsToQuestions AS RD ON RD.Id = R.IdOptionToQuestion
                  INNER JOIN Observers O ON O.Id = R.IdObserver
                  INNER JOIN Questions I ON I.Id = RD.IdQuestion
                  INNER JOIN Ngos N ON O.IdNgo = N.Id
                  INNER JOIN FormSections fs on i.IdSection = fs.Id
                  INNER JOIN Forms f on fs.IdForm = f.Id
                  WHERE RD.Flagged = 1 AND N.IsActive = 1 AND O.IsTestObserver = 0",
                CacheKey = "StatisticiJudete"
            };

            queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
            queryBuilder.AndFormCodeFilter(message.FormCode);
            queryBuilder.Append("GROUP BY R.CountyCode ORDER BY Value DESC");

            var records = await _cacheService.GetOrSaveDataInCacheAsync(queryBuilder.CacheKey,
                async () => await _context.SimpleStatistics
                    .FromSqlRaw(queryBuilder.Query)
                    .ToListAsync(cancellationToken: token),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                }
            );

            var pagedList = records.Paginate(message.Page, message.PageSize);

            return new ApiListResponse<SimpleStatisticsModel>
            {
                Data = pagedList.Select(x => _mapper.Map<SimpleStatisticsModel>(x)).ToList(),
                Page = message.Page,
                PageSize = message.PageSize,
                TotalItems = records.Count
            };
        }

        private async Task<ApiListResponse<SimpleStatisticsModel>> GetPollingStationIrregularities(StatisticsTopIrregularitiesQuery message, CancellationToken token)
        {
            var queryBuilder = new StatisticsQueryBuilder
            {
                Query = @"SELECT R.CountyCode AS Label, R.PollingStationNumber AS Code, COUNT(*) as Value
                  FROM Answers AS R 
                  INNER JOIN OptionsToQuestions AS RD ON RD.Id = R.IdOptionToQuestion
                  INNER JOIN Observers O ON O.Id = R.IdObserver
                  INNER JOIN Ngos N ON O.IdNgo = N.Id
                  INNER JOIN Questions I ON I.Id = RD.IdQuestion
                  INNER JOIN FormSections fs on i.IdSection = fs.Id
                  INNER JOIN Forms f on fs.IdForm = f.Id
                  WHERE RD.Flagged = 1 AND N.IsActive =1 AND O.IsTestObserver = 0",
                CacheKey = "StatisticiSectii"
            };

            queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
            queryBuilder.AndFormCodeFilter(message.FormCode);
            queryBuilder.Append("GROUP BY R.CountyCode, R.PollingStationNumber");


            return await _cacheService.GetOrSaveDataInCacheAsync($"{queryBuilder.CacheKey}-{message.Page}",
                async () =>
                {
                    var records = await _context.ComposedStatistics
                        .FromSqlRaw(queryBuilder.GetPaginatedQuery(message.Page, message.PageSize))
                        .ToListAsync(cancellationToken: token);

                    return new ApiListResponse<SimpleStatisticsModel>
                    {
                        Data = records.Select(x => _mapper.Map<SimpleStatisticsModel>(x)).ToList(),
                        Page = message.Page,
                        PageSize = message.PageSize,
                        TotalItems = await _context.ComposedStatistics.FromSqlRaw(queryBuilder.Query).CountAsync(cancellationToken: token)
                    };
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                }
            );
        }
    }
}
