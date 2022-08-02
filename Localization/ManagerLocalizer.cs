using Microsoft.Extensions.Localization;

namespace HeroesCup.Localization;

public class ManagerLocalizer
{
    public IStringLocalizer<Club> Club { get; private set; }

    public IStringLocalizer<Hero> Hero { get; private set; }

    public IStringLocalizer<Mission> Mission { get; private set; }

    public IStringLocalizer<Story> Story { get; private set; }

    public IStringLocalizer<General> General { get; private set; }

    public IStringLocalizer<MissionIdea> MissionIdea { get; private set; }

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
}