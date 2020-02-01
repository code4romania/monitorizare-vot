using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Statistics.Handlers;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Api.Statistics.Queries;

namespace VoteMonitor.Api.Statistics.Controllers
{
    /// <inheritdoc />
    [Route("api/v1/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ApplicationCacheOptions _cacheOptions;
        private readonly IMediator _mediator;
        private int _cacheHours;
        private int _cacheMinutes;
        private int _cacheSeconds;

        public StatisticsController(IMediator mediator, IConfigurationRoot configuration, IOptions<ApplicationCacheOptions> cacheOptions)
        {
            _mediator = mediator;
            _configuration = configuration;
            _cacheOptions = cacheOptions.Value;
            _cacheHours = _cacheOptions.Hours;
            _cacheMinutes = _cacheOptions.Minutes;
            _cacheSeconds = _cacheOptions.Seconds;
        }

        /// <summary>
        /// Returns top counties by observer number
        /// </summary>
        /// <param name="model">Pagination details</param>
        /// <returns></returns>
        [HttpGet]
        [Route("observerNumber")]
        [Authorize("NgoAdmin")]
        public async Task<ApiListResponse<SimpleStatisticsModel>> NumarObservatori(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            return await _mediator.Send(new StatisticsObserversNumberQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                PageSize = model.PageSize,
                Page = model.Page,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
            });
        }

        /// <summary>
        /// Returns top counties or polling stations by number of irregularities
        /// </summary>
        /// <param name="model">  Pagination details (default Page=1, PageSize=20)
        /// Grouping (0 - County | 1 - PollingStation)
        /// FormCode (formeCode for which you want to retrieve statistics, use empty string "" for all forms)
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("irregularities")]
        [Authorize("NgoAdmin")]
        public async Task<ApiListResponse<SimpleStatisticsModel>> Irregularities(SimpleStatisticsFilter model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            if (model.GroupingType == StatisticsGroupingTypes.Sectie)
            {
                model.PageSize = PagingDefaultsConstants.DEFAULT_PAGE_SIZE;
            }

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = model.GroupingType,
                Formular = model.FormCode,
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
        public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesIrregularities(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Judet,
                Formular = null,
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
            model.PageSize = PagingDefaultsConstants.DEFAULT_PAGE_SIZE;

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Sectie,
                Formular = null,
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
        public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesOpeningIrregularities(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Judet,
                Formular = "A",
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
        public async Task<ApiListResponse<SimpleStatisticsModel>> PollingStationOpeningIrregularities(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
            model.PageSize = PagingDefaultsConstants.DEFAULT_PAGE_SIZE;

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Sectie,
                Formular = "A",
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
        public async Task<ApiListResponse<SimpleStatisticsModel>> CountiesByCountingIrregularities(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Judet,
                Formular = "C",
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
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
        public async Task<ApiListResponse<SimpleStatisticsModel>> PollingStationsByCountingIrregularities(PagingModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
            model.PageSize = PagingDefaultsConstants.DEFAULT_PAGE_SIZE;

            return await _mediator.Send(new StatisticiTopSesizariQuery
            {
                IdONG = idONG,
                Organizator = organizator,
                Grupare = StatisticsGroupingTypes.Sectie,
                Formular = "C",
                Page = model.Page,
                PageSize = model.PageSize,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
            });
        }

        /// <summary>
        /// Returns the number of answers given by the observers
        /// Grouped by the options of the given question
        /// </summary>
        /// <param name="model">Id - the questionId to retrieve statistics for</param>
        /// <returns></returns>
        [HttpGet]
        [Route("RaspunsuriNumarareOptiuni")]
        [Authorize("NgoAdmin")]
        public async Task<StatisticsOptionsModel> CountAnswersForQuestion(OptionsFilterModel model)
        {
            var idONG = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));

            return await _mediator.Send(new StatisticsOptionsQuery
            {
                QuestionId = model.QuestionId,
                Organizator = organizator,
                IdONG = idONG,
                CacheHours = _cacheHours,
                CacheMinutes = _cacheMinutes,
                CacheSeconds = _cacheSeconds
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("mini/answers")]
        public async Task<SimpleStatisticsModel> Answers()
        {
            return await _mediator.Send(new AnswersRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/stations")]
        public async Task<SimpleStatisticsModel> StationsVisited()
        {
            return await _mediator.Send(new StationsVisitedRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/counties")]
        public async Task<SimpleStatisticsModel> Counties()
        {
            return await _mediator.Send(new CountiesVisitedRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/notes")]
        public async Task<SimpleStatisticsModel> Notes()
        {
            return await _mediator.Send(new NotesUploadedRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/loggedinobservers")]
        public async Task<SimpleStatisticsModel> LoggedInObservers()
        {
            return await _mediator.Send(new LoggedInObserversRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/flaggedanswers")]
        public async Task<SimpleStatisticsModel> FlaggedAnswers()
        {
            return await _mediator.Send(new FlaggedAnswersRequest());
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("mini/all")]
        public async Task<List<SimpleStatisticsModel>> All()
        {
            var list = new List<SimpleStatisticsModel>
            {
                await _mediator.Send(new AnswersRequest()),
                await _mediator.Send(new StationsVisitedRequest()),
                await _mediator.Send(new CountiesVisitedRequest()),
                await _mediator.Send(new NotesUploadedRequest()),
                await _mediator.Send(new LoggedInObserversRequest()),
                await _mediator.Send(new FlaggedAnswersRequest())
            };

            return list;
        }

    }
}
