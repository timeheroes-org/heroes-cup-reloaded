using System.ComponentModel.DataAnnotations;
using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Attributes;

namespace HeroesCup.Web.ClubsModule.Models
{
    public class MissionIdeaEditModel
    {
        public MissionIdea MissionIdea { get; set; }

        [MaxSizeFile(2 * 1024 * 1024, ErrorMessage = "MaxSizeErrorMessage")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".svg" }, ErrorMessage = "AllowedFileExtensionsErrorMessage")]
        public IFormFile Image { get; set; }

        public string ImageId { get; set; }

        //public string ImageSrc { get; set; }

        public string ImageFilename { get; set; }

        [Required]
        public string UploadedStartDate { get; set; }

        [Required]
        public string UploadedEndDate { get; set; }
    }
}