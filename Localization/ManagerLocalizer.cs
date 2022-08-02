using Microsoft.Extensions.Localization;

namespace HeroesCup.Localization;

public class ManagerLocalizer
{
    public ManagerLocalizer(IStringLocalizer<Club> club,
        IStringLocalizer<Hero> hero,
        IStringLocalizer<Mission> mission,
        IStringLocalizer<Story> story,
        IStringLocalizer<General> general,
        IStringLocalizer<MissionIdea> missionIdea)
    {
        Club = club;
        Hero = hero;
        Mission = mission;
        Story = story;
        General = general;
        MissionIdea = missionIdea;
    }

    public IStringLocalizer<Club> Club { get; }

    public IStringLocalizer<Hero> Hero { get; }

    public IStringLocalizer<Mission> Mission { get; }

    public IStringLocalizer<Story> Story { get; }

    public IStringLocalizer<General> General { get; }

    public IStringLocalizer<MissionIdea> MissionIdea { get; }
}