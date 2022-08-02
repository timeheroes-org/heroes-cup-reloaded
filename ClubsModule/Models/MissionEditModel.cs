using System.ComponentModel.DataAnnotations;
using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Attributes;

namespace HeroesCup.Web.ClubsModule.Models;

public class MissionEditModel
{
    public Mission Mission { get; set; }

    [MaxSizeFile(2 * 1024 * 1024, ErrorMessage = "MaxSizeErrorMessage")]
    [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".svg" }, ErrorMessage = "AllowedFileExtensionsErrorMessage")]
    public IFormFile Image { get; set; }

    [Required] public string UploadedStartDate { get; set; }

    [Required] public string UploadedEndDate { get; set; }

    public string ImageId { get; set; }

    public string ImageFilename { get; set; }

    public IEnumerable<Club> Clubs { get; set; }

    public Guid ClubId { get; set; }

    public TimeSpan Duration { get; set; }
}