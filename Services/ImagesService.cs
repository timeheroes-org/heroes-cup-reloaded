using HeroesCup.Data.Models;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services;

public class ImagesService : IImagesService
{
    private readonly HeroesCupDbContext dbContext;

    public ImagesService(HeroesCupDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateClubImageAsync(Image image, Club club)
    {
        var oldClubImages = dbContext.ClubImages.Where(ci => ci.ClubId == club.Id)
            .Include(ci => ci.Image);

        if (oldClubImages != null)
            foreach (var clubImage in oldClubImages)
                await DeleteClubImageAsync(clubImage);

        dbContext.Images.Add(image);
        dbContext.ClubImages.Add(new ClubImage
        {
            Club = club,
            Image = image
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteClubImageAsync(ClubImage clubImage, bool commit = false)
    {
        dbContext.Images.Remove(clubImage.Image);

        if (commit) await dbContext.SaveChangesAsync();
    }

    public async Task<ClubImage> GetClubImage(Guid clubId)
    {
        return await dbContext.ClubImages.Where(ci => ci.ClubId == clubId).FirstOrDefaultAsync();
    }

    public async Task<Image> GetImage(Guid id)
    {
        return await dbContext.Images.FirstOrDefaultAsync(i => i.Id == id);
    }

    public string GetImageSource(string contentType, byte[] bytes)
    {
        var bytesTobase64 = Convert.ToBase64String(bytes);
        return string.Format("data:{0};base64,{1}", contentType, bytesTobase64);
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
        var oldMissionImages = dbContext.MissionImages.Where(mi => mi.MissionId == mission.Id)
            .Include(mi => mi.Image);

        if (oldMissionImages != null)
            foreach (var missionImage in oldMissionImages)
                await DeleteMissionImageAsync(missionImage);

        dbContext.Images.Add(image);
        dbContext.MissionImages.Add(new MissionImage
        {
            Mission = mission,
            Image = image
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteMissionImageAsync(MissionImage missionImage, bool commit = false)
    {
        dbContext.Images.Remove(missionImage.Image);

        if (commit) await dbContext.SaveChangesAsync();
    }

    public async Task<MissionImage> GetMissionImage(Guid missionId)
    {
        return await dbContext.MissionImages
            .Where(mi => mi.MissionId == missionId)
            .Include(mi => mi.Image)
            .FirstOrDefaultAsync();
    }

    public async Task CreateStoryImagesAsync(IEnumerable<Image> images, Story story)
    {
        var oldStoryImages = dbContext.StoryImages.Where(si => si.StoryId == story.Id)
            .Include(si => si.Image);

        if (oldStoryImages != null)
            foreach (var storyImage in oldStoryImages)
                await DeleteStoryImageAsync(storyImage);

        foreach (var image in images)
        {
            dbContext.Images.Add(image);
            dbContext.StoryImages.Add(new StoryImage
            {
                Story = story,
                Image = image
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteStoryImageAsync(StoryImage storyImage, bool commit = false)
    {
        dbContext.Images.Remove(storyImage.Image);

        if (commit) await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<StoryImage>> GetStoryImages(Guid storyId)
    {
        return await dbContext.StoryImages.Where(si => si.StoryId == storyId).ToListAsync();
    }

    public async Task CreateMissionIdeaImageAsync(Image image, MissionIdea missionIdea)
    {
        var oldMissionIdeaImages = dbContext.MissionIdeaImages
            .Where(mi => mi.MissionIdeaId == missionIdea.Id)
            .Include(mi => mi.Image);

        if (oldMissionIdeaImages != null)
            foreach (var missionIdeaImage in oldMissionIdeaImages)
                await DeleteMissionIdeaImageAsync(missionIdeaImage);

        dbContext.Images.Add(image);
        dbContext.MissionIdeaImages.Add(new MissionIdeaImage
        {
            MissionIdea = missionIdea,
            Image = image
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteMissionIdeaImageAsync(MissionIdeaImage missionIdeaImage, bool commit = false)
    {
        dbContext.Images.Remove(missionIdeaImage.Image);

        if (commit) await dbContext.SaveChangesAsync();
    }

    public async Task<MissionIdeaImage> GetMissionIdeaImageAsync(Guid missionIdeaId)
    {
        return await dbContext.MissionIdeaImages
            .Where(mi => mi.MissionIdeaId == missionIdeaId)
            .Include(mi => mi.Image)
            .FirstOrDefaultAsync();
    }

    public string GetImageFilename(Image image)
    {
        if (image == null) return null;

        return $"{image.Filename}.{image.Extension}";
    }

    public async Task<string> GetImageFilename(Guid id)
    {
        if (id == Guid.Empty) return null;

        var image = await GetImage(id);

        return $"{image.Filename}.{image.Extension}";
    }

    public async Task<Image> GetImageByFileName(string filename)
    {
        return await dbContext.Images.FirstOrDefaultAsync(i => i.Filename == filename);
    }

    public Image MapFormFileToImage(IFormFile file)
    {
        var image = new Image();
        image.Id = Guid.NewGuid();
        var bytes = GetByteArrayFromImage(file);
        var filename = GetFilename(file, image.Id);
        var contentType = GetFileContentType(file);
        image.Bytes = bytes;
        image.Filename = filename;
        image.ContentType = contentType;

        return image;
    }

    public string getClubImageId(Guid clubId)
    {
        var clubImage = dbContext.ClubImages.Where(ci => ci.ClubId == clubId).FirstOrDefault();
        if (clubImage == null) return null;

        return clubImage.ImageId.ToString();
    }
}