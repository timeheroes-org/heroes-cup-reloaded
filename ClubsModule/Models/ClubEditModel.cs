﻿using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Attributes;

namespace HeroesCup.Web.ClubsModule.Models
{
    public class ClubEditModel
    {
        public Club Club { get; set; }

        public IEnumerable<Hero> Coordinators { get; set; }

        public IEnumerable<Guid> CoordinatorsIds { get; set; }

        [MaxSizeFile(2 * 1024 * 1024, ErrorMessage = "MaxSizeErrorMessage")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".svg" }, ErrorMessage = "AllowedFileExtensionsErrorMessage")]
        public IFormFile UploadedLogo { get; set; }

        public string ClubImageId { get; set; }

        public IEnumerable<Hero> Heroes { get; set; }

        public IEnumerable<Guid> HeroesIds { get; set; }

        public IEnumerable<Mission> Missions { get; set; }

        public IEnumerable<Guid> MissionsIds { get; set; }
    }
}