using HeroesCup.Web.Common;
using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public class MetaDataProvider : IMetaDataProvider
{
    private readonly IConfiguration configuration;
    private readonly IWebUtils webUtils;

    public MetaDataProvider(IConfiguration configuration, IWebUtils webUtils)
    {
        this.configuration = configuration;
        this.webUtils = webUtils;
    }

    public SocialNetworksMetaData getMetaData(HttpContext httpContext, string slug, string title, string url = null,
        string imageUrl = null, string videoUrl = null, string videoType = null)
    {
        var currentUrlBase = webUtils.GetUrlBase(httpContext);
        var image = imageUrl != null ? imageUrl : $"{currentUrlBase}/{configuration["FacebookDefaultImageUrl"]}";
        var validUrl = url != null ? url : $"{currentUrlBase}/{slug}";
        return new SocialNetworksMetaData
        {
            FacebookAppId = configuration["FacebookAppId"],
            Url = validUrl,
            Description = title,
            Title = title,
            Type = configuration["FacebookArticleType"],
            Image = image,
            ImageWidth = configuration["FacebookDefaultImageWidth"],
            ImageHeight = configuration["FacebookDefaultImageHeight"],
            VideoUrl = videoUrl != null ? videoUrl : null,
            VideoType = videoType != null ? videoType : null,
            UrlBase = currentUrlBase,
            TwitterUrl = string.Format(configuration["TwitterShareUrl"], validUrl, title)
        };
    }
}