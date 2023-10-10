using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Api.Statistics.Queries;

namespace VoteMonitor.Api.Statistics.Controllers;

/// <inheritdoc />
[Route("api/v1/statistics")]
public class StatisticsController : Controller
{
    private readonly ApplicationCacheOptions _cacheOptions;
    private readonly IMediator _mediator;
    private readonly int _defaultIdOng;
    private readonly bool _defaultOrganizator;

    public StatisticsController(IMediator mediator, IConfiguration configuration, IOptions<ApplicationCacheOptions> cacheOptions)
    {
        _mediator = mediator;
        _cacheOptions = cacheOptions.Value;
        _defaultIdOng = configuration.GetValue<int>("DefaultIdOng");
        _defaultOrganizator = configuration.GetValue<bool>("DefaultOrganizator");
    }

    private int NgoId => this.GetIdOngOrDefault(_defaultIdOng);
    private bool IsOrganizer => this.GetOrganizatorOrDefault(_defaultOrganizator);

    /// <summary>
    /// Returns top counties by observer number
    /// </summary>
    /// <param name="model">Pagination details</param>
    /// <returns></returns>
    [HttpGet]
    [Route("observerNumber")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> ObserverNumber([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsObserverNumberQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            PageSize = model.PageSize,
            Page = model.Page,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns top counties or polling stations by number of irregularities
    /// </summary>
    /// <param name="model">  Pagination details (default Page=1, PageSize=20)
    /// GroupingType (0 - County | 1 - PollingStation)
    /// FormCode (formCode for which you want to retrieve statistics, use empty string "" for all forms)
    /// </param>
    /// <returns></returns>
    [HttpGet]
    [Route("irregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> Irregularities([FromQuery] FilterStatisticsModel model)
    {
        if (model.GroupingType == StatisticsGroupingTypes.PollingStation)
        {
            model.PageSize = PagingDefaultsConstants.DEFAULT_PAGE_SIZE;
        }

        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = model.GroupingType,
            FormCode = model.FormCode,
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns a top of counties ordered by the number of irregularities
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("countiesIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesIrregularities([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.County,
            FormCode = null,
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns a top of polling stations ordered by the number of irregularities
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("pollingStationIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> PollingStationIrregularities(PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.PollingStation,
            FormCode = null,
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>   
    /// Returns top of counties by number of irregularities on polling station opening
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("countiesOpeningIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesOpeningIrregularities([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.County,
            FormCode = "A",
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns top of polling stations by the number of irregularities on polling station opening
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("pollingStationOpeningIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> PollingStationsOpeningIrregularities([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.PollingStation,
            FormCode = "A",
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns the top of counties ordered by counting irregularities
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("countiesByCountingIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesByCountingIrregularities([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.County,
            FormCode = "C",
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns the top of polling stations ordered by counting irregularities
    /// </summary>
    /// <param name="model">Pagination details (default Page=1, PageSize=20)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("pollingStationsByCountingIrregularities")]
    [Authorize("NgoAdmin")]
    public async Task<ApiListResponse<SimpleStatisticsModel>> PollingStationsByCountingIrregularities([FromQuery] PagingModel model)
    {
        return await _mediator.Send(new StatisticsTopIrregularitiesQuery
        {
            NgoId = NgoId,
            IsOrganizer = IsOrganizer,
            GroupType = StatisticsGroupingTypes.PollingStation,
            FormCode = "C",
            Page = model.Page,
            PageSize = model.PageSize,
            CacheHours = _cacheOptions.Hours,
            CacheMinutes = _cacheOptions.Minutes,
            CacheSeconds = _cacheOptions.Seconds
        });
    }

    /// <summary>
    /// Returns the number of answers given by the observers
    /// Grouped by the options of the given question
    /// </summary>
    /// <param name="model">Id - the questionId to retrieve statistics for</param>
    /// <returns></returns>
    [HttpGet]
    [Route("countAnswersByQuestion")]
    [Authorize("NgoAdmin")]
    public async Task<StatisticsOptionsModel> CountAnswersForQuestion([FromQuery] OptionsFilterModel model)
    {
        return await _mediator.Send(new StatisticsOptionsQuery(
             model.QuestionId,
            NgoId,
            IsOrganizer,
            _cacheOptions.Hours,
            _cacheOptions.Minutes,
            _cacheOptions.Seconds));
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("mini/answers")]
    public async Task<SimpleStatisticsModel> Answers()
    {
        return await _mediator.Send(new AnswersQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/stations")]
    public async Task<SimpleStatisticsModel> StationsVisited()
    {
        return await _mediator.Send(new StationsVisitedQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/counties")]
    public async Task<SimpleStatisticsModel> Counties()
    {
        return await _mediator.Send(new CountiesVisitedQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/notes")]
    public async Task<SimpleStatisticsModel> Notes()
    {
        return await _mediator.Send(new NotesUploadedQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/loggedinobservers")]
    public async Task<SimpleStatisticsModel> LoggedInObservers()
    {
        return await _mediator.Send(new LoggedInObserversQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/flaggedanswers")]
    public async Task<SimpleStatisticsModel> FlaggedAnswers()
    {
        return await _mediator.Send(new FlaggedAnswersQuery());
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("mini/all")]
    public async Task<List<SimpleStatisticsModel>> All()
    {
        var list = new List<SimpleStatisticsModel>
        {
            await _mediator.Send(new AnswersQuery()),
            await _mediator.Send(new StationsVisitedQuery()),
            await _mediator.Send(new CountiesVisitedQuery()),
            await _mediator.Send(new NotesUploadedQuery()),
            await _mediator.Send(new LoggedInObserversQuery()),
            await _mediator.Send(new FlaggedAnswersQuery())
        };

        return list;
    }

}
