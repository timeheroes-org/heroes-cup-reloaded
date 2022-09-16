﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeroesCup.Data.Models;

public class MissionIdea
{
    public Guid Id { get; set; }

    [Required] public string Title { get; set; }

    public string Slug { get; set; }

    public string Location { get; set; }

    public long StartDate { get; set; }

    public long EndDate { get; set; }

    public string Organization { get; set; }

    public ICollection<MissionIdeaImage> MissionIdeaImages { get; set; }

    public string Content { get; set; }

    public string TimeheroesUrl { get; set; }

    public bool IsPublished { get; set; }

    public long CreatedOn { get; set; }

    public long UpdatedOn { get; set; }

  
}