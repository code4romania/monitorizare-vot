using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Api.Statistics.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Statistics.Handlers;

public class StatisticsQueryHandler :
    IRequestHandler<StatisticsObserverNumberQuery, ApiListResponse<SimpleStatisticsModel>>,
    IRequestHandler<StatisticsTopIrregularitiesQuery, ApiListResponse<SimpleStatisticsModel>>,
    IRequestHandler<StatisticsOptionsQuery, StatisticsOptionsModel>
{
    private readonly VoteMonitorContext _context;
    private readonly ICacheService _cacheService;

    public StatisticsQueryHandler(VoteMonitorContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<StatisticsOptionsModel> Handle(StatisticsOptionsQuery message, CancellationToken token)
    {
        var queryBuilder = new StatisticsQueryBuilder
        {
            Query = $@"SELECT O.""{nameof(Option.Text)}"" AS Label, O.""Id"" AS Code, OQ.""{nameof(OptionToQuestion.Flagged)}"" AS Flagged, COUNT(*) as Value
                  FROM public.""Answers"" AS A
                  INNER JOIN public.""OptionsToQuestions"" AS OQ ON OQ.""Id"" = A.""IdOptionToQuestion""
                  INNER JOIN public.""Options"" AS O ON O.""Id"" = OQ.""IdOption""
                  INNER JOIN public.""Observers"" Obs ON Obs.""Id"" = A.""IdObserver""
                  INNER JOIN public.""Ngos"" N ON Obs.""IdNgo"" = N.""Id""
                  WHERE OQ.""Id"" = {message.QuestionId} AND N.""IsActive"" = true AND Obs.""IsTestObserver"" = false",
            CacheKey = $"StatisticiOptiuni-{message.QuestionId}"
        };

        queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
        queryBuilder.Append(@"GROUP BY O.""Text"", O.""Id"", OQ.""Flagged""");

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
            Query = @$"SELECT COUNT(distinct a.""IdObserver"") as Value, C.""Name"" as Label
                          FROM public.""Answers"" A
                          INNER JOIN public.""Observers"" o on A.""IdObserver"" = o.""Id""
                          INNER JOIN public.""Ngos"" N ON O.""IdNgo"" = N.""Id""

                          INNER JOIN public.""PollingStations"" PS ON A.""{nameof(Answer.IdPollingStation)}"" = PS.""Id""
                          INNER JOIN public.""Municipalities"" M ON PS.""{nameof(PollingStation.MunicipalityId)}"" = M.""Id""
                          INNER JOIN public.""Counties"" C ON M.""{nameof(Municipality.CountyId)}"" = C.""Id""

                          WHERE N.""IsActive"" = true AND o.""IsTestObserver"" = false",
            CacheKey = "StatisticiObservatori"
        };

        queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
        queryBuilder.Append(@"group by C.""Name"" order by Value desc");

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
            Data = pagedList.Select(x => new SimpleStatisticsModel()
            {
                Label = x.Label,
                Value = x.Value.ToString()
            }).ToList(),
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
            Query = @$"SELECT C.""Name"" AS Label, COUNT(*) as Value
                  FROM public.""Answers"" AS A 
                  INNER JOIN public.""OptionsToQuestions"" AS RD ON RD.""Id"" = A.""IdOptionToQuestion""
                  INNER JOIN public.""Observers"" O ON O.""Id"" = A.""IdObserver""
                  INNER JOIN public.""Questions"" I ON I.""Id"" = RD.""IdQuestion""
                  INNER JOIN public.""Ngos"" N ON O.""IdNgo"" = N.""Id""
                  INNER JOIN public.""FormSections"" fs on i.""IdSection"" = fs.""Id""
                  INNER JOIN public.""Forms"" f on fs.""IdForm"" = f.""Id""
                  
                  INNER JOIN public.""PollingStations"" PS ON A.""{nameof(Answer.IdPollingStation)}"" = PS.""Id""
                  INNER JOIN public.""Municipalities"" M ON PS.""{nameof(PollingStation.MunicipalityId)}"" = M.""Id""
                  INNER JOIN public.""Counties"" C ON M.""{nameof(Municipality.CountyId)}"" = C.""Id""
                 
                  WHERE RD.""Flagged"" = true AND N.""IsActive"" = true AND O.""IsTestObserver"" = false",
            CacheKey = "StatisticiJudete"
        };

        queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
        queryBuilder.AndFormCodeFilter(message.FormCode);
        queryBuilder.Append(@"GROUP BY C.""Name"" ORDER BY Value DESC");

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
            Data = pagedList.Select(x => new SimpleStatisticsModel()
            {
                Label = x.Label,
                Value = x.Value.ToString()
            }).ToList(),
            Page = message.Page,
            PageSize = message.PageSize,
            TotalItems = records.Count
        };
    }

    private async Task<ApiListResponse<SimpleStatisticsModel>> GetPollingStationIrregularities(StatisticsTopIrregularitiesQuery message, CancellationToken token)
    {
        var queryBuilder = new StatisticsQueryBuilder
        {
            Query = @$"SELECT C.""Name"" || ' ' || M.""Name"" || ' ' || A.""PollingStationNumber"" AS Label, A.""PollingStationNumber"" AS Code, COUNT(*) as Value
                  FROM public.""Answers"" AS A
                  INNER JOIN public.""OptionsToQuestions"" AS RD ON RD.""Id"" = A.""IdOptionToQuestion""
                  INNER JOIN public.""Observers"" O ON O.""Id"" = A.""IdObserver""
                  INNER JOIN public.""Ngos"" N ON O.""IdNgo"" = N.""Id""
                  INNER JOIN public.""Questions"" I ON I.""Id"" = RD.""IdQuestion""
                  INNER JOIN public.""FormSections"" fs on i.""IdSection"" = fs.""Id""
                  INNER JOIN public.""Forms"" f on fs.""IdForm"" = f.""Id""
                  
                  INNER JOIN public.""PollingStations"" PS ON A.""{nameof(Answer.IdPollingStation)}"" = PS.""Id""
                  INNER JOIN public.""Municipalities"" M ON PS.""{nameof(PollingStation.MunicipalityId)}"" = M.""Id""
                  INNER JOIN public.""Counties"" C ON M.""{nameof(Municipality.CountyId)}"" = C.""Id""
                  
                  WHERE RD.""Flagged"" = true AND N.""IsActive"" = true AND O.""IsTestObserver"" = false",
            CacheKey = "StatisticiSectii"
        };

        queryBuilder.AndOngFilter(message.IsOrganizer, message.NgoId);
        queryBuilder.AndFormCodeFilter(message.FormCode);
        queryBuilder.Append(@"GROUP BY C.""Name"", M.""Name"", A.""PollingStationNumber""");

        return await _cacheService.GetOrSaveDataInCacheAsync($"{queryBuilder.CacheKey}-{message.Page}",
            async () =>
            {
                var records = await _context.ComposedStatistics
                    .FromSqlRaw(queryBuilder.GetPaginatedQuery(message.Page, message.PageSize))
                    .ToListAsync(cancellationToken: token);

                return new ApiListResponse<SimpleStatisticsModel>
                {
                    Data = records.Select(x => new SimpleStatisticsModel()
                    {
                        Label = x.Label,
                        Value = x.Value.ToString()
                    }).ToList(),
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
