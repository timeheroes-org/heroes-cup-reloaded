using System.Diagnostics;

namespace HeroesCup.Data.Models;

public class Image
{
    public Guid Id { get; set; }

    public byte[] Bytes { get; set; }

    public string Filename { get; set; }

    public string ContentType { get; set; }

    public String Extension
    {
        get
        {
            return ContentType switch
            {
                "image/png" => "png",
                "application/pdf" => "pdf",
                _ => "jpg"
            };
        }
    }
    public ICollection<ClubImage> ClubImages { get; set; }

    public ICollection<MissionImage> MissionImages { get; set; }

    public ICollection<StoryImage> StoryImages { get; set; }

    public ICollection<MissionIdeaImage> MissionIdeaImages { get; set; }
}