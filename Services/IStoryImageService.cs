﻿using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services;

public interface IStoryImageService
{
    Task CreateStoryImagesAsync(IEnumerable<Image> images, Story story);

    Task DeleteStoryImageAsync(StoryImage storyImage, bool commit = false);

    Task<IEnumerable<StoryImage>> GetStoryImages(Guid storyId);
}