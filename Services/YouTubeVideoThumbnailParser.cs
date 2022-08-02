using Microsoft.Extensions.Configuration;

namespace HeroesCup.Web.Services
{
    public class YouTubeVideoThumbnailParser : IVideoThumbnailParser
    {
        private readonly IConfiguration _configuration;

        public YouTubeVideoThumbnailParser(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string ParseDefaultThumbnailUrl(string embeddedVideoUrl)
        {
            if (string.IsNullOrEmpty(embeddedVideoUrl) || string.IsNullOrWhiteSpace(embeddedVideoUrl))
            {
                return null;
            }

            if (embeddedVideoUrl.IndexOf(this._configuration["YouTubeUrl"]) < 0)
            {
                return null;
            }

            var embedUrlPart = this._configuration["YouTubeEmbedUrlPart"];
            var index = embeddedVideoUrl.IndexOf(embedUrlPart);
            if (index != -1)
            {
                var videoCode = embeddedVideoUrl.Substring(index + embedUrlPart.Length);
                return $"{this._configuration["YouTubeThumbnailUrl"]}/{videoCode}/{this._configuration["YouTubeThumbnailDefaultImageName"]}";
            }

            return null;
        }
    }
}
