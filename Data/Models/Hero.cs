using System.ComponentModel.DataAnnotations;

namespace HeroesCup.Data.Models;

public class Hero
{
    [Key] public Guid Id { get; set; }

    [Required] public string Name { get; set; }

    public Guid ClubId { get; set; }
    public Club Club { get; set; }

    public bool IsCoordinator { get; set; }

    public ICollection<HeroMission> HeroMissions { get; set; }
}