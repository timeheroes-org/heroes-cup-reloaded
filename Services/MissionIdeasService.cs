using System.Globalization;
using HeroesCup.Data.Models;
using HeroesCup.Web.Exceptions;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services;

public class MissionIdeasService : IMissionIdeasService
{
    private readonly IConfiguration configuration;
    private readonly HeroesCupDbContext dbContext;
    private readonly IImagesService imagesService;

    private readonly string dateTimeFormat;

    public MissionIdeasService(HeroesCupDbContext dbContext, IImagesService imagesService, IConfiguration configuration)
    {
        this.dbContext = dbContext;
        this.imagesService = imagesService;
        this.configuration = configuration;
        dateTimeFormat = this.configuration["DateТimeFormat"];
    }

    public async Task<MissionIdeaListModel> GetMissionIdeasListModelAsync()
    {
        var missionIdeas = new List<MissionIdea>();
        missionIdeas = await dbContext.MissionIdeas.ToListAsync();

        var model = new MissionIdeaListModel
        {
            MissionIdeas = missionIdeas
                .OrderBy(m => m.IsPublished)
                .ThenByDescending(m => m.UpdatedOn)
                .Select(m => new MissionIdeaListItem
                {
                    Id = m.Id,
                    Title = m.Title,
                    IsPublished = m.IsPublished,
                    LastUpdateOn = m.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(dateTimeFormat)
                })
        };

        return model;
    }

    public MissionIdeaEditModel CreateMissionIdeaEditModel()
    {
        var model = new MissionIdeaEditModel
        {
            MissionIdea = new MissionIdea()
        };

        return model;
    }

    public IEnumerable<MissionIdea> GetAllPublishedMissionIdeas()
    {
        var missionIdeas = dbContext.MissionIdeas
            .Where(m => m.IsPublished == true)
            .Include(m => m.MissionIdeaImages)
            .OrderByDescending(mi => mi.StartDate != long.MinValue ? mi.StartDate : mi.CreatedOn);

        return missionIdeas;
    }

    public async Task<MissionIdeaEditModel> GetMissionIdeaEditModelByIdAsync(Guid id)
    {
        MissionIdea missionIdea = null;
        missionIdea = await dbContext.MissionIdeas
            .Include(c => c.MissionIdeaImages)
            .ThenInclude(ci => ci.Image)
            .FirstOrDefaultAsync(c => c.Id == id);

        return await MapMissionIdeaToMissionIdeaEditModel(missionIdea);
    }

    public async Task<MissionIdeaEditModel> GetMissionIdeaEditModelBySlugAsync(string slug)
    {
        MissionIdea missionIdea = null;
        missionIdea = await dbContext.MissionIdeas
            .Include(c => c.MissionIdeaImages)
            .ThenInclude(ci => ci.Image)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        return await MapMissionIdeaToMissionIdeaEditModel(missionIdea);
    }

    public async Task<Guid> SaveMissionIdeaEditModelAsync(MissionIdeaEditModel model)
    {
        var missionIdea = await dbContext.MissionIdeas
            .Include(c => c.MissionIdeaImages)
            .ThenInclude(m => m.Image)
            .FirstOrDefaultAsync(m => m.Id == model.MissionIdea.Id);

        var slug = model.MissionIdea.Title.Trim().ToSlug();
        slug = slug.Unidecode();

        var missionIdeaWithSameTitle = await dbContext.MissionIdeas
            .Where(m => (m.Title == model.MissionIdea.Title || m.Slug == slug) &&
                        m.Id != model.MissionIdea.Id)
            .FirstOrDefaultAsync();

        if (missionIdeaWithSameTitle != null) throw new ExistingItemException();

        if (missionIdea == null)
        {
            missionIdea = new MissionIdea();
            missionIdea.Id = model.MissionIdea.Id != Guid.Empty ? model.MissionIdea.Id : Guid.NewGuid();
            missionIdea.CreatedOn = DateTime.Now.ToUnixMilliseconds();
            dbContext.MissionIdeas.Add(missionIdea);
        }

        missionIdea.Title = model.MissionIdea.Title;
        missionIdea.Slug = slug;
        missionIdea.Organization = model.MissionIdea.Organization;
        missionIdea.Location = model.MissionIdea.Location;
        missionIdea.Content = model.MissionIdea.Content;
        missionIdea.TimeheroesUrl = model.MissionIdea.TimeheroesUrl;
        var dateFormat = configuration["DateFormat"];
        var startDate = DateTime.ParseExact(model.UploadedStartDate, dateFormat, CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(model.UploadedEndDate, dateFormat, CultureInfo.InvariantCulture);
        missionIdea.StartDate = startDate.StartOfTheDay().ToUnixMilliseconds();
        missionIdea.EndDate = endDate.EndOfTheDay().ToUnixMilliseconds();

        // set missionIdea image
        if (model.Image != null)
        {
            var image = imagesService.MapFormFileToImage(model.Image);
            await imagesService.CreateMissionIdeaImageAsync(image, missionIdea);
        }

        missionIdea.UpdatedOn = DateTime.Now.ToUnixMilliseconds();

        await dbContext.SaveChangesAsync();
        return missionIdea.Id;
    }

    public async Task<bool> DeleteMissionIdeaAsync(Guid id)
    {
        var missionIdea = dbContext.MissionIdeas.FirstOrDefault(c => c.Id == id);
        if (missionIdea == null) return false;

        dbContext.MissionIdeas.Remove(missionIdea);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishMissionIdeaAsync(Guid missionIdeaId)
    {
        var missionIdea = await dbContext.MissionIdeas.FirstOrDefaultAsync(m => m.Id == missionIdeaId);
        if (missionIdea == null) return false;

        missionIdea.IsPublished = true;
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnpublishMissionIdeaAsync(Guid missionIdeaId)
    {
        var missionIdea = await dbContext.MissionIdeas.FirstOrDefaultAsync(m => m.Id == missionIdeaId);
        if (missionIdea == null) return false;

        missionIdea.IsPublished = false;
        await dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<MissionIdeaEditModel> MapMissionIdeaToMissionIdeaEditModel(MissionIdea missionIdea)
    {
        if (missionIdea == null) return null;

        var model = CreateMissionIdeaEditModel();
        model.MissionIdea = missionIdea;
        var dateFormat = configuration["DateFormat"];
        model.UploadedStartDate = missionIdea.StartDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);
        model.UploadedEndDate = missionIdea.EndDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);

        if (missionIdea.MissionIdeaImages != null && missionIdea.MissionIdeaImages.Count > 0)
        {
            var missionIdeaImage = await imagesService.GetMissionIdeaImageAsync(missionIdea.Id);
            model.ImageId = missionIdea.MissionIdeaImages != null && missionIdea.MissionIdeaImages.Any()
                ? missionIdea.MissionIdeaImages.FirstOrDefault().ImageId.ToString()
                : null;
            model.ImageFilename = missionIdeaImage.Image.Filename;
        }

        return model;
    }
}