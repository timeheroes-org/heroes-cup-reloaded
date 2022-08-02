using System.Globalization;
using Piranha;

namespace HeroesCup.Web.Common;

public class WebUtils : IWebUtils
{
    private readonly IConfiguration _configuration;

    public WebUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetUrlBase(HttpContext httpContext)
    {
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
    }

    public async Task<CultureInfo> GetCulture(IApi api)
    {
        var defaultLanguage = await api.Languages.GetDefaultAsync();
        var siteCulture = defaultLanguage.Culture;
        var culture = new CultureInfo(siteCulture);
        return culture;
    }
}