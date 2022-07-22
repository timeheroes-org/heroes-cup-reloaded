using HeroesCup.Models;
using HeroesCup.Web.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;
using System;
using System.Threading.Tasks;

namespace HeroesCup.Controllers
{
    public class CmsController : Controller
    {
        private readonly IApi _api;
        private readonly IModelLoader _loader;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IStatisticsService _statisticsService;
        private readonly IMissionsService _missionsService;
        private readonly IMetaDataProvider _metaDataProvider;        

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CmsController(
            IApi api, 
            IModelLoader loader, 
            ILeaderboardService leaderboardService, 
            IStatisticsService statisticsService,
            IMissionsService missionsService,
            IMetaDataProvider metaDataProvider)
        {
            this._api = api;
            this._loader = loader;
            this._leaderboardService = leaderboardService;
            this._statisticsService = statisticsService;
            this._missionsService = missionsService;
            this._metaDataProvider = metaDataProvider;
        }

        /// <summary>
        /// Gets the blog archive with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category</param>
        /// <param name="tag">The optional tag</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("archive")]
        public async Task<IActionResult> Archive(Guid id, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null, bool draft = false)
        {
            var model = await this._loader.GetPageAsync<BlogArchive>(id, HttpContext.User, draft);
            model.Archive = await this._api.Archives.GetByIdAsync(id, page, category, tag, year, month);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("page")]
        public async Task<IActionResult> Page(Guid id, bool draft = false)
        {
            var model = await this._loader.GetPageAsync<StandardPage>(id, HttpContext.User, draft);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique post id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("post")]
        public async Task<IActionResult> Post(Guid id, bool draft = false)
        {
            var model = await this._loader.GetPostAsync<BlogPost>(id, HttpContext.User, draft);

            return View(model);
        }

        /// <summary>
        /// Gets the startpage with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("/")]
        [Route("/home")]
        public async Task<IActionResult> Start(Guid id, String selectedSchoolYear = null, bool draft = false)
        {
            var model = await this._loader.GetPageAsync<StartPage>(id, HttpContext.User, draft);

            // Leaderboard
            model.SchoolYears = this._leaderboardService.GetSchoolYears();
            if (selectedSchoolYear != null)
            {
                model.SelectedSchoolYear = selectedSchoolYear;
            }
            else
            {
                model.SelectedSchoolYear = this._leaderboardService.GetLatestSchoolYear();
            }

            var clubsListModel = await this._leaderboardService.GetClubsBySchoolYearAsync(model.SelectedSchoolYear);
            model.Clubs = clubsListModel;

            // Statistics
            model.MissionsCount = this._statisticsService.GetAllMissionsCount();
            model.ClubsCount = this._statisticsService.GetAllClubsCount();
            model.HeroesCount = this._statisticsService.GetAllHeroesCount();
            model.HoursCount = this._statisticsService.GetAllHoursCount();

            model.Missions = await this._missionsService.GetPinnedMissionViewModels();
            model.SocialNetworksMetaData = this._metaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);            

            return View(model);
        }

        /// <summary>
        /// Gets the landing page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("/landing")]
        public async Task<IActionResult> LandingPage(Guid id, bool draft = false)
        {
            var model = await this._loader.GetPageAsync<LandingPage>(id, HttpContext.User, draft);
            model.SocialNetworksMetaData = this._metaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);

            return View(model);
        }

        /// <summary>
        /// Gets the about page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("/about")]
        public async Task<IActionResult> AboutPage(Guid id, bool draft = false)
        {
            var model = await this._loader.GetPageAsync<AboutPage>(id, HttpContext.User, draft);
            model.SocialNetworksMetaData = this._metaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);

            return View(model);
        }
    }
}