namespace HeroesCup.Web.Services;

public class YouTubeVideoThumbnailParser : IVideoThumbnailParser
{
    private readonly IConfiguration _configuration;

    public YouTubeVideoThumbnailParser(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string ParseDefaultThumbnailUrl(string embeddedVideoUrl)
    {
        if (string.IsNullOrEmpty(embeddedVideoUrl) || string.IsNullOrWhiteSpace(embeddedVideoUrl)) return null;

        if (embeddedVideoUrl.IndexOf(_configuration["YouTubeUrl"]) < 0) return null;

        var embedUrlPart = _configuration["YouTubeEmbedUrlPart"];
        var index = embeddedVideoUrl.IndexOf(embedUrlPart);
        if (index != -1)
        {
            var videoCode = embeddedVideoUrl.Substring(index + embedUrlPart.Length);
            return
                $"{_configuration["YouTubeThumbnailUrl"]}/{videoCode}/{_configuration["YouTubeThumbnailDefaultImageName"]}";
        }

        return null;
    }
}