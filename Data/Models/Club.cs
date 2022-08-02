﻿using System.ComponentModel.DataAnnotations;

namespace HeroesCup.Data.Models;

public class Club
{
    [Key] public Guid Id { get; set; }

    [Required] public string Name { get; set; }

    public string OrganizationName { get; set; }

    public string OrganizationType { get; set; }

    public string OrganizationNumber { get; set; }

    public string Location { get; set; }

    public string Description { get; set; }

    public int Points { get; set; }

    public ICollection<Hero> Heroes { get; set; }

    public ICollection<Mission> Missions { get; set; }

    public ICollection<ClubImage> ClubImages { get; set; }

    public Guid OwnerId { get; set; }

    public long CreatedOn { get; set; }

    public long UpdatedOn { get; set; }
}