using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public interface IMetaDataProvider
{
    SocialNetworksMetaData getMetaData(HttpContext httpContext,
        string slug,
        string title,
        string url = null,
        string imageUrl = null,
        string videoUrl = null,
        string videoType = null);
}