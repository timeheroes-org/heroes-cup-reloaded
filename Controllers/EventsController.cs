﻿using HeroesCup.Web.Common;
using HeroesCup.Web.Models;
using HeroesCup.Web.Models.Events;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Piranha;
using Piranha.AspNetCore.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeroesCup.Web.Controllers
{
    public class EventsController : Controller
    {
        private const string PageCountKey = "pageCount";
        private readonly IApi _api;
        private readonly IModelLoader _loader;
        private readonly IConfiguration _configuration;
        private readonly IWebUtils _webUtils;
        private readonly int _eventsCount;
        private readonly IMetaDataProvider _metaDataProvider;

        public EventsController(IApi api, IModelLoader loader, IConfiguration configuration, IWebUtils webUtils, IMetaDataProvider metaDataProvider)
        {
            this._api = api;
            this._loader = loader;
            this._configuration = configuration;
            int.TryParse(this._configuration["EventsCount"], out _eventsCount);
            this._webUtils = webUtils;
            this._metaDataProvider = metaDataProvider;
        }

        // <summary>
        /// Gets the events blog archive with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="loadRequest">The optional load more events</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category</param>
        /// <param name="tag">The optional tag</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("events")]
        public async Task<IActionResult> EventsArchive(Guid id, bool loadRequest, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null, bool draft = false)
        {
            var model = await _loader.GetPageAsync<EventsArchive>(id, HttpContext.User, draft);
            int? currentPageCount = null;

            if (loadRequest == true)
            {
                currentPageCount = HttpContext.Session.GetInt32(PageCountKey);
                if (currentPageCount == null)
                {
                    currentPageCount = 2;
                    HttpContext.Session.SetInt32(PageCountKey, (int)currentPageCount);
                }
                else
                {
                    HttpContext.Session.SetInt32(PageCountKey, (int)(currentPageCount += 1));
                }
            }
            else
            {
                currentPageCount = 1;
                HttpContext.Session.SetInt32(PageCountKey, (int)currentPageCount);
            }

            var eventsArchive = await _api.Archives.GetByIdAsync<EventPost>(id, page, category, tag, year, month);
            var posts = eventsArchive.Posts.OrderByDescending(p => p.Published.Value).Take((int)currentPageCount * _eventsCount).ToList();

            model.Archive = eventsArchive;
            model.Archive.Posts = posts;
            model.SocialNetworksMetaData = this._metaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);

            return View(model);
        }

        /// <summary>
        /// Gets the resource with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("event")]
        public async Task<IActionResult> EventPost(Guid id, bool draft = false)
        {
            var model = await _loader.GetPostAsync<EventPost>(id, HttpContext.User, draft);
            var pages = await _api.Pages.GetAllAsync();
            var eventsArchive = pages.FirstOrDefault(p => p.TypeId == "EventsArchive");
            if (eventsArchive != null)
            {
                var eventsArchiveId = eventsArchive.Id;
                var eventsPosts = await _api.Posts.GetAllAsync<EventPost>(eventsArchiveId);
                int othersCount = 0;
                int.TryParse(_configuration["EventsDetailsOthersCount"], out othersCount);
                model.OtherEvents = eventsPosts.Where(r => r.Id != model.Id).Take(othersCount).ToList();
                var currentUrlBase = _webUtils.GetUrlBase(HttpContext);
                model.CurrentUrlBase = currentUrlBase;
                model.SiteCulture = await _webUtils.GetCulture(this._api);
                var image = model.Hero != null && model.Hero.PrimaryImage.HasValue ? $"{currentUrlBase}{model.Hero.PrimaryImage.Media.PublicUrl.TrimStart(new char[] { '~' })}" : $"{currentUrlBase}/{this._configuration["FacebookDefaultImageUrl"]}";
                var url = $"{currentUrlBase}/{model.Category.Title}/{model.Slug}";
                model.SocialNetworksMetaData = this._metaDataProvider.getMetaData(HttpContext, model.Title, model.Title, url, image);
            }

            return View(model);
        }
    }
}