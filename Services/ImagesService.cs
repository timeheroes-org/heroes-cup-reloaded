using HeroesCup.Data.Models;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services;

public class ImagesService : IImagesService
{
    private readonly HeroesCupDbContext _dbContext;

    public ImagesService(HeroesCupDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task CreateClubImageAsync(Image image, Club club)
    {
        var oldClubImages = _dbContext.ClubImages.Where(ci => ci.ClubId == club.Id)
            .Include(ci => ci.Image);

        foreach (var clubImage in oldClubImages)
            await DeleteClubImageAsync(clubImage);

        _dbContext.Images.Add(image);
        _dbContext.ClubImages.Add(new ClubImage
        {
            Club = club,
            Image = image
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteClubImageAsync(ClubImage clubImage, bool commit = false)
    {
        _dbContext.Images.Remove(clubImage.Image);

        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task<ClubImage> GetClubImage(Guid clubId)
    {
        return await _dbContext.ClubImages
            .Include(c=>c.Image)
            .Where(ci => ci.ClubId == clubId).FirstOrDefaultAsync();
    }

    public async Task<Image> GetImage(Guid id)
    {
        return await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == id);
    }


    public byte[] GetByteArrayFromImage(IFormFile file)
    {
        using (var target = new MemoryStream())
        {
            file.CopyTo(target);
            return target.ToArray();
        }
    }

    public string GetFilename(IFormFile file, Guid imageId)
    {
        var filename = Path.GetFileName(file.FileName);
        var formatIndex = filename.LastIndexOf(".");
        var fileFormat = filename.Substring(formatIndex);
        return $"{imageId}{fileFormat}";
    }

    public string GetFileContentType(IFormFile file)
    {
        return file.ContentType;
    }

    public async Task CreateMissionImageAsync(Image image, Mission mission)
    {
        var oldMissionImages = _dbContext.MissionImages.Where(mi => mi.MissionId == mission.Id)
            .Include(mi => mi.Image);

        if (oldMissionImages != null)
            foreach (var missionImage in oldMissionImages)
                await DeleteMissionImageAsync(missionImage);

        _dbContext.Images.Add(image);
        _dbContext.MissionImages.Add(new MissionImage
        {
            Mission = mission,
            Image = image
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMissionImageAsync(MissionImage missionImage, bool commit = false)
    {
        _dbContext.Images.Remove(missionImage.Image);

        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task<MissionImage> GetMissionImage(Guid missionId)
    {
        return await _dbContext.MissionImages
            .Where(mi => mi.MissionId == missionId)
            .Include(mi => mi.Image)
            .Select(i => new MissionImage
            {
                ImageId = i.ImageId,
                Image = new Image
                {
                    Filename = i.Image.Filename,
                    ContentType = i.Image.ContentType,
                    Id = i.Image.Id
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateStoryImagesAsync(IEnumerable<Image> images, Story story)
    {
        var oldStoryImages = 
                _dbContext.StoryImages
                    .Where(si => si.StoryId == story.Id)
                    .Include(si => si.Image);

        if (oldStoryImages != null)
            foreach (var storyImage in oldStoryImages)
                await DeleteStoryImageAsync(storyImage);

        foreach (var image in images)
        {
            _dbContext.Images.Add(image);
            _dbContext.StoryImages.Add(new StoryImage
            {
                Story = story,
                Image = image
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteStoryImageAsync(StoryImage storyImage, bool commit = false)
    {
        _dbContext.Images.Remove(storyImage.Image);

        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<StoryImage>> GetStoryImages(Guid storyId)
    {
        return await _dbContext.StoryImages.Where(si => si.StoryId == storyId).ToListAsync();
    }

    public async Task CreateMissionIdeaImageAsync(Image image, MissionIdea missionIdea)
    {
        var oldMissionIdeaImages = _dbContext.MissionIdeaImages
            .Where(mi => mi.MissionIdeaId == missionIdea.Id)
            .Include(mi => mi.Image);

        if (oldMissionIdeaImages != null)
            foreach (var missionIdeaImage in oldMissionIdeaImages)
                await DeleteMissionIdeaImageAsync(missionIdeaImage);

        _dbContext.Images.Add(image);
        _dbContext.MissionIdeaImages.Add(new MissionIdeaImage
        {
            MissionIdea = missionIdea,
            Image = image
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMissionIdeaImageAsync(MissionIdeaImage missionIdeaImage, bool commit = false)
    {
        _dbContext.Images.Remove(missionIdeaImage.Image);

        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task<MissionIdeaImage> GetMissionIdeaImageAsync(Guid missionIdeaId)
    {
        return await _dbContext.MissionIdeaImages
            .Where(mi => mi.MissionIdeaId == missionIdeaId)
            .Include(mi => mi.Image)
            .FirstOrDefaultAsync();
    }


    public async Task<string> GetImageFilename(Guid id)
    {
        if (id == Guid.Empty) return null;

        var image = await GetImage(id);

        return image.Filename;
    }

    public async Task<Image> GetImageByFileName(string filename)
    {
        return await _dbContext.Images.FirstOrDefaultAsync(i => i.Filename == filename);
    }

    public Image MapFormFileToImage(IFormFile file)
    {
        var image = new Image();
        image.Id = Guid.NewGuid();
        var filename = GetFilename(file, image.Id);
        var contentType = GetFileContentType(file);
        image.Filename = filename;
        image.ContentType = contentType;

        return image;
    }

    public string getClubImageId(Guid clubId)
    {
        var clubImage = _dbContext.ClubImages.FirstOrDefault(ci => ci.ClubId == clubId);
        if (clubImage == null) return null;

        return clubImage.ImageId.ToString();
    }
}