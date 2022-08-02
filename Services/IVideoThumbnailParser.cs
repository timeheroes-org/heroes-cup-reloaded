namespace HeroesCup.Web.Services;

public interface IVideoThumbnailParser
{
    string ParseDefaultThumbnailUrl(string embeddedVideoUrl);
}