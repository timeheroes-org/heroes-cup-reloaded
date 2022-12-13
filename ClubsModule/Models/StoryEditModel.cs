using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Attributes;

namespace HeroesCup.Web.ClubsModule.Models;

public class StoryEditModel
{
    public Story Story { get; set; }

    public IEnumerable<Mission> Missions { get; set; }

    [MaxSizeFile(2 * 1024 * 1024, ErrorMessage = "MaxSizeErrorMessage")]
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".svg" }, ErrorMessage = "AllowedFileExtensionsErrorMessage")]
    public ICollection<IFormFile> UploadedImages { get; set; }

    public ICollection<string> ImageFileNames { get; set; }

    public ICollection<Hero> Heroes { get; set; }

    public ICollection<Guid> HeroesIds { get; set; }
}