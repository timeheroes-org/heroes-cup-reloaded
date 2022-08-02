﻿using HeroesCup.Web.ClubsModule.Attributes;
using HeroesCup.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace HeroesCup.Web.ClubsModule.Models
{
    public class StoryEditModel
    {
        public Story Story { get; set; }

        public IEnumerable<Mission> Missions { get; set; }

        [MaxSizeFile(2 * 1024 * 1024, ErrorMessage = "MaxSizeErrorMessage")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".svg" }, ErrorMessage = "AllowedFileExtensionsErrorMessage")]
        public ICollection<IFormFile> UploadedImages { get; set; }

        public ICollection<string> ImageIds { get; set; }

        public ICollection<Hero> Heroes { get; set; }

        public ICollection<Guid> HeroesIds { get; set; }
    }
}